from pydantic import Field

from .base import BasePromptSection

class BaseToolSection(BasePromptSection):
    is_tool_section: bool = Field(True,
                                  description="If True, this section is for a tool")
