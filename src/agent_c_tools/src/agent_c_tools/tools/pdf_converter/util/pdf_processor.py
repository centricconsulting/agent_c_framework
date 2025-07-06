import yaml
import logging
from io import BytesIO
from typing import Dict, Any
from datetime import datetime

try:
    import PyPDF2
except ImportError:
    PyPDF2 = None


class PDFProcessor:
    """
    PDF processor that extracts text content, metadata, 
    and structure from PDF files and outputs structured YAML.
    """

    def __init__(self):
        self.logger = logging.getLogger(__name__)

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

    async def process_pdf_file(self, file_path: str, include_metadata: bool = True,
                               extract_by_page: bool = True) -> str:
        """
        Convert PDF file to structured YAML format.

        Args:
            file_path: Path to the PDF file
            include_metadata: Include PDF metadata in output
            extract_by_page: Extract text separated by pages vs all text together

        Returns:
            YAML string with structured PDF data
        """
        try:
            if not file_path:
                return yaml.dump({
                    "error": "No file path provided",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
                })

            if PyPDF2 is None:
                return yaml.dump({
                    "error": "PyPDF2 library not installed",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
                })

            # Read PDF file from path
            try:
                with open(file_path, 'rb') as f:
                    pdf_bytes = f.read()
            except FileNotFoundError:
                return yaml.dump({
                    "error": f"File not found: {file_path}",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
                })
            except PermissionError:
                return yaml.dump({
                    "error": f"Permission denied accessing file: {file_path}",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
                })
            except Exception as e:
                return yaml.dump({
                    "error": f"Error reading file: {str(e)}",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
                })

            # Create PDF reader with warnings suppressed
            pdf_stream = BytesIO(pdf_bytes)
            try:
                pdf_reader = PyPDF2.PdfReader(pdf_stream, strict=False)  # Less strict parsing
            except Exception as e:
                return yaml.dump({
                    "error": f"Could not read PDF: {str(e)}",
                    "success": False,
                    "extracted_at": datetime.now().isoformat()
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

            return yaml.dump(result, default_flow_style=False, sort_keys=False, allow_unicode=True)

        except Exception as e:
            error_msg = f"Error converting PDF to YAML: {str(e)}"
            self.logger.error(error_msg, exc_info=True)

            return yaml.dump({
                "error": error_msg,
                "success": False,
                "extracted_at": datetime.now().isoformat()
            }, default_flow_style=False, sort_keys=False, allow_unicode=True)