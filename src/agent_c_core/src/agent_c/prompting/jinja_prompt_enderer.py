import datetime
import platform
from pathlib import Path

from agent_c.models.prompts.agent_prompt_collection import AgentPromptCollection
from agent_c.registration.configs import locate_config_folder
from jinja2 import Environment, FileSystemBytecodeCache
from agent_c.prompting.section_registry_loader import RegistryLoader
from agent_c.registration.configs import get_system_config


class JinjaPromptRenderer:
    """
    A class to manage Jinja2 template rendering for prompts in the Agent C framework.
    """
    def __init__(self):
        self._create_byte_cache()
        self._env = None


    @property
    def environment(self):
        """ Returns the Jinja2 environment for rendering templates. """
        if self._env is None:
            self._create_environment()
        return self._env

    def _create_globals(self):
        baseline = {}

        from agent_c.util.registries.context_registry import ContextRegistry
        extras = ContextRegistry.find_and_create("*_globals")
        for context_type, context_instance in extras.items():
            if context_instance.add_as_object:
                baseline[context_type] = context_instance

            baseline.update(context_instance.globals)

        return baseline

    def _create_environment(self):
        """ Create a Jinja2 environment for rendering templates.
        This environment uses a custom loader that retrieves templates from the SectionRegistry.
        It also sets up bytecode caching to improve performance.
        """
        config = get_system_config().core.prompt_rendering
        self._env = Environment(loader=RegistryLoader(),
                                enable_async=True,
                                bytecode_cache=self._bccache,
                                cache_size=config.jinja2_cache_size,
                                autoescape=config.jinja2_auto_escape,
                                trim_blocks=config.jinja2_trim_blocks,
                                lstrip_blocks=config.jinja2_lstrip_blocks,
                                keep_trailing_newline=config.jinja2_keep_trailing_newline,
                                auto_reload=config.jinja2_auto_reload,
                                extensions=config.jinja2_extensions,
                                )
        self._env.globals = self._create_globals()
        self._env.newstyle_gettext = config.jinja2_newstyle_gettext

    def _create_byte_cache(self):
        """
        Create a bytecode cache for Jinja2 templates.
        This is used to speed up template loading by caching compiled templates.
        """
        path = Path(locate_config_folder()).joinpath("jinja_2_cache")
        if not path.exists():
            path.mkdir(parents=True, exist_ok=True)

        self._bccache = FileSystemBytecodeCache(str(path))

    @property
    def current_date_time_stamp(self) -> str:
        """
        Retrieves the current local timestamp formatted according to the OS platform.

        Returns:
            str: Formatted timestamp.

        Raises:
            Logs an error if formatting the timestamp fails, returning an error message.
        """
        try:
            local_time_with_tz = datetime.datetime.now(datetime.timezone.utc).astimezone()
            if platform.system() == "Windows":
                formatted_timestamp = local_time_with_tz.strftime('%A %B %#d, %Y %#I:%M%p (%Z %z)')
            else:
                formatted_timestamp = local_time_with_tz.strftime('%A %B %-d, %Y %-I:%M%p (%Z %z)')
            return formatted_timestamp
        except Exception:
            return 'SYSTEM ERROR'

    #from agent_c.models.context.interaction_context import InteractionContext
    def render_agent_prompt(self, context_in: 'InteractionContext') -> str:
        """
        Render the agent prompt using the provided context.
        This method uses the Jinja2 environment to render the prompt template.
        """
        prompt_collection = AgentPromptCollection.from_interaction_context(context_in)
        prompt_context = { 'environment': self._env,
                           'prompt_collection': prompt_collection,
                           'interaction': context_in,
                           'session': context_in.chat_session,
                           'user': context_in.chat_session.user,
                           'machines': context_in.chat_session.machines,
                           'current_date_time_stamp': self.current_date_time_stamp,
                         }

        # Include any additional globals from prompt sections.
        # Jinja doesn't allow dynamic names for imports via templates so this mimics
        # the functionality in Jinja2, allowing sections to provide additional context
        for section in prompt_collection.includes:
            template = self._env.get_template(section.template_key)
            prompt_context[section.import_name] = template.module


        # Render the master prompt template
        return self._env.get_template("agent_master").render(context=prompt_context)


