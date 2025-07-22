"""
Configuration management for the unified web search system.

This module provides centralized configuration management for all web search engines,
including API key validation, environment variable handling, and configuration validation.
"""

import os
from typing import Dict, List, Optional, Tuple, Any
from dataclasses import dataclass, field
from enum import Enum
from agent_c.util.structured_logging import get_logger

logger = get_logger(__name__)


class ConfigurationError(Exception):
    """Raised when there's a configuration error."""
    pass


class ApiKeyStatus(Enum):
    """Status of API key configuration."""
    CONFIGURED = "configured"
    MISSING = "missing"
    INVALID = "invalid"
    NOT_REQUIRED = "not_required"


@dataclass
class EngineConfigStatus:
    """Configuration status for a single engine."""
    
    engine_name: str
    api_key_status: ApiKeyStatus
    api_key_name: Optional[str] = None
    is_available: bool = False
    error_message: Optional[str] = None
    configuration_hints: List[str] = field(default_factory=list)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization."""
        return {
            'engine_name': self.engine_name,
            'api_key_status': self.api_key_status.value,
            'api_key_name': self.api_key_name,
            'is_available': self.is_available,
            'error_message': self.error_message,
            'configuration_hints': self.configuration_hints
        }


@dataclass
class WebSearchConfiguration:
    """Complete configuration status for the web search system."""
    
    engine_configs: Dict[str, EngineConfigStatus] = field(default_factory=dict)
    available_engines: List[str] = field(default_factory=list)
    configured_engines: List[str] = field(default_factory=list)
    missing_api_keys: List[str] = field(default_factory=list)
    configuration_errors: List[str] = field(default_factory=list)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization."""
        return {
            'engine_configs': {name: config.to_dict() for name, config in self.engine_configs.items()},
            'available_engines': self.available_engines,
            'configured_engines': self.configured_engines,
            'missing_api_keys': self.missing_api_keys,
            'configuration_errors': self.configuration_errors,
            'summary': {
                'total_engines': len(self.engine_configs),
                'available_engines': len(self.available_engines),
                'configured_engines': len(self.configured_engines),
                'missing_configurations': len(self.missing_api_keys)
            }
        }


class WebSearchConfigManager:
    """
    Centralized configuration manager for web search engines.
    
    This class handles API key validation, environment variable management,
    and provides clear error messages for configuration issues.
    """
    
    # Engine configuration mapping
    ENGINE_CONFIGS = {
        'duckduckgo': {
            'requires_api_key': False,
            'api_key_name': None,
            'description': 'DuckDuckGo web search (no API key required)',
            'setup_url': 'https://duckduckgo.com/'
        },
        'google_serp': {
            'requires_api_key': True,
            'api_key_name': 'SERPAPI_API_KEY',
            'description': 'Google Search via SerpAPI',
            'setup_url': 'https://serpapi.com/dashboard',
            'setup_instructions': [
                '1. Sign up at https://serpapi.com/dashboard',
                '2. Get your API key from the dashboard',
                '3. Set environment variable: SERPAPI_API_KEY=your_api_key_here'
            ]
        },
        'tavily': {
            'requires_api_key': True,
            'api_key_name': 'TAVILI_API_KEY',
            'description': 'Tavily research search',
            'setup_url': 'https://tavily.com/',
            'setup_instructions': [
                '1. Sign up at https://tavily.com/',
                '2. Get your API key from the dashboard',
                '3. Set environment variable: TAVILI_API_KEY=your_api_key_here'
            ]
        },
        'wikipedia': {
            'requires_api_key': False,
            'api_key_name': None,
            'description': 'Wikipedia search (no API key required)',
            'setup_url': 'https://www.wikipedia.org/'
        },
        'newsapi': {
            'requires_api_key': True,
            'api_key_name': 'NEWSAPI_API_KEY',
            'description': 'News search via NewsAPI',
            'setup_url': 'https://newsapi.org/',
            'setup_instructions': [
                '1. Sign up at https://newsapi.org/',
                '2. Get your API key from the dashboard',
                '3. Set environment variable: NEWSAPI_API_KEY=your_api_key_here'
            ]
        },
        'hackernews': {
            'requires_api_key': False,
            'api_key_name': None,
            'description': 'Hacker News search (no API key required)',
            'setup_url': 'https://news.ycombinator.com/'
        }
    }
    
    def __init__(self):
        """Initialize the configuration manager."""
        self._config_cache: Optional[WebSearchConfiguration] = None
        self._cache_timestamp: Optional[float] = None
        self._cache_ttl = 300  # 5 minutes cache TTL
    
    def get_configuration_status(self, force_refresh: bool = False) -> WebSearchConfiguration:
        """
        Get the current configuration status for all engines.
        
        Args:
            force_refresh: Force refresh of cached configuration
            
        Returns:
            WebSearchConfiguration object with current status
        """
        import time
        
        current_time = time.time()
        
        # Check cache validity
        if (not force_refresh and 
            self._config_cache and 
            self._cache_timestamp and 
            (current_time - self._cache_timestamp) < self._cache_ttl):
            return self._config_cache
        
        # Refresh configuration
        config = WebSearchConfiguration()
        
        for engine_name, engine_info in self.ENGINE_CONFIGS.items():
            engine_config = self._check_engine_configuration(engine_name, engine_info)
            config.engine_configs[engine_name] = engine_config
            
            if engine_config.is_available:
                config.available_engines.append(engine_name)
                
            if engine_config.api_key_status == ApiKeyStatus.CONFIGURED:
                config.configured_engines.append(engine_name)
            elif engine_config.api_key_status == ApiKeyStatus.MISSING:
                config.missing_api_keys.append(engine_config.api_key_name)
                
            if engine_config.error_message:
                config.configuration_errors.append(f"{engine_name}: {engine_config.error_message}")
        
        # Cache the result
        self._config_cache = config
        self._cache_timestamp = current_time
        
        return config
    
    def _check_engine_configuration(self, engine_name: str, engine_info: Dict[str, Any]) -> EngineConfigStatus:
        """
        Check configuration status for a single engine.
        
        Args:
            engine_name: Name of the engine
            engine_info: Engine configuration information
            
        Returns:
            EngineConfigStatus object
        """
        config_status = EngineConfigStatus(
            engine_name=engine_name,
            api_key_status=ApiKeyStatus.NOT_REQUIRED,
            api_key_name=engine_info.get('api_key_name')
        )
        
        try:
            if not engine_info.get('requires_api_key', False):
                # No API key required
                config_status.api_key_status = ApiKeyStatus.NOT_REQUIRED
                config_status.is_available = True
                config_status.configuration_hints = [
                    f"✓ {engine_info.get('description', engine_name)} - No configuration required"
                ]
            else:
                # API key required
                api_key_name = engine_info.get('api_key_name')
                if not api_key_name:
                    config_status.api_key_status = ApiKeyStatus.INVALID
                    config_status.error_message = "API key name not configured"
                    config_status.configuration_hints = ["Contact system administrator"]
                else:
                    api_key_value = os.getenv(api_key_name)
                    if not api_key_value:
                        config_status.api_key_status = ApiKeyStatus.MISSING
                        config_status.error_message = f"Environment variable {api_key_name} not set"
                        config_status.configuration_hints = engine_info.get('setup_instructions', [])
                    elif not api_key_value.strip():
                        config_status.api_key_status = ApiKeyStatus.INVALID
                        config_status.error_message = f"Environment variable {api_key_name} is empty"
                        config_status.configuration_hints = engine_info.get('setup_instructions', [])
                    else:
                        # API key is present, validate format if possible
                        if self._validate_api_key_format(engine_name, api_key_value):
                            config_status.api_key_status = ApiKeyStatus.CONFIGURED
                            config_status.is_available = True
                            config_status.configuration_hints = [
                                f"✓ {engine_info.get('description', engine_name)} - Configured"
                            ]
                        else:
                            config_status.api_key_status = ApiKeyStatus.INVALID
                            config_status.error_message = f"API key format appears invalid for {engine_name}"
                            config_status.configuration_hints = engine_info.get('setup_instructions', [])
        
        except Exception as e:
            config_status.api_key_status = ApiKeyStatus.INVALID
            config_status.error_message = f"Configuration check failed: {str(e)}"
            config_status.configuration_hints = ["Check system configuration"]
            logger.error(f"Configuration check failed for {engine_name}: {e}")
        
        return config_status
    
    def _validate_api_key_format(self, engine_name: str, api_key: str) -> bool:
        """
        Validate API key format for known patterns.
        
        Args:
            engine_name: Name of the engine
            api_key: API key to validate
            
        Returns:
            True if format appears valid, False otherwise
        """
        if not api_key or not api_key.strip():
            return False
        
        # Basic validation patterns for known services
        if engine_name == 'google_serp':
            # SerpAPI keys are typically 64 characters, alphanumeric
            return len(api_key) >= 32 and api_key.replace('-', '').replace('_', '').isalnum()
        elif engine_name == 'tavily':
            # Tavily keys are typically UUIDs or similar format
            return len(api_key) >= 20 and '-' in api_key
        elif engine_name == 'newsapi':
            # NewsAPI keys are typically 32 characters, alphanumeric
            return len(api_key) >= 20 and api_key.replace('-', '').isalnum()
        
        # For unknown engines, just check it's not empty
        return len(api_key.strip()) > 0
    
    def get_missing_configurations(self) -> List[Tuple[str, str, List[str]]]:
        """
        Get list of missing configurations with setup instructions.
        
        Returns:
            List of tuples (engine_name, api_key_name, setup_instructions)
        """
        config = self.get_configuration_status()
        missing_configs = []
        
        for engine_name, engine_config in config.engine_configs.items():
            if engine_config.api_key_status == ApiKeyStatus.MISSING:
                engine_info = self.ENGINE_CONFIGS.get(engine_name, {})
                missing_configs.append((
                    engine_name,
                    engine_config.api_key_name,
                    engine_info.get('setup_instructions', [])
                ))
        
        return missing_configs
    
    def get_configuration_summary(self) -> str:
        """
        Get a human-readable configuration summary.
        
        Returns:
            Formatted string with configuration status
        """
        config = self.get_configuration_status()
        
        summary_lines = [
            "Web Search Configuration Summary",
            "=" * 40,
            f"Total engines: {len(config.engine_configs)}",
            f"Available engines: {len(config.available_engines)}",
            f"Configured engines: {len(config.configured_engines)}",
            ""
        ]
        
        if config.available_engines:
            summary_lines.append("✓ Available engines:")
            for engine in config.available_engines:
                engine_config = config.engine_configs[engine]
                status = "✓" if engine_config.api_key_status == ApiKeyStatus.CONFIGURED else "○"
                summary_lines.append(f"  {status} {engine}")
            summary_lines.append("")
        
        if config.missing_api_keys:
            summary_lines.append("⚠ Missing API keys:")
            for engine_name, engine_config in config.engine_configs.items():
                if engine_config.api_key_status == ApiKeyStatus.MISSING:
                    summary_lines.append(f"  • {engine_name}: {engine_config.api_key_name}")
            summary_lines.append("")
        
        if config.configuration_errors:
            summary_lines.append("✗ Configuration errors:")
            for error in config.configuration_errors:
                summary_lines.append(f"  • {error}")
            summary_lines.append("")
        
        return "\n".join(summary_lines)
    
    def get_setup_instructions(self, engine_name: str) -> Optional[List[str]]:
        """
        Get setup instructions for a specific engine.
        
        Args:
            engine_name: Name of the engine
            
        Returns:
            List of setup instructions or None if not found
        """
        engine_info = self.ENGINE_CONFIGS.get(engine_name)
        if not engine_info:
            return None
        
        return engine_info.get('setup_instructions')
    
    def validate_configuration(self) -> Tuple[bool, List[str]]:
        """
        Validate the current configuration and return status.
        
        Returns:
            Tuple of (is_valid, error_messages)
        """
        config = self.get_configuration_status()
        
        # Configuration is valid if at least one engine is available
        is_valid = len(config.available_engines) > 0
        
        error_messages = []
        
        if not is_valid:
            error_messages.append("No search engines are available")
        
        # Add specific configuration errors
        error_messages.extend(config.configuration_errors)
        
        # Add missing API key warnings (not errors since some engines don't require keys)
        if config.missing_api_keys:
            for engine_name, engine_config in config.engine_configs.items():
                if engine_config.api_key_status == ApiKeyStatus.MISSING:
                    error_messages.append(
                        f"Optional: {engine_name} requires {engine_config.api_key_name} "
                        f"environment variable for full functionality"
                    )
        
        return is_valid, error_messages
    
    def clear_cache(self) -> None:
        """Clear the configuration cache to force refresh."""
        self._config_cache = None
        self._cache_timestamp = None


# Global configuration manager instance
_config_manager: Optional[WebSearchConfigManager] = None


def get_config_manager() -> WebSearchConfigManager:
    """
    Get the global configuration manager instance.
    
    Returns:
        WebSearchConfigManager instance
    """
    global _config_manager
    if _config_manager is None:
        _config_manager = WebSearchConfigManager()
    return _config_manager


def validate_web_search_configuration() -> Tuple[bool, List[str]]:
    """
    Validate the web search configuration.
    
    Returns:
        Tuple of (is_valid, error_messages)
    """
    return get_config_manager().validate_configuration()


def get_configuration_status() -> WebSearchConfiguration:
    """
    Get the current configuration status.
    
    Returns:
        WebSearchConfiguration object
    """
    return get_config_manager().get_configuration_status()


def get_missing_configurations() -> List[Tuple[str, str, List[str]]]:
    """
    Get missing configurations with setup instructions.
    
    Returns:
        List of tuples (engine_name, api_key_name, setup_instructions)
    """
    return get_config_manager().get_missing_configurations()