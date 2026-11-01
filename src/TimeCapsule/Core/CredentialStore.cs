using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace TimeCapsule.Core;

/// <summary>
/// 记住库密码（用户选择"设置一次，以后自动使用"）。
/// 用 Windows DPAPI 以当前用户身份加密，拷到别的机器或别的用户下解不开。
/// </summary>
public static class CredentialStore
{
    private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CryptProtectData(ref DATA_BLOB pDataIn, string? szDataDescr, IntPtr pOptionalEntropy,
        IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CryptUnprotectData(ref DATA_BLOB pDataIn, IntPtr ppszDataDescr, IntPtr pOptionalEntropy,
        IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LocalFree(IntPtr hMem);

    private sealed class Record
    {
        public string VaultPath { get; set; } = "";
        public string Password { get; set; } = "";
    }

    private static string FilePath => Path.Combine(AppState.Dir, "cred.dat");

    public static void Save(string vaultPath, string password)
    {
        try
        {
            Directory.CreateDirectory(AppState.Dir);
            var rec = new Record { VaultPath = Path.GetFullPath(vaultPath), Password = password };
            byte[] plain = JsonSerializer.SerializeToUtf8Bytes(rec);
            byte[] enc = Protect(plain);
            File.WriteAllBytes(FilePath, enc);
        }
        catch { /* 存不了就退回手动输密码 */ }
    }

    public static string? Load(string vaultPath)
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            byte[] enc = File.ReadAllBytes(FilePath);
            byte[] plain = Unprotect(enc);
            var rec = JsonSerializer.Deserialize<Record>(plain);
            if (rec == null || string.IsNullOrEmpty(rec.Password)) return null;
            if (!string.Equals(Path.GetFullPath(rec.VaultPath), Path.GetFullPath(vaultPath), StringComparison.OrdinalIgnoreCase))
                return null;
            return rec.Password;
        }
        catch
        {
            return null;
        }
    }

    public static void Clear()
    {
        try { if (File.Exists(FilePath)) File.Delete(FilePath); } catch { /* 忽略 */ }
    }

    public static bool HasRemembered => File.Exists(FilePath);

    /// <summary>DPAPI 往返自检（不落盘）。</summary>
    public static bool DpapiRoundTrip(out string detail)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes("dpapi-roundtrip-" + Guid.NewGuid().ToString("N"));
            byte[] enc = Protect(data);
            byte[] dec = Unprotect(enc);
            bool ok = dec.AsSpan().SequenceEqual(data) && !enc.AsSpan().SequenceEqual(data);
            detail = ok ? "加密 " + enc.Length + " 字节，解密一致" : "解密结果与原数据不一致";
            return ok;
        }
        catch (Exception ex)
        {
            detail = ex.GetType().Name + " " + ex.Message;
            return false;
        }
    }

    private static byte[] Protect(byte[] data)
    {
        var inBlob = new DATA_BLOB { cbData = data.Length, pbData = Marshal.AllocHGlobal(data.Length) };
        var outBlob = new DATA_BLOB();
        try
        {
            Marshal.Copy(data, 0, inBlob.pbData, data.Length);
            if (!CryptProtectData(ref inBlob, "TimeCapsule", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CRYPTPROTECT_UI_FORBIDDEN, ref outBlob))
                throw new InvalidOperationException("DPAPI 加密失败：" + Marshal.GetLastWin32Error());
            var result = new byte[outBlob.cbData];
            Marshal.Copy(outBlob.pbData, result, 0, outBlob.cbData);
            return result;
        }
        finally
        {
            if (inBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(inBlob.pbData);
            if (outBlob.pbData != IntPtr.Zero) LocalFree(outBlob.pbData);
        }
    }

    private static byte[] Unprotect(byte[] data)
    {
        var inBlob = new DATA_BLOB { cbData = data.Length, pbData = Marshal.AllocHGlobal(data.Length) };
        var outBlob = new DATA_BLOB();
        try
        {
            Marshal.Copy(data, 0, inBlob.pbData, data.Length);
            if (!CryptUnprotectData(ref inBlob, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CRYPTPROTECT_UI_FORBIDDEN, ref outBlob))
                throw new InvalidOperationException("DPAPI 解密失败：" + Marshal.GetLastWin32Error());
            var result = new byte[outBlob.cbData];
            Marshal.Copy(outBlob.pbData, result, 0, outBlob.cbData);
            return result;
        }
        finally
        {
            if (inBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(inBlob.pbData);
            if (outBlob.pbData != IntPtr.Zero) LocalFree(outBlob.pbData);
        }
    }
}
