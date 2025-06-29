from typing import Any, Optional
import yaml

from agent_c.models.input.text_input  import TextInput
from agent_c.models.context.interaction_context import InteractionContext, InteractionInputs
from agent_c.toolsets import Toolset, json_schema
from .base import AgentAssistToolBase
from .prompt import AgentAssistSection

class AgentAssistTools(AgentAssistToolBase):
    def __init__(self, **kwargs: Any):
        if not 'name' in kwargs:
            kwargs['name'] = 'aa'

        super().__init__( **kwargs)
        self.section = AgentAssistSection(tool=self)


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
        context: InteractionContext = kwargs.get("context")
        # TODO: Fix this with agent config v3
        process_context: Optional[str] = kwargs.get('process_context')
        try:
            agent_config = self.agent_loader.catalog[kwargs.get('agent_key')]
        except FileNotFoundError:
            return f"Error: Agent {kwargs.get('agent_key')} not found in catalog."

        await self._render_media_markdown(context,
                                          f"**{context.chat_session.agent_config.key}** agent requesting assistance from '*{agent_config.key}*':\n\n{request}</p>",
                                          'oneshot')

        inputs: InteractionInputs = InteractionInputs(text=TextInput(content=request))

        messages = await self.agent_oneshot(inputs, agent_config, context)
        await self._render_media_markdown(context,
                                          f"Oneshot with {agent_config.key} complete. Control returned to requesting agent.",
                                          'oneshot')

        last_message = messages[-1] if messages else None

        if last_message is not None:
            yaml_response = yaml.dump(last_message, allow_unicode=True)
            agent_response = f"**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.\n\nAgent Response:\n{yaml_response}"
        else:
            agent_response = "No response received from the agent. This usually means that you overloaded the agent with too many tasks."


        return agent_response


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
        context: InteractionContext = kwargs.get("context")
        agent_session_id: Optional[str] = kwargs.get('session_id', None)

        # TODO: Fix this with agent config v3
        process_context: Optional[str] = kwargs.get('process_context')

        try:
            agent_config = self.agent_loader.catalog[kwargs.get('agent_key')]
        except FileNotFoundError:
            return f"Error: Agent {kwargs.get('agent_key')} not found in catalog."

        await self._render_media_markdown(context,
                                          f"**{context.chat_session.agent_config.key}** chatting with '*{agent_config.key}*':\n\n{message}",
                                          'chat')

        inputs: InteractionInputs = InteractionInputs(text=TextInput(content=message))

        agent_session_id, messages = await self.agent_chat(inputs, agent_config, context, agent_session_id)

        await self._render_media_markdown(context,
                                          f"Interaction between **{context.chat_session.agent_config.key}** and *{agent_config.key}* complete. Agent session ID: {agent_session_id}.\n\nControl returned to requesting agent.",
                                          'chat')
        if messages is not None and len(messages) > 0:
            last_message = messages[-1]
            agent_response = yaml.dump(last_message, allow_unicode=True)

            return f"**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.\n\nAgent Session ID: {agent_session_id}\n{agent_response}"

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