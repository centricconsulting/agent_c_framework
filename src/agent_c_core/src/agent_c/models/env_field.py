import os
import json
from typing import Any, Union, get_origin, get_args
from pydantic import Field, BaseModel

def EnvField(
        env_var: str,
        default: Any = None,
        **kwargs
) -> Any:
    """
    Create a Pydantic field that uses an environment variable as the default value.

    Args:
        env_var: The environment variable name to check
        default: Fallback default value if env var is not set
        **kwargs: Additional Field arguments

    Returns:
        A Pydantic Field with environment variable support
    """

    def default_factory():
        # Check if environment variable exists
        env_value = os.getenv(env_var)
        if env_value is not None:
            return env_value
        return default

    # Use default_factory for the Field
    return Field(default_factory=default_factory, **kwargs)


class EnvFieldMixin:
    """Mixin to add environment variable processing to Pydantic models."""

    def __init__(self, **data):
        # Process environment variables before calling parent __init__
        processed_data = self._process_env_fields(data)
        super().__init__(**processed_data)

    def _process_env_fields(self, data: dict) -> dict:
        """Process fields that have environment variable defaults."""
        processed = data.copy()

        for field_name, field_info in self.model_fields.items():
            # Skip if value was explicitly provided
            if field_name in processed:
                continue

            # Check if this field has a default_factory (our EnvField marker)
            if hasattr(field_info, 'default_factory') and field_info.default_factory is not None:
                try:
                    raw_value = field_info.default_factory()
                    if raw_value is not None:
                        # Get the field's type annotation
                        field_type = self.model_fields[field_name].annotation
                        converted_value = self._convert_env_value(raw_value, field_type)
                        processed[field_name] = converted_value
                except Exception as e:
                    # If conversion fails, let Pydantic handle validation
                    print(f"Warning: Failed to process env field {field_name}: {e}")

        return processed

    def _convert_env_value(self, value: str, field_type: type) -> Any:
        """Convert string environment variable value to the appropriate type."""
        if value == "":
            return None

        # Handle None/null values
        if value.lower() in ('none', 'null'):
            return None

        # If it's already the correct type, return as-is
        if isinstance(value, field_type) and not isinstance(value, str):
            return value

        # Handle Optional types
        origin = get_origin(field_type)
        if origin is Union:
            args = get_args(field_type)
            # Check if it's Optional (Union with None)
            if len(args) == 2 and type(None) in args:
                # Get the non-None type
                actual_type = args[0] if args[1] is type(None) else args[1]
                return self._convert_env_value(value, actual_type)

        # Handle basic types
        if field_type == str:
            return value
        elif field_type == int:
            return int(value)
        elif field_type == float:
            return float(value)
        elif field_type == bool:
            return value.lower() in ('true', '1', 'yes', 'on')
        elif field_type == list:
            # Try to parse as JSON, fallback to comma-separated
            try:
                return json.loads(value)
            except json.JSONDecodeError:
                return [item.strip() for item in value.split(',') if item.strip()]
        elif field_type == dict:
            return json.loads(value)
        elif isinstance(field_type, type) and issubclass(field_type, BaseModel):
            # Handle Pydantic models - parse JSON and create instance
            if isinstance(value, str):
                data = json.loads(value)
                return field_type(**data)
            elif isinstance(value, dict):
                return field_type(**value)
            else:
                return field_type(value)
        else:
            # For other types, try JSON parsing first
            try:
                return json.loads(value)
            except (json.JSONDecodeError, TypeError):
                # Fallback to direct conversion
                try:
                    return field_type(value)
                except (TypeError, ValueError):
                    return value
