using System.IO;

namespace TimeCapsule.Core;

/// <summary>排除规则：目录名、文件名通配符。</summary>
public static class Exclusions
{
    public static bool IsExcludedDir(string name, VaultSettings s)
    {
        foreach (var d in s.ExcludeDirs)
        {
            if (string.IsNullOrWhiteSpace(d)) continue;
            if (string.Equals(d, name, StringComparison.OrdinalIgnoreCase)) return true;
            if (Wildcard(name, d)) return true;
        }
        return false;
    }

    public static bool IsExcludedFile(string name, VaultSettings s)
    {
        foreach (var p in s.ExcludePatterns)
        {
            if (string.IsNullOrWhiteSpace(p)) continue;
            if (Wildcard(name, p)) return true;
        }
        return false;
    }

    /// <summary>Windows 风格通配符匹配，支持 * 和 ?。</summary>
    public static bool Wildcard(string text, string pattern)
    {
        int t = 0, p = 0, star = -1, mark = 0;
        while (t < text.Length)
        {
            if (p < pattern.Length && (pattern[p] == '?' ||
                char.ToLowerInvariant(pattern[p]) == char.ToLowerInvariant(text[t])))
            {
                t++; p++;
            }
            else if (p < pattern.Length && pattern[p] == '*')
            {
                star = p++;
                mark = t;
            }
            else if (star >= 0)
            {
                p = star + 1;
                t = ++mark;
            }
            else
            {
                return false;
            }
        }
        while (p < pattern.Length && pattern[p] == '*') p++;
        return p == pattern.Length;
    }
}
