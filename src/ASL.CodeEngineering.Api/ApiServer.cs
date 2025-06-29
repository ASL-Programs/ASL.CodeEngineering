using ASL.CodeEngineering.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.Api;

public class ApiServer : IAsyncDisposable
{
    private readonly IHost _host;

    public ApiServer(int port, string projectRoot, IBuildTestRunner runner)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(web =>
            {
                web.UseUrls($"http://localhost:{port}");
                web.ConfigureServices(services =>
                {
                    services.AddSingleton(runner);
                });
                web.Configure(app =>
                {
                    app.UseMiddleware<ApiKeyMiddleware>();
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapPost("/build", async context =>
                        {
                            var r = context.RequestServices.GetRequiredService<IBuildTestRunner>();
                            string path = await BuildProcess.BuildNewVersionAsync(projectRoot, r);
                            await context.Response.WriteAsync(path);
                        });

                        endpoints.MapPost("/test", async context =>
                        {
                            var r = context.RequestServices.GetRequiredService<IBuildTestRunner>();
                            string result = await r.TestAsync(projectRoot);
                            await context.Response.WriteAsync(result);
                        });

                        endpoints.MapGet("/logs", async context =>
                        {
                            string dir = Environment.GetEnvironmentVariable("LOGS_DIR") ??
                                         Path.Combine(AppContext.BaseDirectory, "logs");
                            var files = Directory.Exists(dir)
                                ? Directory.GetFiles(dir).Select(Path.GetFileName)
                                : Array.Empty<string>();
                            await context.Response.WriteAsJsonAsync(files);
                        });

                        endpoints.MapGet("/logs/{name}", async context =>
                        {
                            string dir = Environment.GetEnvironmentVariable("LOGS_DIR") ??
                                         Path.Combine(AppContext.BaseDirectory, "logs");
                            string name = (string)context.Request.RouteValues["name"]!;
                            string file = Path.Combine(dir, name);
                            if (!File.Exists(file))
                            {
                                context.Response.StatusCode = 404;
                                return;
                            }
                            await context.Response.SendFileAsync(file);
                        });
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

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? key = Environment.GetEnvironmentVariable("API_KEY");
        if (!string.IsNullOrWhiteSpace(key))
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var provided) || provided != key)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        await _next(context);
    }
}
