# Container entrypoint: writes Orchard Settings.txt from environment variables
# then starts IIS and tails logs.

$ErrorActionPreference = 'Stop'

Write-Host "=== Orchard CMS Container Entrypoint ==="
Write-Host "Environment: $env:ORCHARD_ENVIRONMENT"

# Parse database credentials from Secrets Manager JSON (injected by ECS)
$settingsPath = "C:\inetpub\orchard\App_Data\Sites\Default\Settings.txt"

if ($env:ORCHARD_DB_SECRET_ARN) {
    # ECS injects the secret JSON as the environment variable value
    $secretJson = $env:ORCHARD_DB_SECRET_ARN | ConvertFrom-Json
    $dbUser = $secretJson.username
    $dbPass = $secretJson.password
} else {
    # Fallback for local development
    $dbUser = if ($env:ORCHARD_DB_USER) { $env:ORCHARD_DB_USER } else { "sa" }
    $dbPass = if ($env:ORCHARD_DB_PASSWORD) { $env:ORCHARD_DB_PASSWORD } else { "LocalDev123!" }
}

$dbServer = if ($env:ORCHARD_DB_SERVER) { $env:ORCHARD_DB_SERVER } else { "localhost" }
$dbPort = if ($env:ORCHARD_DB_PORT) { $env:ORCHARD_DB_PORT } else { "1433" }
$dbName = if ($env:ORCHARD_DB_NAME) { $env:ORCHARD_DB_NAME } else { "OrchardCms" }

# Only write Settings.txt if it does not already exist (preserve existing setup)
if (-not (Test-Path $settingsPath)) {
    Write-Host "Writing Settings.txt for Default tenant..."
    $connectionString = "Server=$dbServer,$dbPort;Database=$dbName;User Id=$dbUser;Password=$dbPass;TrustServerCertificate=True;"

    $settings = @"
Name: Default
DataProvider: SqlServer
DataConnectionString: $connectionString
State: Running
"@

    New-Item -ItemType Directory -Force -Path (Split-Path $settingsPath) | Out-Null
    Set-Content -Path $settingsPath -Value $settings -Encoding UTF8
    Write-Host "Settings.txt written successfully."
} else {
    Write-Host "Settings.txt already exists - skipping generation."
}

Write-Host "Starting IIS..."

# Start IIS service
Start-Service W3SVC

Write-Host "=== Orchard CMS is running on port 80 ==="

# Keep container alive by tailing IIS logs
$logPath = "C:\inetpub\logs\LogFiles\W3SVC1"
while (-not (Test-Path "$logPath\*.log")) {
    Start-Sleep -Seconds 2
}
Get-ChildItem "$logPath\*.log" | Sort-Object LastWriteTime | Select-Object -Last 1 | Get-Content -Wait
