/**
 * Anthropic MessageParam TypeScript Definitions
 * 
 * This file contains comprehensive type definitions for Anthropic's MessageParam
 * model and all related types, converted from the Python SDK.
 * 
 * These types match the actual server data structures exactly.
 */

// Core MessageParam Interface
export interface MessageParam {
  content: string | Array<ContentBlockParam>;
  role: "user" | "assistant";
}

// Union type for all possible content block parameters
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
  | WebSearchToolResultBlockParam
  | ContentBlock;

// Text Block
export interface TextBlockParam {
  text: string;
  type: "text";
  cache_control?: CacheControlEphemeralParam;
  citations?: TextCitationParam[];
}

// Image Block
export interface ImageBlockParam {
  source: Base64ImageSourceParam | URLImageSourceParam;
  type: "image";
  cache_control?: CacheControlEphemeralParam;
}

export interface Base64ImageSourceParam {
  data: string; // base64 encoded
  media_type: "image/jpeg" | "image/png" | "image/gif" | "image/webp";
  type: "base64";
}

export interface URLImageSourceParam {
  type: "url";
  url: string;
}

// Document Block
export interface DocumentBlockParam {
  source: DocumentSource;
  type: "document";
  cache_control?: CacheControlEphemeralParam;
  citations?: CitationsConfigParam;
  context?: string;
  title?: string;
}

export type DocumentSource = 
  | Base64PDFSourceParam
  | PlainTextSourceParam
  | ContentBlockSourceParam
  | URLPDFSourceParam;

export interface Base64PDFSourceParam {
  data: string; // base64 encoded
  media_type: "application/pdf";
  type: "base64";
}

export interface PlainTextSourceParam {
  data: string;
  media_type: "text/plain";
  type: "text";
}

export interface ContentBlockSourceParam {
  content: string | ContentBlockSourceContentParam[];
  type: "content";
}

export interface URLPDFSourceParam {
  type: "url";
  url: string;
}

// Search Result Block
export interface SearchResultBlockParam {
  content: TextBlockParam[];
  source: string;
  title: string;
  type: "search_result";
  cache_control?: CacheControlEphemeralParam;
  citations?: CitationsConfigParam;
}

// Thinking Blocks
export interface ThinkingBlockParam {
  signature: string;
  thinking: string;
  type: "thinking";
}

export interface RedactedThinkingBlockParam {
  data: string;
  type: "redacted_thinking";
}

// Tool Use Block
export interface ToolUseBlockParam {
  id: string;
  input: Record<string, any>; // object type
  name: string;
  type: "tool_use";
  cache_control?: CacheControlEphemeralParam;
}

// Tool Result Block
export interface ToolResultBlockParam {
  tool_use_id: string;
  type: "tool_result";
  cache_control?: CacheControlEphemeralParam;
  content?: string | ToolResultContent[];
  is_error?: boolean;
}

export type ToolResultContent = 
  | TextBlockParam
  | ImageBlockParam
  | SearchResultBlockParam
  | DocumentBlockParam;

// Server Tool Use Block
export interface ServerToolUseBlockParam {
  id: string;
  input: Record<string, any>; // object type
  name: "web_search"; // literal type
  type: "server_tool_use";
  cache_control?: CacheControlEphemeralParam;
}

// Web Search Tool Result Block
export interface WebSearchToolResultBlockParam {
  content: WebSearchToolResultBlockParamContentParam;
  tool_use_id: string;
  type: "web_search_tool_result";
  cache_control?: CacheControlEphemeralParam;
}

export type WebSearchToolResultBlockParamContentParam = 
  | WebSearchResultBlockParam[]
  | WebSearchToolRequestErrorParam;

export interface WebSearchResultBlockParam {
  encrypted_content: string;
  title: string;
  type: "web_search_result";
  url: string;
  page_age?: string;
}

export interface WebSearchToolRequestErrorParam {
  error_code: "invalid_tool_input" | "unavailable" | "max_uses_exceeded" | "too_many_requests" | "query_too_long";
  type: "web_search_tool_result_error";
}

// Supporting Types
export interface CacheControlEphemeralParam {
  type: "ephemeral";
  ttl?: "5m" | "1h"; // Time-to-live: 5 minutes or 1 hour (defaults to 5m)
}

export interface CitationsConfigParam {
  enabled?: boolean;
}

// Citation Types
export type TextCitationParam = 
  | CitationCharLocationParam
  | CitationPageLocationParam
  | CitationContentBlockLocationParam
  | CitationWebSearchResultLocationParam
  | CitationSearchResultLocationParam;

// Citation Location Types (placeholder interfaces - extend as needed)
export interface CitationCharLocationParam {
  type: "char";
  start?: number;
  end?: number;
}

export interface CitationPageLocationParam {
  type: "page";
  page?: number;
}

export interface CitationContentBlockLocationParam {
  type: "content_block";
  index?: number;
}

export interface CitationWebSearchResultLocationParam {
  type: "web_search_result";
  index?: number;
}

export interface CitationSearchResultLocationParam {
  type: "search_result";
  index?: number;
}

// Content Block Source Content
export type ContentBlockSourceContentParam = TextBlockParam | ImageBlockParam;

// Content Block Union Type (Response blocks, not parameters)
export type ContentBlock = 
  | TextBlock
  | ThinkingBlock
  | RedactedThinkingBlock
  | ToolUseBlock
  | ServerToolUseBlock
  | WebSearchToolResultBlock;

// Response Block Types
export interface TextBlock {
  text: string;
  type: "text";
  citations?: any[]; // Extend as needed
}

export interface ThinkingBlock {
  signature?: string;
  thinking: string;
  type: "thinking";
}

export interface RedactedThinkingBlock {
  data: string;
  type: "redacted_thinking";
}

export interface ToolUseBlock {
  id: string;
  input: Record<string, any>;
  name: string;
  type: "tool_use";
}

export interface ServerToolUseBlock {
  id: string;
  input: Record<string, any>;
  name: "web_search";
  type: "server_tool_use";
}

export interface WebSearchToolResultBlock {
  content: any; // Extend as needed
  tool_use_id: string;
  type: "web_search_tool_result";
}

// Type guards for runtime type checking
export function isTextBlockParam(block: ContentBlockParam): block is TextBlockParam {
  return 'type' in block && block.type === 'text';
}

export function isImageBlockParam(block: ContentBlockParam): block is ImageBlockParam {
  return 'type' in block && block.type === 'image';
}

export function isToolUseBlockParam(block: ContentBlockParam): block is ToolUseBlockParam {
  return 'type' in block && block.type === 'tool_use';
}

export function isToolResultBlockParam(block: ContentBlockParam): block is ToolResultBlockParam {
  return 'type' in block && block.type === 'tool_result';
}

export function isThinkingBlockParam(block: ContentBlockParam): block is ThinkingBlockParam {
  return 'type' in block && block.type === 'thinking';
}

export function isDocumentBlockParam(block: ContentBlockParam): block is DocumentBlockParam {
  return 'type' in block && block.type === 'document';
}

export function isSearchResultBlockParam(block: ContentBlockParam): block is SearchResultBlockParam {
  return 'type' in block && block.type === 'search_result';
}