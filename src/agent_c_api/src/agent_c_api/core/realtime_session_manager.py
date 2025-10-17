import os
import asyncio
import json
import threading

from typing import Dict, Optional, List, Any, Union

from agent_c.models import ChatUser
from agent_c.toolsets import ToolCache, ToolChest

from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.config import ModelConfigurationLoader
from agent_c.chat.session_manager import ChatSessionManager
from agent_c_api.core.realtime_bridge import RealtimeBridge
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.models.realtime_session import RealtimeSession
from agent_c_tools.tools.workspace.base  import BaseWorkspace
from agent_c_api.models.user_runtime_cache_entry import UserRuntimeCacheEntry


from agent_c_tools import *  # noqa
from agent_c_tools.tools.in_process import * # noqa



# Constants
DEFAULT_BACKEND = 'claude'
DEFAULT_MODEL_NAME = 'claude-sonnet-4-20250514'
DEFAULT_OUTPUT_FORMAT = 'raw'
DEFAULT_TOOL_CACHE_DIR = '.tool_cache'
DEFAULT_LOG_DIR = './logs/sessions'
LOCAL_WORKSPACES_FILE = '.local_workspaces.json'
DEFAULT_ENV_NAME = 'development'
OPENAI_REASONING_MODELS = ['o1', 'o1-mini', 'o3', 'o3-mini']

DEFAULT_TOOLSETS = "ThinkTools,WorkspaceTools,AgentCloneTools,AgentAssistTools,AgentTeamTools,WorkspacePlanningTools,BridgeTools,MarkdownToHtmlReportTools,DynamicCommandTools"


class RealtimeSessionManager:
    """
    Maintains the connection between client UI
    """

    def __init__(self, session_manager: ChatSessionManager):
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()
        self.workspaces: Optional[List[BaseWorkspace]] = None
        self.agent_config_loader: AgentConfigLoader = AgentConfigLoader()
        self.model_config_loader = ModelConfigurationLoader()
        self.model_configs: Dict[str, Any] = self.model_config_loader.flattened_config()
        self.tool_cache_dir = DEFAULT_TOOL_CACHE_DIR

        self.user_runtime_cache: Dict[str, UserRuntimeCacheEntry] = {}
        self.ui_sessions: Dict[str, RealtimeSession] = {}
        self._locks: Dict[str, asyncio.Lock] = {}
        self._cancel_events: Dict[str, threading.Event] = {}
        self.agent_config_loader: AgentConfigLoader = AgentConfigLoader()
        self.chat_session_manager: ChatSessionManager = session_manager

    @staticmethod
    def _init__user_workspaces(user_id: str) -> List[BaseWorkspace]:
        local_project = LocalProjectWorkspace()
        user_uploads = LocalStorageWorkspace(workspace_path=f"uploads/{user_id}", name="Uploads", description="Files the user had uploaded")
        workspaces: List[BaseWorkspace] = [local_project, user_uploads]

        try:
            with open(LOCAL_WORKSPACES_FILE, 'r', encoding='utf-8') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            # Local workspaces file is optional
            pass

        return workspaces


    def get_session_data(self, ui_session_id: str) -> Optional[RealtimeSession]:
        """
        Retrieve session data for a given session ID.

        Args:
            ui_session_id (str): The unique identifier for the session

        Returns:
            Optional[RealtimeSession]: The session data if found, else None
        """
        return self.ui_sessions.get(ui_session_id, None)

    def get_user_session_data(self, ui_session_id: str, user_id: str) ->  Optional[RealtimeSession]:
        session = self.get_session_data(ui_session_id)

        if not session or session.user_id != user_id:
            return None

        return session

    async def get_or_create_realtime_session(self, user: ChatUser, session_id: Optional[str] = None) -> RealtimeSession:
        """
        Retrieve an existing session or create a new one if it doesn't exist.

        Args:
            user: The chat user for the session
            session_id: Optional existing session ID to retrieve
        Returns:
            RealtimeSession: The existing or newly created session
        """
        if session_id and session_id in self.ui_sessions:
            return self.ui_sessions[session_id]

        return await self.create_realtime_session(user)


    async def create_realtime_session(self, user: ChatUser, session_id: Optional[str] = None,
                                      chat_session_id: Optional[str] = None,
                                      agent_key: Optional[str] = None) -> RealtimeSession:
        """
        Create a new session or return existing session for reconnection.

        Args:
            user: The chat user for the session
            session_id: Optional session ID to use; if None, a new ID is generated
            chat_session_id: Optional chat session ID to resume
            agent_key: Optional agent key to initialize the session with a specific agent

        Returns:
            RealtimeSession: The session (new or existing)
            
        Note:
            If session already exists for this user, returns it for reconnection.
            The caller should then call bridge.reconnect(websocket) before bridge.run().
        """
        ui_session_id = session_id if session_id else RealtimeSession.generate_session_id(user.user_id)

        if ui_session_id in self.ui_sessions:
            if self.ui_sessions[ui_session_id].user_id == user.user_id:
                self.logger.info(f"Found existing session {ui_session_id} for reconnection")
                return self.ui_sessions[ui_session_id]
            else:
                raise ValueError(f"Session ID {ui_session_id} already exists for a different user.")

        self._locks[ui_session_id] = asyncio.Lock()

        runtime_cache_entry = await self.create_user_runtime_cache_entry(user.user_id)

        async with self._locks[ui_session_id]:
            agent_bridge = RealtimeBridge(self, user, ui_session_id, self.chat_session_manager, runtime_cache_entry)
            await agent_bridge.initialize(chat_session_id, agent_key)
            self.ui_sessions[ui_session_id] = RealtimeSession(session_id=ui_session_id, user_id=user.user_id, bridge=agent_bridge)

            self.logger.info(f"Session {ui_session_id} created")
            return self.ui_sessions[ui_session_id]

    async def cleanup_session(self, ui_session_id: str):
        """
        Clean up resources associated with a specific session.

        Removes session data and releases associated locks. Any errors during
        cleanup are logged but don't halt execution.

        Args:
            ui_session_id (str): The unique identifier of the session to clean up

        Note:
            This method is safe to call multiple times on the same session ID
        """
        if ui_session_id in self.ui_sessions:
            try:
                del self.ui_sessions[ui_session_id]
                if ui_session_id in self._locks:
                    del self._locks[ui_session_id]

            except Exception as e:
                self.logger.error(f"Error cleaning up session {ui_session_id}: {e}")

    def cancel_interaction(self, ui_session_id: str) -> bool:
        """
        Cancel an ongoing interaction for the specified session.

        Args:
            ui_session_id: The session identifier

        Returns:
            bool: True if cancellation was triggered, False if session not found
        """
        if ui_session_id not in self.ui_sessions:
            return False

        # Set the event to signal cancellation
        self.logger.info(f"Cancelling interaction for session: {ui_session_id}")
        self.ui_sessions[ui_session_id].bridge.client_wants_cancel.set()
        return True

    async def send_to_all_user_sessions(self, user_id: str, event):
        """
        Send and event to all sessions associated with a specific user.

        Args:
            user_id (str): The user ID to send the message to
            event: The event to send
        """
        sessions: List[RealtimeSession] = [session for session in self.ui_sessions.values() if session.user_id == user_id]
        for session in sessions:
            await session.bridge.send_event(event)


    async def create_user_runtime_cache_entry(self, user_id: str, hotload_toolsets: Optional[Union[str, List[str]]] = None) -> UserRuntimeCacheEntry:
        """
        Create or retrieve a runtime cache entry for a user.

        Args:
            user_id (str): The user ID to create the cache entry for
        Returns:
            UserRuntimeCacheEntry: The created or existing cache entry
        """
        if user_id in self.user_runtime_cache:
            return self.user_runtime_cache[user_id]

        if hotload_toolsets is None:
            hotload_toolsets = os.environ.get("HOTLOAD_TOOLSETS", DEFAULT_TOOLSETS)


        if isinstance(hotload_toolsets, str):
            hotload_toolsets = [ts.strip() for ts in hotload_toolsets.split(",")]


        workspaces = self._init__user_workspaces(user_id)
        tool_cache = ToolCache(cache_dir=self.tool_cache_dir)

        tool_opts = {
            'tool_cache': tool_cache,
            'session_manager': self.chat_session_manager,
            'workspaces': workspaces,
            'streaming_callback': None,
            'model_configs': self.model_config_loader.get_cached_config()
        }

        tool_chest = ToolChest(**tool_opts)
        await tool_chest.init_tools(tool_opts)
        await tool_chest.activate_toolset(hotload_toolsets)

        cache_entry = UserRuntimeCacheEntry(
            user_id=user_id,
            tool_chest=tool_chest,
            tool_cache=tool_cache,
            model_configs=self.model_configs,
            workspaces=workspaces
        )

        self.user_runtime_cache[user_id] = cache_entry
        return cache_entry