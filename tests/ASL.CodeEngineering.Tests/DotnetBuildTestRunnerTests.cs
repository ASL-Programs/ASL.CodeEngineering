using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class DotnetBuildTestRunnerTests
{
    [Fact]
    public async Task BuildAsync_LogsOutputAndReportsExitCode()
    {
        if (!TestHelpers.ToolExists("dotnet"))
            throw new SkipException("dotnet not installed");

        var runner = new DotnetBuildTestRunner();
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            var logsDir = Path.Combine(AppContext.BaseDirectory, "logs");
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
            Directory.Delete(temp.FullName, true);
        }
    }
}
