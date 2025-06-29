using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Records and aggregates offline training metrics.
/// </summary>
public static class TrainingMetricsAnalyzer
{
    public static void Record(string projectRoot, double loss)
    {
        double accuracy = 1.0 / (1.0 + loss);
        string metaDir = Path.Combine(projectRoot, "knowledge_base", "meta");
        Directory.CreateDirectory(metaDir);
        string path = Path.Combine(metaDir, "training_metrics.jsonl");
        var entry = new { timestamp = DateTime.UtcNow, loss, accuracy };
        File.AppendAllText(path, JsonSerializer.Serialize(entry) + Environment.NewLine);
    }

    public static (double Loss, double Accuracy) GetAverages(string projectRoot, int count = 5)
    {
        string path = Path.Combine(projectRoot, "knowledge_base", "meta", "training_metrics.jsonl");
        if (!File.Exists(path))
            return (double.NaN, double.NaN);

        var lines = File.ReadAllLines(path);
        if (lines.Length == 0)
            return (double.NaN, double.NaN);
        var recent = lines.TakeLast(Math.Min(count, lines.Length));
        var metrics = new List<(double loss, double acc)>();
        foreach (var line in recent)
        {
            try
            {
                var doc = JsonDocument.Parse(line).RootElement;
                double loss = doc.GetProperty("loss").GetDouble();
                double acc = doc.GetProperty("accuracy").GetDouble();
                metrics.Add((loss, acc));
            }
            catch
            {
                // ignore malformed entries
            }
        }
        if (metrics.Count == 0)
            return (double.NaN, double.NaN);
        return (metrics.Average(m => m.loss), metrics.Average(m => m.acc));
    }
}
