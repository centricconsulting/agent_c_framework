@echo off
SETLOCAL

cd /d "%~dp0"\..\src\typescript_client_sdk
pnpm dev
