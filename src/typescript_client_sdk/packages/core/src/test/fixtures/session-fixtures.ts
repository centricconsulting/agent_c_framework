/**
 * Session Test Fixtures
 * 
 * Provides reusable test data for SessionManager and EventStreamProcessor testing,
 * particularly for Phase 3 tool attachment validation
 */

import type { ChatSession, Message, ToolResult } from '../../events/types/CommonTypes';
import type { ToolCallWithResult } from '../../session/SessionManager';

// ============================================================================
// Session Fixtures (Factory Functions for Test Isolation)
// ============================================================================

/**
 * Empty session with no messages
 * Factory function ensures each test gets a fresh copy
 */
export function createEmptySession(): ChatSession {
  return {
    session_id: 'empty-session',
    session_name: 'Empty Test Session',
    created_at: '2024-01-01T00:00:00.000Z',
    updated_at: '2024-01-01T00:00:00.000Z',
    messages: [],
    token_count: 0,
    metadata: {}
  };
}

/**
 * Session with only user messages
 * Factory function ensures each test gets a fresh copy
 */
export function createUserOnlySession(): ChatSession {
  return {
    session_id: 'user-only-session',
    session_name: 'User Messages Only',
    created_at: '2024-01-01T00:00:00.000Z',
    updated_at: '2024-01-01T00:01:00.000Z',
    messages: [
      {
        role: 'user',
        content: 'Hello, how are you?',
        timestamp: '2024-01-01T00:00:30.000Z',
        format: 'text'
      },
      {
        role: 'user',
        content: 'Please help me with something.',
        timestamp: '2024-01-01T00:01:00.000Z',
        format: 'text'
      }
    ],
    token_count: 20,
    metadata: {}
  };
}

/**
 * Session with assistant messages (for backward attachment)
 * Factory function ensures each test gets a fresh copy
 */
export function createWithAssistantMessages(): ChatSession {
  return {
    session_id: 'with-assistant-session',
    session_name: 'Session With Assistant',
    created_at: '2024-01-01T00:00:00.000Z',
    updated_at: '2024-01-01T00:02:00.000Z',
    messages: [
      {
        id: 'msg-user-1',
        role: 'user',
        content: 'Read the package.json file',
        timestamp: '2024-01-01T00:00:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-assistant-1',
        role: 'assistant',
        content: "I'll read the package.json file for you.",
        timestamp: '2024-01-01T00:00:30.000Z',
        format: 'text',
        type: undefined, // Regular message
        metadata: {
          finishReason: 'stop',
          model: 'claude-3-5-sonnet'
        }
      },
      {
        id: 'msg-user-2',
        role: 'user',
        content: 'Now check the README',
        timestamp: '2024-01-01T00:01:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-assistant-2',
        role: 'assistant',
        content: "I'll check the README file.",
        timestamp: '2024-01-01T00:01:30.000Z',
        format: 'text',
        type: undefined,
        metadata: {
          finishReason: 'stop',
          model: 'claude-3-5-sonnet'
        }
      }
    ],
    token_count: 100,
    metadata: {}
  };
}

/**
 * Session with multiple assistant messages (for testing last message selection)
 * Factory function ensures each test gets a fresh copy
 */
export function createMultipleAssistantMessages(): ChatSession {
  return {
    session_id: 'multiple-assistant-session',
    session_name: 'Multiple Assistant Messages',
    created_at: '2024-01-01T00:00:00.000Z',
    updated_at: '2024-01-01T00:05:00.000Z',
    messages: [
      {
        id: 'msg-1',
        role: 'user',
        content: 'First question',
        timestamp: '2024-01-01T00:00:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-2',
        role: 'assistant',
        content: 'First response',
        timestamp: '2024-01-01T00:01:00.000Z',
        format: 'text',
        type: undefined
      },
      {
        id: 'msg-3',
        role: 'user',
        content: 'Second question',
        timestamp: '2024-01-01T00:02:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-4',
        role: 'assistant',
        content: 'Second response',
        timestamp: '2024-01-01T00:03:00.000Z',
        format: 'text',
        type: undefined
      },
      {
        id: 'msg-5',
        role: 'user',
        content: 'Third question',
        timestamp: '2024-01-01T00:04:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-6',
        role: 'assistant',
        content: 'Third response (most recent)',
        timestamp: '2024-01-01T00:05:00.000Z',
        format: 'text',
        type: undefined
      }
    ],
    token_count: 200,
    metadata: {}
  };
}

/**
 * Session with thought message (should NOT be eligible for tool attachment)
 * Factory function ensures each test gets a fresh copy
 */
export function createSessionWithThought(): ChatSession {
  return {
    session_id: 'thought-session',
    session_name: 'Session With Thought',
    created_at: '2024-01-01T00:00:00.000Z',
    updated_at: '2024-01-01T00:01:00.000Z',
    messages: [
      {
        id: 'msg-user-1',
        role: 'user',
        content: 'Help me think about this',
        timestamp: '2024-01-01T00:00:00.000Z',
        format: 'text'
      },
      {
        id: 'msg-thought-1',
        role: 'assistant',
        content: 'Let me consider this carefully...',
        timestamp: '2024-01-01T00:00:30.000Z',
        format: 'text',
        type: 'thought', // NOT eligible for tool attachment
        metadata: {
          finishReason: 'stop'
        }
      }
    ],
    token_count: 50,
    metadata: {}
  };
}

// ============================================================================
// Tool Call Fixtures (Factory Functions for Test Isolation)
// ============================================================================

/**
 * Single workspace_read tool call with result
 * Factory function ensures each test gets a fresh copy
 */
export function createWorkspaceReadToolCall(): ToolCallWithResult {
  return {
    id: 'tool-workspace-read-1',
    type: 'tool_use',
    name: 'workspace_read',
    input: {
      path: '//WORKSPACE/package.json'
    },
    result: {
      type: 'tool_result',
      tool_use_id: 'tool-workspace-read-1',
      content: JSON.stringify({
        name: '@agentc/realtime-core',
        version: '1.0.0'
      })
    }
  };
}

/**
 * workspace_read tool call WITHOUT result (incomplete)
 * Factory function ensures each test gets a fresh copy
 */
export function createWorkspaceReadToolCallIncomplete(): ToolCallWithResult {
  return {
    id: 'tool-workspace-read-incomplete',
    type: 'tool_use',
    name: 'workspace_read',
    input: {
      path: '//WORKSPACE/README.md'
    }
    // No result - tool is still executing or failed
  };
}

/**
 * Multiple tool calls for complex scenarios
 * Factory function ensures each test gets a fresh copy
 */
export function createMultipleToolCalls(): ToolCallWithResult[] {
  return [
    {
      id: 'tool-grep-1',
      type: 'tool_use',
      name: 'workspace_grep',
      input: {
        paths: ['//WORKSPACE/src/**/*.ts'],
        pattern: 'EventStreamProcessor'
      },
      result: {
        type: 'tool_result',
        tool_use_id: 'tool-grep-1',
        content: 'Found 15 matches'
      }
    },
    {
      id: 'tool-read-2',
      type: 'tool_use',
      name: 'workspace_read',
      input: {
        path: '//WORKSPACE/src/events/EventStreamProcessor.ts'
      },
      result: {
        type: 'tool_result',
        tool_use_id: 'tool-read-2',
        content: '// File contents...'
      }
    },
    {
      id: 'tool-tree-3',
      type: 'tool_use',
      name: 'workspace_tree',
      input: {
        path: '//WORKSPACE/src',
        folder_depth: 2
      },
      result: {
        type: 'tool_result',
        tool_use_id: 'tool-tree-3',
        content: 'src/\n  events/\n  session/'
      }
    }
  ];
}

/**
 * Tool call that resulted in an error
 * Factory function ensures each test gets a fresh copy
 */
export function createErrorToolCall(): ToolCallWithResult {
  return {
    id: 'tool-error-1',
    type: 'tool_use',
    name: 'workspace_read',
    input: {
      path: '//WORKSPACE/nonexistent.txt'
    },
    result: {
      type: 'tool_result',
      tool_use_id: 'tool-error-1',
      content: 'Error: File not found',
      is_error: true
    }
  };
}

/**
 * think tool call (special case - internal reasoning)
 * Factory function ensures each test gets a fresh copy
 */
export function createThinkToolCall(): ToolCallWithResult {
  return {
    id: 'tool-think-1',
    type: 'tool_use',
    name: 'think',
    input: {
      thought: 'I need to carefully analyze this request before proceeding...'
    },
    result: {
      type: 'tool_result',
      tool_use_id: 'tool-think-1',
      content: 'Thought process complete'
    }
  };
}

// ============================================================================
// Message Fixtures (Factory Functions for Test Isolation)
// ============================================================================

/**
 * Assistant message WITHOUT tools (before attachment)
 * Factory function ensures each test gets a fresh copy
 */
export function createAssistantMessageWithoutTools(): Message {
  return {
    id: 'msg-before-tools',
    role: 'assistant',
    content: "I'll help you with that task.",
    timestamp: '2024-01-01T00:00:00.000Z',
    format: 'text',
    type: undefined,
    metadata: {
      finishReason: 'stop',
      model: 'claude-3-5-sonnet'
    }
  };
}

/**
 * Assistant message WITH tools (after attachment)
 * Factory function ensures each test gets a fresh copy
 */
export function createAssistantMessageWithTools(): Message {
  return {
    id: 'msg-with-tools',
    role: 'assistant',
    content: "I've completed the file read operation.",
    timestamp: '2024-01-01T00:00:00.000Z',
    format: 'text',
    type: undefined,
    metadata: {
      finishReason: 'stop',
      model: 'claude-3-5-sonnet',
      toolCalls: [createWorkspaceReadToolCall()]
    }
  };
}

/**
 * Thought message (should NOT have tools attached)
 * Factory function ensures each test gets a fresh copy
 */
export function createThoughtMessage(): Message {
  return {
    id: 'msg-thought',
    role: 'assistant',
    content: 'Let me think about this carefully...',
    timestamp: '2024-01-01T00:00:00.000Z',
    format: 'text',
    type: 'thought',
    metadata: {
      finishReason: 'stop'
    }
  };
}

// ============================================================================
// Test Scenario Helpers
// ============================================================================

/**
 * Create a session with a specific number of assistant messages
 * Useful for testing "find last assistant message" logic
 */
export function createSessionWithAssistantMessages(count: number): ChatSession {
  const messages: Message[] = [];
  
  for (let i = 0; i < count; i++) {
    messages.push({
      id: `msg-user-${i}`,
      role: 'user',
      content: `User message ${i}`,
      timestamp: new Date(2024, 0, 1, 0, i * 2, 0).toISOString(),
      format: 'text'
    });
    
    messages.push({
      id: `msg-assistant-${i}`,
      role: 'assistant',
      content: `Assistant response ${i}`,
      timestamp: new Date(2024, 0, 1, 0, i * 2 + 1, 0).toISOString(),
      format: 'text',
      type: undefined,
      metadata: {
        finishReason: 'stop'
      }
    });
  }
  
  return {
    session_id: `test-session-${count}-messages`,
    session_name: `Test Session with ${count} Messages`,
    created_at: messages[0].timestamp,
    updated_at: messages[messages.length - 1].timestamp,
    messages,
    token_count: count * 50,
    metadata: {}
  };
}

/**
 * Create a tool call with custom properties
 */
export function createToolCall(
  overrides?: Partial<ToolCallWithResult>
): ToolCallWithResult {
  return {
    id: `tool-${Date.now()}-${Math.random()}`,
    type: 'tool_use',
    name: 'workspace_read',
    input: { path: '//WORKSPACE/test.txt' },
    result: {
      type: 'tool_result',
      tool_use_id: overrides?.id || 'tool-default',
      content: 'Test content'
    },
    ...overrides
  };
}
