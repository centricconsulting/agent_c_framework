import hashlib
from unstructured.documents.coordinates import CoordinateSystem
from unstructured.documents.elements import Element, ElementMetadata, ListItem, Text, Title
from typing import Callable, List, Optional, Tuple

__all__ = []


# These elements give us the ability to extract more structure from headings than we'd
# normally get loading Wod Documents.  This could have been like 10 lines of ruby code..

class Heading(Text):
    category = "Heading"


class Heading1(Text):
    category = "Heading1"

class Heading2(Text):
    category = "Heading2"

class Heading3(Text):
    category = "Heading3"

class Heading4(Text):
    category = "Heading4"

class Heading5(Text):
    category = "Heading5"

class Heading6(Text):
    category = "Heading6"

class Heading7(Text):
    category = "Heading7"

class Heading8(Text):
    category = "Heading8"

class Heading9(Text):
    category = "Heading9"

class Heading10(Text):
    category = "Heading10"


class TOCHeading(Heading):
    category = "TOCHeading"

class Subtitle(Title):
    category = "Subtitle"

class Quote(Text):
    category = "Quote"


class Caption(Text):
    category = "Caption"


for i in range(2, 4):
    globals()['ListItem' + str(i)] = type('ListItem' + str(i), (Heading,), {'category': 'ListItem' + str(i)})
    __all__.append('ListItem' + str(i))


class ListBullet(ListItem):
    category = "ListBullet"
class ListBullet2(ListItem):
    category = "ListBullet2"

class ListBullet3(ListItem):
    category = "ListBullet3"


class ListContinue(ListItem):
    category = "ListContinue"
class ListContinue2(ListItem):
    category = "ListContinue2"

class ListContinue3(ListItem):
    category = "ListContinue3"

class ListNumber(ListItem):
    category = "ListNumber"

class ListNumber2(ListItem):
    category = "ListNumber2"

class ListNumber3(ListItem):
    category = "ListNumber3"

class ListNumber4(ListItem):
    category = "ListNumber4"

class ListParagraph(ListItem):
    category = "ListParagraph"

class ListItem2(ListItem):
    category = "ListItem2"

class ListItem3(ListItem):
    category = "ListItem3"



# These elements give us a little more flexibility than the unstructured Table
# element provides.  If we have to split a table to fit into a segment then
# we can easily apply the header into the portion of the table

class TableHeader(Text):
    """An element for capturing Table headers."""
    category = "TableHeader"

    pass


class TableRow(Text):
    """An element for capturing Table Rows."""
    category = "TableRow"

    pass


class TableSeparator(Text):
    """An element for capturing Table separators."""
    category = "TableSeparator"

    pass


class CentricTable(Element):
    """An element for capturing Table separators."""
    category = "CentricTable"

    def __init__(
            self,
            header: TableHeader,
            separator: TableSeparator,
            rows: List[TableRow],
            element_id: Optional[str] = None,
            coordinates: Optional[Tuple[Tuple[float, float], ...]] = None,
            coordinate_system: Optional[CoordinateSystem] = None,
            metadata: Optional[ElementMetadata] = None,
    ):
        metadata = metadata if metadata else ElementMetadata()
        self.header = header
        self.separator = separator
        self.rows = rows
        row_text = '\n'.join([row.text for row in rows])
        self.text: str = f"{header}\n{separator}\n{row_text}"

        if isinstance(element_id, NoID):
            # NOTE(robinson) - Cut the SHA256 hex in half to get the first 128 bits
            element_id = hashlib.sha256(self.text.encode()).hexdigest()[:32]

        super().__init__(
            element_id=element_id,
            metadata=metadata,
            coordinates=coordinates,
            coordinate_system=coordinate_system,
        )

    def __str__(self):
        return self.text

    def __eq__(self, other):
        return all(
            [
                (self.text == other.text),
                (self.coordinates == other.coordinates),
                (self._coordinate_system == other._coordinate_system),
                (self.category == other.category),
            ],
        )

    def to_dict(self) -> dict:
        out = super().to_dict()
        out["element_id"] = self.id
        out["type"] = self.category
        out["text"] = self.text
        return out

    def apply(self, *cleaners: Callable):
        """Applies a cleaning brick to the text element. The function that's passed in
        should take a string as input and produce a string as output."""
        cleaned_text = self.text
        for cleaner in cleaners:
            cleaned_text = cleaner(cleaned_text)

        if not isinstance(cleaned_text, str):
            raise ValueError("Cleaner produced a non-string output.")

        self.text = cleaned_text


__all__ = [
    'Heading', 'Heading1', 'Heading2', 'Heading3',
    'Heading4', 'Heading5', 'Heading6', 'Heading7',
    'Heading8', 'Heading9', 'Heading10',
    "ListItem2", "ListItem3", "Caption", "Quote",
    'ListContinue', 'ListContinue2', 'ListContinue3',
    'ListNumber', 'ListNumber2', 'ListNumber3', 'ListNumber4',
    'TOCHeading', 'Subtitle', "ListParagraph",
    'ListBullet', 'ListBullet2', 'ListBullet3',
    'TableHeader', 'TableRow', 'TableSeparator', 'CentricTable']