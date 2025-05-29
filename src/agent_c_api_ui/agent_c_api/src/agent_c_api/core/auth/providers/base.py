"""Base classes for authentication providers.

This module defines the abstract interfaces for authentication providers
and token verifiers used in the Agent C API authentication system.
"""

from abc import ABC, abstractmethod
from typing import Dict, Any, Optional, List
from fastapi import HTTPException

class TokenVerifier(ABC):
    """Abstract base class for token verification.
    
    Token verifiers are responsible for validating authentication tokens
    and extracting user information from them.
    """
    
    @abstractmethod
    async def verify(self, token: str) -> Dict[str, Any]:
        """Verify a token and return its claims if valid.
        
        Args:
            token: The authentication token to verify
            
        Returns:
            A dictionary of claims extracted from the token
            
        Raises:
            HTTPException: If the token is invalid or verification fails
        """
        pass
    
    @abstractmethod
    def get_provider_name(self) -> str:
        """Get the name of the authentication provider.
        
        Returns:
            The name of the authentication provider
        """
        pass

class AuthProvider(ABC):
    """Abstract base class for authentication providers.
    
    Authentication providers are responsible for configuring and managing
    authentication for a specific identity provider (e.g., Microsoft, Google).
    """
    
    @abstractmethod
    def get_name(self) -> str:
        """Get the name of this authentication provider.
        
        Returns:
            The name of the authentication provider
        """
        pass
    
    @abstractmethod
    def is_enabled(self) -> bool:
        """Check if this authentication provider is enabled.
        
        Returns:
            True if this provider is properly configured and enabled
        """
        pass
    
    @abstractmethod
    def get_token_verifier(self) -> TokenVerifier:
        """Get the token verifier for this authentication provider.
        
        Returns:
            A TokenVerifier instance for this provider
        """
        pass
    
    @abstractmethod
    def get_config(self) -> Dict[str, Any]:
        """Get the configuration for this authentication provider.
        
        Returns:
            A dictionary containing provider configuration
        """
        pass
    
    @abstractmethod
    def get_auth_metadata(self) -> Dict[str, Any]:
        """Get metadata about this authentication provider for clients.
        
        This method returns information that can be sent to clients to help
        them configure their authentication UI and logic.
        
        Returns:
            A dictionary containing provider metadata
        """
        pass