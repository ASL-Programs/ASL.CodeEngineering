using System.Diagnostics;

namespace ASL.CodeEngineering.Tests;

public static class TestHelpers
{
    public static bool ToolExists(string name)
    {
        try
        {
            var psi = new ProcessStartInfo(name, "--version")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            if (process == null)
                return false;
            if (!process.WaitForExit(1000))
                process.Kill(true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
