import datetime
import platform
import socket
from pydantic import Field, model_validator

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals
from agent_c.registration import locate_config_folder


class SystemGlobals(BasePromptGlobals):
    """
    Registers system / server related globals in the Jinja 2 environment.
    """
    add_as_object: bool = Field(False)
    config_folder: str = Field(default_factory=lambda: locate_config_folder())
    server_start_time: str = Field(default_factory=lambda: datetime.datetime.isoformat())
    server_host_name: str = Field(default_factory=lambda: socket.gethostbyaddr(socket.gethostname()))
    server_ip: str = Field(default_factory=lambda: socket.gethostbyaddr(socket.gethostname())[2][0])

    api_version: str = Field(...)
    api_name: str = Field(...)
    api_base_dir: str = Field(...)
    runtime_environment: str = Field(...)
    server_operating_system_name: str = Field(...)
    server_operating_system_release: str = Field(...)
    server_operating_system_version: str = Field(...)
    server_hardware_id: str = Field(...)



    @model_validator(mode='after')
    def ensure_api_version(self):
        uname = platform.uname()
        self.api_version = self.system_config.api.app.version
        self.api_name = self.system_config.api.app.name
        self.api_base_dir = self.system_config.api.app.base_dir
        self.runtime_environment = self.system_config.api.app.runtime_environment
        self.server_operating_system_name = uname.system
        self.server_operating_system_release = uname.release
        self.server_operating_system_version = uname.version
        self.server_hardware_id = uname.machine
        return self


