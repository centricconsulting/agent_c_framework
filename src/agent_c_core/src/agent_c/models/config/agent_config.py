from pydantic import Field
from typing import Optional, List, Any, Union, Literal

from typing_inspection.typing_objects import alias

from agent_c.config.model_config_loader import ModelConfigurationLoader
from agent_c.models.async_observable import AsyncObservableModel, AsyncObservableField
from agent_c.models.completion import CompletionParams


class BaseAgentConfiguration(AsyncObservableModel):
    """Base configuration with common fields across all agent configuration versions"""
    name: str = Field(..., description="Name of the agent")
    key: str = Field(..., description="Key for the agent configuration, used for identification")
    agent_description: Optional[str] = Field(None, description="A description of the agent's purpose and capabilities")
    tools: List[str] = Field(default_factory=list, description="List of enabled toolset names the agent can use")
    runtime_params: CompletionParams = AsyncObservableField(..., description="Parameters for the interaction with the agent")

    def __init__(self, **data) -> None:
        super().__init__(**data)
        model_id = self.runtime_params.model_id
        self._current_model_vendor: str = ModelConfigurationLoader.instance().model_id_map[model_id].vendor
        self.runtime_params.add_observer( self.on_runtime_params_model_id_change, "model_id")


    def on_runtime_params_model_id_change(self, old_value: str, new_value: str) -> None:
        """
        Callback when the model_id in runtime_params changes.
        This can be used to update other fields or perform actions based on the change.
        """
        if 'type' not in self.runtime_params:
            self._current_model_vendor: str = ModelConfigurationLoader.instance().model_id_map[new_value].vendor
            self.runtime_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)
            return

        if old_value == new_value:
            return

        new_value_vendor = ModelConfigurationLoader.instance().model_id_map[new_value].vendor
        if self._current_model_vendor == new_value_vendor:
            return

        self._current_model_vendor = new_value_vendor
        self.agent_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)



class AgentConfigurationV1(BaseAgentConfiguration):
    """Version 1 of the Agent Configuration"""
    version: Literal[1] = Field(1, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")

class AgentConfigurationV2(BaseAgentConfiguration):
    """Version 2 of the Agent Configuration - example with new fields"""
    version: Literal[2] = Field(2, description="Configuration version")
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    category: List[str] = Field(default_factory=list, description="A list of categories this agent belongs to from most to least general" )
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")

    def __init__(self, **data) -> None:
        if 'runtime_params' not in data:
            data['runtime_params'] = data['agent_params'] if 'agent_params' in data else {}
        if 'model_id' not in data['runtime_params']:
            data['runtime_params']['model_id'] = data['model_id']
        if 'type' not in data['runtime_params']:
            data['runtime_params'] = ModelConfigurationLoader.instance().default_params_for_model(data['agent_params']['model_id'])
        data.pop('agent_params', None)  # Remove old field if present
        super().__init__(**data)



# Union type for all versions
current_agent_configuration_version: int = 2
AgentConfiguration = Union[AgentConfigurationV1, AgentConfigurationV2]

# Current version alias for convenience
CurrentAgentConfiguration = AgentConfigurationV2