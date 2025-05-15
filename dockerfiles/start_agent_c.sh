#!/bin/bash
set -e

# Go to the directory containing this script
cd "$(dirname "$0")"

# Determine the project root (parent of the dockerfiles directory)
PROJECT_ROOT="$(cd .. && pwd)"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Error: Docker is not running. Please start Docker and try again."
    exit 1
fi

# Set environment variables for agent_c config and directories
export AGENT_C_CONFIG_PATH=$HOME/.agent_c
export AGENT_C_IMAGES_PATH=$HOME/.agent_c/images
export AGENT_C_PERSONAS_PATH=$HOME/.agent_c/personas
export AGENT_C_LOGS_PATH=$HOME/.agent_c/personas

# Add mappings for workspace folders (Documents, Desktop, and Downloads)
export DOCUMENTS_WORKSPACE=$HOME/Documents
export DESKTOP_WORKSPACE=$HOME/Desktop
export DOWNLOADS_WORKSPACE=$HOME/Downloads

# Set PROJECT_WORKSPACE_PATH to the project root directory
export PROJECT_WORKSPACE_PATH=$PROJECT_ROOT

# Create directories if they don't exist
mkdir -p "$AGENT_C_CONFIG_PATH"
mkdir -p "$AGENT_C_IMAGES_PATH"
mkdir -p "$AGENT_C_PERSONAS_PATH"
mkdir -p "$AGENT_C_LOGS_PATH"

# Check if config file exists
if [ ! -f "$AGENT_C_CONFIG_PATH/agent_c.config" ]; then
    cp agent_c.config.example "$AGENT_C_CONFIG_PATH/agent_c.config"
    echo "** Warning**:  Configuration file not found at $AGENT_C_CONFIG_PATH/agent_c.config"
    echo "An example configuration file is being copied to $AGENT_C_CONFIG_PATH"
    echo ""
    echo "Please edit the configuration file with a text editor and rerun this script."
    echo ""
    read -p "Press Enter to exit..."
    exit 1
fi

# Set platform flag for ARM processors
PLATFORM_FLAG=""
export DOCKER_PLATFORM="linux/amd64"  # Default to amd64
if [ "$(uname -m)" = "arm64" ]; then
    PLATFORM_FLAG="--platform linux/arm64"
    export DOCKER_PLATFORM="linux/arm64"
fi

# Run in detached mode (background) with platform support:
COMPOSE_DOCKER_CLI_BUILD=1 DOCKER_BUILDKIT=1 docker-compose -f docker-compose.yml -p agent_c build $PLATFORM_FLAG
docker-compose -f docker-compose.yml -p agent_c up -d

# Wait for the containers to be healthy
echo "Waiting for services to start..."
attempt=1
max_attempts=30
until curl -s http://localhost:8000/health > /dev/null || [ $attempt -eq $max_attempts ]; do
    echo "Waiting for API to be ready... (attempt $attempt/$max_attempts)"
    sleep 2
    attempt=$((attempt + 1))
done

if [ $attempt -eq $max_attempts ]; then
    echo "Error: Services failed to start properly. Check docker logs for more information:"
    echo "docker-compose -p agent_c logs"
    exit 1
fi

echo "Services are ready!"

# Open the browser to view Agent C
bash ./view_agent_c.sh