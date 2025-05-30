"""
Integration tests for chat log pruning system.

This test suite validates the complete end-to-end integration of the chat log
pruning system with agents, session managers, and configurations.
"""

import asyncio
import pytest
import time
from unittest.mock import Mock, AsyncMock, patch, MagicMock
from typing import List, Dict, Any, Optional

from agent_c.util.pruning.config import PrunerConfig
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner
from agent_c.models import MemoryMessage
from agent_c.models.agent_config import AgentConfigurationV3
from agent_c.chat.session_manager import ChatSessionManager


class MockSessionManager(ChatSessionManager):
    """Mock session manager for testing."""
    
    def __init__(self):
        super().__init__()
        self.session_id = "test_session"
        self.user_id = "test_user"
        self._pruner: Optional[ChatLogPruner] = None
        self._messages: List[MemoryMessage] = []
        self._active_memory = Mock()
        self._active_memory.messages = []
        # Override the chat_session to point to our mock
        self.chat_session = Mock()
        self.chat_session.active_memory = self._active_memory
        
    def get_pruner(self) -> Optional[ChatLogPruner]:
        return self._pruner
        
    def set_pruner(self, pruner: Optional[ChatLogPruner]) -> None:
        self._pruner = pruner
        
    def prune_session_if_needed(self, session_id: str, context_limit: int) -> bool:
        print(f"MockSessionManager.prune_session_if_needed called: {len(self._active_memory.messages)} messages, context_limit={context_limit}")
        if self._pruner is None:
            print("No pruner available in MockSessionManager")
            return False
            
        # Convert dict messages to MemoryMessage for pruning
        memory_messages = []
        for msg_dict in self._active_memory.messages:
            memory_msg = MemoryMessage(
                role=msg_dict.get('role', 'user'),
                content=msg_dict.get('content', ''),
                uuid=msg_dict.get('uuid'),
                metadata=msg_dict.get('metadata', {}),
                token_count=msg_dict.get('token_count')
            )
            memory_messages.append(memory_msg)
            
        print(f"Converted to {len(memory_messages)} MemoryMessage objects")
        
        should_prune = self._pruner.should_prune(memory_messages, context_limit)
        print(f"Should prune: {should_prune}")
        if not should_prune:
            return False
            
        print("Starting pruning operation...")
        result = self._pruner.prune_messages(memory_messages, context_limit)
        print(f"Pruning completed: {len(result.messages)} messages after pruning")
        
        # Convert back to dict format
        print(f"Converting {len(result.messages)} pruned messages back to dict format")
        self._active_memory.messages = []
        for memory_msg in result.messages:
            msg_dict = {
                'role': memory_msg.role,
                'content': memory_msg.content
            }
            if memory_msg.uuid:
                msg_dict['uuid'] = memory_msg.uuid
            if memory_msg.metadata:
                msg_dict['metadata'] = memory_msg.metadata
            if memory_msg.token_count:
                msg_dict['token_count'] = memory_msg.token_count
            self._active_memory.messages.append(msg_dict)
            
        print(f"MockSessionManager now has {len(self._active_memory.messages)} messages after pruning")
        return True
        
    def get_session_messages(self) -> List[MemoryMessage]:
        memory_messages = []
        for msg_dict in self._active_memory.messages:
            memory_msg = MemoryMessage(
                role=msg_dict.get('role', 'user'),
                content=msg_dict.get('content', ''),
                uuid=msg_dict.get('uuid'),
                metadata=msg_dict.get('metadata', {}),
                token_count=msg_dict.get('token_count')
            )
            memory_messages.append(memory_msg)
        return memory_messages
        
    def add_test_messages(self, messages: List[Dict[str, Any]]):
        """Helper method to add test messages."""
        print(f"Adding {len(messages)} messages to session manager")
        self._active_memory.messages.extend(messages)
        print(f"Session manager now has {len(self._active_memory.messages)} messages")

    @property
    def active_memory(self):
        return self._active_memory


class MockAgent:
    """Mock agent for testing pruning integration."""
    
    def __init__(self, agent_config: Optional[AgentConfigurationV3] = None):
        self.agent_config = agent_config
        self.model_name = "test-model"
        
    async def _prune_session_if_needed(self, sess_mgr: ChatSessionManager, **kwargs) -> bool:
        """Copy of the actual implementation for testing."""
        try:
            # Debug: print(f"_prune_session_if_needed called with {len(sess_mgr.active_memory.messages)} messages")
            # Get the pruner from session manager (with safe error handling)
            pruner = None
            if hasattr(sess_mgr, 'get_pruner'):
                try:
                    pruner = sess_mgr.get_pruner()
                    print(f"Got pruner: {pruner is not None}")
                except NotImplementedError:
                    # Session manager doesn't implement pruning
                    print("Session manager doesn't implement pruning")
                    return False
            
            if pruner is None:
                print("No pruner available")
                return False
                
            # Get context limit from agent configuration or model config
            context_limit = self._get_context_limit(**kwargs)
            print(f"Context limit: {context_limit}")
            if context_limit is None:
                print("No context limit available")
                return False
                
            # Check if auto-pruning is enabled
            auto_pruning_enabled = self._is_auto_pruning_enabled(**kwargs)
            print(f"Auto-pruning enabled: {auto_pruning_enabled}")
            if not auto_pruning_enabled:
                return False
                
            # Delegate to session manager's pruning method
            session_id = getattr(sess_mgr, 'session_id', 'unknown')
            print(f"Calling prune_session_if_needed with context_limit={context_limit}")
            result = sess_mgr.prune_session_if_needed(session_id, context_limit)
            print(f"Pruning result: {result}")
            return result
            
        except Exception as e:
            # Log error but don't fail the entire request
            print(f"Exception in _prune_session_if_needed: {e}")
            import logging
            import traceback
            traceback.print_exc()
            logging.getLogger(__name__).error(f"Error during session pruning: {e}", exc_info=True)
            return False
            
    def _get_context_limit(self, **kwargs) -> Optional[int]:
        """Copy of the actual implementation for testing."""
        # Check for explicit context limit in kwargs
        context_limit = kwargs.get('context_limit')
        if context_limit is not None:
            return context_limit
            
        # Try to get from agent configuration
        if hasattr(self, 'agent_config') and self.agent_config is not None:
            if hasattr(self.agent_config, 'model_config') and self.agent_config.model_config is not None:
                return getattr(self.agent_config.model_config, 'context_window', None)
                
        # Try to get from model name (fallback)
        model_name = kwargs.get('model_name', getattr(self, 'model_name', None))
        if model_name:
            return self._get_default_context_limit_for_model(model_name)
            
        return None
        
    def _get_default_context_limit_for_model(self, model_name: str) -> Optional[int]:
        """Copy of the actual implementation for testing."""
        # Common model context limits
        model_limits = {
            # Claude models
            'claude-3-5-sonnet-20241022': 200000,
            'claude-3-5-haiku-20241022': 200000,
            'claude-3-opus-20240229': 200000,
            'claude-3-sonnet-20240229': 200000,
            'claude-3-haiku-20240307': 200000,
            # GPT models
            'gpt-4o': 128000,
            'gpt-4o-mini': 128000,
            'gpt-4-turbo': 128000,
            'gpt-4': 8192,
            'gpt-3.5-turbo': 16385,
            # Test models
            'test-model': 1000,
        }
        
        # Exact match first
        if model_name in model_limits:
            return model_limits[model_name]
            
        # Partial matches for model families
        for model_pattern, limit in model_limits.items():
            if model_pattern.split('-')[0] in model_name.lower():
                return limit
                
        return None
        
    def _is_auto_pruning_enabled(self, **kwargs) -> bool:
        """Copy of the actual implementation for testing."""
        # Check explicit override in kwargs
        enable_auto_pruning = kwargs.get('enable_auto_pruning')
        if enable_auto_pruning is not None:
            return enable_auto_pruning
            
        # Check agent configuration
        if hasattr(self, 'agent_config') and self.agent_config is not None:
            return getattr(self.agent_config, 'enable_auto_pruning', False)
            
        return False

    async def _construct_message_array(self, **kwargs) -> List[Dict[str, Any]]:
        """Copy of the actual implementation for testing."""
        sess_mgr: Optional[ChatSessionManager] = kwargs.get("session_manager", None)
        messages: Optional[List[Dict[str, Any]]] = kwargs.get("messages", None)

        if messages is None and sess_mgr is not None:
            # Prune session if needed BEFORE copying messages
            await self._prune_session_if_needed(sess_mgr, **kwargs)
            messages = sess_mgr.active_memory.messages.copy()
            
        return messages or []


def create_test_messages(count: int, tokens_per_message: int = 50) -> List[Dict[str, Any]]:
    """Create test messages for integration testing."""
    messages = []
    
    # Add system message
    messages.append({
        'role': 'system',
        'content': 'You are a helpful assistant.',
        'token_count': 10
    })
    
    # Add conversation messages
    for i in range(count):
        # User message
        messages.append({
            'role': 'user',
            'content': f'User message {i + 1}. ' + 'This is some content. ' * (tokens_per_message // 10),
            'token_count': tokens_per_message
        })
        
        # Assistant message
        messages.append({
            'role': 'assistant',
            'content': f'Assistant response {i + 1}. ' + 'This is a response. ' * (tokens_per_message // 10),
            'token_count': tokens_per_message
        })
        
    return messages


@pytest.mark.asyncio
class TestPruningIntegration:
    """Integration tests for pruning system."""
    
    async def test_basic_pruning_integration(self):
        """Test basic end-to-end pruning integration."""
        print("🧪 Testing basic pruning integration...")
        
        # Create pruner config
        config = PrunerConfig(
            recent_message_count=10,  # Minimum allowed value
            token_threshold_percent=0.5,
            enable_dry_run=False
        )
        
        # Create pruner
        pruner = ChatLogPruner(config)
        
        # Create session manager with pruner
        session_mgr = MockSessionManager()
        session_mgr.set_pruner(pruner)
        
        # Add test messages that exceed context limit
        test_messages = create_test_messages(10, 50)  # ~1000 tokens
        print(f"Created {len(test_messages)} test messages")
        session_mgr.add_test_messages(test_messages)
        print(f"Session manager now has {len(session_mgr.active_memory.messages)} messages")
        
        # Create agent with auto-pruning enabled
        agent_config = AgentConfigurationV3(
            name="test_agent",
            model_id="test-model",
            persona="Test agent",
            enable_auto_pruning=True
        )
        agent = MockAgent(agent_config)
        
        # Test message construction with pruning
        print(f"Before construct_message_array: {len(session_mgr.active_memory.messages)} messages")
        result_messages = await agent._construct_message_array(
            session_manager=session_mgr,
            context_limit=2000  # Should trigger pruning (1010 tokens > 1000 threshold)
        )
        print(f"After construct_message_array: {len(result_messages)} messages returned")
        
        # Verify pruning occurred
        assert len(result_messages) < len(test_messages)
        assert len(result_messages) >= 11  # System + recent 10 messages
        
        # Verify system message preserved
        assert result_messages[0]['role'] == 'system'
        
        # Verify recent messages preserved
        recent_roles = [msg['role'] for msg in result_messages[-10:]]
        assert 'user' in recent_roles and 'assistant' in recent_roles
        
        print("✅ Basic pruning integration test passed")
        
    async def test_pruning_disabled_integration(self):
        """Test that pruning doesn't occur when disabled."""
        print("🧪 Testing pruning disabled integration...")
        
        # Create session manager with pruner but auto-pruning disabled
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        session_mgr = MockSessionManager()
        session_mgr.set_pruner(pruner)
        
        # Add test messages
        test_messages = create_test_messages(5, 50)
        session_mgr.add_test_messages(test_messages)
        
        # Create agent with auto-pruning DISABLED
        agent_config = AgentConfigurationV3(
            name="test_agent",
            model_id="test-model",
            persona="Test agent",
            enable_auto_pruning=False
        )
        agent = MockAgent(agent_config)
        
        # Test message construction
        result_messages = await agent._construct_message_array(
            session_manager=session_mgr,
            context_limit=100  # Would trigger pruning if enabled
        )
        
        # Verify NO pruning occurred
        assert len(result_messages) == len(test_messages)
        
        print("✅ Pruning disabled integration test passed")
        
    async def test_no_pruner_integration(self):
        """Test behavior when no pruner is configured."""
        print("🧪 Testing no pruner integration...")
        
        # Create session manager WITHOUT pruner
        session_mgr = MockSessionManager()
        # Don't set a pruner
        
        # Add test messages
        test_messages = create_test_messages(5, 50)
        session_mgr.add_test_messages(test_messages)
        
        # Create agent with auto-pruning enabled
        agent_config = AgentConfigurationV3(
            name="test_agent",
            model_id="test-model",
            persona="Test agent",
            enable_auto_pruning=True
        )
        agent = MockAgent(agent_config)
        
        # Test message construction
        result_messages = await agent._construct_message_array(
            session_manager=session_mgr,
            context_limit=100  # Would trigger pruning if pruner existed
        )
        
        # Verify NO pruning occurred (no pruner available)
        assert len(result_messages) == len(test_messages)
        
        print("✅ No pruner integration test passed")
        
    async def test_context_limit_detection(self):
        """Test context limit detection from various sources."""
        print("🧪 Testing context limit detection...")
        
        # Test 1: Explicit context limit in kwargs
        agent = MockAgent()
        context_limit = agent._get_context_limit(context_limit=5000)
        assert context_limit == 5000
        
        # Test 2: Model name fallback
        agent.model_name = "gpt-4"
        context_limit = agent._get_context_limit()
        assert context_limit == 8192
        
        # Test 3: Test model
        agent.model_name = "test-model"
        context_limit = agent._get_context_limit()
        assert context_limit == 1000
        
        # Test 4: Unknown model
        agent.model_name = "unknown-model"
        context_limit = agent._get_context_limit()
        assert context_limit is None
        
        print("✅ Context limit detection test passed")
        
    async def test_performance_integration(self):
        """Test pruning performance with large conversations."""
        print("🧪 Testing pruning performance...")
        
        # Create large conversation
        large_messages = create_test_messages(100, 50)  # ~10,000 tokens
        
        # Create pruner and session manager
        config = PrunerConfig(recent_message_count=10)
        pruner = ChatLogPruner(config)
        session_mgr = MockSessionManager()
        session_mgr.set_pruner(pruner)
        session_mgr.add_test_messages(large_messages)
        
        # Create agent
        agent_config = AgentConfigurationV3(
            name="test_agent",
            model_id="test-model",
            persona="Test agent",
            enable_auto_pruning=True
        )
        agent = MockAgent(agent_config)
        
        # Time the pruning operation
        start_time = time.time()
        
        result_messages = await agent._construct_message_array(
            session_manager=session_mgr,
            context_limit=1000  # Should trigger significant pruning
        )
        
        end_time = time.time()
        duration_ms = (end_time - start_time) * 1000
        
        # Verify performance requirement (<100ms)
        assert duration_ms < 100, f"Pruning took {duration_ms:.2f}ms, should be <100ms"
        
        # Verify pruning occurred
        assert len(result_messages) < len(large_messages)
        
        print(f"✅ Performance test passed: {duration_ms:.2f}ms for {len(large_messages)} messages")
        
    async def test_error_handling_integration(self):
        """Test error handling in integration scenarios."""
        print("🧪 Testing error handling integration...")
        
        # Test 1: Session manager without get_pruner method
        class BadSessionManager(ChatSessionManager):
            def __init__(self):
                super().__init__()
                # Override chat_session to avoid property issues
                self.chat_session = Mock()
                self.chat_session.active_memory = Mock()
                self.chat_session.active_memory.messages = []
                
        bad_session_mgr = BadSessionManager()
        agent = MockAgent()
        
        # Should not error, should return False
        result = await agent._prune_session_if_needed(bad_session_mgr)
        assert result is False
        
        # Test 2: Session manager with get_pruner that raises NotImplementedError
        class NotImplementedSessionManager(ChatSessionManager):
            def __init__(self):
                super().__init__()
                # Override chat_session to avoid property issues
                self.chat_session = Mock()
                self.chat_session.active_memory = Mock()
                self.chat_session.active_memory.messages = []
                
            def get_pruner(self):
                raise NotImplementedError("Pruning not implemented")
                
        ni_session_mgr = NotImplementedSessionManager()
        
        # Should not error, should return False
        result = await agent._prune_session_if_needed(ni_session_mgr)
        assert result is False
        
        print("✅ Error handling integration test passed")
        
    async def test_message_preservation_integration(self):
        """Test that important messages are preserved during integration."""
        print("🧪 Testing message preservation integration...")
        
        # Create messages with corrections and tool calls
        messages = [
            {'role': 'system', 'content': 'You are helpful.', 'token_count': 10},
            {'role': 'user', 'content': 'Old message 1', 'token_count': 50},
            {'role': 'assistant', 'content': 'Old response 1', 'token_count': 50},
            {'role': 'user', 'content': 'Actually, I meant something else', 'token_count': 50},  # Correction
            {'role': 'assistant', 'content': 'I understand the correction', 'token_count': 50},
            {'role': 'user', 'content': 'Recent message 1', 'token_count': 50},
            {'role': 'assistant', 'content': 'Recent response 1', 'token_count': 50},
            {'role': 'user', 'content': 'Recent message 2', 'token_count': 50},
            {'role': 'assistant', 'content': 'Recent response 2', 'token_count': 50},
        ]
        
        # Create pruner with correction detection
        config = PrunerConfig(
            recent_message_count=10,  # Minimum allowed value
            correction_keywords=['actually', 'correction', 'wrong', 'mistake'],
            token_threshold_percent=0.5
        )
        pruner = ChatLogPruner(config)
        
        # Set up session manager and agent
        session_mgr = MockSessionManager()
        session_mgr.set_pruner(pruner)
        session_mgr.add_test_messages(messages)
        
        agent_config = AgentConfigurationV3(
            name="test_agent",
            model_id="test-model",
            persona="Test agent",
            enable_auto_pruning=True
        )
        agent = MockAgent(agent_config)
        
        # Trigger pruning
        print(f"Before construct_message_array: {len(session_mgr.active_memory.messages)} messages")
        result_messages = await agent._construct_message_array(
            session_manager=session_mgr,
            context_limit=1000  # Should trigger pruning (450 tokens > 500 threshold)
        )
        print(f"After construct_message_array: {len(result_messages)} messages returned")
        
        # Verify system message preserved
        assert result_messages[0]['role'] == 'system'
        
        # Verify correction message preserved
        correction_contents = [msg['content'] for msg in result_messages]
        assert any('Actually' in content for content in correction_contents)
        
        # Verify recent messages preserved
        recent_contents = [msg['content'] for msg in result_messages[-10:]]
        assert any('Recent message' in content for content in recent_contents)
        
        print("✅ Message preservation integration test passed")


async def run_integration_tests():
    """Run all integration tests."""
    print("🚀 Starting pruning integration tests...")
    
    test_instance = TestPruningIntegration()
    
    try:
        await test_instance.test_basic_pruning_integration()
        await test_instance.test_pruning_disabled_integration()
        await test_instance.test_no_pruner_integration()
        await test_instance.test_context_limit_detection()
        await test_instance.test_performance_integration()
        await test_instance.test_error_handling_integration()
        await test_instance.test_message_preservation_integration()
        
        print("🎉 All integration tests passed!")
        return True
        
    except Exception as e:
        print(f"❌ Integration test failed: {e}")
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    # Run tests directly
    result = asyncio.run(run_integration_tests())
    exit(0 if result else 1)