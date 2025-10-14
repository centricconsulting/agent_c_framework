@echo off
SETLOCAL

:: Change to the scripts directory
cd /d "%~dp0"

echo Starting Agent C API and Client...
echo.

:: Start the API server in a new window
echo Starting API server...
start "Agent C API" cmd /k "start_api.bat"

:: Wait a moment for the API to start
timeout /t 3 /nobreak > nul

:: Start the client in a new window
echo Starting client...
start "Agent C Client" cmd /k "start_client.bat"

echo.
echo Both services are starting in separate windows.
echo Close this window when you're done.
pause
