"""
StructuredLoggerFactory - Central logger creation and configuration

This module provides the core factory pattern for creating structured loggers
with consistent configuration across the Agent C framework.
"""

import os
import logging
import threading
from typing import Optional, Protocol, Dict

import structlog

from ..logging_utils import LoggingManager


class LoggerProtocol(Protocol):
    """Protocol defining the interface that both structured and legacy loggers must implement."""
    
    def debug(self, msg: str, *args, **kwargs) -> None: ...
    def info(self, msg: str, *args, **kwargs) -> None: ...
    def warning(self, msg: str, *args, **kwargs) -> None: ...
    def error(self, msg: str, *args, **kwargs) -> None: ...
    def critical(self, msg: str, *args, **kwargs) -> None: ...
    def exception(self, msg: str, *args, **kwargs) -> None: ...


class StructuredLoggerFactory:
    """
    Factory for creating structured loggers with consistent configuration.
    
    This factory provides a centralized way to create loggers that can operate
    in either structured mode (using structlog) or legacy mode (using LoggingManager)
    based on configuration and feature flags.
    
    Features:
    - Singleton pattern for consistent configuration
    - Environment-aware setup (development vs production)
    - Backward compatibility with LoggingManager
    - Logger caching for performance
    - Feature flag support for gradual rollout
    """
    
    _instance: Optional["StructuredLoggerFactory"] = None
    _lock = threading.Lock()
    _configured = False
    _logger_cache: Dict[str, LoggerProtocol] = {}
    
    def __new__(cls) -> "StructuredLoggerFactory":
        """Ensure singleton pattern."""
        if cls._instance is None:
            with cls._lock:
                if cls._instance is None:
                    cls._instance = super().__new__(cls)
        return cls._instance
    
    def __init__(self):
        """Initialize the factory (only once due to singleton)."""
        if not self._configured:
            self._configure_structlog()
            self._configured = True
    
    def _configure_structlog(self) -> None:
        """Configure structlog with appropriate processors and formatters."""
        # Determine if we're in development or production
        is_development = self._is_development_environment()
        
        # Import processors here to avoid circular imports
        from .processors import (
            add_framework_context,
            add_correlation_id,
            add_timing_info,
            enrich_errors,
            add_agent_context,
            filter_sensitive_data
        )
        
        # Build processor chain
        processors = [
            # Essential structlog processors for stdlib compatibility
            structlog.stdlib.filter_by_level,
            structlog.stdlib.add_log_level,
            structlog.stdlib.PositionalArgumentsFormatter(),
            
            # Our custom processors
            add_framework_context,
            add_correlation_id,
            add_timing_info,
            enrich_errors,
            add_agent_context,
            filter_sensitive_data,
            
            # Final processing
            structlog.processors.format_exc_info,
            structlog.processors.UnicodeDecoder(),
        ]
        
        # Add appropriate final renderer
        if is_development:
            processors.append(structlog.dev.ConsoleRenderer())
        else:
            processors.append(structlog.processors.JSONRenderer())
        
        # Configure structlog
        structlog.configure(
            processors=processors,
            wrapper_class=structlog.stdlib.BoundLogger,
            logger_factory=structlog.stdlib.LoggerFactory(),
            cache_logger_on_first_use=True,
        )
        
        # Configure the standard library root logger
        root_logger = logging.getLogger()
        
        if not root_logger.handlers:
            handler = logging.StreamHandler()
            # Use simple formatter since structlog handles the formatting
            handler.setFormatter(logging.Formatter('%(message)s'))
            root_logger.addHandler(handler)
            root_logger.setLevel(logging.INFO)
    
    def _is_development_environment(self) -> bool:
        """Determine if we're running in development environment."""
        # Check various environment indicators
        env = os.getenv("ENVIRONMENT", "").lower()
        debug_mode = os.getenv("DEBUG", "").lower() in ("true", "1", "yes")
        dev_mode = os.getenv("DEV", "").lower() in ("true", "1", "yes")
        
        # Check if we're in a development-like environment
        is_dev = (
            env in ("development", "dev", "local") or
            debug_mode or
            dev_mode or
            LoggingManager.is_debug_mode()
        )
        
        return is_dev
    
    def _should_use_structured_logging(self, logger_name: str) -> bool:
        """
        Determine if structured logging should be used for this logger.
        
        This method checks feature flags and configuration to decide whether
        to use structured logging or fall back to LoggingManager.
        
        Args:
            logger_name: Name of the logger being created
            
        Returns:
            bool: True if structured logging should be used
        """
        # Check global feature flag
        use_structured = os.getenv("USE_STRUCTURED_LOGGING", "true").lower() in ("true", "1", "yes")
        
        # Check per-module feature flags (for gradual rollout)
        module_flag = os.getenv(f"STRUCTURED_LOGGING_{logger_name.upper().replace('.', '_')}")
        if module_flag is not None:
            use_structured = module_flag.lower() in ("true", "1", "yes")
        
        return use_structured
    
    def get_logger(self, name: str) -> LoggerProtocol:
        """
        Get a logger instance with the specified name.
        
        This method returns either a structured logger or a legacy LoggingManager
        logger based on configuration and feature flags.
        
        Args:
            name: Logger name, typically __name__ from the calling module
            
        Returns:
            LoggerProtocol: A logger instance (structured or legacy)
        """
        # Check cache first
        if name in self._logger_cache:
            return self._logger_cache[name]
        
        # Determine which type of logger to create
        if self._should_use_structured_logging(name):
            logger = self._create_structured_logger(name)
        else:
            logger = self._create_legacy_logger(name)
        
        # Cache the logger
        self._logger_cache[name] = logger
        
        return logger
    
    def _create_structured_logger(self, name: str) -> LoggerProtocol:
        """Create a structured logger using structlog."""
        return structlog.get_logger(name)
    
    def _create_legacy_logger(self, name: str) -> LoggerProtocol:
        """Create a legacy logger using LoggingManager."""
        logging_manager = LoggingManager(name)
        return logging_manager.get_logger()
    
    def clear_cache(self) -> None:
        """Clear the logger cache. Useful for testing."""
        with self._lock:
            self._logger_cache.clear()
    
    @classmethod
    def reset(cls) -> None:
        """Reset the singleton instance. Useful for testing."""
        with cls._lock:
            cls._instance = None
            cls._configured = False
            cls._logger_cache.clear()


# Global factory instance
_factory = StructuredLoggerFactory()


def get_logger(name: str) -> LoggerProtocol:
    """
    Convenience function to get a logger instance.
    
    This is the primary entry point for getting loggers in the Agent C framework.
    It provides a simple interface that maintains backward compatibility while
    enabling structured logging capabilities.
    
    Args:
        name: Logger name, typically __name__ from the calling module
        
    Returns:
        LoggerProtocol: A logger instance
        
    Example:
        from agent_c.util.structured_logging import get_logger
        
        logger = get_logger(__name__)
        logger.info("Operation completed", user_id="user-123", operation="create_session")
    """
    return _factory.get_logger(name)