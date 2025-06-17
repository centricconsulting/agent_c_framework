from typing import Dict, List, Literal, Optional, Any, Union
from datetime import datetime
from uuid import uuid4
from pydantic import BaseModel, Field, field_serializer

from agent_c.util import MnemonicSlugs

# Task Priority type
PriorityType = Literal["low", "medium", "high"]


class TaskModel(BaseModel):
    """Model for a task or subtask within a plan."""
    id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(2))
    title: str
    description: str = ""
    priority: PriorityType = "medium"
    completed: bool = False
    parent_id: Optional[str] = None
    context: str = ""
    child_tasks: List[str] = Field(default_factory=list)
    sequence: Optional[int] = None  # For controlling display order
    created_at: datetime = Field(default_factory=datetime.now)
    updated_at: datetime = Field(default_factory=datetime.now)
    completion_report: Optional[str] = None  # Report on task completion
    completion_signoff_by: Optional[str] = None
    requires_completion_signoff: Optional[str] = Field(default="true", description="Whether task requires signoff.  Maye be one of 'true', false' of 'human_required'")

    @field_serializer('created_at', 'updated_at')
    def serialize_datetime(self, dt: datetime) -> str:
        return dt.isoformat()


class LessonLearnedModel(BaseModel):
    """Model for a lesson learned within a plan."""
    id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(1))
    lesson: str
    learned_task_id: str
    created_at: datetime = Field(default_factory=datetime.now)
    
    @field_serializer('created_at')
    def serialize_datetime(self, dt: datetime) -> str:
        return dt.isoformat()


class PlanModel(BaseModel):
    """Model for a plan that contains tasks and lessons learned."""
    id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3))
    title: str
    description: str = ""
    tasks: Dict[str, TaskModel] = Field(default_factory=dict)
    lessons_learned: List[LessonLearnedModel] = Field(default_factory=list)
    created_at: datetime = Field(default_factory=datetime.now)
    updated_at: datetime = Field(default_factory=datetime.now)
    
    @field_serializer('created_at', 'updated_at')
    def serialize_datetime(self, dt: datetime) -> str:
        return dt.isoformat()