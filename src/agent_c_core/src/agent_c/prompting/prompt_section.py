import inspect
from functools import wraps
from string import Template

from typing import Callable, Any, Dict, Optional
from pydantic import BaseModel, ConfigDict

from agent_c.util.logging_utils import LoggingManager


def property_bag_item(func: Callable) -> Callable:
    """
    Decorator to mark a method as a dynamic property for a PromptSection.
    The method marked with this decorator will be included in the dynamic properties.

    Args:
        func (Callable): The method to be marked as a dynamic property.

    Returns:
        Callable: The wrapped method with an additional attribute to indicate it's a property bag item.
    """

    @wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        return func(*args, **kwargs)

    wrapper.is_property_bag_item = True
    return wrapper


class PromptSection(BaseModel):
    """
    A class representing a section of a prompt with dynamic properties.

    Attributes:
        name (str): The name of the section.
        template (str): The template string for the section.
        render_section_header (bool): Flag to determine if a header should be rendered for the section.
        required (bool): Flag to determine if the section is required.
    """
    model_config = ConfigDict(arbitrary_types_allowed=True, protected_namespaces=())
    name: str
    template: str
    render_section_header: bool = True
    required: bool = False

    def __init__(self, **data: Any):
        """
        Initialize the PromptSection with the provided data.

        Args:
            **data: Arbitrary keyword arguments to initialize the section.
        """
        super().__init__(**data)
        logging_manager = LoggingManager(self.__class__.__name__)
        self._logger = logging_manager.get_logger()

    @classmethod
    def default_context(cls) -> Optional['BaseContext']:
        """
        Returns the default context for the prompt section.

        Returns:
            Optional[BaseContext]: The default context, or None if not set.
        """
        return None

    async def get_dynamic_properties(self, context) -> Dict[str, Any]:
        """
        Retrieves the dynamic properties of the PromptSection.

        Args:
            data: Dict[str, Any]: The data dictionary to pass to attributes that accept it
            context: InteractionContext: The context of the interaction, used to pass additional data to dynamic properties.

        Returns:
            Dict[str, Any]: A dictionary of dynamic property names and their values.
        """
        data = {}
        dynamic_props: Dict[str, Any] = {}
        for attr_name in dir(self):
            # Skip internal or special attributes
            if attr_name.startswith('_'):
                continue

            attr = getattr(self, attr_name)
            # This always trips me up when I see it and I wrote it...
            # Check if the attribute is a callable and has the property_bag_item flag
            # the DEFAULT is false, so if the attribute is not callable or does not have the flag,
            # it will be skipped
            if callable(attr) and getattr(attr, 'is_property_bag_item', False):
                try:
                    sig = inspect.signature(attr)
                    param_count = len(sig.parameters)
                    if param_count == 0:
                        dynamic_props[attr_name] = await attr()
                    elif param_count == 1:
                        dynamic_props[attr_name] = await attr(data)
                    elif param_count == 2:
                        dynamic_props[attr_name] = await attr(context, data)
                    else:
                        self._logger.exception(f"Dynamic property '{attr_name}' has too many parameters: {param_count}")
                except Exception as e:
                    self._logger.exception(f"Error getting dynamic property '{attr_name}': {e}")

        return dynamic_props

    async def render(self, context) -> str:
        template: Template = Template(self.template)
        result = template.substitute(await self.get_dynamic_properties(context))
        return result
