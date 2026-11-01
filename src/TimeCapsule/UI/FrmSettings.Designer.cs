namespace TimeCapsule.UI;

partial class FrmSettings
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblSec1 = new System.Windows.Forms.Label();
        this.lblSrc = new System.Windows.Forms.Label();
        this.txtSrc = new System.Windows.Forms.TextBox();
        this.btnSrc = new System.Windows.Forms.Button();
        this.lblInterval = new System.Windows.Forms.Label();
        this.numInterval = new System.Windows.Forms.NumericUpDown();
        this.lblIntervalUnit = new System.Windows.Forms.Label();
        this.lblDebounce = new System.Windows.Forms.Label();
        this.numDebounce = new System.Windows.Forms.NumericUpDown();
        this.lblDebounceUnit = new System.Windows.Forms.Label();
        this.lblMaxFile = new System.Windows.Forms.Label();
        this.numMaxFile = new System.Windows.Forms.NumericUpDown();
        this.lblMaxFileUnit = new System.Windows.Forms.Label();
        this.lblSec2 = new System.Windows.Forms.Label();
        this.cmbRetain = new System.Windows.Forms.ComboBox();
        this.numRetain = new System.Windows.Forms.NumericUpDown();
        this.lblRetainUnit = new System.Windows.Forms.Label();
        this.lblRetainHint = new System.Windows.Forms.Label();
        this.lblSec3 = new System.Windows.Forms.Label();
        this.lblExcDir = new System.Windows.Forms.Label();
        this.txtExcDir = new System.Windows.Forms.TextBox();
        this.lblExcPat = new System.Windows.Forms.Label();
        this.txtExcPat = new System.Windows.Forms.TextBox();
        this.lblSec4 = new System.Windows.Forms.Label();
        this.chkTray = new System.Windows.Forms.CheckBox();
        this.chkNotify = new System.Windows.Forms.CheckBox();
        this.chkStartup = new System.Windows.Forms.CheckBox();
        this.lblSec5 = new System.Windows.Forms.Label();
        this.lblVaultInfo = new System.Windows.Forms.Label();
        this.btnCheck = new System.Windows.Forms.Button();
        this.btnClean = new System.Windows.Forms.Button();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numDebounce)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxFile)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numRetain)).BeginInit();
        this.SuspendLayout();
        //
        // lblTitle
        //
        this.lblTitle.Location = new System.Drawing.Point(20, 14);
        this.lblTitle.Size = new System.Drawing.Size(320, 26);
        this.lblTitle.Text = "设置";
        this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // lblSec1
        //
        this.lblSec1.Location = new System.Drawing.Point(20, 50);
        this.lblSec1.Size = new System.Drawing.Size(580, 22);
        this.lblSec1.Text = "备份策略";
        this.lblSec1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblSec1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        //
        // lblSrc
        //
        this.lblSrc.Location = new System.Drawing.Point(20, 82);
        this.lblSrc.Size = new System.Drawing.Size(68, 22);
        this.lblSrc.Text = "监控目录";
        this.lblSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // txtSrc
        //
        this.txtSrc.Location = new System.Drawing.Point(96, 81);
        this.txtSrc.Size = new System.Drawing.Size(400, 23);
        this.txtSrc.ReadOnly = true;
        this.txtSrc.BackColor = System.Drawing.Color.FromArgb(247, 248, 250);
        //
        // btnSrc
        //
        this.btnSrc.Location = new System.Drawing.Point(504, 80);
        this.btnSrc.Size = new System.Drawing.Size(100, 26);
        this.btnSrc.Text = "更换目录…";
        this.btnSrc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSrc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnSrc.FlatAppearance.BorderSize = 1;
        this.btnSrc.BackColor = System.Drawing.Color.White;
        this.btnSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnSrc.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // lblInterval
        //
        this.lblInterval.Location = new System.Drawing.Point(20, 118);
        this.lblInterval.Size = new System.Drawing.Size(68, 22);
        this.lblInterval.Text = "备份间隔";
        this.lblInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // numInterval
        //
        this.numInterval.Location = new System.Drawing.Point(96, 117);
        this.numInterval.Size = new System.Drawing.Size(70, 23);
        this.numInterval.Minimum = 1;
        this.numInterval.Maximum = 1440;
        //
        // lblIntervalUnit
        //
        this.lblIntervalUnit.Location = new System.Drawing.Point(170, 118);
        this.lblIntervalUnit.Size = new System.Drawing.Size(34, 22);
        this.lblIntervalUnit.Text = "分钟";
        this.lblIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // lblDebounce
        //
        this.lblDebounce.Location = new System.Drawing.Point(220, 118);
        this.lblDebounce.Size = new System.Drawing.Size(76, 22);
        this.lblDebounce.Text = "变化后延迟";
        this.lblDebounce.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // numDebounce
        //
        this.numDebounce.Location = new System.Drawing.Point(300, 117);
        this.numDebounce.Size = new System.Drawing.Size(58, 23);
        this.numDebounce.Minimum = 1;
        this.numDebounce.Maximum = 120;
        //
        // lblDebounceUnit
        //
        this.lblDebounceUnit.Location = new System.Drawing.Point(360, 118);
        this.lblDebounceUnit.Size = new System.Drawing.Size(30, 22);
        this.lblDebounceUnit.Text = "秒";
        this.lblDebounceUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // lblMaxFile
        //
        this.lblMaxFile.Location = new System.Drawing.Point(404, 118);
        this.lblMaxFile.Size = new System.Drawing.Size(76, 22);
        this.lblMaxFile.Text = "单文件上限";
        this.lblMaxFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // numMaxFile
        //
        this.numMaxFile.Location = new System.Drawing.Point(484, 117);
        this.numMaxFile.Size = new System.Drawing.Size(80, 23);
        this.numMaxFile.Maximum = 1000000;
        //
        // lblMaxFileUnit
        //
        this.lblMaxFileUnit.Location = new System.Drawing.Point(566, 118);
        this.lblMaxFileUnit.Size = new System.Drawing.Size(38, 22);
        this.lblMaxFileUnit.Text = "MB";
        this.lblMaxFileUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // lblSec2
        //
        this.lblSec2.Location = new System.Drawing.Point(20, 154);
        this.lblSec2.Size = new System.Drawing.Size(580, 22);
        this.lblSec2.Text = "快照保留策略";
        this.lblSec2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblSec2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        //
        // cmbRetain
        //
        this.cmbRetain.Location = new System.Drawing.Point(20, 182);
        this.cmbRetain.Size = new System.Drawing.Size(190, 23);
        this.cmbRetain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        //
        // numRetain
        //
        this.numRetain.Location = new System.Drawing.Point(222, 182);
        this.numRetain.Size = new System.Drawing.Size(80, 23);
        this.numRetain.Minimum = 1;
        this.numRetain.Maximum = 100000;
        //
        // lblRetainUnit
        //
        this.lblRetainUnit.Location = new System.Drawing.Point(308, 183);
        this.lblRetainUnit.Size = new System.Drawing.Size(80, 22);
        this.lblRetainUnit.Text = "天";
        this.lblRetainUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // lblRetainHint
        //
        this.lblRetainHint.Location = new System.Drawing.Point(20, 212);
        this.lblRetainHint.Size = new System.Drawing.Size(584, 40);
        this.lblRetainHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblRetainHint.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblRetainHint.Text = "";
        //
        // lblSec3
        //
        this.lblSec3.Location = new System.Drawing.Point(20, 250);
        this.lblSec3.Size = new System.Drawing.Size(580, 22);
        this.lblSec3.Text = "排除规则";
        this.lblSec3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblSec3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        //
        // lblExcDir
        //
        this.lblExcDir.Location = new System.Drawing.Point(20, 278);
        this.lblExcDir.Size = new System.Drawing.Size(280, 20);
        this.lblExcDir.Text = "排除的目录名（每行一个）";
        this.lblExcDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblExcDir.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // txtExcDir
        //
        this.txtExcDir.Location = new System.Drawing.Point(20, 300);
        this.txtExcDir.Size = new System.Drawing.Size(280, 92);
        this.txtExcDir.Multiline = true;
        this.txtExcDir.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtExcDir.AcceptsReturn = true;
        //
        // lblExcPat
        //
        this.lblExcPat.Location = new System.Drawing.Point(316, 278);
        this.lblExcPat.Size = new System.Drawing.Size(288, 20);
        this.lblExcPat.Text = "排除的文件名（支持 * 通配符）";
        this.lblExcPat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblExcPat.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // txtExcPat
        //
        this.txtExcPat.Location = new System.Drawing.Point(316, 300);
        this.txtExcPat.Size = new System.Drawing.Size(288, 92);
        this.txtExcPat.Multiline = true;
        this.txtExcPat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtExcPat.AcceptsReturn = true;
        //
        // lblSec4
        //
        this.lblSec4.Location = new System.Drawing.Point(20, 406);
        this.lblSec4.Size = new System.Drawing.Size(580, 22);
        this.lblSec4.Text = "运行方式";
        this.lblSec4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblSec4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        //
        // chkTray
        //
        this.chkTray.Location = new System.Drawing.Point(20, 434);
        this.chkTray.Size = new System.Drawing.Size(400, 22);
        this.chkTray.Text = "关闭窗口时最小化到托盘，监控不中断";
        //
        // chkNotify
        //
        this.chkNotify.Location = new System.Drawing.Point(20, 460);
        this.chkNotify.Size = new System.Drawing.Size(400, 22);
        this.chkNotify.Text = "每次备份完成后弹出提示";
        //
        // chkStartup
        //
        this.chkStartup.Location = new System.Drawing.Point(20, 486);
        this.chkStartup.Size = new System.Drawing.Size(400, 22);
        this.chkStartup.Text = "开机自动启动（登录后最小化到托盘）";
        //
        // lblSec5
        //
        this.lblSec5.Location = new System.Drawing.Point(20, 520);
        this.lblSec5.Size = new System.Drawing.Size(580, 22);
        this.lblSec5.Text = "库信息";
        this.lblSec5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblSec5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        //
        // lblVaultInfo
        //
        this.lblVaultInfo.Location = new System.Drawing.Point(20, 546);
        this.lblVaultInfo.Size = new System.Drawing.Size(584, 56);
        this.lblVaultInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblVaultInfo.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblVaultInfo.Text = "";
        //
        // btnCheck
        //
        this.btnCheck.Location = new System.Drawing.Point(20, 610);
        this.btnCheck.Size = new System.Drawing.Size(128, 30);
        this.btnCheck.Text = "检查库完整性";
        this.btnCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnCheck.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnCheck.FlatAppearance.BorderSize = 1;
        this.btnCheck.BackColor = System.Drawing.Color.White;
        this.btnCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnCheck.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnClean
        //
        this.btnClean.Location = new System.Drawing.Point(156, 610);
        this.btnClean.Size = new System.Drawing.Size(140, 30);
        this.btnClean.Text = "清理无用数据块";
        this.btnClean.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnClean.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnClean.FlatAppearance.BorderSize = 1;
        this.btnClean.BackColor = System.Drawing.Color.White;
        this.btnClean.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnClean.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnSave
        //
        this.btnSave.Location = new System.Drawing.Point(400, 610);
        this.btnSave.Size = new System.Drawing.Size(100, 30);
        this.btnSave.Text = "保存";
        this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSave.FlatAppearance.BorderSize = 0;
        this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        this.btnSave.ForeColor = System.Drawing.Color.White;
        this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnCancel
        //
        this.btnCancel.Location = new System.Drawing.Point(508, 610);
        this.btnCancel.Size = new System.Drawing.Size(100, 30);
        this.btnCancel.Text = "取消";
        this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnCancel.FlatAppearance.BorderSize = 1;
        this.btnCancel.BackColor = System.Drawing.Color.White;
        this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        //
        // FrmSettings
        //
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.ClientSize = new System.Drawing.Size(624, 656);
        this.BackColor = System.Drawing.Color.White;
        this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "时间胶囊 · 设置";
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.lblSec1);
        this.Controls.Add(this.lblSrc);
        this.Controls.Add(this.txtSrc);
        this.Controls.Add(this.btnSrc);
        this.Controls.Add(this.lblInterval);
        this.Controls.Add(this.numInterval);
        this.Controls.Add(this.lblIntervalUnit);
        this.Controls.Add(this.lblDebounce);
        this.Controls.Add(this.numDebounce);
        this.Controls.Add(this.lblDebounceUnit);
        this.Controls.Add(this.lblMaxFile);
        this.Controls.Add(this.numMaxFile);
        this.Controls.Add(this.lblMaxFileUnit);
        this.Controls.Add(this.lblSec2);
        this.Controls.Add(this.cmbRetain);
        this.Controls.Add(this.numRetain);
        this.Controls.Add(this.lblRetainUnit);
        this.Controls.Add(this.lblRetainHint);
        this.Controls.Add(this.lblSec3);
        this.Controls.Add(this.lblExcDir);
        this.Controls.Add(this.txtExcDir);
        this.Controls.Add(this.lblExcPat);
        this.Controls.Add(this.txtExcPat);
        this.Controls.Add(this.lblSec4);
        this.Controls.Add(this.chkTray);
        this.Controls.Add(this.chkNotify);
        this.Controls.Add(this.chkStartup);
        this.Controls.Add(this.lblSec5);
        this.Controls.Add(this.lblVaultInfo);
        this.Controls.Add(this.btnCheck);
        this.Controls.Add(this.btnClean);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.btnCancel);
        this.AcceptButton = this.btnSave;
        this.CancelButton = this.btnCancel;
        this.btnSrc.Click += new System.EventHandler(this.BtnSrc_Click);
        this.btnCheck.Click += new System.EventHandler(this.BtnCheck_Click);
        this.btnClean.Click += new System.EventHandler(this.BtnClean_Click);
        this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
        this.cmbRetain.SelectedIndexChanged += new System.EventHandler(this.CmbRetain_SelectedIndexChanged);
        ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numDebounce)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxFile)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numRetain)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblSec1;
    private System.Windows.Forms.Label lblSrc;
    private System.Windows.Forms.TextBox txtSrc;
    private System.Windows.Forms.Button btnSrc;
    private System.Windows.Forms.Label lblInterval;
    private System.Windows.Forms.NumericUpDown numInterval;
    private System.Windows.Forms.Label lblIntervalUnit;
    private System.Windows.Forms.Label lblDebounce;
    private System.Windows.Forms.NumericUpDown numDebounce;
    private System.Windows.Forms.Label lblDebounceUnit;
    private System.Windows.Forms.Label lblMaxFile;
    private System.Windows.Forms.NumericUpDown numMaxFile;
    private System.Windows.Forms.Label lblMaxFileUnit;
    private System.Windows.Forms.Label lblSec2;
    private System.Windows.Forms.ComboBox cmbRetain;
    private System.Windows.Forms.NumericUpDown numRetain;
    private System.Windows.Forms.Label lblRetainUnit;
    private System.Windows.Forms.Label lblRetainHint;
    private System.Windows.Forms.Label lblSec3;
    private System.Windows.Forms.Label lblExcDir;
    private System.Windows.Forms.TextBox txtExcDir;
    private System.Windows.Forms.Label lblExcPat;
    private System.Windows.Forms.TextBox txtExcPat;
    private System.Windows.Forms.Label lblSec4;
    private System.Windows.Forms.CheckBox chkTray;
    private System.Windows.Forms.CheckBox chkNotify;
    private System.Windows.Forms.CheckBox chkStartup;
    private System.Windows.Forms.Label lblSec5;
    private System.Windows.Forms.Label lblVaultInfo;
    private System.Windows.Forms.Button btnCheck;
    private System.Windows.Forms.Button btnClean;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
}
