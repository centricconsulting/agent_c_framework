/**
 * Message Helper Utilities
 * 
 * Utility functions for working with multimodal messages in the Agent C Realtime SDK.
 * These functions help extract and analyze message content that may contain text, images,
 * and other content types.
 */

import type { Message, ContentPart } from '@agentc/realtime-core';

/**
 * Check if a message contains file attachments (images)
 * 
 * @param message - The message to check
 * @returns true if the message contains at least one image content block
 * 
 * @example
 * ```typescript
 * const message = { role: 'user', content: [{ type: 'text', text: 'Look!' }, { type: 'image', source: {...} }] };
 * hasFileAttachments(message); // true
 * 
 * const textOnly = { role: 'user', content: 'Just text' };
 * hasFileAttachments(textOnly); // false
 * ```
 */
export function hasFileAttachments(message: Message): boolean {
  // Handle null content
  if (message.content === null) {
    return false;
  }
  
  // Handle string content (text-only message)
  if (typeof message.content === 'string') {
    return false;
  }
  
  // Handle array content (multimodal message)
  // Check if any content block is an image
  return message.content.some((block: ContentPart) => block.type === 'image');
}

/**
 * Count the number of images in a message
 * 
 * @param message - The message to analyze
 * @returns The number of image content blocks in the message
 * 
 * @example
 * ```typescript
 * const message = { 
 *   role: 'user', 
 *   content: [
 *     { type: 'text', text: 'Look at these!' }, 
 *     { type: 'image', source: {...} },
 *     { type: 'image', source: {...} }
 *   ] 
 * };
 * countImages(message); // 2
 * 
 * const textOnly = { role: 'user', content: 'No images here' };
 * countImages(textOnly); // 0
 * ```
 */
export function countImages(message: Message): number {
  // Handle null content
  if (message.content === null) {
    return 0;
  }
  
  // Handle string content (text-only message)
  if (typeof message.content === 'string') {
    return 0;
  }
  
  // Handle array content (multimodal message)
  // Count image content blocks
  return message.content.filter((block: ContentPart) => block.type === 'image').length;
}

/**
 * Extract display text from a message
 * 
 * For multimodal messages with mixed content types, this extracts only the text portions
 * and concatenates them. Image blocks and other non-text content are ignored.
 * 
 * @param message - The message to extract text from
 * @returns The text content of the message, or empty string if no text content
 * 
 * @example
 * ```typescript
 * // String content
 * const simple = { role: 'user', content: 'Hello world' };
 * getMessageDisplayText(simple); // 'Hello world'
 * 
 * // Array content with mixed types
 * const mixed = { 
 *   role: 'user', 
 *   content: [
 *     { type: 'text', text: 'Check out ' },
 *     { type: 'image', source: {...} },
 *     { type: 'text', text: ' this image!' }
 *   ] 
 * };
 * getMessageDisplayText(mixed); // 'Check out  this image!'
 * 
 * // Null content
 * const empty = { role: 'assistant', content: null };
 * getMessageDisplayText(empty); // ''
 * ```
 */
export function getMessageDisplayText(message: Message): string {
  // Handle null content
  if (message.content === null) {
    return '';
  }
  
  // Handle string content (text-only message)
  if (typeof message.content === 'string') {
    return message.content;
  }
  
  // Handle array content (multimodal message)
  // Extract text from all text content blocks and join them
  return message.content
    .filter((block: ContentPart) => block.type === 'text')
    .map((block: ContentPart) => {
      // Type guard to ensure we're working with TextContentPart
      if (block.type === 'text' && 'text' in block) {
        return block.text;
      }
      return '';
    })
    .join('');
}
