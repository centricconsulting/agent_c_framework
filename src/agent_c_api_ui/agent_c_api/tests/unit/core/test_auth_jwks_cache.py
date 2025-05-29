import unittest
from unittest.mock import patch, MagicMock
import time
from agent_c_api.core.auth import TokenVerifier

class TestJWKSCache(unittest.TestCase):
    """Tests for JWKS caching functionality"""
    
    def setUp(self):
        # Mock the httpx.get call used in __init__ and _refresh_jwks_cache
        self.httpx_get_patcher = patch('httpx.get')
        self.mock_httpx_get = self.httpx_get_patcher.start()
        
        # Setup mock responses
        mock_openid_config_response = MagicMock()
        mock_openid_config_response.status_code = 200
        mock_openid_config_response.json.return_value = {
            "jwks_uri": "https://login.microsoftonline.com/common/discovery/v2.0/keys",
            "issuer": "https://login.microsoftonline.com/test-tenant-id/v2.0"
        }
        
        mock_jwks_response = MagicMock()
        mock_jwks_response.status_code = 200
        mock_jwks_response.json.return_value = {
            "keys": [
                {"kid": "key1", "kty": "RSA", "n": "test", "e": "AQAB"},
                {"kid": "key2", "kty": "RSA", "n": "test2", "e": "AQAB"}
            ]
        }
        
        # Configure mock to return different responses based on URL
        def side_effect(url):
            if "/.well-known/openid-configuration" in url:
                return mock_openid_config_response
            else:
                return mock_jwks_response
        
        self.mock_httpx_get.side_effect = side_effect
    
    def tearDown(self):
        self.httpx_get_patcher.stop()
    
    def test_cache_initialization(self):
        """Test that the cache is initialized on startup"""
        verifier = TokenVerifier()
        
        # Verify the cache was populated
        self.assertIsNotNone(verifier.jwks_cache)
        self.assertEqual(len(verifier.jwks_cache_keys_by_kid), 2)
        self.assertTrue("key1" in verifier.jwks_cache_keys_by_kid)
        self.assertTrue("key2" in verifier.jwks_cache_keys_by_kid)
    
    def test_cache_validity(self):
        """Test cache validity check"""
        with patch('agent_c_api.core.auth.JWKS_CACHE_TTL', 10):  # 10 second TTL for testing
            verifier = TokenVerifier()
            
            # Cache should be valid initially
            self.assertTrue(verifier._is_cache_valid())
            
            # Mock cache timestamp to be older than TTL
            verifier.jwks_cache_timestamp = time.time() - 20
            
            # Cache should now be invalid
            self.assertFalse(verifier._is_cache_valid())
    
    def test_cache_refresh(self):
        """Test cache refresh functionality"""
        verifier = TokenVerifier()
        
        # Reset call count
        self.mock_httpx_get.reset_mock()
        
        # Call refresh
        result = verifier._refresh_jwks_cache()
        
        # Verify refresh was successful
        self.assertTrue(result)
        
        # Verify JWKS endpoint was called
        self.mock_httpx_get.assert_called_once()
        self.assertIn("keys", self.mock_httpx_get.call_args[0][0])
    
    def test_cache_lookup(self):
        """Test key lookup from cache"""
        verifier = TokenVerifier()
        
        # Should have 2 keys in cache
        self.assertEqual(len(verifier.jwks_cache_keys_by_kid), 2)
        
        # Lookup existing key
        key = verifier.jwks_cache_keys_by_kid.get("key1")
        self.assertIsNotNone(key)
        self.assertEqual(key["kid"], "key1")
        
        # Lookup non-existent key
        key = verifier.jwks_cache_keys_by_kid.get("non-existent-key")
        self.assertIsNone(key)

if __name__ == '__main__':
    unittest.main()