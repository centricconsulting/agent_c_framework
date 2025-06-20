import datetime
import threading

from pydantic import Field
from typing import Optional

from agent_c.models import ChatSession
from agent_c.models.base import BaseModel
from agent_c_api.core.agent_bridge import AgentBridge
from agent_c.models.completion.agent_config import AgentConfiguration

class UserSession(BaseModel):
    chat_session: ChatSession = Field(..., description="The chat session associated with the user session.")
    agent_bridge: AgentBridge = Field(..., description="The agent bridge instance managing the chat session.")
    cancel_event: threading.Event = Field(None, description="Event to signal cancellation of the session.")
    created_at: str = Field(default_factory=lambda: datetime.datetime.now().isoformat(), description="The timestamp when the user session was created.")
    session_id: Optional[str] = Field(None, description="The unique identifier for the user session.")
    llm_model: Optional[str] = Field(None, description="The LLM model used for the agent.")
    agent_name: Optional[str] = Field(None, description="The name of the agent.")
    agent_c_session_id: Optional[str] = Field(None, description="The unique identifier for the agent C session.")
    agent_config: Optional[AgentConfiguration] = Field(None, description="Configuration for the agent associated with the session.")

    def __init__(self, **data):
        super().__init__(**data)
        if not self.cancel_event:
            self.cancel_event = threading.Event()

        if self.session_id is None:
            self.session_id = self.chat_session.session_id

        if self.agent_config is None:
            self.agent_config = self.chat_session.agent_config

        if self.llm_model is None:
            self.llm_model = self.agent_config.runtime_params.model_id

        if self.agent_name is None:
            self.agent_name = self.agent_config.name

        if self.agent_c_session_id is None:
            self.agent_c_session_id = self.chat_session.session_id
