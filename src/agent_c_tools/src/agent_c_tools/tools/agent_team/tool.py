from typing import Any
from agent_c.toolsets.tool_set import Toolset
from agent_c_tools.tools.agent_assist.tool import AgentAssistTools

from .prompt import AgentTeamSection

class AgentTeamTools(AgentAssistTools):
    """Agent Team Tools for managing and interacting with a team of agents.
       This is essentially the AgentAssistTools with a more restrictive list
       in order available to an agent as a part of a team the ID of the agent
       must be in the categories of the agent configuration.
    """
    def __init__(self, **kwargs: Any):
        if not 'name' in kwargs:
            kwargs['name'] = 'ateam'

        super().__init__( **kwargs)
        self.section = AgentTeamSection(tool=self)

# Register the toolset
Toolset.register(AgentTeamTools, required_tools=['WorkspaceTools'])