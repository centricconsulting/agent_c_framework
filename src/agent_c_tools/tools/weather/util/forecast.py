from typing import Iterable, Optional, Tuple, List
from datetime import datetime, date, time

from .enums import Phase, HeatIndex, WindDirection, Kind, Locale, UltraViolet
from .constants import _Unit, LATLON_REGEX

class BaseForecast:
  __slots__: Tuple[str, ...] = (
    'ultraviolet',
    'humidity',
    'wind_direction',
    'kind',
    'feels_like',
    'temperature',
    'precipitation',
    'pressure',
    'visibility',
    'wind_speed',
    'description',
  )

  ultraviolet: UltraViolet
  """The ultra-violet (UV) index."""

  humidity: int
  """The humidity value in percent."""

  wind_direction: WindDirection
  """The wind direction."""

  kind: Kind
  """The kind of the forecast."""

  feels_like: int
  """What it felt like, in celcius or fahrenheit."""

  temperature: int
  """The temperature in either celcius or Fahrenheit."""

  precipitation: float
  """The precipitation in either millimeters or inches."""

  pressure: float
  """The pressure in either pascal or inches."""

  visibility: int
  """The visibility distance in either kilometers or miles."""

  wind_speed: int
  """The wind speeds in either kilometers/hour or miles/hour."""

  description: str
  """The description regarding the forecast. This can be localized in different languages depending on the localization used."""

  def __init__(self, json: dict, unit: _Unit, locale: Locale):
    description = (
      json['weatherDesc'][0]['value']
      if locale is Locale.ENGLISH
      else json[f'lang_{locale.value}'][0]['value']
    )

    self.ultraviolet = UltraViolet._new(int(json['uvIndex']))
    self.humidity = int(json['humidity'])
    self.wind_direction = WindDirection._new(
      json['winddir16Point'], int(json['winddirDegree'])
    )
    self.kind = Kind(int(json['weatherCode']))
    self.feels_like = int(json[f'FeelsLike{unit.temperature}'])
    self.temperature = int(json[f'temp_{unit.temperature}'])
    self.precipitation = float(json[f'precip{unit.precipitation}'])
    self.pressure = float(json[f'pressure{unit.pressure}'])
    self.visibility = int(json[f'visibility{unit.visibility}'])
    self.wind_speed = int(json[f'windspeed{unit.velocity}'])
    self.description = description.strip()

class HourlyForecast(BaseForecast):
  """Represents a weather forecast of a specific hour."""

  __slots__: Tuple[str, ...] = (
    'chances_of_fog',
    'chances_of_frost',
    'chances_of_high_temperature',
    'chances_of_overcast',
    'chances_of_rain',
    'chances_of_remaining_dry',
    'chances_of_snow',
    'chances_of_sunshine',
    'chances_of_thunder',
    'chances_of_windy',
    'cloud_cover',
    'time',
    'dew_point',
    'heat_index',
    'wind_chill',
    'wind_gust',
  )

  chances_of_fog: int
  """Chances of a fog in percent."""

  chances_of_frost: int
  """Chances of a frost in percent."""

  chances_of_high_temperature: int
  """Chances of a high temperature in percent."""

  chances_of_overcast: int
  """Chances of an overcast in percent."""

  chances_of_rain: int
  """Chances of a rain in percent."""

  chances_of_remaining_dry: int
  """Chances of remaining dry in percent."""

  chances_of_snow: int
  """Chances of a snow in percent."""

  chances_of_sunshine: int
  """Chances of a sunshine in percent."""

  chances_of_thunder: int
  """Chances of a thunder in percent."""

  chances_of_windy: int
  """Chances of windy in percent."""

  cloud_cover: int
  """The cloud cover value in percent."""

  time: 'time'
  """The local time in hours and minutes."""

  dew_point: int
  """The dew point in either celcius or fahrenheit."""

  heat_index: HeatIndex
  """The heat index in either celcius or fahrenheit."""

  wind_chill: int
  """The wind chill value in either celcius or fahrenheit."""

  wind_gust: int
  """The wind gust value in either kilometers/hour or miles/hour."""

  def __init__(self, json: dict, unit: _Unit, locale: Locale):
    # for inheritance purposes
    if 'temp_C' not in json:
      json['temp_C'] = json.pop('tempC')
    if 'temp_F' not in json:
      json['temp_F'] = json.pop('tempF')

    celcius_index = int(json['HeatIndexC'])
    t = json['time']

    self.chances_of_fog = int(json['chanceoffog'])
    self.chances_of_frost = int(json['chanceoffrost'])
    self.chances_of_high_temperature = int(json['chanceofhightemp'])
    self.chances_of_overcast = int(json['chanceofovercast'])
    self.chances_of_rain = int(json['chanceofrain'])
    self.chances_of_remaining_dry = int(json['chanceofremdry'])
    self.chances_of_snow = int(json['chanceofsnow'])
    self.chances_of_sunshine = int(json['chanceofsunshine'])
    self.chances_of_thunder = int(json['chanceofthunder'])
    self.chances_of_windy = int(json['chanceofwindy'])
    self.cloud_cover = int(json['cloudcover'])
    self.time = time() if len(t) < 3 else datetime.strptime(t, '%H%M').time()
    self.dew_point = int(json[f'DewPoint{unit.temperature}'])
    self.heat_index = HeatIndex._new(
      celcius_index,
      int(json[f'HeatIndex{unit.temperature}']),
    )
    self.wind_chill = int(json[f'WindChill{unit.temperature}'])
    self.wind_gust = int(json[f'WindGust{unit.velocity}'])

    super().__init__(json, unit, locale)

  def __repr__(self) -> str:
    return f'<{__class__.__name__} time={self.time!r} temperature={self.temperature!r} description={self.description!r} kind={self.kind!r}>'


class DailyForecast:
  __slots__: Tuple[str, ...] = (
    'moon_illumination',
    'moon_phase',
    'moonrise',
    'moonset',
    'sunrise',
    'sunset',
    'date',
    'sunlight',
    'lowest_temperature',
    'highest_temperature',
    'temperature',
    'snowfall',
    'hourly_forecasts',
  )

  moon_illumination: int
  """The percentage of the moon illuminated."""

  moon_phase: Phase
  """The moon's phase."""

  moonrise: Optional[time]
  """The local time when the moon rises. This can be ``None``."""

  moonset: Optional[time]
  """The local time when the moon sets. This can be ``None``."""

  sunrise: Optional[time]
  """The local time when the sun rises. This can be ``None``."""

  sunset: Optional[time]
  """The local time when the sun sets. This can be ``None``."""

  date: 'date'
  """The local date of this forecast."""

  sunlight: float
  """Hours of sunlight."""

  lowest_temperature: int
  """The lowest temperature in either celcius or fahrenheit."""

  highest_temperature: int
  """The highest temperature in either celcius or fahrenheit."""

  temperature: int
  """The average temperature in either celcius or fahrenheit."""

  snowfall: float
  """Total snowfall in either centimeters or inches."""

  hourly_forecasts: List[HourlyForecast]
  """The hourly forecasts of this day."""

  def __init__(self, json: dict, unit: _Unit, locale: Locale):
    astronomy = json['astronomy'][0]

    self.moon_illumination = int(astronomy['moon_illumination'])
    self.moon_phase = Phase(astronomy['moon_phase'])
    self.moonrise = __class__.__parse_time(astronomy['moonrise'])
    self.moonset = __class__.__parse_time(astronomy['moonset'])
    self.sunrise = __class__.__parse_time(astronomy['sunrise'])
    self.sunset = __class__.__parse_time(astronomy['sunset'])
    self.date = datetime.strptime(json['date'], '%Y-%m-%d').date()
    self.sunlight = float(json['sunHour'])
    self.lowest_temperature = int(json[f'mintemp{unit.temperature}'])
    self.highest_temperature = int(json[f'maxtemp{unit.temperature}'])
    self.temperature = int(json[f'avgtemp{unit.temperature}'])
    self.snowfall = float(json['totalSnow_cm']) / unit.cm_divisor
    self.hourly_forecasts = [
      HourlyForecast(elem, unit, locale) for elem in json['hourly']
    ]

  @staticmethod
  def __parse_time(timestamp: str) -> Optional[time]:
    try:
      return datetime.strptime(timestamp, '%I:%M %p').time()
    except ValueError:
      ...

  def __repr__(self) -> str:
    return f'<{__class__.__name__} date={self.date!r} temperature={self.temperature!r}>'

  def __len__(self) -> int:
    return len(self.hourly_forecasts)

  def __iter__(self) -> Iterable[HourlyForecast]:
    return iter(self.hourly_forecasts)


class Forecast(BaseForecast):
  """Represents today's weather forecast, alongside daily and hourly weather forecasts."""

  __slots__: Tuple[str, ...] = (
    'local_population',
    'region',
    'location',
    'country',
    'datetime',
    'coordinates',
    'daily_forecasts',
  )

  local_population: int
  """The local population count."""

  region: str
  """The local region's name."""

  location: str
  """The location's name."""

  country: str
  """The local country's name."""

  datetime: 'datetime'
  """The local date and time of this weather forecast."""

  coordinates: Tuple[float, float]
  """A tuple of this forecast's latitude and longitude."""

  daily_forecasts: List[DailyForecast]
  """Daily weather forecasts in this location."""

  def __init__(self, json: dict, unit: _Unit, locale: Locale):
    current = json['current_condition'][0]
    nearest = json['nearest_area'][0]

    self.local_population = int(nearest['population'])
    self.region = nearest['region'][0]['value']
    self.location = nearest['areaName'][0]['value']
    self.country = nearest['country'][0]['value']
    self.datetime = datetime.strptime(current['localObsDateTime'], '%Y-%m-%d %I:%M %p')

    try:
      req = next(filter(lambda x: x['type'] == 'LatLon', json['request']))
      match = LATLON_REGEX.match(req['query'])

      self.coordinates = (float(match[1]), float(match[2]))
    except:
      self.coordinates = (float(nearest['latitude']), float(nearest['longitude']))

    self.daily_forecasts = [
      DailyForecast(elem, unit, locale) for elem in json['weather']
    ]

    super().__init__(current, unit, locale)

  def __repr__(self) -> str:
    return f'<{__class__.__name__} location={self.location!r} datetime={self.datetime!r} temperature={self.temperature!r}>'

  def __len__(self) -> int:
    return len(self.daily_forecasts)

  def __iter__(self) -> Iterable[DailyForecast]:
    return iter(self.daily_forecasts)
