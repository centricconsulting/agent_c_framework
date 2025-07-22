# Web Search Tools Inventory and Analysis

## Executive Summary

This document provides a comprehensive analysis of the 8 existing web search tools in the Agent C Framework. The analysis covers functionality, API requirements, parameters, dependencies, and unique capabilities that must be preserved in the unified web search tool design.

## Tool Overview

| Tool Name | Status | Primary Function | API Key Required | Underlying Library |
|-----------|--------|------------------|------------------|-------------------|
| DuckDuckGoTools | Inactive* | General web search | No | duckduckgo_search |
| GoogleSerpTools | Active | Multi-modal Google search | Yes (SERPAPI_API_KEY) | serpapi |
| GoogleTrendsTools | Active | Google Trends analysis | No | pytrends |
| HackerNewsTools | Active | Hacker News content | No | requests |
| NewsApiTools | Active | News aggregation | Yes (NEWSAPI_API_KEY) | newsapi |
| SeekingAlphaTools | Active | Financial news | No | aiohttp, BeautifulSoup |
| TavilyResearchTools | Active | Advanced web research | Yes (TAVILI_API_KEY) | tavily |
| WikipediaTools | Active | Wikipedia search | No | wikipedia |

*DuckDuckGoTools is commented out in __init__.py but code exists

## Detailed Tool Analysis

### 1. DuckDuckGoTools

**Status**: Inactive (commented out in imports)
**Core Method**: `web_search`
**Parameters**:
- `query` (string, required): Search query
- `max_results` (integer, optional, default=20): Maximum results to return
- `safesearch` (enum, optional, default='moderate'): Safe search level ('on', 'moderate', 'off')

**Dependencies**: 
- `duckduckgo_search` library
- No API key required

**Unique Capabilities**:
- Free, anonymous web search
- SafeSearch controls
- Simple, straightforward interface

**Implementation Notes**:
- Uses DDGS().text() method
- Returns JSON-formatted results
- Currently disabled, possibly due to reliability issues

### 2. GoogleSerpTools

**Status**: Active
**Core Methods**: 
- `get_flights`
- `get_events`  
- `get_news`
- `get_search_results`

**API Requirements**:
- SERPAPI_API_KEY environment variable required
- Uses Google-Search-Results library (not SERPAPI library)

**Parameters by Method**:

**get_flights**:
- `departure_id` (string, required): Departure airport ID
- `arrival_id` (string, required): Arrival airport ID
- `outbound_date` (string, required): Departure date
- `return_date` (string, optional): Return date
- `currency` (string, optional, default='USD'): Currency
- `flight_type` (string, optional, default='2'): Flight type (1=RoundTrip, 2=OneWay)
- `search_locale` (string, optional, default='us'): Search locale

**get_events**:
- `query` (string, required): Event search query
- `location` (string, optional): Event location
- `max_return` (integer, optional, default=10): Maximum events to return

**get_news**:
- `query` (string, required): News search query
- `max_return` (integer, optional, default=10): Maximum news items to return

**get_search_results**:
- `query` (string, required): Search query
- `max_return` (integer, optional, default=10): Maximum results to return

**Dependencies**:
- `serpapi` library
- SERPAPI_API_KEY

**Unique Capabilities**:
- Multi-modal search (flights, events, news, general web)
- Google's comprehensive search capabilities
- Flight-specific search with detailed parameters
- Event discovery with location filtering
- News search with Google News integration

### 3. GoogleTrendsTools

**Status**: Active
**Core Methods**:
- `get_google_trending_searches`
- `get_google_trends_for_query`

**API Requirements**: No API key required

**Parameters**:

**get_google_trending_searches**:
- `region` (string, optional, default='united_states'): Region for trending searches

**get_google_trends_for_query**:
- `query` (string, required): Query to search trends for
- `timeframe` (string, optional, default='past_month'): Time period for trends
- `region` (string, optional, default=''): Region filter
- `find_related` (boolean, optional, default=False): Find related queries (currently disabled)

**Dependencies**:
- `pytrends` library
- `pandas` for data processing
- Built-in caching system

**Unique Capabilities**:
- Time-based trend analysis
- Regional filtering
- Predefined timeframe options (past_hour, past_day, past_week, etc.)
- Data caching with parquet format
- Related queries analysis (currently disabled due to Google API issues)
- DataFrame-based data processing

**Timeframe Options**:
- past_hour, past_day, past_week, past_month, past_3months, past_year, past_5years, all
- Custom date ranges supported

### 4. HackerNewsTools

**Status**: Active
**Core Methods**:
- `get_top_stories`
- `get_job_stories`

**API Requirements**: No API key required

**Parameters**:
- `limit` (integer, required): Number of stories to display (default=10)

**Dependencies**:
- `requests` library
- Uses Hacker News Firebase API directly

**Unique Capabilities**:
- Access to Hacker News community content
- Separate job stories functionality
- Direct Firebase API integration
- Tech community-focused content

**Implementation Notes**:
- Base URL: https://hacker-news.firebaseio.com/v0/
- Two-step process: fetch story IDs, then fetch story details
- Simple title-only extraction currently

### 5. NewsApiTools

**Status**: Active
**Core Methods**:
- `get_top_headlines`
- `get_sources`
- `get_all_articles`

**API Requirements**:
- NEWSAPI_API_KEY environment variable required

**Parameters**:

**get_top_headlines**:
- `category` (string, optional): Comma-separated categories (business, entertainment, general, health, science, sports, technology)
- `pageSize` (integer, optional, default=20, max=100): Results per page
- `page` (integer, optional, default=1): Page number

**get_sources**:
- `category` (string, optional): Category filter for sources

**get_all_articles**:
- `q` (string, required): Search query (max 500 characters)
- `start_date` (string, optional): Start date (ISO-8601 format)
- `end_date` (string, optional): End date (ISO-8601 format)
- `max_articles` (integer, optional, default=10): Maximum articles to return
- `pageSize` (integer, optional, default=20, max=100): Results per page
- `page` (integer, optional, default=1): Page number
- `sort` (string, optional, default='relevancy'): Sort order (relevancy, popularity, publishedAt)

**Dependencies**:
- `newsapi-python` library
- NEWSAPI_API_KEY

**Unique Capabilities**:
- Date range filtering
- Category-based news filtering
- Source discovery
- Multiple sorting options
- Comprehensive article metadata
- Pagination support

### 6. SeekingAlphaTools

**Status**: Active
**Core Methods**:
- `get_topk_trending_news`

**API Requirements**: No API key required

**Parameters**:
- `limit` (integer, optional, default=10): Number of news articles to return
- `extract_content` (boolean, optional, default=False): Flag to extract full article content

**Dependencies**:
- `aiohttp` for async HTTP requests
- `BeautifulSoup` for HTML parsing
- Direct web scraping approach

**Unique Capabilities**:
- Financial news specialization
- Content extraction from articles
- Async HTTP client usage
- HTML parsing and text extraction
- Seeking Alpha's curated financial content

**Implementation Notes**:
- Uses Seeking Alpha's API endpoints
- Two-step process: fetch trending list, then extract content if requested
- Error handling for content extraction failures

### 7. TavilyResearchTools

**Status**: Active
**Core Methods**:
- `search_tavily`

**API Requirements**:
- TAVILI_API_KEY environment variable required

**Parameters**:
- `query` (string, required): Search query
- `search_depth` (string, required, default='advanced'): Search depth
- `max_results` (integer, required, default=5): Maximum results to return
- `include_images` (boolean, optional, default=False): Include images in results
- `include_answer` (boolean, optional, default=False): Include answers in results
- `include_raw_content` (boolean, optional, default=True): Include raw content
- `include_domains` (array, optional): Domains to include in search
- `exclude_domains` (array, optional): Domains to exclude from search

**Dependencies**:
- `tavily` library
- TAVILI_API_KEY
- Custom prompt section for guidance

**Unique Capabilities**:
- Advanced web research with LLM integration
- Domain filtering (include/exclude)
- Content extraction with relevance scoring
- Image inclusion option
- Answer generation capability
- Raw content access
- Advanced search depth control

**Special Features**:
- Includes prompt guidance for score filtering (>0.75)
- Requires URL source inclusion in responses
- JSON response format with href, body, and score

### 8. WikipediaTools

**Status**: Active
**Core Methods**:
- `search_wiki`

**API Requirements**: No API key required

**Parameters**:
- `search` (string, required): Query to search Wikipedia

**Dependencies**:
- `wikipedia` library
- Custom utility function for key filtering

**Unique Capabilities**:
- Wikipedia content access
- Educational and reference content
- Free, reliable knowledge base
- Multiple language support (through wikipedia library)

**Implementation Notes**:
- Uses filtered results approach
- Falls back to raw results if filtering fails
- Noted for needing refactoring for better search functionality
- Currently limited to basic search functionality

## Common Patterns and Architecture

### Base Class Integration
All tools extend the `Toolset` base class and use:
- `@json_schema` decorators for method definitions
- `super().__init__()` with tool-specific names
- `Toolset.register()` for framework integration

### Parameter Validation
- Type checking through JSON schema
- Required/optional parameter specifications
- Default value definitions
- Custom validation patterns (e.g., category validation in NewsApiTools)

### Error Handling
- Try-catch blocks for API failures
- Logging integration
- Graceful degradation strategies
- User-friendly error messages

### Response Format
- Consistent JSON response format
- Error responses in JSON format
- Metadata inclusion where relevant

## API Dependencies and Requirements

### Required API Keys
1. **SERPAPI_API_KEY** - GoogleSerpTools
2. **NEWSAPI_API_KEY** - NewsApiTools  
3. **TAVILI_API_KEY** - TavilyResearchTools

### No API Key Required
1. **DuckDuckGoTools** - Free service
2. **GoogleTrendsTools** - Free Google service
3. **HackerNewsTools** - Free public API
4. **SeekingAlphaTools** - Web scraping
5. **WikipediaTools** - Free service

### Library Dependencies
- `duckduckgo_search` - DuckDuckGo search
- `serpapi` - Google services via SERP API
- `pytrends` - Google Trends
- `requests` - HTTP requests
- `newsapi-python` - News API client
- `aiohttp` - Async HTTP client
- `BeautifulSoup` - HTML parsing
- `tavily` - Tavily research API
- `wikipedia` - Wikipedia API
- `pandas` - Data processing (Google Trends)

## Unique Capabilities Matrix

| Capability | Tools |
|------------|--------|
| General Web Search | DuckDuckGoTools, GoogleSerpTools, TavilyResearchTools |
| News Search | GoogleSerpTools, NewsApiTools, SeekingAlphaTools |
| Trend Analysis | GoogleTrendsTools |
| Flight Search | GoogleSerpTools |
| Event Search | GoogleSerpTools |
| Financial News | SeekingAlphaTools |
| Tech Community Content | HackerNewsTools |
| Educational Content | WikipediaTools |
| Advanced Research | TavilyResearchTools |
| Content Extraction | SeekingAlphaTools, TavilyResearchTools |
| Date Range Filtering | NewsApiTools, GoogleTrendsTools |
| Domain Filtering | TavilyResearchTools |
| Caching | GoogleTrendsTools |
| Async Operations | SeekingAlphaTools |

## Critical Features to Preserve

### 1. Multi-Modal Search Capabilities
- General web search
- News search with categorization
- Financial news specialization
- Educational content access
- Technical community content
- Flight and event search
- Trend analysis

### 2. Advanced Filtering Options
- Date range filtering
- Category-based filtering
- Domain inclusion/exclusion
- Geographic/regional filtering
- Content type filtering (images, text, etc.)

### 3. Content Processing Features
- HTML parsing and text extraction
- Relevance scoring
- Content caching
- Data format conversion (JSON, DataFrame)

### 4. API Integration Patterns
- Multiple API key management
- Graceful fallback mechanisms
- Rate limiting and error handling
- Async operation support

### 5. Specialized Parameters
- SafeSearch controls
- Timeframe specifications
- Search depth controls
- Pagination support
- Result limiting

## Recommendations for Unified Design

### 1. Architecture Considerations
- Maintain plugin-based architecture for different search providers
- Implement unified interface with provider-specific backends
- Preserve existing parameter structures where possible
- Design for extensibility to add new providers

### 2. API Key Management
- Centralized API key management
- Optional provider configuration
- Clear documentation of requirements
- Graceful degradation when keys are missing

### 3. Response Standardization
- Unified response format across all search types
- Consistent error handling
- Metadata preservation
- Source attribution

### 4. Performance Optimization
- Implement caching strategy (extend GoogleTrendsTools approach)
- Async operation support
- Connection pooling
- Rate limiting

### 5. Feature Preservation
- Maintain all unique capabilities
- Preserve specialized parameters
- Keep domain-specific optimizations
- Ensure backward compatibility

## Next Steps

1. **Design Phase**: Create unified interface specification
2. **Implementation Phase**: Develop modular backend system
3. **Testing Phase**: Ensure all existing functionality is preserved
4. **Migration Phase**: Gradual replacement of individual tools
5. **Documentation Phase**: Update user documentation and examples

This inventory provides the foundation for designing a comprehensive, unified web search tool that preserves all existing capabilities while providing a cleaner, more maintainable architecture.