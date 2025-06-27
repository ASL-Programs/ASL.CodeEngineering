# LANGUAGE_DECISIONS.md

This document explains which programming languages and technologies are used across the project and why. It must be reviewed and updated after major changes as detailed in `AGENTS.md`.

## Language & Technology Decisions

### 1. Main Application & AI Providers: C#
- Reason: The WPF UI and the `IAIProvider` implementations are written in C#.
  This provides deep Windows integration and leverages the rich .NET tooling.

### 2. Glue Code & Build/Test Runners: Python
- Reason: Flexible scripting and excellent interoperability. Python is used
  both for cross-language glue code and to implement build and test runners for
  non‑.NET languages.

### 3. Data Analytics: R
- Reason: Provides strong statistical libraries.

Add additional sections here whenever new languages or tools are adopted or plans change.
