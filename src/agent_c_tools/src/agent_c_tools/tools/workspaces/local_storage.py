import base64
import os
import json
import logging

from pathlib import Path
from typing import Optional, Union

from agent_c.util import generate_path_tree
from agent_c.util.token_counter import TokenCounter
from agent_c_tools.tools.workspaces.base import BaseWorkspace


class LocalStorageWorkspace(BaseWorkspace):
    def __init__(self, **kwargs):
        super().__init__("local_storage", **kwargs)
        workspace_path: Optional[str] = kwargs.get('workspace_path')
        self.max_token_size: int = kwargs.get('max_size', 50000)
        self.valid: bool = isinstance(workspace_path, str)
        if self.valid:
            self.workspace_root: Path = Path(workspace_path).resolve()

        # Check environment for development/local mode
        env = os.environ.get('ENVIRONMENT', '').lower()
        self.allow_symlinks: bool = kwargs.get('allow_symlinks', 'local' in env or 'dev' in env)
        self.logger: logging.Logger = logging.getLogger(__name__)

        if self.allow_symlinks:
            self.logger.info("Symlink paths are allowed in workspace")

        self.max_filename_length = 200

    def _normalize_input_path(self, path: str) -> str:
        """
        Normalize the input path so it uses the correct OS-specific separator.

        This method first converts all backslashes to forward slashes, and then,
        if the OS separator isn’t '/', it replaces them with the proper one.
        Finally, it normalizes the path.
        """
        # Unify slashes by replacing backslashes with forward slashes
        normalized = path.replace("\\", "/")
        # If the OS separator isn't '/', replace forward slashes accordingly
        if os.sep != "/":
            normalized = normalized.replace("/", os.sep)
        return os.path.normpath(normalized)

    def _is_path_within_workspace(self, path: str) -> bool:
        """Check if the provided path is within the workspace.

        With symlink allowance enabled, this checks the path string itself rather than
        resolving to physical paths, which would break in Docker environments.

        Args:
            path (str): The path to check.

        Returns:
            bool: True if the path is valid within the workspace context, else False.
        """
        norm_path = self._normalize_input_path(path)
        # Prevent path traversal attacks with ".." regardless of symlink settings
        if ".." in norm_path.split(os.sep):
            return False

        if self.allow_symlinks:
            return True

        resolved_path = self.workspace_root.joinpath(norm_path).resolve()
        return self.workspace_root in resolved_path.parents or resolved_path == self.workspace_root

    async def tree(self, relative_path: str) -> str:
        norm_path = self._normalize_input_path(relative_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The path {relative_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(norm_path)
        return '\n'.join(generate_path_tree(str(full_path)))

    async def ls(self, relative_path: str) -> str:
        norm_path = self._normalize_input_path(relative_path)
        if not self._is_path_within_workspace(norm_path) or norm_path == '':
            error_msg = f'The path {relative_path} is not within the workspace.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(norm_path)
        try:
            contents = os.listdir(full_path)
            return json.dumps({'contents': contents})
        except Exception as e:
            error_msg = str(e)
            self.logger.exception("Failed to list directory contents.")
            return json.dumps({'error': error_msg})

    async def read(self, file_path: str) -> str:
        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(norm_path)
        try:
            if full_path.is_file():
                with open(full_path, 'r', encoding='utf-8') as file:
                    contents = file.read()
                    file_tokens = TokenCounter.count(contents)
                    if file_tokens > self.max_token_size:
                        error_msg = (
                            f'The file {file_path} exceeds the token limit of '
                            f'{self.max_token_size}.  Actual size is {file_tokens}.'
                        )
                        self.logger.error(error_msg)
                        return json.dumps({'error': error_msg})
                    return json.dumps({'contents': contents})
            else:
                error_msg = f'The path {file_path} is not a file.'
                self.logger.error(error_msg)
                return json.dumps({'error': error_msg})
        except Exception as e:
            error_msg = f'An error occurred while reading the file: {e}'
            self.logger.exception("Failed to read the file.")
            return json.dumps({'error': error_msg})

    async def read_bytes_internal(self, file_path: str) -> bytes:
        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            raise ValueError(error_msg)

        full_path: Path = self.workspace_root.joinpath(norm_path)
        try:
            if full_path.is_file():
                with open(full_path, 'rb') as file:
                    contents = file.read()
                return contents
            else:
                error_msg = f'The path {full_path} is not a file.'
                self.logger.error(error_msg)
                raise ValueError(error_msg)
        except Exception as e:
            error_msg = f'An error occurred while reading the file: {e}'
            self.logger.exception("Failed to read the file.")
            raise RuntimeError(error_msg)

    async def read_bytes_base64(self, file_path: str) -> str:
        return base64.b64encode(await self.read_bytes_internal(file_path)).decode('utf-8')

    def full_path(self, file_path: str, mkdirs: bool = True) -> Union[str, None]:
        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return None

        full_path: Path = self.workspace_root.joinpath(norm_path)
        if mkdirs:
            full_path.parent.mkdir(parents=True, exist_ok=True)  # Ensure directory exists in case they're going to write.
        return str(full_path)

    async def path_exists(self, file_path: str) -> bool:
        norm_path = self._normalize_input_path(file_path)
        return self.workspace_root.joinpath(norm_path).exists()

    async def write(self, file_path: str, mode: str, data: str) -> str:
        if self.read_only:
            return json.dumps({'error': 'This workspace is read-only.'})

        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(norm_path)
        try:
            full_path.parent.mkdir(parents=True, exist_ok=True)  # Ensure directory exists
            write_mode = 'w' if (mode == 'write' or mode == 'w') else 'a'
            with open(full_path, write_mode, encoding='utf-8') as file:
                file.write(data)

            action = 'written to' if write_mode == 'w' else 'appended in'
            return json.dumps({'message': f'Data successfully {action} {file_path}.'})
        except Exception as e:
            error_msg = f'An error occurred while writing to the file: {e}'
            self.logger.exception(f"Failed to write to the file: {file_path}")
            return json.dumps({'error': error_msg})

    async def write_bytes(self, file_path: str, mode: str, data: bytes) -> str:
        if self.read_only:
            return json.dumps({'error': 'This workspace is read-only.'})

        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(norm_path)
        try:
            full_path.parent.mkdir(parents=True, exist_ok=True)  # Ensure directory exists
            write_mode = 'wb' if (mode == 'write' or mode == 'wb') else 'ab'
            with open(full_path, write_mode) as file:
                file.write(data)

            action = 'written to' if write_mode == 'wb' else 'appended in'
            return json.dumps({'message': f'Data successfully {action} {file_path}.'})
        except Exception as e:
            error_msg = f'An error occurred while writing to the file: {e}'
            self.logger.exception(f"Failed to write to the file: {file_path}")
            return json.dumps({'error': error_msg})


class LocalProjectWorkspace(LocalStorageWorkspace):
    """
    A workspace that automatically determines the project path using a fallback strategy:
    1. Uses PROJECT_WORKSPACE_PATH environment variable if available
    2. Uses 'app/workspaces/project' path if it exists
    3. Defaults to current working directory

    The description can be overridden via PROJECT_WORKSPACE_DESCRIPTION environment variable.
    """
    def __init__(self, name="project", default_description="A workspace holding the `Agent C` source code in Python."):
        self.logger = logging.getLogger("agent_c_tools.tools.workspaces.local_project_workspace")
        self.logger.info("Initializing LocalProjectWorkspace")
        workspace_path = self._determine_workspace_path()
        description = os.environ.get("PROJECT_WORKSPACE_DESCRIPTION", default_description)
        super().__init__(
            name=name,
            workspace_path=workspace_path,
            description=description
        )

    def _determine_workspace_path(self) -> str:
        if "PROJECT_WORKSPACE_PATH" in os.environ:
            self.logger.info(f"Found PROJECT_WORKSPACE_PATH environment variable: {os.environ['PROJECT_WORKSPACE_PATH']}")
            return os.environ["PROJECT_WORKSPACE_PATH"]

        app_workspace_path = Path("/app/workspaces/project")
        if app_workspace_path.exists():
            self.logger.info(f"Found /app/workspaces/project directory: {str(app_workspace_path.absolute())}")
            return str(app_workspace_path.absolute())

        self.logger.info(f"Using current working directory as the project workspace: {os.getcwd()}")
        return os.getcwd()
