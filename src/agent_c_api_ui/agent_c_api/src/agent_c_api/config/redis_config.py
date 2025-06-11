import asyncio
import logging
from typing import Optional, Dict, Any, List
from redis import asyncio as aioredis
from redis.exceptions import ConnectionError, TimeoutError, RedisError
from .env_config import settings

# Import resilient mode components
try:
    from ..core.util.resilient_mode import ResilientModeConfig, OperationMode
except ImportError:
    # Fallback if resilient mode components aren't available
    ResilientModeConfig = None
    OperationMode = None

import time

logger = logging.getLogger(__name__)

class RedisConfig:
    _redis_client: Optional[aioredis.Redis] = None
    _resilient_config: Optional['ResilientModeConfig'] = None
    
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
    
    @classmethod
    def configure_resilient_mode(
        cls, 
        operation_mode: Optional['OperationMode'] = None,
        mode_switch_delay_seconds: float = 30.0,
        health_check_interval_seconds: float = 10.0,
        redis_failure_threshold: int = 3,
        circuit_breaker_timeout_seconds: float = 60.0,
        enable_auto_recovery: bool = True,
        max_transition_retries: int = 3,
        performance_monitoring_enabled: bool = True
    ) -> 'ResilientModeConfig':
        """Configure resilient mode settings."""
        if not ResilientModeConfig or not OperationMode:
            raise ImportError("Resilient mode components not available")
        
        # Default to HYBRID mode if not specified
        if operation_mode is None:
            operation_mode = OperationMode.HYBRID
        
        cls._resilient_config = ResilientModeConfig(
            operation_mode=operation_mode,
            mode_switch_delay_seconds=mode_switch_delay_seconds,
            health_check_interval_seconds=health_check_interval_seconds,
            redis_failure_threshold=redis_failure_threshold,
            circuit_breaker_timeout_seconds=circuit_breaker_timeout_seconds,
            enable_auto_recovery=enable_auto_recovery,
            max_transition_retries=max_transition_retries,
            performance_monitoring_enabled=performance_monitoring_enabled
        )
        
        logger.info(f"Configured resilient mode: {operation_mode.value}")
        return cls._resilient_config
    
    @classmethod
    def get_resilient_config(cls) -> Optional['ResilientModeConfig']:
        """Get current resilient mode configuration."""
        return cls._resilient_config
    
    @classmethod
    def create_default_resilient_config(cls) -> 'ResilientModeConfig':
        """Create a default resilient mode configuration."""
        if not ResilientModeConfig or not OperationMode:
            raise ImportError("Resilient mode components not available")
        
        # Determine default mode based on Redis availability
        default_mode = OperationMode.HYBRID  # Safe default
        
        return cls.configure_resilient_mode(operation_mode=default_mode)
    
    @classmethod
    def build_redis_url(cls) -> str:
        """Build Redis URL from environment settings."""
        if settings.REDIS_PASSWORD:
            if settings.REDIS_USERNAME:
                return f"redis://{settings.REDIS_USERNAME}:{settings.REDIS_PASSWORD}@{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"
            else:
                return f"redis://:{settings.REDIS_PASSWORD}@{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"
        else:
            return f"redis://{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"
    
    @classmethod
    def get_connection_info(cls) -> Dict[str, Any]:
        """Get Redis connection information."""
        return {
            'host': settings.REDIS_HOST,
            'port': settings.REDIS_PORT,
            'db': settings.REDIS_DB,
            'username': settings.REDIS_USERNAME,
            'has_password': bool(settings.REDIS_PASSWORD),
            'url': cls.build_redis_url()
        }
    
    @classmethod
    def validate_configuration(cls) -> Dict[str, Any]:
        """Validate Redis configuration and return validation results."""
        validation_result = {
            'valid': True,
            'errors': [],
            'warnings': [],
            'config_summary': {}
        }
        
        try:
            # Validate operation mode
            operation_mode = cls._validate_operation_mode()
            validation_result['config_summary']['operation_mode'] = operation_mode
            
            # Validate fallback mode
            fallback_mode = cls._validate_fallback_mode()
            validation_result['config_summary']['fallback_mode'] = fallback_mode
            
            # Validate timeouts and intervals
            cls._validate_timeouts(validation_result)
            
            # Validate thresholds
            cls._validate_thresholds(validation_result)
            
            # Validate retry configuration
            cls._validate_retry_config(validation_result)
            
            # Validate stream configuration
            cls._validate_stream_config(validation_result)
            
            # Check configuration consistency
            cls._validate_config_consistency(validation_result)
            
            # Set overall validation status
            validation_result['valid'] = len(validation_result['errors']) == 0
            
        except Exception as e:
            validation_result['valid'] = False
            validation_result['errors'].append(f"Configuration validation error: {str(e)}")
            logger.error(f"Redis configuration validation failed: {e}")
        
        return validation_result
    
    @classmethod
    def _validate_operation_mode(cls) -> OperationMode:
        """Validate and return the operation mode."""
        mode_str = settings.REDIS_OPERATION_MODE.upper()
        
        try:
            if not OperationMode:
                raise ValueError("OperationMode not available")
            return OperationMode(mode_str.lower())
        except ValueError:
            valid_modes = [mode.value for mode in OperationMode] if OperationMode else ['redis_only', 'hybrid', 'async_only']
            raise ValueError(
                f"Invalid REDIS_OPERATION_MODE '{mode_str}'. "
                f"Valid options: {', '.join(valid_modes)}"
            )
    
    @classmethod
    def _validate_fallback_mode(cls) -> OperationMode:
        """Validate and return the fallback mode."""
        fallback_str = settings.REDIS_FALLBACK_MODE.upper()
        
        try:
            if not OperationMode:
                raise ValueError("OperationMode not available")
            return OperationMode(fallback_str.lower())
        except ValueError:
            valid_modes = [mode.value for mode in OperationMode] if OperationMode else ['redis_only', 'hybrid', 'async_only']
            raise ValueError(
                f"Invalid REDIS_FALLBACK_MODE '{fallback_str}'. "
                f"Valid options: {', '.join(valid_modes)}"
            )
    
    @classmethod
    def _validate_timeouts(cls, validation_result: Dict[str, Any]) -> None:
        """Validate timeout configuration values."""
        timeout_configs = [
            ('REDIS_HEALTH_CHECK_INTERVAL', settings.REDIS_HEALTH_CHECK_INTERVAL, 5, 300),
            ('REDIS_HEALTH_CHECK_TIMEOUT', settings.REDIS_HEALTH_CHECK_TIMEOUT, 1, 30),
            ('REDIS_CIRCUIT_BREAKER_TIMEOUT', settings.REDIS_CIRCUIT_BREAKER_TIMEOUT, 10, 600),
            ('REDIS_MODE_SWITCH_DELAY', settings.REDIS_MODE_SWITCH_DELAY, 5, 300),
            ('REDIS_CONNECTION_TIMEOUT', settings.REDIS_CONNECTION_TIMEOUT, 1, 30),
            ('REDIS_SOCKET_TIMEOUT', settings.REDIS_SOCKET_TIMEOUT, 1, 30)
        ]
        
        for name, value, min_val, max_val in timeout_configs:
            if not isinstance(value, (int, float)) or value <= 0:
                validation_result['errors'].append(
                    f"{name} must be a positive number, got: {value}"
                )
            elif value < min_val:
                validation_result['warnings'].append(
                    f"{name} ({value}s) is below recommended minimum ({min_val}s)"
                )
            elif value > max_val:
                validation_result['warnings'].append(
                    f"{name} ({value}s) is above recommended maximum ({max_val}s)"
                )
        
        # Check timeout relationships
        if settings.REDIS_HEALTH_CHECK_TIMEOUT >= settings.REDIS_HEALTH_CHECK_INTERVAL:
            validation_result['errors'].append(
                "REDIS_HEALTH_CHECK_TIMEOUT must be less than REDIS_HEALTH_CHECK_INTERVAL"
            )
    
    @classmethod
    def _validate_thresholds(cls, validation_result: Dict[str, Any]) -> None:
        """Validate threshold configuration values."""
        threshold_configs = [
            ('REDIS_FAILURE_THRESHOLD', settings.REDIS_FAILURE_THRESHOLD, 1, 10),
            ('REDIS_RECOVERY_THRESHOLD', settings.REDIS_RECOVERY_THRESHOLD, 1, 20),
            ('REDIS_MAX_TRANSITION_RETRIES', settings.REDIS_MAX_TRANSITION_RETRIES, 0, 10)
        ]
        
        for name, value, min_val, max_val in threshold_configs:
            if not isinstance(value, int) or value < min_val:
                validation_result['errors'].append(
                    f"{name} must be an integer >= {min_val}, got: {value}"
                )
            elif value > max_val:
                validation_result['warnings'].append(
                    f"{name} ({value}) is above recommended maximum ({max_val})"
                )
    
    @classmethod
    def _validate_retry_config(cls, validation_result: Dict[str, Any]) -> None:
        """Validate retry configuration."""
        if settings.REDIS_MAX_RETRIES < 0:
            validation_result['errors'].append(
                f"REDIS_MAX_RETRIES must be >= 0, got: {settings.REDIS_MAX_RETRIES}"
            )
        
        valid_backoff_strategies = ['exponential', 'linear']
        if settings.REDIS_RETRY_BACKOFF.lower() not in valid_backoff_strategies:
            validation_result['errors'].append(
                f"REDIS_RETRY_BACKOFF must be one of {valid_backoff_strategies}, "
                f"got: {settings.REDIS_RETRY_BACKOFF}"
            )
        
        if settings.REDIS_CONNECTION_POOL_SIZE < 1:
            validation_result['errors'].append(
                f"REDIS_CONNECTION_POOL_SIZE must be >= 1, got: {settings.REDIS_CONNECTION_POOL_SIZE}"
            )
        elif settings.REDIS_CONNECTION_POOL_SIZE > 100:
            validation_result['warnings'].append(
                f"REDIS_CONNECTION_POOL_SIZE ({settings.REDIS_CONNECTION_POOL_SIZE}) "
                "is quite high and may consume excessive resources"
            )
    
    @classmethod
    def _validate_stream_config(cls, validation_result: Dict[str, Any]) -> None:
        """Validate stream configuration."""
        if settings.REDIS_STREAM_MAX_LEN < 100:
            validation_result['errors'].append(
                f"REDIS_STREAM_MAX_LEN must be >= 100, got: {settings.REDIS_STREAM_MAX_LEN}"
            )
        elif settings.REDIS_STREAM_MAX_LEN > 1000000:
            validation_result['warnings'].append(
                f"REDIS_STREAM_MAX_LEN ({settings.REDIS_STREAM_MAX_LEN}) "
                "is very high and may cause memory issues"
            )
        
        if settings.REDIS_EVENT_TTL < 3600:  # 1 hour
            validation_result['warnings'].append(
                f"REDIS_EVENT_TTL ({settings.REDIS_EVENT_TTL}s) is quite short "
                "and may cause premature data loss"
            )
        
        if not settings.REDIS_CONSUMER_GROUP_PREFIX.strip():
            validation_result['errors'].append(
                "REDIS_CONSUMER_GROUP_PREFIX cannot be empty"
            )
    
    @classmethod
    def _validate_config_consistency(cls, validation_result: Dict[str, Any]) -> None:
        """Validate configuration consistency and relationships."""
        
        # Check operation mode and auto-switching compatibility
        try:
            operation_mode = cls._validate_operation_mode()
            
            if operation_mode == OperationMode.REDIS_ONLY and settings.REDIS_AUTO_MODE_SWITCHING:
                validation_result['warnings'].append(
                    "REDIS_AUTO_MODE_SWITCHING is enabled with REDIS_ONLY mode. "
                    "Consider using HYBRID mode for automatic failover."
                )
            
            # Check fallback mode compatibility
            fallback_mode = cls._validate_fallback_mode()
            if operation_mode == OperationMode.ASYNC_ONLY and fallback_mode != OperationMode.ASYNC_ONLY:
                validation_result['warnings'].append(
                    "REDIS_FALLBACK_MODE is ignored when REDIS_OPERATION_MODE is ASYNC_ONLY"
                )
                
        except ValueError as e:
            validation_result['errors'].append(str(e))
        
        # Check resilient mode feature consistency
        if not settings.REDIS_ENABLE_RESILIENT_MODE:
            if settings.REDIS_AUTO_MODE_SWITCHING:
                validation_result['warnings'].append(
                    "REDIS_AUTO_MODE_SWITCHING requires REDIS_ENABLE_RESILIENT_MODE to be True"
                )
    
    @classmethod
    def create_resilient_config_from_env(cls) -> 'ResilientModeConfig':
        """Create ResilientModeConfig from environment variables with validation."""
        if not ResilientModeConfig or not OperationMode:
            raise ImportError("Resilient mode components not available")
        
        # Validate configuration first
        validation_result = cls.validate_configuration()
        if not validation_result['valid']:
            error_msg = "; ".join(validation_result['errors'])
            raise ValueError(f"Invalid Redis configuration: {error_msg}")
        
        # Log warnings if any
        for warning in validation_result['warnings']:
            logger.warning(f"Redis configuration warning: {warning}")
        
        # Create configuration
        try:
            operation_mode = cls._validate_operation_mode()
            
            config = ResilientModeConfig(
                operation_mode=operation_mode,
                mode_switch_delay_seconds=float(settings.REDIS_MODE_SWITCH_DELAY),
                health_check_interval_seconds=float(settings.REDIS_HEALTH_CHECK_INTERVAL),
                redis_failure_threshold=settings.REDIS_FAILURE_THRESHOLD,
                circuit_breaker_timeout_seconds=float(settings.REDIS_CIRCUIT_BREAKER_TIMEOUT),
                enable_auto_recovery=settings.REDIS_ENABLE_AUTO_RECOVERY,
                max_transition_retries=settings.REDIS_MAX_TRANSITION_RETRIES,
                performance_monitoring_enabled=settings.REDIS_PERFORMANCE_MONITORING
            )
            
            logger.info(
                f"Created resilient mode configuration: mode={operation_mode.value}, "
                f"auto_switching={settings.REDIS_AUTO_MODE_SWITCHING}, "
                f"health_interval={settings.REDIS_HEALTH_CHECK_INTERVAL}s"
            )
            
            return config
            
        except Exception as e:
            logger.error(f"Failed to create resilient config from environment: {e}")
            raise
    
    @classmethod
    def get_configuration_summary(cls) -> Dict[str, Any]:
        """Get a comprehensive summary of current Redis configuration."""
        try:
            validation_result = cls.validate_configuration()
            
            summary = {
                'connection': {
                    'host': settings.REDIS_HOST,
                    'port': settings.REDIS_PORT,
                    'database': settings.REDIS_DB,
                    'has_auth': bool(settings.REDIS_USERNAME or settings.REDIS_PASSWORD),
                    'connection_timeout': settings.REDIS_CONNECTION_TIMEOUT,
                    'socket_timeout': settings.REDIS_SOCKET_TIMEOUT,
                    'max_connections': settings.REDIS_MAX_CONNECTIONS
                },
                'resilient_mode': {
                    'enabled': settings.REDIS_ENABLE_RESILIENT_MODE,
                    'operation_mode': settings.REDIS_OPERATION_MODE,
                    'auto_mode_switching': settings.REDIS_AUTO_MODE_SWITCHING,
                    'fallback_mode': settings.REDIS_FALLBACK_MODE
                },
                'health_monitoring': {
                    'check_interval': settings.REDIS_HEALTH_CHECK_INTERVAL,
                    'check_timeout': settings.REDIS_HEALTH_CHECK_TIMEOUT,
                    'failure_threshold': settings.REDIS_FAILURE_THRESHOLD,
                    'recovery_threshold': settings.REDIS_RECOVERY_THRESHOLD
                },
                'performance': {
                    'circuit_breaker_timeout': settings.REDIS_CIRCUIT_BREAKER_TIMEOUT,
                    'max_retries': settings.REDIS_MAX_RETRIES,
                    'retry_backoff': settings.REDIS_RETRY_BACKOFF,
                    'connection_pool_size': settings.REDIS_CONNECTION_POOL_SIZE,
                    'performance_monitoring': settings.REDIS_PERFORMANCE_MONITORING
                },
                'streams': {
                    'prefix': settings.STREAM_PREFIX,
                    'max_length': settings.REDIS_STREAM_MAX_LEN,
                    'consumer_group_prefix': settings.REDIS_CONSUMER_GROUP_PREFIX,
                    'event_ttl': settings.REDIS_EVENT_TTL
                },
                'mode_switching': {
                    'switch_delay': settings.REDIS_MODE_SWITCH_DELAY,
                    'max_transition_retries': settings.REDIS_MAX_TRANSITION_RETRIES,
                    'enable_auto_recovery': settings.REDIS_ENABLE_AUTO_RECOVERY
                },
                'validation': validation_result
            }
            
            return summary
            
        except Exception as e:
            logger.error(f"Failed to generate configuration summary: {e}")
            return {
                'error': str(e),
                'validation': {'valid': False, 'errors': [str(e)]}
            }
    
    @classmethod
    def reload_configuration(cls) -> Dict[str, Any]:
        """Reload configuration and validate changes."""
        try:
            # Clear cached resilient config to force reload
            cls._resilient_config = None
            
            # Validate new configuration
            validation_result = cls.validate_configuration()
            
            if validation_result['valid']:
                # Create new resilient config
                if settings.REDIS_ENABLE_RESILIENT_MODE:
                    cls._resilient_config = cls.create_resilient_config_from_env()
                    logger.info("Redis configuration reloaded successfully")
                else:
                    logger.info("Redis configuration reloaded (resilient mode disabled)")
            else:
                logger.error(f"Configuration reload failed: {validation_result['errors']}")
            
            return {
                'success': validation_result['valid'],
                'validation': validation_result,
                'timestamp': time.time()
            }
            
        except Exception as e:
            logger.error(f"Failed to reload configuration: {e}")
            return {
                'success': False,
                'error': str(e),
                'timestamp': time.time()
            }

