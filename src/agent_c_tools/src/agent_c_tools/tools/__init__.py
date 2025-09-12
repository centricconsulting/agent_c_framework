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
from . import reverse_engineering  # Reverse Engineering toolset
from . import math  # Math toolset
from . import workspace_planning  # Workspace Planning toolset
from . import workspace_knowledge  # Workspace Knowledge toolset
from . import workspace_sequential_thinking  # Workspace Sequential Thinking toolset
from . import browser_playwright  # Browser Playwright toolset
from . import uipath  # UIPath toolset

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
    'reverse_engineering',
    'math',
    'workspace_planning',
    'workspace_knowledge',
    'workspace_sequential_thinking',
    'browser_playwright',
    'uipath',
    'sars',
    'dataframe',
    'data_visualization',
    'dall_e',
    'database_query',
    'dynamics',
    'gmail',
    'linked_in',
    'markdown_to_html_report',
    'office_to_markdown',
    'rss',
    'salesforce',
    'user_preferences',
    'youtube',
    'agent_assist',
    'agent_clone',
    'agent_team',
    'code_interpreter',
    'tool_debugger'
]