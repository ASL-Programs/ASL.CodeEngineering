# plugins

Assemblies in this directory extend the application. A plugin implements `IAnalyzerPlugin`, `ICodeRunnerPlugin`, or `IBuildTestRunner` from `ASL.CodeEngineering.AI`.

The loader scans this folder on startup and registers every plugin found here.
If a plugin uses the same name as a built-in component or another plugin, the duplicate is ignored and a warning is logged.

Loaded plugins appear under the **Plugins** tab of the main window. Each entry shows
its version string and includes a checkbox to enable or disable the plugin for the
current session.

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

## Build/Test runner example

Create a new class library that references `ASL.CodeEngineering.AI`:

```bash
dotnet new classlib -n MyRunner
cd MyRunner
dotnet add reference ../../src/ASL.CodeEngineering.AI/ASL.CodeEngineering.AI.csproj
```

Implement `IBuildTestRunner`:

```csharp
using ASL.CodeEngineering.AI;

public class MyRunner : IBuildTestRunner
{
    public string Name => "MyRunner";

    public Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        // replace with real build logic
        return Task.FromResult("Build succeeded");
    }

    public Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        // replace with real test logic
        return Task.FromResult("Tests passed");
    }
}
```

Build the project and copy `bin/Debug/<framework>/MyRunner.dll` into this folder. Set `PLUGINS_DIR` if your plugins live elsewhere. If a plugin uses the same `Name` as another plugin or a built-in component, the duplicate is ignored and a warning is logged at startup.
