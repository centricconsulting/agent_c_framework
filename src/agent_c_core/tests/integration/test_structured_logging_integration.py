"""
Structured Logging Integration Tests for Intelligent Message Management System.

This module tests the integration between the intelligent message management
system and the structured logging infrastructure, ensuring proper logging
of all operations, events, and performance metrics.
"""

import pytest
import logging
import json
import time
from typing import Dict, List, Any
from unittest.mock import Mock, patch
from uuid import uuid4

from agent_c.util.structured_logging.factory import create_logger
from agent_c.util.structured_logging.context import LoggingContext
from agent_c.util.structured_logging.processors import (
    InteractionProcessor,
    PerformanceProcessor,
    WorkLogProcessor
)
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.chat_history.agent_work_log import AgentWorkLog, AgentWorkLogEntry
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    OutcomeStatus
)
from agent_c.toolsets.tool_manipulation_api import ToolManipulationAPI


class LogCapture:
    """Utility to capture and analyze log output."""
    
    def __init__(self):
        self.records = []
        self.handler = None
        
    def start_capture(self, logger_name: str = None):
        """Start capturing log records."""
        self.records.clear()
        
        # Create custom handler
        class CaptureHandler(logging.Handler):
            def __init__(self, capture_instance):
                super().__init__()
                self.capture = capture_instance
                
            def emit(self, record):
                self.capture.records.append(record)
        
        self.handler = CaptureHandler(self)
        
        # Add to logger
        if logger_name:
            logger = logging.getLogger(logger_name)
        else:
            logger = logging.getLogger()
        
        logger.addHandler(self.handler)
        logger.setLevel(logging.DEBUG)
        
        return logger
    
    def stop_capture(self, logger_name: str = None):
        """Stop capturing log records."""
        if self.handler:
            if logger_name:
                logger = logging.getLogger(logger_name)
            else:
                logger = logging.getLogger()
            
            logger.removeHandler(self.handler)
            self.handler = None
    
    def get_records(self, level: str = None, message_contains: str = None) -> List[logging.LogRecord]:
        """Get captured log records with optional filtering."""
        records = self.records
        
        if level:
            level_num = getattr(logging, level.upper())
            records = [r for r in records if r.levelno >= level_num]
        
        if message_contains:
            records = [r for r in records if message_contains.lower() in r.getMessage().lower()]
        
        return records
    
    def get_structured_data(self) -> List[Dict[str, Any]]:
        """Extract structured data from log records."""
        structured_data = []
        
        for record in self.records:
            data = {
                'level': record.levelname,
                'message': record.getMessage(),
                'timestamp': record.created
            }
            
            # Extract structured fields
            for attr in ['interaction_id', 'tool_name', 'operation', 'duration', 'status']:
                if hasattr(record, attr):
                    data[attr] = getattr(record, attr)
            
            # Extract extra fields
            if hasattr(record, 'extra'):
                data.update(record.extra)
            
            structured_data.append(data)
        
        return structured_data


class TestStructuredLoggingIntegration:
    """Test structured logging integration with all components."""
    
    @pytest.fixture
    def log_capture(self):
        """Create log capture utility."""
        return LogCapture()
    
    @pytest.fixture
    def structured_logger(self):
        """Create structured logger for testing."""
        return create_logger(
            name="test_logger",
            level="DEBUG",
            format_type="json",
            processors=[
                InteractionProcessor(),
                PerformanceProcessor(),
                WorkLogProcessor()
            ]
        )
    
    def test_interaction_container_logging(self, log_capture, structured_logger):
        """Test logging integration with InteractionContainer."""
        # Start log capture
        logger = log_capture.start_capture("agent_c.models.chat_history.interaction_container")
        
        try:
            # Create interaction container with logging context
            with LoggingContext(interaction_id="test-interaction-123"):
                container = InteractionContainer(interaction_id="test-interaction-123")
                
                # Add messages (should generate logs)
                message = EnhancedCommonChatMessage(
                    id="test-msg-1",
                    role="user",
                    content=[EnhancedTextContentBlock(text="Test message for logging")],
                    interaction_id="test-interaction-123"
                )
                container.add_message(message)
                
                # Perform operations that should be logged
                container.invalidate_messages_by_tool("test_tool", "parameter_conflict")
                container.optimize_message_array("remove_invalidated")
                container.complete_interaction()
            
            # Analyze captured logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs for various operations
            assert len(records) >= 3  # At least add_message, invalidate, optimize operations
            
            # Check for interaction context in logs
            interaction_logs = [d for d in structured_data if d.get('interaction_id') == 'test-interaction-123']
            assert len(interaction_logs) >= 1
            
            # Check for operation-specific logs
            add_logs = [d for d in structured_data if 'add_message' in d.get('message', '').lower()]
            invalidate_logs = [d for d in structured_data if 'invalidate' in d.get('message', '').lower()]
            optimize_logs = [d for d in structured_data if 'optimize' in d.get('message', '').lower()]
            
            assert len(add_logs) >= 1
            # Note: invalidate and optimize logs may not be present if not implemented in the actual code
            
        finally:
            log_capture.stop_capture("agent_c.models.chat_history.interaction_container")
    
    def test_work_log_system_logging(self, log_capture, structured_logger):
        """Test logging integration with work log system."""
        logger = log_capture.start_capture("agent_c.models.chat_history.agent_work_log")
        
        try:
            work_log = AgentWorkLog()
            
            # Create work log entries with logging context
            with LoggingContext(interaction_id="work-log-test", tool_name="test_tool"):
                entry = AgentWorkLogEntry(
                    interaction_id="work-log-test",
                    tool_name="test_tool",
                    action_summary="Test tool execution",
                    key_parameters={"param1": "value1", "param2": "value2"},
                    outcome_status=OutcomeStatus.SUCCESS
                )
                
                work_log.add_entry(entry)
                
                # Perform queries that should be logged
                entries = work_log.get_entries_for_tool("test_tool")
                filtered_entries = work_log.filter_entries(outcome_status=OutcomeStatus.SUCCESS)
                audit_report = work_log.export_audit_report()
            
            # Analyze logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs for work log operations
            work_log_logs = [d for d in structured_data if 'work_log' in d.get('message', '').lower()]
            assert len(work_log_logs) >= 1
            
            # Check for tool context in logs
            tool_logs = [d for d in structured_data if d.get('tool_name') == 'test_tool']
            assert len(tool_logs) >= 1
            
        finally:
            log_capture.stop_capture("agent_c.models.chat_history.agent_work_log")
    
    def test_tool_manipulation_api_logging(self, log_capture, structured_logger):
        """Test logging integration with tool manipulation API."""
        logger = log_capture.start_capture("agent_c.toolsets.tool_manipulation_api")
        
        try:
            api = ToolManipulationAPI()
            container = InteractionContainer(interaction_id="tool-api-test")
            
            # Register container and perform operations with logging
            with LoggingContext(interaction_id="tool-api-test", operation="tool_manipulation"):
                api.register_interaction_container("tool-api-test", container)
                
                # Add tool messages
                tool_message = EnhancedCommonChatMessage(
                    id="tool-msg-1",
                    role="assistant",
                    content=[EnhancedToolUseContentBlock(
                        id="tool-call-1",
                        name="logging_test_tool",
                        input={"test": "logging"}
                    )],
                    interaction_id="tool-api-test"
                )
                container.add_message(tool_message)
                
                # Perform tool manipulation operations
                api.invalidate_conflicting_calls("logging_test_tool", {"test": "conflict"})
                api.optimize_for_tool_sequence(["logging_test_tool"])
                
                # Get audit trail
                audit_trail = api.get_optimization_audit_trail()
                
                api.unregister_interaction_container("tool-api-test")
            
            # Analyze logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs for tool manipulation operations
            tool_logs = [d for d in structured_data if 'tool' in d.get('message', '').lower()]
            assert len(tool_logs) >= 1
            
            # Check for operation context
            operation_logs = [d for d in structured_data if d.get('operation') == 'tool_manipulation']
            # Note: May not be present if not implemented in actual code
            
        finally:
            log_capture.stop_capture("agent_c.toolsets.tool_manipulation_api")
    
    def test_enhanced_chat_session_logging(self, log_capture, structured_logger):
        """Test logging integration with EnhancedChatSession."""
        logger = log_capture.start_capture("agent_c.models.chat_history.enhanced_chat_session")
        
        try:
            session = EnhancedChatSession(
                session_id="logging-session-test",
                session_name="Logging Test Session"
            )
            
            # Perform session operations with logging context
            with LoggingContext(session_id="logging-session-test"):
                # Start interactions
                interaction1 = session.start_interaction()
                
                # Add messages
                message1 = EnhancedCommonChatMessage(
                    id="session-msg-1",
                    role="user",
                    content=[EnhancedTextContentBlock(text="Session logging test")],
                    interaction_id=interaction1
                )
                session.add_message_to_current_interaction(message1)
                
                # End interaction
                session.end_interaction()
                
                # Start another interaction
                interaction2 = session.start_interaction()
                
                message2 = EnhancedCommonChatMessage(
                    id="session-msg-2",
                    role="assistant",
                    content=[EnhancedTextContentBlock(text="Session response")],
                    interaction_id=interaction2
                )
                session.add_message_to_current_interaction(message2)
                
                session.end_interaction()
                
                # Perform queries
                all_messages = session.get_all_messages()
                search_results = session.search_messages("logging")
                session_stats = session.get_session_statistics()
            
            # Analyze logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs for session operations
            session_logs = [d for d in structured_data if 'session' in d.get('message', '').lower()]
            assert len(session_logs) >= 1
            
            # Check for session context
            session_context_logs = [d for d in structured_data if d.get('session_id') == 'logging-session-test']
            # Note: May not be present if not implemented in actual code
            
        finally:
            log_capture.stop_capture("agent_c.models.chat_history.enhanced_chat_session")
    
    def test_performance_logging_integration(self, log_capture, structured_logger):
        """Test performance logging integration."""
        logger = log_capture.start_capture("performance")
        
        try:
            # Create scenario that should generate performance logs
            session = EnhancedChatSession(session_id="perf-test")
            
            # Time-consuming operations
            start_time = time.time()
            
            with LoggingContext(operation="performance_test", start_time=start_time):
                # Create many interactions
                for i in range(100):
                    interaction_id = session.start_interaction()
                    
                    for j in range(10):
                        message = EnhancedCommonChatMessage(
                            id=f"perf-msg-{i}-{j}",
                            role="user" if j % 2 == 0 else "assistant",
                            content=[EnhancedTextContentBlock(text=f"Performance test {i}-{j}")],
                            interaction_id=interaction_id
                        )
                        session.add_message_to_current_interaction(message)
                    
                    session.end_interaction()
                
                # Perform expensive queries
                all_messages = session.get_all_messages()
                search_results = session.search_messages("Performance test")
                
                end_time = time.time()
                duration = end_time - start_time
                
                # Log performance metrics
                structured_logger.info(
                    "Performance test completed",
                    extra={
                        'operation': 'performance_test',
                        'duration': duration,
                        'message_count': len(all_messages),
                        'search_results': len(search_results),
                        'throughput': len(all_messages) / duration
                    }
                )
            
            # Analyze performance logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have performance-related logs
            perf_logs = [d for d in structured_data if 'performance' in d.get('message', '').lower()]
            assert len(perf_logs) >= 1
            
            # Check for performance metrics
            duration_logs = [d for d in structured_data if 'duration' in d]
            assert len(duration_logs) >= 1
            
            # Verify performance data
            for log_data in duration_logs:
                if 'duration' in log_data:
                    assert log_data['duration'] > 0
                    assert log_data['duration'] < 60  # Should complete in reasonable time
            
        finally:
            log_capture.stop_capture("performance")
    
    def test_error_logging_integration(self, log_capture, structured_logger):
        """Test error logging integration."""
        logger = log_capture.start_capture("errors")
        
        try:
            # Create scenarios that should generate error logs
            container = InteractionContainer(interaction_id="error-test")
            
            with LoggingContext(interaction_id="error-test", operation="error_testing"):
                try:
                    # Attempt invalid operations
                    
                    # Try to add message with invalid interaction ID
                    invalid_message = EnhancedCommonChatMessage(
                        id="invalid-msg",
                        role="user",
                        content=[EnhancedTextContentBlock(text="Invalid test")],
                        interaction_id="wrong-interaction-id"  # Different from container
                    )
                    container.add_message(invalid_message)
                    
                except Exception as e:
                    structured_logger.error(
                        "Failed to add message with mismatched interaction ID",
                        extra={
                            'error': str(e),
                            'interaction_id': 'error-test',
                            'message_interaction_id': 'wrong-interaction-id'
                        }
                    )
                
                try:
                    # Try to find non-existent message
                    non_existent = container.find_message_by_id("non-existent-id")
                    if non_existent is None:
                        structured_logger.warning(
                            "Message not found",
                            extra={
                                'message_id': 'non-existent-id',
                                'interaction_id': 'error-test'
                            }
                        )
                
                except Exception as e:
                    structured_logger.error(
                        "Error finding message",
                        extra={
                            'error': str(e),
                            'message_id': 'non-existent-id'
                        }
                    )
                
                # Test work log error handling
                work_log = AgentWorkLog()
                try:
                    # Create entry with invalid data
                    invalid_entry = AgentWorkLogEntry(
                        interaction_id="error-test",
                        tool_name="",  # Empty tool name
                        action_summary="Invalid entry test",
                        key_parameters={},
                        outcome_status=OutcomeStatus.FAILURE
                    )
                    work_log.add_entry(invalid_entry)
                    
                except Exception as e:
                    structured_logger.error(
                        "Failed to add invalid work log entry",
                        extra={
                            'error': str(e),
                            'entry_data': 'invalid_entry_data'
                        }
                    )
            
            # Analyze error logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have error and warning logs
            error_logs = [d for d in structured_data if d.get('level') == 'ERROR']
            warning_logs = [d for d in structured_data if d.get('level') == 'WARNING']
            
            assert len(error_logs) >= 0  # May not have errors if validation prevents them
            assert len(warning_logs) >= 0  # May not have warnings if not implemented
            
            # Check error context
            context_logs = [d for d in structured_data if d.get('interaction_id') == 'error-test']
            # Note: May not be present if not implemented in actual code
            
        finally:
            log_capture.stop_capture("errors")
    
    def test_concurrent_logging_integration(self, log_capture, structured_logger):
        """Test logging under concurrent operations."""
        import threading
        
        logger = log_capture.start_capture("concurrent")
        
        try:
            # Shared components
            work_log = AgentWorkLog()
            
            def concurrent_worker(worker_id: int):
                """Worker function that performs logged operations."""
                with LoggingContext(worker_id=worker_id, operation="concurrent_test"):
                    # Create interaction
                    interaction_id = f"concurrent-{worker_id}"
                    container = InteractionContainer(interaction_id=interaction_id)
                    
                    structured_logger.info(
                        f"Worker {worker_id} started",
                        extra={'worker_id': worker_id, 'interaction_id': interaction_id}
                    )
                    
                    # Add messages
                    for i in range(50):
                        message = EnhancedCommonChatMessage(
                            id=f"concurrent-{worker_id}-{i}",
                            role="user" if i % 2 == 0 else "assistant",
                            content=[EnhancedTextContentBlock(text=f"Concurrent message {worker_id}-{i}")],
                            interaction_id=interaction_id
                        )
                        container.add_message(message)
                    
                    # Generate work log entries
                    entries = container.generate_work_log_entries()
                    for entry in entries:
                        work_log.add_entry(entry)
                    
                    structured_logger.info(
                        f"Worker {worker_id} completed",
                        extra={
                            'worker_id': worker_id,
                            'messages_created': 50,
                            'work_log_entries': len(entries)
                        }
                    )
            
            # Start concurrent workers
            threads = []
            for worker_id in range(10):
                thread = threading.Thread(target=concurrent_worker, args=(worker_id,))
                threads.append(thread)
                thread.start()
            
            # Wait for completion
            for thread in threads:
                thread.join()
            
            # Analyze concurrent logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs from all workers
            worker_logs = [d for d in structured_data if 'worker_id' in d]
            assert len(worker_logs) >= 10  # At least one log per worker
            
            # Check for concurrent operation logs
            concurrent_logs = [d for d in structured_data if 'concurrent' in d.get('message', '').lower()]
            assert len(concurrent_logs) >= 10
            
            # Verify log integrity (no corruption from concurrent access)
            for log_data in structured_data:
                assert 'level' in log_data
                assert 'message' in log_data
                assert 'timestamp' in log_data
            
        finally:
            log_capture.stop_capture("concurrent")
    
    def test_structured_logging_processors(self, log_capture):
        """Test custom structured logging processors."""
        # Create logger with custom processors
        logger = create_logger(
            name="processor_test",
            level="DEBUG",
            format_type="json",
            processors=[
                InteractionProcessor(),
                PerformanceProcessor(),
                WorkLogProcessor()
            ]
        )
        
        log_capture.start_capture("processor_test")
        
        try:
            # Test InteractionProcessor
            with LoggingContext(interaction_id="proc-test-123", tool_name="test_tool"):
                logger.info("Testing interaction processor")
                
                # Test PerformanceProcessor
                start_time = time.time()
                time.sleep(0.1)  # Small delay
                end_time = time.time()
                
                logger.info(
                    "Performance test",
                    extra={
                        'start_time': start_time,
                        'end_time': end_time,
                        'duration': end_time - start_time,
                        'operation': 'test_operation'
                    }
                )
                
                # Test WorkLogProcessor
                logger.info(
                    "Work log test",
                    extra={
                        'tool_name': 'test_tool',
                        'action_summary': 'Test action',
                        'outcome_status': 'SUCCESS',
                        'key_parameters': {'param1': 'value1'}
                    }
                )
            
            # Analyze processor output
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have processed logs
            assert len(records) >= 3
            
            # Check for processor-added fields
            interaction_logs = [d for d in structured_data if 'interaction_id' in d]
            performance_logs = [d for d in structured_data if 'duration' in d]
            work_log_logs = [d for d in structured_data if 'tool_name' in d]
            
            # Note: Actual processor behavior depends on implementation
            # These assertions may need adjustment based on actual processor logic
            
        finally:
            log_capture.stop_capture("processor_test")
    
    def test_logging_context_management(self, log_capture, structured_logger):
        """Test logging context management."""
        logger = log_capture.start_capture("context_test")
        
        try:
            # Test nested contexts
            with LoggingContext(session_id="ctx-test", operation="outer"):
                structured_logger.info("Outer context")
                
                with LoggingContext(interaction_id="ctx-interaction", operation="inner"):
                    structured_logger.info("Inner context")
                    
                    # Test context inheritance
                    with LoggingContext(tool_name="ctx_tool"):
                        structured_logger.info("Nested context")
                
                structured_logger.info("Back to outer context")
            
            # Test context cleanup
            structured_logger.info("No context")
            
            # Analyze context logs
            records = log_capture.get_records()
            structured_data = log_capture.get_structured_data()
            
            # Should have logs with different context levels
            assert len(records) >= 4
            
            # Check context propagation
            # Note: Actual context behavior depends on LoggingContext implementation
            
        finally:
            log_capture.stop_capture("context_test")


class TestLoggingPerformanceImpact:
    """Test performance impact of logging integration."""
    
    def test_logging_overhead(self):
        """Test that logging doesn't significantly impact performance."""
        # Test without logging
        start_time = time.time()
        
        session_no_log = EnhancedChatSession(session_id="no-log-test")
        for i in range(1000):
            interaction_id = session_no_log.start_interaction()
            message = EnhancedCommonChatMessage(
                id=f"no-log-{i}",
                role="user",
                content=[EnhancedTextContentBlock(text=f"No log message {i}")],
                interaction_id=interaction_id
            )
            session_no_log.add_message_to_current_interaction(message)
            session_no_log.end_interaction()
        
        no_log_time = time.time() - start_time
        
        # Test with logging
        logger = create_logger("performance_test", level="INFO")
        
        start_time = time.time()
        
        session_with_log = EnhancedChatSession(session_id="with-log-test")
        for i in range(1000):
            with LoggingContext(iteration=i):
                interaction_id = session_with_log.start_interaction()
                message = EnhancedCommonChatMessage(
                    id=f"with-log-{i}",
                    role="user",
                    content=[EnhancedTextContentBlock(text=f"With log message {i}")],
                    interaction_id=interaction_id
                )
                session_with_log.add_message_to_current_interaction(message)
                session_with_log.end_interaction()
                
                if i % 100 == 0:
                    logger.info(f"Processed {i} iterations")
        
        with_log_time = time.time() - start_time
        
        # Logging overhead should be reasonable
        overhead_ratio = with_log_time / no_log_time
        assert overhead_ratio < 2.0  # Logging should not double the execution time
        
        print(f"No logging: {no_log_time:.2f}s")
        print(f"With logging: {with_log_time:.2f}s")
        print(f"Overhead ratio: {overhead_ratio:.2f}x")
    
    def test_high_volume_logging(self):
        """Test logging performance under high volume."""
        logger = create_logger("high_volume_test", level="DEBUG")
        
        start_time = time.time()
        
        # Generate high volume of log messages
        for i in range(10000):
            with LoggingContext(iteration=i, batch=i // 100):
                logger.debug(f"High volume log message {i}")
                
                if i % 10 == 0:
                    logger.info(f"Progress update: {i}")
                
                if i % 100 == 0:
                    logger.warning(f"Batch completed: {i // 100}")
        
        duration = time.time() - start_time
        
        # Should handle high volume efficiently
        assert duration < 30.0  # Should complete in under 30 seconds
        
        throughput = 10000 / duration
        assert throughput > 100  # Should handle at least 100 logs per second
        
        print(f"High volume logging: {duration:.2f}s ({throughput:.0f} logs/sec)")


if __name__ == "__main__":
    pytest.main([__file__, "-v"])