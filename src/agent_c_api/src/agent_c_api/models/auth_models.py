"""
Authentication models for the Avatar API.

These models extend the core ChatUser model with authentication-specific fields
and provide SQLAlchemy database models for persistence.
"""

from datetime import datetime
from typing import Optional, List
from sqlalchemy import Column, String, Boolean, Text, JSON, Integer
from sqlalchemy.ext.declarative import declarative_base
from pydantic import BaseModel, Field, EmailStr

from agent_c.models.agent_config import CurrentAgentConfiguration, AgentCatalogEntry
from agent_c.models.chat_history.chat_session import ChatSessionQueryResponse
from agent_c.models.client_tool_info import ClientToolInfo
from agent_c.models.heygen import Avatar
from agent_c_api.core.voice.models import AvailableVoiceModel

Base = declarative_base()


class ChatUserTable(Base):
    """
    SQLAlchemy table for persisting ChatUser models.
    Simple mapping to store ChatUser data in SQLite.
    """
    __tablename__ = "chat_users"

    user_id = Column(String, primary_key=True, index=True)
    user_name = Column(String, unique=True, index=True, nullable=False)
    email = Column(String, nullable=True)
    first_name = Column(String, nullable=True)
    last_name = Column(String, nullable=True)
    password_hash = Column(String, nullable=True)  # Authentication field
    is_active = Column(Boolean, default=True)
    roles = Column(JSON, default=list)  
    groups = Column(JSON, default=list) 
    meta = Column(JSON, default=dict)
    created_at = Column(String, nullable=True)  # ISO string format to match ChatUser
    updated_at = Column(String, nullable=True)   
    deleted_at = Column(String, nullable=True)
    last_login = Column(String, nullable=True)
    version = Column(Integer, default=1)

    @classmethod
    def from_chat_user(cls, chat_user) -> "ChatUserTable":
        """Create table row from ChatUser model."""
        return cls(
            user_id=chat_user.user_id,
            user_name=chat_user.user_name,
            email=chat_user.email,
            first_name=chat_user.first_name,
            last_name=chat_user.last_name,
            password_hash=chat_user.password_hash,
            is_active=chat_user.is_active,
            roles=chat_user.roles,
            groups=chat_user.groups,
            meta=chat_user.meta,
            created_at=chat_user.created_at,
            updated_at=chat_user.updated_at,
            deleted_at=chat_user.deleted_at,
            last_login=chat_user.last_login,
            version=chat_user.version
        )
    
    def to_chat_user(self):
        """Convert table row to ChatUser model."""
        from agent_c.models.chat_history.user import ChatUser
        return ChatUser(
            user_id=self.user_id,
            user_name=self.user_name,
            email=self.email,
            first_name=self.first_name,
            last_name=self.last_name,
            password_hash=self.password_hash,
            is_active=self.is_active,
            roles=self.roles or [],
            groups=self.groups or [],
            meta=self.meta or {},
            created_at=self.created_at,
            updated_at=self.updated_at,
            deleted_at=self.deleted_at,
            last_login=self.last_login,
            version=self.version
        )


# Pydantic models for API requests/responses

class UserCreateRequest(BaseModel):
    """Request model for creating new users."""
    username: str = Field(..., min_length=3, max_length=50)
    password: str = Field(..., min_length=6, max_length=100)
    email: Optional[EmailStr] = None
    first_name: Optional[str] = Field(None, max_length=50)
    last_name: Optional[str] = Field(None, max_length=50)
    roles: List[str] = Field(default_factory=list)
    user_id: Optional[str] = None  # Allow specifying user_id, else generated


class UserUpdateRequest(BaseModel):
    """Request model for updating user information."""
    user_id: str = Field(..., description="User ID to update")
    email: Optional[EmailStr] = Field(None, description="New email address")
    first_name: Optional[str] = Field(None, max_length=50, description="New first name")
    last_name: Optional[str] = Field(None, max_length=50, description="New last name")
    roles: Optional[List[str]] = Field(None, description="New roles list")
    is_active: Optional[bool] = Field(None, description="Active status")


class PasswordChangeRequest(BaseModel):
    """Request model for changing user password."""
    user_id: str = Field(..., description="User ID whose password to change")
    new_password: str = Field(..., min_length=6, max_length=100, description="New password")
    old_password: Optional[str] = Field(None, min_length=6, max_length=100, description="Current password (required for user mode)")


class UserLoginRequest(BaseModel):
    """Request model for user login."""
    username: str = Field(..., min_length=3, max_length=50)
    password: str = Field(..., min_length=6, max_length=100)


class ChatUserResponse(BaseModel):
    """Response model for ChatUser data (without password_hash)."""
    user_id: str
    user_name: str
    email: Optional[str]
    first_name: Optional[str]
    last_name: Optional[str]
    is_active: bool
    roles: List[str]
    groups: List[str]
    created_at: Optional[str]
    last_login: Optional[str]

    @classmethod
    def from_chat_user(cls, chat_user) -> "ChatUserResponse":
        """Create response from ChatUser, excluding sensitive fields."""
        return cls(
            user_id=chat_user.user_id,
            user_name=chat_user.user_name,
            email=chat_user.email,
            first_name=chat_user.first_name,
            last_name=chat_user.last_name,
            is_active=chat_user.is_active,
            roles=chat_user.roles,
            groups=chat_user.groups,
            created_at=chat_user.created_at,
            last_login=chat_user.last_login
        )

class LoginResponse(BaseModel):
    """Response model for successful login."""
    token: str
    user: ChatUserResponse

class RealtimeLoginResponse(BaseModel):
    """Response model for avatar login with config payload."""
    agent_c_token: str = Field(..., description="JWT authentication token")
    heygen_token: str = Field(..., description="HeyGen access token")
    ui_session_id: str = Field(..., description="UI session identifier")
