namespace ASL.CodeEngineering.AI;

public class OpenAIProvider : IAIProvider
{
    public string Name => "OpenAI";

    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // Placeholder: Implementation would call OpenAI API
        return Task.FromResult("[OpenAI response placeholder]");
    }
}
