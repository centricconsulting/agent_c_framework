from fastapi import Request, HTTPException, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from .token_verifier import TokenVerifier
from ..config.auth_config import AUTH_CONFIG

# Initialize security scheme for Swagger UI
security = HTTPBearer()

# Initialize token verifier
token_verifier = TokenVerifier(
    client_id=AUTH_CONFIG["client_id"],
    tenant_id=AUTH_CONFIG["tenant_id"]
)

async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)):
    """Dependency to get the current authenticated user
    
    Args:
        credentials: The HTTP authorization credentials
        
    Returns:
        dict: User claims from the token
        
    Raises:
        HTTPException: If no valid token is provided
    """
    token = credentials.credentials
    if not token:
        raise HTTPException(status_code=401, detail="No authentication token provided")
    
    # Verify the token and return the claims
    return token_verifier.verify(token)

async def optional_user(request: Request):
    """Dependency to get the current user, but not require authentication
    
    This is useful for endpoints that can work with or without authentication
    
    Args:
        request: The incoming request
        
    Returns:
        dict or None: User claims if token is valid, None otherwise
    """
    authorization = request.headers.get("Authorization", "")
    if not authorization.startswith("Bearer "):
        return None
    
    token = authorization.replace("Bearer ", "")
    if not token:
        return None
    
    try:
        return token_verifier.verify(token)
    except HTTPException:
        return None