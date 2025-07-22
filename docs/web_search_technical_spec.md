# Web Search Unified Tool - Technical Specification

## Document Overview

This technical specification provides implementation-ready details for Phase 2 of the web search consolidation project. It defines exact schemas, method signatures, file structures, and implementation steps required to build the unified `WebSearchTools` class.

**Version**: 1.0  
**Date**: 2024-01-15  
**Phase**: 2 (Implementation)  
**Prerequisites**: Phase 1 (Analysis and Architecture) completed

## 1. Complete JSON Schema Definitions

### 1.1 Core Method Schemas

#### web_search Method Schema

```json
{
  "name": "web_search",
  "description": "Perform general web search across multiple engines with intelligent routing",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Search query string",
        "minLength": 1,
        "maxLength": 2048
      },
      "engine": {
        "type": "string",
        "enum": ["auto", "duckduckgo", "google", "tavily"],
        "default": "auto",
        "description": "Search engine to use. 'auto' enables intelligent routing"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 100,
        "default": 10,
        "description": "Maximum number of results to return"
      },
      "safesearch": {
        "type": "string",
        "enum": ["on", "moderate", "off"],
        "default": "moderate",
        "description": "Safe search filtering level"
      },
      "include_images": {
        "type": "boolean",
        "default": false,
        "description": "Include image results when supported by engine"
      },
      "include_domains": {
        "type": "array",
        "items": {"type": "string"},
        "maxItems": 10,
        "description": "List of domains to include in search results"
      },
      "exclude_domains": {
        "type": "array",
        "items": {"type": "string"},
        "maxItems": 10,
        "description": "List of domains to exclude from search results"
      },
      "search_depth": {
        "type": "string",
        "enum": ["basic", "standard", "advanced"],
        "default": "standard",
        "description": "Search depth level for engines that support it"
      },
      "language": {
        "type": "string",
        "pattern": "^[a-z]{2}$",
        "default": "en",
        "description": "Language code (ISO 639-1)"
      },
      "region": {
        "type": "string",
        "pattern": "^[a-z]{2}$",
        "default": "us",
        "description": "Region code (ISO 3166-1 alpha-2)"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

#### news_search Method Schema

```json
{
  "name": "news_search",
  "description": "Search for news articles across multiple news engines",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "News search query",
        "minLength": 1,
        "maxLength": 500
      },
      "engine": {
        "type": "string",
        "enum": ["auto", "newsapi", "google", "seeking_alpha"],
        "default": "auto",
        "description": "News engine to use"
      },
      "category": {
        "type": "string",
        "enum": ["business", "entertainment", "general", "health", "science", "sports", "technology", "financial"],
        "description": "News category filter"
      },
      "start_date": {
        "type": "string",
        "format": "date",
        "description": "Start date for news search (YYYY-MM-DD)"
      },
      "end_date": {
        "type": "string",
        "format": "date",
        "description": "End date for news search (YYYY-MM-DD)"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 100,
        "default": 10,
        "description": "Maximum number of results"
      },
      "sort": {
        "type": "string",
        "enum": ["relevancy", "popularity", "publishedAt"],
        "default": "relevancy",
        "description": "Sort order for results"
      },
      "language": {
        "type": "string",
        "pattern": "^[a-z]{2}$",
        "default": "en",
        "description": "Language code"
      },
      "extract_content": {
        "type": "boolean",
        "default": false,
        "description": "Extract full article content when supported"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

#### trends_search Method Schema

```json
{
  "name": "trends_search",
  "description": "Search for trending topics and trend analysis",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Query for trend analysis (optional for general trending topics)",
        "maxLength": 200
      },
      "engine": {
        "type": "string",
        "enum": ["auto", "google_trends", "hackernews"],
        "default": "auto",
        "description": "Trends engine to use"
      },
      "timeframe": {
        "type": "string",
        "enum": ["past_hour", "past_day", "past_week", "past_month", "past_3months", "past_year", "past_5years", "all"],
        "default": "past_month",
        "description": "Timeframe for trend analysis"
      },
      "region": {
        "type": "string",
        "default": "united_states",
        "description": "Geographic region for trends"
      },
      "category": {
        "type": "string",
        "enum": ["all", "arts", "automotive", "books", "business", "computers", "food", "games", "health", "home", "news", "recreation", "science", "sports", "technology"],
        "default": "all",
        "description": "Category filter for trends"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 50,
        "default": 10,
        "description": "Maximum number of trending topics"
      },
      "find_related": {
        "type": "boolean",
        "default": false,
        "description": "Find related queries and topics"
      }
    },
    "additionalProperties": false
  }
}
```

#### flight_search Method Schema

```json
{
  "name": "flight_search",
  "description": "Search for flights using Google SERP API",
  "parameters": {
    "type": "object",
    "properties": {
      "departure_id": {
        "type": "string",
        "description": "Departure airport ID (IATA code)",
        "pattern": "^[A-Z]{3}$"
      },
      "arrival_id": {
        "type": "string",
        "description": "Arrival airport ID (IATA code)",
        "pattern": "^[A-Z]{3}$"
      },
      "outbound_date": {
        "type": "string",
        "format": "date",
        "description": "Departure date (YYYY-MM-DD)"
      },
      "return_date": {
        "type": "string",
        "format": "date",
        "description": "Return date (YYYY-MM-DD) - optional for one-way"
      },
      "currency": {
        "type": "string",
        "enum": ["USD", "EUR", "GBP", "CAD", "AUD", "JPY"],
        "default": "USD",
        "description": "Currency for flight prices"
      },
      "flight_type": {
        "type": "string",
        "enum": ["1", "2"],
        "default": "2",
        "description": "Flight type: 1=RoundTrip, 2=OneWay"
      },
      "search_locale": {
        "type": "string",
        "pattern": "^[a-z]{2}$",
        "default": "us",
        "description": "Search locale"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 50,
        "default": 10,
        "description": "Maximum flight results"
      }
    },
    "required": ["departure_id", "arrival_id", "outbound_date"],
    "additionalProperties": false
  }
}
```

#### event_search Method Schema

```json
{
  "name": "event_search",
  "description": "Search for events using Google SERP API",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Event search query",
        "minLength": 1,
        "maxLength": 200
      },
      "location": {
        "type": "string",
        "description": "Event location (city, state, country)",
        "maxLength": 100
      },
      "start_date": {
        "type": "string",
        "format": "date",
        "description": "Event start date filter (YYYY-MM-DD)"
      },
      "end_date": {
        "type": "string",
        "format": "date",
        "description": "Event end date filter (YYYY-MM-DD)"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 50,
        "default": 10,
        "description": "Maximum event results"
      },
      "event_type": {
        "type": "string",
        "enum": ["conference", "concert", "sports", "festival", "workshop", "meetup", "all"],
        "default": "all",
        "description": "Type of events to search for"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

#### research_search Method Schema

```json
{
  "name": "research_search",
  "description": "Advanced research search with content extraction using Tavily",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Research query",
        "minLength": 1,
        "maxLength": 1000
      },
      "search_depth": {
        "type": "string",
        "enum": ["basic", "advanced"],
        "default": "advanced",
        "description": "Search depth level"
      },
      "include_answer": {
        "type": "boolean",
        "default": true,
        "description": "Include AI-generated answers"
      },
      "include_raw_content": {
        "type": "boolean",
        "default": true,
        "description": "Include raw content from sources"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 20,
        "default": 5,
        "description": "Maximum research results"
      },
      "min_score": {
        "type": "number",
        "minimum": 0.0,
        "maximum": 1.0,
        "default": 0.75,
        "description": "Minimum relevance score threshold"
      },
      "include_images": {
        "type": "boolean",
        "default": false,
        "description": "Include images in research results"
      },
      "include_domains": {
        "type": "array",
        "items": {"type": "string"},
        "maxItems": 10,
        "description": "Domains to include in research"
      },
      "exclude_domains": {
        "type": "array",
        "items": {"type": "string"},
        "maxItems": 10,
        "description": "Domains to exclude from research"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

#### educational_search Method Schema

```json
{
  "name": "educational_search",
  "description": "Search for educational content using Wikipedia and other educational sources",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Educational search query",
        "minLength": 1,
        "maxLength": 200
      },
      "engine": {
        "type": "string",
        "enum": ["auto", "wikipedia", "google"],
        "default": "auto",
        "description": "Educational content engine"
      },
      "subject": {
        "type": "string",
        "enum": ["science", "mathematics", "history", "literature", "arts", "technology", "medicine", "philosophy", "all"],
        "default": "all",
        "description": "Subject area filter"
      },
      "level": {
        "type": "string",
        "enum": ["elementary", "secondary", "undergraduate", "graduate", "professional", "all"],
        "default": "all",
        "description": "Educational level"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 30,
        "default": 10,
        "description": "Maximum educational results"
      },
      "language": {
        "type": "string",
        "pattern": "^[a-z]{2}$",
        "default": "en",
        "description": "Content language"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

#### tech_community_search Method Schema

```json
{
  "name": "tech_community_search",
  "description": "Search technology community content from Hacker News",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Technology topic search query (optional for general stories)",
        "maxLength": 200
      },
      "story_type": {
        "type": "string",
        "enum": ["top", "new", "job", "ask", "show"],
        "default": "top",
        "description": "Type of Hacker News stories"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 50,
        "default": 10,
        "description": "Maximum story results"
      },
      "time_range": {
        "type": "string",
        "enum": ["hour", "day", "week", "month", "all"],
        "default": "day",
        "description": "Time range for stories"
      },
      "include_comments": {
        "type": "boolean",
        "default": false,
        "description": "Include comment count and metadata"
      }
    },
    "additionalProperties": false
  }
}
```

#### financial_search Method Schema

```json
{
  "name": "financial_search",
  "description": "Search for financial news and information",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Financial search query",
        "minLength": 1,
        "maxLength": 200
      },
      "engine": {
        "type": "string",
        "enum": ["auto", "seeking_alpha", "newsapi"],
        "default": "auto",
        "description": "Financial news engine"
      },
      "content_type": {
        "type": "string",
        "enum": ["news", "analysis", "earnings", "market_data", "all"],
        "default": "all",
        "description": "Type of financial content"
      },
      "extract_content": {
        "type": "boolean",
        "default": false,
        "description": "Extract full article content"
      },
      "max_results": {
        "type": "integer",
        "minimum": 1,
        "maximum": 50,
        "default": 10,
        "description": "Maximum financial results"
      },
      "symbols": {
        "type": "array",
        "items": {
          "type": "string",
          "pattern": "^[A-Z]{1,5}$"
        },
        "maxItems": 10,
        "description": "Stock symbols to filter by"
      },
      "sector": {
        "type": "string",
        "enum": ["technology", "healthcare", "finance", "energy", "consumer", "industrial", "materials", "utilities", "real_estate", "all"],
        "default": "all",
        "description": "Market sector filter"
      }
    },
    "required": ["query"],
    "additionalProperties": false
  }
}
```

### 1.2 Utility Method Schemas

#### get_available_engines Method Schema

```json
{
  "name": "get_available_engines",
  "description": "Get list of available search engines and their status",
  "parameters": {
    "type": "object",
    "properties": {
      "search_type": {
        "type": "string",
        "enum": ["web", "news", "trends", "flights", "events", "research", "educational", "tech_community", "financial", "all"],
        "default": "all",
        "description": "Filter engines by search type capability"
      },
      "include_unavailable": {
        "type": "boolean",
        "default": false,
        "description": "Include engines that are currently unavailable"
      }
    },
    "additionalProperties": false
  }
}
```

#### get_engine_capabilities Method Schema

```json
{
  "name": "get_engine_capabilities",
  "description": "Get capabilities and features of a specific engine",
  "parameters": {
    "type": "object",
    "properties": {
      "engine": {
        "type": "string",
        "enum": ["duckduckgo", "google", "google_trends", "hackernews", "newsapi", "seeking_alpha", "tavily", "wikipedia"],
        "description": "Engine to get capabilities for"
      }
    },
    "required": ["engine"],
    "additionalProperties": false
  }
}
```

## 2. Engine Parameter Values and Routing Logic

### 2.1 Engine Parameter Mapping

#### DuckDuckGo Engine Parameters

```python
DUCKDUCKGO_PARAMS = {
    'max_results': {
        'param_name': 'max_results',
        'type': 'integer',
        'default': 20,
        'min': 1,
        'max': 100
    },
    'safesearch': {
        'param_name': 'safesearch',
        'type': 'string',
        'mapping': {
            'on': 'strict',
            'moderate': 'moderate',
            'off': 'off'
        },
        'default': 'moderate'
    },
    'region': {
        'param_name': 'region',
        'type': 'string',
        'mapping': {
            'us': 'us-en',
            'uk': 'uk-en',
            'ca': 'ca-en',
            'au': 'au-en'
        },
        'default': 'us-en'
    }
}
```

#### Google SERP Engine Parameters

```python
GOOGLE_SERP_PARAMS = {
    'web_search': {
        'engine': 'google',
        'max_results': {
            'param_name': 'num',
            'type': 'integer',
            'default': 10,
            'max': 100
        },
        'language': {
            'param_name': 'lr',
            'type': 'string',
            'mapping': {
                'en': 'lang_en',
                'es': 'lang_es',
                'fr': 'lang_fr',
                'de': 'lang_de'
            }
        },
        'region': {
            'param_name': 'gl',
            'type': 'string',
            'default': 'us'
        }
    },
    'news_search': {
        'engine': 'google_news',
        'max_results': {
            'param_name': 'num',
            'type': 'integer',
            'default': 10,
            'max': 100
        },
        'sort': {
            'param_name': 'sort',
            'type': 'string',
            'mapping': {
                'relevancy': 'relevance',
                'publishedAt': 'date'
            }
        }
    },
    'flight_search': {
        'engine': 'google_flights',
        'departure_id': {'param_name': 'departure_id', 'type': 'string'},
        'arrival_id': {'param_name': 'arrival_id', 'type': 'string'},
        'outbound_date': {'param_name': 'outbound_date', 'type': 'string'},
        'return_date': {'param_name': 'return_date', 'type': 'string'},
        'currency': {'param_name': 'currency', 'type': 'string'},
        'flight_type': {'param_name': 'type', 'type': 'string'}
    },
    'event_search': {
        'engine': 'google_events',
        'max_results': {
            'param_name': 'num',
            'type': 'integer',
            'default': 10
        }
    }
}
```

#### NewsAPI Engine Parameters

```python
NEWSAPI_PARAMS = {
    'category': {
        'param_name': 'category',
        'type': 'string',
        'enum': ['business', 'entertainment', 'general', 'health', 'science', 'sports', 'technology']
    },
    'max_results': {
        'param_name': 'page_size',
        'type': 'integer',
        'default': 20,
        'max': 100
    },
    'language': {
        'param_name': 'language',
        'type': 'string',
        'default': 'en'
    },
    'sort': {
        'param_name': 'sort_by',
        'type': 'string',
        'mapping': {
            'relevancy': 'relevancy',
            'popularity': 'popularity',
            'publishedAt': 'publishedAt'
        }
    },
    'start_date': {
        'param_name': 'from',
        'type': 'string',
        'format': 'date'
    },
    'end_date': {
        'param_name': 'to',
        'type': 'string',
        'format': 'date'
    }
}
```

#### Tavily Research Engine Parameters

```python
TAVILY_PARAMS = {
    'search_depth': {
        'param_name': 'search_depth',
        'type': 'string',
        'mapping': {
            'basic': 'basic',
            'standard': 'advanced',
            'advanced': 'advanced'
        }
    },
    'max_results': {
        'param_name': 'max_results',
        'type': 'integer',
        'default': 5,
        'max': 20
    },
    'include_images': {
        'param_name': 'include_images',
        'type': 'boolean',
        'default': False
    },
    'include_answer': {
        'param_name': 'include_answer',
        'type': 'boolean',
        'default': True
    },
    'include_raw_content': {
        'param_name': 'include_raw_content',
        'type': 'boolean',
        'default': True
    },
    'include_domains': {
        'param_name': 'include_domains',
        'type': 'array'
    },
    'exclude_domains': {
        'param_name': 'exclude_domains',
        'type': 'array'
    }
}
```

### 2.2 Engine Routing Logic Algorithm

#### Primary Routing Algorithm

```python
def route_search_request(search_type: str, engine: str, query: str, params: dict) -> str:
    """
    Route search request to appropriate engine based on search type,
    engine preference, and availability.
    
    Priority Order:
    1. Explicit engine selection (if available)
    2. Search type optimization
    3. Query analysis routing
    4. Availability-based fallback
    """
    
    # Step 1: Handle explicit engine selection
    if engine != "auto":
        if is_engine_available(engine) and supports_search_type(engine, search_type):
            return engine
        else:
            # Fall back to auto-routing if explicit engine unavailable
            log_warning(f"Requested engine '{engine}' not available, falling back to auto-routing")
    
    # Step 2: Search type optimization
    optimized_engine = get_optimized_engine_for_search_type(search_type)
    if optimized_engine and is_engine_available(optimized_engine):
        return optimized_engine
    
    # Step 3: Query analysis routing
    analyzed_engine = analyze_query_for_engine_preference(query, search_type)
    if analyzed_engine and is_engine_available(analyzed_engine):
        return analyzed_engine
    
    # Step 4: Availability-based fallback
    fallback_engine = get_fallback_engine(search_type)
    if fallback_engine and is_engine_available(fallback_engine):
        return fallback_engine
    
    # Step 5: Final fallback to any available engine
    available_engines = get_available_engines_for_search_type(search_type)
    if available_engines:
        return available_engines[0]
    
    # No engines available
    raise EngineUnavailableError(f"No engines available for search type: {search_type}")
```

#### Search Type Optimization Matrix

```python
SEARCH_TYPE_ENGINE_PREFERENCES = {
    'web': ['tavily', 'google', 'duckduckgo'],
    'news': ['newsapi', 'google', 'seeking_alpha'],
    'trends': ['google_trends', 'hackernews'],
    'flights': ['google'],
    'events': ['google'],
    'research': ['tavily', 'google'],
    'educational': ['wikipedia', 'google'],
    'tech_community': ['hackernews'],
    'financial': ['seeking_alpha', 'newsapi']
}
```

#### Query Analysis Routing

```python
QUERY_ANALYSIS_PATTERNS = {
    'financial_indicators': {
        'patterns': [
            r'\b(stock|price|ticker|NYSE|NASDAQ|IPO|earnings|revenue|profit|loss|dividend)\b',
            r'\b[A-Z]{2,5}\b.*\b(stock|price|shares)\b',
            r'\$([\d,]+\.?\d*)\b.*(stock|price|market|cap)',
            r'\b(bull|bear|market|trading|investor|portfolio|hedge fund)\b'
        ],
        'preferred_engines': ['seeking_alpha', 'newsapi'],
        'search_type': 'financial'
    },
    'technical_topics': {
        'patterns': [
            r'\b(programming|coding|python|javascript|java|react|node|database|API|software|development)\b',
            r'\b(github|stackoverflow|tutorial|documentation|framework|library)\b',
            r'\b(startup|tech|YC|Y Combinator|venture|funding|Silicon Valley)\b'
        ],
        'preferred_engines': ['hackernews', 'tavily'],
        'search_type': 'tech_community'
    },
    'academic_topics': {
        'patterns': [
            r'\b(research|study|university|academic|journal|paper|thesis|dissertation)\b',
            r'\b(science|biology|chemistry|physics|mathematics|history|literature)\b',
            r'\b(definition|explanation|how does|what is|theory|concept)\b'
        ],
        'preferred_engines': ['wikipedia', 'google'],
        'search_type': 'educational'
    },
    'news_indicators': {
        'patterns': [
            r'\b(breaking|news|today|latest|update|report|announced|confirmed)\b',
            r'\b(yesterday|today|this week|recent|current|now)\b',
            r'\b(politics|election|government|policy|law|regulation)\b'
        ],
        'preferred_engines': ['newsapi', 'google'],
        'search_type': 'news'
    }
}

def analyze_query_for_engine_preference(query: str, search_type: str) -> Optional[str]:
    """Analyze query content to determine preferred engine."""
    query_lower = query.lower()
    
    for category, config in QUERY_ANALYSIS_PATTERNS.items():
        for pattern in config['patterns']:
            if re.search(pattern, query_lower, re.IGNORECASE):
                # If search type matches or is general, use preferred engines
                if search_type in ['web', config['search_type']]:
                    for engine in config['preferred_engines']:
                        if is_engine_available(engine):
                            return engine
    
    return None
```

## 3. Standard Method Signatures and Response Formats

### 3.1 Python Method Signatures

#### Main WebSearchTools Class

```python
from typing import Dict, List, Optional, Union, Any
from dataclasses import dataclass
from datetime import datetime
import asyncio

@dataclass
class SearchResult:
    """Standard search result structure"""
    title: str
    url: str
    snippet: str
    published_date: Optional[datetime] = None
    score: Optional[float] = None
    source: Optional[str] = None
    metadata: Optional[Dict[str, Any]] = None

@dataclass
class SearchResponse:
    """Standard search response structure"""
    success: bool
    engine_used: str
    search_type: str
    query: str
    execution_time: float
    results: List[SearchResult]
    total_results: Optional[int] = None
    page: Optional[int] = None
    pages_available: Optional[int] = None
    error: Optional[Dict[str, Any]] = None
    metadata: Optional[Dict[str, Any]] = None

class WebSearchTools(Toolset):
    """Unified web search interface supporting multiple engines and search types."""
    
    def __init__(self):
        super().__init__("WebSearchTools")
        self.config_manager = ConfigurationManager()
        self.engine_router = EngineRouter(self.config_manager)
        self.response_standardizer = ResponseStandardizer()
        self.error_handler = ErrorHandler()
        self.engines = self._initialize_engines()
    
    @json_schema({
        "name": "web_search",
        "description": "Perform general web search across multiple engines",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def web_search(
        self,
        query: str,
        engine: str = "auto",
        max_results: int = 10,
        safesearch: str = "moderate",
        include_images: bool = False,
        include_domains: Optional[List[str]] = None,
        exclude_domains: Optional[List[str]] = None,
        search_depth: str = "standard",
        language: str = "en",
        region: str = "us"
    ) -> SearchResponse:
        """Execute general web search with intelligent engine routing."""
        
        # Implementation details provided in section 4
        pass
    
    @json_schema({
        "name": "news_search",
        "description": "Search for news articles across multiple news engines",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def news_search(
        self,
        query: str,
        engine: str = "auto",
        category: Optional[str] = None,
        start_date: Optional[str] = None,
        end_date: Optional[str] = None,
        max_results: int = 10,
        sort: str = "relevancy",
        language: str = "en",
        extract_content: bool = False
    ) -> SearchResponse:
        """Execute news search with category and date filtering."""
        pass
    
    @json_schema({
        "name": "trends_search",
        "description": "Search for trending topics and trend analysis",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def trends_search(
        self,
        query: Optional[str] = None,
        engine: str = "auto",
        timeframe: str = "past_month",
        region: str = "united_states",
        category: str = "all",
        max_results: int = 10,
        find_related: bool = False
    ) -> SearchResponse:
        """Execute trend analysis search."""
        pass
    
    @json_schema({
        "name": "flight_search",
        "description": "Search for flights using Google SERP API",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def flight_search(
        self,
        departure_id: str,
        arrival_id: str,
        outbound_date: str,
        return_date: Optional[str] = None,
        currency: str = "USD",
        flight_type: str = "2",
        search_locale: str = "us",
        max_results: int = 10
    ) -> SearchResponse:
        """Execute flight search."""
        pass
    
    @json_schema({
        "name": "event_search",
        "description": "Search for events using Google SERP API",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def event_search(
        self,
        query: str,
        location: Optional[str] = None,
        start_date: Optional[str] = None,
        end_date: Optional[str] = None,
        max_results: int = 10,
        event_type: str = "all"
    ) -> SearchResponse:
        """Execute event search."""
        pass
    
    @json_schema({
        "name": "research_search",
        "description": "Advanced research search with content extraction",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def research_search(
        self,
        query: str,
        search_depth: str = "advanced",
        include_answer: bool = True,
        include_raw_content: bool = True,
        max_results: int = 5,
        min_score: float = 0.75,
        include_images: bool = False,
        include_domains: Optional[List[str]] = None,
        exclude_domains: Optional[List[str]] = None
    ) -> SearchResponse:
        """Execute advanced research search."""
        pass
    
    @json_schema({
        "name": "educational_search",
        "description": "Search for educational content",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def educational_search(
        self,
        query: str,
        engine: str = "auto",
        subject: str = "all",
        level: str = "all",
        max_results: int = 10,
        language: str = "en"
    ) -> SearchResponse:
        """Execute educational content search."""
        pass
    
    @json_schema({
        "name": "tech_community_search",
        "description": "Search technology community content",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def tech_community_search(
        self,
        query: Optional[str] = None,
        story_type: str = "top",
        max_results: int = 10,
        time_range: str = "day",
        include_comments: bool = False
    ) -> SearchResponse:
        """Execute technology community search."""
        pass
    
    @json_schema({
        "name": "financial_search",
        "description": "Search for financial news and information",
        "parameters": {
            # JSON schema from section 1.1
        }
    })
    def financial_search(
        self,
        query: str,
        engine: str = "auto",
        content_type: str = "all",
        extract_content: bool = False,
        max_results: int = 10,
        symbols: Optional[List[str]] = None,
        sector: str = "all"
    ) -> SearchResponse:
        """Execute financial search."""
        pass
    
    # Utility methods
    @json_schema({
        "name": "get_available_engines",
        "description": "Get list of available search engines",
        "parameters": {
            # JSON schema from section 1.2
        }
    })
    def get_available_engines(
        self,
        search_type: str = "all",
        include_unavailable: bool = False
    ) -> Dict[str, Any]:
        """Get list of available engines and their status."""
        pass
    
    @json_schema({
        "name": "get_engine_capabilities",
        "description": "Get capabilities of specific engine",
        "parameters": {
            # JSON schema from section 1.2
        }
    })
    def get_engine_capabilities(self, engine: str) -> Dict[str, Any]:
        """Get capabilities and features of specific engine."""
        pass
```

### 3.2 Response Format Specifications

#### Standard Success Response

```python
STANDARD_SUCCESS_RESPONSE = {
    "success": True,
    "engine_used": "google",
    "search_type": "web",
    "query": "python web scraping tutorial",
    "execution_time": 1.247,
    "results": [
        {
            "title": "Python Web Scraping Tutorial - Complete Guide",
            "url": "https://example.com/python-scraping-tutorial",
            "snippet": "Learn web scraping with Python using BeautifulSoup and requests. Complete tutorial with examples and best practices.",
            "published_date": "2024-01-10T14:30:00Z",
            "score": 0.95,
            "source": "example.com",
            "metadata": {
                "content_type": "tutorial",
                "language": "en",
                "word_count": 2500,
                "reading_time": "10 min",
                "author": "John Doe",
                "tags": ["python", "web-scraping", "tutorial"]
            }
        }
    ],
    "total_results": 1247,
    "page": 1,
    "pages_available": 125,
    "metadata": {
        "filters_applied": {
            "language": "en",
            "region": "us",
            "safesearch": "moderate"
        },
        "engine_parameters": {
            "num": 10,
            "lr": "lang_en",
            "gl": "us"
        },
        "related_searches": [
            "python beautifulsoup tutorial",
            "web scraping with requests",
            "scrapy python framework"
        ],
        "search_suggestions": [
            "python web scraping selenium",
            "python scraping javascript sites",
            "python web scraping ethics"
        ]
    }
}
```

#### Standard Error Response

```python
STANDARD_ERROR_RESPONSE = {
    "success": False,
    "error": {
        "code": "ENGINE_UNAVAILABLE",
        "message": "Primary search engine is unavailable",
        "details": "Google SERP API returned 429 Too Many Requests",
        "fallback_used": "duckduckgo",
        "retry_after": 300,
        "timestamp": "2024-01-15T10:30:00Z"
    },
    "engine_used": "duckduckgo",
    "search_type": "web",
    "query": "python web scraping tutorial",
    "execution_time": 0.856,
    "results": [
        # Fallback results from DuckDuckGo
    ],
    "metadata": {
        "fallback_reason": "rate_limit_exceeded",
        "original_engine": "google",
        "fallback_chain": ["google", "duckduckgo"]
    }
}
```

#### Engine-Specific Response Examples

**Google SERP Response Mapping:**
```python
def _map_google_serp_response(raw_response: dict) -> SearchResponse:
    """Map Google SERP response to standard format."""
    results = []
    
    for item in raw_response.get('organic_results', []):
        result = SearchResult(
            title=item.get('title', ''),
            url=item.get('link', ''),
            snippet=item.get('snippet', ''),
            published_date=parse_date(item.get('date')),
            score=item.get('position', 0) / 10.0,  # Convert position to score
            source=extract_domain(item.get('link', '')),
            metadata={
                'position': item.get('position'),
                'displayed_link': item.get('displayed_link'),
                'rich_snippet': item.get('rich_snippet', {})
            }
        )
        results.append(result)
    
    return SearchResponse(
        success=True,
        engine_used="google",
        search_type="web",
        query=raw_response.get('search_parameters', {}).get('q', ''),
        execution_time=raw_response.get('search_metadata', {}).get('total_time_taken', 0),
        results=results,
        total_results=raw_response.get('search_information', {}).get('total_results'),
        metadata={
            'search_parameters': raw_response.get('search_parameters', {}),
            'related_searches': [rs.get('query') for rs in raw_response.get('related_searches', [])],
            'knowledge_graph': raw_response.get('knowledge_graph', {})
        }
    )
```

## 4. File Structure and Organization

### 4.1 Complete Directory Structure

```
agent_c_core/toolsets/web_search/
├── __init__.py                          # Package initialization and exports
├── web_search_tools.py                  # Main WebSearchTools class
├── config/
│   ├── __init__.py
│   ├── configuration_manager.py        # API key and configuration management
│   ├── engine_capabilities.py          # Engine capability definitions
│   └── settings.py                     # Default settings and constants
├── engines/
│   ├── __init__.py
│   ├── base_engine.py                  # Abstract base engine class
│   ├── duckduckgo_engine.py           # DuckDuckGo search backend
│   ├── google_serp_engine.py          # Google SERP API backend
│   ├── google_trends_engine.py        # Google Trends backend
│   ├── hackernews_engine.py           # Hacker News backend
│   ├── newsapi_engine.py              # NewsAPI backend
│   ├── seeking_alpha_engine.py        # Seeking Alpha backend
│   ├── tavily_engine.py               # Tavily Research backend
│   └── wikipedia_engine.py            # Wikipedia backend
├── routing/
│   ├── __init__.py
│   ├── engine_router.py               # Engine selection and routing logic
│   ├── availability_checker.py        # Engine health monitoring
│   └── query_analyzer.py              # Query analysis for routing
├── response/
│   ├── __init__.py
│   ├── standardizer.py                # Response standardization
│   ├── formatters.py                  # Response formatting utilities
│   └── models.py                      # Response data models
├── validation/
│   ├── __init__.py
│   ├── parameter_validator.py         # Parameter validation
│   ├── schema_definitions.py          # JSON schema definitions
│   └── input_sanitizer.py             # Input sanitization
├── utils/
│   ├── __init__.py
│   ├── error_handlers.py              # Error handling utilities
│   ├── caching.py                     # Caching mechanisms
│   ├── helpers.py                     # Helper functions
│   └── constants.py                   # Constants and enums
└── tests/
    ├── __init__.py
    ├── test_web_search_tools.py       # Main class tests
    ├── test_engines/                  # Engine-specific tests
    │   ├── __init__.py
    │   ├── test_duckduckgo_engine.py
    │   ├── test_google_serp_engine.py
    │   ├── test_google_trends_engine.py
    │   ├── test_hackernews_engine.py
    │   ├── test_newsapi_engine.py
    │   ├── test_seeking_alpha_engine.py
    │   ├── test_tavily_engine.py
    │   └── test_wikipedia_engine.py
    ├── test_routing/                  # Routing tests
    │   ├── __init__.py
    │   ├── test_engine_router.py
    │   ├── test_availability_checker.py
    │   └── test_query_analyzer.py
    ├── test_response/                 # Response handling tests
    │   ├── __init__.py
    │   ├── test_standardizer.py
    │   └── test_formatters.py
    ├── test_validation/               # Validation tests
    │   ├── __init__.py
    │   ├── test_parameter_validator.py
    │   └── test_input_sanitizer.py
    ├── fixtures/                      # Test fixtures and mock data
    │   ├── __init__.py
    │   ├── mock_responses.py
    │   └── test_data.py
    └── integration/                   # Integration tests
        ├── __init__.py
        ├── test_end_to_end.py
        └── test_engine_fallback.py
```

### 4.2 Key File Specifications

#### __init__.py (Package Root)

```python
"""
Web Search Unified Tool Package

This package provides a unified interface for multiple web search engines,
consolidating 8 separate search tools into a single, maintainable solution.
"""

from .web_search_tools import WebSearchTools
from .response.models import SearchResult, SearchResponse
from .utils.constants import (
    SUPPORTED_ENGINES,
    SEARCH_TYPES,
    DEFAULT_SETTINGS
)
from .config.engine_capabilities import ENGINE_CAPABILITIES

__version__ = "1.0.0"
__author__ = "Agent C Framework Team"

# Export main classes and utilities
__all__ = [
    'WebSearchTools',
    'SearchResult',
    'SearchResponse',
    'SUPPORTED_ENGINES',
    'SEARCH_TYPES',
    'ENGINE_CAPABILITIES'
]

# Package-level configuration
DEFAULT_CONFIG = {
    'cache_enabled': True,
    'cache_ttl': 3600,
    'fallback_enabled': True,
    'timeout': 30,
    'max_retries': 3
}
```

#### web_search_tools.py (Main Class)

```python
"""
WebSearchTools - Unified Web Search Interface

This module provides the main WebSearchTools class that consolidates
multiple search engines into a single, unified interface.
"""

import time
from typing import Dict, List, Optional, Any
from agent_c_core.toolsets import Toolset, json_schema
from .config.configuration_manager import ConfigurationManager
from .routing.engine_router import EngineRouter
from .response.standardizer import ResponseStandardizer
from .validation.parameter_validator import ParameterValidator
from .utils.error_handlers import ErrorHandler
from .response.models import SearchResponse
from .utils.constants import SEARCH_TYPES, SUPPORTED_ENGINES

class WebSearchTools(Toolset):
    """
    Unified web search interface supporting multiple search engines
    and search types with intelligent routing and standardized responses.
    """
    
    def __init__(self):
        super().__init__("WebSearchTools")
        self.config_manager = ConfigurationManager()
        self.engine_router = EngineRouter(self.config_manager)
        self.response_standardizer = ResponseStandardizer()
        self.parameter_validator = ParameterValidator()
        self.error_handler = ErrorHandler()
        
        # Initialize engines
        self.engines = self._initialize_engines()
        
        # Register with framework
        Toolset.register(self)
    
    def _initialize_engines(self) -> Dict[str, Any]:
        """Initialize all search engine backends."""
        engines = {}
        
        # Dynamic engine loading based on availability
        for engine_name in SUPPORTED_ENGINES:
            try:
                engine_class = self._get_engine_class(engine_name)
                engine_instance = engine_class(self.config_manager)
                engines[engine_name] = engine_instance
            except Exception as e:
                self.logger.warning(f"Failed to initialize {engine_name}: {e}")
        
        return engines
    
    def _get_engine_class(self, engine_name: str):
        """Dynamically import and return engine class."""
        module_map = {
            'duckduckgo': 'engines.duckduckgo_engine.DuckDuckGoEngine',
            'google': 'engines.google_serp_engine.GoogleSerpEngine',
            'google_trends': 'engines.google_trends_engine.GoogleTrendsEngine',
            'hackernews': 'engines.hackernews_engine.HackerNewsEngine',
            'newsapi': 'engines.newsapi_engine.NewsApiEngine',
            'seeking_alpha': 'engines.seeking_alpha_engine.SeekingAlphaEngine',
            'tavily': 'engines.tavily_engine.TavilyEngine',
            'wikipedia': 'engines.wikipedia_engine.WikipediaEngine'
        }
        
        module_path = module_map.get(engine_name)
        if not module_path:
            raise ValueError(f"Unknown engine: {engine_name}")
        
        module_name, class_name = module_path.rsplit('.', 1)
        module = __import__(f"agent_c_core.toolsets.web_search.{module_name}", 
                          fromlist=[class_name])
        return getattr(module, class_name)
    
    def _execute_search(self, search_type: str, **kwargs) -> SearchResponse:
        """Common search execution logic."""
        start_time = time.time()
        
        try:
            # Validate parameters
            validated_params = self.parameter_validator.validate_parameters(
                search_type, kwargs
            )
            
            # Route to appropriate engine
            engine_name = self.engine_router.route_search_request(
                search_type, 
                validated_params.get('engine', 'auto'),
                validated_params.get('query', ''),
                validated_params
            )
            
            # Execute search
            engine = self.engines[engine_name]
            raw_response = engine.execute_search(search_type, validated_params)
            
            # Standardize response
            response = self.response_standardizer.standardize_response(
                raw_response, engine_name, search_type
            )
            
            # Add execution metadata
            response.execution_time = time.time() - start_time
            response.engine_used = engine_name
            response.search_type = search_type
            
            return response
            
        except Exception as e:
            # Handle errors with fallback
            return self.error_handler.handle_error(
                e, search_type, kwargs, time.time() - start_time
            )
    
    # Search method implementations use _execute_search
    @json_schema({
        "name": "web_search",
        "description": "Perform general web search across multiple engines",
        "parameters": {
            # Full JSON schema from section 1.1
        }
    })
    def web_search(self, **kwargs) -> SearchResponse:
        """Execute general web search with intelligent engine routing."""
        return self._execute_search('web', **kwargs)
    
    # Additional methods follow same pattern...
    # Full implementation details in Phase 2
```

#### base_engine.py (Abstract Base Class)

```python
"""
Base Engine Abstract Class

Defines the interface that all search engine backends must implement.
"""

from abc import ABC, abstractmethod
from typing import Dict, Any, List, Optional
from ..response.models import SearchResponse

class BaseEngine(ABC):
    """Abstract base class for all search engines."""
    
    def __init__(self, config_manager):
        self.config_manager = config_manager
        self.engine_name = self._get_engine_name()
        self.capabilities = self._get_capabilities()
        
    @abstractmethod
    def _get_engine_name(self) -> str:
        """Return the name of this engine."""
        pass
    
    @abstractmethod
    def _get_capabilities(self) -> Dict[str, Any]:
        """Return the capabilities of this engine."""
        pass
    
    @abstractmethod
    def execute_search(self, search_type: str, params: Dict[str, Any]) -> Dict[str, Any]:
        """Execute a search and return raw results."""
        pass
    
    @abstractmethod
    def is_available(self) -> bool:
        """Check if the engine is available for use."""
        pass
    
    @abstractmethod
    def supports_search_type(self, search_type: str) -> bool:
        """Check if the engine supports the given search type."""
        pass
    
    def get_health_status(self) -> Dict[str, Any]:
        """Return health status of the engine."""
        return {
            'engine': self.engine_name,
            'available': self.is_available(),
            'capabilities': self.capabilities,
            'last_check': time.time()
        }
```

### 4.3 Configuration Files

#### settings.py (Default Settings)

```python
"""
Default Settings and Configuration

This module contains all default settings and configuration options
for the WebSearchTools package.
"""

import os
from typing import Dict, Any, List

# API Configuration
API_KEYS = {
    'SERPAPI_API_KEY': os.getenv('SERPAPI_API_KEY'),
    'NEWSAPI_API_KEY': os.getenv('NEWSAPI_API_KEY'),
    'TAVILI_API_KEY': os.getenv('TAVILI_API_KEY')
}

# Engine Configuration
ENGINE_CONFIGS = {
    'duckduckgo': {
        'requires_api_key': False,
        'base_url': 'https://duckduckgo.com',
        'timeout': 10,
        'max_retries': 3,
        'supports_search_types': ['web']
    },
    'google': {
        'requires_api_key': True,
        'api_key_name': 'SERPAPI_API_KEY',
        'base_url': 'https://serpapi.com/search',
        'timeout': 15,
        'max_retries': 3,
        'supports_search_types': ['web', 'news', 'flights', 'events']
    },
    'google_trends': {
        'requires_api_key': False,
        'timeout': 20,
        'max_retries': 3,
        'supports_search_types': ['trends'],
        'cache_ttl': 3600
    },
    'hackernews': {
        'requires_api_key': False,
        'base_url': 'https://hacker-news.firebaseio.com/v0',
        'timeout': 10,
        'max_retries': 3,
        'supports_search_types': ['tech_community', 'trends']
    },
    'newsapi': {
        'requires_api_key': True,
        'api_key_name': 'NEWSAPI_API_KEY',
        'base_url': 'https://newsapi.org/v2',
        'timeout': 15,
        'max_retries': 3,
        'supports_search_types': ['news', 'financial']
    },
    'seeking_alpha': {
        'requires_api_key': False,
        'base_url': 'https://seekingalpha.com',
        'timeout': 15,
        'max_retries': 3,
        'supports_search_types': ['financial', 'news']
    },
    'tavily': {
        'requires_api_key': True,
        'api_key_name': 'TAVILI_API_KEY',
        'base_url': 'https://api.tavily.com',
        'timeout': 30,
        'max_retries': 3,
        'supports_search_types': ['web', 'research']
    },
    'wikipedia': {
        'requires_api_key': False,
        'base_url': 'https://en.wikipedia.org/api/rest_v1',
        'timeout': 10,
        'max_retries': 3,
        'supports_search_types': ['educational']
    }
}

# Search Type Preferences
SEARCH_TYPE_ENGINE_PREFERENCES = {
    'web': ['tavily', 'google', 'duckduckgo'],
    'news': ['newsapi', 'google', 'seeking_alpha'],
    'trends': ['google_trends', 'hackernews'],
    'flights': ['google'],
    'events': ['google'],
    'research': ['tavily', 'google'],
    'educational': ['wikipedia', 'google'],
    'tech_community': ['hackernews'],
    'financial': ['seeking_alpha', 'newsapi']
}

# Response Format Settings
RESPONSE_SETTINGS = {
    'max_snippet_length': 300,
    'date_formats': ['%Y-%m-%d', '%Y-%m-%dT%H:%M:%SZ', '%Y-%m-%dT%H:%M:%S.%fZ'],
    'default_score': 0.5,
    'score_precision': 2
}

# Caching Settings
CACHE_SETTINGS = {
    'enabled': True,
    'ttl': 3600,  # 1 hour
    'max_size': 1000,
    'cache_key_prefix': 'websearch_'
}

# Error Handling Settings
ERROR_SETTINGS = {
    'enable_fallback': True,
    'max_fallback_attempts': 3,
    'fallback_delay': 1,  # seconds
    'retry_delay': 2,     # seconds
    'timeout_multiplier': 1.5
}

# Validation Settings
VALIDATION_SETTINGS = {
    'max_query_length': 2048,
    'max_results': 100,
    'min_results': 1,
    'allowed_languages': ['en', 'es', 'fr', 'de', 'it', 'pt', 'ru', 'zh', 'ja', 'ko'],
    'allowed_regions': ['us', 'uk', 'ca', 'au', 'fr', 'de', 'it', 'es', 'br', 'in', 'jp', 'cn']
}

# Default Parameters
DEFAULT_PARAMETERS = {
    'web_search': {
        'engine': 'auto',
        'max_results': 10,
        'safesearch': 'moderate',
        'include_images': False,
        'search_depth': 'standard',
        'language': 'en',
        'region': 'us'
    },
    'news_search': {
        'engine': 'auto',
        'max_results': 10,
        'sort': 'relevancy',
        'language': 'en',
        'extract_content': False
    },
    'trends_search': {
        'engine': 'auto',
        'timeframe': 'past_month',
        'region': 'united_states',
        'category': 'all',
        'max_results': 10,
        'find_related': False
    },
    'flight_search': {
        'currency': 'USD',
        'flight_type': '2',
        'search_locale': 'us',
        'max_results': 10
    },
    'event_search': {
        'max_results': 10,
        'event_type': 'all'
    },
    'research_search': {
        'search_depth': 'advanced',
        'include_answer': True,
        'include_raw_content': True,
        'max_results': 5,
        'min_score': 0.75,
        'include_images': False
    },
    'educational_search': {
        'engine': 'auto',
        'subject': 'all',
        'level': 'all',
        'max_results': 10,
        'language': 'en'
    },
    'tech_community_search': {
        'story_type': 'top',
        'max_results': 10,
        'time_range': 'day',
        'include_comments': False
    },
    'financial_search': {
        'engine': 'auto',
        'content_type': 'all',
        'extract_content': False,
        'max_results': 10,
        'sector': 'all'
    }
}

# Export all settings
__all__ = [
    'API_KEYS',
    'ENGINE_CONFIGS',
    'SEARCH_TYPE_ENGINE_PREFERENCES',
    'RESPONSE_SETTINGS',
    'CACHE_SETTINGS',
    'ERROR_SETTINGS',
    'VALIDATION_SETTINGS',
    'DEFAULT_PARAMETERS'
]
```

## 5. Replacement Strategy for Old Tools

### 5.1 Migration Plan Overview

The replacement strategy follows a 4-phase approach to ensure zero downtime and smooth transition:

1. **Phase 1: Parallel Development** (Weeks 1-4)
2. **Phase 2: Soft Launch** (Weeks 5-6)
3. **Phase 3: Full Migration** (Weeks 7-8)
4. **Phase 4: Cleanup** (Weeks 9-10)

### 5.2 Detailed Migration Steps

#### Phase 1: Parallel Development (Weeks 1-4)

**Week 1: Infrastructure Setup**
- Create new directory structure
- Implement base classes and configuration management
- Set up testing framework
- Create CI/CD pipeline for new package

**Week 2: Engine Backend Development**
- Implement all 8 engine backends
- Create engine router and availability checker
- Implement response standardizer
- Add parameter validation

**Week 3: Main Interface Development**
- Implement WebSearchTools main class
- Add all search methods with JSON schemas
- Implement error handling and fallback logic
- Add caching layer

**Week 4: Testing and Validation**
- Complete unit tests for all components
- Integration testing with live APIs
- Performance testing and optimization
- Documentation creation

#### Phase 2: Soft Launch (Weeks 5-6)

**Week 5: Optional Integration**
- Deploy WebSearchTools as additional option
- Update toolset registry to include new tool
- Create migration guide documentation
- Begin user acceptance testing

**Week 6: Feedback Integration**
- Gather feedback from early adopters
- Fix bugs and performance issues
- Optimize engine routing based on usage patterns
- Update documentation based on feedback

#### Phase 3: Full Migration (Weeks 7-8)

**Week 7: Default Switch**
- Make WebSearchTools the default search interface
- Add deprecation warnings to old tools
- Update all internal references
- Notify users of the change

**Week 8: Legacy Support**
- Maintain old tools for backward compatibility
- Route old tool calls to new interface
- Update all documentation
- Monitor for migration issues

#### Phase 4: Cleanup (Weeks 9-10)

**Week 9: Code Removal**
- Remove old tool implementations
- Clean up imports and dependencies
- Optimize unified implementation
- Update package structure

**Week 10: Final Validation**
- Comprehensive testing of final implementation
- Performance validation
- Security audit
- Final documentation update

### 5.3 Tool-by-Tool Migration Mapping

#### DuckDuckGoTools → WebSearchTools

**Old Interface:**
```python
duckduckgo_tools = DuckDuckGoTools()
result = duckduckgo_tools.web_search(
    query="python tutorial",
    max_results=20,
    safesearch="moderate"
)
```

**New Interface:**
```python
web_search_tools = WebSearchTools()
result = web_search_tools.web_search(
    query="python tutorial",
    engine="duckduckgo",  # Explicit engine selection
    max_results=20,
    safesearch="moderate"
)
```

**Migration Bridge:**
```python
# Temporary bridge class for backward compatibility
class DuckDuckGoToolsBridge:
    def __init__(self):
        self.web_search_tools = WebSearchTools()
    
    def web_search(self, **kwargs):
        # Add deprecation warning
        warnings.warn(
            "DuckDuckGoTools is deprecated. Use WebSearchTools.web_search() instead.",
            DeprecationWarning,
            stacklevel=2
        )
        
        # Route to new interface
        return self.web_search_tools.web_search(engine="duckduckgo", **kwargs)
```

#### GoogleSerpTools → WebSearchTools

**Migration Mapping:**
```python
GOOGLE_SERP_METHOD_MAPPING = {
    'get_search_results': 'web_search',
    'get_news': 'news_search',
    'get_flights': 'flight_search',
    'get_events': 'event_search'
}

class GoogleSerpToolsBridge:
    def __init__(self):
        self.web_search_tools = WebSearchTools()
    
    def get_search_results(self, **kwargs):
        warnings.warn("GoogleSerpTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        return self.web_search_tools.web_search(engine="google", **kwargs)
    
    def get_news(self, **kwargs):
        warnings.warn("GoogleSerpTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        return self.web_search_tools.news_search(engine="google", **kwargs)
    
    def get_flights(self, **kwargs):
        warnings.warn("GoogleSerpTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        return self.web_search_tools.flight_search(**kwargs)
    
    def get_events(self, **kwargs):
        warnings.warn("GoogleSerpTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        return self.web_search_tools.event_search(**kwargs)
```

#### NewsApiTools → WebSearchTools

**Migration Mapping:**
```python
NEWS_API_METHOD_MAPPING = {
    'get_top_headlines': 'news_search',
    'get_all_articles': 'news_search',
    'get_sources': 'get_available_engines'
}

class NewsApiToolsBridge:
    def __init__(self):
        self.web_search_tools = WebSearchTools()
    
    def get_top_headlines(self, **kwargs):
        warnings.warn("NewsApiTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        
        # Convert parameters
        converted_params = self._convert_newsapi_params(kwargs)
        return self.web_search_tools.news_search(engine="newsapi", **converted_params)
    
    def get_all_articles(self, **kwargs):
        warnings.warn("NewsApiTools is deprecated. Use WebSearchTools.", DeprecationWarning)
        
        # Convert parameters
        converted_params = self._convert_newsapi_params(kwargs)
        converted_params['query'] = kwargs.get('q', '')
        return self.web_search_tools.news_search(engine="newsapi", **converted_params)
    
    def _convert_newsapi_params(self, params):
        """Convert NewsAPI parameters to WebSearchTools format."""
        converted = {}
        
        # Parameter mapping
        if 'pageSize' in params:
            converted['max_results'] = params['pageSize']
        if 'sortBy' in params:
            converted['sort'] = params['sortBy']
        if 'from' in params:
            converted['start_date'] = params['from']
        if 'to' in params:
            converted['end_date'] = params['to']
        
        # Copy direct mappings
        for key in ['category', 'language']:
            if key in params:
                converted[key] = params[key]
        
        return converted
```

### 5.4 Automated Migration Scripts

#### migration_validator.py

```python
"""
Migration Validation Script

This script validates that the new WebSearchTools interface
produces equivalent results to the old tools.
"""

import json
import time
from typing import Dict, Any, List
from concurrent.futures import ThreadPoolExecutor, as_completed

class MigrationValidator:
    """Validates migration from old tools to new interface."""
    
    def __init__(self):
        self.test_cases = self._load_test_cases()
        self.results = []
    
    def _load_test_cases(self) -> List[Dict[str, Any]]:
        """Load test cases for validation."""
        return [
            {
                'tool': 'DuckDuckGoTools',
                'method': 'web_search',
                'params': {'query': 'python tutorial', 'max_results': 10},
                'expected_fields': ['title', 'url', 'snippet']
            },
            {
                'tool': 'GoogleSerpTools',
                'method': 'get_search_results',
                'params': {'query': 'machine learning', 'max_return': 5},
                'expected_fields': ['title', 'url', 'snippet']
            },
            {
                'tool': 'NewsApiTools',
                'method': 'get_top_headlines',
                'params': {'category': 'technology', 'pageSize': 5},
                'expected_fields': ['title', 'url', 'snippet', 'published_date']
            }
            # Add more test cases for all tools
        ]
    
    def validate_migration(self) -> Dict[str, Any]:
        """Run comprehensive migration validation."""
        validation_results = {
            'passed': 0,
            'failed': 0,
            'errors': [],
            'performance_comparison': {}
        }
        
        for test_case in self.test_cases:
            try:
                # Run old tool
                old_result, old_time = self._run_old_tool(test_case)
                
                # Run new tool
                new_result, new_time = self._run_new_tool(test_case)
                
                # Compare results
                comparison = self._compare_results(old_result, new_result, test_case)
                
                if comparison['passed']:
                    validation_results['passed'] += 1
                else:
                    validation_results['failed'] += 1
                    validation_results['errors'].append({
                        'test_case': test_case,
                        'comparison': comparison
                    })
                
                # Record performance
                validation_results['performance_comparison'][f"{test_case['tool']}_{test_case['method']}"] = {
                    'old_time': old_time,
                    'new_time': new_time,
                    'improvement': (old_time - new_time) / old_time * 100
                }
                
            except Exception as e:
                validation_results['failed'] += 1
                validation_results['errors'].append({
                    'test_case': test_case,
                    'error': str(e)
                })
        
        return validation_results
    
    def _run_old_tool(self, test_case: Dict[str, Any]) -> tuple:
        """Run old tool and measure performance."""
        start_time = time.time()
        
        # Dynamic import and execution
        tool_class = self._get_old_tool_class(test_case['tool'])
        tool_instance = tool_class()
        method = getattr(tool_instance, test_case['method'])
        result = method(**test_case['params'])
        
        execution_time = time.time() - start_time
        return result, execution_time
    
    def _run_new_tool(self, test_case: Dict[str, Any]) -> tuple:
        """Run new tool and measure performance."""
        start_time = time.time()
        
        # Map to new interface
        web_search_tools = WebSearchTools()
        new_method, new_params = self._map_to_new_interface(test_case)
        method = getattr(web_search_tools, new_method)
        result = method(**new_params)
        
        execution_time = time.time() - start_time
        return result, execution_time
    
    def _compare_results(self, old_result, new_result, test_case) -> Dict[str, Any]:
        """Compare results from old and new tools."""
        comparison = {
            'passed': True,
            'issues': []
        }
        
        # Check if both results have data
        if not old_result or not new_result:
            comparison['passed'] = False
            comparison['issues'].append('One or both results are empty')
            return comparison
        
        # Check expected fields
        for field in test_case['expected_fields']:
            if not self._has_field_in_results(old_result, field):
                comparison['issues'].append(f'Old result missing field: {field}')
            if not self._has_field_in_results(new_result, field):
                comparison['issues'].append(f'New result missing field: {field}')
        
        # Check result count similarity
        old_count = len(old_result.get('results', []))
        new_count = len(new_result.get('results', []))
        
        if abs(old_count - new_count) > 2:  # Allow small differences
            comparison['passed'] = False
            comparison['issues'].append(f'Result count mismatch: old={old_count}, new={new_count}')
        
        return comparison
```

#### migration_script.py

```python
"""
Automated Migration Script

This script performs the actual migration from old tools to new interface.
"""

import os
import shutil
import logging
from typing import List, Dict, Any

class MigrationScript:
    """Handles the automated migration process."""
    
    def __init__(self, backup_dir: str = "/tmp/web_search_migration_backup"):
        self.backup_dir = backup_dir
        self.logger = logging.getLogger(__name__)
        
    def execute_migration(self, phase: str) -> Dict[str, Any]:
        """Execute migration for specified phase."""
        migration_methods = {
            'backup': self._create_backup,
            'deploy': self._deploy_new_tools,
            'bridge': self._create_bridge_tools,
            'switch': self._switch_to_new_tools,
            'cleanup': self._cleanup_old_tools
        }
        
        if phase not in migration_methods:
            raise ValueError(f"Unknown migration phase: {phase}")
        
        return migration_methods[phase]()
    
    def _create_backup(self) -> Dict[str, Any]:
        """Create backup of existing tools."""
        old_tools_dir = "agent_c_core/toolsets"
        backup_info = {
            'timestamp': time.time(),
            'backed_up_tools': [],
            'success': True
        }
        
        try:
            os.makedirs(self.backup_dir, exist_ok=True)
            
            tools_to_backup = [
                'DuckDuckGoTools',
                'GoogleSerpTools',
                'GoogleTrendsTools',
                'HackerNewsTools',
                'NewsApiTools',
                'SeekingAlphaTools',
                'TavilyResearchTools',
                'WikipediaTools'
            ]
            
            for tool in tools_to_backup:
                tool_path = os.path.join(old_tools_dir, tool.lower())
                if os.path.exists(tool_path):
                    backup_path = os.path.join(self.backup_dir, tool.lower())
                    shutil.copytree(tool_path, backup_path)
                    backup_info['backed_up_tools'].append(tool)
            
            self.logger.info(f"Backup created successfully: {self.backup_dir}")
            
        except Exception as e:
            backup_info['success'] = False
            backup_info['error'] = str(e)
            self.logger.error(f"Backup failed: {e}")
        
        return backup_info
    
    def _deploy_new_tools(self) -> Dict[str, Any]:
        """Deploy new WebSearchTools."""
        deploy_info = {
            'timestamp': time.time(),
            'deployed_files': [],
            'success': True
        }
        
        try:
            # Copy new tool files
            source_dir = "agent_c_core/toolsets/web_search"
            target_dir = "agent_c_core/toolsets/web_search"
            
            # Implementation depends on deployment method
            # This is a placeholder for actual deployment logic
            
            self.logger.info("New tools deployed successfully")
            
        except Exception as e:
            deploy_info['success'] = False
            deploy_info['error'] = str(e)
            self.logger.error(f"Deployment failed: {e}")
        
        return deploy_info
    
    def _create_bridge_tools(self) -> Dict[str, Any]:
        """Create bridge tools for backward compatibility."""
        bridge_info = {
            'timestamp': time.time(),
            'bridge_tools': [],
            'success': True
        }
        
        # Implementation for creating bridge tools
        # This ensures backward compatibility during transition
        
        return bridge_info
    
    def _switch_to_new_tools(self) -> Dict[str, Any]:
        """Switch default tools to new interface."""
        switch_info = {
            'timestamp': time.time(),
            'updated_imports': [],
            'success': True
        }
        
        # Implementation for switching imports and references
        # Update __init__.py files, documentation, etc.
        
        return switch_info
    
    def _cleanup_old_tools(self) -> Dict[str, Any]:
        """Clean up old tool implementations."""
        cleanup_info = {
            'timestamp': time.time(),
            'removed_tools': [],
            'success': True
        }
        
        # Implementation for removing old tools
        # Only after successful migration and testing
        
        return cleanup_info
```

## 6. Testing Strategy

### 6.1 Testing Framework Architecture

The testing strategy employs a multi-layered approach:

1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test component interactions
3. **End-to-End Tests**: Test complete workflows
4. **Performance Tests**: Validate performance requirements
5. **Migration Tests**: Ensure compatibility with old tools

### 6.2 Unit Testing Strategy

#### Engine Backend Tests

```python
"""
Test Template for Engine Backends

Each engine backend must pass these standard tests.
"""

import unittest
from unittest.mock import Mock, patch, MagicMock
import pytest
from agent_c_core.toolsets.web_search.engines.base_engine import BaseEngine
from agent_c_core.toolsets.web_search.engines.duckduckgo_engine import DuckDuckGoEngine

class TestDuckDuckGoEngine(unittest.TestCase):
    """Test DuckDuckGo engine implementation."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.config_manager = Mock()
        self.engine = DuckDuckGoEngine(self.config_manager)
        
    def test_engine_initialization(self):
        """Test engine initializes correctly."""
        self.assertEqual(self.engine.engine_name, 'duckduckgo')
        self.assertIsNotNone(self.engine.capabilities)
        self.assertIn('web', self.engine.capabilities['search_types'])
        
    def test_is_available(self):
        """Test engine availability check."""
        # Test when available
        self.assertTrue(self.engine.is_available())
        
        # Test when unavailable (network issues, etc.)
        with patch('requests.get') as mock_get:
            mock_get.side_effect = Exception("Network error")
            self.assertFalse(self.engine.is_available())
    
    def test_supports_search_type(self):
        """Test search type support checking."""
        self.assertTrue(self.engine.supports_search_type('web'))
        self.assertFalse(self.engine.supports_search_type('flights'))
        
    @patch('duckduckgo_search.DDGS')
    def test_execute_search_success(self, mock_ddgs):
        """Test successful search execution."""
        # Mock successful response
        mock_ddgs_instance = Mock()
        mock_ddgs.return_value = mock_ddgs_instance
        mock_ddgs_instance.text.return_value = [
            {
                'title': 'Test Result',
                'href': 'https://example.com',
                'body': 'Test snippet'
            }
        ]
        
        result = self.engine.execute_search('web', {'query': 'test query'})
        
        self.assertIsNotNone(result)
        self.assertIn('results', result)
        self.assertEqual(len(result['results']), 1)
        self.assertEqual(result['results'][0]['title'], 'Test Result')
        
    @patch('duckduckgo_search.DDGS')
    def test_execute_search_failure(self, mock_ddgs):
        """Test search execution failure handling."""
        # Mock failed response
        mock_ddgs.side_effect = Exception("API Error")
        
        with self.assertRaises(Exception):
            self.engine.execute_search('web', {'query': 'test query'})
    
    def test_parameter_validation(self):
        """Test parameter validation."""
        # Valid parameters
        valid_params = {
            'query': 'test query',
            'max_results': 10,
            'safesearch': 'moderate'
        }
        
        # Should not raise exception
        self.engine._validate_parameters(valid_params)
        
        # Invalid parameters
        invalid_params = {
            'query': '',  # Empty query
            'max_results': -1,  # Invalid max_results
            'safesearch': 'invalid'  # Invalid safesearch
        }
        
        with self.assertRaises(ValueError):
            self.engine._validate_parameters(invalid_params)
    
    def test_response_formatting(self):
        """Test response formatting to standard format."""
        raw_response = [
            {
                'title': 'Test Result',
                'href': 'https://example.com',
                'body': 'Test snippet'
            }
        ]
        
        formatted_response = self.engine._format_response(raw_response)
        
        self.assertIn('results', formatted_response)
        self.assertEqual(len(formatted_response['results']), 1)
        
        result = formatted_response['results'][0]
        self.assertEqual(result['title'], 'Test Result')
        self.assertEqual(result['url'], 'https://example.com')
        self.assertEqual(result['snippet'], 'Test snippet')
        
    def test_health_status(self):
        """Test health status reporting."""
        status = self.engine.get_health_status()
        
        self.assertIn('engine', status)
        self.assertIn('available', status)
        self.assertIn('capabilities', status)
        self.assertIn('last_check', status)
        
        self.assertEqual(status['engine'], 'duckduckgo')
        self.assertIsInstance(status['available'], bool)

# Similar test classes for all other engines...
```

#### Router and Configuration Tests

```python
"""
Test Engine Router and Configuration Management
"""

import unittest
from unittest.mock import Mock, patch
from agent_c_core.toolsets.web_search.routing.engine_router import EngineRouter
from agent_c_core.toolsets.web_search.config.configuration_manager import ConfigurationManager

class TestEngineRouter(unittest.TestCase):
    """Test engine routing logic."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.config_manager = Mock()
        self.router = EngineRouter(self.config_manager)
        
    def test_auto_routing_web_search(self):
        """Test auto-routing for web search."""
        # Mock engine availability
        self.config_manager.is_engine_available.side_effect = lambda x: x in ['tavily', 'google']
        
        engine = self.router.route_search_request('web', 'auto', 'test query', {})
        
        self.assertIn(engine, ['tavily', 'google'])  # Should pick preferred engine
        
    def test_explicit_engine_selection(self):
        """Test explicit engine selection."""
        # Mock engine availability
        self.config_manager.is_engine_available.return_value = True
        
        engine = self.router.route_search_request('web', 'duckduckgo', 'test query', {})
        
        self.assertEqual(engine, 'duckduckgo')
        
    def test_fallback_routing(self):
        """Test fallback when preferred engine unavailable."""
        # Mock engine availability - preferred unavailable, fallback available
        def mock_availability(engine_name):
            return engine_name == 'duckduckgo'  # Only duckduckgo available
        
        self.config_manager.is_engine_available.side_effect = mock_availability
        
        engine = self.router.route_search_request('web', 'auto', 'test query', {})
        
        self.assertEqual(engine, 'duckduckgo')
        
    def test_query_analysis_routing(self):
        """Test query analysis for intelligent routing."""
        # Mock engine availability
        self.config_manager.is_engine_available.return_value = True
        
        # Financial query should route to financial engines
        engine = self.router.route_search_request('web', 'auto', 'AAPL stock price', {})
        
        self.assertIn(engine, ['seeking_alpha', 'newsapi'])
        
    def test_no_engines_available(self):
        """Test error handling when no engines available."""
        # Mock no engines available
        self.config_manager.is_engine_available.return_value = False
        
        with self.assertRaises(Exception):
            self.router.route_search_request('web', 'auto', 'test query', {})

class TestConfigurationManager(unittest.TestCase):
    """Test configuration management."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.config_manager = ConfigurationManager()
        
    @patch.dict('os.environ', {'SERPAPI_API_KEY': 'test_key'})
    def test_api_key_detection(self):
        """Test API key detection from environment."""
        self.assertTrue(self.config_manager.is_engine_available('google'))
        
    @patch.dict('os.environ', {}, clear=True)
    def test_missing_api_key(self):
        """Test handling of missing API keys."""
        self.assertFalse(self.config_manager.is_engine_available('google'))
        
    def test_engine_configuration(self):
        """Test engine configuration retrieval."""
        config = self.config_manager.get_engine_config('duckduckgo')
        
        self.assertIsNotNone(config)
        self.assertIn('requires_api_key', config)
        self.assertIn('supports_search_types', config)
        
    def test_validate_api_keys(self):
        """Test API key validation."""
        validation_result = self.config_manager.validate_api_keys()
        
        self.assertIsInstance(validation_result, dict)
        self.assertIn('valid_keys', validation_result)
        self.assertIn('invalid_keys', validation_result)
```

### 6.3 Integration Testing Strategy

#### End-to-End Workflow Tests

```python
"""
End-to-End Integration Tests

These tests validate complete workflows from request to response.
"""

import unittest
from unittest.mock import patch, Mock
from agent_c_core.toolsets.web_search.web_search_tools import WebSearchTools

class TestWebSearchToolsIntegration(unittest.TestCase):
    """Integration tests for WebSearchTools."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.web_search_tools = WebSearchTools()
        
    @patch('agent_c_core.toolsets.web_search.engines.duckduckgo_engine.DDGS')
    def test_complete_web_search_workflow(self, mock_ddgs):
        """Test complete web search workflow."""
        # Mock successful DuckDuckGo response
        mock_ddgs_instance = Mock()
        mock_ddgs.return_value = mock_ddgs_instance
        mock_ddgs_instance.text.return_value = [
            {
                'title': 'Python Tutorial',
                'href': 'https://python.org/tutorial',
                'body': 'Learn Python programming'
            }
        ]
        
        # Execute search
        result = self.web_search_tools.web_search(
            query="python tutorial",
            engine="duckduckgo",
            max_results=5
        )
        
        # Validate response structure
        self.assertIsNotNone(result)
        self.assertTrue(result.success)
        self.assertEqual(result.engine_used, 'duckduckgo')
        self.assertEqual(result.search_type, 'web')
        self.assertEqual(len(result.results), 1)
        
        # Validate result structure
        search_result = result.results[0]
        self.assertEqual(search_result.title, 'Python Tutorial')
        self.assertEqual(search_result.url, 'https://python.org/tutorial')
        self.assertEqual(search_result.snippet, 'Learn Python programming')
        
    def test_engine_fallback_workflow(self):
        """Test engine fallback when primary engine fails."""
        # Mock primary engine failure and fallback success
        with patch.object(self.web_search_tools.engines['google'], 'execute_search') as mock_google:
            with patch.object(self.web_search_tools.engines['duckduckgo'], 'execute_search') as mock_duckduckgo:
                
                # Primary engine fails
                mock_google.side_effect = Exception("API Error")
                
                # Fallback engine succeeds
                mock_duckduckgo.return_value = {
                    'results': [
                        {
                            'title': 'Fallback Result',
                            'url': 'https://example.com',
                            'snippet': 'Fallback snippet'
                        }
                    ]
                }
                
                # Execute search with auto routing
                result = self.web_search_tools.web_search(
                    query="test query",
                    engine="auto"
                )
                
                # Should use fallback engine
                self.assertEqual(result.engine_used, 'duckduckgo')
                self.assertTrue(result.success)
                
    def test_parameter_validation_integration(self):
        """Test parameter validation in complete workflow."""
        # Test invalid parameters
        with self.assertRaises(ValueError):
            self.web_search_tools.web_search(
                query="",  # Empty query should fail
                max_results=1000  # Exceeds maximum
            )
            
        # Test valid parameters
        with patch.object(self.web_search_tools.engines['duckduckgo'], 'execute_search') as mock_search:
            mock_search.return_value = {'results': []}
            
            result = self.web_search_tools.web_search(
                query="valid query",
                max_results=10
            )
            
            self.assertIsNotNone(result)
            
    def test_response_standardization_integration(self):
        """Test response standardization across different engines."""
        # Test with different engines to ensure consistent response format
        test_engines = ['duckduckgo', 'google', 'tavily']
        
        for engine in test_engines:
            if engine in self.web_search_tools.engines:
                with patch.object(self.web_search_tools.engines[engine], 'execute_search') as mock_search:
                    mock_search.return_value = {
                        'results': [
                            {
                                'title': f'{engine} Result',
                                'url': f'https://{engine}.com',
                                'snippet': f'{engine} snippet'
                            }
                        ]
                    }
                    
                    result = self.web_search_tools.web_search(
                        query="test query",
                        engine=engine
                    )
                    
                    # All engines should return same response structure
                    self.assertEqual(result.engine_used, engine)
                    self.assertTrue(hasattr(result, 'results'))
                    self.assertTrue(hasattr(result, 'success'))
                    self.assertTrue(hasattr(result, 'execution_time'))
                    
    def test_multi_search_type_integration(self):
        """Test different search types work correctly."""
        search_types = [
            ('web_search', 'web'),
            ('news_search', 'news'),
            ('trends_search', 'trends')
        ]
        
        for method_name, search_type in search_types:
            method = getattr(self.web_search_tools, method_name)
            
            # Mock appropriate engines
            with patch.object(self.web_search_tools, '_execute_search') as mock_execute:
                mock_execute.return_value = Mock(
                    success=True,
                    search_type=search_type,
                    results=[]
                )
                
                if method_name == 'web_search':
                    result = method(query="test query")
                elif method_name == 'news_search':
                    result = method(query="test news")
                elif method_name == 'trends_search':
                    result = method(query="test trends")
                
                self.assertEqual(result.search_type, search_type)
                self.assertTrue(result.success)
```

### 6.4 Performance Testing Strategy

#### Performance Benchmarks

```python
"""
Performance Testing Suite

Validates that the unified tool meets performance requirements.
"""

import time
import statistics
from concurrent.futures import ThreadPoolExecutor, as_completed
from agent_c_core.toolsets.web_search.web_search_tools import WebSearchTools

class PerformanceTestSuite:
    """Performance testing for WebSearchTools."""
    
    def __init__(self):
        self.web_search_tools = WebSearchTools()
        self.performance_data = {}
        
    def run_performance_tests(self) -> dict:
        """Run comprehensive performance tests."""
        test_results = {}
        
        # Single request performance
        test_results['single_request'] = self.test_single_request_performance()
        
        # Concurrent request performance
        test_results['concurrent_requests'] = self.test_concurrent_performance()
        
        # Engine comparison
        test_results['engine_comparison'] = self.test_engine_performance_comparison()
        
        # Memory usage
        test_results['memory_usage'] = self.test_memory_usage()
        
        # Cache performance
        test_results['cache_performance'] = self.test_cache_performance()
        
        return test_results
    
    def test_single_request_performance(self) -> dict:
        """Test single request performance."""
        test_queries = [
            "python programming tutorial",
            "machine learning algorithms",
            "web development best practices",
            "data science methods",
            "artificial intelligence trends"
        ]
        
        performance_data = {
            'response_times': [],
            'success_rate': 0,
            'average_time': 0,
            'median_time': 0,
            'p95_time': 0
        }
        
        successful_requests = 0
        
        for query in test_queries:
            start_time = time.time()
            
            try:
                result = self.web_search_tools.web_search(
                    query=query,
                    max_results=10
                )
                
                end_time = time.time()
                response_time = end_time - start_time
                
                performance_data['response_times'].append(response_time)
                
                if result.success:
                    successful_requests += 1
                    
            except Exception as e:
                print(f"Error with query '{query}': {e}")
        
        # Calculate statistics
        if performance_data['response_times']:
            performance_data['average_time'] = statistics.mean(performance_data['response_times'])
            performance_data['median_time'] = statistics.median(performance_data['response_times'])
            performance_data['p95_time'] = self._calculate_percentile(performance_data['response_times'], 95)
        
        performance_data['success_rate'] = successful_requests / len(test_queries)
        
        return performance_data
    
    def test_concurrent_performance(self) -> dict:
        """Test concurrent request performance."""
        concurrent_levels = [1, 5, 10, 20]
        test_query = "concurrent performance test"
        
        concurrency_results = {}
        
        for concurrent_level in concurrent_levels:
            start_time = time.time()
            successful_requests = 0
            
            with ThreadPoolExecutor(max_workers=concurrent_level) as executor:
                futures = [
                    executor.submit(self.web_search_tools.web_search, query=test_query, max_results=5)
                    for _ in range(concurrent_level)
                ]
                
                for future in as_completed(futures):
                    try:
                        result = future.result()
                        if result.success:
                            successful_requests += 1
                    except Exception as e:
                        print(f"Concurrent request failed: {e}")
            
            end_time = time.time()
            total_time = end_time - start_time
            
            concurrency_results[concurrent_level] = {
                'total_time': total_time,
                'requests_per_second': successful_requests / total_time,
                'success_rate': successful_requests / concurrent_level
            }
        
        return concurrency_results
    
    def test_engine_performance_comparison(self) -> dict:
        """Compare performance across different engines."""
        engines = ['duckduckgo', 'google', 'tavily']
        test_query = "performance comparison test"
        
        engine_performance = {}
        
        for engine in engines:
            if engine in self.web_search_tools.engines:
                response_times = []
                success_count = 0
                
                # Run multiple tests for each engine
                for _ in range(5):
                    start_time = time.time()
                    
                    try:
                        result = self.web_search_tools.web_search(
                            query=test_query,
                            engine=engine,
                            max_results=10
                        )
                        
                        end_time = time.time()
                        response_time = end_time - start_time
                        response_times.append(response_time)
                        
                        if result.success:
                            success_count += 1
                            
                    except Exception as e:
                        print(f"Error with {engine}: {e}")
                
                engine_performance[engine] = {
                    'average_time': statistics.mean(response_times) if response_times else 0,
                    'success_rate': success_count / 5,
                    'response_times': response_times
                }
        
        return engine_performance
    
    def test_memory_usage(self) -> dict:
        """Test memory usage patterns."""
        import psutil
        import gc
        
        process = psutil.Process()
        
        # Baseline memory
        gc.collect()
        baseline_memory = process.memory_info().rss / 1024 / 1024  # MB
        
        # Execute searches
        for i in range(100):
            self.web_search_tools.web_search(
                query=f"memory test query {i}",
                max_results=5
            )
        
        # Memory after searches
        gc.collect()
        after_memory = process.memory_info().rss / 1024 / 1024  # MB
        
        return {
            'baseline_memory_mb': baseline_memory,
            'after_searches_memory_mb': after_memory,
            'memory_increase_mb': after_memory - baseline_memory,
            'memory_per_search_kb': (after_memory - baseline_memory) * 1024 / 100
        }
    
    def test_cache_performance(self) -> dict:
        """Test caching performance improvements."""
        test_query = "cache performance test"
        
        # First request (no cache)
        start_time = time.time()
        result1 = self.web_search_tools.web_search(query=test_query, max_results=10)
        first_request_time = time.time() - start_time
        
        # Second request (with cache)
        start_time = time.time()
        result2 = self.web_search_tools.web_search(query=test_query, max_results=10)
        second_request_time = time.time() - start_time
        
        return {
            'first_request_time': first_request_time,
            'second_request_time': second_request_time,
            'cache_improvement': (first_request_time - second_request_time) / first_request_time * 100,
            'cache_hit': second_request_time < first_request_time
        }
    
    def _calculate_percentile(self, data: list, percentile: int) -> float:
        """Calculate percentile of data."""
        if not data:
            return 0
        
        sorted_data = sorted(data)
        index = (percentile / 100) * (len(sorted_data) - 1)
        
        if index.is_integer():
            return sorted_data[int(index)]
        else:
            lower_index = int(index)
            upper_index = lower_index + 1
            weight = index - lower_index
            return sorted_data[lower_index] * (1 - weight) + sorted_data[upper_index] * weight
```

### 6.5 Migration Testing Strategy

#### Backward Compatibility Tests

```python
"""
Migration and Backward Compatibility Tests

Ensures smooth transition from old tools to new interface.
"""

import unittest
import warnings
from unittest.mock import Mock, patch
from agent_c_core.toolsets.web_search.web_search_tools import WebSearchTools

class TestMigrationCompatibility(unittest.TestCase):
    """Test migration and backward compatibility."""
    
    def setUp(self):
        """Set up test fixtures."""
        self.web_search_tools = WebSearchTools()
        
    def test_old_tool_interface_compatibility(self):
        """Test that old tool interfaces still work via bridges."""
        # Test DuckDuckGo compatibility
        from agent_c_core.toolsets.web_search.bridges.duckduckgo_bridge import DuckDuckGoToolsBridge
        
        bridge = DuckDuckGoToolsBridge()
        
        # Should issue deprecation warning
        with warnings.catch_warnings(record=True) as w:
            warnings.simplefilter("always")
            
            with patch.object(bridge.web_search_tools, 'web_search') as mock_search:
                mock_search.return_value = Mock(success=True, results=[])
                
                result = bridge.web_search(
                    query="test query",
                    max_results=10,
                    safesearch="moderate"
                )
                
                # Should have issued deprecation warning
                self.assertEqual(len(w), 1)
                self.assertTrue(issubclass(w[0].category, DeprecationWarning))
                
                # Should have called new interface with correct parameters
                mock_search.assert_called_once_with(
                    query="test query",
                    engine="duckduckgo",
                    max_results=10,
                    safesearch="moderate"
                )
                
    def test_parameter_mapping_compatibility(self):
        """Test parameter mapping from old to new interface."""
        # Test NewsAPI parameter mapping
        from agent_c_core.toolsets.web_search.bridges.newsapi_bridge import NewsApiToolsBridge
        
        bridge = NewsApiToolsBridge()
        
        with patch.object(bridge.web_search_tools, 'news_search') as mock_search:
            mock_search.return_value = Mock(success=True, results=[])
            
            # Call with old parameters
            result = bridge.get_all_articles(
                q="test query",
                pageSize=20,
                sortBy="publishedAt",
                category="technology"
            )
            
            # Should map parameters correctly
            mock_search.assert_called_once_with(
                engine="newsapi",
                query="test query",
                max_results=20,
                sort="publishedAt",
                category="technology"
            )
            
    def test_response_format_compatibility(self):
        """Test that response formats remain compatible."""
        # Old tools might expect different response formats
        # New tool should provide backward-compatible responses
        
        with patch.object(self.web_search_tools, '_execute_search') as mock_execute:
            mock_execute.return_value = Mock(
                success=True,
                results=[
                    Mock(
                        title="Test Result",
                        url="https://example.com",
                        snippet="Test snippet"
                    )
                ]
            )
            
            result = self.web_search_tools.web_search(query="test query")
            
            # Should have all expected fields
            self.assertTrue(hasattr(result, 'success'))
            self.assertTrue(hasattr(result, 'results'))
            self.assertTrue(hasattr(result, 'engine_used'))
            
            # Results should have expected structure
            self.assertEqual(len(result.results), 1)
            self.assertEqual(result.results[0].title, "Test Result")
            
    def test_error_handling_compatibility(self):
        """Test that error handling remains compatible."""
        # Old tools might expect different error formats
        # New tool should handle errors in backward-compatible way
        
        with patch.object(self.web_search_tools, '_execute_search') as mock_execute:
            mock_execute.side_effect = Exception("Test error")
            
            with self.assertRaises(Exception):
                self.web_search_tools.web_search(query="test query")
            
    def test_configuration_migration(self):
        """Test that configuration migrates correctly."""
        # Test that old configuration files work with new tool
        # Test that API keys are properly detected
        
        with patch.dict('os.environ', {'SERPAPI_API_KEY': 'test_key'}):
            # Should detect API key
            self.assertTrue(self.web_search_tools.config_manager.is_engine_available('google'))
            
        with patch.dict('os.environ', {}, clear=True):
            # Should handle missing API key
            self.assertFalse(self.web_search_tools.config_manager.is_engine_available('google'))

class TestDataMigration(unittest.TestCase):
    """Test data migration and consistency."""
    
    def test_search_result_consistency(self):
        """Test that search results are consistent between old and new tools."""
        # Compare results from old and new tools
        # This would require actual API calls in integration environment
        pass
        
    def test_performance_regression(self):
        """Test that new tool doesn't have performance regression."""
        # Compare performance metrics
        # This would be part of the performance testing suite
        pass
        
    def test_feature_parity(self):
        """Test that all features from old tools are available in new tool."""
        # Check that all old tool methods have equivalent in new tool
        old_tool_methods = {
            'DuckDuckGoTools': ['web_search'],
            'GoogleSerpTools': ['get_search_results', 'get_news', 'get_flights', 'get_events'],
            'NewsApiTools': ['get_top_headlines', 'get_all_articles', 'get_sources'],
            'GoogleTrendsTools': ['get_google_trending_searches', 'get_google_trends_for_query'],
            'HackerNewsTools': ['get_top_stories', 'get_job_stories'],
            'SeekingAlphaTools': ['get_topk_trending_news'],
            'TavilyResearchTools': ['search_tavily'],
            'WikipediaTools': ['search_wiki']
        }
        
        # Map to new tool methods
        new_tool_methods = {
            'web_search': ['web_search'],
            'news_search': ['news_search'],
            'trends_search': ['trends_search'],
            'flight_search': ['flight_search'],
            'event_search': ['event_search'],
            'research_search': ['research_search'],
            'educational_search': ['educational_search'],
            'tech_community_search': ['tech_community_search'],
            'financial_search': ['financial_search']
        }
        
        # Verify all functionality is covered
        for tool, methods in old_tool_methods.items():
            for method in methods:
                # Each old method should map to at least one new method
                self.assertTrue(
                    any(method in new_methods for new_methods in new_tool_methods.values()),
                    f"Method {method} from {tool} not covered in new tool"
                )
```

### 6.6 Test Execution Plan

#### Automated Testing Pipeline

```yaml
# test_pipeline.yml
name: Web Search Tools Test Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  unit_tests:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        python-version: [3.8, 3.9, 3.10, 3.11]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python ${{ matrix.python-version }}
      uses: actions/setup-python@v4
      with:
        python-version: ${{ matrix.python-version }}
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install pytest pytest-cov pytest-mock
        pip install -r requirements.txt
    
    - name: Run unit tests
      run: |
        pytest tests/unit/ -v --cov=agent_c_core.toolsets.web_search --cov-report=xml
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage.xml

  integration_tests:
    runs-on: ubuntu-latest
    needs: unit_tests
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python 3.9
      uses: actions/setup-python@v4
      with:
        python-version: 3.9
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install pytest pytest-mock
        pip install -r requirements.txt
    
    - name: Run integration tests
      run: |
        pytest tests/integration/ -v
      env:
        SERPAPI_API_KEY: ${{ secrets.SERPAPI_API_KEY }}
        NEWSAPI_API_KEY: ${{ secrets.NEWSAPI_API_KEY }}
        TAVILI_API_KEY: ${{ secrets.TAVILI_API_KEY }}

  performance_tests:
    runs-on: ubuntu-latest
    needs: integration_tests
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python 3.9
      uses: actions/setup-python@v4
      with:
        python-version: 3.9
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install pytest pytest-benchmark
        pip install -r requirements.txt
    
    - name: Run performance tests
      run: |
        pytest tests/performance/ -v --benchmark-only
      env:
        SERPAPI_API_KEY: ${{ secrets.SERPAPI_API_KEY }}
        NEWSAPI_API_KEY: ${{ secrets.NEWSAPI_API_KEY }}
        TAVILI_API_KEY: ${{ secrets.TAVILI_API_KEY }}

  migration_tests:
    runs-on: ubuntu-latest
    needs: unit_tests
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python 3.9
      uses: actions/setup-python@v4
      with:
        python-version: 3.9
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install pytest pytest-mock
        pip install -r requirements.txt
    
    - name: Run migration tests
      run: |
        pytest tests/migration/ -v
      env:
        SERPAPI_API_KEY: ${{ secrets.SERPAPI_API_KEY }}
        NEWSAPI_API_KEY: ${{ secrets.NEWSAPI_API_KEY }}
        TAVILI_API_KEY: ${{ secrets.TAVILI_API_KEY }}
```

#### Manual Testing Checklist

```markdown
# Manual Testing Checklist

## Pre-Implementation Testing
- [ ] Verify all existing tools are working correctly
- [ ] Document current performance baselines
- [ ] Verify API keys are properly configured
- [ ] Test all search types across all engines

## Implementation Testing
- [ ] Verify new tool structure is correct
- [ ] Test configuration management
- [ ] Verify engine routing logic
- [ ] Test parameter validation
- [ ] Verify response standardization

## Integration Testing
- [ ] Test all search methods work correctly
- [ ] Verify engine fallback mechanisms
- [ ] Test error handling across all scenarios
- [ ] Verify caching mechanisms work
- [ ] Test concurrent request handling

## Migration Testing
- [ ] Verify backward compatibility bridges work
- [ ] Test parameter mapping correctness
- [ ] Verify response format compatibility
- [ ] Test deprecation warnings are issued
- [ ] Verify old tool removal doesn't break anything

## Performance Testing
- [ ] Verify response times meet requirements
- [ ] Test memory usage is within acceptable limits
- [ ] Verify concurrent request handling
- [ ] Test cache performance improvements
- [ ] Verify no performance regression

## User Acceptance Testing
- [ ] Test with real user scenarios
- [ ] Verify documentation is accurate
- [ ] Test migration guide effectiveness
- [ ] Verify error messages are helpful
- [ ] Test all edge cases

## Production Readiness
- [ ] Verify monitoring is in place
- [ ] Test logging is comprehensive
- [ ] Verify security measures are implemented
- [ ] Test backup and recovery procedures
- [ ] Verify rollback plan is executable
```

## 7. Implementation Timeline and Milestones

### 7.1 Phase 2 Implementation Schedule

**Total Duration**: 10 weeks  
**Team Size**: 2-3 developers  
**Testing**: Continuous throughout development  

#### Week 1-2: Foundation (Infrastructure)
- **Week 1**: Project setup and base infrastructure
  - Set up directory structure
  - Implement base classes (BaseEngine, ConfigurationManager)
  - Create testing framework
  - Set up CI/CD pipeline

- **Week 2**: Core utilities and validation
  - Implement parameter validation system
  - Create error handling framework
  - Build response standardization system
  - Implement caching layer

#### Week 3-4: Engine Development
- **Week 3**: Primary engines
  - Implement DuckDuckGo engine
  - Implement Google SERP engine
  - Implement Tavily engine
  - Create engine tests

- **Week 4**: Specialized engines
  - Implement NewsAPI engine
  - Implement Google Trends engine
  - Implement Hacker News engine
  - Implement Seeking Alpha engine
  - Implement Wikipedia engine

#### Week 5-6: Routing and Integration
- **Week 5**: Engine routing system
  - Implement engine router
  - Create availability checker
  - Build query analyzer
  - Implement fallback logic

- **Week 6**: Main interface development
  - Implement WebSearchTools class
  - Create all search methods
  - Add JSON schema decorators
  - Implement utility methods

#### Week 7-8: Testing and Optimization
- **Week 7**: Comprehensive testing
  - Complete unit test suite
  - Integration testing
  - Performance testing
  - Migration testing

- **Week 8**: Optimization and refinement
  - Performance optimization
  - Bug fixes
  - Documentation completion
  - Security audit

#### Week 9-10: Migration and Deployment
- **Week 9**: Migration preparation
  - Create bridge tools
  - Implement migration scripts
  - User acceptance testing
  - Documentation finalization

- **Week 10**: Deployment and validation
  - Production deployment
  - Migration execution
  - Final validation
  - Cleanup and monitoring setup

### 7.2 Success Criteria

#### Functional Requirements
- ✅ All 8 existing tools functionality preserved
- ✅ Unified interface with intelligent routing
- ✅ Comprehensive error handling with fallbacks
- ✅ Standardized response formats
- ✅ Parameter validation and sanitization
- ✅ Backward compatibility maintained

#### Performance Requirements
- ✅ Response times ≤ 2 seconds for 95% of requests
- ✅ Support for 100+ concurrent requests
- ✅ Memory usage increase ≤ 50% from baseline
- ✅ 99.9% uptime during migration period
- ✅ Cache hit ratio ≥ 30% for repeated queries

#### Quality Requirements
- ✅ Test coverage ≥ 90%
- ✅ All engines pass integration tests
- ✅ Zero critical security vulnerabilities
- ✅ Documentation completeness score ≥ 95%
- ✅ User acceptance rating ≥ 4.5/5

## 8. Risk Assessment and Mitigation

### 8.1 Technical Risks

#### High Risk: API Rate Limiting
**Risk**: Search engines impose rate limits causing service disruption
**Probability**: High  
**Impact**: High  
**Mitigation**: 
- Implement intelligent rate limiting
- Multiple fallback engines
- Caching layer to reduce API calls
- API key rotation where possible

#### Medium Risk: Engine API Changes
**Risk**: Search engines change their APIs breaking functionality
**Probability**: Medium  
**Impact**: High  
**Mitigation**: 
- Modular engine architecture for easy updates
- Comprehensive monitoring and alerting
- Multiple engines for each search type
- Version pinning for dependencies

#### Medium Risk: Performance Degradation
**Risk**: Unified tool performs worse than individual tools
**Probability**: Medium  
**Impact**: Medium  
**Mitigation**: 
- Extensive performance testing
- Caching implementation
- Connection pooling
- Async operation support

### 8.2 Business Risks

#### High Risk: User Adoption
**Risk**: Users resist migrating to new unified tool
**Probability**: Medium  
**Impact**: High  
**Mitigation**: 
- Comprehensive documentation
- Migration guide and support
- Backward compatibility bridges
- Gradual migration approach

#### Medium Risk: Migration Complexity
**Risk**: Migration process is more complex than anticipated
**Probability**: Medium  
**Impact**: Medium  
**Mitigation**: 
- Detailed migration plan
- Automated migration scripts
- Rollback procedures
- Parallel operation period

## 9. Conclusion

This technical specification provides comprehensive, implementation-ready details for Phase 2 of the web search consolidation project. The specification includes:

### 9.1 Key Deliverables

1. **Complete JSON Schemas**: Detailed parameter specifications for all 9 search methods
2. **Engine Routing Logic**: Intelligent routing algorithms with fallback mechanisms
3. **Method Signatures**: Type-hinted Python interfaces with exact parameter mappings
4. **File Structure**: Complete directory structure with specific file contents
5. **Replacement Strategy**: Step-by-step migration plan with bridge tools
6. **Testing Strategy**: Comprehensive testing approach covering all aspects

### 9.2 Implementation Benefits

- **Unified Interface**: Single tool instead of 8 separate tools
- **Intelligent Routing**: Automatic engine selection based on query analysis
- **Enhanced Reliability**: Fallback mechanisms and error handling
- **Improved Performance**: Caching and optimization strategies
- **Better Maintainability**: Modular architecture with clear separation of concerns
- **Future Extensibility**: Easy addition of new engines and search types

### 9.3 Next Steps

1. **Team Setup**: Assign development team and establish project structure
2. **Environment Preparation**: Set up development environment and CI/CD pipeline
3. **Phase 2 Execution**: Follow the detailed implementation plan
4. **Continuous Testing**: Implement testing strategy throughout development
5. **Migration Execution**: Execute migration plan with user support
6. **Post-Implementation**: Monitor, optimize, and maintain the unified tool

This specification ensures that the implementation team has all necessary details to successfully build and deploy the unified web search tool while maintaining quality, performance, and user satisfaction standards.