from typing import Literal
from .base import StateMachineTemplate, StateConfig, TransitionConfig


class ToggleMachine(StateMachineTemplate):
    """
    A simple toggle state machine for controlling visibility of template blocks.

    Has two states: 'open' and 'closed' with transitions to toggle between them.
    Automatically configured with standard toggle transitions.
    """

    def __init__(
            self,
            name: str,
            default_state: Literal['open', 'closed'] = 'closed',
            **kwargs
    ):
        """
        Initialize a toggle machine.

        Args:
            name: Base name for the toggle (will become toggle_{name}_machine)
            default_state: Initial state ('open' or 'closed')
            **kwargs: Additional StateMachineTemplate arguments
        """

        # Define the two states
        states = [
            StateConfig(name='open'),
            StateConfig(name='closed')
        ]

        # Define transitions for toggling
        transitions = [
            # Explicit state transitions
            TransitionConfig(trigger='open', source='closed', dest='open'),
            TransitionConfig(trigger='close', source='open', dest='closed'),

            # Generic toggle that switches between states
            TransitionConfig(trigger='toggle', source='open', dest='closed'),
            TransitionConfig(trigger='toggle', source='closed', dest='open'),

            # Force states (can be called from any state)
            TransitionConfig(trigger='force_open', source=['open', 'closed'], dest='open'),
            TransitionConfig(trigger='force_close', source=['open', 'closed'], dest='closed'),
        ]

        # Ensure the name follows the toggle convention
        machine_name = name if name.startswith('toggle_') else f"toggle_{name}"

        super().__init__(
            name=machine_name,
            states=states,
            transitions=transitions,
            initial=default_state,
            **kwargs
        )
