from typing import Optional, List, Dict, Any

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
        self.templates[name] = template

        instance = StateMachineInstance(name, template)
        self.instances[name] = instance

        return instance

    def remove_machine(self, name: str):
        """Remove a state machine"""
        if name in self.templates:
            del self.templates[name]
        if name in self.instances:
            del self.instances[name]

    def __getattr__(self, name: str) -> Any:
        if name.startswith('_'):
            raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")

        instances = object.__getattribute__(self, 'instances')
        if name in instances:
            return instances[name]

        raise AttributeError(f"{type(self).__name__!r} object has no attribute or state machine {name!r}")

    def get_machine(self, name: str) -> Optional[StateMachineInstance]:
        """Explicitly get a machine instance by name"""
        return self.instances.get(name)

    def list_machines(self) -> List[str]:
        """List all machine names"""
        return list(self.instances.keys())

    def has_machine(self, name: str) -> bool:
        """Check if a machine exists by name"""
        return name in self.instances