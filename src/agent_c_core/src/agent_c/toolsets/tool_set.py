import os
import copy
import inspect
from typing import Union, List, Callable, Dict, Any

from agent_c.models.chat_event import ChatEvent
from agent_c.prompting.prompt_section import PromptSection
from agent_c.toolsets.tool_cache import ToolCache
from agent_c.chat.session_manager import ChatSessionManager


class Toolset:
    tool_registry: List[Any] = []
    tool_sep: str = "-"

    @classmethod
    def register(cls, tool_cls: Any) -> None:
        """
        Registers a tool class in the tool registry if it's not already present.

        Args:
            tool_cls: The class of the tool to be registered.
        """
        if tool_cls not in cls.tool_registry:
            cls.tool_registry.append(tool_cls)

    def __init__(self, **kwargs: Any) -> None:
        """
        Initializes the Toolset with the provided options.

        Args:
            kwargs:
                name (str): The name of the toolset.
                session_manager (ChatSessionManager): Manages chat sessions.
                tool_chest (ToolChest): Holds the active/tools available to the toolset.
                required_tools (List[str]): A list of tools that are required to be activated.
                tool_cache (ToolCache): Cache for tools.
                section (PromptSection | None): Section-related information.
                agent_can_use_tools (bool): If the agent can use toolsets (defaults to True if unset).
                need_tool_user (bool): Defines if this toolset requires a tool-using agent (defaults to True).
                needed_keys (List[str]): List of environment keys required for the toolset functionality.
                streaming_callback (Callable[..., None]): A callback to be triggered after streaming events.
                output_format (str): Format for output. Defaults to 'raw'.
                tool_role (str): Defines the role of the tool (defaults to 'tool').
        """
        # Initialize properties
        self.name: str = kwargs.get("name")
        if self.name is None:
            raise ValueError("Toolsets must have a name.")

        self.session_manager: ChatSessionManager = kwargs.get("session_manager")
        self.tool_chest: 'ToolChest' = kwargs.get("tool_chest")

        # Handle required tools activation
        required_tools: List[str] = kwargs.get("required_tools", [])
        for tool_name in required_tools:
            if tool_name not in self.tool_chest.active_tools:
                self.valid: bool = self.tool_chest.activate_tool(tool_name)
                if not self.valid:
                    break

        self.tool_cache: ToolCache = kwargs.get("tool_cache")
        self.section: Union[PromptSection, None] = kwargs.get('section')

        # Agent capabilities and tool requirements
        self.agent_can_use_tools: bool = kwargs.get("agent_can_use_tools", True)
        self.need_tool_user: bool = kwargs.get("need_tool_user", True)

        # Validate environment variables
        needed_keys: List[str] = kwargs.get('needed_keys', [])
        self.tool_valid: bool = self._validate_env_keys(needed_keys)

        # If toolset requires a tool-using agent but the agent cannot use tools, invalid toolset
        if self.need_tool_user and not self.agent_can_use_tools:
            self.tool_valid = False

        self.openai_schemas: List[Dict[str, Any]] = self.__openai_schemas()

        # Additional attributes
        self.streaming_callback: Callable[..., None] = kwargs.get('streaming_callback')
        self.output_format: str = kwargs.get('output_format', 'raw')
        self.tool_role: str = kwargs.get('tool_role', 'tool')

    async def chat_callback(self, **kwargs: Any) -> None:
        """
        Callback function for chat events that sends events via the streaming_callback.

        Args:
            kwargs:
                role (str): The role of the tool. If not supplied, defaults to `self.tool_role`.
                output_format (str): The output format of the message. Defaults to `self.output_format`.
                session_id (str | None): The session ID, either from kwargs or fetched from session_manager.
                Other fields are passed to initialize the ChatEvent.
        """
        if not self.streaming_callback:
            return

        # Ensure 'role' and 'output_format' exist in kwargs
        kwargs['role'] = kwargs.get('role', self.tool_role)
        kwargs['output_format'] = kwargs.get('output_format', self.output_format)

        # Add session ID to kwargs if not provided
        if kwargs.get('session_id') is None:
            if self.session_manager is not None:
                kwargs['session_id'] = self.session_manager.chat_session.session_id
            else:
                kwargs['session_id'] = 'None'

        # Create a ChatEvent and invoke the callback
        event = ChatEvent(**kwargs)
        await self.streaming_callback(event)

    async def post_init(self) -> None:
        """
        Optional post-initialization method that can be used for additional setup.
        """
        pass

    @staticmethod
    def _validate_env_keys(needed_keys: List[str]) -> bool:
        """
        Validates that certain environment keys are present and non-empty.

        Args:
            needed_keys (List[str]): List of keys to check in the environment.

        Returns:
            bool: True if all needed keys are present and non-empty, False otherwise.
        """
        for key in needed_keys:
            if not os.getenv(key):
                return False
        return True

    def dict(self) -> Dict[str, 'Toolset']:
        """
        Returns a dictionary representation of the Toolset.

        Returns:
            Dict[str, Toolset]: A dictionary with the toolset name as the key and the Toolset as the value.
        """
        return {self.name: self}

    def __openai_schemas(self) -> List[Dict[str, Any]]:
        """
        Generate OpenAI-compatible schemas based on method metadata.

        Returns:
            List[Dict[str, Any]]: A list of OpenAI schemas for the registered methods in the Toolset.
        """
        openai_schemas = []
        for name, method in inspect.getmembers(self, predicate=inspect.ismethod):
            if hasattr(method, 'schema'):
                schema = copy.deepcopy(method.schema)
                schema['function']['name'] = f"{self.name}{Toolset.tool_sep}{schema['function']['name']}"
                openai_schemas.append(schema)

        return openai_schemas