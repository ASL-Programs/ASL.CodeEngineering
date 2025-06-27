using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class AIProviderLoaderDuplicateTests : IDisposable
{
    private readonly string _dir;

    public AIProviderLoaderDuplicateTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "ai_providers"));
        CompileDuplicates();
    }

    private void Compile(string source, string fileName)
    {
        var syntax = CSharpSyntaxTree.ParseText(source);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAIProvider).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(fileName),
            new[] { syntax }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var path = Path.Combine(_dir, "ai_providers", fileName);
        var result = compilation.Emit(path);
        if (!result.Success)
            throw new InvalidOperationException("Failed to compile provider");
    }

    private void CompileDuplicates()
    {
        const string code1 = @"
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
public class FirstProvider : IAIProvider
{
    public string Name => \"Dup\";
    public bool RequiresNetwork => false;
    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default) => Task.FromResult(prompt);
}";
        const string code2 = @"
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
public class SecondProvider : IAIProvider
{
    public string Name => \"Dup\";
    public bool RequiresNetwork => false;
    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default) => Task.FromResult(prompt);
}";
        Compile(code1, "First.dll");
        Compile(code2, "Second.dll");
    }

    [Fact]
    public void LoadProviders_DuplicateNameThrows()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => AIProviderLoader.LoadProviders(_dir));
        Assert.Contains("Duplicate AI provider name 'Dup'", ex.Message);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
