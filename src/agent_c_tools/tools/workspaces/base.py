from typing import Optional


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
        self.name: Optional[str] = kwargs.get('name')
        self.description: Optional[str] = kwargs.get('description')
        self.type_name: str = type_name
        self.read_only: bool = kwargs.get('read_only', False)
        self.write_status: str = "read only" if self.read_only else "read write"
        self.max_filename_length: int = -1

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
