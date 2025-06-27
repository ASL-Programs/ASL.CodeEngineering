using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class LogWriteErrorTests : IDisposable
{
    private readonly string _dir;

    public LogWriteErrorTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_dir);
    }

    [Fact]
    public void MainWindow_LogError_ReadOnlyDir_NoException()
    {
        string logsDir = Path.Combine(_dir, "logs");
        Directory.CreateDirectory(logsDir);
        var info = new DirectoryInfo(logsDir);
        info.Attributes |= FileAttributes.ReadOnly;
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        var method = typeof(MainWindow).GetMethod("LogError", BindingFlags.NonPublic | BindingFlags.Static)!;
        method.Invoke(null, new object?[] { "op", new Exception("fail") });

        info.Attributes &= ~FileAttributes.ReadOnly;
    }

    [Fact]
    public async Task ProcessRunner_Log_ReadOnlyDir_NoException()
    {
        if (!TestHelpers.ToolExists("dotnet"))
            throw new SkipException("dotnet not installed");

        string logsDir = Path.Combine(_dir, "proc_logs");
        Directory.CreateDirectory(logsDir);
        var info = new DirectoryInfo(logsDir);
        info.Attributes |= FileAttributes.ReadOnly;
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        await ProcessRunner.RunAsync("dotnet", "--version", _dir, "pr");

        info.Attributes &= ~FileAttributes.ReadOnly;
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
