"""Tests for chat model conversion utilities.

This module tests the conversion functions between API ChatMessage models
and core MessageEvent models to ensure proper data translation.
"""
import pytest
from agent_c.models.events.chat import MessageEvent 
from agent_c_api.api.v2.models.chat_models import (
    ChatMessage,
    ChatMessageContent,
    FileBlock,
    ImageBlock,
    TextBlock
)
from agent_c_api.api.v2.utils.model_converters import (
    chat_message_to_message_event,
    message_event_to_chat_message,
    content_blocks_to_message_content,
    message_content_to_content_blocks
)


@pytest.mark.unit
@pytest.mark.utils
@pytest.mark.converters
class TestChatMessageConverters:
    """Tests for conversion between API ChatMessage and core MessageEvent models."""
    
    def test_chat_message_to_message_event(self):
        """Test conversion from ChatMessage to MessageEvent with single text content."""
        # Create a chat message with text content
        chat_message = ChatMessage(
            role="user",
            content=[
                ChatMessageContent(type="text", text="Hello, how are you?")
            ]
        )
        
        # Use MnemonicSlugs format for session_id
        session_id = "tiger-castle"
        
        # Convert to message event
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify conversion
        assert isinstance(message_event, MessageEvent)
        assert message_event.session_id == session_id
        assert message_event.role == "user"
        assert message_event.content == "Hello, how are you?"
        assert message_event.format == "markdown"
    
    def test_chat_message_to_message_event_multiple_content(self):
        """Test conversion from ChatMessage with multiple text content blocks."""
        # Create a chat message with multiple text content blocks
        chat_message = ChatMessage(
            role="assistant",
            content=[
                ChatMessageContent(type="text", text="Hello!"),
                ChatMessageContent(type="text", text="How can I help you today?")
            ]
        )
        
        session_id = "tiger-piano"
        
        # Convert to message event
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify conversion (text should be joined with newlines)
        assert isinstance(message_event, MessageEvent)
        assert message_event.role == "assistant"
        assert message_event.content == "Hello!\nHow can I help you today?"
    
    def test_chat_message_to_message_event_with_image(self):
        """Test conversion from ChatMessage to MessageEvent with image content."""
        # Create a chat message with image content
        chat_message = ChatMessage(
            role="user",
            content=[
                ChatMessageContent(type="text", text="Check out this image:"),
                ChatMessageContent(type="image", file_id="img123", mime_type="image/jpeg")
            ]
        )
        
        session_id = "eagle-banana"
        
        # Convert to message event
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify conversion (should only include the text content)
        assert isinstance(message_event, MessageEvent)
        assert message_event.session_id == session_id
        assert message_event.role == "user"
        assert message_event.content == "Check out this image:"
        assert message_event.format == "markdown"

    def test_chat_message_to_message_event_with_file(self):
        """Test conversion from ChatMessage to MessageEvent with file content."""
        # Create a chat message with file content
        chat_message = ChatMessage(
            role="user",
            content=[
                ChatMessageContent(type="text", text="Here's the document:"),
                ChatMessageContent(type="file", file_id="doc456", mime_type="application/pdf")
            ]
        )
        
        session_id = "dolphin-castle"
        
        # Convert to message event
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify conversion (should only include the text content)
        assert isinstance(message_event, MessageEvent)
        assert message_event.session_id == session_id
        assert message_event.role == "user"
        assert message_event.content == "Here's the document:"
        assert message_event.format == "markdown"
    
    def test_message_event_to_chat_message(self):
        """Test conversion from MessageEvent to ChatMessage."""
        # Create a message event
        session_id = "ocean-mountain"
        message_event = MessageEvent(
            session_id=session_id,
            role="user",
            content="Hello, test message",
            format="markdown"
        )
        
        # Convert to chat message
        chat_message = message_event_to_chat_message(message_event)
        
        # Verify conversion
        assert isinstance(chat_message, ChatMessage)
        assert chat_message.role == "user"
        assert len(chat_message.content) == 1
        assert chat_message.content[0].type == "text"
        assert chat_message.content[0].text == "Hello, test message"
    
    def test_message_event_conversion_roundtrip(self):
        """Test roundtrip conversion between MessageEvent and ChatMessage."""
        # Start with a message event
        session_id = "purple-wizard"
        original_event = MessageEvent(
            session_id=session_id,
            role="assistant",
            content="This is a test message for roundtrip conversion",
            format="markdown"
        )
        
        # Convert to chat message and back
        chat_message = message_event_to_chat_message(original_event)
        converted_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify roundtrip conversion preserved essential data
        assert converted_event.session_id == original_event.session_id
        assert converted_event.role == original_event.role
        assert converted_event.content == original_event.content
        assert converted_event.format == original_event.format

    def test_chat_message_to_message_event_empty_content(self):
        """Test conversion from ChatMessage with empty content."""
        # Create a chat message with empty content list
        chat_message = ChatMessage(
            role="user",
            content=[]
        )
        
        session_id = "coffee-table"
        
        # Convert to message event
        message_event = chat_message_to_message_event(chat_message, session_id)
        
        # Verify conversion (should have empty content)
        assert isinstance(message_event, MessageEvent)
        assert message_event.session_id == session_id
        assert message_event.role == "user"
        assert message_event.content == ""
        assert message_event.format == "markdown"

    def test_message_event_to_chat_message_empty_content(self):
        """Test conversion from MessageEvent with empty content."""
        # Create a message event with empty content
        session_id = "marble-sunset"
        message_event = MessageEvent(
            session_id=session_id,
            role="user",
            content="",
            format="markdown"
        )
        
        # Convert to chat message
        chat_message = message_event_to_chat_message(message_event)
        
        # Verify conversion
        assert isinstance(chat_message, ChatMessage)
        assert chat_message.role == "user"
        assert len(chat_message.content) == 1
        assert chat_message.content[0].type == "text"
        assert chat_message.content[0].text == ""

    def test_content_blocks_to_message_content(self):
        """Test conversion from core content blocks to API message content."""
        # Create content blocks
        blocks = [
            TextBlock(text="Hello world"),
            ImageBlock(source={"file_id": "img123", "mime_type": "image/png"}),
            FileBlock(file_id="doc456", mime_type="application/pdf")
        ]
        
        # Convert to message content
        contents = content_blocks_to_message_content(blocks)
        
        # Verify conversion
        assert len(contents) == 3
        assert contents[0].type == "text"
        assert contents[0].text == "Hello world"
        assert contents[1].type == "image"
        assert contents[1].file_id == "img123"
        assert contents[1].mime_type == "image/png"
        assert contents[2].type == "file"
        assert contents[2].file_id == "doc456"
        assert contents[2].mime_type == "application/pdf"

    def test_message_content_to_content_blocks(self):
        """Test conversion from API message content to core content blocks."""
        # Create message content objects
        contents = [
            ChatMessageContent(type="text", text="Hello world"),
            ChatMessageContent(type="image", file_id="img123", mime_type="image/png"),
            ChatMessageContent(type="file", file_id="doc456", mime_type="application/pdf")
        ]
        
        # Convert to content blocks
        blocks = message_content_to_content_blocks(contents)
        
        # Verify conversion
        assert len(blocks) == 3
        assert blocks[0].type == "text"
        assert blocks[0].text == "Hello world"
        assert blocks[1].type == "image"
        assert blocks[1].source.get("file_id") == "img123"
        assert blocks[1].source.get("mime_type") == "image/png"
        assert blocks[2].type == "file"
        assert blocks[2].file_id == "doc456"
        assert blocks[2].mime_type == "application/pdf"