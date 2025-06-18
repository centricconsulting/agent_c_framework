import yaml
import markdown

from typing import Any, Optional, Dict

from agent_c.toolsets import json_schema
from agent_c.util.slugs import MnemonicSlugs
from agent_c.toolsets.tool_set import Toolset
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c_tools.tools.agent_clone.prompt import AgentCloneSection, CloneBehaviorSection
from agent_c.models.config.agent_config import CurrentAgentConfiguration, AgentConfiguration
from agent_c_tools.tools.agent_assist.base import AgentAssistToolBase
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection



class AgentCloneTools(AgentAssistToolBase):
    def __init__(self, **kwargs):
        if not 'name' in kwargs:
            kwargs['name'] = 'act'

        super().__init__(**kwargs)
        self.section = AgentCloneSection(tool=self)
        self.sections = [CloneBehaviorSection(), ThinkSection(), DynamicPersonaSection()]

    @json_schema(
        ('Make a request of clone of yourself and receive a response. '
         'This is a "oneshot" request, meaning that the agent will not be able to remember anything from this '
         'request in the future.'),
        {
            'request': {
                'type': 'string',
                'description': 'A question, or request for the clone.',
                'required': True
            },
            'process_context': {
                'type': 'string',
                'description': 'Optional process rules, context, or specific instructions to provide to the clone. This will be prepended to the clone\'s persona to guide its behavior for this specific task.',
                'required': False
            }
        }
    )
    async def oneshot(self, **kwargs) -> str:
        orig_request: str = kwargs.get('request', '')
        request: str = ("# Agent Clone Tool Notice\nThe following request is from your prime agent. "
                        f"Your prime is delegating a 'oneshot' task for YOU (the clone) to perform:\n\n{orig_request}")
        process_context: Optional[str] = kwargs.get('process_context')
        tool_context: Dict[str, Any] = kwargs.get("context")
        calling_agent_config: AgentConfiguration = tool_context.get('agent_config', tool_context.get('active_agent'))
        if calling_agent_config is None:
            return f"**ERROR**: No agent configuration found in tool context. This tool requires an active agent configuration to function."
        clone_persona: str = calling_agent_config.persona

        if process_context:
            enhanced_persona = f"# Clone Process Context and Instructions\n\n{process_context.replace('$', '$$')}\n\n# Base Agent Persona\n\n{clone_persona}"
        else:
            enhanced_persona =clone_persona

        slug = MnemonicSlugs.generate_id_slug(2)
        clone_config = CurrentAgentConfiguration.model_validate(calling_agent_config.model_dump())

        clone_tools = [tool for tool in clone_config.tools if tool != 'AgentCloneTools']
        clone_config.persona = enhanced_persona
        clone_config.name = f"{calling_agent_config.name} Clone - {slug}"
        clone_config.model_id = tool_context['calling_model_name']
        clone_config.tools = clone_tools

        await self._render_media_markdown(f"**Prime** agent requesting assistance from clone:\n\n{orig_request}\n\n## Clone context:\n{process_context}", "oneshot",tool_context=tool_context)


        messages =  await self.agent_oneshot(request, clone_config, tool_context['session_id'],
                                             tool_context, client_wants_cancel=tool_context.get('client_wants_cancel', None),
                                             process_context=process_context
                                             )
        await self._render_media_markdown(f"Interaction complete for Agent Clone oneshot. Control returned to prime agent.", "oneshot", tool_context=tool_context)

        last_message = messages[-1] if messages else None
        if last_message is not None:
            content = last_message.get('content', None)

            if content is not None:
                agent_response = yaml.dump(content[-1], allow_unicode=True).replace("\\n", "\n")

                return f"**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.\n---\n\n{agent_response}"
            else:
                self.logger.warning("No content in last message from agent.")
                agent_response = yaml.dump(last_message, allow_unicode=True)
                return agent_response

        return "No response from clone. This usually means that you overloaded the clone with too many tasks."

    @json_schema(
        'Begin or resume a chat session with a clone of yourself. The return value will be the final output from the agent along with the agent session ID.',
        {
            'message': {
                'type': 'string',
                'description': 'The message .',
                'required': True
            },
            'agent_session_id': {
                'type': 'string',
                'description': 'Populate this with a an agent session ID to resume a chat session',
                'required': False
            },
            'process_context': {
                'type': 'string',
                'description': 'Optional process rules, context, or specific instructions to provide to the clone. This will be prepended to the clone\'s persona to guide its behavior for this session.',
                'required': False
            }
        }
    )
    async def chat(self, **kwargs) -> str:
        orig_message: str = kwargs.get('message', '')
        message: str =  ("# Agent Clone Tool Notice\nThe following message is from your prime agent. "
                         f"Your prime is delegating a task for YOU (the clone) to perform.\n\n---\n\n{kwargs.get('message')}")
        process_context: Optional[str] = kwargs.get('process_context')
        tool_context: Dict[str, Any] = kwargs.get("context")
        agent_session_id: Optional[str] = kwargs.get('agent_session_id', None)
        calling_agent_config: AgentConfiguration = tool_context.get('agent_config')

        clone_persona: str = calling_agent_config.persona

        if process_context:
            enhanced_persona = f"# Clone Process Context and Instructions\n\n{process_context.replace('$', '$$')}\n\n# Base Agent Persona\n\n{clone_persona}"
        else:
            enhanced_persona = clone_persona


        agent_key = f"clone_{agent_session_id}"
        clone_config = self.agent_loader.catalog.get(agent_key, None)
        if clone_config is None:
            slug = MnemonicSlugs.generate_id_slug(2)
            clone_config = CurrentAgentConfiguration.model_validate(calling_agent_config.model_dump())

            clone_tools = [tool for tool in clone_config.tools if tool != 'AgentCloneTools']
            clone_config.persona = enhanced_persona
            clone_config.name = f"{calling_agent_config.name} Clone - {slug}"
            clone_config.model_id = tool_context['calling_model_name']
            clone_config.tools = clone_tools

            self.agent_loader.catalog[agent_key] = clone_config

        await self._render_media_markdown(markdown.markdown(f"**Prime** agent requesting assistance from clone:\n\n{orig_message}\n\n## Clone context:\n{process_context}"),
                                                            "chat",
                                                            tool_context=tool_context)

        agent_session_id, messages = await self.agent_chat(message, clone_config, tool_context['session_id'],
                                                           agent_session_id, tool_context,
                                                           process_context=process_context,
                                                           client_wants_cancel=tool_context.get('client_wants_cancel', None))
        await self._render_media_markdown(markdown.markdown(f"Interaction complete for Agent Clone Session ID: {agent_session_id}. Control returned to prime agent."),
                                                            "chat",
                                                            tool_context=tool_context)
        last_message = messages[-1] if messages else None
        if last_message is not None:
            content = last_message.get('content', None)
            if content is not None:
                agent_response = yaml.dump(content[-1], allow_unicode=True).replace("\\n", "\n")
                return f"**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.\n\nAgent Session ID: {agent_session_id}\n{agent_response}"
            else:
                self.logger.warning("No content in last message from agent.")
                agent_response = yaml.dump(last_message, allow_unicode=True)
                return agent_response

        self.logger.warning("No response from agent.")
        return "No response from clone. This usually means that you overloaded the clone with too many tasks. "


Toolset.register(AgentCloneTools, required_tools=['WorkspaceTools'])