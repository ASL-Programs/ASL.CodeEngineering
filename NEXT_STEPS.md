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
