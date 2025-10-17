# TypeScript Types API Reference

Complete type reference for the Agent C Realtime Client SDK. All types are exported from `@agentc/realtime-core`.

## Table of Contents

- [Message Types](#message-types)
  - [Universal Message Types](#universal-message-types)
  - [Anthropic Message Types](#anthropic-message-types)
  - [OpenAI Message Types](#openai-message-types)
  - [Runtime Message Types](#runtime-message-types)
- [Chat Session Types](#chat-session-types)
  - [Session Management](#session-management)
  - [Agent Configuration](#agent-configuration)
  - [Authentication Types](#authentication-types)
- [Event Types](#event-types)
  - [Client Events](#client-events)
  - [Server Events](#server-events)
  - [Common Event Types](#common-event-types)
- [File Upload Types](#file-upload-types)
  - [Upload Response](#upload-response)
  - [Upload Options](#upload-options)
  - [Upload Progress](#upload-progress)
- [Audio Types](#audio-types)
  - [Audio Processing](#audio-processing)
  - [Audio Service](#audio-service)
  - [Audio Configuration](#audio-configuration)
- [Connection Types](#connection-types)
  - [Client Configuration](#client-configuration)
  - [Connection State](#connection-state)
  - [Reconnection](#reconnection)
- [Authentication Types](#authentication-types-2)
  - [Auth Configuration](#auth-configuration)
  - [Token Management](#token-management)
- [Utility Types](#utility-types)
  - [Common Patterns](#common-patterns)
  - [Type Guards](#type-guards)
  - [Logging](#logging)

---

## Message Types

The SDK supports both Anthropic and OpenAI message formats through a unified interface. The vendor field in chat sessions determines which format is used.

### Universal Message Types

Types that work with both Anthropic and OpenAI message formats:

```typescript
/**
 * Unified message type supporting both vendors
 * The vendor field in the chat session determines the actual format:
 * - vendor: "anthropic" → MessageParam
 * - vendor: "openai" → ChatCompletionMessageParam
 */
export type UnifiedMessageParam = MessageParam | ChatCompletionMessageParam;

// Type guards for vendor detection
export function isAnthropicMessage(message: UnifiedMessageParam): message is MessageParam;
export function isOpenAIMessage(message: UnifiedMessageParam): message is ChatCompletionMessageParam;

// Format detection helpers
export function getMessageFormat(vendor: string): 'anthropic' | 'openai' | 'unknown';

// Conversion functions
export function toAnthropicMessage(message: UnifiedMessageParam): MessageParam | null;
export function toOpenAIMessage(message: UnifiedMessageParam): ChatCompletionMessageParam | null;
```

### Anthropic Message Types

Core message structure for Anthropic's Claude models:

```typescript
/**
 * Anthropic message parameter
 */
export interface MessageParam {
  content: string | Array<ContentBlockParam>;
  role: "user" | "assistant";
}

/**
 * Content block union type for rich content
 */
export type ContentBlockParam = 
  | TextBlockParam
  | ImageBlockParam
  | DocumentBlockParam
  | SearchResultBlockParam
  | ThinkingBlockParam
  | RedactedThinkingBlockParam
  | ToolUseBlockParam
  | ToolResultBlockParam
  | ServerToolUseBlockParam
  | WebSearchToolResultBlockParam;

/**
 * Text content block
 */
export interface TextBlockParam {
  text: string;
  type: "text";
  cache_control?: CacheControlEphemeralParam;
  citations?: TextCitationParam[];
}

/**
 * Image content block
 */
export interface ImageBlockParam {
  source: Base64ImageSourceParam | URLImageSourceParam;
  type: "image";
  cache_control?: CacheControlEphemeralParam;
}

/**
 * Tool use block for function calling
 */
export interface ToolUseBlockParam {
  id: string;
  input: Record<string, any>;
  name: string;
  type: "tool_use";
  cache_control?: CacheControlEphemeralParam;
}

/**
 * Tool result block
 */
export interface ToolResultBlockParam {
  tool_use_id: string;
  type: "tool_result";
  cache_control?: CacheControlEphemeralParam;
  content?: string | ToolResultContent[];
  is_error?: boolean;
}

/**
 * Document block for PDF and text documents
 */
export interface DocumentBlockParam {
  source: DocumentSource;
  type: "document";
  cache_control?: CacheControlEphemeralParam;
  citations?: CitationsConfigParam;
  context?: string;
  title?: string;
}

/**
 * Cache control for ephemeral caching
 */
export interface CacheControlEphemeralParam {
  type: "ephemeral";
  ttl?: "5m" | "1h"; // Time-to-live: 5 minutes or 1 hour
}
```

### OpenAI Message Types

Core message structure for OpenAI's GPT models:

```typescript
/**
 * Union of all OpenAI message types
 */
export type ChatCompletionMessageParam = 
  | ChatCompletionDeveloperMessageParam
  | ChatCompletionSystemMessageParam
  | ChatCompletionUserMessageParam
  | ChatCompletionAssistantMessageParam
  | ChatCompletionToolMessageParam
  | ChatCompletionFunctionMessageParam;

/**
 * User message with multimodal support
 */
export interface ChatCompletionUserMessageParam {
  role: "user";
  content: string | ChatCompletionContentPart[];
  name?: string;
}

/**
 * Assistant message with tool calls and audio
 */
export interface ChatCompletionAssistantMessageParam {
  role: "assistant";
  content?: string | ContentArrayOfContentPart[] | null;
  name?: string;
  refusal?: string | null;
  tool_calls?: ChatCompletionMessageToolCallUnion[];
  function_call?: FunctionCall | null; // @deprecated
  audio?: Audio | null;
}

/**
 * System/Developer messages for instructions
 */
export interface ChatCompletionSystemMessageParam {
  role: "system";
  content: string | ChatCompletionContentPartText[];
  name?: string;
}

export interface ChatCompletionDeveloperMessageParam {
  role: "developer";
  content: string | ChatCompletionContentPartText[];
  name?: string;
}

/**
 * Content parts for multimodal messages
 */
export type ChatCompletionContentPart = 
  | ChatCompletionContentPartText
  | ChatCompletionContentPartImage
  | ChatCompletionContentPartInputAudio
  | ChatCompletionContentPartFile;

/**
 * Image content with detail levels
 */
export interface ChatCompletionContentPartImage {
  type: "image_url";
  image_url: {
    url: string; // URL or base64 data
    detail?: "auto" | "low" | "high";
  };
}

/**
 * Audio input content
 */
export interface ChatCompletionContentPartInputAudio {
  type: "input_audio";
  input_audio: {
    data: string; // Base64 encoded
    format: "wav" | "mp3";
  };
}

/**
 * Tool/Function calls
 */
export interface ChatCompletionMessageFunctionToolCall {
  id: string;
  type: "function";
  function: {
    name: string;
    arguments: string; // JSON string
  };
}
```

### Runtime Message Types

Messages as they appear during runtime with additional metadata:

```typescript
/**
 * Runtime message with full metadata
 */
export interface Message {
  role: 'user' | 'assistant' | 'system' | 'assistant (thought)';
  content: MessageContent;
  timestamp?: string;
  format?: 'text' | 'markdown';
  citations?: Citation[] | null;
  model_id?: string; // For vendor-specific handling
  name?: string; // For multi-agent scenarios
}

/**
 * Message content types
 */
export type MessageContent = string | ContentPart[] | null;

/**
 * Content part for multimodal messages
 */
export type ContentPart = 
  | TextContentPart 
  | ImageContentPart 
  | ToolUseContentPart 
  | ToolResultContentPart;

/**
 * Citation information
 */
export interface Citation {
  quote?: string;
  source?: string;
  metadata?: Record<string, any>;
}

// Conversion utilities
export function unifiedMessageParamToMessage(param: UnifiedMessageParam): Message;
export function messageToUnifiedMessageParam(message: Message, vendor: string): UnifiedMessageParam;
```

## Chat Session Types

### Session Management

Types for managing chat sessions:

```typescript
/**
 * Complete chat session with messages and configuration
 */
export interface ChatSession {
  version: number; // Schema version (currently 1)
  session_id: string; // Unique identifier
  token_count: number;
  context_window_size: number;
  session_name?: string | null;
  created_at?: string; // ISO timestamp
  updated_at?: string; // ISO timestamp
  deleted_at?: string | null;
  user_id?: string;
  metadata?: Record<string, any>;
  messages: Message[]; // Runtime messages with metadata
  agent_config?: CurrentAgentConfiguration;
  vendor: string; // "anthropic", "openai", or "none"
  display_name: string; // Computed display name
}

/**
 * Server version with vendor-specific messages
 */
export interface ServerChatSession {
  // Same fields as ChatSession except:
  messages: UnifiedMessageParam[]; // Vendor-specific format
}

/**
 * Session index entry for session lists
 */
export interface ChatSessionIndexEntry {
  session_id: string;
  session_name?: string | null;
  created_at?: string | null;
  updated_at?: string | null;
  user_id?: string | null;
  agent_key?: string;
  agent_name?: string;
}

/**
 * Paginated session query response
 */
export interface ChatSessionQueryResponse {
  chat_sessions: ChatSessionIndexEntry[];
  total_sessions: number;
  offset: number;
}

// Helper functions
export function getVendorFromModelId(modelId?: string): string;
export function getDisplayName(session: Partial<ChatSession>): string;
export function serverChatSessionToRuntimeSession(serverSession: ServerChatSession): ChatSession;
export function runtimeSessionToServerChatSession(session: ChatSession): ServerChatSession;
```

### Agent Configuration

Agent configuration with v1/v2 support:

```typescript
/**
 * Current agent configuration (v2)
 */
export interface AgentConfigurationV2 {
  version: 2;
  name: string;
  key: string; // Unique identifier
  model_id: string; // LLM model ID
  agent_description?: string;
  tools: string[]; // Enabled tools
  blocked_tool_patterns?: string[]; // Security control
  allowed_tool_patterns?: string[]; // Security control
  agent_params?: CompletionParams;
  prompt_metadata?: Record<string, any>;
  persona: string; // Agent persona/instructions
  uid?: string;
  category: string[]; // Categories from general to specific
}

/**
 * Legacy v1 configuration (compatibility)
 */
export interface AgentConfigurationV1 {
  version: 1;
  // Similar fields to v2 without security controls
}

export type AgentConfiguration = AgentConfigurationV1 | AgentConfigurationV2;
export type CurrentAgentConfiguration = AgentConfigurationV2;

/**
 * Agent catalog entry
 */
export interface AgentCatalogEntry {
  name: string;
  key: string;
  agent_description?: string;
  category: string[];
}
```

### Authentication Types

Types for agent authentication configuration:

```typescript
/**
 * Completion parameters union
 */
export type CompletionParams = 
  | ClaudeNonReasoningParams
  | ClaudeReasoningParams
  | GPTNonReasoningCompletionParams
  | GPTReasoningCompletionParams;

/**
 * Claude model parameters
 */
export interface ClaudeReasoningParams extends CommonCompletionParams {
  type: "claude_reasoning";
  budget_tokens?: number; // Must be > max_tokens
  max_searches?: number; // Web search limit
}

export interface ClaudeNonReasoningParams extends CommonCompletionParams {
  type: "claude_non_reasoning";
  temperature?: number; // Don't combine with top_p
}

/**
 * GPT model parameters
 */
export interface GPTReasoningCompletionParams extends CommonCompletionParams {
  type: "g_p_t_reasoning";
  reasoning_effort?: string; // "low" | "medium" | "high"
  tool_choice?: string | Record<string, any>;
  voice?: string;
  presence_penalty?: number; // -2.0 to 2.0
  seed?: number;
  stop?: string | string[];
}

/**
 * API authentication methods
 */
export type ApiAuthInfo = SimpleAuthInfo | AzureAuthInfo | BedrockAuthInfo;

export interface SimpleAuthInfo {
  api_key: string;
}

export interface AzureAuthInfo {
  endpoint: string;
  api_key: string;
  api_version?: string; // Default: "2024-08-01-preview"
}

export interface BedrockAuthInfo {
  aws_access_key?: string;
  aws_secret_key?: string;
  aws_region?: string;
  aws_session_token?: string;
}
```

## Event Types

### Client Events

Events sent from client to server:

```typescript
/**
 * Base client event interface
 */
export interface BaseClientEvent {
  type: string;
}

/**
 * Agent management
 */
export interface GetAgentsEvent extends BaseClientEvent {
  type: 'get_agents';
}

export interface SetAgentEvent extends BaseClientEvent {
  type: 'set_agent';
  agent_key: string;
}

/**
 * Avatar management
 */
export interface SetAvatarEvent extends BaseClientEvent {
  type: 'set_avatar';
  avatar_id: string;
  quality?: string; // Default: "auto"
  video_encoding?: string; // Default: "H265"
}

export interface SetAvatarSessionEvent extends BaseClientEvent {
  type: 'set_avatar_session';
  access_token: string; // HeyGen token
  avatar_session_id: string; // HeyGen session
}

/**
 * Message input
 */
export interface TextInputEvent extends BaseClientEvent {
  type: 'text_input';
  text: string;
  file_ids?: string[];
}

/**
 * Session management
 */
export interface NewChatSessionEvent extends BaseClientEvent {
  type: 'new_chat_session';
  agent_key?: string;
}

export interface ResumeChatSessionEvent extends BaseClientEvent {
  type: 'resume_chat_session';
  session_id: string;
}

export interface SetChatSessionNameEvent extends BaseClientEvent {
  type: 'set_chat_session_name';
  session_name: string;
  session_id?: string; // Current if not specified
}

export interface DeleteChatSessionEvent extends BaseClientEvent {
  type: 'delete_chat_session';
  session_id?: string; // Current if not specified
}

/**
 * Voice control
 */
export interface SetAgentVoiceEvent extends BaseClientEvent {
  type: 'set_agent_voice';
  voice_id: string;
}

export interface SetVoiceInputModeEvent extends BaseClientEvent {
  type: 'set_voice_input_mode';
  mode: 'ptt' | 'vad';
}

export interface PushToTalkStartEvent extends BaseClientEvent {
  type: 'ptt_start';
}

export interface PushToTalkEndEvent extends BaseClientEvent {
  type: 'ptt_end';
}

/**
 * Other events
 */
export interface ClientWantsCancelEvent extends BaseClientEvent {
  type: 'client_wants_cancel';
}

export interface PingEvent extends BaseClientEvent {
  type: 'ping';
}

// Union of all client events
export type ClientEvent = 
  | GetAgentsEvent | SetAgentEvent 
  | SetAvatarEvent | SetAvatarSessionEvent
  | TextInputEvent | NewChatSessionEvent
  | ResumeChatSessionEvent | SetChatSessionNameEvent
  | DeleteChatSessionEvent | SetAgentVoiceEvent
  | SetVoiceInputModeEvent | PushToTalkStartEvent
  | PushToTalkEndEvent | ClientWantsCancelEvent
  | PingEvent /* ... and more */;
```

### Server Events

Events sent from server to client:

```typescript
/**
 * Base server event interface
 */
export interface BaseServerEvent {
  type: string;
}

/**
 * Session events include tracking fields
 */
export interface SessionEvent extends BaseServerEvent {
  session_id: string;
  role: string;
  parent_session_id?: string;
  user_session_id?: string;
}

/**
 * Agent/Avatar updates
 */
export interface AgentListEvent extends BaseServerEvent {
  type: 'agent_list';
  agents: Agent[];
}

export interface AvatarListEvent extends BaseServerEvent {
  type: 'avatar_list';
  avatars: Avatar[];
}

/**
 * Chat session updates
 */
export interface ChatSessionChangedEvent extends BaseServerEvent {
  type: 'chat_session_changed';
  chat_session: ChatSession;
}

export interface ChatSessionAddedEvent extends BaseServerEvent {
  type: 'chat_session_added';
  chat_session: ChatSessionIndexEntry;
}

export interface ChatSessionDeletedEvent extends BaseServerEvent {
  type: 'chat_session_deleted';
  session_id?: string;
}

/**
 * Content streaming events
 */
export interface TextDeltaEvent extends SessionEvent {
  type: 'text_delta';
  role: 'assistant';
  content: string;
  format: MessageFormat;
}

export interface ThoughtDeltaEvent extends SessionEvent {
  type: 'thought_delta';
  role: 'assistant (thought)';
  content: string;
  format: MessageFormat;
}

/**
 * Completion tracking
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
 * Tool execution
 */
export interface ToolCallEvent extends SessionEvent {
  type: 'tool_call';
  role: 'assistant';
  active: boolean;
  vendor: string;
  tool_calls: ToolCall[];
  tool_results?: ToolResult[];
}

export interface ToolSelectDeltaEvent extends SessionEvent {
  type: 'tool_select_delta';
  role: 'assistant';
  tool_calls: ToolCall[];
}

/**
 * Turn management
 */
export interface UserTurnStartEvent extends BaseServerEvent {
  type: 'user_turn_start';
}

export interface UserTurnEndEvent extends BaseServerEvent {
  type: 'user_turn_end';
}

/**
 * System events
 */
export interface ErrorEvent extends BaseServerEvent {
  type: 'error';
  message: string;
  source?: string;
}

export interface SystemMessageEvent extends SessionEvent {
  type: 'system_message';
  role: 'system';
  content: string;
  format: MessageFormat;
  severity?: Severity;
}

/**
 * Rich media rendering
 */
export interface RenderMediaEvent extends SessionEvent {
  type: 'render_media';
  role: 'assistant';
  content_type: string; // e.g., "text/html"
  content: string;
  sent_by_class: string;
  sent_by_function: string;
  foreign_content: boolean; // Security flag
  url?: string;
  name?: string;
  content_bytes?: number;
}

// Union of all server events
export type ServerEvent = 
  | AgentListEvent | AvatarListEvent
  | ChatSessionChangedEvent | ChatSessionAddedEvent
  | TextDeltaEvent | ThoughtDeltaEvent
  | CompletionEvent | ToolCallEvent
  | UserTurnStartEvent | UserTurnEndEvent
  | ErrorEvent | SystemMessageEvent
  | RenderMediaEvent /* ... and more */;
```

### Common Event Types

Shared types used in events:

```typescript
/**
 * User information
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
 * Agent information
 */
export interface Agent {
  name: string;
  key: string;
  agent_description: string | null;
  category: string[];
  tools: string[];
}

/**
 * Avatar from HeyGen
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
 * Voice model
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
 * Tool execution
 */
export interface ToolCall {
  id: string;
  type: 'tool_use';
  name: string;
  input: Record<string, unknown>;
}

export interface ToolResult {
  type: 'tool_result';
  tool_use_id: string;
  content: string;
}

/**
 * File upload response from server
 */
export interface UserFile {
  id: string;           // Unique identifier for the uploaded file
  filename: string;     // Original filename
  mime_type: string;    // File MIME type (e.g., 'image/png')
  size: number;         // File size in bytes
}

/**
 * Type alias for API response compatibility
 */
export type UserFileResponse = UserFile;

/**
 * Options for file upload operations
 */
export interface FileUploadOptions {
  onProgress?: (progress: UploadProgress) => void;  // Progress callback for upload tracking
  signal?: AbortSignal;                              // Abort signal for cancellation support
}

/**
 * Upload progress information
 */
export interface UploadProgress {
  loaded: number;       // Bytes uploaded so far
  total: number;        // Total bytes to upload
  percentage: number;   // Progress percentage (0-100)
}

// Type definitions
export type StopReason = 'stop' | 'length' | 'tool_calls' | 'cancelled';
export type Severity = 'info' | 'warning' | 'error';
export type MessageFormat = 'text' | 'markdown';
```

## File Upload Types

Types for uploading files to include in chat messages, enabling multimodal interactions.

### Upload Response

File metadata returned after successful upload:

```typescript
/**
 * File upload response from server
 * Contains metadata about the uploaded file including a unique ID
 * that can be used to attach the file to messages
 */
export interface UserFile {
  /** Unique identifier for the uploaded file */
  id: string;
  /** Original filename */
  filename: string;
  /** MIME type of the file (e.g., 'image/png', 'application/pdf') */
  mime_type: string;
  /** File size in bytes */
  size: number;
}

/**
 * Type alias for API response compatibility
 * UserFileResponse is identical to UserFile
 */
export type UserFileResponse = UserFile;
```

**Usage Example:**

```typescript
import { UserFileResponse } from '@agentc/realtime-core';

const result: UserFileResponse = await client.uploadFile(file);
console.log('File ID:', result.id);
console.log('Filename:', result.filename);
console.log('MIME type:', result.mime_type);
console.log('Size:', result.size, 'bytes');

// Use file ID in message
client.sendText('Analyze this document', [result.id]);
```

### Upload Options

Configuration options for file upload operations:

```typescript
/**
 * Options for file upload operations
 * Supports progress tracking and cancellation
 */
export interface FileUploadOptions {
  /**
   * Progress callback for upload tracking
   * Called periodically during upload with progress information
   */
  onProgress?: (progress: UploadProgress) => void;
  
  /**
   * Abort signal for cancellation support
   * Can be used to cancel an ongoing upload
   */
  signal?: AbortSignal;
}
```

**Usage Example:**

```typescript
import { FileUploadOptions, UserFileResponse } from '@agentc/realtime-core';

// Upload with progress tracking
const options: FileUploadOptions = {
  onProgress: (progress) => {
    console.log(`Upload: ${progress.percentage}%`);
    progressBar.value = progress.percentage;
  }
};

const result = await client.uploadFile(file, options);

// Upload with cancellation support
const controller = new AbortController();
const cancelOptions: FileUploadOptions = {
  signal: controller.signal,
  onProgress: (progress) => {
    console.log(`Progress: ${progress.percentage}%`);
  }
};

cancelButton.onclick = () => controller.abort();

try {
  const result = await client.uploadFile(file, cancelOptions);
  console.log('Upload complete:', result);
} catch (error) {
  if (error.message === 'Upload cancelled') {
    console.log('User cancelled upload');
  }
}
```

### Upload Progress

Progress information provided during file upload:

```typescript
/**
 * Upload progress information
 * Provides real-time feedback on upload status
 */
export interface UploadProgress {
  /** Bytes uploaded so far */
  loaded: number;
  /** Total bytes to upload */
  total: number;
  /** Progress percentage (0-100) */
  percentage: number;
}
```

**Usage Example:**

```typescript
import { UploadProgress } from '@agentc/realtime-core';

await client.uploadFile(file, {
  onProgress: (progress: UploadProgress) => {
    // Display progress
    console.log(`Uploading: ${progress.loaded} / ${progress.total} bytes`);
    console.log(`Progress: ${progress.percentage}%`);
    
    // Update UI
    progressBar.value = progress.percentage;
    progressText.textContent = `${progress.percentage}%`;
    
    // Estimate time remaining
    const bytesRemaining = progress.total - progress.loaded;
    const uploadSpeed = calculateSpeed(); // Your implementation
    const timeRemaining = bytesRemaining / uploadSpeed;
    console.log(`Estimated time: ${timeRemaining}s`);
  }
});
```

### Configuration Types

File upload configuration in client config:

```typescript
/**
 * File upload configuration in RealtimeClientConfig
 */
interface RealtimeClientConfig {
  // ... other config options ...
  
  /**
   * Maximum file upload size in bytes
   * Default: 10MB (10 * 1024 * 1024)
   */
  maxUploadSize?: number;
  
  /**
   * Allowed file MIME types for upload
   * Default: undefined (allow all types)
   * Example: ['image/png', 'image/jpeg', 'application/pdf']
   */
  allowedMimeTypes?: string[];
  
  /**
   * Maximum number of files per message
   * Default: 10
   */
  maxFilesPerMessage?: number;
}
```

**Configuration Example:**

```typescript
import { RealtimeClient } from '@agentc/realtime-core';

const client = new RealtimeClient({
  apiUrl: 'wss://api.agentc.ai/realtime',
  authManager: authManager,
  
  // File upload configuration
  maxUploadSize: 10 * 1024 * 1024,    // 10MB
  allowedMimeTypes: [
    'image/png',
    'image/jpeg',
    'image/gif',
    'application/pdf',
    'text/plain'
  ],
  maxFilesPerMessage: 10
});
```

### Complete Upload Workflow

Full example demonstrating file upload types:

```typescript
import { 
  RealtimeClient,
  UserFileResponse, 
  FileUploadOptions,
  UploadProgress 
} from '@agentc/realtime-core';

async function uploadWithFullTracking(
  client: RealtimeClient,
  file: File
): Promise<UserFileResponse> {
  // Validate file size before upload
  const maxSize = 10 * 1024 * 1024; // 10MB
  if (file.size > maxSize) {
    throw new Error(`File too large: ${file.size} bytes (max: ${maxSize})`);
  }
  
  // Create abort controller for cancellation
  const controller = new AbortController();
  
  // Configure upload options
  const options: FileUploadOptions = {
    onProgress: (progress: UploadProgress) => {
      // Update UI with progress
      updateProgressBar(progress.percentage);
      updateProgressText(`${progress.loaded} / ${progress.total} bytes`);
      
      // Log progress
      console.log(`Upload progress: ${progress.percentage}%`);
    },
    signal: controller.signal
  };
  
  // Setup cancellation button
  const cancelButton = document.getElementById('cancel');
  cancelButton?.addEventListener('click', () => {
    controller.abort();
  });
  
  try {
    // Upload file
    const result: UserFileResponse = await client.uploadFile(file, options);
    
    // Log result
    console.log('Upload successful:');
    console.log('  ID:', result.id);
    console.log('  Filename:', result.filename);
    console.log('  Type:', result.mime_type);
    console.log('  Size:', result.size, 'bytes');
    
    // Hide progress UI
    hideProgressUI();
    
    return result;
    
  } catch (error) {
    // Handle errors
    if (error.message === 'Upload cancelled') {
      console.log('User cancelled upload');
      showNotification('Upload cancelled');
    } else if (error.message.includes('exceeds maximum')) {
      console.error('File too large');
      showError('File size exceeds 10MB limit');
    } else if (error.message.includes('not allowed')) {
      console.error('File type not supported');
      showError('File type not supported');
    } else {
      console.error('Upload failed:', error);
      showError('Upload failed. Please try again.');
    }
    throw error;
  }
}

// Upload multiple files
async function uploadMultipleFiles(
  client: RealtimeClient,
  files: File[]
): Promise<UserFileResponse[]> {
  // Validate file count
  const maxFiles = 10;
  if (files.length > maxFiles) {
    throw new Error(`Too many files: ${files.length} (max: ${maxFiles})`);
  }
  
  // Upload all files with aggregated progress
  const results: UserFileResponse[] = await client.uploadFiles(files, {
    onProgress: (progress: UploadProgress) => {
      // Overall progress across all files
      console.log(`Overall progress: ${progress.percentage}%`);
      updateOverallProgress(progress.percentage);
    }
  });
  
  // Log results
  console.log(`Successfully uploaded ${results.length} files:`);
  results.forEach((result, index) => {
    console.log(`  ${index + 1}. ${result.filename} (${result.size} bytes)`);
  });
  
  return results;
}
```

### Error Handling

Common error scenarios and handling:

```typescript
import { UserFileResponse } from '@agentc/realtime-core';

try {
  const result: UserFileResponse = await client.uploadFile(file);
  // Success - use result.id in message
  client.sendText('Analyze this', [result.id]);
  
} catch (error) {
  // Handle specific error cases
  const message = error.message;
  
  if (message.includes('Authentication required')) {
    // Not logged in or token expired
    await refreshAuthentication();
    // Retry upload
    
  } else if (message.includes('UI session ID required')) {
    // Session not initialized
    await client.connect();
    await client.waitForInitialization();
    // Retry upload
    
  } else if (message.includes('exceeds maximum')) {
    // File too large
    showError(`File is too large. Maximum size: 10MB`);
    
  } else if (message.includes('not allowed')) {
    // Invalid file type
    showError('File type not supported. Allowed: PNG, JPEG, PDF');
    
  } else if (message.includes('Cannot upload')) {
    // Too many files
    showError('Maximum 10 files per message');
    
  } else if (message === 'Upload cancelled') {
    // User cancelled
    console.log('Upload cancelled by user');
    
  } else if (message.includes('Network error')) {
    // Network issue
    showError('Upload failed due to network error. Please check your connection.');
    
  } else if (message.includes('Upload timed out')) {
    // Timeout
    showError('Upload timed out. The file may be too large or your connection too slow.');
    
  } else {
    // Other errors
    console.error('Upload error:', error);
    showError(`Upload failed: ${message}`);
  }
}
```

## Audio Types

### Audio Processing

Core audio processing types:

```typescript
/**
 * Audio chunk for transmission
 */
export interface AudioChunkData {
  content: ArrayBuffer; // PCM16 audio data
  content_type: string; // MIME type
  audio_level: number; // 0.0 to 1.0
  frame_count: number; // Sequential counter
  timestamp: number; // Capture time (ms)
  sample_rate: number; // Hz (typically 16000)
  sample_count: number; // Samples in chunk
}

/**
 * Audio processor configuration
 */
export interface AudioProcessorConfig {
  sampleRate?: number; // Default: 16000 Hz
  channelCount?: number; // Default: 1 (mono)
  bufferSize?: number; // Default: 2048 samples
  workletPath?: string; // Path to worklet file
  debug?: boolean;
}

/**
 * Processor status
 */
export interface AudioProcessorStatus {
  state: 'idle' | 'loading' | 'ready' | 'processing' | 'error';
  isProcessing: boolean;
  isReady: boolean;
  audioLevel: number; // 0.0 to 1.0
  chunksProcessed: number;
  error?: string;
  contextSampleRate?: number;
  outputSampleRate: number;
}

/**
 * Audio processor errors
 */
export enum AudioProcessorErrorCode {
  AUDIO_CONTEXT_ERROR = 'AUDIO_CONTEXT_ERROR',
  WORKLET_LOAD_ERROR = 'WORKLET_LOAD_ERROR',
  MICROPHONE_ACCESS_ERROR = 'MICROPHONE_ACCESS_ERROR',
  PROCESSING_ERROR = 'PROCESSING_ERROR',
  INVALID_CONFIG = 'INVALID_CONFIG',
  NOT_SUPPORTED = 'NOT_SUPPORTED'
}

export class AudioProcessorError extends Error {
  constructor(
    message: string,
    public code: AudioProcessorErrorCode,
    public details?: unknown
  );
}
```

### Audio Service

Audio service management types:

```typescript
/**
 * Audio service states
 */
export type AudioServiceState = 
  | 'idle' 
  | 'initializing' 
  | 'ready' 
  | 'recording' 
  | 'failed' 
  | 'permission-denied';

/**
 * Audio service status
 */
export interface AudioServiceStatus {
  state: AudioServiceState;
  isRecording: boolean;
  audioLevel: number; // 0.0 to 1.0
  frameCount: number;
  error?: string;
  deviceId?: string;
  sampleRate: number;
  channelCount: number;
}

/**
 * Audio bridge configuration
 */
export interface AudioAgentCBridgeConfig {
  respectTurnState?: boolean; // Default: true
  logAudioChunks?: boolean; // Default: false
  debug?: boolean; // Default: false
}

/**
 * Audio bridge status
 */
export interface AudioAgentCBridgeStatus {
  isStreaming: boolean;
  isConnected: boolean;
  userHasTurn: boolean;
  chunksStreamed: number;
  chunksSuppressed: number;
  respectingTurnState: boolean;
  clientState?: 'connected' | 'disconnected' | 'connecting';
}

/**
 * Audio output status
 */
export interface AudioOutputStatus {
  isPlaying: boolean;
  isEnabled: boolean;
  chunksReceived: number;
  chunksPlayed: number;
  chunksSkipped: number;
  queueLength: number;
  volume: number; // 0.0 to 1.0
  voiceModel: VoiceModel | null;
  skipPlayback: boolean;
}

/**
 * Voice model configuration
 */
export interface VoiceModel {
  voice_id: string; // e.g., 'openai_tts_nova'
  format: string; // e.g., 'pcm16'
  vendor?: string;
  description?: string;
  sampleRate?: number; // Hz
}
```

### Audio Configuration

Combined audio system configuration:

```typescript
/**
 * Audio configuration for client
 */
export interface AudioConfig {
  enableInput?: boolean; // Enable microphone
  enableOutput?: boolean; // Enable speakers
  respectTurnState?: boolean; // Honor turn management
  logAudioChunks?: boolean; // Debug logging
  sampleRate?: number; // Default: 24000
  chunkSize?: number; // Default: 4800
  initialVolume?: number; // 0-1, default: 1.0
  autoGainControl?: boolean;
  echoCancellation?: boolean;
  noiseSuppression?: boolean;
}

/**
 * Combined audio status
 */
export interface AudioStatus {
  // Input status
  isRecording: boolean;
  isStreaming: boolean;
  isProcessing: boolean;
  hasPermission: boolean;
  currentLevel: number; // 0.0 to 1.0
  averageLevel: number; // 0.0 to 1.0
  
  // Output status
  isPlaying: boolean;
  bufferSize: number;
  volume: number; // 0.0 to 1.0
  
  // System status
  isAudioEnabled: boolean;
  isInputEnabled: boolean;
  isOutputEnabled: boolean;
}

// Default configurations
export const DEFAULT_AUDIO_CONFIG: Required<AudioProcessorConfig> = {
  sampleRate: 24000,
  channelCount: 1,
  bufferSize: 2048,
  workletPath: '/worklets/audio-processor.worklet.js',
  debug: false
};
```

## Connection Types

### Client Configuration

Main client configuration types:

```typescript
/**
 * Realtime client configuration
 */
export interface RealtimeClientConfig {
  // Required
  apiUrl: string; // WebSocket URL
  
  // Authentication (one required)
  authToken?: string; // JWT token
  authManager?: AuthManager; // Auto token management
  
  // Session
  sessionId?: string; // Resume existing session
  
  // Connection
  autoReconnect?: boolean; // Default: true
  reconnection?: Partial<ReconnectionConfig>;
  connectionTimeout?: number; // ms, default: 10000
  pingInterval?: number; // ms, default: 30000
  pongTimeout?: number; // ms, default: 10000
  maxMessageSize?: number; // bytes, default: 10MB
  
  // Features
  enableTurnManager?: boolean; // Default: true
  enableAudio?: boolean; // Default: false
  audioConfig?: AudioConfig;
  
  // WebSocket options
  headers?: Record<string, string>;
  protocols?: string[];
  binaryType?: 'blob' | 'arraybuffer';
  
  // Debug
  debug?: boolean; // Default: false
}

// Helper to merge with defaults
export function mergeConfig(
  userConfig: RealtimeClientConfig
): Required<RealtimeClientConfig>;
```

### Connection State

Connection lifecycle states:

```typescript
/**
 * WebSocket connection states
 */
export enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',
  CONNECTING = 'CONNECTING',
  CONNECTED = 'CONNECTED',
  RECONNECTING = 'RECONNECTING'
}

/**
 * Connection status information
 */
export interface ConnectionStatus {
  state: ConnectionState;
  isConnected: boolean;
  isReconnecting: boolean;
  reconnectAttempts: number;
  lastConnectedAt?: Date;
  lastDisconnectedAt?: Date;
  lastError?: Error;
  
  // Statistics
  messagesSent: number;
  messagesReceived: number;
  bytesSent: number;
  bytesReceived: number;
  
  // Timing
  connectionDuration?: number; // ms
  roundTripTime?: number; // ms (from ping/pong)
}
```

### Reconnection

Automatic reconnection configuration:

```typescript
/**
 * Reconnection behavior configuration
 */
export interface ReconnectionConfig {
  enabled: boolean; // Auto-reconnect on disconnect
  initialDelay: number; // ms before first attempt
  maxDelay: number; // Maximum delay between attempts
  backoffMultiplier: number; // Exponential backoff factor
  maxAttempts: number; // 0 = unlimited
  jitterFactor: number; // 0-1 randomization factor
}

// Default configuration
export const defaultReconnectionConfig: ReconnectionConfig = {
  enabled: true,
  initialDelay: 1000, // 1 second
  maxDelay: 30000, // 30 seconds
  backoffMultiplier: 1.5,
  maxAttempts: 0, // Unlimited
  jitterFactor: 0.3 // 30% jitter
};
```

## Authentication Types

### Auth Configuration

Authentication system configuration:

```typescript
/**
 * Authentication configuration
 */
export interface AuthConfig {
  apiUrl: string; // Base API URL
  fetch?: typeof fetch; // Custom fetch
  refreshBufferMs?: number; // Default: 60000 (1 min)
  autoRefresh?: boolean; // Default: true
  onTokensRefreshed?: (tokens: TokenPair) => void;
  onAuthError?: (error: Error) => void;
  storage?: TokenStorage; // Token persistence
}

/**
 * Token storage interface
 */
export interface TokenStorage {
  getTokens(): Promise<TokenPair | null>;
  setTokens(tokens: TokenPair): Promise<void>;
  clearTokens(): Promise<void>;
}

// Built-in storage implementations
export class MemoryTokenStorage implements TokenStorage { /* ... */ }
export class LocalStorageTokenStorage implements TokenStorage { /* ... */ }
```

### Token Management

Authentication tokens and state:

```typescript
/**
 * Authentication token pair
 */
export interface TokenPair {
  agentCToken: string; // JWT for Agent C API
  heygenToken: string; // Access token for HeyGen
  expiresAt?: number; // Unix timestamp (ms)
}

/**
 * Login credentials
 */
export interface LoginCredentials {
  username: string;
  password: string;
}

/**
 * JWT payload structure
 */
export interface JWTPayload {
  user_id: string;
  permissions?: string[];
  exp: number; // Unix timestamp (seconds)
  iat: number; // Unix timestamp (seconds)
  [key: string]: unknown;
}

/**
 * Authentication state
 */
export interface AuthState {
  isAuthenticated: boolean;
  tokens: TokenPair | null;
  user: User | null;
  uiSessionId: string | null;
  wsUrl: string | null;
  isAuthenticating: boolean;
  isRefreshing: boolean;
  error: Error | null;
}

/**
 * API responses
 */
export interface LoginResponse {
  agent_c_token: string;
  heygen_token: string;
  ui_session_id: string;
}

export interface RefreshTokenResponse {
  agent_c_token: string;
  heygen_token: string;
}
```

## Utility Types

### Common Patterns

Common utility types used throughout the SDK:

```typescript
/**
 * Nullable and optional type helpers
 */
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type AsyncFunction<T = void> = () => Promise<T>;

/**
 * Error details structure
 */
export interface ErrorDetails {
  code: string;
  message: string;
  details?: unknown;
}
```

### Type Guards

Runtime type checking utilities:

```typescript
// Message type guards
export function isAnthropicMessage(message: UnifiedMessageParam): message is MessageParam;
export function isOpenAIMessage(message: UnifiedMessageParam): message is ChatCompletionMessageParam;

// Anthropic content block guards
export function isTextBlockParam(block: ContentBlockParam): block is TextBlockParam;
export function isImageBlockParam(block: ContentBlockParam): block is ImageBlockParam;
export function isToolUseBlockParam(block: ContentBlockParam): block is ToolUseBlockParam;
export function isToolResultBlockParam(block: ContentBlockParam): block is ToolResultBlockParam;

// OpenAI message guards
export function isChatCompletionUserMessageParam(
  message: ChatCompletionMessageParam
): message is ChatCompletionUserMessageParam;

export function isChatCompletionAssistantMessageParam(
  message: ChatCompletionMessageParam
): message is ChatCompletionAssistantMessageParam;

// OpenAI content part guards
export function isChatCompletionContentPartText(
  part: ChatCompletionContentPart
): part is ChatCompletionContentPartText;

export function isChatCompletionContentPartImage(
  part: ChatCompletionContentPart
): part is ChatCompletionContentPartImage;

// Agent configuration guards
export function isAgentConfigurationV2(
  config: AgentConfiguration
): config is AgentConfigurationV2;

// Completion parameter guards
export function isClaudeReasoningParams(
  params: CompletionParams
): params is ClaudeReasoningParams;

export function isGPTReasoningParams(
  params: CompletionParams
): params is GPTReasoningCompletionParams;

// Auth type guards
export function isSimpleAuthInfo(auth: ApiAuthInfo): auth is SimpleAuthInfo;
export function isAzureAuthInfo(auth: ApiAuthInfo): auth is AzureAuthInfo;
export function isBedrockAuthInfo(auth: ApiAuthInfo): auth is BedrockAuthInfo;

// Event type guards
export function isClientEvent(obj: unknown): obj is ClientEvent;
export function isServerEvent(obj: unknown): obj is ServerEvent;

// Audio message guards
export function isAudioChunkMessage(message: WorkletMessage): message is AudioChunkMessage;
export function isWorkletStatusMessage(message: WorkletMessage): message is WorkletStatusMessage;
```

### Logging

Logging utilities for debugging:

```typescript
/**
 * Log levels for the SDK
 */
export enum LogLevel {
  ERROR = 0,
  WARN = 1,
  INFO = 2,
  DEBUG = 3,
  TRACE = 4
}

/**
 * Logger class for structured logging
 */
export class Logger {
  static setLevel(level: LogLevel): void;
  static error(message: string, ...args: any[]): void;
  static warn(message: string, ...args: any[]): void;
  static info(message: string, ...args: any[]): void;
  static debug(message: string, ...args: any[]): void;
  static trace(message: string, ...args: any[]): void;
}
```

## Usage Examples

### Working with Messages

```typescript
import { 
  UnifiedMessageParam, 
  Message, 
  isAnthropicMessage,
  toOpenAIMessage,
  unifiedMessageParamToMessage 
} from '@agentc/realtime-core';

// Check message vendor
const message: UnifiedMessageParam = { role: 'user', content: 'Hello' };
if (isAnthropicMessage(message)) {
  console.log('Anthropic format');
}

// Convert between formats
const openAIMessage = toOpenAIMessage(message);

// Convert to runtime message
const runtimeMessage: Message = unifiedMessageParamToMessage(message);
```

### Managing Chat Sessions

```typescript
import {
  ChatSession,
  ServerChatSession,
  serverChatSessionToRuntimeSession,
  getVendorFromModelId
} from '@agentc/realtime-core';

// Determine vendor from model
const vendor = getVendorFromModelId('claude-3-5-sonnet-20241022');
// Returns: 'anthropic'

// Convert server session to runtime
const serverSession: ServerChatSession = await fetchSession();
const session: ChatSession = serverChatSessionToRuntimeSession(serverSession);

// Access runtime messages with metadata
session.messages.forEach(msg => {
  console.log(msg.role, msg.content, msg.timestamp);
});
```

### Handling Events

```typescript
import {
  ServerEvent,
  TextDeltaEvent,
  CompletionEvent,
  isServerEvent
} from '@agentc/realtime-core';

// Type-safe event handling
function handleServerEvent(event: ServerEvent) {
  switch (event.type) {
    case 'text_delta':
      const textEvent = event as TextDeltaEvent;
      console.log('Assistant:', textEvent.content);
      break;
      
    case 'completion':
      const completion = event as CompletionEvent;
      if (!completion.running && completion.stop_reason === 'stop') {
        console.log('Response complete');
      }
      break;
  }
}

// Runtime type checking
const data = JSON.parse(websocketMessage);
if (isServerEvent(data)) {
  handleServerEvent(data);
}
```

### Audio Processing

```typescript
import {
  AudioChunkData,
  AudioProcessorConfig,
  AudioProcessorStatus,
  AudioProcessorError,
  AudioProcessorErrorCode
} from '@agentc/realtime-core';

// Configure audio processor
const config: AudioProcessorConfig = {
  sampleRate: 24000,
  channelCount: 1,
  bufferSize: 4800,
  debug: true
};

// Handle audio chunks
function handleAudioChunk(chunk: AudioChunkData) {
  console.log('Audio level:', chunk.audio_level);
  console.log('Samples:', chunk.sample_count);
  
  // Send to server via WebSocket
  websocket.send(chunk.content);
}

// Handle errors
try {
  await startRecording();
} catch (error) {
  if (error instanceof AudioProcessorError) {
    if (error.code === AudioProcessorErrorCode.MICROPHONE_ACCESS_ERROR) {
      console.error('Microphone permission denied');
    }
  }
}
```

### Client Configuration

```typescript
import {
  RealtimeClientConfig,
  ConnectionState,
  mergeConfig
} from '@agentc/realtime-core';

// Configure client with TypeScript support
const config: RealtimeClientConfig = {
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'jwt-token-here',
  
  // Enable features
  enableAudio: true,
  enableTurnManager: true,
  
  // Audio settings
  audioConfig: {
    enableInput: true,
    enableOutput: true,
    respectTurnState: true,
    sampleRate: 24000,
    initialVolume: 0.8
  },
  
  // Reconnection settings
  reconnection: {
    enabled: true,
    maxAttempts: 10,
    backoffMultiplier: 2
  },
  
  // Debugging
  debug: process.env.NODE_ENV === 'development'
};

// Merge with defaults
const fullConfig = mergeConfig(config);
```

---

## Type Export Summary

All types are exported from the main package entry point:

```typescript
// Import everything
import * as RealtimeTypes from '@agentc/realtime-core';

// Import specific types
import {
  // Messages
  Message,
  UnifiedMessageParam,
  MessageParam,
  ChatCompletionMessageParam,
  
  // Sessions
  ChatSession,
  ChatSessionIndexEntry,
  AgentConfiguration,
  
  // Events
  ClientEvent,
  ServerEvent,
  TextDeltaEvent,
  CompletionEvent,
  
  // Audio
  AudioChunkData,
  AudioProcessorConfig,
  AudioStatus,
  
  // Client
  RealtimeClientConfig,
  ConnectionState,
  
  // Auth
  AuthConfig,
  TokenPair,
  
  // Utilities
  Logger,
  LogLevel
} from '@agentc/realtime-core';
```

## Version Information

Current SDK version: `0.1.0`

```typescript
import { VERSION } from '@agentc/realtime-core';
console.log('SDK Version:', VERSION); // "0.1.0"
```