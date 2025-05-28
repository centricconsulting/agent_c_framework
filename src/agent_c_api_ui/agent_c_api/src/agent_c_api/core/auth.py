from fastapi import FastAPI, HTTPException, Request, Depends, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import jwt, JWTError
import httpx
import os
from dotenv import load_dotenv
from typing import Optional, Dict, Any
import logging
from agent_c_api.core.util.logging_utils import LoggingManager

load_dotenv(override=True)

# Get configuration from environment variables
CLIENT_ID = os.getenv("AZURE_CLIENT_ID")
TENANT_ID = os.getenv("AZURE_TENANT_ID")
AUTHORITY = f"https://login.microsoftonline.com/{TENANT_ID}"
OPENID_CONFIG_URL = f"{AUTHORITY}/v2.0/.well-known/openid-configuration"

# Configure logging
logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

# Security scheme for Bearer token
security = HTTPBearer(auto_error=False)

class TokenVerifier:
    """Verifies JWT tokens from Microsoft Identity Platform"""
    
    def __init__(self):
        try:
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
            self.algorithms = ["RS256"]
        except Exception as e:
            logger.error(f"Error initializing TokenVerifier: {e}")
            self.config = None
            self.jwks_uri = None
            self.issuer = None
    
    async def verify(self, token: str) -> Dict[str, Any]:
        """Verify a JWT token and return its claims if valid"""
        if not self.config:
            logger.error("TokenVerifier not properly initialized")
            raise HTTPException(status_code=500, detail="Authentication service unavailable")
            
        try:
            jwks_response = httpx.get(self.jwks_uri)
            if jwks_response.status_code != 200:
                logger.error(f"Failed to get JWKS: {jwks_response.status_code} {jwks_response.text}")
                raise HTTPException(status_code=500, detail="Failed to retrieve signing keys")
                
            jwks = jwks_response.json()["keys"]
            header = jwt.get_unverified_header(token)
            
            # Find the signing key that matches the token's key ID
            key = None
            for k in jwks:
                if k["kid"] == header["kid"]:
                    key = k
                    break
                    
            if not key:
                logger.error(f"No matching key found for kid: {header.get('kid')}")
                raise HTTPException(status_code=401, detail="Invalid token signature")
                
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
    else:
        logger.warning("Microsoft Authentication NOT configured properly")
        logger.warning("Authentication will not work without proper Azure AD configuration")