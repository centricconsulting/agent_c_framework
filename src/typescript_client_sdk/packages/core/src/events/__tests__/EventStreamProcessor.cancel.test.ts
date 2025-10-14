import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { CancelledEvent } from '../types/ServerEvents';

describe('EventStreamProcessor - Cancel Event Handling', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  
  beforeEach(() => {
    sessionManager = new ChatSessionManager({
      maxSessions: 10,
      persistSessions: false
    });
    processor = new EventStreamProcessor(sessionManager);
    
    // Setup a current session
    sessionManager.setCurrentSession({
      session_id: 'test-session',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0
    });
  });
  
  describe('handleCancelled', () => {
    it('should emit response-cancelled event when cancelled event is received', () => {
      const emitSpy = vi.spyOn(sessionManager, 'emit');
      
      const cancelledEvent: CancelledEvent = {
        type: 'cancelled'
      };
      
      processor.processEvent(cancelledEvent);
      
      // Should emit response-cancelled event
      expect(emitSpy).toHaveBeenCalledWith('response-cancelled', {});
    });
    
    it('should finalize any in-progress message with cancelled status', () => {
      const emitSpy = vi.spyOn(sessionManager, 'emit');
      
      // Start a message by processing a text delta
      processor.processEvent({
        type: 'text_delta',
        role: 'assistant',
        content: 'This is a partial response that will be can',
        format: 'text',
        session_id: 'test-session'
      } as any);
      
      // Process the cancelled event
      const cancelledEvent: CancelledEvent = {
        type: 'cancelled'
      };
      
      processor.processEvent(cancelledEvent);
      
      // Should emit message-complete with the partial message
      const messageCompleteCall = emitSpy.mock.calls.find(
        call => call[0] === 'message-complete'
      );
      
      expect(messageCompleteCall).toBeDefined();
      expect(messageCompleteCall?.[1].message.content).toContain('This is a partial response');
      expect(messageCompleteCall?.[1].message.metadata?.stopReason).toBe('cancelled');
    });
    
    it('should clear active tool notifications when cancelled', () => {
      const emitSpy = vi.spyOn(sessionManager, 'emit');
      
      // Start a tool call
      processor.processEvent({
        type: 'tool_select_delta',
        role: 'assistant',
        tool_calls: [{
          id: 'tool-1',
          type: 'function',
          name: 'test_tool',
          input: {}
        }],
        session_id: 'test-session'
      } as any);
      
      // Process the cancelled event
      const cancelledEvent: CancelledEvent = {
        type: 'cancelled'
      };
      
      processor.processEvent(cancelledEvent);
      
      // Should remove the tool notification
      const removeCall = emitSpy.mock.calls.find(
        call => call[0] === 'tool-notification-removed'
      );
      
      expect(removeCall).toBeDefined();
      // Updated: tool-notification-removed now passes an object, not just the ID
      expect(removeCall?.[1]).toMatchObject({
        sessionId: 'test-session',
        toolCallId: 'tool-1'
      });
    });
    
    it('should reset processor state after cancellation', () => {
      // Start a message
      processor.processEvent({
        type: 'text_delta',
        role: 'assistant',
        content: 'Test message',
        format: 'text',
        session_id: 'test-session'
      } as any);
      
      // Process the cancelled event
      const cancelledEvent: CancelledEvent = {
        type: 'cancelled'
      };
      
      processor.processEvent(cancelledEvent);
      
      // Start a new message - should work without issues
      processor.processEvent({
        type: 'text_delta',
        role: 'assistant',
        content: 'New message after cancel',
        format: 'text',
        session_id: 'test-session'
      } as any);
      
      // Should process normally
      const session = sessionManager.getCurrentSession();
      expect(session).toBeDefined();
    });
  });
});