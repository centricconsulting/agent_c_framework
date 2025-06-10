from .workspace import WorkspaceTools, LocalStorageWorkspace, WorkspaceSection
from .web_search import WikipediaTools, HackerNewsTools, SeekingAlphaTools, TavilyResearchTools
from .web_search import GoogleSerpTools, GoogleTrendsTools, HackerNewsTools, NewsApiTools
from .user_preferences import UserPreference, AddressMeAsPreference, AssistantPersonalityPreference, \
    UserPreferencesTools
from .rss import RssTools
from .mermaid_chart import MermaidChartTools
from .dall_e import DallETools
from .memory import MemoryTools
from .user_bio import UserBioTools
from .web import WebTools
from .weather import Weather
from .random_number import RandomNumberTools
from .think import ThinkTools
from .dynamics import DynamicsTools
from .markdown_to_html_report import MarkdownToHtmlReportTools
from .css_explorer import CssExplorerTools
from .mariadb import MariadbTools
from .math import MathTools
from .workspace_planning import WorkspacePlanningTools
from .workspace_knowledge import WorkspaceKnowledgeTools
from .workspace_sequential_thinking import WorkspaceSequentialThinkingTools
from .browser_playwright import BrowserPlaywrightTools
from .data_visualization import DataVisualizationTools
from .database_query import DatabaseQueryTools
from .code_interpreter import CodeInterpreterTools
from .xml_explorer import XmlExplorerTools
from .dataframe import DataframeTools
from .gmail import GmailSearch, GmailMessage

__all__ = [
    # Essential Tools for good agents
    'MemoryTools',
    'ThinkTools',
    'MarkdownToHtmlReportTools',

    # Code Exploring Tools
    'CssExplorerTools',
    'XmlExplorerTools',

    # Workspace tools
    'WorkspaceTools',
    'LocalStorageWorkspace',
    'WorkspaceSection',

    # Planning and Knowledge Tools
    'WorkspacePlanningTools',
    'WorkspaceKnowledgeTools',
    'WorkspaceSequentialThinkingTools',

    # Web tools
    'WikipediaTools',
    'HackerNewsTools',
    'SeekingAlphaTools',
    'TavilyResearchTools',
    'GoogleSerpTools',
    'GoogleTrendsTools',
    'NewsApiTools',
    'WebTools',
    'GmailSearch',
    'GmailMessage',

    # Centric Dynamics 365 Tools
    "DynamicsTools",

    # User preference tools
    'UserPreference',
    'AddressMeAsPreference',
    'AssistantPersonalityPreference',
    'UserPreferencesTools',

    # Other tools
    'RssTools',
    'MermaidChartTools',
    'DallETools',
    'UserBioTools',
    'Weather',
    'RandomNumberTools',
    'MathTools',
    'BrowserPlaywrightTools',
    'CodeInterpreterTools',

    # Data tools
    'DataVisualizationTools',
    'DatabaseQueryTools',
    'MariadbTools',
    'DataframeTools',

]
