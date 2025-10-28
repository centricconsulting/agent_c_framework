"""UiPath integration toolset for Agent C framework."""

import json
import os
import re
from typing import Optional, Dict, Any, cast
import requests
import yaml
from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema


class UiPathTools(Toolset):
    """
    UiPath integration toolset providing authentication, asset management, and queue management capabilities.
    
    This toolset allows agents to interact with UiPath Cloud Orchestrator to:
    - Authenticate with UiPath Cloud
    - Create and manage assets (Text, Integer, Boolean, Credential)
    - Create and manage queues for work item processing
    - Future: Deploy processes, trigger jobs, manage robots
    
    Configuration is handled via environment variables for security.
    """
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='uipath')
        self._token_cache: Dict[str, str] = {}
        self.workspace_tools: Optional['WorkspaceTools'] = None
        
    async def post_init(self):
        """Initialize the UiPath toolset after all dependencies are loaded."""
        # Get reference to workspace tools for file operations
        from agent_c_tools.tools.workspace.tool import WorkspaceTools
        self.workspace_tools = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))
        if not self.workspace_tools:
            self.logger.warning("WorkspaceTools not available - file operations will be limited")
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
            'folder_id': self._get_config_value('FOLDER_ID'),
           # 'auth_scope': self._get_config_value('AUTH_SCOPE')
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
    
    async def _get_folder_id_by_name(self, folder_name: str, token: str, config: Dict[str, str]) -> Optional[str]:
        """Get folder ID by folder name using UiPath Orchestrator API.
        
        Args:
            folder_name: Display name of the folder to find
            token: Authentication token
            config: Configuration dictionary with org_name and tenant_name
            
        Returns:
            Folder ID if found, None otherwise
        """
        try:
            url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Folders"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "Accept": "application/json",
            }
            
            params = {"$filter": f"DisplayName eq '{folder_name}'"}
            
            self.logger.info(f"Looking up folder ID for folder name: '{folder_name}'")
            response = requests.get(url, headers=headers, params=params)
            
            if response.status_code == 200:
                data = response.json()
                if data["value"]:
                    folder = data["value"][0]
                    folder_id = str(folder["Id"])
                    self.logger.info(f"Found folder '{folder_name}' with ID: {folder_id}")
                    return folder_id
                else:
                    self.logger.warning(f"Folder '{folder_name}' not found in Orchestrator")
                    return None
            else:
                self.logger.error(f"Failed to fetch folder details: {response.status_code} - {response.text}")
                return None
                
        except Exception as e:
            self.logger.error(f"Error looking up folder ID for '{folder_name}': {str(e)}")
            return None
    
    async def _resolve_folder_id(self, folder_name: Optional[str] = None) -> tuple[str, str]:
        """Resolve folder ID from folder name or use default from environment.
        
        Args:
            folder_name: Optional folder name to lookup. If None, uses default from config.
            
        Returns:
            Tuple of (folder_id, folder_info) where folder_info describes the source
        """
        try:
            config = self._validate_config()
            
            if folder_name:
                # User provided a folder name, look up the folder ID
                token = await self._get_auth_token("OR.Folders OR.Folders.Read")
                folder_id = await self._get_folder_id_by_name(folder_name, token, config)
                
                if folder_id:
                    folder_info = f"folder '{folder_name}' (ID: {folder_id})"
                    return folder_id, folder_info
                else:
                    # Folder not found, return error info
                    return "", f"ERROR: Folder '{folder_name}' not found in Orchestrator"
            else:
                # No folder name provided, use default from environment
                default_folder_id = config['folder_id']
                folder_info = f"default folder (ID: {default_folder_id})"
                return default_folder_id, folder_info
                
        except Exception as e:
            return "", f"ERROR: Failed to resolve folder ID: {str(e)}"
    
    async def _get_auth_token(self, scope:str="OR.Administration OR.Administration.Read OR.Administration.Write OR.Analytics OR.Analytics.Read OR.Analytics.Write OR.Assets OR.Assets.Read OR.Assets.Write OR.Audit OR.Audit.Read OR.Audit.Write OR.AutomationSolutions.Access OR.BackgroundTasks OR.BackgroundTasks.Read OR.BackgroundTasks.Write OR.Execution OR.Execution.Read OR.Execution.Write OR.Folders OR.Folders.Read OR.Folders.Write OR.Hypervisor OR.Hypervisor.Read OR.Hypervisor.Write OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.License OR.License.Read OR.License.Write OR.Machines OR.Machines.Read OR.Machines.Write OR.ML OR.ML.Read OR.ML.Write OR.Monitoring OR.Monitoring.Read OR.Monitoring.Write OR.Queues OR.Queues.Read OR.Queues.Write OR.Robots OR.Robots.Read OR.Robots.Write OR.Settings OR.Settings.Read OR.Settings.Write OR.Tasks OR.Tasks.Read OR.Tasks.Write OR.TestDataQueues OR.TestDataQueues.Read OR.TestDataQueues.Write OR.TestSetExecutions OR.TestSetExecutions.Read OR.TestSetExecutions.Write OR.TestSets OR.TestSets.Read OR.TestSets.Write OR.TestSetSchedules OR.TestSetSchedules.Read OR.TestSetSchedules.Write OR.Users OR.Users.Read OR.Users.Write OR.Webhooks OR.Webhooks.Read OR.Webhooks.Write ") -> str:
        """Get authentication token from UiPath Cloud."""
        try:
            config = self._validate_config()
            # scope = config['auth_scope']
            # self.logger.info(f"Using auth scope: {scope}")
            # Check if we have a cached token (simple caching, could be enhanced)
            cache_key = f"{config['org_name']}_{scope}_{config['auth_method']}"
            if cache_key in self._token_cache:
                return self._token_cache[cache_key]
            self.logger.info(f"auth method is {config['auth_method']}")
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
                self.logger.info(f"url is {url} and data for token api is {data}")
                response = requests.post(url, data=data)
                self.logger.info(f"response from token api {response}")
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
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the asset should be created. If not provided, uses the default folder from environment variables.",
                "required": False
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
            folder_name = kwargs.get('folder_name')
            
            if not asset_name:
                return "ERROR: asset_name is required"
            
            # Validate required parameters based on asset type
            if asset_type == 'Credential':
                if not username or not password:
                    return "ERROR: username and password are required for Credential type assets"
            else:
                if not asset_value:
                    return "ERROR: asset_value is required for non-Credential type assets"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token
            token = await self._get_auth_token("OR.Assets OR.Assets.Read OR.Assets.Write")
            self.logger.info(f"token for creating asset is {token}")
            # Prepare the API request
            create_asset_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Assets"
            self.logger.info(f"create_asset_url for creating asset is {create_asset_url}")
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
            self.logger.info(f"headers for creating asset is {headers}")
            # Build payload with correct format for each asset type
            payload = {
                "Name": asset_name,
                "ValueScope": "Global",
                "Description": description
            }
            self.logger.info(f"payload before setting type is {payload}")
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
                    "message": f"Asset '{asset_name}' created successfully in {folder_info}",
                    "asset_id": result_data.get("Id"),
                    "asset_name": asset_name,
                    "asset_type": asset_type,
                    "asset_value": actual_value,
                    "description": description,
                    "folder_info": folder_info
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
        description="Create a new queue in UiPath Orchestrator. Queues are used to store work items that can be processed by UiPath robots. Each queue can have specific settings for retry attempts, SLA, and priority.",
        params={
            "queue_name": {
                "type": "string",
                "description": "The name of the queue to create",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Optional description for the queue",
                "default": "Created via Agent C"
            },
            "max_number_of_retries": {
                "type": "integer",
                "description": "Maximum number of retry attempts for failed items (0-10)",
                "default": 1
            },
            "auto_retry": {
                "type": "boolean",
                "description": "Whether to automatically retry failed items",
                "default": False
            },
            "unique_ref": {
                "type": "boolean",
                "description": "Whether to enforce unique reference IDs for queue items",
                "default": False
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the queue should be created. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def create_queue(self, **kwargs) -> str:
        """Create a new queue in UiPath Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            queue_name = kwargs.get('queue_name')
            description = kwargs.get('description', 'Created via Agent C')
            max_retries = kwargs.get('max_number_of_retries', 0)
            auto_retry = kwargs.get('auto_retry', False)
            unique_ref = kwargs.get('unique_ref', False)
            folder_name = kwargs.get('folder_name')
            
            if not queue_name:
                return "ERROR: queue_name is required"
            
            # Validate parameters
            if not isinstance(max_retries, int) or max_retries < 0 or max_retries > 10:
                return "ERROR: max_number_of_retries must be an integer between 0 and 10"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with queue permissions
            token = await self._get_auth_token("OR.Queues OR.Queues.Read OR.Queues.Write")
            
            # Prepare the API request for queue creation
            create_queue_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/QueueDefinitions"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
            
            # Build payload for queue creation (matching the working createQueue.py implementation)
            payload = {
                "Name": queue_name,
                "Description": description,
                "AcceptAutomaticallyRetry": auto_retry,
                "EnforceUniqueReference": unique_ref,
                "MaxNumberOfRetries": max_retries,
                "ProcessingType": "Multiple",
                "SpecificDataJson": "{}"
            }
            
            # Debug logging
            self.logger.info(f"UiPath Queue API URL: {create_queue_url}")
            self.logger.info(f"UiPath Queue Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"UiPath Queue Payload: {json.dumps(payload, indent=2)}")
            
            # Make the API call
            response = requests.post(create_queue_url, headers=headers, data=json.dumps(payload))
            
            # Debug response
            self.logger.info(f"UiPath Queue Response Status: {response.status_code}")
            self.logger.info(f"UiPath Queue Response Body: {response.text}")
            
            if response.status_code == 201:
                result_data = response.json()
                self.logger.info(f"Successfully created UiPath queue: {queue_name}")
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Queue '{queue_name}' created successfully in {folder_info}",
                    "queue_id": result_data.get("Id"),
                    "queue_name": queue_name,
                    "description": description,
                    "max_number_of_retries": max_retries,
                    "auto_retry": auto_retry,
                    "unique_ref": unique_ref,
                    "processing_type": "Multiple",
                    "folder_info": folder_info
                }
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to create queue. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error creating UiPath queue: {str(e)}"
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
            token = await self._get_auth_token("OR.Administration OR.Administration.Read OR.Administration.Write OR.Analytics OR.Analytics.Read OR.Analytics.Write OR.Assets OR.Assets.Read OR.Assets.Write OR.Audit OR.Audit.Read OR.Audit.Write OR.AutomationSolutions.Access OR.BackgroundTasks OR.BackgroundTasks.Read OR.BackgroundTasks.Write OR.Execution OR.Execution.Read OR.Execution.Write OR.Folders OR.Folders.Read OR.Folders.Write OR.Hypervisor OR.Hypervisor.Read OR.Hypervisor.Write OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.License OR.License.Read OR.License.Write OR.Machines OR.Machines.Read OR.Machines.Write OR.ML OR.ML.Read OR.ML.Write OR.Monitoring OR.Monitoring.Read OR.Monitoring.Write OR.Queues OR.Queues.Read OR.Queues.Write OR.Robots OR.Robots.Read OR.Robots.Write OR.Settings OR.Settings.Read OR.Settings.Write OR.Tasks OR.Tasks.Read OR.Tasks.Write OR.TestDataQueues OR.TestDataQueues.Read OR.TestDataQueues.Write OR.TestSetExecutions OR.TestSetExecutions.Read OR.TestSetExecutions.Write OR.TestSets OR.TestSets.Read OR.TestSets.Write OR.TestSetSchedules OR.TestSetSchedules.Read OR.TestSetSchedules.Write OR.Users OR.Users.Read OR.Users.Write OR.Webhooks OR.Webhooks.Read OR.Webhooks.Write ")

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
    
    @json_schema(
        description="Test workspace file upload functionality",
        params={
            "workspace_path": {
                "type": "string",
                "description": "Workspace UNC path to test",
                "required": True
            }
        }
    )
    async def test_workspace_upload(self, **kwargs) -> str:
        """Test workspace file reading for upload."""
        try:
            workspace_path = kwargs.get('workspace_path')
            
            if not self.workspace_tools:
                return "ERROR: WorkspaceTools not available"
            
            # Parse the UNC path
            error, workspace, relative_path = self.workspace_tools._parse_unc_path(workspace_path)
            if error:
                return f"ERROR: {error}"
            
            # Try to read the file as binary
            file_content = await workspace.read_bytes_internal(relative_path)
            file_size = len(file_content)
            
            result = {
                "status": "success",
                "message": f"Successfully read workspace file: {workspace_path}",
                "file_size_bytes": file_size,
                "workspace_name": workspace.name,
                "relative_path": relative_path
            }
            
            return yaml.dump(result, allow_unicode=True)
            
        except Exception as e:
            return f"ERROR: Failed to read workspace file: {str(e)}"
    
    @json_schema(
        description="Upload a UiPath package (.nupkg file) to UiPath Orchestrator. This allows you to deploy automation packages to the Orchestrator for execution by robots. The package file must be a valid .nupkg file created by UiPath Studio.",
        params={
            "nupkg_path": {
                "type": "string",
                "description": "Full path to the .nupkg package file to upload",
                "required": True
            },
            "extra_fields": {
                "type": "object",
                "description": "Optional additional form fields to include with the upload request",
                "required": False
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the package should be uploaded. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def upload_package(self, **kwargs) -> str:
        """Upload a UiPath package to Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            nupkg_path = kwargs.get('nupkg_path')
            extra_fields = kwargs.get('extra_fields', {})
            folder_name = kwargs.get('folder_name')
            
            if not nupkg_path:
                return "ERROR: nupkg_path is required"
            
            # Check if it's a workspace UNC path or regular filesystem path
            is_workspace_path = nupkg_path.startswith('//')
            
            # Debug logging
            self.logger.info(f"Upload package path: '{nupkg_path}'")
            self.logger.info(f"Path starts with '//': {nupkg_path.startswith('//')}")
            self.logger.info(f"Is workspace path: {is_workspace_path}")
            self.logger.info(f"Workspace tools available: {self.workspace_tools is not None}")
            
            if is_workspace_path:
                # Validate workspace path format - we'll validate file existence during read
                if not self.workspace_tools:
                    return "ERROR: WorkspaceTools not available for workspace file operations"
                
                # Parse the UNC path to validate format
                try:
                    error, workspace, relative_path = self.workspace_tools._parse_unc_path(nupkg_path)
                    if error:
                        return f"ERROR: Invalid workspace path: {error}"
                except Exception as e:
                    return f"ERROR: Failed to parse workspace path {nupkg_path}: {str(e)}"
            else:
                # Regular filesystem path validation
                if not os.path.exists(nupkg_path):
                    abs_path=os.path.abspath(nupkg_path)
                    self.logger.info(f"absolute path is : {abs_path}")
                    return f"ERROR: Package file not found: {nupkg_path}"
            
            # Validate that it's a .nupkg file
            if not nupkg_path.lower().endswith('.nupkg'):
                return "ERROR: File must be a .nupkg package file"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with execution permissions
            token = await self._get_auth_token("OR.Execution OR.Execution.Read OR.Execution.Write")
            #REQUIRED = "OR.Execution OR.Execution.Read OR.Execution.Write"
            # Prepare the upload URL
            upload_url = (
                f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}"
                "/orchestrator_/odata/Processes/UiPath.Server.Configuration.OData.UploadPackage()"
            )
            
            # Prepare headers
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "Accept": "application/json",
                "X-UIPATH-Orchestrator": "True",
                "X-UIPATH-OrganizationUnitId": folder_id
            }
            
            # Debug logging
            self.logger.info(f"UiPath Upload Package URL: {upload_url}")
            self.logger.info(f"UiPath Upload Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"Package file: {nupkg_path}")
            self.logger.info(f"Extra fields: {extra_fields}")
            
            # Upload the package - handle both workspace and filesystem paths
            if is_workspace_path:
                # Read file content from workspace
                try:
                    self.logger.info(f"Attempting to read workspace file: {nupkg_path}")
                    
                    # Read the file as binary data from workspace
                    error, workspace, relative_path = self.workspace_tools._parse_unc_path(nupkg_path)
                    if error:
                        return f"ERROR: Invalid workspace path during upload: {error}"
                    
                    self.logger.info(f"Workspace: {workspace}, Relative path: {relative_path}")
                    
                    # Read binary content from workspace
                    file_content = await workspace.read_bytes_internal(relative_path)
                    filename = os.path.basename(nupkg_path)
                    
                    self.logger.info(f"Successfully read {len(file_content)} bytes from workspace file")
                    
                    # Create files dict with binary content
                    files = {
                        "package": (filename, file_content, "application/octet-stream")
                    }
                    response = requests.post(
                        upload_url, headers=headers, data=extra_fields, files=files
                    )
                except Exception as e:
                    self.logger.exception(f"Exception reading workspace file: {str(e)}")
                    return f"ERROR: Failed to read workspace file {nupkg_path}: {str(e)}"
            else:
                # Regular filesystem path - use original method
                with open(nupkg_path, "rb") as f:
                    files = {
                        "package": (os.path.basename(nupkg_path), f, "application/octet-stream")
                    }
                    response = requests.post(
                        upload_url, headers=headers, data=extra_fields, files=files
                    )
            
            # Debug response
            self.logger.info(f"UiPath Upload Response Status: {response.status_code}")
            self.logger.info(f"UiPath Upload Response Body: {response.text}")
            
            if response.status_code in [200, 201]:
                self.logger.info(f"Successfully uploaded UiPath package: {os.path.basename(nupkg_path)}")
                
                # Try to parse the response as JSON
                try:
                    response_data = response.json()
                except:
                    response_data = {"message": "Package uploaded successfully but response was not JSON"}
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Package '{os.path.basename(nupkg_path)}' uploaded successfully to {folder_info}",
                    "package_name": os.path.basename(nupkg_path),
                    "package_path": nupkg_path,
                    "folder_info": folder_info,
                    "response_data": response_data
                }
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to upload package. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error uploading UiPath package: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"


    @json_schema(
        description="Upload a UiPath package from workspace to Orchestrator (NEW METHOD)",
        params={
            "workspace_nupkg_path": {
                "type": "string",
                "description": "Workspace UNC path to the .nupkg package file",
                "required": True
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the package should be uploaded. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def upload_package_from_workspace(self, **kwargs) -> str:
        """Upload a UiPath package from workspace to Orchestrator."""
        try:
            workspace_path = kwargs.get('workspace_nupkg_path')
            folder_name = kwargs.get('folder_name')
            
            if not workspace_path:
                return "ERROR: workspace_nupkg_path is required"
            
            if not workspace_path.startswith('//'):
                return "ERROR: Path must be a workspace UNC path starting with //"
            
            if not self.workspace_tools:
                return "ERROR: WorkspaceTools not available"
            
            # Validate file extension
            if not workspace_path.lower().endswith('.nupkg'):
                return "ERROR: File must be a .nupkg package file"
            
            # Parse the UNC path
            error, workspace, relative_path = self.workspace_tools._parse_unc_path(workspace_path)
            if error:
                return f"ERROR: Invalid workspace path: {error}"
            
            # Read the binary file content
            try:
                file_content = await workspace.read_bytes_internal(relative_path)
                filename = os.path.basename(workspace_path)
            except Exception as e:
                return f"ERROR: Failed to read workspace file {workspace_path}: {str(e)}"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration and token
            config = self._validate_config()

            token = await self._get_auth_token("OR.Execution OR.Execution.Read OR.Execution.Write")
            
            # Prepare upload URL and headers
            upload_url = (
                f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}"
                "/orchestrator_/odata/Processes/UiPath.Server.Configuration.OData.UploadPackage()"
            )
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "Accept": "application/json",
                "X-UIPATH-Orchestrator": "True",
                "X-UIPATH-OrganizationUnitId": folder_id
            }
            
            # Upload the package
            files = {
                "package": (filename, file_content, "application/octet-stream")
            }
            response = requests.post(upload_url, headers=headers, files=files)
            
            # Handle response
            if response.status_code in [200, 201]:
                try:
                    response_data = response.json()
                except:
                    response_data = {"message": "Package uploaded successfully"}
                
                result = {
                    "status": "success",
                    "message": f"Package '{filename}' uploaded successfully from workspace to {folder_info}",
                    "package_name": filename,
                    "workspace_path": workspace_path,
                    "file_size_bytes": len(file_content),
                    "folder_info": folder_info,
                    "response_data": response_data
                }
                return yaml.dump(result, allow_unicode=True)
            else:
                return f"ERROR: Upload failed. Status: {response.status_code}, Response: {response.text}"
                
        except Exception as e:
            return f"ERROR: Exception during workspace upload: {str(e)}"

    async def _get_package_version(self, package_name: str, token: str, config: Dict[str, str], folder_id: str) -> Optional[str]:
        """Get the latest version of a package from UiPath Orchestrator."""
        try:
            url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Processes"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Accept": "application/json",
            }
            
            params = {
                "$filter": f"Title eq '{package_name}'"
            }
            
            response = requests.get(url, headers=headers, params=params)
            
            if response.status_code == 200:
                data = response.json()
                if data["value"]:
                    # Get the latest version (first one in the list)
                    return data["value"][0]["Version"]
                else:
                    self.logger.warning(f"No package found with name '{package_name}'")
                    return None
            else:
                self.logger.error(f"Failed to fetch package version. Status: {response.status_code}, Response: {response.text}")
                return None
                
        except Exception as e:
            self.logger.error(f"Error getting package version: {str(e)}")
            return None
    
    async def _get_entry_point_id(self, package_name: str, package_version: str, token: str, config: Dict[str, str], folder_id: str) -> Optional[str]:
        """Get the entry point ID for a package version."""
        try:
            url = (
                f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}"
                f"/orchestrator_/odata/Processes/UiPath.Server.Configuration.OData.GetPackageEntryPointsV2(key='{package_name}:{package_version}')"
            )
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Accept": "application/json",
            }
            
            response = requests.get(url, headers=headers)
            
            if response.status_code == 200:
                data = response.json()
                if data["value"]:
                    return data["value"][0]["Id"]
                else:
                    self.logger.error(f"No entry points found for package '{package_name}' version '{package_version}'")
                    return None
            else:
                self.logger.error(f"Failed to fetch entry point ID. Status: {response.status_code}, Response: {response.text}")
                return None
                
        except Exception as e:
            self.logger.error(f"Error getting entry point ID: {str(e)}")
            return None
    
    async def _get_process_release_details(self, process_name: str, token: str, config: Dict[str, str], folder_id: str) -> tuple[Optional[str], Optional[str]]:
        """Get the release ID and key for a process by process name."""
        try:
            url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Releases"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Accept": "application/json",
            }
            
            params = {
                "$filter": f"ProcessKey eq '{process_name}'"
            }
            
            response = requests.get(url, headers=headers, params=params)
            
            if response.status_code == 200:
                data = response.json()
                if data["value"]:
                    release = data["value"][0]  # Get the first (latest) release
                    return release["Id"], release["Key"]
                else:
                    self.logger.error(f"Process '{process_name}' not found in Orchestrator.")
                    return None, None
            else:
                self.logger.error(f"Failed to fetch process details. Status: {response.status_code}, Response: {response.text}")
                return None, None
                
        except Exception as e:
            self.logger.error(f"Error getting process release details: {str(e)}")
            return None, None
    
    def _parse_time_to_cron(self, time_input: str, days_of_week: Optional[str] = None) -> str:
        """Parse human-readable time input and convert to cron expression.
        
        Supports formats like:
        - "9 AM", "9:00 AM", "9:30 AM"
        - "2 PM", "2:00 PM", "2:30 PM" 
        - "14:30", "09:00" (24-hour format)
        - "9", "21" (hour only, assumes :00 minutes)
        
        Args:
            time_input: Human-readable time string
            days_of_week: Optional days specification (e.g., "MON,TUE,FRI", "MON-FRI", "*")
            
        Returns:
            Cron expression string
        """
        try:
            # Clean up the input
            time_str = time_input.strip().upper()
            
            # Default to daily if no days specified
            if not days_of_week:
                days_of_week = "*"
            
            # Parse different time formats
            hour = 0
            minute = 0
            
            # Format: "9 AM", "9:30 AM", "2 PM", "2:30 PM"
            am_pm_pattern = r'^(\d{1,2})(?::(\d{2}))? ?(AM|PM)$'
            match = re.match(am_pm_pattern, time_str)
            if match:
                hour = int(match.group(1))
                minute = int(match.group(2)) if match.group(2) else 0
                am_pm = match.group(3)
                
                # Convert to 24-hour format
                if am_pm == 'PM' and hour != 12:
                    hour += 12
                elif am_pm == 'AM' and hour == 12:
                    hour = 0
            else:
                # Format: "14:30", "09:00" (24-hour format)
                time_pattern = r'^(\d{1,2}):(\d{2})$'
                match = re.match(time_pattern, time_str)
                if match:
                    hour = int(match.group(1))
                    minute = int(match.group(2))
                else:
                    # Format: "9", "21" (hour only)
                    hour_pattern = r'^(\d{1,2})$'
                    match = re.match(hour_pattern, time_str)
                    if match:
                        hour = int(match.group(1))
                        minute = 0
                    else:
                        raise ValueError(f"Unable to parse time format: {time_input}")
            
            # Validate hour and minute ranges
            if not (0 <= hour <= 23):
                raise ValueError(f"Hour must be between 0-23, got: {hour}")
            if not (0 <= minute <= 59):
                raise ValueError(f"Minute must be between 0-59, got: {minute}")
            
            # Build cron expression: second minute hour day-of-month month day-of-week
            cron_expression = f"0 {minute} {hour} ? * {days_of_week}"
            
            self.logger.info(f"Parsed time '{time_input}' to cron expression: '{cron_expression}'")
            return cron_expression
            
        except Exception as e:
            raise ValueError(f"Error parsing time '{time_input}': {str(e)}")
    
    @json_schema(
        description="Create a new process (release) in UiPath Orchestrator from an existing package. The package with the same name as the process must already be uploaded to Orchestrator before creating a process from it. This creates a process definition that can be executed by UiPath robots.",
        params={
            "process_name": {
                "type": "string",
                "description": "The name for the new process/release. The package with this same name must already exist in Orchestrator.",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Optional description for the process",
                "default": "Process created via Agent C"
            },
            "input_arguments": {
                "type": "string",
                "description": "Optional input arguments for the process in JSON format",
                "default": "{}"
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the process should be created. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def create_process(self, **kwargs) -> str:
        """Create a new process (release) in UiPath Orchestrator from an existing package."""
        try:
            tool_context = kwargs.get('tool_context')
            process_name = kwargs.get('process_name')
            description = kwargs.get('description', 'Process created via Agent C')
            input_arguments = kwargs.get('input_arguments', '{}')
            folder_name = kwargs.get('folder_name')
            
            if not process_name:
                return "ERROR: process_name is required"
            
            # Use the same name for both process and package
            package_name = process_name
            
            # Validate input_arguments is valid JSON
            try:
                json.loads(input_arguments)
            except json.JSONDecodeError:
                return "ERROR: input_arguments must be valid JSON format"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with execution permissions

            token = await self._get_auth_token("OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.Execution OR.Execution.Read OR.Execution.Write")
            # Step 1: Get the package version
            self.logger.info(f"Getting version for package '{package_name}' (same as process name)...")
            package_version = await self._get_package_version(package_name, token, config, folder_id)
            
            if not package_version:
                return f"ERROR: Could not find package '{package_name}' in Orchestrator. Make sure the package with name '{process_name}' is uploaded first."
            
            self.logger.info(f"Found package version: {package_version}")
            
            # Step 2: Get the entry point ID
            self.logger.info(f"Getting entry point ID for package '{package_name}' version '{package_version}'...")
            entry_point_id = await self._get_entry_point_id(package_name, package_version, token, config, folder_id)
            
            if not entry_point_id:
                return f"ERROR: Could not get entry point ID for package '{package_name}' version '{package_version}'"
            
            self.logger.info(f"Found entry point ID: {entry_point_id}")
            
            # Step 3: Create the process (release)
            create_process_url = (
                f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}"
                "/orchestrator_/odata/Releases/UiPath.Server.Configuration.OData.CreateRelease"
            )
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Content-Type": "application/json",
                "Accept": "application/json",
            }
            
            payload = {
                "Name": process_name,
                "ProcessKey": package_name,  # Must match the Package ID in Orchestrator
                "Description": description,
                "ProcessVersion": package_version,
                "EntryPointId": entry_point_id,  # Main XAML file ID
                "InputArguments": input_arguments,
                "RetentionPeriod": 30,
                "StaleRetentionAction": "Delete",
                "StaleRetentionPeriod": 180
            }
            
            # Debug logging
            self.logger.info(f"UiPath Create Process URL: {create_process_url}")
            self.logger.info(f"UiPath Create Process Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"UiPath Create Process Payload: {json.dumps(payload, indent=2)}")
            
            # Make the API call
            response = requests.post(create_process_url, headers=headers, data=json.dumps(payload))
            
            # Debug response
            self.logger.info(f"UiPath Create Process Response Status: {response.status_code}")
            self.logger.info(f"UiPath Create Process Response Body: {response.text}")
            
            if response.status_code in [200, 201]:
                result_data = response.json()
                self.logger.info(f"Successfully created UiPath process: {process_name} from package {package_name}")
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Process '{process_name}' created successfully from package '{package_name}' in {folder_info}",
                    "process_id": result_data.get("Id"),
                    "process_name": process_name,
                    "package_name": package_name,
                    "package_version": package_version,
                    "entry_point_id": entry_point_id,
                    "description": description,
                    "input_arguments": input_arguments,
                    "folder_info": folder_info
                }
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to create process. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error creating UiPath process: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"
    
    @json_schema(
        description="Create a new time trigger in UiPath Orchestrator. Time triggers allow you to schedule process executions based on time and day specifications. The process must already exist in Orchestrator before creating a trigger for it. You can specify time in human-readable format (e.g., '9 AM', '2:30 PM') or provide a complete cron expression.",
        params={
            "process_name": {
                "type": "string",
                "description": "The name of the process/release to trigger. This process must already exist in Orchestrator.",
                "required": True
            },
            "trigger_name": {
                "type": "string",
                "description": "The name for the new time trigger",
                "required": True
            },
            "trigger_time": {
                "type": "string",
                "description": "Human-readable time when the trigger should run (e.g., '9 AM', '2:30 PM', '14:30', '9'). If provided, this will be converted to a cron expression. If not provided, uses cron_expression parameter.",
                "required": False
            },
            "days_of_week": {
                "type": "string",
                "description": "Days when the trigger should run (e.g., 'MON,TUE,FRI', 'MON-FRI', '*' for daily). Only used with trigger_time parameter. Default is '*' (daily).",
                "default": "*"
            },
            "cron_expression": {
                "type": "string",
                "description": "Complete cron expression for scheduling (e.g., '0 0 10 * * ?' for daily at 10 AM). Used only if trigger_time is not provided. Default is daily at 10 AM.",
                "default": "0 0 10 * * ?"
            },
            "timezone_id": {
                "type": "string",
                "description": "Timezone ID for the schedule (e.g., 'India Standard Time', 'UTC', 'Eastern Standard Time'). Default is 'India Standard Time'.",
                "default": "India Standard Time"
            },
            "enabled": {
                "type": "boolean",
                "description": "Whether the trigger should be enabled immediately",
                "default": True
            },
            "advanced_cron_details": {
                "type": "string",
                "description": "Advanced cron details in JSON format. Default is daily schedule.",
                "default": '{"advancedCron":"0 0 0 1/1 * ? *"}'
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the time trigger should be created. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def create_time_trigger(self, **kwargs) -> str:
        """Create a new time trigger in UiPath Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            process_name = kwargs.get('process_name')
            trigger_name = kwargs.get('trigger_name')
            trigger_time = kwargs.get('trigger_time')
            days_of_week = kwargs.get('days_of_week', '*')
            cron_expression = kwargs.get('cron_expression', '0 0 10 * * ?')
            timezone_id = kwargs.get('timezone_id', 'India Standard Time')
            enabled = kwargs.get('enabled', True)
            folder_name = kwargs.get('folder_name')
            advanced_cron_details = kwargs.get('advanced_cron_details', '{"advancedCron":"0 0 0 1/1 * ? *"}')
            
            if not process_name:
                return "ERROR: process_name is required"
            
            if not trigger_name:
                return "ERROR: trigger_name is required"
            
            # Determine the cron expression to use
            final_cron_expression = cron_expression
            time_parsing_info = ""
            
            if trigger_time:
                # User provided human-readable time, convert it to cron
                try:
                    final_cron_expression = self._parse_time_to_cron(trigger_time, days_of_week)
                    time_parsing_info = f"Converted time '{trigger_time}' (days: {days_of_week}) to cron: '{final_cron_expression}'"
                    self.logger.info(time_parsing_info)
                except ValueError as e:
                    return f"ERROR: {str(e)}"
            else:
                time_parsing_info = f"Using provided cron expression: '{final_cron_expression}'"
            
            # Validate advanced_cron_details is valid JSON
            try:
                json.loads(advanced_cron_details)
            except json.JSONDecodeError:
                return "ERROR: advanced_cron_details must be valid JSON format"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with appropriate permissions
            token = await self._get_auth_token("OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.Execution OR.Execution.Read OR.Execution.Write")
            
            # Step 1: Get the process release details
            self.logger.info(f"Getting release details for process '{process_name}'...")
            release_id, release_key = await self._get_process_release_details(process_name, token, config, folder_id)
            
            if not release_id:
                return f"ERROR: Could not find process '{process_name}' in Orchestrator. Make sure the process exists and is published."
            
            self.logger.info(f"Found process release - ID: {release_id}, Key: {release_key}")
            
            # Step 2: Create the time trigger
            create_trigger_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/ProcessSchedules"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Content-Type": "application/json",
                "Accept": "application/json",
            }
            
            payload = {
                "Name": trigger_name,
                "Enabled": enabled,
                "ReleaseId": release_id,
                "StartProcessCronDetails": advanced_cron_details,
                "StartStrategy": 1,  # Specific - starts jobs for specific robots
                "StartProcessCron": final_cron_expression,
                "TimeZoneId": timezone_id
            }
            
            # Debug logging
            self.logger.info(f"UiPath Create Time Trigger URL: {create_trigger_url}")
            self.logger.info(f"UiPath Create Time Trigger Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"UiPath Create Time Trigger Payload: {json.dumps(payload, indent=2)}")
            
            # Make the API call
            response = requests.post(create_trigger_url, headers=headers, data=json.dumps(payload))
            
            # Debug response
            self.logger.info(f"UiPath Create Time Trigger Response Status: {response.status_code}")
            self.logger.info(f"UiPath Create Time Trigger Response Body: {response.text}")
            
            if response.status_code in [200, 201]:
                result_data = response.json()
                self.logger.info(f"Successfully created UiPath time trigger: {trigger_name} for process {process_name}")
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Time trigger '{trigger_name}' created successfully for process '{process_name}' in {folder_info}",
                    "trigger_id": result_data.get("Id"),
                    "trigger_name": trigger_name,
                    "process_name": process_name,
                    "release_id": release_id,
                    "cron_expression": final_cron_expression,
                    "timezone_id": timezone_id,
                    "enabled": enabled,
                    "advanced_cron_details": advanced_cron_details,
                    "time_parsing_info": time_parsing_info,
                    "folder_info": folder_info
                }
                
                # Add original time input info if it was provided
                if trigger_time:
                    result["original_time_input"] = trigger_time
                    result["days_of_week_input"] = days_of_week
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to create time trigger. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error creating UiPath time trigger: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"
    
    @json_schema(
        description="Run/execute a UiPath process in Orchestrator. This starts a job from an existing process/release. The process must already exist in Orchestrator before it can be executed.",
        params={
            "process_name": {
                "type": "string",
                "description": "The name of the process/release to run. This process must already exist in Orchestrator.",
                "required": True
            },
            "input_arguments": {
                "type": "string",
                "description": "Optional input arguments for the process execution in JSON format. Default is empty object '{}'.",
                "default": "{}"
            },
            "folder_name": {
                "type": "string",
                "description": "Optional folder name where the process should be executed. If not provided, uses the default folder from environment variables.",
                "required": False
            }
        }
    )
    async def run_process(self, **kwargs) -> str:
        """Run/execute a UiPath process in Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            process_name = kwargs.get('process_name')
            input_arguments = kwargs.get('input_arguments', '{}')
            folder_name = kwargs.get('folder_name')
            
            if not process_name:
                return "ERROR: process_name is required"
            
            # Validate input_arguments is valid JSON
            try:
                parsed_args = json.loads(input_arguments)
            except json.JSONDecodeError:
                return "ERROR: input_arguments must be valid JSON format"
            
            # Resolve folder ID (either from folder_name or default)
            folder_id, folder_info = await self._resolve_folder_id(folder_name)
            if folder_info.startswith("ERROR:"):
                return folder_info
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with job execution permissions
            token = await self._get_auth_token("OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.Execution OR.Execution.Read OR.Execution.Write")
            
            # Step 1: Get the process release details
            self.logger.info(f"Getting release details for process '{process_name}'...")
            release_id, release_key = await self._get_process_release_details(process_name, token, config, folder_id)
            
            if not release_id:
                return f"ERROR: Could not find process '{process_name}' in Orchestrator. Make sure the process exists and is published."
            
            self.logger.info(f"Found process release - ID: {release_id}, Key: {release_key}")
            
            # Step 2: Start the job
            start_job_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": folder_id,
                "Content-Type": "application/json",
                "Accept": "application/json",
            }
            
            # Build the job start request payload
            payload = {
                "startInfo": {
                    "ReleaseKey": release_key,
                    "Strategy": "ModernJobsCount",  # Run on specific robots
                    "RobotIds": [],  # Empty array means any available robot
                    "Source": "Manual",  # Indicates manual execution
                    "InputArguments": input_arguments if input_arguments != '{}' else None
                }
            }
            
            # Remove InputArguments if it's empty to avoid API issues
            if not parsed_args:
                payload["startInfo"].pop("InputArguments", None)
            
            # Debug logging
            self.logger.info(f"UiPath Start Job URL: {start_job_url}")
            self.logger.info(f"UiPath Start Job Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
            self.logger.info(f"UiPath Start Job Payload: {json.dumps(payload, indent=2)}")
            
            # Make the API call to start the job
            response = requests.post(start_job_url, headers=headers, data=json.dumps(payload))
            
            # Debug response
            self.logger.info(f"UiPath Start Job Response Status: {response.status_code}")
            self.logger.info(f"UiPath Start Job Response Body: {response.text}")
            
            if response.status_code in [200, 201]:
                result_data = response.json()
                self.logger.info(f"Successfully started UiPath job for process: {process_name}")
                
                # Extract job information from response
                jobs_started = result_data.get("value", [])
                
                # Return a formatted success response
                result = {
                    "status": "success",
                    "message": f"Process '{process_name}' started successfully in {folder_info}",
                    "process_name": process_name,
                    "release_id": release_id,
                    "release_key": release_key,
                    "input_arguments": input_arguments,
                    "jobs_started_count": len(jobs_started),
                    "folder_info": folder_info
                }
                
                # Add job details if available
                if jobs_started:
                    result["jobs"] = []
                    for job in jobs_started:
                        job_info = {
                            "job_id": job.get("Id"),
                            "key": job.get("Key"),
                            "state": job.get("State"),
                            "creation_time": job.get("CreationTime"),
                            "robot_name": job.get("Robot", {}).get("Name") if job.get("Robot") else None
                        }
                        result["jobs"].append(job_info)
                
                return yaml.dump(result, allow_unicode=True)
                
            else:
                error_msg = f"Failed to start job for process '{process_name}'. Status code: {response.status_code}, Response: {response.text}"
                self.logger.error(error_msg)
                return f"ERROR: {error_msg}"
                
        except Exception as e:
            error_msg = f"Error running UiPath process: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"


    @json_schema(
        description="Copy UiPath project template from source_folder workspace to destination path, renaming folders and updating project.json files. Folders containing 'Name' in their name will have 'Name' replaced with the new project name. Each copied folder's project.json file will be updated with the new folder name and project description.",
        params={
            "destination_path": {
                "type": "string",
                "description": "Destination path where the project template should be copied (can be workspace UNC path like //workspace/path or local filesystem path)",
                "required": True
            },
            "new_project_name": {
                "type": "string",
                "description": "New project name to replace 'Name' in folder names and project.json files",
                "required": True
            }
        }
    )
    async def copy_project_template(self, **kwargs) -> str:
        """Copy UiPath project template from source_folder workspace to destination, renaming folders and updating project.json files."""
        try:
            tool_context = kwargs.get('tool_context')
            destination_path = kwargs.get('destination_path')
            new_project_name = kwargs.get('new_project_name')
            
            if not destination_path:
                return "ERROR: destination_path is required"
            
            if not new_project_name:
                return "ERROR: new_project_name is required"
            
            if not self.workspace_tools:
                return "ERROR: WorkspaceTools not available for workspace operations"
            
            # Source is always from source_folder workspace
            source_workspace_path = "//source_folder"
            
            # Parse the source workspace path to get workspace and relative path
            try:
                error, source_workspace, source_relative_path = self.workspace_tools._parse_unc_path(source_workspace_path)
                if error:
                    return f"ERROR: Invalid source workspace path: {error}"
            except Exception as e:
                return f"ERROR: Failed to parse source workspace path {source_workspace_path}: {str(e)}"
            
            # List all items in the source workspace root
            try:
                source_items = await source_workspace.list_internal(source_relative_path or "")
            except Exception as e:
                return f"ERROR: Failed to list source workspace contents: {str(e)}"
            
            # Filter to only directories
            source_folders = [item for item in source_items if item.is_directory]
            
            if not source_folders:
                return f"ERROR: No folders found in source workspace {source_workspace_path}"
            
            self.logger.info(f"Found {len(source_folders)} folders in source workspace: {[f.name for f in source_folders]}")
            
            # Determine if destination is workspace or filesystem path
            is_dest_workspace = destination_path.startswith('//')
            
            copied_folders = []
            errors = []
            
            # Process each folder
            for folder_item in source_folders:
                try:
                    folder_name = folder_item.name
                    
                    # Determine new folder name (replace "Name" with new_project_name)
                    if "Name" in folder_name:
                        new_folder_name = folder_name.replace("Name", new_project_name)
                    else:
                        new_folder_name = folder_name
                    
                    self.logger.info(f"Processing folder '{folder_name}' -> '{new_folder_name}'")
                    
                    # Source folder path in workspace
                    src_folder_path = f"{source_workspace_path}/{folder_name}"
                    
                    # Destination folder path
                    if is_dest_workspace:
                        dest_folder_path = f"{destination_path}/{new_folder_name}"
                    else:
                        import os
                        dest_folder_path = os.path.join(destination_path, new_folder_name)
                    
                    # Copy the entire folder tree
                    if is_dest_workspace:
                        # Workspace to workspace copy
                        copy_result = await self.workspace_tools.cp(src_path=src_folder_path, dest_path=dest_folder_path, tool_context=tool_context)
                        if copy_result.startswith("ERROR:"):
                            errors.append(f"Failed to copy '{folder_name}': {copy_result}")
                            continue
                    else:
                        # Workspace to filesystem copy
                        # This is more complex - we need to read all files and recreate the structure
                        try:
                            await self._copy_workspace_to_filesystem(src_folder_path, dest_folder_path, source_workspace)
                        except Exception as e:
                            errors.append(f"Failed to copy '{folder_name}' to filesystem: {str(e)}")
                            continue
                    
                    # Update project.json file in the copied folder
                    try:
                        await self._update_project_json(dest_folder_path, new_folder_name, new_project_name, is_dest_workspace)
                        copied_folders.append(f"'{folder_name}' -> '{new_folder_name}'")
                    except Exception as e:
                        errors.append(f"Copied '{folder_name}' but failed to update project.json: {str(e)}")
                        copied_folders.append(f"'{folder_name}' -> '{new_folder_name}' (project.json update failed)")
                    
                except Exception as e:
                    errors.append(f"Failed to process folder '{folder_name}': {str(e)}")
                    continue
            
            # Prepare result
            result = {
                "status": "success" if copied_folders and not errors else "partial" if copied_folders else "error",
                "message": f"Project template copied from {source_workspace_path} to {destination_path}",
                "new_project_name": new_project_name,
                "source_path": source_workspace_path,
                "destination_path": destination_path,
                "folders_processed": len(source_folders),
                "folders_copied": len(copied_folders),
                "copied_folders": copied_folders
            }
            
            if errors:
                result["errors"] = errors
                result["error_count"] = len(errors)
            
            return yaml.dump(result, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error copying UiPath project template: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"
    
    async def _copy_workspace_to_filesystem(self, src_workspace_path: str, dest_filesystem_path: str, source_workspace) -> None:
        """Copy a folder tree from workspace to filesystem."""
        import os
        import shutil
        
        # Parse the workspace path to get relative path
        error, _, src_relative_path = self.workspace_tools._parse_unc_path(src_workspace_path)
        if error:
            raise Exception(f"Invalid workspace path: {error}")
        
        # Create destination directory
        os.makedirs(dest_filesystem_path, exist_ok=True)
        
        # Recursively copy all files and folders
        await self._copy_workspace_folder_recursive(source_workspace, src_relative_path, dest_filesystem_path)
    
    async def _copy_workspace_folder_recursive(self, workspace, src_relative_path: str, dest_path: str) -> None:
        """Recursively copy workspace folder contents to filesystem."""
        import os
        
        try:
            # List items in current workspace folder
            items = await workspace.list_internal(src_relative_path)
            
            for item in items:
                src_item_path = f"{src_relative_path}/{item.name}" if src_relative_path else item.name
                dest_item_path = os.path.join(dest_path, item.name)
                
                if item.is_directory:
                    # Create directory and recurse
                    os.makedirs(dest_item_path, exist_ok=True)
                    await self._copy_workspace_folder_recursive(workspace, src_item_path, dest_item_path)
                else:
                    # Copy file
                    file_content = await workspace.read_bytes_internal(src_item_path)
                    with open(dest_item_path, 'wb') as f:
                        f.write(file_content)
                        
        except Exception as e:
            raise Exception(f"Failed to copy workspace folder {src_relative_path}: {str(e)}")
    
    async def _update_project_json(self, folder_path: str, new_folder_name: str, new_project_name: str, is_workspace: bool) -> None:
        """Update project.json file in the specified folder."""
        import json
        import os
        
        # Construct path to project.json
        if is_workspace:
            project_json_path = f"{folder_path}/project.json"
        else:
            project_json_path = os.path.join(folder_path, "project.json")
        
        try:
            # Check if project.json exists and read it
            if is_workspace:
                # Workspace path
                try:
                    project_json_content = await self.workspace_tools.read(path=project_json_path, tool_context={})
                    if project_json_content.startswith("ERROR:"):
                        self.logger.info(f"No project.json found in '{new_folder_name}' (workspace)")
                        return
                except Exception:
                    self.logger.info(f"No project.json found in '{new_folder_name}' (workspace)")
                    return
            else:
                # Filesystem path
                if not os.path.exists(project_json_path):
                    self.logger.info(f"No project.json found in '{new_folder_name}' (filesystem)")
                    return
                
                with open(project_json_path, 'r', encoding='utf-8') as f:
                    project_json_content = f.read()
            
            # Parse JSON
            try:
                data = json.loads(project_json_content)
            except json.JSONDecodeError as e:
                raise Exception(f"Invalid JSON in project.json: {str(e)}")
            
            # Update the data
            data["name"] = new_folder_name
            data["description"] = f"Project for {new_project_name}"
            
            # Write back the updated JSON
            updated_json = json.dumps(data, indent=4)
            
            if is_workspace:
                # Write to workspace
                write_result = await self.workspace_tools.write(path=project_json_path, data=updated_json, mode="write", tool_context={})
                if write_result.startswith("ERROR:"):
                    raise Exception(f"Failed to write updated project.json: {write_result}")
            else:
                # Write to filesystem
                with open(project_json_path, 'w', encoding='utf-8') as f:
                    f.write(updated_json)
            
            self.logger.info(f"Updated project.json in '{new_folder_name}'")
            
        except Exception as e:
            raise Exception(f"Failed to update project.json in '{new_folder_name}': {str(e)}")


# Register the toolset with WorkspaceTools dependency
Toolset.register(UiPathTools, required_tools=['WorkspaceTools'])