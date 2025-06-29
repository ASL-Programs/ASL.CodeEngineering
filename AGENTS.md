# AGENTS.md - ASL.CodeEngineering  
### Autonomous Polyglot AI Software Engineer for Windows (.exe)

---

## PROJECT EXECUTION RULES – CRITICAL (MUST READ & APPLY)

### 1. Completed Tasks MUST Always Be Checked Off

- **Every task or subtask that is completed MUST be checked in this AGENTS.md file. No exceptions!**
    - The `[ ]` MUST be replaced with `[x]` immediately after completion.
    - The system MUST NOT proceed if AGENTS.md is not up-to-date.

---

### 2. Step Tracking: Always Write the Next Steps to a Separate File

- **For every session, the AI (or user) MUST write out the next set of intended steps into a dedicated file: `NEXT_STEPS.md`.**
    - Every step, decision, or change in strategy MUST be reflected here before execution.
    - All progress must be logged *at each stage* (not only at the end).
    - This file serves as the real-time “work log” and “runbook” for what’s being done next.

---

### 3. Reference Files List: Always Maintain and Update

- **If there are any files (e.g., reference docs, architectural decisions, code snippets, design diagrams, config files, knowledge base notes, etc.) that may need to be re-visited, re-used, or re-examined:**
    - The AI (or user) MUST record the filename and its purpose in `REFERENCE_FILES.md`.
    - This list MUST be kept up-to-date, with a short note for *why* each file might be needed again.
    - Example entry:  
        ```
        | File                  | Purpose/When to Review                        |
        |-----------------------|-----------------------------------------------|
        | AGENTS.md              | Agent design, all high-level decision logic   |
        | PolyglotInterop.md    | Explains how language interop is achieved     |
        | plugin_structure.png  | Visual of plugin system; consult before new   |
        | configs/appsettings.json | Main app config, always keep current        |
        ```

---

### 4. Language/Technology Rationale: Maintain Full Polyglot Transparency

- **A dedicated file `LANGUAGE_DECISIONS.md` MUST be kept and updated, explaining:**
    - Which programming languages are used, for which modules.
    - *Why* each language/tech was chosen (performance, ecosystem, AI support, libraries, platform integration, etc.).
    - This file must be re-examined and updated after *every* major change or after reading `AGENTS.md`.
    - Example outline for the file:
        ```
        ## Language & Technology Decisions

        ### 1. Core UI (WPF): C#
        - Reason: Native Windows integration, best for .exe.

        ### 2. Data Analytics: R
        - Reason: Superior statistical libraries.

        ### 3. Glue Code: Python
        - Reason: Best interop and scripting flexibility.

        (etc.)
        ```
    - *After* reading `AGENTS.md`, the AI (or user) must review and, if needed, revise this file to match the best possible language/tool choices for each subtask.
    - **Every coding or planning session must review both `AGENTS.md` and `LANGUAGE_DECISIONS.md` before proceeding.**

---

### 5. Cross-Reference and Revisit Logic

- **Whenever starting a new task, returning to a project, or troubleshooting, the AI (or user) MUST consult:**
    - `AGENTS.md` (for current progress and what’s next)
    - `NEXT_STEPS.md` (for the precise step-by-step runbook)
    - `REFERENCE_FILES.md` (for what documents, configs, or artifacts might be needed)
    - `LANGUAGE_DECISIONS.md` (for language/tech choices and their justifications)
    - `AGENTS.md` (for top-level agents rules and architecture)
- *Failure to do this review is considered a process violation and MUST be fixed before continuing.*

---

## USAGE SUMMARY

- **Never skip these rule checks!**  
  This ensures all progress, decisions, references, and language choices are transparent, auditable, and always up-to-date.  
  This rule set must appear at the very start of AGENTS.md and must be enforced by any AI or human contributor.

---

## Core Mission

> **ASL.CodeEngineering must be developed as a truly polyglot, self-improving autonomous software engineer for Windows:**
> - All components must be written in the best-suited programming language and technology for their function (C#, Python, C++, Java, R, JavaScript, Bash, etc.).
> - The application itself is a portable, local-first Windows executable (WPF-based .exe), with all data, logs, configs, models, and code stored ONLY in the project root directory.
> - The system continuously learns, self-upgrades, and applies autonomous improvements—both to itself and to any user-given project or roadmap (AGENTS.md).

---

## 0. Project Setup & Core Principles

- [x] 0.1. Initialize a WPF project for ASL.CodeEngineering (.exe, portable, local-only)
- [x] 0.2. Structure all data, logs, configs, plugins, and code under the project root (never use AppData or cloud)
- [x] 0.3. Set up modular, extensible architecture (plugins/extensions for AI, analyzers, code runners, etc.)
- [x] 0.4. Create initial folders: `/data`, `/logs`, `/plugins`, `/ai_providers`, `/knowledge_base`, `/code`
- [x] 0.5. Maintain absolute data sovereignty: nothing leaves the project directory

---

## 1. Multi-AI & Local AI Integration

 - [x] 1.1. Integrate multiple AI providers: OpenAI, Claude, Gemini, and LocalAI (Llama, Ollama, GPT4All, etc.)
- [x] 1.2. Use modular AIProvider abstraction for easy future integrations
- [x] 1.3. Archive every AI's outputs, code, logs, chat, and learning under `/data/{aiName}/`
 - [x] 1.4. Maintain a unified meta-knowledge base for cross-AI comparison, benchmarking, and insights
- [x] 1.5. All data, models, and learning are portable and stored locally

---

## 2. Hyper-Advanced Polyglot Code Editor & Build/Test

 - [x] 2.1. Integrate an advanced code editor (AvalonEdit or similar):
     - Support for multi-language syntax, snippets, navigation, and real-time linting
     - Project explorer: user can select or assign any folder as a project (MCP)
 - [x] 2.2. For every module or feature, **AI must select and implement the best language/technology** (C#, Python, C++, Java, R, JS, Bash, etc.)
 - [x] 2.3. Implement automated build/test runners for each supported language (dotnet, pip/pytest, gcc/clang, javac, Rscript, npm, etc.)
- [x] 2.4. Automatically generate and manage glue code/wrappers for interop (e.g., Python↔C#, C++↔.NET, etc.)
 - [x] 2.5. Benchmark, compare, and meta-learn which languages/tools deliver the best results for each use-case

---

## 3. Dual-Mode Learning & Self-Improvement

- [x] 3.1. **Full Autonomous Learning Mode**
    - On “Start Learning”, the agent continuously explores, learns, and self-upgrades using online sources, AI chats, open-source mining, and self-analysis
    - Seeks to produce and propose better versions of itself—algorithms, workflows, refactorings, and documentation
- [x] 3.2. **Task-Focused Conditional Learning Mode**
    - On “Pause” (or when autonomous learning is disabled), focus solely on user-assigned tasks (e.g., provided AGENTS.md, code review, project improvements)
    - While working, all actions (build, test, fix, optimize) are logged as new learning episodes
    - **If a problem/error cannot be solved using the local knowledge base, the system automatically resumes online research and AI queries—once solved, returns to focused task mode**
    - Both learning layers (autonomous, task-focused) are logged and analyzed for future improvement

---

## 4. Autonomous Project Engineering & Refactoring

- [x] 4.1. Ingest and understand any roadmap (AGENTS.md), project folder, or legacy codebase (multi-language OK)
- [x] 4.2. Auto-plan project execution: break down tasks, select optimal language/tech per task, schedule build/test/integration
- [x] 4.3. For each module:
    - Generate code in the best-suited language
    - Integrate modules, generate required glue code
    - Auto-build/test, self-fix errors/warnings, repeat until successful
- [x] 4.4. For existing projects: analyze code, find bugs/smells/anti-patterns, propose or auto-apply improvements/refactors (multi-language OK)
- [x] 4.5. Every change, fix, or upgrade is versioned and user-reviewable

---

## 5. Automated Knowledge Mining & Reporting

- [x] 5.1. Crawl open source, docs, StackOverflow, HN, Reddit, arxiv, etc. for all supported languages/technologies
- [x] 5.2. Continuously update both per-AI and unified knowledge bases
- [x] 5.3. Generate self-documenting code (docstrings/XMLDocs/Markdown), project milestone reports, and meta-learning summaries
- [x] 5.4. Visual dashboard: learning progress, knowledge graphs, AI/language/tool comparisons, and improvement trends

- [x] 5.5. Automatically update AGENTS.md and NEXT_STEPS.md after each learning cycle, archiving backups under docs/archive/
---

## 6. User Interaction & Control

 - [x] 6.1. User can select/assign any folder as the project root (MCP), provide AGENTS.md or custom tasks
 - [x] 6.2. Show all learning, progress, and self-upgrade suggestions; user can accept/rollback any changes
 - [x] 6.3. Option to pause/resume autonomous learning at any time
 - [x] 6.4. All code, data, and models stay local in the project directory
 - [x] 6.5. Setting `DISABLE_NETWORK_PROVIDERS` removes online providers so
       prompts never leave the project root

---

## 7. Continuous Self-Upgrade & Meta-Learning

 - [x] 7.1. At every learning cycle, the system must seek to create and propose an improved version of itself—algorithms, code, workflows, knowledge structures, and tools
 - [x] 7.2. Archive all previous versions/experiments for user review/comparison
 - [x] 7.3. Meta-analyze which languages, tools, AIs, or strategies produce the best results, and update future learning accordingly

## 8. Planned Work

 - [x] 8.1. Integrate a dedicated self-training package that initializes a local model on first launch
 - [x] 8.2. Expand meta-learning (analysis of training outcomes, adjustment of packages, etc.)
 - [x] 8.3. Support version preview and rollback features
 - [x] 8.4. Document ability to scaffold new software projects
 - [x] 8.5. Introduce SecureLogger with AES-encrypted logs

---

## Short Mission Statement

> ASL.CodeEngineering is an autonomous, self-upgrading, polyglot AI code engineer:  
> - It develops and improves both itself and any project you give it, using the best tools, languages, and strategies—without limitations.
> - All work is fully local, portable, and privacy-first as a Windows .exe.
> - **Every function, module, and helper must be written in the optimal language and technology for its purpose—no language boundaries.**
- [x] Build process archives new version for preview
- [x] Preview Update button runs sandbox build
- [x] Restoring preview on confirmation
- [x] Unit tests verify BuildProcess archives version and invokes runner
