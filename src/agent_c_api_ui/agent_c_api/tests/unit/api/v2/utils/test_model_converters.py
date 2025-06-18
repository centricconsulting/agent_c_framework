"""Tests for model converter utilities.

These tests verify the functionality of the model converter utilities that translate
between v1 and v2 API models, ensuring proper backward compatibility and data integrity
during conversion.
"""

import pytest
from datetime import datetime
from typing import List, Optional

# Import v1 API models
from agent_c_api.api.v1.llm_models.agent_params import (
    AgentInitializationParams,
    AgentUpdateParams
)

# Import v2 API models
from agent_c_api.api.v2.models.session_models import (
    SessionCreate,
    SessionUpdate
)
from agent_c_api.api.v2.models.chat_models import (
    ChatMessage,
    ChatMessageContent,
    TextBlock,
    ImageBlock,
    FileBlock,
    ContentBlockUnion
)

# Import converter utilities
from agent_c_api.api.v2.utils.model_converters import (
    v1_to_v2_session_create,
    v1_to_v2_session_update,
    v2_to_v1_session_params,
    content_blocks_to_message_content,
    message_content_to_content_blocks,
    message_event_to_chat_message,
    chat_message_to_message_event
)

# Import core models
from agent_c.models.events.chat import MessageEvent


@pytest.mark.unit
@pytest.mark.utils
@pytest.mark.converters
class TestV1ToV2Converters:
    """Tests for functions that convert v1 API models to v2 API models.
    
    These tests verify that the conversion functions correctly transform v1 model
    instances to their v2 counterparts, preserving all relevant data and applying
    proper mapping of fields between the different model structures.
    """
    
    def test_v1_to_v2_session_create_minimal(self):
        """Test conversion from v1 AgentInitializationParams to v2 SessionCreate with minimal parameters."""
        # Test with minimal required parameters
        v1_params = AgentInitializationParams(
            model_name="gpt-4o",
            backend="openai"
        )
        
        v2_model = v1_to_v2_session_create(v1_params)
        
        assert v2_model.model_id == "gpt-4o"
        assert v2_model.persona_id == "default"
        assert v2_model.metadata == {"backend": "openai"}
    
    def test_v1_to_v2_session_create_full(self):
        """Test conversion from v1 AgentInitializationParams to v2 SessionCreate with all parameters."""
        # Test with all parameters
        v1_params = AgentInitializationParams(
            model_name="claude-3-5-sonnet-latest",
            backend="anthropic",
            agent_key="researcher",
            custom_prompt="Act as a helpful researcher",
            temperature=0.7,
            max_tokens=4000,
            reasoning_effort="high",
            budget_tokens=1000,
            ui_session_id="existing-session-id"
        )
        
        v2_model = v1_to_v2_session_create(v1_params)
        
        assert v2_model.model_id == "claude-3-5-sonnet-latest"
        assert v2_model.persona_id == "researcher"
        assert v2_model.metadata == {"backend": "anthropic", "custom_prompt": "Act as a helpful researcher"}
        # Parameters are no longer directly stored in the session model
    
    def test_v1_to_v2_session_update(self):
        """Test conversion from v1 AgentUpdateParams to v2 SessionUpdate."""
        # Test update parameters conversion
        v1_params = AgentUpdateParams(
            ui_session_id="session-to-update",
            agent_key="coder",
            custom_prompt="Act as a code reviewer",
            temperature=0.5,
            reasoning_effort="medium"
        )
        
        v2_model = v1_to_v2_session_update(v1_params)
        
        assert v2_model.name is None  # Name update not present in v1
        assert v2_model.metadata == {"custom_prompt": "Act as a code reviewer"}
    
    def test_v1_to_v2_session_create_with_empty_values(self):
        """Test conversion with empty or None values in optional fields."""
        # Test with minimal parameters and None values for optional fields
        v1_params = AgentInitializationParams(
            model_name="gpt-4o",
            backend=None,  # None backend should default to "openai"
            agent_key=None,  # None persona should default to "default"
            custom_prompt=None,
            temperature=None,
            max_tokens=None,
            reasoning_effort=None,
            budget_tokens=None,
            ui_session_id=None
        )
        
        v2_model = v1_to_v2_session_create(v1_params)
        
        assert v2_model.model_id == "gpt-4o"
        assert v2_model.persona_id == "default"
        assert v2_model.metadata == {"backend": "openai"}
        assert "custom_prompt" not in v2_model.metadata


@pytest.mark.unit
@pytest.mark.utils
@pytest.mark.converters
class TestV2ToV1Converters:
    """Tests for functions that convert v2 API models to v1 API models.
    
    These tests verify that the conversion functions correctly transform v2 model
    instances back to their v1 counterparts, preserving all relevant data and 
    applying proper mapping of fields between the different model structures.
    """
    
    def test_v2_to_v1_session_params_minimal(self):
        """Test conversion from v2 SessionCreate to v1 AgentInitializationParams with minimal parameters."""
        # Test with minimal required parameters
        v2_model = SessionCreate(
            model_id="gpt-4o",
            persona_id="default",
            metadata={"backend": "openai"}
        )
        
        v1_params = v2_to_v1_session_params(v2_model)
        
        assert v1_params.model_name == "gpt-4o"
        assert v1_params.backend == "openai"
        assert v1_params.agent_key == "default"
        assert v1_params.ui_session_id is None
    
    def test_v2_to_v1_session_params_full(self):
        """Test conversion from v2 SessionCreate to v1 AgentInitializationParams with all parameters."""
        # Test with all parameters
        v2_model = SessionCreate(
            model_id="claude-3-5-sonnet-latest",
            persona_id="researcher",
            metadata={
                "backend": "anthropic",
                "custom_prompt": "Act as a helpful researcher"
            }
        )
        
        v1_params = v2_to_v1_session_params(v2_model)
        
        assert v1_params.model_name == "claude-3-5-sonnet-latest"
        assert v1_params.backend == "anthropic"
        assert v1_params.agent_key == "researcher"
        assert v1_params.custom_prompt == "Act as a helpful researcher"
        # Check temperature, reasoning_effort, and budget_tokens (should be None by default)
        assert v1_params.temperature is None
        assert v1_params.reasoning_effort is None
        assert v1_params.budget_tokens is None
        
        # For Claude models, max_tokens gets default value of 8192 (set by the model validator)
        assert v1_params.max_tokens == 8192
    
    def test_v2_to_v1_session_params_with_empty_metadata(self):
        """Test conversion with empty metadata dictionary."""
        # Test with empty metadata
        v2_model = SessionCreate(
            model_id="gpt-4o",
            persona_id="default",
            metadata=None  # None metadata should result in default values
        )
        
        v1_params = v2_to_v1_session_params(v2_model)
        
        assert v1_params.model_name == "gpt-4o"
        assert v1_params.backend == "openai"  # Default backend
        assert v1_params.agent_key == "default"
        assert v1_params.custom_prompt is None


@pytest.mark.unit
@pytest.mark.utils
@pytest.mark.converters
class TestMessageConverters:
    """Tests for message content conversion functions.
    
    These tests verify the correct conversion between core content blocks and API
    message content objects, as well as between core MessageEvent and API ChatMessage
    models. These conversions are essential for the API to communicate with the
    core agent functionality.
    """
    
    def test_content_blocks_to_message_content_text(self):
        """Test conversion from text content block to message content."""
        # Create a core text content block
        text_block = TextBlock(text="Hello, world!")
        
        # Convert to API message content
        content = content_blocks_to_message_content([text_block])
        
        assert len(content) == 1
        assert content[0].type == "text"
        assert content[0].text == "Hello, world!"
        assert content[0].file_id is None
        assert content[0].mime_type is None
    
    def test_content_blocks_to_message_content_image(self):
        """Test conversion from image content block to message content."""
        # Create a core image content block
        image_block = ImageBlock(source={"file_id": "image-123", "mime_type": "image/jpeg"})
        
        # Convert to API message content
        content = content_blocks_to_message_content([image_block])
        
        assert len(content) == 1
        assert content[0].type == "image"
        assert content[0].text is None
        assert content[0].file_id == "image-123"
        assert content[0].mime_type == "image/jpeg"
    
    def test_content_blocks_to_message_content_file(self):
        """Test conversion from file content block to message content."""
        # Create a core file content block
        file_block = FileBlock(file_id="file-456", mime_type="application/pdf")
        
        # Convert to API message content
        content = content_blocks_to_message_content([file_block])
        
        assert len(content) == 1
        assert content[0].type == "file"
        assert content[0].text is None
        assert content[0].file_id == "file-456"
        assert content[0].mime_type == "application/pdf"
    
    def test_content_blocks_to_message_content_mixed(self):
        """Test conversion from mixed content blocks to message content."""
        # Create mixed content blocks
        blocks = [
            TextBlock(text="Check out this image:"),
            ImageBlock(source={"file_id": "image-123", "mime_type": "image/jpeg"}),
            TextBlock(text="And this document:"),
            FileBlock(file_id="file-456", mime_type="application/pdf")
        ]
        
        # Convert to API message content
        content = content_blocks_to_message_content(blocks)
        
        assert len(content) == 4
        assert content[0].type == "text"
        assert content[0].text == "Check out this image:"
        assert content[1].type == "image"
        assert content[1].file_id == "image-123"
        assert content[2].type == "text"
        assert content[2].text == "And this document:"
        assert content[3].type == "file"
        assert content[3].file_id == "file-456"
    
    def test_message_content_to_content_blocks_text(self):
        """Test conversion from text message content to content blocks."""
        # Create an API message content with text
        message_content = ChatMessageContent(type="text", text="Hello, world!")
        
        # Convert to core content blocks
        blocks = message_content_to_content_blocks([message_content])
        
        assert len(blocks) == 1
        assert isinstance(blocks[0], TextBlock)
        assert blocks[0].text == "Hello, world!"
    
    def test_message_content_to_content_blocks_image(self):
        """Test conversion from image message content to content blocks."""
        # Create an API message content with image
        message_content = ChatMessageContent(
            type="image", 
            file_id="image-123", 
            mime_type="image/jpeg"
        )
        
        # Convert to core content blocks
        blocks = message_content_to_content_blocks([message_content])
        
        assert len(blocks) == 1
        assert isinstance(blocks[0], ImageBlock)
        assert blocks[0].source.get("file_id") == "image-123"
        assert blocks[0].source.get("mime_type") == "image/jpeg"
    
    def test_message_content_to_content_blocks_file(self):
        """Test conversion from file message content to content blocks."""
        # Create an API message content with file
        message_content = ChatMessageContent(
            type="file", 
            file_id="file-456", 
            mime_type="application/pdf"
        )
        
        # Convert to core content blocks
        blocks = message_content_to_content_blocks([message_content])
        
        assert len(blocks) == 1
        assert isinstance(blocks[0], FileBlock)
        assert blocks[0].file_id == "file-456"
        assert blocks[0].mime_type == "application/pdf"
    
    def test_message_event_to_chat_message(self):
        """Test conversion from a core MessageEvent to an API ChatMessage."""
        # Create a core message event
        message_event = MessageEvent(
            session_id="tiger-castle",
            role="user",
            content="Hello, can you help me with Python?",
            format="markdown"
        )
        
        # Convert to API chat message
        chat_message = message_event_to_chat_message(message_event)
        
        assert chat_message.role == "user"
        assert len(chat_message.content) == 1
        assert chat_message.content[0].type == "text"
        assert chat_message.content[0].text == "Hello, can you help me with Python?"
    
    def test_chat_message_to_message_event(self):
        """Test conversion from an API ChatMessage to a core MessageEvent."""
        # Create an API chat message
        chat_message = ChatMessage(
            role="assistant",
            content=[
                ChatMessageContent(type="text", text="Here's how you can read a file in Python:")
            ]
        )
        
        # Convert to core message event
        session_id = "tiger-castle"
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        assert message_event.session_id == "tiger-castle"
        assert message_event.role == "assistant"
        assert message_event.content == "Here's how you can read a file in Python:"
        assert message_event.format == "markdown"  # Default format
    
    def test_chat_message_to_message_event_multiple_content(self):
        """Test conversion of ChatMessage with multiple text content blocks."""
        # Create an API chat message with multiple text contents
        chat_message = ChatMessage(
            role="user",
            content=[
                ChatMessageContent(type="text", text="First paragraph"),
                ChatMessageContent(type="text", text="Second paragraph")
            ]
        )
        
        # Convert to core message event
        session_id = "tiger-castle"
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        assert message_event.session_id == "tiger-castle"
        assert message_event.role == "user"
        # Text content should be concatenated with newlines
        assert message_event.content == "First paragraph\nSecond paragraph"
    
    def test_empty_content_handling(self):
        """Test handling of empty content lists."""
        # Test empty content blocks to message content
        assert content_blocks_to_message_content([]) == []
        
        # Test empty message content to content blocks
        assert message_content_to_content_blocks([]) == []
        
        # Test chat message with empty content
        chat_message = ChatMessage(
            role="user",
            content=[]
        )
        
        message_event = chat_message_to_message_event(chat_message, "tiger-castle")
        assert message_event.content == ""  # Empty content string