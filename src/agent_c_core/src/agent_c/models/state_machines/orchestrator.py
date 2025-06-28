from typing import Optional, List, Dict

from pydantic import Field

from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.state_machines.base import StateMachineTemplate
from agent_c.models.state_machines.machine_instance import StateMachineInstance

class StateMachineOrchestrator(AsyncObservableModel):
    """Orchestrator for managing multiple state machines"""

    templates: Dict[str, StateMachineTemplate] = Field(default_factory=dict,
                                                       description="State machine templates by name")

    instances: Dict[str, StateMachineInstance] = Field(default_factory=dict,
                                                       description="State machine instances by name",
                                                       exclude=True)

    def add_machine(self, name: str, template: StateMachineTemplate) -> StateMachineInstance:
        """Add a new state machine from a template"""
        # Store the template
        self.templates[name] = template

        # Create the instance
        instance = StateMachineInstance(name, template)
        self.instances[name] = instance

        return instance

    def remove_machine(self, name: str):
        """Remove a state machine"""
        if name in self.templates:
            del self.templates[name]
        if name in self.instances:
            del self.instances[name]

    def __getattr__(self, name: str):
        """Allow access to machines by name: orchestrator.machine_name"""
        instances = object.__getattribute__(self, 'instances')
        if name in instances:
            return instances[name]
        raise AttributeError(f"No state machine named '{name}'")

    def get_machine(self, name: str) -> Optional[StateMachineInstance]:
        """Explicitly get a machine instance by name"""
        return self.instances.get(name)

    def list_machines(self) -> List[str]:
        """List all machine names"""
        return list(self.instances.keys())