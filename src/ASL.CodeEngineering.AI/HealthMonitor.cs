using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Periodically monitors registered components for stalls or errors
/// and restarts them automatically. The last known good timestamp
/// is written under <c>data/health/</c> for recovery.
/// </summary>
public static class HealthMonitor
{
    private class Component
    {
        public Func<CancellationToken, Task> Run = default!;
        public CancellationTokenSource Cts = new();
        public Task? Task;
        public TimeSpan CheckInterval;
        public TimeSpan StallTimeout;
        public string Name = string.Empty;
        public string Root = string.Empty;
    }

    private static readonly ConcurrentDictionary<string, Component> _components = new();
    private static readonly ConcurrentDictionary<string, DateTime> _heartbeats = new();

    /// <summary>
    /// Reports activity for the component so the monitor knows it has not stalled.
    /// </summary>
    public static void Heartbeat(string name) => _heartbeats[name] = DateTime.UtcNow;

    /// <summary>
    /// Registers and starts monitoring a component.
    /// </summary>
    public static void Register(string name, Func<CancellationToken, Task> run,
        string projectRoot, TimeSpan checkInterval, TimeSpan stallTimeout)
    {
        var comp = new Component
        {
            Run = run,
            CheckInterval = checkInterval,
            StallTimeout = stallTimeout,
            Name = name,
            Root = projectRoot
        };
        _components[name] = comp;
        _heartbeats[name] = DateTime.UtcNow;
        comp.Task = Task.Run(() => RunLoop(comp));
    }

    /// <summary>
    /// Stops all monitored components.
    /// </summary>
    public static void StopAll()
    {
        foreach (var c in _components.Values)
            c.Cts.Cancel();
        _components.Clear();
    }

    private static async Task RunLoop(Component comp)
    {
        var statePath = GetStatePath(comp.Root, comp.Name);
        while (!comp.Cts.IsCancellationRequested)
        {
            try
            {
                _heartbeats[comp.Name] = DateTime.UtcNow;
                await comp.Run(comp.Cts.Token);
                File.WriteAllText(statePath, DateTime.UtcNow.ToString("O"));
            }
            catch (Exception ex)
            {
                try { SecureLogger.Write($"HealthMonitor_{comp.Name}", ex.ToString()); } catch { }
            }

            if (comp.Cts.IsCancellationRequested)
                break;

            var checkDelay = Task.Delay(comp.CheckInterval, comp.Cts.Token);
            try { await checkDelay; } catch { }

            if (DateTime.UtcNow - _heartbeats[comp.Name] > comp.StallTimeout)
            {
                try { SecureLogger.Write($"HealthMonitor_{comp.Name}", "Restarting after stall"); } catch { }
                comp.Cts.Cancel();
                try { await comp.Task!; } catch { }
                comp.Cts = new CancellationTokenSource();
                comp.Task = Task.Run(() => RunLoop(comp));
                break;
            }
        }
    }

    private static string GetStatePath(string root, string name)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                           Path.Combine(root, "data");
        string dir = Path.Combine(baseData, "health");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"{name}.txt");
    }
}

