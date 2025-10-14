from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from datetime import datetime

class InteractionSummary(BaseModel):
    id: str
    start_time: datetime
    end_time: Optional[datetime] = None
    duration_seconds: Optional[float] = None
    event_count: int
    file_count: int

class InteractionDetail(InteractionSummary):
    files: List[str]
    event_types: Dict[str, int]  # Count of each event type
    metadata: Optional[Dict[str, Any]] = None
    user_id: Optional[str] = None
    has_thinking: bool = False
    tool_calls: List[str] = []