from typing import Any

from agent_c.util.observable import ObservableDict
from agent_c.models.prompts.base import BasePromptSection
from agent_c.util import to_snake_case



class SectionBag(ObservableDict):
    """
    Custom container that handles section serialization/deserialization ensuring
    that section models not known to core cane be instantiated correctly.
    """

    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if isinstance(v, dict):
            result = {}
            for key, value in v.items():
                if isinstance(value, BasePromptSection):
                    result[key] = value
                elif isinstance(value, dict) or value is None:
                    from agent_c.util.registries.section_registry import SectionRegistry
                    result[key] = SectionRegistry.create(key, value)
                elif isinstance(value, BasePromptSection):
                    result[key] = value
                elif isinstance(value, str):
                    from agent_c.util.registries.section_registry import SectionRegistry
                    if SectionRegistry.is_section_registered(value):
                        section = SectionRegistry.create(value)
                        result[section.section_type] = section
                    else:
                        raise ValueError(f"Section type '{value}' is not registered in SectionRegistry.")
                else:
                    raise ValueError(f"Section value must be a BasePromptSection instance or dict, got {type(value)}")
            return cls(result)
        elif isinstance(v, list):
            result = {}
            for item in v:
                if isinstance(item, BasePromptSection):
                    result[item.section_type] = item
                elif isinstance(item, dict):
                    from agent_c.util.registries.section_registry import SectionRegistry
                    section = SectionRegistry.create(item.get('section_type', 'unknown'), item)
                    result[section.section_type] = section
                elif isinstance(item, str):
                    from agent_c.util.registries.section_registry import SectionRegistry
                    if SectionRegistry.is_section_registered(item):
                        section = SectionRegistry.create(item)
                        result[section.section_type] = section
                    else:
                        raise ValueError(f"Section type '{item}' is not registered in SectionRegistry.")
                else:
                    raise ValueError(f"Section value must be a BasePromptSection instance or dict, got {type(item)}")
            return cls(result)
        return v

    @property
    def section_types(self) -> list[str]:
        return list(self.keys())

    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_section_bag(value: Any) -> 'SectionBag':
            if isinstance(value, cls):
                return value
            if isinstance(value, dict):
                return cls(value)
            if hasattr(value, 'items'):
                return cls(value)
            raise ValueError(f"Cannot convert {type(value)} to SectionBag")

        return core_schema.no_info_plain_validator_function(validate_section_bag)

    @staticmethod
    def _normalize_key(item):
        """Extract the logic for normalizing keys into a separate method."""
        if isinstance(item, type):
            return to_snake_case(item.__name__)
        elif isinstance(item, str):
            return to_snake_case(item)
        elif isinstance(item, object):
            return to_snake_case(item.__class__.__name__)
        else:
            return item

    def __getitem__(self, item) -> BasePromptSection:
        item_key = self._normalize_key(item)

        try:
            return super().__getitem__(item_key)
        except KeyError:
            from agent_c.util.registries.section_registry import SectionRegistry
            # If the item is not found, try to create it from the registry
            if item_key != "shape" and SectionRegistry.is_section_registered(item_key):
                value = SectionRegistry.create(item_key)
                self[item] = value
                return value

            raise KeyError(f"Section '{item}' not found in bag and not registered in SectionRegistry.")

    def __setitem__(self, key, value):
        key = self._normalize_key(key)

        if isinstance(value, dict):
            from agent_c.util.registries.section_registry import SectionRegistry
            value = SectionRegistry.create(key, value)
        elif not isinstance(value, BasePromptSection):
            raise ValueError(f"Item type must be derived from BasePromptSection or dict. Got {type(value)}")

        super().__setitem__(key, value)

    def __getattr__(self, name: str) -> Any:
        """
        Override __getattr__ to ensure dot notation uses the same key normalization
        logic as bracket notation.
        """
        # First, check if we're trying to access a special/private attribute
        if name.startswith('__') and name.endswith('__'):
            raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")

        try:
            return self.__getitem__(name)
        except KeyError as e:
            raise AttributeError(str(e)) from e

    def __setattr__(self, name: str, value: Any) -> None:
        """
        Override __setattr__ to ensure dot notation uses the same logic as bracket notation.
        """
        if name.startswith('_') or hasattr(type(self), name):
            super().__setattr__(name, value)
        else:
            self.__setitem__(name, value)

    def __delattr__(self, name: str) -> None:
        """
        Handle attribute deletion to maintain consistency.
        """
        try:
            super().__delattr__(name)
        except AttributeError:
            try:
                super().__delitem__(self._normalize_key(name))
            except KeyError:
                raise AttributeError(
                    f"{type(self).__name__!r} object has no attribute {name!r} "
                    f"and no dictionary key {name!r}"
                )

    def __contains__(self, item) -> bool:
        return super().__contains__(self._normalize_key(item))

