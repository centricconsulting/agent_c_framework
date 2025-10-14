/**
 * Integration tests for SetAvatarEvent with SDK components
 * 
 * Tests event handling, routing, and integration with EventEmitter
 * and other SDK systems.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventEmitter } from '../../EventEmitter';
import type { SetAvatarEvent, ClientEvent } from '../ClientEvents';
import type { AvatarConnectionChangedEvent } from '../ServerEvents';
import { clientEventFixtures, serverEventFixtures } from '../../../test/fixtures/protocol-events';
import { Logger } from '../../../utils/logger';

// WebSocket constants for test environment
const WS_OPEN = 1;
const WS_CLOSED = 3;

describe('SetAvatarEvent Integration', () => {
  let emitter: EventEmitter<Record<string, any>>;

  beforeEach(() => {
    vi.clearAllMocks();
    emitter = new EventEmitter();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Event Handling with EventEmitter', () => {
    it('should properly emit and handle SetAvatarEvent', () => {
      // Arrange
      const listener = vi.fn();
      emitter.on('set_avatar', listener);

      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'test-avatar',
        quality: 'hd',
        video_encoding: 'H264'
      };

      // Act
      const wasEmitted = emitter.emit('set_avatar', event);

      // Assert
      expect(wasEmitted).toBe(true);
      expect(listener).toHaveBeenCalledOnce();
      expect(listener).toHaveBeenCalledWith(event);
      
      const receivedEvent = listener.mock.calls[0][0];
      expect(receivedEvent.avatar_id).toBe('test-avatar');
      expect(receivedEvent.quality).toBe('hd');
      expect(receivedEvent.video_encoding).toBe('H264');
    });

    it('should handle multiple listeners for SetAvatarEvent', () => {
      // Arrange
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      const listener3 = vi.fn();

      emitter.on('set_avatar', listener1);
      emitter.on('set_avatar', listener2);
      emitter.once('set_avatar', listener3); // One-time listener

      const event = clientEventFixtures.setAvatar;

      // Act
      emitter.emit('set_avatar', event);
      emitter.emit('set_avatar', event); // Second emission

      // Assert
      expect(listener1).toHaveBeenCalledTimes(2);
      expect(listener2).toHaveBeenCalledTimes(2);
      expect(listener3).toHaveBeenCalledTimes(1); // Only once

      // All received the same event
      [listener1, listener2, listener3].forEach(listener => {
        if (listener.mock.calls.length > 0) {
          expect(listener.mock.calls[0][0]).toEqual(event);
        }
      });
    });

    it('should return false when no listeners registered', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'no-listeners'
      };

      // Act
      const wasEmitted = emitter.emit('set_avatar', event);

      // Assert
      expect(wasEmitted).toBe(false);
    });

    it('should handle listener removal correctly', () => {
      // Arrange
      const listener1 = vi.fn();
      const listener2 = vi.fn();

      emitter.on('set_avatar', listener1);
      emitter.on('set_avatar', listener2);

      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'remove-test'
      };

      // Act - Remove first listener
      emitter.off('set_avatar', listener1);
      emitter.emit('set_avatar', event);

      // Assert
      expect(listener1).not.toHaveBeenCalled();
      expect(listener2).toHaveBeenCalledOnce();
      expect(listener2).toHaveBeenCalledWith(event);
    });
  });

  describe('Event Routing', () => {
    it('should route SetAvatarEvent separately from other avatar events', () => {
      // Arrange
      const setAvatarListener = vi.fn();
      const setAvatarSessionListener = vi.fn();
      const clearAvatarListener = vi.fn();

      emitter.on('set_avatar', setAvatarListener);
      emitter.on('set_avatar_session', setAvatarSessionListener);
      emitter.on('clear_avatar_session', clearAvatarListener);

      // Act - Emit SetAvatarEvent
      emitter.emit('set_avatar', clientEventFixtures.setAvatar);

      // Assert - Only SetAvatar listener called
      expect(setAvatarListener).toHaveBeenCalledOnce();
      expect(setAvatarSessionListener).not.toHaveBeenCalled();
      expect(clearAvatarListener).not.toHaveBeenCalled();

      // Act - Emit SetAvatarSessionEvent
      vi.clearAllMocks();
      emitter.emit('set_avatar_session', clientEventFixtures.setAvatarSession);

      // Assert - Only SetAvatarSession listener called
      expect(setAvatarListener).not.toHaveBeenCalled();
      expect(setAvatarSessionListener).toHaveBeenCalledOnce();
      expect(clearAvatarListener).not.toHaveBeenCalled();
    });

    it('should work with typed EventEmitter', () => {
      // Arrange
      type AvatarEventMap = {
        set_avatar: SetAvatarEvent;
        avatar_connection_changed: AvatarConnectionChangedEvent;
      };

      const typedEmitter = new EventEmitter<AvatarEventMap>();
      const listener = vi.fn();

      typedEmitter.on('set_avatar', listener);

      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'typed-test',
        quality: 'auto'
      };

      // Act
      typedEmitter.emit('set_avatar', event);

      // Assert - TypeScript ensures type safety
      expect(listener).toHaveBeenCalledWith(event);
      const received = listener.mock.calls[0][0];
      expect(received.avatar_id).toBe('typed-test');
    });
  });

  describe('Type Narrowing in Handlers', () => {
    it('should allow type-safe access to SetAvatarEvent fields', () => {
      // Arrange
      const handler = vi.fn((event: SetAvatarEvent) => {
        // TypeScript knows the exact type here
        const avatarId: string = event.avatar_id;
        const quality: string | undefined = event.quality;
        const encoding: string | undefined = event.video_encoding;

        return {
          avatarId,
          quality: quality || 'default',
          encoding: encoding || 'H264'
        };
      });

      emitter.on('set_avatar', handler);

      // Act
      emitter.emit('set_avatar', {
        type: 'set_avatar',
        avatar_id: 'type-safe',
        quality: 'hd'
      });

      // Assert
      expect(handler).toHaveBeenCalledOnce();
      const result = handler.mock.results[0].value;
      expect(result).toEqual({
        avatarId: 'type-safe',
        quality: 'hd',
        encoding: 'H264'
      });
    });

    it('should handle event discrimination in general handler', () => {
      // Arrange
      const generalHandler = vi.fn((event: ClientEvent) => {
        // Need to narrow the type
        if (event.type === 'set_avatar') {
          // TypeScript knows this is SetAvatarEvent
          return `Avatar: ${event.avatar_id}`;
        } else if (event.type === 'set_avatar_session') {
          // TypeScript knows this is SetAvatarSessionEvent
          return `Session: ${(event as any).avatar_session_id}`;
        }
        return 'Unknown event';
      });

      // Act
      const avatarResult = generalHandler({
        type: 'set_avatar',
        avatar_id: 'discriminated'
      } as SetAvatarEvent);

      const sessionResult = generalHandler({
        type: 'set_avatar_session',
        access_token: 'token',
        avatar_session_id: 'session'
      } as any);

      // Assert
      expect(avatarResult).toBe('Avatar: discriminated');
      expect(sessionResult).toBe('Session: session');
    });
  });

  describe('Event Flow Simulation', () => {
    it('should simulate avatar selection flow', async () => {
      // Arrange - Simulate client requesting avatar change
      const clientEvents: ClientEvent[] = [];
      const serverResponses: any[] = [];

      // Client event handler (would send to server)
      emitter.on('set_avatar', (event: SetAvatarEvent) => {
        clientEvents.push(event);
        
        // Simulate server response
        setTimeout(() => {
          const response: AvatarConnectionChangedEvent = {
            type: 'avatar_connection_changed',
            avatar_session_request: {
              avatar_id: event.avatar_id,
              quality: event.quality || 'auto',
              video_encoding: event.video_encoding || 'H264',
              voice: null,
              language: null,
              version: 'v2',
              source: null,
              stt_settings: null,
              ia_is_livekit_transport: false,
              knowledge_base: null,
              knowledge_base_id: null,
              disable_idle_timeout: false,
              activity_idle_timeout: 300
            },
            avatar_session: {
              session_id: `session-for-${event.avatar_id}`,
              url: 'wss://avatar.heygen.com/session',
              access_token: 'generated-token',
              session_duration_limit: 3600,
              is_paid: true,
              realtime_endpoint: 'wss://realtime.heygen.com',
              sdp: null,
              ice_servers: [],
              ice_servers2: []
            }
          };
          emitter.emit('avatar_connection_changed', response);
        }, 10);
      });

      // Server response handler
      emitter.on('avatar_connection_changed', (event: AvatarConnectionChangedEvent) => {
        serverResponses.push(event);
      });

      // Act - Client selects an avatar
      emitter.emit('set_avatar', {
        type: 'set_avatar',
        avatar_id: 'selected-avatar',
        quality: 'hd',
        video_encoding: 'H265'
      });

      // Wait for async response
      await new Promise(resolve => setTimeout(resolve, 20));

      // Assert
      expect(clientEvents).toHaveLength(1);
      expect(clientEvents[0].type).toBe('set_avatar');
      expect((clientEvents[0] as SetAvatarEvent).avatar_id).toBe('selected-avatar');

      expect(serverResponses).toHaveLength(1);
      expect(serverResponses[0].type).toBe('avatar_connection_changed');
      expect(serverResponses[0].avatar_session_request.avatar_id).toBe('selected-avatar');
      expect(serverResponses[0].avatar_session_request.quality).toBe('hd');
      expect(serverResponses[0].avatar_session_request.video_encoding).toBe('H265');
      expect(serverResponses[0].avatar_session.session_id).toContain('selected-avatar');
    });

    it('should handle avatar switching scenario', () => {
      // Arrange
      const avatarHistory: string[] = [];
      
      emitter.on('set_avatar', (event: SetAvatarEvent) => {
        avatarHistory.push(event.avatar_id);
      });

      // Act - User switches between avatars
      emitter.emit('set_avatar', { type: 'set_avatar', avatar_id: 'avatar-1' });
      emitter.emit('set_avatar', { type: 'set_avatar', avatar_id: 'avatar-2', quality: 'hd' });
      emitter.emit('set_avatar', { type: 'set_avatar', avatar_id: 'avatar-3', video_encoding: 'VP9' });
      emitter.emit('set_avatar', { type: 'set_avatar', avatar_id: 'avatar-1' }); // Back to first

      // Assert
      expect(avatarHistory).toEqual(['avatar-1', 'avatar-2', 'avatar-3', 'avatar-1']);
    });
  });

  describe('Error Handling', () => {
    it('should handle errors in event listeners', () => {
      // Arrange
      const errorListener = vi.fn(() => {
        throw new Error('Listener error');
      });
      const normalListener = vi.fn();

      emitter.on('set_avatar', errorListener);
      emitter.on('set_avatar', normalListener);

      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

      // Act
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'error-test'
      };
      
      const wasEmitted = emitter.emit('set_avatar', event);

      // Assert
      expect(wasEmitted).toBe(true);
      expect(errorListener).toHaveBeenCalledWith(event);
      expect(normalListener).toHaveBeenCalledWith(event); // Should still be called
      expect(consoleSpy).toHaveBeenCalled();
      
      consoleSpy.mockRestore();
    });

    it('should validate event before processing', () => {
      // Arrange
      const validatingHandler = vi.fn((event: any) => {
        // Validate before processing
        if (!isValidSetAvatarEvent(event)) {
          throw new Error('Invalid SetAvatarEvent');
        }
        
        // Safe to use as SetAvatarEvent
        return `Processing avatar: ${event.avatar_id}`;
      });

      // Act & Assert - Valid event
      const validResult = validatingHandler({
        type: 'set_avatar',
        avatar_id: 'valid-avatar'
      });
      expect(validResult).toBe('Processing avatar: valid-avatar');

      // Act & Assert - Invalid event
      expect(() => {
        validatingHandler({
          type: 'set_avatar'
          // Missing avatar_id
        });
      }).toThrow('Invalid SetAvatarEvent');

      expect(() => {
        validatingHandler({
          type: 'wrong_type',
          avatar_id: 'test'
        });
      }).toThrow('Invalid SetAvatarEvent');
    });
  });

  describe('Memory Management', () => {
    it('should properly clean up listeners', () => {
      // Arrange
      const listeners = Array.from({ length: 10 }, (_, i) => vi.fn());
      
      // Add all listeners
      listeners.forEach(listener => {
        emitter.on('set_avatar', listener);
      });

      // Act - Remove all listeners
      emitter.removeAllListeners('set_avatar');
      
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'cleanup-test'
      };
      
      const wasEmitted = emitter.emit('set_avatar', event);

      // Assert
      expect(wasEmitted).toBe(false); // No listeners
      listeners.forEach(listener => {
        expect(listener).not.toHaveBeenCalled();
      });
    });

    it('should handle high listener count warning', () => {
      // Arrange
      const warnSpy = vi.spyOn(Logger, 'warn').mockImplementation(() => {});

      // Act - Add more than HIGH_LISTENER_THRESHOLD (10) listeners
      // EventEmitter warns when listener count exceeds 10
      for (let i = 0; i < 12; i++) {
        emitter.on('set_avatar', vi.fn());
      }

      // Assert - Logger.warn should be called with high listener count message
      expect(warnSpy).toHaveBeenCalled();
      const warnCall = warnSpy.mock.calls.find(call => 
        call[0] === '[EventEmitter] High listener count detected'
      );
      expect(warnCall).toBeDefined();
      expect(warnCall![1]).toMatchObject({
        event: 'set_avatar',
        count: expect.any(Number),
        threshold: 10,
        message: expect.stringContaining('Possible listener leak')
      });
      
      warnSpy.mockRestore();
    });
  });

  describe('WebSocket Integration Simulation', () => {
    it('should serialize SetAvatarEvent for WebSocket transmission', () => {
      // Arrange
      const mockSend = vi.fn();
      const wsManager = {
        readyState: WS_OPEN,
        send: mockSend,
        sendJSON: function(data: any) {
          if (this.readyState === WS_OPEN) {
            this.send(JSON.stringify(data));
          }
        }
      };

      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'ws-test',
        quality: 'auto',
        video_encoding: 'H264'
      };

      // Act
      wsManager.sendJSON(event);

      // Assert
      expect(mockSend).toHaveBeenCalledOnce();
      const sentData = mockSend.mock.calls[0][0];
      const parsed = JSON.parse(sentData);
      
      expect(parsed).toEqual(event);
      expect(parsed.type).toBe('set_avatar');
      expect(parsed.avatar_id).toBe('ws-test');
      expect(parsed.quality).toBe('auto');
      expect(parsed.video_encoding).toBe('H264');
    });

    it('should not send when WebSocket is closed', () => {
      // Arrange
      const mockSend = vi.fn();
      const wsManager = {
        readyState: WS_CLOSED,
        send: mockSend,
        sendJSON: function(data: any) {
          if (this.readyState === WS_OPEN) {
            this.send(JSON.stringify(data));
          }
        }
      };

      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'closed-test'
      };

      // Act
      wsManager.sendJSON(event);

      // Assert
      expect(mockSend).not.toHaveBeenCalled();
    });
  });
});

/**
 * Helper function to validate SetAvatarEvent
 */
function isValidSetAvatarEvent(event: any): event is SetAvatarEvent {
  return (
    event &&
    typeof event === 'object' &&
    event.type === 'set_avatar' &&
    'avatar_id' in event &&
    typeof event.avatar_id === 'string' &&
    (event.quality === undefined || typeof event.quality === 'string') &&
    (event.video_encoding === undefined || typeof event.video_encoding === 'string')
  );
}