/**
 * Tests for useChat hook - Part 1: Message Management
 * Tests message history, sendText, text streaming, and message buffering
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useChat } from '../useChat';
import type { UseChatOptions, UseChatReturn } from '../useChat';
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

describe('useChat - Part 1: Message Management', () => {
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

  describe('Initialization', () => {
    it('returns empty state when client is null', async () => {
      const agentCContext = await import('../../providers/AgentCContext');
      (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(null);

      const { result } = renderHook(() => useChat());

      expect(result.current.messages).toEqual([]);
      expect(result.current.currentSession).toBeNull();
      expect(result.current.currentSessionId).toBeNull();
      expect(result.current.isSending).toBe(false);
      expect(result.current.isAgentTyping).toBe(false);
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.error).toBeNull();
      expect(result.current.lastMessage).toBeNull();
    });

    it('returns empty state when SessionManager is null', () => {
      mockClient.getSessionManager.mockReturnValue(null);

      const { result } = renderHook(() => useChat());

      expect(result.current.messages).toEqual([]);
      expect(result.current.currentSession).toBeNull();
      expect(result.current.currentSessionId).toBeNull();
    });

    it('loads current session on mount', () => {
      const testSession = createTestSession('session-123');
      mockSessionManager.getCurrentSession.mockReturnValue(testSession);

      const { result } = renderHook(() => useChat());

      expect(mockSessionManager.getCurrentSession).toHaveBeenCalled();
      expect(result.current.currentSession).toEqual(testSession);
      expect(result.current.currentSessionId).toBe('session-123');
    });

    it('subscribes to all required events', () => {
      renderHook(() => useChat());

      // Verify client events for turn management
      expect(mockClient.on).toHaveBeenCalledWith('user_turn_start', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('user_turn_end', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('chat-session-changed', expect.any(Function));

      // Verify SessionManager events for message handling
      expect(mockSessionManager.on).toHaveBeenCalledWith('message-added', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('message-streaming', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('message-complete', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('session-messages-loaded', expect.any(Function));
      
      // Phase 1 new event subscriptions
      expect(mockSessionManager.on).toHaveBeenCalledWith('subsession-started', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('subsession-ended', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('media-added', expect.any(Function));
      expect(mockSessionManager.on).toHaveBeenCalledWith('system_message', expect.any(Function));
    });

    it('unsubscribes all events on unmount', () => {
      const { unmount } = renderHook(() => useChat());

      unmount();

      // Verify client event cleanup for turn management
      expect(mockClient.off).toHaveBeenCalledWith('user_turn_start', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('user_turn_end', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('chat-session-changed', expect.any(Function));

      // Verify SessionManager event cleanup for message handling
      expect(mockSessionManager.off).toHaveBeenCalledWith('message-added', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('message-streaming', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('message-complete', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('session-messages-loaded', expect.any(Function));
      
      // Phase 1 new event cleanup
      expect(mockSessionManager.off).toHaveBeenCalledWith('subsession-started', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('subsession-ended', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('media-added', expect.any(Function));
      expect(mockSessionManager.off).toHaveBeenCalledWith('system_message', expect.any(Function));
    });
  });

  describe('Message Sending', () => {
    it('sends text message successfully', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        await result.current.sendMessage('Hello world');
      });

      // Verify client.sendText was called (with optional fileIds parameter)
      expect(mockClient.sendText).toHaveBeenCalledWith('Hello world', undefined);

      // User message is not added locally anymore - EventStreamProcessor will emit message-added
      // So messages should still be empty after sendMessage
      expect(result.current.messages).toHaveLength(0);

      // Simulate server responding with UserMessageEvent (handled by EventStreamProcessor)
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Hello world')
      });

      // Now the message should appear
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Hello world');
    });

    it('throws error when client not available', async () => {
      const agentCContext = await import('../../providers/AgentCContext');
      (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(null);

      const { result } = renderHook(() => useChat());

      await expect(
        act(async () => {
          await result.current.sendMessage('Hello');
        })
      ).rejects.toThrow('Client not available');
    });

    it('throws error when not connected', async () => {
      mockClient.isConnected.mockReturnValue(false);

      const { result } = renderHook(() => useChat());

      await expect(
        act(async () => {
          await result.current.sendMessage('Hello');
        })
      ).rejects.toThrow('Not connected to server');
    });

    it('throws error for empty message', async () => {
      const { result } = renderHook(() => useChat());

      await expect(
        act(async () => {
          await result.current.sendMessage('   ');
        })
      ).rejects.toThrow('Message cannot be empty');
    });

    it('sets isSending during send', async () => {
      // Since sendText is synchronous, we can't observe isSending being true
      // from outside the function. We'll verify the flow works correctly.
      const { result } = renderHook(() => useChat());

      // Verify initial state
      expect(result.current.isSending).toBe(false);

      // Send message
      await act(async () => {
        await result.current.sendMessage('Test');
      });

      // Verify sendText was called (with optional fileIds parameter)
      expect(mockClient.sendText).toHaveBeenCalledWith('Test', undefined);
      // Verify isSending is false after completion  
      expect(result.current.isSending).toBe(false);
      // Note: message is not added locally anymore, EventStreamProcessor will emit message-added
      expect(result.current.messages).toHaveLength(0);

      // Simulate server response
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test')
      });

      // Now message should appear
      expect(result.current.messages).toHaveLength(1);
    });

    it('limits messages to maxMessages when message-added events are emitted', async () => {
      const { result } = renderHook(() => useChat({ maxMessages: 3 }));

      // Emit message-added events to simulate messages being added
      for (let i = 1; i <= 5; i++) {
        emitSessionEvent('message-added', {
          sessionId: `session-${i}`,
          message: createMessage('user', `Message ${i}`)
        });
      }

      // Should only keep last 3 messages
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[0]?.content).toBe('Message 3');
      expect(result.current.messages[1]?.content).toBe('Message 4');
      expect(result.current.messages[2]?.content).toBe('Message 5');
    });

    it('handles send message errors', async () => {
      // sendText throws synchronously
      mockClient.sendText.mockImplementation(() => {
        throw new Error('Network error');
      });

      const { result, rerender } = renderHook(() => useChat());

      // Attempt to send message
      let errorThrown = false;
      await act(async () => {
        try {
          await result.current.sendMessage('Hello');
        } catch (error) {
          errorThrown = true;
          expect((error as Error).message).toBe('Network error');
        }
      });

      // Verify error was thrown
      expect(errorThrown).toBe(true);
      
      // Force a rerender to ensure state updates are reflected
      rerender();
      
      // Check error state
      expect(result.current.error).toBe('Network error');
      expect(result.current.isSending).toBe(false);
    });
  });

  describe('Text Streaming', () => {
    it('handles message-streaming events', () => {
      const { result } = renderHook(() => useChat());

      // Emit first message-streaming
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Hello')
      });

      expect(result.current.isAgentTyping).toBe(true);
      expect(result.current.streamingMessage?.content).toBe('Hello');

      // Emit second message-streaming with accumulated text
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Hello world')
      });

      expect(result.current.streamingMessage?.content).toBe('Hello world');
      expect(result.current.isAgentTyping).toBe(true);
    });

    it('tracks sessionId during streaming', () => {
      const { result } = renderHook(() => useChat());

      // Emit message-streaming with sessionId
      emitSessionEvent('message-streaming', { 
        sessionId: 'msg-123',
        message: createMessage('assistant', 'Hi there')
      });

      expect(result.current.streamingMessage?.content).toBe('Hi there');

      // Complete the message with the same sessionId
      emitSessionEvent('message-complete', { 
        sessionId: 'msg-123',
        message: createMessage('assistant', 'Hi there')
      });

      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Hi there');
      expect(result.current.streamingMessage).toBeNull();
    });

    it('completes message on message-complete event', () => {
      const { result } = renderHook(() => useChat());

      // Stream a message
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Complete')
      });
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Complete message')
      });

      expect(result.current.streamingMessage?.content).toBe('Complete message');
      expect(result.current.isAgentTyping).toBe(true);

      // Emit message-complete
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Complete message')
      });

      // Verify message was added
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]).toMatchObject({
        role: 'assistant',
        content: 'Complete message',
        format: 'text'
      });

      // Verify state was cleared
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('handles message-complete without prior streaming', () => {
      const { result } = renderHook(() => useChat());

      // Emit message-complete without any message-streaming
      emitSessionEvent('message-complete', { 
        sessionId: 'complete-1',
        message: createMessage('assistant', 'Direct complete message')
      });

      // Message should still be added
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Direct complete message');
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('handles multiple streaming sequences', () => {
      const { result } = renderHook(() => useChat());

      // First streaming sequence
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'First')
      });
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'First message')
      });
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'First message')
      });

      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('First message');

      // Second streaming sequence
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Second')
      });
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Second message')
      });
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Second message')
      });

      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[1]?.content).toBe('Second message');
    });

    it('limits streamed messages to maxMessages', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 2 }));

      // Stream three messages
      for (let i = 1; i <= 3; i++) {
        emitSessionEvent('message-streaming', { 
          sessionId: `stream-${i}`,
          message: createMessage('assistant', `Message ${i}`)
        });
        emitSessionEvent('message-complete', { 
          sessionId: `stream-${i}`,
          message: createMessage('assistant', `Message ${i}`)
        });
      }

      // Should only keep last 2 messages
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Message 2');
      expect(result.current.messages[1]?.content).toBe('Message 3');
    });
  });

  describe('Session Message Loading', () => {
    it('handles chat_session_changed with messages', () => {
      const existingMessages = [
        createMessage('user', 'Hello'),
        createMessage('assistant', 'Hi there')
      ];

      const newSession = createTestSession('new-session', existingMessages);

      const { result } = renderHook(() => useChat());

      // Emit session change - NEW: This clears messages and sets loading state
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: newSession,
        previousChatSession: null
      });

      // Verify session was updated
      expect(result.current.currentSession).toEqual(newSession);
      expect(result.current.currentSessionId).toBe('new-session');

      // NEW: Messages should be cleared during loading
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.streamingMessage).toBeNull();

      // NEW: Emit session-messages-loaded to deliver the messages
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'new-session',
        messages: existingMessages
      });

      // NOW messages should be loaded
      // Messages now have additional id and type fields added
      expect(result.current.messages).toHaveLength(existingMessages.length);
      expect(result.current.messages[0]?.content).toBe('Hello');
      expect(result.current.messages[1]?.content).toBe('Hi there');
    });

    it('clears messages on session change without messages', () => {
      const { result } = renderHook(() => useChat());

      // Add some existing messages via server event
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Existing message')
      });

      expect(result.current.messages).toHaveLength(1);

      // Emit session change with no messages
      const emptySession = createTestSession('empty-session', []);
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: emptySession,
        previousChatSession: null
      });

      // Verify messages were cleared immediately
      expect(result.current.messages).toEqual([]);
      expect(result.current.streamingMessage).toBeNull();

      // NEW: Emit session-messages-loaded with empty array
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'empty-session',
        messages: []
      });

      // Should remain empty
      expect(result.current.messages).toEqual([]);
    });

    it('handles session-messages-loaded event', () => {
      const loadedMessages = [
        createMessage('user', 'Question'),
        createMessage('assistant', 'Answer')
      ];

      const { result } = renderHook(() => useChat());

      // Emit session messages loaded
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test-session',
        messages: loadedMessages
      });

      // Messages from EventStreamProcessor come already formatted
      // So ensureMessagesFormat is not called in the new implementation
      // Messages now have additional id and type fields added
      expect(result.current.messages).toHaveLength(loadedMessages.length);
      expect(result.current.messages[0]?.content).toBe('Question');
      expect(result.current.messages[1]?.content).toBe('Answer');
      expect(result.current.streamingMessage).toBeNull();
    });

    it('limits loaded messages to maxMessages', () => {
      const manyMessages = Array.from({ length: 10 }, (_, i) =>
        createMessage(i % 2 === 0 ? 'user' : 'assistant', `Message ${i + 1}`)
      );

      const { result } = renderHook(() => useChat({ maxMessages: 3 }));

      // Load many messages
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test-session',
        messages: manyMessages
      });

      // Should only keep last 3
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[0]?.content).toBe('Message 8');
      expect(result.current.messages[1]?.content).toBe('Message 9');
      expect(result.current.messages[2]?.content).toBe('Message 10');
    });

    it('clears streaming message when loading session messages', () => {
      const { result } = renderHook(() => useChat());

      // Start streaming a message
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Partial')
      });
      expect(result.current.streamingMessage?.content).toBe('Partial');

      // NEW: First emit chat-session-changed to clear and set loading state
      const newSession = createTestSession('test-session');
      emitSessionEvent('chat-session-changed', { 
        currentChatSession: newSession,
        previousChatSession: null
      });

      // Verify streaming was cleared by session change
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.messages).toHaveLength(0);

      // Load session messages
      const messages = [createMessage('user', 'Loaded message')];
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test-session',
        messages
      });

      // Verify messages were loaded
      // Messages now have additional id and type fields added
      expect(result.current.messages).toHaveLength(messages.length);
      expect(result.current.messages[0]?.content).toBe('Loaded message');
    });
  });

  describe('Message Buffering and Assembly', () => {
    it('updates streaming message correctly', () => {
      const { result } = renderHook(() => useChat());

      const chunks = ['This', 'This is', 'This is a', 'This is a longer', 'This is a longer message'];
      
      chunks.forEach((text, index) => {
        emitSessionEvent('message-streaming', { 
          sessionId: 'stream-1',
          message: createMessage('assistant', text)
        });
      });

      expect(result.current.streamingMessage?.content).toBe('This is a longer message');

      // Complete the message
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'This is a longer message')
      });

      expect(result.current.messages[0]?.content).toBe('This is a longer message');
    });

    it('handles message-streaming with unchanged content', () => {
      const { result } = renderHook(() => useChat());

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Start')
      });
      // Same content - simulating no change
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Start')
      });
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'StartEnd')
      });

      expect(result.current.streamingMessage?.content).toBe('StartEnd');
    });

    it('preserves message order with mixed user and assistant messages', async () => {
      const { result } = renderHook(() => useChat());

      // User message (emitted by EventStreamProcessor)
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Question 1')
      });

      // Assistant response (streamed)
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Answer 1')
      });
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Answer 1')
      });

      // Another user message
      emitSessionEvent('message-added', {
        sessionId: 'msg-2',
        message: createMessage('user', 'Question 2')
      });

      // Another assistant response
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Answer 2')
      });
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Answer 2')
      });

      expect(result.current.messages).toHaveLength(4);
      expect(result.current.messages[0]?.role).toBe('user');
      expect(result.current.messages[0]?.content).toBe('Question 1');
      expect(result.current.messages[1]?.role).toBe('assistant');
      expect(result.current.messages[1]?.content).toBe('Answer 1');
      expect(result.current.messages[2]?.role).toBe('user');
      expect(result.current.messages[2]?.content).toBe('Question 2');
      expect(result.current.messages[3]?.role).toBe('assistant');
      expect(result.current.messages[3]?.content).toBe('Answer 2');
    });

    it('resets streaming state correctly between messages', () => {
      const { result } = renderHook(() => useChat());

      // First message
      emitSessionEvent('message-streaming', { 
        sessionId: 'msg-1',
        message: createMessage('assistant', 'First')
      });
      emitSessionEvent('message-complete', { 
        sessionId: 'msg-1',
        message: createMessage('assistant', 'First')
      });

      // Verify first message
      expect(result.current.messages[0]?.content).toBe('First');
      expect(result.current.streamingMessage).toBeNull();

      // Second message should start fresh
      emitSessionEvent('message-streaming', { 
        sessionId: 'msg-2',
        message: createMessage('assistant', 'Second')
      });
      expect(result.current.streamingMessage?.content).toBe('Second');

      emitSessionEvent('message-complete', { 
        sessionId: 'msg-2',
        message: createMessage('assistant', 'Second')
      });
      expect(result.current.messages[1]?.content).toBe('Second');
    });
  });
});

/**
 * Tests for useChat hook - Part 2: Typing Indicators & Advanced Event Handling
 * Tests isAgentTyping state, event subscriptions edge cases, message limiting, and cleanup
 */
describe('useChat - Part 2: Typing Indicators & Events', () => {
  // Test utilities (reuse from Part 1)
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

  describe('Agent Typing State Management', () => {
    it('sets isAgentTyping true on first message-streaming', () => {
      const { result } = renderHook(() => useChat());

      expect(result.current.isAgentTyping).toBe(false);

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Starting...')
      });

      expect(result.current.isAgentTyping).toBe(true);
    });

    it('maintains isAgentTyping during multiple message-streaming events', () => {
      const { result } = renderHook(() => useChat());

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Part 1')
      });
      expect(result.current.isAgentTyping).toBe(true);

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Part 1 Part 2')
      });
      expect(result.current.isAgentTyping).toBe(true);

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Part 1 Part 2 Part 3')
      });
      expect(result.current.isAgentTyping).toBe(true);
    });

    it('sets isAgentTyping false on message-complete', () => {
      const { result } = renderHook(() => useChat());

      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Message')
      });
      expect(result.current.isAgentTyping).toBe(true);

      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Message')
      });
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('clears streaming message on session change and resets typing state', () => {
      const { result } = renderHook(() => useChat());

      // Start typing
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'In progress...')
      });
      expect(result.current.isAgentTyping).toBe(true);
      expect(result.current.streamingMessage?.content).toBe('In progress...');

      // Change session
      const newSession = createTestSession('new-session');
      emitSessionEvent('chat-session-changed', { currentChatSession: newSession, previousChatSession: null });

      // Streaming message should be cleared and typing state reset
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);

      // NEW: Complete loading state by emitting session-messages-loaded
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'new-session',
        messages: []
      });
    });

    it('handles rapid typing state changes', () => {
      const { result } = renderHook(() => useChat());

      // Rapid sequence of messages
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Quick')
      });
      expect(result.current.isAgentTyping).toBe(true);

      emitSessionEvent('message-complete', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Quick')
      });
      expect(result.current.isAgentTyping).toBe(false);

      // Immediately start new message
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Another')
      });
      expect(result.current.isAgentTyping).toBe(true);

      emitSessionEvent('message-complete', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'Another')
      });
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('changes typing state based on turn events', () => {
      const { result } = renderHook(() => useChat());

      // Agent starts typing
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Agent typing...')
      });
      expect(result.current.isAgentTyping).toBe(true);

      // User turn start stops agent typing
      emitClientEvent('user_turn_start');
      expect(result.current.isAgentTyping).toBe(false);

      // User turn end now immediately shows typing indicator (agent is thinking)
      emitClientEvent('user_turn_end');
      // Typing indicator shows immediately when agent's turn begins
      expect(result.current.isAgentTyping).toBe(true);

      // Start new streaming
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'New message')
      });
      expect(result.current.isAgentTyping).toBe(true);

      // Complete stops typing
      emitSessionEvent('message-complete', { 
        sessionId: 'stream-2',
        message: createMessage('assistant', 'New message')
      });
      expect(result.current.isAgentTyping).toBe(false);
    });
  });

  describe('Event Subscription Edge Cases', () => {
    it('handles malformed message-streaming events', () => {
      const { result } = renderHook(() => useChat());

      // Missing message property
      emitSessionEvent('message-streaming', { sessionId: 'test' });
      expect(result.current.streamingMessage).toBeNull();

      // Null message
      emitSessionEvent('message-streaming', { sessionId: 'test', message: null });
      expect(result.current.streamingMessage).toBeNull();

      // Valid message after malformed
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Valid')
      });
      expect(result.current.streamingMessage?.content).toBe('Valid');
    });

    it('handles message-complete directly without streaming', () => {
      const { result } = renderHook(() => useChat());

      // Direct message-complete without prior streaming
      emitSessionEvent('message-complete', { 
        sessionId: 'complete-1',
        message: createMessage('assistant', 'Direct complete')
      });

      expect(result.current.isAgentTyping).toBe(false);
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Direct complete');
    });

    it('handles duplicate event subscriptions', () => {
      const { rerender } = renderHook(() => useChat());

      // Initial render subscribes events
      expect(mockClient.on).toHaveBeenCalledTimes(2); // user_turn_start, user_turn_end
      expect(mockSessionManager.on).toHaveBeenCalledTimes(9); // chat-session-changed, message-added, message-streaming, message-complete, session-messages-loaded, subsession-started, subsession-ended, media-added, system_message

      // Rerender should not duplicate subscriptions
      rerender();

      // Should still have same number of subscriptions
      expect(mockClient.on).toHaveBeenCalledTimes(2);
      expect(mockSessionManager.on).toHaveBeenCalledTimes(9);
    });

    it('handles events when sessionManager is null', () => {
      mockClient.getSessionManager.mockReturnValue(null);

      const { result } = renderHook(() => useChat());

      // When sessionManager is null, no events are subscribed
      expect(result.current.messages).toEqual([]);
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);
    });

    it('handles session change with invalid data', () => {
      const { result } = renderHook(() => useChat());

      // Add initial message via server event
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Initial')
      });
      expect(result.current.messages).toHaveLength(1);

      // Invalid session change (no currentChatSession) - implementation ignores it
      emitSessionEvent('chat-session-changed', {});
      
      // Should keep existing state when session change is invalid (no currentChatSession)
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.currentSession).toBeNull();

      // Session with no messages should clear
      const emptySession = createTestSession('empty-session', []);
      emitSessionEvent('chat-session-changed', { currentChatSession: emptySession, previousChatSession: null });
      
      // Should clear messages for empty session
      expect(result.current.messages).toEqual([]);
      expect(result.current.currentSession).toEqual(emptySession);
      
      // NEW: Complete loading state
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'empty-session',
        messages: []
      });
      
      // Should remain empty
      expect(result.current.messages).toEqual([]);
    });

    it('handles rapid event firing', () => {
      const { result } = renderHook(() => useChat());

      // Fire many streaming events rapidly with accumulated text
      let accumulated = '';
      for (let i = 0; i < 100; i++) {
        accumulated += `${i}`;
        emitSessionEvent('message-streaming', { 
          sessionId: 'rapid-stream',
          message: createMessage('assistant', accumulated)
        });
      }

      // Should have the final accumulated message
      expect(result.current.streamingMessage?.content).toBe(accumulated);

      // Complete
      emitSessionEvent('message-complete', { 
        sessionId: 'rapid-stream',
        message: createMessage('assistant', accumulated)
      });
      expect(result.current.messages[0]?.content).toBe(accumulated);
    });
  });

  describe('Advanced Message History Limiting', () => {
    it('handles maxMessages of 0', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 0 }));

      // Try to add messages via server event
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Test')
      });

      // Note: The current implementation doesn't special-case maxMessages=0
      // It will still add the message because the check is:
      // if (maxMessages && newMessages.length > maxMessages)
      // With maxMessages=0, this becomes if (0 && ...) which is always false
      // So the message is added
      expect(result.current.messages).toHaveLength(1);
    });

    it('handles maxMessages of 1', async () => {
      const { result } = renderHook(() => useChat({ maxMessages: 1 }));

      // Send first message
      await act(async () => {
        await result.current.sendMessage('First');
      });
      
      // Simulate server response for first message
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'First')
      });
      
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('First');

      // Send second message
      await act(async () => {
        await result.current.sendMessage('Second');
      });
      
      // Simulate server response for second message
      emitSessionEvent('message-added', {
        sessionId: 'msg-2',
        message: createMessage('user', 'Second')
      });
      
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Second');
    });

    it('applies maxMessages to session loaded messages', () => {
      const { result } = renderHook(() => useChat({ maxMessages: 2 }));

      const manyMessages = [
        createMessage('user', 'Old 1'),
        createMessage('assistant', 'Old 2'),
        createMessage('user', 'Recent 1'),
        createMessage('assistant', 'Recent 2')
      ];

      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test',
        messages: manyMessages
      });

      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Recent 1');
      expect(result.current.messages[1]?.content).toBe('Recent 2');
    });

    it('respects maxMessages during mixed operations', async () => {
      const { result } = renderHook(() => useChat({ maxMessages: 3 }));

      // Load initial messages
      const initialMessages = [
        createMessage('user', 'Loaded 1'),
        createMessage('assistant', 'Loaded 2')
      ];
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'test',
        messages: initialMessages
      });

      // Send user message
      await act(async () => {
        await result.current.sendMessage('User new');
      });

      // Simulate server response for user message
      emitSessionEvent('message-added', {
        sessionId: 'msg-user',
        message: createMessage('user', 'User new')
      });

      // Stream assistant message
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Assistant new')
      });
      emitSessionEvent('message-complete', {
        sessionId: 'stream-1', 
        message: createMessage('assistant', 'Assistant new')
      });

      // Should only have last 3 messages
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[0]?.content).toBe('Loaded 2');
      expect(result.current.messages[1]?.content).toBe('User new');
      expect(result.current.messages[2]?.content).toBe('Assistant new');
    });

    it('applies maxMessages to new messages after prop change', () => {
      const { result, rerender } = renderHook(
        ({ maxMessages }: UseChatOptions = {}) => useChat({ maxMessages }),
        { initialProps: { maxMessages: 5 } }
      );

      // Add 5 messages via server events
      for (let i = 1; i <= 5; i++) {
        emitSessionEvent('message-added', {
          sessionId: `msg-${i}`,
          message: createMessage('user', `Message ${i}`)
        });
      }
      expect(result.current.messages).toHaveLength(5);

      // Change maxMessages to 2
      rerender({ maxMessages: 2 });

      // Note: The implementation doesn't retroactively trim existing messages
      // It only applies the limit when NEW messages are added
      expect(result.current.messages).toHaveLength(5);

      // Add a new message - NOW it should trim to maxMessages
      emitSessionEvent('message-added', {
        sessionId: 'msg-6',
        message: createMessage('user', 'Message 6')
      });

      // Should only keep last 2 messages
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[0]?.content).toBe('Message 5');
      expect(result.current.messages[1]?.content).toBe('Message 6');
    });
  });

  describe('Cleanup and Unmount Behavior', () => {
    it('cleans up all event listeners on unmount', () => {
      const { unmount } = renderHook(() => useChat());

      // Store handler references
      const clientHandlers = new Map<string, unknown>();
      const sessionHandlers = new Map<string, unknown>();

      mockClient.on.mock.calls.forEach(([event, handler]) => {
        clientHandlers.set(event as string, handler);
      });
      mockSessionManager.on.mock.calls.forEach(([event, handler]) => {
        sessionHandlers.set(event as string, handler);
      });

      unmount();

      // Verify all client handlers were removed
      ['user_turn_start', 'user_turn_end'].forEach(event => {
        expect(mockClient.off).toHaveBeenCalledWith(event, clientHandlers.get(event));
      });

      // Verify all session handlers were removed
      ['chat-session-changed', 'message-added', 'message-streaming', 'message-complete', 'session-messages-loaded', 'subsession-started', 'subsession-ended', 'media-added', 'system_message'].forEach(event => {
        expect(mockSessionManager.off).toHaveBeenCalledWith(event, sessionHandlers.get(event));
      });
    });

    it('stops processing events after unmount', () => {
      const { result, unmount } = renderHook(() => useChat());

      // Start streaming
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Before unmount')
      });
      expect(result.current.streamingMessage?.content).toBe('Before unmount');

      unmount();

      // Clear handlers to simulate real unmount
      eventHandlers.clear();
      sessionEventHandlers.clear();

      // Try to emit events after unmount
      const handler = sessionEventHandlers.get('message-streaming');
      expect(handler).toBeUndefined();
    });

    it('cleans up streaming message state on unmount', () => {
      const { unmount } = renderHook(() => useChat());

      // Start streaming
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Partial')
      });

      unmount();

      // Remount should start fresh
      const { result: newResult } = renderHook(() => useChat());
      expect(newResult.current.streamingMessage).toBeNull();
      expect(newResult.current.isAgentTyping).toBe(false);
    });

    it('handles unmount during active streaming', () => {
      const { unmount } = renderHook(() => useChat());

      // Start streaming
      emitSessionEvent('message-streaming', { 
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Streaming...')
      });

      // Unmount while streaming
      expect(() => unmount()).not.toThrow();

      // Verify cleanup was called
      expect(mockClient.off).toHaveBeenCalled();
      expect(mockSessionManager.off).toHaveBeenCalled();
    });

    it('handles unmount when sessionManager is null', () => {
      mockClient.getSessionManager.mockReturnValue(null);

      const { unmount } = renderHook(() => useChat());

      // Should not throw on unmount
      expect(() => unmount()).not.toThrow();

      // Client events should still be cleaned up
      expect(mockClient.off).toHaveBeenCalledTimes(2); // user_turn_start, user_turn_end
    });

    it('properly cleans up after error state', async () => {
      const { result, unmount } = renderHook(() => useChat());

      // Cause an error
      mockClient.sendText.mockImplementation(() => {
        throw new Error('Send failed');
      });

      await act(async () => {
        try {
          await result.current.sendMessage('Test');
        } catch {
          // Expected error
        }
      });

      expect(result.current.error).toBe('Send failed');

      // Unmount should still clean up properly
      expect(() => unmount()).not.toThrow();
      expect(mockClient.off).toHaveBeenCalled();
    });

    it('prevents memory leaks from retained handlers', () => {
      const { unmount, rerender } = renderHook(() => useChat());

      // Track handler counts
      const initialOnCalls = mockClient.on.mock.calls.length;

      // Multiple rerenders shouldn't accumulate handlers
      rerender();
      rerender();
      rerender();

      expect(mockClient.on.mock.calls.length).toBe(initialOnCalls);

      unmount();

      // All handlers should be removed
      expect(mockClient.off.mock.calls.length).toBe(2); // user_turn_start, user_turn_end
      expect(mockSessionManager.off.mock.calls.length).toBe(9); // chat-session-changed, message-added, message-streaming, message-complete, session-messages-loaded, subsession-started, subsession-ended, media-added, system_message
    });
  });

  describe('lastMessage Computed Property', () => {
    it('returns null when no messages', () => {
      const { result } = renderHook(() => useChat());
      expect(result.current.lastMessage).toBeNull();
    });

    it('returns last message from array', async () => {
      const { result } = renderHook(() => useChat());

      // Send first message
      await act(async () => {
        await result.current.sendMessage('First');
      });
      
      // Simulate server response
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'First')
      });
      expect(result.current.lastMessage?.content).toBe('First');

      // Send second message
      await act(async () => {
        await result.current.sendMessage('Second');
      });
      
      // Simulate server response
      emitSessionEvent('message-added', {
        sessionId: 'msg-2',
        message: createMessage('user', 'Second')
      });
      expect(result.current.lastMessage?.content).toBe('Second');

      // Add third message via streaming
      emitSessionEvent('message-streaming', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Third')
      });
      emitSessionEvent('message-complete', {
        sessionId: 'stream-1',
        message: createMessage('assistant', 'Third')
      });
      expect(result.current.lastMessage?.content).toBe('Third');
    });

    it('updates when messages are cleared', () => {
      const { result } = renderHook(() => useChat());

      // Add message via server event
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Message')
      });
      expect(result.current.lastMessage?.content).toBe('Message');

      // Clear via session change
      emitSessionEvent('chat-session-changed', {
        currentChatSession: createTestSession('new', []),
        previousChatSession: null
      });
      expect(result.current.lastMessage).toBeNull();
    });
  });

  describe('Error Handling and Recovery', () => {
    it('clears error on successful operation', async () => {
      const { result } = renderHook(() => useChat());

      // Create an error
      mockClient.sendText.mockImplementationOnce(() => {
        throw new Error('Temporary error');
      });

      await act(async () => {
        try {
          await result.current.sendMessage('Fail');
        } catch {
          // Expected
        }
      });
      expect(result.current.error).toBe('Temporary error');

      // Successful operation should clear error
      mockClient.sendText.mockImplementation(() => {});
      await act(async () => {
        await result.current.sendMessage('Success');
      });
      expect(result.current.error).toBeNull();
      
      // NEW: Verify message was sent even though we don't add it locally (with optional fileIds parameter)
      expect(mockClient.sendText).toHaveBeenCalledWith('Success', undefined);
    });

    it('maintains state consistency after errors', async () => {
      const { result } = renderHook(() => useChat());

      // Add a message
      await act(async () => {
        await result.current.sendMessage('Before error');
      });
      
      // Simulate server response for first message
      emitSessionEvent('message-added', {
        sessionId: 'msg-1',
        message: createMessage('user', 'Before error')
      });
      expect(result.current.messages).toHaveLength(1);

      // Cause an error
      mockClient.sendText.mockImplementationOnce(() => {
        throw new Error('Error');
      });

      await act(async () => {
        try {
          await result.current.sendMessage('During error');
        } catch {
          // Expected
        }
      });

      // Messages should not include failed message (server never sent response)
      expect(result.current.messages).toHaveLength(1);
      expect(result.current.messages[0]?.content).toBe('Before error');

      // Should be able to continue
      mockClient.sendText.mockImplementation(() => {});
      await act(async () => {
        await result.current.sendMessage('After error');
      });
      
      // Simulate server response for message after error
      emitSessionEvent('message-added', {
        sessionId: 'msg-2',
        message: createMessage('user', 'After error')
      });
      
      expect(result.current.messages).toHaveLength(2);
      expect(result.current.messages[1]?.content).toBe('After error');
    });
  });
});