from pydantic import Field
from typing import Optional
from agent_c.models import BaseContext
from agent_c.models.config import BaseToolsetConfig

class DallEToolsConfig(BaseToolsetConfig):
    default_save_folder: Optional[str] = Field(None,
                                                description="A UNC workspace path to save images to. If not provided, images will only be displayed in the UI.")

class DallEToolsContext(BaseContext):
    default_save_folder: Optional[str] = Field(None,
                                                description="A UNC workspace path to save images to. If not provided, images will only be displayed in the UI.")
