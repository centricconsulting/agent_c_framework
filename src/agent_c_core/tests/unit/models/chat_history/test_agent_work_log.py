"""
Comprehensive test suite for the Agent Work Log System.

This test suite covers all functionality of the AgentWorkLog system including:
- Work log entry creation and management
- Parameter extraction and classification
- Tool action categorization and impact scope determination
- Filtering and querying capabilities
- Integration with InteractionContainer
- Export and audit functionality
"""

import pytest
import json
import time
from datetime import datetime, timezone, timedelta
from unittest.mock import Mock, patch
from typing import Dict, Any, List

from agent_c.models.chat_history.agent_work_log import (
    AgentWorkLog,
    AgentWorkLogEntry,
    ParameterExtractor,
    ParameterExtractionRule,
    ImpactScope,
    ActionCategory,
    ParameterImportance,
    OutcomeStatus,
    create_work_log,
    create_work_log_entry
)
from agent_c.models.common_chat.enhanced_models import ValidityState


class TestParameterExtractor:
    """Test the ParameterExtractor class."""
    
    def test_default_rules_creation(self):
        """Test that default extraction rules are created properly."""
        extractor = ParameterExtractor()
        assert len(extractor._default_rules) > 0
        
        # Check that we have rules for common tool patterns
        rule_patterns = [rule.tool_pattern for rule in extractor._default_rules]
        assert any('file' in pattern for pattern in rule_patterns)
        assert any('workspace' in pattern for pattern in rule_patterns)
        assert any('plan' in pattern for pattern in rule_patterns)
    
    def test_extract_parameters_file_tool(self):
        """Test parameter extraction for file tools."""
        extractor = ParameterExtractor()
        
        parameters = {
            'path': '/some/file/path.txt',
            'mode': 'write',
            'encoding': 'utf-8',
            'data': 'Very long content that should be truncated because it exceeds the maximum length limit for parameter values'
        }
        
        extracted, metadata = extractor.extract_parameters('file_write', parameters)
        
        assert 'path' in extracted
        assert extracted['path'] == '/some/file/path.txt'
        assert metadata['path'] == ParameterImportance.CRITICAL
        
        assert 'mode' in extracted
        assert metadata['mode'] == ParameterImportance.HIGH
        
        # Data should be excluded as it's marked VERBOSE
        assert 'data' not in extracted
    
    def test_extract_parameters_workspace_tool(self):
        """Test parameter extraction for workspace tools."""
        extractor = ParameterExtractor()
        
        parameters = {
            'path': '//workspace/some/path',
            'pattern': '*.py',
            'recursive': True,
            'max_tokens': 1000
        }
        
        extracted, metadata = extractor.extract_parameters('workspace_glob', parameters)
        
        assert extracted['path'] == '//workspace/some/path'
        assert metadata['path'] == ParameterImportance.CRITICAL
        assert extracted['pattern'] == '*.py'
        assert metadata['pattern'] == ParameterImportance.HIGH
        assert extracted['recursive'] is True
        assert metadata['max_tokens'] == ParameterImportance.LOW
    
    def test_extract_parameters_sensitive_data(self):
        """Test that sensitive data is redacted."""
        extractor = ParameterExtractor()
        
        parameters = {
            'username': 'testuser',
            'password': 'secret123',
            'api_key': 'sk-1234567890',
            'token': 'bearer_token_value'
        }
        
        extracted, metadata = extractor.extract_parameters('auth_tool', parameters)
        
        assert extracted['username'] == 'testuser'
        assert extracted['password'] == '[REDACTED]'
        assert extracted['api_key'] == '[REDACTED]'
        assert extracted['token'] == '[REDACTED]'
    
    def test_extract_parameters_value_truncation(self):
        """Test that long parameter values are truncated."""
        extractor = ParameterExtractor()
        
        long_value = 'x' * 200  # Very long value
        parameters = {
            'long_param': long_value,
            'short_param': 'short'
        }
        
        extracted, metadata = extractor.extract_parameters('generic_tool', parameters)
        
        assert len(extracted['long_param']) < len(long_value)
        assert extracted['long_param'].endswith('...')
        assert extracted['short_param'] == 'short'
    
    def test_register_custom_rule(self):
        """Test registering custom extraction rules."""
        extractor = ParameterExtractor()
        
        custom_rule = ParameterExtractionRule(
            tool_pattern=r'custom_.*',
            parameter_rules={
                'special_param': ParameterImportance.CRITICAL,
                'other_param': ParameterImportance.LOW
            },
            max_value_length=30
        )
        
        extractor.register_extraction_rule(custom_rule)
        
        parameters = {
            'special_param': 'important_value',
            'other_param': 'less_important',
            'unknown_param': 'default_handling'
        }
        
        extracted, metadata = extractor.extract_parameters('custom_tool', parameters)
        
        assert metadata['special_param'] == ParameterImportance.CRITICAL
        assert metadata['other_param'] == ParameterImportance.LOW
        assert 'other_param' not in extracted  # LOW importance excluded
        assert metadata['unknown_param'] == ParameterImportance.MEDIUM  # Default fallback


class TestAgentWorkLogEntry:
    """Test the AgentWorkLogEntry class."""
    
    def test_entry_creation(self):
        """Test basic work log entry creation."""
        entry = AgentWorkLogEntry(
            tool_name='test_tool',
            action_summary='Test action',
            key_parameters={'param1': 'value1'},
            outcome_status=OutcomeStatus.SUCCESS
        )
        
        assert entry.tool_name == 'test_tool'
        assert entry.action_summary == 'Test action'
        assert entry.key_parameters == {'param1': 'value1'}
        assert entry.outcome_status == OutcomeStatus.SUCCESS
        assert entry.entry_id is not None
        assert isinstance(entry.timestamp, datetime)
    
    def test_parameter_sanitization(self):
        """Test that parameters are properly sanitized."""
        # Test with non-serializable object
        class NonSerializable:
            def __str__(self):
                return 'non_serializable_object'
        
        entry = AgentWorkLogEntry(
            tool_name='test_tool',
            action_summary='Test action',
            key_parameters={
                'good_param': 'string_value',
                'bad_param': NonSerializable(),
                'number_param': 42
            }
        )
        
        assert entry.key_parameters['good_param'] == 'string_value'
        assert entry.key_parameters['bad_param'] == 'non_serializable_object'
        assert entry.key_parameters['number_param'] == 42
    
    def test_is_related_to(self):
        """Test relationship detection between entries."""
        entry1 = AgentWorkLogEntry(
            tool_name='tool1',
            action_summary='Action 1'
        )
        
        entry2 = AgentWorkLogEntry(
            tool_name='tool2',
            action_summary='Action 2',
            related_entries=[entry1.entry_id]
        )
        
        entry3 = AgentWorkLogEntry(
            tool_name='tool3',
            action_summary='Action 3',
            parent_entry_id=entry1.entry_id
        )
        
        entry4 = AgentWorkLogEntry(
            tool_name='tool4',
            action_summary='Action 4'
        )
        
        assert entry1.is_related_to(entry2)
        assert entry2.is_related_to(entry1)\n        assert entry1.is_related_to(entry3)
        assert entry3.is_related_to(entry1)
        assert not entry1.is_related_to(entry4)
    
    def test_get_concise_summary(self):
        """Test concise summary generation."""
        entry = AgentWorkLogEntry(
            tool_name='workspace_read',
            action_summary='Read file content',
            key_parameters={'path': '/test/file.txt', 'encoding': 'utf-8'},
            parameter_metadata={
                'path': ParameterImportance.CRITICAL,
                'encoding': ParameterImportance.MEDIUM
            },
            outcome_status=OutcomeStatus.SUCCESS
        )
        
        summary = entry.get_concise_summary()
        
        assert '✅' in summary  # Success icon
        assert 'workspace_read' in summary
        assert 'Read file content' in summary
        assert 'path=/test/file.txt' in summary
        assert 'encoding' not in summary  # Only critical/high importance shown
    
    def test_get_concise_summary_with_long_values(self):
        """Test concise summary with long parameter values."""
        entry = AgentWorkLogEntry(
            tool_name='test_tool',
            action_summary='Test action',
            key_parameters={
                'long_param': 'x' * 50,  # Very long value
                'short_param': 'short'
            },
            parameter_metadata={
                'long_param': ParameterImportance.CRITICAL,
                'short_param': ParameterImportance.HIGH
            },
            outcome_status=OutcomeStatus.FAILURE
        )
        
        summary = entry.get_concise_summary()
        
        assert '❌' in summary  # Failure icon
        assert 'long_param=' in summary
        assert '...' in summary  # Truncation indicator
        assert len(summary) < 200  # Reasonable length


class TestAgentWorkLog:
    """Test the AgentWorkLog class."""
    
    def test_work_log_creation(self):
        """Test basic work log creation."""
        work_log = AgentWorkLog()
        
        assert len(work_log.entries) == 0
        assert len(work_log.interaction_index) == 0
        assert len(work_log.tool_index) == 0
        assert work_log._parameter_extractor is not None
    
    def test_add_entry(self):
        """Test adding entries to work log."""
        work_log = AgentWorkLog()
        
        entry = AgentWorkLogEntry(
            tool_name='test_tool',
            action_summary='Test action',
            interaction_id='test_interaction'
        )
        
        entry_id = work_log.add_entry(entry)
        
        assert entry_id == entry.entry_id
        assert len(work_log.entries) == 1
        assert work_log.entries[0] == entry
        
        # Check indexes
        assert 'test_interaction' in work_log.interaction_index
        assert entry_id in work_log.interaction_index['test_interaction']
        assert 'test_tool' in work_log.tool_index
        assert entry_id in work_log.tool_index['test_tool']
    
    def test_log_tool_call(self):
        """Test logging a tool call."""
        work_log = AgentWorkLog()
        
        parameters = {
            'path': '/test/file.txt',
            'mode': 'read',
            'verbose_data': 'x' * 1000  # Should be filtered out
        }
        
        entry_id = work_log.log_tool_call(
            tool_name='file_read',
            parameters=parameters,
            interaction_id='test_interaction',
            action_summary='Read test file'
        )
        
        assert entry_id is not None
        assert len(work_log.entries) == 1
        
        entry = work_log.entries[0]
        assert entry.tool_name == 'file_read'
        assert entry.action_summary == 'Read test file'
        assert entry.interaction_id == 'test_interaction'
        assert 'path' in entry.key_parameters
        assert 'verbose_data' not in entry.key_parameters  # Should be filtered
    
    def test_update_entry_outcome(self):
        """Test updating entry outcomes."""
        work_log = AgentWorkLog()
        
        entry_id = work_log.log_tool_call(
            tool_name='test_tool',
            parameters={'param': 'value'}
        )
        
        work_log.update_entry_outcome(
            entry_id=entry_id,
            outcome_status=OutcomeStatus.SUCCESS,
            outcome_details='Operation completed successfully',
            execution_time_ms=150.5,
            affected_resources=['file1.txt', 'file2.txt']
        )
        
        entry = work_log.get_entry_by_id(entry_id)
        assert entry.outcome_status == OutcomeStatus.SUCCESS
        assert entry.outcome_details == 'Operation completed successfully'
        assert entry.execution_time_ms == 150.5
        assert 'file1.txt' in entry.affected_resources
        assert 'file2.txt' in entry.affected_resources
    
    def test_get_entries_for_interaction(self):
        """Test retrieving entries by interaction ID."""
        work_log = AgentWorkLog()
        
        # Add entries for different interactions
        entry1_id = work_log.log_tool_call('tool1', {}, interaction_id='interaction1')
        entry2_id = work_log.log_tool_call('tool2', {}, interaction_id='interaction1')
        entry3_id = work_log.log_tool_call('tool3', {}, interaction_id='interaction2')
        
        interaction1_entries = work_log.get_entries_for_interaction('interaction1')
        interaction2_entries = work_log.get_entries_for_interaction('interaction2')
        
        assert len(interaction1_entries) == 2
        assert len(interaction2_entries) == 1
        
        entry_ids_1 = [entry.entry_id for entry in interaction1_entries]
        assert entry1_id in entry_ids_1
        assert entry2_id in entry_ids_1
        
        assert interaction2_entries[0].entry_id == entry3_id
    
    def test_get_entries_for_tool(self):
        """Test retrieving entries by tool name."""
        work_log = AgentWorkLog()
        
        # Add entries for different tools
        entry1_id = work_log.log_tool_call('file_read', {})
        entry2_id = work_log.log_tool_call('file_read', {})
        entry3_id = work_log.log_tool_call('workspace_write', {})
        
        file_read_entries = work_log.get_entries_for_tool('file_read')
        workspace_write_entries = work_log.get_entries_for_tool('workspace_write')
        
        assert len(file_read_entries) == 2
        assert len(workspace_write_entries) == 1
        
        file_read_ids = [entry.entry_id for entry in file_read_entries]
        assert entry1_id in file_read_ids
        assert entry2_id in file_read_ids
        
        assert workspace_write_entries[0].entry_id == entry3_id
    
    def test_filter_entries(self):
        """Test filtering entries by various criteria."""
        work_log = AgentWorkLog()
        
        # Create entries with different characteristics
        base_time = datetime.now(timezone.utc)
        
        entry1 = AgentWorkLogEntry(
            tool_name='tool1',
            action_summary='Action 1',
            interaction_id='interaction1',
            outcome_status=OutcomeStatus.SUCCESS,
            action_category=ActionCategory.INFORMATION_RETRIEVAL,
            impact_scope=ImpactScope.LOCAL,
            timestamp=base_time
        )
        
        entry2 = AgentWorkLogEntry(
            tool_name='tool2',
            action_summary='Action 2',
            interaction_id='interaction2',
            outcome_status=OutcomeStatus.FAILURE,
            action_category=ActionCategory.DATA_MANIPULATION,
            impact_scope=ImpactScope.SYSTEM,
            timestamp=base_time + timedelta(minutes=5)
        )
        
        entry3 = AgentWorkLogEntry(
            tool_name='tool1',
            action_summary='Action 3',
            interaction_id='interaction1',
            outcome_status=OutcomeStatus.SUCCESS,
            action_category=ActionCategory.INFORMATION_RETRIEVAL,
            impact_scope=ImpactScope.SESSION,
            timestamp=base_time + timedelta(minutes=10)
        )
        
        work_log.add_entry(entry1)
        work_log.add_entry(entry2)
        work_log.add_entry(entry3)
        
        # Test filtering by interaction
        interaction1_entries = work_log.filter_entries(interaction_ids=['interaction1'])
        assert len(interaction1_entries) == 2
        
        # Test filtering by tool
        tool1_entries = work_log.filter_entries(tool_names=['tool1'])
        assert len(tool1_entries) == 2
        
        # Test filtering by outcome status
        success_entries = work_log.filter_entries(outcome_statuses=[OutcomeStatus.SUCCESS])
        assert len(success_entries) == 2
        
        # Test filtering by action category
        info_retrieval_entries = work_log.filter_entries(
            action_categories=[ActionCategory.INFORMATION_RETRIEVAL]
        )
        assert len(info_retrieval_entries) == 2
        
        # Test filtering by impact scope
        system_entries = work_log.filter_entries(impact_scopes=[ImpactScope.SYSTEM])
        assert len(system_entries) == 1
        
        # Test time range filtering
        mid_time = base_time + timedelta(minutes=3)
        recent_entries = work_log.filter_entries(start_time=mid_time)
        assert len(recent_entries) == 2
        
        # Test limit
        limited_entries = work_log.filter_entries(limit=1)
        assert len(limited_entries) == 1
        
        # Test combined filters
        combined_entries = work_log.filter_entries(
            interaction_ids=['interaction1'],
            outcome_statuses=[OutcomeStatus.SUCCESS]
        )
        assert len(combined_entries) == 2
    
    def test_get_interaction_summary(self):
        """Test interaction summary generation."""
        work_log = AgentWorkLog()
        
        # Add entries for an interaction
        work_log.log_tool_call(
            'file_read', 
            {'path': '/test.txt'}, 
            interaction_id='test_interaction'
        )
        work_log.log_tool_call(
            'workspace_write', 
            {'path': '/output.txt'}, 
            interaction_id='test_interaction'
        )
        
        # Update outcomes
        entries = work_log.get_entries_for_interaction('test_interaction')
        work_log.update_entry_outcome(
            entries[0].entry_id, 
            OutcomeStatus.SUCCESS, 
            execution_time_ms=100
        )
        work_log.update_entry_outcome(
            entries[1].entry_id, 
            OutcomeStatus.FAILURE, 
            execution_time_ms=50
        )
        
        summary = work_log.get_interaction_summary('test_interaction')
        
        assert summary['interaction_id'] == 'test_interaction'
        assert summary['entry_count'] == 2
        assert 'file_read' in summary['tools_used']
        assert 'workspace_write' in summary['tools_used']
        assert summary['outcome_distribution'][OutcomeStatus.SUCCESS] == 1
        assert summary['outcome_distribution'][OutcomeStatus.FAILURE] == 1
        assert summary['success_rate_percent'] == 50.0
        assert summary['total_execution_time_ms'] == 150.0
        assert len(summary['entries']) == 2
    
    def test_get_interaction_summary_empty(self):
        """Test interaction summary for non-existent interaction."""
        work_log = AgentWorkLog()
        
        summary = work_log.get_interaction_summary('nonexistent')
        
        assert summary['interaction_id'] == 'nonexistent'
        assert summary['entry_count'] == 0
        assert summary['tools_used'] == []
        assert 'No work log entries found' in summary['summary']
    
    def test_export_audit_report(self):
        """Test audit report export."""
        work_log = AgentWorkLog()
        
        # Add some entries
        entry_id = work_log.log_tool_call(
            'test_tool',
            {'param1': 'value1', 'param2': 'value2'},
            interaction_id='test_interaction',
            action_summary='Test action'
        )
        
        work_log.update_entry_outcome(
            entry_id,
            OutcomeStatus.SUCCESS,
            outcome_details='Success details',
            execution_time_ms=123.45
        )
        
        # Export as JSON
        report = work_log.export_audit_report(format='json', include_parameters=True)
        
        assert isinstance(report, str)
        data = json.loads(report)
        assert len(data) == 1
        
        entry_data = data[0]
        assert entry_data['entry_id'] == entry_id
        assert entry_data['tool_name'] == 'test_tool'
        assert entry_data['action_summary'] == 'Test action'
        assert entry_data['interaction_id'] == 'test_interaction'
        assert entry_data['outcome_status'] == OutcomeStatus.SUCCESS.value
        assert 'key_parameters' in entry_data
        assert entry_data['key_parameters']['param1'] == 'value1'
    
    def test_export_audit_report_with_filters(self):
        """Test audit report export with filters."""
        work_log = AgentWorkLog()
        
        # Add entries for different interactions
        work_log.log_tool_call('tool1', {}, interaction_id='interaction1')
        work_log.log_tool_call('tool2', {}, interaction_id='interaction2')
        
        # Export with filter
        report = work_log.export_audit_report(
            format='json',
            filter_criteria={'interaction_ids': ['interaction1']}
        )
        
        data = json.loads(report)
        assert len(data) == 1
        assert data[0]['interaction_id'] == 'interaction1'
    
    def test_get_statistics(self):
        """Test overall statistics generation."""
        work_log = AgentWorkLog()
        
        # Add various entries
        work_log.log_tool_call('tool1', {}, interaction_id='interaction1')
        work_log.log_tool_call('tool1', {}, interaction_id='interaction2')
        work_log.log_tool_call('tool2', {}, interaction_id='interaction1')
        
        # Update some outcomes
        entries = work_log.entries
        work_log.update_entry_outcome(
            entries[0].entry_id, 
            OutcomeStatus.SUCCESS, 
            execution_time_ms=100
        )
        work_log.update_entry_outcome(
            entries[1].entry_id, 
            OutcomeStatus.FAILURE, 
            execution_time_ms=200
        )
        
        stats = work_log.get_statistics()
        
        assert stats['total_entries'] == 3
        assert stats['unique_interactions'] == 2
        assert stats['unique_tools'] == 2
        assert stats['success_rate_percent'] == 33.3  # 1 success out of 3
        assert stats['average_execution_time_ms'] == 150.0  # (100 + 200) / 2
        assert stats['total_execution_time_ms'] == 300.0
        
        # Check most used tools
        most_used = stats['most_used_tools']
        assert len(most_used) == 2
        assert most_used[0][0] == 'tool1'  # Most used tool
        assert most_used[0][1] == 2  # Used twice
    
    def test_register_parameter_extraction_rule(self):
        """Test registering custom parameter extraction rules."""
        work_log = AgentWorkLog()
        
        custom_rule = ParameterExtractionRule(
            tool_pattern=r'custom_.*',
            parameter_rules={
                'important_param': ParameterImportance.CRITICAL
            }
        )
        
        work_log.register_parameter_extraction_rule(custom_rule)
        
        # Test that the custom rule is used
        entry_id = work_log.log_tool_call(
            'custom_tool',
            {'important_param': 'critical_value', 'other_param': 'other_value'}
        )
        
        entry = work_log.get_entry_by_id(entry_id)
        assert 'important_param' in entry.key_parameters
        assert entry.parameter_metadata['important_param'] == ParameterImportance.CRITICAL


class TestConvenienceFunctions:
    """Test convenience functions."""
    
    def test_create_work_log(self):
        """Test create_work_log convenience function."""
        work_log = create_work_log()
        
        assert isinstance(work_log, AgentWorkLog)
        assert len(work_log.entries) == 0
    
    def test_create_work_log_entry(self):
        """Test create_work_log_entry convenience function."""
        entry = create_work_log_entry(
            tool_name='test_tool',
            action_summary='Test action',
            interaction_id='test_interaction',
            outcome_status=OutcomeStatus.SUCCESS
        )
        
        assert isinstance(entry, AgentWorkLogEntry)
        assert entry.tool_name == 'test_tool'
        assert entry.action_summary == 'Test action'
        assert entry.interaction_id == 'test_interaction'
        assert entry.outcome_status == OutcomeStatus.SUCCESS


class TestIntegrationScenarios:
    """Test realistic integration scenarios."""
    
    def test_complete_workflow_scenario(self):
        """Test a complete workflow with multiple tool calls."""
        work_log = AgentWorkLog()
        
        # Simulate a file processing workflow
        interaction_id = 'file_processing_workflow'
        
        # Step 1: Read input file
        read_entry_id = work_log.log_tool_call(
            tool_name='workspace_read',
            parameters={'path': '//workspace/input.txt', 'encoding': 'utf-8'},
            interaction_id=interaction_id,
            action_category=ActionCategory.INFORMATION_RETRIEVAL,
            impact_scope=ImpactScope.LOCAL
        )
        
        # Step 2: Process data (simulate analysis)
        process_entry_id = work_log.log_tool_call(
            tool_name='data_processor',
            parameters={'algorithm': 'nlp_analysis', 'confidence_threshold': 0.8},
            interaction_id=interaction_id,
            action_category=ActionCategory.ANALYSIS,
            impact_scope=ImpactScope.LOCAL
        )
        
        # Step 3: Write output file
        write_entry_id = work_log.log_tool_call(
            tool_name='workspace_write',
            parameters={'path': '//workspace/output.txt', 'mode': 'write'},
            interaction_id=interaction_id,
            action_category=ActionCategory.DATA_MANIPULATION,
            impact_scope=ImpactScope.SESSION
        )
        
        # Update outcomes
        work_log.update_entry_outcome(
            read_entry_id, 
            OutcomeStatus.SUCCESS, 
            execution_time_ms=50,
            affected_resources=['input.txt']
        )
        work_log.update_entry_outcome(
            process_entry_id, 
            OutcomeStatus.SUCCESS, 
            execution_time_ms=500,
            outcome_details='Processed 1000 records with 95% confidence'
        )
        work_log.update_entry_outcome(
            write_entry_id, 
            OutcomeStatus.SUCCESS, 
            execution_time_ms=30,
            affected_resources=['output.txt']
        )
        
        # Verify the complete workflow
        summary = work_log.get_interaction_summary(interaction_id)
        
        assert summary['entry_count'] == 3
        assert summary['success_rate_percent'] == 100.0
        assert summary['total_execution_time_ms'] == 580.0
        assert len(summary['tools_used']) == 3
        
        # Verify tool categorization
        entries = work_log.get_entries_for_interaction(interaction_id)
        categories = [entry.action_category for entry in entries]
        assert ActionCategory.INFORMATION_RETRIEVAL in categories
        assert ActionCategory.ANALYSIS in categories
        assert ActionCategory.DATA_MANIPULATION in categories
        
        # Test audit report
        report = work_log.export_audit_report(
            filter_criteria={'interaction_ids': [interaction_id]},
            include_parameters=True,
            include_metadata=True
        )
        
        report_data = json.loads(report)
        assert len(report_data) == 3
        
        # Verify each entry has required fields
        for entry_data in report_data:
            assert 'entry_id' in entry_data
            assert 'tool_name' in entry_data
            assert 'action_summary' in entry_data
            assert 'outcome_status' in entry_data
            assert 'key_parameters' in entry_data
            assert 'execution_time_ms' in entry_data
    
    def test_error_handling_scenario(self):
        """Test error handling and partial failure scenarios."""
        work_log = AgentWorkLog()
        
        interaction_id = 'error_handling_test'
        
        # Successful operation
        success_entry_id = work_log.log_tool_call(
            'file_read',
            {'path': '/existing/file.txt'},
            interaction_id=interaction_id
        )
        
        # Failed operation
        failure_entry_id = work_log.log_tool_call(
            'file_read',
            {'path': '/nonexistent/file.txt'},
            interaction_id=interaction_id
        )
        
        # Partial success
        partial_entry_id = work_log.log_tool_call(
            'batch_processor',
            {'files': ['file1.txt', 'file2.txt', 'file3.txt']},
            interaction_id=interaction_id
        )
        
        # Update outcomes
        work_log.update_entry_outcome(
            success_entry_id,
            OutcomeStatus.SUCCESS,
            execution_time_ms=100
        )
        
        work_log.update_entry_outcome(
            failure_entry_id,
            OutcomeStatus.FAILURE,
            outcome_details='File not found: /nonexistent/file.txt',
            execution_time_ms=10
        )
        
        work_log.update_entry_outcome(
            partial_entry_id,
            OutcomeStatus.PARTIAL,
            outcome_details='Processed 2 out of 3 files successfully',
            execution_time_ms=250,
            affected_resources=['file1.txt', 'file2.txt']
        )
        
        # Verify error handling
        summary = work_log.get_interaction_summary(interaction_id)
        
        assert summary['entry_count'] == 3
        assert summary['outcome_distribution'][OutcomeStatus.SUCCESS] == 1
        assert summary['outcome_distribution'][OutcomeStatus.FAILURE] == 1
        assert summary['outcome_distribution'][OutcomeStatus.PARTIAL] == 1
        assert summary['success_rate_percent'] == 33.3  # Only 1 full success
        
        # Test filtering by failure status
        failed_entries = work_log.filter_entries(
            interaction_ids=[interaction_id],
            outcome_statuses=[OutcomeStatus.FAILURE]
        )
        assert len(failed_entries) == 1
        assert 'File not found' in failed_entries[0].outcome_details
    
    def test_concurrent_access_simulation(self):
        """Test thread safety with simulated concurrent access."""
        work_log = AgentWorkLog()
        
        import threading
        import time
        
        results = []
        errors = []
        
        def add_entries(thread_id: int):
            try:
                for i in range(10):
                    entry_id = work_log.log_tool_call(
                        f'tool_{thread_id}',
                        {'iteration': i, 'thread': thread_id},
                        interaction_id=f'interaction_{thread_id}'
                    )
                    results.append(entry_id)
                    
                    # Simulate some processing time
                    time.sleep(0.001)
                    
                    # Update outcome
                    work_log.update_entry_outcome(
                        entry_id,
                        OutcomeStatus.SUCCESS,
                        execution_time_ms=i * 10
                    )
            except Exception as e:
                errors.append(e)
        
        # Create multiple threads
        threads = []
        for i in range(5):
            thread = threading.Thread(target=add_entries, args=(i,))
            threads.append(thread)
        
        # Start all threads
        for thread in threads:
            thread.start()
        
        # Wait for completion
        for thread in threads:
            thread.join()
        
        # Verify results
        assert len(errors) == 0, f"Errors occurred: {errors}"
        assert len(results) == 50  # 5 threads * 10 entries each
        assert len(work_log.entries) == 50
        
        # Verify data integrity
        stats = work_log.get_statistics()
        assert stats['total_entries'] == 50
        assert stats['unique_interactions'] == 5
        assert stats['unique_tools'] == 5
        assert stats['success_rate_percent'] == 100.0


if __name__ == '__main__':
    pytest.main([__file__])