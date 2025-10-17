from pydantic import BaseModel, Field, model_validator
from typing import Optional, List


class ToolUpdateRequest(BaseModel):
    ui_session_id: str
    tools: List[str]