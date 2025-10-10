import datetime
from math import floor, ceil

from pydantic import Field, model_validator, computed_field
from typing import Optional, Dict, Any, List, Literal, TYPE_CHECKING

from agent_c.models import AsyncObservableModel
from agent_c.models.prompts import BasePromptSection
from agent_c.models.prompts.section_bag import SectionBag
from agent_c.models.state_machines import StateMachineTemplate, StateMachineOrchestrator
from agent_c.models.state_machines.toggle_machine import ToggleMachine
from agent_c.util.observable import ObservableDict
from agent_c.util.registries.context_registry import ContextRegistry
from agent_c.util.slugs import MnemonicSlugs
from agent_c.models.context.context_bag import ContextBag
from agent_c.models.chat.user import ChatUser

if TYPE_CHECKING:
    from agent_c.models.completion.agent_config import CurrentAgentConfiguration
    from agent_c.models.context.interaction_context import InteractionContext


class ChatSession(AsyncObservableModel):
    """
    Represents a session object with a unique identifier, metadata,
    and other attributes.
    """
    version: int = Field(1,
                         description="The version of the session model. This is used to track changes in the session model.")
    session_id: str = Field(...,
                            description="Unique identifier for the session",)
    parent_session_id: Optional[str] = Field(None,
                                             description="The ID of the parent session, if any")
    user_session_id: Optional[str] = Field(None,
                                           description="The user session ID associated with the session")
    input_token_count: int = Field(0,
                                   description="The number of input tokens in the session")
    output_token_count: int = Field(0,
                                    description="The number of output tokens in the session")
    context_window_used: int = Field(0,
                                     description="The number of tokens in the context window")
    session_name: Optional[str] = Field(None,
                                        description="The name of the session, if any")
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    deleted_at: Optional[str] = Field(None)
    user: ChatUser = Field(...,
                           description="The user associated with the session")
    metadata: Optional[Dict[str, Any]] = Field(default_factory=dict,
                                               description="Metadata associated with the session")
    messages: List[dict[str, Any]] = Field(default_factory=list,
                                           description="List of messages in the session")
    agent_config: Optional['CurrentAgentConfiguration'] = Field(None,
                                                                description="Configuration for the agent associated with the session")
    is_clone_session: bool = Field(False,
                                   description="If True, this session is with an agent clone"
                                               "This is used to indicate that the session is a copy of another session, "
                                               "and should not be modified directly.")

    is_domo_session: bool = Field(False,
                                   description="If True, this session has a 'domo' agent ")



    context: ContextBag = Field(default_factory=ContextBag,
                                description="A dictionary of context models to provide data for tools / prompts.")

    state_machines: ObservableDict[str, StateMachineTemplate] = Field(default_factory=ObservableDict[str, StateMachineTemplate],
                                                                      description="State machine templates for this section, keyed by name.")

    machines: StateMachineOrchestrator = Field(default_factory=StateMachineOrchestrator,
                                               description="State machine orchestrator for this section, used to manage state machines.",
                                               exclude=True)
    sections: SectionBag = Field(default_factory=SectionBag,
                                 description="A bag of sections for this session, used to manage prompt sections and tools.")

    current_interaction: Optional['InteractionContext'] = Field(None,
                                                                description="The current interaction context for this session. "
                                                                            "This is used to store the current interaction context for the session.")
    _tool_registry = None

    @property
    def tool_registry(self):
        """
        Returns the tool registry for this session.
        If the tool registry is not set, it initializes it with the tools from the agent configuration.

        Returns:
            ToolRegistry: The tool registry for this session.
        """
        if self._tool_registry is None:
            from agent_c.util.registries.toolset_registry import ToolsetRegistry
            self._tool_registry = ToolsetRegistry
        return self._tool_registry

    @computed_field
    @property
    def context_window_remaining(self) -> int:
        """
        Returns the remaining context window size for the session.
        This is calculated as the context window size minus the sum of input and output token counts.

        Returns:
            int: The remaining context window size.
        """
        return self.agent_config.context_window_size - self.context_window_size

    @computed_field
    @property
    def context_window_remaining_percentage(self) -> float:
        """
        Returns the remaining context window size as a percentage of the total context window size.
        rounded down to the nearest whole number.

        Returns:
            float: The remaining context window size as a percentage.
        """
        if self.agent_config.context_window_size == 0:
            return 0.0

        return floor((self.context_window_remaining / self.agent_config.context_window_size) * 100)

    @computed_field
    @property
    def context_window_used_percentage(self) -> float:
        """
        Returns the percentage of the context window that has been used.

        Returns:
            float: The percentage of the context window that has been used.
        """
        if self.agent_config.context_window_size == 0:
            return 0.0

        return ceil((self.context_window_size / self.agent_config.context_window_size) * 100)

    @computed_field
    @property
    def user_id(self) -> str:
        """
        Returns the user ID associated with this session.

        Returns:
            str: The user ID of the session's user.
        """
        return self.user.user_id

    @computed_field
    @property
    def parent_session(self) -> Optional['ChatSession']:
        """
        Returns the parent session if it exists, otherwise None.

        Returns:
            Optional[ChatSession]: The parent session or None if not set.
        """
        from agent_c.chat import DefaultChatSessionManager

        if self.parent_session_id is None:
            return None

        return DefaultChatSessionManager.instance().get_parent_session_for(self)

    @computed_field
    @property
    def is_user_session(self) -> bool:
        """
        Checks if the session is a user session.

        Returns:
            bool: True if this session is a user session, False otherwise.
        """
        return self.user_session_id == self.session_id

    @computed_field
    @property
    def is_sub_session(self) -> bool:
        """
        Checks if the session is a sub-session of another session.

        Returns:
            bool: True if this session is a sub-session, False otherwise.
        """
        return self.parent_session_id is not None

    @computed_field
    @property
    def user_session(self) -> 'ChatSession':
        """
        Returns the user session associated with this session.
        If the user session ID is not set, it defaults to the session ID.

        Returns:
            ChatSession: The user session associated with this session.
        """
        if self.user_session_id == self.session_id:
            return self
        else:
            from agent_c.chat import DefaultChatSessionManager

            if self.user_session_id is None:
                self.user_session_id = self.session_id

            return DefaultChatSessionManager.instance().get_user_session_for(self)

    @computed_field
    @property
    def agent_name(self) -> str:
        """
        Returns the name of the agent associated with this session.
        If the agent configuration is not set, it returns "No agent configured".

        Returns:
            str: The name of the agent or "No agent configured".
        """
        if self.agent_config:
            return f"{self.agent_config.name} (clone)" if self.is_clone_session else self.agent_config.name

        return "No agent configured"

    @model_validator(mode='after')
    def post_init(self) -> 'ChatSession':
        """
        Post-initialization method to set up the session.
        This method builds the state machines and initializes the user session if needed.
        """
        self._build_machines()

        if self.user_session_id is None:
            self.user_session_id = self.session_id

        if not self.user:
            raise ValueError("User must be provided for ChatSession")

        self._add_contexts_for_user()

        self._ensure_extras_for_agent_config(self.agent_config)

        from agent_c.chat import DefaultChatSessionManager
        DefaultChatSessionManager.instance().new_session(self)

        self.machines.on('machine_added', self.on_machine_added)
        return self

    @classmethod
    @model_validator(mode='before')
    def validate_session(cls, values: Dict[str, Any]) -> Dict[str, Any]:
        """
        Validates the session data before initialization.
        Ensures that the session_id is set and that the user is a ChatUser instance.

        Args:
            values (Dict[str, Any]): The values to validate.

        Returns:
            Dict[str, Any]: The validated values.
        """
        if 'session_id' not in values or not values['session_id']:
            values['session_id'] = cls.__new_session_id(**values)

        return values

    def on_machine_added(self, template: StateMachineTemplate) -> None:
        """
        Callback for when a state machine is added to the orchestrator by an external source.
        This ensures that we store it in our model collection
        """
        if template.name not in self.state_machines:
            self.state_machines[template.name] = template
            if not ContextRegistry.is_context_registered(template.name):
                ctx = ContextRegistry.create(template.context, template.name, True)
                self.context[template.name] = ctx

    def _ensure_extras_for_agent_config(self, agent_config) -> None:
        """
        Ensure that the context bag has sections for all agent configuration tools.
        This is necessary to ensure that the agent can use the tools correctly.
        """
        if agent_config:
            self._ensure_extras_for_agent_tools(agent_config)
            for section in agent_config.sections:
                if section.section_type not in self.sections:
                    self.sections[section.section_type] = section
                    self.trigger('section_added', section.section_type)

            for state_machine in agent_config.state_machines:
                if state_machine.name not in self.state_machines:
                    self.state_machines[state_machine.name] = state_machine
                    self.trigger('machine_added', state_machine)

            if agent_config.agent_context.context_type not in self.context:
                self.context[agent_config.agent_context.context_type] = agent_config.agent_context

            for context_type in agent_config.context.keys():
                if context_type not in self.context:
                    self.context[context_type] = agent_config.context[context_type]
                    self.trigger('context_added', context_type)
                    # If it wasn't already there, then it wasn't on the user when we started.
                    # So let's make sure we add it to the user as well if needed.
                    if self.user and context_type not in self.user.context:
                        self.user.context[context_type] = self.context[context_type]
                        self.user.trigger('context_added', context_type)

    def _remove_extras_for_agent_config(self, agent_config) -> None:
        """
        Remove any context sections that are no longer needed for the agent configuration.
        This is used to clean up the context bag when tools are removed from the agent.
        """
        if agent_config:
            for tool_name in agent_config.tools:
                self._remove_extras_for_tool(tool_name)

            self._remove_extras_cls(agent_config)

    def _add_contexts_for_user(self) -> None:
        """
        Add context sections for the user.
        This is used to ensure that the context bag has sections for all user-related contexts.
        """
        if self.user:
            for context in self.user.context.values():
                self.context[context.context_type] = self.user.context[context.context_type]
                self.trigger('context_added', context.context_type)

    def _agent_config_changed(self, old, new) -> None:
        for tool_name in old.tools:
            if tool_name not in new.tools:
                self._remove_extras_for_tool(tool_name)

        for section_type in old.sections.section_types:
            if section_type not in new.sections.section_types:
                section = old.sections[section_type]
                for context_type in section.required_contexts:
                    if context_type in self.context:
                        del self.context[context_type]
                        self.trigger('context_removed', context_type)

                for state_machine in section.state_machines:
                    if state_machine.name in self.state_machines:
                        del self.state_machines[state_machine.name]
                        self.trigger('machine_removed', state_machine)

                del self.sections[section_type]
                self.trigger('section_removed', section_type)

        self._ensure_extras_for_agent_config(new)

    def _add_non_interaction_context(self, context_type: str) -> None:
        """
        Add a non-interaction context type to the context bag.
        This is used to ensure that the context bag has sections for all non-interaction contexts.
        """
        if context_type not in self.context:
            self.context[context_type] = self.context[context_type]
            self.trigger('context_added', context_type)
            # If it wasn't already there, then it wasn't on the user when we started.
            # So let's make sure we add it to the user as well if needed.
            if self.user and context_type not in self.user.context:
                self.user.context[context_type] = self.context[context_type]
                self.user.trigger('context_added', context_type)

    def _add_tool_extras(self, tool_name: str) -> None:
        """
        Add a context section for the given tool name.
        This is used to ensure that the context bag has sections for all tools.
        """
        cls = self.tool_registry.get(tool_name)
        if cls.has_context:
            if cls.context_types:
                for context_type in cls.context_types:
                    if not context_type.endswith("interaction"):
                        self._add_non_interaction_context(context_type)


        if cls.has_config:
            # Make the user hase a default config added for any tool they add to an agent.
            for config_type in cls.config_types:
                if config_type not in self.user.config:
                    self.user.config.tools[config_type] = self.user.config.tools[config_type]
                    self.trigger('config_added', config_type)


        if cls.has_prompt_sections:
            for section_type in cls.prompt_section_types:
                if section_type not in self.sections:
                    section: BasePromptSection = self.sections[section_type]
                    if section.context is not None:
                        if section.context.context_type not in self.context:
                            self.context[section.context.context_type] = self.context[section.context.context_type]
                            self.trigger('context_added', section.context.context_type)
                            # If it wasn't already there, then it wasn't on the User when we started.
                            if self.user and section.context.context_type not in self.user.context:
                                self.user.context[section.context.context_type] = self.context[section.context.context_type]
                                self.user.trigger('context_added', section.context.context_type)

                    self.trigger('section_added', section_type)

        if cls.has_state_machines:
            for state_machine in cls.state_machines:
                if state_machine.name not in self.state_machines:
                    self.state_machines[state_machine.name] = state_machine
                    self.trigger('machine_added', state_machine)

    def _remove_extras_for_tool(self, tool_name: str) -> None:
        """
        Remove a context section for the given tool name.
        This is used to ensure that the context bag has sections for all tools.
        """
        cls = self._tool_registry.get(tool_name)
        self._remove_extras_cls(cls)

    def _remove_extras_cls(self, cls) -> None:
        for context_type in cls.context_types:
            if context_type in self.context:
                del self.context[context_type]
                self.trigger('context_removed', context_type)

        for state_machine in cls.state_machines:
            if state_machine.name in self.state_machines:
                del self.state_machines[state_machine.name]
                self.trigger('machine_removed', state_machine)

        for section_type in cls.prompt_section_types:
            if section_type in self.sections:
                section = self.sections[section_type]
                for context_type in section.required_contexts:
                    if context_type in self.context:
                        del self.context[context_type]
                        self.trigger('context_removed', context_type)

                for state_machine in section.state_machines:
                    if state_machine.name in self.state_machines:
                        del self.state_machines[state_machine.name]
                        self.trigger('machine_removed', state_machine)
                del self.sections[section_type]
                self.trigger('section_removed', section_type)

    def _build_machines(self):
        """
        Build the state machines for this section.
        """
        for name, template in self.state_machines.items():
            if template.machine is None:
                if not ContextRegistry.is_context_registered(name):
                    ctx = ContextRegistry.create(template.context, name, True)
                    self.context[name] = ctx

                template.machine = self.machines.add_machine(name, template)
                self.trigger('machine_added', template)

    def on_machine_removed(self, template: StateMachineTemplate) -> None:
        """
        Callback for when a state machine is removed from the state machines collection.
        """
        if template.name in self.machines.instances:
            self.machines.remove_machine(template.name)

    def _ensure_extras_for_agent_tools(self, agent_config) -> None:
        """
        Ensure that the context bag has sections for all tools.
        This is necessary to ensure that the agent can use the tools correctly.
        """
        for tool_name in agent_config.tools:
            self._add_tool_extras(tool_name)

    def toolset_added(self, item: str, index: int) -> None:
        """
        Callback when a toolset is added to the agent.
        This can be used to ensure that the context bag has sections for the new toolset.
        """
        self._add_tool_extras(item)
        self.trigger('toolset_added', item, index)

    def toolset_removed(self, item: str, index: int) -> None:
        """
        Callback when a toolset is removed from the agent.
        This can be used to ensure that the context bag has sections for the removed toolset.
        """
        self._remove_extras_for_tool(item)
        self.trigger('toolset_removed', item, index)

    def toolset_set(self, old: str, new: str, index: int) -> None:
        """
        Callback when a toolset is set in the agent.
        This can be used to ensure that the context bag has sections for the new toolset.
        """
        self._remove_extras_for_tool(old)
        self._add_tool_extras(new)
        self.trigger('toolset_set', old, new, index)

    def toolset_extended(self, start_index, end_index) -> None:
        for index in range(start_index, end_index + 1):
            self._add_tool_extras(self.agent_config.tools[index])

        self.trigger('toolset_extended', start_index, end_index)

    def model_dump_serializable(self, exclude: Optional[List[str]] = None, **kwargs) -> Dict[str, Any]:
        """
        Since we are not the source of authority on some of our fields, we will reduce them to their serializable forms.
        This converts their complex models to IDs to be looked up on hydration.
        """
        my_exclusions = ['user', 'sections']
        if exclude is None:
            exclude = my_exclusions
        else:
            for exclusion in my_exclusions:
                if exclusion not in exclude:
                    exclude.append(exclusion)

        data = super().model_dump(exclude=set(exclude), **kwargs)
        data['user'] = self.user.user_id
        data['sections'] = self.sections.section_types

        return data

    @staticmethod
    def __new_session_id(**data) -> str:
        session_id = data.get('session_id')
        if session_id:
            return session_id

        if 'parent_session_id' in data:
            session_id = f"{data['parent_session_id']}-{MnemonicSlugs.generate_slug(2)}"
        elif 'user_session_id' in data:
            session_id = f"{data['user_session_id']}-{MnemonicSlugs.generate_slug(2)}"
        else:
            user_slug = MnemonicSlugs.generate_id_slug(2, data['user'].user_id)
            session_id = f"{user_slug}-{MnemonicSlugs.generate_slug(2)}"

        return session_id

    def new_sub_session_id(self) -> str:
        """
        Generates a new session ID for a sub-session based on the parent session.
        Returns:
            str: A new session ID for the sub-session.
        """
        return f"{self.session_id}-{MnemonicSlugs.generate_slug(2)}"

    def touch(self) -> None:
        """
        Updates the updated_at timestamp to the current time.
        """
        self.updated_at = datetime.datetime.now().isoformat()

    def get_toggle_state(self, name: str, default: Optional[Literal['open', 'closed']] = 'closed') -> str:
        """
        Get the current state of a toggle machine, creating it if it doesn't exist.

        Args:
            name: Name of the toggle (will create toggle_{name}_machine)
            default: Initial state if machine doesn't exist

        Returns:
            Current state ('open' or 'closed')
        """
        machine_name = f"toggle_{name}_machine"

        # Check if machine already exists
        if self.machines.has_machine(machine_name):
            machine_instance = self.machines.get_machine(machine_name)
            return machine_instance.current_state

        if not default:
            return "No such machine, and no default state provided to create."

        # Create new toggle machine
        toggle_machine = ToggleMachine(name=name, default_state=default)

        # Add to state machines dict (for persistence)
        self.state_machines[machine_name] = toggle_machine

        # Add to orchestrator (for runtime)
        machine_instance = self.machines.add_machine(machine_name, toggle_machine)

        return machine_instance.current_state

    def get_machine_state(self, name: str) -> str:
        """
        Get the current state of a machine by its name.

        Args:
            name: Name of the machine (without toggle_ prefix and _machine suffix)

        Returns:
            Current state ('open' or 'closed') of the toggle machine
        """
        machine_name = f"toggle_{name}_machine"

        if not self.machines.has_machine(machine_name):
            raise ValueError(f"Machine '{machine_name}' does not exist.")

        machine_instance = self.machines.get_machine(machine_name)
        return machine_instance.current_state

    def set_toggle_open(self, name: str, open_for: int = 0) -> None:
        """
        Force a toggle to the 'open' state.

        Args:
            name: Name of the toggle
            open_for: Optional duration in seconds to keep the toggle open for in completions
        """
        machine_name = f"toggle_{name}_machine"

        # Ensure machine exists (create with closed state if not)
        if not self.machines.has_machine(machine_name):
            self.get_toggle_state(name, 'closed')

        machine_instance = self.machines.get_machine(machine_name)
        machine_instance.force_open()

        def close_after_countdown(_):
            nonlocal open_for
            open_for -= 1
            if open_for <= 0:
                machine_instance.force_close()
                self.remove_listener('completion_started', close_after_countdown)

        if open_for > 0:
            # Register a listener to close the toggle after the specified duration
            self.on('completion_started', close_after_countdown)

    def set_toggle_closed(self, name: str) -> None:
        """
        Force a toggle to the 'closed' state.

        Args:
            name: Name of the toggle
        """
        machine_name = f"toggle_{name}_machine"

        # Ensure machine exists (create with open state if not)
        if not self.machines.has_machine(machine_name):
            self.get_toggle_state(name, 'open')

        machine_instance = self.machines.get_machine(machine_name)
        machine_instance.force_close()

    def toggle_switch(self, name: str) -> str:
        """
        Toggle a machine's state and return the new state.

        Args:
            name: Name of the toggle

        Returns:
            New state after toggling ('open' or 'closed')
        """
        machine_name = f"toggle_{name}_machine"

        # Ensure machine exists (create with closed state if not, then toggle to open)
        if not self.machines.has_machine(machine_name):
            self.get_toggle_state(name, 'closed')

        machine_instance = self.machines.get_machine(machine_name)
        machine_instance.toggle()
        return machine_instance.current_state

    def list_toggles(self) -> List[str]:
        """
        List all toggle machine names (without the toggle_ prefix and _machine suffix).

        Returns:
            List of toggle names
        """
        toggles = []
        for machine_name in self.machines.list_machines():
            if machine_name.startswith('toggle_') and machine_name.endswith('_machine'):
                # Extract the toggle name from toggle_{name}_machine
                toggle_name = machine_name[7:-8]  # Remove 'toggle_' and '_machine'
                toggles.append(toggle_name)

        return toggles

    def get_all_toggle_states(self) -> Dict[str, str]:
        """
        Get the current state of all toggle machines.

        Returns:
            Dictionary mapping toggle names to their current states
        """
        states = {}
        for toggle_name in self.list_toggles():
            machine_name = f"toggle_{toggle_name}_machine"
            machine_instance = self.machines.get_machine(machine_name)
            if machine_instance:
                states[toggle_name] = machine_instance.current_state
        return states

    def interaction_started(self):
        self.trigger("interaction_start", self)

    def interaction_ended(self):
        """
        Trigger the interaction_completed event.
        This is used to signal that the interaction has been completed.
        """
        self.trigger("interaction_end", self)

    def completion_started(self):
        """
        Trigger the completion_started event.
        This is used to signal that the completion process has started.
        """
        self.trigger("completion_start", self)

    def completion_ended(self):
        """
        Trigger the completion_completed event.
        This is used to signal that the completion process has been completed.
        """
        self.trigger("completion_end", self)