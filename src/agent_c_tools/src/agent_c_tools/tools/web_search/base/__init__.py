"""
Base classes and interfaces for the unified web search system.

This module provides the foundational classes that all web search engines
and components must implement to ensure consistency and interoperability.
"""

from .engine import BaseWebSearchEngine
from .models import SearchResult, SearchResponse, WebSearchConfig
from .registry import EngineRegistry
from .router import EngineRouter
from .validator import ParameterValidator
from .standardizer import ResponseStandardizer
from .error_handler import ErrorHandler
from .config_manager import (
    WebSearchConfigManager, 
    WebSearchConfiguration, 
    EngineConfigStatus,
    get_config_manager,
    validate_web_search_configuration,
    get_configuration_status,
    get_missing_configurations
)

__all__ = [
    'BaseWebSearchEngine',
    'SearchResult',
    'SearchResponse', 
    'WebSearchConfig',
    'EngineRegistry',
    'EngineRouter',
    'ParameterValidator',
    'ResponseStandardizer',
    'ErrorHandler',
    'WebSearchConfigManager',
    'WebSearchConfiguration',
    'EngineConfigStatus',
    'get_config_manager',
    'validate_web_search_configuration',
    'get_configuration_status',
    'get_missing_configurations'
]