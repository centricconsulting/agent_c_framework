"""
Unified web search engine implementations.

This module contains all the engine adapter implementations that conform to the
BaseWebSearchEngine interface, providing a consistent API across different
search providers.
"""

from .duckduckgo_engine import DuckDuckGoEngine, create_duckduckgo_engine
from .google_serp_engine import GoogleSerpEngine, create_google_serp_engine
from .tavily_engine import TavilyEngine, create_tavily_engine
from .wikipedia_engine import WikipediaEngine, create_wikipedia_engine
from .news_api_engine import NewsApiEngine, create_news_api_engine
from .hacker_news_engine import HackerNewsEngine, create_hacker_news_engine

__all__ = [
    'DuckDuckGoEngine',
    'GoogleSerpEngine', 
    'TavilyEngine',
    'WikipediaEngine',
    'NewsApiEngine',
    'HackerNewsEngine',
    'create_duckduckgo_engine',
    'create_google_serp_engine',
    'create_tavily_engine',
    'create_wikipedia_engine',
    'create_news_api_engine',
    'create_hacker_news_engine'
]