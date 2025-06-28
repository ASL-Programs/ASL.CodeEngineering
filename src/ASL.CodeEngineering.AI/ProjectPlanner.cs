using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public record ModulePlan(string Name, List<string> Tasks);

/// <summary>
/// Reads AGENTS.md and the source directories to generate per-module plans.
/// Plans are written to knowledge_base/plans/plans.json so they can be inspected
/// by other modules or users.
/// </summary>
public static class ProjectPlanner
{
    public static IReadOnlyList<ModulePlan> GeneratePlans(string projectRoot)
    {
        string agentsPath = Path.Combine(projectRoot, "AGENTS.md");
        var openTasks = new List<string>();
        if (File.Exists(agentsPath))
        {
            foreach (var line in File.ReadAllLines(agentsPath))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("- [ ]"))
                    openTasks.Add(trimmed.Substring(5).Trim());
            }
        }

        string srcRoot = Path.Combine(projectRoot, "src");
        var modules = Directory.Exists(srcRoot)
            ? Directory.GetDirectories(srcRoot).Select(Path.GetFileName)!
            : Array.Empty<string>();

        var plans = new List<ModulePlan>();
        foreach (var module in modules)
        {
            var related = openTasks
                .Where(t => t.Contains(module, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (related.Count == 0)
                related.Add("No specific tasks");
            plans.Add(new ModulePlan(module!, related));
        }

        string planDir = Path.Combine(projectRoot, "knowledge_base", "plans");
        Directory.CreateDirectory(planDir);
        string planPath = Path.Combine(planDir, "plans.json");
        File.WriteAllText(planPath, JsonSerializer.Serialize(plans, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        return plans;
    }

    public static async Task ExecutePlansAsync(string projectRoot, CancellationToken token = default)
    {
        var plans = GeneratePlans(projectRoot);
        var runner = new DotnetBuildTestRunner();
        foreach (var plan in plans)
        {
            string path = Path.Combine(projectRoot, "src", plan.Name);
            if (Directory.Exists(path) && Directory.GetFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).Any())
            {
                await runner.BuildAsync(path, token);
                await runner.TestAsync(path, token);
            }
        }
    }
}
