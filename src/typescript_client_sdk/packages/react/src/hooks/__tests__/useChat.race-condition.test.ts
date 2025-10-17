/**
 * Tests for useChat hook - Race Condition Fix for Session Switching
 * Tests the fix for message accumulation when switching sessions
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useChat } from '../useChat';
import type { UseChatOptions } from '../useChat';
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
    error: vi.fn(),
    warn: vi.fn()
  }
}));

describe('useChat - Race Condition Fix for Session Switching', () => {
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
  const emitClientEvent = (eventName: string, data?: unknown) => {
    const handler = eventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

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
    messages?: Message[]
  ): ChatSession => {
    const session: ChatSession = {
      session_id: sessionId,
      context: {}
    } as ChatSession;
    
    // Only add messages property if explicitly provided
    if (messages !== undefined) {
      (session as any).messages = messages;
    }
    
    return session;
  };

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

  describe('Critical Race Condition Scenarios', () => {
    it('CRITICAL: Rapid session switching (A→B→A) prevents message contamination', () => {
      const { result } = renderHook(() => useChat());

      // Session A with messages
      const sessionA = createTestSession('session-a');
      const sessionAMessages = [
        createMessage('user', 'Message from Session A'),
        createMessage('assistant', 'Response from Session A')
      ];

      // Session B with different messages
      const sessionB = createTestSession('session-b');
      const sessionBMessages = [
        createMessage('user', 'Message from Session B'),
        createMessage('assistant', 'Response from Session B')
      ];

      // Initial session A
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionA,
        previousChatSession: null
      });
      // Important: No sessionId in event, simulating EventStreamProcessor behavior
      emitSessionEvent('session-messages-loaded', {
        messages: sessionAMessages
      });

      // Verify Session A messages loaded
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Message from Session A');
      expect(result.current.messages[1]?.content).toBe('Response from Session A');

      // Rapidly switch to Session B
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionB,
        previousChatSession: sessionA
      });
      
      // CRITICAL: Messages should be cleared immediately
      expect(result.current.messages).toHaveLength(0);
      
      // Late-arriving message from Session A (simulating race condition)
      emitSessionEvent('message-added', {
        sessionId: 'msg-from-a',
        message: createMessage('user', 'Late message from Session A')
      });

      // Should NOT add the message during loading
      expect(result.current.messages).toHaveLength(0);

      // Session B messages arrive
      emitSessionEvent('session-messages-loaded', {
        messages: sessionBMessages
      });

      // Only Session B messages should be present
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Message from Session B');
      expect(result.current.messages[1]?.content).toBe('Response from Session B');

      // Switch back to Session A
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionA,
        previousChatSession: sessionB
      });

      // Messages should be cleared immediately again
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('session-messages-loaded', {
        messages: sessionAMessages
      });

      // Only Session A messages should be present, no contamination
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Message from Session A');
      expect(result.current.messages[1]?.content).toBe('Response from Session A');
    });

    it('CRITICAL: Session with many messages → few messages shows complete replacement', () => {
      const { result } = renderHook(() => useChat());

      // Session with many messages
      const manyMessages = Array.from({ length: 50 }, (_, i) => 
        createMessage(i % 2 === 0 ? 'user' : 'assistant', `Message ${i + 1}`)
      );
      
      const sessionMany = createTestSession('session-many');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionMany,
        previousChatSession: null
      });
      emitSessionEvent('session-messages-loaded', {
        messages: manyMessages
      });

      // Verify many messages loaded (limited by default maxMessages)
      expect(result.current.messages.length).toBeGreaterThan(10);
      const firstMessageContent = result.current.messages[0]?.content;

      // Switch to session with few messages
      const fewMessages = [
        createMessage('user', 'Only one question'),
        createMessage('assistant', 'Only one response')
      ];
      
      const sessionFew = createTestSession('session-few');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionFew,
        previousChatSession: sessionMany
      });

      // CRITICAL: All old messages should be cleared immediately
      expect(result.current.messages).toHaveLength(0);

      // Load few messages
      emitSessionEvent('session-messages-loaded', {
        messages: fewMessages
      });

      // Should only have 2 messages now, not any from previous session
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Only one question');
      expect(result.current.messages[1]?.content).toBe('Only one response');
      
      // Verify none of the old messages remain
      expect(result.current.messages.some(m => m.content === firstMessageContent)).toBe(false);
    });

    it('CRITICAL: Empty session loading clears chat completely', () => {
      const { result } = renderHook(() => useChat());

      // Start with a session containing messages
      const messagesInitial = [
        createMessage('user', 'Initial question'),
        createMessage('assistant', 'Initial response'),
        createMessage('user', 'Follow-up'),
        createMessage('assistant', 'Follow-up response')
      ];
      
      const sessionWithMessages = createTestSession('session-with-messages');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionWithMessages,
        previousChatSession: null
      });
      emitSessionEvent('session-messages-loaded', {
        messages: messagesInitial
      });

      expect(result.current.messages).toHaveLength(4);

      // Switch to empty session
      const emptySession = createTestSession('empty-session');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: emptySession,
        previousChatSession: sessionWithMessages
      });

      // CRITICAL: Messages should be cleared immediately
      expect(result.current.messages).toHaveLength(0);

      // Load empty messages array (session has no messages)
      emitSessionEvent('session-messages-loaded', {
        messages: []
      });

      // Chat should remain empty
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('CRITICAL: Concurrent events during switch are handled cleanly', () => {
      const { result } = renderHook(() => useChat());

      // Start with Session A
      const sessionA = createTestSession('session-a');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionA,
        previousChatSession: null
      });
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Session A message')]
      });

      // Start streaming in Session A
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-a',
        message: createMessage('assistant', 'Streaming in A...')
      });
      
      expect(result.current.streamingMessage?.content).toBe('Streaming in A...');
      expect(result.current.isAgentTyping).toBe(true);

      // Switch to Session B while streaming is active
      const sessionB = createTestSession('session-b');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionB,
        previousChatSession: sessionA
      });

      // CRITICAL: Streaming should be cleared immediately
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);
      expect(result.current.messages).toHaveLength(0);

      // Late streaming event from Session A arrives
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-a',
        message: createMessage('assistant', 'Still streaming in A...')
      });

      // Should NOT update streaming (we're in loading state for Session B)
      expect(result.current.streamingMessage).toBeNull();

      // Complete message from Session A arrives late
      emitSessionEvent('message-complete', {
        sessionId: 'stream-a',
        message: createMessage('assistant', 'Completed in A')
      });

      // Should NOT add the message
      expect(result.current.messages).toHaveLength(0);

      // Session B messages load
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Session B message')]
      });

      // Only Session B message should be present
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Session B message');
    });
  });

  describe('Loading State Management', () => {
    it('blocks all message events during session transition', () => {
      const { result } = renderHook(() => useChat());

      // Switch to new session (triggers loading state)
      const session = createTestSession('test-session');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session,
        previousChatSession: null
      });

      // Messages should be cleared
      expect(result.current.messages).toHaveLength(0);

      // Try to add various events during loading
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Should be blocked')
      });
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Should be blocked')
      });
      expect(result.current.streamingMessage).toBeNull();

      emitSessionEvent('message-complete', {
        sessionId: 'complete-1',
        message: createMessage('assistant', 'Should be blocked')
      });
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('subsession-started', {
        subSessionType: 'chat'
      });
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('media-added', {
        sessionId: 'media-1',
        media: { content: 'test', contentType: 'image/png' }
      });
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('system_message', {
        content: 'Should be blocked'
      });
      expect(result.current.messages).toHaveLength(0);

      // Complete the session loading
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Loaded message')]
      });

      // Now only the loaded message should be present
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Loaded message');
    });

    it('validates session ID before accepting messages', () => {
      const { result } = renderHook(() => useChat());

      // Switch to Session A
      const sessionA = createTestSession('session-a');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionA,
        previousChatSession: null
      });

      // Switch to Session B before A finishes loading
      const sessionB = createTestSession('session-b');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: sessionB,
        previousChatSession: sessionA
      });

      // Late messages from Session A arrive (with sessionId to test validation)
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-a',
        messages: [createMessage('user', 'From Session A')]
      });

      // Should NOT load Session A messages (we're expecting Session B)
      expect(result.current.messages).toHaveLength(0);

      // Correct messages from Session B arrive (with sessionId to test validation)
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-b',
        messages: [createMessage('user', 'From Session B')]
      });

      // Should load Session B messages
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('From Session B');
    });

    it('handles session change with new event pattern', () => {
      const { result } = renderHook(() => useChat());

      // Messages for the session
      const sessionMessages = [
        createMessage('user', 'Session message 1'),
        createMessage('assistant', 'Session message 2')
      ];
      
      const session = createTestSession('test-session');
      
      // NEW: chat-session-changed clears messages and sets loading state
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session,
        previousChatSession: null
      });

      // Messages should be cleared during loading
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.currentSessionId).toBe('test-session');

      // NEW: session-messages-loaded delivers the messages
      emitSessionEvent('session-messages-loaded', {
        messages: sessionMessages
      });

      // Now messages should be loaded
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Session message 1');
      expect(result.current.messages[1]?.content).toBe('Session message 2');

      // Try to send another load event (should be accepted as loading is complete)
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'New message')
      });

      // New message should be added
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[2]?.content).toBe('New message');
    });
  });

  describe('Edge Cases and Error Scenarios', () => {
    it('handles multiple rapid session changes', () => {
      const { result } = renderHook(() => useChat());

      // Rapidly switch between multiple sessions
      for (let i = 0; i < 10; i++) {
        const session = createTestSession(`session-${i}`, []);
        const prevSession = i > 0 ? createTestSession(`session-${i-1}`, []) : null;
        emitSessionEvent('chat-session-changed', { 
          currentChatSession: session,
          previousChatSession: prevSession
        });
      }

      // Messages should be empty (last session is loading)
      expect(result.current.messages).toHaveLength(0);

      // Load messages for the last session
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Final session')]
      });

      // Only the last session's messages should be loaded
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Final session');

      // Try to load messages for an earlier session (with sessionId to test validation)
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'session-5',
        messages: [createMessage('user', 'Earlier session')]
      });

      // Should NOT change (wrong session ID)
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Final session');
    });

    it('recovers gracefully from interrupted session switches', () => {
      const { result } = renderHook(() => useChat());

      // Start switching to a session
      const session1 = createTestSession('session-1');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session1,
        previousChatSession: null
      });

      // Interrupt with another session switch
      const session2 = createTestSession('session-2');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session2,
        previousChatSession: session1
      });

      // Complete the second session loading
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Session 2')]
      });

      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Session 2');

      // Verify normal operation resumes
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('assistant', 'New message')
      });

      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[1]?.content).toBe('New message');
    });

    it('handles session switch during active turn', () => {
      const { result } = renderHook(() => useChat());

      // Load initial session
      const session1 = createTestSession('session-1');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session1,
        previousChatSession: null
      });
      emitSessionEvent('session-messages-loaded', {
        messages: [createMessage('user', 'Initial')]
      });

      // Start user turn
      emitClientEvent('user_turn_start');
      expect(result.current.isAgentTyping).toBe(false);

      // End user turn (agent starts typing)
      emitClientEvent('user_turn_end');
      expect(result.current.isAgentTyping).toBe(true);

      // Switch session during agent's turn
      const session2 = createTestSession('session-2');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session2,
        previousChatSession: session1
      });

      // Typing indicator should be cleared
      expect(result.current.isAgentTyping).toBe(false);
      expect(result.current.messages).toHaveLength(0);

      // Complete session 2 loading
      emitSessionEvent('session-messages-loaded', {
        messages: []
      });

      // Verify clean state
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('maintains data consistency with maxMessages during session switch', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 3 }));

      // Load session with many messages
      const manyMessages = Array.from({ length: 10 }, (_, i) =>
        createMessage('user', `Message ${i + 1}`)
      );
      
      const session1 = createTestSession('session-1');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session1,
        previousChatSession: null
      });
      emitSessionEvent('session-messages-loaded', {
        messages: manyMessages
      });

      // Should be limited to 3 messages
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[0]?.content).toBe('Message 8');
      expect(result.current.messages[2]?.content).toBe('Message 10');

      // Switch to new session
      const session2 = createTestSession('session-2');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session2,
        previousChatSession: session1
      });

      // Should clear immediately
      expect(result.current.messages).toHaveLength(0);

      // Load new messages
      emitSessionEvent('session-messages-loaded', {
        messages: [
          createMessage('user', 'New 1'),
          createMessage('user', 'New 2')
        ]
      });

      // Should have only new messages
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('New 1');
      expect(result.current.messages[1]?.content).toBe('New 2');
    });
  });

  describe('Performance and Memory Management', () => {
    it('prevents memory leaks from accumulated messages', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 100 }));

      // Simulate many session switches with messages
      for (let sessionNum = 0; sessionNum < 5; sessionNum++) {
        const session = createTestSession(`session-${sessionNum}`);
        const prevSession = sessionNum > 0 ? createTestSession(`session-${sessionNum-1}`) : null;
        emitSessionEvent('chat-session-changed', { 
          currentChatSession: session,
          previousChatSession: prevSession
        });

        const messages = Array.from({ length: 50 }, (_, i) =>
          createMessage('user', `Session ${sessionNum} Message ${i}`)
        );

        emitSessionEvent('session-messages-loaded', {
          messages
        });
      }

      // Should only have messages from the last session
      expect(result.current.messages).toHaveLength(50);
      expect(result.current.messages[0]?.content).toContain('Session 4');
      
      // No messages from previous sessions
      expect(result.current.messages.some(m => m.content.includes('Session 0'))).toBe(false);
      expect(result.current.messages.some(m => m.content.includes('Session 1'))).toBe(false);
      expect(result.current.messages.some(m => m.content.includes('Session 2'))).toBe(false);
      expect(result.current.messages.some(m => m.content.includes('Session 3'))).toBe(false);
    });

    it('handles session switch with extremely large message history', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 100 }));

      // Create a very large message history
      const largeHistory = Array.from({ length: 1000 }, (_, i) =>
        createMessage(i % 2 === 0 ? 'user' : 'assistant', `Message ${i}`)
      );

      const session = createTestSession('large-session');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: session,
        previousChatSession: null
      });

      emitSessionEvent('session-messages-loaded', {
        messages: largeHistory
      });

      // Should be limited to maxMessages
      expect(result.current.messages).toHaveLength(100);
      expect(result.current.messages[0]?.content).toBe('Message 900');
      expect(result.current.messages[99]?.content).toBe('Message 999');

      // Switch to empty session
      const emptySession = createTestSession('empty');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: emptySession,
        previousChatSession: session
      });

      // Should clear immediately despite large previous history
      expect(result.current.messages).toHaveLength(0);

      emitSessionEvent('session-messages-loaded', {
        messages: []
      });

      // Should remain empty
      expect(result.current.messages).toHaveLength(0);
    });
  });
});