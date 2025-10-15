@echo off
SETLOCAL

:: Change to the directory containing this script
cd /d "%~dp0"
cd ..\..
docker build -t agentc-api:latest -f dockerfiles\api.Dockerfile .

cd src\realtime_client
docker build -t agentc-frontend:latest .

cd /d "%~dp0"