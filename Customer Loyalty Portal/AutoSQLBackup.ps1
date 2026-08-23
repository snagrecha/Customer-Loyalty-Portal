# AutoSQLBackup.ps1
# Standalone automated backup script for Customer Loyalty Portal databases
# Targets: CustomerLoyalty, GRExtreme_PantHouseJ (TPH), GRetail_JUNIOR_New (Junior)
# Destination: Google Drive (G:\My Drive\SQLBackups)

$ErrorActionPreference = "Continue"

$timestamp = Get-Date -Format "yyyy-MM-dd_HHmm"
$googleDriveDir = "G:\My Drive\SQLBackups"
$stagingDir = "D:\SQLBackups\Staging"

if (!(Test-Path $googleDriveDir)) {
    try { New-Item -ItemType Directory -Path $googleDriveDir -Force | Out-Null } catch {}
}
if (!(Test-Path $stagingDir)) {
    try { New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null } catch {}
}

$logFile = "$googleDriveDir\BackupLog.txt"

function Write-BackupLog($msg) {
    $entry = "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] $msg"
    Write-Host $entry
    try { Add-Content -Path $logFile -Value $entry -ErrorAction SilentlyContinue } catch {}
}

Write-BackupLog "=================================================="
Write-BackupLog "Starting Automated SQL Backup to Google Drive ($googleDriveDir)"

Add-Type -AssemblyName System.IO.Compression.FileSystem

$targets = @(
    @{
        Name = "CustomerLoyalty"
        Server = "TPH,1520\SQLSTANDARD19"
        DB = "CustomerLoyalty"
        IsRemoteTPH = $true
        CompressQuery = $true
    },
    @{
        Name = "TPH_GRExtreme"
        Server = "TPH,1520\SQLSTANDARD19"
        DB = "GRExtreme_PantHouseJ"
        IsRemoteTPH = $true
        CompressQuery = $true
    },
    @{
        Name = "Junior_GRetail"
        Server = "JUNIOR\SQLEXPRESS"
        DB = "GRetail_JUNIOR_New"
        IsRemoteTPH = $false
        CompressQuery = $false
    }
)

$successCount = 0

foreach ($t in $targets) {
    $dbFolder = "$googleDriveDir\$($t.Name)"
    if (!(Test-Path $dbFolder)) {
        try { New-Item -ItemType Directory -Path $dbFolder -Force | Out-Null } catch {}
    }

    $finalZip = "$dbFolder\$($t.Name)_$timestamp.zip"
    $bakName = "$($t.Name)_$timestamp.bak"

    if ($t.IsRemoteTPH) {
        $rawBakOnServer = "E:\AutoBackup\$bakName"
        $clientBakPath = "Z:\AutoBackup\$bakName"
    } else {
        $rawBakOnServer = "$stagingDir\$bakName"
        $clientBakPath = $rawBakOnServer
    }

    try {
        Write-BackupLog "[$($t.Name)] Backing up database '$($t.DB)' on '$($t.Server)'..."
        $connStr = "Server=$($t.Server);Initial Catalog=$($t.DB);UID=cl_admin;PWD=Tph@2015;Connect Timeout=30;Pooling=False"
        $conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
        $conn.Open()
        
        $cmd = $conn.CreateCommand()
        $cmd.CommandTimeout = 300
        $compClause = if ($t.CompressQuery) { ", COMPRESSION" } else { "" }
        $cmd.CommandText = "BACKUP DATABASE [$($t.DB)] TO DISK = '$rawBakOnServer' WITH FORMAT, INIT$compClause;"
        $cmd.ExecuteNonQuery() | Out-Null
        $conn.Close()

        # Wait for file visibility
        $waited = 0
        while (!(Test-Path $clientBakPath) -and ($waited -lt 10)) {
            Start-Sleep -Seconds 1
            $waited++
        }

        if (!(Test-Path $clientBakPath)) {
            throw "Backup file not found at: $clientBakPath"
        }

        # Compress to .zip
        if (Test-Path $finalZip) { Remove-Item $finalZip -Force }
        $zip = [System.IO.Compression.ZipFile]::Open($finalZip, [System.IO.Compression.ZipArchiveMode]::Create)
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $clientBakPath, (Split-Path $clientBakPath -Leaf), [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
        $zip.Dispose()

        $zipItem = Get-Item $finalZip
        $mb = [math]::Round($zipItem.Length / 1MB, 1)
        Write-BackupLog "[$($t.Name)] SUCCESS -> $finalZip ($mb MB)"

        # Clean up temporary staging .bak
        if (Test-Path $clientBakPath) {
            Remove-Item $clientBakPath -Force -ErrorAction SilentlyContinue
        }

        $successCount++
    } catch {
        Write-BackupLog "[$($t.Name)] ERROR: $($_.Exception.Message)"
    }
}

# 15-Day Retention Cleanup
$cutoff = (Get-Date).AddDays(-15)
$oldFiles = Get-ChildItem -Path $googleDriveDir -Filter "*.zip" -Recurse | Where-Object { $_.LastWriteTime -lt $cutoff -and $_.CreationTime -lt $cutoff }
foreach ($f in $oldFiles) {
    try {
        Remove-Item $f.FullName -Force
        Write-BackupLog "Retention Cleanup: Removed old archive $($f.Name)"
    } catch {}
}

Write-BackupLog "Backup Completed: $successCount/$($targets.Count) databases backed up successfully."
Write-BackupLog "=================================================="
