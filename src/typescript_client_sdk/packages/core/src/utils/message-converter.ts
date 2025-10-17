/**
 * Message Converter Utilities
 * 
 * Converts between server MessageParam format and client Message format
 * ensuring compatibility with UI components
 */

import type { Message, MessageContent, ContentPart } from '../events/types/CommonTypes';
import type { MessageParam, ContentBlockParam } from '../types/message-params';
import { Logger } from './logger';

/**
 * Convert a ContentBlockParam to a ContentPart for UI rendering
 */
function convertContentBlockToContentPart(block: ContentBlockParam): ContentPart | null {
  Logger.debug('[convertContentBlockToContentPart] Processing block:', block);
  
  // Handle different block types
  if (!block || typeof block !== 'object') {
    Logger.debug('[convertContentBlockToContentPart] Block is not an object, returning null');
    return null;
  }

  // Check if it already has a type field
  if (!('type' in block)) {
    Logger.debug('[convertContentBlockToContentPart] Block has no type field, returning null');
    return null;
  }

  switch (block.type) {
    case 'text':
      // Extract just the text content, ignore cache_control and citations
      return {
        type: 'text',
        text: (block as any).text || ''
      };

    case 'image':
      // Convert image block
      const imageBlock = block as any;
      return {
        type: 'image',
        source: imageBlock.source
      };

    case 'tool_use':
      // Convert tool use block
      const toolUseBlock = block as any;
      return {
        type: 'tool_use',
        id: toolUseBlock.id,
        name: toolUseBlock.name,
        input: toolUseBlock.input
      };

    case 'tool_result':
      // Convert tool result block
      const toolResultBlock = block as any;
      return {
        type: 'tool_result',
        tool_use_id: toolResultBlock.tool_use_id,
        content: toolResultBlock.content,
        is_error: toolResultBlock.is_error
      };

    case 'thinking':
    case 'redacted_thinking':
      // Convert thinking blocks to text for display
      const thinkingBlock = block as any;
      return {
        type: 'text',
        text: thinkingBlock.thinking || thinkingBlock.data || '[Thinking...]'
      };

    case 'document':
    case 'search_result':
    case 'server_tool_use':
    case 'web_search_tool_result':
      // Convert complex blocks to text representation for now
      return {
        type: 'text',
        text: `[${block.type} content]`
      };

    default:
      // Unknown block type - try to extract text if available
      if ('text' in block) {
        return {
          type: 'text',
          text: (block as any).text
        };
      }
      return null;
  }
}

/**
 * Convert MessageParam content to MessageContent for UI rendering
 */
export function convertMessageParamContent(content: string | ContentBlockParam[]): MessageContent {
  Logger.debug('[convertMessageParamContent] Input content:', content);
  
  // Handle null/undefined
  if (content === null || content === undefined) {
    Logger.debug('[convertMessageParamContent] Content is null/undefined');
    return null;
  }

  // Handle string content
  if (typeof content === 'string') {
    Logger.debug('[convertMessageParamContent] String content, returning as-is');
    return content;
  }

  // Handle array of content blocks
  if (Array.isArray(content)) {
    Logger.debug('[convertMessageParamContent] Array content with', content.length, 'blocks');
    const convertedParts: ContentPart[] = [];
    
    for (const block of content) {
      const convertedPart = convertContentBlockToContentPart(block);
      if (convertedPart) {
        convertedParts.push(convertedPart);
      } else {
        Logger.debug('[convertMessageParamContent] Block conversion returned null for:', block);
      }
    }
    
    Logger.debug('[convertMessageParamContent] Converted', convertedParts.length, 'parts');
    // Return converted parts or null if none were converted
    return convertedParts.length > 0 ? convertedParts : null;
  }

  // Fallback for unexpected types
  Logger.debug('[convertMessageParamContent] Unexpected content type:', typeof content);
  return null;
}

/**
 * Convert a MessageParam to a Message for UI rendering
 */
export function convertMessageParamToMessage(param: MessageParam): Message {
  return {
    role: param.role,
    content: convertMessageParamContent(param.content),
    timestamp: new Date().toISOString(),
    format: 'text' as const
  };
}

/**
 * Convert an array of MessageParams to Messages
 */
export function convertMessageParamsToMessages(params: MessageParam[]): Message[] {
  return params.map(convertMessageParamToMessage);
}

/**
 * Ensure a message has the correct format for UI rendering
 * This handles messages that might already be partially converted
 */
export function ensureMessageFormat(message: any): Message {
  // If it's already a proper Message, return it
  if (message && typeof message === 'object' && 'role' in message && 'content' in message) {
    // Ensure content is in the right format
    const content = message.content;
    
    // If content is an array, ALWAYS convert to ensure proper format
    // This handles cases where content blocks have extra fields like 'citations', 'cache_control', etc.
    if (Array.isArray(content)) {
      Logger.debug('[ensureMessageFormat] Processing array content with', content.length, 'items');
      Logger.debug('[ensureMessageFormat] First item:', content[0]);
      
      // Always convert array content to ensure it's normalized
      const convertedContent = convertMessageParamContent(content);
      Logger.debug('[ensureMessageFormat] Converted content:', convertedContent);
      
      return {
        ...message,
        content: convertedContent,
        timestamp: message.timestamp || new Date().toISOString(),
        format: message.format || 'text'
      };
    }
    
    // Already in correct format
    return {
      role: message.role,
      content: message.content,
      timestamp: message.timestamp || new Date().toISOString(),
      format: message.format || 'text'
    };
  }
  
  // Try to convert as MessageParam
  if (message && typeof message === 'object' && 'role' in message) {
    return convertMessageParamToMessage(message as MessageParam);
  }
  
  // Fallback - create an error message
  return {
    role: 'system',
    content: '[Invalid message format]',
    timestamp: new Date().toISOString(),
    format: 'text'
  };
}

/**
 * Ensure an array of messages has the correct format for UI rendering
 * This handles special cases like think tools and delegation tools
 */
export function ensureMessagesFormat(messages: any[]): Message[] {
  if (!Array.isArray(messages)) {
    return [];
  }
  
  const result: Message[] = [];
  
  for (let i = 0; i < messages.length; i++) {
    const message = messages[i];
    if (!message) continue;
    
    // Check if this is an assistant message with tool use blocks
    if (message.role === 'assistant' && Array.isArray(message.content)) {
      let hasProcessedContent = false;
      const textParts: string[] = [];
      
      for (const block of message.content) {
        if (block?.type === 'text') {
          textParts.push(block.text || '');
        } else if (block?.type === 'tool_use') {
          // Special handling for think tools
          if (block.name === 'think') {
            // Convert think tool to thought message
            result.push({
              role: 'assistant (thought)' as any,
              content: (block.input as any)?.thought || '',
              timestamp: new Date().toISOString(),
              format: 'markdown'
            });
            hasProcessedContent = true;
            // Skip the next message if it's the tool result for this think tool
            if (i + 1 < messages.length && messages[i + 1]?.role === 'user') {
              const nextMsg = messages[i + 1];
              if (Array.isArray(nextMsg.content)) {
                const hasThinkResult = nextMsg.content.some((b: any) => 
                  b?.type === 'tool_result' && b?.tool_use_id === block.id
                );
                if (hasThinkResult) {
                  i++; // Skip the tool result message
                }
              }
            }
          }
          // Note: Delegation tools are handled differently - they show as regular messages
          // The subsession dividers are added by the React layer based on events
        }
      }
      
      // If we have text content that wasn't processed, add it as a regular message
      if (textParts.length > 0) {
        result.push({
          role: 'assistant',
          content: textParts.join(''),
          timestamp: new Date().toISOString(),
          format: 'text'
        });
      } else if (!hasProcessedContent) {
        // No text and no special processing, convert normally
        result.push(ensureMessageFormat(message));
      }
    } else if (message.role === 'user' && Array.isArray(message.content)) {
      // Check if this is a tool result message that should be skipped
      const hasOnlyToolResults = message.content.every((block: any) => 
        block?.type === 'tool_result'
      );
      if (!hasOnlyToolResults) {
        // Convert normally if it has content other than tool results
        result.push(ensureMessageFormat(message));
      }
      // Skip pure tool result messages as they're processed with their tool use
    } else {
      // Regular message, convert normally
      result.push(ensureMessageFormat(message));
    }
  }
  
  return result;
}