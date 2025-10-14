"""
Content cleaning utilities for Dynamics CRM entities.

This module handles cleaning of HTML/XML content from CRM entities to reduce
token consumption and improve readability.
"""

from bs4 import BeautifulSoup
from typing import List, Dict, Any, Tuple


class ContentCleaner:
    """Handles cleaning of HTML/XML content from CRM entities."""
    
    # Configuration: (field_name, parser_type)
    CLEAN_CONFIGS = {
        'annotations': ('notetext', 'html.parser'),
        'posts': ('text', 'xml'),
        'emails': ('description', 'html.parser'),
        'phonecalls': ('description', 'html.parser'),
        'appointments': ('description', 'html.parser')
    }
    
    @staticmethod
    def clean_content(text: str, parser: str = 'html.parser') -> str:
        """
        Clean single text content by removing HTML/XML tags.
        
        Args:
            text: Text content to clean
            parser: Parser type ('html.parser' or 'xml')
            
        Returns:
            Cleaned text without HTML/XML tags
        """
        soup = BeautifulSoup(text, parser)
        if parser == 'xml':
            return ''.join(soup.findAll(text=True))
        else:
            return soup.get_text()
    
    @classmethod
    def clean_entities(cls, entities: List[Dict[str, Any]], entity_type: str) -> List[Dict[str, Any]]:
        """
        Clean HTML/XML content from entity list.
        
        Args:
            entities: List of entity dictionaries
            entity_type: Type of entities for field selection
            
        Returns:
            List of entities with cleaned content
        """
        if not entities or entity_type not in cls.CLEAN_CONFIGS:
            return entities
        
        field, parser = cls.CLEAN_CONFIGS[entity_type]
        
        # Handle both single entity and list of entities
        entities_list = entities if isinstance(entities, list) else [entities]
        
        for entity in entities_list:
            if field in entity and entity[field]:
                entity[field] = cls.clean_content(entity[field], parser)
        
        return entities
    
    @classmethod
    def get_clean_config(cls, entity_type: str) -> Tuple[str, str]:
        """
        Get cleaning configuration for entity type.
        
        Args:
            entity_type: Type of entity
            
        Returns:
            Tuple of (field_name, parser_type) or None if not configured
        """
        return cls.CLEAN_CONFIGS.get(entity_type)
