namespace TimeCapsule.UI;

/// <summary>统一配色与控件样式。状态一律用颜色表达，不用符号字符。</summary>
public static class UiTheme
{
    public static readonly Color Bg = Color.FromArgb(0xF4, 0xF6, 0xF9);
    public static readonly Color Surface = Color.White;
    public static readonly Color Line = Color.FromArgb(0xE2, 0xE6, 0xEC);
    public static readonly Color Text = Color.FromArgb(0x1F, 0x23, 0x28);
    public static readonly Color Muted = Color.FromArgb(0x6B, 0x72, 0x80);
    public static readonly Color Accent = Color.FromArgb(0x2F, 0x6F, 0xED);
    public static readonly Color AccentLight = Color.FromArgb(0xEC, 0xF2, 0xFE);
    public static readonly Color Green = Color.FromArgb(0x18, 0x9C, 0x5B);
    public static readonly Color Orange = Color.FromArgb(0xC8, 0x7A, 0x04);
    public static readonly Color Red = Color.FromArgb(0xCE, 0x3B, 0x3B);
    public static readonly Color RowAlt = Color.FromArgb(0xFA, 0xFB, 0xFC);
    public static readonly Color Selection = Color.FromArgb(0xDC, 0xE8, 0xFD);

    private static Font? _base;
    private static Font? _bold;
    private static Font? _title;
    private static Font? _small;

    public static Font Base => _base ??= new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static Font Bold => _bold ??= new Font("微软雅黑", 9F, FontStyle.Bold, GraphicsUnit.Point);
    public static Font Title => _title ??= new Font("微软雅黑", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
    public static Font Small => _small ??= new Font("微软雅黑", 8.25F, FontStyle.Regular, GraphicsUnit.Point);

    public static void Primary(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.BackColor = Accent;
        b.ForeColor = Color.White;
        b.Font = Base;
        b.Cursor = Cursors.Hand;
    }

    public static void Secondary(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = Line;
        b.FlatAppearance.BorderSize = 1;
        b.BackColor = Color.White;
        b.ForeColor = Text;
        b.Font = Base;
        b.Cursor = Cursors.Hand;
    }

    public static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Surface;
        g.BorderStyle = BorderStyle.None;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.GridColor = Line;
        g.EnableHeadersVisualStyles = false;
        g.RowHeadersVisible = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.ReadOnly = true;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.MultiSelect = false;
        g.Font = Base;
        g.RowTemplate.Height = 26;
        g.ColumnHeadersHeight = 30;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0xF7, 0xF8, 0xFA);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
        g.ColumnHeadersDefaultCellStyle.Font = Bold;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0xF7, 0xF8, 0xFA);
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted;
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        g.DefaultCellStyle.BackColor = Surface;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = Selection;
        g.DefaultCellStyle.SelectionForeColor = Text;
        g.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
        g.AutoGenerateColumns = false;
    }

    /// <summary>画一个圆形状态点。</summary>
    public static void DrawDot(object? sender, PaintEventArgs e, Color color)
    {
        if (sender is not Control c) return;
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, c.Width - 1, c.Height - 1);
        using var brush = new SolidBrush(color);
        e.Graphics.FillEllipse(brush, r);
    }
}
