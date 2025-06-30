# PDF Converter Tool - Quick Reference

## 🚀 Quick Start

```python
from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
import base64, json

# Initialize
tool = PDFConverterTools()
await tool.post_init()

# Convert PDF
with open("file.pdf", "rb") as f:
    pdf_base64 = base64.b64encode(f.read()).decode('utf-8')

result = await tool.pdf_to_json(pdf_content=pdf_base64)
data = json.loads(result)
```

## 📋 Function Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pdf_content` | string | **required** | Base64 encoded PDF |
| `include_metadata` | boolean | `true` | Extract PDF metadata |
| `extract_by_page` | boolean | `true` | Page-by-page vs full text |

## 📤 Response Structure

### Success Response
```json
{
  "success": true,
  "extracted_at": "2024-01-15T10:30:00.123456",
  "total_pages": 2,
  "content": {
    "pages": [
      {
        "page_number": 1,
        "text": "Page content...",
        "character_count": 156
      }
    ]
  },
  "metadata": {
    "title": "Document Title",
    "author": "Author Name"
  }
}
```

### Error Response
```json
{
  "success": false,
  "error": "Error description",
  "extracted_at": "2024-01-15T10:30:00.123456"
}
```

## 🔧 Common Use Cases

### Extract All Text
```python
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    extract_by_page=False
)
data = json.loads(result)
full_text = data['content']['full_text']
```

### Get Page Count Only
```python
result = await tool.pdf_to_json(pdf_content=pdf_base64)
data = json.loads(result)
page_count = data['total_pages']
```

### Extract Metadata Only
```python
result = await tool.pdf_to_json(
    pdf_content=pdf_base64,
    include_metadata=True
)
data = json.loads(result)
title = data['metadata'].get('title', 'Unknown')
author = data['metadata'].get('author', 'Unknown')
```

## ⚡ Agent Function Call

```json
{
  "name": "pdf_converter-pdf_to_json",
  "arguments": {
    "pdf_content": "JVBERi0xLjQK...",
    "include_metadata": true,
    "extract_by_page": true
  }
}
```

## 🧪 Testing

```bash
# Quick test
python test_pdf_tool.py

# Full test suite
python test_pdf_comprehensive.py

# Check dependencies
python check_environment.py
```

## 🚨 Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `No PDF content provided` | Missing `pdf_content` parameter | Provide base64 PDF content |
| `Invalid base64 content` | Malformed base64 string | Check base64 encoding |
| `Could not read PDF` | Corrupted/invalid PDF | Verify PDF file integrity |
| `PyPDF2 library not installed` | Missing dependency | `pip install PyPDF2==3.0.1` |

## 📊 Performance Tips

- **Small PDFs (<1MB)**: Process directly
- **Large PDFs (>10MB)**: Consider chunking in future versions
- **Memory**: Base64 encoding increases size by ~33%
- **Speed**: ~50-200ms for typical documents

---

*Part of the Agent C Framework - Happy PDF processing! 🎉*