import datetime
import os
import socket
from pathlib import Path
from typing import Optional, Literal, Dict

from agent_c.registration.configs import locate_config_folder
from jinja2 import Environment, FileSystemBytecodeCache, pass_context
from agent_c.prompting.section_registry_loader import RegistryLoader
from agent_c.registration.configs import get_system_config


class JinjaPromptRenderer:
    """
    A class to manage Jinja2 template rendering for prompts in the Agent C framework.
    """
    def __init__(self):
        self._create_byte_cache()
        self._config = None
        self._config_folder = None
        self._env = None

    def _grab_configs(self):
        self._config_folder = locate_config_folder()
        self._config = get_system_config()

    @property
    def environment(self):
        """ Returns the Jinja2 environment for rendering templates. """
        if self._env is None:
            self._create_environment()
        return self._env

    def _create_globals(self):

        uname = os.uname()
        return {
            'api_version': self._config.api.app.version,
            'api_name': self._config.api.app.name,
            'api_base_dir': self._config.api.app.base_dir,
            'config_folder': locate_config_folder(),
            'runtime_environment': self._config.api.app.runtime_environment,
            'server_start_time': datetime.datetime.isoformat(),
            'server_operating_system_name': uname.sysname,
            'server_operating_system_release': uname.release,
            'server_operating_system_version': uname.version,
            'server_hardware_id': uname.machine,
            'server_host_name': socket.gethostbyaddr(socket.gethostname()),

            # Prompt section rendering helpers
            "prompt_section": self.prompt_section,
            "skip_prompt_section": self.skip_prompt_section,
            "tool_section": self.tool_section,
            "skip_tool_section": self.skip_tool_section,

            # Toggle state management helpers
            "toggle": self.toggle,
            "toggle_open": self.toggle_open,
            "toggle_close": self.toggle_close,
            "toggle_switch": self.toggle_switch,

            "include_for_state": self.include_for_state,
            "include_for_toggle": self.include_for_toggle,
            "switch_on_state": self.switch_on_state,

            # Machine state checking helper
            "machine_states_is": self.machine_states_is,

            "collapsible": self.collapsible,
            "collapsible_section": self.collapsible_section,
            "collapsible_tool_section": self.collapsible_tool_section,
        }


    def _create_environment(self):
        """ Create a Jinja2 environment for rendering templates.
        This environment uses a custom loader that retrieves templates from the SectionRegistry.
        It also sets up bytecode caching to improve performance.
        """
        self._grab_configs()
        config = self._config.core.prompt_rendering
        self._env = Environment(loader=RegistryLoader(),
                                enable_async=True,
                                bytecode_cache=self._bccache,
                                cache_size=config.jinja2_cache_size,
                                autoescape=config.jinja2_auto_escape,
                                trim_blocks=config.jinja2_trim_blocks,
                                lstrip_blocks=config.jinja2_lstrip_blocks,
                                keep_trailing_newline=config.jinja2_keep_trailing_newline,
                                auto_reload=config.jinja2_auto_reload,
                                extensions=config.jinja2_extensions,
                                )
        self._env.globals = self._create_globals()
        self._env.newstyle_gettext = config.jinja2_newstyle_gettext

    def _create_byte_cache(self):
        """
        Create a bytecode cache for Jinja2 templates.
        This is used to speed up template loading by caching compiled templates.
        """
        path = Path(locate_config_folder()).joinpath("jinja_2_cache")
        if not path.exists():
            path.mkdir(parents=True, exist_ok=True)

        self._bccache = FileSystemBytecodeCache(str(path))

    @pass_context
    def prompt_section(self, ctx, name: str, override_seen: Optional[bool] = False) -> str:
        """
        Render the given template name only the first time it’s called
        during a single render pass. Subsequent calls return an empty string.
        """
        # ctx is the current Context; ctx.vars is fresh per render()
        seen = ctx.vars.setdefault("_seen_templates", set())

        if name in seen and not override_seen:
            # Already rendered this one in *this* pass
            return ""
        seen.add(name)

        # Grab and render the subtemplate with the *same* context
        tmpl = ctx.environment.get_template(name)
        return tmpl.render(**ctx.get_all())

    @pass_context
    def skip_prompt_section(self, ctx, name: str) -> str:
        """
        Skip rendering the given template name. This is useful for sections
        that should not be rendered in certain contexts.
        """
        # ctx is the current Context; ctx.vars is fresh per render()
        seen = ctx.vars.setdefault("_seen_templates", set())

        if name in seen:
            # Already rendered this one in *this* pass
            return ""
        seen.add(name)

        # Return an empty string to skip rendering
        return ""

    @pass_context
    def tool_section(self, ctx, section_name: str, **kwargs) -> str:
        """
        Render a tool section with the given name and additional context variables.
        """
        return self.prompt_section(ctx, f"tools/{section_name}", **kwargs)

    @pass_context
    def skip_tool_section(self, ctx, section_name: str) -> str:
        """
        Skip rendering a tool section with the given name.
        """
        return self.skip_prompt_section(ctx, f"tools/{section_name}")

    def get_template_render_slots(self, interaction_context: 'InteractionContext') -> dict:
        """
        Get the template render slots for the given interaction context.
        This method retrieves the render slots from the interaction context's sections.
        """
        return {section.section_type: section.render_slot for section in interaction_context.sections.values()}

    from agent_c.models.context.interaction_context import InteractionContext
    def render_agent_prompt(self, context_in: InteractionContext) -> str:
        """
        Render the agent prompt using the provided context.
        This method uses the Jinja2 environment to render the prompt template.
        """
        agent_config = context_in.chat_session.agent_config
        agent_key: str = agent_config.key
        if context_in.chat_session.is_clone_session:
            agent_key += "_clone"



        prompt_context = { 'environment': self._env,
                           'agent_key': agent_key,
                           'interaction': context_in,
                           'session': context_in.chat_session,
                           'machines': context_in.chat_session.machines,
                         }


        # Render the master prompt template
        return self._env.get_template("agent_master").render(context=prompt_context)

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
            open_for: Optional duration to keep the toggle open (in completions)

        Returns:
            Current state of the toggle machine ('open' or 'closed')
        """
        session = ctx['session']
        return session.get_toggle_state(name, default)

    @pass_context
    def machine_states_is(ctx, name: str, state_name: str) -> bool:
        """
        Jinja helper function to check if a machine is in a specific state.
        """
        session = ctx['session']
        return session.get_machine_state(name) == state_name

    @pass_context
    def toggle_open(ctx, name: str, close_after: int = 0) -> None:
        """
        Helper to explicitly open a toggle.

        Usage:
            {{ toggle_open('debug_info') }}
        """
        session = ctx['session']
        session.set_toggle_open(name, close_after)

    @pass_context
    def toggle_close(ctx, name: str) -> None:
        """
        Helper to explicitly close a toggle.

        Usage:
            {{ toggle_close('debug_info') }}
        """
        session = ctx['session']
        session.set_toggle_closed(name)

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
        session = ctx['session']
        machine_name = f"toggle_{toggle_name}_machine"

        # Build state mapping
        state_templates = {'open': open_template}
        if closed_template:
            state_templates['closed'] = closed_template

        return self.include_for_state(ctx, machine_name, state_templates)

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
        return self.include_for_state(ctx, machine_name, state_templates)

    @pass_context
    def collapsible(self, ctx, toggle_name: str, content: str, collapsed_message: str = "Additional information available") -> str:
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
            collapsed_message: Brief message to show when toggle is closed

        Returns:
            Either full content with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, 'closed')

        if current_state == 'open':
            # Show content + hide instruction
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{collapsed_message} - *Use `toggle({toggle_name})` to display.*"

    @pass_context
    def collapsible_section(self, ctx, toggle_name: str, template_name: str, collapsed_message: str = "Additional section available") -> str:
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

        Returns:
            Either rendered template with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, 'closed')

        if current_state == 'open':
            # Render template + hide instruction
            content = self.prompt_section(ctx, template_name)
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{collapsed_message} - *Use `toggle({toggle_name})` to display.*"

    @pass_context
    def collapsible_tool_section(self, ctx, toggle_name: str, tool_section_name: str, collapsed_message: str = "Additional tool information available") -> str:
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

        Returns:
            Either rendered tool section with hide instruction, or brief message with show instruction
        """
        session = ctx['session']
        current_state = session.get_toggle_state(toggle_name, 'closed')

        if current_state == 'open':
            # Render tool section + hide instruction
            content = self.tool_section(ctx, tool_section_name)
            return f"{content}\n\n*Use `toggle({toggle_name})` to hide this info and save context space.*"
        else:
            # Show brief message + show instruction
            return f"{collapsed_message} - *Use `toggle({toggle_name})` to display.*"

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
        return self.include_for_state(ctx, machine_name, state_templates)
