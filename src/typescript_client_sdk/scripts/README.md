# Docker Scripts

This directory contains helper scripts for building and running the Agent C Realtime Client SDK demo in Docker.

## Available Scripts

### docker-build.sh
Builds the Docker image for the demo application.

```bash
# Build production image
./scripts/docker-build.sh

# Build without cache
./scripts/docker-build.sh --no-cache

# Build development image
./scripts/docker-build.sh --dev
```

### docker-run.sh
Runs the Docker container with the demo application.

```bash
# Run in foreground (default port 3000)
./scripts/docker-run.sh

# Run on different port
./scripts/docker-run.sh --port 8080

# Run in background (detached)
./scripts/docker-run.sh --detach

# Run development version
./scripts/docker-run.sh --dev
```

## Making Scripts Executable

If the scripts are not executable, run:

```bash
chmod +x scripts/docker-build.sh
chmod +x scripts/docker-run.sh
```

## Quick Start

1. Build and run the production image:
```bash
./scripts/docker-build.sh && ./scripts/docker-run.sh
```

2. Build and run the development image:
```bash
./scripts/docker-build.sh --dev && ./scripts/docker-run.sh --dev
```

3. Run in background on port 8080:
```bash
./scripts/docker-run.sh --port 8080 --detach
```