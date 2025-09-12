"""Basic tests for UiPath tools."""

import os
import pytest
from unittest.mock import patch, MagicMock
from .tool import UiPathTools


class TestUiPathTools:
    """Test cases for UiPath integration tools."""
    
    def setup_method(self):
        """Set up test environment."""
        self.tool = UiPathTools()
        
        # Mock environment variables
        self.env_vars = {
            'UIPATH_ORG_NAME': 'test-org',
            'UIPATH_CLIENT_ID': 'test-client-id',
            'UIPATH_CLIENT_SECRET': 'test-client-secret',
            'UIPATH_TENANT_NAME': 'TestTenant',
            'UIPATH_FOLDER_ID': '12345'
        }
    
    @patch.dict(os.environ, {})
    def test_missing_config_validation(self):
        """Test that missing configuration raises appropriate errors."""
        with pytest.raises(ValueError) as exc_info:
            self.tool._validate_config()
        
        assert "Missing required UiPath configuration" in str(exc_info.value)
    
    @patch.dict(os.environ)
    def test_valid_config_validation(self):
        """Test that valid configuration passes validation."""
        os.environ.update(self.env_vars)
        
        config = self.tool._validate_config()
        
        assert config['org_name'] == 'test-org'
        assert config['client_id'] == 'test-client-id'
        assert config['tenant_name'] == 'TestTenant'
        assert config['folder_id'] == '12345'
    
    @patch.dict(os.environ)
    @patch('requests.post')
    async def test_successful_authentication(self, mock_post):
        """Test successful authentication token retrieval."""
        os.environ.update(self.env_vars)
        
        # Mock successful token response
        mock_response = MagicMock()
        mock_response.raise_for_status.return_value = None
        mock_response.json.return_value = {'access_token': 'test-token'}
        mock_post.return_value = mock_response
        
        token = await self.tool._get_auth_token()
        
        assert token == 'test-token'
        assert mock_post.called
    
    @patch.dict(os.environ)
    async def test_get_config_info(self):
        """Test getting configuration information."""
        os.environ.update(self.env_vars)
        
        result = await self.tool.get_config_info(tool_context={})
        
        assert 'test-org' in result
        assert 'TestTenant' in result
        assert '12345' in result
        assert 'test-cli...' in result  # Masked client ID
        assert '***' in result  # Masked secret
    
    @patch.dict(os.environ)
    @patch('requests.post')
    async def test_create_asset_success(self, mock_post):
        """Test successful asset creation."""
        os.environ.update(self.env_vars)
        
        # Mock authentication response
        auth_response = MagicMock()
        auth_response.raise_for_status.return_value = None
        auth_response.json.return_value = {'access_token': 'test-token'}
        
        # Mock asset creation response
        asset_response = MagicMock()
        asset_response.status_code = 201
        asset_response.json.return_value = {'Id': 'asset-123', 'Name': 'TestAsset'}
        
        mock_post.side_effect = [auth_response, asset_response]
        
        result = await self.tool.create_asset(
            asset_name='TestAsset',
            asset_value='Test Value',
            asset_type='Text',
            tool_context={}
        )
        
        assert 'success' in result
        assert 'TestAsset' in result
        assert 'asset-123' in result
    
    @patch.dict(os.environ)
    async def test_create_asset_missing_params(self):
        """Test asset creation with missing required parameters."""
        os.environ.update(self.env_vars)
        
        result = await self.tool.create_asset(tool_context={})
        
        assert result.startswith('ERROR:')
        assert 'required parameters' in result


if __name__ == '__main__':
    pytest.main([__file__])