from typing import Optional, List, Dict, Any

from pydantic import Field, field_validator, model_validator

from agent_c.models import AsyncObservableModel
from agent_c.models.process.step import ProcessStep, StepType
from agent_c.util import to_snake_case


class ProcessDefinition(AsyncObservableModel):
    """Template for defining a process with steps and state machine integration"""
    name: str = Field(..., description="Name of the process template")
    description: Optional[str] = Field(None, description="Description of what this process accomplishes")
    version: str = Field("1.0.0", description="Version of this process template")

    # Process structure
    steps: List[ProcessStep] = Field(..., description="List of steps in this process")
    initial_step: str = Field(..., description="Name of the first step to execute")

    # State machine integration
    auto_generate_state_machine: bool = Field(True, description="Whether to auto-generate state machine from steps")
    state_machine_template: Optional['StateMachineTemplate'] = Field(None, description="Custom state machine template")

    # Context and variables
    context_vars: Dict[str, Any] = Field(default_factory=dict, description="Context variables available to all steps")
    input_schema: Dict[str, Any] = Field(default_factory=dict, description="JSON schema for process inputs")
    output_schema: Dict[str, Any] = Field(default_factory=dict, description="JSON schema for process outputs")

    # Execution control
    max_iterations: Optional[int] = Field(None, description="Maximum number of iterations before process times out")
    timeout_seconds: Optional[int] = Field(None, description="Maximum execution time in seconds")

    # Rendering templates
    render_templates: Dict[str, str] = Field(default_factory=dict, description="Jinja templates for rendering process info")

    # Metadata
    tags: List[str] = Field(default_factory=list, description="Tags for categorizing this process")
    metadata: Dict[str, Any] = Field(default_factory=dict, description="Additional metadata")

    # Auto-registration support
    auto_register: bool = Field(True, description="Whether this template should be auto-registered")

    @field_validator('name', mode='after')
    @classmethod
    def normalize_name(cls, value: str) -> str:
        """Normalize the name to snake_case for consistency"""
        normalized = to_snake_case(value)
        if not normalized.endswith('_process'):
            normalized += '_process'
        return normalized

    @field_validator('initial_step')
    def validate_initial_step(cls, v: str, info) -> str:
        """Ensure initial_step references a valid step"""
        # Note: This validation happens after steps are set in practice
        return v

    @model_validator(mode='after')
    def validate_process_structure(self):
        """Validate the overall process structure"""
        if not self.steps:
            raise ValueError("Process must have at least one step")

        step_names = {step.name for step in self.steps}

        if self.initial_step not in step_names:
            raise ValueError(f"initial_step '{self.initial_step}' not found in steps")

        # Validate step references
        for step in self.steps:
            if step.next_step and step.next_step not in step_names:
                raise ValueError(f"Step '{step.name}' references unknown next_step '{step.next_step}'")

            # Validate branch targets for decision steps
            if step.step_type == StepType.DECISION:
                for condition, target in step.branches.items():
                    if target not in step_names:
                        raise ValueError(f"Decision step '{step.name}' branch '{condition}' references unknown step '{target}'")

        return self

    def get_step(self, step_name: str) -> Optional[ProcessStep]:
        """Get a step by name"""
        for step in self.steps:
            if step.name == step_name:
                return step
        return None

    def generate_state_machine_template(self) -> 'StateMachineTemplate':
        """Generate a state machine template from the process steps"""
        from agent_c.models.state_machines.base import StateConfig, TransitionConfig, StateMachineTemplate

        states = []
        transitions = []

        # Create states from steps
        for step in self.steps:
            state_config = StateConfig(
                name=step.name,
                on_enter=step.on_enter,
                on_exit=step.on_exit
            )
            states.append(state_config)

        # Create transitions based on step flow
        for step in self.steps:
            if step.step_type == StepType.DECISION:
                # Create transitions for each branch
                for condition, target in step.branches.items():
                    transition = TransitionConfig(
                        trigger=f"proceed_from_{step.name}",
                        source=step.name,
                        dest=target,
                        conditions=[condition]
                    )
                    transitions.append(transition)
            elif step.next_step:
                # Create simple transition to next step
                transition = TransitionConfig(
                    trigger=f"proceed_from_{step.name}",
                    source=step.name,
                    dest=step.next_step
                )
                transitions.append(transition)

        return StateMachineTemplate(
            name=f"{self.name}_state_machine",
            states=states,
            transitions=transitions,
            initial=self.initial_step,
            context_vars=self.context_vars
        )

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses if auto_register is True"""
        super().__init_subclass__(**kwargs)
        if getattr(cls, 'auto_register', True):
            from agent_c.util.registries.process_registry import ProcessRegistry
            ProcessRegistry.register(cls)