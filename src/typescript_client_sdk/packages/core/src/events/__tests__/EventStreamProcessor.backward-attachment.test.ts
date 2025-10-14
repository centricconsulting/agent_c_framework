/**
 * EventStreamProcessor - Phase 3 Backward Attachment Tests
 * 
 * Tests for the correct behavior of attaching completed tool calls to previous messages.
 * This validates the CORE FIX where tools attach backward to the previous assistant message,
 * not forward during completion.
 */

import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import type { ToolCallEvent, CompletionEvent, TextDeltaEvent } from '../types/ServerEvents';
import type { Message } from '../types/CommonTypes';
import {
  createWithAssistantMessages,
  createMultipleAssistantMessages,
  createSessionWithThought,
  createUserOnlySession,
  createEmptySession,
  createWorkspaceReadToolCall,
  createMultipleToolCalls,
  createErrorToolCall,
  createToolCall
} from '../../test/fixtures/session-fixtures';

describe('EventStreamProcessor - Phase 3: Backward Tool Attachment', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;

  beforeEach(() => {
    vi.useFakeTimers();
    sessionManager = new ChatSessionManager();
    processor = new EventStreamProcessor(sessionManager);
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  describe('Core Backward Attachment (99% Case)', () => {
    it('should attach completed tools to previous assistant message', () => {
      // Setup: Session with existing assistant message
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      // Get the last assistant message before tool attachment
      const session = sessionManager.getCurrentSession()!;
      const lastAssistantMessage = session.messages[session.messages.length - 1];
      const messageId = lastAssistantMessage.id;

      // Simulate tool completion event (active=false)
      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{
          id: 'tool-1',
          type: 'tool_use',
          name: 'workspace_read',
          input: { path: '//WORKSPACE/test.txt' }
        }],
        tool_results: [{
          type: 'tool_result',
          tool_use_id: 'tool-1',
          content: 'File contents'
        }],
        active: false // Tool completed
      };

      processor.processEvent(toolCallEvent);

      // Verify: message-updated event emitted
      expect(messageUpdatedSpy).toHaveBeenCalledWith(
        expect.objectContaining({
          sessionId: 'with-assistant-session',
          messageId,
          message: expect.objectContaining({
            id: messageId,
            metadata: expect.objectContaining({
              toolCalls: expect.arrayContaining([
                expect.objectContaining({
                  id: 'tool-1',
                  name: 'workspace_read'
                })
              ]),
              toolResults: expect.arrayContaining([
                expect.objectContaining({
                  tool_use_id: 'tool-1',
                  content: 'File contents'
                })
              ])
            })
          })
        })
      );

      // Verify: Tools attached to previous message
      const updatedMessage = session.messages[session.messages.length - 1] as any;
      expect(updatedMessage.metadata?.toolCalls).toHaveLength(1);
      expect(updatedMessage.metadata?.toolResults).toHaveLength(1);
      expect(updatedMessage.metadata?.toolCalls[0].name).toBe('workspace_read');
    });

    it('should attach multiple tool calls to previous message', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'test' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: '//WORKSPACE/file.txt' }
          },
          {
            id: 'tool-3',
            type: 'tool_use',
            name: 'workspace_tree',
            input: { path: '//WORKSPACE' }
          }
        ],
        tool_results: [
          { type: 'tool_result', tool_use_id: 'tool-1', content: 'grep results' },
          { type: 'tool_result', tool_use_id: 'tool-2', content: 'file contents' },
          { type: 'tool_result', tool_use_id: 'tool-3', content: 'tree output' }
        ],
        active: false
      };

      processor.processEvent(toolCallEvent);

      // Verify all 3 tools attached
      expect(lastMessage.metadata?.toolCalls).toHaveLength(3);
      expect(lastMessage.metadata?.toolResults).toHaveLength(3);
      expect(lastMessage.metadata?.toolCalls.map((t: any) => t.name)).toEqual([
        'workspace_grep',
        'workspace_read',
        'workspace_tree'
      ]);
    });

    it('should find last assistant message even with user messages after', () => {
      // Setup session with assistant message followed by user message
      const baseSession = createWithAssistantMessages();
      const session = {
        ...baseSession,
        messages: [
          ...baseSession.messages,
          {
            id: 'msg-user-new',
            role: 'user' as const,
            content: 'Another question',
            timestamp: new Date().toISOString(),
            format: 'text' as const
          }
        ]
      };
      
      sessionManager.setCurrentSession(session);

      // Get the assistant message ID (second-to-last)
      const assistantMessageId = baseSession.messages[baseSession.messages.length - 1].id;

      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test_tool', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result' }],
        active: false
      };

      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      processor.processEvent(toolCallEvent);

      // Should attach to LAST ASSISTANT MESSAGE, not the user message
      expect(messageUpdatedSpy).toHaveBeenCalledWith(
        expect.objectContaining({
          messageId: assistantMessageId
        })
      );
    });

    it('should find correct assistant message in session with multiple assistant messages', () => {
      const testSession = createMultipleAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      
      // Get the LAST assistant message (msg-6)
      const lastAssistantMessage = session.messages[session.messages.length - 1];
      expect(lastAssistantMessage.id).toBe('msg-6');
      expect(lastAssistantMessage.role).toBe('assistant');

      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'multiple-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test_tool', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result' }],
        active: false
      };

      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      processor.processEvent(toolCallEvent);

      // Should attach to msg-6 (most recent assistant)
      expect(messageUpdatedSpy).toHaveBeenCalledWith(
        expect.objectContaining({
          messageId: 'msg-6'
        })
      );

      // Verify msg-6 has tools, others don't
      const msg6 = session.messages.find(m => m.id === 'msg-6') as any;
      const msg4 = session.messages.find(m => m.id === 'msg-4') as any;
      const msg2 = session.messages.find(m => m.id === 'msg-2') as any;

      expect(msg6.metadata?.toolCalls).toHaveLength(1);
      expect(msg4.metadata?.toolCalls).toBeUndefined();
      expect(msg2.metadata?.toolCalls).toBeUndefined();
    });

    it('should NOT attach tools to thought messages', () => {
      const testSession = createSessionWithThought();
      sessionManager.setCurrentSession(testSession);

      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'thought-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test_tool', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result' }],
        active: false
      };

      processor.processEvent(toolCallEvent);

      // Should NOT attach to thought message, should buffer instead
      const session = sessionManager.getCurrentSession()!;
      const thoughtMessage = session.messages[session.messages.length - 1] as any;
      
      expect(thoughtMessage.type).toBe('thought');
      expect(thoughtMessage.metadata?.toolCalls).toBeUndefined();

      // Should have buffered the tools instead
      expect(sessionManager.hasPendingToolCalls('thought-session')).toBe(true);
    });
  });

  describe('Buffering Fallback (1% Case)', () => {
    it('should buffer tools when no session exists', () => {
      // NO session set
      const toolCall = createWorkspaceReadToolCall();
      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'nonexistent-session',
        tool_calls: [toolCall],
        tool_results: [toolCall.result!],
        active: false
      };

      processor.processEvent(toolCallEvent);

      // Should buffer for future message
      expect(sessionManager.hasPendingToolCalls('nonexistent-session')).toBe(true);
      expect(sessionManager.getPendingToolCalls('nonexistent-session')).toHaveLength(1);
    });

    it('should buffer tools when session has no messages', () => {
      const testSession = createEmptySession();
      sessionManager.setCurrentSession(testSession);

      const toolCall = createWorkspaceReadToolCall();
      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'empty-session',
        tool_calls: [toolCall],
        tool_results: [toolCall.result!],
        active: false
      };

      processor.processEvent(toolCallEvent);

      // Should buffer (no assistant message exists)
      expect(sessionManager.hasPendingToolCalls('empty-session')).toBe(true);
    });

    it('should buffer tools when session only has user messages', () => {
      const testSession = createUserOnlySession();
      sessionManager.setCurrentSession(testSession);

      const toolCall = createWorkspaceReadToolCall();
      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'user-only-session',
        tool_calls: [toolCall],
        tool_results: [toolCall.result!],
        active: false
      };

      processor.processEvent(toolCallEvent);

      // Should buffer (no assistant message to attach to)
      expect(sessionManager.hasPendingToolCalls('user-only-session')).toBe(true);
      expect(sessionManager.getPendingToolCalls('user-only-session')).toHaveLength(1);
    });

    it('should attach buffered tools to next completed message', () => {
      // Setup: Buffer tools first (no assistant message yet)
      const testSession = createEmptySession();
      sessionManager.setCurrentSession(testSession);
      
      const toolCall = createWorkspaceReadToolCall();
      const toolCallEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'empty-session',
        tool_calls: [toolCall],
        tool_results: [toolCall.result!],
        active: false
      };

      processor.processEvent(toolCallEvent);
      expect(sessionManager.hasPendingToolCalls('empty-session')).toBe(true);

      // Now create an assistant message via text deltas and completion
      const textDelta: TextDeltaEvent = {
        type: 'text_delta',
        session_id: 'empty-session',
        content: 'I\'ve read the file.'
      };

      const completion: CompletionEvent = {
        type: 'completion',
        session_id: 'empty-session',
        running: false,
        input_tokens: 100,
        output_tokens: 50,
        stop_reason: 'stop'
      };

      processor.processEvent(textDelta);
      processor.processEvent(completion);

      // Verify: Buffered tools attached to new message
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      expect(lastMessage.role).toBe('assistant');
      expect(lastMessage.metadata?.toolCalls).toHaveLength(1);
      expect(lastMessage.metadata?.toolCalls[0].name).toBe('workspace_read');

      // Buffer should be cleared
      expect(sessionManager.hasPendingToolCalls('empty-session')).toBe(false);
    });
  });

  describe('Tool Call Accumulation', () => {
    it('should accumulate multiple tool completions on same message', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      // First tool completes
      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'tool_one', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result-1' }],
        active: false
      });

      expect(lastMessage.metadata?.toolCalls).toHaveLength(1);

      // Second tool completes
      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-2', type: 'tool_use', name: 'tool_two', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-2', content: 'result-2' }],
        active: false
      });

      expect(lastMessage.metadata?.toolCalls).toHaveLength(2);

      // Third tool completes
      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-3', type: 'tool_use', name: 'tool_three', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-3', content: 'result-3' }],
        active: false
      });

      expect(lastMessage.metadata?.toolCalls).toHaveLength(3);
      expect(lastMessage.metadata?.toolResults).toHaveLength(3);
      expect(lastMessage.metadata?.toolCalls.map((t: any) => t.name)).toEqual([
        'tool_one',
        'tool_two',
        'tool_three'
      ]);
    });

    it('should handle tools completing out of order', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      // Tools complete in order: 3, 1, 2
      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-3', type: 'tool_use', name: 'third', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-3', content: 'result-3' }],
        active: false
      });

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'first', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result-1' }],
        active: false
      });

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-2', type: 'tool_use', name: 'second', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-2', content: 'result-2' }],
        active: false
      });

      // All tools should be attached in the order they completed
      expect(lastMessage.metadata?.toolCalls).toHaveLength(3);
      expect(lastMessage.metadata?.toolCalls.map((t: any) => t.name)).toEqual([
        'third',
        'first',
        'second'
      ]);
    });
  });

  describe('Event Emission', () => {
    it('should emit message-updated event when tools attached', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1];

      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test_tool', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result' }],
        active: false
      });

      expect(messageUpdatedSpy).toHaveBeenCalledTimes(1);
      expect(messageUpdatedSpy).toHaveBeenCalledWith({
        sessionId: 'with-assistant-session',
        messageId: lastMessage.id,
        message: expect.objectContaining({
          id: lastMessage.id,
          role: 'assistant',
          metadata: expect.objectContaining({
            toolCalls: expect.any(Array),
            toolResults: expect.any(Array)
          })
        })
      });
    });

    it('should emit message-updated for each tool completion', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      // Complete 3 tools separately
      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'one', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: '1' }],
        active: false
      });

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-2', type: 'tool_use', name: 'two', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-2', content: '2' }],
        active: false
      });

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-3', type: 'tool_use', name: 'three', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-3', content: '3' }],
        active: false
      });

      // Should emit message-updated 3 times (once per tool)
      expect(messageUpdatedSpy).toHaveBeenCalledTimes(3);
    });

    it('should NOT emit message-updated when buffering', () => {
      const testSession = createEmptySession();
      sessionManager.setCurrentSession(testSession);
      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-updated', messageUpdatedSpy);

      processor.processEvent({
        type: 'tool_call',
        session_id: 'empty-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result' }],
        active: false
      });

      // No message-updated because tools were buffered
      expect(messageUpdatedSpy).not.toHaveBeenCalled();
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should handle tool calls with no results', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'test', input: {} }],
        tool_results: [], // No results
        active: false
      });

      // Tool call should still be attached
      expect(lastMessage.metadata?.toolCalls).toHaveLength(1);
      // But no results
      expect(lastMessage.metadata?.toolResults).toHaveLength(0);
    });

    it('should handle tool results with errors', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'error_tool', input: {} }],
        tool_results: [{
          type: 'tool_result',
          tool_use_id: 'tool-1',
          content: 'Error occurred',
          is_error: true
        }],
        active: false
      });

      // Tool and error result should be attached
      expect(lastMessage.metadata?.toolCalls).toHaveLength(1);
      expect(lastMessage.metadata?.toolResults).toHaveLength(1);
      expect(lastMessage.metadata?.toolResults[0].is_error).toBe(true);
    });

    it('should handle rapid tool completions without data loss', () => {
      const testSession = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession);
      const session = sessionManager.getCurrentSession()!;
      const lastMessage = session.messages[session.messages.length - 1] as any;

      // Rapidly complete 10 tools
      for (let i = 0; i < 10; i++) {
        processor.processEvent({
          type: 'tool_call',
          session_id: 'with-assistant-session',
          tool_calls: [{ id: `tool-${i}`, type: 'tool_use', name: `tool_${i}`, input: {} }],
          tool_results: [{ type: 'tool_result', tool_use_id: `tool-${i}`, content: `result-${i}` }],
          active: false
        });
      }

      // All 10 tools should be attached
      expect(lastMessage.metadata?.toolCalls).toHaveLength(10);
      expect(lastMessage.metadata?.toolResults).toHaveLength(10);
    });

    it('should handle session switch mid-tool-execution', () => {
      // Start with session 1
      const testSession1 = createWithAssistantMessages();
      sessionManager.setCurrentSession(testSession1);

      processor.processEvent({
        type: 'tool_call',
        session_id: 'with-assistant-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'first', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'result-1' }],
        active: false
      });

      // Switch to session 2
      const testSession2 = createMultipleAssistantMessages();
      sessionManager.setCurrentSession(testSession2);

      processor.processEvent({
        type: 'tool_call',
        session_id: 'multiple-assistant-session',
        tool_calls: [{ id: 'tool-2', type: 'tool_use', name: 'second', input: {} }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-2', content: 'result-2' }],
        active: false
      });

      // Verify tools attached to correct sessions
      const session1Messages = testSession1.messages;
      const session1LastMessage = session1Messages[session1Messages.length - 1] as any;
      
      const session2 = sessionManager.getCurrentSession()!;
      const session2LastMessage = session2.messages[session2.messages.length - 1] as any;

      expect(session1LastMessage.metadata?.toolCalls[0].name).toBe('first');
      expect(session2LastMessage.metadata?.toolCalls[0].name).toBe('second');
    });
  });

  describe('Integration with Message Lifecycle', () => {
    it('should complete full message → tool → backward attachment flow', () => {
      const testSession = createEmptySession();
      sessionManager.setCurrentSession(testSession);
      const messageCompleteSpy = vi.fn();
      const messageUpdatedSpy = vi.fn();
      sessionManager.on('message-complete', messageCompleteSpy);
      sessionManager.on('message-updated', messageUpdatedSpy);

      // 1. Build and complete a message
      processor.processEvent({
        type: 'text_delta',
        session_id: 'empty-session',
        content: 'Let me check that file.'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        session_id: 'empty-session',
        running: false,
        stop_reason: 'stop'
      } as CompletionEvent);

      expect(messageCompleteSpy).toHaveBeenCalledTimes(1);

      // 2. Tools execute and complete
      processor.processEvent({
        type: 'tool_call',
        session_id: 'empty-session',
        tool_calls: [{ id: 'tool-1', type: 'tool_use', name: 'workspace_read', input: { path: 'test.txt' } }],
        tool_results: [{ type: 'tool_result', tool_use_id: 'tool-1', content: 'File contents' }],
        active: false
      });

      // 3. Verify backward attachment
      expect(messageUpdatedSpy).toHaveBeenCalledTimes(1);
      
      const session = sessionManager.getCurrentSession()!;
      const message = session.messages[0] as any;
      
      expect(message.content).toBe('Let me check that file.');
      expect(message.metadata?.toolCalls).toHaveLength(1);
      expect(message.metadata?.toolCalls[0].name).toBe('workspace_read');
    });
  });
});
