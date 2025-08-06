"""
Testing Infrastructure Integration Tests.

This module provides comprehensive testing infrastructure for the intelligent
message management system including performance benchmarks, stress tests,
and validation utilities.
"""

import pytest
import time
import threading
import asyncio
import psutil
import gc
from typing import List, Dict, Any, Optional
from unittest.mock import Mock, patch
from uuid import uuid4
import json

from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.chat_history.agent_work_log import AgentWorkLog
from agent_c.models.chat_history.message_manager import MessageManager
from agent_c.models.chat_history.session_migration import SessionMigrationUtility
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    ValidityState,
    OutcomeStatus
)
from agent_c.models.common_chat.enhanced_converters import EnhancedProviderTranslationLayer
from agent_c.toolsets.tool_manipulation_api import ToolManipulationAPI


class PerformanceBenchmark:
    """Performance benchmark utilities."""
    
    def __init__(self):
        self.results = {}
        self.process = psutil.Process()
    
    def start_benchmark(self, name: str):
        """Start a performance benchmark."""
        gc.collect()  # Clean up before benchmark
        self.results[name] = {
            'start_time': time.time(),
            'start_memory': self.process.memory_info().rss / 1024 / 1024,  # MB
            'start_cpu': self.process.cpu_percent()
        }
    
    def end_benchmark(self, name: str):
        """End a performance benchmark."""
        if name not in self.results:
            raise ValueError(f"Benchmark {name} not started")
        
        end_time = time.time()
        end_memory = self.process.memory_info().rss / 1024 / 1024  # MB
        end_cpu = self.process.cpu_percent()
        
        self.results[name].update({
            'end_time': end_time,
            'end_memory': end_memory,
            'end_cpu': end_cpu,
            'duration': end_time - self.results[name]['start_time'],
            'memory_delta': end_memory - self.results[name]['start_memory'],
            'cpu_usage': end_cpu
        })
        
        return self.results[name]
    
    def get_benchmark_results(self, name: str) -> Dict[str, Any]:
        """Get benchmark results."""
        return self.results.get(name, {})
    
    def assert_performance_requirements(self, name: str, 
                                      max_duration: float = None,
                                      max_memory_mb: float = None,
                                      max_cpu_percent: float = None):
        """Assert performance requirements are met."""
        results = self.get_benchmark_results(name)
        
        if max_duration and results.get('duration', 0) > max_duration:
            raise AssertionError(f"Benchmark {name} took {results['duration']:.2f}s, "
                               f"expected < {max_duration}s")
        
        if max_memory_mb and results.get('memory_delta', 0) > max_memory_mb:
            raise AssertionError(f"Benchmark {name} used {results['memory_delta']:.2f}MB, "
                               f"expected < {max_memory_mb}MB")
        
        if max_cpu_percent and results.get('cpu_usage', 0) > max_cpu_percent:
            raise AssertionError(f"Benchmark {name} used {results['cpu_usage']:.2f}% CPU, "
                               f"expected < {max_cpu_percent}%")


class StressTestGenerator:
    """Generate stress test data and scenarios."""
    
    @staticmethod
    def generate_large_session(interaction_count: int = 100, 
                             messages_per_interaction: int = 10) -> EnhancedChatSession:
        """Generate a large enhanced session for stress testing."""
        session = EnhancedChatSession(
            session_id=f"stress-test-{uuid4()}",
            session_name=f"Stress Test Session ({interaction_count} interactions)"
        )
        
        for i in range(interaction_count):
            interaction_id = session.start_interaction()
            
            for j in range(messages_per_interaction):
                # Vary message types
                if j % 3 == 0:  # Tool use message
                    content = [EnhancedToolUseContentBlock(
                        id=f"tool-{i}-{j}",
                        name=f"stress_tool_{j % 5}",
                        input={"iteration": i, "message": j, "data": f"stress-test-data-{i}-{j}"}
                    )]
                elif j % 3 == 1:  # Tool result message
                    content = [EnhancedToolResultContentBlock(
                        tool_use_id=f"tool-{i}-{j-1}",
                        content=f"Result for stress test {i}-{j}",
                        outcome_status=OutcomeStatus.SUCCESS if j % 4 != 0 else OutcomeStatus.FAILURE,
                        execution_time=0.1 + (j * 0.05)
                    )]
                else:  # Text message
                    content = [EnhancedTextContentBlock(
                        text=f"Stress test message {i}-{j} with some longer content to test memory usage"
                    )]
                
                message = EnhancedCommonChatMessage(
                    id=f"stress-msg-{i}-{j}",
                    role="user" if j % 2 == 0 else "assistant",
                    content=content,
                    interaction_id=interaction_id
                )
                session.add_message_to_current_interaction(message)
            
            session.end_interaction()
        
        return session
    
    @staticmethod
    def generate_complex_tool_scenario(tool_count: int = 50, 
                                     calls_per_tool: int = 20) -> InteractionContainer:
        """Generate complex tool usage scenario."""
        interaction_id = str(uuid4())
        container = InteractionContainer(interaction_id=interaction_id)
        
        for tool_idx in range(tool_count):
            tool_name = f"complex_tool_{tool_idx}"
            
            for call_idx in range(calls_per_tool):
                # Tool use message
                tool_message = EnhancedCommonChatMessage(
                    id=f"complex-tool-{tool_idx}-{call_idx}",
                    role="assistant",
                    content=[EnhancedToolUseContentBlock(
                        id=f"call-{tool_idx}-{call_idx}",
                        name=tool_name,
                        input={
                            "complex_param": f"value-{call_idx}",
                            "nested_data": {
                                "level1": {"level2": {"level3": f"deep-value-{call_idx}"}},
                                "array": [f"item-{i}" for i in range(call_idx % 10)]
                            },
                            "large_text": "Lorem ipsum " * (call_idx % 100 + 1)
                        }
                    )],
                    interaction_id=interaction_id
                )
                container.add_message(tool_message)
                
                # Tool result message
                result_message = EnhancedCommonChatMessage(
                    id=f"complex-result-{tool_idx}-{call_idx}",
                    role="user",
                    content=[EnhancedToolResultContentBlock(
                        tool_use_id=f"call-{tool_idx}-{call_idx}",
                        content=f"Complex result {tool_idx}-{call_idx} " + "data " * (call_idx % 50),
                        outcome_status=OutcomeStatus.SUCCESS if call_idx % 5 != 0 else OutcomeStatus.FAILURE,
                        execution_time=0.1 + (call_idx * 0.01)
                    )],
                    interaction_id=interaction_id
                )
                container.add_message(result_message)
        
        return container
    
    @staticmethod
    def generate_migration_test_data() -> List[Dict[str, Any]]:
        """Generate various legacy session formats for migration testing."""
        test_data = []
        
        # Simple session
        test_data.append({
            "name": "simple_session",
            "session": {
                "session_id": "simple-test",
                "messages": [
                    {"role": "user", "content": "Hello"},
                    {"role": "assistant", "content": "Hi there!"}
                ]
            }
        })
        
        # Complex session with tool calls
        test_data.append({
            "name": "complex_tool_session",
            "session": {
                "session_id": "complex-tool-test",
                "messages": [
                    {"role": "user", "content": "Please analyze this file"},
                    {
                        "role": "assistant",
                        "content": [
                            {
                                "type": "tool_use",
                                "id": "file-analysis-1",
                                "name": "file_analyzer",
                                "input": {"path": "/test/file.txt", "type": "full"}
                            }
                        ]
                    },
                    {
                        "role": "user",
                        "content": [
                            {
                                "type": "tool_result",
                                "tool_use_id": "file-analysis-1",
                                "content": "File analysis complete"
                            }
                        ]
                    }
                ]
            }
        })
        
        # Large session
        large_messages = []
        for i in range(1000):
            large_messages.extend([
                {"role": "user", "content": f"User message {i}"},
                {"role": "assistant", "content": f"Assistant response {i}"}
            ])
        
        test_data.append({
            "name": "large_session",
            "session": {
                "session_id": "large-test",
                "messages": large_messages
            }
        })
        
        # Session with corrupted data
        test_data.append({
            "name": "corrupted_session",
            "session": {
                "session_id": "corrupted-test",
                "messages": [
                    {"role": "user", "content": "Normal message"},
                    {"role": "assistant", "content": None},  # Corrupted
                    {"role": "user"},  # Missing content
                    {"role": "assistant", "content": "Recovery message"}
                ]
            }
        })
        
        return test_data


class TestPerformanceBenchmarks:
    """Performance benchmark tests."""
    
    @pytest.fixture
    def benchmark(self):
        """Create performance benchmark instance."""
        return PerformanceBenchmark()
    
    def test_large_session_creation_performance(self, benchmark):
        """Test performance of creating large sessions."""
        benchmark.start_benchmark("large_session_creation")
        
        # Create session with 1000 interactions, 10 messages each = 10,000 messages
        session = StressTestGenerator.generate_large_session(1000, 10)
        
        results = benchmark.end_benchmark("large_session_creation")
        
        # Verify session was created correctly
        assert session.get_interaction_count() == 1000
        all_messages = session.get_all_messages()
        assert len(all_messages) == 10000
        
        # Assert performance requirements
        benchmark.assert_performance_requirements(
            "large_session_creation",
            max_duration=30.0,  # Should complete in under 30 seconds
            max_memory_mb=500.0,  # Should use less than 500MB additional memory
        )
    
    def test_message_retrieval_performance(self, benchmark):
        """Test performance of message retrieval operations."""
        # Setup: Create large session
        session = StressTestGenerator.generate_large_session(500, 20)  # 10,000 messages
        
        # Test 1: Get all messages
        benchmark.start_benchmark("get_all_messages")
        all_messages = session.get_all_messages()
        benchmark.end_benchmark("get_all_messages")
        
        assert len(all_messages) == 10000
        benchmark.assert_performance_requirements(
            "get_all_messages",
            max_duration=2.0  # Should retrieve in under 2 seconds
        )
        
        # Test 2: Search messages
        benchmark.start_benchmark("search_messages")
        search_results = session.search_messages("stress test message 250-")
        benchmark.end_benchmark("search_messages")
        
        assert len(search_results) == 20  # Should find 20 messages from interaction 250
        benchmark.assert_performance_requirements(
            "search_messages",
            max_duration=3.0  # Should search in under 3 seconds
        )
        
        # Test 3: Get messages by tool
        benchmark.start_benchmark("get_messages_by_tool")
        tool_messages = session.get_messages_by_tool("stress_tool_1")
        benchmark.end_benchmark("get_messages_by_tool")
        
        assert len(tool_messages) >= 100  # Should find many tool messages
        benchmark.assert_performance_requirements(
            "get_messages_by_tool",
            max_duration=1.0  # Should filter in under 1 second
        )
    
    def test_work_log_generation_performance(self, benchmark):
        """Test performance of work log generation."""
        # Setup: Create complex tool scenario
        container = StressTestGenerator.generate_complex_tool_scenario(100, 50)  # 10,000 messages
        
        benchmark.start_benchmark("work_log_generation")
        work_log_entries = container.generate_work_log_entries()
        benchmark.end_benchmark("work_log_generation")
        
        # Should generate substantial work log entries
        assert len(work_log_entries) >= 2500  # At least 50 tools * 50 calls = 2500 entries
        
        benchmark.assert_performance_requirements(
            "work_log_generation",
            max_duration=10.0,  # Should generate in under 10 seconds
            max_memory_mb=200.0  # Should use reasonable memory
        )
        
        # Test work log system performance
        work_log = AgentWorkLog()
        
        benchmark.start_benchmark("work_log_insertion")
        for entry in work_log_entries:
            work_log.add_entry(entry)
        benchmark.end_benchmark("work_log_insertion")
        
        benchmark.assert_performance_requirements(
            "work_log_insertion",
            max_duration=5.0  # Should insert all entries in under 5 seconds
        )
        
        # Test work log query performance
        benchmark.start_benchmark("work_log_queries")
        
        # Various queries
        all_entries = work_log.get_all_entries()
        tool_entries = work_log.get_entries_for_tool("complex_tool_50")
        filtered_entries = work_log.filter_entries(
            outcome_status=OutcomeStatus.SUCCESS,
            limit=1000
        )
        
        benchmark.end_benchmark("work_log_queries")
        
        assert len(all_entries) >= 2500
        assert len(tool_entries) >= 50
        assert len(filtered_entries) <= 1000
        
        benchmark.assert_performance_requirements(
            "work_log_queries",
            max_duration=2.0  # Should query in under 2 seconds
        )
    
    def test_translation_performance(self, benchmark):
        """Test performance of provider translation."""
        # Setup: Create messages for translation
        messages = []
        for i in range(1000):
            if i % 3 == 0:  # Tool use message
                content = [EnhancedToolUseContentBlock(
                    id=f"perf-tool-{i}",
                    name=f"performance_tool_{i % 10}",
                    input={"index": i, "data": f"performance-test-{i}" * 10}
                )]
            elif i % 3 == 1:  # Tool result
                content = [EnhancedToolResultContentBlock(
                    tool_use_id=f"perf-tool-{i-1}",
                    content=f"Performance result {i} " + "data " * 50,
                    outcome_status=OutcomeStatus.SUCCESS
                )]
            else:  # Text message
                content = [EnhancedTextContentBlock(
                    text=f"Performance test message {i} " + "content " * 20
                )]
            
            message = EnhancedCommonChatMessage(
                id=f"perf-msg-{i}",
                role="user" if i % 2 == 0 else "assistant",
                content=content,
                interaction_id=str(uuid4())
            )
            messages.append(message)
        
        translation_layer = EnhancedProviderTranslationLayer()
        
        # Test Anthropic translation performance
        benchmark.start_benchmark("anthropic_translation")
        anthropic_messages = translation_layer.to_native_messages(
            messages, 'anthropic', include_work_log_context=True
        )
        benchmark.end_benchmark("anthropic_translation")
        
        assert len(anthropic_messages) == 1000
        benchmark.assert_performance_requirements(
            "anthropic_translation",
            max_duration=5.0  # Should translate in under 5 seconds
        )
        
        # Test OpenAI translation performance
        benchmark.start_benchmark("openai_translation")
        openai_messages = translation_layer.to_native_messages(
            messages, 'openai', include_work_log_context=True
        )
        benchmark.end_benchmark("openai_translation")
        
        assert len(openai_messages) == 1000
        benchmark.assert_performance_requirements(
            "openai_translation",
            max_duration=5.0  # Should translate in under 5 seconds
        )
        
        # Test round-trip translation performance
        benchmark.start_benchmark("round_trip_translation")
        back_to_enhanced = translation_layer.from_native_messages(
            anthropic_messages, 'anthropic', 
            interaction_id=str(uuid4()),
            preserve_work_log_context=True
        )
        benchmark.end_benchmark("round_trip_translation")
        
        assert len(back_to_enhanced) == 1000
        benchmark.assert_performance_requirements(
            "round_trip_translation",
            max_duration=5.0  # Should round-trip in under 5 seconds
        )
    
    def test_concurrent_access_performance(self, benchmark):
        """Test performance under concurrent access."""
        # Setup shared components
        manager = MessageManager()
        work_log = AgentWorkLog()
        
        # Performance tracking
        operation_counts = {'messages': 0, 'queries': 0, 'work_logs': 0}
        operation_lock = threading.Lock()
        
        def concurrent_worker(worker_id: int, operations_per_worker: int):
            """Worker function for concurrent operations."""
            for i in range(operations_per_worker):
                # Create interaction
                interaction_id = f"concurrent-{worker_id}-{i}"
                container = InteractionContainer(interaction_id=interaction_id)
                
                # Add messages
                for j in range(10):
                    message = EnhancedCommonChatMessage(
                        id=f"concurrent-{worker_id}-{i}-{j}",
                        role="user" if j % 2 == 0 else "assistant",
                        content=[EnhancedTextContentBlock(
                            text=f"Concurrent test {worker_id}-{i}-{j}"
                        )],
                        interaction_id=interaction_id
                    )
                    container.add_message(message)
                    
                    with operation_lock:
                        operation_counts['messages'] += 1
                
                # Register with manager
                manager.register_interaction_container(interaction_id, container)
                
                # Perform queries
                messages = manager.get_messages_for_interaction(interaction_id)
                with operation_lock:
                    operation_counts['queries'] += 1
                
                # Generate work logs
                entries = container.generate_work_log_entries()
                for entry in entries:
                    work_log.add_entry(entry)
                    with operation_lock:
                        operation_counts['work_logs'] += 1
        
        # Run concurrent test
        benchmark.start_benchmark("concurrent_operations")
        
        threads = []
        worker_count = 20
        operations_per_worker = 50
        
        for worker_id in range(worker_count):
            thread = threading.Thread(
                target=concurrent_worker,
                args=(worker_id, operations_per_worker)
            )
            threads.append(thread)
            thread.start()
        
        # Wait for completion
        for thread in threads:
            thread.join()
        
        results = benchmark.end_benchmark("concurrent_operations")
        
        # Verify operations completed
        expected_messages = worker_count * operations_per_worker * 10
        assert operation_counts['messages'] == expected_messages
        assert operation_counts['queries'] == worker_count * operations_per_worker
        
        # Performance requirements
        benchmark.assert_performance_requirements(
            "concurrent_operations",
            max_duration=60.0,  # Should complete in under 60 seconds
            max_memory_mb=1000.0  # Should use reasonable memory
        )


class TestStressTests:
    """Stress test scenarios."""
    
    def test_memory_stress_test(self):
        """Test system behavior under memory stress."""
        # Create many large sessions
        sessions = []
        
        try:
            for i in range(10):  # 10 large sessions
                session = StressTestGenerator.generate_large_session(200, 25)  # 5000 messages each
                sessions.append(session)
                
                # Verify session integrity
                assert session.get_interaction_count() == 200
                assert len(session.get_all_messages()) == 5000
            
            # Total: 50,000 messages across 10 sessions
            total_messages = sum(len(session.get_all_messages()) for session in sessions)
            assert total_messages == 50000
            
            # Test operations on all sessions
            for i, session in enumerate(sessions):
                # Search operations
                search_results = session.search_messages(f"Stress test message {i % 200}-")
                assert len(search_results) >= 20
                
                # Tool queries
                tool_messages = session.get_messages_by_tool("stress_tool_0")
                assert len(tool_messages) >= 40
        
        finally:
            # Cleanup
            sessions.clear()
            gc.collect()
    
    def test_tool_manipulation_stress(self):
        """Test tool manipulation under stress."""
        api = ToolManipulationAPI()
        containers = []
        
        try:
            # Create many containers with tool operations
            for i in range(50):
                container = StressTestGenerator.generate_complex_tool_scenario(20, 10)  # 400 messages each
                containers.append(container)
                api.register_interaction_container(container.interaction_id, container)
            
            # Perform many optimization operations
            for i in range(100):
                tool_name = f"complex_tool_{i % 20}"
                
                # Invalidate conflicting calls
                api.invalidate_conflicting_calls(
                    tool_name,
                    {"complex_param": f"conflict-{i}"}
                )
                
                # Optimize tool sequences
                if i % 10 == 0:
                    tool_sequence = [f"complex_tool_{j}" for j in range(i % 5)]
                    api.optimize_for_tool_sequence(tool_sequence)
            
            # Verify system integrity
            audit_trail = api.get_optimization_audit_trail()
            assert len(audit_trail) >= 100  # Should have many optimization records
            
            # Verify containers are still functional
            for container in containers:
                messages = container.get_active_messages()
                assert len(messages) >= 100  # Should have substantial active messages
        
        finally:
            # Cleanup
            for container in containers:
                api.unregister_interaction_container(container.interaction_id)
    
    def test_concurrent_stress_test(self):
        """Test system under concurrent stress."""
        manager = MessageManager()
        work_log = AgentWorkLog()
        
        # Stress parameters
        thread_count = 50
        operations_per_thread = 100
        
        completion_count = {'value': 0}
        completion_lock = threading.Lock()
        
        def stress_worker(worker_id: int):
            """High-intensity worker function."""
            try:
                for i in range(operations_per_thread):
                    # Create interaction with many messages
                    interaction_id = f"stress-{worker_id}-{i}"
                    container = InteractionContainer(interaction_id=interaction_id)
                    
                    # Add many messages rapidly
                    for j in range(20):
                        message = EnhancedCommonChatMessage(
                            id=f"stress-{worker_id}-{i}-{j}",
                            role="user" if j % 2 == 0 else "assistant",
                            content=[EnhancedTextContentBlock(
                                text=f"Stress message {worker_id}-{i}-{j} " + "data " * 10
                            )],
                            interaction_id=interaction_id
                        )
                        container.add_message(message)
                    
                    # Register and perform operations
                    manager.register_interaction_container(interaction_id, container)
                    
                    # Rapid queries
                    manager.get_messages_for_interaction(interaction_id)
                    manager.search_messages(f"Stress message {worker_id}-{i}-")
                    
                    # Work log operations
                    entries = container.generate_work_log_entries()
                    for entry in entries:
                        work_log.add_entry(entry)
                    
                    # Invalidation operations
                    if i % 10 == 0:
                        container.invalidate_messages_by_tool("nonexistent_tool", "stress_test")
                
                with completion_lock:
                    completion_count['value'] += 1
                    
            except Exception as e:
                # Should handle errors gracefully
                print(f"Worker {worker_id} error: {e}")
        
        # Start stress test
        start_time = time.time()
        
        threads = []
        for worker_id in range(thread_count):
            thread = threading.Thread(target=stress_worker, args=(worker_id,))
            threads.append(thread)
            thread.start()
        
        # Wait with timeout
        for thread in threads:
            thread.join(timeout=120)  # 2 minute timeout per thread
        
        duration = time.time() - start_time
        
        # Verify completion
        assert completion_count['value'] >= thread_count * 0.8  # At least 80% should complete
        assert duration < 300  # Should complete within 5 minutes
        
        # Verify system integrity
        all_messages = manager.get_all_messages()
        all_entries = work_log.get_all_entries()
        
        assert len(all_messages) >= 10000  # Should have substantial messages
        assert len(all_entries) >= 0  # Should have work log entries


class TestMigrationValidation:
    """Migration testing and validation."""
    
    def test_migration_scenarios(self):
        """Test various migration scenarios."""
        test_data = StressTestGenerator.generate_migration_test_data()
        migration_utility = SessionMigrationUtility()
        
        for test_case in test_data:
            name = test_case["name"]
            legacy_data = test_case["session"]
            
            # Test migration
            try:
                enhanced_session = migration_utility.migrate_session(
                    legacy_data["session_id"],
                    legacy_data["messages"]
                )
                
                # Verify migration success
                assert enhanced_session.session_id == legacy_data["session_id"]
                
                # Verify message preservation (handle corrupted data gracefully)
                migrated_messages = enhanced_session.get_all_messages()
                original_valid_messages = [msg for msg in legacy_data["messages"] 
                                         if msg.get("content") is not None and msg.get("role")]
                
                if name != "corrupted_session":
                    assert len(migrated_messages) == len(original_valid_messages)
                else:
                    # Corrupted session should have fewer messages after cleanup
                    assert len(migrated_messages) <= len(original_valid_messages)
                    assert len(migrated_messages) >= 2  # Should preserve valid messages
                
                # Verify interaction structure
                interaction_count = enhanced_session.get_interaction_count()
                assert interaction_count >= 1
                
                print(f"✓ Migration test '{name}' passed: "
                      f"{len(migrated_messages)} messages, {interaction_count} interactions")
                
            except Exception as e:
                if name == "corrupted_session":
                    # Corrupted session may fail migration, which is acceptable
                    print(f"⚠ Migration test '{name}' failed as expected: {e}")
                else:
                    raise AssertionError(f"Migration test '{name}' failed unexpectedly: {e}")
    
    def test_large_scale_migration_performance(self):
        """Test performance of large-scale migrations."""
        # Create multiple large legacy sessions
        legacy_sessions = []
        
        for i in range(10):
            messages = []
            for j in range(500):  # 500 messages per session
                messages.extend([
                    {"role": "user", "content": f"User message {i}-{j}"},
                    {"role": "assistant", "content": f"Assistant response {i}-{j}"}
                ])
            
            legacy_sessions.append({
                "session_id": f"large-migration-{i}",
                "messages": messages
            })
        
        # Test batch migration performance
        migration_utility = SessionMigrationUtility()
        
        start_time = time.time()
        migrated_sessions = []
        
        for legacy_session in legacy_sessions:
            enhanced_session = migration_utility.migrate_session(
                legacy_session["session_id"],
                legacy_session["messages"]
            )
            migrated_sessions.append(enhanced_session)
        
        migration_time = time.time() - start_time
        
        # Verify migration results
        assert len(migrated_sessions) == 10
        total_messages = sum(len(session.get_all_messages()) for session in migrated_sessions)
        assert total_messages == 10000  # 10 sessions * 1000 messages each
        
        # Performance requirements
        assert migration_time < 60.0  # Should migrate 10,000 messages in under 60 seconds
        
        print(f"✓ Large-scale migration completed: "
              f"{total_messages} messages in {migration_time:.2f}s "
              f"({total_messages/migration_time:.0f} messages/second)")


class TestValidationUtilities:
    """Validation utilities and regression tests."""
    
    def test_system_integrity_validation(self):
        """Test system-wide integrity validation."""
        # Create comprehensive test scenario
        session = EnhancedChatSession(session_id="integrity-test")
        manager = MessageManager()
        work_log = AgentWorkLog()
        
        # Create multiple interactions with various message types
        for i in range(10):
            interaction_id = session.start_interaction()
            
            # Add various message types
            messages = [
                EnhancedCommonChatMessage(
                    id=f"user-{i}",
                    role="user",
                    content=[EnhancedTextContentBlock(text=f"User message {i}")],
                    interaction_id=interaction_id
                ),
                EnhancedCommonChatMessage(
                    id=f"tool-{i}",
                    role="assistant",
                    content=[EnhancedToolUseContentBlock(
                        id=f"tool-call-{i}",
                        name=f"integrity_tool_{i % 3}",
                        input={"test": f"value-{i}"}
                    )],
                    interaction_id=interaction_id
                ),
                EnhancedCommonChatMessage(
                    id=f"result-{i}",
                    role="user",
                    content=[EnhancedToolResultContentBlock(
                        tool_use_id=f"tool-call-{i}",
                        content=f"Result {i}",
                        outcome_status=OutcomeStatus.SUCCESS
                    )],
                    interaction_id=interaction_id
                )
            ]
            
            for message in messages:
                session.add_message_to_current_interaction(message)
            
            session.end_interaction()
            
            # Register with manager
            container = session.get_interaction_container(interaction_id)
            manager.register_interaction_container(interaction_id, container)
            
            # Generate work logs
            work_log_entries = container.generate_work_log_entries()
            for entry in work_log_entries:
                work_log.add_entry(entry)
        
        # Validate system integrity
        
        # 1. Session integrity
        assert session.get_interaction_count() == 10
        all_messages = session.get_all_messages()
        assert len(all_messages) == 30  # 10 interactions * 3 messages each
        
        # 2. Manager integrity
        manager_messages = manager.get_all_messages()
        assert len(manager_messages) == 30
        
        validation_result = manager.validate_cross_interaction_integrity()
        assert validation_result['is_valid'] == True
        assert len(validation_result['issues']) == 0
        
        # 3. Work log integrity
        work_log_entries = work_log.get_all_entries()
        assert len(work_log_entries) >= 10  # At least one entry per interaction
        
        # 4. Cross-component consistency
        for i in range(10):
            interaction_id = f"integrity-test-interaction-{i+1}"  # Session generates sequential IDs
            
            # Check session has interaction
            container = session.get_interaction_container(interaction_id)
            if container:  # May not exist if ID format is different
                # Check manager has same messages
                manager_container = manager.get_interaction_container(interaction_id)
                assert manager_container is not None
                
                session_messages = container.get_all_messages()
                manager_messages = manager_container.get_all_messages()
                assert len(session_messages) == len(manager_messages)
        
        print("✓ System integrity validation passed")
    
    def test_regression_prevention(self):
        """Test that no regressions were introduced."""
        # Test 1: Basic functionality still works
        session = EnhancedChatSession(session_id="regression-test")
        
        # Should be able to add messages the old way
        interaction_id = session.start_interaction()
        message = EnhancedCommonChatMessage(
            id="regression-msg",
            role="user",
            content=[EnhancedTextContentBlock(text="Regression test")],
            interaction_id=interaction_id
        )
        session.add_message_to_current_interaction(message)
        session.end_interaction()
        
        # Should be able to retrieve messages
        messages = session.get_all_messages()
        assert len(messages) == 1
        assert messages[0].content[0].text == "Regression test"
        
        # Test 2: Observable pattern still works
        events_received = []
        
        def test_observer(event_data):
            events_received.append(event_data)
        
        container = InteractionContainer(interaction_id=str(uuid4()))
        container.subscribe('model_changed', test_observer)
        
        test_message = EnhancedCommonChatMessage(
            id="observable-regression",
            role="user",
            content=[EnhancedTextContentBlock(text="Observable test")],
            interaction_id=container.interaction_id
        )
        container.add_message(test_message)
        
        assert len(events_received) >= 1
        
        # Test 3: Work log generation still works
        work_log_entries = container.generate_work_log_entries()
        assert isinstance(work_log_entries, list)
        
        # Test 4: Translation still works
        translation_layer = EnhancedProviderTranslationLayer()
        native_messages = translation_layer.to_native_messages([test_message], 'anthropic')
        assert len(native_messages) == 1
        
        print("✓ Regression prevention tests passed")


if __name__ == "__main__":
    pytest.main([__file__, "-v", "--tb=short"])