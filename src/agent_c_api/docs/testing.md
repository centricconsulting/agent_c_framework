# Testing Guide for Agent C API

This document provides an overview of the testing infrastructure and guidelines for the Agent C API project.

## Testing Structure

The Agent C API follows a structured testing approach with three main test categories:

1. **Unit Tests**: Focus on testing individual components in isolation
2. **Integration Tests**: Validate interactions between components
3. **Functional Tests**: Test complete features and workflows

## Directory Structure

```
//api/tests/
  ├── functional/           # Functional tests
  ├── integration/          # Integration tests
  │   ├── api/              # API integration tests
  │   └── core/             # Core integration tests
  ├── unit/                 # Unit tests
  │   ├── api/              # API unit tests
  │   │   ├── v1/           # API v1 tests
  │   │   └── v2/           # API v2 tests
  │   │       ├── config/   # Configuration tests
  │   │       ├── debug/    # Debug tests
  │   │       ├── history/  # History tests
  │   │       ├── models/   # Model tests
  │   │       ├── sessions/ # Session tests
  │   │       └── utils/    # Utility tests
  │   ├── config/           # Config unit tests
  │   └── core/             # Core unit tests
  ├── conftest.py           # Shared test fixtures
  └── pytest.ini            # PyTest configuration
```

## Test Categories

### Unit Tests

Unit tests are designed to test individual components in isolation. This includes:

- Model validation
- Function behavior
- Service methods
- Utility functions

### Integration Tests

Integration tests validate interactions between components, such as:

- Service-to-service communication
- Database interactions
- External API integrations

### Functional Tests

Functional tests focus on complete workflows and features, ensuring they behave as expected from an end-user perspective.

## Test Markers

The project uses pytest markers to categorize tests. This allows for selective test execution based on categories:

| Marker        | Description                                      |
|---------------|--------------------------------------------------|
| `unit`        | Marks a test as a unit test                      |
| `integration` | Marks a test as an integration test              |
| `functional`  | Marks a test as a functional test                |
| `slow`        | Marks a test as slow running                     |
| `api`         | Marks a test as testing API endpoints            |
| `core`        | Marks a test as testing core functionality       |
| `config`      | Marks a test as testing the api/v2/config        |
| `models`      | Marks a test as testing models                   |
| `endpoints`   | Marks a test as testing endpoints                |
| `services`    | Marks a test as testing service layer            |
| `chat`        | Marks a test as testing chat functionality       |

## Common Fixtures

The project provides several fixtures in `conftest.py` to assist with testing:

- `client`: FastAPI TestClient for API testing
- `mock_agent_manager`: Mock UItoAgentBridgeManager for testing
- `sample_session_detail`: Sample session detail data
- `sample_session_list`: Sample session list data
- `setup_test_cache`: Initializes and clears FastAPICache for testing

## Running Tests

### Running All Tests

```bash
python -m pytest
```

### Running Tests by Category

```bash
# Run unit tests only
python -m pytest -m unit

# Run integration tests only
python -m pytest -m integration

# Run functional tests only
python -m pytest -m functional
```

### Running Tests by Module

```bash
# Run all tests in the v2 config module
python -m pytest tests/unit/api/v2/config/

# Run all service tests
python -m pytest -m services
```

### Running Tests with Coverage

The project is configured to automatically generate coverage reports when running tests:

```bash
python -m pytest
```

This will:
1. Run all tests
2. Generate a terminal coverage report
3. Generate an HTML coverage report in `.coverage_report/`

## Writing Tests

### Test Structure Guidelines

1. **Arrange-Act-Assert**: Tests should follow this pattern
2. **Clear naming**: Test names should be descriptive and indicate what is being tested
3. **One assertion per test**: Ideally, each test should verify one thing
4. **Use fixtures**: Utilize fixtures for common setup and test data
5. **Proper mocking**: Use mocks for external dependencies

### Class vs Function-Based Tests

Both styles are supported:

```python
# Class-based test (good for grouping related tests)
class TestSessionService:
    def test_create_session(self, mock_agent_manager):
        # Test implementation
        
    def test_get_session(self, mock_agent_manager):
        # Test implementation

# Function-based test (simpler for standalone tests)
def test_session_model_validation():
    # Test implementation
```

### Mocking Dependencies

For mocking FastAPI dependencies:

```python
from agent_c_api.main import app

@pytest.fixture
def client(mock_config_service):
    app.dependency_overrides[get_config_service] = lambda: mock_config_service
    yield TestClient(app)
    app.dependency_overrides = {}
```

## Testing Best Practices

1. **Test in isolation**: Mock external dependencies
2. **Use parameterized tests**: For testing multiple inputs
3. **Test edge cases**: Include boundary conditions and error cases
4. **Keep tests fast**: Slow tests discourage frequent running
5. **Ensure independence**: Tests should not depend on other tests
6. **Clean up after tests**: Leave no side effects for subsequent tests

## CI/CD Integration

Tests are automatically run in the CI/CD pipeline. The pipeline will:

1. Run all tests
2. Generate coverage reports
3. Fail builds if coverage falls below thresholds
4. Fail builds if any tests fail

## Debugging Tests

To debug failing tests:

```bash
# Show detailed output
python -m pytest -v

# Show print statements
python -m pytest -v -s

# Run specific failing test
python -m pytest path/to/test.py::TestClass::test_method -v
```