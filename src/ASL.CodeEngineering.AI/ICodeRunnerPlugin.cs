namespace ASL.CodeEngineering.AI;

public interface ICodeRunnerPlugin
{
    string Name { get; }
    Task<string> RunAsync(string workingDirectory, CancellationToken cancellationToken = default);
}
