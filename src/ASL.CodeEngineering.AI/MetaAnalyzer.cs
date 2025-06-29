using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

public static class MetaAnalyzer
{
    public static void Analyze(string projectRoot)
    {
        string benchPath = Path.Combine(projectRoot, "knowledge_base", "benchmarks", "benchmarks.jsonl");
        if (!File.Exists(benchPath))
            return;
        var times = new Dictionary<string, List<long>>();
        var cpus = new Dictionary<string, List<double>>();
        var mems = new Dictionary<string, List<long>>();
        foreach (var line in File.ReadAllLines(benchPath))
        {
            try
            {
                var doc = JsonDocument.Parse(line).RootElement;
                string lang = doc.GetProperty("language").GetString() ?? "Unknown";
                long ms = doc.GetProperty("milliseconds").GetInt64();
                double cpu = doc.TryGetProperty("cpuMs", out var cpuEl) ? cpuEl.GetDouble() : 0;
                long mem = doc.TryGetProperty("memoryBytes", out var memEl) ? memEl.GetInt64() : 0;
                if (!times.ContainsKey(lang))
                {
                    times[lang] = new();
                    cpus[lang] = new();
                    mems[lang] = new();
                }
                times[lang].Add(ms);
                cpus[lang].Add(cpu);
                mems[lang].Add(mem);
            }
            catch
            {
                // ignore parse errors
            }
        }
        if (times.Count == 0)
            return;
        var averages = times.Select(kvp => new
                            {
                                language = kvp.Key,
                                average = kvp.Value.Average(),
                                cpuMs = cpus[kvp.Key].Count == 0 ? 0 : cpus[kvp.Key].Average(),
                                memoryBytes = mems[kvp.Key].Count == 0 ? 0 : mems[kvp.Key].Average()
                            })
                            .OrderBy(r => r.average)
                            .ToList();
        string metaDir = Path.Combine(projectRoot, "knowledge_base", "meta");
        Directory.CreateDirectory(metaDir);
        string path = Path.Combine(metaDir, "language_insights.json");
        File.WriteAllText(path, JsonSerializer.Serialize(averages, new JsonSerializerOptions { WriteIndented = true }));
    }
}
