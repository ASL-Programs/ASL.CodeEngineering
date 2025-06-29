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
- [x] Add SyncServer and SyncClient using SignalR
- [x] Start server and client from MainWindow when variables are set
- [x] Add unit test verifying session sharing
- [x] Document new variables in README
- [x] Update REFERENCE_FILES and LANGUAGE_DECISIONS
- [x] Mark roadmap item 8.6 complete in AGENTS.md
- [x] Run `dotnet test`
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
- [x] Implement ProjectPlanner to generate module plans and run builds
- [x] Implement DocumentationCrawler for gathering code snippets
- [x] Generate Markdown summaries from crawler output
- [x] Add DashboardWindow displaying knowledge graphs
- [x] Run `dotnet test`
- [x] Extend planner and dashboard with multi-language support and deeper analytics
- [x] Extend UI with learning progress grid and accept/rollback buttons
- [x] Add persistent autonomous learning switch
- [x] Implement VersionManager storing copies under data/versions
- [x] Periodically analyze benchmarks via MetaAnalyzer
- [x] Mark roadmap items 6.2, 6.3 and 7 complete
- [x] Run `dotnet test`
- [x] Create knowledge_base/packages with guides
- [x] Load packages in AutonomousLearningEngine
- [x] Document packages in README
- [x] Update REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add OfflineLearning module with tensor ops and trainer (implemented in commit 86fbbf0)
- [x] Save model versions under data/models
- [x] Train offline model from AutonomousLearningEngine
- [x] Document offline learning in README and update references
- [x] Run dotnet test

- [x] Show packages in UI with enable/disable checkboxes
- [x] Pass selected packages to AutonomousLearningEngine
- [x] Document package toggles in README
- [x] Run `dotnet test`

- [x] Implement DocsUpdater for automatic documentation
- [x] Archive docs under docs/archive
- [x] Document DocsUpdater in README
- [x] Update REFERENCE_FILES with DocsUpdater entry
- [x] Run `dotnet test`
- [x] Add OfflineLearningTests verifying model save/load, training and archiving
- [x] Update ProjectPlanner.GeneratePlans with recommended language output
- [x] Extend DashboardWindow to show language recommendations and meta insights
- [x] Add unit tests for planner and dashboard updates
- [x] Document enhanced analytics in README and REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add roadmap section 8 with planned work in AGENTS.md
- [x] Run `dotnet test`

- [x] Extend MetaAnalyzer with training metrics tracker
- [x] Feed metrics into AutonomousLearningEngine for adaptive learning
- [x] Add unit tests for metric recording and adaptation
- [x] Run `dotnet test`

- [x] Introduce ProjectGenerator module accepting project descriptions and language choices
- [x] Integrate New Project mode in MainWindow with start/stop controls
- [x] Save generated projects under projects/
- [x] Add unit tests for ProjectGenerator
- [x] Document ProjectGenerator usage in README
- [x] Mark roadmap item 8.4 complete in AGENTS.md
- [x] Update REFERENCE_FILES with ProjectGenerator entry
- [x] Run `dotnet test`

- [x] Create knowledge_base/offline_training with sample dataset
- [x] Update ModelLoader to initialize from dataset when no model file exists
- [x] Load initial model in AutonomousLearningEngine if modelPath is provided
- [x] Document offline_training in README and REFERENCE_FILES
- [x] Mark roadmap item 8.1 complete in AGENTS.md
- [x] Run `dotnet test`

 - [x] Build self-training package initializing local model on first launch
 - [x] Implement meta-learning adjustments
 - [x] Add version preview mode
- [x] Create project generator
- [x] Document WPF build instructions in README
- [x] Run `dotnet test`
- [x] Add BuildProcessTests verifying version archive and runner call
- [x] Run `dotnet test`

- [x] Introduce SecureLogger for AES-encrypted logs
- [x] Replace logging helpers to use SecureLogger
- [x] Document audit trail and update references
- [x] Run `dotnet test`

## Upcoming
- [x] Extend BenchmarkHarness to record CPU and memory usage
- [x] Update DashboardWindow with benchmark metrics
- [x] Update MetaAnalyzer to aggregate CPU and memory
- [x] Add tests for benchmark aggregation and dashboard
- [x] Run `dotnet test`
- [x] Add plugin/provider panel with enable/disable checkboxes and version info in MainWindow.xaml
- [x] Update MainWindow.xaml.cs to refresh lists when toggled
- [x] Document plugin panel usage in plugins/README.md and README.md
- [x] Update REFERENCE_FILES with new MainWindow notes
- [x] Run `dotnet test`
- [x] Implement HealthMonitor service monitoring components
- [x] Persist recovery state under data/health
- [x] Add unit tests for HealthMonitor
- [x] Update REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add UserProfile model and JSON serialization
- [x] Save profiles under data/profiles/
- [x] Update UI with profile dropdown
- [x] Document profiles in README
- [x] Run dotnet test
- [x] Add resource files for multiple languages
- [x] Replace hard-coded strings with resource bindings
- [x] Add language selector in MainWindow
- [x] Document translation files in REFERENCE_FILES
- [x] Run dotnet test

- [x] Implement Permissions module gating actions by role
- [x] Encrypt role assignments in configs/permissions.enc
- [x] Update MainWindow with login role selection
- [x] Add tests for permissions
- [x] Document permissions in README and REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add ASP.NET Core ApiServer project with build/test/log endpoints
- [x] Add unit tests for ApiServer
- [x] Document API usage and API_KEY in README
- [x] Update REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add Dockerfile for building WPF app using Windows container
- [x] Add optional k8s manifests for scaled learning components
- [x] Mention Docker and Kubernetes deployment options in README
- [x] Update REFERENCE_FILES with Dockerfile and k8s manifests
- [x] Run `dotnet test`
- [x] Integrate TensorFlow.NET package for GPU support
- [x] Implement GpuDeviceManager listing GPUs and setting active device
- [x] Allow GPU selection in AutonomousLearningEngine via TRAINING_GPU variable
- [x] Document GPU settings in README and LANGUAGE_DECISIONS
- [x] Add unit tests for GpuDeviceManager
- [x] Update REFERENCE_FILES
- [x] Run `dotnet test`
- [x] Add ComplianceChecker with export and erase commands
- [x] Document API endpoints in README and update references
- [x] Run `dotnet test`
