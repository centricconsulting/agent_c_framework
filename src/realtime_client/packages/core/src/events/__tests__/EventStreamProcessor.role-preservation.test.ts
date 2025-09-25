/**
 * Test suite for EventStreamProcessor - Role Preservation
 * 
 * These tests verify that special roles are preserved through the entire event chain
 * and not modified or cast to standard vendor roles.
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { SessionManager } from '../../session/SessionManager';
import { ChatSession } from '../types/CommonTypes';
import { MessageParam } from '../../types/message-params';

describe('EventStreamProcessor - Role Preservation', () => {
  let processor: EventStreamProcessor;
  let sessionManager: SessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  const testSessionId = 'test-role-preservation';

  beforeEach(() => {
    vi.clearAllMocks();
    
    const mockSession: ChatSession = {
      session_id: testSessionId,
      session_name: 'Test Role Preservation',
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

  describe('Thought Role Preservation', () => {
    it('should preserve "assistant (thought)" role through event chain', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_1',
              name: 'think',
              input: { thought: 'Analyzing the problem...' }
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

      const thoughtMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant (thought)'
      );

      expect(thoughtMessages).toHaveLength(1);
      expect(thoughtMessages[0][1].message.role).toBe('assistant (thought)');
      expect(thoughtMessages[0][1].message.content).toBe('Analyzing the problem...');
    });

    it('should not cast thought role to regular assistant', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_2',
              name: 'think',
              input: { thought: 'Deep thinking content' }
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

      const assistantMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant'
      );

      const thoughtMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant (thought)'
      );

      expect(assistantMessages).toHaveLength(0);
      expect(thoughtMessages).toHaveLength(1);
    });
  });

  describe('System Role Preservation', () => {
    it('should preserve system role for system messages', () => {
      const messages: MessageParam[] = [
        {
          role: 'system' as any, // System messages might come from special contexts
          content: 'System notification: Connection established'
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          session_id: testSessionId,
          messages
        }
      } as any);

      const systemMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'system'
      );

      expect(systemMessages).toHaveLength(1);
      expect(systemMessages[0][1].message.content).toBe('System notification: Connection established');
    });
  });

  describe('Delegation Tool Handling', () => {
    it('should handle JSON format delegation results', () => {
      const jsonResult = JSON.stringify({
        notice: 'This response is displayed in UI',
        agent_message: {
          role: 'assistant',
          content: [
            { type: 'text', text: 'Delegation response content' }
          ]
        }
      });

      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_3',
              name: 'ateam_chat',
              input: { 
                agent_key: 'test_agent',
                message: 'Test delegation request'
              }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_3',
              content: jsonResult
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

      const userMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'user'
      );

      const assistantMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant'
      );

      expect(subsessionStartCalls).toHaveLength(1);
      expect(userMessages).toHaveLength(1);
      expect(userMessages[0][1].message.content).toBe('Test delegation request');
      expect(assistantMessages).toHaveLength(1);
      expect(assistantMessages[0][1].message.content).toBe('Delegation response content');
    });

    it('should handle YAML format delegation results for backward compatibility', () => {
      const yamlContent = `text: |
        This is a YAML response
        with multiple lines
        for backward compatibility`;

      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_4',
              name: 'act_oneshot',
              input: { 
                request: 'Legacy YAML request'
              }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_4',
              content: yamlContent
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

      const assistantMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant'
      );

      expect(assistantMessages).toHaveLength(1);
      expect(assistantMessages[0][1].message.content).toContain('This is a YAML response');
    });
  });

  describe('Special Tool Detection', () => {
    it('should correctly identify think tools', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_5',
              name: 'think',
              input: { thought: 'Thinking...' }
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

      const thoughtMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.role === 'assistant (thought)'
      );

      expect(thoughtMessages).toHaveLength(1);
    });

    it('should correctly identify delegation tools with act_ prefix', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_6',
              name: 'act_oneshot',
              input: { request: 'Test act tool' }
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

      const subsessionCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );

      expect(subsessionCalls).toHaveLength(1);
      expect(subsessionCalls[0][1].subSessionType).toBe('oneshot');
    });

    it('should correctly identify delegation tools with ateam_ prefix', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_7',
              name: 'ateam_chat',
              input: { 
                agent_key: 'coordinator',
                message: 'Team chat message'
              }
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

      const subsessionCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );

      expect(subsessionCalls).toHaveLength(1);
      expect(subsessionCalls[0][1].subSessionType).toBe('chat');
      expect(subsessionCalls[0][1].subAgentKey).toBe('coordinator');
    });

    it('should handle regular tools without special processing', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_8',
              name: 'calculator',
              input: { expression: '2 + 2' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_8',
              content: '4'
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

      const toolCallCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'tool-call-complete'
      );

      const subsessionCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );

      expect(toolCallCalls).toHaveLength(1);
      expect(subsessionCalls).toHaveLength(0);
    });
  });

  describe('Event Sequencing', () => {
    it('should emit events in correct order for resumed sessions', () => {
      const messages: MessageParam[] = [
        {
          role: 'user',
          content: 'Initial message'
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'I need to think about this.'
            },
            {
              type: 'tool_use',
              id: 'tool_9',
              name: 'think',
              input: { thought: 'Processing...' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_9',
              content: 'null'
            }
          ]
        },
        {
          role: 'assistant',
          content: 'Here is my response.'
        }
      ];

      processor.processEvent({
        type: 'chat_session_changed',
        chat_session: {
          session_id: testSessionId,
          messages
        }
      } as any);

      const allCalls = sessionManagerEmitSpy.mock.calls;
      const eventSequence = allCalls.map(call => call[0]);

      // Verify correct sequence
      expect(eventSequence).toContain('session-messages-loaded');
      expect(eventSequence).toContain('message-added');

      // Verify order of message-added events
      const messageAddedCalls = allCalls.filter(call => call[0] === 'message-added');
      expect(messageAddedCalls[0][1].message.role).toBe('user');
      expect(messageAddedCalls[1][1].message.role).toBe('assistant (thought)');
      expect(messageAddedCalls[1][1].message.content).toBe('Processing...');
      expect(messageAddedCalls[2][1].message.content).toBe('I need to think about this.');
      expect(messageAddedCalls[3][1].message.role).toBe('assistant');
    });

    it('should not emit duplicate events for the same content', () => {
      const messages: MessageParam[] = [
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'tool_10',
              name: 'think',
              input: { thought: 'Unique thought' }
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

      const thoughtMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-added' && 
        call[1].message.content === 'Unique thought'
      );

      expect(thoughtMessages).toHaveLength(1);
    });
  });
});