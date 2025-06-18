from pydantic import Field
from typing import Optional, Any

from agent_c.models.observable import ObservableModel, ObservableField
from agent_c.models.completion.auth import AuthInfo


class CommonCompletionParams(ObservableModel):
    model_id: str = ObservableField(..., description="The name of the model id in the model config to use for the interaction")
    model_name: Optional[str] = Field(None, description="The name of the model as used in API calls, if different from ID")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokens to generate, defaults to backend defaults")
    user_name: Optional[str] = Field(None, description="The name of the user interacting with the agent")
    auth: Optional[AuthInfo] = Field(None, description="The vendor API key or whatnot to use for the agent")

    def __init__(self, **data: Any) -> None:
        """
        Initialize the common completion parameters with defaults.

        Args:
            **data: Additional data to initialize the model
        """
        if 'model_name' not in data:
            data['model_name'] = data.get('model_id')

        super().__init__(**data)

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return self.model_dump(exclude_none=True, exclude=set(['user_name', 'auth', 'model_id', 'model_name'] + extra_excludes))
