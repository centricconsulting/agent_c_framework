from pathlib import Path
from typing import Any, Dict, Type, TypeVar, Optional
from pydantic import ValidationError

from agent_c.models.prompts.base import BasePromptSection
from agent_c.util.string import to_snake_case


SectionType = TypeVar('SectionType', bound=BasePromptSection)


class SectionRegistry:
    """Registry for models with section_type field to enable polymorphic deserialization"""
    _model_registry: Dict[str, Type[BasePromptSection]] = {}
    _sections: Dict[str, BasePromptSection] = {}

    @classmethod
    def register_section_class(cls, section_class: Type[BasePromptSection], section_type: str = None) -> Type[BasePromptSection]:
        """Register a section class with its section_type string"""
        if section_type:
            type_name = section_type
        else:
            # Try to get it from the model's field default
            if hasattr(section_class, 'model_fields') and 'section_type' in section_class.model_fields:
                field = section_class.model_fields['section_type']
                if hasattr(field, 'default') and field.default is not None:
                    type_name = field.default
                else:
                    # Fall back to computed name
                    type_name = to_snake_case(section_class.__name__.removesuffix('Section'))
            else:
                raise ValidationError(
                    f"Section class {section_class.__name__} must have 'section_type' field or be registered with explicit section_type"
                )

        cls._model_registry[type_name] = section_class
        return section_class

    @classmethod
    def register_section(cls, section_type: str, section_instance: BasePromptSection) -> None:
        """Register a section instance with its section_type string"""
        cls._sections[section_type] = section_instance.model_copy()

    @classmethod
    def register_section_dict(cls, section_type: str, section_data: Dict[str, Any]) -> BasePromptSection:
        """Register a section instance from a dictionary"""
        section_instance = cls.create(section_type=section_type, data=section_data)
        cls.register_section(section_type, section_instance)
        return section_instance

    @classmethod
    def register_with_section_type(cls, section_type: str):
        """Decorator for manual registration with explicit section_type"""

        def decorator(section_class: Type[BasePromptSection]) -> Type[BasePromptSection]:
            return cls.register_section_class(section_class, section_type)

        return decorator

    @classmethod
    def get_model_class(cls, section_type: str, allow_base: Optional[bool] = True) -> Type[BasePromptSection]:
        """Get a section class by its section_type string"""
        if section_type not in cls._model_registry:
            if allow_base:
                from agent_c.models.prompts.base import BasePromptSection
                return BasePromptSection

            raise ValueError(f"Section type '{section_type}' is not registered")

        return cls._model_registry[section_type]

    @classmethod
    def create(cls, section_type: Optional[str] = None, data: Optional[Dict[str, Any]] = None) -> BasePromptSection:
        """Create a section instance from data dictionary"""
        if section_type is None and data is not None:
            section_type = data.get('section_type')

        if section_type is None:
            raise ValueError("'section_type' is required to create a section instance")

        if data is None:
            if not cls.is_section_registered(section_type):
                raise ValueError(f"No data provided and section_type '{section_type}' is not registered")

        if section_type in cls._sections:
            section = cls._sections[section_type].model_copy(update=data)
            if section.changed_on_disk():
                from agent_c.config.prompt_section_loader import PromptSectionLoader
                return PromptSectionLoader.instance().load_section_from_file(Path(section.path_on_disk))

        if cls.is_section_model_registered(section_type):
            section_class = cls.get_model_class(section_type)
            return section_class(**data)

        if data:
            if data.get('is_tool_section', False):
                from agent_c.models.prompts.tool import BaseToolSection
                return BaseToolSection(**data)

            from agent_c.models.prompts.base import BasePromptSection
            return BasePromptSection(**data)

        raise ValueError(f"Section type '{section_type}' is not registered or does not have a model class, and no data provided to create an instance")

    @classmethod
    def is_section_model_registered(cls, section_type: str) -> bool:
        """Check if a section_type is registered"""
        return section_type in cls._model_registry


    @classmethod
    def is_section_registered(cls, section_type: str) -> bool:
        """Check if a section_type is registered"""
        return section_type in cls._model_registry or section_type in cls._sections

    @classmethod
    def list_types(cls) -> list[str]:
        """List all registered section types"""
        keys = list(set(list(cls._model_registry.keys()) + list(cls._sections.keys())))
        keys.sort()
        return keys
