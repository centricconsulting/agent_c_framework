from pydantic import Field, model_validator
from typing import Optional, Any

from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.completion.auth import AuthInfo


class CommonCompletionParams(AsyncObservableModel):
    model_id: str = Field(..., description="The name of the model id in the model config to use for the interaction")
    model_name: Optional[str] = Field(None, description="The name of the model as used in API calls, if different from ID")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokens to generate, defaults to backend defaults")
    user_name: Optional[str] = Field(None, description="The name of the user interacting with the agent")
    auth: Optional[AuthInfo] = Field(None, description="The vendor API key or whatnot to use for the agent")

    def model_post_init(self, __context):
        """Hook up observer after model initialization"""
        super().model_post_init(__context)
        if self.model_name is None:
            self.model_name = self.model_id

        return self

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return self.model_dump(exclude_none=True, exclude=set(['user_name', 'auth', 'model_id', 'model_name'] + extra_excludes))
