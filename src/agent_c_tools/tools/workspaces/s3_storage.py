""" Amazon S3 Workspace storage class """

import logging
import base64
import json
from aiobotocore.session import get_session
from botocore.exceptions import NoCredentialsError, ClientError
from agent_c_tools.tools.workspaces.base import BaseWorkspace


class S3StorageWorkspace(BaseWorkspace):
    """
    A workspace implementation for Amazon S3.
    
    This class provides methods for interacting with files stored in Amazon S3
    using aiobotocore for asynchronous operations.
    
    Attributes:
        bucket_name (str): The name of the S3 bucket.
        prefix (str): The prefix (folder path) within the bucket.
        session (aiobotocore.session.AioSession): The aiobotocore session.
    """

    def __init__(self, bucket_name: str, prefix: str = "", **kwargs):
        """
        Initialize the S3StorageWorkspace.
        
        Args:
            bucket_name (str): The name of the S3 bucket.
            prefix (str): Optional prefix (folder path) within the bucket.
            **kwargs: Additional keyword arguments for BaseWorkspace.
        """
        super().__init__(type_name="s3", **kwargs)
        self.bucket_name = bucket_name
        self.prefix = prefix.rstrip('/') + '/' if prefix else ""
        self.session = get_session()

        # S3 has a maximum object key length of 1024 bytes
        self.max_filename_length = 1024 - len(self.prefix)

    def _get_full_path(self, file_path: str) -> str:
        """
        Generate the full S3 key for a given file path.
        
        Args:
            file_path (str): The file path relative to the workspace.
            
        Returns:
            str: The full S3 key.
        """
        # Normalize the path to use forward slashes and remove leading slashes
        normalized_path = file_path.replace('\\', '/').lstrip('/')
        return f"{self.prefix}{normalized_path}"

    async def path_exists(self, file_path: str) -> bool:
        """
        Check if a path exists within the S3 bucket.
        
        Args:
            file_path (str): The path to check for existence.
            
        Returns:
            bool: True if the path exists, False otherwise.
        """
        full_path = self._get_full_path(file_path)
        async with self.session.create_client('s3') as client:
            try:
                # For directories, check if there are objects with the prefix
                if not full_path.endswith('/'):
                    # Check if it's a file
                    await client.head_object(Bucket=self.bucket_name, Key=full_path)
                    return True
                else:
                    # Check if it's a directory by listing objects with this prefix
                    response = await client.list_objects_v2(
                        Bucket=self.bucket_name,
                        Prefix=full_path,
                        MaxKeys=1
                    )
                    return 'Contents' in response
            except ClientError as e:
                logging.error(e)
                return False

    async def read_bytes_internal(self, file_path: str) -> bytes:
        """
        Read bytes directly from a path within the S3 bucket.
        
        Args:
            file_path (str): The path from which to read bytes.
            
        Returns:
            bytes: The content of the file as bytes.
            
        Raises:
            FileNotFoundError: If the file does not exist.
            Exception: For other S3 errors.
        """
        full_path = self._get_full_path(file_path)
        async with self.session.create_client('s3') as client:
            try:
                response = await client.get_object(Bucket=self.bucket_name, Key=full_path)
                async with response['Body'] as stream:
                    return await stream.read()
            except ClientError as clientError:
                logging.error(clientError)
                if clientError.response['Error']['Code'] == 'NoSuchKey':
                    raise FileNotFoundError(f"File not found: {file_path}") from clientError
                else:
                    raise clientError
            #except client.exceptions.NoSuchKey as exc:
            #    raise FileNotFoundError(f"File not found: {file_path}") from exc
            except Exception as e:
                logging.error(e)
                raise ClientError(f"Error reading file {file_path}: {str(e)}",
                                  "read_bytes_internal") from e

    async def read_bytes_base64(self, file_path: str) -> str:
        """
        Read bytes and encode them as base64 from a path within the S3 bucket.
        
        Args:
            file_path (str): The path from which to read bytes.
            
        Returns:
            str: The content of the file as a base64 encoded string.
        """
        content = await self.read_bytes_internal(file_path)
        return base64.b64encode(content).decode('utf-8')

    async def write_bytes(self, file_path: str, mode: str, data: bytes) -> str:
        """
        Write bytes to a path within the S3 bucket.
        
        Args:
            file_path (str): The path where to write bytes.
            mode (str): The mode in which to write the data, can be "write" or "append".
            data (bytes): The data to write into the file.
            
        Returns:
            str: The file path where the data was written.
            
        Raises:
            ValueError: If the workspace is read-only or if the mode is invalid.
            Exception: For S3 errors.
        """
        if self.read_only:
            raise ValueError("Cannot write to read-only workspace")

        if mode not in ["write", "append"]:
            raise ValueError(f"Invalid write mode: {mode}")

        full_path = self._get_full_path(file_path)

        async with self.session.create_client('s3') as client:
            if mode == "append" and await self.path_exists(file_path):
                # For append mode, we need to read existing content first
                existing_data = await self.read_bytes_internal(file_path)
                data = existing_data + data

            try:
                await client.put_object(
                    Bucket=self.bucket_name,
                    Key=full_path,
                    Body=data
                )
                return file_path
            except NoCredentialsError:
                logging.error("No AWS credentials found")
                return json.dumps({'error': 'No AWS credentials found'})
            except Exception as e:
                logging.error(e)
                raise e

    def full_path(self, path: str, mkdirs: bool = True) -> str:
        """
        Generate the full path for a given path in the workspace.
        
        In S3, directories don't need to be explicitly created, so mkdirs is ignored.
        
        Args:
            path (str): The directory or file path.
            mkdirs (bool): Whether to create directories (ignored in S3).
            
        Returns:
            str: The full path within the S3 bucket.
        """
        return self._get_full_path(path)

    async def ls(self, path: str) -> str:
        """
        List all files in a directory within the S3 bucket.
        
        Args:
            path (str): The directory path to list files from.
            
        Returns:
            str: A newline-separated list of file paths.
        """
        full_path = self._get_full_path(path.rstrip('/') + '/')

        async with self.session.create_client('s3') as client:
            try:
                paginator = client.get_paginator('list_objects_v2')
                files = []

                async for page in paginator.paginate(Bucket=self.bucket_name, Prefix=full_path):
                    if 'Contents' in page:
                        for obj in page['Contents']:
                            # Remove the prefix to get the relative path
                            rel_path = obj['Key'][len(full_path):]
                            if rel_path and not rel_path.endswith('/'):
                                files.append(rel_path)

                return '\n'.join(sorted(files))
            except ClientError as clientError:
                logging.error(clientError)
                raise
            except Exception as e:
                logging.error(e)
                raise e

    async def read(self, path: str) -> str:
        """
        Read text from a path within the S3 bucket.
        
        Args:
            path (str): The path from which to read text.
            
        Returns:
            str: The content of the file as a string.
        """
        content = await self.read_bytes_internal(path)
        return content.decode('utf-8')

    async def write(self, path: str, mode: str, data: str) -> str:
        """
        Write text to a path within the S3 bucket.
        
        Args:
            path (str): The path where to write text.
            mode (str): The mode in which to open the file.
            data (str): The data to write into the file.
            
        Returns:
            str: The file path where the data was written.
        """
        return await self.write_bytes(path, mode, data.encode('utf-8'))
