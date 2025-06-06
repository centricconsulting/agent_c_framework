"""Office to Markdown conversion tools for Agent C."""

from .tool import OfficeToMarkdownTools
from .business_logic.office_converter import OfficeToMarkdownConverter, ConversionResult

__all__ = ['OfficeToMarkdownTools', 'OfficeToMarkdownConverter', 'ConversionResult']