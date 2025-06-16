import os
import json
import queue
import time
import asyncio
import threading
import traceback
import uuid
from typing import Any, Dict, List, Union, Optional, AsyncGenerator
from datetime import datetime, timezone

from agent_c.chat import ChatSessionManager
from agent_c_api.core.redis_session_manager import RedisSessionManager
from agent_c.models.input import AudioInput
from agent_c.agents.gpt import GPTChatAgent
from agent_c.models.events import SessionEvent
from agent_c.toolsets import ToolChest, ToolCache
from agent_c_api.config.env_config import settings
from agent_c.models.input.file_input import FileInput
from agent_c.agents.base import BaseAgent, ChatSession
from agent_c_api.core.file_handler import FileHandler
from agent_c.models.input.image_input import ImageInput
from agent_c_tools.tools.think.prompt import ThinkSection
from agent_c_api.config.config_loader import MODELS_CONFIG
from agent_c_tools.tools.workspace import LocalStorageWorkspace
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.core.util.redis_stream import RedisStreamManager
from agent_c_api.core.util.resilient_mode import (
    EventHandlerModeManager, ResilientModeConfig, OperationMode, TransitionReason
)
from agent_c_api.core.util.redis_health_monitor import RedisHealthMonitor
from agent_c.prompting import PromptBuilder, CoreInstructionSection
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c.agents.claude import ClaudeChatAgent, ClaudeBedrockChatAgent
from agent_c.util.event_session_logger_factory import create_with_callback
from agent_c_tools.tools.workspace.local_storage import LocalProjectWorkspace

# Constants
DEFAULT_BACKEND = 'claude'
DEFAULT_MODEL_NAME = 'claude-sonnet-4-20250514'
DEFAULT_OUTPUT_FORMAT = 'raw'
DEFAULT_TOOL_CACHE_DIR = '.tool_cache'
DEFAULT_LOG_DIR = './logs/sessions'
LOCAL_WORKSPACES_FILE = '.local_workspaces.json'
DEFAULT_ENV_NAME = 'development'
OPENAI_REASONING_MODELS = ['o1', 'o1-mini', 'o3', 'o3-mini']



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

    def __init__(
        self,
        chat_session: ChatSession,
        session_manager: Union[ChatSessionManager, RedisSessionManager],
        backend: str = DEFAULT_BACKEND,
        model_name: str = DEFAULT_MODEL_NAME,
        file_handler: Optional[FileHandler] = None,
        **kwargs: Any
    ) -> None:
        """
        Initialize the AgentBridge instance.

        This initializes a bridge interface between the agent_c library and ReactJS
        applications for chat functionality, setting up all necessary components
        including logging, tool management, and agent configuration.

        Args:
            chat_session: The chat session to manage.
            session_manager: Session manager for chat sessions.
            backend: Backend to use for the agent ('openai', 'claude', 'bedrock').
                Defaults to 'claude'.
            model_name: Name of the AI model to use. Defaults to 'claude-sonnet-4-20250514'.
            file_handler: Handler for file operations. Defaults to None.
            **kwargs: Additional optional keyword arguments including:
                - temperature (float): Temperature parameter for non-reasoning models
                - reasoning_effort (float): Reasoning effort parameter for OpenAI models
                - extended_thinking (bool): Extended thinking parameter for Claude models
                - budget_tokens (int): Budget tokens parameter for Claude models
                - agent_name (str): Name for the agent (for debugging)
                - output_format (str): Output format for agent responses
                - tool_cache_dir (str): Directory for tool cache
                - max_tokens (int): Maximum tokens for model responses
                - sections (List): Sections for the prompt builder
        
        Raises:
            Exception: If there are errors during tool or agent initialization.
        """
        # Agent events setup, must come first
        self.__init_events()
        self.chat_session = chat_session
        self.sections = kwargs.get('sections', None)  # Sections for the prompt builder, if any

        # Debugging and Logging Setup
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        # Set up streaming_callback with logging
        self.streaming_callback_with_logging = create_with_callback(
            log_base_dir=os.getenv('AGENT_LOG_DIR', DEFAULT_LOG_DIR),
            callback=self.consolidated_streaming_callback,  # Forward to UI processing
            include_system_prompt=True
        )

        self.debug_event = None

        self.agent_name = kwargs.get('agent_name', self.chat_session.agent_config.name)  # Debugging only

        # Initialize Core Class, there's quite a bit here, so let's go through this.
        # Agent Characteristics
        # - Backend: The backend used for the agent, defaults to 'openai', other open is 'claude'
        # - Model Name: The model name used for the agent, defaults to 'gpt-4o'
        self.backend = backend
        self.model_name = model_name
        self.agent_runtime: Optional[BaseAgent] = None
        self.agent_output_format = kwargs.get('output_format', DEFAULT_OUTPUT_FORMAT)

        # Non-Reasoning Models Parameters
        self.temperature = kwargs.get('temperature')

        # Capture max_tokens if provided
        self.max_tokens = kwargs.get('max_tokens')

        # Open AI Reasoning model parameters
        self.reasoning_effort = kwargs.get('reasoning_effort')

        # Claude Reasoning model parameters
        self.extended_thinking = kwargs.get('extended_thinking')
        self.budget_tokens = kwargs.get('budget_tokens')

        # self.user_prefs: List[UserPreference] = [AddressMeAsPreference(), AssistantPersonalityPreference()]
        self.session_manager = session_manager
        self.logger.info(f"Using provided agent config : {self.chat_session.agent_config.key}...")

        # Tool Chest, Cache, and Setup
        # - Tool Chest: A collection of toolsets that the agent can use, Set to None initialization.
        # - Tool Cache: A cache for storing tool data, set to a default directory.
        # - Output Tool Arguments: A placeholder for tool argument output preference.
        self.tool_chest: Union[ToolChest, None] = None
        self.tool_cache_dir = kwargs.get("tool_cache_dir", DEFAULT_TOOL_CACHE_DIR)
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)
        self.output_tool_arguments = True  # Placeholder for tool argument output preference

        # Agent Workspace Setup
        self.workspaces = None
        self.__init_workspaces()

        self.voice_tools = None

        # Initialize file handling capabilities
        self.file_handler = file_handler
        self.image_inputs: List[ImageInput] = []
        self.audio_inputs: List[AudioInput] = []

        # Redis Streams integration
        self.redis_stream_manager = None
        self._current_redis_stream_id = None
        self._redis_streams_enabled = False
        
        # Resilient mode components
        self.mode_manager: Optional[EventHandlerModeManager] = None
        self.health_monitor: Optional[RedisHealthMonitor] = None
        self.resilient_config: Optional[ResilientModeConfig] = None
        
        # Session flushing optimization attributes
        self._flush_batch_queue: Optional[asyncio.Queue] = None
        self._flush_batch_task: Optional[asyncio.Task] = None
        self._flush_batch_size: int = getattr(settings, 'HISTORY_BATCH_SIZE', 5)
        self._flush_batch_timeout: float = getattr(settings, 'HISTORY_BATCH_TIMEOUT', 2.0)

    def __init_events(self) -> None:
        """
        Initialize threading events used for debugging and input/output management.
        
        Sets up various threading events for coordinating agent operations,
        including exit handling, input state management, and TTS cancellation.
        """
        self.exit_event = threading.Event()
        self.input_active_event = threading.Event()
        self.cancel_tts_event = threading.Event()
        # Get debug event from shared logging manager
        self.debug_event = LoggingManager.get_debug_event()


    def __init_workspaces(self) -> None:
        """
        Initialize the agent's workspaces by loading local workspace configurations.
        
        Sets up the default local project workspace and loads additional workspaces
        from the local configuration file if it exists. This provides the agent
        with access to file system locations for tool operations.
        
        Raises:
            Exception: If there are errors loading workspace configurations
                (FileNotFoundError is handled gracefully).
        """
        local_project = LocalProjectWorkspace()
        self.workspaces = [local_project]
        self.logger.info(
            f"Agent {self.chat_session.agent_config.key} initialized workspaces "
            f"{local_project.workspace_root}"
        )
        # TODO: ALLOWED / DISALLOWED WORKSPACES from agent config
        try:
            with open(LOCAL_WORKSPACES_FILE, 'r', encoding='utf-8') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            # Local workspaces file is optional
            pass

    @property
    def current_chat_log(self) -> List[Dict[str, Any]]:
        """
        Returns the current chat log for the agent.

        Returns:
            Union[List[Dict], None]: The current chat log or None if not set.
        """
        return self.chat_session.messages

    async def update_tools(self, new_tools: List[str]) -> None:
        """
        Update the agent's tools without reinitializing the entire agent.

        This method allows dynamic updating of the tool set while maintaining
        the current session and other configurations. It ensures essential
        tools are preserved while adding or removing additional tools.

        Args:
            new_tools: List of tool names to be added to the essential tools.

        Raises:
            Exception: If there are errors during tool activation.
            
        Notes:
            - Essential tools are always preserved
            - Duplicate tools are automatically removed
            - Tool chest is reinitialized with the updated tool set
            - Agent is reinitialized with new tools while maintaining the session
        """
        self.logger.info(
            f"Requesting new tool list for agent {self.agent_name} to: {new_tools}"
        )
        self.chat_session.agent_config.tools = new_tools
        await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        self.logger.info(
            f"Tools updated successfully. Current Active tools: "
            f"{list(self.tool_chest.active_tools.keys())}"
        )

    async def __init_tool_chest(self) -> None:
        """
        Initialize the agent's tool chest with selected tools and configurations.

        This method sets up the ToolChest with tools from the global Toolset registry
        based on the tools specified in the agent configuration. It configures additional
        tool options, initializes the selected tools, and logs the result. The method handles
        errors and logs any tools that failed to initialize.

        Process:
            1. Set up tool options including cache, session manager, and workspaces
            2. Initialize the ToolChest with configuration options
            3. Initialize and activate the specified toolset
            4. Log successful initialization and any failures

        Raises:
            Exception: If there are errors during tool initialization, logged with full traceback.

        Note:
            Logs warnings if selected tools do not get initialized, typically due to
            misspelled tool class names in the agent configuration.
        """
        self.logger.info(
            f"Requesting initialization of these tools: "
            f"{self.chat_session.agent_config.tools} for agent "
            f"{self.chat_session.agent_config.key}"
        )

        try:
            tool_opts = {
                'tool_cache': self.tool_cache,
                'session_manager': self.session_manager,
                'workspaces': self.workspaces,
                'streaming_callback': self.streaming_callback_with_logging,
                'model_configs': MODELS_CONFIG
            }

            # Initialize the tool chest with essential tools first
            self.tool_chest = ToolChest(**tool_opts)

            # Initialize the tool chest essential tools
            await self.tool_chest.init_tools(tool_opts)
            await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)

            self.logger.info(
                f"Agent {self.chat_session.agent_config.key} successfully initialized "
                f"essential tools: {list(self.tool_chest.active_tools.keys())}"
            )

            # Check for tools that were selected but not initialized
            # Usually indicates misspelling of the tool class name
            initialized_tools = set(self.tool_chest.active_tools.keys())
            uninitialized_tools = [
                tool_name for tool_name in self.chat_session.agent_config.tools
                if tool_name not in initialized_tools
            ]

            if uninitialized_tools:
                self.logger.warning(
                    f"The following selected tools were not initialized: "
                    f"{uninitialized_tools} for Agent {self.chat_session.agent_config.name}"
                )

        except Exception as e:
            self.logger.exception("Error initializing tools: %s", e, exc_info=True)
            raise

    async def _sys_prompt_builder(self) -> PromptBuilder:
        """
        Build the system prompt for the agent.
        
        Creates a PromptBuilder with the core operational sections and
        active tool sections from the tool chest.
        
        Returns:
            PromptBuilder: Configured prompt builder with all necessary sections.
        """
        operating_sections = [
            CoreInstructionSection(),
            ThinkSection(),
            DynamicPersonaSection()
        ]

        prompt_builder = PromptBuilder(
            sections=operating_sections,
            tool_sections=self.tool_chest.active_tool_sections
        )

        return prompt_builder

    async def initialize_agent_parameters(self) -> None:
        """
        Initialize the internal agent with prompt builders, tools, and configurations.

        This method creates and configures the conversational agent for the application.
        It sets up the agent's prompt builder and initializes the appropriate agent
        class based on the specified backend (Claude, Bedrock, or OpenAI).

        Process:
            1. Build the system prompt using configured sections
            2. Prepare common agent parameters
            3. Add backend-specific parameters (temperature, reasoning settings, etc.)
            4. Initialize the appropriate agent class based on backend

        Raises:
            Exception: If an error occurs during agent initialization.
            
        Note:
            Sets self.agent_runtime to the initialized agent instance, which will be
            one of ClaudeChatAgent, ClaudeBedrockChatAgent, or GPTChatAgent.
        """

        prompt_builder = await self._sys_prompt_builder()

        # Prepare common parameters that apply to both backends
        agent_params = {
            "prompt_builder": prompt_builder,
            "model_name": self.model_name,
            "tool_chest": self.tool_chest,
            "streaming_callback": self.streaming_callback_with_logging,
            "output_format": self.agent_output_format
        }

        # Add temperature if it exists (applies to both Claude and GPT)
        if self.temperature is not None:
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

            self.agent_runtime = ClaudeChatAgent(**agent_params)

        elif self.backend == 'bedrock':
            # Add Claude Bedrock-specific parameters
            budget_tokens = self.budget_tokens if self.budget_tokens is not None else 0
            agent_params["budget_tokens"] = budget_tokens
            self.logger.debug(f"Setting agent budget_tokens to {budget_tokens}")

            self.agent_runtime = ClaudeBedrockChatAgent(**agent_params)

        else:
            # Add OpenAI-specific parameters
            # Only pass reasoning_effort if it's set and we're using a reasoning model
            if (self.reasoning_effort is not None and
                any(reasoning_model in self.model_name for reasoning_model in OPENAI_REASONING_MODELS)):
                agent_params["reasoning_effort"] = self.reasoning_effort

            self.agent_runtime = GPTChatAgent(**agent_params)

        self.logger.info(f"Agent initialized using the following parameters: {agent_params}")

    async def reset_streaming_state(self) -> None:
        """
        Reset streaming state to ensure clean session.
        
        Creates a new asyncio Queue for streaming responses and cleans up
        batch processing resources, ensuring that each chat interaction 
        starts with a clean state.
        """
        self.logger.info(
            f"Resetting streaming state for session {self.chat_session.session_id}"
        )
        self._stream_queue = asyncio.Queue()
        
        # Clean up batch processing resources
        await self._cleanup_batch_resources()
    
    async def _cleanup_batch_resources(self) -> None:
        """
        Clean up batch processing resources.
        
        Cancels running batch tasks and clears queues to prevent
        resource leaks and ensure clean state for new sessions.
        """
        try:
            # Cancel running batch task if exists
            if self._flush_batch_task and not self._flush_batch_task.done():
                self._flush_batch_task.cancel()
                try:
                    await self._flush_batch_task
                except asyncio.CancelledError:
                    pass  # Expected when cancelling
                except Exception as e:
                    self.logger.warning(f"Error during batch task cleanup: {e}")
            
            # Clear batch queue if exists
            if self._flush_batch_queue:
                # Drain any remaining items
                try:
                    while not self._flush_batch_queue.empty():
                        try:
                            self._flush_batch_queue.get_nowait()
                        except asyncio.QueueEmpty:
                            break
                except Exception as e:
                    self.logger.warning(f"Error draining batch queue: {e}")
            
            # Reset batch attributes
            self._flush_batch_queue = None
            self._flush_batch_task = None
            
        except Exception as e:
            self.logger.error(f"Error cleaning up batch resources: {e}")

    async def __build_prompt_metadata(self) -> Dict[str, Any]:
        """
        Build metadata for prompts including user and session information.

        Creates a comprehensive metadata dictionary that provides context
        for prompt generation, including session details, user information,
        and system configuration.

        Returns:
            Dict[str, Any]: Metadata dictionary containing:
                - session_id: Session ID for the chat session (not UI session ID)
                - current_user_username: Username of the current user
                - persona_prompt: Prompt for the persona
                - agent_config: Complete agent configuration
                - voice_tools: Voice tools configuration
                - timestamp: Current timestamp in ISO format
                - env_name: Environment name (development, production, etc.)
        """
        return {
            "session_id": self.chat_session.session_id,
            "current_user_username": self.chat_session.user_id,
            "persona_prompt": self.chat_session.agent_config.persona,
            "agent_config": self.chat_session.agent_config,
            "voice_tools": self.voice_tools,
            "timestamp": datetime.now().isoformat(),
            "env_name": os.getenv('ENV_NAME', DEFAULT_ENV_NAME)
        }

    @staticmethod
    def _current_timestamp() -> str:
        """
        Returns the current UTC timestamp as a string.

        Returns:
            str: Current timestamp in ISO format.
        """
        return datetime.now(timezone.utc).isoformat()

    def get_agent_runtime_config(self) -> Dict[str, Any]:
        """
        Get the current configuration of the agent.

        Returns:
            Dict[str, Any]: Dictionary containing agent configuration details
        """
        config = {
            'backend': self.backend,
            'model_name': self.model_name,
            'initialized_tools': [],
            'agent_name': self.agent_name,
            'user_session_id': self.chat_session.session_id,
            'agent_session_id': self.chat_session.session_id,
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
    def convert_to_string(data: Any) -> str:
        """
        Convert input data to a string representation.
        
        Handles various data types by converting them to JSON strings when possible,
        or returning the string directly if already a string.
        
        Args:
            data: The data to convert to a string.
            
        Returns:
            str: String representation of the input data.
            
        Raises:
            ValueError: If data is None, empty, or cannot be converted to JSON.
        """
        if data is None:
            raise ValueError("Input data is None")

        if isinstance(data, str):
            return data

        # Check if the data is empty (works for lists, dicts, etc.)
        if not data:
            raise ValueError("Input data is empty")

        # Attempt to convert to a JSON formatted string
        try:
            return json.dumps(data)
        except (TypeError, ValueError) as e:
            raise ValueError(f"Error converting input to string: {str(e)}") from e

    @staticmethod
    async def _handle_message(event: SessionEvent) -> str:
        """
        Handle message events from the model.

        This is particularly important for handling Anthropic API errors
        and other critical messages that need immediate attention.
        
        Args:
            event: Session event containing message information.
            
        Returns:
            str: JSON-formatted message payload.
        """
        payload = json.dumps({
            "type": "message",
            "data": event.content,
            "role": event.role,
            "format": event.format,
            "critical": True
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_system_message(event: SessionEvent) -> str:
        """
        Handle system message events.
        
        Args:
            event: Session event containing system message information.
            
        Returns:
            str: JSON-formatted system message payload.
        """
        return json.dumps({
            "type": "message",
            "data": event.content,
            "role": event.role,
            "format": event.format,
            "severity": event.severity
        }) + "\n"

    @staticmethod
    async def _ignore_event(_: SessionEvent) -> None:
        """
        Ignore certain event types that don't require processing.
        
        Args:
            _: Session event to ignore.
            
        Returns:
            None: No payload is generated for ignored events.
        """
        return None

    async def _handle_tool_select_delta(self, event: SessionEvent) -> str:
        """
        Handle tool selection events from the agent.
        
        Args:
            event: Session event containing tool selection information.
            
        Returns:
            str: JSON-formatted tool select delta payload.
        """
        data = (
            self.convert_to_string(event.tool_calls)
            if hasattr(event, 'tool_calls')
            else 'No data'
        )
        payload = json.dumps({
            "type": "tool_select_delta",
            "data": data,
            "format": "markdown"
        }) + "\n"
        return payload

    async def _handle_tool_call_delta(self, event: SessionEvent) -> str:
        """
        Handle tool call delta events from the agent.
        
        Args:
            event: Session event containing tool call delta information.
            
        Returns:
            str: JSON-formatted tool call delta payload.
        """
        data = (
            self.convert_to_string(event.content)
            if hasattr(event, 'content')
            else 'No data'
        )
        payload = json.dumps({
            "type": "tool_call_delta",
            "data": data,
            "format": "markdown"
        }) + "\n"
        return payload

    async def _handle_text_delta(self, event: SessionEvent) -> str:
        """
        Handle text delta events from the agent/tools.
        
        Determines the appropriate vendor based on backend and model name,
        then formats the text content for streaming to the client.
        
        Args:
            event: Session event containing text delta information.
            
        Returns:
            str: JSON-formatted content payload with vendor information.
        """
        vendor = self._determine_vendor()

        payload = json.dumps({
            "type": "content",
            "data": event.content,
            "vendor": vendor,
            "format": event.format
        }) + "\n"
        return payload

    def _determine_vendor(self) -> str:
        """
        Determine the vendor based on backend and model name.
        
        Returns:
            str: Vendor name ('anthropic', 'openai', 'bedrock', 'azure').
        """
        if self.backend in  ['claude', 'bedrock']:
            return 'claude'
        elif self.backend in ['openai', 'azure']:
            return 'openai'

        return 'claude'  # Default to 'claude' if backend is not recognized


    @staticmethod
    async def _handle_tool_call(event: SessionEvent) -> str:
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
    async def _handle_render_media(event: SessionEvent) -> str:
        """
        Handle media render events from tools.
        
        Processes media content from tools, generating appropriate HTML
        for display in the client interface.
        
        Args:
            event: Session event containing media render information.
            
        Returns:
            str: JSON-formatted media render payload.
        """
        media_content = ""

        if event.content:
            media_content = event.content
        elif event.url:
            if "image" in event.content_type:
                media_content = (
                    f"<br><img src='{event.url}' "
                    f"style='max-width: 60%; height: auto;'/>"
                )
            else:
                media_content = (
                    f"<br><object type='{event.content_type}' "
                    f"data='{event.url}'></object>"
                )

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

    async def _handle_history(self, event: SessionEvent) -> str:
        """
        Handle history events which update the chat log.
        
        Updates the chat session with new message history and optimally flushes
        the session to persistent storage using async operations and optional batching.
        
        Args:
            event: Session event containing message history.
            
        Returns:
            str: JSON-formatted history payload.
            
        Raises:
            Exception: If session flushing fails.
        """
        # Update session messages
        self.chat_session.messages = event.messages
        
        # Use optimized async flushing strategy
        await self._flush_session_optimized()
        
        payload = json.dumps({
            "type": "history",
            "messages": event.messages,
            "vendor": self.backend,
            "model_name": self.model_name,
        }) + "\n"
        return payload
    
    async def _flush_session_optimized(self) -> None:
        """
        Optimized session flushing with batching and async operations.
        
        This method provides several optimization strategies:
        1. Non-blocking async flushing
        2. Optional batched flushing to reduce I/O operations
        3. Error resilience with fallback to immediate flush
        
        The batching behavior is controlled by environment variables:
        - HISTORY_BATCH_FLUSH: Enable/disable batched flushing (default: False)
        - HISTORY_BATCH_SIZE: Number of events to batch (default: 5)
        - HISTORY_BATCH_TIMEOUT: Max time to wait for batch in seconds (default: 2.0)
        """
        # Check if batched flushing is enabled
        batch_flush_enabled = getattr(settings, 'HISTORY_BATCH_FLUSH', False)
        
        if batch_flush_enabled:
            await self._batch_flush_session()
        else:
            await self._immediate_flush_session()
    
    async def _immediate_flush_session(self) -> None:
        """
        Immediate async session flushing for real-time consistency.
        """
        try:
            # Use asyncio.create_task to make flush non-blocking if session manager supports it
            if hasattr(self.session_manager, 'flush') and asyncio.iscoroutinefunction(self.session_manager.flush):
                # Session manager already supports async operations
                await self.session_manager.flush(self.chat_session.session_id)
            else:
                # Fallback: Run sync flush in thread pool to avoid blocking
                loop = asyncio.get_event_loop()
                await loop.run_in_executor(
                    None, 
                    self.session_manager.flush, 
                    self.chat_session.session_id
                )
        except Exception as e:
            self.logger.error(f"Failed to flush session {self.chat_session.session_id}: {e}")
            # Don't re-raise to avoid breaking the event stream
    
    async def _batch_flush_session(self) -> None:
        """
        Batched session flushing to reduce I/O operations.
        
        Collects multiple flush requests and processes them together to reduce
        overhead from frequent database/storage operations.
        """
        # Initialize batch tracking if not already done
        if not hasattr(self, '_flush_batch_queue'):
            self._flush_batch_queue = asyncio.Queue()
            self._flush_batch_task = None
            self._flush_batch_size = getattr(settings, 'HISTORY_BATCH_SIZE', 5)
            self._flush_batch_timeout = getattr(settings, 'HISTORY_BATCH_TIMEOUT', 2.0)
        
        try:
            # Add flush request to batch queue
            await self._flush_batch_queue.put(self.chat_session.session_id)
            
            # Start batch processing task if not already running
            if self._flush_batch_task is None or self._flush_batch_task.done():
                self._flush_batch_task = asyncio.create_task(self._process_flush_batches())
                
        except Exception as e:
            self.logger.error(f"Failed to queue batch flush for session {self.chat_session.session_id}: {e}")
            # Fallback to immediate flush
            await self._immediate_flush_session()
    
    async def _process_flush_batches(self) -> None:
        """
        Background task to process batched flush requests.
        """
        while True:
            try:
                batch_sessions = set()
                
                # Collect session IDs for batching
                try:
                    # Wait for first session with timeout
                    first_session = await asyncio.wait_for(
                        self._flush_batch_queue.get(),
                        timeout=self._flush_batch_timeout
                    )
                    batch_sessions.add(first_session)
                    
                    # Collect additional sessions up to batch size
                    for _ in range(self._flush_batch_size - 1):
                        try:
                            session_id = await asyncio.wait_for(
                                self._flush_batch_queue.get(),
                                timeout=0.1  # Short timeout for additional items
                            )
                            batch_sessions.add(session_id)
                        except asyncio.TimeoutError:
                            break  # No more items available quickly
                            
                except asyncio.TimeoutError:
                    # No sessions to flush, exit the task
                    break
                
                # Process the batch
                if batch_sessions:
                    await self._flush_session_batch(batch_sessions)
                    
            except Exception as e:
                self.logger.error(f"Error in batch flush processing: {e}")
                # Continue processing despite errors
    
    async def _flush_session_batch(self, session_ids: set) -> None:
        """
        Flush multiple sessions efficiently.
        
        Args:
            session_ids: Set of session IDs to flush
        """
        try:
            # Check if session manager supports batch operations
            if hasattr(self.session_manager, 'flush_batch'):
                # Use native batch flush if available
                await self.session_manager.flush_batch(list(session_ids))
            else:
                # Fallback: Process sessions concurrently
                flush_tasks = []
                for session_id in session_ids:
                    if hasattr(self.session_manager, 'flush') and asyncio.iscoroutinefunction(self.session_manager.flush):
                        task = self.session_manager.flush(session_id)
                    else:
                        # Run sync flush in thread pool
                        loop = asyncio.get_event_loop()
                        task = loop.run_in_executor(None, self.session_manager.flush, session_id)
                    flush_tasks.append(task)
                
                # Execute all flushes concurrently
                if flush_tasks:
                    await asyncio.gather(*flush_tasks, return_exceptions=True)
            
            self.logger.debug(f"Successfully flushed batch of {len(session_ids)} sessions")
            
        except Exception as e:
            self.logger.error(f"Failed to flush session batch: {e}")
            # Don't re-raise to avoid breaking the batch processing

    @staticmethod
    async def _handle_audio_delta(_: SessionEvent) -> None:
        """
        Handle audio events if voice features are enabled.
        
        Currently, audio events don't require special handling for tools/agents.
        
        Args:
            _: Session event containing audio delta information (ignored).
            
        Returns:
            None: No payload is generated for audio deltas.
        """
        return None

    @staticmethod
    async def _handle_completion(event: SessionEvent) -> str:
        """
        Handle completion events from the agent.
        
        Processes completion status information including token usage
        and stop reasons for the chat interaction.
        
        Args:
            event: Session event containing completion information.
            
        Returns:
            str: JSON-formatted completion status payload.
        """
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
    async def _handle_interaction(event: SessionEvent) -> str:
        """
        Handle interaction state events.
        
        Processes interaction start and end events to track
        the lifecycle of chat interactions.
        
        Args:
            event: Session event containing interaction state information.
            
        Returns:
            str: JSON-formatted interaction payload.
        """
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

    async def _handle_thought_delta(self, event: SessionEvent) -> str:
        """
        Handle thinking process events.
        
        Processes thought delta events from reasoning models, formatting
        them with appropriate vendor and model information.
        
        Args:
            event: Session event containing thought delta information.
            
        Returns:
            str: JSON-formatted thought delta payload.
        """
        vendor = self._determine_vendor()

        payload = json.dumps({
            "type": "thought_delta",
            "data": event.content,
            "vendor": vendor,
            "model_name": self.model_name,
            "format": "thinking"
        }) + "\n"
        return payload

    async def initialize(self) -> None:
        """
        Asynchronously initialize the agent's session, tool chest, and internal agent configuration.
        
        This method performs the complete initialization sequence required
        to prepare the agent for chat interactions.
        
        Raises:
            Exception: If initialization of tools or agent parameters fails.
        """
        await self.__init_tool_chest()
        await self.initialize_agent_parameters()
        await self.initialize_redis_streams()


    async def consolidated_streaming_callback(self, event: SessionEvent, redis_stream_id: Optional[str] = None) -> None:
        """
        Process and route various types of session events through appropriate handlers.
        
        This method serves as the central event processing hub, handling various event
        types including text updates, tool calls, media rendering, and completion status.
        It formats the events into JSON payloads suitable for streaming to the client.
        
        Enhanced with Redis Streams support for distributed event processing with
        EventHandlerModeManager integration for dynamic mode switching. Maintains
        backward compatibility with async queue fallback.

        Args:
            event: The session event to process, containing type and payload information.
            redis_stream_id: Optional Redis Stream ID for distributed event publishing.
                           Format: "{session_id}:{interaction_id}"

        Event Types Handled:
            - text_delta: Text content updates
            - tool_call: Tool invocation events  
            - render_media: Media content rendering
            - history: Chat history updates
            - audio_delta: Audio content updates
            - completion: Task completion status
            - interaction: Interaction state changes
            - thought_delta: Thinking process updates
            - tool_call_delta: Tool call streaming updates
            - tool_select_delta: Tool selection streaming updates
            - message: Important messages from the model
            - system_message: System-level messages
            - history_delta: History streaming updates (ignored)
            - complete_thought: Complete thought events (ignored)

        Raises:
            Exception: Logs errors if event handlers fail, but does not re-raise.
        """
        # Debug logging (commented out for performance)
        # try:
        #     self.logger.debug(
        #         f"Consolidated callback received event: {event.model_dump_json(exclude={'content_bytes'})}")
        # except Exception as e:
        #     self.logger.debug(f"Error serializing event {event.type}: {e}")

        # Determine routing strategy based on mode manager
        should_use_redis = await self._should_route_to_redis_streams(redis_stream_id)
        
        # Determine current operation mode to avoid dual processing
        current_mode = None
        if self.mode_manager:
            current_mode = self.mode_manager.current_mode
        
        # Route to Redis Streams if mode manager permits
        if should_use_redis:
            await self._publish_to_redis_stream(event, redis_stream_id)

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
            "system_message": self._handle_system_message,
            "history_delta": self._ignore_event,
            "complete_thought": self._ignore_event,
        }

        handler = handlers.get(event.type)
        if handler:
            try:
                payload = await handler(event)

                # Optimize: Only publish to async queue when not in REDIS_ONLY mode
                # This eliminates dual processing overhead in REDIS_ONLY mode
                should_use_async_queue = (
                    current_mode != OperationMode.REDIS_ONLY or not should_use_redis
                )
                
                if payload is not None and hasattr(self, "_stream_queue") and should_use_async_queue:
                    await self._stream_queue.put(payload)

                    # If this is the end-of-stream event, push final payload and then a termination marker.
                    if event.type == "interaction" and not event.started:
                        await self._stream_queue.put(payload)
                        await self._stream_queue.put(None)

            except Exception as e:
                self.logger.error(f"Error in event handler {handler.__name__} for {event.type}: {str(e)}")
        else:
            self.logger.warning(f"Unhandled event type: {event.type}")

    async def _publish_to_redis_stream(self, event: SessionEvent, redis_stream_id: Optional[str] = None) -> None:
        """
        Publish an event to Redis Stream if enabled and configured.
        
        This method handles the Redis Stream publishing with proper error handling,
        mode manager integration, and fallback behavior. It will not raise exceptions
        to ensure the async queue fallback mechanism continues to work.
        
        Args:
            event: The SessionEvent to publish
            redis_stream_id: Optional Redis Stream ID in format "{session_id}:{interaction_id}"
        """
        # Check if Redis Streams should be used
        if not self._should_use_redis_streams(redis_stream_id):
            return

        try:
            # Use the provided stream ID or fall back to current stream ID
            stream_id = redis_stream_id or self._current_redis_stream_id

            if stream_id and self.redis_stream_manager:
                # Build the Redis Stream key using the configured prefix
                redis_stream_key = f"{settings.STREAM_PREFIX}{stream_id}"

                # Publish the raw event to Redis Stream
                message_id = await self.redis_stream_manager.publish_event(redis_stream_key, event)

                # Record successful Redis operation with mode manager
                if self.mode_manager:
                    self.mode_manager.record_redis_success()

                # Log success (debug level to avoid spam)
                #self.logger.debug(f"Published event {event.type} to Redis Stream {redis_stream_key}: {message_id}")

        except Exception as e:
            # Record Redis failure with mode manager
            if self.mode_manager:
                self.mode_manager.record_redis_failure()
                
                # For REDIS_ONLY mode, check if we should trigger emergency transition
                if (self.mode_manager.current_mode == OperationMode.REDIS_ONLY and
                    self.mode_manager.redis_circuit_breaker.state == "open"):
                    self.logger.warning(
                        f"Redis circuit breaker open in REDIS_ONLY mode. "
                        f"Consider switching to HYBRID mode for reliability."
                    )
            
            # Log error but don't re-raise to ensure async queue fallback works
            self.logger.error(f"Failed to publish event {event.type} to Redis Stream: {e}")
            # Could increment a metric here for monitoring

    async def _should_route_to_redis_streams(self, redis_stream_id: Optional[str] = None) -> bool:
        """
        Determine if events should be routed to Redis Streams based on mode manager.
        
        This method integrates with EventHandlerModeManager to make routing decisions
        based on current operation mode and Redis health status.
        
        Args:
            redis_stream_id: Optional Redis Stream ID to check
            
        Returns:
            bool: True if Redis Streams should be used for routing
        """
        # Check basic Redis Streams availability first
        if not self._should_use_redis_streams(redis_stream_id):
            return False
        
        # If mode manager is not available, fall back to basic check
        if not self.mode_manager:
            return True
        
        # Use mode manager to determine if Redis can be used
        try:
            can_use_redis = self.mode_manager.can_use_redis()
            
            # For HYBRID mode, handle fallback logic in _publish_to_redis_stream
            # For REDIS_ONLY mode, only use Redis if available
            # For ASYNC_ONLY mode, never use Redis
            
            if self.mode_manager.current_mode == OperationMode.ASYNC_ONLY:
                return False
            elif self.mode_manager.current_mode == OperationMode.REDIS_ONLY:
                return can_use_redis
            else:  # HYBRID mode
                return can_use_redis
                
        except Exception as e:
            self.logger.error(f"Error checking mode manager for Redis routing: {e}")
            # Fall back to basic availability check
            return True
    
    def _should_use_redis_streams(self, redis_stream_id: Optional[str] = None) -> bool:
        """
        Determine if Redis Streams should be used for event publishing (basic check).
        
        Args:
            redis_stream_id: Optional Redis Stream ID to check
            
        Returns:
            bool: True if Redis Streams should be used, False otherwise
        """
        return (
            self._redis_streams_enabled and
            self.redis_stream_manager is not None and
            (redis_stream_id is not None or self._current_redis_stream_id is not None)
        )

    async def initialize_redis_streams(self, redis_config: Optional['RedisConfig'] = None) -> Dict[str, Any]:
        """
        Initialize Redis Streams functionality with resilient mode support.
        
        This method initializes Redis Streams functionality with RedisConfig parameter
        support and sets up resilient mode components including EventHandlerModeManager
        and RedisHealthMonitor for dynamic mode switching.
        
        Args:
            redis_config: Optional RedisConfig instance for configuration-driven setup.
                         If not provided, uses environment variables for backward compatibility.
        
        Returns:
            Dict[str, Any]: Initialization status containing:
                - success: bool - True if initialization completed successfully
                - mode: str - Current operation mode
                - redis_enabled: bool - True if Redis Streams are enabled
                - health_monitoring: bool - True if health monitoring is active
                - error: Optional[str] - Error message if initialization failed
        """
        from agent_c_api.config.redis_config import RedisConfig
        
        initialization_status = {
            "success": False,
            "mode": "async_only",
            "redis_enabled": False,
            "health_monitoring": False,
            "error": None
        }
        
        # Check if Redis Streams are enabled via configuration
        #rohan
        use_redis_streams = getattr(settings, 'USE_REDIS_STREAMS', True)

        if not use_redis_streams:
            self.logger.info("Redis Streams disabled via configuration")
            # Still set up resilient mode in ASYNC_ONLY mode
            await self._initialize_async_only_mode()
            initialization_status.update({
                "success": True,
                "mode": "async_only"
            })
            return initialization_status

        try:
            # Initialize or get resilient configuration
            if redis_config:
                self.resilient_config = redis_config.get_resilient_config()
                if not self.resilient_config:
                    self.resilient_config = redis_config.create_default_resilient_config()
            else:
                # Create default configuration for backward compatibility
                self.resilient_config = RedisConfig.create_default_resilient_config()
            
            # Initialize RedisHealthMonitor
            await self._initialize_health_monitor(redis_config)
            
            # Initialize EventHandlerModeManager
            await self._initialize_mode_manager()
            
            # Test Redis connectivity
            redis_available = await self._test_redis_connectivity(redis_config)
            
            if redis_available:
                # Initialize Redis Stream manager
                await self._initialize_redis_stream_manager(redis_config)
                
                # Set initial mode based on configuration and Redis health
                initial_mode = await self._determine_initial_mode()
                
                initialization_status.update({
                    "success": True,
                    "mode": initial_mode.value,
                    "redis_enabled": True,
                    "health_monitoring": True
                })
                
                self.logger.info(
                    f"Redis Streams with resilient mode initialized successfully. "
                    f"Mode: {initial_mode.value}"
                )
            else:
                # Redis not available, start in ASYNC_ONLY mode
                await self._fallback_to_async_only_mode("Redis connectivity failed during initialization")
                
                initialization_status.update({
                    "success": True,
                    "mode": "async_only",
                    "redis_enabled": False,
                    "health_monitoring": True
                })

        except Exception as e:
            self.logger.error(f"Failed to initialize Redis Streams with resilient mode: {e}")
            
            # Attempt graceful fallback to async-only mode
            try:
                await self._fallback_to_async_only_mode(f"Initialization error: {e}")
                initialization_status.update({
                    "success": True,
                    "mode": "async_only",
                    "error": str(e)
                })
            except Exception as fallback_error:
                self.logger.error(f"Failed to initialize even async-only fallback: {fallback_error}")
                initialization_status.update({
                    "success": False,
                    "error": f"Complete initialization failure: {e}, fallback error: {fallback_error}"
                })
        
        return initialization_status
    
    async def _initialize_async_only_mode(self) -> None:
        """
        Initialize resilient mode components for ASYNC_ONLY operation.
        """
        from agent_c_api.config.redis_config import RedisConfig
        
        # Create ASYNC_ONLY configuration
        self.resilient_config = RedisConfig.configure_resilient_mode(
            operation_mode=OperationMode.ASYNC_ONLY
        )
        
        # Initialize mode manager in ASYNC_ONLY mode
        self.mode_manager = EventHandlerModeManager(self.resilient_config)
        
        self.logger.info("Initialized in ASYNC_ONLY mode")
    
    async def _initialize_health_monitor(self, redis_config: Optional['RedisConfig'] = None) -> None:
        """
        Initialize RedisHealthMonitor for Redis connectivity and performance monitoring.
        """
        from agent_c_api.config.redis_config import RedisConfig
        
        try:
            # Create Redis client getter function
            async def get_redis_client():
                if redis_config:
                    return await redis_config.get_redis_client()
                else:
                    return await RedisConfig.get_redis_client()
            
            # Initialize health monitor
            self.health_monitor = RedisHealthMonitor(
                redis_client_getter=get_redis_client,
                check_interval=self.resilient_config.health_check_interval_seconds,
                failure_threshold=self.resilient_config.redis_failure_threshold,
                latency_threshold_ms=100.0,  # Default latency threshold
                error_rate_threshold=0.05   # Default error rate threshold
            )
            
            # Register health callback with mode manager
            self.health_monitor.register_health_callback(
                "mode_manager", 
                self._health_status_callback
            )
            
            # Start background monitoring
            await self.health_monitor.start_monitoring()
            
            self.logger.info("RedisHealthMonitor initialized and started")
            
        except Exception as e:
            self.logger.error(f"Failed to initialize health monitor: {e}")
            raise
    
    async def _initialize_mode_manager(self) -> None:
        """
        Initialize EventHandlerModeManager with configuration and health monitoring.
        """
        try:
            self.mode_manager = EventHandlerModeManager(self.resilient_config)
            
            # Link health monitor to mode manager
            if self.health_monitor:
                self.mode_manager.set_health_monitor(self.health_monitor)
            
            # Register state change callbacks
            self.mode_manager.register_state_callback(
                "agent_bridge",
                self._mode_transition_callback
            )
            
            self.logger.info(f"EventHandlerModeManager initialized in {self.mode_manager.current_mode.value} mode")
            
        except Exception as e:
            self.logger.error(f"Failed to initialize mode manager: {e}")
            raise
    
    async def _test_redis_connectivity(self, redis_config: Optional['RedisConfig'] = None) -> bool:
        """
        Test Redis connectivity during initialization.
        """
        from agent_c_api.config.redis_config import RedisConfig
        
        try:
            if redis_config:
                status = await redis_config.validate_connection()
            else:
                status = await RedisConfig.validate_connection()
            
            if status["connected"]:
                self.logger.info(f"Redis connectivity test passed: {status['host']}:{status['port']}")
                return True
            else:
                self.logger.warning(f"Redis connectivity test failed: {status.get('error', 'Unknown error')}")
                return False
                
        except Exception as e:
            self.logger.error(f"Redis connectivity test error: {e}")
            return False
    
    async def _initialize_redis_stream_manager(self, redis_config: Optional['RedisConfig'] = None) -> None:
        """
        Initialize Redis Stream manager with proper configuration.
        """
        try:
            # Build Redis URL
            if redis_config:
                redis_url = redis_config.build_redis_url()
            else:
                redis_url = self._build_redis_url()
            
            # Initialize the Redis Stream manager (class-level)
            await RedisStreamManager.initialize(redis_url)
            
            # Set instance variables
            self.redis_stream_manager = RedisStreamManager

            self._redis_streams_enabled = True
            
            self.logger.info(f"Redis Stream manager initialized: {redis_url}")
            
        except Exception as e:
            self.logger.error(f"Failed to initialize Redis Stream manager: {e}")
            raise
    
    async def _determine_initial_mode(self) -> OperationMode:
        """
        Determine initial operation mode based on configuration and Redis health.
        """
        # Check configured operation mode
        configured_mode = self.resilient_config.operation_mode
        
        # For REDIS_ONLY mode, ensure Redis is healthy
        if configured_mode == OperationMode.REDIS_ONLY:
            if self.health_monitor and not await self.health_monitor.is_redis_healthy():
                self.logger.warning(
                    "REDIS_ONLY mode requested but Redis is unhealthy. "
                    "Switching to HYBRID mode for safety."
                )
                configured_mode = OperationMode.HYBRID
        
        # Set mode in mode manager
        if self.mode_manager:
            await self.mode_manager.request_mode_transition(
                target_mode=configured_mode,
                reason=TransitionReason.STARTUP_INITIALIZATION,
                force=True
            )
        
        return configured_mode
    
    async def _fallback_to_async_only_mode(self, reason: str) -> None:
        """
        Fallback to ASYNC_ONLY mode with proper initialization.
        """
        self.logger.warning(f"Falling back to ASYNC_ONLY mode: {reason}")
        
        try:
            # Ensure we have a mode manager in ASYNC_ONLY mode
            if not self.mode_manager:
                await self._initialize_async_only_mode()
            else:
                # Transition existing mode manager to ASYNC_ONLY
                await self.mode_manager.request_mode_transition(
                    target_mode=OperationMode.ASYNC_ONLY,
                    reason=TransitionReason.CONNECTION_ERROR,
                    force=True
                )
            
            # Disable Redis Streams
            self._redis_streams_enabled = False
            self.redis_stream_manager = None
            
            self.logger.info("Successfully transitioned to ASYNC_ONLY mode")
            
        except Exception as e:
            self.logger.error(f"Failed to initialize ASYNC_ONLY fallback mode: {e}")
            raise
    
    async def _health_status_callback(self, health_status: 'HealthStatus') -> None:
        """
        Callback for health status changes from RedisHealthMonitor.
        """
        try:
            if not health_status.is_healthy and self.mode_manager:
                current_mode = self.mode_manager.current_mode
                
                # Trigger mode transition for unhealthy Redis
                if current_mode == OperationMode.REDIS_ONLY:
                    await self.mode_manager.request_mode_transition(
                        target_mode=OperationMode.HYBRID,
                        reason=TransitionReason.HEALTH_CHECK_FAILURE,
                        metadata={"health_status": health_status.details}
                    )
                elif current_mode == OperationMode.HYBRID and health_status.failures_in_window > 5:
                    await self.mode_manager.request_mode_transition(
                        target_mode=OperationMode.ASYNC_ONLY,
                        reason=TransitionReason.HEALTH_CHECK_FAILURE,
                        metadata={"health_status": health_status.details}
                    )
            
            elif health_status.is_healthy and self.mode_manager:
                # Potentially recover from ASYNC_ONLY back to HYBRID
                if (self.mode_manager.current_mode == OperationMode.ASYNC_ONLY and
                    self.resilient_config.enable_auto_recovery):
                    await self.mode_manager.request_mode_transition(
                        target_mode=OperationMode.HYBRID,
                        reason=TransitionReason.RECOVERY_COMPLETE,
                        metadata={"health_status": health_status.details}
                    )
        
        except Exception as e:
            self.logger.error(f"Error in health status callback: {e}")
    
    async def _mode_transition_callback(self, event_type: str, transition: 'ModeTransition', error: Exception = None) -> None:
        """
        Callback for mode transition events from EventHandlerModeManager.
        """
        try:
            if event_type == "transition_complete":
                self.logger.info(
                    f"Mode transition completed: {transition.from_mode.value} → {transition.to_mode.value} "
                    f"(reason: {transition.reason.name})"
                )
            elif event_type == "transition_failed":
                self.logger.error(
                    f"Mode transition failed: {transition.from_mode.value} → {transition.to_mode.value} "
                    f"(reason: {transition.reason.name}, error: {transition.error_message})"
                )
            elif event_type == "transition_error":
                self.logger.error(
                    f"Mode transition error: {transition.from_mode.value} → {transition.to_mode.value} "
                    f"(error: {error})"
                )
        
        except Exception as e:
            self.logger.error(f"Error in mode transition callback: {e}")

    def _build_redis_url(self) -> str:
        """
        Build Redis URL from environment settings.
        
        Returns:
            str: Complete Redis URL for connection
        """
        # Build basic URL
        redis_url = f"redis://{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"

        # Add authentication if configured
        if settings.REDIS_PASSWORD:
            if settings.REDIS_USERNAME:
                redis_url = f"redis://{settings.REDIS_USERNAME}:{settings.REDIS_PASSWORD}@{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"
            else:
                redis_url = f"redis://:{settings.REDIS_PASSWORD}@{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"

        return redis_url

    def set_redis_stream_id(self, session_id: str, interaction_id: str) -> None:
        """
        Set the current Redis Stream ID for event publishing.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
        """
        self._current_redis_stream_id = f"{session_id}:{interaction_id}"
        self.logger.debug(f"Set Redis Stream ID: {self._current_redis_stream_id}")

    def clear_redis_stream_id(self) -> None:
        """
        Clear the current Redis Stream ID.
        """
        self._current_redis_stream_id = None
        self.logger.debug("Cleared Redis Stream ID")

    def _create_redis_aware_callback(self, redis_stream_id: Optional[str]):
        """
        Create a callback wrapper that includes Redis Stream ID.
        
        This method creates a callback function that preserves the existing
        logging and event handling while adding Redis Stream support.
        
        Args:
            redis_stream_id: The Redis Stream ID to use for publishing
            
        Returns:
            Callback function compatible with agent_c streaming interface
        """
        # Create the callback wrapper that includes Redis Stream ID
        async def redis_aware_callback(event: SessionEvent):
            """
            Callback wrapper that adds Redis Stream support to consolidated_streaming_callback.
            """
            try:
                # Call the existing consolidated callback with Redis Stream ID
                await self.consolidated_streaming_callback(event, redis_stream_id)
            except Exception as e:
                self.logger.error(f"Error in Redis-aware callback: {e}")
                # Don't re-raise to avoid breaking the chat flow

        # Use the existing logging wrapper but with our Redis-aware callback
        from agent_c.util.event_session_logger_factory import create_with_callback

        return create_with_callback(
            log_base_dir=os.getenv('AGENT_LOG_DIR', DEFAULT_LOG_DIR),
            callback=redis_aware_callback,  # Our Redis-aware callback
            include_system_prompt=True
        )

    async def stream_chat(
        self,
        user_message: str,
        client_wants_cancel: threading.Event,
        file_ids: Optional[List[str]] = None,
    ) -> AsyncGenerator[str, None]:
        """
        Streams chat responses for a given user message.

        This method handles the complete chat interaction process, including:
        - Session updates
        - Custom prompt integration
        - Message processing
        - Response streaming via Redis Streams or async queue fallback
        - Event handling

        Enhanced with Redis Streams support for distributed event processing while
        maintaining backward compatibility with async queue fallback.

        Args:
            user_message (str): The message from the user to process
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

        # Generate unique interaction ID for this chat session
        interaction_id = str(uuid.uuid4())
        session_id = self.chat_session.session_id
        redis_stream_id = f"{session_id}:{interaction_id}"

        # Set up Redis Stream ID for this interaction
        self.set_redis_stream_id(session_id, interaction_id)

        try:
            await self.session_manager.update()

            file_inputs = []
            if file_ids and self.file_handler:
                file_inputs = await self.process_files_for_message(file_ids, self.chat_session.user_id)

                # Log information about processed files
                if file_inputs:
                    input_types = {type(input_obj).__name__: 0 for input_obj in file_inputs}
                    for input_obj in file_inputs:
                        input_types[type(input_obj).__name__] += 1
                    self.logger.info(f"Processing {len(file_inputs)} files: {input_types}")

            prompt_metadata = await self.__build_prompt_metadata()
            # Prepare chat parameters
            tool_params = {}
            if len(self.chat_session.agent_config.tools):
                await self.tool_chest.initialize_toolsets(self.chat_session.agent_config.tools)
                tool_params = self.tool_chest.get_inference_data(self.chat_session.agent_config.tools, self.agent_runtime.tool_format)
                tool_params["toolsets"] = self.chat_session.agent_config.tools

            if self.sections is not None:
                agent_sections = self.sections
            elif "ThinkTools" in self.chat_session.agent_config.tools:
                agent_sections = [ThinkSection(), DynamicPersonaSection()]
            else:
                agent_sections = [DynamicPersonaSection()]

            chat_params = {
                "streaming_queue": queue,
                "user_id": self.chat_session.user_id,
                "chat_session": self.chat_session,
                "user_message": user_message,
                "prompt_metadata": prompt_metadata,
                "output_format": DEFAULT_OUTPUT_FORMAT,
                "client_wants_cancel": client_wants_cancel,
                "streaming_callback": self._create_redis_aware_callback(self._current_redis_stream_id),
                'tool_call_context': {'active_agent': self.chat_session.agent_config},
                'prompt_builder': PromptBuilder(sections=agent_sections)
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

            full_params = chat_params | tool_params

            # Start the chat task
            chat_task = asyncio.create_task(
                self.agent_runtime.chat(**full_params)
            )

            # Use mode manager to determine streaming approach
            consumption_strategy = await self._determine_consumption_strategy(redis_stream_id)
            self.logger.debug(
                f"Using consumption strategy: {consumption_strategy['method']} "
                f"(mode: {consumption_strategy.get('mode', 'unknown')})"
            )
            
            # Execute consumption strategy with dynamic switching support
            try:
                if consumption_strategy['method'] == 'redis_streams':
                    # Consume from Redis Streams with fallback capability
                    async for content in self._consume_with_mode_manager(
                        redis_stream_id, 
                        client_wants_cancel, 
                        initial_strategy=consumption_strategy
                    ):
                        if content is not None:
                            yield content
                        else:
                            self.logger.info("Received termination signal")
                            break
                            
                elif consumption_strategy['method'] == 'hybrid':
                    # Hybrid consumption with fallback monitoring
                    async for content in self._consume_hybrid_mode(
                        redis_stream_id, 
                        queue, 
                        client_wants_cancel
                    ):
                        if content is not None:
                            yield content
                        else:
                            self.logger.info("Received termination signal")
                            break
                            
                else:
                    # Async queue only
                    async for content in self._consume_async_queue_events(queue, client_wants_cancel):
                        if content is not None:
                            yield content
                        else:
                            self.logger.info("Received async queue termination signal")
                            break
                            
            except Exception as e:
                self.logger.error(f"Consumption strategy {consumption_strategy['method']} failed: {e}")
                
                # Emergency fallback to async queue if all else fails
                if consumption_strategy['method'] != 'async_queue':
                    self.logger.warning("Emergency fallback to async queue consumption")
                    async for content in self._consume_async_queue_events(queue, client_wants_cancel):
                        if content is not None:
                            yield content
                        else:
                            self.logger.info("Received async queue termination signal")
                            break
                else:
                    # Re-raise if async queue itself failed
                    raise

            await chat_task
            await self.session_manager.flush(self.chat_session.session_id)

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
            # Clear Redis Stream ID for this interaction
            self.clear_redis_stream_id()
    
    async def _determine_consumption_strategy(self, redis_stream_id: str) -> Dict[str, Any]:
        """
        Determine consumption strategy based on mode manager and current operation mode.
        
        Args:
            redis_stream_id: Redis Stream ID for this interaction
            
        Returns:
            Dict[str, Any]: Strategy information containing method and mode details
        """
        strategy = {
            "method": "async_queue",
            "mode": "unknown",
            "can_use_redis": False,
            "fallback_available": True
        }
        
        # Check basic Redis Streams availability
        basic_redis_available = self._should_use_redis_streams(redis_stream_id)

        # Debug logging rohan
        self.logger.debug(
            f"Redis streams availability check: _redis_streams_enabled={self._redis_streams_enabled}, redis_stream_manager={self.redis_stream_manager is not None}, stream_id available={redis_stream_id is not None or self._current_redis_stream_id is not None}")
        self.logger.debug(
            f"basic_redis_available={basic_redis_available}, mode_manager_available={self.mode_manager is not None}")
        if self.mode_manager:
            self.logger.debug(
                f"mode_manager: current_mode={self.mode_manager.current_mode.value}, can_use_redis={self.mode_manager.can_use_redis()}, circuit_breaker={self.mode_manager.redis_circuit_breaker.state}")
        #
        if not basic_redis_available:
            return strategy
        
        # Use mode manager to determine strategy
        if self.mode_manager:
            try:
                current_mode = self.mode_manager.current_mode
                can_use_redis = self.mode_manager.can_use_redis()

                strategy.update({
                    "mode": current_mode.value,
                    "can_use_redis": can_use_redis
                })
                
                if current_mode == OperationMode.REDIS_ONLY:
                    if can_use_redis:
                        strategy["method"] = "redis_streams"
                        strategy["fallback_available"] = False
                    else:
                        # Redis required but not available - this is an error condition
                        self.logger.error(
                            "REDIS_ONLY mode but Redis is not available. "
                            "This may cause consumption failures."
                        )
                        strategy["method"] = "redis_streams"  # Attempt anyway
                        strategy["fallback_available"] = False
                        
                elif current_mode == OperationMode.HYBRID:
                    if can_use_redis:
                        strategy["method"] = "hybrid"
                    else:
                        strategy["method"] = "async_queue"
                        
                elif current_mode == OperationMode.ASYNC_ONLY:
                    strategy["method"] = "async_queue"
                    
            except Exception as e:
                self.logger.error(f"Error determining consumption strategy: {e}")
                # Fall back to basic availability check
                if basic_redis_available:
                    strategy["method"] = "redis_streams"
        else:
            # No mode manager, use basic availability
            if basic_redis_available:
                strategy["method"] = "redis_streams"

        return strategy
    
    async def _consume_with_mode_manager(
        self, 
        redis_stream_id: str, 
        client_wants_cancel: threading.Event,
        initial_strategy: Dict[str, Any]
    ) -> AsyncGenerator[Optional[str], None]:
        """
        Consume events using mode manager with dynamic switching support.
        
        This method monitors the mode manager during consumption and can switch
        consumption methods if the mode changes during an active session.
        
        Args:
            redis_stream_id: Redis Stream ID
            client_wants_cancel: Cancellation event
            initial_strategy: Initial consumption strategy
            
        Yields:
            Optional[str]: Event payloads or None for termination
        """
        current_strategy = initial_strategy.copy()
        
        try:
            # Monitor for mode changes during consumption
            last_mode_check = time.time()
            mode_check_interval = 5.0  # Check mode every 5 seconds
            
            async for content in self._consume_redis_stream_events(redis_stream_id, client_wants_cancel):
                # Periodically check if mode has changed
                current_time = time.time()
                if current_time - last_mode_check > mode_check_interval:
                    if await self._should_switch_consumption_method(current_strategy):
                        self.logger.info("Mode change detected during consumption, continuing with Redis...")
                        # Note: For now we continue with current method rather than switching mid-stream
                        # Full mid-stream switching would require more complex session management
                    last_mode_check = current_time
                
                if content is not None:
                    yield content
                else:
                    break
                    
                # Check for cancellation
                if client_wants_cancel.is_set():
                    self.logger.debug("Client requested cancellation during mode-managed consumption")
                    break
                    
        except Exception as e:
            self.logger.error(f"Mode-managed consumption failed: {e}")
            
            # Record Redis failure if we have mode manager
            if self.mode_manager:
                self.mode_manager.record_redis_failure()
            
            # For REDIS_ONLY mode, this is a critical failure
            if (self.mode_manager and 
                self.mode_manager.current_mode == OperationMode.REDIS_ONLY):
                self.logger.error(
                    "Critical: Redis consumption failed in REDIS_ONLY mode. "
                    "No fallback available."
                )
                raise
            else:
                # For other modes, this will trigger fallback in the caller
                raise
    
    async def _consume_hybrid_mode(
        self, 
        redis_stream_id: str, 
        queue: asyncio.Queue,
        client_wants_cancel: threading.Event
    ) -> AsyncGenerator[Optional[str], None]:
        """
        Consume events in hybrid mode with Redis primary and async queue fallback.
        
        Args:
            redis_stream_id: Redis Stream ID
            queue: Async queue for fallback
            client_wants_cancel: Cancellation event
            
        Yields:
            Optional[str]: Event payloads or None for termination
        """
        try:
            # Start with Redis Streams
            self.logger.debug("Starting hybrid consumption with Redis Streams primary")
            
            async for content in self._consume_redis_stream_events(redis_stream_id, client_wants_cancel):
                if content is not None:
                    yield content
                else:
                    break
                    
                if client_wants_cancel.is_set():
                    break
                    
        except Exception as e:
            self.logger.warning(f"Redis consumption failed in hybrid mode, falling back to async queue: {e}")
            
            # Record Redis failure
            if self.mode_manager:
                self.mode_manager.record_redis_failure()
            
            # Fallback to async queue
            self.logger.debug("Falling back to async queue in hybrid mode")
            async for content in self._consume_async_queue_events(queue, client_wants_cancel):
                if content is not None:
                    yield content
                else:
                    break
    
    async def _should_switch_consumption_method(self, current_strategy: Dict[str, Any]) -> bool:
        """
        Check if consumption method should be switched due to mode changes.
        
        Args:
            current_strategy: Current consumption strategy
            
        Returns:
            bool: True if method should be switched
        """
        if not self.mode_manager:
            return False
        
        try:
            current_mode = self.mode_manager.current_mode
            
            # Check if mode has changed from what we expected
            if current_strategy.get("mode") != current_mode.value:
                self.logger.debug(
                    f"Mode change detected: {current_strategy.get('mode')} → {current_mode.value}"
                )
                return True
            
            # Check if Redis availability has changed
            can_use_redis = self.mode_manager.can_use_redis()
            if current_strategy.get("can_use_redis") != can_use_redis:
                self.logger.debug(
                    f"Redis availability changed: {current_strategy.get('can_use_redis')} → {can_use_redis}"
                )
                return True
            
            return False
            
        except Exception as e:
            self.logger.error(f"Error checking for consumption method switch: {e}")
            return False

            # Drain any remaining items in the queue.
            while not queue.empty():
                try:
                    await queue.get()
                    queue.task_done()
                except asyncio.QueueEmpty:
                    break

    async def _consume_redis_stream_events(self, redis_stream_id: str, client_wants_cancel: threading.Event) -> AsyncGenerator[Optional[str], None]:
        """
        Consume events from Redis Stream.

        Args:
            redis_stream_id: Redis Stream ID in format "{session_id}:{interaction_id}"
            client_wants_cancel: Event to signal cancellation

        Yields:
            Optional[str]: JSON-formatted event payloads or None for termination
        """
        if not self.redis_stream_manager:
            self.logger.error("Redis Stream manager not available for consumption")
            return

        # Parse session and interaction IDs from redis_stream_id
        try:
            session_id, interaction_id = redis_stream_id.split(':', 1)
        except ValueError:
            self.logger.error(f"Invalid Redis Stream ID format: {redis_stream_id}")
            return

        # Build the Redis Stream key using the configured prefix
        redis_stream_key = f"{settings.STREAM_PREFIX}{redis_stream_id}"

        self.logger.debug(f"Starting Redis Stream consumption: {redis_stream_key}")

        try:
            # Consume events from Redis Stream
            consumer_name = f"bridge_{session_id}_{interaction_id}"

            async for event in self.redis_stream_manager.consume_events(
                stream_id=redis_stream_key,
                consumer_group="agent_bridge",
                consumer_name=consumer_name,
                batch_size=10,
                poll_interval=0.1
            ):
                # Check for cancellation
                if client_wants_cancel.is_set():
                    self.logger.debug("Client requested cancellation during Redis Stream consumption")
                    break

                # Process the event and convert to client format
                try:
                    processed_payload = await self._process_redis_stream_event(event)
                    if processed_payload:
                        yield processed_payload

                    # Check for interaction end
                    if event.type == "interaction" and not getattr(event, 'started', True):
                        self.logger.debug("Received interaction end event from Redis Stream")
                        yield None  # Signal termination
                        break

                except Exception as e:
                    self.logger.error(f"Error processing Redis Stream event: {e}")
                    continue

        except Exception as e:
            self.logger.error(f"Redis Stream consumption error: {e}")
            raise

    async def _process_redis_stream_event(self, event) -> Optional[str]:
        """
        Process a Redis Stream event and convert to client format.

        Args:
            event: SessionEvent from Redis Stream

        Returns:
            Optional[str]: JSON-formatted payload or None if event should be skipped
        """
        try:
            # Use the existing event handlers to process the event
            # This ensures consistent formatting between Redis and queue consumption
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
                "system_message": self._handle_system_message,
            }

            handler = handlers.get(event.type)
            if handler:
                return await handler(event)
            elif event.type in ["history_delta", "complete_thought"]:
                # These events are ignored in the original implementation
                return None
            else:
                self.logger.warning(f"Unhandled Redis Stream event type: {event.type}")
                return None

        except Exception as e:
            self.logger.error(f"Error processing Redis Stream event {event.type}: {e}")
            return None

    async def _consume_async_queue_events(self, queue: asyncio.Queue, client_wants_cancel: threading.Event) -> AsyncGenerator[Optional[str], None]:
        """
        Consume events from async queue (fallback method).

        Args:
            queue: The asyncio Queue to consume from
            client_wants_cancel: Event to signal cancellation

        Yields:
            Optional[str]: JSON-formatted event payloads or None for termination
        """
        while True:
            try:
                timeout = getattr(settings, "CALLBACK_TIMEOUT", 30)  # Get timeout from settings with fallback
                # For now, don't use timeout as it causes issues
                content = await queue.get()
                if content is None:
                    self.logger.info("Received async queue termination signal")
                    yield None
                    break
                yield content
                queue.task_done()
            except asyncio.TimeoutError:
                timeout_msg = f"Timeout waiting for stream content to occur in agent_bridge.py:stream_chat. Waiting for stream surpassed {timeout} seconds, terminating stream."
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
                unexpected_msg = f"Unexpected error in async queue processing: {error_type}: {str(e)}\n{error_traceback}"
                self.logger.error(unexpected_msg)
                # Yield error message and break
                yield json.dumps({"type": "error", "data": unexpected_msg}) + "\n"
                break

    async def process_files_for_message(
        self,
        file_ids: List[str],
        session_id: str
    ) -> List[Union[FileInput, ImageInput, AudioInput]]:
        """
        Process files and convert them to appropriate Input objects for the agent.

        This method processes uploaded files and converts them to the appropriate input
        objects (FileInput, ImageInput, AudioInput) for handling by the agent's multimodal
        capabilities.

        Args:
            file_ids: List of file IDs to process.
            session_id: Session ID for file processing context.

        Returns:
            List of input objects for the agent, typed as FileInput, ImageInput, or AudioInput.

        Raises:
            Exception: If file processing fails (logged but not re-raised).
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