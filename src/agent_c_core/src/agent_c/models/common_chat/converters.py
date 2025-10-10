from typing import Dict, Any, List, cast
from datetime import datetime
import json

from .models import (
    CommonChatMessage,
    ContentBlock,
    TextContentBlock,
    ToolUseContentBlock,
    ToolResultContentBlock,
    ThinkingContentBlock,
    MessageRole,
    ContentBlockType,
    ProviderMetadata,
    StopInfo,
    TokenUsage
)


class AnthropicConverter:
    """Converter between CommonChatMessage and Anthropic message format.
    
    Anthropic Message Object Structure:
    - id: Unique identifier (required)
    - type: Always "message" (required)
    - role: Always "assistant" for responses (required)
    - content: List of content blocks (required)
    - model: Model used to generate the message (required)
    - usage: Token usage information (required)
    - stop_reason: Reason generation stopped (required in non-streaming mode)
    - stop_sequence: Custom stop sequence if generated (optional)
    """
    
    @staticmethod
    def from_anthropic(anthropic_message: Dict[str, Any]) -> CommonChatMessage:
        """Convert an Anthropic message to CommonChatMessage."""
        # Extract common fields
        message_id = anthropic_message.get("id", "")
        role = MessageRole.ASSISTANT  # Anthropic message responses are always from assistant
        created_at = datetime.now()  # Anthropic doesn't provide creation time
        
        # Convert content blocks
        content_blocks: List[ContentBlock] = []
        
        for block in anthropic_message.get("content", []):
            block_type = block.get("type", "")
            
            if block_type == "text":
                # Convert text block
                text_block = TextContentBlock(
                    text=block.get("text", ""),
                    citations=block.get("citations", [])
                )
                content_blocks.append(text_block)
                
            elif block_type == "tool_use":
                # Convert tool use block
                tool_use_block = ToolUseContentBlock(
                    tool_name=block.get("name", ""),
                    tool_id=block.get("id", ""),
                    parameters=block.get("input", {})
                )
                content_blocks.append(tool_use_block)
                
            elif block_type == "thinking" or block_type == "redacted_thinking":
                # Convert thinking block
                thinking_block = ThinkingContentBlock(
                    thought=block.get("thinking", ""),
                    redacted=(block_type == "redacted_thinking")
                )
                content_blocks.append(thinking_block)
        
        # Create stop info
        # In non-streaming mode, stop_reason is required
        stop_info = None
        if "stop_reason" in anthropic_message:
            stop_info = StopInfo(
                reason=anthropic_message["stop_reason"],
                details={"stop_sequence": anthropic_message.get("stop_sequence")}
            )
        
        # Create token usage
        # Usage is a required field in the Anthropic Message type
        usage = None
        if "usage" in anthropic_message:
            usage_data = anthropic_message["usage"]
            usage = TokenUsage(
                input_tokens=usage_data.get("input_tokens", 0),
                output_tokens=usage_data.get("output_tokens", 0),
                total_tokens=usage_data.get("input_tokens", 0) + usage_data.get("output_tokens", 0)
            )
        
        # Create provider metadata
        # Model is a required field in the Anthropic Message type
        provider_metadata = ProviderMetadata(
            provider_name="anthropic",
            raw_message=anthropic_message,
            model=anthropic_message.get("model", ""),
            stop_info=stop_info,
            usage=usage
        )
        
        return CommonChatMessage(
            id=message_id,
            role=role,
            content=content_blocks,
            created_at=created_at,
            provider_metadata=provider_metadata
        )
    
    @staticmethod
    def to_anthropic(common_message: CommonChatMessage) -> Dict[str, Any]:
        """Convert a CommonChatMessage to Anthropic message format."""
        # Start with basic message structure
        anthropic_message = {
            "type": "message",
            "role": common_message.role.value,
            "content": []
        }
        
        # Convert content blocks
        for block in common_message.content:
            if block.type == ContentBlockType.TEXT:
                text_block = cast(TextContentBlock, block)
                anthropic_message["content"].append({
                    "type": "text",
                    "text": text_block.text,
                    "citations": text_block.citations
                })
                
            elif block.type == ContentBlockType.TOOL_USE:
                tool_block = cast(ToolUseContentBlock, block)
                anthropic_message["content"].append({
                    "type": "tool_use",
                    "id": tool_block.tool_id,
                    "name": tool_block.tool_name,
                    "input": tool_block.parameters
                })
                
            elif block.type == ContentBlockType.THINKING:
                thinking_block = cast(ThinkingContentBlock, block)
                block_type = "redacted_thinking" if thinking_block.redacted else "thinking"
                anthropic_message["content"].append({
                    "type": block_type,
                    "thinking": thinking_block.thought
                })
        
        # Add ID if available
        if common_message.id:
            anthropic_message["id"] = common_message.id
        
        # Extract model info from provider metadata
        if common_message.provider_metadata and common_message.provider_metadata.model:
            anthropic_message["model"] = common_message.provider_metadata.model
        
        # Add stop info if available
        if common_message.provider_metadata and common_message.provider_metadata.stop_info:
            stop_info = common_message.provider_metadata.stop_info
            anthropic_message["stop_reason"] = stop_info.reason
            
            if stop_info.details and "stop_sequence" in stop_info.details:
                anthropic_message["stop_sequence"] = stop_info.details["stop_sequence"]
        
        # Add usage info if available
        if common_message.provider_metadata and common_message.provider_metadata.usage:
            usage = common_message.provider_metadata.usage
            anthropic_message["usage"] = {
                "input_tokens": usage.input_tokens,
                "output_tokens": usage.output_tokens
            }
        
        return anthropic_message


class OpenAIConverter:
    """Converter between CommonChatMessage and OpenAI message format.
    
    OpenAI ChatCompletion Message Structure:
    - role: The role of the message author (required)
    - content: The content of the message (optional, null if tool_calls present)
    - tool_calls: List of tool calls (optional)
    - function_call: Legacy function call info (optional, deprecated)
    - name: Name of the author (optional, for function/tool messages)
    """
    
    @staticmethod
    def from_openai(openai_message: Dict[str, Any]) -> CommonChatMessage:
        """Convert an OpenAI message to CommonChatMessage."""
        # Extract common fields
        message_id = openai_message.get("id", "")
        role = MessageRole(openai_message.get("role", MessageRole.ASSISTANT.value))
        created_at = datetime.now()  # OpenAI doesn't provide creation time in the message
        
        # Convert content blocks
        content_blocks: List[ContentBlock] = []
        
        # Handle text content
        if "content" in openai_message and openai_message["content"] is not None:
            text_block = TextContentBlock(text=openai_message["content"])
            content_blocks.append(text_block)
        
        # Handle tool calls
        if "tool_calls" in openai_message and openai_message["tool_calls"]:
            for tool_call in openai_message["tool_calls"]:
                if tool_call["type"] == "function":
                    function = tool_call["function"]
                    try:
                        params = json.loads(function["arguments"])
                    except json.JSONDecodeError:
                        params = {"raw_arguments": function["arguments"]}
                        
                    tool_block = ToolUseContentBlock(
                        tool_name=function["name"],
                        tool_id=tool_call["id"],
                        parameters=params
                    )
                    content_blocks.append(tool_block)
        
        # Handle legacy function_call
        elif "function_call" in openai_message and openai_message["function_call"]:
            function_call = openai_message["function_call"]
            try:
                params = json.loads(function_call["arguments"])
            except json.JSONDecodeError:
                params = {"raw_arguments": function_call["arguments"]}
                
            tool_block = ToolUseContentBlock(
                tool_name=function_call["name"],
                tool_id="function-" + function_call["name"],  # Generate an ID since OpenAI function_call doesn't have one
                parameters=params
            )
            content_blocks.append(tool_block)
        
        # Create provider metadata
        model = ""  # OpenAI message doesn't contain model info
        if "model" in openai_message:
            model = openai_message["model"]
        elif "completion" in openai_message:
            model = openai_message["completion"].get("model", "")
        
        # Create stop info
        stop_info = None
        if "finish_reason" in openai_message:
            stop_info = StopInfo(
                reason=openai_message["finish_reason"],
                details={}
            )
        
        # Create token usage
        usage = None
        if "usage" in openai_message:
            usage_data = openai_message["usage"]
            usage = TokenUsage(
                input_tokens=usage_data.get("prompt_tokens", 0),
                output_tokens=usage_data.get("completion_tokens", 0),
                total_tokens=usage_data.get("total_tokens", 0)
            )
        
        provider_metadata = ProviderMetadata(
            provider_name="openai",
            raw_message=openai_message,
            model=model,
            stop_info=stop_info,
            usage=usage
        )
        
        return CommonChatMessage(
            id=message_id,
            role=role,
            content=content_blocks,
            created_at=created_at,
            provider_metadata=provider_metadata
        )
    
    @staticmethod
    def to_openai(common_message: CommonChatMessage) -> Dict[str, Any]:
        """Convert a CommonChatMessage to OpenAI message format."""
        # Start with basic message structure
        openai_message = {
            "role": common_message.role.value,
        }
        
        # Handle text content
        text_blocks = [block for block in common_message.content if block.type == ContentBlockType.TEXT]
        if text_blocks:
            # Concatenate all text blocks
            openai_message["content"] = "".join(cast(TextContentBlock, block).text for block in text_blocks)
        else:
            openai_message["content"] = None
        
        # Handle tool use blocks
        tool_blocks = [block for block in common_message.content if block.type == ContentBlockType.TOOL_USE]
        if tool_blocks:
            tool_calls = []
            for block in tool_blocks:
                tool_block = cast(ToolUseContentBlock, block)
                tool_calls.append({
                    "id": tool_block.tool_id,
                    "type": "function",
                    "function": {
                        "name": tool_block.tool_name,
                        "arguments": json.dumps(tool_block.parameters)
                    }
                })
            openai_message["tool_calls"] = tool_calls
        
        # Add ID if available
        if common_message.id:
            openai_message["id"] = common_message.id
        
        # Extract finish reason if available (for streaming responses)
        if common_message.provider_metadata and common_message.provider_metadata.stop_info:
            openai_message["finish_reason"] = common_message.provider_metadata.stop_info.reason
        
        return openai_message


class CommonChatMessageConverter:
    """Unified converter that can handle multiple providers."""
    
    @staticmethod
    def from_provider(message: Dict[str, Any], provider: str) -> CommonChatMessage:
        """Convert a provider-specific message to CommonChatMessage."""
        if provider.lower() == "anthropic":
            return AnthropicConverter.from_anthropic(message)
        elif provider.lower() == "openai":
            return OpenAIConverter.from_openai(message)
        else:
            raise ValueError(f"Unsupported provider: {provider}")
    
    @staticmethod
    def to_provider(common_message: CommonChatMessage, provider: str) -> Dict[str, Any]:
        """Convert a CommonChatMessage to a provider-specific message."""
        if provider.lower() == "anthropic":
            return AnthropicConverter.to_anthropic(common_message)
        elif provider.lower() == "openai":
            return OpenAIConverter.to_openai(common_message)
        else:
            raise ValueError(f"Unsupported provider: {provider}")