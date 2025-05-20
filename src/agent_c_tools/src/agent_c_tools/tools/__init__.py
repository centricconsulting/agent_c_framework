# Register all tools for the agent

from . import workspace
from . import web
from . import web_search
from . import memory
from . import think
from . import mermaid_chart
from . import user_bio
from . import weather
from . import random_number
from . import xml_explorer  # XML Explorer toolset
from . import css_explorer  # CSS Explorer toolset
from . import mariadb  # MariaDB toolset
from . import cmd_exec
__all__ = [
    'workspace',
    'web', 
    'web_search', 
    'memory',
    'think',
    'mermaid_chart',
    'user_bio',
    'weather',
    'random_number',
    'xml_explorer',
    'css_explorer',
    'mariadb',
    'cmd_exec'
]