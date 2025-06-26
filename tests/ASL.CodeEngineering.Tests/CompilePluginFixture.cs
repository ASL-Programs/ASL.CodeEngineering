using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering.Tests;

public class CompilePluginFixture : IDisposable
{
    public string BaseDirectory { get; }

    public CompilePluginFixture()
    {
        BaseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(BaseDirectory, "plugins"));
        CompileAnalyzer();
        CompileRunner();
        CompileBuildTestRunner();
    }

    private void CompileAnalyzer()
    {
        const string code = """
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;

public class DummyAnalyzer : IAnalyzerPlugin
{
    public string Name => \"DummyAnalyzer\";
    public Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default)
        => Task.FromResult(\"analyzed:\" + code);
}
""";
        Compile(code, "DummyAnalyzer.dll");
    }

    private void CompileRunner()
    {
        const string code = """
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;

public class DummyRunner : ICodeRunnerPlugin
{
    public string Name => \"DummyRunner\";
    public Task<string> RunAsync(string dir, CancellationToken cancellationToken = default)
        => Task.FromResult(\"ran:\" + dir);
}
""";
        Compile(code, "DummyRunner.dll");
    }

    private void CompileBuildTestRunner()
    {
        const string code = """
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;

public class DummyBuildTestRunner : IBuildTestRunner
{
    public string Name => \"DummyBuildTestRunner\";
    public Task<string> BuildAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(\"built:\" + path);
    public Task<string> TestAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(\"tested:\" + path);
}
""";
        Compile(code, "DummyBuildTestRunner.dll", typeof(IBuildTestRunner));
    }

    private void Compile(string source, string fileName, Type? interfaceType = null)
    {
        var syntax = CSharpSyntaxTree.ParseText(source);
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAnalyzerPlugin).Assembly.Location)
        };
        if (interfaceType != null)
            references.Add(MetadataReference.CreateFromFile(interfaceType.Assembly.Location));

        var compilation = CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(fileName),
            new[] { syntax },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var path = Path.Combine(BaseDirectory, "plugins", fileName);
        var result = compilation.Emit(path);
        if (!result.Success)
        {
            throw new InvalidOperationException("Failed to compile plugin: " +
                string.Join(Environment.NewLine, result.Diagnostics));
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(BaseDirectory))
            Directory.Delete(BaseDirectory, true);
    }
}
