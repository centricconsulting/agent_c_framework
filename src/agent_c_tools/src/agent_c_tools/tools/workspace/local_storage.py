import asyncio
import os
import json
import base64
import re
import shutil
import logging
import functools
import glob

from pathlib import Path
from typing import Optional, Union, Tuple, Callable, TypeVar, List, Dict

from agent_c.util.token_counter import TokenCounter
from agent_c_tools.tools.workspace.base import BaseWorkspace
from agent_c_tools.tools.workspace.util.trees.renderers.minimal import MinimalTreeRenderer
from agent_c_tools.tools.workspace.util.trees.builders.local_file_system import LocalFileSystemTreeBuilder

# Type variable for function return types
T = TypeVar('T')

class LocalStorageWorkspace(BaseWorkspace):
    def __init__(self, **kwargs):
        super().__init__("local_storage", **kwargs)
        workspace_path: Optional[str] = kwargs.get('workspace_path')
        self.max_token_size: int = kwargs.get('max_size', 50000)
        self.valid: bool = isinstance(workspace_path, str)
        if self.valid:
            self.workspace_root: Path = Path(workspace_path).resolve()

        env = os.environ.get('ENVIRONMENT', '').lower()
        self.allow_symlinks: bool = kwargs.get('allow_symlinks', 'local' in env or 'dev' in env)
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.max_filename_length = 200

    @staticmethod
    def _normalize_input_path(path: str) -> str:
        """
        Normalize the input path so it uses the correct OS-specific separator.

        This method first converts all backslashes to forward slashes, and then,
        if the OS separator isn't '/', it replaces them with the proper one.
        Finally, it normalizes the path.
        """
        # Unify slashes by replacing backslashes with forward slashes
        normalized = path.replace("\\", "/")
        # If the OS separator isn't '/', replace forward slashes accordingly
        if os.sep != "/":
            normalized = normalized.replace("/", os.sep)
        return os.path.normpath(normalized)

    def _is_path_within_workspace(self, path: str) -> bool:
        """
        Check if the provided path is within the workspace.

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

    def _find_nearest_ignore_file(self, path: Path) -> str:
        """Find the nearest .agentcignore file by walking up the directory tree."""
        current_dir = path if path.is_dir() else path.parent

        while current_dir.is_relative_to(self.workspace_root):
            ignore_file = current_dir.joinpath('.agentcignore')
            if ignore_file.exists():
                with open(ignore_file, 'r') as f:
                    return f.read()
            current_dir = current_dir.parent

        return ""

    def _error_response(self, message: str) -> str:
        """Create a standardized error response."""
        self.logger.error(message)
        return f'Error: {message}'

    @staticmethod
    def _success_response(message: str) -> str:
        """Create a standardized success response."""
        return message

    def _validate_path(self, path: str, error_context: str = "file") -> Tuple[bool, str, Optional[Path]]:
        """Validate a path and return normalized path and full path if valid.

        Args:
            path: The relative path to validate
            error_context: Context string for error messages ("file", "source path", etc.)

        Returns:
            Tuple of (is_valid, error_message_or_empty, full_path_or_none)
        """
        norm_path = self._normalize_input_path(path)
        if not self._is_path_within_workspace(norm_path):
            return False, f'The {error_context} {path} is not within the workspace.', None

        full_path = self.workspace_root.joinpath(norm_path)
        return True, "", full_path

    def _check_read_only(self) -> Optional[str]:
        """Check if workspace is read-only and return error response if it is."""
        if self.read_only:
            return json.dumps({'error': 'This workspace is read-only.'})
        return None

    @staticmethod
    def _ensure_parent_dirs(path: Path) -> None:
        """Ensure parent directories exist for the given path."""
        path.parent.mkdir(parents=True, exist_ok=True)

    # Decorator for methods that require write access
    def _requires_write_access(self, func: Callable[..., T]) -> Callable[..., T]:
        """Decorator to check if workspace is read-only before executing a method."""

        @functools.wraps(func)
        async def wrapper(*args, **kwargs):
            read_only_error = self._check_read_only()
            if read_only_error:
                return read_only_error
            return await func(*args, **kwargs)

        return wrapper

    async def tree(self, relative_path: str, folder_depth: int, file_depth: int) -> str:
        valid, error_msg, full_path = self._validate_path(relative_path, "path")
        if not valid:
            return self._error_response(error_msg)

        ignore_patterns = self._find_nearest_ignore_file(full_path)

        builder = LocalFileSystemTreeBuilder(ignore_patterns=ignore_patterns)
        if relative_path is not None and relative_path != '' and relative_path != '/':
            root_name = f"//{self.name}/{relative_path}"
        else:
            root_name = f"//{self.name}"

        tree = builder.build_tree(start_path=str(full_path),
                                  root_name=root_name)
        renderer = MinimalTreeRenderer()
        tree_txt = "\n".join(renderer.render(
            tree=tree,
            max_depth=folder_depth,
            max_files_depth=file_depth,
            include_root=True
        ))
        return f"Folder depth: {folder_depth}, File Depth {file_depth}\n{tree_txt}"

    async def ls(self, relative_path: str) -> Union[List, str]:
        valid, error_msg, full_path = self._validate_path(relative_path, "path")
        if not valid:
            return self._error_response(error_msg)

        try:
            contents = os.listdir(full_path)
            return contents
        except Exception as e:
            error_msg = str(e)
            self.logger.exception(f"Failed to list directory contents. {error_msg}", exc_info=True)
            return self._error_response(error_msg)

    async def read(self, file_path: str, encoding: Optional[str] = "utf-8") -> str:
        try:
            contents = await self.read_internal(file_path)
            if len(contents):
                file_tokens = TokenCounter.count(contents)
                if file_tokens > self.max_token_size:
                    return self._error_response(
                        f'The file {file_path} exceeds the token limit of '
                        f'{self.max_token_size}. Actual size is {file_tokens}.'
                    )
            return await self.read_internal(file_path)
        except Exception as e:
            return self._error_response(str(e))

    async def read_internal(self, file_path: str, encoding: Optional[str] = "utf-8") -> str:
        valid, error_msg, full_path = self._validate_path(file_path)
        if not valid:
            raise ValueError(error_msg)

        try:
            if not full_path.exists():
                raise FileNotFoundError(f'The path {file_path} does not exist.')

            if full_path.is_file():
                with open(full_path, 'r', encoding=encoding, errors='replace') as file:
                    return file.read()
            else:
                return self._error_response(f'The path {file_path} is not a file.')
        except Exception as e:
            error_msg = f'LocalStorageWorkspace.read_internal An error occurred while reading the file: {e}'
            self.logger.exception("Failed to read the file.")
            raise Exception(error_msg)

    async def read_bytes_internal(self, file_path: str) -> bytes:
        valid, error_msg, full_path = self._validate_path(file_path)
        if not valid:
            raise ValueError(error_msg)

        try:
            if full_path.is_file():
                with open(full_path, 'rb') as file:
                    return file.read()
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
        valid, error_msg, full_path = self._validate_path(file_path)
        if not valid:
            return None

        if mkdirs:
            self._ensure_parent_dirs(full_path)
        return str(full_path)

    async def path_exists(self, file_path: str) -> bool:
        norm_path = self._normalize_input_path(file_path)
        return self.workspace_root.joinpath(norm_path).exists()
        
    async def is_directory(self, file_path: str) -> bool:
        """Check if the given path is a directory.
        
        Args:
            file_path (str): The path to check, relative to the workspace root
            
        Returns:
            bool: True if the path is a directory, False otherwise
        """
        norm_path = self._normalize_input_path(file_path)
        if not self._is_path_within_workspace(norm_path):
            self.logger.error(f'The path {file_path} is not within the workspace.')
            return False
            
        full_path = self.workspace_root.joinpath(norm_path)
        return full_path.is_dir()

    async def write(self, file_path: str, mode: str, data: str) -> str:
        read_only_error = self._check_read_only()
        if read_only_error:
            return read_only_error
        return await self._write_impl(file_path, mode, data, is_binary=False)

    async def write_bytes(self, file_path: str, mode: str, data: bytes) -> str:
        read_only_error = self._check_read_only()
        if read_only_error:
            return read_only_error
        return await self._write_impl(file_path, mode, data, is_binary=True)

    async def _write_impl(self, file_path: str, mode: str, data: Union[str, bytes], is_binary: bool) -> str:
        valid, error_msg, full_path = self._validate_path(file_path)
        if not valid:
            return self._error_response(error_msg)

        try:
            self._ensure_parent_dirs(full_path)

            if is_binary:
                write_mode = 'wb' if (mode == 'write' or mode == 'wb') else 'ab'
            else:
                write_mode = 'w' if (mode == 'write' or mode == 'w') else 'a'

            with open(full_path, write_mode, encoding=None if is_binary else 'utf-8') as file:
                file.write(data)

            action = 'written to' if write_mode.startswith('w') else 'appended in'
            return self._success_response(f'Data successfully {action} {file_path}.')
        except Exception as e:
            error_msg = f'An error occurred while writing to the file: {e}'
            self.logger.exception(f"Failed to write to the file: {file_path}")
            return self._error_response(error_msg)

    async def cp(self, src_path: str, dest_path: str) -> str:
        """Copy a file or directory from src_path to dest_path within the workspace."""
        read_only_error = self._check_read_only()
        if read_only_error:
            return read_only_error
        return await self._transfer_impl(src_path, dest_path, is_move=False)

    async def mv(self, src_path: str, dest_path: str) -> str:
        """Move a file or directory from src_path to dest_path within the workspace."""
        read_only_error = self._check_read_only()
        if read_only_error:
            return read_only_error
        return await self._transfer_impl(src_path, dest_path, is_move=True)

    async def _transfer_impl(self, src_path: str, dest_path: str, is_move: bool) -> str:
        """Implementation for both copy and move operations."""
        operation = "move" if is_move else "copy"

        # Validate source path
        valid_src, src_error, full_src_path = self._validate_path(src_path, "source path")
        if not valid_src:
            return self._error_response(src_error)

        # Validate destination path
        valid_dest, dest_error, full_dest_path = self._validate_path(dest_path, "destination path")
        if not valid_dest:
            return self._error_response(dest_error)

        # Check if source exists
        if not full_src_path.exists():
            return self._error_response(f'The source path {src_path} does not exist.')

        try:
            # Create parent directories for destination
            self._ensure_parent_dirs(full_dest_path)

            # Handle destination being an existing directory
            if full_dest_path.exists() and full_dest_path.is_dir():
                dest = full_dest_path / full_src_path.name
                if is_move:
                    shutil.move(str(full_src_path), str(dest))
                else:  # copy
                    if full_src_path.is_file():
                        shutil.copy2(full_src_path, dest)
                    else:  # directory
                        shutil.copytree(full_src_path, dest)
                return self._success_response(f'{src_path} successfully {operation}d to {dest_path}/{full_src_path.name}.')
            else:
                # Handle direct file/directory transfer
                if full_src_path.is_file():
                    if is_move:
                        shutil.move(str(full_src_path), str(full_dest_path))
                    else:  # copy
                        shutil.copy2(full_src_path, full_dest_path)
                elif full_src_path.is_dir():
                    if is_move:
                        shutil.move(str(full_src_path), str(full_dest_path))
                    else:  # copy
                        shutil.copytree(full_src_path, full_dest_path)
                else:
                    return self._error_response(f'The source path {src_path} is neither a file nor a directory.')

                return self._success_response(f'{src_path} successfully {operation}d to {dest_path}.')
        except Exception as e:
            error_msg = f'An error occurred while {operation}ing: {e}'
            self.logger.exception(f"Failed to {operation}.")
            return self._error_response(error_msg)

    async def walk(self, start_path: str, extensions: List[str] = None) -> Tuple[Optional[str], List[str]]:
        """
        Walk through the workspace starting from the given path and return a list of files.

        Args:
            start_path (str): The starting path to walk from, relative to the workspace root.
            extensions (List[str]): Optional list of file extensions to filter by.

        Returns:
            List[str]: A list of relative file paths found in the workspace.
        """
        valid, error_msg, full_path = self._validate_path(start_path, "start path")
        if not valid:
            return error_msg, []

        if not full_path.is_dir():
            return f'The path {start_path} is not a directory.', []

        files = []
        for root, _, filenames in os.walk(full_path):
            for filename in filenames:
                if extensions is None or any(filename.endswith(ext) for ext in extensions):
                    relative_path = os.path.relpath(os.path.join(root, filename), self.workspace_root)
                    files.append(relative_path.replace("\\", "/"))

        return None, files

    async def glob(self, pattern: str, recursive: bool = False, include_hidden: bool = False) -> List[str]:
        """
        Find paths matching the specified pattern in the workspace.

        Args:
            pattern (str): The glob pattern to match against paths in the workspace.
            recursive (bool): Whether to search recursively, matching ** patterns.
            include_hidden (bool): Whether to include hidden files in ** pattern matching.

        Returns:
            List[str]: A list of relative paths that match the pattern.
        """
        norm_pattern = self._normalize_input_path(pattern)

        # Extract the non-wildcard prefix more carefully
        # We need to find the directory path that contains no wildcards
        wildcard_match = re.search(r'[*?\[\]]', norm_pattern)

        if wildcard_match:
            # Find the position of the first wildcard
            wildcard_pos = wildcard_match.start()

            # Find the last directory separator before the first wildcard
            # This ensures we get the complete directory path without wildcards
            prefix_part = norm_pattern[:wildcard_pos]
            last_sep_pos = max(
                prefix_part.rfind('/'),
                prefix_part.rfind('\\')
            )

            if last_sep_pos >= 0:
                # Include the separator in the prefix
                non_wildcard_prefix = norm_pattern[:last_sep_pos + 1]
            else:
                # No directory separator found before wildcard
                # The wildcard is in the current directory
                non_wildcard_prefix = ""
        else:
            # No wildcards found, the entire pattern is the prefix
            non_wildcard_prefix = norm_pattern

        # Debug logging (you can remove these later)
        self.logger.debug(f"Original pattern: '{pattern}'")
        self.logger.debug(f"Normalized pattern: '{norm_pattern}'")
        self.logger.debug(f"Non-wildcard prefix: '{non_wildcard_prefix}'")

        # Validate that the prefix is within the workspace
        if non_wildcard_prefix and not self._is_path_within_workspace(non_wildcard_prefix):
            self.logger.error(f'The pattern {pattern} is not within the workspace.')
            return []

        # Construct the full pattern with workspace root
        full_pattern = str(self.workspace_root / norm_pattern)
        self.logger.debug(f"Full glob pattern: '{full_pattern}'")

        try:
            # Use glob.glob with parameters matching Python's standard glob
            matching_paths = glob.glob(
                full_pattern,
                recursive=recursive,
                include_hidden=include_hidden
            )

            self.logger.debug(f"Raw glob results: {len(matching_paths)} matches")

            # Convert absolute paths back to workspace-relative paths
            relative_paths = []
            workspace_root_str = str(self.workspace_root)

            for path in matching_paths:
                # Normalize the path to handle different separators
                normalized_path = os.path.normpath(path)

                # Remove the workspace root prefix and convert to workspace-relative path
                if normalized_path.startswith(workspace_root_str):
                    # Calculate the relative path
                    rel_path = os.path.relpath(normalized_path, workspace_root_str)
                    # Convert to forward slashes for consistency
                    rel_path = rel_path.replace("\\", "/")
                    relative_paths.append(rel_path)

            self.logger.debug(f"Final relative paths: {relative_paths}")
            return relative_paths

        except Exception as e:
            self.logger.exception(f"Error during glob operation with pattern '{pattern}': {e}")
            return []

    
    async def grep(self,
            pattern: str,
            file_paths: Union[str, List[str]],
            ignore_case: bool = False,
            recursive: bool = False
    ) -> str:
        """
        Execute grep command asynchronously with line numbers for pattern matching in files.

        Args:
            pattern: Regular expression pattern to search for
            file_paths: Single file path or list of file paths to search
            ignore_case: Whether to ignore case in pattern matching
            recursive: Whether to search directories recursively
            include_filename: Whether to include filename in output

        Returns:
            String containing the grep output with line numbers
        """
        # Convert single file path to list
        if isinstance(file_paths, str):
            file_paths = [file_paths]

        actual_paths = []
        invalid_paths = []
        for file_path in file_paths:
            valid, error_msg, full_path = self._validate_path(file_path)
            if valid:
                actual_paths.append(str(full_path))
            else:
                invalid_paths.append(str(file_path))

        # Base command with line numbers always enabled
        cmd = ["grep", "-n"]

        # Add optional flags
        if ignore_case:
            cmd.append("-i")

        if recursive:
            if self.allow_symlinks:
                cmd.append("-R")
            else:
                cmd.append("-r")

        # Add pattern and files
        cmd.append(pattern)
        cmd.extend(actual_paths)

        # Log the command for debugging
        self.logger.debug(f"Running grep in {self.name}, command: {' '.join(cmd)}")

        try:
            # Create subprocess
            process = await asyncio.create_subprocess_exec(
                *cmd,
                stdout=asyncio.subprocess.PIPE,
                stderr=asyncio.subprocess.PIPE
            )

            # Wait for the subprocess to complete and get stdout/stderr
            stdout_bytes, stderr_bytes = await process.communicate()
            stdout = stdout_bytes.decode('utf-8').replace("\\", "/")
            stderr = stderr_bytes.decode('utf-8')
            if process.returncode > 1:
                return f"Error executing grep: {stderr}"
            root_path = str(self.workspace_root).replace("\\", "/")
            file_map: Dict[str, List[str]] = {}
            stdout_lines = stdout.split("\n")
            for raw_line in [line.replace(root_path, f"//{self.name}")  for line in stdout_lines]:
                if len(raw_line) and ":" in raw_line:
                    try:
                        path, line, rest = raw_line.split(":", 2)
                    except ValueError as e:
                        line, rest = raw_line.split(":", 1)
                        path = f"//{self.name} single file"

                    if path not in file_map:
                        file_map[path] = []

                    file_map[path].append(f"  - {line}: {rest}")

            result: str = ""
            if len(invalid_paths):
                result += f"Invalid paths: {', '.join(invalid_paths)}\n\n"

            for path, lines in file_map.items():
                result += f"File: {path}\n"
                result += "\n".join(lines)
                result += "\n\n"

            return result
        except Exception as e:
            return f"Exception occurred: {str(e)}"

from agent_c_tools.tools.workspace.local_project import LocalProjectWorkspace  # noqa