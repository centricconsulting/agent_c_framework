from typing import Literal, Optional

from pydantic import Field

from agent_c.models import BaseModel


class OpenAiAuthInfo(BaseModel):
    type: Literal['open_ai'] = Field('basic', description="The type of the auth info, must be basic")
    api_key: str = Field(..., description="The API key to use for the interaction")
    organization: Optional[str] = Field(None, description="The organization to use for the interaction, if any")
    project: Optional[str] = Field(None, description="The project to use for the interaction, if any")