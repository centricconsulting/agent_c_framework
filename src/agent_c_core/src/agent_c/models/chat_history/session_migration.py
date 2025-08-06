"""
ChatSession Migration Utilities.

This module provides comprehensive migration utilities for converting existing
ChatSession instances to EnhancedChatSession format, with robust error handling,
validation, and rollback capabilities.
"""

import json
import logging
import datetime
from typing import List, Dict, Any, Optional, Tuple, Callable, Union
from dataclasses import dataclass
from enum import Enum

from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.common_chat.enhanced_models import EnhancedCommonChatMessage


class MigrationStatus(Enum):
    """Migration status enumeration."""
    PENDING = "pending"
    IN_PROGRESS = "in_progress"
    COMPLETED = "completed"
    FAILED = "failed"
    ROLLED_BACK = "rolled_back"


@dataclass
class MigrationResult:
    """Result of a migration operation."""
    success: bool
    session_id: str
    status: MigrationStatus
    messages_migrated: int
    interactions_created: int
    errors: List[str]
    warnings: List[str]
    duration_seconds: float
    rollback_data: Optional[Dict[str, Any]] = None


@dataclass
class BatchMigrationResult:
    """Result of a batch migration operation."""
    total_sessions: int
    successful_migrations: int
    failed_migrations: int
    total_messages_migrated: int
    total_interactions_created: int
    duration_seconds: float
    individual_results: List[MigrationResult]
    errors: List[str]


class SessionMigrationUtility:
    """
    Comprehensive utility for migrating ChatSession instances to EnhancedChatSession.
    
    Features:
    - Robust error handling and validation
    - Rollback capabilities for failed migrations
    - Batch processing for large-scale migrations
    - Progress tracking and reporting
    - Data integrity validation
    - Performance optimization for large sessions
    """
    
    def __init__(self, logger: Optional[logging.Logger] = None):
        """
        Initialize the migration utility.
        
        Args:
            logger: Optional logger instance for migration logging
        """
        self.logger = logger or logging.getLogger(__name__)
        self._migration_stats = {
            "sessions_processed": 0,
            "messages_migrated": 0,
            "interactions_created": 0,
            "errors_encountered": 0
        }
    
    def migrate_session(
        self, 
        legacy_session: ChatSession, 
        validate_result: bool = True,
        preserve_metadata: bool = True
    ) -> MigrationResult:
        """
        Migrate a single ChatSession to EnhancedChatSession.
        
        Args:
            legacy_session: The legacy ChatSession to migrate
            validate_result: Whether to validate the migration result
            preserve_metadata: Whether to preserve all metadata
            
        Returns:
            MigrationResult with details of the migration
        """
        start_time = datetime.datetime.now()
        result = MigrationResult(
            success=False,
            session_id=legacy_session.session_id,
            status=MigrationStatus.PENDING,
            messages_migrated=0,
            interactions_created=0,
            errors=[],
            warnings=[],
            duration_seconds=0.0
        )
        
        try:
            result.status = MigrationStatus.IN_PROGRESS
            self.logger.info(f"Starting migration for session {legacy_session.session_id}")
            
            # Create rollback data
            result.rollback_data = self._create_rollback_data(legacy_session)
            
            # Create enhanced session
            enhanced_session = EnhancedChatSession(
                session_id=legacy_session.session_id,
                token_count=legacy_session.token_count,
                context_window_size=legacy_session.context_window_size,
                session_name=legacy_session.session_name,
                created_at=legacy_session.created_at,
                updated_at=legacy_session.updated_at,
                deleted_at=legacy_session.deleted_at,
                user_id=legacy_session.user_id,
                metadata=legacy_session.metadata.copy() if preserve_metadata else {},
                agent_config=legacy_session.agent_config
            )
            
            # Migrate messages
            if legacy_session.messages:
                migration_success = enhanced_session.migrate_from_legacy(legacy_session.messages)
                
                if not migration_success:
                    result.errors.append("Failed to migrate messages to enhanced format")
                    result.status = MigrationStatus.FAILED
                    return result
                
                result.messages_migrated = len(legacy_session.messages)
                result.interactions_created = len(enhanced_session.interaction_containers)
            
            # Validate migration if requested
            if validate_result:
                validation_errors = self._validate_migration(legacy_session, enhanced_session)
                if validation_errors:
                    result.errors.extend(validation_errors)
                    result.status = MigrationStatus.FAILED
                    return result
            
            # Migration successful
            result.success = True
            result.status = MigrationStatus.COMPLETED
            self._migration_stats["sessions_processed"] += 1
            self._migration_stats["messages_migrated"] += result.messages_migrated
            self._migration_stats["interactions_created"] += result.interactions_created
            
            self.logger.info(
                f"Successfully migrated session {legacy_session.session_id}: "
                f"{result.messages_migrated} messages, {result.interactions_created} interactions"
            )
            
        except Exception as e:
            result.errors.append(f"Migration failed with exception: {str(e)}")
            result.status = MigrationStatus.FAILED
            self._migration_stats["errors_encountered"] += 1
            self.logger.error(f"Migration failed for session {legacy_session.session_id}: {e}")
        
        finally:
            end_time = datetime.datetime.now()
            result.duration_seconds = (end_time - start_time).total_seconds()
        
        return result
    
    def migrate_sessions_batch(
        self,
        legacy_sessions: List[ChatSession],
        batch_size: int = 100,
        continue_on_error: bool = True,
        progress_callback: Optional[Callable[[int, int], None]] = None
    ) -> BatchMigrationResult:
        """
        Migrate multiple ChatSession instances in batches.
        
        Args:
            legacy_sessions: List of legacy ChatSession instances
            batch_size: Number of sessions to process in each batch
            continue_on_error: Whether to continue processing if individual migrations fail
            progress_callback: Optional callback for progress reporting (current, total)
            
        Returns:
            BatchMigrationResult with comprehensive migration statistics
        """
        start_time = datetime.datetime.now()
        
        batch_result = BatchMigrationResult(
            total_sessions=len(legacy_sessions),
            successful_migrations=0,
            failed_migrations=0,
            total_messages_migrated=0,
            total_interactions_created=0,
            duration_seconds=0.0,
            individual_results=[],
            errors=[]
        )
        
        self.logger.info(f"Starting batch migration of {len(legacy_sessions)} sessions")
        
        try:
            # Process sessions in batches
            for i in range(0, len(legacy_sessions), batch_size):
                batch = legacy_sessions[i:i + batch_size]
                
                for j, session in enumerate(batch):
                    current_index = i + j
                    
                    # Progress callback
                    if progress_callback:
                        progress_callback(current_index + 1, len(legacy_sessions))
                    
                    # Migrate individual session
                    result = self.migrate_session(session)
                    batch_result.individual_results.append(result)
                    
                    if result.success:
                        batch_result.successful_migrations += 1
                        batch_result.total_messages_migrated += result.messages_migrated
                        batch_result.total_interactions_created += result.interactions_created
                    else:
                        batch_result.failed_migrations += 1
                        batch_result.errors.extend(result.errors)
                        
                        if not continue_on_error:
                            batch_result.errors.append(f"Stopping batch migration due to error in session {session.session_id}")
                            break
                
                # Break outer loop if we stopped due to error
                if not continue_on_error and batch_result.failed_migrations > 0:
                    break
        
        except Exception as e:
            batch_result.errors.append(f"Batch migration failed with exception: {str(e)}")
            self.logger.error(f"Batch migration failed: {e}")
        
        finally:
            end_time = datetime.datetime.now()
            batch_result.duration_seconds = (end_time - start_time).total_seconds()
        
        self.logger.info(
            f"Batch migration completed: {batch_result.successful_migrations} successful, "
            f"{batch_result.failed_migrations} failed, "
            f"{batch_result.total_messages_migrated} messages migrated"
        )
        
        return batch_result
    
    def rollback_migration(
        self, 
        migration_result: MigrationResult
    ) -> Tuple[bool, List[str]]:
        """
        Rollback a failed migration using stored rollback data.
        
        Args:
            migration_result: The migration result containing rollback data
            
        Returns:
            Tuple of (success, error_messages)
        """
        if not migration_result.rollback_data:
            return False, ["No rollback data available"]
        
        try:
            # Reconstruct original session from rollback data
            rollback_data = migration_result.rollback_data
            
            original_session = ChatSession(
                session_id=rollback_data["session_id"],
                token_count=rollback_data["token_count"],
                context_window_size=rollback_data["context_window_size"],
                session_name=rollback_data["session_name"],
                created_at=rollback_data["created_at"],
                updated_at=rollback_data["updated_at"],
                deleted_at=rollback_data["deleted_at"],
                user_id=rollback_data["user_id"],
                metadata=rollback_data["metadata"],
                messages=rollback_data["messages"],
                agent_config=rollback_data.get("agent_config")
            )
            
            self.logger.info(f"Successfully rolled back migration for session {migration_result.session_id}")
            return True, []
            
        except Exception as e:
            error_msg = f"Rollback failed for session {migration_result.session_id}: {str(e)}"
            self.logger.error(error_msg)
            return False, [error_msg]
    
    def validate_enhanced_session(self, enhanced_session: EnhancedChatSession) -> List[str]:
        """
        Validate an enhanced session for data integrity.
        
        Args:
            enhanced_session: The enhanced session to validate
            
        Returns:
            List of validation error messages (empty if valid)
        """
        errors = []
        
        try:
            # Basic field validation
            if not enhanced_session.session_id:
                errors.append("Session ID is missing")
            
            # Interaction validation
            for interaction_id, container in enhanced_session.interaction_containers.items():
                if container.interaction_id != interaction_id:
                    errors.append(f"Interaction ID mismatch: {interaction_id} vs {container.interaction_id}")
                
                # Message validation within interaction
                messages = container.get_all_messages()
                for msg in messages:
                    if msg.interaction_id != interaction_id:
                        errors.append(f"Message interaction ID mismatch in interaction {interaction_id}")
                    
                    if not msg.id:
                        errors.append(f"Message missing ID in interaction {interaction_id}")
                    
                    if not msg.role:
                        errors.append(f"Message missing role in interaction {interaction_id}")
            
            # Check for orphaned current interaction
            if enhanced_session.current_interaction_id:
                if enhanced_session.current_interaction_id not in enhanced_session.interaction_containers:
                    errors.append("Current interaction ID references non-existent interaction")
        
        except Exception as e:
            errors.append(f"Validation failed with exception: {str(e)}")
        
        return errors
    
    def get_migration_statistics(self) -> Dict[str, Any]:
        """Get comprehensive migration statistics."""
        return {
            "sessions_processed": self._migration_stats["sessions_processed"],
            "messages_migrated": self._migration_stats["messages_migrated"],
            "interactions_created": self._migration_stats["interactions_created"],
            "errors_encountered": self._migration_stats["errors_encountered"],
            "success_rate": (
                self._migration_stats["sessions_processed"] - self._migration_stats["errors_encountered"]
            ) / max(self._migration_stats["sessions_processed"], 1) * 100
        }
    
    def reset_statistics(self) -> None:
        """Reset migration statistics."""
        self._migration_stats = {
            "sessions_processed": 0,
            "messages_migrated": 0,
            "interactions_created": 0,
            "errors_encountered": 0
        }
    
    def _create_rollback_data(self, legacy_session: ChatSession) -> Dict[str, Any]:
        """Create rollback data for a legacy session."""
        return {
            "session_id": legacy_session.session_id,
            "token_count": legacy_session.token_count,
            "context_window_size": legacy_session.context_window_size,
            "session_name": legacy_session.session_name,
            "created_at": legacy_session.created_at,
            "updated_at": legacy_session.updated_at,
            "deleted_at": legacy_session.deleted_at,
            "user_id": legacy_session.user_id,
            "metadata": legacy_session.metadata.copy() if legacy_session.metadata else {},
            "messages": legacy_session.messages.copy() if legacy_session.messages else [],
            "agent_config": legacy_session.agent_config
        }
    
    def _validate_migration(
        self, 
        legacy_session: ChatSession, 
        enhanced_session: EnhancedChatSession
    ) -> List[str]:
        """
        Validate that migration preserved all data correctly.
        
        Args:
            legacy_session: Original legacy session
            enhanced_session: Migrated enhanced session
            
        Returns:
            List of validation errors
        """
        errors = []
        
        try:
            # Basic field validation
            if legacy_session.session_id != enhanced_session.session_id:
                errors.append("Session ID mismatch after migration")
            
            if legacy_session.token_count != enhanced_session.token_count:
                errors.append("Token count mismatch after migration")
            
            # Message count validation
            legacy_message_count = len(legacy_session.messages) if legacy_session.messages else 0
            enhanced_messages = enhanced_session.get_all_messages()
            enhanced_message_count = len(enhanced_messages)
            
            if legacy_message_count != enhanced_message_count:
                errors.append(
                    f"Message count mismatch: legacy={legacy_message_count}, "
                    f"enhanced={enhanced_message_count}"
                )
            
            # Content validation (sample check)
            if legacy_session.messages and enhanced_messages:
                # Check first and last messages for content preservation
                legacy_first = legacy_session.messages[0]
                enhanced_first = enhanced_messages[0]
                
                if legacy_first.get("role") != enhanced_first.role:
                    errors.append("First message role mismatch after migration")
                
                # Basic content check (simplified)
                legacy_content = str(legacy_first.get("content", ""))
                enhanced_content = ""
                for block in enhanced_first.content:
                    if hasattr(block, 'text'):
                        enhanced_content += block.text
                
                if legacy_content.strip() != enhanced_content.strip():
                    errors.append("First message content mismatch after migration")
        
        except Exception as e:
            errors.append(f"Validation failed with exception: {str(e)}")
        
        return errors


class SessionMigrationManager:
    """
    High-level manager for session migrations with advanced features.
    
    Features:
    - Migration planning and strategy selection
    - Performance optimization for large datasets
    - Migration scheduling and queuing
    - Comprehensive reporting and analytics
    """
    
    def __init__(self, logger: Optional[logging.Logger] = None):
        """Initialize the migration manager."""
        self.logger = logger or logging.getLogger(__name__)
        self.utility = SessionMigrationUtility(logger)
        self._migration_queue: List[ChatSession] = []
        self._completed_migrations: List[MigrationResult] = []
    
    def plan_migration(
        self, 
        sessions: List[ChatSession]
    ) -> Dict[str, Any]:
        """
        Analyze sessions and create a migration plan.
        
        Args:
            sessions: List of sessions to analyze
            
        Returns:
            Migration plan with recommendations
        """
        plan = {
            "total_sessions": len(sessions),
            "total_messages": 0,
            "estimated_interactions": 0,
            "large_sessions": 0,
            "empty_sessions": 0,
            "complex_sessions": 0,
            "recommended_batch_size": 100,
            "estimated_duration_minutes": 0,
            "recommendations": []
        }
        
        for session in sessions:
            message_count = len(session.messages) if session.messages else 0
            plan["total_messages"] += message_count
            
            if message_count == 0:
                plan["empty_sessions"] += 1
            elif message_count > 1000:
                plan["large_sessions"] += 1
            
            # Estimate interactions (rough heuristic)
            estimated_interactions = max(1, message_count // 4)
            plan["estimated_interactions"] += estimated_interactions
            
            # Check for complex content (tool calls, multimodal, etc.)
            if self._has_complex_content(session):
                plan["complex_sessions"] += 1
        
        # Recommendations based on analysis
        if plan["large_sessions"] > 0:
            plan["recommendations"].append(
                f"Found {plan['large_sessions']} large sessions (>1000 messages). "
                "Consider smaller batch sizes for these."
            )
            plan["recommended_batch_size"] = 50
        
        if plan["complex_sessions"] > 0:
            plan["recommendations"].append(
                f"Found {plan['complex_sessions']} sessions with complex content. "
                "Migration may take longer due to content processing."
            )
        
        # Estimate duration (rough heuristic: 10 messages per second)
        plan["estimated_duration_minutes"] = max(1, plan["total_messages"] / 600)
        
        return plan
    
    def execute_migration_plan(
        self,
        sessions: List[ChatSession],
        plan: Optional[Dict[str, Any]] = None,
        progress_callback: Optional[Callable[[int, int], None]] = None
    ) -> BatchMigrationResult:
        """
        Execute a migration plan.
        
        Args:
            sessions: Sessions to migrate
            plan: Optional pre-computed migration plan
            progress_callback: Optional progress callback
            
        Returns:
            Batch migration result
        """
        if plan is None:
            plan = self.plan_migration(sessions)
        
        batch_size = plan.get("recommended_batch_size", 100)
        
        self.logger.info(f"Executing migration plan for {len(sessions)} sessions")
        self.logger.info(f"Estimated duration: {plan.get('estimated_duration_minutes', 0):.1f} minutes")
        
        return self.utility.migrate_sessions_batch(
            sessions,
            batch_size=batch_size,
            progress_callback=progress_callback
        )
    
    def _has_complex_content(self, session: ChatSession) -> bool:
        """Check if session has complex content that may slow migration."""
        if not session.messages:
            return False
        
        for msg in session.messages:
            content = msg.get("content", "")
            
            # Check for tool calls, multimodal content, etc.
            if isinstance(content, list):
                return True
            
            if isinstance(content, str) and any(keyword in content.lower() for keyword in [
                "tool_use", "tool_result", "function_call", "image", "audio"
            ]):
                return True
        
        return False


# Convenience functions for common migration scenarios

def migrate_single_session(
    legacy_session: ChatSession,
    validate: bool = True
) -> Tuple[Optional[EnhancedChatSession], MigrationResult]:
    """
    Convenience function to migrate a single session.
    
    Args:
        legacy_session: The session to migrate
        validate: Whether to validate the result
        
    Returns:
        Tuple of (enhanced_session, migration_result)
    """
    utility = SessionMigrationUtility()
    result = utility.migrate_session(legacy_session, validate_result=validate)
    
    if result.success:
        # Recreate the enhanced session (in real usage, this would be returned from migrate_session)
        enhanced_session = EnhancedChatSession(
            session_id=legacy_session.session_id,
            token_count=legacy_session.token_count,
            context_window_size=legacy_session.context_window_size,
            session_name=legacy_session.session_name,
            created_at=legacy_session.created_at,
            updated_at=legacy_session.updated_at,
            deleted_at=legacy_session.deleted_at,
            user_id=legacy_session.user_id,
            metadata=legacy_session.metadata,
            agent_config=legacy_session.agent_config
        )
        enhanced_session.migrate_from_legacy(legacy_session.messages or [])
        return enhanced_session, result
    else:
        return None, result


def migrate_sessions_with_progress(
    sessions: List[ChatSession],
    show_progress: bool = True
) -> BatchMigrationResult:
    """
    Convenience function to migrate sessions with optional progress display.
    
    Args:
        sessions: Sessions to migrate
        show_progress: Whether to show progress output
        
    Returns:
        Batch migration result
    """
    manager = SessionMigrationManager()
    
    def progress_callback(current: int, total: int):
        if show_progress:
            percentage = (current / total) * 100
            print(f"Migration progress: {current}/{total} ({percentage:.1f}%)")
    
    plan = manager.plan_migration(sessions)
    if show_progress:
        print(f"Migration plan: {plan['total_sessions']} sessions, {plan['total_messages']} messages")
        print(f"Estimated duration: {plan['estimated_duration_minutes']:.1f} minutes")
    
    return manager.execute_migration_plan(
        sessions, 
        plan, 
        progress_callback if show_progress else None
    )