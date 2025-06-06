"""Core business logic for converting office files to markdown using MarkItDown."""

import logging
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
from typing import Dict, Any, Optional, Set

from markitdown import MarkItDown

logger = logging.getLogger(__name__)


@dataclass
class ConversionResult:
    """Result of an office file to markdown conversion."""
    success: bool
    markdown_content: Optional[str] = None
    error_message: Optional[str] = None
    source_file: Optional[str] = None
    file_type: Optional[str] = None
    conversion_metadata: Optional[Dict[str, Any]] = None


class OfficeToMarkdownConverter:
    """
    Business logic class for converting office files to markdown using MarkItDown.
    
    This class handles the core conversion logic extracted from the API FileHandler,
    providing a clean interface for office document conversion with proper error
    handling and logging.
    """
    
    def __init__(self):
        """Initialize the converter with supported file types."""
        self.logger = logging.getLogger(__name__)
        
        # Supported file extensions based on MarkItDown capabilities
        self._supported_extensions = {
            '.docx',  # Word documents
            '.doc',   # Legacy Word documents
            '.pptx',  # PowerPoint presentations
            '.ppt',   # Legacy PowerPoint
            '.xlsx',  # Excel spreadsheets
            '.xls',   # Legacy Excel
            '.pdf',   # PDF documents
            '.html',  # HTML files
            '.htm'    # HTML files (alternative extension)
        }
        
        # Extension to MIME type mapping for reference
        self._extension_to_mime = {
            '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
            '.doc': 'application/msword',
            '.pptx': 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
            '.ppt': 'application/vnd.ms-powerpoint',
            '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            '.xls': 'application/vnd.ms-excel',
            '.pdf': 'application/pdf',
            '.html': 'text/html',
            '.htm': 'text/html'
        }
    
    def get_supported_extensions(self) -> Set[str]:
        """
        Get the set of supported file extensions.
        
        Returns:
            Set of supported file extensions (e.g., {'.docx', '.pdf', ...})
        """
        return self._supported_extensions.copy()
    
    def is_supported_file(self, file_path: str) -> bool:
        """
        Check if a file type is supported for conversion.
        
        Args:
            file_path: Path to the file to check
            
        Returns:
            True if the file type is supported, False otherwise
        """
        file_extension = Path(file_path).suffix.lower()
        return file_extension in self._supported_extensions
    
    def get_file_mime_type(self, file_path: str) -> Optional[str]:
        """
        Get the MIME type for a file based on its extension.
        
        Args:
            file_path: Path to the file
            
        Returns:
            MIME type string or None if not supported
        """
        file_extension = Path(file_path).suffix.lower()
        return self._extension_to_mime.get(file_extension)
    
    def convert_file_to_markdown(self, file_path: str) -> ConversionResult:
        """
        Convert an office file to markdown content using MarkItDown.
        
        This method extracts the core conversion logic from the API FileHandler
        and provides a clean interface for office document conversion.
        
        Args:
            file_path: Absolute path to the office file to convert
            
        Returns:
            ConversionResult with success status, content, and metadata
        """
        try:
            # Validate file exists
            file_obj = Path(file_path)
            if not file_obj.exists():
                error_msg = f"File not found: {file_path}"
                self.logger.error(error_msg)
                return ConversionResult(
                    success=False,
                    error_message=error_msg,
                    source_file=file_path
                )
            
            # Check if file type is supported
            file_extension = file_obj.suffix.lower()
            if not self.is_supported_file(file_path):
                error_msg = f"Unsupported file type: {file_extension}. Supported types: {', '.join(sorted(self._supported_extensions))}"
                self.logger.error(error_msg)
                return ConversionResult(
                    success=False,
                    error_message=error_msg,
                    source_file=file_path,
                    file_type=file_extension
                )
            
            # Perform conversion using MarkItDown
            # This is the core logic extracted from the API FileHandler.process_file method
            self.logger.info(f"Starting conversion of {file_obj.name} using MarkItDown")
            
            md = MarkItDown()
            result = md.convert(file_path)
            
            # Validate conversion result
            if not result or not hasattr(result, 'text_content'):
                error_msg = f"MarkItDown conversion failed: Invalid result object"
                self.logger.error(error_msg)
                return ConversionResult(
                    success=False,
                    error_message=error_msg,
                    source_file=file_path,
                    file_type=file_extension
                )
            
            markdown_content = result.text_content
            if not markdown_content:
                error_msg = f"MarkItDown conversion resulted in empty content"
                self.logger.warning(error_msg)
                return ConversionResult(
                    success=False,
                    error_message=error_msg,
                    source_file=file_path,
                    file_type=file_extension
                )
            
            # Create conversion metadata
            file_stats = file_obj.stat()
            conversion_metadata = {
                'converted_at': datetime.now().isoformat(),
                'source_size_bytes': file_stats.st_size,
                'content_length_chars': len(markdown_content),
                'mime_type': self.get_file_mime_type(file_path),
                'original_filename': file_obj.name
            }
            
            self.logger.info(f"Successfully converted {file_obj.name} to markdown ({len(markdown_content)} characters)")
            
            return ConversionResult(
                success=True,
                markdown_content=markdown_content,
                source_file=file_path,
                file_type=file_extension,
                conversion_metadata=conversion_metadata
            )
            
        except Exception as e:
            error_msg = f"Conversion failed for {file_path}: {str(e)}"
            self.logger.exception(error_msg)
            return ConversionResult(
                success=False,
                error_message=error_msg,
                source_file=file_path,
                file_type=Path(file_path).suffix.lower() if Path(file_path).exists() else None
            )
    
    def get_conversion_info(self) -> Dict[str, Any]:
        """
        Get information about the converter capabilities.
        
        Returns:
            Dictionary with converter information
        """
        return {
            'supported_extensions': sorted(list(self._supported_extensions)),
            'supported_mime_types': sorted(list(self._extension_to_mime.values())),
            'converter_name': 'MarkItDown',
            'total_supported_types': len(self._supported_extensions)
        }