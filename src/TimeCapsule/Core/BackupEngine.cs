namespace TimeCapsule.Core;

public sealed class BackupResult
{
    public bool Created { get; set; }
    public bool NoChange { get; set; }
    public string? SnapshotId { get; set; }
    public int Scanned { get; set; }
    public int FileCount { get; set; }
    public int Added { get; set; }
    public int Modified { get; set; }
    public int Removed { get; set; }
    public int EncryptedNew { get; set; }
    public long TotalBytes { get; set; }
    public long NewBytes { get; set; }
    public string Message { get; set; } = "";
    public List<string> Errors { get; set; } = new();
}

/// <summary>一次备份：扫描 → 去重 → 加密 → 生成快照。</summary>
public sealed class BackupEngine
{
    private readonly Vault _vault;
    private readonly BlobStore _blobs;
    private readonly SnapshotStore _snaps;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public BackupEngine(Vault vault)
    {
        _vault = vault;
        _blobs = new BlobStore(vault);
        _snaps = new SnapshotStore(vault);
    }

    public bool IsBusy => _gate.CurrentCount == 0;

    /// <summary>执行一次备份。同一时刻只允许一个备份任务，第二个直接返回 null。</summary>
    public BackupResult? Run(string trigger, CancellationToken ct = default, Action<string>? progress = null)
    {
        if (!_gate.Wait(0)) return null;
        try
        {
            return RunCore(trigger, ct, progress);
        }
        finally
        {
            _gate.Release();
        }
    }

    private BackupResult RunCore(string trigger, CancellationToken ct, Action<string>? progress)
    {
        var settings = _vault.Settings;
        string root = settings.SourceRoot;
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
        {
            return new BackupResult { Message = "监控目录不存在：" + root };
        }

        var result = new BackupResult();
        progress?.Invoke("正在扫描目录…");

        var files = new List<string>();
        Collect(root, settings, files, root);

        var prevInfo = _snaps.Latest();
        var prevMap = new Dictionary<string, SnapshotEntry>(StringComparer.OrdinalIgnoreCase);
        if (prevInfo != null)
        {
            var pm = _snaps.Load(prevInfo.Id);
            if (pm != null)
            {
                foreach (var e in pm.Entries) prevMap[e.Path] = e;
            }
        }

        long maxFileBytes = settings.MaxFileSizeMB <= 0
            ? long.MaxValue
            : settings.MaxFileSizeMB * 1024L * 1024L;

        var entries = new List<SnapshotEntry>(files.Count);
        int scanned = 0;
        long totalBytes = 0;

        for (int i = 0; i < files.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            string file = files[i];
            if (i % 20 == 0) progress?.Invoke($"正在处理 {i + 1}/{files.Count}：{Path.GetFileName(file)}");

            FileInfo fi;
            try
            {
                fi = new FileInfo(file);
                if (!fi.Exists) continue;
            }
            catch (Exception ex)
            {
                result.Errors.Add(Path.GetRelativePath(root, file) + "：" + ex.Message);
                continue;
            }

            if (fi.Length > maxFileBytes)
            {
                result.Errors.Add("超过单文件上限，已跳过：" + Path.GetRelativePath(root, file));
                continue;
            }

            string rel = Path.GetRelativePath(root, file);
            long size = fi.Length;
            DateTime mtimeUtc = fi.LastWriteTimeUtc;
            scanned++;

            // 大小与修改时间都没变、旧块还在 → 直接复用，不重读文件
            if (prevMap.TryGetValue(rel, out var old) && old.Size == size && old.MtimeUtc == mtimeUtc && _blobs.Has(old.Hash))
            {
                entries.Add(old);
                totalBytes += size;
                continue;
            }

            try
            {
                string hash = Crypto.HashFile(file);
                long blobSize;
                if (_blobs.Has(hash))
                {
                    blobSize = _blobs.SizeOf(hash);
                }
                else
                {
                    blobSize = _blobs.Put(file, hash, ct);
                    result.EncryptedNew++;
                    result.NewBytes += blobSize;
                }
                entries.Add(new SnapshotEntry
                {
                    Path = rel,
                    Hash = hash,
                    Size = size,
                    MtimeUtc = mtimeUtc,
                    BlobSize = blobSize
                });
                totalBytes += size;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                result.Errors.Add(rel + "：" + ex.Message);
            }
        }

        // 与上一张快照比对
        var newPaths = new HashSet<string>(entries.Select(e => e.Path), StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            if (!prevMap.TryGetValue(e.Path, out var o)) result.Added++;
            else if (!string.Equals(o.Hash, e.Hash, StringComparison.OrdinalIgnoreCase)) result.Modified++;
        }
        foreach (var p in prevMap.Keys)
        {
            if (!newPaths.Contains(p)) result.Removed++;
        }

        result.Scanned = scanned;
        result.FileCount = entries.Count;
        result.TotalBytes = totalBytes;

        if (prevInfo != null && result.Added == 0 && result.Modified == 0 && result.Removed == 0)
        {
            result.NoChange = true;
            result.Message = "内容自上次备份后没有变化，未生成新快照";
            return result;
        }

        progress?.Invoke("正在写入快照…");
        var manifest = new SnapshotManifest
        {
            Id = _snaps.NewId(),
            TakenAt = DateTimeOffset.Now,
            SourceRoot = root,
            Trigger = trigger,
            Prev = prevInfo?.Id,
            FileCount = entries.Count,
            TotalBytes = totalBytes,
            Added = result.Added,
            Modified = result.Modified,
            Removed = result.Removed,
            Entries = entries
        };
        _snaps.Save(manifest);
        result.Created = true;
        result.SnapshotId = manifest.Id;
        result.Message = $"已生成快照 {manifest.Id}";

        _vault.Log.Info($"备份完成 [{trigger}] 快照={manifest.Id} 文件={entries.Count} 新增={result.Added} 修改={result.Modified} 删除={result.Removed} 新块={result.EncryptedNew}");
        foreach (var err in result.Errors)
        {
            _vault.Log.Warn("备份跳过：" + err);
        }

        if (settings.RetainMode != "keepAll")
        {
            try { _snaps.ApplyRetention(); }
            catch (Exception ex) { _vault.Log.Warn("保留策略执行失败：" + ex.Message); }
        }

        return result;
    }

    private void Collect(string dir, VaultSettings s, List<string> into, string sourceRoot)
    {
        string full = Path.GetFullPath(dir);
        // 库本身在监控目录里时不能把自己也备进去
        if (IsUnder(full, _vault.Root)) return;

        string[] files;
        try { files = Directory.GetFiles(dir); }
        catch (Exception ex) { _vault.Log.Warn("无法读取目录 " + dir + "：" + ex.Message); return; }

        foreach (var f in files)
        {
            if (Exclusions.IsExcludedFile(Path.GetFileName(f), s)) continue;
            into.Add(f);
        }

        string[] dirs;
        try { dirs = Directory.GetDirectories(dir); }
        catch (Exception ex) { _vault.Log.Warn("无法读取目录 " + dir + "：" + ex.Message); return; }

        foreach (var d in dirs)
        {
            string name = Path.GetFileName(d);
            if (Exclusions.IsExcludedDir(name, s)) continue;
            try
            {
                var attrs = new DirectoryInfo(d).Attributes;
                if ((attrs & FileAttributes.ReparsePoint) != 0) continue; // 跳过符号链接，防死循环
            }
            catch { continue; }
            Collect(d, s, into, sourceRoot);
        }
    }

    private static bool IsUnder(string path, string root)
    {
        string a = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string b = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    }
}
