""" Unit tests for the BlobStorageWorkspace class. """

import base64
import pytest
from agent_c_tools.tools.workspace.blob_storage import BlobStorageWorkspace
from azure.core.exceptions import ResourceNotFoundError
from azure.storage.blob.aio import ContainerClient as AsyncContainerClient


@pytest.mark.unit
class TestBlobStorageWorkspace:
    """
    Unit tests for the BlobStorageWorkspace class.
    """

    class MockBlob:
        """Mock blob class to simulate Azure Blob objects"""

        def __init__(self, name):
            self.name = name

    class MockAsyncIterator:
        """Custom async iterator to mock the AsyncItemPaged returned by list_blobs"""

        def __init__(self, items):
            self.items = items
            self.index = 0

        def __aiter__(self):
            return self

        async def __anext__(self):
            if self.index >= len(self.items):
                raise StopAsyncIteration
            item = self.items[self.index]
            self.index += 1
            return item

    @pytest.fixture(autouse=True)
    def azure_credentials(self, monkeypatch: pytest.MonkeyPatch):
        """Set up environment variables for testing"""
        monkeypatch.setenv(
            "AZURE_STORAGE_CONNECTION_STRING",
            "DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;"
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq"
            "2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;"
            "BlobEndpoint=https://devstoreaccount1.blob.core.windows.net/;"
        )
        monkeypatch.setenv("AZURE_STORAGE_ACCOUNT_NAME", "devstoreaccount1")
        monkeypatch.setenv(
            "AZURE_STORAGE_ACCOUNT_KEY",
            "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/"
            "K1SZFPTOtr/KBHBeksoGMGw=="
        )

    @pytest.fixture
    def workspace_setup(self, mocker):
        """Fixture to initialize workspace and mock clients."""
        container_name = "test-container"
        prefix = "test/prefix"
        workspace = BlobStorageWorkspace(
            container_name=container_name, prefix=prefix)

        # Mock the service client and container client
        mock_service_client = mocker.AsyncMock()
        mock_container_client = mocker.MagicMock(spec=AsyncContainerClient)
        workspace._get_service_client = mocker.AsyncMock(
            return_value=mock_service_client)
        mock_service_client.get_container_client.return_value = mock_container_client

        return workspace, mock_service_client, mock_container_client

    def test_initialization(self):
        """Test the initialization of BlobStorageWorkspace."""
        # Test with default values
        workspace = BlobStorageWorkspace(container_name="test-container")
        assert workspace.container_name == "test-container"
        assert workspace.prefix == ""
        assert workspace.type_name == "azure_blob"
        assert workspace.read_only is False
        assert workspace.max_filename_length == 1024

        # Test with custom values
        workspace = BlobStorageWorkspace(
            container_name="test-container",
            prefix="my/prefix",
            name="test-workspace",
            description="Test workspace",
            read_only=True
        )
        assert workspace.container_name == "test-container"
        assert workspace.prefix == "my/prefix/"
        assert workspace.name == "test-workspace"
        assert workspace.description == "Test workspace"
        assert workspace.read_only is True
        assert workspace.max_filename_length == 1024 - len("my/prefix/")

    def test_normalize_path(self, workspace_setup):
        """Test the _normalize_path method."""
        workspace, _, _ = workspace_setup
        assert workspace._normalize_path("file.txt") == "test/prefix/file.txt"
        assert workspace._normalize_path("/file.txt") == "test/prefix/file.txt"

    @pytest.mark.asyncio
    async def test_path_exists(self, workspace_setup, mocker):
        """Test the path_exists method."""
        workspace, _, _ = workspace_setup
        mock_blob_client = mocker.AsyncMock()
        workspace._get_blob_client = mocker.AsyncMock(
            return_value=mock_blob_client)

        # Test existing path
        mock_blob_client.get_blob_properties = mocker.AsyncMock()
        assert await workspace.path_exists("file.txt") is True

        # Test non-existing path
        mock_blob_client.get_blob_properties.side_effect = ResourceNotFoundError
        assert await workspace.path_exists("nonexistent.txt") is False

    @pytest.mark.asyncio
    async def test_read_bytes_internal(self, workspace_setup, mocker):
        """Test the read_bytes_internal method."""
        workspace, _, _ = workspace_setup
        mock_blob_client = mocker.AsyncMock()
        workspace._get_blob_client = mocker.AsyncMock(
            return_value=mock_blob_client)

        # Test reading bytes
        mock_blob_client.download_blob.return_value.readall = mocker.AsyncMock(
            return_value=b"test content")
        content = await workspace.read_bytes_internal("file.txt")
        assert content == b"test content"

        # Test file not found
        mock_blob_client.download_blob.return_value.readall.side_effect = ResourceNotFoundError
        with pytest.raises(FileNotFoundError):
            await workspace.read_bytes_internal("nonexistent.txt")

    @pytest.mark.asyncio
    async def test_read_bytes_base64(self, workspace_setup, mocker):
        """Test the read_bytes_base64 method."""
        workspace, _, _ = workspace_setup
        mock_read = mocker.patch.object(
            workspace, "read_bytes_internal", mocker.AsyncMock(return_value=b"test content"))

        content = await workspace.read_bytes_base64("file.txt")
        assert content == base64.b64encode(b"test content").decode('utf-8')
        mock_read.assert_called_once_with("file.txt")

    @pytest.mark.asyncio
    async def test_write_bytes(self, workspace_setup, mocker):
        """Test the write_bytes method."""
        workspace, _, _ = workspace_setup
        mock_blob_client = mocker.AsyncMock()
        workspace._get_blob_client = mocker.AsyncMock(
            return_value=mock_blob_client)

        # Test write mode
        await workspace.write_bytes("file.txt", "write", b"test content")
        mock_blob_client.upload_blob.assert_called_once_with(
            b"test content", overwrite=True)

        # Test append mode
        mocker.patch.object(workspace, "read_bytes_internal",
                            mocker.AsyncMock(return_value=b"existing content "))
        await workspace.write_bytes("file.txt", "append", b"new content")
        mock_blob_client.upload_blob.assert_called_with(
            b"existing content new content", overwrite=True)

        # Test invalid mode
        with pytest.raises(ValueError, match="Invalid mode: invalid. Must be 'write' or 'append'"):
            await workspace.write_bytes("file.txt", "invalid", b"test content")

        # Test read-only workspace
        workspace.read_only = True
        with pytest.raises(ValueError, match="Cannot write to a read-only workspace"):
            await workspace.write_bytes("file.txt", "write", b"test content")

    @pytest.mark.asyncio
    async def test_ls(self, workspace_setup, mocker):
        """Test the ls method."""
        workspace, _, mock_container_client = workspace_setup

        mock_blobs = [
            self.MockBlob("test/prefix/file1.txt"),
            self.MockBlob("test/prefix/file2.txt"),
            self.MockBlob("test/prefix/folder/")
        ]

        mock_container_client.list_blobs = mocker.MagicMock(
            return_value=self.MockAsyncIterator(mock_blobs))

        workspace._get_container_client = mocker.AsyncMock(
            return_value=mock_container_client)

        result = await workspace.ls("")
        assert "file1.txt" in result
        assert "file2.txt" in result
        assert "folder/" in result

    @pytest.mark.asyncio
    async def test_cp(self, workspace_setup, mocker):
        """Test the cp method."""
        workspace, _, _ = workspace_setup

        mock_src_blob_client = mocker.AsyncMock()
        mock_dest_blob_client = mocker.AsyncMock()
        mocker.patch.object(workspace, "_get_blob_client", side_effect=[
                            mock_src_blob_client, mock_dest_blob_client])
        mocker.patch.object(workspace, "path_exists", return_value=True)

        result = await workspace.cp("source/path", "dest/path")
        assert result == "Successfully copied source/path to dest/path"
        mock_dest_blob_client.start_copy_from_url.assert_called_once_with(
            mock_src_blob_client.url)

        # Test source file not found
        mocker.patch.object(workspace, "path_exists", return_value=False)
        with pytest.raises(FileNotFoundError, match="Source file source/path not found"):
            await workspace.cp("source/path", "dest/path")

    @pytest.mark.asyncio
    async def test_mv(self, workspace_setup, mocker):
        """Test the mv method."""
        workspace, _, _ = workspace_setup

        mock_cp = mocker.patch.object(
            workspace, "cp", return_value="Successfully copied")
        mock_src_blob_client = mocker.AsyncMock()
        mocker.patch.object(workspace, "_get_blob_client",
                            return_value=mock_src_blob_client)

        result = await workspace.mv("source/path", "dest/path")
        assert result == "Successfully moved source/path to dest/path"
        mock_cp.assert_called_once_with("source/path", "dest/path")
        mock_src_blob_client.delete_blob.assert_called_once()

        # Test read-only workspace
        workspace.read_only = True
        with pytest.raises(ValueError, match="Cannot move files in a read-only workspace"):
            await workspace.mv("source/path", "dest/path")
