import uuid
import datetime

from pydantic import Field, field_validator
from typing import Optional, Dict, Any, Union, List

from agent_c_core.agent_c.models.base import BaseModel

class ChatUser(BaseModel):
    """
    Represents a user object with a unique identifier, metadata,
    and other attributes.

    Attributes
    ----------
    uuid : Optional[UUID]
        A unique identifier for the user. Used internally as a primary key.
    id : Optional[int]
        The ID of the user. Used as a cursor for pagination.
    created_at : Optional[datetime]
        The timestamp when the user was created.
    updated_at : Optional[datetime]
        The timestamp when the user was last updated.
    deleted_at : Optional[datetime]
        The timestamp when the user was deleted.
    user_id : str
        The unique identifier of the user.
    email : Optional[str]
        The email of the user.
    first_name : Optional[str]
        The first name of the user.
    last_name : Optional[str]
        The last name of the user.
    metadata : Optional[Dict[str, Any]]
        The metadata associated with the user.
    """

    uuid: Optional[str] = Field(default_factory=lambda: str(uuid.uuid4()))
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = None
    deleted_at: Optional[str] = None
    user_id: str
    email: Optional[str] = None
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    metadata: Optional[Dict[str, Any]] = {}