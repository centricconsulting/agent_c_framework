"""
Query configuration data class for Dynamics CRM queries.

This module provides a data class to hold query configuration, making parameter
passing cleaner and providing type safety.
"""

from dataclasses import dataclass, field
from typing import Optional, List, Dict, Any


@dataclass
class QueryConfig:
    """
    Configuration object for Dynamics CRM entity queries.
    
    This class encapsulates all the parameters needed to query entities from
    Dynamics CRM, providing type safety and cleaner method signatures.
    
    Attributes:
        entity_type: Type of entity to query (e.g., 'opportunities', 'accounts')
        entity_id: Optional specific entity ID to retrieve
        query_params: OData query string for filtering and options
        limit: Maximum number of records to return (0 = all records)
        additional_fields: Fields to add beyond the default set
        override_fields: Fields that completely replace the default set
        additional_expand: Additional relationships to expand in the query
        workspace_name: Workspace to use for file operations
        file_path: Path within workspace to save files
        force_save: Whether to force saving results to a file
        filter_by_service_offering: Named filter for service offerings
        filter_by_business_unit: Named filter for business units
        filter_by_industry_vertical: Named filter for industry verticals
    """
    
    # Core query parameters
    entity_type: str = 'opportunities'
    entity_id: Optional[str] = None
    query_params: str = ''
    limit: int = 10
    
    # Field customization
    additional_fields: Optional[List[str]] = None
    override_fields: Optional[List[str]] = None
    additional_expand: Optional[Dict[str, List[str]]] = None
    
    # File handling
    workspace_name: str = 'project'
    file_path: str = ''
    force_save: bool = False
    
    # Named filters (for Phase 3)
    filter_by_service_offering: Optional[List[str]] = None
    filter_by_business_unit: Optional[List[str]] = None
    filter_by_industry_vertical: Optional[List[str]] = None
    
    def __post_init__(self):
        """Validate and normalize configuration after initialization."""
        # Normalize file_path
        if self.file_path:
            self.file_path = self.file_path.strip('/')
        
        # Ensure entity_type is lowercase
        self.entity_type = self.entity_type.lower()
        
        # Ensure limit is non-negative
        if self.limit < 0:
            self.limit = 10
