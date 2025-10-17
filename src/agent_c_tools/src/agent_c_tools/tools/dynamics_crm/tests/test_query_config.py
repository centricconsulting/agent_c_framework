"""
Unit tests for query_config.py

Tests the QueryConfig dataclass and its validation logic.
"""

import unittest
from agent_c_tools.tools.dynamics_crm.query_config import QueryConfig


class TestQueryConfig(unittest.TestCase):
    """Test QueryConfig dataclass."""
    
    def test_default_initialization(self):
        """Test QueryConfig with default values."""
        config = QueryConfig()
        
        self.assertEqual(config.entity_type, 'opportunities')
        self.assertIsNone(config.entity_id)
        self.assertEqual(config.query_params, '')
        self.assertEqual(config.limit, 10)
        self.assertIsNone(config.additional_fields)
        self.assertIsNone(config.override_fields)
        self.assertIsNone(config.additional_expand)
        self.assertEqual(config.workspace_name, 'project')
        self.assertEqual(config.file_path, '')
        self.assertFalse(config.force_save)
        self.assertIsNone(config.filter_by_service_offering)
        self.assertIsNone(config.filter_by_business_unit)
        self.assertIsNone(config.filter_by_industry_vertical)
    
    def test_custom_initialization(self):
        """Test QueryConfig with custom values."""
        config = QueryConfig(
            entity_type='accounts',
            entity_id='123-456',
            query_params='$filter=name eq \'test\'',
            limit=50,
            additional_fields=['field1', 'field2'],
            override_fields=['id', 'name'],
            workspace_name='test_workspace',
            file_path='/data/exports',
            force_save=True,
            filter_by_service_offering=['Data & Analytics'],
            filter_by_business_unit=['Columbus'],
            filter_by_industry_vertical=['Healthcare']
        )
        
        self.assertEqual(config.entity_type, 'accounts')
        self.assertEqual(config.entity_id, '123-456')
        self.assertEqual(config.query_params, '$filter=name eq \'test\'')
        self.assertEqual(config.limit, 50)
        self.assertEqual(config.additional_fields, ['field1', 'field2'])
        self.assertEqual(config.override_fields, ['id', 'name'])
        self.assertEqual(config.workspace_name, 'test_workspace')
        self.assertEqual(config.file_path, 'data/exports')  # Leading slash removed
        self.assertTrue(config.force_save)
        self.assertEqual(config.filter_by_service_offering, ['Data & Analytics'])
        self.assertEqual(config.filter_by_business_unit, ['Columbus'])
        self.assertEqual(config.filter_by_industry_vertical, ['Healthcare'])
    
    def test_file_path_normalization_leading_slash(self):
        """Test that leading slashes are removed from file_path."""
        config = QueryConfig(file_path='/some/path')
        self.assertEqual(config.file_path, 'some/path')
    
    def test_file_path_normalization_trailing_slash(self):
        """Test that trailing slashes are removed from file_path."""
        config = QueryConfig(file_path='some/path/')
        self.assertEqual(config.file_path, 'some/path')
    
    def test_file_path_normalization_both_slashes(self):
        """Test that both leading and trailing slashes are removed."""
        config = QueryConfig(file_path='/some/path/')
        self.assertEqual(config.file_path, 'some/path')
    
    def test_file_path_empty_string(self):
        """Test that empty file_path stays empty."""
        config = QueryConfig(file_path='')
        self.assertEqual(config.file_path, '')
    
    def test_file_path_just_slash(self):
        """Test file_path with just a slash becomes empty."""
        config = QueryConfig(file_path='/')
        self.assertEqual(config.file_path, '')
    
    def test_entity_type_lowercase_conversion(self):
        """Test that entity_type is converted to lowercase."""
        config = QueryConfig(entity_type='OPPORTUNITIES')
        self.assertEqual(config.entity_type, 'opportunities')
        
        config2 = QueryConfig(entity_type='Accounts')
        self.assertEqual(config2.entity_type, 'accounts')
    
    def test_negative_limit_resets_to_default(self):
        """Test that negative limit is reset to default of 10."""
        config = QueryConfig(limit=-5)
        self.assertEqual(config.limit, 10)
    
    def test_zero_limit_is_valid(self):
        """Test that limit of 0 (all records) is valid."""
        config = QueryConfig(limit=0)
        self.assertEqual(config.limit, 0)
    
    def test_positive_limit_is_preserved(self):
        """Test that positive limits are preserved."""
        config = QueryConfig(limit=100)
        self.assertEqual(config.limit, 100)
    
    def test_named_filters_default_to_none(self):
        """Test that named filter parameters default to None."""
        config = QueryConfig()
        self.assertIsNone(config.filter_by_service_offering)
        self.assertIsNone(config.filter_by_business_unit)
        self.assertIsNone(config.filter_by_industry_vertical)
    
    def test_named_filters_with_multiple_values(self):
        """Test named filters with multiple values."""
        config = QueryConfig(
            filter_by_service_offering=['Data & Analytics', 'Cybersecurity'],
            filter_by_business_unit=['Columbus', 'Chicago', 'Cincinnati']
        )
        
        self.assertEqual(len(config.filter_by_service_offering), 2)
        self.assertEqual(len(config.filter_by_business_unit), 3)
        self.assertIsNone(config.filter_by_industry_vertical)
    
    def test_query_config_is_dataclass(self):
        """Test that QueryConfig behaves as a dataclass."""
        config1 = QueryConfig(entity_type='opportunities', limit=5)
        config2 = QueryConfig(entity_type='opportunities', limit=5)
        
        # Dataclasses with same values should be equal
        self.assertEqual(config1, config2)
    
    def test_query_config_repr(self):
        """Test that QueryConfig has a useful string representation."""
        config = QueryConfig(entity_type='accounts', limit=20)
        repr_str = repr(config)
        
        # Should contain class name and key fields
        self.assertIn('QueryConfig', repr_str)
        self.assertIn('accounts', repr_str)
        self.assertIn('20', repr_str)


if __name__ == '__main__':
    unittest.main()
