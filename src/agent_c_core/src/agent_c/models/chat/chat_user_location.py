import pgeocode

from typing import Optional
from pydantic import Field
# from uszipcode import SearchEngine
from timezonefinder import TimezoneFinder

from agent_c.models.base import BaseModel

class ChatUserLocation(BaseModel):
    postal_code: Optional[str] = Field(None, description="The postal code of the user's location")
    city: Optional[str] = Field(None, description="The city where the user is located")
    country: Optional[str] = Field(None, description="The country where the user is located")
    region: Optional[str] = Field(None, description="The region or state where the user is located")
    timezone: Optional[str] = Field(None, description="The IANA timezone of the user's location, e.g., 'America/New_York'")

    @classmethod
    def from_postal_code(cls, postal_code: str, country_code: str = "US") -> 'ChatUserLocation':
        nomi = pgeocode.Nominatim(country_code.lower())
        result = nomi.query_postal_code(postal_code)

        if hasattr(result, 'place_name') and str(result.place_name) != 'nan':
            tf = TimezoneFinder()
            timezone = tf.timezone_at(lat=float(str(result.latitude)), lng=float(str(result.longitude)))

            return cls(
                postal_code=postal_code,
                city=str(result.place_name),
                country=str(result.country_code).upper(),
                region=str(result.state_name) if hasattr(result, 'state_name') else None,
                timezone=timezone
            )
        else:
            return cls(postal_code=postal_code)

    @classmethod
    def from_zipcode(cls, zipcode: str) -> 'ChatUserLocation':
        return cls(postal_code=zipcode)
        # usezipcode has issues and needs fixed before it can be used reliably
        #
        # search = SearchEngine()
        # result = search.by_zipcode(zipcode)
        #
        # if result.zipcode:
        #     return cls(
        #         postal_code=result.zipcode,
        #         city=result.major_city,
        #         country="United States",
        #         region=result.state,
        #         timezone=result.timezone
        #     )
        # else:

