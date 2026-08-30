@echo off
chcp 65001 >nul
title STORM FELLOWSHIP v0.2.2

echo ===================================================
echo   ⚡ STORM FELLOWSHIP v0.2.2 — Запуск приложения
echo ===================================================
echo.

:: 1. Проверка и запуск из установленной директории пользователя
if exist "%LOCALAPPDATA%\Programs\StormFellowship\StormFellowship.exe" (
    start "" "%LOCALAPPDATA%\Programs\StormFellowship\StormFellowship.exe"
    exit /b 0
)

:: 2. Запуск через сборку Assembling
if exist "%~dp0Assembling\StormFellowship.dll" (
    start "" dotnet "%~dp0Assembling\StormFellowship.dll"
    exit /b 0
)

:: 3. Запуск через dotnet run
dotnet run --project "%~dp0Sources\StormFellowship\StormFellowship.csproj" -c Release
exit /b 0
