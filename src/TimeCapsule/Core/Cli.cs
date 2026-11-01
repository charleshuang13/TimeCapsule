using System.Text;

namespace TimeCapsule.Core;

/// <summary>命令行入口（自检、以及给脚本调用的建库/备份/恢复）。</summary>
public static class Cli
{
    public static int Run(string[] args)
    {
        try { Console.OutputEncoding = Encoding.UTF8; } catch { /* 忽略 */ }
        string cmd = args[0].ToLowerInvariant();
        try
        {
            switch (cmd)
            {
                case "--selftest":
                    return SelfTest.Run(args.Length > 1 ? args[1] : Environment.CurrentDirectory);

                case "--create":
                    {
                        var v = Vault.Create(args[1], args[2], args[3]);
                        Console.WriteLine("库已创建：" + v.Root);
                        return 0;
                    }

                case "--backup":
                    {
                        var v = Vault.Open(args[1], args[2]);
                        var r = new BackupEngine(v).Run("manual", default, m => Console.WriteLine(m));
                        if (r == null) { Console.WriteLine("已有备份任务在跑"); return 2; }
                        Console.WriteLine(r.Message);
                        Console.WriteLine($"文件 {r.FileCount} 个 / {Humanize.Size(r.TotalBytes)}，新增 {r.Added}，修改 {r.Modified}，删除 {r.Removed}，新加密块 {r.EncryptedNew}");
                        foreach (var e in r.Errors) Console.WriteLine("跳过：" + e);
                        return r.Errors.Count == 0 ? 0 : 3;
                    }

                case "--list":
                    {
                        var v = Vault.Open(args[1], args[2]);
                        var list = new SnapshotStore(v).List();
                        Console.WriteLine($"共 {list.Count} 张快照");
                        foreach (var s in list)
                        {
                            Console.WriteLine($"{s.Id}  {s.TakenAt:yyyy-MM-dd HH:mm:ss}  {s.TriggerText}  文件 {s.FileCount}  {Humanize.Size(s.TotalBytes)}  {s.ChangeText}");
                        }
                        return 0;
                    }

                case "--info":
                    {
                        var v = Vault.Open(args[1], args[2]);
                        var blobs = new BlobStore(v);
                        Console.WriteLine($"库：{v.Root}");
                        Console.WriteLine($"监控目录：{v.Settings.SourceRoot}");
                        Console.WriteLine($"快照：{new SnapshotStore(v).Count()} 张");
                        Console.WriteLine($"数据块：{blobs.CountBlobs()} 个 / {Humanize.Size(blobs.TotalBlobBytes())}");
                        return 0;
                    }

                case "--restore":
                    {
                        var v = Vault.Open(args[1], args[2]);
                        var opt = new RestoreOptions { SnapshotId = args[3], TargetRoot = args[4] };
                        for (int i = 5; i < args.Length; i++)
                        {
                            if (args[i] == "--paths" && i + 1 < args.Length)
                                opt.OnlyPaths = args[++i].Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                            else if (args[i] == "--overwrite") opt.OverwriteExisting = true;
                        }
                        var r = new RestoreEngine(v).Restore(opt, m => Console.WriteLine(m));
                        Console.WriteLine($"恢复完成：成功 {r.Restored}，跳过 {r.Skipped}，失败 {r.Failed} → {r.TargetRoot}");
                        foreach (var e in r.Errors) Console.WriteLine("失败：" + e);
                        return r.Failed == 0 ? 0 : 4;
                    }

                default:
                    Console.WriteLine("未知命令：" + cmd);
                    Console.WriteLine(Usage);
                    return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("错误：" + ex.GetType().Name + " " + ex.Message);
            return 1;
        }
    }

    public const string Usage =
        "用法：\n" +
        "  TimeCapsule.exe                              启动图形界面\n" +
        "  TimeCapsule.exe --selftest <工作目录>         全流程自检\n" +
        "  TimeCapsule.exe --create <库目录> <密码> <监控目录>\n" +
        "  TimeCapsule.exe --backup <库目录> <密码>\n" +
        "  TimeCapsule.exe --list <库目录> <密码>\n" +
        "  TimeCapsule.exe --info <库目录> <密码>\n" +
        "  TimeCapsule.exe --restore <库目录> <密码> <快照ID> <目标目录> [--paths a;b] [--overwrite]";
}
