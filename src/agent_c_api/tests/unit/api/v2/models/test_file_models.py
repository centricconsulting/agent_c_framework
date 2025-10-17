# tests/unit/api/v2/models/test_file_models.py
import pytest
from datetime import datetime
from uuid import uuid4
from pydantic import ValidationError

from agent_c_api.api.v2.models.file_models import (
    FileMeta, FileUploadResponse
)
from agent_c_api.api.v2.models.chat_models import FileBlock


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.files
class TestFileMeta:
    """Tests for the FileMeta model.
    
    These tests verify the initialization, validation, and behavior of the
    FileMeta model, which contains metadata about files uploaded to sessions.
    """
    
    def test_file_meta_minimal_fields(self):
        """Test FileMeta initialization with only required fields.
        
        This test verifies that the FileMeta model can be instantiated with only
        the required fields, and that default values are properly applied to
        optional fields.
        """
        session_id = str(uuid4())  # Convert UUID to string for MnemonicSlug format
        now = datetime.now()
        
        file_meta = FileMeta(
            id="file-123",
            filename="document.pdf",
            content_type="application/pdf",
            size=12345,
            uploaded_at=now,
            session_id=session_id
        )
        assert file_meta.id == "file-123"
        assert file_meta.filename == "document.pdf"
        assert file_meta.content_type == "application/pdf"
        assert file_meta.size == 12345
        assert file_meta.uploaded_at == now
        assert file_meta.session_id == session_id
        assert file_meta.metadata == {}, "Default value for metadata should be an empty dict"
    
    def test_file_meta_all_fields(self):
        """Test FileMeta initialization with all fields including optional ones.
        
        This test verifies that the FileMeta model can be instantiated with all
        fields, including optional ones, and that all values are correctly stored.
        """
        session_id = str(uuid4())  # Convert UUID to string for MnemonicSlug format
        now = datetime.now()
        
        file_meta = FileMeta(
            id="file-456",
            filename="image.jpg",
            content_type="image/jpeg",
            size=54321,
            uploaded_at=now,
            session_id=session_id,
            metadata={"width": 1920, "height": 1080}
        )
        assert file_meta.id == "file-456"
        assert file_meta.filename == "image.jpg"
        assert file_meta.content_type == "image/jpeg"
        assert file_meta.size == 54321
        assert file_meta.uploaded_at == now
        assert file_meta.session_id == session_id
        assert file_meta.metadata == {"width": 1920, "height": 1080}
    
    def test_file_meta_schema_config(self):
        """Test that the FileMeta model has the expected schema configuration.
        
        This test verifies the model's schema configuration, including field descriptions
        and examples that are used for API documentation.
        """
        # Check schema for the model
        schema = FileMeta.model_json_schema()
        
        # Verify field descriptions
        assert "id" in schema["properties"]
        assert "description" in schema["properties"]["id"]
        assert "filename" in schema["properties"]
        assert "description" in schema["properties"]["filename"]
        assert "content_type" in schema["properties"]
        assert "description" in schema["properties"]["content_type"]
        assert "size" in schema["properties"]
        assert "description" in schema["properties"]["size"]
        assert "uploaded_at" in schema["properties"]
        assert "description" in schema["properties"]["uploaded_at"]
        assert "session_id" in schema["properties"]
        assert "description" in schema["properties"]["session_id"]
        assert "metadata" in schema["properties"]
        assert "description" in schema["properties"]["metadata"]
        
        # Verify examples
        assert "examples" in schema, "Schema should have examples"
        assert len(schema["examples"]) > 0, "Should have at least one example"
        example = schema["examples"][0]
        assert "id" in example, "Example should have id field"
        assert "filename" in example, "Example should have filename field"
        assert "content_type" in example, "Example should have content_type field"
        assert "size" in example, "Example should have size field"
        assert "uploaded_at" in example, "Example should have uploaded_at field"
        assert "session_id" in example, "Example should have session_id field"
        assert "metadata" in example, "Example should have metadata field"
    
    def test_file_meta_complex_metadata(self):
        """Test FileMeta with complex nested metadata structures.
        
        This test verifies that the metadata field can store complex nested structures,
        which is useful for extended file information like processing results.
        """
        session_id = str(uuid4())  # Convert UUID to string for MnemonicSlug format
        now = datetime.now()
        
        complex_metadata = {
            "processed": True,
            "processing_details": {
                "timing": {
                    "start": "2025-05-01T12:00:00Z",
                    "end": "2025-05-01T12:01:30Z",
                    "duration_seconds": 90
                },
                "results": {
                    "page_count": 5,
                    "word_count": 2500,
                    "extracted_entities": ["Company A", "Person B", "Location C"]
                },
                "status": "completed"
            }
        }
        
        file_meta = FileMeta(
            id="file-789",
            filename="report.pdf",
            content_type="application/pdf",
            size=98765,
            uploaded_at=now,
            session_id=session_id,
            metadata=complex_metadata
        )
        
        assert file_meta.metadata == complex_metadata
        assert file_meta.metadata["processed"] is True
        assert file_meta.metadata["processing_details"]["results"]["page_count"] == 5
        assert len(file_meta.metadata["processing_details"]["results"]["extracted_entities"]) == 3


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.files
class TestFileUploadResponse:
    """Tests for the FileUploadResponse model.
    
    These tests verify the initialization, validation, and behavior of the
    FileUploadResponse model, which represents the response returned when
    a file is successfully uploaded.
    """
    
    def test_file_upload_response_required_fields(self):
        """Test FileUploadResponse initialization with required fields.
        
        This test verifies that the FileUploadResponse model can be instantiated
        with all required fields and that values are correctly stored.
        """
        response = FileUploadResponse(
            file_id="file-123",
            filename="document.pdf",
            content_type="application/pdf",
            size=12345
        )
        assert response.file_id == "file-123"
        assert response.filename == "document.pdf"
        assert response.content_type == "application/pdf"
        assert response.size == 12345
    
    def test_file_upload_response_missing_fields(self):
        """Test FileUploadResponse validation with missing required fields.
        
        This test verifies that the model correctly raises ValidationError
        when required fields are missing.
        """
        # Test with missing filename
        with pytest.raises(ValidationError) as exc_info:
            FileUploadResponse(
                file_id="file-123",
                # filename is missing
                content_type="application/pdf",
                size=12345
            )
        assert "filename" in str(exc_info.value), "Error should mention the missing filename field"
        
        # Test with missing file_id
        with pytest.raises(ValidationError) as exc_info:
            FileUploadResponse(
                # file_id is missing
                filename="document.pdf",
                content_type="application/pdf",
                size=12345
            )
        assert "file_id" in str(exc_info.value), "Error should mention the missing file_id field"
    
    def test_file_upload_response_schema_config(self):
        """Test that the FileUploadResponse model has the expected schema configuration.
        
        This test verifies the model's schema configuration, including field descriptions
        and examples that are used for API documentation.
        """
        # Check schema for the model
        schema = FileUploadResponse.model_json_schema()
        
        # Verify field descriptions
        assert "file_id" in schema["properties"]
        assert "description" in schema["properties"]["file_id"]
        assert "filename" in schema["properties"]
        assert "description" in schema["properties"]["filename"]
        assert "content_type" in schema["properties"]
        assert "description" in schema["properties"]["content_type"]
        assert "size" in schema["properties"]
        assert "description" in schema["properties"]["size"]
        
        # Verify examples
        assert "examples" in schema, "Schema should have examples"
        assert len(schema["examples"]) > 0, "Should have at least one example"
        example = schema["examples"][0]
        assert "file_id" in example, "Example should have file_id field"
        assert "filename" in example, "Example should have filename field"
        assert "content_type" in example, "Example should have content_type field"
        assert "size" in example, "Example should have size field"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.files
@pytest.mark.chat
class TestFileBlock:
    """Tests for the FileBlock model from chat_models.
    
    These tests verify the initialization, validation, and behavior of the
    FileBlock model, which represents a file attachment in a chat message content block.
    """
    
    def test_file_block_minimal_fields(self):
        """Test FileBlock initialization with only required fields.
        
        This test verifies that the FileBlock model can be instantiated with only
        the required fields, and that default values are properly applied to
        optional fields.
        """
        file_block = FileBlock(file_id="file-123")
        
        assert file_block.type == "file", "Type should be set to 'file'"
        assert file_block.file_id == "file-123"
        assert file_block.mime_type is None, "Default mime_type should be None"
    
    def test_file_block_all_fields(self):
        """Test FileBlock initialization with all fields.
        
        This test verifies that the FileBlock model can be instantiated with all
        fields and that values are correctly stored.
        """
        file_block = FileBlock(
            file_id="file-456",
            mime_type="application/pdf"
        )
        
        assert file_block.type == "file", "Type should be set to 'file'"
        assert file_block.file_id == "file-456"
        assert file_block.mime_type == "application/pdf"
    
    def test_file_block_conversion_from_message_content(self):
        """Test conversion between ChatMessageContent and FileBlock.
        
        This test verifies that a FileBlock can be created from a ChatMessageContent
        object, ensuring proper interoperability between these models.
        """
        from agent_c_api.api.v2.models.chat_models import ChatMessageContent
        
        # Create a message content object
        content = ChatMessageContent(
            type="file",
            file_id="file-789",
            mime_type="application/pdf"
        )
        
        # Convert to a content block
        block = content.to_content_block()
        
        # Verify it's a FileBlock with correct properties
        assert isinstance(block, FileBlock)
        assert block.type == "file"
        assert block.file_id == "file-789"
        assert block.mime_type == "application/pdf"
    
    def test_file_block_conversion_to_message_content(self):
        """Test conversion from FileBlock to ChatMessageContent.
        
        This test verifies that a ChatMessageContent object can be created from 
        a FileBlock, ensuring proper interoperability between these models.
        """
        from agent_c_api.api.v2.models.chat_models import ChatMessageContent
        
        # Create a file block
        file_block = FileBlock(
            file_id="file-987",
            mime_type="image/png"
        )
        
        # Convert to message content
        content = ChatMessageContent.from_content_block(file_block)
        
        # Verify properties
        assert content.type == "file"
        assert content.file_id == "file-987"
        assert content.mime_type == "image/png"
        assert content.text is None