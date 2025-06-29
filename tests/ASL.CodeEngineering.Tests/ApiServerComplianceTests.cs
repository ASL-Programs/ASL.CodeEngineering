using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using ASL.CodeEngineering.Api;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ApiServerComplianceTests : IAsyncLifetime
{
    private ApiServer? _server;
    private HttpClient? _client;
    private readonly string _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    private class StubRunner : IBuildTestRunner
    {
        public string Name => "Stub";
        public Task<string> BuildAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult("built");
        public Task<string> TestAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult("tested");
    }

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        Directory.CreateDirectory(Path.Combine(_root, "data", "profiles"));
        _server = new ApiServer(6302, _root, new StubRunner());
        await _server.StartAsync();
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:6302") };
    }

    public async Task DisposeAsync()
    {
        await _server!.DisposeAsync();
        _client!.Dispose();
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }

    [Fact]
    public async Task ComplianceEndpoint_ReturnsJson()
    {
        var resp = await _client!.GetAsync("/compliance");
        string json = await resp.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.GetProperty("IsCompliant").GetBoolean());
    }

    [Fact]
    public async Task ExportAndEraseEndpoints_Work()
    {
        File.WriteAllText(Path.Combine(_root, "data", "profiles", "a.json"), "{}");
        var resp = await _client!.PostAsync("/export-data", null);
        Assert.True(resp.Content.Headers.ContentType?.MediaType == "application/octet-stream" || resp.IsSuccessStatusCode);
        var erase = await _client.PostAsync("/erase-data", null);
        Assert.True(erase.IsSuccessStatusCode);
        Assert.False(File.Exists(Path.Combine(_root, "data", "profiles", "a.json")));
    }
}
