from typing import Optional
from agent_c.models.chat.user import ChatUser
from ..repositories.user_repository import UserRepository


class UserService:
    def __init__(self, user_repository: UserRepository):
        self.user_repository = user_repository

    async def add_user(self, user: ChatUser) -> ChatUser:
        """
        Add a new user to the system.

        Args:
            user (ChatUser): The user to add

        Returns:
            ChatUser: The added user
        """
        # Add any business logic here (validation, etc.)
        return await self.user_repository.add_user(user)

    async def get_user(self, user_id: str) -> Optional[ChatUser]:
        """
        Retrieve a user by ID.

        Args:
            user_id (str): The ID of the user to retrieve

        Returns:
            Optional[ChatUser]: The user if found, None otherwise
        """
        return await self.user_repository.get_user(user_id)

    async def delete_user(self, user_id: str) -> bool:
        """
        Delete a user by ID.

        Args:
            user_id (str): The ID of the user to delete

        Returns:
            bool: True if user was deleted, False otherwise
        """
        return await self.user_repository.delete_user(user_id)

    async def update_user(self, user: ChatUser) -> ChatUser:
        """
        Update an existing user.

        Args:
            user (ChatUser): The user to update

        Returns:
            ChatUser: The updated user
        """
        return await self.user_repository.update_user(user)