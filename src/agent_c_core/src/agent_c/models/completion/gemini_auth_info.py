from typing import Literal, Optional

from pydantic import Field

from agent_c.models import BaseModel


class GeminiAuthInfo(BaseModel):
    type: Literal['gemini'] = Field('gemini', description="The type of the auth info")
    api_key: Optional[str] = Field(None, description="The API key to use for the interaction")
    organization: Optional[str] = Field(None, description="The organization to use for the interaction, if any")
    project: Optional[str] = Field(None, description="The project to use for the interaction, if any")
    base_url: Literal['https://generativelanguage.googleapis.com/v1beta/openai/'] = Field('https://generativelanguage.googleapis.com/v1beta/openai/', description="The base URL to use for the interaction, if not provided, the endpoint will be used")
