using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class BenchmarkHarnessTests
{
    [Fact]
    public async Task RunAsync_WritesBenchmarkResults()
    {
        if (!TestHelpers.ToolExists("dotnet") || !TestHelpers.ToolExists("python"))
            throw new SkipException("dotnet or python not installed");

        string kbDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Environment.SetEnvironmentVariable("KB_DIR", kbDir);
        Directory.CreateDirectory(kbDir);

        await BenchmarkHarness.RunAsync();

        string file = Path.Combine(kbDir, "benchmarks", "benchmarks.jsonl");
        Assert.True(File.Exists(file));
        string[] lines = File.ReadAllLines(file);
        Assert.True(lines.Length >= 2);
        var doc = System.Text.Json.JsonDocument.Parse(lines[0]).RootElement;
        Assert.True(doc.TryGetProperty("cpuMs", out _));
        Assert.True(doc.TryGetProperty("memoryBytes", out _));

        Environment.SetEnvironmentVariable("KB_DIR", null);
        Directory.Delete(kbDir, true);
    }
}
