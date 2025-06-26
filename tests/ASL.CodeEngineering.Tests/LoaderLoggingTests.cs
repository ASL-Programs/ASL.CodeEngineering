using System;
using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class LoaderLoggingTests : IDisposable
{
    private readonly string _dir;
    private readonly string _logsDir;

    public LoaderLoggingTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "plugins"));
        Directory.CreateDirectory(Path.Combine(_dir, "ai_providers"));
        File.WriteAllText(Path.Combine(_dir, "plugins", "bad.dll"), "not a dll");
        File.WriteAllText(Path.Combine(_dir, "ai_providers", "bad.dll"), "not a dll");
        _logsDir = Path.Combine(_dir, "logs");
    }

    [Fact]
    public void LoadProviders_InvalidAssembly_LogsError()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", _logsDir);
        Directory.CreateDirectory(_logsDir);
        var before = Directory.GetFiles(_logsDir);

        AIProviderLoader.LoadProviders(_dir);

        var after = Directory.GetFiles(_logsDir);
        Assert.True(after.Length > before.Length);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
    }

    [Fact]
    public void LoadPlugins_InvalidAssembly_LogsError()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", _logsDir);
        Directory.CreateDirectory(_logsDir);
        var before = Directory.GetFiles(_logsDir);

        PluginLoader.LoadAnalyzers(_dir);

        var after = Directory.GetFiles(_logsDir);
        Assert.True(after.Length > before.Length);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
