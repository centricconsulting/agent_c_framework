import datetime
import platform
import socket

from click import pass_context
from pydantic import Field, model_validator

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals
from agent_c.registration import locate_config_folder


class AgentAssistToolsGlobals(BasePromptGlobals):
    """
    Registers system / server related globals in the Jinja 2 environment.
    """
    add_as_object: bool = Field(False)

    @pass_context
    def assistant_session_list(self, ctx):
        """
        Returns a list of active assistant sessions.
        """
        if 'AgentAssistTools' not in ctx.interaction.toolsets:
            return []
        return ctx.interaction.toolsets['AgentAssistTools'].list_active_sessions(ctx.interaction)