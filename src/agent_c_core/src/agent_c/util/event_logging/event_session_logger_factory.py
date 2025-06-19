"""
EventSessionLogger Factory Functions and Utilities

This module provides comprehensive factory functions, migration utilities, and configuration
helpers for creating EventSessionLogger instances with common patterns. It supports:

- Basic factory functions for common use cases
- Migration utilities from old SessionLogger
- Environment-based configuration
- Development vs production patterns
- Transport integration helpers
- Configuration validation and error checking
- Performance monitoring and debugging utilities
"""

import os
import logging
import warnings
from pathlib import Path
from typing import Optional, Callable, Dict, Any, List
from dataclasses import dataclass, field
from enum import Enum

from agent_c.util.event_logging.event_session_logger import EventSessionLogger
from agent_c.util.logging_utils import LoggingManager
from agent_c.util.transports import (
    TransportInterface, CallbackTransport, LoggingTransport, NullTransport,
    FileTransport, HTTPTransport, QueueTransport, RetryTransport
)
from agent_c.util.transport_exceptions import EventSessionLoggerError


class LoggerEnvironment(Enum):
    """Environment types for logger configuration"""
    DEVELOPMENT = "development"
    TESTING = "testing"
    STAGING = "staging"
    PRODUCTION = "production"


class TransportType(Enum):
    """Supported transport types"""
    CALLBACK = "callback"
    LOGGING = "logging"
    NULL = "null"
    FILE = "file"
    HTTP = "http"
    QUEUE = "queue"
    RETRY = "retry"


@dataclass
class LoggerConfiguration:
    """Comprehensive configuration for EventSessionLogger"""
    # Basic configuration
    log_base_dir: str = "logs/sessions"
    include_system_prompt: bool = True
    log_format: str = "jsonl"
    file_naming_pattern: str = "%Y%m%d_%H%M%S"
    enable_local_logging: bool = True
    session_directory_pattern: str = "{session_id}"
    unknown_session_pattern: str = "unknown_{uuid}"
    
    # Transport configuration
    transport_type: Optional[TransportType] = None
    transport_config: Dict[str, Any] = field(default_factory=dict)
    
    # Retry configuration
    max_retry_attempts: int = 3
    retry_delay_seconds: float = 1.0
    enable_retry: bool = False
    fallback_transport_type: Optional[TransportType] = None
    fallback_transport_config: Dict[str, Any] = field(default_factory=dict)
    
    # Environment-specific settings
    environment: LoggerEnvironment = LoggerEnvironment.DEVELOPMENT
    debug_mode: bool = False
    performance_monitoring: bool = False
    
    # Migration settings
    migration_mode: bool = False
    deprecation_warnings: bool = True
    
    def validate(self) -> List[str]:
        """Validate configuration and return list of errors"""
        errors = []
        
        # Validate log_base_dir
        try:
            Path(self.log_base_dir).resolve()
        except Exception as e:
            errors.append(f"Invalid log_base_dir: {e}")
        
        # Validate retry settings
        if self.max_retry_attempts < 0:
            errors.append("max_retry_attempts must be >= 0")
        
        if self.retry_delay_seconds < 0:
            errors.append("retry_delay_seconds must be >= 0")
        
        # Validate transport configuration
        if self.transport_type == TransportType.HTTP:
            if 'endpoint_url' not in self.transport_config:
                errors.append("HTTP transport requires 'endpoint_url' in transport_config")
        
        if self.transport_type == TransportType.QUEUE:
            required_keys = ['queue_name', 'connection_string']
            for key in required_keys:
                if key not in self.transport_config:
                    errors.append(f"Queue transport requires '{key}' in transport_config")
        
        return errors


def _load_default_config() -> Dict[str, Any]:
    """Load default configuration from environment variables"""
    return {
        'log_base_dir': os.getenv('AGENT_LOG_DIR', 'logs/sessions'),
        'log_format': os.getenv('AGENT_LOG_FORMAT', 'jsonl'),
        'include_system_prompt': os.getenv('AGENT_LOG_INCLUDE_PROMPT', 'true').lower() == 'true',
        'max_retry_attempts': int(os.getenv('AGENT_LOG_MAX_RETRIES', '3')),
        'retry_delay_seconds': float(os.getenv('AGENT_LOG_RETRY_DELAY', '1.0')),
        'file_naming_pattern': os.getenv('AGENT_LOG_FILE_PATTERN', '%Y%m%d_%H%M%S'),
        'enable_local_logging': os.getenv('AGENT_LOG_ENABLE_LOCAL', 'true').lower() == 'true',
        'session_directory_pattern': os.getenv('AGENT_LOG_SESSION_PATTERN', '{session_id}'),
        'unknown_session_pattern': os.getenv('AGENT_LOG_UNKNOWN_PATTERN', 'unknown_{uuid}')
    }


def load_configuration_from_env() -> LoggerConfiguration:
    """Load comprehensive configuration from environment variables"""
    config = LoggerConfiguration()
    
    # Basic settings
    config.log_base_dir = os.getenv('AGENT_LOG_DIR', config.log_base_dir)
    config.include_system_prompt = os.getenv('AGENT_LOG_INCLUDE_PROMPT', 'true').lower() == 'true'
    config.log_format = os.getenv('AGENT_LOG_FORMAT', config.log_format)
    config.file_naming_pattern = os.getenv('AGENT_LOG_FILE_PATTERN', config.file_naming_pattern)
    config.enable_local_logging = os.getenv('AGENT_LOG_ENABLE_LOCAL', 'true').lower() == 'true'
    
    # Transport settings
    transport_type_str = os.getenv('AGENT_TRANSPORT_TYPE')
    if transport_type_str:
        try:
            config.transport_type = TransportType(transport_type_str.lower())
        except ValueError:
            warnings.warn(f"Unknown transport type: {transport_type_str}")
    
    # Retry settings
    config.max_retry_attempts = int(os.getenv('AGENT_LOG_MAX_RETRIES', str(config.max_retry_attempts)))
    config.retry_delay_seconds = float(os.getenv('AGENT_LOG_RETRY_DELAY', str(config.retry_delay_seconds)))
    config.enable_retry = os.getenv('AGENT_LOG_ENABLE_RETRY', 'false').lower() == 'true'
    
    # Environment settings
    env_str = os.getenv('AGENT_ENVIRONMENT', 'development')
    try:
        config.environment = LoggerEnvironment(env_str.lower())
    except ValueError:
        warnings.warn(f"Unknown environment: {env_str}")
    
    config.debug_mode = os.getenv('AGENT_DEBUG', 'false').lower() == 'true'
    config.performance_monitoring = os.getenv('AGENT_PERFORMANCE_MONITORING', 'false').lower() == 'true'
    
    # Migration settings
    config.migration_mode = os.getenv('AGENT_MIGRATION_MODE', 'false').lower() == 'true'
    config.deprecation_warnings = os.getenv('AGENT_DEPRECATION_WARNINGS', 'true').lower() == 'true'
    
    # Transport-specific configuration
    if config.transport_type == TransportType.HTTP:
        config.transport_config = {
            'endpoint_url': os.getenv('AGENT_HTTP_ENDPOINT'),
            'timeout': float(os.getenv('AGENT_HTTP_TIMEOUT', '30.0')),
            'headers': {}
        }
        
        # Parse headers from environment
        auth_header = os.getenv('AGENT_HTTP_AUTH_HEADER')
        if auth_header:
            config.transport_config['headers']['Authorization'] = auth_header
    
    elif config.transport_type == TransportType.QUEUE:
        config.transport_config = {
            'queue_name': os.getenv('AGENT_QUEUE_NAME'),
            'connection_string': os.getenv('AGENT_QUEUE_CONNECTION')
        }
    
    elif config.transport_type == TransportType.FILE:
        config.transport_config = {
            'file_path': os.getenv('AGENT_FILE_TRANSPORT_PATH', 'transport_events.jsonl')
        }
    
    return config


def create_session_logger(
    log_base_dir: Optional[str] = None,
    downstream_callback: Optional[Callable] = None,
    **kwargs
) -> EventSessionLogger:
    """
    Create EventSessionLogger with basic configuration.
    
    Args:
        log_base_dir: Base directory for logs
        downstream_callback: Optional callback for transition
        **kwargs: Additional configuration options
        
    Returns:
        Configured EventSessionLogger instance
    """
    config = _load_default_config()
    if log_base_dir:
        config['log_base_dir'] = log_base_dir
    if downstream_callback:
        config['downstream_callback'] = downstream_callback
    config.update(kwargs)
    
    return EventSessionLogger(**config)


def create_logging_only(
    log_base_dir: Optional[str] = None,
    **kwargs
) -> EventSessionLogger:
    """
    Create EventSessionLogger for local logging only (no downstream).
    
    Useful for:
    - Development and testing
    - Simple logging scenarios
    - Migration testing
    
    Args:
        log_base_dir: Base directory for logs
        **kwargs: Additional configuration options
        
    Returns:
        EventSessionLogger configured for local logging only
    """
    return create_session_logger(
        log_base_dir=log_base_dir,
        downstream_callback=None,
        downstream_transport=None,
        **kwargs
    )


def create_with_callback(
    callback: Callable,
    log_base_dir: Optional[str] = None,
    **kwargs
) -> EventSessionLogger:
    """
    Create EventSessionLogger with downstream callback (transition pattern).
    
    Args:
        callback: Downstream callback function
        log_base_dir: Base directory for logs
        **kwargs: Additional configuration options
        
    Returns:
        EventSessionLogger configured with callback
    """
    return create_session_logger(
        log_base_dir=log_base_dir,
        downstream_callback=callback,
        **kwargs
    )


def create_with_transport(
    transport: TransportInterface,
    log_base_dir: Optional[str] = None,
    **kwargs
) -> EventSessionLogger:
    """
    Create EventSessionLogger with transport layer (future pattern).
    
    Args:
        transport: Transport interface implementation
        log_base_dir: Base directory for logs
        **kwargs: Additional configuration options
        
    Returns:
        EventSessionLogger configured with transport
    """
    return create_session_logger(
        log_base_dir=log_base_dir,
        downstream_transport=transport,
        **kwargs
    )


def create_from_environment(**overrides) -> EventSessionLogger:
    """
    Create EventSessionLogger from environment variables.
    
    Args:
        **overrides: Override specific configuration values
        
    Returns:
        EventSessionLogger configured from environment
    """
    config = _load_default_config()
    config.update(overrides)
    return EventSessionLogger(**config)


def migrate_from_session_logger(
    old_session_logger: Any,  # Avoid circular import, use Any for SessionLogger
    downstream_callback: Optional[Callable] = None
) -> EventSessionLogger:
    """
    Create EventSessionLogger with configuration migrated from old SessionLogger.
    
    Args:
        old_session_logger: Existing SessionLogger instance
        downstream_callback: Optional downstream callback
        
    Returns:
        EventSessionLogger with migrated configuration
    """
    # Extract configuration from old SessionLogger
    log_base_dir = None
    include_system_prompt = True
    
    try:
        # Try to extract log_base_dir from old SessionLogger
        if hasattr(old_session_logger, 'log_file_path'):
            log_base_dir = str(old_session_logger.log_file_path.parent.parent)
        
        # Try to extract include_system_prompt
        if hasattr(old_session_logger, 'include_system_prompt'):
            include_system_prompt = old_session_logger.include_system_prompt
    except Exception:
        # Fallback to defaults if extraction fails
        pass
    
    return create_session_logger(
        log_base_dir=log_base_dir,
        include_system_prompt=include_system_prompt,
        downstream_callback=downstream_callback
    )


def create_backward_compatible(**kwargs) -> EventSessionLogger:
    """
    Create EventSessionLogger with maximum backward compatibility.
    
    Maintains all current SessionLogger behavior while adding gateway capabilities.
    
    Args:
        **kwargs: Additional configuration options
        
    Returns:
        EventSessionLogger with backward compatible configuration
    """
    config = {
        'log_format': "jsonl",  # Match current format
        'file_naming_pattern': "%Y%m%d_%H%M%S",  # Match current naming
        'include_system_prompt': True,  # Match current default
        'enable_local_logging': True,  # Always enable
    }
    config.update(kwargs)
    
    return create_session_logger(**config)


# Enhanced Factory Functions and Utilities


def _create_transport(transport_type: TransportType, config: Dict[str, Any]) -> TransportInterface:
    """Create transport instance based on type and configuration"""
    if transport_type == TransportType.CALLBACK:
        callback = config.get('callback')
        if not callback:
            raise EventSessionLoggerError("Callback transport requires 'callback' in config")
        return CallbackTransport(callback)
    
    elif transport_type == TransportType.LOGGING:
        logger_name = config.get('logger_name', 'event_transport')
        log_level = config.get('log_level', logging.INFO)
        return LoggingTransport(logger_name, log_level)
    
    elif transport_type == TransportType.NULL:
        return NullTransport()
    
    elif transport_type == TransportType.FILE:
        file_path = config.get('file_path')
        if not file_path:
            raise EventSessionLoggerError("File transport requires 'file_path' in config")
        format_type = config.get('format', 'jsonl')
        return FileTransport(file_path, format_type)
    
    elif transport_type == TransportType.HTTP:
        endpoint_url = config.get('endpoint_url')
        if not endpoint_url:
            raise EventSessionLoggerError("HTTP transport requires 'endpoint_url' in config")
        headers = config.get('headers', {})
        timeout = config.get('timeout', 30.0)
        return HTTPTransport(endpoint_url, headers, timeout)
    
    elif transport_type == TransportType.QUEUE:
        queue_name = config.get('queue_name')
        connection_string = config.get('connection_string')
        if not queue_name or not connection_string:
            raise EventSessionLoggerError("Queue transport requires 'queue_name' and 'connection_string' in config")
        return QueueTransport(queue_name, connection_string)
    
    else:
        raise EventSessionLoggerError(f"Unknown transport type: {transport_type}")


def create_logger_from_config(config: LoggerConfiguration) -> EventSessionLogger:
    """Create EventSessionLogger from comprehensive configuration"""
    # Validate configuration
    errors = config.validate()
    if errors:
        raise EventSessionLoggerError(f"Configuration validation failed: {', '.join(errors)}")
    
    # Create transport if specified
    transport = None
    if config.transport_type:
        transport = _create_transport(config.transport_type, config.transport_config)
        
        # Wrap with retry transport if enabled
        if config.enable_retry:
            fallback_transport = None
            if config.fallback_transport_type:
                fallback_transport = _create_transport(
                    config.fallback_transport_type, 
                    config.fallback_transport_config
                )
            
            transport = RetryTransport(
                transport,
                max_retries=config.max_retry_attempts,
                base_delay=config.retry_delay_seconds,
                fallback_transport=fallback_transport
            )
    
    # Create error handler for debugging
    error_handler = None
    if config.debug_mode:
        def debug_error_handler(error: Exception, context: str) -> None:
            logging_manager = LoggingManager("EventSessionLogger.Debug")
            logger = logging_manager.get_logger()
            logger.error(f"Error in {context}: {error}", exc_info=True)

        error_handler = debug_error_handler
    
    # Create EventSessionLogger
    return EventSessionLogger(
        log_base_dir=config.log_base_dir,
        downstream_transport=transport,
        include_system_prompt=config.include_system_prompt,
        log_format=config.log_format,
        file_naming_pattern=config.file_naming_pattern,
        error_handler=error_handler,
        max_retry_attempts=config.max_retry_attempts,
        retry_delay_seconds=config.retry_delay_seconds,
        enable_local_logging=config.enable_local_logging,
        session_directory_pattern=config.session_directory_pattern,
        unknown_session_pattern=config.unknown_session_pattern
    )


# Environment-Specific Factory Functions

def create_development_logger(
    log_base_dir: Optional[str] = None,
    enable_debug: bool = True,
    transport_type: TransportType = TransportType.LOGGING
) -> EventSessionLogger:
    """Create EventSessionLogger optimized for development"""
    config = LoggerConfiguration(
        log_base_dir=log_base_dir or "logs/dev",
        environment=LoggerEnvironment.DEVELOPMENT,
        debug_mode=enable_debug,
        transport_type=transport_type,
        transport_config={'logger_name': 'dev_events', 'log_level': logging.DEBUG},
        deprecation_warnings=True,
        performance_monitoring=True
    )
    return create_logger_from_config(config)


def create_testing_logger(
    log_base_dir: Optional[str] = None,
    transport_type: TransportType = TransportType.NULL
) -> EventSessionLogger:
    """Create EventSessionLogger optimized for testing"""
    config = LoggerConfiguration(
        log_base_dir=log_base_dir or "logs/test",
        environment=LoggerEnvironment.TESTING,
        debug_mode=False,
        transport_type=transport_type,
        deprecation_warnings=False,
        performance_monitoring=False
    )
    return create_logger_from_config(config)


def create_production_logger(
    log_base_dir: str,
    transport_type: TransportType,
    transport_config: Dict[str, Any],
    enable_retry: bool = True,
    fallback_to_file: bool = True
) -> EventSessionLogger:
    """Create EventSessionLogger optimized for production"""
    config = LoggerConfiguration(
        log_base_dir=log_base_dir,
        environment=LoggerEnvironment.PRODUCTION,
        debug_mode=False,
        transport_type=transport_type,
        transport_config=transport_config,
        enable_retry=enable_retry,
        max_retry_attempts=5,
        retry_delay_seconds=2.0,
        deprecation_warnings=False,
        performance_monitoring=True
    )
    
    # Add file fallback for production reliability
    if fallback_to_file:
        config.fallback_transport_type = TransportType.FILE
        config.fallback_transport_config = {
            'file_path': f"{log_base_dir}/fallback_events.jsonl"
        }
    
    return create_logger_from_config(config)


# Migration and Compatibility Functions

def create_migration_logger(
    old_session_logger: Any,
    callback: Optional[Callable] = None,
    enable_warnings: bool = True
) -> EventSessionLogger:
    """Create EventSessionLogger for migration from old SessionLogger"""
    config = LoggerConfiguration(
        migration_mode=True,
        deprecation_warnings=enable_warnings,
        transport_type=TransportType.CALLBACK if callback else None,
        transport_config={'callback': callback} if callback else {}
    )
    
    # Extract configuration from old SessionLogger
    try:
        if hasattr(old_session_logger, 'log_file_path'):
            config.log_base_dir = str(old_session_logger.log_file_path.parent.parent)
        
        if hasattr(old_session_logger, 'include_system_prompt'):
            config.include_system_prompt = old_session_logger.include_system_prompt
    except Exception:
        # Use defaults if extraction fails
        pass
    
    if enable_warnings:
        warnings.warn(
            "Using migration logger. Please update to use transport-based patterns.",
            DeprecationWarning,
            stacklevel=2
        )
    
    return create_logger_from_config(config)


# Specialized Factory Functions

def create_monitoring_logger(
    log_base_dir: str,
    monitoring_callback: Callable[[Dict[str, Any]], None]
) -> EventSessionLogger:
    """Create EventSessionLogger with performance monitoring"""
    import time
    
    class MonitoringTransport(CallbackTransport):
        def __init__(self, callback, monitoring_callback):
            super().__init__(callback)
            self.monitoring_callback = monitoring_callback
        
        async def send(self, event, metadata=None):
            start_time = time.time()
            result = await super().send(event, metadata)
            end_time = time.time()
            
            # Send monitoring data
            monitoring_data = {
                'event_type': getattr(event, 'type', 'unknown'),
                'success': result,
                'duration_ms': (end_time - start_time) * 1000,
                'timestamp': end_time,
                'transport': self.name
            }
            
            try:
                self.monitoring_callback(monitoring_data)
            except Exception:
                pass  # Don't fail on monitoring errors
            
            return result
    
    def dummy_callback(event):
        pass  # Monitoring transport needs a callback
    
    transport = MonitoringTransport(dummy_callback, monitoring_callback)
    
    return EventSessionLogger(
        log_base_dir=log_base_dir,
        downstream_transport=transport,
        performance_monitoring=True
    )


def create_multi_transport_logger(
    log_base_dir: str,
    transports: List[TransportInterface]
) -> EventSessionLogger:
    """Create EventSessionLogger that sends to multiple transports"""
    from .transports import TransportState
    
    class MultiTransport(TransportInterface):
        def __init__(self, transports: List[TransportInterface]):
            super().__init__("MultiTransport")
            self.transports = transports
            self.state = TransportState.CONNECTED
        
        async def send(self, event, metadata=None):
            results = []
            for transport in self.transports:
                try:
                    result = await transport.send(event, metadata)
                    results.append(result)
                except Exception:
                    results.append(False)
            
            # Return True if any transport succeeded
            return any(results)
        
        async def connect(self):
            for transport in self.transports:
                await transport.connect()
            return True
        
        async def disconnect(self):
            for transport in self.transports:
                await transport.disconnect()
        
        async def close(self):
            for transport in self.transports:
                await transport.close()
    
    multi_transport = MultiTransport(transports)
    
    return EventSessionLogger(
        log_base_dir=log_base_dir,
        downstream_transport=multi_transport
    )


# Utility Functions

def validate_logger_config(config: LoggerConfiguration) -> bool:
    """Validate logger configuration and print errors"""
    errors = config.validate()
    if errors:
        print("Configuration validation errors:")
        for error in errors:
            print(f"  - {error}")
        return False
    return True


def print_logger_info(logger: EventSessionLogger) -> None:
    """Print information about logger configuration"""
    print(f"EventSessionLogger Configuration:")
    print(f"  Log Directory: {logger.log_base_dir}")
    print(f"  Local Logging: {logger.enable_local_logging}")
    print(f"  Downstream Callback: {'Yes' if logger.downstream_callback else 'No'}")
    print(f"  Downstream Transport: {logger.downstream_transport.name if logger.downstream_transport else 'None'}")
    print(f"  Include System Prompt: {logger.include_system_prompt}")
    print(f"  Max Retry Attempts: {logger.max_retry_attempts}")
    print(f"  Retry Delay: {logger.retry_delay_seconds}s")


def create_logger_with_validation(config: LoggerConfiguration) -> EventSessionLogger:
    """Create logger with configuration validation and info printing"""
    if not validate_logger_config(config):
        raise EventSessionLoggerError("Configuration validation failed")
    
    logger = create_logger_from_config(config)
    print_logger_info(logger)
    return logger