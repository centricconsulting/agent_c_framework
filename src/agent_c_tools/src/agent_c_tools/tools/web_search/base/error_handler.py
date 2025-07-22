"""
Error handling and fallback logic for web search operations.

This module provides comprehensive error handling, fallback strategies,
and recovery mechanisms for the unified web search system.
"""

import time
from typing import Dict, Any, List, Optional, Callable
from datetime import datetime, timedelta
from enum import Enum
from agent_c.util.structured_logging import get_logger

from .models import SearchResponse, SearchParameters, SearchType
from .engine import EngineException, EngineUnavailableException

logger = get_logger(__name__)


class ErrorSeverity(Enum):
    """Error severity levels for categorizing failures."""
    LOW = "low"           # Minor issues, can continue
    MEDIUM = "medium"     # Significant issues, may need fallback
    HIGH = "high"         # Critical issues, requires immediate attention
    CRITICAL = "critical" # System-level failures


class ErrorCategory(Enum):
    """Categories of errors that can occur."""
    NETWORK = "network"               # Network connectivity issues
    AUTHENTICATION = "authentication" # API key or auth problems
    RATE_LIMIT = "rate_limit"        # Rate limiting issues
    VALIDATION = "validation"         # Parameter validation errors
    ENGINE_ERROR = "engine_error"     # Engine-specific errors
    TIMEOUT = "timeout"              # Request timeout errors
    PARSING = "parsing"              # Response parsing errors
    CONFIGURATION = "configuration"   # Configuration issues
    UNKNOWN = "unknown"              # Unclassified errors


class ErrorHandler:
    """Handles errors and implements fallback strategies for web search operations."""
    
    def __init__(self, registry=None, router=None):
        """
        Initialize the error handler.
        
        Args:
            registry: Engine registry for fallback engine discovery
            router: Engine router for fallback routing
        """
        self.registry = registry
        self.router = router
        self.error_history: List[Dict[str, Any]] = []
        self.engine_failure_counts: Dict[str, int] = {}
        self.engine_last_failure: Dict[str, datetime] = {}
        self.circuit_breaker_thresholds = {
            'failure_count': 5,
            'time_window': timedelta(minutes=15),
            'recovery_time': timedelta(minutes=5)
        }
    
    def handle_search_error(
        self,
        error: Exception,
        params: SearchParameters,
        engine_name: str,
        execution_time: float,
        attempt_fallback: bool = True
    ) -> SearchResponse:
        """
        Handle a search error with appropriate fallback strategies.
        
        Args:
            error: The exception that occurred
            params: Original search parameters
            engine_name: Name of the engine that failed
            execution_time: Time spent before failure
            attempt_fallback: Whether to attempt fallback to other engines
            
        Returns:
            SearchResponse with error details or fallback results
        """
        # Categorize and log the error
        error_info = self._categorize_error(error, engine_name)
        self._log_error(error_info, params, execution_time)
        
        # Update failure tracking
        self._update_failure_tracking(engine_name, error_info)
        
        # Attempt fallback if enabled and appropriate
        if attempt_fallback and self._should_attempt_fallback(error_info, params):
            fallback_response = self._attempt_fallback_search(params, engine_name, error_info)
            if fallback_response and fallback_response.success:
                return fallback_response
        
        # Return error response if no fallback succeeded
        return self._create_error_response(error_info, params, engine_name, execution_time)
    
    def _categorize_error(self, error: Exception, engine_name: str) -> Dict[str, Any]:
        """
        Categorize an error to determine appropriate handling strategy.
        
        Args:
            error: The exception to categorize
            engine_name: Name of the engine that failed
            
        Returns:
            Dictionary with error categorization information
        """
        error_type = type(error).__name__
        error_message = str(error)
        
        # Determine category based on error type and message
        category = ErrorCategory.UNKNOWN
        severity = ErrorSeverity.MEDIUM
        is_retryable = False
        suggested_action = "fallback"
        
        # Network-related errors
        if any(keyword in error_message.lower() for keyword in [
            'connection', 'network', 'dns', 'timeout', 'unreachable'
        ]):
            category = ErrorCategory.NETWORK
            severity = ErrorSeverity.HIGH
            is_retryable = True
            suggested_action = "retry_with_delay"
        
        # Authentication errors
        elif any(keyword in error_message.lower() for keyword in [
            'unauthorized', 'forbidden', 'api key', 'authentication', 'invalid key'
        ]):
            category = ErrorCategory.AUTHENTICATION
            severity = ErrorSeverity.HIGH
            is_retryable = False
            suggested_action = "check_configuration"
        
        # Rate limiting
        elif any(keyword in error_message.lower() for keyword in [
            'rate limit', 'too many requests', 'quota exceeded', 'throttle'
        ]):
            category = ErrorCategory.RATE_LIMIT
            severity = ErrorSeverity.MEDIUM
            is_retryable = True
            suggested_action = "retry_with_backoff"
        
        # Timeout errors
        elif 'timeout' in error_type.lower() or 'timeout' in error_message.lower():
            category = ErrorCategory.TIMEOUT
            severity = ErrorSeverity.MEDIUM
            is_retryable = True
            suggested_action = "retry_with_longer_timeout"
        
        # Validation errors
        elif 'validation' in error_type.lower() or isinstance(error, ValueError):
            category = ErrorCategory.VALIDATION
            severity = ErrorSeverity.LOW
            is_retryable = False
            suggested_action = "fix_parameters"
        
        # Engine-specific errors
        elif isinstance(error, EngineException):
            category = ErrorCategory.ENGINE_ERROR
            severity = ErrorSeverity.MEDIUM
            is_retryable = isinstance(error, EngineUnavailableException)
            suggested_action = "fallback" if is_retryable else "check_engine_config"
        
        # Parsing errors
        elif any(keyword in error_type.lower() for keyword in [
            'json', 'parse', 'decode', 'format'
        ]):
            category = ErrorCategory.PARSING
            severity = ErrorSeverity.LOW
            is_retryable = True
            suggested_action = "retry_or_fallback"
        
        return {
            'error_type': error_type,
            'error_message': error_message,
            'category': category,
            'severity': severity,
            'is_retryable': is_retryable,
            'suggested_action': suggested_action,
            'engine_name': engine_name,
            'timestamp': datetime.now(),
            'original_exception': error
        }
    
    def _log_error(
        self, 
        error_info: Dict[str, Any], 
        params: SearchParameters, 
        execution_time: float
    ) -> None:
        """Log error information for monitoring and debugging."""
        # Map severity to log method
        severity_to_method = {
            ErrorSeverity.LOW: logger.info,
            ErrorSeverity.MEDIUM: logger.warning,
            ErrorSeverity.HIGH: logger.error,
            ErrorSeverity.CRITICAL: logger.critical
        }
        
        log_method = severity_to_method.get(error_info['severity'], logger.error)
        
        log_method(
            f"Search error: {error_info['category'].value} - "
            f"{error_info['error_type']} in {error_info['engine_name']} - "
            f"Query: '{params.query[:50]}...' - "
            f"Time: {execution_time:.2f}s - "
            f"Message: {error_info['error_message'][:200]}"
        )
        
        # Add to error history for analysis
        self.error_history.append({
            **error_info,
            'query': params.query,
            'search_type': params.search_type.value,
            'execution_time': execution_time
        })
        
        # Keep only recent errors (last 1000)
        if len(self.error_history) > 1000:
            self.error_history = self.error_history[-1000:]
    
    def _update_failure_tracking(self, engine_name: str, error_info: Dict[str, Any]) -> None:
        """Update failure tracking for circuit breaker logic."""
        now = datetime.now()
        
        # Increment failure count
        self.engine_failure_counts[engine_name] = self.engine_failure_counts.get(engine_name, 0) + 1
        self.engine_last_failure[engine_name] = now
        
        # Check if engine should be circuit-broken
        failure_count = self.engine_failure_counts[engine_name]
        if failure_count >= self.circuit_breaker_thresholds['failure_count']:
            logger.warning(
                f"Engine {engine_name} has {failure_count} failures, "
                f"may be circuit-broken for {self.circuit_breaker_thresholds['recovery_time']}"
            )
    
    def _should_attempt_fallback(
        self, 
        error_info: Dict[str, Any], 
        params: SearchParameters
    ) -> bool:
        """
        Determine if fallback should be attempted based on error characteristics.
        
        Args:
            error_info: Categorized error information
            params: Original search parameters
            
        Returns:
            True if fallback should be attempted
        """
        # Don't fallback for validation errors
        if error_info['category'] == ErrorCategory.VALIDATION:
            return False
        
        # Don't fallback if no registry/router available
        if not self.registry or not self.router:
            return False
        
        # Don't fallback if explicit engine was requested (unless it's unavailable)
        if (params.engine != "auto" and 
            error_info['category'] != ErrorCategory.ENGINE_ERROR):
            return False
        
        # Check if there are other available engines
        available_engines = self.registry.get_healthy_engines()
        available_for_search_type = [
            engine for engine in available_engines
            if self.registry.get_engine(engine).supports_search_type(params.search_type)
        ]
        
        # Remove the failed engine from consideration
        available_for_search_type = [
            engine for engine in available_for_search_type
            if engine != error_info['engine_name']
        ]
        
        return len(available_for_search_type) > 0
    
    def _attempt_fallback_search(
        self,
        params: SearchParameters,
        failed_engine: str,
        error_info: Dict[str, Any]
    ) -> Optional[SearchResponse]:
        """
        Attempt to execute the search using a fallback engine.
        
        Args:
            params: Original search parameters
            failed_engine: Name of the engine that failed
            error_info: Information about the failure
            
        Returns:
            SearchResponse from fallback engine or None if all fallbacks fail
        """
        try:
            # Get available engines excluding the failed one
            available_engines = self.registry.get_healthy_engines()
            available_engines = [e for e in available_engines if e != failed_engine]
            
            # Filter by search type support
            suitable_engines = []
            for engine_name in available_engines:
                engine = self.registry.get_engine(engine_name)
                if engine and engine.supports_search_type(params.search_type):
                    # Check circuit breaker status
                    if not self._is_engine_circuit_broken(engine_name):
                        suitable_engines.append(engine_name)
            
            if not suitable_engines:
                logger.warning("No suitable fallback engines available")
                return None
            
            # Create modified parameters for fallback
            fallback_params = SearchParameters(
                query=params.query,
                engine="auto",  # Let router choose from available engines
                search_type=params.search_type,
                max_results=params.max_results,
                safesearch=params.safesearch,
                language=params.language,
                region=params.region,
                include_images=params.include_images,
                include_domains=params.include_domains,
                exclude_domains=params.exclude_domains,
                search_depth=params.search_depth,
                start_date=params.start_date,
                end_date=params.end_date,
                page=params.page,
                additional_params=params.additional_params
            )
            
            # Route to fallback engine
            fallback_engine_name = self.router.route_search_request(
                fallback_params, suitable_engines
            )
            
            # Execute fallback search
            fallback_engine = self.registry.get_engine(fallback_engine_name)
            if fallback_engine:
                logger.info(f"Attempting fallback search with {fallback_engine_name}")
                response = fallback_engine.execute_search(fallback_params)
                
                # Add fallback metadata
                if response.success:
                    response.metadata = response.metadata or {}
                    response.metadata['fallback_info'] = {
                        'original_engine': failed_engine,
                        'fallback_engine': fallback_engine_name,
                        'original_error': error_info['error_message'],
                        'fallback_reason': error_info['category'].value
                    }
                    logger.info(f"Fallback search successful with {fallback_engine_name}")
                
                return response
            
        except Exception as e:
            logger.error(f"Fallback search failed: {e}")
        
        return None
    
    def _is_engine_circuit_broken(self, engine_name: str) -> bool:
        """
        Check if an engine is currently circuit-broken.
        
        Args:
            engine_name: Name of the engine to check
            
        Returns:
            True if engine is circuit-broken
        """
        now = datetime.now()
        
        # Check if engine has enough failures
        failure_count = self.engine_failure_counts.get(engine_name, 0)
        if failure_count < self.circuit_breaker_thresholds['failure_count']:
            return False
        
        # Check if enough time has passed for recovery
        last_failure = self.engine_last_failure.get(engine_name)
        if last_failure:
            time_since_failure = now - last_failure
            if time_since_failure > self.circuit_breaker_thresholds['recovery_time']:
                # Reset failure count for recovery attempt
                self.engine_failure_counts[engine_name] = 0
                logger.info(f"Engine {engine_name} circuit breaker reset - attempting recovery")
                return False
        
        return True
    
    def _create_error_response(
        self,
        error_info: Dict[str, Any],
        params: SearchParameters,
        engine_name: str,
        execution_time: float
    ) -> SearchResponse:
        """
        Create a standardized error response.
        
        Args:
            error_info: Categorized error information
            params: Original search parameters
            engine_name: Name of the failed engine
            execution_time: Time spent before failure
            
        Returns:
            SearchResponse with error details
        """
        return SearchResponse(
            success=False,
            engine_used=engine_name,
            search_type=params.search_type.value,
            query=params.query,
            execution_time=execution_time,
            results=[],
            error={
                'type': error_info['error_type'],
                'message': error_info['error_message'],
                'category': error_info['category'].value,
                'severity': error_info['severity'].value,
                'engine': engine_name,
                'is_retryable': error_info['is_retryable'],
                'suggested_action': error_info['suggested_action'],
                'timestamp': error_info['timestamp'].isoformat()
            }
        )
    
    def get_error_statistics(self) -> Dict[str, Any]:
        """
        Get error statistics for monitoring and analysis.
        
        Returns:
            Dictionary with error statistics
        """
        if not self.error_history:
            return {'total_errors': 0}
        
        now = datetime.now()
        recent_errors = [
            error for error in self.error_history
            if (now - error['timestamp']).total_seconds() < 3600  # Last hour
        ]
        
        # Count by category
        category_counts = {}
        for error in recent_errors:
            category = error['category'].value
            category_counts[category] = category_counts.get(category, 0) + 1
        
        # Count by engine
        engine_counts = {}
        for error in recent_errors:
            engine = error['engine_name']
            engine_counts[engine] = engine_counts.get(engine, 0) + 1
        
        # Count by severity
        severity_counts = {}
        for error in recent_errors:
            severity = error['severity'].value
            severity_counts[severity] = severity_counts.get(severity, 0) + 1
        
        return {
            'total_errors': len(self.error_history),
            'recent_errors_1h': len(recent_errors),
            'errors_by_category': category_counts,
            'errors_by_engine': engine_counts,
            'errors_by_severity': severity_counts,
            'circuit_broken_engines': [
                engine for engine in self.engine_failure_counts.keys()
                if self._is_engine_circuit_broken(engine)
            ],
            'failure_counts': self.engine_failure_counts.copy()
        }
    
    def reset_error_tracking(self, engine_name: Optional[str] = None) -> None:
        """
        Reset error tracking for an engine or all engines.
        
        Args:
            engine_name: Specific engine to reset, or None for all engines
        """
        if engine_name:
            self.engine_failure_counts.pop(engine_name, None)
            self.engine_last_failure.pop(engine_name, None)
            logger.info(f"Reset error tracking for engine: {engine_name}")
        else:
            self.engine_failure_counts.clear()
            self.engine_last_failure.clear()
            self.error_history.clear()
            logger.info("Reset error tracking for all engines")
    
    def get_engine_health_recommendations(self) -> Dict[str, List[str]]:
        """
        Get health recommendations based on error patterns.
        
        Returns:
            Dictionary mapping engine names to lists of recommendations
        """
        recommendations = {}
        
        for engine_name, failure_count in self.engine_failure_counts.items():
            engine_recommendations = []
            
            if failure_count >= self.circuit_breaker_thresholds['failure_count']:
                engine_recommendations.append("Engine is circuit-broken - check configuration and connectivity")
            elif failure_count >= 3:
                engine_recommendations.append("High failure rate - monitor engine health")
            
            # Analyze recent errors for this engine
            recent_engine_errors = [
                error for error in self.error_history[-100:]  # Last 100 errors
                if error['engine_name'] == engine_name
            ]
            
            if recent_engine_errors:
                # Find most common error categories
                category_counts = {}
                for error in recent_engine_errors:
                    category = error['category'].value
                    category_counts[category] = category_counts.get(category, 0) + 1
                
                most_common_category = max(category_counts, key=category_counts.get)
                
                if most_common_category == 'authentication':
                    engine_recommendations.append("Check API key configuration")
                elif most_common_category == 'rate_limit':
                    engine_recommendations.append("Consider implementing request throttling")
                elif most_common_category == 'network':
                    engine_recommendations.append("Check network connectivity and timeouts")
                elif most_common_category == 'timeout':
                    engine_recommendations.append("Consider increasing timeout values")
            
            if engine_recommendations:
                recommendations[engine_name] = engine_recommendations
        
        return recommendations