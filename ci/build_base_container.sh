#!/bin/bash
set -e
cd "$(dirname "$0")"/..
docker build -t ghcr.io/centricconsulting/agent_c_python_base:latest -f pythonbase.Dockerfile .
