/**
 * useChatSessionListOptimized - Performance-optimized React hook for managing chat session list
 * Provides interface for loading, searching, and managing chat sessions with improved performance
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
 * Memoized date parser with microseconds support
 * Handles formats like '2025-09-06T20:16:26.515250' or ISO 8601
 */
const parseDate = (() => {
  const cache = new Map<string, Date>();
  const MAX_CACHE_SIZE = 1000;
  
  return (dateString: string | null | undefined): Date => {
    if (!dateString) return new Date(0);
    
    // Check cache first
    const cached = cache.get(dateString);
    if (cached) {
      return cached;
    }
    
    let date: Date;
    
    try {
      // Handle microseconds by truncating to milliseconds
      const microsecondPattern = /^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})\.(\d{6})$/;
      const match = dateString.match(microsecondPattern);
      
      if (match) {
        const [, dateTimePart, microseconds] = match;
        const milliseconds = microseconds ? microseconds.substring(0, 3) : '000';
        const normalizedDate = `${dateTimePart}.${milliseconds}Z`;
        date = new Date(normalizedDate);
      } else {
        date = new Date(dateString);
      }
      
      // Check if date is valid
      if (isNaN(date.getTime())) {
        console.warn(`Invalid date string: ${dateString}`);
        date = new Date(0);
      }
    } catch (error) {
      console.error(`Error parsing date: ${dateString}`, error);
      date = new Date(0);
    }
    
    // Manage cache size
    if (cache.size >= MAX_CACHE_SIZE) {
      const firstKey = cache.keys().next().value;
      if (firstKey !== undefined) {
        cache.delete(firstKey);
      }
    }
    
    cache.set(dateString, date);
    return date;
  };
})();

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
 * Optimized session grouping with caching
 */
const groupSessionsByTime = (() => {
  // Cache for grouping results
  const cache = new WeakMap<ChatSessionIndexEntry[], GroupedSessions>();
  
  return (sessions: ChatSessionIndexEntry[]): GroupedSessions => {
    // Check cache first
    if (cache.has(sessions)) {
      return cache.get(sessions)!;
    }
    
    const now = new Date();
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    const twoWeeksAgo = new Date(today.getTime() - (14 * 24 * 60 * 60 * 1000));
    const oneYearFromNow = new Date(now.getTime() + (365 * 24 * 60 * 60 * 1000));
    
    const groups: GroupedSessions = {
      today: [],
      recent: [],
      past: []
    };
    
    // Single pass through sessions
    for (let i = 0; i < sessions.length; i++) {
      const session = sessions[i];
      if (!session) continue; // Skip undefined entries
      
      const sessionDate = parseDate(session.updated_at || session.created_at);
      
      // Categorize session
      if (sessionDate > oneYearFromNow) {
        // Future date - treat as recent
        groups.recent.push(session);
      } else if (sessionDate >= today) {
        groups.today.push(session);
      } else if (sessionDate >= twoWeeksAgo) {
        groups.recent.push(session);
      } else {
        groups.past.push(session);
      }
    }
    
    // Cache the result
    cache.set(sessions, groups);
    
    return groups;
  };
})();

/**
 * Optimized search filter with memoization
 */
const createSearchFilter = () => {
  const cache = new Map<string, WeakMap<ChatSessionIndexEntry[], ChatSessionIndexEntry[]>>();
  const MAX_QUERY_CACHE = 50;
  
  return (query: string, sessionList: ChatSessionIndexEntry[]): ChatSessionIndexEntry[] => {
    const trimmedQuery = query.trim();
    
    if (!trimmedQuery) {
      return sessionList;
    }
    
    // Get or create cache for this query
    if (!cache.has(trimmedQuery)) {
      if (cache.size >= MAX_QUERY_CACHE) {
        // Remove oldest query from cache
        const firstKey = cache.keys().next().value;
        if (firstKey) {
          cache.delete(firstKey);
        }
      }
      cache.set(trimmedQuery, new WeakMap());
    }
    
    const queryCache = cache.get(trimmedQuery) || new WeakMap();
    
    // Check if we have cached result for this session list
    if (queryCache.has(sessionList)) {
      return queryCache.get(sessionList)!;
    }
    
    // Perform filtering
    const lowerQuery = trimmedQuery.toLowerCase();
    const filtered = sessionList.filter(session => {
      const name = (session.session_name || 'Untitled Session').toLowerCase();
      const agentName = (session.agent_name || '').toLowerCase();
      return name.includes(lowerQuery) || agentName.includes(lowerQuery);
    });
    
    // Cache the result
    queryCache.set(sessionList, filtered);
    
    return filtered;
  };
};

/**
 * Options for the useChatSessionListOptimized hook
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
  
  /** Enable aggressive caching (default: true) */
  enableCaching?: boolean;
}

/**
 * Return type for the useChatSessionListOptimized hook
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
 * Performance-optimized React hook for managing chat session list
 * Provides comprehensive session management with pagination, search, and caching
 */
export function useChatSessionListOptimized(options: UseChatSessionListOptions = {}): UseChatSessionListReturn {
  const {
    pageSize = 50,
    autoLoad = true,
    searchDebounceMs = 300,
    maxCachedSessions = 500,
    enableCaching = true
  } = options;
  
  const client = useRealtimeClientSafe();
  
  // State with functional updates for better batching
  const [state, setState] = useState(() => ({
    sessions: [] as ChatSessionIndexEntry[],
    filteredSessions: [] as ChatSessionIndexEntry[],
    searchQuery: '',
    isLoading: false,
    isPaginationLoading: false,
    error: null as Error | null,
    hasMore: true,
    totalCount: 0,
    currentSessionId: null as string | null
  }));
  
  // Refs for tracking state without causing re-renders
  const offsetRef = useRef(0);
  const isLoadingRef = useRef(false);
  const deletedSessionsRef = useRef<Set<string>>(new Set());
  const rollbackDataRef = useRef<ChatSessionIndexEntry | null>(null);
  const searchFilterRef = useRef(createSearchFilter());
  const debouncedSearchTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  
  // Memoized filter function
  const filterSessions = useCallback((query: string, sessionList: ChatSessionIndexEntry[]) => {
    if (!enableCaching) {
      // Non-cached version for comparison
      const trimmedQuery = query.trim();
      if (!trimmedQuery) return sessionList;
      
      const lowerQuery = trimmedQuery.toLowerCase();
      return sessionList.filter(session => {
        const name = (session.session_name || 'Untitled Session').toLowerCase();
        const agentName = (session.agent_name || '').toLowerCase();
        return name.includes(lowerQuery) || agentName.includes(lowerQuery);
      });
    }
    
    return searchFilterRef.current(query, sessionList);
  }, [enableCaching]);
  
  // Memoized grouped sessions
  const groupedSessions = useMemo(
    () => groupSessionsByTime(state.filteredSessions),
    [state.filteredSessions]
  );
  
  // Memoized session groups with metadata
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
  
  // Optimized load sessions with batched state updates
  const loadSessions = useCallback((isInitial: boolean = false) => {
    if (!client || !client.isConnected()) {
      setState(prev => ({ ...prev, error: new Error('Not connected to server') }));
      return;
    }
    
    if (isLoadingRef.current) {
      return;
    }
    
    isLoadingRef.current = true;
    
    // Batch state updates
    setState(prev => ({
      ...prev,
      isLoading: isInitial ? true : prev.isLoading,
      isPaginationLoading: !isInitial ? true : prev.isPaginationLoading,
      error: null
    }));
    
    if (isInitial) {
      offsetRef.current = 0;
    }
    
    // Request sessions from server
    client.sendEvent({
      type: 'get_user_sessions',
      offset: offsetRef.current,
      limit: pageSize
    });
  }, [client, pageSize]);
  
  // Optimized load more
  const loadMore = useCallback(() => {
    if (!client || !client.isConnected()) {
      return;
    }
    
    if (isLoadingRef.current) {
      return;
    }
    
    // Check hasMore by comparing offset with total
    if (offsetRef.current >= state.totalCount && state.totalCount > 0) {
      return;
    }
    
    loadSessions(false);
  }, [client, state.totalCount, loadSessions]);
  
  // Optimized select session
  const selectSession = useCallback((sessionId: string) => {
    if (!client || !client.isConnected()) {
      setState(prev => ({ ...prev, error: new Error('Not connected to server') }));
      return;
    }
    
    try {
      client.resumeChatSession(sessionId);
      setState(prev => ({ ...prev, currentSessionId: sessionId, error: null }));
    } catch (err) {
      const error = err instanceof Error ? err : new Error('Failed to select session');
      setState(prev => ({ ...prev, error }));
      console.error('Failed to select session:', err);
    }
  }, [client]);
  
  // Optimized delete session with batched updates
  const deleteSession = useCallback(async (sessionId: string): Promise<void> => {
    if (!client || !client.isConnected()) {
      throw new Error('Not connected to server');
    }
    
    // Find the session for potential rollback
    const sessionToDelete = state.sessions.find(s => s.session_id === sessionId);
    if (!sessionToDelete) {
      throw new Error('Session not found');
    }
    
    // Optimistic update with single state change
    rollbackDataRef.current = sessionToDelete;
    deletedSessionsRef.current.add(sessionId);
    
    setState(prev => {
      const newSessions = prev.sessions.filter(s => s.session_id !== sessionId);
      const newFiltered = prev.filteredSessions.filter(s => s.session_id !== sessionId);
      
      return {
        ...prev,
        sessions: newSessions,
        filteredSessions: newFiltered,
        totalCount: Math.max(0, prev.totalCount - 1),
        currentSessionId: prev.currentSessionId === sessionId ? null : prev.currentSessionId
      };
    });
    
    try {
      // Send delete event to server
      client.sendEvent({
        type: 'delete_chat_session',
        session_id: sessionId
      });
    } catch (err) {
      // Rollback on error
      if (rollbackDataRef.current) {
        setState(prev => {
          const newSessions = [...prev.sessions, rollbackDataRef.current!];
          // Sort by updated_at descending
          newSessions.sort((a, b) => {
            const dateA = parseDate(a.updated_at || a.created_at).getTime();
            const dateB = parseDate(b.updated_at || b.created_at).getTime();
            return dateB - dateA;
          });
          
          // Re-apply filter
          const newFiltered = filterSessions(prev.searchQuery, newSessions);
          
          return {
            ...prev,
            sessions: newSessions,
            filteredSessions: newFiltered,
            totalCount: prev.totalCount + 1,
            error: err instanceof Error ? err : new Error('Failed to delete session')
          };
        });
        
        deletedSessionsRef.current.delete(sessionId);
      }
      
      throw err instanceof Error ? err : new Error('Failed to delete session');
    } finally {
      rollbackDataRef.current = null;
    }
  }, [client, state.sessions, filterSessions]);
  
  // Optimized search with debouncing
  const searchSessions = useCallback((query: string) => {
    // Update search query immediately for UI responsiveness
    setState(prev => ({ ...prev, searchQuery: query }));
    
    // Clear any existing timeout
    if (debouncedSearchTimeoutRef.current) {
      clearTimeout(debouncedSearchTimeoutRef.current);
    }
    
    // If clearing search, update immediately
    if (!query.trim()) {
      setState(prev => ({
        ...prev,
        filteredSessions: prev.sessions
      }));
      return;
    }
    
    // Debounce the actual filtering
    debouncedSearchTimeoutRef.current = setTimeout(() => {
      setState(prev => ({
        ...prev,
        filteredSessions: filterSessions(query, prev.sessions)
      }));
    }, searchDebounceMs);
  }, [filterSessions, searchDebounceMs]);
  
  // Optimized refresh
  const refresh = useCallback(() => {
    offsetRef.current = 0;
    deletedSessionsRef.current.clear();
    
    setState({
      sessions: [],
      filteredSessions: [],
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: true,
      totalCount: 0,
      currentSessionId: state.currentSessionId
    });
    
    loadSessions(true);
  }, [loadSessions, state.currentSessionId]);
  
  // Optimized event handlers with batched state updates
  useEffect(() => {
    if (!client) return;
    
    // Handle get_user_sessions_response
    const handleSessionsResponse = (event: GetUserSessionsResponseEvent) => {
      if (!event.sessions) {
        isLoadingRef.current = false;
        setState(prev => ({
          ...prev,
          isLoading: false,
          isPaginationLoading: false
        }));
        return;
      }
      
      const response = event.sessions;
      const newSessions = response.chat_sessions.filter(
        session => !deletedSessionsRef.current.has(session.session_id)
      );
      
      // Batch all state updates
      setState(prev => {
        let sessions: ChatSessionIndexEntry[];
        
        if (offsetRef.current === 0) {
          // Initial load
          sessions = newSessions;
        } else {
          // Pagination
          const combined = [...prev.sessions, ...newSessions];
          sessions = combined.length > maxCachedSessions 
            ? combined.slice(0, maxCachedSessions)
            : combined;
        }
        
        const filtered = filterSessions(prev.searchQuery, sessions);
        
        // Update offset
        const newOffset = offsetRef.current + newSessions.length;
        offsetRef.current = newOffset;
        
        return {
          ...prev,
          sessions,
          filteredSessions: filtered,
          totalCount: response.total_sessions,
          hasMore: newOffset < response.total_sessions,
          isLoading: false,
          isPaginationLoading: false,
          error: null
        };
      });
      
      isLoadingRef.current = false;
    };
    
    // Handle chat_session_changed
    const handleSessionChanged = (event: ChatSessionChangedEvent) => {
      if (!event.chat_session) return;
      
      const updatedSession = event.chat_session;
      const sessionId = updatedSession.session_id;
      
      setState(prev => {
        const sessionIndex = prev.sessions.findIndex(s => s.session_id === sessionId);
        let newSessions: ChatSessionIndexEntry[];
        
        if (sessionIndex === -1) {
          // New session
          const newEntry: ChatSessionIndexEntry = {
            session_id: sessionId,
            session_name: updatedSession.session_name,
            created_at: updatedSession.created_at,
            updated_at: updatedSession.updated_at,
            user_id: updatedSession.user_id,
            agent_key: updatedSession.agent_config?.key,
            agent_name: updatedSession.agent_config?.name
          };
          newSessions = [newEntry, ...prev.sessions];
        } else {
          // Update existing
          newSessions = [...prev.sessions];
          newSessions[sessionIndex] = {
            ...newSessions[sessionIndex],
            session_name: updatedSession.session_name || null,
            updated_at: updatedSession.updated_at || null
          } as ChatSessionIndexEntry;
          
          // Re-sort
          newSessions.sort((a, b) => {
            const dateA = parseDate(a.updated_at || a.created_at).getTime();
            const dateB = parseDate(b.updated_at || b.created_at).getTime();
            return dateB - dateA;
          });
        }
        
        const filtered = filterSessions(prev.searchQuery, newSessions);
        
        return {
          ...prev,
          sessions: newSessions,
          filteredSessions: filtered,
          currentSessionId: sessionId
        };
      });
    };
    
    // Other event handlers remain similar but use batched setState
    const handleSessionNameChanged = (event: ChatSessionNameChangedEvent) => {
      if (!event.session_name) return;
      
      const sessionId = event.session_id || state.currentSessionId;
      if (!sessionId) return;
      
      setState(prev => {
        const updatedSessions = prev.sessions.map(session => 
          session.session_id === sessionId
            ? { ...session, session_name: event.session_name || null, updated_at: new Date().toISOString() } as ChatSessionIndexEntry
            : session
        );
        
        const updatedFiltered = prev.filteredSessions.map(session =>
          session.session_id === sessionId
            ? { ...session, session_name: event.session_name || null, updated_at: new Date().toISOString() } as ChatSessionIndexEntry
            : session
        );
        
        return {
          ...prev,
          sessions: updatedSessions,
          filteredSessions: updatedFiltered
        };
      });
    };
    
    const handleSessionAdded = (event: ChatSessionAddedEvent) => {
      if (!event.chat_session) return;
      
      const newSession = event.chat_session;
      
      if (deletedSessionsRef.current.has(newSession.session_id)) {
        return;
      }
      
      setState(prev => {
        if (prev.sessions.some(s => s.session_id === newSession.session_id)) {
          return prev;
        }
        
        const newSessions = [newSession, ...prev.sessions];
        const limitedSessions = newSessions.length > maxCachedSessions
          ? newSessions.slice(0, maxCachedSessions)
          : newSessions;
        
        const filtered = filterSessions(prev.searchQuery, limitedSessions);
        
        return {
          ...prev,
          sessions: limitedSessions,
          filteredSessions: filtered,
          totalCount: prev.totalCount + 1
        };
      });
    };
    
    const handleSessionDeleted = (event: ChatSessionDeletedEvent) => {
      const sessionId = event.session_id || state.currentSessionId;
      if (!sessionId) return;
      
      deletedSessionsRef.current.add(sessionId);
      
      setState(prev => ({
        ...prev,
        sessions: prev.sessions.filter(s => s.session_id !== sessionId),
        filteredSessions: prev.filteredSessions.filter(s => s.session_id !== sessionId),
        totalCount: Math.max(0, prev.totalCount - 1),
        currentSessionId: prev.currentSessionId === sessionId ? null : prev.currentSessionId
      }));
    };
    
    const handleConnected = () => {
      if (autoLoad && state.sessions.length === 0) {
        loadSessions(true);
      }
    };
    
    const handleDisconnected = () => {
      isLoadingRef.current = false;
      setState(prev => ({
        ...prev,
        isLoading: false,
        isPaginationLoading: false
      }));
    };
    
    const handleReconnected = () => {
      if (autoLoad) {
        setTimeout(() => refresh(), 500);
      }
    };
    
    const handleError = (error: unknown) => {
      console.error('Session list error:', error);
      
      isLoadingRef.current = false;
      setState(prev => ({
        ...prev,
        isLoading: false,
        isPaginationLoading: false,
        error: error instanceof Error ? error : new Error('Unknown error')
      }));
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
  }, [client, autoLoad, state.currentSessionId, state.sessions.length, filterSessions, maxCachedSessions, loadSessions, refresh]);
  
  // Initial load on mount
  useEffect(() => {
    if (autoLoad && client && client.isConnected() && state.sessions.length === 0 && !isLoadingRef.current) {
      loadSessions(true);
    }
  }, [autoLoad, client, state.sessions.length, loadSessions]);
  
  // Update current session from SessionManager
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    if (sessionManager) {
      const currentSession = sessionManager.getCurrentSession();
      if (currentSession) {
        setState(prev => ({ ...prev, currentSessionId: currentSession.session_id }));
      }
    }
  }, [client]);
  
  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (debouncedSearchTimeoutRef.current) {
        clearTimeout(debouncedSearchTimeoutRef.current);
      }
    };
  }, []);
  
  return {
    sessions: state.sessions,
    filteredSessions: state.filteredSessions,
    groupedSessions,
    sessionGroups,
    searchQuery: state.searchQuery,
    isLoading: state.isLoading,
    isPaginationLoading: state.isPaginationLoading,
    error: state.error,
    hasMore: state.hasMore,
    totalCount: state.totalCount,
    currentSessionId: state.currentSessionId,
    loadMore,
    selectSession,
    deleteSession,
    searchSessions,
    refresh
  };
}