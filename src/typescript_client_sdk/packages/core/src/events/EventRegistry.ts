/**
 * Event Registry - Type mappings between event type strings and interfaces
 * This provides a centralized mapping for type-safe event handling
 */

import {
  GetAgentsEvent,
  SetAgentEvent,
  GetAvatarsEvent,
  SetAvatarEvent,
  SetAvatarSessionEvent,
  ClearAvatarSessionEvent,
  SetAgentVoiceEvent,
  TextInputEvent,
  NewChatSessionEvent,
  ResumeChatSessionEvent,
  SetChatSessionNameEvent,
  SetSessionMetadataEvent,
  SetSessionMessagesEvent,
  GetUserSessionsEvent,
  GetVoicesEvent,
  GetToolCatalogEvent,
  PingEvent,
  DeleteChatSessionEvent,
  ClientWantsCancelEvent,
  ClientEvent
} from './types/ClientEvents';

import {
  AgentListEvent,
  AgentConfigurationChangedEvent,
  AvatarListEvent,
  AvatarConnectionChangedEvent,
  ChatSessionChangedEvent,
  ChatSessionNameChangedEvent,
  SessionMetadataChangedEvent,
  TextDeltaEvent,
  ThoughtDeltaEvent,
  CompletionEvent,
  InteractionEvent,
  HistoryEvent,
  ToolCallEvent,
  SystemMessageEvent,
  UserTurnStartEvent,
  UserTurnEndEvent,
  AgentVoiceChangedEvent,
  ErrorEvent,
  GetUserSessionsResponseEvent,
  ChatUserDataEvent,
  UISessionIDChangedEvent,
  VoiceListEvent,
  ToolCatalogEvent,
  PongEvent,
  VoiceInputSupportedEvent,
  ServerListeningEvent,
  ChatSessionAddedEvent,
  ChatSessionDeletedEvent,
  ToolSelectDeltaEvent,
  RenderMediaEvent,
  HistoryDeltaEvent,
  SystemPromptEvent,
  UserRequestEvent,
  UserMessageEvent,
  OpenAIUserMessageEvent,
  AnthropicUserMessageEvent,
  SubsessionStartedEvent,
  SubsessionEndedEvent,
  CancelledEvent,
  ServerEvent
} from './types/ServerEvents';

/**
 * Mapping of client event types to their corresponding interfaces
 */
export interface ClientEventMap {
  'get_agents': GetAgentsEvent;
  'set_agent': SetAgentEvent;
  'get_avatars': GetAvatarsEvent;
  'set_avatar': SetAvatarEvent;
  'set_avatar_session': SetAvatarSessionEvent;
  'clear_avatar_session': ClearAvatarSessionEvent;
  'set_agent_voice': SetAgentVoiceEvent;
  'text_input': TextInputEvent;
  'new_chat_session': NewChatSessionEvent;
  'resume_chat_session': ResumeChatSessionEvent;
  'set_chat_session_name': SetChatSessionNameEvent;
  'set_session_metadata': SetSessionMetadataEvent;
  'set_session_messages': SetSessionMessagesEvent;
  'get_user_sessions': GetUserSessionsEvent;
  'get_voices': GetVoicesEvent;
  'get_tool_catalog': GetToolCatalogEvent;
  'ping': PingEvent;
  'delete_chat_session': DeleteChatSessionEvent;
  'client_wants_cancel': ClientWantsCancelEvent;
}

/**
 * Mapping of server event types to their corresponding interfaces
 */
export interface ServerEventMap {
  'agent_list': AgentListEvent;
  'agent_configuration_changed': AgentConfigurationChangedEvent;
  'avatar_list': AvatarListEvent;
  'avatar_connection_changed': AvatarConnectionChangedEvent;
  'chat_session_changed': ChatSessionChangedEvent;
  'chat_session_name_changed': ChatSessionNameChangedEvent;
  'session_metadata_changed': SessionMetadataChangedEvent;
  'text_delta': TextDeltaEvent;
  'thought_delta': ThoughtDeltaEvent;
  'completion': CompletionEvent;
  'interaction': InteractionEvent;
  'history': HistoryEvent;
  'tool_call': ToolCallEvent;
  'system_message': SystemMessageEvent;
  'user_turn_start': UserTurnStartEvent;
  'user_turn_end': UserTurnEndEvent;
  'agent_voice_changed': AgentVoiceChangedEvent;
  'error': ErrorEvent;
  'get_user_sessions_response': GetUserSessionsResponseEvent;
  'chat_user_data': ChatUserDataEvent;
  'ui_session_id_changed': UISessionIDChangedEvent;
  'voice_list': VoiceListEvent;
  'tool_catalog': ToolCatalogEvent;
  'pong': PongEvent;
  'voice_input_supported': VoiceInputSupportedEvent;
  'server_listening': ServerListeningEvent;
  'chat_session_added': ChatSessionAddedEvent;
  'chat_session_deleted': ChatSessionDeletedEvent;
  'tool_select_delta': ToolSelectDeltaEvent;
  'render_media': RenderMediaEvent;
  'history_delta': HistoryDeltaEvent;
  'system_prompt': SystemPromptEvent;
  'user_request': UserRequestEvent;
  'user_message': UserMessageEvent;
  'anthropic_user_message': AnthropicUserMessageEvent;
  'openai_user_message': OpenAIUserMessageEvent;
  'subsession_started': SubsessionStartedEvent;
  'subsession_ended': SubsessionEndedEvent;
  'cancelled': CancelledEvent;
}

/**
 * Combined event map for all events in the system
 */
export interface RealtimeEventMap extends ClientEventMap, ServerEventMap {
  // Special events for binary data and connection management
  'binary_audio': ArrayBuffer;  // Legacy event for backward compatibility
  'audio:output': ArrayBuffer;   // Binary audio output from server (TTS audio)
  'connected': void;
  'disconnected': { code: number; reason: string };
  'reconnecting': { attempt: number; delay: number };
  'reconnected': void;
  // WebSocket ping/pong events
  'ping': any;
  'pong': any;
}

/**
 * Type for all event types
 */
export type EventType = keyof RealtimeEventMap;

/**
 * Type for client event types
 */
export type ClientEventType = keyof ClientEventMap;

/**
 * Type for server event types
 */
export type ServerEventType = keyof ServerEventMap;

/**
 * Helper to get event data type from event type string
 */
export type EventData<T extends EventType> = RealtimeEventMap[T];

/**
 * Registry class for event type validation and conversion
 */
export class EventRegistry {
  private static readonly clientEventTypes = new Set<string>([
    'get_agents',
    'set_agent',
    'get_avatars',
    'set_avatar',
    'set_avatar_session',
    'clear_avatar_session',
    'set_agent_voice',
    'text_input',
    'new_chat_session',
    'resume_chat_session',
    'set_chat_session_name',
    'set_session_metadata',
    'set_session_messages',
    'get_user_sessions',
    'get_voices',
    'get_tool_catalog',
    'ping',
    'delete_chat_session',
    'client_wants_cancel'
  ]);

  private static readonly serverEventTypes = new Set<string>([
    'agent_list',
    'agent_configuration_changed',
    'avatar_list',
    'avatar_connection_changed',
    'chat_session_changed',
    'chat_session_name_changed',
    'session_metadata_changed',
    'text_delta',
    'thought_delta',
    'completion',
    'interaction',
    'history',
    'tool_call',
    'system_message',
    'user_turn_start',
    'user_turn_end',
    'agent_voice_changed',
    'error',
    'get_user_sessions_response',
    'chat_user_data',
    'ui_session_id_changed',
    'voice_list',
    'tool_catalog',
    'pong',
    'voice_input_supported',
    'server_listening',
    'chat_session_added',
    'chat_session_deleted',
    'tool_select_delta',
    'render_media',
    'history_delta',
    'system_prompt',
    'user_request',
    'user_message',
    'anthropic_user_message',
    'openai_user_message',
    'subsession_started',
    'subsession_ended',
    'cancelled'
  ]);

  /**
   * Check if a string is a valid client event type
   */
  static isClientEventType(type: string): type is ClientEventType {
    return this.clientEventTypes.has(type);
  }

  /**
   * Check if a string is a valid server event type
   */
  static isServerEventType(type: string): type is ServerEventType {
    return this.serverEventTypes.has(type);
  }

  /**
   * Check if a string is a valid event type
   */
  static isValidEventType(type: string): type is EventType {
    return this.isClientEventType(type) || 
           this.isServerEventType(type) ||
           ['binary_audio', 'audio:output', 'connected', 'disconnected', 'reconnecting', 'reconnected', 'ping', 'pong'].includes(type);
  }

  /**
   * Get all client event types
   */
  static getClientEventTypes(): ClientEventType[] {
    return Array.from(this.clientEventTypes) as ClientEventType[];
  }

  /**
   * Get all server event types
   */
  static getServerEventTypes(): ServerEventType[] {
    return Array.from(this.serverEventTypes) as ServerEventType[];
  }

  /**
   * Parse an event from JSON string or object
   */
  static parseEvent(data: string | any): ClientEvent | ServerEvent | null {
    try {
      const event = typeof data === 'string' ? JSON.parse(data) : data;
      
      if (!event || typeof event.type !== 'string') {
        return null;
      }

      if (this.isClientEventType(event.type) || this.isServerEventType(event.type)) {
        return event;
      }

      return null;
    } catch (error) {
      console.error('Failed to parse event:', error);
      return null;
    }
  }

  /**
   * Create a typed event object
   */
  static createEvent<T extends EventType>(
    type: T,
    data: Omit<EventData<T>, 'type'>
  ): EventData<T> {
    return { type, ...data } as EventData<T>;
  }
}