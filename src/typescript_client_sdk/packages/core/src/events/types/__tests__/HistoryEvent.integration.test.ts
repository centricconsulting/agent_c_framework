/**
 * Integration tests for HistoryEvent vendor field
 * 
 * Tests how the vendor field works throughout the system
 * for proper message format handling in multi-vendor scenarios.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { HistoryEvent } from '../ServerEvents';
import type { Message } from '../CommonTypes';
import type { ChatSession, ServerChatSession } from '../../../types/chat-session';
import type { MessageParam } from '../../../types/message-params';
import type { ChatCompletionMessageParam } from '../../../types/openai-message-params';
import type { UnifiedMessageParam } from '../../../types/ChatTypes';
import { EventEmitter } from '../../EventEmitter';
import { 
  getMessageFormat, 
  isAnthropicMessage, 
  isOpenAIMessage,
  toAnthropicMessage,
  toOpenAIMessage
} from '../../../types/ChatTypes';

// Mock message converter for testing
class MessageFormatConverter {
  private vendor: 'anthropic' | 'openai';

  constructor(vendor: 'anthropic' | 'openai') {
    this.vendor = vendor;
  }

  convertMessage(message: Message): UnifiedMessageParam {
    if (this.vendor === 'anthropic') {
      return this.toAnthropicFormat(message);
    } else {
      return this.toOpenAIFormat(message);
    }
  }

  private toAnthropicFormat(message: Message): MessageParam {
    const param: MessageParam = {
      role: message.role as 'user' | 'assistant',
      content: typeof message.content === 'string' 
        ? message.content 
        : Array.isArray(message.content) 
          ? message.content 
          : [{ type: 'text', text: JSON.stringify(message.content) }]
    };

    if (message.name) {
      param.name = message.name;
    }

    return param;
  }

  private toOpenAIFormat(message: Message): ChatCompletionMessageParam {
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
  }

  convertHistory(event: HistoryEvent): UnifiedMessageParam[] {
    return event.messages.map(msg => this.convertMessage(msg));
  }
}

// Mock chat session manager for testing
class ChatSessionManager {
  private sessions: Map<string, { session: ChatSession; vendor: 'anthropic' | 'openai' }> = new Map();
  
  addSession(session: ChatSession, vendor: 'anthropic' | 'openai') {
    this.sessions.set(session.session_id, { session, vendor });
  }

  getHistoryEvent(sessionId: string): HistoryEvent | null {
    const data = this.sessions.get(sessionId);
    if (!data) return null;

    return {
      type: 'history',
      role: 'system',
      vendor: data.vendor,
      messages: data.session.messages,
      session_id: sessionId,
      created_at: data.session.created_at || new Date().toISOString()
    };
  }

  processHistoryWithVendor(event: HistoryEvent): { 
    vendor: string; 
    format: string; 
    messageCount: number 
  } {
    const format = event.vendor === 'anthropic' ? 'MessageParam' : 'ChatCompletionMessageParam';
    
    return {
      vendor: event.vendor,
      format,
      messageCount: event.messages.length
    };
  }
}

describe('HistoryEvent Integration', () => {
  let emitter: EventEmitter<Record<string, any>>;
  let sessionManager: ChatSessionManager;

  beforeEach(() => {
    vi.clearAllMocks();
    emitter = new EventEmitter();
    sessionManager = new ChatSessionManager();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Message Format Switching Integration', () => {
    it('should correctly process Anthropic format messages', () => {
      // Arrange
      const anthropicSession: ChatSession = {
        version: 1,
        session_id: 'anthropic-session',
        token_count: 0,
        context_window_size: 0,
        messages: [
          {
            role: 'user',
            content: 'Hello Claude',
            timestamp: new Date().toISOString(),
            model_id: 'claude-3-5-sonnet-20241022'
          },
          {
            role: 'assistant',
            content: 'Hello! How can I help you today?',
            timestamp: new Date().toISOString(),
            model_id: 'claude-3-5-sonnet-20241022'
          }
        ],
        vendor: 'anthropic',
        display_name: 'Claude Chat',
        created_at: new Date().toISOString()
      };

      sessionManager.addSession(anthropicSession, 'anthropic');
      const historyEvent = sessionManager.getHistoryEvent('anthropic-session');

      // Act
      const converter = new MessageFormatConverter('anthropic');
      const convertedMessages = converter.convertHistory(historyEvent!);

      // Assert
      expect(historyEvent!.vendor).toBe('anthropic');
      expect(convertedMessages).toHaveLength(2);
      
      // Check that messages are converted to Anthropic format
      // The converter explicitly converts to Anthropic format when vendor is 'anthropic'
      const firstMessage = convertedMessages[0] as MessageParam;
      expect(firstMessage.role).toBe('user');
      expect(firstMessage.content).toBe('Hello Claude');
      
      // Verify the converter used the correct format
      expect(converter['vendor']).toBe('anthropic');
    });

    it('should correctly process OpenAI format messages', () => {
      // Arrange
      const openaiSession: ChatSession = {
        version: 1,
        session_id: 'openai-session',
        token_count: 0,
        context_window_size: 0,
        messages: [
          {
            role: 'user',
            content: 'Hello GPT',
            timestamp: new Date().toISOString(),
            model_id: 'gpt-4-turbo'
          },
          {
            role: 'assistant',
            content: 'Hello! How can I assist you?',
            timestamp: new Date().toISOString(),
            model_id: 'gpt-4-turbo'
          }
        ],
        vendor: 'openai',
        display_name: 'GPT Chat',
        created_at: new Date().toISOString()
      };

      sessionManager.addSession(openaiSession, 'openai');
      const historyEvent = sessionManager.getHistoryEvent('openai-session');

      // Act
      const converter = new MessageFormatConverter('openai');
      const convertedMessages = converter.convertHistory(historyEvent!);

      // Assert
      expect(historyEvent!.vendor).toBe('openai');
      expect(convertedMessages).toHaveLength(2);
      
      // Check format detection
      const format = getMessageFormat(historyEvent!.vendor);
      expect(format).toBe('openai');

      const firstMessage = convertedMessages[0] as ChatCompletionMessageParam;
      expect(firstMessage.role).toBe('user');
      expect(firstMessage.content).toBe('Hello GPT');
    });

    it('should handle vendor switching in multi-session scenario', () => {
      // Arrange
      const sessions = [
        {
          id: 'claude-1',
          vendor: 'anthropic' as const,
          messages: [{ role: 'user' as const, content: 'Claude test 1' }]
        },
        {
          id: 'gpt-1',
          vendor: 'openai' as const,
          messages: [{ role: 'user' as const, content: 'GPT test 1' }]
        },
        {
          id: 'claude-2',
          vendor: 'anthropic' as const,
          messages: [{ role: 'user' as const, content: 'Claude test 2' }]
        }
      ];

      const historyEvents: HistoryEvent[] = sessions.map(s => ({
        type: 'history',
        role: 'system',
        vendor: s.vendor,
        messages: s.messages.map(m => ({
          ...m,
          timestamp: new Date().toISOString()
        })),
        session_id: s.id,
        created_at: new Date().toISOString()
      }));

      // Act - Process each event with appropriate converter
      const processedEvents = historyEvents.map(event => {
        const converter = new MessageFormatConverter(event.vendor as 'anthropic' | 'openai');
        const converted = converter.convertHistory(event);
        
        // Check format based on converter vendor, not message content
        // Since both formats can have 'user' role, we check based on the converter used
        const formatUsed = converter['vendor']; // Access private field for testing
        
        return {
          sessionId: event.session_id,
          vendor: event.vendor,
          formatUsed,
          messageCount: converted.length
        };
      });

      // Assert
      expect(processedEvents[0].vendor).toBe('anthropic');
      expect(processedEvents[0].formatUsed).toBe('anthropic');
      expect(processedEvents[0].messageCount).toBe(1);

      expect(processedEvents[1].vendor).toBe('openai');
      expect(processedEvents[1].formatUsed).toBe('openai');
      expect(processedEvents[1].messageCount).toBe(1);

      expect(processedEvents[2].vendor).toBe('anthropic');
      expect(processedEvents[2].formatUsed).toBe('anthropic');
      expect(processedEvents[2].messageCount).toBe(1);
    });
  });

  describe('Event Flow Integration', () => {
    it('should emit history events with correct vendor', () => {
      // Arrange
      const historyEvents: HistoryEvent[] = [];
      
      emitter.on('history', (event: HistoryEvent) => {
        historyEvents.push(event);
      });

      // Act - Emit different vendor events
      emitter.emit('history', {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'anthropic-emit',
        created_at: new Date().toISOString()
      });

      emitter.emit('history', {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [],
        session_id: 'openai-emit',
        created_at: new Date().toISOString()
      });

      // Assert
      expect(historyEvents).toHaveLength(2);
      expect(historyEvents[0].vendor).toBe('anthropic');
      expect(historyEvents[1].vendor).toBe('openai');
    });

    it('should handle vendor field in event routing', () => {
      // Arrange
      const anthropicEvents: HistoryEvent[] = [];
      const openaiEvents: HistoryEvent[] = [];

      // Router that separates events by vendor
      emitter.on('history', (event: HistoryEvent) => {
        if (event.vendor === 'anthropic') {
          anthropicEvents.push(event);
        } else if (event.vendor === 'openai') {
          openaiEvents.push(event);
        }
      });

      // Act - Emit mixed vendor events
      for (let i = 0; i < 10; i++) {
        const vendor = i % 3 === 0 ? 'anthropic' : 'openai';
        emitter.emit('history', {
          type: 'history',
          role: 'system',
          vendor,
          messages: [],
          session_id: `session-${i}`,
          created_at: new Date().toISOString()
        });
      }

      // Assert
      expect(anthropicEvents).toHaveLength(4); // 0, 3, 6, 9
      expect(openaiEvents).toHaveLength(6); // 1, 2, 4, 5, 7, 8
      
      anthropicEvents.forEach(event => {
        expect(event.vendor).toBe('anthropic');
      });
      
      openaiEvents.forEach(event => {
        expect(event.vendor).toBe('openai');
      });
    });

    it('should maintain vendor through event chain', () => {
      // Arrange
      let finalVendor: string | null = null;
      
      // Chain of event handlers
      emitter.on('history', (event: HistoryEvent) => {
        emitter.emit('history:processed', {
          ...event,
          processed: true
        });
      });

      emitter.on('history:processed', (event: HistoryEvent & { processed: boolean }) => {
        emitter.emit('history:final', {
          ...event,
          final: true
        });
      });

      emitter.on('history:final', (event: HistoryEvent & { processed: boolean; final: boolean }) => {
        finalVendor = event.vendor;
      });

      // Act
      emitter.emit('history', {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'chain-test',
        created_at: new Date().toISOString()
      });

      // Assert
      expect(finalVendor).toBe('anthropic');
    });
  });

  describe('Type Guards and Validation Integration', () => {
    it('should use vendor field for runtime type routing', () => {
      // Arrange
      class MessageProcessor {
        processHistoryEvent(event: HistoryEvent): string {
          // Runtime routing based on vendor
          switch (event.vendor) {
            case 'anthropic':
              return this.processAnthropicHistory(event);
            case 'openai':
              return this.processOpenAIHistory(event);
            default:
              throw new Error(`Unknown vendor: ${event.vendor}`);
          }
        }

        private processAnthropicHistory(event: HistoryEvent): string {
          return `Processing ${event.messages.length} Anthropic messages`;
        }

        private processOpenAIHistory(event: HistoryEvent): string {
          return `Processing ${event.messages.length} OpenAI messages`;
        }
      }

      const processor = new MessageProcessor();

      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [
          { role: 'user', content: 'test' }
        ],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [
          { role: 'user', content: 'test' },
          { role: 'assistant', content: 'response' }
        ],
        session_id: 'test',
        created_at: new Date().toISOString()
      };

      // Act
      const anthropicResult = processor.processHistoryEvent(anthropicEvent);
      const openaiResult = processor.processHistoryEvent(openaiEvent);

      // Assert
      expect(anthropicResult).toBe('Processing 1 Anthropic messages');
      expect(openaiResult).toBe('Processing 2 OpenAI messages');
    });

    it('should validate vendor field in data pipeline', () => {
      // Arrange
      class DataPipeline {
        private validVendors = new Set(['anthropic', 'openai']);

        validate(event: unknown): event is HistoryEvent {
          if (!event || typeof event !== 'object') return false;
          
          const obj = event as any;
          
          if (obj.type !== 'history') return false;
          if (obj.role !== 'system') return false;
          if (!this.validVendors.has(obj.vendor)) return false;
          if (!Array.isArray(obj.messages)) return false;
          if (typeof obj.session_id !== 'string') return false;
          
          return true;
        }

        process(data: unknown): HistoryEvent | null {
          if (this.validate(data)) {
            return data;
          }
          return null;
        }
      }

      const pipeline = new DataPipeline();

      // Test valid events
      const validEvents = [
        {
          type: 'history',
          role: 'system',
          vendor: 'anthropic',
          messages: [],
          session_id: 'valid-1',
          created_at: new Date().toISOString()
        },
        {
          type: 'history',
          role: 'system',
          vendor: 'openai',
          messages: [],
          session_id: 'valid-2',
          created_at: new Date().toISOString()
        }
      ];

      // Test invalid events
      const invalidEvents = [
        {
          type: 'history',
          role: 'system',
          vendor: 'invalid',
          messages: [],
          session_id: 'invalid-1'
        },
        {
          type: 'history',
          role: 'system',
          // missing vendor
          messages: [],
          session_id: 'invalid-2'
        },
        {
          type: 'history',
          role: 'system',
          vendor: 'claude', // Not in valid list
          messages: [],
          session_id: 'invalid-3'
        }
      ];

      // Act & Assert
      validEvents.forEach(event => {
        const result = pipeline.process(event);
        expect(result).not.toBeNull();
        expect(result!.vendor).toBe(event.vendor);
      });

      invalidEvents.forEach(event => {
        const result = pipeline.process(event);
        expect(result).toBeNull();
      });
    });
  });

  describe('WebSocket Protocol Integration', () => {
    it('should handle vendor field in WebSocket message flow', () => {
      // Arrange
      class WebSocketProtocol {
        private messageQueue: HistoryEvent[] = [];

        sendHistory(event: HistoryEvent): string {
          this.messageQueue.push(event);
          return JSON.stringify({
            type: 'history',
            data: event
          });
        }

        receiveHistory(message: string): HistoryEvent | null {
          try {
            const parsed = JSON.parse(message);
            if (parsed.type === 'history' && parsed.data) {
              const event = parsed.data as HistoryEvent;
              
              // Validate vendor field
              if (event.vendor !== 'anthropic' && event.vendor !== 'openai') {
                throw new Error(`Invalid vendor: ${event.vendor}`);
              }
              
              return event;
            }
          } catch (e) {
            return null;
          }
          return null;
        }

        getQueueByVendor(vendor: string): HistoryEvent[] {
          return this.messageQueue.filter(e => e.vendor === vendor);
        }
      }

      const protocol = new WebSocketProtocol();

      // Act - Send events
      const anthropicEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [],
        session_id: 'ws-anthropic',
        created_at: new Date().toISOString()
      };

      const openaiEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [],
        session_id: 'ws-openai',
        created_at: new Date().toISOString()
      };

      const anthropicMessage = protocol.sendHistory(anthropicEvent);
      const openaiMessage = protocol.sendHistory(openaiEvent);

      // Receive events
      const receivedAnthropic = protocol.receiveHistory(anthropicMessage);
      const receivedOpenAI = protocol.receiveHistory(openaiMessage);

      // Assert
      expect(receivedAnthropic).not.toBeNull();
      expect(receivedAnthropic!.vendor).toBe('anthropic');
      
      expect(receivedOpenAI).not.toBeNull();
      expect(receivedOpenAI!.vendor).toBe('openai');

      expect(protocol.getQueueByVendor('anthropic')).toHaveLength(1);
      expect(protocol.getQueueByVendor('openai')).toHaveLength(1);
    });

    it('should handle vendor field in reconnection scenarios', () => {
      // Arrange
      class ReconnectionHandler {
        private lastKnownState: Map<string, { vendor: string; messageCount: number }> = new Map();

        saveState(event: HistoryEvent) {
          this.lastKnownState.set(event.session_id, {
            vendor: event.vendor,
            messageCount: event.messages.length
          });
        }

        restoreState(sessionId: string): { vendor: string; messageCount: number } | null {
          return this.lastKnownState.get(sessionId) || null;
        }

        handleReconnection(sessionId: string, newEvent: HistoryEvent): boolean {
          const lastState = this.restoreState(sessionId);
          
          if (!lastState) {
            // First connection
            this.saveState(newEvent);
            return true;
          }

          // Validate vendor consistency
          if (lastState.vendor !== newEvent.vendor) {
            throw new Error(`Vendor mismatch: expected ${lastState.vendor}, got ${newEvent.vendor}`);
          }

          // Update state
          this.saveState(newEvent);
          return true;
        }
      }

      const handler = new ReconnectionHandler();

      // Act - Initial connection
      const initialEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [{ role: 'user', content: 'test' }],
        session_id: 'reconnect-test',
        created_at: new Date().toISOString()
      };

      handler.handleReconnection('reconnect-test', initialEvent);
      
      // Reconnection with same vendor
      const reconnectEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [
          { role: 'user', content: 'test' },
          { role: 'assistant', content: 'response' }
        ],
        session_id: 'reconnect-test',
        created_at: new Date().toISOString()
      };

      const reconnectSuccess = handler.handleReconnection('reconnect-test', reconnectEvent);

      // Assert
      expect(reconnectSuccess).toBe(true);
      
      const state = handler.restoreState('reconnect-test');
      expect(state).not.toBeNull();
      expect(state!.vendor).toBe('anthropic');
      expect(state!.messageCount).toBe(2);

      // Test vendor mismatch
      const mismatchEvent: HistoryEvent = {
        ...reconnectEvent,
        vendor: 'openai'
      };

      expect(() => {
        handler.handleReconnection('reconnect-test', mismatchEvent);
      }).toThrow('Vendor mismatch');
    });
  });

  describe('Session Management Integration', () => {
    it('should maintain vendor consistency in session lifecycle', () => {
      // Arrange
      class SessionLifecycle {
        private sessions: Map<string, HistoryEvent> = new Map();

        createSession(vendor: 'anthropic' | 'openai', sessionId: string): HistoryEvent {
          const event: HistoryEvent = {
            type: 'history',
            role: 'system',
            vendor,
            messages: [],
            session_id: sessionId,
            created_at: new Date().toISOString()
          };

          this.sessions.set(sessionId, event);
          return event;
        }

        addMessage(sessionId: string, message: Message): boolean {
          const event = this.sessions.get(sessionId);
          if (!event) return false;

          event.messages.push(message);
          return true;
        }

        getVendor(sessionId: string): string | null {
          const event = this.sessions.get(sessionId);
          return event ? event.vendor : null;
        }

        switchVendor(sessionId: string, newVendor: 'anthropic' | 'openai'): HistoryEvent {
          const oldEvent = this.sessions.get(sessionId);
          if (!oldEvent) {
            throw new Error(`Session ${sessionId} not found`);
          }

          // Create new session with new vendor
          const newSessionId = `${sessionId}-${newVendor}`;
          const newEvent = this.createSession(newVendor, newSessionId);
          
          // Copy messages (would need format conversion in real scenario)
          newEvent.messages = [...oldEvent.messages];
          
          return newEvent;
        }
      }

      const lifecycle = new SessionLifecycle();

      // Act
      const anthropicSession = lifecycle.createSession('anthropic', 'session-1');
      lifecycle.addMessage('session-1', {
        role: 'user',
        content: 'Hello Claude'
      });

      const vendor1 = lifecycle.getVendor('session-1');

      // Switch vendor
      const openaiSession = lifecycle.switchVendor('session-1', 'openai');
      const vendor2 = lifecycle.getVendor('session-1-openai');

      // Assert
      expect(vendor1).toBe('anthropic');
      expect(vendor2).toBe('openai');
      expect(anthropicSession.vendor).toBe('anthropic');
      expect(openaiSession.vendor).toBe('openai');
      expect(openaiSession.messages).toHaveLength(1);
    });

    it('should track vendor statistics across sessions', () => {
      // Arrange
      class VendorStats {
        private events: HistoryEvent[] = [];

        addEvent(event: HistoryEvent) {
          this.events.push(event);
        }

        getStats(): {
          total: number;
          byVendor: Record<string, number>;
          messagesByVendor: Record<string, number>;
        } {
          const stats = {
            total: this.events.length,
            byVendor: {} as Record<string, number>,
            messagesByVendor: {} as Record<string, number>
          };

          this.events.forEach(event => {
            // Count events by vendor
            if (!stats.byVendor[event.vendor]) {
              stats.byVendor[event.vendor] = 0;
              stats.messagesByVendor[event.vendor] = 0;
            }
            stats.byVendor[event.vendor]++;
            stats.messagesByVendor[event.vendor] += event.messages.length;
          });

          return stats;
        }

        getMostUsedVendor(): string | null {
          const stats = this.getStats();
          let maxCount = 0;
          let mostUsed: string | null = null;

          Object.entries(stats.byVendor).forEach(([vendor, count]) => {
            if (count > maxCount) {
              maxCount = count;
              mostUsed = vendor;
            }
          });

          return mostUsed;
        }
      }

      const stats = new VendorStats();

      // Act - Add various events
      for (let i = 0; i < 10; i++) {
        const vendor = i < 7 ? 'anthropic' : 'openai';
        const messageCount = i % 3 + 1;
        
        stats.addEvent({
          type: 'history',
          role: 'system',
          vendor,
          messages: Array.from({ length: messageCount }, (_, j) => ({
            role: j % 2 === 0 ? 'user' : 'assistant',
            content: `Message ${j}`
          })),
          session_id: `session-${i}`,
          created_at: new Date().toISOString()
        });
      }

      const vendorStats = stats.getStats();
      const mostUsed = stats.getMostUsedVendor();

      // Assert
      expect(vendorStats.total).toBe(10);
      expect(vendorStats.byVendor['anthropic']).toBe(7);
      expect(vendorStats.byVendor['openai']).toBe(3);
      expect(mostUsed).toBe('anthropic');
      
      // Check message counts
      const anthropicMessages = vendorStats.messagesByVendor['anthropic'];
      const openaiMessages = vendorStats.messagesByVendor['openai'];
      expect(anthropicMessages).toBeGreaterThan(0);
      expect(openaiMessages).toBeGreaterThan(0);
    });
  });

  describe('Error Handling Integration', () => {
    it('should handle invalid vendor gracefully', () => {
      // Arrange
      class VendorValidator {
        private validVendors = ['anthropic', 'openai'];

        validateEvent(event: any): { valid: boolean; error?: string } {
          if (!event.vendor) {
            return { valid: false, error: 'Missing vendor field' };
          }

          if (!this.validVendors.includes(event.vendor)) {
            return { valid: false, error: `Invalid vendor: ${event.vendor}` };
          }

          return { valid: true };
        }

        processEvent(event: any): HistoryEvent | null {
          const validation = this.validateEvent(event);
          
          if (!validation.valid) {
            console.error(`Validation failed: ${validation.error}`);
            return null;
          }

          return event as HistoryEvent;
        }
      }

      const validator = new VendorValidator();

      // Test cases
      const testCases = [
        {
          event: {
            type: 'history',
            role: 'system',
            vendor: 'anthropic',
            messages: [],
            session_id: 'valid',
            created_at: new Date().toISOString()
          },
          expectValid: true
        },
        {
          event: {
            type: 'history',
            role: 'system',
            vendor: 'invalid-vendor',
            messages: [],
            session_id: 'invalid',
            created_at: new Date().toISOString()
          },
          expectValid: false
        },
        {
          event: {
            type: 'history',
            role: 'system',
            // missing vendor
            messages: [],
            session_id: 'missing',
            created_at: new Date().toISOString()
          },
          expectValid: false
        }
      ];

      // Act & Assert
      testCases.forEach(({ event, expectValid }) => {
        const result = validator.processEvent(event);
        
        if (expectValid) {
          expect(result).not.toBeNull();
          expect(result!.vendor).toBe(event.vendor);
        } else {
          expect(result).toBeNull();
        }
      });
    });

    it('should handle vendor migration scenarios', () => {
      // Arrange
      class VendorMigration {
        migrateVendor(oldVendor: string): 'anthropic' | 'openai' | null {
          const migrations: Record<string, 'anthropic' | 'openai'> = {
            'claude': 'anthropic',
            'claude-3': 'anthropic',
            'claude-3.5': 'anthropic',
            'gpt': 'openai',
            'gpt-4': 'openai',
            'gpt-3.5': 'openai',
            'chatgpt': 'openai'
          };

          return migrations[oldVendor.toLowerCase()] || null;
        }

        migrateEvent(event: any): HistoryEvent | null {
          // Handle old format or invalid vendor
          if (!event.vendor || !['anthropic', 'openai'].includes(event.vendor)) {
            const migratedVendor = this.migrateVendor(event.vendor || '');
            
            if (!migratedVendor) {
              // Try to infer from model_id in messages
              if (event.messages && event.messages.length > 0) {
                const firstMessage = event.messages[0];
                if (firstMessage.model_id) {
                  const modelLower = firstMessage.model_id.toLowerCase();
                  if (modelLower.includes('claude')) {
                    event.vendor = 'anthropic';
                  } else if (modelLower.includes('gpt')) {
                    event.vendor = 'openai';
                  }
                }
              }
            } else {
              event.vendor = migratedVendor;
            }
          }

          // Final validation
          if (!['anthropic', 'openai'].includes(event.vendor)) {
            return null;
          }

          return event as HistoryEvent;
        }
      }

      const migration = new VendorMigration();

      // Test migration cases
      const oldFormatEvent = {
        type: 'history',
        role: 'system',
        vendor: 'claude-3',
        messages: [],
        session_id: 'old-format',
        created_at: new Date().toISOString()
      };

      const inferFromModelEvent = {
        type: 'history',
        role: 'system',
        vendor: 'unknown',
        messages: [{
          role: 'user',
          content: 'test',
          model_id: 'gpt-4-turbo'
        }],
        session_id: 'infer-model',
        created_at: new Date().toISOString()
      };

      // Act
      const migratedOld = migration.migrateEvent(oldFormatEvent);
      const migratedInfer = migration.migrateEvent(inferFromModelEvent);

      // Assert
      expect(migratedOld).not.toBeNull();
      expect(migratedOld!.vendor).toBe('anthropic');

      expect(migratedInfer).not.toBeNull();
      expect(migratedInfer!.vendor).toBe('openai');
    });
  });
});