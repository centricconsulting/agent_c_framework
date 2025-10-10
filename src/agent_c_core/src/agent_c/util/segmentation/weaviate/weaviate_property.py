from typing import Any, Optional

from pydantic import Field
from weaviate.classes.config import DataType


def WeaviateProperty(
        description: str,
        data_type: DataType,
        *,
        module_config: Optional[dict] = None,
        nested_properties: Optional[dict] = None,
        index_filterable: bool = True,
        index_searchable: bool = True,
        **kwargs: Any
):
    weaviate = {
        "dataType": [data_type],
        "indexFilterable": index_filterable,
        "indexSearchable": index_searchable
    }

    if module_config is not None:
        weaviate["moduleConfig"] = module_config

    if nested_properties is not None:
        weaviate["nestedProperties"] = nested_properties

    return Field(description=description, **kwargs, json_schema_extra= {'weaviate':weaviate})
