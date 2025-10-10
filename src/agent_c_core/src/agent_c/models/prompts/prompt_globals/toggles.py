from typing import Optional, Literal, Dict

from jinja2 import pass_context
from pydantic import Field

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals


class ToggleHelperGlobals(BasePromptGlobals):
    add_as_object: bool = Field(False)

    @staticmethod
    @pass_context
    def toggle(ctx, name: str, default: Literal['open', 'closed'] = 'closed') -> str:
        """
        Jinja helper function for managing toggle state machines.

        Creates a toggle state machine on first use and returns its current state.
        Subsequent calls return the current state of the existing machine.

        Usage in templates:
            {% if toggle('debug_info', default='closed') == 'open' %}
                <debug information here>
            {% endif %}

            {% if toggle('feature_x') == 'open' %}
                <feature content>
            {% endif %}

        Args:
            ctx: Jinja context (automatically passed)
            name: Name of the toggle (will create toggle_{name}_machine)
            default: Initial state if machine doesn't exist ('open' or 'closed')

        Returns:
            Current state of the toggle machine ('open' or 'closed')
        """
        session = ctx['session']
        return session.get_toggle_state(name, default)

    @staticmethod
    @pass_context
    def machine_states_is(ctx, name: str, state_name: str) -> bool:
        """
        Jinja helper function to check if a machine is in a specific state.
        """
        session = ctx['session']
        return session.get_machine_state(name) == state_name

    @staticmethod
    @pass_context
    def toggle_open(ctx, name: str, close_after: int = 0) -> None:
        """
        Helper to explicitly open a toggle.

        Usage:
            {{ toggle_open('debug_info') }}
        """
        session = ctx['session']
        session.set_toggle_open(name, close_after)

    @staticmethod
    @pass_context
    def toggle_close(ctx, name: str) -> None:
        """
        Helper to explicitly close a toggle.

        Usage:
            {{ toggle_close('debug_info') }}
        """
        session = ctx['session']
        session.set_toggle_closed(name)

    @staticmethod
    @pass_context
    def toggle_switch(ctx, name: str) -> str:
        """
        Helper to toggle the state and return the new state.

        Usage:
            {{ toggle_switch('debug_info') }}

        Returns:
            New state after toggling ('open' or 'closed')
        """
        session = ctx['session']
        return session.toggle_switch(name)


    @pass_context
    def include_for_state(self, ctx, machine_name: str, state_templates: Dict[str, str], fallback: Optional[str] = None) -> str:
        """
        Include a template based on the current state of a machine.

        This helper looks up the current state of the specified machine and includes
        the corresponding template from the state_templates mapping.

        Usage in templates:
            {{ include_for_state('user_auth_machine', {
                'logged_in': 'prompts/authenticated_user',
                'guest': 'prompts/guest_user',
                'admin': 'prompts/admin_user',
                'default': 'prompts/fallback_user'
            }) }}

            {{ include_for_state('debug_level_machine', {
                'verbose': 'debug/verbose_context',
                'normal': 'debug/standard_context',
                'minimal': 'debug/minimal_context'
            }, fallback='debug/no_debug') }}

        Args:
            ctx: Jinja context (automatically passed)
            machine_name: Name of the state machine to check
            state_templates: Dictionary mapping state names to template names
            fallback: Optional fallback template if state not found in mapping

        Returns:
            Rendered template content for the current state, or empty string if no match
        """
        session = ctx['session']

        # Get the current state of the machine
        if not session.machines.has_machine(machine_name):
            # Machine doesn't exist
            if fallback:
                return self.prompt_section(ctx, fallback)
            elif 'default' in state_templates:
                return self.prompt_section(ctx, state_templates['default'])
            return ""

        machine_instance = session.machines.get_machine(machine_name)
        current_state = machine_instance.current_state

        # Look up template for current state
        template_name = None

        # First try exact state match
        if current_state in state_templates:
            template_name = state_templates[current_state]
        # Then try fallback parameter
        elif fallback:
            template_name = fallback
        # Finally try 'default' key in mapping
        elif 'default' in state_templates:
            template_name = state_templates['default']

        # Render the template if we found one
        if template_name:
            return self.prompt_section(ctx, template_name)

        return ""

    @pass_context
    def include_for_toggle(self, ctx, toggle_name: str, open_template: str, closed_template: Optional[str] = None) -> str:
        """
        Convenience helper for including templates based on toggle state.

        Usage in templates:
            {{ include_for_toggle('debug_info', 'debug/verbose_context') }}

            {{ include_for_toggle('feature_x',
                                 open_template='features/feature_x_enabled',
                                 closed_template='features/feature_x_disabled') }}

        Args:
            ctx: Jinja context (automatically passed)
            toggle_name: Name of the toggle (without toggle_ prefix or _machine suffix)
            open_template: Template to include when toggle is 'open'
            closed_template: Optional template to include when toggle is 'closed'

        Returns:
            Rendered template content based on toggle state
        """
        machine_name = f"toggle_{toggle_name}_machine"

        # Build state mapping
        state_templates = {'open': open_template}
        if closed_template:
            state_templates['closed'] = closed_template

        return self.include_for_state(machine_name, state_templates)


    @pass_context
    def switch_on_state(self, ctx, machine_name: str, **state_templates) -> str:
        """
        Alternative syntax for state-based template inclusion using keyword arguments.

        Usage in templates:
            {{ switch_on_state('user_role_machine',
                              admin='prompts/admin_context',
                              user='prompts/user_context',
                              guest='prompts/guest_context',
                              default='prompts/fallback') }}

        Args:
            ctx: Jinja context (automatically passed)
            machine_name: Name of the state machine to check
            **state_templates: Keyword arguments mapping state names to template names

        Returns:
            Rendered template content for the current state
        """
        return self.include_for_state(machine_name, state_templates)


    @pass_context
    def collapsible(self, ctx, toggle_name: str, content: str, message: str = "Additional information available", start_state: str = "closed") -> str:
        """
        Create collapsible content that shows/hides based on a toggle with user instructions.

        When toggle is open: Shows full content + instruction to hide
        When toggle is closed: Shows brief message + instruction to show

        Usage in templates:
            {{ collapsible('debug_info',
                          '<detailed debug context here>',
                          'Debug information available') }}

            {{ collapsible('verbose_mode',
                          prompt_section('debug/verbose_context'),
                          'Verbose logging details available') }}

        Args:
            ctx: Jinja context (automatically passed)
            toggle_name: Name of the toggle to control visibility
            content: Full content to show when toggle is open
            message: Brief message to show when toggle is closed
            start_state: Initial state of the toggle ('open' or 'closed')

        Returns:
            Either full content with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, start_state)

        if current_state == 'open':
            # Show content + hide instruction
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{message} - *Use `toggle({toggle_name})` to display.*"


    @pass_context
    def collapsible_section(self, ctx, toggle_name: str, template_name: str, collapsed_message: str = "Additional section available", start_state: str = "open") -> str:
        """
        Create collapsible template section that shows/hides based on a toggle.

        When toggle is open: Renders template + instruction to hide
        When toggle is closed: Shows brief message + instruction to show

        Usage in templates:
            {{ collapsible_section('debug_info',
                                   'debug/verbose_context',
                                   'Debug information available') }}

        Args:
            ctx: Jinja context (automatically passed)
            toggle_name: Name of the toggle to control visibility
            template_name: Template to render when toggle is open
            collapsed_message: Brief message to show when toggle is closed
            start_state: Initial state of the toggle ('open' or 'closed')

        Returns:
            Either rendered template with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, start_state)

        if current_state == 'open':
            # Render template + hide instruction
            content = self.prompt_section(ctx, template_name)
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{collapsed_message} - *Use `toggle({toggle_name})` to display.*"

    @pass_context
    def expandable_section(self, ctx, toggle_name: str, template_name: str, collapsed_message: str = "Additional section available", start_state: str = "closed") -> str:
        return self.collapsible_section(ctx, toggle_name, template_name, collapsed_message, start_state)

    @pass_context
    def collapsible_tool_section(self, ctx, toggle_name: str, tool_section_name: str, collapsed_message: str = "Additional tool information available", start_state: str = "open") -> str:
        """
        Create collapsible tool section that shows/hides based on a toggle.

        Usage in templates:
            {{ collapsible_tool_section('advanced_tools',
                                       'advanced_search',
                                       'Advanced search tools available') }}

        Args:
            ctx: Jinja context (automatically passed)
            toggle_name: Name of the toggle to control visibility
            tool_section_name: Tool section name (tools/{name})
            collapsed_message: Brief message to show when toggle is closed
            start_state: Initial state of the toggle ('open' or 'closed')

        Returns:
            Either rendered tool section with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, start_state)

        if current_state == 'open':
            # Render tool section + hide instruction
            content = self.tool_section(ctx, tool_section_name)
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{collapsed_message} - *Use `toggle({toggle_name})` to display.*"

    @pass_context
    def expandable_tool_section(self, ctx, toggle_name: str, template_name: str, collapsed_message: str = "Additional section available", start_state: str = "closed") -> str:
        return self.collapsible_tool_section(ctx, toggle_name, template_name, collapsed_message, start_state)

    @pass_context
    def switch_on_state(self, ctx, machine_name: str, **state_templates) -> str:
        """
        Alternative syntax for state-based template inclusion using keyword arguments.

        Usage in templates:
            {{ switch_on_state('user_role_machine',
                              admin='prompts/admin_context',
                              user='prompts/user_context',
                              guest='prompts/guest_context',
                              default='prompts/fallback') }}

        Args:
            ctx: Jinja context (automatically passed)
            machine_name: Name of the state machine to check
            **state_templates: Keyword arguments mapping state names to template names

        Returns:
            Rendered template content for the current state
        """
        return self.include_for_state(machine_name, state_templates)
