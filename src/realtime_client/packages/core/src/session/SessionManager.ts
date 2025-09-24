/**
 * Session Manager for Agent C Realtime SDK
 * Manages multiple chat sessions, tracks current session, and handles message history
 */

import { EventEmitter } from '../events/EventEmitter';
import { ChatSession, Message, ChatSessionIndexEntry, ChatSessionQueryResponse } from '../events/types/CommonTypes';
import { Logger } from '../utils/logger';
import type { EnhancedMessage } from '../events/MessageBuilder';
import type { ToolNotification } from '../events/ToolCallManager';

/**
 * Event map for SessionManager events
 */
export interface SessionManagerEventMap {
  'session-changed': { 
    previousSession: ChatSession | null; 
    currentSession: ChatSession | null;
  };
  'message-added': { 
    sessionId: string; 
    message: Message; 
  };
  'sessions-updated': { 
    sessions: Map<string, ChatSession>; 
  };
  'session-cleared': { 
    sessionId: string; 
  };
  'all-sessions-cleared': void;
  'sessions-index-updated': {
    sessionIndex: ChatSessionIndexEntry[];
    totalSessions: number;
  };
  'request-user-sessions': {
    offset: number;
    limit: number;
  };
  // New events for message streaming and rich content
  'message-streaming': {
    sessionId: string;
    message: Partial<EnhancedMessage>;
  };
  'message-complete': {
    sessionId: string;
    message: EnhancedMessage;
  };
  'session-messages-loaded': {
    sessionId: string;
    messages: Message[];
  };
  'tool-notification': ToolNotification;
  'tool-notification-removed': string; // Tool ID
  'tool-call-complete': {
    toolCalls: Array<{ id: string; name: string; input: any }>;
    toolResults?: Array<{ tool_use_id: string; content: string; is_error?: boolean }>;
  };
  'media-added': {
    sessionId: string;
    media: EnhancedMessage;
  };
  // DEPRECATED: Use 'system_message' instead for API naming consistency
  'system-notification': {
    type: 'system' | 'error';
    severity: 'info' | 'warning' | 'error';
    content: string;
    source?: string;
    timestamp: string;
  };
  // SystemMessageEvent handler - maintains API naming consistency
  'system_message': {
    type: string; // The event type from server
    session_id: string;
    role: string;
    content: string;
    format: string;
    severity: 'info' | 'warning' | 'error';
    parent_session_id?: string;
    user_session_id?: string;
  };
  // ErrorEvent handler - for toast notifications, NOT session events
  'error': {
    type: string; // The event type from server
    message: string;
    source?: string;
    timestamp: string;
  };
  'user-message': {
    vendor: string;
    message?: Record<string, any>;
  };
  'subsession-started': {
    subSessionType: 'chat' | 'oneshot';
    subAgentType: 'clone' | 'team' | 'assist' | 'tool';
    primeAgentKey: string;
    subAgentKey: string;
  };
  'subsession-ended': Record<string, never>;
  'response-cancelled': Record<string, never>;
}

/**
 * Configuration options for SessionManager
 */
export interface SessionManagerConfig {
  maxSessions?: number;
  persistSessions?: boolean;
  defaultSessionName?: string;
}

/**
 * Manages chat sessions including history, current session tracking, and message accumulation
 */
export class SessionManager extends EventEmitter<SessionManagerEventMap> {
  private sessions: Map<string, ChatSession>;
  private currentSessionId: string | null;
  private textAccumulator: string;
  private isAccumulating: boolean;
  private config: SessionManagerConfig;
  private sessionIndex: ChatSessionIndexEntry[];
  private totalSessionCount: number;
  private lastFetchOffset: number;

  /**
   * Create a new SessionManager instance
   * @param config - Optional configuration for the session manager
   */
  constructor(config: SessionManagerConfig = {}) {
    super();
    
    this.sessions = new Map();
    this.currentSessionId = null;
    this.textAccumulator = '';
    this.isAccumulating = false;
    this.sessionIndex = [];
    this.totalSessionCount = 0;
    this.lastFetchOffset = 0;
    
    this.config = {
      maxSessions: config.maxSessions || 100,
      persistSessions: config.persistSessions || false,
      defaultSessionName: config.defaultSessionName || 'Chat Session'
    };
    
    Logger.info('[SessionManager] SessionManager initialized', this.config);
  }

  /**
   * Set the current active session from server
   * @param session - ChatSession object from server
   * @emits session-changed event
   */
  setCurrentSession(session: ChatSession): void {
    if (!session || !session.session_id) {
      Logger.error('[SessionManager] Invalid session provided to setCurrentSession');
      throw new Error('Invalid session: session must have a session_id');
    }

    const previousSession = this.getCurrentSession();
    const previousId = this.currentSessionId;

    // Update or add the session
    this.sessions.set(session.session_id, session);
    this.currentSessionId = session.session_id;

    // Check if we're at max capacity
    if (this.sessions.size > this.config.maxSessions!) {
      this.pruneOldestSession();
    }

    Logger.info(`[SessionManager] Current session set to: ${session.session_id}`, {
      sessionName: session.session_name,
      messageCount: session.messages.length,
      tokenCount: session.token_count
    });

    // Emit events if session actually changed
    if (previousId !== session.session_id) {
      this.emit('session-changed', {
        previousSession,
        currentSession: session
      });
    }

    this.emit('sessions-updated', {
      sessions: this.sessions
    });
  }

  /**
   * Get the current active session
   * @returns Current ChatSession or null if no session is active
   */
  getCurrentSession(): ChatSession | null {
    if (!this.currentSessionId) {
      return null;
    }
    return this.sessions.get(this.currentSessionId) || null;
  }

  /**
   * Get the current session ID
   * @returns Current session ID or null
   */
  getCurrentSessionId(): string | null {
    return this.currentSessionId;
  }

  /**
   * Get a specific session by ID
   * @param sessionId - ID of the session to retrieve
   * @returns ChatSession or null if not found
   */
  getSession(sessionId: string): ChatSession | null {
    return this.sessions.get(sessionId) || null;
  }

  /**
   * Get all sessions
   * @returns Map of all sessions
   */
  getAllSessions(): Map<string, ChatSession> {
    return new Map(this.sessions);
  }

  /**
   * Get all session IDs
   * @returns Array of session IDs
   */
  getSessionIds(): string[] {
    return Array.from(this.sessions.keys());
  }

  /**
   * Add a user message to the current session
   * @param content - Message content from the user
   * @returns The created message or null if no current session
   * @emits message-added event
   */
  addUserMessage(content: string): Message | null {
    const session = this.getCurrentSession();
    if (!session) {
      Logger.warn('[SessionManager] Cannot add user message: no current session');
      return null;
    }

    const message: Message = {
      role: 'user',
      content,
      timestamp: new Date().toISOString(),
      format: 'text'
    };

    session.messages.push(message);
    
    // Update token count (rough estimate: ~4 chars per token)
    session.token_count += Math.ceil(content.length / 4);
    session.updated_at = new Date().toISOString();

    Logger.debug(`[SessionManager] User message added to session ${session.session_id}`, {
      contentLength: content.length,
      totalMessages: session.messages.length
    });

    this.emit('message-added', {
      sessionId: session.session_id,
      message
    });

    return message;
  }

  /**
   * Handle text delta from assistant (accumulate text)
   * @deprecated Use EventStreamProcessor for delta handling. This method is no longer called by RealtimeClient.
   * @param delta - Text chunk to accumulate
   */
  handleTextDelta(delta: string): void {
    if (!delta) {
      return;
    }

    if (!this.getCurrentSession()) {
      Logger.warn('[SessionManager] Received text delta but no current session');
      return;
    }

    this.isAccumulating = true;
    this.textAccumulator += delta;
    
    Logger.debug(`[SessionManager] Text delta accumulated`, {
      deltaLength: delta.length,
      totalAccumulated: this.textAccumulator.length
    });
  }

  /**
   * Finalize assistant message from accumulated text
   * @deprecated Use EventStreamProcessor for message finalization. This method is no longer called by RealtimeClient.
   * @returns The created message or null if no accumulation or session
   * @emits message-added event
   */
  handleTextDone(): Message | null {
    if (!this.isAccumulating || !this.textAccumulator) {
      Logger.debug('[SessionManager] handleTextDone called but nothing to finalize');
      return null;
    }

    const session = this.getCurrentSession();
    if (!session) {
      Logger.warn('[SessionManager] Cannot finalize assistant message: no current session');
      this.resetAccumulator();
      return null;
    }

    const message: Message = {
      role: 'assistant',
      content: this.textAccumulator,
      timestamp: new Date().toISOString(),
      format: 'text'
    };

    session.messages.push(message);
    
    // Update token count
    session.token_count += Math.ceil(this.textAccumulator.length / 4);
    session.updated_at = new Date().toISOString();

    Logger.info(`[SessionManager] Assistant message finalized in session ${session.session_id}`, {
      contentLength: this.textAccumulator.length,
      totalMessages: session.messages.length,
      tokenCount: session.token_count
    });

    this.emit('message-added', {
      sessionId: session.session_id,
      message
    });

    // Reset accumulator
    const finalMessage = message;
    this.resetAccumulator();
    
    return finalMessage;
  }

  /**
   * Clear the text accumulator
   */
  resetAccumulator(): void {
    this.textAccumulator = '';
    this.isAccumulating = false;
  }

  /**
   * Get the current accumulated text
   * @deprecated EventStreamProcessor now handles text accumulation
   * @returns Current accumulated text
   */
  getAccumulatedText(): string {
    return this.textAccumulator;
  }

  /**
   * Check if currently accumulating text
   * @deprecated EventStreamProcessor now handles text accumulation
   * @returns True if accumulating
   */
  isAccumulatingText(): boolean {
    return this.isAccumulating;
  }

  /**
   * Clear a specific session
   * @param sessionId - ID of the session to clear
   * @returns True if session was cleared, false if not found
   * @emits session-cleared event
   */
  clearSession(sessionId: string): boolean {
    const session = this.sessions.get(sessionId);
    if (!session) {
      Logger.warn(`[SessionManager] Cannot clear session: ${sessionId} not found`);
      return false;
    }

    // If clearing current session, reset current session ID
    if (this.currentSessionId === sessionId) {
      this.currentSessionId = null;
      this.resetAccumulator();
    }

    this.sessions.delete(sessionId);
    
    Logger.info(`[SessionManager] Session cleared: ${sessionId}`);
    
    this.emit('session-cleared', { sessionId });
    this.emit('sessions-updated', { sessions: this.sessions });
    
    return true;
  }

  /**
   * Clear all sessions and reset state
   * @emits all-sessions-cleared event
   */
  reset(): void {
    const sessionCount = this.sessions.size;
    
    this.sessions.clear();
    this.currentSessionId = null;
    this.resetAccumulator();
    
    Logger.info(`[SessionManager] All sessions cleared (${sessionCount} sessions removed)`);
    
    this.emit('all-sessions-cleared', undefined);
    this.emit('sessions-updated', { sessions: this.sessions });
  }

  /**
   * Update session metadata
   * @param sessionId - Session ID to update
   * @param metadata - Metadata to merge with existing metadata
   * @returns True if updated, false if session not found
   */
  updateSessionMetadata(sessionId: string, metadata: Record<string, unknown>): boolean {
    const session = this.sessions.get(sessionId);
    if (!session) {
      Logger.warn(`[SessionManager] Cannot update metadata: session ${sessionId} not found`);
      return false;
    }

    session.metadata = {
      ...session.metadata,
      ...metadata
    };
    session.updated_at = new Date().toISOString();

    Logger.debug(`[SessionManager] Session metadata updated for ${sessionId}`, metadata);
    
    this.emit('sessions-updated', { sessions: this.sessions });
    
    return true;
  }

  /**
   * Update session name
   * @param sessionId - Session ID to update
   * @param name - New session name
   * @returns True if updated, false if session not found
   */
  updateSessionName(sessionId: string, name: string): boolean {
    const session = this.sessions.get(sessionId);
    if (!session) {
      Logger.warn(`[SessionManager] Cannot update name: session ${sessionId} not found`);
      return false;
    }

    session.session_name = name;
    session.updated_at = new Date().toISOString();

    Logger.info(`[SessionManager] Session name updated for ${sessionId}: ${name}`);
    
    this.emit('sessions-updated', { sessions: this.sessions });
    
    return true;
  }

  /**
   * Prune the oldest session when at max capacity
   */
  private pruneOldestSession(): void {
    if (this.sessions.size === 0) return;

    let oldestSessionId: string | null = null;
    let oldestTime: Date | null = null;

    // Find the oldest session (excluding current)
    for (const [id, session] of this.sessions) {
      if (id === this.currentSessionId) continue;
      
      const sessionTime = new Date(session.updated_at || session.created_at || 0);
      if (!oldestTime || sessionTime < oldestTime) {
        oldestTime = sessionTime;
        oldestSessionId = id;
      }
    }

    if (oldestSessionId) {
      Logger.info(`[SessionManager] Pruning oldest session: ${oldestSessionId}`);
      this.clearSession(oldestSessionId);
    }
  }

  /**
   * Set the session index from a paginated query response
   * Used when receiving initial sessions from login or when fetching additional sessions
   * @param response - ChatSessionQueryResponse from server
   * @param append - Whether to append to existing index or replace it
   */
  setSessionIndex(response: ChatSessionQueryResponse, append: boolean = false): void {
    if (!response) {
      Logger.error('[SessionManager] Invalid session query response provided to setSessionIndex');
      return;
    }

    if (append) {
      // Append new sessions to existing index, avoiding duplicates
      const existingIds = new Set(this.sessionIndex.map(s => s.session_id));
      const newSessions = response.chat_sessions.filter(s => !existingIds.has(s.session_id));
      this.sessionIndex = [...this.sessionIndex, ...newSessions];
    } else {
      // Replace the entire index
      this.sessionIndex = response.chat_sessions || [];
    }

    this.totalSessionCount = response.total_sessions;
    this.lastFetchOffset = response.offset;

    Logger.info(`[SessionManager] Session index updated`, {
      sessionCount: this.sessionIndex.length,
      totalSessions: this.totalSessionCount,
      offset: this.lastFetchOffset,
      appended: append
    });

    this.emit('sessions-index-updated', {
      sessionIndex: this.sessionIndex,
      totalSessions: this.totalSessionCount
    });
  }

  /**
   * Get the current session index
   * @returns Array of ChatSessionIndexEntry
   */
  getSessionIndex(): ChatSessionIndexEntry[] {
    return [...this.sessionIndex];
  }

  /**
   * Get total number of sessions available on the server
   * @returns Total session count from last query
   */
  getTotalSessionCount(): number {
    return this.totalSessionCount;
  }

  /**
   * Check if there are more sessions to fetch
   * @returns True if there are unfetched sessions
   */
  hasMoreSessions(): boolean {
    return this.sessionIndex.length < this.totalSessionCount;
  }

  /**
   * Request more sessions from the server
   * Emits an event that should be handled by RealtimeClient to send the request
   * @param limit - Number of sessions to fetch (default 50)
   */
  requestMoreSessions(limit: number = 50): void {
    const offset = this.sessionIndex.length;
    
    Logger.info(`[SessionManager] Requesting more sessions`, {
      offset,
      limit,
      currentCount: this.sessionIndex.length,
      totalAvailable: this.totalSessionCount
    });

    this.emit('request-user-sessions', {
      offset,
      limit
    });
  }

  /**
   * Get session statistics
   * @returns Statistics about current sessions
   */
  getStatistics(): {
    totalSessions: number;
    totalMessages: number;
    totalTokens: number;
    currentSessionId: string | null;
    hasActiveAccumulation: boolean;
    sessionIndexCount: number;
    totalSessionsAvailable: number;
  } {
    let totalMessages = 0;
    let totalTokens = 0;

    for (const session of this.sessions.values()) {
      totalMessages += session.messages.length;
      totalTokens += session.token_count;
    }

    return {
      totalSessions: this.sessions.size,
      totalMessages,
      totalTokens,
      currentSessionId: this.currentSessionId,
      hasActiveAccumulation: this.isAccumulating,
      sessionIndexCount: this.sessionIndex.length,
      totalSessionsAvailable: this.totalSessionCount
    };
  }

  /**
   * Clean up resources
   */
  cleanup(): void {
    this.reset();
    this.sessionIndex = [];
    this.totalSessionCount = 0;
    this.lastFetchOffset = 0;
    this.removeAllListeners();
    Logger.info('[SessionManager] SessionManager cleaned up');
  }

  /**
   * Destroy the session manager
   * Alias for cleanup for consistency
   */
  destroy(): void {
    this.cleanup();
  }
}