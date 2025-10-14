@echo off
SETLOCAL

cd /d "%~dp0"

docker-compose -f docker-compose-client-only.yml -p agent_c_dev_client %*

ENDLOCAL