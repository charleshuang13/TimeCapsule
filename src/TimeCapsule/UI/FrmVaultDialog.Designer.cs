namespace TimeCapsule.UI;

partial class FrmVaultDialog
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

    #region Windows 窗体设计器生成的代码

    private void InitializeComponent()
    {
        this.lblHint = new System.Windows.Forms.Label();
        this.lblLib = new System.Windows.Forms.Label();
        this.txtLib = new System.Windows.Forms.TextBox();
        this.btnLib = new System.Windows.Forms.Button();
        this.lblPwd = new System.Windows.Forms.Label();
        this.txtPwd = new System.Windows.Forms.TextBox();
        this.lblPwd2 = new System.Windows.Forms.Label();
        this.txtPwd2 = new System.Windows.Forms.TextBox();
        this.lblSrc = new System.Windows.Forms.Label();
        this.txtSrc = new System.Windows.Forms.TextBox();
        this.btnSrc = new System.Windows.Forms.Button();
        this.lblNote = new System.Windows.Forms.Label();
        this.btnOk = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // lblHint
        // 
        this.lblHint.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblHint.Location = new System.Drawing.Point(24, 18);
        this.lblHint.Name = "lblHint";
        this.lblHint.Size = new System.Drawing.Size(512, 44);
        this.lblHint.TabIndex = 0;
        this.lblHint.Text = "先设定一个主密码。以后打开软件会自动使用它，不用每次输入；密码用 Windows 账户加密保存在本机，换台电脑打不开。";
        // 
        // lblLib
        // 
        this.lblLib.Location = new System.Drawing.Point(24, 70);
        this.lblLib.Name = "lblLib";
        this.lblLib.Size = new System.Drawing.Size(72, 22);
        this.lblLib.TabIndex = 1;
        this.lblLib.Text = "库位置";
        this.lblLib.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtLib
        // 
        this.txtLib.Location = new System.Drawing.Point(104, 69);
        this.txtLib.Name = "txtLib";
        this.txtLib.Size = new System.Drawing.Size(320, 23);
        this.txtLib.TabIndex = 2;
        // 
        // btnLib
        // 
        this.btnLib.Location = new System.Drawing.Point(432, 68);
        this.btnLib.Name = "btnLib";
        this.btnLib.Size = new System.Drawing.Size(100, 26);
        this.btnLib.TabIndex = 3;
        this.btnLib.Text = "选择目录…";
        this.btnLib.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnLib.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnLib.FlatAppearance.BorderSize = 1;
        this.btnLib.BackColor = System.Drawing.Color.White;
        this.btnLib.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnLib.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnLib.UseVisualStyleBackColor = true;
        // 
        // lblPwd
        // 
        this.lblPwd.Location = new System.Drawing.Point(24, 106);
        this.lblPwd.Name = "lblPwd";
        this.lblPwd.Size = new System.Drawing.Size(72, 22);
        this.lblPwd.TabIndex = 4;
        this.lblPwd.Text = "主密码";
        this.lblPwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtPwd
        // 
        this.txtPwd.Location = new System.Drawing.Point(104, 105);
        this.txtPwd.Name = "txtPwd";
        this.txtPwd.Size = new System.Drawing.Size(428, 23);
        this.txtPwd.TabIndex = 5;
        this.txtPwd.UseSystemPasswordChar = true;
        // 
        // lblPwd2
        // 
        this.lblPwd2.Location = new System.Drawing.Point(24, 142);
        this.lblPwd2.Name = "lblPwd2";
        this.lblPwd2.Size = new System.Drawing.Size(72, 22);
        this.lblPwd2.TabIndex = 6;
        this.lblPwd2.Text = "确认密码";
        this.lblPwd2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtPwd2
        // 
        this.txtPwd2.Location = new System.Drawing.Point(104, 141);
        this.txtPwd2.Name = "txtPwd2";
        this.txtPwd2.Size = new System.Drawing.Size(428, 23);
        this.txtPwd2.TabIndex = 7;
        this.txtPwd2.UseSystemPasswordChar = true;
        // 
        // lblSrc
        // 
        this.lblSrc.Location = new System.Drawing.Point(24, 178);
        this.lblSrc.Name = "lblSrc";
        this.lblSrc.Size = new System.Drawing.Size(72, 22);
        this.lblSrc.TabIndex = 8;
        this.lblSrc.Text = "监控目录";
        this.lblSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // txtSrc
        // 
        this.txtSrc.Location = new System.Drawing.Point(104, 177);
        this.txtSrc.Name = "txtSrc";
        this.txtSrc.Size = new System.Drawing.Size(320, 23);
        this.txtSrc.TabIndex = 9;
        // 
        // btnSrc
        // 
        this.btnSrc.Location = new System.Drawing.Point(432, 176);
        this.btnSrc.Name = "btnSrc";
        this.btnSrc.Size = new System.Drawing.Size(100, 26);
        this.btnSrc.TabIndex = 10;
        this.btnSrc.Text = "选择目录…";
        this.btnSrc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSrc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnSrc.FlatAppearance.BorderSize = 1;
        this.btnSrc.BackColor = System.Drawing.Color.White;
        this.btnSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnSrc.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnSrc.UseVisualStyleBackColor = true;
        // 
        // lblNote
        // 
        this.lblNote.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblNote.Location = new System.Drawing.Point(24, 214);
        this.lblNote.Name = "lblNote";
        this.lblNote.Size = new System.Drawing.Size(512, 40);
        this.lblNote.TabIndex = 11;
        this.lblNote.Text = "监控目录里所有文件（可按规则排除）都会被加密备份，原目录里的文件不会被改动。";
        // 
        // btnOk
        // 
        this.btnOk.Location = new System.Drawing.Point(318, 268);
        this.btnOk.Name = "btnOk";
        this.btnOk.Size = new System.Drawing.Size(120, 32);
        this.btnOk.TabIndex = 12;
        this.btnOk.Text = "创建并开始备份";
        this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnOk.FlatAppearance.BorderSize = 0;
        this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        this.btnOk.ForeColor = System.Drawing.Color.White;
        this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnOk.UseVisualStyleBackColor = true;
        // 
        // btnCancel
        // 
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(446, 268);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(90, 32);
        this.btnCancel.TabIndex = 13;
        this.btnCancel.Text = "取消";
        this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnCancel.FlatAppearance.BorderSize = 1;
        this.btnCancel.BackColor = System.Drawing.Color.White;
        this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnCancel.UseVisualStyleBackColor = true;
        // 
        // FrmVaultDialog
        // 
        this.AcceptButton = this.btnOk;
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.BackColor = System.Drawing.Color.White;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(560, 316);
        this.Controls.Add(this.lblHint);
        this.Controls.Add(this.lblLib);
        this.Controls.Add(this.txtLib);
        this.Controls.Add(this.btnLib);
        this.Controls.Add(this.lblPwd);
        this.Controls.Add(this.txtPwd);
        this.Controls.Add(this.lblPwd2);
        this.Controls.Add(this.txtPwd2);
        this.Controls.Add(this.lblSrc);
        this.Controls.Add(this.txtSrc);
        this.Controls.Add(this.btnSrc);
        this.Controls.Add(this.lblNote);
        this.Controls.Add(this.btnOk);
        this.Controls.Add(this.btnCancel);
        this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FrmVaultDialog";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "新建时间胶囊库";
        this.btnLib.Click += new System.EventHandler(this.BtnLib_Click);
        this.btnSrc.Click += new System.EventHandler(this.BtnSrc_Click);
        this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label lblHint;
    private System.Windows.Forms.Label lblLib;
    private System.Windows.Forms.TextBox txtLib;
    private System.Windows.Forms.Button btnLib;
    private System.Windows.Forms.Label lblPwd;
    private System.Windows.Forms.TextBox txtPwd;
    private System.Windows.Forms.Label lblPwd2;
    private System.Windows.Forms.TextBox txtPwd2;
    private System.Windows.Forms.Label lblSrc;
    private System.Windows.Forms.TextBox txtSrc;
    private System.Windows.Forms.Button btnSrc;
    private System.Windows.Forms.Label lblNote;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
}
