namespace ASL.CodeEngineering.AI;

public interface IAIProvider
{
    string Name { get; }
    /// <summary>
    /// Indicates whether the provider communicates with external services over
    /// the network. Providers returning <c>true</c> can be disabled when the
    /// application runs in offline mode.
    /// </summary>
    bool RequiresNetwork { get; }
    Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default);
}
