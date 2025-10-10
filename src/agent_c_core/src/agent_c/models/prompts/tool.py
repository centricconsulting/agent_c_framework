from pydantic import Field

from .base import BasePromptSection

class BaseToolSection(BasePromptSection):
    is_tool_section: bool = Field(True,
                                  description="If True, this section is for a tool")

class BaseToolImportSection(BaseToolSection):
    """
    This is a base class for tool sections that are used as imports in Jinja templates.
    It is used to define sections that should be included in other templates.
    """
    is_include: bool = Field(True, description="This section is intended to be used as an include / macro file.")