"""
Authentication service for user management and authentication.

This module provides high-level authentication operations including password
hashing, user authentication, and token management for the Avatar API.
"""

import time
from typing import Optional, Dict, Any, List
from datetime import timedelta
import structlog
from passlib.context import CryptContext
from sqlalchemy.ext.asyncio import AsyncSession

from agent_c.models.chat_history.user import ChatUser
from agent_c_api.config.database import get_database_config
from agent_c_api.core.repositories.auth_repository import AuthRepository
from agent_c_api.core.util.jwt import create_jwt_token, verify_jwt_token
from agent_c_api.models.auth_models import UserCreateRequest, ChatUserResponse, LoginResponse


class AuthService:
    """Service for user authentication and management operations."""
    
    def __init__(self):
        """
        Initialize the authentication service as a singleton.
        
        This service manages its own database session lifecycle and should be
        initialized once during application startup.
        """
        self.logger = structlog.get_logger(__name__)
        self.db_session: Optional[AsyncSession] = None
        self.auth_repo: Optional[AuthRepository] = None
        
        # Initialize password hashing context
        self.pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
    
    async def initialize(self):
        """
        Initialize the database session and repository.
        
        This should be called during application startup after the service
        is created but before it's used.
        """
        try:
            self.logger.info("auth_service_initializing")
            
            # Get database configuration and create session
            db_config = get_database_config()
            self.db_session = db_config.async_session_factory()
            
            # Initialize repository with the session
            self.auth_repo = AuthRepository(self.db_session)
            
            self.logger.info("auth_service_initialized")

            user = await self.auth_repo.get_user_by_username("admin")
            if not user:
                self.logger.info("creating_default_admin_user")
                await self.create_user(
                    UserCreateRequest(
                        username="admin",
                        password="changeme",  # In production, use a secure password or environment variable
                        email="changeme@centricconsulting.com",
                        first_name="Admin",
                        last_name="User",
                        roles=["admin"],
                        user_id="admin"  # Fixed ID for default admin user
                    )
                )
                self.logger.info("default_admin_user_created")
            
        except Exception as e:
            self.logger.error(
                "auth_service_initialization_failed",
                error=str(e)
            )
            raise
    
    async def close(self):
        """
        Close the database session and cleanup resources.
        
        This should be called during application shutdown.
        """
        try:
            self.logger.info("auth_service_closing")
            
            if self.db_session:
                await self.db_session.close()
                self.db_session = None
            
            self.auth_repo = None
            
            self.logger.info("auth_service_closed")
            
        except Exception as e:
            self.logger.error(
                "auth_service_close_error",
                error=str(e)
            )
    
    def _ensure_initialized(self):
        """
        Ensure the service is properly initialized.
        
        Raises:
            RuntimeError: If the service hasn't been initialized
        """
        if not self.db_session or not self.auth_repo:
            raise RuntimeError(
                "AuthService not initialized. Call initialize() during app startup."
            )
    
    def _hash_password(self, password: str) -> str:
        """
        Hash a password using bcrypt.
        
        Args:
            password: Plain text password
            
        Returns:
            str: Hashed password
        """
        return self.pwd_context.hash(password)
    
    def _verify_password(self, plain_password: str, hashed_password: str) -> bool:
        """
        Verify a password against its hash.
        
        Args:
            plain_password: Plain text password to verify
            hashed_password: Stored hash to verify against
            
        Returns:
            bool: True if password matches
        """
        try:
            return self.pwd_context.verify(plain_password, hashed_password)
        except Exception as e:
            self.logger.error(
                "password_verification_failed",
                error_type=type(e).__name__,
                hash_prefix=hashed_password[:10] if hashed_password else "None",
                hash_length=len(hashed_password) if hashed_password else 0
            )
            return False
    
    async def create_user(self, user_request: UserCreateRequest) -> ChatUserResponse:
        """
        Create a new user with hashed password.
        
        Args:
            user_request: User creation request data
            
        Returns:
            UserResponse: Created user data (without password)
            
        Raises:
            ValueError: If username already exists
        """
        self._ensure_initialized()
        start_time = time.time()
        
        try:
            self.logger.info(
                "auth_user_creating",
                username=user_request.username,
                has_email=bool(user_request.email),
                roles=user_request.roles
            )
            
            # Hash the password
            password_hash = self._hash_password(user_request.password)
            
            # Create user in database
            user = await self.auth_repo.create_user(
                username=user_request.username,
                password_hash=password_hash,
                email=user_request.email,
                first_name=user_request.first_name,
                last_name=user_request.last_name,
                roles=user_request.roles,
                user_id=user_request.user_id
            )
            
            duration = time.time() - start_time
            self.logger.info(
                "auth_user_created",
                username=user_request.username,
                user_id=user.user_id,
                duration_ms=round(duration * 1000, 2)
            )
            
            return ChatUserResponse.from_chat_user(user)
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "auth_user_creation_failed",
                username=user_request.username,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def authenticate_user(self, username: str, password: str) -> Optional[ChatUser]:
        """
        Authenticate a user with username and password.
        
        Args:
            username: Username to authenticate
            password: Plain text password
            
        Returns:
            Optional[User]: User model if authentication successful, None otherwise
        """
        self._ensure_initialized()
        start_time = time.time()
        
        try:
            self.logger.info(
                "auth_user_authenticating",
                username=username
            )
            
            # Get user from database
            user = await self.auth_repo.get_user_by_username(username)
            if not user:
                duration = time.time() - start_time
                self.logger.warning(
                    "auth_user_not_found",
                    username=username,
                    duration_ms=round(duration * 1000, 2)
                )
                return None
            
            # Check if user is active
            if not user.is_active:
                duration = time.time() - start_time
                self.logger.warning(
                    "auth_user_inactive",
                    username=username,
                    user_id=user.user_id,
                    duration_ms=round(duration * 1000, 2)
                )
                return None
            
            # Verify password
            if not self._verify_password(password, user.password_hash):
                duration = time.time() - start_time
                self.logger.warning(
                    "auth_password_invalid",
                    username=username,
                    user_id=user.user_id,
                    duration_ms=round(duration * 1000, 2)
                )
                return None
            
            # Update last login time
            await self.auth_repo.update_last_login(user.user_id)
            
            duration = time.time() - start_time
            self.logger.info(
                "auth_user_authenticated",
                username=username,
                user_id=user.user_id,
                duration_ms=round(duration * 1000, 2)
            )
            
            return user
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "auth_authentication_error",
                username=username,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def login(self, username: str, password: str) -> Optional[LoginResponse]:
        """
        Perform user login and return JWT token with user data.
        
        Args:
            username: Username to authenticate
            password: Plain text password
            
        Returns:
            Optional[LoginResponse]: Login response with token and user data, or None if failed
        """
        self._ensure_initialized()
        start_time = time.time()
        
        try:
            # Authenticate user
            user = await self.authenticate_user(username, password)
            if not user:
                return None
            
            # Generate JWT token
            token = create_jwt_token(
                user=user,
                time_delta=timedelta(days=31)  # 24-hour token expiration
            )
            
            # Prepare response
            user_response = ChatUserResponse.from_chat_user(user)

            duration = time.time() - start_time
            self.logger.info(
                "auth_login_successful",
                username=username,
                user_id=user.user_id,
                token_created=True,
                duration_ms=round(duration * 1000, 2)
            )
            
            return LoginResponse(token=token, user=user_response)
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "auth_login_error",
                username=username,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_user_by_token(self, token: str) -> Optional[ChatUser]:
        """
        Get user information from JWT token.
        
        Args:
            token: JWT token string
            
        Returns:
            Optional[ChatUser]: User model if token is valid, None otherwise
        """
        self._ensure_initialized()
        try:
            # Verify and decode token
            payload = verify_jwt_token(token)
            user_id = payload.get("user_id")
            
            if not user_id:
                self.logger.warning("auth_token_missing_user_id")
                return None
            
            # Get user from database
            user = await self.auth_repo.get_user_by_id(user_id)
            if not user or not user.is_active:
                self.logger.warning(
                    "auth_token_user_invalid",
                    user_id=user_id,
                    user_found=user is not None,
                    user_active=user.is_active if user else False
                )
                return None
            
            # Convert to ChatUser for framework compatibility
            return user.to_chat_user()
            
        except Exception as e:
            self.logger.warning(
                "auth_token_verification_failed",
                error=str(e)
            )
            return None
    
    async def list_users(self) -> List[ChatUserResponse]:
        """
        Get list of all users.
        
        Returns:
            List[UserResponse]: List of all users (without passwords)
        """
        self._ensure_initialized()
        users = await self.auth_repo.list_users()
        return [ChatUserResponse.from_chat_user(user) for user in users]
    
    async def delete_user(self, user_id: str) -> bool:
        """
        Delete a user by ID.
        
        Args:
            user_id: User ID to delete
            
        Returns:
            bool: True if user was deleted, False if not found
        """
        self._ensure_initialized()
        self.logger.info("auth_user_deleting", user_id=user_id)
        result = await self.auth_repo.delete_user(user_id)
        
        if result:
            self.logger.info("auth_user_deleted", user_id=user_id)
        else:
            self.logger.warning("auth_user_delete_not_found", user_id=user_id)
        
        return result