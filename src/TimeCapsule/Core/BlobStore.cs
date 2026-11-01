namespace TimeCapsule.Core;

/// <summary>内容寻址的加密块仓库。块名 = 明文 SHA-256，只增不改。</summary>
public sealed class BlobStore
{
    private readonly Vault _vault;

    public BlobStore(Vault vault) => _vault = vault;

    public string PathFor(string hashHex)
    {
        string bucket = hashHex.Length >= 2 ? hashHex[..2] : "00";
        return Path.Combine(_vault.BlobDir, bucket, hashHex + ".tcb");
    }

    public bool Has(string hashHex) => File.Exists(PathFor(hashHex));

    public long SizeOf(string hashHex)
    {
        var fi = new FileInfo(PathFor(hashHex));
        return fi.Exists ? fi.Length : 0;
    }

    /// <summary>加密一个新块并返回实际写入的大小。已存在则直接复用（绝不覆盖）。</summary>
    public long Put(string sourcePath, string hashHex, CancellationToken ct)
    {
        string dest = PathFor(hashHex);
        if (File.Exists(dest)) return new FileInfo(dest).Length;

        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        string actual = Crypto.EncryptFile(sourcePath, dest, _vault.BlobKey(hashHex), ct);
        if (!string.Equals(actual, hashHex, StringComparison.OrdinalIgnoreCase))
        {
            try { File.Delete(dest); } catch { /* 忽略 */ }
            throw new InvalidDataException("源文件在备份过程中被修改，已丢弃本次结果：" + sourcePath);
        }
        return new FileInfo(dest).Length;
    }

    public string Get(string hashHex, string destPath, CancellationToken ct)
    {
        string src = PathFor(hashHex);
        if (!File.Exists(src)) throw new FileNotFoundException("缺少数据块：" + hashHex, src);
        string actual = Crypto.DecryptFile(src, destPath, _vault.BlobKey(hashHex), ct);
        if (!string.Equals(actual, hashHex, StringComparison.OrdinalIgnoreCase))
        {
            try { File.Delete(destPath); } catch { /* 忽略 */ }
            throw new InvalidDataException("解密后内容校验不通过（块可能已损坏）：" + hashHex);
        }
        return actual;
    }

    public int CountBlobs()
    {
        if (!Directory.Exists(_vault.BlobDir)) return 0;
        return Directory.EnumerateFiles(_vault.BlobDir, "*.tcb", SearchOption.AllDirectories).Count();
    }

    public long TotalBlobBytes()
    {
        if (!Directory.Exists(_vault.BlobDir)) return 0;
        long total = 0;
        foreach (var f in Directory.EnumerateFiles(_vault.BlobDir, "*.tcb", SearchOption.AllDirectories))
        {
            try { total += new FileInfo(f).Length; } catch { /* 忽略 */ }
        }
        return total;
    }

    /// <summary>清理没有任何快照引用的孤儿块。</summary>
    public int CollectGarbage(SnapshotStore snapshots)
    {
        var referenced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var info in snapshots.List())
        {
            var m = snapshots.Load(info.Id);
            if (m == null) continue;
            foreach (var e in m.Entries) referenced.Add(e.Hash);
        }
        int removed = 0;
        if (!Directory.Exists(_vault.BlobDir)) return 0;
        foreach (var f in Directory.EnumerateFiles(_vault.BlobDir, "*.tcb", SearchOption.AllDirectories))
        {
            string name = Path.GetFileNameWithoutExtension(f);
            if (referenced.Contains(name)) continue;
            try { File.Delete(f); removed++; } catch { /* 忽略 */ }
        }
        return removed;
    }
}
