from datetime import datetime
from typing import Optional
from agent_c.models.chat_history.user import ChatUser
from redis import asyncio as aioredis


class UserRepository:
    def __init__(self, redis_client: aioredis.Redis):
        self.redis = redis_client

    async def add_user(self, user: ChatUser) -> ChatUser:
        """
        Add a new chat user to Redis.

        Args:
            user (ChatUser): The user to add

        Returns:
            ChatUser: The added user with any updated fields
        """
        # Create hash with user data
        user_key = f"user:{user.user_id}"

        # Get user data using Pydantic model_dump
        user_data = user.model_dump()
        
        # Special handling for metadata since Redis can't store dictionaries directly
        user_data["metadata"] = str(user_data["metadata"] or {})
        
        # Store in Redis
        await self.redis.hset(user_key, mapping=user_data)

        # Add to user index
        await self.redis.sadd("users", user.user_id)

        return user

    async def get_user(self, user_id: str) -> Optional[ChatUser]:
        """
        Retrieve a user from Redis by user_id.

        Args:
            user_id (str): The ID of the user to retrieve

        Returns:
            Optional[ChatUser]: The user if found, None otherwise
        """
        user_key = f"user:{user_id}"
        user_data = await self.redis.hgetall(user_key)

        if not user_data:
            return None

        # Convert Redis data back to ChatUser model
        # Use ast.literal_eval instead of eval for safety
        import ast
        user_data["metadata"] = ast.literal_eval(user_data["metadata"])  # Convert string back to dict
        return ChatUser(**user_data)

    async def delete_user(self, user_id: str) -> bool:
        """
        Delete a user from Redis.

        Args:
            user_id (str): The ID of the user to delete

        Returns:
            bool: True if user was deleted, False otherwise
        """
        user_key = f"user:{user_id}"

        # Remove from user index
        await self.redis.srem("users", user_id)

        # Delete user hash
        deleted = await self.redis.delete(user_key)
        return deleted > 0

    async def update_user(self, user: ChatUser) -> ChatUser:
        """
        Update an existing user in Redis.

        Args:
            user (ChatUser): The user to update

        Returns:
            ChatUser: The updated user
        """
        user.updated_at = datetime.now().isoformat()
        return await self.add_user(user)