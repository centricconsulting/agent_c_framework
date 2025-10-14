/**
 * Common type definitions used throughout the Agent C Realtime API
 * These types are shared between client and server events
 */

/**
 * User information returned from authentication
 */
export interface User {
  user_id: string;
  user_name: string;
  email: string | null;
  first_name: string | null;
  last_name: string | null;
  is_active: boolean;
  roles: string[];
  groups: string[];
  created_at: string | null;
  last_login: string | null;
}

/**
 * Agent configuration information
 */
export interface Agent {
  name: string;
  key: string;
  agent_description: string | null;
  category: string[];
  tools: string[];
}

// Re-export AgentConfiguration from chat-session types
export type { AgentConfiguration, CurrentAgentConfiguration, AgentConfigurationV2 } from '../../types/chat-session';

/**
 * Avatar information from HeyGen
 */
export interface Avatar {
  avatar_id: string;
  created_at: number;
  default_voice: string;
  is_public: boolean;
  normal_preview: string;
  pose_name: string;
  status: string;
}

/**
 * Avatar session request configuration
 */
export interface AvatarSessionRequest {
  avatar_id: string | null;
  quality: string | null;
  voice: Record<string, any> | null;
  language: string | null;
  version: string | null;
  video_encoding: string | null;
  source: string | null;
  stt_settings: Record<string, any> | null;
  ia_is_livekit_transport: boolean | null;
  knowledge_base: string | null;
  knowledge_base_id: string | null;
  disable_idle_timeout: boolean | null;
  activity_idle_timeout: number | null;
}

/**
 * Avatar session information
 */
export interface AvatarSession {
  session_id: string;
  url: string;
  access_token: string;
  session_duration_limit: number;
  is_paid: boolean;
  realtime_endpoint: string;
  sdp: string | null;
  ice_servers: unknown[] | null;
  ice_servers2: unknown[] | null;
}

/**
 * Voice model configuration
 */
export interface Voice {
  voice_id: string;
  vendor: string;
  description: string;
  output_format: string;
}

/**
 * Tool information
 */
export interface Tool {
  name: string;
  description: string;
  schemas: Record<string, ToolSchema>;
}

/**
 * Tool schema definition
 */
export interface ToolSchema {
  type: 'function';
  function: {
    name: string;
    description: string;
    parameters: {
      type: 'object';
      properties: Record<string, any>;
      required?: string[];
    };
  };
}

/**
 * Content part types for multimodal messages
 */
export type ContentPartType = 'text' | 'image' | 'audio' | 'video' | 'tool_use' | 'tool_result';

/**
 * Base content part interface
 */
export interface BaseContentPart {
  type: ContentPartType;
}

/**
 * Text content part
 */
export interface TextContentPart extends BaseContentPart {
  type: 'text';
  text: string;
}

/**
 * Image source types
 */
export interface ImageSource {
  type: 'base64' | 'url';
  media_type?: string; // e.g., 'image/png', 'image/jpeg'
  data?: string; // base64 encoded data (when type is 'base64')
  url?: string; // URL to image (when type is 'url')
}

/**
 * Image content part
 */
export interface ImageContentPart extends BaseContentPart {
  type: 'image';
  source: ImageSource;
}

/**
 * Tool use content part (for Anthropic-style tool calls in messages)
 */
export interface ToolUseContentPart extends BaseContentPart {
  type: 'tool_use';
  id: string;
  name: string;
  input: Record<string, any>;
}

/**
 * Tool result content part (for Anthropic-style tool results in messages)
 */
export interface ToolResultContentPart extends BaseContentPart {
  type: 'tool_result';
  tool_use_id: string;
  content: string | ContentPart[];
  is_error?: boolean;
}

/**
 * Union type for all content part types
 */
export type ContentPart = 
  | TextContentPart 
  | ImageContentPart 
  | ToolUseContentPart 
  | ToolResultContentPart;

/**
 * Message content can be either:
 * - A simple string (for text-only messages)
 * - An array of content parts (for multimodal messages)
 * - A null value (for messages with only citations or other metadata)
 */
export type MessageContent = string | ContentPart[] | null;

/**
 * Citation information for messages
 */
export interface Citation {
  quote?: string;
  source?: string;
  metadata?: Record<string, any>;
}

/**
 * Chat message structure
 * Supports both simple text messages and multimodal content
 */
export interface Message {
  role: 'user' | 'assistant' | 'system' | 'assistant (thought)';
  content: MessageContent;
  timestamp?: string;
  format?: 'text' | 'markdown';
  citations?: Citation[] | null;
  model_id?: string; // e.g., 'claude-3-5-sonnet-20241022' for vendor-specific handling
  name?: string; // Optional name field for role-play or multi-agent scenarios
}

// Re-export ChatSession types from chat-session module
export type { ChatSession, ChatSessionIndexEntry, ChatSessionQueryResponse } from '../../types/chat-session';

/**
 * Tool call structure for function calling
 */
export interface ToolCall {
  id: string;
  type: 'tool_use';
  name: string;
  input: Record<string, unknown>;
}

/**
 * Tool result structure
 */
export interface ToolResult {
  type: 'tool_result';
  tool_use_id: string;
  content: string;
}

/**
 * Completion options from various vendors
 */
export interface CompletionOptions {
  [key: string]: unknown;
}

/**
 * Stop reason for completions
 */
export type StopReason = 'stop' | 'length' | 'tool_calls' | 'cancelled';

/**
 * Severity levels for system messages
 */
export type Severity = 'info' | 'warning' | 'error';

/**
 * Message format types
 */
export type MessageFormat = 'text' | 'markdown';

/**
 * File upload response from server
 */
export interface UserFile {
  /** Unique identifier for the uploaded file */
  id: string;
  /** Original filename */
  filename: string;
  /** MIME type of the file */
  mime_type: string;
  /** File size in bytes */
  size: number;
}

/**
 * Type alias for API response compatibility
 */
export type UserFileResponse = UserFile;

/**
 * Options for file upload operations
 */
export interface FileUploadOptions {
  /** Progress callback for upload tracking */
  onProgress?: (progress: UploadProgress) => void;
  /** Abort signal for cancellation */
  signal?: AbortSignal;
}

/**
 * Upload progress information
 */
export interface UploadProgress {
  /** Bytes uploaded so far */
  loaded: number;
  /** Total bytes to upload */
  total: number;
  /** Progress percentage (0-100) */
  percentage: number;
}

/**
 * Login response structure
 * NOTE: Breaking change - login now only returns tokens and session ID.
 * All configuration data (user, agents, avatars, voices, tools, sessions) 
 * now comes through WebSocket events after connection.
 */
export interface LoginResponse {
  agent_c_token: string;
  heygen_token: string;
  ui_session_id: string;
}

/**
 * Token refresh response
 */
export interface RefreshTokenResponse {
  agent_c_token: string;
  heygen_token: string;
}