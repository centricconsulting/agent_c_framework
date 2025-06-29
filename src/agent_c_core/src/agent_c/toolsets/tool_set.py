import os
import copy
import yaml
import inspect

from typing import Union, List, Dict, Any, Optional, Type, TYPE_CHECKING
from agent_c.models.events import RenderMediaEvent
from agent_c.util.markdown_to_html import md_to_html
from agent_c.util.logging_utils import LoggingManager

if TYPE_CHECKING:
    from agent_c.toolsets.tool_chest import ToolChest
    from agent_c.toolsets.tool_cache import ToolCache
    from agent_c.models.context.interaction_context import InteractionContext
    from agent_c.models.context.base import BaseContext
    from agent_c.prompting.prompt_section import OldPromptSection


class Toolset:
    # The class variables below are used by the ToolsetRegistry to provide metadata about the toolset.
    # These are not instance variables and should not be set in the constructor.
    # Derived classes should declare their own overrides of these variables
    context_types: List[str] = []           # Types of context based classes used by this toolset, for registry lookup.
    config_types: List[str] = []            # Types of XxConfig based classes used by this toolset, for registry lookup.
    prompt_section_types: List[str] = []    # Types of prompt sections used by this toolset, for registry lookup.
    prompt_macro_types: List[str] = []      # Types of prompt sections with macros this toolset provides tor rendering in templates.
    multi_user: bool = False                # Set to true if this toolset is designed to be used by multiple users simultaneously
    required_toolsets: List[str] = []       # List of toolset classes that this toolset requires to function properly.
    force_prefix: bool = True               # If true, the toolset name will always be prefixed to tool names.
                                            # Used to  either to group its tools or avoid collisions with other tools.
    tool_prefix: str = ""                   # The actual prefix used for this toolset, typically the short name.
                                            # If not set, the toolset class name will be converted to snake_case, minus "Tools" suffix and used


    # toolset_name: str = "My Cool Toolset" # This is not defaulted in the base class, but can be set in subclasses
                                            # if not defined the toolset class name will be converted to Title Case

    #                             This is also not defaulted in the base class, but can be set in subclasses
    #                             The registry will use the `__doc__` attribute to provide a description
    #                             of the toolset if this is not set.
    # user_description: str = "A toolset that provides various tools for use in the agent."

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

        from agent_c.util.registries.toolset_registry import ToolsetRegistry
        ToolsetRegistry.register(tool_cls)

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
    def default_context(cls) -> Optional['BaseContext']:
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
                tool_chest (ToolChest): Holds the active/tools available to the toolset.
                tool_cache (ToolCache): Cache for tools.
                section (OldPromptSection | None): Section-related information.
                needed_keys (List[str]): List of environment keys required for the toolset functionality.
                tool_role (str): Defines the role of the tool (defaults to 'tool').
        """
        # Initialize properties
        self.name: str = kwargs.get("name")


        if self.name is None:
            raise ValueError("Toolsets must have a name.")

        logging_manager = LoggingManager(self.__class__.__name__)
        self.logger = logging_manager.get_logger()

        # Store tool_chest first since it's critical for dependencies
        self.tool_chest: 'ToolChest' = kwargs.get("tool_chest")

        self.use_prefix: bool = kwargs.get("use_prefix", True)

        self.valid: bool = True

        self.tool_cache: 'ToolCache' = kwargs.get("tool_cache")
        self.section: Union['OldPromptSection', None] = kwargs.get('section')
        self.tool_role: str = kwargs.get('tool_role', 'tool')

    @staticmethod
    def _count_tokens(text: str, tool_context) -> int:
        if not text or len(text) == 0:
            return 0

        return tool_context.agent_runtime.count_tokens(text)

    def get_dependency(self, toolset_name: str, context: 'InteractionContext') -> Optional['Toolset']:
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
        tool_chest: Optional['ToolChest'] = context.tool_chest if hasattr(context, 'tool_chest') else self.tool_chest
        if not tool_chest:
            raise RuntimeError(f"Toolset {self.name} attempted to access dependency {toolset_name} but no tool_chest is available")


        return tool_chest.available_tools.get(toolset_name)

    @property
    def prefix(self) -> str:
        """
        Returns the prefix for the toolset.

        Returns:
            str: The prefix for the toolset.
        """
        if self.use_prefix:
            return f"{self.name}"

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


    async def _render_media_markdown(self, tool_context: 'InteractionContext', markdown_text: str, sent_by: str, **kwargs: Any) -> None:
        await self._raise_render_media(tool_context,
                sent_by_class=self.__class__.__name__,
                sent_by_function=sent_by,
                content_type="text/html",
                content=md_to_html(markdown_text),
                **kwargs)

    async def _raise_render_media(self, tool_context: 'InteractionContext', content_type: Optional[str] = 'text/markdown', **kwargs: Any) -> None:
        """
        Raises a render media event.

        Args:
            kwargs: The arguments to be passed to the render media event.
        """
        kwargs['role']  = kwargs.get('role', self.tool_role)
        if content_type == 'text/markdown' and 'content' in kwargs:
            content_type = 'text/html'
            kwargs['content'] = md_to_html(kwargs['content'])

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
        return {self.name: self, "schemas": self.tool_schemas, "doc": self.__doc__}

    @classmethod
    def tool_methods(cls: Type["Toolset"]):
        """
        Returns all tool methods declared on this class.

        Looks for any function in the class namespace that has a `json_schema` decorator.
        """
        tool_methods = []

        # Iterate over class-level attributes
        for name, member in cls.__dict__.items():
            if name.startswith("_"):
                continue

            if not (inspect.isfunction(member) or inspect.ismethod(member)):
                continue

            if hasattr(member, "schema"):
                tool_methods.append(member)

        return tool_methods

    @classmethod
    def tool_schemas(cls: Type["Toolset"], prefix: Optional[str] = None) -> List[Dict[str, Any]]:
        """
        Returns the tool schemas declared on this class.

        Looks for any function in the class namespace with a `.schema` attribute,
        copies it, applies the prefix if requested, and returns the list.
        """
        schemas: List[Dict[str, Any]] = []
        for member in cls.tool_methods():
            if prefix is not None:
                schema = copy.deepcopy(member.schema)
                schema["function"]["name"] = f"{prefix}{Toolset.tool_sep}{schema['function']['name']}"
            else:
                schema = member.schema

            schemas.append(schema)

        return schemas
