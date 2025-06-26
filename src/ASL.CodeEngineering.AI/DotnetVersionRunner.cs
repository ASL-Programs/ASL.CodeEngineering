using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public class DotnetVersionRunner : ICodeRunnerPlugin
{
    public string Name => "DotnetVersion";

    public async Task<string> RunAsync(string workingDirectory, CancellationToken cancellationToken = default)
    {
        var psi = new ProcessStartInfo("dotnet", "--version")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };
        using var process = Process.Start(psi)!;
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync(cancellationToken);
        return output.Trim();
    }
}
