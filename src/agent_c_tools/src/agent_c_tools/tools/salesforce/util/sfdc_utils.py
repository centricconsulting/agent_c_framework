import re

class SalesforceQueryError(Exception):
    pass

def validate_soql_query(query: str) -> bool:
    """
    Validate a SOQL query for basic syntax and structure.
    This is a basic validation and may not catch all possible errors.
    """
    # Remove leading/trailing whitespace
    query = query.strip()

    # Check if the query starts with SELECT
    if not re.match(r'^SELECT', query, re.IGNORECASE):
        raise SalesforceQueryError("Query must start with SELECT")

    # Check if the query contains FROM
    if 'FROM' not in query.upper():
        raise SalesforceQueryError("Query must contain FROM clause")

    # Check for balanced parentheses
    if query.count('(') != query.count(')'):
        raise SalesforceQueryError("Unbalanced parentheses in query")

    # Check for common SOQL clauses
    valid_clauses = ['WHERE', 'GROUP BY', 'HAVING', 'ORDER BY', 'LIMIT', 'OFFSET']
    clause_pattern = r'\b(' + '|'.join(valid_clauses) + r')\b'
    clauses = re.findall(clause_pattern, query, re.IGNORECASE)

    # Check clause order
    expected_order = [clause.upper() for clause in valid_clauses if clause.upper() in [c.upper() for c in clauses]]
    if clauses != expected_order:
        raise SalesforceQueryError("Invalid clause order in query")

    # Additional checks could be added here for more specific SOQL syntax rules

    return True

def clean_salesforce_record(record: dict) -> dict:
    """
    Clean a Salesforce record by removing Salesforce-specific metadata fields.
    """
    cleaned_record = {}
    for key, value in record.items():
        if not key.startswith('attributes'):
            if isinstance(value, dict) and 'attributes' in value:
                cleaned_record[key] = clean_salesforce_record(value)
            else:
                cleaned_record[key] = value
    return cleaned_record

# Additional utility functions can be added here as needed