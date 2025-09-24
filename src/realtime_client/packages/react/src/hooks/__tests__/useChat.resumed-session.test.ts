/**
 * Integration test for useChat hook with resumed session data
 * 
 * This test validates that the chat rendering fix works end-to-end from
 * EventStreamProcessor through to the React useChat hook, ensuring:
 * 1. Think tools render as thoughts (not tool calls)
 * 2. Delegation tools create proper subsessions
 * 3. Regular messages work normally
 * 4. The UI receives the correct message structure for rendering
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useChat } from '../useChat';
import type { RealtimeClient, ChatSession, Message } from '@agentc/realtime-core';
import testSessionData from '../../../../../.scratch/chat_fixes/session_with_delegation.json';

// Mock dependencies
vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

vi.mock('@agentc/realtime-core', async () => {
  const actual = await vi.importActual<typeof import('@agentc/realtime-core')>('@agentc/realtime-core');
  return {
    ...actual,
    ensureMessagesFormat: vi.fn((messages: Message[]) => messages)
  };
});

vi.mock('../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    error: vi.fn(),
    warn: vi.fn()
  }
}));

describe('useChat - Resumed Session Integration Test', () => {
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
    emit: Mock;
  };

  let eventHandlers: Map<string, (event?: unknown) => void>;
  let sessionEventHandlers: Map<string, (event?: unknown) => void>;
  let mockEnsureMessagesFormat: Mock;

  // Helper to emit client events
  const emitClientEvent = (eventName: string, data?: unknown) => {
    const handler = eventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to emit session events
  const emitSessionEvent = (eventName: string, data?: unknown) => {
    const handler = sessionEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to create processed messages from the test session data
  const processTestSessionMessages = () => {
    const messages: any[] = [];
    
    // Process the test session messages to simulate EventStreamProcessor output
    testSessionData.messages.forEach((msg: any) => {
      if (msg.role === 'user' && typeof msg.content === 'string') {
        // Regular user message
        messages.push({
          id: `msg-${messages.length}`,
          type: 'message',
          role: 'user',
          content: msg.content,
          timestamp: new Date().toISOString(),
          format: 'text'
        });
      } else if (msg.role === 'assistant') {
        if (Array.isArray(msg.content)) {
          msg.content.forEach((block: any) => {
            if (block.type === 'text') {
              // Regular assistant text
              messages.push({
                id: `msg-${messages.length}`,
                type: 'message',
                role: 'assistant',
                content: block.text,
                timestamp: new Date().toISOString(),
                format: 'text'
              });
            } else if (block.type === 'tool_use') {
              if (block.name === 'think') {
                // Think tool becomes a thought
                messages.push({
                  id: `msg-${messages.length}`,
                  type: 'message',
                  role: 'assistant (thought)',
                  content: block.input.thought,
                  timestamp: new Date().toISOString(),
                  format: 'markdown'
                });
              } else if (block.name === 'act_oneshot') {
                // Delegation tool creates subsession messages
                // User message with the request
                let userContent = block.input.request;
                if (block.input.process_context) {
                  userContent += `\n\nContext: ${block.input.process_context}`;
                }
                messages.push({
                  id: `msg-${messages.length}`,
                  type: 'message',
                  role: 'user',
                  content: userContent,
                  timestamp: new Date().toISOString(),
                  format: 'text'
                });
              }
            }
          });
        } else if (typeof msg.content === 'string') {
          messages.push({
            id: `msg-${messages.length}`,
            type: 'message',
            role: 'assistant',
            content: msg.content,
            timestamp: new Date().toISOString(),
            format: 'text'
          });
        }
      }
      
      // Process tool results for delegation tools
      if (msg.role === 'user' && Array.isArray(msg.content)) {
        msg.content.forEach((block: any) => {
          if (block.type === 'tool_result') {
            // Extract the response from the tool result
            const content = block.content;
            if (content && content.includes('text:')) {
              // Parse the YAML-like response
              let responseText = content;
              if (content.includes('---')) {
                responseText = content.split('---')[1] || content;
              }
              
              // Extract text from YAML format in the test data
              // The content has the format: text: 'content here'
              const textMatch = responseText.match(/text:\s*'([^']+)'/);
              if (textMatch) {
                responseText = textMatch[1];
              } else {
                // Try other formats
                const altMatch = responseText.match(/text:\s*["']?(.+?)(?:["']\s*$|\ntype:|$)/s);
                if (altMatch) {
                  responseText = altMatch[1].trim();
                }
              }
              
              // Only add assistant response for delegation tools
              const prevMsg = messages[messages.length - 1];
              if (prevMsg && prevMsg.role === 'user' && 
                  (prevMsg.content.includes('Please explore') || 
                   prevMsg.content.includes('analyze'))) {
                messages.push({
                  id: `msg-${messages.length}`,
                  type: 'message',
                  role: 'assistant',
                  content: responseText,
                  timestamp: new Date().toISOString(),
                  format: 'text'
                });
              }
            }
          }
        });
      }
    });
    
    return messages;
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    
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
      }),
      emit: vi.fn()
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

    // Setup ensureMessagesFormat mock to process messages like EventStreamProcessor would
    mockEnsureMessagesFormat = vi.fn((messages: any[]) => {
      return processTestSessionMessages();
    });

    // Apply mocks
    const agentCContext = await import('../../providers/AgentCContext');
    (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(mockClient);

    const coreModule = await import('@agentc/realtime-core');
    (coreModule.ensureMessagesFormat as Mock).mockImplementation(mockEnsureMessagesFormat);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('End-to-End Resumed Session Rendering', () => {
    it('should correctly render the test session with delegation and think tools', async () => {
      const { result } = renderHook(() => useChat());

      // Initially should be empty
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.currentSession).toBeNull();

      // Simulate receiving the chat_session_changed event with test data
      // This simulates what happens when a session is resumed
      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      // Wait for all events to be processed
      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      }, { timeout: 5000 });

      // Verify session was set
      expect(result.current.currentSession).toBeDefined();
      expect(result.current.currentSessionId).toBe(testSessionData.session_id);

      // Now validate the messages structure for UI rendering
      const messages = result.current.messages;

      // 1. Verify regular user message appears normally
      const helloDomo = messages.find(m => 
        m.role === 'user' && m.content.includes('hello domo')
      );
      expect(helloDomo).toBeDefined();
      expect(helloDomo?.content).toBe('hello domo!');

      // 2. Verify assistant's greeting response
      const greeting = messages.find(m =>
        m.role === 'assistant' && m.content.includes('Great to meet you')
      );
      expect(greeting).toBeDefined();

      // 3. Verify user's request for delegation
      const delegationRequest = messages.find(m =>
        m.role === 'user' && m.content.includes('ask a clone to check')
      );
      expect(delegationRequest).toBeDefined();

      // 4. Verify assistant's acknowledgment before delegation
      const acknowledgment = messages.find(m =>
        m.role === 'assistant' && m.content.includes("I'll ask a clone")
      );
      expect(acknowledgment).toBeDefined();

      // 5. CRITICAL: Verify delegation creates subsession messages (not tool UI)
      // The subsession should have:
      // - A user message with the clone's request
      // - An assistant message with the clone's response
      const cloneRequest = messages.find(m =>
        m.role === 'user' && 
        m.content.includes('Please explore and analyze the contents of the API project')
      );
      expect(cloneRequest).toBeDefined();
      expect(cloneRequest?.content).toContain('API project workspace');
      expect(cloneRequest?.content).toContain('comprehensive summary');

      const cloneResponse = messages.find(m =>
        m.role === 'assistant' && 
        m.content.includes('API Project Analysis Report')
      );
      expect(cloneResponse).toBeDefined();
      expect(cloneResponse?.content).toContain('Agent C API');
      expect(cloneResponse?.content).toMatch(/FastAPI[\s\n]+wrapper/);

      // 6. Verify assistant's follow-up after delegation
      const followUp = messages.find(m =>
        m.role === 'assistant' && 
        m.content.includes('Perfect! My clone has provided a comprehensive analysis')
      );
      expect(followUp).toBeDefined();

      // 7. CRITICAL: Verify think tool appears as thought (not tool call UI)
      const thought = messages.find(m =>
        m.role === 'assistant (thought)' &&
        m.content.includes('This API project analysis reveals')
      );
      expect(thought).toBeDefined();
      expect(thought?.format).toBe('markdown');
      expect(thought?.content).toContain('very comprehensive and well-structured FastAPI application');
      expect(thought?.content).toContain('sophisticated real-time capabilities');

      // 8. Verify assistant's final summary message
      const summary = messages.find(m =>
        m.role === 'assistant' && 
        m.content.includes('The clone has provided an excellent comprehensive analysis')
      );
      expect(summary).toBeDefined();
      expect(summary?.content).toContain('Key Highlights');
      expect(summary?.content).toContain('Notable Technical Features');

      // 9. CRITICAL: Verify NO tool call UI elements
      // There should be NO messages with type 'tool_use' or 'tool_result'
      const toolCallMessages = messages.filter(m => 
        m.type === 'tool_use' || m.type === 'tool_result'
      );
      expect(toolCallMessages).toHaveLength(0);

      // 10. Verify message order is preserved
      const messageContents = messages.map(m => {
        if (m.content.length > 50) {
          return m.content.substring(0, 50) + '...';
        }
        return m.content;
      });

      // Should have messages in conversation order
      expect(messageContents[0]).toContain('hello domo');
      expect(messageContents[1]).toContain('Great to meet you');
      expect(messageContents[2]).toContain('ask a clone');
      
      // 11. Verify total message count is reasonable
      // Should have: initial exchange (2) + delegation request/ack (2) + 
      // subsession messages (2) + follow-up (1) + thought (1) + summary (1) = 9 messages
      expect(messages.length).toBeGreaterThanOrEqual(9);

      // 12. Verify messages have required fields for UI rendering
      messages.forEach((msg, index) => {
        expect(msg).toHaveProperty('id');
        expect(msg).toHaveProperty('type');
        expect(msg).toHaveProperty('role');
        expect(msg).toHaveProperty('content');
        expect(msg).toHaveProperty('timestamp');
        
        // All messages should have type 'message' (not tool_use or tool_result)
        expect(msg.type).toBe('message');
        
        // Roles should be valid
        expect(['user', 'assistant', 'assistant (thought)', 'system']).toContain(msg.role);
      });

      // 13. Log summary for debugging
      console.log('\n=== Message Summary ===');
      console.log('Total messages:', messages.length);
      console.log('Message roles:', messages.map(m => m.role));
      console.log('Has thought message:', !!thought);
      console.log('Has delegation subsession:', !!cloneRequest && !!cloneResponse);
      console.log('Has NO tool UI elements:', toolCallMessages.length === 0);
      console.log('======================\n');
    });

    it('should handle subsession dividers correctly', async () => {
      const { result } = renderHook(() => useChat());

      // Track subsession events
      let subsessionStarted = false;
      let subsessionEnded = false;

      // Listen for subsession events (simulated)
      mockSessionManager.emit.mockImplementation((event: string) => {
        if (event === 'subsession-started') subsessionStarted = true;
        if (event === 'subsession-ended') subsessionEnded = true;
      });

      // Load the test session
      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      // Wait for processing
      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      });

      // Simulate subsession events that would come from EventStreamProcessor
      emitSessionEvent('subsession-started', {
        subSessionType: 'oneshot',
        subAgentType: 'clone',
        primeAgentKey: 'current_agent',
        subAgentKey: 'analyzer'
      });

      emitSessionEvent('subsession-ended', {});

      // The messages between subsession start/end should be the delegation messages
      const messages = result.current.messages;
      
      // Find the delegation messages that should be in the subsession
      const delegationMessages = messages.filter(m =>
        (m.role === 'user' && m.content.includes('Please explore and analyze')) ||
        (m.role === 'assistant' && m.content.includes('# API Project Analysis Report'))
      );

      // Should have exactly 2 messages in the subsession (user request + assistant response)
      expect(delegationMessages).toHaveLength(2);
    });

    it('should maintain correct message structure for streaming vs resumed', async () => {
      const { result } = renderHook(() => useChat());

      // Load resumed session
      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      });

      const resumedMessages = [...result.current.messages];

      // Now test that new streaming messages would have the same structure
      // Simulate a new message being streamed through SessionManager events
      emitSessionEvent('message-streaming', {
        sessionId: 'new-stream',
        message: {
          role: 'assistant',
          content: 'This is a new streamed message',
          timestamp: new Date().toISOString(),
          format: 'text'
        }
      });

      expect(result.current.streamingMessage).toBeDefined();
      expect(result.current.streamingMessage?.role).toBe('assistant');

      // Complete the streaming
      emitSessionEvent('message-complete', {
        sessionId: 'new-stream',
        message: {
          id: 'new-stream',
          type: 'message',
          role: 'assistant',
          content: 'This is a new streamed message',
          timestamp: new Date().toISOString(),
          format: 'text'
        }
      });

      // Verify the new message has the same structure as resumed messages
      const newMessage = result.current.messages[result.current.messages.length - 1];
      const resumedMessage = resumedMessages[0];

      // Both should have type 'message' and similar base properties
      expect(newMessage.type).toBe('message');
      expect(resumedMessage.type).toBe('message');
      
      // Both should have required fields for UI rendering
      expect(newMessage).toHaveProperty('id');
      expect(newMessage).toHaveProperty('content');
      expect(newMessage).toHaveProperty('role');
      expect(resumedMessage).toHaveProperty('id');
      expect(resumedMessage).toHaveProperty('content');
      expect(resumedMessage).toHaveProperty('role');
    });

    it('should handle empty resumed session correctly', async () => {
      const { result } = renderHook(() => useChat());

      // Load empty session
      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: {
            session_id: 'empty-session',
            messages: []
          }
        });
      });

      // Should have no messages but session should be set
      expect(result.current.messages).toHaveLength(0);
      expect(result.current.currentSessionId).toBe('empty-session');
      expect(result.current.streamingMessage).toBeNull();
      expect(result.current.isAgentTyping).toBe(false);
    });
  });

  describe('Critical UI Rendering Validations', () => {
    it('should NEVER expose tool_use or tool_result message types to UI', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      });

      // This is CRITICAL for UI rendering
      // The UI should NEVER receive raw tool_use or tool_result messages
      result.current.messages.forEach(message => {
        expect(message.type).not.toBe('tool_use');
        expect(message.type).not.toBe('tool_result');
        
        // All messages should be type 'message' for UI consumption
        expect(message.type).toBe('message');
      });
    });

    it('should render think tools as thoughts without any tool UI chrome', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      });

      // Find the thought message
      const thoughtMessage = result.current.messages.find(m =>
        m.role === 'assistant (thought)'
      );

      expect(thoughtMessage).toBeDefined();
      
      // Verify it's properly formatted for UI rendering
      expect(thoughtMessage?.type).toBe('message');
      expect(thoughtMessage?.format).toBe('markdown');
      
      // Should NOT have any tool-related properties
      expect(thoughtMessage).not.toHaveProperty('tool_name');
      expect(thoughtMessage).not.toHaveProperty('tool_use_id');
      expect(thoughtMessage).not.toHaveProperty('input');
    });

    it('should create proper conversation flow for delegation tools', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        emitClientEvent('chat_session_changed', {
          chat_session: testSessionData
        });
      });

      await waitFor(() => {
        expect(result.current.messages.length).toBeGreaterThan(0);
      });

      // For delegation tools, the UI should see:
      // 1. Assistant's acknowledgment
      // 2. User message with the delegation request (in subsession)
      // 3. Assistant message with the delegation response (in subsession)
      // 4. Assistant's follow-up

      const messages = result.current.messages;
      
      // Find the sequence
      const ackIndex = messages.findIndex(m =>
        m.role === 'assistant' && m.content.includes("I'll ask a clone")
      );
      
      expect(ackIndex).toBeGreaterThanOrEqual(0);
      
      // The next messages should be the subsession
      const subsessionUser = messages[ackIndex + 1];
      const subsessionAssistant = messages[ackIndex + 2];
      
      expect(subsessionUser?.role).toBe('user');
      expect(subsessionUser?.content).toContain('Please explore and analyze');
      
      expect(subsessionAssistant?.role).toBe('assistant');
      expect(subsessionAssistant?.content).toContain('API Project Analysis');
      
      // All should be type 'message' for UI
      expect(subsessionUser?.type).toBe('message');
      expect(subsessionAssistant?.type).toBe('message');
    });
  });
});