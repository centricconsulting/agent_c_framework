from typing import Any, Dict,  Optional,  cast
import base64
import os
import tempfile
import uuid
import yaml
from datetime import datetime


from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.browser_playwright.prompt import BrowserPlaywrightSection
from agent_c_tools.tools.browser_playwright.models import (
    BrowserSessionModel, SnapshotModel, TabModel, ElementModel, 
    ElementType, TabActionType
)
from agent_c_tools.tools.workspace.tool import WorkspaceTools


class BrowserPlaywrightTools(Toolset):
    """
    BrowserPlaywrightTools provides methods for web browser automation using Playwright.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(name='browser', **kwargs)
        self.section = BrowserPlaywrightSection()
        self.workspace_tool: Optional[WorkspaceTools] = None
        self.sessions: Dict[str, Any] = {}  # Will hold Playwright browser sessions
        self.current_session_id: Optional[str] = None
        self.temp_dir = tempfile.mkdtemp(prefix="browser_playwright_")

    async def post_init(self):
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))
    
    async def _initialize_playwright(self):
        """
        Initialize the Playwright engine and set up any necessary configurations.
        """
        try:
            from playwright.async_api import async_playwright
            self.playwright_module = async_playwright
            self.logger.info("Playwright module loaded successfully")
            return True
        except ImportError:
            self.logger.error("Failed to import playwright module. Make sure it's installed.")
            return False

    @json_schema(
        description="Initialize a new browser session",
        params={
            "browser_type": {
                "type": "string",
                "description": "Type of browser to launch: 'chromium', 'firefox', or 'webkit'",
                "enum": ["chromium", "firefox", "webkit"],
                "required": True
            },
            "headless": {
                "type": "boolean",
                "description": "Whether to run browser in headless mode"
            },
            "user_agent": {
                "type": "string",
                "description": "Custom user agent string"
            },
            "viewport_width": {
                "type": "integer",
                "description": "Width of the browser viewport in pixels"
            },
            "viewport_height": {
                "type": "integer",
                "description": "Height of the browser viewport in pixels"
            }
        }
    )
    async def initialize_browser(self, **kwargs) -> str:
        """
        Initialize a new browser session with the specified parameters.
        """
        browser_type = kwargs.get("browser_type")
        headless = kwargs.get("headless", False)
        user_agent = kwargs.get("user_agent")
        viewport_width = kwargs.get("viewport_width", 1280)
        viewport_height = kwargs.get("viewport_height", 720)
        
        if not browser_type:
            return "ERROR: browser_type parameter is required"
        
        # Verify Playwright is available
        if not hasattr(self, 'playwright_module'):
            init_success = await self._initialize_playwright()
            if not init_success:
                return "ERROR: Failed to initialize Playwright. Please ensure the playwright package is installed."
        
        try:
            # Start a new browser instance
            playwright = await self.playwright_module().__aenter__()
            browser_obj = getattr(playwright, browser_type)
            
            # Set up browser launch options
            launch_options = {"headless": headless}
            browser = await browser_obj.launch(**launch_options)
            
            # Create a context with viewport and user agent if provided
            context_options = {
                "viewport": {"width": viewport_width, "height": viewport_height}
            }
            if user_agent:
                context_options["user_agent"] = user_agent
            
            context = await browser.new_context(**context_options)
            
            # Create an initial page
            page = await context.new_page()
            
            # Generate a unique session ID
            session_id = str(uuid.uuid4())
            
            # Store the session information
            self.sessions[session_id] = {
                "playwright": playwright,
                "browser": browser,
                "context": context,
                "page": page
            }
            
            # Create a session model
            session_model = BrowserSessionModel(
                id=session_id,
                tabs=[TabModel(index=0, title="New Tab", url="about:blank", is_active=True)],
                active_tab_index=0
            )
            
            # Set this as the current session
            self.current_session_id = session_id
            
            result = {
                "success": True,
                "session": session_model.model_dump(),
                "message": f"Browser session initialized with {browser_type}"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to initialize browser: {str(e)}")
            return f"ERROR: Failed to initialize browser: {str(e)}"

    @json_schema(
        description="Navigate to a URL in the current browser session",
        params={
            "url": {
                "type": "string",
                "description": "URL to navigate to",
                "required": True
            },
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "wait_until": {
                "type": "string",
                "description": "When to consider navigation succeeded: 'load', 'domcontentloaded', 'networkidle', or 'commit'",
                "enum": ["load", "domcontentloaded", "networkidle", "commit"]
            }
        }
    )
    async def navigate(self, **kwargs) -> str:
        """
        Navigate to a URL in the current browser session.
        """
        url = kwargs.get("url")
        session_id = kwargs.get("session_id") or self.current_session_id
        wait_until = kwargs.get("wait_until", "load")
        
        if not url:
            return "ERROR: url parameter is required"
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the current page
            session = self.sessions[session_id]
            page = session["page"]
            
            # Navigate to the URL
            await page.goto(url, wait_until=wait_until)
            
            # Update the session model's active tab information
            title = await page.title()
            current_url = page.url
            
            # Return success with the current page information
            result = {
                "success": True,
                "url": current_url,
                "title": title,
                "message": f"Navigated to {url}"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to navigate to {url}: {str(e)}")
            return f"ERROR: Failed to navigate to {url}: {str(e)}"

    @json_schema(
        description="Get a snapshot of the current page",
        params={
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "include_hidden": {
                "type": "boolean",
                "description": "Whether to include hidden elements in the snapshot"
            }
        }
    )
    async def get_snapshot(self, **kwargs) -> str:
        """
        Get a snapshot of the current page with accessibility information.
        """
        session_id = kwargs.get("session_id") or self.current_session_id
        include_hidden = kwargs.get("include_hidden", False)
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the current page
            session = self.sessions[session_id]
            page = session["page"]
            
            # Get the page title and URL
            title = await page.title()
            url = page.url
            
            # Get the accessibility snapshot
            snapshot = await page.accessibility.snapshot(interestingOnly=not include_hidden)
            
            # Process the snapshot to create our model
            elements = []
            
            # Helper function to process nodes recursively
            def process_node(node, parent_ref=""):
                node_elements = []
                
                # Create a reference ID for this element
                ref = f"{parent_ref}/{len(node_elements)}" if parent_ref else str(len(elements))
                
                # Determine element type
                element_type = "other"
                role = node.get("role", "").lower()
                
                if role == "link":
                    element_type = "link"
                elif role in ["button", "menuitem"]:
                    element_type = "button"
                elif role in ["textbox", "searchbox"]:
                    element_type = "input"
                elif role == "img":
                    element_type = "image"
                elif role == "checkbox":
                    element_type = "checkbox"
                elif role == "radio":
                    element_type = "radio"
                elif role == "combobox":
                    element_type = "select"
                elif role in ["text", "paragraph", "heading"]:
                    element_type = "text"
                
                # Create the element model
                element = ElementModel(
                    ref=ref,
                    element_type=element_type,
                    description=node.get("name", ""),
                    text=node.get("name", ""),
                    value=node.get("value", ""),
                    attributes={},  # We don't have direct access to attributes in the accessibility tree
                    accessible_name=node.get("name", ""),
                    is_visible=True,  # Assuming visible since it's in the accessibility tree
                    is_enabled=not node.get("disabled", False)
                )
                
                node_elements.append(element)
                
                # Process child nodes
                if "children" in node:
                    for child in node["children"]:
                        child_elements = process_node(child, ref)
                        node_elements.extend(child_elements)
                
                return node_elements
            
            # Process the root node
            if snapshot:
                elements.extend(process_node(snapshot))
            
            # Create the snapshot model
            snapshot_model = SnapshotModel(
                url=url,
                title=title,
                elements=elements
            )
            
            result = {
                "success": True,
                "snapshot": snapshot_model.model_dump()
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to get page snapshot: {str(e)}")
            return f"ERROR: Failed to get page snapshot: {str(e)}"

    @json_schema(
        description="Click on an element in the page",
        params={
            "element_ref": {
                "type": "string",
                "description": "Reference to the element to click (from snapshot)",
                "required": True
            },
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "force": {
                "type": "boolean",
                "description": "Whether to bypass actionability checks"
            },
            "modifiers": {
                "type": "array",
                "description": "Modifier keys to press while clicking",
                "items": {
                    "type": "string",
                    "enum": ["Alt", "Control", "Meta", "Shift"]
                }
            }
        }
    )
    async def click(self, **kwargs) -> str:
        """
        Click on an element in the page.
        """
        element_ref = kwargs.get("element_ref")
        session_id = kwargs.get("session_id") or self.current_session_id
        force = kwargs.get("force", False)
        modifiers = kwargs.get("modifiers")
        
        if not element_ref:
            return "ERROR: element_ref parameter is required"
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the current page
            session = self.sessions[session_id]
            page = session["page"]
            
            # Convert element_ref to a CSS selector or XPath
            # This is a simplification - in a real implementation, you would need to map
            # from your element_ref to an actual element on the page
            element = await page.wait_for_selector(f"[data-ref='{element_ref}']", state="visible")
            
            if not element:
                # Try a different approach - this is just an example
                # In reality, you'd need a more robust way to find elements from your references
                element_index = int(element_ref)
                elements = await page.query_selector_all("*[role]:not([role='none'])")
                if element_index < len(elements):
                    element = elements[element_index]
                else:
                    return f"ERROR: Element with reference {element_ref} not found"
            
            # Set up click options
            click_options = {"force": force}
            if modifiers:
                click_options["modifiers"] = modifiers
            
            # Click the element
            await element.click(**click_options)
            
            # Wait a moment for any page updates
            await page.wait_for_load_state("networkidle")
            
            result = {
                "success": True,
                "message": f"Clicked element with reference {element_ref}"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to click element: {str(e)}")
            return f"ERROR: Failed to click element: {str(e)}"

    @json_schema(
        description="Type text into an input element",
        params={
            "element_ref": {
                "type": "string",
                "description": "Reference to the input element (from snapshot)",
                "required": True
            },
            "text": {
                "type": "string",
                "description": "Text to type into the element",
                "required": True
            },
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "delay": {
                "type": "integer",
                "description": "Delay between keystrokes in milliseconds"
            },
            "clear_first": {
                "type": "boolean",
                "description": "Whether to clear the input first"
            }
        }
    )
    async def type_text(self, **kwargs) -> str:
        """
        Type text into an input element.
        """
        element_ref = kwargs.get("element_ref")
        text = kwargs.get("text")
        session_id = kwargs.get("session_id") or self.current_session_id
        delay = kwargs.get("delay", 0)
        clear_first = kwargs.get("clear_first", True)
        
        if not element_ref:
            return "ERROR: element_ref parameter is required"
        if not text:
            return "ERROR: text parameter is required"
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the current page
            session = self.sessions[session_id]
            page = session["page"]
            
            # Similar to click, we need to find the element
            # This is a simplification
            element = await page.wait_for_selector(f"[data-ref='{element_ref}']", state="visible")
            
            if not element:
                element_index = int(element_ref)
                elements = await page.query_selector_all("input, textarea, [contenteditable='true']")
                if element_index < len(elements):
                    element = elements[element_index]
                else:
                    return f"ERROR: Input element with reference {element_ref} not found"
            
            # Clear the input first if requested
            if clear_first:
                await element.fill("")
            
            # Type the text
            await element.type(text, delay=delay)
            
            result = {
                "success": True,
                "message": f"Typed text into element with reference {element_ref}"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to type text: {str(e)}")
            return f"ERROR: Failed to type text: {str(e)}"

    @json_schema(
        description="Take a screenshot of the current page or a specific element",
        params={
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "element_ref": {
                "type": "string",
                "description": "Reference to the element to screenshot (from snapshot)"
            },
            "full_page": {
                "type": "boolean",
                "description": "Whether to capture the full page or just the viewport"
            },
            "quality": {
                "type": "integer",
                "description": "JPEG quality (0-100) for the screenshot"
            },
            "file_format": {
                "type": "string",
                "description": "Format of the screenshot: 'png' or 'jpeg'",
                "enum": ["png", "jpeg"]
            }
        }
    )
    async def take_screenshot(self, **kwargs) -> str:
        """
        Take a screenshot of the current page or a specific element.
        """
        session_id = kwargs.get("session_id") or self.current_session_id
        element_ref = kwargs.get("element_ref")
        full_page = kwargs.get("full_page", False)
        quality = kwargs.get("quality", 80)
        file_format = kwargs.get("file_format", "png")
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the current page
            session = self.sessions[session_id]
            page = session["page"]
            
            # Set up screenshot options
            screenshot_options = {
                "full_page": full_page,
                "type": file_format
            }
            
            if file_format == "jpeg":
                screenshot_options["quality"] = quality
            
            # Take the screenshot
            if element_ref:
                # Find the element first
                element = await page.wait_for_selector(f"[data-ref='{element_ref}']", state="visible")
                
                if not element:
                    element_index = int(element_ref)
                    elements = await page.query_selector_all("*[role]:not([role='none'])")
                    if element_index < len(elements):
                        element = elements[element_index]
                    else:
                        return f"ERROR: Element with reference {element_ref} not found"
                
                # Take a screenshot of just this element
                screenshot_bytes = await element.screenshot(**screenshot_options)
            else:
                # Take a screenshot of the page
                screenshot_bytes = await page.screenshot(**screenshot_options)
            
            # Save the screenshot to a temp file
            timestamp = datetime.now().strftime("%Y%m%d%H%M%S")
            filename = f"screenshot_{timestamp}.{file_format}"
            filepath = os.path.join(self.temp_dir, filename)
            
            with open(filepath, "wb") as f:
                f.write(screenshot_bytes)
            
            # Convert the screenshot to base64 for display
            base64_screenshot = base64.b64encode(screenshot_bytes).decode("utf-8")
            
            # Get the media type
            media_type = f"image/{file_format}"
            
            # Raise a media event to display the screenshot
            await self._raise_render_media(
                content_type=media_type,
                name=filename,
                content_bytes=screenshot_bytes,
                content=f"data:{media_type};base64,{base64_screenshot}"
            )
            
            result = {
                "success": True,
                "file_path": filepath,
                "message": f"Screenshot saved to {filepath}"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to take screenshot: {str(e)}")
            return f"ERROR: Failed to take screenshot: {str(e)}"

    @json_schema(
        description="Close the browser session",
        params={
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to close"
            }
        }
    )
    async def close_browser(self, **kwargs) -> str:
        """
        Close the browser session and clean up resources.
        """
        session_id = kwargs.get("session_id") or self.current_session_id
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session to close."
        
        try:
            # Get the session
            session = self.sessions[session_id]
            
            # Close the browser
            await session["browser"].close()
            
            # Close the playwright instance
            await session["playwright"].__aexit__(None, None, None)
            
            # Remove the session from our storage
            del self.sessions[session_id]
            
            # If this was the current session, clear the current session ID
            if session_id == self.current_session_id:
                self.current_session_id = None if not self.sessions else next(iter(self.sessions))
            
            result = {
                "success": True,
                "message": f"Browser session {session_id} closed successfully"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            self.logger.error(f"Failed to close browser session: {str(e)}")
            return f"ERROR: Failed to close browser session: {str(e)}"

    @json_schema(
        description="Manage browser tabs",
        params={
            "action": {
                "type": "string",
                "description": "Tab action to perform: 'new', 'close', 'select', or 'list'",
                "enum": ["new", "close", "select", "list"],
                "required": True
            },
            "session_id": {
                "type": "string",
                "description": "ID of the browser session to use"
            },
            "url": {
                "type": "string",
                "description": "URL to navigate to (for 'new' action)"
            },
            "tab_index": {
                "type": "integer",
                "description": "Index of the tab to operate on (for 'close' and 'select' actions)"
            }
        }
    )
    async def manage_tabs(self, **kwargs) -> str:
        """
        Manage browser tabs: create, close, select, or list tabs.
        """
        action = kwargs.get("action")
        session_id = kwargs.get("session_id") or self.current_session_id
        url = kwargs.get("url")
        tab_index = kwargs.get("tab_index")
        
        if not action:
            return "ERROR: action parameter is required"
        
        if not session_id or session_id not in self.sessions:
            return "ERROR: No active browser session. Initialize a browser first."
        
        try:
            # Get the session
            session = self.sessions[session_id]
            context = session["context"]
            
            # Helper to get all pages and update the session model
            async def update_tabs_model():
                pages = context.pages
                tabs = []
                
                for i, page in enumerate(pages):
                    is_active = page == session["page"]
                    title = await page.title() or "New Tab"
                    url = page.url or "about:blank"
                    
                    tabs.append(TabModel(
                        index=i,
                        title=title,
                        url=url,
                        is_active=is_active
                    ))
                
                # Update the active tab index
                active_index = next((i for i, tab in enumerate(tabs) if tab.is_active), 0)
                
                return tabs, active_index
            
            # Perform the requested action
            if action == "new":
                # Create a new tab
                new_page = await context.new_page()
                
                # If a URL was provided, navigate to it
                if url:
                    await new_page.goto(url)
                
                # Set this as the active page
                session["page"] = new_page
                
                # Update the tabs model
                tabs, active_tab_index = await update_tabs_model()
                
                result = {
                    "success": True,
                    "tabs": [tab.model_dump() for tab in tabs],
                    "active_tab_index": active_tab_index,
                    "message": "New tab created"
                }
                return yaml.dump(result, allow_unicode=True)
            
            elif action == "close":
                # Close a tab
                if tab_index is None:
                    return "ERROR: Tab index must be provided for 'close' action"
                
                # Get all pages
                pages = context.pages
                
                if tab_index < 0 or tab_index >= len(pages):
                    return f"ERROR: Invalid tab index: {tab_index}"
                
                # Close the page
                page_to_close = pages[tab_index]
                await page_to_close.close()
                
                # If we closed the active page, set a new active page
                if page_to_close == session["page"]:
                    pages = context.pages
                    if pages:
                        session["page"] = pages[0]
                
                # Update the tabs model
                tabs, active_tab_index = await update_tabs_model()
                
                result = {
                    "success": True,
                    "tabs": [tab.model_dump() for tab in tabs],
                    "active_tab_index": active_tab_index,
                    "message": f"Tab {tab_index} closed"
                }
                return yaml.dump(result, allow_unicode=True)
            
            elif action == "select":
                # Select a tab
                if tab_index is None:
                    return "ERROR: Tab index must be provided for 'select' action"
                
                # Get all pages
                pages = context.pages
                
                if tab_index < 0 or tab_index >= len(pages):
                    return f"ERROR: Invalid tab index: {tab_index}"
                
                # Set the active page
                session["page"] = pages[tab_index]
                
                # Update the tabs model
                tabs, active_tab_index = await update_tabs_model()
                
                result = {
                    "success": True,
                    "tabs": [tab.model_dump() for tab in tabs],
                    "active_tab_index": active_tab_index,
                    "message": f"Tab {tab_index} selected"
                }
                return yaml.dump(result, allow_unicode=True)
            
            elif action == "list":
                # List all tabs
                tabs, active_tab_index = await update_tabs_model()
                
                result = {
                    "success": True,
                    "tabs": [tab.model_dump() for tab in tabs],
                    "active_tab_index": active_tab_index
                }
                return yaml.dump(result, allow_unicode=True)
            
            else:
                return f"ERROR: Invalid action: {action}"
        except Exception as e:
            self.logger.error(f"Failed to manage tabs: {str(e)}")
            return f"ERROR: Failed to manage tabs: {str(e)}"


# Register the toolset with required dependencies
Toolset.register(BrowserPlaywrightTools, required_tools=['WorkspaceTools'])