# REFERENCE_FILES.md


This list tracks documents, config files and other resources that may need to be revisited. Update it whenever a new reference becomes important as required by `AGENTS.md`.

| File | Purpose / When to Review |
|------|--------------------------|
| `AGENTS.md` | Agent design, all high-level decision logic |
| `ai_providers/README.md` | Quick reference for building custom providers |
| `plugins/README.md` | Quick reference for building plugins |
| `README.md` | Environment variables and duplicate name warnings |
| `LANGUAGE_DECISIONS.md` | Overview of language choices and rationale |
| `src/ASL.CodeEngineering.AI/PathHelpers.cs` | Helper for sanitizing provider names |
| `src/ASL.CodeEngineering.AI/LocalAIProvider.cs` | Lightweight offline provider used in tests |
| `src/ASL.CodeEngineering.App/MainWindow.xaml.cs` | Paths respect environment directories; duplicate plugin/provider names log warnings; handles log write errors; offline mode filter; writes shared summaries |
| `src/ASL.CodeEngineering.AI/PythonBuildTestRunner.cs` | Logs to LOGS_DIR with fallback to executable directory |
| `src/ASL.CodeEngineering.AI/ProcessRunner.cs` | Helper to execute processes and write logs respecting LOGS_DIR; handles log write errors |
| `src/ASL.CodeEngineering.AI/AIProviderLoader.cs` | Loads AI providers and logs duplicate names |
| `src/ASL.CodeEngineering.AI/PluginLoader.cs` | Loads plugins and logs duplicate names |
| `.github/labeler.yml` | Label definitions applied by Labeler workflow |
| `.github/workflows/codeql.yml` | CodeQL analysis workflow |
| `.github/workflows/label.yml` | Handles PR labeling |
| `tests/ASL.CodeEngineering.Tests/LogWriteErrorTests.cs` | Ensures log writes fall back or ignore when directory is read-only |
| `.gitignore` | Excludes generated data and knowledge base content |
| `.editorconfig` | Formatting rules consumed by Visual Studio and dotnet format |
| `src/ASL.CodeEngineering.AI/Interop/InteropGenerator.cs` | Generates wrapper projects for language interop |
| `knowledge_base/meta/summaries.jsonl` | Aggregated summaries from all providers |
| `src/ASL.CodeEngineering.AI/FeatureLanguageAnalyzer.cs` | Recommends languages for new features |
| `src/ASL.CodeEngineering.AI/BenchmarkHarness.cs` | Builds sample projects and records performance |
| `knowledge_base/benchmarks/benchmarks.jsonl` | Timing results from benchmark harness |

Add new entries in the table above with a short explanation of why the file might be needed again.
