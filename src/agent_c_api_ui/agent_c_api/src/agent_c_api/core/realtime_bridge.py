import asyncio
import copy
import json
import os
import traceback
from contextlib import suppress
from datetime import datetime
from typing import List, Optional, Any, Dict, AsyncIterator

from fastapi import WebSocket, WebSocketDisconnect
from functools import singledispatchmethod

from starlette.websockets import WebSocketState
from websockets import State

from agent_c.chat import ChatSessionManager
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.models import ChatSession, ChatUser
from agent_c.models.events import BaseEvent, TextDeltaEvent, HistoryEvent, RenderMediaEvent, SystemMessageEvent
from agent_c.models.events.chat import AudioInputDeltaEvent
from agent_c.models.heygen import HeygenAvatarSessionData, NewSessionRequest
from agent_c.toolsets import Toolset
from agent_c.util import MnemonicSlugs
from agent_c.util.heygen_streaming_avatar_client import HeyGenStreamingClient
from agent_c.util.registries.event import EventRegistry
from agent_c_api.api.rt.models.control_events import GetAgentsEvent, ErrorEvent, AgentListEvent, GetAvatarsEvent, AvatarListEvent, TextInputEvent, SetAvatarEvent, AvatarConnectionChangedEvent, \
    SetAgentEvent, AgentConfigurationChangedEvent, SetAvatarSessionEvent, ChatSessionChangedEvent, ResumeChatSessionEvent, \
    NewChatSessionEvent, SetAgentVoiceEvent, AgentVoiceChangedEvent, UserTurnStartEvent, UserTurnEndEvent, GetUserSessionsEvent, GetUserSessionsResponseEvent, PingEvent, PongEvent, \
    GetToolCatalogEvent, ToolCatalogEvent, ChatUserDataEvent, GetVoicesEvent, VoiceListEvent, ChatSessionAddedEvent, DeleteChatSessionEvent, ClientWantsCancelEvent, CancelledEvent
from agent_c_api.api.rt.models.control_events import SetChatSessionNameEvent, SetSessionMessagesEvent, ChatSessionNameChangedEvent, SetSessionMetadataEvent, SessionMetadataChangedEvent
from agent_c_api.core.agent_bridge import AgentBridge
from agent_c.models.input import AudioInput

from agent_c.models.input.file_input import FileInput
from agent_c_api.core.file_handler import RTFileHandler
from agent_c.models.input.image_input import ImageInput
from agent_c_api.core.voice.models import open_ai_voice_models, AvailableVoiceModel, heygen_avatar_voice_model, no_voice_model
from agent_c_api.core.voice.voice_io_manager import VoiceIOManager
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c.prompting import PromptBuilder, EnvironmentInfoSection
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection


class RealtimeBridge(AgentBridge):
    def __init__(self,
                 chat_user: ChatUser,
                 ui_session_id: str,
                 session_manager: ChatSessionManager):
        file_handler = RTFileHandler(chat_user.user_id, ui_session_id)

        super().__init__(None, session_manager, file_handler)
        self._is_new_bridge = True
        self.chat_session_manager: ChatSessionManager = session_manager
        self.chat_user: ChatUser = chat_user
        self.ui_session_id: str = ui_session_id

        self.websocket: Optional[WebSocket] = None
        self.is_running = False
        self.agent_config_loader: AgentConfigLoader = AgentConfigLoader()
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

    async def flush_session(self, touch: bool = True, chat_session: Optional[ChatSession] = None) -> None:
        """Flush the current chat session to persistent storage"""
        chat_session = chat_session or self.chat_session
        if chat_session:
            await self.session_manager.flush_session(chat_session, touch)


    # Handlers for events coming from the client websocket
    @singledispatchmethod
    async def handle_client_event(self, event: BaseEvent) -> None:
        """Default handler for unknown events"""
        self.logger.warning(f"RealtimeBridge {self.ui_session_id}: Unhandled event type: {event.type}")
        # await self.send_error(f"Unknown event type: {event.type}")

    @handle_client_event.register
    async def _(self, _: PingEvent) -> None:
        await self.send_event(PongEvent())

    @handle_client_event.register
    async def _(self, _: GetAgentsEvent) -> None:
        await self.send_agent_list()

    @handle_client_event.register
    async def _(self, _: ClientWantsCancelEvent) -> None:
        await self.cancel_interaction()

    async def cancel_interaction(self) -> None:
        self.client_wants_cancel.set()
        await self.send_event(CancelledEvent())
        await self.send_event(SystemMessageEvent(content="Interaction cancelled by user, waiting for runtime to exit..", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))

    @handle_client_event.register
    async def _(self, event: DeleteChatSessionEvent):
        await self.delete_chat_session(event.session_id)

    async def delete_chat_session(self, session_id: str) -> None:
        if session_id is None:
            session_id = self.chat_session.session_id

        success = await self.chat_session_manager.delete_session(session_id, self.chat_user.user_id)
        if success:
            self.logger.info(f"RealtimeBridge {self.ui_session_id}: Deleted chat session {session_id}")
        else:
            self.logger.warning(f"RealtimeBridge {self.ui_session_id}: Failed to delete chat session {session_id}")

        await self.send_event(DeleteChatSessionEvent(session_id=session_id))

        if session_id == self.chat_session.session_id:
            self.chat_session = None
            await self.new_chat_session()

        await self.send_event(SystemMessageEvent(content=f"Chat Session {session_id} deleted", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))


    @handle_client_event.register
    async def _(self, event: SetAgentVoiceEvent):
        if event.voice_id == "none":
            await self.set_agent_voice(no_voice_model)
            return

        voice = next((v for v in self._voices if v.voice_id == event.voice_id), None)
        if not voice:
            await self.send_error(f"Voice '{event.voice_id}' not found", source="set_agent_voice")
            return

        await self.set_agent_voice(voice)

    async def set_agent_voice(self, voice: AvailableVoiceModel) -> None:
        self.voice_io_manager.change_voice(voice)
        await self.send_event(AgentVoiceChangedEvent(voice=voice))

    @handle_client_event.register
    async def _(self, event: TextInputEvent) -> None:
        if self._active_interact_task and not self._active_interact_task.done():
            self.client_wants_cancel.set()
            self._active_interact_task.cancel()
            with suppress(asyncio.CancelledError):
                await self._active_interact_task

        self.client_wants_cancel.clear()
        # Fire-and-manage: don't await here, let run() keep reading the socket
        self._active_interact_task = asyncio.create_task(
            self.interact(user_message=event.text, file_ids=event.file_ids),
            name=f"interact-{self.chat_session.session_id}"
        )

    async def send_agent_list(self) -> None:
        catalog = self.agent_config_loader.client_catalog
        await self.send_event(AgentListEvent(agents=catalog))

    @handle_client_event.register
    async def _(self, _: GetAvatarsEvent) -> None:
        await self.send_avatar_list()

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

    @handle_client_event.register
    async def _(self, event: SetAvatarEvent) -> None:
        await self.set_avatar(event.avatar_id, event.quality, event.video_encoding)

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

    @handle_client_event.register
    async def _(self, event: AudioInputDeltaEvent) -> None:
        self.logger.info("RealtimeBridge received audio input delta event")

    @handle_client_event.register
    async def _(self, event: SetAvatarSessionEvent) -> None:
        """Set the avatar session using the provided access token and session ID"""
        await self.set_avatar_session(event.access_token, event.avatar_session_id)

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

    @handle_client_event.register
    async def _(self, event: ResumeChatSessionEvent) -> None:
        await self.resume_chat_session(event.session_id)

    async def resume_chat_session(self, session_id: str) -> None:
        await self.release_current_session()
        session_info = await self.chat_session_manager.get_session(session_id, self.chat_user.user_id)
        if not session_info or session_info.user_id != self.chat_user.user_id:
            await self.send_error(f"Session '{session_id}' not found", source="resume_chat_session")
            return

        self.chat_session = session_info

        if 'BridgeTools' not in self.chat_session.agent_config.tools:
            self.chat_session.agent_config.tools.append('BridgeTools')

        await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        self.logger.info(f"RealtimeBridge resumed chat session {self.chat_session.session_id}")
        await self.send_chat_session()
        await self.send_event(SystemMessageEvent(content=F"Welcome back! **Session ID:** {self.chat_session.session_id}", session_id=self.chat_session.session_id,
                                                 severity="info", role="system"))


    @handle_client_event.register
    async def _(self, event: SetChatSessionNameEvent) -> None:
        await self.set_chat_session_name(event.session_name, event.session_id)

    async def rename_current_session(self, new_name: str):
        await self.set_chat_session_name(new_name)

    async def set_chat_session_name(self, session_name: str, session_id: Optional[str] = None) -> None:
        """Set the name of the current chat session"""
        if session_id is None:
            session_id = self.chat_session.session_id

        entry = await self.chat_session_manager.rename_session(session_id, self.chat_session.user_id, session_name)
        self.logger.info(f"RealtimeBridge {self.chat_session.session_id}: Session name set to '{session_name}'")

        await self.send_event(ChatSessionNameChangedEvent(session_name=entry.session_name, session_id=entry.session_id))

    @handle_client_event.register
    async def _(self, event: GetUserSessionsEvent) -> None:
        await self.send_user_sessions(event.offset, event.limit)

    async def send_user_sessions(self, offset: int, limit: int = 50) -> None:
        sessions = await self.chat_session_manager.get_user_sessions(self.chat_user.user_id, offset, limit)
        await self.send_event(GetUserSessionsResponseEvent(sessions=sessions))

    @handle_client_event.register
    async def _(self, event: SetSessionMetadataEvent) -> None:
        await self.set_session_metadata(event.meta)

    async def set_session_metadata(self, meta: Dict[str, Any]) -> None:
        """Set the metadata for the current chat session"""
        self.chat_session.metadata = meta
        self.logger.info(f"RealtimeBridge {self.chat_session.session_id}: Session metadata updated")

        await self.send_chat_session_meta()

    @handle_client_event.register
    async def _(self, event: NewChatSessionEvent) -> None:
        await self.new_chat_session(event.agent_key)

    async def release_current_session(self) -> None:
        if self.chat_session is not None:
            return
        await self.chat_session_manager.release_session(self.chat_session.session_id, self.chat_session.user_id)

    async def new_chat_session(self, agent_key: Optional[str] = None) -> None:
        self.logger.info(f"Creating new chat session with {agent_key} from {self.chat_session.session_id}")
        agent_key = agent_key or self.chat_session.agent_config.key
        await self.flush_session()
        await self.release_current_session()

        session_id = f"{self.chat_session.user_id}-{MnemonicSlugs.generate_slug(2)}"
        self.chat_session =  await self._get_or_create_chat_session(session_id=session_id, agent_key=agent_key)
        await self.send_chat_session()

    @handle_client_event.register
    async def _(self, event: GetVoicesEvent):
        await self.send_voices()

    async def send_voices(self) -> None:
        voices = [no_voice_model, heygen_avatar_voice_model] + open_ai_voice_models
        await self.send_event(VoiceListEvent(voices=voices))

    @handle_client_event.register
    async def _(self, event: GetToolCatalogEvent):
        await self.send_tool_catalog()

    async def send_tool_catalog(self) -> None:
        await self.send_event(ToolCatalogEvent(tools=Toolset.get_client_registry()))

    async def send_user_info(self) -> None:
        await self.send_event(ChatUserDataEvent(user=self.chat_user))

    @handle_client_event.register
    async def _(self, event: SetSessionMessagesEvent) -> None:
        await self.set_session_messages(event.messages)

    async def set_session_messages(self, messages: List[Dict[str, Any]]) -> None:
        """Set the messages for the current chat session"""
        self.chat_session.messages = messages
        self.logger.info(f"RealtimeBridge {self.chat_session.session_id}: Session messages updated")
        await self.flush_session()
        await self.send_event(HistoryEvent(messages=self.chat_session.messages, session_id=self.chat_session.session_id))

    @handle_client_event.register
    async def _(self, event: SetAgentEvent) -> None:
        await self.set_agent(event.agent_key)

    async def set_agent(self, agent_key: str) -> None:
        """Set the agent for the current session"""
        if not self.chat_session.agent_config or self.chat_session.agent_config.key != agent_key:
            agent_config = self.agent_config_loader.duplicate(agent_key)
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
        except Exception as e:
            self.logger.exception(f"Failed to send event to {self.chat_session.session_id}: {e}")
            self.is_running = False

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
            self.logger.error(f"RealtimeBridge {self.chat_session.session_id}: No active avatar session to send message")
            return
        try:
            await self.avatar_client.send_task(text)
        except Exception as e:
            self.logger.error(f"RealtimeBridge {self.chat_session.session_id}: Failed to send message to avatar: {e}")
            await self.send_error(f"Failed to send message to avatar: {str(e)}")

        await self.send_event(TextDeltaEvent(
            content=text,
            session_id=self.chat_session.session_id,
            role=role
        ))

    async def runtime_callback(self, event: BaseEvent):
        """Handle runtime events from the agent"""
        #if event.type not in ["ping", "pong"]:
            #self.logger.debug(f"RealtimeBridge {self.chat_session.session_id}: Received runtime event: {event.type}")
        # These are already in model format and don't need parsed.
        await self.handle_runtime_event(event)
        await asyncio.sleep(0)

    async def send_client_initial_data(self):
        if self._is_new_bridge:
            await self.send_user_info()
            await self.send_avatar_list()
            await self.send_voices()
            await self.send_agent_list()
            await self.send_tool_catalog()
            self._is_new_bridge = False

        await self.send_chat_session()
        message = "# Welcome to Agent C\n\nFirst time here? Send '*Hello Domo*' to get started!"

        if len(self.chat_session.messages) > 0:
            message = "# Welcome back Agent C\n\nYour previous session has been restored."

        await self.raise_render_media_markdown(message)

    async def run(self, websocket: WebSocket, chat_session_id: Optional[str] = None):
        """Main run loop for the bridge"""
        await websocket.accept()
        self.websocket=websocket
        self.is_running = True

        if chat_session_id is not None and (self.chat_session is None or self.chat_session.session_id != chat_session_id):
            self.chat_session = await self._get_or_create_chat_session(session_id=chat_session_id, user_id=self.chat_user.user_id)

        self.logger.info (f"RealtimeBridge started for session {self.chat_session.session_id}")
        await self.send_client_initial_data()
        await self.send_user_turn_start()

        try:
            while self.is_running:
                try:
                    message = await self.websocket.receive() # .receive_json()
                    if message["type"] == "websocket.receive":
                        if "text" in message:
                            event = self.parse_event(json.loads(message["text"]))
                            if event.type not in ["ping", "pong"]:
                                self.logger.info(f"Received event {event.type} from session {self.chat_session.session_id}")

                            if event.type == "resume_chat_session" and event.session_id == self.chat_session.session_id:
                                self.logger("Client requested to resume the current session, ignoring.")
                                await self.send_chat_session()
                                break

                            if self._active_interact_task and not self._active_interact_task.done() and event.type not in ["ping", "pong", "client_wants_cancel"]:
                                self.logger.warning(f"Session {self.chat_session.session_id} has an active interaction, shutting down interaction due to  new event {event.type}")
                                self.client_wants_cancel.set()
                                self._active_interact_task.cancel()
                                with suppress(asyncio.CancelledError):
                                    await self._active_interact_task

                            await self.handle_client_event(event)
                        elif "bytes" in message:
                            await self.voice_io_manager.add_audio(message["bytes"])
                    elif message["type"] == "websocket.disconnect":
                        self.websocket = None
                        self.logger.info(f"Session {self.chat_session.session_id} disconnected normally")
                        break

                except WebSocketDisconnect:
                    self.websocket = None
                    self.logger.info(f"Session {self.ui_session_id} disconnected normally")
                    break
                except json.JSONDecodeError:
                    await self.send_error("Invalid JSON received")
                except Exception as e:
                    self.logger.exception(f"Error handling event for session {self.ui_session_id}: {e}")
                    await self.send_error(f"Error processing event: {str(e)}")
        finally:
            self.logger.info(f"RealtimeBridge stopped for session {self.ui_session_id}")

    async def raise_render_media_markdown(self, text: str):
        event = RenderMediaEvent(content=text, session_id=self.chat_session.session_id,
                                 content_type="text/markdown", sent_by_class= "RealtimeBridge", foreign_content=False,
                                 user_session_id=self.chat_session.session_id,
                                 role="assistant")

        await self.send_event(event)


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

    async def interact(self, user_message: str, file_ids: Optional[List[str]] = None, on_event: Optional[callable] = None, send_turn: bool = True) -> None:
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
            await self.session_manager.update()
            if len(self.chat_session.messages) == 0:
                await self.send_event(ChatSessionAddedEvent(chat_session=self.chat_session.as_index_entry()))

            agent_runtime = self.runtime_for_agent(self.chat_session.agent_config)
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

            if self.sections is not None:
                agent_sections = self.sections
            elif "ThinkTools" in self.chat_session.agent_config.tools:
                agent_sections = [ThinkSection(), EnvironmentInfoSection(),  DynamicPersonaSection()]
            else:
                agent_sections = [EnvironmentInfoSection(), DynamicPersonaSection()]

            chat_params: Dict[str, Any] = {
                "user_id": self.chat_session.user_id,
                "chat_session": self.chat_session,
                "tool_chest": self.tool_chest,
                "user_message": user_message,
                "prompt_metadata": prompt_metadata,
                "client_wants_cancel": self.client_wants_cancel,
                "streaming_callback": on_event  if on_event else self.runtime_callback,
                'tool_context': {'active_agent': self.chat_session.agent_config,
                                 'bridge': self,
                                 'parent_session_id': None,
                                 'user_session_id': self.chat_session.session_id,
                                 'user_id': self.chat_session.user_id,
                                 'session_id': self.chat_session.session_id,
                                 "client_wants_cancel": self.client_wants_cancel,
                                 "env_name": os.getenv('ENV_NAME', 'development'),
                                 "streaming_callback": on_event  if on_event is not None else self.runtime_callback,
                                 "prompt_metadata": prompt_metadata},
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
        session_id = session_id or self.ui_session_id
        user_id = user_id or self.chat_user.user_id
        chat_session = await self.chat_session_manager.get_session(session_id, user_id)

        if chat_session is None:
            agent_config = self.agent_config_loader.duplicate(agent_key)
            user_id = user_id or self.chat_user.user_id
            chat_session = ChatSession(session_id=session_id, agent_config=agent_config, user_id=user_id)

        return chat_session

    async def initialize(self) -> None:
        self.chat_session = await self._get_or_create_chat_session()
        await self._init_tool_chest()
        await self.initialize_agent_parameters()
