/**
 * Integration tests for AgentConfigurationV2 security fields
 * 
 * Tests how blocked_tool_patterns and allowed_tool_patterns work
 * throughout the system for tool access control.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { 
  AgentConfigurationV2,
  AgentConfiguration,
  ChatSession 
} from '../chat-session';
import { EventEmitter } from '../../events/EventEmitter';

// Mock tool executor for testing
class ToolExecutor {
  private config: AgentConfigurationV2;
  private executionLog: Array<{ tool: string; allowed: boolean; reason?: string }> = [];

  constructor(config: AgentConfigurationV2) {
    this.config = config;
  }

  canExecuteTool(toolName: string): boolean {
    const allowed = this.isToolAllowed(toolName);
    const reason = this.getBlockReason(toolName);
    
    this.executionLog.push({ tool: toolName, allowed, reason });
    return allowed;
  }

  private isToolAllowed(toolName: string): boolean {
    // Check blocked patterns first (highest priority)
    if (this.config.blocked_tool_patterns) {
      for (const pattern of this.config.blocked_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          return false; // Blocked
        }
      }
    }

    // Check allowed patterns
    if (this.config.allowed_tool_patterns && this.config.allowed_tool_patterns.length > 0) {
      for (const pattern of this.config.allowed_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          return true; // Explicitly allowed
        }
      }
      return false; // Not in allow list
    }

    // No restrictions or empty allow list means allow all
    return true;
  }

  private matchesPattern(toolName: string, pattern: string): boolean {
    // Convert glob pattern to regex
    const regexPattern = pattern
      .replace(/[.+^${}()|[\]\\]/g, '\\$&') // Escape regex special chars
      .replace(/\*/g, '.*')  // * matches any characters
      .replace(/\?/g, '.');  // ? matches single character
    
    const regex = new RegExp(`^${regexPattern}$`);
    return regex.test(toolName);
  }

  private getBlockReason(toolName: string): string | undefined {
    if (this.config.blocked_tool_patterns) {
      for (const pattern of this.config.blocked_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          return `Blocked by pattern: ${pattern}`;
        }
      }
    }

    if (this.config.allowed_tool_patterns && this.config.allowed_tool_patterns.length > 0) {
      let matchesAny = false;
      for (const pattern of this.config.allowed_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          matchesAny = true;
          break;
        }
      }
      if (!matchesAny) {
        return 'Not in allowed list';
      }
    }

    return undefined;
  }

  getExecutionLog() {
    return this.executionLog;
  }

  clearLog() {
    this.executionLog = [];
  }

  executeTool(toolName: string, args: any): { success: boolean; result?: any; error?: string } {
    if (!this.canExecuteTool(toolName)) {
      return {
        success: false,
        error: this.getBlockReason(toolName) || 'Tool execution blocked'
      };
    }

    // Simulate tool execution
    return {
      success: true,
      result: `Executed ${toolName} with args: ${JSON.stringify(args)}`
    };
  }
}

describe('AgentConfigurationV2 Security Integration', () => {
  let emitter: EventEmitter<Record<string, any>>;

  beforeEach(() => {
    vi.clearAllMocks();
    emitter = new EventEmitter();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Tool Executor with Security Patterns', () => {
    it('should enforce blocked patterns in tool execution', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'SecureExecutor',
        key: 'secure_executor',
        model_id: 'gpt-4',
        tools: ['read_file', 'write_file', 'delete_file', 'admin_panel'],
        blocked_tool_patterns: ['delete_*', 'admin_*', '*_dangerous'],
        persona: 'Secure assistant',
        category: ['secure']
      };

      const executor = new ToolExecutor(config);

      // Act
      const results = [
        { tool: 'read_file', expected: true },
        { tool: 'write_file', expected: true },
        { tool: 'delete_file', expected: false },
        { tool: 'delete_user', expected: false },
        { tool: 'admin_panel', expected: false },
        { tool: 'admin_settings', expected: false },
        { tool: 'tool_dangerous', expected: false },
        { tool: 'safe_tool', expected: true }
      ];

      const execResults = results.map(({ tool, expected }) => ({
        tool,
        expected,
        actual: executor.canExecuteTool(tool)
      }));

      // Assert
      execResults.forEach(({ tool, expected, actual }) => {
        expect(actual).toBe(expected);
      });

      // Check execution log
      const log = executor.getExecutionLog();
      expect(log).toHaveLength(results.length);
      
      const blockedEntries = log.filter(entry => !entry.allowed);
      expect(blockedEntries).toHaveLength(5);
      blockedEntries.forEach(entry => {
        expect(entry.reason).toContain('Blocked by pattern');
      });
    });

    it('should enforce allowed patterns in tool execution', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'AllowListExecutor',
        key: 'allowlist_executor',
        model_id: 'gpt-4',
        tools: [],
        allowed_tool_patterns: ['read_*', 'list_*', 'get_*', 'search_*'],
        persona: 'Restricted assistant',
        category: ['restricted']
      };

      const executor = new ToolExecutor(config);

      // Act
      const results = [
        { tool: 'read_file', expected: true },
        { tool: 'read_database', expected: true },
        { tool: 'list_users', expected: true },
        { tool: 'get_status', expected: true },
        { tool: 'search_documents', expected: true },
        { tool: 'write_file', expected: false },
        { tool: 'delete_user', expected: false },
        { tool: 'admin_panel', expected: false },
        { tool: 'execute_command', expected: false }
      ];

      const execResults = results.map(({ tool, expected }) => ({
        tool,
        expected,
        actual: executor.canExecuteTool(tool)
      }));

      // Assert
      execResults.forEach(({ tool, expected, actual }) => {
        expect(actual).toBe(expected);
      });

      // Check that non-allowed tools have proper reason
      const log = executor.getExecutionLog();
      const deniedEntries = log.filter(entry => !entry.allowed);
      deniedEntries.forEach(entry => {
        expect(entry.reason).toBe('Not in allowed list');
      });
    });

    it('should handle pattern precedence correctly', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'PrecedenceExecutor',
        key: 'precedence_executor',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['admin_*', '*_delete', 'dangerous_*'],
        allowed_tool_patterns: ['*'], // Allow all except blocked
        persona: 'Testing precedence',
        category: ['test']
      };

      const executor = new ToolExecutor(config);

      // Act & Assert
      // Blocked patterns take precedence over allow all
      expect(executor.canExecuteTool('admin_read')).toBe(false);
      expect(executor.canExecuteTool('user_delete')).toBe(false);
      expect(executor.canExecuteTool('dangerous_operation')).toBe(false);
      
      // Not blocked, so allowed by '*'
      expect(executor.canExecuteTool('safe_operation')).toBe(true);
      expect(executor.canExecuteTool('read_file')).toBe(true);
    });

    it('should handle actual tool execution with security', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'ExecutionTestAgent',
        key: 'execution_test',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['dangerous_*', 'admin_*'],
        allowed_tool_patterns: ['safe_*', 'read_*'],
        persona: 'Execution test',
        category: ['test']
      };

      const executor = new ToolExecutor(config);

      // Act
      const safeResult = executor.executeTool('safe_operation', { param: 'value' });
      const readResult = executor.executeTool('read_file', { path: '/tmp/test' });
      const dangerousResult = executor.executeTool('dangerous_delete', { target: 'system' });
      const unauthorizedResult = executor.executeTool('write_file', { path: '/etc/passwd' });

      // Assert
      expect(safeResult.success).toBe(true);
      expect(safeResult.result).toContain('safe_operation');

      expect(readResult.success).toBe(true);
      expect(readResult.result).toContain('read_file');

      expect(dangerousResult.success).toBe(false);
      expect(dangerousResult.error).toContain('Blocked by pattern: dangerous_*');

      expect(unauthorizedResult.success).toBe(false);
      expect(unauthorizedResult.error).toBe('Not in allowed list');
    });
  });

  describe('Event-Based Security Integration', () => {
    it('should emit security events when tools are blocked', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'EventSecurityAgent',
        key: 'event_security',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['blocked_*'],
        persona: 'Event test',
        category: ['test']
      };

      const securityEvents: any[] = [];
      emitter.on('security:tool_blocked', (event) => {
        securityEvents.push(event);
      });

      class SecurityAwareExecutor extends ToolExecutor {
        constructor(config: AgentConfigurationV2, private emitter: EventEmitter<any>) {
          super(config);
        }

        canExecuteTool(toolName: string): boolean {
          const allowed = super.canExecuteTool(toolName);
          
          if (!allowed) {
            this.emitter.emit('security:tool_blocked', {
              tool: toolName,
              agent: this['config'].name,
              timestamp: Date.now()
            });
          }
          
          return allowed;
        }
      }

      const executor = new SecurityAwareExecutor(config, emitter);

      // Act
      executor.canExecuteTool('allowed_tool');
      executor.canExecuteTool('blocked_tool');
      executor.canExecuteTool('blocked_admin');

      // Assert
      expect(securityEvents).toHaveLength(2);
      expect(securityEvents[0].tool).toBe('blocked_tool');
      expect(securityEvents[1].tool).toBe('blocked_admin');
      expect(securityEvents[0].agent).toBe('EventSecurityAgent');
    });

    it('should handle configuration updates through events', () => {
      // Arrange
      let currentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'DynamicSecurityAgent',
        key: 'dynamic_security',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['initial_blocked_*'],
        persona: 'Dynamic security',
        category: ['dynamic']
      };

      emitter.on('config:update_security', (update: Partial<AgentConfigurationV2>) => {
        if (update.blocked_tool_patterns) {
          currentConfig.blocked_tool_patterns = update.blocked_tool_patterns;
        }
        if (update.allowed_tool_patterns) {
          currentConfig.allowed_tool_patterns = update.allowed_tool_patterns;
        }
      });

      // Act - Initial state
      let executor = new ToolExecutor(currentConfig);
      const initialCheck = executor.canExecuteTool('initial_blocked_tool');

      // Update configuration
      emitter.emit('config:update_security', {
        blocked_tool_patterns: ['updated_blocked_*'],
        allowed_tool_patterns: ['safe_*']
      });

      // Create new executor with updated config
      executor = new ToolExecutor(currentConfig);
      const afterUpdateCheck1 = executor.canExecuteTool('initial_blocked_tool');
      const afterUpdateCheck2 = executor.canExecuteTool('updated_blocked_tool');
      const afterUpdateCheck3 = executor.canExecuteTool('safe_tool');

      // Assert
      expect(initialCheck).toBe(false); // Initially blocked
      expect(afterUpdateCheck1).toBe(false); // Not in allowed list after update
      expect(afterUpdateCheck2).toBe(false); // Blocked by new pattern
      expect(afterUpdateCheck3).toBe(true); // Allowed by new pattern
    });
  });

  describe('WebSocket Configuration Transmission', () => {
    it('should transmit security configuration over simulated WebSocket', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'WebSocketSecurityAgent',
        key: 'ws_security',
        model_id: 'gpt-4',
        tools: ['tool1', 'tool2'],
        blocked_tool_patterns: ['ws_blocked_*', 'remote_admin_*'],
        allowed_tool_patterns: ['ws_safe_*', 'public_*'],
        persona: 'WebSocket test',
        category: ['network']
      };

      class MockWebSocket {
        private handlers: Map<string, Function[]> = new Map();
        
        send(data: string) {
          // Simulate receiving on the other end
          const parsed = JSON.parse(data);
          this.trigger('message', { data: JSON.stringify(parsed) });
        }

        on(event: string, handler: Function) {
          if (!this.handlers.has(event)) {
            this.handlers.set(event, []);
          }
          this.handlers.get(event)!.push(handler);
        }

        trigger(event: string, data: any) {
          const handlers = this.handlers.get(event) || [];
          handlers.forEach(handler => handler(data));
        }
      }

      const ws = new MockWebSocket();
      let receivedConfig: AgentConfigurationV2 | null = null;

      ws.on('message', (event: { data: string }) => {
        const data = JSON.parse(event.data);
        if (data.version === 2) {
          receivedConfig = data as AgentConfigurationV2;
        }
      });

      // Act
      ws.send(JSON.stringify(config));

      // Assert
      expect(receivedConfig).not.toBeNull();
      expect(receivedConfig!.blocked_tool_patterns).toEqual(config.blocked_tool_patterns);
      expect(receivedConfig!.allowed_tool_patterns).toEqual(config.allowed_tool_patterns);
      expect(receivedConfig!.version).toBe(2);
      expect(receivedConfig!.name).toBe('WebSocketSecurityAgent');
    });

    it('should handle configuration sync between client and server', () => {
      // Arrange
      interface ConfigSyncMessage {
        type: 'config_sync';
        config: AgentConfigurationV2;
      }

      const serverConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'ServerAgent',
        key: 'server_agent',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['server_blocked_*', 'admin_*'],
        allowed_tool_patterns: ['server_allowed_*'],
        persona: 'Server agent',
        category: ['server']
      };

      let clientConfig: AgentConfigurationV2 | null = null;
      let syncCompleted = false;

      // Simulate server sending config
      emitter.on('server:config_sync', (message: ConfigSyncMessage) => {
        clientConfig = message.config;
        
        // Validate security fields are present
        if (clientConfig.blocked_tool_patterns && clientConfig.allowed_tool_patterns !== undefined) {
          syncCompleted = true;
          emitter.emit('client:config_sync_ack', { received: true });
        }
      });

      // Act
      emitter.emit('server:config_sync', {
        type: 'config_sync',
        config: serverConfig
      });

      // Assert
      expect(syncCompleted).toBe(true);
      expect(clientConfig).not.toBeNull();
      expect(clientConfig!.blocked_tool_patterns).toEqual(serverConfig.blocked_tool_patterns);
      expect(clientConfig!.allowed_tool_patterns).toEqual(serverConfig.allowed_tool_patterns);
    });
  });

  describe('Security Patterns in Chat Sessions', () => {
    it('should apply agent security config to chat session', () => {
      // Arrange
      const agentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'ChatAgent',
        key: 'chat_agent',
        model_id: 'gpt-4',
        tools: ['chat_tool'],
        blocked_tool_patterns: ['chat_delete_*', 'chat_admin_*'],
        allowed_tool_patterns: ['chat_read_*', 'chat_send_*'],
        persona: 'Chat assistant',
        category: ['chat']
      };

      const chatSession: Partial<ChatSession> = {
        session_id: 'session_123',
        agent_config: agentConfig,
        vendor: 'openai'
      };

      // Function to check if tool is allowed in session context
      const isToolAllowedInSession = (toolName: string, session: Partial<ChatSession>): boolean => {
        if (!session.agent_config || session.agent_config.version !== 2) {
          return true; // No V2 config, allow by default
        }

        const config = session.agent_config as AgentConfigurationV2;
        const executor = new ToolExecutor(config);
        return executor.canExecuteTool(toolName);
      };

      // Act & Assert
      expect(isToolAllowedInSession('chat_read_messages', chatSession)).toBe(true);
      expect(isToolAllowedInSession('chat_send_message', chatSession)).toBe(true);
      expect(isToolAllowedInSession('chat_delete_history', chatSession)).toBe(false);
      expect(isToolAllowedInSession('chat_admin_panel', chatSession)).toBe(false);
      expect(isToolAllowedInSession('random_tool', chatSession)).toBe(false); // Not in allow list
    });

    it('should handle security config changes during session', () => {
      // Arrange
      const initialConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'MutableAgent',
        key: 'mutable_agent',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['initial_blocked_*'],
        allowed_tool_patterns: undefined, // Initially allow all except blocked
        persona: 'Mutable assistant',
        category: ['mutable']
      };

      const session: Partial<ChatSession> = {
        session_id: 'mutable_session',
        agent_config: initialConfig,
        vendor: 'anthropic'
      };

      // Act - Check initial state
      let executor = new ToolExecutor(session.agent_config as AgentConfigurationV2);
      const before = {
        blocked: executor.canExecuteTool('initial_blocked_tool'),
        allowed: executor.canExecuteTool('any_other_tool')
      };

      // Update security configuration
      (session.agent_config as AgentConfigurationV2).blocked_tool_patterns = ['updated_blocked_*'];
      (session.agent_config as AgentConfigurationV2).allowed_tool_patterns = ['specific_*'];

      // Check after update
      executor = new ToolExecutor(session.agent_config as AgentConfigurationV2);
      const after = {
        oldBlocked: executor.canExecuteTool('initial_blocked_tool'),
        newBlocked: executor.canExecuteTool('updated_blocked_tool'),
        specific: executor.canExecuteTool('specific_tool'),
        other: executor.canExecuteTool('any_other_tool')
      };

      // Assert
      expect(before.blocked).toBe(false);
      expect(before.allowed).toBe(true);
      expect(after.oldBlocked).toBe(false); // Not in allow list now
      expect(after.newBlocked).toBe(false); // Blocked by new pattern
      expect(after.specific).toBe(true); // In allow list
      expect(after.other).toBe(false); // Not in allow list
    });
  });

  describe('Multi-Agent Security Scenarios', () => {
    it('should handle different security levels for different agents', () => {
      // Arrange
      const adminAgent: AgentConfigurationV2 = {
        version: 2,
        name: 'AdminAgent',
        key: 'admin',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [], // Admin has no blocks
        allowed_tool_patterns: undefined, // Can use everything
        persona: 'Admin assistant',
        category: ['admin']
      };

      const userAgent: AgentConfigurationV2 = {
        version: 2,
        name: 'UserAgent',
        key: 'user',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['admin_*', 'system_*', '*_delete', '*_write'],
        allowed_tool_patterns: ['user_*', 'read_*', 'list_*'],
        persona: 'User assistant',
        category: ['user']
      };

      const guestAgent: AgentConfigurationV2 = {
        version: 2,
        name: 'GuestAgent',
        key: 'guest',
        model_id: 'gpt-3.5-turbo',
        tools: [],
        blocked_tool_patterns: undefined, // Don't block everything
        allowed_tool_patterns: ['public_read_*', 'help_*'], // Use allowlist approach - Very limited
        persona: 'Guest assistant',
        category: ['guest']
      };

      const agents = [adminAgent, userAgent, guestAgent];
      const testTools = [
        'admin_panel',
        'system_config',
        'user_profile',
        'read_docs',
        'delete_file',
        'public_read_faq',
        'help_center'
      ];

      // Act
      const results = agents.map(agent => {
        const executor = new ToolExecutor(agent);
        return {
          agent: agent.name,
          permissions: testTools.map(tool => ({
            tool,
            allowed: executor.canExecuteTool(tool)
          }))
        };
      });

      // Assert
      // Admin can use everything
      const adminResults = results.find(r => r.agent === 'AdminAgent')!;
      expect(adminResults.permissions.every(p => p.allowed)).toBe(true);

      // User has restrictions
      const userResults = results.find(r => r.agent === 'UserAgent')!;
      expect(userResults.permissions.find(p => p.tool === 'admin_panel')!.allowed).toBe(false);
      expect(userResults.permissions.find(p => p.tool === 'user_profile')!.allowed).toBe(true);
      expect(userResults.permissions.find(p => p.tool === 'read_docs')!.allowed).toBe(true);
      expect(userResults.permissions.find(p => p.tool === 'delete_file')!.allowed).toBe(false);

      // Guest is highly restricted
      const guestResults = results.find(r => r.agent === 'GuestAgent')!;
      expect(guestResults.permissions.find(p => p.tool === 'public_read_faq')!.allowed).toBe(true);
      expect(guestResults.permissions.find(p => p.tool === 'help_center')!.allowed).toBe(true);
      expect(guestResults.permissions.find(p => p.tool === 'admin_panel')!.allowed).toBe(false);
      expect(guestResults.permissions.find(p => p.tool === 'user_profile')!.allowed).toBe(false);
    });

    it('should enforce blocked pattern precedence over allowed patterns', () => {
      // Arrange - Test that blocked patterns ALWAYS take precedence
      const conflictingConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'ConflictTestAgent',
        key: 'conflict_test',
        model_id: 'gpt-4',
        tools: [],
        // This configuration attempts to block everything then allow specific patterns
        // This should result in EVERYTHING being blocked due to precedence rules
        blocked_tool_patterns: ['*'], // Block everything
        allowed_tool_patterns: ['public_*', 'safe_*'], // Try to allow some
        persona: 'Testing blocked precedence',
        category: ['security_test']
      };

      const executor = new ToolExecutor(conflictingConfig);

      // Act - Test tools that match allowed patterns but should still be blocked
      const testCases = [
        { tool: 'public_read', expected: false }, // Matches allowed but blocked by '*'
        { tool: 'safe_operation', expected: false }, // Matches allowed but blocked by '*'
        { tool: 'any_tool', expected: false }, // Blocked by '*'
        { tool: 'admin_tool', expected: false } // Blocked by '*'
      ];

      // Assert - ALL tools should be blocked due to '*' in blocked_patterns
      testCases.forEach(({ tool, expected }) => {
        const result = executor.canExecuteTool(tool);
        expect(result).toBe(expected);
      });

      // Verify the execution log shows blocked by pattern
      const log = executor.getExecutionLog();
      log.forEach(entry => {
        expect(entry.allowed).toBe(false);
        expect(entry.reason).toContain('Blocked by pattern');
      });
    });

    it('should handle agent delegation with security inheritance', () => {
      // Arrange
      const parentAgent: AgentConfigurationV2 = {
        version: 2,
        name: 'ParentAgent',
        key: 'parent',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['parent_blocked_*'],
        allowed_tool_patterns: ['parent_allowed_*', 'shared_*'],
        persona: 'Parent agent',
        category: ['parent']
      };

      const childAgent: AgentConfigurationV2 = {
        version: 2,
        name: 'ChildAgent',
        key: 'child',
        model_id: 'gpt-3.5-turbo',
        tools: [],
        blocked_tool_patterns: ['child_blocked_*'],
        allowed_tool_patterns: ['child_allowed_*', 'shared_*'],
        persona: 'Child agent',
        category: ['child']
      };

      // Function to merge security configs (most restrictive wins)
      const mergeSecurityConfigs = (
        parent: AgentConfigurationV2,
        child: AgentConfigurationV2
      ): { blocked: string[]; allowed: string[] } => {
        // Combine blocked patterns (union)
        const blocked = [
          ...(parent.blocked_tool_patterns || []),
          ...(child.blocked_tool_patterns || [])
        ];

        // Intersect allowed patterns (more restrictive)
        const parentAllowed = new Set(parent.allowed_tool_patterns || []);
        const childAllowed = new Set(child.allowed_tool_patterns || []);
        
        let allowed: string[] = [];
        if (parentAllowed.size === 0) {
          allowed = Array.from(childAllowed);
        } else if (childAllowed.size === 0) {
          allowed = Array.from(parentAllowed);
        } else {
          // Intersection of both
          allowed = Array.from(parentAllowed).filter(p => childAllowed.has(p));
        }

        return { blocked, allowed };
      };

      // Act
      const merged = mergeSecurityConfigs(parentAgent, childAgent);

      // Assert
      expect(merged.blocked).toContain('parent_blocked_*');
      expect(merged.blocked).toContain('child_blocked_*');
      expect(merged.allowed).toContain('shared_*'); // Common pattern
      expect(merged.allowed).not.toContain('parent_allowed_*'); // Not in child
      expect(merged.allowed).not.toContain('child_allowed_*'); // Not in parent
    });
  });

  describe('Audit and Logging', () => {
    it('should create audit trail for security decisions', () => {
      // Arrange
      interface SecurityAuditEntry {
        timestamp: number;
        agent: string;
        tool: string;
        allowed: boolean;
        pattern?: string;
        reason?: string;
      }

      const auditLog: SecurityAuditEntry[] = [];

      class AuditingToolExecutor extends ToolExecutor {
        constructor(
          config: AgentConfigurationV2,
          private audit: SecurityAuditEntry[]
        ) {
          super(config);
        }

        canExecuteTool(toolName: string): boolean {
          const allowed = super.canExecuteTool(toolName);
          const reason = this.getAuditReason(toolName, allowed);
          
          this.audit.push({
            timestamp: Date.now(),
            agent: this['config'].name,
            tool: toolName,
            allowed,
            ...reason
          });
          
          return allowed;
        }

        private getAuditReason(toolName: string, allowed: boolean): { pattern?: string; reason?: string } {
          const config = this['config'];
          
          if (!allowed) {
            // Check which blocked pattern matched
            if (config.blocked_tool_patterns) {
              for (const pattern of config.blocked_tool_patterns) {
                if (this['matchesPattern'](toolName, pattern)) {
                  return { pattern, reason: 'Blocked by pattern' };
                }
              }
            }
            
            // Must be not in allow list
            return { reason: 'Not in allowed list' };
          }
          
          // Check which allowed pattern matched
          if (config.allowed_tool_patterns) {
            for (const pattern of config.allowed_tool_patterns) {
              if (this['matchesPattern'](toolName, pattern)) {
                return { pattern, reason: 'Allowed by pattern' };
              }
            }
          }
          
          return { reason: 'No restrictions' };
        }
      }

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'AuditedAgent',
        key: 'audited',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['dangerous_*', 'admin_*'],
        allowed_tool_patterns: ['safe_*', 'read_*'],
        persona: 'Audited assistant',
        category: ['audit']
      };

      const executor = new AuditingToolExecutor(config, auditLog);

      // Act
      executor.canExecuteTool('safe_operation');
      executor.canExecuteTool('dangerous_delete');
      executor.canExecuteTool('read_file');
      executor.canExecuteTool('write_file');
      executor.canExecuteTool('admin_panel');

      // Assert
      expect(auditLog).toHaveLength(5);
      
      // Check safe_operation
      const safeEntry = auditLog.find(e => e.tool === 'safe_operation');
      expect(safeEntry!.allowed).toBe(true);
      expect(safeEntry!.pattern).toBe('safe_*');
      
      // Check dangerous_delete
      const dangerousEntry = auditLog.find(e => e.tool === 'dangerous_delete');
      expect(dangerousEntry!.allowed).toBe(false);
      expect(dangerousEntry!.pattern).toBe('dangerous_*');
      expect(dangerousEntry!.reason).toBe('Blocked by pattern');
      
      // Check write_file (not in allow list)
      const writeEntry = auditLog.find(e => e.tool === 'write_file');
      expect(writeEntry!.allowed).toBe(false);
      expect(writeEntry!.reason).toBe('Not in allowed list');
      
      // All entries should have timestamps
      auditLog.forEach(entry => {
        expect(entry.timestamp).toBeGreaterThan(0);
        expect(entry.agent).toBe('AuditedAgent');
      });
    });

    it('should generate security report from execution history', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'ReportingAgent',
        key: 'reporting',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['blocked_*'],
        allowed_tool_patterns: ['allowed_*'],
        persona: 'Reporting assistant',
        category: ['report']
      };

      const executor = new ToolExecutor(config);

      // Execute various tools
      const toolTests = [
        'allowed_read',
        'allowed_write',
        'blocked_delete',
        'blocked_admin',
        'other_tool',
        'allowed_list',
        'blocked_danger'
      ];

      toolTests.forEach(tool => executor.canExecuteTool(tool));

      // Generate report
      const generateSecurityReport = (log: Array<{ tool: string; allowed: boolean; reason?: string }>) => {
        const total = log.length;
        const allowed = log.filter(e => e.allowed).length;
        const blocked = log.filter(e => !e.allowed).length;
        
        const blockedByPattern = log.filter(e => e.reason?.includes('Blocked by pattern')).length;
        const notInAllowList = log.filter(e => e.reason === 'Not in allowed list').length;
        
        return {
          summary: {
            total_checks: total,
            allowed: allowed,
            blocked: blocked,
            block_rate: (blocked / total * 100).toFixed(2) + '%'
          },
          blocked_reasons: {
            by_pattern: blockedByPattern,
            not_in_allow_list: notInAllowList
          },
          details: log.filter(e => !e.allowed).map(e => ({
            tool: e.tool,
            reason: e.reason
          }))
        };
      };

      // Act
      const report = generateSecurityReport(executor.getExecutionLog());

      // Assert
      expect(report.summary.total_checks).toBe(7);
      expect(report.summary.allowed).toBe(3);
      expect(report.summary.blocked).toBe(4);
      expect(report.summary.block_rate).toBe('57.14%');
      expect(report.blocked_reasons.by_pattern).toBe(3);
      expect(report.blocked_reasons.not_in_allow_list).toBe(1);
    });
  });
});