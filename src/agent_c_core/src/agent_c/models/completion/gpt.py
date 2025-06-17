import os

from openai import Omit
from openai.types.chat.completion_create_params import WebSearchOptions
from pydantic import Field
from typing import Any, Dict, List, Union, Optional, Literal, Mapping

from agent_c.models.base import BaseModel
from agent_c.models.completion.common import CommonCompletionParams

class GPTResponseFormat(BaseModel):
    """
    Represents the response format for GPT completions.
    """
    type: Union[Literal['json_object'], Literal['json_schema']] = Field(None,
                                                                        description="The type of the JSON output requested")
    json_schema: Optional[str] = Field(None,
                                       description="The schema for the JSON response, if the type is json_schema")

    def __init__(self, **data: Any) -> None:
        if 'type' not in data:
            data['type'] = 'json_schema' if 'json_schema' in data else 'json_object'
        super().__init__(**data)

class GPTCommonParams(CommonCompletionParams):
    """
    Common parameters for GPT-based completions.
    """
    frequency_penalty: Optional[float] = Field(None,
                                               description="Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.")
    tool_choice: Optional[Union[str, dict]] = Field(None,
                                                    description="The tool choice to use for the interaction, See OpenAI API docs for details")
    voice: Optional[str] = Field(None,
                                 description="The voice to use for the interaction, if applicable")
    presence_penalty: Optional[float] = Field(None,
                                              description="Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.")
    seed: Optional[int] = Field(None,
                                description="This feature is in Beta. If specified, Open AI will make a best effort to sample deterministically, such that repeated requests with the same seed and parameters should return the same result. Determinism is not guaranteed")

    stop: Optional[Union[str, List[str]]] = Field(None,
                                                  description="Up to 4 sequences where the API will stop generating further tokens.")
    modalities: Optional[List[Literal["text", "audio"]]] = Field(None,
                                                                 description="The modalities to use for the interaction, must be text or audio")
    service_tier: Optional[Literal["auto", "default", "flex"]] = Field(None,
                                                                       description="See Open AI Docs for details")
    response_format: Optional[GPTResponseFormat] = Field(None,
                                                         description="The response format to use for the interaction, must be json_object or json_schema")
    store: Optional[bool] = Field(None,
                                  description="Whether or not to store the output of this chat completion request for use in Open AI model distillation or evals products. Defaults to False. ")
    web_search_options: Optional[WebSearchOptions] = Field(None,
                                                           description="The web search options to use for the interaction, see OpenAI API docs for details")
    extra_headers: Optional[Mapping[str, Union[str, Omit]]] = Field(None,
                                                                   description="Extra headers to include in the request, see OpenAI API docs for details")

class GPTNonReasoningCompletionParams(GPTCommonParams):
    """
    Vendor specific parameters for interacting with the GPT agent.
    """
    type: Literal['gpt_non_reasoning'] = Field('gpt_non_reasoning', description="The type of the completion params.")
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

class GPTReasoningCompletionParams(GPTCommonParams):
    type: Literal['gpt_reasoning'] = Field('gpt_reasoning', description="The type of the completion params.")
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
