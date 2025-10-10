from typing import Any, Dict, Type, Union
from pydantic import BaseModel

from agent_c.models.tools.toolset_registry_entry import ToolsetRegistryEntry
from agent_c.toolsets.tool_set import Toolset
from agent_c.util import to_snake_case


class ToolsetRegistry:
    """Registry for models with context_type field to enable polymorphic deserialization"""
    _registry: Dict[str, ToolsetRegistryEntry] = {}
    _system_instances: Dict[str, 'Toolset'] = {}
    _user_instances: Dict[str, Dict[str, 'Toolset']] = {}

    @classmethod
    def _tool_key_for(cls, toolset_class: Union[Type['Toolset'], str]) -> str:
        """Convert a toolset class to a registry key"""
        if isinstance(toolset_class, str):
            base_key = toolset_class
        else:
            base_key = toolset_class.__name__

        return to_snake_case(base_key.removesuffix('Tools'))

    @classmethod
    def register(cls, toolset_class: Type['Toolset']):
        entry = ToolsetRegistryEntry.from_toolset(toolset_class)
        cls._registry[entry.key] = entry

    @classmethod
    def get(cls, toolset_class: Union[Type['Toolset'], str]) -> ToolsetRegistryEntry:
        key = cls._tool_key_for(toolset_class)
        if key not in cls._registry:
            raise ValueError(f"Unknown tool {key}. Registered types: {list(cls._registry.keys())}")

        return cls._registry[key]

    @classmethod
    def instantiate(cls, toolset_class: Union[Type['Toolset'], str], context) -> 'Toolset':
        entry = cls.get(toolset_class)
        return entry.toolset_class(context)

    @classmethod
    def is_registered(cls, toolset_class: Union[Type['Toolset'], str]) -> bool:
        key = cls._tool_key_for(toolset_class)
        return key in cls._registry

    @classmethod
    def list_toolsets(cls) -> list[str]:
        return list(cls._registry.keys())
