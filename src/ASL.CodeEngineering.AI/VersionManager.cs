using System;
using System.IO;

namespace ASL.CodeEngineering.AI;

public static class VersionManager
{
    public static void SaveVersion(string projectRoot)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                            Path.Combine(projectRoot, "data");
        string versionsDir = Path.Combine(baseData, "versions");
        Directory.CreateDirectory(versionsDir);
        string stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string dest = Path.Combine(versionsDir, stamp);
        CopyDirectory(Path.Combine(projectRoot, "src"), Path.Combine(dest, "src"));
    }

    public static void RestoreLatest(string projectRoot)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                            Path.Combine(projectRoot, "data");
        string versionsDir = Path.Combine(baseData, "versions");
        if (!Directory.Exists(versionsDir))
            return;
        string? latest = null;
        foreach (var dir in Directory.GetDirectories(versionsDir))
        {
            if (latest is null || string.Compare(dir, latest, StringComparison.OrdinalIgnoreCase) > 0)
                latest = dir;
        }
        if (latest is null)
            return;
        string srcDir = Path.Combine(projectRoot, "src");
        if (Directory.Exists(srcDir))
            Directory.Delete(srcDir, true);
        CopyDirectory(Path.Combine(latest, "src"), srcDir);
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            string target = Path.Combine(destination, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite: true);
        }
    }
}
