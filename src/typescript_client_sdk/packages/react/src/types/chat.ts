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
  /** The media content to render (may be absent if only URL provided) */
  content?: string;
  /** Media content type (e.g., "text/markdown", "text/html", "image/svg+xml") */
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

// File Upload Types

/**
 * File attachment with upload metadata
 * Represents a file being prepared for upload or currently uploading
 */
export interface FileAttachment {
  /** The File object to be uploaded */
  file: File;
  /** Unique identifier assigned after successful upload (null if pending/uploading) */
  id: string | null;
  /** Current upload status */
  status: 'pending' | 'uploading' | 'complete' | 'error';
  /** Upload progress percentage (0-100) */
  progress: number;
  /** Error message if status is 'error' */
  error?: string;
  /** Optional preview URL for image files (generated from File object) */
  previewUrl?: string;
}

/**
 * Configuration options for the useFileUpload hook
 */
export interface UseFileUploadOptions {
  /** Maximum file size in bytes (default: 10MB) */
  maxFileSize?: number;
  /** Allowed MIME types (default: all image types) */
  allowedMimeTypes?: string[];
  /** Maximum number of files that can be attached (default: 5) */
  maxFiles?: number;
  /** Whether to automatically upload files when added (default: false) */
  autoUpload?: boolean;
  /** Whether to generate preview URLs for image files (default: true) */
  generatePreviews?: boolean;
}

/**
 * Return type for the useFileUpload hook
 * Provides file upload state management and operations
 */
export interface UseFileUploadReturn {
  /** Array of current file attachments */
  attachments: FileAttachment[];
  /** Add new files to the attachment list */
  addFiles: (files: File[]) => Promise<void>;
  /** Remove a file from the attachment list by index */
  removeFile: (index: number) => void;
  /** Upload all pending/error files */
  uploadAll: () => Promise<void>;
  /** Upload a specific file by index */
  uploadFile: (index: number) => Promise<void>;
  /** Clear all attachments */
  clearAll: () => void;
  /** Get array of successfully uploaded file IDs */
  getUploadedFileIds: () => string[];
  /** Whether any files are currently uploading */
  isUploading: boolean;
  /** Whether all files have completed upload successfully */
  allComplete: boolean;
  /** Whether any files have upload errors */
  hasErrors: boolean;
  /** Overall upload progress percentage (0-100) */
  overallProgress: number;
  /** Current validation error message, if any */
  validationError: string | null;
}

// Multimodal Message Content Types

/**
 * Text content block for multimodal messages
 */
export interface TextContentBlock {
  /** Content block type identifier */
  type: 'text';
  /** The text content */
  text: string;
}

/**
 * Image content block for multimodal messages
 */
export interface ImageContentBlock {
  /** Content block type identifier */
  type: 'image';
  /** Image source data */
  source: {
    /** Source type: base64-encoded data or external URL */
    type: 'base64' | 'url';
    /** MIME type of the image (e.g., 'image/png', 'image/jpeg') */
    media_type: string;
    /** Base64-encoded image data (present when type is 'base64') */
    data?: string;
    /** External image URL (present when type is 'url') */
    url?: string;
  };
}

/**
 * Union type for message content blocks
 * Messages can contain either text or image content blocks
 */
export type MessageContentBlock = TextContentBlock | ImageContentBlock;

/**
 * Type guard to check if a content block is text
 */
export function isTextContent(block: MessageContentBlock): block is TextContentBlock {
  return block.type === 'text';
}

/**
 * Type guard to check if a content block is an image
 */
export function isImageContent(block: MessageContentBlock): block is ImageContentBlock {
  return block.type === 'image';
}