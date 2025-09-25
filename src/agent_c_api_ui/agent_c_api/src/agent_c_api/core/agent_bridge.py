import os
import json
import asyncio
import threading
import traceback
from typing import Any, Dict, List, Union, Optional, AsyncGenerator, TYPE_CHECKING
from datetime import datetime, timezone


from agent_c.config import ModelConfigurationLoader
from agent_c.models.context.interaction_context import InteractionContext
from agent_c.models.input import AudioInput
from agent_c.agents.gpt import GPTChatAgent, AzureGPTChatAgent
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
from agent_c.prompting import PromptBuilder, CoreInstructionSection
from agent_c.prompting.basic_sections.persona import DynamicPersonaSection
from agent_c.agents.claude import ClaudeChatAgent, ClaudeBedrockChatAgent
from agent_c.util.event_session_logger_factory import create_with_callback
from agent_c_tools.tools.workspace.base import BaseWorkspace
from agent_c_tools.tools.workspace.local_storage import LocalProjectWorkspace

if TYPE_CHECKING:
    from agent_c.models.events import SessionEvent, SystemMessageEvent
    from agent_c.models.agent_config import CurrentAgentConfiguration
    from agent_c.chat import ChatSessionManager



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
    __vendor_agent_map = {
        "azure_openai": AzureGPTChatAgent,
        "openai": GPTChatAgent,
        "claude": ClaudeChatAgent,
        "bedrock": ClaudeBedrockChatAgent
    }

    def __init__(
        self,
        chat_session: ChatSession,
        session_manager: 'ChatSessionManager',
        file_handler: Optional[FileHandler] = None
    ) -> None:
        """
        Initialize the AgentBridge instance.

        This initializes a bridge interface between the agent_c library and ReactJS
        applications for chat functionality, setting up all necessary components
        including logging, tool management, and agent configuration.

        Args:
            chat_session: The chat session to manage.
            session_manager: Session manager for chat sessions.
            model_name: Name of the AI model to use. Defaults to 'claude-sonnet-4-20250514'.
            file_handler: Handler for file operations. Defaults to None.
            **kwargs: Additional optional keyword arguments including:
                - tool_cache_dir (str): Directory for tool cache
                - sections (List): Sections for the prompt builder
        
        Raises:
            Exception: If there are errors during tool or agent initialization.
        """
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()
        # Set up streaming_callback with logging
        self.streaming_callback_with_logging = create_with_callback(
            log_base_dir=os.getenv('AGENT_LOG_DIR', DEFAULT_LOG_DIR),
            callback=self.consolidated_streaming_callback,  # Forward to UI processing
            include_system_prompt=True
        )

        self.__init_events()

        self.chat_session = chat_session
        self.sections = None

        self.model_config_loader = ModelConfigurationLoader()
        self.model_configs: Dict[str, Any] = self.model_config_loader.flattened_config()
        self.runtime_cache: Dict[str, BaseAgent] = {}


        self.debug_event = None
        self.session_manager = session_manager


        # Tool Chest, Cache, and Setup
        # - Tool Chest: A collection of toolsets that the agent can use, Set to None initialization.
        # - Tool Cache: A cache for storing tool data, set to a default directory.
        # - Output Tool Arguments: A placeholder for tool argument output preference.
        self.tool_chest: Union[ToolChest, None] = None
        self.tool_cache_dir = DEFAULT_TOOL_CACHE_DIR
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)
        self.output_tool_arguments = True  # Placeholder for tool argument output preference

        # Agent Workspace Setup
        self.workspaces: Optional[List[BaseWorkspace]] = None
        self.__init_workspaces()

        # Initialize file handling capabilities
        self.file_handler = file_handler
        self.image_inputs: List[ImageInput] = []
        self.audio_inputs: List[AudioInput] = []

    def runtime_for_agent(self, agent_config: 'CurrentAgentConfiguration'):
        if agent_config.key in self.runtime_cache:
            return self.runtime_cache[agent_config.key]
        else:
            self.runtime_cache[agent_config.key] = self._runtime_for_agent(agent_config)
            return self.runtime_cache[agent_config.key]

    def _runtime_for_agent(self, agent_config: 'CurrentAgentConfiguration') -> BaseAgent:
        model_config = self.model_configs[agent_config.model_id]
        runtime_cls = self.__vendor_agent_map[model_config["vendor"]]

        auth_info = agent_config.agent_params.auth.model_dump() if agent_config.agent_params.auth is not None else  {}
        client = runtime_cls.client(**auth_info)
        return runtime_cls(model_name=model_config["id"], client=client)


    def __init_events(self) -> None:
        """
        Initialize threading events used for debugging and input/output management.
        
        Sets up various threading events for coordinating agent operations,
        including exit handling, input state management, and TTS cancellation.
        """
        self.exit_event = threading.Event()
        self.input_active_event = threading.Event()
        self.cancel_tts_event = threading.Event()
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
        # TODO: ALLOWED / DISALLOWED WORKSPACES from agent config
        try:
            with open(LOCAL_WORKSPACES_FILE, 'r', encoding='utf-8') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            # Local workspaces file is optional
            pass

    async def rename_current_session(self, new_name: str) -> None:
        """
        Rename the current chat session.

        Args:
            new_name: The new name for the chat session.

        Raises:
            Exception: If there are errors during renaming or session flushing.
        """
        self.logger.info(
            f"Renaming session {self.chat_session.session_id} to '{new_name}'"
        )
        self.chat_session.name = new_name


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
            - Duplicate tools are automatically removed
            - Tool chest is reinitialized with the updated tool set
            - Agent is reinitialized with new tools while maintaining the session
        """
        self.logger.info(
            f"Requesting new tool list for agent {self.chat_session.agent_config.key} to: {new_tools}"
        )
        self.chat_session.agent_config.tools = new_tools
        await self.tool_chest.activate_toolset(self.chat_session.agent_config.tools)
        self.logger.info(
            f"Tools updated successfully. Current Active tools: "
            f"{list(self.tool_chest.active_tools.keys())}"
        )

    async def _init_tool_chest(self) -> None:
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
        agent_params = self.chat_session.agent_config.agent_params.model_dump(exclude_none=True)

        agent_params |= {
            "prompt_builder": prompt_builder,
            "tool_chest": self.tool_chest,
            "streaming_callback": self.streaming_callback_with_logging
        }

        self.logger.info(f"Agent initialized using the following parameters: {agent_params}")

    async def reset_streaming_state(self) -> None:
        """
        Reset streaming state to ensure clean session.
        
        Creates a new asyncio Queue for streaming responses, ensuring that
        each chat interaction starts with a clean state.
        """
        self.logger.info(
            f"Resetting streaming state for session {self.chat_session.session_id}"
        )
        self._stream_queue = asyncio.Queue()

    async def _build_prompt_metadata(self) -> Dict[str, Any]:
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
        agent_meta = self.chat_session.agent_config.prompt_metadata or {}
        return {
            "session_id": self.chat_session.session_id,
            "current_user_username": self.chat_session.user_id,
            "persona_prompt": self.chat_session.agent_config.persona,
            "agent_config": self.chat_session.agent_config,
            "timestamp": datetime.now().isoformat(),
            "env_name": os.getenv('ENV_NAME', DEFAULT_ENV_NAME),
            "user_session_id": self.chat_session.session_id,
            "chat_session": self.chat_session,
            "chat_user": self.chat_user
        } | agent_meta

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
        initialized_tools = [{
            'instance_name': instance_name,
            'class_name': tool_instance.__class__.__name__,
            'developer_tool_name': getattr(tool_instance, 'name', instance_name),
        } for instance_name, tool_instance in self.tool_chest.active_tools.items()]

        config = {
            'backend': "anthropic",
            'model_name': self.chat_session.agent_config.agent_params.model_name,
            'initialized_tools': initialized_tools,
            'agent_name': self.chat_session.agent_config.name,
            'user_session_id': self.chat_session.session_id,
            'agent_session_id': self.chat_session.session_id,
            'output_format': "markdown",
            'created_time': self._current_timestamp(),
            'temperature': self.chat_session.agent_config.agent_params.temperature if "temperature" in self.chat_session.agent_config.agent_params.__fields__ else None,
            'reasoning_effort': self.chat_session.agent_config.agent_params.reasoning_effort if "reasoning_effort" in self.chat_session.agent_config.agent_params.__fields__ else None,
            'agent_parameters': self.chat_session.agent_config.agent_params.model_dump(exclude_defaults=False)
        }

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
    async def _handle_message(event: 'SessionEvent') -> str:
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
    async def _handle_system_message(event: 'SystemMessageEvent') -> str:
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
    async def _ignore_event(_: 'SessionEvent') -> None:
        """
        Ignore certain event types that don't require processing.
        
        Args:
            _: Session event to ignore.
            
        Returns:
            None: No payload is generated for ignored events.
        """
        return None

    async def _handle_tool_select_delta(self, event: 'SessionEvent') -> str:
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

    async def _handle_tool_call_delta(self, event: 'SessionEvent') -> str:
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

    async def _handle_text_delta(self, event: 'SessionEvent') -> str:
        """
        Handle text delta events from the agent/tools.
        
        Determines the appropriate vendor based on backend and model name,
        then formats the text content for streaming to the client.
        
        Args:
            event: Session event containing text delta information.
            
        Returns:
            str: JSON-formatted content payload with vendor information.
        """

        
        payload = json.dumps({
            "type": "content",
            "data": event.content,
            "vendor": self.runtime_for_agent(self.chat_session.agent_config).tool_format,
            "format": event.format
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_tool_call(event: 'SessionEvent') -> str:
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
    async def _handle_render_media(event: 'SessionEvent') -> str:
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

    async def _handle_history(self, event: 'SessionEvent') -> str:
        """
        Handle history events which update the chat log.
        
        Updates the chat session with new message history and flushes
        the session to persistent storage.
        
        Args:
            event: Session event containing message history.
            
        Returns:
            str: JSON-formatted history payload.
            
        Raises:
            Exception: If session flushing fails.
        """
        self.chat_session.messages = event.messages
        await self.session_manager.flush(self.chat_session.session_id, self.chat_session.user_id)
        
        payload = json.dumps({
            "type": "history",
            "messages": event.messages,
            "vendor": self.runtime_for_agent(self.chat_session.agent_config).tool_format,
            "model_name": self.chat_session.agent_config.agent_params.model_name,
        }) + "\n"
        return payload

    @staticmethod
    async def _handle_audio_delta(_: 'SessionEvent') -> None:
        """
        Handle audio events if voice features are enabled.
        
        Currently, audio events don't require special handling for tools/agents.
        
        Args:
            _: Session event containing audio delta information (ignored).
            
        Returns:
            None: No payload is generated for audio deltas.
        """
        return None


    async def _handle_completion(self, event: 'SessionEvent') -> str:
        """
        Handle completion events from the agent.
        
        Processes completion status information including token usage
        and stop reasons for the chat interaction.
        
        Args:
            event: Session event containing completion information.
            
        Returns:
            str: JSON-formatted completion status payload.
        """
        if not event.running:
            size = event.input_tokens + event.output_tokens
            if size > 0:
                self.chat_session.context_window_size = size
                self.chat_session.token_count += size

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
    async def _handle_interaction(event: 'SessionEvent') -> str:
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

    async def _handle_thought_delta(self, event: 'SessionEvent') -> str:
        """
        Handle thinking process events.
        
        Processes thought delta events from reasoning models, formatting
        them with appropriate vendor and model information.
        
        Args:
            event: Session event containing thought delta information.
            
        Returns:
            str: JSON-formatted thought delta payload.
        """

        
        payload = json.dumps({
            "type": "thought_delta",
            "data": event.content,
            "vendor": self.runtime_for_agent(self.chat_session.agent_config).tool_format,
            "model_name": self.chat_session.agent_config.agent_params.model_name,
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
        await self._init_tool_chest()
        await self.initialize_agent_parameters()


    async def consolidated_streaming_callback(self, event: 'SessionEvent') -> None:
        """
        Process and route various types of session events through appropriate handlers.

        This method serves as the central event processing hub, handling various event
        types including text updates, tool calls, media rendering, and completion status.
        It formats the events into JSON payloads suitable for streaming to the client.

        Args:
            event: The session event to process, containing type and payload information.

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
        # try:
        #     self.logger.debug(
        #         f"Consolidated callback received event: {event.model_dump_json(exclude={'content_bytes'})}")
        # except Exception as e:
        #     self.logger.debug(f"Error serializing event {event.type}: {e}")

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
            "system_prompt": self._ignore_event,
            "user_request": self._ignore_event,
        }

        handler = handlers.get(event.type)
        if handler:
            try:
                payload = await handler(event)
                if payload is not None and hasattr(self, "_stream_queue"):
                    await self._stream_queue.put(payload)

                    # If this is the end-of-stream event, push final payload and then a termination marker.
                    if event.type == "interaction" and not event.started and event.session_id == self.chat_session.session_id:
                        await self._stream_queue.put(None)


            except Exception as e:
                self.logger.error(f"Error in event handler {handler.__name__} for {event.type}: {str(e)}")
        else:
            self.logger.warning(f"Unhandled event type: {event.type}")

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
        - Response streaming
        - Event handling

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

        try:
            await self.session_manager.update()
            agent_runtime = self.runtime_for_agent(self.chat_session.agent_config)
            file_inputs = []
            if file_ids and self.file_handler:
                file_inputs = await self.process_files_for_message(file_ids, self.chat_session.session_id)

                # Log information about processed files
                if file_inputs:
                    input_types = {type(input_obj).__name__: 0 for input_obj in file_inputs}
                    for input_obj in file_inputs:
                        input_types[type(input_obj).__name__] += 1
                    self.logger.info(f"Processing {len(file_inputs)} files: {input_types}")

            prompt_metadata = await self._build_prompt_metadata()
            # Prepare chat parameters
            tool_params = {}
            if len(self.chat_session.agent_config.tools):
                await self.tool_chest.initialize_toolsets(self.chat_session.agent_config.tools)
                tool_params = self.tool_chest.get_inference_data(self.chat_session.agent_config.tools, agent_runtime.tool_format)
                tool_params['schemas'] = self.chat_session.agent_config.filter_allowed_tools(tool_params['schemas'])
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
                "tool_chest": self.tool_chest,
                "user_message": user_message,
                "prompt_metadata": prompt_metadata,
                "output_format": DEFAULT_OUTPUT_FORMAT,
                "client_wants_cancel": client_wants_cancel,
                "streaming_callback": self.streaming_callback_with_logging,
                'prompt_builder': PromptBuilder(sections=agent_sections),
                'bridge': self,
                'tool_context': {'active_agent': self.chat_session.agent_config,
                                 'bridge': self,
                                 'parent_session_id': None,
                                 'user_session_id': self.chat_session.session_id,
                                 'user_id': self.chat_session.user_id,
                                 'session_id': self.chat_session.session_id,
                                 "client_wants_cancel": self.client_wants_cancel,
                                 "env_name": os.getenv('ENV_NAME', 'development'),
                                 "streaming_callback": self.streaming_callback_with_logging,
                                 "prompt_metadata": prompt_metadata
                                 },

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

            context = InteractionContext(client_wants_cancel=client_wants_cancel,
                                         agent_config=self.chat_session.agent_config,
                                         tool_chest=self.tool_chest, sections=agent_sections,
                                         streaming_callback=self.streaming_callback_with_logging,
                                         chat_session=self.chat_session, tool_schemas=tool_params["schemas"],
                                         agent_runtime=agent_runtime,
                                         user_session_id=self.chat_session.session_id,
                                          )

            # Start the chat task
            chat_task = asyncio.create_task(agent_runtime.chat(**full_params))

            while True:
                try:
                    try:
                        timeout = getattr(settings, "CALLBACK_TIMEOUT")  # Get timeout from settings with fallback
                        #content = await asyncio.wait_for(queue.get(), timeout=timeout)
                        content = await queue.get()
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
            await self.session_manager.flush(self.chat_session.session_id, self.chat_session.user_id)

        except Exception as e:
            self.logger.exception (f"Error in stream_chat: {e}", exc_info=True)
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
