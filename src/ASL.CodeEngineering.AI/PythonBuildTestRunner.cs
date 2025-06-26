using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public class PythonBuildTestRunner : IBuildTestRunner
{
    public string Name => "Python";

    public async Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(projectPath, "*.py", SearchOption.AllDirectories);
        if (files.Length == 0)
            return "No Python files";
        var args = "-m py_compile " + string.Join(' ', files.Select(f => $"\"{f}\""));
        return await RunProcess("python", args, projectPath, "pybuild", cancellationToken);
    }

    public async Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await RunProcess("pytest", string.Empty, projectPath, "pytest", cancellationToken);
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
        var result = string.IsNullOrWhiteSpace(error) ? output : output + Environment.NewLine + error;
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
