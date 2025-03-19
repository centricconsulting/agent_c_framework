import json

from agent_c_tools.tools.weather.util import *

from agent_c_core.agent_c import json_schema, Toolset


# Simple demonstration tool that grabs a weather forecast for a location.
class WeatherTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='weather')
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
        location_name = kwargs.get('location_name')
        weather = await Weather(locale='en', unit='imperial').get_forecast(location_name)

        forecasts: [dict] = []

        try:
            for forecast in weather.daily_forecasts:
                forecasts.append({'date': forecast.date.strftime('%Y-%m-%d'),
                                  'high_temperature': forecast.highest_temperature,
                                  'low_temperature': forecast.lowest_temperature})

            results = json.dumps({"currently": {"current_temperature": weather.temperature, "sky":weather.kind.emoji,
                                                "feels_like": weather.feels_like, "humidity": weather.humidity,
                                                "wind_speed": weather.wind_speed, "wind_direction": (weather.wind_direction.value+weather.wind_direction.emoji),
                                                "visibility": weather.visibility, "uv_index": weather.ultraviolet.index,
                                  "description": weather.description, "forecasts": forecasts}})
        except Exception as e:
            self.logger.error(f"Error getting weather: {str(e)}")
            return f"Error getting weather: {str(e)}"

        # await self.chat_callback(render_media={"content-type": "text/html", "content": f"<div>{results}</div>"})
        return results


Toolset.register(WeatherTools)

