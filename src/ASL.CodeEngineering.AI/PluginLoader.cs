using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ASL.CodeEngineering.AI;

public static class PluginLoader
{
    public static IDictionary<string, Func<IAnalyzerPlugin>> LoadAnalyzers(string? baseDirectory = null)
        => LoadPlugins<IAnalyzerPlugin>(baseDirectory);

    public static IDictionary<string, Func<ICodeRunnerPlugin>> LoadRunners(string? baseDirectory = null)
        => LoadPlugins<ICodeRunnerPlugin>(baseDirectory);

    public static IDictionary<string, Func<IBuildTestRunner>> LoadBuildTestRunners(string? baseDirectory = null)
        => LoadPlugins<IBuildTestRunner>(baseDirectory);

    private static IDictionary<string, Func<T>> LoadPlugins<T>(string? baseDirectory)
    {
        baseDirectory ??= AppContext.BaseDirectory;
        var envPath = Environment.GetEnvironmentVariable("PLUGINS_DIR");

        var plugins = new Dictionary<string, Func<T>>();
        var sources = new Dictionary<string, string>();
        var path = string.IsNullOrWhiteSpace(envPath)
            ? Path.Combine(baseDirectory, "plugins")
            : envPath;
        if (!Directory.Exists(path))
            return plugins;

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch (Exception ex)
            {
                LogError("PluginLoader_Load", ex);
                continue;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            foreach (var type in types)
            {
                if (type is null)
                    continue;
                if (!typeof(T).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                    continue;
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                try
                {
                    var instance = (T)Activator.CreateInstance(type)!;
                    var nameProp = typeof(T).GetProperty("Name");
                    var name = (string?)nameProp?.GetValue(instance);
                    if (string.IsNullOrWhiteSpace(name))
                        continue;
                    if (plugins.ContainsKey(name!))
                    {
                        var message = $"Duplicate plugin name '{name}' found in '{file}'. Original from '{sources[name!]}'";
                        throw new InvalidOperationException(message);
                    }
                    plugins[name!] = () => (T)Activator.CreateInstance(type)!;
                    sources[name!] = file;
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    LogError("PluginLoader_Create", ex);
                    // ignore
                }
            }
        }

        return plugins.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
    }

    private static void LogError(string context, Exception ex)
    {
        Debug.WriteLine($"{context}: {ex}");
        try
        {
            var logsDir = Environment.GetEnvironmentVariable("LOGS_DIR") ??
                          Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logsDir);
            var file = Path.Combine(logsDir, $"{context}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
            File.WriteAllText(file, ex.ToString());
        }
        catch
        {
            // ignore logging failures
        }
    }
}
