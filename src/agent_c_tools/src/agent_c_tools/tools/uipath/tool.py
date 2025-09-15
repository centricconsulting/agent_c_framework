"""UiPath integration toolset for Agent C framework."""

import json
import os
from typing import Optional, Dict, Any
import requests
import yaml
from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema


class UiPathTools(Toolset):
    """
    UiPath integration toolset providing authentication and asset management capabilities.
    
    This toolset allows agents to interact with UiPath Cloud Orchestrator to:
    - Authenticate with UiPath Cloud
    - Create and manage assets
    - Future: Deploy processes, trigger jobs, manage robots
    
    Configuration is handled via environment variables for security.
    """
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='uipath')
        self._token_cache: Dict[str, str] = {}
        
    async def post_init(self):
        """Initialize the UiPath toolset after all dependencies are loaded."""
        self.logger.info("UiPath toolset initialized successfully")
    
    def _get_config_value(self, key: str, default: Optional[str] = None) -> Optional[str]:
        """Get configuration value from environment variables."""
        return os.getenv(f"UIPATH_{key}", default)
    
    def _validate_config(self) -> Dict[str, str]:
        """Validate that required configuration is available."""
        # Check for PAT token first (simpler authentication)
        pat_token = self._get_config_value('PAT_TOKEN')
        
        required_configs = {
            'org_name': self._get_config_value('ORG_NAME'),
            'tenant_name': self._get_config_value('TENANT_NAME', 'DefaultTenant'),
            'folder_id': self._get_config_value('FOLDER_ID')
        }
        
        if pat_token:
            # Using Personal Access Token authentication
            required_configs['pat_token'] = pat_token
            required_configs['auth_method'] = 'pat'
        else:
            # Using OAuth2 client credentials authentication
            required_configs['client_id'] = self._get_config_value('CLIENT_ID')
            required_configs['client_secret'] = self._get_config_value('CLIENT_SECRET')
            required_configs['auth_method'] = 'oauth2'
            
            if not required_configs['client_id'] or not required_configs['client_secret']:
                raise ValueError("Missing UiPath authentication. Please set either:\n"
                               "- UIPATH_PAT_TOKEN for Personal Access Token authentication, OR\n"
                               "- UIPATH_CLIENT_ID and UIPATH_CLIENT_SECRET for OAuth2 authentication")
        
        # Check for required base configs
        missing_configs = [key for key, value in required_configs.items() 
                          if key not in ['auth_method', 'pat_token', 'client_id', 'client_secret'] and not value]
        if missing_configs:
            raise ValueError(f"Missing required UiPath configuration: {', '.join(missing_configs)}. "
                           f"Please set environment variables: {', '.join([f'UIPATH_{key.upper()}' for key in missing_configs])}")
        
        return required_configs
    
    async def _get_auth_token(self, scope: str = "OR.Assets OR.Assets.Read OR.Assets.Write") -> str:
        """Get authentication token from UiPath Cloud."""
        try:
            config = self._validate_config()
            
            # Check if we have a cached token (simple caching, could be enhanced)
            cache_key = f"{config['org_name']}_{scope}_{config['auth_method']}"
            if cache_key in self._token_cache:
                return self._token_cache[cache_key]
            
            if config['auth_method'] == 'pat':
                # Using Personal Access Token - return it directly
                access_token = config['pat_token']
                self.logger.info("Using Personal Access Token for UiPath authentication")
            else:
                # Using OAuth2 client credentials flow
                url = f"https://cloud.uipath.com/{config['org_name']}/identity_/connect/token"
                
                data = {
                    "client_id": config['client_id'],
                    "client_secret": config['client_secret'],
                    "scope": scope,
                    "grant_type": "client_credentials",
                }
                
                response = requests.post(url, data=data)
                response.raise_for_status()
                
                token_data = response.json()
                access_token = token_data.get("access_token")
                
                if not access_token:
                    raise ValueError("No access token received from UiPath")
                
                self.logger.info("Successfully retrieved UiPath OAuth2 authentication token")
            
            # Cache the token (in production, you'd want to handle expiration)
            self._token_cache[cache_key] = access_token
            
            return access_token
            
        except requests.RequestException as e:
            error_msg = f"Failed to authenticate with UiPath: {str(e)}"
            self.logger.error(error_msg)
            raise Exception(error_msg)
        except Exception as e:
            error_msg = f"UiPath authentication error: {str(e)}"
            self.logger.error(error_msg)
            raise Exception(error_msg)
    
    @json_schema(
        description="Create a new asset in UiPath Orchestrator. Assets are configuration values that can be accessed by UiPath robots during automation execution. For credentials, provide username and password as separate parameters.",
        params={
            "asset_name": {
                "type": "string",
                "description": "The name of the asset to create",
                "required": True
            },
            "asset_value": {
                "type": "string", 
                "description": "The value to store in the asset. For Text assets, provide the string value. For Integer assets, provide the number as string. For Boolean assets, provide 'true' or 'false'. For Credential assets, this parameter is ignored - use username and password instead.",
                "required": False
            },
            "asset_type": {
                "type": "string",
                "description": "The type of asset (Text, Integer, Boolean, Credential)",
                "enum": ["Text", "Integer", "Boolean", "Credential"],
                "default": "Text"
            },
            "username": {
                "type": "string",
                "description": "Username for Credential type assets. Required only when asset_type is 'Credential'.",
                "required": False
            },
            "password": {
                "type": "string",
                "description": "Password for Credential type assets. Required only when asset_type is 'Credential'.",
                "required": False
            },
            "description": {
                "type": "string",
                "description": "Optional description for the asset",
                "default": "Created via Agent C"
            }
        }
    )
    async def create_asset(self, **kwargs) -> str:
        """Create a new asset in UiPath Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            asset_name = kwargs.get('asset_name')
            asset_value = kwargs.get('asset_value')
            asset_type = kwargs.get('asset_type', 'Text')
            username = kwargs.get('username')
            password = kwargs.get('password')
            description = kwargs.get('description', 'Created via Agent C')
            
            if not asset_name:
                return "ERROR: asset_name is required"
            
            # Validate required parameters based on asset type
            if asset_type == 'Credential':
                if not username or not password:
                    return "ERROR: username and password are required for Credential type assets"
            else:
                if not asset_value:
                    return "ERROR: asset_value is required for non-Credential type assets"
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token
            token = await self._get_auth_token()
            
            # Prepare the API request
            create_asset_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Assets"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": str(config['folder_id']),
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
            
            # Build payload with correct format for each asset type
            payload = {
                "Name": asset_name,
                "ValueScope": "Global",
                "Description": description
            }
            
            # Set the correct value field and type based on asset_type
            if asset_type == 'Boolean':
                # Boolean assets need BoolValue and ValueType "Bool"
                bool_value = asset_value.lower() in ['true', '1', 'yes', 'on']
                payload["BoolValue"] = bool_value
                payload["ValueType"] = "Bool"
            elif asset_type == 'Integer':
                # Integer assets need IntValue with numeric value
                payload["IntValue"] = int(asset_value)
                payload["ValueType"] = asset_type
            elif asset_type == 'Credential':
                # Credential assets need separate CredentialUsername and CredentialPassword fields
                payload["CredentialUsername"] = username
                payload["CredentialPassword"] = password
                payload["ValueType"] = asset_type
            else:
                # Text assets use StringValue
                payload["StringValue"] = asset_value
                payload["ValueType"] = asset_type
            
            # Set actual_value for response
            if asset_type == 'Credential':
                actual_value = f"{{username: '{username}', password: '[HIDDEN]'}}"
            else:
                actual_value = asset_value
                
            # Validate asset_value for non-Credential types
            if asset_type == 'Integer':
                try:
                    int(asset_value)  # Just validate, but still store as string
                except ValueError:
                    return f"ERROR: Invalid integer value '{asset_value}' for Integer asset type"
            elif asset_type == 'Boolean':
                if asset_value.lower() not in ['true', 'false', '1', '0', 'yes', 'no', 'on', 'off']:
                    return f"ERROR: Invalid boolean value '{asset_value}'. Use 'true' or 'false'"
            
            # Debug logging and console output for debugging
            self.logger.info(f"UiPath API URL: {create_asset_url}")
            self.logger.info(f"UiPath Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"UiPath Payload: {json.dumps(payload, indent=2)}")
            
            # Also return payload info for debugging
            debug_info = f"DEBUG - Payload being sent:\n{json.dumps(payload, indent=2)}\n\n"
            
            # Make the API call
            response = requests.post(create_asset_url, headers=headers, data=json.dumps(payload))
            
            # Debug response
            self.logger.info(f"UiPath Response Status: {response.status_code}")
            self.logger.info(f"UiPath Response Body: {response.text}")
            
            # Include debug info in error responses
            if response.status_code != 201:
                error_msg = f"Failed to create asset. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}\n\n{debug_info}"
            
            if response.status_code == 201:
                result_data = response.json()
                self.logger.info(f"Successfully created UiPath asset: {asset_name} (type: {asset_type})")
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Asset '{asset_name}' created successfully",
                    "asset_id": result_data.get("Id"),
                    "asset_name": asset_name,
                    "asset_type": asset_type,
                    "asset_value": actual_value,
                    "description": description
                }
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to create asset. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error creating UiPath asset: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"
    
    @json_schema(
        description="Test UiPath connection and authentication. This will verify that the configuration is correct and the service is accessible.",
        params={}
    )
    async def test_connection(self, **kwargs) -> str:
        """Test the connection to UiPath Cloud."""
        try:
            tool_context = kwargs.get('tool_context')
            
            # Try to get a token
            token = await self._get_auth_token()
            
            if token:
                config = self._validate_config()
                result = {
                    "status": "success",
                    "message": "Successfully connected to UiPath Cloud",
                    "org_name": config['org_name'],
                    "tenant_name": config['tenant_name'],
                    "folder_id": config['folder_id']
                }
                return yaml.dump(result, allow_unicode=True)
            else:
                return "ERROR: Failed to obtain authentication token"
                
        except Exception as e:
            error_msg = f"Connection test failed: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"
    
    @json_schema(
        description="Get information about the current UiPath configuration without exposing sensitive data.",
        params={}
    )
    async def get_config_info(self, **kwargs) -> str:
        """Get information about the current UiPath configuration."""
        try:
            tool_context = kwargs.get('tool_context')
            
            config = self._validate_config()
            
            # Return configuration info without sensitive data
            config_info = {
                "org_name": config['org_name'],
                "tenant_name": config['tenant_name'],
                "folder_id": config['folder_id'],
                "auth_method": config['auth_method']
            }
            
            if config['auth_method'] == 'pat':
                config_info["pat_token"] = config['pat_token'][:8] + "..." if config['pat_token'] else "Not set"
            else:
                config_info["client_id"] = config['client_id'][:8] + "..." if config['client_id'] else "Not set"
                config_info["client_secret"] = "***" if config['client_secret'] else "Not set"
            
            return yaml.dump(config_info, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error getting configuration info: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"


# Register the toolset
Toolset.register(UiPathTools)