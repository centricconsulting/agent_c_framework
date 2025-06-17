from typing import Literal, Annotated, Union

from pydantic import Field

from .azure_auth_info import AzureAuthInfo
from .open_ai_auth_info import OpenAiAuthInfo
from  .bedrock_auth_info import BedrockAuthInfo
from .. import BaseModel


class APIkeyAuthInfo(BaseModel):
    type: Literal['api_key'] = Field('basic', description="The type of the auth info, must be basic")
    api_key: str = Field(..., description="The API key to use for the interaction")

AuthInfo = Annotated[
    Union[
        APIkeyAuthInfo,
        AzureAuthInfo,
        OpenAiAuthInfo,
        BedrockAuthInfo,
    ],
    Field(discriminator='type')
]