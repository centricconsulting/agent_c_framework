from typing import Any, Optional, Dict
import re
import markdown
import yaml
import json

from agent_c.models.agent_config import CurrentAgentConfiguration
from agent_c.models.events import SystemMessageEvent
from agent_c.models.events.chat import SubsessionStartedEvent, SubsessionEndedEvent
from agent_c.toolsets import Toolset, json_schema
from agent_c.util import MnemonicSlugs
from .base import AgentAssistToolBase
from .prompt import AgentAssistSection

class AgentAssistTools(AgentAssistToolBase):
    """
    Enables your agent to collaborate with other specialized AI agents and assistants to solve complex problems.
    Your agent can delegate specific tasks to expert agents, have ongoing conversations with them, or get one-time
    assistance with particular challenges that require specialized knowledge or capabilities.
    """
    def __init__(self, **kwargs: Any):
        if not 'name' in kwargs:
            kwargs['name'] = 'aa'

        super().__init__( **kwargs)
        self.section = AgentAssistSection(tool=self)



    @staticmethod
    def fix_markdown_formatting(text: str) -> str:
        """Fix common markdown formatting issues caused by missing newlines."""

        if not isinstance(text, str):
            text = str(text)

        print("=== BEFORE FIXING ===")
        print(repr(text))
        print("=== END BEFORE ===")

        # STEP 1: Handle specific cases first (bold followed by lists)
        original_text = text
        text = re.sub(r'(\*\*[^*]+\*\*:?)\n([-*+] )', r'\1\n\n\2', text)
        if text != original_text:
            print("=== AFTER BOLD→BULLET FIX ===")
            print(repr(text))
            print("=== END AFTER ===")

        text = re.sub(r'(\*\*[^*]+\*\*:?)\n(\d+\. )', r'\1\n\n\2', text)

        # STEP 2: Handle general cases
        text = re.sub(r'(?<!^)(?<!\n\n)(\n[-*+] )', r'\n\1', text)
        text = re.sub(r'(?<!^)(?<!\n\n)(\n\d+\. )', r'\n\1', text)

        # STEP 3: Other markdown elements
        text = re.sub(r'(?<!^)(?<!\n\n)(\n#{1,6} )', r'\n\1', text)
        text = re.sub(r'(?<!^)(?<!\n\n)(\n```)', r'\n\1', text)
        text = re.sub(r'(?<!^)(?<!\n\n)(\n> )', r'\n\1', text)
        text = re.sub(r'(?<!^)(?<!\n\n)(\n---)', r'\n\1', text)
        text = re.sub(r'(?<!^)(?<!\n\n)(\n\*\*\*)', r'\n\1', text)

        print("=== FINAL RESULT ===")
        print(repr(text))
        print("=== END FINAL ===")

        return markdown.markdown(text, extensions=['markdown.extensions.nl2br', 'markdown.extensions.tables'])

    @json_schema(
        ('Make a request of an agent and receive a response. This is a reasoning agent with a large thinking budget. '
         'This is a "oneshot" request, meaning that the agent will not be able to remember anything from this '
         'request in the future.'),
        {
            'request': {
                'type': 'string',
                'description': 'A question, or request for the agent.',
                'required': True
            },
            'agent_key': {
                'type': 'string',
                'description': 'The ID key of the agent to make a request of.',
                'required': True
            },
            'process_context': {
                'type': 'string',
                'description': 'Optional process rules, context, or specific instructions to provide to the assistant. This will be prepended to the assistant\'s persona to guide its behavior for this oneshot.',
                'required': False
            }
        }
    )
    async def oneshot(self, **kwargs) -> str:
        request: str = ("# Agent Assist Tool Notice\nThe following oneshot request is from another agent. "
                        f"The agent is delegating a task for YOU to perform.\n\n---\n\n{kwargs.get('request')}\n")
        tool_context: Dict[str, Any] = kwargs.get('tool_context')
        process_context: Optional[str] = kwargs.get('process_context')
        calling_agent_config: CurrentAgentConfiguration = tool_context.get('agent_config', tool_context.get('active_agent'))

        user_session_id = tool_context.get('user_session_id', tool_context['session_id'])
        parent_session_id = tool_context.get('session_id')

        try:
            agent_config = self.agent_loader.catalog[kwargs.get('agent_key')]
        except FileNotFoundError:
            return f"Error: Agent {kwargs.get('agent_key')} not found in catalog."

        content = f"**Prime agent** requesting assistance:\n\n{request}"



        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='oneshot',
            content_type="text/markdown",
            content=f"**Prime** agent requesting assistance from '*{agent_config.name}*':\n\n{request})\n\n",
            tool_context=tool_context,
            streaming_callback=tool_context['streaming_callback']
        )

        agent_session_id = MnemonicSlugs.generate_slug(2)

        messages = await self.agent_oneshot(content, agent_config, user_session_id, tool_context,
                                             process_context=process_context,
                                             client_wants_cancel=tool_context.get('client_wants_cancel', None),
                                             parent_session_id=parent_session_id,
                                             agent_session_id=agent_session_id,
                                             sub_agent_type="assist", prime_agent_key=calling_agent_config.key)

        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='chat',
            content_type="text/markdown",
            content=f"Interaction complete for Agent Assist oneshot with {agent_config.name}. Control returned to requesting agent.",
            tool_context=tool_context,
            streaming_callback=tool_context['streaming_callback']
        )

        if messages is not None and len(messages) > 0:
            last_message = messages[-1]

            response = {'notice': 'This response is also displayed in the UI for the user, you do not need to relay it.',
                        'agent_message': last_message}

            return json.dumps(response, ensure_ascii=False)

        await self._raise_event(SystemMessageEvent(content=f"Agent Assist session {agent_session_id} completed with no messages returned.",
                                                   session_id=agent_session_id, parent_session_id=parent_session_id,
                                                   user_session_id=user_session_id, ), tool_context['streaming_callback'])

        return f"No messages returned from agent session {agent_session_id}.  This usually means that you overloaded the agent with too many tasks."

    @json_schema(
        'Begin or resume a chat session with an agent assistant. The return value will be the final output from the agent along with the agent session ID.',
        {
            'message': {
                'type': 'string',
                'description': 'The message .',
                'required': True
            },
            'agent_key': {
                'type': 'string',
                'description': 'The ID key of the agent to chat with.',
                'required': True
            },
            'process_context': {
                'type': 'string',
                'description': 'Optional process rules, context, or specific instructions to provide to the assistant. This will be prepended to the assistant\'s persona to guide its behavior for this interaction.',
                'required': False
            },
            'session_id': {
                'type': 'string',
                'description': 'Populate this with a an agent session ID to resume a chat session',
                'required': False
            }
        }
    )
    async def chat(self, **kwargs) -> str:
        message: str = ("# Agent Assist Tool Notice\nThe following chat message is from another agent. "
                        f"The agent is delegating to YOU for your expertise.\n\n---\n\n{kwargs.get('message')}\n")
        tool_context: Dict[str, Any] = kwargs.get('tool_context')
        agent_session_id: Optional[str] = kwargs.get('session_id', None)
        process_context: Optional[str] = kwargs.get('process_context')

        try:
            agent_config = self.agent_loader.catalog[kwargs.get('agent_key')]
        except FileNotFoundError:
            return f"Error: Agent {kwargs.get('agent_key')} not found in catalog."

        calling_agent_config: CurrentAgentConfiguration = tool_context.get('agent_config', tool_context.get('active_agent'))
        user_session_id = tool_context.get('user_session_id', tool_context['session_id'])
        parent_session_id = tool_context.get('session_id')

        content = f"**Prime agent** requesting assistance:\n\n{message}"

        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='chat',
            content_type="text/markdown",
            content=content,
            tool_context=tool_context
        )

        agent_session_id, messages = await self.agent_chat(content, agent_config,
                                                           user_session_id,
                                                           agent_session_id,
                                                           tool_context,
                                                           process_context=process_context,
                                                           client_wants_cancel=tool_context.get('client_wants_cancel', None),
                                                           parent_session_id=parent_session_id,
                                                           sub_agent_type="assist",
                                                           prime_agent_key=calling_agent_config.key
                                                           )
        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='chat',
            content_type="text/html",
            content=self.fix_markdown_formatting(f"Interaction complete for Agent Assist Session ID: {agent_session_id} with {agent_config.name}. Control returned to requesting agent."),
            tool_context=tool_context,
            streaming_callback=tool_context['streaming_callback'])

        if messages is not None and len(messages) > 0:
            last_message = messages[-1]

            response = {'notice': 'This response is also displayed in the UI for the user, you do not need to relay it.',
                        'agent_message': last_message}

            return json.dumps(response, ensure_ascii=False)

        await self._raise_event(SystemMessageEvent(content=f"Agent Assist session {agent_session_id} completed with no messages returned.",
                                                   session_id=agent_session_id, parent_session_id=parent_session_id,
                                                   user_session_id=user_session_id,), tool_context['streaming_callback'])

        return f"No messages returned from agent session {agent_session_id}.  This usually means that you overloaded the agent with too many tasks."

    @json_schema(
        'Load an agent agent as a YAML string for you to review',
        {
            'agent_id': {
                'type': 'string',
                'description': 'The ID of the agent to load.',
                'required': True
            },
        }
    )
    async def load_agent(self, **kwargs) -> str:
        try:
            return yaml.dump(self.agent_loader.catalog[kwargs.get('agent_id')].model_dump(), allow_unicode=True)
        except Exception:
            return f"Error: Agent {kwargs.get('agent_id')} not found in catalog."

# Register the toolset
Toolset.register(AgentAssistTools, required_tools=['WorkspaceTools'])