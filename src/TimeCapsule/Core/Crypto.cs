using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace TimeCapsule.Core;

/// <summary>
/// AES-256-GCM 逐文件加密。文件按 1MB 分块，每块独立 nonce（基础 nonce + 块序号），
/// 因此 GB 级文件也只占 1MB 内存，且任意一块被篡改都会被 GCM 标签逮住。
/// </summary>
public static class Crypto
{
    public const int TagSize = 16;
    public const int ChunkSize = 1024 * 1024;
    public const int HeaderSize = 28;

    private static readonly byte[] Magic = { (byte)'T', (byte)'C', (byte)'V', (byte)'1' };

    /// <summary>PBKDF2-HMAC-SHA256 派生 256 位主密钥。</summary>
    public static byte[] DeriveMasterKey(string password, byte[] salt, int iterations)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);
    }

    /// <summary>由主密钥 + 标签派生出用途唯一的子密钥（每个文件、每个 manifest 各一把）。</summary>
    public static byte[] DeriveSubKey(byte[] masterKey, string label)
    {
        using var hmac = new HMACSHA256(masterKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes("tc:" + label.ToLowerInvariant()));
    }

    public static string HashFile(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 16);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(fs);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static string HashBytes(byte[] data)
    {
        return Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
    }

    public static byte[] EncryptBytes(byte[] plain, byte[] key)
    {
        using var input = new MemoryStream(plain, writable: false);
        using var output = new MemoryStream();
        WriteEncrypted(output, input, key, CancellationToken.None);
        return output.ToArray();
    }

    public static byte[] DecryptBytes(byte[] data, byte[] key)
    {
        using var input = new MemoryStream(data, writable: false);
        using var output = new MemoryStream();
        ReadEncrypted(input, output, key, CancellationToken.None);
        return output.ToArray();
    }

    /// <summary>
    /// 把明文文件加密成块文件（先写 .part 再原子改名，断电容错）。
    /// 返回明文的 SHA-256，调用方必须与预期的哈希比对，不一致说明源文件在读取期间被改动。
    /// </summary>
    public static string EncryptFile(string sourcePath, string destPath, byte[] key, CancellationToken ct)
    {
        string tempPath = destPath + ".part";
        try
        {
            string hash;
            using (var input = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 16))
            using (var output = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16))
            {
                hash = WriteEncrypted(output, input, key, ct);
                output.Flush(true);
            }
            if (File.Exists(destPath)) File.Delete(destPath);
            File.Move(tempPath, destPath);
            return hash;
        }
        catch
        {
            TryDelete(tempPath);
            throw;
        }
    }

    /// <summary>把块文件解密到目标路径，返回明文的 SHA-256。</summary>
    public static string DecryptFile(string blobPath, string destPath, byte[] key, CancellationToken ct)
    {
        string tempPath = destPath + ".tcpart";
        try
        {
            string hash;
            using (var input = new FileStream(blobPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16))
            using (var output = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16))
            {
                hash = ReadEncrypted(input, output, key, ct);
                output.Flush(true);
            }
            if (File.Exists(destPath)) File.Delete(destPath);
            File.Move(tempPath, destPath);
            return hash;
        }
        catch
        {
            TryDelete(tempPath);
            throw;
        }
    }

    private static string WriteEncrypted(Stream output, Stream input, byte[] key, CancellationToken ct)
    {
        var header = new byte[HeaderSize];
        Magic.CopyTo(header, 0);
        header[4] = 1;      // 版本
        header[5] = 0;      // 标志位
        var nonceBase = RandomNumberGenerator.GetBytes(8);
        nonceBase.CopyTo(header, 8);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(16), ChunkSize);
        BinaryPrimitives.WriteInt64BigEndian(header.AsSpan(20), 0); // 明文长度稍后回填
        output.Write(header, 0, header.Length);

        using var aes = new AesGcm(key, TagSize);
        using var sha = SHA256.Create();
        var plain = new byte[ChunkSize];
        var cipher = new byte[ChunkSize];
        var tag = new byte[TagSize];
        var nonce = new byte[12];
        var lenBuf = new byte[4];
        long total = 0;
        int index = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            int read = ReadFull(input, plain);
            if (read == 0) break;

            sha.TransformBlock(plain, 0, read, null, 0);
            nonceBase.CopyTo(nonce, 0);
            BinaryPrimitives.WriteInt32BigEndian(nonce.AsSpan(8), index);
            aes.Encrypt(nonce, plain.AsSpan(0, read), cipher.AsSpan(0, read), tag);
            BinaryPrimitives.WriteInt32BigEndian(lenBuf, read);
            output.Write(lenBuf, 0, 4);
            output.Write(cipher, 0, read);
            output.Write(tag, 0, TagSize);

            total += read;
            index++;
        }

        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

        long end = output.Position;
        output.Seek(20, SeekOrigin.Begin);
        var lenBuf8 = new byte[8];
        BinaryPrimitives.WriteInt64BigEndian(lenBuf8, total);
        output.Write(lenBuf8, 0, 8);
        output.Seek(end, SeekOrigin.Begin);

        return Convert.ToHexString(sha.Hash!).ToLowerInvariant();
    }

    private static string ReadEncrypted(Stream input, Stream output, byte[] key, CancellationToken ct)
    {
        var header = new byte[HeaderSize];
        if (ReadFull(input, header) != HeaderSize)
            throw new InvalidDataException("块文件头不完整");
        for (int i = 0; i < 4; i++)
        {
            if (header[i] != Magic[i]) throw new InvalidDataException("不是时间胶囊的块文件");
        }
        if (header[4] != 1) throw new InvalidDataException("块文件版本不支持：" + header[4]);

        var nonceBase = header.AsSpan(8, 8).ToArray();
        long declaredLength = BinaryPrimitives.ReadInt64BigEndian(header.AsSpan(20));

        using var aes = new AesGcm(key, TagSize);
        using var sha = SHA256.Create();
        var cipher = new byte[ChunkSize];
        var plain = new byte[ChunkSize];
        var tag = new byte[TagSize];
        var nonce = new byte[12];
        var lenBuf = new byte[4];
        long total = 0;
        int index = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            if (ReadFull(input, lenBuf) == 0) break;
            int length = BinaryPrimitives.ReadInt32BigEndian(lenBuf);
            if (length < 0 || length > ChunkSize) throw new InvalidDataException("块长度非法：" + length);
            if (ReadFull(input, cipher.AsSpan(0, length)) != length) throw new InvalidDataException("块数据被截断");
            if (ReadFull(input, tag) != TagSize) throw new InvalidDataException("块校验码被截断");

            nonceBase.CopyTo(nonce, 0);
            BinaryPrimitives.WriteInt32BigEndian(nonce.AsSpan(8), index);
            aes.Decrypt(nonce, cipher.AsSpan(0, length), tag, plain.AsSpan(0, length));

            sha.TransformBlock(plain, 0, length, null, 0);
            output.Write(plain, 0, length);
            total += length;
            index++;
        }

        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        if (total != declaredLength)
            throw new InvalidDataException($"明文长度不符：头里写 {declaredLength}，实际 {total}");

        return Convert.ToHexString(sha.Hash!).ToLowerInvariant();
    }

    private static int ReadFull(Stream s, byte[] buffer)
    {
        return ReadFull(s, buffer.AsSpan());
    }

    private static int ReadFull(Stream s, Span<byte> buffer)
    {
        int total = 0;
        while (total < buffer.Length)
        {
            int n = s.Read(buffer.Slice(total));
            if (n <= 0) break;
            total += n;
        }
        return total;
    }

    private static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { /* 忽略 */ }
    }
}
