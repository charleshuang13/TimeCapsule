namespace TimeCapsule.UI;

partial class FrmMain
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
        this.panelTop = new System.Windows.Forms.Panel();
        this.lblSourceTitle = new System.Windows.Forms.Label();
        this.txtSource = new System.Windows.Forms.TextBox();
        this.btnBrowse = new System.Windows.Forms.Button();
        this.btnBackupNow = new System.Windows.Forms.Button();
        this.btnPause = new System.Windows.Forms.Button();
        this.btnSettings = new System.Windows.Forms.Button();
        this.panelDot = new System.Windows.Forms.Panel();
        this.lblStatus = new System.Windows.Forms.Label();
        this.panelBottom = new System.Windows.Forms.Panel();
        this.lblRestoreHint = new System.Windows.Forms.Label();
        this.chkOverwrite = new System.Windows.Forms.CheckBox();
        this.txtTarget = new System.Windows.Forms.TextBox();
        this.btnPickTarget = new System.Windows.Forms.Button();
        this.btnRestore = new System.Windows.Forms.Button();
        this.progressBar = new System.Windows.Forms.ProgressBar();
        this.splitMain = new System.Windows.Forms.SplitContainer();
        this.dgvSnapshots = new System.Windows.Forms.DataGridView();
        this.colTimeTxt = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colTrigger = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colFiles = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.lblSnapHeader = new System.Windows.Forms.Label();
        this.dgvFiles = new System.Windows.Forms.DataGridView();
        this.colSel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colSizeText = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colMtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.panelDetail = new System.Windows.Forms.Panel();
        this.lblDetailTitle = new System.Windows.Forms.Label();
        this.lblDetailBody = new System.Windows.Forms.TextBox();
        this.btnRestoreOne = new System.Windows.Forms.Button();
        this.lblFileHeader = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
        this.splitMain.Panel1.SuspendLayout();
        this.splitMain.Panel2.SuspendLayout();
        this.splitMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvSnapshots)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
        this.panelTop.SuspendLayout();
        this.panelBottom.SuspendLayout();
        this.panelDetail.SuspendLayout();
        this.SuspendLayout();
        //
        // panelTop
        //
        this.panelTop.BackColor = System.Drawing.Color.White;
        this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
        this.panelTop.Height = 96;
        this.panelTop.Size = new System.Drawing.Size(1180, 96);
        this.panelTop.Controls.Add(this.lblSourceTitle);
        this.panelTop.Controls.Add(this.txtSource);
        this.panelTop.Controls.Add(this.btnBrowse);
        this.panelTop.Controls.Add(this.btnBackupNow);
        this.panelTop.Controls.Add(this.btnPause);
        this.panelTop.Controls.Add(this.btnSettings);
        this.panelTop.Controls.Add(this.panelDot);
        this.panelTop.Controls.Add(this.lblStatus);
        //
        // lblSourceTitle
        //
        this.lblSourceTitle.Location = new System.Drawing.Point(16, 16);
        this.lblSourceTitle.Size = new System.Drawing.Size(62, 24);
        this.lblSourceTitle.Text = "监控目录";
        this.lblSourceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // txtSource
        //
        this.txtSource.Location = new System.Drawing.Point(82, 16);
        this.txtSource.Size = new System.Drawing.Size(400, 23);
        this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.txtSource.ReadOnly = true;
        this.txtSource.BackColor = System.Drawing.Color.FromArgb(247, 248, 250);
        this.txtSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        //
        // btnBrowse
        //
        this.btnBrowse.Location = new System.Drawing.Point(490, 14);
        this.btnBrowse.Size = new System.Drawing.Size(78, 27);
        this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnBrowse.Text = "更换目录";
        this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnBrowse.FlatAppearance.BorderSize = 1;
        this.btnBrowse.BackColor = System.Drawing.Color.White;
        this.btnBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnBackupNow
        //
        this.btnBackupNow.Location = new System.Drawing.Point(576, 14);
        this.btnBackupNow.Size = new System.Drawing.Size(96, 27);
        this.btnBackupNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnBackupNow.Text = "立即备份";
        this.btnBackupNow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnBackupNow.FlatAppearance.BorderSize = 0;
        this.btnBackupNow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        this.btnBackupNow.ForeColor = System.Drawing.Color.White;
        this.btnBackupNow.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnPause
        //
        this.btnPause.Location = new System.Drawing.Point(680, 14);
        this.btnPause.Size = new System.Drawing.Size(96, 27);
        this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnPause.Text = "暂停监控";
        this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnPause.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnPause.FlatAppearance.BorderSize = 1;
        this.btnPause.BackColor = System.Drawing.Color.White;
        this.btnPause.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnPause.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnSettings
        //
        this.btnSettings.Location = new System.Drawing.Point(784, 14);
        this.btnSettings.Size = new System.Drawing.Size(78, 27);
        this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnSettings.Text = "设置";
        this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnSettings.FlatAppearance.BorderSize = 1;
        this.btnSettings.BackColor = System.Drawing.Color.White;
        this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // panelDot
        //
        this.panelDot.Location = new System.Drawing.Point(18, 60);
        this.panelDot.Size = new System.Drawing.Size(10, 10);
        //
        // lblStatus
        //
        this.lblStatus.Location = new System.Drawing.Point(36, 53);
        this.lblStatus.Size = new System.Drawing.Size(1000, 24);
        this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.lblStatus.Text = "正在启动…";
        this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        //
        // panelBottom
        //
        this.panelBottom.BackColor = System.Drawing.Color.White;
        this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.panelBottom.Height = 74;
        this.panelBottom.Size = new System.Drawing.Size(1180, 74);
        this.panelBottom.Controls.Add(this.lblRestoreHint);
        this.panelBottom.Controls.Add(this.chkOverwrite);
        this.panelBottom.Controls.Add(this.txtTarget);
        this.panelBottom.Controls.Add(this.btnPickTarget);
        this.panelBottom.Controls.Add(this.btnRestore);
        this.panelBottom.Controls.Add(this.progressBar);
        //
        // lblRestoreHint
        //
        this.lblRestoreHint.Location = new System.Drawing.Point(16, 9);
        this.lblRestoreHint.Size = new System.Drawing.Size(1100, 22);
        this.lblRestoreHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.lblRestoreHint.Text = "请在上方选择一个时间点";
        this.lblRestoreHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblRestoreHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // chkOverwrite
        //
        this.chkOverwrite.Location = new System.Drawing.Point(650, 36);
        this.chkOverwrite.Size = new System.Drawing.Size(140, 26);
        this.chkOverwrite.AutoSize = false;
        this.chkOverwrite.Text = "覆盖同名文件";
        this.chkOverwrite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //
        // txtTarget
        //
        this.txtTarget.Location = new System.Drawing.Point(16, 38);
        this.txtTarget.Size = new System.Drawing.Size(400, 23);
        this.txtTarget.ReadOnly = true;
        this.txtTarget.BackColor = System.Drawing.Color.FromArgb(247, 248, 250);
        this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        //
        // btnPickTarget
        //
        this.btnPickTarget.Location = new System.Drawing.Point(424, 36);
        this.btnPickTarget.Size = new System.Drawing.Size(96, 27);
        this.btnPickTarget.Text = "选择目标…";
        this.btnPickTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnPickTarget.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnPickTarget.FlatAppearance.BorderSize = 1;
        this.btnPickTarget.BackColor = System.Drawing.Color.White;
        this.btnPickTarget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnPickTarget.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // btnRestore
        //
        this.btnRestore.Location = new System.Drawing.Point(528, 36);
        this.btnRestore.Size = new System.Drawing.Size(110, 27);
        this.btnRestore.Text = "开始恢复";
        this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnRestore.FlatAppearance.BorderSize = 0;
        this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))));
        this.btnRestore.ForeColor = System.Drawing.Color.White;
        this.btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // progressBar
        //
        this.progressBar.Location = new System.Drawing.Point(800, 42);
        this.progressBar.Size = new System.Drawing.Size(300, 16);
        this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
        //
        // splitMain
        //
        this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitMain.Location = new System.Drawing.Point(0, 96);
        this.splitMain.Name = "splitMain";
        this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
        this.splitMain.Panel1MinSize = 120;
        this.splitMain.Panel2MinSize = 150;
        this.splitMain.Size = new System.Drawing.Size(1180, 590);
        this.splitMain.SplitterDistance = 265;
        this.splitMain.SplitterWidth = 6;
        this.splitMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.splitMain.Panel1.BackColor = System.Drawing.Color.White;
        this.splitMain.Panel2.BackColor = System.Drawing.Color.White;
        this.splitMain.Panel1.Controls.Add(this.dgvSnapshots);
        this.splitMain.Panel1.Controls.Add(this.lblSnapHeader);
        this.splitMain.Panel2.Controls.Add(this.dgvFiles);
        this.splitMain.Panel2.Controls.Add(this.panelDetail);
        this.splitMain.Panel2.Controls.Add(this.lblFileHeader);
        //
        // lblSnapHeader
        //
        this.lblSnapHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblSnapHeader.Height = 30;
        this.lblSnapHeader.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
        this.lblSnapHeader.Text = "快照时间轴（选一行查看那一刻的文件）";
        this.lblSnapHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblSnapHeader.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // dgvSnapshots
        //
        this.dgvSnapshots.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvSnapshots.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTimeTxt,
            this.colTrigger,
            this.colFiles,
            this.colSize,
            this.colChange,
            this.colNote});
        this.dgvSnapshots.MultiSelect = false;
        //
        // colTimeTxt
        //
        this.colTimeTxt.DataPropertyName = "Time";
        this.colTimeTxt.HeaderText = "时间";
        this.colTimeTxt.Width = 168;
        this.colTimeTxt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colTrigger
        //
        this.colTrigger.DataPropertyName = "Trigger";
        this.colTrigger.HeaderText = "触发方式";
        this.colTrigger.Width = 96;
        this.colTrigger.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colFiles
        //
        this.colFiles.DataPropertyName = "FilesText";
        this.colFiles.HeaderText = "文件数";
        this.colFiles.Width = 78;
        this.colFiles.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
        this.colFiles.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colSize
        //
        this.colSize.DataPropertyName = "SizeText";
        this.colSize.HeaderText = "总计";
        this.colSize.Width = 96;
        this.colSize.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
        this.colSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colChange
        //
        this.colChange.DataPropertyName = "ChangeText";
        this.colChange.HeaderText = "较上次";
        this.colChange.Width = 168;
        this.colChange.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colNote
        //
        this.colNote.DataPropertyName = "Note";
        this.colNote.HeaderText = "备注";
        this.colNote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        this.colNote.MinimumWidth = 120;
        this.colNote.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // lblFileHeader
        //
        this.lblFileHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblFileHeader.Height = 30;
        this.lblFileHeader.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
        this.lblFileHeader.Text = "该时间点的文件";
        this.lblFileHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblFileHeader.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // dgvFiles
        //
        this.dgvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSel,
            this.colPath,
            this.colSizeText,
            this.colMtime});
        //
        // colSel
        //
        this.colSel.DataPropertyName = "Selected";
        this.colSel.HeaderText = "";
        this.colSel.Width = 38;
        this.colSel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
        //
        // colPath
        //
        this.colPath.DataPropertyName = "Path";
        this.colPath.HeaderText = "相对路径";
        this.colPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        this.colPath.MinimumWidth = 160;
        this.colPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colSizeText
        //
        this.colSizeText.DataPropertyName = "SizeText";
        this.colSizeText.HeaderText = "大小";
        this.colSizeText.Width = 96;
        this.colSizeText.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
        this.colSizeText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // colMtime
        //
        this.colMtime.DataPropertyName = "MtimeText";
        this.colMtime.HeaderText = "修改时间";
        this.colMtime.Width = 148;
        this.colMtime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        //
        // panelDetail
        //
        this.panelDetail.Dock = System.Windows.Forms.DockStyle.Right;
        this.panelDetail.Width = 320;
        this.panelDetail.Size = new System.Drawing.Size(320, 313);
        this.panelDetail.BackColor = System.Drawing.Color.FromArgb(250, 251, 252);
        this.panelDetail.Controls.Add(this.lblDetailTitle);
        this.panelDetail.Controls.Add(this.lblDetailBody);
        this.panelDetail.Controls.Add(this.btnRestoreOne);
        //
        // lblDetailTitle
        //
        this.lblDetailTitle.Location = new System.Drawing.Point(14, 10);
        this.lblDetailTitle.Size = new System.Drawing.Size(272, 22);
        this.lblDetailTitle.Text = "文件详情";
        this.lblDetailTitle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // lblDetailBody
        //
        this.lblDetailBody.Location = new System.Drawing.Point(14, 38);
        this.lblDetailBody.Size = new System.Drawing.Size(272, 210);
        this.lblDetailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.lblDetailBody.Multiline = true;
        this.lblDetailBody.ReadOnly = true;
        this.lblDetailBody.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.lblDetailBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.lblDetailBody.BackColor = System.Drawing.Color.FromArgb(250, 251, 252);
        this.lblDetailBody.TabStop = false;
        this.lblDetailBody.Text = "在上面的列表里选一个文件，这里显示它的大小、修改时间和内容指纹。";
        this.lblDetailBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
        this.lblDetailBody.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //
        // btnRestoreOne
        //
        this.btnRestoreOne.Location = new System.Drawing.Point(14, 258);
        this.btnRestoreOne.Size = new System.Drawing.Size(272, 30);
        this.btnRestoreOne.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.btnRestoreOne.Text = "恢复此文件到目标目录";
        this.btnRestoreOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnRestoreOne.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))));
        this.btnRestoreOne.FlatAppearance.BorderSize = 1;
        this.btnRestoreOne.BackColor = System.Drawing.Color.White;
        this.btnRestoreOne.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.btnRestoreOne.Cursor = System.Windows.Forms.Cursors.Hand;
        //
        // FrmMain
        //
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.ClientSize = new System.Drawing.Size(1180, 760);
        this.MinimumSize = new System.Drawing.Size(1000, 620);
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
        this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "时间胶囊";
        this.Controls.Add(this.splitMain);
        this.Controls.Add(this.panelBottom);
        this.Controls.Add(this.panelTop);
        this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
        this.btnBackupNow.Click += new System.EventHandler(this.BtnBackupNow_Click);
        this.btnPause.Click += new System.EventHandler(this.BtnPause_Click);
        this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
        this.btnPickTarget.Click += new System.EventHandler(this.BtnPickTarget_Click);
        this.btnRestore.Click += new System.EventHandler(this.BtnRestore_Click);
        this.btnRestoreOne.Click += new System.EventHandler(this.BtnRestoreOne_Click);
        this.chkOverwrite.CheckedChanged += new System.EventHandler(this.ChkOverwrite_CheckedChanged);
        this.dgvSnapshots.SelectionChanged += new System.EventHandler(this.DgvSnapshots_SelectionChanged);
        this.dgvFiles.SelectionChanged += new System.EventHandler(this.DgvFiles_SelectionChanged);
        this.dgvFiles.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvFiles_CurrentCellDirtyStateChanged);
        this.dgvFiles.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvFiles_CellValueChanged);
        this.Resize += new System.EventHandler(this.FrmMain_Resize);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
        this.splitMain.Panel1.ResumeLayout(false);
        this.splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
        this.splitMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvSnapshots)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
        this.panelTop.ResumeLayout(false);
        this.panelTop.PerformLayout();
        this.panelBottom.ResumeLayout(false);
        this.panelBottom.PerformLayout();
        this.panelDetail.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Panel panelTop;
    private System.Windows.Forms.Label lblSourceTitle;
    private System.Windows.Forms.TextBox txtSource;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.Button btnBackupNow;
    private System.Windows.Forms.Button btnPause;
    private System.Windows.Forms.Button btnSettings;
    private System.Windows.Forms.Panel panelDot;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Panel panelBottom;
    private System.Windows.Forms.Label lblRestoreHint;
    private System.Windows.Forms.CheckBox chkOverwrite;
    private System.Windows.Forms.TextBox txtTarget;
    private System.Windows.Forms.Button btnPickTarget;
    private System.Windows.Forms.Button btnRestore;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.SplitContainer splitMain;
    private System.Windows.Forms.DataGridView dgvSnapshots;
    private System.Windows.Forms.DataGridViewTextBoxColumn colTimeTxt;
    private System.Windows.Forms.DataGridViewTextBoxColumn colTrigger;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFiles;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
    private System.Windows.Forms.DataGridViewTextBoxColumn colChange;
    private System.Windows.Forms.DataGridViewTextBoxColumn colNote;
    private System.Windows.Forms.Label lblSnapHeader;
    private System.Windows.Forms.DataGridView dgvFiles;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colSel;
    private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSizeText;
    private System.Windows.Forms.DataGridViewTextBoxColumn colMtime;
    private System.Windows.Forms.Panel panelDetail;
    private System.Windows.Forms.Label lblDetailTitle;
    private System.Windows.Forms.TextBox lblDetailBody;
    private System.Windows.Forms.Button btnRestoreOne;
    private System.Windows.Forms.Label lblFileHeader;
}
