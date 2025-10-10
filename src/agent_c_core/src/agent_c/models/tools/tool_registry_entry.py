from typing import Dict, Any

from pydantic import Field
from agent_c.models.base import BaseModel

class ToolRegistryEntry(BaseModel):
    tool_name: str = Field(...,
                           description="A human readable name for the tool")
    toolset_name: str = Field(...,
                              description="The name of the toolset this tool belongs to")
    is_prefixed: bool = Field(False,
                              description="If true, the tool name will be prefixed with the toolset short name")
    json_schema: Dict[str, Any] = Field(default=None,
                                        description="JSON schema for the tool's input parameters, if applicable")
    agent_instructions: str = Field(...,
                                    description="The help text shown when this tool is equipped by an agent")

    multi_user: bool = Field(False,
                             description="If true, this tool is designed be safely used by multiple users simultaneously")

