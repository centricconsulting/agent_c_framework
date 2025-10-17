"""
Unit tests for data_presenter.py

Tests the DataPresenter class with YAML output and tiered response strategy.
"""

import unittest
import yaml
from agent_c_tools.tools.dynamics_crm.data_presenter import DataPresenter, SUMMARY_FIELDS


class MockAgentRuntime:
    """Mock agent runtime for token counting."""
    
    def __init__(self, tokens_per_char=1):
        """Initialize with configurable token ratio."""
        self.tokens_per_char = tokens_per_char
    
    def count_tokens(self, text):
        """Mock token counting based on text length."""
        return len(text) * self.tokens_per_char


class TestDataPresenter(unittest.TestCase):
    """Test DataPresenter class."""
    
    def test_init_without_agent_runtime(self):
        """Test DataPresenter initialization without agent_runtime."""
        presenter = DataPresenter()
        
        self.assertIsNone(presenter.agent_runtime)
    
    def test_init_with_agent_runtime(self):
        """Test DataPresenter initialization with agent_runtime."""
        mock_runtime = MockAgentRuntime()
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        self.assertEqual(presenter.agent_runtime, mock_runtime)
    
    def test_present_empty_entities(self):
        """Test presenting empty entity list."""
        presenter = DataPresenter()
        result = presenter.present([], 'opportunities')
        
        self.assertEqual(result['summary'], 'No entities found')
        self.assertEqual(result['entity_type'], 'opportunities')
        self.assertEqual(result['total_count'], 0)
        self.assertEqual(result['showing'], 'empty')
        self.assertEqual(result['estimated_tokens'], 0)
        self.assertEqual(result['entities'], [])
    
    def test_present_small_dataset_full_yaml(self):
        """Test presenting 1-3 entities returns full data."""
        entities = [
            {'id': '1', 'name': 'Entity 1'},
            {'id': '2', 'name': 'Entity 2'}
        ]
        
        presenter = DataPresenter()
        result = presenter.present(entities, 'opportunities')
        
        self.assertEqual(result['total_count'], 2)
        self.assertEqual(result['showing'], 'full')
        # Entities should be returned as list of dicts
        self.assertIsInstance(result['entities'], list)
        self.assertEqual(len(result['entities']), 2)
        self.assertEqual(result['entities'][0]['name'], 'Entity 1')
        self.assertEqual(result['entities'][1]['name'], 'Entity 2')
    
    def test_present_medium_dataset_under_token_limit(self):
        """Test presenting 4-10 entities with low token count returns full."""
        entities = [{'id': str(i), 'name': f'E{i}'} for i in range(5)]
        
        # Mock runtime that returns low token count
        mock_runtime = MockAgentRuntime(tokens_per_char=0.01)
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        result = presenter.present(entities, 'opportunities')
        
        self.assertEqual(result['total_count'], 5)
        self.assertEqual(result['showing'], 'full')
    
    def test_present_medium_dataset_over_token_limit(self):
        """Test presenting 4-10 entities with high token count returns summary."""
        entities = [
            {
                'opportunityid': str(i),
                'name': f'Opportunity {i}',
                'estimatedvalue': 100000 + i,
                'extra_field': 'lots of extra data' * 100
            } for i in range(5)
        ]
        
        # Mock runtime that returns high token count
        mock_runtime = MockAgentRuntime(tokens_per_char=10)
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        result = presenter.present(entities, 'opportunities')
        
        self.assertEqual(result['total_count'], 5)
        self.assertEqual(result['showing'], 'summary')
    
    def test_present_large_dataset_returns_summary(self):
        """Test presenting 11-50 entities returns summary."""
        entities = [{'id': str(i), 'name': f'Entity {i}'} for i in range(20)]
        
        presenter = DataPresenter()
        result = presenter.present(entities, 'opportunities')
        
        self.assertEqual(result['total_count'], 20)
        self.assertEqual(result['showing'], 'summary')
    
    def test_present_very_large_dataset_returns_file_mode(self):
        """Test presenting 50+ entities returns file mode."""
        entities = [{'id': str(i), 'name': f'Entity {i}'} for i in range(60)]
        
        presenter = DataPresenter()
        result = presenter.present(entities, 'opportunities')
        
        self.assertEqual(result['total_count'], 60)
        self.assertEqual(result['showing'], 'file')
    
    def test_determine_mode_0_to_3_records(self):
        """Test mode determination for 0-3 records."""
        presenter = DataPresenter()
        
        self.assertEqual(presenter._determine_mode([{'id': '1'}]), 'yaml')
        self.assertEqual(presenter._determine_mode([{'id': '1'}, {'id': '2'}]), 'yaml')
        self.assertEqual(presenter._determine_mode([{'id': '1'}, {'id': '2'}, {'id': '3'}]), 'yaml')
    
    def test_determine_mode_4_to_10_records_low_tokens(self):
        """Test mode determination for 4-10 records with low token count."""
        entities = [{'id': str(i), 'name': f'E{i}'} for i in range(5)]
        mock_runtime = MockAgentRuntime(tokens_per_char=0.01)
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        mode = presenter._determine_mode(entities)
        
        self.assertEqual(mode, 'yaml')
    
    def test_determine_mode_4_to_10_records_high_tokens(self):
        """Test mode determination for 4-10 records with high token count."""
        entities = [{'id': str(i), 'data': 'x' * 1000} for i in range(5)]
        mock_runtime = MockAgentRuntime(tokens_per_char=1)
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        mode = presenter._determine_mode(entities)
        
        self.assertEqual(mode, 'summary')
    
    def test_determine_mode_11_to_50_records(self):
        """Test mode determination for 11-50 records."""
        entities = [{'id': str(i)} for i in range(25)]
        presenter = DataPresenter()
        
        mode = presenter._determine_mode(entities)
        
        self.assertEqual(mode, 'summary')
    
    def test_determine_mode_over_50_records(self):
        """Test mode determination for 50+ records."""
        entities = [{'id': str(i)} for i in range(75)]
        presenter = DataPresenter()
        
        mode = presenter._determine_mode(entities)
        
        self.assertEqual(mode, 'file')
    
    def test_to_yaml_basic(self):
        """Test YAML conversion."""
        entities = [{'id': '1', 'name': 'Test'}]
        presenter = DataPresenter()
        
        result = presenter._to_yaml(entities)
        
        # Should be valid YAML
        parsed = yaml.safe_load(result)
        self.assertEqual(parsed[0]['id'], '1')
        self.assertEqual(parsed[0]['name'], 'Test')
    
    def test_to_yaml_preserves_unicode(self):
        """Test YAML conversion preserves Unicode characters."""
        entities = [{'name': 'Test™', 'desc': 'Data & Analytics'}]
        presenter = DataPresenter()
        
        result = presenter._to_yaml(entities)
        
        self.assertIn('Test™', result)
        self.assertIn('Data & Analytics', result)
    
    def test_to_yaml_summary_with_configured_type(self):
        """Test YAML summary for configured entity type."""
        entities = [
            {
                'opportunityid': '123',
                'name': 'Test Opportunity',
                'estimatedvalue': 100000,
                'extra_field1': 'Extra data 1',
                'extra_field2': 'Extra data 2'
            }
        ]
        presenter = DataPresenter()
        
        result = presenter._to_yaml_summary(entities, 'opportunities')
        
        # Should include summary fields
        self.assertIn('opportunityid', result)
        self.assertIn('name', result)
        self.assertIn('estimatedvalue', result)
        # Should NOT include extra fields
        self.assertNotIn('extra_field1', result)
        self.assertNotIn('extra_field2', result)
    
    def test_to_yaml_summary_with_unconfigured_type(self):
        """Test YAML summary for unconfigured entity type returns full data."""
        entities = [
            {
                'id': '123',
                'name': 'Test',
                'extra': 'data'
            }
        ]
        presenter = DataPresenter()
        
        result = presenter._to_yaml_summary(entities, 'unknown_type')
        
        # Should return full data
        parsed = yaml.safe_load(result)
        self.assertEqual(parsed[0]['id'], '123')
        self.assertEqual(parsed[0]['extra'], 'data')
    
    def test_to_yaml_summary_filters_multiple_entities(self):
        """Test YAML summary filters multiple entities correctly."""
        entities = [
            {
                'opportunityid': '1',
                'name': 'Opp 1',
                'estimatedvalue': 100000,
                'extra': 'data1'
            },
            {
                'opportunityid': '2',
                'name': 'Opp 2',
                'estimatedvalue': 200000,
                'extra': 'data2'
            }
        ]
        presenter = DataPresenter()
        
        result = presenter._to_yaml_summary(entities, 'opportunities')
        parsed = yaml.safe_load(result)
        
        # Should have 2 entities
        self.assertEqual(len(parsed), 2)
        # Should have summary fields
        self.assertEqual(parsed[0]['opportunityid'], '1')
        self.assertEqual(parsed[1]['opportunityid'], '2')
        # Should not have extra field
        self.assertNotIn('extra', parsed[0])
        self.assertNotIn('extra', parsed[1])
    
    def test_estimate_tokens_with_agent_runtime(self):
        """Test token estimation with agent_runtime."""
        mock_runtime = MockAgentRuntime(tokens_per_char=2)
        presenter = DataPresenter(agent_runtime=mock_runtime)
        
        tokens = presenter._estimate_tokens("12345")
        
        # Should use agent_runtime
        self.assertEqual(tokens, 10)  # 5 chars * 2
    
    def test_estimate_tokens_without_agent_runtime(self):
        """Test token estimation without agent_runtime (approximation)."""
        presenter = DataPresenter()
        
        tokens = presenter._estimate_tokens("1234567890123456")
        
        # Should use approximation (len / 4)
        self.assertEqual(tokens, 4)  # 16 chars / 4
    
    def test_get_note_for_mode_yaml_small(self):
        """Test note generation for small YAML datasets."""
        presenter = DataPresenter()
        
        note = presenter._get_note_for_mode('yaml', 2)
        
        self.assertIn('complete data', note.lower())
    
    def test_get_note_for_mode_yaml_medium(self):
        """Test note generation for medium YAML datasets."""
        presenter = DataPresenter()
        
        note = presenter._get_note_for_mode('yaml', 5)
        
        self.assertIn('complete data', note.lower())
        self.assertIn('token budget', note.lower())
    
    def test_get_note_for_mode_summary(self):
        """Test note generation for summary mode."""
        presenter = DataPresenter()
        
        note = presenter._get_note_for_mode('summary', 20)
        
        self.assertIn('summary', note.lower())
        self.assertIn('override_fields', note)
    
    def test_get_note_for_mode_file(self):
        """Test note generation for file mode."""
        presenter = DataPresenter()
        
        note = presenter._get_note_for_mode('file', 100)
        
        self.assertIn('file', note.lower())
        self.assertIn('preview', note.lower())
    
    def test_present_mode_override_yaml(self):
        """Test manual mode override to yaml."""
        entities = [{'id': str(i)} for i in range(50)]  # Would normally be 'file'
        presenter = DataPresenter()
        
        result = presenter.present(entities, 'opportunities', mode='yaml')
        
        self.assertEqual(result['showing'], 'full')
    
    def test_present_mode_override_summary(self):
        """Test manual mode override to summary."""
        entities = [{'id': '1'}]  # Would normally be 'yaml'
        presenter = DataPresenter()
        
        result = presenter.present(entities, 'opportunities', mode='summary')
        
        self.assertEqual(result['showing'], 'summary')
    
    def test_present_returns_valid_structure(self):
        """Test that present() returns valid structure with all fields."""
        entities = [{'id': '1', 'name': 'Test'}]
        presenter = DataPresenter()
        
        result = presenter.present(entities, 'accounts')
        
        # Verify all expected fields exist
        self.assertIn('summary', result)
        self.assertIn('entity_type', result)
        self.assertIn('total_count', result)
        self.assertIn('showing', result)
        self.assertIn('estimated_tokens', result)
        self.assertIn('note', result)
        self.assertIn('entities', result)
    
    def test_summary_fields_constant_defined(self):
        """Test that SUMMARY_FIELDS constant is properly defined."""
        # Should have entries for common entity types
        self.assertIn('opportunities', SUMMARY_FIELDS)
        self.assertIn('accounts', SUMMARY_FIELDS)
        self.assertIn('leads', SUMMARY_FIELDS)
        self.assertIn('contacts', SUMMARY_FIELDS)
        
        # Each should be a list of field names
        self.assertIsInstance(SUMMARY_FIELDS['opportunities'], list)
        self.assertGreater(len(SUMMARY_FIELDS['opportunities']), 0)
        
        # Key fields should be present
        self.assertIn('opportunityid', SUMMARY_FIELDS['opportunities'])
        self.assertIn('name', SUMMARY_FIELDS['opportunities'])


if __name__ == '__main__':
    unittest.main()
