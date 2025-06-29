using Microsoft.AspNetCore.SignalR.Client;

namespace ASL.CodeEngineering;

public class SyncClient : IAsyncDisposable
{
    private readonly string _projectRoot;
    private readonly HubConnection _connection;
    private readonly FileSystemWatcher _agentsWatcher;
    private readonly FileSystemWatcher _stepsWatcher;

    public SyncClient(string url, string projectRoot)
    {
        _projectRoot = projectRoot;
        _connection = new HubConnectionBuilder()
            .WithUrl(url.TrimEnd('/') + "/roadmap")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string>("UpdateFile", (name, content) =>
        {
            string path = Path.Combine(_projectRoot, name);
            File.WriteAllText(path, content);
        });

        _agentsWatcher = new FileSystemWatcher(_projectRoot, "AGENTS.md");
        _stepsWatcher = new FileSystemWatcher(_projectRoot, "NEXT_STEPS.md");
        _agentsWatcher.Changed += LocalChanged;
        _stepsWatcher.Changed += LocalChanged;
    }

    private void LocalChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            string content = File.ReadAllText(e.FullPath);
            _connection.SendAsync("UpdateFile", Path.GetFileName(e.FullPath), content);
        }
        catch { }
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
        _agentsWatcher.EnableRaisingEvents = true;
        _stepsWatcher.EnableRaisingEvents = true;
    }

    public async ValueTask DisposeAsync()
    {
        _agentsWatcher.EnableRaisingEvents = false;
        _stepsWatcher.EnableRaisingEvents = false;
        _agentsWatcher.Dispose();
        _stepsWatcher.Dispose();
        await _connection.DisposeAsync();
    }
}
