/**
 * MessageBuilder - Handles streaming text accumulation and message building
 * Manages partial messages until CompletionEvent
 */

import { Message, ToolCall, ToolResult, StopReason } from './types/CommonTypes';
import { Logger } from '../utils/logger';

/**
 * Enhanced message type with additional metadata
 */
export interface EnhancedMessage extends Message {
  id: string;
  type: 'message' | 'media' | 'notification';  // 'thought' removed - use role instead
  status: 'streaming' | 'complete' | 'error';
  contentType?: 'text' | 'html' | 'svg' | 'image' | 'unknown';
  metadata?: MessageMetadata;
  toolCalls?: ToolCall[];
  toolResults?: ToolResult[];
  isCollapsed?: boolean; // For thoughts (identified by role === 'assistant (thought)')
}

/**
 * Metadata associated with a message
 */
export interface MessageMetadata {
  inputTokens?: number;
  outputTokens?: number;
  stopReason?: StopReason;
  toolCalls?: ToolCall[];
  toolResults?: ToolResult[];
  sentByClass?: string;
  sentByFunction?: string;
  timestamp?: string;
}

/**
 * Message types that can be built
 */
export type MessageType = 'assistant' | 'thought' | 'user';

/**
 * Builds messages from streaming content
 */
export class MessageBuilder {
  private currentMessage: Partial<EnhancedMessage> | null = null;
  private messageType: MessageType = 'assistant';
  private content: string = '';
  
  constructor() {
  }
  
  /**
   * Start building a new message
   */
  startMessage(type: MessageType): void {
    if (this.currentMessage) {
      Logger.warn('[MessageBuilder] Starting new message while another is in progress');
    }
    
    this.currentMessage = {
      id: this.generateMessageId(),
      role: type === 'thought' ? 'assistant (thought)' : type === 'user' ? 'user' : 'assistant',
      content: '',
      timestamp: new Date().toISOString(),
      type: 'message', // Always 'message' - UI only recognizes this type
      status: 'streaming',
      format: 'text'
    };
    
    this.messageType = type;
    this.content = '';
    
    Logger.debug(`[MessageBuilder] Started ${type} message with ID: ${this.currentMessage.id}`);
  }
  
  /**
   * Append text to the current message
   */
  appendText(delta: string): void {
    if (!this.currentMessage) {
      Logger.warn('[MessageBuilder] Attempting to append text without active message');
      this.startMessage('assistant');
    }
    
    this.content += delta;
    if (this.currentMessage) {
      this.currentMessage.content = this.content;
    }
    
    Logger.debug(`[MessageBuilder] Appended ${delta.length} characters to message`);
  }
  
  /**
   * Finalize the current message
   */
  finalize(metadata?: MessageMetadata): EnhancedMessage {
    if (!this.currentMessage) {
      throw new Error('No message to finalize');
    }
    
    const finalMessage: EnhancedMessage = {
      id: this.currentMessage.id!,
      role: this.currentMessage.role as EnhancedMessage['role'],
      content: this.content,
      timestamp: this.currentMessage.timestamp!,
      type: this.currentMessage.type as EnhancedMessage['type'],
      status: 'complete',
      format: this.currentMessage.format || 'text',
      metadata: metadata || {},
      toolCalls: metadata?.toolCalls,
      toolResults: metadata?.toolResults
    };
    
    // Set collapsed state for thoughts by default
    // Check role instead of type since type is always 'message' now
    if (finalMessage.role === 'assistant (thought)') {
      finalMessage.isCollapsed = true;
    }
    
    Logger.info(`[MessageBuilder] Finalized ${this.messageType} message`, {
      id: finalMessage.id,
      contentLength: typeof finalMessage.content === 'string' 
        ? finalMessage.content.length 
        : Array.isArray(finalMessage.content) 
          ? finalMessage.content.length 
          : 0,
      hasToolCalls: !!finalMessage.toolCalls?.length,
      inputTokens: metadata?.inputTokens,
      outputTokens: metadata?.outputTokens
    });
    
    return finalMessage;
  }
  
  /**
   * Get the current message being built
   */
  getCurrentMessage(): Partial<EnhancedMessage> | null {
    if (!this.currentMessage) {
      return null;
    }
    
    return {
      ...this.currentMessage,
      content: this.content
    };
  }
  
  /**
   * Check if there's a message currently being built
   */
  hasCurrentMessage(): boolean {
    return this.currentMessage !== null;
  }
  
  /**
   * Get the type of the current message
   */
  getCurrentMessageType(): MessageType | null {
    return this.currentMessage ? this.messageType : null;
  }
  
  /**
   * Reset the builder state
   */
  reset(): void {
    this.currentMessage = null;
    this.content = '';
    this.messageType = 'assistant';
    Logger.debug('[MessageBuilder] MessageBuilder reset');
  }
  
  /**
   * Generate a unique message ID
   */
  private generateMessageId(): string {
    return `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }
  
  /**
   * Get current content length
   */
  getContentLength(): number {
    return this.content.length;
  }
  
  /**
   * Check if the builder is currently streaming
   */
  isStreaming(): boolean {
    return this.currentMessage !== null && this.currentMessage.status === 'streaming';
  }
}