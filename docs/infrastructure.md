# Infrastructure

## Domain

`birthday-reminder.online`

## Subdomains

| Project | Dev | Prod |
|---------|-----|------|
| WebsiteAdmin | `dev.birthday-reminder.online` | `prod.birthday-reminder.online` |
| ApiMobile | `api-dev.birthday-reminder.online` | `api-prod.birthday-reminder.online` |
| WebsiteCustomer | - | `birthday-reminder.online` |

**Note:** WebsiteCustomer (landing page) has no dev environment - only one production site.

## Public URLs

| Purpose | URL |
|---------|-----|
| Privacy Policy | `https://birthday-reminder.online/privacy` |
| Terms of Service | `https://birthday-reminder.online/terms` |

## Projects

| Project | Description |
|---------|-------------|
| Mobile | .NET MAUI App (Android & iOS) |
| ApiMobile | ASP.NET Core API for mobile app |
| WebsiteAdmin | ASP.NET Core admin website (dev + prod) |
| WebsiteCustomer | ASP.NET Core landing page (prod only) |
| Common | Shared class library |
| MobileLanguages | Localization resources (resx) |

## JWT Configuration

- **Issuer:** `birthday-reminder.online`
- **Audience:** `birthday-reminder-mobile`

## Admin Account

- **Email:** `admin@birthday-reminder.online`
