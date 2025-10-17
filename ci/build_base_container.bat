@echo off
SETLOCAL

:: Change to the parent directory containing this script
cd /d "%~dp0"\..
call build_support\windows.env.bat
docker build -t ghcr.io/centricconsulting/agent_c_python_base:latest -f pythonbase.Dockerfile .