/**
 * Test fixtures for Agent C Realtime API protocol events
 * These are REAL protocol events that the SDK sends and receives over WebSocket
 */

import type {
  // Server Events
  SystemPromptEvent,
  UserRequestEvent,
  TextDeltaEvent,
  ThoughtDeltaEvent,
  CompletionEvent,
  InteractionEvent,
  HistoryEvent,
  HistoryDeltaEvent,
  ToolCallEvent,
  ToolSelectDeltaEvent,
  SystemMessageEvent,
  ErrorEvent,
  UserTurnStartEvent,
  UserTurnEndEvent,
  ServerListeningEvent,
  ChatSessionChangedEvent,
  ChatSessionAddedEvent,
  ChatSessionDeletedEvent,
  ChatSessionNameChangedEvent,
  SessionMetadataChangedEvent,
  AgentListEvent,
  AgentConfigurationChangedEvent,
  VoiceListEvent,
  AgentVoiceChangedEvent,
  ToolCatalogEvent,
  ChatUserDataEvent,
  GetUserSessionsResponseEvent,
  AvatarListEvent,
  AvatarConnectionChangedEvent,
  VoiceInputSupportedEvent,
  PongEvent,
  RenderMediaEvent,
} from '../../events/types/ServerEvents';

import type {
  // Client Events
  TextInputEvent,
  GetAgentsEvent,
  SetAgentEvent,
  GetAvatarsEvent,
  SetAvatarEvent,
  SetAvatarSessionEvent,
  ClearAvatarSessionEvent,
  SetAgentVoiceEvent,
  NewChatSessionEvent,
  ResumeChatSessionEvent,
  SetChatSessionNameEvent,
  SetSessionMetadataEvent,
  SetSessionMessagesEvent,
  GetUserSessionsEvent,
  GetVoicesEvent,
  GetToolCatalogEvent,
  PingEvent,
  PushToTalkStartEvent,
  PushToTalkEndEvent,
  SetVoiceInputModeEvent,
  DeleteChatSessionEvent,
} from '../../events/types/ClientEvents';

import type { Message } from '../../events/types/CommonTypes';

/**
 * Sample server events for testing
 */
export const serverEventFixtures = {
  systemPrompt: {
    type: 'system_prompt',
    session_id: 'test-session-123',
    role: 'system',
    content: '# Test System Prompt\n\nYou are a test assistant.',
    format: 'markdown',
  } as SystemPromptEvent,

  userRequest: {
    type: 'user_request',
    session_id: 'test-session-123',
    role: 'user',
    data: {
      message: 'Hello, how can you help me?',
    },
  } as UserRequestEvent,

  textDelta: {
    type: 'text_delta',
    session_id: 'test-session-123',
    role: 'assistant',
    content: 'I can help you with ',
    format: 'text',
  } as TextDeltaEvent,

  thoughtDelta: {
    type: 'thought_delta',
    session_id: 'test-session-123',
    role: 'assistant (thought)',
    content: 'The user is asking for help...',
    format: 'text',
  } as ThoughtDeltaEvent,

  completionRunning: {
    type: 'completion',
    session_id: 'test-session-123',
    role: 'assistant',
    running: true,
    completion_options: {
      model: 'claude-sonnet-4-20250514',
      max_tokens: 64000,
      temperature: 1,
    },
    stop_reason: undefined,
    input_tokens: 0,
    output_tokens: 0,
  } as CompletionEvent,

  completionFinished: {
    type: 'completion',
    session_id: 'test-session-123',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-sonnet-4-20250514',
      max_tokens: 64000,
      temperature: 1,
    },
    stop_reason: 'stop',
    input_tokens: 1234,
    output_tokens: 567,
  } as CompletionEvent,

  interactionStart: {
    type: 'interaction',
    session_id: 'test-session-123',
    role: 'assistant',
    started: true,
    id: 'interaction-456',
  } as InteractionEvent,

  interactionEnd: {
    type: 'interaction',
    session_id: 'test-session-123',
    role: 'assistant',
    started: false,
    id: 'interaction-456',
  } as InteractionEvent,

  history: {
    type: 'history',
    session_id: 'test-session-123',
    role: 'system',
    messages: [
      {
        role: 'user',
        content: 'Previous question',
        timestamp: '2025-01-01T10:00:00.000Z',
      },
      {
        role: 'assistant',
        content: 'Previous answer',
        timestamp: '2025-01-01T10:00:01.000Z',
      },
    ],
  } as HistoryEvent,

  historyDelta: {
    type: 'history_delta',
    session_id: 'test-session-123',
    role: 'assistant',
    messages: [
      {
        role: 'assistant',
        content: 'Here is my response',
        format: 'text',
        citations: null,
      },
    ],
  } as HistoryDeltaEvent,

  toolCall: {
    type: 'tool_call',
    session_id: 'test-session-123',
    role: 'assistant',
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'tool-call-789',
        type: 'function',
        function: {
          name: 'think',
          arguments: '{"thought":"Processing request..."}',
        },
      },
    ],
    tool_results: [],
  } as ToolCallEvent,

  toolSelectDelta: {
    type: 'tool_select_delta',
    session_id: 'test-session-123',
    role: 'assistant',
    tool_calls: [
      {
        id: 'tool-call-789',
        type: 'function',
        function: {
          name: 'calculate',
          arguments: '{}',
        },
      },
    ],
  } as ToolSelectDeltaEvent,

  systemMessage: {
    type: 'system_message',
    session_id: 'test-session-123',
    role: 'system',
    content: 'Connection established',
    format: 'text',
    severity: 'info',
  } as SystemMessageEvent,

  error: {
    type: 'error',
    message: 'Test error message',
    source: 'websocket',
  } as ErrorEvent,

  userTurnStart: {
    type: 'user_turn_start',
  } as UserTurnStartEvent,

  userTurnEnd: {
    type: 'user_turn_end',
  } as UserTurnEndEvent,

  serverListening: {
    type: 'server_listening',
  } as ServerListeningEvent,

  voiceList: {
    type: 'voice_list',
    voices: [
      {
        voice_id: 'nova',
        vendor: 'openai',
        description: 'Friendly voice',
        output_format: 'pcm16',
      },
      {
        voice_id: 'alloy',
        vendor: 'openai',
        description: 'Neutral voice',
        output_format: 'pcm16',
      },
    ],
  } as VoiceListEvent,

  agentVoiceChanged: {
    type: 'agent_voice_changed',
    voice: {
      voice_id: 'nova',
      vendor: 'openai',
      description: 'Friendly voice',
      output_format: 'pcm16',
    },
  } as AgentVoiceChangedEvent,

  pong: {
    type: 'pong',
  } as PongEvent,

  chatSessionChanged: {
    type: 'chat_session_changed',
    chat_session: {
      version: 2,
      session_id: 'test-session-123',
      token_count: 1500,
      context_window_size: 128000,
      session_name: 'Updated Chat Session',
      created_at: '2025-01-01T10:00:00.000Z',
      updated_at: '2025-01-01T12:00:00.000Z',
      user_id: 'user-abc123',
      metadata: {
        topic: 'technical discussion',
        tags: ['javascript', 'testing'],
      },
      messages: [],
      vendor: 'anthropic',
      display_name: 'Updated Chat Session',
    },
  } as ChatSessionChangedEvent,

  chatSessionAdded: {
    type: 'chat_session_added',
    chat_session: {
      session_id: 'new-session-456',
      session_name: 'New Chat Session',
      created_at: '2025-01-01T12:00:00.000Z',
      updated_at: '2025-01-01T12:00:00.000Z',
      user_id: 'user-abc123',
      agent_key: 'default',
      agent_name: 'Default Agent',
    },
  } as ChatSessionAddedEvent,

  chatSessionDeleted: {
    type: 'chat_session_deleted',
    session_id: 'deleted-session-789',
  } as ChatSessionDeletedEvent,

  chatSessionNameChanged: {
    type: 'chat_session_name_changed',
    session_name: 'Renamed Chat Session',
    session_id: 'test-session-123',
  } as ChatSessionNameChangedEvent,

  sessionMetadataChanged: {
    type: 'session_metadata_changed',
    meta: {
      updated_at: '2025-01-01T13:00:00.000Z',
      priority: 'high',
      status: 'active',
    },
  } as SessionMetadataChangedEvent,

  agentList: {
    type: 'agent_list',
    agents: [
      {
        name: 'Default Agent',
        key: 'default',
        agent_description: 'General purpose assistant',
        category: ['general', 'assistant'],
        tools: ['think', 'web_search'],
      },
      {
        name: 'Specialist Agent',
        key: 'specialist',
        agent_description: 'Domain-specific expert',
        category: ['specialized', 'technical'],
        tools: ['calculator', 'code_interpreter'],
      },
    ],
  } as AgentListEvent,

  agentConfigurationChanged: {
    type: 'agent_configuration_changed',
    agent_config: {
      version: 2,
      name: 'Default Agent',
      key: 'default',
      model_id: 'claude-3-5-sonnet-20241022',
      agent_description: 'General purpose assistant',
      tools: ['think', 'web_search'],
      agent_params: {
        type: 'claude_non_reasoning',
        model_name: 'claude-3-5-sonnet-20241022',
        max_tokens: 64000,
        temperature: 0.7,
      },
      persona: 'You are a helpful assistant.',
      category: ['general', 'assistant'],
    },
  } as AgentConfigurationChangedEvent,

  toolCatalog: {
    type: 'tool_catalog',
    tools: [
      {
        name: 'math_tools',
        description: 'Mathematical tools',
        schemas: {
          calculator: {
            type: 'function',
            function: {
              name: 'calculator',
              description: 'Perform mathematical calculations',
              parameters: {
                type: 'object',
                properties: {
                  expression: {
                    type: 'string',
                    description: 'Mathematical expression to evaluate',
                  },
                },
                required: ['expression'],
              },
            },
          },
        },
      },
      {
        name: 'search_tools',
        description: 'Search and research tools',
        schemas: {
          web_search: {
            type: 'function',
            function: {
              name: 'web_search',
              description: 'Search the web for information',
              parameters: {
                type: 'object',
                properties: {
                  query: {
                    type: 'string',
                    description: 'Search query',
                  },
                },
                required: ['query'],
              },
            },
          },
        },
      },
    ],
  } as ToolCatalogEvent,

  chatUserData: {
    type: 'chat_user_data',
    user: {
      user_id: 'user-abc123',
      user_name: 'testuser',
      email: 'test@example.com',
      first_name: 'Test',
      last_name: 'User',
      is_active: true,
      roles: ['user', 'beta_tester'],
      groups: ['general'],
      created_at: '2024-01-01T00:00:00.000Z',
      last_login: '2025-01-01T10:00:00.000Z',
    },
  } as ChatUserDataEvent,

  getUserSessionsResponse: {
    type: 'get_user_sessions_response',
    sessions: {
      chat_sessions: [
        {
          session_id: 'session-1',
          session_name: 'Morning Discussion',
          created_at: '2025-01-01T08:00:00.000Z',
          updated_at: '2025-01-01T09:30:00.000Z',
          user_id: 'user-abc123',
          agent_key: 'default',
          agent_name: 'Default Agent',
        },
        {
          session_id: 'session-2',
          session_name: 'Technical Review',
          created_at: '2025-01-01T14:00:00.000Z',
          updated_at: '2025-01-01T15:45:00.000Z',
          user_id: 'user-abc123',
          agent_key: 'specialist',
          agent_name: 'Specialist Agent',
        },
      ],
      total_sessions: 2,
      offset: 0,
    },
  } as GetUserSessionsResponseEvent,

  avatarList: {
    type: 'avatar_list',
    avatars: [
      {
        avatar_id: 'avatar-1',
        created_at: 1704067200000,
        default_voice: 'en-US-Neural2-A',
        is_public: true,
        normal_preview: 'https://example.com/avatar1-preview.jpg',
        pose_name: 'professional',
        status: 'active',
      },
      {
        avatar_id: 'avatar-2',
        created_at: 1704153600000,
        default_voice: 'en-US-Neural2-F',
        is_public: false,
        normal_preview: 'https://example.com/avatar2-preview.jpg',
        pose_name: 'casual',
        status: 'active',
      },
    ],
  } as AvatarListEvent,

  avatarConnectionChanged: {
    type: 'avatar_connection_changed',
    avatar_session_request: {
      avatar_id: 'avatar-1',
      quality: 'hd',
      voice: { voice_id: 'en-US-Neural2-A' },
      language: 'en',
      version: 'v2',
      video_encoding: 'h264',
      source: 'webrtc',
      stt_settings: null,
      ia_is_livekit_transport: false,
      knowledge_base: null,
      knowledge_base_id: null,
      disable_idle_timeout: false,
      activity_idle_timeout: 300,
    },
    avatar_session: {
      session_id: 'avatar-session-xyz',
      url: 'wss://avatar.heygen.com/session',
      access_token: 'token-abc123',
      session_duration_limit: 3600,
      is_paid: true,
      realtime_endpoint: 'wss://realtime.heygen.com',
      sdp: null,
      ice_servers: [],
      ice_servers2: [],
    },
  } as AvatarConnectionChangedEvent,

  voiceInputSupported: {
    type: 'voice_input_supported',
    modes: ['ptt', 'vad'],
  } as VoiceInputSupportedEvent,

  renderMedia: {
    type: 'render_media',
    session_id: 'test-session-123',
    role: 'assistant',
    content_type: 'text/html',
    content: '<div>Rendered content</div>',
    sent_by_class: 'VisualizationTool',
    sent_by_function: 'render_chart',
    foreign_content: false,  // SECURITY: Content is internally generated
    url: undefined,
    name: 'Chart Visualization'
  } as RenderMediaEvent,

  renderMediaForeign: {
    type: 'render_media',
    session_id: 'test-session-123',
    role: 'assistant',
    content_type: 'text/html',
    content: '<iframe src="https://external.com"></iframe>',
    sent_by_class: 'WebScraperTool',
    sent_by_function: 'fetch_external_content',
    foreign_content: true,  // SECURITY: Content from external source - MUST SANITIZE
    url: 'https://external.com/content',
    name: 'External Content'
  } as RenderMediaEvent,
};

/**
 * Sample client events for testing
 */
export const clientEventFixtures = {
  textInput: {
    type: 'text_input',
    text: 'Hello, world!',
    file_ids: [],
  } as TextInputEvent,

  getAgents: {
    type: 'get_agents',
  } as GetAgentsEvent,

  setAgent: {
    type: 'set_agent',
    agent_key: 'default',
  } as SetAgentEvent,

  setAgentVoice: {
    type: 'set_agent_voice',
    voice_id: 'nova',
  } as SetAgentVoiceEvent,

  newChatSession: {
    type: 'new_chat_session',
    agent_key: 'default',
  } as NewChatSessionEvent,

  resumeChatSession: {
    type: 'resume_chat_session',
    session_id: 'test-session-123',
  } as ResumeChatSessionEvent,

  ping: {
    type: 'ping',
  } as PingEvent,

  pttStart: {
    type: 'ptt_start',
  } as PushToTalkStartEvent,

  pttEnd: {
    type: 'ptt_end',
  } as PushToTalkEndEvent,

  setVoiceInputMode: {
    type: 'set_voice_input_mode',
    mode: 'ptt',
  } as SetVoiceInputModeEvent,

  getAvatars: {
    type: 'get_avatars',
  } as GetAvatarsEvent,

  setAvatar: {
    type: 'set_avatar',
    avatar_id: 'avatar-1',
    quality: 'hd',
    video_encoding: 'H264',
  } as SetAvatarEvent,

  setAvatarSession: {
    type: 'set_avatar_session',
    access_token: 'avatar-access-token-xyz',
    avatar_session_id: 'avatar-session-xyz',
  } as SetAvatarSessionEvent,

  clearAvatarSession: {
    type: 'clear_avatar_session',
    session_id: 'avatar-session-xyz',
  } as ClearAvatarSessionEvent,

  setChatSessionName: {
    type: 'set_chat_session_name',
    session_name: 'Updated Session Name',
    session_id: 'test-session-123',
  } as SetChatSessionNameEvent,

  setSessionMetadata: {
    type: 'set_session_metadata',
    meta: {
      topic: 'development',
      priority: 'high',
      tags: ['urgent', 'review'],
    },
  } as SetSessionMetadataEvent,

  setSessionMessages: {
    type: 'set_session_messages',
    messages: [
      {
        role: 'user',
        content: 'What is TypeScript?',
        timestamp: '2025-01-01T10:00:00.000Z',
      },
      {
        role: 'assistant',
        content: 'TypeScript is a typed superset of JavaScript.',
        timestamp: '2025-01-01T10:00:05.000Z',
      },
    ],
  } as SetSessionMessagesEvent,

  getUserSessions: {
    type: 'get_user_sessions',
    offset: 0,
    limit: 50,
  } as GetUserSessionsEvent,

  getVoices: {
    type: 'get_voices',
  } as GetVoicesEvent,

  getToolCatalog: {
    type: 'get_tool_catalog',
  } as GetToolCatalogEvent,

  deleteChatSession: {
    type: 'delete_chat_session',
    session_id: 'session-to-delete',
  } as DeleteChatSessionEvent,
};

/**
 * Sample messages with multimodal content
 */
export const messageFixtures = {
  textMessage: {
    role: 'user',
    content: 'Simple text message',
    timestamp: '2025-01-01T10:00:00.000Z',
  } as Message,

  multimodalMessage: {
    role: 'user',
    content: [
      {
        type: 'text',
        text: 'Look at this image',
      },
      {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==',
        },
      },
    ],
    timestamp: '2025-01-01T10:00:00.000Z',
  } as Message,

  assistantWithCitations: {
    role: 'assistant',
    content: 'Here is the answer',
    format: 'text',
    citations: [
      {
        quote: 'Important fact',
        source: 'https://example.com',
        metadata: { page: 1 },
      },
    ],
  } as Message,

  toolUseMessage: {
    role: 'assistant',
    content: [
      {
        type: 'text',
        text: 'Let me calculate that',
      },
      {
        type: 'tool_use',
        id: 'tool-use-123',
        name: 'calculator',
        input: { expression: '2 + 2' },
      },
    ],
  } as Message,

  toolResultMessage: {
    role: 'user',
    content: [
      {
        type: 'tool_result',
        tool_use_id: 'tool-use-123',
        content: '4',
        is_error: false,
      },
    ],
  } as Message,
};

/**
 * Binary audio data for testing audio events
 */
export const audioFixtures = {
  // PCM16 audio data (16-bit signed integer, little-endian)
  // This is just sample data - replace with actual PCM16 data for real tests
  pcm16Chunk: new ArrayBuffer(320), // 10ms of 16kHz mono PCM16
  
  // Simulate receiving audio output from server
  audioOutputFrame: new ArrayBuffer(640), // 20ms of 16kHz mono PCM16
};

/**
 * Test sequences of events
 */
export const eventSequences = {
  // Typical text interaction flow
  textInteraction: [
    serverEventFixtures.interactionStart,
    serverEventFixtures.userRequest,
    serverEventFixtures.completionRunning,
    serverEventFixtures.textDelta,
    serverEventFixtures.textDelta,
    serverEventFixtures.historyDelta,
    serverEventFixtures.completionFinished,
    serverEventFixtures.interactionEnd,
  ],

  // Tool use flow
  toolUseFlow: [
    serverEventFixtures.interactionStart,
    serverEventFixtures.userRequest,
    serverEventFixtures.completionRunning,
    serverEventFixtures.toolSelectDelta,
    serverEventFixtures.toolCall,
    serverEventFixtures.historyDelta,
    serverEventFixtures.completionFinished,
    serverEventFixtures.interactionEnd,
  ],

  // Voice interaction flow with turn management
  voiceInteraction: [
    serverEventFixtures.userTurnStart,
    serverEventFixtures.serverListening,
    // Audio input would be sent as binary frames here
    serverEventFixtures.userTurnEnd,
    serverEventFixtures.interactionStart,
    serverEventFixtures.completionRunning,
    // Audio output would be received as binary frames here
    serverEventFixtures.completionFinished,
    serverEventFixtures.interactionEnd,
    serverEventFixtures.userTurnStart,
  ],

  // Connection initialization flow
  connectionInit: [
    serverEventFixtures.systemMessage,
    serverEventFixtures.voiceList,
    // Additional initialization events would follow
  ],
};

/**
 * Helper to create a mock WebSocket message event
 */
export function createMockMessageEvent(data: unknown): MessageEvent {
  return new MessageEvent('message', {
    data: typeof data === 'string' ? data : JSON.stringify(data),
    origin: 'wss://api.example.com',
  });
}

/**
 * Helper to create a mock binary message event
 */
export function createMockBinaryEvent(buffer: ArrayBuffer): MessageEvent {
  return new MessageEvent('message', {
    data: buffer,
    origin: 'wss://api.example.com',
  });
}