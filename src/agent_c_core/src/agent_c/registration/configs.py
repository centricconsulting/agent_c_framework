from agent_c.models.config import *


def get_system_config():
    from agent_c.config.system_config_loader import  SystemConfigurationLoader
    return SystemConfigurationLoader.instance().config

def get_config_registry():
    from agent_c.util.registries.config_registry import ConfigRegistry
    return ConfigRegistry
