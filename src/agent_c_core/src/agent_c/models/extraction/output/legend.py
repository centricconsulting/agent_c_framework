from enum import Enum
from typing import List, Union

from pydantic import BaseModel, ConfigDict


class EntryType(Enum):
    symbol = 'symbol'
    color = 'color'
    other = 'other'

class AnnotationLevel(Enum):
    single_value = 'single value'
    full_table = 'full table'
    row = 'row'
    column = 'column'

class LegendEntry(BaseModel):
    model_config = ConfigDict(use_enum_values=True, json_encoders = {
            EntryType: lambda v: v.name,
            AnnotationLevel: lambda v: v.name
        })
    entry_type: EntryType
    applies_to: AnnotationLevel
    description: str
    meaning: str

    @property
    def option_help_text_for_model(self):
        return f"- {self.description} indicates {self.meaning}"

    @property
    def __str__(self):
        return f"- {self.description} indicates {self.meaning}"


class LegendTable(BaseModel):
    entries: List[LegendEntry]

    def get_annotation_meaning(self, annotation: str) -> Union[str, None]:
        for entry in self.entries:
            if entry.description == annotation:
                return entry.meaning

        return None

    @property
    def option_help_text_for_model(self):
        text = f"The following visual indicators are used in the table at different levels:"
        for et in [AnnotationLevel.single_value, AnnotationLevel.row, AnnotationLevel.column, AnnotationLevel.full_table]:
            annos = [entry for entry in self.entries if entry.applies_to == et.value]
            if annos:
                text += f"\n\n## {et.value.capitalize()} level:\n"
                text += '\n'.join([entry.option_help_text_for_model for entry in annos])


        return text

