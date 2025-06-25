from typing import Dict, Any, Optional

from newsapi.const import categories
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

    def __init__(self, **data: Any) -> None:
        """
        Initializes the SystemConfigFile with the provided data.

        Args:
            **data: Additional keyword arguments to initialize the model.
        """
        for cat in ['core', 'tools', 'api', 'misc', 'runtimes']:
            if cat not in data:
                data[cat] = ConfigCollection()
            elif isinstance(data[cat], ConfigCollection):
                # If it's already a ConfigCollection, no need to convert
                continue
            elif isinstance(data[cat], dict):
                # Convert dict to ConfigCollection
                data[cat] = ConfigCollection(data[cat])
            else:
                raise ValueError(f"Invalid type for {cat}: {type(data[cat])}. Expected dict or ConfigCollection.")

        super().__init__(**data)


    def model_dump_yaml(self) -> Dict[str, Any]:
        """
        Dumps the model to a YAML string.

        Args:
            **kwargs: Additional keyword arguments to pass to the dump function.

        Returns:
            str: The YAML representation of the model.
        """
        categories = {
            'runtimes': self.runtimes,
            'core': self.core,
            'tools': self.tools,
            'api': self.api,
            'misc': self.misc
        }
        result: Dict[str, Any] = {'version': self.version}
        for name in ['runtimes', 'core', 'tools', 'api', 'misc']:
            if len(categories[name]) > 0:
                result[name] = {}
                col = categories[name]
                for config in col.values():
                    result[name][config.config_type] = config.model_dump(exclude=['category', 'config_type'])

        return result
