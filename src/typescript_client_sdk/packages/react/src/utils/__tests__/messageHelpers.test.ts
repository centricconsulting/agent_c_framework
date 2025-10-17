/**
 * Tests for Message Helper Utilities
 */

import { describe, it, expect } from 'vitest';
import { hasFileAttachments, countImages, getMessageDisplayText } from '../messageHelpers';
import type { Message, ContentPart } from '@agentc/realtime-core';

describe('messageHelpers', () => {
  describe('hasFileAttachments', () => {
    it('returns false for null content', () => {
      const message: Message = {
        role: 'assistant',
        content: null
      };
      
      expect(hasFileAttachments(message)).toBe(false);
    });
    
    it('returns false for string content', () => {
      const message: Message = {
        role: 'user',
        content: 'Just plain text'
      };
      
      expect(hasFileAttachments(message)).toBe(false);
    });
    
    it('returns false for array content with no images', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Hello' } as ContentPart,
          { type: 'text', text: 'World' } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(false);
    });
    
    it('returns true for array content with one image', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Check this out' } as ContentPart,
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/png', 
              data: 'base64data...' 
            } 
          } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(true);
    });
    
    it('returns true for array content with multiple images', () => {
      const message: Message = {
        role: 'user',
        content: [
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/png', 
              data: 'data1...' 
            } 
          } as ContentPart,
          { 
            type: 'image', 
            source: { 
              type: 'url', 
              url: 'https://example.com/image.jpg' 
            } 
          } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(true);
    });
    
    it('returns true for mixed content with images', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'First text' } as ContentPart,
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/jpeg', 
              data: 'imagedata...' 
            } 
          } as ContentPart,
          { type: 'text', text: 'Second text' } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(true);
    });
    
    it('handles empty array content', () => {
      const message: Message = {
        role: 'assistant',
        content: []
      };
      
      expect(hasFileAttachments(message)).toBe(false);
    });
    
    it('ignores tool_use and tool_result blocks', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Using a tool...' } as ContentPart,
          { 
            type: 'tool_use', 
            id: 'tool_123', 
            name: 'search', 
            input: { query: 'test' } 
          } as ContentPart,
          { 
            type: 'tool_result', 
            tool_use_id: 'tool_123', 
            content: 'Result...' 
          } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(false);
    });
  });
  
  describe('countImages', () => {
    it('returns 0 for null content', () => {
      const message: Message = {
        role: 'system',
        content: null
      };
      
      expect(countImages(message)).toBe(0);
    });
    
    it('returns 0 for string content', () => {
      const message: Message = {
        role: 'user',
        content: 'Text only message'
      };
      
      expect(countImages(message)).toBe(0);
    });
    
    it('returns 0 for array content with no images', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Response' } as ContentPart,
          { 
            type: 'tool_use', 
            id: 'tool_1', 
            name: 'calculator', 
            input: { expression: '2+2' } 
          } as ContentPart
        ]
      };
      
      expect(countImages(message)).toBe(0);
    });
    
    it('returns 1 for array content with one image', () => {
      const message: Message = {
        role: 'user',
        content: [
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/png', 
              data: 'imagedata...' 
            } 
          } as ContentPart
        ]
      };
      
      expect(countImages(message)).toBe(1);
    });
    
    it('returns correct count for multiple images', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Three images:' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/1.jpg' } 
          } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/2.jpg' } 
          } as ContentPart,
          { type: 'text', text: 'And more text' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/3.jpg' } 
          } as ContentPart
        ]
      };
      
      expect(countImages(message)).toBe(3);
    });
    
    it('returns 0 for empty array', () => {
      const message: Message = {
        role: 'user',
        content: []
      };
      
      expect(countImages(message)).toBe(0);
    });
    
    it('counts only image blocks, not other types', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Text block' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'base64', media_type: 'image/png', data: 'data...' } 
          } as ContentPart,
          { 
            type: 'tool_use', 
            id: 'tool_1', 
            name: 'search', 
            input: {} 
          } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/img.png' } 
          } as ContentPart,
          { 
            type: 'tool_result', 
            tool_use_id: 'tool_1', 
            content: 'Result' 
          } as ContentPart
        ]
      };
      
      expect(countImages(message)).toBe(2);
    });
  });
  
  describe('getMessageDisplayText', () => {
    it('returns empty string for null content', () => {
      const message: Message = {
        role: 'assistant',
        content: null
      };
      
      expect(getMessageDisplayText(message)).toBe('');
    });
    
    it('returns string content directly', () => {
      const message: Message = {
        role: 'user',
        content: 'Hello, world!'
      };
      
      expect(getMessageDisplayText(message)).toBe('Hello, world!');
    });
    
    it('returns empty string for empty array', () => {
      const message: Message = {
        role: 'assistant',
        content: []
      };
      
      expect(getMessageDisplayText(message)).toBe('');
    });
    
    it('extracts text from single text block', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Single text block' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Single text block');
    });
    
    it('concatenates multiple text blocks', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'First ' } as ContentPart,
          { type: 'text', text: 'Second ' } as ContentPart,
          { type: 'text', text: 'Third' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('First Second Third');
    });
    
    it('ignores image blocks', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Before image' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'base64', media_type: 'image/png', data: 'data...' } 
          } as ContentPart,
          { type: 'text', text: 'After image' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Before imageAfter image');
    });
    
    it('ignores tool blocks', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Using search tool' } as ContentPart,
          { 
            type: 'tool_use', 
            id: 'tool_1', 
            name: 'search', 
            input: { query: 'test' } 
          } as ContentPart,
          { 
            type: 'tool_result', 
            tool_use_id: 'tool_1', 
            content: 'Search results...' 
          } as ContentPart,
          { type: 'text', text: 'Based on search' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Using search toolBased on search');
    });
    
    it('handles mixed content correctly', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Look at ' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/1.jpg' } 
          } as ContentPart,
          { type: 'text', text: 'this and ' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/2.jpg' } 
          } as ContentPart,
          { type: 'text', text: 'that!' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Look at this and that!');
    });
    
    it('returns empty string when only non-text blocks present', () => {
      const message: Message = {
        role: 'user',
        content: [
          { 
            type: 'image', 
            source: { type: 'base64', media_type: 'image/png', data: 'data...' } 
          } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/img.jpg' } 
          } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('');
    });
    
    it('handles empty text blocks', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: '' } as ContentPart,
          { type: 'text', text: 'Hello' } as ContentPart,
          { type: 'text', text: '' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Hello');
    });
    
    it('preserves whitespace in text', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Line 1\n' } as ContentPart,
          { type: 'text', text: 'Line 2\n' } as ContentPart,
          { type: 'text', text: '  Indented' } as ContentPart
        ]
      };
      
      expect(getMessageDisplayText(message)).toBe('Line 1\nLine 2\n  Indented');
    });
  });
  
  describe('integration scenarios', () => {
    it('handles real-world user message with image', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'What do you see in this image?' } as ContentPart,
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/jpeg', 
              data: 'base64encodedimagedata...' 
            } 
          } as ContentPart
        ],
        timestamp: '2024-01-01T00:00:00Z'
      };
      
      expect(hasFileAttachments(message)).toBe(true);
      expect(countImages(message)).toBe(1);
      expect(getMessageDisplayText(message)).toBe('What do you see in this image?');
    });
    
    it('handles assistant response without images', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'I see a beautiful landscape with mountains and a lake.' } as ContentPart
        ],
        timestamp: '2024-01-01T00:00:01Z',
        model_id: 'claude-3-5-sonnet-20241022'
      };
      
      expect(hasFileAttachments(message)).toBe(false);
      expect(countImages(message)).toBe(0);
      expect(getMessageDisplayText(message)).toBe('I see a beautiful landscape with mountains and a lake.');
    });
    
    it('handles message with multiple images and text', () => {
      const message: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Compare these two images: ' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/image1.png' } 
          } as ContentPart,
          { type: 'text', text: ' and ' } as ContentPart,
          { 
            type: 'image', 
            source: { type: 'url', url: 'https://example.com/image2.png' } 
          } as ContentPart,
          { type: 'text', text: '. What are the differences?' } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(true);
      expect(countImages(message)).toBe(2);
      expect(getMessageDisplayText(message)).toBe('Compare these two images:  and . What are the differences?');
    });
    
    it('handles assistant message with tool use', () => {
      const message: Message = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Let me search for that information.' } as ContentPart,
          { 
            type: 'tool_use', 
            id: 'toolu_123', 
            name: 'web_search', 
            input: { query: 'latest AI news' } 
          } as ContentPart
        ]
      };
      
      expect(hasFileAttachments(message)).toBe(false);
      expect(countImages(message)).toBe(0);
      expect(getMessageDisplayText(message)).toBe('Let me search for that information.');
    });
  });
});
