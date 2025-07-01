from pdfplumber.pdf import PDF
from pydantic import Field

from agent_c.models.extraction.bounding_box_model import BoundingBoxModel
from agent_c.models.async_observable import AsyncObservableModel
from agent_c.util.observable import ObservableList


class PDFFileModel(AsyncObservableModel):
    """
    Model for a PDF file, containing metadata and the parsed PDF object.
    """
    source_filename: str = Field(...,
                                 description="The filename of the PDF file.")
    pdf: PDF = Field(...,
                     description="The parsed PDF Plumber PDF object.",
                     exclude=True)
    image_resolution: int = Field(150,
                                  description="Resolution for images extracted from the PDF.")
    bounding_boxes: ObservableList[BoundingBoxModel] = Field(default_factory=ObservableList,
                                                             description="List of bounding boxes extracted from the PDF.")

    def add_bounding_box(self, bbox: BoundingBoxModel):
        self.bounding_boxes.append(bbox)
