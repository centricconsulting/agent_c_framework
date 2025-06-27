# agent_c_api/core/util/logging_utils.py
# 
# ⚠️  DEPRECATED MODULE ⚠️
# 
# This module is deprecated in favor of agent_c.util.structured_logging.
# It now provides a compatibility wrapper for gradual migration.
# 
# MIGRATION GUIDE:
# 
# Old pattern:
#   from agent_c_api.core.util.logging_utils import LoggingManager
#   logging_manager = LoggingManager(__name__)
#   logger = logging_manager.get_logger()
# 
# New pattern:
#   from agent_c.util.structured_logging import get_logger, LoggingContext
#   logger = get_logger(__name__)
#   # Use LoggingContext for contextual logging where appropriate
# 
# Benefits of structured logging:
# - Better performance and reduced memory usage
# - Support for structured key-value logging
# - Built-in context management
# - More flexible configuration options
#

import logging
import os
import sys
from typing import Optional, Dict, Any
import threading
import warnings

from agent_c.util.structured_logging import get_logger

# Global event for debug mode
_debug_event = threading.Event()

# Issue deprecation warning
warnings.warn(
    "agent_c_api.core.util.logging_utils is deprecated. "
    "Use agent_c.util.structured_logging instead.",
    DeprecationWarning,
    stacklevel=2
)


# ANSI color codes for terminal output
class Colors:
    RESET = "\033[0m"
    BLACK = "\033[30m"
    RED = "\033[31m"
    GREEN = "\033[32m"
    YELLOW = "\033[33m"
    BLUE = "\033[34m"
    MAGENTA = "\033[35m"
    CYAN = "\033[36m"
    WHITE = "\033[37m"
    BOLD = "\033[1m"
    UNDERLINE = "\033[4m"


# Define a colored formatter
class ColoredFormatter(logging.Formatter):
    """
    A formatter that adds colors to logs based on their level.
    """
    LEVEL_COLORS = {
        logging.DEBUG: Colors.BLUE,
        logging.INFO: Colors.GREEN,
        logging.WARNING: Colors.YELLOW,
        logging.ERROR: Colors.RED,
        logging.CRITICAL: Colors.BOLD + Colors.RED,
    }

    def format(self, record):
        # Get the original formatted message
        formatted_message = super().format(record)

        # Add color to the level name based on the level
        level_color = self.LEVEL_COLORS.get(record.levelno, Colors.RESET)

        # Color just the level name, keeping the rest of the formatting the same
        parts = formatted_message.split(" - ", 2)  # Split on first two ' - ' sequences
        if len(parts) >= 3:
            # Format: timestamp - logger - level - message
            timestamp, logger_name, rest = parts
            level_end = rest.find(" - ")
            if level_end > 0:
                level = rest[:level_end]
                message = rest[level_end:]
                return f"{timestamp} - {logger_name} - {level_color}{level}{Colors.RESET}{message}"

        # Fallback if the format doesn't match expected pattern
        return formatted_message


class LoggingManager:
    """
    DEPRECATED: Compatibility wrapper for LoggingManager.
    
    This class is deprecated in favor of agent_c.util.structured_logging.
    It now delegates to the structured logging system while maintaining
    backward compatibility.
    
    New code should use:
        from agent_c.util.structured_logging import get_logger
        logger = get_logger(__name__)
    """

    def __init__(self, logger_name: str):
        """
        Initialize the logging manager for a specific module.
        
        Args:
            logger_name (str): Name of the logger, typically __name__ from the calling module
        """
        warnings.warn(
            f"LoggingManager is deprecated. Use get_logger('{logger_name}') instead.",
            DeprecationWarning,
            stacklevel=2
        )
        self.logger_name = logger_name
        self._logger = get_logger(logger_name)
    
    def get_logger(self) -> logging.Logger:
        """
        Get the configured logger instance.
        
        Returns:
            logging.Logger: The configured logger
        """
        return self._logger

    @classmethod
    def configure_root_logger(cls) -> None:
        """
        DEPRECATED: Configure the root logger with consistent formatting.
        
        The structured logging system handles logger configuration automatically.
        This method is retained for backward compatibility but does nothing.
        """
        warnings.warn(
            "LoggingManager.configure_root_logger() is deprecated. "
            "The structured logging system handles configuration automatically.",
            DeprecationWarning,
            stacklevel=2
        )
        # No-op for compatibility

    @classmethod
    def configure_external_loggers(cls, logger_levels=None):
        """
        DEPRECATED: Configure external library loggers to appropriate levels.
        
        This functionality should be handled through the structured logging configuration.
        This method is retained for backward compatibility but functionality is limited.
        
        Args:
            logger_levels (dict, optional): Dictionary mapping logger names to their desired levels.
        """
        warnings.warn(
            "LoggingManager.configure_external_loggers() is deprecated. "
            "Configure external loggers through the structured logging system.",
            DeprecationWarning,
            stacklevel=2
        )
        
        # Maintain minimal functionality for compatibility
        default_levels = {
            "httpx": "WARNING",
            "urllib3": "WARNING",
            "uvicorn.access": "WARNING",
            "asyncio": "WARNING",
            "httpcore": "WARNING",
            "python_multipart": "WARNING",
            "anthropic": "WARNING",
            "openai": "WARNING",
        }
        
        if logger_levels:
            default_levels.update(logger_levels)
        
        for logger_name, level in default_levels.items():
            try:
                level_value = getattr(logging, level.upper())
                logging.getLogger(logger_name).setLevel(level_value)
            except (AttributeError, TypeError) as e:
                print(f"Error setting log level for {logger_name}: {e}")

    @staticmethod
    def get_debug_event() -> threading.Event:
        """
        Get the shared debug event for coordination across modules.

        Returns:
            threading.Event: The debug event
        """
        return _debug_event

    @staticmethod
    def set_debug_mode(enabled: bool = True) -> None:
        """
        Set the debug mode state.

        Args:
            enabled (bool): Whether debug mode should be enabled
        """
        if enabled:
            _debug_event.set()
        else:
            _debug_event.clear()

    @staticmethod
    def is_debug_mode() -> bool:
        """
        Check if debug mode is enabled.

        Returns:
            bool: True if debug mode is enabled, False otherwise
        """
        return _debug_event.is_set()