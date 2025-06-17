from pydantic import Field
from typing import Optional, Literal
from agent_c.models.base import BaseModel

class AzureAuthInfo(BaseModel):
    type: Literal['azure'] = Field('azure', description="The type of the auth info, must be azure")
    endpoint: str = Field(..., description="The Azure OpenAI endpoint to use for the interaction")
    api_key: str = Field(..., description="The Azure OpenAI API key to use for the interaction")
    api_version: Optional[str] = Field("2024-08-01-preview", description="The Azure OpenAI API version to use for the interaction")

