# agent_c_api/core/util/logging_utils.py
import logging
import os
import sys
from typing import Optional, Dict, Any
import threading
#
# import platform
# if platform.system() == 'Windows':
#     import colorama
#     colorama.init()

# Global event for debug mode
_debug_event = threading.Event()


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
    Centralized logging manager for the application that ensures consistent
    logging configuration across all modules.

    This class provides a unified approach to logging by:
    1. Creating loggers with consistent formatting
    2. Supporting different log levels based on environment
    3. Managing debug mode through a shared event
    4. Supporting both file and console logging

    Attributes:
        logger_name (str): Name of the logger to create/retrieve
        _logger (logging.Logger): The configured logger instance
    """

    # Class variables for shared configuration
    # LOG_FORMAT = '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
    LOG_FORMAT = '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
    LOG_LEVEL = os.getenv('LOG_LEVEL', 'DEBUG').upper() # This is where we can change debug log levels.
    FILE_LOG_ENABLED = os.getenv('FILE_LOG_ENABLED', 'true').lower() == 'true'
    LOG_FILE = os.getenv('LOG_FILE', 'logs\\agent_c_api.log')

    # Track created loggers to avoid duplicate handlers
    _loggers: Dict[str, logging.Logger] = {}

    def __init__(self, logger_name: str):
        """
        Initialize the logging manager for a specific module.

        Args:
            logger_name (str): Name of the logger, typically __name__ from the calling module
        """
        self.logger_name = logger_name
        self._logger = self._get_or_create_logger()

    def _get_or_create_logger(self) -> logging.Logger:
        """
        Get an existing logger or create a new one with the proper configuration.

        Returns:
            logging.Logger: Configured logger instance
        """
        # Return existing logger if already created
        if self.logger_name in LoggingManager._loggers:
            return LoggingManager._loggers[self.logger_name]

        # Create new logger
        logger = logging.getLogger(self.logger_name)

        # Set log level
        try:
            logger.setLevel(getattr(logging, self.LOG_LEVEL))
        except AttributeError:
            logger.setLevel(logging.INFO)
            print(f"Invalid LOG_LEVEL '{self.LOG_LEVEL}', defaulting to INFO")

        # IMPORTANT: Remove existing handlers to prevent duplicates
        if logger.handlers:
            logger.handlers.clear()

        # Also disable propagation to prevent duplicate logs from parent loggers
        logger.propagate = False


        # Only add handlers if none exist to prevent duplicates
        if not logger.handlers:
            # Console handler
            console_handler = logging.StreamHandler(sys.stdout)
            # console_handler.setFormatter(logging.Formatter(self.LOG_FORMAT))
            console_handler.setFormatter(ColoredFormatter(self.LOG_FORMAT))
            logger.addHandler(console_handler)

            # File handler (optional)
            if self.FILE_LOG_ENABLED:
                try:
                    # Create directory if it doesn't exist
                    log_dir = os.path.dirname(self.LOG_FILE)
                    if log_dir:  # Only try to create directory if there is one specified
                        os.makedirs(log_dir, exist_ok=True)

                    file_handler = logging.FileHandler(self.LOG_FILE, encoding='utf-8')
                    file_handler.setFormatter(logging.Formatter(self.LOG_FORMAT))
                    logger.addHandler(file_handler)
                except (IOError, PermissionError) as e:
                    print(f"Could not create log file {self.LOG_FILE}: {e}")

        # Store logger for reuse
        LoggingManager._loggers[self.logger_name] = logger
        return logger

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
        Configure the root logger with consistent formatting.
        This should be called once at application startup.
        """
        root_logger = logging.getLogger()

        # Set the root logger level
        try:
            root_logger.setLevel(getattr(logging, cls.LOG_LEVEL))
        except AttributeError:
            root_logger.setLevel(logging.INFO)

        # Only add handlers if none exist
        if not root_logger.handlers:
            # Console handler
            console_handler = logging.StreamHandler(sys.stdout)
            # console_handler.setFormatter(logging.Formatter(cls.LOG_FORMAT))
            console_handler.setFormatter(ColoredFormatter(cls.LOG_FORMAT))
            root_logger.addHandler(console_handler)

            # File handler (optional)
            if cls.FILE_LOG_ENABLED:
                try:
                    # Create directory if it doesn't exist
                    log_dir = os.path.dirname(cls.LOG_FILE)
                    if log_dir:  # Only try to create directory if there is one specified
                        os.makedirs(log_dir, exist_ok=True)

                    file_handler = logging.FileHandler(cls.LOG_FILE, encoding='utf-8')
                    file_handler.setFormatter(logging.Formatter(cls.LOG_FORMAT))
                    root_logger.addHandler(file_handler)
                except (IOError, PermissionError) as e:
                    print(f"Could not create log file {cls.LOG_FILE}: {e}")

    @classmethod
    def configure_external_loggers(cls, logger_levels=None):
        """
        Configure external library loggers to appropriate levels.

        Args:
            logger_levels (dict, optional): Dictionary mapping logger names to their desired levels.
                Example: {"httpx": "WARNING", "uvicorn.access": "ERROR"}
        """
        # Default configuration for common noisy loggers
        default_levels = {
            "httpx": "WARNING",
            "urllib3": "WARNING",
            "uvicorn.access": "WARNING",
            "asyncio": "WARNING",
            "httpcore": "WARNING",
            "python_multipart": "WARNING",
            "anthropic": "WARNING",
            "openai": "WARNING",
            "readability": "WARNING",
            "selenium": "WARNING",
            "mysql": "WARNING",
            # Add others as needed
        }

        # Update with any custom settings
        if logger_levels:
            default_levels.update(logger_levels)

        # Apply the configuration
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