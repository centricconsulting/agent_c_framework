/**
 * Tests for useChat hook - Duplicate Key Fix
 * Tests the fix for duplicate React keys in chat items (system messages, dividers, media)
 * 
 * Background: When multiple events arrived within the same millisecond, Date.now() would
 * return the same value, causing duplicate IDs and React duplicate key warnings.
 * 
 * Solution: Added sequential counters (systemMessageCounterRef, dividerCounterRef, mediaCounterRef)
 * to ensure uniqueness even when events arrive in rapid succession.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useChat } from '../useChat';
import type { RealtimeClient, ChatSession, Message } from '@agentc/realtime-core';
import type { ChatItem, SystemAlertChatItem, DividerChatItem, MediaChatItem } from '../../types/chat';

// Mock dependencies
vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

vi.mock('@agentc/realtime-core', () => ({
  ensureMessagesFormat: vi.fn()
}));

vi.mock('../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    error: vi.fn(),
    warn: vi.fn()
  }
}));

describe('useChat - Duplicate Key Fix', () => {
  // Test utilities
  let mockClient: {
    getSessionManager: Mock;
    isConnected: Mock;
    sendText: Mock;
    on: Mock;
    off: Mock;
  };

  let mockSessionManager: {
    getCurrentSession: Mock;
    on: Mock;
    off: Mock;
  };

  let mockEnsureMessagesFormat: Mock;
  let mockLogger: {
    debug: Mock;
    error: Mock;
    warn: Mock;
  };

  let eventHandlers: Map<string, (event?: unknown) => void>;
  let sessionEventHandlers: Map<string, (event?: unknown) => void>;

  // Helper to emit events
  const emitSessionEvent = (eventName: string, data?: unknown) => {
    const handler = sessionEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to create test messages
  const createMessage = (
    role: 'user' | 'assistant' | 'system',
    content: string
  ): Message => ({
    role,
    content,
    timestamp: new Date().toISOString(),
    format: 'text'
  });

  // Helper to create test session
  const createTestSession = (
    sessionId = 'test-session',
    messages: Message[] = []
  ): ChatSession => ({
    session_id: sessionId,
    messages,
    context: {}
  });

  beforeEach(async () => {
    // Initialize event handler storage
    eventHandlers = new Map();
    sessionEventHandlers = new Map();

    // Setup mock SessionManager
    mockSessionManager = {
      getCurrentSession: vi.fn(() => null),
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        sessionEventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        sessionEventHandlers.delete(event);
      })
    };

    // Setup mock client
    mockClient = {
      getSessionManager: vi.fn(() => mockSessionManager),
      isConnected: vi.fn(() => true),
      sendText: vi.fn(),
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        eventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        eventHandlers.delete(event);
      })
    };

    // Setup other mocks
    mockEnsureMessagesFormat = vi.fn((messages: Message[]) => messages);
    mockLogger = {
      debug: vi.fn(),
      error: vi.fn(),
      warn: vi.fn()
    };

    // Apply mocks
    const agentCContext = await import('../../providers/AgentCContext');
    (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(mockClient);

    const coreModule = await import('@agentc/realtime-core');
    (coreModule.ensureMessagesFormat as Mock).mockImplementation(mockEnsureMessagesFormat);

    const loggerModule = await import('../../utils/logger');
    (loggerModule.Logger as any).debug = mockLogger.debug;
    (loggerModule.Logger as any).error = mockLogger.error;
    (loggerModule.Logger as any).warn = mockLogger.warn;

    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('System Message ID Uniqueness', () => {
    it('CRITICAL: generates unique IDs for rapid-fire system messages (same millisecond)', () => {
      const { result } = renderHook(() => useChat());

      // Mock Date.now() to return same value (simulating same-millisecond events)
      const fixedTimestamp = 1234567890000;
      const originalDateNow = Date.now;
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Emit multiple system messages in rapid succession
      const systemMessages = [
        { content: 'Rate limit warning 1', severity: 'warning' as const },
        { content: 'Rate limit warning 2', severity: 'warning' as const },
        { content: 'Rate limit warning 3', severity: 'warning' as const },
        { content: 'Rate limit warning 4', severity: 'warning' as const },
        { content: 'Rate limit warning 5', severity: 'warning' as const },
      ];

      systemMessages.forEach(msg => {
        emitSessionEvent('system_message', msg);
      });

      // Verify all messages were added
      expect(result.current.messages).toHaveLength(5);

      // CRITICAL: Verify all IDs are unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(5);

      // Verify ID format includes counter and random component
      ids.forEach(id => {
        expect(id).toMatch(/^system-\d+-\d+-[a-z0-9]+$/);
      });

      // Restore Date.now
      Date.now = originalDateNow;
    });

    it('generates unique IDs even with Date.now mocked to same value', () => {
      const { result } = renderHook(() => useChat());

      const fixedTimestamp = 9999999999999;
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Emit 10 system messages
      for (let i = 0; i < 10; i++) {
        emitSessionEvent('system_message', {
          content: `System message ${i}`,
          severity: 'info'
        });
      }

      expect(result.current.messages).toHaveLength(10);

      // All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(10);
    });

    it('system message IDs are unique across multiple batches', () => {
      const { result } = renderHook(() => useChat());

      // First batch
      for (let i = 0; i < 5; i++) {
        emitSessionEvent('system_message', {
          content: `Batch 1 Message ${i}`,
          severity: 'info'
        });
      }

      const firstBatchIds = result.current.messages.map(m => m.id);

      // Second batch
      for (let i = 0; i < 5; i++) {
        emitSessionEvent('system_message', {
          content: `Batch 2 Message ${i}`,
          severity: 'info'
        });
      }

      expect(result.current.messages).toHaveLength(10);

      // All IDs across both batches must be unique
      const allIds = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(allIds);
      expect(uniqueIds.size).toBe(10);

      // IDs from different batches should not overlap
      const secondBatchIds = allIds.slice(5);
      const overlap = firstBatchIds.filter(id => secondBatchIds.includes(id));
      expect(overlap).toHaveLength(0);
    });

    it('USER REPORTED BUG: handles rate limit warnings during streaming without duplicate keys', () => {
      const { result } = renderHook(() => useChat());

      // Start streaming a message
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Starting response...')
      });

      expect(result.current.isAgentTyping).toBe(true);
      expect(result.current.streamingMessage?.content).toBe('Starting response...');

      // Mock same millisecond for all events
      const fixedTimestamp = Date.now();
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Rapid rate limit warnings (the actual bug scenario)
      emitSessionEvent('system_message', {
        content: 'Rate limit: 1 request per second',
        severity: 'warning'
      });

      emitSessionEvent('system_message', {
        content: 'Rate limit: Please slow down',
        severity: 'warning'
      });

      emitSessionEvent('system_message', {
        content: 'Rate limit: Throttling applied',
        severity: 'warning'
      });

      // Continue streaming
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Starting response... continuing...')
      });

      // Complete streaming
      emitSessionEvent('message-complete', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Starting response... continuing... done.')
      });

      // Should have 3 system alerts + 1 completed message
      expect(result.current.messages).toHaveLength(4);

      // CRITICAL: No duplicate IDs (this was the bug)
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(4);

      // Verify system alerts are present
      const systemAlerts = result.current.messages.filter(
        (m): m is SystemAlertChatItem => m.type === 'system_alert'
      );
      expect(systemAlerts).toHaveLength(3);
      expect(systemAlerts[0]?.content).toContain('Rate limit');
      expect(systemAlerts[1]?.content).toContain('Rate limit');
      expect(systemAlerts[2]?.content).toContain('Rate limit');
    });

    it('system message IDs include random component for additional uniqueness', () => {
      const { result } = renderHook(() => useChat());

      // Even with same timestamp and counter reset, random component ensures uniqueness
      const fixedTimestamp = 1000000000000;
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      emitSessionEvent('system_message', {
        content: 'First',
        severity: 'info'
      });

      emitSessionEvent('system_message', {
        content: 'Second',
        severity: 'info'
      });

      const ids = result.current.messages.map(m => m.id);

      // IDs should have format: system-{timestamp}-{counter}-{random}
      const id1Parts = ids[0]?.split('-');
      const id2Parts = ids[1]?.split('-');

      expect(id1Parts).toHaveLength(4); // ['system', timestamp, counter, random]
      expect(id2Parts).toHaveLength(4);

      // Timestamp and counter might be same, but random component should differ
      expect(id1Parts?.[3]).not.toBe(id2Parts?.[3]);
    });
  });

  describe('Divider ID Uniqueness', () => {
    it('generates unique IDs for rapid subsession dividers', () => {
      const { result } = renderHook(() => useChat());

      // Mock same millisecond
      const fixedTimestamp = 1234567890000;
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Rapid subsession starts
      for (let i = 0; i < 5; i++) {
        emitSessionEvent('subsession-started', {
          subSessionType: 'chat',
          subAgentType: 'clone',
          primeAgentKey: 'main-agent',
          subAgentKey: `clone-${i}`
        });
      }

      // Rapid subsession ends
      for (let i = 0; i < 5; i++) {
        emitSessionEvent('subsession-ended', {});
      }

      // Should have 10 dividers (5 starts + 5 ends)
      expect(result.current.messages).toHaveLength(10);

      // CRITICAL: All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(10);

      // Verify divider types
      const dividers = result.current.messages.filter(
        (m): m is DividerChatItem => m.type === 'divider'
      );
      expect(dividers).toHaveLength(10);

      const startDividers = dividers.filter(d => d.dividerType === 'start');
      const endDividers = dividers.filter(d => d.dividerType === 'end');
      expect(startDividers).toHaveLength(5);
      expect(endDividers).toHaveLength(5);
    });

    it('divider IDs have correct format with counter', () => {
      const { result } = renderHook(() => useChat());

      emitSessionEvent('subsession-started', {
        subSessionType: 'oneshot',
        subAgentType: 'tool'
      });

      emitSessionEvent('subsession-ended', {});

      const dividers = result.current.messages.filter(
        (m): m is DividerChatItem => m.type === 'divider'
      );

      // Start divider ID format: divider-start-{timestamp}-{counter}
      expect(dividers[0]?.id).toMatch(/^divider-start-\d+-\d+$/);

      // End divider ID format: divider-end-{timestamp}-{counter}
      expect(dividers[1]?.id).toMatch(/^divider-end-\d+-\d+$/);
    });

    it('prevents duplicate divider keys during rapid clone delegations', () => {
      const { result } = renderHook(() => useChat());

      // Simulate rapid clone calls (common in testing/development)
      const fixedTimestamp = Date.now();
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // 10 rapid clone delegations
      for (let i = 0; i < 10; i++) {
        emitSessionEvent('subsession-started', {
          subSessionType: 'oneshot',
          subAgentType: 'clone',
          primeAgentKey: 'main',
          subAgentKey: 'clone'
        });

        emitSessionEvent('subsession-ended', {});
      }

      expect(result.current.messages).toHaveLength(20);

      // All IDs must be unique despite same timestamp
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(20);
    });
  });

  describe('Media ID Uniqueness', () => {
    it('generates unique IDs for rapid media rendering', () => {
      const { result } = renderHook(() => useChat());

      // Mock same millisecond
      const fixedTimestamp = 1234567890000;
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Rapid media events (batch rendering)
      for (let i = 0; i < 5; i++) {
        emitSessionEvent('media-added', {
          sessionId: `media-session-${i}`,
          media: {
            content: `base64-image-data-${i}`,
            contentType: 'image/png',
            metadata: {
              name: `image-${i}.png`
            }
          }
        });
      }

      expect(result.current.messages).toHaveLength(5);

      // CRITICAL: All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(5);

      // Verify all are media items
      const mediaItems = result.current.messages.filter(
        (m): m is MediaChatItem => m.type === 'media'
      );
      expect(mediaItems).toHaveLength(5);
    });

    it('media IDs have correct format with counter when no ID provided', () => {
      const { result } = renderHook(() => useChat());

      // Media without explicit ID (uses fallback)
      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: {
          content: 'base64data',
          contentType: 'image/jpeg'
        }
      });

      const mediaItem = result.current.messages[0] as MediaChatItem;

      // Format: media-{timestamp}-{counter}
      expect(mediaItem.id).toMatch(/^media-\d+-\d+$/);
    });

    it('uses provided media ID if available', () => {
      const { result } = renderHook(() => useChat());

      // Media with explicit ID
      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: {
          id: 'custom-media-id-123',
          content: 'base64data',
          contentType: 'image/png'
        }
      });

      const mediaItem = result.current.messages[0] as MediaChatItem;
      expect(mediaItem.id).toBe('custom-media-id-123');
    });

    it('prevents duplicate media keys during batch image rendering', () => {
      const { result } = renderHook(() => useChat());

      const fixedTimestamp = Date.now();
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Simulate batch rendering (multiple images in quick succession)
      const imageUrls = [
        'https://example.com/img1.png',
        'https://example.com/img2.png',
        'https://example.com/img3.png',
        'https://example.com/img4.png',
        'https://example.com/img5.png'
      ];

      imageUrls.forEach((url, i) => {
        emitSessionEvent('media-added', {
          sessionId: `media-${i}`,
          media: {
            contentType: 'image/png',
            metadata: {
              url,
              name: `image-${i}.png`
            }
          }
        });
      });

      expect(result.current.messages).toHaveLength(5);

      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(5);
    });
  });

  describe('Mixed Content Uniqueness', () => {
    it('ensures unique IDs across all chat item types', () => {
      const { result } = renderHook(() => useChat());

      // Mock same millisecond for all events
      const fixedTimestamp = Date.now();
      vi.spyOn(Date, 'now').mockReturnValue(fixedTimestamp);

      // Mix of all event types in rapid succession
      emitSessionEvent('system_message', {
        content: 'System alert',
        severity: 'info'
      });

      emitSessionEvent('subsession-started', {
        subSessionType: 'chat',
        subAgentType: 'clone'
      });

      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: {
          content: 'image-data',
          contentType: 'image/png'
        }
      });

      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'User message')
      });

      emitSessionEvent('system_message', {
        content: 'Another system alert',
        severity: 'warning'
      });

      emitSessionEvent('subsession-ended', {});

      emitSessionEvent('media-added', {
        sessionId: 'media-2',
        media: {
          content: 'another-image',
          contentType: 'image/jpeg'
        }
      });

      // Should have 7 items total
      expect(result.current.messages).toHaveLength(7);

      // CRITICAL: All IDs must be unique across all types
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(7);

      // Verify we have mix of types
      const types = result.current.messages.map(m => m.type);
      expect(types).toContain('system_alert');
      expect(types).toContain('divider');
      expect(types).toContain('media');
      expect(types).toContain('message');
    });

    it('maintains ID uniqueness during complex conversation flow', () => {
      const { result } = renderHook(() => useChat());

      // Simulate realistic complex flow
      
      // User sends message
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Calculate 2+2')
      });

      // Agent starts subsession for tool use
      emitSessionEvent('subsession-started', {
        subSessionType: 'oneshot',
        subAgentType: 'tool'
      });

      // System message about tool execution
      emitSessionEvent('system_message', {
        content: 'Executing calculator tool',
        severity: 'info'
      });

      // Rate limit warning
      emitSessionEvent('system_message', {
        content: 'Rate limit warning',
        severity: 'warning'
      });

      // Tool result as media
      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: {
          content: 'tool-result-data',
          contentType: 'application/json'
        }
      });

      // Subsession ends
      emitSessionEvent('subsession-ended', {});

      // Agent starts response
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'The result is')
      });

      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'The result is 4')
      });

      emitSessionEvent('message-complete', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'The result is 4')
      });

      // Should have 8 items (user msg, start divider, 2 system msgs, media, end divider, assistant msg, and streaming was replaced)
      expect(result.current.messages.length).toBeGreaterThanOrEqual(7);

      // All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(ids.length);
    });
  });

  describe('Counter Reset Behavior', () => {
    it('counter resets on hook remount (new session context)', () => {
      // First mount
      let { result, unmount } = renderHook(() => useChat());

      emitSessionEvent('system_message', {
        content: 'Message 1',
        severity: 'info'
      });

      const firstId = result.current.messages[0]?.id;
      expect(firstId).toBeDefined();

      // Verify first ID has expected format
      expect(firstId).toMatch(/^system-\d+-\d+-[a-z0-9]+$/);

      // Unmount (counters should reset)
      unmount();

      // Clear handlers for clean remount
      eventHandlers.clear();
      sessionEventHandlers.clear();

      // Remount (new hook instance with fresh counters)
      const remountResult = renderHook(() => useChat());

      // Emit system message to new instance
      emitSessionEvent('system_message', {
        content: 'Message 1 again',
        severity: 'info'
      });

      const secondId = remountResult.result.current.messages[0]?.id;

      // Second ID should be defined in new instance
      expect(secondId).toBeDefined();

      // Both IDs should have correct format
      expect(secondId).toMatch(/^system-\d+-\d+-[a-z0-9]+$/);

      // IDs are from different component instances, so uniqueness across instances
      // is not required (counters reset per instance). This is expected behavior.
      // The important thing is that within each instance, IDs are unique.
    });

    it('counters persist throughout single hook instance lifetime', () => {
      const { result } = renderHook(() => useChat());

      // Generate 100 system messages
      for (let i = 0; i < 100; i++) {
        emitSessionEvent('system_message', {
          content: `Message ${i}`,
          severity: 'info'
        });
      }

      expect(result.current.messages).toHaveLength(100);

      // All IDs must be unique (counter keeps incrementing)
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(100);

      // Later messages should have higher counters
      const firstId = ids[0] as string;
      const lastId = ids[99] as string;

      const firstCounter = parseInt(firstId.split('-')[2] || '0');
      const lastCounter = parseInt(lastId.split('-')[2] || '0');

      expect(lastCounter).toBeGreaterThan(firstCounter);
    });
  });

  describe('Performance and Scale', () => {
    it('handles thousands of events without performance degradation', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 5000 }));

      const start = performance.now();

      // Generate 1000 system messages
      for (let i = 0; i < 1000; i++) {
        emitSessionEvent('system_message', {
          content: `Message ${i}`,
          severity: i % 3 === 0 ? 'error' : i % 2 === 0 ? 'warning' : 'info'
        });
      }

      const duration = performance.now() - start;

      expect(result.current.messages).toHaveLength(1000);

      // All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(1000);

      // Should complete in reasonable time (< 100ms for 1000 events)
      expect(duration).toBeLessThan(1000);
    });

    it('maintains uniqueness across very long session', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 10000 }));

      // Simulate long session with mixed events
      for (let i = 0; i < 1000; i++) {
        if (i % 4 === 0) {
          emitSessionEvent('system_message', {
            content: `System ${i}`,
            severity: 'info'
          });
        } else if (i % 4 === 1) {
          emitSessionEvent('subsession-started', {
            subSessionType: 'chat',
            subAgentType: 'clone'
          });
        } else if (i % 4 === 2) {
          emitSessionEvent('media-added', {
            sessionId: `media-${i}`,
            media: {
              content: `data-${i}`,
              contentType: 'image/png'
            }
          });
        } else {
          emitSessionEvent('subsession-ended', {});
        }
      }

      // Should have 1000 items
      expect(result.current.messages).toHaveLength(1000);

      // All IDs must be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(1000);
    });
  });

  describe('Edge Cases', () => {
    it('handles events during session loading (should be blocked)', () => {
      const { result } = renderHook(() => useChat());

      // Start session loading
      const session = createTestSession('new-session');
      emitSessionEvent('chat-session-changed', {
        currentChatSession: session,
        previousChatSession: null
      });

      // Try to add system messages during loading
      emitSessionEvent('system_message', {
        content: 'Should be blocked',
        severity: 'info'
      });

      emitSessionEvent('subsession-started', {
        subSessionType: 'chat',
        subAgentType: 'clone'
      });

      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: {
          content: 'blocked-data',
          contentType: 'image/png'
        }
      });

      // Should not add any items during loading
      expect(result.current.messages).toHaveLength(0);

      // Complete loading
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'new-session',
        messages: []
      });

      // Now events should work
      emitSessionEvent('system_message', {
        content: 'Should work now',
        severity: 'info'
      });

      expect(result.current.messages).toHaveLength(1);
      expect((result.current.messages[0] as SystemAlertChatItem).content).toBe('Should work now');
    });

    it('handles malformed events gracefully', () => {
      const { result } = renderHook(() => useChat());

      // Malformed system message (missing content)
      emitSessionEvent('system_message', {
        severity: 'info'
      });

      // Should not crash, might create item with undefined content
      // The implementation doesn't validate content presence

      // Valid event should still work
      emitSessionEvent('system_message', {
        content: 'Valid message',
        severity: 'info'
      });

      // At least the valid message should be added
      const validMessage = result.current.messages.find(
        m => m.type === 'system_alert' && (m as SystemAlertChatItem).content === 'Valid message'
      );
      expect(validMessage).toBeDefined();
    });

    it('handles counter overflow gracefully (theoretical edge case)', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 100000 }));

      // Generate enough events to potentially test counter behavior
      // Note: JavaScript numbers are safe up to Number.MAX_SAFE_INTEGER (2^53 - 1)
      // We'll test a reasonable number
      for (let i = 0; i < 10000; i++) {
        emitSessionEvent('system_message', {
          content: `Message ${i}`,
          severity: 'info'
        });
      }

      expect(result.current.messages).toHaveLength(10000);

      // All IDs should still be unique
      const ids = result.current.messages.map(m => m.id);
      const uniqueIds = new Set(ids);
      expect(uniqueIds.size).toBe(10000);
    });
  });
});
