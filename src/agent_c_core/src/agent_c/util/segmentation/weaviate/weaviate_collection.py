from typing import Optional, List, Union, Type, Any
from weaviate.collections import Collection
from weaviate.collections.classes.types import Properties, References
from weaviate.collections.classes.config import _InvertedIndexConfigCreate, _MultiTenancyConfigCreate, _ReferencePropertyBase, _ReplicationConfigCreate,  _ShardingConfigCreate
from weaviate.collections.classes.config_vector_index import _VectorIndexConfigCreate
from weaviate.collections.classes.config_vectorizers import _VectorizerConfigCreate
from weaviate.collections.classes.config_named_vectors import _NamedVectorConfigCreate

from pydantic import BaseModel, Field


class WeaviateCollection(BaseModel):
    """
    This is a model of the Weaviate collection schema from: https://weaviate.io/developers/weaviate/config-refs/schema

    Set the item_model to a model that contains WeaviateProperty fields to define the properties of the collection.

    """
    # Properties are extracted from this item model class
    item_model: Any = Field(description="The model of the items in the collection.")

    # This will be the name of the collection in Weaviate
    name: str = Field(description="The name of the collection. i.e. 'Question'")

    # The rest of the fields are the Weaviate collection schema
    description: str = Field("", description="A description of the collection for your reference.")
    inverted_index_config: Optional[_InvertedIndexConfigCreate] = None
    multi_tenancy_config: Optional[_MultiTenancyConfigCreate] = None
    references: Optional[List[_ReferencePropertyBase]] = None
    replication_config: Optional[_ReplicationConfigCreate] = None
    sharding_config: Optional[_ShardingConfigCreate] = None
    vector_index_config: Optional[_VectorIndexConfigCreate] = None
    vectorizer_config: Optional[Union[_VectorizerConfigCreate, List[_NamedVectorConfigCreate]]] = None
    data_model_properties: Optional[Any] = None
    data_model_references: Optional[Any] = None
    skip_argument_validation: bool = False

    @property
    def properties(self) -> List[dict]:
        props = []
        for field_name, field_model in self.item_model.__fields__.items():
            weaviate_meta = field_model.field_info.extra.get('weaviate')
            if weaviate_meta:
                prop = {
                    "name": field_name,
                    "description": field_model.field_info.description,
                    "dataType": weaviate_meta["dataType"],
                    "indexFilterable": weaviate_meta["indexFilterable"],
                    "indexSearchable": weaviate_meta["indexSearchable"]
                }

                if "moduleConfig" in weaviate_meta:
                    prop["moduleConfig"] = weaviate_meta["moduleConfig"]

                props.append(prop)

        return props

    def model_dump(self, **kwargs) -> dict[str, Any]:
        props = super().model_dump(exclude={"item_model", "name"}, exclude_none=True)
        props["properties"] = self.properties
        return props

    def create_collection(self, collections) -> Collection[Properties, References]:
        """
        Create the collection in Weaviate.
        """
        return collections.create(self.name,  **self.model_dump())

    def collection_exists(self, collections) -> bool:
        """
        Check if the collection exists in Weaviate.
        """
        return collections.exists(self.name)

    def delete_collection(self, collections) -> None:
        """
        Delete the collection in Weaviate.
        """
        collections.delete(self.name)

    def get_collection(self, collections,
                       data_model_properties: Optional[Type[Properties]] = None,
                       data_model_references: Optional[Type[References]] = None,
                       skip_argument_validation: bool = False,) -> Collection[Properties, References]:
        """
        Get the collection in Weaviate.
        """
        return collections.get(self.name, data_model_properties=data_model_properties, data_model_references=data_model_references, skip_argument_validation=skip_argument_validation)

