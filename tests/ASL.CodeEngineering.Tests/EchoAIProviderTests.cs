using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class EchoAIProviderTests
{
    [Theory]
    [InlineData("hello")]
    [InlineData("test prompt")] 
    [InlineData("")]
    public async Task SendChatAsync_EchoesPrompt(string prompt)
    {
        var provider = new EchoAIProvider();
        string result = await provider.SendChatAsync(prompt, CancellationToken.None);
        Assert.Equal($"Echo: {prompt}", result);
    }
}
