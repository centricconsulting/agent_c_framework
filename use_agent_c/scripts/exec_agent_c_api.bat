@echo off
SETLOCAL

:: Change to the directory containing this script
cd /d "%~dp0"
cd ..
:: Set environment variables for agent_c config and directories
:: %USERPROFILE% is equivalent to HOME on Windows
set AGENT_C_CONFIG_PATH=%USERPROFILE%\.agent_c
set AGENT_C_IMAGES_PATH=%USERPROFILE%\.agent_c\images
set AGENT_C_PERSONAS_PATH=%USERPROFILE%\.agent_c\personas
set AGENT_C_SAVED_CHAT_FOLDER=%USERPROFILE%\.agent_c\saved_sessions

:: Add mappings for workspace folders (Documents, Desktop, and Downloads)
set DOCUMENTS_WORKSPACE=%USERPROFILE%\Documents
set DESKTOP_WORKSPACE=%USERPROFILE%\Desktop
set DOWNLOADS_WORKSPACE=%USERPROFILE%\Downloads

:: Create directories if they don't exist
if not exist "%AGENT_C_CONFIG_PATH%" mkdir "%AGENT_C_CONFIG_PATH%"
if not exist "%AGENT_C_IMAGES_PATH%" mkdir "%AGENT_C_IMAGES_PATH%"
if not exist "%AGENT_C_PERSONAS_PATH%" mkdir "%AGENT_C_PERSONAS_PATH%"
if not exist "%AGENT_C_SAVED_CHAT_FOLDER%" mkdir "%AGENT_C_SAVED_CHAT_FOLDER%"

:: Check if config file exists
if not exist "%AGENT_C_CONFIG_PATH%\agent_c.config" (
    copy config\agent_c.config.example "%AGENT_C_CONFIG_PATH%\agent_c.config"
    copy data\*.db  "%AGENT_C_CONFIG_PATH%"
    echo ** Warning**: Configuration file not found at %AGENT_C_CONFIG_PATH%\agent_c.config
    echo An example file has been added to %AGENT_C_CONFIG_PATH%
    echo.
    echo Please edit the configuration file and rerun this script.
    echo.
    pause
    start notepad "%AGENT_C_CONFIG_PATH%\agent_c.config"
    exit /b 1
)


docker-compose -f docker-compose.yml -p agent_c exec %*


ENDLOCAL