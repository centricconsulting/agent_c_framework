"""Prompt content for UiPath integration tools."""

UIPATH_PROMPT = """
# UiPath Integration Tools

You have access to UiPath Cloud Orchestrator integration tools that allow you to:

## Available Tools:
- **uipath-test_connection**: Test connectivity to UiPath Cloud
- **uipath-get_config_info**: Get current configuration information
- **uipath-create_asset**: Create assets in UiPath Orchestrator
- **uipath-create_queue**: Create queues in UiPath Orchestrator
- **uipath-upload_package**: Upload .nupkg packages to UiPath Orchestrator
- **uipath-upload_package_from_workspace**: Upload packages from workspace to Orchestrator
- **uipath-create_process**: Create a process/release from an uploaded package
- **uipath-create_time_trigger**: Create time-based triggers to schedule process execution

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

### UiPath Time Triggers
Time triggers in UiPath allow you to schedule process executions based on time patterns. You can specify time in human-readable format or use cron expressions. They provide:
- **Scheduled execution**: Run processes at specific times or intervals
- **Human-readable time input**: Use formats like '9 AM', '2:30 PM', '14:30'
- **Flexible day selection**: Specify days like 'MON,TUE,FRI' or 'MON-FRI'
- **Cron-based scheduling**: Full cron expression support for complex patterns
- **Timezone support**: Schedule across different time zones
- **Enable/disable control**: Turn triggers on or off as needed

### Time Trigger Configuration:
- **trigger_time**: Human-readable time (e.g., '9 AM', '2:30 PM', '14:30', '9') - automatically converted to cron
- **days_of_week**: Days specification (e.g., 'MON,TUE,FRI', 'MON-FRI', '*' for daily)
- **cron_expression**: Complete cron format (e.g., '0 0 10 * * ?' for daily at 10 AM) - used if trigger_time not provided
- **timezone_id**: Timezone for scheduling (e.g., 'India Standard Time', 'UTC')
- **enabled**: Whether the trigger is active (defaults to true)
- **advanced_cron_details**: JSON format for advanced scheduling options

### Supported Time Formats:
- **12-hour format**: '9 AM', '2:30 PM', '11:45 AM'
- **24-hour format**: '09:00', '14:30', '23:15'
- **Hour only**: '9', '14', '23' (assumes :00 minutes)
- **Flexible input**: Spaces and case don't matter ('9am', '2:30 pm', '9 AM')

### Common Cron Expressions:
- Daily at 10 AM: `0 0 10 * * ?`
- Every hour: `0 0 * * * ?`
- Every 30 minutes: `0 0,30 * * * ?`
- Monday to Friday at 9 AM: `0 0 9 ? * MON-FRI`
- First day of every month at midnight: `0 0 0 1 * ?`

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

### Package and Process Management:
10. **Upload a package**: `uipath-upload_package` with nupkg_path="/path/to/package.nupkg"
11. **Upload from workspace**: `uipath-upload_package_from_workspace` with workspace_nupkg_path="//workspace/package.nupkg"
12. **Create a process**: `uipath-create_process` with process_name="MyProcess", description="Automated process"

### Time Trigger Management:
13. **Create a simple time trigger**: `uipath-create_time_trigger` with process_name="MyProcess", trigger_name="DailyRun", trigger_time="9 AM"
14. **Create a weekday trigger**: `uipath-create_time_trigger` with process_name="WeeklyReport", trigger_name="WeekdayTrigger", trigger_time="2:30 PM", days_of_week="MON,TUE,FRI"
15. **Create with 24-hour format**: `uipath-create_time_trigger` with process_name="NightProcess", trigger_name="NightRun", trigger_time="14:30", days_of_week="MON-FRI"
16. **Create with cron expression**: `uipath-create_time_trigger` with process_name="MonthlyProcess", trigger_name="MonthlyRun", cron_expression="0 0 0 1 * ?", enabled=true

Remember: Assets, queues, processes, and triggers created through these tools can be accessed and managed in UiPath Orchestrator and used by UiPath robots in your automation workflows.
"""