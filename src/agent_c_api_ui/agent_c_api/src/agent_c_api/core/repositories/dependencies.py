"""
Repository dependency injection functions.

This module provides FastAPI dependencies for repository classes,
separated from the main dependencies module to avoid circular imports.
"""

from fastapi import Depends
from typing import Optional
from redis import asyncio as aioredis

from agent_c_api.api.dependencies import get_redis_client, get_redis_client_optional
from .session_repository import SessionRepository


async def get_session_repository(
    redis_client: aioredis.Redis = Depends(get_redis_client)
) -> SessionRepository:
    """
    FastAPI dependency that provides a SessionRepository.
    
    This dependency creates a SessionRepository with a Redis client,
    failing fast if Redis is not available.
    
    Args:
        redis_client: Redis client dependency
        
    Returns:
        SessionRepository instance
    """
    return SessionRepository(redis_client)


async def get_session_repository_optional(
    redis_client: Optional[aioredis.Redis] = Depends(get_redis_client_optional)
) -> Optional[SessionRepository]:
    """
    FastAPI dependency that provides an optional SessionRepository.
    
    This dependency provides graceful degradation when Redis is not available,
    returning None instead of raising an exception.
    
    Args:
        redis_client: Optional Redis client dependency
        
    Returns:
        SessionRepository instance if Redis is available, None otherwise
    """
    if redis_client is None:
        return None
    return SessionRepository(redis_client)