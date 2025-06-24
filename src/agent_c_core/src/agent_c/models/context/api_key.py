from typing import Optional, Literal, Any

from pydantic import Field


from agent_c.models.context.base import BaseContext


class APIKeyContext(BaseContext):
    """Base model for contexts that require an API key for interaction."""
    api_key: Optional[str] = Field(None, description="The API key to use for the interaction")


    def model_dump(
        self,
        *,
        mode: Literal['json', 'python'] | str = 'python',
        include = None,
        exclude = None,
        context: Any | None = None,
        by_alias: bool = False,
        exclude_unset: bool = False,
        exclude_defaults: bool = False,
        exclude_none: bool = False,
        round_trip: bool = False,
        warnings: bool | Literal['none', 'warn', 'error'] = True,
        serialize_as_any: bool = False,
    ) -> dict[str, Any]:
        exclude_none = False
        exclude_unset = False
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
              serialize_as_any=serialize_as_any
         )
