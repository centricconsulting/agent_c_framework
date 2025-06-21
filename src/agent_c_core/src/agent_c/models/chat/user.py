import uuid
import datetime

from pydantic import Field, field_validator
from typing import Optional, Dict, Any, Union, List

from agent_c.models.base import BaseModel
from agent_c.models.chat.chat_user_location import ChatUserLocation
from agent_c.models.config.config_collection import ConfigCollection
from agent_c.models.context.context_bag import ContextBag
from agent_c.util import MnemonicSlugs


class ChatUser(BaseModel):
    """
    Represents a user object with a unique identifier, metadata,
    and other attributes.

    Attributes
    ----------
    id : str
        The  ID of the user, in slug format. This is the core ID for the user.
    user_name : str
        The user name associated with the user from the application auth.
    created_at : datetime
        The timestamp when the user was created.
    updated_at : Optional[datetime]
        The timestamp when the user was last updated.
    deleted_at : Optional[datetime]
        The timestamp when the user was deleted.
    email : Optional[str]
        The email of the user.
    first_name : Optional[str]
        The first name of the user.
    last_name : Optional[str]
        The last name of the user.
    context :ContextBag
        A dictionary of context models to provide data for tools / prompts.
    """
    version: int = Field(1, description="The version of the user model. This is used to track changes in the user model.")
    user_id: str = Field(None,
                         description="The ID of the user, in slug format. This is the core ID for the user. "
                                     "If not provided, it will be generated based on the user name.")
    user_name: str = Field("agent_c_user",
                           description="The user name associated with the user from the application auth")
    email: Optional[str] = Field(None,
                                 description="The email of the user, used for notifications and account management. ")

    first_name: Optional[str] = Field("New",
                                      description="The first name of the user, used for personalization "
                                                  "in interactions. ")
    last_name: Optional[str] = Field("User",
                                    description="The last name of the user, used for personalization "
                                                "in interactions. ")
    created_at: str = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = None
    deleted_at: Optional[str] = None
    default_location: ChatUserLocation = Field(default_factory=ChatUserLocation,
                                               description="The default location of the user, used for "
                                                           "context in interactions and Open AI search tools. ")
    context: ContextBag = Field(default_factory=dict,
                                description="A dictionary of context models to provide data for tools / prompts. ")
    config: ConfigCollection = Field(default_factory=dict,
                                     description="A collection of configuration settings for the user, used for "
                                                 "providing configuration to tools and runtimes")

    roles: List[str] = Field(default_factory=list,
                             description="A list of roles associated with the user, used for authorization and "
                                         "personalization in interactions. ")

    groups: List[str] = Field(default_factory=list,
                             description="A list of groups associated with the user, used for authorization and "
                                         "personalization in interactions. ")
    metadata: Dict[str, Any] = Field(default_factory=dict,
                                        description="A dictionary of metadata associated with the user, used for "
                                                    "personalization in interactions and agent memory. ")

    def __init__(self, **data: Any) -> None:
        if 'user_id' not in data or not data['user_id']:
            data['user_id'] = MnemonicSlugs.generate_id_slug(2, data['user_name'] if 'user_name' in data else 'agent_c_user')
        super().__init__(**data)
