"""
Redis Admin Authentication Middleware

Provides authentication and authorization middleware for Redis admin endpoints.
Implements role-based access control, request validation, and audit logging.
"""

import logging
import time
from typing import Optional, Dict, Any, List
from datetime import datetime
from fastapi import HTTPException, Request, status, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from functools import wraps

from ..core.util.redis_control_auth import (
    RedisControlAuth, 
    ControllerRole, 
    ControllerPermission,
    AuthenticationError,
    AuthorizationError
)

logger = logging.getLogger(__name__)

# Security scheme for Bearer token authentication
security = HTTPBearer(auto_error=False)


class AdminAuthMiddleware:
    """Authentication middleware for Redis admin endpoints."""
    
    def __init__(self):
        """Initialize the admin authentication middleware."""
        self.auth_manager = None
        self._init_auth_manager()
    
    def _init_auth_manager(self):
        """Initialize the authentication manager."""
        try:
            self.auth_manager = RedisControlAuth()
            logger.info("Redis admin authentication middleware initialized")
        except Exception as e:
            logger.error(f"Failed to initialize auth manager: {e}")
            # Continue without auth manager - will require emergency access
    
    async def authenticate_request(
        self, 
        request: Request,
        credentials: Optional[HTTPAuthorizationCredentials] = None,
        required_permission: ControllerPermission = ControllerPermission.READ_ONLY
    ) -> Dict[str, Any]:
        """
        Authenticate and authorize a request.
        
        Args:
            request: FastAPI request object
            credentials: HTTP authorization credentials
            required_permission: Required permission level
            
        Returns:
            User information dictionary
            
        Raises:
            HTTPException: If authentication or authorization fails
        """
        # Log the request for audit purposes
        self._log_request(request, required_permission)
        
        # Check if auth manager is available
        if not self.auth_manager:
            # Allow emergency access for critical operations
            if required_permission == ControllerPermission.SUPER_ADMIN:
                return await self._emergency_access_check(request)
            else:
                raise HTTPException(
                    status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                    detail="Authentication service unavailable"
                )
        
        try:
            # Extract token from credentials
            token = None
            if credentials:
                token = credentials.credentials
            else:
                # Try to get token from query parameters (for SSE endpoints)
                token = request.query_params.get("token")
                # Try to get API key from headers
                if not token:
                    token = request.headers.get("X-API-Key")
            
            if not token:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Authentication credentials required",
                    headers={"WWW-Authenticate": "Bearer"}
                )
            
            # Validate token and get user info
            user_info = await self._validate_token(token, request)
            
            # Check authorization
            self._check_authorization(user_info, required_permission)
            
            # Log successful authentication
            self._log_success(request, user_info, required_permission)
            
            return user_info
            
        except HTTPException:
            raise
        except Exception as e:
            logger.error(f"Authentication error: {e}")
            self._log_failure(request, str(e), required_permission)
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail="Authentication system error"
            )
    
    async def _validate_token(self, token: str, request: Request) -> Dict[str, Any]:
        """
        Validate authentication token.
        
        Args:
            token: Authentication token
            request: FastAPI request object
            
        Returns:
            User information dictionary
            
        Raises:
            HTTPException: If token validation fails
        """
        try:
            # Check if it's a JWT token or API key
            if token.startswith("eyJ"):  # JWT tokens start with "eyJ"
                user_info = self.auth_manager.validate_jwt_token(token)
            else:
                # Treat as API key
                user_info = self.auth_manager.validate_api_key(token)
            
            if not user_info:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid authentication token"
                )
            
            # Check if user is active
            if not user_info.get("active", True):
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="User account is disabled"
                )
            
            # Update last access time
            user_info["last_access"] = datetime.now()
            user_info["ip_address"] = self._get_client_ip(request)
            
            return user_info
            
        except AuthenticationError as e:
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail=str(e)
            )
        except Exception as e:
            logger.error(f"Token validation error: {e}")
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail="Token validation failed"
            )
    
    def _check_authorization(
        self, 
        user_info: Dict[str, Any], 
        required_permission: ControllerPermission
    ):
        """
        Check if user has required authorization.
        
        Args:
            user_info: User information dictionary
            required_permission: Required permission level
            
        Raises:
            HTTPException: If authorization check fails
        """
        try:
            user_role = ControllerRole(user_info.get("role", "VIEWER"))
            user_permissions = user_info.get("permissions", [])
            
            # Check role-based permissions
            if required_permission == ControllerPermission.READ_ONLY:
                # All roles have read access
                return
            elif required_permission == ControllerPermission.ADMIN:
                if user_role not in [ControllerRole.ADMIN, ControllerRole.SUPER_ADMIN]:
                    raise HTTPException(
                        status_code=status.HTTP_403_FORBIDDEN,
                        detail="Admin privileges required"
                    )
            elif required_permission == ControllerPermission.SUPER_ADMIN:
                if user_role != ControllerRole.SUPER_ADMIN:
                    raise HTTPException(
                        status_code=status.HTTP_403_FORBIDDEN,
                        detail="Super admin privileges required"
                    )
            
            # Check specific permissions if provided
            required_perm_str = required_permission.value
            if required_perm_str not in user_permissions:
                logger.warning(
                    f"User {user_info.get('user_id')} lacks permission: {required_perm_str}"
                )
                # Don't fail on permission check if role is sufficient
                # This allows for backward compatibility
            
        except ValueError as e:
            logger.error(f"Invalid user role: {user_info.get('role')}")
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail="Invalid user role"
            )
        except HTTPException:
            raise
        except Exception as e:
            logger.error(f"Authorization check error: {e}")
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail="Authorization system error"
            )
    
    async def _emergency_access_check(self, request: Request) -> Dict[str, Any]:
        """
        Handle emergency access when auth system is unavailable.
        
        Args:
            request: FastAPI request object
            
        Returns:
            Emergency user info
            
        Raises:
            HTTPException: If emergency access is denied
        """
        # Check for emergency access code in headers
        emergency_code = request.headers.get("X-Emergency-Code")
        
        if not emergency_code:
            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Authentication service unavailable. Emergency access code required."
            )
        
        # Validate emergency code (this should be configured securely)
        # In production, this should be a time-limited, cryptographically secure code
        if not self._validate_emergency_code(emergency_code, request):
            logger.critical(f"Invalid emergency access attempt from {self._get_client_ip(request)}")
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail="Invalid emergency access code"
            )
        
        logger.warning(f"Emergency access granted from {self._get_client_ip(request)}")
        
        return {
            "user_id": "emergency",
            "role": "SUPER_ADMIN",
            "permissions": ["super_admin"],
            "emergency_access": True,
            "ip_address": self._get_client_ip(request),
            "last_access": datetime.now()
        }
    
    def _validate_emergency_code(self, code: str, request: Request) -> bool:
        """
        Validate emergency access code.
        
        Args:
            code: Emergency access code
            request: FastAPI request object
            
        Returns:
            True if code is valid
        """
        # This is a simplified implementation
        # In production, use time-limited, cryptographically secure codes
        # that are generated and distributed securely
        
        # For now, accept a configured emergency code
        # This should be loaded from secure configuration
        import os
        emergency_code = os.getenv("REDIS_EMERGENCY_ACCESS_CODE")
        
        if not emergency_code:
            return False
        
        return code == emergency_code
    
    def _get_client_ip(self, request: Request) -> str:
        """Get client IP address from request."""
        # Check for forwarded headers first
        forwarded_for = request.headers.get("X-Forwarded-For")
        if forwarded_for:
            # Take the first IP in the chain
            return forwarded_for.split(",")[0].strip()
        
        real_ip = request.headers.get("X-Real-IP")
        if real_ip:
            return real_ip
        
        # Fall back to direct connection
        if hasattr(request.client, "host"):
            return request.client.host
        
        return "unknown"
    
    def _log_request(self, request: Request, permission: ControllerPermission):
        """Log incoming request for audit purposes."""
        logger.info(
            f"Admin API request: {request.method} {request.url.path} "
            f"from {self._get_client_ip(request)} "
            f"requiring {permission.value}"
        )
    
    def _log_success(
        self, 
        request: Request, 
        user_info: Dict[str, Any], 
        permission: ControllerPermission
    ):
        """Log successful authentication."""
        logger.info(
            f"Admin API authenticated: user={user_info.get('user_id')} "
            f"role={user_info.get('role')} "
            f"endpoint={request.url.path} "
            f"ip={self._get_client_ip(request)}"
        )
    
    def _log_failure(
        self, 
        request: Request, 
        error: str, 
        permission: ControllerPermission
    ):
        """Log authentication failure."""
        logger.warning(
            f"Admin API auth failed: endpoint={request.url.path} "
            f"ip={self._get_client_ip(request)} "
            f"error={error} "
            f"required_permission={permission.value}"
        )


# Global middleware instance
admin_auth = AdminAuthMiddleware()


# Dependency functions for different permission levels

async def require_read_access(
    request: Request,
    credentials: Optional[HTTPAuthorizationCredentials] = Depends(security)
) -> Dict[str, Any]:
    """Dependency for endpoints requiring read access."""
    return await admin_auth.authenticate_request(
        request, credentials, ControllerPermission.READ_ONLY
    )


async def require_admin_access(
    request: Request,
    credentials: Optional[HTTPAuthorizationCredentials] = Depends(security)
) -> Dict[str, Any]:
    """Dependency for endpoints requiring admin access."""
    return await admin_auth.authenticate_request(
        request, credentials, ControllerPermission.ADMIN
    )


async def require_super_admin_access(
    request: Request,
    credentials: Optional[HTTPAuthorizationCredentials] = Depends(security)
) -> Dict[str, Any]:
    """Dependency for endpoints requiring super admin access."""
    return await admin_auth.authenticate_request(
        request, credentials, ControllerPermission.SUPER_ADMIN
    )


# Rate limiting decorator
def rate_limit(max_requests: int = 60, window_seconds: int = 60):
    """
    Rate limiting decorator for admin endpoints.
    
    Args:
        max_requests: Maximum requests per window
        window_seconds: Time window in seconds
    """
    def decorator(func):
        @wraps(func)
        async def wrapper(*args, **kwargs):
            # Get user info from kwargs (injected by auth dependency)
            user_info = None
            for arg in args:
                if isinstance(arg, dict) and "user_id" in arg:
                    user_info = arg
                    break
            
            if user_info and admin_auth.auth_manager:
                user_id = user_info.get("user_id", "unknown")
                
                # Check rate limit
                if not admin_auth.auth_manager.check_rate_limit(
                    user_id, max_requests, window_seconds
                ):
                    raise HTTPException(
                        status_code=status.HTTP_429_TOO_MANY_REQUESTS,
                        detail=f"Rate limit exceeded: {max_requests} requests per {window_seconds} seconds"
                    )
            
            return await func(*args, **kwargs)
        return wrapper
    return decorator


# Audit logging decorator
def audit_log(operation: str, resource: str):
    """
    Audit logging decorator for admin operations.
    
    Args:
        operation: Operation name
        resource: Resource being accessed
    """
    def decorator(func):
        @wraps(func)
        async def wrapper(*args, **kwargs):
            user_info = None
            request = None
            
            # Extract user info and request from arguments
            for arg in args:
                if isinstance(arg, dict) and "user_id" in arg:
                    user_info = arg
                elif hasattr(arg, "method") and hasattr(arg, "url"):
                    request = arg
            
            start_time = time.time()
            result = "unknown"
            details = {}
            
            try:
                response = await func(*args, **kwargs)
                result = "success"
                if hasattr(response, "success"):
                    result = "success" if response.success else "failure"
                return response
            except Exception as e:
                result = "error"
                details["error"] = str(e)
                raise
            finally:
                # Log audit entry
                if user_info and admin_auth.auth_manager:
                    duration = time.time() - start_time
                    details.update({
                        "duration_seconds": round(duration, 3),
                        "endpoint": request.url.path if request else "unknown"
                    })
                    
                    admin_auth.auth_manager.log_audit_event(
                        user_id=user_info.get("user_id", "unknown"),
                        operation=operation,
                        resource=resource,
                        details=details,
                        result=result,
                        ip_address=user_info.get("ip_address")
                    )
        
        return wrapper
    return decorator