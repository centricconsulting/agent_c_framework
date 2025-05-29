"""Registry for authentication providers.

This module provides a registry for authentication providers to allow
dynamic registration and retrieval of providers.
"""

from typing import Dict, List, Optional, Type, Any
import logging

from agent_c_api.core.auth.providers.base import AuthProvider
from agent_c_api.core.util.logging_utils import LoggingManager

# Configure logging
logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

class AuthProviderRegistry:
    """Registry for authentication providers.
    
    This class manages the registration and retrieval of authentication
    providers, allowing the application to support multiple identity
    providers concurrently.
    """
    
    def __init__(self):
        """Initialize the registry."""
        self._providers: Dict[str, AuthProvider] = {}
        self._default_provider: Optional[str] = None
    
    def register(self, provider: AuthProvider) -> None:
        """Register an authentication provider.
        
        Args:
            provider: The authentication provider to register
        """
        provider_name = provider.get_name()
        
        # Check if provider is enabled before registering
        if provider.is_enabled():
            logger.info(f"Registering authentication provider: {provider_name}")
            self._providers[provider_name] = provider
            
            # Set as default if no default exists yet
            if self._default_provider is None:
                self._default_provider = provider_name
                logger.info(f"Setting {provider_name} as default authentication provider")
        else:
            logger.info(f"Authentication provider {provider_name} is not enabled, skipping registration")
    
    def set_default_provider(self, provider_name: str) -> bool:
        """Set the default authentication provider.
        
        Args:
            provider_name: The name of the provider to set as default
            
        Returns:
            True if successful, False if provider not found
        """
        if provider_name in self._providers:
            self._default_provider = provider_name
            logger.info(f"Set {provider_name} as default authentication provider")
            return True
        else:
            logger.warning(f"Cannot set {provider_name} as default: provider not registered")
            return False
    
    def get_provider(self, provider_name: Optional[str] = None) -> Optional[AuthProvider]:
        """Get an authentication provider by name.
        
        Args:
            provider_name: The name of the provider to get, or None for default
            
        Returns:
            The authentication provider, or None if not found
        """
        if provider_name is None:
            provider_name = self._default_provider
            
        if provider_name is None:
            logger.warning("No default authentication provider set")
            return None
            
        provider = self._providers.get(provider_name)
        if provider is None:
            logger.warning(f"Authentication provider not found: {provider_name}")
        
        return provider
    
    def get_default_provider(self) -> Optional[AuthProvider]:
        """Get the default authentication provider.
        
        Returns:
            The default authentication provider, or None if not set
        """
        if self._default_provider is None:
            return None
        return self._providers.get(self._default_provider)
    
    def get_all_providers(self) -> List[AuthProvider]:
        """Get all registered authentication providers.
        
        Returns:
            A list of all registered authentication providers
        """
        return list(self._providers.values())
    
    def get_enabled_providers(self) -> List[AuthProvider]:
        """Get all enabled authentication providers.
        
        Returns:
            A list of all enabled authentication providers
        """
        return [p for p in self._providers.values() if p.is_enabled()]
    
    def get_provider_metadata(self) -> Dict[str, Dict[str, Any]]:
        """Get metadata for all enabled providers.
        
        Returns:
            A dictionary mapping provider names to their metadata
        """
        return {
            provider.get_name(): provider.get_auth_metadata()
            for provider in self.get_enabled_providers()
        }
    
    def clear(self) -> None:
        """Clear all registered providers.
        
        This is primarily useful for testing.
        """
        self._providers = {}
        self._default_provider = None

# Create a singleton instance of the registry
auth_provider_registry = AuthProviderRegistry()