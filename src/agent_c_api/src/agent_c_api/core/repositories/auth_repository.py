"""
Repository for user authentication operations.

This module provides database operations for user management, authentication,
and session handling for the Avatar API.
"""

import time
from datetime import datetime
from typing import Optional, List
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy.future import select
from sqlalchemy.exc import IntegrityError
import structlog

from agent_c.models.chat_history.user import ChatUser
from agent_c.util import MnemonicSlugs
from agent_c_api.models.auth_models import ChatUserTable


class AuthRepository:
    """Repository for user authentication and profile management."""
    
    def __init__(self, session: AsyncSession):
        """
        Initialize the authentication repository.
        
        Args:
            session: Async SQLAlchemy database session
        """
        self.session = session
        self.logger = structlog.get_logger(__name__)
    
    async def create_user(self, username: str, password_hash: str, email: Optional[str] = None,
                         first_name: Optional[str] = None, last_name: Optional[str] = None,
                         roles: List[str] = None,
                         user_id: Optional[str] = None) -> ChatUser:
        """
        Create a new user in the database.
        
        Args:
            username: Unique username for authentication
            password_hash: Bcrypt hashed password
            email: Optional email address
            first_name: User's first name
            last_name: User's last name
            roles: List of user roles
            user_id: Optional user ID, if not provided will be generated
            
        Returns:
            ChatUser: Created user model
            
        Raises:
            ValueError: If username already exists
        """
        start_time = time.time()
        
        try:
            # Generate user ID using mnemonic slugs
            if user_id is None:
                user_id = MnemonicSlugs.generate_id_slug(2, username)
            
            self.logger.info(
                "user_creating",
                username=username,
                user_id=user_id,
                has_email=bool(email)
            )
            
            # Create ChatUser model
            chat_user = ChatUser(
                user_id=user_id,
                user_name=username,
                email=email,
                password_hash=password_hash,
                first_name=first_name or "New",
                last_name=last_name or "User",
                roles=roles or [],
                groups=[],
                meta={},
                is_active=True,
                created_at=datetime.now().isoformat()
            )
            
            # Convert to table model and save
            table_user = ChatUserTable.from_chat_user(chat_user)
            self.session.add(table_user)
            await self.session.commit()
            await self.session.refresh(table_user)
            
            duration = time.time() - start_time
            self.logger.info(
                "user_created",
                username=username,
                user_id=user_id,
                duration_ms=round(duration * 1000, 2)
            )
            
            return table_user.to_chat_user()
            
        except IntegrityError as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.warning(
                "user_creation_failed",
                username=username,
                error="username_or_email_already_exists",
                duration_ms=round(duration * 1000, 2)
            )
            raise ValueError(f"Username '{username}' already exists") from e
        except Exception as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.error(
                "user_creation_error",
                username=username,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_user_by_username(self, username: str) -> Optional[ChatUser]:
        """
        Get user by username.
        
        Args:
            username: Username to search for
            
        Returns:
            Optional[User]: User model if found, None otherwise
        """
        start_time = time.time()
        
        try:
            self.logger.debug(
                "user_retrieving",
                username=username,
                lookup_type="username"
            )
            
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_name == username)
            )
            table_user = result.scalar_one_or_none()
            user = table_user.to_chat_user() if table_user else None
            
            duration = time.time() - start_time
            self.logger.debug(
                "user_retrieved",
                username=username,
                found=user is not None,
                duration_ms=round(duration * 1000, 2)
            )
            
            return user
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "user_retrieval_error",
                username=username,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_user_by_id(self, user_id: str) -> Optional[ChatUser]:
        """
        Get user by user ID.
        
        Args:
            user_id: User ID to search for
            
        Returns:
            Optional[User]: User model if found, None otherwise
        """
        start_time = time.time()
        
        try:
            self.logger.debug(
                "user_retrieving",
                user_id=user_id,
                lookup_type="user_id"
            )
            
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_id == user_id)
            )
            table_user = result.scalar_one_or_none()
            user = table_user.to_chat_user() if table_user else None
            
            duration = time.time() - start_time
            self.logger.debug(
                "user_retrieved",
                user_id=user_id,
                found=user is not None,
                duration_ms=round(duration * 1000, 2)
            )
            
            return user
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "user_retrieval_error",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def update_last_login(self, user_id: str) -> bool:
        """
        Update user's last login timestamp.
        
        Args:
            user_id: User ID to update
            
        Returns:
            bool: True if updated successfully
        """
        start_time = time.time()
        
        try:
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_id == user_id)
            )
            table_user = result.scalar_one_or_none()
            user = table_user.to_chat_user() if table_user else None
            
            if user:
                user.last_login = datetime.utcnow()
                await self.session.commit()
                
                duration = time.time() - start_time
                self.logger.debug(
                    "user_login_updated",
                    user_id=user_id,
                    duration_ms=round(duration * 1000, 2)
                )
                return True
            
            return False
            
        except Exception as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.error(
                "user_login_update_error",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def list_users(self) -> List[ChatUser]:
        """
        Get all users in the system.
        
        Returns:
            List[User]: List of all users
        """
        start_time = time.time()
        
        try:
            result = await self.session.execute(select(ChatUserTable))
            table_users = result.scalars().all()
            users = [table_user.to_chat_user() for table_user in table_users]
            
            duration = time.time() - start_time
            self.logger.debug(
                "users_listed",
                count=len(users),
                duration_ms=round(duration * 1000, 2)
            )
            
            return users
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "users_list_error",
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def update_user(
        self,
        user_id: str,
        email: Optional[str] = None,
        first_name: Optional[str] = None,
        last_name: Optional[str] = None,
        roles: Optional[List[str]] = None,
        is_active: Optional[bool] = None
    ) -> Optional[ChatUser]:
        """
        Update user profile information.
        
        Only provided fields (not None) will be updated. This supports partial updates.
        
        Args:
            user_id: User ID to update
            email: New email address (optional)
            first_name: New first name (optional)
            last_name: New last name (optional)
            roles: New roles list (optional)
            is_active: New active status (optional)
            
        Returns:
            Optional[ChatUser]: Updated user model if found, None if user doesn't exist
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "user_updating",
                user_id=user_id,
                has_email=email is not None,
                has_first_name=first_name is not None,
                has_last_name=last_name is not None,
                has_roles=roles is not None,
                has_is_active=is_active is not None
            )
            
            # Get existing user
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_id == user_id)
            )
            table_user = result.scalar_one_or_none()
            
            if not table_user:
                self.logger.warning(
                    "user_update_not_found",
                    user_id=user_id
                )
                return None
            
            # Update only provided fields
            if email is not None:
                table_user.email = email
            if first_name is not None:
                table_user.first_name = first_name
            if last_name is not None:
                table_user.last_name = last_name
            if roles is not None:
                table_user.roles = roles
            if is_active is not None:
                table_user.is_active = is_active
            
            # Update timestamp
            table_user.updated_at = datetime.now().isoformat()
            
            # Commit changes
            await self.session.commit()
            await self.session.refresh(table_user)
            
            duration = time.time() - start_time
            self.logger.info(
                "user_updated",
                user_id=user_id,
                duration_ms=round(duration * 1000, 2)
            )
            
            return table_user.to_chat_user()
            
        except Exception as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.error(
                "user_update_error",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def update_password(self, user_id: str, password_hash: str) -> bool:
        """
        Update user's password hash.
        
        Args:
            user_id: User ID whose password to update
            password_hash: New bcrypt hashed password
            
        Returns:
            bool: True if password was updated, False if user not found
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "user_password_updating",
                user_id=user_id
            )
            
            # Get existing user
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_id == user_id)
            )
            table_user = result.scalar_one_or_none()
            
            if not table_user:
                self.logger.warning(
                    "user_password_update_not_found",
                    user_id=user_id
                )
                return False
            
            # Update password and timestamp
            table_user.password_hash = password_hash
            table_user.updated_at = datetime.now().isoformat()
            
            # Commit changes
            await self.session.commit()
            
            duration = time.time() - start_time
            self.logger.info(
                "user_password_updated",
                user_id=user_id,
                duration_ms=round(duration * 1000, 2)
            )
            
            return True
            
        except Exception as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.error(
                "user_password_update_error",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def delete_user(self, user_id: str) -> bool:
        """
        Delete a user from the database.
        
        Args:
            user_id: User ID to delete
            
        Returns:
            bool: True if user was deleted, False if not found
        """
        start_time = time.time()
        
        try:
            result = await self.session.execute(
                select(ChatUserTable).where(ChatUserTable.user_id == user_id)
            )
            table_user = result.scalar_one_or_none()
            
            if table_user:
                await self.session.delete(table_user)
                await self.session.commit()
                
                duration = time.time() - start_time
                self.logger.info(
                    "user_deleted",
                    user_id=user_id,
                    username=table_user.user_name,
                    duration_ms=round(duration * 1000, 2)
                )
                return True
            
            return False
            
        except Exception as e:
            await self.session.rollback()
            duration = time.time() - start_time
            self.logger.error(
                "user_deletion_error",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise