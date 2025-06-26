# ai_providers

This directory contains additional AI provider assemblies. Each provider implements `IAIProvider` from the `ASL.CodeEngineering.AI` project.

The application scans this folder at startup. Any `*.dll` dropped here becomes available in the provider list automatically.

## Minimal example

Create a new class library that references `ASL.CodeEngineering.AI`:

```bash
dotnet new classlib -n MyProvider
cd MyProvider
dotnet add reference ../../src/ASL.CodeEngineering.AI/ASL.CodeEngineering.AI.csproj
```

Implement `IAIProvider`:

```csharp
using ASL.CodeEngineering.AI;

public class MyProvider : IAIProvider
{
    public string Name => "MyProvider";

    public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // replace with real logic
        return Task.FromResult($"echo {prompt}");
    }
}
```

Build the project:

```bash
dotnet build
```

Copy `bin/Debug/<framework>/MyProvider.dll` into this `ai_providers` folder (or set the `AI_PROVIDERS_DIR` environment variable to point elsewhere).
