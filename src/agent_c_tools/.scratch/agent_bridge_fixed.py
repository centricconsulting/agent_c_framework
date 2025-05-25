import asyncio
import json
import os
import uuid

import threading
import traceback
from typing import Union, List, Dict, Any, AsyncGenerator, Optional
from datetime import datetime, timezone

# Redis imports
from agent_c_api.config.env_config import settings
from agent_c_api.core.util.redis_stream import RedisStreamManager

from agent_c import BaseAgent
from agent_c_api.config.env_config import settings

from agent_c.models.input.image_input import ImageInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.input.file_input import FileInput
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c.agents import GPTChatAgent
from agent_c.agents.claude import ClaudeChatAgent
from agent_c.models.events import SessionEvent
from agent_c.models.input import AudioInput
from agent_c_api.core.file_handler import FileHandler
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_tools.tools.workspace import LocalStorageWorkspace
from agent_c_tools.tools.random_number import RandomNumberTools
from agent_c.toolsets import ToolChest, ToolCache, Toolset
from agent_c.toolsets.mcp_tool_chest import MCPToolChest, MCPServer
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c.chat import ChatSessionManager

#from agent_c_tools.tools.user_preferences import AssistantPersonalityPreference, AddressMeAsPreference, UserPreference
from agent_c.prompting import PromptBuilder, CoreInstructionSection
from agent_c_tools.tools.workspace.local_storage import LocalProjectWorkspace
from agent_c_tools.tools import *


class AgentBridge:
    """
    A bridge interface between the agent_c library and ReactJS applications for chat functionality.

    This class provides a comprehensive interface for managing AI chat interactions,
    including session management, tool integration, and streaming responses. It supports
    multiple AI backends (OpenAI, Claude) and provides dynamic tool loading capabilities.

    Features:
        - Asynchronous chat streaming
        - Dynamic tool management
        - Multiple AI backend support
        - Custom persona management
        - Comprehensive event handling
        - Workspace management
        - File handling capabilities
    """

    def __init__(self, user_id: str = 'default', session_manager: Union[ChatSessionManager, None] = None,
                 backend: str = 'openai', model_name: str = 'gpt-4o', persona_name: str = 'default',
                 custom_prompt: str = None,
                 essential_tools: List[str] = None,
                 additional_tools: List[str] = None,
                 file_handler: Optional[FileHandler] = None,
                 **kwargs):
        """
        Initialize the SimplifiedAgent instance.

        Args:
            user_id (str): Identifier for the user. Defaults to 'default'.
        session_manager (Union[ChatSessionManager, None]): Session manager for chat sessions. Defaults to None.
        backend (str): Backend to use for the agent (e.g., 'openai', 'claude'). Defaults to 'openai'.
        model_name (str): Name of the AI model to use. Defaults to 'gpt-4o'.
        persona_name (str): Name of the persona to use. Defaults to 'default'.
        custom_prompt (str): Custom text to use for the agent's persona. Defaults to None.
        essential_toolsets (List[str]): List of essential tools the agent must have. Defaults to None.
        additional_toolsets (List[str]): List of additional tools to add to the agent. Defaults to None.
        **kwargs: Additional optional keyword arguments including:
            temperature (float): Temperature parameter for non-reasoning models
            reasoning_effort (float): Reasoning effort parameter for OpenAI models
            extended_thinking (bool): Extended thinking parameter for Claude models
            budget_tokens (int): Budget tokens parameter for Claude models
            agent_name (str): Name for the agent (for debugging)
            output_format (str): Output format for agent responses
            tool_cache_dir (str): Directory for tool cache
            file_handler (Optional[FileHandler]): Handler for file operations.
        """


        # Agent events setup, must come first
        self.__init_events()

        # Debugging and Logging Setup
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        self.debug_event = None

        self.agent_name = kwargs.get('agent_name', None)  # Debugging only

        # Initialize Core Class, there's quite a bit here, so let's go through this.
        # Agent Characteristics
        # - Backend: The backend used for the agent, defaults to 'openai', other open is 'claude'
        # - Model Name: The model name used for the agent, defaults to 'gpt-4o'
        self.backend = backend
        self.model_name = model_name
        self.agent: Optional[BaseAgent] = None
        self.agent_output_format = kwargs.get('output_format', 'raw')

        # Non-Reasoning Models Parameters
        self.temperature = kwargs.get('temperature')

        # Capture max_tokens if provided
        self.max_tokens = kwargs.get('max_tokens')

        # Open AI Reasoning model parameters
        self.reasoning_effort = kwargs.get('reasoning_effort')

        # Claude Reasoning model parameters
        self.extended_thinking = kwargs.get('extended_thinking')
        self.budget_tokens = kwargs.get('budget_tokens')

        # Agent "User" Management - these are used to keep agents and sessions separate in zep cache
        # - User ID: The user ID for the agent, this is used for user preference management, it is required.
        # In the future, I'll probably change this to a UUID
        # - User Preferences: A list of user preferences that the agent can use, this is a list of UserPreference objects.
        self.user_id = user_id
        # self.user_prefs: List[UserPreference] = [AddressMeAsPreference(), AssistantPersonalityPreference()]
        self.session_manager = session_manager

        # Agent persona management, pass in the persona name and we'll initialize the persona text now
        # we will pass in custom text later
        self.persona_name = persona_name
        self.custom_prompt = custom_prompt

        if self.persona_name is None or self.persona_name == '':
            self.persona_name = 'default'

        if self.custom_prompt is None:
            try:
                self.logger.info(f"Loading persona file for {self.persona_name} because custom_prompt is None")
                self.custom_prompt = self.__load_persona(self.persona_name)
            except Exception as e:
                self.logger.error(f"Error loading persona {self.persona_name}: {e}")
        else:
            self.logger.info(f"Using provided custom_prompt: {self.custom_prompt[:10]}...")

        # Chat Management, this is where the agent stores the chat history
        self.current_chat_Log: Union[List[Dict], None] = None

        # Tool Chest, Cache, and Setup
        # - Tool Chest: A collection of toolsets that the agent can use, Set to None initialization.
        # - Tool Cache: A cache for storing tool data, set to a default directory.
        # - Additional Tools: A list of additional toolsets to add to the tool chest, this is where use passes in other toolsets
        # - Selected Tools: A list of toolsets that the agent will use, this is a combination of essential toolsets and additional toolsets.
        # - Output Tool Arguments: A placeholder for tool argument output preference.
        self.tool_chest: Union[ToolChest, None] = None
        self.tool_cache_dir = kwargs.get("tool_cache_dir", ".tool_cache")
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)

        if essential_tools is None:
            self.essential_toolsets = ['WorkspaceTools', 'ThinkTools', 'RandomNumberTools', 'MarkdownToHtmlReportTools']
        else:
            self.essential_toolsets = essential_tools
        self.additional_toolsets = additional_tools or []
        self.selected_tools = self.essential_toolsets + self.additional_toolsets
        self.output_tool_arguments = True  # Placeholder for tool argument output preference

        # Agent Workspace Setup
        self.workspaces = None
        self.__init_workspaces()

        self.voice_tools = None

        # Initialize file handling capabilities
        self.file_handler = file_handler
        self.image_inputs: List[ImageInput] = []
        self.audio_inputs: List[AudioInput] = []

    def __init_events(self):
        """
        Initialize threading events used for debugging and input/output management.
        """
        self.exit_event = threading.Event()
        self.input_active_event = threading.Event()
        self.cancel_tts_event = threading.Event()
        # going to get from shared logging manager
        self.debug_event = LoggingManager.get_debug_event()
        
        # Redis stream ID for this agent instance
        self.redis_stream_id = None
        if settings.REDIS_STREAM_ENABLED:
            # Generate a unique stream ID for this agent instance
            self.redis_stream_id = f"agent_events:{str(uuid.uuid4())}"


    def __load_persona(self, persona_name: str = None) -> str:
        """
        Load the persona prompt from a file based on the given persona name.

        Returns:
            str: Loaded persona prompt text.

        Raises:
            Exception: If the persona file cannot be loaded.
        """
        if persona_name is None or persona_name == '' or persona_name.lower() == 'custom':
            return ''
        try:
            self.logger.info(f"Agent {self.agent_name} is loading persona: {self.persona_name}")
            persona_path = os.path.join('personas', f"{self.persona_name}.md")

            # If persona file exists, read it
            if os.path.exists(persona_path):
                with open(persona_path, 'r') as file:
                    return file.read()

            # If no persona file, return empty string
            self.logger.warning(f"No persona file found for {self.persona_name}. ")
            return ''

        except Exception as e:
            self.logger.error(f"Error loading persona {self.persona_name}: {e}")
            return ''

    def __init_workspaces(self):
        """
        Initialize the agent's workspaces by loading local workspace configurations.
        """
        local_project = LocalProjectWorkspace()
        self.workspaces = [local_project]
        self.logger.info(f"Agent {self.agent_name} initialized workspaces {local_project.workspace_root}")

        try:
            with open('.local_workspaces.json', 'r') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            pass

    async def __init_session(self):
        """
        Initialize the chat session for the agent, including setting up the session manager.
        """
        if self.session_manager is None:
            self.session_manager = ChatSessionManager() # This always gets a new session_manager object
            await self.session_manager.init(self.user_id) # This initializes the new session_manager object
        elif not hasattr(self.session_manager, 'chat_session') or self.session_manager.chat_session is None:
            # Only initialize if chat_session doesn't already exist on an already existing session_manager object.  Defensive programming here.
            # Because we're messing with how things are setup,
            # We may have a session_manager that is uninitialzied getting passed in, in that case we do need to initialize it.
            await self.session_manager.init(self.user_id)

        self.session_id = self.session_manager.chat_session.session_id
        self.logger.info(f"Agent {self.agent_name} completed session initialization: {self.session_id}")

    async def update_tools(self, new_tools: List[str]):
        """
        Updates the agent's tools without reinitializing the entire agent.

        This method allows dynamic updating of the tool set while maintaining
        the current session and other configurations. It ensures essential
        tools are preserved while adding or removing additional tools.

        Args:
            new_tools (List[str]): List of tool names to be added to the essential tools

        Notes:
            - Essential tools are always preserved
            - Duplicate tools are automatically removed
            - Tool chest is reinitialized with the updated tool set
            - Agent is reinitialized with new tools while maintaining the session
        """
        self.logger.info(f"Requesting new tool list for agent {self.agent_name} to: {new_tools}")

        # Remove duplicates and ensure essential tools are included
        all_tools = list(set(self.essential_toolsets + new_tools))

        self.additional_toolsets = [t for t in new_tools if t not in self.essential_toolsets]
        self.selected_tools = all_tools

        # Ensure you grab all the necessary options for the tool chest
        tool_opts = {
            'tool_cache': self.tool_cache,
            'session_manager': self.session_manager,
            'workspaces': self.workspaces,
            'streaming_callback': self.consolidated_streaming_callback
        }
        # Reinitialize just the tool chest
        await self.tool_chest.set_active_toolsets(self.additional_toolsets, tool_opts=tool_opts)
        self.agent.prompt_builder.tool_sections = self.tool_chest.active_tool_sections

        self.logger.info(f"Tools updated successfully. Current Active tools: {list(self.tool_chest.active_tools.keys())}")

    async def __init_tool_chest(self):
        """
        Initialize the agent's tool chest with selected tools and configurations.

        This method sets up the `ToolChest` with tools from the global `Toolset` registry
        based on the tools specified in `self.selected_tools`. It also configures additional
        tool options, initializes the selected tools, and logs the result. The method handles
        errors and logs any tools that failed to initialize, providing useful debugging information.

        Steps:
        1. Filter tools from the `Toolset` registry to match those in `self.selected_tools`.
        2. Initialize the `ToolChest` with the filtered tools.
        3. Create and set up a `ToolCache` for caching tool-related data.
        4. Pass the necessary configuration options to initialize the tools in the `ToolChest`.
        5. Log the successfully initialized tools.
        6. Debug and log any tools that were selected but failed to initialize.

        Raises:
            Exception: Logs and prints an error message if the tool initialization fails.

        Attributes:
            self.tool_chest (ToolChest): The tool chest instance containing the initialized tools.
            self.tool_cache (ToolCache): Cache manager for tool-related data.
            self.selected_tools (list): Names of tools selected for initialization.
            self.logger (Logger): Logger instance for logging messages.
            self.agent_name (str): The name of the agent using this tool chest.

        Debugging:
            Logs warnings if selected tools do not get initialized, typically due to
            misspelled tool class names in the `self.selected_tools` list.

        Returns:
            None
        """
        self.logger.info(f"Requesting initialization of these tools: {self.selected_tools}")

        # self.tool_cache = ToolCache(cache_dir=".tool_cache") # self.tool_cache is already initialized in __init__

        try:
            tool_opts = {
                'tool_cache': self.tool_cache,
                'session_manager': self.session_manager,
                'workspaces': self.workspaces,
                'streaming_callback': self.consolidated_streaming_callback
            }

            # Initialize the tool chest with essential tools first
            self.tool_chest = ToolChest(essential_toolsets=self.essential_toolsets, **tool_opts)

            # Initialize the tool chest essential tools
            await self.tool_chest.init_tools(tool_opts)
            self.logger.info(
                f"Agent {self.agent_name} successfully initialized essential tools: {list(self.tool_chest.active_tools.keys())}")

            if len(self.additional_toolsets):
                await self.tool_chest.set_active_toolsets(self.additional_toolsets, tool_opts=tool_opts)
                self.logger.info(
                    f"Agent {self.agent_name} successfully initialized additional tools: {list(self.tool_chest.active_tools.keys())}")



            # Usually it's a misspelling of the tool class name from the LLM
            initialized_tools = set(self.tool_chest.active_tools.keys())

            # Find tools that were selected but not initialized
            uninitialized_tools = [
                tool_name for tool_name in self.selected_tools
                if tool_name not in initialized_tools
            ]
            if uninitialized_tools:
                self.logger.warning(
                    f"The following selected tools were not initialized: {uninitialized_tools} for Agent {self.agent_name}")
        except Exception as e:
            print(f"Error initializing tools: {e}")
        return

    async def _sys_prompt_builder(self) -> PromptBuilder:
        operating_sections = [
            CoreInstructionSection(),
            ThinkSection(),
            DynamicPersonaSection()
        ]

        sections = operating_sections  # + info_sections
        prompt_builder = PromptBuilder(sections=sections, tool_sections=self.tool_chest.active_tool_sections)

        return prompt_builder

    async def initialize_agent_parameters(self):
        """
        Initialize the internal agent with prompt builders, tools, and configurations.

        This method creates and configures the conversational agent for the application.
        It sets up the agent's operational and informational sections, constructs a prompt
        builder with these sections, and initializes the agent based on the specified backend.

        Steps:
        1. Define the operational sections of the agent, including core instructions, dynamic
           persona settings, and active tool-related sections from the `ToolChest`.
        2. Define the informational sections, such as environmental context and user bio details.
        3. Combine operational and informational sections into a `PromptBuilder` instance.
        4. Initialize the agent using the `ClaudeChatAgent` or `GPTChatAgent` class,
           depending on the specified backend (`self.backend`).

        Attributes:
            self (ChatAgent): The initialized conversational agent instance
                (either `ClaudeChatAgent` or `GPTChatAgent`).
            self.backend (str): The backend type, determining which agent class is used.
            self.model_name (str): The name of the AI model used by the agent.
            self.tool_chest (ToolChest): The initialized tool chest containing active tools.
            self.__chat_callback (function): Callback function for handling streaming responses.
            self.agent_output_format (str): Format for the agent's output.

        Raises:
            Exception: If an error occurs during agent initialization, an exception may be raised.

        Returns:
            None
        """

        prompt_builder = await self._sys_prompt_builder()

        # Prepare common parameters that apply to both backends
        agent_params = {
            "prompt_builder": prompt_builder,
            "model_name": self.model_name,
            "tool_chest": self.tool_chest,
            "output_format": self.agent_output_format
        }
        
        # Set either streaming_callback or redis_stream_id, not both
        if settings.REDIS_STREAM_ENABLED and self.redis_stream_id:
            agent_params["redis_stream_id"] = self.redis_stream_id
            self.logger.info(f"Using Redis stream ID: {self.redis_stream_id} for event streaming")
        else:
            agent_params["streaming_callback"] = self.consolidated_streaming_callback

        # Add temperature if it exists (applies to both Claude and GPT)
        if self.temperature is not None:
            # self.logger.debug(f"Setting agent temperature to {self.temperature}")
            agent_params["temperature"] = self.temperature

        if self.max_tokens is not None:
            self.logger.debug(f"Setting agent max_tokens to {self.max_tokens}")
            agent_params["max_tokens"] = self.max_tokens

        if self.backend == 'claude':
            # Add Claude-specific parameters
            # Because claude.py only includes completion params for budget_tokens > 0
            # we can set it to 0 and it won't affect 3.5 or 3.7 models.
            budget_tokens = self.budget_tokens if self.budget_tokens is not None else 0
            agent_params["budget_tokens"] = budget_tokens
            self.logger.debug(f"Setting agent budget_tokens to {budget_tokens}")

            self.agent = ClaudeChatAgent(**agent_params)
        else:
            # Add OpenAI-specific parameters
            # Only pass reasoning_effort if it's set and we're using a reasoning model
            if self.reasoning_effort is not None and any(
                    reasoning_model in self.model_name
                    for reasoning_model in ["o1", "o1-mini", "o3", "o3-mini"]
            ):
                agent_params["reasoning_effort"] = self.reasoning_effort

            self.agent = GPTChatAgent(**agent_params)

        self.logger.info(f"Agent initialized using the following parameters: {agent_params}")

    async def reset_streaming_state(self):
        """Reset streaming state to ensure clean session"""
        self.logger.info(f"Resetting streaming state for session {self.session_id}")
        self._stream_queue = asyncio.Queue()

    async def __build_prompt_metadata(self) -> Dict[str, Any]:
        """
        Build metadata for prompts including user and session information.

        Returns:
            Dict[str, Any]: Metadata for prompts.
            - session_id (str): Session ID for the chat session. Not the UI session ID!
            - current_user_username (str): Username of the current user.
            - current_user_name (str): Name of the current user.
            - session_summary (str): Summary of the current chat session.
            - persona_prompt (str): Prompt for the persona.
        """
        return {
            "session_id": self.session_id,
            "current_user_username": self.session_manager.user.user_id,
            "current_user_name": self.session_manager.user.first_name,
            "session_summary": self.session_manager.chat_session.active_memory.summary,
            "persona_prompt": self.custom_prompt,
            "voice_tools": self.voice_tools,  # Add voice tools to metadata
            "timestamp": datetime.now().isoformat(),
            "env_name": os.getenv('ENV_NAME', 'development'),
            "session_info": self.session_manager.chat_session.session_id if self.session_manager else None
        }

    @staticmethod
    def _current_timestamp() -> str:
        """
        Returns the current UTC timestamp as a string.

        Returns:
            str: Current timestamp in ISO format.
        """
        return datetime.now(timezone.utc).isoformat()

    def _get_agent_config(self) -> Dict[str, Any]:
        """
        Get the current configuration of the agent.

        Returns:
            Dict[str, Any]: Dictionary containing agent configuration details
        """
        config = {
            'user_id': self.user_id,
            'backend': self.backend,
            'model_name': self.model_name,
            'persona_name': self.persona_name,
            'initialized_tools': [],
            'agent_name': self.agent_name,
            'agent_session_id': self.session_id,
            'custom_prompt': self.custom_prompt,
            'output_format': self.agent_output_format,
            'created_time': self._current_timestamp(),
            'temperature': self.temperature,
            'reasoning_effort': self.reasoning_effort,
            'agent_parameters': {
                'temperature': getattr(self, 'temperature', None),
                'reasoning_effort': getattr(self, 'reasoning_effort', None),
                'extended_thinking': getattr(self, 'extended_thinking', None),
                'budget_tokens': getattr(self, 'budget_tokens', None),
                'max_tokens': getattr(self, 'max_tokens', None)
            }
        }

        if self.tool_chest:
            config['initialized_tools'] = [{
                'instance_name': instance_name,
                'class_name': tool_instance.__class__.__name__,
                'developer_tool_name': getattr(tool_instance, 'name', instance_name),
                # 'description': tool_instance.__class__.__doc__
            } for instance_name, tool_instance in self.tool_chest.active_tools.items()]

        # self.logger.debug(f"Agent {self.agent_name} reporting config: {config[:100]}")
        return config

    @staticmethod
    def convert_to_string(data):
        # Check if data is None
        if data is None:
            raise ValueError("Input data is None")

        # Check if it's already a string
        if isinstance(data, str):
            return data

        # Check if the data is empty (this works for lists, dicts, etc.)
        if not data:
            raise ValueError("Input data is empty")

        # Attempt to convert to a JSON formatted string
        try:
            return json.dumps(data)
        except (TypeError, ValueError) as e:
            raise ValueError("Error converting input to string: " + str(e))

    async def _handle_message(self, event):
        """
        Handle message events (such as errors) from the model

        This is particularly important for handling Anthropic API errors
        """
        # Check if this is an error message about prompt length
        payload = json.dumps({
            "type": "message",
            "data": event.content,
            "role": event.role,
            "format": event.format,
            "critical": True
        }) + "\n"
        return payload

    async def _handle_tool_select_delta(self, event):
        """Handle tool selection events from the agent"""
        # self.logger.debug(f"tool SELECT delta event. {event.model_dump()}")
        payload = json.dumps({
            "type": "tool_select_delta",
            "data": self.convert_to_string(event.tool_calls) if hasattr(event, 'tool_calls') else 'No data',
            "format": "markdown"
        }) + "\n"
        return payload

    async def _handle_tool_call_delta(self, event):
        """Handle tool selection events from the agent"""
        # self.logger.debug(f"tool CALL delta event. {event.model_dump()}")
        payload = json.dumps({
            "type": "tool_call_delta",
            "data": self.convert_to_string(event.content) if hasattr(event, 'content') else 'No data',
            "format": "markdown"
        }) + "\n"
        return payload

    async def _handle_text_delta(self, event):
        """Handle text delta events from the agent/tools"""
        vendor = 'anthropic' if self.backend == 'claude' else 'openai'
        if vendor is None:
            model_name = self.model_name.lower()
            if any(name in model_name for name in ['sonnet', 'haiku', 'opus', 'claude']):
                vendor = 'anthropic'  # These are Anthropic models, vendor should be 'anthropic'
            elif any(name in model_name for name in ['gpt', 'davinci', 'o1', 'o1-mini', 'o3', 'o3-mini']):
                vendor = 'openai'  # These are OpenAI models, vendor should be 'openai'
            else:
                vendor = 'unknown'
        payload = json.dumps({
            "type": "content",
            "data": event.content,
            "vendor": vendor,
            "format": event.format
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_tool_call(event):
        """
        Unified tool call handler that checks the vendor to determine how to send messages.
        The front end expects a schema: tool_calls has id, name, arguments.  tool_results has role, tool_call_id, name, and content.
        For OpenAI:
          - When event.active is True, send a "tool_calls" message.
          - When event.active is False, send a "tool_results" message.

        For Claude:
          - Since Claude may not send an active tool call message, always emit both a
            "tool_calls" and a "tool_results" message (with active set to False) when complete.
        """
        # Ensure tool_calls is always a list.
        if event.tool_calls is None:
            calls = []
        elif isinstance(event.tool_calls, list):
            calls = event.tool_calls
        else:
            calls = [event.tool_calls]

        # Ensure tool_results is always a list.
        if event.tool_results is None:
            results = []
        elif isinstance(event.tool_results, list):
            results = event.tool_results
        else:
            results = [event.tool_results]

        if event.vendor == "open_ai":
            # For OpenAI, follow the standard streaming behavior.
            if event.active:
                payload_obj = {
                    "type": "tool_calls",
                    "tool_calls": calls
                }
            else:
                payload_obj = {
                    "type": "tool_results",
                    "tool_results": results
                }
            return json.dumps(payload_obj) + "\n"
        elif event.vendor == "anthropic":
            # For Claude, send tool calls if we have calls, and tool results if we have results
            if event.tool_results:  # We have results
                tool_results = []
                for result in results:
                    payload_result = {
                        "role": "tool",
                        "tool_call_id": result.get("tool_use_id", ""),
                        "name": calls[0].get("name", "") if calls else "",
                        "content": result.get("content", "{}")
                    }
                    tool_results.append(payload_result)

                return json.dumps({
                    "type": "tool_results",
                    "tool_results": tool_results
                }) + "\n"
            else:  # This is just a tool call
                tool_calls = []
                for call in calls:
                    tool_call = {
                        "id": call.get("id", ""),
                        "name": call.get("name", ""),
                        "arguments": json.dumps(call.get("input", {}))
                    }
                    tool_calls.append(tool_call)

                return json.dumps({
                    "type": "tool_calls",
                    "tool_calls": tool_calls
                }) + "\n"
        else:
            raise ValueError(f"Unsupported vendor: {event.vendor}")

    @staticmethod
    async def _handle_render_media(event):
        """Handle media render events from tools"""
        media_content = ""
        if event.content:
            media_content = event.content
        elif event.url:
            if "image" in event.content_type:
                media_content = f"<br><img src='{event.url}' style='max-width: 60%; height: auto;'/>"
            else:
                media_content = f"<br><object type='{event.content_type}' data='{event.url}'></object>"
        payload = json.dumps({
            "type": "render_media",
            "content": media_content,
            "content_type": event.content_type,
            "role": event.role,
            "metadata": {
                "sent_by_class": getattr(event, 'sent_by_class', None),
                "sent_by_function": getattr(event, 'sent_by_function', None),
                "session_id": getattr(event, 'session_id', None),
                "name": getattr(event, 'name', None)
            }
        }) + "\n"
        return payload

    async def _handle_history(self, event):
        """Handle history events which update the chat log"""
        self.current_chat_Log = event.messages
        payload = json.dumps({
            "type": "history",
            "messages": event.messages,
            "vendor": self.backend,
            "model_name": self.model_name,
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_audio_delta(event):
        """Handle audio events if voice features are enabled"""
        # For tools/agents these typically don't need special handling
        return None

    @staticmethod
    async def _handle_completion(event):
        """Handle completion events from the agent"""
        payload = json.dumps({
            "type": "completion_status",
            "data": {
                "running": event.running,
                "stop_reason": event.stop_reason,
                "input_tokens": event.input_tokens,
                "output_tokens": event.output_tokens,
            }
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_interaction(event):
        """Handle interaction state events"""
        if event.started:
            payload = json.dumps({
                "type": "interaction_start",
                "id": event.id,
                "session_id": event.session_id
            }) + "\n"
        else:
            payload = json.dumps({
                "type": "interaction_end",
                "id": event.id,
                "session_id": event.session_id
            }) + "\n"
        return payload

    async def _handle_thought_delta(self, event):
        """Handle thinking process events"""
        vendor = 'anthropic' if self.backend == 'claude' else 'openai'
        if vendor is None:
            model_name = self.model_name.lower()
            if any(name in model_name for name in ['sonnet', 'haiku', 'opus', 'claude']):
                vendor = 'anthropic'  # These are Anthropic models, vendor should be 'anthropic'
            elif any(name in model_name for name in ['gpt', 'davinci', 'o1', 'o1-mini', 'o3', 'o3-mini']):
                vendor = 'openai'  # These are OpenAI models, vendor should be 'openai'
            else:
                vendor = 'unknown'
        payload = json.dumps({
            "type": "thought_delta",
            "data": event.content,
            "vendor": vendor,
            "model_name": self.model_name,
            "format": "thinking"
        }) + "\n"
        return payload

    async def initialize(self):
        """
        Asynchronously initialize the agent's session, tool chest, and internal agent configuration.
        """
        # Initialize Redis stream if enabled
        if settings.REDIS_STREAM_ENABLED and self.redis_stream_id:
            try:
                await RedisStreamManager.initialize(settings.REDIS_URL)
                self.logger.info(f"Initialized Redis stream with ID: {self.redis_stream_id}")
            except Exception as e:
                self.logger.error(f"Failed to initialize Redis stream: {e}")
                # If Redis initialization fails, disable it for this session
                self.redis_stream_id = None
        
        await self.__init_session()
        await self.__init_tool_chest()
        await self.initialize_agent_parameters()


    async def consolidated_streaming_callback(self, event: SessionEvent = None, redis_stream_id: str = None):
        """
        Processes and routes various types of session events through appropriate handlers.

        This method serves as the central event processing hub, handling various event
        types including text updates, tool calls, media rendering, and completion status.
        It formats the events into JSON payloads suitable for streaming to the client.

        There are two modes of operation:
        1. Direct callback mode: When events are passed directly to this method
        2. Redis stream mode: When a redis_stream_id is provided, it reads events from the Redis stream

        Args:
            event (SessionEvent, optional): The event to process in direct callback mode
            redis_stream_id (str, optional): ID of the Redis stream to consume events from

        Event Types Handled:
            - text_delta: Text content updates
            - tool_call: Tool invocation events
            - render_media: Media content rendering
            - history: Chat history updates
            - audio_delta: Audio content updates
            - completion: Task completion status
            - interaction: Interaction state changes
            - thought_delta: Thinking process updates

        Notes:
            Events are processed and formatted into JSON strings with appropriate
            type markers and payloads for client-side handling.
        """
        # A simple dispatch dictionary that maps event types to handler methods.
        handlers = {
            "text_delta": self._handle_text_delta,
            "tool_call": self._handle_tool_call,
            "render_media": self._handle_render_media,
            "history": self._handle_history,
            "audio_delta": self._handle_audio_delta,
            "completion": self._handle_completion,
            "interaction": self._handle_interaction,
            "thought_delta": self._handle_thought_delta,
            "tool_call_delta": self._handle_tool_call_delta,
            "tool_select_delta": self._handle_tool_select_delta,
            "message": self._handle_message,
        }
        
        # Redis stream mode
        if redis_stream_id and not event:
            # Import here to avoid circular imports
            from agent_c_api.core.util.redis_stream import RedisStreamManager
            
            try:
                # Initialize Redis connection if not already done
                if not getattr(RedisStreamManager, '_initialized', False):
                    await RedisStreamManager.initialize(settings.REDIS_URL)
                
                # Read events from the Redis stream and process them
                consumer_name = f"agent_{self.agent_name}_{self.session_id}"
                self.logger.info(f"Reading events from Redis stream: {redis_stream_id}")
                
                # Get a batch of events from the stream
                async for stream_event in RedisStreamManager.consume_events(
                    stream_id=redis_stream_id,
                    consumer_group="agent_bridge",
                    consumer_name=consumer_name
                ):
                    # Process each event through the normal handler workflow
                    handler = handlers.get(stream_event.type)
                    if handler:
                        try:
                            payload = await handler(stream_event)
                            if payload is not None and hasattr(self, "_stream_queue"):
                                await self._stream_queue.put(payload)
                                
                                # If this is the end-of-stream event, push final payload and termination marker
                                if stream_event.type == "interaction" and not stream_event.started:
                                    await self._stream_queue.put(payload)
                                    await self._stream_queue.put(None)
                                    break  # End the loop when interaction ends
                        except Exception as e:
                            self.logger.error(f"Error handling Redis stream event of type {stream_event.type}: {str(e)}")
                    else:
                        self.logger.warning(f"Unhandled event type from Redis stream: {stream_event.type}")
            except Exception as e:
                self.logger.error(f"Error consuming from Redis stream {redis_stream_id}: {str(e)}")
                return
        
        # Direct callback mode - process a single event
        elif event:
            handler = handlers.get(event.type)
            if handler:
                try:
                    payload = await handler(event)
                    if payload is not None and hasattr(self, "_stream_queue"):
                        await self._stream_queue.put(payload)

                        # If this is the end-of-stream event, push final payload and termination marker
                        if event.type == "interaction" and not event.started:
                            await self._stream_queue.put(payload)
                            await self._stream_queue.put(None)
                except Exception as e:
                    self.logger.error(f"Error in event handler {handler.__name__} for {event.type}: {str(e)}")
            else:
                self.logger.warning(f"Unhandled event type: {event.type}")
        else:
            self.logger.error("consolidated_streaming_callback called with neither event nor redis_stream_id")

    async def stream_chat(self, user_message: str, custom_prompt: str = None, file_ids: List[str] = None, client_wants_cancel: Optional[threading.Event] = None):
        """
        Streams chat responses for a given user message.

        This method handles the complete chat interaction process, including:
        - Session updates
        - Custom prompt integration
        - Message processing
        - Response streaming
        - Event handling

        Args:
            user_message (str): The message from the user to process
            custom_prompt (str, optional): Custom prompt to override default persona. Defaults to None.
            file_ids (List[str], optional): IDs of files to include with the message
            client_wants_cancel (threading.Event, optional): Event to signal cancellation of the chat. Defaults to None.

        Yields:
            str: JSON-formatted strings containing various response types:
                - Content updates ("type": "content")
                - Tool calls ("type": "tool_calls")
                - Tool results ("type": "tool_results")
                - Media renders ("type": "render_media")
                - Completion status ("type": "completion_status")
                - Errors ("type": "error")

        Raises:
            Exception: Any errors during chat processing
        """
        await self.reset_streaming_state()

        queue = asyncio.Queue()
        self._stream_queue = queue  # Make the queue available to the callback

        try:
            await self.session_manager.update()

            if custom_prompt is not None:
                self.custom_prompt = custom_prompt

            file_inputs = []
            if file_ids and self.file_handler:
                file_inputs = await self.process_files_for_message(file_ids, self.user_id)

                # Log information about processed files
                if file_inputs:
                    input_types = {type(input_obj).__name__: 0 for input_obj in file_inputs}
                    for input_obj in file_inputs:
                        input_types[type(input_obj).__name__] += 1
                    self.logger.info(f"Processing {len(file_inputs)} files: {input_types}")

            # Setup streaming: either with Redis or direct callback
            original_callback = None
            if settings.REDIS_STREAM_ENABLED and self.redis_stream_id:
                # For Redis streaming, no need to set the agent's callback
                # The consolidated_streaming_callback will read directly from the Redis stream
                self.logger.info(f"Using Redis stream for event handling: {self.redis_stream_id}")
                
                # Start a task to read from the Redis stream and process events
                asyncio.create_task(self.consolidated_streaming_callback(redis_stream_id=self.redis_stream_id))
            else:
                # For direct callback, set the agent's streaming callback to our consolidated version
                original_callback = self.agent.streaming_callback
                self.agent.streaming_callback = self.consolidated_streaming_callback

            prompt_metadata = await self.__build_prompt_metadata()

            # Prepare chat parameters
            chat_params = {
                "streaming_queue": queue,
                "session_manager": self.session_manager,
                "user_message": user_message,
                "prompt_metadata": prompt_metadata,
                "output_format": 'raw',
                "client_wants_cancel": client_wants_cancel,
            }

            # Categorize file inputs by type to pass to appropriate parameters
            image_inputs = [input_obj for input_obj in file_inputs
                            if isinstance(input_obj, ImageInput)]
            audio_inputs = [input_obj for input_obj in file_inputs
                            if isinstance(input_obj, AudioInput)]
            document_inputs = [input_obj for input_obj in file_inputs
                               if isinstance(input_obj, FileInput) and
                               not isinstance(input_obj, ImageInput) and
                               not isinstance(input_obj, AudioInput)]

            # Only add parameters if there are inputs of that type
            if image_inputs:
                chat_params["images"] = image_inputs
            if audio_inputs:
                chat_params["audio_clips"] = audio_inputs
            if document_inputs:
                chat_params["files"] = document_inputs

            # Start the chat task
            chat_task = asyncio.create_task(
                self.agent.chat(**chat_params)
            )

            while True:
                try:
                    try:
                        timeout = getattr(settings, "CALLBACK_TIMEOUT")  # Get timeout from settings with fallback
                        content = await asyncio.wait_for(queue.get(), timeout=timeout)
                        if content is None:
                            self.logger.info("Received stream termination signal")
                            break
                        # self.logger.info(f"Yielding chunk: {content.replace("\n","")}")
                        yield content
                        queue.task_done()
                    except asyncio.TimeoutError:
                        timeout_msg =f"Timeout waiting for stream content to occur in agent_bridge.py:stream_chat. Waiting for stream surpassed {timeout} seconds, terminating stream."
                        self.logger.warning(timeout_msg)
                        yield json.dumps({
                            "type": "error",
                            "data": timeout_msg
                        }) + "\n"
                        break
                    except asyncio.CancelledError as e:
                        error_type = type(e).__name__
                        error_traceback = traceback.format_exc()
                        cancelled_msg = f"Asyncio Cancelled error in agent_bridge.py:stream_chat {error_type}: {str(e)}\n{error_traceback}"
                        self.logger.error(cancelled_msg)
                        yield json.dumps({
                            "type": "error",
                            "data": cancelled_msg
                        }) + "\n"
                        break
                except Exception as e:
                    error_type = type(e).__name__
                    error_traceback = traceback.format_exc()
                    unexpected_msg = f"Unexpected error in stream processing: {error_type}: {str(e)}\n{error_traceback}"
                    self.logger.error(unexpected_msg)
                    # Yield error message and break
                    yield json.dumps({"type": "error", "data": unexpected_msg}) + "\n"
                    break

            await chat_task
            
            # Clean up based on streaming method used
            if not settings.REDIS_STREAM_ENABLED and original_callback is not None:
                # Restore the original callback if using direct callback
                self.agent.streaming_callback = original_callback
                
            await self.session_manager.flush()

        except Exception as e:
            self.logger.exception(f"Error in stream_chat: {e}", exc_info=True)
            error_type = type(e).__name__
            error_traceback = traceback.format_exc()
            self.logger.error(f"Error in event_bridge.py:stream_chat {error_type}: {str(e)}\n{error_traceback}")
            yield json.dumps({
                "type": "error",
                "data": f"Error in event_bridge.py:stream_chat {error_type}: {str(e)}\n{error_traceback}"
            }) + "\n"
        finally:
            # Drain any remaining items in the queue.
            while not queue.empty():
                try:
                    await queue.get()
                    queue.task_done()
                except asyncio.QueueEmpty:
                    break

    async def process_files_for_message(self, file_ids: List[str], session_id: str) -> List[
        Union[FileInput, ImageInput, AudioInput]]:
        """
        Process files and convert them to appropriate Input objects for the agent.

        This method processes uploaded files and converts them to the appropriate input
        objects (FileInput, ImageInput, AudioInput) for handling by the agent's multimodal
        capabilities.

        Args:
            file_ids: List of file IDs to process
            session_id: Session ID

        Returns:
            List[Union[FileInput, ImageInput, AudioInput]]: List of input objects for the agent
        """
        if not self.file_handler or not file_ids:
            return []

        input_objects = []

        for file_id in file_ids:
            # Get file metadata
            metadata = self.file_handler.get_file_metadata(file_id, session_id)
            if not metadata:
                metadata = await self.file_handler.process_file(file_id, session_id)

            if not metadata:
                self.logger.warning(f"Could not get metadata for file {file_id}")
                continue

            # Create the appropriate input object based on file type
            input_obj = self.file_handler.get_file_as_input(file_id, session_id)
            if input_obj:
                self.logger.info(f"Created {type(input_obj).__name__} for file {metadata.original_filename}")
                input_objects.append(input_obj)
            else:
                self.logger.warning(f"Failed to create input object for file {metadata.original_filename}")

        return input_objects