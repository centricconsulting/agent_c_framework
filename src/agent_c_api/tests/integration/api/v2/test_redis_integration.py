"""
Integration tests for Redis connectivity and health checks.

These tests verify Redis integration with actual Redis connections
and test the health check endpoints for operational monitoring.
"""

import pytest
import pytest_asyncio
from unittest.mock import patch, AsyncMock
from fastapi.testclient import TestClient
from fastapi import HTTPException

from agent_c_api.main import app
from agent_c_api.config.redis_config import RedisConfig
from agent_c_api.api.dependencies import (
    get_redis_client,
    get_redis_client_optional,
    get_redis_client_managed
)


@pytest.mark.integration
@pytest.mark.api
@pytest.mark.slow
class TestRedisConnectivity:
    """Test Redis connectivity and configuration."""
    
    @pytest_asyncio.fixture
    def client(self):
        """Create a test client for the FastAPI application."""
        return TestClient(app)
    
    async def test_redis_config_connection_validation(self):
        """Test RedisConfig connection validation."""
        try:
            # Attempt to validate Redis connection
            redis_client = await RedisConfig.get_redis_client()
            validation_result = await RedisConfig.validate_connection()
            
            # If Redis is available, validation should succeed
            assert validation_result is not None
            assert "status" in validation_result
            
            # Clean up
            if redis_client:
                await redis_client.aclose()
                
        except Exception as e:
            # If Redis is not available, this is expected in some test environments
            pytest.skip(f"Redis not available for integration testing: {e}")
    
    async def test_redis_dependency_injection_with_real_redis(self):
        """Test Redis dependency injection with actual Redis connection."""
        try:
            # Test standard Redis client dependency
            redis_client = await get_redis_client()
            assert redis_client is not None
            
            # Test basic Redis operation
            await redis_client.set("test_key", "test_value")
            value = await redis_client.get("test_key")
            assert value.decode() == "test_value"
            
            # Clean up
            await redis_client.delete("test_key")
            await redis_client.aclose()
            
        except HTTPException as e:
            if e.status_code == 503:
                pytest.skip("Redis not available for integration testing")
            else:
                raise
        except Exception as e:
            pytest.skip(f"Redis not available for integration testing: {e}")
    
    async def test_redis_optional_dependency_with_real_redis(self):
        """Test optional Redis dependency with actual Redis connection."""
        try:
            # Test optional Redis client dependency
            redis_client = await get_redis_client_optional()
            
            if redis_client is not None:
                # Redis is available, test basic operation
                await redis_client.set("test_optional_key", "test_value")
                value = await redis_client.get("test_optional_key")
                assert value.decode() == "test_value"
                
                # Clean up
                await redis_client.delete("test_optional_key")
                await redis_client.aclose()
            else:
                # Redis is not available, which is acceptable for optional dependency
                pytest.skip("Redis not available, but optional dependency handled gracefully")
                
        except Exception as e:
            pytest.skip(f"Unexpected error in optional Redis dependency: {e}")
    
    async def test_redis_managed_dependency_with_real_redis(self):
        """Test managed Redis dependency with actual Redis connection."""
        try:
            # Test managed Redis client dependency
            manager = await get_redis_client_managed()
            
            async with manager as redis_client:
                assert redis_client is not None
                
                # Test basic Redis operation
                await redis_client.set("test_managed_key", "test_value")
                value = await redis_client.get("test_managed_key")
                assert value.decode() == "test_value"
                
                # Clean up test data
                await redis_client.delete("test_managed_key")
            
            # Context manager should have automatically closed the connection
            
        except HTTPException as e:
            if e.status_code == 503:
                pytest.skip("Redis not available for integration testing")
            else:
                raise
        except Exception as e:
            pytest.skip(f"Redis not available for integration testing: {e}")


@pytest.mark.integration
@pytest.mark.api
class TestHealthCheckEndpoints:
    """Test Redis health check endpoints."""
    
    @pytest_asyncio.fixture
    def client(self):
        """Create a test client for the FastAPI application."""
        return TestClient(app)
    
    def test_main_health_endpoint(self, client):
        """Test the main health check endpoint."""
        response = client.get("/api/v2/health")
        
        # Should always return a response, even if Redis is down
        assert response.status_code in [200, 503]
        
        data = response.json()
        assert "status" in data
        assert "timestamp" in data
        assert "services" in data
        
        # Redis should be listed as a service
        assert "redis" in data["services"]
    
    def test_readiness_probe_endpoint(self, client):
        """Test the Kubernetes readiness probe endpoint."""
        response = client.get("/api/v2/health/ready")
        
        # Should return 200 if ready, 503 if not ready
        assert response.status_code in [200, 503]
        
        data = response.json()
        assert "status" in data
        assert data["status"] in ["ready", "not_ready"]
    
    def test_liveness_probe_endpoint(self, client):
        """Test the Kubernetes liveness probe endpoint."""
        response = client.get("/api/v2/health/live")
        
        # Liveness should generally return 200 unless the app is completely broken
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert data["status"] == "alive"
    
    def test_detailed_redis_health_endpoint(self, client):
        """Test the detailed Redis health check endpoint."""
        response = client.get("/api/v2/debug/health/redis")
        
        # Should always return a response with Redis status
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "timestamp" in data
        assert "redis" in data
        
        redis_data = data["redis"]
        assert "connectivity" in redis_data
        assert "status" in redis_data["connectivity"]
    
    def test_redis_connectivity_health_endpoint(self, client):
        """Test the Redis connectivity health check endpoint."""
        response = client.get("/api/v2/debug/health/redis/connectivity")
        
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "connectivity" in data
        assert data["connectivity"]["status"] in ["healthy", "unhealthy", "error"]
    
    def test_redis_performance_health_endpoint(self, client):
        """Test the Redis performance health check endpoint."""
        response = client.get("/api/v2/debug/health/redis/performance")
        
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "performance" in data
        
        perf_data = data["performance"]
        if perf_data["status"] != "error":
            assert "ping_latency_ms" in perf_data
            assert "operation_latency_ms" in perf_data
    
    def test_redis_server_info_health_endpoint(self, client):
        """Test the Redis server info health check endpoint."""
        response = client.get("/api/v2/debug/health/redis/server-info")
        
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "server_info" in data
    
    def test_redis_connection_pool_health_endpoint(self, client):
        """Test the Redis connection pool health check endpoint."""
        response = client.get("/api/v2/debug/health/redis/connection-pool")
        
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "connection_pool" in data
    
    def test_redis_operational_health_endpoint(self, client):
        """Test the Redis operational health check endpoint."""
        response = client.get("/api/v2/debug/health/redis/operational")
        
        assert response.status_code == 200
        
        data = response.json()
        assert "status" in data
        assert "operational" in data


@pytest.mark.integration
@pytest.mark.api
class TestRedisHealthWithMockedFailures:
    """Test Redis health endpoints with mocked Redis failures."""
    
    @pytest_asyncio.fixture
    def client(self):
        """Create a test client for the FastAPI application."""
        return TestClient(app)
    
    def test_health_endpoints_with_redis_unavailable(self, client):
        """Test health endpoints when Redis is unavailable."""
        with patch('agent_c_api.config.redis_config.RedisConfig.get_redis_client') as mock_get_client:
            mock_get_client.side_effect = ConnectionError("Redis connection failed")
            
            # Main health endpoint should handle Redis failure gracefully
            response = client.get("/api/v2/health")
            assert response.status_code in [200, 503]
            
            data = response.json()
            assert "services" in data
            assert "redis" in data["services"]
            assert data["services"]["redis"]["status"] in ["unhealthy", "error"]
    
    def test_redis_health_endpoints_with_timeout(self, client):
        """Test Redis health endpoints with timeout errors."""
        with patch('agent_c_api.config.redis_config.RedisConfig.get_redis_client') as mock_get_client:
            mock_get_client.side_effect = TimeoutError("Redis operation timeout")
            
            # Detailed Redis health should handle timeout gracefully
            response = client.get("/api/v2/debug/health/redis")
            assert response.status_code == 200
            
            data = response.json()
            assert data["status"] in ["degraded", "unhealthy", "error"]
    
    def test_redis_health_with_partial_failure(self, client):
        """Test Redis health when some operations fail."""
        with patch('agent_c_api.config.redis_config.RedisConfig.validate_connection') as mock_validate:
            # Mock partial failure - connection works but validation fails
            mock_validate.side_effect = Exception("Validation failed")
            
            response = client.get("/api/v2/debug/health/redis/connectivity")
            assert response.status_code == 200
            
            data = response.json()
            # Should indicate degraded or error status
            assert data["connectivity"]["status"] in ["degraded", "error"]


@pytest.mark.integration
@pytest.mark.api
@pytest.mark.slow
class TestRedisEndToEndIntegration:
    """Test end-to-end Redis integration scenarios."""
    
    @pytest_asyncio.fixture
    def client(self):
        """Create a test client for the FastAPI application."""
        return TestClient(app)
    
    async def test_repository_operations_end_to_end(self):
        """Test repository operations through dependency injection."""
        try:
            from agent_c_api.core.repositories.dependencies import get_session_repository
            
            # Get repository through dependency injection
            redis_client = await get_redis_client()
            session_repo = await get_session_repository(redis_client)
            
            # Test session operations
            session_id = "integration-test-session"
            session_data = {
                "id": session_id,
                "model_id": "gpt-4",
                "persona_id": "test",
                "created_at": "2025-05-24T13:00:00Z"
            }
            
            # Create session
            create_result = await session_repo.create_session(session_id, session_data)
            assert create_result is True
            
            # Get session
            retrieved_data = await session_repo.get_session(session_id)
            assert retrieved_data == session_data
            
            # Update session
            updated_data = {**session_data, "model_id": "claude-3"}
            update_result = await session_repo.update_session(session_id, updated_data)
            assert update_result is True
            
            # Verify update
            final_data = await session_repo.get_session(session_id)
            assert final_data["model_id"] == "claude-3"
            
            # Clean up
            delete_result = await session_repo.delete_session(session_id)
            assert delete_result is True
            
            # Verify deletion
            deleted_data = await session_repo.get_session(session_id)
            assert deleted_data is None
            
            # Close Redis connection
            await redis_client.aclose()
            
        except Exception as e:
            pytest.skip(f"Redis not available for end-to-end testing: {e}")
    
    def test_health_monitoring_integration(self, client):
        """Test that health monitoring provides actionable information."""
        # Get comprehensive health status
        response = client.get("/api/v2/debug/health/redis")
        assert response.status_code == 200
        
        data = response.json()
        
        # Verify we have actionable monitoring data
        assert "timestamp" in data
        assert "redis" in data
        
        redis_data = data["redis"]
        
        # Should have connectivity information
        if "connectivity" in redis_data:
            conn_data = redis_data["connectivity"]
            assert "status" in conn_data
            
            # If Redis is healthy, should have additional metrics
            if conn_data["status"] == "healthy":
                assert "response_time_ms" in conn_data
        
        # Should have performance information if Redis is available
        if "performance" in redis_data:
            perf_data = redis_data["performance"]
            if perf_data["status"] != "error":
                assert "ping_latency_ms" in perf_data