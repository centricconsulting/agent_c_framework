import time

from pydantic import Field, model_validator
from typing import Optional, List, Any, Union, Literal, Type

from agent_c.models import BaseDynamicContext
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.completion import CompletionParams
from agent_c.models.context.context_bag import ContextBag
from agent_c.toolsets import Toolset
from agent_c.util.observable.list import ObservableList


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

    context: BaseDynamicContext = Field(None,
                                        description="Context models for tools and prompts. This is used to provide data for tools and prompts.")
    state_machines: ObservableList[str] = Field(default_factory=ObservableList,
                                                description="List of state machine names that define the agent's behavior and capabilities")
    @property
    def prompt_section_types(self) -> ObservableList[str]:
        return self.sections

    @property
    def context_types(self) -> List[str]:
        return [self.context.context_type]

    def model_post_init(self, __context):
        """Hook up observer after model initialization"""
        super().model_post_init(__context)
        if self.context is None:
            self.context = BaseDynamicContext(context_type=f"{self.key}_context")


        self.runtime_params.on(self.on_runtime_params_model_id_change, "model_id")
        self.context.add_observers(
            self.on_context_added,
            self.on_context_set,
            self.on_context_removed,
            self.on_context_extended()
        )
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

    def on_context_added(self, item, index: int) -> None:
        """
        Callback when a context is added to the agent.
        This can be used to ensure that the context bag has sections for the new context.
        """
        self.trigger('context_added', item, index)

    def on_context_removed(self, item, index: int) -> None:
        """
        Callback when a context is removed from the agent.
        This can be used to ensure that the context bag has sections for the removed context.
        """
        self.trigger('context_removed', item, index)

    def on_context_set(self, old, new, index: int) -> None:
        """
        Callback when a context is set in the agent.
        This can be used to ensure that the context bag has sections for the new context.
        """
        self.trigger('context_set', old, new, index)

    def on_context_extended(self, start_index: int, end_index: int) -> None:
        """
        Callback when contexts are extended in the agent.
        This can be used to ensure that the context bag has sections for the new contexts.
        """
        for index in range(start_index, end_index + 1):
            context_name = self.context[index]
            self.trigger('context_extended', context_name, index)

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

    def on_state_machine_extended(self, start_index: int, end_index: int) -> None:
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

    def on_section_extended(self, start_index: int, end_index: int) -> None:
        """
        Callback when sections are extended in the agent.
        This can be used to ensure that the context bag has sections for the new sections.
        """
        for index in range(start_index, end_index + 1):
            section_name = self.sections[index]
            self.trigger('section_extended', section_name, index)



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
            from agent_c.config.model_config_loader import ModelConfigurationLoader
            self.runtime_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)
            return

        if old_value == new_value:
            return

        from agent_c.config.model_config_loader import ModelConfigurationLoader
        current_model_vendor: str = ModelConfigurationLoader.instance().model_id_map[old_value].vendor
        new_value_vendor = ModelConfigurationLoader.instance().model_id_map[new_value].vendor
        if current_model_vendor == new_value_vendor:
            return

        self.runtime_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)



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
    agent_team_members: List[str] = Field(default_factory=list,
                                          description="List of agent IDs to make available as team members for this agent. ")

    def __init__(self, **data: Any) -> None:
        self._tool_registry: Optional[Type['ToolsetRegistry']] = None  # Toolset registry instance
        super().__init__(**data)

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

    def toolset_extended(self, start_index, end_index) -> None:
        for index in range(start_index, end_index + 1):
            tool_name = self.tools[index]
            self._add_contexts_for_tool(tool_name)


    @model_validator(mode='after')
    def validate_and_setup(self):
        if not self.compatible_model_ids:
            self.compatible_model_ids = [self.runtime_params.model_id]
            self.mark_dirty()

        if self.clone_instructions == self.agent_instructions:
            self.clone_instructions = ""
            self.mark_dirty()

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

        if any( 'oneshot' in field for field in [self.key, self.name, self.agent_instructions, self.clone_instructions]):
            self.oneshot = True
            self.mark_dirty()

        return self


# Union type for all versions
current_agent_configuration_version: int = 3
AgentConfiguration = Union[AgentConfigurationV1, AgentConfigurationV2, AgentConfigurationV3]

# Current version alias for convenience
CurrentAgentConfiguration = AgentConfigurationV3