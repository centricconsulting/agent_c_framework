"""Authentication routes for Agent C API"""

from fastapi import APIRouter, Depends, HTTPException
from ..auth.middleware import get_current_user, optional_user

router = APIRouter(prefix="/auth", tags=["auth"])

@router.get("/validate")
async def validate_token(user: dict = Depends(get_current_user)):
    """Validate the authentication token
    
    Returns:
        dict: Basic user information from the token
    """
    return {
        "valid": True,
        "user": {
            "id": user.get("oid") or user.get("sub"),
            "name": user.get("name"),
            "email": user.get("preferred_username") or user.get("email"),
        }
    }

@router.get("/me")
async def get_user_info(user: dict = Depends(get_current_user)):
    """Get information about the current authenticated user
    
    Returns:
        dict: User profile information
    """
    return {
        "id": user.get("oid") or user.get("sub"),
        "name": user.get("name"),
        "email": user.get("preferred_username") or user.get("email"),
        "roles": user.get("roles", []),
        # Include other relevant user information from the token
    }