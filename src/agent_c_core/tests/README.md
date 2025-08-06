# Intelligent Message Management System - Test Suite

This directory contains comprehensive tests for the Intelligent Message Management System, including unit tests, integration tests, performance benchmarks, and validation utilities.

## Test Structure

```
tests/
├── unit/                           # Unit tests for individual components
│   ├── agents/                     # Agent-related unit tests
│   ├── models/                     # Model unit tests
│   │   ├── chat_history/          # Chat history model tests
│   │   └── common_chat/           # Common chat model tests
│   ├── toolsets/                  # Toolset unit tests
│   └── util/                      # Utility unit tests
├── integration/                   # Integration tests
│   ├── test_comprehensive_system_integration.py
│   ├── test_context_integration.py
│   ├── test_structured_logging_integration.py
│   └── test_testing_infrastructure_integration.py
├── pytest.ini                    # Pytest configuration
├── run_comprehensive_tests.py    # Comprehensive test runner
└── README.md                     # This file
```

## Test Categories

### Unit Tests (`tests/unit/`)
- **InteractionContainer**: Core container functionality, message management, tool manipulation
- **EnhancedChatSession**: Session management, interaction tracking, migration
- **AgentWorkLog**: Work log generation, parameter extraction, audit capabilities
- **MessageManager**: Cross-interaction message management and queries
- **ToolManipulationAPI**: Tool optimization, invalidation strategies, conflict resolution
- **Enhanced Models**: Message models, content blocks, validation
- **Enhanced Converters**: Provider translation, work log context preservation

### Integration Tests (`tests/integration/`)
- **Comprehensive System Integration**: End-to-end workflows with all components
- **Context Integration**: ChatSession integration with backward compatibility
- **Structured Logging Integration**: Logging integration across all components
- **Testing Infrastructure**: Performance benchmarks, stress tests, migration validation

### Performance Tests
- **Large Session Handling**: 10,000+ message sessions
- **Tool Manipulation Overhead**: Concurrent tool optimization
- **Work Log Generation**: High-volume work log processing
- **Translation Performance**: Provider translation benchmarks
- **Memory Usage Optimization**: Memory efficiency validation
- **Concurrent Access**: Thread safety and performance under load

### Stress Tests
- **Memory Stress**: Multiple large sessions (50,000+ messages)
- **Tool Manipulation Stress**: High-volume tool operations
- **Concurrent Stress**: 50+ concurrent threads with intensive operations

### Migration Tests
- **Various Session Formats**: Simple, complex, large, and corrupted sessions
- **Data Integrity**: Validation of message preservation
- **Performance**: Large-scale migration benchmarks
- **Rollback Scenarios**: Migration failure recovery

## Running Tests

### Quick Test Run (Recommended for Development)
```bash
# Run unit and integration tests only
python run_comprehensive_tests.py --quick

# Or using pytest directly
pytest tests/unit/ tests/integration/ -v --tb=short
```

### Comprehensive Test Run
```bash
# Run all test categories
python run_comprehensive_tests.py

# Run specific categories
python run_comprehensive_tests.py --categories unit integration performance

# Verbose output
python run_comprehensive_tests.py --verbose
```

### Individual Test Categories
```bash
# Unit tests only
python run_comprehensive_tests.py --categories unit

# Performance tests only
python run_comprehensive_tests.py --categories performance

# Stress tests only
python run_comprehensive_tests.py --categories stress
```

### Using Pytest Directly
```bash
# All tests
pytest

# Unit tests with coverage
pytest tests/unit/ --cov=src/agent_c --cov-report=html

# Integration tests
pytest tests/integration/ -v

# Performance tests
pytest -m performance -s

# Specific test file
pytest tests/unit/models/chat_history/test_interaction_container.py -v
```

## Test Markers

Tests are categorized using pytest markers:

- `@pytest.mark.unit`: Unit tests
- `@pytest.mark.integration`: Integration tests
- `@pytest.mark.performance`: Performance benchmarks
- `@pytest.mark.stress`: Stress tests
- `@pytest.mark.slow`: Tests taking >1 second
- `@pytest.mark.concurrent`: Concurrent operation tests
- `@pytest.mark.migration`: Migration tests

## Performance Requirements

The test suite validates the following performance requirements:

### Message Operations
- **Large session creation**: 10,000 messages in <30 seconds
- **Message retrieval**: 10,000 messages in <2 seconds
- **Message search**: Search across 10,000 messages in <3 seconds
- **Tool filtering**: Filter by tool in <1 second

### Work Log Operations
- **Work log generation**: 2,500+ entries in <10 seconds
- **Work log insertion**: 2,500+ entries in <5 seconds
- **Work log queries**: Complex queries in <2 seconds

### Translation Operations
- **Provider translation**: 1,000 messages in <5 seconds
- **Round-trip translation**: 1,000 messages in <5 seconds

### Concurrent Operations
- **20 concurrent threads**: Complete in <60 seconds
- **50 concurrent threads**: Complete in <300 seconds

### Migration Operations
- **Large-scale migration**: 10,000 messages in <60 seconds

## Coverage Requirements

- **Minimum coverage**: 85%
- **Unit test coverage**: 95%+
- **Integration test coverage**: 80%+

Coverage reports are generated in:
- `htmlcov/`: HTML coverage reports
- `coverage.xml`: XML coverage report for CI/CD

## Test Data and Fixtures

### Shared Fixtures
- `enhanced_session`: EnhancedChatSession with sample data
- `interaction_container`: InteractionContainer with test messages
- `tool_chest`: ToolChest with mock tools
- `work_log`: AgentWorkLog with sample entries

### Test Data Generators
- `StressTestGenerator.generate_large_session()`: Large sessions for stress testing
- `StressTestGenerator.generate_complex_tool_scenario()`: Complex tool usage scenarios
- `StressTestGenerator.generate_migration_test_data()`: Various migration scenarios

## Debugging Failed Tests

### Common Issues
1. **Import errors**: Ensure all dependencies are installed
2. **Timeout errors**: Increase timeout for performance tests
3. **Concurrency issues**: Check thread safety implementations
4. **Memory issues**: Monitor memory usage during stress tests

### Debugging Commands
```bash
# Run with detailed output
pytest tests/path/to/test.py -v -s --tb=long

# Run single test method
pytest tests/path/to/test.py::TestClass::test_method -v

# Debug with pdb
pytest tests/path/to/test.py --pdb

# Show local variables on failure
pytest tests/path/to/test.py --tb=long -vvv
```

## Continuous Integration

The test suite is designed for CI/CD integration:

### Required Environment
- Python 3.8+
- All project dependencies installed
- Sufficient memory for stress tests (4GB+ recommended)
- Timeout allowance for comprehensive tests (30+ minutes)

### CI Configuration Example
```yaml
# Example GitHub Actions configuration
- name: Run comprehensive tests
  run: |
    python run_comprehensive_tests.py --categories unit integration validation
    
- name: Run performance tests
  run: |
    python run_comprehensive_tests.py --categories performance
  timeout-minutes: 30

- name: Upload coverage
  uses: codecov/codecov-action@v1
  with:
    file: ./coverage.xml
```

## Contributing

When adding new tests:

1. **Follow naming conventions**: `test_*.py` files, `test_*` methods
2. **Use appropriate markers**: Mark tests with category markers
3. **Add docstrings**: Document test purpose and requirements
4. **Include assertions**: Verify both positive and negative cases
5. **Consider performance**: Add performance assertions for critical paths
6. **Test edge cases**: Include boundary conditions and error scenarios

### Test Template
```python
import pytest
from agent_c.models.your_module import YourClass

class TestYourClass:
    """Test suite for YourClass functionality."""
    
    @pytest.fixture
    def your_instance(self):
        """Create YourClass instance for testing."""
        return YourClass(param="test")
    
    @pytest.mark.unit
    def test_basic_functionality(self, your_instance):
        """Test basic functionality works correctly."""
        result = your_instance.method()
        assert result == expected_value
    
    @pytest.mark.unit
    def test_error_handling(self, your_instance):
        """Test error handling for invalid inputs."""
        with pytest.raises(ValueError):
            your_instance.method(invalid_param)
    
    @pytest.mark.performance
    def test_performance_requirements(self, your_instance):
        """Test performance meets requirements."""
        import time
        start_time = time.time()
        
        # Perform operation
        result = your_instance.expensive_operation()
        
        duration = time.time() - start_time
        assert duration < 1.0  # Should complete in under 1 second
        assert result is not None
```

## Support

For test-related issues:

1. Check test output and error messages
2. Review test documentation and requirements
3. Run individual test categories to isolate issues
4. Use verbose mode for detailed debugging information
5. Check performance requirements and system resources

The comprehensive test suite ensures the Intelligent Message Management System meets all functional, performance, and reliability requirements while maintaining backward compatibility and providing extensive validation coverage.