"""
Unit tests for error_handler.py

Tests the standardized YAML error formatting functions.
"""

import unittest
import yaml
from agent_c_tools.tools.dynamics_crm.error_handler import (
    DynamicsErrorType,
    format_error,
    format_invalid_odata_error,
    format_lookup_not_found_error,
    format_multiple_matches_error,
    format_api_error,
    format_excel_creation_error
)


class TestErrorHandler(unittest.TestCase):
    """Test error formatting functions."""
    
    def test_format_error_basic(self):
        """Test basic error formatting."""
        result = format_error(
            error_type=DynamicsErrorType.API_ERROR,
            message="Test error message",
            suggestion="Test suggestion"
        )
        
        # Parse YAML
        parsed = yaml.safe_load(result)
        
        # Verify structure
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.API_ERROR)
        self.assertEqual(parsed['message'], "Test error message")
        self.assertEqual(parsed['suggestion'], "Test suggestion")
        self.assertNotIn('details', parsed)
    
    def test_format_error_with_details(self):
        """Test error formatting with additional details."""
        details = {'query': 'test query', 'count': 5}
        result = format_error(
            error_type=DynamicsErrorType.INVALID_ODATA_QUERY,
            message="Invalid query",
            suggestion="Fix the query",
            details=details
        )
        
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['details'], details)
    
    def test_format_invalid_odata_error(self):
        """Test OData error formatting."""
        query = "$filter=invalid syntax"
        error_msg = "Unbalanced parentheses"
        
        result = format_invalid_odata_error(query, error_msg)
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.INVALID_ODATA_QUERY)
        self.assertIn("Invalid OData query", parsed['message'])
        self.assertIn(error_msg, parsed['message'])
        self.assertIn("Check your OData query syntax", parsed['suggestion'])
        self.assertEqual(parsed['details']['query'], query)
    
    def test_format_lookup_not_found_error(self):
        """Test lookup not found error formatting."""
        lookup_type = "businessunits"
        search_value = "NonexistentCity"
        available = ["Columbus", "Chicago", "Cincinnati"]
        
        result = format_lookup_not_found_error(lookup_type, search_value, available)
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.LOOKUP_NOT_FOUND)
        self.assertIn(search_value, parsed['message'])
        self.assertIn("Columbus", parsed['suggestion'])
        self.assertEqual(parsed['details']['lookup_type'], lookup_type)
        self.assertEqual(parsed['details']['search_value'], search_value)
        self.assertEqual(parsed['details']['available_count'], 3)
    
    def test_format_lookup_not_found_error_truncates_options(self):
        """Test that lookup error truncates long option lists."""
        lookup_type = "accounts"
        search_value = "test"
        # Create 15 options
        available = [f"Option{i}" for i in range(15)]
        
        result = format_lookup_not_found_error(lookup_type, search_value, available)
        parsed = yaml.safe_load(result)
        
        # Suggestion should only show first 10
        suggestion = parsed['suggestion']
        self.assertIn("Option0", suggestion)
        self.assertIn("Option9", suggestion)
        self.assertNotIn("Option14", suggestion)
        self.assertIn("...", suggestion)
    
    def test_format_multiple_matches_error(self):
        """Test multiple matches error formatting."""
        lookup_type = "serviceofferings"
        search_value = "Data"
        matches = ["Data & Analytics", "Data Management", "Data Science"]
        
        result = format_multiple_matches_error(lookup_type, search_value, matches)
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.MULTIPLE_MATCHES)
        self.assertIn("Multiple", parsed['message'])
        self.assertIn(search_value, parsed['message'])
        self.assertIn("more specific", parsed['suggestion'])
        self.assertEqual(parsed['details']['matches'], matches)
        self.assertEqual(parsed['details']['total_matches'], 3)
    
    def test_format_multiple_matches_error_truncates_matches(self):
        """Test that multiple matches error truncates long match lists."""
        lookup_type = "accounts"
        search_value = "test"
        matches = [f"Match{i}" for i in range(15)]
        
        result = format_multiple_matches_error(lookup_type, search_value, matches)
        parsed = yaml.safe_load(result)
        
        # Details should only include first 10
        details_matches = parsed['details']['matches']
        self.assertEqual(len(details_matches), 10)
        self.assertEqual(parsed['details']['total_matches'], 15)
    
    def test_format_api_error(self):
        """Test general API error formatting."""
        operation = "fetching opportunities"
        error_msg = "Connection timeout"
        
        result = format_api_error(operation, error_msg)
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.API_ERROR)
        self.assertIn(operation, parsed['message'])
        self.assertIn(error_msg, parsed['message'])
        self.assertIn("temporary issue", parsed['suggestion'])
        self.assertIn("network connection", parsed['suggestion'])
    
    def test_format_excel_creation_error(self):
        """Test Excel creation error formatting."""
        error_msg = "Invalid data structure"
        
        result = format_excel_creation_error(error_msg)
        parsed = yaml.safe_load(result)
        
        self.assertTrue(parsed['error'])
        self.assertEqual(parsed['error_type'], DynamicsErrorType.EXCEL_CREATION_ERROR)
        self.assertIn(error_msg, parsed['message'])
        self.assertIn("data may not be in the expected format", parsed['suggestion'])
    
    def test_error_type_constants(self):
        """Test that error type constants are defined."""
        self.assertEqual(DynamicsErrorType.INVALID_ODATA_QUERY, "InvalidODataQuery")
        self.assertEqual(DynamicsErrorType.LOOKUP_NOT_FOUND, "LookupNotFound")
        self.assertEqual(DynamicsErrorType.MULTIPLE_MATCHES, "MultipleMatches")
        self.assertEqual(DynamicsErrorType.API_ERROR, "APIError")
        self.assertEqual(DynamicsErrorType.EXCEL_CREATION_ERROR, "ExcelCreationError")
    
    def test_yaml_output_is_valid(self):
        """Test that all error formatters produce valid YAML."""
        test_cases = [
            format_error("TestError", "Test message", "Test suggestion"),
            format_invalid_odata_error("$filter=test", "Error"),
            format_lookup_not_found_error("type", "value", ["opt1", "opt2"]),
            format_multiple_matches_error("type", "value", ["match1", "match2"]),
            format_api_error("operation", "error"),
            format_excel_creation_error("error")
        ]
        
        for result in test_cases:
            # Should not raise exception
            parsed = yaml.safe_load(result)
            # All errors should have these fields
            self.assertIn('error', parsed)
            self.assertIn('error_type', parsed)
            self.assertIn('message', parsed)
            self.assertIn('suggestion', parsed)


if __name__ == '__main__':
    unittest.main()
