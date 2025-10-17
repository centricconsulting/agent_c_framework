@echo off
SETLOCAL

:: Change to the parent directory containing this script
cd /d "%~dp0"\..
call build_support\windows.env.bat
docker build -t ghcr.io/centricconsulting/agent_c_python_base:latest -f pythonbase.Dockerfile .
docker build -t ghcr.io/centricconsulting/agentc-api:latest -f api.Dockerfile .
docker build -t ghcr.io/centricconsulting/agentc-client:latest  -f client.Dockerfile src\typescript_client_sdk

ENDLOCAL