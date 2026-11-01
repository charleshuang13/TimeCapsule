# -*- coding: utf-8 -*-
import pathlib
import shutil
import sys
import tempfile

sys.stdout.reconfigure(encoding="utf-8", errors="replace")

base = pathlib.Path(tempfile.gettempdir()) / "tc-demo"
if base.exists():
    shutil.rmtree(base, ignore_errors=True)

src = base / "src"
(src / "图纸").mkdir(parents=True)
(src / "文档").mkdir(parents=True)
(src / "程序").mkdir(parents=True)

files = {
    r"图纸\SMT07-主板-版本A.dwg": b"AC1032 drawing placeholder " * 900,
    r"图纸\SMT09-治具.dwg": b"AC1032 fixture placeholder " * 400,
    r"文档\作业指导书-回流焊.docx": b"PK\x03\x04 docx placeholder " * 1200,
    r"文档\产线点检表.xlsx": b"PK\x03\x04 xlsx placeholder " * 600,
    r"文档\变更记录.txt": "2026-10-30 首版发布\r\n2026-11-01 补充治具说明\r\n".encode("utf-8"),
    r"程序\XOUT生成器.cs": b"public class XoutBuilder { } // v1\n" * 60,
    r"程序\readme.md": "# XOUT 生成器\r\n\r\n按规则文件生成 xout。\r\n".encode("utf-8"),
}
for name, data in files.items():
    (src / name).write_bytes(data)

(src / "ignore.tmp").write_bytes(b"should be excluded")
print("演示目录已建：", src)
for p in sorted(src.rglob("*")):
    print("  ", p.relative_to(src), p.stat().st_size if p.is_file() else "<dir>")
