"""
This module defines a custom YAML representer for a string type that should be represented as a
literal block in YAML using the pipe notation instead of escaping each line
.
It allows multiline strings to be represented in a more readable format.

Use `LiteralStr` in place of `str` in your model and it will be serialized as a YAML literal block.

"""
from typing import Annotated, Any, Optional

import yaml
from pydantic import PlainValidator


class LiteralStr(str):
    pass

def literal_str_representer(dumper, data):
    return dumper.represent_scalar('tag:yaml.org,2002:str', data, style='|')

yaml.add_representer(LiteralStr, literal_str_representer)
yaml.add_representer(LiteralStr, literal_str_representer, Dumper=yaml.SafeDumper)


def ensure_literal_str(v: Any) -> Optional[LiteralStr]:
    if v is None:
        return None
    if isinstance(v, LiteralStr):
        return v
    if isinstance(v, str):
        return LiteralStr(v)

    raise TypeError(f'Invalid type for LiteralStr: {type(v)}')

LiteralStrField = Annotated[LiteralStr, PlainValidator(ensure_literal_str)]
