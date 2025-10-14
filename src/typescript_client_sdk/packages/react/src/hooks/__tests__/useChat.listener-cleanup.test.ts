/**
 * Tests for useChat hook - Listener Cleanup Fix
 * Validates that event listeners are properly registered and cleaned up
 * to prevent duplicate event firing
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useChat } from '../useChat';
import type { RealtimeClient, ChatSession, Message } from '@agentc/realtime-core';

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
    error: vi.fn()
  }
}));

describe('useChat - Listener Cleanup Fix', () => {
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
    emit: Mock; // Add emit for testing
  };

  let eventHandlers: Map<string, Set<(event?: unknown) => void>>;
  let sessionEventHandlers: Map<string, Set<(event?: unknown) => void>>;

  // Helper to emit events to ALL registered handlers
  const emitClientEvent = (eventName: string, data?: unknown) => {
    const handlers = eventHandlers.get(eventName);
    if (handlers) {
      act(() => {
        handlers.forEach(handler => handler(data));
      });
    }
  };

  const emitSessionEvent = (eventName: string, data?: unknown) => {
    const handlers = sessionEventHandlers.get(eventName);
    if (handlers) {
      act(() => {
        handlers.forEach(handler => handler(data));
      });
    }
  };

  // Helper to count registered handlers
  const getHandlerCount = (eventName: string, isSession = false): number => {
    const map = isSession ? sessionEventHandlers : eventHandlers;
    return map.get(eventName)?.size || 0;
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
    // Initialize event handler storage - IMPORTANT: Use Sets to track multiple handlers
    eventHandlers = new Map();
    sessionEventHandlers = new Map();

    // Setup mock SessionManager
    mockSessionManager = {
      getCurrentSession: vi.fn(() => null),
      // Track ALL registered handlers in a Set (to catch duplicates)
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        if (!sessionEventHandlers.has(event)) {
          sessionEventHandlers.set(event, new Set());
        }
        sessionEventHandlers.get(event)!.add(handler);
      }),
      // Remove specific handler by reference
      off: vi.fn((event: string, handler: (event?: unknown) => void) => {
        const handlers = sessionEventHandlers.get(event);
        if (handlers) {
          const hadHandler = handlers.has(handler);
          handlers.delete(handler);
          return hadHandler; // Return true if handler was actually removed
        }
        return false;
      }),
      emit: vi.fn()
    };

    // Setup mock client
    mockClient = {
      getSessionManager: vi.fn(() => mockSessionManager),
      isConnected: vi.fn(() => true),
      sendText: vi.fn(),
      // Track ALL registered handlers in a Set (to catch duplicates)
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        if (!eventHandlers.has(event)) {
          eventHandlers.set(event, new Set());
        }
        eventHandlers.get(event)!.add(handler);
      }),
      // Remove specific handler by reference
      off: vi.fn((event: string, handler: (event?: unknown) => void) => {
        const handlers = eventHandlers.get(event);
        if (handlers) {
          const hadHandler = handlers.has(handler);
          handlers.delete(handler);
          return hadHandler; // Return true if handler was actually removed
        }
        return false;
      })
    };

    // Apply mocks
    const agentCContext = await import('../../providers/AgentCContext');
    (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(mockClient);

    const coreModule = await import('@agentc/realtime-core');
    (coreModule.ensureMessagesFormat as Mock).mockImplementation((messages: Message[]) => messages);

    const loggerModule = await import('../../utils/logger');
    (loggerModule.Logger as any).debug = vi.fn();
    (loggerModule.Logger as any).error = vi.fn();

    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Listener Registration and Cleanup', () => {
    it('registers exactly one handler per event on mount', () => {
      renderHook(() => useChat());

      // Verify exactly one handler registered for each client event
      expect(getHandlerCount('user_turn_start')).toBe(1);
      expect(getHandlerCount('user_turn_end')).toBe(1);
      expect(getHandlerCount('chat-session-changed', true)).toBe(1);

      // Verify exactly one handler registered for each session event
      expect(getHandlerCount('message-added', true)).toBe(1);
      expect(getHandlerCount('message-streaming', true)).toBe(1);
      expect(getHandlerCount('message-complete', true)).toBe(1);
      expect(getHandlerCount('session-messages-loaded', true)).toBe(1);
      expect(getHandlerCount('subsession-started', true)).toBe(1);
      expect(getHandlerCount('subsession-ended', true)).toBe(1);
      expect(getHandlerCount('media-added', true)).toBe(1);
      expect(getHandlerCount('system_message', true)).toBe(1);
    });

    it('does NOT duplicate handlers on rerender', () => {
      const { rerender } = renderHook(() => useChat());

      // Initial count
      const initialSessionCount = getHandlerCount('message-added', true);
      const initialClientCount = getHandlerCount('user_turn_start');

      expect(initialSessionCount).toBe(1);
      expect(initialClientCount).toBe(1);

      // Rerender multiple times
      rerender();
      rerender();
      rerender();

      // Should still have exactly one handler per event
      expect(getHandlerCount('message-added', true)).toBe(1);
      expect(getHandlerCount('user_turn_start')).toBe(1);
      expect(getHandlerCount('message-streaming', true)).toBe(1);
      expect(getHandlerCount('media-added', true)).toBe(1);
    });

    it('properly removes handlers on unmount', () => {
      const { unmount } = renderHook(() => useChat());

      // Verify handlers are registered
      expect(getHandlerCount('message-added', true)).toBe(1);
      expect(getHandlerCount('user_turn_start')).toBe(1);

      unmount();

      // Verify all handlers were removed
      expect(getHandlerCount('user_turn_start')).toBe(0);
      expect(getHandlerCount('user_turn_end')).toBe(0);
      expect(getHandlerCount('chat-session-changed', true)).toBe(0);
      expect(getHandlerCount('message-added', true)).toBe(0);
      expect(getHandlerCount('message-streaming', true)).toBe(0);
      expect(getHandlerCount('message-complete', true)).toBe(0);
      expect(getHandlerCount('session-messages-loaded', true)).toBe(0);
      expect(getHandlerCount('subsession-started', true)).toBe(0);
      expect(getHandlerCount('subsession-ended', true)).toBe(0);
      expect(getHandlerCount('media-added', true)).toBe(0);
      expect(getHandlerCount('system_message', true)).toBe(0);
    });

    it('cleanup removes the correct handler references', () => {
      const { unmount } = renderHook(() => useChat());

      // Get the registered handler
      const messageHandlers = sessionEventHandlers.get('message-added');
      expect(messageHandlers?.size).toBe(1);
      const originalHandler = Array.from(messageHandlers!)[0];

      unmount();

      // Verify the EXACT handler was removed (reference equality)
      const handlersAfterCleanup = sessionEventHandlers.get('message-added');
      expect(handlersAfterCleanup?.has(originalHandler)).toBe(false);
    });

    it('handles mount -> unmount -> remount cycle cleanly', () => {
      // First mount
      const { unmount: unmount1 } = renderHook(() => useChat());
      expect(getHandlerCount('message-added', true)).toBe(1);

      // Unmount
      unmount1();
      expect(getHandlerCount('message-added', true)).toBe(0);

      // Second mount
      const { unmount: unmount2 } = renderHook(() => useChat());
      expect(getHandlerCount('message-added', true)).toBe(1);

      // Clean up
      unmount2();
      expect(getHandlerCount('message-added', true)).toBe(0);
    });
  });

  describe('Duplicate Event Prevention', () => {
    it('fires message-added handler only once per event', () => {
      const { result } = renderHook(() => useChat());

      let callCount = 0;
      const originalMessageCount = result.current.messages.length;

      // Track calls to the message-added handler
      // We do this by emitting the event and checking the result
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test message')
      });

      // Should have exactly one new message
      expect(result.current.messages.length).toBe(originalMessageCount + 1);
      
      // Verify handler count is still 1
      expect(getHandlerCount('message-added', true)).toBe(1);
    });

    it('fires session-messages-loaded handler only once', () => {
      const { result } = renderHook(() => useChat());

      const loadedMessages = [
        createMessage('user', 'Message 1'),
        createMessage('assistant', 'Message 2')
      ];

      // Emit once
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test-session',
        messages: loadedMessages
      });

      // Should have exactly the loaded messages (not doubled)
      expect(result.current.messages.length).toBe(loadedMessages.length);
      expect(result.current.messages[0]?.content).toBe('Message 1');
      expect(result.current.messages[1]?.content).toBe('Message 2');
      
      // Verify handler count is still 1
      expect(getHandlerCount('session-messages-loaded', true)).toBe(1);
    });

    it('fires media-added handler only once per event', () => {
      const { result } = renderHook(() => useChat());

      const mediaEvent = {
        sessionId: 'media-1',
        media: {
          id: 'media-item-1',
          content: 'base64content',
          contentType: 'image/png',
          timestamp: new Date().toISOString()
        }
      };

      emitSessionEvent('media-added', mediaEvent);

      // Should have exactly one media item
      const mediaItems = result.current.messages.filter(m => m.type === 'media');
      expect(mediaItems.length).toBe(1);
      expect(mediaItems[0]?.contentType).toBe('image/png');
      
      // Verify handler count is still 1
      expect(getHandlerCount('media-added', true)).toBe(1);
    });

    it('fires subsession-started handler only once', () => {
      const { result } = renderHook(() => useChat());

      const subsessionEvent = {
        subSessionType: 'chat' as const,
        subAgentType: 'clone' as const,
        primeAgentKey: 'parent-agent',
        subAgentKey: 'child-agent'
      };

      emitSessionEvent('subsession-started', subsessionEvent);

      // Should have exactly one divider
      const dividers = result.current.messages.filter(m => m.type === 'divider');
      expect(dividers.length).toBe(1);
      expect((dividers[0] as any).dividerType).toBe('start');
      
      // Verify handler count is still 1
      expect(getHandlerCount('subsession-started', true)).toBe(1);
    });
  });

  describe('StrictMode Simulation', () => {
    it('handles double mount/unmount/mount cycle (StrictMode behavior)', () => {
      // First mount
      const { unmount: unmount1 } = renderHook(() => useChat());
      expect(getHandlerCount('message-added', true)).toBe(1);

      // StrictMode unmount
      unmount1();
      expect(getHandlerCount('message-added', true)).toBe(0);

      // StrictMode remount
      const { result: result2 } = renderHook(() => useChat());
      expect(getHandlerCount('message-added', true)).toBe(1);

      // Emit event - should only fire once
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test')
      });

      expect(result2.current.messages.length).toBe(1);
      expect(result2.current.messages[0]?.content).toBe('Test');
    });

    it('no duplicate events after StrictMode double-mount', () => {
      // Simulate StrictMode: mount → unmount → remount
      const { unmount: unmount1 } = renderHook(() => useChat());
      unmount1();
      const { result } = renderHook(() => useChat());

      // Load session messages
      const messages = [
        createMessage('user', 'Question'),
        createMessage('assistant', 'Answer')
      ];

      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-1',
        messages
      });

      // Should have exactly 2 messages (not 4 from duplicates)
      expect(result.current.messages.length).toBe(2);
      expect(result.current.messages[0]?.content).toBe('Question');
      expect(result.current.messages[1]?.content).toBe('Answer');
    });
  });

  describe('Multiple Hook Instances', () => {
    it('each hook instance has independent handlers', () => {
      const { result: result1 } = renderHook(() => useChat());
      const { result: result2 } = renderHook(() => useChat());

      // Should have 2 handlers for each event (one per hook)
      expect(getHandlerCount('message-added', true)).toBe(2);
      expect(getHandlerCount('user_turn_start')).toBe(2);

      // Emit event to both
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test')
      });

      // Both should receive the message
      expect(result1.current.messages.length).toBe(1);
      expect(result2.current.messages.length).toBe(1);
    });

    it('unmounting one instance does not affect others', () => {
      const { unmount: unmount1 } = renderHook(() => useChat());
      const { result: result2 } = renderHook(() => useChat());

      // Should have 2 handlers
      expect(getHandlerCount('message-added', true)).toBe(2);

      // Unmount first instance
      unmount1();

      // Should have 1 handler remaining
      expect(getHandlerCount('message-added', true)).toBe(1);

      // Second instance should still work
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test')
      });

      expect(result2.current.messages.length).toBe(1);
    });
  });

  describe('Session Change Listener Stability', () => {
    it('maintains single listener across session changes', () => {
      const { result } = renderHook(() => useChat());

      // Initial handler count
      expect(getHandlerCount('session-messages-loaded', true)).toBe(1);

      // Change session
      const session1 = createTestSession('session-1', []);
      emitSessionEvent('chat-session-changed', { currentChatSession: session1, previousChatSession: null });

      // Should still have exactly one handler
      expect(getHandlerCount('session-messages-loaded', true)).toBe(1);

      // Load messages for session 1
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-1',
        messages: [createMessage('user', 'Session 1 message')]
      });

      expect(result.current.messages.length).toBe(1);

      // Change to session 2
      const session2 = createTestSession('session-2', []);
      emitSessionEvent('chat-session-changed', { currentChatSession: session2, previousChatSession: null });

      // Should STILL have exactly one handler (not accumulated)
      expect(getHandlerCount('session-messages-loaded', true)).toBe(1);

      // Load messages for session 2
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-2',
        messages: [createMessage('user', 'Session 2 message')]
      });

      // Should have new session's message (not duplicated)
      expect(result.current.messages.length).toBe(1);
      expect(result.current.messages[0]?.content).toBe('Session 2 message');
    });
  });

  describe('Diagnostic Logging Validation', () => {
    it('logs registration and cleanup with matching IDs', () => {
      const consoleSpy = vi.spyOn(console, 'log');

      const { unmount } = renderHook(() => useChat());

      // Should have logged registration
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('[useChat:DIAGNOSTIC] 🔵 Registering listeners')
      );

      // Get the registration ID from the log
      const registerCall = consoleSpy.mock.calls.find(call =>
        call[0]?.includes('Registering listeners')
      );
      const registrationId = registerCall?.[0].match(/#(\d+)/)?.[1];

      unmount();

      // Should have logged cleanup with same ID
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining(`[useChat:DIAGNOSTIC] 🔴 Cleaning up listeners #${registrationId}`)
      );
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining(`[useChat:DIAGNOSTIC] ✅ Cleanup complete for #${registrationId}`)
      );

      consoleSpy.mockRestore();
    });

    it('each component instance has independent listener count', () => {
      const consoleSpy = vi.spyOn(console, 'log');

      // First instance - starts at #1
      const { unmount: unmount1 } = renderHook(() => useChat());
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('Registering listeners #1')
      );

      unmount1();
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('Cleaning up listeners #1')
      );

      // Second instance - also starts at #1 (new component, new ref)
      consoleSpy.mockClear();
      const { unmount: unmount2 } = renderHook(() => useChat());
      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('Registering listeners #1')
      );

      unmount2();

      consoleSpy.mockRestore();
    });
  });
});
