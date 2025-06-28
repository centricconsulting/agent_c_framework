import copy
import json
import asyncio
import logging

from typing import Type, List, Union, Dict, Any, Tuple, Optional

from agent_c.toolsets.tool_set import Toolset
from agent_c.util.logging_utils import LoggingManager
from agent_c.prompting.prompt_section import OldPromptSection
from agent_c.models.context.interaction_context import InteractionContext

class ToolChest:
    """
    A class representing a collection of toolsets that can be used by an agent.
    
    Attributes:
        __toolset_instances (dict[str, Toolset]): A private dictionary to store all instantiated toolsets.
        __available_toolset_classes (list[Type[Toolset]]): A private list to store all available toolset classes.
        __toolsets_awaiting_init (dict[str, Toolset]): A private dictionary to store toolsets created but awaiting post_init.
        __tool_opts (dict): A private dictionary to store the kwargs from the last init_tools call.
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

    def __init__(self, tool_cache: Optional['ToolCache'] = None):
        """
        Initializes the ToolChest with toolset instances, toolset classes, and a logger.
        """
        self.logger = LoggingManager(self.__class__.__name__).get_logger()

        if tool_cache is None:
            self.logger.warning("No tool_cache provided, using default ToolCache which stores in agent_c_config/tool_cache.")
            from agent_c.toolsets.tool_cache import ToolCache
            self.tool_cache = ToolCache()
        else:
            self.tool_cache = tool_cache

        # This list is all POSSIBLE toolsets that could be used, provided
        # you have any required keys etc.
        self.__available_toolset_classes = Toolset.tool_registry

        # This list contains instances of toolsets that have been instantiated.
        # The key is the toolset class name.
        self.__toolset_instances: dict[str, Toolset] = {}
        self._activation_stack: List[str] = []  # Stack to track activation order and circular dependencies

        # This maps actual TOOL methods names to the toolset instance that contains them.
        self._tool_name_to_instance_map: Dict[str, Toolset] = {}

        # Initialize tracking for lazy initialization
        self.__toolsets_awaiting_init = {}
        self.__tool_opts = {'tool_chest': self, 'tool_cache': self.tool_cache}

    @property
    def available_toolset_classes(self) -> List:
        return self.__available_toolset_classes

    def _update_toolset_metadata(self):
        self._tool_name_to_instance_map = {}
        for toolset in self.__toolset_instances.values():
            for schema in toolset.tool_schemas(toolset.prefix):
                self._tool_name_to_instance_map[schema['function']['name']] = toolset

    async def initialize_toolsets(self, toolset_name_or_names: Union[str, List[str]], tool_opts: Optional[Dict[str, any]] = None) -> bool:
        # Convert to list if a single string is provided
        toolset_names = [toolset_name_or_names] if isinstance(toolset_name_or_names, str) else toolset_name_or_names
        self.__tool_opts.update(tool_opts or {})

        # Track which toolsets are initialized during this activation call
        # This is used to ensure post_init is called in the correct order
        newly_instantiated = []
        
        success = True
        for name in toolset_names:
            # Skip if already active
            if name in self.__toolset_instances:
                continue
            
            # Check for circular dependencies
            if name in self._activation_stack:
                self.logger.warning(f"Circular dependency detected when activating {name}")
                success = False
                continue
            
            self._activation_stack.append(name)

            if name in self.__toolset_instances:
                success = True
            else:
                # Find the class for this toolset
                toolset_class = next((cls for cls in self.__available_toolset_classes 
                                      if cls.__name__ == name), None)
                
                if not toolset_class:
                    self._activation_stack.remove(name)
                    self.logger.warning(f"Toolset class {name} not found in available toolsets")
                    success = False
                    continue
                
                required_tools = Toolset.get_required_tools(name)
                
                # Log dependencies for debugging
                if required_tools:
                    self.logger.info(f"Toolset {name} requires: {', '.join(required_tools)}")
                    
                    # Recursively activate required tools
                    required_success = await self.initialize_toolsets(required_tools, tool_opts)
                    if not required_success:
                        self.logger.warning(f"Failed to activate required tools for {name}")
                        success = False
                        self._activation_stack.remove(name)
                        continue
                    
                    # Verify all required tools are actually active now
                    missing_tools = [tool for tool in required_tools if tool not in self.__toolset_instances]
                    if missing_tools:
                        self.logger.warning(f"Required tools {missing_tools} for {name} not active despite activation attempt")
                        success = False
                        self._activation_stack.remove(name)
                        continue

                try:
                    # Create the toolset instance
                    toolset_obj = toolset_class(**self.__tool_opts)
                    self.__toolset_instances[name] = toolset_obj
                    newly_instantiated.append(name)
                    
                    self.logger.info(f"Created toolset instance {name}")
                except Exception as e:
                    self.logger.exception(f"Error creating toolset {name}: {str(e)}", exc_info=True)
                    success = False
            
            self._activation_stack.remove(name)
        
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
                    self.logger.info(f"Completed post_init for toolset {name}")
            except Exception as e:
                self.logger.exception(f"Error in post_init for toolset {name}: {str(e)}", exc_info=True)
                success = False

        
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
        return await self.initialize_toolsets(tool_name, tool_opts)

    @property
    def active_tools(self) -> dict[str, Toolset]:
        """
        Property that returns the currently active toolset instances.
        
        Returns:
            dict[str, Toolset]: Dictionary of active tool instances.
        """
        return self.__toolset_instances

    @property
    def available_tools(self) -> dict[str, Toolset]:
        """
        Property that returns all instantiated toolset instances.
        
        Returns:
            dict[str, Toolset]: Dictionary of all instantiated tool instances.
        """
        return self.__toolset_instances


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
        
        if activate:
            self.__active_toolset_instances[name] = instance
            self._update_toolset_metadata()

    async def init_tools(self, tool_opts: Dict[str, any], initial_toolsets: Optional[List[str]] = None):
        """
        Initialize toolsets

        Args:
            tool_opts: Arguments to pass to toolset initialization.
            inital_toolsets: Optional list of toolset names to initialize.
        """
        # Create a copy of tool_opts to avoid modifying the original
        if initial_toolsets is None:
            initial_toolsets = ['WorkspaceTools', 'ThinkTools']

        self.__tool_opts.update(tool_opts)

        await self.initialize_toolsets(initial_toolsets)

    async def call_tools(self, tool_calls: List[dict], context: InteractionContext, format_type: str = "claude") -> List[dict]:
        """
        Execute multiple tool calls concurrently and return the results.
        
        Args:
            tool_calls (List[dict]): List of tool calls to execute.
            context (InteractionContext): The interaction context for the tool calls.
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
                args = json.loads(tool_call['arguments'])

                ai_call = {
                    "id": tool_call['id'],
                    "function": {"name": fn, "arguments": tool_call['arguments']},
                    'type': 'function'
                }

                
            try:

                full_args = copy.deepcopy(args)
                full_args['context'] = context
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
        This method is similar to AgentRuntime._call_function but lives in ToolChest.
        
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
            return f"Important! Tell the user an error occurred calling {function_id} on {src_obj.name}. {e}"

    def get_tool_sections(self, toolset_names: List[str]) -> List[OldPromptSection]:
        """
        Get prompt sections for specified toolsets.

        Args:
            toolset_names: List of toolset names to get sections for

        Returns:
            List of OldPromptSection objects for the specified toolsets
        """
        sections = []
        for name in toolset_names:
            if name in self.__toolset_instances:
                section = self.__toolset_instances[name].section
                if section is not None:
                    sections.append(section)
            else:
                self.logger.warning(f"Requested toolset '{name}' not found in available toolsets")

        return sections

    def _open_ai_tool_schemas(self, toolset_names: List[str]) -> List[Dict[str, Any]]:
        """
        Get OpenAI-format tool schemas for specified toolsets.

        Args:
            toolset_names: List of toolset names to get schemas for
        Returns:
            List of tool schemas in OpenAI format
        """
        schemas = []
        for name in toolset_names:
            if name in self.__toolset_instances:
                schemas.extend(self.__toolset_instances[name].tool_schemas(self.__toolset_instances[name].prefix))
            else:
                self.logger.warning(f"toolchest._open_ai_tool_schemas Requested toolset '{name}' not found in available toolsets")

        return schemas

    def get_tool_schemas(self, toolset_names: List[str], tool_format: str = "claude") ->  List[Dict[str, Any]]:
        """
        Get tool schemas for specified toolsets.

        Args:
            toolset_names: List of toolset names to get schemas for
            tool_format: Format for tool schemas ("claude" or "openai")

        Returns:
            List of tool schemas in OpenAI format
        """
        openai_schemas = self._open_ai_tool_schemas(toolset_names)

        if tool_format.lower() != "claude":
            return openai_schemas

        schemas = []
        for schema in openai_schemas:
            new_schema = copy.deepcopy(schema['function'])
            new_schema['input_schema'] = new_schema.pop('parameters')
            schemas.append(new_schema)

        return schemas

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
                - 'sections': List of OldPromptSection objects for the toolsets
        """

        return {
            "schemas": self.get_tool_schemas(toolset_names, tool_format),
            "sections": self.get_tool_sections(toolset_names)
        }