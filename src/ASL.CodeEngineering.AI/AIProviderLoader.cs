using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ASL.CodeEngineering.AI;

public static class AIProviderLoader
{
    /// <summary>
    /// Scans the application's <c>ai_providers</c> directory for assemblies and
    /// returns factories for all discovered <see cref="IAIProvider"/> types.
    /// </summary>
    /// <param name="baseDirectory">Directory to search from, typically <see cref="AppContext.BaseDirectory"/>.</param>
    public static IDictionary<string, Func<IAIProvider>> LoadProviders(string? baseDirectory = null,
        IDictionary<string, string>? versions = null)
    {
        Permissions.Require(Role.Operator);
        baseDirectory ??= AppContext.BaseDirectory;
        var envPath = Environment.GetEnvironmentVariable("AI_PROVIDERS_DIR");

        var providers = new Dictionary<string, Func<IAIProvider>>();
        var sourceFiles = new Dictionary<string, string>();
        var path = string.IsNullOrWhiteSpace(envPath)
            ? Path.Combine(baseDirectory, "ai_providers")
            : envPath;
        if (!Directory.Exists(path))
            return providers;

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch (Exception ex)
            {
                LogError("AIProviderLoader_Load", ex);
                continue; // Skip invalid assemblies
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
                if (!typeof(IAIProvider).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                    continue;
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                try
                {
                    var instance = (IAIProvider)Activator.CreateInstance(type)!;
                    var name = instance.Name;
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    if (providers.ContainsKey(name))
                    {
                        var message = $"Duplicate AI provider name '{name}' found in '{file}'. " +
                                       $"Original provider from '{sourceFiles[name]}'.";
                        LogError("AIProviderLoader_Duplicate", new InvalidOperationException(message));
                        continue;
                    }

                    providers[name] = () => (IAIProvider)Activator.CreateInstance(type)!;
                    sourceFiles[name] = file;
                    versions?[name] = assembly.GetName().Version?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    LogError("AIProviderLoader_Create", ex);
                    // Ignore types that fail to instantiate
                }
            }
        }

        return providers
            .OrderBy(p => p.Key)
            .ToDictionary(p => p.Key, p => p.Value);
    }

    private static void LogError(string context, Exception ex)
    {
        Debug.WriteLine($"{context}: {ex}");
        try
        {
            SecureLogger.Write(context, ex.ToString());
        }
        catch
        {
            // ignore logging failures
        }
    }
}
