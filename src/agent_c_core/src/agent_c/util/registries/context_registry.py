from typing import Any, Dict, Type, TypeVar, Optional
from pydantic import BaseModel, ValidationError

from agent_c.util.string import to_snake_case


ContextType = TypeVar('ContextType', bound=BaseModel)


class ContextRegistry:
    """Registry for models with context_type field to enable polymorphic deserialization"""
    _registry: Dict[str, Type[BaseModel]] = {}

    @classmethod
    def register(cls, context_class: Type[BaseModel], context_type: str = None) -> Type[BaseModel]:
        """Register a context class with its context_type string"""
        if context_type:
            type_name = context_type
        else:
            # Try to get it from the model's field default
            if hasattr(context_class, 'model_fields') and 'context_type' in context_class.model_fields:
                field = context_class.model_fields['context_type']
                if hasattr(field, 'default') and field.default is not None:
                    type_name = field.default
                else:
                    # Fall back to computed name
                    type_name = to_snake_case(context_class.__name__.removesuffix('Context'))
            else:
                raise ValidationError(
                    f"Context class {context_class.__name__} must have 'context_type' field or be registered with explicit context_type"
                )

        cls._registry[type_name] = context_class
        return context_class

    @classmethod
    def register_with_context_type(cls, context_type: str):
        """Decorator for manual registration with explicit context_type"""

        def decorator(context_class: Type[BaseModel]) -> Type[BaseModel]:
            return cls.register(context_class, context_type)

        return decorator

    @classmethod
    def get(cls, context_type: str) -> Type[BaseModel]:
        """Get a context class by its context_type string"""
        if context_type not in cls._registry:
            raise ValueError(f"Unknown context type: {context_type}. Registered types: {list(cls._registry.keys())}")
        return cls._registry[context_type]

    @classmethod
    def create(cls, data: Dict[str, Any], context_type: Optional[str] = None) -> BaseModel:
        """Create a context instance from data dictionary"""
        if context_type is None:
            context_type = data.get('context_type')
        if context_type is None:
            raise ValueError("Context data must include 'context_type' field")

        data['context_type'] = context_type
        context_class = cls.get(context_type)
        return context_class(**data)

    @classmethod
    def is_context_registered(cls, context_type: str) -> bool:
        """Check if a context_type is registered"""
        return context_type in cls._registry

    @classmethod
    def list_types(cls) -> list[str]:
        """List all registered context types"""
        return list(cls._registry.keys())
