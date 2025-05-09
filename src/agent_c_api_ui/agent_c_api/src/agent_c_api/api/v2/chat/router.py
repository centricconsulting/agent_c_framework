from fastapi import APIRouter, HTTPException, Depends, Path, Request, Body
from typing import Optional, Dict, List, Any, Sequence, Union

from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from .services import ChatService
from ..models.chat_models import ChatMessage, ChatRequest, ChatResponse

# Create router with prefix and tags
router = APIRouter(
    prefix="/chat",  # Prefix matches the module name
    tags=["chat"],    # Tag for grouping in OpenAPI docs
    responses={
        404: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {
                        "detail": {
                            "error": "SESSION_NOT_FOUND",
                            "error_code": "SESSION_NOT_FOUND",
                            "message": "Session not found",
                            "params": {"session_id": "example_session_id"}
                        }
                    }
                }
            }
        },
        500: {
            "description": "Internal server error",
            "content": {
                "application/json": {
                    "example": {
                        "detail": {
                            "error": "Internal server error",
                            "error_code": "SERVER_ERROR",
                            "message": "An unexpected error occurred while processing the request"
                        }
                    }
                }
            }
        }
    }
)

# Dependency for getting the chat service
def get_chat_service(request: Request):
    """Dependency to get the chat service
    
    Args:
        request: The FastAPI request object
        
    Returns:
        ChatService: Initialized chat service
    """
    return ChatService()

@router.post("/sessions/{session_id}/messages", 
           summary="Add Message",
           description="Adds a new message to the specified chat session.")
async def add_message(
    session_id: str = Path(..., description="The unique identifier of the session"),
    message: Dict[str, Any] = Body(..., description="The message to add"),
    service: ChatService = Depends(get_chat_service)
):
    """Add a message to a chat session.
    
    This endpoint adds a new message to the specified chat session. The message
    can be a user message, assistant message, or system message.
    
    Args:
        session_id: The unique identifier of the session
        message: The message to add
        
    Returns:
        A success message
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        message = {
            "role": "user",
            "content": "Hello, how can you help me today?"
        }
        
        response = requests.post(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/messages", 
            json=message
        )
        ```
    """
    try:
        await service.add_message(session_id, message)
        return {"status": "success", "message": "Message added to session"}
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to add message",
                "error_code": "MESSAGE_ADDITION_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/messages", 
          response_model=List[Dict[str, Any]],
          summary="Get Messages",
          description="Retrieves messages from the specified chat session.")
async def get_messages(
    session_id: str = Path(..., description="The unique identifier of the session"),
    start: Optional[str] = None,
    end: Optional[str] = None,
    count: Optional[int] = 100,
    service: ChatService = Depends(get_chat_service)
):
    """Get messages from a chat session.
    
    This endpoint retrieves messages from the specified chat session. It supports
    pagination and filtering by timestamp range.
    
    Args:
        session_id: The unique identifier of the session
        start: Start ID for range query (default: "-" = oldest)
        end: End ID for range query (default: "+" = newest)
        count: Maximum number of messages to retrieve (default: 100)
        
    Returns:
        A list of messages from the session
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        
        # Get the most recent 50 messages
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/messages", 
            params={"count": 50}
        )
        messages = response.json()
        ```
    """
    try:
        # Set default values if not provided
        start_val = start if start is not None else "-"
        end_val = end if end is not None else "+"
        count_val = count if count is not None else 100
        
        messages = await service.get_messages(session_id, start_val, end_val, count_val)
        return messages
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve messages",
                "error_code": "MESSAGE_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/meta", 
          response_model=Dict[str, Any],
          summary="Get Session Metadata",
          description="Retrieves metadata for the specified chat session.")
async def get_session_meta(
    session_id: str = Path(..., description="The unique identifier of the session"),
    service: ChatService = Depends(get_chat_service)
):
    """Get session metadata.
    
    This endpoint retrieves metadata for the specified chat session, such as
    creation time, update time, and custom metadata fields.
    
    Args:
        session_id: The unique identifier of the session
        
    Returns:
        Session metadata
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/meta"
        )
        metadata = response.json()
        ```
    """
    try:
        metadata = await service.get_session_meta(session_id)
        return metadata
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve session metadata",
                "error_code": "METADATA_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )

@router.put("/sessions/{session_id}/meta/{key}", 
          summary="Set Session Metadata",
          description="Sets metadata for the specified chat session.")
async def set_session_meta(
    session_id: str = Path(..., description="The unique identifier of the session"),
    key: str = Path(..., description="The metadata key to set"),
    value: Any = Body(..., description="The metadata value"),
    service: ChatService = Depends(get_chat_service)
):
    """Set session metadata.
    
    This endpoint sets metadata for the specified chat session. It can be used
    to store custom information about the session.
    
    Args:
        session_id: The unique identifier of the session
        key: The metadata key to set
        value: The metadata value
        
    Returns:
        A success message
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        key = "user_preference"
        value = {"theme": "dark", "language": "en"}
        
        response = requests.put(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/meta/{key}", 
            json=value
        )
        ```
    """
    try:
        await service.set_session_meta(session_id, key, value)
        return {"status": "success", "message": f"Metadata '{key}' set for session"}
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to set session metadata",
                "error_code": "METADATA_UPDATE_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/managed_meta", 
          response_model=Dict[str, Any],
          summary="Get Managed Metadata",
          description="Retrieves managed metadata for the specified chat session.")
async def get_managed_meta(
    session_id: str = Path(..., description="The unique identifier of the session"),
    service: ChatService = Depends(get_chat_service)
):
    """Get managed session metadata.
    
    This endpoint retrieves managed metadata for the specified chat session. Managed
    metadata is typically used internally by the system for session management.
    
    Args:
        session_id: The unique identifier of the session
        
    Returns:
        Managed session metadata
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/managed_meta"
        )
        managed_metadata = response.json()
        ```
    """
    try:
        metadata = await service.get_managed_meta(session_id)
        return metadata
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve managed metadata",
                "error_code": "MANAGED_METADATA_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )

@router.put("/sessions/{session_id}/managed_meta/{key}", 
          summary="Set Managed Metadata",
          description="Sets managed metadata for the specified chat session.")
async def set_managed_meta(
    session_id: str = Path(..., description="The unique identifier of the session"),
    key: str = Path(..., description="The metadata key to set"),
    value: Any = Body(..., description="The metadata value"),
    service: ChatService = Depends(get_chat_service)
):
    """Set managed session metadata.
    
    This endpoint sets managed metadata for the specified chat session. Managed
    metadata is typically used internally by the system for session management.
    
    Args:
        session_id: The unique identifier of the session
        key: The metadata key to set
        value: The metadata value
        
    Returns:
        A success message
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        key = "agent_state"
        value = {"last_processed_message": "msg_123"}
        
        response = requests.put(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/managed_meta/{key}", 
            json=value
        )
        ```
    """
    try:
        await service.set_managed_meta(session_id, key, value)
        return {"status": "success", "message": f"Managed metadata '{key}' set for session"}
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to set managed metadata",
                "error_code": "MANAGED_METADATA_UPDATE_ERROR",
                "message": str(e)
            }
        )

@router.post("/sessions/{session_id}/tool_calls", 
           summary="Add Tool Call",
           description="Adds a new tool call to the specified chat session.")
async def add_tool_call(
    session_id: str = Path(..., description="The unique identifier of the session"),
    tool_call: Dict[str, Any] = Body(..., description="The tool call to add"),
    service: ChatService = Depends(get_chat_service)
):
    """Add a tool call to a chat session.
    
    This endpoint adds a new tool call to the specified chat session.
    
    Args:
        session_id: The unique identifier of the session
        tool_call: The tool call to add
        
    Returns:
        A success message
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        tool_call = {
            "name": "get_weather",
            "arguments": {"location": "Seattle", "units": "imperial"},
            "result": {"temperature": 72, "condition": "sunny"}
        }
        
        response = requests.post(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/tool_calls", 
            json=tool_call
        )
        ```
    """
    try:
        await service.add_tool_call(session_id, tool_call)
        return {"status": "success", "message": "Tool call added to session"}
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to add tool call",
                "error_code": "TOOL_CALL_ADDITION_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/tool_calls", 
          response_model=List[Dict[str, Any]],
          summary="Get Tool Calls",
          description="Retrieves tool calls from the specified chat session.")
async def get_tool_calls(
    session_id: str = Path(..., description="The unique identifier of the session"),
    start: Optional[str] = None,
    end: Optional[str] = None,
    count: Optional[int] = 100,
    service: ChatService = Depends(get_chat_service)
):
    """Get tool calls from a chat session.
    
    This endpoint retrieves tool calls from the specified chat session. It supports
    pagination and filtering by timestamp range.
    
    Args:
        session_id: The unique identifier of the session
        start: Start ID for range query (default: "-" = oldest)
        end: End ID for range query (default: "+" = newest)
        count: Maximum number of tool calls to retrieve (default: 100)
        
    Returns:
        A list of tool calls from the session
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        
        # Get the most recent 50 tool calls
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/tool_calls", 
            params={"count": 50}
        )
        tool_calls = response.json()
        ```
    """
    try:
        # Set default values if not provided
        start_val = start if start is not None else "-"
        end_val = end if end is not None else "+"
        count_val = count if count is not None else 100
        
        tool_calls = await service.get_tool_calls(session_id, start_val, end_val, count_val)
        return tool_calls
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve tool calls",
                "error_code": "TOOL_CALL_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )

@router.post("/sessions/{session_id}/interactions", 
           summary="Add Interaction",
           description="Adds multiple messages as a single interaction to the chat session.")
async def add_interaction(
    session_id: str = Path(..., description="The unique identifier of the session"),
    interaction: Dict[str, Any] = Body(..., description="The interaction data, including messages and tool calls"),
    service: ChatService = Depends(get_chat_service)
):
    """Add multiple messages as a single interaction to a chat session.
    
    This endpoint adds multiple messages and optional tool calls as a single interaction to
    the specified chat session. This is useful for grouping related messages together.
    
    Args:
        session_id: The unique identifier of the session
        interaction: The interaction data, including messages and tool calls
        
    Returns:
        A success message with the interaction ID
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        interaction = {
            "messages": [
                {"role": "user", "content": "What's the weather in Seattle?"},
                {"role": "assistant", "content": "Here's the current weather in Seattle:"}
            ],
            "tool_calls": [
                {
                    "name": "get_weather",
                    "arguments": {"location": "Seattle", "units": "imperial"},
                    "result": {"temperature": 72, "condition": "sunny"}
                }
            ],
            "id": "optional-custom-id"  # Optional
        }
        
        response = requests.post(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/interactions", 
            json=interaction
        )
        ```
    """
    try:
        # Extract data from request
        messages = interaction.get("messages", [])
        tool_calls = interaction.get("tool_calls", [])
        interaction_id = interaction.get("id")
        
        # Validate required fields
        if not messages:
            raise ValueError("At least one message is required")
        
        # Add interaction
        result_id = await service.add_interaction(
            session_id=session_id,
            messages=messages,
            tool_calls=tool_calls,
            interaction_id=interaction_id
        )
        
        return {
            "status": "success", 
            "message": "Interaction added to session",
            "interaction_id": result_id
        }
    except ValueError as e:
        raise HTTPException(
            status_code=400, 
            detail={
                "error": "INVALID_INTERACTION_DATA",
                "error_code": "VALIDATION_ERROR",
                "message": str(e)
            }
        )
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to add interaction",
                "error_code": "INTERACTION_ADDITION_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/interactions", 
          response_model=List[str],
          summary="Get Interactions",
          description="Retrieves all interaction IDs for the chat session.")
async def get_interactions(
    session_id: str = Path(..., description="The unique identifier of the session"),
    service: ChatService = Depends(get_chat_service)
):
    """Get all interaction IDs for a chat session.
    
    This endpoint retrieves all interaction IDs for the specified chat session.
    
    Args:
        session_id: The unique identifier of the session
        
    Returns:
        A list of interaction IDs
        
    Raises:
        HTTPException: 404 error if the session is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/interactions"
        )
        interaction_ids = response.json()
        ```
    """
    try:
        interactions = await service.get_interactions(session_id)
        return interactions
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve interactions",
                "error_code": "INTERACTION_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )

@router.get("/sessions/{session_id}/interactions/{interaction_id}", 
          response_model=Dict[str, Any],
          summary="Get Interaction Details",
          description="Retrieves details of a specific interaction.")
async def get_interaction(
    session_id: str = Path(..., description="The unique identifier of the session"),
    interaction_id: str = Path(..., description="The unique identifier of the interaction"),
    service: ChatService = Depends(get_chat_service)
):
    """Get details of a specific interaction.
    
    This endpoint retrieves detailed information about a specific interaction, including
    all messages and tool calls that are part of the interaction.
    
    Args:
        session_id: The unique identifier of the session
        interaction_id: The unique identifier of the interaction
        
    Returns:
        Detailed information about the interaction
        
    Raises:
        HTTPException: 404 error if the session or interaction is not found
        
    Example:
        ```python
        import requests
        
        session_id = "abc123"
        interaction_id = "int_xyz789"
        
        response = requests.get(
            f"https://your-agent-c-instance.com/api/v2/chat/sessions/{session_id}/interactions/{interaction_id}"
        )
        interaction = response.json()
        ```
    """
    try:
        interaction = await service.get_interaction(session_id, interaction_id)
        if not interaction:
            raise HTTPException(
                status_code=404, 
                detail={
                    "error": "INTERACTION_NOT_FOUND",
                    "error_code": "INTERACTION_NOT_FOUND",
                    "message": f"Interaction {interaction_id} not found",
                    "params": {"interaction_id": interaction_id}
                }
            )
        return interaction
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail={
                "error": "Failed to retrieve interaction details",
                "error_code": "INTERACTION_DETAIL_RETRIEVAL_ERROR",
                "message": str(e)
            }
        )