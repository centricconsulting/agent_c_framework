import logging
from functools import wraps
from string import Template

from typing import Callable, Any, Dict
from pydantic import BaseModel, ConfigDict


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

    async def get_dynamic_properties(self) -> Dict[str, Any]:
        """
        Retrieves the dynamic properties of the PromptSection.

        Returns:
            Dict[str, Any]: A dictionary of dynamic property names and their values.
        """
        dynamic_props: Dict[str, Any] = {}
        for attr_name in dir(self):
            # Skip internal or special attributes
            if attr_name.startswith('_'):
                continue

            attr = getattr(self, attr_name)
            if callable(attr) and getattr(attr, 'is_property_bag_item', False):
                try:
                    dynamic_props[attr_name] = await attr()
                except Exception as e:
                    logging.exception(f"Error getting dynamic property '{attr_name}': {e}")
        return dynamic_props

    async def render(self, data: Dict[str, Any]) -> str:
        section_data: Dict[str, Any] = {**data, ** await self.get_dynamic_properties()}
        template: Template = Template(self.template)
        result = template.substitute(section_data)
        return result
