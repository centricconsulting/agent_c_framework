"""Conversion utilities for translating between V1 and V2 API models.

This module provides functions to convert between V1 and V2 data models,
enabling backward compatibility and smooth transition between API versions.
It also includes utilities for converting between API models and core models.
"""

from typing import Dict, Any, Optional, List
from agent_c_api.api.v1.llm_models.agent_params import (
    AgentInitializationParams, 
    AgentUpdateParams
)
from agent_c.models.events.chat import MessageEvent
# Updated import to use session_models for AgentConfig
from agent_c_api.api.v2.models.session_models import (
    AgentConfig,
    SessionCreate,
    SessionUpdate
)
from agent_c_api.api.v2.models.chat_models import (
    ChatMessage,
    ChatMessageContent,
    ContentBlockUnion
)


def v1_to_v2_session_create(v1_params: AgentInitializationParams) -> SessionCreate:
    """
    Convert V1 agent initialization parameters to V2 session creation model.
    
    Args:
        v1_params: V1 agent initialization parameters
        
    Returns:
        V2 session creation model
    """
    # Extract model parameters for the parameters dict
    parameters = {}
    if v1_params.temperature is not None:
        parameters['temperature'] = v1_params.temperature
    if v1_params.max_tokens is not None:
        parameters['max_tokens'] = v1_params.max_tokens
    if v1_params.reasoning_effort is not None:
        parameters['reasoning_effort'] = v1_params.reasoning_effort
    if v1_params.budget_tokens is not None:
        parameters['budget_tokens'] = v1_params.budget_tokens
    
    # Create session creation model
    return SessionCreate(
        model_id=v1_params.model_name,
        persona_id=v1_params.agent_key or "default",
        metadata={
            "backend": v1_params.backend or "openai",
            "custom_prompt": v1_params.custom_prompt
        } if v1_params.custom_prompt else {"backend": v1_params.backend or "openai"}
    )


def v1_to_v2_session_update(v1_params: AgentUpdateParams) -> SessionUpdate:
    """
    Convert V1 agent update parameters to V2 session update model.
    
    Args:
        v1_params: V1 agent update parameters
        
    Returns:
        V2 session update model
    """
    # Build metadata updates if needed
    metadata_updates = {}
    if v1_params.custom_prompt is not None:
        metadata_updates["custom_prompt"] = v1_params.custom_prompt

    # Create session update model
    return SessionUpdate(
        name=None,  # No name update in v1
        metadata=metadata_updates if metadata_updates else None
    )


def v2_to_v1_session_params(v2_session: SessionCreate) -> AgentInitializationParams:
    """
    Convert V2 session creation model to V1 agent initialization parameters.
    
    Args:
        v2_session: V2 session creation model
        
    Returns:
        V1 agent initialization parameters
    """
    # Extract backend and custom prompt from metadata if available
    metadata = v2_session.metadata or {}
    backend = metadata.get("backend", "openai")
    custom_prompt = metadata.get("custom_prompt")
    
    return AgentInitializationParams(
        model_name=v2_session.model_id,
        backend=backend,
        agent_key=v2_session.persona_id,
        custom_prompt=custom_prompt,
        # These fields will use their default values or None
        temperature=None,
        max_tokens=None,
        reasoning_effort=None,
        budget_tokens=None,
        ui_session_id=None
    )


# Message model conversion functions

def content_blocks_to_message_content(blocks: List[ContentBlockUnion]) -> List[ChatMessageContent]:
    """
    Convert core content blocks to API message content objects.
    
    Args:
        blocks: List of core content blocks
        
    Returns:
        List of API chat message content objects
    """
    return [ChatMessageContent.from_content_block(block) for block in blocks]


def message_content_to_content_blocks(contents: List[ChatMessageContent]) -> List[ContentBlockUnion]:
    """
    Convert API message content objects to core content blocks.
    
    Args:
        contents: List of API chat message content objects
        
    Returns:
        List of core content blocks
    """
    return [content.to_content_block() for content in contents]


def message_event_to_chat_message(event: MessageEvent) -> ChatMessage:
    """
    Convert a core MessageEvent to an API ChatMessage.
    
    Args:
        event: Core message event
        
    Returns:
        API chat message
    """
    return ChatMessage.from_message_event(event)


def chat_message_to_message_event(message: ChatMessage, session_id: str) -> MessageEvent:
    """
    Convert an API ChatMessage to a core MessageEvent.
    
    Args:
        message: API chat message
        session_id: Session ID to associate with the message
        
    Returns:
        Core message event
    """
    return message.to_message_event(session_id)