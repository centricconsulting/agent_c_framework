from pydantic import BaseModel, Field
from typing import List, Optional, Any, Union, Dict
from transitions import Machine
from transitions.extensions import HierarchicalMachine

from agent_c.models.async_observable import AsyncObservableModel



class StateConfig(BaseModel):
    name: str
    on_enter: Optional[str] = None  # Method name or Jinja template
    on_exit: Optional[str] = None
    children: Optional[List['StateConfig']] = None


class TransitionConfig(BaseModel):
    trigger: str
    source: str | List[str]
    dest: str
    conditions: Optional[List[str]] = None
    unless: Optional[List[str]] = None
    before: Optional[str] = None
    after: Optional[str] = None


class StateMachineTemplate(AsyncObservableModel):
    name: str
    states: List[StateConfig] = Field(default_factory=list,
                                      description="List of states for the state machine")

    transitions: List[TransitionConfig] = Field(default_factory=list,
                                                description="List of transitions for the state machine")
    initial: str = Field(...,
                         description="Initial state of the state machine")

    context_vars: Dict[str,Any] = Field(default_factory=dict,
                                        description="Context variables for the state machine, used in Jinja templates")

    hierarchical: bool = Field(False,
                               description="If True, use HierarchicalMachine instead of Machine")

    def __init__(self, **data):
        self._machine: Optional[Union[HierarchicalMachine, Machine]] = None
        super().__init__(**data)


    def build(self, model_instance):
        """Build actual state machine from config"""
        MachineClass = HierarchicalMachine if self.hierarchical else Machine

        states = [s.model_dump(exclude_none=True) for s in self.states]
        transitions = [t.model_dump(exclude_none=True) for t in self.transitions]

        return MachineClass(
            model=model_instance,
            states=states,
            transitions=transitions,
            initial=self.initial,
            auto_transitions=False,
            queued=True
        )
