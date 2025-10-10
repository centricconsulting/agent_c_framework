from typing import Literal, Optional

from pydantic import Field

from agent_c.models import BaseModel


class OpenAiAuthInfo(BaseModel):
    type: Literal['open_ai'] = Field('open_ai', description="The type of the auth info")
    api_key: str = Field(None, description="The API key to use for the interaction")
    organization: Optional[str] = Field(None, description="The organization to use for the interaction, if any")
    project: Optional[str] = Field(None, description="The project to use for the interaction, if any")
    base_url: Optional[str] = Field(None, description="The base URL to use for the interaction, if not provided, the endpoint will be used")