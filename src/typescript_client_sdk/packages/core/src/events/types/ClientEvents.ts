/**
 * Client-to-server event definitions for the Agent C Realtime API
 * These events are sent from the client to control the server
 */

import { Message } from './CommonTypes';

/**
 * Base interface for all client events
 */
export interface BaseClientEvent {
  type: string;
}

/**
 * Request list of available agents
 */
export interface GetAgentsEvent extends BaseClientEvent {
  type: 'get_agents';
}

/**
 * Set the active agent for the session
 */
export interface SetAgentEvent extends BaseClientEvent {
  type: 'set_agent';
  agent_key: string;
}

/**
 * Request list of available avatars
 */
export interface GetAvatarsEvent extends BaseClientEvent {
  type: 'get_avatars';
}

/**
 * Set the avatar for the current session
 */
export interface SetAvatarEvent extends BaseClientEvent {
  type: 'set_avatar';
  avatar_id: string;           // The ID of the avatar to set
  quality?: string;            // Optional quality setting (default: "auto")
  video_encoding?: string;     // Optional video encoding (default: "H265")
}

/**
 * Connect to an existing avatar session
 */
export interface SetAvatarSessionEvent extends BaseClientEvent {
  type: 'set_avatar_session';
  access_token: string;      // HeyGen access token for the session
  avatar_session_id: string;  // HeyGen avatar session ID
}

/**
 * Clear avatar session when HeyGen session ends
 */
export interface ClearAvatarSessionEvent extends BaseClientEvent {
  type: 'clear_avatar_session';
  session_id: string;
}

/**
 * Set the TTS voice for the current agent
 */
export interface SetAgentVoiceEvent extends BaseClientEvent {
  type: 'set_agent_voice';
  voice_id: string;
}

/**
 * Send text message to agent
 */
export interface TextInputEvent extends BaseClientEvent {
  type: 'text_input';
  text: string;
  file_ids?: string[];
}

/**
 * Create a new chat session
 */
export interface NewChatSessionEvent extends BaseClientEvent {
  type: 'new_chat_session';
  agent_key?: string;
}

/**
 * Resume an existing chat session
 */
export interface ResumeChatSessionEvent extends BaseClientEvent {
  type: 'resume_chat_session';
  session_id: string;
}

/**
 * Set the name/title for a chat session
 */
export interface SetChatSessionNameEvent extends BaseClientEvent {
  type: 'set_chat_session_name';
  session_name: string;
  session_id?: string; // Optional - if not set, applies to current session
}

/**
 * Update session metadata
 */
export interface SetSessionMetadataEvent extends BaseClientEvent {
  type: 'set_session_metadata';
  meta: Record<string, any>;
}

/**
 * Replace all messages in the current session
 */
export interface SetSessionMessagesEvent extends BaseClientEvent {
  type: 'set_session_messages';
  messages: Message[];
}

/**
 * Request paginated list of user sessions
 * Used for loading additional sessions beyond the initial login response
 */
export interface GetUserSessionsEvent extends BaseClientEvent {
  type: 'get_user_sessions';
  offset: number; // Starting offset for pagination (default 0)
  limit: number;  // Number of sessions to fetch (default 50)
}

/**
 * Request list of available TTS voices
 * Server responds with voice_list event
 */
export interface GetVoicesEvent extends BaseClientEvent {
  type: 'get_voices';
}

/**
 * Request tool catalog with available tools and schemas
 * Server responds with tool_catalog event
 */
export interface GetToolCatalogEvent extends BaseClientEvent {
  type: 'get_tool_catalog';
}

/**
 * Connection health check ping
 * Server responds with pong event
 */
export interface PingEvent extends BaseClientEvent {
  type: 'ping';
}

/**
 * Notify server that client is starting push-to-talk audio input
 */
export interface PushToTalkStartEvent extends BaseClientEvent {
  type: 'ptt_start';
}

/**
 * Notify server that client has finished push-to-talk audio input
 */
export interface PushToTalkEndEvent extends BaseClientEvent {
  type: 'ptt_end';
}

/**
 * Set the voice input mode for the session
 */
export interface SetVoiceInputModeEvent extends BaseClientEvent {
  type: 'set_voice_input_mode';
  mode: 'ptt' | 'vad';
}

/**
 * Delete a chat session belonging to the user
 */
export interface DeleteChatSessionEvent extends BaseClientEvent {
  type: 'delete_chat_session';
  session_id?: string; // Optional - if not set, deletes the current session
}

/**
 * Request to cancel the current agent response
 * Client sends this to notify it wants to cancel the current agent response
 */
export interface ClientWantsCancelEvent extends BaseClientEvent {
  type: 'client_wants_cancel';
}

/**
 * Union type of all client events
 */
export type ClientEvent =
  | GetAgentsEvent
  | SetAgentEvent
  | GetAvatarsEvent
  | SetAvatarEvent
  | SetAvatarSessionEvent
  | ClearAvatarSessionEvent
  | SetAgentVoiceEvent
  | TextInputEvent
  | NewChatSessionEvent
  | ResumeChatSessionEvent
  | SetChatSessionNameEvent
  | SetSessionMetadataEvent
  | SetSessionMessagesEvent
  | GetUserSessionsEvent
  | GetVoicesEvent
  | GetToolCatalogEvent
  | PingEvent
  | PushToTalkStartEvent
  | PushToTalkEndEvent
  | SetVoiceInputModeEvent
  | DeleteChatSessionEvent
  | ClientWantsCancelEvent;

/**
 * Type guard to check if an object is a client event
 */
export function isClientEvent(obj: unknown): obj is ClientEvent {
  return !!obj && typeof obj === 'object' && 'type' in obj && typeof (obj as any).type === 'string' && [
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
    'ptt_start',
    'ptt_end',
    'set_voice_input_mode',
    'delete_chat_session',
    'client_wants_cancel'
  ].includes((obj as any).type);
}