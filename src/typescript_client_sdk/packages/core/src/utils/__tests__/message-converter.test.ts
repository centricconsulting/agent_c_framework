/**
 * Unit tests for message-converter.ts utility functions
 * 
 * Tests pure functions that convert between server MessageParam format 
 * and client Message format for UI rendering
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  convertMessageParamContent,
  convertMessageParamToMessage,
  convertMessageParamsToMessages,
  ensureMessageFormat,
  ensureMessagesFormat,
} from '../message-converter';
import type { MessageParam, ContentBlockParam, TextBlockParam, ImageBlockParam, ToolUseBlockParam, ToolResultBlockParam } from '../../types/message-params';
import type { Message, ContentPart } from '../../events/types/CommonTypes';
import { messageFixtures } from '../../test/fixtures/protocol-events';

describe('message-converter', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('convertMessageParamContent', () => {
    it('should handle null content', () => {
      // Arrange
      const content = null as any;

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBeNull();
    });

    it('should handle undefined content', () => {
      // Arrange
      const content = undefined as any;

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBeNull();
    });

    it('should handle string content', () => {
      // Arrange
      const content = 'Hello, world!';

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBe('Hello, world!');
    });

    it('should handle empty string content', () => {
      // Arrange
      const content = '';

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBe('');
    });

    it('should convert text block to text content part', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'text',
          text: 'This is a text block',
        } as TextBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: 'This is a text block',
        },
      ]);
    });

    it('should strip cache_control and citations from text blocks', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'text',
          text: 'Text with metadata',
          cache_control: { type: 'ephemeral', ttl: '5m' },
          citations: [{ type: 'char', start: 0, end: 5 }],
        } as TextBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: 'Text with metadata',
        },
      ]);
    });

    it('should convert image block to image content part', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'base64data',
          },
        } as ImageBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'base64data',
          },
        },
      ]);
    });

    it('should convert tool_use block to tool_use content part', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'tool_use',
          id: 'tool-123',
          name: 'calculator',
          input: { expression: '2 + 2' },
        } as ToolUseBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'tool_use',
          id: 'tool-123',
          name: 'calculator',
          input: { expression: '2 + 2' },
        },
      ]);
    });

    it('should convert tool_result block to tool_result content part', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'tool_result',
          tool_use_id: 'tool-123',
          content: '4',
          is_error: false,
        } as ToolResultBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'tool_result',
          tool_use_id: 'tool-123',
          content: '4',
          is_error: false,
        },
      ]);
    });

    it('should convert thinking blocks to text for display', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'thinking',
          thinking: 'Processing request...',
          signature: 'sig123',
        } as any,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: 'Processing request...',
        },
      ]);
    });

    it('should convert redacted_thinking blocks to text', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'redacted_thinking',
          data: 'Redacted content',
        } as any,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: 'Redacted content',
        },
      ]);
    });

    it('should handle thinking block with no content', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'thinking',
        } as any,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: '[Thinking...]',
        },
      ]);
    });

    it('should convert complex block types to text representation', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        { type: 'document' } as any,
        { type: 'search_result' } as any,
        { type: 'server_tool_use' } as any,
        { type: 'web_search_tool_result' } as any,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        { type: 'text', text: '[document content]' },
        { type: 'text', text: '[search_result content]' },
        { type: 'text', text: '[server_tool_use content]' },
        { type: 'text', text: '[web_search_tool_result content]' },
      ]);
    });

    it('should handle mixed content blocks', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        { type: 'text', text: 'Here is an image:' } as TextBlockParam,
        {
          type: 'image',
          source: { type: 'url', url: 'https://example.com/image.png' },
        } as ImageBlockParam,
        { type: 'text', text: 'And here is a calculation:' } as TextBlockParam,
        {
          type: 'tool_use',
          id: 'calc-1',
          name: 'calculator',
          input: { expression: '5 * 5' },
        } as ToolUseBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        { type: 'text', text: 'Here is an image:' },
        { type: 'image', source: { type: 'url', url: 'https://example.com/image.png' } },
        { type: 'text', text: 'And here is a calculation:' },
        { type: 'tool_use', id: 'calc-1', name: 'calculator', input: { expression: '5 * 5' } },
      ]);
    });

    it('should filter out null conversions from invalid blocks', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        { type: 'text', text: 'Valid text' } as TextBlockParam,
        {} as any, // Invalid block with no type
        { notAType: 'invalid' } as any, // Invalid block
        { type: 'text', text: 'Another valid text' } as TextBlockParam,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        { type: 'text', text: 'Valid text' },
        { type: 'text', text: 'Another valid text' },
      ]);
    });

    it('should return null for empty array', () => {
      // Arrange
      const content: ContentBlockParam[] = [];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBeNull();
    });

    it('should handle unknown block type with text field', () => {
      // Arrange
      const content: ContentBlockParam[] = [
        {
          type: 'unknown_type',
          text: 'Fallback text content',
        } as any,
      ];

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toEqual([
        {
          type: 'text',
          text: 'Fallback text content',
        },
      ]);
    });

    it('should return null for unexpected content type', () => {
      // Arrange
      const content = 123 as any; // Number instead of string or array

      // Act
      const result = convertMessageParamContent(content);

      // Assert
      expect(result).toBeNull();
    });
  });

  describe('convertMessageParamToMessage', () => {
    it('should convert simple text message param', () => {
      // Arrange
      const param: MessageParam = {
        role: 'user',
        content: 'Hello, assistant!',
      };

      // Act
      const result = convertMessageParamToMessage(param);

      // Assert
      expect(result).toMatchObject({
        role: 'user',
        content: 'Hello, assistant!',
        format: 'text',
      });
      expect(result.timestamp).toBeDefined();
      expect(new Date(result.timestamp!).getTime()).toBeCloseTo(Date.now(), -3);
    });

    it('should convert assistant message with array content', () => {
      // Arrange
      const param: MessageParam = {
        role: 'assistant',
        content: [
          { type: 'text', text: 'Let me help you with that.' } as TextBlockParam,
        ],
      };

      // Act
      const result = convertMessageParamToMessage(param);

      // Assert
      expect(result).toMatchObject({
        role: 'assistant',
        content: [
          { type: 'text', text: 'Let me help you with that.' },
        ],
        format: 'text',
      });
      expect(result.timestamp).toBeDefined();
    });

    it('should handle null content', () => {
      // Arrange
      const param: MessageParam = {
        role: 'system',
        content: null as any,
      };

      // Act
      const result = convertMessageParamToMessage(param);

      // Assert
      expect(result).toMatchObject({
        role: 'system',
        content: null,
        format: 'text',
      });
    });

    it('should handle empty string content', () => {
      // Arrange
      const param: MessageParam = {
        role: 'user',
        content: '',
      };

      // Act
      const result = convertMessageParamToMessage(param);

      // Assert
      expect(result).toMatchObject({
        role: 'user',
        content: '',
        format: 'text',
      });
    });
  });

  describe('convertMessageParamsToMessages', () => {
    it('should convert array of message params', () => {
      // Arrange
      const params: MessageParam[] = [
        { role: 'user', content: 'Question 1' },
        { role: 'assistant', content: 'Answer 1' },
        { role: 'user', content: 'Question 2' },
      ];

      // Act
      const result = convertMessageParamsToMessages(params);

      // Assert
      expect(result).toHaveLength(3);
      expect(result[0]).toMatchObject({ role: 'user', content: 'Question 1' });
      expect(result[1]).toMatchObject({ role: 'assistant', content: 'Answer 1' });
      expect(result[2]).toMatchObject({ role: 'user', content: 'Question 2' });
    });

    it('should handle empty array', () => {
      // Arrange
      const params: MessageParam[] = [];

      // Act
      const result = convertMessageParamsToMessages(params);

      // Assert
      expect(result).toEqual([]);
    });

    it('should convert mixed content types', () => {
      // Arrange
      const params: MessageParam[] = [
        { role: 'user', content: 'Simple text' },
        {
          role: 'assistant',
          content: [
            { type: 'text', text: 'Complex response' } as TextBlockParam,
            {
              type: 'tool_use',
              id: 'tool-1',
              name: 'search',
              input: { query: 'test' },
            } as ToolUseBlockParam,
          ],
        },
      ];

      // Act
      const result = convertMessageParamsToMessages(params);

      // Assert
      expect(result).toHaveLength(2);
      expect(result[0].content).toBe('Simple text');
      expect(result[1].content).toEqual([
        { type: 'text', text: 'Complex response' },
        { type: 'tool_use', id: 'tool-1', name: 'search', input: { query: 'test' } },
      ]);
    });
  });

  describe('ensureMessageFormat', () => {
    it('should return valid message as-is with string content', () => {
      // Arrange
      const message: Message = {
        role: 'user',
        content: 'Test message',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'text',
      };

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result).toEqual(message);
    });

    it('should add missing timestamp', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: 'Response',
      };

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result.timestamp).toBeDefined();
      expect(new Date(result.timestamp!).getTime()).toBeCloseTo(Date.now(), -3);
    });

    it('should add missing format', () => {
      // Arrange
      const message = {
        role: 'user' as const,
        content: 'Question',
        timestamp: '2025-01-01T10:00:00.000Z',
      };

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result.format).toBe('text');
    });

    it('should normalize array content with extra fields', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: [
          {
            type: 'text',
            text: 'Normalized text',
            cache_control: { type: 'ephemeral' }, // Should be stripped
            citations: [], // Should be stripped
          } as any,
        ],
      };

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result.content).toEqual([
        {
          type: 'text',
          text: 'Normalized text',
        },
      ]);
    });

    it('should convert MessageParam-like object', () => {
      // Arrange
      const messageParam = {
        role: 'user' as const,
        content: 'Convert me',
        // Missing timestamp and format
      };

      // Act
      const result = ensureMessageFormat(messageParam);

      // Assert
      expect(result).toMatchObject({
        role: 'user',
        content: 'Convert me',
        format: 'text',
      });
      expect(result.timestamp).toBeDefined();
    });

    it('should handle invalid message format', () => {
      // Arrange
      const invalidMessage = {
        notARole: 'invalid',
        notContent: 'bad',
      };

      // Act
      const result = ensureMessageFormat(invalidMessage);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '[Invalid message format]',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should handle null input', () => {
      // Arrange
      const message = null;

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '[Invalid message format]',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should handle undefined input', () => {
      // Arrange
      const message = undefined;

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '[Invalid message format]',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should handle non-object input', () => {
      // Arrange
      const message = 'not an object';

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result).toEqual({
        role: 'system',
        content: '[Invalid message format]',
        timestamp: expect.any(String),
        format: 'text',
      });
    });

    it('should preserve existing properties while ensuring format', () => {
      // Arrange
      const message = {
        role: 'assistant' as const,
        content: 'Preserved content',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'markdown' as const,
        citations: [{ quote: 'test', source: 'example.com' }],
        customField: 'preserved',
      };

      // Act
      const result = ensureMessageFormat(message);

      // Assert
      expect(result).toMatchObject({
        role: 'assistant',
        content: 'Preserved content',
        timestamp: '2025-01-01T10:00:00.000Z',
        format: 'markdown',
      });
    });
  });

  describe('ensureMessagesFormat', () => {
    it('should convert array of messages', () => {
      // Arrange
      const messages = [
        { role: 'user' as const, content: 'Message 1' },
        { role: 'assistant' as const, content: 'Message 2' },
        { invalid: 'message' },
      ];

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toHaveLength(3);
      expect(result[0]).toMatchObject({ role: 'user', content: 'Message 1' });
      expect(result[1]).toMatchObject({ role: 'assistant', content: 'Message 2' });
      expect(result[2]).toMatchObject({ role: 'system', content: '[Invalid message format]' });
    });

    it('should handle empty array', () => {
      // Arrange
      const messages: any[] = [];

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for non-array input', () => {
      // Arrange
      const messages = 'not an array' as any;

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for null input', () => {
      // Arrange
      const messages = null as any;

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should return empty array for undefined input', () => {
      // Arrange
      const messages = undefined as any;

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toEqual([]);
    });

    it('should handle messages from protocol fixtures', () => {
      // Arrange
      const messages = [
        messageFixtures.textMessage,
        messageFixtures.multimodalMessage,
        messageFixtures.assistantWithCitations,
      ];

      // Act
      const result = ensureMessagesFormat(messages);

      // Assert
      expect(result).toHaveLength(3);
      expect(result[0]).toMatchObject({ role: 'user', content: 'Simple text message' });
      expect(result[1].content).toBeInstanceOf(Array);
      expect(result[2]).toMatchObject({ role: 'assistant', content: 'Here is the answer' });
    });
  });
});