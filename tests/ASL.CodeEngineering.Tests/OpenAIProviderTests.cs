using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
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

    [Fact]
    public void Constructor_DefaultsToPublicApiUrlWhenEnvEmpty()
    {
        var originalKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var originalUrl = Environment.GetEnvironmentVariable("OPENAI_API_URL");

        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "dummy");
        Environment.SetEnvironmentVariable("OPENAI_API_URL", null);

        try
        {
            var provider = new OpenAIProvider();
            var field = typeof(OpenAIProvider).GetField("_apiUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var url = (string)field!.GetValue(provider)!;
            Assert.Equal("https://api.openai.com/v1/chat/completions", url);
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalKey);
            Environment.SetEnvironmentVariable("OPENAI_API_URL", originalUrl);
        }
    }

    [Fact]
    public void Constructor_UsesCustomApiUrlFromEnvironment()
    {
        var originalKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var originalUrl = Environment.GetEnvironmentVariable("OPENAI_API_URL");

        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "dummy");
        Environment.SetEnvironmentVariable("OPENAI_API_URL", "http://test.com");

        try
        {
            var provider = new OpenAIProvider();
            var field = typeof(OpenAIProvider).GetField("_apiUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var url = (string)field!.GetValue(provider)!;
            Assert.Equal("http://test.com", url);
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalKey);
            Environment.SetEnvironmentVariable("OPENAI_API_URL", originalUrl);
        }
    }
}
