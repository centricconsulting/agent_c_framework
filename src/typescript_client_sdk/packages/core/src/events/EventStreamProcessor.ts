/**
 * EventStreamProcessor - Central coordinator for processing incoming events and building messages
 * Routes events to appropriate handlers and coordinates message building
 */

import { ChatSessionManager } from '../session/SessionManager';
import { MessageBuilder } from './MessageBuilder';
import { ToolCallManager } from './ToolCallManager';
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
  CancelledEvent,
  UserTurnStartEvent
} from './types/ServerEvents';
import { Message, MessageContent, ContentPart } from './types/CommonTypes';
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
  private sessionManager: ChatSessionManager;
  private currentChatSessionId: string | null = null;
  private recentEventIds: Set<string> = new Set();
  private readonly EVENT_ID_TTL = 5000; // 5 seconds - events older than this are forgotten
  private eventCounter = 0; // Sequential counter for events without IDs
  
  constructor(sessionManager: ChatSessionManager) {
    this.sessionManager = sessionManager;
    this.messageBuilder = new MessageBuilder();
    this.toolCallManager = new ToolCallManager();
  }
  
  /**
   * Set the current chat session ID for sub-session detection
   */
  setCurrentChatSessionId(id: string): void {
    this.currentChatSessionId = id;
    Logger.debug(`[EventStreamProcessor] Current chat session ID set: ${id}`);
  }
  
  /**
   * Check if an event represents a sub-session
   * 
   * Sub-sessions occur when an agent uses delegation tools to communicate with
   * other agents/clones. These events have a different session_id than the
   * current user's chat session.
   * 
   * @param event - Event to check
   * @returns true if event is from a sub-session, false otherwise
   */
  private isSubSession(event: any): boolean {
    // Sub-session = event's session_id doesn't match current user chat session
    if (this.currentChatSessionId && event.session_id) {
      return event.session_id !== this.currentChatSessionId;
    }
    return false;
  }
  
  /**
   * Generate a unique fingerprint for an event to detect duplicates.
   * Uses event ID if available, otherwise creates fingerprint from event properties.
   * 
   * For streaming events without IDs (text_delta, thought_delta, etc.), uses a
   * sequential counter to ensure uniqueness even when events arrive in the same
   * millisecond (which is common for rapid streaming).
   */
  private generateEventFingerprint(event: ServerEvent): string {
    // If event has an ID field, use it
    if ('id' in event && event.id) {
      return String(event.id);
    }
    
    // For events without IDs, use sequential counter to ensure uniqueness
    // This prevents false duplicates when events arrive rapidly (same millisecond)
    const sessionId = 'session_id' in event ? event.session_id : 'no-session';
    return `${event.type}_${sessionId || 'no-session'}_${++this.eventCounter}`;
  }

  /**
   * Check if an event was recently processed.
   */
  private hasProcessedRecently(eventId: string): boolean {
    return this.recentEventIds.has(eventId);
  }

  /**
   * Mark an event as processed and schedule cleanup after TTL.
   */
  private markAsProcessed(eventId: string): void {
    this.recentEventIds.add(eventId);
    
    // Clean up after TTL to prevent memory growth
    setTimeout(() => {
      this.recentEventIds.delete(eventId);
    }, this.EVENT_ID_TTL);
  }
  
  /**
   * Normalize MessageParam content to MessageContent format for UI compatibility
   * Handles conversion from both Anthropic ContentBlockParam[] and OpenAI ChatCompletionContentPart[]
   */
  private normalizeMessageContent(content: string | ContentBlockParam[] | any[]): MessageContent {
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
          // Anthropic image format
          normalizedParts.push({
            type: 'image',
            source: block.source
          });
        } else if ('type' in block && block.type === 'image_url') {
          // OpenAI image format - convert to our normalized format
          const openAiImage = block as any;
          normalizedParts.push({
            type: 'image',
            source: {
              type: 'url',
              url: openAiImage.image_url?.url || '',
              // Preserve detail if present
              ...(openAiImage.image_url?.detail && { detail: openAiImage.image_url.detail })
            }
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
    console.log('[EventStreamProcessor] processEvent CALLED:', event.type);
    // Check for duplicate events
    const eventId = this.generateEventFingerprint(event);
    
    if (this.hasProcessedRecently(eventId)) {
      Logger.debug('[EventStreamProcessor] Skipping duplicate event', {
        type: event.type,
        eventId,
        sessionId: 'session_id' in event ? (event.session_id || 'none') : 'none'
      });
      return; // Skip processing
    }
    
    // Mark as processed before handling
    this.markAsProcessed(eventId);
    
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
      case 'openai_user_message':
        this.handleOpenAIUserMessage(event as OpenAIUserMessageEvent);
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
      case 'user_turn_start':
        this.handleUserTurnStart(event as UserTurnStartEvent);
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
      
      // Clear tool notifications for this session when interaction ends
      // This prevents orphaned "Agent is using X" notifications
      const sessionId = event.session_id;
      this.toolCallManager.clearSessionNotifications(sessionId);
      
      // Emit event to notify UI that session notifications were cleared
      this.sessionManager.emit('session-notifications-cleared', { sessionId });
      
      Logger.debug(`[EventStreamProcessor] Cleared tool notifications for session: ${sessionId}`);
    }
  }
  
  /**
   * Handle user turn start events (safety net for cleanup)
   */
  private handleUserTurnStart(_event: UserTurnStartEvent): void {
    Logger.info('[EventStreamProcessor] User turn started - clearing all active tool notifications');
    
    // Nuclear cleanup: clear ALL active notifications across all sessions
    // This is a safety net to prevent orphaned notifications from any edge cases
    this.toolCallManager.clearAllActiveNotifications();
    
    // Emit event to notify UI that all notifications were cleared
    this.sessionManager.emit('all-notifications-cleared', undefined);
    
    Logger.debug('[EventStreamProcessor] All tool notifications cleared on user turn start');
  }
  
  /**
   * Handle streaming text delta events
   */
  private handleTextDelta(event: TextDeltaEvent): void {
    console.log('[EventStreamProcessor] handleTextDelta CALLED', { content: event.content });
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
      // We check active notifications for this session only
      const sessionNotifications = this.toolCallManager.getActiveNotificationsForSession(event.session_id);
      sessionNotifications.forEach(notification => {
        if (notification.toolName === 'think') {
          this.sessionManager.emit('tool-notification-removed', {
            sessionId: notification.sessionId,
            toolCallId: notification.id
          });
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
      // Phase 3: Check for buffered tool calls (1% case - tools completed before any message)
      const sessionId = event.session_id;
      let bufferedToolCalls: any[] = [];
      let bufferedToolResults: any[] = [];
      
      if (this.sessionManager.hasPendingToolCalls(sessionId)) {
        const pending = this.sessionManager.getPendingToolCalls(sessionId);
        
        bufferedToolCalls = pending.map(tc => ({
          id: tc.id,
          type: tc.type,
          name: tc.name,
          input: tc.input
        }));
        
        bufferedToolResults = pending
          .filter(tc => tc.result)
          .map(tc => tc.result!);
        
        Logger.info('[EventStreamProcessor] Attaching buffered tools to new message', {
          sessionId,
          toolCount: pending.length,
          toolNames: pending.map(tc => tc.name)
        });
        
        // Clear buffered tools after attaching
        this.sessionManager.clearPendingToolCalls(sessionId);
      }
      
      // Finalize the message with metadata (including buffered tools if any)
      const message = this.messageBuilder.finalize({
        inputTokens: event.input_tokens,
        outputTokens: event.output_tokens,
        stopReason: event.stop_reason,
        toolCalls: bufferedToolCalls.length > 0 ? bufferedToolCalls : undefined,
        toolResults: bufferedToolResults.length > 0 ? bufferedToolResults : undefined
      });
      
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
    const notifications = this.toolCallManager.onToolSelect(event);
    
    // Emit notification for each tool
    notifications.forEach(notification => {
      // Special handling for the "think" tool
      if (notification.toolName === 'think') {
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
    });
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
      // Still remove the notification (with session context)
      this.sessionManager.emit('tool-notification-removed', {
        sessionId: event.session_id,
        toolCallId: toolCall.id
      });
      return;
    }
    
    if (event.active) {
      // Tool is executing
      const notifications = this.toolCallManager.onToolCallActive(event);
      notifications.forEach(notification => {
        this.sessionManager.emit('tool-notification', notification);
      });
    } else {
      // Tool completed - Phase 3: Backward attachment
      Logger.info('[EventStreamProcessor] handleToolCall: Tool completed (active=false)', {
        sessionId: event.session_id,
        toolCount: event.tool_calls.length,
        toolNames: event.tool_calls.map(tc => tc.name)
      });
      
      const completedToolCalls = this.toolCallManager.onToolCallComplete(event);
      
      Logger.info('[EventStreamProcessor] ToolCallManager returned completed tools', {
        completedCount: completedToolCalls.length,
        completedTools: completedToolCalls.map(tc => tc.name)
      });
      
      // Emit tool-call-complete event for UI to track results
      this.sessionManager.emit('tool-call-complete', {
        toolCalls: event.tool_calls,
        toolResults: event.tool_results
      });
      
      // Remove notifications for completed tools (with session context)
      event.tool_calls.forEach(tc => {
        this.sessionManager.emit('tool-notification-removed', {
          sessionId: event.session_id,
          toolCallId: tc.id
        });
      });
      
      // Phase 3: Attach completed tools to previous message (backward attachment)
      if (completedToolCalls.length > 0) {
        Logger.info('[EventStreamProcessor] Calling attachToolCallsToPreviousMessage', {
          sessionId: event.session_id,
          toolCount: completedToolCalls.length
        });
        this.attachToolCallsToPreviousMessage(event.session_id, completedToolCalls);
      } else {
        Logger.warn('[EventStreamProcessor] No completed tool calls to attach!', {
          sessionId: event.session_id,
          originalToolCount: event.tool_calls.length
        });
      }
    }
  }
  
  /**
   * Phase 3: Attach completed tool calls to the previous message (backward attachment)
   * This is the correct behavior - tools execute AFTER the message, so results attach to previous message
   * 
   * @param sessionId - Session ID
   * @param completedToolCalls - Tool calls with results to attach
   */
  private attachToolCallsToPreviousMessage(
    sessionId: string,
    completedToolCalls: Array<{ id: string; type: 'tool_use'; name: string; input: Record<string, unknown>; result?: any }>
  ): void {
    Logger.info('[EventStreamProcessor] attachToolCallsToPreviousMessage CALLED', {
      sessionId,
      toolCount: completedToolCalls.length,
      toolNames: completedToolCalls.map(tc => tc.name)
    });
    
    const session = this.sessionManager.getSession(sessionId);
    if (!session || !session.messages || !session.messages.length) {
      // No session or no messages yet - buffer for next message (1% case)
      Logger.debug('[EventStreamProcessor] No previous message to attach tools to - buffering', {
        sessionId,
        toolCount: completedToolCalls.length,
        toolNames: completedToolCalls.map(tc => tc.name)
      });
      this.sessionManager.bufferPendingToolCalls(sessionId, completedToolCalls);
      return;
    }
    
    // Find the last assistant or thought message (99% case)
    // Tools CAN attach to thoughts - they're just internal reasoning that used tools
    // BUT: Stop if we hit a user message - tool calls can't attach to messages before the user spoke!
    const messages = session.messages;
    let lastAssistantMessageIndex = -1;
    
    for (let i = messages.length - 1; i >= 0; i--) {
      const message = messages[i] as any;
      
      // Stop if we hit a user message - tool calls belong to THIS turn, not a previous turn
      if (message && message.role === 'user') {
        Logger.debug('[EventStreamProcessor] Hit user message while looking backward - stopping search', {
          sessionId,
          messageIndex: i
        });
        break;
      }
      
      // Check for assistant messages OR thought messages (role includes '(thought)')
      const isAssistant = message && message.role === 'assistant';
      const isThought = message && (message.role === 'assistant (thought)' || message.type === 'thought');
      
      if (isAssistant || isThought) {
        lastAssistantMessageIndex = i;
        break;
      }
    }
    
    if (lastAssistantMessageIndex === -1) {
      // No assistant message found - buffer for next message
      Logger.debug('[EventStreamProcessor] No assistant message to attach tools to - buffering', {
        sessionId,
        toolCount: completedToolCalls.length,
        messageCount: messages.length
      });
      this.sessionManager.bufferPendingToolCalls(sessionId, completedToolCalls);
      return;
    }
    
    // Attach tools to the previous assistant message
    const targetMessage = messages[lastAssistantMessageIndex] as any; // EnhancedMessage
    
    // Extract tool calls and results from this batch only
    const newToolCalls = completedToolCalls.map(tc => ({
      id: tc.id,
      type: tc.type,
      name: tc.name,
      input: tc.input
    }));
    
    const newToolResults = completedToolCalls
      .filter(tc => tc.result)
      .map(tc => tc.result!);
    
    // Initialize metadata if needed
    if (!targetMessage.metadata) {
      targetMessage.metadata = {};
    }
    
    // Create NEW arrays with existing + new tools (don't mutate existing arrays)
    // This ensures React detects the reference change and re-renders components
    const existingToolCalls = targetMessage.metadata.toolCalls || [];
    const existingToolResults = targetMessage.metadata.toolResults || [];
    
    targetMessage.metadata.toolCalls = [...existingToolCalls, ...newToolCalls];
    targetMessage.metadata.toolResults = [...existingToolResults, ...newToolResults];
    
    // Sync top-level fields (create new references)
    targetMessage.toolCalls = targetMessage.metadata.toolCalls;
    targetMessage.toolResults = targetMessage.metadata.toolResults;
    
    Logger.info('[EventStreamProcessor] Attached tools to previous message (backward attachment)', {
      sessionId,
      messageIndex: lastAssistantMessageIndex,
      messageId: targetMessage.id,
      toolCount: completedToolCalls.length,
      toolNames: completedToolCalls.map(tc => tc.name)
    });
    
    // Only emit message-updated if the message has an ID
    // If no ID yet (streaming message), the tool calls are already attached and will be included in completion
    if (targetMessage.id) {
      this.sessionManager.emit('message-updated', {
        sessionId,
        messageId: targetMessage.id,
        message: targetMessage
      });
    } else {
      Logger.debug('[EventStreamProcessor] Message has no ID yet (streaming) - tool calls attached, will be included in completion');
    }
  }
  
  /**
   * Handle rich media rendering events
   */
  private handleRenderMedia(event: RenderMediaEvent): void {
    // Generate ID for React key prop
    const id = `media_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    
    // Emit media event preserving server structure
    this.sessionManager.emit('media-added', {
      sessionId: event.session_id,
      media: {
        id,
        role: event.role || 'assistant',
        type: 'media',
        content: event.content,
        contentType: event.content_type, // PRESERVE server field, don't transform
        timestamp: new Date().toISOString(),
        status: 'complete',
        // Preserve ALL server fields in metadata
        metadata: {
          sent_by_class: event.sent_by_class,
          sent_by_function: event.sent_by_function,
          foreign_content: event.foreign_content,
          url: event.url,
          name: event.name
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
    
    // Track the current user's chat session ID for sub-session detection
    this.setCurrentChatSessionId(event.chat_session.session_id);
    
    // Convert the server session to runtime ChatSession with normalized messages
    const session = this.convertServerSession(event.chat_session);
    
    Logger.info(`[EventStreamProcessor] Processing session change: ${session.session_id} with ${session.messages?.length || 0} messages and agent_config: ${session.agent_config?.key || 'none'}`);
    
    // Update the session in ChatSessionManager with the converted version
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
      let fallbackContent = '[User message]';
      if (messageParam !== null && messageParam !== undefined) {
        try {
          fallbackContent = JSON.stringify(messageParam);
        } catch (e) {
          // Keep default fallback if stringify fails
        }
      }
      message = {
        role: 'user',
        content: fallbackContent,
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
   * Handle OpenAI-specific user message events with sub-session detection
   */
  private handleOpenAIUserMessage(event: OpenAIUserMessageEvent): void {
    // Convert the OpenAI message to our normalized format
    const messageParam = event.message as any;
    let message: Message;
    
    // Check if it has the expected MessageParam structure
    if (messageParam && messageParam.content !== undefined && messageParam.role !== undefined) {
      message = this.convertMessageParam(messageParam);
    } else {
      // Fallback for malformed messages
      let fallbackContent = '[User message]';
      if (messageParam !== null && messageParam !== undefined) {
        try {
          fallbackContent = JSON.stringify(messageParam);
        } catch (e) {
          // Keep default fallback if stringify fails
        }
      }
      message = {
        role: 'user',
        content: fallbackContent,
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
      this.sessionManager.emit('tool-notification-removed', {
        sessionId: notification.sessionId,
        toolCallId: notification.id
      });
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
    Logger.debug(`[EventStreamProcessor] Processing ${messages.length} resumed messages`);
    
    // Collect all processed messages instead of emitting individual events
    const processedMessages: Message[] = [];
    
    // Process each message
    for (let i = 0; i < messages.length; i++) {
      const message = messages[i];
      if (!message) continue; // Safety check
      
      // Handle based on role
      if (message.role === 'assistant') {
        // Process assistant message and collect the results
        // Pass processedMessages so function can look backward for tool attachment
        const result = this.processAssistantMessageForResume(message, messages[i + 1], sessionId, processedMessages);
        processedMessages.push(...result.messages);
        // Skip the next message if it was consumed as a tool result
        if (result.messagesConsumed > 0) {
          i += result.messagesConsumed;
        }
      } else if (message.role === 'user') {
        // Process user message (tool results are handled with their tool use)
        const userMessage = this.processUserMessageForResume(message, sessionId);
        if (userMessage) {
          processedMessages.push(userMessage);
        }
      } else if (message.role === 'system') {
        const systemMessage = this.processSystemMessageForResume(message, sessionId);
        if (systemMessage) {
          processedMessages.push(systemMessage);
        }
      }
    }
    
    // Emit a single session-messages-loaded event with all processed messages
    // This ensures the React hook receives all messages at once after clearing
    this.sessionManager.emit('session-messages-loaded', {
      sessionId,
      messages: processedMessages
    });
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
   * Reset the processor state
   */
  reset(): void {
    this.messageBuilder.reset();
    this.toolCallManager.reset();
    this.recentEventIds.clear(); // Clear deduplication cache
    Logger.info('[EventStreamProcessor] EventStreamProcessor reset');
  }
  
  /**
   * Clean up resources
   */
  destroy(): void {
    Logger.info('[EventStreamProcessor] EventStreamProcessor destroyed');
  }
  
  /**
   * Process an assistant message for resume and return messages instead of emitting events
   * 
   * NOTE: processedMessages parameter is the array of messages built up so far during resume.
   * This allows us to look backward and attach tool calls to previous messages.
   */
  private processAssistantMessageForResume(
    message: MessageParam,
    nextMessage: MessageParam | undefined,
    sessionId: string,
    processedMessages: Message[]
  ): { messages: Message[], messagesConsumed: number } {
    const messages: Message[] = [];
    let messagesConsumed = 0;
    
    // Check for tool use blocks
    if (message.content && Array.isArray(message.content)) {
      let hasTextContent = false;
      const textParts: string[] = [];
      const toolCalls: Array<{ id: string; type: 'tool_use'; name: string; input: Record<string, unknown> }> = [];
      const toolResults: Array<{ type: 'tool_result'; tool_use_id: string; content: string; is_error?: boolean }> = [];
      
      for (const block of message.content) {
        if (isTextBlockParam(block)) {
          hasTextContent = true;
          textParts.push(block.text);
        } else if (isToolUseBlockParam(block)) {
          // THINK TOOL - Special handling
          if (block.name === 'think') {
            const thoughtContent = (block.input as any).thought || '';
            messages.push({
              role: 'assistant (thought)' as any,
              content: thoughtContent,
              timestamp: new Date().toISOString(),
              format: 'markdown'
            } as Message);
            messagesConsumed = 1; // Skip the tool result
            continue;
          }
          
          // DELEGATION TOOLS - Special handling
          if (this.isDelegationTool(block.name)) {
            const subSessionType = block.name.includes('oneshot') ? 'oneshot' : 'chat';
            const subAgentKey = (block.input as any).agent_key || 'clone';
            
            this.sessionManager.emit('subsession-started', {
              subSessionType,
              subAgentType: block.name.startsWith('act_') ? 'clone' : 'team',
              primeAgentKey: 'current_agent',
              subAgentKey
            });
            
            const request = (block.input as any).request || (block.input as any).message || '';
            const processContext = (block.input as any).process_context || '';
            const userContent = processContext ? 
              request + '\n# Process Context\n\n' + processContext : 
              request;
            
            messages.push({
              role: 'user',
              content: userContent,
              timestamp: new Date().toISOString(),
              format: 'text'
            } as Message);
            
            if (nextMessage && nextMessage.role === 'user' && nextMessage.content) {
              let resultContent = '';
              
              if (Array.isArray(nextMessage.content)) {
                for (const resultBlock of nextMessage.content) {
                  if ('type' in resultBlock && resultBlock.type === 'tool_result') {
                    const content = (resultBlock as any).content || '';
                    resultContent = this.parseAssistantFromDelegationResult(content);
                    break;
                  }
                }
              }
              
              if (resultContent) {
                messages.push({
                  role: 'assistant',
                  content: resultContent,
                  timestamp: new Date().toISOString(),
                  format: 'text'
                } as Message);
              }
            }
            
            this.sessionManager.emit('subsession-ended', {});
            messagesConsumed = 1;
            continue;
          }
          
          // REGULAR TOOL CALLS - Extract for attachment
          Logger.debug(`[EventStreamProcessor] Extracting resumed tool call: ${block.name} (${block.id})`);
          
          toolCalls.push({
            id: block.id,
            type: block.type as 'tool_use',
            name: block.name,
            input: block.input as Record<string, unknown>
          });
          
          // Find matching result in next message
          if (nextMessage && nextMessage.role === 'user' && Array.isArray(nextMessage.content)) {
            for (const resultBlock of nextMessage.content) {
              if ('type' in resultBlock && resultBlock.type === 'tool_result') {
                const toolResultBlock = resultBlock as any;
                if (toolResultBlock.tool_use_id === block.id) {
                  Logger.debug(`[EventStreamProcessor] Found matching tool result for ${block.id}`);
                  toolResults.push({
                    type: 'tool_result',
                    tool_use_id: toolResultBlock.tool_use_id,
                    content: toolResultBlock.content || '',
                    is_error: toolResultBlock.is_error
                  });
                  break;
                }
              }
            }
          }
        }
      }
      
      // Consume the tool result message if we extracted tool calls
      if (toolCalls.length > 0) {
        messagesConsumed = 1;
      }
      
      // Handle tool calls - attach backward or buffer forward
      if (toolCalls.length > 0) {
        Logger.info(`[EventStreamProcessor] Processing ${toolCalls.length} tool calls from resumed message`);
        
        // Look BACKWARD in processedMessages for previous assistant/thought message
        // BUT: Stop if we hit a user message - tool calls belong to THIS turn, not a previous turn
        let attachedBackward = false;
        for (let i = processedMessages.length - 1; i >= 0; i--) {
          const prevMsg = processedMessages[i] as any;
          
          // Stop if we hit a user message - tool calls can't attach to messages before the user spoke
          if (prevMsg.role === 'user') {
            Logger.debug(`[EventStreamProcessor] Hit user message while looking backward during resume - stopping search`);
            break;
          }
          
          if (prevMsg.role === 'assistant' || prevMsg.role === 'assistant (thought)') {
            Logger.info(`[EventStreamProcessor] Attaching ${toolCalls.length} tool calls backward to previous message`);
            
            // Initialize metadata and arrays if needed
            prevMsg.metadata = prevMsg.metadata || {};
            prevMsg.toolCalls = prevMsg.toolCalls || [];
            prevMsg.toolResults = prevMsg.toolResults || [];
            prevMsg.metadata.toolCalls = prevMsg.metadata.toolCalls || [];
            prevMsg.metadata.toolResults = prevMsg.metadata.toolResults || [];
            
            // APPEND tool calls (don't replace!)
            prevMsg.toolCalls.push(...toolCalls);
            prevMsg.toolResults.push(...toolResults);
            prevMsg.metadata.toolCalls.push(...toolCalls);
            prevMsg.metadata.toolResults.push(...toolResults);
            
            attachedBackward = true;
            break;
          }
        }
        
        // If no previous message found, buffer for NEXT message
        if (!attachedBackward) {
          Logger.info(`[EventStreamProcessor] No previous message found - buffering ${toolCalls.length} tool calls for next message`);
          
          // Convert to ToolCallWithResult format
          const toolCallsWithResults = toolCalls.map(tc => {
            const result = toolResults.find(tr => tr.tool_use_id === tc.id);
            return {
              id: tc.id,
              type: tc.type,
              name: tc.name,
              input: tc.input,
              result: result
            };
          });
          
          this.sessionManager.bufferPendingToolCalls(sessionId, toolCallsWithResults);
        }
        
        // Don't create a message for tool-only assistant messages
        // Tool calls attach to other messages, never standalone
      }
      
      // Create message if there's text content
      if (hasTextContent) {
        const combinedText = textParts.join('');
        const msg: any = {
          role: 'assistant',
          content: combinedText,
          timestamp: new Date().toISOString(),
          format: 'text'
        };
        
        // Check for buffered tool calls to attach forward
        const buffered = this.sessionManager.getPendingToolCalls(sessionId);
        if (buffered.length > 0) {
          Logger.info(`[EventStreamProcessor] Attaching ${buffered.length} buffered tool calls forward to text message`);
          
          // Convert buffered ToolCallWithResult back to separate arrays
          const bufferedToolCalls = buffered.map(tc => ({
            id: tc.id,
            type: tc.type,
            name: tc.name,
            input: tc.input
          }));
          const bufferedToolResults = buffered
            .filter(tc => tc.result)
            .map(tc => tc.result!);
          
          // Combine current tool calls with buffered ones
          const allToolCalls = [...bufferedToolCalls, ...toolCalls];
          const allToolResults = [...bufferedToolResults, ...toolResults];
          
          if (allToolCalls.length > 0) {
            msg.metadata = {
              toolCalls: allToolCalls,
              toolResults: allToolResults
            };
            msg.toolCalls = allToolCalls;
            msg.toolResults = allToolResults;
          }
          
          // Clear the buffer
          this.sessionManager.clearPendingToolCalls(sessionId);
        } else if (toolCalls.length > 0) {
          // Just current tool calls, no buffered ones
          msg.metadata = {
            toolCalls: toolCalls,
            toolResults: toolResults
          };
          msg.toolCalls = toolCalls;
          msg.toolResults = toolResults;
        }
        
        messages.push(msg as Message);
      }
    } else {
      // Simple text message
      const normalizedContent = this.normalizeMessageContent(message.content);
      messages.push({
        role: 'assistant',
        content: normalizedContent,
        timestamp: new Date().toISOString(),
        format: 'text'
      } as Message);
    }
    
    return { messages, messagesConsumed };
  }
  
  /**
   * Parse assistant message from delegation tool result
   */
  private parseAssistantFromDelegationResult(resultContent: string): string {
    // First try JSON parsing (new format)
    try {
      const json = JSON.parse(resultContent);
      if (json.agent_message && json.agent_message.content) {
        // Extract text from the agent_message content array
        if (Array.isArray(json.agent_message.content)) {
          // Find text blocks in content array
          for (const block of json.agent_message.content) {
            if (block.type === 'text') {
              return block.text || '';
            }
          }
        } else if (typeof json.agent_message.content === 'string') {
          return json.agent_message.content;
        }
      }
    } catch {
      // Fall back to YAML parsing for backwards compatibility
      return this.parseAssistantFromYaml(resultContent);
    }
    return resultContent;
  }
  
  /**
   * Parse assistant message from YAML format (legacy)
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
      const textMatch = content.match(/^text:\s*['"]?([\s\S]*?)['"]?$/m);
      if (textMatch && textMatch[1]) {
        let text = textMatch[1];
        
        // Handle multi-line YAML strings
        if (text.startsWith('|') || text.startsWith('>')) {
          // Get everything after the indicator
          const lines = content.split('\n');
          const textLineIndex = lines.findIndex(line => line.trim().startsWith('text:'));
          if (textLineIndex >= 0) {
            // Get all lines after text: that are indented
            const textLines = [];
            for (let i = textLineIndex + 1; i < lines.length; i++) {
              const line = lines[i];
              if (!line) continue;
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
   * Process a user message for resume and return the message
   */
  private processUserMessageForResume(
    message: MessageParam,
    _sessionId: string
  ): Message | null {
    // Skip tool result messages - they're handled with tool use
    if (message.content && Array.isArray(message.content)) {
      const hasToolResult = message.content.some(block => 
        typeof block === 'object' && 'type' in block && block.type === 'tool_result'
      );
      if (hasToolResult) {
        return null; // Skip tool results
      }
    }
    
    const normalizedContent = this.normalizeMessageContent(message.content);
    return {
      role: 'user',
      content: normalizedContent,
      timestamp: new Date().toISOString(),
      format: 'text'
    } as Message;
  }
  
  /**
   * Process a system message for resume and return the message
   */
  private processSystemMessageForResume(
    message: MessageParam,
    _sessionId: string  
  ): Message | null {
    const normalizedContent = this.normalizeMessageContent(message.content);
    return {
      role: 'system',
      content: typeof normalizedContent === 'string' ? normalizedContent : JSON.stringify(normalizedContent),
      timestamp: new Date().toISOString(),
      format: 'text',
      severity: 'info'
    } as Message;
  }
}