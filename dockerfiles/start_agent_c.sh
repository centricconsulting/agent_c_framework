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

# Check if config file exists and set up AI keys
if [ ! -f "$AGENT_C_CONFIG_PATH/agent_c.config" ]; then
    echo "Creating new configuration file..."
    cp agent_c.config.example "$AGENT_C_CONFIG_PATH/agent_c.config"
    
    # Prompt for OpenAI API key
    read -p "Please enter your OpenAI API key: " OPENAI_API_KEY
    if [ -n "$OPENAI_API_KEY" ]; then
        sed -i '' "s/OPENAI_API_KEY=FROM-OPEN-AI/OPENAI_API_KEY=$OPENAI_API_KEY/" "$AGENT_C_CONFIG_PATH/agent_c.config"
    fi
    
    # Prompt for Anthropic API key
    read -p "Please enter your Anthropic API key (press Enter to skip): " ANTHROPIC_API_KEY
    if [ -n "$ANTHROPIC_API_KEY" ]; then
        sed -i '' "s/^#ANTHROPIC_API_KEY=.*/ANTHROPIC_API_KEY=$ANTHROPIC_API_KEY/" "$AGENT_C_CONFIG_PATH/agent_c.config"
    fi
    
    # Prompt for Zep API key
    read -p "Please enter your Zep API key (press Enter to skip if using Zep CE): " ZEP_API_KEY
    if [ -n "$ZEP_API_KEY" ]; then
        sed -i '' "s/ZEP_API_KEY=FROM-ZEP/ZEP_API_KEY=$ZEP_API_KEY/" "$AGENT_C_CONFIG_PATH/agent_c.config"
    else
        # If no Zep API key, prompt for Zep CE configuration
        read -p "Are you using Zep CE locally? (y/n): " USE_ZEP_CE
        if [ "$USE_ZEP_CE" = "y" ]; then
            read -p "Enter your Zep CE key: " ZEP_CE_KEY
            read -p "Enter your Zep CE URL (default: http://localhost:8001): " ZEP_URL
            ZEP_URL=${ZEP_URL:-http://localhost:8001}
            
            # Comment out Zep Cloud settings and uncomment Zep CE settings
            sed -i '' 's/^ZEP_API_KEY=.*/#&/' "$AGENT_C_CONFIG_PATH/agent_c.config"
            sed -i '' 's/^#ZEP_CE_KEY=.*/ZEP_CE_KEY='"$ZEP_CE_KEY"'/' "$AGENT_C_CONFIG_PATH/agent_c.config"
            sed -i '' 's/^#ZEP_URL=.*/ZEP_URL='"$ZEP_URL"'/' "$AGENT_C_CONFIG_PATH/agent_c.config"
        fi
    fi
    
    # Prompt for user ID
    read -p "Enter your user ID (default: Taytay): " USER_ID
    USER_ID=${USER_ID:-Taytay}
    sed -i '' "s/CLI_CHAT_USER_ID=Taytay/CLI_CHAT_USER_ID=$USER_ID/" "$AGENT_C_CONFIG_PATH/agent_c.config"
    
    echo "Configuration file has been created at $AGENT_C_CONFIG_PATH/agent_c.config"
    echo "Please review and edit the configuration file if needed."
    echo ""
    read -p "Press Enter to continue..."
fi

# Set platform for Docker based on architecture
export DOCKER_DEFAULT_PLATFORM="linux/amd64"  # Default to amd64
if [ "$(uname -m)" = "arm64" ]; then
    export DOCKER_DEFAULT_PLATFORM="linux/arm64"
fi

# Run in detached mode (background) with platform support:
COMPOSE_DOCKER_CLI_BUILD=1 DOCKER_BUILDKIT=1 docker-compose -f docker-compose.yml -p agent_c build
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