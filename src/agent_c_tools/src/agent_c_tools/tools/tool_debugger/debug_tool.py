import asyncio
import importlib
import json
import logging
import os
import sys
import time
import types
from pathlib import Path
from typing import Dict, List, Any, Type, Optional, Union


# Import the base ToolChest class
from agent_c.toolsets import ToolChest, ToolCache
# Try to import python-dotenv for .env file loading
try:
    from dotenv import load_dotenv
    DOTENV_AVAILABLE = True
except ImportError:
    DOTENV_AVAILABLE = False

try:
    import yaml
    YAML_AVAILABLE = True
except ImportError:
    YAML_AVAILABLE = False


# Add a mock for TokenCounter to fix the error in local_storage.py
class MockTokenCounter:
    @staticmethod
    def count(text):
        # Simple mock that returns a token count based on text length
        # This avoids the "NoneType has no attribute 'count_tokens'" error when using Workspaces without an agent
        return len(text) // 10  # Rough approximation (10 chars per token)

    @staticmethod
    def count_tokens(text):
        # Simple mock that returns a token count based on text length
        # This avoids the "NoneType has no attribute 'count_tokens'" error when using Workspaces without an agent
        return len(text) // 10  # Rough approximation (10 chars per token)

# Add the mock to sys.modules so it can be imported by local_storage.py
mock_module = types.ModuleType('agent_c.util.token_counter')
mock_module.TokenCounter = MockTokenCounter
sys.modules['agent_c.util.token_counter'] = mock_module


# Import the base ToolChest class AFTER the mock is set up
from agent_c.toolsets import ToolChest, ToolCache


class ToolDebugger:
    """
    A testing framework for agent_c tools.
    Allows dynamically importing and testing any tool with configurable payloads.
    """

    def __init__(self, log_level=logging.INFO, init_local_workspaces: bool = True, agent_c_base_path: str = None):
        """
        Initialize the tool tester with optional logging configuration.

        Args:
            log_level: Logging level (default: logging.INFO)
            init_local_workspaces: Flag to initialize local workspaces (default: True). Helpful when tools have dependencies on local workspaces. If you set to false and your toolset requires local workspaces, it will likely blow up.
            agent_c_base_path: Path to agent_c base directory. If None, will try to auto-detect.
        """
        # Configure logging
        logging.basicConfig(level=log_level,
                            format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
        self.logger = logging.getLogger(__name__)

        # Find the agent_c base directory
        self.agent_c_base_path = self._find_agent_c_base_path(agent_c_base_path)
        self.logger.info(f"Using agent_c base path: {self.agent_c_base_path}")
        
        # Load .env file if available
        self._load_env_file()

        # Prep for workspaces if needed
        self.init_local_workspaces = init_local_workspaces
        self.workspaces = None

        ## if workspace and local setup.  This is manually controlled.  If a dependent tool needs them initialized and you set this to False
        # your dependent tool is likely to blow up.
        if self.init_local_workspaces:
            self.logger.info("Initializing local workspaces")
            # Initialize local workspaces
            self.init_workspaces()

        # Tool Cache
        self.tool_cache_dir = ".tool_cache"
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)

        # Initialize the tool chest
        self.tool_chest = ToolChest()
        self.logger.info("ToolDebugger initialized")

    def _find_agent_c_base_path(self, provided_path: str = None) -> str:
        """
        Find the agent_c base directory path.
        
        Args:
            provided_path: Optional path provided by user
            
        Returns:
            Path to agent_c base directory
        """
        if provided_path:
            if os.path.exists(provided_path):
                return provided_path
            else:
                self.logger.warning(f"Provided path {provided_path} does not exist, trying auto-detection")
        
        # Try to auto-detect by looking for common patterns
        current_dir = Path(__file__).resolve()
        
        # Look for agent_c directory by traversing up the directory tree
        for parent in current_dir.parents:
            if parent.name == "agent_c":
                return str(parent)
            # Also check if this directory contains expected files
            if (parent / ".local_workspaces.json").exists() or (parent / ".env").exists():
                return str(parent)
        
        # If all else fails, use current directory
        self.logger.warning("Could not auto-detect agent_c base path, using current directory")
        return str(Path.cwd())
    
    def _load_env_file(self) -> None:
        """
        Load .env file from the agent_c base directory.
        """
        env_file_path = os.path.join(self.agent_c_base_path, ".env")
        
        if not DOTENV_AVAILABLE:
            if os.path.exists(env_file_path):
                self.logger.warning(".env file found but python-dotenv not installed. Install with: pip install python-dotenv")
            return
        
        if os.path.exists(env_file_path):
            self.logger.info(f"Loading .env file from: {env_file_path}")
            load_dotenv(env_file_path)
            self.logger.info("Environment variables loaded from .env file")
        else:
            self.logger.info(f"No .env file found at: {env_file_path}")



    async def import_tool_class(self, tool_import_path: str) -> Type:
        """
        Dynamically import a tool class from its import path.

        Args:
            tool_import_path: Import path of the tool class (e.g., 'agent_c_tools.FlashDocsTools')
                              Format: 'module_path.ClassName'

        Returns:
            The imported tool class
        """
        try:
            # Split the import path into module and class name
            if '.' in tool_import_path:
                module_path, class_name = tool_import_path.rsplit('.', 1)

                # Import the module
                self.logger.info(f"Importing module: {module_path}")
                module = importlib.import_module(module_path)

                # Get the class from the module
                self.logger.info(f"Getting class: {class_name}")
                tool_class = getattr(module, class_name)

                return tool_class
            else:
                # Assume it's a class in the current module
                current_module = importlib.import_module('__main__')
                return getattr(current_module, tool_import_path)

        except (ImportError, AttributeError) as e:
            self.logger.error(f"Failed to import tool class {tool_import_path}: {str(e)}")
            raise

    async def setup_tool(self, tool_import_path: str, tool_opts: Dict[str, Any] = None) -> None:
        """
        Import and initialize a tool in the tool chest.

        Args:
            tool_import_path: Import path of the tool class
            tool_opts: Dictionary of initialization parameters for the tool, wrapped in 'tool_opts'
        """
        try:
            # Import the tool class
            tool_class = await self.import_tool_class(tool_import_path)

            # Add the tool class to the tool chest
            self.logger.info(f"Adding tool class {tool_class.__name__} to tool chest")
            self.tool_chest.add_tool_class(tool_class)

            # Initialize the tool cache
            if not self.tool_cache is None:
                tool_opts['tool_cache'] = self.tool_cache

            # Add workspace to tool_opts if local workspaces are initialized included
            # if a dependent tool needs them initialized and you set this to False your dependent tool is likely to blow up
            if self.init_local_workspaces:
                tool_opts['workspaces'] = self.workspaces

            # Initialize the tools with parameters
            self.logger.info(f"Initializing tools with parameters: {tool_opts}")
            await self.tool_chest.init_tools(tool_opts=tool_opts)

            # Set the active toolsets
            await self.tool_chest.set_active_toolsets([tool_class.__name__], tool_opts=tool_opts)
            self.logger.info(f"Set active toolset: {tool_class.__name__}")

            self.logger.info(f"Tool {tool_class.__name__} setup complete")

            # Log available tools for debugging
            self.logger.info(f"Active tools: {list(self.tool_chest.active_tools.keys())}")

            if 'WorkspaceTools' in self.tool_chest.active_tools and not self.init_local_workspaces:
                self.logger.warning(
                    f"ALERT: Tool {tool_class.__name__} has activated WorkspaceTools as a dependency, "
                    f"but init_local_workspaces is set to False. This may cause errors when running tools."
                )

        except Exception as e:
            self.logger.error(f"Failed to setup tool {tool_import_path}: {str(e)}")
            raise

    def init_workspaces(self) -> None:
        if not self.init_local_workspaces:
            return

        from agent_c_tools.tools.workspace.local_storage import LocalStorageWorkspace
        from agent_c_tools.tools.workspace.local_project import LocalProjectWorkspace

        import agent_c_tools.tools.workspace.local_storage as local_storage_module
        local_storage_module.TokenCounter = MockTokenCounter

        local_project = LocalProjectWorkspace()

        self.workspaces = [local_project]
        self.logger.info(f"Initialized workspaces {local_project.workspace_root}")

        # Load .local_workspaces.json from agent_c base directory
        workspaces_file_path = os.path.join(self.agent_c_base_path, ".local_workspaces.json")
        
        try:
            self.logger.info(f"Loading local workspaces from: {workspaces_file_path}")
            with open(workspaces_file_path, 'r') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
                
            self.logger.info(f"Loaded {len(local_workspaces['local_workspaces'])} additional workspaces")
        except FileNotFoundError:
            self.logger.info(f"No .local_workspaces.json file found at: {workspaces_file_path}")
        except Exception as e:
            self.logger.error(f"Error loading .local_workspaces.json: {str(e)}")

    def print_tool_info(self) -> None:
        """
        Print detailed information about the available tools for debugging.
        """
        print("\n=== Tool Information ===")

        for name, tool in self.tool_chest.active_tools.items():
            print(f"\nTool '{name}' details:")
            print(f"  Class: {tool.__class__.__name__}")
            print(f"  Name attribute: {tool.name if hasattr(tool, 'name') else 'No name attribute'}")
            print(f"  Available methods:")

            # Get methods that don't start with _ and are callable
            methods = [method for method in dir(tool)
                       if not method.startswith('_') and callable(getattr(tool, method))]

            for method in methods:
                print(f"    - {method}")

        # Print schema information
        print("\nToolset schemas:")
        for name, toolset in self.tool_chest.active_tools.items():
            print(f"\nToolset '{name}' schemas:")
            try:
                for i, schema in enumerate(toolset.openai_schemas):
                    function_name = schema.get('function', {}).get('name', 'unknown')
                    print(f"  Schema {i}: function name = {function_name}")
            except Exception as e:
                print(f"  Error getting schemas: {e}")

        # Debug the _tool_name_to_instance_map that's used for lookups
        print("\nTool name to instance map:")
        try:
            for name, instance in self.tool_chest._tool_name_to_instance_map.items():
                print(f"  {name} -> {instance.__class__.__name__}")
        except Exception as e:
            print(f"  Error accessing map: {e}")

    async def run_tool_test(self, tool_name: str, tool_params: Dict[str, Any], tool_context: Dict[str, Any] = None) -> List[Dict[str, Any]]:
        """
        Run a test for a specific tool with given parameters.

        Args:
            tool_context: Context information passed to tools (optional, defaults to basic context)
            tool_name: Name of the tool to call (e.g., 'flash_docs_outline_to_powerpoint')
            tool_params: Parameters for the tool call

        Returns:
            The raw tool results
        """
        test_id = f"test_{int(time.time())}"

        # Format the tool call
        tool_call = [{
            "id": test_id,
            "name": tool_name,
            "input": tool_params
        }]

        if tool_context is None:
            tool_context = {
                "session_id": f"debug_session_{test_id}",
                "user_id": "debug_user",
                "workspace_id": "debug_workspace",
                "debug_mode": True
            }

        try:
            self.logger.info(f"Executing tool call: {tool_name}")
            self.logger.debug(f"Tool parameters: {json.dumps(tool_params, indent=2, default=str)}")
            self.logger.debug(f"Tool context: {json.dumps(tool_context, indent=2, default=str)}")

            # Call the tool
            results = await self.tool_chest.call_tools(tool_call, tool_context)

            return results

        except Exception as e:
            error_msg = f"Error executing tool call {tool_name}: {str(e)}"
            self.logger.error(error_msg)
            return [{"error": error_msg}]

    def extract_content_from_results(self, results: List[Dict[str, Any]]) -> str:
        """
        Helper method to extract content from the standard results format.

        Args:
            results: Results from a tool call

        Returns:
            Extracted content as a string, or error message if extraction fails
        """
        try:
            # First, check if this is an error response
            if results and isinstance(results, list) and len(results) == 1 and 'error' in results[0]:
                return f"Error: {results[0]['error']}"

            # Check for error messages in the standard format
            if len(results) > 1 and 'content' in results[1]:
                content_list = results[1]['content']
                for content_item in content_list:
                    if isinstance(content_item, dict) and 'content' in content_item and 'type' in content_item:
                        if content_item['type'] == 'tool_result':
                            return content_item['content']

            # Fallback - return the full results as a JSON string
            return f"Could not extract content, full results: {json.dumps(results, indent=2, default=str)}"

        except Exception as e:
            self.logger.error(f"Error extracting content: {str(e)}")
            return f"Error extracting content: {str(e)}"

    def extract_structured_content(self, results: List[Dict[str, Any]], format_hint: str = 'auto') -> Dict[str, Any]:
        """
        Extract content from results and parse it as JSON or YAML.

        Args:
            results: Results from a tool call
            format_hint: 'json', 'yaml', or 'auto' to auto-detect format

        Returns:
            Parsed content as a dictionary, or None if parsing fails
        """
        content_str = self.extract_content_from_results(results)

        if not isinstance(content_str, str) or not content_str.strip():
            return None

        try:
            if format_hint == 'auto':
                # Auto-detect format based on content
                detected_format = self.detect_content_format(results)
                format_hint = detected_format if detected_format != 'unknown' else 'json'

            if format_hint == 'json':
                return self._parse_json_content(content_str)
            elif format_hint == 'yaml':
                return self._parse_yaml_content(content_str)
            else:
                # Try JSON first, then YAML as fallback
                json_result = self._parse_json_content(content_str)
                if json_result is not None:
                    return json_result
                return self._parse_yaml_content(content_str)

        except Exception as e:
            self.logger.warning(f"Could not parse content as structured data: {str(e)}")
            return None

    def extract_json_content(self, results: List[Dict[str, Any]]) -> Dict[str, Any]:
        """
        Extract content from results and parse it as JSON.

        Args:
            results: Results from a tool call

        Returns:
            Parsed JSON content as a dictionary, or None if parsing fails
        """
        content_str = self.extract_content_from_results(results)
        return self._parse_json_content(content_str)

    def extract_yaml_content(self, results: List[Dict[str, Any]]) -> Dict[str, Any]:
        """
        Extract content from results and parse it as YAML.

        Args:
            results: Results from a tool call

        Returns:
            Parsed YAML content as a dictionary, or None if parsing fails
        """
        content_str = self.extract_content_from_results(results)
        return self._parse_yaml_content(content_str)

    def _parse_json_content(self, content_str: str) -> Dict[str, Any]:
        """
        Internal method to parse JSON content.

        Args:
            content_str: String content to parse

        Returns:
            Parsed JSON content as a dictionary, or None if parsing fails
        """
        try:
            if isinstance(content_str, str) and content_str.strip():
                stripped = content_str.strip()
                # More robust JSON detection
                if stripped.startswith(('{', '[')):
                    return json.loads(stripped)
            return None
        except json.JSONDecodeError as e:
            self.logger.debug(f"Could not parse content as JSON: {str(e)}")
            return None

    def _parse_yaml_content(self, content_str: str) -> Dict[str, Any]:
        """
        Internal method to parse YAML content.

        Args:
            content_str: String content to parse

        Returns:
            Parsed YAML content as a dictionary, or None if parsing fails
        """
        if not YAML_AVAILABLE:
            self.logger.warning("YAML parsing requested but PyYAML not installed. Install with: pip install PyYAML")
            return None

        try:
            if isinstance(content_str, str) and content_str.strip():
                result = yaml.safe_load(content_str.strip())
                # yaml.safe_load can return None for empty content, or primitives
                # We want to return dict/list structures
                if isinstance(result, (dict, list)):
                    return result
            return None
        except yaml.YAMLError as e:
            self.logger.debug(f"Could not parse content as YAML: {str(e)}")
            return None

    def detect_content_format(self, results: List[Dict[str, Any]]) -> str:
        """
        Detect the format of the content in the results.

        Args:
            results: Results from a tool call

        Returns:
            'json', 'yaml', or 'unknown'
        """
        content_str = self.extract_content_from_results(results)

        if not isinstance(content_str, str) or not content_str.strip():
            return 'unknown'

        stripped = content_str.strip()

        # Check for JSON first (more definitive indicators)
        if stripped.startswith(('{', '[')):
            # Try to parse as JSON to confirm
            try:
                json.loads(stripped)
                return 'json'
            except json.JSONDecodeError:
                pass

        # Check for YAML if JSON parsing failed
        if YAML_AVAILABLE:
            try:
                result = yaml.safe_load(stripped)
                # If it parses as YAML and isn't just a simple string/primitive
                if isinstance(result, (dict, list)):
                    return 'yaml'
            except yaml.YAMLError:
                pass

        # Additional heuristics for YAML (when parsing fails but structure looks like YAML)
        yaml_indicators = [
            ':' in stripped and '\n' in stripped,  # Key-value pairs with newlines
            stripped.startswith('- '),  # List items
            '---' in stripped,  # YAML document separator
            stripped.count('\n') > 0 and ': ' in stripped  # Multi-line with key-value
        ]

        if any(yaml_indicators):
            return 'yaml'

        return 'unknown'


    def get_available_tool_names(self) -> List[str]:
        """
        Get a list of all available tool names that can be used in run_tool_test.

        Returns:
            List of tool names
        """
        tool_names = []
        try:
            tool_names = list(self.tool_chest._tool_name_to_instance_map.keys())
        except Exception as e:
            self.logger.error(f"Error getting tool names: {str(e)}")

        return tool_names

