#!/bin/bash

# Docker build script for Agent C Realtime Client SDK Demo
# Usage: ./scripts/docker-build.sh [--no-cache] [--dev]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Default values
NO_CACHE=""
DEV_MODE=false
IMAGE_NAME="agentc-realtime-demo"
IMAGE_TAG="latest"

# Parse arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --no-cache)
      NO_CACHE="--no-cache"
      echo -e "${YELLOW}Building without cache...${NC}"
      shift
      ;;
    --dev)
      DEV_MODE=true
      IMAGE_TAG="dev"
      echo -e "${YELLOW}Building for development...${NC}"
      shift
      ;;
    --help)
      echo "Usage: $0 [--no-cache] [--dev]"
      echo "  --no-cache  Build without using cache"
      echo "  --dev       Build development version"
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

echo -e "${GREEN}Starting Docker build for ${IMAGE_NAME}:${IMAGE_TAG}...${NC}"

# Build the image
if [ "$DEV_MODE" = true ]; then
    echo -e "${YELLOW}Building development image...${NC}"
    docker build $NO_CACHE \
        --target builder \
        --build-arg BUILDKIT_INLINE_CACHE=1 \
        -t ${IMAGE_NAME}:${IMAGE_TAG} \
        -t ${IMAGE_NAME}:dev-cache \
        .
else
    echo -e "${YELLOW}Building production image...${NC}"
    docker build $NO_CACHE \
        --build-arg BUILDKIT_INLINE_CACHE=1 \
        -t ${IMAGE_NAME}:${IMAGE_TAG} \
        .
fi

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Docker image built successfully: ${IMAGE_NAME}:${IMAGE_TAG}${NC}"
    echo ""
    echo "To run the container:"
    echo "  docker run -p 3000:3000 ${IMAGE_NAME}:${IMAGE_TAG}"
    echo ""
    echo "Or using docker-compose:"
    if [ "$DEV_MODE" = true ]; then
        echo "  docker-compose -f docker-compose.yml -f docker-compose.dev.yml up"
    else
        echo "  docker-compose up"
    fi
else
    echo -e "${RED}❌ Docker build failed${NC}"
    exit 1
fi