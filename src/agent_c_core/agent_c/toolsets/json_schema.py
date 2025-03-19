import copy
from typing import Callable, Dict, Union, Any


def json_schema(description: str, params: Union[Dict[str, dict[str, Any]], None]) -> Callable:
    """
    A decorator to attach an OpenAI compatible JSON fields_wanted to a function. The fields_wanted contains
    information about the function's name, description, parameters, and required parameters.


    :param description: A description of the function.
    :param params: A dictionary containing information about the parameters of the function.
    :return: The original function with an attached JSON fields_wanted.
    """

    def decorator(func: Callable) -> Callable:
        if params is None:
            parameters = None
        else:
            properties: dict[str, dict[str, Any]] = copy.deepcopy(params)

            # Keep track of required parameters
            required = []
            for key in list(properties.keys()):
                if properties[key].get('required', False):
                    required.append(key)

                properties[key].pop('required', None)

            parameters = {
                'type': 'object',
                'properties': properties
            }

            if len(required) > 0:
                parameters['required'] = required

        # Define the fields_wanted with function name, description, and parameters
        schema = {
            "type": "function",
            "function": {
                'name': func.__name__,
                'description': description
            }
        }

        # Add parameters if they exist
        if parameters:
            schema['function']['parameters'] = parameters

        # Attach the fields_wanted to the original function
        func.schema = schema

        # Return the original function
        return func

    return decorator
