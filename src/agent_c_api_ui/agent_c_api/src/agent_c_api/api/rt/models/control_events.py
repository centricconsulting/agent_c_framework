from typing import List, Optional, Any, Literal

from pydantic import Field

from agent_c.models import ChatSession, ChatUser
from agent_c.models.agent_config import CurrentAgentConfiguration, AgentCatalogEntry
from agent_c.models.chat_history.chat_session import ChatSessionQueryResponse, ChatSessionIndexEntry
from agent_c.models.client_tool_info import ClientToolInfo
from agent_c.models.events import BaseEvent
from agent_c.models.heygen import Avatar, HeygenAvatarSessionData, NewSessionRequest
from agent_c_api.core.voice.models import AvailableVoiceModel


class GetAgentsEvent(BaseEvent):
    """
    Event to request a list of available agents.

    This event does not require any additional data.
    """
    pass

class AgentListEvent(BaseEvent):
    """
    Event to send a list of available agents.

    Attributes:
        agents (list): A list of available agent keys.
    """
    agents: List[AgentCatalogEntry]

class SetAgentEvent(BaseEvent):
    """
    Event to set the agent for the current session.

    Attributes:
        agent_key (str): The key of the agent to set.
    """
    agent_key: str

class AgentConfigurationChangedEvent(BaseEvent):
    """
    Event to notify that the agent configuration has changed.

    Attributes:
        agent_config (CurrentAgentConfiguration): The new agent configuration.
    """
    agent_config: CurrentAgentConfiguration

class GetAvatarsEvent(BaseEvent):
    """
    Event to request a list of available avatars.

    This event does not require any additional data.
    """
    pass

class AvatarListEvent(BaseEvent):
    avatars: List[Avatar]

class SetAvatarSessionEvent(BaseEvent):
    """
    Contains the access token and avatar session ID to allow the server to drive the HeyGen avatar speech.
    """
    access_token: str
    avatar_session_id: str

class SetAvatarEvent(BaseEvent):
    """
    Event to set the avatar for the current session.

    Attributes:
        avatar_id (str): The ID of the avatar to set.
    """
    avatar_id: str
    quality: str = "medium"
    video_encoding: str = "H264"

class AvatarConnectionChangedEvent(BaseEvent):
    avatar_session_request: NewSessionRequest
    avatar_session: HeygenAvatarSessionData


class TextInputEvent(BaseEvent):
    """
    Event to send text input to the avatar.

    Attributes:
        text (str): The text input to send.
    """
    text: str
    file_ids: List[str] =  Field(default_factory=list)

class ErrorEvent(BaseEvent):
    """
    Event to send an error message.

    Attributes:
        message (str): The error message to send.
    """
    message: str
    source: Optional[str] = None

class NewChatSessionEvent(BaseEvent):
    """
    Event to request a new chat session.

    This event does not require any additional data.
    """
    agent_key: Optional[str] = None

class ResumeChatSessionEvent(BaseEvent):
    """
    Event to request resuming a chat session.

    Attributes:
        session_id (str): The ID of the session to resume.
    """
    session_id: str

class ChatSessionChangedEvent(BaseEvent):
    """
    Event to notify that the chat session has changed.

    Attributes:
        chat_session (ChatSession): The updated chat session.
    """
    chat_session: ChatSession

class SetChatSessionNameEvent(BaseEvent):
    """
    Event to set the name of the current chat session.

    Attributes:
        session_name (str): The new name for the chat session.
        session_id (Optional[str]): The ID of the session to rename, if not set the current session is used.
    """
    session_name: str
    session_id: Optional[str] = None

class ChatSessionNameChangedEvent(BaseEvent):
    """
    Event to notify that the chat session name has changed.

    Attributes:
        session_name (str): The updated name of the chat session.
        session_id (Optional[str]): The ID of the session to rename, if not set the current session is used.
    """
    session_name: str
    session_id: Optional[str] = None

class GetUserSessionsEvent(BaseEvent):
    """
    Event to request a list of user chat sessions.

    This event does not require any additional data.
    """
    offset: int = 0
    limit: int = 50

class GetUserSessionsResponseEvent(BaseEvent):
    """
    Event to send a list of user chat sessions.

    Attributes:
        sessions (ChatSessionQueryResponse): A model with a list of user chat session, total count, offset
    """
    sessions: ChatSessionQueryResponse

class PingEvent(BaseEvent):
    """
    Event to check if the connection is alive.

    This event does not require any additional data.
    """
    pass

class PongEvent(BaseEvent):
    """
    Event to respond to a ping event.

    This event does not require any additional data.
    """
    pass


class SetSessionMetadataEvent(BaseEvent):
    """
    Event to set metadata for the current chat session.
    """
    meta: dict

class SessionMetadataChangedEvent(BaseEvent):
    """
    Event to notify that the chat session metadata has changed.

    Attributes:
        meta (dict): The updated metadata of the chat session.
    """
    meta: dict

class SetSessionMessagesEvent(BaseEvent):
    """
    Event to set messages for the current chat session.
    """
    messages: List[dict[str, Any]] = Field(default_factory=list, description="New list of messages in the session")

class SetAgentVoiceEvent(BaseEvent):
    """
    Event to set the voice for the current agent.

    Attributes:
        voice_id (str): The ID of the voice to set.
    """
    voice_id: str

class GetVoicesEvent(BaseEvent):
    """
    Event to request a list of available voices.

    This event does not require any additional data.
    """
    pass

class VoiceListEvent(BaseEvent):
    """
    Event to send a list of available voices.

    Attributes:
        voices (list): A list of available voices.
    """
    voices: List[AvailableVoiceModel]

class GetToolCatalogEvent(BaseEvent):
    """
    Event to request the tool catalog.

    This event does not require any additional data.
    """
    pass

class ToolCatalogEvent(BaseEvent):
    """
    Event to send the tool catalog.

    Attributes:
        tools (list): A list of available tools.
    """
    tools: List[ClientToolInfo]

class ChatUserDataEvent(BaseEvent):
    """
    Event to send user-specific data.

    Attributes:
        user (ChatUser): The user data.
    """
    user: ChatUser


class AgentVoiceChangedEvent(BaseEvent):
    """
    Event to notify that the agent voice has changed.

    Attributes:
        voice (AvailableVoiceModel): The updated voice of the agent.
    """
    voice: AvailableVoiceModel

class UserTurnStartEvent(BaseEvent):
    """
    Event to notify that the client we're accepting user input

    This event does not require any additional data.
    """
    pass

class UserTurnEndEvent(BaseEvent):
    """
    Event to notify that the client we're accepting user input

    This event does not require any additional data.
    """
    pass

class PushToTalkStartEvent(BaseEvent):
    """
    Event for the client to notify that it's starting to accept push-to-talk audio input.
    """
    type: str = Field( "ptt_start", description="The type of the event. Defaults to the snake case class name without event")

class PushToTalkEndEvent(BaseEvent):
    """
    Event for the client to notify that it's stopping accepting push-to-talk audio input.
    """
    type: str = Field( "ptt_end", description="The type of the event. Defaults to the snake case class name without event")

class SetVoiceInputMode(BaseEvent):
    """
    Sent by the client to set the voice input mode.
    """
    mode: Literal["ptt", "vad", "realtime"] = Field(..., description="The voice input mode, either 'ptt' for push-to-talk or 'vad' for voice activity detection.")

class VoiceInputSupportedEvent(BaseEvent):
    """
    Sent by the server to notify the client of the supported voice input modes.
    """
    modes: List[Literal["ptt", "vad", "realtime"]] = Field(["ptt"], description="The list of supported voice input modes, either 'ptt' for push-to-talk or 'vad' for voice activity detection.")

class ServerListeningEvent(BaseEvent):
    """
    Event to notify that the server is listening for audio input.

    This event does not require any additional data.
    """
    pass

class ChatSessionAddedEvent(BaseEvent):
    """
    Event to notify that a new chat session has been added.
    New chat sessions are don't get indexed and added to the list of user sessions until there's been at least one message in the session.

    Attributes:
        chat_session (ChatSessionIndexEntry): The index for the new chat session.
    """
    chat_session: ChatSessionIndexEntry

class DeleteChatSessionEvent(BaseEvent):
    """
    Event sent by the client to delete a chat session belonging to the user

    Attributes:
        session_id (Optional[str]): The ID of the session to rename, if not set the current session is used.
    """
    session_id: Optional[str] = None

class ChatSessionDeletedEvent(BaseEvent):
    """
    Event sent by the server when a chat session has been deleted

    Attributes:
        session_id (Optional[str]): The ID of the session to rename, if not set the current session is used.
    """
    session_id: Optional[str] = None

class ClientWantsCancelEvent(BaseEvent):
    """
    Event sent by the client to notify that it wants to cancel the current agent response.
    """
    pass

class CancelledEvent(BaseEvent):
    """
    Event sent by the server to notify that the current agent response has been cancelled.
    """
    pass

class UISessionIDChangedEvent(BaseEvent):
    """
    Event sent by the server to notify that the UI session ID has changed.
    This can happen when the client connects with a session ID that doesn't exist, and a new session is created.
    Or when the user opens a new tab when they've already logged in in another tab and there's no ui_session_id in memory.
    The client should update its stored session ID to the new value.

    Attributes:
        ui_session_id (str): The new UI session ID.
    """
    type: str = Field( 'ui_session_id_changed`', description="The type of the event. Defaults to the snake case class name without event")
    ui_session_id: str

    def __init__(self, **data: Any) -> None:
        data['type'] = 'ui_session_id_changed'
        super().__init__(**data)

class SetAgentToolsEvent(BaseEvent):
    """
    Event to set the tools for the current agent.
    Once received the server will send an updated agent configuration with the current tools equipped.
    IMPORTANT: Just because a tool was requested to be equipped doesn't mean it was successfully equipped.
               clients should wait for the AgentConfigurationChangedEvent to get the actual list of equipped tools.

    Any errors will be sent as a system message to tu user.

    Attributes:
        tools (List[str]): The toolset names that should be equipped.
    """
    tools: List[str] = Field(default_factory=list)