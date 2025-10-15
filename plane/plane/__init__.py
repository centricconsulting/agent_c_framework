"""
PLANE Tools for Agent C

Comprehensive toolsets for managing PLANE project management system.

This package provides 7 toolsets with 33 tools total:

1. PlaneProjectTools (5 tools) - Project management
2. PlaneIssueTools (6 tools) - Issue/task management  
3. PlaneSearchTools (3 tools) - Search and discovery
4. PlaneAnalyticsTools (3 tools) - Analytics and reporting
5. PlaneIssueRelationsTools (6 tools) - Sub-issues and relations
6. PlaneLabelTools (5 tools) - Label management
7. PlaneBulkTools (5 tools) - Bulk operations

Authentication is handled via secure cookie storage.
"""

# Import all toolsets to trigger registration
from .tools.plane_projects import PlaneProjectTools
from .tools.plane_issues import PlaneIssueTools
from .tools.plane_search import PlaneSearchTools
from .tools.plane_analytics import PlaneAnalyticsTools
from .tools.plane_issue_relations import PlaneIssueRelationsTools
from .tools.plane_labels import PlaneLabelTools
from .tools.plane_bulk import PlaneBulkTools

__all__ = [
    'PlaneProjectTools',
    'PlaneIssueTools', 
    'PlaneSearchTools',
    'PlaneAnalyticsTools',
    'PlaneIssueRelationsTools',
    'PlaneLabelTools',
    'PlaneBulkTools',
]
