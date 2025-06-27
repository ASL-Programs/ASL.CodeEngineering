using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

public class OpenAIProvider : IAIProvider
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private static readonly HttpClient HttpClient = new();
    private readonly string? _apiKey;

    public string Name => "OpenAI";
    public bool RequiresNetwork => true;

    public OpenAIProvider()
    {
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                  ReadApiKeyFromFile();
    }

    public async Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            return string.Empty;

        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("OpenAI API key not configured.");

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] { new { role = "user", content = prompt } }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            using var response = await HttpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            return document.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString()!
                           .Trim();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return $"[error: {ex.Message}]";
        }
    }

    private static string? ReadApiKeyFromFile()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "openai_api_key.txt");
        return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
    }
}
