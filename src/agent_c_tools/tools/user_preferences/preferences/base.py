from pydantic import BaseModel, ConfigDict


class UserPreference(BaseModel):
    """
    Represents a basic user preference setting, with attributes that define the preference object.
    It can be subclassed to create specific user preferences with default settings and instructions.

    A preference with visible_to_model set, needs model instructions

    """
    model_config = ConfigDict(protected_namespaces = ())
    name: str
    default_value: str = ""
    user_instructions: str = ""
    visible_to_model: bool = False
    model_instructions: str = ""
