"""Microsoft authentication provider implementation.

This module provides a concrete implementation of the authentication
provider interface for Microsoft Azure AD.
"""

import os
import time
import httpx
from datetime import datetime
from typing import Dict, Any, Optional, List

from fastapi import HTTPException
from jose import jwt, JWTError
from dotenv import load_dotenv

from agent_c_api.core.auth.providers.base import AuthProvider, TokenVerifier
from agent_c_api.core.util.logging_utils import LoggingManager

# Initialize logging
logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

# Load environment variables
load_dotenv(override=True)

class MicrosoftTokenVerifier(TokenVerifier):
    """Token verifier for Microsoft identity tokens.
    
    This class verifies JWT tokens issued by Microsoft Identity Platform
    using JWKS (JSON Web Key Set) for signature verification.
    """
    
    def __init__(self, client_id: str, tenant_id: str, cache_ttl: int = 3600):
        """Initialize the Microsoft token verifier.
        
        Args:
            client_id: Azure AD client ID
            tenant_id: Azure AD tenant ID
            cache_ttl: JWKS cache TTL in seconds (default: 1 hour)
        """
        self.client_id = client_id
        self.tenant_id = tenant_id
        self.authority = f"https://login.microsoftonline.com/{tenant_id}"
        self.openid_config_url = f"{self.authority}/v2.0/.well-known/openid-configuration"
        self.cache_ttl = cache_ttl
        
        # Initialize cache variables
        self.jwks_cache = None
        self.jwks_cache_timestamp = 0
        self.jwks_cache_keys_by_kid = {}
        self.jwks_cache_hit_count = 0
        self.jwks_cache_miss_count = 0
        
        # Default JWT verification algorithm
        self.algorithms = ["RS256"]
        
        # Initialize configuration
        self._init_config()
        
    def _init_config(self) -> None:
        """Initialize the OpenID configuration."""
        try:
            response = httpx.get(self.openid_config_url)
            if response.status_code != 200:
                logger.error(f"Failed to get OpenID configuration: {response.status_code} {response.text}")
                self.config = None
                self.jwks_uri = None
                self.issuer = None
            else:
                self.config = response.json()
                self.jwks_uri = self.config["jwks_uri"]
                self.issuer = self.config["issuer"]
                
                # Initialize the JWKS cache on startup
                self._refresh_jwks_cache()
        except Exception as e:
            logger.error(f"Error initializing Microsoft token verifier: {e}")
            self.config = None
            self.jwks_uri = None
            self.issuer = None
    
    def _refresh_jwks_cache(self) -> bool:
        """Refresh the JWKS cache from the identity provider.
        
        Returns:
            bool: True if cache was successfully refreshed, False otherwise
        """
        try:
            if not self.jwks_uri:
                logger.error("Cannot refresh JWKS cache: JWKS URI not available")
                return False
                
            logger.debug("Refreshing JWKS cache for Microsoft provider")
            jwks_response = httpx.get(self.jwks_uri)
            
            if jwks_response.status_code != 200:
                logger.error(f"Failed to get JWKS during cache refresh: {jwks_response.status_code} {jwks_response.text}")
                return False
                
            # Update the cache
            self.jwks_cache = jwks_response.json()
            self.jwks_cache_timestamp = time.time()
            
            # Create a lookup dictionary by key ID for faster access
            self.jwks_cache_keys_by_kid = {}
            for key in self.jwks_cache.get("keys", []):
                kid = key.get("kid")
                if kid:
                    self.jwks_cache_keys_by_kid[kid] = key
            
            cache_time = datetime.fromtimestamp(self.jwks_cache_timestamp).strftime('%Y-%m-%d %H:%M:%S')
            logger.info(f"Microsoft JWKS cache refreshed at {cache_time} with {len(self.jwks_cache_keys_by_kid)} keys")
            return True
            
        except Exception as e:
            logger.error(f"Error refreshing Microsoft JWKS cache: {e}")
            return False
    
    def _is_cache_valid(self) -> bool:
        """Check if the JWKS cache is still valid.
        
        Returns:
            bool: True if cache is valid, False if it needs to be refreshed
        """
        if not self.jwks_cache:
            return False
            
        cache_age = time.time() - self.jwks_cache_timestamp
        return cache_age < self.cache_ttl
    
    async def verify(self, token: str) -> Dict[str, Any]:
        """Verify a JWT token and return its claims if valid.
        
        Args:
            token: The JWT token to verify
            
        Returns:
            A dictionary of claims extracted from the token
            
        Raises:
            HTTPException: If the token is invalid or verification fails
        """
        if not self.config:
            logger.error("Microsoft token verifier not properly initialized")
            raise HTTPException(status_code=500, detail="Authentication service unavailable")
            
        try:
            # Get the token header to extract the key ID (kid)
            header = jwt.get_unverified_header(token)
            if not header or "kid" not in header:
                logger.error("Invalid token header or missing kid")
                raise HTTPException(status_code=401, detail="Invalid token format")
                
            kid = header["kid"]
            key = None
            
            # Check if we need to refresh the cache
            if not self._is_cache_valid():
                logger.debug("Microsoft JWKS cache expired, refreshing")
                self.jwks_cache_miss_count += 1
                if not self._refresh_jwks_cache():
                    raise HTTPException(status_code=500, detail="Failed to retrieve signing keys")
            else:
                self.jwks_cache_hit_count += 1
                
            # Try to find the key in our cached lookup dictionary
            key = self.jwks_cache_keys_by_kid.get(kid)
            
            # If key not found in cache, try to refresh cache once
            if not key:
                logger.info(f"Key ID {kid} not found in Microsoft cache, refreshing JWKS")
                self.jwks_cache_miss_count += 1
                if self._refresh_jwks_cache():
                    key = self.jwks_cache_keys_by_kid.get(kid)
            
            # Still no key? Then it's invalid
            if not key:
                logger.error(f"No matching key found for kid: {kid}")
                raise HTTPException(status_code=401, detail="Invalid token signature")
                
            # Log cache performance occasionally
            total_lookups = self.jwks_cache_hit_count + self.jwks_cache_miss_count
            if total_lookups % 100 == 0:
                hit_rate = (self.jwks_cache_hit_count / total_lookups) * 100 if total_lookups > 0 else 0
                logger.info(f"Microsoft JWKS cache performance: {hit_rate:.1f}% hit rate ({self.jwks_cache_hit_count} hits, {self.jwks_cache_miss_count} misses)")
                
            # Decode and verify the token
            payload = jwt.decode(
                token, 
                key, 
                algorithms=self.algorithms,
                audience=self.client_id,
                issuer=self.issuer
            )
            
            return payload
            
        except JWTError as e:
            logger.error(f"Microsoft JWT validation error: {e}")
            raise HTTPException(status_code=401, detail="Invalid authentication token")
        except Exception as e:
            logger.error(f"Microsoft token verification error: {e}")
            raise HTTPException(status_code=401, detail="Authentication error")
    
    def get_provider_name(self) -> str:
        """Get the name of the authentication provider.
        
        Returns:
            The name of the authentication provider
        """
        return "microsoft"

class MicrosoftAuthProvider(AuthProvider):
    """Authentication provider for Microsoft Azure AD.
    
    This class provides authentication functionality for Microsoft Azure AD
    identity services.
    """
    
    def __init__(self):
        """Initialize the Microsoft authentication provider."""
        # Get configuration from environment variables
        self.client_id = os.getenv("AZURE_CLIENT_ID")
        self.tenant_id = os.getenv("AZURE_TENANT_ID")
        self.cache_ttl = int(os.getenv("JWKS_CACHE_TTL_SECONDS", 3600))
        
        # Create token verifier
        if self.is_enabled():
            self.token_verifier = MicrosoftTokenVerifier(
                client_id=self.client_id,
                tenant_id=self.tenant_id,
                cache_ttl=self.cache_ttl
            )
        else:
            self.token_verifier = None
    
    def get_name(self) -> str:
        """Get the name of this authentication provider.
        
        Returns:
            The name of the authentication provider
        """
        return "microsoft"
    
    def is_enabled(self) -> bool:
        """Check if this authentication provider is enabled.
        
        Returns:
            True if this provider is properly configured and enabled
        """
        return bool(self.client_id and self.tenant_id)
    
    def get_token_verifier(self) -> Optional[TokenVerifier]:
        """Get the token verifier for this authentication provider.
        
        Returns:
            A TokenVerifier instance for this provider
        """
        return self.token_verifier
    
    def get_config(self) -> Dict[str, Any]:
        """Get the configuration for this authentication provider.
        
        Returns:
            A dictionary containing provider configuration
        """
        authority = f"https://login.microsoftonline.com/{self.tenant_id}"
        return {
            "client_id": self.client_id,
            "tenant_id": self.tenant_id,
            "authority": authority,
            "cache_ttl": self.cache_ttl,
        }
    
    def get_auth_metadata(self) -> Dict[str, Any]:
        """Get metadata about this authentication provider for clients.
        
        Returns:
            A dictionary containing provider metadata
        """
        authority = f"https://login.microsoftonline.com/{self.tenant_id}"
        return {
            "provider": "microsoft",
            "display_name": "Microsoft",
            "client_id": self.client_id,
            "authority": authority,
            "scopes": ["openid", "profile", "User.Read"],
            "enabled": self.is_enabled(),
        }