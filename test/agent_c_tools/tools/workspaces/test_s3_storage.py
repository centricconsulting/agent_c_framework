""" Unit tests for the S3Workspace class. """

import base64
import pytest
from botocore.exceptions import ClientError
from agent_c_tools.tools.workspaces.s3_storage import S3StorageWorkspace

class TestS3StorageWorkspace:
    """
    Unit tests for the S3StorageWorkspace class.
    """

    @pytest.fixture(autouse=True)
    def aws_credentials(self, monkeypatch: pytest.MonkeyPatch):
        """Mocked AWS Credentials for mock_aws."""
        monkeypatch.setenv("AWS_ACCESS_KEY_ID", "testing")
        monkeypatch.setenv("AWS_SECRET_ACCESS_KEY", "testing")
        monkeypatch.setenv("AWS_SECURITY_TOKEN", "testing")
        monkeypatch.setenv("AWS_SESSION_TOKEN", "testing")
        monkeypatch.setenv("AWS_DEFAULT_REGION", "us-east-1")

    @pytest.fixture(autouse=True)
    def setup(self, workspace_setup):
        """Set up test fixtures."""
        self.workspace, self.mock_client = workspace_setup
        self.bucket_name = "test-bucket"
        self.prefix = "test/prefix"

    @pytest.fixture
    def workspace_setup(self, mocker):
        """Fixture to initialize workspace and mock client."""
        bucket_name = "test-bucket"
        prefix = "test/prefix"
        workspace = S3StorageWorkspace(bucket_name=bucket_name,
                                       prefix=prefix, name="test-workspace")

        # Create a mock for the aiobotocore session
        mock_session = mocker.MagicMock()
        workspace.session = mock_session

        # Set up a mock client
        mock_client = mocker.AsyncMock()
        mock_session.create_client.return_value.__aenter__.return_value = mock_client
        mock_session.create_client.return_value.__aexit__.return_value = None

        return workspace, mock_client

    def test_initialization(self):
        """Test the initialization of S3Workspace."""
        # Test with default values
        workspace = S3StorageWorkspace(bucket_name="test-bucket")
        assert workspace.bucket_name == "test-bucket"
        assert workspace.prefix == ""
        assert workspace.type_name == "s3"
        assert workspace.read_only == False
        assert workspace.max_filename_length == 1024

        # Test with custom values
        workspace = S3StorageWorkspace(
            bucket_name="test-bucket",
            prefix="my/prefix",
            name="test-workspace",
            description="Test workspace",
            read_only=True
        )
        assert workspace.bucket_name == "test-bucket"
        assert workspace.prefix == "my/prefix/"
        assert workspace.name == "test-workspace"
        assert workspace.description == "Test workspace"
        assert workspace.read_only == True
        assert workspace.max_filename_length == 1024 - len("my/prefix/")

    def test_get_full_path(self):
         """Test the _get_full_path method."""
         # Test with normal path
         assert self.workspace._get_full_path("file.txt") == "test/prefix/file.txt"

         # Test with leading slash
         assert self.workspace._get_full_path("/file.txt") == "test/prefix/file.txt"

         # Test with backslashes
         assert self.workspace._get_full_path("folder\\file.txt") == "test/prefix/folder/file.txt"

         # Test with nested path
         assert self.workspace._get_full_path("folder/file.txt") == "test/prefix/folder/file.txt"

    @pytest.mark.asyncio
    async def test_path_exists_file(self,mocker):
        """Test the path_exists method for files."""
        # Mock the head_object method for file existence
        self.mock_client.head_object = mocker.AsyncMock(return_value={"ContentLength": 100})

        # Test existing file
        assert await self.workspace.path_exists("file.txt") is True
        self.mock_client.head_object.assert_called_once_with(
            Bucket=self.bucket_name, Key="test/prefix/file.txt"
        )

        # Test non-existing file
        self.mock_client.head_object.side_effect = ClientError(
            {"Error": {"Code": "NoSuchKey", "Message": "Not found"}},
            "head_object"
        )
        assert await self.workspace.path_exists("nonexistent.txt") is False

    @pytest.mark.asyncio
    async def test_path_exists_directory(self,mocker):
        """Test the path_exists method for directories."""
        # Mock the list_objects_v2 method for directory existence
        self.mock_client.list_objects_v2 = mocker.AsyncMock(return_value={
            "Contents": [{"Key": "test/prefix/folder/file.txt"}]
        })

        # Test existing directory
        assert await self.workspace.path_exists("folder/") is True
        self.mock_client.list_objects_v2.assert_called_once_with(
            Bucket=self.bucket_name, Prefix="test/prefix/folder/", MaxKeys=1
        )

        # Test non-existing directory
        self.mock_client.list_objects_v2.return_value = {}
        assert await self.workspace.path_exists("nonexistent/") is False

    @pytest.mark.asyncio
    async def test_read_bytes_internal(self, mocker):
        """Test the read_bytes_internal method."""
        # Mock the get_object method
        mock_body = mocker.AsyncMock()
        mock_body.__aenter__.return_value.read = mocker.AsyncMock(return_value=b"test content")
        mock_body.__aexit__.return_value = None
        self.mock_client.get_object = mocker.AsyncMock(return_value={"Body": mock_body})

        # Test reading bytes
        content = await self.workspace.read_bytes_internal("file.txt")
        assert content == b"test content"
        assert self.mock_client.get_object.call_count == 1
        assert self.mock_client.get_object.call_args == mocker.call(Bucket=self.bucket_name,
                                                                    Key="test/prefix/file.txt")

        # Test file not found
        error_response = {
            "Error": {
                "Code": "NoSuchKey",
                "Message": "The specified key does not exist"
            }
        }
        self.mock_client.get_object.side_effect = ClientError(error_response,"read_bytes_internal")
        with pytest.raises(FileNotFoundError) as exc_info:
            await self.workspace.read_bytes_internal("nonexistent.txt")

        self.mock_client.get_object.side_effect = Exception("Test error")
        with pytest.raises(Exception) as exc_info:
            await self.workspace.read_bytes_internal("file.txt")

    @pytest.mark.asyncio
    async def test_read_bytes_base64(self,mocker):
        """Test the read_bytes_base64 method."""
        # Mock the read_bytes_internal method
        mock_read = mocker.patch.object(self.workspace, "read_bytes_internal",
                          mocker.AsyncMock(return_value=b"test content"))

        content = await self.workspace.read_bytes_base64("file.txt")
        assert content == base64.b64encode(b"test content").decode('utf-8')
        assert mock_read.call_count == 1
        assert mock_read.call_args == mocker.call("file.txt")

    @pytest.mark.asyncio
    async def test_write_bytes(self,mocker):
        """Test the write_bytes method."""
        # Test write mode
        await self.workspace.write_bytes("file.txt", "write", b"test content")
        assert self.mock_client.put_object.call_count == 1
        assert self.mock_client.put_object.call_args == mocker.call(
            Bucket=self.bucket_name, Key="test/prefix/file.txt", Body=b"test content"
        )

        # Test append mode - file exists
        mocker.patch.object(self.workspace, "path_exists", mocker.AsyncMock(return_value=True))
        mocker.patch.object(self.workspace, "read_bytes_internal",
                              mocker.AsyncMock(return_value=b"existing content "))

        await self.workspace.write_bytes("file.txt", "append", b"new content")
        assert self.mock_client.put_object.call_count == 2
        assert self.mock_client.put_object.call_args == mocker.call(
            Bucket=self.bucket_name, Key="test/prefix/file.txt",
            Body=b"existing content new content"
        )

        # Test append mode - file does not exist
        mocker.patch.object(self.workspace, "path_exists", mocker.AsyncMock(return_value=False))
        await self.workspace.write_bytes("file.txt", "append", b"new content")
        assert self.mock_client.put_object.call_count == 3
        assert self.mock_client.put_object.call_args == mocker.call(
            Bucket=self.bucket_name, Key="test/prefix/file.txt", Body=b"new content"
        )

        # Test invalid mode
        with pytest.raises(ValueError) as exc_info:
            await self.workspace.write_bytes("file.txt", "invalid", b"test content")
        assert "Invalid write mode: invalid" == str(exc_info.value)

        # Test read-only workspace
        self.workspace.read_only = True
        with pytest.raises(ValueError) as exc_info:
            await self.workspace.write_bytes("file.txt", "write", b"test content")
        assert "Cannot write to read-only workspace" == str(exc_info.value)
        self.workspace.read_only = False

        # Test S3 error
        self.mock_client.put_object.side_effect = Exception("Test error")
        with pytest.raises(Exception) as exc_info:
            await self.workspace.write_bytes("file.txt", "write", b"test content")
        assert "Test error" == str(exc_info.value)

    def test_full_path(self):
        """Test the full_path method."""
        assert self.workspace.full_path("file.txt") == "test/prefix/file.txt"
        assert self.workspace.full_path("folder/file.txt") == "test/prefix/folder/file.txt"

    @pytest.mark.asyncio
    async def test_ls(self,mocker):
        """Test the ls method."""

        # Define an async generator to simulate paginates async iterable behavior
        async def paginate_mock(*args, **kwargs):
            yield {
                "Contents": [
                    {"Key": "test/prefix/folder/file1.txt"},
                    {"Key": "test/prefix/folder/file2.txt"},
                    {"Key": "test/prefix/folder/subdir/"}
                ]
            }
            yield {
                "Contents": [
                    {"Key": "test/prefix/folder/file3.txt"}
                ]
            }

        # Mock get_paginator to return a paginator with the async generator
        mock_paginator = mocker.MagicMock()
        mock_paginator.paginate = paginate_mock  # Directly assign the async generator
        self.mock_client.get_paginator = mocker.MagicMock(return_value=mock_paginator)

        # Test listing files
        result = await self.workspace.ls("folder")
        assert result == "file1.txt\nfile2.txt\nfile3.txt"
        assert self.mock_client.get_paginator.call_count == 1
        assert self.mock_client.get_paginator.call_args == mocker.call("list_objects_v2")

        # Test empty folder
        async def empty_paginate_mock(*args, **kwargs):
            yield {}

        mock_paginator.paginate = empty_paginate_mock
        result = await self.workspace.ls("empty")
        assert result == ""

        # Test error case
        mock_paginator.paginate = paginate_mock
        error_response = {
            "Error": {
                "Code": "DirectoryError",
                "Message": "Error listing directory"
            }
        }
        self.mock_client.get_paginator.side_effect = ClientError(error_response, "get_paginator")
        with pytest.raises(ClientError) as exc_info:
            await self.workspace.ls("folder")
        assert error_response == exc_info.value.response

    @pytest.mark.asyncio
    async def test_read(self,mocker):
        """Test the read method."""
        # Mock the read_bytes_internal method
        mock_read = mocker.patch.object(self.workspace, "read_bytes_internal",
                          mocker.AsyncMock(return_value=b"test content"))
        content = await self.workspace.read("file.txt")
        assert content == "test content"
        assert mock_read.call_count == 1
        assert mock_read.call_args == mocker.call("file.txt")

    @pytest.mark.asyncio
    async def test_write(self,mocker):
        """Test the write method."""
        # Mock the write_bytes method
        mock_write = mocker.patch.object(self.workspace, "write_bytes",
                          mocker.AsyncMock(return_value="file.txt"))
        result = await self.workspace.write("file.txt", "write", "test content")
        assert result == "file.txt"
        assert mock_write.call_count == 1
        assert mock_write.call_args == mocker.call("file.txt", "write", b"test content")

if __name__ == "__main__":
    pytest.main()
