using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class SessionSharingTests : IAsyncLifetime
{
    private SyncServer? _server;
    private SyncClient? _client1;
    private SyncClient? _client2;
    private string _dir1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private string _dir2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(_dir1);
        Directory.CreateDirectory(_dir2);
        File.WriteAllText(Path.Combine(_dir1, "AGENTS.md"), "a1");
        File.WriteAllText(Path.Combine(_dir1, "NEXT_STEPS.md"), "s1");
        File.WriteAllText(Path.Combine(_dir2, "AGENTS.md"), "a2");
        File.WriteAllText(Path.Combine(_dir2, "NEXT_STEPS.md"), "s2");

        _server = new SyncServer(6200);
        await _server.StartAsync();

        _client1 = new SyncClient("http://localhost:6200", _dir1);
        _client2 = new SyncClient("http://localhost:6200", _dir2);
        await _client1.StartAsync();
        await _client2.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _client1!.DisposeAsync();
        await _client2!.DisposeAsync();
        await _server!.DisposeAsync();
        if (Directory.Exists(_dir1)) Directory.Delete(_dir1, true);
        if (Directory.Exists(_dir2)) Directory.Delete(_dir2, true);
    }

    [Fact]
    public async Task UpdateFile_BroadcastsToOtherClient()
    {
        var path = Path.Combine(_dir1, "AGENTS.md");
        File.WriteAllText(path, "new");
        await Task.Delay(200);
        string content = File.ReadAllText(Path.Combine(_dir2, "AGENTS.md"));
        Assert.Equal("new", content);
    }
}
