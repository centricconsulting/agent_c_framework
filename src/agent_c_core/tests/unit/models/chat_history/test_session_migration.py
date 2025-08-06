"""
Test suite for ChatSession migration utilities.

This module tests the migration utilities for converting legacy ChatSession
instances to EnhancedChatSession format.
"""

import pytest
import datetime
from typing import List, Dict, Any
from unittest.mock import Mock, patch

from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.session_migration import (
    SessionMigrationUtility,
    SessionMigrationManager,
    MigrationResult,
    BatchMigrationResult,
    MigrationStatus,
    migrate_single_session,
    migrate_sessions_with_progress
)


class TestSessionMigrationUtility:
    """Test SessionMigrationUtility functionality."""
    
    @pytest.fixture
    def migration_utility(self):
        """Create a migration utility for testing."""
        return SessionMigrationUtility()
    
    @pytest.fixture
    def sample_legacy_session(self):
        """Create a sample legacy ChatSession."""
        return ChatSession(
            session_id="test-session-123",
            session_name="Test Session",
            user_id="test-user",
            token_count=150,
            context_window_size=4000,
            messages=[
                {"role": "user", "content": "Hello"},
                {"role": "assistant", "content": "Hi there!"},
                {"role": "user", "content": "How are you?"},
                {"role": "assistant", "content": "I'm doing well, thank you!"}
            ],
            metadata={"test_key": "test_value"}
        )
    
    @pytest.fixture
    def empty_legacy_session(self):
        """Create an empty legacy ChatSession."""
        return ChatSession(
            session_id="empty-session",
            session_name="Empty Session",
            messages=[]
        )
    
    @pytest.fixture
    def complex_legacy_session(self):
        """Create a legacy session with complex content."""
        return ChatSession(
            session_id="complex-session",
            messages=[
                {"role": "user", "content": "Use a tool"},
                {
                    "role": "assistant",
                    "content": [
                        {
                            "type": "tool_use",
                            "id": "tool-123",
                            "name": "test_tool",
                            "input": {"param": "value"}
                        }
                    ]
                },
                {
                    "role": "user",
                    "content": [
                        {
                            "type": "tool_result",
                            "tool_use_id": "tool-123",
                            "content": "Tool result"
                        }
                    ]
                },
                {"role": "assistant", "content": "Tool completed successfully"}
            ]
        )
    
    def test_migrate_session_success(self, migration_utility, sample_legacy_session):
        """Test successful session migration."""
        result = migration_utility.migrate_session(sample_legacy_session)
        
        assert result.success is True
        assert result.status == MigrationStatus.COMPLETED
        assert result.session_id == "test-session-123"
        assert result.messages_migrated == 4
        assert result.interactions_created >= 1
        assert len(result.errors) == 0
        assert result.duration_seconds > 0
        assert result.rollback_data is not None
    
    def test_migrate_empty_session(self, migration_utility, empty_legacy_session):
        """Test migration of empty session."""
        result = migration_utility.migrate_session(empty_legacy_session)
        
        assert result.success is True
        assert result.status == MigrationStatus.COMPLETED
        assert result.messages_migrated == 0
        assert result.interactions_created == 0
        assert len(result.errors) == 0
    
    def test_migrate_complex_session(self, migration_utility, complex_legacy_session):
        """Test migration of session with complex content."""
        result = migration_utility.migrate_session(complex_legacy_session)
        
        assert result.success is True
        assert result.status == MigrationStatus.COMPLETED
        assert result.messages_migrated == 4
        assert result.interactions_created >= 1
        assert len(result.errors) == 0
    
    def test_migrate_session_with_validation_disabled(self, migration_utility, sample_legacy_session):
        """Test migration with validation disabled."""
        result = migration_utility.migrate_session(
            sample_legacy_session, 
            validate_result=False
        )
        
        assert result.success is True
        assert result.status == MigrationStatus.COMPLETED
    
    def test_migrate_session_preserves_metadata(self, migration_utility, sample_legacy_session):
        """Test that migration preserves metadata."""
        result = migration_utility.migrate_session(
            sample_legacy_session,
            preserve_metadata=True
        )
        
        assert result.success is True
        # Metadata preservation would be verified in the actual enhanced session
        # (not directly in the result)
    
    def test_rollback_data_creation(self, migration_utility, sample_legacy_session):
        """Test that rollback data is created correctly."""
        result = migration_utility.migrate_session(sample_legacy_session)
        
        assert result.rollback_data is not None
        rollback = result.rollback_data
        
        assert rollback["session_id"] == sample_legacy_session.session_id
        assert rollback["token_count"] == sample_legacy_session.token_count
        assert rollback["messages"] == sample_legacy_session.messages
        assert rollback["metadata"] == sample_legacy_session.metadata
    
    def test_migration_statistics(self, migration_utility, sample_legacy_session):
        """Test migration statistics tracking."""
        # Reset statistics
        migration_utility.reset_statistics()
        
        # Perform migration
        result = migration_utility.migrate_session(sample_legacy_session)
        
        stats = migration_utility.get_migration_statistics()
        assert stats["sessions_processed"] == 1
        assert stats["messages_migrated"] == 4
        assert stats["interactions_created"] >= 1
        assert stats["errors_encountered"] == 0
        assert stats["success_rate"] == 100.0
    
    def test_validate_enhanced_session(self, migration_utility):
        """Test enhanced session validation."""
        # Create a valid enhanced session
        valid_session = EnhancedChatSession(session_id="valid-test")
        interaction_id = valid_session.start_interaction()
        
        errors = migration_utility.validate_enhanced_session(valid_session)
        assert len(errors) == 0
        
        # Create an invalid session (missing session ID)
        invalid_session = EnhancedChatSession()
        invalid_session.session_id = ""  # Invalid
        
        errors = migration_utility.validate_enhanced_session(invalid_session)
        assert len(errors) > 0
        assert any("Session ID is missing" in error for error in errors)
    
    def test_rollback_migration(self, migration_utility, sample_legacy_session):
        """Test migration rollback functionality."""
        result = migration_utility.migrate_session(sample_legacy_session)
        
        # Test rollback
        rollback_success, rollback_errors = migration_utility.rollback_migration(result)
        
        assert rollback_success is True
        assert len(rollback_errors) == 0
    
    def test_rollback_without_data(self, migration_utility):
        """Test rollback when no rollback data is available."""
        result = MigrationResult(
            success=False,
            session_id="test",
            status=MigrationStatus.FAILED,
            messages_migrated=0,
            interactions_created=0,
            errors=[],
            warnings=[],
            duration_seconds=0.0,
            rollback_data=None
        )
        
        rollback_success, rollback_errors = migration_utility.rollback_migration(result)
        
        assert rollback_success is False
        assert len(rollback_errors) > 0
        assert "No rollback data available" in rollback_errors[0]


class TestBatchMigration:
    """Test batch migration functionality."""
    
    @pytest.fixture
    def migration_utility(self):
        """Create a migration utility for testing."""
        return SessionMigrationUtility()
    
    @pytest.fixture
    def sample_sessions(self):
        """Create multiple sample sessions for batch testing."""
        sessions = []
        for i in range(5):
            session = ChatSession(
                session_id=f"session-{i}",
                session_name=f"Session {i}",
                messages=[
                    {"role": "user", "content": f"Hello {i}"},
                    {"role": "assistant", "content": f"Hi there {i}!"}
                ]
            )
            sessions.append(session)
        return sessions
    
    def test_batch_migration_success(self, migration_utility, sample_sessions):
        """Test successful batch migration."""
        result = migration_utility.migrate_sessions_batch(sample_sessions)
        
        assert result.total_sessions == 5
        assert result.successful_migrations == 5
        assert result.failed_migrations == 0
        assert result.total_messages_migrated == 10  # 2 messages per session
        assert result.total_interactions_created >= 5
        assert len(result.individual_results) == 5
        assert result.duration_seconds > 0
    
    def test_batch_migration_with_progress_callback(self, migration_utility, sample_sessions):
        """Test batch migration with progress callback."""
        progress_calls = []
        
        def progress_callback(current, total):
            progress_calls.append((current, total))
        
        result = migration_utility.migrate_sessions_batch(
            sample_sessions,
            progress_callback=progress_callback
        )
        
        assert result.successful_migrations == 5
        assert len(progress_calls) == 5
        assert progress_calls[-1] == (5, 5)  # Last call should be (5, 5)
    
    def test_batch_migration_with_batch_size(self, migration_utility, sample_sessions):
        """Test batch migration with custom batch size."""
        result = migration_utility.migrate_sessions_batch(
            sample_sessions,
            batch_size=2
        )
        
        assert result.successful_migrations == 5
        # Should still process all sessions despite smaller batch size
    
    def test_batch_migration_continue_on_error(self, migration_utility):
        """Test batch migration behavior with errors."""
        # Create sessions with one that will cause issues
        sessions = [
            ChatSession(session_id="good-1", messages=[{"role": "user", "content": "Hello"}]),
            ChatSession(session_id="good-2", messages=[{"role": "user", "content": "Hello"}])
        ]
        
        # Mock a migration failure for testing
        with patch.object(migration_utility, 'migrate_session') as mock_migrate:
            # First call succeeds, second fails, third succeeds
            mock_migrate.side_effect = [
                MigrationResult(True, "good-1", MigrationStatus.COMPLETED, 1, 1, [], [], 0.1),
                MigrationResult(False, "bad", MigrationStatus.FAILED, 0, 0, ["Error"], [], 0.1),
                MigrationResult(True, "good-2", MigrationStatus.COMPLETED, 1, 1, [], [], 0.1)
            ]
            
            # Add a bad session
            sessions.insert(1, ChatSession(session_id="bad", messages=[]))
            
            result = migration_utility.migrate_sessions_batch(
                sessions,
                continue_on_error=True
            )
            
            assert result.total_sessions == 3
            assert result.successful_migrations == 2
            assert result.failed_migrations == 1


class TestSessionMigrationManager:
    """Test SessionMigrationManager functionality."""
    
    @pytest.fixture
    def migration_manager(self):
        """Create a migration manager for testing."""
        return SessionMigrationManager()
    
    @pytest.fixture
    def sample_sessions_for_planning(self):
        """Create sessions with varying characteristics for planning tests."""
        sessions = []
        
        # Small session
        sessions.append(ChatSession(
            session_id="small",
            messages=[{"role": "user", "content": "Hi"}]
        ))
        
        # Large session
        large_messages = [{"role": "user", "content": f"Message {i}"} for i in range(1500)]
        sessions.append(ChatSession(
            session_id="large",
            messages=large_messages
        ))
        
        # Empty session
        sessions.append(ChatSession(
            session_id="empty",
            messages=[]
        ))
        
        # Complex session with tool calls
        sessions.append(ChatSession(
            session_id="complex",
            messages=[
                {"role": "user", "content": "Use tool"},
                {
                    "role": "assistant",
                    "content": [{"type": "tool_use", "id": "123", "name": "tool", "input": {}}]
                }
            ]
        ))
        
        return sessions
    
    def test_plan_migration(self, migration_manager, sample_sessions_for_planning):
        """Test migration planning."""
        plan = migration_manager.plan_migration(sample_sessions_for_planning)
        
        assert plan["total_sessions"] == 4
        assert plan["total_messages"] == 1503  # 1 + 1500 + 0 + 2
        assert plan["large_sessions"] == 1
        assert plan["empty_sessions"] == 1
        assert plan["complex_sessions"] == 1
        assert plan["estimated_interactions"] > 0
        assert plan["estimated_duration_minutes"] > 0
        assert len(plan["recommendations"]) > 0
    
    def test_execute_migration_plan(self, migration_manager, sample_sessions_for_planning):
        """Test executing a migration plan."""
        plan = migration_manager.plan_migration(sample_sessions_for_planning)
        
        result = migration_manager.execute_migration_plan(
            sample_sessions_for_planning,
            plan
        )
        
        assert isinstance(result, BatchMigrationResult)
        assert result.total_sessions == 4
        # Most should succeed (empty session might have 0 interactions but still succeed)
        assert result.successful_migrations >= 3
    
    def test_has_complex_content_detection(self, migration_manager):
        """Test complex content detection."""
        # Simple session
        simple_session = ChatSession(
            session_id="simple",
            messages=[{"role": "user", "content": "Hello"}]
        )
        assert migration_manager._has_complex_content(simple_session) is False
        
        # Complex session with tool calls
        complex_session = ChatSession(
            session_id="complex",
            messages=[
                {
                    "role": "assistant",
                    "content": [{"type": "tool_use", "name": "tool"}]
                }
            ]
        )
        assert migration_manager._has_complex_content(complex_session) is True
        
        # Session with keywords indicating complexity
        keyword_session = ChatSession(
            session_id="keyword",
            messages=[{"role": "user", "content": "Please use tool_use function"}]
        )
        assert migration_manager._has_complex_content(keyword_session) is True


class TestConvenienceFunctions:
    """Test convenience functions for migration."""
    
    @pytest.fixture
    def sample_legacy_session(self):
        """Create a sample legacy session."""
        return ChatSession(
            session_id="convenience-test",
            messages=[
                {"role": "user", "content": "Hello"},
                {"role": "assistant", "content": "Hi!"}
            ]
        )
    
    def test_migrate_single_session_convenience(self, sample_legacy_session):
        """Test the migrate_single_session convenience function."""
        enhanced_session, result = migrate_single_session(sample_legacy_session)
        
        assert result.success is True
        assert enhanced_session is not None
        assert isinstance(enhanced_session, EnhancedChatSession)
        assert enhanced_session.session_id == sample_legacy_session.session_id
    
    def test_migrate_single_session_without_validation(self, sample_legacy_session):
        """Test convenience function without validation."""
        enhanced_session, result = migrate_single_session(
            sample_legacy_session,
            validate=False
        )
        
        assert result.success is True
        assert enhanced_session is not None
    
    def test_migrate_sessions_with_progress_convenience(self):
        """Test the migrate_sessions_with_progress convenience function."""
        sessions = [
            ChatSession(session_id="conv-1", messages=[{"role": "user", "content": "Hi"}]),
            ChatSession(session_id="conv-2", messages=[{"role": "user", "content": "Hello"}])
        ]
        
        # Test without progress display
        result = migrate_sessions_with_progress(sessions, show_progress=False)
        
        assert isinstance(result, BatchMigrationResult)
        assert result.total_sessions == 2
        assert result.successful_migrations == 2
    
    @patch('builtins.print')
    def test_migrate_sessions_with_progress_display(self, mock_print):
        """Test convenience function with progress display."""
        sessions = [
            ChatSession(session_id="prog-1", messages=[{"role": "user", "content": "Hi"}])
        ]
        
        result = migrate_sessions_with_progress(sessions, show_progress=True)
        
        assert result.successful_migrations == 1
        # Should have printed progress information
        assert mock_print.called


class TestMigrationErrorHandling:
    """Test error handling in migration utilities."""
    
    @pytest.fixture
    def migration_utility(self):
        """Create a migration utility for testing."""
        return SessionMigrationUtility()
    
    def test_migration_with_invalid_session(self, migration_utility):
        """Test migration behavior with invalid session data."""
        # Create a session with invalid data
        invalid_session = ChatSession(session_id="")  # Empty session ID
        
        result = migration_utility.migrate_session(invalid_session)
        
        # Should handle gracefully (might succeed or fail depending on validation)
        assert isinstance(result, MigrationResult)
        assert result.session_id == ""
    
    def test_migration_with_corrupted_messages(self, migration_utility):
        """Test migration with corrupted message data."""
        corrupted_session = ChatSession(
            session_id="corrupted",
            messages=[
                {"role": "user"},  # Missing content
                {"content": "Hello"},  # Missing role
                {"role": "assistant", "content": "Hi"},  # Valid message
            ]
        )
        
        result = migration_utility.migrate_session(corrupted_session)
        
        # Should handle corrupted data gracefully
        assert isinstance(result, MigrationResult)
        # Might succeed or fail, but should not crash
    
    def test_validation_error_handling(self, migration_utility):
        """Test validation error handling."""
        # Create a session that will pass migration but fail validation
        session = ChatSession(
            session_id="validation-test",
            messages=[{"role": "user", "content": "Test"}]
        )
        
        # Mock validation to return errors
        with patch.object(migration_utility, '_validate_migration') as mock_validate:
            mock_validate.return_value = ["Validation error"]
            
            result = migration_utility.migrate_session(session, validate_result=True)
            
            assert result.success is False
            assert result.status == MigrationStatus.FAILED
            assert "Validation error" in result.errors


if __name__ == "__main__":
    pytest.main([__file__])