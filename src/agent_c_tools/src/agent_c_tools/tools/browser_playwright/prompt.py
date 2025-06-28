from typing import Any, Optional
from agent_c.prompting.prompt_section import OldPromptSection


class BrowserPlaywrightSection(OldPromptSection):
    """Prompt section for the Browser Playwright tool."""

    def __init__(self, **data: Any):
        TEMPLATE = (
            "The Browser Playwright tool provides a way to automate web browser interactions using the Playwright framework. "
            "This tool allows you to navigate to websites, interact with elements, capture screenshots, and more, "
            "all using structured accessibility information rather than visual processing.\n\n"
            
            "## Core Capabilities\n"
            "- **Navigation**: Visit URLs, go back/forward, and refresh pages\n"
            "- **Page Interaction**: Click elements, type text, select options from dropdowns\n"
            "- **Form Filling**: Input text, check/uncheck boxes, submit forms\n"
            "- **Tab Management**: Create, close, and switch between tabs\n"
            "- **Information Extraction**: Get page snapshots with accessibility information\n"
            "- **Screenshots**: Capture full page or element screenshots\n\n"
            
            "## Using Element References\n"
            "Many commands require element references that can be obtained from a page snapshot. "
            "Always get a page snapshot first before attempting to interact with elements on a page.\n\n"
            
            "## Best Practices\n"
            "1. Always check the page snapshot before interacting with elements\n"
            "2. Use appropriate waiting mechanisms when page content might be changing\n"
            "3. Handle navigation and page transitions properly\n"
            "4. Close browser sessions when done to free up resources\n"
            "5. Use appropriate error handling for more resilient automation\n\n"
            
            "## Example Workflow\n"
            "1. Navigate to a webpage\n"
            "2. Get a page snapshot to understand the page structure\n"
            "3. Locate elements of interest from the snapshot\n"
            "4. Interact with those elements (click, type, etc.)\n"
            "5. Capture results or screenshots as needed\n"
        )
        super().__init__(template=TEMPLATE, required=True, name="Browser Playwright", render_section_header=True, **data)