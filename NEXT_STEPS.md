# NEXT_STEPS.md

This file captures the current and upcoming steps for the project. It acts as the real-time work log and runbook as described in `AGENTS.md`.

## How to use
- List each planned or in‑progress step in order.
- Mark steps as `[x]` when completed and update the details if plans change.
- Keep this file current at all times.

### Example template
```
# - Set up initial project folders
# - Add AIProvider abstraction
# - Document reference files
```

## Current Session
- [x] Update loaders to detect duplicate plugins/providers and throw descriptive exceptions
- [x] Add unit tests verifying duplicate detection
- [x] Run `dotnet test`
- [x] Log SendChatAsync exceptions to `logs` directory
- [x] Notify user when summary generation fails
- [x] Document test environment requirements in README
- [x] Add example READMEs in `ai_providers/` and `plugins/` describing how to build and drop DLLs
- [x] Mark completed roadmap items in `AGENTS.md`

- [x] Log loader failures to `logs` directory
- [x] Add tests for loader logging
- [x] Run `dotnet test`
- [x] Create helper to sanitize provider names for file paths
- [x] Use sanitized names for `data` and `knowledge_base` directories
- [x] Document name sanitization requirement in `ai_providers/README.md`
- [x] Add test verifying directories created for sanitized provider names
- [x] Introduce DATA_DIR, LOGS_DIR and KB_DIR environment variables
- [x] Document variables in README and update REFERENCE_FILES
- [x] Add tests for environment variable output directories
- [x] Run `dotnet test`

- [x] Wrap `File.AppendAllText` operations in `SendButton_Click` with try/catch
- [x] Add test for logging when `DATA_DIR` is read-only
- [x] Run `dotnet test`
- [x] Document workflow files in README
- [x] Run `dotnet test`

- [x] Update PythonBuildTestRunner to respect LOGS_DIR
- [x] Adjust PythonBuildTestRunnerTests for new log directory
- [x] Document new reference in REFERENCE_FILES
- [x] Run `dotnet test`

- [x] Pass `_projectRoot` to runner in RunButton_Click
- [x] Add MainWindowRunnerTests verifying RunButtonClick uses project root
- [x] Run `dotnet test`

- [x] Add MainWindowBuildTestRunnerTests verifying BuildButtonClick and TestButtonClick use project root
- [x] Run `dotnet test`

 - [x] Detect duplicate names in MainWindow and log warnings
 - [x] Document duplicate warning behavior
 - [x] Add unit test for duplicate plugin warning
 - [x] Run `dotnet test`
- [x] Introduce ProcessRunner helper for executing processes and logging
- [x] Refactor DotnetBuildTestRunner and PythonBuildTestRunner to use helper
- [x] Update REFERENCE_FILES with new helper
- [x] Run `dotnet test`

- [x] Create .github/labeler.yml for labeling paths
- [x] Update REFERENCE_FILES with labeler configuration
- [x] Update CodeQL workflow to set build-mode manual
- [x] Add dotnet build step before analysis
- [x] Run `dotnet test`
- [x] Document workflow update in REFERENCE_FILES
- [x] Add MainWindowSummaryLoggingTests verifying summary failure logs
- [x] Run `dotnet test`

- [x] Wrap file writes in `MainWindow.LogError` and `ProcessRunner.Log` with try/catch
- [x] On failure, fall back to default directory or ignore
- [x] Add tests for read-only `LOGS_DIR` ensuring no exceptions
- [x] Update `REFERENCE_FILES.md`
- [x] Run `dotnet test`
 - [x] Document offline mode variable in README and AGENTS
 - [x] Add ReverseAIProvider and offline filtering logic
 - [x] Update tests for new interface and offline mode
 - [x] Run `dotnet test`
- [x] Document LOGS_DIR fallback behavior in README

- [x] Verify existence of PolyglotInterop.md, plugin_structure.png and configs/appsettings.json
- [x] Remove missing references from `REFERENCE_FILES.md`
- [x] Run `dotnet test`
- [x] Add `data/` and `knowledge_base/` to .gitignore
- [x] Update `REFERENCE_FILES.md` with `.gitignore`
- [x] Run `dotnet test`
- [x] Append `.github/workflows/label.yml` reference to `REFERENCE_FILES.md`
- [x] Run `dotnet test`

- [x] Move `tests/` bullet from README environment variables to repository structure section
- [x] Run `dotnet test`
- [x] Implement LocalAIProvider and register in MainWindow
- [x] Aggregate summaries into knowledge_base/meta
- [x] Add tests for LocalAI provider and provider list
- [x] Mark roadmap items 1.1 and 1.4 complete and update references
- [x] Run `dotnet test`
- [x] Add `.editorconfig` with C#, Markdown and general formatting rules
- [x] Reference `.editorconfig` in README and REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Document duplicate provider warning in `ai_providers/README.md`
- [x] Document duplicate plugin warning in `plugins/README.md`
- [x] Run `dotnet test`
- [x] Clarify language choices in `LANGUAGE_DECISIONS.md`
- [x] Add `LANGUAGE_DECISIONS.md` to `REFERENCE_FILES.md`
- [x] Update loaders to log and skip duplicate providers/plugins
- [x] Update duplicate loader tests
- [x] Mention LocalAIProvider and Local provider in README
- [x] Run `dotnet test`
- [x] Document aggregator location in README
- [x] Add `knowledge_base/meta/summaries.jsonl` to `REFERENCE_FILES.md`
- [x] Run `dotnet test`
- [x] Add InteropGenerator module to scaffold wrappers
- [x] Write InteropGeneratorTests verifying Python wrapper builds
- [x] Document wrapper usage in README
- [x] Mark roadmap item 2.4 complete and update REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add OPENAI_API_URL environment variable to OpenAIProvider
- [x] Default to https://api.openai.com/v1/chat/completions when URL is empty
- [x] Document OPENAI_API_URL in README
- [x] Run dotnet test
- [x] Extend InteropGenerator with Node wrapper
- [x] Add InteropGenerator Node wrapper tests
- [x] Document Node requirement in README
- [x] Update LANGUAGE_DECISIONS with Node.js section
- [x] Run `dotnet test`
- [x] Add Build/Test runner plugin example to plugins README
- [x] Run `dotnet test`
- [x] Implement FeatureLanguageAnalyzer for recommending languages
- [x] Add BenchmarkHarness storing results under knowledge_base/benchmarks
- [x] Write unit tests for analyzer and harness
- [x] Update README and reference files
- [x] Mark roadmap items 2.2 and 2.5 complete
- [x] Run `dotnet test`
- [x] Implement Full Autonomous Learning Mode with logging under knowledge_base/auto
- [x] Implement Task-Focused Conditional Learning Mode and UI controls
- [x] Update AGENTS.md section 3 and reference files
- [x] Run `dotnet test`
- [ ] Implement ProjectPlanner to generate module plans and run builds
