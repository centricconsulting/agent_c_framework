import markdown
import threading

from datetime import datetime
from functools import partial
from typing import Any, Dict, List, Optional, cast, Tuple, Literal

from agent_c.prompting.prompt_section import PromptSection
from agent_c.config.model_config_loader import ModelConfigurationLoader
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.models.events import SessionEvent
from agent_c.models.events.chat import HistoryDeltaEvent, CompleteThoughtEvent, SubsessionEndedEvent, SubsessionStartedEvent
from agent_c.util.slugs import MnemonicSlugs
from agent_c.toolsets.tool_set import Toolset
from agent_c.models.agent_config import AgentConfiguration
from agent_c_tools.tools.agent_assist.expiring_session_cache import AsyncExpiringCache
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c.prompting.prompt_builder import PromptBuilder
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c.agents.gpt import BaseAgent, GPTChatAgent, AzureGPTChatAgent
from agent_c.agents.claude import ClaudeChatAgent, ClaudeBedrockChatAgent
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c_tools.tools.agent_assist.prompt import AssistantBehaviorSection


class AgentAssistToolBase(Toolset):
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

    async def _raise_event(self, event: SessionEvent, streaming_callback):
        if streaming_callback:
            event.session_id = event.user_session_id
            await streaming_callback(event)

    async def _raise_subsession_start(self, event: SubsessionStartedEvent, streaming_callback):
        await self._raise_event(event, streaming_callback)

    async def _raise_subsession_end(self, event: SubsessionEndedEvent, streaming_callback):
        await self._raise_event(event, streaming_callback)

    async def _streaming_callback_for_subagent(self, agent: AgentConfiguration, parent_streaming_callback, parent_session_id, event: SessionEvent):
        if event.type not in [ 'history_delta', 'history'] and parent_streaming_callback is not None:
            await parent_streaming_callback(event)

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

    async def __chat_params(self, agent: AgentConfiguration, agent_runtime: BaseAgent,
                            user_session_id: Optional[str] = None, parent_tool_context: Optional[Dict[str, any]] = None, **opts) -> Dict[str, Any]:
        tool_params = {}
        client_wants_cancel = opts.get('client_wants_cancel')
        parent_streaming_callback = self.streaming_callback
        agent_session_id = opts.get('agent_session_id', MnemonicSlugs.generate_id_slug(2))


        if parent_tool_context is not None:
            parent_session_id = opts.get('parent_session_id', parent_tool_context.get('session_id', None))
            client_wants_cancel = parent_tool_context.get('client_wants_cancel', None)
            parent_streaming_callback = parent_tool_context.get('streaming_callback', self.streaming_callback)
            tool_context = parent_tool_context.copy()
        else:
            parent_session_id = opts.get('parent_session_id')
            tool_context = opts.get("tool_context", {})

        tool_context['active_agent'] = agent
        tool_context['client_wants_cancel'] = client_wants_cancel
        tool_context['streaming_callback'] = parent_streaming_callback
        tool_context["session_id"] = agent_session_id
        tool_context["parent_session_id"] =  parent_session_id
        tool_context["user_session_id"] =  user_session_id

        if client_wants_cancel is None:
            client_wants_cancel = tool_context.get('client_wants_cancel', None)

        prompt_metadata = await self.__build_prompt_metadata(agent, user_session_id, **opts)


        chat_params = {"prompt_metadata": prompt_metadata, "output_format": 'raw',
                       "streaming_callback": partial(self._streaming_callback_for_subagent, agent, parent_streaming_callback, user_session_id),
                       "client_wants_cancel": client_wants_cancel, "tool_chest": self.tool_chest,
                       "prompt_builder": PromptBuilder(sections=self.sections),
                       "session_id": agent_session_id, "parent_session_id": parent_session_id,
                       "user_session_id": user_session_id, 'tool_context': tool_context
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
        agent_session_id = opts.get('agent_session_id', MnemonicSlugs.generate_id_slug(2))
        parent_session_id = opts.get('parent_session_id', None)
        return {"session_id": agent_session_id, "persona_prompt": agent_config.persona,
                "agent_config": agent_config, "user_session_id": user_session_id, "parent_session_id": parent_session_id,
                "timestamp": datetime.now().isoformat()} | agent_props | opts

    async def agent_oneshot(self, user_message: str, agent: AgentConfiguration, user_session_id: Optional[str] = None,
                            parent_tool_context: Optional[Dict[str, Any]] = None,
                            prime_agent_key: Optional[str] = None,
                            parent_session_id: Optional[str] = None,
                            sub_agent_type: Literal["clone", "team", "assist", "tool"] = "assist",
                            **additional_metadata) -> Optional[List[Dict[str, Any]]]:

        self.logger.info(f"Running one-shot with persona: {agent.key}, user session: {user_session_id}")
        agent_runtime = await self.runtime_for_agent(agent)

        agent_session_id = additional_metadata.pop('agent_session_id', f"oneshot-{MnemonicSlugs.generate_slug(2)}")

        chat_params = await self.__chat_params(agent, agent_runtime, user_session_id,
                                               parent_tool_context=parent_tool_context,
                                               agent_session_id=agent_session_id,
                                               sub_session_type="chat",
                                               sub_agent_type=sub_agent_type,
                                               prime_agent_key=prime_agent_key,
                                               parent_session_id=parent_session_id,
                                               **additional_metadata)
        try:
            await self._raise_subsession_start(SubsessionStartedEvent(session_id=agent_session_id,
                                                                      user_session_id=user_session_id,
                                                                      parent_session_id=user_session_id,
                                                                      sub_session_type="oneshot",
                                                                      sub_agent_type=sub_agent_type,
                                                                      prime_agent_key=prime_agent_key,
                                                                      sub_agent_key=agent.key,
                                                                      role="system"),
                                               chat_params['streaming_callback'])
            messages = await agent_runtime.one_shot(user_message=user_message, **chat_params)

            await self._raise_subsession_end(SubsessionEndedEvent(session_id=agent_session_id, user_session_id=user_session_id,
                                                                  parent_session_id=user_session_id, role="system"),
                                             chat_params['streaming_callback'])

            return messages
        except Exception as e:
            self.logger.exception(f"Error during one-shot with persona {agent.name}: {e}", exc_info=True)
            await self._raise_subsession_end(SubsessionEndedEvent(session_id=agent_session_id, user_session_id=user_session_id,
                                                                  parent_session_id=user_session_id, role="system"),
                                             chat_params['streaming_callback'])
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
                         prime_agent_key: Optional[str] = None,
                         parent_session_id: Optional[str] = None,
                         sub_agent_type: Literal["clone", "team", "assist", "tool"] = "assist",
                         **additional_metadata) -> Tuple[str, List[Dict[str, Any]]]:
        """
        Chat with a persona, maintaining conversation history.
        """
        self.logger.info(f"Running chat with persona: {agent.name}, user session: {user_session_id}")
        agent_runtime = await self.runtime_for_agent(agent)


        if agent_session_id is None:
            agent_session_id = MnemonicSlugs.generate_slug(2)

        session = self.session_cache.get(agent_session_id)
        if session is None:
            session = await self._new_agent_session(agent, user_session_id, agent_session_id=agent_session_id)


        chat_params = await self.__chat_params(agent, agent_runtime, user_session_id,
                                               parent_tool_context=tool_context,
                                               agent_session_id=agent_session_id,
                                               parent_session_id=parent_session_id,
                                               sub_session_type="chat",
                                               sub_agent_type=sub_agent_type,
                                               prime_agent_key=prime_agent_key,
                                               sub_agent_key=agent.key,
                                               **additional_metadata)

        chat_params['messages'] = session['messages']

        try:
            await self._raise_subsession_start(SubsessionStartedEvent(session_id=agent_session_id,
                                                                      user_session_id=user_session_id,
                                                                      parent_session_id=user_session_id,
                                                                      sub_session_type="chat",
                                                                      sub_agent_type=sub_agent_type,
                                                                      prime_agent_key=prime_agent_key,
                                                                      sub_agent_key=agent.key,
                                                                      role="system"),
                                                chat_params['streaming_callback'])

            messages = await agent_runtime.chat(user_message=user_message, **chat_params)
            if messages is not None:
                session['messages'] = messages
                await self.session_cache.set(agent_session_id, session)

        except Exception as e:
            self.logger.exception(f"Error during chat with persona {agent.name}: {e}", exc_info=True)
            await self._raise_subsession_end(SubsessionEndedEvent(session_id=agent_session_id, user_session_id=user_session_id,
                                                                  parent_session_id=user_session_id, role="system"),
                                             chat_params['streaming_callback'])

            return agent_session_id, [{'role': 'error', 'content': str(e)}]

        await self._raise_subsession_end(SubsessionEndedEvent(session_id=agent_session_id, user_session_id=user_session_id,
                                                              parent_session_id=user_session_id, role="system"),
                                         chat_params['streaming_callback'])

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

