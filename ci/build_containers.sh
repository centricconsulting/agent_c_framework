#!/bin/bash
set -e
cd "$(dirname "$0")"/..

docker build -t ghcr.io/centricconsulting/agent_c_python_base:latest -f pythonbase.Dockerfile .
docker build -t ghcr.io/centricconsulting/agentc-api:latest -f api.Dockerfile .
docker build -t ghcr.io/centricconsulting/agentc-client:latest  -f client.Dockerfile src/typescript_client_sdk
