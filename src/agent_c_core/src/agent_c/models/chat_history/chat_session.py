import datetime
from pydantic import Field
from typing import Optional, Dict, Any, List

from agent_c.models.base import BaseModel
from agent_c.util.slugs import MnemonicSlugs
from agent_c.models.config.agent_config import AgentConfiguration


class ChatSession(BaseModel):
    """
    Represents a session object with a unique identifier, metadata,
    and other attributes.
    """
    session_id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3))
    parent_session_id: Optional[str] = Field(None, description="The ID of the parent session, if any")
    user_session_id: Optional[str] = Field(None, description="The user session ID associated with the session")
    input_token_count: int = Field(0, description="The number of input tokens in the session")
    output_token_count: int = Field(0, description="The number of output tokens in the session")
    context_window_size: int = Field(0, description="The number of tokens in the context window")
    session_name: Optional[str] = Field(None, description="The name of the session, if any")
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    deleted_at: Optional[str] = Field(None, description="Timestamp when the session was deleted")
    user_id: Optional[str] = Field("Agent C user", description="The user ID associated with the session")
    metadata: Optional[Dict[str, Any]] = Field(default_factory=dict, description="Metadata associated with the session")
    messages: List[dict[str, Any]] = Field(default_factory=list, description="List of messages in the session")
    agent_config: Optional[AgentConfiguration] = Field(None, description="Configuration for the agent associated with the session")

    def __init__(self, **data):
        """
        Initializes the ChatSession with the provided data.

        Args:
            **data: Arbitrary keyword arguments to initialize the session.
        """
        super().__init__(**data)
        if self.user_session_id is None:
            self.user_session_id = self.session_id

    def touch(self) -> None:
        """
        Updates the updated_at timestamp to the current time.
        """
        self.updated_at = datetime.datetime.now().isoformat()
