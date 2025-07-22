# Web Search Integration Tests

This directory contains integration tests for the unified web search system that test with real APIs and network connections.

## Test Structure

```
integration/
├── conftest.py                    # Pytest configuration and fixtures
├── pytest.ini                    # Pytest settings for integration tests
├── README.md                      # This file
├── test_real_api_engines.py       # Tests for individual engines with real APIs
├── test_unified_interface.py      # Tests for unified WebSearchTools interface
├── test_api_key_management.py     # Tests for API key handling and configuration
└── test_rate_limiting.py          # Tests for rate limiting and performance
```

## Test Categories

### Real API Engine Tests (`test_real_api_engines.py`)
- **DuckDuckGo Integration**: Tests real web search, safe search, availability
- **Google SERP Integration**: Tests web, news, flights, events search with real API
- **Tavily Integration**: Tests research search with domain filtering
- **Wikipedia Integration**: Tests educational search with real Wikipedia API
- **NewsAPI Integration**: Tests news search with date filtering
- **HackerNews Integration**: Tests tech community search
- **Rate Limiting**: Tests handling of rapid requests and rate limits
- **Health Monitoring**: Tests engine health status and monitoring

### Unified Interface Tests (`test_unified_interface.py`)
- **Auto Routing**: Tests automatic engine selection based on query type
- **Explicit Engine Selection**: Tests specifying engines explicitly
- **Search Type Routing**: Tests routing for different search types (news, educational, etc.)
- **Parameter Handling**: Tests safe search, domain filtering, language, region
- **Error Handling**: Tests fallback behavior and error recovery
- **Performance**: Tests response times and concurrent requests

### API Key Management Tests (`test_api_key_management.py`)
- **Key Detection**: Tests detection of API keys from environment variables
- **Graceful Degradation**: Tests fallback when API keys are missing
- **Configuration Validation**: Tests handling of invalid API keys
- **Service Recovery**: Tests recovery when configuration is restored
- **Environment Variables**: Tests proper handling of environment variables

### Rate Limiting Tests (`test_rate_limiting.py`)
- **Sequential Requests**: Tests behavior with sequential API calls
- **Concurrent Requests**: Tests handling of concurrent requests
- **Rapid Requests**: Tests rate limiting with rapid-fire requests
- **Quota Handling**: Tests API quota awareness and exhaustion
- **Performance Under Load**: Tests sustained load and memory stability
- **Error Recovery**: Tests recovery from network timeouts and failures

## Running Integration Tests

### Prerequisites

1. **Network Connection**: All tests require internet connectivity
2. **API Keys** (optional but recommended):
   ```bash
   export SERPAPI_API_KEY="your_serpapi_key"
   export NEWSAPI_API_KEY="your_newsapi_key"
   export TAVILI_API_KEY="your_tavily_key"
   ```

### Basic Test Execution

```bash
# Run all integration tests
cd /project/src/agent_c_tools/tests/tools/web_search/integration
pytest

# Run specific test categories
pytest -m "integration and not requires_api_key"  # Tests without API keys
pytest -m "integration and network"               # All network tests
pytest -m "integration and not slow"              # Skip slow tests
```

### Test Filtering Options

```bash
# Run tests for specific engines
pytest test_real_api_engines.py::TestDuckDuckGoIntegration
pytest test_real_api_engines.py::TestGoogleSerpIntegration

# Run unified interface tests
pytest test_unified_interface.py

# Run API key management tests
pytest test_api_key_management.py

# Run rate limiting tests
pytest test_rate_limiting.py

# Skip tests requiring API keys
pytest -m "not requires_api_key"

# Run only fast tests
pytest -m "not slow"

# Run load tests
pytest -m "load"
```

### With API Keys

```bash
# Set API keys and run full test suite
export SERPAPI_API_KEY="your_key"
export NEWSAPI_API_KEY="your_key"
export TAVILI_API_KEY="your_key"
pytest

# Run only API-dependent tests
pytest -m "requires_api_key"
```

### Performance Testing

```bash
# Run performance and load tests
pytest -m "slow" -v

# Run with performance monitoring
pytest --durations=0 -v
```

## Test Markers

- `@pytest.mark.integration`: All integration tests
- `@pytest.mark.network`: Tests requiring network access
- `@pytest.mark.requires_api_key`: Tests requiring API keys
- `@pytest.mark.slow`: Long-running tests (>30 seconds)
- `@pytest.mark.load`: Load and performance tests

## Expected Behavior

### Without API Keys
- **DuckDuckGo, Wikipedia, HackerNews**: Should work normally
- **Google SERP, Tavily, NewsAPI**: Should fallback gracefully or skip
- **Unified Interface**: Should route to available engines

### With API Keys
- **All Engines**: Should work with real API calls
- **Rate Limiting**: May encounter rate limits with rapid requests
- **Quotas**: May hit daily/monthly quotas with extensive testing

### Performance Expectations
- **Response Times**: Most requests should complete within 10 seconds
- **Concurrent Requests**: Should handle 3-5 concurrent requests
- **Memory Usage**: Should remain stable under load
- **Error Recovery**: Should recover from temporary failures

## Troubleshooting

### Common Issues

1. **Import Errors**:
   ```bash
   # Ensure PYTHONPATH is set correctly
   export PYTHONPATH="../../../../src/agent_c_tools/src/agent_c_tools/tools/web_search"
   ```

2. **Network Timeouts**:
   ```bash
   # Increase timeout for slow networks
   pytest --timeout=600
   ```

3. **API Rate Limits**:
   ```bash
   # Run with delays between tests
   pytest --dist=no -v
   ```

4. **API Key Issues**:
   ```bash
   # Verify API keys are set
   echo $SERPAPI_API_KEY
   echo $NEWSAPI_API_KEY
   echo $TAVILI_API_KEY
   ```

### Debug Mode

```bash
# Run with verbose output and no capture
pytest -v -s

# Run single test with debugging
pytest test_real_api_engines.py::TestDuckDuckGoIntegration::test_real_web_search -v -s

# Show all print statements
pytest --capture=no
```

## Test Data and Cleanup

- **No Persistent Data**: Integration tests don't create persistent data
- **Rate Limiting**: Tests include delays to respect API rate limits
- **Cleanup**: Environment variables are restored after each test
- **Isolation**: Each test runs independently

## Continuous Integration

These tests are designed to run in CI environments:

```bash
# CI-friendly test run (skip slow tests, no API keys)
pytest -m "integration and not slow and not requires_api_key" --tb=short

# With API keys in CI
pytest -m "integration and not slow" --tb=short
```

## Contributing

When adding new integration tests:

1. **Use Appropriate Markers**: Mark tests with correct pytest markers
2. **Handle API Keys**: Use `@pytest.mark.skipif` for API key requirements
3. **Respect Rate Limits**: Add appropriate delays between requests
4. **Test Fallbacks**: Ensure tests work with and without API keys
5. **Performance Awareness**: Mark slow tests appropriately
6. **Error Handling**: Test both success and failure scenarios

## Security Notes

- **API Keys**: Never commit API keys to version control
- **Rate Limiting**: Respect API provider rate limits and terms of service
- **Usage Monitoring**: Monitor API usage to avoid unexpected charges
- **Test Data**: Use non-sensitive test queries