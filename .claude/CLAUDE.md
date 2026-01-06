# CLAUDE.md – BirthdayReminder

## Known Issues (iOS) – Caching Problems

**Environment:** Windows 11 + USB cable + iPhone (no Mac until ~March 2025)

- [ ] **App Icon**: Home screen still shows ".NET" icon – needs Mac cache clear
- [ ] **Splash Screen**: Still shows ".NET" branding – needs Mac cache clear
- [x] **MainPage Hamburger**: Uses Shell's built-in navbar with Flyout
- [x] **Flyout**: Restored to real Shell Flyout with version in footer

**When Mac is available again:**
- Delete `~/Library/Caches/Xamarin`
- Delete `~/Library/Developer/Xcode/DerivedData`
- Clean & rebuild

---

## Current State (Dec 2024)

- **Navigation**: NavigationPage (no Shell), CustomHeader + FlyoutMenu controls
- **ContactsService**: Returns `List<Contact>`, converts to Person via `ConvertContactToPerson`
- **App.xaml.cs**: Central state with `App.Account`, `App.Persons`, `App.Contacts`
- **StartPage wizard flow**: 1→2→3→4→5/6/7→8→9 (based on UseContacts and ReminderCount)
- **Reminder model**: Method flags (Email, SMS, LockScreen, WhatsApp, Signal), times, and Days
- **Person.Reminder_1/_2/_3**: Replaced individual reminder flags with Reminder objects

---

## Working Directory
`BirthdayReminder/` – Claude Code is started from here. Source code is in `src/`, `.claude/` config is at root.

## Platform
Windows – Use PowerShell or Windows commands. Linux/macOS commands (rm, ls, cat, etc.) do not work.

**Claude Code Bash Tool:** Runs in Git Bash, not CMD. Use `> /dev/null` for null redirection, NOT `> nul` (creates a file named "nul").

---

## Communication Style

- **No suggestions** - Do not offer unsolicited suggestions or next steps
- Just do what is asked, report results briefly
- **User changes have priority** - If user mentions they already made changes (e.g., "I changed line X"), check git diff or read the file FIRST to see what they wrote before implementing anything. Their naming, structure, and decisions take precedence.

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
- **IMPORTANT:** On every commit, increment `ApplicationVersion` in `src/Mobile/Mobile.csproj` by 1
- **IMPORTANT:** Run `dotnet build` before committing to ensure the build succeeds

---

## Versioning

- **ApplicationDisplayVersion** (e.g. `1.0`): Only changes on deploy/release
- **ApplicationVersion** (e.g. `135`): Increment by 1 on every commit
- Display format in app: `Version 1.0 (135)`

---

## XAML / UI Rules

- **All pages**: Always put buttons inside the ScrollView, not in a separate Grid row outside
- **Do NOT use Frame** - it's obsolete in .NET 9. Use Border instead

## Page Initialization

- **Prefer Init() methods** called from the constructor over OnAppearing()
- OnAppearing() only when it makes sense (e.g., data refresh on return, visibility-dependent logic)
- Init() keeps initialization predictable and happens once at construction

---

## Build

- **Do NOT run `dotnet build` unless explicitly requested** - it breaks Hot Reload!
- **Clean command:** `pwsh -File .claude/clean.ps1` – deletes all bin/obj folders and runs dotnet clean

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
├── .claude/                        # Claude Code configuration
├── docs/                           # Documentation
│   └── infrastructure.md           # URLs, subdomains, environments
├── src/                            # Source code
│   ├── BirthdayReminder.sln
│   ├── ApiMobile/                  # ASP.NET Core API for mobile app
│   ├── Common/                     # Shared class library
│   │   ├── Core/
│   │   │   └── CommonConstants.cs  # DOMAIN, API_BASE_URL
│   │   ├── Database/               # EF Core, migrations
│   │   └── DTOs/
│   ├── Mobile/                     # MAUI App (Android & iOS only)
│   │   ├── Core/
│   │   ├── Helper/
│   │   ├── Models/
│   │   ├── Pages/
│   │   ├── Platforms/
│   │   ├── Services/
│   │   └── Resources/
│   ├── MobileLanguages/            # Resource files (resx) for localization
│   ├── WebsiteAdmin/               # ASP.NET Core - admin.birthday-reminder.online
│   └── WebsiteCustomer/            # ASP.NET Core - birthday-reminder.online
└── README.md
```
