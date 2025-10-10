import asyncio
import logging
from typing import Optional, List, Any, Tuple

import yaml
import tempfile

from yaml import FullLoader

from agent_c.util.uncish_path import UNCishPath


class BaseWorkspace:
    """
    This is a base class for workspace representations.

    Attributes:
        name (str): The name of the workspace, as provided by kwargs.
        description (str): The description of the workspace, if provided.
        type_name (str): The type name of the workspace.
        read_only (bool): A flag indicating whether the workspace is read-only.
        write_status (str): A textual representation of the read/write status.
        max_filename_length (int): The maximum length of filenames in the workspace.
                                  A value of -1 indicates no specific limit.
    """

    supports_run_command: bool = False

    def __init__(self, type_name: str, **kwargs):
        """
        The initializer for the BaseWorkspace class.

        Args:
            type_name (str): The type name of the workspace.
            **kwargs: Keyword arguments for the workspace properties.
                      - 'name' (str): The name of the workspace.
                      - 'description' (str): The description of the workspace.
                      - 'read_only' (bool): If the workspace should be read-only.
        """
        self.name: str = kwargs.get('name','').lower()
        self.meta_file_path: str = kwargs.get('meta_file_path', '.agent_c.meta.yaml')
        self.description: Optional[str] = kwargs.get('description')
        self.type_name: str = type_name
        self.read_only: bool = kwargs.get('read_only', False)
        self.write_status: str = "RO" if self.read_only else "R/W"
        self.max_filename_length: int = -1
        self._metadata: Optional[dict[str, Any]] = None
        self._metadata_lock: asyncio.Lock = asyncio.Lock()
        self.logger = kwargs.get("logger", logging.getLogger(__name__))

    def __str__(self) -> str:
        """
        String representation of the BaseWorkspace instance.

        Returns:
            str: A formatted string summary of the workspace.
        """
        return f"- workspace_name: `{self.name}`, rw_status: ({self.write_status}), workspace_type: \"{self.type_name}\", description: {self.description}"

    async def path_exists(self, file_path: str) -> bool:
        """
        Abstract method to check if a path exists within the workspace.

        Args:
            file_path (str): The path to check for existence.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def tree(self, relative_path: str, folder_depth: int, file_depth: int) -> str:
        raise NotImplementedError

    async def walk(self, start_path: str, extensions: List[str] = None) -> Tuple[Optional[str], List[str]]:
        raise NotImplementedError

    async def local_path_for(self, file_path: str) -> str:
        """
        Base class method to get the local path for a file within the workspace.
        Will read the file and write it to a temp file, returning the path to that file.
        """
        unc_path: UNCishPath = UNCishPath(file_path)
        file_bytes= await self.read_bytes_internal(file_path)
        if file_bytes is None:
            raise FileNotFoundError(f"File not found in workspace: {file_path}")

        with tempfile.NamedTemporaryFile(delete=False, suffix=f".{unc_path.path_parts[-1]}") as temp_file:
            temp_file.write(file_bytes)
            temp_file_path = temp_file.name

        return temp_file_path

    async def read_bytes_internal(self, file_path: str) -> bytes:
        """
        Abstract method to read bytes directly from a path within the workspace.

        Args:
            file_path (str): The path from which to read bytes.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def read_bytes_base64(self, file_path: str) -> str:
        """
        Abstract method to read bytes and encode them base64 from a path within the workspace.

        Args:
            file_path (str): The path from which to read bytes.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def write_bytes(self, file_path: str, mode: str, data: bytes) -> str:
        """
        Abstract method to write bytes to a path within the workspace.

        Args:
            file_path (str): The path where to write bytes.
            mode (str): The mode in which to write the data, can be "write" or "append".
            data (bytes): The data to write into the file.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    def full_path(self, path: str, mkdirs: bool = True) -> Optional[str]:
        """
        Method to generate the full path for a given path in the workspace.

        Args:
            path (str): The directory or file path.
            mkdirs (bool): Whether to create directories along the path if they do not exist.

        Returns:
            Optional[str]: The full path or None if the path is not within the workspace.
        """
        return None

    async def ls(self, path: str) -> str:
        """
        Abstract method to list all files in a directory within the workspace.

        Args:
            path (str): The directory path to list files from.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def read(self, path: str) -> str:
        """
        Abstract method to read text from a path within the workspace.

        Args:
            path (str): The path from which to read text.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def read_internal(self, file_path: str, encoding: Optional[str] = "utf-8") -> str:
        """
        Abstract method to read text from a path within the workspace.
        Used internally for reading files, raises exceptions instead of returning error json.

        Args:
            path (str): The path from which to read text.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def write(self, path: str, mode: str, data: str) -> str:
        """
        Abstract method to write text to a path within the workspace.

        Args:
            path (str): The path where to write text.
            mode (str): The mode in which to open the file.
            data (str): The data to write into the file.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def cp(self, src_path: str, dest_path: str) -> str:
        """
        Abstract method to copy a file from one path to another within the workspace.

        Args:
            src_path (str): The source path of the file to copy.
            det_path (str): The destination path where the file should be copied.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def mv(self, src_path: str, dest_path: str) -> str:
        """
        Abstract method to move a file from one path to another within the workspace.

        Args:
            src_path (str): The source path of the file to move.
            dest_path (str): The destination path where the file should be moved.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError

    async def is_directory(self, path: str) -> bool:
        """
        Abstract method to check if a given path is a directory.

        Args:
            path (str): The path to check.

        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError
        
    async def glob(self, pattern: str, recursive: bool = False, include_hidden: bool = False) -> List[str]:
        """
        Abstract method to find paths matching the specified pattern.
        
        Args:
            pattern (str): The glob pattern to match against paths in the workspace.
            recursive (bool): Whether to search recursively, matching ** patterns.
            include_hidden (bool): Whether to include hidden files in ** pattern matching.
            
        Returns:
            List[str]: A list of relative paths that match the pattern.
            
        Raises:
            NotImplementedError: This method should be implemented by subclasses.
        """
        raise NotImplementedError("glob method must be implemented")

    async def safe_metadata(self, key: str) -> Any:
        async with self._metadata_lock:
            if self._metadata is None:
                await self.load_metadata()

            return self.metadata(key)

    def metadata(self, key: str, include_hidden: bool = False) -> Any:
        """
        Property to access the metadata of the workspace.
        Args:
            key: The key to access the metadata.
            include_hidden: If True, include metadata keys that start with '_'.
        Returns:
             Any: The metadata value or and empty dict.
        """
        value = self._metadata
        if len(key) != 0 and key != '/' and key != 'meta':
            key_parts = key.removeprefix('/').removeprefix('meta/').split('/')
            if len(key_parts):
                for part in key_parts:
                    if isinstance(value, dict) and part in value:
                        value = value[part]
                    else:
                        value = None

        # create a new dictionary without keys that start with '_'
        if not include_hidden and isinstance(value, dict):
            return  {k: v for k, v in value.items() if not k.startswith('_')}

        return value

    async def safe_metadata_write(self, key: str, value: any, auto_save: bool = True) -> Any:
        """Write metadata with thread safety. By default, automatically saves to disk."""
        async with self._metadata_lock:
            if self._metadata is None:
                await self.load_metadata()

            result = self.metadata_write(key, value)
            
            if auto_save:
                await self._save_metadata_internal()
            
            return result
    
    async def _save_metadata_internal(self, meta_file_path: Optional[str] = None):
        """Internal save method that assumes the lock is already held."""
        try:
            if meta_file_path is None:
                meta_file_path = self.meta_file_path

            yaml_content = yaml.dump(self._metadata, default_flow_style=False, allow_unicode=True)
            await self.write(meta_file_path, "write", yaml_content)
        except Exception as e:
            self.logger.exception(f"Failed to save metadata to //{self.name}/{meta_file_path}: {e}", exc_info=True)
            raise e

    def metadata_write(self, key: str, value: Any) -> Any:
        """
        Property to access the metadata of the workspace.

        Returns:
            Optional[dict[str, Any]]: The metadata dictionary or None if not set.
        """
        key_parts = key.removeprefix('meta/').split('/')
        current = self._metadata
        if len(key_parts) > 0:
            for i, part in enumerate(key_parts[:-1]):
                if part not in current or not isinstance(current[part], dict):
                    current[part] = {}

                current = current[part]

            # Set the value at the final key
            current[key_parts[-1]] = value

        return value

    async def save_metadata(self, meta_file_path: Optional[str] = None):
        """Save metadata to file with thread safety."""
        async with self._metadata_lock:
            await self._save_metadata_internal(meta_file_path)

    async def load_metadata(self, meta_file_path: Optional[str] = None):
        """Load metadata from file. Note: When called from safe_metadata/safe_metadata_write, the lock is already held."""
        try:
            if meta_file_path is None:
                meta_file_path = self.meta_file_path

            if not await self.path_exists(meta_file_path):
                if self._metadata is None:
                    self._metadata = {}
            else:
                content = await self.read_internal(meta_file_path)
                self._metadata = yaml.load(content, FullLoader) or {}
        except Exception as e:
            self.logger.exception(f"Failed to load metadata from //{self.name}/{meta_file_path}: {e}", exc_info=True)

        return self._metadata

    async def run_command(self, command: str,
                          working_directory: Optional[str] = None,
                          timeout: Optional[int] = None,
                          suppress_success_output: Optional[bool] = None):
        """
        Override in subclasses that support commands.
        Having this on the base class satisfies static type checkers/IDEs.
        """
        raise NotImplementedError(f"{self.__class__.__name__} does not support run_command")
