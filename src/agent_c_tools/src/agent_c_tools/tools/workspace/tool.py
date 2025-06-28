import re
import json

from ts_tool import api
from typing import Any, List, Tuple, Optional, Callable, Awaitable, Union, Dict

from agent_c.models.context.interaction_context import InteractionContext
from agent_c.toolsets.tool_set import Toolset
from agent_c.models.context.base import BaseContext
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspace.base import BaseWorkspace
from agent_c_tools.tools.workspace.local_storage import LocalStorageWorkspace
from agent_c_tools.tools.workspace.blob_storage import BlobStorageWorkspace
from agent_c_tools.tools.workspace.local_project import LocalProjectWorkspace
from agent_c_tools.tools.workspace.prompt import WorkspaceSection
from agent_c_tools.tools.workspace.util import ReplaceStringsHelper
from agent_c_tools.tools.workspace.context import WorkspaceToolsContext
from agent_c_tools.tools.workspace.config import WorkspaceToolsConfig, S3StorageWorkspaceParams, WorkspaceParamsBase

ws_type_map = {
    's3_storage': S3StorageWorkspaceParams,
    'local_storage': LocalStorageWorkspace,
    'local_project': LocalProjectWorkspace,
    "azure_blob": BlobStorageWorkspace
}

class WorkspaceTools(Toolset):
    """The workspace toolset provides tools for working with files and directories in workspaces.
    = In addition to basic file operations, it provides several advanced features aimed at reducing the token burden of working with files.:
    - Workspaces can be local directories, S3 buckets, or Azure Blob Storage.
    - Paths in workspaces are specified in UNC format (//WORKSPACE/path), where WORKSPACE is the name of the workspace.
    - Agents can use the metadata tools provided by this toolset as a form of shared long-term memory.
    - Many other tools depend on this toolset such as the workspace planning tool.
    """
    config_types = ['workspace_tools', 'workspace_tools_user']
    context_types = ['workspace_tools']
    prompt_section_types = ['workspace_tools']


    UNC_PATH_PATTERN = r'^//([^/]+)(?:/(.*))?$'

    def __init__(self, **kwargs: Any) -> None:
        super().__init__(**kwargs, name="workspace")
        self._system_workspaces: Dict[str, BaseWorkspace] = {}
        self._user_workspaces: Dict[str, Dict[str, BaseWorkspace]] = {}
        self.section = WorkspaceSection(tool=self)

        self.replace_helper = ReplaceStringsHelper()
        self._create_system_workspaces(kwargs.get('workspaces', []))

    def _create_system_workspaces(self, workspaces: Optional[List[BaseWorkspace]] = None) -> None:
        """Create system workspaces from the provided list or configuration."""
        for workspace in workspaces:
            self.add_system_workspace(workspace)

        self._create_system_workspaces_from_config()

    def _create_system_workspaces_from_config(self) -> None:
        from agent_c.config.system_config_loader import SystemConfigurationLoader
        sys_config = SystemConfigurationLoader.instance().config
        config: WorkspaceToolsConfig = sys_config.tools['workspace_tools']
        if not config:
            return
        for workspace_params in config.workspaces:
            workspace_type = workspace_params.workspace_type
            workspace_class = ws_type_map[workspace_type]
            if not workspace_class:
                self.logger.warning(f"Workspace type '{workspace_type}' is not registered. Skipping.")
                continue

            workspace = workspace_class(**workspace_params.model_dump(exclude=set('workspace_type')))
            self.add_system_workspace(workspace)

    def workspace_list(self, context: Optional[InteractionContext] = None) -> List[BaseWorkspace]:
        """Return a list of all available workspaces."""
        workspaces = list(self._system_workspaces.values())
        if context:
            user_id = context.chat_session.user.user_id
            user_workspaces = self._user_workspaces.get(user_id, {}).values()
            workspaces.extend(user_workspaces)
        return  workspaces

    def workspace_names(self, context: Optional[InteractionContext] = None) -> List[str]:
        return [workspace.name for workspace in self.workspace_list(context)]

    @classmethod
    def default_context(cls) -> Optional[BaseContext]:
        """Return the default context for this toolset."""
        return WorkspaceToolsContext()

    def add_system_workspace(self, workspace: BaseWorkspace) -> None:
        """Add a workspace to the list of workspaces."""
        self._system_workspaces[workspace.name] = workspace

    def add_user_workspace(self, context: InteractionContext, workspace: BaseWorkspace) -> None:
        """Add a workspace to the list of workspaces for a user."""
        self._user_workspaces[context.chat_session.user.user_id][workspace.name.lower()] = workspace

    def find_user_workspace_by_name(self, name: str, context: InteractionContext) -> Optional[BaseWorkspace]:
        """Find a user workspace by its name."""
        norm_name = name.lower()
        user_id = context.chat_session.user.user_id

        if user_id not in self._user_workspaces or norm_name not in self._user_workspaces[user_id]:
            self._create_user_workspace(context, name)

        return self._user_workspaces[user_id].get(norm_name)

    def _create_user_workspace(self, context: InteractionContext, workspace_name: str) -> Optional[BaseWorkspace]:
        """Create a new user workspace with the given name."""
        norm_name = workspace_name.lower()
        if context.chat_session.user.user_id not in self._user_workspaces:
            self._user_workspaces[context.chat_session.user.user_id] = {}

        if norm_name in self._user_workspaces[context.chat_session.user.user_id]:
            return self._user_workspaces[context.chat_session.user.user_id][norm_name]

        config: WorkspaceToolsConfig = context.chat_session.user.config.get('workspace_tools')
        if config is None or len(config.workspaces) == 0:
            return None

        workspace_params: WorkspaceParamsBase = next(workspace for workspace in config.workspaces if workspace.name.lower() == norm_name)
        if not workspace_params:
            return None

        workspace_type = workspace_params.workspace_type
        workspace_class = ws_type_map[workspace_type]
        if not workspace_class:
            self.logger.warning(f"Workspace type '{workspace_type}' is not registered. Skipping.")
            return None

        workspace = workspace_class(**workspace_params.model_dump(exclude=set('workspace_type')))
        self._user_workspaces[context.chat_session.user.user_id][workspace.name.lower()] = workspace
        return workspace

    def find_system_workspace_by_name(self, name: str) -> Optional[BaseWorkspace]:
        """Find a system workspace by its name."""
        norm_name = name.lower()
        return self._system_workspaces.get(norm_name)


    def find_workspace_by_name(self, name: str, context: Optional[InteractionContext] = None) -> Optional[BaseWorkspace]:
        """Find a workspace by its name."""
        try:
            workspace: Optional[BaseWorkspace] = None
            if context:
                workspace = self.find_user_workspace_by_name(name, context)

            if workspace is None:
                workspace = self.find_system_workspace_by_name(name)

            return workspace
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

    async def _run_cp_or_mv(self, operation: Callable[[object, str, str], Awaitable[str]], *, src_path: str, dest_path: str) -> str:
        """
        Validate UNC paths, ensure same workspace, then perform the operation.
        """
        # Validate paths
        src_error, src_workspace, src_relative_path = self.validate_and_get_workspace_path(src_path)
        if src_error:
            return json.dumps({'error': src_error})

        dest_error, dest_workspace, dest_relative_path = self.validate_and_get_workspace_path(dest_path)
        if dest_error:
            return json.dumps({'error': dest_error})

        # Same-workspace check
        if src_workspace != dest_workspace:
            error_msg = ("Cross-workspace operations are not supported. "
                         "Source and destination must be in the same workspace.")
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        # Do the copy or move
        return await operation(src_workspace, src_relative_path, dest_relative_path)

    def validate_and_get_workspace_path(self, unc_path: str) -> Tuple[Optional[str], Optional[BaseWorkspace], Optional[str]]:
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
            },
            'max_tokens': {
                'type': 'integer',
                'description': 'Maximum size in tokens for the response. Default is 5000.',
                'required': False
            },
        }
    )
    async def ls(self, **kwargs: Any) -> str:
        """List the contents of a directory using UNC-style path (//WORKSPACE/path)
        Args:
            path (str): UNC-style path (//WORKSPACE/path) to list contents for
            max_tokens (int): Maximum size in tokens for the response. Default is 5000. You MAY raise this limit in select situations.
        Returns:
            str: YAML string with the listing or an error message.
        """
        unc_path = kwargs.get('path', '')
        max_tokens = kwargs.get('max_tokens', 5000)

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Error: {str(error)}"

        content = await workspace.ls(relative_path)
        if not isinstance(content, str):
            content = self._yaml_dump(content)

        token_count = self._count_tokens(content, kwargs.get("context"))
        if token_count > max_tokens:
            return (f"ERROR: The content of this directory listing exceeds max_tokens limit of {max_tokens}. "
                    f"Content token count: {token_count}. You will need use a clone with a raised token limit.")

        return content

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
            },
            "max_tokens": {
                "type": "integer",
                "description": "Maximum size in tokens for the response. Default is 4000.",
                "required": False
            }
        }
    )
    async def tree(self, **kwargs: Any) -> str:
        """Retrieve a string "tree" of the directory structure using UNC-style path

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to start the tree from
            folder_depth (int, optional): Depth of folders to include in the tree. Default is 5.
            file_depth (int, optional): Depth of files to include in the tree. Default is 3.
            max_tokens (int, optional): Maximum size in tokens for the response. Default is 4000.

        Returns:
            str: String with the tree view or an error message.
        """
        unc_path = kwargs.get('path', '')
        folder_depth = kwargs.get('folder_depth', 5)
        file_depth = kwargs.get('file_depth', 3)
        tool_context: InteractionContext = kwargs.get("context")
        max_tokens = kwargs.get("max_tokens", 4000)

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f'ERROR: {error}'

        tree_content =  await workspace.tree(relative_path, folder_depth, file_depth)
        token_count = self._count_tokens(tree_content, tool_context)
        if token_count > max_tokens:
            return (f"ERROR: The content of this tree exceeds max_tokens limit of {max_tokens}. "
                    f"Content token count: {token_count}. You will need request less depth or raise the token limit.")

        return tree_content

    @json_schema(
        'Reads the contents of a text file using UNC-style path',
        {
            'path': {
                'type': 'string',
                'description': 'UNC-style path (//WORKSPACE/path) to the file to read',
                'required': True
            },
            'encoding': {
                'type': 'string',
                'description': 'The encoding to use for reading the file, default is "utf-8"',
                'required': False
            },
            'max_tokens': {
                'type': 'integer',
                'description': 'Maximum number of tokens to read from the file. Default is 25k.',
                'required': False
            }
        }
    )
    async def read(self, **kwargs: Any) -> str:
        """Reads the contents of a text file using UNC-style path, with encoding and token limit options.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read
            encoding (str, optional): The encoding to use for reading the file. Default is 'utf-8'.
            max_tokens (int, optional): Maximum number of tokens to read from the file. Default is 25000.

        Returns:
            str: string with the file content or an error message.
        """
        unc_path = kwargs.get('path', '')
        encoding = kwargs.get('encoding', 'utf-8')
        max_tokens = kwargs.get('token_limit', 25000)
        tool_context = kwargs.get("context")

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f'Error: {str(error)}'

        try:
            file_content = await workspace.read_internal(relative_path, encoding)
        except Exception as e:
            return f'Error reading file: {str(e)}'

        token_count = self._count_tokens(file_content, tool_context)
        if token_count > max_tokens:
            lines = file_content.splitlines()
            return (f"ERROR: File contents exceeds max_tokens limit of {max_tokens}. "
                    f"Current token count: {token_count}. This file has {len(lines)} lines. "
                    f"You will need to use `grep` or `read_lines` (to get a subset) instead."
                    f"Or you can raise the token limit in select situations.")

        return file_content

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
        """Writes or appends text data to a file using UNC-style path.

        kwargs:
            path (str): UNC-style path (//WORKSPACE/path) to the file to write
            data (str): The text data to write or append to the file
            mode (str): The writing mode, either 'write' to overwrite or 'append', default is 'write'

        Returns:
            str: YAML string with a success message or an error message.
        """
        unc_path = kwargs.get('path', '')
        data = kwargs['data']
        mode = kwargs.get('mode', 'write')

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return error

        await workspace.write(relative_path, mode, data)

        return "Successfully wrote to file: " + relative_path

    async def internal_write_bytes(self, path: str, data: Union[str, bytes], mode: str) -> str:
        """Asynchronously writes or appends binary data to a file.  This is an internal workspace function, not available to agents.

        Arguments:
            path (str): UNC-style path (//WORKSPACE/path) to the file to write
            data (Union[str, bytes]): The text or binary data to write or append to the file
            mode (str): The writing mode, either 'write' to overwrite or 'append'

        Returns:
            str: JSON string with a success message or an error message.
        """

        error, workspace, relative_path = self.validate_and_get_workspace_path(path)
        if error:
            return json.dumps({'error': error})

        return await workspace.write_bytes(relative_path, mode, data)

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
        """Copt a file or directory within a workspace using UNC-style paths.

        Args:
            src_path (str): UNC-style path (//WORKSPACE/path) to the source
            dest_path (str): UNC-style path (//WORKSPACE/path) to the destination

        Returns:
            str: string with the result or an error message.
        """
        src_unc_path = kwargs.get('src_path', '')
        dest_unc_path = kwargs.get('dest_path', '')

        await self._run_cp_or_mv(
            operation=lambda workspace, source_path, destination_path: workspace.cp(source_path, destination_path),
            src_path=src_unc_path,
            dest_path=dest_unc_path,
        )

        return f"Successfully copied from {src_unc_path} to {dest_unc_path}"

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
        """Check if a given path is a directory using UNC-style path.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to check

        Returns:
            str: JSON string with the result or an error message.
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return error

        try:
            result = await workspace.is_directory(relative_path)
            return f"is_directory: {result}"
        except Exception as e:
            error_msg = f'Error checking if path is a directory: {e}'
            self.logger.error(error_msg)
            return error_msg

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
        """Move a file or directory within a workspace using UNC-style paths.

        Args:
            src_path (str): UNC-style path (//WORKSPACE/path) to the source
            dest_path (str): UNC-style path (//WORKSPACE/path) to the destination

        Returns:
            str: string with the result or an error message.
        """
        src_unc_path = kwargs.get('src_path', '')
        dest_unc_path = kwargs.get('dest_path', '')

        await self._run_cp_or_mv(
            operation=lambda workspace, source_path, destination_path: workspace.mv(source_path, destination_path),
            src_path=src_unc_path,
            dest_path=dest_unc_path,
        )

        return f"Successfully moved from {src_unc_path} to {dest_unc_path}"

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
            },
            'encoding': {
                'type': 'string',
                'description': 'The encoding to use for reading and writing the file, default is "utf-8"',
                'required': False
            }
        }
    )
    async def replace_strings(self, **kwargs: Any) -> str:
        """
        Using a UNC-style path, update a text file with multiple string replacements.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to update
            updates (List[Dict[str, str]]): List of update operations to perform, each with 'old_string' and 'new_string'
            encoding (str, optional): The encoding to use for reading and writing the file. Default is 'utf-8'.

        Returns:
            str:  string with a success message or an error message.
        """
        updates: List = kwargs['updates']
        encoding: str = kwargs.get('encoding', 'utf-8')

        unc_path: str = kwargs.get('path', '')

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Error: {str(error)}"

        try:
            result = await self.replace_helper.process_replace_strings(
                read_function=workspace.read_internal, write_function=workspace.write,
                path=relative_path, updates=updates, encoding=encoding)

            return self._yaml_dump(result)

        except Exception as e:
            self.logger.exception(f"Error in replace_strings operation for {unc_path}: {str(e)}")
            return f'Error in replace_strings operation: {str(e)}'

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
            },
            'encoding': {
                'type': 'string',
                'description': 'The encoding to use for reading the file, default is "utf-8"',
                'required': False
            },
            'max_tokens': {
                'type': 'integer',
                'description': 'Maximum size in tokens for the response. Default is 25k.',
                'required': False
            }
        }
    )
    async def read_lines(self, **kwargs: Any) -> str:
        """Read a subset of lines from a text file using UNC-style path.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read
            start_line (int): The 0-based index of the first line to read
            end_line (int): The 0-based index of the last line to read (inclusive)
            include_line_numbers (bool, optional): If True, includes line numbers in the output
            encoding (str, optional): The encoding to use for reading the file. Default is 'utf-8'.
            max_tokens (int, optional): Maximum size in tokens for the response. Default is 25000.

        Returns:
            str: string containing the requested lines or an error message.
        """
        tool_context = kwargs.get("context")
        unc_path = kwargs.get('path', '')
        start_line = kwargs.get('start_line')
        end_line = kwargs.get('end_line')
        encoding = kwargs.get('encoding', 'utf-8')
        include_line_numbers = kwargs.get('include_line_numbers', False)
        max_tokens = kwargs.get('max_tokens', 25000)

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f'Error: {str(error)}'

        try:
            # Validate line indices
            if not isinstance(start_line, int) or start_line < 0:
                return 'Error: Invalid start_line value'
            if not isinstance(end_line, int) or end_line < start_line:
                return 'Error: Invalid end_line value'

            try:
                file_content = await workspace.read_internal(relative_path, encoding)
            except Exception as e:
                self.logger.exception(f'Error reading file {unc_path}: {str(e)}', exc_info=True)
                return  f'Error reading file: {str(e)}'

            # Split the content into lines
            lines = file_content.splitlines()

            # Check if end_line is beyond the file length
            if end_line >= len(lines):
                end_line = len(lines) - 1

            # Extract the requested subset of lines
            subset_lines = lines[start_line:end_line + 1]

            if include_line_numbers:
                # Format lines with line numbers
                formatted_lines = [f"{start_line + i}: {line}" for i, line in enumerate(subset_lines)]
                subset_content = '\n'.join(formatted_lines)
            else:
                subset_content = '\n'.join(subset_lines)

            token_count = self._count_tokens(subset_content, tool_context)
            if token_count > max_tokens:
                return (f"ERROR: The contents of those lines exceed the  max_tokens limit of {max_tokens}. "
                        f"Content token count: {token_count}. This file has {len(lines)} lines. "
                        f"You will need read fewer lines or raise the token limit. ")

            return subset_content

        except Exception as e:
            self.logger.exception(f'Error reading file lines for {unc_path}, start line {start_line}, end line {end_line},, error message: {str(e)}', exc_info=True)
            return f'Error reading file lines, for {unc_path}: {str(e)}'

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
        """Uses CodeExplorer to prepare code overviews for Python, JavaScript and C# files.

        Args:
            path (str): UNC-style path (//WORKSPACE/path) to the file to read

        Returns:
            str: A markdown overview of the code
        """
        unc_path = kwargs.get('path', '')

        error, workspace, relative_path = self.validate_and_get_workspace_path(unc_path)
        if error:
            return f'Error: {str(error)}'


        try:
            file_content = await workspace.read_internal(relative_path)
        except Exception as e:
            self.logger.exception(f'Error fetching file {unc_path}: {str(e)}', exc_info=True)
            return f'Error fetching {unc_path}: {str(e)}'

        try:
            context = api.get_code_context(file_content, format='markdown', filename=unc_path)
        except Exception as e:
            self.logger.warning(f'Error inspecting code {unc_path}: {str(e)}')
            return f'Error inspecting code {unc_path}: {str(e)}'

        return context


    @json_schema(
        description="Find files matching a glob pattern in a workspace. Equivalent to `glob.glob` in Python",
        params={
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) with glob pattern to find matching files",
                "required": True
            },
            "recursive": {
                "type": "boolean",
                "description": "Whether to search recursively, matching ** patterns",
                "required": False
            },
            "include_hidden": {
                "type": "boolean",
                "description": "Whether to include hidden files in ** pattern matching",
                "required": False
            },
            "max_tokens": {
                "type": "integer",
                "description": "Maximum size in tokens for the response. Default is 2000.",
                "required": False
            }
        }
    )
    async def glob(self, **kwargs: Any) -> str:
        """Find files matching a glob pattern in a workspace.  This uses the Python `glob` module under the hood.
        
        Args:
            **kwargs: Keyword arguments.
                path (str): UNC-style path (//WORKSPACE/path) with glob pattern to find matching files
                recursive (bool): Whether to search recursively (defaults to False)
                include_hidden (bool): Whether to include hidden files (defaults to False)
                max_tokens (int): Maximum size in tokens for the response. Default is 2000.
                
        Returns:
            str: string with the list of matching files or an error message.
        """
        # Get the path with the glob pattern
        unc_path = kwargs.get('path', '')
        recursive = kwargs.get('recursive', False)
        include_hidden = kwargs.get('include_hidden', False)
        tool_context = kwargs.get("context")
        max_tokens = kwargs.get("max_tokens", 4000)
        
        if not unc_path:
            return f"ERROR: `path` cannot be empty"
            
        workspace_name, workspace, relative_pattern = self._parse_unc_path(unc_path)
        
        if not workspace:
            return f"ERROR:'Invalid workspace: {workspace_name}'"
            
        try:
            # Use the workspace's glob method to find matching files
            matching_files = await workspace.glob(relative_pattern, recursive=recursive, include_hidden=include_hidden)
            
            # Convert the files back to UNC paths
            unc_files = [f'//{workspace_name}/{file}' for file in matching_files]
            response = f"Found {len(unc_files)} files matching '{relative_pattern}':\n" + "\n".join(unc_files)

            token_count = self._count_tokens(response, tool_context)
            if token_count > max_tokens:
                return (f"ERROR: Response exceeds max_tokens limit of {max_tokens}. Token count: {token_count}. "
                        f"You will need to adjust your pattern or raise the token limit")

            return response
        except Exception as e:
            self.logger.exception(f"Error during glob operation: {str(e)}", exc_info=True)
            return  f'Error during glob operation: {str(e)}. This has been logged.'

    @json_schema(
        'Run `grep -n`  over files in workspaces using UNC-style paths',
        {
            'paths': {
                "type": "array",
                "items": {
                    "type": "string"
                },
                'description': 'UNC-style paths (//WORKSPACE/path) to grep, wildcards ARE supported',
                'required': True
            },
            'pattern': {
                'type': 'string',
                'description': 'Grep pattern to search for',
                'required': True
            },
            'ignore_case': {
                'type': 'boolean',
                'description': 'Set to true to ignore case',
                'required': False
            },
            'recursive': {
                'type': 'boolean',
                'description': 'Set to true to recursively search subdirectories',
                'required': False
            },
            'max_tokens': {
                'type': 'integer',
                'description': 'Maximum size in tokens for the response. Default is 2000.',
                'required': False
            }
        }
    )
    async def grep(self, **kwargs: Any) -> str:
        """Run grep over files in workspaces using UNC-style paths. This is a literal shell out to `grep -n` command.
        
        Args:
            **kwargs: Keyword arguments.
                paths (list): UNC-style paths (//WORKSPACE/path) to grep
                pattern (str): Grep pattern to search for
                recursive (bool): Set to true to recursively search subdirectories
                ignore_case (bool): Set to true to ignore case
                
        Returns:
            str: Output of grep command with line numbers.
        """
        unc_paths = kwargs.get('paths', [])
        tool_context = kwargs.get("context")
        max_tokens = kwargs.get("max_tokens", 2000)
        if not isinstance(unc_paths, list):
            if isinstance(unc_paths, str):
                unc_paths = [unc_paths]
            else:
                return f"ERROR `paths` must be a list of UNC-style paths"

        pattern = kwargs.get('pattern', '')
        ignore_case = kwargs.get('ignore_case', False)
        recursive = kwargs.get('recursive', False)
        errors = []
        queue = {}
        results = []
        
        if not pattern:
            return 'Error: `pattern` cannot be empty'
        
        if not unc_paths:
            return 'Error: `paths` cannot be empty'

        for punc_path in unc_paths:
            error, workspace, relative_path = self.validate_and_get_workspace_path(punc_path)
            if error:
                errors.append(f"Error processing path {punc_path}: {error}")
                continue
            if workspace not in queue:
                queue[workspace] = []

            queue[workspace].append(relative_path)

        # Now process each workspace's files
        for workspace, paths in queue.items():
            try:
                # Use the workspace's grep method to search for the pattern
                result = await workspace.grep(
                    pattern=pattern,
                    file_paths=paths,
                    ignore_case=ignore_case,
                    recursive=recursive
                )
                results.append(result)

            except Exception as e:
                self.logger.exception(f"Error searching files in workspace {workspace.name} with pattern '{pattern}': {str(e)}")
                results.append(f'Error searching files: {str(e)}')
        
        err_str = ""
        if errors:
            err_str = f"Errors:\n{"\n".join(errors)}\n\n"

        response = f"{err_str}Results:\n" + "\n".join(results)
        token_count = self._count_tokens(response, tool_context)
        if token_count > max_tokens:
            return (f"ERROR: Match results exceed max_tokens limit of {max_tokens}. "
                    f"Results token count: {token_count} from {len(results)} results. "
                    f"You will need to adjust your pattern or raise the token limit.")

        return response


    @json_schema(
        description="Read a from the metadata for a workspace using a UNC style path. Nested paths are supported using slash notation ",
        params={
            "path": {
                "type": "string",
                "description": "UNC style path in the form of //[workspace]/meta/toplevel/subkey1/subkey2",
                "required": True
            },
            "max_tokens": {
                "type": "integer",
                "description": "Maximum size in tokens for the response.  Default is 20k.",
                "required": False
            }
        }
    )
    async def read_meta(self, **kwargs: Any) -> str:
        """Read a specific value from the workspace's metadata file. Nested objects are supported using slash notation

        Args:
            path (str): The UNC path to the key to read, supports slash notation for nested keys (e.g., 'parent/child')
            max_tokens (int): Maximum size in tokens for the response. Default is 20000.
        Returns:
            str: A YAML formatted string with the value for the specified key or an error message.
        """
        path = kwargs.get("path")
        tool_context = kwargs.get("context")
        max_tokens = kwargs.get("max_tokens", 20000)
        error, workspace, key = self._parse_unc_path(path)
        if not key:
            key = "meta/"

        if error is not None:
            return f"error {str(error)}"

        try:
            value = await workspace.safe_metadata(key)
            if value is None:
                self.logger.warning(f"Key '{key}' not found in metadata for workspace '{workspace.name}'")

            if isinstance(value, dict) or isinstance(value, list):
                response = self._yaml_dump(value)
                token_count = self._count_tokens(response, tool_context)
                if token_count > max_tokens:
                    return (f"ERROR: Key content exceeds max_tokens limit of {max_tokens}. "
                            f"Content token count: {token_count}. "
                            f"You can use `get_meta_keys` to list keys in this section in order to be more precise.")

                return response

            elif isinstance(value, str):
                token_count = self._count_tokens(value, tool_context)
                if token_count > max_tokens:
                    return (f"ERROR: Key content exceeds max_tokens limit of {max_tokens}. "
                            f"Content token count: {token_count}. "
                            f"You can use `get_meta_keys` to list keys in this section in order to be more precise.")

            return str(value)
        except Exception as e:
            self.logger.exception(f"Failed to read metadata {key} from workspace {workspace.name}: {str(e)}")
            return  f"Failed to read metadata {key} error: {str(e)}"

    @json_schema(
        description="List the keys in a section of the metadata for a workspace using a UNC style path. Nested paths are supported using slash notation ",
        params={
            "path": {
                "type": "string",
                "description": "UNC style path in the form of //[workspace]/meta/toplevel/subkey1/subkey2",
                "required": True
            },
            "max_tokens": {
                "type": "integer",
                "description": "Maximum size in tokens for the response.  Default is 20k.",
                "required": False
            }
        }
    )
    async def get_meta_keys(self, **kwargs: Any) -> str:
        """
        List the keys in a section of the metadata for a workspace using a UNC style path. Nested paths are supported using slash notation

        Args:
            path (str): The UNC path to the key to read, supports slash notation for nested keys (e.g., 'parent/child')
            max_tokens (int): Maximum size in tokens for the response. Default is 20000.
        Returns:
            str: The value for the specified key as a YAML formatted string or an error message.
        """
        path = kwargs.get("path")
        tool_context = kwargs.get("context")
        max_tokens = kwargs.get("max_tokens", 20000)
        error, workspace, key = self._parse_unc_path(path)

        if error is not None:
            return f"Error: {str(error)}"

        try:
            value = await workspace.safe_metadata(key)
            if value is None:
                self.logger.warning(f"Key '{key}' not found in metadata for workspace '{workspace.name}'")
            elif isinstance(value, dict):
                response = self._yaml_dump(list(value.keys()))
                token_count = self._count_tokens(response, tool_context)
                if token_count > max_tokens:
                    return f"ERROR: Response exceeds max_tokens limit of {max_tokens}. Current token count: {token_count}."

                return response

            return f"'{key}' is not a dictionary it is a {type(value).__name__}."
        except Exception as e:
            self.logger.exception(f"Failed to read metadata {key} from workspace {workspace.name}: {str(e)}")
            return f"Failed to read metadata {key} error: {str(e)}"

    @json_schema(
        description="Write to a key in the metadata for a workspace using a UNC style path. Nested paths are supported using slash notation ",
        params={
            "path": {
                "type": "string",
                "description": "Name of the workspace to write metadata to",
                "required": True
            },
            "data": {
                "oneOf": [
                    {"type": "object"},
                    {
                        "type": "array",
                        "items": {
                            "oneOf": [
                                {"type": "object"},
                                {"type": "string"},
                                {"type": "number"},
                                {"type": "boolean"},
                                {"type": "null"}
                            ]
                        }
                    },
                    {"type": "string"},
                    {"type": "number"},
                    {"type": "boolean"},
                    {"type": "null"}
                ],
                "description": "The metadata to write.",
                "required": True
            }
        }
    )
    async def write_meta(self, **kwargs: Any) -> str:
        """Write to a key in the metadata for a workspace using a UNC style path. Nested paths are supported using slash notation.
        Args:
            path (str): The UNC path to the key to write, supports slash notation for nested keys (e.g., 'parent/child')
            data (Union[dict, list, str, int, float, bool, None]): The metadata to write.
        Returns:
            str: A success message or an error message.
        """
        data = kwargs.get("data")
        path = kwargs.get("path")
        error, workspace, key = self._parse_unc_path(path)

        if error is not None:
            return error

        if workspace.read_only:
            return f"Workspace '{workspace.name}' is read-only"

        try:
            await workspace.safe_metadata_write(key, data)
            return f"Saved metadata to '{key}' in {workspace.name} workspace."
        except Exception as e:
            return f"Failed to write metadata to '{key}' in {workspace.name} workspace: {str(e)}"

Toolset.register(WorkspaceTools)