from typing import List, Dict

from pydantic import Field, computed_field

from agent_c.models.context.base import BaseContext

from agent_c_tools.tools.agent_assist.models import AgentAssistSession


class AgentAssistToolsContext(BaseContext):
    process_context: str = Field(None,
                                 description="The context of the process, used to determine the current state of the agent assist tools.")
    agent_sessions: Dict[str, AgentAssistSession] = Field(default_factory=dict)

    @computed_field
    @property
    def available_assistants(self) -> List:
        """List of available assistants"""
        from agent_c.config.agent_config_loader import AgentConfigLoader
        return AgentConfigLoader.instance().find_by_category("agent_assist")
