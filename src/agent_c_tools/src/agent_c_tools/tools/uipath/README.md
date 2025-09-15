# UiPath Integration Tools

This toolset provides integration with UiPath Cloud Orchestrator, allowing agents to manage assets and interact with UiPath automation infrastructure.

## Features

- **Authentication**: Secure OAuth2 authentication with UiPath Cloud
- **Asset Management**: Create and manage assets in UiPath Orchestrator
- **Configuration Management**: Environment variable-based configuration for security
- **Connection Testing**: Verify connectivity and configuration
- **Extensible Design**: Built to easily add more UiPath API functionality

## Configuration

Set the following environment variables:

```bash
# Required
UIPATH_ORG_NAME=your-organization-name
UIPATH_CLIENT_ID=your-client-id
UIPATH_CLIENT_SECRET=your-client-secret
UIPATH_FOLDER_ID=your-folder-id

# Optional (defaults to "DefaultTenant")
UIPATH_TENANT_NAME=your-tenant-name
```

## Available Tools

### `uipath-test_connection`
Test connectivity to UiPath Cloud and verify configuration.

**Parameters:** None

**Returns:** Connection status and configuration summary

### `uipath-get_config_info`
Get current configuration information (without exposing sensitive data).

**Parameters:** None

**Returns:** Configuration details with masked sensitive values

### `uipath-create_asset`
Create a new asset in UiPath Orchestrator.

**Parameters:**
- `asset_name` (required): Name of the asset
- `asset_value` (conditional): Value to store in the asset. Required for Text, Integer, and Boolean types. Ignored for Credential type.
- `asset_type` (optional): Type of asset (Text, Integer, Boolean, Credential) - defaults to "Text"
- `username` (conditional): Username for Credential assets. Required only when asset_type is "Credential"
- `password` (conditional): Password for Credential assets. Required only when asset_type is "Credential"
- `description` (optional): Description of the asset - defaults to "Created via Agent C"

**Returns:** Asset creation details including asset ID

## Asset Types

- **Text**: String values (URLs, file paths, configuration text)
- **Integer**: Numeric values (timeouts, retry counts) - provide as string, will be converted to integer
- **Boolean**: True/false values (feature flags) - use 'true' or 'false' as string values
- **Credential**: Secure username/password pairs - stored as JSON: {"username":"user","password":"pass"}

## Usage Examples

```python
# Test connection
await uipath_tools.test_connection()

# Create a text asset
await uipath_tools.create_asset(
    asset_name="DatabaseURL",
    asset_value="https://api.example.com",
    asset_type="Text",
    description="API endpoint for database access"
)

# Create an integer asset
await uipath_tools.create_asset(
    asset_name="RetryCount",
    asset_value="5",
    asset_type="Integer",
    description="Number of retry attempts"
)

# Create a boolean asset
await uipath_tools.create_asset(
    asset_name="EnableLogging",
    asset_value="true",
    asset_type="Boolean",
    description="Enable detailed logging in automation"
)

# Create a credential asset
await uipath_tools.create_asset(
    asset_name="LoginCredentials",
    asset_type="Credential",
    username="rohan",
    password="rohan123",
    description="Login credentials for external system"
)
```

## Security Considerations

- Client secrets are never logged or exposed in responses
- All authentication tokens are cached securely
- Configuration validation prevents missing required values
- Environment variables are used for all sensitive configuration

## Future Enhancements

This toolset is designed to be extended with additional UiPath functionality:

- Process deployment and management
- Job triggering and monitoring
- Robot management
- Queue item processing
- Audit log retrieval
- Tenant and folder management

## Error Handling

The toolset provides comprehensive error handling:

- Configuration validation with clear error messages
- Authentication failure detection and reporting
- API error details including status codes and responses
- Structured logging for troubleshooting

## Dependencies

- `requests`: HTTP client for API calls
- `yaml`: Response formatting
- Standard Agent C framework components