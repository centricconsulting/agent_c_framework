from typing import Dict, Any, List, Type, Optional

from pydantic import Field
from agent_c.models.base import BaseModel
from agent_c.util import to_snake_case


class ToolsetRegistryEntry(BaseModel):
    toolset_name: str = Field(...,
                              description="The human readable name of the toolset this tool belongs to")
    toolset_class_name: str = Field(...,
                                    description="The class name of the toolset this entry represents")
    toolset_class: Type['Toolset'] = Field(...,
                                           description="The class of the toolset this entry represents")

    key: str = Field(...,
                        description="A unique key for this toolset, typically the toolset name in snake_case format")
    prefixed: bool = Field(True,
                           description="If true, the tool name will be prefixed with the prefix which defines the toolset name in snake_case format")
    prefix: str = Field(...,
                        description="The prefix used for this toolset, typically the short name.  Used to provide grouping")
    required_tools: List[str] = Field(default_factory=list,
                                      description="List of required tools for this tool to function")
    json_schemas: List[Dict[str, Any]] = Field(default_factory=list,
                                                description="JSON schema for the tool's input parameters, if applicable")
    agent_instructions: str = Field(...,
                                    description="The help text shown when this tool is equipped by an agent")
    user_description: str = Field(...,
                                  description="A human readable description of the tool's purpose and usage")
    multi_user: bool = Field(False,
                             description="If true, this tool is designed be safely used by multiple users simultaneously")

    prompt_class: Type['PromptSection'] = Field(...,
                                                description="The class of the prompt section used for this toolset")
    context_type: Optional[str] = Field(...,
                                        description="The context type this toolset uses for user configuration. Used to look up the correct context class in the registry")
    config_type: Optional[str] = Field(...,
                                        description="The config type this toolset uses for server configuration. Used to look up the correct config class in the registry")

    @property
    def has_config(self) -> bool:
        """
        Check if this toolset has a configuration type defined.

        Returns:
            bool: True if config_type is set, False otherwise.
        """
        return self.config_type is not None

    @property
    def has_context(self) -> bool:
        """
        Check if this toolset has a context type defined.

        Returns:
            bool: True if context_type is set, False otherwise.
        """
        return self.context_type is not None


    @classmethod
    def from_toolset(cls, toolset) -> 'ToolsetRegistryEntry':
        """
        Create a ToolsetRegistryEntry from a Toolset instance.

        Args:
            toolset: The Toolset instance to convert.

        Returns:
            ToolsetRegistryEntry: The corresponding registry entry.
        """
        from agent_c.toolsets.tool_set import Toolset
        toolset_class_name: str = toolset.__name__
        key = to_snake_case(toolset_class_name.removesuffix('Tools'))
        prefixed = getattr(toolset, 'force_prefix', True)
        prefix = getattr(toolset, 'prefix', key)
        toolset_name = getattr(toolset, 'name', key.title().replace("_", " "))



        return cls(
            toolset_class_name=toolset_class_name,
            toolset_name=toolset_name,
            key=key,
            toolset_class=toolset,
            required_tools=getattr(toolset, 'required_tools', []),
            prefixed=prefixed,
            prefix=prefix,
            json_schemas=toolset.tool_schemas(),
            agent_instructions=toolset.agent_instructions,
            user_description=toolset.user_description,
            multi_user=toolset.multi_user
        )



