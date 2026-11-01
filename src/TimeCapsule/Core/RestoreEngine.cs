namespace TimeCapsule.Core;

public sealed class RestoreOptions
{
    public string SnapshotId { get; set; } = "";
    public string TargetRoot { get; set; } = "";

    /// <summary>只恢复这些相对路径；null 或空 = 整个快照。</summary>
    public List<string>? OnlyPaths { get; set; }

    /// <summary>目标已存在文件时是否覆盖（覆盖前旧文件移到库的 trash 目录）。</summary>
    public bool OverwriteExisting { get; set; }
}

public sealed class RestoreResult
{
    public int Restored { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public string TargetRoot { get; set; } = "";
    public string? TrashDir { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>从快照里把文件解出来。默认绝不覆盖任何现有文件。</summary>
public sealed class RestoreEngine
{
    private readonly Vault _vault;
    private readonly BlobStore _blobs;
    private readonly SnapshotStore _snaps;

    public RestoreEngine(Vault vault)
    {
        _vault = vault;
        _blobs = new BlobStore(vault);
        _snaps = new SnapshotStore(vault);
    }

    public RestoreResult Restore(RestoreOptions opt, Action<string>? progress = null, CancellationToken ct = default)
    {
        var manifest = _snaps.Load(opt.SnapshotId)
            ?? throw new InvalidOperationException("找不到快照：" + opt.SnapshotId);

        if (string.IsNullOrWhiteSpace(opt.TargetRoot))
            throw new ArgumentException("恢复目标目录不能为空");

        var result = new RestoreResult();
        string target = Path.GetFullPath(opt.TargetRoot);
        result.TargetRoot = target;
        Directory.CreateDirectory(target);

        HashSet<string>? only = null;
        if (opt.OnlyPaths != null && opt.OnlyPaths.Count > 0)
            only = new HashSet<string>(opt.OnlyPaths, StringComparer.OrdinalIgnoreCase);

        string? trashRoot = null;
        if (opt.OverwriteExisting)
        {
            trashRoot = Path.Combine(_vault.Root, "trash", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            result.TrashDir = trashRoot;
        }

        int index = 0;
        foreach (var e in manifest.Entries)
        {
            ct.ThrowIfCancellationRequested();
            index++;
            if (only != null && !only.Contains(e.Path)) continue;
            if (index % 10 == 0) progress?.Invoke($"正在恢复 {index}/{manifest.Entries.Count}：{e.Path}");

            string rel = SanitizeRelative(e.Path);
            string dest;
            try
            {
                dest = Path.GetFullPath(Path.Combine(target, rel));
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add(e.Path + "：路径非法（" + ex.Message + "）");
                continue;
            }

            string prefix = target.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!dest.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && !string.Equals(dest, target, StringComparison.OrdinalIgnoreCase))
            {
                result.Failed++;
                result.Errors.Add(e.Path + "：路径越界，已拒绝");
                continue;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

                if (File.Exists(dest))
                {
                    if (!opt.OverwriteExisting)
                    {
                        result.Skipped++;
                        continue;
                    }
                    string backup = Path.Combine(trashRoot!, SanitizeRelative(e.Path));
                    Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                    if (File.Exists(backup)) File.Delete(backup);
                    File.Move(dest, backup);
                }

                _blobs.Get(e.Hash, dest, ct);
                try { File.SetLastWriteTimeUtc(dest, e.MtimeUtc); } catch { /* 时间戳设不上不影响内容 */ }
                result.Restored++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add(e.Path + "：" + ex.Message);
                try { if (File.Exists(dest)) File.Delete(dest); } catch { /* 忽略 */ }
            }
        }

        _vault.Log.Info($"恢复完成 快照={opt.SnapshotId} 目标={target} 成功={result.Restored} 跳过={result.Skipped} 失败={result.Failed}");
        foreach (var err in result.Errors) _vault.Log.Warn("恢复失败：" + err);

        return result;
    }

    public static string SanitizeRelative(string relative)
    {
        var parts = relative.Replace('/', '\\').Split('\\', StringSplitOptions.RemoveEmptyEntries);
        var clean = new List<string>();
        foreach (var raw in parts)
        {
            if (raw == "." ) continue;
            if (raw == "..") { clean.Add("_上级目录_"); continue; }
            var sb = new System.Text.StringBuilder(raw.Length);
            foreach (char ch in raw)
            {
                sb.Append(Array.IndexOf(Path.GetInvalidFileNameChars(), ch) >= 0 ? '_' : ch);
            }
            string name = sb.ToString().TrimEnd('.', ' ');
            if (name.Length == 0) name = "_";
            clean.Add(name);
        }
        return clean.Count == 0 ? "_" : string.Join(Path.DirectorySeparatorChar, clean);
    }
}
