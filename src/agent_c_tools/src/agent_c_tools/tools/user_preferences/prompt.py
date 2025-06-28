from typing import Any, List, Callable
from .preferences.base import UserPreference
from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class UserPrefSection(OldPromptSection):
    user_preferences: List[UserPreference]
    model_preferences: Callable

    def __init__(self, **data: Any):
        TEMPLATE = ("A tool has been provided to you that allows you to work with user preferences.\n"
                    "The available user preferences are: ${pref_names}\n\n"
                    "Pay careful attention to the following user preferences as they include special instructions for you to follow based on their value: \n"
                    "Each preference is listed as `name`: `value`, default: `default_value`. The line following each contains directions for you to fallow based on this preference"
                    "${prefs_model_instructions}\n\n"
                    )
        data['template'] = data.get('template', TEMPLATE)
        super().__init__(required=True, name="User Preferences", render_section_header=True, **data)


    @property_bag_item
    async def prefs_model_instructions(self):
        return "\n".join([f"- `{p['name']}: `{p['current_value']}`, default: `{p['default_value']}`\n  - {p['model_instructions']}" for p in self.model_preferences()])

    @property_bag_item
    async def pref_names(self):
        return ', '.join(f"`{pref.name}`" for pref in self.user_preferences)


class UserPrefSectionNoToolUse(UserPrefSection):
    def __init__(self, **data: Any):
        TEMPLATE = ("Pay careful attention to the following user preferences as they include special instructions for you to follow based on their value: \n"
                    "Each preference is listed as `name`: `value`, default: `default_value`. The line following each contains directions for you to fallow based on this preference"
                    "${prefs_model_instructions}\n\n"
                    )

        super().__init__(template=TEMPLATE,  **data)