# Codex.md

## Aim
This document defines the coding style, conventions, and infrastructure setup for the BirthdayReminder solution.

---

## Project Overview

### End User Perspective
- **Customers:** Mobile App only (+ Landing Page at birthday-reminder.online)
- **Admin:** WebsiteAdmin (internal use only)

### Domains (Dev Environment)
| App | URL |
|-----|-----|
| WebsiteAdmin | `dev.birthday-reminder.online` |
| ApiMobile | `api-dev.birthday-reminder.online` |
| Landing Page | `birthday-reminder.online` |

### Domain Pattern per Environment
| Environment | WebsiteAdmin | ApiMobile |
|-------------|--------------|-----------|
| Dev | `dev.DOMAIN` | `api-dev.DOMAIN` |
| Prod | `prod.DOMAIN` | `api-prod.DOMAIN` |

**Note:** Since end users only interact with the Mobile App (and never see these URLs), we use consistent prefixes across all environments for clarity and structure.

---

## IIS Deployment

### Naming Convention
All IIS sites and App Pools follow this pattern:
```
BirthdayReminder-[AppName]-[Environment]
```

### Sites & App Pools
| App | IIS Site Name | App Pool |
|-----|---------------|----------|
| WebsiteAdmin | BirthdayReminder-WebsiteAdmin-Dev | BirthdayReminder-WebsiteAdmin-Dev |
| ApiMobile | BirthdayReminder-ApiMobile-Dev | BirthdayReminder-ApiMobile-Dev |

### Environments
- `Dev` – Development/Test server
- `Prod` – Production

### Rules
- One AppPool per solution/domain
- Always use `DOTNET_ENVIRONMENT`, never `ASPNETCORE_ENVIRONMENT`
- App Pool identity: dedicated service account per environment

---

## Database

### Provider
- **Development:** SQL Server Express (local)
- **Production:** MS SQL Server (Contabo server)

### Connection String Template
```
Server={ServerName};Database=BirthdayReminder;Trusted_Connection=True;TrustServerCertificate=True;
```

---

## File Encoding & Line Endings

**Encoding**
- All files: UTF-8 with BOM
- Reason: Visual Studio and .NET tooling expect BOM for proper encoding detection

**Line Endings**
- All files: CRLF (`\r\n`)
- Configure Git to enforce this:
  ```
  git config core.autocrlf true
  ```
- `.gitattributes` in repo root:
  ```
  * text=auto eol=crlf
  *.cs text eol=crlf
  *.cshtml text eol=crlf
  *.json text eol=crlf
  *.md text eol=crlf
  *.ps1 text eol=crlf
  ```

**EditorConfig (.editorconfig)**
```ini
root = true

[*]
charset = utf-8-bom
end_of_line = crlf
indent_style = space
indent_size = 4
insert_final_newline = true
trim_trailing_whitespace = true
```

---

## General Principles
- Allman style for braces (each `{` and `}` on its own line).
- Explicit code, no hidden abstractions.
- Prefer long, descriptive names (e.g., `PersonService`).
- **Namespaces = Project name only.** Folder names never appear in namespaces.
- **File-scoped namespaces preferred** (C# 10+): `namespace Common;` instead of `namespace Common { }`.
- One constants file per project (e.g., `MobileConstants.cs`).

---

## Namespaces
- **Rule:** The root namespace **must equal the project name** (e.g., project `WebsiteAdmin` ⇒ `namespace WebsiteAdmin`).
- **No folder-based subnamespaces.** Folders are for physical organization only and do not influence namespaces.
- **Class name uniqueness:** Since folders don't create subnamespaces, ensure distinct class names.
- **File-scoped namespace (preferred):** Use `namespace ProjectName;` (without braces) for single-namespace files.

### Examples
- Project `Common` → `namespace Common;`
- Project `WebsiteAdmin` → `namespace WebsiteAdmin;`
- Project `ApiMobile` → `namespace ApiMobile;`
- Project `Mobile` → `namespace Mobile;`

### File-scoped vs. Traditional
```csharp
// Preferred (file-scoped, C# 10+)
namespace Common;

public class MyClass
{
    // No extra indentation!
}
```

**Use file-scoped for all new files.**

---

## Usings
- Always wrapped in `#region Usings`.
- Order: system usings first, then project usings, alphabetical inside groups.

```csharp
#region Usings
using System;
using System.Collections.Generic;
using Common;
using Mobile;
#endregion
```

---

## Regions
Each class file should follow this region order (omit empty ones):
1. `#region Usings`
2. `#region Properties`
3. `#region Private Fields`
4. `#region Constructor`
5. `#region Public Methods`
6. `#region Private Methods`

Rules:
- No blank line directly after `#region` or before `#endregion`.
- Regions group logically related code.

---

## Summaries
- All public and private methods require `///` XML documentation comments.
- Summaries must use:
  - **Aim**: Short description of the method/class.
  - **Params**: List all parameters with type and meaning.
  - **Return**: Specify return type and meaning (omit for constructors and void).

Example:

```csharp
/// <summary>
/// Aim: Send heartbeat to backend.
/// Params: None.
/// Return (bool): True if successful, false otherwise.
/// </summary>
public async Task<bool> SendHeartbeatAsync()
```

---

## Constants & Enums
**Project-specific**
- Each project has a `Core/` folder with:
  - `<ProjectName>Constants.cs`
  - `<ProjectName>Enums.cs`
- Project-specific constants and enums are maintained exclusively in these two files.

**Cross-project**
- For shared constants/enums use:
  - `CommonConstants.cs` (in `Common` project)
  - `CommonEnums.cs` (in `Common` project)
- No duplicates of cross-project values in project files; migrate to `Common*` if needed.

**Conventions**
- **Enum baseline:** Always `NotSet = 0` as first entry.
- **Constants naming:** UPPER_SNAKE_CASE (e.g., `DEFAULT_CULTURE`, `DOMAIN`).

---

## Object Initialization Style
- For local variables of known types, use the style:

```csharp
Dictionary<string, string> replacements = new();
```

- When initializing with attributes, use inline object initializers:

```csharp
Person person = new()
{
    Id = personId,
    FirstName = "John",
    Birthday = new DateTime(1990, 5, 15)
};
```

---

## Control Flow (if / while)
- **Single statement rule:** For a single command, braces `{ }` may be omitted.
  Example:
  ```csharp
  if (user == null) return;
  ```

- **Multi-statement rule:** For two or more commands, braces **must** be used in **Allman style**.
  Example:
  ```csharp
  if (user == null)
  {
      LogError("User missing");
      return;
  }
  ```

---

## Project Structure

```
BirthdayReminder/
├── BirthdayReminder.sln
├── Common/                         # Shared class library
│   ├── Core/
│   │   ├── CommonConstants.cs
│   │   └── CommonEnums.cs
│   ├── DTOs/
│   │   ├── HeartbeatDto.cs
│   │   ├── SupportTicketDto.cs
│   │   └── SubscriptionStatusDto.cs
│   ├── Models/
│   │   └── ErrorModel.cs
│   └── [[Guidelines]]/
│       └── Codex.md
│
├── Mobile/                         # MAUI App (Android & iOS)
│   ├── Core/
│   │   ├── MobileConstants.cs
│   │   └── MobileEnums.cs
│   ├── Pages/
│   ├── Services/
│   └── Platforms/
│
├── MobileLanguages/                # Resource files (resx) for localization
│   ├── Resources.resx              # English (default)
│   ├── Resources.de.resx           # German
│   └── Resources.Designer.cs
│
├── ApiMobile/                      # ASP.NET Core API for Mobile
│   ├── Controllers/
│   ├── Data/
│   ├── Entities/
│   └── Services/
│
├── WebsiteAdmin/                   # ASP.NET Core - Admin Panel
│   ├── Data/
│   ├── Entities/
│   ├── Pages/
│   └── Services/
│
└── WebsiteCustomer/                # ASP.NET Core - Landing Page
    └── Pages/
```

### Dependency Hierarchy
- `Common` → Base (no dependencies)
- `Mobile` → Common, MobileLanguages
- `ApiMobile` → Common
- `WebsiteAdmin` → Common
- `WebsiteCustomer` → (standalone)

---

## Checklist Before Commit
- [ ] File encoding: UTF-8 with BOM.
- [ ] Line endings: CRLF.
- [ ] Usings wrapped in region.
- [ ] Regions ordered correctly, no empty regions.
- [ ] No blank lines after `#region` or before `#endregion`.
- [ ] Summaries with Aim/Params/Return everywhere.
- [ ] **Namespace equals project name (no folder-based namespaces).**
- [ ] **File-scoped namespace (`namespace ProjectName;`) for new files.**
- [ ] **Project-specific** constants in `Core/<ProjectName>Constants.cs`, enums in `Core/<ProjectName>Enums.cs`.
- [ ] **Cross-project** constants/enums only in `CommonConstants.cs` / `CommonEnums.cs` (no duplicates).
