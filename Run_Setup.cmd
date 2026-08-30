@echo off
title STORM FELLOWSHIP v0.2.2 - Setup Launcher
cd /d "%~dp0"

powershell -NoProfile -Command "Get-ChildItem -Path '%~dp0' -Recurse | Unblock-File -ErrorAction SilentlyContinue" >nul 2>&1

if exist "%~dp0Files\STORM_FELLOWSHIP_0.2.2_setup.exe" (
    start "" "%~dp0Files\STORM_FELLOWSHIP_0.2.2_setup.exe"
    exit /b 0
)

start "" "dotnet" "%~dp0Sources\StormFellowship.Installer\bin\Release\net8.0-windows\win-x64\StormFellowship.Installer.dll"
exit
