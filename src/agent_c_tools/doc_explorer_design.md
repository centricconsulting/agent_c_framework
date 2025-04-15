# Doc Explorer Design Document

## Overview

The Doc Explorer is a specialized HTML document exploration and manipulation tool designed to help AI agents efficiently work with HTML content. It provides tokenized-efficient ways to explore HTML structure, search for specific elements, modify content, and render HTML in different formats.

### Purpose

HTML files are notoriously token-heavy when sent directly to LLMs. The Doc Explorer tool addresses this challenge by providing methods to:

1. Browse HTML structure without loading the full document content
2. Target specific elements using CSS selectors and XPath
3. Selectively modify parts of HTML documents
4. Convert HTML to Markdown for more token-efficient content analysis
5. Send HTML or rendered snippets to the UI for visualization

## Key Features

### 1. HTML Structure Exploration

- **Summary View**: Generate a structural summary of HTML documents showing the hierarchy of elements, their attributes, and basic statistics
- **Tree Navigation**: Navigate HTML like a file system, with the ability to "ls" at different levels
- **Path Information**: Show the full path to elements, including tag types, IDs, and classes

### 2. Element Selection

- **CSS Selectors**: Find and extract elements using familiar CSS selector syntax
- **XPath Queries**: Support for XPath queries for more complex selection needs
- **Element Information**: Get detailed information about selected elements including attributes, content snippets, and context

### 3. Content Modification

- **Element Replacement**: Replace specific elements identified by selectors
- **Attribute Modification**: Add, modify, or remove attributes from elements
- **Content Insertion**: Insert content before, after, or inside selected elements
- **Content Removal**: Remove elements matching specific selectors

### 4. Format Conversion

- **HTML to Markdown**: Convert entire documents or specific sections to Markdown
- **Text Extraction**: Extract plain text from documents or sections
- **Structured Data Extraction**: Convert tabular HTML content to JSON or other structured formats

### 5. Rendering Support

- **Render Elements**: Send specific elements or snippets to the UI for rendering
- **Preview Changes**: Render before/after versions of modified content
- **Highlight Matches**: Visually highlight elements matching a selector in rendered output

## Architecture

### Package Structure

```
doc_explorer/
├── src/
│   ├── doc_explorer/
│   │   ├── __init__.py
│   │   ├── tool.py                 # Main tool implementation
│   │   ├── html_parser.py          # HTML parsing utilities
│   │   ├── selector_engine.py      # CSS/XPath selector implementation
│   │   ├── html_renderer.py        # HTML to markdown/output formatting
│   │   ├── html_modifier.py        # Document modification utilities
│   │   └── errors.py               # Custom error definitions
│   ├── pyproject.toml
│   └── setup.py
├── tests/
│   ├── test_parser.py
│   ├── test_selectors.py
│   ├── test_renderer.py
│   ├── test_modifier.py
│   └── sample_data/                # HTML test files
└── README.md
```

### Core Components

#### 1. HTML Parser

Responsible for loading HTML from files, streams, or URLs and providing a BeautifulSoup-based interface for traversal and manipulation.

```python
class HTMLParser:
    def __init__(self, workspace):
        self.workspace = workspace
        self.logger = workspace.logger
        
    async def load_document(self, file_path: str) -> BeautifulSoup:
        """Load HTML document from workspace path"""
        
    async def get_structure(self, soup: BeautifulSoup, max_depth: int = 5) -> dict:
        """Generate a structure summary of the HTML document"""
```

#### 2. Selector Engine

Provides unified interface for both CSS selectors and XPath queries, with methods to search, extract, and validate selections.

```python
class SelectorEngine:
    def __init__(self, soup: BeautifulSoup):
        self.soup = soup
        
    def query(self, selector: str, selector_type: str = "css") -> List[Tag]:
        """Find elements using either CSS selector or XPath"""
        
    def get_path(self, element: Tag) -> str:
        """Get a CSS/XPath path to the element"""
```

#### 3. HTML Renderer

Handles conversion between formats (HTML to Markdown, text extraction) and preparation of rendered output for the UI.

```python
class HTMLRenderer:
    def __init__(self):
        pass
        
    def to_markdown(self, html_content: str) -> str:
        """Convert HTML to Markdown"""
        
    def prepare_render_media(self, html_content: str, highlight_selector: str = None) -> dict:
        """Prepare RenderMedia event data"""
```

#### 4. HTML Modifier

Provides methods to safely modify HTML content with validation and error handling.

```python
class HTMLModifier:
    def __init__(self, soup: BeautifulSoup):
        self.soup = soup
        
    def replace_element_content(self, selector: str, new_content: str, selector_type: str = "css") -> bool:
        """Replace content of elements matching selector"""
        
    def modify_attributes(self, selector: str, attributes: Dict[str, str], selector_type: str = "css") -> bool:
        """Add or modify attributes for elements matching selector"""
```

### Main Tool Interface

The main `DocExplorerTools` class will provide the agent-facing methods that integrate the core components.

```python
class DocExplorerTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(name="doc_explorer", **kwargs)
        self.workspace_tools = None

    async def post_init(self):
        # Access workspace tools
        if 'workspace' in self.tool_chest.active_tools:
            self.workspace_tools = self.tool_chest.active_tools['workspace']

    @json_schema(
        "Get structure information about an HTML document",
        {
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) to the HTML file",
                "required": True
            },
            "max_depth": {
                "type": "integer",
                "description": "Maximum depth to explore in the document structure",
                "required": False
            }
        }
    )
    async def structure(self, **kwargs) -> str:
        # Implementation
        
    @json_schema(
        "Query an HTML document using CSS selectors or XPath",
        {
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) to the HTML file",
                "required": True
            },
            "selector": {
                "type": "string",
                "description": "CSS selector or XPath to query elements",
                "required": True
            },
            "selector_type": {
                "type": "string",
                "description": "Type of selector: 'css' or 'xpath'",
                "required": False
            },
            "limit": {
                "type": "integer",
                "description": "Maximum number of results to return",
                "required": False
            }
        }
    )
    async def query(self, **kwargs) -> str:
        # Implementation
        
    @json_schema(
        "Get the content of selected elements as Markdown",
        {
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) to the HTML file",
                "required": True
            },
            "selector": {
                "type": "string",
                "description": "CSS selector or XPath to target elements for conversion",
                "required": True
            },
            "selector_type": {
                "type": "string",
                "description": "Type of selector: 'css' or 'xpath'",
                "required": False
            }
        }
    )
    async def to_markdown(self, **kwargs) -> str:
        # Implementation
        
    @json_schema(
        "Modify HTML document by replacing content matching a selector",
        {
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) to the HTML file",
                "required": True
            },
            "selector": {
                "type": "string",
                "description": "CSS selector or XPath identifying elements to modify",
                "required": True
            },
            "new_content": {
                "type": "string",
                "description": "New HTML content to replace the matched elements with",
                "required": True
            },
            "selector_type": {
                "type": "string",
                "description": "Type of selector: 'css' or 'xpath'",
                "required": False
            },
            "output_path": {
                "type": "string",
                "description": "Optional path to save the modified HTML document",
                "required": False
            }
        }
    )
    async def modify(self, **kwargs) -> str:
        # Implementation
        
    @json_schema(
        "Render HTML content or specific elements in the UI",
        {
            "path": {
                "type": "string",
                "description": "UNC-style path (//WORKSPACE/path) to the HTML file",
                "required": True
            },
            "selector": {
                "type": "string",
                "description": "Optional CSS selector or XPath to choose elements to render",
                "required": False
            },
            "highlight": {
                "type": "string",
                "description": "Optional CSS selector for elements to highlight in the rendered output",
                "required": False
            },
            "selector_type": {
                "type": "string",
                "description": "Type of selector: 'css' or 'xpath'",
                "required": False
            }
        }
    )
    async def render(self, **kwargs) -> str:
        # Implementation
```

## API Design

The Doc Explorer will provide five main methods to agents:

### 1. structure

Generates a compact representation of the HTML document structure showing tag hierarchy, counts, and attribute patterns.

**Input**: 
- `path`: UNC path to HTML file
- `max_depth`: (optional) How deep in the structure to report

**Output**: JSON with structured summary of HTML document

**Example Usage**:
```python
structure_info = await doc_explorer.structure(path="//workspace/index.html", max_depth=3)
```

### 2. query

Finds elements in the document matching a CSS selector or XPath expression.

**Input**:
- `path`: UNC path to HTML file
- `selector`: CSS selector or XPath query
- `selector_type`: "css" or "xpath" (default "css")
- `limit`: (optional) Max number of results to return

**Output**: JSON with matching elements, their attributes, and content snippets

**Example Usage**:
```python
div_results = await doc_explorer.query(
    path="//workspace/index.html", 
    selector="div.content", 
    limit=5
)
```

### 3. to_markdown

Converts selected HTML content to Markdown for more token-efficient analysis.

**Input**:
- `path`: UNC path to HTML file
- `selector`: (optional) CSS selector or XPath to convert specific parts
- `selector_type`: "css" or "xpath" (default "css")

**Output**: Markdown representation of the HTML content

**Example Usage**:
```python
markdown_content = await doc_explorer.to_markdown(
    path="//workspace/index.html", 
    selector="main"
)
```

### 4. modify

Modifies HTML content by replacing elements matching a selector with new content.

**Input**:
- `path`: UNC path to HTML file
- `selector`: CSS selector or XPath identifying elements to modify
- `new_content`: New HTML content to replace the matched elements
- `selector_type`: "css" or "xpath" (default "css")
- `output_path`: (optional) Where to save the modified document

**Output**: Status information about the modification

**Example Usage**:
```python
result = await doc_explorer.modify(
    path="//workspace/index.html",
    selector="h1.title",
    new_content="<h1 class='title'>Updated Title</h1>",
    output_path="//workspace/updated_index.html"
)
```

### 5. render

Prepares HTML content for rendering in the UI, optionally highlighting specific elements.

**Input**:
- `path`: UNC path to HTML file
- `selector`: (optional) CSS selector or XPath for elements to render
- `highlight`: (optional) CSS selector for elements to highlight
- `selector_type`: "css" or "xpath" (default "css")

**Output**: RenderMedia event triggered with the HTML content

**Example Usage**:
```python
await doc_explorer.render(
    path="//workspace/index.html",
    selector="article",
    highlight="code, pre"
)
```

## Dependencies

- **Beautiful Soup 4**: For HTML parsing and manipulation
- **lxml**: Backend for BeautifulSoup and XPath support
- **html2text**: For high-quality HTML to Markdown conversion
- **cssselect**: For translating CSS selectors to XPath expressions

## Implementation Considerations

### Parsing Strategy

The tool should use progressive parsing where possible to handle large HTML files efficiently:

1. First-pass quick scanning for structure analysis
2. Targeted parsing for selector queries
3. Full parsing only when necessary for complete transformations

### Token Efficiency

The tool should optimize for token efficiency by:

1. Providing structure summaries instead of raw HTML
2. Using selectors to extract only needed elements
3. Converting HTML to Markdown to reduce token count
4. Supplying aggregate info (element counts, etc.) instead of raw dumps

### Error Handling

Robust error handling should include:

1. Validation of input HTML (detect malformed HTML)
2. Validation of selectors before use
3. Clear error messages with suggestions for fixes
4. Graceful degradation for partial parsing when documents have issues

### State Management

The tool should:

1. Avoid holding state between calls when possible
2. Use caching to improve performance on repeated operations 
3. Clean up resources properly after each call

## Extension Possibilities

- **Form Analysis**: Special handling for forms, inputs, and validation rules
- **Accessibility Analysis**: Check for ARIA attributes, semantic structure, etc.
- **Layout Analysis**: Provide information about page layout and structure
- **JavaScript Analysis**: Extract and report on script elements and event handlers
- **Metadata Extraction**: Special handling for meta tags, Open Graph, etc.

## Testing Strategy

### Unit Tests

- Parser tests with various HTML complexity levels
- Selector tests for both CSS and XPath
- Renderer tests for HTML to Markdown conversion accuracy
- Modifier tests for all modification operations

### Integration Tests

- End-to-end tests using real-world HTML documents
- Performance tests with large HTML files
- Error handling tests with malformed HTML

### Test Data

- Sample HTML files of varying complexity
- Real-world examples from popular websites
- Edge cases with unusual HTML structures