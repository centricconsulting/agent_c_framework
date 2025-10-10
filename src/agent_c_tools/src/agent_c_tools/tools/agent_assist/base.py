from typing import Any, Dict, List, Optional, cast, Tuple

from agent_c.agent_runtimes.runtime_registry import RuntimeRegistry
from agent_c.models.chat.chat_session import ChatSession
from agent_c.models.events import SessionEvent
from agent_c.util.slugs import MnemonicSlugs
from agent_c.toolsets.tool_set import Toolset
from agent_c.chat import DefaultChatSessionManager
from agent_c_tools.tools.agent_assist.agent_session_cache import AgentSessionCache
from agent_c.models.completion.agent_config import AgentConfiguration
from agent_c_tools.tools.agent_assist.models import AgentAssistSession
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.models.context.interaction_context import InteractionContext, InteractionInputs
from agent_c.agent_runtimes.base import AgentRuntime


class AgentAssistToolBase(Toolset):
    multi_user: bool = True
    force_prefix: bool = False

    # TODO: We need to capture events and forward them to the UI now that we have the CHatSession

    # If not set, the toolset class name will be converted to snake_case, minus "Tools" suffix and used
    def __init__(self, **kwargs: Any):
        if not 'name' in kwargs:
            kwargs['name'] = 'aa'

        super().__init__( **kwargs)
        self.session_cache: AgentSessionCache = AgentSessionCache()
        self.agent_loader = AgentConfigLoader.instance()
        self.runtime_cache: Dict[str, AgentRuntime] = {}
        self.workspace_tool: Optional[WorkspaceTools] = None


    async def post_init(self):
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))


    def runtime_for_agent(self, agent_config: AgentConfiguration, context: InteractionContext):
        model_id = agent_config.runtime_params.model_id
        if model_id in self.runtime_cache:
            return self.runtime_cache[model_id]
        else:
            self.runtime_cache[model_id] = RuntimeRegistry.instantiate_model_runtime(model_id, context)
            return self.runtime_cache[model_id]


    def create_agent_chat_session(self, agent_config: AgentConfiguration, parent_context: InteractionContext, name: str, agent_session_id: Optional[str] = None,persist: bool = False, is_clone: bool = False) -> ChatSession:
        agent_session =  ChatSession(user_session_id=parent_context.user_session_id, agent_config=agent_config, session_name=name,
                                     parent_session_id=parent_context.chat_session.session_id, user=parent_context.chat_session.user,
                                     session_id=agent_session_id or MnemonicSlugs.generate_slug(3), is_clone_session=is_clone)
        if persist:
            DefaultChatSessionManager.instance().new_session(agent_session)
            self.session_cache.set(agent_session.session_id, agent_session)
            parent_context.chat_session.context.agent_assist_tools.agent_sessions[agent_session.session_id] = AgentAssistSession(chat_session=agent_session)

        return agent_session


    def get_or_create_agent_chat_session(self, agent_config: AgentConfiguration, parent_context: InteractionContext, name: str, agent_session_id: Optional[str] = None, persist: bool = False, is_clone:bool = False) -> ChatSession:
        if agent_session_id is not None:
            session = self.session_cache.get(agent_session_id)
            if session is not None:
                return session

            session = parent_context.chat_session.context.agent_assist_tools.agent_sessions.get(agent_session_id)
            if session is not None:
                return session.chat_session

            session = DefaultChatSessionManager.instance().get_session(agent_session_id)
            if session is not None:
                parent_context.chat_session.context.agent_assist_tools.agent_sessions[session.session_id] = AgentAssistSession(chat_session=session)
                return session

        return self.create_agent_chat_session(agent_config, parent_context, name, agent_session_id, persist, is_clone)


    @staticmethod
    def create_agent_context(agent_chat_session: ChatSession, inputs: InteractionInputs,
                             parent_context: InteractionContext, runtime: AgentRuntime):

        def event_forwarder(event:SessionEvent):
            """
            Forward events from the agent runtime to the UI for display
            We capture them here so we can update the token counts and context window size
            for the agent chat session.
            """
            if event.type == "completion" and not event.running:
                size = event.input_tokens + event.output_tokens
                if size > 0:
                    agent_chat_session.context_window_used = size
                    agent_chat_session.output_token_count += event.output_tokens
                    agent_chat_session.input_token_count += event.input_tokens

            parent_context.streaming_callback(event)

        return InteractionContext(client_wants_cancel=parent_context.client_wants_cancel,
                                  tool_chest=parent_context.tool_chest,
                                  streaming_callback=event_forwarder,
                                  chat_session=agent_chat_session,
                                  user_session_id=agent_chat_session.user_session_id,
                                  inputs=inputs,
                                  agent_runtime=runtime)


    async def agent_oneshot(self, inputs: InteractionInputs, sub_agent_config: AgentConfiguration,
                            parent_context: InteractionContext, is_clone: bool = False) -> Optional[List[Dict[str, Any]]]:

        session_name: str = f"Oneshot with {sub_agent_config.name} by {parent_context.chat_session.agent_config.key} for user session {parent_context.user_session_id}"
        self.logger.info(f"Running {session_name}")
        try:
            agent_runtime = self.runtime_for_agent(sub_agent_config, parent_context)
            agent_session = self.create_agent_chat_session(sub_agent_config, parent_context, name=session_name, is_clone=is_clone)
            agent_context = self.create_agent_context(agent_session, inputs, parent_context, agent_runtime)

            messages = await agent_runtime.one_shot(agent_context)
            return messages
        except Exception as e:
            self.logger.exception(f"Error during {session_name}: {e}", exc_info=True)
            return None

    async def agent_chat(self, inputs: InteractionInputs, sub_agent_config: AgentConfiguration,
                         parent_context: InteractionContext, agent_session_id: Optional[str]=None,
                         is_clone: bool = False) -> Tuple[Optional[str], Optional[List[Dict[str, Any]]]]:
        session_name: str = f"Chat session with {sub_agent_config.name} by {parent_context.chat_session.agent_config.key} for user session {parent_context.user_session_id}"

        self.logger.info(f"Running {session_name}")
        try:
            agent_runtime = self.runtime_for_agent(sub_agent_config, parent_context)
            agent_session = self.get_or_create_agent_chat_session(sub_agent_config, parent_context, session_name, agent_session_id, True, is_clone)
            agent_session_id = agent_session.session_id
            agent_context = self.create_agent_context(agent_session, inputs, parent_context, agent_runtime)
        except Exception as e:
            self.logger.exception(f"Error setting up {session_name}: {e}", exc_info=True)
            return agent_session_id, None

        try:
            messages = await agent_runtime.one_shot(agent_context)
        except Exception as e:
            self.logger.exception(f"Error during {session_name}: {e}", exc_info=True)
            return agent_session_id, None

        if messages is None or len(messages) == 0:
            self.logger.error(f"No messages returned from {session_name}")
            return agent_session.session_id, [{'role': 'error', 'content': "No response from agent."}]

        return agent_session_id, messages

    def get_session_info(self, agent_session_id: str) -> Optional[Dict[str, Any]]:
        return self.session_cache.get(agent_session_id)

    def list_active_sessions(self, context: InteractionContext) -> List[Dict[str, Any]]:
        """List all active sessions."""
        active_ids = self.session_cache.list_active()
        sessions = []

        for agent_session_id in active_ids:
            session: ChatSession = self.session_cache.get(agent_session_id)

            if session.parent_session_id == context.chat_session.session_id and not session.is_clone_session:
                sessions.append(session)

        return sessions

    def list_clone_sessions(self, context: InteractionContext) -> List[Dict[str, Any]]:
        """List all active sessions."""
        active_ids = self.session_cache.list_active()
        sessions = []

        for agent_session_id in active_ids:
            session: ChatSession = self.session_cache.get(agent_session_id)

            if session.parent_session_id == context.chat_session.session_id and session.is_clone_session:
                sessions.append(session)

        return sessions

    def list_team_sessions(self, context: InteractionContext) -> List[Dict[str, Any]]:
        """List all active sessions."""
        active_ids = self.session_cache.list_active()
        sessions = []

        for agent_session_id in active_ids:
            session: ChatSession = self.session_cache.get(agent_session_id)

            if session.parent_session_id == context.chat_session.session_id and session.agent_config.key in context.chat_session.agent_config.team_members:
                sessions.append(session)

        return sessions

