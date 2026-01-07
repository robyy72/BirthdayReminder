# Setup-Databases.ps1
# Creates SQL Server databases and user with appropriate permissions

param(
    [string]$SqlServer = "localhost",
    [string]$Username = "birthdayreminder",
    [string]$Password
)

$databases = @("BirthdayReminder-Dev", "BirthdayReminder-Prod")

Write-Host "=== Database Setup ===" -ForegroundColor Cyan

# Prompt for password if not provided
if (-not $Password)
{
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

# Create databases and assign permissions
foreach ($db in $databases)
{
    Write-Host ""
    Write-Host "Setting up database: $db" -ForegroundColor Yellow

    $setupDbSql = @"
-- Create database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$db')
BEGIN
    CREATE DATABASE [$db]
    PRINT 'Database $db created'
END
ELSE
BEGIN
    PRINT 'Database $db already exists'
END
GO

USE [$db]
GO

-- Create user if not exists
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '$Username')
BEGIN
    CREATE USER [$Username] FOR LOGIN [$Username]
    PRINT 'User $Username created in $db'
END
ELSE
BEGIN
    PRINT 'User $Username already exists in $db'
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
        Write-Host "Failed to setup database $db : $result" -ForegroundColor Red
        exit 1
    }
    Write-Host $result
}

Write-Host ""
Write-Host "Database setup completed successfully." -ForegroundColor Green
Write-Host ""
Write-Host "Connection string format:" -ForegroundColor Cyan
Write-Host "Server=$SqlServer;Database=BirthdayReminder-Dev;User Id=$Username;Password=<password>;TrustServerCertificate=True"
