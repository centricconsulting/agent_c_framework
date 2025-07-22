# Web Search Tools - Comprehensive Documentation

This directory contains the unified web search system that consolidates all web search functionality into a single, powerful interface. This documentation provides complete usage instructions, parameter references, and best practices for the WebSearchTools system.

## Directory Structure

```
web_search/
├── web_search_tools.py          # Main unified interface (USE THIS)
├── base/                        # Core infrastructure
│   ├── engine.py               # Base engine interface
│   ├── models.py               # Data models and types
│   ├── registry.py             # Engine registry system
│   ├── router.py               # Intelligent routing logic
│   ├── validator.py            # Parameter validation
│   ├── standardizer.py         # Response standardization
│   └── error_handler.py        # Error handling
├── engines/                     # Engine adapters
│   ├── duckduckgo_engine.py    # DuckDuckGo adapter
│   ├── google_serp_engine.py   # Google SERP adapter
│   ├── tavily_engine.py        # Tavily research adapter
│   ├── wikipedia_engine.py     # Wikipedia adapter
│   ├── news_api_engine.py      # NewsAPI adapter
│   └── hacker_news_engine.py   # HackerNews adapter
├── deprecated/                  # Deprecated standalone tools
│   ├── google_trends/          # Google Trends (not integrated)
│   └── seeking_alpha/          # Seeking Alpha (not integrated)
├── duck_duck_go/               # Legacy tool (used by engine)
├── google_serp/                # Legacy tool (used by engine)
├── hacker_news/                # Legacy tool (used by engine)
├── news_api/                   # Legacy tool (used by engine)
├── tavily_research/            # Legacy tool (used by engine)
├── wikipedia/                  # Legacy tool (used by engine)
├── MIGRATION_GUIDE.md          # Migration guide
└── README.md                   # This file
```

## Configuration

### API Keys Setup

Some search engines require API keys for full functionality:

```bash
# Required for Google Search, News, Flights, Events
export SERPAPI_API_KEY="your_serpapi_key_here"

# Required for advanced research searches
export TAVILI_API_KEY="your_tavily_key_here"

# Required for news-specific searches
export NEWSAPI_API_KEY="your_newsapi_key_here"
```

**Note**: DuckDuckGo, Wikipedia, and Hacker News work without API keys.

### Configuration Validation

```python
from agent_c_tools.tools.web_search import WebSearchTools

search = WebSearchTools()

# Check configuration status
status = await search.get_engine_info()
print(status)

# Get configuration summary
summary = search.get_configuration_summary()
print(summary)
```

### Environment Setup

1. Copy `.env.example` to `.env` in the web_search directory
2. Configure your API keys
3. Run the validation script:

```bash
python src/agent_c_tools/src/agent_c_tools/tools/web_search/validate_config.py
```

## Quick Start

### Using the Unified Interface (Recommended)

```python
from agent_c_tools.tools.web_search import WebSearchTools

# Create the unified search tool
search = WebSearchTools()

# General web search with automatic engine selection
results = await search.web_search(query="machine learning")

# Specialized searches
news = await search.news_search(query="AI breakthroughs")
research = await search.research_search(query="quantum computing")
tech = await search.tech_search(query="Python frameworks")
```

## Complete Method Reference

### 1. web_search() - General Web Search

The primary search method with intelligent engine routing and comprehensive parameter support.

**Parameters:**
- `query` (str, required): Search query string
- `engine` (str, optional): Specific engine to use ("duckduckgo", "google_serp", "tavily", "wikipedia", "newsapi", "hackernews")
- `search_type` (str, optional): Type of search ("web", "news", "research", "educational", "tech", "flights", "events")
- `max_results` (int, optional): Maximum number of results (1-100, default varies by engine)
- `safe_search` (str, optional): Safe search level ("off", "moderate", "strict")
- `include_domains` (list, optional): Domains to include in search
- `exclude_domains` (list, optional): Domains to exclude from search
- `start_date` (str, optional): Start date for time-filtered search (YYYY-MM-DD)
- `end_date` (str, optional): End date for time-filtered search (YYYY-MM-DD)
- `language` (str, optional): Language code ("en", "es", "fr", etc.)
- `region` (str, optional): Region code ("us", "uk", "ca", etc.)
- `search_depth` (str, optional): Search depth for research ("basic", "advanced")
- `include_images` (bool, optional): Include images in results
- `include_setup_instructions` (bool, optional): Include setup instructions in response

**Example:**
```python
results = await search.web_search(
    query="artificial intelligence ethics",
    search_type="research",
    max_results=20,
    include_domains=["arxiv.org", "nature.com"],
    exclude_domains=["wikipedia.org"],
    start_date="2023-01-01",
    language="en",
    region="us"
)
```

**Response Format:**
```json
{
  "query": "artificial intelligence ethics",
  "engine_used": "tavily",
  "search_type": "research",
  "total_results": 15,
  "results": [
    {
      "title": "AI Ethics Guidelines",
      "url": "https://example.com/ai-ethics",
      "snippet": "Comprehensive guide to AI ethics...",
      "domain": "example.com",
      "published_date": "2023-06-15",
      "relevance_score": 0.95,
      "content_type": "article",
      "language": "en"
    }
  ],
  "metadata": {
    "search_time": "2023-07-15T10:30:00Z",
    "engine_health": "healthy",
    "fallback_used": false
  }
}
```

### 2. news_search() - News-Optimized Search

Specialized search for news articles with date filtering and news-specific engines.

**Parameters:**
- `query` (str, required): News search query
- `engine` (str, optional): Specific engine ("newsapi", "google_serp", "duckduckgo")
- `max_results` (int, optional): Maximum results (1-100, default 20)
- `start_date` (str, optional): Start date for news (YYYY-MM-DD)
- `end_date` (str, optional): End date for news (YYYY-MM-DD)
- `language` (str, optional): Language code
- `region` (str, optional): Region code
- `sort_by` (str, optional): Sort order ("relevance", "publishedAt", "popularity")

**Example:**
```python
news = await search.news_search(
    query="climate change summit",
    start_date="2023-07-01",
    end_date="2023-07-15",
    language="en",
    sort_by="publishedAt",
    max_results=25
)
```

### 3. educational_search() - Academic Content Search

Search for educational and academic content, optimized for Wikipedia and educational sources.

**Parameters:**
- `query` (str, required): Educational search query
- `engine` (str, optional): Specific engine ("wikipedia", "tavily", "google_serp")
- `max_results` (int, optional): Maximum results (1-50, default 15)
- `language` (str, optional): Language code
- `search_depth` (str, optional): Search depth ("basic", "advanced")

**Example:**
```python
educational = await search.educational_search(
    query="quantum mechanics principles",
    engine="wikipedia",
    language="en",
    search_depth="advanced"
)
```

### 4. research_search() - Deep Research Search

Advanced search with content analysis and detailed information extraction.

**Parameters:**
- `query` (str, required): Research query
- `engine` (str, optional): Specific engine ("tavily", "google_serp", "duckduckgo")
- `max_results` (int, optional): Maximum results (1-20, default 10)
- `include_domains` (list, optional): Preferred domains for research
- `exclude_domains` (list, optional): Domains to exclude
- `search_depth` (str, optional): Search depth ("basic", "advanced")
- `include_images` (bool, optional): Include images in results

**Example:**
```python
research = await search.research_search(
    query="machine learning bias detection",
    include_domains=["arxiv.org", "acm.org", "ieee.org"],
    search_depth="advanced",
    max_results=15
)
```

### 5. tech_search() - Technology Community Search

Search for technology and programming content from community sources.

**Parameters:**
- `query` (str, required): Technology search query
- `engine` (str, optional): Specific engine ("hackernews", "google_serp", "duckduckgo")
- `max_results` (int, optional): Maximum results (1-50, default 20)
- `search_type` (str, optional): Content type ("stories", "jobs", "all")

**Example:**
```python
tech = await search.tech_search(
    query="Python async programming",
    engine="hackernews",
    search_type="stories",
    max_results=30
)
```

### 6. flights_search() - Flight Search

Search for flights using Google Flights integration.

**Parameters:**
- `query` (str, required): Flight search query (e.g., "flights from NYC to LAX")
- `departure_date` (str, optional): Departure date (YYYY-MM-DD)
- `return_date` (str, optional): Return date (YYYY-MM-DD)
- `passengers` (int, optional): Number of passengers (default 1)
- `class_type` (str, optional): Flight class ("economy", "business", "first")
- `max_results` (int, optional): Maximum results (1-50, default 20)

**Example:**
```python
flights = await search.flights_search(
    query="flights from New York to Los Angeles",
    departure_date="2023-08-15",
    return_date="2023-08-22",
    passengers=2,
    class_type="economy"
)
```

### 7. events_search() - Event Search

Search for events using Google Events integration.

**Parameters:**
- `query` (str, required): Event search query
- `location` (str, optional): Location for events
- `start_date` (str, optional): Start date for events (YYYY-MM-DD)
- `end_date` (str, optional): End date for events (YYYY-MM-DD)
- `max_results` (int, optional): Maximum results (1-50, default 20)

**Example:**
```python
events = await search.events_search(
    query="tech conferences",
    location="San Francisco",
    start_date="2023-08-01",
    end_date="2023-08-31"
)
```

### 8. get_engine_info() - Engine Status and Capabilities

Get detailed information about available engines, their status, and capabilities.

**Parameters:**
- `include_setup_instructions` (bool, optional): Include setup instructions for unconfigured engines
- `check_health` (bool, optional): Perform real-time health checks (default true)

**Example:**
```python
info = await search.get_engine_info(
    include_setup_instructions=True,
    check_health=True
)
```

**Response Format:**
```json
{
  "available_engines": [
    {
      "name": "duckduckgo",
      "status": "available",
      "requires_api_key": false,
      "capabilities": ["web_search", "safe_search"],
      "max_results": 50,
      "health_status": "healthy"
    }
  ],
  "configuration_status": {
    "total_engines": 6,
    "available_engines": 4,
    "configured_engines": 3,
    "missing_api_keys": ["NEWSAPI_API_KEY"]
  },
  "setup_instructions": {
    "NEWSAPI_API_KEY": "Get your API key from https://newsapi.org/"
  }
}
```

## Search Engine Capabilities

### Engine Comparison Matrix

| Engine | API Key Required | Max Results | Capabilities | Best For |
|--------|------------------|-------------|--------------|----------|
| **DuckDuckGo** | No | 50 | Web search, safe search | General web search, privacy-focused |
| **Google SERP** | Yes (SERPAPI_API_KEY) | 100 | Web, news, flights, events | Comprehensive search, current events |
| **Tavily** | Yes (TAVILI_API_KEY) | 20 | Research, content analysis | Academic research, detailed analysis |
| **Wikipedia** | No | 50 | Educational content | Educational content, reference material |
| **NewsAPI** | Yes (NEWSAPI_API_KEY) | 100 | News articles, date filtering | News search, current events |
| **HackerNews** | No | 50 | Tech stories, job postings | Technology community, programming |

### Engine-Specific Examples

#### DuckDuckGo Engine
```python
# Privacy-focused web search
results = await search.web_search(
    query="data privacy laws",
    engine="duckduckgo",
    safe_search="strict",
    max_results=30
)
```

#### Google SERP Engine
```python
# Comprehensive search with multiple types
web_results = await search.web_search(
    query="artificial intelligence trends",
    engine="google_serp",
    max_results=50
)

news_results = await search.news_search(
    query="AI breakthrough",
    engine="google_serp",
    start_date="2023-07-01"
)

flight_results = await search.flights_search(
    query="flights from NYC to LAX",
    departure_date="2023-08-15"
)
```

#### Tavily Engine
```python
# Research-focused search with content analysis
research_results = await search.research_search(
    query="climate change impact on agriculture",
    engine="tavily",
    include_domains=["nature.com", "science.org"],
    search_depth="advanced"
)
```

#### Wikipedia Engine
```python
# Educational content search
educational_results = await search.educational_search(
    query="photosynthesis process",
    engine="wikipedia",
    language="en"
)
```

#### NewsAPI Engine
```python
# News-specific search with advanced filtering
news_results = await search.news_search(
    query="renewable energy",
    engine="newsapi",
    start_date="2023-07-01",
    end_date="2023-07-15",
    sort_by="publishedAt",
    language="en"
)
```

#### HackerNews Engine
```python
# Technology community search
tech_results = await search.tech_search(
    query="rust programming language",
    engine="hackernews",
    search_type="stories",
    max_results=25
)

job_results = await search.tech_search(
    query="python developer",
    engine="hackernews",
    search_type="jobs"
)
```

## Best Practices for Engine Selection

### Automatic Engine Selection (Recommended)

The system automatically selects the best engine based on:
- **Query Analysis**: Content type detection (financial, technical, academic, etc.)
- **Search Type**: Optimization for news, research, educational content
- **Engine Availability**: Health status and API key configuration
- **Performance**: Response time and reliability metrics

```python
# Let the system choose the best engine
results = await search.web_search(query="climate change impacts")
```

### When to Use Specific Engines

#### Use DuckDuckGo when:
- Privacy is a concern
- No API keys are available
- General web search is needed
- Safe search filtering is important

#### Use Google SERP when:
- Comprehensive search results are needed
- Current events and news are important
- Flight or event search is required
- Maximum result count is needed

#### Use Tavily when:
- Academic research is the focus
- Content analysis and extraction is needed
- Domain-specific research is required
- Detailed, authoritative sources are preferred

#### Use Wikipedia when:
- Educational content is needed
- Reference material is required
- Factual, encyclopedic information is desired
- No API keys are available

#### Use NewsAPI when:
- News-specific search is required
- Date filtering is important
- Sorting by publication date is needed
- News source diversity is important

#### Use HackerNews when:
- Technology community insights are needed
- Programming-related content is required
- Job postings in tech are desired
- Community discussions are valuable

### Query Optimization Tips

#### For Research Queries:
```python
# Use specific, academic terms
results = await search.research_search(
    query="machine learning bias mitigation techniques",
    include_domains=["arxiv.org", "acm.org", "ieee.org"],
    search_depth="advanced"
)
```

#### For News Queries:
```python
# Include time context and specific terms
results = await search.news_search(
    query="artificial intelligence regulation 2023",
    start_date="2023-01-01",
    sort_by="publishedAt"
)
```

#### For Technical Queries:
```python
# Use specific technical terms and frameworks
results = await search.tech_search(
    query="FastAPI async database connection pooling",
    engine="hackernews"
)
```

### Advanced Parameter Combinations

#### Domain-Filtered Research:
```python
results = await search.research_search(
    query="quantum computing algorithms",
    include_domains=["arxiv.org", "nature.com", "science.org"],
    exclude_domains=["wikipedia.org", "reddit.com"],
    search_depth="advanced",
    max_results=15
)
```

#### Time-Sensitive News Search:
```python
results = await search.news_search(
    query="cryptocurrency market analysis",
    start_date="2023-07-01",
    end_date="2023-07-15",
    sort_by="publishedAt",
    language="en",
    region="us"
)
```

#### Multi-Language Educational Search:
```python
results = await search.educational_search(
    query="histoire de la révolution française",
    language="fr",
    engine="wikipedia"
)
```

## Architecture

### Core Components

- **WebSearchTools**: Main unified interface
- **Engine Registry**: Manages available search engines
- **Engine Router**: Intelligent routing based on query analysis
- **Parameter Validator**: Validates and normalizes parameters
- **Error Handler**: Comprehensive error handling with fallbacks

### Engine Adapters

Each search provider has an adapter that:
- Implements the `BaseWebSearchEngine` interface
- Wraps the legacy tool implementation
- Standardizes responses to a common format
- Provides health monitoring and availability checking

### Intelligent Routing

The system automatically selects the best engine based on:
- Query content analysis (financial, technical, academic, etc.)
- Search type preferences
- Engine availability and health
- User preferences (explicit engine selection)

## Legacy Tools

Legacy individual tools are deprecated but still available for backward compatibility:

- `DuckDuckGoTools` → Use `WebSearchTools.web_search(engine="duckduckgo")`
- `GoogleSerpTools` → Use `WebSearchTools.web_search(engine="google_serp")`
- `NewsApiTools` → Use `WebSearchTools.news_search(engine="newsapi")`
- `TavilyResearchTools` → Use `WebSearchTools.research_search(engine="tavily")`
- `WikipediaTools` → Use `WebSearchTools.educational_search(engine="wikipedia")`
- `HackerNewsTools` → Use `WebSearchTools.tech_search(engine="hackernews")`

## Migration

See `MIGRATION_GUIDE.md` for detailed migration instructions from legacy tools to the unified interface.

## Benefits

- **Unified Interface**: Single tool for all search needs
- **Intelligent Routing**: Automatic engine selection
- **Standardized Responses**: Consistent JSON format
- **Enhanced Features**: Domain filtering, date ranges, etc.
- **Better Error Handling**: Comprehensive error management
- **Type Safety**: Full parameter validation
- **Extensible**: Easy to add new engines

## Comprehensive Troubleshooting Guide

### Configuration Issues

#### 1. "Engine not available" Errors

**Symptoms:**
- Error message: "Engine [name] is not available"
- Search fails with configuration error
- Engine shows as "unavailable" in `get_engine_info()`

**Solutions:**
```python
# Check engine status
info = await search.get_engine_info(check_health=True)
print(info['available_engines'])

# Check specific engine configuration
for engine in info['available_engines']:
    if engine['name'] == 'google_serp':
        print(f"Status: {engine['status']}")
        print(f"Requires API key: {engine['requires_api_key']}")
        print(f"Health: {engine['health_status']}")
```

**Common Causes:**
- Missing API key environment variable
- Incorrect API key format
- API quota exceeded
- Network connectivity issues
- Service temporarily unavailable

#### 2. API Key Configuration Problems

**Symptoms:**
- "Configuration Error" messages
- "API key not found" errors
- Engine shows as "configured" but fails to work

**Diagnostic Steps:**
```python
# Get configuration summary
summary = search.get_configuration_summary()
print(summary)

# Check environment variables
import os
print(f"SERPAPI_API_KEY: {'SET' if os.getenv('SERPAPI_API_KEY') else 'NOT SET'}")
print(f"TAVILI_API_KEY: {'SET' if os.getenv('TAVILI_API_KEY') else 'NOT SET'}")
print(f"NEWSAPI_API_KEY: {'SET' if os.getenv('NEWSAPI_API_KEY') else 'NOT SET'}")

# Validate configuration
from agent_c_tools.tools.web_search.validate_config import validate_web_search_configuration
validation_result = validate_web_search_configuration()
print(validation_result)
```

**Solutions:**
- Ensure environment variables are set correctly (case-sensitive)
- Verify API keys are not empty or contain only whitespace
- Check API key format matches provider requirements
- Restart application after setting environment variables

#### 3. API Quota and Rate Limiting

**Symptoms:**
- "Quota exceeded" errors
- "Rate limit exceeded" messages
- Intermittent failures during high usage

**Solutions:**
```python
# Check engine health and quota status
info = await search.get_engine_info()
for engine in info['available_engines']:
    if engine['health_status'] == 'quota_exceeded':
        print(f"Engine {engine['name']} has exceeded quota")

# Use fallback engines
results = await search.web_search(
    query="your query",
    engine="duckduckgo"  # No API key required
)
```

**Prevention:**
- Monitor API usage on provider dashboards
- Implement caching for repeated queries
- Use free engines (DuckDuckGo, Wikipedia, HackerNews) as fallbacks
- Consider upgrading to paid plans for higher limits

### Search Quality Issues

#### 1. Poor or Irrelevant Results

**Solutions:**
```python
# Use more specific queries
results = await search.web_search(
    query="machine learning bias detection algorithms 2023",
    search_type="research"
)

# Filter by domain for quality sources
results = await search.research_search(
    query="climate change impacts",
    include_domains=["nature.com", "science.org", "arxiv.org"]
)

# Use appropriate search type
results = await search.educational_search(
    query="photosynthesis process",  # Better for educational content
    engine="wikipedia"
)
```

#### 2. Outdated Results

**Solutions:**
```python
# Use date filtering for recent content
results = await search.news_search(
    query="AI developments",
    start_date="2023-07-01",
    sort_by="publishedAt"
)

# Use engines that prioritize fresh content
results = await search.web_search(
    query="latest tech trends",
    engine="google_serp"  # Better for current content
)
```

#### 3. Insufficient Results

**Solutions:**
```python
# Increase max_results
results = await search.web_search(
    query="your query",
    max_results=50  # Increase from default
)

# Use broader query terms
results = await search.web_search(
    query="AI"  # Instead of "artificial intelligence machine learning deep learning"
)

# Try different engines
results = await search.web_search(
    query="your query",
    engine="google_serp"  # Often has more results
)
```

### Performance Issues

#### 1. Slow Response Times

**Diagnostic:**
```python
import time

start_time = time.time()
results = await search.web_search(query="test query")
end_time = time.time()

print(f"Search took {end_time - start_time:.2f} seconds")
```

**Solutions:**
- Reduce `max_results` for faster responses
- Use faster engines (DuckDuckGo, Wikipedia) for simple queries
- Implement caching for repeated queries
- Check network connectivity

#### 2. Timeout Errors

**Solutions:**
```python
# Use engines with better reliability
results = await search.web_search(
    query="your query",
    engine="duckduckgo"  # Generally more reliable
)

# Reduce complexity of search
results = await search.web_search(
    query="simplified query",
    max_results=10  # Reduce result count
)
```

### Advanced Debugging

#### Enable Detailed Logging
```python
import logging

# Enable debug logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger('agent_c_tools.tools.web_search')
logger.setLevel(logging.DEBUG)

# Now run your search
results = await search.web_search(query="test")
```

#### Configuration Validation Script
```python
# Run the validation script
python src/agent_c_tools/src/agent_c_tools/tools/web_search/validate_config.py
```

#### Test Individual Engines
```python
# Test each engine individually
engines = ['duckduckgo', 'google_serp', 'tavily', 'wikipedia', 'newsapi', 'hackernews']

for engine in engines:
    try:
        results = await search.web_search(
            query="test",
            engine=engine,
            max_results=5
        )
        print(f"✅ {engine}: Working")
    except Exception as e:
        print(f"❌ {engine}: {str(e)}")
```

### API Key Setup and Management

#### SerpAPI (Google Search)
- **URL**: https://serpapi.com/dashboard
- **Free Tier**: 100 searches/month
- **Environment Variable**: `SERPAPI_API_KEY`
- **Format**: String key from dashboard

#### Tavily Research
- **URL**: https://tavily.com/
- **Free Tier**: Check current limits
- **Environment Variable**: `TAVILI_API_KEY`
- **Format**: API key from account settings

#### NewsAPI
- **URL**: https://newsapi.org/
- **Free Tier**: 1000 requests/day
- **Environment Variable**: `NEWSAPI_API_KEY`
- **Format**: API key from account dashboard

#### Environment Variable Setup
```bash
# Add to your .env file or environment
export SERPAPI_API_KEY="your_serpapi_key_here"
export TAVILI_API_KEY="your_tavily_key_here"
export NEWSAPI_API_KEY="your_newsapi_key_here"
```

### Getting Help

1. **Check Configuration**: Run `get_engine_info()` first
2. **Validate Setup**: Use the validation script
3. **Test Individual Engines**: Isolate the problem
4. **Check Logs**: Enable debug logging
5. **Review Documentation**: Ensure correct parameter usage
6. **Check API Dashboards**: Verify quota and key status

## Response Format Reference

### Standard Response Structure

All search methods return JSON strings with the following standardized structure:

```json
{
  "query": "original search query",
  "engine_used": "engine_name",
  "search_type": "web|news|research|educational|tech|flights|events",
  "total_results": 15,
  "results": [
    {
      "title": "Result Title",
      "url": "https://example.com/page",
      "snippet": "Brief description of the content...",
      "domain": "example.com",
      "published_date": "2023-07-15T10:30:00Z",
      "relevance_score": 0.95,
      "content_type": "article|video|image|document",
      "language": "en",
      "region": "us",
      "metadata": {
        "author": "Author Name",
        "word_count": 1500,
        "reading_time": "5 min",
        "tags": ["technology", "AI"]
      }
    }
  ],
  "metadata": {
    "search_time": "2023-07-15T10:30:00Z",
    "response_time_ms": 1250,
    "engine_health": "healthy|degraded|unhealthy",
    "fallback_used": false,
    "cache_hit": false,
    "total_available_results": 1000,
    "search_parameters": {
      "max_results": 20,
      "safe_search": "moderate",
      "language": "en",
      "region": "us"
    }
  },
  "warnings": [
    "API quota approaching limit",
    "Some results filtered by safe search"
  ],
  "setup_instructions": {
    "missing_engines": {
      "newsapi": "Get API key from https://newsapi.org/"
    }
  }
}
```

### Engine-Specific Response Variations

#### News Search Response
```json
{
  "results": [
    {
      "title": "News Article Title",
      "url": "https://news.example.com/article",
      "snippet": "Article summary...",
      "published_date": "2023-07-15T10:30:00Z",
      "source": "News Source Name",
      "author": "Reporter Name",
      "category": "technology",
      "sentiment": "neutral",
      "image_url": "https://example.com/image.jpg"
    }
  ]
}
```

#### Research Search Response
```json
{
  "results": [
    {
      "title": "Research Paper Title",
      "url": "https://arxiv.org/abs/2307.12345",
      "snippet": "Abstract or summary...",
      "authors": ["Author 1", "Author 2"],
      "publication_date": "2023-07-15",
      "journal": "Journal Name",
      "citations": 42,
      "doi": "10.1000/journal.2023.12345",
      "content_analysis": {
        "key_concepts": ["machine learning", "neural networks"],
        "methodology": "experimental",
        "confidence_score": 0.92
      }
    }
  ]
}
```

#### Flight Search Response
```json
{
  "results": [
    {
      "title": "Flight Option",
      "airline": "Airline Name",
      "departure_time": "2023-08-15T08:00:00Z",
      "arrival_time": "2023-08-15T11:30:00Z",
      "duration": "5h 30m",
      "price": "$299",
      "stops": 0,
      "aircraft": "Boeing 737",
      "departure_airport": "JFK",
      "arrival_airport": "LAX"
    }
  ]
}
```

### Error Response Format

```json
{
  "error": true,
  "error_type": "configuration_error|api_error|validation_error|network_error",
  "error_message": "Detailed error description",
  "error_code": "ERR_001",
  "suggestions": [
    "Check API key configuration",
    "Verify network connectivity",
    "Try a different engine"
  ],
  "fallback_available": true,
  "fallback_engines": ["duckduckgo", "wikipedia"],
  "setup_instructions": {
    "required_action": "Set SERPAPI_API_KEY environment variable",
    "help_url": "https://serpapi.com/dashboard"
  }
}
```

## Integration Examples

### Agent Integration

```python
from agent_c_tools.tools.web_search import WebSearchTools

class ResearchAgent:
    def __init__(self):
        self.search = WebSearchTools()
    
    async def research_topic(self, topic: str):
        """Comprehensive research on a topic"""
        
        # Start with general research
        research_results = await self.search.research_search(
            query=topic,
            search_depth="advanced",
            max_results=15
        )
        
        # Get recent news
        news_results = await self.search.news_search(
            query=topic,
            start_date="2023-07-01",
            max_results=10
        )
        
        # Get educational background
        educational_results = await self.search.educational_search(
            query=topic,
            engine="wikipedia",
            max_results=5
        )
        
        return {
            "research": research_results,
            "news": news_results,
            "educational": educational_results
        }
```

### Batch Processing

```python
import asyncio

async def batch_search(queries: list[str]):
    """Process multiple queries concurrently"""
    search = WebSearchTools()
    
    tasks = [
        search.web_search(query=query, max_results=10)
        for query in queries
    ]
    
    results = await asyncio.gather(*tasks, return_exceptions=True)
    
    return {
        query: result for query, result in zip(queries, results)
        if not isinstance(result, Exception)
    }
```

### Configuration Management

```python
class SearchManager:
    def __init__(self):
        self.search = WebSearchTools()
        self.engine_preferences = {
            "news": ["newsapi", "google_serp"],
            "research": ["tavily", "google_serp"],
            "general": ["google_serp", "duckduckgo"]
        }
    
    async def smart_search(self, query: str, search_type: str = "general"):
        """Search with intelligent engine selection"""
        
        # Get engine status
        engine_info = await self.search.get_engine_info()
        available_engines = {
            engine['name']: engine['status'] == 'available'
            for engine in engine_info['available_engines']
        }
        
        # Select best available engine
        preferred_engines = self.engine_preferences.get(search_type, ["duckduckgo"])
        selected_engine = None
        
        for engine in preferred_engines:
            if available_engines.get(engine, False):
                selected_engine = engine
                break
        
        # Perform search
        if search_type == "news":
            return await self.search.news_search(
                query=query,
                engine=selected_engine
            )
        elif search_type == "research":
            return await self.search.research_search(
                query=query,
                engine=selected_engine
            )
        else:
            return await self.search.web_search(
                query=query,
                engine=selected_engine
            )
```

## Performance Optimization

### Caching Strategy

```python
import hashlib
import json
from datetime import datetime, timedelta

class CachedWebSearch:
    def __init__(self, cache_ttl_minutes: int = 60):
        self.search = WebSearchTools()
        self.cache = {}
        self.cache_ttl = timedelta(minutes=cache_ttl_minutes)
    
    def _cache_key(self, method: str, **kwargs) -> str:
        """Generate cache key from method and parameters"""
        cache_data = {"method": method, "params": kwargs}
        cache_string = json.dumps(cache_data, sort_keys=True)
        return hashlib.md5(cache_string.encode()).hexdigest()
    
    def _is_cache_valid(self, timestamp: datetime) -> bool:
        """Check if cache entry is still valid"""
        return datetime.now() - timestamp < self.cache_ttl
    
    async def web_search(self, **kwargs):
        """Cached web search"""
        cache_key = self._cache_key("web_search", **kwargs)
        
        if cache_key in self.cache:
            result, timestamp = self.cache[cache_key]
            if self._is_cache_valid(timestamp):
                return result
        
        # Perform search and cache result
        result = await self.search.web_search(**kwargs)
        self.cache[cache_key] = (result, datetime.now())
        
        return result
```

### Concurrent Search Pattern

```python
async def comprehensive_search(topic: str):
    """Perform multiple search types concurrently"""
    search = WebSearchTools()
    
    # Define search tasks
    tasks = {
        "web": search.web_search(query=topic, max_results=20),
        "news": search.news_search(query=topic, max_results=15),
        "research": search.research_search(query=topic, max_results=10),
        "educational": search.educational_search(query=topic, max_results=10)
    }
    
    # Execute concurrently
    results = {}
    for search_type, task in tasks.items():
        try:
            results[search_type] = await task
        except Exception as e:
            results[search_type] = {"error": str(e)}
    
    return results
```

## Future Enhancements

### Planned Features
- **Google Trends Integration**: Trending topics and search volume data
- **Seeking Alpha Integration**: Financial news and analysis
- **Additional Search Engines**: Bing, Yandex, Baidu support
- **Advanced Caching**: Redis-based distributed caching
- **Query Analytics**: Search pattern analysis and optimization
- **Result Ranking**: Custom relevance scoring algorithms
- **Content Extraction**: Full-text content retrieval and analysis
- **Multilingual Support**: Enhanced language detection and translation

### Architecture Improvements
- **Async Engine Pool**: Connection pooling for better performance
- **Circuit Breaker Pattern**: Enhanced fault tolerance
- **Metrics Collection**: Detailed performance and usage metrics
- **A/B Testing Framework**: Engine performance comparison
- **Rate Limiting**: Built-in rate limiting for API protection
- **Result Deduplication**: Intelligent duplicate removal across engines

### API Enhancements
- **Streaming Results**: Real-time result streaming for large queries
- **Webhook Support**: Async result delivery via webhooks
- **GraphQL Interface**: Flexible query interface
- **Batch API**: Efficient bulk search processing
- **Search Templates**: Predefined search configurations
- **Custom Scoring**: User-defined relevance scoring

This comprehensive documentation provides everything needed to effectively use the WebSearchTools system, from basic usage to advanced integration patterns and troubleshooting.