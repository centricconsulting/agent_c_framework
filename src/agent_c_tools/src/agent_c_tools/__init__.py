# agent_c_tools/__init__.py
import os
import pkgutil
import importlib
from typing import TYPE_CHECKING


def _discover_tools():
    tools_map = {}
    # Directory containing your tools
    tools_dir = os.path.join(os.path.dirname(__file__), "tools")

    # Process top-level directories
    for finder, package_name, ispkg in pkgutil.iter_modules([tools_dir]):
        if ispkg:
            # Check if this is a direct tool with tool.py
            tool_module_path = os.path.join(tools_dir, package_name, "tool.py")
            if os.path.exists(tool_module_path):
                # Direct tool - handle as before
                module_name = f"agent_c_tools.tools.{package_name}.tool"
                camel_name = "".join(word.capitalize() for word in package_name.split("_"))
                tool_class_name = f"{camel_name}Tools"
                tools_map[tool_class_name] = module_name
            else:
                # This is a container for multiple tools - check subdirectories
                subdirectory = os.path.join(tools_dir, package_name)
                for sub_finder, sub_package, sub_ispkg in pkgutil.iter_modules([subdirectory]):
                    if sub_ispkg:
                        # Check if this sub-package has a tool.py
                        sub_tool_path = os.path.join(subdirectory, sub_package, "tool.py")
                        if os.path.exists(sub_tool_path):
                            module_name = f"agent_c_tools.tools.{package_name}.{sub_package}.tool"
                            # You might want to customize the naming convention here
                            camel_sub_package = "".join(word.capitalize() for word in sub_package.split("_"))
                            tool_class_name = f"{camel_sub_package}Tools"
                            tools_map[tool_class_name] = module_name

    return tools_map


_tools_mapping = _discover_tools()

__all__ = list(_tools_mapping.keys()) + ['WebSearchTools']
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c_tools.tools.think.tool import ThinkTools
from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools

# PLANE tools auto-discovered via tool.py

def __getattr__(name: str):
    if name in _tools_mapping:
        module = importlib.import_module(_tools_mapping[name])
        tool_class = getattr(module, name)
        return tool_class
    # Handle manually registered tools
    if name == 'WebSearchTools':
        return WebSearchTools
    
    raise AttributeError(f"module {__name__!r} has no attribute {name!r}")


if TYPE_CHECKING:
    # For type checkers
    from agent_c_tools.tools.full import *
