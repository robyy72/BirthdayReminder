# Clean script for BirthdayReminder
# Deletes bin/obj folders and runs dotnet clean

Write-Host "Cleaning BirthdayReminder solution..." -ForegroundColor Cyan

# Get root directory (parent of .claude folder)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir

# Delete bin and obj folders
Write-Host "Deleting bin and obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path $rootDir -Include bin,obj -Recurse -Directory | ForEach-Object {
    Write-Host "  Removing: $($_.FullName)"
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

# Run dotnet clean
Write-Host "Running dotnet clean..." -ForegroundColor Yellow
dotnet clean "$rootDir\BirthdayReminder.sln" --verbosity minimal

# macOS-specific: Clear Xamarin and Xcode caches
if ($IsMacOS) {
    Write-Host "Clearing macOS caches..." -ForegroundColor Yellow

    $xamarinCache = "$HOME/Library/Caches/Xamarin"
    if (Test-Path $xamarinCache) {
        Write-Host "  Removing: $xamarinCache"
        Remove-Item -Path $xamarinCache -Recurse -Force -ErrorAction SilentlyContinue
    }

    $derivedData = "$HOME/Library/Developer/Xcode/DerivedData"
    if (Test-Path $derivedData) {
        Write-Host "  Removing: $derivedData"
        Remove-Item -Path $derivedData -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Done!" -ForegroundColor Green
