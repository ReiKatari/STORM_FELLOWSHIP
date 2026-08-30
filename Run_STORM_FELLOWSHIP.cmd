@echo off
title STORM FELLOWSHIP v0.2.2 - Launcher
cd /d "%~dp0"

powershell -NoProfile -Command "Get-ChildItem -Path '%~dp0' -Recurse | Unblock-File -ErrorAction SilentlyContinue" >nul 2>&1

start "" "dotnet" "%~dp0Assembling\StormFellowship.dll"
exit
