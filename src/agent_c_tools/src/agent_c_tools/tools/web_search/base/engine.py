"""
Abstract base class for web search engines.

This module defines the interface that all search engine implementations
must follow to ensure consistency and interoperability within the unified
web search system.
"""

from abc import ABC, abstractmethod
from typing import Dict, Any, List, Optional
import time
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from .models import (
    SearchResponse, SearchResult, SearchParameters, 
    WebSearchConfig, EngineHealthStatus, SearchType
)

logger = get_logger(__name__)


class BaseWebSearchEngine(ABC):
    """
    Abstract base class for all web search engines.
    
    All search engine implementations must inherit from this class and
    implement the required abstract methods to ensure consistent behavior
    across the unified web search system.
    """
    
    def __init__(self, config: WebSearchConfig):
        """
        Initialize the search engine with configuration.
        
        Args:
            config: WebSearchConfig object containing engine configuration
        """
        self.config = config
        self.engine_name = config.engine_name
        self.capabilities = config.capabilities
        self._last_health_check: Optional[datetime] = None
        self._health_status: Optional[EngineHealthStatus] = None
        
        # Initialize engine-specific setup
        self._initialize_engine()
    
    @abstractmethod
    def _initialize_engine(self) -> None:
        """
        Initialize engine-specific components.
        
        This method should set up any engine-specific clients, configurations,
        or other initialization requirements.
        """
        pass
    
    @abstractmethod
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute the actual search request.
        
        This method should implement the engine-specific search logic and
        return raw results that will be standardized by the response handler.
        
        Args:
            params: Validated and normalized search parameters
            
        Returns:
            Raw search results from the engine
            
        Raises:
            EngineException: If the search fails
        """
        pass
    
    @abstractmethod
    def _check_availability(self) -> bool:
        """
        Check if the engine is currently available.
        
        This method should perform a lightweight check to determine if the
        engine can handle requests (API keys configured, service reachable, etc.).
        
        Returns:
            True if engine is available, False otherwise
        """
        pass
    
    def execute_search(self, params: SearchParameters) -> SearchResponse:
        """
        Execute a search with standardized error handling and response formatting.
        
        This is the main entry point for search execution. It handles validation,
        error handling, and response standardization.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Standardized SearchResponse object
        """
        start_time = time.time()
        
        try:
            # Validate that engine supports the search type
            if not self.supports_search_type(params.search_type):
                raise EngineException(
                    f"Engine {self.engine_name} does not support search type: {params.search_type.value}"
                )
            
            # Check engine availability
            if not self.is_available():
                raise EngineException(f"Engine {self.engine_name} is not available")
            
            # Execute the search
            raw_results = self._execute_search(params)
            
            # Standardize the response
            response = self._standardize_response(raw_results, params, start_time)
            
            logger.info(
                f"Search completed successfully: engine={self.engine_name}, "
                f"query='{params.query}', results={len(response.results)}, "
                f"time={response.execution_time:.2f}s"
            )
            
            return response
            
        except Exception as e:
            execution_time = time.time() - start_time
            logger.error(
                f"Search failed: engine={self.engine_name}, "
                f"query='{params.query}', error={str(e)}"
            )
            
            return SearchResponse(
                success=False,
                engine_used=self.engine_name,
                search_type=params.search_type.value,
                query=params.query,
                execution_time=execution_time,
                results=[],
                error={
                    'type': type(e).__name__,
                    'message': str(e),
                    'engine': self.engine_name
                }
            )
    
    def _standardize_response(
        self, 
        raw_results: Dict[str, Any], 
        params: SearchParameters,
        start_time: float
    ) -> SearchResponse:
        """
        Convert raw engine results to standardized SearchResponse.
        
        This method should be overridden by engines that need custom
        response standardization logic.
        
        Args:
            raw_results: Raw results from the engine
            params: Original search parameters
            start_time: Search start time for execution time calculation
            
        Returns:
            Standardized SearchResponse object
        """
        execution_time = time.time() - start_time
        
        # Default standardization - engines should override if needed
        results = []
        if 'results' in raw_results:
            for item in raw_results['results']:
                result = SearchResult(
                    title=item.get('title', ''),
                    url=item.get('url', ''),
                    snippet=item.get('snippet', ''),
                    published_date=self._parse_date(item.get('published_date')),
                    score=item.get('score'),
                    source=item.get('source', self.engine_name),
                    metadata=item.get('metadata', {})
                )
                results.append(result)
        
        return SearchResponse(
            success=True,
            engine_used=self.engine_name,
            search_type=params.search_type.value,
            query=params.query,
            execution_time=execution_time,
            results=results,
            total_results=raw_results.get('total_results'),
            page=params.page,
            pages_available=raw_results.get('pages_available'),
            metadata=raw_results.get('metadata', {})
        )
    
    def _parse_date(self, date_str: Optional[str]) -> Optional[datetime]:
        """
        Parse date string to datetime object.
        
        Args:
            date_str: Date string in various formats
            
        Returns:
            Parsed datetime object or None if parsing fails
        """
        if not date_str:
            return None
            
        try:
            # Try common date formats
            formats = [
                '%Y-%m-%d',
                '%Y-%m-%dT%H:%M:%S',
                '%Y-%m-%dT%H:%M:%SZ',
                '%Y-%m-%d %H:%M:%S'
            ]
            
            for fmt in formats:
                try:
                    return datetime.strptime(date_str, fmt)
                except ValueError:
                    continue
                    
            logger.warning(f"Could not parse date: {date_str}")
            return None
            
        except Exception as e:
            logger.warning(f"Date parsing error: {e}")
            return None
    
    def is_available(self) -> bool:
        """
        Check if the engine is available for use.
        
        This method caches availability checks to avoid excessive API calls.
        
        Returns:
            True if engine is available, False otherwise
        """
        now = datetime.now()
        
        # Check cache validity (5 minute cache)
        if (self._last_health_check and 
            (now - self._last_health_check).total_seconds() < 300):
            return self._health_status.is_available if self._health_status else False
        
        # Perform fresh availability check
        try:
            is_available = self._check_availability()
            self._last_health_check = now
            self._health_status = EngineHealthStatus(
                engine_name=self.engine_name,
                is_available=is_available,
                last_check=now,
                api_key_configured=self.config.is_api_key_configured(),
                capabilities=self.capabilities
            )
            return is_available
            
        except Exception as e:
            logger.error(f"Availability check failed for {self.engine_name}: {e}")
            self._health_status = EngineHealthStatus(
                engine_name=self.engine_name,
                is_available=False,
                last_check=now,
                error_message=str(e),
                api_key_configured=self.config.is_api_key_configured(),
                capabilities=self.capabilities
            )
            return False
    
    def supports_search_type(self, search_type: SearchType) -> bool:
        """
        Check if the engine supports a specific search type.
        
        Args:
            search_type: The search type to check
            
        Returns:
            True if supported, False otherwise
        """
        return self.capabilities.supports_search_type(search_type)
    
    def get_health_status(self) -> EngineHealthStatus:
        """
        Get detailed health status of the engine.
        
        Returns:
            EngineHealthStatus object with current status
        """
        # Trigger availability check to refresh status
        self.is_available()
        return self._health_status or EngineHealthStatus(
            engine_name=self.engine_name,
            is_available=False,
            last_check=datetime.now(),
            error_message="Health status not initialized",
            capabilities=self.capabilities
        )
    
    def get_supported_parameters(self) -> Dict[str, Any]:
        """
        Get the parameters supported by this engine.
        
        Returns:
            Dictionary of supported parameters and their specifications
        """
        base_params = {
            'query': {'type': 'string', 'required': True},
            'max_results': {'type': 'integer', 'default': 10, 'min': 1, 'max': self.capabilities.max_results_per_request}
        }
        
        if self.capabilities.supports_safe_search:
            base_params['safesearch'] = {'type': 'string', 'enum': ['on', 'moderate', 'off']}
        
        if self.capabilities.supports_language_filtering:
            base_params['language'] = {'type': 'string', 'default': 'en'}
        
        if self.capabilities.supports_region_filtering:
            base_params['region'] = {'type': 'string', 'default': 'us'}
        
        if self.capabilities.supports_domain_filtering:
            base_params['include_domains'] = {'type': 'array', 'items': {'type': 'string'}}
            base_params['exclude_domains'] = {'type': 'array', 'items': {'type': 'string'}}
        
        if self.capabilities.supports_date_filtering:
            base_params['start_date'] = {'type': 'string', 'format': 'date'}
            base_params['end_date'] = {'type': 'string', 'format': 'date'}
        
        if self.capabilities.supports_pagination:
            base_params['page'] = {'type': 'integer', 'default': 1, 'min': 1}
        
        return base_params
    
    def __str__(self) -> str:
        """String representation of the engine."""
        return f"{self.__class__.__name__}(name={self.engine_name}, available={self.is_available()})"
    
    def __repr__(self) -> str:
        """Detailed string representation of the engine."""
        return (
            f"{self.__class__.__name__}("
            f"name={self.engine_name}, "
            f"available={self.is_available()}, "
            f"search_types={[st.value for st in self.capabilities.search_types]}"
            f")"
        )


class EngineException(Exception):
    """Base exception for engine-related errors."""
    
    def __init__(self, message: str, engine_name: Optional[str] = None):
        super().__init__(message)
        self.engine_name = engine_name


class EngineUnavailableException(EngineException):
    """Exception raised when an engine is unavailable."""
    pass


class EngineConfigurationException(EngineException):
    """Exception raised when an engine is misconfigured."""
    pass


class EngineTimeoutException(EngineException):
    """Exception raised when an engine request times out."""
    pass


class EngineRateLimitException(EngineException):
    """Exception raised when an engine rate limit is exceeded."""
    pass