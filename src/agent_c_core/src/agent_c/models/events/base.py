# from typing import Any
# from pydantic import Field
#
# from agent_c.util import to_snake_case
# from agent_c.models.base import BaseModel
#
# class BaseEvent(BaseModel):
#     """
#     A base model for event objects.
#
#     Attributes:
#         type (str): The type identifier for the event. Defaults to the snake_case
#             version of the class name if not provided.
#
#     Args:
#         **data: Arbitrary keyword arguments that will be used to initialize the model.
#             If 'type' is not provided in the data, it will be automatically set to
#             the snake_case version of the class name, without "event".
#     """
#     #type: str = Field(..., description="The type of the event. Defaults to the snake case class name without event" )
#
#     def __init__(self, **data: Any) -> None:
#         if 'type' not in data:
#             data['type'] = to_snake_case(self.__class__.__name__.removesuffix('Event'))
#
#         super().__init__(**data)


from typing import Any, Optional
from pydantic import Field

from agent_c.util import to_snake_case
from agent_c.models.base import BaseModel


class BaseEvent(BaseModel):
    """
    A base model for event objects.

    Attributes:
        type (str): The type identifier for the event. Defaults to the snake_case
            version of the class name if not provided.
        session_id (Optional[str]): Identifier for the chat session.
        interaction_id (Optional[str]): Identifier for the interaction (for tracing).
    """

    type: str = Field(..., description="The type of the event. Defaults to the snake case class name without event")
    session_id: Optional[str] = Field(None, description="Identifier for the chat session")
    interaction_id: Optional[str] = Field(None, description="Identifier for the interaction")

    def __init__(self, **data: Any) -> None:
        if 'type' not in data:
            data['type'] = to_snake_case(self.__class__.__name__.removesuffix('Event'))
        super().__init__(**data)
