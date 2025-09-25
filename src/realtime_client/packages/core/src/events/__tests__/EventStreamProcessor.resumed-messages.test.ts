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
import { SessionManager } from '../../session/SessionManager';
import { ChatSession } from '../types/CommonTypes';
import { MessageParam } from '../../types/message-params';
import testSession from './fixtures/session_with_delegation.json';

describe('EventStreamProcessor - Resumed Messages Mapping', () => {
  let processor: EventStreamProcessor;
  let sessionManager: SessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  let mockSession: ChatSession;
  const testSessionId = 'test-resumed-session';

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Create a mock session
    mockSession = {
      session_id: testSessionId,
      session_name: 'Test Resumed Session',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      metadata: {}
    };

    // Create real instances
    sessionManager = new SessionManager();
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

      // Process the chat_session_changed event
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          session_id: testSessionId,
          messages
        }
      } as any);

      // Verify the events emitted
      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      // Should have 3 message-added events
      expect(messageAddedCalls).toHaveLength(3);

      // First: regular assistant text
      expect(messageAddedCalls[0][1]).toEqual({
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'assistant',
          content: 'Let me think about this...'
        })
      });

      // Second: thought rendered as assistant (thought)
      expect(messageAddedCalls[1][1]).toEqual({
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'assistant (thought)',
          content: 'I need to analyze the request carefully. The user is asking about...',
          format: 'markdown'
        })
      });

      // Third: final assistant response
      expect(messageAddedCalls[2][1]).toEqual({
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'assistant',
          content: 'Based on my analysis, here is the answer...'
        })
      });

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

      // Process the chat_session_changed event
      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
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

      // Verify messages in subsession
      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      // Find the subsession messages
      const subsessionUserMsg = messageAddedCalls.find(call => 
        call[1].message.content.includes('Please analyze the code structure')
      );
      const subsessionAssistantMsg = messageAddedCalls.find(call =>
        call[1].message.content.includes('Code Structure Analysis')
      );

      expect(subsessionUserMsg).toBeDefined();
      expect(subsessionAssistantMsg).toBeDefined();
      
      // Verify user message includes process context
      expect(subsessionUserMsg![1].message.content).toContain('Focus on the main components');
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      const assistantResponse = messageAddedCalls.find(call =>
        call[1].message.content === 'Simple response text'
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      const assistantResponse = messageAddedCalls.find(call =>
        call[1].message.role === 'assistant' &&
        call[1].message.content.includes('Line 1 of the response')
      );

      expect(assistantResponse).toBeDefined();
      expect(assistantResponse![1].message.content).toContain('Line 1 of the response');
      expect(assistantResponse![1].message.content).toContain('Line 2 of the response');
      expect(assistantResponse![1].message.content).toContain('Line 3 of the response');
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      const assistantResponse = messageAddedCalls.find(call =>
        call[1].message.role === 'assistant' &&
        call[1].message.content === 'Response without preamble'
      );

      expect(assistantResponse).toBeDefined();
      // Verify preamble was removed
      expect(assistantResponse![1].message.content).not.toContain('**IMPORTANT**');
    });
  });

  describe('Regular Tool Calls', () => {
    it('should handle regular tool calls with tool-call-complete events', () => {
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
          session_id: testSessionId,
          messages
        }
      } as any);

      // Verify tool-call-complete event
      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );

      expect(toolCallCompleteCalls).toHaveLength(1);
      expect(toolCallCompleteCalls[0][1]).toEqual({
        toolCalls: [{
          id: 'toolu_6',
          name: 'calculator',
          input: { operation: 'add', a: 5, b: 3 }
        }],
        toolResults: [{
          tool_use_id: 'toolu_6',
          content: '{"result": 8}',
          is_error: false
        }]
      });

      // Verify NO subsession events for regular tools
      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );
      expect(subsessionStartCalls).toHaveLength(0);
    });

    it('should handle tools without results', () => {
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );

      expect(toolCallCompleteCalls).toHaveLength(1);
      expect(toolCallCompleteCalls[0][1]).toEqual({
        toolCalls: [{
          id: 'toolu_7',
          name: 'send_notification',
          input: { message: 'Task complete' }
        }],
        toolResults: undefined
      });
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
      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
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
      
      // Verify we have messages
      expect(messageAddedCalls.length).toBeGreaterThan(0);

      // Find the delegation subsession - ateam_chat to realtime_core_coordinator
      const delegationStart = subsessionStartCalls.find(call =>
        call[1].subAgentKey === 'realtime_core_coordinator' || call[1].subSessionType === 'chat'
      );
      expect(delegationStart).toBeDefined();

      // Find the think tool thought
      const thoughtMessage = messageAddedCalls.find(call =>
        call[1].message.role === 'assistant (thought)' &&
        call[1].message.content.includes('thought from agent')
      );
      expect(thoughtMessage).toBeDefined();

      // Verify subsessions are balanced
      expect(subsessionStartCalls).toHaveLength(subsessionEndCalls.length);

      // Verify user messages are present
      const userMessages = messageAddedCalls.filter(call =>
        call[1].message.role === 'user'
      );
      expect(userMessages.length).toBeGreaterThan(0);

      // Check that delegation user message is present
      const helloDomo = userMessages.find(call =>
        call[1].message.content.includes('Hello other agent')
      );
      expect(helloDomo).toBeDefined();
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
          session_id: testSessionId,
          messages
        }
      } as any);

      // Should have both text message and tool call
      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );
      const toolCallCompleteCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );

      // Find the text message
      const textMessage = messageAddedCalls.find(call =>
        call[1].message.content === 'Let me calculate that for you.'
      );
      
      expect(textMessage).toBeDefined();
      expect(toolCallCompleteCalls).toHaveLength(1);
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      // Should only have the normal user message
      expect(messageAddedCalls).toHaveLength(1);
      expect(messageAddedCalls[0][1].message.content).toBe('Normal user message');
      
      // Should NOT have the tool result as a message
      const toolResultMessage = messageAddedCalls.find(call =>
        call[1].message.content.includes('This should be skipped')
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
            session_id: testSessionId,
            messages
          }
        } as any);
      }).not.toThrow();

      // Should still create subsession with raw content
      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      const assistantMessage = messageAddedCalls.find(call =>
        call[1].message.role === 'assistant' &&
        call[1].message.content.includes('Not valid YAML')
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
          session_id: testSessionId,
          messages
        }
      } as any);

      const messageAddedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added'
      );

      expect(messageAddedCalls).toHaveLength(1);
      expect(messageAddedCalls[0][1]).toEqual({
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'system',
          content: 'System initialization message'
        })
      });
    });
  });
});