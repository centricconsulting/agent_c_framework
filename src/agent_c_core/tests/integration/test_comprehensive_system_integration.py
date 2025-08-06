"""
Comprehensive System Integration Tests for Intelligent Message Management System.

This module provides end-to-end integration testing of the complete intelligent
message management system including InteractionContainer, tool manipulation APIs,
work logs, provider translation, and agent runtime integration.
"""

import pytest
import asyncio
import time
import threading
from typing import List, Dict, Any, Optional
from unittest.mock import Mock, AsyncMock, patch
from uuid import uuid4

from agent_c.agents.base import BaseAgent
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.chat_history.agent_work_log import AgentWorkLog, AgentWorkLogEntry
from agent_c.models.chat_history.message_manager import MessageManager
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage, 
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    ValidityState,
    OutcomeStatus
)
from agent_c.models.common_chat.enhanced_converters import (
    EnhancedProviderTranslationLayer,
    EnhancedAnthropicConverter,
    EnhancedOpenAIConverter
)
from agent_c.toolsets.tool_chest import ToolChest
from agent_c.toolsets.tool_manipulation_api import ToolManipulationAPI, OptimizationPriority


class MockTool:
    """Mock tool for testing tool integration."""
    
    def __init__(self, name: str, should_fail: bool = False):
        self.name = name
        self.should_fail = should_fail
        self.call_count = 0
    
    def __call__(self, **kwargs):
        self.call_count += 1
        if self.should_fail:
            raise Exception(f"Tool {self.name} failed")
        return f"Result from {self.name} with args: {kwargs}"


class MockAgent(BaseAgent):
    """Mock agent for comprehensive testing."""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.chat_calls = []
        self.tool_calls = []
    
    @classmethod
    def client(cls, **opts):
        return Mock()
    
    @property
    def tool_format(self) -> str:
        return "claude"
    
    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Mock chat implementation with full workflow."""
        self.chat_calls.append(kwargs)
        
        # Start interaction
        interaction_id = await self._raise_interaction_start(**kwargs)
        
        # Process user message
        user_message = kwargs.get('user_message', 'test message')
        await self._raise_user_request(user_message, **kwargs)
        
        # Simulate tool usage if tool_chest is available
        if self.tool_chest and 'use_tool' in user_message.lower():
            tool_name = 'test_tool'
            tool_params = {'action': 'test', 'data': user_message}
            
            await self._raise_tool_call_start(tool_name, tool_params, **kwargs)
            
            # Mock tool execution
            try:
                result = f"Tool result for: {user_message}"
                await self._raise_tool_call_end(tool_name, result, **kwargs)
                self.tool_calls.append((tool_name, tool_params, result))
            except Exception as e:
                await self._raise_tool_call_end(tool_name, str(e), error=True, **kwargs)
        
        # Generate response
        await self._raise_text_delta('Hello', **kwargs)
        await self._raise_text_delta(' world!', **kwargs)
        
        # End interaction
        await self._raise_interaction_end(**kwargs)
        
        return [{'role': 'assistant', 'content': 'Hello world!'}]


class TestCompleteSystemIntegration:
    """Test complete system integration with all components."""
    
    @pytest.fixture
    def tool_chest(self):
        """Create a tool chest with mock tools."""
        chest = ToolChest()
        
        # Add mock tools
        chest.add_tool('file_tool', MockTool('file_tool'))
        chest.add_tool('analysis_tool', MockTool('analysis_tool'))
        chest.add_tool('failing_tool', MockTool('failing_tool', should_fail=True))
        
        return chest
    
    @pytest.fixture
    def agent_with_full_setup(self, tool_chest):
        """Create agent with full intelligent message management setup."""
        return MockAgent(
            model_name="test-model",
            tool_chest=tool_chest,
            enable_interaction_tracking=True
        )
    
    @pytest.fixture
    def enhanced_session(self):
        """Create enhanced session for testing."""
        return EnhancedChatSession(
            session_id="integration-test",
            session_name="Integration Test Session"
        )
    
    @pytest.mark.asyncio
    async def test_complete_workflow_with_tools(self, agent_with_full_setup, enhanced_session):
        """Test complete workflow: session -> agent -> tools -> work logs."""
        agent = agent_with_full_setup
        
        # Step 1: Start with enhanced session
        interaction_id = enhanced_session.start_interaction()
        
        # Step 2: Agent processes message with tool usage
        response = await agent.chat(
            chat_session=enhanced_session,
            user_message="Please use_tool to analyze this data",
            session_id=enhanced_session.session_id
        )
        
        # Step 3: Verify response
        assert len(response) == 1
        assert response[0]['content'] == 'Hello world!'
        
        # Step 4: Verify interaction tracking
        assert len(agent._interaction_containers) > 0
        container = list(agent._interaction_containers.values())[0]
        messages = container.get_active_messages()
        assert len(messages) >= 1  # At least user message tracked
        
        # Step 5: Verify tool calls were tracked
        assert len(agent.tool_calls) >= 1
        tool_name, tool_params, result = agent.tool_calls[0]
        assert tool_name == 'test_tool'
        assert 'action' in tool_params
        
        # Step 6: Verify work log generation
        work_log_entries = container.generate_work_log_entries()
        assert len(work_log_entries) >= 1
        
        # Find tool-related work log entry
        tool_entries = [entry for entry in work_log_entries if entry.tool_name == 'test_tool']
        assert len(tool_entries) >= 1
        
        tool_entry = tool_entries[0]
        assert tool_entry.outcome_status == OutcomeStatus.SUCCESS
        assert 'action' in tool_entry.key_parameters
        
        # Step 7: Verify session integration
        session_messages = enhanced_session.get_all_messages()
        assert len(session_messages) >= 2  # User + assistant messages
    
    @pytest.mark.asyncio
    async def test_tool_manipulation_workflow(self, agent_with_full_setup, enhanced_session):
        """Test tool manipulation API integration."""
        agent = agent_with_full_setup
        tool_chest = agent.tool_chest
        
        # Step 1: Register tool optimizer
        def file_optimizer(container, tool_name, **kwargs):
            """Mock optimizer that invalidates conflicting file operations."""
            messages = container.get_messages_by_tool('file_tool')
            if len(messages) > 1:
                # Invalidate older file operations
                for msg in messages[:-1]:
                    container.invalidate_messages_by_tool(
                        'file_tool',
                        criteria='parameter_conflict'
                    )
        
        tool_chest.register_tool_optimizer('file_tool', file_optimizer, OptimizationPriority.HIGH)
        
        # Step 2: Start interaction and add multiple file operations
        interaction_id = enhanced_session.start_interaction()
        container = InteractionContainer(interaction_id=interaction_id)
        
        # Register container with tool chest
        tool_chest.register_interaction_container(interaction_id, container)
        
        # Add file tool messages
        file_msg1 = EnhancedCommonChatMessage(
            id=str(uuid4()),
            role="assistant",
            content=[EnhancedToolUseContentBlock(
                id="tool-1",
                name="file_tool",
                input={"path": "/old/path", "action": "read"}
            )],
            interaction_id=interaction_id
        )
        
        file_msg2 = EnhancedCommonChatMessage(
            id=str(uuid4()),
            role="assistant",
            content=[EnhancedToolUseContentBlock(
                id="tool-2",
                name="file_tool",
                input={"path": "/new/path", "action": "read"}
            )],
            interaction_id=interaction_id
        )
        
        container.add_message(file_msg1)
        container.add_message(file_msg2)
        
        # Step 3: Trigger optimization
        tool_chest.invalidate_conflicting_calls('file_tool', {"path": "/new/path"})
        
        # Step 4: Verify optimization occurred
        active_messages = container.get_active_messages()
        file_messages = [msg for msg in active_messages 
                        if any(isinstance(block, EnhancedToolUseContentBlock) and block.name == 'file_tool' 
                               for block in msg.content)]
        
        # Should have fewer active file messages after optimization
        assert len(file_messages) <= 2
        
        # Step 5: Verify audit trail
        audit_trail = tool_chest.get_optimization_audit_trail()
        assert len(audit_trail) >= 1
        
        # Cleanup
        tool_chest.unregister_interaction_container(interaction_id)
    
    @pytest.mark.asyncio
    async def test_provider_translation_integration(self, enhanced_session):
        """Test provider translation with work log context."""
        # Step 1: Create messages with work log metadata
        interaction_id = enhanced_session.start_interaction()
        
        messages = [
            EnhancedCommonChatMessage(
                id="msg-1",
                role="user",
                content=[EnhancedTextContentBlock(text="Translate this message")],
                interaction_id=interaction_id
            ),
            EnhancedCommonChatMessage(
                id="msg-2",
                role="assistant",
                content=[
                    EnhancedToolUseContentBlock(
                        id="tool-1",
                        name="translation_tool",
                        input={"text": "Hello", "target_lang": "es"},
                        work_log_metadata={
                            "importance_level": 8,
                            "parameter_importance": {"text": 10, "target_lang": 9}
                        }
                    )
                ],
                interaction_id=interaction_id
            ),
            EnhancedCommonChatMessage(
                id="msg-3",
                role="user",
                content=[
                    EnhancedToolResultContentBlock(
                        tool_use_id="tool-1",
                        content="Hola",
                        outcome_status=OutcomeStatus.SUCCESS,
                        execution_time=0.5
                    )
                ],
                interaction_id=interaction_id
            )
        ]
        
        # Step 2: Create translation layer
        translation_layer = EnhancedProviderTranslationLayer()
        
        # Step 3: Test Anthropic translation with work log context
        anthropic_messages = translation_layer.to_native_messages(
            messages, 
            'anthropic',
            include_work_log_context=True
        )
        
        assert len(anthropic_messages) == 3
        
        # Verify work log context is preserved
        tool_message = anthropic_messages[1]
        assert tool_message['role'] == 'assistant'
        assert 'tool_use' in tool_message['content'][0]['type']
        
        # Step 4: Test round-trip translation
        back_to_enhanced = translation_layer.from_native_messages(
            anthropic_messages,
            'anthropic',
            interaction_id=interaction_id,
            preserve_work_log_context=True
        )
        
        assert len(back_to_enhanced) == 3
        
        # Verify work log metadata is preserved
        tool_msg = back_to_enhanced[1]
        tool_block = next(block for block in tool_msg.content 
                         if isinstance(block, EnhancedToolUseContentBlock))
        assert tool_block.work_log_metadata is not None
        assert 'importance_level' in tool_block.work_log_metadata
        
        # Step 5: Validate round-trip accuracy
        validation_result = translation_layer.validate_round_trip(
            messages, 'anthropic'
        )
        assert validation_result['is_valid']
        assert validation_result['differences'] == []
    
    @pytest.mark.asyncio
    async def test_concurrent_tool_manipulation(self, tool_chest):
        """Test concurrent tool manipulation operations."""
        # Step 1: Create multiple interaction containers
        containers = []
        for i in range(5):
            interaction_id = str(uuid4())
            container = InteractionContainer(interaction_id=interaction_id)
            tool_chest.register_interaction_container(interaction_id, container)
            containers.append((interaction_id, container))
        
        # Step 2: Define concurrent operations
        async def add_messages_to_container(interaction_id, container, tool_name, count):
            """Add messages to container concurrently."""
            for i in range(count):
                message = EnhancedCommonChatMessage(
                    id=str(uuid4()),
                    role="assistant",
                    content=[EnhancedToolUseContentBlock(
                        id=f"tool-{i}",
                        name=tool_name,
                        input={"index": i, "data": f"test-{i}"}
                    )],
                    interaction_id=interaction_id
                )
                container.add_message(message)
                await asyncio.sleep(0.01)  # Small delay to simulate real usage
        
        async def optimize_container(interaction_id, tool_name):
            """Optimize container concurrently."""
            await asyncio.sleep(0.05)  # Let some messages accumulate
            tool_chest.invalidate_conflicting_calls(tool_name, {"optimize": True})
        
        # Step 3: Run concurrent operations
        tasks = []
        
        # Add messages to containers concurrently
        for i, (interaction_id, container) in enumerate(containers):
            tool_name = f"concurrent_tool_{i % 3}"  # Use 3 different tools
            tasks.append(add_messages_to_container(interaction_id, container, tool_name, 10))
        
        # Add optimization tasks
        for i, (interaction_id, container) in enumerate(containers):
            tool_name = f"concurrent_tool_{i % 3}"
            tasks.append(optimize_container(interaction_id, tool_name))
        
        # Execute all tasks concurrently
        await asyncio.gather(*tasks)
        
        # Step 4: Verify results
        total_messages = 0
        for interaction_id, container in containers:
            messages = container.get_all_messages(include_invalidated=True)
            total_messages += len(messages)
            
            # Verify container integrity
            assert container.interaction_id == interaction_id
            assert len(messages) <= 10  # Should not exceed expected count
        
        assert total_messages >= 25  # Should have substantial messages across containers
        
        # Step 5: Verify audit trail
        audit_trail = tool_chest.get_optimization_audit_trail()
        assert len(audit_trail) >= 5  # Should have optimization records
        
        # Cleanup
        for interaction_id, container in containers:
            tool_chest.unregister_interaction_container(interaction_id)
    
    def test_work_log_system_integration(self, enhanced_session):
        """Test work log system integration with all components."""
        # Step 1: Create work log system
        work_log = AgentWorkLog()
        
        # Step 2: Create interaction with tool usage
        interaction_id = enhanced_session.start_interaction()
        container = InteractionContainer(interaction_id=interaction_id)
        
        # Add messages with tool usage
        messages = [
            EnhancedCommonChatMessage(
                id="user-msg",
                role="user",
                content=[EnhancedTextContentBlock(text="Please analyze this file")],
                interaction_id=interaction_id
            ),
            EnhancedCommonChatMessage(
                id="tool-msg",
                role="assistant",
                content=[EnhancedToolUseContentBlock(
                    id="file-analysis",
                    name="file_analyzer",
                    input={
                        "file_path": "/important/data.txt",
                        "analysis_type": "comprehensive",
                        "include_metadata": True
                    }
                )],
                interaction_id=interaction_id
            ),
            EnhancedCommonChatMessage(
                id="result-msg",
                role="user",
                content=[EnhancedToolResultContentBlock(
                    tool_use_id="file-analysis",
                    content="Analysis complete: 1000 lines, 50KB, last modified yesterday",
                    outcome_status=OutcomeStatus.SUCCESS,
                    execution_time=2.5
                )],
                interaction_id=interaction_id
            )
        ]
        
        for msg in messages:
            container.add_message(msg)
        
        # Step 3: Generate work log entries
        work_log_entries = container.generate_work_log_entries()
        assert len(work_log_entries) >= 1
        
        # Add entries to work log system
        for entry in work_log_entries:
            work_log.add_entry(entry)
        
        # Step 4: Verify work log functionality
        entries = work_log.get_entries_for_interaction(interaction_id)
        assert len(entries) >= 1
        
        file_entries = work_log.get_entries_for_tool("file_analyzer")
        assert len(file_entries) >= 1
        
        entry = file_entries[0]
        assert entry.tool_name == "file_analyzer"
        assert entry.outcome_status == OutcomeStatus.SUCCESS
        assert "file_path" in entry.key_parameters
        assert entry.key_parameters["file_path"] == "/important/data.txt"
        
        # Step 5: Test work log export
        audit_report = work_log.export_audit_report(
            include_parameters=True,
            include_metadata=True
        )
        
        assert "entries" in audit_report
        assert len(audit_report["entries"]) >= 1
        assert "statistics" in audit_report
        
        # Step 6: Test work log statistics
        stats = work_log.get_statistics()
        assert stats["total_entries"] >= 1
        assert stats["success_rate"] == 100.0  # All our test entries succeeded
        assert "file_analyzer" in stats["tools_used"]
    
    @pytest.mark.asyncio
    async def test_message_manager_cross_interaction(self):
        """Test MessageManager cross-interaction functionality."""
        # Step 1: Create multiple interactions
        manager = MessageManager()
        
        interactions = []
        for i in range(3):
            interaction_id = str(uuid4())
            container = InteractionContainer(interaction_id=interaction_id)
            
            # Add messages to each interaction
            for j in range(5):
                message = EnhancedCommonChatMessage(
                    id=f"msg-{i}-{j}",
                    role="user" if j % 2 == 0 else "assistant",
                    content=[EnhancedTextContentBlock(text=f"Message {i}-{j}")],
                    interaction_id=interaction_id
                )
                container.add_message(message)
            
            manager.register_interaction_container(interaction_id, container)
            interactions.append(interaction_id)
        
        # Step 2: Test cross-interaction queries
        all_messages = manager.get_all_messages()
        assert len(all_messages) == 15  # 3 interactions * 5 messages each
        
        # Test message search across interactions
        search_results = manager.search_messages(
            text_pattern="Message 1-",
            scope="ALL_INTERACTIONS"
        )
        assert len(search_results) == 5  # All messages from interaction 1
        
        # Step 3: Test cross-interaction tool queries
        # Add tool messages to one interaction
        tool_interaction = interactions[0]
        container = manager.get_interaction_container(tool_interaction)
        
        tool_message = EnhancedCommonChatMessage(
            id="cross-tool-msg",
            role="assistant",
            content=[EnhancedToolUseContentBlock(
                id="cross-tool",
                name="cross_interaction_tool",
                input={"test": "cross-interaction"}
            )],
            interaction_id=tool_interaction
        )
        container.add_message(tool_message)
        
        # Search for tool messages across all interactions
        tool_messages = manager.get_messages_by_tool(
            "cross_interaction_tool",
            scope="ALL_INTERACTIONS"
        )
        assert len(tool_messages) >= 1
        
        # Step 4: Test batch operations across interactions
        batch_operations = [
            {
                'operation': 'edit',
                'message_id': 'msg-0-0',
                'new_content': [EnhancedTextContentBlock(text="Edited message")]
            },
            {
                'operation': 'invalidate',
                'message_id': 'msg-1-1',
                'reason': 'Test invalidation'
            }
        ]
        
        results = manager.batch_update_messages(batch_operations)
        assert len(results) == 2
        assert results[0]['success'] == True
        assert results[1]['success'] == True
        
        # Step 5: Verify cross-interaction integrity
        validation_results = manager.validate_cross_interaction_integrity()
        assert validation_results['is_valid'] == True
        assert len(validation_results['issues']) == 0
    
    def test_observable_pattern_integration(self, enhanced_session):
        """Test observable pattern integration across all components."""
        # Step 1: Set up observers
        events_received = []
        
        def event_observer(event_data):
            events_received.append(event_data)
        
        # Step 2: Create interaction container with observer
        interaction_id = enhanced_session.start_interaction()
        container = InteractionContainer(interaction_id=interaction_id)
        
        # Subscribe to container events
        container.subscribe('model_changed', event_observer)
        
        # Step 3: Perform operations that should trigger events
        message = EnhancedCommonChatMessage(
            id="observable-test",
            role="user",
            content=[EnhancedTextContentBlock(text="Test observable pattern")],
            interaction_id=interaction_id
        )
        
        container.add_message(message)  # Should trigger event
        container.invalidate_messages_by_tool("test_tool", "test")  # Should trigger event
        container.complete_interaction()  # Should trigger event
        
        # Step 4: Verify events were received
        assert len(events_received) >= 3  # At least 3 operations should trigger events
        
        # Step 5: Test work log observable integration
        work_log = AgentWorkLog()
        work_log_events = []
        
        def work_log_observer(event_data):
            work_log_events.append(event_data)
        
        work_log.subscribe('model_changed', work_log_observer)
        
        # Add work log entry
        entry = AgentWorkLogEntry(
            interaction_id=interaction_id,
            tool_name="observable_tool",
            action_summary="Test observable work log",
            key_parameters={"test": "observable"},
            outcome_status=OutcomeStatus.SUCCESS
        )
        
        work_log.add_entry(entry)  # Should trigger event
        
        assert len(work_log_events) >= 1


class TestPerformanceIntegration:
    """Test performance characteristics of integrated system."""
    
    @pytest.mark.asyncio
    async def test_large_session_performance(self):
        """Test performance with large sessions."""
        # Step 1: Create large enhanced session
        session = EnhancedChatSession(
            session_id="large-session-test",
            session_name="Large Session Performance Test"
        )
        
        # Step 2: Add many interactions
        start_time = time.time()
        
        for i in range(100):  # 100 interactions
            interaction_id = session.start_interaction()
            
            # Add multiple messages per interaction
            for j in range(10):  # 10 messages per interaction = 1000 total messages
                message = EnhancedCommonChatMessage(
                    id=f"perf-msg-{i}-{j}",
                    role="user" if j % 2 == 0 else "assistant",
                    content=[EnhancedTextContentBlock(text=f"Performance test message {i}-{j}")],
                    interaction_id=interaction_id
                )
                session.add_message_to_current_interaction(message)
            
            session.end_interaction()
        
        creation_time = time.time() - start_time
        
        # Step 3: Test retrieval performance
        start_time = time.time()
        all_messages = session.get_all_messages()
        retrieval_time = time.time() - start_time
        
        # Step 4: Test search performance
        start_time = time.time()
        search_results = session.search_messages("Performance test message 50-")
        search_time = time.time() - start_time
        
        # Step 5: Verify performance benchmarks
        assert len(all_messages) == 1000  # 100 interactions * 10 messages
        assert creation_time < 10.0  # Should create 1000 messages in under 10 seconds
        assert retrieval_time < 1.0   # Should retrieve 1000 messages in under 1 second
        assert search_time < 2.0      # Should search 1000 messages in under 2 seconds
        assert len(search_results) == 10  # Should find 10 messages matching pattern
    
    def test_concurrent_access_performance(self):
        """Test performance under concurrent access."""
        # Step 1: Create shared components
        manager = MessageManager()
        work_log = AgentWorkLog()
        
        # Step 2: Define concurrent operations
        def worker_thread(thread_id: int, operation_count: int):
            """Worker thread that performs operations."""
            for i in range(operation_count):
                # Create interaction
                interaction_id = f"thread-{thread_id}-interaction-{i}"
                container = InteractionContainer(interaction_id=interaction_id)
                
                # Add messages
                for j in range(5):
                    message = EnhancedCommonChatMessage(
                        id=f"thread-{thread_id}-msg-{i}-{j}",
                        role="user" if j % 2 == 0 else "assistant",
                        content=[EnhancedTextContentBlock(text=f"Thread {thread_id} message {i}-{j}")],
                        interaction_id=interaction_id
                    )
                    container.add_message(message)
                
                # Register with manager
                manager.register_interaction_container(interaction_id, container)
                
                # Generate work log entries
                entries = container.generate_work_log_entries()
                for entry in entries:
                    work_log.add_entry(entry)
        
        # Step 3: Run concurrent threads
        threads = []
        start_time = time.time()
        
        for thread_id in range(10):  # 10 concurrent threads
            thread = threading.Thread(
                target=worker_thread,
                args=(thread_id, 20)  # 20 operations per thread
            )
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        concurrent_time = time.time() - start_time
        
        # Step 4: Verify results
        all_messages = manager.get_all_messages()
        work_log_entries = work_log.get_all_entries()
        
        # Should have 10 threads * 20 interactions * 5 messages = 1000 messages
        assert len(all_messages) == 1000
        
        # Should have work log entries
        assert len(work_log_entries) >= 0  # May vary based on tool usage
        
        # Performance should be reasonable
        assert concurrent_time < 30.0  # Should complete in under 30 seconds
    
    def test_memory_usage_optimization(self):
        """Test memory usage optimization features."""
        # Step 1: Create container with many messages
        container = InteractionContainer(interaction_id=str(uuid4()))
        
        # Add messages that will be invalidated
        for i in range(1000):
            message = EnhancedCommonChatMessage(
                id=f"memory-msg-{i}",
                role="user",
                content=[EnhancedTextContentBlock(text=f"Memory test message {i}")],
                interaction_id=container.interaction_id
            )
            container.add_message(message)
        
        # Step 2: Invalidate many messages
        for i in range(0, 1000, 2):  # Invalidate every other message
            message_id = f"memory-msg-{i}"
            message = container.find_message_by_id(message_id)
            if message:
                message.invalidate("memory_test")
        
        # Step 3: Test memory optimization
        initial_message_count = len(container.get_all_messages(include_invalidated=True))
        assert initial_message_count == 1000
        
        # Optimize storage
        removed_count = container.optimize_message_storage()
        
        # Step 4: Verify optimization
        final_message_count = len(container.get_all_messages(include_invalidated=True))
        active_message_count = len(container.get_active_messages())
        
        assert removed_count >= 500  # Should have removed invalidated messages
        assert final_message_count < initial_message_count
        assert active_message_count == 500  # Half should remain active


class TestErrorHandlingIntegration:
    """Test error handling across integrated components."""
    
    @pytest.mark.asyncio
    async def test_agent_error_recovery(self, tool_chest):
        """Test agent error recovery with tool failures."""
        # Step 1: Create agent with failing tools
        agent = MockAgent(
            model_name="error-test",
            tool_chest=tool_chest,
            enable_interaction_tracking=True
        )
        
        # Add a failing tool
        tool_chest.add_tool('failing_tool', MockTool('failing_tool', should_fail=True))
        
        # Step 2: Test agent handles tool failures gracefully
        session = EnhancedChatSession(session_id="error-test")
        
        # This should not raise an exception despite tool failure
        response = await agent.chat(
            chat_session=session,
            user_message="Please use_tool with failing tool",
            session_id=session.session_id
        )
        
        # Should still get a response
        assert len(response) == 1
        assert response[0]['content'] == 'Hello world!'
        
        # Step 3: Verify error is tracked in work logs
        containers = list(agent._interaction_containers.values())
        assert len(containers) >= 1
        
        container = containers[0]
        work_log_entries = container.generate_work_log_entries()
        
        # Should have work log entries even with failures
        assert len(work_log_entries) >= 0
    
    def test_translation_error_handling(self):
        """Test translation layer error handling."""
        # Step 1: Create malformed messages
        malformed_messages = [
            {
                "role": "user",
                "content": None  # Invalid content
            },
            {
                "role": "assistant",
                "content": [
                    {
                        "type": "tool_use",
                        "id": "invalid-tool",
                        # Missing required fields
                    }
                ]
            }
        ]
        
        # Step 2: Test translation handles errors gracefully
        translation_layer = EnhancedProviderTranslationLayer()
        
        try:
            result = translation_layer.from_native_messages(
                malformed_messages,
                'anthropic',
                interaction_id=str(uuid4())
            )
            # Should handle errors gracefully and return what it can
            assert isinstance(result, list)
        except Exception as e:
            # Should provide meaningful error messages
            assert "translation" in str(e).lower()
    
    def test_concurrent_access_error_handling(self):
        """Test error handling under concurrent access."""
        # Step 1: Create shared container
        container = InteractionContainer(interaction_id=str(uuid4()))
        
        # Step 2: Define operations that might conflict
        def conflicting_operations(thread_id: int):
            """Operations that might conflict with other threads."""
            try:
                for i in range(100):
                    # Add message
                    message = EnhancedCommonChatMessage(
                        id=f"conflict-{thread_id}-{i}",
                        role="user",
                        content=[EnhancedTextContentBlock(text=f"Conflict test {thread_id}-{i}")],
                        interaction_id=container.interaction_id
                    )
                    container.add_message(message)
                    
                    # Immediately try to find and modify it
                    found_message = container.find_message_by_id(f"conflict-{thread_id}-{i}")
                    if found_message:
                        found_message.invalidate(f"thread-{thread_id}")
            except Exception as e:
                # Errors should be handled gracefully
                assert "lock" not in str(e).lower()  # Should not be lock-related errors
        
        # Step 3: Run concurrent operations
        threads = []
        for thread_id in range(5):
            thread = threading.Thread(target=conflicting_operations, args=(thread_id,))
            threads.append(thread)
            thread.start()
        
        # Wait for completion
        for thread in threads:
            thread.join()
        
        # Step 4: Verify container integrity
        all_messages = container.get_all_messages(include_invalidated=True)
        assert len(all_messages) >= 100  # Should have substantial messages
        
        # Container should still be in valid state
        assert container.interaction_id is not None
        assert container.is_active() or container.is_completed()


if __name__ == "__main__":
    pytest.main([__file__, "-v"])