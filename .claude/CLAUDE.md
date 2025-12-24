# CLAUDE.md – BirthdayReminder

## Current State (Dec 2024)

- **Settings → Account**: Renamed model, service, page
- **StartPage wizard flow**: 1→2→3→4→5/6/7→8 (based on UseContacts and ReminderCount)
- **Reminder model**: Method flags (Email, SMS, LockScreen, WhatsApp, Signal), times, and Days
- **Person.Reminder_1/_2/_3**: Replaced individual reminder flags with Reminder objects
- **App.xaml.cs**: Central state with `App.Account`, `App.Persons`, `App.Reminder_X_Template`
- **Code may not compile** - some pages may still reference old properties

---

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
- No blank line after `#region` and before `#endregion`
- Return values: Assign to variable first, then return (easier debugging)
- Summaries with Aim/Params/Return
- UTF-8 with BOM, CRLF line endings

---

## Git Commits

- Do NOT add Co-Authored-By or "Generated with Claude Code" to commit messages
- Do NOT check previous commit messages for style - just write a clear commit message
- Each solution (in `apps/` and `libs/`) has its own git repository
- **IMPORTANT:** Before starting work, run `git pull` in each repo to get latest changes
- Use `git pull` (merge), NOT `git pull --rebase` to preserve history
- **Commit only, don't push too often** - user will request push when needed
- **Do NOT suggest "push" after commits** - user will push when ready
- **IMPORTANT:** On every commit, increment `ApplicationVersion` in `Mobile/Mobile.csproj` by 1

---

## Versioning

- **ApplicationDisplayVersion** (e.g. `1.0`): Only changes on deploy/release
- **ApplicationVersion** (e.g. `135`): Increment by 1 on every commit
- Display format in app: `Version 1.0 (135)`

---

## XAML / UI Rules

- **All pages**: Always put buttons inside the ScrollView, not in a separate Grid row outside

---

## Build

- **Do NOT run `dotnet build` unless explicitly requested** - it breaks Hot Reload!

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
├── Common/                         # Shared class library
│   ├── Core/
│   │   └── CommonConstants.cs      # DOMAIN, API_BASE_URL
│   └── Models/
│       └── ErrorModel.cs           # Error logging model
├── Mobile/                         # MAUI App (Android & iOS only)
│   ├── Core/
│   │   ├── MobileConstants.cs
│   │   └── MobileEnums.cs
│   ├── Helpers/
│   │   └── PrefsHelper.cs
│   ├── Pages/
│   │   ├── MainPage.xaml(.cs)
│   │   ├── AllBirthdaysPage.xaml(.cs)
│   │   ├── SettingsAllPage.xaml(.cs)
│   │   ├── SettingsReminderPage.xaml(.cs)
│   │   ├── PrivacyPage.xaml(.cs)
│   │   ├── Welcome_1Page.xaml(.cs)
│   │   └── Welcome_2Page.xaml(.cs)
│   ├── Services/
│   │   ├── ApiService.cs           # HTTP client with <T> support
│   │   ├── BirthdayService.cs
│   │   ├── ErrorService.cs         # Online: Sentry, Offline: prefs
│   │   ├── MobileService.cs        # Network access detection
│   │   └── SettingsService.cs
│   ├── Platforms/
│   │   ├── Android/
│   │   └── iOS/
│   ├── Resources/
│   ├── App.xaml / App.xaml.cs
│   ├── AppShell.xaml / AppShell.xaml.cs
│   └── MauiProgram.cs
├── MobileLanguages/                # Resource files (resx) for localization
│   ├── Resources.resx              # English (default)
│   ├── Resources.de.resx           # German
│   └── Resources.Designer.cs
├── WebsiteAdmin/                   # ASP.NET Core - admin.birthday-reminder.online
│   ├── Core/
│   │   └── WebsiteAdminConstants.cs
│   ├── Services/
│   │   └── ApiService.cs
│   └── Pages/
└── WebsiteCustomer/                # ASP.NET Core - birthday-reminder.online (landing page)
    └── Core/
        └── WebsiteCustomerConstants.cs
```
