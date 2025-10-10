from pydantic import Field, model_validator, computed_field
from typing import Optional, List

from agent_c.models.context.interaction_extra import BaseInteractionContextExtra
from agent_c.models.context.base import BaseContext
from agent_c_tools.tools.workspace.base import BaseWorkspace


class WorkspaceToolsContext(BaseContext):
    default_workspace: str = Field(default="desktop", description="The default workspace to use if no other is specified. ")
    allowed_workspaces: Optional[List[str]] = Field(default_factory=list, description="If provided, only these workspaces will be used in the interaction. "
                                                                                      "If not provided, any workspace not disallowed.")
    disallowed_workspaces: Optional[List[str]] = Field(default_factory=list, description="If provided, these workspaces will not be used in the interaction. "
                                                                                         "If not provided, all workspaces are allowed.")
    force_read_only: bool = Field(default=False, description="If True, all workspace operations will be read-only. ")

class WorkspaceToolsInteractionContext(BaseInteractionContextExtra):
    """
    This context is used to store information during an interaction by the workspace.
    """
    did_trigger_context_warning: bool = Field(default=False,
                                              description="If True, a context warning has been triggered by one or more tool calls ")
    workspace_list: Optional[List[BaseWorkspace]] = Field(None,
                                                          description="A list of workspaces that are available for the interaction. ")


    @model_validator(mode='after')
    def validate_workspaces(self):
        self.workspace_list = self.tool.workspace_list(self.interaction)

        return self

    @computed_field
    @property
    def workspace_names(self):
        """
        Returns a list of workspace names.
        """
        return [workspace.name for workspace in self.workspace_list] if self.workspace_list else []