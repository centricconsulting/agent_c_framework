import unittest
from unittest.mock import patch, MagicMock
from fastapi import HTTPException

from agent_c_api.core.auth.providers.base import AuthProvider, TokenVerifier
from agent_c_api.core.auth.providers.registry import AuthProviderRegistry
from agent_c_api.core.auth.auth_service import AuthService

# Create a mock provider for testing
class MockTokenVerifier(TokenVerifier):
    def __init__(self, provider_name, should_succeed=True):
        self.provider_name = provider_name
        self.should_succeed = should_succeed
        self.verify_count = 0
    
    async def verify(self, token):
        self.verify_count += 1
        if not self.should_succeed:
            raise HTTPException(status_code=401, detail="Invalid token")
        return {"sub": "test-user", "provider": self.provider_name}
    
    def get_provider_name(self):
        return self.provider_name

class MockAuthProvider(AuthProvider):
    def __init__(self, name, enabled=True, should_succeed=True):
        self.name = name
        self._enabled = enabled
        self.token_verifier = MockTokenVerifier(name, should_succeed)
    
    def get_name(self):
        return self.name
    
    def is_enabled(self):
        return self._enabled
    
    def get_token_verifier(self):
        return self.token_verifier
    
    def get_config(self):
        return {"name": self.name, "enabled": self._enabled}
    
    def get_auth_metadata(self):
        return {"provider": self.name, "display_name": self.name.capitalize()}

class TestAuthProvider(unittest.TestCase):
    """Tests for the abstract authentication provider system"""
    
    def setUp(self):
        # Create a clean registry for each test
        self.registry = AuthProviderRegistry()
    
    def test_provider_registration(self):
        """Test provider registration and retrieval"""
        # Register providers
        provider1 = MockAuthProvider("provider1")
        provider2 = MockAuthProvider("provider2")
        self.registry.register(provider1)
        self.registry.register(provider2)
        
        # Test retrieval
        self.assertEqual(self.registry.get_provider("provider1"), provider1)
        self.assertEqual(self.registry.get_provider("provider2"), provider2)
        self.assertEqual(self.registry.get_provider("non-existent"), None)
        
        # Test default provider
        self.assertEqual(self.registry.get_default_provider(), provider1)
        
        # Change default provider
        self.registry.set_default_provider("provider2")
        self.assertEqual(self.registry.get_default_provider(), provider2)
    
    def test_disabled_provider(self):
        """Test handling of disabled providers"""
        # Register enabled and disabled providers
        enabled_provider = MockAuthProvider("enabled", enabled=True)
        disabled_provider = MockAuthProvider("disabled", enabled=False)
        
        self.registry.register(enabled_provider)
        self.registry.register(disabled_provider)
        
        # Only enabled provider should be in the registry
        self.assertEqual(self.registry.get_provider("enabled"), enabled_provider)
        self.assertEqual(self.registry.get_provider("disabled"), None)
        
        # Only enabled provider should be in enabled providers list
        enabled_providers = self.registry.get_enabled_providers()
        self.assertEqual(len(enabled_providers), 1)
        self.assertEqual(enabled_providers[0], enabled_provider)
    
    def test_provider_metadata(self):
        """Test provider metadata retrieval"""
        # Register providers
        provider1 = MockAuthProvider("provider1")
        provider2 = MockAuthProvider("provider2")
        self.registry.register(provider1)
        self.registry.register(provider2)
        
        # Test metadata
        metadata = self.registry.get_provider_metadata()
        self.assertEqual(len(metadata), 2)
        self.assertEqual(metadata["provider1"]["provider"], "provider1")
        self.assertEqual(metadata["provider2"]["display_name"], "Provider2")

class TestAuthService(unittest.TestCase):
    """Tests for the authentication service"""
    
    def setUp(self):
        # Patch the registry to use a clean one for each test
        self.registry_patcher = patch('agent_c_api.core.auth.auth_service.auth_provider_registry', new=AuthProviderRegistry())
        self.mock_registry = self.registry_patcher.start()
        
        # Create auth service
        self.auth_service = AuthService()
        
        # Add mock providers to the registry directly
        self.provider1 = MockAuthProvider("provider1")
        self.provider2 = MockAuthProvider("provider2")
        self.failing_provider = MockAuthProvider("failing", should_succeed=False)
        
        self.mock_registry.register(self.provider1)
        self.mock_registry.register(self.provider2)
        self.mock_registry.register(self.failing_provider)
    
    def tearDown(self):
        self.registry_patcher.stop()
    
    async def test_verify_token_with_provider(self):
        """Test token verification with a specific provider"""
        # Verify with provider1
        result = await self.auth_service.verify_token("test-token", "provider1")
        self.assertEqual(result["provider"], "provider1")
        self.assertEqual(self.provider1.token_verifier.verify_count, 1)
        
        # Verify with provider2
        result = await self.auth_service.verify_token("test-token", "provider2")
        self.assertEqual(result["provider"], "provider2")
        self.assertEqual(self.provider2.token_verifier.verify_count, 1)
    
    async def test_verify_token_auto_detect(self):
        """Test token verification with auto-detection"""
        # Default provider should be tried first
        result = await self.auth_service.verify_token("test-token")
        self.assertEqual(result["provider"], "provider1")
        
        # Change default provider
        self.mock_registry.set_default_provider("provider2")
        result = await self.auth_service.verify_token("test-token")
        self.assertEqual(result["provider"], "provider2")
    
    async def test_verify_token_failover(self):
        """Test token verification failover between providers"""
        # Set failing provider as default
        self.mock_registry.set_default_provider("failing")
        
        # Should fail over to a working provider
        result = await self.auth_service.verify_token("test-token")
        
        # Either provider1 or provider2 should succeed
        self.assertIn(result["provider"], ["provider1", "provider2"])
        
        # Failing provider should have been tried
        self.assertEqual(self.failing_provider.token_verifier.verify_count, 1)
    
    async def test_verify_token_all_fail(self):
        """Test behavior when all providers fail"""
        # Create registry with only failing providers
        registry = AuthProviderRegistry()
        registry.register(MockAuthProvider("failing1", should_succeed=False))
        registry.register(MockAuthProvider("failing2", should_succeed=False))
        
        # Create auth service with failing providers
        with patch('agent_c_api.core.auth.auth_service.auth_provider_registry', new=registry):
            auth_service = AuthService()
            
            # Should raise an exception
            with self.assertRaises(HTTPException) as context:
                await auth_service.verify_token("test-token")
            
            # Should be a 401 error
            self.assertEqual(context.exception.status_code, 401)

if __name__ == '__main__':
    unittest.main()