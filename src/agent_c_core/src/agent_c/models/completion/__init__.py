from pydantic import Field
from typing import Annotated, Union, TypeAlias, Optional, Literal

from agent_c.models.completion.common import CommonCompletionParams
from agent_c.models.completion.claude import ClaudeNonReasoningParams, ClaudeReasoningParams, ClaudeCommonParams
from agent_c.models.completion.gpt import GPTNonReasoningCompletionParams, GPTReasoningCompletionParams, \
                                          GPTCommonParams
from agent_c.models.completion.auth import OpenAiAuthInfo, AzureAuthInfo, BedrockAuthInfo, GeminiAuthInfo, \
                                           AuthInfo, APIkeyAuthInfo, ClaudeAuthInfo, OpenAiAuthInfoFull

ReasoningEffort: TypeAlias = Optional[Literal["low", "medium", "high"]]

CompletionParams = Annotated[
    Union[
        ClaudeNonReasoningParams,
        ClaudeReasoningParams,
        GPTNonReasoningCompletionParams,
        GPTReasoningCompletionParams,
    ],
    Field(discriminator='type')
]


__all__ = [
    "CommonCompletionParams",
    "ClaudeNonReasoningParams",
    "ClaudeReasoningParams",
    "GPTNonReasoningCompletionParams",
    "GPTReasoningCompletionParams",
    "ClaudeCommonParams",
    "GPTCommonParams",
    "CompletionParams",
    "ReasoningEffort",
    "OpenAiAuthInfo",
    "AzureAuthInfo",
    "BedrockAuthInfo",
    "GeminiAuthInfo",
    "AuthInfo",
    "APIkeyAuthInfo",
    "ClaudeAuthInfo",
    "OpenAiAuthInfoFull"
]
