using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Updates AGENTS.md and NEXT_STEPS.md after each learning cycle.
/// Creates timestamped backups under docs/archive for rollback.
/// </summary>
public static class DocsUpdater
{
    public static void RecordCycle(string projectRoot, IEnumerable<string>? completedTasks, int cycleNumber)
    {
        string docsDir = Path.Combine(projectRoot, "docs", "archive");
        Directory.CreateDirectory(docsDir);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string agentsPath = Path.Combine(projectRoot, "AGENTS.md");
        string nextStepsPath = Path.Combine(projectRoot, "NEXT_STEPS.md");
        File.Copy(agentsPath, Path.Combine(docsDir, $"AGENTS_{timestamp}.md"), true);
        File.Copy(nextStepsPath, Path.Combine(docsDir, $"NEXT_STEPS_{timestamp}.md"), true);

        if (completedTasks != null)
        {
            UpdateFile(agentsPath, completedTasks);
            UpdateFile(nextStepsPath, completedTasks);
        }

        AppendCycle(agentsPath, cycleNumber);
    }

    public static void GenerateReleaseReport(string projectRoot, string versionPath)
    {
        string releasesDir = Path.Combine(projectRoot, "docs", "releases");
        Directory.CreateDirectory(releasesDir);
        string versionName = Path.GetFileName(versionPath.TrimEnd(Path.DirectorySeparatorChar));
        string reportPath = Path.Combine(releasesDir, $"{versionName}.md");

        string dataDir = Environment.GetEnvironmentVariable("DATA_DIR") ??
                            Path.Combine(projectRoot, "data");
        string versionsDir = Path.Combine(dataDir, "versions");
        string? prevVersion = Directory.GetDirectories(versionsDir)
                                      .OrderBy(d => d)
                                      .Where(d => d != versionPath)
                                      .LastOrDefault();

        var sb = new StringBuilder();
        sb.AppendLine($"# Release {versionName}");
        sb.AppendLine();
        sb.AppendLine("## Code Changes");
        if (prevVersion is null)
        {
            sb.AppendLine("Initial version.");
        }
        else
        {
            foreach (var change in SummarizeChanges(Path.Combine(prevVersion, "src"), Path.Combine(versionPath, "src")))
                sb.AppendLine($"- {change}");
        }
        sb.AppendLine();
        sb.AppendLine("## API Usage Examples");
        foreach (var ex in GetApiExamples(projectRoot, 3))
            sb.AppendLine($"- `{ex.provider}`: {ex.result}");
        sb.AppendLine();
        sb.AppendLine("## Training Outcomes");
        var metrics = TrainingMetricsAnalyzer.GetAverages(projectRoot);
        if (!double.IsNaN(metrics.Loss))
        {
            sb.AppendLine($"- Average loss: {metrics.Loss:F3}");
            sb.AppendLine($"- Average accuracy: {metrics.Accuracy:F3}");
        }
        else
        {
            sb.AppendLine("No training data.");
        }

        File.WriteAllText(reportPath, sb.ToString());
    }

    private static IEnumerable<string> SummarizeChanges(string prevDir, string newDir)
    {
        var prevFiles = Directory.Exists(prevDir)
            ? Directory.GetFiles(prevDir, "*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(prevDir, f))
            : Array.Empty<string>();
        var newFiles = Directory.Exists(newDir)
            ? Directory.GetFiles(newDir, "*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(newDir, f))
            : Array.Empty<string>();

        var prevSet = new HashSet<string>(prevFiles);
        var newSet = new HashSet<string>(newFiles);

        foreach (var added in newSet.Except(prevSet))
            yield return $"Added {added}";
        foreach (var removed in prevSet.Except(newSet))
            yield return $"Removed {removed}";
        foreach (var common in newSet.Intersect(prevSet))
        {
            string prevPath = Path.Combine(prevDir, common);
            string newPath = Path.Combine(newDir, common);
            if (!FileEquals(prevPath, newPath))
                yield return $"Modified {common}";
        }
    }

    private static bool FileEquals(string path1, string path2)
    {
        byte[] a = File.ReadAllBytes(path1);
        byte[] b = File.ReadAllBytes(path2);
        if (a.Length != b.Length)
            return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return false;
        return true;
    }

    private static IEnumerable<(string provider, string result)> GetApiExamples(string projectRoot, int count)
    {
        string path = Path.Combine(projectRoot, "knowledge_base", "auto", "auto.jsonl");
        if (!File.Exists(path))
            yield break;
        foreach (var line in File.ReadLines(path).Reverse().Take(count))
        {
            try
            {
                var doc = JsonDocument.Parse(line).RootElement;
                string provider = doc.GetProperty("provider").GetString() ?? "";
                string result = doc.GetProperty("result").GetString() ?? "";
                yield return (provider, result);
            }
            catch
            {
                // ignore malformed lines
            }
        }
    }

    private static void UpdateFile(string path, IEnumerable<string> tasks)
    {
        var lines = File.ReadAllLines(path).ToList();
        foreach (string task in tasks)
        {
            bool found = false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(task, StringComparison.OrdinalIgnoreCase))
                {
                    if (lines[i].Contains("[ ]"))
                    {
                        lines[i] = lines[i].Replace("[ ]", "[x]");
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                lines.Add($"- [x] {task}");
            }
        }
        File.WriteAllLines(path, lines);
    }

    private static void AppendCycle(string agentsPath, int cycleNumber)
    {
        File.AppendAllText(agentsPath, $"\n- [x] Learning cycle {cycleNumber} completed\n");
    }
}
