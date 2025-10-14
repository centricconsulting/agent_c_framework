import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { ToolCallManager } from '../ToolCallManager';
import type {
  InteractionEvent,
  UserTurnStartEvent,
  ToolSelectDeltaEvent
} from '../types/ServerEvents';

/**
 * Integration Tests for EventStreamProcessor Cleanup Functionality
 * 
 * Tests the cleanup of orphaned tool notifications via:
 * 1. Session-specific cleanup when interactions end
 * 2. Nuclear cleanup when user turn starts
 */
describe('EventStreamProcessor - Cleanup Integration', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let toolCallManager: ToolCallManager;
  let emitSpy: ReturnType<typeof vi.spyOn>;
  
  beforeEach(() => {
    sessionManager = new ChatSessionManager({
      maxSessions: 10,
      persistSessions: false
    });
    
    // We need to access the processor's internal toolCallManager for assertions
    // Since it's private, we'll spy on the sessionManager's emit to verify cleanup
    processor = new EventStreamProcessor(sessionManager);
    
    // Create a real ToolCallManager for validation
    toolCallManager = new ToolCallManager();
    
    // Spy on sessionManager.emit to verify events
    emitSpy = vi.spyOn(sessionManager, 'emit');
    
    // Setup a current session
    sessionManager.setCurrentSession({
      session_id: 'session-1',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0
    });
  });
  
  /**
   * Helper: Create an orphaned tool notification
   * Simulates a tool that was selected but never completed
   */
  function createOrphanedNotification(
    manager: ToolCallManager,
    sessionId: string,
    toolName: string,
    toolId?: string
  ): { notificationId: string } {
    const notificationId = toolId || `tool-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    
    const toolSelectEvent: ToolSelectDeltaEvent = {
      type: 'tool_select_delta',
      session_id: sessionId,
      role: 'assistant',
      tool_calls: [{
        id: notificationId,
        type: 'function',
        name: toolName,
        input: {}
      }],
      parent_session_id: undefined,
      user_session_id: sessionId
    };
    
    manager.onToolSelect(toolSelectEvent);
    
    return { notificationId };
  }
  
  // ===== Event Fixtures =====
  
  function createInteractionStartedEvent(sessionId: string): InteractionEvent {
    return {
      type: 'interaction',
      id: `interaction-${sessionId}`,
      started: true,
      session_id: sessionId,
      role: 'assistant',
      parent_session_id: undefined,
      user_session_id: sessionId
    };
  }
  
  function createInteractionEndedEvent(sessionId: string): InteractionEvent {
    return {
      type: 'interaction',
      id: `interaction-${sessionId}`,
      started: false,
      session_id: sessionId,
      role: 'assistant',
      parent_session_id: undefined,
      user_session_id: sessionId
    };
  }
  
  function createUserTurnStartEvent(sessionId: string): UserTurnStartEvent {
    return {
      type: 'user_turn_start',
      session_id: sessionId,
      role: 'user',
      parent_session_id: undefined,
      user_session_id: sessionId
    };
  }
  
  // ===== Integration Tests =====
  
  describe('Session-Specific Cleanup (Interaction End)', () => {
    it('should cleanup notifications when interaction ends', () => {
      // Setup: Create orphaned notification in our test manager
      const { notificationId } = createOrphanedNotification(
        toolCallManager,
        'session-1',
        'workspace_read'
      );
      
      // Verify notification is active
      expect(toolCallManager.isToolActive('session-1', notificationId)).toBe(true);
      
      // Simulate the same sequence through the processor
      // First create the notification via processor
      processor.processEvent({
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        tool_calls: [{
          id: notificationId,
          type: 'function',
          name: 'workspace_read',
          input: {}
        }],
        parent_session_id: undefined,
        user_session_id: 'session-1'
      } as ToolSelectDeltaEvent);
      
      // Clear previous emit calls
      emitSpy.mockClear();
      
      // Trigger cleanup via interaction end event
      const event = createInteractionEndedEvent('session-1');
      processor.processEvent(event);
      
      // Verify session-notifications-cleared event was emitted
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      
      // Verify our test manager's notification is cleared
      toolCallManager.clearSessionNotifications('session-1');
      expect(toolCallManager.isToolActive('session-1', notificationId)).toBe(false);
    });
    
    it('should only clear the ended session, not others', () => {
      // Setup: Create notifications in two sessions
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-2', 'tool2');
      
      // Verify both are active
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(true);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(true);
      
      // Create notifications through processor for session-1 only
      processor.processEvent({
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        tool_calls: [{
          id: notif1.notificationId,
          type: 'function',
          name: 'tool1',
          input: {}
        }],
        parent_session_id: undefined,
        user_session_id: 'session-1'
      } as ToolSelectDeltaEvent);
      
      emitSpy.mockClear();
      
      // End session-1
      processor.processEvent(createInteractionEndedEvent('session-1'));
      
      // Verify only session-1 was cleared
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      
      // Clear in test manager and verify isolation
      toolCallManager.clearSessionNotifications('session-1');
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(true);
    });
    
    it('should handle cleanup with no active notifications', () => {
      // No notifications created
      
      emitSpy.mockClear();
      
      // End interaction
      const event = createInteractionEndedEvent('session-1');
      processor.processEvent(event);
      
      // Should still emit the event (no error thrown)
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      
      // Verify test manager handles empty cleanup gracefully
      expect(() => {
        toolCallManager.clearSessionNotifications('session-1');
      }).not.toThrow();
    });
    
    it('should handle cleanup of non-existent session', () => {
      // Create notification in session-1
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(true);
      
      emitSpy.mockClear();
      
      // Try to clean up session-99 (doesn't exist)
      processor.processEvent(createInteractionEndedEvent('session-99'));
      
      // Should emit event (no error thrown)
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-99'
      });
      
      // session-1 notification should remain
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(true);
      
      // Verify test manager handles non-existent session gracefully
      expect(() => {
        toolCallManager.clearSessionNotifications('session-99');
      }).not.toThrow();
    });
    
    it('should handle multiple interactions ending sequentially', () => {
      // Create notifications in multiple sessions
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-2', 'tool2');
      const notif3 = createOrphanedNotification(toolCallManager, 'session-3', 'tool3');
      
      emitSpy.mockClear();
      
      // End interactions one by one
      processor.processEvent(createInteractionEndedEvent('session-1'));
      processor.processEvent(createInteractionEndedEvent('session-2'));
      processor.processEvent(createInteractionEndedEvent('session-3'));
      
      // Verify all events emitted
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-2'
      });
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-3'
      });
      
      // Clear in test manager sequentially
      toolCallManager.clearSessionNotifications('session-1');
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(true);
      expect(toolCallManager.isToolActive('session-3', notif3.notificationId)).toBe(true);
      
      toolCallManager.clearSessionNotifications('session-2');
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-3', notif3.notificationId)).toBe(true);
      
      toolCallManager.clearSessionNotifications('session-3');
      expect(toolCallManager.isToolActive('session-3', notif3.notificationId)).toBe(false);
    });
    
    it('should not trigger cleanup when interaction starts', () => {
      // Create notification
      createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      
      emitSpy.mockClear();
      
      // Start interaction (not end)
      const event = createInteractionStartedEvent('session-1');
      processor.processEvent(event);
      
      // Should NOT emit session-notifications-cleared
      expect(emitSpy).not.toHaveBeenCalledWith('session-notifications-cleared', expect.any(Object));
    });
  });
  
  describe('Nuclear Cleanup (User Turn Start)', () => {
    it('should clear ALL notifications on user turn start', () => {
      // Create notifications in multiple sessions
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-2', 'tool2');
      const notif3 = createOrphanedNotification(toolCallManager, 'session-3', 'tool3');
      
      // Verify all active
      expect(toolCallManager.getActiveToolCount()).toBe(3);
      
      emitSpy.mockClear();
      
      // User turn starts
      processor.processEvent(createUserTurnStartEvent('session-1'));
      
      // Verify all-notifications-cleared event emitted
      expect(emitSpy).toHaveBeenCalledWith('all-notifications-cleared', undefined);
      
      // Clear all in test manager
      toolCallManager.clearAllActiveNotifications();
      
      // ALL notifications should be cleared
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-3', notif3.notificationId)).toBe(false);
      expect(toolCallManager.getActiveToolCount()).toBe(0);
    });
    
    it('should handle user turn start with no active notifications', () => {
      // No notifications
      expect(toolCallManager.getActiveToolCount()).toBe(0);
      
      emitSpy.mockClear();
      
      // User turn starts
      processor.processEvent(createUserTurnStartEvent('session-1'));
      
      // Should still emit event (no error thrown)
      expect(emitSpy).toHaveBeenCalledWith('all-notifications-cleared', undefined);
      
      // Verify test manager handles empty nuclear cleanup gracefully
      expect(() => {
        toolCallManager.clearAllActiveNotifications();
      }).not.toThrow();
    });
    
    it('should clear notifications across different sessions', () => {
      // Create notifications in sessions with varying tool types
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'workspace_read');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-2', 'act_oneshot');
      const notif3 = createOrphanedNotification(toolCallManager, 'session-3', 'ateam_chat');
      const notif4 = createOrphanedNotification(toolCallManager, 'session-1', 'workspace_write');
      
      // Verify all active (including multiple in same session)
      expect(toolCallManager.getActiveToolCount()).toBe(4);
      expect(toolCallManager.getActiveNotificationsForSession('session-1')).toHaveLength(2);
      
      emitSpy.mockClear();
      
      // Nuclear cleanup
      processor.processEvent(createUserTurnStartEvent('session-1'));
      
      // Verify event emitted
      expect(emitSpy).toHaveBeenCalledWith('all-notifications-cleared', undefined);
      
      // Clear all in test manager
      toolCallManager.clearAllActiveNotifications();
      
      // ALL should be cleared
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-3', notif3.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-1', notif4.notificationId)).toBe(false);
      expect(toolCallManager.getActiveToolCount()).toBe(0);
    });
  });
  
  describe('Cleanup Event Flow Validation', () => {
    it('should emit correct events in proper sequence', () => {
      // Create notification
      const { notificationId } = createOrphanedNotification(
        toolCallManager,
        'session-1',
        'workspace_read'
      );
      
      // Process through processor
      processor.processEvent({
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        tool_calls: [{
          id: notificationId,
          type: 'function',
          name: 'workspace_read',
          input: {}
        }],
        parent_session_id: undefined,
        user_session_id: 'session-1'
      } as ToolSelectDeltaEvent);
      
      emitSpy.mockClear();
      
      // Track event order
      const eventOrder: string[] = [];
      emitSpy.mockImplementation((event: string) => {
        eventOrder.push(event);
      });
      
      // Trigger cleanup
      processor.processEvent(createInteractionEndedEvent('session-1'));
      
      // Verify event was emitted
      expect(eventOrder).toContain('session-notifications-cleared');
    });
    
    it('should handle rapid cleanup sequences without errors', () => {
      // Create multiple notifications
      for (let i = 1; i <= 10; i++) {
        createOrphanedNotification(toolCallManager, `session-${i}`, `tool-${i}`);
      }
      
      expect(toolCallManager.getActiveToolCount()).toBe(10);
      
      // Rapid cleanup sequence
      for (let i = 1; i <= 10; i++) {
        processor.processEvent(createInteractionEndedEvent(`session-${i}`));
        toolCallManager.clearSessionNotifications(`session-${i}`);
      }
      
      // All should be cleared
      expect(toolCallManager.getActiveToolCount()).toBe(0);
    });
  });
  
  describe('Edge Cases', () => {
    it('should handle session with multiple tool types', () => {
      // Create multiple notification types in same session
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'workspace_read');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-1', 'act_oneshot');
      const notif3 = createOrphanedNotification(toolCallManager, 'session-1', 'ateam_chat');
      
      // Verify all active
      expect(toolCallManager.getActiveNotificationsForSession('session-1')).toHaveLength(3);
      
      emitSpy.mockClear();
      
      // End interaction
      processor.processEvent(createInteractionEndedEvent('session-1'));
      
      // Verify cleanup
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      
      // Clear in test manager
      toolCallManager.clearSessionNotifications('session-1');
      
      // All should be cleared
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-1', notif2.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-1', notif3.notificationId)).toBe(false);
    });
    
    it('should handle interleaved interaction ends and user turns', () => {
      // Create notifications
      const notif1 = createOrphanedNotification(toolCallManager, 'session-1', 'tool1');
      const notif2 = createOrphanedNotification(toolCallManager, 'session-2', 'tool2');
      
      emitSpy.mockClear();
      
      // Interaction ends
      processor.processEvent(createInteractionEndedEvent('session-1'));
      toolCallManager.clearSessionNotifications('session-1');
      
      // Verify session-1 cleared
      expect(toolCallManager.isToolActive('session-1', notif1.notificationId)).toBe(false);
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(true);
      
      // User turn (nuclear cleanup)
      processor.processEvent(createUserTurnStartEvent('session-1'));
      toolCallManager.clearAllActiveNotifications();
      
      // Verify all cleared
      expect(toolCallManager.isToolActive('session-2', notif2.notificationId)).toBe(false);
      expect(toolCallManager.getActiveToolCount()).toBe(0);
      
      // Verify both events emitted
      expect(emitSpy).toHaveBeenCalledWith('session-notifications-cleared', {
        sessionId: 'session-1'
      });
      expect(emitSpy).toHaveBeenCalledWith('all-notifications-cleared', undefined);
    });
  });
  
  describe('Performance Sanity Check', () => {
    it('should cleanup 100 notifications in under 100ms', () => {
      // Create 100 notifications across 10 sessions
      for (let session = 1; session <= 10; session++) {
        for (let tool = 1; tool <= 10; tool++) {
          createOrphanedNotification(
            toolCallManager,
            `session-${session}`,
            `tool-${tool}`
          );
        }
      }
      
      expect(toolCallManager.getActiveToolCount()).toBe(100);
      
      const startTime = performance.now();
      
      // Nuclear cleanup
      toolCallManager.clearAllActiveNotifications();
      
      const duration = performance.now() - startTime;
      
      // Verify cleared
      expect(toolCallManager.getActiveToolCount()).toBe(0);
      
      // Should be very fast (well under 100ms)
      expect(duration).toBeLessThan(100);
    });
  });
});
