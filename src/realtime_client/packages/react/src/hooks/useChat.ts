/**
 * useChat - React hook for chat functionality
 * Provides interface for sending messages and accessing chat history
 */

import { useEffect, useState, useCallback, useRef } from 'react';
import type { ChatSession } from '@agentc/realtime-core';
import { ensureMessagesFormat } from '@agentc/realtime-core';
import { Logger } from '../utils/logger';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
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
  
  /** Send a text message */
  sendMessage: (text: string) => Promise<void>;
  
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
  
  // Update session and messages from client
  const updateChatInfo = useCallback(() => {
    if (!client) {
      setCurrentSession(null);
      setCurrentSessionId(null);
      setMessages([]);
      return;
    }
    
    const sessionManager = client.getSessionManager();
    if (!sessionManager) {
      setCurrentSession(null);
      setCurrentSessionId(null);
      setMessages([]);
      return;
    }
    
    try {
      const session = sessionManager.getCurrentSession();
      setCurrentSession(session);
      setCurrentSessionId(session?.session_id || null);
      
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
  const sendMessage = useCallback(async (text: string): Promise<void> => {
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
      client.sendText(text);
      
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
  
  // Subscribe to chat events
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    
    // Initial update
    updateChatInfo();
    
    // Handle message-added events (complete messages from EventStreamProcessor)
    const handleMessageAdded = (event: unknown) => {
      const messageEvent = event as { sessionId: string; message: ExtendedMessage };
      Logger.debug('[useChat] Message added event:', messageEvent);
      
      setMessages(prev => {
        // Ensure the message has a type and id for compatibility
        const messageToAdd: MessageChatItem = messageEvent.message.type 
          ? messageEvent.message 
          : { ...messageEvent.message, type: 'message', id: messageEvent.sessionId };
        
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
    };
    
    // Handle message-streaming events (partial messages being built)
    const handleMessageStreaming = (event: unknown) => {
      const streamEvent = event as { sessionId: string; message?: ExtendedMessage };
      Logger.debug('[useChat] Message streaming event received');
      Logger.debug('[useChat] Stream event structure:', {
        hasSessionId: 'sessionId' in (streamEvent as any),
        hasMessage: 'message' in (streamEvent as any),
        messageType: streamEvent.message ? typeof streamEvent.message : 'undefined',
        messageContent: streamEvent.message?.content,
        messageRole: streamEvent.message?.role
      });
      
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
    };
    
    // Handle message-complete events (finalized messages)
    const handleMessageComplete = (event: unknown) => {
      const completeEvent = event as { sessionId: string; message: ExtendedMessage };
      Logger.debug('[useChat] Message complete event:', completeEvent);
      
      // Add the completed message to the messages array
      setMessages(prev => {
        // Ensure the message has a type and id for compatibility
        const messageToAdd: MessageChatItem = completeEvent.message.type 
          ? completeEvent.message 
          : { ...completeEvent.message, type: 'message', id: completeEvent.sessionId };
        
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
    };
    
    // Handle turn events from server for typing indicators
    const handleUserTurnStart = () => {
      // User is speaking/typing, agent is not
      setIsAgentTyping(false);
    };
    
    const handleUserTurnEnd = () => {
      // User's turn has ended, agent is about to respond
      // Show typing indicator until actual content arrives
      setIsAgentTyping(true);
      Logger.debug('[useChat] User turn ended, showing typing indicator');
    };
    
    // Handle session change events
    const handleSessionChanged = (event: unknown) => {
      const sessionEvent = event as { chat_session?: ChatSession };
      if (sessionEvent.chat_session) {
        Logger.debug('[useChat] Session changed event received');
        Logger.debug('[useChat] Session ID:', sessionEvent.chat_session.session_id);
        Logger.debug('[useChat] Raw messages from server:', sessionEvent.chat_session.messages?.length || 0, 'messages');
        
        setCurrentSession(sessionEvent.chat_session);
        setCurrentSessionId(sessionEvent.chat_session.session_id);
        
        // Load messages from the session if present (for backward compatibility)
        if (sessionEvent.chat_session.messages && sessionEvent.chat_session.messages.length > 0) {
          const formattedMessages = ensureMessagesFormat(sessionEvent.chat_session.messages);
          // Ensure all messages have type field for compatibility
          const messagesWithType = formattedMessages.map((msg, idx) => {
            if ('type' in msg) return msg;
            return { ...msg, type: 'message' as const, id: `session-${idx}` };
          });
          const messagesToSet = maxMessages && maxMessages > 0 
            ? messagesWithType.slice(-maxMessages)
            : messagesWithType;
          setMessages(messagesToSet as ChatItem[]);
        } else {
          // Clear messages if session has no messages
          setMessages([]);
        }
        
        // Clear any streaming state when switching sessions
        setStreamingMessage(null);
        streamingMessageIdRef.current = null;
        setIsAgentTyping(false);
      }
      // If no chat_session provided, don't update state (maintain existing messages)
    };
    
    // Handle session messages loaded event (from EventStreamProcessor)
    const handleSessionMessagesLoaded = (event: unknown) => {
      const messagesEvent = event as { sessionId?: string; messages?: ExtendedMessage[] };
      Logger.debug('[useChat] Session messages loaded event');
      Logger.debug('[useChat] Event data:', messagesEvent);
      
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
      }
    };
    
    // Handle subsession started events
    const handleSubsessionStarted = (event: unknown) => {
      const subsessionEvent = event as {
        subSessionType?: 'chat' | 'oneshot';
        subAgentType?: 'clone' | 'team' | 'assist' | 'tool';
        primeAgentKey?: string;
        subAgentKey?: string;
      };
      Logger.debug('[useChat] Subsession started event:', subsessionEvent);
      
      const divider: DividerChatItem = {
        id: `divider-start-${Date.now()}`,
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
    };
    
    // Handle subsession ended events
    const handleSubsessionEnded = (_event: unknown) => {
      Logger.debug('[useChat] Subsession ended event');
      
      const divider: DividerChatItem = {
        id: `divider-end-${Date.now()}`,
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
    };
    
    // Handle media added events (RenderMedia)
    const handleMediaAdded = (event: unknown) => {
      const mediaEvent = event as {
        sessionId: string;
        media: {
          id?: string;
          content: string;
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
      
      const mediaItem: MediaChatItem = {
        id: mediaEvent.media.id || `media-${Date.now()}`,
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
    };
    
    // Handle system message events (system alerts in chat)
    const handleSystemMessage = (event: unknown) => {
      const systemEvent = event as {
        content: string;
        severity?: 'info' | 'warning' | 'error';
        format?: 'markdown' | 'text';
        timestamp?: string;
      };
      Logger.debug('[useChat] System message event:', systemEvent);
      
      const systemAlert: SystemAlertChatItem = {
        id: `system-${Date.now()}`,
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
    };
    
    // Subscribe to turn events on client for typing indicators
    // These are always subscribed if client exists
    client.on('user_turn_start', handleUserTurnStart);
    client.on('user_turn_end', handleUserTurnEnd);
    client.on('chat_session_changed', handleSessionChanged);
    
    // Subscribe to SessionManager events for message handling (only if sessionManager exists)
    if (sessionManager) {
      sessionManager.on('message-added', handleMessageAdded);
      sessionManager.on('message-streaming', handleMessageStreaming);
      sessionManager.on('message-complete', handleMessageComplete);
      sessionManager.on('session-messages-loaded', handleSessionMessagesLoaded);
      
      // New event subscriptions for Phase 1
      sessionManager.on('subsession-started', handleSubsessionStarted);
      sessionManager.on('subsession-ended', handleSubsessionEnded);
      sessionManager.on('media-added', handleMediaAdded);
      sessionManager.on('system_message', handleSystemMessage);
      // Note: 'error' events are handled in useErrors hook for toast notifications
    }
    
    return () => {
      // client and sessionManager must be accessible in closure
      if (client) {
        client.off('user_turn_start', handleUserTurnStart);
        client.off('user_turn_end', handleUserTurnEnd);
        client.off('chat_session_changed', handleSessionChanged);
      }
      
      // Get sessionManager again in cleanup to ensure it's available
      const cleanupSessionManager = client?.getSessionManager();
      if (cleanupSessionManager) {
        cleanupSessionManager.off('message-added', handleMessageAdded);
        cleanupSessionManager.off('message-streaming', handleMessageStreaming);
        cleanupSessionManager.off('message-complete', handleMessageComplete);
        cleanupSessionManager.off('session-messages-loaded', handleSessionMessagesLoaded);
        
        // Cleanup new event subscriptions
        cleanupSessionManager.off('subsession-started', handleSubsessionStarted);
        cleanupSessionManager.off('subsession-ended', handleSubsessionEnded);
        cleanupSessionManager.off('media-added', handleMediaAdded);
        cleanupSessionManager.off('system_message', handleSystemMessage);
      }
    };
  }, [client, maxMessages, updateChatInfo]);
  
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