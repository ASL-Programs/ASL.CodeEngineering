using System.Threading;
using System.Threading.Tasks;


namespace ASL.CodeEngineering.AI;

public class DotnetBuildTestRunner : IBuildTestRunner
{
    public string Name => "Dotnet";

    public async Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await ProcessRunner.RunAsync("dotnet", "build", projectPath, "build", cancellationToken);
    }

    public async Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await ProcessRunner.RunAsync("dotnet", "test", projectPath, "test", cancellationToken);
    }
}
