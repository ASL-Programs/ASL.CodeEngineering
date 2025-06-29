using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASL.CodeEngineering;

public class SyncServer : IAsyncDisposable
{
    private readonly IHost _host;

    public SyncServer(int port)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(web =>
            {
                web.UseUrls($"http://localhost:{port}");
                web.ConfigureServices(services => services.AddSignalR());
                web.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<RoadmapHub>("/roadmap");
                    });
                });
            })
            .Build();
    }

    public Task StartAsync() => _host.StartAsync();

    public Task StopAsync() => _host.StopAsync();

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
