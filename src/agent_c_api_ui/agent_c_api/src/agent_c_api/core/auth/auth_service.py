"""Authentication service for Agent C API.

This module provides the central authentication service that manages
and coordinates the various authentication providers.
"""

from fastapi import FastAPI, HTTPException, Depends, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from typing import Dict, Any, Optional, List
import logging

from agent_c_api.core.auth.providers.base import AuthProvider, TokenVerifier
from agent_c_api.core.auth.providers.registry import auth_provider_registry
from agent_c_api.core.auth.providers.microsoft import MicrosoftAuthProvider
from agent_c_api.core.auth.providers.google import GoogleAuthProvider
from agent_c_api.core.util.logging_utils import LoggingManager

# Configure logging
logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

# Security scheme for Bearer token
security = HTTPBearer(auto_error=False)

class AuthService:
    """Authentication service for Agent C API.
    
    This class provides authentication functionality and coordinates
    multiple authentication providers.
    """
    
    def __init__(self):
        """Initialize the authentication service."""
        self.provider_registry = auth_provider_registry
        
        # Register built-in providers
        self._register_built_in_providers()
    
    def _register_built_in_providers(self):
        """Register built-in authentication providers."""
        # Register Microsoft provider
        self.provider_registry.register(MicrosoftAuthProvider())
        
        # Register Google provider (currently a placeholder implementation)
        # Uncomment when fully implemented
        # self.provider_registry.register(GoogleAuthProvider())
    
    async def verify_token(self, token: str, provider_name: Optional[str] = None) -> Dict[str, Any]:
        """Verify an authentication token.
        
        Args:
            token: The authentication token to verify
            provider_name: The name of the provider to use, or None for auto-detection
            
        Returns:
            A dictionary of claims extracted from the token
            
        Raises:
            HTTPException: If the token is invalid or verification fails
        """
        # If provider specified, use it directly
        if provider_name:
            provider = self.provider_registry.get_provider(provider_name)
            if not provider or not provider.is_enabled():
                logger.error(f"Requested authentication provider not available: {provider_name}")
                raise HTTPException(status_code=400, detail=f"Authentication provider '{provider_name}' not available")
                
            verifier = provider.get_token_verifier()
            return await verifier.verify(token)
        
        # Try all enabled providers
        enabled_providers = self.provider_registry.get_enabled_providers()
        if not enabled_providers:
            logger.error("No authentication providers are enabled")
            raise HTTPException(status_code=500, detail="Authentication service unavailable")
        
        # Try default provider first
        default_provider = self.provider_registry.get_default_provider()
        if default_provider:
            try:
                verifier = default_provider.get_token_verifier()
                return await verifier.verify(token)
            except HTTPException as e:
                # Only continue to other providers if this is an authorization error
                if e.status_code != 401:
                    raise
                logger.debug(f"Token verification failed with default provider: {e}")
        
        # Try each provider in turn
        last_error = None
        for provider in enabled_providers:
            # Skip default provider if we already tried it
            if default_provider and provider.get_name() == default_provider.get_name():
                continue
                
            try:
                verifier = provider.get_token_verifier()
                return await verifier.verify(token)
            except HTTPException as e:
                logger.debug(f"Token verification failed with provider {provider.get_name()}: {e}")
                last_error = e
        
        # If we get here, all providers failed
        if last_error:
            raise last_error
        else:
            raise HTTPException(status_code=401, detail="Invalid authentication token")
    
    def get_enabled_providers(self) -> List[Dict[str, Any]]:
        """Get metadata for all enabled authentication providers.
        
        Returns:
            A list of provider metadata dictionaries
        """
        return [
            provider.get_auth_metadata()
            for provider in self.provider_registry.get_enabled_providers()
        ]
    
    def get_default_provider_name(self) -> Optional[str]:
        """Get the name of the default authentication provider.
        
        Returns:
            The name of the default provider, or None if not set
        """
        default_provider = self.provider_registry.get_default_provider()
        return default_provider.get_name() if default_provider else None

# Create a singleton instance of the auth service
_auth_service = None

def get_auth_service() -> AuthService:
    """Get the global authentication service instance.
    
    Returns:
        The global AuthService instance
    """
    global _auth_service
    if _auth_service is None:
        _auth_service = AuthService()
    return _auth_service

async def get_current_user(
    credentials: Optional[HTTPAuthorizationCredentials] = Security(security),
    provider: Optional[str] = None
) -> Dict[str, Any]:
    """Dependency to extract and validate the JWT token from the Authorization header.
    
    This function can be used as a FastAPI dependency to validate authentication
    and extract user information from the token.
    
    Args:
        credentials: The HTTP Authorization credentials
        provider: Optional provider name to use for validation
        
    Returns:
        A dictionary of user information extracted from the token
        
    Raises:
        HTTPException: If authentication fails
    """
    if not credentials:
        raise HTTPException(status_code=401, detail="Not authenticated")
    
    token = credentials.credentials
    auth_service = get_auth_service()
    return await auth_service.verify_token(token, provider)

def configure_auth(app: FastAPI) -> None:
    """Configure authentication for the FastAPI application.
    
    Args:
        app: The FastAPI application to configure
    """
    # Initialize auth service
    auth_service = get_auth_service()
    
    # Log the authentication configuration
    enabled_providers = auth_service.get_enabled_providers()
    default_provider = auth_service.get_default_provider_name()
    
    if enabled_providers:
        provider_names = ", ".join(p["provider"] for p in enabled_providers)
        logger.info(f"Authentication enabled with providers: {provider_names}")
        logger.info(f"Default authentication provider: {default_provider}")
        
        # Log details for each provider
        for provider_meta in enabled_providers:
            provider_name = provider_meta["provider"]
            if "client_id" in provider_meta and provider_meta["client_id"]:
                client_id = provider_meta["client_id"]
                # Don't log the full client ID for security reasons
                client_id_masked = f"{client_id[:6]}...{client_id[-4:]}" if len(client_id) > 10 else "[CONFIGURED]"
                logger.info(f"{provider_name.capitalize()} provider client ID: {client_id_masked}")
    else:
        logger.warning("No authentication providers are enabled")
        logger.warning("Authentication will not work without proper configuration")