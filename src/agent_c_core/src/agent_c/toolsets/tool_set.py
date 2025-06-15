import os
import copy
import yaml
import inspect
import markdown

from typing import Union, List, Dict, Any, Optional


from agent_c.toolsets.tool_cache import ToolCache
from agent_c.models.events import RenderMediaEvent
from agent_c.models.context.base import BaseContext
from agent_c.util.markdown_to_html import md_to_html
from agent_c.util.logging_utils import LoggingManager
from agent_c.prompting.prompt_section import PromptSection
from agent_c.models.context.interaction_context import InteractionContext




class Toolset:
    tool_registry: List[Any] = []
    tool_sep: str = "_"
    tool_dependencies: Dict[str, List[str]] = {}

    @classmethod
    def register(cls, tool_cls: Any, required_tools: Optional[List[str]] = None) -> None:
        """
        Registers a tool class in the tool registry with its dependencies.
        
        Args:
            tool_cls: The class of the tool to be registered.
            required_tools: List of tool names that this tool requires.
        """
        if tool_cls not in cls.tool_registry:
            cls.tool_registry.append(tool_cls)

            # Store the required tools mapping if provided
            if required_tools:
                cls.tool_dependencies[tool_cls.__name__] = required_tools

    @classmethod
    def get_required_tools(cls, toolset_name: str) -> List[str]:
        """
        Get the required tools for a specific toolset.
        
        Args:
            toolset_name: The name of the toolset to get required tools for.
            
        Returns:
            List[str]: List of required tool names, or empty list if none.
        """
        return cls.tool_dependencies.get(toolset_name, [])

    @classmethod
    def default_context(cls) -> Optional[BaseContext]:
        """
        Returns the default context for the toolset.

        Returns:
            Optional[BaseContext]: The default context, or None if not set.
        """
        return None

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
        self._schemas: list[Dict[str, Any]] = []

        if self.name is None:
            raise ValueError("Toolsets must have a name.")

        logging_manager = LoggingManager(self.__class__.__name__)
        self.logger = logging_manager.get_logger()

        # Store tool_chest first since it's critical for dependencies
        self.tool_chest: 'ToolChest' = kwargs.get("tool_chest")

        self.use_prefix: bool = kwargs.get("use_prefix", True)

        self.valid: bool = True  # post init will deactivate invalid tools.

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

        # Additional attributes
        self.streaming_callback = kwargs.get('streaming_callback')
        self.output_format: str = kwargs.get('output_format', 'raw')
        self.tool_role: str = kwargs.get('tool_role', 'tool')

    @staticmethod
    def _count_tokens(text: str, tool_context) -> int:
        if not text or len(text) == 0:
            return 0

        return tool_context.agent_runtime.count_tokens(text)

    def get_dependency(self, toolset_name: str) -> Optional['Toolset']:
        """
        Safely get a dependency toolset by name.
        This is a safer way to access dependencies than going through tool_chest.active_tools directly.
        
        Args:
            toolset_name: Name of the dependency toolset to get
            
        Returns:
            The toolset instance if found, None otherwise
            
        Example:
            base_toolset = self.get_dependency('BaseToolset')
            if base_toolset:
                # Use the toolset safely
                pass
        """
        if not self.tool_chest:
            raise RuntimeError(f"Toolset {self.name} attempted to access dependency {toolset_name} but no tool_chest is available")


        return self.tool_chest.available_tools.get(toolset_name)

    @property
    def prefix(self) -> str:
        """
        Returns the prefix for the toolset.

        Returns:
            str: The prefix for the toolset.
        """
        if self.use_prefix:
            return f"{self.name}{Toolset.tool_sep}"

        return ""

    async def call(self, tool_name: str, args: dict[str, Any]):
        """
        Calls a tool on this toolset with the given name and arguments.

        Args:
            tool_name (str): The name of the tool to call.
            args (dict[str, Any]): The arguments to pass to the tool.

        Returns:
            Any: The result of the tool call.
        """
        function_name = tool_name.removeprefix(self.prefix)
        function_to_call: Any = getattr(self, function_name)
        return await function_to_call(**args)

    @staticmethod
    def _yaml_dump(data: Any) -> str:
        """
        Dumps data to a YAML formatted string.

        Args:
            data (Any): The data to be dumped.

        Returns:
            str: The YAML formatted string.
        """
        return yaml.dump(data, allow_unicode=True, sort_keys=False)


    async def _render_media_markdown(self, tool_context: InteractionContext, markdown_text: str, sent_by: str, **kwargs: Any) -> None:
        await self._raise_render_media(tool_context,
                sent_by_class=self.__class__.__name__,
                sent_by_function=sent_by,
                content_type="text/html",
                content=md_to_html(markdown_text),
                **kwargs)

    async def _raise_render_media(self, tool_context: InteractionContext, content_type: Optional[str] = 'text/markdown', **kwargs: Any) -> None:
        """
        Raises a render media event.

        Args:
            kwargs: The arguments to be passed to the render media event.
        """
        kwargs['role']  = kwargs.get('role', self.tool_role)
        if content_type == 'text/markdown' and 'content' in kwargs:
            content_type = 'text/html'
            kwargs['content'] = markdown.markdown(kwargs['content'])

        # Create the event object
        render_media_event = RenderMediaEvent(content_type=content_type,
                                              session_id=tool_context.chat_session.user_session_id,
                                              **kwargs)

        await tool_context.streaming_callback(render_media_event)


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

    @property
    def tool_schemas(self) -> List[Dict[str, Any]]:
        """
        Returns the tool schemas for the Toolset.

        Returns:
            List[Dict[str, Any]]: A list of tool schemas.
        """
        return self._tool_schemas()

    def _tool_schemas(self) -> List[Dict[str, Any]]:
        """
        Generate OpenAI-compatible JSON schemas based on method metadata.

        Returns:
            List[Dict[str, Any]]: A list of JSON schemas for the registered methods in the Toolset.
        """
        if len(self._schemas) > 0:
            return self._schemas

        for name in dir(self):
            if name.startswith('_') or name == 'tool_schemas':
                continue
            try:
                attr = getattr(self, name)
                if inspect.ismethod(attr) and hasattr(attr, 'schema'):
                    schema = copy.deepcopy(attr.schema)
                    if self.use_prefix:
                        schema['function']['name'] = f"{self.prefix}{schema['function']['name']}"
                    self._schemas.append(schema)
            except (AttributeError, RecursionError):
                # Skip attributes that cause issues
                continue

        return self._schemas
