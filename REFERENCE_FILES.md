# REFERENCE_FILES.md

This list tracks documents, config files and other resources that may need to be revisited. Update it whenever a new reference becomes important as required by `AGENTS.md`.

| File | Purpose / When to Review |
|------|--------------------------|
| `Agent.md` | Agent design, all high-level decision logic |
| `PolyglotInterop.md` | Explains how language interop is achieved |
| `plugin_structure.png` | Visual overview of plugin system |
| `configs/appsettings.json` | Main app configuration; always keep current |
| `ai_providers/README.md` | Quick reference for building custom providers |
| `plugins/README.md` | Quick reference for building plugins |
| `README.md` | Documented environment variables |
| `src/ASL.CodeEngineering.AI/PathHelpers.cs` | Helper for sanitizing provider names |
| `src/ASL.CodeEngineering.App/MainWindow.xaml.cs` | Paths respect DATA_DIR, KB_DIR and LOGS_DIR |

Add new entries in the table above with a short explanation of why the file might be needed again.
