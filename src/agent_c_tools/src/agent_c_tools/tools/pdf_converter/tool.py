import json
import base64
import logging
from io import BytesIO
from typing import Dict, Any
from datetime import datetime

try:
    import PyPDF2
except ImportError:
    PyPDF2 = None

from agent_c.toolsets import Toolset, json_schema


class PDFConverterTools(Toolset):
    """
    PDF to JSON converter tool that extracts text content, metadata, 
    and structure from PDF files and outputs structured JSON.
    """
    
    def __init__(self, **kwargs):
        super().__init__(name='pdf_converter', **kwargs)
        self.logger = logging.getLogger(__name__)
        
    async def post_init(self):
        """Optional post-initialization setup."""
        if PyPDF2 is None:
            self.logger.warning("PyPDF2 not installed - PDF conversion will not work")

    def _safe_extract_metadata(self, pdf_reader):
        """Safely extract metadata, handling various format issues."""
        metadata = {}
        
        if not pdf_reader.metadata:
            return metadata
            
        try:
            # Extract string metadata safely
            if pdf_reader.metadata.title:
                metadata["title"] = str(pdf_reader.metadata.title)
        except Exception as e:
            self.logger.debug(f"Could not extract title: {e}")
            
        try:
            if pdf_reader.metadata.author:
                metadata["author"] = str(pdf_reader.metadata.author)
        except Exception as e:
            self.logger.debug(f"Could not extract author: {e}")
            
        try:
            if pdf_reader.metadata.subject:
                metadata["subject"] = str(pdf_reader.metadata.subject)
        except Exception as e:
            self.logger.debug(f"Could not extract subject: {e}")
            
        try:
            if pdf_reader.metadata.creator:
                metadata["creator"] = str(pdf_reader.metadata.creator)
        except Exception as e:
            self.logger.debug(f"Could not extract creator: {e}")
            
        # Handle dates more carefully
        try:
            if hasattr(pdf_reader.metadata, 'creation_date') and pdf_reader.metadata.creation_date:
                metadata["creation_date"] = str(pdf_reader.metadata.creation_date)
        except Exception as e:
            self.logger.debug(f"Could not extract creation_date: {e}")
            # Try to get raw creation date string
            try:
                raw_date = pdf_reader.metadata.get('/CreationDate')
                if raw_date:
                    metadata["creation_date_raw"] = str(raw_date)
            except:
                pass
                
        try:
            if hasattr(pdf_reader.metadata, 'modification_date') and pdf_reader.metadata.modification_date:
                metadata["modification_date"] = str(pdf_reader.metadata.modification_date)
        except Exception as e:
            self.logger.debug(f"Could not extract modification_date: {e}")
            # Try to get raw modification date string
            try:
                raw_date = pdf_reader.metadata.get('/ModDate')
                if raw_date:
                    metadata["modification_date_raw"] = str(raw_date)
            except:
                pass
                
        return metadata

    @json_schema(
        description="Convert PDF file to structured JSON format with text content and metadata",
        params={
            "pdf_content": {
                "type": "string",
                "description": "Base64 encoded PDF content",
                "required": True
            },
            "include_metadata": {
                "type": "boolean", 
                "description": "Include PDF metadata in output (title, author, etc.)",
                "required": False,
                "default": True
            },
            "extract_by_page": {
                "type": "boolean",
                "description": "Extract text separated by pages vs all text together", 
                "required": False,
                "default": True
            }
        }
    )
    async def pdf_to_json(self, **kwargs) -> str:
        """
        Convert PDF content to structured JSON format.
        """
        try:
            # Get parameters
            pdf_content = kwargs.get("pdf_content")
            include_metadata = kwargs.get("include_metadata", True)
            extract_by_page = kwargs.get("extract_by_page", True)
            tool_context = kwargs.get("tool_context", {})
            
            if not pdf_content:
                return json.dumps({
                    "error": "No PDF content provided",
                    "success": False
                })
                
            if PyPDF2 is None:
                return json.dumps({
                    "error": "PyPDF2 library not installed",
                    "success": False
                })
            
            # Decode base64 content
            try:
                pdf_bytes = base64.b64decode(pdf_content)
            except Exception as e:
                return json.dumps({
                    "error": f"Invalid base64 content: {str(e)}",
                    "success": False
                })
            
            # Create PDF reader with warnings suppressed
            pdf_stream = BytesIO(pdf_bytes)
            try:
                pdf_reader = PyPDF2.PdfReader(pdf_stream, strict=False)  # Less strict parsing
            except Exception as e:
                return json.dumps({
                    "error": f"Could not read PDF: {str(e)}",
                    "success": False
                })
            
            # Prepare result structure
            result = {
                "success": True,
                "extracted_at": datetime.now().isoformat(),
                "total_pages": len(pdf_reader.pages),
                "content": {},
                "metadata": {}
            }
            
            # Extract metadata if requested (using safe method)
            if include_metadata:
                result["metadata"] = self._safe_extract_metadata(pdf_reader)
            
            # Extract text content
            if extract_by_page:
                # Extract text page by page
                pages = []
                for page_num, page in enumerate(pdf_reader.pages, 1):
                    try:
                        text = page.extract_text()
                        pages.append({
                            "page_number": page_num,
                            "text": text.strip() if text else "",
                            "character_count": len(text) if text else 0
                        })
                    except Exception as e:
                        pages.append({
                            "page_number": page_num,
                            "text": "",
                            "character_count": 0,
                            "error": f"Failed to extract: {str(e)}"
                        })
                        
                result["content"]["pages"] = pages
            else:
                # Extract all text together
                all_text = ""
                for page in pdf_reader.pages:
                    try:
                        text = page.extract_text()
                        if text:
                            all_text += text + "\n"
                    except Exception as e:
                        self.logger.warning(f"Failed to extract text from page: {str(e)}")
                        
                result["content"]["full_text"] = all_text.strip()
                result["content"]["character_count"] = len(all_text)
            
            # Log success
            self.logger.info(f"Successfully converted PDF with {result['total_pages']} pages")
            
            return json.dumps(result, ensure_ascii=False, indent=2)
            
        except Exception as e:
            error_msg = f"Error converting PDF to JSON: {str(e)}"
            self.logger.error(error_msg, exc_info=True)
            
            return json.dumps({
                "error": error_msg,
                "success": False,
                "extracted_at": datetime.now().isoformat()
            })


# Register the toolset
Toolset.register(PDFConverterTools)
