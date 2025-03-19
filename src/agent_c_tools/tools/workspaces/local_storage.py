import base64
import os
import json
import logging

from pathlib import Path
from typing import Optional, Union

from agent_c_core.agent_c.util.token_counter import TokenCounter
from agent_c_tools.tools.workspaces.base import BaseWorkspace


class LocalStorageWorkspace(BaseWorkspace):
    def __init__(self, **kwargs):
        super().__init__("local_storage", **kwargs)
        workspace_path: Optional[str] = kwargs.get('workspace_path')
        self.max_token_size: int = kwargs.get('max_size', 50000)
        self.valid: bool = isinstance(workspace_path, str)
        if self.valid:
            self.workspace_root: Path = Path(workspace_path).resolve()

        self.max_filename_length = 200
        self.logger: logging.Logger = logging.getLogger(__name__)

    def _is_path_within_workspace(self, path: str) -> bool:
        """Check if the provided path is within the workspaces.

        Args:
            path (str): The path to check.

        Returns:
            bool: True if the path is within the workspaces, else False.
        """
        resolved_path = self.workspace_root.joinpath(path).resolve()
        return self.workspace_root in resolved_path.parents or resolved_path == self.workspace_root

    async def ls(self, relative_path: str) -> str:
        if not self._is_path_within_workspace(relative_path):
            error_msg = f'The path {relative_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(relative_path)
        try:
            contents = os.listdir(full_path)
            return json.dumps({'contents': contents})
        except Exception as e:
            error_msg = str(e)
            self.logger.exception("Failed to list directory contents.")
            return json.dumps({'error': error_msg})

    async def read(self, file_path: str) -> str:
        if not self._is_path_within_workspace(file_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(file_path)
        try:
            if full_path.is_file():
                # TODO: Direct model to use a dedicated large file tool when complete
                with open(full_path, 'r', encoding='utf-8') as file:
                    contents = file.read()
                    file_tokens = TokenCounter.count(contents)

                    if file_tokens > self.max_token_size:
                        error_msg = f'The file {file_path} exceeds the token limit of {self.max_token_size}.  Actual size is {file_tokens}.'
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
        if not self._is_path_within_workspace(file_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            raise ValueError(error_msg)

        full_path: Path = self.workspace_root.joinpath(file_path)
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
        if not self._is_path_within_workspace(file_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return None

        full_path: Path = self.workspace_root.joinpath(file_path)
        if mkdirs:
            full_path.parent.mkdir(parents=True, exist_ok=True)  # Ensure directory exists in case they're going to write.

        return str(full_path)

    async def path_exists(self, file_path: str) -> bool:
        return self.workspace_root.joinpath(file_path).exists()

    async def write(self, file_path: str, mode: str, data: str) -> str:
        if self.read_only:
            return json.dumps({'error': 'This workspace is read-only.'})

        if not self._is_path_within_workspace(file_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(file_path)
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

        if not self._is_path_within_workspace(file_path):
            error_msg = f'The file {file_path} is not within the workspaces.'
            self.logger.error(error_msg)
            return json.dumps({'error': error_msg})

        full_path: Path = self.workspace_root.joinpath(file_path)
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
