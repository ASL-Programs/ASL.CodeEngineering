using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
