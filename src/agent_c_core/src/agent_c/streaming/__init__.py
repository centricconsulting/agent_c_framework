"""
Redis Streams Integration Module for Agent C

This module provides Redis Streams functionality for distributed event processing
in the Agent C system, enabling horizontal scaling and event persistence.
"""

from .redis_stream_manager import RedisStreamManager
from .stream_key_manager import StreamKeyManager
from .event_serializer import EventSerializer

__all__ = [
    'RedisStreamManager',
    'StreamKeyManager', 
    'EventSerializer'
]