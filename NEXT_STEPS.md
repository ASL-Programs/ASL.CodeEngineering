# NEXT_STEPS.md

This file captures the current and upcoming steps for the project. It acts as the real-time work log and runbook as described in `AGENTS.md`.

## How to use
- List each planned or in‑progress step in order.
- Mark steps as `[x]` when completed and update the details if plans change.
- Keep this file current at all times.

### Example template
```
- [ ] Set up initial project folders
- [ ] Add AIProvider abstraction
- [ ] Document reference files
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
