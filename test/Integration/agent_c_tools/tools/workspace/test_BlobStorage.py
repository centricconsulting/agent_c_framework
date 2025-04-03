"""Integration tests for BlobStorageWorkspace."""

import os
import pytest
import pytest_asyncio
from azure.storage.blob.aio import BlobServiceClient
from azure.core.exceptions import AzureError
from agent_c_tools.tools.workspace.blob_storage import BlobStorageWorkspace


@pytest.mark.integration
class TestBlobStorageWorkspaceIntegration:
    """
    Integration tests for BlobStorageWorkspace.

    IMPORTANT: These tests require:
    1. Valid Azure Blob Storage credentials configured in environment variables.
    2. The ability to create and delete containers in the Azure Storage account.
    """

    @pytest.fixture(scope="class")
    def container_name(self):
        """Generate a unique container name for testing."""
        return "agentcblobstoragetest"

    @pytest.fixture(scope="class")
    def prefix(self):
        """Generate a unique prefix for test files."""
        return "integration-test/"

    @pytest.fixture(scope="class")
    def workspace(self, container_name, prefix):
        """Create a workspace instance for integration testing."""
        return BlobStorageWorkspace(
            container_name=container_name,
            prefix=prefix,
            name="integration-test-workspace"
        )

    @pytest_asyncio.fixture(scope="class", autouse=True)
    async def setup_testing(self, container_name):
        """Setup the testing environment by ensuring the container exists."""
        connection_string = os.getenv("AZURE_STORAGE_CONNECTION_STRING")
        if not connection_string:
            pytest.fail(
                "AZURE_STORAGE_CONNECTION_STRING is not set in environment variables.")

        service_client = BlobServiceClient.from_connection_string(
            connection_string)
        container_client = service_client.get_container_client(container_name)

        try:
            await container_client.create_container()
        except AzureError:
            pass  # Ignore if the container already exists

        yield

        # Cleanup after tests
        try:
            await container_client.delete_container()
        except AzureError as e:
            print(f"Error during container cleanup: {e}")

    @pytest.mark.asyncio
    async def test_write_and_read_text(self, workspace):
        """Test writing and reading text files."""
        text_content = "Hello, Blob Storage Workspace Integration Test!"
        file_path = "test_file.txt"

        await workspace.write(file_path, "write", text_content)
        read_content = await workspace.read(file_path)

        assert read_content == text_content, "Written and read content should match"

    @pytest.mark.asyncio
    async def test_write_and_read_bytes(self, workspace):
        """Test writing and reading binary files."""
        binary_content = b"Binary test content with some \x00 null bytes"
        file_path = "test_binary_file.bin"

        await workspace.write_bytes(file_path, "write", binary_content)
        read_bytes = await workspace.read_bytes_internal(file_path)

        assert read_bytes == binary_content, "Written and read binary content should match"

    @pytest.mark.asyncio
    async def test_append_content(self, workspace):
        """Test appending content to a file."""
        file_path = "append_test.txt"

        await workspace.write(file_path, "write", "First line\n")
        await workspace.write(file_path, "append", "Second line\n")

        content = await workspace.read(file_path)
        assert content == "First line\nSecond line\n", "Appended content should match"

    @pytest.mark.asyncio
    async def test_list_files(self, workspace):
        """Test listing files in a directory."""
        files_to_create = [
            "list_test/file1.txt",
            "list_test/file2.txt",
            "list_test/subdir/file3.txt"
        ]

        for file_path in files_to_create:
            await workspace.write(file_path, "write", f"Content for {file_path}")

        listed_files = await workspace.ls("list_test")
        assert "file1.txt" in listed_files
        assert "file2.txt" in listed_files
        assert "subdir/file3.txt" in listed_files

    @pytest.mark.asyncio
    async def test_path_exists(self, workspace):
        """Test checking file existence."""
        await workspace.write("existence_test/file.txt", "write", "Test content")

        assert await workspace.path_exists("existence_test/file.txt") is True
        assert await workspace.path_exists("existence_test/nonexistent.txt") is False

    @pytest.mark.asyncio
    async def test_read_non_existent_file(self, workspace):
        """Test reading a non-existent file raises the correct exception."""
        with pytest.raises(FileNotFoundError):
            await workspace.read("non_existent_file.txt")

    @pytest.mark.asyncio
    async def test_read_only_workspace(self, workspace):
        """Test that a read-only workspace prevents writes."""
        ro_workspace = BlobStorageWorkspace(
            container_name=workspace.container_name,
            prefix=workspace.prefix,
            read_only=True
        )

        with pytest.raises(ValueError, match="Cannot write to a read-only workspace"):
            await ro_workspace.write("test.txt", "write", "Test content")

    @pytest.mark.asyncio
    async def test_tree(self, workspace):
        """Test generating a tree-like structure of files and directories."""
        files_to_create = [
            "tree_test/file1.txt",
            "tree_test/file2.txt",
            "tree_test/subdir/file3.txt"
        ]

        for file_path in files_to_create:
            await workspace.write(file_path, "write", f"Content for {file_path}")

        tree_output = await workspace.tree("tree_test")
        assert "file1.txt" in tree_output
        assert "file2.txt" in tree_output
        assert "subdir/file3.txt" in tree_output

    @pytest.mark.asyncio
    async def test_cp(self, workspace):
        """Test copying a file within the container."""
        src_path = "copy_test/source.txt"
        dest_path = "copy_test/destination.txt"

        await workspace.write(src_path, "write", "Content to copy")
        await workspace.cp(src_path, dest_path)

        src_content = await workspace.read(src_path)
        dest_content = await workspace.read(dest_path)

        assert src_content == "Content to copy", "Source content should match"
        assert dest_content == "Content to copy", "Destination content should match"

    @pytest.mark.asyncio
    async def test_mv(self, workspace):
        """Test moving a file within the container."""
        src_path = "move_test/source.txt"
        dest_path = "move_test/destination.txt"

        await workspace.write(src_path, "write", "Content to move")
        await workspace.mv(src_path, dest_path)

        assert not await workspace.path_exists(src_path), "Source file should not exist after move"
        dest_content = await workspace.read(dest_path)
        assert dest_content == "Content to move", "Destination content should match"
