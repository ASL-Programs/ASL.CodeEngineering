namespace ASL.CodeEngineering.AI;

public interface IAnalyzerPlugin
{
    string Name { get; }
    Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default);
}
