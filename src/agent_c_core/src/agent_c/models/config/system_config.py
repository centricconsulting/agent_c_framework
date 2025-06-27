from typing import Dict, Any
from pydantic import Field

from agent_c.models import ObservableModel
from agent_c.models.config.config_collection import ConfigCollection, ConfigCollectionField, ensure_config_collection


class SystemConfigFile(ObservableModel):
    version: int = Field(1,
                         description="The version of the system config file format")
    runtimes: ConfigCollectionField = Field(default_factory=ConfigCollection,
                                            description="Configuration for the various agent runtime APIs from the vendors")
    core: ConfigCollectionField = Field(default_factory=ConfigCollection,
                                        description="Configuration for Agent C core")
    tools: ConfigCollectionField = Field(default_factory=ConfigCollection,
                                         description="Configuration for the toolsets that require keys and other configuration")
    api: ConfigCollectionField = Field(default_factory=ConfigCollection,
                                       description="Configuration for the API FastAPI endpoints and other API related settings")

    misc: ConfigCollectionField = Field(default_factory=ConfigCollection,
                                        description="Miscellaneous configuration that does not fit into other categories")

    def __init__(self, **data) -> None:
        """
        Initializes the SystemConfigFile with the provided data.

        Args:
            **data: Keyword arguments to initialize the model.
        """
        super().__init__(**data)

        if isinstance(self.runtimes, dict) and not isinstance(self.runtimes, ConfigCollection):
            foo = ConfigCollection(self.runtimes)
            self.runtimes = ensure_config_collection(self.runtimes)
            self.runtimes= foo



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
