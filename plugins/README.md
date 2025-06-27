# plugins

Assemblies in this directory extend the application. A plugin implements `IAnalyzerPlugin`, `ICodeRunnerPlugin`, or `IBuildTestRunner` from `ASL.CodeEngineering.AI`.

The loader scans this folder on startup and registers every plugin found here.
If a plugin uses the same name as a built-in component or another plugin, the duplicate is ignored and a warning is logged.

## Minimal analyzer example

```bash
dotnet new classlib -n HelloAnalyzer
cd HelloAnalyzer
dotnet add reference ../../src/ASL.CodeEngineering.AI/ASL.CodeEngineering.AI.csproj
```

```csharp
using ASL.CodeEngineering.AI;

public class HelloAnalyzer : IAnalyzerPlugin
{
    public string Name => "Hello";

    public Task<string> AnalyzeAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Hello analyzer ran.");
    }
}
```

Build with `dotnet build` and copy `bin/Debug/<framework>/HelloAnalyzer.dll` into this folder. Set `PLUGINS_DIR` to use a custom directory.
