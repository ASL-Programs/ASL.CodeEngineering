using System;
using System.IO;
using System.Text.Json;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class TrainingMetricsAnalyzerTests : IDisposable
{
    private readonly string _dir;

    public TrainingMetricsAnalyzerTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_dir);
    }

    [Fact]
    public void Record_WritesMetricsFile()
    {
        TrainingMetricsAnalyzer.Record(_dir, 0.5);
        string file = Path.Combine(_dir, "knowledge_base", "meta", "training_metrics.jsonl");
        Assert.True(File.Exists(file));
        var doc = JsonDocument.Parse(File.ReadAllText(file));
        Assert.Equal(0.5, doc.RootElement.GetProperty("loss").GetDouble(), 3);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
