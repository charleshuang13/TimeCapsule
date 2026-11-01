using System.Text;
using TimeCapsule.Core;

namespace TimeCapsule.UI;

public sealed partial class FrmSettings : Form
{
    private Vault? _vault;
    private bool _busy;

    public bool Saved { get; private set; }

    /// <summary>设计器需要的无参构造函数。</summary>
    public FrmSettings()
    {
        InitializeComponent();
        cmbRetain.Items.Add("全部保留，不自动删除");
        cmbRetain.Items.Add("按天数保留");
        cmbRetain.Items.Add("按库容量上限保留");
        cmbRetain.SelectedIndex = 0;
        UpdateRetainUi();
    }

    public FrmSettings(Vault vault) : this()
    {
        _vault = vault;
        LoadValues();
        RefreshVaultInfo();
    }

    private void LoadValues()
    {
        if (_vault == null) return;
        var s = _vault.Settings;
        txtSrc.Text = s.SourceRoot;
        numInterval.Value = Clamp(s.IntervalMinutes, (int)numInterval.Minimum, (int)numInterval.Maximum);
        numDebounce.Value = Clamp(s.DebounceSeconds, (int)numDebounce.Minimum, (int)numDebounce.Maximum);
        numMaxFile.Value = Clamp(s.MaxFileSizeMB, (int)numMaxFile.Minimum, (int)numMaxFile.Maximum);

        cmbRetain.SelectedIndex = s.RetainMode switch
        {
            "days" => 1,
            "totalSize" => 2,
            _ => 0
        };

        if (s.RetainMode == "totalSize")
        {
            numRetain.Value = Clamp((int)Math.Max(1, s.RetainTotalMB / 1024), (int)numRetain.Minimum, (int)numRetain.Maximum);
        }
        else
        {
            numRetain.Value = Clamp(s.RetainDays, (int)numRetain.Minimum, (int)numRetain.Maximum);
        }

        txtExcDir.Text = string.Join("\r\n", s.ExcludeDirs);
        txtExcPat.Text = string.Join("\r\n", s.ExcludePatterns);

        chkTray.Checked = s.MinimizeToTray;
        chkNotify.Checked = s.NotifyOnBackup;
        chkStartup.Checked = s.StartWithWindows;

        UpdateRetainUi();
    }

    private static decimal Clamp(int value, decimal min, decimal max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private void UpdateRetainUi()
    {
        switch (cmbRetain.SelectedIndex)
        {
            case 1:
                numRetain.Enabled = true;
                lblRetainUnit.Text = "天";
                lblRetainHint.Text = "只保留最近这么多天的快照，更早的自动删除（数据块也会一并回收）。最新的一张永远保留。";
                break;
            case 2:
                numRetain.Enabled = true;
                lblRetainUnit.Text = "GB";
                lblRetainHint.Text = "库的原始文件总量超过这个值后，从最旧的快照开始删。最新的一张永远保留。";
                break;
            default:
                numRetain.Enabled = false;
                lblRetainUnit.Text = "";
                lblRetainHint.Text = "全部保留：一张都不删，容量会随时间增长。这是最安全的选择，需要清理时手动处理。";
                break;
        }
    }

    private void RefreshVaultInfo()
    {
        if (_vault == null) return;
        var blobs = new BlobStore(_vault);
        var snaps = new SnapshotStore(_vault);
        var sb = new StringBuilder();
        sb.Append("库位置：").Append(_vault.Root).Append("\r\n");
        sb.Append("快照 ").Append(snaps.Count()).Append(" 张 · 数据块 ")
          .Append(blobs.CountBlobs()).Append(" 个 · 加密后占用 ").Append(Humanize.Size(blobs.TotalBlobBytes()));
        lblVaultInfo.Text = sb.ToString();
    }

    private void CmbRetain_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateRetainUi();
    }

    private void BtnSrc_Click(object? sender, EventArgs e)
    {
        if (_vault == null) return;
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择要监控并备份的目录",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(txtSrc.Text) ? txtSrc.Text : ""
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            if (string.Equals(Path.GetFullPath(dlg.SelectedPath).TrimEnd('\\'),
                    Path.GetFullPath(_vault.Root).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(this, "不能监控备份库自己。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txtSrc.Text = dlg.SelectedPath;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (_busy || _vault == null) return;

        string src = txtSrc.Text.Trim();
        if (src.Length == 0 || !Directory.Exists(src))
        {
            MessageBox.Show(this, "监控目录不存在，请重新选择。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var s = _vault.Settings;
        s.SourceRoot = src;
        s.IntervalMinutes = (int)numInterval.Value;
        s.DebounceSeconds = (int)numDebounce.Value;
        s.MaxFileSizeMB = (int)numMaxFile.Value;

        s.RetainMode = cmbRetain.SelectedIndex switch
        {
            1 => "days",
            2 => "totalSize",
            _ => "keepAll"
        };
        if (s.RetainMode == "days") s.RetainDays = (int)numRetain.Value;
        if (s.RetainMode == "totalSize") s.RetainTotalMB = (long)numRetain.Value * 1024;

        s.MinimizeToTray = chkTray.Checked;
        s.NotifyOnBackup = chkNotify.Checked;
        s.StartWithWindows = chkStartup.Checked;
        s.ExcludeDirs = SplitLines(txtExcDir.Text);
        s.ExcludePatterns = SplitLines(txtExcPat.Text);

        try
        {
            _vault.SaveConfig();
            Startup.Set(s.StartWithWindows, Application.ExecutablePath);
            Saved = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "保存失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static List<string> SplitLines(string text)
    {
        var list = new List<string>();
        foreach (var line in text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string t = line.Trim();
            if (t.Length > 0) list.Add(t);
        }
        return list;
    }

    private async void BtnClean_Click(object? sender, EventArgs e)
    {
        if (_busy || _vault == null) return;
        var confirm = MessageBox.Show(this,
            "会扫描所有快照，把没有任何快照引用的数据块删掉。\n正常情况下不会有孤儿块，只有在异常中断后才需要做这个。\n\n继续吗？",
            "时间胶囊", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        _busy = true;
        btnClean.Enabled = false;
        btnClean.Text = "正在清理…";
        try
        {
            var blobs = new BlobStore(_vault);
            var snaps = new SnapshotStore(_vault);
            int removed = await Task.Run(() => blobs.CollectGarbage(snaps));
            RefreshVaultInfo();
            MessageBox.Show(this, "清理完成，删除了 " + removed + " 个无用数据块。", "时间胶囊",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "清理失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnClean.Enabled = true;
            btnClean.Text = "清理无用数据块";
        }
    }

    private async void BtnCheck_Click(object? sender, EventArgs e)
    {
        if (_busy || _vault == null) return;
        var confirm = MessageBox.Show(this,
            "完整检查会把库里每个数据块都解密一遍校验指纹，文件多、库大的时候可能要好几分钟。\n\n现在开始吗？",
            "时间胶囊", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        _busy = true;
        btnCheck.Enabled = false;
        string original = btnCheck.Text;
        btnCheck.Text = "正在检查…";
        lblVaultInfo.Text = "正在逐个解密校验数据块，请稍候…";

        try
        {
            var blobs = new BlobStore(_vault);
            var snaps = new SnapshotStore(_vault);

            var result = await Task.Run(() =>
            {
                var hashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int brokenManifest = 0;
                foreach (var info in snaps.List())
                {
                    var m = snaps.Load(info.Id);
                    if (m == null) { brokenManifest++; continue; }
                    foreach (var entry in m.Entries) hashes.Add(entry.Hash);
                }

                int ok = 0, missing = 0, corrupt = 0;
                string temp = Path.Combine(Path.GetTempPath(), "tc-check-" + Guid.NewGuid().ToString("N")[..8] + ".tmp");
                foreach (var hash in hashes)
                {
                    if (!blobs.Has(hash)) { missing++; continue; }
                    try
                    {
                        blobs.Get(hash, temp, CancellationToken.None);
                        ok++;
                    }
                    catch
                    {
                        corrupt++;
                    }
                }
                try { if (File.Exists(temp)) File.Delete(temp); } catch { /* 忽略 */ }

                return (ok, missing, corrupt, brokenManifest, hashes.Count);
            });

            RefreshVaultInfo();
            string msg = $"检查完成。\n\n引用的数据块 {result.Item5} 个\n正常 {result.ok} 个\n缺失 {result.missing} 个\n损坏 {result.corrupt} 个\n读不出的快照 {result.Item4} 张";
            if (result.missing == 0 && result.corrupt == 0 && result.Item4 == 0)
            {
                msg += "\n\n库是完好的。";
            }
            else
            {
                msg += "\n\n凡是缺失或损坏的文件都没法恢复，请尽早检查备份策略。";
            }
            MessageBox.Show(this, msg, "时间胶囊", MessageBoxButtons.OK,
                result.missing == 0 && result.corrupt == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "检查失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnCheck.Enabled = true;
            btnCheck.Text = original;
        }
    }
}
