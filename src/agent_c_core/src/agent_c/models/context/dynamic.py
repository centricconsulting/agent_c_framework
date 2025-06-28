from typing import Any, Dict, Literal
from pydantic import model_validator, Field
from pydantic.main import IncEx

from agent_c.models.context import BaseContext


class BaseDynamicContext(BaseContext):
    dynamic_fields: Dict[str, Any] = Field(default_factory=dict,
                                           description="Dynamic fields that can be added to the context at runtime. "
                                                       "These fields are not defined in the model schema and can be used for flexible data storage.",
                                           exclude=True)

    def __init__(self, **data: Any) -> None:
        object.__setattr__(self, 'dynamic_fields', data)
        if isinstance(data, dict):
            super().__init__(context_type=data['context_type'])

    @model_validator(mode='before')
    @classmethod
    def ensure_context_type(cls, data: Any):
        if isinstance(data, dict):
            if 'context_type' not in data:
                raise ValueError("BaseDynamicContext must have a 'context_type' field")

        return data

    def __getattr__(self, name: str) -> Any:
        try:
            return self.dynamic_fields[name]
        except KeyError:
            raise AttributeError(f"{type(self).__name__!r} object has no attribute or key {name!r}")

    def model_dump(
        self,
        *,
        mode: Literal['json', 'python'] | str = 'python',
        include: IncEx | None = None,
        exclude: IncEx | None = None,
        context: Any | None = None,
        by_alias: bool = False,
        exclude_unset: bool = False,
        exclude_defaults: bool = False,
        exclude_none: bool = False,
        round_trip: bool = False,
        warnings: bool | Literal['none', 'warn', 'error'] = True,
        serialize_as_any: bool = False,
    ) -> dict[str, Any]:
        """
        Override model_dump to include dynamic fields in the output.
        """
        data = super().model_dump(
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
            serialize_as_any=serialize_as_any
        )
        data.update(self.dynamic_fields)
        return data
