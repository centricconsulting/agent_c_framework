/**
 * Utility functions for message processing and normalization
 */

import type { Message } from '../events/types/CommonTypes';
import { Logger } from './logger';

/**
 * Normalize message content to ensure it's always a string
 * Handles cases where content might be an object or other non-string type
 * @param content - The content to normalize
 * @returns Normalized string content
 */
export function normalizeMessageContent(content: any): string {
  // If already a string, return as is
  if (typeof content === 'string') {
    return content;
  }
  
  // If null or undefined, return empty string
  if (content == null) {
    Logger.warn('[MessageUtils] Message content is null or undefined, using empty string');
    return '';
  }
  
  // If it's an object, try to extract text content or stringify
  if (typeof content === 'object') {
    // Check for common text properties
    if ('text' in content && typeof content.text === 'string') {
      Logger.debug('[MessageUtils] Extracted text property from content object');
      return content.text;
    }
    
    if ('content' in content && typeof content.content === 'string') {
      Logger.debug('[MessageUtils] Extracted nested content property from content object');
      return content.content;
    }
    
    if ('value' in content && typeof content.value === 'string') {
      Logger.debug('[MessageUtils] Extracted value property from content object');
      return content.value;
    }
    
    // Check if it's an array of text segments
    if (Array.isArray(content)) {
      const textParts = content
        .map(item => {
          if (typeof item === 'string') return item;
          if (typeof item === 'object' && item && 'text' in item) return item.text;
          return '';
        })
        .filter(text => text);
      
      if (textParts.length > 0) {
        Logger.debug('[MessageUtils] Extracted text from array content');
        return textParts.join('');
      }
    }
    
    // As a last resort, stringify the object
    Logger.warn('[MessageUtils] Content is an object without known text properties, stringifying:', content);
    try {
      return JSON.stringify(content, null, 2);
    } catch (error) {
      Logger.error('[MessageUtils] Failed to stringify content object:', error);
      return '[Complex Object]';
    }
  }
  
  // For any other type, convert to string
  Logger.warn(`[MessageUtils] Unexpected content type: ${typeof content}, converting to string`);
  return String(content);
}

/**
 * Normalize a message to ensure all fields are valid
 * @param message - The message to normalize
 * @returns Normalized message with guaranteed string content
 */
export function normalizeMessage(message: any): Message {
  if (!message || typeof message !== 'object') {
    Logger.error('[MessageUtils] Invalid message provided to normalizeMessage');
    return {
      role: 'system',
      content: '',
      timestamp: new Date().toISOString(),
      format: 'text'
    };
  }
  
  return {
    role: message.role || 'assistant',
    content: normalizeMessageContent(message.content),
    timestamp: message.timestamp || new Date().toISOString(),
    format: message.format || 'text'
  };
}

/**
 * Normalize an array of messages
 * @param messages - Array of messages to normalize
 * @returns Array of normalized messages with guaranteed string content
 */
export function normalizeMessages(messages: any[]): Message[] {
  if (!Array.isArray(messages)) {
    Logger.error('[MessageUtils] Invalid messages array provided to normalizeMessages');
    return [];
  }
  
  return messages.map(normalizeMessage);
}

/**
 * Check if a message has valid structure
 * @param message - The message to validate
 * @returns True if the message is valid
 */
export function isValidMessage(message: any): message is Message {
  return Boolean(
    message &&
    typeof message === 'object' &&
    typeof message.role === 'string' &&
    typeof message.content === 'string' &&
    ['user', 'assistant', 'system', 'assistant (thought)'].includes(message.role)
  );
}

export default {
  normalizeMessageContent,
  normalizeMessage,
  normalizeMessages,
  isValidMessage
};