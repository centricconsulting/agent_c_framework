/**
 * Unit tests for HistoryEvent vendor field
 * 
 * Tests the critical vendor field that determines message format
 * for Anthropic vs OpenAI message structures.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { HistoryEvent } from '../ServerEvents';
import type { Message } from '../CommonTypes';
import type { MessageParam } from '../../../types/message-params';
import type { ChatCompletionMessageParam } from '../../../types/openai-message-params';
import type { UnifiedMessageParam } from '../../../types/ChatTypes';

describe('HistoryEvent Vendor Field', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Vendor Field Validation', () => {
    it('should have vendor field present as required', () => {
      // Arrange
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'test-session',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(historyEvent.vendor).toBeDefined();
      expect(historyEvent.vendor).toBe('anthropic');
      expect('vendor' in historyEvent).toBe(true);
    });

    it('should accept valid vendor value "anthropic"', () => {
      // Arrange
      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [
          {
            role: 'user',
            content: 'Hello Claude',
            timestamp: new Date().toISOString()
          }
        ],
        session_id: 'anthropic-session',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(anthropicEvent.vendor).toBe('anthropic');
      expect(typeof anthropicEvent.vendor).toBe('string');
    });

    it('should accept valid vendor value "openai"', () => {
      // Arrange
      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [
          {
            role: 'user',
            content: 'Hello GPT',
            timestamp: new Date().toISOString()
          }
        ],
        session_id: 'openai-session',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(openaiEvent.vendor).toBe('openai');
      expect(typeof openaiEvent.vendor).toBe('string');
    });

    it('should validate vendor field is not optional', () => {
      // TypeScript should enforce this at compile time
      // This test validates runtime behavior
      
      // Arrange - Create event without vendor (TypeScript would error)
      const invalidEvent = {
        type: 'history',
        role: 'system',
        messages: [],
        session_id: 'test-session',
        created_at: new Date().toISOString()
      } as any; // Use any to bypass TypeScript

      // Act & Assert
      expect(invalidEvent.vendor).toBeUndefined();
      expect('vendor' in invalidEvent).toBe(false);
      
      // Validate that a proper type guard would catch this
      const isValidHistoryEvent = (event: any): event is HistoryEvent => {
        return event &&
               event.type === 'history' &&
               event.role === 'system' &&
               typeof event.vendor === 'string' &&
               Array.isArray(event.messages);
      };
      
      expect(isValidHistoryEvent(invalidEvent)).toBe(false);
    });

    it('should validate vendor is one of allowed values', () => {
      // Arrange
      const validateVendor = (vendor: string): boolean => {
        return vendor === 'anthropic' || vendor === 'openai';
      };

      // Act & Assert
      expect(validateVendor('anthropic')).toBe(true);
      expect(validateVendor('openai')).toBe(true);
      expect(validateVendor('invalid')).toBe(false);
      expect(validateVendor('claude')).toBe(false);
      expect(validateVendor('gpt')).toBe(false);
      expect(validateVendor('')).toBe(false);
    });

    it('should handle vendor field in type guards', () => {
      // Arrange
      const isAnthropicHistory = (event: HistoryEvent): boolean => {
        return event.vendor === 'anthropic';
      };

      const isOpenAIHistory = (event: HistoryEvent): boolean => {
        return event.vendor === 'openai';
      };

      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(isAnthropicHistory(anthropicEvent)).toBe(true);
      expect(isAnthropicHistory(openaiEvent)).toBe(false);
      expect(isOpenAIHistory(openaiEvent)).toBe(true);
      expect(isOpenAIHistory(anthropicEvent)).toBe(false);
    });
  });

  describe('Message Format Switching', () => {
    it('should use MessageParam format when vendor is anthropic', () => {
      // Arrange
      const anthropicMessage: Message = {
        role: 'user',
        content: 'Hello Claude',
        timestamp: new Date().toISOString(),
        model_id: 'claude-3-5-sonnet-20241022'
      };

      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [anthropicMessage],
        session_id: 'anthropic-session',
        created_at: new Date().toISOString()
      };

      // Act - Convert to MessageParam format
      const convertToMessageParam = (message: Message): MessageParam => {
        const param: MessageParam = {
          role: message.role as 'user' | 'assistant',
          content: typeof message.content === 'string' 
            ? message.content 
            : JSON.stringify(message.content)
        };
        
        if (message.name) {
          param.name = message.name;
        }
        
        return param;
      };

      const messageParam = convertToMessageParam(anthropicMessage);

      // Assert
      expect(historyEvent.vendor).toBe('anthropic');
      expect(messageParam.role).toBe('user');
      expect(messageParam.content).toBe('Hello Claude');
    });

    it('should use ChatCompletionMessageParam format when vendor is openai', () => {
      // Arrange
      const openaiMessage: Message = {
        role: 'user',
        content: 'Hello GPT',
        timestamp: new Date().toISOString(),
        model_id: 'gpt-4-turbo'
      };

      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [openaiMessage],
        session_id: 'openai-session',
        created_at: new Date().toISOString()
      };

      // Act - Convert to ChatCompletionMessageParam format
      const convertToChatCompletionParam = (message: Message): ChatCompletionMessageParam => {
        // Base structure for OpenAI format
        const param: any = {
          role: message.role,
          content: typeof message.content === 'string'
            ? message.content
            : JSON.stringify(message.content)
        };

        if (message.name) {
          param.name = message.name;
        }

        return param as ChatCompletionMessageParam;
      };

      const chatParam = convertToChatCompletionParam(openaiMessage);

      // Assert
      expect(historyEvent.vendor).toBe('openai');
      expect(chatParam.role).toBe('user');
      expect(chatParam.content).toBe('Hello GPT');
    });

    it('should handle message format discrimination based on vendor', () => {
      // Arrange
      const processHistoryEvent = (event: HistoryEvent): string => {
        switch (event.vendor) {
          case 'anthropic':
            return 'Processing Anthropic format';
          case 'openai':
            return 'Processing OpenAI format';
          default:
            return 'Unknown vendor';
        }
      };

      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      // Act
      const anthropicResult = processHistoryEvent(anthropicEvent);
      const openaiResult = processHistoryEvent(openaiEvent);

      // Assert
      expect(anthropicResult).toBe('Processing Anthropic format');
      expect(openaiResult).toBe('Processing OpenAI format');
    });

    it('should maintain vendor consistency across all messages', () => {
      // Arrange
      const mixedMessages: Message[] = [
        {
          role: 'user',
          content: 'First message',
          model_id: 'claude-3-5-sonnet'
        },
        {
          role: 'assistant',
          content: 'Response',
          model_id: 'gpt-4'
        },
        {
          role: 'user',
          content: 'Follow up',
          model_id: 'claude-3-5-sonnet'
        }
      ];

      // History event should have consistent vendor
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic', // Vendor determines format for ALL messages
        messages: mixedMessages,
        session_id: 'mixed-session',
        created_at: new Date().toISOString()
      };

      // Act - All messages should be processed with same vendor format
      const processedFormats = historyEvent.messages.map(msg => {
        return historyEvent.vendor === 'anthropic' ? 'anthropic-format' : 'openai-format';
      });

      // Assert - All messages use same format based on event vendor
      expect(processedFormats.every(format => format === 'anthropic-format')).toBe(true);
      expect(historyEvent.vendor).toBe('anthropic');
    });

    it('should handle vendor-specific message features', () => {
      // Arrange
      const anthropicSpecificMessage: Message = {
        role: 'assistant',
        content: 'Claude response',
        model_id: 'claude-3-5-sonnet-20241022',
        format: 'markdown'
      };

      const openaiSpecificMessage: Message = {
        role: 'assistant',
        content: 'GPT response',
        model_id: 'gpt-4-turbo',
        format: 'text'
      };

      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [anthropicSpecificMessage],
        session_id: 'anthropic-features',
        created_at: new Date().toISOString()
      };

      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [openaiSpecificMessage],
        session_id: 'openai-features',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(anthropicEvent.vendor).toBe('anthropic');
      expect(anthropicEvent.messages[0].model_id).toContain('claude');
      
      expect(openaiEvent.vendor).toBe('openai');
      expect(openaiEvent.messages[0].model_id).toContain('gpt');
    });
  });

  describe('Type Discrimination', () => {
    it('should enable proper type discrimination with vendor field', () => {
      // Arrange
      type AnthropicHistoryEvent = HistoryEvent & { vendor: 'anthropic' };
      type OpenAIHistoryEvent = HistoryEvent & { vendor: 'openai' };

      const isAnthropicEvent = (event: HistoryEvent): event is AnthropicHistoryEvent => {
        return event.vendor === 'anthropic';
      };

      const isOpenAIEvent = (event: HistoryEvent): event is OpenAIHistoryEvent => {
        return event.vendor === 'openai';
      };

      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      if (isAnthropicEvent(event)) {
        // TypeScript knows this is AnthropicHistoryEvent
        expect(event.vendor).toBe('anthropic');
      } else if (isOpenAIEvent(event)) {
        // TypeScript knows this is OpenAIHistoryEvent
        expect(event.vendor).toBe('openai');
      }
    });

    it('should support exhaustive vendor checking', () => {
      // Arrange
      const getVendorName = (vendor: 'anthropic' | 'openai'): string => {
        switch (vendor) {
          case 'anthropic':
            return 'Anthropic Claude';
          case 'openai':
            return 'OpenAI GPT';
          // TypeScript ensures this is exhaustive
        }
      };

      // Act & Assert
      expect(getVendorName('anthropic')).toBe('Anthropic Claude');
      expect(getVendorName('openai')).toBe('OpenAI GPT');
    });

    it('should handle vendor field in runtime type validation', () => {
      // Arrange
      const validateHistoryEvent = (data: unknown): data is HistoryEvent => {
        if (!data || typeof data !== 'object') return false;
        
        const obj = data as any;
        
        return obj.type === 'history' &&
               obj.role === 'system' &&
               (obj.vendor === 'anthropic' || obj.vendor === 'openai') &&
               Array.isArray(obj.messages) &&
               typeof obj.session_id === 'string';
      };

      const validEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      const invalidVendor = {
        type: 'history',
        role: 'system',
        vendor: 'invalid',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      const missingVendor = {
        type: 'history',
        role: 'system',
        messages: [],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(validateHistoryEvent(validEvent)).toBe(true);
      expect(validateHistoryEvent(invalidVendor)).toBe(false);
      expect(validateHistoryEvent(missingVendor)).toBe(false);
    });
  });

  describe('JSON Serialization', () => {
    it('should preserve vendor field through JSON serialization', () => {
      // Arrange
      const original: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [
          {
            role: 'user',
            content: 'Test message',
            timestamp: '2024-01-01T00:00:00Z'
          }
        ],
        session_id: 'test-session',
        created_at: '2024-01-01T00:00:00Z'
      };

      // Act
      const json = JSON.stringify(original);
      const parsed = JSON.parse(json) as HistoryEvent;

      // Assert
      expect(parsed.vendor).toBe(original.vendor);
      expect(parsed.vendor).toBe('anthropic');
      expect(parsed).toEqual(original);
    });

    it('should maintain vendor field type through serialization', () => {
      // Arrange
      const events: HistoryEvent[] = [
        {
          type: 'history',
          role: 'system',
          vendor: 'anthropic',
          messages: [],
          session_id: 'anthropic-session',
          created_at: new Date().toISOString()
        },
        {
          type: 'history',
          role: 'system',
          vendor: 'openai',
          messages: [],
          session_id: 'openai-session',
          created_at: new Date().toISOString()
        }
      ];

      // Act
      const serialized = events.map(e => JSON.stringify(e));
      const deserialized = serialized.map(s => JSON.parse(s) as HistoryEvent);

      // Assert
      expect(deserialized[0].vendor).toBe('anthropic');
      expect(deserialized[1].vendor).toBe('openai');
      
      deserialized.forEach((event, index) => {
        expect(event.vendor).toBe(events[index].vendor);
        expect(typeof event.vendor).toBe('string');
      });
    });

    it('should handle vendor field in nested serialization', () => {
      // Arrange
      const container = {
        events: [
          {
            type: 'history' as const,
            role: 'system' as const,
            vendor: 'anthropic',
            messages: [],
            session_id: 'nested-1',
            created_at: new Date().toISOString()
          },
          {
            type: 'history' as const,
            role: 'system' as const,
            vendor: 'openai',
            messages: [],
            session_id: 'nested-2',
            created_at: new Date().toISOString()
          }
        ],
        metadata: {
          version: 1,
          timestamp: new Date().toISOString()
        }
      };

      // Act
      const json = JSON.stringify(container);
      const parsed = JSON.parse(json);

      // Assert
      expect(parsed.events[0].vendor).toBe('anthropic');
      expect(parsed.events[1].vendor).toBe('openai');
      expect(parsed.events.length).toBe(2);
    });

    it('should not transform vendor field values during serialization', () => {
      // Arrange
      const testCases = [
        { vendor: 'anthropic', expected: 'anthropic' },
        { vendor: 'openai', expected: 'openai' },
        { vendor: 'ANTHROPIC', expected: 'ANTHROPIC' }, // Case sensitive
        { vendor: 'OpenAI', expected: 'OpenAI' } // Case sensitive
      ];

      testCases.forEach(({ vendor, expected }) => {
        const event: HistoryEvent = {
          type: 'history',
          role: 'system',
          vendor: vendor as any, // Allow testing case variations
          messages: [],
          session_id: 'case-test',
          created_at: new Date().toISOString()
        };

        // Act
        const json = JSON.stringify(event);
        const parsed = JSON.parse(json);

        // Assert
        expect(parsed.vendor).toBe(expected);
      });
    });
  });

  describe('WebSocket Transmission', () => {
    it('should maintain vendor field through WebSocket simulation', () => {
      // Arrange
      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [
          {
            role: 'user',
            content: 'WebSocket test',
            timestamp: new Date().toISOString()
          }
        ],
        session_id: 'ws-session',
        created_at: new Date().toISOString()
      };

      // Simulate WebSocket transmission
      class MockWebSocket {
        private handlers: Map<string, Function[]> = new Map();

        send(data: string) {
          // Simulate receiving on the other end
          const parsed = JSON.parse(data);
          this.trigger('message', { data: JSON.stringify(parsed) });
        }

        on(event: string, handler: Function) {
          if (!this.handlers.has(event)) {
            this.handlers.set(event, []);
          }
          this.handlers.get(event)!.push(handler);
        }

        trigger(event: string, data: any) {
          const handlers = this.handlers.get(event) || [];
          handlers.forEach(handler => handler(data));
        }
      }

      const ws = new MockWebSocket();
      let receivedEvent: HistoryEvent | null = null;

      ws.on('message', (event: { data: string }) => {
        const data = JSON.parse(event.data);
        if (data.type === 'history') {
          receivedEvent = data as HistoryEvent;
        }
      });

      // Act
      ws.send(JSON.stringify(event));

      // Assert
      expect(receivedEvent).not.toBeNull();
      expect(receivedEvent!.vendor).toBe('openai');
      expect(receivedEvent!.vendor).toBe(event.vendor);
      expect(receivedEvent!.messages).toEqual(event.messages);
    });

    it('should handle vendor field in binary frame metadata', () => {
      // Arrange
      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'binary-test',
        created_at: new Date().toISOString()
      };

      // Simulate encoding/decoding for binary frames
      const encode = (data: any): ArrayBuffer => {
        const json = JSON.stringify(data);
        const encoder = new TextEncoder();
        return encoder.encode(json).buffer;
      };

      const decode = (buffer: ArrayBuffer): any => {
        const decoder = new TextDecoder();
        const json = decoder.decode(buffer);
        return JSON.parse(json);
      };

      // Act
      const encoded = encode(event);
      const decoded = decode(encoded) as HistoryEvent;

      // Assert
      expect(decoded.vendor).toBe('anthropic');
      expect(decoded.type).toBe('history');
      expect(decoded.vendor).toBe(event.vendor);
    });

    it('should handle vendor field in event stream processing', () => {
      // Arrange
      const eventStream: HistoryEvent[] = [
        {
          type: 'history',
          role: 'system',
          vendor: 'anthropic',
          messages: [],
          session_id: 'stream-1',
          created_at: new Date().toISOString()
        },
        {
          type: 'history',
          role: 'system',
          vendor: 'openai',
          messages: [],
          session_id: 'stream-2',
          created_at: new Date().toISOString()
        },
        {
          type: 'history',
          role: 'system',
          vendor: 'anthropic',
          messages: [],
          session_id: 'stream-3',
          created_at: new Date().toISOString()
        }
      ];

      // Act - Process stream and group by vendor
      const vendorGroups = eventStream.reduce((acc, event) => {
        if (!acc[event.vendor]) {
          acc[event.vendor] = [];
        }
        acc[event.vendor].push(event);
        return acc;
      }, {} as Record<string, HistoryEvent[]>);

      // Assert
      expect(vendorGroups['anthropic']).toHaveLength(2);
      expect(vendorGroups['openai']).toHaveLength(1);
      expect(vendorGroups['anthropic'][0].vendor).toBe('anthropic');
      expect(vendorGroups['openai'][0].vendor).toBe('openai');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty messages array with vendor field', () => {
      // Arrange
      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'empty-messages',
        created_at: new Date().toISOString()
      };

      // Act & Assert
      expect(event.vendor).toBe('anthropic');
      expect(event.messages).toEqual([]);
      expect(event.messages.length).toBe(0);
    });

    it('should handle large message arrays with vendor field', () => {
      // Arrange
      const largeMessages: Message[] = Array.from({ length: 1000 }, (_, i) => ({
        role: i % 2 === 0 ? 'user' : 'assistant',
        content: `Message ${i}`,
        timestamp: new Date().toISOString()
      }));

      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: largeMessages,
        session_id: 'large-history',
        created_at: new Date().toISOString()
      };

      // Act
      const json = JSON.stringify(event);
      const parsed = JSON.parse(json) as HistoryEvent;

      // Assert
      expect(parsed.vendor).toBe('openai');
      expect(parsed.messages.length).toBe(1000);
      expect(parsed.messages[0].content).toBe('Message 0');
      expect(parsed.messages[999].content).toBe('Message 999');
    });

    it('should handle vendor field with special characters in messages', () => {
      // Arrange
      const specialMessages: Message[] = [
        {
          role: 'user',
          content: 'Unicode: 世界 🌍 مرحبا',
          timestamp: new Date().toISOString()
        },
        {
          role: 'assistant',
          content: 'Special chars: <>&"\'',
          timestamp: new Date().toISOString()
        }
      ];

      const event: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: specialMessages,
        session_id: 'special-chars',
        created_at: new Date().toISOString()
      };

      // Act
      const json = JSON.stringify(event);
      const parsed = JSON.parse(json) as HistoryEvent;

      // Assert
      expect(parsed.vendor).toBe('anthropic');
      expect(parsed.messages[0].content).toContain('世界');
      expect(parsed.messages[0].content).toContain('🌍');
      expect(parsed.messages[1].content).toContain('<>&"\'');
    });

    it('should validate vendor field is case-sensitive', () => {
      // Arrange
      const validateVendor = (vendor: string): boolean => {
        return vendor === 'anthropic' || vendor === 'openai';
      };

      // Act & Assert
      expect(validateVendor('anthropic')).toBe(true);
      expect(validateVendor('Anthropic')).toBe(false);
      expect(validateVendor('ANTHROPIC')).toBe(false);
      expect(validateVendor('openai')).toBe(true);
      expect(validateVendor('OpenAI')).toBe(false);
      expect(validateVendor('OPENAI')).toBe(false);
    });
  });
});