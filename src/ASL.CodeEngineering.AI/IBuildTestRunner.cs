namespace ASL.CodeEngineering.AI;

public interface IBuildTestRunner
{
    string Name { get; }
    Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default);
    Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default);
}
