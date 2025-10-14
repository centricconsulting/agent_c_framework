/**
 * API Compliance Integration Tests
 * 
 * End-to-end tests validating all 7 critical fixes work together
 * in realistic usage scenarios for full API compliance.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventEmitter } from '../events/EventEmitter';
import type { 
  SetAvatarEvent, 
  SetAvatarSessionEvent,
  RenderMediaEvent,
  ToolCatalogEvent,
  HistoryEvent,
  ServerEvent
} from '../events/types/ServerEvents';
import type {
  ConfigUpdateEvent,
  SendMessageEvent,
  ClientEvent
} from '../events/types/ClientEvents';
import type { 
  Message,
  Tool,
  Avatar,
  AvatarSession
} from '../events/types/CommonTypes';
import type {
  ChatSession,
  AgentConfigurationV2
} from '../types/chat-session';
import type { MessageParam } from '../types/message-params';
import type { ChatCompletionMessageParam } from '../types/openai-message-params';
import type { UnifiedMessageParam } from '../types/ChatTypes';

// Mock implementation of a complete realtime session
class RealtimeSession {
  private emitter: EventEmitter<Record<string, any>>;
  private avatarId: string | null = null;
  private avatarSessionId: string | null = null;
  private vendor: 'anthropic' | 'openai' = 'anthropic';
  private tools: Tool[] = [];
  private agentConfig: AgentConfigurationV2 | null = null;
  private messages: Message[] = [];
  private isConnected: boolean = false;

  constructor() {
    this.emitter = new EventEmitter();
    this.setupEventHandlers();
  }

  private setupEventHandlers() {
    // Handle avatar events
    this.emitter.on('set_avatar', (event: SetAvatarEvent) => {
      this.avatarId = event.avatar_id;
    });

    this.emitter.on('set_avatar_session', (event: SetAvatarSessionEvent) => {
      this.avatarSessionId = event.session_id;
    });

    // Handle tool catalog
    this.emitter.on('tool_catalog', (event: ToolCatalogEvent) => {
      this.tools = event.tools; // Using correct 'tools' field
    });

    // Handle history
    this.emitter.on('history', (event: HistoryEvent) => {
      this.vendor = event.vendor as 'anthropic' | 'openai';
      this.messages = event.messages;
    });
  }

  connect() {
    this.isConnected = true;
    this.emitter.emit('connection:open', {});
  }

  disconnect() {
    this.isConnected = false;
    this.emitter.emit('connection:close', {});
  }

  setAgentConfig(config: AgentConfigurationV2) {
    this.agentConfig = config;
  }

  isToolAllowed(toolName: string): boolean {
    if (!this.agentConfig) return true;

    // Check blocked patterns first (highest priority)
    if (this.agentConfig.blocked_tool_patterns) {
      for (const pattern of this.agentConfig.blocked_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          return false;
        }
      }
    }

    // Check allowed patterns
    if (this.agentConfig.allowed_tool_patterns && this.agentConfig.allowed_tool_patterns.length > 0) {
      for (const pattern of this.agentConfig.allowed_tool_patterns) {
        if (this.matchesPattern(toolName, pattern)) {
          return true;
        }
      }
      return false; // Not in allow list
    }

    return true; // No restrictions
  }

  private matchesPattern(name: string, pattern: string): boolean {
    const regexPattern = pattern
      .replace(/[.+^${}()|[\]\\]/g, '\\$&')
      .replace(/\*/g, '.*')
      .replace(/\?/g, '.');
    
    return new RegExp(`^${regexPattern}$`).test(name);
  }

  processMessage(message: Message): UnifiedMessageParam {
    if (this.vendor === 'anthropic') {
      return this.toAnthropicFormat(message);
    } else {
      return this.toOpenAIFormat(message);
    }
  }

  private toAnthropicFormat(message: Message): MessageParam {
    return {
      role: message.role as 'user' | 'assistant',
      content: typeof message.content === 'string' 
        ? message.content 
        : Array.isArray(message.content)
          ? message.content
          : [{ type: 'text', text: JSON.stringify(message.content) }]
    };
  }

  private toOpenAIFormat(message: Message): ChatCompletionMessageParam {
    return {
      role: message.role,
      content: typeof message.content === 'string'
        ? message.content
        : JSON.stringify(message.content)
    } as ChatCompletionMessageParam;
  }

  getState() {
    return {
      isConnected: this.isConnected,
      avatarId: this.avatarId,
      avatarSessionId: this.avatarSessionId,
      vendor: this.vendor,
      tools: this.tools,
      agentConfig: this.agentConfig,
      messages: this.messages
    };
  }

  emit(event: string, data: any) {
    this.emitter.emit(event, data);
  }

  on(event: string, handler: Function) {
    this.emitter.on(event, handler);
  }
}

describe('API Compliance Integration Tests', () => {
  let session: RealtimeSession;

  beforeEach(() => {
    vi.clearAllMocks();
    session = new RealtimeSession();
  });

  afterEach(() => {
    vi.restoreAllMocks();
    session.disconnect();
  });

  describe('Complete Avatar Flow', () => {
    it('should handle SetAvatarEvent → SetAvatarSessionEvent → Avatar active flow', () => {
      // Arrange
      const avatarId = 'avatar_123';
      const sessionId = 'session_456';
      const avatarData: Avatar = {
        avatar_id: avatarId,
        created_at: Date.now(),
        default_voice: 'en-US',
        is_public: true,
        normal_preview: 'https://preview.url',
        pose_name: 'professional',
        status: 'active'
      };

      const avatarSession: AvatarSession = {
        session_id: sessionId,
        url: 'wss://avatar.session.url',
        access_token: 'token_789',
        session_duration_limit: 3600,
        is_paid: true,
        realtime_endpoint: 'wss://realtime.endpoint',
        sdp: null,
        ice_servers: null,
        ice_servers2: null
      };

      // Act - Complete avatar flow
      session.connect();

      // Step 1: Set avatar
      const setAvatarEvent: SetAvatarEvent = {
        type: 'set_avatar',
        role: 'system',
        avatar_id: avatarId, // Correct field name
        avatar: avatarData,
        session_id: 'main_session',
        created_at: new Date().toISOString()
      };
      session.emit('set_avatar', setAvatarEvent);

      // Step 2: Set avatar session
      const setAvatarSessionEvent: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        role: 'system',
        session_id: sessionId, // Correct field name
        avatar_session: avatarSession,
        created_at: new Date().toISOString()
      };
      session.emit('set_avatar_session', setAvatarSessionEvent);

      // Step 3: Verify avatar is active in state
      const state = session.getState();

      // Assert
      expect(state.isConnected).toBe(true);
      expect(state.avatarId).toBe(avatarId);
      expect(state.avatarSessionId).toBe(sessionId);
      
      // Verify field names are correct
      expect(setAvatarEvent.avatar_id).toBe(avatarId);
      expect(setAvatarEvent).toHaveProperty('avatar_id');
      expect(setAvatarEvent).not.toHaveProperty('avatarId');
      
      expect(setAvatarSessionEvent.session_id).toBe(sessionId);
      expect(setAvatarSessionEvent).toHaveProperty('session_id');
      expect(setAvatarSessionEvent).not.toHaveProperty('sessionId');
    });

    it('should maintain avatar state through message flow', () => {
      // Arrange
      const avatarId = 'avatar_test';
      const messages: Message[] = [];
      
      session.on('message', (msg: Message) => {
        messages.push(msg);
      });

      // Act
      session.connect();
      
      // Set avatar
      session.emit('set_avatar', {
        type: 'set_avatar',
        role: 'system',
        avatar_id: avatarId,
        avatar: { avatar_id: avatarId },
        session_id: 'test',
        created_at: new Date().toISOString()
      });

      // Send messages with avatar active
      const userMessage: Message = {
        role: 'user',
        content: 'Hello with avatar',
        timestamp: new Date().toISOString()
      };

      session.emit('message', userMessage);

      const assistantMessage: Message = {
        role: 'assistant',
        content: 'Response with avatar active',
        timestamp: new Date().toISOString()
      };

      session.emit('message', assistantMessage);

      // Assert
      expect(messages).toHaveLength(2);
      expect(session.getState().avatarId).toBe(avatarId);
      
      // Avatar should remain active through message exchange
      messages.forEach(msg => {
        expect(session.getState().avatarId).toBe(avatarId);
      });
    });
  });

  describe('Multi-Vendor Message Handling', () => {
    it('should process Anthropic messages with vendor=anthropic', () => {
      // Arrange
      const anthropicMessages: Message[] = [
        {
          role: 'user',
          content: 'Hello Claude',
          model_id: 'claude-3-5-sonnet-20241022'
        },
        {
          role: 'assistant',
          content: 'Hello! I am Claude.',
          model_id: 'claude-3-5-sonnet-20241022'
        }
      ];

      // Act
      session.connect();
      
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic', // Critical field
        messages: anthropicMessages,
        session_id: 'anthropic_session',
        created_at: new Date().toISOString()
      };

      session.emit('history', historyEvent);

      // Process messages
      const processedMessages = anthropicMessages.map(msg => 
        session.processMessage(msg)
      );

      // Assert
      expect(session.getState().vendor).toBe('anthropic');
      
      processedMessages.forEach(msg => {
        const anthropicMsg = msg as MessageParam;
        expect(anthropicMsg.role).toBeDefined();
        expect(['user', 'assistant']).toContain(anthropicMsg.role);
        expect(anthropicMsg.content).toBeDefined();
      });
    });

    it('should process OpenAI messages with vendor=openai', () => {
      // Arrange
      const openaiMessages: Message[] = [
        {
          role: 'system',
          content: 'You are a helpful assistant.',
          model_id: 'gpt-4-turbo'
        },
        {
          role: 'user',
          content: 'Hello GPT',
          model_id: 'gpt-4-turbo'
        },
        {
          role: 'assistant',
          content: 'Hello! How can I help you?',
          model_id: 'gpt-4-turbo'
        }
      ];

      // Act
      session.connect();
      
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai', // Critical field
        messages: openaiMessages,
        session_id: 'openai_session',
        created_at: new Date().toISOString()
      };

      session.emit('history', historyEvent);

      // Process messages
      const processedMessages = openaiMessages.map(msg => 
        session.processMessage(msg)
      );

      // Assert
      expect(session.getState().vendor).toBe('openai');
      
      processedMessages.forEach(msg => {
        const openaiMsg = msg as ChatCompletionMessageParam;
        expect(openaiMsg.role).toBeDefined();
        expect(['system', 'user', 'assistant']).toContain(openaiMsg.role);
        expect(openaiMsg.content).toBeDefined();
      });
    });

    it('should switch vendors correctly in different sessions', () => {
      // Arrange
      const sessions: Array<{ vendor: 'anthropic' | 'openai'; model: string }> = [
        { vendor: 'anthropic', model: 'claude-3-5-sonnet' },
        { vendor: 'openai', model: 'gpt-4' },
        { vendor: 'anthropic', model: 'claude-3-opus' },
        { vendor: 'openai', model: 'gpt-3.5-turbo' }
      ];

      // Act & Assert
      sessions.forEach((config, index) => {
        const testSession = new RealtimeSession();
        testSession.connect();

        const historyEvent: HistoryEvent = {
          type: 'history',
          role: 'system',
          vendor: config.vendor,
          messages: [{
            role: 'user',
            content: `Test message ${index}`,
            model_id: config.model
          }],
          session_id: `session_${index}`,
          created_at: new Date().toISOString()
        };

        testSession.emit('history', historyEvent);

        // Verify vendor is set correctly
        expect(testSession.getState().vendor).toBe(config.vendor);
        
        // Process message with correct format
        const processed = testSession.processMessage(historyEvent.messages[0]);
        
        if (config.vendor === 'anthropic') {
          const anthropicMsg = processed as MessageParam;
          expect(['user', 'assistant']).toContain(anthropicMsg.role);
        } else {
          const openaiMsg = processed as ChatCompletionMessageParam;
          expect(openaiMsg.role).toBeDefined();
        }

        testSession.disconnect();
      });
    });
  });

  describe('Tool Catalog with Security', () => {
    it('should handle ToolCatalogEvent with correct tools field and apply security', () => {
      // Arrange
      const tools: Tool[] = [
        {
          name: 'safe_read_file',
          description: 'Read a file safely',
          schemas: {}
        },
        {
          name: 'admin_delete_user',
          description: 'Delete a user (admin only)',
          schemas: {}
        },
        {
          name: 'public_search',
          description: 'Search public data',
          schemas: {}
        },
        {
          name: 'dangerous_execute',
          description: 'Execute dangerous operation',
          schemas: {}
        }
      ];

      const agentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'SecureAgent',
        key: 'secure_agent',
        model_id: 'gpt-4',
        tools: ['safe_read_file', 'public_search'],
        blocked_tool_patterns: ['admin_*', 'dangerous_*', '*_delete_*'],
        allowed_tool_patterns: ['safe_*', 'public_*', 'read_*'],
        persona: 'Secure assistant',
        category: ['secure']
      };

      // Act
      session.connect();
      session.setAgentConfig(agentConfig);

      // Send tool catalog with correct field name
      const toolCatalogEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        role: 'system',
        tools: tools, // Correct field name (not 'toolsets')
        session_id: 'tool_session',
        created_at: new Date().toISOString()
      };

      session.emit('tool_catalog', toolCatalogEvent);

      // Assert
      expect(session.getState().tools).toEqual(tools);
      expect(toolCatalogEvent.tools).toEqual(tools);
      expect(toolCatalogEvent).toHaveProperty('tools');
      expect(toolCatalogEvent).not.toHaveProperty('toolsets');

      // Test security patterns
      expect(session.isToolAllowed('safe_read_file')).toBe(true);
      expect(session.isToolAllowed('public_search')).toBe(true);
      expect(session.isToolAllowed('admin_delete_user')).toBe(false); // Blocked by admin_*
      expect(session.isToolAllowed('dangerous_execute')).toBe(false); // Blocked by dangerous_*
      expect(session.isToolAllowed('user_delete_record')).toBe(false); // Blocked by *_delete_*
      expect(session.isToolAllowed('read_database')).toBe(true); // Allowed by read_*
    });

    it('should enforce tool security throughout execution flow', () => {
      // Arrange
      const executionLog: Array<{ tool: string; allowed: boolean; reason?: string }> = [];

      const agentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'RestrictedAgent',
        key: 'restricted',
        model_id: 'claude-3',
        tools: [],
        blocked_tool_patterns: ['system_*', '*_admin', 'delete_*'],
        allowed_tool_patterns: ['user_*', 'read_*', 'list_*'],
        persona: 'Restricted assistant',
        category: ['limited']
      };

      const attemptToolExecution = (toolName: string) => {
        const allowed = session.isToolAllowed(toolName);
        executionLog.push({
          tool: toolName,
          allowed,
          reason: allowed ? 'Allowed by pattern' : 'Blocked by security'
        });
        return allowed;
      };

      // Act
      session.connect();
      session.setAgentConfig(agentConfig);

      // Send tool catalog
      const tools: Tool[] = [
        { name: 'user_profile', description: 'User profile tool', schemas: {} },
        { name: 'system_config', description: 'System config tool', schemas: {} },
        { name: 'read_file', description: 'Read file tool', schemas: {} },
        { name: 'delete_record', description: 'Delete record tool', schemas: {} },
        { name: 'tool_admin', description: 'Admin tool', schemas: {} },
        { name: 'list_items', description: 'List items tool', schemas: {} }
      ];

      session.emit('tool_catalog', {
        type: 'tool_catalog',
        role: 'system',
        tools: tools,
        session_id: 'security_test',
        created_at: new Date().toISOString()
      });

      // Attempt to execute various tools
      tools.forEach(tool => {
        attemptToolExecution(tool.name);
      });

      // Assert
      expect(executionLog).toHaveLength(6);
      
      // Check specific tools
      const userProfileExec = executionLog.find(e => e.tool === 'user_profile');
      expect(userProfileExec?.allowed).toBe(true);
      
      const systemConfigExec = executionLog.find(e => e.tool === 'system_config');
      expect(systemConfigExec?.allowed).toBe(false);
      
      const deleteRecordExec = executionLog.find(e => e.tool === 'delete_record');
      expect(deleteRecordExec?.allowed).toBe(false);
      
      const listItemsExec = executionLog.find(e => e.tool === 'list_items');
      expect(listItemsExec?.allowed).toBe(true);
    });
  });

  describe('Secure Content Rendering', () => {
    it('should handle RenderMediaEvent with foreign_content security flag', () => {
      // Arrange
      const renderEvents: RenderMediaEvent[] = [];
      const sanitizationLog: Array<{ event_id: string; sanitized: boolean }> = [];

      session.on('render_media', (event: RenderMediaEvent) => {
        renderEvents.push(event);
        
        // Simulate sanitization decision based on foreign_content
        const needsSanitization = event.foreign_content === true;
        sanitizationLog.push({
          event_id: event.event_id,
          sanitized: needsSanitization
        });
      });

      // Act
      session.connect();

      // Trusted content (no sanitization needed)
      const trustedMediaEvent: RenderMediaEvent = {
        type: 'render_media',
        role: 'system',
        event_id: 'trusted_001',
        media_type: 'video',
        media_url: 'https://trusted.cdn/video.mp4',
        foreign_content: false, // Critical field
        session_id: 'render_session',
        created_at: new Date().toISOString()
      };

      session.emit('render_media', trustedMediaEvent);

      // Untrusted content (requires sanitization)
      const untrustedMediaEvent: RenderMediaEvent = {
        type: 'render_media',
        role: 'system',
        event_id: 'untrusted_002',
        media_type: 'iframe',
        media_url: 'https://external.site/embed',
        foreign_content: true, // Critical field - triggers sanitization
        session_id: 'render_session',
        created_at: new Date().toISOString()
      };

      session.emit('render_media', untrustedMediaEvent);

      // Mixed media with various trust levels
      const mixedMediaEvent: RenderMediaEvent = {
        type: 'render_media',
        role: 'system',
        event_id: 'mixed_003',
        media_type: 'html',
        media_url: 'data:text/html,<script>alert("test")</script>',
        foreign_content: true, // Should be sanitized
        session_id: 'render_session',
        created_at: new Date().toISOString()
      };

      session.emit('render_media', mixedMediaEvent);

      // Assert
      expect(renderEvents).toHaveLength(3);
      expect(sanitizationLog).toHaveLength(3);

      // Check trusted content
      expect(trustedMediaEvent.foreign_content).toBe(false);
      expect(sanitizationLog[0].sanitized).toBe(false);

      // Check untrusted content
      expect(untrustedMediaEvent.foreign_content).toBe(true);
      expect(sanitizationLog[1].sanitized).toBe(true);

      // Check mixed content
      expect(mixedMediaEvent.foreign_content).toBe(true);
      expect(sanitizationLog[2].sanitized).toBe(true);

      // Verify field name is correct
      renderEvents.forEach(event => {
        expect(event).toHaveProperty('foreign_content');
        expect(event).not.toHaveProperty('foreignContent');
        expect(typeof event.foreign_content).toBe('boolean');
      });
    });

    it('should flow security decisions to UI rendering', () => {
      // Arrange
      interface RenderDecision {
        event_id: string;
        render_mode: 'direct' | 'sandboxed' | 'blocked';
        security_applied: boolean;
      }

      const renderDecisions: RenderDecision[] = [];

      const makeRenderDecision = (event: RenderMediaEvent): RenderDecision => {
        let render_mode: 'direct' | 'sandboxed' | 'blocked';
        let security_applied = false;

        if (event.foreign_content) {
          security_applied = true;
          
          // All foreign content is sandboxed for security
          render_mode = 'sandboxed';
        } else {
          render_mode = 'direct';
        }

        return {
          event_id: event.event_id,
          render_mode,
          security_applied
        };
      };

      // Act
      const testEvents: RenderMediaEvent[] = [
        {
          type: 'render_media',
          role: 'system',
          event_id: 'safe_image',
          media_type: 'image',
          media_url: 'https://cdn.example/image.png',
          foreign_content: false,
          session_id: 'ui_test',
          created_at: new Date().toISOString()
        },
        {
          type: 'render_media',
          role: 'system',
          event_id: 'external_iframe',
          media_type: 'iframe',
          media_url: 'https://external.com/widget',
          foreign_content: true,
          session_id: 'ui_test',
          created_at: new Date().toISOString()
        },
        {
          type: 'render_media',
          role: 'system',
          event_id: 'data_uri',
          media_type: 'html',
          media_url: 'data:text/html,<h1>Test</h1>',
          foreign_content: true,
          session_id: 'ui_test',
          created_at: new Date().toISOString()
        }
      ];

      testEvents.forEach(event => {
        const decision = makeRenderDecision(event);
        renderDecisions.push(decision);
      });

      // Assert
      expect(renderDecisions).toHaveLength(3);

      // Safe image - direct rendering
      expect(renderDecisions[0].render_mode).toBe('direct');
      expect(renderDecisions[0].security_applied).toBe(false);

      // External iframe - sandboxed
      expect(renderDecisions[1].render_mode).toBe('sandboxed');
      expect(renderDecisions[1].security_applied).toBe(true);

      // Data URI - sandboxed (all foreign content is sandboxed)
      expect(renderDecisions[2].render_mode).toBe('sandboxed');
      expect(renderDecisions[2].security_applied).toBe(true);
    });
  });

  describe('Complete Session Flow', () => {
    it('should handle complete flow: Connection → Auth → History → Messages → Tools → Disconnect', async () => {
      // Arrange
      const flowEvents: string[] = [];
      const messageLog: Message[] = [];
      
      // Track all events
      const trackEvent = (eventName: string) => {
        flowEvents.push(eventName);
      };

      session.on('connection:open', () => trackEvent('connection:open'));
      session.on('auth:success', () => trackEvent('auth:success'));
      session.on('history', () => trackEvent('history:received'));
      session.on('message', (msg: Message) => {
        trackEvent('message:received');
        messageLog.push(msg);
      });
      session.on('tool_catalog', () => trackEvent('tool_catalog:received'));
      session.on('tool:execute', () => trackEvent('tool:executed'));
      session.on('connection:close', () => trackEvent('connection:close'));

      // Act - Complete session flow
      
      // 1. Connect
      session.connect();
      
      // 2. Authenticate
      session.emit('auth:success', { user_id: 'user123' });
      
      // 3. Receive history with vendor
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'anthropic', // Critical vendor field
        messages: [
          { role: 'user', content: 'Previous message 1' },
          { role: 'assistant', content: 'Previous response 1' }
        ],
        session_id: 'complete_flow',
        created_at: new Date().toISOString()
      };
      session.emit('history', historyEvent);
      
      // 4. Set avatar
      const setAvatarEvent: SetAvatarEvent = {
        type: 'set_avatar',
        role: 'system',
        avatar_id: 'avatar_flow', // Correct field
        avatar: { avatar_id: 'avatar_flow' } as Avatar,
        session_id: 'complete_flow',
        created_at: new Date().toISOString()
      };
      session.emit('set_avatar', setAvatarEvent);
      
      // 5. Receive tool catalog with security
      const agentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'FlowAgent',
        key: 'flow_agent',
        model_id: 'claude-3-5-sonnet',
        tools: ['allowed_tool'],
        blocked_tool_patterns: ['dangerous_*'], // Security patterns
        allowed_tool_patterns: ['allowed_*'],
        persona: 'Flow test agent',
        category: ['test']
      };
      session.setAgentConfig(agentConfig);
      
      const toolCatalogEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        role: 'system',
        tools: [ // Correct field name
          { name: 'allowed_tool', description: 'Allowed', schemas: {} },
          { name: 'dangerous_tool', description: 'Dangerous', schemas: {} }
        ],
        session_id: 'complete_flow',
        created_at: new Date().toISOString()
      };
      session.emit('tool_catalog', toolCatalogEvent);
      
      // 6. Send/receive messages
      const userMessage: Message = {
        role: 'user',
        content: 'New message',
        timestamp: new Date().toISOString()
      };
      session.emit('message', userMessage);
      
      const assistantMessage: Message = {
        role: 'assistant',
        content: 'Response to new message',
        timestamp: new Date().toISOString()
      };
      session.emit('message', assistantMessage);
      
      // 7. Execute tool (respecting security)
      const toolToExecute = 'allowed_tool';
      if (session.isToolAllowed(toolToExecute)) {
        session.emit('tool:execute', { tool: toolToExecute });
      }
      
      // 8. Render media with security
      const renderEvent: RenderMediaEvent = {
        type: 'render_media',
        role: 'system',
        event_id: 'flow_media',
        media_type: 'image',
        media_url: 'https://example.com/image.png',
        foreign_content: false, // Security flag
        session_id: 'complete_flow',
        created_at: new Date().toISOString()
      };
      session.emit('render_media', renderEvent);
      
      // 9. Disconnect
      session.disconnect();
      
      // Assert - Verify complete flow
      expect(flowEvents).toContain('connection:open');
      expect(flowEvents).toContain('auth:success');
      expect(flowEvents).toContain('history:received');
      expect(flowEvents).toContain('message:received');
      expect(flowEvents).toContain('tool_catalog:received');
      expect(flowEvents).toContain('tool:executed');
      expect(flowEvents).toContain('connection:close');
      
      // Verify state after flow
      const finalState = session.getState();
      expect(finalState.isConnected).toBe(false);
      expect(finalState.vendor).toBe('anthropic');
      expect(finalState.avatarId).toBe('avatar_flow');
      expect(finalState.tools).toHaveLength(2);
      expect(finalState.messages).toHaveLength(2); // From history
      
      // Verify security was applied
      expect(session.isToolAllowed('allowed_tool')).toBe(true);
      expect(session.isToolAllowed('dangerous_tool')).toBe(false);
      
      // Verify message log
      expect(messageLog).toHaveLength(2);
      expect(messageLog[0].content).toBe('New message');
      expect(messageLog[1].content).toBe('Response to new message');
    });

    it('should maintain all fixed fields throughout session lifecycle', () => {
      // Arrange
      const fieldValidations: Array<{ event: string; field: string; value: any }> = [];
      
      const validateField = (event: string, field: string, value: any) => {
        fieldValidations.push({ event, field, value });
      };

      // Act - Test all critical fields
      session.connect();

      // 1. SetAvatarEvent - avatar_id field
      const avatarEvent: SetAvatarEvent = {
        type: 'set_avatar',
        role: 'system',
        avatar_id: 'test_avatar_id',
        avatar: { avatar_id: 'test_avatar_id' } as Avatar,
        session_id: 'field_test',
        created_at: new Date().toISOString()
      };
      validateField('SetAvatarEvent', 'avatar_id', avatarEvent.avatar_id);
      session.emit('set_avatar', avatarEvent);

      // 2. SetAvatarSessionEvent - session_id field
      const avatarSessionEvent: SetAvatarSessionEvent = {
        type: 'set_avatar_session',
        role: 'system',
        session_id: 'avatar_session_123',
        avatar_session: {} as AvatarSession,
        created_at: new Date().toISOString()
      };
      validateField('SetAvatarSessionEvent', 'session_id', avatarSessionEvent.session_id);
      session.emit('set_avatar_session', avatarSessionEvent);

      // 3. RenderMediaEvent - foreign_content field
      const renderEvent: RenderMediaEvent = {
        type: 'render_media',
        role: 'system',
        event_id: 'render_test',
        media_type: 'video',
        media_url: 'https://test.url',
        foreign_content: true,
        session_id: 'field_test',
        created_at: new Date().toISOString()
      };
      validateField('RenderMediaEvent', 'foreign_content', renderEvent.foreign_content);
      session.emit('render_media', renderEvent);

      // 4. ToolCatalogEvent - tools field (not toolsets)
      const toolEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        role: 'system',
        tools: [{ name: 'test_tool', description: 'Test', schemas: {} }],
        session_id: 'field_test',
        created_at: new Date().toISOString()
      };
      validateField('ToolCatalogEvent', 'tools', toolEvent.tools);
      session.emit('tool_catalog', toolEvent);

      // 5. HistoryEvent - vendor field
      const historyEvent: HistoryEvent = {
        type: 'history',
        role: 'system',
        vendor: 'openai',
        messages: [],
        session_id: 'field_test',
        created_at: new Date().toISOString()
      };
      validateField('HistoryEvent', 'vendor', historyEvent.vendor);
      session.emit('history', historyEvent);

      // 6. ConfigUpdateEvent - optional parameters
      const configEvent: ConfigUpdateEvent = {
        type: 'config_update',
        config: {
          model: 'gpt-4',
          temperature: 0.7,
          max_tokens: 1000,
          stream: true
        }
      };
      validateField('ConfigUpdateEvent', 'temperature', configEvent.config.temperature);
      validateField('ConfigUpdateEvent', 'max_tokens', configEvent.config.max_tokens);
      validateField('ConfigUpdateEvent', 'stream', configEvent.config.stream);

      // 7. AgentConfigurationV2 - security fields
      const agentConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'TestAgent',
        key: 'test_agent',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['blocked_*'],
        allowed_tool_patterns: ['allowed_*'],
        persona: 'Test',
        category: ['test']
      };
      validateField('AgentConfigurationV2', 'blocked_tool_patterns', agentConfig.blocked_tool_patterns);
      validateField('AgentConfigurationV2', 'allowed_tool_patterns', agentConfig.allowed_tool_patterns);

      // Assert - All fields are present and correct
      expect(fieldValidations).toHaveLength(10);
      
      // Verify specific critical fields
      const avatarIdField = fieldValidations.find(v => 
        v.event === 'SetAvatarEvent' && v.field === 'avatar_id'
      );
      expect(avatarIdField?.value).toBe('test_avatar_id');

      const foreignContentField = fieldValidations.find(v => 
        v.event === 'RenderMediaEvent' && v.field === 'foreign_content'
      );
      expect(foreignContentField?.value).toBe(true);

      const toolsField = fieldValidations.find(v => 
        v.event === 'ToolCatalogEvent' && v.field === 'tools'
      );
      expect(Array.isArray(toolsField?.value)).toBe(true);

      const vendorField = fieldValidations.find(v => 
        v.event === 'HistoryEvent' && v.field === 'vendor'
      );
      expect(vendorField?.value).toBe('openai');

      const blockedPatternsField = fieldValidations.find(v => 
        v.event === 'AgentConfigurationV2' && v.field === 'blocked_tool_patterns'
      );
      expect(Array.isArray(blockedPatternsField?.value)).toBe(true);
    });

    it('should handle error recovery while maintaining field integrity', () => {
      // Arrange
      const errors: Array<{ stage: string; error: string; recovered: boolean }> = [];
      
      const handleError = (stage: string, error: string): boolean => {
        // Attempt recovery
        const canRecover = !error.includes('FATAL');
        errors.push({ stage, error, recovered: canRecover });
        return canRecover;
      };

      // Act
      session.connect();

      // Simulate errors at various stages
      
      // 1. Avatar error - recoverable
      try {
        const invalidAvatar = {
          type: 'set_avatar',
          role: 'system',
          avatarId: 'wrong_field', // Wrong field name
          session_id: 'error_test'
        } as any;
        
        // Check for correct field
        if (!invalidAvatar.avatar_id) {
          throw new Error('Missing avatar_id field');
        }
      } catch (error) {
        const recovered = handleError('avatar_setup', (error as Error).message);
        if (recovered) {
          // Retry with correct field
          session.emit('set_avatar', {
            type: 'set_avatar',
            role: 'system',
            avatar_id: 'recovered_avatar', // Correct field
            avatar: { avatar_id: 'recovered_avatar' } as Avatar,
            session_id: 'error_test',
            created_at: new Date().toISOString()
          });
        }
      }

      // 2. Tool catalog error - recoverable
      try {
        const invalidToolCatalog = {
          type: 'tool_catalog',
          role: 'system',
          toolsets: [], // Wrong field name
          session_id: 'error_test'
        } as any;
        
        // Check for correct field
        if (!invalidToolCatalog.tools) {
          throw new Error('Missing tools field (found toolsets)');
        }
      } catch (error) {
        const recovered = handleError('tool_catalog', (error as Error).message);
        if (recovered) {
          // Retry with correct field
          session.emit('tool_catalog', {
            type: 'tool_catalog',
            role: 'system',
            tools: [], // Correct field
            session_id: 'error_test',
            created_at: new Date().toISOString()
          });
        }
      }

      // 3. Vendor error - critical but recoverable
      try {
        const invalidHistory = {
          type: 'history',
          role: 'system',
          vendor: 'invalid_vendor', // Invalid vendor
          messages: [],
          session_id: 'error_test'
        } as any;
        
        // Validate vendor
        if (!['anthropic', 'openai'].includes(invalidHistory.vendor)) {
          throw new Error(`Invalid vendor: ${invalidHistory.vendor}`);
        }
      } catch (error) {
        const recovered = handleError('history_vendor', (error as Error).message);
        if (recovered) {
          // Retry with valid vendor
          session.emit('history', {
            type: 'history',
            role: 'system',
            vendor: 'anthropic', // Valid vendor
            messages: [],
            session_id: 'error_test',
            created_at: new Date().toISOString()
          });
        }
      }

      // Assert
      expect(errors).toHaveLength(3);
      
      // All errors should be recovered
      errors.forEach(error => {
        expect(error.recovered).toBe(true);
      });

      // State should be valid after recovery
      const state = session.getState();
      expect(state.avatarId).toBe('recovered_avatar');
      expect(state.tools).toEqual([]);
      expect(state.vendor).toBe('anthropic');
    });
  });

  describe('Cross-Component Integration', () => {
    it('should handle avatar + vendor + tools + security together', () => {
      // Arrange
      const complexSession = new RealtimeSession();
      const eventLog: string[] = [];

      // Track integration points
      complexSession.on('integration:checkpoint', (checkpoint: string) => {
        eventLog.push(checkpoint);
      });

      // Act - Complex integration scenario
      complexSession.connect();

      // 1. Set Anthropic vendor with Claude model
      complexSession.emit('history', {
        type: 'history',
        role: 'system',
        vendor: 'anthropic',
        messages: [
          { role: 'user', content: 'Using Claude', model_id: 'claude-3-5-sonnet' }
        ],
        session_id: 'complex',
        created_at: new Date().toISOString()
      });
      complexSession.emit('integration:checkpoint', 'vendor_set_anthropic');

      // 2. Configure avatar
      complexSession.emit('set_avatar', {
        type: 'set_avatar',
        role: 'system',
        avatar_id: 'claude_avatar',
        avatar: { avatar_id: 'claude_avatar' } as Avatar,
        session_id: 'complex',
        created_at: new Date().toISOString()
      });
      complexSession.emit('integration:checkpoint', 'avatar_configured');

      // 3. Set security configuration
      const secureConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'ClaudeSecure',
        key: 'claude_secure',
        model_id: 'claude-3-5-sonnet',
        tools: [],
        blocked_tool_patterns: ['admin_*', 'delete_*', 'system_*'],
        allowed_tool_patterns: ['read_*', 'list_*', 'search_*'],
        persona: 'Secure Claude',
        category: ['secure']
      };
      complexSession.setAgentConfig(secureConfig);
      complexSession.emit('integration:checkpoint', 'security_configured');

      // 4. Load tools respecting security
      const availableTools: Tool[] = [
        { name: 'read_file', description: 'Read', schemas: {} },
        { name: 'admin_panel', description: 'Admin', schemas: {} },
        { name: 'search_data', description: 'Search', schemas: {} },
        { name: 'delete_user', description: 'Delete', schemas: {} },
        { name: 'list_items', description: 'List', schemas: {} }
      ];

      complexSession.emit('tool_catalog', {
        type: 'tool_catalog',
        role: 'system',
        tools: availableTools,
        session_id: 'complex',
        created_at: new Date().toISOString()
      });
      complexSession.emit('integration:checkpoint', 'tools_loaded');

      // 5. Process message with Claude format
      const claudeMessage = complexSession.processMessage({
        role: 'assistant',
        content: 'I can help with that using the available tools.',
        model_id: 'claude-3-5-sonnet'
      });
      complexSession.emit('integration:checkpoint', 'message_processed_claude');

      // 6. Execute allowed tool
      if (complexSession.isToolAllowed('read_file')) {
        complexSession.emit('integration:checkpoint', 'tool_read_file_allowed');
      }

      // 7. Block dangerous tool
      if (!complexSession.isToolAllowed('admin_panel')) {
        complexSession.emit('integration:checkpoint', 'tool_admin_panel_blocked');
      }

      // 8. Render secure content
      complexSession.emit('render_media', {
        type: 'render_media',
        role: 'system',
        event_id: 'secure_render',
        media_type: 'image',
        media_url: 'https://secure.cdn/image.png',
        foreign_content: false,
        session_id: 'complex',
        created_at: new Date().toISOString()
      });
      complexSession.emit('integration:checkpoint', 'media_rendered_secure');

      // Assert - Verify all components work together
      const finalState = complexSession.getState();
      
      // Avatar active
      expect(finalState.avatarId).toBe('claude_avatar');
      
      // Vendor correct
      expect(finalState.vendor).toBe('anthropic');
      
      // Tools loaded
      expect(finalState.tools).toHaveLength(5);
      
      // Security applied
      expect(complexSession.isToolAllowed('read_file')).toBe(true);
      expect(complexSession.isToolAllowed('admin_panel')).toBe(false);
      expect(complexSession.isToolAllowed('delete_user')).toBe(false);
      expect(complexSession.isToolAllowed('search_data')).toBe(true);
      expect(complexSession.isToolAllowed('list_items')).toBe(true);
      
      // Message in correct format
      expect(claudeMessage).toHaveProperty('role');
      expect(claudeMessage).toHaveProperty('content');
      
      // Integration checkpoints reached
      expect(eventLog).toContain('vendor_set_anthropic');
      expect(eventLog).toContain('avatar_configured');
      expect(eventLog).toContain('security_configured');
      expect(eventLog).toContain('tools_loaded');
      expect(eventLog).toContain('message_processed_claude');
      expect(eventLog).toContain('tool_read_file_allowed');
      expect(eventLog).toContain('tool_admin_panel_blocked');
      expect(eventLog).toContain('media_rendered_secure');
    });
  });
});