# STORM FELLOWSHIP v0.0.1 — PowerShell Uninstaller Script
Write-Host "=============================================" -ForegroundColor Yellow
Write-Host "  UNINSTALLING STORM FELLOWSHIP" -ForegroundColor White
Write-Host "=============================================" -ForegroundColor Yellow

$localAppData = [System.Environment]::GetFolderPath('LocalApplicationData')
$installDir = Join-Path $localAppData "StormFellowship"
$desktopDir = [System.Environment]::GetFolderPath('Desktop')
$startMenuDir = Join-Path ([System.Environment]::GetFolderPath('StartMenu')) "Programs\STORM FELLOWSHIP"

# Kill running process
Stop-Process -Name "StormFellowship" -Force -ErrorAction SilentlyContinue

# Remove Registry entries
Remove-Item -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\StormFellowship" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "HKCU:\Software\Classes\storm" -Recurse -Force -ErrorAction SilentlyContinue

# Remove Shortcuts
Remove-Item -Path (Join-Path $desktopDir "STORM FELLOWSHIP.lnk") -Force -ErrorAction SilentlyContinue
Remove-Item -Path $startMenuDir -Recurse -Force -ErrorAction SilentlyContinue

# Remove Install Directory
Remove-Item -Path $installDir -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "`n[SUCCESS] STORM FELLOWSHIP has been completely uninstalled." -ForegroundColor Green
