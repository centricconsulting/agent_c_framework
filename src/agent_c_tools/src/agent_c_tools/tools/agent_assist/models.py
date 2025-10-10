from typing import Any

from pydantic import Field, computed_field
from agent_c.models import BaseModel


class AgentAssistSession(BaseModel):
    """
    Tracks key data about a chat session with an agent assistant.
    """
    chat_session: Any = Field(...,
                               description="The ChatSession associated with this agent assistant session")

    @computed_field
    @property
    def session_id(self) -> str:
        """
        Returns the session ID of the associated ChatSession.
        """
        return self.chat_session.session_id

    @computed_field
    @property
    def agent_name(self) -> str:
        """
        Returns the name of the agent assistant associated with this session.
        """
        return self.chat_session.agent_name

    @computed_field
    @property
    def context_window_remaining(self) -> int:
        """
        Returns the remaining context window size for the session.
        This is calculated as the maximum context window size minus the current context window size.
        """
        return self.chat_session.context_window_remaining

    @computed_field
    @property
    def context_window_used(self) -> int:
        """
        Returns the current context window size for the session.
        This is the total number of tokens used in the session.
        """
        return self.chat_session.context_window_size

    @computed_field
    @property
    def context_window_used_percentage(self) -> float:
        """
        Returns the percentage of the context window that has been used in this session.
        This is calculated as the used context window size divided by the maximum context window size.
        """
        return self.chat_session.context_window_used_percentage


    @computed_field
    @property
    def context_window_remaining_percentage(self) -> float:
        """
        Returns the percentage of the context window that is still available.
        This is calculated as the remaining context window size divided by the maximum context window size.
        """
        return self.chat_session.context_window_remaining_percentage

    @computed_field
    @property
    def message_count(self) -> int:
        """
        Returns the total number of messages in the session.
        This includes both user and agent messages.
        """
        return len(self.chat_session.messages)


    @computed_field
    @property
    def is_clone_session(self) -> bool:
        """
        Returns whether this session is a clone of another session.
        A clone session is a copy of an existing session, typically used for parallel processing or testing.
        """
        return self.chat_session.is_clone_session