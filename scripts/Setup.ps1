# Setup.ps1
# Main setup script that calls database and IIS setup

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "=== BirthdayReminder Setup ===" -ForegroundColor Cyan
Write-Host ""

# Run database setup
Write-Host "Step 1: Setting up databases..." -ForegroundColor Yellow
& "$scriptPath\Setup-Databases.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Database setup failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""

# Run IIS setup
Write-Host "Step 2: Setting up IIS sites..." -ForegroundColor Yellow
& "$scriptPath\Setup-IIS-Sites.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "IIS setup failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Setup completed ===" -ForegroundColor Green
