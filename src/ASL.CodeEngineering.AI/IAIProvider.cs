namespace ASL.CodeEngineering.AI;

public interface IAIProvider
{
    string Name { get; }
    Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default);
}
