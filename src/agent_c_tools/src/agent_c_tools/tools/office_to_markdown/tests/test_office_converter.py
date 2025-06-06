"""Unit tests for the OfficeToMarkdownConverter business logic."""

import unittest
from pathlib import Path
from unittest.mock import Mock, patch, MagicMock

from ..business_logic.office_converter import OfficeToMarkdownConverter, ConversionResult


class TestOfficeToMarkdownConverter(unittest.TestCase):
    """Test cases for the OfficeToMarkdownConverter class."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.converter = OfficeToMarkdownConverter()
    
    def test_get_supported_extensions(self):
        """Test that supported extensions are returned correctly."""
        extensions = self.converter.get_supported_extensions()
        
        # Check that all expected extensions are present
        expected_extensions = {'.docx', '.doc', '.pptx', '.ppt', '.xlsx', '.xls', '.pdf', '.html', '.htm'}
        self.assertEqual(extensions, expected_extensions)
        
        # Ensure it returns a copy (not the original set)
        extensions.add('.fake')
        original_extensions = self.converter.get_supported_extensions()
        self.assertNotIn('.fake', original_extensions)
    
    def test_is_supported_file(self):
        """Test file type support detection."""
        # Test supported files
        self.assertTrue(self.converter.is_supported_file('document.docx'))
        self.assertTrue(self.converter.is_supported_file('presentation.pptx'))
        self.assertTrue(self.converter.is_supported_file('spreadsheet.xlsx'))
        self.assertTrue(self.converter.is_supported_file('document.pdf'))
        self.assertTrue(self.converter.is_supported_file('page.html'))
        
        # Test case insensitivity
        self.assertTrue(self.converter.is_supported_file('DOCUMENT.DOCX'))
        self.assertTrue(self.converter.is_supported_file('Document.Pdf'))
        
        # Test unsupported files
        self.assertFalse(self.converter.is_supported_file('document.txt'))
        self.assertFalse(self.converter.is_supported_file('image.jpg'))
        self.assertFalse(self.converter.is_supported_file('archive.zip'))
    
    def test_get_file_mime_type(self):
        """Test MIME type detection."""
        # Test known MIME types
        self.assertEqual(
            self.converter.get_file_mime_type('doc.docx'),
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
        )
        self.assertEqual(
            self.converter.get_file_mime_type('presentation.pptx'),
            'application/vnd.openxmlformats-officedocument.presentationml.presentation'
        )
        self.assertEqual(
            self.converter.get_file_mime_type('document.pdf'),
            'application/pdf'
        )
        self.assertEqual(
            self.converter.get_file_mime_type('page.html'),
            'text/html'
        )
        
        # Test unknown file type
        self.assertIsNone(self.converter.get_file_mime_type('unknown.xyz'))
    
    def test_convert_file_not_found(self):
        """Test conversion with non-existent file."""
        result = self.converter.convert_file_to_markdown('/nonexistent/file.docx')
        
        self.assertFalse(result.success)
        self.assertIn('File not found', result.error_message)
        self.assertEqual(result.source_file, '/nonexistent/file.docx')
        self.assertIsNone(result.markdown_content)
    
    def test_convert_unsupported_file_type(self):
        """Test conversion with unsupported file type."""
        with patch('pathlib.Path.exists', return_value=True):
            result = self.converter.convert_file_to_markdown('/path/to/file.txt')
            
            self.assertFalse(result.success)
            self.assertIn('Unsupported file type', result.error_message)
            self.assertIn('.txt', result.error_message)
            self.assertEqual(result.file_type, '.txt')
    
    @patch('agent_c_tools.tools.office_to_markdown.business_logic.office_converter.MarkItDown')
    def test_successful_conversion(self, mock_markitdown_class):
        """Test successful file conversion."""
        # Mock the MarkItDown conversion
        mock_markitdown = Mock()
        mock_result = Mock()
        mock_result.text_content = "# Test Document\n\nThis is converted markdown content."
        mock_markitdown.convert.return_value = mock_result
        mock_markitdown_class.return_value = mock_markitdown
        
        # Mock file system operations
        with patch('pathlib.Path.exists', return_value=True), \
             patch('pathlib.Path.stat') as mock_stat:
            
            mock_stat.return_value.st_size = 1024
            
            result = self.converter.convert_file_to_markdown('/path/to/document.docx')
            
            # Verify successful conversion
            self.assertTrue(result.success)
            self.assertEqual(result.markdown_content, "# Test Document\n\nThis is converted markdown content.")
            self.assertEqual(result.source_file, '/path/to/document.docx')
            self.assertEqual(result.file_type, '.docx')
            self.assertIsNotNone(result.conversion_metadata)
            
            # Verify metadata
            metadata = result.conversion_metadata
            self.assertEqual(metadata['source_size_bytes'], 1024)
            self.assertEqual(metadata['content_length_chars'], len(result.markdown_content))
            self.assertIn('converted_at', metadata)
            self.assertIn('mime_type', metadata)
    
    @patch('agent_c_tools.tools.office_to_markdown.business_logic.office_converter.MarkItDown')
    def test_conversion_empty_result(self, mock_markitdown_class):
        """Test conversion with empty result."""
        # Mock MarkItDown to return empty content
        mock_markitdown = Mock()
        mock_result = Mock()
        mock_result.text_content = ""
        mock_markitdown.convert.return_value = mock_result
        mock_markitdown_class.return_value = mock_markitdown
        
        with patch('pathlib.Path.exists', return_value=True):
            result = self.converter.convert_file_to_markdown('/path/to/document.docx')
            
            self.assertFalse(result.success)
            self.assertIn('empty content', result.error_message)
    
    @patch('agent_c_tools.tools.office_to_markdown.business_logic.office_converter.MarkItDown')
    def test_conversion_exception(self, mock_markitdown_class):
        """Test conversion with MarkItDown exception."""
        # Mock MarkItDown to raise an exception
        mock_markitdown = Mock()
        mock_markitdown.convert.side_effect = Exception("MarkItDown conversion error")
        mock_markitdown_class.return_value = mock_markitdown
        
        with patch('pathlib.Path.exists', return_value=True):
            result = self.converter.convert_file_to_markdown('/path/to/document.docx')
            
            self.assertFalse(result.success)
            self.assertIn('Conversion failed', result.error_message)
            self.assertIn('MarkItDown conversion error', result.error_message)
    
    def test_get_conversion_info(self):
        """Test conversion info retrieval."""
        info = self.converter.get_conversion_info()
        
        self.assertIn('supported_extensions', info)
        self.assertIn('supported_mime_types', info)
        self.assertIn('converter_name', info)
        self.assertIn('total_supported_types', info)
        
        self.assertEqual(info['converter_name'], 'MarkItDown')
        self.assertEqual(info['total_supported_types'], 9)  # 9 supported extensions
        self.assertIsInstance(info['supported_extensions'], list)
        self.assertIsInstance(info['supported_mime_types'], list)


if __name__ == '__main__':
    unittest.main()