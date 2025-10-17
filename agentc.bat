@echo off
SETLOCAL

cd /d "%~dp0"
CALL build_support\windows.env.bat

:: Create directories if they don't exist
if not exist "%AGENT_C_CONFIG_PATH%" mkdir "%AGENT_C_CONFIG_PATH%"
if not exist "%AGENT_C_IMAGES_PATH%" mkdir "%AGENT_C_IMAGES_PATH%"
if not exist "%AGENT_C_AGENTS_PATH%" mkdir "%AGENT_C_AGENTS_PATH%"
if not exist "%AGENT_C_SAVED_CHAT_FOLDER%" mkdir "%AGENT_C_SAVED_CHAT_FOLDER%"

:: Check if config file exists
if not exist "%AGENT_C_CONFIG_PATH%\agent_c.config" (
    copy build_support\config\agent_c.config.example "%AGENT_C_CONFIG_PATH%\agent_c.config"
    copy build_support\data\*.db  "%AGENT_C_CONFIG_PATH%"
    echo ** Warning**: Configuration file not found at %AGENT_C_CONFIG_PATH%\agent_c.config
    echo An example file has been added to %AGENT_C_CONFIG_PATH%
    echo.
    echo Please edit the configuration file and rerun this script.
    echo.
    pause
    start notepad "%AGENT_C_CONFIG_PATH%\agent_c.config"
    exit /b 1
)

docker-compose -f docker-compose.yml -p agent_c %*

ENDLOCAL