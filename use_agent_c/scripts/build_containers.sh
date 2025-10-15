#!/bin/bash
set -e

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Error: Docker is not running. Please start Docker and try again."
    exit 1


cd "$(dirname "$0")"

cd ..\..
docker build -t agentc-api:latest -f dockerfiles\api.Dockerfile .

cd src/realtime_client
docker build -t agentc-frontend:latest .

cd "$(dirname "$0")"
