"""UiPath integration toolset for Agent C framework."""

import json
import os
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
            'auth_scope': self._get_config_value('AUTH_SCOPE')
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
    
    async def _get_auth_token(self, scope: str = "OR.Administration OR.Administration.Read OR.Administration.Write OR.Analytics OR.Analytics.Read OR.Analytics.Write OR.Assets OR.Assets.Read OR.Assets.Write OR.Audit OR.Audit.Read OR.Audit.Write OR.AutomationSolutions.Access OR.BackgroundTasks OR.BackgroundTasks.Read OR.BackgroundTasks.Write OR.Execution OR.Execution.Read OR.Execution.Write OR.Folders OR.Folders.Read OR.Folders.Write OR.Hypervisor OR.Hypervisor.Read OR.Hypervisor.Write OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.License OR.License.Read OR.License.Write OR.Machines OR.Machines.Read OR.Machines.Write OR.ML OR.ML.Read OR.ML.Write OR.Monitoring OR.Monitoring.Read OR.Monitoring.Write OR.Queues OR.Queues.Read OR.Queues.Write OR.Robots OR.Robots.Read OR.Robots.Write OR.Settings OR.Settings.Read OR.Settings.Write OR.Tasks OR.Tasks.Read OR.Tasks.Write OR.TestDataQueues OR.TestDataQueues.Read OR.TestDataQueues.Write OR.TestSetExecutions OR.TestSetExecutions.Read OR.TestSetExecutions.Write OR.TestSets OR.TestSets.Read OR.TestSets.Write OR.TestSetSchedules OR.TestSetSchedules.Read OR.TestSetSchedules.Write OR.Users OR.Users.Read OR.Users.Write OR.Webhooks OR.Webhooks.Read OR.Webhooks.Write ") -> str:
        """Get authentication token from UiPath Cloud."""
        try:
            config = self._validate_config()
            # scope = config['auth_scope']
            # self.logger.info(f"Using auth scope: {scope}")
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
            
            if not queue_name:
                return "ERROR: queue_name is required"
            
            # Validate parameters
            if not isinstance(max_retries, int) or max_retries < 0 or max_retries > 10:
                return "ERROR: max_number_of_retries must be an integer between 0 and 10"
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with queue permissions
            token = await self._get_auth_token("OR.Queues OR.Queues.Read OR.Queues.Write")
            
            # Prepare the API request for queue creation
            create_queue_url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/QueueDefinitions"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": str(config['folder_id']),
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
                    "message": f"Queue '{queue_name}' created successfully",
                    "queue_id": result_data.get("Id"),
                    "queue_name": queue_name,
                    "description": description,
                    "max_number_of_retries": max_retries,
                    "auto_retry": auto_retry,
                    "unique_ref": unique_ref,
                    "processing_type": "Multiple"
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
            }
        }
    )
    async def upload_package(self, **kwargs) -> str:
        """Upload a UiPath package to Orchestrator."""
        try:
            tool_context = kwargs.get('tool_context')
            nupkg_path = kwargs.get('nupkg_path')
            extra_fields = kwargs.get('extra_fields', {})
            
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
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with execution permissions
            token = await self._get_auth_token("OR.Execution OR.Execution.Read OR.Execution.Write")
            
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
                "X-UIPATH-OrganizationUnitId": str(config['folder_id'])
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
                    "message": f"Package '{os.path.basename(nupkg_path)}' uploaded successfully",
                    "package_name": os.path.basename(nupkg_path),
                    "package_path": nupkg_path,
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
            }
        }
    )
    async def upload_package_from_workspace(self, **kwargs) -> str:
        """Upload a UiPath package from workspace to Orchestrator."""
        try:
            workspace_path = kwargs.get('workspace_nupkg_path')
            
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
                "X-UIPATH-OrganizationUnitId": str(config['folder_id'])
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
                    "message": f"Package '{filename}' uploaded successfully from workspace",
                    "package_name": filename,
                    "workspace_path": workspace_path,
                    "file_size_bytes": len(file_content),
                    "response_data": response_data
                }
                return yaml.dump(result, allow_unicode=True)
            else:
                return f"ERROR: Upload failed. Status: {response.status_code}, Response: {response.text}"
                
        except Exception as e:
            return f"ERROR: Exception during workspace upload: {str(e)}"

    async def _get_package_version(self, package_name: str, token: str, config: Dict[str, str]) -> Optional[str]:
        """Get the latest version of a package from UiPath Orchestrator."""
        try:
            url = f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}/orchestrator_/odata/Processes"
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": str(config['folder_id']),
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
    
    async def _get_entry_point_id(self, package_name: str, package_version: str, token: str, config: Dict[str, str]) -> Optional[str]:
        """Get the entry point ID for a package version."""
        try:
            url = (
                f"https://cloud.uipath.com/{config['org_name']}/{config['tenant_name']}"
                f"/orchestrator_/odata/Processes/UiPath.Server.Configuration.OData.GetPackageEntryPointsV2(key='{package_name}:{package_version}')"
            )
            
            headers = {
                "Authorization": f"Bearer {token}",
                "X-UIPATH-TenantName": config['tenant_name'],
                "X-UIPATH-OrganizationUnitId": str(config['folder_id']),
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
            
            if not process_name:
                return "ERROR: process_name is required"
            
            # Use the same name for both process and package
            package_name = process_name
            
            # Validate input_arguments is valid JSON
            try:
                json.loads(input_arguments)
            except json.JSONDecodeError:
                return "ERROR: input_arguments must be valid JSON format"
            
            # Get configuration
            config = self._validate_config()
            
            # Get authentication token with execution permissions
            token = await self._get_auth_token("OR.Administration OR.Administration.Read OR.Administration.Write OR.Analytics OR.Analytics.Read OR.Analytics.Write OR.Assets OR.Assets.Read OR.Assets.Write OR.Audit OR.Audit.Read OR.Audit.Write OR.AutomationSolutions.Access OR.BackgroundTasks OR.BackgroundTasks.Read OR.BackgroundTasks.Write OR.Execution OR.Execution.Read OR.Execution.Write OR.Folders OR.Folders.Read OR.Folders.Write OR.Hypervisor OR.Hypervisor.Read OR.Hypervisor.Write OR.Jobs OR.Jobs.Read OR.Jobs.Write OR.License OR.License.Read OR.License.Write OR.Machines OR.Machines.Read OR.Machines.Write OR.ML OR.ML.Read OR.ML.Write OR.Monitoring OR.Monitoring.Read OR.Monitoring.Write OR.Queues OR.Queues.Read OR.Queues.Write OR.Robots OR.Robots.Read OR.Robots.Write OR.Settings OR.Settings.Read OR.Settings.Write OR.Tasks OR.Tasks.Read OR.Tasks.Write OR.TestDataQueues OR.TestDataQueues.Read OR.TestDataQueues.Write OR.TestSetExecutions OR.TestSetExecutions.Read OR.TestSetExecutions.Write OR.TestSets OR.TestSets.Read OR.TestSets.Write OR.TestSetSchedules OR.TestSetSchedules.Read OR.TestSetSchedules.Write OR.Users OR.Users.Read OR.Users.Write OR.Webhooks OR.Webhooks.Read OR.Webhooks.Write ")
            
            # Step 1: Get the package version
            self.logger.info(f"Getting version for package '{package_name}' (same as process name)...")
            package_version = await self._get_package_version(package_name, token, config)
            
            if not package_version:
                return f"ERROR: Could not find package '{package_name}' in Orchestrator. Make sure the package with name '{process_name}' is uploaded first."
            
            self.logger.info(f"Found package version: {package_version}")
            
            # Step 2: Get the entry point ID
            self.logger.info(f"Getting entry point ID for package '{package_name}' version '{package_version}'...")
            entry_point_id = await self._get_entry_point_id(package_name, package_version, token, config)
            
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
                "X-UIPATH-OrganizationUnitId": str(config['folder_id']),
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
                    "message": f"Process '{process_name}' created successfully from package '{package_name}' (same name)",
                    "process_id": result_data.get("Id"),
                    "process_name": process_name,
                    "package_name": package_name,
                    "package_version": package_version,
                    "entry_point_id": entry_point_id,
                    "description": description,
                    "input_arguments": input_arguments
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


# Register the toolset with WorkspaceTools dependency
Toolset.register(UiPathTools, required_tools=['WorkspaceTools'])