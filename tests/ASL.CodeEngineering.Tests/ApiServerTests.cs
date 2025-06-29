using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using ASL.CodeEngineering.Api;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ApiServerTests : IAsyncLifetime
{
    private ApiServer? _server;
    private HttpClient? _client;
    private readonly string _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly StubRunner _runner = new();

    private class StubRunner : IBuildTestRunner
    {
        public string Name => "Stub";
        public Task<string> BuildAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult("built");
        public Task<string> TestAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult("tested");
    }

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        _server = new ApiServer(6301, _root, _runner);
        await _server.StartAsync();
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:6301") };
    }

    public async Task DisposeAsync()
    {
        await _server!.DisposeAsync();
        _client!.Dispose();
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
        Environment.SetEnvironmentVariable("API_KEY", null);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
    }

    [Fact]
    public async Task Build_ReturnsOutput()
    {
        var resp = await _client!.PostAsync("/build", null);
        var text = await resp.Content.ReadAsStringAsync();
        Assert.Equal("built", text);
    }

    [Fact]
    public async Task Logs_RequiresKeyAndReturnsFile()
    {
        string logsDir = Path.Combine(_root, "logs");
        Directory.CreateDirectory(logsDir);
        File.WriteAllText(Path.Combine(logsDir, "a.log"), "ok");
        Environment.SetEnvironmentVariable("API_KEY", "k");
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        var req = new HttpRequestMessage(HttpMethod.Get, "/logs");
        req.Headers.Add("X-Api-Key", "k");
        var resp = await _client!.SendAsync(req);
        string json = await resp.Content.ReadAsStringAsync();
        Assert.Contains("a.log", json);

        var req2 = new HttpRequestMessage(HttpMethod.Get, "/logs/a.log");
        req2.Headers.Add("X-Api-Key", "k");
        var resp2 = await _client.SendAsync(req2);
        string content = await resp2.Content.ReadAsStringAsync();
        Assert.Equal("ok", content);
    }
}
