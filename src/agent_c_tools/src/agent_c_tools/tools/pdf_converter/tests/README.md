# PDF Converter Tool - Tests

This directory contains comprehensive tests for the PDF Converter Tool.

## 🧪 Test Structure

```
tests/
├── __init__.py                 # Test package initialization
├── pytest.ini                 # Pytest configuration
├── test-requirements.txt      # Testing dependencies
├── run_tests.py               # Test runner script
├── README.md                  # This file
├── test_pdf_converter.py      # Main test suite
└── test_data/                 # Test data and sample files
    ├── __init__.py
    └── sample_pdfs/           # Sample PDF files for testing
```

## 🚀 Running Tests

### Quick Start
```bash
# Run all tests
python run_tests.py

# Run specific test types
python run_tests.py --unit
python run_tests.py --integration
python run_tests.py --error-handling

# Skip slow tests
python run_tests.py --fast
```

### Using pytest directly
```bash
# Run all tests
pytest

# Run with markers
pytest -m unit
pytest -m integration
pytest -m error_handling

# Run specific test file
pytest test_pdf_converter.py

# Verbose output
pytest -v --tb=short
```

## 📋 Test Categories

### Unit Tests (`-m unit`)
- Core PDF processing functionality
- Text extraction accuracy
- Metadata extraction
- Base64 handling
- JSON output structure

### Integration Tests (`-m integration`)
- Agent C framework integration
- Toolset registration
- Function calling from agents
- Logging integration

### Error Handling Tests (`-m error_handling`)
- Invalid PDF content
- Corrupted files
- Missing parameters
- Base64 decoding errors
- Edge cases and boundary conditions

### Performance Tests (`-m performance`)
- Processing speed benchmarks
- Memory usage validation
- Large file handling
- Concurrent processing

## 🔧 Test Dependencies

Install test dependencies:
```bash
pip install -r test-requirements.txt
```

Required packages:
- `pytest>=7.0.0` - Test framework
- `pytest-asyncio>=0.21.0` - Async test support
- `reportlab>=3.6.0` - PDF creation for testing
- `python-dotenv>=1.0.0` - Environment configuration

## 📊 Test Coverage

The test suite covers:
- ✅ All public methods and functions
- ✅ Error handling scenarios
- ✅ Edge cases and boundary conditions
- ✅ Integration with Agent C framework
- ✅ Performance characteristics
- ✅ Documentation examples

## 🎯 Test Data

### Sample PDFs
The `test_data/` directory contains:
- Simple single-page PDFs
- Multi-page documents
- PDFs with special characters
- Documents with metadata
- Corrupted/invalid files for error testing

### Creating Test Data
```python
# Example: Create test PDF with ReportLab
from reportlab.pdfgen import canvas
from io import BytesIO

buffer = BytesIO()
p = canvas.Canvas(buffer)
p.drawString(100, 750, 'Test PDF Content')
p.save()
pdf_bytes = buffer.getvalue()
```

## 🚨 Troubleshooting

### Common Issues

**PyPDF2 not found**
```bash
pip install PyPDF2==3.0.1
```

**ReportLab not available**
```bash
pip install reportlab
```

**Tests fail with import errors**
- Ensure you're running from the correct directory
- Check that agent_c_tools is properly installed
- Verify Python path includes the tool location

**Async test issues**
- Ensure `pytest-asyncio` is installed
- Check that `asyncio_mode = auto` is in pytest.ini

## 📈 Adding New Tests

### Test File Structure
```python
import pytest
import asyncio
from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools

class TestPDFConverter:
    
    @pytest.mark.unit
    async def test_basic_functionality(self):
        # Test implementation
        pass
    
    @pytest.mark.integration
    async def test_agent_integration(self):
        # Test implementation
        pass
    
    @pytest.mark.error_handling
    async def test_error_scenarios(self):
        # Test implementation
        pass
```

### Test Markers
Always use appropriate markers:
- `@pytest.mark.unit` - Unit tests
- `@pytest.mark.integration` - Integration tests
- `@pytest.mark.error_handling` - Error handling
- `@pytest.mark.performance` - Performance tests
- `@pytest.mark.slow` - Tests that take >5 seconds

## 🎉 Contributing

When adding new tests:
1. Follow the existing test structure
2. Use appropriate markers
3. Include docstrings
4. Test both success and failure cases
5. Update this README if needed

---

*Part of the Agent C Tools ecosystem - Happy testing! 🧪*