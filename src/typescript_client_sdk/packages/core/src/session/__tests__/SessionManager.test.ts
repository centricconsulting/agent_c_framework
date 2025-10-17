/**
 * SessionManager Tests
 * 
 * Comprehensive tests for SessionManager including Phase 3 tool call buffering
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ChatSessionManager } from '../SessionManager';
import type { ChatSession } from '../../events/types/CommonTypes';
import type { ToolCallWithResult } from '../SessionManager';
import {
  emptySession,
  userOnlySession,
  withAssistantMessages,
  multipleAssistantMessages,
  workspaceReadToolCall,
  workspaceReadToolCallIncomplete,
  multipleToolCalls,
  errorToolCall,
  thinkToolCall,
  createSessionWithAssistantMessages,
  createToolCall
} from '../../test/fixtures/session-fixtures';

describe('SessionManager - Phase 3 Tool Call Buffering', () => {
  let manager: ChatSessionManager;

  beforeEach(() => {
    manager = new ChatSessionManager();
  });

  describe('bufferPendingToolCalls', () => {
    it('should buffer tool calls for a session', () => {
      const sessionId = 'test-session';
      const toolCalls = [workspaceReadToolCall];

      manager.bufferPendingToolCalls(sessionId, toolCalls);

      const pending = manager.getPendingToolCalls(sessionId);
      expect(pending).toHaveLength(1);
      expect(pending[0]).toEqual(workspaceReadToolCall);
    });

    it('should buffer multiple tool calls', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);
      manager.bufferPendingToolCalls(sessionId, [errorToolCall]);
      manager.bufferPendingToolCalls(sessionId, [thinkToolCall]);

      const pending = manager.getPendingToolCalls(sessionId);
      expect(pending).toHaveLength(3);
      expect(pending.map(t => t.name)).toEqual(['workspace_read', 'workspace_read', 'think']);
    });

    it('should accumulate tool calls from multiple calls', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, multipleToolCalls.slice(0, 2));
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(2);

      manager.bufferPendingToolCalls(sessionId, multipleToolCalls.slice(2));
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(3);
    });

    it('should handle empty tool call arrays', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, []);
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(0);
    });

    it('should isolate buffered tools by session', () => {
      manager.bufferPendingToolCalls('session-1', [workspaceReadToolCall]);
      manager.bufferPendingToolCalls('session-2', [errorToolCall]);
      manager.bufferPendingToolCalls('session-3', [thinkToolCall]);

      expect(manager.getPendingToolCalls('session-1')).toHaveLength(1);
      expect(manager.getPendingToolCalls('session-2')).toHaveLength(1);
      expect(manager.getPendingToolCalls('session-3')).toHaveLength(1);

      expect(manager.getPendingToolCalls('session-1')[0].name).toBe('workspace_read');
      expect(manager.getPendingToolCalls('session-2')[0].name).toBe('workspace_read');
      expect(manager.getPendingToolCalls('session-3')[0].name).toBe('think');
    });

    it('should buffer tool calls with and without results', () => {
      const sessionId = 'test-session';
      const toolsWithMixedResults = [
        workspaceReadToolCall, // Has result
        workspaceReadToolCallIncomplete // No result
      ];

      manager.bufferPendingToolCalls(sessionId, toolsWithMixedResults);

      const pending = manager.getPendingToolCalls(sessionId);
      expect(pending).toHaveLength(2);
      expect(pending[0].result).toBeDefined();
      expect(pending[1].result).toBeUndefined();
    });
  });

  describe('getPendingToolCalls', () => {
    it('should return empty array for session with no pending tools', () => {
      const pending = manager.getPendingToolCalls('nonexistent-session');
      expect(pending).toEqual([]);
    });

    it('should return buffered tool calls in order', () => {
      const sessionId = 'test-session';
      const toolCalls = [
        createToolCall({ id: 'tool-1', name: 'first' }),
        createToolCall({ id: 'tool-2', name: 'second' }),
        createToolCall({ id: 'tool-3', name: 'third' })
      ];

      manager.bufferPendingToolCalls(sessionId, toolCalls);

      const pending = manager.getPendingToolCalls(sessionId);
      expect(pending.map(t => t.name)).toEqual(['first', 'second', 'third']);
    });

    it('should return copy of buffered tools (not modifiable)', () => {
      const sessionId = 'test-session';
      const toolCalls = [workspaceReadToolCall];

      manager.bufferPendingToolCalls(sessionId, toolCalls);

      const pending1 = manager.getPendingToolCalls(sessionId);
      const pending2 = manager.getPendingToolCalls(sessionId);

      // Should be equal but not same reference
      expect(pending1).toEqual(pending2);
      expect(pending1).not.toBe(pending2);
    });
  });

  describe('clearPendingToolCalls', () => {
    it('should clear pending tool calls for a session', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, multipleToolCalls);
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(3);

      manager.clearPendingToolCalls(sessionId);
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(0);
    });

    it('should only clear specified session', () => {
      manager.bufferPendingToolCalls('session-1', [workspaceReadToolCall]);
      manager.bufferPendingToolCalls('session-2', [errorToolCall]);
      manager.bufferPendingToolCalls('session-3', [thinkToolCall]);

      manager.clearPendingToolCalls('session-2');

      expect(manager.getPendingToolCalls('session-1')).toHaveLength(1);
      expect(manager.getPendingToolCalls('session-2')).toHaveLength(0);
      expect(manager.getPendingToolCalls('session-3')).toHaveLength(1);
    });

    it('should handle clearing non-existent session', () => {
      expect(() => manager.clearPendingToolCalls('nonexistent-session')).not.toThrow();
    });

    it('should handle multiple consecutive clears', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);

      manager.clearPendingToolCalls(sessionId);
      manager.clearPendingToolCalls(sessionId);
      manager.clearPendingToolCalls(sessionId);

      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(0);
    });
  });

  describe('hasPendingToolCalls', () => {
    it('should return true when session has pending tools', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);
      expect(manager.hasPendingToolCalls(sessionId)).toBe(true);
    });

    it('should return false when session has no pending tools', () => {
      expect(manager.hasPendingToolCalls('session-without-tools')).toBe(false);
    });

    it('should return false after clearing pending tools', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);
      expect(manager.hasPendingToolCalls(sessionId)).toBe(true);

      manager.clearPendingToolCalls(sessionId);
      expect(manager.hasPendingToolCalls(sessionId)).toBe(false);
    });

    it('should return false for empty tool call array', () => {
      const sessionId = 'test-session';

      manager.bufferPendingToolCalls(sessionId, []);
      expect(manager.hasPendingToolCalls(sessionId)).toBe(false);
    });
  });

  describe('Buffering Edge Cases', () => {
    it('should handle parent and child sessions independently', () => {
      const parentSession = 'parent-session';
      const childSession = 'child-session';

      manager.bufferPendingToolCalls(parentSession, [workspaceReadToolCall]);
      manager.bufferPendingToolCalls(childSession, [errorToolCall, thinkToolCall]);

      expect(manager.getPendingToolCalls(parentSession)).toHaveLength(1);
      expect(manager.getPendingToolCalls(childSession)).toHaveLength(2);

      manager.clearPendingToolCalls(childSession);

      expect(manager.getPendingToolCalls(parentSession)).toHaveLength(1);
      expect(manager.getPendingToolCalls(childSession)).toHaveLength(0);
    });

    it('should handle many sessions with buffered tools', () => {
      const sessionCount = 50;

      for (let i = 0; i < sessionCount; i++) {
        const sessionId = `session-${i}`;
        manager.bufferPendingToolCalls(sessionId, [
          createToolCall({ id: `tool-${i}-1` }),
          createToolCall({ id: `tool-${i}-2` })
        ]);
      }

      // Verify all sessions have buffered tools
      for (let i = 0; i < sessionCount; i++) {
        const sessionId = `session-${i}`;
        expect(manager.hasPendingToolCalls(sessionId)).toBe(true);
        expect(manager.getPendingToolCalls(sessionId)).toHaveLength(2);
      }

      // Clear half the sessions
      for (let i = 0; i < sessionCount / 2; i++) {
        manager.clearPendingToolCalls(`session-${i}`);
      }

      // Verify cleared sessions
      for (let i = 0; i < sessionCount / 2; i++) {
        expect(manager.hasPendingToolCalls(`session-${i}`)).toBe(false);
      }

      // Verify remaining sessions unaffected
      for (let i = sessionCount / 2; i < sessionCount; i++) {
        expect(manager.hasPendingToolCalls(`session-${i}`)).toBe(true);
        expect(manager.getPendingToolCalls(`session-${i}`)).toHaveLength(2);
      }
    });

    it('should handle large number of tools in single session', () => {
      const sessionId = 'test-session';
      const toolCount = 100;
      const manyTools = Array.from({ length: toolCount }, (_, i) =>
        createToolCall({ id: `tool-${i}`, name: `tool_${i}` })
      );

      manager.bufferPendingToolCalls(sessionId, manyTools);

      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(toolCount);
      expect(manager.hasPendingToolCalls(sessionId)).toBe(true);
    });
  });

  describe('Integration with Session Lifecycle', () => {
    it('should maintain buffered tools when session is set as current', () => {
      const sessionId = 'test-session';
      const session: ChatSession = {
        ...emptySession,
        session_id: sessionId
      };

      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);
      manager.setCurrentSession(session);

      expect(manager.hasPendingToolCalls(sessionId)).toBe(true);
      expect(manager.getPendingToolCalls(sessionId)).toHaveLength(1);
    });

    it('should maintain buffered tools across multiple sessions', () => {
      const session1: ChatSession = {
        ...emptySession,
        session_id: 'session-1'
      };
      const session2: ChatSession = {
        ...emptySession,
        session_id: 'session-2'
      };

      // Buffer tools for session 1
      manager.bufferPendingToolCalls('session-1', [workspaceReadToolCall]);
      manager.setCurrentSession(session1);

      expect(manager.hasPendingToolCalls('session-1')).toBe(true);

      // Switch to session 2 and buffer tools
      manager.bufferPendingToolCalls('session-2', [errorToolCall]);
      manager.setCurrentSession(session2);

      // Both sessions should maintain their buffered tools
      expect(manager.hasPendingToolCalls('session-1')).toBe(true);
      expect(manager.hasPendingToolCalls('session-2')).toBe(true);
      expect(manager.getPendingToolCalls('session-1')[0].name).toBe('workspace_read');
      expect(manager.getPendingToolCalls('session-2')[0].name).toBe('workspace_read');
    });

    it('should NOT auto-clear buffered tools when session is cleared', () => {
      const sessionId = 'test-session';
      const session: ChatSession = {
        ...emptySession,
        session_id: sessionId
      };

      manager.setCurrentSession(session);
      manager.bufferPendingToolCalls(sessionId, [workspaceReadToolCall]);

      manager.clearSession(sessionId);

      // Buffered tools persist even after session is cleared
      // (EventStreamProcessor is responsible for clearing them)
      expect(manager.hasPendingToolCalls(sessionId)).toBe(true);
    });

    it('should NOT auto-clear buffered tools on reset', () => {
      manager.bufferPendingToolCalls('session-1', [workspaceReadToolCall]);
      manager.bufferPendingToolCalls('session-2', [errorToolCall]);

      manager.reset();

      // Buffered tools persist even after reset
      // (EventStreamProcessor is responsible for cleanup)
      expect(manager.hasPendingToolCalls('session-1')).toBe(true);
      expect(manager.hasPendingToolCalls('session-2')).toBe(true);
    });
  });

  describe('Performance', () => {
    it('should handle rapid buffering and clearing efficiently', () => {
      const sessionId = 'perf-session';
      const iterations = 1000;
      const startTime = performance.now();

      for (let i = 0; i < iterations; i++) {
        manager.bufferPendingToolCalls(sessionId, [
          createToolCall({ id: `tool-${i}` })
        ]);
        
        if (i % 10 === 0) {
          manager.clearPendingToolCalls(sessionId);
        }
      }

      const elapsed = performance.now() - startTime;

      // Should complete in under 100ms
      expect(elapsed).toBeLessThan(100);
    });

    it('should handle many concurrent sessions efficiently', () => {
      const sessionCount = 100;
      const startTime = performance.now();

      for (let i = 0; i < sessionCount; i++) {
        manager.bufferPendingToolCalls(`session-${i}`, [
          createToolCall({ id: `tool-${i}-1` }),
          createToolCall({ id: `tool-${i}-2` }),
          createToolCall({ id: `tool-${i}-3` })
        ]);
      }

      const elapsed = performance.now() - startTime;

      // Should complete in under 50ms
      expect(elapsed).toBeLessThan(50);

      // Verify all sessions have buffered tools
      for (let i = 0; i < sessionCount; i++) {
        expect(manager.getPendingToolCalls(`session-${i}`)).toHaveLength(3);
      }
    });
  });
});

describe('SessionManager - Basic Functionality', () => {
  let manager: ChatSessionManager;

  beforeEach(() => {
    manager = new ChatSessionManager();
  });

  describe('Session Management', () => {
    it('should set and get current session', () => {
      manager.setCurrentSession(withAssistantMessages);

      const current = manager.getCurrentSession();
      expect(current).not.toBeNull();
      expect(current?.session_id).toBe('with-assistant-session');
    });

    it('should return null for no current session', () => {
      expect(manager.getCurrentSession()).toBeNull();
      expect(manager.getCurrentSessionId()).toBeNull();
    });

    it('should get session by ID', () => {
      manager.setCurrentSession(withAssistantMessages);

      const session = manager.getSession('with-assistant-session');
      expect(session).not.toBeNull();
      expect(session?.session_name).toBe('Session With Assistant');
    });

    it('should emit chat-session-changed event', () => {
      const listener = vi.fn();
      manager.on('chat-session-changed', listener);

      manager.setCurrentSession(withAssistantMessages);

      expect(listener).toHaveBeenCalledWith({
        previousChatSession: null,
        currentChatSession: withAssistantMessages
      });
    });

    it('should clear specific session', () => {
      manager.setCurrentSession(withAssistantMessages);
      const cleared = manager.clearSession('with-assistant-session');

      expect(cleared).toBe(true);
      expect(manager.getCurrentSession()).toBeNull();
    });

    it('should reset all sessions', () => {
      manager.setCurrentSession(withAssistantMessages);
      manager.reset();

      expect(manager.getCurrentSession()).toBeNull();
      expect(manager.getAllSessions().size).toBe(0);
    });
  });
});
