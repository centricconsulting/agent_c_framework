import logging
from typing import Optional, Type
from pydantic import BaseModel, Field
from agent_c.models.extraction.output.legend import LegendTable

from agent_c import ImageInput

class FieldAnnotation(BaseModel):
    field_name: str
    note: str

class ExtractedTableRow(BaseModel):
    field_annotations: list[FieldAnnotation]

    def get_display_for(self, field_name: str, legend: LegendTable) -> tuple[Optional[str], Optional[str]]:
        try:
            field_value = getattr(self, field_name)
            # Iterate through field annotations to find a match
            for field_annotation in self.field_annotations:
                if field_annotation.field_name == field_name:
                    anno = legend.get_annotation_meaning(field_annotation.annotation)
                    return f"{field_value} ({field_annotation.annotation})", f"{field_annotation.field_name}: {anno}"

            # If no annotation found, return field_name as is
            return field_value, None

        except Exception as e:
            logging.error(f"Error in get_display_for: {e}")
            return None, None

    @property
    def notes(self) -> list[str]:
        return [field_annotation.note for field_annotation in self.field_annotations]

class ExtractedTable(BaseModel):
    table_type: str = Field(..., description="The type of the extracted table, used for deserialization.")
    title: Optional[str] = None
    legend: Optional[LegendTable] = None
    box_uuid: Optional[str] = None
    image_inputs: Optional[list[ImageInput]] = None
    table_annotations: Optional[list[str]] = None

    def __init__(self, **data):
        # Set the type field to the class name automatically
        data['table_type'] = self.__class__.__name__
        super().__init__(**data)

    @classmethod
    def get_subclass_for_type(cls, type_name: str) -> Type['ExtractedTable']:
        """Find the correct subclass based on the type name."""
        for subclass in cls.__subclasses__():
            if subclass.__name__ == type_name:
                return subclass

        raise ValueError(f"Unknown table type: {type_name}")
