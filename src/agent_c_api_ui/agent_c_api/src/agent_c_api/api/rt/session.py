from typing import Optional, Dict, Any, TYPE_CHECKING
from fastapi import APIRouter, HTTPException, Depends, WebSocket, Request, Form
from fastapi.responses import JSONResponse


from agent_c.util import MnemonicSlugs
from agent_c.util.logging_utils import LoggingManager
from agent_c_api.api.dependencies import get_auth_service, get_heygen_client
from agent_c_api.core.util.jwt import validate_request_jwt, create_jwt_token, verify_jwt_token
from agent_c_api.models.auth_models import UserLoginRequest, RealtimeLoginResponse

if TYPE_CHECKING:
    from agent_c.util.heygen_streaming_avatar_client import HeyGenStreamingClient
    from agent_c_api.core.services.auth_service import AuthService
    from agent_c_api.core.realtime_session_manager import RealtimeSessionManager


router = APIRouter()
logger = LoggingManager(__name__).get_logger()



@router.post("/login", response_model=RealtimeLoginResponse)
async def login(login_request: UserLoginRequest,
                auth_service: "AuthService" = Depends(get_auth_service),
                heygen_client: "HeyGenStreamingClient" = Depends(get_heygen_client)) -> RealtimeLoginResponse:
    """
    Authenticate user and return config with token.
    """
    login_response = await auth_service.login(login_request.username, login_request.password)
    
    if not login_response:
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    # Get config data
    if heygen_client is not None:
        heygen_token = await heygen_client.create_streaming_access_token()
    else:
        heygen_token = "None"

    return RealtimeLoginResponse(agent_c_token=login_response.token,
                                 heygen_token=heygen_token,
                                 ui_session_id=MnemonicSlugs.generate_slug(3))

@router.get("/refresh_token")
async def refresh_token(request: Request):
    """
    Refreshes the JWT token for the user.
    """
    user_info = await validate_request_jwt(request)
    if not user_info:
        raise HTTPException(status_code=401, detail="Invalid token")

    heygen_client = HeyGenStreamingClient()
    heygen_token = await heygen_client.create_streaming_access_token()
    new_token = create_jwt_token(user_info['user_id'], user_info.get('permissions', []))
    return {"agent_c_token": new_token, 'heygen_token': heygen_token}


@router.websocket("/ws")
async def initialize_realtime_session(websocket: WebSocket,
                                      token: str,
                                      ui_session_id: Optional[str] = None,
                                      chat_session_id: Optional[str] = None,
                                      agent_key: Optional[str] = None):
    """
    Creates an agent session with the provided parameters.
    """
    try:
        user_info: Dict[str, Any] = verify_jwt_token(token)
        auth_service: 'AuthService' = websocket.app.state.auth_service
        user = await auth_service.auth_repo.get_user_by_id(user_info['user_id'])
        manager = websocket.app.state.realtime_manager
        ui_session = await manager.create_realtime_session(user, ui_session_id)

        await ui_session.bridge.run(websocket, chat_session_id, agent_key)

    except Exception as e:
        logger.exception(f"Error during session initialization: {str(e)}", exc_info=True)
        raise HTTPException(status_code=500, detail=str(e))


@router.post("/cancel")
async def cancel_chat(request: Request, ui_session_id: str = Form(...)):
    """
    Endpoint for cancelling an ongoing chat interaction.

    Args
        request: FastAPI request object
        ui_session_id: Session ID
    Returns:
        JSONResponse: Status of the cancellation request
    """
    user_info = await validate_request_jwt(request)
    if not user_info:
        raise HTTPException(status_code=401, detail="Invalid token")

    logger.info(f"Received cancellation request for session: {ui_session_id}")
    manager: RealtimeSessionManager = request.app.state.realtime_manager
    ui_session = manager.get_user_session_data(ui_session_id, user_info['user_id'])

    if not ui_session:
        logger.error(f"No session found for session_id: {ui_session_id}")
        raise HTTPException(status_code=404, detail="Session not found")

    # Trigger cancellation
    await ui_session.bridge.cancel_interaction()
    return JSONResponse({
        "status": "success",
        "message": f"Cancellation signal sent for session: {ui_session_id}"
    })
