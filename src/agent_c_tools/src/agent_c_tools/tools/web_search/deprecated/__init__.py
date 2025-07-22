"""
Deprecated web search tools.

These tools are deprecated and will be removed in a future version.
Use WebSearchTools instead for all web search functionality.

Tools in this directory:
- google_trends: Provides Google Trends functionality (not yet integrated)
- seeking_alpha: Provides Seeking Alpha financial news (not yet integrated)
"""

# Import deprecated tools for backward compatibility
from .google_trends import GoogleTrendsTools
from .seeking_alpha import SeekingAlphaTools

__all__ = ['GoogleTrendsTools', 'SeekingAlphaTools']