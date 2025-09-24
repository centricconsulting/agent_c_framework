# Excel Tool Tests

This directory contains comprehensive tests for the Excel tool following the official Agent C Tools testing guide.

## Test Structure

### 🏗️ Component Tests (`test_business_component.py`)
- **Purpose**: Test business logic classes in isolation with real functionality
- **Tests**: WorkbookManager, ConcurrencyManager, ExcelOperations, Models
- **Speed**: Fast (seconds)
- **Benefits**: Quick feedback during development, isolated testing

### 🔗 Integration Tests (`test_tool_integration.py`)
- **Purpose**: Test complete tool interface using ToolDebugger
- **Tests**: ExcelTools interface → Business logic → Real functionality
- **Speed**: Moderate (10-30 seconds)
- **Benefits**: End-to-end validation, tests actual user experience

## Running Tests

### Install Dependencies
```bash
cd src/agent_c_tools/tools/excel/tests
pip install -r test-requirements.txt
```

### Run All Tests
```bash
# Using test runner (recommended)
python run_tests.py

# Using pytest directly
pytest -v
```

### Run Specific Test Types
```bash
# Component tests only (fast feedback)
python run_tests.py component
pytest -m component -v

# Integration tests only (end-to-end validation)  
python run_tests.py integration
pytest -m integration -v
```

### Run Individual Test Files
```bash
pytest test_business_component.py -v
pytest test_tool_integration.py -v
```

## Test Categories

### Component Tests ✅

#### TestWorkbookManagerComponent
- `test_workbook_manager_initialization()` - Basic initialization
- `test_create_workbook_success()` - Workbook creation
- `test_get_workbook_info_with_workbook()` - Workbook info retrieval
- `test_get_workbook_info_without_workbook()` - Error handling
- `test_create_sheet_success()` - Sheet creation
- `test_create_duplicate_sheet_error()` - Duplicate sheet error handling

#### TestConcurrencyManagerComponent  
- `test_concurrency_manager_initialization()` - Basic initialization
- `test_reserve_rows_success()` - Row reservation functionality
- `test_multiple_reservations_no_overlap()` - Multi-agent coordination
- `test_get_operation_status()` - Status reporting

#### TestExcelOperationsComponent
- `test_excel_operations_initialization()` - Basic initialization
- `test_get_next_available_row_info_empty_sheet()` - Row calculation
- `test_get_next_available_row_info_with_reserved_rows()` - Reserved row handling
- `test_read_sheet_data_empty_sheet()` - Reading empty sheets
- `test_read_sheet_data_nonexistent_sheet()` - Error handling

#### TestModelsComponent
- `test_operation_result_to_dict()` - OperationResult serialization
- `test_write_result_to_dict()` - WriteResult serialization
- `test_read_result_to_dict()` - ReadResult serialization

### Integration Tests ✅

#### TestExcelToolsIntegration
- `test_tool_availability()` - Tool registration and availability
- `test_create_workbook_integration()` - Workbook creation via tool interface
- `test_list_sheets_after_create()` - Sheet listing functionality
- `test_create_sheet_integration()` - Sheet creation via tool interface
- `test_reserve_rows_integration()` - Row reservation via tool interface
- `test_append_records_integration()` - Record appending functionality
- `test_read_sheet_data_integration()` - Data reading functionality
- `test_tool_error_handling()` - Error handling through tool interface
- `test_tool_parameter_validation()` - Parameter validation

## Test Configuration

### pytest.ini
- Configured with proper markers (`component`, `integration`, `asyncio`)
- Async test support enabled
- Strict configuration for reliability

### test-requirements.txt
- pytest>=7.0.0
- pytest-asyncio>=0.21.0  
- python-dotenv>=1.0.0
- openpyxl>=3.0.0

## Expected Results

### Component Tests
- ✅ All business logic classes initialize correctly
- ✅ Workbook creation and management works
- ✅ Multi-agent row reservation prevents conflicts
- ✅ Excel operations handle various scenarios
- ✅ Data models serialize correctly

### Integration Tests  
- ✅ Tool is properly registered and available
- ✅ All tool methods are accessible via ToolDebugger
- ✅ End-to-end workflows function correctly
- ✅ Error handling works through tool interface
- ✅ Parameter validation functions properly

## Troubleshooting

### Common Issues

#### Missing Dependencies
```bash
pip install -r test-requirements.txt
```

#### ToolDebugger Not Found
- Integration tests will skip if ToolDebugger is not available
- Check that tool_debugger is in the expected path
- Component tests will still run independently

#### Import Errors
- Tests use absolute imports (agent_c_tools.tools.excel.*)
- Path resolution is handled automatically in test files
- Check that PYTHONPATH includes the src directory

### Test Debugging
```bash
# Run single test with verbose output
pytest test_business_component.py::TestWorkbookManagerComponent::test_create_workbook_success -v

# Show print statements
pytest test_business_component.py -v -s

# Run with coverage (if installed)
pytest --cov=agent_c_tools.tools.excel test_business_component.py
```

## Test Philosophy

Following the official Agent C Tools testing guide:

- **✅ Test real functionality** - No excessive mocking
- **✅ Component tests** give fast feedback on business logic
- **✅ Integration tests** validate complete user experience  
- **✅ Focus on behavior** not implementation details
- **✅ Maintain test independence** - Tests don't depend on each other

This testing approach ensures the Excel tool is **reliable, maintainable, and actually works**! 🧪✅