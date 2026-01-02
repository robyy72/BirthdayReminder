# BirthdayReminder - Wildcard Certificate Creation Script
# Run this script in an elevated PowerShell terminal

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  BirthdayReminder Certificate Setup" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Start win-acme interactively
& 'C:\ProgramData\chocolatey\bin\wacs.exe' `
    --source manual `
    --host "birthday-reminder.online,*.birthday-reminder.online" `
    --validation manual `
    --validationmode dns-01 `
    --store certificatestore,pemfiles `
    --pemfilespath "C:\Certificates\BirthdayReminder" `
    --certificatestore My `
    --accepttos `
    --emailaddress robyy@gmx.de

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "  Certificate creation complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Certificates are stored in:" -ForegroundColor Yellow
Write-Host "  - Windows Certificate Store (My)" -ForegroundColor White
Write-Host "  - C:\Certificates\BirthdayReminder\" -ForegroundColor White
Write-Host ""
