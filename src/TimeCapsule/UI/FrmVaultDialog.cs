using TimeCapsule.Core;

namespace TimeCapsule.UI;

/// <summary>建库 / 解锁对话框。设计器里看到的就是「新建」模式的排版；「打开」模式在运行时收紧高度。</summary>
public sealed partial class FrmVaultDialog : Form
{
    public enum DialogMode
    {
        Create,
        Open
    }

    private DialogMode _mode = DialogMode.Create;
    private bool _busy;

    public Vault? Vault { get; private set; }

    /// <summary>设计器需要的无参构造函数。</summary>
    public FrmVaultDialog()
    {
        InitializeComponent();
    }

    public FrmVaultDialog(DialogMode mode, string? vaultPath, string? sourceRoot) : this()
    {
        _mode = mode;
        txtLib.Text = vaultPath ?? "";
        txtSrc.Text = sourceRoot ?? "";
    }

    /// <summary>启动入口：能自动打开就直接进，否则弹窗。</summary>
    public static Vault? ShowStartup(AppState app)
    {
        if (!string.IsNullOrWhiteSpace(app.LastVaultPath) && Vault.LooksLikeVault(app.LastVaultPath))
        {
            string? saved = CredentialStore.Load(app.LastVaultPath);
            if (saved != null)
            {
                try
                {
                    return Vault.Open(app.LastVaultPath, saved);
                }
                catch (BadPasswordException)
                {
                    // 密码已经不对了，退回手动输入
                }
                catch
                {
                    // 库读不了，退回手动
                }
            }
            using var open = new FrmVaultDialog(DialogMode.Open, app.LastVaultPath, null);
            return open.ShowDialog() == DialogResult.OK ? open.Vault : null;
        }

        using var create = new FrmVaultDialog(DialogMode.Create, DefaultVaultPath(), null);
        return create.ShowDialog() == DialogResult.OK ? create.Vault : null;
    }

    public static string DefaultVaultPath()
    {
        try
        {
            var d = DriveInfo.GetDrives().FirstOrDefault(x =>
                x.DriveType == DriveType.Fixed && x.IsReady &&
                string.Equals(x.Name, "D:\\", StringComparison.OrdinalIgnoreCase));
            if (d != null) return Path.Combine(d.RootDirectory.FullName, "TimeCapsule");
        }
        catch { /* 忽略 */ }
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "时间胶囊库");
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (DesignMode) return;
        ApplyMode(_mode);
    }

    /// <summary>
    /// 只调整「打开」模式的差异；「新建」模式完全使用设计器里的坐标，
    /// 这样在 VS 设计器里拖控件改出来的位置就是运行时看到的位置。
    /// </summary>
    private void ApplyMode(DialogMode mode)
    {
        _mode = mode;
        bool create = mode == DialogMode.Create;

        lblPwd2.Visible = create;
        txtPwd2.Visible = create;
        lblSrc.Visible = create;
        txtSrc.Visible = create;
        btnSrc.Visible = create;

        if (create)
        {
            Text = "新建时间胶囊库";
            lblHint.Text = "先设定一个主密码。以后打开软件会自动使用它，不用每次输入；密码用 Windows 账户加密保存在本机，换台电脑打不开。";
            lblHint.Size = new Size(512, 44);
            lblNote.Location = new Point(24, 214);
            lblNote.Size = new Size(512, 40);
            lblNote.Text = "监控目录里所有文件（可按规则排除）都会被加密备份，原目录里的文件不会被改动。";
            btnOk.Text = "创建并开始备份";
            btnOk.Location = new Point(318, 268);
            btnCancel.Location = new Point(446, 268);
            ClientSize = new Size(560, 316);
        }
        else
        {
            Text = "打开时间胶囊库";
            lblHint.Text = "输入这个备份库的主密码。";
            lblHint.Size = new Size(512, 22);
            lblNote.Text = "密码只用于本机解密，不会上传到任何地方。";
            lblNote.Location = new Point(24, 158);
            lblNote.Size = new Size(512, 22);
            btnOk.Text = "打开库";
            btnOk.Location = new Point(318, 196);
            btnCancel.Location = new Point(446, 196);
            ClientSize = new Size(560, 244);
        }
    }

    private void RestorePrimaryText()
    {
        btnOk.Text = _mode == DialogMode.Create ? "创建并开始备份" : "打开库";
    }

    /// <summary>自动化测试用：填入表单并触发创建/打开按钮。</summary>
    internal void AutoFillForTest(string vaultPath, string password, string? sourceRoot)
    {
        txtLib.Text = vaultPath;
        txtPwd.Text = password;
        txtPwd2.Text = password;
        if (sourceRoot != null) txtSrc.Text = sourceRoot;
    }

    internal void ClickPrimaryForTest()
    {
        btnOk.PerformClick();
    }

    private void BtnLib_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择备份库放在哪里（会在这个目录下建立 blobs 和 snaps 两个子目录）",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(txtLib.Text) ? txtLib.Text : DefaultVaultPath()
        };
        if (dlg.ShowDialog(this) == DialogResult.OK) txtLib.Text = dlg.SelectedPath;
    }

    private void BtnSrc_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择要监控并备份的目录",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(txtSrc.Text) ? txtSrc.Text : ""
        };
        if (dlg.ShowDialog(this) == DialogResult.OK) txtSrc.Text = dlg.SelectedPath;
    }

    private async void BtnOk_Click(object sender, EventArgs e)
    {
        if (_busy) return;

        string lib = txtLib.Text.Trim();
        string pwd = txtPwd.Text;

        if (lib.Length == 0)
        {
            MessageBox.Show(this, "请先选择备份库的位置。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_mode == DialogMode.Create)
        {
            if (Vault.LooksLikeVault(lib))
            {
                var r = MessageBox.Show(this,
                    "这个目录里已经有一个备份库了，要直接打开它吗？\n（选“否”请另选一个空目录新建）",
                    "时间胶囊", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    ApplyMode(DialogMode.Open);
                    txtLib.Text = lib;
                    txtPwd.Focus();
                }
                return;
            }

            if (pwd.Length < 4)
            {
                MessageBox.Show(this, "主密码至少 4 位。忘记了将无法解开备份，请务必记牢。", "时间胶囊",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPwd.Focus();
                return;
            }

            if (pwd != txtPwd2.Text)
            {
                MessageBox.Show(this, "两次输入的密码不一致。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPwd2.Focus();
                return;
            }

            string src = txtSrc.Text.Trim();
            if (src.Length == 0 || !Directory.Exists(src))
            {
                MessageBox.Show(this, "请选择一个存在的监控目录。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ConfirmVaultOutsideSource(lib, src)) return;

            await CreateAsync(lib, pwd, src);
        }
        else
        {
            if (pwd.Length == 0)
            {
                MessageBox.Show(this, "请输入密码。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPwd.Focus();
                return;
            }
            await OpenAsync(lib, pwd);
        }
    }

    private bool ConfirmVaultOutsideSource(string lib, string src)
    {
        string l = Path.GetFullPath(lib).TrimEnd('\\') + "\\";
        string s = Path.GetFullPath(src).TrimEnd('\\') + "\\";
        if (!l.StartsWith(s, StringComparison.OrdinalIgnoreCase)) return true;
        var r = MessageBox.Show(this,
            "备份库位于监控目录内部。程序虽然会自动跳过库目录，但仍建议把库放到别的位置，避免误删源目录时连库一起丢失。\n\n继续吗？",
            "时间胶囊", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        return r == DialogResult.Yes;
    }

    private async Task CreateAsync(string lib, string pwd, string src)
    {
        _busy = true;
        btnOk.Enabled = false;
        btnOk.Text = "正在创建…";
        try
        {
            var vault = await Task.Run(() => Vault.Create(lib, pwd, src));
            CredentialStore.Save(lib, pwd);
            Vault = vault;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "创建失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnOk.Enabled = true;
            RestorePrimaryText();
        }
    }

    private async Task OpenAsync(string lib, string pwd)
    {
        _busy = true;
        btnOk.Enabled = false;
        btnOk.Text = "正在打开…";
        try
        {
            var vault = await Task.Run(() => Vault.Open(lib, pwd));
            CredentialStore.Save(lib, pwd);
            Vault = vault;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (BadPasswordException)
        {
            MessageBox.Show(this, "密码不对，打不开这个库。", "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPwd.SelectAll();
            txtPwd.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "打开失败：" + ex.Message, "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            btnOk.Enabled = true;
            RestorePrimaryText();
        }
    }
}
