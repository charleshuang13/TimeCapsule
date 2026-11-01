using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeCapsule.Core;

public sealed class VaultSettings
{
    /// <summary>被监控的目录。</summary>
    public string SourceRoot { get; set; } = "";

    /// <summary>定时兜底扫描间隔（分钟）。</summary>
    public int IntervalMinutes { get; set; } = 30;

    /// <summary>变化后等待多少秒再拍快照（合并连续写入）。</summary>
    public int DebounceSeconds { get; set; } = 5;

    /// <summary>保留策略：keepAll 全留 / days 按天数 / totalSize 按总量上限。</summary>
    public string RetainMode { get; set; } = "keepAll";
    public int RetainDays { get; set; } = 30;
    public long RetainTotalMB { get; set; } = 50 * 1024;

    /// <summary>单文件大小上限（MB），0 = 不限。</summary>
    public int MaxFileSizeMB { get; set; } = 500;

    /// <summary>超过这些秒数没被写过的文件视为落定；0 = 不判断（照备）。</summary>
    public int SkipFilesNewerThanSeconds { get; set; } = 0;

    public List<string> ExcludeDirs { get; set; } = new()
    {
        "node_modules", ".git", ".vs", ".svn", "$RECYCLE.BIN", "System Volume Information", "obj"
    };

    public List<string> ExcludePatterns { get; set; } = new()
    {
        "*.tmp", "*.part", "*.crdownload", "~$*", "*.swp", "Thumbs.db", "desktop.ini"
    };

    public bool MinimizeToTray { get; set; } = true;
    public bool NotifyOnBackup { get; set; }
    public bool StartWithWindows { get; set; }

    public VaultSettings Clone()
    {
        return JsonSerializer.Deserialize<VaultSettings>(JsonSerializer.Serialize(this, JsonOpts))!;
    }

    internal static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };
}

public sealed class VaultConfig
{
    public int Version { get; set; } = 1;
    public string VaultId { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public int KdfIterations { get; set; } = 210_000;
    public string Salt { get; set; } = "";

    /// <summary>用主密钥加密的一段固定文本，用来校验密码是否正确。</summary>
    public string CheckBlob { get; set; } = "";

    public VaultSettings Settings { get; set; } = new();
}

public sealed class BadPasswordException : Exception
{
    public BadPasswordException() : base("密码不正确") { }
}

/// <summary>一个胶囊库：配置、主密钥、目录结构。</summary>
public sealed class Vault
{
    public const string ConfigFileName = "vault.json";
    private const string CheckText = "TIMECAPSULE-VAULT-VERIFY-V1";

    public string Root { get; }
    public VaultConfig Config { get; private set; }
    public byte[] MasterKey { get; }
    public Log Log { get; }

    private Vault(string root, VaultConfig config, byte[] masterKey)
    {
        Root = root;
        Config = config;
        MasterKey = masterKey;
        Log = new Log(Path.Combine(root, "logs"));
    }

    public string BlobDir => Path.Combine(Root, "blobs");
    public string SnapsDir => Path.Combine(Root, "snaps");
    public string ConfigPath => Path.Combine(Root, ConfigFileName);
    public VaultSettings Settings => Config.Settings;
    public string Id => Config.VaultId;

    public static bool LooksLikeVault(string root)
        => File.Exists(Path.Combine(root, ConfigFileName));

    public static Vault Create(string root, string password, string sourceRoot)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            throw new ArgumentException("密码至少 4 位");
        if (string.IsNullOrWhiteSpace(root))
            throw new ArgumentException("库位置不能为空");

        Directory.CreateDirectory(root);
        var salt = RandomNumberGenerator.GetBytes(32);
        int iterations = 210_000;
        var key = Crypto.DeriveMasterKey(password, salt, iterations);

        var cfg = new VaultConfig
        {
            VaultId = Guid.NewGuid().ToString("N")[..12],
            CreatedAt = DateTimeOffset.Now,
            KdfIterations = iterations,
            Salt = Convert.ToBase64String(salt),
            Settings = new VaultSettings { SourceRoot = sourceRoot ?? "" }
        };
        cfg.CheckBlob = Convert.ToBase64String(
            Crypto.EncryptBytes(Encoding.UTF8.GetBytes(CheckText), Crypto.DeriveSubKey(key, "check")));

        var vault = new Vault(root, cfg, key);
        Directory.CreateDirectory(vault.BlobDir);
        Directory.CreateDirectory(vault.SnapsDir);
        Directory.CreateDirectory(vault.Log.Dir);
        vault.SaveConfig();
        vault.Log.Info($"库已创建：{root}");
        return vault;
    }

    public static Vault Open(string root, string password)
    {
        string path = Path.Combine(root, ConfigFileName);
        if (!File.Exists(path)) throw new FileNotFoundException("这个目录不是时间胶囊库（缺少 vault.json）", path);

        VaultConfig? cfg;
        try
        {
            cfg = JsonSerializer.Deserialize<VaultConfig>(File.ReadAllText(path), VaultSettings.JsonOpts);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("库配置文件损坏，无法读取：" + ex.Message, ex);
        }
        if (cfg == null) throw new InvalidDataException("库配置文件为空");

        var salt = Convert.FromBase64String(cfg.Salt);
        var key = Crypto.DeriveMasterKey(password, salt, cfg.KdfIterations);

        bool ok;
        try
        {
            var plain = Crypto.DecryptBytes(Convert.FromBase64String(cfg.CheckBlob), Crypto.DeriveSubKey(key, "check"));
            ok = Encoding.UTF8.GetString(plain) == CheckText;
        }
        catch
        {
            ok = false;
        }
        if (!ok) throw new BadPasswordException();

        var vault = new Vault(root, cfg, key);
        Directory.CreateDirectory(vault.BlobDir);
        Directory.CreateDirectory(vault.SnapsDir);
        return vault;
    }

    public void SaveConfig()
    {
        Directory.CreateDirectory(Root);
        string tmp = ConfigPath + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(Config, VaultSettings.JsonOpts), new UTF8Encoding(false));
        if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
        File.Move(tmp, ConfigPath);
    }

    public byte[] ManifestKey => Crypto.DeriveSubKey(MasterKey, "manifest");
    public byte[] BlobKey(string hashHex) => Crypto.DeriveSubKey(MasterKey, "file:" + hashHex);
}
