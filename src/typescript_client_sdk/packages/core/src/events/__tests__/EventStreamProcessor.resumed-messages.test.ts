/**
 * Test suite for EventStreamProcessor - mapResumedMessagesToEvents functionality
 * 
 * These tests verify that resumed messages are correctly converted to events
 * that will render identically to streamed messages:
 * 1. Think tools render as thoughts (not tool calls)
 * 2. Delegation tools (act_*, ateam_*) create subsessions with user/assistant messages
 * 3. YAML parsing from tool results works correctly
 * 4. Regular tool calls work normally
 * 5. Empty sessions clear properly
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { ChatSession } from '../types/CommonTypes';
import { MessageParam } from '../../types/message-params';
import { WebSocketTracker, MockWebSocket } from '../../test/mocks/websocket.mock';
import { server, startMockServer, resetMockServer, stopMockServer } from '../../test/mocks/server';
import { 
  chatSessionChangedSequence,
  toolCallSequence,
  multiToolCallSequence,
  subsessionSequence
} from '../../test/fixtures/event-sequences';
import testSession from './fixtures/session_with_delegation.json';

describe('EventStreamProcessor - Resumed Messages Mapping', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  let wsTracker: WebSocketTracker;
  let mockSession: ChatSession;
  const testSessionId = 'test-resumed-session';

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Start MSW server
    startMockServer();
    
    // Setup WebSocket tracker
    wsTracker = new WebSocketTracker();
    wsTracker.install();
    
    // Create a properly structured mock session
    mockSession = {
      session_id: testSessionId,
      session_name: 'Test Resumed Session',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      context_window_size: 128000,
      user_id: 'test-user',
      vendor: 'anthropic',
      metadata: {
        test: true,
        mode: 'resumed'
      }
    };

    // Create real instances
    sessionManager = new ChatSessionManager();
    processor = new EventStreamProcessor(sessionManager);
    
    // Setup spies
    sessionManagerEmitSpy = vi.spyOn(sessionManager, 'emit');
    
    // Set the current session
    sessionManager.setCurrentSession(mockSession);
  });

  afterEach(() => {
    vi.restoreAllMocks();
    processor.destroy();
    sessionManager.destroy();
    wsTracker.uninstall();
    resetMockServer();
  });

  describe('Think Tool Rendering', () => {
    it('should render think tools as thoughts, not tool calls', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'Let me think about this...'
            }
          ]
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'think-1',
              name: 'think',
              input: {
                thought: 'I need to analyze the request carefully. The user is asking about...'
              }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'think-1',
              content: ''
            }
          ]
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'Based on my analysis, here is the answer...'
            }
          ]
        }
      ];

      // Process the chat_session_changed event with version field
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Get events emitted
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      // Should have loaded the messages
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Should have 3 messages
      expect(loadedMessages).toHaveLength(3);

      // First: regular assistant text
      expect(loadedMessages[0]).toEqual(expect.objectContaining({
        role: 'assistant',
        content: 'Let me think about this...'
      }));

      // Second: thought rendered as assistant (thought)
      expect(loadedMessages[1]).toEqual(expect.objectContaining({
        role: 'assistant (thought)',
        content: 'I need to analyze the request carefully. The user is asking about...',
        format: 'markdown'
      }));

      // Third: final assistant response
      expect(loadedMessages[2]).toEqual(expect.objectContaining({
        role: 'assistant',
        content: 'Based on my analysis, here is the answer...'
      }));

      // Verify NO tool-call-complete events for think tool
      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );
      expect(toolCallCompleteCalls).toHaveLength(0);
    });
  });

  describe('Delegation Tool Subsessions', () => {
    it('should create subsessions for act_oneshot delegation', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'I\'ll ask a clone to help with that.'
            }
          ]
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_1',
              name: 'act_oneshot',
              input: {
                request: 'Please analyze the code structure',
                process_context: 'Focus on the main components',
                agent_key: 'analyzer'
              }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_1',
              content: `**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.
---

text: |
  # Code Structure Analysis
  
  The code is well organized with:
  - Clear module separation
  - Proper typing
  - Good test coverage`
            }
          ]
        }
      ];

      // Process the chat_session_changed event with version field
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Verify subsession events
      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );
      const subsessionEndCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-ended'
      );

      // Should have one subsession start and end
      expect(subsessionStartCalls).toHaveLength(1);
      expect(subsessionEndCalls).toHaveLength(1);

      // Verify subsession start details
      expect(subsessionStartCalls[0][1]).toEqual({
        subSessionType: 'oneshot',
        subAgentType: 'clone',
        primeAgentKey: 'current_agent',
        subAgentKey: 'analyzer'
      });

      // Get loaded messages
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Find the subsession messages
      const subsessionUserMsg = loadedMessages.find((msg: any) => 
        msg.content.includes('Please analyze the code structure')
      );
      const subsessionAssistantMsg = loadedMessages.find((msg: any) =>
        msg.content.includes('Code Structure Analysis')
      );

      expect(subsessionUserMsg).toBeDefined();
      expect(subsessionAssistantMsg).toBeDefined();
      
      // Verify user message includes process context
      expect(subsessionUserMsg.content).toContain('Focus on the main components');
    });

    it('should handle ateam_chat delegation with proper agent type', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_2',
              name: 'ateam_chat',
              input: {
                agent_key: 'team_member',
                message: 'What is the status of the project?',
                session_id: 'chat-123'
              }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_2',
              content: `text: 'The project is on track and 75% complete.'`
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );

      expect(subsessionStartCalls[0][1]).toEqual({
        subSessionType: 'chat',
        subAgentType: 'team',
        primeAgentKey: 'current_agent',
        subAgentKey: 'team_member'
      });
    });
  });

  describe('YAML Parsing', () => {
    it('should correctly parse simple YAML from tool results', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_3',
              name: 'act_oneshot',
              input: { request: 'Test request' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_3',
              content: `text: 'Simple response text'`
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const assistantResponse = loadedMessages.find((msg: any) =>
        msg.content === 'Simple response text'
      );

      expect(assistantResponse).toBeDefined();
    });

    it('should parse multi-line YAML with pipe notation', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_4',
              name: 'act_oneshot',
              input: { request: 'Multi-line test' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_4',
              content: `text: |
  Line 1 of the response
  Line 2 of the response
  Line 3 of the response`
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const assistantResponse = loadedMessages.find((msg: any) =>
        msg.role === 'assistant' &&
        msg.content.includes('Line 1 of the response')
      );

      expect(assistantResponse).toBeDefined();
      expect(assistantResponse.content).toContain('Line 1 of the response');
      expect(assistantResponse.content).toContain('Line 2 of the response');
      expect(assistantResponse.content).toContain('Line 3 of the response');
    });

    it('should handle YAML with preamble correctly', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_5',
              name: 'act_oneshot',
              input: { request: 'Preamble test' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_5',
              content: `**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.
---
text: 'Response without preamble'`
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const assistantResponse = loadedMessages.find((msg: any) =>
        msg.role === 'assistant' &&
        msg.content === 'Response without preamble'
      );

      expect(assistantResponse).toBeDefined();
      // Verify preamble was removed
      expect(assistantResponse.content).not.toContain('**IMPORTANT**');
    });
  });

  describe('Regular Tool Calls - Phase 4 Implementation', () => {
    it('should extract tool calls and attach them to message metadata', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_6',
              name: 'calculator',
              input: { operation: 'add', a: 5, b: 3 }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_6',
              content: '{"result": 8}'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Resumed messages should NOT emit tool-call-complete events
      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );
      expect(toolCallCompleteCalls).toHaveLength(0);

      // Verify NO subsession events for regular tools
      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );
      expect(subsessionStartCalls).toHaveLength(0);

      // PHASE 4: Verify tool calls are attached to message
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Should have one assistant message with tool call metadata
      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];
      
      // Verify message structure
      expect(assistantMsg.role).toBe('assistant');
      expect(assistantMsg.content).toBe('[Tool execution]'); // No text content

      // Verify metadata contains tool calls
      expect(assistantMsg.metadata).toBeDefined();
      expect(assistantMsg.metadata.toolCalls).toHaveLength(1);
      expect(assistantMsg.metadata.toolCalls[0]).toEqual({
        id: 'toolu_6',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'add', a: 5, b: 3 }
      });

      // Verify metadata contains tool results
      expect(assistantMsg.metadata.toolResults).toHaveLength(1);
      expect(assistantMsg.metadata.toolResults[0]).toEqual({
        type: 'tool_result',
        tool_use_id: 'toolu_6',
        content: '{"result": 8}',
        is_error: undefined
      });

      // Verify top-level fields (for compatibility)
      expect(assistantMsg.toolCalls).toEqual(assistantMsg.metadata.toolCalls);
      expect(assistantMsg.toolResults).toEqual(assistantMsg.metadata.toolResults);
    });

    it('should handle multiple tool calls in a single message (3+ tools)', () => {
      // CRITICAL TEST CASE: 3+ regular tool calls in one message
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_1',
              name: 'web_search',
              input: { query: 'current weather' }
            },
            {
              type: 'tool_use',
              id: 'tool_2',
              name: 'calculator',
              input: { operation: 'convert', value: 72, from: 'F', to: 'C' }
            },
            {
              type: 'tool_use',
              id: 'tool_3',
              name: 'data_formatter',
              input: { format: 'json', data: 'weather data' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_1',
              content: '{"temp": 72, "condition": "sunny"}'
            },
            {
              type: 'tool_result',
              tool_use_id: 'tool_2',
              content: '{"result": 22.2}'
            },
            {
              type: 'tool_result',
              tool_use_id: 'tool_3',
              content: '{"formatted": "Temperature: 72°F (22.2°C), Condition: Sunny"}'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];

      // Verify all 3 tool calls are extracted
      expect(assistantMsg.metadata.toolCalls).toHaveLength(3);
      expect(assistantMsg.metadata.toolCalls[0].name).toBe('web_search');
      expect(assistantMsg.metadata.toolCalls[1].name).toBe('calculator');
      expect(assistantMsg.metadata.toolCalls[2].name).toBe('data_formatter');

      // Verify all 3 tool results are matched
      expect(assistantMsg.metadata.toolResults).toHaveLength(3);
      expect(assistantMsg.metadata.toolResults[0].tool_use_id).toBe('tool_1');
      expect(assistantMsg.metadata.toolResults[1].tool_use_id).toBe('tool_2');
      expect(assistantMsg.metadata.toolResults[2].tool_use_id).toBe('tool_3');

      // Verify content is placeholder
      expect(assistantMsg.content).toBe('[Tool execution]');
    });

    it('should handle tool calls with text content', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'Let me search for that information and calculate the result.'
            },
            {
              type: 'tool_use',
              id: 'search_1',
              name: 'web_search',
              input: { query: 'AI advances 2024' }
            },
            {
              type: 'tool_use',
              id: 'calc_1',
              name: 'calculator',
              input: { operation: 'multiply', a: 100, b: 1.5 }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'search_1',
              content: '{"results": ["AI improvements"]}'
            },
            {
              type: 'tool_result',
              tool_use_id: 'calc_1',
              content: '{"result": 150}'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];

      // Verify text content is preserved
      expect(assistantMsg.content).toBe('Let me search for that information and calculate the result.');

      // Verify tool calls are still attached
      expect(assistantMsg.metadata.toolCalls).toHaveLength(2);
      expect(assistantMsg.metadata.toolResults).toHaveLength(2);
    });

    it('should handle tool calls without text content', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'notify_1',
              name: 'send_notification',
              input: { message: 'Task complete' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'notify_1',
              content: '{"status": "sent"}'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];

      // Should use placeholder content
      expect(assistantMsg.content).toBe('[Tool execution]');

      // Should have tool call metadata
      expect(assistantMsg.metadata.toolCalls).toHaveLength(1);
      expect(assistantMsg.metadata.toolResults).toHaveLength(1);
    });

    it('should handle tools without results (mismatched/missing results)', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_7',
              name: 'send_notification',
              input: { message: 'Task complete' }
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );
      expect(toolCallCompleteCalls).toHaveLength(0);

      // Verify message still created with tool call
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];

      // Tool call should be present
      expect(assistantMsg.metadata.toolCalls).toHaveLength(1);
      expect(assistantMsg.metadata.toolCalls[0].id).toBe('toolu_7');

      // Tool results should be empty (no match found)
      expect(assistantMsg.metadata.toolResults).toHaveLength(0);
    });

    it('should handle tool call error results', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'error_tool',
              name: 'failing_tool',
              input: { param: 'invalid' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'error_tool',
              content: 'Tool execution failed: Invalid parameter',
              is_error: true
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      const assistantMsg = loadedMessages[0];

      // Verify error result is captured
      expect(assistantMsg.metadata.toolResults).toHaveLength(1);
      expect(assistantMsg.metadata.toolResults[0].is_error).toBe(true);
      expect(assistantMsg.metadata.toolResults[0].content).toContain('Tool execution failed');
    });
  });

  describe('Empty Sessions', () => {
    it('should clear messages properly for empty sessions', () => {
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          session_id: testSessionId,
          messages: []
        }
      } as any);

      // Verify session-messages-loaded was called with empty array
      const sessionMessagesLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionMessagesLoadedCalls).toHaveLength(1);
      expect(sessionMessagesLoadedCalls[0][1]).toEqual({
        sessionId: testSessionId,
        messages: []
      });
    });

    it('should handle undefined messages array', () => {
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          session_id: testSessionId,
          messages: undefined as any
        }
      } as any);

      const sessionMessagesLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionMessagesLoadedCalls).toHaveLength(1);
      expect(sessionMessagesLoadedCalls[0][1]).toEqual({
        sessionId: testSessionId,
        messages: []
      });
    });
  });

  describe('Real Test Data from session_with_delegation.json', () => {
    it('should correctly process the actual test session with delegation', () => {
      // Use the actual test data
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: testSession as any
      } as any);

      // Count different event types
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );
      const subsessionEndCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-ended'
      );

      // The session has:
      // - Regular user and assistant messages
      // - One act_oneshot delegation
      // - One think tool
      
      // Get loaded messages
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;
      expect(loadedMessages.length).toBeGreaterThan(0);

      // Find the delegation subsession - ateam_chat to realtime_core_coordinator
      const delegationStart = subsessionStartCalls.find(call =>
        call[1].subAgentKey === 'realtime_core_coordinator' || call[1].subSessionType === 'chat'
      );
      expect(delegationStart).toBeDefined();

      // Find the think tool thought
      const thoughtMessage = loadedMessages.find((msg: any) =>
        msg.role === 'assistant (thought)' &&
        msg.content.includes('thought from agent')
      );
      expect(thoughtMessage).toBeDefined();

      // Verify subsessions are balanced
      expect(subsessionStartCalls).toHaveLength(subsessionEndCalls.length);

      // Verify user messages are present
      const userMessages = loadedMessages.filter((msg: any) =>
        msg.role === 'user'
      );
      expect(userMessages.length).toBeGreaterThan(0);

      // Check that delegation user message is present
      const helloDomo = userMessages.find((msg: any) =>
        msg.content.includes('Hello other agent')
      );
      expect(helloDomo).toBeDefined();
    });

    it('should handle fixture sequences with resumed messages', () => {
      // Test with the chatSessionChangedSequence from fixtures
      const event = chatSessionChangedSequence[0];
      processor.processEvent(event);

      // Verify session was loaded
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      expect(sessionLoadedCalls).toHaveLength(1);

      // Get loaded messages
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Check for TypeScript-related messages from the fixture
      const typescriptMessages = loadedMessages.filter(
        (msg: any) => msg.content.includes('TypeScript')
      );
      expect(typescriptMessages.length).toBeGreaterThan(0);

      // Verify thought role preservation from resumed session
      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant (thought)'
      );
      expect(thoughtMessages.length).toBeGreaterThan(0);
      expect(thoughtMessages[0].content).toContain('The user is asking about TypeScript');
    });
  });

  describe('Mixed Content Types', () => {
    it('should handle messages with both text and tool use blocks', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'Let me calculate that for you.'
            },
            {
              type: 'tool_use',
              id: 'toolu_8',
              name: 'calculator',
              input: { operation: 'multiply', a: 7, b: 6 }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_8',
              content: '{"result": 42}'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Get loaded messages
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );

      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Find the text message
      const textMessage = loadedMessages.find((msg: any) =>
        msg.content === 'Let me calculate that for you.'
      );
      
      expect(textMessage).toBeDefined();
      // Resumed messages should NOT emit tool-call-complete events
      expect(toolCallCompleteCalls).toHaveLength(0);
    });

    it('should skip tool result messages in user role', () => {
      const messages: MessageParam[] = [
        {
          role: 'user',
          content: 'Normal user message'
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'some-tool',
              content: 'This should be skipped'
            }
          ]
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Should only have the normal user message
      expect(loadedMessages).toHaveLength(1);
      expect(loadedMessages[0].content).toBe('Normal user message');
      
      // Should NOT have the tool result as a message
      const toolResultMessage = loadedMessages.find((msg: any) =>
        msg.content.includes('This should be skipped')
      );
      expect(toolResultMessage).toBeUndefined();
    });
  });

  describe('Edge Cases', () => {
    it('should handle malformed YAML gracefully', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'toolu_9',
              name: 'act_oneshot',
              input: { request: 'Malformed test' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'toolu_9',
              content: 'Not valid YAML: {{{{'
            }
          ]
        }
      ];

      // Should not throw
      expect(() => {
        processor.processEvent({
          type: 'chat_session_changed',
          chat_session: {
            version: 2,
            session_id: testSessionId,
            messages
          }
        } as any);
      }).not.toThrow();

      // Should still create subsession with raw content
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const assistantMessage = loadedMessages.find((msg: any) =>
        msg.role === 'assistant' &&
        msg.content.includes('Not valid YAML')
      );
      
      expect(assistantMessage).toBeDefined();
    });

    it('should handle messages with undefined content gracefully', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: undefined as any
        }
      ];

      expect(() => {
        processor.processEvent({
          type: 'chat_session_changed',
          chat_session: {
            version: 2,
            session_id: testSessionId,
            messages
          }
        } as any);
      }).not.toThrow();
    });

    it('should handle system messages', () => {
      const messages: MessageParam[] = [
        {
          role: 'system',
          content: 'System initialization message'
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      expect(loadedMessages).toHaveLength(1);
      expect(loadedMessages[0]).toEqual(expect.objectContaining({
        role: 'system',
        content: 'System initialization message'
      }));
    });
  });
});