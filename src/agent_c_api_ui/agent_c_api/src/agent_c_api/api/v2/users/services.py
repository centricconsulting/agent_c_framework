from typing import Optional, List
from fastapi_cache.decorator import cache

from agent_c.models.chat_history.user import ChatUser
from agent_c_api.config.redis_config import RedisConfig
from agent_c_api.core.repositories.user_repository import UserRepository
from agent_c_api.core.services.user_service import UserService as CoreUserService

class UserService:
    """Service for managing users"""
    
    async def _get_core_service(self) -> CoreUserService:
        """Get the core user service with dependencies"""
        redis_client = await RedisConfig.get_redis_client()
        user_repository = UserRepository(redis_client)
        return CoreUserService(user_repository)
    
    async def add_user(self, user: ChatUser) -> ChatUser:
        """Create a new user
        
        Args:
            user: The user data to add
            
        Returns:
            The created user with any updated fields
        """
        core_service = await self._get_core_service()
        return await core_service.add_user(user)
    
    async def get_user(self, user_id: str) -> Optional[ChatUser]:
        """Get a user by ID
        
        Args:
            user_id: The unique identifier of the user
            
        Returns:
            The user if found, None otherwise
        """
        core_service = await self._get_core_service()
        return await core_service.get_user(user_id)
    
    async def update_user(self, user: ChatUser) -> ChatUser:
        """Update an existing user
        
        Args:
            user: The updated user data
            
        Returns:
            The updated user
        """
        core_service = await self._get_core_service()
        return await core_service.update_user(user)
    
    async def delete_user(self, user_id: str) -> bool:
        """Delete a user by ID
        
        Args:
            user_id: The unique identifier of the user to delete
            
        Returns:
            True if user was deleted, False otherwise
        """
        core_service = await self._get_core_service()
        return await core_service.delete_user(user_id)