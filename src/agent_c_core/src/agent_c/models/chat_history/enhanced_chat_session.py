"""
Enhanced ChatSession with InteractionContainer integration.

This module provides an enhanced version of ChatSession that uses InteractionContainer
for message management instead of bare message arrays, enabling advanced features like
tool optimization, work log generation, and interaction tracking.
"""

import datetime
import uuid
import warnings
from typing import Optional, Dict, Any, List, Union
from pydantic import Field, field_validator

from agent_c.models.base import BaseModel
from agent_c.models.agent_config import AgentConfiguration
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.common_chat.enhanced_models import EnhancedCommonChatMessage
from agent_c.util.slugs import MnemonicSlugs


class EnhancedChatSession(BaseModel):
    """
    Enhanced ChatSession that uses InteractionContainer for intelligent message management.
    
    This class replaces bare message arrays with InteractionContainer instances,
    enabling advanced features like:
    - Tool-driven message optimization
    - Automatic work log generation
    - Interaction boundary tracking
    - Message validity and lifecycle management
    - Observable pattern integration for UI updates
    """
    
    session_id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3))
    token_count: int = Field(0, description="The number of tokens used in the session")
    context_window_size: int = Field(0, description="The number of tokens in the context window")
    session_name: Optional[str] = Field(None, description="The name of the session, if any")
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    updated_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    deleted_at: Optional[str] = Field(None, description="Timestamp when the session was deleted")
    user_id: Optional[str] = Field("Agent C user", description="The user ID associated with the session")
    metadata: Optional[Dict[str, Any]] = Field(default_factory=dict, description="Metadata associated with the session")
    agent_config: Optional[AgentConfiguration] = Field(None, description="Configuration for the agent associated with the session")
    
    # Enhanced message management with InteractionContainer
    interaction_containers: Dict[str, InteractionContainer] = Field(
        default_factory=dict, 
        description="Dictionary of interaction containers indexed by interaction ID"
    )
    current_interaction_id: Optional[str] = Field(
        None, 
        description="ID of the currently active interaction"
    )
    enable_work_logs: bool = Field(
        True, 
        description="Enable automatic work log generation"
    )
    enable_tool_optimization: bool = Field(
        True, 
        description="Enable tool-driven message optimization"
    )
    
    # Backward compatibility fields
    legacy_messages: Optional[List[dict[str, Any]]] = Field(
        None, 
        description="Legacy message array for backward compatibility",
        alias="messages"
    )
    migration_status: str = Field(
        "enhanced", 
        description="Migration status: 'legacy', 'migrating', 'enhanced'"
    )

    def touch(self) -> None:
        """Updates the updated_at timestamp to the current time."""
        self.updated_at = datetime.datetime.now().isoformat()
    
    # Interaction Management Methods
    
    def start_interaction(self, interaction_id: Optional[str] = None) -> str:
        """
        Start a new interaction in this session.
        
        Args:
            interaction_id: Optional custom interaction ID
            
        Returns:
            The interaction ID of the started interaction
        """
        if interaction_id is None:
            interaction_id = str(uuid.uuid4())
        
        container = InteractionContainer(
            interaction_id=interaction_id,
            interaction_start=datetime.datetime.now().timestamp()
        )
        
        self.interaction_containers[interaction_id] = container
        self.current_interaction_id = interaction_id
        self.touch()
        
        return interaction_id
    
    def end_interaction(self, interaction_id: Optional[str] = None) -> None:
        """
        End the specified or current interaction.
        
        Args:
            interaction_id: Optional interaction ID to end (defaults to current)
        """
        target_id = interaction_id or self.current_interaction_id
        if target_id is None:
            return
        
        container = self.interaction_containers.get(target_id)
        if container is not None:
            container.complete_interaction()
            
            # Generate work log entries if enabled
            if self.enable_work_logs:
                try:
                    container.generate_work_log_entries()
                except Exception as e:
                    # Log error but don't fail the operation
                    pass
        
        # Clear current interaction if it's the one being ended
        if target_id == self.current_interaction_id:
            self.current_interaction_id = None
        
        self.touch()
    
    def get_current_interaction(self) -> Optional[InteractionContainer]:
        """Get the current interaction container."""
        if self.current_interaction_id is None:
            return None
        return self.interaction_containers.get(self.current_interaction_id)
    
    def get_interaction(self, interaction_id: str) -> Optional[InteractionContainer]:
        """Get a specific interaction container by ID."""
        return self.interaction_containers.get(interaction_id)
    
    def add_message_to_current_interaction(self, message: EnhancedCommonChatMessage) -> bool:
        """
        Add a message to the current interaction.
        
        Args:
            message: The enhanced message to add
            
        Returns:
            True if successful, False otherwise
        """
        current = self.get_current_interaction()
        if current is None:
            # Auto-start an interaction if none exists
            self.start_interaction()
            current = self.get_current_interaction()
        
        if current is not None:
            current.add_message(message)
            self.touch()
            return True
        
        return False
    
    # Message Access Methods (Enhanced)
    
    def get_all_messages(self, include_invalidated: bool = False) -> List[EnhancedCommonChatMessage]:
        """
        Get all messages from all interactions in chronological order.
        
        Args:
            include_invalidated: Whether to include invalidated messages
            
        Returns:
            List of all messages across all interactions
        """
        all_messages = []
        
        # Sort interactions by start time
        sorted_containers = sorted(
            self.interaction_containers.values(),
            key=lambda c: c.interaction_start
        )
        
        for container in sorted_containers:
            if include_invalidated:
                all_messages.extend(container.get_all_messages())
            else:
                all_messages.extend(container.get_active_messages())
        
        return all_messages
    
    def get_messages_for_llm(self) -> List[dict[str, Any]]:
        """
        Get messages in LLM-compatible format (dict format).
        
        This method converts EnhancedCommonChatMessage instances back to
        the dict format expected by LLM APIs for backward compatibility.
        
        Returns:
            List of messages in dict format
        """
        enhanced_messages = self.get_all_messages(include_invalidated=False)
        
        # Convert enhanced messages to dict format
        dict_messages = []
        for msg in enhanced_messages:
            # Simple conversion - extract text content
            content = ""
            if msg.content:
                for block in msg.content:
                    if hasattr(block, 'text'):
                        content += block.text
                    elif hasattr(block, 'content'):
                        content += str(block.content)
            
            dict_messages.append({
                "role": msg.role,
                "content": content
            })
        
        return dict_messages
    
    # Backward Compatibility Methods
    
    @property
    def messages(self) -> List[dict[str, Any]]:
        """
        Backward compatibility property for accessing messages.
        
        Returns messages in the legacy dict format for existing code.
        """
        if self.migration_status == "legacy" and self.legacy_messages is not None:
            return self.legacy_messages
        
        return self.get_messages_for_llm()
    
    @messages.setter
    def messages(self, value: List[dict[str, Any]]) -> None:
        """
        Backward compatibility setter for messages.
        
        When legacy code sets messages, we convert them to enhanced format.
        """
        if value is None:
            return
        
        # Clear existing interactions
        self.interaction_containers.clear()
        self.current_interaction_id = None
        
        if len(value) == 0:
            return
        
        # Convert legacy messages to enhanced format
        interaction_id = self.start_interaction()
        container = self.get_current_interaction()
        
        for msg_dict in value:
            try:
                # Convert dict message to enhanced message
                enhanced_msg = self._convert_dict_to_enhanced_message(msg_dict, interaction_id)
                container.add_message(enhanced_msg)
            except Exception as e:
                # If conversion fails, store as legacy
                self.legacy_messages = value
                self.migration_status = "legacy"
                return
        
        self.migration_status = "enhanced"
        self.touch()
    
    def _convert_dict_to_enhanced_message(
        self, 
        msg_dict: dict[str, Any], 
        interaction_id: str
    ) -> EnhancedCommonChatMessage:
        """
        Convert a legacy dict message to an EnhancedCommonChatMessage.
        
        Args:
            msg_dict: Legacy message dictionary
            interaction_id: Interaction ID to assign
            
        Returns:
            Enhanced message instance
        """
        from agent_c.models.common_chat.enhanced_models import (
            EnhancedCommonChatMessage,
            EnhancedTextContentBlock,
            EnhancedToolUseContentBlock,
            EnhancedToolResultContentBlock
        )
        
        role = msg_dict.get("role", "user")
        content = msg_dict.get("content", "")
        
        # Handle different content types
        content_blocks = []
        
        if isinstance(content, str):
            # Simple text content
            content_blocks = [EnhancedTextContentBlock(text=content)]
        elif isinstance(content, list):
            # Complex content with multiple blocks
            for block in content:
                if isinstance(block, dict):
                    block_type = block.get("type", "text")
                    if block_type == "text":
                        content_blocks.append(EnhancedTextContentBlock(text=block.get("text", "")))
                    elif block_type == "tool_use":
                        content_blocks.append(EnhancedToolUseContentBlock(
                            id=block.get("id", str(uuid.uuid4())),
                            name=block.get("name", ""),
                            input=block.get("input", {})
                        ))
                    elif block_type == "tool_result":
                        content_blocks.append(EnhancedToolResultContentBlock(
                            tool_use_id=block.get("tool_use_id", ""),
                            content=block.get("content", "")
                        ))
                    else:
                        # Unknown block type, convert to text
                        content_blocks.append(EnhancedTextContentBlock(text=str(block)))
                else:
                    # Non-dict content, convert to text
                    content_blocks.append(EnhancedTextContentBlock(text=str(block)))
        else:
            # Other content types, convert to text
            content_blocks = [EnhancedTextContentBlock(text=str(content))]
        
        return EnhancedCommonChatMessage(
            id=msg_dict.get("id", str(uuid.uuid4())),
            role=role,
            content=content_blocks,
            interaction_id=interaction_id,
            created_at=datetime.datetime.now().timestamp()
        )
    
    # Migration and Utility Methods
    
    def migrate_from_legacy(self, legacy_messages: List[dict[str, Any]]) -> bool:
        """
        Migrate from legacy message format to enhanced format.
        
        Args:
            legacy_messages: List of legacy message dictionaries
            
        Returns:
            True if migration successful, False otherwise
        """
        try:
            self.migration_status = "migrating"
            
            # Clear existing data
            self.interaction_containers.clear()
            self.current_interaction_id = None
            
            if not legacy_messages:
                self.migration_status = "enhanced"
                return True
            
            # Group messages into interactions based on conversation flow
            interactions = self._group_messages_into_interactions(legacy_messages)
            
            for interaction_msgs in interactions:
                interaction_id = self.start_interaction()
                container = self.get_current_interaction()
                
                for msg_dict in interaction_msgs:
                    enhanced_msg = self._convert_dict_to_enhanced_message(msg_dict, interaction_id)
                    container.add_message(enhanced_msg)
                
                self.end_interaction(interaction_id)
            
            self.migration_status = "enhanced"
            self.legacy_messages = None
            self.touch()
            return True
            
        except Exception as e:
            # Migration failed, revert to legacy
            self.migration_status = "legacy"
            self.legacy_messages = legacy_messages
            return False
    
    def _group_messages_into_interactions(
        self, 
        messages: List[dict[str, Any]]
    ) -> List[List[dict[str, Any]]]:
        """
        Group legacy messages into logical interactions.
        
        An interaction typically consists of:
        1. User message
        2. Optional tool calls and results
        3. Assistant response
        
        Args:
            messages: List of legacy message dictionaries
            
        Returns:
            List of interaction groups (each group is a list of messages)
        """
        if not messages:
            return []
        
        interactions = []
        current_interaction = []
        
        for msg in messages:
            role = msg.get("role", "")
            
            # Start new interaction on user message (unless it's the first message)
            if role == "user" and current_interaction:
                # Save current interaction and start new one
                interactions.append(current_interaction)
                current_interaction = [msg]
            else:
                current_interaction.append(msg)
        
        # Add the last interaction if it has messages
        if current_interaction:
            interactions.append(current_interaction)
        
        return interactions
    
    def export_session_data(self, include_work_logs: bool = True) -> Dict[str, Any]:
        """
        Export complete session data including interactions and work logs.
        
        Args:
            include_work_logs: Whether to include work log data
            
        Returns:
            Complete session data dictionary
        """
        data = {
            "session_id": self.session_id,
            "session_name": self.session_name,
            "created_at": self.created_at,
            "updated_at": self.updated_at,
            "user_id": self.user_id,
            "metadata": self.metadata,
            "token_count": self.token_count,
            "context_window_size": self.context_window_size,
            "migration_status": self.migration_status,
            "interactions": {}
        }
        
        # Export interaction containers
        for interaction_id, container in self.interaction_containers.items():
            interaction_data = {
                "interaction_id": interaction_id,
                "interaction_start": container.interaction_start,
                "interaction_stop": container.interaction_stop,
                "validity_state": container.validity_state.value if container.validity_state else None,
                "messages": [msg.model_dump() for msg in container.get_all_messages()],
                "optimization_metadata": container.optimization_metadata
            }
            
            if include_work_logs:
                interaction_data["work_log_entries"] = [
                    entry_id for entry_id in container.work_log_entries
                ]
            
            data["interactions"][interaction_id] = interaction_data
        
        return data
    
    def import_session_data(self, data: Dict[str, Any]) -> bool:
        """
        Import session data from exported format.
        
        Args:
            data: Session data dictionary from export_session_data
            
        Returns:
            True if import successful, False otherwise
        """
        try:
            # Import basic session fields
            self.session_id = data.get("session_id", self.session_id)
            self.session_name = data.get("session_name", self.session_name)
            self.created_at = data.get("created_at", self.created_at)
            self.updated_at = data.get("updated_at", self.updated_at)
            self.user_id = data.get("user_id", self.user_id)
            self.metadata = data.get("metadata", {})
            self.token_count = data.get("token_count", 0)
            self.context_window_size = data.get("context_window_size", 0)
            self.migration_status = data.get("migration_status", "enhanced")
            
            # Clear existing interactions
            self.interaction_containers.clear()
            self.current_interaction_id = None
            
            # Import interactions
            interactions_data = data.get("interactions", {})
            for interaction_id, interaction_data in interactions_data.items():
                container = InteractionContainer(
                    interaction_id=interaction_id,
                    interaction_start=interaction_data.get("interaction_start", 0.0),
                    interaction_stop=interaction_data.get("interaction_stop"),
                    optimization_metadata=interaction_data.get("optimization_metadata", {})
                )
                
                # Import messages
                messages_data = interaction_data.get("messages", [])
                for msg_data in messages_data:
                    enhanced_msg = EnhancedCommonChatMessage(**msg_data)
                    container.add_message(enhanced_msg)
                
                # Set validity state
                validity_state = interaction_data.get("validity_state")
                if validity_state:
                    from agent_c.models.chat_history.interaction_container import ValidityState
                    container.validity_state = ValidityState(validity_state)
                
                self.interaction_containers[interaction_id] = container
            
            return True
            
        except Exception as e:
            return False
    
    # Statistics and Analysis Methods
    
    def get_session_statistics(self) -> Dict[str, Any]:
        """Get comprehensive session statistics."""
        stats = {
            "session_id": self.session_id,
            "total_interactions": len(self.interaction_containers),
            "total_messages": len(self.get_all_messages()),
            "active_messages": len(self.get_all_messages(include_invalidated=False)),
            "token_count": self.token_count,
            "context_window_size": self.context_window_size,
            "session_duration": None,
            "interactions_completed": 0,
            "tools_used": set(),
            "message_types": {}
        }
        
        # Calculate session duration
        if self.created_at and self.updated_at:
            try:
                created = datetime.datetime.fromisoformat(self.created_at)
                updated = datetime.datetime.fromisoformat(self.updated_at)
                stats["session_duration"] = (updated - created).total_seconds()
            except:
                pass
        
        # Analyze interactions
        for container in self.interaction_containers.values():
            if container.is_completed():
                stats["interactions_completed"] += 1
            
            # Analyze messages in this interaction
            for msg in container.get_all_messages():
                # Count message types
                stats["message_types"][msg.role] = stats["message_types"].get(msg.role, 0) + 1
                
                # Extract tool usage
                for block in msg.content:
                    if hasattr(block, 'name'):  # Tool use block
                        stats["tools_used"].add(block.name)
        
        # Convert set to list for JSON serialization
        stats["tools_used"] = list(stats["tools_used"])
        
        return stats


# Backward compatibility alias
ChatSessionEnhanced = EnhancedChatSession