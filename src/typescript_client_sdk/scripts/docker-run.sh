#!/bin/bash

# Docker run script for Agent C Realtime Client SDK Demo
# Usage: ./scripts/docker-run.sh [--dev] [--port PORT] [--detach]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default values
PORT=3000
DEV_MODE=false
DETACH=""
IMAGE_NAME="agentc-realtime-demo"
IMAGE_TAG="latest"
CONTAINER_NAME="agentc-realtime-demo"

# Parse arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --dev)
      DEV_MODE=true
      IMAGE_TAG="dev"
      CONTAINER_NAME="${CONTAINER_NAME}-dev"
      shift
      ;;
    --port)
      PORT="$2"
      shift 2
      ;;
    --detach|-d)
      DETACH="-d"
      shift
      ;;
    --help)
      echo "Usage: $0 [--dev] [--port PORT] [--detach]"
      echo "  --dev       Run development version"
      echo "  --port      Port to expose (default: 3000)"
      echo "  --detach    Run in background"
      echo "  --help      Show this help message"
      exit 0
      ;;
    *)
      echo -e "${RED}Unknown option: $1${NC}"
      echo "Use --help for usage information"
      exit 1
      ;;
  esac
done

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Docker is not installed. Please install Docker first.${NC}"
    exit 1
fi

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo -e "${RED}Docker daemon is not running. Please start Docker.${NC}"
    exit 1
fi

# Check if image exists
if ! docker image inspect ${IMAGE_NAME}:${IMAGE_TAG} &> /dev/null; then
    echo -e "${YELLOW}Image ${IMAGE_NAME}:${IMAGE_TAG} not found. Building it first...${NC}"
    if [ "$DEV_MODE" = true ]; then
        ./scripts/docker-build.sh --dev
    else
        ./scripts/docker-build.sh
    fi
fi

# Check if container with same name is already running
if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo -e "${YELLOW}Container ${CONTAINER_NAME} already exists. Removing it...${NC}"
    docker rm -f ${CONTAINER_NAME}
fi

# Run the container
echo -e "${GREEN}Starting ${IMAGE_NAME}:${IMAGE_TAG} on port ${PORT}...${NC}"

if [ "$DEV_MODE" = true ]; then
    echo -e "${YELLOW}Running in development mode...${NC}"
    docker run $DETACH \
        --name ${CONTAINER_NAME} \
        -p ${PORT}:3000 \
        -e NODE_ENV=development \
        -e NEXT_TELEMETRY_DISABLED=1 \
        --restart unless-stopped \
        ${IMAGE_NAME}:${IMAGE_TAG}
else
    echo -e "${YELLOW}Running in production mode...${NC}"
    docker run $DETACH \
        --name ${CONTAINER_NAME} \
        -p ${PORT}:3000 \
        -e NODE_ENV=production \
        --restart unless-stopped \
        ${IMAGE_NAME}:${IMAGE_TAG}
fi

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Container started successfully${NC}"
    echo ""
    echo -e "${BLUE}Access the application at: http://localhost:${PORT}${NC}"
    echo ""
    if [ -n "$DETACH" ]; then
        echo "Container is running in background."
        echo "To view logs: docker logs -f ${CONTAINER_NAME}"
        echo "To stop: docker stop ${CONTAINER_NAME}"
    else
        echo "Press Ctrl+C to stop the container"
    fi
else
    echo -e "${RED}❌ Failed to start container${NC}"
    exit 1
fi