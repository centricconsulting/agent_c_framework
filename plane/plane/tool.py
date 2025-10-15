"""
PLANE Tools - Main Entry Point

This file ensures all PLANE toolsets are imported and registered.
The Agent C auto-discovery looks for tool.py files.
"""

# Import all toolsets to trigger their Toolset.register() calls
from .tools.plane_projects import PlaneProjectTools
from .tools.plane_issues import PlaneIssueTools
from .tools.plane_search import PlaneSearchTools
from .tools.plane_analytics import PlaneAnalyticsTools
from .tools.plane_issue_relations import PlaneIssueRelationsTools
from .tools.plane_labels import PlaneLabelTools
from .tools.plane_bulk import PlaneBulkTools

# These imports execute the Toolset.register() calls in each module,
# making the toolsets available to the Agent C framework

__all__ = [
    'PlaneProjectTools',
    'PlaneIssueTools',
    'PlaneSearchTools',
    'PlaneAnalyticsTools',
    'PlaneIssueRelationsTools',
    'PlaneLabelTools',
    'PlaneBulkTools',
]
