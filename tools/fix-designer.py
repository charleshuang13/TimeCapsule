# -*- coding: utf-8 -*-
"""把 Designer.cs 里的 UiTheme.* 换成设计器原生写法（Color.FromArgb / new Font），
保证 VS 设计器能正常反序列化、保存时不会因为无法序列化静态属性而出错。"""
import pathlib
import sys

sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = pathlib.Path(__file__).resolve().parent.parent / "src" / "TimeCapsule" / "UI"

MAP = [
    ("UiTheme.Surface", "System.Drawing.Color.White"),
    ("UiTheme.Bg", "System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))))"),
    ("UiTheme.Line", "System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(236)))))"),
    ("UiTheme.Text", "System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))))"),
    ("UiTheme.Muted", "System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))))"),
    ("UiTheme.Accent", "System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(237)))))"),
    ("UiTheme.Base", 'new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)))'),
    ("UiTheme.Bold", 'new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)))'),
    ("UiTheme.Title", 'new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)))'),
    ("UiTheme.Small", 'new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)))'),
]

for path in sorted(ROOT.glob("*.Designer.cs")):
    text = path.read_text(encoding="utf-8-sig")
    before = text
    for old, new in MAP:
        text = text.replace(old, new)
    if text != before:
        path.write_text(text, encoding="utf-8-sig")
        left = text.count("UiTheme.")
        print(f"{path.name}: 已替换，剩余 UiTheme 引用 {left} 处")
    else:
        print(f"{path.name}: 无变化")
