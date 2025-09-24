/**
 * Extended chat types for the React package
 * These types extend the core Message type to support additional UI states
 */

import type { Message } from '@agentc/realtime-core';

/**
 * Chat item types that can appear in the message list
 */
export type ChatItemType = 
  | 'message'        // Regular user/assistant/system message
  | 'divider'        // Subsession start/end divider
  | 'media'          // RenderMedia content
  | 'system_alert';  // System message with severity

/**
 * Base interface for all chat items
 */
export interface BaseChatItem {
  /** Unique identifier for the item */
  id: string;
  /** Type of chat item */
  type: ChatItemType;
  /** Timestamp when the item was created */
  timestamp: string;
}

/**
 * Regular message item (extends core Message)
 */
export interface MessageChatItem extends Message {
  /** Unique identifier for the item */
  id: string;
  /** Type of chat item */
  type: 'message';
  /** Optional subsession metadata */
  isSubSession?: boolean;
  metadata?: {
    sessionId: string;
    parentSessionId?: string;
    userSessionId?: string;
  };
}

/**
 * Subsession divider item
 */
export interface DividerChatItem extends BaseChatItem {
  type: 'divider';
  /** Whether this is a start or end divider */
  dividerType: 'start' | 'end';
  /** Optional subsession metadata */
  metadata?: {
    subSessionType?: 'chat' | 'oneshot';
    subAgentType?: 'clone' | 'team' | 'assist' | 'tool';
    primeAgentKey?: string;
    subAgentKey?: string;
  };
}

/**
 * Media content item (from RenderMedia events)
 */
export interface MediaChatItem extends BaseChatItem {
  type: 'media';
  /** The media content to render */
  content: string;
  /** Media content type (e.g., "text/html", "image/svg+xml") */
  contentType: string;
  /** Optional metadata about the source */
  metadata?: {
    sentByClass?: string;
    sentByFunction?: string;
    foreignContent?: boolean;
    url?: string;
    name?: string;
  };
}

/**
 * System alert item (from SystemMessage events)
 */
export interface SystemAlertChatItem extends BaseChatItem {
  type: 'system_alert';
  /** Alert message content */
  content: string;
  /** Alert severity level */
  severity: 'info' | 'warning' | 'error';
  /** Content format */
  format: 'markdown' | 'text';
}

/**
 * Union type for all chat items
 */
export type ChatItem = 
  | MessageChatItem 
  | DividerChatItem 
  | MediaChatItem 
  | SystemAlertChatItem;

/**
 * Type guard to check if an item is a message
 */
export function isMessageItem(item: ChatItem): item is MessageChatItem {
  return item.type === 'message';
}

/**
 * Type guard to check if an item is a divider
 */
export function isDividerItem(item: ChatItem): item is DividerChatItem {
  return item.type === 'divider';
}

/**
 * Type guard to check if an item is media
 */
export function isMediaItem(item: ChatItem): item is MediaChatItem {
  return item.type === 'media';
}

/**
 * Type guard to check if an item is a system alert
 */
export function isSystemAlertItem(item: ChatItem): item is SystemAlertChatItem {
  return item.type === 'system_alert';
}

/**
 * Error info for toast notifications
 */
export interface ErrorInfo {
  /** Unique identifier for the error */
  id: string;
  /** Error message */
  message: string;
  /** Optional source of the error */
  source?: string;
  /** Timestamp when error occurred */
  timestamp: string;
  /** Whether the error has been dismissed */
  dismissed: boolean;
}