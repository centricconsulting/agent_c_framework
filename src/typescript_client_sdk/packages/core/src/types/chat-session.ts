/**
 * Agent C ChatSession TypeScript Definitions
 * 
 * This file contains comprehensive type definitions for the Agent C ChatSession
 * model and all related types, converted from the Python implementation.
 * 
 * These types match the actual server data structures exactly.
 */

import type { MessageParam } from './message-params';
import type { ChatCompletionMessageParam } from './openai-message-params';
import type { UnifiedMessageParam } from './ChatTypes';

// Re-import Message type from CommonTypes for compatibility
// The Message type is the runtime version with additional fields
// UnifiedMessageParam is the server data structure that supports both Anthropic and OpenAI
import type { Message } from '../events/types/CommonTypes';

// Core ChatSession Interfaces
export interface ChatSessionIndexEntry {
  session_id: string; // Unique identifier for the chat session
  session_name?: string | null; // The name of the session, if any (can be null)
  created_at?: string | null; // ISO timestamp, defaults to current time (can be null)
  updated_at?: string | null; // ISO timestamp, defaults to current time (can be null)
  user_id?: string | null; // The user ID associated with the session (defaults to "admin", can be null)
  agent_key?: string; // The key of the agent associated with the session
  agent_name?: string; // The name of the agent associated with the session
}

export interface ChatSessionQueryResponse {
  chat_sessions: ChatSessionIndexEntry[]; // List of chat session index entries (defaults to empty array)
  total_sessions: number; // Total number of sessions available for the query (defaults to 0)
  offset: number; // The offset used in the query (defaults to 0)
}

export interface ChatSession {
  version: number; // Version of the chat session schema (defaults to 1)
  session_id: string; // Unique identifier generated using mnemonic slugs (defaults to 2-word slug)
  token_count: number; // The number of tokens used in the session (defaults to 0)
  context_window_size: number; // The number of tokens in the context window (defaults to 0)
  session_name?: string | null; // The name of the session, if any
  created_at?: string; // ISO timestamp when created (defaults to current time)
  updated_at?: string; // ISO timestamp when last updated (defaults to current time)
  deleted_at?: string | null; // Timestamp when the session was deleted
  user_id?: string; // The user ID associated with the session (defaults to "admin")
  metadata?: Record<string, any>; // Metadata associated with the session (defaults to empty object)
  messages: Message[]; // List of messages in the session (runtime type with additional fields)
  agent_config?: CurrentAgentConfiguration; // Configuration for the agent associated with the session
  vendor: string; // Computed field: Returns "anthropic", "openai", or "none" based on model_id
  display_name: string; // Computed field: Returns session_name or generated name with agent
}

/**
 * Server version of ChatSession with UnifiedMessageParam[]
 * Used when receiving raw data from the server
 * Supports both Anthropic (MessageParam) and OpenAI (ChatCompletionMessageParam) formats
 */
export interface ServerChatSession {
  version: number;
  session_id: string;
  token_count: number;
  context_window_size: number;
  session_name?: string | null;
  created_at?: string;
  updated_at?: string;
  deleted_at?: string | null;
  user_id?: string;
  metadata?: Record<string, any>;
  messages: UnifiedMessageParam[]; // Server sends UnifiedMessageParam structure (Anthropic or OpenAI)
  agent_config?: CurrentAgentConfiguration;
  vendor: string; // Determines message format: "anthropic" or "openai"
  display_name: string;
}

// Agent Configuration Types
export interface AgentCatalogEntry {
  name: string; // Name of the agent configuration
  key: string; // Key for the agent configuration, used for identification
  agent_description?: string; // A description of the agent's purpose and capabilities
  category: string[]; // List of categories this agent belongs to (from most to least general)
}

// Legacy V1 Configuration (for compatibility)
export interface AgentConfigurationV1 {
  version: 1;
  name: string;
  key: string;
  model_id: string;
  agent_description?: string;
  tools: string[];
  agent_params?: CompletionParams;
  prompt_metadata?: Record<string, any>;
  persona: string;
  uid?: string;
  category: string[];
}

// Current V2 Configuration
export interface AgentConfigurationV2 {
  version: 2; // Configuration version
  name: string; // Name of the persona file
  key: string; // Key for the agent configuration, used for identification (defaults to name if not provided)
  model_id: string; // ID of the LLM model being used by the agent
  agent_description?: string; // A description of the agent's purpose and capabilities
  tools: string[]; // List of enabled tool names the agent can use (defaults to empty array)
  blocked_tool_patterns?: string[]; // Patterns for tools that should be blocked for this agent (security control)
  allowed_tool_patterns?: string[]; // Patterns for tools that are explicitly allowed for this agent (security control)
  agent_params?: CompletionParams; // Parameters for the interaction with the agent
  prompt_metadata?: Record<string, any>; // Metadata for the prompt
  persona: string; // Persona prompt defining the agent's behavior
  uid?: string; // Unique identifier for the configuration
  category: string[]; // List of categories this agent belongs to (from most to least general)
}

// Type aliases for convenience
export type AgentConfiguration = AgentConfigurationV1 | AgentConfigurationV2;
export type CurrentAgentConfiguration = AgentConfigurationV2;

// Completion Parameters Types
export interface CommonCompletionParams {
  model_name: string; // The name of the model to use for the interaction
  max_tokens?: number; // The maximum number of tokens to generate (defaults to backend defaults)
  user_name?: string; // The name of the user interacting with the agent
  auth?: ApiAuthInfo; // The vendor API key or authentication info to use for the agent
}

// Claude Completion Parameters
export interface ClaudeNonReasoningParams extends CommonCompletionParams {
  type: "claude_non_reasoning"; // The type of the completion params
  temperature?: number; // The temperature to use for the interaction (do not combine with top_p)
}

export interface ClaudeReasoningParams extends CommonCompletionParams {
  type: "claude_reasoning"; // The type of the completion params
  budget_tokens?: number; // The budget tokens to use for the interaction (must be higher than max tokens)
  max_searches?: number; // The maximum number of web searches for the claude models to perform
}

// GPT Completion Parameters
export interface GPTNonReasoningCompletionParams extends CommonCompletionParams {
  type: "g_p_t_non_reasoning"; // The type of the completion params
  tool_choice?: string | Record<string, any>; // The tool choice to use for the interaction (see OpenAI API docs)
  voice?: string; // Voice parameter for audio generation
  presence_penalty?: number; // Number between -2.0 and 2.0 for penalizing new tokens (defaults to 0)
  seed?: number; // For deterministic sampling (Beta feature)
  service_tier?: string; // See OpenAI Docs for details
  stop?: string | string[]; // Up to 4 sequences where the API will stop generating tokens
  temperature?: number; // The temperature to use for the interaction (do not combine with top_p)
}

export interface GPTReasoningCompletionParams extends CommonCompletionParams {
  type: "g_p_t_reasoning"; // The type of the completion params
  tool_choice?: string | Record<string, any>; // The tool choice to use for the interaction (see OpenAI API docs)
  voice?: string; // Voice parameter for audio generation
  presence_penalty?: number; // Number between -2.0 and 2.0 for penalizing new tokens (defaults to 0)
  seed?: number; // For deterministic sampling (Beta feature)
  service_tier?: string; // See OpenAI Docs for details
  stop?: string | string[]; // Up to 4 sequences where the API will stop generating tokens
  reasoning_effort?: string; // The reasoning effort to use for the interaction (must be low, medium, or high)
}

// Union type for all completion parameter variants
export type CompletionParams = 
  | ClaudeNonReasoningParams
  | ClaudeReasoningParams
  | GPTNonReasoningCompletionParams
  | GPTReasoningCompletionParams;

// Authentication Types
export interface SimpleAuthInfo {
  api_key: string; // The API key to use for the interaction
}

export interface AzureAuthInfo {
  endpoint: string; // The Azure OpenAI endpoint to use for the interaction
  api_key: string; // The Azure OpenAI API key to use for the interaction
  api_version?: string; // The Azure OpenAI API version to use (defaults to "2024-08-01-preview")
}

export interface BedrockAuthInfo {
  aws_access_key?: string; // The AWS access key to use for the interaction
  aws_secret_key?: string; // The AWS secret key to use for the interaction
  aws_region?: string; // The AWS region to use for the interaction
  aws_session_token?: string; // Temporary credentials can be used with aws_session_token
}

export type ApiAuthInfo = SimpleAuthInfo | AzureAuthInfo | BedrockAuthInfo;

// Type guards for runtime type checking
export function isAgentConfigurationV2(config: AgentConfiguration): config is AgentConfigurationV2 {
  return config.version === 2;
}

export function isClaudeNonReasoningParams(params: CompletionParams): params is ClaudeNonReasoningParams {
  return params.type === 'claude_non_reasoning';
}

export function isClaudeReasoningParams(params: CompletionParams): params is ClaudeReasoningParams {
  return params.type === 'claude_reasoning';
}

export function isGPTNonReasoningParams(params: CompletionParams): params is GPTNonReasoningCompletionParams {
  return params.type === 'g_p_t_non_reasoning';
}

export function isGPTReasoningParams(params: CompletionParams): params is GPTReasoningCompletionParams {
  return params.type === 'g_p_t_reasoning';
}

export function isSimpleAuthInfo(auth: ApiAuthInfo): auth is SimpleAuthInfo {
  return 'api_key' in auth && !('endpoint' in auth) && !('aws_access_key' in auth);
}

export function isAzureAuthInfo(auth: ApiAuthInfo): auth is AzureAuthInfo {
  return 'endpoint' in auth && 'api_key' in auth;
}

export function isBedrockAuthInfo(auth: ApiAuthInfo): auth is BedrockAuthInfo {
  return 'aws_access_key' in auth || 'aws_secret_key' in auth || 'aws_region' in auth;
}

// Helper function to compute vendor from model_id
export function getVendorFromModelId(modelId?: string): string {
  if (!modelId) return 'none';
  
  const modelIdLower = modelId.toLowerCase();
  
  if (modelIdLower.includes('claude') || modelIdLower.includes('anthropic')) {
    return 'anthropic';
  } else if (modelIdLower.includes('gpt') || modelIdLower.includes('openai')) {
    return 'openai';
  }
  
  return 'none';
}

// Helper function to compute display name
export function getDisplayName(session: Partial<ChatSession>): string {
  if (session.session_name) {
    return session.session_name;
  }
  
  const agentName = session.agent_config?.name || 'Agent';
  return `New chat with ${agentName}`;
}

/**
 * Convert a UnifiedMessageParam to a Message (runtime type)
 * Adds runtime fields like timestamp and format
 * Handles both Anthropic and OpenAI message formats
 */
export function unifiedMessageParamToMessage(param: UnifiedMessageParam): Message {
  // Extract role and content based on message format
  let role: 'user' | 'assistant' | 'system' | 'assistant (thought)';
  let content: any;
  
  if ('role' in param) {
    // Map OpenAI roles to Message roles
    switch (param.role) {
      case 'user':
        role = 'user';
        break;
      case 'assistant':
        role = 'assistant';
        break;
      case 'system':
      case 'developer': // Map developer to system
        role = 'system';
        break;
      case 'tool':
      case 'function':
        // Tool and function messages are treated as assistant messages
        role = 'assistant';
        break;
      default:
        // Fallback to user for any unexpected role
        role = 'user';
    }
    
    // Handle different content formats
    if ('content' in param) {
      content = param.content;
    } else {
      // Some OpenAI message types (like assistant) can have optional content
      content = null;
    }
  } else {
    // Fallback for any unexpected format
    role = 'user';
    content = '';
  }
  
  return {
    role,
    content: content as any,
    timestamp: new Date().toISOString(),
    format: 'text' as const
  };
}

/**
 * Convert a Message to a UnifiedMessageParam (server type)
 * Strips runtime-only fields and returns appropriate format based on vendor
 */
export function messageToUnifiedMessageParam(message: Message, vendor: string): UnifiedMessageParam {
  // Based on vendor, create appropriate message format
  if (vendor === 'openai') {
    // Map Message roles to OpenAI roles
    let openAIRole: 'user' | 'assistant' | 'system' | 'developer';
    switch (message.role) {
      case 'user':
        openAIRole = 'user';
        break;
      case 'assistant':
      case 'assistant (thought)':
        openAIRole = 'assistant';
        break;
      case 'system':
        openAIRole = 'system';
        break;
      default:
        openAIRole = 'user'; // Fallback
    }
    
    // Create OpenAI format message
    const openAIMessage: ChatCompletionMessageParam = {
      role: openAIRole,
      content: message.content === null ? '' : message.content as any
    } as ChatCompletionMessageParam;
    return openAIMessage;
  } else {
    // Map Message roles to Anthropic roles (only user/assistant supported)
    let anthropicRole: 'user' | 'assistant';
    switch (message.role) {
      case 'user':
      case 'system': // System messages become user messages in Anthropic
        anthropicRole = 'user';
        break;
      case 'assistant':
      case 'assistant (thought)':
        anthropicRole = 'assistant';
        break;
      default:
        anthropicRole = 'user'; // Fallback
    }
    
    // Default to Anthropic format
    const anthropicMessage: MessageParam = {
      role: anthropicRole,
      content: message.content === null ? '' : message.content as any
    };
    return anthropicMessage;
  }
}

/**
 * Legacy function for backward compatibility
 * @deprecated Use unifiedMessageParamToMessage instead
 */
export function messageParamToMessage(param: MessageParam): Message {
  return unifiedMessageParamToMessage(param);
}

/**
 * Legacy function for backward compatibility
 * @deprecated Use messageToUnifiedMessageParam instead
 */
export function messageToMessageParam(message: Message): MessageParam {
  return messageToUnifiedMessageParam(message, 'anthropic') as MessageParam;
}

/**
 * Convert a ServerChatSession to a ChatSession
 * Transforms UnifiedMessageParam[] to Message[] and adds runtime fields
 */
export function serverChatSessionToRuntimeSession(serverSession: ServerChatSession): ChatSession {
  return {
    ...serverSession,
    messages: serverSession.messages.map(unifiedMessageParamToMessage)
  };
}

/**
 * Convert a ChatSession to a ServerChatSession
 * Transforms Message[] to UnifiedMessageParam[] for server communication
 * Uses the vendor field to determine the correct message format
 */
export function runtimeSessionToServerChatSession(session: ChatSession): ServerChatSession {
  const vendor = session.vendor || getVendorFromModelId(session.agent_config?.model_id);
  return {
    ...session,
    messages: session.messages.map(msg => messageToUnifiedMessageParam(msg, vendor))
  };
}