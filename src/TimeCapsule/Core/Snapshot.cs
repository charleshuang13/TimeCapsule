using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeCapsule.Core;

public sealed class SnapshotEntry
{
    public string Path { get; set; } = "";
    public string Hash { get; set; } = "";
    public long Size { get; set; }
    public DateTime MtimeUtc { get; set; }
    public long BlobSize { get; set; }
}

public sealed class SnapshotManifest
{
    public int Version { get; set; } = 1;
    public string Id { get; set; } = "";
    public DateTimeOffset TakenAt { get; set; }
    public string SourceRoot { get; set; } = "";
    public string Trigger { get; set; } = "manual";
    public string? Prev { get; set; }
    public int FileCount { get; set; }
    public long TotalBytes { get; set; }
    public int Added { get; set; }
    public int Modified { get; set; }
    public int Removed { get; set; }
    public List<SnapshotEntry> Entries { get; set; } = new();

    [JsonIgnore]
    public long StoredBytes => Entries.Sum(e => e.BlobSize);
}

/// <summary>快照摘要（列表用，不加载条目）。</summary>
public sealed class SnapshotInfo
{
    public string Id { get; set; } = "";
    public DateTimeOffset TakenAt { get; set; }
    public string Trigger { get; set; } = "manual";
    public int FileCount { get; set; }
    public long TotalBytes { get; set; }
    public int Added { get; set; }
    public int Modified { get; set; }
    public int Removed { get; set; }
    public string? Prev { get; set; }

    public static SnapshotInfo From(SnapshotManifest m) => new()
    {
        Id = m.Id,
        TakenAt = m.TakenAt,
        Trigger = m.Trigger,
        FileCount = m.FileCount,
        TotalBytes = m.TotalBytes,
        Added = m.Added,
        Modified = m.Modified,
        Removed = m.Removed,
        Prev = m.Prev
    };

    public string TriggerText => Trigger switch
    {
        "watch" => "变化触发",
        "timer" => "定时扫描",
        "manual" => "手动备份",
        _ => Trigger
    };

    public string ChangeText
    {
        get
        {
            var parts = new List<string>();
            if (Added > 0) parts.Add("新增 " + Added);
            if (Modified > 0) parts.Add("修改 " + Modified);
            if (Removed > 0) parts.Add("删除 " + Removed);
            return parts.Count == 0 ? "无变化" : string.Join(" / ", parts);
        }
    }
}

public static class Humanize
{
    public static string Size(long bytes)
    {
        if (bytes < 1024) return bytes + " B";
        double v = bytes / 1024.0;
        if (v < 1024) return v.ToString("0.#") + " KB";
        v /= 1024.0;
        if (v < 1024) return v.ToString("0.##") + " MB";
        v /= 1024.0;
        return v.ToString("0.##") + " GB";
    }
}
