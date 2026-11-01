namespace TimeCapsule.Core;

/// <summary>
/// 目录监控：FileSystemWatcher 实时监听 + 防抖合并 + 定时兜底全量扫描。
/// 必须在 UI 线程创建（内部用 WinForms Timer）。
/// </summary>
public sealed class FolderMonitor : IDisposable
{
    private FileSystemWatcher? _watcher;
    private readonly System.Windows.Forms.Timer _debounce = new();
    private readonly System.Windows.Forms.Timer _interval = new();
    private Control? _host;
    private string _root = "";

    /// <summary>触发备份，参数是触发来源：watch / timer。</summary>
    public event Action<string>? Triggered;

    public FolderMonitor()
    {
        _debounce.Interval = 5000;
        _debounce.Tick += DebounceTick;
        _interval.Tick += IntervalTick;
    }

    /// <summary>绑定 UI 宿主控件：FileSystemWatcher 的回调在后台线程，必须投递回 UI 线程操作 Timer。</summary>
    public void Attach(Control host)
    {
        _host = host;
    }

    private void PostRestart()
    {
        Control? host = _host;
        if (host != null)
        {
            try
            {
                if (host.IsHandleCreated)
                {
                    host.BeginInvoke(new Action(RestartDebounce));
                    return;
                }
            }
            catch { /* 宿主已销毁，退回直接调用 */ }
        }
        RestartDebounce();
    }

    private void RestartDebounce()
    {
        _debounce.Stop();
        _debounce.Start();
    }

    public bool Running => _watcher != null;

    public void Start(string root, int intervalMinutes, int debounceSeconds)
    {
        Stop();
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root)) return;

        _root = root;
        _debounce.Interval = Math.Max(1, debounceSeconds) * 1000;
        _interval.Interval = Math.Max(1, intervalMinutes) * 60 * 1000;

        var watcher = new FileSystemWatcher(root)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = 64 * 1024,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName |
                           NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes
        };
        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnWatcherError;
        watcher.EnableRaisingEvents = true;
        _watcher = watcher;

        _interval.Start();
    }

    public void Stop()
    {
        _debounce.Stop();
        _interval.Stop();
        if (_watcher != null)
        {
            try
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Changed -= OnChanged;
                _watcher.Created -= OnChanged;
                _watcher.Deleted -= OnChanged;
                _watcher.Renamed -= OnRenamed;
                _watcher.Error -= OnWatcherError;
                _watcher.Dispose();
            }
            catch { /* 忽略 */ }
            _watcher = null;
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (IsIgnored(e.FullPath)) return;
        PostRestart();
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (IsIgnored(e.FullPath) && IsIgnored(e.OldFullPath)) return;
        PostRestart();
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        // 缓冲区溢出等：靠定时扫描兜底，这里只提前触发一次
        PostRestart();
    }

    private bool IsIgnored(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath)) return true;
        string name = Path.GetFileName(fullPath);
        if (name.Length == 0) return false;
        try
        {
            var settings = _settingsProvider?.Invoke();
            if (settings == null) return false;
            return Exclusions.IsExcludedFile(name, settings) || Exclusions.IsExcludedDir(name, settings);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>由外部（主窗体）提供当前设置，用于实时判断排除项。</summary>
    public Func<VaultSettings?>? SettingsProvider
    {
        get => _settingsProvider;
        set => _settingsProvider = value;
    }
    private Func<VaultSettings?>? _settingsProvider;

    private void DebounceTick(object? sender, EventArgs e)
    {
        _debounce.Stop();
        Triggered?.Invoke("watch");
    }

    private void IntervalTick(object? sender, EventArgs e)
    {
        Triggered?.Invoke("timer");
    }

    public void Dispose()
    {
        Stop();
        _debounce.Dispose();
        _interval.Dispose();
    }
}
