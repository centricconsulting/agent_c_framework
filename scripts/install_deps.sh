#!/usr/bin/env bash
set -e
source .venv/bin/activate
# Install the requirements
echo "Installing agent_c_packages"
python -m pip install --upgrade pip
cd src
pip install ace_proto/ts_tool-0.1.0-py3-none-any.whl
pip install -e agent_c_core
pip install -e agent_c_tools
playwright install
pip install -e agent_c_api[dev]


echo "Performing a clean build of the Typescript client SDK and demo..."
cd  typescript_client_sdk
scripts/rebuild.sh

echo "Initial setup completed successfully."
echo "Remember to activate the virtual environment with 'source .venv/bin/activate' before you start working."

# Exit from the script without deactivating (since it's a new shell instance)
