import asyncio
import copy
import json
import os
import traceback
from contextlib import suppress
from datetime import datetime
from functools import singledispatchmethod
from operator import truediv
from typing import List, Optional, Any, Dict, AsyncIterator, Union, TYPE_CHECKING

from sympy import false

from agent_c.config import locate_config_path
from agent_c.prompting.basic_sections.markdown import MarkdownFormatting
from agent_c_api.core.commands.handler import ChatCommandHandler
from agent_c_api.models.user_runtime_cache_entry import UserRuntimeCacheEntry
from fastapi import WebSocket, WebSocketDisconnect
from starlette.websockets import WebSocketState

from agent_c.chat import ChatSessionManager

from agent_c.models import ChatSession, ChatUser
from agent_c.models.events import BaseEvent, TextDeltaEvent, HistoryEvent, RenderMediaEvent
from agent_c.models.heygen import HeygenAvatarSessionData, NewSessionRequest
from agent_c.models.input import AudioInput
from agent_c.models.input.file_input import FileInput
from agent_c.models.input.image_input import ImageInput
from agent_c.prompting import PromptBuilder, EnvironmentInfoSection
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c.toolsets import Toolset, ToolChest
from agent_c.util import MnemonicSlugs
from agent_c.util.heygen_streaming_avatar_client import HeyGenStreamingClient
from agent_c.util.logging_utils import LoggingManager
from agent_c.util.registries.event import EventRegistry
from agent_c_api.api.rt.models.control_events import ChatSessionNameChangedEvent, SessionMetadataChangedEvent, UISessionIDChangedEvent
from agent_c_api.api.rt.models.control_events import ErrorEvent, AgentListEvent, AvatarListEvent, AvatarConnectionChangedEvent, \
    AgentConfigurationChangedEvent, ChatSessionChangedEvent, AgentVoiceChangedEvent, UserTurnStartEvent, UserTurnEndEvent, GetUserSessionsResponseEvent, ToolCatalogEvent, ChatUserDataEvent, \
    VoiceListEvent, ChatSessionAddedEvent, DeleteChatSessionEvent, CancelledEvent

from agent_c_api.core.event_handlers.client_event_handlers import ClientEventHandler
from agent_c_api.core.file_handler import RTFileHandler, FileMetadata
from agent_c_api.core.voice.models import open_ai_voice_models, AvailableVoiceModel, heygen_avatar_voice_model, no_voice_model
from agent_c_api.core.voice.voice_io_manager import VoiceIOManager

from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c.models.events import  SystemMessageEvent

if TYPE_CHECKING:
    from agent_c.chat import ChatSessionManager
    from agent_c_api.core.realtime_session_manager import RealtimeSessionManager


# Constants
DEFAULT_BACKEND = 'claude'
DEFAULT_MODEL_NAME = 'claude-sonnet-4-20250514'
DEFAULT_OUTPUT_FORMAT = 'raw'
DEFAULT_TOOL_CACHE_DIR = '.tool_cache'
DEFAULT_LOG_DIR = './logs/sessions'
LOCAL_WORKSPACES_FILE = '.local_workspaces.json'
DEFAULT_ENV_NAME = 'development'
OPENAI_REASONING_MODELS = ['o1', 'o1-mini', 'o3', 'o3-mini']


class RealtimeBridge(ClientEventHandler):

    def __init__(self,
                 ui_session_manager: 'RealtimeSessionManager',
                 chat_user: ChatUser,
                 ui_session_id: str,
                 session_manager: ChatSessionManager,
                 runtime_cache_entry: UserRuntimeCacheEntry):

        self.runtime_cache: UserRuntimeCacheEntry = runtime_cache_entry
        self.ui_session_manager = ui_session_manager
        self.chat_session: Optional[ChatSession] = None
        self._websocket: Optional[WebSocket] = None
        self._websocket_lock = asyncio.Lock()
        self.is_running = False
        self.is_connected = False
        self._is_new_bridge = True
        self.chat_session_manager: ChatSessionManager = session_manager
        self.chat_user: ChatUser = chat_user
        self.ui_session_id: str = ui_session_id
        self.logger = LoggingManager(__name__).get_logger()

        self.tool_chest: ToolChest  = self.runtime_cache.tool_chest

        if os.environ.get("HEYGEN_API_KEY") is not None:
            self.heygen_client = HeyGenStreamingClient()
        else:
            self.heygen_client = None

        self.avatar_client = self.heygen_client
        self.client_wants_cancel = asyncio.Event()
        self._active_interact_task: Optional[asyncio.Task] = None
        self._send_lock = asyncio.Lock()
        self.avatar_session: Optional[HeygenAvatarSessionData] = None
        self.avatar_session_id: Optional[str] = None
        self.avatar_session_token: Optional[str] = None
        self._partial_agent_message: str = ""
        self._avatar_did_think = False
        self.voice_io_manager = VoiceIOManager(self)
        self._voices = open_ai_voice_models
        self._voice: AvailableVoiceModel = no_voice_model
        self.file_handler: RTFileHandler = RTFileHandler(self, chat_user.user_id)
        self.image_inputs: List[ImageInput] = []
        self.audio_inputs: List[AudioInput] = []

        self.command_handler =  ChatCommandHandler()

    @property
    def websocket(self) -> Optional[WebSocket]:
        """Get current websocket connection."""
        return self._websocket

    async def _set_websocket(self, websocket: Optional[WebSocket]) -> None:
        """
        Internal method to set websocket with proper locking and cleanup.
        
        Args:
            websocket: New websocket connection, or None to clear
        """
        async with self._websocket_lock:
            # Close old websocket if it exists and is still open
            if self._websocket is not None:
                try:
                    if self._websocket.client_state == WebSocketState.CONNECTED:
                        await self._websocket.close()
                        self.logger.debug(f"Closed old websocket for session {self.ui_session_id}")
                except Exception as e:
                    self.logger.debug(f"Error closing old websocket for session {self.ui_session_id}: {e}")
            
            # Set new websocket
            self._websocket = websocket
            self.is_connected = websocket is not None
            
            if websocket is not None:
                self.logger.info(f"WebSocket connected for session {self.ui_session_id}")
            else:
                self.logger.info(f"WebSocket disconnected for session {self.ui_session_id}")

    async def _clear_websocket(self) -> None:
        """
        Clear websocket reference without trying to close it.
        Use this when the websocket is already closed by the client.
        """
        async with self._websocket_lock:
            self._websocket = None
            self.is_connected = False
            self.logger.debug(f"Cleared websocket reference for session {self.ui_session_id}")

    async def reconnect(self, websocket: WebSocket) -> None:
        """
        Reconnect the bridge with a new websocket connection.
        
        This allows a client to reconnect to an existing session without
        losing bridge state (chat history, agent config, files, etc.).
        
        Args:
            websocket: New websocket connection to use
            
        Note:
            If an interaction is currently running, it will continue
            and the new client will receive events once connected.
        """
        self.logger.info(f"Reconnecting UI session {self.ui_session_id}")
        
        # Accept the new websocket
        await websocket.accept()
        
        # Replace the old websocket (this handles cleanup)
        await self._set_websocket(websocket)
        
        # Reset new bridge flag to false since this is a reconnection
        self._is_new_bridge = False

        await self.send_chat_session()

        # Let client know they can send input (unless interaction is running)
        if not self._active_interact_task or self._active_interact_task.done():
            await self.send_user_turn_start()
        else:
            self.logger.info(f"UI session {self.ui_session_id} reconnected during active interaction")
        
        self.logger.info(f"UI session {self.ui_session_id} successfully reconnected")

    async def flush_session(self, touch: bool = True, chat_session: Optional[ChatSession] = None) -> None:
        """Flush the current chat session to persistent storage"""
        chat_session = chat_session or self.chat_session
        if chat_session:
            await self.chat_session_manager.flush_session(chat_session, touch)
            if touch:
                await self.send_to_all_user_sessions(ChatSessionAddedEvent(chat_session=chat_session.as_index_entry()))

    async def send_tool_error(self, tool_name: str, error: str) -> None:
        """Report a tool error to the client"""
        await self.send_event(SystemMessageEvent(content=f"# Error using tool '{tool_name}':\n\n```{error}```", session_id=self.chat_session.session_id,
                                                 severity="error", role="system"))
    async def send_tool_warning(self, tool_name: str, error: str) -> None:
        """Report a tool error to the client"""
        await self.send_event(SystemMessageEvent(content=f"# Error using tool '{tool_name}':\n\n```{error}```", session_id=self.chat_session.session_id,
                                                 severity="error", role="warning"))

    async def send_system_message(self, content: str, severity: str = "info") -> None:
        """Send a system message to the client"""
        await self.send_event(SystemMessageEvent(content=content, session_id=self.chat_session.session_id,
                                                 severity=severity, role="system"))

    async def cancel_interaction(self) -> None:
        self.client_wants_cancel.set()
        await self.send_event(CancelledEvent())
        await self.send_event(SystemMessageEvent(content="Interaction cancelled by user, waiting for runtime to exit..", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))

    async def delete_chat_session(self, session_id: str) -> None:
        if session_id is None:
            session_id = self.chat_session.session_id

        try:
            await self.chat_session_manager.delete_session(session_id, self.chat_user.user_id)
        except Exception as e:
            self.logger.warning(f"RealtimeBridge {self.ui_session_id}: Failed to delete chat session {session_id}")
            await self.send_error(f"Session '{session_id}' not found", source="delete_chat_session")
            return

        self.logger.info(f"RealtimeBridge {self.ui_session_id}: Deleted chat session {session_id}")
        await self.send_to_all_user_sessions(DeleteChatSessionEvent(session_id=session_id))

        if session_id == self.chat_session.session_id:
            self.chat_session = None
            await self.new_chat_session()

        await self.send_event(SystemMessageEvent(content=f"Chat Session {session_id} deleted", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))

    async def set_agent_voice(self, voice_id: str) -> None:
        if voice_id == "none":
           voice = no_voice_model
        else:
            voice = next((v for v in self._voices if v.voice_id == voice_id), None)

        if not voice:
            await self.send_system_message(f"Voice '{voice_id}' not found", severity="error")
            return

        await self.set_agent_voice(voice)


        self.voice_io_manager.change_voice(voice)
        await self.send_event(AgentVoiceChangedEvent(voice=voice))

    async def reload_agents(self) -> None:
        self.ui_session_manager.agent_config_loader.load_agents()
        self.chat_session.agent_config = self.ui_session_manager.agent_config_loader.duplicate(self.chat_session.agent_config.key)
        await self.send_agent_list()
        await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        await self.send_event(AgentConfigurationChangedEvent(agent_config=self.chat_session.agent_config))


    async def send_agent_list(self) -> None:
        catalog = self.ui_session_manager.agent_config_loader.client_catalog
        await self.send_event(AgentListEvent(agents=catalog))

    async def send_avatar_list(self) -> None:
        if self.heygen_client:
            resp = await self.heygen_client.list_avatars()
            await self.send_event(AvatarListEvent(avatars=resp.data))
        else:
            await self.send_event(AvatarListEvent(avatars=[]))

    async def end_avatar_session(self) -> None:
        """End the current avatar session if it exists"""
        if self.avatar_session:
            try:
                await self.heygen_client.close_session()
                self.logger.info(f"Avatar session {self.avatar_session.session_id} ended successfully")
                self.voice_io_manager.close()
            except Exception as e:
                self.logger.error(f"Failed to end avatar session {self.avatar_session.session_id}: {e}")
            finally:
                self.avatar_session = None
                self.heygen_client = None

    async def set_avatar(self, avatar_id: str, quality: str = "medium", video_encoding: str = "H264") -> None:
        """Set the avatar for the current session by creating a HeyGen session"""
        if self.avatar_session is not None:
            await self.end_avatar_session()

        self.avatar_client = self.heygen_client
        session_request = NewSessionRequest(avatar_id=avatar_id, quality=quality, video_encoding=video_encoding)
        self.avatar_session = await self.heygen_client.create_new_session(session_request)
        await self.send_event(AvatarConnectionChangedEvent(avatar_session=self.avatar_session,
                                                           avatar_session_request= session_request))
        self.voice_io_manager.start(heygen_avatar_voice_model)

    async def set_avatar_session(self, access_token: str, avatar_session_id: str) -> None:
        """Set the avatar session for the current session"""
        if self.avatar_session is not None:
            await self.end_avatar_session()
        self.avatar_session_id = avatar_session_id
        self.avatar_session_token = access_token
        self.avatar_client = HeyGenStreamingClient(api_key=access_token)
        self.avatar_client.session_id = avatar_session_id
        await self.send_event(AgentVoiceChangedEvent(voice=heygen_avatar_voice_model))
        self.voice_io_manager.start(heygen_avatar_voice_model)
        await self.avatar_say("Avtar bridge connected to avatar session", role="system")

    async def resume_chat_session(self, session_id: str) -> None:
        await self.release_current_session()
        session_info = await self.chat_session_manager.get_session(session_id, self.chat_user.user_id)
        if not session_info or session_info.user_id != self.chat_user.user_id:
            await self.send_error(f"Session '{session_id}' not found", source="resume_chat_session")
            return

        self.chat_session = session_info
        self.chat_session.agent_config = self.ui_session_manager.agent_config_loader.duplicate(self.chat_session.agent_config.key)

        if 'BridgeTools' not in self.chat_session.agent_config.tools:
            self.chat_session.agent_config.tools.append('BridgeTools')

        await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        self.logger.info(f"RealtimeBridge resumed chat session {self.chat_session.session_id}")
        await self.send_chat_session()
        await self.send_event(SystemMessageEvent(content=F"Welcome back! **Session ID:** {self.chat_session.session_id}", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))

    async def rename_current_session(self, new_name: str):
        await self.set_chat_session_name(new_name)

    async def set_chat_session_name(self, session_name: str, session_id: Optional[str] = None) -> None:
        """Set the name of the current chat session"""
        if session_id is None:
            session_id = self.chat_session.session_id

        if session_id != self.chat_session.session_id:
            entry = await self.chat_session_manager.rename_session(session_id, self.chat_session.user_id, session_name)
            if not entry:
                await self.send_error(f"Session '{session_id}' not found", source="set_chat_session_name")
                return
        else:
            self.chat_session.session_name = session_name

        self.logger.info(f"RealtimeBridge {session_id}: Session name set to '{session_name}'")

        await self.send_to_all_user_sessions(ChatSessionNameChangedEvent(session_name=session_name, session_id=session_id))

    async def send_user_sessions(self, offset: int, limit: int = 50) -> None:
        sessions = await self.chat_session_manager.get_user_sessions(self.chat_user.user_id, offset, limit)
        await self.send_event(GetUserSessionsResponseEvent(sessions=sessions))

    async def add_tool(self, new_tool: str) -> bool:
        if new_tool not in self.chat_session.agent_config.tools:
            tools = [new_tool]
            tools.extend(self.chat_session.agent_config.tools)
            return await self.update_tools(tools)

        return True

    async def remove_tool(self, tool_name: str) -> bool:
        if tool_name in self.chat_session.agent_config.tools:
            tools = [t for t in self.chat_session.agent_config.tools if t != tool_name]
            return await self.update_tools(tools)

        return True

    async def update_tools(self, new_tools: List[str]) -> bool:
        """
        Update the agent's tools without reinitializing the entire agent.

        This method allows dynamic updating of the tool set while maintaining
        the current session and other configurations. It ensures essential
        tools are preserved while adding or removing additional tools.

        Args:
            new_tools: List of tool names to be added to the essential tools.
        """
        self.logger.info(f"Requesting new tool list for agent {self.chat_session.agent_config.key} to: {new_tools}")
        equipped: bool = False
        try:
            equipped = await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        except Exception as e:
            self.logger.error(f"Error updating tools: {e}\n{traceback.format_exc()}")
            await self.send_system_message(f"Error updating tools: {e}", severity="error")
            return false

        if not equipped:
            self.logger.warning(f"Tool update failed. Current Active tools: "
                                f"{list(self.tool_chest.available_tools.keys())}")
            new_tools = [t for t in self.chat_session.agent_config.tools if t in self.tool_chest.available_tools]
            await self.send_system_message("Some tools failed to equip, see server logs for details", severity="error")

        self.chat_session.agent_config.tools = new_tools
        await self.send_event(AgentConfigurationChangedEvent(agent_config=self.chat_session.agent_config))
        await self.send_system_message("Agent tools updated", severity="info")
        return equipped

    async def call_tool(self, tool_name: str, params: Dict[str, Any]) -> None:
        context = await self._tool_context()
        result = await self.tool_chest.call_tool_internal(tool_name, params, context)
        if result is not None:
            await self.send_system_message(f"Tool call result:\n\n```\n{result}\n```\n\n", severity="info")

    async def set_session_metadata(self, meta: Dict[str, Any]) -> None:
        """Set the metadata for the current chat session"""
        self.chat_session.metadata = meta
        self.logger.info(f"RealtimeBridge {self.chat_session.session_id}: Session metadata updated")

        await self.send_chat_session_meta()

    async def release_current_session(self) -> None:
        if self.chat_session is None:
            return

        await self.chat_session_manager.release_session(self.chat_session.session_id, self.chat_session.user_id)

    async def new_chat_session(self, agent_key: Optional[str] = None) -> None:
        self.logger.info(f"Creating new chat session with {agent_key} from {self.ui_session_id}")
        agent_key = agent_key or self.chat_session.agent_config.key
        await self.flush_session()
        await self.release_current_session()

        session_id = f"{self.chat_session.user_id}-{MnemonicSlugs.generate_slug(2)}"
        self.chat_session =  await self._get_or_create_chat_session(session_id=session_id, agent_key=agent_key)
        if 'BridgeTools' not in self.chat_session.agent_config.tools:
            self.chat_session.agent_config.tools.append('BridgeTools')
        await self.send_chat_session()

    async def send_voices(self):
        voices = [no_voice_model, heygen_avatar_voice_model] + open_ai_voice_models
        await self.send_event(VoiceListEvent(voices=voices))

    def is_user_input_message(self, message: Dict[str, Any]) -> bool:
        if message.get('role') != 'user':
            return False

        content = message['content']
        if isinstance(content, str):
            return True

        if isinstance(content, list):
            for item in content:
                if isinstance(item, dict) and item.get('type') == 'tool_result':
                    return False

        return True


    async def rewind_session(self, count: int):
        if count == 0:
            count = 1

        found = 0

        # Loop backwards through messages to find user messages
        for i in range(len(self.chat_session.messages) - 1, -1, -1):
            message = self.chat_session.messages[i]
            if self.is_user_input_message(message):
                found += 1
                if found >= count:
                    # Found the Nth user message, truncate here
                    self.chat_session.messages = self.chat_session.messages[:(i-1)]
                    await self.flush_session()
                    await self.send_chat_session()
                    await self.send_system_message(f"Rewound session by {count} user message(s)", severity="info")
                    return


    async def fork_session(self, session_id: Optional[str] = None):
        if session_id is None:
            session_id = self.chat_session.session_id

        new_session_id = f"{self.chat_session.user_id}-{MnemonicSlugs.generate_slug(2)}"

        if session_id != self.chat_session.session_id:
            old_session: ChatSession = await self.chat_session_manager.get_session(session_id, self.chat_user.user_id)
            if not old_session or old_session.user_id != self.chat_user.user_id:
                await self.send_error(f"Session '{session_id}' not found", source="fork_session")
                return
            name = old_session.display_name
            session_data = old_session.model_dump(exclude={'display_name', 'vendor'})
        else:
            session_data = self.chat_session.model_dump(exclude={'display_name', 'vendor'})
            name = self.chat_session.display_name

        session_data['session_id'] = new_session_id
        session_data['session_name'] = f"{name} (fork)"

        try:
            new_session = ChatSession.model_validate(session_data)
            await self.chat_session_manager.flush_session(new_session, True)
            await self.send_to_all_user_sessions(ChatSessionAddedEvent(chat_session=new_session.as_index_entry()))
            await self.resume_chat_session(new_session.session_id)
            await self.send_system_message(f"Forked from {session_id}.", severity="info")

        except Exception as e:
            self.logger.error(f"Failed to fork session {session_id}: {e}\n{traceback.format_exc()}")
            await self.send_error(f"Failed to fork session '{session_id}': {e}", source="fork_session")
            return




    async def send_tool_catalog(self) -> None:
        event = ToolCatalogEvent(tools=Toolset.get_client_registry())
        await self.send_event(event)

    async def send_user_info(self) -> None:
        await self.send_event(ChatUserDataEvent(user=self.chat_user))

    async def set_session_messages(self, messages: List[Dict[str, Any]]) -> None:
        """Set the messages for the current chat session"""
        self.chat_session.messages = messages
        self.logger.info(f"RealtimeBridge {self.chat_session.session_id}: Session messages updated")
        await self.flush_session()
        await self.send_event(HistoryEvent(messages=self.chat_session.messages, session_id=self.chat_session.session_id))

    async def set_agent(self, agent_key: str) -> None:
        """Set the agent for the current session"""
        if not self.chat_session.agent_config or self.chat_session.agent_config.key != agent_key:
            agent_config = self.ui_session_manager.agent_config_loader.duplicate(agent_key)
            if not agent_config:
                await self.send_error(f"Agent '{agent_key}' not found")
                return

            self.chat_session.agent_config = agent_config

            if 'BridgeTools' not in self.chat_session.agent_config.tools:
                self.chat_session.agent_config.tools.append('BridgeTools')

            await self.send_event(AgentConfigurationChangedEvent(agent_config=self.chat_session.agent_config))
            await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
            self.logger.debug(f"RealtimeBridge {self.chat_session.session_id}: Agent set to {agent_key}")

    def parse_event(self, data: dict) -> BaseEvent:
        """Parse incoming data into appropriate event type"""
        event_type = data.get("type")
        if not event_type:
            raise ValueError("Event must have a 'type' field")

        event_class = EventRegistry.get_class(event_type)

        if event_class:
            return event_class(**data)
        else:
            self.logger.warning(f"Unknown event type '{event_type}', using BaseEvent")
            return BaseEvent(**data)

    async def send_user_turn_start(self):
        """Notify client that user turn is starting"""
        await self.send_event(UserTurnStartEvent())

    async def send_user_turn_end(self):
        """Notify client that user turn is starting"""
        await self.send_event(UserTurnEndEvent())

    async def send_event(self, event: BaseEvent):
        """
        Send event to connected client.
        
        Silently returns if no client is connected, allowing long-running
        interactions to continue even if client disconnects. This is critical
        for operations that may run for hours.
        
        Args:
            event: Event to send to client
        """
        if self.websocket is None or self.websocket.client_state != WebSocketState.CONNECTED:
            return

        model_dump = event.model_dump()
        if 'session_id' in model_dump:
            if self.chat_session.session_id not in [model_dump['session_id'], model_dump.get('user_session_id', "nousersession")]:
                return

        try:
            async with self._send_lock:
                event_str = json.dumps(model_dump)
                await self.websocket.send_text(event_str)

            #if event.type not in ["ping", "pong"]:
            #    self.logger.info(f"Sent event {event.type} to {self.chat_session.session_id}")
                # self.logger.info(event_str)
        except RuntimeError as e:
            # Race condition: websocket closed between check and send
            # This is expected during disconnection, don't spam logs or stop interaction
            if "websocket.send" in str(e) or "websocket.close" in str(e):
                self.logger.debug(f"WebSocket closed during send for session {self.ui_session_id}: {e}")
            else:
                # Unexpected RuntimeError, log at warning level
                self.logger.warning(f"RuntimeError sending event to session {self.ui_session_id}: {e}")
        except Exception as e:
            # Other exceptions (JSON errors, network issues, etc.)
            # Log but don't stop the interaction
            self.logger.warning(f"Failed to send event to session {self.ui_session_id}: {e}")

    async def send_error(self, message: str, source: Optional[str] = None):
        """Send error message to client"""
        await self.send_event(ErrorEvent(message=message, source=source))

    async def send_chat_session(self):
        """Send the current chat session state to the client"""
        if self.chat_session:
                await self.send_event(ChatSessionChangedEvent(chat_session=self.chat_session))

    async def send_chat_session_meta(self):
        """Send the current chat session state to the client"""
        if self.chat_session:
            await self.send_event(SessionMetadataChangedEvent(meta=self.chat_session.metadata))

    async def send_chat_session_name(self, session: Optional[ChatSession] = None):
        """Send the current chat session name to the client"""
        chat_session = session or self.chat_session
        if chat_session:
            await self.send_event(ChatSessionNameChangedEvent(session_name=chat_session.name, session_id=chat_session.session_id))

    # Handlers for runtime events coming over the callback
    @singledispatchmethod
    async def handle_runtime_event(self, event: BaseEvent):
        """Default handler for runtime events, forward to client"""
        await self.send_event(event)

    @handle_runtime_event.register
    async def _(self, event: HistoryEvent):
        if event.session_id == self.chat_session.session_id:
            self.chat_session.messages = copy.deepcopy(event.messages)

    @property
    def avatar_think_message(self) -> str:
        # Placeholder for avatar thinking message
        return "Let me think about that..."

    async def avatar_say(self, text: str, role: str = "assistant"):
        if not self.avatar_session and not self.avatar_session_id:
            self.logger.error(f"RealtimeBridge {self.ui_session_id}: No active avatar session to send message")
            return
        try:
            await self.avatar_client.send_task(text)
        except Exception as e:
            self.logger.error(f"RealtimeBridge {self.ui_session_id}: Failed to send message to avatar: {e}")
            await self.send_error(f"Failed to send message to avatar: {str(e)}")

        await self.send_event(TextDeltaEvent(
            content=text,
            session_id=self.chat_session.session_id,
            role=role
        ))

    async def runtime_callback(self, event: BaseEvent):
        """Handle runtime events from the agent"""
        await self.handle_runtime_event(event)
        await asyncio.sleep(0)

    async def send_ui_session_id(self):
        """Send the UI session ID to the client"""
        await self.send_event(UISessionIDChangedEvent(ui_session_id=self.ui_session_id))

    async def send_client_initial_data(self):
        await self.send_ui_session_id()
        if self._is_new_bridge:
            await self.send_user_info()
            await self.send_avatar_list()
            await self.send_voices()
            await self.send_agent_list()
            await self.send_tool_catalog()
            self._is_new_bridge = False

        await self.send_chat_session()
        message = ("# Welcome to Agent C\n\n:::TIP\n- **First time here?** Send *Hello Domo* to get started!\n"
                   "- Send `!help` for information on available chat commands.\n\n:::\n\n")



        if len(self.chat_session.messages) > 0:
            message = "# Welcome back Agent C\n\nYour previous session has been restored."

        await self.raise_render_media_markdown(message)

    async def run(self, websocket: WebSocket):
        """
        Main run loop for the bridge.
        
        Can be called multiple times for reconnection. If the websocket
        has already been set up via reconnect(), this will skip the
        initialization and go straight to the event loop.
        
        Args:
            websocket: WebSocket connection to service
        """
        # If we're not already servicing this websocket, set it up
        if self._websocket is not websocket:
            await websocket.accept()
            await self._set_websocket(websocket)
            await self.send_client_initial_data()
            await self.send_user_turn_start()
            self.logger.info(f"RealtimeBridge started servicing websocket for UI session {self.ui_session_id}")
        else:
            self.logger.info(f"RealtimeBridge resuming event loop for UI session {self.ui_session_id}")
        
        self.is_running = True

        try:
            while self.is_running:
                try:
                    message = await websocket.receive()
                    if message["type"] == "websocket.receive":
                        if "text" in message:
                            event = self.parse_event(json.loads(message["text"]))
                            if event.type not in ["ping", "pong"]:
                                self.logger.debug(f"Received event {event.type} from session {self.ui_session_id}")

                            if event.type == "resume_chat_session" and event.session_id == self.chat_session.session_id:
                                self.logger.info("Client requested to resume the current session, ignoring.")
                                await self.send_chat_session()
                                continue

                            if self._active_interact_task and not self._active_interact_task.done() and event.type not in ["ping", "pong", "client_wants_cancel"]:
                                self.logger.warning(f"Chat Session {self.chat_session.session_id} has an active interaction, shutting down interaction due to  new event {event.type}")
                                self.client_wants_cancel.set()
                                self._active_interact_task.cancel()
                                with suppress(asyncio.CancelledError):
                                    await self._active_interact_task

                            await self.handle_client_event(event)
                        elif "bytes" in message:
                            await self.voice_io_manager.add_audio(message["bytes"])
                    elif message["type"] == "websocket.disconnect":
                        await self._clear_websocket()
                        self.logger.info(f"UI session {self.ui_session_id} disconnected normally")
                        break

                except WebSocketDisconnect:
                    await self._clear_websocket()
                    self.logger.info(f"UI session {self.ui_session_id} disconnected normally")
                    break
                except json.JSONDecodeError:
                    await self.send_error("Invalid JSON received")
                except Exception as e:
                    self.logger.exception(f"Error handling event for session {self.ui_session_id}: {e}")
                    await self.send_error(f"Error processing event: {str(e)}")
        finally:
            self.logger.info(f"RealtimeBridge stopped servicing websocket for session {self.ui_session_id}")
            # Ensure websocket is cleared if we exit the loop
            if self.is_connected:
                await self._clear_websocket()

    async def raise_render_media_markdown(self, text: str, sent_by_class: str = "RealtimeBridge"):
        event = RenderMediaEvent(content=text, session_id=self.chat_session.session_id,
                                 content_type="text/markdown", sent_by_class= "RealtimeBridge", foreign_content=False,
                                 user_session_id=self.chat_session.session_id,
                                 role="assistant")

        await self.send_event(event)

    async def process_text_input(self, text: str, file_ids: Optional[List[str]] = None):
        handled_command = await self.command_handler.handle_command(text, self)
        if handled_command:
            return

        if self._active_interact_task and not self._active_interact_task.done():
            self.client_wants_cancel.set()
            self._active_interact_task.cancel()
            with suppress(asyncio.CancelledError):
                await self._active_interact_task

        self.client_wants_cancel.clear()

        # let run() keep reading the socket
        self._active_interact_task = asyncio.create_task(
            self.interact(user_message=text, file_ids=file_ids),
            name=f"interact-{self.chat_session.session_id}"
        )

    async def iter_interact(self, text: str, file_ids: Optional[List[str]] = None, max_buffer: int = 512) -> AsyncIterator[str]:
        """
        Wraps the interaction in an async generator to yield text chunks
        as they are received from the agent so that they can be processed
        by TTS system in real-time.

        Usage:
            async for chunk in iter_interact(agent, "Hello"):
                yield chunk  # -> pipe to TTS

        Backpressure policy: if producer is faster than consumer, we drop the oldest
        item to keep latency low (tweak as you like).
        """
        queue: asyncio.Queue[Any] = asyncio.Queue(maxsize=max_buffer)
        DONE = object()

        def on_event(event: BaseEvent):
            if event.type == "text_delta":
                try:
                    queue.put_nowait(event.content)
                except asyncio.QueueFull:
                    with suppress(Exception):
                        queue.get_nowait()
                    queue.put_nowait(event.content)

            self.runtime_callback(event)

        async def run_chat():
            try:
                await self.interact(text, file_ids, on_event)
            except Exception as e:
                # propagate runtime errors
                await queue.put(e)
            finally:
                await queue.put(DONE)

        # Run your chat in parallel while we consume the queue.
        task = asyncio.create_task(run_chat())

        try:
            while True:
                item = await queue.get()
                if item is DONE:
                    break
                if isinstance(item, BaseException):
                    raise item
                yield item  # item is a str chunk
        finally:
            # If the consumer stops early (barge-in, stop button), cancel the chat.
            if not task.done():
                task.cancel()
                with suppress(asyncio.CancelledError):
                    await task

    async def _build_prompt_metadata(self) -> Dict[str, Any]:
        """
        Build metadata for prompts including user and session information.

        Creates a comprehensive metadata dictionary that provides context
        for prompt generation, including session details, user information,
        and system configuration.

        Returns:
            Dict[str, Any]: Metadata dictionary containing:
                - session_id: Session ID for the chat session (not UI session ID)
                - current_user_username: Username of the current user
                - persona_prompt: Prompt for the persona
                - agent_config: Complete agent configuration
                - voice_tools: Voice tools configuration
                - timestamp: Current timestamp in ISO format
                - env_name: Environment name (development, production, etc.)
        """
        agent_meta = self.chat_session.agent_config.prompt_metadata or {}
        return {
            "session_id": self.chat_session.session_id,
            "current_user_username": self.chat_session.user_id,
            "persona_prompt": self.chat_session.agent_config.persona,
            "agent_config": self.chat_session.agent_config,
            "timestamp": datetime.now().isoformat(),
            "env_name": os.getenv('ENV_NAME', "development"),
            "user_session_id": self.chat_session.session_id,
            "chat_session": self.chat_session,
            "chat_user": self.chat_user,
        } | agent_meta

    async def send_to_all_user_sessions(self, event: BaseEvent):
        """Send an event to all sessions for the current user"""
        await self.ui_session_manager.send_to_all_user_sessions(self.chat_user.user_id, event)

    async def _tool_context(self, on_event: Optional[callable] = None, prompt_metadata: Optional[Dict[str, Any]] = None) -> Dict[str, Any]:
        if prompt_metadata is None:
            prompt_metadata = await self._build_prompt_metadata()

        return {'active_agent': self.chat_session.agent_config,
                'bridge': self,
                'parent_session_id': None,
                'user_session_id': self.chat_session.session_id,
                'user_id': self.chat_session.user_id,
                'session_id': self.chat_session.session_id,
                "client_wants_cancel": self.client_wants_cancel,
                "env_name": os.getenv('ENV_NAME', 'development'),
                "streaming_callback": on_event if on_event is not None else self.runtime_callback,
                "prompt_metadata": prompt_metadata}

    async def interact(self, user_message: str, file_ids: Optional[List[str]] = None, on_event: Optional[callable] = None) -> None:
        """
        Streams chat responses for a given user message.

        This method handles the complete chat interaction process, including:
        - Session updates
        - Custom prompt integration
        - Message processing
        - Response streaming
        - Event handling

        Args:
            user_message (str): The message from the user to process
            file_ids (List[str], optional): IDs of files to include with the message
            on_event (callable, optional): Callback function to override the default runtime callback
                                           the iter_interact method uses to stream text chunks
        Raises:
            Exception: Any errors during chat processing
        """
        self.client_wants_cancel.clear()
        await self.send_user_turn_end()
        try:
            await self.chat_session_manager.update()
            self.chat_session.touch()
            await self.send_to_all_user_sessions(ChatSessionAddedEvent(chat_session=self.chat_session.as_index_entry()))

            agent_runtime = self.runtime_cache.runtime_for_agent(self.chat_session.agent_config)
            file_inputs = []
            if file_ids and self.file_handler:
                file_inputs = await self.process_files_for_message(file_ids, self.chat_session.session_id)

                # Log information about processed files
                if file_inputs:
                    input_types = {type(input_obj).__name__: 0 for input_obj in file_inputs}
                    for input_obj in file_inputs:
                        input_types[type(input_obj).__name__] += 1
                    self.logger.info(f"Processing {len(file_inputs)} files: {input_types}")

            prompt_metadata = await self._build_prompt_metadata()
            tool_params = {}
            if len(self.chat_session.agent_config.tools):
                await self.tool_chest.initialize_toolsets(self.chat_session.agent_config.tools)
                tool_params = self.tool_chest.get_inference_data(self.chat_session.agent_config.tools, agent_runtime.tool_format)
                tool_params['schemas'] = self.chat_session.agent_config.filter_allowed_tools(tool_params['schemas'])
                tool_params["toolsets"] = self.chat_session.agent_config.tools

            if "ThinkTools" in self.chat_session.agent_config.tools:
                agent_sections = [ThinkSection(), EnvironmentInfoSection(),  DynamicPersonaSection(), MarkdownFormatting()]
            else:
                agent_sections = [EnvironmentInfoSection(), DynamicPersonaSection(), MarkdownFormatting()]

            chat_params: Dict[str, Any] = {
                "user_id": self.chat_session.user_id,
                "chat_session": self.chat_session,
                "tool_chest": self.tool_chest,
                "user_message": user_message,
                "prompt_metadata": prompt_metadata,
                "client_wants_cancel": self.client_wants_cancel,
                "streaming_callback": on_event  if on_event else self.runtime_callback,
                'tool_context': await self._tool_context(on_event, prompt_metadata),
                'prompt_builder': PromptBuilder(sections=agent_sections),
            }

            # Categorize file inputs by type to pass to appropriate parameters
            image_inputs = [input_obj for input_obj in file_inputs
                            if isinstance(input_obj, ImageInput)]
            audio_inputs = [input_obj for input_obj in file_inputs
                            if isinstance(input_obj, AudioInput)]
            document_inputs = [input_obj for input_obj in file_inputs
                               if isinstance(input_obj, FileInput) and
                               not isinstance(input_obj, ImageInput) and
                               not isinstance(input_obj, AudioInput)]

            # Only add parameters if there are inputs of that type
            if image_inputs:
                chat_params["images"] = image_inputs
            if audio_inputs:
                chat_params["audio_clips"] = audio_inputs
            if document_inputs:
                chat_params["files"] = document_inputs

            full_params = chat_params | tool_params
        except Exception as e:
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            self.logger.error(f"Error preparing chat parameters: {error_type}: {str(e)}\n{error_traceback}")
            await self.send_error(f"Error preparing chat parameters: {error_type}: {str(e)}\n{error_traceback}")
            await self.send_user_turn_start()
            return

        try:
            await agent_runtime.chat(**full_params)
        except Exception as e:
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            self.logger.error(f"Error in agent_runtime.chat: {error_type}: {str(e)}\n{error_traceback}")
            await  self.send_error( f"Error in agent_runtime.chat: {error_type}: {str(e)}\n{error_traceback}")
            await self.send_user_turn_start()
            return

        try:
            await self.flush_session()
            await self.send_user_turn_start()

        except Exception as e:
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            self.logger.error(f"Error flushing session manager: {error_type}: {str(e)}\n{error_traceback}")
            await self.send_error(f"Error flushing session manager: {error_type}: {str(e)}\n{error_traceback}")
            await self.send_user_turn_start()
            return

    async def _get_or_create_chat_session(self, session_id: Optional[str] = None, user_id: Optional[str] = None, agent_key: str = 'default_realtime') -> ChatSession:
        session_id = session_id or f"{self.chat_user.user_id}-{MnemonicSlugs.generate_slug(2)}"
        user_id = user_id or self.chat_user.user_id
        chat_session = await self.chat_session_manager.get_session(session_id, user_id)

        if chat_session is None:
            agent_config = self.ui_session_manager.agent_config_loader.duplicate(agent_key)
            user_id = user_id or self.chat_user.user_id
            chat_session = ChatSession(session_id=session_id, agent_config=agent_config, user_id=user_id)
        else:
            chat_session.agent_config = self.ui_session_manager.agent_config_loader.duplicate(agent_key)

        return chat_session

    async def initialize(self, chat_session_id: Optional[str] = None, agent_key: Optional[str] = None) -> None:
        self.chat_session = await self._get_or_create_chat_session(session_id=chat_session_id, agent_key=agent_key)
        await self.initialize_agent_parameters()

    async def process_files_for_message(
            self,
            file_ids: List[str],
            session_id: str
    ) -> List[Union[FileInput, ImageInput, AudioInput]]:
        """
        Process files and convert them to appropriate Input objects for the agent.

        This method processes uploaded files and converts them to the appropriate input
        objects (FileInput, ImageInput, AudioInput) for handling by the agent's multimodal
        capabilities.

        Args:
            file_ids: List of file IDs to process.
            session_id: Session ID for file processing context.

        Returns:
            List of input objects for the agent, typed as FileInput, ImageInput, or AudioInput.

        Raises:
            Exception: If file processing fails (logged but not re-raised).
        """
        if not self.file_handler or not file_ids:
            return []

        input_objects = []

        for file_id in file_ids:
            # Get file metadata
            metadata = self.file_handler.get_file_metadata(file_id, session_id)
            if not metadata:
                metadata = await self.file_handler.process_file(file_id, session_id)

            if not metadata:
                self.logger.warning(f"Could not get metadata for file {file_id}")
                continue

            if isinstance(metadata, dict):
                metadata = FileMetadata.model_validate(metadata)

            # Create the appropriate input object based on file type
            input_obj = self.file_handler.get_file_as_input(file_id, session_id)
            if input_obj:
                self.logger.info(f"Created {type(input_obj).__name__} for file {metadata.original_filename}")
                input_objects.append(input_obj)
            else:
                self.logger.warning(f"Failed to create input object for file {metadata.original_filename}")

        return input_objects


    async def initialize_agent_parameters(self) -> None:
        """
        Initialize the internal agent with prompt builders, tools, and configurations.

        This method creates and configures the conversational agent for the application.
        It sets up the agent's prompt builder and initializes the appropriate agent
        class based on the specified backend (Claude, Bedrock, or OpenAI).

        Process:
            1. Build the system prompt using configured sections
            2. Prepare common agent parameters
            3. Add backend-specific parameters (temperature, reasoning settings, etc.)
            4. Initialize the appropriate agent class based on backend

        Raises:
            Exception: If an error occurs during agent initialization.

        Note:
            Sets self.agent_runtime to the initialized agent instance, which will be
            one of ClaudeChatAgent, ClaudeBedrockChatAgent, or GPTChatAgent.
        """
        agent_params = self.chat_session.agent_config.agent_params.model_dump(exclude_none=True)

        agent_params |= {
            "tool_chest": self.tool_chest,
            "streaming_callback": self.runtime_callback
        }

        self.logger.info(f"Agent initialized using the following parameters: {agent_params}")