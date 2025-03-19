import asyncio
import json
from typing import Union
from urllib.parse import quote_plus

import httpx
import logging
from .constants import _Unit, METRIC, IMPERIAL
from .enums import Locale
from .forecast import BaseForecast, HourlyForecast, DailyForecast, Forecast


class Weather:
    def __init__(self, locale: str = 'en', unit: str = 'imperial'):
        self.locale = Locale(locale)
        self.unit = IMPERIAL if unit.lower() == 'imperial' else METRIC
        self.logger: logging.Logger = logging.getLogger(__name__)

    async def get_forecast(self, location: str) -> Forecast | str:
        url = f'https://{self.locale.value}.wttr.in/{quote_plus(location)}?format=j1'
        return await self._fetch_url(url)

    async def _fetch_url(self, url: str, raw: bool = False, max_retries: int = 3) -> str:
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:131.0) Gecko/20100101 Firefox/131.0',
            'Content-Type': 'application/json'
        }

        for attempt in range(max_retries):
            async with httpx.AsyncClient() as client:
                try:
                    response = await client.get(url, headers=headers)

                    if response.status_code == 404 and response.text:
                        self.logger.warning(f'Got 404 but received content for URL: {url}')
                        response_content = response.text
                        if not raw:
                            response_content = self._format_content(response.content, url)
                        return response_content

                    response.raise_for_status()
                    response_content = response.text

                    if not raw:
                        response_content = self._format_content(response.content, url)
                    return response_content

                except httpx.HTTPStatusError as e:
                    if attempt == max_retries - 1:
                        self.logger.error(f'HTTP error occurred: {e}')
                        raise
                    await asyncio.sleep(1 * (attempt + 1))  # exponential backoff
                except httpx.RequestError as e:
                    self.logger.error(f'Request error occurred: {e}')
                    raise
                except Exception as e:
                    self.logger.error(f'An error occurred: {e}')
                    raise

    def _format_content(self, content: Union[str, bytes], url: str) -> Forecast:
        try:
            if isinstance(content, bytes):
                content = content.decode('utf-8')
            data = json.loads(content)
            return Forecast(data, self.unit, self.locale)
        except json.JSONDecodeError:
            self.logger.error(f"Failed to decode JSON from {url}")
            raise
        except Exception as e:
            self.logger.error(f"Error formatting content from {url}: {str(e)}")
            raise