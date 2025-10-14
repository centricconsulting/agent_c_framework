# Docker Setup for Agent C Realtime Client SDK Demo

This Docker setup provides a production-ready containerized deployment of the Agent C Realtime Client SDK demo application.

## Features

- **Multi-stage build** for optimized image size
- **Production-ready** Next.js configuration
- **Layer caching** for faster rebuilds
- **Non-root user** for security
- **Health checks** included
- **Resource limits** configured
- Builds all workspace packages from source

## Prerequisites

- Docker Engine 20.10 or newer
- Docker Compose v2.0 or newer (optional, for docker-compose usage)

## Quick Start

### Using Docker

1. Build the image:
```bash
docker build -t agentc-realtime-demo:latest .
```

2. Run the container:
```bash
docker run -p 3000:3000 agentc-realtime-demo:latest
```

3. Access the application at `http://localhost:3000`

### Using Docker Compose

1. Build and start the service:
```bash
docker-compose up --build
```

2. Access the application at `http://localhost:3000`

3. Stop the service:
```bash
docker-compose down
```

## Development Usage

For development with hot-reload capabilities:

```bash
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

## Environment Variables

Configure the following environment variables as needed:

- `NODE_ENV` - Set to `production` for production builds (default: production)
- `PORT` - Port for the Next.js server (default: 3000)
- `NEXT_PUBLIC_API_URL` - Your Agent C API endpoint (if required)

Add these to your `docker-compose.yml` or pass them with `-e` flag when running docker:

```bash
docker run -p 3000:3000 \
  -e NEXT_PUBLIC_API_URL=https://api.example.com \
  agentc-realtime-demo:latest
```

## Build Architecture

The Dockerfile uses a multi-stage build process:

1. **base** - Sets up Node.js Alpine with pnpm
2. **deps** - Installs all dependencies
3. **builder** - Builds all workspace packages in order:
   - `@agentc/realtime-core`
   - `@agentc/realtime-react` 
   - `@agentc/realtime-ui`
   - `@agentc/demo-app`
4. **runner** - Minimal production image with built assets

## Package Build Order

The monorepo packages are built in dependency order:

```
core (no deps) → react (deps: core) → ui (deps: core, react) → demo (deps: all)
```

## Image Size Optimization

The production image is optimized by:
- Using Alpine Linux base image
- Multi-stage build to exclude build dependencies
- Production-only node_modules
- Excluding test files and development dependencies
- Proper .dockerignore configuration

## Troubleshooting

### Port Already in Use

If port 3000 is already in use, change the port mapping:

```bash
docker run -p 8080:3000 agentc-realtime-demo:latest
```

Or in docker-compose.yml:
```yaml
ports:
  - "8080:3000"
```

### Build Cache Issues

To rebuild without cache:

```bash
docker build --no-cache -t agentc-realtime-demo:latest .
```

### Memory Issues

If the build fails due to memory constraints, increase Docker's memory limit in Docker Desktop settings, or adjust the resource limits in `docker-compose.yml`.

### Permission Issues

The container runs as non-root user (uid: 1001). Ensure any mounted volumes have appropriate permissions.

## Security Considerations

- Container runs as non-root user `nextjs` (uid: 1001)
- No shell or unnecessary packages in production image
- Source maps disabled in production
- Environment variables should be passed securely (use Docker secrets in production)

## Performance Tuning

Adjust resource limits in `docker-compose.yml` based on your needs:

```yaml
deploy:
  resources:
    limits:
      cpus: '2'      # Maximum CPU cores
      memory: 2G     # Maximum RAM
    reservations:
      cpus: '0.5'    # Reserved CPU cores
      memory: 512M   # Reserved RAM
```

## CI/CD Integration

For CI/CD pipelines, use build arguments for caching:

```bash
docker build \
  --build-arg BUILDKIT_INLINE_CACHE=1 \
  --cache-from agentc-realtime-demo:latest \
  -t agentc-realtime-demo:latest .
```

## Support

For issues related to the Docker setup, please check:
1. Docker daemon is running
2. Sufficient disk space and memory
3. Network connectivity for package downloads
4. Correct Node.js version compatibility (v20 LTS)