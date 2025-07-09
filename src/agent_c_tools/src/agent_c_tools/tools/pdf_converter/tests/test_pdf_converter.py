#!/usr/bin/env python3
"""
Comprehensive test suite for PDF Converter Tool

Tests cover:
- Core functionality and text extraction
- Metadata extraction and handling
- Error handling scenarios
- Integration with Agent C framework
- Performance characteristics
"""

import pytest
import asyncio
import base64
import json
import sys
from io import BytesIO
from typing import Dict, Any

# Import the tool from production location
from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools


class TestPDFConverterDependencies:
    """Test dependency availability and imports"""
    
    @pytest.mark.unit
    def test_pypdf2_available(self):
        """Test that PyPDF2 is available and functional"""
        try:
            import PyPDF2
            assert hasattr(PyPDF2, '__version__')
            
            # Test basic functionality
            test_data = b"%PDF-1.4\n1 0 obj\n<<\n/Type /Catalog\n/Pages 2 0 R\n>>\nendobj\n2 0 obj\n<<\n/Type /Pages\n/Kids [3 0 R]\n/Count 1\n>>\nendobj\n3 0 obj\n<<\n/Type /Page\n/Parent 2 0 R\n/MediaBox [0 0 612 792]\n>>\nendobj\nxref\n0 4\n0000000000 65535 f \n0000000009 00000 n \n0000000074 00000 n \n0000000120 00000 n \ntrailer\n<<\n/Size 4\n/Root 1 0 R\n>>\nstartxref\n179\n%%EOF"
            pdf_stream = BytesIO(test_data)
            reader = PyPDF2.PdfReader(pdf_stream)
            assert len(reader.pages) >= 0
            
        except ImportError:
            pytest.fail("PyPDF2 not available - install with: pip install PyPDF2==3.0.1")
    
    @pytest.mark.unit
    def test_tool_import(self):
        """Test that the PDF converter tool can be imported"""
        assert PDFConverterTools is not None
        
        # Test initialization
        tool = PDFConverterTools()
        assert tool is not None
        assert hasattr(tool, 'pdf_to_json')


class TestPDFCreation:
    """Utilities for creating test PDFs"""
    
    @staticmethod
    def create_test_pdf_with_reportlab() -> bytes:
        """Create a test PDF using ReportLab if available"""
        try:
            from reportlab.pdfgen import canvas
            from reportlab.lib.pagesizes import letter
            
            buffer = BytesIO()
            p = canvas.Canvas(buffer, pagesize=letter)
            
            # Set metadata
            p.setTitle("Test PDF Document")
            p.setAuthor("PDF Converter Test Suite")
            p.setSubject("Testing PDF to JSON conversion")
            
            # Page 1
            p.drawString(100, 750, 'Hello, this is a test PDF!')
            p.drawString(100, 730, 'This is page 1 content.')
            p.drawString(100, 710, 'Testing special characters: áéíóú ñ ü')
            p.drawString(100, 690, 'Numbers and symbols: 123 $%& @#!')
            p.showPage()
            
            # Page 2
            p.drawString(100, 750, 'This is page 2!')
            p.drawString(100, 730, 'More content on page 2.')
            p.drawString(100, 710, 'Testing longer text that might wrap around...')
            p.drawString(100, 690, 'Line 4 of page 2')
            p.showPage()
            
            # Page 3 - Minimal content
            p.drawString(100, 750, 'Page 3 - minimal content')
            p.showPage()
            
            p.save()
            return buffer.getvalue()
            
        except ImportError:
            pytest.skip("ReportLab not available for PDF creation")
    
    @staticmethod
    def create_minimal_pdf() -> bytes:
        """Create a minimal valid PDF without external dependencies"""
        return b"""%PDF-1.4
1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj
2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj  
3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R>>endobj
4 0 obj<</Length 44>>stream
BT /F1 12 Tf 100 750 Td (Test PDF Content) Tj ET
endstream endobj
xref 0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000130 00000 n 
0000000219 00000 n 
trailer<</Size 5/Root 1 0 R>>
startxref 312 %%EOF"""


class TestPDFConverterCore:
    """Test core PDF conversion functionality"""
    
    @pytest.mark.unit
    async def test_tool_initialization(self):
        """Test tool initialization and post_init"""
        tool = PDFConverterTools()
        await tool.post_init()
        assert tool is not None
    
    @pytest.mark.unit
    async def test_minimal_pdf_conversion(self):
        """Test conversion of a minimal PDF"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        # Create minimal test PDF
        pdf_bytes = TestPDFCreation.create_minimal_pdf()
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        # Test conversion
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True
        )
        
        result_data = json.loads(result)
        
        assert result_data["success"] is True
        assert result_data["total_pages"] >= 1
        assert "content" in result_data
        assert "pages" in result_data["content"]
    
    @pytest.mark.unit
    async def test_page_by_page_extraction(self):
        """Test page-by-page text extraction"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        try:
            pdf_bytes = TestPDFCreation.create_test_pdf_with_reportlab()
        except pytest.skip.Exception:
            pdf_bytes = TestPDFCreation.create_minimal_pdf()
        
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True
        )
        
        result_data = json.loads(result)
        
        assert result_data["success"] is True
        assert "pages" in result_data["content"]
        
        pages = result_data["content"]["pages"]
        assert len(pages) >= 1
        
        for page in pages:
            assert "page_number" in page
            assert "text" in page
            assert "character_count" in page
            assert isinstance(page["character_count"], int)
    
    @pytest.mark.unit
    async def test_full_text_extraction(self):
        """Test full document text extraction"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        try:
            pdf_bytes = TestPDFCreation.create_test_pdf_with_reportlab()
        except pytest.skip.Exception:
            pdf_bytes = TestPDFCreation.create_minimal_pdf()
        
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=False,
            extract_by_page=False
        )
        
        result_data = json.loads(result)
        
        assert result_data["success"] is True
        assert "full_text" in result_data["content"]
        assert "character_count" in result_data["content"]
        assert isinstance(result_data["content"]["character_count"], int)
        assert len(result_data.get("metadata", {})) == 0  # No metadata requested


class TestPDFConverterErrorHandling:
    """Test error handling scenarios"""
    
    @pytest.mark.error_handling
    async def test_no_pdf_content(self):
        """Test handling of missing PDF content"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        result = await tool.pdf_to_json()
        result_data = json.loads(result)
        
        assert result_data["success"] is False
        assert "No PDF content" in result_data.get("error", "")
    
    @pytest.mark.error_handling
    async def test_invalid_base64(self):
        """Test handling of invalid base64 content"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        result = await tool.pdf_to_json(pdf_content="invalid_base64_content!@#$%^&*()")
        result_data = json.loads(result)
        
        assert result_data["success"] is False
        error_msg = result_data.get("error", "")
        assert ("Invalid base64" in error_msg or "Could not read PDF" in error_msg)
    
    @pytest.mark.error_handling
    async def test_invalid_pdf_content(self):
        """Test handling of valid base64 but invalid PDF content"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        fake_content = base64.b64encode(b"This is just text, not a PDF file").decode('utf-8')
        result = await tool.pdf_to_json(pdf_content=fake_content)
        result_data = json.loads(result)
        
        assert result_data["success"] is False
        assert "error" in result_data
    
    @pytest.mark.error_handling
    async def test_empty_base64(self):
        """Test handling of empty base64 content"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        result = await tool.pdf_to_json(pdf_content="")
        result_data = json.loads(result)
        
        assert result_data["success"] is False


class TestPDFConverterIntegration:
    """Test integration with Agent C framework"""
    
    @pytest.mark.integration
    def test_toolset_inheritance(self):
        """Test that the tool properly inherits from Toolset"""
        from agent_c.toolsets import Toolset
        
        tool = PDFConverterTools()
        assert isinstance(tool, Toolset)
    
    @pytest.mark.integration
    def test_json_schema_decoration(self):
        """Test that the pdf_to_json method has proper JSON schema decoration"""
        tool = PDFConverterTools()
        
        # Check that the method has the json_schema decoration
        assert hasattr(tool.pdf_to_json, 'schema')
    
    @pytest.mark.integration
    async def test_tool_context_handling(self):
        """Test that the tool properly handles tool_context parameter"""
        tool = PDFConverterTools()
        await tool.post_init()
        
        pdf_bytes = TestPDFCreation.create_minimal_pdf()
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        # Mock tool context
        mock_context = {
            'session_id': 'test_session',
            'current_user_username': 'test_user',
            'timestamp': '2024-01-15T10:30:00.123456'
        }
        
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            tool_context=mock_context
        )
        
        result_data = json.loads(result)
        assert result_data["success"] is True


class TestPDFConverterPerformance:
    """Test performance characteristics"""
    
    @pytest.mark.performance
    @pytest.mark.slow
    async def test_processing_speed(self):
        """Test PDF processing speed for typical documents"""
        import time
        
        tool = PDFConverterTools()
        await tool.post_init()
        
        try:
            pdf_bytes = TestPDFCreation.create_test_pdf_with_reportlab()
        except pytest.skip.Exception:
            pdf_bytes = TestPDFCreation.create_minimal_pdf()
        
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        # Measure processing time
        start_time = time.time()
        result = await tool.pdf_to_json(pdf_content=pdf_base64)
        end_time = time.time()
        
        processing_time = end_time - start_time
        result_data = json.loads(result)
        
        assert result_data["success"] is True
        assert processing_time < 5.0  # Should process in under 5 seconds
        
        print(f"Processing time: {processing_time:.3f} seconds")


# Test runner for standalone execution
if __name__ == "__main__":
    pytest.main([__file__, "-v", "--tb=short"])