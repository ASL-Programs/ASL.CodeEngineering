using System;
using System.IO;
using System.Text.Json;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MetaAnalyzerTests : IDisposable
{
    private readonly string _dir;

    public MetaAnalyzerTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "knowledge_base", "benchmarks"));
    }

    [Fact]
    public void Analyze_WritesAverages()
    {
        string file = Path.Combine(_dir, "knowledge_base", "benchmarks", "benchmarks.jsonl");
        File.WriteAllText(file, "{\"language\":\"C#\",\"milliseconds\":2,\"cpuMs\":1,\"memoryBytes\":10}\n" +
                               "{\"language\":\"C#\",\"milliseconds\":4,\"cpuMs\":3,\"memoryBytes\":30}\n");
        MetaAnalyzer.Analyze(_dir);
        string outPath = Path.Combine(_dir, "knowledge_base", "meta", "language_insights.json");
        Assert.True(File.Exists(outPath));
        var doc = JsonDocument.Parse(File.ReadAllText(outPath));
        var first = doc.RootElement[0];
        Assert.Equal(3, first.GetProperty("average").GetDouble());
        Assert.Equal(2, first.GetProperty("cpuMs").GetDouble());
        Assert.Equal(20, first.GetProperty("memoryBytes").GetDouble());
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
