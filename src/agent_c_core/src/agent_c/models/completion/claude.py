import os

from pydantic import Field, model_validator
from typing import Any, Optional, Literal, Union, List, Dict
from agent_c.registration.configs import get_system_config
from agent_c.models.completion.common import CommonCompletionParams

class ClaudeCommonParams(CommonCompletionParams):
    max_searches: Optional[int] = Field(None, description="The maximum number of web searches for the claude models to perform")
    allow_betas: bool = Field(False, description="Whether to allow beta features in the interaction, defaults to False")
    allow_server_tools: bool = Field(False, description="Whether to allow server tools in the interaction, defaults to False")

class ClaudeNonReasoningParams(ClaudeCommonParams):
    """
    Vendor specific parameters for interacting with the Claude agent.
    """
    type: Literal['claude_non_reasoning'] = Field('claude_non_reasoning', description="The type of the completion params.")
    temperature: Optional[float] = Field(None, description="The temperature to use for the interaction, do not combine with top_p")
    max_tokens: Optional[int] = Field(None, description="The maximum number of tokens to generate in the interaction")

    def __init__(self, **data: Any) -> None:
        if 'model_id' not in data:
            data['model_id'] = os.environ.get("CLAUDE_INTERACTION_MODEL", "claude-sonnet-4-20250514")

        super().__init__(**data)

    @model_validator(mode='before')
    def validate_model_id(cls, values: Dict[str, Any]) -> Dict[str, Any]:
        """
        Validates the model_id to ensure it is set to a valid Claude model.
        """
        if 'model_id' not in values or not values['model_id']:
            values['model_id'] = get_system_config().runtimes.claude.default_non_reasoning_model

        # Additional validation can be added here if needed
        return values

    def as_completion_params(self, extra_excludes: Optional[List[str]] = None) -> Dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        if extra_excludes is None:
            extra_excludes = []

        opts = super().as_completion_params(extra_excludes + ['type', 'allow_betas', 'allow_server_tools', 'max_searches'])
        from agent_c.config import ModelConfigurationLoader
        opts['model'] = ModelConfigurationLoader.instance().model_id_map[self.model_id].model_name
        return opts

class ClaudeReasoningParams(ClaudeCommonParams):
    type: Literal['claude_reasoning'] = Field('claude_reasoning', description="The type of the completion params.")
    budget_tokens: Optional[int] = Field(None, description="The budget tokens to use for the interaction, must be higher than max tokens")
    temperature: Literal[1] = Field(1, description="The temperature for reasoning interactions is fixed at 1")

    @model_validator(mode='before')
    def validate_model_id(cls, values: Dict[str, Any]) -> Dict[str, Any]:
        """
        Validates the model_id to ensure it is set to a valid Claude model.
        """
        if 'model_id' not in values or not values['model_id']:
            values['model_id'] = get_system_config().runtimes.claude.default_reasoning_model

        # Additional validation can be added here if needed
        return values

    def as_completion_params(self, extra_excludes: Optional[List[str]] = None) -> Dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        if extra_excludes is None:
            extra_excludes = []

        opts = super().as_completion_params(extra_excludes + ['type', 'allow_betas', 'allow_server_tools', 'budget_tokens'])
        from agent_c.config import ModelConfigurationLoader
        opts['model'] = ModelConfigurationLoader.instance().model_id_map[self.model_id].model_name
        return opts




ClaudeCompletionParams = Union[ClaudeNonReasoningParams, ClaudeReasoningParams]
