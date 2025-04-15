import json
import logging
import re
from typing import Any, List, Tuple, Optional
from ts_tool import api

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspace.base import BaseWorkspace
from agent_c_tools.tools.workspace.prompt import WorkspaceSection
from agent_c_tools.tools.workspace.util import ReplaceStringsHelper

class WorkspaceTools(Toolset):
    """
    WorkspaceTools allows the model to read / write data to one or more workspaces.
    This allows us to abstract things like S3, Azure Storage and the like.

    Uses UNC-style paths (//WORKSPACE/path) to reference files and directories.
    """

    UNC_PATH_PATTERN = r'^//([^/]+)(?:/(.*))?$'

    def __init__(self, **kwargs: Any) -> None:
        super().__init__(**kwargs, name="workspace")
        self.workspaces: List[BaseWorkspace] = kwargs.get('workspaces', [])
        self._create_section()
        self.logger = logging.getLogger(__name__)
        self.replace_helper = ReplaceStringsHelper()

    def add_workspace(self, workspace: BaseWorkspace) -> None:
        """Add a workspace to the list of workspaces."""
        self.workspaces.append(workspace)
        self._create_section()

    def _create_section(self):
        spaces: str = "\n".join([str(space) for space in self.workspaces])
        self.section = WorkspaceSection(workspaces=spaces)

    def find_workspace_by_name(self, name):
        """Find a workspace by its name."""
        try:
            return next(workspace for workspace in self.workspaces if workspace.name == name)
        except StopIteration:
            # Handle the case where no workspace with the given name is found
            self.logger.warning(f"No workspace found with the name: {name}")
            return None

    def _parse_unc_path(self, path: str) -> Tuple[Optional[str], Optional[BaseWorkspace], Optional[str]]:
        """
        Parse a UNC path (//WORKSPACE/path) into workspace name and relative path.

        Args:
            path (str): The UNC path to parse

        Returns:
            Tuple[Optional[str], Optional[str], Optional[str]]:
                - Error message (if any)
                - Workspace name (if no error)
                - Relative path (if no error)
        """
        if not path:
            return "Path cannot be empty", None, None

        match = re.match(self.UNC_PATH_PATTERN, path)
        if not match:
            return f"Invalid UNC path format: {path}. Expected format: //WORKSPACE/path", None, None

        workspace_name = match.group(1)
        relative_path = match.group(2) or ''

        workspace = self.find_workspace_by_name(workspace_name)
        if workspace is None:
            return f"No workspace found with the name: {workspace_name}", None, None

        return None, workspace, relative_path

    def _validate_and_get_workspace_path(self, unc_path: str) -> Tuple[Optional[str], Optional[BaseWorkspace], Optional[str]]:
        """
        Validate a UNC path and return the workspace object and relative path.

        Args:
            unc_path (str): UNC-style path to validate

        Returns:
            Tuple[Optional[str], Optional[BaseWorkspace], Optional[str]]:
                - Error message (if any)
                - Workspace object (if no error)
                - Relative path (if no error)
        """
        error, workspace, relative_path = self._parse_unc_path(unc_path)
        if error:
            self.logger.error(error)
            return error, None, None

        return None, workspace, relative_path

    @json_schema(
        'List the contents of a directory using UNC-style path (//WORKSPACE/path)',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to list contents for',
                'required': True
            }
        }
    )
    async def ls(self, **kwargs: Any) -> str:
        """Asynchronously lists the contents of a workspace directory.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to list contents for

        Returns:
            str: JSON string with the listing or an error message.
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        return await workspace.ls(relative_path)

    @json_schema(
        'Retrieve a string "tree" of the directory structure using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to start the tree from',
                'required': True
            },
            'folder_depth': {
                'type': 'integer',
                'description': 'Depth of folders to include in the tree',
                'required': False
            },
            'file_depth': {
                'type': 'integer',
                'description': 'Depth of files to include in the tree',
                'required': False
            }
        }
    )
    async def tree(self, **kwargs: Any) -> str:
        """Asynchronously generates a tree view of a directory.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to start the tree from

        Returns:
            str: JSON string with the tree view or an error message.
        """
        unc_path = kwargs.get('path', '')
        folder_depth = kwargs.get('folder_depth', 5)
        file_depth = kwargs.get('file_depth', 3)

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        return await workspace.tree(relative_path, folder_depth, file_depth)

    @json_schema(
        'Reads the contents of a text file using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to read',
                'required': True
            }
        }
    )
    async def read(self, **kwargs: Any) -> str:
        """Asynchronously reads the content of a text file.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read

        Returns:
            str: JSON string with the file content or an error message.
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        return await workspace.read(relative_path)

    @json_schema(
        'Writes or appends text data to a file using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to write',
                'required': True
            },
            'data': {
                'type': 'string',
                'description': 'The text data to write or append to the file',
                'required': True
            },
            'mode': {
                'type': 'string',
                'description': 'The writing mode: "write" to overwrite or "append" to add to the file',
                'required': False
            }
        }
    )
    async def write(self, **kwargs: Any) -> str:
        """Asynchronously writes or appends data to a file.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to write
            data (str): The text data to write or append to the file
            mode (str): The writing mode, either 'write' to overwrite or 'append'

        Returns:
            str: JSON string with a success message or an error message.
        """
        unc_path = kwargs.get('path', '')
        data = kwargs['data']
        mode = kwargs.get('mode', 'write')

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        return await workspace.write(relative_path, mode, data)

    @json_schema(
        'Copy a file or directory using UNC-style paths',
        {
            'src_path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the source',
                'required': True
            },
            'dest_path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the destination',
                'required': True
            }
        }
    )
    async def cp(self, **kwargs: Any) -> str:
        """Asynchronously copies a file or directory.

        Args:
            src_path (str): UNC-style path (//WORKSPACE/path) to the source
            dest_path (str): UNC-style path (//WORKSPACE/path) to the destination

        Returns:
            str: JSON string with the result or an error message.
        """
        src_unc_path = kwargs.get('src_path', '')
        dest_unc_path = kwargs.get('dest_path', '')

        # Validate source path
        src_error, src_workspace, src_relative_path = self._validate_and_get_workspace_path(src_unc_path)
        if src_error:
            return json.dumps({'error': src_error})

        # Validate destination path
        dest_error, dest_workspace, dest_relative_path = self._validate_and_get_workspace_path(dest_unc_path)
        if dest_error:
            return json.dumps({'error': dest_error})

        # Check if both paths are in the same workspace
        if src_workspace != dest_workspace:
            error_msg = f"Cross-workspace operations are not supported. Source and destination must be in the same workspace."
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        return await src_workspace.cp(src_relative_path, dest_relative_path)

    @json_schema(
        'Check if a path is a directory using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to check',
                'required': True
            }
        }
    )
    async def is_directory(self, **kwargs: Any) -> str:
        """Asynchronously checks if a path is a directory.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to check

        Returns:
            str: JSON string with the result or an error message.
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        try:
            result = await workspace.is_directory(relative_path)
            return json.dumps({'is_directory': result})
        except Exception as e:
            error_msg = f'Error checking if path is a directory: {e}'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})
    
    @json_schema(
        'Move a file or directory using UNC-style paths',
        {
            'src_path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the source',
                'required': True
            },
            'dest_path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the destination',
                'required': True
            }
        }
    )
    async def mv(self, **kwargs: Any) -> str:
        """Asynchronously moves a file or directory.

        Args:
            src_path (str): UNC-style path (//WORKSPACE/path) to the source
            dest_path (str): UNC-style path (//WORKSPACE/path) to the destination

        Returns:
            str: JSON string with the result or an error message.
        """
        src_unc_path = kwargs.get('src_path', '')
        dest_unc_path = kwargs.get('dest_path', '')

        # Validate source path
        src_error, src_workspace, src_relative_path = self._validate_and_get_workspace_path(src_unc_path)
        if src_error:
            return json.dumps({'error': src_error})

        # Validate destination path
        dest_error, dest_workspace, dest_relative_path = self._validate_and_get_workspace_path(dest_unc_path)
        if dest_error:
            return json.dumps({'error': dest_error})

        # Check if both paths are in the same workspace
        if src_workspace != dest_workspace:
            error_msg = f"Cross-workspace operations are not supported. Source and destination must be in the same workspace."
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        return await src_workspace.mv(src_relative_path, dest_relative_path)

    @json_schema(
        'Using a UNC-style path, update a text file with multiple string replacements. ',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to update',
                'required': True
            },
            'updates': {
                'type': 'array',
                'description': 'Array of update operations to perform',
                'items': {
                    'type': 'object',
                    'properties': {
                        'old_string': {
                            'type': 'string',
                            'description': 'The exact string to be replaced. This can be a multiline string. UTF8 encoding.'
                        },
                        'new_string': {
                            'type': 'string',
                            'description': 'The new string that will replace the old string. This can be a multiline string. UTF8 encoding'
                        }
                    },
                    'required': ['old_string', 'new_string']
                },
                'required': True
            }
        }
    )
    async def replace_strings(self, **kwargs: Any) -> str:
        """
        Asynchronously updates a file with multiple string replacements

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to update
            updates (list): A list of update operations, each containing 'old_string' and 'new_string'

        Returns:
            str: JSON string with a success message or an error message.
        """
        updates = kwargs['updates']

        unc_path = kwargs.get('path', '')
        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        try:
            result = await self.replace_helper.process_replace_strings(
                read_function=workspace.read, write_function=workspace.write,
                path=relative_path, updates=updates)

            return json.dumps(result)
        except Exception as e:
            error_msg = f'Error in replace_strings operation: {str(e)}'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

    @json_schema(
        'Read a subset of lines from a text file using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to read',
                'required': True
            },
            'start_line': {
                'type': 'integer',
                'description': 'The 0-based index of the first line to read',
                'required': True
            },
            'end_line': {
                'type': 'integer',
                'description': 'The 0-based index of the last line to read (inclusive)',
                'required': True
            },
            'include_line_numbers': {
                'type': 'boolean',
                'description': 'Whether to include line numbers in the output',
                'required': False
            }
        }
    )
    async def read_lines(self, **kwargs: Any) -> str:
        """Asynchronously reads a subset of lines from a text file.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read
            start_line (int): The 0-based index of the first line to read
            end_line (int): The 0-based index of the last line to read (inclusive)
            include_line_numbers (bool, optional): If True, includes line numbers in the output

        Returns:
            str: JSON string containing the requested lines or an error message.
        """
        unc_path = kwargs.get('path', '')
        start_line = kwargs.get('start_line')
        end_line = kwargs.get('end_line')
        include_line_numbers = kwargs.get('include_line_numbers', False)

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        try:
            # Validate line indices
            if not isinstance(start_line, int) or start_line < 0:
                return json.dumps({'error': 'Invalid start_line value'})
            if not isinstance(end_line, int) or end_line < start_line:
                return json.dumps({'error': 'Invalid end_line value'})

            file_content_response = await workspace.read(relative_path)

            # Parse the response to get the actual content
            try:
                file_content_json = json.loads(file_content_response)
                if 'error' in file_content_json:
                    return file_content_response  # Return the error from read operation
                file_content = file_content_json.get('contents', '')
            except json.JSONDecodeError:
                file_content = file_content_response

            # Split the content into lines
            lines = file_content.splitlines()

            # Check if end_line is beyond the file length
            if end_line >= len(lines):
                return json.dumps({
                    'error': f'End line {end_line} exceeds file length {len(lines)}'
                })

            # Extract the requested subset of lines
            subset_lines = lines[start_line:end_line + 1]

            if include_line_numbers:
                # Format lines with line numbers
                formatted_lines = [f"{start_line + i}: {line}" for i, line in enumerate(subset_lines)]
                subset_content = '\n'.join(formatted_lines)
            else:
                subset_content = '\n'.join(subset_lines)

            return subset_content

        except Exception as e:
            error_msg = f'Error reading file lines: {str(e)}'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

    @json_schema(
        'Inspects a code file and returns details about its contents and usage.',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to read',
                'required': True
            }
        }
    )
    async def inspect_code(self, **kwargs: Any) -> str:
        """Uses CodeExplorer to prepare code overviews.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read

        Returns:
            str: A markdown overview of the code
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self._validate_and_get_workspace_path(unc_path)
        if error:
            return json.dumps({'error': error})

        try:
            file_content_response = await workspace.read(relative_path)
            # Parse the response to get the actual content

            file_content_json = json.loads(file_content_response)
            if 'error' in file_content_json:
                return file_content_response  # Return the error from read operation
            file_content = file_content_json.get('contents', '')
        except  Exception as e:
            error_msg = f'Error fetching {unc_path}: {str(e)}'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})
        try:
            context = api.get_code_context(file_content, format='markdown', filename=unc_path)
        except Exception as e:
            error_msg = f'Error inspecting code {unc_path}: {str(e)}'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        return context

Toolset.register(WorkspaceTools)