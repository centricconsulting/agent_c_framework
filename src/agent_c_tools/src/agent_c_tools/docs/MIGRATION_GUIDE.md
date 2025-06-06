# Tool Migration Guide

## Refactoring Existing Tools to Clean Architecture

This guide provides a **step-by-step process** for migrating existing Agent C tools to the clean architecture pattern with proper testing.

## Table of Contents

1. [Migration Overview](#migration-overview)
2. [Assessment Phase](#assessment-phase)
3. [Refactoring Steps](#refactoring-steps)
4. [Testing Implementation](#testing-implementation)
5. [Migration Examples](#migration-examples)
6. [Best Practices](#best-practices)
7. [Common Pitfalls](#common-pitfalls)
8. [Validation Checklist](#validation-checklist)

---

## Migration Overview

### **Why Migrate?**

Migrating existing tools provides:

- ✅ **Better maintainability** - Clear separation of concerns
- ✅ **Improved testability** - Business logic can be tested independently
- ✅ **Enhanced reusability** - Logic can be used outside tool context
- ✅ **Easier debugging** - Clear boundaries between components
- ✅ **Professional quality** - Industry standard architecture patterns

### **Migration Philosophy**

The goal is to transform tools from:

```
❌ BEFORE: Mixed Architecture
Tool Class (50+ lines)
├── Tool interface (schema, registration)
├── Business logic (API calls, data processing)
├── Error handling (complex exception management)
└── Data formatting (JSON transformation)
```

To:

```
✅ AFTER: Clean Architecture  
Tool Class (25 lines)        Business Logic Class/Functions
├── Tool interface           ├── API calls
├── Parameter collection     ├── Data processing  
└── Simple delegation   ────→├── Error handling
                              └── Data formatting
```

### **Success Metrics**

A successful migration achieves:

- 🎯 **Tool class ≤ 10 lines per function**
- 🎯 **Zero business logic** in tool class
- 🎯 **Complete test coverage** (component + integration)
- 🎯 **No breaking changes** to tool interface
- 🎯 **Improved error handling** and user experience

---

## Assessment Phase

### **Step 1: Analyze Current Tool**

Before starting migration, assess the existing tool:

#### **Tool Complexity Analysis**

```python
# Run this analysis on your existing tool
def analyze_tool_complexity(tool_file_path):
    with open(tool_file_path, 'r') as f:
        lines = f.readlines()
    
    total_lines = len(lines)
    method_lines = []
    
    # Count lines per method (rough analysis)
    print(f\"Total lines: {total_lines}\")
    print(f\"Complexity indicators:\")
    
    # Look for complexity indicators
    content = ''.join(lines)
    indicators = {
        'HTTP calls': content.count('await') + content.count('requests.'),
        'JSON processing': content.count('json.'),
        'Error handling': content.count('try:') + content.count('except'),
        'Data transformation': content.count('for ') + content.count('map('),
        'String formatting': content.count('f\"') + content.count('.format(')
    }
    
    for indicator, count in indicators.items():
        print(f\"  {indicator}: {count}\")
    
    return total_lines, indicators

# Example usage
# total, indicators = analyze_tool_complexity('path/to/your/tool.py')
```

#### **Identify Business Logic**

Look for these patterns in your existing tool:

```python
# ❌ Business logic patterns to extract:

# 1. External API calls
async def tool_method(self, **kwargs):
    # This should be moved to business logic class
    response = await httpx.get(f\"https://api.example.com/{param}\")
    
# 2. Data processing loops
for item in data:
    processed_item = transform_data(item)
    
# 3. Complex error handling
try:
    result = complex_operation()
except SpecificException as e:
    return handle_specific_error(e)
except AnotherException as e:
    return handle_another_error(e)
    
# 4. JSON transformation
formatted_data = {
    \"field1\": raw_data.get('original_field1'),
    \"field2\": process_field(raw_data.get('original_field2')),
    \"nested\": {
        \"subfield\": raw_data.get('sub').get('field')
    }
}
```

#### **Dependency Analysis**

Identify what the tool depends on:

```python
# Check imports and dependencies
import httpx          # External API calls
import json          # Data processing
import pandas        # Data manipulation
from .util import *  # Existing utility classes
```

### **Step 2: Plan the Migration**

Based on your analysis, choose the appropriate pattern:

#### **Pattern A: Extend Existing Class** (Preferred)
- Use when utility classes already exist (`util/` directory)
- Add new method to existing domain class
- Example: Weather tool extending `Weather` class

#### **Pattern B: Create Service Function** 
- Use when no suitable existing class exists
- Create focused function for tool-specific logic
- Keep it simple and focused

#### **Pattern C: Create Service Class**
- Use only when complex state management is needed
- Avoid if possible (tends to create unnecessary abstraction)

---

## Refactoring Steps  

### **Step 1: Backup and Setup**

```bash
# Create backup of existing tool
cp tool.py tool.py.backup

# Create test directory structure
mkdir tests
cd tests
touch __init__.py
touch test-requirements.txt
touch pytest.ini
touch run_tests.py
touch README.md
```

### **Step 2: Extract Business Logic**

#### **Option A: Extend Existing Class (Recommended)**

```python
# Before: Business logic mixed in tool.py
class ExampleTools(Toolset):
    async def process_data(self, **kwargs):
        param = kwargs.get('param')
        
        # ❌ This should be extracted
        client = ExistingClass()
        raw_data = await client.get_data(param)
        
        formatted_data = {
            \"processed\": raw_data.get('field1'),
            \"formatted\": raw_data.get('field2').upper()
        }
        
        return json.dumps(formatted_data)

# After: Extend existing class
# In util/existing_class.py (or wherever the class lives)
class ExistingClass:
    # ... existing methods ...
    
    async def get_formatted_data_for_tool(self, param: str) -> str:
        \"\"\"Get data formatted for tool consumption.\"\"\"
        try:
            raw_data = await self.get_data(param)
            
            formatted_data = {
                \"processed\": raw_data.get('field1'),
                \"formatted\": raw_data.get('field2').upper()
            }
            
            return json.dumps(formatted_data)
        except Exception as e:
            self.logger.error(f\"Error getting formatted data: {e}\")
            return f\"Error: {str(e)}\"

# In tool.py (simplified)
class ExampleTools(Toolset):
    async def process_data(self, **kwargs) -> str:
        param = kwargs.get('param')
        
        # ✅ Simple delegation
        client = ExistingClass()
        return await client.get_formatted_data_for_tool(param)
```

#### **Option B: Create Service Function**

```python
# Create new file: example_service.py
async def process_data_for_tool(param: str, config: dict = None) -> str:
    \"\"\"Process data for tool consumption.\"\"\"
    try:
        # Business logic implementation
        result = await process_business_logic(param, config)
        return json.dumps(result)
    except Exception as e:
        logging.error(f\"Error processing data: {e}\")
        return f\"Error: {str(e)}\"

# In tool.py (simplified)
from .example_service import process_data_for_tool

class ExampleTools(Toolset):
    async def process_data(self, **kwargs) -> str:
        param = kwargs.get('param')
        config = kwargs.get('config')
        
        # ✅ Simple delegation
        return await process_data_for_tool(param, config)
```

### **Step 3: Simplify Tool Class**

Transform your tool class to focus only on interface concerns:

```python
# Clean tool class template
from .business_logic import business_function  # or existing class
from agent_c.toolsets import json_schema, Toolset
import logging

class YourTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='your_tool', use_prefix=False)
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        'Description of what your tool does',
        {
            'param1': {
                'type': 'string',
                'description': 'Description of param1',
                'required': True
            },
            'param2': {
                'type': 'string', 
                'description': 'Description of param2',
                'required': False
            }
        }
    )
    async def your_tool_method(self, **kwargs) -> str:
        \"\"\"
        Tool method description.
        
        Args:
            **kwargs: Tool arguments
            
        Returns:
            JSON string with results
        \"\"\"
        # Extract parameters
        param1 = kwargs.get('param1')
        param2 = kwargs.get('param2')
        
        # Delegate to business logic (choose one pattern)
        
        # Pattern A: Existing class extension
        business_client = ExistingClass(config='tool_config')
        return await business_client.method_for_tool(param1, param2)
        
        # Pattern B: Service function
        # return await business_function(param1, param2)

Toolset.register(YourTools)
```

### **Step 4: Implement Component Tests**

```python
# tests/test_your_business_component.py
\"\"\"
Component tests for [your business logic].
These tests call the business logic directly to validate functionality.
\"\"\"
import pytest
import json
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from util.your_class import YourClass  # or import your_service

class TestYourBusinessComponent:
    \"\"\"Component tests for business logic.\"\"\"
    
    def setup_method(self):
        self.business_client = YourClass(config='test_config')
    
    @pytest.mark.component
    @pytest.mark.asyncio
    async def test_main_functionality_success(self):
        \"\"\"Test successful operation.\"\"\"
        result = await self.business_client.method_for_tool('test_input')
        
        assert not result.startswith('Error:')
        
        # Validate JSON structure
        data = json.loads(result)
        assert 'expected_field' in data
        assert isinstance(data['expected_field'], str)
    
    @pytest.mark.component
    @pytest.mark.asyncio
    async def test_error_handling(self):
        \"\"\"Test error handling.\"\"\"
        result = await self.business_client.method_for_tool('invalid_input')
        
        assert result.startswith('Error:')
```

### **Step 5: Implement Integration Tests**

```python
# tests/test_your_tool_integration.py
\"\"\"
Integration tests for YourTools using debug_tool.
\"\"\"
import pytest
import sys
import os

# Import debug_tool
tool_debugger_path = os.path.join(
    os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), 
    'tool_debugger'
)
sys.path.insert(0, tool_debugger_path)

from debug_tool import ToolDebugger

class TestYourToolsIntegration:
    \"\"\"Integration tests for YourTools.\"\"\"
    
    def setup_method(self):
        self.debugger = None
    
    def teardown_method(self):
        if self.debugger:
            pass
    
    async def setup_tool(self):
        self.debugger = ToolDebugger(init_local_workspaces=False)
        await self.debugger.setup_tool(
            tool_import_path='agent_c_tools.YourTools',
            tool_opts={}
        )
        return self.debugger
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_tool_main_functionality(self):
        \"\"\"Test main functionality through tool interface.\"\"\"
        debugger = await self.setup_tool()
        
        results = await debugger.run_tool_test(
            tool_name='your_tool_method',
            tool_params={'param1': 'test_value'}
        )
        
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith('Error:')
```
# NOTE
- Always prefer `yaml.dump({"rows": results, "count": len(results)}, default_flow_style=False, sort_keys=False, allow_unicode=True)` yaml dump over json.dumps for returning data to the tool.  Yaml uses less tokens than json.
---

## Migration Examples

### **Example 1: Simple API Tool**

#### **Before: Mixed Architecture**

```python
# ❌ BEFORE: All logic mixed in tool class
class WeatherTools(Toolset):
    async def get_current_weather(self, **kwargs) -> str:
        location_name = kwargs.get('location_name')
        
        # Business logic mixed in tool
        weather = await Weather(locale='en', unit='imperial').get_forecast(location_name)
        
        forecasts = []
        for forecast in weather.daily_forecasts:
            forecasts.append({
                'date': forecast.date.strftime('%Y-%m-%d'),
                'high_temperature': forecast.highest_temperature,
                'low_temperature': forecast.lowest_temperature
            })

        try:
            results = json.dumps({
                \"currently\": {
                    \"current_temperature\": weather.temperature,
                    \"sky\": weather.kind.emoji,
                    \"feels_like\": weather.feels_like,
                    \"humidity\": weather.humidity,
                    \"wind_speed\": weather.wind_speed,
                    \"wind_direction\": (weather.wind_direction.value + weather.wind_direction.emoji),
                    \"visibility\": weather.visibility,
                    \"uv_index\": weather.ultraviolet.index,
                    \"description\": weather.description,
                    \"forecasts\": forecasts
                }
            })
        except Exception as e:
            self.logger.error(f\"Error getting weather: {str(e)}\")
            return f\"Error getting weather: {str(e)}\"

        return results
```

#### **After: Clean Architecture**

```python
# ✅ AFTER: Business logic in Weather class
# util/client.py
class Weather:
    # ... existing methods ...
    
    async def get_formatted_weather_data(self, location: str) -> str:
        \"\"\"Get weather data formatted for tool consumption.\"\"\"
        try:
            weather = await self.get_forecast(location)
            
            if isinstance(weather, str):  # Error case
                return f\"Error getting weather: {weather}\"
            
            # Data processing logic
            forecasts = []
            for forecast in weather.daily_forecasts:
                forecasts.append({
                    'date': forecast.date.strftime('%Y-%m-%d'),
                    'high_temperature': forecast.highest_temperature,
                    'low_temperature': forecast.lowest_temperature
                })
            
            # Build response
            weather_data = {
                \"currently\": {
                    \"current_temperature\": weather.temperature,
                    \"sky\": weather.kind.emoji,
                    \"feels_like\": weather.feels_like,
                    \"humidity\": weather.humidity,
                    \"wind_speed\": weather.wind_speed,
                    \"wind_direction\": (weather.wind_direction.value + weather.wind_direction.emoji),
                    \"visibility\": weather.visibility,
                    \"uv_index\": weather.ultraviolet.index,
                    \"description\": weather.description,
                    \"forecasts\": forecasts
                }
            }
            
            return json.dumps(weather_data)
            
        except Exception as e:
            error_msg = f\"Error getting weather: {str(e)}\"
            self.logger.error(error_msg)
            return error_msg

# tool.py - Simplified
class WeatherTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='weather', use_prefix=False)
        self.logger = logging.getLogger(__name__)

    @json_schema(
        'Call this to get the weather forecast for a location',
        {
            'location_name': {
                'type': 'string',
                'description': 'The location to get the weather for.',
                'required': True
            }
        }
    )
    async def get_current_weather(self, **kwargs) -> str:
        location_name = kwargs.get('location_name')
        
        # Simple delegation
        weather_client = Weather(locale='en', unit='imperial')
        return await weather_client.get_formatted_weather_data(location_name)
```

### **Example 2: Database Tool**

#### **Before: Complex Mixed Logic**

```python
# ❌ BEFORE: Database logic mixed in tool
class DatabaseTools(Toolset):
    async def query_data(self, **kwargs) -> str:
        query = kwargs.get('query')
        
        # Database connection logic in tool
        try:
            conn = psycopg2.connect(database_url)
            cursor = conn.cursor()
            cursor.execute(query)
            results = cursor.fetchall()
            
            # Data processing in tool
            formatted_results = []
            for row in results:
                formatted_results.append({
                    'id': row[0],
                    'name': row[1],
                    'created_at': row[2].isoformat() if row[2] else None
                })
            
            return json.dumps({
                'results': formatted_results,
                'count': len(formatted_results)
            })
            
        except Exception as e:
            return f\"Database error: {str(e)}\"
        finally:
            if conn:
                conn.close()
```

#### **After: Clean Architecture**

```python
# ✅ AFTER: Business logic in database client
# database_client.py
class DatabaseClient:
    def __init__(self, connection_string: str):
        self.connection_string = connection_string
        self.logger = logging.getLogger(__name__)
    
    async def execute_query_for_tool(self, query: str) -> str:
        \"\"\"Execute query and return tool-friendly format.\"\"\"
        try:
            conn = psycopg2.connect(self.connection_string)
            cursor = conn.cursor()
            cursor.execute(query)
            results = cursor.fetchall()
            
            # Data processing
            formatted_results = []
            for row in results:
                formatted_results.append({
                    'id': row[0],
                    'name': row[1], 
                    'created_at': row[2].isoformat() if row[2] else None
                })
            
            return json.dumps({
                'results': formatted_results,
                'count': len(formatted_results)
            })
            
        except Exception as e:
            error_msg = f\"Database error: {str(e)}\"
            self.logger.error(error_msg)
            return error_msg
        finally:
            if conn:
                conn.close()

# tool.py - Simplified  
class DatabaseTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='database', use_prefix=False)
        self.db_client = DatabaseClient(os.getenv('DATABASE_URL'))

    @json_schema('Execute database query', {...})
    async def query_data(self, **kwargs) -> str:
        query = kwargs.get('query')
        
        # Simple delegation
        return await self.db_client.execute_query_for_tool(query)
```

---

## Testing Implementation

### **Component Test Pattern**

```python
# Always test the business logic directly
class TestBusinessLogicComponent:
    def setup_method(self):
        self.client = BusinessLogicClass(test_config)
    
    @pytest.mark.component
    @pytest.mark.asyncio
    async def test_success_case(self):
        result = await self.client.method_for_tool('valid_input')
        
        # Validate result format
        assert not result.startswith('Error:')
        data = json.loads(result)
        assert 'expected_field' in data
    
    @pytest.mark.component
    @pytest.mark.asyncio
    async def test_error_case(self):
        result = await self.client.method_for_tool('invalid_input')
        assert result.startswith('Error:')
```

### **Integration Test Pattern**

```python
# Always test through the tool interface
class TestToolIntegration:
    async def setup_tool(self):
        self.debugger = ToolDebugger(init_local_workspaces=False)
        await self.debugger.setup_tool('agent_c_tools.YourTool', {})
        return self.debugger
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_tool_interface(self):
        debugger = await self.setup_tool()
        
        results = await debugger.run_tool_test(
            'tool_method',
            {'param': 'value'}
        )
        
        content = debugger.extract_content_from_results(results)
        assert content is not None
```

---

## Best Practices

### **During Migration** ✅

1. **Work incrementally** - Migrate one method at a time
2. **Keep existing tests passing** - Don't break existing functionality
3. **Test frequently** - Run tests after each change
4. **Backup original code** - Keep `.backup` files until migration is complete
5. **Document changes** - Note what was moved where

### **Code Organization** ✅

1. **Follow existing patterns** - Look at how utility classes are organized
2. **Use descriptive names** - `get_data_for_tool()` is better than `process()`
3. **Keep methods focused** - One responsibility per method
4. **Handle errors consistently** - Return user-friendly error messages
5. **Add proper logging** - Log errors for debugging

### **Testing Strategy** ✅

1. **Test business logic thoroughly** - Component tests with real API calls
2. **Test tool interface minimally** - Integration tests validate delegation
3. **Mock sparingly** - Only mock when absolutely necessary
4. **Test error scenarios** - Invalid inputs, network failures
5. **Keep tests maintainable** - Clear, focused test methods

---

## Common Pitfalls

### **❌ Over-Engineering**

```python
# DON'T create unnecessary abstraction layers
class UnnecessaryService:
    def __init__(self, existing_client):
        self.client = existing_client
    
    def process(self, data):
        # Just calling existing method - adds no value
        return self.client.existing_method(data)

# DO extend existing classes naturally
class ExistingClient:
    def existing_method(self, data):
        # ... existing functionality
        
    def new_method_for_tool(self, data) -> str:
        # New tool-specific functionality
        result = self.existing_method(data)
        return json.dumps(result)
```

### **❌ Incomplete Migration**

```python
# DON'T leave business logic in tool class
class PartiallyMigratedTool(Toolset):
    async def tool_method(self, **kwargs) -> str:
        param = kwargs.get('param')
        
        # ❌ Still has business logic
        if param == 'special_case':
            return handle_special_case(param)
        
        # ✅ But delegates for normal cases
        return await business_logic(param)
```

### **❌ Poor Error Handling**

```python
# DON'T just pass exceptions through
async def bad_business_method(self, param):
    # ❌ No error handling - exceptions bubble up to tool
    data = await external_api_call(param)
    return json.dumps(data)

# DO handle errors at business logic level
async def good_business_method(self, param) -> str:
    try:
        data = await external_api_call(param)
        return json.dumps(data)
    except ExternalAPIError as e:
        self.logger.error(f\"API error: {e}\")
        return f\"Error: Unable to fetch data from external service\"
    except Exception as e:
        self.logger.error(f\"Unexpected error: {e}\")
        return f\"Error: An unexpected error occurred\"
```

### **❌ Breaking Tool Interface**

```python
# DON'T change the tool method signature
# BEFORE
async def tool_method(self, **kwargs) -> str:
    return process_data(kwargs)

# ❌ DON'T change to this (breaks existing users)
async def tool_method(self, param1: str, param2: str) -> dict:
    return {'result': process_data(param1, param2)}

# ✅ DO keep the same interface
async def tool_method(self, **kwargs) -> str:
    param1 = kwargs.get('param1')
    param2 = kwargs.get('param2')
    return await business_logic(param1, param2)
```

---

## Validation Checklist

### **Code Quality** ✅

- [ ] Tool class is ≤ 30 lines
- [ ] Tool class contains no business logic
- [ ] Business logic is in appropriate location (existing class or service)
- [ ] Error handling returns user-friendly messages
- [ ] Tool interface unchanged (no breaking changes)

### **Testing** ✅

- [ ] Component tests test business logic directly
- [ ] Integration tests test tool interface via debug_tool
- [ ] Both test types make real API calls (no mocking)
- [ ] Error scenarios are tested
- [ ] Tests run cleanly with no warnings

### **Documentation** ✅

- [ ] Business logic methods have clear docstrings
- [ ] Test files have descriptive class and method names
- [ ] README.md explains testing approach
- [ ] Migration changes are documented

### **File Organization** ✅

- [ ] Tests are in dedicated `tests/` directory
- [ ] Test configuration files are properly set up
- [ ] Test runner script works correctly
- [ ] All files are properly organized and named

### **Functionality** ✅

- [ ] Tool works exactly as before migration
- [ ] All existing functionality preserved
- [ ] Error messages are helpful to users
- [ ] Performance is acceptable
- [ ] No regressions introduced

---

## Migration Workflow Summary

```bash
# 1. Assessment Phase
cd path/to/your/tool
python -c \"import analyze_tool; analyze_tool.assess('tool.py')\"

# 2. Create backup and test structure
cp tool.py tool.py.backup
mkdir tests && cd tests
touch test_your_component.py test_your_integration.py
touch pytest.ini run_tests.py test-requirements.txt README.md

# 3. Extract business logic
# Edit util/existing_class.py or create new service file
# Add method_for_tool() methods

# 4. Simplify tool class
# Edit tool.py to remove business logic
# Keep only interface concerns

# 5. Implement tests
cd tests
pip install -r test-requirements.txt
python run_tests.py

# 6. Validate migration
python run_tests.py component      # Test business logic
python run_tests.py integration   # Test tool interface
python run_tests.py all           # Test everything

# 7. Clean up
rm tool.py.backup  # After confirming everything works
```

## Example Migration Candidates

Based on complexity, suggested migration order:

### **Easy Migrations** (Start Here)
- `random_number` - Simple logic, minimal dependencies
- `user_bio` - Basic data operations
- `user_preferences` - Configuration management

### **Medium Migrations**
- `web_search` - External API calls, data formatting
- `workspace` - File operations, multiple methods
- `rss` - XML processing, data transformation

### **Complex Migrations** (Advanced)
- `mariadb` - Database operations, connection management
- `dynamics` - Complex business logic, multiple dependencies
- `flash_docs` - Document processing, multiple formats

Start with an **easy migration** to learn the process, then apply the same pattern to more complex tools!

This migration guide provides everything you need to successfully refactor your existing tools to the clean architecture pattern. **Follow the steps, use the examples, and you'll have maintainable, testable tools that follow industry best practices!** 🚀
