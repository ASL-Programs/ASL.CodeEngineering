using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Simple local provider that returns the prompt text reversed. Used for offline mode.
/// </summary>
public class ReverseAIProvider : IAIProvider
{
    public string Name => "Reverse";
    public bool RequiresNetwork => false;

    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        char[] chars = prompt.ToCharArray();
        System.Array.Reverse(chars);
        return Task.FromResult(new string(chars));
    }
}
