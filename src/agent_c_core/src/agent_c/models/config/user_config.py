from typing import Dict, Any
from pydantic import Field

from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.config.config_collection import UserConfigCollection, UserConfigCollectionField


class UserConfig(AsyncObservableModel):
    version: int = Field(1,
                         description="The version of the system config file format")
    runtimes: UserConfigCollectionField = Field(default_factory=UserConfigCollection,
                                                description="Configuration for the various agent runtime APIs from the vendors")
    tools: UserConfigCollectionField =Field(default_factory=UserConfigCollection,
                                            description="Configuration for the toolsets that require keys and other configuration")
    misc: UserConfigCollectionField = Field(default_factory=UserConfigCollection,
                                            description="Miscellaneous configuration that does not fit into other categories")

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
            'tools': self.tools,
            'misc': self.misc
        }
        result: Dict[str, Any] = {'version': self.version}
        for name in ['runtimes','tools', 'misc']:
            if len(categories[name]) > 0:
                result[name] = {}
                col = categories[name]
                for config in col.values():
                    result[name][config.config_type] = config.model_dump(exclude=['category'])

        return result
