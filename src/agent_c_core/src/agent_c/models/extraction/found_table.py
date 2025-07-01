from pydantic import Field

from agent_c.models.base import BaseModel
from pdfplumber.table import Table
from pdfplumber.page import Page

class FoundTableModel(BaseModel):
    page_no: int = Field(..., description="Page number where the table is found")
    table: Table = Field(..., description="The pdfplumber table object found on the page")
    page: Page = Field(..., description="The pdfplumber page object containing the table")
