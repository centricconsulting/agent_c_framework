import json
from typing import Optional

from fastapi import APIRouter, HTTPException, Form, Depends
import logging
from fastapi.responses import StreamingResponse, JSONResponse
import traceback

from agent_c_api.api.dependencies import get_agent_manager
from agent_c_api.core.file_handler import FileHandler

router = APIRouter()
logger = logging.getLogger(__name__)


@router.post("/chat")
async def chat_endpoint(
        ui_session_id: str = Form(...),
        message: str = Form(...),
        file_ids: Optional[str] = Form(None),
        agent_manager=Depends(get_agent_manager)
):
    """
    Endpoint for sending a message and getting a streaming response from the agent.

    Args:
        ui_session_id: Session ID
        message: User message
        file_ids: Optional JSON string of file IDs to include
        agent_manager: Agent manager dependency

    Returns:
        StreamingResponse: Streaming response from the agent
    """
    logger.info(f"Received chat request for session: {ui_session_id}")
    session_data = await agent_manager.get_session_data(ui_session_id)
    # logger.debug(f"Available sessions: {list(agent_manager.sessions.keys())}")

    if not session_data:
        logger.error(f"No session found for session_id: {ui_session_id}")
        raise HTTPException(status_code=404, detail="Session not found")

    # Parse file IDs if provided
    file_id_list = None
    if file_ids:
        try:
            file_id_list = json.loads(file_ids)
            logger.info(f"Chat includes files: {file_id_list}")
        except json.JSONDecodeError:
            logger.error(f"Invalid file_ids format: {file_ids}")
            raise HTTPException(status_code=400, detail="Invalid file_ids format")

    async def event_stream():
        """Inner generator for streaming response chunks"""
        # logger.debug(f"Starting event stream for session: {ui_session_id}")
        try:
            # try to force through browser buffering
            # yield " " * 2048 + "\n"
            async for token in agent_manager.stream_response(
                    ui_session_id,
                    file_ids=file_id_list,
                    user_message=message,
            ):
                # Each token is a piece of the assistant's reply
                if not token.endswith('\n'):
                    token += '\n'
                yield token
        except Exception as e:
            logger.error(f"Error in stream_response: {e}")
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            logger.error(f"Error in chat.py:stream_response - {error_type}: {str(e)}\n{error_traceback}")
            yield f"Error: {str(e)}\n{error_traceback}"

    return StreamingResponse(
        event_stream(),
        media_type="text/plain",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
        },
        # headers={ "Cache-Control": "no-cache, no-store, must-revalidate, max-age=0",
        # "Pragma": "no-cache",
        # "Expires": "0",
        # "Connection": "keep-alive",
        # "X-Accel-Buffering": "no",  # Disable Nginx buffering
        # "Transfer-Encoding": "chunked",  # Force chunked transfer encoding
        # "Content-Encoding": "identity"}
    )
    # return StreamingResponse(
    #     event_stream(),
    #     media_type="text/event-stream",
    #     headers={
    #         "Cache-Control": "no-cache, no-store, must-revalidate, max-age=0",
    #         "Pragma": "no-cache",
    #         "Expires": "0",
    #         "Connection": "keep-alive",
    #         "X-Accel-Buffering": "no",  # Disable Nginx buffering
    #         "Transfer-Encoding": "chunked"  # Force chunked transfer encoding
    #     }
    # )


@router.post("/cancel")
async def cancel_chat(
        ui_session_id: str = Form(...),
        agent_manager=Depends(get_agent_manager)
):
    """
    Endpoint for cancelling an ongoing chat interaction.
    
    Args:
        ui_session_id: Session ID
        agent_manager: Agent manager dependency
        
    Returns:
        JSONResponse: Status of the cancellation request
    """
    logger.info(f"Received cancellation request for session: {ui_session_id}")
    session_data = await agent_manager.get_session_data(ui_session_id)
    
    if not session_data:
        logger.error(f"No session found for session_id: {ui_session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    # Trigger cancellation
    success = agent_manager.cancel_interaction(ui_session_id)
    
    if success:
        return JSONResponse({
            "status": "success",
            "message": f"Cancellation signal sent for session: {ui_session_id}"
        })
    else:
        return JSONResponse({
            "status": "error",
            "message": f"Failed to cancel interaction for session: {ui_session_id}"
        }, status_code=500)