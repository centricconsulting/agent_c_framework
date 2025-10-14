/**
 * Test suite for EventStreamProcessor - Role Preservation
 * 
 * These tests verify that special roles are preserved through the entire event chain
 * and not modified or cast to standard vendor roles.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { ChatSession } from '../types/CommonTypes';
import { MessageParam } from '../../types/message-params';
import { WebSocketTracker } from '../../test/mocks/websocket.mock';
import { 
  thoughtStreamingSequence,
  chatSessionChangedSequence,
  subsessionSequence,
  createCustomSequence
} from '../../test/fixtures/event-sequences';
import { server, startMockServer, resetMockServer, stopMockServer } from '../../test/mocks/server';

describe('EventStreamProcessor - Role Preservation', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  let wsTracker: WebSocketTracker;
  const testSessionId = 'test-role-preservation';

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Start MSW server for any HTTP mocking needed
    startMockServer();
    
    // Setup WebSocket tracker
    wsTracker = new WebSocketTracker();
    wsTracker.install();
    
    // Initialize session with proper structure
    const mockSession: ChatSession = {
      session_id: testSessionId,
      session_name: 'Test Role Preservation',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      context_window_size: 128000,
      user_id: 'test-user',
      vendor: 'anthropic',
      metadata: {
        test: true
      }
    };

    // Create instances and setup spies
    sessionManager = new ChatSessionManager();
    processor = new EventStreamProcessor(sessionManager);
    sessionManagerEmitSpy = vi.spyOn(sessionManager, 'emit');
    
    // Initialize session manager with the session
    sessionManager.setCurrentSession(mockSession);
  });

  afterEach(() => {
    // Cleanup
    vi.restoreAllMocks();
    processor.destroy();
    sessionManager.destroy();
    wsTracker.uninstall();
    resetMockServer();
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
              content: ''
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

      // Check for session-messages-loaded event which contains all processed messages
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;
      
      // Find thought messages in the loaded messages
      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant (thought)'
      );

      expect(thoughtMessages).toHaveLength(1);
      expect(thoughtMessages[0].role).toBe('assistant (thought)');
      expect(thoughtMessages[0].content).toBe('Analyzing the problem...');
      expect(thoughtMessages[0].format).toBe('markdown');
    });

    it('should handle streaming thought events correctly', () => {
      // Process the thought streaming sequence from fixtures
      thoughtStreamingSequence.forEach(event => {
        processor.processEvent(event);
      });

      // For streaming events, check for message-complete events
      const completedMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'message-complete' &&
        call[1].message.role === 'assistant (thought)'
      );

      expect(completedMessages).toHaveLength(1);
      expect(completedMessages[0][1].message.content).toContain('I need to analyze this carefully.');
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
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_2',
              content: ''
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

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;
      
      const assistantMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant' && !msg.role.includes('thought')
      );

      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant (thought)'
      );

      expect(assistantMessages).toHaveLength(0);
      expect(thoughtMessages).toHaveLength(1);
    });
  });

  describe('System Role Preservation', () => {
    it('should preserve system role for system messages', () => {
      const messages: MessageParam[] = [
        {
          role: 'system' as any,
          content: 'System notification: Connection established'
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

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;
      
      const systemMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'system'
      );

      expect(systemMessages).toHaveLength(1);
      expect(systemMessages[0].content).toBe('System notification: Connection established');
    });

    it('should handle system_message events from server', () => {
      processor.processEvent({
        type: 'system_message',
        session_id: testSessionId,
        role: 'system',
        content: 'Server maintenance in 5 minutes',
        format: 'text',
        severity: 'warning'
      });

      // System messages emit 'system_message', not 'message-added'
      const systemMessages = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'system_message'
      );

      expect(systemMessages).toHaveLength(1);
      expect(systemMessages[0][1].content).toBe('Server maintenance in 5 minutes');
      expect(systemMessages[0][1].severity).toBe('warning');
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
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Get events
      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );
      
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const userMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'user'
      );

      const assistantMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant' && !msg.role.includes('thought')
      );

      expect(subsessionStartCalls).toHaveLength(1);
      expect(subsessionStartCalls[0][1].subAgentKey).toBe('test_agent');
      expect(subsessionStartCalls[0][1].subSessionType).toBe('chat');
      expect(userMessages).toHaveLength(1);
      expect(userMessages[0].content).toBe('Test delegation request');
      expect(assistantMessages).toHaveLength(1);
      expect(assistantMessages[0].content).toBe('Delegation response content');
    });

    it('should handle subsession events from server correctly', () => {
      // Process subsession sequence from fixtures
      subsessionSequence.forEach(event => {
        processor.processEvent(event);
      });

      const subsessionStartCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-started'
      );

      const subsessionEndCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'subsession-ended'
      );

      expect(subsessionStartCalls).toHaveLength(1);
      expect(subsessionEndCalls).toHaveLength(1);
      expect(subsessionStartCalls[0][1].subAgentKey).toBe('specialist');
      expect(subsessionStartCalls[0][1].subSessionType).toBe('oneshot');
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
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const assistantMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant' && !msg.role.includes('thought')
      );

      expect(assistantMessages).toHaveLength(1);
      expect(assistantMessages[0].content).toContain('This is a YAML response');
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
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_5',
              content: ''
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

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant (thought)'
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
          version: 2,
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

      // Resumed messages should NOT emit tool-call-complete events
      expect(toolCallCalls).toHaveLength(0);
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
              content: ''
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
          version: 2,
          session_id: testSessionId,
          messages
        }
      } as any);

      const allCalls = sessionManagerEmitSpy.mock.calls;
      const eventSequence = allCalls.map(call => call[0]);

      // Verify correct sequence - bulk loads use session-messages-loaded, not message-added
      expect(eventSequence).toContain('session-messages-loaded');
      expect(eventSequence).not.toContain('message-added');

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = allCalls.filter(call => call[0] === 'session-messages-loaded');
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;
      
      // Verify order and content of loaded messages
      expect(loadedMessages[0].role).toBe('user');
      expect(loadedMessages[0].content).toBe('Initial message');
      expect(loadedMessages[1].role).toBe('assistant (thought)');
      expect(loadedMessages[1].content).toBe('Processing...');
      expect(loadedMessages[2].role).toBe('assistant');
      expect(loadedMessages[2].content).toBe('I need to think about this.');
      expect(loadedMessages[3].role).toBe('assistant');
      expect(loadedMessages[3].content).toBe('Here is my response.');
    });

    it('should handle chat session changed events from fixtures', () => {
      // Use the chatSessionChanged sequence from fixtures
      const sessionChangedEvent = chatSessionChangedSequence[0];
      processor.processEvent(sessionChangedEvent);

      // Verify session was loaded
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );

      expect(sessionLoadedCalls).toHaveLength(1);
      
      // Get loaded messages from session-messages-loaded event
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      // Should have user messages and assistant messages
      const userMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'user'
      );
      const assistantMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant' && !msg.role.includes('thought')
      );
      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.role === 'assistant (thought)'
      );

      expect(userMessages.length).toBeGreaterThan(0);
      expect(assistantMessages.length).toBeGreaterThan(0);
      expect(thoughtMessages.length).toBeGreaterThan(0);
      
      // Verify the thought content
      expect(thoughtMessages[0].content).toContain('The user is asking about TypeScript');
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
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'tool_10',
              content: ''
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

      // Get loaded messages from session-messages-loaded event
      const sessionLoadedCalls = sessionManagerEmitSpy.mock.calls.filter(
        call => call[0] === 'session-messages-loaded'
      );
      
      expect(sessionLoadedCalls).toHaveLength(1);
      const loadedMessages = sessionLoadedCalls[0][1].messages;

      const thoughtMessages = loadedMessages.filter(
        (msg: any) => msg.content === 'Unique thought'
      );

      expect(thoughtMessages).toHaveLength(1);
    });
  });
});