using System.Security.Cryptography;
using System.Text;

namespace TimeCapsule.Core;

/// <summary>命令行自检：把方案里的验收清单逐条跑一遍，任何一条失败返回非 0。</summary>
public static class SelfTest
{
    private static readonly StringBuilder Report = new();
    private static int _failures;
    private static int _passes;

    public static int Run(string workDir)
    {
        try { Console.OutputEncoding = Encoding.UTF8; } catch { /* 无控制台句柄 */ }
        string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string root = Path.Combine(workDir, "selftest-" + stamp);
        string src = Path.Combine(root, "src");
        string vaultDir = Path.Combine(root, "vault");

        Directory.CreateDirectory(src);
        Directory.CreateDirectory(Path.Combine(src, "sub"));

        var originalHash = new Dictionary<string, string>();

        void Check(string name, bool ok, string detail = "")
        {
            if (ok) _passes++; else _failures++;
            string line = (ok ? "PASS  " : "FAIL  ") + name + (detail.Length > 0 ? "   [" + detail + "]" : "");
            Report.AppendLine(line);
            try { Console.WriteLine(line); } catch { /* 忽略 */ }
        }

        try
        {
            // 造数据
            File.WriteAllText(Path.Combine(src, "a.txt"), "hello 时间胶囊 v1", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(src, "sub", "b.txt"), "second file", new UTF8Encoding(false));
            File.WriteAllBytes(Path.Combine(src, "big.bin"), RandomNumberGenerator.GetBytes(3 * 1024 * 1024 + 123));
            File.WriteAllBytes(Path.Combine(src, "empty.dat"), Array.Empty<byte>());
            File.WriteAllText(Path.Combine(src, "ignore.tmp"), "must be excluded");
            Directory.CreateDirectory(Path.Combine(src, "node_modules"));
            File.WriteAllText(Path.Combine(src, "node_modules", "junk.js"), "excluded dir");
            originalHash["big.bin"] = Crypto.HashFile(Path.Combine(src, "big.bin"));

            // 1. 建库与密码
            var vault = Vault.Create(vaultDir, "test-pass-123", src);
            Check("建库成功", Vault.LooksLikeVault(vaultDir), vaultDir);

            bool rejected = false;
            try { Vault.Open(vaultDir, "wrong-password"); }
            catch (BadPasswordException) { rejected = true; }
            Check("错误密码被拒绝", rejected);

            var opened = Vault.Open(vaultDir, "test-pass-123");
            Check("正确密码可开库", opened.Id == vault.Id && opened.MasterKey.Length == 32);

            // 2. 首次备份
            var engine = new BackupEngine(opened);
            var r1 = engine.Run("manual");
            Check("首次备份生成快照", r1 != null && r1.Created, r1?.Message ?? "null");
            Check("首次文件数为 4 且排除生效", r1 != null && r1.FileCount == 4,
                r1 == null ? "null" : "fileCount=" + r1.FileCount + " errors=" + r1.Errors.Count);

            var store = new SnapshotStore(opened);
            var m1 = store.Load(r1!.SnapshotId!);
            Check("清单可解密读取", m1 != null && m1.Entries.Count == 4);
            Check("排除 *.tmp 与 node_modules", m1 != null && m1.Entries.All(e => !e.Path.EndsWith(".tmp") && !e.Path.Contains("node_modules")));
            Check("空文件也被备份", m1 != null && m1.Entries.Any(e => e.Path == "empty.dat"));

            // 3. 无变化不产生快照
            var r2 = engine.Run("timer");
            Check("内容未变不生成新快照", r2 != null && r2.NoChange, r2?.Message ?? "null");

            // 4. 变化后增量
            File.WriteAllText(Path.Combine(src, "a.txt"), "hello 时间胶囊 v2 - changed", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(src, "c.txt"), "new file", new UTF8Encoding(false));
            File.Delete(Path.Combine(src, "sub", "b.txt"));
            var r3 = engine.Run("watch");
            Check("变化后生成新快照", r3 != null && r3.Created, r3?.Message ?? "null");
            Check("增删改统计正确", r3 != null && r3.Added == 1 && r3.Modified == 1 && r3.Removed == 1,
                r3 == null ? "null" : $"a={r3.Added} m={r3.Modified} r={r3.Removed}");
            Check("增量只加密新内容(2 个块)", r3 != null && r3.EncryptedNew == 2,
                r3 == null ? "null" : "encrypted=" + r3.EncryptedNew);
            Check("快照数量为 2", store.List().Count == 2, "count=" + store.List().Count);

            // 5. 整快照恢复
            var restore = new RestoreEngine(opened);
            string restoreNew = Path.Combine(root, "restore-new");
            var rr = restore.Restore(new RestoreOptions { SnapshotId = r3!.SnapshotId!, TargetRoot = restoreNew });
            Check("整个快照恢复到新目录", rr.Restored == 4 && rr.Failed == 0, $"ok={rr.Restored} fail={rr.Failed}");
            Check("恢复内容与源一致", ReadText(Path.Combine(restoreNew, "a.txt")) == "hello 时间胶囊 v2 - changed");

            // 6. 恢复旧版本（时间胶囊的核心）
            string restoreOld = Path.Combine(root, "restore-old");
            var rrOld = restore.Restore(new RestoreOptions { SnapshotId = r1.SnapshotId!, TargetRoot = restoreOld });
            Check("能恢复到更早的时间点", rrOld.Restored == 4 && rrOld.Failed == 0, $"ok={rrOld.Restored}");
            Check("旧版本内容正确", ReadText(Path.Combine(restoreOld, "a.txt")) == "hello 时间胶囊 v1");
            Check("已删除的文件能从旧快照取回", File.Exists(Path.Combine(restoreOld, "sub", "b.txt")));
            Check("大文件解密后哈希一致", Crypto.HashFile(Path.Combine(restoreOld, "big.bin")) == originalHash["big.bin"]);
            Check("空文件还原为 0 字节", new FileInfo(Path.Combine(restoreOld, "empty.dat")).Length == 0);

            // 7. 只恢复勾选文件
            string restoreOne = Path.Combine(root, "restore-one");
            var rrOne = restore.Restore(new RestoreOptions
            {
                SnapshotId = r3.SnapshotId!,
                TargetRoot = restoreOne,
                OnlyPaths = new List<string> { "a.txt" }
            });
            Check("只恢复勾选文件", rrOne.Restored == 1 && !File.Exists(Path.Combine(restoreOne, "c.txt")),
                $"ok={rrOne.Restored}");

            // 8. 默认不覆盖
            var rrSkip = restore.Restore(new RestoreOptions
            {
                SnapshotId = r1.SnapshotId!,
                TargetRoot = restoreOne,
                OnlyPaths = new List<string> { "a.txt" }
            });
            Check("默认不覆盖已存在文件", rrSkip.Skipped == 1 && rrSkip.Restored == 0);
            Check("跳过后文件内容未被改动", ReadText(Path.Combine(restoreOne, "a.txt")) == "hello 时间胶囊 v2 - changed");

            // 9. 覆盖模式 + 回收
            var rrOver = restore.Restore(new RestoreOptions
            {
                SnapshotId = r1.SnapshotId!,
                TargetRoot = restoreOne,
                OnlyPaths = new List<string> { "a.txt" },
                OverwriteExisting = true
            });
            Check("覆盖模式把旧文件移入回收目录", rrOver.Restored == 1 && rrOver.TrashDir != null && File.Exists(Path.Combine(rrOver.TrashDir, "a.txt")));
            Check("覆盖后内容为旧版本", ReadText(Path.Combine(restoreOne, "a.txt")) == "hello 时间胶囊 v1");

            // 10. 篡改检测
            var blobStore = new BlobStore(opened);
            var entryA = m1!.Entries.First(e => e.Path == "a.txt");
            string blobPath = blobStore.PathFor(entryA.Hash);
            byte[] rawBlob = File.ReadAllBytes(blobPath);
            rawBlob[rawBlob.Length - 5] ^= 0xFF;
            File.WriteAllBytes(blobPath, rawBlob);

            string restoreTamper = Path.Combine(root, "restore-tamper");
            var rrTamper = restore.Restore(new RestoreOptions { SnapshotId = r1.SnapshotId!, TargetRoot = restoreTamper });
            Check("块被篡改时能被检出", rrTamper.Failed == 1, $"fail={rrTamper.Failed}");
            Check("其余文件仍能正常恢复", rrTamper.Restored == 3, $"ok={rrTamper.Restored}");

            // 11. 重开库
            var reopen = Vault.Open(vaultDir, "test-pass-123");
            Check("重开库后快照仍在", new SnapshotStore(reopen).List().Count == 2);
            var rrReopen = new RestoreEngine(reopen).Restore(new RestoreOptions
            {
                SnapshotId = r3.SnapshotId!,
                TargetRoot = Path.Combine(root, "restore-reopen"),
                OnlyPaths = new List<string> { "c.txt" }
            });
            Check("重开后仍能恢复", rrReopen.Restored == 1);

            // 13. 密码托管（DPAPI）
            bool dpapiOk = CredentialStore.DpapiRoundTrip(out string dpapiDetail);
            Check("本机密码保护（DPAPI）可用", dpapiOk, dpapiDetail);

            // 12. 库统计
            int blobs = blobStore.CountBlobs();
            Check("数据块数量合理", blobs >= 5, "blobs=" + blobs);
        }
        catch (Exception ex)
        {
            _failures++;
            string line = "FAIL  自检过程抛出异常：" + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace;
            Report.AppendLine(line);
            try { Console.WriteLine(line); } catch { /* 忽略 */ }
        }

        string summary = $"自检结果：通过 {_passes} 项，失败 {_failures} 项";
        Report.AppendLine(summary);
        try { Console.WriteLine(summary); } catch { /* 忽略 */ }

        try
        {
            Directory.CreateDirectory(Path.Combine(workDir, "reports"));
            File.WriteAllText(Path.Combine(workDir, "reports", "selftest-" + stamp + ".log"), Report.ToString(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(workDir, "reports", "selftest-latest.log"), Report.ToString(), new UTF8Encoding(false));
        }
        catch { /* 忽略 */ }

        return _failures == 0 ? 0 : 1;
    }

    private static string ReadText(string path)
        => File.Exists(path) ? File.ReadAllText(path) : "<missing>";
}
