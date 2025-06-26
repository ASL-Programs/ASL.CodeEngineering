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

    private static IDictionary<string, Func<T>> LoadPlugins<T>(string? baseDirectory)
    {
        baseDirectory ??= AppContext.BaseDirectory;
        var plugins = new Dictionary<string, Func<T>>();
        var sources = new Dictionary<string, string>();
        var path = Path.Combine(baseDirectory, "plugins");
        if (!Directory.Exists(path))
            return plugins;

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch
            {
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
                        Debug.WriteLine(message);
                        continue;
                    }
                    plugins[name!] = () => (T)Activator.CreateInstance(type)!;
                    sources[name!] = file;
                }
                catch
                {
                    // ignore
                }
            }
        }

        return plugins.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
    }
}
