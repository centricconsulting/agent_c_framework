import json
from typing import Optional, List, Dict, Any
from agent_c_core.agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.user_preferences import UserPreference
from agent_c_tools.tools.user_preferences.prompt import UserPrefSection, UserPrefSectionNoToolUse


class PreferenceTools(Toolset):
    """
    PreferenceTools class provides methods to manage and interact with user preferences.
    This includes finding, getting, resetting or listing user preferences and their values.

    """

    def __init__(self, **kwargs: Any) -> None:
        """Initialize PreferenceTools with a list of user preferences.

        Args:
            **kwargs: Arbitrary keyword arguments, expecting 'user_preferences' to be included.
        """
        super().__init__(**kwargs, name='prefs', need_tool_user=False)
        self.user_preferences: List[UserPreference] = kwargs.get('user_preferences', [])

        if self.agent_can_use_tools:
            pref_cls = UserPrefSection
        else:
            pref_cls = UserPrefSectionNoToolUse

        # Note: user preferences is passed as a list, the ones that are visible to the model are provided via a callback
        self.section = kwargs.get('section', pref_cls(user_preferences=self.user_preferences, model_preferences=self.get_model_prefs))

    def find_preference_by_name(self, name: str) -> Optional[UserPreference]:
        """Find a user preference by its name.

        Args:
            name (str): The name of the preference to find.

        Returns:
            Optional[UserPreference]: The UserPreference object if found, else None.
        """
        for preference in self.user_preferences:
            if preference.name == name:
                return preference
        return None

    def get_user_preference(self, key: str) -> str:
        """Get the value of a user preference by its key.

        Args:
            key (str): The key of the preference to get.

        Returns:
            str: The value of the preference, either from user metadata or default.
        """
        preference: Optional[UserPreference] = self.find_preference_by_name(key)
        if not preference or self.session_manager.user.metadata is None:
            return preference.default_value if preference else ""

        return self.session_manager.user.metadata.get(f"pref_{preference.name}", preference.default_value)

    def get_model_prefs(self) -> List[Dict[str, Any]]:
        """Get the list of model preferences visible to the model along with their current and default values.

        Returns:
            List[Dict[str, Any]]: The list of preferences that are visible to the model.
        """
        model_subset: List[UserPreference] = [p for p in self.user_preferences if p.visible_to_model]
        preferences_list: List[Dict[str, Any]] = []

        for preference in model_subset:
            current_value: str = self.session_manager.user.metadata.get(
                f"pref_{preference.name}", preference.default_value) if self.session_manager.user.metadata else preference.default_value

            preference_dict: Dict[str, str] = {
                "name": preference.name,
                "default_value": preference.default_value,
                "current_value": current_value,
                "model_instructions": preference.model_instructions,
            }
            preferences_list.append(preference_dict)

        return preferences_list

    def __is_invalid_pref(self, key: str) -> bool:
        """Check if the provided key is not a valid preference.

        Args:
            key (str): The key to check.

        Returns:
            bool: True if the preference name is invalid, otherwise False.
        """
        return all(preference.name != key for preference in self.user_preferences)

    @json_schema(
        'This tool allows you to set one of the various user preference options mentioned in the user preferences section.',
        {
            'key': {
                'type': 'string',
                'description': 'The preference to set.',
                'required': True
            },
            'value': {
                'type': 'string',
                'description': 'The preference value to store',
                'required': True
            }

        }
    )
    async def save_user_pref(self, **kwargs: Any) -> str:
        """Save a user preference with a given key to the provided value.

        Args:
            **kwargs: Arbitrary keyword arguments:
                'key' (str): The preference to set.
                'value' (str): The preference value to store.
                'prefix' (str, optional): Prefix used for user preference metadata keys.

        Returns:
            str: Message indicating the status of the preference update.
        """
        key: str = kwargs["key"]
        value: str = kwargs["value"]
        prefix: str = kwargs.get("prefix", "pref_")

        if self.__is_invalid_pref(key):
            return f"`{key}` is not a valid user preference name. Did you mean to save a user KVP?"

        self.session_manager.user.metadata = self.session_manager.user.metadata or {}
        self.session_manager.user.metadata[f"{prefix}{key}"] = value
        return f"User preference `{key}` set to `{value}`."

    @json_schema(
        "This tool allows you set a user preference to its default value.",
        {
            'key': {
                'type': 'string',
                'description': 'The preference to reset.',
                'required': True
            }
        }
    )
    async def reset_user_pref(self, **kwargs: Any) -> str:
        """Reset a user preference to its default value.

        Args:
            **kwargs: Arbitrary keyword arguments:
                'key' (str): The preference to reset.
                'prefix' (str, optional): Prefix used for user preference metadata keys.

        Returns:
            str: Message indicating the status of the preference reset.
        """
        key: str = kwargs["key"]
        prefix: str = kwargs.get("prefix", "pref_")

        if self.__is_invalid_pref(key):
            return f"`{key}` is not a valid user preference name. Did you mean to save a user KVP?"

        pref: Optional[UserPreference] = self.find_preference_by_name(key)
        if pref is not None:
            self.session_manager.user.metadata = self.session_manager.user.metadata or {}
            self.session_manager.user.metadata[f"{prefix}{key}"] = pref.default_value

        return f"User preference `{key}` reset."

    @json_schema(
        "This tool provides the full list of user preferences that can be set, their default values and a description for the user.",
        {}
    )
    async def list_user_prefs(self, **kwargs: Any) -> str:
        """List all user preferences including name, default value, current value, and user instructions.

        Args:
            **kwargs: Arbitrary keyword arguments (unused in this method).

        Returns:
            str: JSON string containing the list of user preferences.
        """
        preferences_list: List[Dict[str, Any]] = []

        for preference in self.user_preferences:
            current_value: str = self.session_manager.user.metadata.get(
                f"pref_{preference.name}", preference.default_value) if self.session_manager.user.metadata else preference.default_value

            preference_dict: Dict[str, str] = {
                "name": preference.name,
                "default_value": preference.default_value,
                "current_value": current_value,
                "user_instructions": preference.user_instructions,
            }
            preferences_list.append(preference_dict)

        return json.dumps({'user_preferences': preferences_list})


Toolset.register(PreferenceTools)
