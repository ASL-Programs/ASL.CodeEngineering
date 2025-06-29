using System.Threading;
using System.Threading.Tasks;


namespace ASL.CodeEngineering.AI;

public class DotnetBuildTestRunner : IBuildTestRunner
{
    public string Name => "Dotnet";

    public async Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        var result = await BuildWithMetricsAsync(projectPath, cancellationToken);
        return result.Output;
    }

    public async Task<(string Output, double CpuMs, long PeakMemory)> BuildWithMetricsAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await ProcessRunner.RunWithMetricsAsync("dotnet", "build", projectPath, "build", cancellationToken);
    }

    public async Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await ProcessRunner.RunAsync("dotnet", "test", projectPath, "test", cancellationToken);
    }
}
