import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ToolCallManager } from '../ToolCallManager';
import type { ToolSelectDeltaEvent, ToolCallEvent } from '../types/ServerEvents';
import type { ToolCall, ToolResult } from '../types/CommonTypes';

describe('ToolCallManager', () => {
  let manager: ToolCallManager;

  beforeEach(() => {
    manager = new ToolCallManager();
    vi.clearAllMocks();
  });

  describe('onToolSelect', () => {
    it('should create a notification for tool selection', () => {
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { param1: 'value1' }
          }
        ]
      };

      const notifications = manager.onToolSelect(event);

      expect(notifications).toHaveLength(1);
      expect(notifications[0]).toEqual({
        id: 'tool-1',
        sessionId: 'test-session',
        toolName: 'test_tool',
        status: 'preparing',
        timestamp: expect.any(Date),
        arguments: JSON.stringify({ param1: 'value1' })
      });
    });

    it('should handle multiple tools in one event', () => {
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool_one',
            input: { param1: 'value1' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool_two',
            input: { param2: 'value2' }
          },
          {
            id: 'tool-3',
            type: 'tool_use',
            name: 'tool_three',
            input: { param3: 'value3' }
          }
        ]
      };

      const notifications = manager.onToolSelect(event);

      expect(notifications).toHaveLength(3);
      expect(notifications[0].toolName).toBe('tool_one');
      expect(notifications[1].toolName).toBe('tool_two');
      expect(notifications[2].toolName).toBe('tool_three');
      expect(notifications.every(n => n.sessionId === 'test-session')).toBe(true);
    });

    it('should handle the think tool specially', () => {
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'think-1',
            type: 'tool_use',
            name: 'think',
            input: {}
          }
        ]
      };

      const notifications = manager.onToolSelect(event);

      expect(notifications).toHaveLength(1);
      expect(notifications[0].toolName).toBe('think');
      expect(notifications[0].status).toBe('preparing');
    });

    it('should throw error if no tool calls', () => {
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: []
      };

      expect(() => manager.onToolSelect(event)).toThrow('ToolSelectDeltaEvent has no tool calls');
    });

    it('should isolate tools by session', () => {
      const event1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      const event2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-2',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-2',
        tool_calls: [
          {
            id: 'tool-1', // Same tool ID, different session
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      manager.onToolSelect(event1);
      manager.onToolSelect(event2);

      // Both should be active because they're in different sessions
      expect(manager.isToolActive('session-1', 'tool-1')).toBe(true);
      expect(manager.isToolActive('session-2', 'tool-1')).toBe(true);
    });
  });

  describe('onToolCallActive', () => {
    it('should update existing notification to executing', () => {
      // First select the tool
      const selectEvent: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { param1: 'value1' }
          }
        ]
      };
      manager.onToolSelect(selectEvent);

      // Then mark it as active
      const activeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { param1: 'value1' }
          }
        ]
      };

      const notifications = manager.onToolCallActive(activeEvent);

      expect(notifications).toHaveLength(1);
      expect(notifications[0]).toEqual({
        id: 'tool-1',
        sessionId: 'test-session',
        toolName: 'test_tool',
        status: 'executing',
        timestamp: expect.any(Date),
        arguments: JSON.stringify({ param1: 'value1' })
      });
    });

    it('should handle multiple tools in active event', () => {
      // First select multiple tools
      const selectEvent: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool_one',
            input: {}
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool_two',
            input: {}
          }
        ]
      };
      manager.onToolSelect(selectEvent);

      // Then mark them as active
      const activeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool_one',
            input: {}
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool_two',
            input: {}
          }
        ]
      };

      const notifications = manager.onToolCallActive(activeEvent);

      expect(notifications).toHaveLength(2);
      expect(notifications.every(n => n.status === 'executing')).toBe(true);
    });

    it('should create new notification if not previously selected', () => {
      const activeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'another_tool',
            input: { key: 'value' }
          }
        ]
      };

      const notifications = manager.onToolCallActive(activeEvent);

      expect(notifications).toHaveLength(1);
      expect(notifications[0]).toEqual({
        id: 'tool-2',
        sessionId: 'test-session',
        toolName: 'another_tool',
        status: 'executing',
        timestamp: expect.any(Date),
        arguments: JSON.stringify({ key: 'value' })
      });
    });

    it('should return empty array for inactive event', () => {
      const inactiveEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      const notifications = manager.onToolCallActive(inactiveEvent);
      expect(notifications).toEqual([]);
    });
  });

  describe('onToolCallComplete', () => {
    it('should handle tool completion with results', () => {
      // First select and activate the tool
      const selectEvent: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolSelect(selectEvent);

      // Complete the tool with results
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ],
        tool_results: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: '{"result": "success"}'
          }
        ]
      };

      const completed = manager.onToolCallComplete(completeEvent);

      expect(completed).toHaveLength(1);
      expect(completed[0]).toEqual({
        id: 'tool-1',
        type: 'tool_use',
        name: 'test_tool',
        input: {},
        result: {
          type: 'tool_result',
          tool_use_id: 'tool-1',
          content: '{"result": "success"}'
        }
      });

      // Verify it's removed from active tools
      expect(manager.isToolActive('test-session', 'tool-1')).toBe(false);
    });

    it('should handle completion without results', () => {
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      const completed = manager.onToolCallComplete(completeEvent);

      expect(completed).toHaveLength(1);
      expect(completed[0].result).toBeUndefined();
    });

    it('should return empty array for active event', () => {
      const activeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      const completed = manager.onToolCallComplete(activeEvent);
      expect(completed).toEqual([]);
    });

    it('should isolate completion by session', () => {
      // Session 1 tool
      const select1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolSelect(select1);

      // Session 2 tool (same ID)
      const select2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-2',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-2',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolSelect(select2);

      // Complete only session-1 tool
      const complete1: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolCallComplete(complete1);

      // Session-1 should be inactive, session-2 should still be active
      expect(manager.isToolActive('session-1', 'tool-1')).toBe(false);
      expect(manager.isToolActive('session-2', 'tool-1')).toBe(true);
    });
  });

  describe('getActiveNotifications', () => {
    it('should return only active notifications', () => {
      // Add multiple tools
      const event1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool1',
            input: {}
          }
        ]
      };
      const event2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool2',
            input: {}
          }
        ]
      };

      manager.onToolSelect(event1);
      manager.onToolSelect(event2);

      const notifications = manager.getActiveNotifications();
      expect(notifications).toHaveLength(2);
      expect(notifications.map(n => n.id)).toEqual(['tool-1', 'tool-2']);
    });
  });

  describe('getActiveNotificationsForSession', () => {
    it('should return only notifications for specified session', () => {
      // Add tools for session 1
      const event1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool1',
            input: {}
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool2',
            input: {}
          }
        ]
      };
      manager.onToolSelect(event1);

      // Add tools for session 2
      const event2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-2',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-2',
        tool_calls: [
          {
            id: 'tool-3',
            type: 'tool_use',
            name: 'tool3',
            input: {}
          }
        ]
      };
      manager.onToolSelect(event2);

      const session1Notifications = manager.getActiveNotificationsForSession('session-1');
      expect(session1Notifications).toHaveLength(2);
      expect(session1Notifications.every(n => n.sessionId === 'session-1')).toBe(true);

      const session2Notifications = manager.getActiveNotificationsForSession('session-2');
      expect(session2Notifications).toHaveLength(1);
      expect(session2Notifications[0].sessionId).toBe('session-2');
    });

    it('should return empty array for session with no tools', () => {
      const notifications = manager.getActiveNotificationsForSession('nonexistent-session');
      expect(notifications).toEqual([]);
    });
  });

  describe('clearSessionNotifications', () => {
    it('should clear only notifications for specified session', () => {
      // Add tools for multiple sessions
      const event1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool1',
            input: {}
          }
        ]
      };
      const event2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-2',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-2',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool2',
            input: {}
          }
        ]
      };

      manager.onToolSelect(event1);
      manager.onToolSelect(event2);

      // Clear session-1
      manager.clearSessionNotifications('session-1');

      // Session-1 should be cleared, session-2 should remain
      expect(manager.getActiveNotificationsForSession('session-1')).toHaveLength(0);
      expect(manager.getActiveNotificationsForSession('session-2')).toHaveLength(1);
    });

    it('should handle clearing non-existent session', () => {
      // Should not throw
      expect(() => manager.clearSessionNotifications('nonexistent-session')).not.toThrow();
    });
  });

  describe('clearAllActiveNotifications', () => {
    it('should clear all notifications across all sessions', () => {
      // Add tools for multiple sessions
      const event1: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool1',
            input: {}
          }
        ]
      };
      const event2: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-2',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-2',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool2',
            input: {}
          }
        ]
      };

      manager.onToolSelect(event1);
      manager.onToolSelect(event2);

      // Clear all
      manager.clearAllActiveNotifications();

      // All should be cleared
      expect(manager.getActiveNotifications()).toHaveLength(0);
      expect(manager.getActiveNotificationsForSession('session-1')).toHaveLength(0);
      expect(manager.getActiveNotificationsForSession('session-2')).toHaveLength(0);
    });
  });

  describe('reset', () => {
    it('should clear all state', () => {
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };

      manager.onToolSelect(event);
      expect(manager.getActiveNotifications()).toHaveLength(1);

      manager.reset();

      expect(manager.getActiveNotifications()).toHaveLength(0);
      expect(manager.getCompletedToolCalls()).toHaveLength(0);
    });
  });

  describe('getStatistics', () => {
    it('should return correct statistics', () => {
      // Add active tool
      const selectEvent: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolSelect(selectEvent);

      // Complete a different tool
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'another_tool',
            input: {}
          }
        ]
      };
      manager.onToolCallComplete(completeEvent);

      const stats = manager.getStatistics();
      expect(stats).toEqual({
        activeCount: 1,
        completedCount: 1,
        totalCount: 2
      });
    });
  });

  describe('getNotification', () => {
    it('should retrieve specific notification by session and tool ID', () => {
      // First select a tool
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { key: 'value' }
          }
        ]
      };
      manager.onToolSelect(event);

      // Retrieve the notification
      const notification = manager.getNotification('test-session', 'tool-1');
      
      expect(notification).toBeDefined();
      expect(notification?.id).toBe('tool-1');
      expect(notification?.sessionId).toBe('test-session');
      expect(notification?.toolName).toBe('test_tool');
      expect(notification?.status).toBe('preparing');
    });

    it('should return undefined for non-existent notification', () => {
      const notification = manager.getNotification('non-existent-session', 'non-existent-tool');
      expect(notification).toBeUndefined();
    });

    it('should return undefined for wrong session ID', () => {
      // Add tool for session-1
      const event: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'session-1',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'session-1',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          }
        ]
      };
      manager.onToolSelect(event);

      // Try to retrieve with wrong session ID
      const notification = manager.getNotification('session-2', 'tool-1');
      expect(notification).toBeUndefined();
    });
  });

  describe('clearCompleted', () => {
    it('should clear completed tool calls', () => {
      // Complete a tool
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: {}
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'another_tool',
            input: {}
          }
        ]
      };
      manager.onToolCallComplete(completeEvent);

      // Verify completed tools exist
      expect(manager.getCompletedToolCalls()).toHaveLength(2);

      // Clear completed
      manager.clearCompleted();

      // Verify cleared
      expect(manager.getCompletedToolCalls()).toHaveLength(0);
    });

    it('should not affect active tools', () => {
      // Add active tool
      const selectEvent: ToolSelectDeltaEvent = {
        type: 'tool_select_delta',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        tool_calls: [
          {
            id: 'active-tool',
            type: 'tool_use',
            name: 'active',
            input: {}
          }
        ]
      };
      manager.onToolSelect(selectEvent);

      // Complete a different tool
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id: 'completed-tool',
            type: 'tool_use',
            name: 'completed',
            input: {}
          }
        ]
      };
      manager.onToolCallComplete(completeEvent);

      // Clear completed
      manager.clearCompleted();

      // Active tool should still exist
      expect(manager.getActiveNotifications()).toHaveLength(1);
      expect(manager.getCompletedToolCalls()).toHaveLength(0);
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should return empty array when onToolCallActive receives event with empty tool_calls', () => {
      const event: ToolCallEvent = {
        type: 'tool_call',
        session_id: 'test-session',
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic',
        tool_calls: []
      };

      const notifications = manager.onToolCallActive(event);
      expect(notifications).toEqual([]);
    });

    it('should return empty array when onToolCallActive receives event with undefined tool_calls', () => {
      const event = {
        type: 'tool_call' as const,
        session_id: 'test-session',
        role: 'assistant' as const,
        parent_session_id: undefined,
        user_session_id: 'test-session',
        active: true,
        vendor: 'anthropic' as const,
        tool_calls: undefined
      } as any;

      const notifications = manager.onToolCallActive(event);
      expect(notifications).toEqual([]);
    });
  });

  describe('Phase 2: Cleanup Behavior Verification', () => {
    describe('Session-Specific Cleanup Isolation', () => {
      it('should clear only the specified session without affecting others', () => {
        // Setup: Create tools for 3 independent sessions
        const sessions = ['session-1', 'session-2', 'session-3'];
        sessions.forEach(sessionId => {
          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: [
              {
                id: `tool-${sessionId}`,
                type: 'tool_use',
                name: 'test_tool',
                input: {}
              }
            ]
          };
          manager.onToolSelect(event);
        });

        // Verify all sessions have active tools
        expect(manager.isToolActive('session-1', 'tool-session-1')).toBe(true);
        expect(manager.isToolActive('session-2', 'tool-session-2')).toBe(true);
        expect(manager.isToolActive('session-3', 'tool-session-3')).toBe(true);

        // Clear only session-2
        manager.clearSessionNotifications('session-2');

        // Verify session-2 is cleared
        expect(manager.isToolActive('session-2', 'tool-session-2')).toBe(false);
        expect(manager.getNotification('session-2', 'tool-session-2')).toBeUndefined();
        expect(manager.getActiveNotificationsForSession('session-2')).toHaveLength(0);

        // Verify session-1 and session-3 are UNAFFECTED
        expect(manager.isToolActive('session-1', 'tool-session-1')).toBe(true);
        expect(manager.getNotification('session-1', 'tool-session-1')).toBeDefined();
        expect(manager.isToolActive('session-3', 'tool-session-3')).toBe(true);
        expect(manager.getNotification('session-3', 'tool-session-3')).toBeDefined();
      });

      it('should handle parent and child sessions independently', () => {
        // Setup: Parent session with tool
        const parentEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'parent-session',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'parent-session',
          tool_calls: [
            {
              id: 'parent-tool',
              type: 'tool_use',
              name: 'parent_calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(parentEvent);

        // Setup: Child session with tool (parent_session_id points to parent)
        const childEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'child-session',
          role: 'assistant',
          parent_session_id: 'parent-session',
          user_session_id: 'parent-session',
          tool_calls: [
            {
              id: 'child-tool',
              type: 'tool_use',
              name: 'child_calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(childEvent);

        // Verify both are active
        expect(manager.isToolActive('parent-session', 'parent-tool')).toBe(true);
        expect(manager.isToolActive('child-session', 'child-tool')).toBe(true);

        // Clear child session (interaction ends)
        manager.clearSessionNotifications('child-session');

        // Verify child is cleared
        expect(manager.isToolActive('child-session', 'child-tool')).toBe(false);
        expect(manager.getNotification('child-session', 'child-tool')).toBeUndefined();

        // Verify parent is UNAFFECTED
        expect(manager.isToolActive('parent-session', 'parent-tool')).toBe(true);
        expect(manager.getNotification('parent-session', 'parent-tool')).toBeDefined();
      });

      it('should handle same tool ID across different sessions independently', () => {
        // Setup: Same tool ID in 3 different sessions
        const sharedToolId = 'shared-tool-id';
        const sessions = ['session-A', 'session-B', 'session-C'];
        
        sessions.forEach(sessionId => {
          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: [
              {
                id: sharedToolId,
                type: 'tool_use',
                name: 'shared_tool',
                input: {}
              }
            ]
          };
          manager.onToolSelect(event);
        });

        // Verify all sessions have the same tool ID active
        sessions.forEach(sessionId => {
          expect(manager.isToolActive(sessionId, sharedToolId)).toBe(true);
        });

        // Clear session-B only
        manager.clearSessionNotifications('session-B');

        // Verify session-B is cleared
        expect(manager.isToolActive('session-B', sharedToolId)).toBe(false);
        expect(manager.getNotification('session-B', sharedToolId)).toBeUndefined();

        // Verify session-A and session-C still have the tool active
        expect(manager.isToolActive('session-A', sharedToolId)).toBe(true);
        expect(manager.getNotification('session-A', sharedToolId)).toBeDefined();
        expect(manager.isToolActive('session-C', sharedToolId)).toBe(true);
        expect(manager.getNotification('session-C', sharedToolId)).toBeDefined();
      });

      it('should verify complete removal from internal tracking', () => {
        // Setup: Create tool
        const event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'test-session',
          tool_calls: [
            {
              id: 'test-tool',
              type: 'tool_use',
              name: 'test_calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(event);

        // Verify tool exists before cleanup
        expect(manager.isToolActive('test-session', 'test-tool')).toBe(true);
        expect(manager.getNotification('test-session', 'test-tool')).toBeDefined();
        expect(manager.getActiveNotificationsForSession('test-session')).toHaveLength(1);

        // Clear session
        manager.clearSessionNotifications('test-session');

        // Verify COMPLETE removal - no trace left
        expect(manager.isToolActive('test-session', 'test-tool')).toBe(false);
        expect(manager.getNotification('test-session', 'test-tool')).toBeUndefined();
        expect(manager.getActiveNotificationsForSession('test-session')).toHaveLength(0);
        expect(manager.getActiveNotifications()).toHaveLength(0);
      });
    });

    describe('Nuclear Cleanup (Clear All)', () => {
      it('should clear ALL notifications across ALL sessions', () => {
        // Setup: Create tools for multiple sessions
        const sessions = ['session-1', 'session-2', 'session-3', 'session-4'];
        sessions.forEach(sessionId => {
          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: [
              {
                id: `tool-${sessionId}-1`,
                type: 'tool_use',
                name: 'calculator',
                input: {}
              },
              {
                id: `tool-${sessionId}-2`,
                type: 'tool_use',
                name: 'search',
                input: {}
              }
            ]
          };
          manager.onToolSelect(event);
        });

        // Verify all tools are active (4 sessions × 2 tools = 8 total)
        expect(manager.getActiveNotifications()).toHaveLength(8);
        sessions.forEach(sessionId => {
          expect(manager.getActiveNotificationsForSession(sessionId)).toHaveLength(2);
        });

        // Nuclear cleanup (user_turn_start trigger)
        manager.clearAllActiveNotifications();

        // Verify EVERYTHING is cleared
        expect(manager.getActiveNotifications()).toHaveLength(0);
        sessions.forEach(sessionId => {
          expect(manager.getActiveNotificationsForSession(sessionId)).toHaveLength(0);
          expect(manager.isToolActive(sessionId, `tool-${sessionId}-1`)).toBe(false);
          expect(manager.isToolActive(sessionId, `tool-${sessionId}-2`)).toBe(false);
        });
      });

      it('should clear parent, child, and unrelated sessions all together', () => {
        // Setup: Complex session hierarchy
        const parentEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'parent',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'parent',
          tool_calls: [
            {
              id: 'parent-tool',
              type: 'tool_use',
              name: 'parent_calc',
              input: {}
            }
          ]
        };
        manager.onToolSelect(parentEvent);

        const child1Event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'child-1',
          role: 'assistant',
          parent_session_id: 'parent',
          user_session_id: 'parent',
          tool_calls: [
            {
              id: 'child1-tool',
              type: 'tool_use',
              name: 'child1_calc',
              input: {}
            }
          ]
        };
        manager.onToolSelect(child1Event);

        const child2Event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'child-2',
          role: 'assistant',
          parent_session_id: 'parent',
          user_session_id: 'parent',
          tool_calls: [
            {
              id: 'child2-tool',
              type: 'tool_use',
              name: 'child2_calc',
              input: {}
            }
          ]
        };
        manager.onToolSelect(child2Event);

        const unrelatedEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'unrelated',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'unrelated',
          tool_calls: [
            {
              id: 'unrelated-tool',
              type: 'tool_use',
              name: 'unrelated_calc',
              input: {}
            }
          ]
        };
        manager.onToolSelect(unrelatedEvent);

        // Verify all active (4 sessions, 1 tool each)
        expect(manager.getActiveNotifications()).toHaveLength(4);

        // Nuclear cleanup
        manager.clearAllActiveNotifications();

        // Verify ALL cleared - parent, children, unrelated
        expect(manager.getActiveNotifications()).toHaveLength(0);
        expect(manager.isToolActive('parent', 'parent-tool')).toBe(false);
        expect(manager.isToolActive('child-1', 'child1-tool')).toBe(false);
        expect(manager.isToolActive('child-2', 'child2-tool')).toBe(false);
        expect(manager.isToolActive('unrelated', 'unrelated-tool')).toBe(false);
      });

      it('should handle nuclear cleanup with many sessions and many tools', () => {
        // Setup: Create 10 sessions with 5 tools each
        const sessionCount = 10;
        const toolsPerSession = 5;

        for (let i = 0; i < sessionCount; i++) {
          const sessionId = `session-${i}`;
          const toolCalls = [];
          for (let j = 0; j < toolsPerSession; j++) {
            toolCalls.push({
              id: `tool-${i}-${j}`,
              type: 'tool_use' as const,
              name: `tool_${j}`,
              input: {}
            });
          }

          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: toolCalls
          };
          manager.onToolSelect(event);
        }

        // Verify all tools are active (10 × 5 = 50 tools)
        expect(manager.getActiveNotifications()).toHaveLength(50);

        // Nuclear cleanup
        manager.clearAllActiveNotifications();

        // Verify EVERYTHING is cleared
        expect(manager.getActiveNotifications()).toHaveLength(0);
        for (let i = 0; i < sessionCount; i++) {
          expect(manager.getActiveNotificationsForSession(`session-${i}`)).toHaveLength(0);
        }
      });
    });

    describe('Edge Cases and Error Handling', () => {
      it('should handle multiple consecutive session cleanups without error', () => {
        const sessionId = 'test-session';
        
        // Create tool
        const event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: sessionId,
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: sessionId,
          tool_calls: [
            {
              id: 'test-tool',
              type: 'tool_use',
              name: 'calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(event);

        // Multiple cleanups should not error
        expect(() => manager.clearSessionNotifications(sessionId)).not.toThrow();
        expect(() => manager.clearSessionNotifications(sessionId)).not.toThrow();
        expect(() => manager.clearSessionNotifications(sessionId)).not.toThrow();

        // Verify still empty after multiple cleanups
        expect(manager.getActiveNotificationsForSession(sessionId)).toHaveLength(0);
      });

      it('should handle cleanup of session with no active notifications', () => {
        // Clear non-existent session should not error
        expect(() => manager.clearSessionNotifications('nonexistent-session')).not.toThrow();
        
        // Verify no side effects
        expect(manager.getActiveNotifications()).toHaveLength(0);
      });

      it('should handle multiple consecutive nuclear cleanups without error', () => {
        // Create some tools
        const event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'test-session',
          tool_calls: [
            {
              id: 'test-tool',
              type: 'tool_use',
              name: 'calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(event);

        // Multiple nuclear cleanups should not error
        expect(() => manager.clearAllActiveNotifications()).not.toThrow();
        expect(() => manager.clearAllActiveNotifications()).not.toThrow();
        expect(() => manager.clearAllActiveNotifications()).not.toThrow();

        // Verify still empty after multiple cleanups
        expect(manager.getActiveNotifications()).toHaveLength(0);
      });

      it('should handle nuclear cleanup with no active notifications', () => {
        // Nuclear cleanup on empty state should not error
        expect(() => manager.clearAllActiveNotifications()).not.toThrow();
        
        // Verify no side effects
        expect(manager.getActiveNotifications()).toHaveLength(0);
      });
    });

    describe('Cleanup State Verification', () => {
      it('should verify session cleanup removes all query methods', () => {
        const sessionId = 'test-session';
        const toolId = 'test-tool';

        // Create tool
        const event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: sessionId,
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: sessionId,
          tool_calls: [
            {
              id: toolId,
              type: 'tool_use',
              name: 'calculator',
              input: {}
            }
          ]
        };
        manager.onToolSelect(event);

        // Verify exists before cleanup
        expect(manager.isToolActive(sessionId, toolId)).toBe(true);
        expect(manager.getNotification(sessionId, toolId)).toBeDefined();
        expect(manager.getActiveNotificationsForSession(sessionId).length).toBeGreaterThan(0);
        expect(manager.getActiveNotifications().length).toBeGreaterThan(0);

        // Clear session
        manager.clearSessionNotifications(sessionId);

        // Verify ALL query methods return empty/false/undefined
        expect(manager.isToolActive(sessionId, toolId)).toBe(false);
        expect(manager.getNotification(sessionId, toolId)).toBeUndefined();
        expect(manager.getActiveNotificationsForSession(sessionId)).toHaveLength(0);
        expect(manager.getActiveNotifications()).toHaveLength(0);
        expect(manager.getActiveToolCount()).toBe(0);
      });

      it('should verify nuclear cleanup removes all query methods', () => {
        const sessions = ['session-1', 'session-2'];
        
        // Create tools for multiple sessions
        sessions.forEach(sessionId => {
          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: [
              {
                id: `tool-${sessionId}`,
                type: 'tool_use',
                name: 'calculator',
                input: {}
              }
            ]
          };
          manager.onToolSelect(event);
        });

        // Verify exists before cleanup
        sessions.forEach(sessionId => {
          expect(manager.getActiveNotificationsForSession(sessionId).length).toBeGreaterThan(0);
        });
        expect(manager.getActiveNotifications().length).toBeGreaterThan(0);

        // Nuclear cleanup
        manager.clearAllActiveNotifications();

        // Verify ALL query methods return empty for ALL sessions
        sessions.forEach(sessionId => {
          expect(manager.isToolActive(sessionId, `tool-${sessionId}`)).toBe(false);
          expect(manager.getNotification(sessionId, `tool-${sessionId}`)).toBeUndefined();
          expect(manager.getActiveNotificationsForSession(sessionId)).toHaveLength(0);
        });
        expect(manager.getActiveNotifications()).toHaveLength(0);
        expect(manager.getActiveToolCount()).toBe(0);
      });
    });
  });

  describe('Performance and Memory', () => {
    it('should handle many concurrent tools efficiently', () => {
      const startTime = performance.now();
      const toolCount = 100;

      // Create 100 tool selections across 10 sessions
      for (let i = 0; i < toolCount; i++) {
        const sessionId = `session-${i % 10}`;
        const event: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: sessionId,
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: sessionId,
          tool_calls: [
            {
              id: `tool-${i}`,
              type: 'tool_use',
              name: `tool_${i % 5}`,
              input: { index: i }
            }
          ]
        };
        manager.onToolSelect(event);
      }

      const elapsed = performance.now() - startTime;

      // Should complete in under 50ms
      expect(elapsed).toBeLessThan(50);

      // Verify correct count
      expect(manager.getActiveNotifications()).toHaveLength(toolCount);

      // Verify session isolation
      const session0Tools = manager.getActiveNotificationsForSession('session-0');
      expect(session0Tools).toHaveLength(10); // 100 tools / 10 sessions
    });

    it('should efficiently clear many notifications by session', () => {
      // Create tools for multiple sessions
      for (let sessionIdx = 0; sessionIdx < 5; sessionIdx++) {
        const sessionId = `session-${sessionIdx}`;
        for (let toolIdx = 0; toolIdx < 20; toolIdx++) {
          const event: ToolSelectDeltaEvent = {
            type: 'tool_select_delta',
            session_id: sessionId,
            role: 'assistant',
            parent_session_id: undefined,
            user_session_id: sessionId,
            tool_calls: [
              {
                id: `tool-${sessionIdx}-${toolIdx}`,
                type: 'tool_use',
                name: 'test_tool',
                input: {}
              }
            ]
          };
          manager.onToolSelect(event);
        }
      }

      expect(manager.getActiveNotifications()).toHaveLength(100);

      const startTime = performance.now();
      manager.clearSessionNotifications('session-2');
      const elapsed = performance.now() - startTime;

      // Should complete in under 5ms
      expect(elapsed).toBeLessThan(5);

      // Verify only session-2 was cleared
      expect(manager.getActiveNotifications()).toHaveLength(80);
      expect(manager.getActiveNotificationsForSession('session-2')).toHaveLength(0);
      expect(manager.getActiveNotificationsForSession('session-0')).toHaveLength(20);
    });

    it('should not leak memory with many tool completions', () => {
      // Complete many tools
      for (let i = 0; i < 1000; i++) {
        const event: ToolCallEvent = {
          type: 'tool_call',
          session_id: 'test-session',
          role: 'assistant',
          parent_session_id: undefined,
          user_session_id: 'test-session',
          active: false,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: `tool-${i}`,
              type: 'tool_use',
              name: 'test_tool',
              input: {}
            }
          ]
        };
        manager.onToolCallComplete(event);
      }

      // Verify completed tools are tracked
      expect(manager.getCompletedToolCalls()).toHaveLength(1000);

      // Clear completed
      manager.clearCompleted();

      // Verify memory is released
      expect(manager.getCompletedToolCalls()).toHaveLength(0);
    });
  });
});


/**
 * Test Helper: Create Orphaned Notification
 * 
 * Creates a tool notification that simulates an orphaned state (tool selected but never completed)
 * Useful for testing cleanup mechanisms in EventStreamProcessor
 * 
 * @param manager - ToolCallManager instance
 * @param sessionId - Session ID for the notification
 * @param toolName - Name of the tool
 * @param toolCallId - Optional tool call ID (defaults to generated ID)
 * @returns Object with notification ID and cleanup function
 */
export function createOrphanedNotification(
  manager: ToolCallManager,
  sessionId: string,
  toolName: string,
  toolCallId?: string
): { notificationId: string; cleanup: () => void } {
  const id = toolCallId || `orphaned-${Date.now()}-${Math.random()}`;
  
  // Create a tool_select_delta event to initialize the notification
  const selectEvent: ToolSelectDeltaEvent = {
    type: 'tool_select_delta',
    session_id: sessionId,
    role: 'assistant',
    parent_session_id: undefined,
    user_session_id: sessionId,
    tool_calls: [
      {
        id,
        type: 'tool_use',
        name: toolName,
        input: {}
      }
    ]
  };
  
  // Select the tool (creates notification)
  manager.onToolSelect(selectEvent);
  
  // Optionally mark it as executing to simulate mid-execution orphan
  const activeEvent: ToolCallEvent = {
    type: 'tool_call',
    session_id: sessionId,
    role: 'assistant',
    parent_session_id: undefined,
    user_session_id: sessionId,
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id,
        type: 'tool_use',
        name: toolName,
        input: {}
      }
    ]
  };
  manager.onToolCallActive(activeEvent);
  
  // Return notification ID and cleanup function
  return {
    notificationId: id,
    cleanup: () => {
      // Force cleanup by completing the tool
      const completeEvent: ToolCallEvent = {
        type: 'tool_call',
        session_id: sessionId,
        role: 'assistant',
        parent_session_id: undefined,
        user_session_id: sessionId,
        active: false,
        vendor: 'anthropic',
        tool_calls: [
          {
            id,
            type: 'tool_use',
            name: toolName,
            input: {}
          }
        ]
      };
      manager.onToolCallComplete(completeEvent);
    }
  };
}
