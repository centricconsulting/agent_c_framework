from typing import List, TYPE_CHECKING, Dict

from pydantic import Field, computed_field

from agent_c.models.context.base import BaseContext
from agent_c.config.agent_config_loader import AgentConfigLoader, AgentConfiguration
from agent_c_tools.tools.agent_assist.models import AgentAssistSession


class AgentCloneToolsContext(BaseContext):
    process_context: str = Field(None,
                                 description="The context of the process, used to determine the current state of the agent assist tools.")
    clone_sessions: Dict[str, AgentAssistSession] = Field(default_factory=dict)

