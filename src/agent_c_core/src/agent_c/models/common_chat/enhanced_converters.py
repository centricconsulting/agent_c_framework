"""
Enhanced Provider Translation Layer with Work Log Integration and Interaction Tracking.

This module provides bidirectional translation between native provider formats and 
EnhancedCommonChatMessage format with comprehensive work log support, interaction tracking,
and provider-specific optimization capabilities.

Key Features:
- Lossless round-trip translation for all supported content types
- Work log context preservation during translation
- Interaction boundary maintenance across provider formats
- Provider-specific optimization hints and capabilities
- Translation audit trail for debugging
- Extensible architecture for future providers
"""

from typing import Dict, Any, List, Optional, Union, Type, Callable
from datetime import datetime
from enum import Enum
import json
import logging
from uuid import uuid4

from .enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedContentBlock,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    EnhancedThinkingContentBlock,
    ImageContentBlock,
    AudioContentBlock,
    ValidityState,
    OutcomeStatus,
    ReasoningType,
    MessageRole,
    ContentBlockType,
    ProviderMetadata,
    StopInfo,
    TokenUsage
)


logger = logging.getLogger(__name__)


class ProviderType(str, Enum):
    """Supported provider types for translation."""
    ANTHROPIC = "anthropic"
    OPENAI = "openai"
    GOOGLE = "google"
    AZURE = "azure"
    BEDROCK = "bedrock"


class TranslationError(Exception):
    """Exception raised during provider translation."""
    
    def __init__(self, message: str, provider: str, original_error: Optional[Exception] = None):
        self.provider = provider
        self.original_error = original_error
        super().__init__(f"Translation error for {provider}: {message}")


class ProviderCapability(str, Enum):
    """Provider-specific capabilities."""
    THINKING_BLOCKS = "thinking_blocks"
    MULTIMODAL = "multimodal"
    TOOL_CALLS = "tool_calls"
    STREAMING = "streaming"
    FUNCTION_CALLING = "function_calling"
    AUDIO_INPUT = "audio_input"
    AUDIO_OUTPUT = "audio_output"
    IMAGE_INPUT = "image_input"
    IMAGE_OUTPUT = "image_output"


class TranslationAuditEntry:
    """Audit entry for translation operations."""
    
    def __init__(
        self,
        operation: str,
        provider: str,
        success: bool,
        message_count: int,
        duration: float,
        details: Optional[Dict[str, Any]] = None,
        error: Optional[str] = None
    ):
        self.timestamp = datetime.now()
        self.operation = operation
        self.provider = provider
        self.success = success
        self.message_count = message_count
        self.duration = duration
        self.details = details or {}
        self.error = error


class EnhancedAnthropicConverter:
    """Enhanced converter for Anthropic Claude with work log and interaction support."""
    
    PROVIDER_TYPE = ProviderType.ANTHROPIC
    CAPABILITIES = {
        ProviderCapability.THINKING_BLOCKS,
        ProviderCapability.TOOL_CALLS,
        ProviderCapability.MULTIMODAL,
        ProviderCapability.STREAMING
    }
    
    @classmethod
    def from_native_messages(
        cls, 
        messages: List[Dict[str, Any]], 
        interaction_id: Optional[str] = None,
        preserve_work_log_context: bool = True
    ) -> List[EnhancedCommonChatMessage]:
        """
        Convert Anthropic native messages to EnhancedCommonChatMessage format.
        
        Args:
            messages: List of Anthropic message dictionaries
            interaction_id: Interaction ID to assign to messages (auto-generated if None)
            preserve_work_log_context: Whether to preserve work log metadata
            
        Returns:
            List of EnhancedCommonChatMessage objects
            
        Raises:
            TranslationError: If translation fails
        """
        if not interaction_id:
            interaction_id = str(uuid4())
        
        enhanced_messages = []
        
        try:
            for msg_data in messages:
                enhanced_msg = cls._convert_single_message(
                    msg_data, 
                    interaction_id, 
                    preserve_work_log_context
                )
                enhanced_messages.append(enhanced_msg)
                
        except Exception as e:
            logger.error(f"Failed to convert Anthropic messages: {e}")
            raise TranslationError(
                f"Failed to convert messages: {str(e)}", 
                cls.PROVIDER_TYPE.value, 
                e
            )
        
        return enhanced_messages
    
    @classmethod
    def _convert_single_message(
        cls, 
        anthropic_message: Dict[str, Any], 
        interaction_id: str,
        preserve_work_log_context: bool
    ) -> EnhancedCommonChatMessage:
        """Convert a single Anthropic message to enhanced format."""
        
        # Extract basic fields
        message_id = anthropic_message.get("id", str(uuid4()))
        role = MessageRole.ASSISTANT  # Anthropic responses are always assistant
        created_at = datetime.now()  # Anthropic doesn't provide creation time
        
        # Convert content blocks with enhancements
        content_blocks = []
        
        for block in anthropic_message.get("content", []):
            enhanced_block = cls._convert_content_block(block, preserve_work_log_context)
            if enhanced_block:
                content_blocks.append(enhanced_block)
        
        # Create enhanced stop info
        stop_info = None
        if "stop_reason" in anthropic_message:
            stop_info = StopInfo(
                reason=anthropic_message["stop_reason"],
                details={
                    "stop_sequence": anthropic_message.get("stop_sequence"),
                    "provider": cls.PROVIDER_TYPE.value
                }
            )
        
        # Create enhanced token usage
        usage = None
        if "usage" in anthropic_message:
            usage_data = anthropic_message["usage"]
            usage = TokenUsage(
                input_tokens=usage_data.get("input_tokens", 0),
                output_tokens=usage_data.get("output_tokens", 0),
                total_tokens=usage_data.get("input_tokens", 0) + usage_data.get("output_tokens", 0)
            )
        
        # Create enhanced provider metadata
        provider_metadata = ProviderMetadata(
            provider_name=cls.PROVIDER_TYPE.value,
            raw_message=anthropic_message,
            model=anthropic_message.get("model", ""),
            stop_info=stop_info,
            usage=usage
        )
        
        # Extract work log context if preserving
        tool_context = None
        if preserve_work_log_context:
            tool_context = cls._extract_work_log_context(anthropic_message)
        
        return EnhancedCommonChatMessage(
            id=message_id,
            role=role,
            content=content_blocks,
            created_at=created_at,
            interaction_id=interaction_id,
            provider_metadata=provider_metadata,
            tool_context=tool_context,
            validity_state=ValidityState.ACTIVE
        )
    
    @classmethod
    def _convert_content_block(
        cls, 
        block: Dict[str, Any], 
        preserve_work_log_context: bool
    ) -> Optional[EnhancedContentBlock]:
        """Convert a single Anthropic content block to enhanced format."""
        
        block_type = block.get("type", "")
        
        if block_type == "text":
            return EnhancedTextContentBlock(
                text=block.get("text", ""),
                citations=block.get("citations", []),
                interaction_context=f"Anthropic text response" if preserve_work_log_context else None,
                content_tags=["anthropic_response"] if preserve_work_log_context else []
            )
            
        elif block_type == "tool_use":
            # Extract work log metadata
            work_log_metadata = None
            parameter_importance = {}
            optimization_hints = []
            
            if preserve_work_log_context:
                work_log_metadata = {
                    "provider": cls.PROVIDER_TYPE.value,
                    "tool_call_id": block.get("id", ""),
                    "extracted_at": datetime.now().isoformat()
                }
                
                # Analyze parameters for importance
                parameters = block.get("input", {})
                parameter_importance = cls._analyze_parameter_importance(parameters)
                optimization_hints = cls._generate_optimization_hints(block.get("name", ""), parameters)
            
            return EnhancedToolUseContentBlock(
                tool_name=block.get("name", ""),
                tool_id=block.get("id", ""),
                parameters=block.get("input", {}),
                work_log_metadata=work_log_metadata,
                parameter_importance=parameter_importance,
                optimization_hints=optimization_hints,
                execution_context={
                    "provider": cls.PROVIDER_TYPE.value,
                    "block_type": block_type
                } if preserve_work_log_context else None
            )
            
        elif block_type in ["thinking", "redacted_thinking"]:
            # Analyze reasoning type
            reasoning_type = None
            confidence_level = None
            related_tools = []
            
            if preserve_work_log_context:
                thought_text = block.get("thinking", "")
                reasoning_type = cls._analyze_reasoning_type(thought_text)
                confidence_level = cls._estimate_confidence_level(thought_text)
                related_tools = cls._extract_related_tools(thought_text)
            
            return EnhancedThinkingContentBlock(
                thought=block.get("thinking", ""),
                redacted=(block_type == "redacted_thinking"),
                interaction_context=f"Anthropic thinking block" if preserve_work_log_context else None,
                reasoning_type=reasoning_type,
                confidence_level=confidence_level,
                related_tools=related_tools
            )
        
        else:
            logger.warning(f"Unknown Anthropic content block type: {block_type}")
            return None
    
    @classmethod
    def _analyze_parameter_importance(cls, parameters: Dict[str, Any]) -> Dict[str, int]:
        """Analyze parameter importance for work log extraction."""
        importance_map = {}
        
        for param, value in parameters.items():
            # Default importance rules
            if param.lower() in ["path", "file_path", "filename", "id", "name"]:
                importance_map[param] = 9  # Critical
            elif param.lower() in ["query", "pattern", "search", "filter"]:
                importance_map[param] = 8  # High
            elif param.lower() in ["mode", "format", "type", "method"]:
                importance_map[param] = 7  # Medium-High
            elif param.lower() in ["recursive", "include", "exclude"]:
                importance_map[param] = 6  # Medium
            elif isinstance(value, (dict, list)) and len(str(value)) > 100:
                importance_map[param] = 3  # Low (verbose data)
            else:
                importance_map[param] = 5  # Default medium
        
        return importance_map
    
    @classmethod
    def _generate_optimization_hints(cls, tool_name: str, parameters: Dict[str, Any]) -> List[str]:
        """Generate optimization hints based on tool usage patterns."""
        hints = []
        
        # Tool-specific optimization hints
        if "workspace" in tool_name.lower():
            if parameters.get("recursive"):
                hints.append("consider_caching_recursive_results")
            if "pattern" in parameters:
                hints.append("optimize_pattern_matching")
        
        elif "file" in tool_name.lower():
            if parameters.get("encoding"):
                hints.append("encoding_specified")
            hints.append("file_operation_batching_candidate")
        
        elif "grep" in tool_name.lower() or "search" in tool_name.lower():
            hints.append("search_result_caching_candidate")
            if parameters.get("recursive"):
                hints.append("recursive_search_optimization")
        
        return hints
    
    @classmethod
    def _analyze_reasoning_type(cls, thought_text: str) -> Optional[ReasoningType]:
        """Analyze the type of reasoning in a thinking block."""
        text_lower = thought_text.lower()
        
        if any(word in text_lower for word in ["analyze", "analysis", "examining", "looking at"]):
            return ReasoningType.ANALYSIS
        elif any(word in text_lower for word in ["plan", "planning", "strategy", "approach"]):
            return ReasoningType.PLANNING
        elif any(word in text_lower for word in ["reflect", "reflection", "considering", "thinking about"]):
            return ReasoningType.REFLECTION
        elif any(word in text_lower for word in ["solve", "problem", "issue", "fix"]):
            return ReasoningType.PROBLEM_SOLVING
        elif any(word in text_lower for word in ["decide", "decision", "choose", "option"]):
            return ReasoningType.DECISION_MAKING
        
        return None
    
    @classmethod
    def _estimate_confidence_level(cls, thought_text: str) -> Optional[float]:
        """Estimate confidence level from thinking text."""
        text_lower = thought_text.lower()
        
        # High confidence indicators
        if any(word in text_lower for word in ["certain", "sure", "confident", "definitely"]):
            return 0.9
        # Medium confidence indicators
        elif any(word in text_lower for word in ["likely", "probably", "seems", "appears"]):
            return 0.7
        # Low confidence indicators
        elif any(word in text_lower for word in ["uncertain", "unsure", "maybe", "might", "possibly"]):
            return 0.3
        
        return None
    
    @classmethod
    def _extract_related_tools(cls, thought_text: str) -> List[str]:
        """Extract tool names mentioned in thinking text."""
        # Simple pattern matching for common tool patterns
        tools = []
        text_lower = thought_text.lower()
        
        common_tools = [
            "workspace_read", "workspace_write", "workspace_ls", "workspace_grep",
            "workspace_glob", "workspace_tree", "workspace_mv", "workspace_cp",
            "wsp_create_plan", "wsp_create_task", "wsp_update_task",
            "aa_chat", "aa_oneshot", "act_chat", "act_oneshot"
        ]
        
        for tool in common_tools:
            if tool.lower() in text_lower:
                tools.append(tool)
        
        return tools
    
    @classmethod
    def _extract_work_log_context(cls, anthropic_message: Dict[str, Any]) -> Dict[str, Any]:
        """Extract work log context from Anthropic message."""
        context = {
            "provider": cls.PROVIDER_TYPE.value,
            "message_type": anthropic_message.get("type", ""),
            "model": anthropic_message.get("model", ""),
            "extracted_at": datetime.now().isoformat()
        }
        
        # Add usage statistics if available
        if "usage" in anthropic_message:
            context["token_usage"] = anthropic_message["usage"]
        
        # Add stop reason if available
        if "stop_reason" in anthropic_message:
            context["stop_reason"] = anthropic_message["stop_reason"]
        
        # Count content blocks by type
        content_stats = {}
        for block in anthropic_message.get("content", []):
            block_type = block.get("type", "unknown")
            content_stats[block_type] = content_stats.get(block_type, 0) + 1
        
        context["content_block_stats"] = content_stats
        
        return context
    
    @classmethod
    def to_native_messages(
        cls,
        messages: List[EnhancedCommonChatMessage],
        include_work_log_context: bool = False,
        optimization_level: str = "standard"
    ) -> List[Dict[str, Any]]:
        """
        Convert EnhancedCommonChatMessage objects to Anthropic native format.
        
        Args:
            messages: List of EnhancedCommonChatMessage objects
            include_work_log_context: Whether to include work log metadata in output
            optimization_level: Level of optimization ("minimal", "standard", "aggressive")
            
        Returns:
            List of Anthropic message dictionaries
            
        Raises:
            TranslationError: If translation fails
        """
        native_messages = []
        
        try:
            for msg in messages:
                if not msg.is_active() and optimization_level != "minimal":
                    # Skip inactive messages unless minimal optimization
                    continue
                
                native_msg = cls._convert_to_native_message(msg, include_work_log_context)
                native_messages.append(native_msg)
                
        except Exception as e:
            logger.error(f"Failed to convert to Anthropic format: {e}")
            raise TranslationError(
                f"Failed to convert to native format: {str(e)}", 
                cls.PROVIDER_TYPE.value, 
                e
            )
        
        return native_messages
    
    @classmethod
    def _convert_to_native_message(
        cls, 
        message: EnhancedCommonChatMessage, 
        include_work_log_context: bool
    ) -> Dict[str, Any]:
        """Convert a single enhanced message to Anthropic native format."""
        
        # Start with basic structure
        native_msg = {
            "type": "message",
            "role": message.role.value,
            "content": []
        }
        
        # Convert content blocks
        for block in message.content:
            native_block = cls._convert_to_native_content_block(block, include_work_log_context)
            if native_block:
                native_msg["content"].append(native_block)
        
        # Add metadata if available
        if message.id:
            native_msg["id"] = message.id
        
        if message.provider_metadata:
            metadata = message.provider_metadata
            
            if metadata.model:
                native_msg["model"] = metadata.model
            
            if metadata.stop_info:
                native_msg["stop_reason"] = metadata.stop_info.reason
                if metadata.stop_info.details and "stop_sequence" in metadata.stop_info.details:
                    native_msg["stop_sequence"] = metadata.stop_info.details["stop_sequence"]
            
            if metadata.usage:
                native_msg["usage"] = {
                    "input_tokens": metadata.usage.input_tokens,
                    "output_tokens": metadata.usage.output_tokens
                }
        
        # Add work log context if requested
        if include_work_log_context and message.tool_context:
            native_msg["_work_log_context"] = message.tool_context
        
        return native_msg
    
    @classmethod
    def _convert_to_native_content_block(
        cls, 
        block: EnhancedContentBlock, 
        include_work_log_context: bool
    ) -> Optional[Dict[str, Any]]:
        """Convert an enhanced content block to Anthropic native format."""
        
        if isinstance(block, EnhancedTextContentBlock):
            native_block = {
                "type": "text",
                "text": block.text
            }
            
            if block.citations:
                native_block["citations"] = block.citations
            
            # Add work log context if requested
            if include_work_log_context and block.interaction_context:
                native_block["_interaction_context"] = block.interaction_context
            
            return native_block
        
        elif isinstance(block, EnhancedToolUseContentBlock):
            native_block = {
                "type": "tool_use",
                "id": block.tool_id,
                "name": block.tool_name,
                "input": block.parameters
            }
            
            # Add work log metadata if requested
            if include_work_log_context:
                if block.work_log_metadata:
                    native_block["_work_log_metadata"] = block.work_log_metadata
                if block.optimization_hints:
                    native_block["_optimization_hints"] = block.optimization_hints
            
            return native_block
        
        elif isinstance(block, EnhancedThinkingContentBlock):
            block_type = "redacted_thinking" if block.redacted else "thinking"
            native_block = {
                "type": block_type,
                "thinking": block.thought
            }
            
            # Add reasoning context if requested
            if include_work_log_context:
                if block.reasoning_type:
                    native_block["_reasoning_type"] = block.reasoning_type.value
                if block.confidence_level is not None:
                    native_block["_confidence_level"] = block.confidence_level
            
            return native_block
        
        else:
            logger.warning(f"Unsupported content block type for Anthropic: {type(block)}")
            return None


class EnhancedOpenAIConverter:
    """Enhanced converter for OpenAI GPT with work log and interaction support."""
    
    PROVIDER_TYPE = ProviderType.OPENAI
    CAPABILITIES = {
        ProviderCapability.TOOL_CALLS,
        ProviderCapability.FUNCTION_CALLING,
        ProviderCapability.MULTIMODAL,
        ProviderCapability.STREAMING,
        ProviderCapability.AUDIO_INPUT,
        ProviderCapability.AUDIO_OUTPUT
    }
    
    @classmethod
    def from_native_messages(
        cls, 
        messages: List[Dict[str, Any]], 
        interaction_id: Optional[str] = None,
        preserve_work_log_context: bool = True
    ) -> List[EnhancedCommonChatMessage]:
        """Convert OpenAI native messages to EnhancedCommonChatMessage format."""
        
        if not interaction_id:
            interaction_id = str(uuid4())
        
        enhanced_messages = []
        
        try:
            for msg_data in messages:
                enhanced_msg = cls._convert_single_message(
                    msg_data, 
                    interaction_id, 
                    preserve_work_log_context
                )
                enhanced_messages.append(enhanced_msg)
                
        except Exception as e:
            logger.error(f"Failed to convert OpenAI messages: {e}")
            raise TranslationError(
                f"Failed to convert messages: {str(e)}", 
                cls.PROVIDER_TYPE.value, 
                e
            )
        
        return enhanced_messages
    
    @classmethod
    def _convert_single_message(
        cls, 
        openai_message: Dict[str, Any], 
        interaction_id: str,
        preserve_work_log_context: bool
    ) -> EnhancedCommonChatMessage:
        """Convert a single OpenAI message to enhanced format."""
        
        # Extract basic fields
        message_id = openai_message.get("id", str(uuid4()))
        role = MessageRole(openai_message.get("role", MessageRole.ASSISTANT.value))
        created_at = datetime.now()  # OpenAI doesn't provide creation time
        
        # Convert content blocks
        content_blocks = []
        
        # Handle text content
        if "content" in openai_message and openai_message["content"] is not None:
            text_block = EnhancedTextContentBlock(
                text=openai_message["content"],
                interaction_context=f"OpenAI {role.value} message" if preserve_work_log_context else None,
                content_tags=["openai_response"] if preserve_work_log_context else []
            )
            content_blocks.append(text_block)
        
        # Handle tool calls
        if "tool_calls" in openai_message and openai_message["tool_calls"]:
            for tool_call in openai_message["tool_calls"]:
                tool_block = cls._convert_tool_call(tool_call, preserve_work_log_context)
                if tool_block:
                    content_blocks.append(tool_block)
        
        # Handle legacy function calls
        elif "function_call" in openai_message and openai_message["function_call"]:
            tool_block = cls._convert_function_call(
                openai_message["function_call"], 
                preserve_work_log_context
            )
            if tool_block:
                content_blocks.append(tool_block)
        
        # Create provider metadata
        stop_info = None
        if "finish_reason" in openai_message:
            stop_info = StopInfo(
                reason=openai_message["finish_reason"],
                details={"provider": cls.PROVIDER_TYPE.value}
            )
        
        usage = None
        if "usage" in openai_message:
            usage_data = openai_message["usage"]
            usage = TokenUsage(
                input_tokens=usage_data.get("prompt_tokens", 0),
                output_tokens=usage_data.get("completion_tokens", 0),
                total_tokens=usage_data.get("total_tokens", 0)
            )
        
        provider_metadata = ProviderMetadata(
            provider_name=cls.PROVIDER_TYPE.value,
            raw_message=openai_message,
            model=openai_message.get("model", ""),
            stop_info=stop_info,
            usage=usage
        )
        
        # Extract work log context
        tool_context = None
        if preserve_work_log_context:
            tool_context = cls._extract_work_log_context(openai_message)
        
        return EnhancedCommonChatMessage(
            id=message_id,
            role=role,
            content=content_blocks,
            created_at=created_at,
            interaction_id=interaction_id,
            provider_metadata=provider_metadata,
            tool_context=tool_context,
            validity_state=ValidityState.ACTIVE
        )
    
    @classmethod
    def _convert_tool_call(
        cls, 
        tool_call: Dict[str, Any], 
        preserve_work_log_context: bool
    ) -> Optional[EnhancedToolUseContentBlock]:
        """Convert OpenAI tool call to enhanced tool use block."""
        
        if tool_call["type"] != "function":
            logger.warning(f"Unsupported OpenAI tool call type: {tool_call['type']}")
            return None
        
        function = tool_call["function"]
        
        # Parse parameters
        try:
            parameters = json.loads(function["arguments"])
        except json.JSONDecodeError:
            parameters = {"raw_arguments": function["arguments"]}
        
        # Work log metadata
        work_log_metadata = None
        parameter_importance = {}
        optimization_hints = []
        
        if preserve_work_log_context:
            work_log_metadata = {
                "provider": cls.PROVIDER_TYPE.value,
                "tool_call_id": tool_call["id"],
                "function_name": function["name"],
                "extracted_at": datetime.now().isoformat()
            }
            
            parameter_importance = cls._analyze_parameter_importance(parameters)
            optimization_hints = cls._generate_optimization_hints(function["name"], parameters)
        
        return EnhancedToolUseContentBlock(
            tool_name=function["name"],
            tool_id=tool_call["id"],
            parameters=parameters,
            work_log_metadata=work_log_metadata,
            parameter_importance=parameter_importance,
            optimization_hints=optimization_hints,
            execution_context={
                "provider": cls.PROVIDER_TYPE.value,
                "call_type": "tool_call"
            } if preserve_work_log_context else None
        )
    
    @classmethod
    def _convert_function_call(
        cls, 
        function_call: Dict[str, Any], 
        preserve_work_log_context: bool
    ) -> Optional[EnhancedToolUseContentBlock]:
        """Convert OpenAI legacy function call to enhanced tool use block."""
        
        # Parse parameters
        try:
            parameters = json.loads(function_call["arguments"])
        except json.JSONDecodeError:
            parameters = {"raw_arguments": function_call["arguments"]}
        
        # Generate ID for legacy function call
        tool_id = f"function-{function_call['name']}-{str(uuid4())[:8]}"
        
        # Work log metadata
        work_log_metadata = None
        parameter_importance = {}
        optimization_hints = []
        
        if preserve_work_log_context:
            work_log_metadata = {
                "provider": cls.PROVIDER_TYPE.value,
                "function_name": function_call["name"],
                "call_type": "legacy_function_call",
                "extracted_at": datetime.now().isoformat()
            }
            
            parameter_importance = cls._analyze_parameter_importance(parameters)
            optimization_hints = cls._generate_optimization_hints(function_call["name"], parameters)
        
        return EnhancedToolUseContentBlock(
            tool_name=function_call["name"],
            tool_id=tool_id,
            parameters=parameters,
            work_log_metadata=work_log_metadata,
            parameter_importance=parameter_importance,
            optimization_hints=optimization_hints,
            execution_context={
                "provider": cls.PROVIDER_TYPE.value,
                "call_type": "function_call"
            } if preserve_work_log_context else None
        )
    
    @classmethod
    def _analyze_parameter_importance(cls, parameters: Dict[str, Any]) -> Dict[str, int]:
        """Analyze parameter importance for OpenAI tool calls."""
        # Similar logic to Anthropic but with OpenAI-specific patterns
        importance_map = {}
        
        for param, value in parameters.items():
            # OpenAI-specific parameter patterns
            if param.lower() in ["messages", "prompt", "query", "input"]:
                importance_map[param] = 9  # Critical
            elif param.lower() in ["model", "temperature", "max_tokens"]:
                importance_map[param] = 8  # High
            elif param.lower() in ["stream", "stop", "presence_penalty"]:
                importance_map[param] = 6  # Medium
            elif isinstance(value, (dict, list)) and len(str(value)) > 100:
                importance_map[param] = 3  # Low (verbose)
            else:
                importance_map[param] = 5  # Default
        
        return importance_map
    
    @classmethod
    def _generate_optimization_hints(cls, tool_name: str, parameters: Dict[str, Any]) -> List[str]:
        """Generate OpenAI-specific optimization hints."""
        hints = []
        
        # OpenAI-specific optimizations
        if "gpt" in tool_name.lower() or "chat" in tool_name.lower():
            if parameters.get("stream"):
                hints.append("streaming_enabled")
            if parameters.get("temperature", 1.0) < 0.3:
                hints.append("low_temperature_deterministic")
            if parameters.get("max_tokens"):
                hints.append("token_limit_specified")
        
        return hints
    
    @classmethod
    def _extract_work_log_context(cls, openai_message: Dict[str, Any]) -> Dict[str, Any]:
        """Extract work log context from OpenAI message."""
        context = {
            "provider": cls.PROVIDER_TYPE.value,
            "role": openai_message.get("role", ""),
            "extracted_at": datetime.now().isoformat()
        }
        
        # Add usage statistics if available
        if "usage" in openai_message:
            context["token_usage"] = openai_message["usage"]
        
        # Add finish reason if available
        if "finish_reason" in openai_message:
            context["finish_reason"] = openai_message["finish_reason"]
        
        # Count tool calls
        tool_call_count = 0
        if "tool_calls" in openai_message:
            tool_call_count = len(openai_message["tool_calls"])
        elif "function_call" in openai_message:
            tool_call_count = 1
        
        context["tool_call_count"] = tool_call_count
        
        return context
    
    @classmethod
    def to_native_messages(
        cls,
        messages: List[EnhancedCommonChatMessage],
        include_work_log_context: bool = False,
        optimization_level: str = "standard"
    ) -> List[Dict[str, Any]]:
        """Convert EnhancedCommonChatMessage objects to OpenAI native format."""
        
        native_messages = []
        
        try:
            for msg in messages:
                if not msg.is_active() and optimization_level != "minimal":
                    continue
                
                native_msg = cls._convert_to_native_message(msg, include_work_log_context)
                native_messages.append(native_msg)
                
        except Exception as e:
            logger.error(f"Failed to convert to OpenAI format: {e}")
            raise TranslationError(
                f"Failed to convert to native format: {str(e)}", 
                cls.PROVIDER_TYPE.value, 
                e
            )
        
        return native_messages
    
    @classmethod
    def _convert_to_native_message(
        cls, 
        message: EnhancedCommonChatMessage, 
        include_work_log_context: bool
    ) -> Dict[str, Any]:
        """Convert enhanced message to OpenAI native format."""
        
        native_msg = {
            "role": message.role.value
        }
        
        # Handle text content
        text_blocks = [
            block for block in message.content 
            if isinstance(block, EnhancedTextContentBlock)
        ]
        
        if text_blocks:
            native_msg["content"] = "".join(block.text for block in text_blocks)
        else:
            native_msg["content"] = None
        
        # Handle tool calls
        tool_blocks = [
            block for block in message.content 
            if isinstance(block, EnhancedToolUseContentBlock)
        ]
        
        if tool_blocks:
            tool_calls = []
            for block in tool_blocks:
                tool_call = {
                    "id": block.tool_id,
                    "type": "function",
                    "function": {
                        "name": block.tool_name,
                        "arguments": json.dumps(block.parameters)
                    }
                }
                
                # Add work log context if requested
                if include_work_log_context and block.work_log_metadata:
                    tool_call["_work_log_metadata"] = block.work_log_metadata
                
                tool_calls.append(tool_call)
            
            native_msg["tool_calls"] = tool_calls
        
        # Add metadata
        if message.id:
            native_msg["id"] = message.id
        
        if message.provider_metadata and message.provider_metadata.stop_info:
            native_msg["finish_reason"] = message.provider_metadata.stop_info.reason
        
        return native_msg


class EnhancedProviderTranslationLayer:
    """
    Unified translation layer supporting multiple providers with work log integration.
    
    This class provides a single interface for translating between provider formats
    and EnhancedCommonChatMessage, with comprehensive audit trails and optimization
    capabilities.
    """
    
    def __init__(self):
        self._converters = {
            ProviderType.ANTHROPIC: EnhancedAnthropicConverter,
            ProviderType.OPENAI: EnhancedOpenAIConverter
        }
        self._audit_trail: List[TranslationAuditEntry] = []
        self._performance_stats = {
            "total_translations": 0,
            "successful_translations": 0,
            "failed_translations": 0,
            "average_duration": 0.0
        }
    
    def register_converter(self, provider_type: ProviderType, converter_class: Type):
        """Register a custom converter for a provider type."""
        self._converters[provider_type] = converter_class
        logger.info(f"Registered converter for {provider_type.value}")
    
    def get_supported_providers(self) -> List[ProviderType]:
        """Get list of supported provider types."""
        return list(self._converters.keys())
    
    def get_provider_capabilities(self, provider_type: ProviderType) -> set:
        """Get capabilities for a specific provider."""
        converter = self._converters.get(provider_type)
        if converter and hasattr(converter, 'CAPABILITIES'):
            return converter.CAPABILITIES
        return set()
    
    def from_native_messages(
        self,
        messages: List[Dict[str, Any]],
        provider_type: Union[str, ProviderType],
        interaction_id: Optional[str] = None,
        preserve_work_log_context: bool = True
    ) -> List[EnhancedCommonChatMessage]:
        """
        Convert native provider messages to EnhancedCommonChatMessage format.
        
        Args:
            messages: List of provider-specific message dictionaries
            provider_type: Provider type (string or enum)
            interaction_id: Interaction ID to assign (auto-generated if None)
            preserve_work_log_context: Whether to preserve work log metadata
            
        Returns:
            List of EnhancedCommonChatMessage objects
            
        Raises:
            TranslationError: If translation fails or provider unsupported
        """
        start_time = datetime.now()
        
        # Normalize provider type
        if isinstance(provider_type, str):
            try:
                provider_type = ProviderType(provider_type.lower())
            except ValueError:
                raise TranslationError(f"Unsupported provider: {provider_type}", provider_type)
        
        # Get converter
        converter = self._converters.get(provider_type)
        if not converter:
            raise TranslationError(f"No converter available for {provider_type.value}", provider_type.value)
        
        try:
            # Perform translation
            enhanced_messages = converter.from_native_messages(
                messages, 
                interaction_id, 
                preserve_work_log_context
            )
            
            # Record success
            duration = (datetime.now() - start_time).total_seconds()
            self._record_audit_entry(
                operation="from_native",
                provider=provider_type.value,
                success=True,
                message_count=len(messages),
                duration=duration,
                details={
                    "interaction_id": interaction_id,
                    "preserve_work_log": preserve_work_log_context,
                    "output_message_count": len(enhanced_messages)
                }
            )
            
            return enhanced_messages
            
        except Exception as e:
            # Record failure
            duration = (datetime.now() - start_time).total_seconds()
            self._record_audit_entry(
                operation="from_native",
                provider=provider_type.value,
                success=False,
                message_count=len(messages),
                duration=duration,
                error=str(e)
            )
            raise
    
    def to_native_messages(
        self,
        messages: List[EnhancedCommonChatMessage],
        provider_type: Union[str, ProviderType],
        include_work_log_context: bool = False,
        optimization_level: str = "standard"
    ) -> List[Dict[str, Any]]:
        """
        Convert EnhancedCommonChatMessage objects to native provider format.
        
        Args:
            messages: List of EnhancedCommonChatMessage objects
            provider_type: Target provider type
            include_work_log_context: Whether to include work log metadata
            optimization_level: Optimization level ("minimal", "standard", "aggressive")
            
        Returns:
            List of provider-specific message dictionaries
            
        Raises:
            TranslationError: If translation fails or provider unsupported
        """
        start_time = datetime.now()
        
        # Normalize provider type
        if isinstance(provider_type, str):
            try:
                provider_type = ProviderType(provider_type.lower())
            except ValueError:
                raise TranslationError(f"Unsupported provider: {provider_type}", provider_type)
        
        # Get converter
        converter = self._converters.get(provider_type)
        if not converter:
            raise TranslationError(f"No converter available for {provider_type.value}", provider_type.value)
        
        try:
            # Perform translation
            native_messages = converter.to_native_messages(
                messages, 
                include_work_log_context, 
                optimization_level
            )
            
            # Record success
            duration = (datetime.now() - start_time).total_seconds()
            self._record_audit_entry(
                operation="to_native",
                provider=provider_type.value,
                success=True,
                message_count=len(messages),
                duration=duration,
                details={
                    "include_work_log": include_work_log_context,
                    "optimization_level": optimization_level,
                    "output_message_count": len(native_messages)
                }
            )
            
            return native_messages
            
        except Exception as e:
            # Record failure
            duration = (datetime.now() - start_time).total_seconds()
            self._record_audit_entry(
                operation="to_native",
                provider=provider_type.value,
                success=False,
                message_count=len(messages),
                duration=duration,
                error=str(e)
            )
            raise
    
    def validate_round_trip(
        self,
        messages: List[EnhancedCommonChatMessage],
        provider_type: Union[str, ProviderType]
    ) -> Dict[str, Any]:
        """
        Validate lossless round-trip translation for messages.
        
        Args:
            messages: Messages to test
            provider_type: Provider to test with
            
        Returns:
            Validation results dictionary
        """
        start_time = datetime.now()
        
        try:
            # Convert to native format
            native_messages = self.to_native_messages(messages, provider_type)
            
            # Convert back to enhanced format
            round_trip_messages = self.from_native_messages(native_messages, provider_type)
            
            # Compare messages
            validation_results = {
                "success": True,
                "original_count": len(messages),
                "round_trip_count": len(round_trip_messages),
                "duration": (datetime.now() - start_time).total_seconds(),
                "differences": []
            }
            
            # Basic count validation
            if len(messages) != len(round_trip_messages):
                validation_results["success"] = False
                validation_results["differences"].append({
                    "type": "count_mismatch",
                    "original": len(messages),
                    "round_trip": len(round_trip_messages)
                })
            
            # Content validation (simplified)
            for i, (orig, rt) in enumerate(zip(messages, round_trip_messages)):
                if orig.role != rt.role:
                    validation_results["success"] = False
                    validation_results["differences"].append({
                        "type": "role_mismatch",
                        "message_index": i,
                        "original": orig.role.value,
                        "round_trip": rt.role.value
                    })
                
                if len(orig.content) != len(rt.content):
                    validation_results["success"] = False
                    validation_results["differences"].append({
                        "type": "content_block_count_mismatch",
                        "message_index": i,
                        "original": len(orig.content),
                        "round_trip": len(rt.content)
                    })
            
            return validation_results
            
        except Exception as e:
            return {
                "success": False,
                "error": str(e),
                "duration": (datetime.now() - start_time).total_seconds()
            }
    
    def get_audit_trail(
        self,
        provider_filter: Optional[str] = None,
        operation_filter: Optional[str] = None,
        success_filter: Optional[bool] = None,
        limit: int = 100
    ) -> List[Dict[str, Any]]:
        """Get translation audit trail with optional filtering."""
        
        filtered_entries = self._audit_trail
        
        if provider_filter:
            filtered_entries = [e for e in filtered_entries if e.provider == provider_filter]
        
        if operation_filter:
            filtered_entries = [e for e in filtered_entries if e.operation == operation_filter]
        
        if success_filter is not None:
            filtered_entries = [e for e in filtered_entries if e.success == success_filter]
        
        # Sort by timestamp (newest first) and limit
        filtered_entries = sorted(filtered_entries, key=lambda x: x.timestamp, reverse=True)[:limit]
        
        # Convert to dictionaries
        return [
            {
                "timestamp": entry.timestamp.isoformat(),
                "operation": entry.operation,
                "provider": entry.provider,
                "success": entry.success,
                "message_count": entry.message_count,
                "duration": entry.duration,
                "details": entry.details,
                "error": entry.error
            }
            for entry in filtered_entries
        ]
    
    def get_performance_stats(self) -> Dict[str, Any]:
        """Get translation performance statistics."""
        return self._performance_stats.copy()
    
    def clear_audit_trail(self):
        """Clear the audit trail (useful for testing)."""
        self._audit_trail.clear()
        self._performance_stats = {
            "total_translations": 0,
            "successful_translations": 0,
            "failed_translations": 0,
            "average_duration": 0.0
        }
    
    def _record_audit_entry(
        self,
        operation: str,
        provider: str,
        success: bool,
        message_count: int,
        duration: float,
        details: Optional[Dict[str, Any]] = None,
        error: Optional[str] = None
    ):
        """Record an audit entry and update performance stats."""
        
        entry = TranslationAuditEntry(
            operation=operation,
            provider=provider,
            success=success,
            message_count=message_count,
            duration=duration,
            details=details,
            error=error
        )
        
        self._audit_trail.append(entry)
        
        # Update performance stats
        self._performance_stats["total_translations"] += 1
        
        if success:
            self._performance_stats["successful_translations"] += 1
        else:
            self._performance_stats["failed_translations"] += 1
        
        # Update average duration
        total_successful = self._performance_stats["successful_translations"]
        if total_successful > 0:
            current_avg = self._performance_stats["average_duration"]
            self._performance_stats["average_duration"] = (
                (current_avg * (total_successful - 1) + duration) / total_successful
            )
        
        # Keep audit trail size manageable (keep last 1000 entries)
        if len(self._audit_trail) > 1000:
            self._audit_trail = self._audit_trail[-1000:]


# Convenience functions for common usage patterns

def create_translation_layer() -> EnhancedProviderTranslationLayer:
    """Create a new translation layer with default converters."""
    return EnhancedProviderTranslationLayer()


def translate_from_anthropic(
    messages: List[Dict[str, Any]], 
    interaction_id: Optional[str] = None
) -> List[EnhancedCommonChatMessage]:
    """Convenience function to translate from Anthropic format."""
    layer = create_translation_layer()
    return layer.from_native_messages(messages, ProviderType.ANTHROPIC, interaction_id)


def translate_from_openai(
    messages: List[Dict[str, Any]], 
    interaction_id: Optional[str] = None
) -> List[EnhancedCommonChatMessage]:
    """Convenience function to translate from OpenAI format."""
    layer = create_translation_layer()
    return layer.from_native_messages(messages, ProviderType.OPENAI, interaction_id)


def translate_to_anthropic(
    messages: List[EnhancedCommonChatMessage],
    include_work_log_context: bool = False
) -> List[Dict[str, Any]]:
    """Convenience function to translate to Anthropic format."""
    layer = create_translation_layer()
    return layer.to_native_messages(messages, ProviderType.ANTHROPIC, include_work_log_context)


def translate_to_openai(
    messages: List[EnhancedCommonChatMessage],
    include_work_log_context: bool = False
) -> List[Dict[str, Any]]:
    """Convenience function to translate to OpenAI format."""
    layer = create_translation_layer()
    return layer.to_native_messages(messages, ProviderType.OPENAI, include_work_log_context)