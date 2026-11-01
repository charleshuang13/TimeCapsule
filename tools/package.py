# -*- coding: utf-8 -*-
"""把 dist 里的发布目录打成 zip（UTF-8 文件名，Windows 资源管理器可直接解压）。"""
import pathlib
import sys
import zipfile

sys.stdout.reconfigure(encoding="utf-8", errors="replace")

root = pathlib.Path(__file__).resolve().parent.parent
dist = root / "dist"

if len(sys.argv) > 1:
    name = sys.argv[1]
else:
    dirs = sorted([p for p in dist.iterdir() if p.is_dir()], key=lambda p: p.stat().st_mtime)
    if not dirs:
        print("dist 里没有发布目录")
        sys.exit(1)
    name = dirs[-1].name

src = dist / name
if not src.is_dir():
    print("找不到目录：", src)
    sys.exit(1)

zip_path = dist / (name + ".zip")
if zip_path.exists():
    zip_path.unlink()

count = 0
with zipfile.ZipFile(zip_path, "w", zipfile.ZIP_DEFLATED, compresslevel=9) as zf:
    for p in sorted(src.rglob("*")):
        if not p.is_file():
            continue
        arc = pathlib.PurePosixPath(name) / pathlib.PurePosixPath(p.relative_to(src).as_posix())
        zf.write(p, arc.as_posix())
        count += 1

size = zip_path.stat().st_size
print(f"已打包：{zip_path}")
print(f"  条目 {count} 个，压缩后 {size / 1024 / 1024:.1f} MB（原始 {sum(f.stat().st_size for f in src.rglob('*') if f.is_file()) / 1024 / 1024:.1f} MB）")
