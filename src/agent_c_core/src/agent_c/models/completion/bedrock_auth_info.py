from pydantic import Field
from typing import Optional, Literal
from agent_c.models.base import BaseModel

class BedrockAuthInfo(BaseModel):
    type: Literal['bedrock'] = Field('bedrock', description="The type of the auth info, must be bedrock")
    aws_access_key:  Optional[str] = Field(None, description="The AWS access key to use for the interaction")
    aws_secret_key:  Optional[str] = Field(None, description="The AWS secret key to use for the interaction")
    aws_region: Optional[str] = Field(None, description="The AWS region to use for the interaction")
    aws_session_token: Optional[str] = Field(None, description="Temporary credentials can be used with aws_session_token.")
