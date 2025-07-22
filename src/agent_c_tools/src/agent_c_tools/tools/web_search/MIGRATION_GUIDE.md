# Web Search Tools Migration Guide

## Overview

The web search tools have been consolidated into a unified `WebSearchTools` class that provides a single, consistent interface for all web search functionality. This migration guide explains the changes and how to update your code.

## New Unified Interface

### Primary Tool: `WebSearchTools`

The new `WebSearchTools` class provides:
- **Unified Interface**: Single tool for all search types
- **Intelligent Routing**: Automatic engine selection based on query analysis
- **Standardized Responses**: Consistent JSON format across all engines
- **Enhanced Capabilities**: Advanced features like domain filtering, date ranges, etc.

### Available Search Methods

1. **`web_search()`** - General web search with automatic engine selection
2. **`news_search()`** - News-optimized search
3. **`educational_search()`** - Academic and educational content
4. **`research_search()`** - Deep research with content analysis
5. **`tech_search()`** - Technology and programming community content
6. **`flights_search()`** - Flight search using Google Flights
7. **`events_search()`** - Event search using Google Events
8. **`get_engine_info()`** - Engine status and capabilities

## Migration Examples

### Before (Legacy Tools)
```python
# Old way - multiple different tools
from agent_c_tools.tools.web_search import (
    DuckDuckGoTools, GoogleSerpTools, NewsApiTools
)

ddg = DuckDuckGoTools()
results = await ddg.web_search(query="python programming")

news = NewsApiTools()
articles = await news.get_all_articles(q="AI news")
```

### After (Unified Tool)
```python
# New way - single unified tool
from agent_c_tools.tools.web_search import WebSearchTools

search = WebSearchTools()

# General web search (automatically selects best engine)
results = await search.web_search(query="python programming")

# News search (automatically uses news-optimized engines)
articles = await search.news_search(query="AI news")

# Tech community search
tech_results = await search.tech_search(query="python programming")
```

## Legacy Tool Status

### Integrated Tools (Deprecated)
These tools are now wrapped by engine adapters and accessed through `WebSearchTools`:

- ✅ **DuckDuckGoTools** → Use `WebSearchTools.web_search(engine="duckduckgo")`
- ✅ **GoogleSerpTools** → Use `WebSearchTools.web_search(engine="google_serp")`
- ✅ **NewsApiTools** → Use `WebSearchTools.news_search(engine="newsapi")`
- ✅ **TavilyResearchTools** → Use `WebSearchTools.research_search(engine="tavily")`
- ✅ **WikipediaTools** → Use `WebSearchTools.educational_search(engine="wikipedia")`
- ✅ **HackerNewsTools** → Use `WebSearchTools.tech_search(engine="hackernews")`

### Standalone Tools (Still Available)
These tools provide unique functionality not yet integrated:

- 🔄 **GoogleTrendsTools** - Provides Google Trends analysis (unique functionality)
- 🔄 **SeekingAlphaTools** - Provides financial news from Seeking Alpha (unique functionality)

## Benefits of Migration

### For Users
- **Simpler API**: One tool instead of many
- **Better Results**: Intelligent engine selection
- **Consistent Format**: Standardized responses
- **Enhanced Features**: Domain filtering, date ranges, etc.

### For Developers
- **Easier Maintenance**: Single codebase
- **Better Error Handling**: Unified error management
- **Extensible**: Easy to add new engines
- **Type Safety**: Comprehensive parameter validation

## Advanced Features

### Engine Selection
```python
# Automatic selection (recommended)
results = await search.web_search(query="machine learning")

# Explicit engine selection
results = await search.web_search(query="machine learning", engine="tavily")

# Search type optimization
results = await search.research_search(query="machine learning")  # Uses research-optimized engines
```

### Advanced Parameters
```python
results = await search.web_search(
    query="climate change research",
    search_type="research",
    max_results=20,
    include_domains=["nature.com", "science.org"],
    exclude_domains=["wikipedia.org"],
    start_date="2023-01-01",
    end_date="2024-01-01",
    search_depth="advanced"
)
```

## Timeline

- **Phase 1** (Current): Unified tool available, legacy tools deprecated
- **Phase 2** (Future): Legacy tool directories will be removed
- **Phase 3** (Future): Only `WebSearchTools` will be available

## Support

For questions or issues with migration:
1. Check the `WebSearchTools` documentation
2. Use `get_engine_info()` to check engine capabilities
3. Review this migration guide
4. Test with the unified interface before removing legacy tool usage

## Backward Compatibility

Legacy tools remain available for now but are deprecated. Update your code to use `WebSearchTools` to ensure future compatibility.