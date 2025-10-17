# Environment Configuration Guide

## Overview

This guide covers the environment configuration for the Agent C API, including Redis setup, deployment scenarios, and best practices for different environments.

## Configuration Files

### Primary Configuration
- **`src/agent_c_api/config/env_config.py`** - Main configuration settings
- **`.env`** (project root) - Environment-specific overrides

### Configuration Loading
The application uses Pydantic Settings with the following precedence:
1. Environment variables
2. `.env` file (if present)
3. Default values in `env_config.py`

## Redis Configuration

### Required Settings

```bash
# Redis Connection (Required)
REDIS_HOST=localhost          # Redis server hostname/IP
REDIS_PORT=6379              # Redis server port
REDIS_DB=0                   # Redis database number (0-15)

# Optional Authentication
REDIS_USERNAME=              # Redis username (Redis 6+)
REDIS_PASSWORD=              # Redis password
```

### Connection Pool Settings

```bash
# Connection Timeouts
REDIS_CONNECTION_TIMEOUT=5   # Connection timeout in seconds
REDIS_SOCKET_TIMEOUT=5       # Socket timeout in seconds
REDIS_MAX_CONNECTIONS=50     # Maximum connections in pool
```

### Session Management

```bash
# Session Configuration
SESSION_TTL=86400            # Session TTL in seconds (24 hours)
SESSION_CLEANUP_INTERVAL=3600 # Cleanup interval in seconds (1 hour)
SESSION_CLEANUP_BATCH_SIZE=100 # Cleanup batch size
USE_REDIS_SESSIONS=true      # Enable Redis-based sessions
```

## Deployment Scenarios

### Development Environment

**Local Redis with Docker:**
```bash
# Start Redis container
docker run -d --name redis-dev -p 6379:6379 redis:7-alpine

# Environment variables
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_DB=0
```

**Environment file (`.env`):**
```bash
# Development Configuration
AGENT_C_API_HOST=0.0.0.0
AGENT_C_API_PORT=8000
RELOAD=true

# Redis Configuration
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_DB=0

# Session Settings
SESSION_TTL=86400
USE_REDIS_SESSIONS=true

# Feature Flags
PROFILING_ENABLED=false
```

### Staging Environment

**Managed Redis Service:**
```bash
# Environment variables for staging
REDIS_HOST=staging-redis.example.com
REDIS_PORT=6379
REDIS_DB=0
REDIS_PASSWORD=staging_password
REDIS_CONNECTION_TIMEOUT=10
REDIS_MAX_CONNECTIONS=25
```

**Docker Compose Example:**
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8000:8000"
    environment:
      - REDIS_HOST=redis
      - REDIS_PORT=6379
      - REDIS_DB=0
    depends_on:
      - redis
    
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: redis-server --appendonly yes

volumes:
  redis_data:
```

### Production Environment

**Cloud Redis Service (AWS ElastiCache, Azure Cache, etc.):**
```bash
# Production Redis Configuration
REDIS_HOST=prod-redis-cluster.cache.amazonaws.com
REDIS_PORT=6379
REDIS_DB=0
REDIS_PASSWORD=${REDIS_PASSWORD}  # From secrets management
REDIS_CONNECTION_TIMEOUT=5
REDIS_SOCKET_TIMEOUT=5
REDIS_MAX_CONNECTIONS=100

# Production Session Settings
SESSION_TTL=86400
SESSION_CLEANUP_INTERVAL=1800  # 30 minutes
SESSION_CLEANUP_BATCH_SIZE=500
USE_REDIS_SESSIONS=true

# Production API Settings
AGENT_C_API_HOST=0.0.0.0
AGENT_C_API_PORT=8000
RELOAD=false
PROFILING_ENABLED=false
```

**Kubernetes Deployment:**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: agent-c-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: agent-c-api
  template:
    metadata:
      labels:
        app: agent-c-api
    spec:
      containers:
      - name: api
        image: agent-c-api:latest
        ports:
        - containerPort: 8000
        env:
        - name: REDIS_HOST
          value: "redis-service"
        - name: REDIS_PORT
          value: "6379"
        - name: REDIS_PASSWORD
          valueFrom:
            secretKeyRef:
              name: redis-secret
              key: password
        livenessProbe:
          httpGet:
            path: /api/v2/health/live
            port: 8000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /api/v2/health/ready
            port: 8000
          initialDelaySeconds: 5
          periodSeconds: 5
```

## Redis Setup Instructions

### Docker (Recommended for Development)

```bash
# Basic Redis
docker run -d --name redis -p 6379:6379 redis:7-alpine

# Redis with persistence
docker run -d --name redis -p 6379:6379 -v redis_data:/data redis:7-alpine redis-server --appendonly yes

# Redis with password
docker run -d --name redis -p 6379:6379 redis:7-alpine redis-server --requirepass mypassword
```

### Native Installation

**Ubuntu/Debian:**
```bash
sudo apt update
sudo apt install redis-server
sudo systemctl enable redis-server
sudo systemctl start redis-server
```

**CentOS/RHEL:**
```bash
sudo yum install epel-release
sudo yum install redis
sudo systemctl enable redis
sudo systemctl start redis
```

**macOS:**
```bash
brew install redis
brew services start redis
```

### Cloud Services

**AWS ElastiCache:**
- Use Redis engine version 6.x or 7.x
- Enable encryption in transit and at rest
- Configure appropriate instance size based on memory requirements
- Use VPC security groups for network access control

**Azure Cache for Redis:**
- Choose appropriate pricing tier (Basic, Standard, Premium)
- Enable SSL/TLS encryption
- Configure firewall rules for access control

**Google Cloud Memorystore:**
- Select Redis version 6.x or 7.x
- Configure authorized networks
- Enable AUTH for security

## Health Monitoring

### Health Check Endpoints

The API provides comprehensive health monitoring:

```bash
# Basic health check
curl http://localhost:8000/api/v2/health

# Kubernetes readiness probe
curl http://localhost:8000/api/v2/health/ready

# Kubernetes liveness probe
curl http://localhost:8000/api/v2/health/live

# Detailed Redis diagnostics
curl http://localhost:8000/api/v2/debug/health/redis
```

### Monitoring Integration

**Prometheus Metrics:**
- Monitor health endpoint response times
- Track Redis connection status
- Alert on health check failures

**Grafana Dashboard:**
- Visualize Redis performance metrics
- Monitor connection pool usage
- Track session management statistics

## Troubleshooting

### Common Issues

**Connection Refused:**
```bash
# Check if Redis is running
redis-cli ping

# Check Redis logs
docker logs redis  # For Docker
sudo journalctl -u redis  # For systemd
```

**Authentication Errors:**
```bash
# Test Redis authentication
redis-cli -h localhost -p 6379 -a yourpassword ping
```

**High Latency:**
- Check network connectivity between API and Redis
- Monitor Redis memory usage and eviction policies
- Review connection pool settings

### Configuration Validation

The API validates Redis configuration on startup:
- Connection connectivity test
- Authentication verification
- Performance baseline measurement

Check startup logs for detailed Redis status information.

## Security Best Practices

### Network Security
- Use VPC/private networks for Redis access
- Configure firewall rules to restrict Redis port access
- Enable TLS/SSL encryption for Redis connections

### Authentication
- Always use Redis AUTH in production
- Use strong, randomly generated passwords
- Rotate Redis passwords regularly

### Data Protection
- Enable Redis persistence (AOF or RDB) for data durability
- Configure appropriate backup and recovery procedures
- Consider Redis encryption at rest for sensitive data

## Performance Optimization

### Connection Pooling
- Tune `REDIS_MAX_CONNECTIONS` based on expected load
- Monitor connection pool utilization
- Adjust timeouts based on network latency

### Session Management
- Optimize `SESSION_TTL` based on user behavior
- Tune cleanup intervals for performance vs. resource usage
- Monitor session creation and cleanup rates

### Redis Configuration
- Configure appropriate Redis memory policies
- Tune Redis persistence settings for your use case
- Monitor Redis memory usage and fragmentation

## Migration from Embedded Redis

If migrating from an older version with embedded Redis:

1. **Set up external Redis** using one of the methods above
2. **Update configuration** to point to external Redis
3. **Remove deprecated settings** (they are ignored but can be cleaned up)
4. **Test connectivity** using health check endpoints
5. **Migrate data** if needed (sessions will be recreated automatically)

The deprecated settings are:
- `REDIS_DATA_DIR` - No longer used
- `REDIS_STARTUP_TIMEOUT` - No longer used  
- `MANAGE_REDIS_LIFECYCLE` - Always False (external Redis only)

These settings are kept for backward compatibility but are completely ignored by the application.