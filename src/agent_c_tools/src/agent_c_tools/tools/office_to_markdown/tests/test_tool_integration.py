"""Integration tests for the OfficeToMarkdownTools tool interface."""

import json
import unittest
import yaml
from unittest.mock import Mock, patch, AsyncMock, MagicMock

from ..tool import OfficeToMarkdownTools
from ..business_logic.office_converter import ConversionResult


class TestOfficeToMarkdownToolsIntegration(unittest.IsolatedAsyncioTestCase):
    """Integration test cases for the OfficeToMarkdownTools class."""
    
    async def asyncSetUp(self):
        """Set up test fixtures."""
        self.tool = OfficeToMarkdownTools()
        
        # Mock workspace tool
        self.mock_workspace_tool = AsyncMock()
        self.tool.workspace_tool = self.mock_workspace_tool
        
        # Mock converter
        self.mock_converter = Mock()
        self.tool.converter = self.mock_converter
        
        # Mock tool chest for media events
        self.tool.tool_chest = Mock()
        self.tool._raise_render_media = AsyncMock()
    
    def _parse_yaml_response(self, response: str) -> dict:
        """Helper to parse YAML response."""
        return yaml.safe_load(response)
    
    async def test_convert_office_to_markdown_missing_required_fields(self):
        """Test conversion with missing required fields."""
        # Test missing input_workspace
        result = await self.tool.convert_office_to_markdown(
            input_path="document.docx",
            output_path="document.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Required fields cannot be empty', response['message'])
        
        # Test missing input_path
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            output_path="document.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Required fields cannot be empty', response['message'])
        
        # Test missing output_path
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.docx"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Required fields cannot be empty', response['message'])
    
    async def test_convert_office_to_markdown_file_read_error(self):
        """Test conversion when input file cannot be read."""
        # Mock workspace tool to return error
        self.mock_workspace_tool.read.return_value = '{"error": "File not found"}'
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="nonexistent.docx",
            output_path="output.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Error reading input file', response['message'])
        self.assertIn('File not found', response['message'])
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_path_resolution_error(self, mock_os_path):
        """Test conversion when file system path cannot be resolved."""
        # Mock successful file read but failed path resolution
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.return_value = None
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.docx",
            output_path="output.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Could not resolve file system path', response['message'])
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_conversion_failure(self, mock_os_path):
        """Test conversion when business logic fails."""
        # Mock successful file read and path resolution
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.return_value = "/path/to/file.docx"
        
        # Mock converter to return failure
        self.mock_converter.convert_file_to_markdown.return_value = ConversionResult(
            success=False,
            error_message="Unsupported file type: .xyz",
            file_type=".xyz"
        )
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.xyz",
            output_path="output.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertEqual(response['message'], "Unsupported file type: .xyz")
        self.assertEqual(response['file_type'], ".xyz")
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_write_error(self, mock_os_path):
        """Test conversion when output file cannot be written."""
        # Mock successful file read, path resolution, and conversion
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.return_value = "/path/to/file.docx"
        
        self.mock_converter.convert_file_to_markdown.return_value = ConversionResult(
            success=True,
            markdown_content="# Converted Content\n\nThis is the converted markdown.",
            file_type=".docx"
        )
        
        # Mock write failure
        self.mock_workspace_tool.write.return_value = '{"error": "Permission denied"}'
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.docx",
            output_path="output.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertIn('Failed to write output file', response['message'])
        self.assertIn('Permission denied', response['message'])
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_success(self, mock_os_path):
        """Test successful office file conversion."""
        # Mock successful file read and path resolution
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.side_effect = ["/path/to/input.docx", "/path/to/output.md"]
        
        # Mock successful conversion
        conversion_metadata = {
            'converted_at': '2024-01-01T12:00:00',
            'source_size_bytes': 1024,
            'content_length_chars': 45
        }
        
        self.mock_converter.convert_file_to_markdown.return_value = ConversionResult(
            success=True,
            markdown_content="# Converted Content\n\nThis is the converted markdown.",
            file_type=".docx",
            conversion_metadata=conversion_metadata
        )
        
        # Mock successful write
        self.mock_workspace_tool.write.return_value = '{"success": true}'
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.docx",
            output_path="output.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertTrue(response['success'])
        self.assertEqual(response['message'], "Office file converted to markdown successfully")
        self.assertEqual(response['input_file'], "//docs/document.docx")
        self.assertEqual(response['output_file'], "//docs/output.md")  # defaults to input workspace
        self.assertEqual(response['file_type'], ".docx")
        self.assertEqual(response['conversion_metadata'], conversion_metadata)
        self.assertEqual(response['os_path'], "/path/to/output.md")
        
        # Verify media event was raised
        self.tool._raise_render_media.assert_called_once()
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_with_different_output_workspace(self, mock_os_path):
        """Test conversion with different output workspace."""
        # Mock successful operations
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.side_effect = ["/path/to/input.docx", "/path/to/output.md"]
        
        self.mock_converter.convert_file_to_markdown.return_value = ConversionResult(
            success=True,
            markdown_content="# Content",
            file_type=".docx"
        )
        
        self.mock_workspace_tool.write.return_value = '{"success": true}'
        
        result = await self.tool.convert_office_to_markdown(
            input_workspace="source_docs",
            input_path="document.docx",
            output_workspace="converted_docs",
            output_path="document.md"
        )
        
        response = self._parse_yaml_response(result)
        self.assertTrue(response['success'])
        self.assertEqual(response['input_file'], "//source_docs/document.docx")
        self.assertEqual(response['output_file'], "//converted_docs/document.md")
    
    @patch('agent_c_tools.tools.office_to_markdown.tool.ensure_file_extension')
    @patch('agent_c_tools.tools.office_to_markdown.tool.os_file_system_path')
    async def test_convert_office_to_markdown_ensures_md_extension(self, mock_os_path, mock_ensure_ext):
        """Test that output path gets .md extension."""
        # Mock the ensure_file_extension function
        mock_ensure_ext.return_value = "output.md"
        
        # Mock successful operations
        self.mock_workspace_tool.read.return_value = "file content"
        mock_os_path.side_effect = ["/path/to/input.docx", "/path/to/output.md"]
        
        self.mock_converter.convert_file_to_markdown.return_value = ConversionResult(
            success=True,
            markdown_content="# Content",
            file_type=".docx"
        )
        
        self.mock_workspace_tool.write.return_value = '{"success": true}'
        
        await self.tool.convert_office_to_markdown(
            input_workspace="docs",
            input_path="document.docx",
            output_path="output"  # No extension
        )
        
        # Verify ensure_file_extension was called
        mock_ensure_ext.assert_called_once_with("output", "md")
    
    async def test_get_conversion_info_success(self):
        """Test successful retrieval of conversion info."""
        # Mock converter info
        converter_info = {
            'supported_extensions': ['.docx', '.pptx', '.xlsx', '.pdf'],
            'supported_mime_types': ['application/pdf', 'text/html'],
            'converter_name': 'MarkItDown',
            'total_supported_types': 4
        }
        
        self.mock_converter.get_conversion_info.return_value = converter_info
        
        result = await self.tool.get_conversion_info()
        
        response = self._parse_yaml_response(result)
        self.assertTrue(response['success'])
        self.assertEqual(response['message'], "Converter information retrieved successfully")
        self.assertEqual(response['supported_extensions'], converter_info['supported_extensions'])
        self.assertEqual(response['converter_name'], 'MarkItDown')
        self.assertEqual(response['total_supported_types'], 4)
    
    async def test_get_conversion_info_no_converter(self):
        """Test conversion info when converter is not initialized."""
        self.tool.converter = None
        
        result = await self.tool.get_conversion_info()
        
        response = self._parse_yaml_response(result)
        self.assertFalse(response['success'])
        self.assertEqual(response['message'], "Converter not initialized")
    
    async def test_format_response_helper(self):
        """Test the _format_response helper method."""
        # Test success response
        result = self.tool._format_response(True, "Success message", extra_data="test")
        response = self._parse_yaml_response(result)
        
        self.assertTrue(response['success'])
        self.assertEqual(response['message'], "Success message")
        self.assertEqual(response['extra_data'], "test")
        
        # Test failure response without message
        result = self.tool._format_response(False, extra_info="error details")
        response = self._parse_yaml_response(result)
        
        self.assertFalse(response['success'])
        self.assertNotIn('message', response)
        self.assertEqual(response['extra_info'], "error details")


if __name__ == '__main__':
    unittest.main()