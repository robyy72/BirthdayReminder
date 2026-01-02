param(
    [string]$Action,
    [string]$Identifier,
    [string]$RecordName,
    [string]$Token
)

# Log to file for reference
$logPath = "C:\Certificates\BirthdayReminder\dns-challenge.txt"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$content = @"
============================================
  MANUAL DNS TXT RECORD REQUIRED
============================================
  Time: $timestamp

  Please create the following TXT record at IONOS:

  Host:  $RecordName
  Type:  TXT
  Value: $Token

============================================
"@

$content | Out-File $logPath -Force
Write-Host $content

# Wait for DNS propagation (user needs to create record)
# Check every 10 seconds for up to 5 minutes
$maxAttempts = 30
$attempt = 0
$found = $false

Write-Host ""
Write-Host "Waiting for DNS record to propagate..." -ForegroundColor Yellow

while ($attempt -lt $maxAttempts -and -not $found) {
    Start-Sleep -Seconds 10
    $attempt++

    try {
        $result = nslookup -type=TXT $RecordName 8.8.8.8 2>&1
        if ($result -match [regex]::Escape($Token)) {
            $found = $true
            Write-Host "DNS record found! Continuing..." -ForegroundColor Green
        } else {
            Write-Host "Attempt $attempt/$maxAttempts - Record not found yet..." -ForegroundColor Gray
        }
    } catch {
        Write-Host "Attempt $attempt/$maxAttempts - DNS query failed..." -ForegroundColor Gray
    }
}

if (-not $found) {
    Write-Host "WARNING: DNS record not verified after 5 minutes. Continuing anyway..." -ForegroundColor Red
}
