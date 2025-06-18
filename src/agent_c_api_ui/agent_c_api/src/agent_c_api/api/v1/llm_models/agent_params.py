from pydantic import BaseModel, Field, model_validator, ConfigDict
from typing import Optional


class AgentCommonParams(BaseModel):
    model_config = ConfigDict(protected_namespaces=())
    agent_key: Optional[str] = Field("default", description="Key of the agent config to use")
    temperature: Optional[float] = Field(None, description="Temperature for chat models")
    extended_thinking: Optional[bool] = Field(None, description="Extended thinking for chat models; defaults to False if not provided")
    reasoning_effort: Optional[str] = Field(
        None, description="Reasoning effort for OpenAI models; must be 'low', 'medium', or 'high'"
    )
    budget_tokens: Optional[int] = Field(None, description="Budget tokens (used by some Claude models)")

    def __init__(self, **data):
        if 'agent_key' not in data and "persona_name" in data:
            data['agent_key'] = data.pop('persona_name', 'default')
        if 'custom_prompt' in data:
            data.pop('custom_prompt', None)

        super().__init__(**data)


class AgentUpdateParams(AgentCommonParams):
    ui_session_id: str = Field(..., description="Session ID of the agent to update")

class AgentInitializationParams(AgentCommonParams):
    model_name: Optional[str] = Field(..., description="The model name to use")
    backend: Optional[str] = Field("openai", description="Backend provider (e.g., 'openai', 'claude')")
    max_tokens: Optional[int] = Field(None, description="Maximum tokens for the model output")
    ui_session_id: Optional[str] = Field(None, description="Existing UI session ID - this enables us to transfer chat sessions. If None or not passed, a new session will be created.")

    @model_validator(mode="before")
    def validate_and_set_defaults(cls, values):
        model_name = values.get('model_name')
        if not model_name:
            raise ValueError("model_name is required")

        # For chat models like 'gpt-4o', 'gpt-4o-audio-preview', and 'claude-3-5-sonnet-latest'
        if model_name in ['gpt-4o', 'gpt-4o-audio-preview', 'claude-3-5-sonnet-latest']:
            if model_name == 'claude-3-5-sonnet-latest' and (values.get('max_tokens') is None):
                values['max_tokens'] = 8192

        # For OpenAI reasoning models: 'o1' and 'o3-mini'
        if model_name in ['o1', '01-mini', 'o3-mini']:
            if values.get('reasoning_effort') not in ['low', 'medium', 'high']:
                values['reasoning_effort'] = 'medium'

        # For Claude reasoning model: 'claude-3-7-sonnet-latest'
        if 'sonnet' in model_name:
            if values.get('budget_tokens') is not None:
                # we'll use the passed in budget_tokens, and determine what max_tokens to set
                if values.get('max_tokens') is None:
                    values['max_tokens'] = 64000
            else:
                values['budget_tokens'] = 0
                values['max_tokens'] = 8192

        return values

    def to_agent_kwargs(self) -> dict:
        """
        Returns a dictionary with only the parameters relevant for the selected model,
        mapped to the names expected by the underlying agent.

        The mapping will pass the kwarg name as the 2nd argument and the value for the kwarg from the params first argument value.
        Meaning, for o1, if max_tokens=16000, the mapping will be {'max_completion_tokens': 16000}
        """
        allowed_params_mapping = {
            'gpt-4o': {'temperature': 'temperature', 'max_tokens': 'max_tokens'},
            'gpt-4o-audio-preview': {'temperature': 'temperature', 'max_tokens': 'max_tokens'},
            'claude-3-5-sonnet-latest': {'temperature': 'temperature', 'max_tokens': 'max_tokens'},
            'o1': {'reasoning_effort': 'reasoning_effort', 'max_tokens': 'max_completion_tokens'},
            'o1-mini': {'reasoning_effort': 'reasoning_effort', 'max_tokens': 'max_completion_tokens'},
            'o3-mini': {'reasoning_effort': 'reasoning_effort', 'max_tokens': 'max_completion_tokens'},
            'claude-3-7-sonnet-latest': {'budget_tokens': 'budget_tokens', 'max_tokens': 'max_tokens'},
        }
        mapping = allowed_params_mapping.get(self.model_name, {})
        filtered = {}
        for pydantic_field, agent_field in mapping.items():
            value = getattr(self, pydantic_field, None)
            if value is not None:
                filtered[agent_field] = value
        return filtered

    def to_additional_params(self) -> dict:
        """
        Returns additional parameters based on the create_session logic.
        This includes:
          - 'custom_prompt': if a custom prompt is provided.
          - 'agent_key': always included, defaulting to 'default' if not provided.
        """
        additional = {}
        # Always include agent_key, defaulting to 'default'
        additional['agent_key'] = self.agent_key if self.agent_key else 'default'
        additional['agent_key'] = self.agent_key if self.agent_key else 'default'
        return additional
