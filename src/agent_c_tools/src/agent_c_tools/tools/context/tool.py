from typing import List, TYPE_CHECKING
from agent_c.toolsets import json_schema, Toolset

if TYPE_CHECKING:
    from agent_c.models.context.interaction_context import InteractionContext

class ContextTools(Toolset):
    """
    This toolset provides tools for working with the runtime context of the Agent C framework.
    It allows agents to make use the various "toggles" for selective rendering of the system prompt
    as well as providing access to the current chat session and interaction context.
    """
    auto_equipable: bool = True
    toolset_name: str = 'Session Context'
    prompt_section_types: List[str] = ['context_tools']
    multi_user: bool = True
    force_prefix: bool = False
    user_visible: bool = False

    @staticmethod
    def _have_machine_in_context(context: 'InteractionContext', machine_name: str) -> bool:
        """
        Check if the machine is in the context.
        """
        return context.chat_session.machines.has_machine(machine_name)

    @json_schema(
        description='Get the states of a toggle in the current chat session. ',
        params={
            'toggle_name': {
                'type': 'string',
                'description': 'The name of the toggle to get the state for.',
                'required': True
            }
        }
    )
    def get_toggle_state(self, **kwargs) -> str:
        context: 'InteractionContext' = kwargs.get('context')
        toggle_name = kwargs.get('toggle_name')

        if not self._have_machine_in_context(context, toggle_name):
            return f"Toggle '{toggle_name}' does not exist in the current chat session."
        try:
            return context.chat_session.get_toggle_state(toggle_name, None)
        except Exception as e:
            return f"Error getting state for toggle '{toggle_name}': {str(e)}"

    @json_schema(
        description='Toggles the state of a switch in the current chat session. ',
        params={
            'toggle_name': {
                'type': 'string',
                'description': 'The name of the toggle to toggle the state of.',
                'required': True
            }
        }
    )
    def toggle(self, **kwargs) -> str:
        context: 'InteractionContext' = kwargs.get('context')
        toggle_name = kwargs.get('toggle_name')
        if not self._have_machine_in_context(context, toggle_name):
            return f"Toggle '{toggle_name}' does not exist in the current chat session."

        try:
            return context.chat_session.toggle_switch(toggle_name)
        except Exception as e:
            return f"Error toggling switch '{toggle_name}': {str(e)}"

    @json_schema(
        description='Forces a toggle switch open in the current chat session.',
        params={
            'toggle_name': {
                'type': 'string',
                'description': 'The name of the toggle to open.',
                'required': True
            }
        }
    )
    def set_toggle_open(self, **kwargs) -> str:
        context: 'InteractionContext' = kwargs.get('context')
        toggle_name = kwargs.get('toggle_name')
        if not self._have_machine_in_context(context, toggle_name):
            return f"Toggle '{toggle_name}' does not exist in the current chat session."
        try:
            context.chat_session.set_toggle_open(toggle_name)
            return f"Toggle '{toggle_name}' has been set to open."
        except Exception as e:
            return f"Error setting toggle '{toggle_name}' to open: {str(e)}"

    @json_schema(
        description='Forces a toggle switch closed in the current chat session.',
        params={
            'toggle_name': {
                'type': 'string',
                'description': 'The name of the toggle to close.',
                'required': True
            }
        }
    )
    def set_toggle_closed(self, **kwargs) -> str:
        context: 'InteractionContext' = kwargs.get('context')
        toggle_name = kwargs.get('toggle_name')
        if not self._have_machine_in_context(context, toggle_name):
            return f"Toggle '{toggle_name}' does not exist in the current chat session."

        try:
            context.chat_session.set_toggle_closed(toggle_name)
            return f"Toggle '{toggle_name}' has been set to closed."
        except Exception as e:
            return f"Error setting toggle '{toggle_name}' to closed: {str(e)}"