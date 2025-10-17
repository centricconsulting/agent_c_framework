# tests/unit/api/v2/models/test_chat_models.py
from typing import Literal

import pytest
from datetime import datetime
from uuid import UUID, uuid4
from agent_c_api.api.v2.models.chat_models import (
    ChatMessageContent, ChatMessage, ChatRequest, ChatResponse,
    ChatEventType
)
from agent_c_api.api.v2.models.tool_models import (
    ToolCall, ToolResult
)

# Import core models for conversion tests
from agent_c.models.events.chat import MessageEvent
from agent_c.models.common_chat import (
    TextContentBlock, ImageContentBlock, BaseContentBlock
)
from agent_c_api.api.v2.models.chat_models import FileBlock

@pytest.mark.unit
@pytest.mark.models
@pytest.mark.chat
class TestChatModels:
    """Tests for the chat models used in the API v2.
    
    These tests verify that the chat-related models correctly validate input,
    store properties, and handle conversion between API models and core models.
    """
    
    class TestChatMessageContent:
        """Tests for the ChatMessageContent model.
        
        ChatMessageContent represents different types of content in a chat message,
        such as text, images, or file attachments.
        """
        
        def test_text_content(self):
            """Test text content type validation and properties."""
            # Test text content
            content = ChatMessageContent(
                type="text",
                text="Hello, world!"
            )
            assert content.type == "text"
            assert content.text == "Hello, world!"
            assert content.file_id is None
            assert content.mime_type is None
            
            # Test missing text for text type
            with pytest.raises(ValueError):
                ChatMessageContent(type="text")
        
        def test_image_content(self):
            """Test image content type validation and properties."""
            # Test image content
            content = ChatMessageContent(
                type="image",
                file_id="img-123",
                mime_type="image/jpeg"
            )
            assert content.type == "image"
            assert content.text is None
            assert content.file_id == "img-123"
            assert content.mime_type == "image/jpeg"
            
            # Test missing file_id for image type
            with pytest.raises(ValueError):
                ChatMessageContent(type="image")
        
        def test_file_content(self):
            """Test file content type validation and properties."""
            # Test file content
            content = ChatMessageContent(
                type="file",
                file_id="doc-456",
                mime_type="application/pdf"
            )
            assert content.type == "file"
            assert content.text is None
            assert content.file_id == "doc-456"
            assert content.mime_type == "application/pdf"
            
            # Test missing file_id for file type
            with pytest.raises(ValueError):
                ChatMessageContent(type="file")
        
        def test_to_content_block(self):
            """Test conversion from ChatMessageContent to core content blocks."""
            # Test text content conversion
            text_content = ChatMessageContent(type="text", text="Hello world")
            text_block = text_content.to_content_block()
            assert isinstance(text_block, TextContentBlock)
            assert text_block.text == "Hello world"
            
            # Test image content conversion
            image_content = ChatMessageContent(
                type="image", 
                file_id="img-123", 
                mime_type="image/jpeg"
            )
            image_block = image_content.to_content_block()
            assert isinstance(image_block, ImageContentBlock)
            assert image_block.source["file_id"] == "img-123"
            assert image_block.source["mime_type"] == "image/jpeg"
            
            # Test file content conversion
            file_content = ChatMessageContent(
                type="file", 
                file_id="doc-456", 
                mime_type="application/pdf"
            )
            file_block = file_content.to_content_block()
            assert isinstance(file_block, FileBlock)
            assert file_block.file_id == "doc-456"
            assert file_block.mime_type == "application/pdf"
            
            # Test invalid type
            invalid_content = ChatMessageContent(
                type="text", 
                text="Hello world"
            )
            # Monkey patch the type to cause an error
            invalid_content.type = "invalid"
            with pytest.raises(ValueError):
                invalid_content.to_content_block()
        
        def test_from_content_block(self):
            """Test conversion from core content blocks to ChatMessageContent."""
            # Test TextBlock conversion
            text_block = TextContentBlock(text="Hello world")
            text_content = ChatMessageContent.from_content_block(text_block)
            assert text_content.type == "text"
            assert text_content.text == "Hello world"
            assert text_content.file_id is None
            
            # Test ImageBlock conversion
            image_block = ImageContentBlock(source={"file_id": "img-123", "mime_type": "image/jpeg"})
            image_content = ChatMessageContent.from_content_block(image_block)
            assert image_content.type == "image"
            assert image_content.file_id == "img-123"
            assert image_content.mime_type == "image/jpeg"
            
            # Test FileBlock conversion
            file_block = FileBlock(file_id="doc-456", mime_type="application/pdf")
            file_content = ChatMessageContent.from_content_block(file_block)
            assert file_content.type == "file"
            assert file_content.file_id == "doc-456"
            assert file_content.mime_type == "application/pdf"
            
            # Test invalid block type
            class InvalidBlock(BaseContentBlock):
                type: Literal["invalid"] = "invalid"
            
            invalid_block = InvalidBlock()
            with pytest.raises(ValueError):
                ChatMessageContent.from_content_block(invalid_block)
    
    class TestChatMessage:
        """Tests for the ChatMessage model.
        
        ChatMessage represents a message in a chat conversation, which can contain
        multiple content parts, tool calls, and tool results.
        """
        
        def test_basic_properties(self):
            """Test ChatMessage with various combinations of properties."""
            message_id = uuid4()
            now = datetime.now()
            content = ChatMessageContent(type="text", text="Hello")
            
            # Test with minimal fields
            message = ChatMessage(
                role="user",
                content=[content]
            )
            assert message.id is None
            assert message.role == "user"
            assert message.created_at is None
            assert len(message.content) == 1
            assert message.content[0].text == "Hello"
            assert message.tool_calls is None
            assert message.tool_results is None
            
            # Test with all fields
            tool_call = ToolCall(tool_id="calculator", parameters={"expression": "2+2"})
            tool_result = ToolResult(tool_id="calculator", success=True, result=4)
            
            message = ChatMessage(
                id=message_id,
                role="assistant",
                created_at=now,
                content=[content],
                tool_calls=[tool_call],
                tool_results=[tool_result]
            )
            assert message.id == message_id
            assert message.role == "assistant"
            assert message.created_at == now
            assert len(message.content) == 1
            assert len(message.tool_calls) == 1
            assert message.tool_calls[0].tool_id == "calculator"
            assert len(message.tool_results) == 1
            assert message.tool_results[0].result == 4
        
        def test_to_message_event(self):
            """Test conversion from ChatMessage to core MessageEvent."""
            # Simple text content
            content = ChatMessageContent(type="text", text="Hello world")
            message = ChatMessage(role="user", content=[content])
            
            # Convert to MessageEvent
            event = message.to_message_event(session_id="test-session")
            
            # Verify properties
            assert isinstance(event, MessageEvent)
            assert event.session_id == "test-session"
            assert event.role == "user"
            assert event.content == "Hello world"
            assert event.format == "markdown"
            
            # Test with multiple text contents
            content1 = ChatMessageContent(type="text", text="Hello")
            content2 = ChatMessageContent(type="text", text="world")
            message = ChatMessage(role="user", content=[content1, content2])
            
            # Convert to MessageEvent
            event = message.to_message_event(session_id="test-session")
            
            # Verify text concatenation
            assert event.content == "Hello\nworld"
        
        def test_from_message_event(self):
            """Test conversion from core MessageEvent to ChatMessage."""
            # Create a MessageEvent
            event = MessageEvent(
                session_id="test-session",
                role="assistant",
                content="Hello, how can I help you today?",
                format="markdown"
            )
            
            # Convert to ChatMessage
            message = ChatMessage.from_message_event(event)
            
            # Verify properties
            assert message.role == "assistant"
            assert len(message.content) == 1
            assert message.content[0].type == "text"
            assert message.content[0].text == "Hello, how can I help you today?"
    
    class TestChatRequest:
        """Tests for the ChatRequest model.
        
        ChatRequest is used to send a message to the agent with streaming control.
        """
        
        def test_properties(self):
            """Test ChatRequest properties and defaults."""
            content = ChatMessageContent(type="text", text="What is 2+2?")
            message = ChatMessage(role="user", content=[content])
            
            # Test with default streaming
            request = ChatRequest(message=message)
            assert request.message.role == "user"
            assert request.message.content[0].text == "What is 2+2?"
            assert request.stream is True
            
            # Test with streaming disabled
            request = ChatRequest(message=message, stream=False)
            assert request.stream is False
    
    class TestChatResponse:
        """Tests for the ChatResponse model.
        
        ChatResponse is used for non-streaming responses, containing the complete
        message from the assistant.
        """
        
        def test_properties(self):
            """Test ChatResponse properties."""
            message_id = uuid4()
            now = datetime.now()
            content = ChatMessageContent(type="text", text="Paris is the capital of France.")
            
            # Create a message
            message = ChatMessage(
                id=message_id,
                role="assistant",
                created_at=now,
                content=[content]
            )
            
            # Create a response
            response = ChatResponse(
                message=message,
                completion_id="comp_xyz789"
            )
            
            # Verify properties
            assert response.message.id == message_id
            assert response.message.role == "assistant"
            assert response.message.created_at == now
            assert response.message.content[0].text == "Paris is the capital of France."
            assert response.completion_id == "comp_xyz789"
            
            # Test without completion_id (optional field)
            response = ChatResponse(message=message)
            assert response.message.id == message_id
            assert response.completion_id is None
    
    class TestChatEventType:
        """Tests for the ChatEventType enum.
        
        ChatEventType defines the possible event types during a streaming chat interaction.
        """
        
        def test_constants(self):
            """Test ChatEventType constants."""
            # Test event type constants
            assert ChatEventType.INTERACTION == "interaction"
            assert ChatEventType.COMPLETION == "completion"
            assert ChatEventType.MESSAGE == "message"
            assert ChatEventType.TEXT_DELTA == "text_delta"
            assert ChatEventType.THOUGHT_DELTA == "thought_delta"
            assert ChatEventType.TOOL_CALL == "tool_call"
            assert ChatEventType.TOOL_CALL_DELTA == "tool_select_delta"
            assert ChatEventType.HISTORY == "history"