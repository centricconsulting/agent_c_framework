/**
 * Comprehensive unit tests for SetAvatarEvent
 * 
 * This event was completely missing from the codebase and needs thorough testing
 * to ensure proper integration with the Avatar system.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { SetAvatarEvent, ClientEvent, BaseClientEvent } from '../ClientEvents';
import { clientEventFixtures } from '../../../test/fixtures/protocol-events';

describe('SetAvatarEvent - Comprehensive Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Event Type Existence', () => {
    it('should be properly exported from ClientEvents', () => {
      // Arrange & Act
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'test-avatar'
      };

      // Assert
      expect(event).toBeDefined();
      expect(event.type).toBe('set_avatar');
    });

    it('should have correct type discriminator value', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'avatar-123'
      };

      // Act & Assert
      expect(event.type).toBe('set_avatar');
      expect(typeof event.type).toBe('string');
      
      // Type discriminator should be literal 'set_avatar'
      const typeValue: 'set_avatar' = event.type;
      expect(typeValue).toBe('set_avatar');
    });

    it('should properly extend BaseClientEvent', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'base-test'
      };

      // Act - Verify it satisfies BaseClientEvent interface
      const baseEvent: BaseClientEvent = event;

      // Assert
      expect(baseEvent).toBeDefined();
      expect(baseEvent.type).toBe('set_avatar');
      
      // Should have only properties from BaseClientEvent + SetAvatarEvent
      expect('type' in baseEvent).toBe(true);
    });

    it('should be assignable to ClientEvent union type', () => {
      // Arrange
      const avatarEvent: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'union-test'
      };

      // Act - Should be assignable to ClientEvent union
      const clientEvent: ClientEvent = avatarEvent;

      // Assert
      expect(clientEvent.type).toBe('set_avatar');
      
      // Type narrowing should work
      if (clientEvent.type === 'set_avatar') {
        expect((clientEvent as SetAvatarEvent).avatar_id).toBe('union-test');
      }
    });
  });

  describe('Event Structure Validation', () => {
    describe('required fields', () => {
      it('should require avatar_id as a mandatory field', () => {
        // Arrange - TypeScript enforces this at compile time
        const validEvent: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'required-id'
        };

        // The following would fail TypeScript compilation:
        // const invalidEvent: SetAvatarEvent = {
        //   type: 'set_avatar'
        //   // Missing avatar_id - TypeScript error
        // };

        // Act & Assert
        expect(validEvent.avatar_id).toBe('required-id');
        expect(validEvent.avatar_id).toBeDefined();
        expect(typeof validEvent.avatar_id).toBe('string');
      });

      it('should have type as a required field', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test'
        };

        // Act & Assert
        expect(event.type).toBeDefined();
        expect(event.type).toBe('set_avatar');
      });
    });

    describe('optional fields', () => {
      it('should allow quality as optional string field', () => {
        // Arrange
        const withQuality: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-1',
          quality: 'hd'
        };

        const withoutQuality: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-2'
        };

        // Act & Assert
        expect(withQuality.quality).toBe('hd');
        expect(withoutQuality.quality).toBeUndefined();
        
        // Should accept various quality values
        const qualityValues = ['auto', 'low', 'medium', 'hd', '4k'];
        qualityValues.forEach(q => {
          const event: SetAvatarEvent = {
            type: 'set_avatar',
            avatar_id: 'test',
            quality: q
          };
          expect(event.quality).toBe(q);
        });
      });

      it('should allow video_encoding as optional string field', () => {
        // Arrange
        const withEncoding: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-1',
          video_encoding: 'H264'
        };

        const withoutEncoding: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-2'
        };

        // Act & Assert
        expect(withEncoding.video_encoding).toBe('H264');
        expect(withoutEncoding.video_encoding).toBeUndefined();

        // Should accept various encoding values
        const encodings = ['H264', 'H265', 'VP8', 'VP9', 'AV1'];
        encodings.forEach(enc => {
          const event: SetAvatarEvent = {
            type: 'set_avatar',
            avatar_id: 'test',
            video_encoding: enc
          };
          expect(event.video_encoding).toBe(enc);
        });
      });

      it('should allow all combinations of optional fields', () => {
        // Arrange - All possible combinations
        const onlyRequired: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-1'
        };

        const withQualityOnly: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-2',
          quality: 'hd'
        };

        const withEncodingOnly: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-3',
          video_encoding: 'H265'
        };

        const withBoth: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-4',
          quality: 'auto',
          video_encoding: 'H264'
        };

        // Act & Assert
        expect(onlyRequired.quality).toBeUndefined();
        expect(onlyRequired.video_encoding).toBeUndefined();

        expect(withQualityOnly.quality).toBe('hd');
        expect(withQualityOnly.video_encoding).toBeUndefined();

        expect(withEncodingOnly.quality).toBeUndefined();
        expect(withEncodingOnly.video_encoding).toBe('H265');

        expect(withBoth.quality).toBe('auto');
        expect(withBoth.video_encoding).toBe('H264');
      });
    });

    describe('field type validation', () => {
      it('should enforce string type for avatar_id', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'string-avatar-id'
        };

        // Act & Assert
        expect(typeof event.avatar_id).toBe('string');
        
        // Runtime validation
        function validateAvatarId(id: any): boolean {
          return typeof id === 'string' && id.length > 0;
        }
        
        expect(validateAvatarId(event.avatar_id)).toBe(true);
        expect(validateAvatarId(123)).toBe(false);
        expect(validateAvatarId(null)).toBe(false);
        expect(validateAvatarId(undefined)).toBe(false);
      });

      it('should enforce string type for optional fields when present', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test',
          quality: 'hd',
          video_encoding: 'H264'
        };

        // Act & Assert
        expect(typeof event.quality).toBe('string');
        expect(typeof event.video_encoding).toBe('string');
      });
    });

    describe('no extra fields', () => {
      it('should not have fields from SetAvatarSessionEvent', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test-avatar'
        };

        // Act & Assert - Ensure no confusion with SetAvatarSessionEvent
        expect(event).not.toHaveProperty('access_token');
        expect(event).not.toHaveProperty('avatar_session_id');
        expect(event).not.toHaveProperty('session_id');
      });

      it('should only contain expected fields', () => {
        // Arrange
        const event: SetAvatarEvent = {
          type: 'set_avatar',
          avatar_id: 'test',
          quality: 'hd',
          video_encoding: 'H264'
        };

        // Act
        const keys = Object.keys(event);

        // Assert - Only expected fields
        expect(keys).toHaveLength(4);
        expect(keys).toContain('type');
        expect(keys).toContain('avatar_id');
        expect(keys).toContain('quality');
        expect(keys).toContain('video_encoding');
        
        // No unexpected fields
        keys.forEach(key => {
          expect(['type', 'avatar_id', 'quality', 'video_encoding']).toContain(key);
        });
      });
    });
  });

  describe('JSON Serialization', () => {
    it('should maintain structure through JSON round-trip', () => {
      // Arrange
      const original: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'json-test',
        quality: 'hd',
        video_encoding: 'H265'
      };

      // Act
      const json = JSON.stringify(original);
      const parsed = JSON.parse(json) as SetAvatarEvent;

      // Assert
      expect(parsed).toEqual(original);
      expect(parsed.type).toBe('set_avatar');
      expect(parsed.avatar_id).toBe('json-test');
      expect(parsed.quality).toBe('hd');
      expect(parsed.video_encoding).toBe('H265');
    });

    it('should not include undefined optional fields in JSON', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'minimal'
      };

      // Act
      const json = JSON.stringify(event);
      const parsed = JSON.parse(json);

      // Assert
      expect(json).not.toContain('quality');
      expect(json).not.toContain('video_encoding');
      expect(Object.keys(parsed)).toEqual(['type', 'avatar_id']);
    });

    it('should handle special characters in field values', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'avatar/with/slashes-and-dashes_underscores.123',
        quality: 'hd@2x',
        video_encoding: 'H.264/AVC'
      };

      // Act
      const json = JSON.stringify(event);
      const parsed = JSON.parse(json) as SetAvatarEvent;

      // Assert
      expect(parsed.avatar_id).toBe(event.avatar_id);
      expect(parsed.quality).toBe(event.quality);
      expect(parsed.video_encoding).toBe(event.video_encoding);
    });

    it('should handle empty string values', () => {
      // Arrange
      const event: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: '', // Empty but valid
        quality: '',
        video_encoding: ''
      };

      // Act
      const json = JSON.stringify(event);
      const parsed = JSON.parse(json) as SetAvatarEvent;

      // Assert
      expect(parsed.avatar_id).toBe('');
      expect(parsed.quality).toBe('');
      expect(parsed.video_encoding).toBe('');
    });
  });

  describe('Fixture Integration', () => {
    it('should have SetAvatarEvent in test fixtures', () => {
      // Act & Assert
      expect(clientEventFixtures.setAvatar).toBeDefined();
      expect(clientEventFixtures.setAvatar.type).toBe('set_avatar');
      expect(clientEventFixtures.setAvatar.avatar_id).toBe('avatar-1');
    });

    it('should match fixture structure with type definition', () => {
      // Arrange
      const fixture = clientEventFixtures.setAvatar;

      // Act - Verify fixture satisfies type
      const typedEvent: SetAvatarEvent = fixture;

      // Assert
      expect(typedEvent).toBeDefined();
      expect(typedEvent.type).toBe('set_avatar');
      expect(typedEvent.avatar_id).toBeDefined();
      expect(typeof typedEvent.avatar_id).toBe('string');
      
      // Optional fields from fixture
      if (fixture.quality) {
        expect(typeof fixture.quality).toBe('string');
      }
      if (fixture.video_encoding) {
        expect(typeof fixture.video_encoding).toBe('string');
      }
    });

    it('should use fixture for consistent testing', () => {
      // Arrange
      const fixture = clientEventFixtures.setAvatar;

      // Act - Use fixture in a simulated test scenario
      function processAvatarEvent(event: SetAvatarEvent): string {
        return `Setting avatar ${event.avatar_id} with quality ${event.quality || 'default'}`;
      }

      const result = processAvatarEvent(fixture);

      // Assert
      expect(result).toContain('avatar-1');
      expect(result).toContain('hd');
    });
  });

  describe('Runtime Validation', () => {
    it('should validate event structure at runtime', () => {
      // Arrange
      const validEvent = {
        type: 'set_avatar',
        avatar_id: 'runtime-test',
        quality: 'hd'
      };

      const invalidEvent1 = {
        type: 'set_avatar'
        // Missing avatar_id
      };

      const invalidEvent2 = {
        type: 'wrong_type',
        avatar_id: 'test'
      };

      const invalidEvent3 = {
        type: 'set_avatar',
        avatar_id: 123 // Wrong type
      };

      // Act & Assert
      expect(() => validateSetAvatarEvent(validEvent)).not.toThrow();
      expect(() => validateSetAvatarEvent(invalidEvent1)).toThrow('Missing required field: avatar_id');
      expect(() => validateSetAvatarEvent(invalidEvent2)).toThrow('Invalid event type');
      expect(() => validateSetAvatarEvent(invalidEvent3)).toThrow('Invalid field type: avatar_id must be a string');
    });

    it('should validate optional fields when present', () => {
      // Arrange
      const validWithOptionals = {
        type: 'set_avatar',
        avatar_id: 'test',
        quality: 'hd',
        video_encoding: 'H264'
      };

      const invalidQuality = {
        type: 'set_avatar',
        avatar_id: 'test',
        quality: 123 // Should be string
      };

      const invalidEncoding = {
        type: 'set_avatar',
        avatar_id: 'test',
        video_encoding: false // Should be string
      };

      // Act & Assert
      expect(() => validateSetAvatarEvent(validWithOptionals)).not.toThrow();
      expect(() => validateSetAvatarEvent(invalidQuality)).toThrow('Invalid field type: quality must be a string');
      expect(() => validateSetAvatarEvent(invalidEncoding)).toThrow('Invalid field type: video_encoding must be a string');
    });
  });

  describe('Type Guards', () => {
    it('should correctly identify SetAvatarEvent', () => {
      // Arrange
      const avatarEvent: ClientEvent = {
        type: 'set_avatar',
        avatar_id: 'guard-test'
      } as SetAvatarEvent;

      const otherEvent: ClientEvent = {
        type: 'text_input',
        text: 'hello',
        file_ids: []
      } as any;

      // Act & Assert
      expect(isSetAvatarEvent(avatarEvent)).toBe(true);
      expect(isSetAvatarEvent(otherEvent)).toBe(false);

      // Type narrowing
      if (isSetAvatarEvent(avatarEvent)) {
        // TypeScript knows this is SetAvatarEvent
        expect(avatarEvent.avatar_id).toBe('guard-test');
      }
    });

    it('should reject malformed events', () => {
      // Arrange
      const missingType = { avatar_id: 'test' };
      const wrongType = { type: 'wrong', avatar_id: 'test' };
      const missingAvatarId = { type: 'set_avatar' };
      const wrongAvatarIdType = { type: 'set_avatar', avatar_id: 123 };
      const wrongQualityType = { type: 'set_avatar', avatar_id: 'test', quality: true };
      const wrongEncodingType = { type: 'set_avatar', avatar_id: 'test', video_encoding: 42 };

      // Act & Assert
      expect(isSetAvatarEvent(missingType)).toBe(false);
      expect(isSetAvatarEvent(wrongType)).toBe(false);
      expect(isSetAvatarEvent(missingAvatarId)).toBe(false);
      expect(isSetAvatarEvent(wrongAvatarIdType)).toBe(false);
      expect(isSetAvatarEvent(wrongQualityType)).toBe(false);
      expect(isSetAvatarEvent(wrongEncodingType)).toBe(false);
    });

    it('should handle null and undefined', () => {
      // Act & Assert
      expect(isSetAvatarEvent(null)).toBe(false);
      expect(isSetAvatarEvent(undefined)).toBe(false);
      expect(isSetAvatarEvent('')).toBe(false);
      expect(isSetAvatarEvent(123)).toBe(false);
      expect(isSetAvatarEvent([])).toBe(false);
      expect(isSetAvatarEvent({})).toBe(false);
    });
  });

  describe('Event Comparison', () => {
    it('should be distinct from SetAvatarSessionEvent', () => {
      // Arrange
      const setAvatar: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'avatar-123'
      };

      const setAvatarSession = {
        type: 'set_avatar_session',
        access_token: 'token',
        avatar_session_id: 'session'
      };

      // Act & Assert
      expect(setAvatar.type).not.toBe(setAvatarSession.type);
      expect(isSetAvatarEvent(setAvatar)).toBe(true);
      expect(isSetAvatarEvent(setAvatarSession)).toBe(false);
      
      // Different fields
      expect('avatar_id' in setAvatar).toBe(true);
      expect('access_token' in setAvatar).toBe(false);
      expect('avatar_session_id' in setAvatar).toBe(false);
    });

    it('should have different use cases from SetAvatarSessionEvent', () => {
      // SetAvatarEvent - Request to set/change avatar
      const requestAvatar: SetAvatarEvent = {
        type: 'set_avatar',
        avatar_id: 'new-avatar',
        quality: 'hd',
        video_encoding: 'H264'
      };

      // SetAvatarSessionEvent - Connect to existing HeyGen session
      const connectSession = {
        type: 'set_avatar_session',
        access_token: 'heygen-token',
        avatar_session_id: 'heygen-session'
      };

      // These serve different purposes
      expect(requestAvatar.type).toBe('set_avatar');
      expect(connectSession.type).toBe('set_avatar_session');
    });
  });
});

/**
 * Runtime validation function for SetAvatarEvent
 */
function validateSetAvatarEvent(event: any): asserts event is SetAvatarEvent {
  if (!event || typeof event !== 'object') {
    throw new Error('Event must be an object');
  }

  if (event.type !== 'set_avatar') {
    throw new Error(`Invalid event type: ${event.type}`);
  }

  if (!('avatar_id' in event)) {
    throw new Error('Missing required field: avatar_id');
  }

  if (typeof event.avatar_id !== 'string') {
    throw new Error('Invalid field type: avatar_id must be a string');
  }

  // Validate optional fields if present
  if ('quality' in event && event.quality !== undefined && typeof event.quality !== 'string') {
    throw new Error('Invalid field type: quality must be a string');
  }

  if ('video_encoding' in event && event.video_encoding !== undefined && typeof event.video_encoding !== 'string') {
    throw new Error('Invalid field type: video_encoding must be a string');
  }
}

/**
 * Type guard for SetAvatarEvent
 */
function isSetAvatarEvent(event: any): event is SetAvatarEvent {
  if (!event || typeof event !== 'object') {
    return false;
  }
  
  return (
    event.type === 'set_avatar' &&
    'avatar_id' in event &&
    typeof event.avatar_id === 'string' &&
    (event.quality === undefined || typeof event.quality === 'string') &&
    (event.video_encoding === undefined || typeof event.video_encoding === 'string')
  );
}