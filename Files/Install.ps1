# STORM FELLOWSHIP v0.2.2 - PowerShell Automated Installer Script
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  STORM FELLOWSHIP v0.2.2 - INSTALLER" -ForegroundColor White
Write-Host "=============================================" -ForegroundColor Cyan

$localAppData = [System.Environment]::GetFolderPath('LocalApplicationData')
$installDir = Join-Path $localAppData "StormFellowship"
$desktopDir = [System.Environment]::GetFolderPath('Desktop')
$startMenuDir = Join-Path ([System.Environment]::GetFolderPath('StartMenu')) "Programs\STORM FELLOWSHIP"
$sourceDir = "E:\STORM FELLOWSHIP\Assembling"
$redistDir = Join-Path $sourceDir "Redist"

# 1. Check and install bundled prerequisites
if (Test-Path $redistDir) {
    Write-Host "• Verifying and configuring bundled runtimes..." -ForegroundColor Gray
    
    # VC++ Redistributable
    $vcRedist = Join-Path $redistDir "vc_redist.x64.exe"
    if (Test-Path $vcRedist) {
        Write-Host "  -> Installing Visual C++ 2015-2022 Redistributable (x64)..." -ForegroundColor DarkGray
        Start-Process -FilePath $vcRedist -ArgumentList "/install /quiet /norestart" -Wait -NoNewWindow -ErrorAction SilentlyContinue
    }
    
    # WebView2 Bootstrapper
    $wvRedist = Join-Path $redistDir "MicrosoftEdgeWebview2Setup.exe"
    if (Test-Path $wvRedist) {
        Write-Host "  -> Installing WebView2 Evergreen Runtime..." -ForegroundColor DarkGray
        Start-Process -FilePath $wvRedist -ArgumentList "/silent /install" -Wait -NoNewWindow -ErrorAction SilentlyContinue
    }
}

Write-Host "• Installing to: $installDir" -ForegroundColor Gray

# Ensure directories
New-Item -ItemType Directory -Force -Path $installDir | Out-Null
New-Item -ItemType Directory -Force -Path $startMenuDir | Out-Null

# Copy release payload
Write-Host "• Copying application binaries and native libraries..." -ForegroundColor Gray
Copy-Item -Path "$sourceDir\*" -Destination $installDir -Recurse -Force

$exePath = Join-Path $installDir "StormFellowship.exe"
$iconPath = Join-Path $installDir "Assets\AppIcon.ico"

# Create Desktop Shortcut
Write-Host "• Creating Desktop shortcut..." -ForegroundColor Gray
$wsh = New-Object -ComObject WScript.Shell
$desktopShortcut = $wsh.CreateShortcut((Join-Path $desktopDir "STORM FELLOWSHIP.lnk"))
$desktopShortcut.TargetPath = $exePath
$desktopShortcut.WorkingDirectory = $installDir
$desktopShortcut.IconLocation = "$iconPath,0"
$desktopShortcut.Description = "STORM FELLOWSHIP - High-Performance Communication Platform"
$desktopShortcut.Save()

# Create Start Menu Shortcut
Write-Host "• Creating Start Menu shortcut..." -ForegroundColor Gray
$startShortcut = $wsh.CreateShortcut((Join-Path $startMenuDir "STORM FELLOWSHIP.lnk"))
$startShortcut.TargetPath = $exePath
$startShortcut.WorkingDirectory = $installDir
$startShortcut.IconLocation = "$iconPath,0"
$startShortcut.Description = "STORM FELLOWSHIP"
$startShortcut.Save()

# Registry registration (Add/Remove Programs)
Write-Host "• Registering in Windows Registry..." -ForegroundColor Gray
$regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\StormFellowship"
New-Item -Path $regPath -Force | Out-Null
Set-ItemProperty -Path $regPath -Name "DisplayName" -Value "STORM FELLOWSHIP"
Set-ItemProperty -Path $regPath -Name "DisplayVersion" -Value "0.2.2"
Set-ItemProperty -Path $regPath -Name "Publisher" -Value "ReiKatari"
Set-ItemProperty -Path $regPath -Name "DisplayIcon" -Value $iconPath
Set-ItemProperty -Path $regPath -Name "InstallLocation" -Value $installDir
Set-ItemProperty -Path $regPath -Name "UninstallString" -Value ('powershell.exe -ExecutionPolicy Bypass -File "' + $installDir + '\Uninstall.ps1"')
Set-ItemProperty -Path $regPath -Name "NoModify" -Value 1 -Type DWord
Set-ItemProperty -Path $regPath -Name "NoRepair" -Value 1 -Type DWord

# Protocol storm:// registration
$protoPath = "HKCU:\Software\Classes\storm"
New-Item -Path $protoPath -Force | Out-Null
Set-ItemProperty -Path $protoPath -Name "(default)" -Value "URL:STORM FELLOWSHIP Protocol"
Set-ItemProperty -Path $protoPath -Name "URL Protocol" -Value ""
$protoCmd = "HKCU:\Software\Classes\storm\shell\open\command"
New-Item -Path $protoCmd -Force | Out-Null
Set-ItemProperty -Path $protoCmd -Name "(default)" -Value ('"' + $exePath + '" "%1"')

# Copy uninstall script into install dir
Copy-Item -Path "$PSScriptRoot\Uninstall.ps1" -Destination (Join-Path $installDir "Uninstall.ps1") -Force -ErrorAction SilentlyContinue

Write-Host "`n[SUCCESS] STORM FELLOWSHIP v0.2.2 successfully installed with all runtimes embedded!" -ForegroundColor Green
Write-Host "Desktop shortcut and Start menu entry created." -ForegroundColor Cyan
