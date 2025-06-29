using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public static class ProcessRunner
{
    public static async Task<string> RunAsync(string fileName, string arguments, string workingDirectory, string logPrefix, CancellationToken cancellationToken = default)
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
        await process.WaitForExitAsync(cancellationToken);
        var result = string.IsNullOrWhiteSpace(error) ? output : output + Environment.NewLine + error;
        if (process.ExitCode != 0)
        {
            result += Environment.NewLine + $"Exit code: {process.ExitCode}";
        }
        Log(logPrefix, result);
        return result.Trim();
    }

    public static async Task<(string Output, double CpuMs, long PeakMemory)> RunWithMetricsAsync(string fileName, string arguments, string workingDirectory, string logPrefix, CancellationToken cancellationToken = default)
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
        await process.WaitForExitAsync(cancellationToken);
        var result = string.IsNullOrWhiteSpace(error) ? output : output + Environment.NewLine + error;
        if (process.ExitCode != 0)
        {
            result += Environment.NewLine + $"Exit code: {process.ExitCode}";
        }
        Log(logPrefix, result);
        return (result.Trim(), process.TotalProcessorTime.TotalMilliseconds, process.PeakWorkingSet64);
    }

    private static void Log(string prefix, string content)
    {
        SecureLogger.Write(prefix, content);
    }
}
