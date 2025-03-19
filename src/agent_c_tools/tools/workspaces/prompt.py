from typing import Any

from agent_c_core.agent_c.prompting.prompt_section import PromptSection, property_bag_item


class WorkspaceSection(PromptSection):
    workspaces: str

    def __init__(self, **data: Any):
        TEMPLATE = ("The workspace toolsets provide you a way to work with files in one or more workspaces.\n"
                    "The list below contains the available workspaces by name, along with their read/write status, the type of workspace it is and a description of the workspace.\n"
                    "You this list to help you determine the name of the workspace to use with the workspace toolsets:\n"
                    "**important** If the user asks about the available workspaces use the info in this list.\n"
                    "### Available Workspaces\n"
                    "${workspace_list}")
        super().__init__(template=TEMPLATE, required=True, name="Workspaces", render_section_header=True,  **data)

    @property_bag_item
    async def workspace_list(self):
        return self.workspaces
