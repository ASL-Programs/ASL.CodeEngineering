using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class PluginLoaderDuplicateTests : IDisposable
{
    private readonly string _dir;
    private readonly string _logsDir;

    public PluginLoaderDuplicateTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "plugins"));
        _logsDir = Path.Combine(_dir, "logs");
        Directory.CreateDirectory(_logsDir);
        CompileDuplicates();
    }

    private void Compile(string source, string fileName)
    {
        var syntax = CSharpSyntaxTree.ParseText(source);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAnalyzerPlugin).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(fileName),
            new[] { syntax }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var path = Path.Combine(_dir, "plugins", fileName);
        var result = compilation.Emit(path);
        if (!result.Success)
            throw new InvalidOperationException("Failed to compile plugin");
    }

    private void CompileDuplicates()
    {
        const string code1 = @"
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
public class FirstPlugin : IAnalyzerPlugin
{
    public string Name => \"DupPlugin\";
    public Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(code);
}";
        const string code2 = @"
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
public class SecondPlugin : IAnalyzerPlugin
{
    public string Name => \"DupPlugin\";
    public Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(code);
}";
        Compile(code1, "First.dll");
        Compile(code2, "Second.dll");
    }

    [Fact]
    public void LoadAnalyzers_DuplicateNameLogsAndSkips()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", _logsDir);
        var before = Directory.GetFiles(_logsDir);

        var plugins = PluginLoader.LoadAnalyzers(_dir);

        var after = Directory.GetFiles(_logsDir);
        Assert.True(after.Length > before.Length);
        Assert.Single(plugins);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
