"""
Data presentation module for Dynamics CRM results.

This module provides intelligent data presentation strategies to minimize token usage
while maintaining usability. It supports multiple output formats and automatically
selects the best approach based on dataset size.
"""

import yaml
from typing import List, Dict, Any, Optional, Literal

# Summary field configurations - essential fields per entity type
SUMMARY_FIELDS = {
    'opportunities': [
        'opportunityid', 'name', 'estimatedvalue', 'estimatedclosedate',
        'statecode', 'statecode_resolved', 'statuscode', 'statuscode_resolved',
        'cen_weightedrevenue', 'cen_airelated', 'cen_airelated_resolved',
        'cen_opportunitystage', 'cen_opportunitystage_resolved',
        '_parentaccountid_value', '_parentaccountid_value_resolved',  # Client/Account
        '_owningbusinessunit_value', '_owningbusinessunit_value_resolved',
        '_cen_serviceofferingcapabiity1_value_resolved',
        '_cen_serviceofferingcapability2_value_resolved',
        '_cen_serviceofferingcapability3_value_resolved',
        'webclienturl'
    ],
    'accounts': [
        'accountid', 'name', 'telephone1', 'emailaddress1',
        'websiteurl', '_primarycontactid_value', '_primarycontactid_value_resolved',
        '_owningbusinessunit_value', '_owningbusinessunit_value_resolved',
        'statecode', 'webclienturl'
    ],
    'leads': [
        'leadid', 'fullname', 'companyname', 
        'statuscode', 'statuscode_resolved', 'statecode', 'statecode_resolved',
        'estimatedamount', 'cen_airelated', 'cen_airelated_resolved', 
        'telephone1', 'emailaddress1',
        '_owningbusinessunit_value', '_owningbusinessunit_value_resolved',
        'webclienturl'
    ],
    'contacts': [
        'contactid', 'fullname', 'emailaddress1', 'telephone1',
        'jobtitle', '_parentcustomerid_value', '_parentcustomerid_value_resolved',
        '_owningbusinessunit_value', '_owningbusinessunit_value_resolved',
        'webclienturl'
    ],
    'tasks': [
        'activityid', 'subject', 'scheduledend', 'statecode', 'statuscode',
        '_regardingobjectid_value', '_regardingobjectid_value_resolved',
        '_createdby_value', '_createdby_value_resolved',
        '_owningbusinessunit_value', '_owningbusinessunit_value_resolved',
        'webclienturl'
    ],
    'appointments': [
        'activityid', 'subject', 'scheduledstart', 'scheduledend',
        'statecode', 'statuscode', 'location',
        '_regardingobjectid_value', '_regardingobjectid_value_resolved',
        '_organizer_value', '_organizer_value_resolved',
        'webclienturl'
    ],
    'phonecalls': [
        'activityid', 'subject', 'scheduledend', 'statecode', 'statuscode',
        'phonenumber', 'direction',
        '_regardingobjectid_value', '_regardingobjectid_value_resolved',
        '_createdby_value', '_createdby_value_resolved',
        'webclienturl'
    ],
    'emails': [
        'activityid', 'subject', 'createdon', 'statecode', 'statuscode',
        '_regardingobjectid_value', '_regardingobjectid_value_resolved',
        '_sender_value', '_sender_value_resolved',
        'webclienturl'
    ],
    'annotations': [
        'annotationid', 'subject', 'filename', 'filesize',
        '_objectid_value', '_objectid_value_resolved',
        '_createdby_value', '_createdby_value_resolved',
        'createdon'
    ],
    'posts': [
        'postid', 'text', 'createdon', 'type',
        '_regardingobjectid_value', '_regardingobjectid_value_resolved',
        '_createdby_value', '_createdby_value_resolved'
    ],
}


class DataPresenter:
    """
    Handles presentation of Dynamics CRM data with token optimization.
    
    This class implements a tiered response strategy that automatically
    selects the best presentation format based on dataset size:
    - Small datasets (0-3 records): Full data in YAML
    - Medium datasets (4-10 records): Full or summary based on token estimate
    - Large datasets (11-50 records): Summary only
    - Very large datasets (50+ records): File save with preview
    
    Attributes:
        agent_runtime: Optional runtime for accurate token counting
    """
    
    def __init__(self, agent_runtime=None):
        """
        Initialize the DataPresenter.
        
        Args:
            agent_runtime: Optional agent runtime for token counting
        """
        self.agent_runtime = agent_runtime
    
    def present(
        self,
        entities: List[Dict[str, Any]],
        entity_type: str,
        mode: Literal['auto', 'yaml', 'summary', 'file'] = 'auto'
    ) -> Dict[str, Any]:
        """
        Present entities in the most appropriate format.
        
        Args:
            entities: List of entity dictionaries
            entity_type: Type of entities being presented
            mode: Presentation mode ('auto', 'yaml', 'summary', 'file')
            
        Returns:
            Dictionary containing presentation results with metadata
        """
        if not entities:
            return {
                'summary': 'No entities found',
                'entity_type': entity_type,
                'total_count': 0,
                'showing': 'empty',
                'estimated_tokens': 0,
                'entities': []
            }
        
        # Determine the actual mode to use
        if mode == 'auto':
            mode = self._determine_mode(entities)
        
        # Determine which entities to include and prepare them
        if mode == 'yaml':
            entities_output = entities  # Full entity list as dicts
            showing = 'full'
        elif mode == 'summary':
            entities_output = self._prepare_summary_entities(entities, entity_type)
            showing = 'summary'
        elif mode == 'file':
            # File mode will be handled by the caller (tool.py)
            # Return preview data as summary
            preview_count = min(10, len(entities))
            entities_output = self._prepare_summary_entities(entities[:preview_count], entity_type)
            showing = 'file'
        else:
            # Fallback to full entities
            entities_output = entities
            showing = 'full'
        
        # Estimate tokens based on YAML representation
        yaml_preview = self._to_yaml(entities_output)
        estimated_tokens = self._estimate_tokens(yaml_preview)
        
        return {
            'summary': f'Found {len(entities)} {entity_type}',
            'entity_type': entity_type,
            'total_count': len(entities),
            'showing': showing,
            'estimated_tokens': estimated_tokens,
            'note': self._get_note_for_mode(mode, len(entities)),
            'entities': entities_output  # Return as list of dicts, not YAML string
        }
    
    def _determine_mode(self, entities: List[Dict[str, Any]]) -> str:
        """
        Automatically determine the best presentation mode.
        
        Strategy:
        - 0-3 records: Return full data (yaml)
        - 4-10 records: Return full if tokens < 5000, else summary
        - 11-50 records: Return summary if tokens < 15000, else file
        - 50+ records: Return file mode
        
        Args:
            entities: List of entity dictionaries
            
        Returns:
            Mode string ('yaml', 'summary', or 'file')
        """
        count = len(entities)
        
        if count <= 3:
            return 'yaml'
        elif count <= 10:
            # Check token estimate
            yaml_output = self._to_yaml(entities)
            estimated_tokens = self._estimate_tokens(yaml_output)
            return 'yaml' if estimated_tokens < 5000 else 'summary'
        elif count <= 50:
            # For medium-large datasets, check token size
            yaml_output = self._to_yaml(entities)
            estimated_tokens = self._estimate_tokens(yaml_output)
            return 'summary' if estimated_tokens < 15000 else 'file'
        else:
            return 'file'
    
    def _to_yaml(self, entities: List[Dict[str, Any]]) -> str:
        """
        Convert entities to YAML format.
        
        Args:
            entities: List of entity dictionaries
            
        Returns:
            YAML formatted string
        """
        return yaml.dump(
            entities,
            default_flow_style=False,
            sort_keys=False,
            allow_unicode=True
        )
    
    def _prepare_summary_entities(
        self,
        entities: List[Dict[str, Any]],
        entity_type: str
    ) -> List[Dict[str, Any]]:
        """
        Prepare entities with only summary fields.
        
        Args:
            entities: List of entity dictionaries
            entity_type: Type of entities for field selection
            
        Returns:
            List of entity dicts with only summary fields
        """
        # Get summary fields for this entity type
        summary_fields = SUMMARY_FIELDS.get(entity_type.lower(), None)
        
        if not summary_fields:
            # If no summary fields defined, return full entities
            return entities
        
        # Extract only summary fields from each entity
        summary_entities = []
        for entity in entities:
            summary_entity = {}
            for field in summary_fields:
                if field in entity:
                    summary_entity[field] = entity[field]
            summary_entities.append(summary_entity)
        
        return summary_entities
    
    def _to_yaml_summary(
        self,
        entities: List[Dict[str, Any]],
        entity_type: str
    ) -> str:
        """
        Convert entities to YAML format with only summary fields.
        
        Args:
            entities: List of entity dictionaries
            entity_type: Type of entities for field selection
            
        Returns:
            YAML formatted string with summary fields only
        """
        summary_entities = self._prepare_summary_entities(entities, entity_type)
        return self._to_yaml(summary_entities)
    
    def _estimate_tokens(self, text: str) -> int:
        """
        Estimate token count for text.
        
        Uses agent_runtime if available, otherwise uses approximation.
        
        Args:
            text: Text to estimate tokens for
            
        Returns:
            Estimated token count
        """
        if self.agent_runtime and hasattr(self.agent_runtime, 'count_tokens'):
            return self.agent_runtime.count_tokens(text)
        else:
            # Rough approximation: 1 token ≈ 4 characters
            return len(text) // 4
    
    def _get_note_for_mode(self, mode: str, count: int) -> str:
        """
        Get helpful note based on presentation mode.
        
        Args:
            mode: Presentation mode
            count: Number of entities
            
        Returns:
            Note string with context for user/LLM
        """
        if mode == 'yaml' and count <= 3:
            return 'Showing complete data for all records.'
        elif mode == 'yaml':
            return 'Showing complete data. Dataset is within token budget.'
        elif mode == 'summary':
            return 'Showing summary fields only to optimize token usage. Use override_fields parameter to get specific fields.'
        elif mode == 'file':
            return 'Dataset is large and has been saved to file. Showing preview of first 10 records.'
        else:
            return ''
