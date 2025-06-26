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

    public PluginLoaderDuplicateTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "plugins"));
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
    public void LoadAnalyzers_DuplicateNameThrows()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => PluginLoader.LoadAnalyzers(_dir));
        Assert.Contains("Duplicate plugin name 'DupPlugin'", ex.Message);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
