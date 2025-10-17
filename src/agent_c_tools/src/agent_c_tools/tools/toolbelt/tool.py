from typing import TYPE_CHECKING

from agent_c.toolsets import Toolset, json_schema

if TYPE_CHECKING:
    from agent_c_api.core.realtime_bridge import RealtimeBridge

class ToolbeltTools(Toolset):
    """
    Provides your agent with capabilities to equip and remove toolsets.
    """

    def __init__(self, **kwargs):
        """
        Initialize the ToolbeltTools.

        Args:
            **kwargs: Keyword arguments passed to parent Toolset class.
        """
        super().__init__(**kwargs, name='toolbelt')

    @json_schema(
        description="Equip a toolset so it's tools can be used",
        params={
            'toolset_name': {
                'type': 'string',
                'description': 'The name of the toolset to equip',
                'required': True
            }
        }
    )
    async def equip(self, **kwargs) -> str:
        """
       Add a toolset to the agent's available tools via the RealTimeBridge.

        Args:
            toolset_name (str): The name of the toolset to equip.

        Returns:
            str: Success message or error description
        """
        toolset_name = kwargs.get('toolset_name')
        tool_context = kwargs.get('tool_context')

        try:
            if not toolset_name:
                return "ERROR: toolset_name parameter is required"
                
            if not tool_context or 'bridge' not in tool_context:
                return "ERROR: Bridge not available in tool context"

            bridge: 'RealtimeBridge'  = tool_context['bridge']

            equipped = await bridge.add_tool(toolset_name)
            if not equipped:
                return f"ERROR: Failed to equip toolset: {toolset_name}"

            await bridge.send_system_message(f"Agent has equipped {toolset_name} toolset.", severity="info")

            return f"{toolset_name} equipped"
            
        except Exception as e:
            error_msg = f"Failed to equip toolset: {toolset_name} {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Remove a toolset that is no longer needed.",
        params={
            'toolset_name': {
                'type': 'string',
                'description': 'The name of the toolset to remove',
                'required': True
            }
        }
    )
    async def remove(self, **kwargs) -> str:
        """
        Remove a toolset from the agent's available tools via the RealTimeBridge.

        Args:
            toolset_name (str): The name of the toolset to equip.

        Returns:
            str: Success message or error description
        """
        toolset_name = kwargs.get('toolset_name')
        tool_context = kwargs.get('tool_context')
        if not toolset_name:
            return "ERROR: toolset_name parameter is required"

        if not tool_context or 'bridge' not in tool_context:
            return "ERROR: Bridge not available in tool context"

        bridge: 'RealtimeBridge' = tool_context['bridge']

        try:
            removed = await bridge.remove_tool(toolset_name)
            if not removed:
                return f"ERROR: Failed to remove toolset: {toolset_name}"

            await bridge.send_system_message(f"Agent has removed {toolset_name} toolset.", severity="info")

            return f"{toolset_name} removed"

        except Exception as e:
            error_msg = f"Failed to remove toolset: {toolset_name} {str(e)}"
            await bridge.send_system_message(f"Agent has removed {toolset_name} toolset.", severity="error")
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get the list of toolset names",
        params={}
    )
    async def list(self, **kwargs) -> str:
        """
        Get a list of available toolset names.
        """
        try:
            catalog = Toolset.get_client_registry()
            names = [tool.name for tool in catalog]

            return self._yaml_dump({"toolsets": names})

        except Exception as e:
            error_msg = f"Failed to list toolsets: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get the details on an available toolset",
        params={
            'toolset_name': {
                'type': 'string',
                'description': 'The name of the toolset to get details for',
                'required': True
            }
        }
    )
    async def details(self, **kwargs) -> str:
        """
        Get a list of available toolset names.
        """
        toolset_name = kwargs.get('toolset_name')
        if not toolset_name:
            return "ERROR: toolset_name parameter is required"

        try:
            catalog = Toolset.get_client_registry()
            if toolset_name:
                catalog = [tool for tool in catalog if tool.name == toolset_name]

            if not catalog:
                return f"ERROR: No toolset found with name: {toolset_name}"

            return self._yaml_dump(catalog)


        except Exception as e:
            error_msg = f"Failed to get details for toolset {toolset_name}: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get the list of currently active/equipped toolset names, with optional filtering by validity",
        params={
            'show_invalid': {
                'type': 'boolean',
                'description': 'Whether to include invalid tools in the results (default: True)',
                'required': True
            }
        }
    )
    async def active(self, **kwargs) -> str:
        """
        Get a list of currently active/equipped toolset names.
        Can optionally include or exclude invalid tools.
        """
        try:
            tool_context = kwargs.get('tool_context')
            show_invalid = kwargs.get('show_invalid', True)

            # Get active tools from bridge's tool_chest
            if not tool_context or 'bridge' not in tool_context:
                return "ERROR: Bridge not available in tool context"

            bridge: 'RealtimeBridge' = tool_context['bridge']
            
            if not hasattr(bridge, 'tool_chest'):
                return "ERROR: Bridge does not have tool_chest"

            # Get available tools from the tool_chest
            active_tools = bridge.tool_chest.available_tools

            if not active_tools:
                return self._yaml_dump({"valid_toolsets": [], "invalid_toolsets": [], "total_active": 0})

            # Separate valid and invalid tools
            valid_tools = []
            invalid_tools = []

            for name, toolset in active_tools.items():
                # Check if toolset has tool_valid attribute and is valid
                if hasattr(toolset, 'tool_valid'):
                    if toolset.tool_valid:
                        valid_tools.append(name)
                    else:
                        invalid_tools.append(name)
                else:
                    # If no tool_valid attribute, assume valid (legacy support)
                    valid_tools.append(name)

            # Build result based on show_invalid parameter
            result = {
                "valid_toolsets": valid_tools
            }

            if show_invalid:
                result["invalid_toolsets"] = invalid_tools
                result["total_active"] = len(valid_tools) + len(invalid_tools)

            return self._yaml_dump(result)

        except Exception as e:
            error_msg = f"Failed to get active toolsets: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

# Register the toolset
Toolset.register(ToolbeltTools)