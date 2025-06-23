import datetime

from pydantic import Field
from typing import Optional, Dict, Any, List

from agent_c.util.slugs import MnemonicSlugs
from agent_c.models.base import BaseModel
from agent_c.models.context.context_bag import ContextBag
from agent_c.models.chat.user import ChatUser


class ChatSession(BaseModel):
    """
    Represents a session object with a unique identifier, metadata,
    and other attributes.
    """
    version: int = Field(1,
                         description="The version of the session model. This is used to track changes in the session model.")
    session_id: str = Field(...,
                            description="Unique identifier for the session",)
    parent_session_id: Optional[str] = Field(None,
                                             description="The ID of the parent session, if any")
    user_session_id: Optional[str] = Field(None,
                                           description="The user session ID associated with the session")
    input_token_count: int = Field(0,
                                   description="The number of input tokens in the session")
    output_token_count: int = Field(0,
                                    description="The number of output tokens in the session")
    context_window_size: int = Field(0,
                                     description="The number of tokens in the context window")
    session_name: Optional[str] = Field(None,
                                        description="The name of the session, if any")
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    deleted_at: Optional[str] = Field(None)
    user: ChatUser = Field(...,
                           description="The user associated with the session")
    metadata: Optional[Dict[str, Any]] = Field(default_factory=dict,
                                               description="Metadata associated with the session")
    messages: List[dict[str, Any]] = Field(default_factory=list,
                                           description="List of messages in the session")
    agent_config: Optional['AgentConfiguration'] = Field(None,
                                                         description="Configuration for the agent associated with the session")
    context: ContextBag = Field(default_factory=dict,
                                description="A dictionary of context models to provide data for tools / prompts.")

    def model_dump_serializable(self, exclude: Optional[List[str]] = None, **kwargs) -> Dict[str, Any]:
        """
        Dumps the model to a dictionary, excluding None values and certain fields.
        """
        if exclude is None:
            exclude = ['user']
        else:
            exclude.append('user')

        data = super().model_dump(exclude=set(exclude), **kwargs)
        data['user'] = self.user.user_id

    @staticmethod
    def __new_session_id(**data) -> str:
        session_id = data.get('session_id')
        if session_id:
            return session_id

        if isinstance(data['user'], str):
            from agent_c.chat import UserLoader
            data['user'] = UserLoader.instance().load_user_id(data['user'])

        if 'parent_session_id' in data:
            session_id = f"{data['parent_session_id']}-{MnemonicSlugs.generate_slug(2)}"
        elif 'user_session_id' in data:
            session_id = f"{data['user_session_id']}-{MnemonicSlugs.generate_slug(2)}"
        else:
            user_slug = MnemonicSlugs.generate_id_slug(2, data['user'].user_id)
            session_id = f"{user_slug}-{MnemonicSlugs.generate_slug(2)}"

        return session_id

    @property
    def parent_session(self) -> Optional['ChatSession']:
        """
        Returns the parent session if it exists, otherwise None.

        Returns:
            Optional[ChatSession]: The parent session or None if not set.
        """
        from agent_c.chat import DefaultChatSessionManager

        if self.parent_session_id is None:
            return None

        return DefaultChatSessionManager.instance().get_parent_session_for(self)

    @property
    def is_user_session(self) -> bool:
        """
        Checks if the session is a user session.

        Returns:
            bool: True if this session is a user session, False otherwise.
        """
        return self.user_session_id == self.session_id

    @property
    def is_sub_session(self) -> bool:
        """
        Checks if the session is a sub-session of another session.

        Returns:
            bool: True if this session is a sub-session, False otherwise.
        """
        return self.parent_session_id is not None

    @property
    def user_session(self) -> 'ChatSession':
        """
        Returns the user session associated with this session.
        If the user session ID is not set, it defaults to the session ID.

        Returns:
            ChatSession: The user session associated with this session.
        """
        if self.user_session_id == self.session_id:
            return self
        else:
            from agent_c.chat import DefaultChatSessionManager

            if self.user_session_id is None:
                self.user_session_id = self.session_id

            return DefaultChatSessionManager.instance().get_user_session_for(self)

    def new_sub_session_id(self) -> str:
        """
        Generates a new session ID for a sub-session based on the parent session.

        Args:
            parent_session (ChatSession): The parent session to base the new ID on.

        Returns:
            str: A new session ID for the sub-session.
        """
        return f"{self.session_id}-{MnemonicSlugs.generate_slug(2)}"

    def __init__(self, **data):
        """
        Initializes the ChatSession with the provided data.

        Args:
            **data: Arbitrary keyword arguments to initialize the session.
        """
        if 'session_id' not in data:
            data['session_id'] = self.__new_session_id(**data)

        super().__init__(**data)
        if self.user_session_id is None:
            self.user_session_id = self.session_id

    def touch(self) -> None:
        """
        Updates the updated_at timestamp to the current time.
        """
        self.updated_at = datetime.datetime.now().isoformat()
