import asyncio
import logging
from typing import Optional, Dict, Any
from redis import asyncio as aioredis
from redis.exceptions import ConnectionError, TimeoutError, RedisError
from .env_config import settings

logger = logging.getLogger(__name__)

class RedisConfig:
    _redis_client: Optional[aioredis.Redis] = None
    
    @classmethod
    async def get_redis_client(cls) -> aioredis.Redis:
        """Get Redis client with connection pooling and proper error handling."""
        if cls._redis_client is None:
            try:
                cls._redis_client = aioredis.Redis(
                    host=settings.REDIS_HOST,
                    port=settings.REDIS_PORT,
                    db=settings.REDIS_DB,
                    username=settings.REDIS_USERNAME,
                    password=settings.REDIS_PASSWORD,
                    encoding="utf8",
                    decode_responses=True,
                    # Connection pooling settings
                    max_connections=20,
                    retry_on_error=[ConnectionError, TimeoutError, RedisError],
                    # Timeout settings
                    socket_connect_timeout=5,
                    socket_timeout=5,
                    socket_keepalive=True,
                    socket_keepalive_options={},
                    health_check_interval=30
                )
                logger.info(f"Redis client created for {settings.REDIS_HOST}:{settings.REDIS_PORT}")
            except Exception as e:
                logger.error(f"Failed to create Redis client: {e}")
                raise
        return cls._redis_client
    
    @classmethod
    async def validate_connection(cls) -> Dict[str, Any]:
        """Validate Redis connection and return detailed status information."""
        status = {
            "connected": False,
            "server_info": None,
            "error": None,
            "host": settings.REDIS_HOST,
            "port": settings.REDIS_PORT,
            "db": settings.REDIS_DB
        }
        
        try:
            redis = await cls.get_redis_client()
            # Test connection with ping
            await redis.ping()
            status["connected"] = True
            
            # Get server information
            try:
                info = await redis.info()
                status["server_info"] = {
                    "redis_version": info.get("redis_version"),
                    "redis_mode": info.get("redis_mode"),
                    "used_memory_human": info.get("used_memory_human"),
                    "connected_clients": info.get("connected_clients"),
                    "uptime_in_seconds": info.get("uptime_in_seconds")
                }
            except Exception as e:
                logger.warning(f"Could not get Redis server info: {e}")
                
            logger.info(f"Redis connection validated successfully at {settings.REDIS_HOST}:{settings.REDIS_PORT}")
            
        except (ConnectionError, ConnectionRefusedError) as e:
            status["error"] = f"Connection refused: {e}"
            logger.error(f"Redis connection failed: {e}")
        except TimeoutError as e:
            status["error"] = f"Connection timeout: {e}"
            logger.error(f"Redis connection timeout: {e}")
        except Exception as e:
            status["error"] = f"Unexpected error: {e}"
            logger.error(f"Unexpected Redis error: {e}")
            
        return status
    
    @classmethod
    async def close_client(cls) -> None:
        """Close Redis client connection properly."""
        if cls._redis_client is not None:
            try:
                await cls._redis_client.aclose()
                logger.info("Redis client connection closed")
            except Exception as e:
                logger.error(f"Error closing Redis client: {e}")
            finally:
                cls._redis_client = None

