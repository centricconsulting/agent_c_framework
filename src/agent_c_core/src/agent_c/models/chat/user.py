import datetime

from pydantic import Field,  model_validator
from typing import Optional, Dict, Any, List

from agent_c.models import AsyncObservableModel
from agent_c.util import MnemonicSlugs
from agent_c.models.chat.chat_user_location import ChatUserLocation
from agent_c.models.config.user_config import UserConfig
from agent_c.models.context.context_bag import ContextBag


class ChatUser(AsyncObservableModel):
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
    version: int = Field(1,
                         description="The version of the user model. This is used to track changes in the user model.")
    user_id: Optional[str] = Field(None,
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
    created_at: str = Field(default_factory=lambda: datetime.datetime.now().isoformat(),
                            description="The timestamp when the user was created, used for tracking changes. ")
    updated_at: Optional[str] = Field(None,
                                      description="The timestamp when the user was last updated, used for tracking changes. ")
    deleted_at: Optional[str] = Field(None,
                                       description="The timestamp when the user was deleted, used for tracking changes. ")
    default_location: ChatUserLocation = Field(default_factory=ChatUserLocation,
                                               description="The default location of the user, used for "
                                                           "context in interactions and Open AI search tools. ")
    context: ContextBag = Field(default_factory=ContextBag,
                                description="A dictionary of context models to provide data for tools / prompts. ")
    config: UserConfig = Field(default_factory=UserConfig,
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

    @model_validator(mode='after')
    def post_init(self):
        if not self.user_id:
            self.user_id = MnemonicSlugs.generate_id_slug(2, self.user_name)
        return self

    def is_new_user(self) -> bool:
        return self.user_name == self.model_fields['user_name'].default


    @classmethod
    def from_user_id(cls, user_id: str) -> 'ChatUser':
        """
        Create a ChatUser instance from a user ID.
        This method is used to load a user from the database or other storage.
        """
        from agent_c.config.user_loader import UserLoader
        user_dict = UserLoader.instance().load_user_dict(user_id)
        if not user_dict:
            raise ValueError(f"User with ID {user_id} not found.")
        return cls(**user_dict)

    #@model_validator(mode="before")
    @classmethod
    def ensure_chat_user(cls, v: Any) -> Any:
        """
        Ensure that the user_id is set to a valid slug format.
        If not provided, it will be generated based on the user_name.
        """
        if isinstance(v, cls) or v is None:
            return v

        if isinstance(v, str):
            from agent_c.config.user_loader import UserLoader
            v = UserLoader.instance().load_user_dict(v)

        if isinstance(v, dict):
            if 'user_id' not in v or not v['user_id']:
                v['user_id'] = MnemonicSlugs.generate_id_slug(2, v.get('user_name', 'agent_c_user'))
            return cls(**v)

        raise ValueError(f"Expected ChatUser, dict or str, got {type(v)}")
