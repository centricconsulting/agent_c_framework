from typing import Optional, List, Any, Union, Literal
from pydantic import Field, ConfigDict

from agent_c.models.base import BaseModel
from agent_c.models.completion import CompletionParams


class AgentConfigurationV1(BaseModel):
    """Version 1 of the Agent Configuration"""
    version: Literal[1] = Field(1, description="Configuration version")
    name: str = Field(..., description="Name of the persona file")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    agent_description: Optional[str] = Field(None, description="A description of the agent's purpose and capabilities")
    tools: List[str] = Field(default_factory=list, description="List of enabled toolset names the agent can use")
    agent_params: Optional[CompletionParams] = Field(None, description="Parameters for the interaction with the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt, such as versioning or author information")
    persona: str = Field(..., description="Persona prompt of the persona defining the agent's behavior")


class AgentConfigurationV2(BaseModel):
    """Version 2 of the Agent Configuration - example with new fields"""
    version: Literal[2] = Field(2, description="Configuration version")
    name: str = Field(..., description="Name of the persona file")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    agent_description: Optional[str] = Field(None, description="A description of the agent's purpose and capabilities")
    tools: List[str] = Field(default_factory=list, description="List of enabled toolset names the agent can use")
    agent_params: Optional[CompletionParams] = Field(None, description="Parameters for the interaction with the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt of the persona defining the agent's behavior")

    category: List[str] = Field(default_factory=list, description="A list of categories this agent belongs to from most to least general" )


# Union type for all versions
AgentConfiguration = Union[AgentConfigurationV1, AgentConfigurationV2]

# Current version alias for convenience
CurrentAgentConfiguration = AgentConfigurationV2