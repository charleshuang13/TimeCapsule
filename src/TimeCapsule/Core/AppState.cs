using System.Text.Json;

namespace TimeCapsule.Core;

/// <summary>程序级状态（记住上次打开的库），存在 %APPDATA%\TimeCapsule\app.json。</summary>
public sealed class AppState
{
    public string? LastVaultPath { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }

    public static string Dir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimeCapsule");

    private static string FilePath => Path.Combine(Dir, "app.json");

    public static AppState Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var s = JsonSerializer.Deserialize<AppState>(File.ReadAllText(FilePath));
                if (s != null) return s;
            }
        }
        catch { /* 损坏就当没有 */ }
        return new AppState();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { /* 忽略 */ }
    }
}

/// <summary>开机自启（当前用户注册表 Run 项，不需要管理员）。</summary>
public static class Startup
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "TimeCapsule";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey);
            return key?.GetValue(ValueName) != null;
        }
        catch { return false; }
    }

    public static void Set(bool enabled, string exePath)
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RunKey);
            if (key == null) return;
            if (enabled) key.SetValue(ValueName, "\"" + exePath + "\" --tray");
            else key.DeleteValue(ValueName, throwOnMissingValue: false);
        }
        catch { /* 被组策略限制时忽略 */ }
    }
}
