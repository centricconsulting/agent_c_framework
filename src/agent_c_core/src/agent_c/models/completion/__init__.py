from pydantic import Field
from typing import Annotated, Union, TypeAlias, Optional, Literal

from agent_c.models.completion.common import CommonCompletionParams
from agent_c.models.completion.claude import ClaudeNonReasoningParams, ClaudeReasoningParams
from agent_c.models.completion.gpt import GPTNonReasoningCompletionParams, GPTReasoningCompletionParams

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


__all__ = ["ReasoningEffort"]



