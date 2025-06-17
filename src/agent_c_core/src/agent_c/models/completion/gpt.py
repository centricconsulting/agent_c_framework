import os
from pydantic import Field
from typing import Any, Dict, List, Union, Optional, Literal
from agent_c.models.completion.common import CommonCompletionParams


class GPTNonReasoningCompletionParams(CommonCompletionParams):
    """
    Vendor specific parameters for interacting with the GPT agent.
    """
    type: Literal['g_p_t_non_reasoning'] = Field('g_p_t_non_reasoning', description="The type of the completion params.")
    tool_choice: Optional[Union[str, dict]] = Field(None, description="The tool choice to use for the interaction, See OpenAI API docs for details")
    voice:  Optional[str] = None
    presence_penalty: Optional[float] = Field(0, description="Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.")
    seed: Optional[int] = Field(None, description="This feature is in Beta. If specified, Open AI will make a best effort to sample deterministically, such that repeated requests with the same seed and parameters should return the same result. Determinism is not guaranteed")
    service_tier: Optional[str] = Field(None, description="See Open AI Docs for details")
    stop: Optional[Union[str, List[str]]] = Field(None, description="Up to 4 sequences where the API will stop generating further tokens.")
    temperature: Optional[float] = Field(None, description="The temperature to use for the interaction, do not combine with top_p")
    root_message_role: str = Field('system', description="The role of the root message in the reasoning interaction, must be user or system")

    def __init__(self, **data: Any) -> None:
        if 'model_id' not in data:
            data['model_id'] = os.environ.get("GPT_INTERACTION_MODEL", "gpt-4o")

        super().__init__(**data)

    def as_completion_params(self, extra_excludes: Optional[List[str]] = None) -> Dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        if extra_excludes is None:
            extra_excludes = []

        opts = super().as_completion_params(extra_excludes + ['type', 'voice', 'root_message_role', 'max_tokens'])
        if self.max_tokens:
            opts['max_completion_tokens'] = self.max_tokens
        opts['model'] = self.model_id
        return opts

class GPTReasoningCompletionParams(CommonCompletionParams):
    type: Literal['g_p_t_reasoning'] = Field('g_p_t_reasoning', description="The type of the completion params.")
    tool_choice: Optional[Union[str, dict]] = Field(None, description="The tool choice to use for the interaction, See OpenAI API docs for details")
    voice:  Optional[str] = None
    presence_penalty: Optional[float] = Field(0, description="Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.")
    seed: Optional[int] = Field(None, description="This feature is in Beta. If specified, Open AI will make a best effort to sample deterministically, such that repeated requests with the same seed and parameters should return the same result. Determinism is not guaranteed")
    service_tier: Optional[str] = Field(None, description="See Open AI Docs for details")
    stop: Optional[Union[str, List[str]]] = Field(None, description="Up to 4 sequences where the API will stop generating further tokens.")
    reasoning_effort: Optional[str] = Field(None, description="The reasoning effort to use for the interaction, must be low, medium, or high")
    root_message_role: str = Field('developer', description="The role of the root message in the reasoning interaction, must be user or system")

    def __init__(self, **data: Any) -> None:
        if 'model_id' not in data:
            data['model_id'] = os.environ.get("GPT_REASONING_INTERACTION_MODEL", "o3")

        super().__init__(**data)

    def as_completion_params(self, extra_excludes: Optional[List[str]] = None) -> Dict[str, Any]:
        """
        Converts the model to a dictionary of completion parameters.
        """
        if extra_excludes is None:
            extra_excludes = []

        opts = super().as_completion_params(extra_excludes + ['type', 'voice', 'root_message_role', 'max_tokens'])
        opts['model'] = self.model_id
        if self.max_tokens:
            opts['max_completion_tokens'] = self.max_tokens
        return opts

GPTCompletionParams = Union[GPTReasoningCompletionParams, GPTNonReasoningCompletionParams]
