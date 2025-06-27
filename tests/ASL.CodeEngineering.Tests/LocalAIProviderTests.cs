using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class LocalAIProviderTests
{
    [Theory]
    [InlineData("hello", "Local: HELLO")]
    [InlineData("", "Local: ")]
    public async Task SendChatAsync_ReturnsUpperCase(string prompt, string expected)
    {
        var provider = new LocalAIProvider();
        string result = await provider.SendChatAsync(prompt, CancellationToken.None);
        Assert.Equal(expected, result);
    }
}
