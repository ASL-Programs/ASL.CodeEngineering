# ASL.CodeEngineering

This repository contains the early skeleton of **ASL.CodeEngineering**, a polyglot
autonomous code engineering tool. Currently the project includes:

- `ASL.CodeEngineering.App` – WPF application that hosts the main UI.
- `ASL.CodeEngineering.AI` – library defining the `IAIProvider` interface and a
  sample `EchoAIProvider`.

Run `dotnet build ASL.CodeEngineering.sln` to build all projects (requires the .NET SDK).

To launch the WPF application, run `dotnet run --project src/ASL.CodeEngineering.App`.

Initial structure for the autonomous polyglot code engineering system.

