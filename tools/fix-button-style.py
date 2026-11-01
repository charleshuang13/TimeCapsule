# -*- coding: utf-8 -*-
"""把按钮外观（扁平/主次色）写进 Designer.cs，让 VS 设计器里看到的样式 = 运行时样式，
用户在属性面板里改颜色也不会被构造函数覆盖。"""
import pathlib
import sys

sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = pathlib.Path(__file__).resolve().parent.parent / "src" / "TimeCapsule" / "UI"

ACCENT = "System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))))"
LINE = "System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))))"
TEXT = "System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))))"

PLAN = {
    "FrmMain.Designer.cs": {
        "primary": ["btnBackupNow", "btnRestore"],
        "secondary": ["btnBrowse", "btnPause", "btnSettings", "btnPickTarget", "btnRestoreOne"],
    },
    "FrmVaultDialog.Designer.cs": {
        "primary": ["btnOk"],
        "secondary": ["btnLib", "btnSrc", "btnCancel"],
    },
    "FrmSettings.Designer.cs": {
        "primary": ["btnSave"],
        "secondary": ["btnCancel", "btnCheck", "btnClean", "btnSrc"],
    },
}


def style_lines(name, kind):
    pad = "        "
    lines = [f"{pad}this.{name}.FlatStyle = System.Windows.Forms.FlatStyle.Flat;"]
    if kind == "primary":
        lines += [
            f"{pad}this.{name}.FlatAppearance.BorderSize = 0;",
            f"{pad}this.{name}.BackColor = {ACCENT};",
            f"{pad}this.{name}.ForeColor = System.Drawing.Color.White;",
        ]
    else:
        lines += [
            f"{pad}this.{name}.FlatAppearance.BorderColor = {LINE};",
            f"{pad}this.{name}.FlatAppearance.BorderSize = 1;",
            f"{pad}this.{name}.BackColor = System.Drawing.Color.White;",
            f"{pad}this.{name}.ForeColor = {TEXT};",
        ]
    lines.append(f"{pad}this.{name}.Cursor = System.Windows.Forms.Cursors.Hand;")
    return lines


for fname, plan in PLAN.items():
    path = ROOT / fname
    text = path.read_text(encoding="utf-8-sig")
    targets = {}
    for kind, names in plan.items():
        for n in names:
            targets[n] = kind

    out = []
    added = []
    for line in text.splitlines():
        out.append(line)
        s = line.strip()
        for name, kind in targets.items():
            if s.startswith(f"this.{name}.Text = ") and name not in added:
                out.extend(style_lines(name, kind))
                added.append(name)
    path.write_text("\n".join(out) + "\n", encoding="utf-8-sig")
    missing = [n for n in targets if n not in added]
    print(f"{fname}: 写入 {len(added)} 个按钮样式" + (f"，未找到 {missing}" if missing else ""))
