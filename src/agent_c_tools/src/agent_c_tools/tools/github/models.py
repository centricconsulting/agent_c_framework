from typing import Dict, List, Any, Optional
from datetime import datetime
from pydantic import BaseModel, Field


class Repository(BaseModel):
    """
    Represents a GitHub repository.
    """
    id: int
    name: str
    full_name: str
    owner: str = Field(..., description="Repository owner login")
    description: Optional[str] = None
    html_url: str
    language: Optional[str] = None
    stargazers_count: int = 0
    forks_count: int = 0
    open_issues_count: int = 0
    default_branch: str
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None
    pushed_at: Optional[datetime] = None
    
    @classmethod
    def from_dict(cls, data: Dict[str, Any]) -> "Repository":
        """
        Create a Repository instance from a dictionary.
        
        Args:
            data: Dictionary containing repository data.
            
        Returns:
            Repository instance.
        """
        # Extract the owner login from the owner object
        owner = data.get("owner", {}).get("login") if isinstance(data.get("owner"), dict) else data.get("owner")
        
        # Create a new dictionary with the processed data
        processed_data = {
            **data,
            "owner": owner
        }
        
        return cls(**processed_data)


class Issue(BaseModel):
    """
    Represents a GitHub issue.
    """
    id: int
    number: int
    title: str
    body: Optional[str] = None
    state: str
    html_url: str
    user: str = Field(..., description="Issue creator login")
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None
    closed_at: Optional[datetime] = None
    labels: List[str] = Field(default_factory=list)
    assignees: List[str] = Field(default_factory=list)
    
    @classmethod
    def from_dict(cls, data: Dict[str, Any]) -> "Issue":
        """
        Create an Issue instance from a dictionary.
        
        Args:
            data: Dictionary containing issue data.
            
        Returns:
            Issue instance.
        """
        # Extract the user login from the user object
        user = data.get("user", {}).get("login") if isinstance(data.get("user"), dict) else data.get("user")
        
        # Process labels and assignees
        labels = [label.get("name") for label in data.get("labels", [])] if isinstance(data.get("labels"), list) else []
        assignees = [assignee.get("login") for assignee in data.get("assignees", [])] if isinstance(data.get("assignees"), list) else []
        
        # Create a new dictionary with the processed data
        processed_data = {
            **data,
            "user": user,
            "labels": labels,
            "assignees": assignees
        }
        
        return cls(**processed_data)


class PullRequest(BaseModel):
    """
    Represents a GitHub pull request.
    """
    id: int
    number: int
    title: str
    body: Optional[str] = None
    state: str
    html_url: str
    user: str = Field(..., description="Pull request creator login")
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None
    closed_at: Optional[datetime] = None
    merged_at: Optional[datetime] = None
    base: str = Field(..., description="Base branch reference")
    head: str = Field(..., description="Head branch reference")
    merged: Optional[bool] = None
    mergeable: Optional[bool] = None
    draft: bool = False
    
    @classmethod
    def from_dict(cls, data: Dict[str, Any]) -> "PullRequest":
        """
        Create a PullRequest instance from a dictionary.
        
        Args:
            data: Dictionary containing pull request data.
            
        Returns:
            PullRequest instance.
        """
        # Extract the user login from the user object
        user = data.get("user", {}).get("login") if isinstance(data.get("user"), dict) else data.get("user")
        
        # Extract branch references
        base = data.get("base", {}).get("ref") if isinstance(data.get("base"), dict) else data.get("base")
        head = data.get("head", {}).get("ref") if isinstance(data.get("head"), dict) else data.get("head")
        
        # Create a new dictionary with the processed data
        processed_data = {
            **data,
            "user": user,
            "base": base,
            "head": head
        }
        
        return cls(**processed_data)