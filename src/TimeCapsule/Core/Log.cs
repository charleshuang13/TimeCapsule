using System.Text;

namespace TimeCapsule.Core;

/// <summary>按月分文件的运行日志。只记操作与错误，不记文件内容。</summary>
public sealed class Log
{
    private readonly string _dir;
    private readonly object _gate = new();

    public Log(string dir)
    {
        _dir = dir;
        try { Directory.CreateDirectory(dir); } catch { /* 记不了日志也不能崩 */ }
    }

    public string Dir => _dir;

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message, Exception? ex = null)
        => Write("ERROR", ex == null ? message : message + " | " + ex.GetType().Name + ": " + ex.Message);

    private void Write(string level, string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
        lock (_gate)
        {
            try
            {
                Directory.CreateDirectory(_dir);
                string file = Path.Combine(_dir, $"backup-{DateTime.Now:yyyyMM}.log");
                File.AppendAllText(file, line + Environment.NewLine, new UTF8Encoding(false));
            }
            catch { /* 忽略 */ }
        }
        try { Console.WriteLine(line); } catch { /* 无控制台 */ }
    }
}
