import logging

from contextlib import contextmanager
from typing import Any, Callable, Generator

from pydantic import Field, ConfigDict
from observable import Observable

from agent_c_core.agent_c.models.base import BaseModel


def ObservableField(
        *args: Any,
        observable: bool = True,
        **kwargs: Any
) -> Any:
    """
    Helper function to create a field with optional observability.

    Args:
        *args: Positional arguments forwarded to the `Field` function.
        observable (bool): Flag to denote if the field should be observable. Defaults to True.
        **kwargs: Additional keyword arguments forwarded to the `Field` function.

    Returns:
        A Pydantic `Field` with optional observability.
    """
    json_schema_extra = kwargs.pop('json_schema_extra', {})

    if observable:
        json_schema_extra['observable'] = True

    return Field(*args, json_schema_extra=json_schema_extra, **kwargs)


class ObservableModel(BaseModel):
    """
    Base class for Pydantic models with observable fields.

    Attributes:
        _batch_active (bool): Internal flag used to determine if a batch operation is active.
        _observable (Observable): Observable object for handling event-based notifications.
    """
    model_config = ConfigDict(arbitrary_types_allowed=True)

    def __init__(self, **data: Any) -> None:
        """
        Initialize the ObservableModel with provided data.

        Args:
            **data: Keyword arguments used to initialize the model fields.
        """
        super().__init__(**data)
        self._batch_active: bool = False
        self._observable: Observable = Observable()

    def __is_observable_field(self, field_name: str) -> bool:
        """
        Check if a given field is marked as observable.

        Args:
            field_name (str): The name of the field to check.

        Returns:
            bool: True if the field is observable, False otherwise.
        """
        if field_name not in self.model_fields:
            return False

        if self.model_fields[field_name].json_schema_extra is None:
            return False

        return self.model_fields[field_name].json_schema_extra.get('observable', False)

    def add_observer(
            self,
            callback: Callable[[Any], None],
            field_name: str = 'model'
    ) -> None:
        """
        Add an observer callback for a specific field.

        Args:
            callback (Callable[[Any], None]): The callback function to be called when the field changes.
            field_name (str): The name of the field to observe. Defaults to 'model'.

        Raises:
            ValueError: If the specified field name is not observable.
        """
        if field_name != 'model' and not self.__is_observable_field(field_name):
            raise ValueError(f"Field '{field_name}' is not an observable field.")

        self._observable.on(f"{field_name}_changed", callback)

    def __setattr__(self, name: str, value: Any) -> None:
        """
        Override default attribute setter to trigger observable events when field values change.

        Args:
            name (str): The name of the attribute being set.
            value (Any): The new value of the attribute.
        """
        old_value = getattr(self, name, None)
        if value == old_value:
            return

        super().__setattr__(name, value)

        if name.startswith("_"):
            return

        if self._batch_active or not self.__is_observable_field(name):
            return

        if self.model_fields[name].json_schema_extra.get('observable', False):
            logging.debug(f"Notifying that {name} changed")
            self._observable.trigger(f"{name}_changed", self)

            logging.debug("Notifying that model changed")
            self._observable.trigger("model_changed", self)

            logging.debug(f"Notifications for {name} complete")

    def trigger_model_changed(self) -> None:
        """
        Trigger a "model_changed" event to notify observers that the model has changed.
        """
        self._observable.trigger("model_changed", self)

    def begin_batch(self) -> None:
        """
        Begin a batch operation, during which observable events are deferred.
        """
        self._batch_active = True

    def end_batch(self, trigger: bool = True) -> None:
        """
        End a batch operation and optionally trigger a "model_changed" event.

        Args:
            trigger (bool): Whether to trigger a "model_changed" event after ending the batch. Defaults to True.
        """
        self._batch_active = False
        if trigger:
            self._observable.trigger("model_changed", self)

    @contextmanager
    def batch(self, trigger: bool = True) -> Generator[None, None, None]:
        """
        Context manager for batching changes.

        Args:
            trigger (bool): Whether to trigger a "model_changed" event after the batch ends. Defaults to True.

        Yields:
            None: Batch context for performing multiple updates.
        """
        try:
            self.begin_batch()
            yield
        finally:
            self.end_batch(trigger)
