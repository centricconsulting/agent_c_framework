from pydantic import Field
from typing import Optional

from agent_c.models.context.base import BaseContext

class WorkspacePlanningToolsContext(BaseContext):
  current_plan_path: Optional[str] = Field(default=None,
                                           description="The path to the current plan in the workspace. This is used to track the current plan being worked on.")
