#!/usr/bin/env bash
set -e
# Install the requirements
echo "Installing agent_c_packages"
cd src
pip install -e agent_c_core
pip install -e agent_c_tools
pip install -e my_agent_c
pip install -e agent_c_reference_apps
pip install -e agent_c_api_ui/agent_c_api
pip install -e ../test/agent_c_tools

echo "Installing NPM dependencies..."
cd agent_c_api_ui/agent_c_react_client  # Use forward slashes for Linux/macOS
npm install
cd ../../src

echo "Initial setup completed successfully."
echo "Remember to activate the virtual environment with 'source .venv/bin/activate' before you start working."

# Exit from the script without deactivating (since it's a new shell instance)
