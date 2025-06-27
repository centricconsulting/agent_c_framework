from typing import Any, Dict, Type, TypeVar, List, Optional
from pydantic import BaseModel, ValidationError
from agent_c.util.string import to_snake_case


ConfigType = TypeVar('ConfigType', bound=BaseModel)


class ConfigRegistry:
    """Registry for models with config_type field to enable polymorphic deserialization"""
    _registry: Dict[str, Type[BaseModel]] = {}
    _user_registry: Dict[str, Type[BaseModel]] = {}

    @classmethod
    def register(cls, config_class: Type[BaseModel], config_type: str = None, force_user_level: bool = False) -> Type[BaseModel]:
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
                    type_name = to_snake_case(config_class.__name__.removesuffix('Config')).replace("_a_i", "_ai")
            else:
                raise ValidationError(
                    f"Config class {config_class.__name__} must have 'config_type' field or be registered with explicit config_type"
                )
        if force_user_level or cls._is_user_level(config_class):
            cls._user_registry[type_name] = config_class
        else:
            cls._registry[type_name] = config_class

        return config_class

    @classmethod
    def _select_registry(cls, user_level: bool) -> Dict[str, Type[BaseModel]]:
        """Select the appropriate registry based on user level"""
        if user_level:
            return cls._user_registry
        return cls._registry

    @classmethod
    def _is_user_level(cls, config_class: Type[BaseModel]) -> bool:
        """Check if the config class is user-level"""
        if hasattr(config_class, 'is_user_level'):
            return config_class.is_user_level()

        return getattr(config_class, 'user_level', False)

    @classmethod
    def get_config_category(cls, config_type: str, user_level: bool = False) -> str:
        """Get the category of a registered config type"""
        if config_type not in cls._registry:
            raise ValueError(f"Unknown config type: {config_type}. Registered types: {list(cls._registry.keys())}")

        return cls._select_registry(user_level)[config_type].model_fields['category'].default

    @classmethod
    def get_config_classes_in_category(cls, category: str, user_level: bool = False) -> Dict[str, Type[BaseModel]]:
        """Get all registered config models in a specific category"""

        return  { name: model for name, model in cls._select_registry(user_level).items() if model.model_fields['category'].default == category }

    @classmethod
    def get_default_configs_in_category(cls, category: str, user_level: bool = False) -> Dict[str, BaseModel]:
        """Get all registered config models in a specific category"""
        return {name: model(config_type=name) for name, model in cls._select_registry(user_level).items() if model.model_fields['category'].default == category}

    @classmethod
    def register_with_config_type(cls, config_type: str, force_user_level: bool = False) -> callable:
        """Decorator for manual registration with explicit config_type"""

        def decorator(config_class: Type[BaseModel]) -> Type[BaseModel]:
            return cls.register(config_class, config_type, force_user_level=force_user_level)

        return decorator

    @classmethod
    def get_config_class(cls, config_type: str, user_level: bool = False) -> Type[BaseModel]:
        """Get a config class by its config_type string"""
        if config_type not in cls._select_registry(user_level):
            raise ValueError(f"Unknown config type: {config_type}. Registered types: {list(cls._select_registry(user_level).keys())}")
        return cls._select_registry(user_level)[config_type]


    @classmethod
    def create_config(cls, data: Optional[Dict[str, Any]], config_type: Optional[str] = None, user_level: bool = False) -> BaseModel:
        """Create a config instance from data dictionary"""
        if config_type is None:
            raise ValueError("Config data must include 'config_type'")

        data['config_type'] = config_type
        config_class = cls.get_config_class(config_type, user_level)
        return config_class(**data)


    @classmethod
    def is_config_registered(cls, config_type: str, user_level: bool = False) -> bool:
        """Check if a config_type is registered"""
        return config_type in cls._select_registry(user_level)

    @classmethod
    def list_config_types(cls, user_level: bool = False) -> list[str]:
        """List all registered config types"""
        return list(cls._select_registry(user_level).keys())

