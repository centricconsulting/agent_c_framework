"""
Compatibility Layer for Structured Logging

This module provides backward compatibility with the existing LoggingManager
while enabling gradual migration to structured logging.
"""

import logging
import os
import threading
from typing import Any, Dict, Optional, Union

import structlog

from .factory import StructuredLoggerFactory
from ..logging_utils import LoggingManager


class StructuredLoggingAdapter:
    """
    Adapter that provides the LoggingManager interface while using structured logging underneath.
    
    This adapter maintains 100% API compatibility with LoggingManager, allowing
    gradual migration to structured logging without breaking existing code.
    """
    
    def __init__(self, logger_name: str):
        """
        Initialize the adapter for a specific module.
        
        Args:
            logger_name (str): Name of the logger, typically __name__ from the calling module
        """
        self.logger_name = logger_name
        self._logger: Optional[logging.Logger] = None
        self._structured_logger: Optional[Any] = None
        self._use_structured = True

    def _get_logger(self) -> logging.Logger:
        """
        Get the appropriate logger (structured or traditional).
        
        Returns:
            logging.Logger: The configured logger
        """
        if self._logger is None:
            if self._use_structured:
                # Use structured logging
                factory = StructuredLoggerFactory()
                self._structured_logger = factory.get_logger(self.logger_name)
                # The factory returns a structlog logger, but we need to get its stdlib logger
                self._logger = structlog.stdlib.get_logger(self.logger_name)
            else:
                # Use traditional LoggingManager
                manager = LoggingManager(self.logger_name)
                self._logger = manager.get_logger()
                
        return self._logger
    
    def get_logger(self) -> logging.Logger:
        """
        Get the configured logger instance.
        
        Returns:
            logging.Logger: The configured logger
        """
        return self._get_logger()
    
    # Delegate all LoggingManager class methods
    @classmethod
    def configure_root_logger(cls) -> None:
        """
        Configure the root logger with consistent formatting.
        This should be called once at application startup.
        """
        LoggingManager.configure_root_logger()
    
    @classmethod
    def configure_external_loggers(cls, logger_levels=None):
        """
        Configure external library loggers to appropriate levels.
        
        Args:
            logger_levels (dict, optional): Dictionary mapping logger names to their desired levels.
        """
        LoggingManager.configure_external_loggers(logger_levels)
    
    @staticmethod
    def get_debug_event() -> threading.Event:
        """
        Get the shared debug event for coordination across modules.
        
        Returns:
            threading.Event: The debug event
        """
        return LoggingManager.get_debug_event()
    
    @staticmethod
    def set_debug_mode(enabled: bool = True) -> None:
        """
        Set the debug mode state.
        
        Args:
            enabled (bool): Whether debug mode should be enabled
        """
        LoggingManager.set_debug_mode(enabled)
    
    @staticmethod
    def is_debug_mode() -> bool:
        """
        Check if debug mode is enabled.
        
        Returns:
            bool: True if debug mode is enabled, False otherwise
        """
        return LoggingManager.is_debug_mode()


class StructuredLoggingMonkeyPatch:
    """
    Utilities for monkey-patching existing LoggingManager usage to use structured logging.
    
    This allows for gradual migration without changing import statements.
    """
    
    _original_logging_manager = None
    _patched = False
    
    @classmethod
    def patch_logging_manager(cls) -> None:
        """
        Monkey-patch LoggingManager to use StructuredLoggingAdapter.
        
        This allows existing code that imports LoggingManager to automatically
        use structured logging without any code changes.
        """
        if cls._patched:
            return
            
        # Store the original class
        cls._original_logging_manager = LoggingManager
        
        # Replace LoggingManager with our adapter
        import agent_c.util.logging_utils as logging_utils_module
        logging_utils_module.LoggingManager = StructuredLoggingAdapter
        
        cls._patched = True
    
    @classmethod
    def unpatch_logging_manager(cls) -> None:
        """
        Restore the original LoggingManager.
        
        This is primarily useful for testing or rollback scenarios.
        """
        if not cls._patched or cls._original_logging_manager is None:
            return
            
        # Restore the original class
        import agent_c.util.logging_utils as logging_utils_module
        logging_utils_module.LoggingManager = cls._original_logging_manager
        
        cls._patched = False
    
    @classmethod
    def is_patched(cls) -> bool:
        """
        Check if LoggingManager is currently patched.
        
        Returns:
            bool: True if patched, False otherwise
        """
        return cls._patched


def enable_structured_logging_globally() -> None:
    """
    Enable structured logging globally by setting environment variables and patching.
    
    This is a convenience function for enabling structured logging across
    the entire application.
    """
    os.environ['USE_STRUCTURED_LOGGING'] = 'true'
    StructuredLoggingMonkeyPatch.patch_logging_manager()


def disable_structured_logging_globally() -> None:
    """
    Disable structured logging globally and restore original behavior.
    
    This is useful for rollback scenarios or testing.
    """
    os.environ.pop('USE_STRUCTURED_LOGGING', None)
    StructuredLoggingMonkeyPatch.unpatch_logging_manager()


def enable_structured_logging_for_module(module_name: str) -> None:
    """
    Enable structured logging for a specific module only.
    
    Args:
        module_name (str): The module name to enable structured logging for
    """
    os.environ['USE_STRUCTURED_LOGGING'] = 'true'
    os.environ['STRUCTURED_LOGGING_MODULE_NAME'] = module_name


def get_migration_status() -> Dict[str, Any]:
    """
    Get the current migration status and configuration.
    
    Returns:
        Dict[str, Any]: Status information about structured logging migration
    """
    return {
        'structured_logging_enabled': os.getenv('USE_STRUCTURED_LOGGING', 'false').lower() == 'true',
        'module_override': os.getenv('STRUCTURED_LOGGING_MODULE_NAME'),
        'monkey_patch_active': StructuredLoggingMonkeyPatch.is_patched(),
        'feature_flags': {
            'USE_STRUCTURED_LOGGING': os.getenv('USE_STRUCTURED_LOGGING'),
            'STRUCTURED_LOGGING_MODULE_NAME': os.getenv('STRUCTURED_LOGGING_MODULE_NAME'),
        }
    }


# Convenience function for creating loggers with automatic compatibility
def get_compatible_logger(name: str) -> logging.Logger:
    """
    Get a logger that automatically uses structured logging if enabled, otherwise traditional logging.
    
    Args:
        name (str): Logger name
        
    Returns:
        logging.Logger: Compatible logger instance
    """
    adapter = StructuredLoggingAdapter(name)
    return adapter.get_logger()