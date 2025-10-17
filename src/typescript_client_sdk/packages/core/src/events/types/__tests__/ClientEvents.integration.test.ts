/**
 * Integration tests for Client Events with EventEmitter and WebSocketManager
 * 
 * These tests ensure that SetAvatarSessionEvent and other critical events
 * work correctly with the SDK's event system and WebSocket communication.
 */

import { describe, it, expect, beforeEach, afterEach, vi, MockedFunction } from 'vitest';
import { EventEmitter } from '../../EventEmitter';
import type {
  SetAvatarSessionEvent,
  SetAvatarEvent,
  ClientEvent
} from '../ClientEvents';
import type { ServerEvent } from '../ServerEvents';
import { clientEventFixtures } from '../../../test/fixtures/protocol-events';

// WebSocket constants for test environment
const WS_CONNECTING = 0;
const WS_OPEN = 1;
const WS_CLOSING = 2;
const WS_CLOSED = 3;

// Mock WebSocket for testing
class MockWebSocket {
  static readonly CONNECTING = WS_CONNECTING;
  static readonly OPEN = WS_OPEN;
  static readonly CLOSING = WS_CLOSING;
  static readonly CLOSED = WS_CLOSED;
  
  readyState = WS_OPEN;
  send = vi.fn();
  close = vi.fn();
  
  constructor(public url: string) {}
  
  // Simulate receiving a message
  simulateMessage(data: string | ArrayBuffer) {
    if (this.onmessage) {
      this.onmessage(new MessageEvent('message', { data }));
    }
  }

  onopen?: (event: Event) => void;
  onclose?: (event: CloseEvent) => void;
  onmessage?: (event: MessageEvent) => void;
  onerror?: (event: Event) => void;
}

// Simplified WebSocketManager for testing
class TestWebSocketManager {
  private ws: MockWebSocket | null = null;
  private eventEmitter: EventEmitter<Record<string, any>>;

  constructor() {
    this.eventEmitter = new EventEmitter();
  }

  connect(url: string): void {
    this.ws = new MockWebSocket(url);
    
    this.ws.onmessage = (event) => {
      if (typeof event.data === 'string') {
        try {
          const parsed = JSON.parse(event.data);
          this.eventEmitter.emit(parsed.type, parsed);
        } catch (error) {
          console.error('Failed to parse message:', error);
        }
      }
    };
  }

  send(data: string | ArrayBuffer): void {
    if (this.ws && this.ws.readyState === WS_OPEN) {
      this.ws.send(data);
    }
  }

  sendJSON(data: unknown): void {
    this.send(JSON.stringify(data));
  }

  sendEvent(event: ClientEvent): void {
    this.sendJSON(event);
  }

  on(event: string, listener: (data: any) => void): void {
    this.eventEmitter.on(event, listener);
  }

  getWebSocket(): MockWebSocket | null {
    return this.ws;
  }
}

describe('ClientEvents Integration', () => {
  let wsManager: TestWebSocketManager;
  let mockWs: MockWebSocket;

  beforeEach(() => {
    vi.clearAllMocks();
    wsManager = new TestWebSocketManager();
    wsManager.connect('wss://test.example.com');
    mockWs = wsManager.getWebSocket()!;
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('SetAvatarSessionEvent with EventEmitter', () => {
    it('should emit SetAvatarSessionEvent with correct field names', () => {
      // Arrange
      const emitter = new EventEmitter<{
        set_avatar_session: SetAvatarSessionEvent;
      }>();
      const listener = vi.fn();
      emitter.on('set_avatar_session', listener);

      const event: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'test-token',
        avatar_session_id: 'test-session'
      };

      // Act
      emitter.emit('set_avatar_session', event);

      // Assert
      expect(listener).toHaveBeenCalledOnce();
      expect(listener).toHaveBeenCalledWith(event);
      const receivedEvent = listener.mock.calls[0][0];
      expect(receivedEvent.access_token).toBe('test-token');
      expect(receivedEvent.avatar_session_id).toBe('test-session');
    });

    it('should handle multiple listeners for SetAvatarSessionEvent', () => {
      // Arrange
      const emitter = new EventEmitter<{
        set_avatar_session: SetAvatarSessionEvent;
      }>();
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      
      emitter.on('set_avatar_session', listener1);
      emitter.on('set_avatar_session', listener2);

      const event = clientEventFixtures.setAvatarSession;

      // Act
      emitter.emit('set_avatar_session', event);

      // Assert
      expect(listener1).toHaveBeenCalledWith(event);
      expect(listener2).toHaveBeenCalledWith(event);
      expect(listener1.mock.calls[0][0]).toHaveProperty('access_token');
      expect(listener2.mock.calls[0][0]).toHaveProperty('avatar_session_id');
    });

    it('should maintain field names through event chain', () => {
      // Arrange
      const sourceEmitter = new EventEmitter<{ event: SetAvatarSessionEvent }>();
      const targetEmitter = new EventEmitter<{ event: SetAvatarSessionEvent }>();
      const finalListener = vi.fn();

      // Chain events
      sourceEmitter.on('event', (data) => {
        targetEmitter.emit('event', data);
      });
      targetEmitter.on('event', finalListener);

      const event: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'chain-token',
        avatar_session_id: 'chain-session'
      };

      // Act
      sourceEmitter.emit('event', event);

      // Assert
      expect(finalListener).toHaveBeenCalledWith(event);
      const received = finalListener.mock.calls[0][0];
      expect(received.access_token).toBe('chain-token');
      expect(received.avatar_session_id).toBe('chain-session');
      expect(received).not.toHaveProperty('session_id');
      expect(received).not.toHaveProperty('avatar_id');
    });
  });

  describe('SetAvatarSessionEvent with WebSocketManager', () => {
    it('should serialize SetAvatarSessionEvent correctly for WebSocket', () => {
      // Arrange
      const event: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'ws-token',
        avatar_session_id: 'ws-session'
      };

      // Act
      wsManager.sendEvent(event);

      // Assert
      expect(mockWs.send).toHaveBeenCalledOnce();
      const sentData = mockWs.send.mock.calls[0][0];
      expect(typeof sentData).toBe('string');
      
      const parsed = JSON.parse(sentData as string);
      expect(parsed).toEqual(event);
      expect(parsed.access_token).toBe('ws-token');
      expect(parsed.avatar_session_id).toBe('ws-session');
    });

    it('should not transform field names during WebSocket send', () => {
      // Arrange
      const event = clientEventFixtures.setAvatarSession;

      // Act
      wsManager.sendEvent(event);

      // Assert
      const sentData = mockWs.send.mock.calls[0][0] as string;
      expect(sentData).toContain('"access_token"');
      expect(sentData).toContain('"avatar_session_id"');
      expect(sentData).not.toContain('"session_id"');
      expect(sentData).not.toContain('"avatar_id"');
    });

    it('should handle WebSocket reconnection with correct field names', () => {
      // Arrange
      const event: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'reconnect-token',
        avatar_session_id: 'reconnect-session'
      };

      // Act - Send before reconnection
      wsManager.sendEvent(event);
      
      // Simulate reconnection
      wsManager.connect('wss://test.example.com');
      const newMockWs = wsManager.getWebSocket()!;
      
      // Send after reconnection
      wsManager.sendEvent(event);

      // Assert
      expect(newMockWs.send).toHaveBeenCalledOnce();
      const sentData = JSON.parse(newMockWs.send.mock.calls[0][0] as string);
      expect(sentData.access_token).toBe('reconnect-token');
      expect(sentData.avatar_session_id).toBe('reconnect-session');
    });

    it('should deserialize received SetAvatarSessionEvent correctly', () => {
      // Arrange
      const listener = vi.fn();
      wsManager.on('set_avatar_session', listener);

      const incomingEvent: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'incoming-token',
        avatar_session_id: 'incoming-session'
      };

      // Act - Simulate receiving the event from server
      mockWs.simulateMessage(JSON.stringify(incomingEvent));

      // Assert
      expect(listener).toHaveBeenCalledOnce();
      const received = listener.mock.calls[0][0];
      expect(received).toEqual(incomingEvent);
      expect(received.access_token).toBe('incoming-token');
      expect(received.avatar_session_id).toBe('incoming-session');
    });

    it('should handle batched events with SetAvatarSessionEvent', () => {
      // Arrange
      const events: ClientEvent[] = [
        {
          type: 'set_avatar_session',
          access_token: 'batch-token-1',
          avatar_session_id: 'batch-session-1'
        } as SetAvatarSessionEvent,
        {
          type: 'text_input',
          text: 'Hello',
          file_ids: []
        },
        {
          type: 'set_avatar_session',
          access_token: 'batch-token-2',
          avatar_session_id: 'batch-session-2'
        } as SetAvatarSessionEvent
      ];

      // Act
      events.forEach(event => wsManager.sendEvent(event));

      // Assert
      expect(mockWs.send).toHaveBeenCalledTimes(3);
      
      const firstCall = JSON.parse(mockWs.send.mock.calls[0][0] as string);
      expect(firstCall.access_token).toBe('batch-token-1');
      expect(firstCall.avatar_session_id).toBe('batch-session-1');
      
      const thirdCall = JSON.parse(mockWs.send.mock.calls[2][0] as string);
      expect(thirdCall.access_token).toBe('batch-token-2');
      expect(thirdCall.avatar_session_id).toBe('batch-session-2');
    });
  });

  describe('SetAvatarEvent with WebSocketManager', () => {
    it('should serialize SetAvatarEvent correctly', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'avatar-123',
        quality: 'hd',
        video_encoding: 'H264'
      };

      // Act
      wsManager.sendEvent(event);

      // Assert
      expect(mockWs.send).toHaveBeenCalledOnce();
      const sentData = JSON.parse(mockWs.send.mock.calls[0][0] as string);
      expect(sentData).toEqual(event);
      expect(sentData.avatar_id).toBe('avatar-123');
    });

    it('should handle optional fields in SetAvatarEvent', () => {
      // Arrange
      const minimalEvent: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'minimal-avatar'
      };

      // Act
      wsManager.sendEvent(minimalEvent);

      // Assert
      const sentData = JSON.parse(mockWs.send.mock.calls[0][0] as string);
      expect(sentData.avatar_id).toBe('minimal-avatar');
      expect(sentData.quality).toBeUndefined();
      expect(sentData.video_encoding).toBeUndefined();
    });
  });

  describe('Error scenarios', () => {
    it('should handle malformed SetAvatarSessionEvent gracefully', () => {
      // Arrange
      const listener = vi.fn();
      const errorListener = vi.fn();
      wsManager.on('set_avatar_session', listener);
      wsManager.on('error', errorListener);

      // Malformed event (wrong field names)
      const malformedEvent = {
        type: 'set_avatar_session',
        session_id: 'wrong',      // Wrong field name
        avatar_id: 'wrong'        // Wrong field name
      };

      // Act - Try to process malformed event
      mockWs.simulateMessage(JSON.stringify(malformedEvent));

      // Assert - Event is received but with wrong fields
      expect(listener).toHaveBeenCalledOnce();
      const received = listener.mock.calls[0][0];
      expect(received).not.toHaveProperty('access_token');
      expect(received).not.toHaveProperty('avatar_session_id');
      expect(received).toHaveProperty('session_id'); // Wrong field is present
      expect(received).toHaveProperty('avatar_id');   // Wrong field is present
    });

    it('should handle WebSocket send failures', () => {
      // Arrange
      mockWs.readyState = WS_CLOSED;
      const event: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'failed-token',
        avatar_session_id: 'failed-session'
      };

      // Act
      wsManager.sendEvent(event);

      // Assert - Send should not be called when WebSocket is closed
      expect(mockWs.send).not.toHaveBeenCalled();
    });

    it('should validate event structure before sending', () => {
      // Arrange
      const validateAndSend = (event: any): boolean => {
        // Validation logic
        if (!event.type || typeof event.type !== 'string') {
          return false;
        }
        
        if (event.type === 'set_avatar_session') {
          if (!event.access_token || !event.avatar_session_id) {
            return false;
          }
          if (typeof event.access_token !== 'string' || typeof event.avatar_session_id !== 'string') {
            return false;
          }
        }
        
        wsManager.sendEvent(event);
        return true;
      };

      // Act & Assert - Valid event
      const validEvent: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'valid-token',
        avatar_session_id: 'valid-session'
      };
      expect(validateAndSend(validEvent)).toBe(true);
      expect(mockWs.send).toHaveBeenCalledOnce();

      // Act & Assert - Invalid event (missing fields)
      vi.clearAllMocks();
      const invalidEvent = {
        type: 'set_avatar_session',
        access_token: 'only-token'
        // Missing avatar_session_id
      };
      expect(validateAndSend(invalidEvent)).toBe(false);
      expect(mockWs.send).not.toHaveBeenCalled();

      // Act & Assert - Invalid event (wrong types)
      vi.clearAllMocks();
      const wrongTypesEvent = {
        type: 'set_avatar_session',
        access_token: 123,           // Should be string
        avatar_session_id: true      // Should be string
      };
      expect(validateAndSend(wrongTypesEvent)).toBe(false);
      expect(mockWs.send).not.toHaveBeenCalled();
    });
  });

  describe('Type safety', () => {
    it('should enforce correct types through TypeScript', () => {
      // This test validates that TypeScript compilation enforces correct types
      const emitter = new EventEmitter<{
        set_avatar_session: SetAvatarSessionEvent;
        set_avatar: SetAvatarEvent;
      }>();

      // Correct usage - should compile
      emitter.on('set_avatar_session', (event) => {
        // TypeScript knows the type
        const token: string = event.access_token;
        const sessionId: string = event.avatar_session_id;
        expect(token).toBeDefined();
        expect(sessionId).toBeDefined();
      });

      emitter.on('set_avatar', (event) => {
        // TypeScript knows the type
        const avatarId: string = event.avatar_id;
        const quality: string | undefined = event.quality;
        expect(avatarId).toBeDefined();
        expect(quality).toBeDefined();
      });

      // Emit with correct structure
      emitter.emit('set_avatar_session', {
        type: 'set_avatar_session',
        access_token: 'typed-token',
        avatar_session_id: 'typed-session'
      });

      // The following would cause TypeScript errors:
      // emitter.emit('set_avatar_session', {
      //   type: 'set_avatar_session',
      //   session_id: 'wrong',        // ❌ TypeScript error
      //   avatar_id: 'wrong'          // ❌ TypeScript error
      // });
    });

    it('should maintain type safety through JSON serialization', () => {
      // Arrange
      const typedEvent: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        access_token: 'typed-token',
        avatar_session_id: 'typed-session'
      };

      // Act - Serialize and deserialize
      const json = JSON.stringify(typedEvent);
      const parsed: SetAvatarSessionEvent = JSON.parse(json);

      // Assert - Type assertion with runtime checks
      function assertIsSetAvatarSessionEvent(obj: any): asserts obj is SetAvatarSessionEvent {
        if (!obj || typeof obj !== 'object') {
          throw new Error('Not an object');
        }
        if (obj.type !== 'set_avatar_session') {
          throw new Error('Wrong type');
        }
        if (typeof obj.access_token !== 'string') {
          throw new Error('access_token must be string');
        }
        if (typeof obj.avatar_session_id !== 'string') {
          throw new Error('avatar_session_id must be string');
        }
      }

      // Should not throw
      expect(() => assertIsSetAvatarSessionEvent(parsed)).not.toThrow();
      
      // After assertion, TypeScript knows the type
      assertIsSetAvatarSessionEvent(parsed);
      const token: string = parsed.access_token;
      const sessionId: string = parsed.avatar_session_id;
      
      expect(token).toBe('typed-token');
      expect(sessionId).toBe('typed-session');
    });
  });
});