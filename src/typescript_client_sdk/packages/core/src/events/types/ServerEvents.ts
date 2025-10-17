/**
 * Server-to-client event definitions for the Agent C Realtime API
 * These events are sent from the server to update the client
 */

import {
  Agent,
  AgentConfiguration,
  Avatar,
  AvatarSessionRequest,
  AvatarSession,
  ChatSession,
  ChatSessionIndexEntry,
  ChatSessionQueryResponse,
  Voice,
  Message,
  ToolCall,
  ToolResult,
  CompletionOptions,
  StopReason,
  Severity,
  MessageFormat,
  User,
  Tool
} from './CommonTypes';

/**
 * Base interface for all server events
 */
export interface BaseServerEvent {
  type: string;
}

/**
 * Session events are generated within a chat session and always include session tracking fields.
 * All runtime events that are part of the interaction flow inherit from this base.
 * This matches the Python SessionEvent base class.
 */
export interface SessionEvent extends BaseServerEvent {
  /** The session ID for this event */
  session_id: string;
  /** The role that triggered this event */
  role: string;
  /** The parent session ID, if this session is a child session */
  parent_session_id?: string;
  /** The top level user session ID, if this session is a child session */
  user_session_id?: string;
}

/**
 * Response to get_agents with available agents
 */
export interface AgentListEvent extends BaseServerEvent {
  type: 'agent_list';
  agents: Agent[];
}

/**
 * Notification that agent configuration has changed
 */
export interface AgentConfigurationChangedEvent extends BaseServerEvent {
  type: 'agent_configuration_changed';
  agent_config: AgentConfiguration;
}

/**
 * Response to get_avatars with available avatars
 */
export interface AvatarListEvent extends BaseServerEvent {
  type: 'avatar_list';
  avatars: Avatar[];
}

/**
 * Notification that avatar connection status has changed
 */
export interface AvatarConnectionChangedEvent extends BaseServerEvent {
  type: 'avatar_connection_changed';
  avatar_session_request: AvatarSessionRequest;
  avatar_session: AvatarSession;
}

/**
 * Notification that chat session has been updated
 */
export interface ChatSessionChangedEvent extends BaseServerEvent {
  type: 'chat_session_changed';
  chat_session: ChatSession;
}

/**
 * Notification that session name was updated
 */
export interface ChatSessionNameChangedEvent extends BaseServerEvent {
  type: 'chat_session_name_changed';
  session_name: string;
  session_id?: string; // Optional - if not set, refers to current session
}

/**
 * Notification that session metadata was updated
 */
export interface SessionMetadataChangedEvent extends BaseServerEvent {
  type: 'session_metadata_changed';
  meta: Record<string, any>;
}

/**
 * Streaming text content from the agent
 */
export interface TextDeltaEvent extends SessionEvent {
  type: 'text_delta';
  role: 'assistant';
  content: string;
  format: MessageFormat;
}

/**
 * Streaming thought content from the agent
 */
export interface ThoughtDeltaEvent extends SessionEvent {
  type: 'thought_delta';
  role: 'assistant (thought)';
  content: string;
  format: MessageFormat;
}

/**
 * Completion status updates
 */
export interface CompletionEvent extends SessionEvent {
  type: 'completion';
  role: 'assistant';
  running: boolean;
  completion_options: CompletionOptions;
  stop_reason?: StopReason;
  input_tokens?: number;
  output_tokens?: number;
}

/**
 * Interaction lifecycle events
 */
export interface InteractionEvent extends SessionEvent {
  type: 'interaction';
  role: 'assistant';
  started: boolean;
  id: string;
}

/**
 * Complete message history update
 */
export interface HistoryEvent extends SessionEvent {
  type: 'history';
  role: 'system';
  vendor: string;
  messages: Message[];
}

/**
 * Tool execution events
 */
export interface ToolCallEvent extends SessionEvent {
  type: 'tool_call';
  role: 'assistant';
  active: boolean;
  vendor: string;
  tool_calls: ToolCall[];
  tool_results?: ToolResult[];
}

/**
 * System notifications and errors
 */
export interface SystemMessageEvent extends SessionEvent {
  type: 'system_message';
  role: 'system';
  content: string;
  format: MessageFormat;
  severity?: Severity;
}

/**
 * Notification that the server is ready to accept user input
 */
export interface UserTurnStartEvent extends BaseServerEvent {
  type: 'user_turn_start';
}

/**
 * Notification that the server has received user input and is no longer accepting input
 */
export interface UserTurnEndEvent extends BaseServerEvent {
  type: 'user_turn_end';
}

/**
 * Notification that the agent's voice has been updated
 */
export interface AgentVoiceChangedEvent extends BaseServerEvent {
  type: 'agent_voice_changed';
  voice: Voice;
}

/**
 * Error notifications
 */
export interface ErrorEvent extends BaseServerEvent {
  type: 'error';
  message: string;
  source?: string;
}

/**
 * Response to get_user_sessions request with paginated session list
 */
export interface GetUserSessionsResponseEvent extends BaseServerEvent {
  type: 'get_user_sessions_response';
  sessions: ChatSessionQueryResponse;
}

/**
 * User data sent during connection initialization
 * Contains current user profile and authentication info
 */
export interface ChatUserDataEvent extends BaseServerEvent {
  type: 'chat_user_data';
  user: User;
}

/**
 * UI Session ID notification sent during connection initialization
 * Updates the client with the current UI session ID to use for reconnection
 */
export interface UISessionIDChangedEvent extends BaseServerEvent {
  type: 'ui_session_id_changed';
  ui_session_id: string;
}

/**
 * Voice list sent during initialization or in response to get_voices
 * Contains complete voice model information
 */
export interface VoiceListEvent extends BaseServerEvent {
  type: 'voice_list';
  voices: Voice[];
}

/**
 * Tool catalog sent during initialization or in response to get_tool_catalog
 */
export interface ToolCatalogEvent extends BaseServerEvent {
  type: 'tool_catalog';
  tools: Tool[];
}

/**
 * Response to ping request for connection health check
 */
export interface PongEvent extends BaseServerEvent {
  type: 'pong';
}

/**
 * Notification of supported voice input modes
 */
export interface VoiceInputSupportedEvent extends BaseServerEvent {
  type: 'voice_input_supported';
  modes: ('ptt' | 'vad')[];
}

/**
 * Notification that server is listening for audio input
 */
export interface ServerListeningEvent extends BaseServerEvent {
  type: 'server_listening';
}

/**
 * Notification that a new chat session has been added
 * Note: New chat sessions don't get indexed and added to the list of user sessions
 * until there's been at least one message in the session
 */
export interface ChatSessionAddedEvent extends BaseServerEvent {
  type: 'chat_session_added';
  chat_session: ChatSessionIndexEntry;
}

/**
 * Notification that a chat session has been deleted
 */
export interface ChatSessionDeletedEvent extends BaseServerEvent {
  type: 'chat_session_deleted';
  session_id?: string; // Optional - if not set, refers to current session
}

/**
 * Tool selection notification
 * Fired when a tool is selected BEFORE parameters have streamed
 * Important for showing UI feedback that a tool is about to be used
 */
export interface ToolSelectDeltaEvent extends SessionEvent {
  type: 'tool_select_delta';
  role: 'assistant';
  tool_calls: ToolCall[];
}

/**
 * Special media rendering event from tools
 * Used when tools need to display rich media content beyond text
 */
export interface RenderMediaEvent extends SessionEvent {
  type: 'render_media';
  role: 'assistant';
  content_type: string;           // MIME type (REQUIRED)
  content?: string;                // Optional: Text or base64 encoded content
  url?: string;                    // Optional: URL to the media
  name?: string;                   // Optional: Name/title of the media content
  sent_by_class?: string;          // Optional: Tool class that sent the event
  sent_by_function?: string;       // Optional: Function that generated the event
  foreign_content: boolean;        // Security flag for external content
  // Remove content_bytes field - not sent over WebSocket JSON
}

/**
 * Incremental history update
 * Updates to the conversation history with the agent's response
 */
export interface HistoryDeltaEvent extends SessionEvent {
  type: 'history_delta';
  role: 'assistant';
  messages: Message[];   // Array of message objects with content and citations
}

/**
 * System prompt initialization event
 * Initializes the agent with system instructions, persona, and available tools
 */
export interface SystemPromptEvent extends SessionEvent {
  type: 'system_prompt';
  role: 'system';
  content: string;       // Full markdown content containing agent instructions
  format: MessageFormat; // Content format (typically "markdown")
}

/**
 * User request event
 * Generated by the agent runtime when processing user input
 */
export interface UserRequestEvent extends SessionEvent {
  type: 'user_request';
  role: 'user';
  data: {
    message: string;     // The user's message text
  };
}

/**
 * Base user message event with vendor information
 * Replaces UserRequestEvent for vendor-specific message formats
 */
export interface UserMessageEvent extends SessionEvent {
  type: 'user_message';
  vendor: string;
  /** The message content - structure varies by vendor */
  message?: string | Record<string, any>;
}

/**
 * OpenAI-specific user message event
 */
export interface OpenAIUserMessageEvent extends SessionEvent {
  type: 'openai_user_message';
  vendor: 'openai';
  message: Record<string, any>; // OpenAI message format
}

/**
 * Anthropic-specific user message event
 */
export interface AnthropicUserMessageEvent extends SessionEvent {
  type: 'anthropic_user_message';
  vendor: 'anthropic';
  message: Record<string, any>; // Anthropic message format
}

/**
 * Notification that a subsession has started
 * Events between this and SubsessionEndedEvent are part of the subsession
 */
export interface SubsessionStartedEvent extends SessionEvent {
  type: 'subsession_started';
  sub_session_type: 'chat' | 'oneshot';
  sub_agent_type: 'clone' | 'team' | 'assist' | 'tool';
  prime_agent_key: string;
  sub_agent_key: string;
}

/**
 * Notification that a subsession has ended
 * Matches with a previous SubsessionStartedEvent
 */
export interface SubsessionEndedEvent extends SessionEvent {
  type: 'subsession_ended';
}

/**
 * Notification that the current agent response has been cancelled
 * Server sends this to confirm the cancellation requested by the client
 */
export interface CancelledEvent extends BaseServerEvent {
  type: 'cancelled';
}

/**
 * Union type of all server events
 */
export type ServerEvent =
  | AgentListEvent
  | AgentConfigurationChangedEvent
  | AvatarListEvent
  | AvatarConnectionChangedEvent
  | ChatSessionChangedEvent
  | ChatSessionNameChangedEvent
  | SessionMetadataChangedEvent
  | TextDeltaEvent
  | ThoughtDeltaEvent
  | CompletionEvent
  | InteractionEvent
  | HistoryEvent
  | ToolCallEvent
  | SystemMessageEvent
  | UserTurnStartEvent
  | UserTurnEndEvent
  | AgentVoiceChangedEvent
  | ErrorEvent
  | GetUserSessionsResponseEvent
  | ChatUserDataEvent
  | UISessionIDChangedEvent
  | VoiceListEvent
  | ToolCatalogEvent
  | PongEvent
  | VoiceInputSupportedEvent
  | ServerListeningEvent
  | ChatSessionAddedEvent
  | ChatSessionDeletedEvent
  | ToolSelectDeltaEvent
  | RenderMediaEvent
  | HistoryDeltaEvent
  | SystemPromptEvent
  | UserRequestEvent
  | UserMessageEvent
  | OpenAIUserMessageEvent
  | AnthropicUserMessageEvent
  | SubsessionStartedEvent
  | SubsessionEndedEvent
  | CancelledEvent;

/**
 * Type guard to check if an object is a server event
 */
export function isServerEvent(obj: unknown): obj is ServerEvent {
  return !!obj && typeof obj === 'object' && 'type' in obj && typeof (obj as any).type === 'string' && [
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
  ].includes((obj as any).type);
}