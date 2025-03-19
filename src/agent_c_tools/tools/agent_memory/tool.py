from typing import Any

from agent_c_core.agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.agent_memory.prompt import MemorySection


class MemoryTools(Toolset):
    """
    MemoryTools is a class that extends ZepDependentToolset to provide methods
    for storing and clearing metadata associated with the current user/session.

    This simple tool unlocks a LOT of capabilities, the model can store complex information
    using nothing more than this tool and some model instructions.  This is a tool you'll likely want
    to supply your own version of the `PromptSection` to provide better model instructions.
    """

    def __init__(self, **kwargs: Any):
        """Initialize MemoryTools with a MemorySection instance.

        Args:
            **kwargs (Any): Keyword arguments including those for ZepDependentToolset.
        """
        super().__init__(**kwargs, name='memory')
        self.section = kwargs.get('section', MemorySection(session_manager=self.session_manager))

    @json_schema(
        (
            'This tool allows you to store information in the metadata for the current user/session. '
            'Anything stored here will appear in the `Assistant data` heading for each location. '
            'You may store strings, or more complex data objects depending on your need. '
            'Only top level keys can be set, if you need to update a field in an object value, '
            'you must supply the whole object.'
        ),
        {
            'location': {
                'type': 'string',
                'description': 'Where to store the metadata. Must be one of `user` or `session`.',
                'required': True
            },
            'key': {
                'type': 'string',
                'description': 'The key to store the value under.',
                'required': True
            },
            'value': {
                "anyOf": [{"type": "object", "additionalProperties": True}, {"type": "string"}],
                'description': ('The value to store, this can be a string or a dictionary type '
                                'structure to hold more complex information.'),
                'required': True
            }
        }
    )
    async def store_metadata(self, **kwargs: Any) -> str:
        """Store or update metadata associated with the current user or session.

        Args:
            key (str): The key to store the value under.
            location (str): Where to store the metadata ('user' or 'session').
            value (str | dict): The value to store, can be a string or a more complex data object.
            prefix (str): Prefix used for metadata keys, defaults to 'ai_'.

        Returns:
            str: A message indicating that the value has been stored.
        """
        key: str = kwargs["key"]
        location: str = kwargs.get("location", "user")
        value: Any = kwargs["value"]
        prefix: str = kwargs.get("prefix", "ai_")

        if location == "user":
            md_temp = self.session_manager.get_user_meta_meta(prefix)
        else:
            md_temp = self.session_manager.get_session_meta_meta(prefix)

        md_temp[key] = value

        if location == "user":
            self.session_manager.set_user_meta_meta(prefix, md_temp)
        else:
            self.session_manager.set_session_meta_meta(prefix, md_temp)

        return f"Value for {key} stored in {location} metadata"

    @json_schema(
        'This tool allows you to remove a key from the user/session metadata.',
        {
            'location': {
                'type': 'string',
                'description': 'Which set of metadata to update. Must be one of `user` or `session`.',
                'required': True
            },
            'key': {
                'type': 'string',
                'description': 'The key to remove.',
                'required': True
            }
        }
    )
    async def clear_metadata_key(self, **kwargs: Any) -> str:
        """Remove metadata associated with a specified key from the current user or session.

        Args:
            key (str): The key for which to clear the value.
            location (str): Which set of metadata to update ('user' or 'session').
            prefix (str): Prefix used for metadata keys, defaults to 'ai_'.

        Returns:
            str: A message indicating that the key has been cleared.
        """
        key: str = kwargs["key"]
        location: str = kwargs.get("location", "user")
        prefix: str = kwargs.get("prefix", "ai_")

        if location == "user":
            md_temp = self.session_manager.get_user_meta_meta(prefix)
        else:
            md_temp = self.session_manager.get_session_meta_meta(prefix)

        md_temp.pop(key, None)

        if location == "user":
            self.session_manager.set_user_meta_meta(prefix, md_temp)
        else:
            self.session_manager.set_session_meta_meta(prefix, md_temp)

        return f"Value for {key} cleared from {location} metadata"


Toolset.register(MemoryTools)
