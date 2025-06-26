using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public class TodoAnalyzer : IAnalyzerPlugin
{
    public string Name => "Todo";

    public Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default)
    {
        var result = code.Contains("TODO", StringComparison.OrdinalIgnoreCase)
            ? "TODO found"
            : "No TODO";
        return Task.FromResult(result);
    }
}
