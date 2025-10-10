from typing import Optional, List, Dict, Any
from pydantic import BaseModel, Field, field_validator, model_validator
from enum import Enum

from agent_c.models import ContextBag
from agent_c.util.string import to_snake_case


class StepType(str, Enum):
    """Types of process steps"""
    ACTION = "action"  # Regular step that performs an action
    SUBPROCESS = "subprocess"  # Step that calls another process
    DECISION = "decision"  # Step that branches based on conditions
    PARALLEL = "parallel"  # Step that can run multiple sub-steps in parallel
    WAIT = "wait"  # Step that waits for external input/condition


class StepStatus(str, Enum):
    """Status of a process step"""
    PENDING = "pending"
    IN_PROGRESS = "in_progress"
    COMPLETED = "completed"
    FAILED = "failed"
    SKIPPED = "skipped"
    BLOCKED = "blocked"


class ProcessStep(BaseModel):
    """Individual step within a process"""
    name: str = Field(..., description="Unique name for this step within the process")
    description: Optional[str] = Field(None, description="Human-readable description of what this step does")
    step_type: StepType = Field(StepType.ACTION, description="Type of step")
    instructions: Optional[str] = Field(None, description="Instructions or details for this step (can be Jinja template)")

    # Subprocess handling
    subprocess_name: Optional[str] = Field(None, description="Name of subprocess to execute (for subprocess type)")
    subprocess_context: ContextBag = Field(default_factory=ContextBag, description="Context variables to pass to subprocess")

    # Conditional execution
    conditions: List[str] = Field(default_factory=list, description="Conditions that must be met to execute this step")
    unless_conditions: List[str] = Field(default_factory=list, description="Conditions that prevent execution of this step")

    # State machine integration
    on_enter: Optional[str] = Field(None, description="Method name or Jinja template to execute when entering step")
    on_exit: Optional[str] = Field(None, description="Method name or Jinja template to execute when exiting step")

    # Parallel execution (for parallel type)
    parallel_steps: List['ProcessStep'] = Field(default_factory=list, description="Sub-steps to execute in parallel")

    # Decision branching (for decision type)
    branches: Dict[str, str] = Field(default_factory=dict, description="Condition -> next_step_name mapping for decision steps")

    # Metadata and state
    metadata: Dict[str, Any] = Field(default_factory=dict, description="Additional metadata for this step")
    status: StepStatus = Field(StepStatus.PENDING, description="Current status of this step")
    error_message: Optional[str] = Field(None, description="Error message if step failed")

    # Navigation
    next_step: Optional[str] = Field(None, description="Default next step (can be overridden by conditions/branches)")

    @field_validator('name')
    @classmethod
    def validate_name(cls, v: str) -> str:
        """Ensure step name is valid identifier"""
        return to_snake_case(v)

    @model_validator(mode='after')
    def validate_step_config(self):
        """Validate step configuration based on type"""
        if self.step_type == StepType.SUBPROCESS and not self.subprocess_name:
            raise ValueError("subprocess_name is required for subprocess type steps")

        if self.step_type == StepType.PARALLEL and not self.parallel_steps:
            raise ValueError("parallel_steps is required for parallel type steps")

        if self.step_type == StepType.DECISION and not self.branches:
            raise ValueError("branches is required for decision type steps")

        return self








