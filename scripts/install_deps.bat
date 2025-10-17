@echo off
SETLOCAL

:: Store the starting directory
pushd %CD%
CALL .venv\scripts\activate
:: Install the requirements
echo Installing dependencies.
python -m pip install --upgrade pip

cd src
pip install -e agent_c_core

if errorlevel 1 (
    echo Failed to install agent_c_core.
    exit /b 1
)


pip install -e agent_c_tools

if errorlevel 1 (
    echo Failed to install agent_c_tools.
    exit /b 1
)

playwright install
pip install -e agent_c_api[dev]

if errorlevel 1 (
    echo Failed to install agent_c_api.
    exit /b 1
)

pip install -e ..\test\unit\agent_c_tools

echo Installing dependencies for realtime client and performing a clean build...
cd realtime_client
call scripts\rebuild.bat
if errorlevel 1 (
    echo Failed to install the packages.
    popd
    exit /b 1
)

:: Return to the original directory
popd

ENDLOCAL
