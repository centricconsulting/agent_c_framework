from abc import ABC, abstractmethod
import re
import ast
import shlex
from argparse import ArgumentParser
from typing import Any


class CommandParser(ABC):
    @abstractmethod
    def parse(self, args_string: str) -> dict:
        """Parse argument string into dict of params"""
        pass


class NoArgsParser(CommandParser):
    def parse(self, args_string: str) -> dict:
        return {}


class JsonStyleParser(CommandParser):
    """Handles: !remind "14:00", "do the thing" """

    def parse(self, args_string: str) -> dict:
        # Wrap in brackets to make it a valid list
        try:
            args_list = ast.literal_eval(f"[{args_string}]")
            return {"args": args_list}
        except:
            raise ValueError(f"Invalid JSON-style arguments: {args_string}")


class ArgparseParser(CommandParser):
    """Handles: !command --with foo --flag"""

    def __init__(self, parser: ArgumentParser):
        self.parser = parser

    def parse(self, args_string: str) -> dict:
        # Use shlex to properly handle quoted strings
        args = shlex.split(args_string)
        parsed = self.parser.parse_args(args)
        return vars(parsed)


class FunctionCallParser(CommandParser):
    """Handles: !call_tool wsp_get_plan(plan_id: 'id', other: 123)"""

    def parse(self, args_string: str) -> dict:
        # Match: function_name(arg1: val1, arg2: val2)
        match = re.match(r'(\w+)\((.*)\)', args_string.strip())
        if not match:
            raise ValueError(f"Invalid function call syntax: {args_string}")

        func_name = match.group(1)
        params_str = match.group(2)

        # Parse the parameters
        params = {}
        if params_str.strip():
            # Convert "key: value" to "key=value" for ast parsing
            # Then parse as dict literal
            try:
                # Build a dict string and evaluate it
                param_pairs = []
                for param in params_str.split(','):
                    if ':' in param:
                        key, val = param.split(':', 1)
                        key = key.strip()
                        val = val.strip()
                        param_pairs.append(f'"{key}": {val}')

                dict_str = "{" + ", ".join(param_pairs) + "}"
                params = ast.literal_eval(dict_str)
            except:
                raise ValueError(f"Could not parse parameters: {params_str}")

        return {
            "function_name": func_name,
            "params": params
        }











