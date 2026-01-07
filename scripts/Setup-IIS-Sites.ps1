# Setup-IIS-Sites.ps1
# Sets up IIS sites for BirthdayReminder (requires wildcard certificate)

param(
    [string]$Domain = "birthday-reminder.online"
)

Write-Host "=== IIS Sites Setup ===" -ForegroundColor Cyan

# Check for admin privileges
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin)
{
    Write-Host "This script requires administrator privileges. Please run as Administrator." -ForegroundColor Red
    exit 1
}

# Import WebAdministration module
Import-Module WebAdministration -ErrorAction SilentlyContinue
if (-not (Get-Module WebAdministration))
{
    Write-Host "WebAdministration module not found. Please install IIS." -ForegroundColor Red
    exit 1
}

# Check for wildcard certificate
Write-Host "Checking for wildcard certificate: *.$Domain" -ForegroundColor Yellow

$wildcardPattern = "*.$Domain"
$cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {
    $_.Subject -like "*$wildcardPattern*" -or
    ($_.Extensions | Where-Object { $_.Oid.FriendlyName -eq "Subject Alternative Name" } | ForEach-Object {
        $_.Format($false) -like "*$wildcardPattern*"
    })
}

if (-not $cert)
{
    Write-Host ""
    Write-Host "Wildcard certificate for *.$Domain not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install a wildcard certificate before running this script." -ForegroundColor Yellow
    Write-Host "You can use scripts/create-certificate.ps1 for a self-signed certificate (dev only)" -ForegroundColor Yellow
    Write-Host "or obtain a certificate from Let's Encrypt or another CA." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Setup cancelled." -ForegroundColor Red
    exit 1
}

Write-Host "Wildcard certificate found: $($cert.Thumbprint)" -ForegroundColor Green
$thumbprint = $cert.Thumbprint

# Define sites configuration
$sites = @(
    @{
        Name = "BirthdayReminder-WebsiteCustomer"
        AppPool = "BirthdayReminder-WebsiteCustomer"
        Path = "C:\inetpub\BirthdayReminder\WebsiteCustomer\Prod\current"
        Bindings = @(
            @{ Host = $Domain; Port = 443; Protocol = "https" }
        )
    },
    @{
        Name = "BirthdayReminder-WebsiteAdmin-Dev"
        AppPool = "BirthdayReminder-WebsiteAdmin-Dev"
        Path = "C:\inetpub\BirthdayReminder\WebsiteAdmin\Dev\current"
        Bindings = @(
            @{ Host = "dev.$Domain"; Port = 443; Protocol = "https" }
        )
    },
    @{
        Name = "BirthdayReminder-WebsiteAdmin-Prod"
        AppPool = "BirthdayReminder-WebsiteAdmin-Prod"
        Path = "C:\inetpub\BirthdayReminder\WebsiteAdmin\Prod\current"
        Bindings = @(
            @{ Host = "prod.$Domain"; Port = 443; Protocol = "https" }
        )
    }
)

foreach ($site in $sites)
{
    Write-Host ""
    Write-Host "Setting up: $($site.Name)" -ForegroundColor Yellow

    # Create directory structure
    $basePath = Split-Path -Parent $site.Path
    if (-not (Test-Path $basePath))
    {
        New-Item -ItemType Directory -Path $basePath -Force | Out-Null
        Write-Host "  Created directory: $basePath"
    }

    # Create AppPool if not exists
    if (-not (Test-Path "IIS:\AppPools\$($site.AppPool)"))
    {
        New-WebAppPool -Name $site.AppPool | Out-Null
        Set-ItemProperty "IIS:\AppPools\$($site.AppPool)" -Name "managedRuntimeVersion" -Value ""
        Write-Host "  Created AppPool: $($site.AppPool)"
    }
    else
    {
        Write-Host "  AppPool exists: $($site.AppPool)"
    }

    # Create Site if not exists
    if (-not (Test-Path "IIS:\Sites\$($site.Name)"))
    {
        $firstBinding = $site.Bindings[0]
        New-Website -Name $site.Name -PhysicalPath $basePath -ApplicationPool $site.AppPool | Out-Null
        Write-Host "  Created Site: $($site.Name)"
    }
    else
    {
        Write-Host "  Site exists: $($site.Name)"
    }

    # Configure bindings
    foreach ($binding in $site.Bindings)
    {
        $bindingInfo = "*:$($binding.Port):$($binding.Host)"

        # Remove existing binding if any
        Get-WebBinding -Name $site.Name -Protocol $binding.Protocol -HostHeader $binding.Host -ErrorAction SilentlyContinue | Remove-WebBinding -ErrorAction SilentlyContinue

        # Add HTTPS binding with certificate
        New-WebBinding -Name $site.Name -Protocol $binding.Protocol -Port $binding.Port -HostHeader $binding.Host -SslFlags 1

        # Bind certificate
        $bindingPath = "IIS:\SslBindings\!$($binding.Port)!$($binding.Host)"
        if (Test-Path $bindingPath)
        {
            Remove-Item $bindingPath -Force
        }
        New-Item -Path $bindingPath -Value $cert -Force | Out-Null

        Write-Host "  Configured binding: https://$($binding.Host)"
    }
}

Write-Host ""
Write-Host "IIS setup completed successfully." -ForegroundColor Green
Write-Host ""
Write-Host "Sites configured:" -ForegroundColor Cyan
Write-Host "  - https://$Domain (WebsiteCustomer)"
Write-Host "  - https://dev.$Domain (WebsiteAdmin Dev)"
Write-Host "  - https://prod.$Domain (WebsiteAdmin Prod)"
