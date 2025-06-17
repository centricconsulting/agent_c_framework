from typing import Literal

from pydantic import Field

from agent_c.models import BaseModel


class OpenAiAuthInfo(BaseModel):
    type: Literal['open_ai'] = Field('basic', description="The type of the auth info, must be basic")
    api_key: str = Field(..., description="The API key to use for the interaction")
