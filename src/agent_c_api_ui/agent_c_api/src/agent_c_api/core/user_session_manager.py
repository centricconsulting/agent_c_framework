import asyncio
import threading
import traceback

from typing import Dict, Optional, List

from agent_c.chat import DefaultChatSessionManager
from agent_c.util.slugs import MnemonicSlugs
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.chat.session_manager import ChatSessionManager, ChatSession
from agent_c_api.core.agent_bridge import AgentBridge
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.models.user_session import UserSession


class UserSessionManager:
    """
    Manages agent sessions in a multi-agent environment.

    This class handles the lifecycle of user sessions, including creation,
    cleanup, and response streaming from the runtime.

    Attributes:
        logger (logging.Logger): Instance logger
        ui_sessions (Dict[str, UserSession]): Active session storage
        _locks (Dict[str, asyncio.Lock]): Session operation locks
    """

    def __init__(self, chat_session_manager: ChatSessionManager):
        self.logger = LoggingManager(__name__).get_logger()
        self.ui_sessions: Dict[str, UserSession] = {}
        self._locks: Dict[str, asyncio.Lock] = {}
        self._cancel_events: Dict[str, threading.Event] = {}
        self.agent_config_loader: AgentConfigLoader = AgentConfigLoader.instance()
        self.chat_session_manager: DefaultChatSessionManager = chat_session_manager



    async def get_user_session(self, user_session_id: str) -> UserSession:
        """
        Retrieve session data for a given session ID.

        Args:
            user_session_id (str): The unique identifier for the session

        Returns:
            Dict[str, Any]: Session data dictionary containing agent and configuration
                information, or None if session doesn't exist
        """
        if user_session_id not in self.ui_sessions:
            return await self.create_user_session(existing_ui_session_id=user_session_id)

        return self.ui_sessions.get(user_session_id)

    async def create_user_session(self,
                                  agent_key: str = None,
                                  existing_ui_session_id: str = None,
                                  user_id: str = "Agent_C_User") -> UserSession:
        """
        Create a new session or update an existing session with a new agent.

        Args:
            agent_key: The key of the AgentConfiguration to use
            existing_ui_session_id: If provided, updates existing session instead of creating new one
            user_id: The user ID associated with the session (default: "Agent_C_User")

        Returns:
            str: The session ID (either new or existing)

        Raises:
            Exception: If agent initialization fails
        """
        # If updating existing session, use that ID, otherwise generate new one
        ui_session_id = existing_ui_session_id if existing_ui_session_id else MnemonicSlugs.generate_slug(3)

        if agent_key not in self.agent_config_loader.catalog:
            self.logger.warning(f"Agent key '{agent_key}' not found in catalog. Using default agent configuration.")
            agent_key = "default"


        if ui_session_id not in self._locks:
            self._locks[ui_session_id] = asyncio.Lock()

        async with self._locks[ui_session_id]:
            existing_session = self.ui_sessions.get(ui_session_id, None)

            if existing_session is not None:
                # This is the "client disconnected" case where we re-use an existing session
                if existing_session.chat_session.agent_config.key == agent_key:
                    return existing_session  # No change needed, return existing session
                else:
                    agent_config = self.agent_config_loader.duplicate(agent_key)
                    existing_session.chat_session.agent_config = agent_config
                    return existing_session

            if ui_session_id in self.chat_session_manager.session_id_list:
                # The user is resuming an existing session
                chat_session = await self.chat_session_manager.get_session(ui_session_id)

                if chat_session.agent_config.key != agent_key:
                    agent_config = self.agent_config_loader.duplicate(agent_key)
                    chat_session.agent_config = agent_config
                    chat_session.touch()
            else:
                # This is a legit new session creation
                agent_config = self.agent_config_loader.duplicate(agent_key)
                chat_session = ChatSession(session_id=ui_session_id, agent_config=agent_config, user_id=user_id)
                await self.chat_session_manager.new_session(chat_session)

            agent_bridge = AgentBridge(chat_session, self.chat_session_manager)
            await agent_bridge.initialize()

            user_session = UserSession(session_id=ui_session_id,
                                       chat_session=chat_session,
                                       agent_bridge=agent_bridge)

            self._cancel_events[ui_session_id] = user_session.cancel_event
            self.ui_sessions[ui_session_id] = user_session

            self.logger.info(f"Session {ui_session_id} created with agent: {agent_bridge}")
            return user_session

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
                # Clean up the cancel event
                if ui_session_id in self._cancel_events:
                    del self._cancel_events[ui_session_id]

                # Remove session data and lock
                del self.ui_sessions[ui_session_id]
                if ui_session_id in self._locks:
                    del self._locks[ui_session_id]

            except Exception as e:
                self.logger.error(f"Error cleaning up session {ui_session_id}: {e}")

    async def stream_response(
            self,
            ui_session_id: str,
            user_message: str,
            file_ids: Optional[List[str]] = None
    ):
        """
        Get streaming response from the agent for a given message.
        Uses ReactJSAgent's stream_chat method for proper streaming support.

        Args:
            ui_session_id: The session identifier
            user_message: The user's message to process
            file_ids: Optional list of file IDs to include with the message

        Yields:
            Chunks of the response as they become available

        Raises:
            ValueError: If the session ID is invalid
            Exception: If streaming fails
        """
        session_data: UserSession = await self.get_user_session(ui_session_id)
        if not session_data:
            raise ValueError(f"Invalid session ID: {ui_session_id}")

        agent_bridge: AgentBridge = session_data.agent_bridge

        try:
            # Get the cancel event for this session
            cancel_event = self._cancel_events.get(ui_session_id)
            cancel_event.clear()

            if file_ids is None or agent_bridge.file_handler is None:
                file_ids = []

            async for chunk in agent_bridge.stream_chat(user_message, cancel_event, file_ids=file_ids):
                yield chunk

        except Exception as e:
            self.logger.error(f"Error in stream_response: {e}")
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            self.logger.error(f"Error in agent_manager.py:stream_response - {error_type}: {str(e)}\n{error_traceback}")

    def cancel_interaction(self, ui_session_id: str) -> bool:
        """
        Cancel an ongoing interaction for the specified session.
        
        Args:
            ui_session_id: The session identifier
            
        Returns:
            bool: True if cancellation was triggered, False if session not found
        """
        if ui_session_id not in self._cancel_events:
            self.logger.warning(f"No cancel event found for session: {ui_session_id}")
            return False
            
        # Set the event to signal cancellation
        self.logger.info(f"Cancelling interaction for session: {ui_session_id}")
        self._cancel_events[ui_session_id].set()

        return True
        
    async def debug_session(self, ui_session_id: str):
        """
        Generate a diagnostic report for a session to help debug issues.

        Args:
            ui_session_id: The session identifier

        Returns:
            Dict containing diagnostic information about the session

        Raises:
            ValueError: If the session ID is invalid
        """
        session_data: UserSession = await self.get_user_session(ui_session_id)
        if not session_data:
            raise ValueError(f"Invalid session ID: {ui_session_id}")

        agent_bridge = session_data.agent_bridge
        if not agent_bridge:
            return {"error": "No agent bridge found in session data"}

        diagnostic = {
            "session_id": ui_session_id,
            "agent_c_session_id": ui_session_id,
            "agent_name": agent_bridge.chat_session.agent_config.name,
            "created_at": agent_bridge.chat_session.created_at,
            "backend": agent_bridge.chat_session.agent_config.agent_params.type,
            "model_name": agent_bridge.chat_session.agent_config.agent_params.model_id,
        }

        # Check session manager
        session_manager = getattr(agent_bridge, "session_manager", None)
        if session_manager:
            diagnostic["session_manager"] = {
                "exists": True,
                "user_id": getattr(session_manager, "user_id", "unknown"),
                "has_chat_session": hasattr(session_manager,
                                            "chat_session") and session_manager.chat_session is not None,
            }

            if hasattr(session_manager, "chat_session") and session_manager.chat_session:
                diagnostic["chat_session"] = {
                    "session_id": session_manager.chat_session.session_id,
                    "has_active_memory": hasattr(session_manager,
                                                 "active_memory") and session_manager.active_memory is not None,
                }

                if hasattr(session_manager, "active_memory") and session_manager.active_memory:
                    message_count = len(session_manager.active_memory.messages)
                    diagnostic["messages"] = {
                        "count": message_count,
                        "user_messages": sum(1 for m in session_manager.active_memory.messages if m.role == "user"),
                        "assistant_messages": sum(
                            1 for m in session_manager.active_memory.messages if m.role == "assistant"),
                        "latest_message": str(session_manager.active_memory.messages[-1].content)[:100] + "..."
                        if message_count > 0 else "none"
                    }

                    # Sample of recent messages (last 3)
                    recent_messages = []
                    for msg in list(session_manager.active_memory.messages)[-3:]:
                        recent_messages.append({
                            "role": msg.role,
                            "content_preview": str(msg.content)[:50] + "..." if len(str(msg.content)) > 50 else str(
                                msg.content),
                            "timestamp": str(getattr(msg, "timestamp", "unknown"))
                        })
                    diagnostic["recent_messages"] = recent_messages
        else:
            diagnostic["session_manager"] = {"exists": False}

        # Check current_chat_Log
        current_chat_log = getattr(agent_bridge, "current_chat_Log", None)
        diagnostic["current_chat_Log"] = {
            "exists": current_chat_log is not None,
            "count": len(current_chat_log) if current_chat_log else 0
        }

        # Check tool chest
        tool_chest = getattr(agent_bridge, "tool_chest", None)
        if tool_chest:
            diagnostic["tool_chest"] = {
                "exists": True,
                "active_tools": list(tool_chest.active_tools.keys()) if hasattr(tool_chest, "active_tools") else []
            }
        else:
            diagnostic["tool_chest"] = {"exists": False}

        return diagnostic
