import time
from datetime import datetime
from typing import Optional

from agent_c.util.structured_logging import get_logger, LoggingContext
from agent_c.models.chat_history.user import ChatUser
from redis import asyncio as aioredis


class UserRepository:
    def __init__(self, redis_client: aioredis.Redis):
        self.redis = redis_client
        self.logger = get_logger(__name__)
        
        self.logger.info(
            "user_repository_initialized",
            redis_client_type=type(redis_client).__name__
        )

    async def add_user(self, user: ChatUser) -> ChatUser:
        """
        Add a new chat user to Redis.

        Args:
            user (ChatUser): The user to add

        Returns:
            ChatUser: The added user with any updated fields
        """
        with LoggingContext(operation="add_user", user_id=user.user_id):
            start_time = time.time()
            
            try:
                self.logger.info(
                    "user_creating",
                     username=getattr(user, 'username', 'unknown')
                )
                
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
                
                duration = time.time() - start_time
                self.logger.info(
                    "user_created",
                     username=getattr(user, 'username', 'unknown'),
                    duration_ms=round(duration * 1000, 2)
                )

                return user
                
            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "user_creation_failed",
                    username=getattr(user, 'username', 'unknown'),
                    error=str(e),
                    duration_ms=round(duration * 1000, 2)
                )
                raise

    async def get_user(self, user_id: str) -> Optional[ChatUser]:
        """
        Retrieve a user from Redis by user_id.

        Args:
            user_id (str): The ID of the user to retrieve

        Returns:
            Optional[ChatUser]: The user if found, None otherwise
        """
        with LoggingContext(operation="get_user", user_id=user_id):
            start_time = time.time()
            
            try:
                self.logger.info("user_retrieving")
                
                user_key = f"user:{user_id}"
                user_data = await self.redis.hgetall(user_key)

                if not user_data:
                    duration = time.time() - start_time
                    self.logger.info(
                        "user_not_found",
                        found=False,
                        duration_ms=round(duration * 1000, 2)
                    )
                    return None

                # Convert Redis data back to ChatUser model
                # Use ast.literal_eval instead of eval for safety
                import ast
                user_data["metadata"] = ast.literal_eval(user_data["metadata"])  # Convert string back to dict
                user = ChatUser(**user_data)
                
                duration = time.time() - start_time
                self.logger.info(
                    "user_retrieved",
                    found=True,
                    username=getattr(user, 'username', 'unknown'),
                    duration_ms=round(duration * 1000, 2)
                )
                
                return user
                
            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "user_retrieval_failed",
                    error=str(e),
                    duration_ms=round(duration * 1000, 2)
                )
                raise

    async def delete_user(self, user_id: str) -> bool:
        """
        Delete a user from Redis.

        Args:
            user_id (str): The ID of the user to delete

        Returns:
            bool: True if user was deleted, False otherwise
        """
        with LoggingContext(operation="delete_user", user_id=user_id):
            start_time = time.time()
            
            try:
                self.logger.info("user_deleting")
                
                user_key = f"user:{user_id}"

                # Remove from user index
                await self.redis.srem("users", user_id)

                # Delete user hash
                deleted = await self.redis.delete(user_key)
                success = deleted > 0
                
                duration = time.time() - start_time
                if success:
                    self.logger.info(
                        "user_deleted",
                        success=True,
                        duration_ms=round(duration * 1000, 2)
                    )
                else:
                    self.logger.warning(
                        "user_deletion_failed",
                        success=False,
                        reason="user_not_found_or_already_deleted",
                        duration_ms=round(duration * 1000, 2)
                    )
                
                return success
                
            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "user_deletion_error",
                    error=str(e),
                    duration_ms=round(duration * 1000, 2)
                )
                raise

    async def update_user(self, user: ChatUser) -> ChatUser:
        """
        Update an existing user in Redis.

        Args:
            user (ChatUser): The user to update

        Returns:
            ChatUser: The updated user
        """
        with LoggingContext(operation="update_user", user_id=user.user_id):
            start_time = time.time()
            
            try:
                self.logger.info(
                    "user_updating",
                    username=getattr(user, 'username', 'unknown')
                )
                
                user.updated_at = datetime.now().isoformat()
                updated_user = await self.add_user(user)
                
                duration = time.time() - start_time
                self.logger.info(
                    "user_updated",
                    username=getattr(user, 'username', 'unknown'),
                    duration_ms=round(duration * 1000, 2)
                )
                
                return updated_user
                
            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "user_update_failed",
                    username=getattr(user, 'username', 'unknown'),
                    error=str(e),
                    duration_ms=round(duration * 1000, 2)
                )
                raise