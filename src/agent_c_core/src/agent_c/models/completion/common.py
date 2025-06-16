from pydantic import Field
from typing import Optional, Any, TypeAlias, List

from agent_c.util import to_snake_case
from agent_c.models.base import BaseModel
from agent_c.models.completion.azure_auth_info import AzureAuthInfo
from agent_c.models.completion.bedrock_auth_info import BedrockAuthInfo

class SimpleAuthInfo(BaseModel):
    api_key: str = Field(..., description="The API key to use for the interaction")

ApiAuthInfo: TypeAlias = (SimpleAuthInfo | AzureAuthInfo | BedrockAuthInfo)


class CommonCompletionParams(BaseModel):
    model_id: str = Field(..., description="The name of the model id in the model config to use for the interaction")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokens to generate, defaults to backend defaults")
    user_name: Optional[str] = Field(None, description="The name of the user interacting with the agent")
    auth: Optional[ApiAuthInfo] = Field(None, description="The vendor API key or whatnot to use for the agent")

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return self.model_dump(exclude_none=True, exclude=set(['user_name', 'auth', 'model_id'] + extra_excludes))