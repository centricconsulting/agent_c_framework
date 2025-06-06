from pydantic import BaseModel, Field
from typing import Dict, Any, Optional, List
from enum import Enum
from datetime import datetime

class ReplayControlRequest(BaseModel):
    action: str  # play, pause, stop, seek
    position: Optional[str] = None  # timestamp to seek to

class EventType(str, Enum):
    # Core chat events
    INTERACTION = "interaction"
    COMPLETION = "completion"
    MESSAGE = "message"
    SYSTEM_MESSAGE = "system_message"
    HISTORY = "history"
    HISTORY_DELTA = "history_delta"
    
    # Text streaming events
    TEXT_DELTA = "text_delta"
    THOUGHT_DELTA = "thought_delta"
    COMPLETE_THOUGHT = "complete_thought"
    
    # Tool events
    TOOL_CALL = "tool_call"
    TOOL_SELECT_DELTA = "tool_select_delta"
    
    # Media events
    RENDER_MEDIA = "render_media"
    
    # Audio events
    AUDIO_DELTA = "audio_delta"
    AUDIO_INPUT_BEGIN = "audio_input_begin"
    AUDIO_INPUT_DELTA = "audio_input_delta"
    AUDIO_INPUT_END = "audio_input_end"
    
    # Legacy/deprecated events (keeping for backward compatibility)
    SYSTEM_PROMPT = "system_prompt"  # deprecated: use SYSTEM_MESSAGE
    COMPLETION_OPTIONS = "completion_options"  # deprecated: not used in core
    USER_REQUEST = "user_request"  # deprecated: not used in core

class Event(BaseModel):
    # whenever you do a model_dump here, ensure you do mode='json', so that datetime objects are serialized
    timestamp: datetime
    type: EventType
    session_id: Optional[str] = None
    role: Optional[str] = None
    content: Optional[Any] = None
    format: Optional[str] = None
    running: Optional[bool] = None
    active: Optional[bool] = None
    vendor: Optional[str] = None
    tool_calls: Optional[List[Dict[str, Any]]] = None
    tool_results: Optional[List[Dict[str, Any]]] = None
    raw: Dict[str, Any]  # Original event data