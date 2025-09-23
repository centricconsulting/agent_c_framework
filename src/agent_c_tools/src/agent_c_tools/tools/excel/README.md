# Excel Tools for Agent C Framework

A comprehensive Excel manipulation toolset designed for high-volume operations and multi-agent workflows.

## Overview

The Excel Tools provide powerful Excel file manipulation capabilities specifically designed for:
- **High-volume data writing** (thousands to millions of records)
- **Multi-agent concurrent operations** with conflict resolution
- **Memory-efficient processing** for large datasets
- **Generic data processing** from any source
- **Bulk operations and batch processing**

## Key Features

### 🚀 **Concurrent-Safe Operations**
- **Row Reservation System**: Agents can reserve specific row ranges to prevent conflicts
- **Append-Only Operations**: Multiple agents can safely append data simultaneously
- **Automatic Conflict Resolution**: Tool handles coordination transparently
- **Progress Tracking**: Real-time progress updates for long-running operations

### 💾 **Memory-Efficient Processing**
- **Chunked Processing**: Large datasets processed in configurable batches
- **Streaming Operations**: Avoid loading entire workbooks into memory
- **Smart Caching**: Large datasets cached when they exceed token limits
- **Automatic Cleanup**: Memory management for long-running operations

### 📊 **Enterprise Scale**
- **High Volume**: Designed for thousands to millions of records
- **Multi-Sheet Support**: Automatic sheet partitioning for large datasets
- **Operation Tracking**: Monitor and debug large operations
- **Error Recovery**: Robust error handling for partial failures

## Core Methods

### Workbook Operations
```python
# Create new workbook
await excel_tool.create_workbook()

# Load existing workbook
await excel_tool.load_workbook(path="//workspace/data/file.xlsx")

# Save workbook
await excel_tool.save_workbook(path="//workspace/output/result.xlsx", create_backup=True)
```

### Sheet Management
```python
# List all sheets
await excel_tool.list_sheets()

# Create new sheet
await excel_tool.create_sheet(sheet_name="DataSheet", index=0)

# Read sheet data with caching for large datasets
await excel_tool.read_sheet_data(
    sheet_name="DataSheet", 
    start_row=1, 
    include_headers=True,
    max_rows=10000
)
```

### High-Volume Writing
```python
# Simple append (safe for concurrent use)
await excel_tool.append_records(
    records=data_rows,
    sheet_name="Results", 
    headers=["Name", "Age", "City"],
    chunk_size=10000
)

# Coordinated multi-agent writing
reservation = await excel_tool.reserve_rows(
    row_count=1000, 
    sheet_name="Results",
    agent_id="data_agent_1"
)

await excel_tool.write_to_reservation(
    reservation_id=reservation["reservation_id"],
    records=processed_data
)
```

### Operation Monitoring
```python
# Get next available row for coordination
await excel_tool.get_next_available_row(sheet_name="Results")

# Monitor operation status
await excel_tool.get_operation_status(operation_id="excel_op_123456")

# Load cached data for large datasets
await excel_tool.load_cached_data(cache_key="excel_data_789012")
```

## Multi-Agent Workflow Example

Here's how multiple agents can safely write to the same Excel file:

```python
# Agent 1: Data Processing Agent
async def data_processor_agent():
    # Process data from any source
    processed_data = process_data_source("data_file.csv")
    
    # Convert to Excel format
    excel_records = [[row['name'], row['value'], row['date']] for row in processed_data]
    
    # Safely append to shared workbook
    result = await excel_tool.append_records(
        records=excel_records,
        sheet_name="Processed_Data",
        agent_id="processor_1"
    )
    
    return result

# Agent 2: Validation Agent  
async def validator_agent():
    # Reserve space for validation results
    reservation = await excel_tool.reserve_rows(
        row_count=100,
        sheet_name="Validation_Results",
        agent_id="validator_1"
    )
    
    # Process validation
    validation_results = perform_data_validation()
    
    # Write to reserved space
    result = await excel_tool.write_to_reservation(
        reservation_id=reservation["reservation_id"],
        records=validation_results
    )
    
    return result
```

## Configuration Options

### Performance Tuning
- **chunk_size**: Records per batch (default: 10,000)
- **max_rows**: Maximum rows to read at once (default: 10,000)

### Memory Management
- **MAX_EXCEL_TOKEN_SIZE**: Token limit before caching (35,000)
- **MAX_ROWS_PER_SHEET**: Rows per sheet limit (1,000,000)
- **Cache expiration**: 1 hour for cached data

## Error Handling

The tool provides comprehensive error handling:
- **Validation errors**: Missing parameters, invalid sheet names
- **Concurrency errors**: Reservation conflicts, write collisions
- **Memory errors**: Dataset too large, token limits exceeded
- **File errors**: Path not found, permission denied, corrupted files

All errors are returned as JSON responses with clear error messages.

## Integration with Other Tools

The Excel tool integrates seamlessly with:
- **WorkspaceTools**: File I/O using UNC paths
- **DataframeTools**: Convert between Excel and pandas DataFrames
- **CobolTools**: Specialized legacy data processing
- **VisualizationTools**: Export charts and graphs to Excel

## Best Practices

### For High-Volume Operations
1. Use chunked processing for large datasets
2. Monitor memory usage and use caching when needed
3. Implement progress tracking for user feedback
4. Handle cancellation gracefully

### For Multi-Agent Scenarios
1. Use `append_records()` for simple concurrent writing
2. Use row reservations for guaranteed positioning
3. Coordinate through operation status monitoring
4. Implement retry logic for failed operations

### For Data Integration
1. Ensure consistent data formatting across sources
2. Use appropriate batch sizes for memory efficiency
3. Validate data integrity after operations
4. Handle null/empty values consistently

## Dependencies

- **pandas**: DataFrame operations and Excel I/O
- **openpyxl**: Advanced Excel file manipulation
- **agent_c_core**: Base toolset framework
- **WorkspaceTools**: File system integration

## Thread Safety

The Excel tool is designed for concurrent use:
- **Write operations**: Protected by async locks
- **Row reservations**: Atomic reservation system
- **Memory management**: Thread-safe caching
- **Progress tracking**: Safe across multiple agents

This tool is production-ready for enterprise-scale data processing projects and high-volume data migration scenarios from any source system.