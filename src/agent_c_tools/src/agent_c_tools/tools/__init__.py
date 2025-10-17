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
from . import insurance_demo  # Insurance Demo toolset
from . import sars  # SARS toolset
from agent_c.toolsets.claude_server_tools import ClaudeWebSearchTools, ClaudeWebFetchTools, ClaudeComputerUseTools, ClaudeCodeExecutionTools
from .toolbelt.tool import ToolbeltTools
from .microsoft_stream.tool import MicrosoftStreamTools

