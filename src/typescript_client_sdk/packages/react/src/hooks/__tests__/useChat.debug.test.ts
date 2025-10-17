/**
 * Debug test for useChat race condition fix
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
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
    error: vi.fn(),
    warn: vi.fn()
  }
}));

describe('useChat - Debug Race Condition', () => {
  let mockClient: any;
  let mockSessionManager: any;
  let mockEnsureMessagesFormat: Mock;
  let mockLogger: any;
  let eventHandlers: Map<string, Function>;
  let sessionEventHandlers: Map<string, Function>;

  const emitClientEvent = (eventName: string, data?: any) => {
    const handler = eventHandlers.get(eventName);
    if (handler) {
      act(() => handler(data));
    }
  };

  const emitSessionEvent = (eventName: string, data?: any) => {
    const handler = sessionEventHandlers.get(eventName);
    if (handler) {
      act(() => handler(data));
    }
  };

  const createMessage = (role: 'user' | 'assistant', content: string): Message => ({
    role,
    content,
    timestamp: new Date().toISOString(),
    format: 'text'
  });

  beforeEach(async () => {
    eventHandlers = new Map();
    sessionEventHandlers = new Map();

    mockSessionManager = {
      getCurrentSession: vi.fn(() => null),
      on: vi.fn((event: string, handler: Function) => {
        sessionEventHandlers.set(event, handler);
        console.log(`[TEST] Registered session handler for: ${event}`);
      }),
      off: vi.fn()
    };

    mockClient = {
      getSessionManager: vi.fn(() => mockSessionManager),
      isConnected: vi.fn(() => true),
      sendText: vi.fn(),
      on: vi.fn((event: string, handler: Function) => {
        eventHandlers.set(event, handler);
        console.log(`[TEST] Registered client handler for: ${event}`);
      }),
      off: vi.fn()
    };

    mockEnsureMessagesFormat = vi.fn((messages: Message[]) => messages);
    mockLogger = {
      debug: vi.fn((...args: any[]) => console.log('[HOOK DEBUG]', ...args)),
      error: vi.fn(),
      warn: vi.fn()
    };

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

  it('Simple session switch test', async () => {
    const { result, waitForNextUpdate } = renderHook(() => useChat());
    
    // Initial state
    expect(result.current.messages).toHaveLength(0);
    
    // Create session without messages property
    const session: ChatSession = {
      session_id: 'test-session',
      context: {}
    } as ChatSession;
    
    // Trigger session change - this sets loading state asynchronously
    await act(async () => {
      emitSessionEvent('chat-session-changed', { currentChatSession: session, previousChatSession: null });
      // Give React time to process state updates
      await new Promise(resolve => setTimeout(resolve, 0));
    });
    
    // Messages should be cleared
    expect(result.current.messages).toHaveLength(0);
    
    const testMessages = [
      createMessage('user', 'Test message')
    ];
    
    // Load messages for session - emit in next tick to ensure loading state is set
    await act(async () => {
      // Use setTimeout to ensure this runs after loading state is set
      await new Promise(resolve => {
        setTimeout(() => {
          emitSessionEvent('session-messages-loaded', {
            messages: testMessages
          });
          resolve(undefined);
        }, 0);
      });
    });
    
    // Check final state
    console.log('[TEST] Final messages:', result.current.messages);
    console.log('[TEST] Final messages length:', result.current.messages.length);
    
    // This should pass if the race condition fix is working
    expect(result.current.messages).toHaveLength(1);
    expect(result.current.messages[0]?.content).toBe('Test message');
  });
  
  it('Session switch follows new event pattern', async () => {
    const { result } = renderHook(() => useChat());
    
    const sessionMessages = [
      createMessage('user', 'Session message 1'),
      createMessage('assistant', 'Session message 2')
    ];
    
    // Session WITHOUT messages property (new format)
    const session: ChatSession = {
      session_id: 'new-session',
      context: {}
    } as ChatSession;
    
    // NEW: Trigger session change - clears messages and sets loading
    await act(async () => {
      emitSessionEvent('chat-session-changed', { currentChatSession: session, previousChatSession: null });
      await new Promise(resolve => setTimeout(resolve, 0));
    });
    
    // Messages should be cleared during loading
    console.log('[TEST] Messages after session change:', result.current.messages);
    expect(result.current.messages).toHaveLength(0);
    expect(result.current.currentSessionId).toBe('new-session');
    
    // NEW: Emit session-messages-loaded to deliver messages
    await act(async () => {
      emitSessionEvent('session-messages-loaded', {
        sessionId: 'new-session',
        messages: sessionMessages
      });
    });
    
    // Now messages should be loaded
    console.log('[TEST] Messages after load:', result.current.messages);
    expect(result.current.messages).toHaveLength(2);
    expect(result.current.messages[0]?.content).toBe('Session message 1');
    expect(result.current.messages[1]?.content).toBe('Session message 2');
  });
});