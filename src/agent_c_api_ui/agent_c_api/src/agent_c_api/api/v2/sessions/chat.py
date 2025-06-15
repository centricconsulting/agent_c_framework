from typing import List, Optional, AsyncGenerator, Dict, Any, Union, Sequence
from uuid import UUID
from fastapi import APIRouter, Depends, HTTPException, Path, Body, status
from fastapi.responses import StreamingResponse, JSONResponse
import json
import asyncio
import structlog
from fastapi_cache.decorator import cache

from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from agent_c_api.api.dependencies import get_bridge_manager, get_redis_client
from agent_c_api.core.repositories.chat_repository import ChatRepository
from agent_c_api.core.services.chat_service import ChatService as CoreChatService

from fastapi import Request


async def get_chat_service(
    request: Request,
    redis_client = Depends(get_redis_client)
):
    """Dependency to get the chat service

    Args:
        request: The FastAPI request object
        redis_client: Redis client dependency
        
    Returns:
        ChatService: Initialized chat service
    """
    agent_manager = get_bridge_manager(request)
    return ChatService(agent_manager=agent_manager, redis_client=redis_client)

from agent_c_api.api.v2.models.chat_models import (
    ChatMessage,
    ChatRequest,
    ChatResponse,
    ChatEventUnion
)

from agent_c_api.api.v2.sessions.services import SessionService

router = APIRouter(tags=["sessions"])
logger = structlog.get_logger(__name__)


class ChatService:
    """Service for handling chat operations with sessions and persistence"""

    def __init__(self, agent_manager: Any, redis_client: Any):
        """Initialize the chat service
        
        Args:
            agent_manager: The agent manager instance
            redis_client: Redis client instance
        """  # Use Any instead of UItoAgentBridgeManager type
        self.agent_manager = agent_manager
        self.redis_client = redis_client
        self.logger = structlog.get_logger(__name__)

    async def send_message(
        self, 
        session_id: str, 
        message: ChatMessage,
        file_ids: Optional[List[str]] = None
    ):
        """Send a message to the agent and stream the response
        
        This method sends a message to an agent in a specific session and returns a stream
        of JSON-encoded events representing the agent's response. The stream includes
        incremental text updates, tool calls, and completion status updates.
        
        Args:
            session_id: The unique identifier for the session
            message: The message to send, including content parts (text, files, etc.)
            file_ids: Optional list of file IDs to attach to the message (can be extracted from message content)
            
        Yields:
            Stream of JSON-encoded chat events from the agent, including:
            - text_delta: Incremental text updates from the assistant
            - tool_call: Tool invocation events when tools are used
            - thought_delta: Thinking process updates (when available)
            - completion: Final completion status when the response is complete
            
        Raises:
            HTTPException(404): If the session doesn't exist
            HTTPException(400): If the message has invalid format or empty content
            HTTPException(500): If there's an error processing the message
        """
        # Verify the session exists
        session_data = await self.agent_manager.get_user_session(session_id)
        if not session_data:
            self.logger.error("User session_not_found", session_id=session_id)
            raise HTTPException(status_code=404, detail="Session not found")
        
        # For 'user' messages, extract the text content for processing
        # In the future, we can enhance this to handle multimodal content
        if message.role != "user":
            self.logger.error("invalid_message_role", role=message.role)
            raise HTTPException(status_code=400, detail="Only 'user' role messages are accepted")
        
        # Extract text content from the message (concatenating multiple text blocks if present)
        text_content = ""
        for content_part in message.content:
            if content_part.type == "text" and content_part.text:
                text_content += content_part.text + "\n"
        
        text_content = text_content.strip()
        if not text_content:
            self.logger.error("empty_message_content")
            raise HTTPException(status_code=400, detail="Message must contain text content")
            
        try:
            # Stream the response from the agent manager
            # The events are already JSON-encoded strings from the agent,
            # so we just pass them through directly
            async for event_json in self.agent_manager.stream_response(
                session_id,
                user_message=text_content,
                file_ids=file_ids
            ):
                # Just yield the JSON event string directly
                yield event_json
        except Exception as e:
            self.logger.error("stream_response_error", session_id=session_id, error=str(e))
            raise HTTPException(
                status_code=500, 
                detail=f"Error streaming response: {str(e)}"
            )
    
    async def cancel_interaction(self, session_id: str) -> bool:
        """Cancel an ongoing interaction in a session
        
        This method sends a cancellation signal to an ongoing interaction in the specified
        session. Cancellation is best-effort; there's no guarantee that the agent will
        stop immediately, but it will attempt to halt processing as soon as possible.
        
        Args:
            session_id: The unique identifier for the session containing the interaction to cancel
            
        Returns:
            True if cancellation signal was successfully sent, False if cancellation failed
            or there was no active interaction to cancel
            
        Raises:
            HTTPException(404): If the session doesn't exist
        """
        # Verify the session exists
        session_data = await self.agent_manager.get_user_session(session_id)
        if not session_data:
            self.logger.error("cancel_session_not_found", session_id=session_id)
            raise HTTPException(status_code=404, detail="Session not found")
        
        # Attempt to cancel the interaction
        success = self.agent_manager.cancel_interaction(session_id)
        
        self.logger.info(
            "interaction_cancellation", 
            session_id=session_id, 
            success=success
        )
        
        return success

    # ----- Chat persistence methods (added from v2/chat/services.py) -----
    
    def _get_core_service(self, session_id: str) -> CoreChatService:
        """Get the core chat service with dependencies
        
        Args:
            session_id: The session ID
            
        Returns:
            Initialized core chat service
        """
        chat_repository = ChatRepository(self.redis_client, session_id)
        return CoreChatService(chat_repository)
    
    async def add_message(self, session_id: str, message: Union[MessageEvent, Dict[str, Any]]) -> None:
        """Add a message to a chat session
        
        Args:
            session_id: The session ID
            message: The message to add
        """
        core_service = self._get_core_service(session_id)
        await core_service.add_message(message)
    
    async def get_messages(self, session_id: str, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """Get messages from a chat session
        
        Args:
            session_id: The session ID
            start: Start ID for range query (default: "-" = oldest)
            end: End ID for range query (default: "+" = newest)
            count: Maximum number of messages to retrieve
            
        Returns:
            List of messages
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_messages(start, end, count)
    
    async def get_session_meta(self, session_id: str) -> Dict[str, Any]:
        """Get session metadata
        
        Args:
            session_id: The session ID
            
        Returns:
            Session metadata
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_session_meta()
    
    async def set_session_meta(self, session_id: str, key: str, value: Any) -> None:
        """Set session metadata
        
        Args:
            session_id: The session ID
            key: Metadata key
            value: Metadata value
        """
        core_service = self._get_core_service(session_id)
        await core_service.set_session_meta(key, value)
    
    async def get_managed_meta(self, session_id: str) -> Dict[str, Any]:
        """Get managed session metadata
        
        Args:
            session_id: The session ID
            
        Returns:
            Managed session metadata
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_managed_meta()
    
    async def set_managed_meta(self, session_id: str, key: str, value: Any) -> None:
        """Set managed session metadata
        
        Args:
            session_id: The session ID
            key: Metadata key
            value: Metadata value
        """
        core_service = self._get_core_service(session_id)
        await core_service.set_managed_meta(key, value)
    
    async def add_tool_call(self, session_id: str, tool_call: Union[ToolCallEvent, Dict[str, Any]]) -> None:
        """Add a tool call to a chat session
        
        Args:
            session_id: The session ID
            tool_call: The tool call to add
        """
        core_service = self._get_core_service(session_id)
        await core_service.add_tool_call(tool_call)
    
    async def get_tool_calls(self, session_id: str, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """Get tool calls from a chat session
        
        Args:
            session_id: The session ID
            start: Start ID for range query (default: "-" = oldest)
            end: End ID for range query (default: "+" = newest)
            count: Maximum number of tool calls to retrieve
            
        Returns:
            List of tool calls
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_tool_calls(start, end, count)
    
    async def add_interaction(self, session_id: str, 
                            messages: Sequence[Union[MessageEvent, Dict[str, Any]]], 
                            tool_calls: Optional[Sequence[Union[ToolCallEvent, Dict[str, Any]]]] = None,
                            interaction_id: Optional[str] = None) -> str:
        """Add multiple messages as a single interaction to a chat session
        
        Args:
            session_id: The session ID
            messages: Messages to add
            tool_calls: Tool calls to add (optional)
            interaction_id: Custom interaction ID (optional)
            
        Returns:
            The interaction ID
        """
        core_service = self._get_core_service(session_id)
        return await core_service.add_interaction(messages, tool_calls, interaction_id)
    
    async def get_interactions(self, session_id: str) -> List[str]:
        """Get all interaction IDs for a chat session
        
        Args:
            session_id: The session ID
            
        Returns:
            List of interaction IDs
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_interactions()
    
    async def get_interaction(self, session_id: str, interaction_id: str) -> Dict[str, Any]:
        """Get details of a specific interaction
        
        Args:
            session_id: The session ID
            interaction_id: The interaction ID
            
        Returns:
            Interaction details including messages and tool calls
        """
        core_service = self._get_core_service(session_id)
        return await core_service.get_interaction(interaction_id)


@router.post(
    "/{session_id}/chat", 
    response_model=None,  # Using None as we'll return a streaming response
    status_code=status.HTTP_200_OK,
    summary="Send a chat message to the agent",
    description="Sends a chat message to the agent and receives a streaming response of events",
    responses={
        200: {
            "description": "Successful response with streaming events",
            "content": {
                "text/event-stream": {
                    "example": "\n".join([
                        '{"type":"text_delta","content":"Hello, I\'m Agent C."}',
                        '{"type":"text_delta","content":" How can I help you today?"}',
                        '{"type":"completion","id":"comp_123","status":"complete"}'
                    ])
                }
            }
        },
        400: {
            "description": "Bad request - invalid message format",
            "content": {
                "application/json": {
                    "example": {"detail": "Message must contain text content"}
                }
            }
        },
        404: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {"detail": "Session tiger-castle not found"}
                }
            }
        },
        500: {
            "description": "Server error processing the message",
            "content": {
                "application/json": {
                    "example": {"detail": "Error streaming response: Agent processing failed"}
                }
            }
        },
        501: {
            "description": "Non-streaming mode not supported",
            "content": {
                "application/json": {
                    "example": {"detail": "Non-streaming mode is not supported yet"}
                }
            }
        }
    }
)
async def send_chat_message(
    session_id: str = Path(..., description="UUID of the session to send the message to"),
    request: ChatRequest = Body(..., description="Chat message request containing the message and streaming preference"),
    chat_service: ChatService = Depends(get_chat_service),
    session_service: SessionService = Depends()
) -> StreamingResponse:
    """Send a chat message to the agent and receive the response
    
    This endpoint allows sending a message to an agent in a specific session and receiving
    the agent's response. By default, the response is streamed as Server-Sent Events (SSE),
    which enables real-time updates as the agent generates its response.
    
    The message can contain text content, file attachments, or both. File attachments must
    have been previously uploaded using the file upload endpoint.
    
    Args:
        session_id: String ID of  the session to send the message to
        request: The chat message request containing the message and streaming preference
        chat_service: Dependency-injected chat service
        session_service: Dependency-injected session service
        
    Returns:
        StreamingResponse: A stream of SSE events representing the agent's response
        
    Raises:
        HTTPException(404): If the session doesn't exist
        HTTPException(400): If the message has invalid format or empty content
        HTTPException(500): If there's an error processing the message
        HTTPException(501): If non-streaming mode is requested (currently unsupported)
        
    Example:
        ```python
        import requests
        import json
        import sseclient
        
        # Prepare the message
        message = {
            "message": {
                "role": "user",
                "content": [{
                    "type": "text",
                    "text": "What's the capital of France?"
                }]
            },
            "stream": True
        }
        
        # Send the message and get streaming response
        response = requests.post(
            "https://api.example.com/api/v2/sessions/tiger-castle/chat",
            json=message,
            stream=True,
            headers={"Accept": "text/event-stream"}
        )
        
        # Process the streaming response
        client = sseclient.SSEClient(response)
        for event in client.events():
            data = json.loads(event.data)
            if data.get("type") == "text_delta":
                print(data.get("content"), end="")
        ```
    """
    # Verify the session exists
    session = await session_service.get_session(session_id)
    if not session:
        raise HTTPException(status_code=404, detail=f"Session {session_id} not found")
    
    # Extract file IDs from message content if present
    file_ids = []
    for content in request.message.content:
        if content.type in ["file", "image"] and content.file_id:
            file_ids.append(content.file_id)
    
    # If streaming is requested, return a streaming response
    if request.stream:
        async def event_stream():
            try:
                async for event_json in chat_service.send_message(
                    session_id, 
                    request.message,
                    file_ids if file_ids else None
                ):
                    # The events from agent_manager are already JSON strings
                    # Just make sure they end with a newline for SSE
                    if not event_json.endswith('\n'):
                        event_json += '\n'
                    yield event_json
            except Exception as e:
                logger.error("chat_streaming_error", error=str(e))
                error_event = {
                    "type": "error",
                    "content": f"Error: {str(e)}"
                }
                yield json.dumps(error_event) + '\n'
        
        return StreamingResponse(
            event_stream(),
            media_type="text/event-stream",
            headers={
                "Cache-Control": "no-cache",
                "Connection": "keep-alive",
            },
        )
    else:
        # For non-streaming responses, we would need to collect the full response
        # This would involve changing send_message to support non-streaming mode
        # For now, we'll just reject non-streaming requests
        raise HTTPException(
            status_code=501, 
            detail="Non-streaming mode is not supported yet"
        )


@router.delete(
    "/{session_id}/chat", 
    response_model=Dict[str, Any],
    status_code=status.HTTP_200_OK,
    summary="Cancel an ongoing chat interaction",
    description="Cancels an ongoing interaction in the specified session",
    responses={
        200: {
            "description": "Successful cancellation",
            "content": {
                "application/json": {
                    "example": {
                        "status": "success",
                        "message": "Cancellation signal sent for session: tiger-castle"
                    }
                }
            }
        },
        404: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {"detail": "Session tiger-castle not found"}
                }
            }
        },
        500: {
            "description": "Failed to cancel interaction",
            "content": {
                "application/json": {
                    "example": {
                        "status": "error",
                        "message": "Failed to cancel interaction for session: tiger-castle"
                    }
                }
            }
        }
    }
)

async def cancel_chat_interaction(
    session_id: str = Path(..., description="ID of the session with the interaction to cancel"),
    chat_service: ChatService = Depends(get_chat_service),
    session_service: SessionService = Depends()
):
    """Cancel an ongoing chat interaction
    
    This endpoint sends a cancellation signal to stop an ongoing interaction with an agent.
    It's useful when you need to interrupt a long-running interaction, such as when the
    user wants to ask a different question or the agent is stuck in a loop.
    
    Cancellation is best-effort; there's no guarantee that the agent will stop immediately,
    but it will attempt to halt processing as soon as possible.
    
    Args:
        session_id: ID of the session with the interaction to cancel
        chat_service: Dependency-injected chat service
        session_service: Dependency-injected session service
        
    Returns:
        Dict[str, Any]: JSON response with cancellation status (success/error) and message
        
    Raises:
        HTTPException(404): If the session doesn't exist
        
    Example:
        ```python
        import requests
        
        # Send cancellation request
        response = requests.delete(
            "https://api.example.com/api/v2/sessions/tiger-castle/chat",
        )
        
        result = response.json()
        print(f"Cancellation {result['status']}: {result['message']}")
        ```
    """
    # Verify the session exists
    session = await session_service.get_session(session_id)
    if not session:
        raise HTTPException(status_code=404, detail=f"Session {session_id} not found")
    
    # Attempt to cancel the interaction
    success = await chat_service.cancel_interaction(session_id)
    
    if success:
        return {
            "status": "success",
            "message": f"Cancellation signal sent for session: {session_id}"
        }
    else:
        return {
            "status": "error",
            "message": f"Failed to cancel interaction for session: {session_id}"
        }