from fastapi import FastAPI, HTTPException, Request, Depends, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import jwt, JWTError
import httpx
import os
import time
from datetime import datetime, timedelta
from dotenv import load_dotenv
from typing import Optional, Dict, Any, List
import logging
from agent_c_api.core.util.logging_utils import LoggingManager

load_dotenv(override=True)

# Get configuration from environment variables
CLIENT_ID = os.getenv("AZURE_CLIENT_ID")
TENANT_ID = os.getenv("AZURE_TENANT_ID")
AUTHORITY = f"https://login.microsoftonline.com/{TENANT_ID}"
OPENID_CONFIG_URL = f"{AUTHORITY}/v2.0/.well-known/openid-configuration"

# JWKS Cache configuration
JWKS_CACHE_TTL = int(os.getenv("JWKS_CACHE_TTL_SECONDS", 3600))  # Default: 1 hour

# Configure logging
logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

# Security scheme for Bearer token
security = HTTPBearer(auto_error=False)

class TokenVerifier:
    """Verifies JWT tokens from Microsoft Identity Platform with JWKS caching"""
    
    def __init__(self):
        try:
            # Initialize cache variables
            self.jwks_cache = None
            self.jwks_cache_timestamp = 0
            self.jwks_cache_keys_by_kid = {}
            self.jwks_cache_hit_count = 0
            self.jwks_cache_miss_count = 0
            
            # Get OpenID configuration
            response = httpx.get(OPENID_CONFIG_URL)
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
                
            self.algorithms = ["RS256"]
        except Exception as e:
            logger.error(f"Error initializing TokenVerifier: {e}")
            self.config = None
            self.jwks_uri = None
            self.issuer = None
    
    def _refresh_jwks_cache(self) -> bool:
        """Refresh the JWKS cache from the identity provider
        
        Returns:
            bool: True if cache was successfully refreshed, False otherwise
        """
        try:
            if not self.jwks_uri:
                logger.error("Cannot refresh JWKS cache: JWKS URI not available")
                return False
                
            logger.debug("Refreshing JWKS cache")
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
            logger.info(f"JWKS cache refreshed at {cache_time} with {len(self.jwks_cache_keys_by_kid)} keys")
            return True
            
        except Exception as e:
            logger.error(f"Error refreshing JWKS cache: {e}")
            return False
    
    def _is_cache_valid(self) -> bool:
        """Check if the JWKS cache is still valid
        
        Returns:
            bool: True if cache is valid, False if it needs to be refreshed
        """
        if not self.jwks_cache:
            return False
            
        cache_age = time.time() - self.jwks_cache_timestamp
        return cache_age < JWKS_CACHE_TTL
    
    async def verify(self, token: str) -> Dict[str, Any]:
        """Verify a JWT token and return its claims if valid
        
        Uses cached JWKS when available and valid to improve performance.
        """
        if not self.config:
            logger.error("TokenVerifier not properly initialized")
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
                logger.debug("JWKS cache expired, refreshing")
                self.jwks_cache_miss_count += 1
                if not self._refresh_jwks_cache():
                    raise HTTPException(status_code=500, detail="Failed to retrieve signing keys")
            else:
                self.jwks_cache_hit_count += 1
                
            # Try to find the key in our cached lookup dictionary
            key = self.jwks_cache_keys_by_kid.get(kid)
            
            # If key not found in cache, try to refresh cache once
            if not key:
                logger.info(f"Key ID {kid} not found in cache, refreshing JWKS")
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
                logger.info(f"JWKS cache performance: {hit_rate:.1f}% hit rate ({self.jwks_cache_hit_count} hits, {self.jwks_cache_miss_count} misses)")
                
            # Decode and verify the token
            payload = jwt.decode(
                token, 
                key, 
                algorithms=self.algorithms,
                audience=CLIENT_ID,
                issuer=self.issuer
            )
            
            return payload
            
        except JWTError as e:
            logger.error(f"JWT validation error: {e}")
            raise HTTPException(status_code=401, detail="Invalid authentication token")
        except Exception as e:
            logger.error(f"Token verification error: {e}")
            raise HTTPException(status_code=401, detail="Authentication error")

# Create a singleton instance of the token verifier
token_verifier = TokenVerifier()

async def get_current_user(credentials: Optional[HTTPAuthorizationCredentials] = Security(security)) -> Dict[str, Any]:
    """Dependency to extract and validate the JWT token from the Authorization header"""
    if not credentials:
        raise HTTPException(status_code=401, detail="Not authenticated")
    
    token = credentials.credentials
    return await token_verifier.verify(token)

def configure_auth(app: FastAPI) -> None:
    """Configure authentication for the FastAPI application"""
    # This function can be used to add additional auth-related setup
    # such as registering auth routes or configuring more complex auth strategies
    
    # Log the authentication configuration
    auth_enabled = bool(CLIENT_ID and TENANT_ID)
    
    if auth_enabled:
        logger.info("Microsoft Authentication enabled")
        logger.info(f"Authority: {AUTHORITY}")
        # Don't log the full client ID for security reasons
        client_id_masked = f"{CLIENT_ID[:6]}...{CLIENT_ID[-4:]}" if len(CLIENT_ID) > 10 else "[CONFIGURED]"
        logger.info(f"Client ID: {client_id_masked}")
        logger.info(f"JWKS Cache TTL: {JWKS_CACHE_TTL} seconds")
    else:
        logger.warning("Microsoft Authentication NOT configured properly")
        logger.warning("Authentication will not work without proper Azure AD configuration")