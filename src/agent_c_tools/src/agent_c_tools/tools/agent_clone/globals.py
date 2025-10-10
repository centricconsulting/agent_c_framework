import datetime
import platform
import socket

from click import pass_context
from pydantic import Field, model_validator

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals
from agent_c.registration import locate_config_folder


class AgentCloneToolsGlobals(BasePromptGlobals):
    """
    Registers system / server related globals in the Jinja 2 environment.
    """
    add_as_object: bool = Field(False)

    @pass_context
    def clone_session_list(self, ctx):
        """
        Returns a list of active assistant sessions.
        """
        if 'AgentCloneTools' not in ctx.interaction.toolsets:
            return []

        return ctx.interaction.toolsets['AgentCloneTools'].list_clone_sessions(ctx.interaction)