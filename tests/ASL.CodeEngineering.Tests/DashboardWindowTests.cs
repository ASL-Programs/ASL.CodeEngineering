using System;
using System.IO;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class DashboardWindowTests : IDisposable
{
    private readonly string _dir;

    public DashboardWindowTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "knowledge_base", "plans"));
        Directory.CreateDirectory(Path.Combine(_dir, "knowledge_base", "meta"));
        Directory.CreateDirectory(Path.Combine(_dir, "knowledge_base", "benchmarks"));
        File.WriteAllText(Path.Combine(_dir, "knowledge_base", "plans", "plans.json"),
            "[{\"Name\":\"Module\",\"Tasks\":[],\"Language\":\"C#\"}]");
        File.WriteAllText(Path.Combine(_dir, "knowledge_base", "meta", "language_insights.json"),
            "[{\"language\":\"C#\",\"average\":1}]");
        File.WriteAllText(Path.Combine(_dir, "knowledge_base", "meta", "crawl.jsonl"),
            "{}");
        File.WriteAllText(Path.Combine(_dir, "knowledge_base", "benchmarks", "benchmarks.jsonl"),
            "{\"language\":\"C#\",\"milliseconds\":1,\"cpuMs\":1,\"memoryBytes\":1}\n");
    }

    [StaFact]
    public void Constructor_LoadsData()
    {
        var window = new ASL.CodeEngineering.DashboardWindow(_dir);
        Assert.NotNull(window.PlanGrid.ItemsSource);
        Assert.NotNull(window.SummaryGrid.ItemsSource);
        Assert.NotNull(window.InsightGrid.ItemsSource);
        Assert.NotNull(window.BenchmarkGrid.ItemsSource);
        window.Close();
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}

