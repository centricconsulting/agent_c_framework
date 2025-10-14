#!/bin/bash
set -e
cd "$(dirname "$0")"/..
docker build -t ghcr.io/centricconsulting/agentc-api:latest -f api.Dockerfile .
