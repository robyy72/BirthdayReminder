# Setup-Databases.ps1
# Creates SQL Server databases and user with appropriate permissions
# Uses Windows Authentication (Trusted Connection) to connect to SQL Server

param(
    [ValidateSet("Local", "Dev", "Prod")]
    [string]$Environment = "Local",
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$Username = "birthdayreminder",
    [string]$Password
)

# Set database name based on environment
$database = "BirthdayReminder-$Environment"

Write-Host "=== Database Setup ===" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Cyan
Write-Host "Database: $database" -ForegroundColor Cyan
Write-Host ""
Write-Host "Connecting to SQL Server '$SqlServer' using Windows Authentication..." -ForegroundColor Yellow

# Prompt for password if not provided via parameter
if (-not $Password)
{
    Write-Host ""
    if ($Environment -eq "Local") {
        Write-Host "Password not provided. You can find it in secrets.json:" -ForegroundColor Yellow
        Write-Host "  %APPDATA%\Microsoft\UserSecrets\d5cf0508-0f02-4542-8e1a-72dfcef31cc0\secrets.json" -ForegroundColor Gray
        Write-Host ""
    }
    $securePassword = Read-Host "Enter password for SQL user '$Username'" -AsSecureString
    $Password = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    )
}

if (-not $Password)
{
    Write-Host "Password is required. Aborting." -ForegroundColor Red
    exit 1
}

# Check if sqlcmd is available
$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd)
{
    Write-Host "sqlcmd not found. Please install SQL Server command line tools." -ForegroundColor Red
    exit 1
}

# Create login and databases
$createLoginSql = @"
-- Create login if not exists
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$Username')
BEGIN
    CREATE LOGIN [$Username] WITH PASSWORD = '$Password'
    PRINT 'Login $Username created'
END
ELSE
BEGIN
    PRINT 'Login $Username already exists'
END
"@

Write-Host "Creating SQL login '$Username'..." -ForegroundColor Yellow
$result = sqlcmd -S $SqlServer -E -Q $createLoginSql 2>&1
if ($LASTEXITCODE -ne 0)
{
    Write-Host "Failed to create login: $result" -ForegroundColor Red
    exit 1
}
Write-Host $result

# Create database and assign permissions
Write-Host ""
Write-Host "Setting up database: $database" -ForegroundColor Yellow

$setupDbSql = @"
-- Create database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$database')
BEGIN
    CREATE DATABASE [$database]
    PRINT 'Database $database created'
END
ELSE
BEGIN
    PRINT 'Database $database already exists'
END
GO

USE [$database]
GO

-- Create user if not exists
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '$Username')
BEGIN
    CREATE USER [$Username] FOR LOGIN [$Username]
    PRINT 'User $Username created in $database'
END
ELSE
BEGIN
    PRINT 'User $Username already exists in $database'
END

-- Assign roles
ALTER ROLE db_datareader ADD MEMBER [$Username]
ALTER ROLE db_datawriter ADD MEMBER [$Username]
ALTER ROLE db_ddladmin ADD MEMBER [$Username]
PRINT 'Roles assigned: db_datareader, db_datawriter, db_ddladmin'
"@

$result = sqlcmd -S $SqlServer -E -Q $setupDbSql 2>&1
if ($LASTEXITCODE -ne 0)
{
    Write-Host "Failed to setup database $database : $result" -ForegroundColor Red
    exit 1
}
Write-Host $result

Write-Host ""
Write-Host "Database setup completed successfully." -ForegroundColor Green
Write-Host ""
Write-Host "Connection string:" -ForegroundColor Cyan
Write-Host "Server=$SqlServer;Database=$database;User Id=$Username;Password=<from secrets.json>;TrustServerCertificate=True"
