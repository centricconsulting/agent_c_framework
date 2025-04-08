""" Integration tests for S3Workspace. """

import logging
import os
import uuid
import pytest
import pytest_asyncio
from aiobotocore.session import get_session
from botocore.exceptions import ClientError
from agent_c_tools.tools.workspace.s3_storage import S3StorageWorkspace


@pytest.mark.integration
class TestS3WorkspaceIntegration:
    """
    Integration tests for S3Workspace.

    IMPORTANT: These tests require:
    1. Valid AWS credentials configured
    2. Credentials have ability to create S3 bucket and can write objects to it
    """

    @pytest.fixture(scope="class")
    def bucket_name(self):
        """
        Generate a unique bucket name for testing.
        In a real scenario, you'd use a pre-existing test bucket.
        """
        return "test-bucket-agentc"

    @pytest.fixture(scope="class")
    def prefix(self):
        """
        Generate a unique prefix for test files.
        """
        return f"integration-test/{uuid.uuid4().hex[:8]}/"

    @pytest.fixture(scope="class")
    def workspace(self, bucket_name, prefix):
        """
        Create a workspace instance for integration testing.
        """

        workspace = S3StorageWorkspace(
            bucket_name=bucket_name,
            prefix=prefix,
            name="integration-test-workspace"
        )

        return workspace

    @pytest_asyncio.fixture(scope="class", autouse=True)
    async def setup_testing(self, bucket_name):
        """
        Setup the testing environment.
        """

        region_name = os.getenv('AWS_REGION_NAME')
        access_key_id = os.getenv('AWS_ACCESS_KEY_ID')
        secret_access_key = os.getenv('AWS_SECRET_ACCESS_KEY')
        session_token = os.getenv('AWS_SESSION_TOKEN')

        session = get_session()
        async with session.create_client('s3', region_name=region_name,
                                         aws_access_key_id=access_key_id,
                                         aws_secret_access_key=secret_access_key,
                                         aws_session_token=session_token) as client:
            try:
                await client.head_bucket(Bucket=bucket_name)
                logging.info("bucket already exists")
            except ClientError as ce:
                if ce.response['Error']['Code'] == '404':
                    try:
                        await client.create_bucket(Bucket=bucket_name,
                                                   ACL='private',
                                                   CreateBucketConfiguration={
                                                       'LocationConstraint': region_name
                                                   })
                        logging.info("bucket created")
                    except Exception as e:
                        logging.error(e)
                        raise e

    @pytest_asyncio.fixture(scope="class", autouse=True)
    async def cleanup_workspace(self, bucket_name):
        """
        Fixture to clean up all test files after tests complete

        Runs automatically due to autouse=True
        Cleans up all files under the test prefix
        """
        # Yield control to tests
        yield

        # Cleanup after all tests in the class complete
        try:
            await self.integration_test_cleanup(bucket_name)

        except Exception as e:
            # Log or print cleanup errors without failing the test suite
            print(f"Error during cleanup: {e}")

    @pytest.mark.asyncio
    async def test_write_and_read_text(self, workspace):
        """
        Test writing and reading text files.
        """
        # Write a text file
        text_content = "Hello, S3 Workspace Integration Test!"
        file_path = "test_file.txt"

        await workspace.write(file_path, "write", text_content)

        # Read the file back
        read_content = await workspace.read(file_path)

        assert read_content == text_content, "Written and read content should match"

    @pytest.mark.asyncio
    async def test_write_and_read_bytes(self, workspace):
        """
        Test writing and reading binary files.
        """
        # Create some binary content
        binary_content = b"Binary test content with some \x00 null bytes"
        file_path = "test_binary_file.bin"

        await workspace.write_bytes(file_path, "write", binary_content)

        # Read bytes back
        read_bytes = await workspace.read_bytes_internal(file_path)

        assert read_bytes == binary_content, "Written and read binary content should match"

    @pytest.mark.asyncio
    async def test_append_content(self, workspace):
        """
        Test appending content to a file.
        """
        file_path = "append_test.txt"

        # First write
        await workspace.write(file_path, "write", "First line\n")

        # Append
        await workspace.write(file_path, "append", "Second line\n")

        # Read and verify
        content = await workspace.read(file_path)

        assert content == "First line\nSecond line\n", "Appended content should match"

    @pytest.mark.asyncio
    async def test_list_files(self, workspace):
        """
        Test listing files in a directory.
        """
        # Create multiple test files
        files_to_create = [
            "list_test/file1.txt",
            "list_test/file2.txt",
            "list_test/subdir/file3.txt"
        ]

        # Write some content to these files
        for file_path in files_to_create:
            await workspace.write(file_path, "write", f"Content for {file_path}")

        # List files in the directory
        listed_files = await workspace.ls("list_test")

        # Split the result and check
        listed_files_set = set(listed_files.split('\n'))
        expected_files_set = {"file1.txt", "file2.txt", "subdir/file3.txt"}

        assert listed_files_set == expected_files_set, "Listed files should match expected"

    @pytest.mark.asyncio
    async def test_path_exists(self, workspace):
        """
        Test checking file and directory existence.
        """
        # Create a test file
        await workspace.write("existence_test/file.txt", "write", "Test content")

        # Check file existence
        assert await workspace.path_exists("existence_test/file.txt") is True
        assert await workspace.path_exists("existence_test/nonexistent.txt") is False

        # Check directory existence
        assert await workspace.path_exists("existence_test/") is True
        assert await workspace.path_exists("nonexistent_dir/") is False

    @pytest.mark.asyncio
    async def test_read_non_existent_file(self, workspace):
        """
        Test reading a non-existent file raises the correct exception.
        """
        with pytest.raises(FileNotFoundError):
            await workspace.read("non_existent_file.txt")

    @pytest.mark.asyncio
    async def test_read_only_workspace(self, workspace):
        """
        Test that a read-only workspace prevents writes.
        """
        # Create a read-only workspace
        ro_workspace = S3StorageWorkspace(
            bucket_name=workspace.bucket_name,
            prefix=workspace.prefix,
            read_only=True
        )

        with pytest.raises(ValueError, match="Cannot write to read-only workspace"):
            await ro_workspace.write("test.txt", "write", "Test content")

    @pytest.mark.asyncio
    async def test_tree(self, workspace):
        """
        Test generating a tree-like structure of files and directories.
        """
        # Create test files and directories
        files_to_create = [
            "tree_test/file1.txt",
            "tree_test/file2.txt",
            "tree_test/subdir/file3.txt"
        ]

        for file_path in files_to_create:
            await workspace.write(file_path, "write", f"Content for {file_path}")

        # Generate tree structure
        tree_output = await workspace.tree("tree_test")

        # Expected tree structure
        expected_tree = "\n".join(sorted([
            "file1.txt",
            "file2.txt",
            "subdir/file3.txt"
        ]))

        assert tree_output == expected_tree, "Tree structure should match expected output"

    @pytest.mark.asyncio
    async def test_cp(self, workspace):
        """
        Test copying a file within the S3 bucket.
        """
        src_path = "copy_test/source.txt"
        dest_path = "copy_test/destination.txt"

        # Write source file
        await workspace.write(src_path, "write", "Content to copy")

        # Copy the file
        await workspace.cp(src_path, dest_path)

        # Verify both source and destination exist with the same content
        src_content = await workspace.read(src_path)
        dest_content = await workspace.read(dest_path)

        assert src_content == "Content to copy", "Source content should match"
        assert dest_content == "Content to copy", "Destination content should match"

    @pytest.mark.asyncio
    async def test_mv(self, workspace):
        """
        Test moving a file within the S3 bucket.
        """
        src_path = "move_test/source.txt"
        dest_path = "move_test/destination.txt"

        # Write source file
        await workspace.write(src_path, "write", "Content to move")

        # Move the file
        await workspace.mv(src_path, dest_path)

        # Verify source no longer exists and destination has the content
        assert not await workspace.path_exists(src_path), "Source file should not exist after move"
        dest_content = await workspace.read(dest_path)
        assert dest_content == "Content to move", "Destination content should match"

    @pytest.mark.asyncio
    async def integration_test_cleanup(self, bucket_name):
        """
        Method for cleaning up S3 bucket after integration tests.
        """

        # Proceed with deletion if context is valid
        session = get_session()
        async with session.create_client('s3', region_name=os.getenv('AWS_REGION_NAME'),
                                         aws_access_key_id=os.getenv(
                                             'AWS_ACCESS_KEY_ID'),
                                         aws_secret_access_key=os.getenv(
                                             'AWS_SECRET_ACCESS_KEY'),
                                         aws_session_token=os.getenv('AWS_SESSION_TOKEN')) as client:
            try:
                # List all objects in the bucket
                paginator = client.get_paginator('list_objects_v2')
                async for page in paginator.paginate(Bucket=bucket_name):
                    if 'Contents' in page:
                        delete_requests = [{'Key': obj['Key']}
                                           for obj in page['Contents']]

                        # Delete objects in batches
                        await client.delete_objects(
                            Bucket=bucket_name,
                            Delete={'Objects': delete_requests}
                        )
                await client.delete_bucket(Bucket=bucket_name)
            except ClientError as client_error:
                logging.error("Error during cleanup: %s", client_error)
            except Exception as e:
                logging.error(e)
                raise e
