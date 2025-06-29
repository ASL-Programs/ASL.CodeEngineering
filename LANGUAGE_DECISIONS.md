# LANGUAGE_DECISIONS.md

This document explains which programming languages and technologies are used across the project and why. It must be reviewed and updated after major changes as detailed in `AGENTS.md`.

## Language & Technology Decisions

### 1. Main Application & AI Providers: C#
 - Reason: The WPF UI, `IAIProvider` implementations and the SignalR sync server are written in C#.
   This provides deep Windows integration and leverages the rich .NET tooling for networking.

### 2. Glue Code & Build/Test Runners: Python
- Reason: Flexible scripting and excellent interoperability. Python is used
  both for cross-language glue code and to implement build and test runners for
  non‑.NET languages.

### 3. Data Analytics: R
- Reason: Provides strong statistical libraries.

### 4. JavaScript Interop Wrappers: Node.js
- Reason: Node.js is leveraged for lightweight script execution when a task is best served with JavaScript tooling.

### 5. Language Selection & Benchmarking
- The `FeatureLanguageAnalyzer` module inspects feature requests and suggests the best language based on keywords.
- `BenchmarkHarness` builds sample projects with each runner and stores timing results in `knowledge_base/benchmarks/benchmarks.jsonl`.
- Initial runs show simple .NET and Python builds complete in under a second on the test machine.

Add additional sections here whenever new languages or tools are adopted or plans change.

### 6. API Server: ASP.NET Core
- Reason: ASP.NET Core provides a small, cross-platform web server for exposing
  build, test and log endpoints.
