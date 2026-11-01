# -*- coding: utf-8 -*-
"""重建一套干净的演示数据（三张快照），用于界面截图。"""
import pathlib
import shutil
import subprocess
import sys
import tempfile
import time

sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = pathlib.Path(__file__).resolve().parent.parent          # 仓库根目录
exe = ROOT / "src" / "TimeCapsule" / "bin" / "Release" / "net8.0-windows" / "TimeCapsule.exe"
base = pathlib.Path(tempfile.gettempdir()) / "tc-demo"
src = base / "src"
vault = base / "vault"
pwd = "demo-pass"

if base.exists():
    shutil.rmtree(base, ignore_errors=True)

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


def run(*args, quiet=True):
    r = subprocess.run([str(exe), *args], capture_output=True)
    if quiet:
        return r.returncode
    print(r.stdout.decode("utf-8", "replace"))
    return r.returncode


run("--create", str(vault), pwd, str(src))
run("--backup", str(vault), pwd)
time.sleep(1.2)

(src / "文档" / "变更记录.txt").write_text(
    "2026-10-30 首版发布\r\n2026-11-01 补充治具说明\r\n2026-11-01 增补：回流焊温度曲线调整\r\n",
    encoding="utf-8")
(src / "文档" / "工艺参数-新.xlsx").write_bytes(b"PK\x03\x04 xlsx new " * 300)
run("--backup", str(vault), pwd)
time.sleep(1.2)

(src / "图纸" / "SMT09-治具.dwg").write_bytes(b"AC1032 fixture placeholder v2 " * 460)
(src / "程序" / "readme.md").write_text(
    "# XOUT 生成器\r\n\r\n按规则文件生成 xout。\r\n\r\n版本 1.2：支持多板号。\r\n", encoding="utf-8")
run("--backup", str(vault), pwd)

print("演示库已重建：", vault)
run("--list", str(vault), pwd, quiet=False)
