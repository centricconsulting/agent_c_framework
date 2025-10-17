import re
from urllib.parse import urlparse, parse_qs, urlencode


def validate_odata_query(query_string):
    if query_string is None:
        return True

    # Remove leading '?' if present
    if query_string.startswith('?'):
        query_string = query_string[1:]

    # Parse the query string
    parsed = parse_qs(query_string)

    # List of valid OData parameters
    valid_params = ['$filter', '$expand', '$select', '$orderby', '$top', '$skip', '$count', '$search']

    # Check for invalid parameters
    invalid_params = [k for k in parsed.keys() if k not in valid_params]
    if invalid_params:
        raise ValueError(f"Invalid OData parameters: {', '.join(invalid_params)}")

    # Validate $filter
    if '$filter' in parsed:
        filter_value = parsed['$filter'][0]
        # Check for balanced parentheses
        if filter_value.count('(') != filter_value.count(')'):
            raise ValueError(f"Unbalanced parentheses in $filter={filter_value}")
        # Check for valid operators
        valid_operators = ['eq', 'ne', 'gt', 'ge', 'lt', 'le', 'and', 'or', 'not', 'contains']
        used_operators = re.findall(r'\b(' + '|'.join(valid_operators) + r')\b', filter_value)
        invalid_operators = [op for op in used_operators if op not in valid_operators]
        if invalid_operators:
            raise ValueError(f"Invalid operators in $filter: {', '.join(invalid_operators)}")

    # Validate $expand
    if '$expand' in parsed:
        expand_value = parsed['$expand'][0]
        # Check for balanced parentheses
        if expand_value.count('(') != expand_value.count(')'):
            raise ValueError(f"Unbalanced parentheses in $expand={expand_value}")

    # Validate $top
    if '$top' in parsed:
        try:
            top_value = int(parsed['$top'][0])
            if top_value <= 0:
                raise ValueError(f"$top must be a positive integer passed in {top_value}")
        except ValueError:
            raise ValueError(f"Invalid $top value in - {parsed}")

    # Reconstruct the query string
    # validated_query = urlencode(parsed, doseq=True)

    return True

# Example usage:
# try:
#     query = "$filter=contains(name, 'AI') and (contains(parentaccountid/name, 'Red Roof') or contains(parentaccountid/websiteurl, 'redroof'))&$expand=parentaccountid($select=name,accountid,websiteurl)&$top=50"
#     validated_query = validate_odata_query(query)
#     print("Validated query:", validated_query)
# except ValueError as e:
#     print("Validation error:", str(e))