using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering.Tests;

public class CompileProviderFixture : IDisposable
{
    public string ProvidersDirectory { get; };

    public CompileProviderFixture()
    {
        // create a unique temp directory for compiled provider assemblies
        ProvidersDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(ProvidersDirectory);
        CompileDummyProvider();
    }

    private void CompileDummyProvider()
    {
        const string code = """
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;

public class DummyProvider : IAIProvider
{
    public string Name => "Dummy";
    public bool RequiresNetwork => false;
    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
        => Task.FromResult("dummy:" + prompt);
}
""";

        var syntax = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAIProvider).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            "DummyProvider",
            new[] { syntax },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var path = Path.Combine(ProvidersDirectory, "DummyProvider.dll");
        var result = compilation.Emit(path);
        if (!result.Success)
        {
            throw new InvalidOperationException("Failed to compile dummy provider: " +
                string.Join(Environment.NewLine, result.Diagnostics));
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(ProvidersDirectory))
            Directory.Delete(ProvidersDirectory, true);
    }
}
