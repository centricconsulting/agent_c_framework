import datetime
import platform
import socket

from click import pass_context
from pydantic import Field, model_validator

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals
from agent_c.registration import locate_config_folder


class AgentTeamToolsGlobals(BasePromptGlobals):
    """
    Registers system / server related globals in the Jinja 2 environment.
    """
    add_as_object: bool = Field(False)

    @pass_context
    def team_session_list(self, ctx):
        """
        Returns a list of active assistant sessions.
        """
        if 'AgentTeamTools' not in ctx.interaction.toolsets:
            return []

        return ctx.interaction.toolsets['AgentTeamTools'].list_team_sessions(ctx.interaction)

    @pass_context
    def agent_team_members(self, ctx):
        """
        Returns a list of team members in the current interaction.
        """
        if 'AgentTeamTools' not in ctx.interaction.toolsets:
            return []

        return ctx.chat_session.agent_config.team_members