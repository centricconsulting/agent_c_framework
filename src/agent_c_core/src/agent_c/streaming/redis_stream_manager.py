"""
Redis Stream Manager for Agent C

Core Redis Streams functionality for distributed event processing, providing
publish/consume capabilities with error handling and fallback mechanisms.
"""

import asyncio
import logging
import os
import time
import json
import yaml
from pathlib import Path
from typing import Optional, Dict, Any, AsyncGenerator, Tuple, Union
from datetime import datetime, timedelta
from enum import Enum

try:
    import redis
    import redis.asyncio as aioredis
    from redis.exceptions import ConnectionError, TimeoutError, RedisError
    REDIS_AVAILABLE = True
except ImportError:
    REDIS_AVAILABLE = False
    redis = None
    aioredis = None

from agent_c.models import ChatEvent
from .stream_key_manager import StreamKeyManager
from .event_serializer import EventSerializer, EventContext


logger = logging.getLogger(__name__)


class OperationMode(Enum):
    """Redis Streams operation modes."""
    REDIS_ONLY = "redis_only"  # Use only Redis Streams for event handling
    HYBRID = "hybrid"          # Use Redis for publishing but async queue for consumption (current behavior)
    ASYNC_ONLY = "async_only"  # Use only async queue (legacy mode)


class FailoverStrategy(Enum):
    """Failover strategies for Redis unavailability."""
    AUTO_FAILOVER = "auto_failover"  # Automatically fall back to async queue if Redis is unavailable (default)
    NO_FAILOVER = "no_failover"      # Fail operations if Redis is unavailable


class RedisConfig:
    """Enhanced configuration for Redis Streams with operation modes and failover strategies.
    
    Supports multiple configuration sources:
    - Environment variables (with REDIS_ prefix)
    - Constructor parameters (highest priority)
    - Configuration files (JSON/YAML)
    - Default values
    
    Operation Modes:
    - REDIS_ONLY: Use only Redis Streams for event handling
    - HYBRID: Use Redis for publishing but async queue for consumption (current behavior)
    - ASYNC_ONLY: Use only async queue (legacy mode)
    
    Failover Strategies:
    - AUTO_FAILOVER: Automatically fall back to async queue if Redis is unavailable (default)
    - NO_FAILOVER: Fail operations if Redis is unavailable
    """
    
    def __init__(self, 
                 config_file: Optional[str] = None,
                 **kwargs):
        """Initialize Redis configuration.
        
        Args:
            config_file: Path to configuration file (JSON or YAML)
            **kwargs: Override parameters (highest priority)
        """
        # Version detection properties
        self.redis_version: Optional[str] = None
        self.streams_supported: Optional[bool] = None
        self.version_check_enabled: bool = kwargs.get('version_check_enabled', True)
        self.auto_mode_switch: bool = kwargs.get('auto_mode_switch', True)
        
        # Load base configuration from multiple sources
        self._load_configuration(config_file, **kwargs)
        
        # Validate configuration
        self.validate()
        
        logger.info(f"RedisConfig initialized: mode={self.operation_mode.value}, "
                   f"failover={self.failover_strategy.value}, enabled={self.enabled}")
    
    def _load_configuration(self, config_file: Optional[str] = None, **kwargs):
        """Load configuration from multiple sources in order of priority."""
        # 1. Start with defaults
        config = self._get_default_config()
        
        # 2. Load from configuration file if provided
        if config_file:
            file_config = self._load_config_file(config_file)
            config.update(file_config)
        
        # 3. Override with environment variables
        env_config = self._load_env_config()
        config.update(env_config)
        
        # 4. Override with constructor parameters (highest priority)
        config.update(kwargs)
        
        # Apply configuration to instance
        self._apply_config(config)
    
    def _get_default_config(self) -> Dict[str, Any]:
        """Get default configuration values."""
        return {
            # Redis connection settings
            'host': 'localhost',
            'port': 6379,
            'password': None,
            'db': 0,
            'ssl': False,
            
            # Core configuration
            'operation_mode': OperationMode.HYBRID.value,
            'failover_strategy': FailoverStrategy.AUTO_FAILOVER.value,
            'health_check_interval': 30,  # seconds
            'connection_timeout': 5,      # seconds
            'max_retry_attempts': 3,
            'circuit_breaker_threshold': 5,  # failures before opening circuit
            'recovery_interval': 60,      # seconds between recovery attempts
            
            # Performance configuration
            'batch_size': 100,           # events per batch
            'buffer_size': 10000,        # maximum buffer size for event queuing
            'stream_max_len': 1000,      # maximum length for Redis Streams
            'stream_trim_interval': 300, # seconds between stream trimming
            
            # Legacy stream configuration (backward compatibility)
            'stream_max_length': 1000,   # alias for stream_max_len
            'stream_ttl': 3600,          # 1 hour
            'read_timeout': 1000,        # milliseconds
            
            # Connection pool configuration
            'max_connections': 10,
            'socket_timeout': 5,
            'socket_connect_timeout': 5,
            
            # Feature flags
            'enabled': False,
            'fallback_enabled': True,    # legacy compatibility
            'exclusive_callback_via_redis': False,  # When True, callbacks are only handled via Redis (not directly)
        }
    
    def _load_env_config(self) -> Dict[str, Any]:
        """Load configuration from environment variables with REDIS_ prefix."""
        config = {}
        
        # Environment variable mappings
        env_mappings = {
            'REDIS_HOST': ('host', str),
            'REDIS_PORT': ('port', int),
            'REDIS_PASSWORD': ('password', str),
            'REDIS_DB': ('db', int),
            'REDIS_SSL': ('ssl', lambda x: x.lower() == 'true'),
            
            # Core configuration
            'REDIS_OPERATION_MODE': ('operation_mode', str),
            'REDIS_FAILOVER_STRATEGY': ('failover_strategy', str),
            'REDIS_HEALTH_CHECK_INTERVAL': ('health_check_interval', int),
            'REDIS_CONNECTION_TIMEOUT': ('connection_timeout', int),
            'REDIS_MAX_RETRY_ATTEMPTS': ('max_retry_attempts', int),
            'REDIS_CIRCUIT_BREAKER_THRESHOLD': ('circuit_breaker_threshold', int),
            'REDIS_RECOVERY_INTERVAL': ('recovery_interval', int),
            
            # Performance configuration
            'REDIS_BATCH_SIZE': ('batch_size', int),
            'REDIS_BUFFER_SIZE': ('buffer_size', int),
            'REDIS_STREAM_MAX_LEN': ('stream_max_len', int),
            'REDIS_STREAM_TRIM_INTERVAL': ('stream_trim_interval', int),
            
            # Legacy compatibility
            'REDIS_STREAM_MAX_LENGTH': ('stream_max_length', int),
            'REDIS_STREAM_TTL': ('stream_ttl', int),
            'REDIS_STREAM_READ_TIMEOUT': ('read_timeout', int),
            'REDIS_MAX_CONNECTIONS': ('max_connections', int),
            'REDIS_SOCKET_TIMEOUT': ('socket_timeout', int),
            'REDIS_SOCKET_CONNECT_TIMEOUT': ('socket_connect_timeout', int),
            
            # Feature flags
            'ENABLE_REDIS_STREAMS': ('enabled', lambda x: x.lower() == 'true'),
            'REDIS_STREAMS_FALLBACK_TO_QUEUE': ('fallback_enabled', lambda x: x.lower() == 'true'),
            'REDIS_EXCLUSIVE_CALLBACK': ('exclusive_callback_via_redis', lambda x: x.lower() == 'true'),
        }
        
        for env_key, (config_key, converter) in env_mappings.items():
            env_value = os.getenv(env_key)
            if env_value is not None:
                try:
                    if converter == str and env_value is None:
                        config[config_key] = None
                    else:
                        config[config_key] = converter(env_value)
                except (ValueError, TypeError) as e:
                    logger.warning(f"Invalid environment variable {env_key}={env_value}: {e}")
        
        return config
    
    def _load_config_file(self, config_file: str) -> Dict[str, Any]:
        """Load configuration from JSON or YAML file."""
        try:
            config_path = Path(config_file)
            if not config_path.exists():
                logger.warning(f"Configuration file not found: {config_file}")
                return {}
            
            with open(config_path, 'r') as f:
                if config_path.suffix.lower() in ['.yaml', '.yml']:
                    config = yaml.safe_load(f) or {}
                elif config_path.suffix.lower() == '.json':
                    config = json.load(f) or {}
                else:
                    logger.warning(f"Unsupported config file format: {config_file}")
                    return {}
            
            # Extract redis section if it exists
            if 'redis' in config:
                config = config['redis']
            
            logger.info(f"Loaded configuration from {config_file}")
            return config
            
        except Exception as e:
            logger.error(f"Failed to load configuration file {config_file}: {e}")
            return {}
    
    def _apply_config(self, config: Dict[str, Any]):
        """Apply configuration values to instance attributes."""
        # Redis connection settings
        self.host = config['host']
        self.port = config['port']
        self.password = config['password']
        self.db = config['db']
        self.ssl = config['ssl']
        
        # Core configuration - convert string values to enums
        operation_mode_str = config['operation_mode']
        if isinstance(operation_mode_str, str):
            try:
                self.operation_mode = OperationMode(operation_mode_str.lower())
            except ValueError:
                logger.warning(f"Invalid operation mode '{operation_mode_str}', using HYBRID")
                self.operation_mode = OperationMode.HYBRID
        else:
            self.operation_mode = operation_mode_str
        
        failover_strategy_str = config['failover_strategy']
        if isinstance(failover_strategy_str, str):
            try:
                self.failover_strategy = FailoverStrategy(failover_strategy_str.lower())
            except ValueError:
                logger.warning(f"Invalid failover strategy '{failover_strategy_str}', using AUTO_FAILOVER")
                self.failover_strategy = FailoverStrategy.AUTO_FAILOVER
        else:
            self.failover_strategy = failover_strategy_str
        
        self.health_check_interval = config['health_check_interval']
        self.connection_timeout = config['connection_timeout']
        self.max_retry_attempts = config['max_retry_attempts']
        self.circuit_breaker_threshold = config['circuit_breaker_threshold']
        self.recovery_interval = config['recovery_interval']
        
        # Performance configuration
        self.batch_size = config['batch_size']
        self.buffer_size = config['buffer_size']
        self.stream_max_len = config['stream_max_len']
        self.stream_trim_interval = config['stream_trim_interval']
        
        # Legacy compatibility
        self.stream_max_length = config.get('stream_max_length', self.stream_max_len)
        self.stream_ttl = config['stream_ttl']
        self.read_timeout = config['read_timeout']
        
        # Connection pool configuration
        self.max_connections = config['max_connections']
        self.socket_timeout = config['socket_timeout']
        self.socket_connect_timeout = config['socket_connect_timeout']
        
        # Feature flags
        self.enabled = config['enabled']
        self.fallback_enabled = config['fallback_enabled']
        self.exclusive_callback_via_redis = config.get('exclusive_callback_via_redis', False)
    
    def is_redis_primary(self) -> bool:
        """Returns True if Redis is the primary event handling method.
        
        Returns:
            True for REDIS_ONLY and HYBRID modes, False for ASYNC_ONLY
        """
        return self.operation_mode in [OperationMode.REDIS_ONLY, OperationMode.HYBRID]
    
    def is_failover_enabled(self) -> bool:
        """Returns True if automatic failover is enabled.
        
        Returns:
            True if failover strategy is AUTO_FAILOVER
        """
        return self.failover_strategy == FailoverStrategy.AUTO_FAILOVER
    
    def get_effective_mode(self, redis_healthy: bool = True) -> OperationMode:
        """Returns the current effective operation mode.
        
        Args:
            redis_healthy: Whether Redis is currently healthy/available
            
        Returns:
            The effective operation mode considering Redis health and failover settings
        """
        if not self.enabled:
            return OperationMode.ASYNC_ONLY
        
        if self.operation_mode == OperationMode.ASYNC_ONLY:
            return OperationMode.ASYNC_ONLY
        
        if not redis_healthy:
            if self.is_failover_enabled():
                # Failover to async mode if Redis is unhealthy
                return OperationMode.ASYNC_ONLY
            else:
                # No failover - maintain original mode but operations may fail
                return self.operation_mode
        
        # Redis is healthy, use configured mode
        return self.operation_mode
    
    def validate(self) -> None:
        """Validates the configuration for consistency and raises ValueError if invalid.
        
        Raises:
            ValueError: If configuration is invalid
        """
        errors = []
        
        # Validate basic connection parameters
        if not isinstance(self.host, str) or not self.host.strip():
            errors.append("host must be a non-empty string")
        
        if not isinstance(self.port, int) or not (1 <= self.port <= 65535):
            errors.append("port must be an integer between 1 and 65535")
        
        if not isinstance(self.db, int) or self.db < 0:
            errors.append("db must be a non-negative integer")
        
        # Validate timeout and interval values
        if self.health_check_interval <= 0:
            errors.append("health_check_interval must be positive")
        
        if self.connection_timeout <= 0:
            errors.append("connection_timeout must be positive")
        
        if self.max_retry_attempts < 0:
            errors.append("max_retry_attempts must be non-negative")
        
        if self.circuit_breaker_threshold <= 0:
            errors.append("circuit_breaker_threshold must be positive")
        
        if self.recovery_interval <= 0:
            errors.append("recovery_interval must be positive")
        
        # Validate performance parameters
        if self.batch_size <= 0:
            errors.append("batch_size must be positive")
        
        if self.buffer_size <= 0:
            errors.append("buffer_size must be positive")
        
        if self.stream_max_len <= 0:
            errors.append("stream_max_len must be positive")
        
        if self.stream_trim_interval <= 0:
            errors.append("stream_trim_interval must be positive")
        
        # Validate stream configuration
        if self.stream_ttl <= 0:
            errors.append("stream_ttl must be positive")
        
        if self.read_timeout <= 0:
            errors.append("read_timeout must be positive")
        
        # Validate connection pool settings
        if self.max_connections <= 0:
            errors.append("max_connections must be positive")
        
        if self.socket_timeout <= 0:
            errors.append("socket_timeout must be positive")
        
        if self.socket_connect_timeout <= 0:
            errors.append("socket_connect_timeout must be positive")
        
        # Validate logical consistency
        if self.operation_mode == OperationMode.REDIS_ONLY and not self.enabled:
            errors.append("Cannot use REDIS_ONLY mode when Redis Streams is disabled")
        
        if self.operation_mode == OperationMode.REDIS_ONLY and self.failover_strategy == FailoverStrategy.NO_FAILOVER:
            # This is actually valid - it means Redis-only with no fallback
            pass
        
        # Warn about performance considerations
        if self.batch_size > self.buffer_size:
            logger.warning("batch_size is larger than buffer_size, this may cause performance issues")
        
        if self.stream_max_len > 10000:
            logger.warning("stream_max_len is very large, this may impact Redis memory usage")
        
        if errors:
            raise ValueError(f"Configuration validation failed: {'; '.join(errors)}")
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert configuration to dictionary for serialization.
        
        Returns:
            Dictionary representation of the configuration
        """
        return {
            # Redis connection settings
            'host': self.host,
            'port': self.port,
            'password': self.password,
            'db': self.db,
            'ssl': self.ssl,
            
            # Core configuration
            'operation_mode': self.operation_mode.value,
            'failover_strategy': self.failover_strategy.value,
            'health_check_interval': self.health_check_interval,
            'connection_timeout': self.connection_timeout,
            'max_retry_attempts': self.max_retry_attempts,
            'circuit_breaker_threshold': self.circuit_breaker_threshold,
            'recovery_interval': self.recovery_interval,
            
            # Performance configuration
            'batch_size': self.batch_size,
            'buffer_size': self.buffer_size,
            'stream_max_len': self.stream_max_len,
            'stream_trim_interval': self.stream_trim_interval,
            
            # Legacy compatibility
            'stream_max_length': self.stream_max_length,
            'stream_ttl': self.stream_ttl,
            'read_timeout': self.read_timeout,
            
            # Connection pool configuration
            'max_connections': self.max_connections,
            'socket_timeout': self.socket_timeout,
            'socket_connect_timeout': self.socket_connect_timeout,
            
            # Feature flags
            'enabled': self.enabled,
            'fallback_enabled': self.fallback_enabled,
            'exclusive_callback_via_redis': self.exclusive_callback_via_redis,
        }
    
    def supports_streams(self) -> bool:
        """Check if Redis version supports Streams (5.0+)."""
        if self.streams_supported is not None:
            return self.streams_supported
        
        if self.redis_version:
            try:
                version_parts = self.redis_version.split('.')
                major = int(version_parts[0])
                minor = int(version_parts[1]) if len(version_parts) > 1 else 0
                return major > 5 or (major == 5 and minor >= 0)
            except (ValueError, AttributeError, IndexError):
                return False
        
        return False  # Conservative default
    
    def __repr__(self) -> str:
        """String representation of the configuration."""
        return (f"RedisConfig(host={self.host}, port={self.port}, "
                f"mode={self.operation_mode.value}, "
                f"failover={self.failover_strategy.value}, "
                f"enabled={self.enabled})")


class FallbackEventQueue:
    """In-memory fallback queue when Redis is unavailable."""
    
    def __init__(self, max_size: int = 10000):
        self._queue = asyncio.Queue(maxsize=max_size)
        self._fallback_active = False
        self._dropped_events = 0
    
    async def put(self, event_data: Dict[str, Any]):
        """Add event to fallback queue."""
        try:
            self._queue.put_nowait(event_data)
        except asyncio.QueueFull:
            self._dropped_events += 1
            logger.warning(f"Fallback queue full, dropped event. Total dropped: {self._dropped_events}")
    
    async def get(self) -> Optional[Dict[str, Any]]:
        """Get event from fallback queue."""
        try:
            return await asyncio.wait_for(self._queue.get(), timeout=1.0)
        except asyncio.TimeoutError:
            return None
    
    def activate(self):
        """Activate fallback mode."""
        self._fallback_active = True
        logger.warning("Redis Streams unavailable, activating in-memory fallback")
    
    def deactivate(self):
        """Deactivate fallback mode."""
        if self._fallback_active:
            logger.info("Redis Streams restored, deactivating fallback")
            self._fallback_active = False
    
    @property
    def is_active(self) -> bool:
        return self._fallback_active
    
    @property
    def size(self) -> int:
        return self._queue.qsize()


class RedisStreamManager:
    """Core Redis Streams manager for Agent C event processing."""
    
    def __init__(self, redis_config: Optional[RedisConfig] = None):
        """
        Initialize Redis Stream Manager.
        
        Args:
            redis_config: Redis configuration. If None, uses default configuration.
        """
        self.config = redis_config or RedisConfig()
        self._redis_client = None
        self._connection_pool = None
        self._fallback_queue = FallbackEventQueue()
        self._is_healthy = False
        self._last_health_check = None
        self._sequence_counter = 0
        
        # Check if Redis is available and enabled
        if not REDIS_AVAILABLE:
            logger.warning("Redis library not available, Redis Streams disabled")
            self.config.enabled = False
        
        if not self.config.enabled:
            logger.info("Redis Streams disabled, will use fallback queue only")
            self._fallback_queue.activate()
    
    async def initialize(self):
        """Initialize Redis connection and health checking."""
        if not self.config.enabled:
            return
        
        try:
            # Create connection pool
            self._connection_pool = aioredis.ConnectionPool(
                host=self.config.host,
                port=self.config.port,
                password=self.config.password,
                db=self.config.db,
                ssl=self.config.ssl,
                max_connections=self.config.max_connections,
                socket_timeout=self.config.socket_timeout,
                socket_connect_timeout=self.config.socket_connect_timeout,
                health_check_interval=30
            )
            
            # Create Redis client
            self._redis_client = aioredis.Redis(connection_pool=self._connection_pool)
            
            # Perform version detection
            if self.config.version_check_enabled:
                await self._detect_redis_capabilities()
            
            # Test connection
            await self._health_check()
            
            if self._is_healthy:
                logger.info("Redis Streams initialized successfully")
                self._fallback_queue.deactivate()
            else:
                logger.warning("Redis connection failed, using fallback queue")
                self._fallback_queue.activate()
                
        except Exception as e:
            logger.error(f"Failed to initialize Redis Streams: {e}")
            self._fallback_queue.activate()
    
    async def publish_event(self, 
                          event_type: str, 
                          event_data: Union[Dict[str, Any], ChatEvent], 
                          session_id: str, 
                          interaction_id: str, 
                          source: str = "BaseAgent",
                          user_id: str = "unknown") -> Optional[str]:
        """
        Publish an event to Redis Stream.
        
        Args:
            event_type: Type of the event
            event_data: Event data (dict or ChatEvent)
            session_id: Session identifier
            interaction_id: Interaction identifier
            source: Source component name
            user_id: User identifier
            
        Returns:
            Redis message ID if successful, None if failed
        """
        
        # Create event context
        context = EventContext(
            session_id=session_id,
            interaction_id=interaction_id,
            user_id=user_id,
            sequence=self._get_next_sequence(),
            source=source
        )
        
        # If Redis is not available or healthy, use fallback
        if not self._is_healthy or not self.config.enabled:
            await self._publish_to_fallback(event_type, event_data, context)
            return None
        
        # Check if Redis Streams are supported
        if self.config.version_check_enabled and not self.config.supports_streams():
            logger.debug(f"Redis Streams not supported (Redis version: {self.config.redis_version}), using fallback queue")
            await self._publish_to_fallback(event_type, event_data, context)
            return None
        
        try:
            # Build stream key
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            
            # Serialize event
            if isinstance(event_data, ChatEvent):
                serialized_message = EventSerializer.serialize_event(event_data, context)
            else:
                # Create a simple event wrapper for dict data
                class SimpleEvent:
                    def __init__(self, event_type: str, data: Dict[str, Any]):
                        self.type = event_type
                        self.data = data
                        
                    def to_dict(self):
                        return self.data
                
                simple_event = SimpleEvent(event_type, event_data)
                serialized_message = EventSerializer.serialize_event(simple_event, context)
            
            # Publish to Redis Stream
            message_id = await self._redis_client.xadd(
                stream_key,
                serialized_message,
                maxlen=self.config.stream_max_length,
                approximate=True
            )
            
            logger.debug(f"Published event {event_type} to stream {stream_key}: {message_id}")
            return message_id.decode() if isinstance(message_id, bytes) else message_id
            
        except Exception as e:
            # Check if this is a version compatibility error
            if self._is_version_compatibility_error(e):
                logger.error(f"Redis version compatibility error: {e}")
                self.config.streams_supported = False
                # Force mode switch if configured
                if self.config.auto_mode_switch and self.config.operation_mode != OperationMode.ASYNC_ONLY:
                    logger.warning("Switching to ASYNC_ONLY mode due to Redis version incompatibility")
                    self.config.operation_mode = OperationMode.ASYNC_ONLY
            else:
                logger.error(f"Failed to publish event to Redis Stream: {e}")
                
            await self._handle_redis_error(e)
            
            # Fallback to queue
            await self._publish_to_fallback(event_type, event_data, context)
            return None
    
    async def consume_events(self, 
                           session_id: str, 
                           interaction_id: str, 
                           last_id: str = '0-0', 
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """
        Consume events from Redis Stream.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
            last_id: Last message ID received
            block: Block time in milliseconds, None for non-blocking
            
        Yields:
            Event data dictionaries
        """
        
        # If Redis is not available, consume from fallback
        if not self._is_healthy or not self.config.enabled:
            async for event_data in self._consume_from_fallback():
                yield event_data
            return
            
        # Check if Redis Streams are supported
        if self.config.version_check_enabled and not self.config.supports_streams():
            logger.debug(f"Redis Streams not supported (Redis version: {self.config.redis_version}), using fallback queue")
            async for event_data in self._consume_from_fallback():
                yield event_data
            return
        
        try:
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            current_id = last_id
            
            while True:
                try:
                    # Read from stream
                    response = await self._redis_client.xread(
                        {stream_key: current_id},
                        count=10,
                        block=block or self.config.read_timeout
                    )
                    
                    if not response:
                        continue  # No new messages
                    
                    # Process messages
                    for stream, messages in response:
                        for message_id, fields in messages:
                            try:
                                # Convert bytes to strings
                                string_fields = {}
                                for key, value in fields.items():
                                    key_str = key.decode() if isinstance(key, bytes) else key
                                    value_str = value.decode() if isinstance(value, bytes) else value
                                    string_fields[key_str] = value_str
                                
                                # Add message ID
                                string_fields['message_id'] = message_id.decode() if isinstance(message_id, bytes) else message_id
                                
                                # Deserialize event
                                event_data, context = EventSerializer.deserialize_event(string_fields)
                                
                                # Add context info to event data
                                event_data['_context'] = {
                                    'message_id': string_fields['message_id'],
                                    'session_id': context.session_id,
                                    'interaction_id': context.interaction_id,
                                    'source': context.source,
                                    'sequence': context.sequence
                                }
                                
                                yield event_data
                                current_id = message_id
                                
                            except Exception as e:
                                logger.error(f"Error processing message {message_id}: {e}")
                                continue
                    
                except asyncio.TimeoutError:
                    continue  # Timeout is expected with blocking reads
                except Exception as e:
                    logger.error(f"Error reading from Redis Stream {stream_key}: {e}")
                    await self._handle_redis_error(e)
                    break
                    
        except Exception as e:
            logger.error(f"Error in consume_events: {e}")
            # Fallback to queue consumption
            async for event_data in self._consume_from_fallback():
                yield event_data
    
    async def create_stream(self, session_id: str, interaction_id: str) -> str:
        """
        Create a new stream with metadata.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
            
        Returns:
            Stream key
        """
        stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
        
        if not self._is_healthy or not self.config.enabled:
            logger.debug(f"Redis not available, stream {stream_key} created in memory only")
            return stream_key
        
        try:
            # Create metadata event
            metadata_event = EventSerializer.create_stream_metadata_event(
                "stream_created",
                session_id,
                interaction_id,
                created_at=datetime.now().isoformat(),
                version="1.0"
            )
            
            # Add to stream
            message_id = await self._redis_client.xadd(stream_key, metadata_event)
            
            # Set TTL
            await self._redis_client.expire(stream_key, self.config.stream_ttl)
            
            logger.debug(f"Created stream {stream_key} with message {message_id}")
            return stream_key
            
        except Exception as e:
            logger.error(f"Failed to create stream {stream_key}: {e}")
            await self._handle_redis_error(e)
            return stream_key
    
    async def close_stream(self, session_id: str, interaction_id: str):
        """
        Close a stream with final metadata.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
        """
        if not self._is_healthy or not self.config.enabled:
            return
        
        try:
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            
            # Add closure metadata
            metadata_event = EventSerializer.create_stream_metadata_event(
                "stream_closed",
                session_id,
                interaction_id,
                closed_at=datetime.now().isoformat(),
                final_event="true"
            )
            
            await self._redis_client.xadd(stream_key, metadata_event)
            
            # Extend TTL for historical retention
            extended_ttl = self.config.stream_ttl * 24  # 24 hours for closed streams
            await self._redis_client.expire(stream_key, extended_ttl)
            
            logger.debug(f"Closed stream {stream_key}")
            
        except Exception as e:
            logger.error(f"Failed to close stream {stream_key}: {e}")
    
    async def cleanup_old_streams(self, max_age_seconds: int = 3600):
        """
        Clean up old streams.
        
        Args:
            max_age_seconds: Maximum age of streams to keep
        """
        if not self._is_healthy or not self.config.enabled:
            return
        
        try:
            pattern = StreamKeyManager.get_all_streams_pattern()
            async for key in self._redis_client.scan_iter(match=pattern):
                try:
                    # Check stream age using TTL
                    ttl = await self._redis_client.ttl(key)
                    if ttl == -1:  # No TTL set
                        await self._redis_client.expire(key, self.config.stream_ttl)
                    elif ttl < 0:  # Stream expired
                        continue
                        
                except Exception as e:
                    logger.error(f"Error checking stream {key}: {e}")
                    
        except Exception as e:
            logger.error(f"Error during cleanup: {e}")
    
    async def close(self):
        """Clean shutdown of Redis connections."""
        if self._redis_client:
            try:
                await self._redis_client.close()
            except Exception as e:
                logger.error(f"Error closing Redis client: {e}")
        
        if self._connection_pool:
            try:
                await self._connection_pool.disconnect()
            except Exception as e:
                logger.error(f"Error closing connection pool: {e}")
    
    async def _detect_redis_capabilities(self):
        """Detect Redis server capabilities."""
        try:
            info = await self._redis_client.info()
            self.config.redis_version = info.get('redis_version', 'unknown')
            self.config.streams_supported = self.config.supports_streams()
            
            logger.info(f"Redis capabilities - Version: {self.config.redis_version}, "
                       f"Streams supported: {self.config.streams_supported}")
            
            # Auto-adjust operation mode if needed
            if not self.config.streams_supported and self.config.auto_mode_switch:
                if self.config.operation_mode != OperationMode.ASYNC_ONLY:
                    logger.warning(f"Redis version {self.config.redis_version} doesn't support Streams. "
                               f"Auto-switching to ASYNC_ONLY mode.")
                    self.config.operation_mode = OperationMode.ASYNC_ONLY
                
        except Exception as e:
            logger.warning(f"Could not detect Redis capabilities: {e}")
            self.config.redis_version = 'unknown'
            self.config.streams_supported = False
    
    async def _health_check(self):
        """Perform Redis health check."""
        try:
            if self._redis_client:
                await self._redis_client.ping()
                self._is_healthy = True
                self._last_health_check = datetime.now()
                if self._fallback_queue.is_active:
                    self._fallback_queue.deactivate()
        except Exception as e:
            self._is_healthy = False
            logger.warning(f"Redis health check failed: {e}")
            if not self._fallback_queue.is_active:
                self._fallback_queue.activate()
    
    async def _handle_redis_error(self, error: Exception):
        """Handle Redis connection errors."""
        self._is_healthy = False
        logger.error(f"Redis error: {error}")
        
        if not self._fallback_queue.is_active:
            self._fallback_queue.activate()
    
    async def _publish_to_fallback(self, event_type: str, event_data: Any, context: EventContext):
        """Publish event to fallback queue."""
        fallback_event = {
            'event_type': event_type,
            'event_data': event_data.to_dict() if hasattr(event_data, 'to_dict') else event_data,
            'context': context.__dict__,
            'timestamp': datetime.now().isoformat()
        }
        await self._fallback_queue.put(fallback_event)
    
    async def _consume_from_fallback(self) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events from fallback queue."""
        while True:
            event_data = await self._fallback_queue.get()
            if event_data:
                yield event_data
            else:
                await asyncio.sleep(0.1)  # Brief pause when no events
    
    def _is_version_compatibility_error(self, error: Exception) -> bool:
        """Check if error is due to Redis version incompatibility."""
        error_msg = str(error).lower()
        version_indicators = [
            "unknown command 'xadd'",
            "unknown command xadd",
            "unknown command 'xread'", 
            "unknown command xread", 
            "unknown command 'xinfo'",
            "unknown command xinfo",
            "unknown command 'xtrim'",
            "unknown command xtrim"
        ]
        return any(indicator in error_msg for indicator in version_indicators)
    
    def _get_next_sequence(self) -> int:
        """Get next sequence number."""
        self._sequence_counter += 1
        return self._sequence_counter
    
    @property
    def is_healthy(self) -> bool:
        """Check if Redis connection is healthy."""
        return self._is_healthy
    
    @property
    def is_fallback_active(self) -> bool:
        """Check if fallback queue is active."""
        return self._fallback_queue.is_active