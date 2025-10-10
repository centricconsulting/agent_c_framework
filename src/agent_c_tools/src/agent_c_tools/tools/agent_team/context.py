from typing import  Dict
from pydantic import Field

from agent_c.models.context.base import BaseContext
from agent_c_tools.tools.agent_assist.models import AgentAssistSession

class AgentTeamToolsContext(BaseContext):
    process_context: str = Field(None,
                                 description="The context of the process, used to determine the current state of the agent assist tools.")
    team_sessions: Dict[str, AgentAssistSession] = Field(default_factory=dict)


