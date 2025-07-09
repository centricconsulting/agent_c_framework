# PDF Converter Tool for Agent C

A robust PDF to JSON converter tool that integrates seamlessly with the Agent C framework. This tool extracts text content, metadata, and structure from PDF files and outputs structured JSON data.

## 🎯 Overview

The PDF Converter Tool provides AI agents with the ability to process PDF documents by:
- Extracting text content from single or multi-page PDFs
- Preserving document structure with page-by-page extraction
- Safely extracting PDF metadata (title, author, creation date, etc.)
- Handling various PDF formats and encoding issues gracefully
- Providing structured JSON output for easy consumption by other tools

## ✨ Features

### Core Functionality
- **Text Extraction**: Extract text from PDF documents with support for special characters
- **Metadata Extraction**: Safely extract PDF metadata including title, author, subject, creator, and dates
- **Flexible Output**: Choose between page-by-page extraction or full document text
- **Error Handling**: Robust error handling for invalid PDFs, corrupted files, and encoding issues
- **Base64 Input**: Accepts PDF content as base64-encoded strings for easy integration

### Agent C Integration
- **Toolset Compliance**: Fully compliant with Agent C toolset architecture
- **Async Support**: Asynchronous operations for non-blocking PDF processing
- **JSON Schema**: Properly decorated with `@json_schema` for LLM function calling
- **Logging**: Integrated with Agent C logging system for debugging and monitoring

## 🔧 Installation & Dependencies

### Required Dependencies
```bash
pip install PyPDF2==3.0.1
```

### Optional Dependencies (for testing)
```bash
pip install reportlab  # For creating test PDFs
```

### Agent C Framework
This tool requires the Agent C framework to be installed and configured.

## 📖 Usage

### Basic Usage

```python
from agent_c_demo.pdf_converter.tool import PDFConverterTools
import base64
import json

# Initialize the tool
tool = PDFConverterTools()
await tool.post_init()

# Read PDF file and encode as base64
with open("document.pdf", "rb") as f:
    pdf_bytes = f.read()
    pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')

# Convert PDF to JSON
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    include_metadata=True,
    extract_by_page=True
)

# Parse the result
result_data = json.loads(result)
if result_data["success"]:
    print(f"Extracted {result_data['total_pages']} pages")
    for page in result_data['content']['pages']:
        print(f"Page {page['page_number']}: {page['character_count']} characters")
```

### Agent Function Calling

When used by an AI agent, the tool can be called via function calling:

```json
{
  "name": "pdf_converter-pdf_to_json",
  "arguments": {
    "pdf_content": "JVBERi0xLjQKMSAwIG9iago8PAovVHlwZSAvQ2F0YWxvZwovUGFnZXMgMiAwIFIKPj4KZW5kb2JqCg==",
    "include_metadata": true,
    "extract_by_page": true
  }
}
```

## 📋 API Reference

### `pdf_to_json(**kwargs) -> str`

Converts PDF content to structured JSON format.

#### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `pdf_content` | string | Yes | - | Base64 encoded PDF content |
| `include_metadata` | boolean | No | `true` | Include PDF metadata in output |
| `extract_by_page` | boolean | No | `true` | Extract text separated by pages vs all text together |

#### Returns

Returns a JSON string with the following structure:

```json
{
  "success": true,
  "extracted_at": "2024-01-15T10:30:00.123456",
  "total_pages": 3,
  "content": {
    "pages": [
      {
        "page_number": 1,
        "text": "Page content here...",
        "character_count": 156
      }
    ]
  },
  "metadata": {
    "title": "Document Title",
    "author": "Author Name",
    "subject": "Document Subject",
    "creator": "PDF Creator",
    "creation_date": "2024-01-01T00:00:00",
    "modification_date": "2024-01-15T10:00:00"
  }
}
```

#### Error Response

```json
{
  "success": false,
  "error": "Error description here",
  "extracted_at": "2024-01-15T10:30:00.123456"
}
```

## 🛡️ Error Handling

The tool provides robust error handling for common scenarios:

### Supported Error Cases
- **Missing PDF Content**: Returns clear error when no content is provided
- **Invalid Base64**: Handles malformed base64 encoding gracefully
- **Corrupted PDFs**: Manages corrupted or invalid PDF files
- **Encoding Issues**: Handles various text encoding problems
- **Metadata Extraction Failures**: Safely extracts available metadata even if some fields fail

### Error Response Format
All errors return a consistent JSON structure:
```json
{
  "success": false,
  "error": "Descriptive error message",
  "extracted_at": "ISO timestamp"
}
```

## 🧪 Testing

### Running Tests

The tool includes comprehensive tests to verify functionality:

```bash
# Basic functionality test
python test_pdf_tool.py

# Comprehensive test suite
python test_pdf_comprehensive.py

# Environment check
python check_environment.py
```

### Test Coverage
- ✅ Dependency availability (PyPDF2)
- ✅ Basic PDF creation and parsing
- ✅ Multi-page document handling
- ✅ Special character support (áéíóú ñ ü)
- ✅ Metadata extraction
- ✅ Error handling scenarios
- ✅ Both extraction modes (page-by-page and full text)

## 🔍 Examples

### Example 1: Page-by-Page Extraction

```python
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    include_metadata=True,
    extract_by_page=True
)

# Access individual pages
result_data = json.loads(result)
for page in result_data['content']['pages']:
    print(f"Page {page['page_number']}: {page['text']}")
```

### Example 2: Full Text Extraction

```python
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    include_metadata=False,
    extract_by_page=False
)

# Access full document text
result_data = json.loads(result)
full_text = result_data['content']['full_text']
print(f"Document contains {result_data['content']['character_count']} characters")
```

### Example 3: Metadata Only

```python
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    include_metadata=True,
    extract_by_page=True
)

result_data = json.loads(result)
metadata = result_data['metadata']
print(f"Title: {metadata.get('title', 'Unknown')}")
print(f"Author: {metadata.get('author', 'Unknown')}")
print(f"Created: {metadata.get('creation_date', 'Unknown')}")
```

## 🚀 Performance Considerations

### Memory Usage
- The tool loads entire PDF content into memory during processing
- For large PDFs (>10MB), consider processing in chunks or implementing streaming
- Base64 encoding increases memory usage by ~33%

### Processing Speed
- Typical processing time: 50-200ms for documents under 1MB
- Processing time scales roughly linearly with document size
- Metadata extraction adds minimal overhead

### Recommendations
- **Small PDFs (<1MB)**: Process directly as shown in examples
- **Medium PDFs (1-10MB)**: Monitor memory usage, consider async processing
- **Large PDFs (>10MB)**: Consider implementing pagination or streaming in future versions

## 🔮 Future Enhancements

The current implementation provides solid core functionality. Potential future enhancements include:

### Planned Features
- **Image Extraction**: Extract embedded images from PDFs
- **Table Detection**: Identify and extract tabular data
- **OCR Integration**: Process scanned PDFs with OCR
- **Streaming Support**: Handle large files without loading entirely into memory
- **Format Options**: Support additional output formats (YAML, XML)

### Advanced Features
- **Layout Preservation**: Maintain document layout and formatting
- **Annotation Extraction**: Extract comments, highlights, and annotations
- **Form Data**: Extract form field data and values
- **Digital Signatures**: Validate and extract signature information

## 🤝 Contributing

This tool is part of the Agent C framework ecosystem. To contribute:

1. Follow Agent C toolset development guidelines
2. Ensure all tests pass before submitting changes
3. Add tests for new functionality
4. Update documentation for new features

## 📄 License

This tool is licensed under the same terms as the Agent C framework.

---

## 🎉 Getting Started

Ready to use the PDF Converter Tool? Here's a quick start:

1. **Install dependencies**: `pip install PyPDF2==3.0.1`
2. **Run tests**: `python test_pdf_comprehensive.py`
3. **Try the example**: `python test_pdf_tool.py`
4. **Integrate with your agent**: Import and use as shown in the examples above

Happy PDF processing! 🚀📄✨