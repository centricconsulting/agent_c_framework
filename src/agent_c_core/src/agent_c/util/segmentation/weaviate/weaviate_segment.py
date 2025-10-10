from typing import ClassVar
from logging import Logger
import logging
import uuid
from datetime import datetime, timezone
from typing import Optional, List

from agent_c.util.segmentation.weaviate.weaviate_property import WeaviateProperty, DataType
from pydantic import BaseModel


class WeaviateSegment(BaseModel):
    """
    A base class for segments which will be stored in and searched for in Weaviate.  It covers the basic fields that one would expect to find in a segment.
    As well as a few fields that allow for filtering and searching based on categories and keywords.

    This uses a strategy of indexing far simpler content for search and retrieving more complex content for presentation to the model.

    """
    logger: ClassVar[Logger] = logging.getLogger(__name__)

    uuid: Optional[str] = None

    created: str = WeaviateProperty("timestamp", DataType.DATE, default_factory=lambda: datetime.now(timezone.utc).isoformat(),
                                    index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": False, "vectorizePropertyName": True}})

    content: str = WeaviateProperty("The content of the segment to be displayed to the model", DataType.TEXT,
                                    index_filterable=False, index_searchable=False, module_config={"text2vec-openai": {"skip": True, "vectorizePropertyName": False}})

    index_content: str = WeaviateProperty("The content of the segment to be indexed for search", DataType.TEXT,
                                          index_filterable=False, index_searchable=True, module_config={"text2vec-openai": {"skip": False, "vectorizePropertyName": False}})

    sequence: int = WeaviateProperty("The sequence number of the segment", DataType.INT,
                                     index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": True, "vectorizePropertyName": False}})

    token_count: int = WeaviateProperty("The number of tokens in the content property", DataType.INT,
                                        index_filterable=True, index_searchable=False, module_config={"text2vec-openai": {"skip": True, "vectorizePropertyName": False}})

    citation: str = WeaviateProperty("The citation of the segment, should be a URI/filename or other unique human readable ID for the model to cite.", DataType.TEXT,
                                     index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": True, "vectorizePropertyName": True}})

    categories: List[str] = WeaviateProperty("One or more categories for the segment", DataType.TEXT_ARRAY,
                                             index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": False, "vectorizePropertyName": True}},
                                             default=[])

    keywords: List[str] = WeaviateProperty("One or more keywords for the segment", DataType.TEXT_ARRAY,
                                           index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": False, "vectorizePropertyName": True}},
                                           default=[])

    parent_segment: Optional[str] = WeaviateProperty("A UUID pointing to a larger, parent segment.", DataType.UUID,
                                                     index_filterable=True, index_searchable=True, module_config={"text2vec-openai": {"skip": True, "vectorizePropertyName": False}},
                                                     default=None)

    def as_weaviate_object(self) -> dict:
        """
        Returns a dictionary representation of the segment suitable for storing in Weaviate.

        Returns:
            dict: A dictionary representing the segment.
        """

        weaviate_fields = {}
        for field_name, field_model in self.model_fields.items():
            if (field_model.json_schema_extra is not None and
                    'weaviate' in field_model.json_schema_extra):
                weaviate_fields[field_name] = getattr(self, field_name)

        uuid_str = self.uuid or str(uuid.uuid5(uuid.NAMESPACE_DNS, f"{self.citation}:{self.sequence}"))

        return {'properties': weaviate_fields, 'uuid': uuid_str}
