using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Windows;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowDuplicatePluginTests : IDisposable
{
    private readonly string _dir;
    private readonly string _logsDir;

    public MainWindowDuplicatePluginTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "plugins"));
        _logsDir = Path.Combine(_dir, "logs");
        Directory.CreateDirectory(_logsDir);
        CompileDuplicateAnalyzer();
    }

    private void CompileDuplicateAnalyzer()
    {
        const string code = @"using System.Threading;using System.Threading.Tasks;using ASL.CodeEngineering.AI;public class DupAnalyzer : IAnalyzerPlugin{public string Name=>\"Todo\";public Task<string> AnalyzeAsync(string code,CancellationToken ct=default)=>Task.FromResult(code);}";
        var syntax = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAnalyzerPlugin).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create("DupAnalyzer.dll", new[] { syntax }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var path = Path.Combine(_dir, "plugins", "DupAnalyzer.dll");
        var result = compilation.Emit(path);
        if (!result.Success)
            throw new InvalidOperationException("Failed to compile plugin");
    }

    [StaFact]
    public void Constructor_DuplicateAnalyzer_LogsWarning()
    {
        Environment.SetEnvironmentVariable("PLUGINS_DIR", Path.Combine(_dir, "plugins"));
        Environment.SetEnvironmentVariable("LOGS_DIR", _logsDir);

        var before = Directory.GetFiles(_logsDir);
        var window = new MainWindow();
        var after = Directory.GetFiles(_logsDir);
        Assert.True(after.Length > before.Length);
        window.Close();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("PLUGINS_DIR", null);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
