#!/usr/bin/env python3
"""
Pytest-Compatible Comprehensive EventSessionLogger Test Suite

This version is restructured to work with pytest while maintaining
all the comprehensive testing functionality.
"""

import asyncio
import json
import sys
import tempfile
import time
import psutil
import pytest
from pathlib import Path

# Add the src directory to Python path for imports
agent_c_core_dir = Path(__file__).parents[4]
sys.path.insert(0, str(agent_c_core_dir / 'src'))

from agent_c.util.event_logging.event_session_logger import EventSessionLogger
from agent_c.util.event_logging.event_session_logger_factory import (
    create_development_logger, create_testing_logger, LoggerConfiguration, TransportType
)
from agent_c.util.transports import (
    NullTransport, LoggingTransport, FileTransport, TransportState
)
from agent_c.util.transport_exceptions import (
    TransportError
)
from agent_c.models.events.chat import (
    InteractionEvent, CompletionEvent, MessageEvent, TextDeltaEvent,
    HistoryEvent, SystemMessageEvent
)
from agent_c.models.events.render_media import RenderMediaEvent
from agent_c.models.events.tool_calls import ToolCallEvent, ToolCallDeltaEvent


class TestResults:
    """Track test results and metrics"""

    def __init__(self):
        self.passed = 0
        self.failed = 0
        self.total = 0
        self.performance_metrics = {}
        self.coverage_data = {}
        self.error_scenarios = {}


@pytest.fixture
def results():
    """Pytest fixture for test results tracking"""
    return TestResults()


class FailingTransport:
    """Transport that always fails for error testing"""

    def __init__(self, failure_type="general"):
        self.failure_type = failure_type
        self.attempt_count = 0
        self.state = TransportState.CONNECTED

    async def send(self, event, metadata=None):
        self.attempt_count += 1
        if self.failure_type == "timeout":
            await asyncio.sleep(10)  # Simulate timeout
        raise TransportError(f"Simulated {self.failure_type} failure")

    async def connect(self):
        return True

    async def disconnect(self):
        pass

    async def close(self):
        pass

    async def health_check(self):
        return False

    @property
    def is_connected(self):
        return True


class SlowTransport:
    """Transport with configurable delays for performance testing"""

    def __init__(self, delay_ms=10):
        self.delay_ms = delay_ms
        self.events_processed = 0
        self.state = TransportState.CONNECTED

    async def send(self, event, metadata=None):
        await asyncio.sleep(self.delay_ms / 1000.0)
        self.events_processed += 1
        return True

    async def connect(self):
        return True

    async def disconnect(self):
        pass

    async def close(self):
        pass

    async def health_check(self):
        return True

    @property
    def is_connected(self):
        return True


class MemoryTracker:
    """Track memory usage during tests"""

    def __init__(self):
        self.process = psutil.Process()
        self.initial_memory = self.get_memory_mb()
        self.peak_memory = self.initial_memory
        self.measurements = []

    def get_memory_mb(self):
        return self.process.memory_info().rss / 1024 / 1024

    def record_measurement(self, label=""):
        current_memory = self.get_memory_mb()
        self.peak_memory = max(self.peak_memory, current_memory)
        self.measurements.append({
            'label': label,
            'memory_mb': current_memory,
            'delta_mb': current_memory - self.initial_memory,
            'timestamp': time.time()
        })
        return current_memory

    def get_peak_usage(self):
        return self.peak_memory - self.initial_memory


# Unit Tests

@pytest.mark.asyncio
async def test_gateway_core_functionality():
    """Test core EventSessionLogger gateway functionality"""
    print("Testing gateway core functionality...")

    with tempfile.TemporaryDirectory() as temp_dir:
        # Test basic gateway operation
        logger = create_testing_logger(log_base_dir=temp_dir)

        # Test event processing
        event = MessageEvent(
            session_id="gateway_test",
            role="assistant",
            content="Gateway test message"
        )

        result = await logger(event)
        assert result == True, "Gateway should process events successfully"

        # Verify local logging
        session_dir = Path(temp_dir) / "gateway_test"
        assert session_dir.exists(), "Session directory should be created"

        log_files = list(session_dir.glob("*.jsonl"))
        assert len(log_files) > 0, "Log files should be created"

        # Verify log content
        with open(log_files[0], 'r') as f:
            log_entry = json.loads(f.read().strip())
            assert 'timestamp' in log_entry, "Log should have timestamp"
            assert 'event' in log_entry, "Log should have event data"
            assert log_entry['event']['session_id'] == "gateway_test", "Session ID should match"

        await logger.close()

    print("✅ Gateway core functionality test passed")


@pytest.mark.asyncio
async def test_transport_abstraction():
    """Test transport interface abstraction"""
    print("Testing transport abstraction...")

    with tempfile.TemporaryDirectory() as temp_dir:
        # Test different transport types
        transports = [
            NullTransport(),
            LoggingTransport("test_transport"),
            FileTransport(str(Path(temp_dir) / "transport_test.jsonl"))
        ]

        for transport in transports:
            await transport.connect()

            # Test transport interface
            assert hasattr(transport, 'send'), "Transport should have send method"
            assert hasattr(transport, 'connect'), "Transport should have connect method"
            assert hasattr(transport, 'close'), "Transport should have close method"
            assert hasattr(transport, 'health_check'), "Transport should have health_check method"
            assert hasattr(transport, 'is_connected'), "Transport should have is_connected property"

            # Test send operation
            event = {"type": "test", "data": "transport test"}
            result = await transport.send(event)
            assert result == True, f"Transport {transport.__class__.__name__} should send successfully"

            # Test health check
            health = await transport.health_check()
            assert health == True, f"Transport {transport.__class__.__name__} should be healthy"

            await transport.close()

    print("✅ Transport abstraction test passed")


@pytest.mark.asyncio
async def test_error_isolation():
    """Test error isolation between logging and transport"""
    print("Testing error isolation...")

    with tempfile.TemporaryDirectory() as temp_dir:
        # Test transport failure doesn't affect logging
        failing_transport = FailingTransport("connection")
        logger = EventSessionLogger(
            log_base_dir=temp_dir,
            downstream_transport=failing_transport
        )

        event = MessageEvent(
            session_id="error_isolation_test",
            role="assistant",
            content="Error isolation test"
        )

        # Should succeed despite transport failure
        result = await logger(event)
        assert result == True, "Local logging should succeed despite transport failure"

        # Verify local logging worked
        session_dir = Path(temp_dir) / "error_isolation_test"
        assert session_dir.exists(), "Session directory should be created"

        log_files = list(session_dir.glob("*.jsonl"))
        assert len(log_files) > 0, "Log files should be created despite transport failure"

        # Verify transport was attempted
        assert failing_transport.attempt_count > 0, "Transport should have been attempted"

        await logger.close()

    print("✅ Error isolation test passed")


@pytest.mark.asyncio
async def test_configuration_system():
    """Test configuration and factory functions"""
    print("Testing configuration system...")

    with tempfile.TemporaryDirectory() as temp_dir:
        # Test configuration-based creation
        config = LoggerConfiguration(
            log_base_dir=temp_dir,
            transport_type=TransportType.NULL,
            debug_mode=True,
            max_retry_attempts=5
        )

        from agent_c.util.event_logging.event_session_logger_factory import create_logger_from_config
        logger = create_logger_from_config(config)

        assert str(logger.log_base_dir) == temp_dir, "Should use configured directory"
        assert logger.max_retry_attempts == 5, "Should use configured retry attempts"

        # Test environment-specific loggers
        dev_logger = create_development_logger(log_base_dir=temp_dir)
        assert dev_logger.downstream_transport is not None, "Dev logger should have transport"

        test_logger = create_testing_logger(log_base_dir=temp_dir)
        assert isinstance(test_logger.downstream_transport, NullTransport), "Test logger should use null transport"

        await logger.close()
        await dev_logger.close()
        await test_logger.close()

    print("✅ Configuration system test passed")


# Integration Tests

@pytest.mark.asyncio
async def test_event_model_integration():
    """Test integration with all event models"""
    print("Testing event model integration...")

    with tempfile.TemporaryDirectory() as temp_dir:
        logger = create_testing_logger(log_base_dir=temp_dir)

        # Test all event types
        events = [
            InteractionEvent(session_id="integration_test", role="assistant", started=True, id="int_1"),
            CompletionEvent(session_id="integration_test", role="assistant", running=True, completion_options={}),
            MessageEvent(session_id="integration_test", role="assistant", content="Test message"),
            TextDeltaEvent(session_id="integration_test", role="assistant", content="Delta text"),
            HistoryEvent(session_id="integration_test", role="system", messages=[]),
            SystemMessageEvent(session_id="integration_test", role="system", content="System message"),
            RenderMediaEvent(session_id="integration_test", role="assistant", **{"content-type": "image/png"}),
            ToolCallEvent(session_id="integration_test", role="assistant", active=True, vendor="test", tool_calls=[]),
            ToolCallDeltaEvent(session_id="integration_test", role="assistant", tool_calls=[])
        ]

        # Process all events
        for event in events:
            result = await logger(event)
            assert result == True, f"Should process {event.__class__.__name__} successfully"

        # Verify all events were logged
        session_dir = Path(temp_dir) / "integration_test"
        assert session_dir.exists(), "Session directory should be created"

        log_files = list(session_dir.glob("*.jsonl"))
        assert len(log_files) > 0, "Log files should be created"

        # Count logged events
        total_logged = 0
        for log_file in log_files:
            with open(log_file, 'r') as f:
                total_logged += len(f.readlines())

        assert total_logged == len(events), "All events should be logged"

        await logger.close()

    print("✅ Event model integration test passed")


@pytest.mark.asyncio
async def test_agent_compatibility():
    """Test compatibility with agent streaming callbacks"""
    print("Testing agent compatibility...")

    with tempfile.TemporaryDirectory() as temp_dir:
        # Simulate agent streaming callback usage
        received_events = []

        async def mock_agent_callback(event):
            received_events.append(event)

        # Create logger that acts as streaming callback
        logger = EventSessionLogger(
            log_base_dir=temp_dir,
            downstream_callback=mock_agent_callback
        )

        # Simulate agent calling streaming callback
        event = MessageEvent(
            session_id="agent_compat_test",
            role="assistant",
            content="Agent compatibility test"
        )

        # Agent would call: await streaming_callback(event)
        result = await logger(event)
        assert result == True, "Should work as streaming callback"

        # Verify both local logging and callback were called
        assert len(received_events) == 1, "Callback should receive event"
        assert received_events[0] == event, "Callback should receive correct event"

        # Verify local logging
        session_dir = Path(temp_dir) / "agent_compat_test"
        assert session_dir.exists(), "Local logging should still work"

        await logger.close()

    print("✅ Agent compatibility test passed")


# Performance Tests (marked as slow)

@pytest.mark.slow
@pytest.mark.asyncio
async def test_high_volume_processing():
    """Test high-volume event processing"""
    print("Testing high-volume event processing...")

    with tempfile.TemporaryDirectory() as temp_dir:
        memory_tracker = MemoryTracker()
        logger = create_testing_logger(log_base_dir=temp_dir)

        # Process large number of events
        num_events = 1000
        start_time = time.time()
        memory_tracker.record_measurement("start")

        for i in range(num_events):
            event = MessageEvent(
                session_id=f"perf_session_{i % 10}",  # 10 different sessions
                role="assistant",
                content=f"Performance test message {i}"
            )
            await logger(event)

            # Record memory every 100 events
            if i % 100 == 0:
                memory_tracker.record_measurement(f"event_{i}")

        end_time = time.time()
        memory_tracker.record_measurement("end")

        # Calculate performance metrics
        total_time = end_time - start_time
        events_per_second = num_events / total_time
        peak_memory_mb = memory_tracker.get_peak_usage()

        # Performance assertions
        assert events_per_second > 50, f"Should process >50 events/sec, got {events_per_second:.2f}"
        assert peak_memory_mb < 150, f"Should use <150MB peak memory, got {peak_memory_mb:.2f}MB"

        await logger.close()

    print(f"✅ High-volume processing test passed ({events_per_second:.2f} events/sec, {peak_memory_mb:.2f}MB peak)")


# Run all tests if called directly
if __name__ == "__main__":
    # This allows the file to still be run standalone
    pytest.main([__file__, "-v"])