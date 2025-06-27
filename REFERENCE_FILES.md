# REFERENCE_FILES.md

This list tracks documents, config files and other resources that may need to be revisited. Update it whenever a new reference becomes important as required by `AGENTS.md`.

| File | Purpose / When to Review |
|------|--------------------------|
| `AGENTS.md` | Agent design, all high-level decision logic |
| `PolyglotInterop.md` | Explains how language interop is achieved |
| `plugin_structure.png` | Visual overview of plugin system |
| `configs/appsettings.json` | Main app configuration; always keep current |
| `ai_providers/README.md` | Quick reference for building custom providers |
| `plugins/README.md` | Quick reference for building plugins |
| `README.md` | Environment variables and duplicate name warnings |
| `src/ASL.CodeEngineering.AI/PathHelpers.cs` | Helper for sanitizing provider names |
| `src/ASL.CodeEngineering.App/MainWindow.xaml.cs` | Paths respect environment directories; duplicate plugin/provider names log warnings |
| `src/ASL.CodeEngineering.AI/PythonBuildTestRunner.cs` | Logs to LOGS_DIR with fallback to executable directory |
| `src/ASL.CodeEngineering.AI/ProcessRunner.cs` | Helper to execute processes and write logs respecting LOGS_DIR |
| `.github/labeler.yml` | Label definitions applied by Labeler workflow |
| `.github/workflows/codeql.yml` | CodeQL analysis workflow |

Add new entries in the table above with a short explanation of why the file might be needed again.
