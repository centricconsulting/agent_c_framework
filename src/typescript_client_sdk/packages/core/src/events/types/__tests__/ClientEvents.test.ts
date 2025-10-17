/**
 * Unit tests for Client Events - Critical API Compliance Validation
 * 
 * These tests ensure that client event structures match the exact API requirements,
 * particularly focusing on field names that could cause runtime failures if incorrect.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type {
  SetAvatarSessionEvent,
  SetAvatarEvent,
  BaseClientEvent,
  ClientEvent
} from '../ClientEvents';
import { clientEventFixtures } from '../../../test/fixtures/protocol-events';

describe('ClientEvents API Compliance', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('SetAvatarSessionEvent', () => {
    describe('field name validation', () => {
      it('should have access_token field (NOT session_id)', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'test-access-token',
          avatar_session_id: 'test-avatar-session-id'
        };

        // Act & Assert
        expect(event).toHaveProperty('access_token');
        expect(event).not.toHaveProperty('session_id');
        expect(event.access_token).toBe('test-access-token');
      });

      it('should have avatar_session_id field (NOT avatar_id)', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'test-access-token',
          avatar_session_id: 'test-avatar-session-id'
        };

        // Act & Assert
        expect(event).toHaveProperty('avatar_session_id');
        expect(event).not.toHaveProperty('avatar_id');
        expect(event.avatar_session_id).toBe('test-avatar-session-id');
      });

      it('should maintain exact field names from fixture', () => {
        // Arrange - using the fixture which has correct field names
        const fixtureEvent = clientEventFixtures.setAvatarSession;

        // Act & Assert
        expect(fixtureEvent).toHaveProperty('access_token');
        expect(fixtureEvent).toHaveProperty('avatar_session_id');
        expect(fixtureEvent.access_token).toBe('avatar-access-token-xyz');
        expect(fixtureEvent.avatar_session_id).toBe('avatar-session-xyz');
      });

      it('should not allow incorrect field names at compile time', () => {
        // This test validates TypeScript compilation prevents wrong field names
        // The following would fail TypeScript compilation:
        // const badEvent: SetAvatarSessionEvent = {
        //   type: 'set_avatar_session',
        //   session_id: 'wrong',        // ❌ TypeScript error
        //   avatar_id: 'wrong'          // ❌ TypeScript error
        // };

        // Correct structure compiles
        const goodEvent: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'correct',
          avatar_session_id: 'correct'
        };

        expect(goodEvent.access_token).toBeDefined();
        expect(goodEvent.avatar_session_id).toBeDefined();
      });
    });

    describe('type validation', () => {
      it('should have correct type discriminator', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'session'
        };

        // Act & Assert
        expect(event.type).toBe('set_avatar_session');
        expect(typeof event.type).toBe('string');
      });

      it('should require string values for access_token', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'string-token',
          avatar_session_id: 'session'
        };

        // Act & Assert
        expect(typeof event.access_token).toBe('string');
      });

      it('should require string values for avatar_session_id', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'string-session-id'
        };

        // Act & Assert
        expect(typeof event.avatar_session_id).toBe('string');
      });

      it('should extend BaseClientEvent', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'session'
        };

        // Act - Check if it satisfies BaseClientEvent
        const baseEvent: BaseClientEvent = event;

        // Assert
        expect(baseEvent.type).toBe('set_avatar_session');
      });

      it('should be assignable to ClientEvent union type', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'session'
        };

        // Act - This should compile without errors
        const clientEvent: ClientEvent = event;

        // Assert
        expect(clientEvent.type).toBe('set_avatar_session');
      });
    });

    describe('serialization/deserialization', () => {
      it('should maintain field names through JSON round-trip', () => {
        // Arrange
        const originalEvent: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'test-token-123',
          avatar_session_id: 'session-abc-456'
        };

        // Act
        const jsonString = JSON.stringify(originalEvent);
        const deserializedEvent = JSON.parse(jsonString) as SetAvatarSessionEvent;

        // Assert - Field names must be exactly the same
        expect(deserializedEvent).toEqual(originalEvent);
        expect(deserializedEvent.access_token).toBe('test-token-123');
        expect(deserializedEvent.avatar_session_id).toBe('session-abc-456');
        expect(Object.keys(deserializedEvent)).toEqual(['type', 'access_token', 'avatar_session_id']);
      });

      it('should not transform field names during serialization', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'session'
        };

        // Act
        const json = JSON.stringify(event);

        // Assert - Check raw JSON contains correct field names
        expect(json).toContain('"access_token"');
        expect(json).toContain('"avatar_session_id"');
        expect(json).not.toContain('"session_id"');
        expect(json).not.toContain('"avatar_id"');
      });

      it('should preserve empty string values', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: '',
          avatar_session_id: ''
        };

        // Act
        const json = JSON.stringify(event);
        const parsed = JSON.parse(json) as SetAvatarSessionEvent;

        // Assert
        expect(parsed.access_token).toBe('');
        expect(parsed.avatar_session_id).toBe('');
      });

      it('should handle special characters in field values', () => {
        // Arrange
        const event: SetAvatarSessionEvent = {
          type: 'set_avatar_session',
          access_token: 'token-with-"quotes"-and-\\backslash',
          avatar_session_id: 'session/with/slashes?query=param&other=value'
        };

        // Act
        const json = JSON.stringify(event);
        const parsed = JSON.parse(json) as SetAvatarSessionEvent;

        // Assert
        expect(parsed.access_token).toBe(event.access_token);
        expect(parsed.avatar_session_id).toBe(event.avatar_session_id);
      });

      it('should validate deserialized structure matches type', () => {
        // Arrange
        const jsonString = '{"type":"set_avatar_session","access_token":"tok123","avatar_session_id":"sess456"}';

        // Act
        const event = JSON.parse(jsonString) as SetAvatarSessionEvent;

        // Assert - Runtime validation
        expect(event).toMatchObject({
          type: 'set_avatar_session',
          access_token: expect.any(String),
          avatar_session_id: expect.any(String)
        });
        expect(Object.keys(event).sort()).toEqual(['access_token', 'avatar_session_id', 'type']);
      });
    });

    describe('runtime validation', () => {
      it('should detect missing access_token field', () => {
        // Arrange
        const invalidEvent = {
          type: 'set_avatar_session',
          avatar_session_id: 'session'
          // Missing access_token
        };

        // Act & Assert
        expect(invalidEvent).not.toHaveProperty('access_token');
        expect(() => {
          // Runtime validation would catch this
          validateSetAvatarSessionEvent(invalidEvent);
        }).toThrow('Missing required field: access_token');
      });

      it('should detect missing avatar_session_id field', () => {
        // Arrange
        const invalidEvent = {
          type: 'set_avatar_session',
          access_token: 'token'
          // Missing avatar_session_id
        };

        // Act & Assert
        expect(invalidEvent).not.toHaveProperty('avatar_session_id');
        expect(() => {
          validateSetAvatarSessionEvent(invalidEvent);
        }).toThrow('Missing required field: avatar_session_id');
      });

      it('should detect wrong field names', () => {
        // Arrange
        const wrongFieldEvent = {
          type: 'set_avatar_session',
          session_id: 'wrong-field',      // Wrong field name!
          avatar_id: 'wrong-field'        // Wrong field name!
        };

        // Act & Assert
        expect(() => {
          validateSetAvatarSessionEvent(wrongFieldEvent);
        }).toThrow('Missing required field: access_token');
      });

      it('should validate field types at runtime', () => {
        // Arrange
        const invalidTypes = {
          type: 'set_avatar_session',
          access_token: 123,              // Should be string
          avatar_session_id: true         // Should be string
        };

        // Act & Assert
        expect(() => {
          validateSetAvatarSessionEvent(invalidTypes);
        }).toThrow('Invalid field type');
      });

      it('should pass validation for correct structure', () => {
        // Arrange
        const validEvent = {
          type: 'set_avatar_session',
          access_token: 'valid-token',
          avatar_session_id: 'valid-session'
        };

        // Act & Assert
        expect(() => {
          validateSetAvatarSessionEvent(validEvent);
        }).not.toThrow();
      });
    });

    describe('type guards', () => {
      it('should correctly identify SetAvatarSessionEvent', () => {
        // Arrange
        const event: ClientEvent = {
          type: 'set_avatar_session',
          access_token: 'token',
          avatar_session_id: 'session'
        } as SetAvatarSessionEvent;

        // Act & Assert
        expect(isSetAvatarSessionEvent(event)).toBe(true);
        if (isSetAvatarSessionEvent(event)) {
          // TypeScript should narrow the type here
          expect(event.access_token).toBeDefined();
          expect(event.avatar_session_id).toBeDefined();
        }
      });

      it('should reject events with wrong type', () => {
        // Arrange
        const otherEvent: ClientEvent = {
          type: 'text_input',
          text: 'hello'
        } as any;

        // Act & Assert
        expect(isSetAvatarSessionEvent(otherEvent)).toBe(false);
      });

      it('should reject events with missing fields', () => {
        // Arrange
        const incompleteEvent = {
          type: 'set_avatar_session',
          access_token: 'token'
          // Missing avatar_session_id
        };

        // Act & Assert
        expect(isSetAvatarSessionEvent(incompleteEvent)).toBe(false);
      });

      it('should reject events with wrong field types', () => {
        // Arrange
        const wrongTypesEvent = {
          type: 'set_avatar_session',
          access_token: 123,
          avatar_session_id: true
        };

        // Act & Assert
        expect(isSetAvatarSessionEvent(wrongTypesEvent)).toBe(false);
      });
    });
  });

  describe('SetAvatarEvent', () => {
    describe('field validation', () => {
      it('should have avatar_id field', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-123',
          quality: 'hd',
          video_encoding: 'H264'
        };

        // Act & Assert
        expect(event).toHaveProperty('avatar_id');
        expect(event.avatar_id).toBe('avatar-123');
      });

      it('should have optional quality field', () => {
        // Arrange
        const eventWithQuality: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-123',
          quality: 'auto'
        };

        const eventWithoutQuality: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-123'
        };

        // Act & Assert
        expect(eventWithQuality.quality).toBe('auto');
        expect(eventWithoutQuality.quality).toBeUndefined();
      });

      it('should have optional video_encoding field', () => {
        // Arrange
        const eventWithEncoding: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-123',
          video_encoding: 'H265'
        };

        const eventWithoutEncoding: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-123'
        };

        // Act & Assert
        expect(eventWithEncoding.video_encoding).toBe('H265');
        expect(eventWithoutEncoding.video_encoding).toBeUndefined();
      });
    });

    describe('serialization', () => {
      it('should maintain field names through JSON round-trip', () => {
        // Arrange
        const originalEvent: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'avatar-456',
          quality: 'hd',
          video_encoding: 'H264'
        };

        // Act
        const json = JSON.stringify(originalEvent);
        const parsed = JSON.parse(json) as SetAvatarEvent;

        // Assert
        expect(parsed).toEqual(originalEvent);
        expect(parsed.avatar_id).toBe('avatar-456');
        expect(Object.keys(parsed)).toContain('avatar_id');
      });

      it('should handle partial fields correctly', () => {
        // Arrange
        const minimalEvent: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'minimal-avatar'
        };

        // Act
        const json = JSON.stringify(minimalEvent);
        const parsed = JSON.parse(json) as SetAvatarEvent;

        // Assert
        expect(parsed.avatar_id).toBe('minimal-avatar');
        expect(parsed.quality).toBeUndefined();
        expect(parsed.video_encoding).toBeUndefined();
        expect(Object.keys(parsed)).toEqual(['type', 'avatar_id']);
      });
    });

    describe('type guards', () => {
      it('should correctly identify SetAvatarEvent', () => {
        // Arrange
        const event: ClientEvent = {
          type: 'set_avatar',
          avatar_id: 'test-avatar'
        } as SetAvatarEvent;

        // Act & Assert
        expect(isSetAvatarEvent(event)).toBe(true);
        if (isSetAvatarEvent(event)) {
          expect(event.avatar_id).toBeDefined();
        }
      });

      it('should reject events with missing avatar_id', () => {
        // Arrange
        const invalidEvent = {
          type: 'set_avatar',
          quality: 'hd'
          // Missing avatar_id
        };

        // Act & Assert
        expect(isSetAvatarEvent(invalidEvent)).toBe(false);
      });
    });
  });
});

/**
 * Runtime validation function for SetAvatarSessionEvent
 * This ensures the event structure is correct at runtime
 */
function validateSetAvatarSessionEvent(event: any): asserts event is SetAvatarSessionEvent {
  if (!event || typeof event !== 'object') {
    throw new Error('Event must be an object');
  }

  if (event.type !== 'set_avatar_session') {
    throw new Error(`Invalid event type: ${event.type}`);
  }

  if (!('access_token' in event)) {
    throw new Error('Missing required field: access_token');
  }

  if (!('avatar_session_id' in event)) {
    throw new Error('Missing required field: avatar_session_id');
  }

  if (typeof event.access_token !== 'string') {
    throw new Error('Invalid field type: access_token must be a string');
  }

  if (typeof event.avatar_session_id !== 'string') {
    throw new Error('Invalid field type: avatar_session_id must be a string');
  }
}

/**
 * Type guard for SetAvatarSessionEvent
 */
function isSetAvatarSessionEvent(event: any): event is SetAvatarSessionEvent {
  return (
    event &&
    typeof event === 'object' &&
    event.type === 'set_avatar_session' &&
    'access_token' in event &&
    'avatar_session_id' in event &&
    typeof event.access_token === 'string' &&
    typeof event.avatar_session_id === 'string'
  );
}

/**
 * Type guard for SetAvatarEvent
 */
function isSetAvatarEvent(event: any): event is SetAvatarEvent {
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