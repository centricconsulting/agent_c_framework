<<<<<<< Updated upstream
"""
PDF Converter Tool for Agent C

A robust PDF to JSON converter tool that extracts text content, metadata, 
and structure from PDF files and outputs structured JSON data.

Features:
- Text extraction from single or multi-page PDFs
- Safe metadata extraction (title, author, dates, etc.)
- Flexible output modes (page-by-page or full text)
- Base64 input support for easy integration
- Robust error handling for various PDF formats

Dependencies:
- PyPDF2==3.0.1

Usage:
    from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
    
    tool = PDFConverterTools()
    await tool.post_init()
    
    result = await tool.pdf_to_json(
        pdf_content=pdf_base64,
        include_metadata=True,
        extract_by_page=True
    )
"""

from .tool import PDFConverterTools

__all__ = ['PDFConverterTools']
__version__ = '1.0.0'
=======
from .tool import PDFConverterTools
>>>>>>> Stashed changes
