# Clean script for BirthdayReminder
# Deletes bin/obj folders and runs dotnet clean

Write-Host "Cleaning BirthdayReminder solution..." -ForegroundColor Cyan

# Get script directory
$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Delete bin and obj folders
Write-Host "Deleting bin and obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path $rootDir -Include bin,obj -Recurse -Directory | ForEach-Object {
    Write-Host "  Removing: $($_.FullName)"
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

# Run dotnet clean
Write-Host "Running dotnet clean..." -ForegroundColor Yellow
dotnet clean "$rootDir\BirthdayReminder.sln" --verbosity minimal

Write-Host "Done!" -ForegroundColor Green
