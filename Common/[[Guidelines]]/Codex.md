# Codex.md

## Aim
This document defines the coding style, conventions, and infrastructure setup for all solutions in the TestyFriend organization.

---

## Project Overview

### End User Perspective
- **Customers:** Mobile App only (+ Landing Page, URL TBD)
- **Admin:** WebsiteAdmin (internal use only)

### Domains (Dev Environment)
| App | URL |
|-----|-----|
| WebsiteAdmin | `dev.DOMAIN.tld` |
| Hangfire Dashboard | `dev.DOMAIN.tld/hangfire` |
| ApiMobile | `api-dev.DOMAIN.tld` |

### Domain Pattern per Environment
| Environment | WebsiteAdmin | ApiMobile |
|-------------|--------------|-----------|
| Dev | `dev.DOMAIN.tld` | `api-dev.DOMAIN.tld` |
| Stage | `stage.DOMAIN.tld` | `api-stage.DOMAIN.tld` |
| Prod | `prod.DOMAIN.tld` | `api-prod.DOMAIN.tld` |

**Note:** Typically the "prod" prefix is omitted for production URLs. However, since end users only interact with the Mobile App (and never see these URLs), we use consistent prefixes across all environments for clarity and structure.

---

## IIS Deployment

### Naming Convention
All IIS sites and App Pools follow this pattern:
```
TestyFriend-[AppName]-[Environment]
```

### Sites & App Pools
| App | IIS Site Name | App Pool |
|-----|---------------|----------|
| WebsiteAdmin | TestyFriend-WebsiteAdmin-Dev | TestyFriend-WebsiteAdmin-Dev |
| ApiMobile | TestyFriend-ApiMobile-Dev | TestyFriend-ApiMobile-Dev |
| BackgroundJobsHost | TestyFriend-BackgroundJobsHost-Dev | TestyFriend-BackgroundJobsHost-Dev |

### Environments
- `Dev` – Development/Test server
- `Stage` – Pre-production
- `Prod` – Production

### Rules
- One AppPool per solution/domain
- Always use `DOTNET_ENVIRONMENT`, never `ASPNETCORE_ENVIRONMENT`
- App Pool identity: dedicated service account per environment

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
- Prefer long, descriptive names (e.g., `WeatherProviderClient`).
- **Namespaces = Project name only.** Folder names never appear in namespaces.
- **File-scoped namespaces preferred** (C# 10+): `namespace Common;` instead of `namespace Common { }`.
- One constants file per project (e.g., `MobileConstants.cs`).

---

## Namespaces
- **Rule:** The root namespace **must equal the project name** (e.g., project `WebsiteAdmin` ⇒ `namespace WebsiteAdmin`).
- **No folder-based subnamespaces.** Folders are for physical organization only and do not influence namespaces.
- **Class name uniqueness:** Since folders don't create subnamespaces, ensure distinct class names (e.g., `DatabaseLogger`, `ServiceLogger`).
- **File-scoped namespace (preferred):** Use `namespace ProjectName;` (without braces) for single-namespace files. This saves one indentation level and is the modern C# 10+ style.

### Examples
- Project `Common` → `namespace Common;`
- Project `WebsiteAdmin` → `namespace WebsiteAdmin;`
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

```csharp
// Legacy (traditional, still valid)
namespace Common
{
    public class MyClass
    {
        // Extra indentation level
    }
}
```

**Use file-scoped for all new files.** Only use traditional braces when multiple namespaces are needed in one file (rare).

### Pitfalls & Guidance
- **Mixed legacy namespaces:** Refactor imports/usings when flattening to project-only namespaces.
- **Tooling defaults:** Disable or ignore IDE prompts like "Match namespace to folder".
- **Code generators:** EF Core and other generators may create folder-based namespaces. After generating, **adjust** to project namespace.

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
/// Aim: Returns a cached value or creates a new one using the provided factory.
/// Params:
///   key (string) - The cache key.
///   factory (Func<Task<byte[]?>>) - Function to generate the value if not cached.
///   ttl (TimeSpan?) - Optional TTL override.
/// Return (byte[]?): The cached or newly created value, or null if not available.
/// </summary>
public async Task<byte[]?> GetOrCreateAsync(string key, Func<Task<byte[]?>> factory, TimeSpan? ttl = null)
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

**Provider Constants**
- Provider-specific constants (e.g., `WeatherProviderConstants`, `DeviceProviderConstants`) are placed in `Common/Constants/ProviderConstants/`.
- Namespace remains `Common` (flat), despite subfolder.

**Conventions**
- **Enum baseline:** Always `NotSet = 0` as first entry.
- **Constants naming:** UPPER_SNAKE_CASE (e.g., `DEFAULT_CULTURE`, `IIS_DEV_SECRETS_FILE_PATH`).
- **Placeholders:** `[UPPERCASE_WITH_UNDERSCORES]` (e.g., `[PROJECT_TITLE]`, `[APP_ENVIRONMENT]`, `[VERSION]`), replaced via `CommonHelper.ReplacePlaceholders(...)` before processing.

---

## Object Initialization Style
- For local variables of known types, use the style:  

```csharp
Dictionary<string, string> replacements = new();
```

- When initializing with attributes, use inline object initializers:  

```csharp
User systemUser = new()
{
    Id = systemUserId,
    Firstname = "System",
    IsSystemUser = true
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

- Developer discretion: May indent long single statements to the next line for readability.

---

## File Headers (C# Files)
- **All C# files in subdirectories** should include a header comment block to identify their location and purpose.
- Use the following format at the top, after usings and before namespace:

```csharp
#region Usings
using System;
#endregion

// Project:     WebsiteAdmin
// File:        HomeController.Dashboard.BackgroundJobs.cs
// Location:    Controllers/HomeControllerDashboardFiles
// Type:        Partial class - Dashboard data method

namespace WebsiteAdmin;
```

- **Purpose:** Files with similar names (especially partials) need clear identification.
- **Location:** Helps navigate between related files in subdirectories.
- **Type:** Brief description of file purpose (e.g., "Partial class", "ViewModel", "Helper").

---

## Views (.cshtml Files)
- **Header comments required** for all views to identify their purpose and location.
- Use the following format at the top of each view file:

```cshtml
@* 
   Project:     WebsiteAdmin
   File:        Create.cshtml
   Location:    Views/Customer
   Type:        Customer creation form
*@
@model CustomerViewModel
```

- **Purpose:** Views with common names (Create.cshtml, Edit.cshtml, Index.cshtml) need clear identification.
- **Location:** Helps navigate between similar-named files.
- **Type:** Brief description of what the view does.

---

## Example File Layout (File-scoped Namespace)

```csharp
#region Usings
using System.Collections.Concurrent;
#endregion

namespace Common;

/// <summary>
/// Aim: Separate in-memory cache for crypto keys (byte[]).
/// Keys have a very short TTL; values are zeroized on remove, expire, or clear.
/// </summary>
public sealed class KeyCache
{
    #region Private Fields
    private sealed class Entry
    {
        public byte[]? Value { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
    }

    private readonly ConcurrentDictionary<string, Entry> _cache = new();
    private readonly ConcurrentDictionary<string, object> _locks = new();
    private readonly TimeSpan _defaultTtl;
    #endregion

    #region Constructor
    /// <summary>
    /// Aim: Initializes a new cache instance with a default TTL.
    /// Params:
    ///   defaultTtl (TimeSpan?) - Optional default time-to-live for entries. Defaults to 2 minutes.
    /// </summary>
    public KeyCache(TimeSpan? defaultTtl = null)
    {
        _defaultTtl = defaultTtl ?? TimeSpan.FromMinutes(2);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Aim: Adds or replaces a cache entry with the specified TTL.
    /// Params:
    ///   key (string) - The cache key. Must not be null or whitespace.
    ///   value (byte[]) - The byte array value. Must not be null.
    ///   ttl (TimeSpan?) - Optional time-to-live. Defaults to the cache's default TTL.
    /// </summary>
    public void Set(string key, byte[] value, TimeSpan? ttl = null)
    {
        // ...
    }
    #endregion
}
```

---

## Project Structure

```
src/
├── powershell-scripts/
│   ├── Prebuild-Local.ps1
│   ├── Create-Local-Migrations.ps1
│   └── Update-Local-Database.ps1
│
├── libs/
│   ├── Common/
│   │   └── Common.sln
│   │       ├── Common
│   │       └── Common.Tests
│   │
│   ├── ApiService/
│   │   └── ApiService.sln
│   │       ├── ApiService
│   │       ├── ApiService.Tests
│   │       └── [Common] (extern)
│   │
│   ├── WeatherProviders/
│   │   └── WeatherProviders.sln
│   │       ├── WeatherProviderMeteonomiqs
│   │       ├── WeatherProviders.Tests
│   │       ├── [ApiService] (extern)
│   │       └── [Common] (extern)
│   │
│   ├── SecretProviders/
│   │   └── SecretProvider.sln
│   │       ├── LocalDevSecrets
│   │       ├── AwsSecrets
│   │       ├── SecretProvider.Tests
│   │       └── [Common] (extern)
│   │
│   └── BackgroundJobs/
│       └── BackgroundJobs.sln
│           ├── BackgroundJobs
│           ├── ConsoleTest
│           ├── BackgroundJobs.Tests
│           └── [Common] (extern)
│
└── apps/
    ├── Mobile/
    │   └── Mobile.sln
    │       ├── Mobile
    │       ├── Mobile.Tests
    │       └── Shared (extern)/
    │           ├── [Common]
    │           └── [ApiService]
    │
    ├── WebsiteAdmin/
    │   └── WebsiteAdmin.sln
    │       ├── WebsiteAdmin
    │       ├── WebsiteAdmin.Tests
    │       ├── Projektmappenelemente/
    │       │   └── .editorconfig
    │       └── Shared (extern)/
    │           └── [Common]
    │
    ├── ApiMobile/
    │   └── ApiMobile.sln
    │       ├── ApiMobile
    │       ├── ApiMobile.Tests
    │       └── Shared (extern)/
    │           └── [Common]
    │
    └── BackgroundJobsHost/
        └── BackgroundJobsHost.sln
            ├── BackgroundJobsHost
            ├── ConsoleTest
            ├── BackgroundJobsHost.Tests
            └── [Common] (extern)

Legend:
- [ProjectName] = External project reference (from another solution)
- (extern) = Solution folder for external references
```

### Dependency Hierarchy
- `Common` → Base (no dependencies)
- `ApiService` → Common
- `WeatherProviders` → ApiService, Common
- `SecretProviders` → Common
- `BackgroundJobs` → Common
- Apps → Common + specific libs as needed

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
- [ ] **Provider Constants** in `Common/Constants/ProviderConstants/` with `namespace Common;`.
