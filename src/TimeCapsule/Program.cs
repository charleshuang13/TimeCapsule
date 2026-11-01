using System.Drawing.Imaging;
using System.Text;
using TimeCapsule.Core;
using TimeCapsule.UI;

namespace TimeCapsule;

internal static class Program
{
    private static Mutex? _instanceMutex;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0].StartsWith("--", StringComparison.Ordinal))
        {
            if (args[0].Equals("--screenshot", StringComparison.OrdinalIgnoreCase)) return Screenshot(args);
            if (args[0].Equals("--geom", StringComparison.OrdinalIgnoreCase)) return GeometryCheck(args);
            if (args[0].Equals("--watchtest", StringComparison.OrdinalIgnoreCase)) return WatchTest(args);
            if (args[0].Equals("--uitest", StringComparison.OrdinalIgnoreCase)) return UiTest(args);
            if (args[0].Equals("--designcheck", StringComparison.OrdinalIgnoreCase)) return DesignCheck();
            return Cli.Run(args);
        }

        bool createdNew;
        _instanceMutex = new Mutex(true, @"Local\TimeCapsule.SingleInstance", out createdNew);
        if (!createdNew)
        {
            MessageBox.Show("时间胶囊已经在运行了，请看任务栏右下角的胶囊图标（双击打开主界面）。",
                "时间胶囊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return 0;
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var app = AppState.Load();
        var vault = FrmVaultDialog.ShowStartup(app);
        if (vault == null) return 0;

        Application.Run(new FrmMain(vault, app, true));
        return 0;
    }

    /// <summary>--screenshot &lt;库目录&gt; &lt;密码&gt; &lt;输出png&gt;：把主界面渲染成图片（自检用）。</summary>
    private static int Screenshot(string[] args)
    {
        try
        {
            if (args.Length < 4)
            {
                Console.WriteLine("用法：--screenshot <库目录> <密码> <输出png>");
                return 1;
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var vault = Vault.Open(args[1], args[2]);
            using var form = new FrmMain(vault, new AppState(), true, false);
            form.Show();
            for (int i = 0; i < 12; i++)
            {
                Application.DoEvents();
                Thread.Sleep(40);
            }

            using var bmp = new Bitmap(form.Width, form.Height);
            form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
            string outPath = Path.GetFullPath(args[3]);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
            bmp.Save(outPath, ImageFormat.Png);
            form.Hide();

            string dir = Path.GetDirectoryName(outPath)!;
            using (var settings = new FrmSettings(vault))
            {
                settings.Show();
                Pump();
                Capture(settings, Path.Combine(dir, "settings.png"));
                settings.Hide();
            }

            using (var dlg = new FrmVaultDialog(FrmVaultDialog.DialogMode.Create, "D:\\TimeCapsule", vault.Settings.SourceRoot))
            {
                dlg.Show();
                Pump();
                Capture(dlg, Path.Combine(dir, "newvault.png"));
                dlg.Hide();
            }

            string line = "截图已保存：" + outPath + " (" + bmp.Width + "x" + bmp.Height + ")，另附 settings.png / newvault.png";
            Console.WriteLine(line);
            TryWriteReport(dir, "screenshot.log", line);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("截图失败：" + ex);
            return 1;
        }
    }

    /// <summary>--geom &lt;库目录&gt; &lt;密码&gt;：检查界面控件是否重叠/越界。</summary>
    private static int GeometryCheck(string[] args)
    {
        try
        {
            if (args.Length < 3)
            {
                Console.WriteLine("用法：--geom <库目录> <密码>");
                return 1;
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var vault = Vault.Open(args[1], args[2]);
            using var form = new FrmMain(vault, new AppState(), false, false);
            form.Show();
            for (int i = 0; i < 12; i++)
            {
                Application.DoEvents();
                Thread.Sleep(40);
            }

            var sb = new StringBuilder();
            foreach (var size in new[] { new Size(1180, 760), new Size(1000, 620), new Size(1600, 980) })
            {
                form.Size = size;
                for (int i = 0; i < 8; i++)
                {
                    Application.DoEvents();
                    Thread.Sleep(30);
                }
                var issues = form.CheckGeometry();
                sb.AppendLine($"== 尺寸 {form.Width}x{form.Height}：问题 {issues.Count} 条 ==");
                foreach (var issue in issues) sb.AppendLine("  " + issue);
            }

            form.Hide();
            string text = sb.ToString();
            Console.WriteLine(text);
            string exeDir = AppContext.BaseDirectory;
            TryWriteReport(Path.Combine(exeDir, "reports"), "geom.log", text);
            return text.Contains("问题 0 条") ? 0 : 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine("几何自检失败：" + ex);
            return 1;
        }
    }

    /// <summary>
    /// --designcheck：检查每个窗体是否满足「VS 设计器能打开」的条件。
    /// 设计器要的是：public 无参构造函数、构造不抛异常、InitializeComponent 建出的控件树完整。
    /// </summary>
    private static int DesignCheck()
    {
        var lines = new List<string>();
        int failures = 0;

        void Check(string name, bool ok, string detail = "")
        {
            if (!ok) failures++;
            lines.Add((ok ? "PASS  " : "FAIL  ") + name + (detail.Length > 0 ? "   [" + detail + "]" : ""));
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var forms = new[] { typeof(FrmMain), typeof(FrmVaultDialog), typeof(FrmSettings) };
        foreach (var type in forms)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            Check(type.Name + " 有 public 无参构造函数（设计器必需）", ctor != null);
            if (ctor == null) continue;

            Form? form = null;
            try
            {
                form = (Form)ctor.Invoke(null);
            }
            catch (Exception ex)
            {
                Check(type.Name + " 能被设计器实例化", false, ex.InnerException?.Message ?? ex.Message);
                continue;
            }

            Check(type.Name + " 能被设计器实例化", true);
            try
            {
                int count = CountControls(form);
                Check(type.Name + " 控件树完整", count > 3, count + " 个控件");
                Check(type.Name + " 有标题文字", !string.IsNullOrWhiteSpace(form.Text), form.Text);

                var init = type.GetMethod("InitializeComponent",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                Check(type.Name + " 用 InitializeComponent 建控件（设计器可编辑）", init != null,
                    init == null ? "找不到 InitializeComponent" : "找到");
            }
            finally
            {
                form?.Dispose();
            }
        }

        lines.Add("设计器兼容性检查：失败 " + failures + " 项");
        string text = string.Join(Environment.NewLine, lines);
        Console.WriteLine(text);
        TryWriteReport(Path.Combine(AppContext.BaseDirectory, "reports"), "designcheck.log", text);
        return failures == 0 ? 0 : 1;
    }

    private static int CountControls(Control root)
    {
        int n = 0;
        foreach (Control c in root.Controls)
        {
            n += 1 + CountControls(c);
        }
        return n;
    }

    /// <summary>--uitest &lt;工作目录&gt;：走一遍真实的建库对话框 → 主界面 → 首次备份。</summary>
    private static int UiTest(string[] args)
    {
        var lines = new List<string>();
        int failures = 0;

        void Check(string name, bool ok, string detail = "")
        {
            if (!ok) failures++;
            lines.Add((ok ? "PASS  " : "FAIL  ") + name + (detail.Length > 0 ? "   [" + detail + "]" : ""));
            Console.WriteLine(lines[^1]);
        }

        try
        {
            if (args.Length < 2)
            {
                Console.WriteLine("用法：--uitest <工作目录>");
                return 1;
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string root = Path.Combine(args[1], "uitest-" + DateTime.Now.ToString("HHmmss"));
            string src = Path.Combine(root, "被监控目录");
            string lib = Path.Combine(root, "总备份库");
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src, "说明.txt"), "界面测试用文件\r\n", new UTF8Encoding(false));
            Directory.CreateDirectory(Path.Combine(src, "子目录"));
            File.WriteAllText(Path.Combine(src, "子目录", "第二个文件.txt"), "hello\r\n", new UTF8Encoding(false));

            string credPath = Path.Combine(AppState.Dir, "cred.dat");
            bool hadCred = File.Exists(credPath);
            byte[]? oldCred = hadCred ? File.ReadAllBytes(credPath) : null;

            try
            {
                using var dlg = new FrmVaultDialog(FrmVaultDialog.DialogMode.Create, lib, src);
                dlg.Show();
                Pump();
                dlg.AutoFillForTest(lib, "ui-test-pass", src);
                dlg.ClickPrimaryForTest();
                for (int i = 0; i < 120 && dlg.Vault == null && dlg.Visible; i++)
                {
                    Application.DoEvents();
                    Thread.Sleep(50);
                }
                Check("建库对话框完成创建", dlg.Vault != null, dlg.Vault == null ? "对话框没有返回库" : dlg.Vault.Root);
                Check("库文件已生成", Vault.LooksLikeVault(lib));

                if (dlg.Vault == null)
                {
                    return 1;
                }

                var vault = dlg.Vault;
                Check("密码已记住并可用", CredentialStore.Load(lib) == "ui-test-pass");

                var app = new AppState { LastVaultPath = lib };
                using var main = new FrmMain(vault, app, false, false);
                main.Show();
                Pump();
                var issues = main.CheckGeometry();
                Check("主界面几何自检", issues.Count == 0, string.Join("；", issues.Take(3)));

                var engine = new BackupEngine(vault);
                var result = engine.Run("manual");
                Check("首次备份成功", result != null && result.Created && result.FileCount == 2,
                    result == null ? "null" : $"文件 {result.FileCount} 个");

                main.Hide();
            }
            finally
            {
                if (hadCred && oldCred != null) File.WriteAllBytes(credPath, oldCred);
                else if (!hadCred) CredentialStore.Clear();
            }

            lines.Add("界面自检结果：失败 " + failures + " 项");
            TryWriteReport(Path.Combine(AppContext.BaseDirectory, "reports"), "uitest.log", string.Join(Environment.NewLine, lines));
            return failures == 0 ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("界面自检失败：" + ex);
            try { TryWriteReport(Path.Combine(AppContext.BaseDirectory, "reports"), "uitest.log", ex.ToString()); } catch { }
            return 1;
        }
    }

    /// <summary>--watchtest &lt;库&gt; &lt;密码&gt; &lt;探针文件&gt;：验证文件变化能自动触发备份。</summary>
    private static int WatchTest(string[] args)
    {
        try
        {
            if (args.Length < 4)
            {
                Console.WriteLine("用法：--watchtest <库目录> <密码> <探针文件全路径>");
                return 1;
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var vault = Vault.Open(args[1], args[2]);
            string probe = args[3];
            int before = new SnapshotStore(vault).Count();
            Console.WriteLine("开始前快照数：" + before);

            var form = new FrmMain(vault, new AppState(), true, false);
            form.Show();

            int elapsed = 0;
            bool wrote = false;
            int exitCode = 1;
            var timer = new System.Windows.Forms.Timer { Interval = 500 };
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                if (!wrote && elapsed >= 3000)
                {
                    wrote = true;
                    File.WriteAllText(probe, "watch test " + DateTime.Now.ToString("O"));
                    Console.WriteLine("已改动文件，等待自动备份…");
                }
                if (elapsed >= 20000)
                {
                    timer.Stop();
                    int after = new SnapshotStore(vault).Count();
                    bool ok = after > before;
                    string line = ok
                        ? $"PASS 文件变化自动触发备份：{before} → {after} 张快照"
                        : $"FAIL 没有自动生成快照（仍为 {after} 张）";
                    Console.WriteLine(line);
                    TryWriteReport(Path.Combine(AppContext.BaseDirectory, "reports"), "watchtest.log", line);
                    exitCode = ok ? 0 : 1;
                    Application.Exit();
                }
            };
            timer.Start();
            Application.Run(form);
            return exitCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine("监控测试失败：" + ex);
            return 1;
        }
    }

    private static void Pump()
    {
        for (int i = 0; i < 10; i++)
        {
            Application.DoEvents();
            Thread.Sleep(30);
        }
    }

    private static void Capture(Form form, string path)
    {
        using var bmp = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
        bmp.Save(path, ImageFormat.Png);
    }

    private static void TryWriteReport(string dir, string name, string text)
    {
        try
        {
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, name), text, new UTF8Encoding(false));
        }
        catch { /* 忽略 */ }
    }
}
