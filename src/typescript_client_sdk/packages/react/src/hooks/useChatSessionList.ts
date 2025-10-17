/**
 * useChatSessionList - React hook for managing chat session list
 * Provides interface for loading, searching, and managing chat sessions
 */

import { useEffect, useState, useCallback, useRef, useMemo } from 'react';
import type { 
  ChatSessionIndexEntry,
  GetUserSessionsResponseEvent,
  ChatSessionChangedEvent,
  ChatSessionNameChangedEvent,
  ChatSessionAddedEvent,
  ChatSessionDeletedEvent
} from '@agentc/realtime-core';
import { useRealtimeClientSafe } from '../providers/AgentCContext';

/**
 * Parse date string with microseconds support
 * Handles formats like '2025-09-06T20:16:26.515250' or ISO 8601
 */
function parseDate(dateString: string | null | undefined): Date {
  if (!dateString) return new Date(0);
  
  try {
    // Handle microseconds by truncating to milliseconds
    // Format: YYYY-MM-DDTHH:mm:ss.microseconds
    const microsecondPattern = /^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})\.(\d{6})$/;
    const match = dateString.match(microsecondPattern);
    
    if (match) {
      // Convert microseconds to milliseconds (take first 3 digits)
      const [, dateTimePart, microseconds] = match;
      const milliseconds = microseconds ? microseconds.substring(0, 3) : '000';
      const normalizedDate = `${dateTimePart}.${milliseconds}Z`;
      return new Date(normalizedDate);
    }
    
    // Try standard ISO parsing
    const date = new Date(dateString);
    
    // Check if date is valid
    if (isNaN(date.getTime())) {
      console.warn(`Invalid date string: ${dateString}`);
      return new Date(0);
    }
    
    return date;
  } catch (error) {
    console.error(`Error parsing date: ${dateString}`, error);
    return new Date(0);
  }
}

/**
 * Session group type
 */
export type SessionGroup = 'today' | 'recent' | 'past';

/**
 * Grouped sessions interface
 */
export interface GroupedSessions {
  today: ChatSessionIndexEntry[];
  recent: ChatSessionIndexEntry[];  // Past 14 days
  past: ChatSessionIndexEntry[];    // Older than 14 days
}

/**
 * Session group metadata
 */
export interface SessionGroupMeta {
  group: SessionGroup;
  label: string;
  count: number;
  sessions: ChatSessionIndexEntry[];
}

/**
 * Group sessions by time periods with robust date parsing
 */
function groupSessionsByTime(sessions: ChatSessionIndexEntry[]): GroupedSessions {
  const now = new Date();
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const twoWeeksAgo = new Date(today.getTime() - (14 * 24 * 60 * 60 * 1000));
  
  const groups: GroupedSessions = {
    today: [],
    recent: [],
    past: []
  };
  
  // Debug logging for date parsing issues
  const debugDates = sessions.slice(0, 3).map(s => ({
    id: s.session_id.substring(0, 8),
    updated: s.updated_at,
    parsed: parseDate(s.updated_at || s.created_at)
  }));
  
  if (debugDates.length > 0) {
    console.debug('Sample date parsing:', debugDates);
  }
  
  sessions.forEach(session => {
    const sessionDate = parseDate(session.updated_at || session.created_at);
    
    // Additional validation - if date is in the far future, treat as recent
    const oneYearFromNow = new Date(now.getTime() + (365 * 24 * 60 * 60 * 1000));
    if (sessionDate > oneYearFromNow) {
      console.warn(`Session ${session.session_id} has future date: ${session.updated_at}, treating as recent`);
      groups.recent.push(session);
    } else if (sessionDate >= today) {
      groups.today.push(session);
    } else if (sessionDate >= twoWeeksAgo) {
      groups.recent.push(session);
    } else {
      groups.past.push(session);
    }
  });
  
  // Log grouping results for debugging
  console.debug('Session grouping results:', {
    total: sessions.length,
    today: groups.today.length,
    recent: groups.recent.length,
    past: groups.past.length,
    todayStart: today.toISOString(),
    twoWeeksAgoStart: twoWeeksAgo.toISOString()
  });
  
  return groups;
}

/**
 * Options for the useChatSessionList hook
 */
export interface UseChatSessionListOptions {
  /** Page size for pagination (default: 50) */
  pageSize?: number;
  
  /** Whether to load sessions on mount (default: true) */
  autoLoad?: boolean;
  
  /** Debounce delay for search in ms (default: 300) */
  searchDebounceMs?: number;
  
  /** Maximum number of sessions to cache (default: 500) */
  maxCachedSessions?: number;
}

/**
 * Return type for the useChatSessionList hook
 */
export interface UseChatSessionListReturn {
  /** All loaded sessions */
  sessions: ChatSessionIndexEntry[];
  
  /** Filtered sessions based on search */
  filteredSessions: ChatSessionIndexEntry[];
  
  /** Sessions grouped by date */
  groupedSessions: GroupedSessions;
  
  /** Session groups with metadata */
  sessionGroups: SessionGroupMeta[];
  
  /** Current search query */
  searchQuery: string;
  
  /** Whether initial load is in progress */
  isLoading: boolean;
  
  /** Whether pagination load is in progress */
  isPaginationLoading: boolean;
  
  /** Error state */
  error: Error | null;
  
  /** Whether more sessions are available */
  hasMore: boolean;
  
  /** Total count of sessions on server */
  totalCount: number;
  
  /** Currently selected session ID */
  currentSessionId: string | null;
  
  /** Load more sessions (pagination) */
  loadMore: () => void;
  
  /** Select and resume a session */
  selectSession: (sessionId: string) => void;
  
  /** Delete a session with optimistic update */
  deleteSession: (sessionId: string) => Promise<void>;
  
  /** Search/filter sessions locally */
  searchSessions: (query: string) => void;
  
  /** Refresh sessions from server */
  refresh: () => void;
}

/**
 * React hook for managing chat session list
 * Provides comprehensive session management with pagination and search
 */
export function useChatSessionList(options: UseChatSessionListOptions = {}): UseChatSessionListReturn {
  const {
    pageSize = 50,
    autoLoad = true,
    searchDebounceMs = 300,
    maxCachedSessions = 500
  } = options;
  
  const client = useRealtimeClientSafe();
  
  // State
  const [sessions, setSessions] = useState<ChatSessionIndexEntry[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isPaginationLoading, setIsPaginationLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [hasMore, setHasMore] = useState(true);
  const [totalCount, setTotalCount] = useState(0);
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  
  // Refs for tracking state
  const offsetRef = useRef(0);
  const isLoadingRef = useRef(false);
  const deletedSessionsRef = useRef<Set<string>>(new Set());
  const rollbackDataRef = useRef<ChatSessionIndexEntry | null>(null);
  
  /**
   * Filter sessions based on search query
   */
  const filterSessions = useCallback((query: string, sessionList: ChatSessionIndexEntry[]) => {
    if (!query.trim()) {
      return sessionList;
    }
    
    const lowerQuery = query.toLowerCase();
    return sessionList.filter(session => {
      const name = (session.session_name || 'Untitled Session').toLowerCase();
      const agentName = (session.agent_name || '').toLowerCase();
      return name.includes(lowerQuery) || agentName.includes(lowerQuery);
    });
  }, []);
  
  /**
   * Filtered sessions - automatically recalculated when sessions or searchQuery changes
   */
  const filteredSessions = useMemo(
    () => filterSessions(searchQuery, sessions),
    [searchQuery, sessions, filterSessions]
  );
  
  /**
   * Group filtered sessions by date with memoization
   */
  const groupedSessions = useMemo(
    () => groupSessionsByTime(filteredSessions),
    [filteredSessions]
  );
  
  /**
   * Create session groups with metadata for UI
   */
  const sessionGroups = useMemo((): SessionGroupMeta[] => {
    const groups: SessionGroupMeta[] = [];
    
    if (groupedSessions.today.length > 0) {
      groups.push({
        group: 'today',
        label: 'Today',
        count: groupedSessions.today.length,
        sessions: groupedSessions.today
      });
    }
    
    if (groupedSessions.recent.length > 0) {
      groups.push({
        group: 'recent',
        label: 'Recent',
        count: groupedSessions.recent.length,
        sessions: groupedSessions.recent
      });
    }
    
    if (groupedSessions.past.length > 0) {
      groups.push({
        group: 'past',
        label: 'Past Sessions',
        count: groupedSessions.past.length,
        sessions: groupedSessions.past
      });
    }
    
    return groups;
  }, [groupedSessions]);
  
  /**
   * Load sessions from server
   */
  const loadSessions = useCallback((isInitial: boolean = false) => {
    if (!client || !client.isConnected()) {
      setError(new Error('Not connected to server'));
      return;
    }
    
    if (isLoadingRef.current) {
      console.debug('Load already in progress, skipping');
      return;
    }
    
    isLoadingRef.current = true;
    
    if (isInitial) {
      setIsLoading(true);
      offsetRef.current = 0;
    } else {
      setIsPaginationLoading(true);
    }
    
    setError(null);
    
    // Request sessions from server using sendEvent
    console.debug('Sending get_user_sessions event', { offset: offsetRef.current, limit: pageSize });
    client.sendEvent({
      type: 'get_user_sessions',
      offset: offsetRef.current,
      limit: pageSize
    });
  }, [client, pageSize]);
  
  /**
   * Load more sessions (pagination)
   */
  const loadMore = useCallback(() => {
    // Check current state values directly
    if (!client || !client.isConnected()) {
      console.debug('Cannot load more: client not connected');
      return;
    }
    
    if (isLoadingRef.current) {
      console.debug('Cannot load more: already loading');
      return;
    }
    
    // Check hasMore by comparing offset with total
    if (offsetRef.current >= totalCount && totalCount > 0) {
      console.debug('Cannot load more: no more sessions', { offset: offsetRef.current, total: totalCount });
      return;
    }
    
    console.debug('Loading more sessions...', { offset: offsetRef.current, total: totalCount });
    loadSessions(false);
  }, [client, totalCount, loadSessions]);
  
  /**
   * Select and resume a session
   */
  const selectSession = useCallback((sessionId: string) => {
    if (!client || !client.isConnected()) {
      setError(new Error('Not connected to server'));
      return;
    }
    
    setError(null);
    
    try {
      client.resumeChatSession(sessionId);
      setCurrentSessionId(sessionId);
    } catch (err) {
      const error = err instanceof Error ? err : new Error('Failed to select session');
      setError(error);
      console.error('Failed to select session:', err);
    }
  }, [client]);
  
  /**
   * Delete a session with optimistic update
   */
  const deleteSession = useCallback(async (sessionId: string): Promise<void> => {
    if (!client || !client.isConnected()) {
      throw new Error('Not connected to server');
    }
    
    // Find the session for potential rollback
    const sessionToDelete = sessions.find(s => s.session_id === sessionId);
    if (!sessionToDelete) {
      throw new Error('Session not found');
    }
    
    // Optimistic update - remove from UI immediately
    rollbackDataRef.current = sessionToDelete;
    setSessions(prev => prev.filter(s => s.session_id !== sessionId));
    // filteredSessions will update automatically via useMemo
    setTotalCount(prev => Math.max(0, prev - 1));
    
    // Mark as deleted to prevent re-adding from events
    deletedSessionsRef.current.add(sessionId);
    
    try {
      // Send delete event to server
      client.sendEvent({
        type: 'delete_chat_session',
        session_id: sessionId
      });
      
      // Clear current session if it was deleted
      if (currentSessionId === sessionId) {
        setCurrentSessionId(null);
      }
    } catch (err) {
      // Rollback on error
      if (rollbackDataRef.current) {
        setSessions(prev => {
          const newSessions = [...prev, rollbackDataRef.current!];
          // Sort by updated_at descending
          return newSessions.sort((a, b) => {
            const dateA = parseDate(a.updated_at || a.created_at).getTime();
            const dateB = parseDate(b.updated_at || b.created_at).getTime();
            return dateB - dateA;
          });
        });
        // filteredSessions will update automatically via useMemo
        
        setTotalCount(prev => prev + 1);
        deletedSessionsRef.current.delete(sessionId);
      }
      
      const error = err instanceof Error ? err : new Error('Failed to delete session');
      setError(error);
      throw error;
    } finally {
      rollbackDataRef.current = null;
    }
  }, [client, sessions, currentSessionId, searchQuery, filterSessions]);
  
  /**
   * Debounced search handler that uses current sessions state
   */
  const debouncedSearchTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  
  const searchSessions = useCallback((query: string) => {
    setSearchQuery(query);
    
    // Clear any existing timeout
    if (debouncedSearchTimeoutRef.current) {
      clearTimeout(debouncedSearchTimeoutRef.current);
    }
    
    // filteredSessions will update automatically via useMemo when searchQuery changes
    // No need for manual updates or debouncing logic here
  }, [filterSessions, searchDebounceMs]);
  
  /**
   * Refresh sessions from server
   */
  const refresh = useCallback(() => {
    setSessions([]);
    // filteredSessions will update automatically via useMemo
    offsetRef.current = 0;
    deletedSessionsRef.current.clear();
    setHasMore(true);
    loadSessions(true);
  }, [loadSessions]);
  
  // Handle server events
  useEffect(() => {
    if (!client) return;
    
    // Handle get_user_sessions_response
    const handleSessionsResponse = (event: GetUserSessionsResponseEvent) => {
      console.debug('Received get_user_sessions_response', { 
        hasData: !!event.sessions, 
        currentOffset: offsetRef.current 
      });
      
      if (!event.sessions) {
        console.warn('No sessions data in response');
        isLoadingRef.current = false;
        setIsLoading(false);
        setIsPaginationLoading(false);
        return;
      }
      
      const response = event.sessions;
      const newSessions = response.chat_sessions.filter(
        session => !deletedSessionsRef.current.has(session.session_id)
      );
      
      console.debug('Processing sessions response', {
        received: response.chat_sessions.length,
        filtered: newSessions.length,
        total: response.total_sessions,
        currentOffset: offsetRef.current
      });
      
      if (offsetRef.current === 0) {
        // Initial load - replace all sessions
        setSessions(newSessions);
        // filteredSessions will update automatically via useMemo
      } else {
        // Pagination - append new sessions
        setSessions(prev => {
          const combined = [...prev, ...newSessions];
          const limited = combined.length > maxCachedSessions 
            ? combined.slice(0, maxCachedSessions)
            : combined;
          
          // filteredSessions will update automatically via useMemo
          return limited;
        });
      }
      
      // Update pagination state
      const newOffset = offsetRef.current + newSessions.length;
      offsetRef.current = newOffset;
      setTotalCount(response.total_sessions);
      
      // Calculate hasMore - true if we haven't loaded all sessions yet
      const hasMoreSessions = newOffset < response.total_sessions;
      setHasMore(hasMoreSessions);
      
      console.debug('Updated pagination state', {
        newOffset,
        totalSessions: response.total_sessions,
        hasMore: hasMoreSessions
      });
      
      // Reset loading states
      isLoadingRef.current = false;
      setIsLoading(false);
      setIsPaginationLoading(false);
      setError(null);
    };
    
    // Handle chat_session_changed - Update the session in the list with new data
    const handleSessionChanged = (event: ChatSessionChangedEvent) => {
      if (!event.chat_session) return;
      
      const updatedSession = event.chat_session;
      const sessionId: string = updatedSession.session_id;
      
      // ALWAYS update current session ID first
      // This removes highlighting from the old session
      // If the new session isn't in the list, no session will be highlighted (correct behavior)
      setCurrentSessionId(sessionId);
      
      // Update the session in our list with the new data
      // Use functional update to avoid stale closure
      setSessions(prev => {
        // Check if session exists using current state
        const sessionIndex = prev.findIndex(s => s.session_id === sessionId);
        
        if (sessionIndex === -1) {
          // Session not in list yet (probably a new session without messages)
          // currentSessionId is updated, so old session loses highlight
          // No session will be highlighted until this session appears in the list
          return prev;
        }
        
        // Update existing session and re-sort by updated_at
        const updated = [...prev];
        updated[sessionIndex] = {
          ...updated[sessionIndex],
          session_name: updatedSession.session_name,
          updated_at: updatedSession.updated_at,
          agent_key: updatedSession.agent_config?.key,
          agent_name: updatedSession.agent_config?.name
        } as ChatSessionIndexEntry;
        
        // Re-sort by updated_at descending (newest first)
        return updated.sort((a, b) => {
          const dateA = parseDate(a.updated_at || a.created_at).getTime();
          const dateB = parseDate(b.updated_at || b.created_at).getTime();
          return dateB - dateA;
        });
      });
      // filteredSessions will update automatically via useMemo
    };
    
    // Handle chat_session_name_changed
    const handleSessionNameChanged = (event: ChatSessionNameChangedEvent) => {
      if (!event.session_name) return;
      
      // Use session_id from event if provided, otherwise use current session
      const sessionId = event.session_id || currentSessionId;
      if (!sessionId) return;
      
      // Update session name in our list
      setSessions(prev => prev.map(session => {
        if (session.session_id === sessionId) {
          const updated: ChatSessionIndexEntry = { 
            ...session, 
            session_name: event.session_name,
            // Update timestamp since the session was modified
            updated_at: new Date().toISOString()
          };
          return updated;
        }
        return session;
      }));
      // filteredSessions will update automatically via useMemo
    };
    
    // Handle chat_session_added - handles both new sessions AND updates to existing sessions
    const handleSessionAdded = (event: ChatSessionAddedEvent) => {
      if (!event.chat_session) return;
      
      const sessionData = event.chat_session;
      
      // Don't process if it was recently deleted
      if (deletedSessionsRef.current.has(sessionData.session_id)) {
        return;
      }
      
      setSessions(prev => {
        const existingIndex = prev.findIndex(s => s.session_id === sessionData.session_id);
        
        if (existingIndex !== -1) {
          // UPDATE existing session
          const updated = [...prev];
          const existingSession = updated[existingIndex];
          if (existingSession) {
            updated[existingIndex] = {
              ...existingSession,
              ...sessionData,  // Merge all fields from the event
              updated_at: sessionData.updated_at || existingSession.updated_at
            };
          }
          
          // Re-sort by updated_at descending (newest first)
          return updated.sort((a, b) => {
            const dateA = parseDate(a.updated_at || a.created_at).getTime();
            const dateB = parseDate(b.updated_at || b.created_at).getTime();
            return dateB - dateA;
          });
        } else {
          // ADD new session at the beginning
          const updated = [sessionData, ...prev];
          
          // Limit cache size
          if (updated.length > maxCachedSessions) {
            return updated.slice(0, maxCachedSessions);
          }
          return updated;
        }
      });
      // filteredSessions will update automatically via useMemo
      
      // Only increment total count for new sessions
      setSessions(prev => {
        const exists = prev.some(s => s.session_id === sessionData.session_id);
        if (!exists) {
          setTotalCount(count => count + 1);
        }
        return prev;
      });
      
      // Check SessionManager directly to see if this is the current session
      // This avoids stale closure issues with currentSessionId state
      const sessionManager = client?.getSessionManager();
      const currentSession = sessionManager?.getCurrentSession();
      if (currentSession && currentSession.session_id === sessionData.session_id) {
        // Force state update to trigger re-render with highlighting
        setCurrentSessionId(sessionData.session_id);
      }
    };
    
    // Handle chat_session_deleted
    const handleSessionDeleted = (event: ChatSessionDeletedEvent) => {
      const sessionId = event.session_id || currentSessionId;
      if (!sessionId) return;
      
      // Remove from our lists
      setSessions(prev => prev.filter(s => s.session_id !== sessionId));
      // filteredSessions will update automatically via useMemo
      setTotalCount(prev => Math.max(0, prev - 1));
      
      // Mark as deleted
      deletedSessionsRef.current.add(sessionId);
      
      // Clear current session if it was deleted
      if (currentSessionId === sessionId) {
        setCurrentSessionId(null);
      }
    };
    
    // Handle connection events
    const handleConnected = () => {
      // Load sessions when connected if autoLoad is enabled
      if (autoLoad && sessions.length === 0) {
        loadSessions(true);
      }
    };
    
    const handleDisconnected = () => {
      isLoadingRef.current = false;
      setIsLoading(false);
      setIsPaginationLoading(false);
    };
    
    // Handle reconnection - refresh to sync state
    const handleReconnected = () => {
      // On reconnection, refresh the session list to ensure we're in sync
      if (autoLoad) {
        // Small delay to let the connection stabilize
        setTimeout(() => {
          refresh();
        }, 500);
      }
    };
    
    // Handle errors
    const handleError = (error: unknown) => {
      console.error('Session list error:', error);
      
      // Always reset loading states on error
      isLoadingRef.current = false;
      setIsLoading(false);
      setIsPaginationLoading(false);
      
      const err = error instanceof Error ? error : new Error(
        (error as { message?: string })?.message || 'Unknown error'
      );
      setError(err);
    };
    
    // Subscribe to events
    client.on('get_user_sessions_response', handleSessionsResponse);
    client.on('chat_session_changed', handleSessionChanged);
    client.on('chat_session_name_changed', handleSessionNameChanged);
    client.on('chat_session_added', handleSessionAdded);
    client.on('chat_session_deleted', handleSessionDeleted);
    client.on('connected', handleConnected);
    client.on('disconnected', handleDisconnected);
    client.on('reconnected', handleReconnected);
    client.on('error', handleError);
    
    // Cleanup
    return () => {
      client.off('get_user_sessions_response', handleSessionsResponse);
      client.off('chat_session_changed', handleSessionChanged);
      client.off('chat_session_name_changed', handleSessionNameChanged);
      client.off('chat_session_added', handleSessionAdded);
      client.off('chat_session_deleted', handleSessionDeleted);
      client.off('connected', handleConnected);
      client.off('disconnected', handleDisconnected);
      client.off('reconnected', handleReconnected);
      client.off('error', handleError);
    };
  }, [client, autoLoad, searchQuery, currentSessionId, filterSessions, maxCachedSessions, loadSessions, refresh]);
  
  // Initial load on mount if autoLoad is enabled
  useEffect(() => {
    if (autoLoad && client && client.isConnected() && sessions.length === 0) {
      loadSessions(true);
    }
  }, [autoLoad, client, sessions.length, loadSessions]);
  
  // Update current session from SessionManager using event-driven approach
  // This matches the pattern used by SessionNameDropdown for real-time session tracking
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    if (!sessionManager) return;
    
    // Set initial value from SessionManager
    const currentSession = sessionManager.getCurrentSession();
    if (currentSession) {
      setCurrentSessionId(currentSession.session_id);
    } else {
      setCurrentSessionId(null);
    }
    
    // Handler for session changes from ChatSessionManager
    // The EventStreamProcessor consumes chat_session_changed and the ChatSessionManager emits chat-session-changed
    const handleSessionManagerChange = (event: { previousChatSession?: any; currentChatSession?: any }) => {
      if (event.currentChatSession) {
        // Update currentSessionId when SessionManager's current session changes
        const newSessionId = event.currentChatSession.session_id;
        console.debug('Session changed via SessionManager event:', newSessionId);
        setCurrentSessionId(newSessionId);
      } else {
        // No current session
        setCurrentSessionId(null);
      }
    };
    
    // Subscribe to SessionManager's chat-session-changed event (hyphenated)
    // This is the proper event that fires when the current session changes
    sessionManager.on('chat-session-changed', handleSessionManagerChange);
    
    return () => {
      sessionManager.off('chat-session-changed', handleSessionManagerChange);
    };
  }, [client]);
  
  return {
    sessions,
    filteredSessions,
    groupedSessions,
    sessionGroups,
    searchQuery,
    isLoading,
    isPaginationLoading,
    error,
    hasMore,
    totalCount,
    currentSessionId,
    loadMore,
    selectSession,
    deleteSession,
    searchSessions,
    refresh
  };
}