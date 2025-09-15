"""Prompt content for UiPath integration tools."""

UIPATH_PROMPT = """
# UiPath Integration Tools

You have access to UiPath Cloud Orchestrator integration tools that allow you to:

## Available Tools:
- **uipath-test_connection**: Test connectivity to UiPath Cloud
- **uipath-get_config_info**: Get current configuration information
- **uipath-create_asset**: Create assets in UiPath Orchestrator
- **uipath-create_queue**: Create queues in UiPath Orchestrator

## Key Concepts:

### UiPath Assets
Assets in UiPath are configuration values that robots can access during automation execution. They can store:
- **Text**: String values like URLs, file paths, or configuration text
- **Integer**: Numeric values
- **Boolean**: True/false values  
- **Credential**: Secure username/password pairs

### Asset Types and Usage:
- Use **Text** assets for most configuration values (URLs, file paths, settings)
- Use **Integer** assets for numeric configurations (timeouts, retry counts) - provide the number as a string
- Use **Boolean** assets for feature flags or yes/no settings - use 'true' or 'false' as the value
- Use **Credential** assets for secure authentication information - provide separate username and password parameters

### UiPath Queues
Queues in UiPath are containers for work items that robots can process. They provide:
- **Work item management**: Store data that needs to be processed by robots
- **Retry mechanisms**: Automatic retry of failed items with configurable settings
- **Priority handling**: Process items based on priority levels
- **Reference tracking**: Optional unique reference enforcement for items

### Queue Configuration Options:
- **max_number_of_retries**: How many times to retry failed items (0-10)
- **auto_retry**: Whether to enable automatic retry (defaults to false)
- **unique_ref**: Prevent duplicate reference IDs (defaults to false)
- **processing_type**: Set to "Multiple" for standard queue processing

## Best Practices:
1. **Always test connection first** using `uipath-test_connection` before creating assets
2. **Use descriptive asset names** that clearly indicate their purpose
3. **Add meaningful descriptions** to help other users understand the asset's purpose
4. **Choose appropriate asset types** based on the data being stored
5. **Verify asset creation** by checking the returned asset ID

## Configuration Requirements:
The UiPath tools require these environment variables to be set:
- `UIPATH_ORG_NAME`: Your UiPath organization name
- `UIPATH_CLIENT_ID`: OAuth2 client ID for authentication
- `UIPATH_CLIENT_SECRET`: OAuth2 client secret for authentication
- `UIPATH_TENANT_NAME`: Tenant name (defaults to "DefaultTenant")
- `UIPATH_FOLDER_ID`: The folder ID where assets will be created

## Error Handling:
- If configuration is missing, the tools will return clear error messages
- Authentication failures will be logged and reported
- API errors include status codes and response details for troubleshooting

## Example Workflows:

### Asset Management:
1. **Test connectivity**: `uipath-test_connection`
2. **Check configuration**: `uipath-get_config_info`
3. **Create a text asset**: `uipath-create_asset` with asset_name="MyConfig", asset_value="some text", asset_type="Text"
4. **Create an integer asset**: `uipath-create_asset` with asset_name="Timeout", asset_value="30", asset_type="Integer"
5. **Create a boolean asset**: `uipath-create_asset` with asset_name="FeatureEnabled", asset_value="true", asset_type="Boolean"
6. **Create a credential asset**: `uipath-create_asset` with asset_name="LoginCreds", asset_type="Credential", username="rohan", password="rohan123"

### Queue Management:
7. **Create a basic queue**: `uipath-create_queue` with queue_name="ProcessInvoices", description="Queue for invoice processing"
8. **Create a queue with custom retry settings**: `uipath-create_queue` with queue_name="CriticalTasks", max_number_of_retries=3, auto_retry=true
9. **Create a queue with unique references**: `uipath-create_queue` with queue_name="UniqueOrders", unique_ref=true

Remember: Assets and queues created through these tools can be accessed by UiPath robots in your automation workflows.
"""