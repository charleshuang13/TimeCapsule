using System.Text.Json;

namespace TimeCapsule.Core;

/// <summary>快照仓储：每张快照一个目录，清单本身也加密（文件路径属于隐私）。</summary>
public sealed class SnapshotStore
{
    private readonly Vault _vault;
    private readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public SnapshotStore(Vault vault) => _vault = vault;

    public string DirFor(string id) => Path.Combine(_vault.SnapsDir, id);
    public string FileFor(string id) => Path.Combine(DirFor(id), "manifest.tcm");

    public string NewId()
    {
        string baseId = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string id = baseId;
        int n = 2;
        while (Directory.Exists(DirFor(id))) id = baseId + "-" + n++;
        return id;
    }

    public void Save(SnapshotManifest manifest)
    {
        string dir = DirFor(manifest.Id);
        Directory.CreateDirectory(dir);
        byte[] plain = JsonSerializer.SerializeToUtf8Bytes(manifest, _json);
        byte[] enc = Crypto.EncryptBytes(plain, _vault.ManifestKey);
        string path = FileFor(manifest.Id);
        string tmp = path + ".tmp";
        File.WriteAllBytes(tmp, enc);
        if (File.Exists(path)) File.Delete(path);
        File.Move(tmp, path, true);
    }

    public SnapshotManifest? Load(string id)
    {
        string path = FileFor(id);
        if (!File.Exists(path)) return null;
        try
        {
            byte[] plain = Crypto.DecryptBytes(File.ReadAllBytes(path), _vault.ManifestKey);
            return JsonSerializer.Deserialize<SnapshotManifest>(plain, _json);
        }
        catch (Exception ex)
        {
            _vault.Log.Error("读取快照失败：" + id, ex);
            return null;
        }
    }

    /// <summary>按时间倒序列出所有快照。</summary>
    public List<SnapshotInfo> List()
    {
        var list = new List<SnapshotInfo>();
        if (!Directory.Exists(_vault.SnapsDir)) return list;
        foreach (var dir in Directory.EnumerateDirectories(_vault.SnapsDir))
        {
            string id = Path.GetFileName(dir);
            var m = Load(id);
            if (m == null) continue;
            list.Add(SnapshotInfo.From(m));
        }
        list.Sort((a, b) => string.CompareOrdinal(b.Id, a.Id));
        return list;
    }

    public SnapshotInfo? Latest()
    {
        var all = List();
        return all.Count == 0 ? null : all[0];
    }

    public void Delete(string id)
    {
        string dir = DirFor(id);
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
    }

    public int Count() => Directory.Exists(_vault.SnapsDir)
        ? Directory.EnumerateDirectories(_vault.SnapsDir).Count()
        : 0;

    /// <summary>按保留策略清理旧快照，返回删除张数。</summary>
    public int ApplyRetention()
    {
        var settings = _vault.Settings;
        var all = List();
        if (all.Count <= 1) return 0;

        var toDelete = new List<string>();
        switch (settings.RetainMode)
        {
            case "days":
                var cutoff = DateTimeOffset.Now.AddDays(-Math.Max(1, settings.RetainDays));
                foreach (var s in all)
                {
                    if (s.TakenAt < cutoff) toDelete.Add(s.Id);
                }
                break;

            case "totalSize":
                long limit = Math.Max(0, settings.RetainTotalMB) * 1024L * 1024L;
                if (limit <= 0) return 0;
                long acc = 0;
                foreach (var s in all) // 从新到旧累加
                {
                    acc += s.TotalBytes;
                    if (acc > limit) toDelete.Add(s.Id);
                }
                break;
        }

        // 永远保留最新的那一张
        toDelete.Remove(all[0].Id);
        int n = 0;
        foreach (var id in toDelete)
        {
            try { Delete(id); n++; } catch (Exception ex) { _vault.Log.Warn("删除快照失败 " + id + "：" + ex.Message); }
        }
        if (n > 0)
        {
            int orphans = new BlobStore(_vault).CollectGarbage(this);
            _vault.Log.Info($"保留策略清理：删除 {n} 张快照，回收 {orphans} 个数据块");
        }
        return n;
    }
}
