"""
Option set value mappings for Dynamics CRM.

Maps numeric option set values to their human-readable text equivalents.
"""

# TODO: Possibly replace with Prefer: odata.include-annotations="OData.Community.Display.V1.FormattedValue" header to get formatted values

# AI-Related field
CEN_AIRELATED = {
    279120000: 'Yes (AI-Related)',
    279120001: 'No (Not AI-Related)'
}

# Opportunity Stage
CEN_OPPORTUNITYSTAGE = {
    279120000: 'Identify',
    279120001: 'Develop',
    279120002: 'Propose',
    279120003: 'Deliver'
}

# State Code (common across many entities)
STATECODE = {
    0: 'Active',
    1: 'Inactive',
    2: 'Closed'
}

# Status Code for Opportunities
OPPORTUNITY_STATUSCODE = {
    1: 'In Progress',
    2: 'On Hold',
    3: 'Won',
    4: 'Canceled',
    5: 'Out-Sold'
}

# All option set mappings
OPTION_SET_MAPPINGS = {
    'cen_airelated': CEN_AIRELATED,
    'cen_opportunitystage': CEN_OPPORTUNITYSTAGE,
    'statecode': STATECODE,
    'statuscode': OPPORTUNITY_STATUSCODE  # Entity-specific, might need refinement
}


def resolve_option_set(field_name: str, value: int) -> str:
    """
    Resolve an option set value to its text representation.
    
    Args:
        field_name: Name of the field (e.g., 'cen_airelated')
        value: Numeric option set value
        
    Returns:
        Text representation of the value, or str(value) if not found
    """
    if field_name not in OPTION_SET_MAPPINGS:
        return str(value)
    
    mapping = OPTION_SET_MAPPINGS[field_name]
    return mapping.get(value, str(value))
