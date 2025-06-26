using System;
using System.Collections.Generic;
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
    public static IDictionary<string, Func<IAIProvider>> LoadProviders(string? baseDirectory = null)
    {
        baseDirectory ??= AppContext.BaseDirectory;
        var providers = new Dictionary<string, Func<IAIProvider>>();
        var path = Path.Combine(baseDirectory, "ai_providers");
        if (!Directory.Exists(path))
            return providers;

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch
            {
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
                    if (!string.IsNullOrWhiteSpace(name) && !providers.ContainsKey(name))
                        providers[name] = () => (IAIProvider)Activator.CreateInstance(type)!;
                }
                catch
                {
                    // Ignore types that fail to instantiate
                }
            }
        }

        return providers;
    }
}
