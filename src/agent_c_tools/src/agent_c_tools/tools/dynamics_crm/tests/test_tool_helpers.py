"""
Unit tests for tool.py helper methods

Tests the helper methods extracted during refactoring.
"""

import unittest
from unittest.mock import Mock, MagicMock
from agent_c_tools.tools.dynamics_crm.tool import DynamicsCrmTools
from agent_c_tools.tools.dynamics_crm.query_config import QueryConfig


class TestToolHelpers(unittest.TestCase):
    """Test helper methods from DynamicsCrmTools."""
    
    def setUp(self):
        """Set up test fixtures."""
        # Create mock tool chest
        self.mock_tool_chest = Mock()
        
        # Create mock dynamics API
        self.mock_dynamics = Mock()
        self.mock_dynamics.common_lookups = {
            'businessunits': {
                'guid-columbus-1': 'Columbus',
                'guid-chicago-1': 'Chicago',
                'guid-cincinnati-1': 'Cincinnati',
                'guid-cleveland-1': 'Cleveland'
            },
            'serviceofferings': {
                'guid-data-analytics-1': 'Data & Analytics',
                'guid-cybersecurity-1': 'Cybersecurity',
                'guid-cloud-1': 'Cloud Services',
                'guid-data-mgmt-1': 'Data Management'
            },
            'industryverticals': {
                'guid-healthcare-1': 'Healthcare',
                'guid-financial-1': 'Financial Services',
                'guid-insurance-1': 'Insurance'
            }
        }
        
        # Create tool instance
        self.tool = DynamicsCrmTools(tool_chest=self.mock_tool_chest)
        self.tool.dynamics_object = self.mock_dynamics
    
    # _is_guid() tests
    
    def test_is_guid_valid_lowercase(self):
        """Test _is_guid with valid lowercase GUID."""
        guid = "12345678-1234-1234-1234-123456789abc"
        self.assertTrue(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_valid_uppercase(self):
        """Test _is_guid with valid uppercase GUID."""
        guid = "12345678-1234-1234-1234-123456789ABC"
        self.assertTrue(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_valid_mixed_case(self):
        """Test _is_guid with mixed case GUID."""
        guid = "12345678-1234-1234-1234-123456789AbC"
        self.assertTrue(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_invalid_too_short(self):
        """Test _is_guid with too short string."""
        guid = "12345678-1234-1234-1234"
        self.assertFalse(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_invalid_no_hyphens(self):
        """Test _is_guid without hyphens."""
        guid = "12345678123412341234123456789abc"
        self.assertFalse(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_invalid_wrong_format(self):
        """Test _is_guid with wrong hyphen positions."""
        guid = "123456-78-1234-1234-1234-123456789abc"
        self.assertFalse(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_invalid_not_hex(self):
        """Test _is_guid with non-hex characters."""
        guid = "12345678-1234-1234-1234-12345678xyz9"
        self.assertFalse(DynamicsCrmTools._is_guid(guid))
    
    def test_is_guid_empty_string(self):
        """Test _is_guid with empty string."""
        self.assertFalse(DynamicsCrmTools._is_guid(""))
    
    def test_is_guid_plain_text(self):
        """Test _is_guid with plain text."""
        self.assertFalse(DynamicsCrmTools._is_guid("Columbus"))
    
    # _resolve_lookup_filter() tests
    
    def test_resolve_lookup_filter_with_guid(self):
        """Test that GUIDs are returned unchanged."""
        guid = "12345678-1234-1234-1234-123456789abc"
        result = self.tool._resolve_lookup_filter(guid, 'businessunits')
        
        self.assertEqual(result, guid)
    
    def test_resolve_lookup_filter_exact_match(self):
        """Test lookup resolution with exact match."""
        result = self.tool._resolve_lookup_filter('Columbus', 'businessunits')
        
        self.assertEqual(result, 'guid-columbus-1')
    
    def test_resolve_lookup_filter_case_insensitive(self):
        """Test lookup resolution is case-insensitive."""
        result = self.tool._resolve_lookup_filter('columbus', 'businessunits')
        
        self.assertEqual(result, 'guid-columbus-1')
    
    def test_resolve_lookup_filter_partial_match(self):
        """Test lookup resolution with partial match that has multiple matches."""
        # 'Data' matches both 'Data & Analytics' and 'Data Management'
        with self.assertRaises(ValueError) as context:
            self.tool._resolve_lookup_filter('Data', 'serviceofferings')
        
        error_str = str(context.exception)
        self.assertIn('Multiple', error_str)
        self.assertIn('Data & Analytics', error_str)
        self.assertIn('Data Management', error_str)
    
    def test_resolve_lookup_filter_special_characters(self):
        """Test lookup resolution with special characters."""
        result = self.tool._resolve_lookup_filter('Data & Analytics', 'serviceofferings')
        
        self.assertEqual(result, 'guid-data-analytics-1')
    
    def test_resolve_lookup_filter_not_found(self):
        """Test lookup resolution with no match raises ValueError."""
        with self.assertRaises(ValueError) as context:
            self.tool._resolve_lookup_filter('Nonexistent', 'businessunits')
        
        # Should contain helpful error message
        error_str = str(context.exception)
        self.assertIn('No businessunits found', error_str)
        self.assertIn('Columbus', error_str)  # Should suggest available options
    
    def test_resolve_lookup_filter_multiple_matches_no_exact(self):
        """Test lookup resolution with multiple partial matches and no exact match."""
        # 'Cin' matches both 'Cincinnati' (businessunits) 
        with self.assertRaises(ValueError) as context:
            self.tool._resolve_lookup_filter('C', 'businessunits')
        
        # 'C' matches multiple: Columbus, Chicago, Cincinnati, Cleveland
        error_str = str(context.exception)
        self.assertIn('Multiple', error_str)
        self.assertIn('more specific', error_str)
    
    def test_resolve_lookup_filter_invalid_entity_type(self):
        """Test lookup resolution with invalid entity type."""
        with self.assertRaises(ValueError) as context:
            self.tool._resolve_lookup_filter('Test', 'invalid_type')
        
        error_str = str(context.exception)
        # Error comes back as YAML formatted error
        self.assertIn('error: true', error_str)
        self.assertIn('LookupNotFound', error_str)
        self.assertIn('businessunits', error_str)  # Should suggest valid types
    
    # _build_named_filters() tests
    
    def test_build_named_filters_no_filters(self):
        """Test building filters with no named filters specified."""
        config = QueryConfig()
        
        result = self.tool._build_named_filters(config)
        
        self.assertIsNone(result)
    
    def test_build_named_filters_single_business_unit(self):
        """Test building filter for single business unit."""
        config = QueryConfig(
            filter_by_business_unit=['Columbus']
        )
        
        result = self.tool._build_named_filters(config)
        
        self.assertIn("_owningbusinessunit_value eq 'guid-columbus-1'", result)
    
    def test_build_named_filters_multiple_business_units(self):
        """Test building filter for multiple business units."""
        config = QueryConfig(
            filter_by_business_unit=['Columbus', 'Chicago']
        )
        
        result = self.tool._build_named_filters(config)
        
        # Should have OR conditions
        self.assertIn("_owningbusinessunit_value eq 'guid-columbus-1'", result)
        self.assertIn(" or ", result)
        self.assertIn("_owningbusinessunit_value eq 'guid-chicago-1'", result)
    
    def test_build_named_filters_single_service_offering(self):
        """Test building filter for single service offering (checks 3 fields)."""
        config = QueryConfig(
            filter_by_service_offering=['Data & Analytics']
        )
        
        result = self.tool._build_named_filters(config)
        
        # Should check all 3 capability fields
        self.assertIn("_cen_serviceofferingcapabiity1_value eq 'guid-data-analytics-1'", result)
        self.assertIn("_cen_serviceofferingcapability2_value eq 'guid-data-analytics-1'", result)
        self.assertIn("_cen_serviceofferingcapability3_value eq 'guid-data-analytics-1'", result)
        # Should have OR between fields
        self.assertIn(" or ", result)
    
    def test_build_named_filters_multiple_service_offerings(self):
        """Test building filter for multiple service offerings."""
        config = QueryConfig(
            filter_by_service_offering=['Data & Analytics', 'Cybersecurity']
        )
        
        result = self.tool._build_named_filters(config)
        
        # Should have conditions for both offerings across all 3 fields (6 OR conditions)
        self.assertIn("guid-data-analytics-1", result)
        self.assertIn("guid-cybersecurity-1", result)
        # Count OR operators - should be at least 5 (for 6 conditions)
        self.assertGreaterEqual(result.count(" or "), 5)
    
    def test_build_named_filters_single_industry_vertical(self):
        """Test building filter for single industry vertical."""
        config = QueryConfig(
            filter_by_industry_vertical=['Healthcare']
        )
        
        result = self.tool._build_named_filters(config)
        
        self.assertIn("_cen_centricindustryvertical_value eq 'guid-healthcare-1'", result)
    
    def test_build_named_filters_multiple_industry_verticals(self):
        """Test building filter for multiple industry verticals."""
        config = QueryConfig(
            filter_by_industry_vertical=['Healthcare', 'Insurance']
        )
        
        result = self.tool._build_named_filters(config)
        
        self.assertIn("_cen_centricindustryvertical_value eq 'guid-healthcare-1'", result)
        self.assertIn(" or ", result)
        self.assertIn("_cen_centricindustryvertical_value eq 'guid-insurance-1'", result)
    
    def test_build_named_filters_combined_all_types(self):
        """Test building filters with all three filter types combined."""
        config = QueryConfig(
            filter_by_service_offering=['Data & Analytics'],
            filter_by_business_unit=['Columbus'],
            filter_by_industry_vertical=['Healthcare']
        )
        
        result = self.tool._build_named_filters(config)
        
        # Should have all three filter types
        self.assertIn("_cen_serviceofferingcapabiity1_value", result)
        self.assertIn("_owningbusinessunit_value", result)
        self.assertIn("_cen_centricindustryvertical_value", result)
        # Should be combined with 'and'
        self.assertIn(" and ", result)
    
    def test_build_named_filters_with_guid_input(self):
        """Test building filters when GUID is provided instead of name."""
        guid = "12345678-1234-1234-1234-123456789abc"
        config = QueryConfig(
            filter_by_business_unit=[guid]
        )
        
        result = self.tool._build_named_filters(config)
        
        # Should use the GUID directly
        self.assertIn(guid, result)
    
    # _build_unc_path() tests
    
    def test_build_unc_path_without_file_path(self):
        """Test UNC path construction without file_path."""
        result = self.tool._build_unc_path('project', '', 'test.xlsx')
        
        self.assertEqual(result, '//project/test.xlsx')
    
    def test_build_unc_path_with_file_path(self):
        """Test UNC path construction with file_path."""
        result = self.tool._build_unc_path('project', 'data/exports', 'test.xlsx')
        
        self.assertEqual(result, '//project/data/exports/test.xlsx')
    
    def test_build_unc_path_with_nested_file_path(self):
        """Test UNC path construction with nested file_path."""
        result = self.tool._build_unc_path('workspace', 'a/b/c', 'file.xlsx')
        
        self.assertEqual(result, '//workspace/a/b/c/file.xlsx')
    
    def test_build_unc_path_different_workspace(self):
        """Test UNC path construction with different workspace."""
        result = self.tool._build_unc_path('custom_workspace', 'reports', 'data.xlsx')
        
        self.assertEqual(result, '//custom_workspace/reports/data.xlsx')
    
    # _create_file_url() tests
    
    def test_create_file_url_with_absolute_path_unix(self):
        """Test file URL creation with Unix absolute path."""
        file_path = '/home/user/workspace/file.xlsx'
        
        result = self.tool._create_file_url(file_path, '//project/file.xlsx')
        
        self.assertEqual(result, 'file:///home/user/workspace/file.xlsx')
    
    def test_create_file_url_with_absolute_path_windows(self):
        """Test file URL creation with Windows absolute path."""
        file_path = 'C:\\Users\\user\\workspace\\file.xlsx'
        
        result = self.tool._create_file_url(file_path, '//project/file.xlsx')
        
        # Backslashes should be converted to forward slashes
        self.assertEqual(result, 'file:///C:/Users/user/workspace/file.xlsx')
    
    def test_create_file_url_with_none_file_path(self):
        """Test file URL creation when file_path is None (fallback to UNC)."""
        unc_path = '//project/data/file.xlsx'
        
        result = self.tool._create_file_url(None, unc_path)
        
        self.assertEqual(result, 'file:///project/data/file.xlsx')
    
    def test_create_file_url_fallback_to_unc(self):
        """Test file URL creation falls back to UNC path."""
        unc_path = '//workspace/reports/data.xlsx'
        
        result = self.tool._create_file_url(None, unc_path)
        
        self.assertIn('workspace/reports/data.xlsx', result)
        self.assertTrue(result.startswith('file:///'))


if __name__ == '__main__':
    unittest.main()
