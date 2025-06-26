using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class TodoAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_TodoFound()
    {
        var analyzer = new TodoAnalyzer();
        var result = await analyzer.AnalyzeAsync("// TODO: fix");
        Assert.Equal("TODO found", result);
    }

    [Fact]
    public async Task AnalyzeAsync_NoTodo()
    {
        var analyzer = new TodoAnalyzer();
        var result = await analyzer.AnalyzeAsync("Console.WriteLine(\"hi\");");
        Assert.Equal("No TODO", result);
    }
}

