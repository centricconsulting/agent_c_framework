import json
from typing import Optional

from fastapi import APIRouter, HTTPException, Form, Depends, Request
from fastapi.responses import StreamingResponse, JSONResponse

from agent_c.util.logging_utils import LoggingManager
from agent_c_api.models.user_session import UserSession

router = APIRouter()
logger = LoggingManager(__name__).get_logger()


@router.post("/chat")
async def chat_endpoint(
        request: Request,
        ui_session_id: str = Form(...),
        message: str = Form(...),
        file_ids: Optional[str] = Form(None)
):
    """
    Endpoint for sending a message and getting a streaming response from the agent.

    Args:
        ui_session_id: Session ID
        message: User message
        file_ids: Optional JSON string of file IDs to include

    Returns:
        StreamingResponse: Streaming response from the agent
    """
    logger.info(f"Received chat request for session: {ui_session_id}")
    session_manager = request.app.state.user_session_manager
    session_data = await session_manager.get_user_session(ui_session_id)
    # logger.debug(f"Available sessions: {list(user_session_manager.sessions.keys())}")

    if not session_data:
        logger.exception(f"No session found for session_id: {ui_session_id}", exc_info=True)
        raise HTTPException(status_code=404, detail="Session not found")

    # Parse file IDs if provided
    file_id_list = None
    if file_ids:
        try:
            file_id_list = json.loads(file_ids)
            logger.info(f"Chat includes files: {file_id_list}")
        except json.JSONDecodeError:
            logger.exception(f"Invalid file_ids format: {file_ids}", exc_info=True)
            raise HTTPException(status_code=400, detail="Invalid file_ids format")

    async def event_stream():
        """Inner generator for streaming response chunks"""
        # logger.debug(f"Starting event stream for session: {ui_session_id}")
        try:
            # try to force through browser buffering
            # yield " " * 2048 + "\n"
            async for token in session_manager.stream_response(
                    ui_session_id,
                    file_ids=file_id_list,
                    user_message=message,
            ):
                # Each token is a piece of the assistant's reply
                if not token.endswith('\n'):
                    token += '\n'
                yield token
        except Exception as e:
            logger.exception(f"Error in stream_response: {e}", exc_info=True)
            yield f"Error: {str(e)}"

    return StreamingResponse(
        event_stream(),
        media_type="text/plain",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
        },

    )


@router.post("/cancel")
async def cancel_chat(request: Request,
        ui_session_id: str = Form(...),

):
    """
    Endpoint for cancelling an ongoing chat interaction.
    
    Args:
        ui_session_id: Session ID

    Returns:
        JSONResponse: Status of the cancellation request
    """
    logger.info(f"Received cancellation request for session: {ui_session_id}")
    session_manager = request.app.state.user_session_manager
    session_data: UserSession = await session_manager.get_user_session(ui_session_id)
    
    if not session_data:
        logger.error(f"No session found for session_id: {ui_session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    # Trigger cancellation
    success = session_manager.cancel_interaction(ui_session_id)
    
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