from .util import Weather
from agent_c.toolsets import json_schema, Toolset
import logging

from ...helpers.validate_kwargs import validate_required_fields


# Simple demonstration tool that grabs a weather forecast for a location.
class WeatherTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='weather', use_prefix=False)
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        'Call this to get the weather forecast for a location. Make sure you check what location to use',
        {
            'location_name': {
                'type': 'string',
                'description': 'The location to get the weather for.',
                'required': True
            }
        }
    )
    async def get_current_weather(self, **kwargs) -> str:
        """
        Tool method to get current weather - delegates to Weather class.
        
        Args:
            **kwargs: Tool arguments containing location_name
            
        Returns:
            JSON string with weather data
        """
        success, message = validate_required_fields(kwargs=kwargs, required_fields=['location_name'])

        if not success:
            raise ValueError(f"Error: {message}")

        location_name = kwargs.get('location_name')
        
        # Use the existing Weather class with the new formatted method
        weather_client = Weather(locale='en', unit='imperial')
        return await weather_client.get_formatted_weather_data(location_name)


Toolset.register(WeatherTools)
