import datetime
import os
import socket
from pathlib import Path

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
        self._config = None
        self._config_folder = None

    def create_environment(self):
        """ Create a Jinja2 environment for rendering templates. """
        self._grab_configs()
        self._create_environment()
        return self._env

    def _grab_configs(self):
        self._config_folder = locate_config_folder()
        self._config = get_system_config()


    def _create_globals(self):

        uname = os.uname()
        return {
            'api_version': self._config.api.app.version,
            'api_name': self._config.api.app.name,
            'api_base_dir': self._config.api.app.base_dir,
            'config_folder': locate_config_folder(),
            'runtime_environment': self._config.api.app.runtime_environment,
            'server_start_time': datetime.datetime.isoformat(),
            'server_operating_system_name': uname.sysname,
            'server_operating_system_release': uname.release,
            'server_operating_system_version': uname.version,
            'server_hardware_id': uname.machine,
            'server_host_name': socket.gethostbyaddr(socket.gethostname()),
        }


    def _create_environment(self):
        """ Create a Jinja2 environment for rendering templates.
        This environment uses a custom loader that retrieves templates from the SectionRegistry.
        It also sets up bytecode caching to improve performance.
        """
        config = self._config.core.prompt_rendering
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