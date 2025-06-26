using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class OpenAIProviderTests
{
    [Fact]
    public async Task SendChatAsync_NoApiKey_ThrowsInvalidOperationException()
    {
        var originalKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

        var keyFile = Path.Combine(AppContext.BaseDirectory, "openai_api_key.txt");
        if (File.Exists(keyFile))
            File.Delete(keyFile);

        try
        {
            var provider = new OpenAIProvider();
            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SendChatAsync("hi", CancellationToken.None));
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalKey);
        }
    }
}
