from pydantic import Field
from typing import Optional, Any

from .auth import AuthInfo
from agent_c.models.base import BaseModel



class CommonCompletionParams(BaseModel):
    model_id: str = Field(..., description="The name of the model id in the model config to use for the interaction")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokens to generate, defaults to backend defaults")
    user_name: Optional[str] = Field(None, description="The name of the user interacting with the agent")
    auth: Optional[AuthInfo] = Field(None, description="The vendor API key or whatnot to use for the agent")

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return self.model_dump(exclude_none=True, exclude=set(['user_name', 'auth', 'model_id'] + extra_excludes))