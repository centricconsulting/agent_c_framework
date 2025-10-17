from typing import Dict, Any, List

from agent_c_tools.tools.workspace.base import BaseWorkspace
from pydantic import Field

from agent_c.models import BaseModel
from agent_c.toolsets import ToolChest, ToolCache
from agent_c.agents import ClaudeChatAgent, BaseAgent
from agent_c.agents.claude import ClaudeBedrockChatAgent
from agent_c.agents.gpt import AzureGPTChatAgent, GPTChatAgent
from agent_c.models.agent_config import CurrentAgentConfiguration



class UserRuntimeCacheEntry(BaseModel):
    __vendor_agent_map = {
        "azure_openai": AzureGPTChatAgent,
        "openai": GPTChatAgent,
        "claude": ClaudeChatAgent,
        "bedrock": ClaudeBedrockChatAgent
    }

    user_id: str = Field(..., description="The user ID associated with the cache entry")
    tool_chest: ToolChest = Field(..., description="The tool chest instance for the user")
    tool_cache: ToolCache = Field(..., description="The tool cache instance for the user")
    model_configs: Dict[str, Any] = Field(..., description="The model configurations available to the user")
    runtime_cache: Dict[str, BaseAgent] = Field(default_factory=dict, description="Cache of runtime agents by model ID")
    workspaces: List[BaseWorkspace] = Field(default_factory=list, description="The list of workspaces associated with the user")

    def runtime_for_agent(self, agent_config: CurrentAgentConfiguration):
        if agent_config.model_id in self.runtime_cache:
            return self.runtime_cache[agent_config.model_id]
        else:
            self.runtime_cache[agent_config.model_id] = self._runtime_for_agent(agent_config)
            return self.runtime_cache[agent_config.model_id]


    def _runtime_for_agent(self, agent_config: CurrentAgentConfiguration) -> BaseAgent:
        model_config = self.model_configs[agent_config.model_id]
        runtime_cls = self.__vendor_agent_map[model_config["vendor"]]

        auth_info = agent_config.agent_params.auth.model_dump() if agent_config.agent_params.auth is not None else {}
        client = runtime_cls.client(**auth_info)
        return runtime_cls(model_name=model_config["id"], client=client)

