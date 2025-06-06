import threading
from pydantic import Field
from typing import List, AnyStr, Optional, Dict, Any, Callable, Awaitable

from agent_c.toolsets.tool_chest import ToolChest, PromptSection
from agent_c.agents.base import BaseAgent
from agent_c.models.context.base import BaseContext
from agent_c.models.agent_config import AgentConfiguration
from agent_c.models.events import BaseEvent
from agent_c.util import MnemonicSlugs


class InteractionContext(BaseContext):
    """
    Represents the context of an interaction with an agent.
    This includes the agent configuration and the agent itself.
    """
    interaction_id: str = Field(..., description="The ID of the interaction, used to identify the interaction in the system. Will be generated if not provided")
    agent: AgentConfiguration = Field(..., description="The agent configuration to use for the interaction")
    agent_runtime: BaseAgent = Field(..., description="The agent runtime to use for the interaction")
    tool_chest: ToolChest = Field(..., description="The tool chest to use for the interaction")
    session_id: str = Field(..., description="The session ID the interaction belongs to.")
    user_session_id: str = Field(..., description="The user session ID the interaction belongs to.")
    prompt_context: List[BaseContext] = Field(default_factory=list, description="A list of context models for any prompt sections that are used in the interaction.")
    tool_context: List[BaseContext] = Field(default_factory=list, description="A list of context models for any tool that are used in the interaction.")
    messages: Optional[List[Dict[str, Any]]] = Field(default_factory=list, description="A list of messages that are part of the interaction. This is used to store the conversation history.")
    client_wants_cancel: threading.Event = Field(default_factory=threading.Event, description="An event that is set when the client wants to cancel the interaction. This is used to signal the agent to stop processing.")
    sections: Optional[List[PromptSection]] = Field(default_factory=list, description="A list of prompt sections that are used in the interaction. This is used to store the prompt sections that are used in the interaction.")
    tool_schemas: Optional[Dict[str, Any]] = Field(default_factory=dict, description="A dictionary of tool schemas that are used in the interaction. This is used to store the schemas of the tools that are used in the interaction.")
    streaming_callback: Callable[ [BaseEvent], Awaitable[None]] = Field(
        default=lambda event: None,
        description="A callback function that is called when a streaming event occurs. This is used to handle streaming events from the agent."
    )

    def __init__(self, **data) -> None:
        if 'interaction_id' not in data:
            data['interaction_id'] = f"{data['session_id']}:{MnemonicSlugs.generate_slug(2)}"

        super().__init__(**data)
