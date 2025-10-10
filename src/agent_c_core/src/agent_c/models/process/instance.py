from typing import List, Dict, Any, Optional

from pydantic import Field

from agent_c.models import AsyncObservableModel, ContextBag
from agent_c.models.process.step import StepStatus


class ProcessInstance(AsyncObservableModel):
    """Runtime instance of a process being executed"""
    process_name: str = Field(..., description="Name of the process template being executed")
    instance_id: str = Field(..., description="Unique identifier for this instance")

    # Current state
    current_step: str = Field(..., description="Current step being executed")
    status: StepStatus = Field(StepStatus.PENDING, description="Overall process status")

    # Step tracking
    completed_steps: List[str] = Field(default_factory=list, description="Steps that have been completed")
    failed_steps: List[str] = Field(default_factory=list, description="Steps that have failed")

    # Runtime context
    context: ContextBag = Field(default_factory=ContextBag, description="Runtime context variables")
    inputs: Dict[str, Any] = Field(default_factory=dict, description="Input values provided to process")
    outputs: Dict[str, Any] = Field(default_factory=dict, description="Output values produced by process")

    # Subprocess tracking
    subprocesses: Dict[str, 'ProcessInstance'] = Field(default_factory=dict, description="Active subprocess instances")
    parent_process: Optional[str] = Field(None, description="Parent process instance ID if this is a subprocess")
    return_step: Optional[str] = Field(None, description="Step to return to when subprocess completes")

    # Execution metadata
    started_at: Optional[str] = Field(None, description="ISO timestamp when process started")
    completed_at: Optional[str] = Field(None, description="ISO timestamp when process completed")
    error_message: Optional[str] = Field(None, description="Error message if process failed")

    # State machine instance
    state_machine: Optional[Any] = Field(None, exclude=True, description="Runtime state machine instance")

    def is_completed(self) -> bool:
        """Check if the process instance has completed"""
        return self.status in [StepStatus.COMPLETED, StepStatus.FAILED]

    def is_active(self) -> bool:
        """Check if the process instance is actively running"""
        return self.status == StepStatus.IN_PROGRESS
