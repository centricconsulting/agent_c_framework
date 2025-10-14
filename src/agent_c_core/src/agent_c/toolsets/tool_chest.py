import asyncio
import copy
import json
import logging
from typing import Type, List, Union, Dict, Any, Tuple, Optional


from agent_c.prompting.basic_sections.tool_guidelines import EndToolGuideLinesSection, BeginToolGuideLinesSection
from agent_c.prompting.prompt_section import PromptSection
from agent_c.toolsets.tool_set import Toolset
from agent_c.util.logging_utils import LoggingManager


class ToolChest:
    """
    A class representing a collection of toolsets that can be used by an agent.
    
    Attributes:
        __toolset_instances (dict[str, Toolset]): A private dictionary to store all instantiated toolsets.
        __active_toolset_instances (dict[str, Toolset]): A private dictionary to store currently active toolsets.
        __essential_toolsets (list[str]): A private list to store names of toolsets that should always be active.
        __available_toolset_classes (list[Type[Toolset]]): A private list to store all available toolset classes.
        __toolsets_awaiting_init (dict[str, Toolset]): A private dictionary to store toolsets created but awaiting post_init.
        __tool_opts (dict): A private dictionary to store the kwargs from the last init_tools call.
        _active_tool_schemas (List[dict]): A private list to store OpenAI schemas for active toolsets.
        _tool_name_to_instance_map (Dict[str, Toolset]): A mapping from tool function names to their toolset instance.
        logger (logging.Logger): An instance of a logger.
        
    Methods:
        activate_toolset(toolset_name_or_names: Union[str, List[str]], **kwargs) -> bool: Activate one or more toolsets by name.
        deactivate_toolset(toolset_name_or_names: Union[str, List[str]]) -> bool: Deactivate one or more toolsets by name.
        set_active_toolsets(toolset_names: List[str], **kwargs) -> bool: Set the complete list of active toolsets.
        activate_tool(tool_name: str, **kwargs) -> bool: Backward compatibility method for activating a toolset.
        add_tool_class(cls: Type[Toolset]): Add a new toolset class to the available toolsets.
        add_tool_instance(instance: Toolset, activate: bool = True): Add a new toolset instance directly.
        init_tools(**kwargs): Initialize toolsets based on essential toolsets configuration.
        call_tools(tool_calls: List[dict], format_type: str) -> List[dict]: Execute multiple tool calls concurrently.
        _execute_tool_call(function_id: str, function_args: Dict) -> Any: Execute a single tool call.
    """

    def __init__(self, **kwargs):
        """
        Initializes the ToolChest with toolset instances, toolset classes, and a logger.
        
        Args:
            **kwargs: Arbitrary keyword arguments. Supports:
                - available_toolset_classes: List of toolset classes available for activation
                - essential_toolset_names: List of names of toolsets that should always be active
                - (legacy) tool_classes: Alias for available_toolset_classes for backward compatibility
                - tool_cache: Optional ToolCache instance to use
                - session_manager: Optional SessionManager instance to use
        """
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()
        self.__toolset_instances: dict[str, Toolset] = {}  # All instantiated toolsets
        self.__available_toolset_classes = kwargs.get('available_toolset_classes',
                                                      kwargs.get('tool_classes', Toolset.tool_registry))
        self.__toolsets_awaiting_init = {}
        self.__tool_opts = {}
        self._tool_name_to_instance_map: Dict[str, Toolset] = {}
        self.tool_cache = kwargs.get('tool_cache')


    @property
    def available_toolset_classes(self) -> List:
        return self.__available_toolset_classes

    def _update_toolset_metadata(self):
        """
        Update tool sections, schemas, and maps based on active toolsets.
        """
        # Clear existing metadata
        self._tool_name_to_instance_map = {}
        
        # Update with data from active toolsets
        for toolset in self.__toolset_instances.values():
            # Add schemas and update function name mapping
            for schema in toolset.tool_schemas:
                if 'function' in schema and 'name' in schema['function']:
                    self._tool_name_to_instance_map[schema['function']['name']] = toolset

    async def activate_toolset(self, toolset_name_or_names: Union[str, List[str]], tool_opts: Optional[Dict[str, any]] = None) -> bool:
        """
        Activate one or more toolsets by name.
        
        Args:
            toolset_name_or_names: A single toolset name or list of toolset names to activate
            tool_opts: Additional arguments to pass to post_init if the toolset needs initialization
            init_only: If True, only initialize the toolset without activating it
            
        Returns:
            bool: True if all toolsets were activated successfully, False otherwise
        """
        # Convert to list if a single string is provided
        toolset_names = [toolset_name_or_names] if isinstance(toolset_name_or_names, str) else toolset_name_or_names

        self.logger.debug(f"Activating toolsets: {toolset_names} with options: {tool_opts}")
        # Track activation stack to prevent infinite recursion
        activation_stack = getattr(self, '_activation_stack', [])
        self._activation_stack = activation_stack
        
        # Track which toolsets are initialized during this activation call
        # This is used to ensure post_init is called in the correct order
        newly_instantiated = []
        
        success = True
        for name in toolset_names:
            # Skip if already active
            if name in self.__toolset_instances:
                continue
            
            # Check for circular dependencies
            if name in activation_stack:
                self.logger.warning(f"Circular dependency detected when activating {name}")
                success = False
                continue
            
            activation_stack.append(name)
                

            # Find the class for this toolset
            toolset_class = next((cls for cls in self.__available_toolset_classes
                                  if cls.__name__ == name), None)

            if not toolset_class:
                self.logger.warning(f"Toolset class {name} not found in available toolsets")
                success = False
                activation_stack.remove(name)
                continue

            required_tools = Toolset.get_required_tools(name)

            # Log dependencies for debugging
            if required_tools:
                self.logger.debug(f"Toolset {name} requires: {', '.join(required_tools)}")

                # Recursively activate required tools
                required_success = await self.activate_toolset(required_tools, tool_opts)
                if not required_success:
                    self.logger.warning(f"Failed to activate required tools for {name}")
                    success = False
                    activation_stack.remove(name)
                    continue

                # Verify all required tools are actually active now
                missing_tools = [tool for tool in required_tools if tool not in self.__toolset_instances]
                if missing_tools:
                    self.logger.warning(f"Required tools {missing_tools} for {name} not active despite activation attempt")
                    success = False
                    activation_stack.remove(name)
                    continue

            # Prepare tool_opts with current tool_chest
            local_tool_opts = self.__tool_opts
            if tool_opts is not None:
                local_tool_opts.update(tool_opts)

            # Always ensure tool_chest is set to self
            local_tool_opts['tool_chest'] = self

            # Pass tool_cache if we have it
            if hasattr(self, 'tool_cache') and self.tool_cache is not None:
                local_tool_opts['tool_cache'] = self.tool_cache

            # Create instance
            try:
                # Store for use in other methods
                self.__tool_opts = local_tool_opts

                # Create the toolset instance
                toolset_obj = toolset_class(**local_tool_opts)

                # Add to instances and active instances
                self.__toolset_instances[name] = toolset_obj
                # Track for post_init later
                newly_instantiated.append(name)

                self.logger.info(f"Created toolset instance {name}")
            except Exception as e:
                self.logger.exception(f"Error creating toolset {name}: {str(e)}", stacklevel=2)
                success = False

            activation_stack.remove(name)
        
        # Update metadata for active toolsets - do this before post_init to ensure
        # active_tools is properly populated for any toolset that needs to access it
        self._update_toolset_metadata()
        
        # Now run post_init on newly instantiated toolsets in order
        # This is done after all instances are created to ensure dependencies
        # can be accessed during post_init
        for name in newly_instantiated:
            try:
                toolset_obj = self.__toolset_instances.get(name)
                if toolset_obj:
                    await toolset_obj.post_init()
                    self.logger.debug(f"Completed post_init for toolset {name}")
            except Exception as e:
                self.logger.warning(f"Error in post_init for toolset {name}: {str(e)}")
                success = False
                # Don't remove from active instances as it may still be partially functional
        self.logger.debug(f"Active toolsets after activation: {list(self.__toolset_instances.keys())}.  Success key was: {success}")
        return success


    async def activate_tool(self, tool_name: str, tool_opts: Optional[Dict[str, any]] = None) -> bool:
        """
        Activates a tool by name (backward compatibility method).
        
        Args:
            tool_name: The name of the tool to activate.
            tool_opts: Additional arguments to pass to post_init if needed
            
        Returns:
            bool: True if the tool was activated successfully, False otherwise
        """
        return await self.activate_toolset(tool_name, tool_opts)


    @property
    def available_tools(self) -> dict[str, Toolset]:
        """
        Property that returns all instantiated toolset instances.
        
        Returns:
            dict[str, Toolset]: Dictionary of all instantiated tool instances.
        """
        return self.__toolset_instances

    @property
    def active_open_ai_schemas(self) -> List[dict]:
        """
        Property that returns the active tool instances in Open AI format.

        Returns:
            List[dict]: List of OpenAI schemas for active toolsets.
        """
        return self._active_tool_schemas



    def add_tool_class(self, cls: Type[Toolset]):
        """
        Add a new toolset class to the available toolsets.
        
        Args:
            cls (Type[Toolset]): The toolset class to add.
        """
        if cls not in self.__available_toolset_classes:
            self.__available_toolset_classes.append(cls)

    async def add_tool_instance(self, instance: Toolset, activate: bool = True):
        """
        Add a new toolset instance directly.
        
        Args:
            instance (Toolset): The toolset instance to add.
            activate (bool): Whether to also activate the toolset.
        """
        name = instance.__class__.__name__
        self.__toolset_instances[name] = instance
        self._update_toolset_metadata()



    async def init_tools(self, tool_opts: Dict[str, any]):
        """
        Initialize toolsets based on essential toolsets configuration.
        
        For backward compatibility, if no essential toolsets are specified,
        this will initialize all available toolsets.
        
        Args:
            tool_opts: Arguments to pass to toolset initialization.
        """
        # Create a copy of tool_opts to avoid modifying the original
        local_tool_opts = {}
        if tool_opts:
            local_tool_opts.update(tool_opts)
            
        # Add tool_cache if available
        if hasattr(self, 'tool_cache') and self.tool_cache is not None and 'tool_cache' not in local_tool_opts:
            local_tool_opts['tool_cache'] = self.tool_cache
            
        self.__tool_opts = local_tool_opts


    async def call_tool_internal(self, function_id: str, function_args: Dict[str,Any], tool_context: Dict[str,Any]) -> Optional[str]:
        """
        Internal method to call a single tool function.

        Args:
            function_id (str): The function identifier.
            function_args (Dict[str,Any]): Arguments to pass to the function.
            tool_context (Dict[str, Any]): Context to pass to the tool, including bridge and session info.

        Returns:
            Any: The result of the function call.
        """
        try:
            full_args = copy.deepcopy(function_args)
            full_args['tool_context'] = tool_context
            return await self._execute_tool_call(function_id, full_args)
        except Exception as e:
            self.logger.exception(f"Failed calling {function_id}. {e}", stacklevel=2)
            await tool_context['bridge'].send_system_message(f"# CRITICAL ERROR\n\nFailed calling {function_id}.\n{e}\n", "error")
            return None



    async def call_tools(self, tool_calls: List[dict], tool_context: Dict[str,Any], format_type: str = "claude") -> List[dict]:
        """
        Execute multiple tool calls concurrently and return the results.
        
        Args:
            tool_calls (List[dict]): List of tool calls to execute.
            format_type (str): The format to use for the results ("claude" or "gpt").
            
        Returns:
            List[dict]: Tool call results formatted according to the agent type.
        """
        async def make_call(tool_call: dict) -> Tuple[dict, dict]:
            # Common logic for executing a tool call
            # TODO: refactor this to common model and push the format back down
            if format_type == "claude":
                fn = tool_call['name']
                args = tool_call['input']
                ai_call = copy.deepcopy(tool_call)
            else:  # gpt
                fn = tool_call['name']
                # Handle the case where the test provides Claude format but expects GPT processing
                if 'arguments' in tool_call:
                    args = json.loads(tool_call['arguments'])
                elif 'input' in tool_call:
                    # Fallback to 'input' if 'arguments' is not available
                    args = tool_call['input']
                    # Add 'arguments' field to the tool_call for compatibility
                    tool_call['arguments'] = json.dumps(args)
                ai_call = {
                    "id": tool_call['id'],
                    "function": {"name": fn, "arguments": tool_call['arguments']},
                    'type': 'function'
                }
                
            try:

                full_args = copy.deepcopy(args)
                full_args['tool_context'] = tool_context
                function_response = await self._execute_tool_call(fn, full_args)
                
                if format_type == "claude":
                    call_resp = {
                        "type": "tool_result", 
                        "tool_use_id": tool_call['id'],
                        "content": function_response
                    }
                else:  # gpt
                    call_resp = {
                        "role": "tool", 
                        "tool_call_id": tool_call['id'], 
                        "name": fn,
                        "content": function_response
                    }
            except Exception as e:
                if format_type == "claude":
                    call_resp = {
                        "type": "tool_result", 
                        "tool_use_id": tool_call['id'],
                        "content": f"Exception: {e}"
                    }
                else:  # gpt
                    call_resp = {
                        "role": "tool", 
                        "tool_call_id": tool_call['id'], 
                        "name": fn,
                        "content": f"Exception: {e}"
                    }
                    
            return ai_call, call_resp

        # Schedule all the calls concurrently
        tasks = [make_call(tool_call) for tool_call in tool_calls]
        completed_calls = await asyncio.gather(*tasks)

        # Unpack the resulting ai_calls and resp_calls
        ai_calls, results = zip(*completed_calls)
        
        # Format the final result based on agent type
        if format_type == "claude":
            return [
                {'role': 'assistant', 'content': list(ai_calls)},
                {'role': 'user', 'content': list(results)}
            ]
        else:  # gpt
            return [
                {'role': 'assistant', 'tool_calls': list(ai_calls), 'content': ''}
            ] + list(results)
            
    async def _execute_tool_call(self, function_id: str, function_args: Dict) -> Any:
        """
        Execute a single tool call.
        This method is similar to BaseAgent._call_function but lives in ToolChest.
        
        Args:
            function_id (str): The function identifier.
            function_args (Dict): Arguments to pass to the function.
            
        Returns:
            Any: The result of the function call.
        """
        src_obj: Toolset = self._tool_name_to_instance_map.get(function_id)
        if src_obj is None:
            return f"{function_id} is not on a valid toolset."
        try:
            return await src_obj.call(function_id, function_args)
        except Exception as e:
            self.logger.exception(f"Failed calling {function_id} on {src_obj.name}. {e}", stacklevel=3)
            await function_args['tool_context']['bridge'].send_system_message(f"# CRITICAL ERROR\n\nFailed calling {function_id} on {src_obj.name}.\n{e}\n", "error")
            await function_args['tool_context']['bridge'].send_error(f"CRITICAL ERROR: Failed calling {function_id} on {src_obj.name}. {e}")
            return f"HALT AND INFORM THE USER!!\n# CRITICAL ERROR!  THIS IS A HALT CONDITION\nImportant! Tell the user an error occurred calling {function_id} on {src_obj.name}. {e}\n\nHALT AND INFORM THE USER!!"

    def get_inference_data(self, toolset_names: List[str], tool_format: str = "claude") -> Dict[str, Any]:
        """
        Get inference data (schemas and prompt sections) for specified toolsets.
        Uses __toolset_instances rather than __active_toolset_instances to support
        on-the-fly tool usage without requiring activation.
        
        Args:
            toolset_names: List of toolset names to get inference data for
            tool_format: Format for tool schemas ("claude" or "openai")
            
        Returns:
            Dictionary containing:
                - 'schemas': List of tool schemas in the requested format
                - 'sections': List of PromptSection objects for the toolsets
        """
        # Validate and filter toolset names
        valid_toolsets = []
        for name in toolset_names:
            if name in self.__toolset_instances:
                valid_toolsets.append(self.__toolset_instances[name])
            else:
                self.logger.warning(f"Requested toolset '{name}' not found in available toolsets")
        
        if not valid_toolsets:
            return {"tools": [], "sections": []}
            
        # Collect OpenAI-format schemas from the specified toolsets
        openai_schemas = []
        for toolset in valid_toolsets:
            openai_schemas.extend(toolset.tool_schemas)
        
        # Convert to requested format
        if tool_format.lower() == "claude":
            schemas = []
            for schema in openai_schemas:
                if "function" in schema:
                    new_schema = copy.deepcopy(schema['function'])
                    new_schema['input_schema'] = new_schema.pop('parameters')
                    schemas.append(new_schema)
                else:
                    schemas.append(schema)
        else:  # Default to OpenAI format
            schemas = openai_schemas
        
        # Collect prompt sections
        sections = [toolset.section for toolset in valid_toolsets if toolset.section is not None]
        
        return {
            "schemas": schemas,
            "sections": sections
        }