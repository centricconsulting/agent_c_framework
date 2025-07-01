import os
import time
from enum import IntEnum
from typing import Literal, Optional, List
from pydantic import Field, model_validator, field_serializer, field_validator

from agent_c.models import BaseDynamicContext
from agent_c.util.string import to_snake_case
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.util.observable.dict import ObservableDict
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

    is_include: bool = Field(False,
                             description="If True, this section intended to be used as an include / macro file.")
    is_tool_section: bool = Field(False,
                                  description="If True, this section is for a tool")

    load_on_start: bool = Field(False,
                                description="If True, this section will be added to Jinja environment on startup."
                                            "This is for sections and macros that should be generally available or see constant use.")

    required_contexts: List[str] = Field(default_factory=list,
                                         description="List of additional required context types beyond the bag contents for this section to function.")

    context: Optional[BaseDynamicContext] = Field(None,
                                                  description="Optional context for this section, used to store section specific data."
                                                              "This will be also be available in the prompt template as `bag.my .")


    state_machines: ObservableDict[str, StateMachineTemplate] = Field(default_factory=ObservableDict[str, StateMachineTemplate],
                                                                      description="State machine templates for this section, keyed by name.")

    load_time: float = Field(default_factory=lambda: time.time(),
                             description="The time this section was loaded.  Used to determine if the section has changed on disk",
                             exclude=True)

    path_on_disk: Optional[str] = Field(None,
                                        description="The path to the section file on disk. Used to determine if the section has changed on disk.",
                                        exclude=True)
    tool_class_name: str = Field(None,
                                 description="The class name of the tool this section is for. "
                                             "This is used to find the tool if needed")

    @model_validator(mode="before")
    @classmethod
    def validate_class_names(cls, values):
        """Ensure tool_class_name is set if is_tool_section is True."""
        if values.get("is_tool_section") and not values.get("tool_class_name"):
            values["tool_class_name"] = cls.__name__.removesuffix("Section")
        return values

    @field_serializer('tool_class_name', when_used='unless-none')
    def serialize_conditional(self, value):
        return value

    @model_validator(mode='after')
    def post_init(self):
        """
        Ensure section_type is set to the snake_case class name if not provided.
        """
        if not self.section_type:
            self.section_type = to_snake_case(self.__class__.__name__.removesuffix('Section'))

        if not self.context:
            self.context = BaseDynamicContext(context_type=f"{self.section_type}_section_context")

        return self

    def model_post_init(self, __context):
        """Hook up observer after model initialization"""
        super().model_post_init(__context)


    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        if not getattr(cls, 'auto_register', True) or cls.__name__.startswith('Base'):
            return

        from agent_c.util.registries.section_registry import SectionRegistry
        SectionRegistry.register_section_class(cls)

    def changed_on_disk(self) -> bool:
        """
        Check if the section has changed on disk since it was loaded.
        This is done by comparing the current load time with the last modified time of the file.
        """
        if self.path_on_disk is None:
            return False
        if not os.path.exists(self.path_on_disk):
            return False

        return os.path.getmtime(self.path_on_disk) > self.load_time

def create_example_section() -> BasePromptSection:
    section =  BasePromptSection(
        section_type="example_section",
        section_description="This is an example Prompt Section with fields defined",
        template="This is an example template for the section. These templates can include Jinja 2 syntax for dynamic content.\n\nLike this: {{ context.my.example_context_variable }}\n\nTemplates can otherwise be considered plain text in Markdown format.\n\n",

    )
    return section


if __name__ == "__main__":
    async def main():
        import yaml
        from agent_c.registration.configs import locate_config_folder
        config_folder = locate_config_folder().removesuffix("/")
        section = create_example_section()
        yaml = yaml.dump(section.model_dump(), allow_unicode=True, sort_keys=False, default_flow_style=False)
        path = f"{config_folder}/sections/example.yaml"
        with open(path, "w", encoding="utf-8") as f:
            f.write(yaml)

        print(yaml)

    import asyncio
    asyncio.run(main())
