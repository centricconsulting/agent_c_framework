"""
Core Processors for Structured Logging

This module provides the processor chain that enriches log entries with
context, timing, error information, and framework-specific metadata.
"""

import time
import traceback
from typing import Any, Dict, List
from datetime import datetime

import structlog

from .context import get_context_dict


def add_framework_context(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Add framework-wide context to log entries.
    
    This processor automatically injects context variables (correlation_id, agent_id, etc.)
    into every log entry, providing consistent context across the framework.
    
    Args:
        logger: The logger instance
        method_name: The logging method name (debug, info, etc.)
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Enhanced event dictionary with framework context
    """
    # Get current context and merge it
    context = get_context_dict()
    
    # Add context to the event, but don't override explicit values
    for key, value in context.items():
        if key not in event_dict:
            event_dict[key] = value
    
    # Add framework metadata
    event_dict.setdefault('framework', 'agent_c')
    event_dict.setdefault('framework_version', '1.0.0')  # TODO: Get from package
    
    return event_dict


def add_correlation_id(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Ensure every log entry has a correlation ID.
    
    If no correlation ID is present in context, this processor can generate one
    or mark it as missing for debugging purposes.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Event dictionary with correlation ID
    """
    if 'correlation_id' not in event_dict:
        # Could generate a correlation ID here if needed
        # For now, we'll let it be None to indicate missing correlation
        pass
    
    return event_dict


def add_timing_info(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Add timing information to log entries.
    
    This processor adds timestamp information and can be extended to include
    operation timing and performance metrics.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Event dictionary with timing information
    """
    # Add ISO timestamp if not present
    if 'timestamp' not in event_dict:
        event_dict['timestamp'] = datetime.utcnow().isoformat() + 'Z'
    
    # Add high-precision timestamp for performance analysis
    if 'time_ns' not in event_dict:
        event_dict['time_ns'] = time.time_ns()
    
    return event_dict


def enrich_errors(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Enrich error log entries with additional context.
    
    This processor detects error conditions and adds helpful debugging information
    such as stack traces, error categorization, and recovery hints.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Event dictionary with error enrichment
    """
    # Check if this is an error-level log
    if method_name in ('error', 'critical', 'exception'):
        # Add error category if not present
        if 'error_category' not in event_dict:
            event_dict['error_category'] = 'unknown'
        
        # Add stack trace for exceptions if not present
        if 'exc_info' in event_dict and event_dict['exc_info']:
            if 'stack_trace' not in event_dict:
                event_dict['stack_trace'] = traceback.format_exc()
        
        # Add recovery hints based on error patterns
        if 'recovery_hint' not in event_dict:
            event_dict['recovery_hint'] = _get_recovery_hint(event_dict)
    
    return event_dict


def _get_recovery_hint(event_dict: Dict[str, Any]) -> str:
    """
    Generate recovery hints based on error patterns.
    
    Args:
        event_dict: The log event dictionary
        
    Returns:
        str: Recovery hint for the error
    """
    event = event_dict.get('event', '').lower()
    
    # Pattern-based recovery hints
    if 'connection' in event or 'redis' in event:
        return "Check Redis connection and retry"
    elif 'session' in event and 'not_found' in event:
        return "Verify session ID and check expiration"
    elif 'validation' in event or 'invalid' in event:
        return "Check input parameters and format"
    elif 'permission' in event or 'unauthorized' in event:
        return "Verify user permissions and authentication"
    else:
        return "Check logs for more details and retry if transient"


def add_agent_context(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Add agent-specific context and event enrichment.
    
    This processor detects agent-related events and adds relevant metadata
    for agent operations, conversations, and tool usage.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Event dictionary with agent context
    """
    event = event_dict.get('event', '').lower()
    
    # Detect agent events and add metadata
    if any(keyword in event for keyword in ['agent', 'chat', 'conversation', 'tool']):
        event_dict.setdefault('component', 'agent')
        
        # Add agent operation type
        if 'chat' in event or 'conversation' in event:
            event_dict.setdefault('operation_type', 'conversation')
        elif 'tool' in event:
            event_dict.setdefault('operation_type', 'tool_usage')
        elif 'agent' in event:
            event_dict.setdefault('operation_type', 'agent_management')
    
    return event_dict


def filter_sensitive_data(logger, method_name: str, event_dict: Dict[str, Any]) -> Dict[str, Any]:
    """
    Filter sensitive data from log entries.
    
    This processor removes or masks sensitive information to ensure
    security and privacy compliance.
    
    Args:
        logger: The logger instance
        method_name: The logging method name
        event_dict: The log event dictionary
        
    Returns:
        Dict[str, Any]: Event dictionary with sensitive data filtered
    """
    # List of sensitive field patterns
    sensitive_patterns = [
        'password', 'token', 'key', 'secret', 'credential',
        'auth', 'bearer', 'api_key', 'access_token'
    ]
    
    # Filter sensitive fields
    for key in list(event_dict.keys()):
        if any(pattern in key.lower() for pattern in sensitive_patterns):
            event_dict[key] = '[REDACTED]'
    
    # Filter sensitive content in messages
    if 'message' in event_dict:
        message = str(event_dict['message'])
        # Simple pattern to mask potential tokens/keys
        import re
        # Mask anything that looks like a token (long alphanumeric strings)
        message = re.sub(r'\b[A-Za-z0-9]{20,}\b', '[TOKEN]', message)
        event_dict['message'] = message
    
    return event_dict


def get_default_processors() -> List:
    """
    Get the default processor chain for structured logging.
    
    This function returns the standard processor chain that should be used
    across the Agent C framework for consistent log enrichment.
    
    Returns:
        List: List of processors in execution order
    """
    return [
        # Core structlog processors
        structlog.stdlib.filter_by_level,
        structlog.stdlib.add_logger_name,
        structlog.stdlib.add_log_level,
        structlog.stdlib.PositionalArgumentsFormatter(),
        
        # Framework-specific processors
        add_framework_context,
        add_correlation_id,
        add_timing_info,
        enrich_errors,
        add_agent_context,
        filter_sensitive_data,
        
        # Final processing
        structlog.processors.StackInfoRenderer(),
        structlog.processors.format_exc_info,
        structlog.processors.UnicodeDecoder(),
        
        # Final renderer for stdlib compatibility
        structlog.processors.JSONRenderer(),
    ]


def get_simple_processors() -> List:
    """
    Get a simplified processor chain for testing and basic usage.
    
    This processor chain avoids stdlib-specific processors that might
    cause issues with different logger types.
    
    Returns:
        List: List of processors in execution order
    """
    return [
        # Basic structlog processors that work with stdlib
        structlog.stdlib.add_log_level,
        structlog.stdlib.PositionalArgumentsFormatter(),
        
        # Framework-specific processors
        add_framework_context,
        add_correlation_id,
        add_timing_info,
        enrich_errors,
        add_agent_context,
        filter_sensitive_data,
        
        # Basic final processing
        structlog.processors.format_exc_info,
        structlog.processors.UnicodeDecoder(),
    ]