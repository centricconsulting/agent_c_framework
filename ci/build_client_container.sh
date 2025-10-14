#!/bin/bash
set -e
cd "$(dirname "$0")"/..
docker build -t ghcr.io/centricconsulting/agentc-client:latest  -f client.Dockerfile src/typescript_client_sdk
