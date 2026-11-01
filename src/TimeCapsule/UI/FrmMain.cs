using System.Diagnostics;
using TimeCapsule.Core;

namespace TimeCapsule.UI;

/// <summary>快照列表的一行（只做显示格式化）。</summary>
public sealed class SnapshotRow
{
    public string Time { get; set; } = "";
    public string Trigger { get; set; } = "";
    public string FilesText { get; set; } = "";
    public string SizeText { get; set; } = "";
    public string ChangeText { get; set; } = "";
    public string Note { get; set; } = "";
    public SnapshotInfo Info { get; set; } = new();
}

/// <summary>文件列表的一行，Selected 由复选框绑定。</summary>
public sealed class FileRow
{
    public bool Selected { get; set; }
    public string Path { get; set; } = "";
    public string SizeText { get; set; } = "";
    public string MtimeText { get; set; } = "";
    public SnapshotEntry Entry { get; set; } = new();
}

public partial class FrmMain : Form
{
    private Vault? _vault;
    private BackupEngine? _engine;
    private RestoreEngine? _restore;
    private SnapshotStore? _snaps;
    private BlobStore? _blobs;
    private FolderMonitor? _monitor;
    private NotifyIcon? _tray;
    private AppState _app = new();

    private bool _busy;
    private bool _paused;
    private bool _pending;
    private bool _reallyExit;
    private bool _suppressSelection;
    private bool _trayHintShown;
    private bool _splitterUserAdjusted;

    private DateTime? _lastBackupAt;
    private long _vaultBytes;
    private string _autoTarget = "";
    private SnapshotManifest? _currentManifest;
    private List<FileRow> _fileRows = new();

    private Color _dotColor = UiTheme.Muted;
    private readonly bool _autoStartMonitor;
    private readonly bool _persistState;

    public FrmMain() : this(null, new AppState(), true, false)
    {
    }

    public FrmMain(Vault? vault, AppState app, bool autoStartMonitor, bool persistState = true)
    {
        InitializeComponent();
        _autoStartMonitor = autoStartMonitor;
        _persistState = persistState;
        _app = app;

        UiTheme.StyleGrid(dgvSnapshots);
        UiTheme.StyleGrid(dgvFiles);
        // 表格皮肤统一在 UiTheme.StyleGrid 里维护，其余外观都在 Designer 里，设计器所见即所得
        dgvFiles.ReadOnly = false;
        dgvFiles.MultiSelect = false;

        foreach (DataGridViewColumn col in dgvFiles.Columns)
        {
            col.ReadOnly = !ReferenceEquals(col, colSel);
        }

        panelDot.Paint += PanelDot_Paint;
        splitMain.SplitterMoved += SplitMain_SplitterMoved;

        if (vault != null) AttachVault(vault, app);
    }

    // ─────────────────────────── 装配 ───────────────────────────

    private void AttachVault(Vault vault, AppState app)
    {
        _vault = vault;
        _app = app;
        _engine = new BackupEngine(vault);
        _restore = new RestoreEngine(vault);
        _snaps = new SnapshotStore(vault);
        _blobs = new BlobStore(vault);

        if (_persistState)
        {
            _app.LastVaultPath = vault.Root;
            _app.Save();
        }

        txtSource.Text = vault.Settings.SourceRoot;
        Text = "时间胶囊 · " + Path.GetFileName(vault.Root.TrimEnd('\\'));

        if (_autoStartMonitor)
        {
            StartMonitor();
            BuildTray();
        }

        RefreshSnapshots(null);
        UpdateVaultBytes();
        UpdateStatusBar();
        ApplySavedSize();
    }

    private void ApplySavedSize()
    {
        if (_app.WindowWidth >= MinimumSize.Width && _app.WindowHeight >= MinimumSize.Height)
        {
            Size = new Size(_app.WindowWidth, _app.WindowHeight);
        }
    }

    private void StartMonitor()
    {
        if (_vault == null) return;
        if (_monitor == null)
        {
            _monitor = new FolderMonitor();
            _monitor.Attach(this);
            _monitor.SettingsProvider = () => _vault.Settings;
            _monitor.Triggered += OnMonitorTriggered;
        }
        if (_paused) return;
        var s = _vault.Settings;
        _monitor.Start(s.SourceRoot, s.IntervalMinutes, s.DebounceSeconds);
    }

    private void OnMonitorTriggered(string trigger)
    {
        if (_vault == null) return;
        StartBackup(trigger);
    }

    // ─────────────────────────── 状态显示 ───────────────────────────

    private void SetStatus(string text, Color color)
    {
        lblStatus.Text = text;
        lblStatus.ForeColor = UiTheme.Muted;
        _dotColor = color;
        panelDot.Invalidate();
    }

    private void UpdateStatusBar()
    {
        if (_vault == null) return;
        string state = _paused ? "已暂停" : (_monitor?.Running == true ? "监控中" : "未监控");
        string last = _lastBackupAt.HasValue ? _lastBackupAt.Value.ToString("yyyy-MM-dd HH:mm") : LatestSnapshotTime();
        int count = _snaps?.Count() ?? 0;
        SetStatus($"{state} · 上次备份 {last} · 快照 {count} 张 · 库占用 {Humanize.Size(_vaultBytes)}", _paused ? UiTheme.Muted : UiTheme.Green);
    }

    private string LatestSnapshotTime()
    {
        var info = _snaps?.Latest();
        return info == null ? "—" : info.TakenAt.ToString("yyyy-MM-dd HH:mm");
    }

    private void UpdateVaultBytes()
    {
        if (_blobs == null) return;
        try { _vaultBytes = _blobs.TotalBlobBytes(); }
        catch { _vaultBytes = 0; }
    }

    private void PanelDot_Paint(object? sender, PaintEventArgs e)
    {
        UiTheme.DrawDot(sender, e, _dotColor);
    }

    // ─────────────────────────── 快照列表 ───────────────────────────

    private void RefreshSnapshots(string? selectId)
    {
        if (_snaps == null) return;
        List<SnapshotInfo> list;
        try
        {
            list = _snaps.List();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "读取快照列表失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var rows = list.Select(info => new SnapshotRow
        {
            Time = info.TakenAt.ToString("yyyy-MM-dd HH:mm:ss"),
            Trigger = info.TriggerText,
            FilesText = info.FileCount.ToString(),
            SizeText = Humanize.Size(info.TotalBytes),
            ChangeText = info.ChangeText,
            Note = info.FileCount == 0 ? "目录为空" : (info.Prev == null ? "起点" : ""),
            Info = info
        }).ToList();

        _suppressSelection = true;
        dgvSnapshots.DataSource = null;
        dgvSnapshots.DataSource = rows;
        _suppressSelection = false;

        if (rows.Count == 0)
        {
            dgvFiles.DataSource = null;
            _fileRows = new List<FileRow>();
            _currentManifest = null;
            lblFileHeader.Text = "该时间点的文件";
            lblDetailBody.Text = "还没有任何快照。点右上角“立即备份”，或等文件一有变化自动拍一张。";
            UpdateRestoreHint();
            AutoFitSplitter();
            return;
        }

        int index = 0;
        if (selectId != null)
        {
            int found = rows.FindIndex(r => r.Info.Id == selectId);
            if (found >= 0) index = found;
        }
        dgvSnapshots.ClearSelection();
        dgvSnapshots.Rows[index].Selected = true;
        dgvSnapshots.CurrentCell = dgvSnapshots.Rows[index].Cells[0];
        ShowSnapshot(rows[index].Info);
        AutoFitSplitter();
    }

    /// <summary>快照不多时把上半区收窄，别留一大片空白。</summary>
    private void AutoFitSplitter()
    {
        if (_splitterUserAdjusted) return;
        try
        {
            int h = splitMain.ClientSize.Height;
            if (h < 240) return;
            int content = 30 + dgvSnapshots.ColumnHeadersHeight + dgvSnapshots.Rows.Count * dgvSnapshots.RowTemplate.Height + 26;
            int want = Math.Min((int)(h * 0.58), Math.Max(150, content));
            int minBottom = Math.Max(150, splitMain.Panel2MinSize);
            int max = h - splitMain.SplitterWidth - minBottom;
            if (max < 120) max = 120;
            splitMain.SplitterDistance = Math.Max(120, Math.Min(max, want));
        }
        catch
        {
            // 尺寸极端时忽略
        }
    }

    private void DgvSnapshots_SelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressSelection) return;
        if (dgvSnapshots.CurrentRow?.DataBoundItem is not SnapshotRow row) return;
        ShowSnapshot(row.Info);
    }

    private void ShowSnapshot(SnapshotInfo info)
    {
        if (_snaps == null) return;
        var manifest = _snaps.Load(info.Id);
        _currentManifest = manifest;

        _fileRows = new List<FileRow>();
        if (manifest != null)
        {
            foreach (var entry in manifest.Entries)
            {
                _fileRows.Add(new FileRow
                {
                    Selected = false,
                    Path = entry.Path,
                    SizeText = Humanize.Size(entry.Size),
                    MtimeText = entry.MtimeUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    Entry = entry
                });
            }
        }

        dgvFiles.DataSource = null;
        dgvFiles.DataSource = _fileRows;
        dgvFiles.ClearSelection();

        lblFileHeader.Text = manifest == null
            ? "该时间点的文件（读取失败）"
            : $"该时间点的文件（{manifest.Entries.Count} 个 / {Humanize.Size(manifest.TotalBytes)}）";

        lblDetailBody.Text = "在上面的列表里选一个文件，这里显示它的大小、修改时间和内容指纹。";

        if (_currentManifest != null)
        {
            string suggestion = SuggestTarget();
            if (txtTarget.Text.Trim().Length == 0 || string.Equals(txtTarget.Text, _autoTarget, StringComparison.OrdinalIgnoreCase))
            {
                _autoTarget = suggestion;
                txtTarget.Text = suggestion;
            }
        }

        UpdateRestoreHint();
    }

    private void DgvFiles_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvFiles.CurrentRow?.DataBoundItem is not FileRow row) return;
        var entry = row.Entry;
        lblDetailBody.Text =
            "路径\r\n" + entry.Path + "\r\n" +
            "大小\r\n" + Humanize.Size(entry.Size) + "（" + entry.Size.ToString("N0") + " 字节）\r\n" +
            "修改时间\r\n" + entry.MtimeUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" +
            "加密后\r\n" + Humanize.Size(entry.BlobSize) + "\r\n" +
            "内容指纹\r\n" + entry.Hash[..16] + "\r\n" + entry.Hash[16..];
    }

    private void DgvFiles_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (dgvFiles.IsCurrentCellDirty && dgvFiles.CurrentCell is DataGridViewCheckBoxCell)
        {
            dgvFiles.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void DgvFiles_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        UpdateRestoreHint();
    }

    private List<string> SelectedPaths()
    {
        var list = new List<string>();
        foreach (var row in _fileRows)
        {
            if (row.Selected) list.Add(row.Path);
        }
        return list;
    }

    private void UpdateRestoreHint()
    {
        if (_currentManifest == null)
        {
            lblRestoreHint.Text = "请在上方选择一个时间点";
            return;
        }
        var selected = SelectedPaths();
        string head = $"已选 {_currentManifest.TakenAt:yyyy-MM-dd HH:mm} 的快照（{_currentManifest.Entries.Count} 个文件 / {Humanize.Size(_currentManifest.TotalBytes)}）";
        lblRestoreHint.Text = selected.Count > 0
            ? head + $" · 已勾选 {selected.Count} 个，将只恢复勾选的文件"
            : head + " · 未勾选任何文件，将恢复整张快照";
    }

    // ─────────────────────────── 备份 ───────────────────────────

    private void BtnBackupNow_Click(object? sender, EventArgs e)
    {
        StartBackup("manual");
    }

    private async void StartBackup(string trigger)
    {
        if (_vault == null || _engine == null) return;
        if (_busy)
        {
            _pending = true;
            return;
        }
        if (_paused && trigger == "watch") return;

        _busy = true;
        btnBackupNow.Enabled = false;
        progressBar.Style = ProgressBarStyle.Marquee;
        SetStatus(trigger == "watch" ? "检测到文件变化，正在备份…" : "正在备份…", UiTheme.Orange);

        IProgress<string> progress = new Progress<string>(text => lblStatus.Text = text);
        try
        {
            var result = await Task.Run(() =>
                _engine!.Run(trigger, CancellationToken.None, text => progress.Report(text)));

            if (result == null)
            {
                SetStatus("已有备份任务在进行，本次跳过", UiTheme.Orange);
            }
            else if (result.Created && result.SnapshotId != null)
            {
                _lastBackupAt = DateTime.Now;
                string extra = result.Errors.Count > 0 ? $" · 跳过 {result.Errors.Count} 个（见日志）" : "";
                SetStatus($"备份完成 · {result.FileCount} 个文件 · 新增 {result.Added} / 修改 {result.Modified} / 删除 {result.Removed}{extra}", UiTheme.Green);
                UpdateVaultBytes();
                RefreshSnapshots(result.SnapshotId);
                UpdateStatusBar();
                if (_vault.Settings.NotifyOnBackup && _tray != null)
                {
                    _tray.ShowBalloonTip(2500, "时间胶囊", $"已完成一次备份：{result.FileCount} 个文件", ToolTipIcon.Info);
                }
            }
            else if (result.NoChange)
            {
                SetStatus(result.Message, UiTheme.Green);
                if (trigger == "manual")
                {
                    MessageBox.Show(this, "内容自上次备份后没有变化，没有生成新的快照。", "时间胶囊",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                SetStatus(result.Message, UiTheme.Red);
                MessageBox.Show(this, result.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            SetStatus("备份失败：" + ex.Message, UiTheme.Red);
            _vault?.Log.Error("备份失败", ex);
            MessageBox.Show(this, "备份失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnBackupNow.Enabled = true;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            UpdateStatusBar();
            if (_pending)
            {
                _pending = false;
                StartBackup("watch");
            }
        }
    }

    private void BtnPause_Click(object? sender, EventArgs e)
    {
        if (_vault == null) return;
        _paused = !_paused;
        if (_paused)
        {
            _monitor?.Stop();
            btnPause.Text = "继续监控";
            SetStatus("监控已暂停，文件变化不会再自动备份", UiTheme.Muted);
        }
        else
        {
            _monitor?.Stop();
            StartMonitor();
            btnPause.Text = "暂停监控";
            UpdateStatusBar();
        }
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        if (_vault == null) return;
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择要监控并备份的目录",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(_vault.Settings.SourceRoot) ? _vault.Settings.SourceRoot : ""
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        if (string.Equals(Path.GetFullPath(dlg.SelectedPath).TrimEnd('\\'),
                Path.GetFullPath(_vault.Root).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, "不能监控备份库自己。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _vault.Settings.SourceRoot = dlg.SelectedPath;
        _vault.SaveConfig();
        txtSource.Text = dlg.SelectedPath;
        _monitor?.Stop();
        StartMonitor();
        UpdateStatusBar();
        MessageBox.Show(this, "监控目录已更换。建议点一次“立即备份”，把新目录的当前状态存成起点。", "时间胶囊",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        if (_vault == null) return;
        using var dlg = new FrmSettings(_vault);
        dlg.ShowDialog(this);
        if (dlg.Saved)
        {
            txtSource.Text = _vault.Settings.SourceRoot;
            _monitor?.Stop();
            StartMonitor();
            UpdateVaultBytes();
            UpdateStatusBar();
        }
    }

    // ─────────────────────────── 恢复 ───────────────────────────

    private void BtnPickTarget_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择恢复到哪个目录（程序不会改动原目录，除非你特意选它）",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true,
            SelectedPath = Directory.Exists(txtTarget.Text) ? txtTarget.Text : SuggestTarget()
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txtTarget.Text = dlg.SelectedPath;
        }
    }

    private string SuggestTarget()
    {
        if (_vault == null || _currentManifest == null) return "";
        string source = _vault.Settings.SourceRoot.TrimEnd('\\');
        string parent = Path.GetDirectoryName(source) ?? source;
        string name = Path.GetFileName(source);
        if (string.IsNullOrEmpty(name)) name = "恢复";
        return Path.Combine(parent, "时间胶囊恢复", name + "_" + _currentManifest.Id);
    }

    private void ChkOverwrite_CheckedChanged(object? sender, EventArgs e)
    {
        if (!chkOverwrite.Checked) return;
        var r = MessageBox.Show(this,
            "勾选后，恢复时目标目录里的同名文件会被覆盖。\n被覆盖的旧文件不会真的删除，会先移到备份库的 trash 目录里，事后可以找回。\n\n确定要覆盖吗？",
            "时间胶囊", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (r != DialogResult.Yes) chkOverwrite.Checked = false;
    }

    private void BtnRestore_Click(object? sender, EventArgs e)
    {
        StartRestore(null);
    }

    private void BtnRestoreOne_Click(object? sender, EventArgs e)
    {
        if (dgvFiles.CurrentRow?.DataBoundItem is not FileRow row)
        {
            MessageBox.Show(this, "请先在下面的列表里选一个文件。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        StartRestore(new List<string> { row.Path });
    }

    private async void StartRestore(List<string>? onlyPaths)
    {
        if (_vault == null || _restore == null || _currentManifest == null)
        {
            MessageBox.Show(this, "请先在上方选择一个时间点。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (_busy)
        {
            MessageBox.Show(this, "正在忙别的任务，请稍等。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var paths = onlyPaths ?? SelectedPaths();
        if (paths.Count == 0) paths = null;

        string target = txtTarget.Text.Trim();
        if (target.Length == 0)
        {
            target = SuggestTarget();
            if (target.Length == 0)
            {
                MessageBox.Show(this, "请先选择恢复目标目录。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txtTarget.Text = target;
        }

        int count = paths?.Count ?? _currentManifest.Entries.Count;
        string overwriteText = chkOverwrite.Checked ? "同名文件会被覆盖（旧文件先移到回收目录）。" : "同名文件会被跳过，不会覆盖。";
        var confirm = MessageBox.Show(this,
            $"把 {count} 个文件恢复到：\n{target}\n\n{overwriteText}\n\n开始恢复吗？",
            "确认恢复", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        _busy = true;
        btnRestore.Enabled = false;
        btnRestoreOne.Enabled = false;
        progressBar.Style = ProgressBarStyle.Marquee;
        SetStatus("正在恢复…", UiTheme.Orange);

        IProgress<string> progress = new Progress<string>(text => { lblRestoreHint.Text = text; });
        try
        {
            var options = new RestoreOptions
            {
                SnapshotId = _currentManifest.Id,
                TargetRoot = target,
                OnlyPaths = paths,
                OverwriteExisting = chkOverwrite.Checked
            };

            var result = await Task.Run(() =>
                _restore!.Restore(options, text => progress.Report(text)));

            SetStatus($"恢复完成：成功 {result.Restored}，跳过 {result.Skipped}，失败 {result.Failed}", result.Failed > 0 ? UiTheme.Orange : UiTheme.Green);
            UpdateRestoreHint();

            string msg = $"恢复完成。\n\n成功 {result.Restored} 个\n跳过 {result.Skipped} 个（目标里已存在同名文件）\n失败 {result.Failed} 个\n\n目标目录：{result.TargetRoot}";
            if (result.TrashDir != null && Directory.Exists(result.TrashDir))
            {
                msg += "\n\n被覆盖的旧文件已移到：\n" + result.TrashDir;
            }
            if (result.Errors.Count > 0)
            {
                msg += "\n\n前几条失败原因：\n" + string.Join("\n", result.Errors.Take(5));
            }

            var ask = MessageBox.Show(this, msg + "\n\n要现在打开目标目录吗？", "时间胶囊",
                MessageBoxButtons.YesNo, result.Failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            if (ask == DialogResult.Yes && Directory.Exists(result.TargetRoot))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "\"" + result.TargetRoot + "\"") { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            SetStatus("恢复失败：" + ex.Message, UiTheme.Red);
            UpdateRestoreHint();
            MessageBox.Show(this, "恢复失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnRestore.Enabled = true;
            btnRestoreOne.Enabled = true;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            UpdateStatusBar();
        }
    }

    // ─────────────────────────── 托盘与窗口 ───────────────────────────

    private void BuildTray()
    {
        if (_tray != null) return;
        _tray = new NotifyIcon
        {
            Icon = BuildAppIcon(),
            Text = "时间胶囊",
            Visible = true
        };
        var menu = new ContextMenuStrip();
        menu.Items.Add("打开主界面", null, TrayOpen_Click);
        menu.Items.Add("立即备份", null, TrayBackup_Click);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("退出", null, TrayExit_Click);
        _tray.ContextMenuStrip = menu;
        _tray.DoubleClick += TrayOpen_Click;
    }

    private static Icon BuildAppIcon()
    {
        using var bmp = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            using var back = new SolidBrush(UiTheme.Accent);
            g.FillEllipse(back, 1, 1, 30, 30);
            using var body = new SolidBrush(Color.White);
            g.FillRectangle(body, 9, 12, 14, 9);
            g.FillEllipse(body, 6, 10, 8, 13);
            g.FillEllipse(body, 18, 10, 8, 13);
            using var cut = new SolidBrush(UiTheme.Accent);
            g.FillEllipse(cut, 12, 14, 8, 5);
        }
        return Icon.FromHandle(bmp.GetHicon());
    }

    private void TrayOpen_Click(object? sender, EventArgs e)
    {
        ShowFromTray();
    }

    private void TrayBackup_Click(object? sender, EventArgs e)
    {
        StartBackup("manual");
    }

    private void TrayExit_Click(object? sender, EventArgs e)
    {
        _reallyExit = true;
        Close();
    }

    private void ShowFromTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_vault != null && _vault.Settings.MinimizeToTray && !_reallyExit && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            if (!_trayHintShown && _tray != null)
            {
                _trayHintShown = true;
                _tray.ShowBalloonTip(3000, "时间胶囊", "已最小化到托盘，监控仍在继续。右键托盘图标可以退出。", ToolTipIcon.Info);
            }
            return;
        }

        _app.WindowWidth = Width;
        _app.WindowHeight = Height;
        if (_persistState)
        {
            if (_vault != null) _app.LastVaultPath = _vault.Root;
            _app.Save();
        }

        _tray?.Dispose();
        _tray = null;
        _monitor?.Dispose();
        _monitor = null;
    }

    private void FrmMain_Resize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized && _vault != null && _vault.Settings.MinimizeToTray)
        {
            Hide();
        }
    }

    private void SplitMain_SplitterMoved(object? sender, SplitterEventArgs e)
    {
        // 用户自己拖过分隔条之后就尊重他的选择，不再按内容自动收放
        if (!DesignMode) _splitterUserAdjusted = true;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AutoFitSplitter();
    }

    /// <summary>几何自检：同父控件重叠 / 越界。</summary>
    public List<string> CheckGeometry()
    {
        var issues = new List<string>();
        var parents = new Control[] { panelTop, panelBottom, splitMain.Panel1, splitMain.Panel2, panelDetail };
        foreach (var parent in parents)
        {
            var kids = parent.Controls.Cast<Control>().Where(c => c.Visible).ToList();
            foreach (var c in kids)
            {
                if (c.Left < 0 || c.Top < 0 || c.Right > parent.ClientSize.Width || c.Bottom > parent.ClientSize.Height)
                {
                    issues.Add($"{parent.Name}/{c.Name} 越界：{c.Bounds} 父尺寸 {parent.ClientSize}");
                }
            }
            for (int i = 0; i < kids.Count; i++)
            {
                for (int j = i + 1; j < kids.Count; j++)
                {
                    if (kids[i].Bounds.IntersectsWith(kids[j].Bounds))
                    {
                        issues.Add($"{parent.Name} 内 {kids[i].Name} 与 {kids[j].Name} 重叠：{kids[i].Bounds} / {kids[j].Bounds}");
                    }
                }
            }
        }
        return issues;
    }
}
