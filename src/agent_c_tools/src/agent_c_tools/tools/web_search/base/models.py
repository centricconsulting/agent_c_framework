"""
Data models for standardized web search responses and configuration.

This module defines the core data structures used throughout the unified
web search system to ensure consistent response formats and configuration.
"""

from dataclasses import dataclass, field
from typing import Dict, List, Optional, Union, Any
from datetime import datetime
from enum import Enum


class SearchType(Enum):
    """Enumeration of supported search types."""
    WEB = "web"
    NEWS = "news"
    TRENDS = "trends"
    FLIGHTS = "flights"
    EVENTS = "events"
    RESEARCH = "research"
    EDUCATIONAL = "educational"
    TECH_COMMUNITY = "tech_community"
    FINANCIAL = "financial"


class SafeSearchLevel(Enum):
    """Safe search filtering levels."""
    OFF = "off"
    MODERATE = "moderate"
    ON = "on"


class SearchDepth(Enum):
    """Search depth levels for engines that support it."""
    BASIC = "basic"
    STANDARD = "standard"
    ADVANCED = "advanced"


@dataclass
class SearchResult:
    """Standard search result structure used across all engines."""
    
    title: str
    url: str
    snippet: str
    published_date: Optional[datetime] = None
    score: Optional[float] = None
    source: Optional[str] = None
    metadata: Optional[Dict[str, Any]] = field(default_factory=dict)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization."""
        return {
            'title': self.title,
            'url': self.url,
            'snippet': self.snippet,
            'published_date': self.published_date.isoformat() if self.published_date else None,
            'score': self.score,
            'source': self.source,
            'metadata': self.metadata or {}
        }


@dataclass
class SearchResponse:
    """Standard search response structure used across all engines."""
    
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
    metadata: Optional[Dict[str, Any]] = field(default_factory=dict)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization."""
        return {
            'success': self.success,
            'engine_used': self.engine_used,
            'search_type': self.search_type,
            'query': self.query,
            'execution_time': self.execution_time,
            'results': [result.to_dict() for result in self.results],
            'total_results': self.total_results,
            'page': self.page,
            'pages_available': self.pages_available,
            'error': self.error,
            'metadata': self.metadata or {}
        }


@dataclass
class EngineCapabilities:
    """Defines what capabilities an engine supports."""
    
    search_types: List[SearchType]
    supports_pagination: bool = False
    supports_date_filtering: bool = False
    supports_domain_filtering: bool = False
    supports_safe_search: bool = False
    supports_language_filtering: bool = False
    supports_region_filtering: bool = False
    supports_image_search: bool = False
    supports_content_extraction: bool = False
    max_results_per_request: int = 100
    rate_limit_per_minute: Optional[int] = None
    
    def supports_search_type(self, search_type: Union[str, SearchType]) -> bool:
        """Check if engine supports a specific search type."""
        if isinstance(search_type, str):
            try:
                search_type = SearchType(search_type)
            except ValueError:
                return False
        return search_type in self.search_types


@dataclass
class WebSearchConfig:
    """Configuration for web search engines."""
    
    engine_name: str
    requires_api_key: bool = False
    api_key_name: Optional[str] = None
    base_url: Optional[str] = None
    timeout: int = 30
    max_retries: int = 3
    retry_delay: float = 1.0
    capabilities: Optional[EngineCapabilities] = None
    default_parameters: Optional[Dict[str, Any]] = field(default_factory=dict)
    health_check_url: Optional[str] = None
    cache_ttl: int = 300  # 5 minutes default cache
    
    def __post_init__(self):
        """Initialize default capabilities if not provided."""
        if self.capabilities is None:
            self.capabilities = EngineCapabilities(
                search_types=[SearchType.WEB]
            )
    
    def is_api_key_configured(self) -> bool:
        """Check if required API key is configured."""
        if not self.requires_api_key:
            return True
        
        if not self.api_key_name:
            return False
            
        import os
        return bool(os.getenv(self.api_key_name))
    
    def get_api_key(self) -> Optional[str]:
        """Get the configured API key."""
        if not self.requires_api_key or not self.api_key_name:
            return None
            
        import os
        return os.getenv(self.api_key_name)


@dataclass
class SearchParameters:
    """Validated and normalized search parameters."""
    
    query: str
    engine: str = "auto"
    search_type: SearchType = SearchType.WEB
    max_results: int = 10
    safesearch: SafeSearchLevel = SafeSearchLevel.MODERATE
    language: str = "en"
    region: str = "us"
    include_images: bool = False
    include_domains: Optional[List[str]] = None
    exclude_domains: Optional[List[str]] = None
    search_depth: SearchDepth = SearchDepth.STANDARD
    start_date: Optional[datetime] = None
    end_date: Optional[datetime] = None
    page: int = 1
    additional_params: Optional[Dict[str, Any]] = field(default_factory=dict)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for engine consumption."""
        result = {
            'query': self.query,
            'engine': self.engine,
            'search_type': self.search_type.value,
            'max_results': self.max_results,
            'safesearch': self.safesearch.value,
            'language': self.language,
            'region': self.region,
            'include_images': self.include_images,
            'search_depth': self.search_depth.value,
            'page': self.page
        }
        
        if self.include_domains:
            result['include_domains'] = self.include_domains
        if self.exclude_domains:
            result['exclude_domains'] = self.exclude_domains
        if self.start_date:
            result['start_date'] = self.start_date.isoformat()
        if self.end_date:
            result['end_date'] = self.end_date.isoformat()
        if self.additional_params:
            result.update(self.additional_params)
            
        return result


@dataclass
class EngineHealthStatus:
    """Health status information for an engine."""
    
    engine_name: str
    is_available: bool
    last_check: datetime
    response_time: Optional[float] = None
    error_message: Optional[str] = None
    api_key_configured: bool = False
    capabilities: Optional[EngineCapabilities] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization."""
        return {
            'engine_name': self.engine_name,
            'is_available': self.is_available,
            'last_check': self.last_check.isoformat(),
            'response_time': self.response_time,
            'error_message': self.error_message,
            'api_key_configured': self.api_key_configured,
            'capabilities': {
                'search_types': [st.value for st in self.capabilities.search_types],
                'supports_pagination': self.capabilities.supports_pagination,
                'supports_date_filtering': self.capabilities.supports_date_filtering,
                'supports_domain_filtering': self.capabilities.supports_domain_filtering,
                'supports_safe_search': self.capabilities.supports_safe_search,
                'max_results_per_request': self.capabilities.max_results_per_request
            } if self.capabilities else None
        }