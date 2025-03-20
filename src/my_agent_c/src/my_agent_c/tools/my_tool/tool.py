import json

import python_weather

from agent_c_core.agent_c import json_schema, Toolset
from agent_c_core.agent_c.util.slugs import MnemonicSlugs


# Simple demonstration tool that grabs a weather forecast for a location.
class MyTool(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='wod')

    @json_schema(
            "Call this to get the word of the day.  If you don't know the password, ask the user.",

            {
                'password': {
                    'type': 'string',
                    'description': 'The password needed to get the word of the day.',
                    'required': True
                }
            }
    )
    async def get_word_of_day(self, **kwargs) -> str:
        password = kwargs.get('password')

        if password != 'swordfish':
            return "Invalid password. Ask the user for the password and try again."
        else:
            return f"The word of the day is '{MnemonicSlugs.generate_slug(1)}'."

Toolset.register(MyTool)

