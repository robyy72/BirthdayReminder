param(
    [string]$Action,
    [string]$Identifier,
    [string]$RecordName,
    [string]$Token
)

Write-Host ""
Write-Host "============================================" -ForegroundColor Yellow
Write-Host "  DNS TXT RECORD CAN BE REMOVED" -ForegroundColor Yellow
Write-Host "============================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "You can now delete the TXT record at IONOS:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Host:  $RecordName" -ForegroundColor Red
Write-Host "  Value: $Token" -ForegroundColor Red
Write-Host ""
Write-Host "============================================" -ForegroundColor Yellow

# Log to file
$logPath = "C:\Certificates\BirthdayReminder\dns-challenge.log"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
"$timestamp DELETE: $RecordName = $Token" | Out-File -Append $logPath
