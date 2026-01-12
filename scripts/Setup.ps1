# Setup.ps1
# Main setup script that calls database and IIS setup

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Function to generate a random password
function New-RandomPassword {
    param([int]$Length = 32)
    $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*'
    -join ((1..$Length) | ForEach-Object { $chars[(Get-Random -Maximum $chars.Length)] })
}

Write-Host "=== BirthdayReminder Setup ===" -ForegroundColor Cyan
Write-Host ""

# Prompt for DbPassword once (used by both database and IIS setup)
Write-Host "Enter the database password for SQL user 'birthdayreminder' (or press Enter to generate):" -ForegroundColor Yellow
$securePassword = Read-Host "DbPassword" -AsSecureString
$DbPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
)

if (-not $DbPassword) {
    $DbPassword = New-RandomPassword -Length 32
    Write-Host "Generated DbPassword: $DbPassword" -ForegroundColor Green
    Write-Host "  (Save this password!)" -ForegroundColor Yellow
}

Write-Host ""

# Run database setup for Dev
Write-Host "Step 1a: Setting up Dev database..." -ForegroundColor Yellow
& "$scriptPath\Setup-Databases.ps1" -Environment Dev -Password $DbPassword
if ($LASTEXITCODE -ne 0) {
    Write-Host "Dev database setup failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""

# Run database setup for Prod
Write-Host "Step 1b: Setting up Prod database..." -ForegroundColor Yellow
& "$scriptPath\Setup-Databases.ps1" -Environment Prod -Password $DbPassword
if ($LASTEXITCODE -ne 0) {
    Write-Host "Prod database setup failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""

# Run IIS setup
Write-Host "Step 2: Setting up IIS sites..." -ForegroundColor Yellow
& "$scriptPath\Setup-IIS-Sites.ps1" -DbPassword $DbPassword
if ($LASTEXITCODE -ne 0) {
    Write-Host "IIS setup failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Setup completed ===" -ForegroundColor Green
