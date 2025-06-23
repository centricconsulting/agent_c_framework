from typing import Any, Dict, Type, TypeVar, List
from pydantic import BaseModel, ValidationError
from agent_c.util.string import to_snake_case


ConfigType = TypeVar('ConfigType', bound=BaseModel)


class ConfigRegistry:
    """Registry for models with config_type field to enable polymorphic deserialization"""
    _registry: Dict[str, Type[BaseModel]] = {}

    @classmethod
    def register(cls, config_class: Type[BaseModel], config_type: str = None) -> Type[BaseModel]:
        """Register a config class with its config_type string"""
        if config_type:
            type_name = config_type
        else:
            # Try to get it from the model's field default
            if hasattr(config_class, 'model_fields') and 'config_type' in config_class.model_fields:
                field = config_class.model_fields['config_type']
                if hasattr(field, 'default') and field.default is not None:
                    type_name = field.default
                else:
                    # Fall back to computed name
                    type_name = to_snake_case(config_class.__name__.removesuffix('Config'))
            else:
                raise ValidationError(
                    f"Config class {config_class.__name__} must have 'config_type' field or be registered with explicit config_type"
                )

        cls._registry[type_name] = config_class
        return config_class

    @classmethod
    def get_category(cls, config_type: str) -> str:
        """Get the category of a registered config type"""
        if config_type not in cls._registry:
            raise ValueError(f"Unknown config type: {config_type}. Registered types: {list(cls._registry.keys())}")
        return cls._registry[config_type].model_fields['category'].default

    @classmethod
    def get_models_in_category(cls, category: str) -> List[Type[BaseModel]]:
        """Get all registered config models in a specific category"""
        return [model for name, model in cls._registry.items() if getattr(model, 'category', 'misc') == category]

    @classmethod
    def register_with_config_type(cls, config_type: str):
        """Decorator for manual registration with explicit config_type"""

        def decorator(config_class: Type[BaseModel]) -> Type[BaseModel]:
            return cls.register(config_class, config_type)

        return decorator

    @classmethod
    def get(cls, config_type: str) -> Type[BaseModel]:
        """Get a config class by its config_type string"""
        if config_type not in cls._registry:
            raise ValueError(f"Unknown config type: {config_type}. Registered types: {list(cls._registry.keys())}")
        return cls._registry[config_type]

    @classmethod
    def create(cls, data: Dict[str, Any]) -> BaseModel:
        """Create a config instance from data dictionary"""
        if 'config_type' not in data:
            raise ValueError("Config data must include 'config_type' field")

        config_type = data['config_type']
        config_class = cls.get(config_type)
        return config_class(**data)

    @classmethod
    def is_registered(cls, config_type: str) -> bool:
        """Check if a config_type is registered"""
        return config_type in cls._registry

    @classmethod
    def list_types(cls) -> list[str]:
        """List all registered config types"""
        return list(cls._registry.keys())
