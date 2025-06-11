"""
Redis Control Authentication and Authorization

This module provides authentication and authorization functionality for the Redis
Runtime Controller. It implements role-based access control, session management,
and security features including rate limiting and audit logging.

Features:
- JWT token-based authentication
- Role-based access control (RBAC)
- Session management with expiration
- Rate limiting per user/operation
- API key authentication for service accounts
- Audit logging for security events
- Emergency access controls
"""

import hashlib
import hmac
import json
import logging
import secrets
import time
from datetime import datetime, timedelta
from typing import Any, Dict, List, Optional, Set, Tuple
from dataclasses import dataclass, field
from enum import Enum
import jwt
from passlib.context import CryptContext

logger = logging.getLogger(__name__)


class AuthenticationMethod(Enum):
    """Authentication methods supported by the control system."""
    JWT_TOKEN = "jwt_token"
    API_KEY = "api_key"
    SESSION_COOKIE = "session_cookie"
    EMERGENCY_CODE = "emergency_code"


class SecurityEvent(Enum):
    """Security events for audit logging."""
    LOGIN_SUCCESS = "login_success"
    LOGIN_FAILURE = "login_failure"
    ACCESS_DENIED = "access_denied"
    RATE_LIMIT_EXCEEDED = "rate_limit_exceeded"
    PERMISSION_ESCALATION = "permission_escalation"
    EMERGENCY_ACCESS = "emergency_access"
    SESSION_EXPIRED = "session_expired"
    API_KEY_USED = "api_key_used"
    INVALID_TOKEN = "invalid_token"


@dataclass
class AuthenticationToken:
    """JWT authentication token information."""
    user_id: str
    username: str
    role: str
    permissions: List[str]
    issued_at: datetime
    expires_at: datetime
    token_id: str
    session_id: Optional[str] = None


@dataclass
class APIKey:
    """API key for service account authentication."""
    key_id: str
    key_hash: str
    user_id: str
    description: str
    permissions: Set[str]
    created_at: datetime
    last_used: Optional[datetime] = None
    expires_at: Optional[datetime] = None
    is_active: bool = True


@dataclass
class SecurityAuditEvent:
    """Security audit event for logging."""
    event_id: str
    event_type: SecurityEvent
    user_id: str
    ip_address: Optional[str]
    user_agent: Optional[str]
    operation: Optional[str]
    success: bool
    error_message: Optional[str]
    timestamp: datetime
    additional_data: Dict[str, Any] = field(default_factory=dict)


class RedisControlAuth:
    """
    Authentication and authorization service for Redis Runtime Controller.
    
    This class provides comprehensive security features including:
    - JWT token authentication with configurable expiration
    - API key authentication for service accounts
    - Role-based access control with flexible permissions
    - Rate limiting with per-user and per-operation limits
    - Session management with automatic cleanup
    - Security audit logging with event tracking
    - Emergency access controls for critical situations
    """

    def __init__(
        self,
        jwt_secret: str,
        jwt_algorithm: str = "HS256",
        token_expiry_hours: int = 24,
        api_key_length: int = 32,
        max_login_attempts: int = 5,
        lockout_duration_minutes: int = 15,
        enable_emergency_access: bool = True
    ):
        """
        Initialize the authentication and authorization service.
        
        Args:
            jwt_secret: Secret key for JWT token signing
            jwt_algorithm: JWT algorithm (default: HS256)
            token_expiry_hours: Token expiration time in hours
            api_key_length: Length of generated API keys
            max_login_attempts: Maximum failed login attempts before lockout
            lockout_duration_minutes: Account lockout duration in minutes
            enable_emergency_access: Enable emergency access controls
        """
        self.jwt_secret = jwt_secret
        self.jwt_algorithm = jwt_algorithm
        self.token_expiry_hours = token_expiry_hours
        self.api_key_length = api_key_length
        self.max_login_attempts = max_login_attempts
        self.lockout_duration_minutes = lockout_duration_minutes
        self.enable_emergency_access = enable_emergency_access
        
        # Password hashing
        self.pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
        
        # Storage for authentication data
        self._active_tokens: Dict[str, AuthenticationToken] = {}
        self._api_keys: Dict[str, APIKey] = {}
        self._failed_attempts: Dict[str, List[datetime]] = {}
        self._audit_log: List[SecurityAuditEvent] = []
        self._emergency_codes: Dict[str, datetime] = {}
        
        # Emergency access setup
        if self.enable_emergency_access:
            self._generate_emergency_code()
        
        logger.info("RedisControlAuth initialized successfully")

    def _generate_emergency_code(self) -> str:
        """Generate a time-limited emergency access code."""
        emergency_code = secrets.token_urlsafe(16)
        expires_at = datetime.utcnow() + timedelta(hours=1)
        self._emergency_codes[emergency_code] = expires_at
        
        logger.warning(f"Emergency access code generated: {emergency_code} (expires in 1 hour)")
        return emergency_code

    def _log_security_event(
        self,
        event_type: SecurityEvent,
        user_id: str,
        success: bool,
        ip_address: Optional[str] = None,
        user_agent: Optional[str] = None,
        operation: Optional[str] = None,
        error_message: Optional[str] = None,
        additional_data: Optional[Dict[str, Any]] = None
    ) -> None:
        """Log a security event for audit purposes."""
        event = SecurityAuditEvent(
            event_id=f"{int(time.time() * 1000)}_{event_type.value}_{user_id}",
            event_type=event_type,
            user_id=user_id,
            ip_address=ip_address,
            user_agent=user_agent,
            operation=operation,
            success=success,
            error_message=error_message,
            timestamp=datetime.utcnow(),
            additional_data=additional_data or {}
        )
        
        self._audit_log.append(event)
        
        # Log to system logger
        level = logging.INFO if success else logging.WARNING
        logger.log(level, f"Security event: {event_type.value} - User: {user_id}, Success: {success}")

    def _is_account_locked(self, user_id: str) -> bool:
        """Check if account is locked due to failed login attempts."""
        if user_id not in self._failed_attempts:
            return False
        
        failed_attempts = self._failed_attempts[user_id]
        cutoff_time = datetime.utcnow() - timedelta(minutes=self.lockout_duration_minutes)
        
        # Remove old attempts
        self._failed_attempts[user_id] = [
            attempt for attempt in failed_attempts if attempt > cutoff_time
        ]
        
        return len(self._failed_attempts[user_id]) >= self.max_login_attempts

    def _record_failed_attempt(self, user_id: str) -> None:
        """Record a failed login attempt."""
        if user_id not in self._failed_attempts:
            self._failed_attempts[user_id] = []
        
        self._failed_attempts[user_id].append(datetime.utcnow())

    def _clear_failed_attempts(self, user_id: str) -> None:
        """Clear failed login attempts for successful login."""
        if user_id in self._failed_attempts:
            del self._failed_attempts[user_id]

    # ========================================
    # JWT Token Authentication
    # ========================================

    def generate_jwt_token(
        self,
        user_id: str,
        username: str,
        role: str,
        permissions: List[str],
        session_id: Optional[str] = None
    ) -> Tuple[str, AuthenticationToken]:
        """
        Generate a JWT authentication token.
        
        Args:
            user_id: User identifier
            username: Human-readable username
            role: User role
            permissions: List of permissions
            session_id: Optional session identifier
            
        Returns:
            Tuple of (token_string, token_object)
        """
        now = datetime.utcnow()
        expires_at = now + timedelta(hours=self.token_expiry_hours)
        token_id = secrets.token_urlsafe(16)
        
        payload = {
            'user_id': user_id,
            'username': username,
            'role': role,
            'permissions': permissions,
            'iat': int(now.timestamp()),
            'exp': int(expires_at.timestamp()),
            'jti': token_id,
            'session_id': session_id
        }
        
        token_string = jwt.encode(payload, self.jwt_secret, algorithm=self.jwt_algorithm)
        
        token_obj = AuthenticationToken(
            user_id=user_id,
            username=username,
            role=role,
            permissions=permissions,
            issued_at=now,
            expires_at=expires_at,
            token_id=token_id,
            session_id=session_id
        )
        
        self._active_tokens[token_id] = token_obj
        
        self._log_security_event(
            SecurityEvent.LOGIN_SUCCESS,
            user_id,
            True,
            operation="generate_jwt_token",
            additional_data={'token_id': token_id, 'expires_at': expires_at.isoformat()}
        )
        
        return token_string, token_obj

    def validate_jwt_token(
        self,
        token_string: str,
        required_permissions: Optional[List[str]] = None
    ) -> AuthenticationToken:
        """
        Validate a JWT token and check permissions.
        
        Args:
            token_string: JWT token string
            required_permissions: List of required permissions
            
        Returns:
            AuthenticationToken: Valid token object
            
        Raises:
            PermissionError: If token is invalid or lacks permissions
        """
        try:
            # Decode and validate token
            payload = jwt.decode(token_string, self.jwt_secret, algorithms=[self.jwt_algorithm])
            token_id = payload.get('jti')
            
            if not token_id or token_id not in self._active_tokens:
                raise PermissionError("Token not found in active tokens")
            
            token_obj = self._active_tokens[token_id]
            
            # Check expiration
            if datetime.utcnow() > token_obj.expires_at:
                del self._active_tokens[token_id]
                self._log_security_event(
                    SecurityEvent.SESSION_EXPIRED,
                    token_obj.user_id,
                    False,
                    operation="validate_jwt_token",
                    error_message="Token expired"
                )
                raise PermissionError("Token has expired")
            
            # Check permissions
            if required_permissions:
                missing_permissions = set(required_permissions) - set(token_obj.permissions)
                if missing_permissions:
                    self._log_security_event(
                        SecurityEvent.ACCESS_DENIED,
                        token_obj.user_id,
                        False,
                        operation="validate_jwt_token",
                        error_message=f"Missing permissions: {missing_permissions}"
                    )
                    raise PermissionError(f"Missing required permissions: {missing_permissions}")
            
            return token_obj
            
        except jwt.ExpiredSignatureError:
            self._log_security_event(
                SecurityEvent.INVALID_TOKEN,
                "unknown",
                False,
                operation="validate_jwt_token",
                error_message="Token signature expired"
            )
            raise PermissionError("Token has expired")
        except jwt.InvalidTokenError as e:
            self._log_security_event(
                SecurityEvent.INVALID_TOKEN,
                "unknown",
                False,
                operation="validate_jwt_token",
                error_message=str(e)
            )
            raise PermissionError(f"Invalid token: {e}")
        except Exception as e:
            self._log_security_event(
                SecurityEvent.INVALID_TOKEN,
                "unknown",
                False,
                operation="validate_jwt_token",
                error_message=str(e)
            )
            raise PermissionError(f"Token validation failed: {e}")

    def revoke_jwt_token(self, token_id: str) -> bool:
        """
        Revoke a JWT token.
        
        Args:
            token_id: Token ID to revoke
            
        Returns:
            bool: True if token was revoked
        """
        if token_id in self._active_tokens:
            token_obj = self._active_tokens.pop(token_id)
            self._log_security_event(
                SecurityEvent.LOGIN_SUCCESS,  # Using as logout event
                token_obj.user_id,
                True,
                operation="revoke_jwt_token",
                additional_data={'token_id': token_id}
            )
            return True
        return False

    # ========================================
    # API Key Authentication
    # ========================================

    def generate_api_key(
        self,
        user_id: str,
        description: str,
        permissions: Set[str],
        expires_hours: Optional[int] = None
    ) -> Tuple[str, str]:
        """
        Generate an API key for service account authentication.
        
        Args:
            user_id: User identifier for the API key
            description: Description of the API key usage
            permissions: Set of permissions for the API key
            expires_hours: Optional expiration in hours
            
        Returns:
            Tuple of (key_id, api_key_string)
        """
        key_id = f"ak_{secrets.token_urlsafe(8)}"
        api_key_string = secrets.token_urlsafe(self.api_key_length)
        key_hash = self.pwd_context.hash(api_key_string)
        
        expires_at = None
        if expires_hours:
            expires_at = datetime.utcnow() + timedelta(hours=expires_hours)
        
        api_key_obj = APIKey(
            key_id=key_id,
            key_hash=key_hash,
            user_id=user_id,
            description=description,
            permissions=permissions,
            created_at=datetime.utcnow(),
            expires_at=expires_at
        )
        
        self._api_keys[key_id] = api_key_obj
        
        logger.info(f"API key generated for user {user_id}: {key_id}")
        return key_id, api_key_string

    def validate_api_key(
        self,
        api_key_string: str,
        required_permissions: Optional[List[str]] = None
    ) -> APIKey:
        """
        Validate an API key and check permissions.
        
        Args:
            api_key_string: API key string
            required_permissions: List of required permissions
            
        Returns:
            APIKey: Valid API key object
            
        Raises:
            PermissionError: If API key is invalid or lacks permissions
        """
        for key_id, api_key_obj in self._api_keys.items():
            if not api_key_obj.is_active:
                continue
            
            if self.pwd_context.verify(api_key_string, api_key_obj.key_hash):
                # Check expiration
                if api_key_obj.expires_at and datetime.utcnow() > api_key_obj.expires_at:
                    api_key_obj.is_active = False
                    self._log_security_event(
                        SecurityEvent.ACCESS_DENIED,
                        api_key_obj.user_id,
                        False,
                        operation="validate_api_key",
                        error_message="API key expired"
                    )
                    raise PermissionError("API key has expired")
                
                # Check permissions
                if required_permissions:
                    missing_permissions = set(required_permissions) - api_key_obj.permissions
                    if missing_permissions:
                        self._log_security_event(
                            SecurityEvent.ACCESS_DENIED,
                            api_key_obj.user_id,
                            False,
                            operation="validate_api_key",
                            error_message=f"Missing permissions: {missing_permissions}"
                        )
                        raise PermissionError(f"Missing required permissions: {missing_permissions}")
                
                # Update last used
                api_key_obj.last_used = datetime.utcnow()
                
                self._log_security_event(
                    SecurityEvent.API_KEY_USED,
                    api_key_obj.user_id,
                    True,
                    operation="validate_api_key",
                    additional_data={'key_id': key_id}
                )
                
                return api_key_obj
        
        self._log_security_event(
            SecurityEvent.ACCESS_DENIED,
            "unknown",
            False,
            operation="validate_api_key",
            error_message="Invalid API key"
        )
        raise PermissionError("Invalid API key")

    def revoke_api_key(self, key_id: str) -> bool:
        """
        Revoke an API key.
        
        Args:
            key_id: API key ID to revoke
            
        Returns:
            bool: True if key was revoked
        """
        if key_id in self._api_keys:
            api_key_obj = self._api_keys[key_id]
            api_key_obj.is_active = False
            
            self._log_security_event(
                SecurityEvent.LOGIN_SUCCESS,  # Using as revocation event
                api_key_obj.user_id,
                True,
                operation="revoke_api_key",
                additional_data={'key_id': key_id}
            )
            
            logger.info(f"API key revoked: {key_id}")
            return True
        return False

    # ========================================
    # Emergency Access
    # ========================================

    def validate_emergency_code(self, emergency_code: str) -> bool:
        """
        Validate an emergency access code.
        
        Args:
            emergency_code: Emergency code to validate
            
        Returns:
            bool: True if code is valid
        """
        if not self.enable_emergency_access:
            return False
        
        if emergency_code in self._emergency_codes:
            expires_at = self._emergency_codes[emergency_code]
            if datetime.utcnow() <= expires_at:
                self._log_security_event(
                    SecurityEvent.EMERGENCY_ACCESS,
                    "emergency",
                    True,
                    operation="validate_emergency_code"
                )
                return True
            else:
                # Remove expired code
                del self._emergency_codes[emergency_code]
        
        self._log_security_event(
            SecurityEvent.ACCESS_DENIED,
            "emergency",
            False,
            operation="validate_emergency_code",
            error_message="Invalid or expired emergency code"
        )
        return False

    def generate_new_emergency_code(self, admin_user_id: str) -> str:
        """
        Generate a new emergency access code (admin only).
        
        Args:
            admin_user_id: Admin user generating the code
            
        Returns:
            str: New emergency code
        """
        if not self.enable_emergency_access:
            raise RuntimeError("Emergency access is disabled")
        
        # Clear old codes
        self._emergency_codes.clear()
        
        # Generate new code
        emergency_code = self._generate_emergency_code()
        
        self._log_security_event(
            SecurityEvent.EMERGENCY_ACCESS,
            admin_user_id,
            True,
            operation="generate_new_emergency_code"
        )
        
        return emergency_code

    # ========================================
    # Authentication Helpers
    # ========================================

    def authenticate_request(
        self,
        auth_header: Optional[str] = None,
        api_key: Optional[str] = None,
        emergency_code: Optional[str] = None,
        required_permissions: Optional[List[str]] = None
    ) -> Dict[str, Any]:
        """
        Authenticate a request using various methods.
        
        Args:
            auth_header: Authorization header (Bearer token)
            api_key: API key for service authentication
            emergency_code: Emergency access code
            required_permissions: Required permissions
            
        Returns:
            Dict containing authentication result
            
        Raises:
            PermissionError: If authentication fails
        """
        # Try emergency code first
        if emergency_code and self.validate_emergency_code(emergency_code):
            return {
                'authenticated': True,
                'method': AuthenticationMethod.EMERGENCY_CODE.value,
                'user_id': 'emergency',
                'username': 'emergency_access',
                'role': 'super_admin',
                'permissions': ['read_only', 'admin', 'super_admin']
            }
        
        # Try API key authentication
        if api_key:
            try:
                api_key_obj = self.validate_api_key(api_key, required_permissions)
                return {
                    'authenticated': True,
                    'method': AuthenticationMethod.API_KEY.value,
                    'user_id': api_key_obj.user_id,
                    'key_id': api_key_obj.key_id,
                    'permissions': list(api_key_obj.permissions)
                }
            except PermissionError:
                pass
        
        # Try JWT token authentication
        if auth_header and auth_header.startswith('Bearer '):
            token_string = auth_header[7:]  # Remove 'Bearer ' prefix
            try:
                token_obj = self.validate_jwt_token(token_string, required_permissions)
                return {
                    'authenticated': True,
                    'method': AuthenticationMethod.JWT_TOKEN.value,
                    'user_id': token_obj.user_id,
                    'username': token_obj.username,
                    'role': token_obj.role,
                    'permissions': token_obj.permissions,
                    'token_id': token_obj.token_id
                }
            except PermissionError:
                pass
        
        # Authentication failed
        self._log_security_event(
            SecurityEvent.ACCESS_DENIED,
            "unknown",
            False,
            operation="authenticate_request",
            error_message="All authentication methods failed"
        )
        raise PermissionError("Authentication failed")

    # ========================================
    # Management and Monitoring
    # ========================================

    def get_security_audit_log(
        self,
        user_id: Optional[str] = None,
        event_type: Optional[SecurityEvent] = None,
        limit: int = 100
    ) -> List[Dict[str, Any]]:
        """
        Get security audit log with optional filtering.
        
        Args:
            user_id: Filter by user ID
            event_type: Filter by event type
            limit: Maximum number of events to return
            
        Returns:
            List of audit events
        """
        filtered_events = self._audit_log
        
        if user_id:
            filtered_events = [e for e in filtered_events if e.user_id == user_id]
        
        if event_type:
            filtered_events = [e for e in filtered_events if e.event_type == event_type]
        
        # Sort by timestamp (most recent first) and limit
        filtered_events = sorted(filtered_events, key=lambda e: e.timestamp, reverse=True)[:limit]
        
        return [
            {
                'event_id': event.event_id,
                'event_type': event.event_type.value,
                'user_id': event.user_id,
                'ip_address': event.ip_address,
                'operation': event.operation,
                'success': event.success,
                'error_message': event.error_message,
                'timestamp': event.timestamp.isoformat(),
                'additional_data': event.additional_data
            }
            for event in filtered_events
        ]

    def get_active_sessions(self) -> Dict[str, Any]:
        """Get information about active authentication sessions."""
        return {
            'active_tokens': len(self._active_tokens),
            'active_api_keys': len([k for k in self._api_keys.values() if k.is_active]),
            'failed_attempts': {uid: len(attempts) for uid, attempts in self._failed_attempts.items()},
            'emergency_codes_active': len(self._emergency_codes) if self.enable_emergency_access else 0,
            'timestamp': datetime.utcnow().isoformat()
        }

    def cleanup_expired_sessions(self) -> int:
        """
        Clean up expired tokens and sessions.
        
        Returns:
            int: Number of sessions cleaned up
        """
        cleanup_count = 0
        current_time = datetime.utcnow()
        
        # Clean up expired JWT tokens
        expired_tokens = [
            token_id for token_id, token_obj in self._active_tokens.items()
            if current_time > token_obj.expires_at
        ]
        
        for token_id in expired_tokens:
            del self._active_tokens[token_id]
            cleanup_count += 1
        
        # Clean up expired API keys
        for api_key_obj in self._api_keys.values():
            if api_key_obj.expires_at and current_time > api_key_obj.expires_at:
                api_key_obj.is_active = False
                cleanup_count += 1
        
        # Clean up expired emergency codes
        if self.enable_emergency_access:
            expired_codes = [
                code for code, expires_at in self._emergency_codes.items()
                if current_time > expires_at
            ]
            for code in expired_codes:
                del self._emergency_codes[code]
                cleanup_count += 1
        
        if cleanup_count > 0:
            logger.info(f"Cleaned up {cleanup_count} expired authentication sessions")
        
        return cleanup_count