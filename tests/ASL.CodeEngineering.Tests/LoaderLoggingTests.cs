using System;
using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class LoaderLoggingTests : IDisposable
{
    private readonly string _dir;

    public LoaderLoggingTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "plugins"));
        Directory.CreateDirectory(Path.Combine(_dir, "ai_providers"));
        File.WriteAllText(Path.Combine(_dir, "plugins", "bad.dll"), "not a dll");
        File.WriteAllText(Path.Combine(_dir, "ai_providers", "bad.dll"), "not a dll");
    }

    [Fact]
    public void LoadProviders_InvalidAssembly_LogsError()
    {
        var logsDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logsDir);
        var before = Directory.GetFiles(logsDir);

        AIProviderLoader.LoadProviders(_dir);

        var after = Directory.GetFiles(logsDir);
        Assert.True(after.Length > before.Length);
    }

    [Fact]
    public void LoadPlugins_InvalidAssembly_LogsError()
    {
        var logsDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logsDir);
        var before = Directory.GetFiles(logsDir);

        PluginLoader.LoadAnalyzers(_dir);

        var after = Directory.GetFiles(logsDir);
        Assert.True(after.Length > before.Length);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
