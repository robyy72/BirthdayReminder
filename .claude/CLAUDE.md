# CLAUDE.md – BirthdayReminder

## Working Directory
`BirthdayReminder/` – This file lives here and Claude Code is started from here.

## Platform
Windows – Use PowerShell or Windows commands. Linux/macOS commands (rm, ls, cat, etc.) do not work.

---

**Key Rules:**
- Allman style for braces
- Namespace = Project name (no folder-based namespaces)
- File-scoped namespaces: `namespace Common;`
- Usings wrapped in `#region Usings`
- Summaries with Aim/Params/Return
- UTF-8 with BOM, CRLF line endings

---

## Git Commits

- Do NOT add Co-Authored-By or "Generated with Claude Code" to commit messages
- Do NOT check previous commit messages for style - just write a clear commit message
- Each solution (in `apps/` and `libs/`) has its own git repository
- **IMPORTANT:** Before starting work, run `git pull` in each repo to get latest changes
- Use `git pull` (merge), NOT `git pull --rebase` to preserve history

---

## Permissions (settings.local.json)

The settings file has three areas:
- **allow**: Commands that run without asking (git, dotnet, powershell, etc.)
- **deny**: Commands that are blocked
- **ask**: Commands that require user confirmation (npm, npx, node)

If a command is in `allow`, execute it directly without asking for permission.

---

## Project Structure

```
BirthdayReminder/
├── BirthdayReminder.sln
├── Mobile/                         # MAUI App
│   ├── Core/                       # Core logic
│   │   ├── MobileConstants.cs
│   │   └── MobileEnums.cs
│   ├── Platforms/                  # Platform-specific code
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── MacCatalyst/
│   │   ├── Tizen/
│   │   └── Windows/
│   ├── Resources/                  # Assets, fonts, styles
│   ├── App.xaml / App.xaml.cs
│   ├── AppShell.xaml / AppShell.xaml.cs
│   ├── MainPage.xaml / MainPage.xaml.cs
│   └── MauiProgram.cs
└── MobileLanguages/                # Resource files (resx) for localization
    └── MobileLanguages.csproj
```
