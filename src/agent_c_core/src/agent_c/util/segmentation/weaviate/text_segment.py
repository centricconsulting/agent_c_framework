from typing import ClassVar
from logging import Logger
import logging
import hashlib
import uuid
from typing import Optional
from pydantic import PrivateAttr

from agent_c.util.segmentation.weaviate import WeaviateSegment


class TextSegment(WeaviateSegment):
    """
    A base class for segments which will be stored in and searched for in Weaviate.  It covers the basic fields that one would expect to find in a segment.
    As well as a few fields that allow for filtering and searching based on categories and keywords.

    This uses a strategy of indexing far simpler content for search and retrieving more complex content for presentation to the model.

    """
    logger: ClassVar[Logger] = logging.getLogger(__name__)
    _distance: float = PrivateAttr(default=0)

    def append_content(self, content: str, content_len: int):
        self.content += content
        self.token_count += content_len

    @property
    def distance(self) -> Optional[float]:
        return self._distance

    @distance.setter
    def distance(self, value: float):
        self._distance = value

    @classmethod
    def from_weaviate_response_object(cls, obj):
        cls.logger.debug(f"Creating TextSegment from Weaviate response - UUID: {obj.uuid}")
        # cls.logger.debug(f"UUID: {obj.uuid}")
        # cls.logger.debug(f"Properties: {obj.properties}")
        # if 'parent_segment' in obj.properties:
        #     cls.logger.debug(f"Found parent_segment: {obj.properties['parent_segment']}")
        # else:
        #     cls.logger.warning("No parent_segment in Weaviate response!")
        return cls(
            uuid=str(obj.uuid),
            created=obj.properties['created'].isoformat(),
            content=obj.properties['content'],
            index_content=obj.properties['index_content'],
            sequence=obj.properties['sequence'],
            token_count=obj.properties['token_count'],
            citation=obj.properties['citation'],
            #categories=obj['categories'],
            #keywords=obj['keywords'],
            parent_segment=str(obj.properties['parent_segment']) if obj.properties.get('parent_segment') else None,
            distance=obj.metadata.distance
        )

    @classmethod
    def from_weaviate_response(cls, response):
        return [cls.from_weaviate_response_object(obj) for obj in response.objects]

    @staticmethod
    def stable_hash(text: str) -> str:
        # Could use sha1 or sha256. sha1 is shorter, but sha256 is more robust.
        # Either is deterministic for the same text across runs.
        return hashlib.sha1(text.encode("utf-8")).hexdigest()

    def generate_uuid(self, force: bool = False):
        """
        Generates a deterministic UUID for this segment based on:
          - citation
          - sequence
          - parent_segment (if present)
          - stable hash of content

        That way, two segments with the same (citation, sequence, parent_segment, content)
        will get the exact same UUID.
        """
        if self.uuid is None or force:
            base_str = f"{self.citation}:{self.sequence}"
            if self.parent_segment:
                base_str += f":{self.parent_segment}"
            # Incorporate the stable hash of the content
            base_str += f":{self.stable_hash(self.content)}"

            self.uuid = str(uuid.uuid5(uuid.NAMESPACE_DNS, base_str))

    # def generate_uuid_from_citation(self, force: bool = False):
    #     if self.uuid is None or force:
    #         if self.parent_segment is not None:
    #             self.uuid = str(
    #                 uuid.uuid5(uuid.NAMESPACE_DNS, f"{self.citation}:{self.sequence}:{self.parent_segment}"))
    #         else:
    #             self.uuid = str(uuid.uuid5(uuid.NAMESPACE_DNS, f"{self.citation}:{self.sequence}"))