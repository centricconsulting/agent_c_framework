import time

from pydantic import Field, model_validator, computed_field
from typing import Optional, List, Any, Union, Literal, Type, TYPE_CHECKING, Dict, Callable

from pydantic.main import IncEx

from agent_c.models import BaseDynamicContext
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.completion import CompletionParams
from agent_c.models.context.context_bag import ContextBag
from agent_c.toolsets import Toolset
from agent_c.util.observable.list import ObservableList

if TYPE_CHECKING:
    from agent_c.util.registries.toolset_registry import ToolsetRegistry as ToolsetRegistryType
    from agent_c.config.model_config_loader import ModelConfigurationLoader
    from agent_c.config.agent_config_loader import AgentConfigLoader


class BaseAgentConfiguration(AsyncObservableModel):
    """Base configuration with common fields across all agent configuration versions"""
    name: str = Field(...,
                      description="Name of the agent")
    key: str = Field(...,
                     description="Key for the agent configuration, used for identification")
    agent_description: str = Field(None,
                                   description="A description of the agent's purpose and capabilities")
    tools: ObservableList = Field(default_factory=ObservableList,
                                  description="List of enabled toolset names the agent can use")
    runtime_params: CompletionParams = Field(...,
                                             description="Parameters for the interaction with the agent")
    sections: ObservableList[str] = Field(default_factory=ObservableList,
                                          description="List of prompt sections that define the agent's behavior and capabilities")

    load_time: float = Field(default_factory=lambda: time.time(),
                             description="The time this section was loaded.  Used to determine if the section has changed on disk",
                             exclude=True)

    path_on_disk: Optional[str] = Field(None,
                                        description="The path to the section file on disk. Used to determine if the section has changed on disk.",
                                        exclude=True)
    agent_context: Optional[BaseDynamicContext] = Field(None,
                                                        description="Context for the agent itself, used to provide data for the instructions prompts ")
    context: ContextBag = Field(default_factory=ContextBag,
                                description="Context models for tools and prompts. This is used to provide data for tools and prompts.")

    state_machines: ObservableList[str] = Field(default_factory=ObservableList,
                                                description="List of state machine names that define the agent's behavior and capabilities")

    context_window_size: int = Field(0,
                                    description="The size of the context window for the agent. set automatically by the model configuration loader based on the model_id",
                                    exclude=True)

    _model_config_loader = None
    _agent_config_loader = None
    _tool_registry = None

    @computed_field
    @property
    def tool_registry(self) -> Type:
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
    def prompt_section_types(self) -> ObservableList[str]:
        return self.sections

    @computed_field
    @property
    def context_types(self) -> List[str]:
        return [self.agent_context.context_type] + [context.context_type for context in self.context.values()]

    def model_post_init(self, __context):
        """Hook up observer after model initialization"""
        super().model_post_init(__context)
        if self.agent_context is None:
            self.agent_context = BaseDynamicContext(context_type=f"{self.key}_context")


        self.runtime_params.on(self.on_runtime_params_model_id_change, "model_id")

        self.state_machines.add_observers(
            self.on_state_machine_added,
            self.on_state_machine_set,
            self.on_state_machine_removed,
            self.on_state_machine_extended
        )
        self.sections.add_observers(
            self.on_section_added,
            self.on_section_set,
            self.on_section_removed,
            self.on_section_extended
        )
        return self

    def on_state_machine_added(self, item, index: int) -> None:
        """
        Callback when a state machine is added to the agent.
        This can be used to ensure that the context bag has sections for the new state machine.
        """
        self.trigger('state_machine_added', item, index)

    def on_state_machine_removed(self, item, index: int) -> None:
        """
        Callback when a state machine is removed from the agent.
        This can be used to ensure that the context bag has sections for the removed state machine.
        """
        self.trigger('state_machine_removed', item, index)

    def on_state_machine_set(self, old, new, index: int) -> None:
        """
        Callback when a state machine is set in the agent.
        This can be used to ensure that the context bag has sections for the new state machine.
        """
        self.trigger('state_machine_set', old, new, index)

    def on_state_machine_extended(self, _the_list,  start_index: int, end_index: int) -> None:
        """
        Callback when state machines are extended in the agent.
        This can be used to ensure that the context bag has sections for the new state machines.
        """
        for index in range(start_index, end_index + 1):
            state_machine_name = self.state_machines[index]
            self.trigger('state_machine_extended', state_machine_name, index)

    def on_section_added(self, item: str, index: int) -> None:
        """
        Callback when a section is added to the agent.
        This can be used to ensure that the context bag has sections for the new section.
        """
        self.trigger('section_added', item, index)

    def on_section_removed(self, item: str, index: int) -> None:
        """
        Callback when a section is removed from the agent.
        This can be used to ensure that the context bag has sections for the removed section.
        """
        self.trigger('section_removed', item, index)

    def on_section_set(self, old: str, new: str, index: int) -> None:
        """
        Callback when a section is set in the agent.
        This can be used to ensure that the context bag has sections for the new section.
        """
        self.trigger('section_set', old, new, index)

    def on_section_extended(self, _the_list, start_index: int, end_index: int) -> None:
        """
        Callback when sections are extended in the agent.
        This can be used to ensure that the context bag has sections for the new sections.
        """
        for index in range(start_index, end_index + 1):
            section_name = self.sections[index]
            self.trigger('section_extended', section_name, index)


    @property
    def model_config_loader(self) -> 'ModelConfigurationLoader':
        """
        Returns the model configuration loader instance.
        This is used to load default parameters for the model.
        """
        if self._model_config_loader is not None:
            return self._model_config_loader
        from agent_c.config.model_config_loader import ModelConfigurationLoader
        self._model_config_loader =  ModelConfigurationLoader.instance()
        return self._model_config_loader


    @property
    def agent_config_loader(self) -> 'AgentConfigLoader':
        if self._agent_config_loader is None:
            from agent_c.config.agent_config_loader import AgentConfigLoader
            self._agent_config_loader = AgentConfigLoader.instance()

        return self._agent_config_loader

    @computed_field
    @property
    def tool_classes(self) -> List[Type[Toolset]]:
        """
        Returns a list of toolset classes that the agent can use.
        This is derived from the tools field, which contains toolset names.
        """
        from agent_c.util.registries.toolset_registry import ToolsetRegistry as reg
        return [reg.get(cls).toolset_class for cls in self.tools if reg.is_registered(cls)]

    def on_runtime_params_model_id_change(self, old_value: str, new_value: str) -> None:
        """
        Callback when the model_id in runtime_params changes.
        This can be used to update other fields or perform actions based on the change.
        """
        if 'type' not in self.runtime_params:
            model_config = self.model_config_loader.model_id_map.get(new_value)
            self.context_window_size = model_config.context_window_size
            self.runtime_params = model_config.default_completion_params
            return

        if old_value == new_value:
            return

        new_config = self.model_config_loader.model_id_map[new_value]
        self.context_window_size = new_config.context_window_size

        if self.model_config_loader.model_id_map[old_value].vendor == new_config.vendor:
            return

        self.runtime_params = new_config.default_completion_params



class AgentConfigurationV1(BaseAgentConfiguration):
    """Version 1 of the Agent Configuration"""
    version: Literal[1] = Field(1, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")

class AgentConfigurationV2(BaseAgentConfiguration):
    """Version 2 of the Agent Configuration - example with new fields"""
    version: Literal[2] = Field(2, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    category: List[str] = Field(default_factory=list, description="A list of categories this agent belongs to from most to least general" )
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")


class AgentConfigurationV3(BaseAgentConfiguration):
    """Version 2 of the Agent Configuration - example with new fields"""
    version: Literal[3] = Field(3,
                                 description="Configuration version")
    owner_id: str = Field('server',
                           description="User ID of the owner of this agent configuration. Or 'server' for server-wide agents.")
    user_interactive: bool = Field(True,
                                   description="If True, the agent is intended for interaction with users.")
    general_assistant: bool = Field(False,
                                     description="If True, the agent is is made available in the Agent Assist tool for other agents")
    oneshot: bool = Field(False,
                           description="If True, the agent is a oneshot 'agent as tool'")
    category: List[str] = Field(default_factory=list,
                                description="A list of categories this agent belongs to from most to least general" )
    context: ContextBag = Field(default_factory=ContextBag,
                                description="Context models for tools and prompts")
    agent_instructions: str = Field("",
                                           description="Primary agent instructions defining the agent's behavior")
    clone_instructions: str = Field("",
                                           description="Agent instructions defining the behavior of clones of this agent")
    compatible_model_ids: List[str] = Field(default_factory=list,
                                            description="List of compatible model IDs for this agent")
    agent_team_members: ObservableList[str] = Field(default_factory=ObservableList,
                                                    description="List of agent IDs to make available as team members for this agent. ")

    _team_members: Dict[str, 'AgentConfigurationV3'] = {}

    def __init__(self, **data: Any) -> None:
        self._tool_registry: Optional[Type['ToolsetRegistry']] = None  # Toolset registry instance
        super().__init__(**data)

    @computed_field()
    @property
    def is_tool_agent(self) -> bool:
        return self.oneshot and not self.user_interactive and not self.general_assistant

    def model_post_init(self, __context):
        """Hook up observers after model initialization"""
        super().model_post_init(__context)
        from agent_c.util.registries.toolset_registry import ToolsetRegistry
        self._tool_registry = ToolsetRegistry
        self._ensure_contexts_for_tools()
        self.tools.add_observers(self.toolset_added, self.toolset_set, self.toolset_removed, self.toolset_extended)
        return self

    @model_validator(mode='after')
    def validate_and_setup(self):
        if 'domo' in self.category:
            self.category.remove('domo')
            self.user_interactive = True
            self.mark_dirty()

        if 'agent_assist' in self.category:
            self.category.remove('agent_assist')
            self.general_assistant = True
            self.mark_dirty()

        if 'oneshot' in self.category:
            self.category.remove('oneshot')
            self.oneshot = True
            self.mark_dirty()

        if any('oneshot' in field for field in [self.key, self.name, self.agent_instructions, self.clone_instructions]):
            self.oneshot = True
            self.mark_dirty()

        return self

    def has_context(self, context_type: str) -> bool:
        """
        Check if the agent configuration has a specific context type.
        """
        return context_type in self.context or context_type == self.agent_context.context_type

    @computed_field()
    @property
    def team_members(self) -> List['AgentConfigurationV3']:
        """
        Returns a list of agent configurations that are team members of this agent.
        """
        return list(self._team_members.values())

    def model_dump(
        self,
        *,
        mode: Literal['json', 'python'] | str = 'python',
        include: IncEx | None = None,
        exclude: IncEx | None = None,
        context: Any | None = None,
        by_alias: bool | None = None,
        exclude_unset: bool = False,
        exclude_defaults: bool = False,
        exclude_none: bool = False,
        round_trip: bool = False,
        warnings: bool | Literal['none', 'warn', 'error'] = True,
        fallback: Callable[[Any], Any] | None = None,
        serialize_as_any: bool = False,
    ) -> dict[str, Any]:
        my_exclude = {'team_members', 'tool_registry', 'is_tool_agent', 'path_on_disk',
                      'model_config_loader', 'agent_config_loader', 'tool_classes', 'context_window_size',
                      'prompt_section_types', 'context_types'}
        if exclude is None:
            exclude = my_exclude
        else:
            exclude = my_exclude.union(exclude)

        return super().model_dump(
            mode=mode,
            include=include,
            exclude=exclude,
            context=context,
            by_alias=by_alias,
            exclude_unset=exclude_unset,
            exclude_defaults=exclude_defaults,
            exclude_none=exclude_none,
            round_trip=round_trip,
            warnings=warnings,
            fallback=fallback,
            serialize_as_any=serialize_as_any
        )

    def _ensure_contexts_for_tools(self) -> None:
        """
        Ensure that the context bag has sections for all tools in the agent configuration.
        This is used to ensure that the context bag has sections for all tools.
        """
        for tool_name in self.tools:
            self._add_contexts_for_tool(tool_name)

    def _add_contexts_for_tool(self, tool_name: str) -> None:
        """
        Add a context section for the given tool name.
        This is used to ensure that the context bag has sections for all tools.
        """
        cls = self._tool_registry.get(tool_name)
        if cls.has_context:
            for context_type in cls.context_types:
                if not context_type.endswith("interaction") and context_type not in self.context:
                    self.context[context_type] = self.context[context_type]
                    self.trigger('context_added', context_type)

    def _remove_contexts_for_tool(self, tool_name: str):
        cls = self._tool_registry.get(tool_name)
        for context_type in cls.context_types:
            if context_type in self.context:
                del self.context[context_type]
                self.trigger('context_removed', context_type)

    # Event handlers for toolset changes
    def toolset_added(self, item: str, index: int) -> None:
        """
        Callback when a toolset is added to the agent.
        This can be used to ensure that the context bag has sections for the new toolset.
        """
        self._add_contexts_for_tool(item)
        self.trigger('toolset_added', item, index)

    def toolset_removed(self, item: str, index: int) -> None:
        """
        Callback when a toolset is removed from the agent.
        This can be used to ensure that the context bag has sections for the removed toolset.
        """
        self._remove_contexts_for_tool(item)
        self.trigger('toolset_removed', item, index)

    def toolset_set(self, old: str, new: str, index: int) -> None:
        """
        Callback when a toolset is set in the agent.
        This can be used to ensure that the context bag has sections for the new toolset.
        """
        self._remove_contexts_for_tool(old)
        self._add_contexts_for_tool(new)
        self.trigger('toolset_set', old, new, index)

    def toolset_extended(self, _the_list, start_index, end_index) -> None:
        for index in range(start_index, end_index + 1):
            tool_name = self.tools[index]
            self._add_contexts_for_tool(tool_name)

    # Event handlers for team members changes
    def team_member_added(self, item: str, _: int) -> None:
        """
        Callback when a team member is added to the agent.
        This can be used to ensure that the context bag has sections for the new team member.
        """
        self.trigger('team_member_added', item)
        self._team_members[item] = self.agent_config_loader.catalog[item]

    def team_member_removed(self, item: str, _: int) -> None:
        self.trigger('team_member_removed', item)
        self._team_members.pop(item, None)

    def team_member_set(self, old: str, new: str, index: int) -> None:
        """
        Callback when a team member is set in the agent.
        This can be used to ensure that the context bag has sections for the new team member.
        """
        self._team_members.pop(old, None)
        self._team_members[new] = self.agent_config_loader.catalog[new]
        self.trigger('team_member_set', old, new)

    def team_member_extended(self, the_list, start_index: int, end_index: int) -> None:
        """
        Callback when team members are extended in the agent.
        This can be used to ensure that the context bag has sections for the new team members.
        """
        for index in range(start_index, end_index + 1):
            team_member_name = the_list[index]
            if team_member_name not in self._team_members:
                self._team_members[team_member_name] = self.agent_config_loader.catalog[team_member_name]

        self.trigger('team_member_extended', the_list, start_index, end_index)


# Union type for all versions
current_agent_configuration_version: int = 3
AgentConfiguration = Union[AgentConfigurationV1, AgentConfigurationV2, AgentConfigurationV3]

# Current version alias for convenience
CurrentAgentConfiguration = AgentConfigurationV3