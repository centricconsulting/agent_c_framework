""" Azure Blob Storage Workspace """

from io import StringIO
import os
import base64
import logging
from typing import Optional
from azure.storage.blob.aio import BlobServiceClient as AsyncBlobServiceClient
from azure.storage.blob.aio import BlobClient as AsyncBlobClient
from azure.storage.blob.aio import ContainerClient as AsyncContainerClient
from azure.core.exceptions import ResourceNotFoundError, ResourceExistsError, AzureError
from agent_c_tools.tools.workspace.base import BaseWorkspace


class BlobStorageWorkspace(BaseWorkspace):
    """
    A workspace implementation that uses Azure Blob Storage.

    This class provides asynchronous methods to read, write, and manage files
    in an Azure Blob Storage container. Authentication is handled through
    environment variables.

    Environment Variables:
        AZURE_STORAGE_CONNECTION_STRING: Connection string for the Azure Storage account
        AZURE_STORAGE_ACCOUNT_NAME: Storage account name (used if connection string not provided)
        AZURE_STORAGE_ACCOUNT_KEY: Storage account key (used with account name)
        AZURE_STORAGE_SAS_TOKEN: SAS token (can be used instead of account key)

    Attributes:
        container_name (str): The name of the Azure Blob container to use
        prefix (str, optional): A prefix to prepend to all blob paths
    """

    def __init__(self, container_name: str, prefix: str = "", **kwargs):
        """
        Initialize an Azure Blob Storage workspace.

        Args:
            container_name (str): The name of the Azure Blob container to use
            prefix (str, optional): A prefix to prepend to all blob paths
            **kwargs: Additional arguments to pass to BaseWorkspace
                - name (str): The name of the workspace
                - description (str): The description of the workspace
                - read_only (bool): If the workspace should be read-only
        """
        super().__init__(type_name="azure_blob", **kwargs)
        self.container_name = container_name
        self.prefix = prefix.rstrip("/") + "/" if prefix else ""

        # Get Azure credentials from environment variables
        self.connection_string = os.environ.get(
            "AZURE_STORAGE_CONNECTION_STRING")
        self.account_name = os.environ.get("AZURE_STORAGE_ACCOUNT_NAME")
        self.account_key = os.environ.get("AZURE_STORAGE_ACCOUNT_KEY")
        self.sas_token = os.environ.get("AZURE_STORAGE_SAS_TOKEN")

        if not (self.connection_string or
                (self.account_name
                 and (self.account_key or self.sas_token)
                 )
                ):
            raise ValueError(
                "Azure Storage credentials not found in environment variables. "
                "Please set either AZURE_STORAGE_CONNECTION_STRING or "
                "AZURE_STORAGE_ACCOUNT_NAME with either AZURE_STORAGE_ACCOUNT_KEY or "
                "AZURE_STORAGE_SAS_TOKEN."
            )

        # Set reasonable filename length limit for blob storage
        self.max_filename_length = 1024 - len(self.prefix)

        # Setup logger
        self.logger = logging.getLogger(__name__)

    async def _get_service_client(self) -> AsyncBlobServiceClient:
        """
        Get an asynchronous Azure Blob Service client.

        Returns:
            AsyncBlobServiceClient: The authenticated blob service client
        """
        if self.connection_string:
            return AsyncBlobServiceClient.from_connection_string(self.connection_string)
        elif self.account_name and self.account_key:
            url = f"https://{self.account_name}.blob.core.windows.net"
            return AsyncBlobServiceClient(account_url=url, credential=self.account_key)
        elif self.account_name and self.sas_token:
            url = f"https://{self.account_name}.blob.core.windows.net{self.sas_token}"
            return AsyncBlobServiceClient(account_url=url)
        else:
            raise ValueError(
                "Azure Storage credentials not properly configured")

    async def _get_container_client(self) -> AsyncContainerClient:
        """
        Get an asynchronous container client for the configured container.

        Returns:
            AsyncContainerClient: The container client
        """
        service_client = await self._get_service_client()
        container_client = service_client.get_container_client(
            self.container_name)

        # Ensure the container exists
        try:
            if not container_client.exists():
                await container_client.create_container(exist_ok=True)
        except ResourceExistsError:
            pass
        except AzureError as e:
            self.logger.warning("Could not create container: %s", e)

        return container_client

    async def _get_blob_client(self, blob_path: str) -> AsyncBlobClient:
        """
        Get an asynchronous blob client for a specific blob.

        Args:
            blob_path (str): The path to the blob

        Returns:
            AsyncBlobClient: The blob client
        """
        service_client = await self._get_service_client()
        return service_client.get_blob_client(
            container=self.container_name,
            blob=self._normalize_path(blob_path)
        )

    def _normalize_path(self, path: str) -> str:
        """
        Normalize a path by adding the prefix and removing leading slashes.

        Args:
            path (str): The path to normalize

        Returns:
            str: The normalized path
        """
        # Remove leading slashes and add the prefix
        return self.prefix + path.lstrip("/")

    async def path_exists(self, file_path: str) -> bool:
        """
        Check if a path exists within the workspace.

        Args:
            file_path (str): The path to check for existence

        Returns:
            bool: True if the path exists, False otherwise
        """
        blob_client = await self._get_blob_client(file_path)
        try:
            await blob_client.get_blob_properties()
            return True
        except ResourceNotFoundError:
            return False
        except AzureError as e:
            self.logger.error("Error checking path existence: %s", e)
            return False

    async def tree(self, relative_path: str = "") -> str:
        """
        Get a tree representation of files starting from a path.

        Args:
            relative_path (str): The starting path for the tree

        Returns:
            str: A formatted string showing the directory tree
        """
        container_client = await self._get_container_client()
        prefix = self._normalize_path(relative_path)

        # List all blobs with the prefix
        blob_list = []
        async for blob in container_client.list_blobs(name_starts_with=prefix):
            # Remove the workspace prefix to get the relative path
            rel_path = blob.name
            if self.prefix and rel_path.startswith(self.prefix):
                rel_path = rel_path[len(self.prefix):]
            blob_list.append(rel_path)

        # Build the tree representation
        result = StringIO()
        result.write(f"Files in {relative_path or '/'}\n")

        for i, path in enumerate(sorted(blob_list)):
            is_last = i == len(blob_list) - 1
            result.write(f"{'└── ' if is_last else '├── '}{path}\n")

        return result.getvalue()

    async def read_bytes_internal(self, file_path: str) -> bytes:
        """
        Read bytes directly from a path within the workspace.

        Args:
            file_path (str): The path from which to read bytes

        Returns:
            bytes: The content of the file as bytes

        Raises:
            FileNotFoundError: If the blob does not exist
        """
        blob_client = await self._get_blob_client(file_path)
        try:
            download_stream = await blob_client.download_blob()
            data = await download_stream.readall()
            return data
        except ResourceNotFoundError as rnfe:
            raise FileNotFoundError(
                f"File {file_path} not found in Azure Blob Storage") from rnfe
        except Exception as e:
            self.logger.error("Error reading bytes from %s: %s", file_path, e)
            raise

    async def read_bytes_base64(self, file_path: str) -> str:
        """
        Read bytes and encode them as base64 from a path within the workspace.

        Args:
            file_path (str): The path from which to read bytes

        Returns:
            str: The base64-encoded content of the file

        Raises:
            FileNotFoundError: If the blob does not exist
        """
        data = await self.read_bytes_internal(file_path)
        return base64.b64encode(data).decode('utf-8')

    async def write_bytes(self, file_path: str, mode: str, data: bytes) -> str:
        """
        Write bytes to a path within the workspace.

        Args:
            file_path (str): The path where to write bytes
            mode (str): The mode in which to write the data, can be "write" or "append"
            data (bytes): The data to write into the file

        Returns:
            str: A message indicating the result of the operation

        Raises:
            ValueError: If the workspace is read-only or the mode is invalid
        """
        if self.read_only:
            raise ValueError("Cannot write to a read-only workspace")

        if mode not in ["write", "append"]:
            raise ValueError(
                f"Invalid mode: {mode}. Must be 'write' or 'append'")

        blob_client = await self._get_blob_client(file_path)

        try:
            if mode == "append":
                # For append mode, we need to download existing data, append to it,
                # and then upload the combined result
                try:
                    existing_data = await self.read_bytes_internal(file_path)
                    data = existing_data + data
                except FileNotFoundError:
                    # File doesn't exist yet, continue with just the new data
                    pass

            await blob_client.upload_blob(data, overwrite=True)
            return f"Successfully wrote {len(data)} bytes to {file_path}"
        except Exception as e:
            self.logger.error("Error writing bytes to %s: %s", file_path, e)
            raise

    def full_path(self, path: str, mkdirs: bool = True) -> Optional[str]:
        """
        Generate the full path for a given path in the workspace.

        Note: In Azure Blob Storage, we don't need to create directories
        since it uses a flat namespace. This method normalizes the path
        and returns it.

        Args:
            path (str): The directory or file path
            mkdirs (bool): Not applicable for Azure Blob Storage

        Returns:
            Optional[str]: The normalized path
        """
        return self._normalize_path(path)

    async def ls(self, path: str) -> str:
        """
        List all files in a directory within the workspace.

        Args:
            path (str): The directory path to list files from

        Returns:
            str: A formatted string listing the files
        """
        container_client = await self._get_container_client()
        prefix = self._normalize_path(path)

        if not prefix.endswith('/') and prefix:
            prefix += '/'

        # List all blobs with the prefix
        result = StringIO()

        # Track unique "directory" names at the current level
        files = []

        async for blob in container_client.list_blobs(name_starts_with=prefix):
            # Remove the current prefix to get relative path
            rel_path = blob.name[len(prefix):] if prefix else blob.name

            # Skip empty paths
            if not rel_path:
                continue

            files.append(rel_path)

        for file_name in sorted(files):
            result.write(f"f {file_name}\n")

        return result.getvalue()

    async def read(self, path: str) -> str:
        """
        Read text from a path within the workspace.

        Args:
            path (str): The path from which to read text

        Returns:
            str: The content of the file as text

        Raises:
            FileNotFoundError: If the blob does not exist
        """
        try:
            data = await self.read_bytes_internal(path)
            return data.decode('utf-8')
        except UnicodeDecodeError as ude:
            raise ValueError(
                f"File {path} contains non-UTF-8 data. "
                "Use read_bytes_internal for binary data.") from ude

    async def write(self, path: str, mode: str, data: str) -> str:
        """
        Write text to a path within the workspace.

        Args:
            path (str): The path where to write text
            mode (str): The mode in which to write the data, can be "write" or "append"
            data (str): The data to write into the file

        Returns:
            str: A message indicating the result of the operation
        """
        return await self.write_bytes(path, mode, data.encode('utf-8'))

    async def cp(self, src_path: str, dest_path: str) -> str:
        """
        Copy a file from one path to another within the workspace.

        Args:
            src_path (str): The source path
            dest_path (str): The destination path

        Returns:
            str: A message indicating the result of the operation

        Raises:
            ValueError: If the workspace is read-only
            FileNotFoundError: If the source file does not exist
        """
        if self.read_only:
            raise ValueError("Cannot copy files in a read-only workspace")

        # Check if source exists
        if not await self.path_exists(src_path):
            raise FileNotFoundError(f"Source file {src_path} not found")

        # Get source and destination blob clients
        src_blob_client = await self._get_blob_client(src_path)
        dest_blob_client = await self._get_blob_client(dest_path)

        # Get the source URL with SAS token if applicable
        source_url = src_blob_client.url
        if self.sas_token and '?' not in source_url:
            source_url += self.sas_token

        # Perform the copy operation
        await dest_blob_client.start_copy_from_url(source_url)
        return f"Successfully copied {src_path} to {dest_path}"

    async def mv(self, src_path: str, dest_path: str) -> str:
        """
        Move a file from one path to another within the workspace.

        Args:
            src_path (str): The source path
            dest_path (str): The destination path

        Returns:
            str: A message indicating the result of the operation

        Raises:
            ValueError: If the workspace is read-only
            FileNotFoundError: If the source file does not exist
        """
        if self.read_only:
            raise ValueError("Cannot move files in a read-only workspace")

        # Copy the file first
        await self.cp(src_path, dest_path)

        # Then delete the source
        src_blob_client = await self._get_blob_client(src_path)
        await src_blob_client.delete_blob()

        return f"Successfully moved {src_path} to {dest_path}"
