using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ReverseAIProviderTests
{
    [Theory]
    [InlineData("hello", "olleh")]
    [InlineData("ASL", "LSA")]
    [InlineData("", "")]
    public async Task SendChatAsync_ReversesPrompt(string prompt, string expected)
    {
        var provider = new ReverseAIProvider();
        string result = await provider.SendChatAsync(prompt, CancellationToken.None);
        Assert.Equal(expected, result);
    }
}
