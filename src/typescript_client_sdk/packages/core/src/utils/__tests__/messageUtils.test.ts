/**
 * Unit tests for messageUtils.ts utility functions
 * 
 * Tests pure functions for message processing and normalization
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  normalizeMessageContent,
  normalizeMessage,
  normalizeMessages,
  isValidMessage,
} from '../messageUtils';
import type { Message } from '../../events/types/CommonTypes';
import { messageFixtures } from '../../test/fixtures/protocol-events';

describe('messageUtils', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('normalizeMessageContent', () => {
    it('should return string content as-is', () => {
      // Arrange
      const content = 'This is a simple string';

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('This is a simple string');
    });

    it('should handle empty string', () => {
      // Arrange
      const content = '';

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('');
    });

    it('should return empty string for null', () => {
      // Arrange
      const content = null;

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('');
    });

    it('should return empty string for undefined', () => {
      // Arrange
      const content = undefined;

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('');
    });

    it('should extract text property from object', () => {
      // Arrange
      const content = {
        text: 'Extracted text content',
        otherProp: 'ignored',
      };

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Extracted text content');
    });

    it('should extract nested content property from object', () => {
      // Arrange
      const content = {
        content: 'Nested content string',
        otherProp: 'ignored',
      };

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Nested content string');
    });

    it('should extract value property from object', () => {
      // Arrange
      const content = {
        value: 'Value property content',
        otherProp: 'ignored',
      };

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Value property content');
    });

    it('should prioritize text property over other properties', () => {
      // Arrange
      const content = {
        text: 'Text property',
        content: 'Content property',
        value: 'Value property',
      };

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Text property');
    });

    it('should handle array of strings', () => {
      // Arrange
      const content = ['Hello', ' ', 'World'];

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Hello World');
    });

    it('should handle array of objects with text property', () => {
      // Arrange
      const content = [
        { text: 'First part' },
        { text: ' and ' },
        { text: 'second part' },
      ];

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('First part and second part');
    });

    it('should handle mixed array of strings and objects', () => {
      // Arrange
      const content = [
        'Start',
        { text: ' middle ' },
        'end',
      ];

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Start middle end');
    });

    it('should filter out non-text items from array', () => {
      // Arrange
      const content = [
        'Valid',
        null,
        { text: ' text' },
        undefined,
        { noText: 'ignored' },
        '',
      ];

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Valid text');
    });

    it('should return stringified representation for empty array', () => {
      // Arrange
      const content: any[] = [];

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('[]'); // Empty arrays get stringified
    });

    it('should stringify object without known properties', () => {
      // Arrange
      const content = {
        unknownProp: 'value',
        nested: { data: 'test' },
      };

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe(JSON.stringify(content, null, 2));
    });

    it('should handle circular reference in object', () => {
      // Arrange
      const content: any = { prop: 'value' };
      content.circular = content;

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('[Complex Object]');
    });

    it('should convert number to string', () => {
      // Arrange
      const content = 42;

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('42');
    });

    it('should convert boolean to string', () => {
      // Arrange
      const contentTrue = true;
      const contentFalse = false;

      // Act
      const resultTrue = normalizeMessageContent(contentTrue);
      const resultFalse = normalizeMessageContent(contentFalse);

      // Assert
      expect(resultTrue).toBe('true');
      expect(resultFalse).toBe('false');
    });

    it('should handle symbol type', () => {
      // Arrange
      const content = Symbol('test');

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('Symbol(test)');
    });

    it('should handle BigInt type', () => {
      // Arrange
      const content = BigInt(9007199254740991);

      // Act
      const result = normalizeMessageContent(content);

      // Assert
      expect(result).toBe('9007199254740991');
    });
  });

  describe('normalizeMessage', () => {
    it('should normalize valid message', () => {
      // Arrange
      const message = {
        role: 'user' as const,
        content: 'Test message',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'text' as const,
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result).toEqual(message);
    });

    it('should add default role when missing', () => {
      // Arrange
      const message = {
        content: 'Message without role',
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.role).toBe('assistant');
    });

    it('should add timestamp when missing', () => {
      // Arrange
      const message = {
        role: 'user' as const,
        content: 'Message without timestamp',
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.timestamp).toBeDefined();
      expect(new Date(result.timestamp!).getTime()).toBeCloseTo(Date.now(), -3);
    });

    it('should add format when missing', () => {
      // Arrange
      const message = {
        role: 'system' as const,
        content: 'Message without format',
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.format).toBe('text');
    });

    it('should normalize complex content to string', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: {
          text: 'Complex content object',
          metadata: 'ignored',
        },
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.content).toBe('Complex content object');
    });

    it('should handle null message', () => {
      // Arrange
      const message = null;

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should handle undefined message', () => {
      // Arrange
      const message = undefined;

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should handle non-object message', () => {
      // Arrange
      const message = 'not an object';

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should normalize message with null content', () => {
      // Arrange
      const message = {
        role: 'user' as const,
        content: null,
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.content).toBe('');
    });

    it('should normalize message with undefined content', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: undefined,
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result.content).toBe('');
    });

    it('should preserve extra properties while normalizing', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: 'Content',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'markdown' as const,
        customProp: 'preserved',
      };

      // Act
      const result = normalizeMessage(message);

      // Assert
      expect(result).toEqual({
        role: 'assistant',
        content: 'Content',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'markdown',
      });
    });
  });

  describe('normalizeMessages', () => {
    it('should normalize array of messages', () => {
      // Arrange
      const messages = [
        { role: 'user' as const, content: 'Message 1' },
        { content: 'Message 2' }, // Missing role
        { role: 'system' as const, content: { text: 'Message 3' } }, // Complex content
      ];

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toHaveLength(3);
      expect(result[0]).toMatchObject({ role: 'user', content: 'Message 1' });
      expect(result[1]).toMatchObject({ role: 'assistant', content: 'Message 2' });
      expect(result[2]).toMatchObject({ role: 'system', content: 'Message 3' });
    });

    it('should handle empty array', () => {
      // Arrange
      const messages: any[] = [];

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for non-array input', () => {
      // Arrange
      const messages = 'not an array' as any;

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for null input', () => {
      // Arrange
      const messages = null as any;

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for undefined input', () => {
      // Arrange
      const messages = undefined as any;

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should handle mixed valid and invalid messages', () => {
      // Arrange
      const messages = [
        { role: 'user' as const, content: 'Valid' },
        null,
        undefined,
        'invalid',
        { content: { text: 'Complex' } },
      ];

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toHaveLength(5);
      expect(result[0]).toMatchObject({ role: 'user', content: 'Valid' });
      expect(result[1]).toMatchObject({ role: 'system', content: '' });
      expect(result[2]).toMatchObject({ role: 'system', content: '' });
      expect(result[3]).toMatchObject({ role: 'system', content: '' });
      expect(result[4]).toMatchObject({ role: 'assistant', content: 'Complex' });
    });

    it('should normalize messages from protocol fixtures', () => {
      // Arrange
      const messages = [
        messageFixtures.textMessage,
        messageFixtures.multimodalMessage,
        messageFixtures.assistantWithCitations,
      ];

      // Act
      const result = normalizeMessages(messages);

      // Assert
      expect(result).toHaveLength(3);
      expect(result[0]).toMatchObject({ 
        role: 'user', 
        content: 'Simple text message',
        format: 'text',
      });
      // multimodalMessage has array content, should be stringified
      expect(typeof result[1].content).toBe('string');
      expect(result[2]).toMatchObject({ 
        role: 'assistant', 
        content: 'Here is the answer',
      });
    });
  });

  describe('isValidMessage', () => {
    it('should return true for valid user message', () => {
      // Arrange
      const message: Message = {
        role: 'user',
        content: 'Valid user message',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'text',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(true);
    });

    it('should return true for valid assistant message', () => {
      // Arrange
      const message: Message = {
        role: 'assistant',
        content: 'Valid assistant message',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(true);
    });

    it('should return true for valid system message', () => {
      // Arrange
      const message: Message = {
        role: 'system',
        content: 'Valid system message',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(true);
    });

    it('should return true for valid assistant thought message', () => {
      // Arrange
      const message: Message = {
        role: 'assistant (thought)',
        content: 'Thinking...',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(true);
    });

    it('should return false for invalid role', () => {
      // Arrange
      const message = {
        role: 'invalid',
        content: 'Message with invalid role',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for non-string content', () => {
      // Arrange
      const message = {
        role: 'user',
        content: { text: 'Not a string' },
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for array content', () => {
      // Arrange
      const message = {
        role: 'user',
        content: ['Array', 'content'],
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for missing role', () => {
      // Arrange
      const message = {
        content: 'Message without role',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for missing content', () => {
      // Arrange
      const message = {
        role: 'user',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for null', () => {
      // Arrange
      const message = null;

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for undefined', () => {
      // Arrange
      const message = undefined;

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for non-object', () => {
      // Arrange
      const message = 'not an object';

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for empty object', () => {
      // Arrange
      const message = {};

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for role as non-string', () => {
      // Arrange
      const message = {
        role: 123,
        content: 'Content',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for content as number', () => {
      // Arrange
      const message = {
        role: 'user',
        content: 123,
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for content as null', () => {
      // Arrange
      const message = {
        role: 'user',
        content: null,
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return false for content as undefined', () => {
      // Arrange
      const message = {
        role: 'assistant',
        content: undefined,
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(false);
    });

    it('should return true with empty string content', () => {
      // Arrange
      const message = {
        role: 'user',
        content: '',
      };

      // Act
      const result = isValidMessage(message);

      // Assert
      expect(result).toBe(true);
    });

    it('should validate protocol fixture messages', () => {
      // Arrange & Act & Assert
      expect(isValidMessage(messageFixtures.textMessage)).toBe(true);
      expect(isValidMessage(messageFixtures.multimodalMessage)).toBe(false); // Has array content
      expect(isValidMessage(messageFixtures.assistantWithCitations)).toBe(true);
      expect(isValidMessage(messageFixtures.toolUseMessage)).toBe(false); // Has array content
      expect(isValidMessage(messageFixtures.toolResultMessage)).toBe(false); // Has array content
    });
  });
});