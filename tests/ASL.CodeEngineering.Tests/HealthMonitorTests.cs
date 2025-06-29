using System;
using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class HealthMonitorTests : IDisposable
{
    private readonly string _root;

    public HealthMonitorTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_root);
    }

    [Fact]
    public async Task Register_RestartsAfterException()
    {
        int runs = 0;
        HealthMonitor.Register("comp1", async ct =>
        {
            runs++;
            if (runs == 1)
                throw new InvalidOperationException("fail");
            HealthMonitor.Heartbeat("comp1");
            await Task.Delay(20, ct);
        }, _root, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(40));

        await Task.Delay(150);
        HealthMonitor.StopAll();

        Assert.True(runs >= 2);
    }

    [Fact]
    public async Task Register_RestartsAfterStall()
    {
        int runs = 0;
        HealthMonitor.Register("comp2", async ct =>
        {
            runs++;
            if (runs == 1)
            {
                await Task.Delay(100, ct);
            }
            else
            {
                HealthMonitor.Heartbeat("comp2");
                await Task.Delay(20, ct);
            }
        }, _root, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(40));

        await Task.Delay(200);
        HealthMonitor.StopAll();

        Assert.True(runs >= 2);
    }

    [Fact]
    public async Task Register_WritesStateFile()
    {
        HealthMonitor.Register("comp3", async ct =>
        {
            HealthMonitor.Heartbeat("comp3");
            await Task.Delay(20, ct);
        }, _root, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(40));

        await Task.Delay(50);
        HealthMonitor.StopAll();

        string stateFile = Path.Combine(_root, "data", "health", "comp3.txt");
        Assert.True(File.Exists(stateFile));
    }

    public void Dispose()
    {
        HealthMonitor.StopAll();
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
