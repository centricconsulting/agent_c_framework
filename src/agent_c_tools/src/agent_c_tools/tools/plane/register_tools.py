"""
PLANE Tools Registration

Imports and registers all PLANE toolsets with the Agent C framework.
This file should be imported by the agent_c_tools package to make
PLANE tools available to agents.
"""

# Import all toolsets to trigger their registration
from .tools.plane_projects import PlaneProjectTools
from .tools.plane_issues import PlaneIssueTools
from .tools.plane_search import PlaneSearchTools
from .tools.plane_analytics import PlaneAnalyticsTools
from .tools.plane_issue_relations import PlaneIssueRelationsTools
from .tools.plane_labels import PlaneLabelTools
from .tools.plane_bulk import PlaneBulkTools

# Tools are auto-registered via Toolset.register() calls in each module

__all__ = [
    'PlaneProjectTools',
    'PlaneIssueTools',
    'PlaneSearchTools',
    'PlaneAnalyticsTools',
    'PlaneIssueRelationsTools',
    'PlaneLabelTools',
    'PlaneBulkTools',
]
