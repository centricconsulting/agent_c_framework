import threading

from pydantic import Field, model_validator, computed_field
from typing import List, Optional, Dict, Any, Callable, Awaitable

from agent_c.models.events import BaseEvent
from agent_c.models.context.base import BaseContext
from agent_c.models.context.context_bag import ContextBag
from agent_c.models.context.interaction_inputs import InteractionInputs
from agent_c.models.chat.chat_session import ChatSession, MnemonicSlugs
from agent_c.models.prompts.section_bag import SectionBag
from agent_c.util.registries.context_registry import ContextRegistry
from agent_c.agent_runtimes.base import AgentRuntime
from agent_c.toolsets.tool_chest import ToolChest
from agent_c.toolsets.tool_set import Toolset


class InteractionContext(BaseContext):
    """
    Represents the context of an interaction with an agent.
    This includes the agent configuration and the agent itself.

    This is available as `interaction` in the Jinja environment.
    """
    interaction_id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3),
                                description="The ID of the interaction, used to identify the interaction in the system. "
                                            "Will be generated if not provided")
    chat_session: ChatSession = Field(..., description="The chat session that this interaction is part of. "
                                                              "This is used to group interactions together.")
    inputs: InteractionInputs = Field(..., description="The inputs for the interaction. ")
    agent_runtime: Optional[AgentRuntime] = Field(None, description="The agent runtime to use for the interaction")
    tool_chest: ToolChest = Field(..., description="The tool chest to use for the interaction")
    client_wants_cancel: threading.Event = Field(default_factory=threading.Event,
                                                 description="An event that is set when the client wants to cancel the interaction. T"
                                                             "his is used to signal the agent to stop processing.")

    streaming_callback: Callable[[BaseEvent], Awaitable[None]] = Field(
        default=lambda event: None,
        description="A callback function that is called when a streaming event occurs. This is used to handle streaming events from the agent."
    )

    sections: SectionBag = Field(default_factory=SectionBag,
                                 description="A bag of sections that are used in the interaction. ")

    context: ContextBag = Field(default_factory=dict, description="A dictionary of context models to provide data for tools / prompts."
                                                                   "Used to pass additional data to tools and prompts during the interaction. "
                                                                   "Key is the context model type, value is the context model."
                                                                   "This is available as `interaction.context` in the Jinja environment.")

    external_tool_schemas: List[Dict[str, Any]] = Field(default_factory=list, description="A dictionary of tool schemas that are used in the interaction. "
                                                                                           "This is used to store the schemas of the tools that are used in the interaction.")
    user_session_id: Optional[str] = Field(None, description="The user session ID associated with the interaction. If this is a sub session "
                                                                    "This is used to identify the user session that this interaction belongs to.")

    parent_context: Optional['InteractionContext'] = Field(None, description="The parent context of this interaction. "
                                                                                    "This is used to link interactions together in a hierarchy.")

    runtime_role: Optional[str] = Field("assistant", description="The role the runtime should used for events in the interaction. ")

    def interaction_started(self):
        self.trigger("interaction_start", self)
        self.chat_session.interaction_started()

    def interaction_ended(self):
        """
        Trigger the interaction_completed event.
        This is used to signal that the interaction has been completed.
        """
        self.trigger("interaction_end", self)
        self.chat_session.interaction_ended()

    def completion_started(self):
        """
        Trigger the completion_started event.
        This is used to signal that the completion process has started.
        """
        self.trigger("completion_start", self)
        self.chat_session.completion_started()

    def completion_ended(self):
        """
        Trigger the completion_completed event.
        This is used to signal that the completion process has been completed.
        """
        self.trigger("completion_end", self)
        self.chat_session.completion_ended()

    @computed_field
    @property
    def model_id(self) -> str:
        return self.chat_session.agent_config.runtime_params.model_id

    @model_validator(mode='after')
    def post_init(self) -> 'InteractionContext':
        self._ensure_extras_for_sections()
        return self

    def _ensure_extras_for_sections(self):
        for section_type, section in self.sections.items():
            self.context.merge(section.context)

    @classmethod
    @model_validator(mode='before')
    def validate_interaction_id(cls, values):
        """
        Ensure the interaction_id is set to a unique value based on the chat session ID.
        """
        if 'interaction_id' not in values or not values['interaction_id']:
            values['interaction_id'] = f"{values['chat_session'].session_id}:{MnemonicSlugs.generate_slug(2)}"
        return values

    @classmethod
    def model_rebuild(
        cls,
        *,
        force: bool = False,
        raise_errors: bool = True,
        _parent_namespace_depth: int = 2,
        _types_namespace: dict[str, Any] | None = None,
    ) -> bool | None:
        from agent_c.agent_runtimes.base import AgentRuntime # noqa
        from agent_c.toolsets.tool_chest import ToolChest # noqa


        super().model_rebuild(
            force=force,
            raise_errors=raise_errors,
            _parent_namespace_depth=_parent_namespace_depth,
            _types_namespace=_types_namespace
        )

    @computed_field
    @property
    def internal_tool_schemas(self) -> List[Dict[str, Any]]:
        """
        Returns the tool schemas for the interaction.
        """
        schemas: List[Dict[str, Any]] = []
        for toolset in self.toolsets.values():
            schemas.extend(toolset.tool_schemas(self.agent_runtime.vendor()))

        return schemas


    @computed_field
    @property
    def tool_schemas(self) -> List[Dict[str, Any]]:
        """
        Returns the tool schemas for the interaction.
        This includes both internal and external tool schemas.
        """
        return self.internal_tool_schemas + self.external_tool_schemas

    @computed_field
    @property
    def toolsets(self) -> Dict[str, Toolset]:
        """
        Returns a dictionary of available toolsets for the interaction, by filtering the available tools in the tool chest.
        """
        available: Dict[str, Toolset] = self.tool_chest.available_tools
        if not available:
            return {}

        return {name: toolset for name, toolset in available.items() if name in self.chat_session.agent_config.tools and toolset.allow_for_context(self)}

    def add_extra_context(self, context_type: str):
        if not context_type not in self.context:
            self.context[context_type] = ContextRegistry.create({'interaction': self}, context_type, True)
