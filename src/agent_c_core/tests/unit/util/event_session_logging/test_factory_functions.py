#!/usr/bin/env python3
"""
Factory Functions Test Suite - Pytest Version

This script tests the comprehensive factory functions and utilities for creating
EventSessionLogger instances with various configurations and patterns.
"""

import os
import sys
import tempfile
import warnings
from pathlib import Path
import pytest

# Add the src directory to Python path for imports
agent_c_core_dir = Path(__file__).parents[4]
sys.path.insert(0, str(agent_c_core_dir / 'src'))

from agent_c.util.event_logging.event_session_logger_factory import (
    LoggerConfiguration, LoggerEnvironment, TransportType,
    load_configuration_from_env, create_development_logger, create_testing_logger, create_production_logger,
    create_migration_logger, create_monitoring_logger, create_multi_transport_logger,
    validate_logger_config, print_logger_info, create_logger_with_validation,
    create_session_logger, create_logging_only, create_with_callback, create_with_transport,
    create_backward_compatible
)
from agent_c.util.transports import LoggingTransport, NullTransport, FileTransport
from agent_c.util.transport_exceptions import EventSessionLoggerError
from agent_c.models.events.chat import MessageEvent


# Pytest fixtures
@pytest.fixture
def temp_dir():
    """Provide a temporary directory for tests"""
    with tempfile.TemporaryDirectory() as temp_dir:
        yield temp_dir


@pytest.fixture
def mock_session_logger():
    """Mock old SessionLogger for migration testing"""

    class MockSessionLogger:
        def __init__(self, log_file_path: str, include_system_prompt: bool = True):
            self.log_file_path = Path(log_file_path)
            self.include_system_prompt = include_system_prompt

    return MockSessionLogger


@pytest.fixture
def sample_message_event():
    """Provide a sample MessageEvent for testing"""
    return MessageEvent(
        session_id="test_session",
        role="assistant",
        content="Test message"
    )


# Test functions with pytest decorators
@pytest.mark.asyncio
async def test_logger_configuration():
    """Test LoggerConfiguration class"""
    # Test default configuration
    config = LoggerConfiguration()
    assert config.log_base_dir == "logs/sessions", "Should have default log directory"
    assert config.environment == LoggerEnvironment.DEVELOPMENT, "Should default to development"
    assert config.include_system_prompt == True, "Should include system prompts by default"

    # Test validation
    errors = config.validate()
    assert len(errors) == 0, "Default configuration should be valid"

    # Test invalid configuration
    invalid_config = LoggerConfiguration(
        max_retry_attempts=-1,
        retry_delay_seconds=-1.0,
        transport_type=TransportType.HTTP,
        transport_config={}  # Missing required endpoint_url
    )

    errors = invalid_config.validate()
    assert len(errors) > 0, "Invalid configuration should have errors"
    assert any("max_retry_attempts" in error for error in errors), "Should validate retry attempts"
    assert any("retry_delay_seconds" in error for error in errors), "Should validate retry delay"
    assert any("endpoint_url" in error for error in errors), "Should validate HTTP config"


@pytest.mark.asyncio
async def test_environment_configuration():
    """Test environment-based configuration loading"""
    # Save original environment
    original_env = {}
    test_env = {
        'AGENT_LOG_DIR': '/tmp/test_env_logs',
        'AGENT_TRANSPORT_TYPE': 'logging',
        'AGENT_ENVIRONMENT': 'production',
        'AGENT_DEBUG': 'true',
        'AGENT_LOG_MAX_RETRIES': '5'
    }

    for key, value in test_env.items():
        original_env[key] = os.environ.get(key)
        os.environ[key] = value

    try:
        config = load_configuration_from_env()

        assert config.log_base_dir == '/tmp/test_env_logs', "Should load log directory from env"
        assert config.transport_type == TransportType.LOGGING, "Should load transport type from env"
        assert config.environment == LoggerEnvironment.PRODUCTION, "Should load environment from env"
        assert config.debug_mode == True, "Should load debug mode from env"
        assert config.max_retry_attempts == 5, "Should load retry attempts from env"

    finally:
        # Restore original environment
        for key, original_value in original_env.items():
            if original_value is None:
                os.environ.pop(key, None)
            else:
                os.environ[key] = original_value


@pytest.mark.asyncio
async def test_basic_factory_functions(temp_dir):
    """Test basic factory functions"""
    # Test create_session_logger
    logger1 = create_session_logger(log_base_dir=temp_dir)
    assert str(logger1.log_base_dir) == temp_dir, "Should set log directory"
    assert logger1.downstream_callback is None, "Should have no callback by default"
    await logger1.close()

    # Test create_logging_only
    logger2 = create_logging_only(log_base_dir=temp_dir)
    assert logger2.downstream_transport is None, "Should have no transport"
    await logger2.close()

    # Test create_with_callback
    test_events = []

    async def test_callback(event):
        test_events.append(event)

    logger3 = create_with_callback(test_callback, log_base_dir=temp_dir)
    assert logger3.downstream_callback == test_callback, "Should have callback set"
    await logger3.close()

    # Test create_with_transport
    transport = NullTransport()
    logger4 = create_with_transport(transport, log_base_dir=temp_dir)
    assert logger4.downstream_transport == transport, "Should have transport set"
    await logger4.close()

    # Test create_backward_compatible
    logger5 = create_backward_compatible(log_base_dir=temp_dir)
    assert logger5.log_format == "jsonl", "Should use JSON Lines format"
    assert logger5.include_system_prompt == True, "Should include system prompts"
    await logger5.close()


@pytest.mark.asyncio
async def test_environment_specific_loggers(temp_dir):
    """Test environment-specific logger creation"""
    # Test development logger with default path
    dev_logger = create_development_logger()
    assert "dev" in str(dev_logger.log_base_dir), "Should use dev directory"
    assert dev_logger.downstream_transport is not None, "Should have transport for development"
    await dev_logger.close()

    # Test development logger with custom path
    dev_logger_custom = create_development_logger(log_base_dir=temp_dir)
    assert str(dev_logger_custom.log_base_dir) == temp_dir, "Should use custom directory"
    await dev_logger_custom.close()

    # Test testing logger with default path
    test_logger = create_testing_logger()
    assert "test" in str(test_logger.log_base_dir), "Should use test directory"
    assert isinstance(test_logger.downstream_transport, NullTransport), "Should use null transport for testing"
    await test_logger.close()

    # Test testing logger with custom path
    test_logger_custom = create_testing_logger(log_base_dir=temp_dir)
    assert str(test_logger_custom.log_base_dir) == temp_dir, "Should use custom directory"
    await test_logger_custom.close()

    # Test production logger
    prod_config = {
        'queue_name': 'prod_events',
        'connection_string': 'amqp://prod-queue'
    }
    prod_logger = create_production_logger(
        temp_dir,
        TransportType.QUEUE,
        prod_config
    )
    assert str(prod_logger.log_base_dir) == temp_dir, "Should use specified directory"
    assert prod_logger.downstream_transport is not None, "Should have transport"
    await prod_logger.close()


@pytest.mark.asyncio
async def test_migration_logger(temp_dir, mock_session_logger):
    """Test migration logger functionality"""
    # Create mock old SessionLogger
    old_logger = mock_session_logger(
        log_file_path=f"{temp_dir}/session/test.jsonl",
        include_system_prompt=False
    )

    # Test migration without warnings
    with warnings.catch_warnings(record=True) as w:
        warnings.simplefilter("always")

        migration_logger = create_migration_logger(
            old_logger,
            enable_warnings=False
        )

        # Should not generate warnings
        assert len(w) == 0, "Should not generate warnings when disabled"
        assert migration_logger.include_system_prompt == False, "Should extract config from old logger"
        await migration_logger.close()

    # Test migration with warnings
    with warnings.catch_warnings(record=True) as w:
        warnings.simplefilter("always")

        migration_logger = create_migration_logger(
            old_logger,
            enable_warnings=True
        )

        # Should generate deprecation warning
        assert len(w) > 0, "Should generate deprecation warning"
        assert issubclass(w[0].category, DeprecationWarning), "Should be deprecation warning"
        await migration_logger.close()


@pytest.mark.asyncio
async def test_monitoring_logger(temp_dir, sample_message_event):
    """Test monitoring logger functionality"""
    monitoring_data = []

    def monitoring_callback(data):
        monitoring_data.append(data)

    logger = create_monitoring_logger(temp_dir, monitoring_callback)

    # Send test event
    result = await logger(sample_message_event)
    assert result == True, "Should succeed"

    # Check monitoring data
    assert len(monitoring_data) > 0, "Should capture monitoring data"

    monitor_entry = monitoring_data[0]
    assert 'event_type' in monitor_entry, "Should include event type"
    assert 'success' in monitor_entry, "Should include success status"
    assert 'duration_ms' in monitor_entry, "Should include duration"
    assert 'timestamp' in monitor_entry, "Should include timestamp"

    await logger.close()


@pytest.mark.asyncio
async def test_multi_transport_logger(temp_dir, sample_message_event):
    """Test multi-transport logger functionality"""
    # Create multiple transports
    transport1 = LoggingTransport("transport1")
    transport2 = NullTransport()
    file_path = Path(temp_dir) / "multi_transport.jsonl"
    transport3 = FileTransport(str(file_path))

    await transport3.connect()

    # Create multi-transport logger
    logger = create_multi_transport_logger(
        temp_dir,
        [transport1, transport2, transport3]
    )

    # Send test event
    result = await logger(sample_message_event)
    assert result == True, "Should succeed with multi-transport"

    # Verify file transport received event
    assert file_path.exists(), "File transport should create file"

    await logger.close()


@pytest.mark.asyncio
async def test_configuration_validation():
    """Test configuration validation utilities"""
    # Test valid configuration
    valid_config = LoggerConfiguration(
        log_base_dir="logs/test",
        transport_type=TransportType.NULL
    )

    assert validate_logger_config(valid_config) == True, "Valid config should pass validation"

    # Test invalid configuration
    invalid_config = LoggerConfiguration(
        max_retry_attempts=-1,
        transport_type=TransportType.HTTP,
        transport_config={}  # Missing endpoint_url
    )

    # Capture print output
    import io
    from contextlib import redirect_stdout

    output = io.StringIO()
    with redirect_stdout(output):
        result = validate_logger_config(invalid_config)

    assert result == False, "Invalid config should fail validation"
    output_text = output.getvalue()
    assert "Configuration validation errors:" in output_text, "Should print validation errors"


@pytest.mark.asyncio
async def test_logger_info_printing(temp_dir):
    """Test logger info printing utility"""
    logger = create_testing_logger(log_base_dir=temp_dir)

    # Capture print output
    import io
    from contextlib import redirect_stdout

    output = io.StringIO()
    with redirect_stdout(output):
        print_logger_info(logger)

    output_text = output.getvalue()
    assert "EventSessionLogger Configuration:" in output_text, "Should print configuration header"
    assert "Log Directory:" in output_text, "Should print log directory"
    assert "Local Logging:" in output_text, "Should print local logging status"
    assert "Downstream Transport:" in output_text, "Should print transport info"

    await logger.close()


@pytest.mark.asyncio
async def test_logger_with_validation(temp_dir):
    """Test create_logger_with_validation function"""
    # Test valid configuration
    valid_config = LoggerConfiguration(
        log_base_dir=temp_dir,
        transport_type=TransportType.NULL
    )

    # Capture print output
    import io
    from contextlib import redirect_stdout

    output = io.StringIO()
    with redirect_stdout(output):
        logger = create_logger_with_validation(valid_config)

    output_text = output.getvalue()
    assert "EventSessionLogger Configuration:" in output_text, "Should print logger info"

    await logger.close()

    # Test invalid configuration
    invalid_config = LoggerConfiguration(
        max_retry_attempts=-1
    )

    with pytest.raises(EventSessionLoggerError) as exc_info:
        create_logger_with_validation(invalid_config)

    assert "Configuration validation failed" in str(exc_info.value), "Should raise validation error"