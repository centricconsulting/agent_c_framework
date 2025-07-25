import threading
from pydantic import Field
from typing import List, AnyStr, Optional, Dict, Any, Callable, Awaitable

from agent_c.models import ChatSession
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
    interaction_id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3),
                                description="The ID of the interaction, used to identify the interaction in the system. "
                                            "Will be generated if not provided")
    chat_session: ChatSession = Field(..., description="The chat session that this interaction is part of. "
                                                              "This is used to group interactions together.")
    agent_config: AgentConfiguration = Field(..., description="The agent configuration to use for the interaction")
    agent_runtime: BaseAgent = Field(..., description="The agent runtime to use for the interaction")
    tool_chest: ToolChest = Field(..., description="The tool chest to use for the interaction")
    client_wants_cancel: threading.Event = Field(default_factory=threading.Event,
                                                 description="An event that is set when the client wants to cancel the interaction. T"
                                                             "his is used to signal the agent to stop processing.")

    streaming_callback: Callable[[BaseEvent], Awaitable[None]] = Field(
        default=lambda event: None,
        description="A callback function that is called when a streaming event occurs. This is used to handle streaming events from the agent."
    )

    sub_contexts: Dict[str, BaseContext] = Field(default_factory=dict, description="A dictionary of context models to provide data for tools / prompts."
                                                                                   "Used to pass additional data to tools and prompts during the interaction. "
                                                                                   "Key is the context model type, value is the context model.")

    sections: Optional[List[PromptSection]] = Field(default_factory=list, description="A list of prompt sections that are used in the interaction. "
                                                                                      "This is used to store the prompt sections that are used in the interaction.")
    tool_schemas: Optional[List[Dict[str, Any]]] = Field(default_factory=dict, description="A dictionary of tool schemas that are used in the interaction. "
                                                                                     "This is used to store the schemas of the tools that are used in the interaction.")
    user_session_id: Optional[str] = Field(None, description="The user session ID associated with the interaction. If this is a sub session "
                                                                    "This is used to identify the user session that this interaction belongs to.")

    parent_context: Optional['InteractionContext'] = Field(None, description="The parent context of this interaction. "
                                                                           "This is used to link interactions together in a hierarchy.")

    def __init__(self, **data) -> None:
        if 'interaction_id' not in data:
            data['interaction_id'] = f"{data['chat_session'].session_id}:{MnemonicSlugs.generate_slug(2)}"

        super().__init__(**data)
