"""
Formatters for Structured Logging

This module provides formatters for different environments and output formats,
building on the existing ColoredFormatter while adding structured capabilities.
"""

import json
import logging
from typing import Any, Dict
from ..logging_utils import ColoredFormatter


class StructuredConsoleFormatter(ColoredFormatter):
    """
    Enhanced console formatter that builds on the existing ColoredFormatter
    while adding structured data support for development environments.
    
    This formatter maintains the familiar colored output while displaying
    structured context in a readable format.
    """
    
    def __init__(self, fmt: str = None):
        """
        Initialize the structured console formatter.
        
        Args:
            fmt: Format string for log messages. If None, uses a default format.
        """
        if fmt is None:
            fmt = '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
        super().__init__(fmt)
    
    def format(self, record: logging.LogRecord) -> str:
        """
        Format log record with colors and structured data.
        
        Args:
            record: The log record to format
            
        Returns:
            str: Formatted log message with colors and structured data
        """
        # Use the parent ColoredFormatter for basic formatting
        base_message = super().format(record)
        
        # Add structured data if present
        if hasattr(record, 'structured_data') and record.structured_data:
            structured_parts = []
            
            # Format key-value pairs in a readable way
            for key, value in record.structured_data.items():
                if key not in ('event', 'message', 'level', 'logger', 'timestamp'):
                    if isinstance(value, str):
                        structured_parts.append(f"{key}={value}")
                    else:
                        structured_parts.append(f"{key}={json.dumps(value)}")
            
            if structured_parts:
                # Add structured data in a visually distinct way
                structured_str = " | ".join(structured_parts)
                base_message += f" [{structured_str}]"
        
        return base_message


class StructuredJSONFormatter(logging.Formatter):
    """
    JSON formatter for production environments.
    
    This formatter outputs structured logs as JSON for easy parsing
    by log aggregation systems and monitoring tools.
    """
    
    def format(self, record: logging.LogRecord) -> str:
        """
        Format log record as JSON.
        
        Args:
            record: The log record to format
            
        Returns:
            str: JSON formatted log message
        """
        # Start with basic log record data
        log_data = {
            'timestamp': self.formatTime(record),
            'level': record.levelname,
            'logger': record.name,
            'message': record.getMessage(),
        }
        
        # Add structured data if present
        if hasattr(record, 'structured_data') and record.structured_data:
            log_data.update(record.structured_data)
        
        # Add exception information if present
        if record.exc_info:
            if record.exc_info is True:
                # When exc_info is True, we need to get the current exception info
                import sys
                exc_info = sys.exc_info()
                if exc_info[0] is not None:
                    log_data['exception'] = self.formatException(exc_info)
            else:
                # exc_info is a tuple (type, value, traceback)
                log_data['exception'] = self.formatException(record.exc_info)
        
        # Add stack info if present
        if record.stack_info:
            log_data['stack_info'] = record.stack_info
        
        return json.dumps(log_data, default=str, ensure_ascii=False)


class CompatibilityFormatter(logging.Formatter):
    """
    Compatibility formatter that bridges LoggingManager and structured logging.
    
    This formatter can handle both traditional log records and structured
    log records, providing a smooth transition path.
    """
    
    def __init__(self, use_colors: bool = True):
        """
        Initialize the compatibility formatter.
        
        Args:
            use_colors: Whether to use colored output (for development)
        """
        super().__init__()
        self.use_colors = use_colors
        if use_colors:
            self.colored_formatter = ColoredFormatter()
    
    def format(self, record: logging.LogRecord) -> str:
        """
        Format log record with compatibility support.
        
        Args:
            record: The log record to format
            
        Returns:
            str: Formatted log message
        """
        # Check if this is a structured log record
        if hasattr(record, 'structured_data') and record.structured_data:
            # Use structured formatting
            if self.use_colors:
                formatter = StructuredConsoleFormatter()
            else:
                formatter = StructuredJSONFormatter()
            return formatter.format(record)
        else:
            # Use traditional formatting
            if self.use_colors:
                return self.colored_formatter.format(record)
            else:
                return super().format(record)


def get_console_formatter() -> logging.Formatter:
    """
    Get the console formatter for development environments.
    
    Returns:
        logging.Formatter: Console formatter with colors and structured data support
    """
    return StructuredConsoleFormatter()


def get_json_formatter() -> logging.Formatter:
    """
    Get the JSON formatter for production environments.
    
    Returns:
        logging.Formatter: JSON formatter for structured logs
    """
    return StructuredJSONFormatter()


def get_compatibility_formatter(use_colors: bool = True) -> logging.Formatter:
    """
    Get the compatibility formatter for mixed environments.
    
    Args:
        use_colors: Whether to use colored output
        
    Returns:
        logging.Formatter: Compatibility formatter
    """
    return CompatibilityFormatter(use_colors=use_colors)


# Custom structlog processor to prepare data for standard library logging
def prepare_for_stdlib(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Prepare structured data for standard library logging.
    
    This processor extracts the message and stores structured data
    in a format that can be used by standard library formatters.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Processed event dictionary
    """
    # Extract the main message
    message = event_dict.pop('event', '')
    
    # Store remaining data as structured_data
    structured_data = {k: v for k, v in event_dict.items() if k != 'event'}
    
    return {
        'event': message,
        'structured_data': structured_data
    }