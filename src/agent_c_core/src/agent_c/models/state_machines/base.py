from pydantic import BaseModel, Field
from typing import List, Optional, Any, Union, Dict, Literal, Callable

from pydantic.main import IncEx
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

    saved_state: Optional[str] = Field(None,
                                       description="Current state of the state machine, used for persistence across sessions")

    machine: Optional[Union['HierarchicalMachine', 'Machine']] = Field(None,
                                                                       description="The actual state machine instance built from this template. "
                                                                                  "This is set after the build method is called.",
                                                                       exclude=True)

    def __init__(self, **data):
        self._machine: Optional[Union[HierarchicalMachine, Machine]] = None
        super().__init__(**data)

    def model_dump(
        self,
        *,
        mode: Literal['json', 'python'] | str = 'python',
        include: IncEx | None = None,
        exclude: IncEx | None = None,
        context: Any | None = None,
        by_alias: bool | None = None,
        exclude_unset: bool = False,
        exclude_defaults: bool = False,
        exclude_none: bool = False,
        round_trip: bool = False,
        warnings: bool | Literal['none', 'warn', 'error'] = True,
        fallback: Callable[[Any], Any] | None = None,
        serialize_as_any: bool = False,
    ) -> dict[str, Any]:

        # Ensure the current state is saved before dumping the model
        self.saved_state = self._machine.current_state if self._machine else None

        return super().model_dump(
            mode=mode,
            include=include,
            exclude=exclude,
            context=context,
            by_alias=by_alias,
            exclude_unset=exclude_unset,
            exclude_defaults=exclude_defaults,
            exclude_none=exclude_none,
            round_trip=round_trip,
            warnings=warnings,
            fallback=fallback,
            serialize_as_any=serialize_as_any
        )

    def build(self, model_instance):
        """Build actual state machine from config"""
        MachineClass = HierarchicalMachine if self.hierarchical else Machine

        states = [s.model_dump(exclude_none=True) for s in self.states]
        transitions = [t.model_dump(exclude_none=True) for t in self.transitions]
        initial = self.saved_state if self.saved_state else self.initial
        return MachineClass(
            model=model_instance,
            states=states,
            transitions=transitions,
            initial=initial,
            auto_transitions=False,
            queued=True
        )
