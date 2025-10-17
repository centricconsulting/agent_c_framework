@echo off
SETLOCAL

:: Check for Python and print the version
python --version >NUL 2>&1
if errorlevel 9009 (
    echo Python is not installed or not in the PATH.
    exit /b 1
)

:: Check if the virtual environment already exists
if exist .venv (
    echo Virtual environment already exists. Skipping creation.
) else (
    :: Create a virtual environment
    echo Creating virtual environment.
    python -m venv .venv
    if errorlevel 1 (
        echo Failed to create a virtual environment.
        exit /b 1
    )
)

if exist .env (
    echo Config file already exists. Skipping creation.
) else (
    echo Copying example.env to .env ...
    copy example.env .env
    cls
    echo ******************************************************************************
    echo *
    echo *  Please edit the .env file to configure your API keys.
    echo *  When notepad closes initial setup will continue.
    notepad .env
)

if exist .local_workspaces.json (
    echo Local workspaces config file already exists. Skipping creation.
) else (
    echo Creating local workspaces config file from example ...
    copy local_workspaces.example.json .local_workspaces.json
)

if exist src\typescript_client_sdk\packages\demo\.env.local (
    echo Demo local config file already exists. Skipping creation.
) else (
    echo Copying demo example.env to src\typescript_client_sdk\packages\demo\.env.local ...
    copy src\typescript_client_sdk\packages\demo\.env.example src\typescript_client_sdk\packages\demo\.env.local
)

:: Activate the virtual environment
echo Activating virtual environment.
CALL .venv\scripts\activate.bat
if errorlevel 1 (
    echo Failed to activate the virtual environment.
    exit /b 1
)

:: Upgrade pip to latest version
echo Upgrading Pip to the latest version.
python -m pip install --upgrade pip
pip install tomli

echo "Installing pnpm version 9 and lerna globally..."
CALL npm install -g pnpm@9 lerna

if errorlevel 1 (
    echo Failed to install pnpm and lerna.
    exit /b 1
)

echo "Fetching initial client dependencies with pnpm..."
cd src\typescript_client_sdk
pnpm install

if errorlevel 1 (
    echo Failed to install client dependencies with pnpm.
    exit /b 1
)

cd ..\..

:: Install the requirements
echo Installing dependencies.
call scripts/install_deps.bat

if errorlevel 1 (
    echo Failed to install the required packages.
    exit /b 1
)


echo Initial setup completed successfully.
echo Remember to activate the virtual environment with '.venv\scripts\activate.bat' before you start working.
ENDLOCAL
