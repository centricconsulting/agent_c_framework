from typing import Optional, List
from fastapi import Depends
from fastapi_cache.decorator import cache

from agent_c.models.chat.user import ChatUser
from agent_c_api.core.repositories import get_user_repository, UserRepository
from agent_c_api.core.services.user_service import UserService as CoreUserService


# User service dependency
async def get_user_service(
    user_repository: UserRepository = Depends(get_user_repository)
) -> "UserService":
    """Dependency to get the user service
    
    Args:
        user_repository: UserRepository dependency
        
    Returns:
        UserService: Initialized user service
    """
    return UserService(user_repository)

class UserService:
    """Service for managing users"""
    
    def __init__(self, user_repository: UserRepository):
        """Initialize the user service
        
        Args:
            user_repository: UserRepository dependency
        """
        self.user_repository = user_repository
        self._core_service = CoreUserService(user_repository)
    
    async def _get_core_service(self) -> CoreUserService:
        """Get the core user service with dependencies"""
        return self._core_service
    
    async def add_user(self, user: ChatUser) -> ChatUser:
        """Create a new user
        
        Args:
            user: The user data to add
            
        Returns:
            The created user with any updated fields
        """
        return await self._core_service.add_user(user)
    
    async def get_user(self, user_id: str) -> Optional[ChatUser]:
        """Get a user by ID
        
        Args:
            user_id: The unique identifier of the user
            
        Returns:
            The user if found, None otherwise
        """
        return await self._core_service.get_user(user_id)
    
    async def update_user(self, user: ChatUser) -> ChatUser:
        """Update an existing user
        
        Args:
            user: The updated user data
            
        Returns:
            The updated user
        """
        return await self._core_service.update_user(user)
    
    async def delete_user(self, user_id: str) -> bool:
        """Delete a user by ID
        
        Args:
            user_id: The unique identifier of the user to delete
            
        Returns:
            True if user was deleted, False otherwise
        """
        return await self._core_service.delete_user(user_id)