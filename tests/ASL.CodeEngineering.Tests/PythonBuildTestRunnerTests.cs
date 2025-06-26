using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class PythonBuildTestRunnerTests
{
    [Fact]
    public async Task BuildAsync_LogsOutputAndReportsExitCode()
    {
        if (!TestHelpers.ToolExists("python"))
            throw new SkipException("python not installed");

        var runner = new PythonBuildTestRunner();
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllText(Path.Combine(temp.FullName, "bad.py"), "def f(:\n");
            var logsDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);
            Directory.CreateDirectory(logsDir);
            var before = Directory.GetFiles(logsDir);

            string result = await runner.BuildAsync(temp.FullName);
            Assert.Contains("Exit code", result);

            var after = Directory.GetFiles(logsDir);
            var newLog = after.Except(before).FirstOrDefault();
            Assert.NotNull(newLog);
            var logContent = File.ReadAllText(newLog!);
            Assert.Contains(result.Trim(), logContent);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LOGS_DIR", null);
            Directory.Delete(temp.FullName, true);
        }
    }
}
