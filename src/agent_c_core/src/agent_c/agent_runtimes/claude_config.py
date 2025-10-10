import os
from typing import Optional
from pydantic import Field
from agent_c.models.config import BaseRuntimeConfig
from agent_c.models.completion import ClaudeAuthInfo
from agent_c.models.completion.bedrock_auth_info import BedrockAuthInfo

class BaseClaudeConfig(BaseRuntimeConfig):
    default_non_reasoning_model: str = Field(default_factory=lambda: os.environ.get("CLAUDE_INTERACTION_MODEL", "claude-sonnet-4-latest"),
                                             description="Default model for non-reasoning interactions with Claude.")

    default_reasoning_model: str = Field(default_factory=lambda: os.environ.get("CLAUDE_INTERACTION_REASON_MODEL", "claude-sonnet-4-latest-reasoning"),
                                         description="Default model for reasoning interactions with Claude.")


class ClaudeConfig(BaseClaudeConfig):
    auth: Optional[ClaudeAuthInfo] = Field(default_factory=lambda: ClaudeAuthInfo(api_key=os.environ.get("ANTHROPIC_API_KEY")),
                                            description="Authentication information for the Claude API, including API key and auth token.")

class ClaudeUserConfig(BaseClaudeConfig):
    auth: Optional[ClaudeAuthInfo] = Field(default_factory=lambda: ClaudeAuthInfo(),
                                           description="Authentication information for the Claude API, including API key and auth token.")

class BedrockClaudeConfig(BaseClaudeConfig):
    auth: Optional[BedrockAuthInfo] = Field(default_factory=lambda: BedrockAuthInfo(),
                                            description="Authentication information for the Claude API on AWS Bedrock, including region and credentials.")

