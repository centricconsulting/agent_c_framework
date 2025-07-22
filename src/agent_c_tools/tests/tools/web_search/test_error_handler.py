"""
Unit tests for error handling in the web search system.
"""
import pytest
from unittest.mock import Mock, patch
from datetime import datetime, timedelta
from typing import Dict, Any

from base.error_handler import ErrorHandler
from base.engine import (
    EngineException, EngineUnavailableException, EngineConfigurationException,
    EngineTimeoutException, EngineRateLimitException
)
from base.models import SearchParameters, SearchType


class TestErrorHandler:
    """Test suite for ErrorHandler class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.error_handler = ErrorHandler()
        self.available_engines = ["duckduckgo", "google_serp", "tavily", "wikipedia"]
    
    def test_error_handler_initialization(self):
        """Test ErrorHandler initialization."""
        handler = ErrorHandler()
        assert handler is not None
    
    def test_handle_engine_exception(self):
        """Test handling of generic engine exceptions."""
        error = EngineException("Search failed", engine_name="duckduckgo")
        
        result = self.error_handler.handle_engine_error(
            error, "duckduckgo", self.available_engines
        )
        
        assert result is not None
        assert "error" in result
        assert result["error"]["type"] == "EngineException"
        assert result["error"]["engine"] == "duckduckgo"
        assert "Search failed" in result["error"]["message"]
    
    def test_handle_engine_unavailable_exception(self):
        """Test handling of engine unavailable exceptions."""
        error = EngineUnavailableException("Engine not available")
        
        result = self.error_handler.handle_engine_error(
            error, "google_serp", self.available_engines
        )
        
        assert result is not None
        assert result["error"]["type"] == "EngineUnavailableException"
        assert "fallback_engine" in result
        assert result["fallback_engine"] in self.available_engines
        assert result["fallback_engine"] != "google_serp"
    
    def test_handle_engine_configuration_exception(self):
        """Test handling of engine configuration exceptions."""
        error = EngineConfigurationException("API key missing")
        
        result = self.error_handler.handle_engine_error(
            error, "google_serp", self.available_engines
        )
        
        assert result is not None
        assert result["error"]["type"] == "EngineConfigurationException"
        assert "API key" in result["error"]["message"]
        assert "configuration_help" in result
    
    def test_handle_engine_timeout_exception(self):
        """Test handling of engine timeout exceptions."""
        error = EngineTimeoutException("Request timed out")
        
        result = self.error_handler.handle_engine_error(
            error, "tavily", self.available_engines
        )
        
        assert result is not None
        assert result["error"]["type"] == "EngineTimeoutException"
        assert "retry_recommended" in result
        assert result["retry_recommended"] is True
    
    def test_handle_engine_rate_limit_exception(self):
        """Test handling of engine rate limit exceptions."""
        error = EngineRateLimitException("Rate limit exceeded")
        
        result = self.error_handler.handle_engine_error(
            error, "news_api", self.available_engines
        )
        
        assert result is not None
        assert result["error"]["type"] == "EngineRateLimitException"
        assert "retry_after" in result
        assert "fallback_engine" in result
    
    def test_should_retry_logic(self):
        """Test retry logic for different error types."""
        # Timeout errors should be retryable
        timeout_error = EngineTimeoutException("Timeout")
        assert self.error_handler.should_retry(timeout_error, attempt=1)
        
        # Rate limit errors should be retryable with delay
        rate_limit_error = EngineRateLimitException("Rate limit")
        assert self.error_handler.should_retry(rate_limit_error, attempt=1)
        
        # Configuration errors should not be retryable
        config_error = EngineConfigurationException("Bad config")
        assert not self.error_handler.should_retry(config_error, attempt=1)
        
        # Generic errors should be retryable up to a limit
        generic_error = EngineException("Generic error")
        assert self.error_handler.should_retry(generic_error, attempt=1)
        assert self.error_handler.should_retry(generic_error, attempt=2)
        assert not self.error_handler.should_retry(generic_error, attempt=4)
    
    def test_get_fallback_engine(self):
        """Test fallback engine selection."""
        # Test fallback for unavailable engine
        fallback = self.error_handler.get_fallback_engine(
            "google_serp", self.available_engines, SearchType.WEB
        )
        assert fallback in self.available_engines
        assert fallback != "google_serp"
        
        # Test fallback for search type optimization
        fallback = self.error_handler.get_fallback_engine(
            "news_api", self.available_engines, SearchType.EDUCATIONAL
        )
        assert fallback == "wikipedia"  # Should prefer Wikipedia for educational
        
        # Test fallback when no engines available
        fallback = self.error_handler.get_fallback_engine(
            "duckduckgo", [], SearchType.WEB
        )
        assert fallback is None
    
    def test_get_retry_delay(self):
        """Test retry delay calculation."""
        # Test exponential backoff
        delay1 = self.error_handler.get_retry_delay(1)
        delay2 = self.error_handler.get_retry_delay(2)
        delay3 = self.error_handler.get_retry_delay(3)
        
        assert delay1 < delay2 < delay3
        assert all(delay > 0 for delay in [delay1, delay2, delay3])
        
        # Test rate limit specific delay
        rate_limit_error = EngineRateLimitException("Rate limit")
        delay = self.error_handler.get_retry_delay(1, rate_limit_error)
        assert delay >= 60  # Should be at least 1 minute for rate limits
    
    def test_circuit_breaker_functionality(self):
        """Test circuit breaker pattern for failing engines."""
        engine_name = "failing_engine"
        
        # Simulate multiple failures
        for i in range(5):
            error = EngineException(f"Error {i}")
            self.error_handler.record_engine_failure(engine_name, error)
        
        # Engine should be circuit broken
        assert self.error_handler.is_engine_circuit_broken(engine_name)
        
        # Should not retry circuit broken engine
        assert not self.error_handler.should_retry_engine(engine_name)
        
        # Circuit should reset after time
        with patch('time.time', return_value=time.time() + 3600):  # 1 hour later
            assert not self.error_handler.is_engine_circuit_broken(engine_name)
    
    def test_error_categorization(self):
        """Test error categorization for better handling."""
        # Test network errors
        network_error = EngineException("Connection failed")
        category = self.error_handler.categorize_error(network_error)
        assert category == "network"
        
        # Test authentication errors
        auth_error = EngineConfigurationException("Invalid API key")
        category = self.error_handler.categorize_error(auth_error)
        assert category == "authentication"
        
        # Test rate limit errors
        rate_error = EngineRateLimitException("Too many requests")
        category = self.error_handler.categorize_error(rate_error)
        assert category == "rate_limit"
        
        # Test timeout errors
        timeout_error = EngineTimeoutException("Request timeout")
        category = self.error_handler.categorize_error(timeout_error)
        assert category == "timeout"
    
    def test_error_statistics_tracking(self):
        """Test error statistics tracking."""
        engine_name = "test_engine"
        
        # Record various errors
        self.error_handler.record_engine_failure(
            engine_name, EngineException("Error 1")
        )
        self.error_handler.record_engine_failure(
            engine_name, EngineTimeoutException("Timeout")
        )
        self.error_handler.record_engine_failure(
            engine_name, EngineRateLimitException("Rate limit")
        )
        
        # Get statistics
        stats = self.error_handler.get_engine_error_stats(engine_name)
        
        assert stats["total_errors"] == 3
        assert stats["error_types"]["EngineException"] == 1
        assert stats["error_types"]["EngineTimeoutException"] == 1
        assert stats["error_types"]["EngineRateLimitException"] == 1
        assert stats["error_rate"] > 0
    
    def test_error_recovery_suggestions(self):
        """Test error recovery suggestions."""
        # Test configuration error suggestions
        config_error = EngineConfigurationException("Missing API key")
        suggestions = self.error_handler.get_recovery_suggestions(
            config_error, "google_serp"
        )
        
        assert "api_key" in suggestions
        assert "configuration" in suggestions
        
        # Test timeout error suggestions
        timeout_error = EngineTimeoutException("Request timeout")
        suggestions = self.error_handler.get_recovery_suggestions(
            timeout_error, "tavily"
        )
        
        assert "retry" in suggestions
        assert "timeout" in suggestions
        
        # Test rate limit error suggestions
        rate_error = EngineRateLimitException("Rate limit exceeded")
        suggestions = self.error_handler.get_recovery_suggestions(
            rate_error, "news_api"
        )
        
        assert "rate_limit" in suggestions
        assert "alternative_engine" in suggestions
    
    def test_error_context_preservation(self):
        """Test that error context is preserved for debugging."""
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=10
        )
        
        error = EngineException("Search failed")
        
        result = self.error_handler.handle_engine_error(
            error, "duckduckgo", self.available_engines, search_params=params
        )
        
        assert "context" in result
        assert result["context"]["query"] == "test query"
        assert result["context"]["engine"] == "duckduckgo"
        assert result["context"]["search_type"] == "web"
    
    def test_error_aggregation(self):
        """Test error aggregation across engines."""
        # Record errors for multiple engines
        engines = ["duckduckgo", "google_serp", "tavily"]
        
        for engine in engines:
            for i in range(3):
                error = EngineException(f"Error {i}")
                self.error_handler.record_engine_failure(engine, error)
        
        # Get aggregated statistics
        aggregate_stats = self.error_handler.get_aggregate_error_stats()
        
        assert aggregate_stats["total_errors"] == 9
        assert len(aggregate_stats["engines"]) == 3
        assert all(stats["total_errors"] == 3 for stats in aggregate_stats["engines"].values())
    
    def test_error_severity_assessment(self):
        """Test error severity assessment."""
        # Test critical errors
        critical_error = EngineConfigurationException("Invalid configuration")
        severity = self.error_handler.assess_error_severity(critical_error)
        assert severity == "critical"
        
        # Test warning errors
        warning_error = EngineTimeoutException("Request timeout")
        severity = self.error_handler.assess_error_severity(warning_error)
        assert severity == "warning"
        
        # Test info errors
        info_error = EngineRateLimitException("Rate limit")
        severity = self.error_handler.assess_error_severity(info_error)
        assert severity == "info"
    
    def test_error_notification_system(self):
        """Test error notification system."""
        # Test that critical errors trigger notifications
        critical_error = EngineConfigurationException("Critical failure")
        
        with patch.object(self.error_handler, '_send_error_notification') as mock_notify:
            self.error_handler.handle_engine_error(
                critical_error, "google_serp", self.available_engines
            )
            
            mock_notify.assert_called_once()
            call_args = mock_notify.call_args[0]
            assert call_args[0] == "critical"
            assert "Critical failure" in call_args[1]
    
    def test_error_recovery_workflow(self):
        """Test complete error recovery workflow."""
        params = SearchParameters(
            query="test query",
            engine="failing_engine",
            search_type=SearchType.WEB
        )
        
        # Simulate engine failure
        error = EngineUnavailableException("Engine down")
        
        # Handle error and get recovery plan
        result = self.error_handler.handle_engine_error(
            error, "failing_engine", self.available_engines, search_params=params
        )
        
        # Verify recovery plan
        assert "fallback_engine" in result
        assert "retry_recommended" in result
        assert "recovery_steps" in result
        
        # Verify fallback engine is suitable
        fallback = result["fallback_engine"]
        assert fallback in self.available_engines
        assert fallback != "failing_engine"
        
        # Verify recovery steps are actionable
        steps = result["recovery_steps"]
        assert len(steps) > 0
        assert all("action" in step for step in steps)
    
    def test_error_logging_integration(self):
        """Test integration with logging system."""
        error = EngineException("Test error for logging")
        
        with patch('logging.Logger.error') as mock_log:
            self.error_handler.handle_engine_error(
                error, "duckduckgo", self.available_engines
            )
            
            mock_log.assert_called()
            log_call = mock_log.call_args[0][0]
            assert "Test error for logging" in log_call
            assert "duckduckgo" in log_call