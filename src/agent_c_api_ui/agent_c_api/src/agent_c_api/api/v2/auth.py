from fastapi import APIRouter, Depends, HTTPException
from agent_c_api.core.auth import get_current_user

router = APIRouter(prefix="/auth", tags=["auth"])

@router.get("/me", summary="Get current authenticated user information")
async def get_me(user: dict = Depends(get_current_user)):
    """Returns information about the currently authenticated user.
    
    This endpoint is used by the front-end to validate authentication
    and get user details like name, email, and roles.
    """
    # Extract only the user information we want to expose
    return {
        "authenticated": True,
        "user_id": user.get("oid"),  # Azure AD Object ID
        "name": user.get("name"),
        "email": user.get("preferred_username"),
        "roles": user.get("roles", []),
        # Additional claims can be extracted from the token as needed
    }

@router.get("/validate", summary="Validate authentication token")
async def validate_token(user: dict = Depends(get_current_user)):
    """Simple endpoint to validate if the current token is valid.
    
    Returns a 200 OK response if the token is valid, or a 401 Unauthorized
    if the token is invalid or missing.
    """
    return {"valid": True}