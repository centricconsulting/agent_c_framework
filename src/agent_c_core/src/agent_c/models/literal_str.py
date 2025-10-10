"""
This module defines a custom YAML representer for a string type that should be represented as a
literal block in YAML using the pipe notation instead of escaping each line
.
It allows multiline strings to be represented in a more readable format.

Use `LiteralStr` in place of `str` in your model and it will be serialized as a YAML literal block.

"""
import yaml
from typing import Any


class LiteralStr(str):
    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_literal_str(value: Any) -> 'LiteralStr':
            if value is None:
                return None
            if isinstance(value, cls):
                return value
            if isinstance(value, str):
                return cls(value)

            return cls(str(value))


        return core_schema.no_info_plain_validator_function(validate_literal_str)

def literal_str_representer(dumper, data):
    return dumper.represent_scalar('tag:yaml.org,2002:str', data, style='|')

yaml.add_representer(LiteralStr, literal_str_representer)
yaml.add_representer(LiteralStr, literal_str_representer, Dumper=yaml.SafeDumper)

def str_presenter(dumper, data):
  if len(data.splitlines()) > 1:  # check for multiline string
    return dumper.represent_scalar('tag:yaml.org,2002:str', data, style='|')
  return dumper.represent_scalar('tag:yaml.org,2002:str', data)

yaml.add_representer(str, str_presenter)

# to use with safe_dump:
yaml.representer.SafeRepresenter.add_representer(str, str_presenter)
