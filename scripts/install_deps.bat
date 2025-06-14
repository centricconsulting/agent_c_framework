@echo off
SETLOCAL

:: Store the starting directory
pushd %CD%

:: Install the requirements
echo Installing dependencies.
python -m pip install --upgrade pip

cd src
::pip install ace_proto/ts_tool-0.1.0-py3-none-any.whl
pip install -e agent_c_core
pip install -e agent_c_tools
playwright install
pip install -e agent_c_api_ui/agent_c_api[dev]
pip install -e ..\test\unit\agent_c_tools


if errorlevel 1 (
    echo Failed to install the required python packages.
    popd
    exit /b 1
)

:: Install NPM packages
echo Installing NPM dependencies...
cd agent_c_api_ui\agent_c_react_client
npm install

if errorlevel 1 (
    echo Failed to install the NPM packages.
    popd
    exit /b 1
)

echo Due to bug we need to install, remove package-lock.json and node_modules, then install again. https://github.com/npm/cli/issues/4828
echo Removing node_modules directory...
if exist node_modules (
  rmdir /s /q node_modules
) else (
  echo node_modules directory doesn't exist, skipping...
)

echo Removing package-lock.json...
if exist package-lock.json (
  del package-lock.json
) else (
  echo package-lock.json doesn't exist, skipping...
)

npm install
echo ********
if errorlevel 1 (
    echo Failed to install the NPM packages.
    popd
    exit /b 1
)

:: Return to the original directory
popd

ENDLOCAL
