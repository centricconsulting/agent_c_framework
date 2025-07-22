"""
Unit tests for engine registry in the web search system.
"""
import pytest
from unittest.mock import Mock, patch
from typing import List, Dict, Any

from base.registry import EngineRegistry
from base.engine import BaseWebSearchEngine, EngineUnavailableException
from base.models import WebSearchConfig, EngineCapabilities, SearchType


class TestEngineRegistry:
    """Test suite for EngineRegistry class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.registry = EngineRegistry()
    
    def test_registry_initialization(self):
        """Test EngineRegistry initialization."""
        registry = EngineRegistry()
        assert registry is not None
        assert hasattr(registry, '_engines')
        assert hasattr(registry, '_engine_configs')
    
    def test_register_engine(self):
        """Test engine registration."""
        # Create mock engine
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        
        # Register engine
        self.registry.register_engine(mock_engine)
        
        # Verify registration
        assert "test_engine" in self.registry.get_available_engines()
        assert self.registry.get_engine("test_engine") == mock_engine
    
    def test_register_duplicate_engine(self):
        """Test registration of duplicate engine names."""
        # Create mock engines with same name
        mock_engine1 = Mock(spec=BaseWebSearchEngine)
        mock_engine1.name = "duplicate_engine"
        mock_engine1.is_available.return_value = True
        
        mock_engine2 = Mock(spec=BaseWebSearchEngine)
        mock_engine2.name = "duplicate_engine"
        mock_engine2.is_available.return_value = True
        
        # Register first engine
        self.registry.register_engine(mock_engine1)
        
        # Registering second engine should replace first
        self.registry.register_engine(mock_engine2)
        
        # Verify second engine is registered
        assert self.registry.get_engine("duplicate_engine") == mock_engine2
    
    def test_unregister_engine(self):
        """Test engine unregistration."""
        # Register engine
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        
        self.registry.register_engine(mock_engine)
        assert "test_engine" in self.registry.get_available_engines()
        
        # Unregister engine
        self.registry.unregister_engine("test_engine")
        assert "test_engine" not in self.registry.get_available_engines()
    
    def test_get_engine_not_found(self):
        """Test getting non-existent engine."""
        with pytest.raises(KeyError):
            self.registry.get_engine("non_existent_engine")
    
    def test_get_available_engines(self):
        """Test getting list of available engines."""
        # Register multiple engines
        engines = []
        for i in range(3):
            mock_engine = Mock(spec=BaseWebSearchEngine)
            mock_engine.name = f"engine_{i}"
            mock_engine.is_available.return_value = True
            engines.append(mock_engine)
            self.registry.register_engine(mock_engine)
        
        available = self.registry.get_available_engines()
        assert len(available) == 3
        assert all(f"engine_{i}" in available for i in range(3))
    
    def test_get_available_engines_filters_unavailable(self):
        """Test that unavailable engines are filtered out."""
        # Register available engine
        available_engine = Mock(spec=BaseWebSearchEngine)
        available_engine.name = "available_engine"
        available_engine.is_available.return_value = True
        self.registry.register_engine(available_engine)
        
        # Register unavailable engine
        unavailable_engine = Mock(spec=BaseWebSearchEngine)
        unavailable_engine.name = "unavailable_engine"
        unavailable_engine.is_available.return_value = False
        self.registry.register_engine(unavailable_engine)
        
        available = self.registry.get_available_engines()
        assert "available_engine" in available
        assert "unavailable_engine" not in available
    
    def test_is_engine_available(self):
        """Test checking engine availability."""
        # Register available engine
        available_engine = Mock(spec=BaseWebSearchEngine)
        available_engine.name = "available_engine"
        available_engine.is_available.return_value = True
        self.registry.register_engine(available_engine)
        
        # Register unavailable engine
        unavailable_engine = Mock(spec=BaseWebSearchEngine)
        unavailable_engine.name = "unavailable_engine"
        unavailable_engine.is_available.return_value = False
        self.registry.register_engine(unavailable_engine)
        
        assert self.registry.is_engine_available("available_engine")
        assert not self.registry.is_engine_available("unavailable_engine")
        assert not self.registry.is_engine_available("non_existent_engine")
    
    def test_get_engines_by_search_type(self):
        """Test getting engines by supported search type."""
        # Create engines with different capabilities
        web_engine = Mock(spec=BaseWebSearchEngine)
        web_engine.name = "web_engine"
        web_engine.is_available.return_value = True
        web_engine.supports_search_type.return_value = True
        web_engine.capabilities = Mock()
        web_engine.capabilities.supported_search_types = [SearchType.WEB]
        
        news_engine = Mock(spec=BaseWebSearchEngine)
        news_engine.name = "news_engine"
        news_engine.is_available.return_value = True
        news_engine.supports_search_type.return_value = True
        news_engine.capabilities = Mock()
        news_engine.capabilities.supported_search_types = [SearchType.NEWS]
        
        multi_engine = Mock(spec=BaseWebSearchEngine)
        multi_engine.name = "multi_engine"
        multi_engine.is_available.return_value = True
        multi_engine.supports_search_type.return_value = True
        multi_engine.capabilities = Mock()
        multi_engine.capabilities.supported_search_types = [SearchType.WEB, SearchType.NEWS]
        
        # Register engines
        self.registry.register_engine(web_engine)
        self.registry.register_engine(news_engine)
        self.registry.register_engine(multi_engine)
        
        # Test getting engines by search type
        web_engines = self.registry.get_engines_by_search_type(SearchType.WEB)
        news_engines = self.registry.get_engines_by_search_type(SearchType.NEWS)
        
        assert "web_engine" in web_engines
        assert "multi_engine" in web_engines
        assert "news_engine" not in web_engines
        
        assert "news_engine" in news_engines
        assert "multi_engine" in news_engines
        assert "web_engine" not in news_engines
    
    def test_get_engine_capabilities(self):
        """Test getting engine capabilities."""
        # Create engine with capabilities
        mock_capabilities = Mock(spec=EngineCapabilities)
        mock_capabilities.supported_search_types = [SearchType.WEB]
        mock_capabilities.max_results = 100
        mock_capabilities.requires_api_key = False
        
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        mock_engine.capabilities = mock_capabilities
        
        self.registry.register_engine(mock_engine)
        
        capabilities = self.registry.get_engine_capabilities("test_engine")
        assert capabilities == mock_capabilities
        assert capabilities.max_results == 100
        assert not capabilities.requires_api_key
    
    def test_get_engine_capabilities_not_found(self):
        """Test getting capabilities for non-existent engine."""
        with pytest.raises(KeyError):
            self.registry.get_engine_capabilities("non_existent_engine")
    
    def test_discover_engines(self):
        """Test automatic engine discovery."""
        # Mock engine discovery
        with patch('base.registry.EngineRegistry._discover_available_engines') as mock_discover:
            mock_engines = [
                Mock(spec=BaseWebSearchEngine, name="discovered_engine_1"),
                Mock(spec=BaseWebSearchEngine, name="discovered_engine_2")
            ]
            mock_discover.return_value = mock_engines
            
            registry = EngineRegistry()
            registry.discover_engines()
            
            # Verify engines were discovered and registered
            mock_discover.assert_called_once()
            assert len(registry._engines) == 2
    
    def test_refresh_engine_availability(self):
        """Test refreshing engine availability status."""
        # Register engine that becomes unavailable
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        
        self.registry.register_engine(mock_engine)
        assert self.registry.is_engine_available("test_engine")
        
        # Engine becomes unavailable
        mock_engine.is_available.return_value = False
        
        # Refresh availability
        self.registry.refresh_engine_availability()
        
        # Should now be unavailable
        assert not self.registry.is_engine_available("test_engine")
    
    def test_get_engine_health_status(self):
        """Test getting engine health status."""
        # Create engine with health status
        mock_health = Mock()
        mock_health.is_available = True
        mock_health.response_time = 0.5
        mock_health.error_rate = 0.01
        
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        mock_engine.get_health_status.return_value = mock_health
        
        self.registry.register_engine(mock_engine)
        
        health = self.registry.get_engine_health_status("test_engine")
        assert health == mock_health
        assert health.is_available
        assert health.response_time == 0.5
        assert health.error_rate == 0.01
    
    def test_get_registry_statistics(self):
        """Test getting registry statistics."""
        # Register multiple engines
        for i in range(3):
            mock_engine = Mock(spec=BaseWebSearchEngine)
            mock_engine.name = f"engine_{i}"
            mock_engine.is_available.return_value = i < 2  # First 2 available
            self.registry.register_engine(mock_engine)
        
        stats = self.registry.get_registry_statistics()
        
        assert stats["total_engines"] == 3
        assert stats["available_engines"] == 2
        assert stats["unavailable_engines"] == 1
        assert len(stats["engine_list"]) == 3
    
    def test_validate_engine_configuration(self):
        """Test engine configuration validation."""
        # Create engine with configuration requirements
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        mock_engine.capabilities = Mock()
        mock_engine.capabilities.requires_api_key = True
        
        self.registry.register_engine(mock_engine)
        
        # Test validation with missing API key
        with patch.dict('os.environ', {}, clear=True):
            is_valid = self.registry.validate_engine_configuration("test_engine")
            assert not is_valid
        
        # Test validation with API key present
        with patch.dict('os.environ', {'TEST_ENGINE_API_KEY': 'test_key'}):
            is_valid = self.registry.validate_engine_configuration("test_engine")
            assert is_valid
    
    def test_get_engine_by_priority(self):
        """Test getting engines ordered by priority."""
        # Register engines with different priorities
        engines = []
        for i in range(3):
            mock_engine = Mock(spec=BaseWebSearchEngine)
            mock_engine.name = f"engine_{i}"
            mock_engine.is_available.return_value = True
            mock_engine.priority = i  # Lower number = higher priority
            engines.append(mock_engine)
            self.registry.register_engine(mock_engine)
        
        # Get engines by priority
        prioritized = self.registry.get_engines_by_priority()
        
        # Should be ordered by priority (ascending)
        assert prioritized[0] == "engine_0"
        assert prioritized[1] == "engine_1"
        assert prioritized[2] == "engine_2"
    
    def test_engine_load_balancing(self):
        """Test engine load balancing functionality."""
        # Register multiple engines
        engines = []
        for i in range(3):
            mock_engine = Mock(spec=BaseWebSearchEngine)
            mock_engine.name = f"engine_{i}"
            mock_engine.is_available.return_value = True
            mock_engine.get_load_factor.return_value = 0.5  # 50% load
            engines.append(mock_engine)
            self.registry.register_engine(mock_engine)
        
        # Get least loaded engine
        least_loaded = self.registry.get_least_loaded_engine()
        assert least_loaded in [f"engine_{i}" for i in range(3)]
    
    def test_engine_failover(self):
        """Test engine failover functionality."""
        # Register primary and backup engines
        primary_engine = Mock(spec=BaseWebSearchEngine)
        primary_engine.name = "primary_engine"
        primary_engine.is_available.return_value = False  # Unavailable
        
        backup_engine = Mock(spec=BaseWebSearchEngine)
        backup_engine.name = "backup_engine"
        backup_engine.is_available.return_value = True
        
        self.registry.register_engine(primary_engine)
        self.registry.register_engine(backup_engine)
        
        # Get failover engine
        failover = self.registry.get_failover_engine("primary_engine")
        assert failover == "backup_engine"
    
    def test_engine_monitoring(self):
        """Test engine monitoring functionality."""
        # Register engine
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "monitored_engine"
        mock_engine.is_available.return_value = True
        
        self.registry.register_engine(mock_engine)
        
        # Start monitoring
        self.registry.start_engine_monitoring("monitored_engine")
        
        # Verify monitoring is active
        assert self.registry.is_engine_monitored("monitored_engine")
        
        # Stop monitoring
        self.registry.stop_engine_monitoring("monitored_engine")
        
        # Verify monitoring is stopped
        assert not self.registry.is_engine_monitored("monitored_engine")
    
    def test_engine_configuration_updates(self):
        """Test updating engine configurations."""
        # Register engine
        mock_engine = Mock(spec=BaseWebSearchEngine)
        mock_engine.name = "test_engine"
        mock_engine.is_available.return_value = True
        
        self.registry.register_engine(mock_engine)
        
        # Update configuration
        new_config = WebSearchConfig(
            engine_name="test_engine",
            timeout=60,
            max_retries=5
        )
        
        self.registry.update_engine_configuration("test_engine", new_config)
        
        # Verify configuration was updated
        updated_config = self.registry.get_engine_configuration("test_engine")
        assert updated_config.timeout == 60
        assert updated_config.max_retries == 5
    
    def test_bulk_engine_operations(self):
        """Test bulk operations on engines."""
        # Register multiple engines
        engines = []
        for i in range(5):
            mock_engine = Mock(spec=BaseWebSearchEngine)
            mock_engine.name = f"engine_{i}"
            mock_engine.is_available.return_value = True
            engines.append(mock_engine)
            self.registry.register_engine(mock_engine)
        
        # Test bulk availability check
        availability = self.registry.check_bulk_availability()
        assert len(availability) == 5
        assert all(status for status in availability.values())
        
        # Test bulk health check
        health_statuses = self.registry.get_bulk_health_status()
        assert len(health_statuses) == 5
        
        # Test bulk configuration update
        new_timeout = 45
        self.registry.bulk_update_configuration({"timeout": new_timeout})
        
        # Verify all engines have updated configuration
        for engine_name in [f"engine_{i}" for i in range(5)]:
            config = self.registry.get_engine_configuration(engine_name)
            assert config.timeout == new_timeout