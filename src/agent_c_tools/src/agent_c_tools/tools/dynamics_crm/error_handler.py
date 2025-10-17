"""
Error handling utilities for Dynamics CRM tool.

This module provides standardized error formatting and helpful error messages
for common issues encountered when working with Dynamics CRM.
"""

import yaml
from typing import Dict, Any, Optional


class DynamicsErrorType:
    """Error type constants for Dynamics CRM errors."""
    INVALID_ODATA_QUERY = "InvalidODataQuery"
    LOOKUP_NOT_FOUND = "LookupNotFound"
    MULTIPLE_MATCHES = "MultipleMatches"
    API_ERROR = "APIError"
    EXCEL_CREATION_ERROR = "ExcelCreationError"
    AUTHENTICATION_ERROR = "AuthenticationError"
    ENTITY_NOT_FOUND = "EntityNotFound"
    PERMISSION_ERROR = "PermissionError"


def format_error(
    error_type: str,
    message: str,
    suggestion: str,
    details: Optional[Dict[str, Any]] = None
) -> str:
    """
    Format error as YAML for consistent error responses.
    
    Args:
        error_type: Type of error (use DynamicsErrorType constants)
        message: Human-readable error message
        suggestion: What to do next
        details: Optional additional error details
        
    Returns:
        YAML formatted error string
    """
    error_dict = {
        'error': True,
        'error_type': error_type,
        'message': message,
        'suggestion': suggestion
    }
    
    if details:
        error_dict['details'] = details
    
    return yaml.dump(error_dict, default_flow_style=False, sort_keys=False, allow_unicode=True)


def format_invalid_odata_error(query: str, error_message: str) -> str:
    """
    Format OData query error with helpful suggestions.
    
    Args:
        query: The invalid query
        error_message: Error message from the API
        
    Returns:
        YAML formatted error string
    """
    suggestion = (
        "Check your OData query syntax. Common issues:\n"
        "- Unbalanced parentheses\n"
        "- Missing quotes around string values\n"
        "- Invalid field names\n"
        "- Incorrect operators (use 'eq', 'ne', 'gt', 'lt', etc.)\n"
        "Try using named filters (filter_by_service_offering, filter_by_business_unit) "
        "instead of complex $filter queries."
    )
    
    return format_error(
        error_type=DynamicsErrorType.INVALID_ODATA_QUERY,
        message=f"Invalid OData query: {error_message}",
        suggestion=suggestion,
        details={'query': query}
    )


def format_lookup_not_found_error(
    lookup_type: str,
    search_value: str,
    available_options: list
) -> str:
    """
    Format lookup not found error with available options.
    
    Args:
        lookup_type: Type of lookup (e.g., 'businessunits', 'serviceofferings')
        search_value: Value that was searched for
        available_options: List of available options (first 10)
        
    Returns:
        YAML formatted error string
    """
    options_str = ', '.join(available_options[:10])
    
    message = f"No {lookup_type} found matching '{search_value}'."
    suggestion = (
        f"Try being more specific or use one of these: {options_str}"
        f"{'...' if len(available_options) > 10 else ''}"
    )
    
    return format_error(
        error_type=DynamicsErrorType.LOOKUP_NOT_FOUND,
        message=message,
        suggestion=suggestion,
        details={
            'lookup_type': lookup_type,
            'search_value': search_value,
            'available_count': len(available_options)
        }
    )


def format_multiple_matches_error(
    lookup_type: str,
    search_value: str,
    matches: list
) -> str:
    """
    Format multiple matches error with disambiguation options.
    
    Args:
        lookup_type: Type of lookup
        search_value: Value that was searched for
        matches: List of matching names
        
    Returns:
        YAML formatted error string
    """
    matches_str = ', '.join(matches[:10])
    
    message = f"Multiple {lookup_type} found matching '{search_value}': {matches_str}"
    suggestion = "Please be more specific in your search term to match exactly one item."
    
    return format_error(
        error_type=DynamicsErrorType.MULTIPLE_MATCHES,
        message=message,
        suggestion=suggestion,
        details={
            'lookup_type': lookup_type,
            'search_value': search_value,
            'matches': matches[:10],
            'total_matches': len(matches)
        }
    )


def format_api_error(operation: str, error_message: str) -> str:
    """
    Format general API error.
    
    Args:
        operation: Operation being performed
        error_message: Error message from the API
        
    Returns:
        YAML formatted error string
    """
    suggestion = (
        "This may be a temporary issue. Try:\n"
        "- Checking your network connection\n"
        "- Verifying your permissions\n"
        "- Using force_login to re-authenticate\n"
        "- Simplifying your query"
    )
    
    return format_error(
        error_type=DynamicsErrorType.API_ERROR,
        message=f"Error during {operation}: {error_message}",
        suggestion=suggestion
    )


def format_excel_creation_error(error_message: str) -> str:
    """
    Format Excel file creation error.
    
    Args:
        error_message: Error message from the Excel creation
        
    Returns:
        YAML formatted error string
    """
    suggestion = (
        "The data may not be in the expected format. This usually happens when:\n"
        "- The API returned an error message instead of data\n"
        "- The data structure is invalid\n"
        "Try running the query with a smaller limit or different filters."
    )
    
    return format_error(
        error_type=DynamicsErrorType.EXCEL_CREATION_ERROR,
        message=f"Error creating Excel file: {error_message}",
        suggestion=suggestion
    )
