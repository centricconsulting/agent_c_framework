/**
 * Unified Chat Types for Agent C Realtime SDK
 * 
 * This file provides a unified interface for handling both Anthropic and OpenAI
 * message formats. The vendor field in chat sessions determines which format to use.
 */

import type { MessageParam } from './message-params';
import type { ChatCompletionMessageParam } from './openai-message-params';

/**
 * Unified message type that supports both Anthropic and OpenAI formats
 * The vendor field in the chat session determines which format is used:
 * - vendor: "anthropic" → use MessageParam
 * - vendor: "openai" → use ChatCompletionMessageParam
 */
export type UnifiedMessageParam = MessageParam | ChatCompletionMessageParam;

/**
 * Type guard to check if a message is in Anthropic format
 */
export function isAnthropicMessage(message: UnifiedMessageParam): message is MessageParam {
  // Anthropic messages have role "user" or "assistant" only
  // and don't have the additional roles that OpenAI has
  return message.role === 'user' || message.role === 'assistant';
}

/**
 * Type guard to check if a message is in OpenAI format
 */
export function isOpenAIMessage(message: UnifiedMessageParam): message is ChatCompletionMessageParam {
  // OpenAI messages can have additional roles beyond user/assistant
  return message.role === 'developer' || 
         message.role === 'system' || 
         message.role === 'tool' || 
         message.role === 'function' ||
         // For user/assistant, check for OpenAI-specific fields
         (message.role === 'user' && 'name' in message) ||
         (message.role === 'assistant' && ('tool_calls' in message || 'function_call' in message || 'audio' in message || 'refusal' in message));
}

/**
 * Helper to determine message format based on vendor
 */
export function getMessageFormat(vendor: string): 'anthropic' | 'openai' | 'unknown' {
  switch (vendor.toLowerCase()) {
    case 'anthropic':
      return 'anthropic';
    case 'openai':
      return 'openai';
    default:
      return 'unknown';
  }
}

/**
 * Convert a unified message to Anthropic format if possible
 */
export function toAnthropicMessage(message: UnifiedMessageParam): MessageParam | null {
  if (isAnthropicMessage(message)) {
    return message;
  }
  
  // Try to convert OpenAI message to Anthropic format
  if (isOpenAIMessage(message)) {
    // Only user and assistant messages can be converted
    if (message.role === 'user' || message.role === 'assistant') {
      return {
        role: message.role,
        content: typeof message.content === 'string' ? message.content : []
      };
    }
  }
  
  return null;
}

/**
 * Convert a unified message to OpenAI format if possible
 */
export function toOpenAIMessage(message: UnifiedMessageParam): ChatCompletionMessageParam | null {
  if (isOpenAIMessage(message)) {
    return message;
  }
  
  // Convert Anthropic message to OpenAI format
  if (isAnthropicMessage(message)) {
    if (message.role === 'user') {
      return {
        role: 'user',
        content: typeof message.content === 'string' ? message.content : []
      };
    } else if (message.role === 'assistant') {
      return {
        role: 'assistant',
        content: typeof message.content === 'string' ? message.content : []
      };
    }
  }
  
  return null;
}

// Re-export the individual message types for convenience
export type { MessageParam } from './message-params';
export type { 
  ChatCompletionMessageParam,
  ChatCompletionDeveloperMessageParam,
  ChatCompletionSystemMessageParam,
  ChatCompletionUserMessageParam,
  ChatCompletionAssistantMessageParam,
  ChatCompletionToolMessageParam,
  ChatCompletionFunctionMessageParam,
  ChatCompletionContentPart,
  ChatCompletionContentPartText,
  ChatCompletionContentPartImage,
  ChatCompletionContentPartInputAudio,
  ChatCompletionContentPartFile,
  ChatCompletionContentPartRefusal,
  ContentArrayOfContentPart,
  Audio,
  FunctionCall,
  ChatCompletionMessageToolCallUnion,
  ChatCompletionMessageFunctionToolCall,
  ChatCompletionMessageCustomToolCall,
  ImageURL,
  InputAudio,
  FileFile
} from './openai-message-params';

export type {
  ContentBlockParam,
  TextBlockParam,
  ImageBlockParam,
  DocumentBlockParam,
  SearchResultBlockParam,
  ThinkingBlockParam,
  RedactedThinkingBlockParam,
  ToolUseBlockParam,
  ToolResultBlockParam,
  ServerToolUseBlockParam,
  WebSearchToolResultBlockParam
} from './message-params';