from pydantic import Field

from agent_c.models import ObservableModel
from agent_c.models.config.config_collection import ConfigCollection


class SystemConfigFile(ObservableModel):
    version: int = Field(1,
                         description="The version of the system config file format")
    runtimes: ConfigCollection = Field(default_factory=ConfigCollection,
                                       description="Configuration for the various agent runtime APIs from the vendors")
    core: ConfigCollection = Field(default_factory=ConfigCollection,
                                   description="Configuration for Agent C core")
    tools: ConfigCollection = Field(default_factory=ConfigCollection,
                                       description="Configuration for the toolsets that require keys and other configuration")
    api: ConfigCollection = Field(default_factory=ConfigCollection,
                                  description="Configuration for the API FastAPI endpoints and other API related settings")

    misc: ConfigCollection = Field(default_factory=ConfigCollection,
                                  description="Miscellaneous configuration that does not fit into other categories")


