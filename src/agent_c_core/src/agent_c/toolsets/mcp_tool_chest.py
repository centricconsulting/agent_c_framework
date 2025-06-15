"""MCP Tool Chest implementation for Agent C.

This module provides the MCPToolChest class which extends the standard ToolChest
to include support for MCP servers and their tools.
"""

import asyncio
import json
import logging
import os
from pathlib import Path
from typing import Any, Dict, List, Optional, Type, Union

import yaml

from agent_c.toolsets.tool_chest import ToolChest
from agent_c.toolsets.mcp_server import MCPServer
from agent_c.toolsets.mcp_toolset import MCPToolset


class MCPToolChest(ToolChest):
    """A tool chest that includes support for MCP servers.
    
    Manages multiple MCP server connections and makes their tools available
    as standard Agent C tools.
    """
    
    def __init__(self, **kwargs):
        """Initialize the MCPToolChest.
        
        Args:
            **kwargs: Arguments to pass to the ToolChest base class including:
                - available_toolset_classes: List of toolset classes available for activation
                - essential_toolset_names: List of names of toolsets that should always be active
                - mcp_servers: Dict of MCPServer instances to include
        """
        super().__init__(kwargs.get('tool_cache', None))
        self.mcp_servers: Dict[str, MCPServer] = kwargs.get("mcp_servers", {})
        self.logger = logging.getLogger("agent_c.mcp_tool_chest")
    
    def load_config(self, config_path: str):
        """Load MCP server configurations from a file.
        
        Args:
            config_path: Path to the configuration file (YAML or JSON)
            
        Returns:
            Dict[str, Any]: Loaded configuration
            
        Raises:
            FileNotFoundError: If the config file does not exist
            ValueError: If the config file format is invalid
        """
        config_path = Path(config_path)
        if not config_path.exists():
            raise FileNotFoundError(f"Configuration file not found: {config_path}")
        
        # Determine file format and load accordingly
        if config_path.suffix.lower() in [".yaml", ".yml"]:
            with open(config_path, "r") as f:
                config = yaml.safe_load(f)
        elif config_path.suffix.lower() == ".json":
            with open(config_path, "r") as f:
                config = json.load(f)
        else:
            raise ValueError(f"Unsupported configuration file format: {config_path.suffix}")
        
        # Process servers configuration
        if "servers" not in config:
            raise ValueError("Invalid configuration: 'servers' key is missing")
        
        # Add each server from the configuration
        for server_id, server_config in config["servers"].items():
            self.add_server(server_id, **server_config)
        
        self.logger.info(f"Loaded {len(config['servers'])} MCP server configurations from {config_path}")
        return config
    
    def add_server(self, server_id: str, **server_config):
        """Add an MCP server programmatically.
        
        Args:
            server_id: Unique identifier for this server
            **server_config: Configuration for the server including:
                - command: Command to run the server
                - args: List of command line arguments
                - env: Environment variables for server
        """
        if server_id in self.mcp_servers:
            self.logger.warning(f"Replacing existing MCP server configuration for {server_id}")
        
        # Create MCPServer instance
        server = MCPServer(server_id, server_config)
        self.mcp_servers[server_id] = server
        self.logger.info(f"Added MCP server configuration for {server_id}")
    
    async def connect_server(self, server_id: str):
        """Connect to a specific MCP server and create its toolset.
        
        Args:
            server_id: The ID of the server to connect
            
        Returns:
            bool: True if connection was successful, False otherwise
        """
        server = self.mcp_servers.get(server_id)
        if not server:
            self.logger.warning(f"No MCP server found with ID: {server_id}")
            return False
            
        try:
            success = await server.connect()
            
            # If successfully connected, create a toolset for this server
            if success:
                toolset = MCPToolset(server)
                await self.add_tool_instance(toolset)
                self.logger.info(f"Added toolset for MCP server {server_id}")
            else:
                self.logger.warning(f"Failed to connect to MCP server {server_id}")
                
            return success
        except Exception as e:
            self.logger.error(f"Error connecting to MCP server {server_id}: {e}")
            return False
    
    async def connect_servers(self):
        """Connect to all configured MCP servers.
        
        Returns:
            Dict[str, bool]: Dictionary mapping server IDs to connection success status
        """
        connection_results = {}
        connection_tasks = []
        
        # Create connection tasks for all servers
        for server_id in self.mcp_servers.keys():
            task = asyncio.create_task(self.connect_server(server_id))
            connection_tasks.append((server_id, task))
        
        # Wait for all connections to complete
        for server_id, task in connection_tasks:
            try:
                success = await task
                connection_results[server_id] = success
            except Exception as e:
                self.logger.error(f"Error handling connection to MCP server {server_id}: {e}")
                connection_results[server_id] = False
        
        return connection_results
    
    async def disconnect_server(self, server_id: str):
        """Disconnect from a specific MCP server and deactivate its toolset.
        
        Args:
            server_id: The ID of the server to disconnect
            
        Returns:
            bool: True if disconnection was successful, False otherwise
        """
        server = self.mcp_servers.get(server_id)
        if not server:
            self.logger.warning(f"No MCP server found with ID: {server_id}")
            return False
            
        if not server.connected:
            # Nothing to do if not connected
            return True
            
        try:
            # First deactivate the toolset if it exists
            toolset_name = f"MCPToolset_{server_id}"
            self.deactivate_toolset(toolset_name)
            
            # Then disconnect the server
            success = await server.disconnect()
            return success
        except Exception as e:
            self.logger.error(f"Error disconnecting from MCP server {server_id}: {e}")
            return False
    
    async def disconnect_servers(self):
        """Disconnect from all MCP servers.
        
        Returns:
            Dict[str, bool]: Dictionary mapping server IDs to disconnection success status
        """
        disconnection_results = {}
        disconnection_tasks = []
        
        # Create disconnection tasks for all connected servers
        for server_id, server in self.mcp_servers.items():
            if server.connected:
                task = asyncio.create_task(self.disconnect_server(server_id))
                disconnection_tasks.append((server_id, task))
        
        # Wait for all disconnections to complete
        for server_id, task in disconnection_tasks:
            try:
                success = await task
                disconnection_results[server_id] = success
            except Exception as e:
                self.logger.error(f"Error handling disconnection from MCP server {server_id}: {e}")
                disconnection_results[server_id] = False
        
        return disconnection_results
    
    async def activate_toolset(self, toolset_name_or_names: Union[str, List[str]], tool_opts: Optional[Dict[str, any]] = None) -> bool:
        """Override to handle MCP server connections for toolsets being activated.
        
        Args:
            toolset_name_or_names: A single toolset name or list of toolset names to activate
            **kwargs: Additional arguments to pass to post_init if needed
            
        Returns:
            bool: True if all toolsets were activated successfully, False otherwise
        """
        # First check if any of the requested toolsets are MCP server toolsets
        toolset_names = [toolset_name_or_names] if isinstance(toolset_name_or_names, str) else toolset_name_or_names
        
        # Keep track of success status
        success = True
        
        # Handle regular toolsets first
        regular_toolsets = []
        for name in toolset_names:
            # Check if this is an MCP toolset by name pattern
            if name.startswith("MCPToolset_"):
                # Extract server ID from toolset name
                server_id = name[len("MCPToolset_"):]
                # Connect to this server if it exists
                if server_id in self.mcp_servers:
                    server_success = await self.connect_server(server_id)
                    if not server_success:
                        success = False
                else:
                    self.logger.warning(f"No MCP server found with ID: {server_id}")
                    success = False
            else:
                # Regular toolset, add to list for regular activation
                regular_toolsets.append(name)
        
        # Activate regular toolsets
        if regular_toolsets:
            regular_success = await super().activate_toolset(regular_toolsets, tool_opts)
            if not regular_success:
                success = False
        
        return success
    
    def deactivate_toolset(self, toolset_name_or_names: Union[str, List[str]]) -> bool:
        """Override to handle MCP server disconnections for toolsets being deactivated.
        
        Args:
            toolset_name_or_names: A single toolset name or list of toolset names to deactivate
            
        Returns:
            bool: True if all toolsets were deactivated successfully, False otherwise
        """
        # Process normally, but don't disconnect servers yet
        success = super().deactivate_toolset(toolset_name_or_names)
        
        # We don't need to disconnect servers when toolsets are deactivated
        # The server connections will be maintained in case the toolset is reactivated
        # Actual disconnection happens during shutdown or explicit disconnect request
        
        return success
    
    async def init_tools(self, **kwargs):
        """Override of ToolChest.init_tools to also initialize MCP servers.
        
        Args:
            **kwargs: Arguments to pass to the parent class and tools during initialization
        """
        # First init regular tools
        await super().init_tools(**kwargs)
        
        # Then connect and initialize MCP servers if we're in backward compatibility mode
        # or if any MCP toolsets are in the essential toolsets list
        essential_has_mcp = any(name.startswith("MCPToolset_") for name in self._ToolChest__essential_toolset_names)
        
        if not self._ToolChest__essential_toolset_names or essential_has_mcp:
            connection_results = await self.connect_servers()
            
            # Log connection results
            successful = sum(1 for result in connection_results.values() if result)
            failed = sum(1 for result in connection_results.values() if not result)
            self.logger.info(f"Initialized MCP servers and tools: {successful} connected, {failed} failed")
            
            if failed > 0:
                self.logger.warning(f"Some MCP servers failed to connect. Check logs for details.")
            
            return connection_results
        
        return {}
    
    async def shutdown(self):
        """Shutdown all MCP servers and perform cleanup.
        
        This method should be called when the application is shutting down
        to ensure proper cleanup of MCP server connections.
        
        Returns:
            Dict[str, bool]: Dictionary mapping server IDs to disconnection success status
        """
        self.logger.info("Shutting down MCP servers...")
        return await self.disconnect_servers()