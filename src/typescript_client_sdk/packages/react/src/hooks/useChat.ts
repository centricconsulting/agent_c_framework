/**
 * useChat - React hook for chat functionality
 * Provides interface for sending messages and accessing chat history
 */

import { useEffect, useState, useCallback, useRef } from 'react';
import type { ChatSession } from '@agentc/realtime-core';
import { Logger } from '../utils/logger';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
import { AgentStorage } from '../utils/agentStorage';
import { 
  ChatItem, 
  MessageChatItem, 
  DividerChatItem, 
  MediaChatItem,
  SystemAlertChatItem,
  isMessageItem 
} from '../types/chat';

// For backward compatibility, keep ExtendedMessage as an alias
type ExtendedMessage = MessageChatItem;

/**
 * Options for the useChat hook
 */
export interface UseChatOptions {
  /** Maximum number of messages to keep in memory */
  maxMessages?: number;
  
  /** Whether to auto-scroll to new messages */
  autoScroll?: boolean;
}

/**
 * Return type for the useChat hook
 */
export interface UseChatReturn {
  /** Current chat items (messages, dividers, media, alerts) */
  messages: ChatItem[];
  
  /** Current session information */
  currentSession: ChatSession | null;
  
  /** Current session ID */
  currentSessionId: string | null;
  
  /** Send a text message with optional file attachments */
  sendMessage: (text: string, fileIds?: string[]) => Promise<void>;
  
  /** Clear chat history (client-side only) */
  clearMessages: () => void;
  
  /** Whether a message is currently being sent */
  isSending: boolean;
  
  /** Whether the agent is currently typing/processing */
  isAgentTyping: boolean;
  
  /** Current streaming message from agent */
  streamingMessage: MessageChatItem | null;
  
  /** Error state */
  error: string | null;
  
  /** Get the last message */
  lastMessage: MessageChatItem | null;
  
  /** Get messages from a specific role */
  getMessagesByRole: (role: 'user' | 'assistant' | 'system') => MessageChatItem[];
  
  /** Check if a message is from a sub-session */
  isSubSessionMessage: (message: ChatItem) => boolean;
}

/**
 * React hook for chat functionality
 * Provides interface to SessionManager and text messaging
 */
export function useChat(options: UseChatOptions = {}): UseChatReturn {
  const { maxMessages = 100 } = options;
  const client = useRealtimeClientSafe();
  
  // State
  const [messages, setMessages] = useState<ChatItem[]>([]);
  const [currentSession, setCurrentSession] = useState<ChatSession | null>(null);
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  const [isSending, setIsSending] = useState(false);
  const [isAgentTyping, setIsAgentTyping] = useState(false);
  const [streamingMessage, setStreamingMessage] = useState<MessageChatItem | null>(null);
  const [error, setError] = useState<string | null>(null);
  
  // Track the current streaming message ID to avoid duplicates
  const streamingMessageIdRef = useRef<string | null>(null);
  // Track the expected session ID to validate incoming messages
  const expectedSessionIdRef = useRef<string | null>(null);
  // Track if we're currently loading a new session (use ref to avoid stale closures)
  const isLoadingSessionRef = useRef<boolean>(false);
  // Track if messages have been loaded for the current session
  const messagesLoadedForSessionRef = useRef<string | null>(null);
  // Track current session ID in ref for immediate access in event handlers
  const currentSessionIdRef = useRef<string | null>(null);
  
  // Update session and messages from client
  const updateChatInfo = useCallback(() => {
    if (!client) {
      setCurrentSession(null);
      setCurrentSessionId(null);
      currentSessionIdRef.current = null;
      setMessages([]);
      return;
    }
    
    const sessionManager = client.getSessionManager();
    if (!sessionManager) {
      setCurrentSession(null);
      setCurrentSessionId(null);
      currentSessionIdRef.current = null;
      setMessages([]);
      return;
    }
    
    try {
      const session = sessionManager.getCurrentSession();
      setCurrentSession(session);
      const sessionId = session?.session_id || null;
      setCurrentSessionId(sessionId);
      currentSessionIdRef.current = sessionId;
      
      if (session) {
        // SessionManager doesn't have getMessageHistory, we'll track messages ourselves
        // Messages are accumulated from events
      }
      
      setError(null);
    } catch (err) {
      console.error('Failed to get chat information:', err);
      setError(err instanceof Error ? err.message : 'Failed to get chat information');
    }
  }, [client]);
  
  // Send message
  const sendMessage = useCallback(async (text: string, fileIds?: string[]): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    if (!client.isConnected()) {
      throw new Error('Not connected to server');
    }
    
    if (!text.trim()) {
      throw new Error('Message cannot be empty');
    }
    
    setIsSending(true);
    setError(null);
    
    try {
      // Use sendText method - EventStreamProcessor will handle the message events
      // Pass fileIds if provided for multimodal messages
      client.sendText(text, fileIds);
      
      // Don't add user message locally - EventStreamProcessor will emit message-added event
      // This ensures we get the proper sub-session metadata if applicable
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to send message';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsSending(false);
    }
  }, [client]);
  
  // Clear messages (client-side only)
  const clearMessages = useCallback(() => {
    setMessages([]);
    setStreamingMessage(null);
    streamingMessageIdRef.current = null;
  }, []);
  
  // Get messages by role
  const getMessagesByRole = useCallback((role: 'user' | 'assistant' | 'system'): MessageChatItem[] => {
    return messages
      .filter(isMessageItem)
      .filter(msg => msg.role === role);
  }, [messages]);
  
  // Check if a message is from a sub-session
  const isSubSessionMessage = useCallback((message: ChatItem): boolean => {
    return isMessageItem(message) && message.isSubSession === true;
  }, []);
  
  // Track listener registration for diagnostics
  const listenerCountRef = useRef(0);
  
  // Counter for generating unique message IDs when messages lack IDs
  const messageIdCounterRef = useRef(0);
  
  // Define stable handler functions using useCallback
  // These need to be stable references so cleanup can properly remove them
  
  // Handle message-added events (complete messages from EventStreamProcessor)
  const handleMessageAdded = useCallback((event: unknown) => {
      const messageEvent = event as { sessionId: string; message: ExtendedMessage };
      Logger.debug('[useChat] Message added event:', messageEvent);
      
      // Don't add messages while loading a new session - wait for session-messages-loaded
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring message-added during session loading');
        return;
      }
      
      setMessages(prev => {
        // Ensure the message has a type and id for compatibility
        const messageToAdd: MessageChatItem = messageEvent.message.type 
          ? messageEvent.message 
          : { ...messageEvent.message, type: 'message', id: `msg-added-${Date.now()}-${++messageIdCounterRef.current}` };
        
        const newMessages = [...prev, messageToAdd];
        // Limit messages if needed
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
      
      // Clear streaming state if this was the streaming message
      if (streamingMessageIdRef.current === messageEvent.sessionId) {
        setStreamingMessage(null);
        streamingMessageIdRef.current = null;
        setIsAgentTyping(false);
      }
    }, [maxMessages]);
    
    // Handle message-streaming events (partial messages being built)
    const handleMessageStreaming = useCallback((event: unknown) => {
      const streamEvent = event as { sessionId: string; message?: ExtendedMessage };
      Logger.debug('[useChat] Message streaming event received');
      Logger.debug('[useChat] Stream event structure:', {
        hasSessionId: 'sessionId' in (streamEvent as any),
        hasMessage: 'message' in (streamEvent as any),
        messageType: streamEvent.message ? typeof streamEvent.message : 'undefined',
        messageContent: streamEvent.message?.content,
        messageRole: streamEvent.message?.role
      });
      
      // Don't update streaming state while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring message-streaming during session loading');
        return;
      }
      
      // Only update if we have a valid message
      if (streamEvent.message) {
        Logger.debug('[useChat] Setting streaming message:', streamEvent.message);
        
        // Keep isAgentTyping true during streaming
        // The UI will show the streaming message instead of typing indicator when streamingMessage exists
        setIsAgentTyping(true);
        setStreamingMessage(streamEvent.message);
        streamingMessageIdRef.current = streamEvent.sessionId;
      } else {
        Logger.warn('[useChat] Message streaming event received without message property');
      }
    }, []);
    
    // Handle message-complete events (finalized messages)
    const handleMessageComplete = useCallback((event: unknown) => {
      const completeEvent = event as { sessionId: string; message: ExtendedMessage };
      Logger.debug('[useChat] Message complete event:', completeEvent);
      
      // Don't add messages while loading a new session - wait for session-messages-loaded
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring message-complete during session loading');
        return;
      }
      
      // Add the completed message to the messages array
      setMessages(prev => {
        // Ensure the message has a type and id for compatibility
        const messageToAdd: MessageChatItem = completeEvent.message.type 
          ? completeEvent.message 
          : { ...completeEvent.message, type: 'message', id: `msg-complete-${Date.now()}-${++messageIdCounterRef.current}` };
        
        const newMessages = [...prev, messageToAdd];
        // Limit messages if needed
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
      
      // Clear all streaming/typing state
      setStreamingMessage(null);
      streamingMessageIdRef.current = null;
      setIsAgentTyping(false);
      Logger.debug('[useChat] Message complete - cleared streaming and typing state');
    }, [maxMessages]);
    
    // Handle message-updated events (tool calls attached to existing messages)
    const handleMessageUpdated = useCallback((event: unknown) => {
      const updateEvent = event as { sessionId: string; messageId: string; message: ExtendedMessage };
      Logger.debug('[useChat] Message updated event:', updateEvent);
      
      // Don't update messages while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring message-updated during session loading');
        return;
      }
      
      // Find and update the message in the array
      setMessages(prev => {
        const messageIndex = prev.findIndex(m => 
          isMessageItem(m) && m.id === updateEvent.messageId
        );
        
        if (messageIndex === -1) {
          Logger.warn('[useChat] Message to update not found:', updateEvent.messageId);
          return prev;
        }
        
        // Create new array with updated message
        const newMessages = [...prev];
        newMessages[messageIndex] = {
          ...updateEvent.message,
          type: updateEvent.message.type || 'message'
        } as MessageChatItem;
        
        Logger.info('[useChat] Updated message in array:', {
          messageId: updateEvent.messageId,
          index: messageIndex,
          toolCallsCount: (updateEvent.message as any).toolCalls?.length || 0,
          hasMetadataToolCalls: !!(updateEvent.message as any).metadata?.toolCalls,
          hasTopLevelToolCalls: !!(updateEvent.message as any).toolCalls,
          messageStructure: {
            role: updateEvent.message.role,
            hasMetadata: !!(updateEvent.message as any).metadata,
            metadataKeys: Object.keys((updateEvent.message as any).metadata || {}),
            topLevelKeys: Object.keys(updateEvent.message)
          }
        });
        
        return newMessages;
      });
    }, []);
    
    // Handle turn events from server for typing indicators
    const handleUserTurnStart = useCallback(() => {
      // User is speaking/typing, agent is not
      setIsAgentTyping(false);
    }, []);
    
    const handleUserTurnEnd = useCallback(() => {
      // User's turn has ended, agent is about to respond
      // Show typing indicator until actual content arrives
      setIsAgentTyping(true);
      Logger.debug('[useChat] User turn ended, showing typing indicator');
    }, []);
    
    // Handle session change events
    const handleSessionChanged = useCallback((event: unknown) => {
      const sessionEvent = event as { chat_session?: ChatSession };
      
      // CRITICAL: Always clear messages when session changes, NO EXCEPTIONS
      console.log('[useChat] SESSION CHANGE - CLEARING MESSAGES');
      setMessages([]);
      setStreamingMessage(null);
      streamingMessageIdRef.current = null;
      setIsAgentTyping(false);
      
      if (sessionEvent.chat_session) {
        const newSessionId = sessionEvent.chat_session.session_id;
        console.log('[useChat] New session ID:', newSessionId);
        console.log('[useChat] Has inline messages:', sessionEvent.chat_session.messages?.length || 0);
        
        // Update session state
        setCurrentSession(sessionEvent.chat_session);
        setCurrentSessionId(newSessionId);
        currentSessionIdRef.current = newSessionId;
        
        // CRITICAL FIX: Save agent key when session changes
        // This ensures we persist the agent key when resuming a chat session
        // as required by the connection/reconnection improvements
        if (sessionEvent.chat_session.agent_config?.key) {
          try {
            AgentStorage.saveAgentKey(sessionEvent.chat_session.agent_config.key);
            console.log('[useChat] Saved agent key from session change:', sessionEvent.chat_session.agent_config.key);
          } catch (err) {
            console.warn('[useChat] Failed to save agent key:', err);
          }
        }
        
        // Set loading state to block incoming events during transition
        isLoadingSessionRef.current = true;
        expectedSessionIdRef.current = newSessionId;
        messagesLoadedForSessionRef.current = null;
        
        // CRITICAL CHANGE: Do NOT load inline messages here!
        // Let the EventStreamProcessor handle ALL message loading via events
        // This prevents the immediate re-population of messages after clearing
        
        if (sessionEvent.chat_session.messages && sessionEvent.chat_session.messages.length > 0) {
          console.log('[useChat] Session has inline messages, but NOT loading them directly');
          console.log('[useChat] Waiting for EventStreamProcessor to emit events');
          // EventStreamProcessor will handle these via mapResumedMessagesToEvents or session-messages-loaded
        } else {
          console.log('[useChat] Empty session - waiting for session-messages-loaded event');
        }
      } else {
        // No session - clear everything
        console.log('[useChat] No session provided - clearing all state');
        setCurrentSession(null);
        setCurrentSessionId(null);
        currentSessionIdRef.current = null;
        isLoadingSessionRef.current = false;
        expectedSessionIdRef.current = null;
        messagesLoadedForSessionRef.current = null;
      }
    }, []); // No dependencies - uses refs and setters
    
    // Named handler for chat-session-changed event from SessionManager
    // This wraps handleSessionChanged and can be properly cleaned up
    const handleChatSessionChanged = useCallback((data: { currentChatSession: ChatSession | null; previousChatSession: ChatSession | null }) => {
      if (data.currentChatSession) {
        handleSessionChanged({ chat_session: data.currentChatSession });
      }
    }, [handleSessionChanged]);
    
    // Handle session messages loaded event (from EventStreamProcessor)
    const handleSessionMessagesLoaded = useCallback((event: unknown) => {
      const messagesEvent = event as { sessionId?: string; messages?: ExtendedMessage[] };
      const eventSessionId = messagesEvent.sessionId;
      
      Logger.debug('[useChat] Session messages loaded event');
      Logger.debug('[useChat] Event session ID:', eventSessionId);
      Logger.debug('[useChat] Expected session ID:', expectedSessionIdRef.current);
      Logger.debug('[useChat] Is loading session:', isLoadingSessionRef.current);
      Logger.debug('[useChat] Current session ID:', currentSessionIdRef.current);
      
      // Validation logic for race condition prevention:
      // 1. During session loading (isLoadingSessionRef.current === true):
      //    - Accept if no sessionId (backward compat)
      //    - Accept if sessionId matches expected
      //    - Reject if sessionId doesn't match expected
      // 2. Not during session loading (isLoadingSessionRef.current === false):
      //    - If no current session: accept any (test scenario)
      //    - If has current session:
      //      - Check if we already loaded messages for this session (prevent duplicates)
      //      - With sessionId: must match current
      //      - Without sessionId: reject if already loaded
      
      if (isLoadingSessionRef.current) {
        // During session transition - validate sessionId if provided
        if (eventSessionId && eventSessionId !== expectedSessionIdRef.current) {
          Logger.debug('[useChat] Ignoring messages - wrong session during transition');
          return;
        }
      } else {
        // Not loading
        const currentSession = currentSessionIdRef.current;
        if (!currentSession) {
          // No current session - accept for test scenarios
          Logger.debug('[useChat] No current session - accepting messages for test');
        } else {
          // Has current session
          // Check if we already loaded messages for this session
          if (messagesLoadedForSessionRef.current === currentSession && !eventSessionId) {
            Logger.debug('[useChat] Ignoring messages - already loaded for current session');
            return;
          }
          
          // If event has sessionId, it must match current
          if (eventSessionId && eventSessionId !== currentSession) {
            Logger.debug('[useChat] Ignoring messages - wrong session ID');
            return;
          }
        }
      }
      
      // IMPORTANT: Handle both populated and empty message arrays
      // An empty array means we need to clear the chat display
      if (messagesEvent.messages !== undefined) {
        Logger.debug('[useChat] Messages from event:', messagesEvent.messages.length, 'messages');
        if (messagesEvent.messages.length > 0) {
          Logger.debug('[useChat] First 3 messages from event:', messagesEvent.messages.slice(0, 3));
        } else {
          Logger.debug('[useChat] Empty messages array - clearing chat display');
        }
        
        // Ensure all messages have type field for compatibility
        const messagesWithType = messagesEvent.messages.map((msg, idx) => 
          msg.type ? msg : { ...msg, type: 'message' as const, id: `loaded-${idx}` }
        );
        
        // Messages come from EventStreamProcessor already formatted with sub-session metadata
        const messagesToSet = maxMessages && maxMessages > 0
          ? messagesWithType.slice(-maxMessages)
          : messagesWithType;
        Logger.debug('[useChat] After slice for loaded messages:', messagesToSet.length, 'messages');
        
        // Set messages - this will clear the display if messagesToSet is empty
        setMessages(messagesToSet as ChatItem[]);
        
        // Clear any streaming state
        setStreamingMessage(null);
        streamingMessageIdRef.current = null;
        setIsAgentTyping(false);
        
        // Mark messages as loaded for the appropriate session
        // During loading, use expectedSessionIdRef; otherwise use currentSessionIdRef
        const sessionToMark = isLoadingSessionRef.current ? expectedSessionIdRef.current : currentSessionIdRef.current;
        if (sessionToMark) {
          messagesLoadedForSessionRef.current = sessionToMark;
        }
        
        // Clear loading state if we were loading
        if (isLoadingSessionRef.current) {
          isLoadingSessionRef.current = false;
          Logger.debug('[useChat] Session loading complete');
        }
      }
    }, [maxMessages]); // currentSessionId accessed via ref
    
    // Counter for generating unique divider IDs
    const dividerCounterRef = useRef(0);
    
    // Handle subsession started events
    const handleSubsessionStarted = useCallback((event: unknown) => {
      const subsessionEvent = event as {
        subSessionType?: 'chat' | 'oneshot';
        subAgentType?: 'clone' | 'team' | 'assist' | 'tool';
        primeAgentKey?: string;
        subAgentKey?: string;
      };
      Logger.debug('[useChat] Subsession started event:', subsessionEvent);
      
      // Don't add dividers while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring subsession-started during session loading');
        return;
      }
      
      const divider: DividerChatItem = {
        id: `divider-start-${Date.now()}-${++dividerCounterRef.current}`,
        type: 'divider',
        dividerType: 'start',
        timestamp: new Date().toISOString(),
        metadata: {
          subSessionType: subsessionEvent.subSessionType,
          subAgentType: subsessionEvent.subAgentType,
          primeAgentKey: subsessionEvent.primeAgentKey,
          subAgentKey: subsessionEvent.subAgentKey
        }
      };
      
      setMessages(prev => {
        const newMessages = [...prev, divider];
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
    }, [maxMessages]);
    
    // Handle subsession ended events
    const handleSubsessionEnded = useCallback((_event: unknown) => {
      Logger.debug('[useChat] Subsession ended event');
      
      // Don't add dividers while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring subsession-ended during session loading');
        return;
      }
      
      const divider: DividerChatItem = {
        id: `divider-end-${Date.now()}-${++dividerCounterRef.current}`,
        type: 'divider',
        dividerType: 'end',
        timestamp: new Date().toISOString()
      };
      
      setMessages(prev => {
        const newMessages = [...prev, divider];
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
    }, [maxMessages]);
    
    // Counter for generating unique media IDs
    const mediaCounterRef = useRef(0);
    
    // Handle media added events (RenderMedia)
    const handleMediaAdded = useCallback((event: unknown) => {
      const mediaEvent = event as {
        sessionId: string;
        media: {
          id?: string;
          content?: string;
          contentType: string;
          timestamp?: string;
          metadata?: {
            sent_by_class?: string;
            sent_by_function?: string;
            foreign_content?: boolean;
            url?: string;
            name?: string;
          };
        };
      };
      Logger.debug('[useChat] Media added event:', mediaEvent);
      
      // Don't add media while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring media-added during session loading');
        return;
      }
      
      const mediaItem: MediaChatItem = {
        id: mediaEvent.media.id || `media-${Date.now()}-${++mediaCounterRef.current}`,
        type: 'media',
        timestamp: mediaEvent.media.timestamp || new Date().toISOString(),
        content: mediaEvent.media.content,
        contentType: mediaEvent.media.contentType,
        metadata: {
          sentByClass: mediaEvent.media.metadata?.sent_by_class,
          sentByFunction: mediaEvent.media.metadata?.sent_by_function,
          foreignContent: mediaEvent.media.metadata?.foreign_content,
          url: mediaEvent.media.metadata?.url,
          name: mediaEvent.media.metadata?.name
        }
      };
      
      setMessages(prev => {
        const newMessages = [...prev, mediaItem];
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
    }, [maxMessages]);
    
    // Counter for generating unique system message IDs
    const systemMessageCounterRef = useRef(0);
    
    // Handle system message events (system alerts in chat)
    const handleSystemMessage = useCallback((event: unknown) => {
      const systemEvent = event as {
        content: string;
        severity?: 'info' | 'warning' | 'error';
        format?: 'markdown' | 'text';
        timestamp?: string;
        session_id?: string;
      };
      Logger.debug('[useChat] System message event:', systemEvent);
      
      // Don't add system messages while loading a new session
      if (isLoadingSessionRef.current) {
        Logger.debug('[useChat] Ignoring system_message during session loading');
        return;
      }
      
      // Generate a unique ID using timestamp, counter, and random component
      // This prevents collisions even when multiple events arrive in the same millisecond
      const uniqueId = `system-${Date.now()}-${++systemMessageCounterRef.current}-${Math.random().toString(36).substr(2, 9)}`;
      
      const systemAlert: SystemAlertChatItem = {
        id: uniqueId,
        type: 'system_alert',
        timestamp: systemEvent.timestamp || new Date().toISOString(),
        content: systemEvent.content,
        severity: systemEvent.severity || 'info',
        format: systemEvent.format || 'markdown'
      };
      
      setMessages(prev => {
        const newMessages = [...prev, systemAlert];
        if (maxMessages && maxMessages > 0 && newMessages.length > maxMessages) {
          return newMessages.slice(-maxMessages);
        }
        return newMessages;
      });
    }, [maxMessages]);
  
  // Subscribe to chat events
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    
    // Initial update
    updateChatInfo();
    
    // Increment listener count for diagnostics
    listenerCountRef.current += 1;
    const registrationId = listenerCountRef.current;
    console.log(`[useChat:DIAGNOSTIC] 🔵 Registering listeners #${registrationId}`);
    
    // Subscribe to turn events on client for typing indicators
    // These are always subscribed if client exists
    client.on('user_turn_start', handleUserTurnStart);
    client.on('user_turn_end', handleUserTurnEnd);
    
    // Subscribe to ChatSessionManager events for message handling (only if sessionManager exists)
    if (sessionManager) {
      // Chat session change events come from ChatSessionManager, not client
      sessionManager.on('chat-session-changed', handleChatSessionChanged);
      sessionManager.on('message-added', handleMessageAdded);
      sessionManager.on('message-streaming', handleMessageStreaming);
      sessionManager.on('message-complete', handleMessageComplete);
      sessionManager.on('message-updated', handleMessageUpdated);
      sessionManager.on('session-messages-loaded', handleSessionMessagesLoaded);
      
      // New event subscriptions for Phase 1
      sessionManager.on('subsession-started', handleSubsessionStarted);
      sessionManager.on('subsession-ended', handleSubsessionEnded);
      sessionManager.on('media-added', handleMediaAdded);
      sessionManager.on('system_message', handleSystemMessage);
      // Note: 'error' events are handled in useErrors hook for toast notifications
    }
    
    return () => {
      // Log cleanup for diagnostics
      console.log(`[useChat:DIAGNOSTIC] 🔴 Cleaning up listeners #${registrationId}`);
      
      // client and sessionManager must be accessible in closure
      if (client) {
        client.off('user_turn_start', handleUserTurnStart);
        client.off('user_turn_end', handleUserTurnEnd);
      }
      
      // Get sessionManager again in cleanup to ensure it's available
      const cleanupSessionManager = client?.getSessionManager();
      if (cleanupSessionManager) {
        // Clean up chat-session-changed listener using named handler reference
        cleanupSessionManager.off('chat-session-changed', handleChatSessionChanged);
        cleanupSessionManager.off('message-added', handleMessageAdded);
        cleanupSessionManager.off('message-streaming', handleMessageStreaming);
        cleanupSessionManager.off('message-complete', handleMessageComplete);
        cleanupSessionManager.off('message-updated', handleMessageUpdated);
        cleanupSessionManager.off('session-messages-loaded', handleSessionMessagesLoaded);
        
        // Cleanup new event subscriptions
        cleanupSessionManager.off('subsession-started', handleSubsessionStarted);
        cleanupSessionManager.off('subsession-ended', handleSubsessionEnded);
        cleanupSessionManager.off('media-added', handleMediaAdded);
        cleanupSessionManager.off('system_message', handleSystemMessage);
      }
      
      console.log(`[useChat:DIAGNOSTIC] ✅ Cleanup complete for #${registrationId}`);
    };
  }, [
    client,
    // updateChatInfo removed - only called once on mount, not used by handlers
    handleMessageAdded,
    handleMessageStreaming,
    handleMessageComplete,
    handleMessageUpdated,
    handleUserTurnStart,
    handleUserTurnEnd,
    handleChatSessionChanged, // CRITICAL FIX: Was handleSessionChanged, but we subscribe with handleChatSessionChanged
    handleSessionMessagesLoaded,
    handleSubsessionStarted,
    handleSubsessionEnded,
    handleMediaAdded,
    handleSystemMessage
  ]);
  
  // Computed properties
  const lastMessage: MessageChatItem | null = (() => {
    // Find the last actual message (not divider, media, or alert)
    for (let i = messages.length - 1; i >= 0; i--) {
      const item = messages[i];
      if (item && isMessageItem(item)) {
        return item;
      }
    }
    return null;
  })();
  
  return {
    messages,
    currentSession,
    currentSessionId,
    sendMessage,
    clearMessages,
    isSending,
    isAgentTyping,
    streamingMessage,
    error,
    lastMessage,
    getMessagesByRole,
    isSubSessionMessage
  };
}