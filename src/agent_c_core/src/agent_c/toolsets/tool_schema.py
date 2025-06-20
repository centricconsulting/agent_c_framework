import inspect
import re
from typing import Callable, Dict, Any, Optional, Union, List, get_origin, get_args


def parse_docstring_params(docstring: str) -> Dict[str, str]:
    """
    Parse parameter descriptions from docstring.
    Supports Google, Sphinx, and NumPy style docstrings.
    """
    if not docstring:
        return {}

    param_descriptions = {}

    # Google style: Args: param_name (type): description
    google_pattern = r'^\s*(\w+)\s*(?:\([^)]+\))?\s*:\s*(.+)$'

    # Sphinx style: :param param_name: description
    sphinx_pattern = r'^\s*:param\s+(\w+)\s*:\s*(.+)$'

    # Look for Args section (Google style)
    args_match = re.search(r'Args:\s*\n(.*?)(?:\n\s*\n|\n[A-Z][a-z]+:|\Z)', docstring, re.DOTALL)
    if args_match:
        args_section = args_match.group(1)
        for line in args_section.split('\n'):
            match = re.match(google_pattern, line.strip())
            if match:
                param_name, description = match.groups()
                param_descriptions[param_name] = description.strip()

    # Look for Sphinx style params
    for line in docstring.split('\n'):
        match = re.match(sphinx_pattern, line.strip())
        if match:
            param_name, description = match.groups()
            param_descriptions[param_name] = description.strip()

    return param_descriptions


def python_type_to_json_schema(py_type: type) -> Dict[str, Any]:
    """Convert Python type annotations to JSON schema types."""

    # Handle Union types (including Optional)
    origin = get_origin(py_type)
    if origin is Union:
        args = get_args(py_type)
        # Check if it's Optional (Union with None)
        if len(args) == 2 and type(None) in args:
            non_none_type = args[0] if args[1] is type(None) else args[1]
            return python_type_to_json_schema(non_none_type)
        else:
            # For now, just use the first type in Union
            return python_type_to_json_schema(args[0])

    # Basic type mappings
    type_map = {
        str: {"type": "string"},
        int: {"type": "integer"},
        float: {"type": "number"},
        bool: {"type": "boolean"},
        list: {"type": "array"},
        dict: {"type": "object"},
        Dict: {"type": "object"},
    }

    if py_type in type_map:
        return type_map[py_type]

    # Handle generic types
    if hasattr(py_type, '__origin__'):
        origin = py_type.__origin__
        if origin is list:
            return {"type": "array"}
        elif origin is dict:
            return {"type": "object"}

    # Default to string for unknown types
    return {"type": "string"}


def tool(user_description: Optional[str] = None, skip_params: Optional[List[str]] = None) -> Callable:
    """
    A decorator that automatically generates JSON schema from function signature and docstring.

    Args:
        user_description: Optional description override. If not provided, uses function docstring.
        skip_params: List of parameter names to exclude from schema (e.g., internal context params)

    Returns:
        The decorated function with attached schema.
    """

    def decorator(func: Callable) -> Callable:
        # Get function signature
        sig = inspect.signature(func)

        # Always extract description from docstring for JSON schema
        schema_description = None
        docstring = inspect.getdoc(func)
        if docstring:
            # Get everything before "Args:" or use first paragraph
            desc_match = re.search(r'^(.*?)(?:\n\s*\n|Args:|Parameters:|\Z)', docstring, re.DOTALL)
            if desc_match:
                schema_description = desc_match.group(1).strip()
            else:
                schema_description = docstring.strip()

        # Use schema description as fallback for user description
        if user_description is None:
            final_user_description = schema_description
        else:
            final_user_description = user_description

        # Fallback if no docstring found
        if schema_description is None:
            schema_description = f"Function {func.__name__}"

        # Parse parameter descriptions from docstring
        param_descriptions = parse_docstring_params(docstring or "")

        # Set default parameters to skip (internal framework params)
        if skip_params is None:
            params_to_skip = ['self', 'context']
        else:
            # Always include 'self' in skip list
            params_to_skip = ['self'] + skip_params

        # Build parameters schema
        properties = {}
        required = []

        for param_name, param in sig.parameters.items():
            # Skip internal parameters
            if param_name in params_to_skip:
                continue

            # Get type information
            param_type = param.annotation if param.annotation != inspect.Parameter.empty else str
            schema_type = python_type_to_json_schema(param_type)

            # Add description if available
            if param_name in param_descriptions:
                schema_type["description"] = param_descriptions[param_name]

            properties[param_name] = schema_type

            # Check if parameter is required (no default value)
            if param.default == inspect.Parameter.empty:
                required.append(param_name)

        # Build the complete schema
        schema = {
            "type": "function",
            "function": {
                "name": func.__name__,
                "description": schema_description
            }
        }

        if properties:
            parameters = {
                "type": "object",
                "properties": properties
            }
            if required:
                parameters["required"] = required
            schema["function"]["parameters"] = parameters

        # Attach schema and user description to function
        func.schema = schema
        func.user_description = final_user_description

        return func

    return decorator


# Alternative decorator that infers description from docstring
def auto_tool(func: Callable) -> Callable:
    """
    A decorator that automatically generates JSON schema with no parameters needed.
    Everything is inferred from the function signature and docstring.
    Automatically skips 'self' and 'context' parameters.
    """
    return tool()(func)


# Example usage:
if __name__ == "__main__":
    # Assuming you have an InteractionContext type
    class InteractionContext:
        pass


    @tool(user_description="List the keys in a section of the metadata for a workspace using a UNC style path")
    async def get_meta_keys(context: InteractionContext, path: str, max_tokens: int = 20000) -> str:
        """
        List the keys in a section of the metadata for a workspace using a UNC style path.
        This tool will allow you gain insight into the structure of the metadata without consuming too many tokens.

        Args:
            path: UNC style path in the form of //[workspace]/meta/toplevel/subkey1/subkey2
            max_tokens: Maximum size in tokens for the response. Default is 20k.

        Returns:
            str: The value for the specified key as a YAML formatted string or an error message.
        """
        # Your implementation here - context is available but not in schema
        pass


    # Or even simpler - auto-infer everything:
    @tool
    async def search_files(context: InteractionContext, query: str, file_type: Optional[str] = None, limit: int = 10) -> str:
        """
        Search for files in the workspace based on query.

        Args:
            query: The search query string
            file_type: Optional file type filter (e.g., 'pdf', 'txt')
            limit: Maximum number of results to return

        Returns:
            List of matching file paths as JSON string
        """
        pass


    # Print the generated schemas to show context is excluded
    import json

    print("get_meta_keys schema (context excluded):")
    print(json.dumps(get_meta_keys.schema, indent=2))
    print(f"User description: {get_meta_keys.user_description}")

    print("\nsearch_files schema (context excluded):")
    print(json.dumps(search_files.schema, indent=2))
    print(f"User description: {search_files.user_description}")
