# Redis Environment Variables Configuration

This document provides comprehensive documentation for all Redis-related environment variables in the Agent C API, including the new resilient mode configuration options.

## Table of Contents

1. [Basic Redis Connection](#basic-redis-connection)
2. [Resilient Mode Configuration](#resilient-mode-configuration)
3. [Health Monitoring](#health-monitoring)
4. [Performance Configuration](#performance-configuration)
5. [Stream Configuration](#stream-configuration)
6. [Mode Switching Configuration](#mode-switching-configuration)
7. [Configuration Validation](#configuration-validation)
8. [Examples](#examples)

## Basic Redis Connection

### Connection Settings

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `REDIS_HOST` | string | `localhost` | Redis server hostname or IP address |
| `REDIS_PORT` | integer | `6379` | Redis server port number |
| `REDIS_DB` | integer | `0` | Redis database number (0-15) |
| `REDIS_USERNAME` | string | `None` | Redis username for authentication (Redis 6+) |
| `REDIS_PASSWORD` | string | `None` | Redis password for authentication |

### Connection Pooling and Timeouts

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `REDIS_CONNECTION_TIMEOUT` | integer | `5` | Connection timeout in seconds |
| `REDIS_SOCKET_TIMEOUT` | integer | `5` | Socket timeout in seconds |
| `REDIS_MAX_CONNECTIONS` | integer | `50` | Maximum connections in the pool |

## Resilient Mode Configuration

### Core Resilient Mode Settings

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `REDIS_ENABLE_RESILIENT_MODE` | boolean | `true` | Enable/disable resilient mode features |
| `REDIS_OPERATION_MODE` | string | `HYBRID` | Primary operation mode: `REDIS_ONLY`, `HYBRID`, `ASYNC_ONLY` |
| `REDIS_AUTO_MODE_SWITCHING` | boolean | `true` | Enable automatic mode switching based on health |
| `REDIS_FALLBACK_MODE` | string | `ASYNC_ONLY` | Fallback mode when Redis is unavailable |

#### Operation Modes Explained

- **`REDIS_ONLY`**: Use Redis Streams exclusively. System fails if Redis is unavailable.
- **`HYBRID`**: Prefer Redis Streams, fallback to async queue when Redis fails.
- **`ASYNC_ONLY`**: Use async queue exclusively, bypass Redis entirely.

## Health Monitoring

| Variable | Type | Default | Range | Description |
|----------|------|---------|-------|-------------|
| `REDIS_HEALTH_CHECK_INTERVAL` | integer | `30` | 5-300 | Health check interval in seconds |
| `REDIS_HEALTH_CHECK_TIMEOUT` | integer | `5` | 1-30 | Health check timeout in seconds |
| `REDIS_FAILURE_THRESHOLD` | integer | `3` | 1-10 | Failures before mode switch |
| `REDIS_RECOVERY_THRESHOLD` | integer | `5` | 1-20 | Successes before recovery |

### Health Check Requirements

- `REDIS_HEALTH_CHECK_TIMEOUT` must be less than `REDIS_HEALTH_CHECK_INTERVAL`
- Health checks test Redis connectivity and basic operations
- Consecutive failures trigger automatic mode switching (if enabled)

## Performance Configuration

| Variable | Type | Default | Range | Description |
|----------|------|---------|-------|-------------|
| `REDIS_CIRCUIT_BREAKER_TIMEOUT` | integer | `60` | 10-600 | Circuit breaker timeout in seconds |
| `REDIS_MAX_RETRIES` | integer | `3` | 0-10 | Maximum retry attempts |
| `REDIS_RETRY_BACKOFF` | string | `exponential` | - | Retry strategy: `exponential` or `linear` |
| `REDIS_CONNECTION_POOL_SIZE` | integer | `20` | 1-100 | Redis connection pool size |
| `REDIS_PERFORMANCE_MONITORING` | boolean | `true` | - | Enable performance metrics collection |

### Circuit Breaker Behavior

- **Closed**: Normal operation, all requests allowed
- **Open**: Redis failures detected, requests blocked for timeout period
- **Half-Open**: Testing recovery, limited requests allowed

## Stream Configuration

| Variable | Type | Default | Range | Description |
|----------|------|---------|-------|-------------|
| `REDIS_STREAM_MAX_LEN` | integer | `10000` | 100-1000000 | Maximum events per stream |
| `REDIS_CONSUMER_GROUP_PREFIX` | string | `agent_c` | - | Prefix for consumer group names |
| `REDIS_EVENT_TTL` | integer | `604800` | 3600+ | Event TTL in seconds (7 days default) |
| `STREAM_PREFIX` | string | `agent_c:stream:` | - | Stream name prefix |

### Stream Management

- Streams are automatically trimmed when they exceed `REDIS_STREAM_MAX_LEN`
- Events older than `REDIS_EVENT_TTL` are eligible for cleanup
- Consumer groups use `REDIS_CONSUMER_GROUP_PREFIX` for naming

## Mode Switching Configuration

| Variable | Type | Default | Range | Description |
|----------|------|---------|-------|-------------|
| `REDIS_MODE_SWITCH_DELAY` | integer | `30` | 5-300 | Minimum delay between mode switches |
| `REDIS_MAX_TRANSITION_RETRIES` | integer | `3` | 0-10 | Maximum retries for mode transitions |
| `REDIS_ENABLE_AUTO_RECOVERY` | boolean | `true` | - | Enable automatic recovery from failures |

### Mode Switching Behavior

- Mode switches are rate-limited by `REDIS_MODE_SWITCH_DELAY`
- Failed transitions are retried up to `REDIS_MAX_TRANSITION_RETRIES`
- Auto-recovery attempts to restore preferred mode when Redis recovers

## Configuration Validation

The system automatically validates all configuration values:

### Validation Rules

1. **Type Validation**: All values must match expected types
2. **Range Validation**: Numeric values must be within recommended ranges
3. **Consistency Validation**: Related settings must be compatible
4. **Mode Validation**: Operation modes must be valid enum values

### Common Validation Errors

```bash
# Invalid operation mode
REDIS_OPERATION_MODE=INVALID_MODE
# Error: Invalid REDIS_OPERATION_MODE 'INVALID_MODE'. Valid options: redis_only, hybrid, async_only

# Timeout relationship error
REDIS_HEALTH_CHECK_TIMEOUT=10
REDIS_HEALTH_CHECK_INTERVAL=5
# Error: REDIS_HEALTH_CHECK_TIMEOUT must be less than REDIS_HEALTH_CHECK_INTERVAL

# Negative threshold
REDIS_FAILURE_THRESHOLD=-1
# Error: REDIS_FAILURE_THRESHOLD must be an integer >= 1
```

## Examples

### Development Environment (.env)

```bash
# Basic Redis connection
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_DB=0

# Resilient mode for development
REDIS_ENABLE_RESILIENT_MODE=true
REDIS_OPERATION_MODE=HYBRID
REDIS_AUTO_MODE_SWITCHING=true
REDIS_FALLBACK_MODE=ASYNC_ONLY

# Conservative health monitoring
REDIS_HEALTH_CHECK_INTERVAL=30
REDIS_HEALTH_CHECK_TIMEOUT=5
REDIS_FAILURE_THRESHOLD=3
REDIS_RECOVERY_THRESHOLD=5

# Performance settings
REDIS_CIRCUIT_BREAKER_TIMEOUT=60
REDIS_MAX_RETRIES=3
REDIS_RETRY_BACKOFF=exponential
REDIS_CONNECTION_POOL_SIZE=10

# Stream configuration
REDIS_STREAM_MAX_LEN=5000
REDIS_EVENT_TTL=86400  # 1 day
```

### Production Environment

```bash
# Production Redis cluster
REDIS_HOST=redis-cluster.production.local
REDIS_PORT=6379
REDIS_DB=0
REDIS_USERNAME=agent_c_api
REDIS_PASSWORD=secure_password_here

# Production resilient mode
REDIS_ENABLE_RESILIENT_MODE=true
REDIS_OPERATION_MODE=HYBRID
REDIS_AUTO_MODE_SWITCHING=true
REDIS_FALLBACK_MODE=ASYNC_ONLY

# Aggressive health monitoring
REDIS_HEALTH_CHECK_INTERVAL=15
REDIS_HEALTH_CHECK_TIMEOUT=3
REDIS_FAILURE_THRESHOLD=2
REDIS_RECOVERY_THRESHOLD=5

# Production performance
REDIS_CIRCUIT_BREAKER_TIMEOUT=30
REDIS_MAX_RETRIES=5
REDIS_RETRY_BACKOFF=exponential
REDIS_CONNECTION_POOL_SIZE=50

# Large-scale streams
REDIS_STREAM_MAX_LEN=50000
REDIS_EVENT_TTL=604800  # 7 days

# Performance monitoring
REDIS_PERFORMANCE_MONITORING=true
```

### High-Availability Environment

```bash
# Redis with high availability
REDIS_HOST=redis-ha.cluster.local
REDIS_PORT=6379
REDIS_DB=0
REDIS_USERNAME=agent_c_ha
REDIS_PASSWORD=ha_secure_password

# Conservative resilient mode
REDIS_ENABLE_RESILIENT_MODE=true
REDIS_OPERATION_MODE=REDIS_ONLY
REDIS_AUTO_MODE_SWITCHING=false
REDIS_FALLBACK_MODE=HYBRID

# Frequent health checks
REDIS_HEALTH_CHECK_INTERVAL=10
REDIS_HEALTH_CHECK_TIMEOUT=2
REDIS_FAILURE_THRESHOLD=1
REDIS_RECOVERY_THRESHOLD=3

# Fast recovery settings
REDIS_CIRCUIT_BREAKER_TIMEOUT=15
REDIS_MAX_RETRIES=7
REDIS_RETRY_BACKOFF=exponential
REDIS_CONNECTION_POOL_SIZE=100

# High-throughput streams
REDIS_STREAM_MAX_LEN=100000
REDIS_EVENT_TTL=1209600  # 14 days

# Mode switching settings
REDIS_MODE_SWITCH_DELAY=10
REDIS_MAX_TRANSITION_RETRIES=5
REDIS_ENABLE_AUTO_RECOVERY=true
```

### Testing Environment

```bash
# Local Redis for testing
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_DB=1  # Use different DB for tests

# Test-friendly resilient mode
REDIS_ENABLE_RESILIENT_MODE=true
REDIS_OPERATION_MODE=HYBRID
REDIS_AUTO_MODE_SWITCHING=true
REDIS_FALLBACK_MODE=ASYNC_ONLY

# Fast health checks for testing
REDIS_HEALTH_CHECK_INTERVAL=5
REDIS_HEALTH_CHECK_TIMEOUT=1
REDIS_FAILURE_THRESHOLD=1
REDIS_RECOVERY_THRESHOLD=2

# Quick recovery for tests
REDIS_CIRCUIT_BREAKER_TIMEOUT=10
REDIS_MAX_RETRIES=2
REDIS_RETRY_BACKOFF=linear
REDIS_CONNECTION_POOL_SIZE=5

# Small streams for testing
REDIS_STREAM_MAX_LEN=100
REDIS_EVENT_TTL=3600  # 1 hour

# Fast mode switching for tests
REDIS_MODE_SWITCH_DELAY=5
REDIS_MAX_TRANSITION_RETRIES=2
```

## Configuration Best Practices

### Security

1. **Never commit passwords**: Use environment-specific `.env` files
2. **Use Redis AUTH**: Always set `REDIS_USERNAME` and `REDIS_PASSWORD` in production
3. **Network security**: Ensure Redis is not exposed to public internet

### Performance

1. **Connection pooling**: Set `REDIS_CONNECTION_POOL_SIZE` based on expected load
2. **Health check frequency**: Balance monitoring responsiveness with overhead
3. **Stream management**: Configure `REDIS_STREAM_MAX_LEN` based on memory constraints

### Reliability

1. **Use HYBRID mode**: Provides best balance of performance and reliability
2. **Enable auto-switching**: Allows automatic recovery from Redis failures
3. **Conservative thresholds**: Avoid false positives in failure detection

### Monitoring

1. **Enable performance monitoring**: Set `REDIS_PERFORMANCE_MONITORING=true`
2. **Log configuration**: Review configuration summary at startup
3. **Validate settings**: Use configuration validation endpoints

## Troubleshooting

### Common Issues

1. **Configuration validation errors**: Check environment variable types and ranges
2. **Mode switching loops**: Increase `REDIS_MODE_SWITCH_DELAY` or adjust thresholds
3. **Connection failures**: Verify Redis server accessibility and authentication
4. **Performance issues**: Adjust connection pool size and timeout values

### Debug Commands

```python
# Check configuration validation
from agent_c_api.config.redis_config import RedisConfig
validation = RedisConfig.validate_configuration()
print(validation)

# Get configuration summary
summary = RedisConfig.get_configuration_summary()
print(summary)

# Test Redis connectivity
status = await RedisConfig.validate_connection()
print(status)
```