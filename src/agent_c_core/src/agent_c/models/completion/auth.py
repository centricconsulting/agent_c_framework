from pydantic import Field
from typing import Literal, Annotated, Union

from agent_c.models.base import BaseModel
from agent_c.models.completion.azure_auth_info import AzureAuthInfo
from agent_c.models.completion.open_ai_auth_info import OpenAiAuthInfo
from agent_c.models.completion.bedrock_auth_info import BedrockAuthInfo
from agent_c.models.completion.gemini_auth_info import GeminiAuthInfo

OpenAiAuthInfoFull = Annotated[
    Union[
        AzureAuthInfo,
        OpenAiAuthInfo
    ],
    Field(discriminator='type')
]

class APIkeyAuthInfo(BaseModel):
    type: Literal['api_key'] = Field('api_key', description="The type of the auth info")
    api_key: str = Field(..., description="The API key to use for the interaction")

class ClaudeAuthInfo(APIkeyAuthInfo):
    type: Literal['claude'] = Field('claude', description="The type of the auth info")


AuthInfo = Annotated[
    Union[
        APIkeyAuthInfo,
        AzureAuthInfo,
        OpenAiAuthInfo,
        ClaudeAuthInfo,
        BedrockAuthInfo,
        GeminiAuthInfo
    ],
    Field(discriminator='type')
]