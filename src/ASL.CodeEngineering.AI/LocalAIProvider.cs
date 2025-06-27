using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Simple offline provider that returns the prompt in upper case.
/// Acts as a lightweight local model with no network dependency.
/// </summary>
public class LocalAIProvider : IAIProvider
{
    public string Name => "Local";
    public bool RequiresNetwork => false;

    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        string response = string.IsNullOrWhiteSpace(prompt) ? string.Empty : prompt.ToUpperInvariant();
        return Task.FromResult($"Local: {response}");
    }
}
