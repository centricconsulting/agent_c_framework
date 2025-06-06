"""MCP ToolChest Server implementation for Agent C tools.

This module provides the MCPToolChestServer class which allows exposing
a ToolChest of Agent C tools through an MCP server interface using the
official MCP Python SDK.
"""

import sys
import json
import logging


from contextlib import asynccontextmanager
from typing import Any, Callable, Dict, List, Optional, Tuple, Union, AsyncIterator, get_type_hints

from mcp.server.fastmcp import FastMCP
# MCP parameter handling is done through standard type annotations and dictionaries

from agent_c.toolsets import ToolChest, MCPToolChest, ToolCache
from agent_c.chat.session_manager import  ChatSessionManager
from agent_c_tools.tools.workspace.local_storage import LocalStorageWorkspace

from agent_c_tools.server.config import ServerConfig, MCPServersConfig
from agent_c_tools.server.discovery import discover_and_load_tools

logger = logging.getLogger(__name__)


def is_test_environment() -> bool:
    """Detect if we're running in a test environment.
    
    Returns:
        True if running in a test environment, False otherwise
    """
    return 'pytest' in sys.modules



class MCPToolChestServer:
    """A server that exposes a ToolChest through the MCP protocol.
    
    This class allows any ToolChest to be exposed as an MCP server, making its
    tools available to other systems that can communicate via the MCP protocol.
    
    The MCPToolChestServer acts as a proxy/adapter between the MCP protocol
    and the Agent C ToolChest. It does not duplicate tool registry or calling
    mechanisms - instead, it dynamically creates functions with signatures that
    match the OpenAI schema from the ToolChest and registers those with MCP.
    
    When tools are called via MCP, the server delegates the execution to the
    ToolChest's existing call_tools mechanism, ensuring all tools work consistently
    regardless of whether they're called directly or through the MCP server.
    
    Key technical details:
    - Tool schemas are sourced directly from the ToolChest's OpenAI schemas
    - Functions with proper signatures are dynamically created to match each schema
    - All tool execution is delegated to the ToolChest's existing call_tools method
    - No duplicate tool registry or execution logic is implemented
    
    Features:
    - Tool discovery and execution via MCP protocol
    - Automatic conversion of ToolChest tools to MCP tools with proper signatures
    - Tool prompt/example support for better documentation
    - Security features including authentication and access control
    - Rate limiting to prevent abuse
    - Configuration through files, environment variables, or direct settings
    """
    
    def __init__(self,
                 tool_chest: Optional[Union[ToolChest, MCPToolChest]] = None,
                 config_path: Optional[str] = None,
                 server_config: Optional[Dict[str, Any]] = None,
                 mcp_servers_config_path: Optional[str] = None):
        """Initialize an MCP server for a ToolChest.
        
        Args:
            tool_chest: Optional ToolChest or MCPToolChest to expose through MCP
            config_path: Path to a configuration file (YAML or JSON)
            server_config: Optional configuration dictionary for the MCP server
                - name: Name of the server (default: "Agent C Tools")
                - host: Host to bind to (default: 127.0.0.1)
                - port: Port to listen on (default: 8000)
                - allowed_tools: Specific tools to expose (if None, all tools are exposed)
                - dependencies: Additional dependencies to load
            mcp_servers_config_path: Optional path to an MCP servers configuration file
        """
        # Initialize configuration
        self.config = ServerConfig()
        
        # Load configuration from file if provided
        if config_path:
            self.config = ServerConfig.from_file(config_path)
        
        # Override configuration with provided server_config
        if server_config:
            for key, value in server_config.items():
                if key == "allowed_tools" and value is not None:
                    self.config.security.allowed_tools = value
                    self.config.security.__post_init__()  # Recompile patterns
                elif hasattr(self.config, key):
                    setattr(self.config, key, value)
        
        # Load MCP servers configuration if provided
        if mcp_servers_config_path:
            try:
                self.config.mcp_servers = MCPServersConfig.from_file(mcp_servers_config_path)
            except (FileNotFoundError, ValueError) as e:
                logger.warning(f"Could not load MCP servers config file: {e}")
        
        # Initialize tool chest
        if tool_chest is None:
            # If no tool chest provided, create an MCPToolChest
            self.tool_chest = MCPToolChest()
        elif isinstance(tool_chest, ToolChest) and not isinstance(tool_chest, MCPToolChest):
            # If a regular ToolChest was provided, convert it to an MCPToolChest
            mcp_tool_chest = MCPToolChest()
            # Copy over any existing tools
            for toolset in tool_chest.active_tools.values():
                mcp_tool_chest.add_tool_instance(toolset)
            self.tool_chest = mcp_tool_chest
        else:
            # Use the provided tool chest as is
            self.tool_chest = tool_chest
        
        # Apply MCP server configurations if any
        self._apply_mcp_server_configs()
        
        # Initialize MCP server
        self.mcp_server = FastMCP(
            self.config.name,
            dependencies=[],  # Dependencies are handled separately
            host=self.config.host,
            port=self.config.port,
            lifespan=self.mcp_server_lifespan
        )
        
        # Initialize server state
        self.running = False

    @asynccontextmanager
    async def mcp_server_lifespan(self, server: FastMCP) -> AsyncIterator[Dict[str, Any]]:
        """Lifespan context manager for the MCP server.

        This function is used to initialize resources when the server starts
        and clean them up when it stops.

        Yields:
            Dictionary with lifespan context (currently empty)
        """
        # Initialize resources here
        logger.info("Initializing MCP server resources")
        await self._discover_and_load_tools()
        self._register_tools()

        logger.info(f"Server is serving {len(self.tool_chest.active_tools)} tools")

        try:
            yield {}
        finally:
            # Clean up resources here
            logger.info("Cleaning up MCP server resources")
            
            # If using MCPToolChest, shut down MCP servers
            if isinstance(self.tool_chest, MCPToolChest):
                logger.info("Shutting down MCP servers...")
                try:
                    await self.tool_chest.shutdown()
                except Exception as e:
                    logger.error(f"Error shutting down MCP servers: {e}")

    def _apply_mcp_server_configs(self) -> None:
        """Apply MCP server configurations from the ServerConfig to the MCPToolChest.
        
        This method adds all configured MCP servers to the MCPToolChest instance.
        Supports both STDIO and SSE transport types.
        """
        if not isinstance(self.tool_chest, MCPToolChest):
            logger.warning("Cannot apply MCP server configs - tool chest is not an MCPToolChest")
            return
        
        # Add each configured server to the MCPToolChest
        for server_id, server_config in self.config.mcp_servers.servers.items():
            logger.info(f"Adding MCP server configuration for {server_id} using {server_config.transport_type} transport")
            
            # Handle different transport types
            if server_config.transport_type == 'stdio':
                # For STDIO transport
                if not server_config.command:
                    logger.warning(f"Skipping MCP server {server_id} - no command provided for STDIO transport")
                    continue
                    
                self.tool_chest.add_server(
                    server_id,
                    command=server_config.command,
                    args=server_config.args,
                    env=server_config.env
                )
            elif server_config.transport_type == 'sse':
                # For SSE transport
                if not server_config.url:
                    logger.warning(f"Skipping MCP server {server_id} - no URL provided for SSE transport")
                    continue
                    
                # Add SSE server configuration
                config = {
                    'transport_type': 'sse',
                    'url': server_config.url,
                }
                
                # Add optional fields if provided
                if server_config.headers:
                    config['headers'] = server_config.headers
                if server_config.timeout is not None:
                    config['timeout'] = server_config.timeout
                    
                self.tool_chest.add_server(server_id, **config)
            else:
                logger.warning(f"Unsupported transport type '{server_config.transport_type}' for MCP server {server_id}")
                continue
    
    async def _discover_and_load_tools(self) -> None:
        """Discover and load tools based on configuration.
        
        If the tool chest is already initialized with tools, this will use those tools.
        Otherwise, it will initialize tools based on the server configuration.
        """
        # Check if tools are already initialized
        if self.tool_chest.active_tools:
            logger.info(f"Using existing toolchest with {len(self.tool_chest.active_tools)} active tools")
            # Make sure the OpenAI schemas are also initialized
            if not self.tool_chest.active_open_ai_schemas:
                logger.info("Refreshing OpenAI schemas in the tool chest")
                # Ensure schemas are regenerated for the tools
                for toolset in self.tool_chest.available_tools.values():
                    self.tool_chest._active_tool_schemas += toolset.tool_schemas
            return
            

        # If we get here, we need to discover and load tools for a regular ToolChest
        logger.info("Discovering and loading tools")
        
        # Create tool options for initialization
        tool_cache_dir = ".tool_cache"
        tool_cache = ToolCache(cache_dir=tool_cache_dir)
        session_manager = ChatSessionManager()
        workspaces = []

        try:
            local_workspaces = json.load(open(".local_workspaces.json", "r"))
            for ws in local_workspaces['local_workspaces']:
                workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            pass

        await session_manager.init("TEMP_PLACEHOLDER", "TEMP_PLACEHOLDER")
        tool_opts = {'tool_cache': tool_cache, 'session_manager': session_manager,
                     'workspaces': workspaces}

        await discover_and_load_tools(
            self.tool_chest,
            self.config.tools.discover,
            self.config.tools.imports,
            self.config.tools.config,
            tool_opts
        )
    
    def _register_tools(self) -> None:
        """Register tools from the tool chest with the MCP server.
        
        This method iterates through the OpenAI schemas from the active toolchest
        and registers each tool with the MCP server.
        """
        registered_count = 0
        
        # Use the OpenAI schemas from the active tool chest
        for schema in self.tool_chest.active_open_ai_schemas:
            full_tool_name = schema['function']['name']
            
            # Check if tool is allowed
            if not self.config.security.is_tool_allowed(full_tool_name):
                logger.info(f"Skipping tool {full_tool_name} - not in allowed tools")
                continue
            
            # Register the tool
            self._register_tool(schema)
            registered_count += 1
        
        logger.info(f"Registered {registered_count} tools with MCP server")
    
    def _register_tool(self, schema: dict) -> None:
        """Register a single tool with the MCP server using its OpenAI schema.
        
        This method dynamically creates a function with a signature matching the
        OpenAI schema, and registers it with the MCP server. When called, the
        function delegates to the ToolChest to execute the actual tool.
        
        Args:
            schema: The OpenAI function schema defining the tool
        """
        function_def = schema['function']
        full_tool_name = function_def['name']
        description = function_def.get('description', '')
        parameters_schema = function_def.get('parameters', {})
        
        logger.debug(f"Registering tool: {full_tool_name}")
        
        # Extract parameter information from schema
        param_names, param_annotations, param_defaults = self._extract_parameter_info(parameters_schema)
        
        # Create tool implementation function that delegates to the toolchest
        tool_impl = self._create_tool_implementation(full_tool_name, param_names)
        
        # Create and register the MCP function with proper signature
        self._create_and_register_mcp_function(
            full_tool_name, 
            description, 
            param_names, 
            param_annotations, 
            param_defaults, 
            tool_impl
        )
    
    def _extract_parameter_info(self, parameters_schema: dict) -> Tuple[List[str], Dict[str, Any], Dict[str, Any]]:
        """Extract parameter information from an OpenAI schema.
        
        Args:
            parameters_schema: The parameters section of an OpenAI function schema
            
        Returns:
            Tuple containing:
                - List of parameter names
                - Dictionary mapping parameter names to type annotations
                - Dictionary mapping parameter names to default values
        """
        param_names = []
        param_annotations = {}
        param_defaults = {}
        
        if 'properties' in parameters_schema:
            for param_name, param_props in parameters_schema['properties'].items():
                param_names.append(param_name)
                
                # Set type annotation based on schema type
                param_type = param_props.get('type')
                if param_type == 'string':
                    param_annotations[param_name] = str
                elif param_type == 'integer':
                    param_annotations[param_name] = int
                elif param_type == 'number':
                    param_annotations[param_name] = float
                elif param_type == 'boolean':
                    param_annotations[param_name] = bool
                elif param_type == 'array':
                    param_annotations[param_name] = list
                elif param_type == 'object':
                    param_annotations[param_name] = dict
                else:
                    param_annotations[param_name] = Any
                
                # Set default value for optional parameters
                if 'required' in parameters_schema and param_name not in parameters_schema['required']:
                    param_defaults[param_name] = None
        
        return param_names, param_annotations, param_defaults
    
    def _create_tool_implementation(self, full_tool_name: str, param_names: List[str]) -> Callable:
        """Create a tool implementation function that delegates to the toolchest.
        
        This creates a function that takes the parameters extracted from the schema,
        formats them appropriately for the toolchest call_tools method, and handles
        the response processing.
        
        Args:
            full_tool_name: The full name of the tool (toolset-method)
            param_names: List of parameter names from the schema
            
        Returns:
            An async function that implements the tool by delegating to toolchest
        """
        async def tool_impl(*args, **kwargs):
            # Merge positional and keyword arguments
            call_kwargs = dict(zip(param_names, args))
            call_kwargs.update(kwargs)
            
            # Create a mock tool call in the format expected by ToolChest.call_tools
            tool_call = {
                "id": f"mcp-{full_tool_name}-{id(call_kwargs)}",  # Generate a unique ID
                "name": full_tool_name,
                "input": call_kwargs  # MCP expects 'input' format (Claude style)
            }
            
            try:
                # Call the tool through the tool chest
                results = await self.tool_chest.call_tools([tool_call], format_type="claude")
                
                # Extract the result from the response
                # The result will be in the second message, in the 'content' field of the first item
                if len(results) >= 2 and 'content' in results[1] and len(results[1]['content']) > 0:
                    tool_result = results[1]['content'][0]['content']
                    return tool_result
                else:
                    logger.error(f"No result returned from tool {full_tool_name}")
                    return "Error: No result returned from tool"
            except Exception as e:
                logger.error(f"Error executing tool {full_tool_name}: {e}")
                raise
        
        return tool_impl
    
    def _create_and_register_mcp_function(
        self, 
        full_tool_name: str, 
        description: str, 
        param_names: List[str], 
        param_annotations: Dict[str, Any], 
        param_defaults: Dict[str, Any], 
        tool_impl: Callable
    ) -> None:
        """Create and register a function with the MCP server.
        
        This method dynamically creates a function with the exact signature 
        required by the schema and registers it with the MCP server.
        
        Args:
            full_tool_name: The full name of the tool (toolset-method)
            description: The tool description
            param_names: List of parameter names
            param_annotations: Dictionary mapping parameter names to type annotations
            param_defaults: Dictionary mapping parameter names to default values
            tool_impl: The implementation function to delegate to
        """
        # Create dynamic function with proper signature using exec
        # This is necessary because MCP inspects the function signature
        param_str = ', '.join([f"{p}={param_defaults.get(p, 'None')}" if p in param_defaults else p for p in param_names])
        func_name = f"mcp_tool_{full_tool_name.replace('-', '_')}"
        
        # Define the function dynamically with exactly the right signature
        exec_globals = {'tool_impl': tool_impl, 'tool_annotations': param_annotations}
        exec_code = f"""\nasync def {func_name}({param_str}):\n    return await tool_impl({', '.join(param_names)})\n"""
        
        # Execute the dynamic function definition
        exec(exec_code, exec_globals)
        
        # Get the defined function and set annotations 
        mcp_tool = exec_globals[func_name]
        mcp_tool.__annotations__ = param_annotations
        
        # Register with MCP server - without parameters, relying on function signature
        self.mcp_server.tool(name=full_tool_name, description=description)(mcp_tool)
    
    def run(self) -> None:
        """Run the MCP server.
        
        This method initializes the tools, registers them with the MCP server,
        and then calls the MCP server's run method, which blocks until the server
        is stopped.
        """
        if self.running:
            logger.warning("MCP server is already running")
            return
        
        try:
            logger.info(f"Starting MCP server at {self.config.host}:{self.config.port}")
            
            # Set up asyncio event loop for initialization
            #loop = asyncio.new_event_loop()
            #asyncio.set_event_loop(loop)
            
            # Initialize tools in the event loop
            #loop.run_until_complete(self._discover_and_load_tools())
            
            # Register tools with MCP server
            #self._register_tools()
            
            # Mark the server as running
            self.running = True
            
            # Log server information
            #logger.info(f"Server is serving {len(self.tool_chest.active_tools)} tools")
            
            # Run the MCP server - this will block until the server is stopped
            # Let MCP manage its own event loop
            self.mcp_server.run(transport="sse")
            
        except Exception as e:
            logger.error(f"Error running MCP server: {e}")
            raise
    
    def stop(self) -> None:
        """Stop the MCP server.
        
        Note: This method is primarily for API compatibility. The actual server
        shutdown happens when the FastMCP.run() method exits (via Ctrl-C or other means).
        The shutdown of MCP servers happens in the lifespan context manager.
        """
        if not self.running:
            logger.warning("MCP server is not running")
            return
        
        logger.info("Stopping MCP server")
        self.running = False
        logger.info("MCP server stopped")

        
        # The actual shutdown of the server happens through FastMCP's own mechanisms
    
    def is_running(self) -> bool:
        """Check if the MCP server is running.
        
        Returns:
            True if the server is running, False otherwise
        """
        return self.running