"""Authentication module for Agent C API.

This module provides the main authentication functionality for the Agent C API.
It is a compatibility layer that forwards to the new modular authentication system.
"""

from fastapi import FastAPI, HTTPException, Depends, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from typing import Optional, Dict, Any

# Import from the new modular auth system
from agent_c_api.core.auth.auth_service import (
    get_auth_service,
    get_current_user as new_get_current_user,
    configure_auth as new_configure_auth,
)

# Re-export the security scheme for backward compatibility
security = HTTPBearer(auto_error=False)

async def get_current_user(credentials: Optional[HTTPAuthorizationCredentials] = Security(security)) -> Dict[str, Any]:
    """Dependency to extract and validate the JWT token from the Authorization header.
    
    This is a compatibility wrapper around the new modular auth system.
    
    Args:
        credentials: The HTTP Authorization credentials
        
    Returns:
        A dictionary of user information extracted from the token
        
    Raises:
        HTTPException: If authentication fails
    """
    return await new_get_current_user(credentials)

def configure_auth(app: FastAPI) -> None:
    """Configure authentication for the FastAPI application.
    
    This is a compatibility wrapper around the new modular auth system.
    
    Args:
        app: The FastAPI application to configure
    """
    return new_configure_auth(app)