using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public static class BenchmarkHarness
{
    public static async Task RunAsync(CancellationToken cancellationToken = default)
    {
        string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ??
                         Path.Combine(AppContext.BaseDirectory, "knowledge_base");
        string benchDir = Path.Combine(baseKb, "benchmarks");
        Directory.CreateDirectory(benchDir);
        string resultPath = Path.Combine(benchDir, "benchmarks.jsonl");

        await RunRunnerAsync(new DotnetBuildTestRunner(), CreateDotnetSample(), resultPath, cancellationToken);
        await RunRunnerAsync(new PythonBuildTestRunner(), CreatePythonSample(), resultPath, cancellationToken);
    }

    private static async Task RunRunnerAsync(object runner, string projectPath, string resultPath, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        (string Output, double CpuMs, long PeakMemory) metrics = runner switch
        {
            DotnetBuildTestRunner d => await d.BuildWithMetricsAsync(projectPath, cancellationToken),
            PythonBuildTestRunner p => await p.BuildWithMetricsAsync(projectPath, cancellationToken),
            IBuildTestRunner r => (await ProcessRunner.RunWithMetricsAsync(r.Name.ToLowerInvariant(), "build", projectPath, "build", cancellationToken)),
            _ => ("", 0, 0)
        };
        sw.Stop();
        bool success = !metrics.Output.Contains("Exit code", StringComparison.OrdinalIgnoreCase);
        var entry = new
        {
            timestamp = DateTime.UtcNow,
            language = runner is IBuildTestRunner br ? br.Name : "Unknown",
            success,
            milliseconds = sw.ElapsedMilliseconds,
            cpuMs = metrics.CpuMs,
            memoryBytes = metrics.PeakMemory
        };
        string line = JsonSerializer.Serialize(entry);
        File.AppendAllText(resultPath, line + Environment.NewLine);
        Directory.Delete(projectPath, true);
    }

    private static string CreateDotnetSample()
    {
        string dir = Directory.CreateTempSubdirectory().FullName;
        File.WriteAllText(Path.Combine(dir, "Sample.csproj"),
            "<Project Sdk=\"Microsoft.NET.Sdk\">\n  <PropertyGroup>\n    <OutputType>Exe</OutputType>\n    <TargetFramework>net7.0</TargetFramework>\n  </PropertyGroup>\n</Project>\n");
        File.WriteAllText(Path.Combine(dir, "Program.cs"), "Console.WriteLine(\"hi\");\n");
        return dir;
    }

    private static string CreatePythonSample()
    {
        string dir = Directory.CreateTempSubdirectory().FullName;
        File.WriteAllText(Path.Combine(dir, "script.py"), "print('hi')\n");
        return dir;
    }
}
