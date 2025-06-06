# Redis Dependencies for Agent C API

## Overview

This document outlines the Redis-related dependencies required for the Agent C API, particularly for the Redis Session Manager implementation.

## Dependencies

- **redis>=5.0.0**: The main Redis client library for Python
- **redis[hiredis]>=5.0.0**: Redis with hiredis parser for improved performance
- **aioredis>=2.0.0**: Async Redis client library for asyncio applications

## Purpose

These dependencies enable:

1. **Session Storage**: Persistent storage of chat sessions across multiple application instances
2. **Event Streams**: Storage and retrieval of event data for distributed event tracking
3. **Caching**: Improved performance through Redis-based caching

## Version Requirements

- Redis 5.0.0 or higher is required for stream support
- Python 3.12 or higher is required for all async functionality

## Configuration

Redis connection settings are managed through environment variables defined in `env_config.py`:

- `REDIS_HOST`
- `REDIS_PORT`
- `REDIS_DB`
- `REDIS_USERNAME`
- `REDIS_PASSWORD`
- `REDIS_CONNECTION_TIMEOUT`
- `REDIS_SOCKET_TIMEOUT`
- `REDIS_MAX_CONNECTIONS`
- `USE_REDIS_SESSIONS` (feature flag)
- `SESSION_TTL` (24 hours by default)

## Performance Considerations

- The hiredis parser significantly improves parsing performance for large responses
- For production deployments, consider using Redis Sentinel or Redis Cluster for high availability
- Adjust connection pool settings based on expected load

## Testing

For testing without a Redis dependency, consider using fakeredis:

```python
from fakeredis.aioredis import FakeRedis

# Use in place of a real Redis client in tests
fake_redis = FakeRedis()
```

## Further Reading

- [Redis-py Documentation](https://redis-py.readthedocs.io/)
- [Aioredis Documentation](https://aioredis.readthedocs.io/)
- [Redis Streams Introduction](https://redis.io/topics/streams-intro)