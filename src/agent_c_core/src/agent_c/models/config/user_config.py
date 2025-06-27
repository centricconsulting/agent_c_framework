from typing import Dict, Any
from pydantic import Field, field_validator

from agent_c.models import ObservableModel
from agent_c.models.config.config_collection import UserConfigCollection, UserConfigCollectionField


class UserConfig(ObservableModel):
    version: int = Field(1,
                         description="The version of the system config file format")
    runtimes: UserConfigCollectionField = Field(default_factory=UserConfigCollection,
                                                description="Configuration for the various agent runtime APIs from the vendors")
    tools: UserConfigCollectionField =Field(default_factory=UserConfigCollection,
                                            description="Configuration for the toolsets that require keys and other configuration")
    misc: UserConfigCollectionField = Field(default_factory=UserConfigCollection,
                                            description="Miscellaneous configuration that does not fit into other categories")



    def __init__(self, **data: Any) -> None:
        """
        Initializes the SystemConfigFile with the provided data.

        Args:
            **data: Additional keyword arguments to initialize the model.
        """
        # for cat in ['tools', 'misc', 'runtimes']:
        #     if cat not in data:
        #         data[cat] = UserConfigCollection()
        #     elif isinstance(data[cat], UserConfigCollection):
        #         # If it's already a ConfigCollection, no need to convert
        #         continue
        #     elif isinstance(data[cat], dict):
        #         # Convert dict to ConfigCollection
        #         data[cat] = UserConfigCollection(data[cat])
        #     else:
        #         raise ValueError(f"Invalid type for {cat}: {type(data[cat])}. Expected dict or UserConfigCollection.")

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
