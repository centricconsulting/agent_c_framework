"""MCP Toolset implementation for Agent C.

This module provides the MCPToolset class which represents tools from an MCP server
as a standard Agent C toolset.
"""
import copy

from typing import Any, Callable, Dict, List, Optional, Type, Union

from grpc import server

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.mcp_server import MCPServer
from agent_c.util.structured_logging import get_logger, LoggingContext


class MCPToolset(Toolset):
    """A toolset representing tools from an MCP server.
    
    Dynamically creates methods for MCP tools to be used by Agent C.
    """
    
    def __init__(self, server: MCPServer, **kwargs):
        """Initialize the MCPToolset.
        
        Args:
            server: The MCPServer this toolset represents
            **kwargs: Additional arguments for the Toolset base class
        """
        # Initialize with a name based on the server ID
        name = kwargs.pop("name", f"mcp_{server.server_id}")
        super().__init__(name=name, use_prefix=False, **kwargs)
        self.openai_schemas = []
        self.server = server
        self.logger = get_logger(__name__)

        
        # Create dynamic methods for MCP tools
        self._create_tool_methods()
    
    def _create_tool_methods(self):
        """Dynamically create methods for each tool in the server.
        
        This method creates a Python method for each tool in the MCP server,
        with appropriate docstrings and type hints to work with Agent C.
        """
        with LoggingContext(operation="mcp_tool_registration", server_id=self.server.server_id):
            for tool_name, tool_info in self.server.tools.items():
                # Create a method that will call the MCP tool
                method = self._create_tool_method(tool_name, tool_info)
                
                # Add the method to the class instance
                method_name = f"{tool_name.replace('-', '_')}"
                setattr(self, method_name, method)
                schema = copy.deepcopy(method.schema)
                self.openai_schemas.append(schema)
                self.logger.info("Created method for MCP tool", 
                                method_name=method_name, 
                                mcp_tool_name=tool_name, 
                )
    
    def _create_tool_method(self, tool_name: str, tool_info: Dict[str, Any]) -> Callable:
        """Create a method that calls an MCP tool.
        
        Args:
            tool_name: Name of the MCP tool
            tool_info: Tool information including description and schema
            
        Returns:
            A callable method that will invoke the MCP tool
        """
        # Get tool schema and description
        schema = tool_info.get("schema", {})
        description = tool_info.get("description", f"MCP tool: {tool_name}")
        
        # Generate function signature from the schema
        param_names = []
        param_docs = []
        properties = {}
        required = []

        if "properties" in schema:
            param_names = list(schema["properties"].keys())
            for param_name, param_props in schema["properties"].items():
                param_desc = param_props.get("description", f"Parameter {param_name}")
                param_type = param_props.get("type", "any")
                param_docs.append(f"    {param_name}: {param_desc} ({param_type})")
                
                # Create property definition for schema
                properties[param_name] = {
                    "type": param_props.get("type", "string"),
                    "description": param_desc
                }
                
                # Add to required list if marked as required
                if param_name in schema.get("required", []):
                    required.append(param_name)
        
        # Build a proper docstring with parameter descriptions
        full_docstring = f"{description}\n\nArgs:\n{chr(10).join(param_docs)}\n\nReturns:\n    The result from the MCP tool execution"
        
        # Define the actual async method that will call the MCP tool
        async def mcp_tool_method(**kwargs):
            # Remove any kwargs that are not part of the MCP tool's expected parameters
            # This is to accommodate Agent C's method calling convention
            filtered_kwargs = {k: v for k, v in kwargs.items() if k in param_names}
            with LoggingContext(mcp_tool_name=tool_name,server_id=self.server_id):
                # Log the tool call
                self.logger.info("Calling MCP tool",
                                arguments=filtered_kwargs)

                # Call the MCP tool through the server
                try:
                    result = await self.server.call_tool(tool_name, filtered_kwargs)
                    self.logger.debug("Result from MCP tool",
                                     result_type=type(result).__name__)
                    return result
                except Exception as e:
                    self.logger.error("Error calling MCP tool",
                                     error=str(e))
                    raise RuntimeError(f"Failed to execute MCP tool '{tool_name}': {e}")
        
        # Set docstring and metadata for proper display in schemas
        mcp_tool_method.__doc__ = full_docstring
        mcp_tool_method.__name__ = f"{tool_name.replace('-', '_')}"
        
        # Create and attach schema attribute to the method (similar to what json_schema decorator does)
        method_schema = {
            "type": "function",
            "function": {
                "name": mcp_tool_method.__name__,
                "description": description,
                "parameters": {
                    "type": "object",
                    "properties": properties
                }
            }
        }
        
        # Add required parameters if any
        if required:
            method_schema["function"]["parameters"]["required"] = required
            
        # Attach schema to the method - this is what Toolset.__openai_schemas() looks for
        mcp_tool_method.schema = method_schema
        
        return mcp_tool_method