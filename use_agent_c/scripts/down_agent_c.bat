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

:: Run in detached mode (background):
:: Pass the PROJECT_WORKSPACE_PATH environment variable to docker-compose
docker-compose -f docker-compose.yml -p agent_c down

ENDLOCAL