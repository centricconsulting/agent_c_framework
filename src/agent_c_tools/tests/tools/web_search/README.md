# Web Search System Unit Tests

This directory contains comprehensive unit tests for the unified web search system.

## Test Structure

```
tests/tools/web_search/
├── conftest.py                     # Pytest configuration and shared fixtures
├── pytest.ini                     # Pytest settings and configuration
├── README.md                       # This file
├── test_models.py                  # Tests for data models and enums
├── test_parameter_validator.py     # Tests for parameter validation
├── test_engine_router.py          # Tests for engine routing logic
├── test_engine_adapters.py        # Tests for engine adapter implementations
├── test_response_standardizer.py  # Tests for response standardization
├── test_error_handler.py          # Tests for error handling
├── test_registry.py               # Tests for engine registry
└── test_web_search_tools.py       # Tests for main WebSearchTools class
```

## Test Categories

### Unit Tests
- **Models** (`test_models.py`): Data model validation, serialization, and constraints
- **Parameter Validation** (`test_parameter_validator.py`): Input validation and normalization
- **Engine Routing** (`test_engine_router.py`): Query analysis and engine selection
- **Engine Adapters** (`test_engine_adapters.py`): Individual engine implementations
- **Response Standardization** (`test_response_standardizer.py`): Response format unification
- **Error Handling** (`test_error_handler.py`): Error categorization and recovery
- **Registry** (`test_registry.py`): Engine registration and management
- **WebSearchTools** (`test_web_search_tools.py`): Main toolset interface

### Test Markers

Tests are marked with the following categories:

- `@pytest.mark.unit`: Core unit tests
- `@pytest.mark.integration`: Integration tests
- `@pytest.mark.engine`: Engine-specific tests
- `@pytest.mark.validation`: Parameter validation tests
- `@pytest.mark.routing`: Engine routing tests
- `@pytest.mark.error_handling`: Error handling tests
- `@pytest.mark.performance`: Performance tests
- `@pytest.mark.slow`: Long-running tests
- `@pytest.mark.requires_api_key`: Tests requiring API keys
- `@pytest.mark.network`: Tests requiring network access

## Running Tests

### Run All Tests
```bash
cd /project/src/agent_c_tools/tests/tools/web_search
pytest
```

### Run Specific Test Categories
```bash
# Run only unit tests
pytest -m unit

# Run only validation tests
pytest -m validation

# Run only engine tests
pytest -m engine

# Skip slow tests
pytest -m "not slow"

# Skip tests requiring API keys
pytest -m "not requires_api_key"
```

### Run Specific Test Files
```bash
# Test parameter validation
pytest test_parameter_validator.py

# Test engine routing
pytest test_engine_router.py

# Test specific engine adapters
pytest test_engine_adapters.py::TestDuckDuckGoEngine
```

### Run with Coverage
```bash
pytest --cov=base --cov=engines --cov=web_search_tools --cov-report=html
```

### Run in Parallel
```bash
pytest -n auto  # Requires pytest-xdist
```

## Test Configuration

### Environment Variables
Tests can be configured with environment variables:

- `TEST_MODE=true`: Enables test mode
- `LOG_LEVEL=WARNING`: Sets logging level for tests
- `PYTHONPATH`: Set to include web search modules

### API Keys for Integration Tests
Some tests require API keys to run against real services:

- `SERPAPI_API_KEY`: For Google SERP engine tests
- `NEWSAPI_API_KEY`: For NewsAPI engine tests
- `TAVILI_API_KEY`: For Tavily engine tests

Set these environment variables to run the full test suite:
```bash
export SERPAPI_API_KEY="your_serpapi_key"
export NEWSAPI_API_KEY="your_newsapi_key"
export TAVILI_API_KEY="your_tavily_key"
pytest
```

## Test Fixtures

### Shared Fixtures (conftest.py)
- `mock_search_parameters`: Mock SearchParameters object
- `mock_search_result`: Mock SearchResult object
- `mock_search_response`: Mock SearchResponse object
- `mock_web_search_config`: Mock WebSearchConfig object
- `mock_engine_capabilities`: Mock EngineCapabilities object
- `mock_engine_health_status`: Mock EngineHealthStatus object
- `mock_base_engine`: Mock BaseWebSearchEngine object
- `mock_engine_registry`: Mock EngineRegistry object
- `mock_query_analyzer`: Mock QueryAnalyzer object
- `mock_engine_router`: Mock EngineRouter object
- `mock_parameter_validator`: Mock ParameterValidator object
- `mock_response_standardizer`: Mock ResponseStandardizer object
- `mock_error_handler`: Mock ErrorHandler object
- `sample_raw_search_results`: Sample raw search results data
- `sample_engine_configs`: Sample engine configurations
- `mock_legacy_tools`: Mock legacy tool implementations
- `mock_api_responses`: Mock API responses for different engines

## Test Coverage Goals

The test suite aims for:
- **90%+ code coverage** across all modules
- **100% coverage** of critical paths (error handling, validation)
- **Edge case coverage** for all public APIs
- **Integration coverage** between components
- **Performance regression detection**

## Writing New Tests

### Test Naming Convention
- Test files: `test_<module_name>.py`
- Test classes: `Test<ClassName>`
- Test methods: `test_<functionality_being_tested>`

### Test Structure
```python
class TestClassName:
    """Test suite for ClassName."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        # Initialize test objects
        pass
    
    def test_basic_functionality(self):
        """Test basic functionality with valid inputs."""
        # Arrange
        # Act
        # Assert
        pass
    
    def test_error_conditions(self):
        """Test error handling with invalid inputs."""
        # Test error conditions
        pass
    
    def test_edge_cases(self):
        """Test edge cases and boundary conditions."""
        # Test edge cases
        pass
```

### Mock Usage Guidelines
- Use `unittest.mock.Mock` for external dependencies
- Use `@patch` decorator for patching imports
- Create realistic mock responses that match actual API formats
- Use fixtures for commonly used mock objects

### Assertion Guidelines
- Use specific assertions (`assert x == y` vs `assert x`)
- Test both positive and negative cases
- Verify side effects and state changes
- Check error messages and exception types

## Debugging Tests

### Running Individual Tests
```bash
# Run single test with verbose output
pytest test_parameter_validator.py::TestParameterValidator::test_validate_query_parameter -v

# Run with pdb debugger
pytest --pdb test_file.py::test_function

# Run with print statements visible
pytest -s test_file.py
```

### Common Issues
1. **Import Errors**: Ensure PYTHONPATH includes web search modules
2. **Mock Issues**: Verify mock objects match expected interfaces
3. **Async Tests**: Use `@pytest.mark.asyncio` for async test functions
4. **API Key Tests**: Set required environment variables

## Continuous Integration

Tests are designed to run in CI environments:
- No external dependencies required for core tests
- API key tests are marked and can be skipped
- Network tests are isolated and marked
- Fast execution time for most tests
- Comprehensive error reporting

## Performance Testing

Performance tests are included but marked as `slow`:
```bash
# Run performance tests
pytest -m performance

# Skip performance tests in normal runs
pytest -m "not performance"
```

Performance tests cover:
- Engine response times
- Concurrent request handling
- Memory usage patterns
- Cache effectiveness
- Routing decision speed