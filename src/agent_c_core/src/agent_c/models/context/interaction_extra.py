from typing import Optional, TYPE_CHECKING, Any

from pydantic import Field, computed_field, field_validator

from agent_c.models import BaseDynamicContext

if TYPE_CHECKING:
    from agent_c.models.context.interaction_context import InteractionContext
    from agent_c.toolsets.tool_set import Toolset



class BaseInteractionContextExtra(BaseDynamicContext):
   interaction: 'InteractionContext' = Field(...,
                                                description="The interaction context for this dynamic context. "
                                                            "This is used to store information about the interaction, such as the chat session and agent configuration.",
                                                exclude=True)

class BaseToolInteractionContextExtra(BaseInteractionContextExtra):
    _my_tool: Optional['Toolset'] = None
    toolset_class: str = Field(...,
                                description="The toolset class name for this interaction context extra. "
                                                            "This is used to store the toolset class name for the interaction.",
                                                exclude=True)

    @classmethod
    @field_validator( 'toolset_class', mode='before')
    def ensure_toolset_class(cls, value: Any):
        """
        Ensure the toolset_class is set to the snake case class name and ends in toolset.
        """
        if value is None:
            value = cls.__name__.removesuffix("InteractionContextExtra").removesuffix("InteractionContext").removesuffix("Context")

        return value

    @computed_field
    @property
    def tool(self) -> 'Toolset':
        """
        Returns the toolset associated with this interaction context extra.
        This is used to access the toolset for the interaction.
        """
        if self._my_tool is None and not self.toolset_class is None:
            self._my_tool = self.interaction.toolsets.get(self.toolset_class)

        return self._my_tool


