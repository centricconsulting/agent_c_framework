import logging
from typing import Any, List

from agent_c_core.agent_c.toolsets.tool_set import Toolset
from agent_c_core.agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspaces.base import BaseWorkspace
from agent_c_tools.tools.workspaces.prompt import WorkspaceSection

logger = logging.getLogger(__name__)


class WorkspaceTools(Toolset):
    """
    WorkspaceTools allows the model to read / write data to one or more workspaces.
    This allows us to absract things like S3, Azure Storage and the like.

    This really just a rough outline at this point.
    """

    def __init__(self, **kwargs: Any) -> None:
        super().__init__(**kwargs, name="workspace")
        self.workspaces: List[BaseWorkspace] = kwargs.get('workspaces', [])
        self._create_section()

    def add_workspace(self, workspace: BaseWorkspace) -> None:
        """Add a workspace to the list of workspaces."""
        self.workspaces.append(workspace)
        self._create_section()

    def _create_section(self):
        spaces: str = "\n".join([str(space) for space in self.workspaces])
        self.section = WorkspaceSection(workspaces=spaces)

    def find_workspace_by_name(self, name):
        try:
            return next(workspace for workspace in self.workspaces if workspace.name == name)
        except StopIteration:
            # Handle the case where no workspaces with the given name is found
            logger.warning(f"No workspaces found with the name: {name}")
            return None


    @json_schema(
        'List the contents of a directory within a workspaces.',
        {
            'workspace': {
                'type': 'string',
                'description': 'The name of the workspace the path resides in. Refer to the "available workspaces" list for valid names.',
                'required': True
            },
            'path': {
                'type': 'string',
                'description': 'The path, relative to the workspace root folder, to list.',
                'required': False
            }
        }
    )
    async def ls(self, **kwargs: Any) -> str:
        """Asynchronously lists the contents of a workspaces or a subdirectory in it.

        Args:
            workspace (str): The workspaces to use
            path (str): Relative path within the workspaces to list contents for.

        Returns:
            str: JSON string with the listing or an error message.
        """
        relative_path: str = kwargs.get('path', '')
        workspace = self.find_workspace_by_name(kwargs.get('workspace'))
        if workspace is None:
            return f'No workspaces found with the name: {workspace}'

        return await workspace.ls(relative_path)




    @json_schema(
        'Reads the contents of a text file within the workspaces.',
        {
            'workspace': {
                'type': 'string',
                'description': 'The name of the workspace the file_path resides in. Refer to the "available workspaces" list for valid names.',
                'required': True
            },
            'file_path': {
                'type': 'string',
                'description': 'The path to the file, relative to the workspace root folder',
                'required': True
            }
        }
    )
    async def read(self, **kwargs: Any) -> str:
        """Asynchronously reads the content of a text file within the workspaces.

        Args:
            file_path (str): Relative path to the text file within the workspaces.

        Returns:
            str: JSON string with the file content or an error message.
        """
        file_path: str = kwargs['file_path']
        workspace = self.find_workspace_by_name(kwargs.get('workspace'))
        if workspace is None:
            return f'No workspaces found with the name: {workspace}'

        return await workspace.read(file_path)

    @json_schema(
        'Writes or appends text data to a file within the workspaces.',
        {
            'workspace': {
                'type': 'string',
                'description': 'The name of the workspace the file_path resides in. Refer to the "available workspaces" list for valid names.',
                'required': True
            },
            'file_path': {
                'type': 'string',
                'description': 'The path, relative to the workspace root folder, to the file within the workspace.',
                'required': True
            },
            'data': {
                'type': 'string',
                'description': 'The text data to write or append to the file.',
                'required': True
            },
            'mode': {
                'type': 'string',
                'description': 'The writing mode: "write" to overwrite or "append" to add to the file.',
                'required': False
            }
        }
    )
    async def write(self, **kwargs: Any) -> str:
        """Asynchronously writes or appends data to a file within the workspaces.

        Args:
            file_path (str): Relative path to the file within the workspaces.
            data (str): The text data to write or append to the file.
            mode (str): The writing mode, either 'write' to overwrite or 'append'.

        Returns:
            str: JSON string with a success message or an error message.
        """
        file_path: str = kwargs['file_path']
        data: str = kwargs['data']
        mode: str = kwargs.get('mode', 'write')

        workspace = self.find_workspace_by_name(kwargs.get('workspace'))
        if workspace is None:
            return f'No workspaces found with the name: {workspace}'

        return await workspace.write(file_path, mode, data)


Toolset.register(WorkspaceTools)