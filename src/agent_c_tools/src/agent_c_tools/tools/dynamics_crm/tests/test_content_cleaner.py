"""
Unit tests for content_cleaner.py

Tests the ContentCleaner class for HTML/XML cleaning.
"""

import unittest
from agent_c_tools.tools.dynamics_crm.content_cleaner import ContentCleaner


class TestContentCleaner(unittest.TestCase):
    """Test ContentCleaner class."""
    
    def test_clean_content_html_basic(self):
        """Test cleaning basic HTML content."""
        html = "<p>This is <b>bold</b> text.</p>"
        result = ContentCleaner.clean_content(html, parser='html.parser')
        
        self.assertEqual(result, "This is bold text.")
    
    def test_clean_content_html_complex(self):
        """Test cleaning complex HTML with nested tags."""
        html = "<div><p>Paragraph 1</p><ul><li>Item 1</li><li>Item 2</li></ul></div>"
        result = ContentCleaner.clean_content(html, parser='html.parser')
        
        # Should extract all text
        self.assertIn("Paragraph 1", result)
        self.assertIn("Item 1", result)
        self.assertIn("Item 2", result)
        # Should not contain HTML tags
        self.assertNotIn("<div>", result)
        self.assertNotIn("<p>", result)
    
    def test_clean_content_html_with_attributes(self):
        """Test cleaning HTML with attributes."""
        html = '<a href="http://example.com">Link Text</a>'
        result = ContentCleaner.clean_content(html, parser='html.parser')
        
        self.assertEqual(result, "Link Text")
        self.assertNotIn("http://example.com", result)
        self.assertNotIn("<a", result)
    
    def test_clean_content_xml_basic(self):
        """Test cleaning basic XML content."""
        xml = "<root><item>Item 1</item><item>Item 2</item></root>"
        result = ContentCleaner.clean_content(xml, parser='xml')
        
        self.assertEqual(result, "Item 1Item 2")
    
    def test_clean_content_xml_with_attributes(self):
        """Test cleaning XML with attributes."""
        xml = '<data id="123">Content</data>'
        result = ContentCleaner.clean_content(xml, parser='xml')
        
        self.assertEqual(result, "Content")
    
    def test_clean_content_empty_string(self):
        """Test cleaning empty string."""
        result = ContentCleaner.clean_content("", parser='html.parser')
        self.assertEqual(result, "")
    
    def test_clean_content_plain_text(self):
        """Test that plain text passes through unchanged."""
        text = "This is plain text with no HTML."
        result = ContentCleaner.clean_content(text, parser='html.parser')
        
        self.assertEqual(result, text)
    
    def test_clean_entities_annotations(self):
        """Test cleaning annotations entities."""
        entities = [
            {
                'annotationid': '123',
                'notetext': '<p>This is <b>annotated</b> text.</p>',
                'subject': 'Test Note'
            },
            {
                'annotationid': '456',
                'notetext': '<div>Another note</div>',
                'subject': 'Another Test'
            }
        ]
        
        result = ContentCleaner.clean_entities(entities, 'annotations')
        
        # First entity
        self.assertEqual(result[0]['notetext'], 'This is annotated text.')
        self.assertEqual(result[0]['subject'], 'Test Note')
        
        # Second entity
        self.assertEqual(result[1]['notetext'], 'Another note')
    
    def test_clean_entities_posts(self):
        """Test cleaning posts entities (XML parser)."""
        entities = [
            {
                'postid': '123',
                'text': '<post>Test post content</post>'
            }
        ]
        
        result = ContentCleaner.clean_entities(entities, 'posts')
        
        self.assertEqual(result[0]['text'], 'Test post content')
    
    def test_clean_entities_emails(self):
        """Test cleaning emails entities."""
        entities = [
            {
                'activityid': '123',
                'description': '<html><body>Email body with <b>formatting</b></body></html>',
                'subject': 'Test Email'
            }
        ]
        
        result = ContentCleaner.clean_entities(entities, 'emails')
        
        # Description should be cleaned
        self.assertIn('Email body with formatting', result[0]['description'])
        self.assertNotIn('<html>', result[0]['description'])
        # Subject should be unchanged
        self.assertEqual(result[0]['subject'], 'Test Email')
    
    def test_clean_entities_phonecalls(self):
        """Test cleaning phonecalls entities."""
        entities = [
            {
                'activityid': '123',
                'description': '<p>Phone call notes</p>'
            }
        ]
        
        result = ContentCleaner.clean_entities(entities, 'phonecalls')
        
        self.assertEqual(result[0]['description'], 'Phone call notes')
    
    def test_clean_entities_appointments(self):
        """Test cleaning appointments entities."""
        entities = [
            {
                'activityid': '123',
                'description': '<div>Meeting notes</div>'
            }
        ]
        
        result = ContentCleaner.clean_entities(entities, 'appointments')
        
        self.assertEqual(result[0]['description'], 'Meeting notes')
    
    def test_clean_entities_missing_field(self):
        """Test cleaning entities when field is missing."""
        entities = [
            {
                'annotationid': '123',
                'subject': 'Test Note'
                # notetext is missing
            }
        ]
        
        # Should not raise exception
        result = ContentCleaner.clean_entities(entities, 'annotations')
        
        # Entity should be unchanged
        self.assertEqual(result[0]['annotationid'], '123')
        self.assertEqual(result[0]['subject'], 'Test Note')
        self.assertNotIn('notetext', result[0])
    
    def test_clean_entities_field_is_none(self):
        """Test cleaning entities when field value is None."""
        entities = [
            {
                'annotationid': '123',
                'notetext': None
            }
        ]
        
        # Should not raise exception
        result = ContentCleaner.clean_entities(entities, 'annotations')
        
        # notetext should remain None
        self.assertIsNone(result[0]['notetext'])
    
    def test_clean_entities_unconfigured_type(self):
        """Test cleaning entities of unconfigured type."""
        entities = [
            {
                'opportunityid': '123',
                'name': 'Test Opportunity'
            }
        ]
        
        # Should return entities unchanged
        result = ContentCleaner.clean_entities(entities, 'opportunities')
        
        self.assertEqual(result, entities)
    
    def test_clean_entities_empty_list(self):
        """Test cleaning empty entity list."""
        result = ContentCleaner.clean_entities([], 'annotations')
        
        self.assertEqual(result, [])
    
    def test_clean_entities_none(self):
        """Test cleaning None entity list."""
        result = ContentCleaner.clean_entities(None, 'annotations')
        
        self.assertIsNone(result)
    
    def test_clean_entities_single_entity_not_list(self):
        """Test cleaning single entity (not in a list)."""
        entity = {
            'annotationid': '123',
            'notetext': '<p>Test note</p>'
        }
        
        # Should handle single entity
        result = ContentCleaner.clean_entities(entity, 'annotations')
        
        # Should still be a single dict (not a list)
        self.assertIsInstance(result, dict)
        self.assertEqual(result['notetext'], 'Test note')
    
    def test_get_clean_config_annotations(self):
        """Test getting clean config for annotations."""
        config = ContentCleaner.get_clean_config('annotations')
        
        self.assertEqual(config, ('notetext', 'html.parser'))
    
    def test_get_clean_config_posts(self):
        """Test getting clean config for posts."""
        config = ContentCleaner.get_clean_config('posts')
        
        self.assertEqual(config, ('text', 'xml'))
    
    def test_get_clean_config_emails(self):
        """Test getting clean config for emails."""
        config = ContentCleaner.get_clean_config('emails')
        
        self.assertEqual(config, ('description', 'html.parser'))
    
    def test_get_clean_config_phonecalls(self):
        """Test getting clean config for phonecalls."""
        config = ContentCleaner.get_clean_config('phonecalls')
        
        self.assertEqual(config, ('description', 'html.parser'))
    
    def test_get_clean_config_appointments(self):
        """Test getting clean config for appointments."""
        config = ContentCleaner.get_clean_config('appointments')
        
        self.assertEqual(config, ('description', 'html.parser'))
    
    def test_get_clean_config_unconfigured_type(self):
        """Test getting clean config for unconfigured type."""
        config = ContentCleaner.get_clean_config('opportunities')
        
        self.assertIsNone(config)
    
    def test_clean_configs_constant(self):
        """Test that CLEAN_CONFIGS constant is properly defined."""
        configs = ContentCleaner.CLEAN_CONFIGS
        
        # Should have entries for all configured types
        self.assertIn('annotations', configs)
        self.assertIn('posts', configs)
        self.assertIn('emails', configs)
        self.assertIn('phonecalls', configs)
        self.assertIn('appointments', configs)
        
        # Each should be a tuple of (field_name, parser)
        self.assertIsInstance(configs['annotations'], tuple)
        self.assertEqual(len(configs['annotations']), 2)


if __name__ == '__main__':
    unittest.main()
