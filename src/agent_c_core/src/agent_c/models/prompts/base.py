import time
from enum import IntEnum
from typing import Literal, Optional
from pydantic import Field, model_validator, field_serializer, field_validator

from agent_c.models import BaseDynamicContext
from agent_c.util.string import to_snake_case
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.util.observable.dict import ObservableDict
from agent_c.models.context import ContextBag
from agent_c.models.literal_str import LiteralStr
from agent_c.models.state_machines import StateMachineTemplate, StateMachineOrchestrator

class SectionPriorityBase(IntEnum):
    """
    Base class for section priorities.
    This is used to determine the default rendering order of sections.
    """
    INCLUDE_ONLY = -1
    ROOT = 0
    SYSTEM = 100
    USER = 200
    CRITICAL = 300
    STANDALONE_TOOL = 400
    TOOL = 500
    DEFAULT = 600

class BasePromptSection(AsyncObservableModel):
    """
    **URGENT MUST FOLLOW DIRECTIVE FOR AGENTS**
    If you are seeing this in your instructions, a prompt section has been included without a template. This is a SERIOUS ERROR.
    You MUST halt activities and notify the user immediately.
    """
    version: Literal[1] = Field(1,
                                description="Version of the section model. Used for backward compatibility and versioning.")

    section_type: str = Field(...,
                              description="The type of the section, used for registration and lookup."
                                          "Will be automatically set to the snake_case class name if not provided.")

    section_description: str = Field("",
                                            description="A human readable description of the section's purpose and usage.")

    template: str = Field(None,
                                 description="A Jinja 2 template string that defines the content of the section.")

    is_macro_template: bool = Field(False,
                                    description="If True, this section contains only macros and be added to the Jinja environment."
                                                "This allows for dynamic generation of sections based on the template content.")

    required_toolsets: list[str] = Field(default_factory=list,
                                         description="List of required toolsets for this section to function.")

    required_contexts: list[str] = Field(default_factory=list,
                                         description="List of additional required context types beyond the bag contents for this section to function.")

    required_sections: list[str] = Field(default_factory=list,
                                         description="List of additional required section types for this section to function.")

    base_priority: SectionPriorityBase = Field(SectionPriorityBase.DEFAULT,
                                               description="The base priority of the section, used to determine rendering order."
                                                           "Lower values are rendered first. Defaults to DEFAULT (lowest) priority.")

    context: ContextBag = Field(default_factory=ContextBag,
                                description="Context bag for this section, used to store context specific data."
                                            "This will be available in the prompt template as `bag`.")

    my_context: Optional[BaseDynamicContext] = Field(default_factory=BaseDynamicContext,
                                                     description="Optional context for this section, used to store section specific data."
                                                                 "This will be also be available in the prompt template as `bag.my .")


    state_machines: ObservableDict[str, StateMachineTemplate] = Field(default_factory=ObservableDict[str, StateMachineTemplate],
                                                                      description="State machine templates for this section, keyed by name.")

    machines: StateMachineOrchestrator = Field(default_factory=StateMachineOrchestrator,
                                               description="State machine orchestrator for this section, used to manage state machines.",
                                               exclude=True)

    load_time: float = Field(default_factory=lambda: time.time(),
                             description="The time this section was loaded.  Used to determine if the section has changed on disk",
                             exclude=True)

    path_on_disk: Optional[str] = Field(None,
                                        description="The path to the section file on disk. Used to determine if the section has changed on disk.",
                                        exclude=True)

    @field_serializer('base_priority')
    def serialize_priority(self, value: SectionPriorityBase) -> str:
        """Serialize enum as its string name."""
        return value.name

    @field_validator('base_priority', mode='before')
    @classmethod
    def validate_priority(cls, v):
        """Accept both string names and integer values, convert to enum."""
        if isinstance(v, str):
            return SectionPriorityBase[v]  # Convert string name to enum
        elif isinstance(v, int):
            return SectionPriorityBase(v)  # Convert integer value to enum
        return v  # Already an enum instance

    @model_validator(mode='after')
    def post_init(self):
        """
        - Ensure section_type is set to the snake_case class name if not provided.
        - Construct the state machines for this section.
        """
        if not self.section_type:
            self.section_type = to_snake_case(self.__class__.__name__.removesuffix('Section'))

        if not self.template:
            self.template = LiteralStr( self.__class__.__doc__)

        self._build_machines()

        return self

    def _build_machines(self):
        """
        Build the state machines for this section.
        """
        for name, template in self.state_machines.items():
            self.machines.add_machine(name, template)

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        if not getattr(cls, 'auto_register', True) or cls.__name__.startswith('Base'):
            return

        from agent_c.util.registries.section_registry import SectionRegistry
        SectionRegistry.register_section_class(cls)

def create_example_section() -> BasePromptSection:
    return BasePromptSection(
        section_type="example_section",
        section_description="This is an example Prompt Section with fields defined",
        template="This is an example template for the section. These templates can include Jinja 2 syntax for dynamic content.\n\nLike this: {{ context.my.example_context_variable }}\n\nTemplates can otherwise be considered plain text in Markdown format.\n\n",
        my_context=BaseDynamicContext(context_type="example_section_context",
                                      example_context_variable="Hello World!"),
    )


if __name__ == "__main__":
    async def main():
        import yaml
        from agent_c.config.config_loader import locate_config_folder
        config_folder = locate_config_folder().removesuffix("/")
        section = create_example_section()
        yaml = yaml.dump(section.model_dump(), allow_unicode=True, sort_keys=False, default_flow_style=False)
        path = f"{config_folder}/sections/example.yaml"
        with open(path, "w", encoding="utf-8") as f:
            f.write(yaml)

        print(yaml)

    import asyncio
    asyncio.run(main())
