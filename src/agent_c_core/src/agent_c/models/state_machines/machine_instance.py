from agent_c.models.state_machines.base import StateMachineTemplate


class StateMachineInstance:
    """Wrapper for a state machine instance with its model"""
    def __init__(self, name: str, template: StateMachineTemplate):
        self.name = name
        self.template = template

        # Create a state holder object for this machine
        self.model = type(f'{name.title()}StateModel', (), {
            'name': name,
            'context': {},  # Machine-specific context
            '_machine_ref': None
        })()

        # Build the actual state machine
        self.machine = template.build(self.model, use_hierarchical=template.hierarchical)
        self.model._machine_ref = self.machine

    def __getattr__(self, item):
        # Delegate attribute access to the model
        # This allows: orchestrator.machine_name.state, .trigger(), etc.
        return getattr(self.model, item)
