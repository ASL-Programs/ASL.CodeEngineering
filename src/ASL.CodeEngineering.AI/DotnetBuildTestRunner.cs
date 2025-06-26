using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Diagnostics;

namespace ASL.CodeEngineering.AI;

public class DotnetBuildTestRunner : IBuildTestRunner
{
    public string Name => "Dotnet";

    public async Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await RunProcess("dotnet", "build", projectPath, "build", cancellationToken);
    }

    public async Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await RunProcess("dotnet", "test", projectPath, "test", cancellationToken);
    }

    private static async Task<string> RunProcess(string fileName, string arguments, string workingDirectory, string op, CancellationToken ct)
    {
        var psi = new ProcessStartInfo(fileName, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };
        using var process = Process.Start(psi)!;
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync(ct);
        var result = string.IsNullOrWhiteSpace(error) ? output : output + System.Environment.NewLine + error;
        Log(op, result);
        return result.Trim();
    }

    private static void Log(string operation, string content)
    {
        var logsDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logsDir);
        var file = Path.Combine(logsDir, $"{operation}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
        File.WriteAllText(file, content);
    }
}
