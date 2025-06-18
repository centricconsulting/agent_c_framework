from pydantic import Field
from typing import Optional, List, Any, Union, Literal

from agent_c.models.base import BaseModel
from agent_c.models.completion import CompletionParams
from agent_c.util import to_snake_case


class BaseAgentConfiguration(BaseModel):
    """Base configuration with common fields across all agent configuration versions"""
    name: str = Field(..., description="Name of the agent")
    key: str = Field(..., description="Key for the agent configuration, used for identification")
    agent_description: Optional[str] = Field(None, description="A description of the agent's purpose and capabilities")
    tools: List[str] = Field(default_factory=list, description="List of enabled toolset names the agent can use")
    agent_params: Optional[CompletionParams] = Field(None, description="Parameters for the interaction with the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")

    def __init__(self, **data) -> None:
        if 'key' not in data:
            data['key'] = to_snake_case(data['name'])

        super().__init__(**data)


class AgentConfigurationV1(BaseAgentConfiguration):
    """Version 1 of the Agent Configuration"""
    version: Literal[1] = Field(1, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")


class AgentConfigurationV2(BaseAgentConfiguration):
    """Version 2 of the Agent Configuration - example with new fields"""
    version: Literal[2] = Field(2, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    category: List[str] = Field(default_factory=list, description="A list of categories this agent belongs to from most to least general" )

    def __init__(self, **data) -> None:
        if 'model_name' in data['agent_params']:
            data['agent_params']['model_id'] = data['agent_params'].pop('model_name')

        super().__init__(**data)



# Union type for all versions
current_agent_configuration_version: int = 2
AgentConfiguration = Union[AgentConfigurationV1, AgentConfigurationV2]

# Current version alias for convenience
CurrentAgentConfiguration = AgentConfigurationV2