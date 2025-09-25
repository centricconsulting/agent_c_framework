/**
 * Debug test for session-messages-loaded issue
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useChat } from './src/hooks/useChat';
import type { RealtimeClient, Message } from '@agentc/realtime-core';

// Enable debug logging
process.env.DEBUG = 'true';

// Mock dependencies
vi.mock('./src/providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

// Mock logger to capture output
const logOutput: string[] = [];
vi.mock('./src/utils/logger', () => ({
  Logger: {
    debug: vi.fn((...args: any[]) => {
      const msg = args.map((a: any) => typeof a === 'object' ? JSON.stringify(a) : String(a)).join(' ');
      logOutput.push(msg);
      console.log('[DEBUG]', msg);
    }),
    error: vi.fn(),
    warn: vi.fn()
  }
}));

describe('Debug session-messages-loaded', () => {
  let mockClient: any;
  let mockSessionManager: any;
  let clientHandlers: Record<string, Function> = {};
  let sessionHandlers: Record<string, Function> = {};

  beforeEach(() => {
    logOutput.length = 0;
    clientHandlers = {};
    sessionHandlers = {};
    
    mockSessionManager = {
      getCurrentSession: vi.fn(() => ({ id: 'test-session' })),
      on: vi.fn((event, handler) => {
        sessionHandlers[event] = handler;
      }),
      off: vi.fn()
    };

    mockClient = {
      getSessionManager: vi.fn(() => mockSessionManager),
      isConnected: vi.fn(() => true),
      sendText: vi.fn(),
      on: vi.fn((event, handler) => {
        clientHandlers[event] = handler;
      }),
      off: vi.fn()
    };

    const { useRealtimeClientSafe } = await import('./src/providers/AgentCContext');
    (useRealtimeClientSafe as any).mockReturnValue(mockClient);
  });

  it('shows session-messages-loaded debug flow', async () => {
    const { result } = renderHook(() => useChat());
    
    // Create test messages
    const messages: Message[] = [
      {
        id: 'msg-1',
        role: 'user',
        content: 'Test message',
        timestamp: new Date().toISOString()
      }
    ];
    
    console.log('\n=== TEST: Emitting session-messages-loaded directly ===');
    
    // Emit session-messages-loaded directly (without session change)
    if (sessionHandlers['session-messages-loaded']) {
      sessionHandlers['session-messages-loaded']({
        sessionId: 'test-session',
        messages
      });
    }
    
    console.log('\n=== Debug logs captured ===');
    logOutput.forEach(log => console.log(log));
    
    console.log('\n=== Result ===');
    console.log('Messages length:', result.current.messages.length);
    console.log('Messages:', result.current.messages);
    
    // This should fail because isLoadingSessionRef.current is false
    expect(result.current.messages).toHaveLength(0); // Should be 0 due to early return
  });
  
  it('shows session-messages-loaded with session change first', async () => {
    const { result } = renderHook(() => useChat());
    
    // Create test messages  
    const messages: Message[] = [
      {
        id: 'msg-1',
        role: 'user',
        content: 'Test message',
        timestamp: new Date().toISOString()
      }
    ];
    
    console.log('\n=== TEST: With session change first ===');
    
    // First trigger session change to set loading state
    if (clientHandlers['chat_session_changed']) {
      clientHandlers['chat_session_changed']({
        chat_session: {
          id: 'new-session',
          messages: [] // No inline messages
        }
      });
    }
    
    console.log('\n=== After session change, before messages loaded ===');
    logOutput.forEach(log => console.log(log));
    
    // Clear logs for clarity
    logOutput.length = 0;
    
    // Now emit session-messages-loaded
    if (sessionHandlers['session-messages-loaded']) {
      sessionHandlers['session-messages-loaded']({
        sessionId: 'new-session',
        messages
      });
    }
    
    console.log('\n=== After session-messages-loaded ===');
    logOutput.forEach(log => console.log(log));
    
    console.log('\n=== Result ===');
    console.log('Messages length:', result.current.messages.length);
    console.log('Messages:', result.current.messages);
    
    // This should work if loading state was set properly
    expect(result.current.messages).toHaveLength(1);
  });
});