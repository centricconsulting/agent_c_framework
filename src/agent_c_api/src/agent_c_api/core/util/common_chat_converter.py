from typing import Dict, Any, Optional, List
from datetime import datetime
from uuid import uuid4

# Import the CommonChatMessage model and related classes
from agent_c.models.common_chat.models import (
    CommonChatMessage,
    MessageRole,
    ContentBlockType,
    TextContentBlock,
    ToolUseContentBlock,
    ToolResultContentBlock
)

# Import the current event models
from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent


class CommonChatConverter:
    """Converter between MessageEvent/ToolCallEvent and CommonChatMessage models."""
    
    @staticmethod
    def message_event_to_common_chat(message: MessageEvent) -> CommonChatMessage:
        """Convert a MessageEvent to CommonChatMessage.
        
        Args:
            message (MessageEvent): The message event to convert
            
        Returns:
            CommonChatMessage: The converted message
        """
        # Create content blocks
        content_blocks: List[Any] = []
        
        # Add text content if available
        if message.content:
            content_blocks.append(TextContentBlock(text=message.content))
        
        # Determine the message role
        role = MessageRole.USER if message.role == "user" else \
               MessageRole.ASSISTANT if message.role == "assistant" else \
               MessageRole.SYSTEM if message.role == "system" else \
               MessageRole.TOOL
        
        # Create the CommonChatMessage
        return CommonChatMessage(
            id=message.id or str(uuid4()),
            role=role,
            content=content_blocks,
            created_at=datetime.fromisoformat(message.timestamp) if message.timestamp else datetime.now()
        )
    
    @staticmethod
    def tool_call_event_to_common_chat(tool_call: ToolCallEvent) -> CommonChatMessage:
        """Convert a ToolCallEvent to CommonChatMessage.
        
        Args:
            tool_call (ToolCallEvent): The tool call event to convert
            
        Returns:
            CommonChatMessage: The converted message
        """
        # Create content blocks
        content_blocks = []
        
        # Add tool use content
        content_blocks.append(ToolUseContentBlock(
            tool_name=tool_call.name,
            tool_id=tool_call.id or str(uuid4()),
            parameters=tool_call.parameters or {}
        ))
        
        # If result is available, add tool result content
        if hasattr(tool_call, 'result') and tool_call.result is not None:
            content_blocks.append(ToolResultContentBlock(
                tool_name=tool_call.name,
                tool_id=tool_call.id or str(uuid4()),
                result=tool_call.result
            ))
        
        # Create the CommonChatMessage
        return CommonChatMessage(
            id=tool_call.id or str(uuid4()),
            role=MessageRole.TOOL,
            content=content_blocks,
            created_at=datetime.fromisoformat(tool_call.timestamp) if tool_call.timestamp else datetime.now()
        )
    
    @staticmethod
    def common_chat_to_message_event(common_message: CommonChatMessage) -> Optional[MessageEvent]:
        """Convert a CommonChatMessage to MessageEvent if possible.
        
        This will only work for messages that have text content and are not tool messages.
        
        Args:
            common_message (CommonChatMessage): The common chat message to convert
            
        Returns:
            Optional[MessageEvent]: The converted message event or None if not convertible
        """
        # Skip tool messages
        if common_message.role == MessageRole.TOOL:
            return None
        
        # Get text content
        text_content = common_message.text_content
        
        # Create MessageEvent
        return MessageEvent(
            id=common_message.id,
            role=common_message.role.value,
            content=text_content,
            timestamp=common_message.created_at.isoformat()
        )
    
    @staticmethod
    def common_chat_to_tool_call_event(common_message: CommonChatMessage) -> Optional[ToolCallEvent]:
        """Convert a CommonChatMessage to ToolCallEvent if possible.
        
        This will only work for messages that have tool use content.
        
        Args:
            common_message (CommonChatMessage): The common chat message to convert
            
        Returns:
            Optional[ToolCallEvent]: The converted tool call event or None if not convertible
        """
        # Must be a tool message
        if common_message.role != MessageRole.TOOL:
            return None
        
        # Find tool use blocks
        tool_use_blocks = [block for block in common_message.content 
                         if block.type == ContentBlockType.TOOL_USE]
        
        if not tool_use_blocks:
            return None
        
        # Use the first tool use block
        tool_block = tool_use_blocks[0]
        
        # Find tool result if available
        result = None
        tool_result_blocks = [block for block in common_message.content 
                            if block.type == ContentBlockType.TOOL_RESULT 
                            and getattr(block, 'tool_id', None) == tool_block.tool_id]
        
        if tool_result_blocks:
            result = tool_result_blocks[0].result
        
        # Create ToolCallEvent
        return ToolCallEvent(
            id=tool_block.tool_id,
            name=tool_block.tool_name,
            parameters=tool_block.parameters,
            result=result,
            timestamp=common_message.created_at.isoformat()
        )