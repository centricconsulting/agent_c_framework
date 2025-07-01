from typing import Optional

import uuid

from pydantic import Field

from agent_c.models.extraction.bounding_box_type import BoundingBoxType
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.models.extraction.output.extracted_table import ExtractedTable


class BoundingBoxModel(AsyncObservableModel):
    page_no: int
    x1: float = Field(default=0.0,
                      description="X coordinate of the top left corner of the bounding box")
    y1: float = Field(default=0.0,
                        description="Y coordinate of the top left corner of the bounding box")
    x2: float = Field(default=0.0,
                      description="X coordinate of the bottom right corner of the bounding box")
    y2: float = Field(default=0.0,
                      description="Y coordinate of the bottom right corner of the bounding box")
    label: str = Field(default="Bounding Box")
    box_type: BoundingBoxType = Field(default=BoundingBoxType.bbox)
    sub_boxes: list['BoundingBoxModel'] = Field(default_factory=list,
                                                description="List of sub-bounding boxes contained within this bounding box")
    _extracted_table: Optional[ExtractedTable] = None
    __uuid: Optional[str] = None
    parent: Optional['BoundingBoxModel'] = Field(default=None,
                                                    description="Parent bounding box if this is a sub-box")


    @property
    def extracted_table(self) -> Optional[ExtractedTable]:
        if self._extracted_table is not None and self._extracted_table.box_uuid != self.uuid:
            self._extracted_table = None

        return self._extracted_table

    @extracted_table.setter
    def extracted_table(self, value: Optional[ExtractedTable]):
        if value is not None and value.box_uuid != self.uuid:
            raise ValueError("UUID does not match bounding box")

        self._extracted_table = value
        self._observable.trigger("extracted_table_changed", self)
        self._observable.trigger("model_changed", self)

    @property
    def extracted_tables(self) -> Optional[list[ExtractedTable]]:
        tables = []
        if self.extracted_table is not None:
            tables.append(self.extracted_table)

        for box in self.sub_boxes:
            tables.extend(box.extracted_tables)

        return tables

    @property
    def id_map(self) -> dict[str, 'BoundingBoxModel']:
        id_map = {self.uuid: self}
        for box in self.sub_boxes:
            id_map.update(box.id_map)

        return id_map

    @property
    def uuid(self) -> str:
        """This property generates a UUID based on the bounding box properties.
           This allows us to map saved data back to the correct bounding box unless something important has changed about the box
        """
        if self.__uuid is None:
            # TODO: This should factor it the sub-box sizes as well I think...
            key: str = f"{self.box_type}-{self.page_no}-{self.x1}-{self.y1}-{self.x2}-{self.y2}-{self.label}-{len(self.sub_boxes)}"
            self.__uuid = uuid.uuid5(uuid.NAMESPACE_DNS, key).hex

        return self.__uuid

    @property
    def uuids(self) -> list[str]:
        uuids = [self.uuid]
        for box in self.sub_boxes:
            uuids.extend(box.uuids)

        return uuids

    def sub_boxes_of_type(self, box_type: BoundingBoxType):
        return [box for box in self.sub_boxes if box.box_type == box_type]

    @property
    def tuple_coords(self):
        return self.x1, self.y1, self.x2, self.y2

    def enclose(self, other: 'BoundingBoxModel'):
        self.x1 = min(self.x1, other.x1)
        self.y1 = min(self.y1, other.y1)
        self.x2 = max(self.x2, other.x2)
        self.y2 = max(self.y2, other.y2)

    def set(self, x1: float, y1: float, x2: float, y2: float):
        self.begin_batch()
        self.x1 = x1
        self.y1 = y1
        self.x2 = x2
        self.y2 = y2
        self.end_batch()

    def pad(self, padding: float = 5.0):
        self.set(self.x1 - padding, self.y1 - padding, self.x2 + padding, self.y2 + padding)

    def extend(self, x1: float = 0.0, y1: float = 0.0, x2: float = 0.0, y2: float = 0.0):
        self.set(self.x1 - x1, self.y1 - y1, self.x2 + x2, self.y2 + y2)


