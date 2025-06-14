import markdown
import threading

from datetime import datetime
from functools import partial
from typing import Any, Dict, List, Optional, cast, Tuple

from agent_c.models.context.interaction_context import InteractionContext
from agent_c.util.slugs import MnemonicSlugs
from agent_c.toolsets.tool_set import Toolset
from agent_c.models.events import SessionEvent
from agent_c.models.events.chat import HistoryDeltaEvent
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c.prompting.prompt_section import PromptSection
from agent_c.models.agent_config import AgentConfiguration
from agent_c.prompting.prompt_builder import PromptBuilder
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.config.model_config_loader import ModelConfigurationLoader
from agent_c.agents.gpt import BaseAgent, GPTChatAgent, AzureGPTChatAgent
from agent_c.agents.claude import ClaudeChatAgent, ClaudeBedrockChatAgent
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c_tools.tools.agent_assist.prompt import AssistantBehaviorSection
from agent_c_tools.tools.agent_assist.expiring_session_cache import AsyncExpiringCache

class AgentAssistToolBase(Toolset):
    # TODO: Make this a core singleton map
    __vendor_agent_map = {
        "azure_openai": AzureGPTChatAgent,
        "openai": GPTChatAgent,
        "claude": ClaudeChatAgent,
        "claude_aws": ClaudeBedrockChatAgent
    }

    def __init__(self, **kwargs: Any):
        if not 'name' in kwargs:
            kwargs['name'] = 'aa'
        super().__init__( **kwargs)
        self.agent_loader = AgentConfigLoader()
        self.model_config_loader = ModelConfigurationLoader()

        self.sections: List[PromptSection] = [ThinkSection(), AssistantBehaviorSection(),  DynamicPersonaSection()]

        self.session_cache = AsyncExpiringCache(default_ttl=kwargs.get('agent_session_ttl', 300))
        self.model_configs: Dict[str, Any] = self.model_config_loader.flattened_config()
        self.runtime_cache: Dict[str, BaseAgent] = {}
        self._model_name = kwargs.get('agent_assist_model_name', 'claude-3-7-sonnet-latest')
        self.persona_cache: Dict[str, AgentConfiguration] = {}
        self.workspace_tool: Optional[WorkspaceTools] = None


    async def _emit_content_from_agent(self, agent: AgentConfiguration, content: str, tool_context: InteractionContext, name: Optional[str] = None):
        if name is None:
            name = agent.name
        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function=name,
            content_type="text/html",
            content=markdown.markdown(content),
            tool_context=tool_context)

    async def _handle_history_delta(self, agent, event: HistoryDeltaEvent, tool_context: InteractionContext):
        content = []
        for message in event.messages:
            contents = message.get('content', [])
            for resp in contents:
                if 'text' in resp:
                    content.append(resp['text'])

        if len(content):
            await self._emit_content_from_agent(agent, "\n\n".join(content), tool_context)

    async def post_init(self):
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))

    async def runtime_for_agent(self, agent_config: AgentConfiguration):
        if agent_config.name in self.runtime_cache:
            return self.runtime_cache[agent_config.name]
        else:
            self.runtime_cache[agent_config.name] = await self._runtime_for_agent(agent_config)
            return self.runtime_cache[agent_config.name]

    async def _runtime_for_agent(self, agent_config: AgentConfiguration) -> BaseAgent:
        model_config = self.model_configs[agent_config.model_id]
        runtime_cls = self.__vendor_agent_map[model_config["vendor"]]

        auth_info = agent_config.agent_params.auth.model_dump() if agent_config.agent_params.auth is not None else  {}
        client = runtime_cls.client(**auth_info)
        if self.sections is not None:
            agent_sections = self.sections
        elif "ThinkTools" in agent_config.tools:
            agent_sections = [ThinkSection(), DynamicPersonaSection()]
        else:
            agent_sections = [DynamicPersonaSection()]

        await self.tool_chest.activate_toolset(agent_config.tools)

        return runtime_cls(model_name=model_config["id"], client=client,prompt_builder=PromptBuilder(sections=agent_sections))

    async def __chat_params(self, agent: AgentConfiguration, agent_runtime: BaseAgent, tool_context: InteractionContext, **opts) -> Dict[str, Any]:
        tool_params = {}
        client_wants_cancel = opts.get('client_wants_cancel')
        parent_streaming_callback = self.streaming_callback

        if opts:
            parent_tool_context = opts.get('parent_tool_context', None)
            if parent_tool_context is not None:
                client_wants_cancel = opts['parent_tool_context'].get('client_wants_cancel', client_wants_cancel)
                parent_streaming_callback = opts['parent_tool_context'].get('streaming_callback', self.streaming_callback)

            tool_context = opts.get("tool_call_context", {})
            tool_context['active_agent'] = agent
            tool_context['agent_config'] = agent
            opts['tool_call_context'] = tool_context


        prompt_metadata = await self.__build_prompt_metadata(agent, user_session_id, **opts)
        chat_params = {"prompt_metadata": prompt_metadata, "output_format": 'raw',
                       "streaming_callback": partial(self._streaming_callback_for_subagent, agent, parent_streaming_callback, user_session_id),
                       "client_wants_cancel": client_wants_cancel, "tool_chest": self.tool_chest,
                       "prompt_builder": PromptBuilder(sections=self.sections)
                       }

        if len(agent.tools):
            await self.tool_chest.initialize_toolsets(agent.tools)
            tool_params = self.tool_chest.get_inference_data(agent.tools, agent_runtime.tool_format)
            tool_params["toolsets"] = agent.tools

        agent_params = agent.agent_params.model_dump(exclude_none=True)
        if "model_name" not in agent_params:
            agent_params["model_name"] = agent.model_id

        return chat_params | tool_params | agent_params

    @staticmethod
    async def __build_prompt_metadata(agent_config: AgentConfiguration, user_session_id: Optional[str] = None, **opts) -> Dict[str, Any]:
        agent_props = agent_config.prompt_metadata if agent_config.prompt_metadata else {}

        return {"session_id": user_session_id, "persona_prompt": agent_config.persona,
                "agent_config": agent_config,
                "timestamp": datetime.now().isoformat()} | agent_props | opts

    async def agent_oneshot(self, user_message: str, agent: AgentConfiguration, user_session_id: Optional[str] = None,
                            parent_tool_context: Optional[Dict[str, Any]] = None,
                            prompt_builder: Optional[PromptBuilder] = None,
                            **additional_metadata) -> Optional[List[Dict[str, Any]]]:

        try:
            self.logger.info(f"Running one-shot with persona: {agent.key}, user session: {user_session_id}")
            agent_runtime = await self.runtime_for_agent(agent)

            chat_params = await self.__chat_params(agent, agent_runtime, user_session_id,
                                                   parent_tool_context=parent_tool_context,
                                                   **additional_metadata)
            messages = await agent_runtime.one_shot(user_message=user_message, **chat_params)
            return messages
        except Exception as e:
            self.logger.exception(f"Error during one-shot with persona {agent.name}: {e}", exc_info=True)
            return None

    async def parallel_agent_oneshots(self, user_messages: List[str], persona: AgentConfiguration, user_session_id: Optional[str] = None,
                                      tool_context: Optional[Dict[str, Any]] = None, **additional_metadata) -> List[str]:
        self.logger.info(f"Running parallel one-shots with persona: {persona.name}, user session: {user_session_id}")
        agent = await self.runtime_for_agent(persona)
        chat_params = await self.__chat_params(persona, agent, user_session_id, parent_tool_context=tool_context, **additional_metadata)
        #chat_params['allow_server_tools'] = True
        result: List[str] = await agent.parallel_one_shots(inputs=user_messages, **chat_params)
        return result

    async def _new_agent_session(self, agent: AgentConfiguration, user_session_id: str, agent_session_id: str) -> Dict[str, Any]:
        metadata = {'persona_name': agent.name}
        session = {'user_session_id': user_session_id, 'messages': [], 'metadata': metadata,
                   'created_at': datetime.now(), 'agent_key': agent.key, 'session_id': agent_session_id}

        await self.session_cache.set(agent_session_id, session)
        return  session

    async def agent_chat(self,
                         user_message: str,
                         agent: AgentConfiguration,
                         user_session_id: Optional[str] = None,
                         agent_session_id: Optional[str] = None,
                         tool_context: Optional[Dict[str, Any]] = None,
                         **additional_metadata) -> Tuple[str, List[Dict[str, Any]]]:
        """
        Chat with a persona, maintaining conversation history.
        """
        self.logger.info(f"Running chat with persona: {agent.name}, user session: {user_session_id}")
        agent_runtime = await self.runtime_for_agent(agent)

        if agent_session_id is None:
            agent_session_id = MnemonicSlugs.generate_id_slug(2)

        session = self.session_cache.get(agent_session_id)
        if session is None:
            session = await self._new_agent_session(agent, user_session_id, agent_session_id=agent_session_id)
        try:
            chat_params = await self.__chat_params(agent, agent_runtime, user_session_id, parent_tool_context=tool_context, agent_session_id=agent_session_id, **additional_metadata)
            chat_params['messages'] = session['messages']
            #chat_params['allow_server_tools'] = True  # Allow server tools to be used in the chat
            # Use non-streaming chat to avoid flooding the event stream
            messages = await agent_runtime.chat(user_message=user_message, **chat_params)
            if messages is not None:
                session['messages'] = messages
                await self.session_cache.set(agent_session_id, session)
        except Exception as e:
            self.logger.exception(f"Error during chat with persona {agent.name}: {e}", exc_info=True)
            return agent_session_id, [{'role': 'error', 'content': str(e)}]

        if messages is None or len(messages) == 0:
            self.logger.error(f"No messages returned from chat with persona {agent.name}.")
            return agent_session_id, [{'role': 'error', 'content': "No response from agent."}]

        return agent_session_id, messages

    def get_session_info(self, agent_session_id: str) -> Optional[Dict[str, Any]]:
        return self.session_cache.get(agent_session_id)

    def list_active_sessions(self, user_session_id: Optional[str] = None) -> List[Dict[str, Any]]:
        """List all active sessions."""
        active_ids = self.session_cache.list_active()
        sessions = []

        for agent_session_id in active_ids:
            session = self.session_cache.get(agent_session_id)
            if session:  # Just in case it expired between list and get
                if session['user_session_id'] == user_session_id or user_session_id is None:
                    sessions.append({
                        'agent_session_id': agent_session_id,
                        'user_session_id': session['user_session_id'],
                        'created_at': session.get('created_at'),
                        'last_activity': session.get('last_activity', session.get('created_at')),
                        'message_count': len(session.get('messages', [])),
                        'agent_key': session.get('agent_key'),
                        'metadata': session.get('metadata', {}),
                        'messages': session.get('messages', []),
                    })

        return sessions

