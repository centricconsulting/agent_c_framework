from datetime import datetime
import json
from typing import Dict, List, Any, Optional
import structlog

from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.core.repositories.session_repository import SessionRepository
from agent_c_api.api.v2.models.session_models import SessionCreate, SessionUpdate

class SessionMigration:
    """Helper class for migrating sessions to Redis storage"""
    
    def __init__(self, agent_manager: UItoAgentBridgeManager, session_repository: SessionRepository):
        """Initialize the session migration helper
        
        Args:
            agent_manager: The agent manager instance
            session_repository: The session repository instance
        """
        self.agent_manager = agent_manager
        self.session_repository = session_repository
        self.logger = structlog.get_logger(__name__)
    
    async def migrate_session(self, session_id: str) -> bool:
        """Migrate a single session to Redis
        
        Args:
            session_id: ID of the session to migrate
            
        Returns:
            bool: True if successful, False otherwise
        """
        try:
            # Get session data from agent manager
            session_data = await self.agent_manager.get_session_data(session_id)
            if not session_data:
                self.logger.warning("migrate_session_not_found", session_id=session_id)
                return False
            
            # Extract agent_c_session_id
            agent_c_session_id = session_data.get("agent_c_session_id", "")
            
            # Create session data model
            session_create = SessionCreate(
                id=session_id,
                model_id=session_data.get("model_name", ""),
                persona_id=session_data.get("persona_name", "default"),
                temperature=session_data.get("temperature"),
                reasoning_effort=session_data.get("reasoning_effort"),
                budget_tokens=session_data.get("budget_tokens"),
                max_tokens=session_data.get("max_tokens"),
                tools=session_data.get("additional_tools", []),
                custom_prompt=session_data.get("custom_prompt")
            )
            
            # Create session in Redis
            session = await self.session_repository.create_session(session_create)
            if not session:
                self.logger.error("migrate_session_create_failed", session_id=session_id)
                return False
            
            # Update with agent_internal_id if available
            if agent_c_session_id:
                await self.session_repository.update_session(
                    session_id,
                    SessionUpdate(agent_internal_id=agent_c_session_id)
                )
            
            self.logger.info("migrate_session_success", session_id=session_id)
            return True
            
        except Exception as e:
            self.logger.error("migrate_session_failed", session_id=session_id, error=str(e))
            return False
    
    async def migrate_all_sessions(self) -> Dict[str, Any]:
        """Migrate all sessions to Redis
        
        Returns:
            Dict containing migration statistics
        """
        stats = {
            "total": 0,
            "success": 0,
            "failed": 0,
            "failed_sessions": []
        }
        
        try:
            # Get all sessions from agent manager
            sessions = self.agent_manager.ui_sessions
            stats["total"] = len(sessions)
            
            # Migrate each session
            for session_id in sessions:
                success = await self.migrate_session(session_id)
                if success:
                    stats["success"] += 1
                else:
                    stats["failed"] += 1
                    stats["failed_sessions"].append(session_id)
            
            self.logger.info("migrate_all_sessions_complete", 
                           total=stats["total"],
                           success=stats["success"],
                           failed=stats["failed"])
            
            return stats
            
        except Exception as e:
            self.logger.error("migrate_all_sessions_failed", error=str(e))
            return stats
    
    async def validate_migration(self, session_id: str) -> Dict[str, Any]:
        """Validate a migrated session
        
        Args:
            session_id: ID of the session to validate
            
        Returns:
            Dict containing validation results
        """
        results = {
            "session_id": session_id,
            "exists_in_manager": False,
            "exists_in_redis": False,
            "data_matches": False,
            "differences": {}
        }
        
        try:
            # Check agent manager
            manager_data = await self.agent_manager.get_session_data(session_id)
            results["exists_in_manager"] = manager_data is not None
            
            # Check Redis
            redis_session = await self.session_repository.get_session(session_id)
            results["exists_in_redis"] = redis_session is not None
            
            # Compare data if both exist
            if results["exists_in_manager"] and results["exists_in_redis"]:
                # Compare key fields
                fields_to_compare = [
                    ("model_id", "model_name"),
                    ("persona_id", "persona_name"),
                    ("temperature", "temperature"),
                    ("reasoning_effort", "reasoning_effort"),
                    ("budget_tokens", "budget_tokens"),
                    ("max_tokens", "max_tokens"),
                    ("tools", "additional_tools"),
                    ("custom_prompt", "custom_prompt"),
                    ("agent_internal_id", "agent_c_session_id")
                ]
                
                all_match = True
                for redis_field, manager_field in fields_to_compare:
                    redis_value = getattr(redis_session, redis_field, None)
                    manager_value = manager_data.get(manager_field)
                    
                    if redis_value != manager_value:
                        all_match = False
                        results["differences"][redis_field] = {
                            "redis": redis_value,
                            "manager": manager_value
                        }
                
                results["data_matches"] = all_match
            
            return results
            
        except Exception as e:
            self.logger.error("validate_migration_failed", session_id=session_id, error=str(e))
            results["error"] = str(e)
            return results
    
    async def validate_all_migrations(self) -> Dict[str, Any]:
        """Validate all migrated sessions
        
        Returns:
            Dict containing validation statistics and results
        """
        stats = {
            "total": 0,
            "fully_matched": 0,
            "partially_matched": 0,
            "missing": 0,
            "detailed_results": []
        }
        
        try:
            # Get all sessions from agent manager
            sessions = self.agent_manager.ui_sessions
            stats["total"] = len(sessions)
            
            # Validate each session
            for session_id in sessions:
                results = await self.validate_migration(session_id)
                stats["detailed_results"].append(results)
                
                if not results["exists_in_redis"]:
                    stats["missing"] += 1
                elif results["data_matches"]:
                    stats["fully_matched"] += 1
                else:
                    stats["partially_matched"] += 1
            
            self.logger.info("validate_all_migrations_complete", 
                           total=stats["total"],
                           fully_matched=stats["fully_matched"],
                           partially_matched=stats["partially_matched"],
                           missing=stats["missing"])
            
            return stats
            
        except Exception as e:
            self.logger.error("validate_all_migrations_failed", error=str(e))
            return stats