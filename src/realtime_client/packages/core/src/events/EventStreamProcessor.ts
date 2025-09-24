/**
 * EventStreamProcessor - Central coordinator for processing incoming events and building messages
 * Routes events to appropriate handlers and coordinates message building
 */

import { SessionManager } from '../session/SessionManager';
import { MessageBuilder } from './MessageBuilder';
import { ToolCallManager } from './ToolCallManager';
import { RichMediaHandler } from './RichMediaHandler';
import { 
  ServerEvent,
  TextDeltaEvent, 
  ThoughtDeltaEvent,
  CompletionEvent,
  ToolSelectDeltaEvent,
  ToolCallEvent,
  RenderMediaEvent,
  InteractionEvent,
  SystemMessageEvent,
  ErrorEvent,
  HistoryDeltaEvent,
  ChatSessionChangedEvent,
  UserMessageEvent,
  OpenAIUserMessageEvent,
  AnthropicUserMessageEvent,
  SubsessionStartedEvent,
  SubsessionEndedEvent,
  CancelledEvent
} from './types/ServerEvents';
import { Message, MessageContent, ContentPart, ToolCall } from './types/CommonTypes';
import { ChatSession } from '../types/chat-session';
import { 
  MessageParam,
  ContentBlockParam,
  isTextBlockParam,
  isImageBlockParam,
  isToolUseBlockParam,
  isDocumentBlockParam,
  isThinkingBlockParam
} from '../types/message-params';
import { Logger } from '../utils/logger';

/**
 * Central event processor that routes server events to appropriate handlers
 */
export class EventStreamProcessor {
  private messageBuilder: MessageBuilder;
  private toolCallManager: ToolCallManager;
  private richMediaHandler: RichMediaHandler;
  private sessionManager: SessionManager;
  private userSessionId: string | null = null;
  
  constructor(sessionManager: SessionManager) {
    this.sessionManager = sessionManager;
    this.messageBuilder = new MessageBuilder();
    this.toolCallManager = new ToolCallManager();
    this.richMediaHandler = new RichMediaHandler();
  }
  
  /**
   * Set the user session ID for sub-session detection
   */
  setUserSessionId(id: string): void {
    this.userSessionId = id;
    Logger.debug(`[EventStreamProcessor] User session ID set: ${id}`);
  }
  
  /**
   * Check if an event represents a sub-session
   */
  private isSubSession(event: any): boolean {
    // Primary check: use event fields if available
    if (event.user_session_id && event.session_id) {
      return event.session_id !== event.user_session_id;
    }
    
    // Fallback: use stored user session ID
    if (this.userSessionId && event.session_id) {
      return event.session_id !== this.userSessionId;
    }
    
    return false;
  }
  
  /**
   * Normalize MessageParam content to MessageContent format for UI compatibility
   * Handles the conversion from Anthropic ContentBlockParam[] to simplified ContentPart[]
   */
  private normalizeMessageContent(content: string | ContentBlockParam[]): MessageContent {
    // Handle simple string content
    if (typeof content === 'string') {
      return content;
    }
    
    // Handle array of content blocks
    if (Array.isArray(content)) {
      // First, check if all blocks are text blocks
      // If so, concatenate them into a single string for proper markdown rendering
      const allTextBlocks = content.every(block => isTextBlockParam(block));
      
      if (allTextBlocks) {
        // Concatenate all text blocks into a single string
        const concatenatedText = content
          .filter(isTextBlockParam)
          .map(block => block.text)
          .join('');
        
        return concatenatedText;
      }
      
      // For mixed content, convert ContentBlockParam[] to ContentPart[]
      const normalizedParts: ContentPart[] = [];
      
      for (const block of content) {
        if (isTextBlockParam(block)) {
          normalizedParts.push({
            type: 'text',
            text: block.text
          });
        } else if (isImageBlockParam(block)) {
          normalizedParts.push({
            type: 'image',
            source: block.source
          });
        } else if (isToolUseBlockParam(block)) {
          normalizedParts.push({
            type: 'tool_use',
            id: block.id,
            name: block.name,
            input: block.input
          });
        } else if ('type' in block && block.type === 'tool_result') {
          // Handle tool result blocks
          normalizedParts.push({
            type: 'tool_result',
            tool_use_id: (block as any).tool_use_id,
            content: typeof (block as any).content === 'string' ? (block as any).content : '',
            is_error: (block as any).is_error
          });
        } else if (isThinkingBlockParam(block)) {
          // Convert thinking blocks to text for now
          normalizedParts.push({
            type: 'text',
            text: `[Thinking] ${block.thinking}`
          });
        } else if (isDocumentBlockParam(block)) {
          // For documents, create a text representation
          normalizedParts.push({
            type: 'text',
            text: `[Document: ${block.title || 'Untitled'}]`
          });
        } else {
          // For any other block types, try to extract text or skip
          Logger.debug(`[EventStreamProcessor] Skipping unsupported content block type: ${(block as any).type}`);
        }
      }
      
      // If we have normalized parts, return them
      if (normalizedParts.length > 0) {
        return normalizedParts;
      }
    }
    
    // Return empty string as fallback
    return '';
  }
  
  /**
   * Convert a MessageParam to a Message with proper content normalization
   */
  private convertMessageParam(param: MessageParam): Message {
    const normalizedContent = this.normalizeMessageContent(param.content);
    
    return {
      role: param.role as Message['role'],
      content: normalizedContent,
      timestamp: new Date().toISOString(),
      format: 'text' as const
    };
  }
  
  /**
   * Convert ServerChatSession to ChatSession with normalized messages
   */
  private convertServerSession(serverSession: any): ChatSession {
    // Check if this is already a ChatSession (has Message[]) or needs conversion
    const needsConversion = serverSession.messages && 
      serverSession.messages.length > 0 && 
      serverSession.messages[0] && 
      !('timestamp' in serverSession.messages[0]);
    
    if (!needsConversion) {
      // Already in the correct format but ensure messages is an array
      return {
        ...serverSession,
        messages: serverSession.messages || []
      } as ChatSession;
    }
    
    // Convert messages from MessageParam[] to Message[]
    const convertedMessages: Message[] = [];
    
    if (serverSession.messages && Array.isArray(serverSession.messages)) {
      for (const msg of serverSession.messages) {
        try {
          const convertedMessage = this.convertMessageParam(msg);
          convertedMessages.push(convertedMessage);
        } catch (error) {
          Logger.error('[EventStreamProcessor] Failed to convert message:', error);
          // Create a fallback message
          convertedMessages.push({
            role: msg.role || 'assistant',
            content: '[Message could not be displayed]',
            timestamp: new Date().toISOString(),
            format: 'text'
          });
        }
      }
    }
    
    return {
      ...serverSession,
      messages: convertedMessages
    } as ChatSession;
  }
  
  /**
   * Process incoming server events
   */
  processEvent(event: ServerEvent): void {
    // Filter out ignored events early
    const IGNORED_EVENTS = ['history', 'history_delta', 'complete_thought', 'system_prompt'];
    
    if (IGNORED_EVENTS.includes(event.type)) {
      Logger.debug(`[EventStreamProcessor] Ignoring event type: ${event.type}`);
      return;
    }
    
    Logger.debug(`[EventStreamProcessor] Processing event: ${event.type}`);
    
    switch (event.type) {
      case 'interaction':
        this.handleInteraction(event);
        break;
      case 'text_delta':
        this.handleTextDelta(event);
        break;
      case 'thought_delta':
        this.handleThoughtDelta(event);
        break;
      case 'completion':
        this.handleCompletion(event);
        break;
      case 'tool_select_delta':
        this.handleToolSelect(event);
        break;
      case 'tool_call':
        this.handleToolCall(event);
        break;
      case 'render_media':
        this.handleRenderMedia(event);
        break;
      case 'system_message':
        this.handleSystemMessage(event);
        break;
      case 'error':
        this.handleError(event);
        break;
      case 'history_delta':
        this.handleHistoryDelta(event);
        break;
      case 'chat_session_changed':
        this.handleChatSessionChanged(event);
        break;
      case 'user_message':
        this.handleUserMessage(event as UserMessageEvent);
        break;
      case 'anthropic_user_message':
        this.handleAnthropicUserMessage(event as AnthropicUserMessageEvent);
        break;
      case 'subsession_started':
        this.handleSubsessionStarted(event as SubsessionStartedEvent);
        break;
      case 'subsession_ended':
        this.handleSubsessionEnded(event as SubsessionEndedEvent);
        break;
      case 'cancelled':
        this.handleCancelled(event as CancelledEvent);
        break;
      // Other events are handled elsewhere or don't require processing here
      default:
        Logger.debug(`[EventStreamProcessor] Event type ${event.type} not handled by EventStreamProcessor`);
    }
  }
  
  /**
   * Handle interaction lifecycle events
   */
  private handleInteraction(event: InteractionEvent): void {
    if (event.started) {
      Logger.info(`[EventStreamProcessor] Interaction started: ${event.id}`);
      
      // Reset state for new interaction
      this.messageBuilder.reset();
      this.toolCallManager.reset();
    } else {
      Logger.info(`[EventStreamProcessor] Interaction ended: ${event.id}`);
    }
  }
  
  /**
   * Handle streaming text delta events
   */
  private handleTextDelta(event: TextDeltaEvent): void {
    // Start a new message if needed
    if (!this.messageBuilder.hasCurrentMessage()) {
      Logger.debug('[EventStreamProcessor] Starting new assistant message for text delta');
      this.messageBuilder.startMessage('assistant');
    }
    
    // Append the text delta
    Logger.debug('[EventStreamProcessor] Appending text delta:', {
      deltaContent: event.content,
      deltaLength: event.content.length
    });
    this.messageBuilder.appendText(event.content);
    
    // Update the streaming message in session manager
    const currentMessage = this.messageBuilder.getCurrentMessage();
    if (currentMessage) {
      Logger.debug('[EventStreamProcessor] Emitting message-streaming:', {
        sessionId: event.session_id,
        messageContent: currentMessage.content,
        messageRole: currentMessage.role,
        messageStatus: currentMessage.status
      });
      this.sessionManager.emit('message-streaming', {
        sessionId: event.session_id,
        message: currentMessage
      });
    } else {
      Logger.warn('[EventStreamProcessor] No current message after text delta');
    }
  }
  
  /**
   * Handle streaming thought delta events
   */
  private handleThoughtDelta(event: ThoughtDeltaEvent): void {
    // Start a new thought message if needed
    if (!this.messageBuilder.hasCurrentMessage() || this.messageBuilder.getCurrentMessageType() !== 'thought') {
      // If there's a regular message in progress, finalize it first
      if (this.messageBuilder.hasCurrentMessage()) {
        const message = this.messageBuilder.finalize();
        this.sessionManager.emit('message-complete', {
          sessionId: event.session_id,
          message
        });
      }
      this.messageBuilder.startMessage('thought');
      
      // Remove any "Agent is thinking..." notification when thought deltas start
      // We check all active notifications for the think tool
      const activeNotifications = this.toolCallManager.getActiveNotifications();
      activeNotifications.forEach(notification => {
        if (notification.toolName === 'think') {
          this.sessionManager.emit('tool-notification-removed', notification.id);
        }
      });
    }
    
    // Append the thought delta
    this.messageBuilder.appendText(event.content);
    
    // Update the streaming message
    const currentMessage = this.messageBuilder.getCurrentMessage();
    if (currentMessage) {
      this.sessionManager.emit('message-streaming', {
        sessionId: event.session_id,
        message: currentMessage
      });
    }
  }
  
  /**
   * Handle completion events
   */
  private handleCompletion(event: CompletionEvent): void {
    if (!event.running && this.messageBuilder.hasCurrentMessage()) {
      // Get any completed tool calls (which may include results)
      const completedToolCalls = this.toolCallManager.getCompletedToolCalls();
      
      // Separate tool calls from tool results
      const toolCalls = completedToolCalls.map(tc => ({
        id: tc.id,
        type: tc.type,
        name: tc.name,
        input: tc.input
      })) as ToolCall[];
      
      const toolResults = completedToolCalls
        .filter(tc => tc.result)
        .map(tc => tc.result!);
      
      // Finalize the message with metadata
      const message = this.messageBuilder.finalize({
        inputTokens: event.input_tokens,
        outputTokens: event.output_tokens,
        stopReason: event.stop_reason,
        toolCalls: toolCalls.length > 0 ? toolCalls : undefined,
        toolResults: toolResults.length > 0 ? toolResults : undefined
      });
      
      // Clear the completed tool calls after attaching them to the message
      if (completedToolCalls.length > 0) {
        this.toolCallManager.clearCompleted();
      }
      
      // Add to session and emit completion
      const session = this.sessionManager.getCurrentSession();
      if (session) {
        session.messages.push(message);
        if (event.input_tokens || event.output_tokens) {
          session.token_count += (event.input_tokens || 0) + (event.output_tokens || 0);
        }
        session.updated_at = new Date().toISOString();
      }
      
      this.sessionManager.emit('message-complete', {
        sessionId: event.session_id,
        message
      });
      
      // Reset message builder for next message
      this.messageBuilder.reset();
    }
  }
  
  /**
   * Handle tool selection events (before execution)
   */
  private handleToolSelect(event: ToolSelectDeltaEvent): void {
    const notification = this.toolCallManager.onToolSelect(event);
    
    // Special handling for the "think" tool
    const toolCall = event.tool_calls[0];
    if (toolCall && toolCall.name === 'think') {
      // For the think tool, we emit a special notification
      // The thought deltas will be handled separately
      this.sessionManager.emit('tool-notification', {
        ...notification,
        toolName: 'think',
        status: 'preparing' as const
      });
    } else {
      this.sessionManager.emit('tool-notification', notification);
    }
  }
  
  /**
   * Handle tool call events (during/after execution)
   */
  private handleToolCall(event: ToolCallEvent): void {
    // Check if this is the think tool - if so, ignore it
    const toolCall = event.tool_calls[0];
    if (toolCall && toolCall.name === 'think') {
      // For the think tool, we ignore the tool_call events
      // as the content is already rendered via thought deltas
      Logger.debug('[EventStreamProcessor] Ignoring tool_call event for think tool');
      // Still remove the notification
      this.sessionManager.emit('tool-notification-removed', toolCall.id);
      return;
    }
    
    if (event.active) {
      // Tool is executing
      const notification = this.toolCallManager.onToolCallActive(event);
      if (notification) {
        this.sessionManager.emit('tool-notification', notification);
      }
    } else {
      // Tool completed
      this.toolCallManager.onToolCallComplete(event);
      
      // Emit tool-call-complete event for UI to track results
      this.sessionManager.emit('tool-call-complete', {
        toolCalls: event.tool_calls,
        toolResults: event.tool_results
      });
      
      // Remove notifications for completed tools
      event.tool_calls.forEach(tc => {
        this.sessionManager.emit('tool-notification-removed', tc.id);
      });
    }
  }
  
  /**
   * Handle rich media rendering events
   */
  private handleRenderMedia(event: RenderMediaEvent): void {
    const media = this.richMediaHandler.processRenderMediaEvent(event);
    
    // Emit media event for UI
    this.sessionManager.emit('media-added', {
      sessionId: event.session_id,
      media: {
        id: media.id,
        role: 'assistant',
        type: 'media',
        content: media.content,
        contentType: media.type,
        timestamp: media.metadata.timestamp.toISOString(),
        status: 'complete',
        metadata: {
          ...media.metadata,
          timestamp: media.metadata.timestamp.toISOString()
        }
      }
    });
  }
  
  /**
   * Handle system messages
   * SystemMessageEvent is a SessionEvent that should be displayed in the chat as an alert-style bubble
   * Must maintain API naming consistency - emit as 'system_message' not 'system-notification'
   */
  private handleSystemMessage(event: SystemMessageEvent): void {
    // Emit the system message event with all original fields preserved
    // This maintains API naming consistency with the server event type
    this.sessionManager.emit('system_message', {
      type: event.type,
      session_id: event.session_id,
      role: event.role,
      content: event.content,
      format: event.format,
      severity: event.severity || 'info',
      // Include session event fields if present
      parent_session_id: event.parent_session_id,
      user_session_id: event.user_session_id
    });
  }
  
  /**
   * Handle error events
   * ErrorEvent is NOT a SessionEvent - it indicates unhandled server errors
   * Should be displayed as a toast notification, not in the chat stream
   */
  private handleError(event: ErrorEvent): void {
    // Emit as 'error' event for toast notifications
    // This is distinct from system messages and should not appear in chat
    this.sessionManager.emit('error', {
      type: event.type,
      message: event.message,
      source: event.source,
      timestamp: new Date().toISOString()
    });
  }
  
  /**
   * Handle history delta events (incremental history updates)
   */
  private handleHistoryDelta(event: HistoryDeltaEvent): void {
    const session = this.sessionManager.getCurrentSession();
    if (session) {
      // Convert and add the new messages to the session
      event.messages.forEach(msg => {
        // Check if the message needs conversion
        if (!('timestamp' in msg)) {
          // This is a MessageParam, convert it
          const convertedMessage = this.convertMessageParam(msg as any);
          session.messages.push(convertedMessage);
        } else {
          // Already a proper Message
          session.messages.push(msg);
        }
      });
      session.updated_at = new Date().toISOString();
      
      // Emit update event
      this.sessionManager.emit('sessions-updated', {
        sessions: this.sessionManager.getAllSessions()
      });
    }
  }
  
  /**
   * Handle chat session changed events
   * This is called when resuming an existing session or switching sessions
   */
  private handleChatSessionChanged(event: ChatSessionChangedEvent): void {
    if (!event.chat_session) {
      Logger.warn('[EventStreamProcessor] ChatSessionChangedEvent received without chat_session');
      return;
    }
    
    // Extract user_session_id if available for sub-session detection
    // ChatSessionChangedEvent doesn't extend SessionEvent, so check if it exists
    const eventWithSessionInfo = event as any;
    if (eventWithSessionInfo.user_session_id) {
      this.setUserSessionId(eventWithSessionInfo.user_session_id);
    } else if (event.chat_session.session_id) {
      // If no explicit user_session_id, assume the current session is the user session
      this.setUserSessionId(event.chat_session.session_id);
    }
    
    // Convert the server session to runtime ChatSession with normalized messages
    const session = this.convertServerSession(event.chat_session);
    
    Logger.info(`[EventStreamProcessor] Processing session change: ${session.session_id} with ${session.messages?.length || 0} messages and agent_config: ${session.agent_config?.key || 'none'}`);
    
    // Update the session in SessionManager with the converted version
    this.sessionManager.setCurrentSession(session);
    
    // Reset the message builder for new session context
    this.messageBuilder.reset();
    
    // Use new event-based approach instead of bulk loading
    if (event.chat_session.messages && event.chat_session.messages.length > 0) {
      // Check if messages need conversion (are they MessageParam[] or Message[]?)
      const firstMessage = event.chat_session.messages[0];
      
      if (firstMessage && 'timestamp' in firstMessage) {
        // Already converted to Message[] - use the existing bulk loading for now
        // This happens when messages are already in runtime format
        this.sessionManager.emit('session-messages-loaded', {
          sessionId: session.session_id,
          messages: session.messages || []
        });
      } else {
        // Raw MessageParam[] from server - process through event system
        this.mapResumedMessagesToEvents(event.chat_session.messages as MessageParam[], event.chat_session.session_id);
      }
    } else {
      // Empty session - clear messages
      this.sessionManager.emit('session-messages-loaded', {
        sessionId: event.chat_session.session_id,
        messages: []
      });
    }
  }
  
  /**
   * Handle Anthropic-specific user message events with sub-session detection
   */
  private handleAnthropicUserMessage(event: AnthropicUserMessageEvent): void {
    // Convert the Anthropic message to our normalized format
    const messageParam = event.message as any;
    let message: Message;
    
    // Check if it has the expected MessageParam structure
    if (messageParam && messageParam.content !== undefined && messageParam.role !== undefined) {
      message = this.convertMessageParam(messageParam);
    } else {
      // Fallback for malformed messages
      message = {
        role: 'user',
        content: JSON.stringify(messageParam) || '[User message]',
        timestamp: new Date().toISOString(),
        format: 'text'
      };
    }
    
    // Check for sub-session using SessionEvent fields
    if (this.isSubSession(event)) {
      // Add sub-session metadata to the message
      (message as any).isSubSession = true;
      (message as any).metadata = {
        sessionId: event.session_id,
        parentSessionId: event.parent_session_id,
        userSessionId: event.user_session_id
      };
      
      Logger.debug(`[EventStreamProcessor] Sub-session detected: ${event.session_id} (parent: ${event.parent_session_id})`);
    }
    
    // Add to current session
    const session = this.sessionManager.getCurrentSession();
    if (session) {
      session.messages.push(message);
      session.updated_at = new Date().toISOString();
      
      // Emit for UI consumption
      this.sessionManager.emit('message-added', {
        sessionId: session.session_id,
        message
      });
    }
    
    // Also emit the original user-message event for backward compatibility
    this.sessionManager.emit('user-message', {
      vendor: event.vendor,
      message: event.message
    });
  }
  
  /**
   * Handle user message events (vendor-specific format)
   */
  private handleUserMessage(event: UserMessageEvent | OpenAIUserMessageEvent | AnthropicUserMessageEvent): void {
    // Delegate to specific handler if it's an Anthropic message
    if (event.type === 'anthropic_user_message') {
      this.handleAnthropicUserMessage(event as AnthropicUserMessageEvent);
      return;
    }
    
    // Convert based on vendor
    let message: Message;
    
    if ('message' in event && event.vendor === 'openai') {
      // Handle OpenAI format if needed
      const openAiEvent = event as OpenAIUserMessageEvent;
      const openAiMessage = openAiEvent.message;
      
      // Try to extract content from OpenAI format
      if (typeof openAiMessage === 'string') {
        message = {
          role: 'user',
          content: openAiMessage,
          timestamp: new Date().toISOString(),
          format: 'text'
        };
      } else if (openAiMessage && typeof openAiMessage === 'object') {
        // Handle structured OpenAI message
        message = {
          role: 'user',
          content: (openAiMessage as any).content || JSON.stringify(openAiMessage),
          timestamp: new Date().toISOString(),
          format: 'text'
        };
      } else {
        message = {
          role: 'user',
          content: '[User message]',
          timestamp: new Date().toISOString(),
          format: 'text'
        };
      }
    } else {
      // Handle generic user message format
      // The event likely has a message field that's not in the type definition
      const eventWithMessage = event as any;
      
      if (eventWithMessage.message) {
        // Extract content from the message field
        if (typeof eventWithMessage.message === 'string') {
          message = {
            role: 'user',
            content: eventWithMessage.message,
            timestamp: new Date().toISOString(),
            format: 'text'
          };
        } else if (eventWithMessage.message && typeof eventWithMessage.message === 'object') {
          // Handle structured message - could be MessageParam format
          const messageParam = eventWithMessage.message;
          if (messageParam.content !== undefined) {
            message = {
              role: messageParam.role || 'user',
              content: messageParam.content,
              timestamp: new Date().toISOString(),
              format: 'text'
            };
          } else {
            // Fallback to stringifying the object
            message = {
              role: 'user',
              content: JSON.stringify(eventWithMessage.message),
              timestamp: new Date().toISOString(),
              format: 'text'
            };
          }
        } else {
          // Fallback if message field exists but is neither string nor object
          message = {
            role: 'user',
            content: '[User message]',
            timestamp: new Date().toISOString(),
            format: 'text'
          };
        }
      } else if (eventWithMessage.text) {
        // Try text field as fallback (might be present in some events)
        message = {
          role: 'user',
          content: eventWithMessage.text,
          timestamp: new Date().toISOString(),
          format: 'text'
        };
      } else {
        // Last resort fallback
        Logger.warn('[EventStreamProcessor] UserMessageEvent missing message content', event);
        message = {
          role: 'user',
          content: '[User message]',
          timestamp: new Date().toISOString(),
          format: 'text'
        };
      }
    }
    
    // Check for sub-session using SessionEvent fields (all UserMessageEvents extend SessionEvent)
    if (this.isSubSession(event)) {
      // Add sub-session metadata to the message
      (message as any).isSubSession = true;
      (message as any).metadata = {
        sessionId: event.session_id,
        parentSessionId: event.parent_session_id,
        userSessionId: event.user_session_id
      };
      
      Logger.debug(`[EventStreamProcessor] Sub-session detected in generic handler: ${event.session_id}`);
    }
    
    // Add to session
    const session = this.sessionManager.getCurrentSession();
    if (session) {
      session.messages.push(message);
      session.updated_at = new Date().toISOString();
      
      // Emit for UI consumption
      this.sessionManager.emit('message-added', {
        sessionId: session.session_id,
        message
      });
    }
    
    // Also emit the original user-message event for backward compatibility
    this.sessionManager.emit('user-message', {
      vendor: event.vendor,
      message: 'message' in event ? (event as any).message : undefined
    });
  }
  
  /**
   * Handle subsession started events
   */
  private handleSubsessionStarted(event: SubsessionStartedEvent): void {
    // Emit subsession start event for UI tracking
    this.sessionManager.emit('subsession-started', {
      subSessionType: event.sub_session_type,
      subAgentType: event.sub_agent_type,
      primeAgentKey: event.prime_agent_key,
      subAgentKey: event.sub_agent_key
    });
  }
  
  /**
   * Handle subsession ended events
   */
  private handleSubsessionEnded(_event: SubsessionEndedEvent): void {
    // Emit subsession end event for UI tracking
    this.sessionManager.emit('subsession-ended', {});
  }
  
  /**
   * Handle cancelled events
   */
  private handleCancelled(_event: CancelledEvent): void {
    Logger.info('[EventStreamProcessor] Agent response cancelled');
    
    // If there's a message being built, mark it as cancelled and finalize
    if (this.messageBuilder.hasCurrentMessage()) {
      const message = this.messageBuilder.finalize({
        stopReason: 'cancelled'
      });
      
      // Add to session with cancelled status
      const session = this.sessionManager.getCurrentSession();
      if (session) {
        session.messages.push(message);
        session.updated_at = new Date().toISOString();
      }
      
      // Emit the cancelled message
      this.sessionManager.emit('message-complete', {
        sessionId: session?.session_id || '',
        message
      });
    }
    
    // Clear any active tool calls
    const activeNotifications = this.toolCallManager.getActiveNotifications();
    activeNotifications.forEach(notification => {
      this.sessionManager.emit('tool-notification-removed', notification.id);
    });
    
    // Reset state for clean slate
    this.messageBuilder.reset();
    this.toolCallManager.reset();
    
    // Emit cancellation event for UI to handle
    this.sessionManager.emit('response-cancelled', {});
  }
  
  /**
   * Map resumed messages to events for proper rendering
   * Task 1.1: Core converter method that processes resumed messages as if they were streamed
   */
  private mapResumedMessagesToEvents(messages: MessageParam[], sessionId: string): void {
    // Clear existing messages first
    this.sessionManager.emit('session-messages-loaded', {
      sessionId,
      messages: []
    });
    
    // Process each message
    for (let i = 0; i < messages.length; i++) {
      const message = messages[i];
      if (!message) continue; // Safety check
      
      // Handle based on role
      if (message.role === 'assistant') {
        // Process assistant message and check how many messages it consumed
        const messagesConsumed = this.processAssistantMessage(message, messages[i + 1], sessionId);
        // Skip the next message if it was consumed as a tool result
        if (messagesConsumed > 0) {
          i += messagesConsumed;
        }
      } else if (message.role === 'user') {
        // Process user message (tool results are handled with their tool use)
        this.processUserMessage(message, sessionId);
      } else if (message.role === 'system') {
        this.processSystemMessage(message, sessionId);
      }
    }
  }
  
  /**
   * Process an assistant message and emit appropriate events
   * Returns the number of additional messages consumed (for tool results)
   */
  private processAssistantMessage(
    message: MessageParam,
    nextMessage: MessageParam | undefined,
    sessionId: string
  ): number {
    let messagesConsumed = 0;
    
    // Check for tool use blocks
    if (message.content && Array.isArray(message.content)) {
      let hasTextContent = false;
      const textParts: string[] = [];
      
      for (const block of message.content) {
        if (isTextBlockParam(block)) {
          hasTextContent = true;
          textParts.push(block.text);
        } else if (isToolUseBlockParam(block)) {
          // Task 1.2: THINK TOOL - Special handling
          if (block.name === 'think') {
            // Emit message-added event for think tool as a thought
            const thoughtContent = (block.input as any).thought || '';
            this.sessionManager.emit('message-added', {
              sessionId,
              message: {
                role: 'assistant (thought)' as 'assistant',
                content: thoughtContent,
                timestamp: new Date().toISOString(),
                format: 'markdown'
              }
            });
            // Mark that we'll consume the next message (tool result)
            messagesConsumed = 1;
            continue;
          }
          
          // Task 1.3: DELEGATION TOOLS - Special handling
          if (this.isDelegationTool(block.name)) {
            const consumed = this.processDelegationTool(block, nextMessage, sessionId);
            messagesConsumed = Math.max(messagesConsumed, consumed);
            continue;
          }
          
          // Task 1.5: Regular tool calls
          const consumed = this.processRegularToolCall(block, nextMessage, sessionId);
          messagesConsumed = Math.max(messagesConsumed, consumed);
        }
      }
      
      // Emit any text content as a regular message
      if (hasTextContent) {
        const combinedText = textParts.join('');
        this.sessionManager.emit('message-added', {
          sessionId,
          message: {
            role: 'assistant',
            content: combinedText,
            timestamp: new Date().toISOString(),
            format: 'text'
          }
        });
      }
    } else {
      // Task 1.6: Handle regular text messages
      this.emitTextMessage(message, sessionId);
    }
    
    return messagesConsumed;
  }
  
  /**
   * Check if a tool name is a delegation tool
   */
  private isDelegationTool(name: string): boolean {
    return name.startsWith('act_') || 
           name.startsWith('ateam_') || 
           name.startsWith('aa_');
  }
  
  /**
   * Process a delegation tool and emit subsession events
   * Returns 1 if it consumed the next message (tool result)
   */
  private processDelegationTool(
    toolBlock: any,
    toolResult: MessageParam | undefined,
    sessionId: string
  ): number {
    // 1. Start subsession
    const subSessionType = toolBlock.name.includes('oneshot') ? 'oneshot' : 'chat';
    const subAgentKey = (toolBlock.input as any).agent_key || 'clone';
    
    this.sessionManager.emit('subsession-started', {
      subSessionType,
      subAgentType: this.getAgentType(toolBlock.name),
      primeAgentKey: 'current_agent',
      subAgentKey
    });
    
    // 2. User message from tool input
    const request = (toolBlock.input as any).request || '';
    const processContext = (toolBlock.input as any).process_context || '';
    const userContent = processContext ? 
      request + '\n# Process Context\n\n' + processContext : 
      request;
    
    this.sessionManager.emit('message-added', {
      sessionId,
      message: {
        role: 'user',
        content: userContent,
        timestamp: new Date().toISOString(),
        format: 'text'
      }
    });
    
    // 3. Assistant message from tool result
    if (toolResult && toolResult.role === 'user' && toolResult.content) {
      // Tool result is in the next user message
      let resultContent = '';
      
      if (Array.isArray(toolResult.content)) {
        // Find the tool_result block
        for (const block of toolResult.content) {
          if ('type' in block && block.type === 'tool_result') {
            const yamlContent = (block as any).content || '';
            resultContent = this.parseAssistantFromYaml(yamlContent);
            break;
          }
        }
      }
      
      if (resultContent) {
        this.sessionManager.emit('message-added', {
          sessionId,
          message: {
            role: 'assistant',
            content: resultContent,
            timestamp: new Date().toISOString(),
            format: 'text'
          }
        });
      }
    }
    
    // 4. End subsession
    this.sessionManager.emit('subsession-ended', {});
    
    return 1; // Consumed the tool result message
  }
  
  /**
   * Get agent type from tool name
   */
  private getAgentType(toolName: string): 'clone' | 'team' | 'assist' | 'tool' {
    if (toolName.startsWith('act_')) return 'clone';
    if (toolName.startsWith('ateam_')) return 'team';
    if (toolName.startsWith('aa_')) return 'assist';
    return 'tool'; // Default to 'tool' instead of 'unknown'
  }
  
  /**
   * Parse assistant content from YAML tool result
   * Task 1.4: YAML parser for delegation results
   */
  private parseAssistantFromYaml(yamlContent: string): string {
    // Remove preamble if present
    let content = yamlContent;
    const preamble = '**IMPORTANT**: The following response is also displayed in the UI for the user, you do not need to relay it.';
    if (content.includes(preamble)) {
      content = content.replace(preamble, '').trim();
      // Remove the --- separator if present
      if (content.startsWith('---')) {
        content = content.substring(3).trim();
      }
    }
    
    // Try to parse YAML
    try {
      // Simple YAML parsing for the text field
      // Look for text: 'content' or text: "content" or text: content
      const textMatch = content.match(/^text:\s*['"]?([\s\S]*?)['"]?$/m);
      if (textMatch && textMatch[1]) {
        // Handle multi-line YAML strings
        let text = textMatch[1];
        
        // If it starts with | or >, it's a multi-line string
        if (text.startsWith('|') || text.startsWith('>')) {
          // Get everything after the indicator
          const lines = content.split('\n');
          const textLineIndex = lines.findIndex(line => line.trim().startsWith('text:'));
          if (textLineIndex >= 0) {
            // Get all lines after text: that are indented
            const textLines = [];
            for (let i = textLineIndex + 1; i < lines.length; i++) {
              const line = lines[i];
              if (!line) continue; // Skip undefined lines
              // Stop if we hit another field (not indented)
              if (line.length > 0 && !line.startsWith(' ') && !line.startsWith('\t')) {
                break;
              }
              textLines.push(line);
            }
            text = textLines.join('\n').trim();
          }
        } else if (text.startsWith("'") && text.endsWith("'")) {
          // Single-quoted string
          text = text.slice(1, -1).replace(/''/g, "'");
        } else if (text.startsWith('"') && text.endsWith('"')) {
          // Double-quoted string
          text = text.slice(1, -1);
        }
        
        return text;
      }
      
      // If no text field found, return the cleaned content
      return content;
    } catch (e) {
      Logger.error('[EventStreamProcessor] Failed to parse delegation result YAML:', e);
      return content; // Fallback to raw content
    }
  }
  
  /**
   * Process a regular tool call
   * Returns 1 if it consumed the next message (tool result)
   */
  private processRegularToolCall(
    toolBlock: any,
    nextMessage: MessageParam | undefined,
    _sessionId: string
  ): number {
    // Find the corresponding tool result
    let toolResult = undefined;
    
    if (nextMessage && nextMessage.role === 'user' && Array.isArray(nextMessage.content)) {
      for (const block of nextMessage.content) {
        if ('type' in block && block.type === 'tool_result' && 
            (block as any).tool_use_id === toolBlock.id) {
          toolResult = (block as any).content;
          break;
        }
      }
    }
    
    // Emit tool call event with active=false (completed)
    this.sessionManager.emit('tool-call-complete', {
      toolCalls: [{
        id: toolBlock.id,
        name: toolBlock.name,
        input: toolBlock.input
      }],
      toolResults: toolResult ? [{
        tool_use_id: toolBlock.id,
        content: toolResult,
        is_error: false
      }] : undefined
    });
    
    return toolResult ? 1 : 0; // Consume next message if we found a result
  }
  
  /**
   * Process a user message
   */
  private processUserMessage(message: MessageParam | undefined, sessionId: string): void {
    if (!message) return;
    
    // Skip if this is a tool result (those are handled with their tool use)
    if (Array.isArray(message.content)) {
      const hasToolResult = message.content.some(block => 
        'type' in block && block.type === 'tool_result'
      );
      if (hasToolResult) {
        return; // Skip tool results
      }
    }
    
    // Task 1.6: Emit regular user message
    const normalizedContent = this.normalizeMessageContent(message.content);
    this.sessionManager.emit('message-added', {
      sessionId,
      message: {
        role: 'user',
        content: normalizedContent,
        timestamp: new Date().toISOString(),
        format: 'text'
      }
    });
  }
  
  /**
   * Process a system message  
   */
  private processSystemMessage(message: MessageParam | undefined, sessionId: string): void {
    if (!message) return;
    
    const normalizedContent = this.normalizeMessageContent(message.content);
    this.sessionManager.emit('message-added', {
      sessionId,
      message: {
        role: 'system',
        content: normalizedContent,
        timestamp: new Date().toISOString(),
        format: 'text'
      }
    });
  }
  
  /**
   * Emit a text message event
   */
  private emitTextMessage(message: MessageParam | undefined, sessionId: string): void {
    if (!message) return;
    
    const normalizedContent = this.normalizeMessageContent(message.content);
    this.sessionManager.emit('message-added', {
      sessionId,
      message: {
        role: message.role as Message['role'],
        content: normalizedContent,
        timestamp: new Date().toISOString(),
        format: 'text'
      }
    });
  }
  
  /**
   * Reset the processor state
   */
  reset(): void {
    this.messageBuilder.reset();
    this.toolCallManager.reset();
    Logger.info('[EventStreamProcessor] EventStreamProcessor reset');
  }
  
  /**
   * Clean up resources
   */
  destroy(): void {
    this.reset();
    Logger.info('[EventStreamProcessor] EventStreamProcessor destroyed');
  }
}