import os

from pydantic import Field
from typing import Any, Optional, Literal, Union

from agent_c.models.completion.common import CommonCompletionParams

class ClaudeNonReasoningParams(CommonCompletionParams):
    """
    Vendor specific parameters for interacting with the Claude agent.
    """
    type: Literal['claude_non_reasoning'] = Field('claude_non_reasoning', description="The type of the completion params.")
    temperature: Optional[float] = Field(None, description="The temperature to use for the interaction, do not combine with top_p")
    max_searches: Optional[int] = Field(None, description="The maximum number of web searches for the claude models to perform")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokensto generate in the interaction")
    allow_betas: bool = Field(False, description="Whether to allow beta features in the interaction, defaults to False")
    allow_server_tools: bool = Field(False, description="Whether to allow server tools in the interaction, defaults to False")

    def __init__(self, **data: Any) -> None:
        if 'model_name' not in data:
            data['model_name'] = os.environ.get("CLAUDE_INTERACTION_MODEL", "claude-3-4-sonnet-latest")

        super().__init__(**data)

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return super().as_completion_params(extra_excludes + ['type','allow_betas', 'allow_server_tools'])

class ClaudeReasoningParams(CommonCompletionParams):
    type: Literal['claude_reasoning'] = Field('claude_reasoning', description="The type of the completion params.")
    budget_tokens: Optional[int] = Field(None, description="The budget tokens to use for the interaction, must be higher than max tokens")
    max_searches: Optional[int] = Field(None, description="The maximum number of web searches for the claude models to perform")
    allow_betas: bool = Field(False, description="Whether to allow beta features in the interaction, defaults to False")
    allow_server_tools: bool = Field(False, description="Whether to allow server tools in the interaction, defaults to False")
    temperature: Literal[1] = Field(1, description="The temperature for reasoning interactions is fixed at 1")

    def __init__(self, **data: Any) -> None:
        if 'model_name' not in data:
            data['model_name'] = os.environ.get("CLAUDE_REASONING_INTERACTION_MODEL", "claude-4-sonnet-latest")

        super().__init__(**data)

    def as_completion_params(self, extra_excludes: list[str]) -> dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        return super().as_completion_params(extra_excludes + ['type', 'allow_betas', 'allow_server_tools', 'budget_tokens'])


ClaudeCompletionParams = Union[ClaudeNonReasoningParams, ClaudeReasoningParams]
