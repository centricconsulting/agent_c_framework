/**
 * Test suite for EventStreamProcessor - session-messages-loaded event verification
 * 
 * This test verifies that resumed messages emit a single session-messages-loaded event,
 * not individual message-added events, which is critical for the React loading state.
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { SessionManager } from '../../session/SessionManager';
import { ChatSession } from '../types/CommonTypes';
import { MessageParam } from '../../types/message-params';

describe('EventStreamProcessor - Session Messages Loaded Event', () => {
  let processor: EventStreamProcessor;
  let sessionManager: SessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  const testSessionId = 'test-session-loaded';

  beforeEach(() => {
    vi.clearAllMocks();
    
    const mockSession: ChatSession = {
      session_id: testSessionId,
      session_name: 'Test Session Loading',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      metadata: {}
    };

    sessionManager = new SessionManager();
    processor = new EventStreamProcessor(sessionManager);
    sessionManagerEmitSpy = vi.spyOn(sessionManager, 'emit');
    sessionManager.setCurrentSession(mockSession);
  });

  it('should emit single session-messages-loaded event, NOT individual message-added events', () => {
    const messages: MessageParam[] = [
      {
        role: 'user',
        content: 'Hello'
      },
      {
        role: 'assistant',
        content: 'Hi there!'
      },
      {
        role: 'user',
        content: 'How are you?'
      },
      {
        role: 'assistant',
        content: 'I am doing well, thank you!'
      }
    ];

    processor.processEvent({
      type: 'chat_session_changed',
      chat_session: {
        session_id: testSessionId,
        messages
      }
    } as any);

    // Verify NO individual message-added events were emitted
    const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'message-added'
    );
    expect(messageAddedCalls).toHaveLength(0);

    // Verify exactly ONE session-messages-loaded event was emitted
    const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'session-messages-loaded'
    );
    expect(sessionLoadedCalls).toHaveLength(1);

    // Verify the event has all messages
    const loadedEvent = sessionLoadedCalls[0][1];
    expect(loadedEvent.sessionId).toBe(testSessionId);
    expect(loadedEvent.messages).toHaveLength(4);
    expect(loadedEvent.messages[0].content).toBe('Hello');
    expect(loadedEvent.messages[1].content).toBe('Hi there!');
    expect(loadedEvent.messages[2].content).toBe('How are you?');
    expect(loadedEvent.messages[3].content).toBe('I am doing well, thank you!');
  });

  it('should handle empty message array correctly', () => {
    processor.processEvent({
      type: 'chat_session_changed',
      chat_session: {
        session_id: testSessionId,
        messages: []
      }
    } as any);

    const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'session-messages-loaded'
    );
    
    expect(sessionLoadedCalls).toHaveLength(1);
    expect(sessionLoadedCalls[0][1]).toEqual({
      sessionId: testSessionId,
      messages: []
    });
  });

  it('should handle complex messages with tools correctly', () => {
    const messages: MessageParam[] = [
      {
        role: 'assistant',
        content: [
          {
            type: 'text',
            text: 'Let me think about that.'
          },
          {
            type: 'tool_use',
            id: 'tool_1',
            name: 'think',
            input: { thought: 'Processing request...' }
          }
        ]
      },
      {
        role: 'user',
        content: [
          {
            type: 'tool_result',
            tool_use_id: 'tool_1',
            content: 'null'
          }
        ]
      }
    ];

    processor.processEvent({
      type: 'chat_session_changed',
      chat_session: {
        session_id: testSessionId,
        messages
      }
    } as any);

    // Still NO individual message-added events
    const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'message-added'
    );
    expect(messageAddedCalls).toHaveLength(0);

    // Only ONE session-messages-loaded event
    const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'session-messages-loaded'
    );
    expect(sessionLoadedCalls).toHaveLength(1);

    // Verify messages were processed correctly
    const loadedEvent = sessionLoadedCalls[0][1];
    expect(loadedEvent.messages).toHaveLength(2);
    // First message is the thought (processed from tool_use)
    expect(loadedEvent.messages[0].role).toBe('assistant (thought)');
    expect(loadedEvent.messages[0].content).toBe('Processing request...');
    // Second message is the text content
    expect(loadedEvent.messages[1].content).toBe('Let me think about that.');
  });

  it('should include sessionId in the event', () => {
    const customSessionId = 'custom-session-123';
    
    processor.processEvent({
      type: 'chat_session_changed',
      chat_session: {
        session_id: customSessionId,
        messages: [
          {
            role: 'user',
            content: 'Test message'
          }
        ]
      }
    } as any);

    const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
      call => call[0] === 'session-messages-loaded'
    );

    expect(sessionLoadedCalls).toHaveLength(1);
    expect(sessionLoadedCalls[0][1].sessionId).toBe(customSessionId);
  });
});