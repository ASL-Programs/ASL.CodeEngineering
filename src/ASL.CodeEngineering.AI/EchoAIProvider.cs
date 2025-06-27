namespace ASL.CodeEngineering.AI;

public class EchoAIProvider : IAIProvider
{
    public string Name => "Echo";
    public bool RequiresNetwork => false;

    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Echo: {prompt}");
    }
}
