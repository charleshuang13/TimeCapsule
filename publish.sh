#!/usr/bin/env bash
# 时间胶囊 出包脚本：编译 → 自包含发布 → 三组自检 → 打 zip
set -uo pipefail
cd "$(dirname "$0")"

VER=$(grep -o '<Version>[^<]*' src/TimeCapsule/TimeCapsule.csproj | sed 's/<Version>//')
[ -n "$VER" ] || { echo "读不到版本号，别发"; exit 1; }
NAME="时间胶囊_v${VER}"
OUT="dist/$NAME"
TMP="$LOCALAPPDATA/Temp/tc-publish-check"

echo "[1/5] 版本 $VER"
rm -rf "$OUT"
mkdir -p "$TMP"

echo "[2/5] 编译并自包含发布（目标机不用装 .NET）…"
if ! dotnet publish src/TimeCapsule/TimeCapsule.csproj -c Release -r win-x64 --self-contained true \
        -p:PublishReadyToRun=false -o "$OUT" --nologo -v q; then
    echo "发布失败，别发"
    exit 1
fi
[ -f "$OUT/TimeCapsule.exe" ] || { echo "没生成 TimeCapsule.exe，别发"; exit 1; }
cp README.md "$OUT/使用说明.md"

echo "[3/5] 内核自检（建库/加密/快照/恢复/篡改检测）…"
rm -rf "$TMP/selftest"; mkdir -p "$TMP/selftest"
if ! "$OUT/TimeCapsule.exe" --selftest "$TMP/selftest" >/dev/null 2>&1; then
    echo "自检没过，别发：$TMP/selftest/reports/selftest-latest.log"
    tail -20 "$TMP/selftest/reports/selftest-latest.log"
    exit 1
fi
tail -1 "$TMP/selftest/reports/selftest-latest.log"

echo "[4/5] 界面流程自检（建库对话框 → 主界面 → 首次备份）…"
if ! "$OUT/TimeCapsule.exe" --uitest "$TMP" >/dev/null 2>&1; then
    echo "界面自检没过，别发：$OUT/reports/uitest.log"
    cat "$OUT/reports/uitest.log"
    exit 1
fi
grep -c '^PASS' "$OUT/reports/uitest.log" | sed 's/^/  界面检查通过项：/'

echo "[4b/5] 设计器兼容性自检（VS 能不能打开设计器）…"
if ! "$OUT/TimeCapsule.exe" --designcheck >/dev/null 2>&1; then
    echo "设计器兼容性没过，别发：$OUT/reports/designcheck.log"
    cat "$OUT/reports/designcheck.log"
    exit 1
fi
grep -c '^PASS' "$OUT/reports/designcheck.log" | sed 's/^/  设计器检查通过项：/'
rm -rf "$OUT/reports"

echo "[5/5] 打包…"
if ! python tools/package.py "$NAME"; then
    echo "打包失败，目录还在：$OUT"
    exit 1
fi

FILES=$(find "$OUT" -type f | wc -l | tr -d ' ')
SIZE=$(du -sh "$OUT" | cut -f1)
ZIP="dist/$NAME.zip"
ZSIZE=$(du -h "$ZIP" | cut -f1)
SHA=$(sha256sum "$ZIP" | cut -c1-16)

echo ""
echo "完成："
echo "  目录   $OUT  （$FILES 个文件 / $SIZE）"
echo "  压缩包 $ZIP  （$ZSIZE）"
echo "  sha256 $SHA…"
echo "  抓图   reports/main.png / settings.png / newvault.png"
