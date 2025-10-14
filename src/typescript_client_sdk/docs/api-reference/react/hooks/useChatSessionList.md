# useChatSessionList

## Overview

The `useChatSessionList` hook provides comprehensive session management capabilities for the Agent C Realtime SDK. It handles loading, searching, filtering, and organizing chat sessions with built-in pagination, real-time updates, and optimistic UI updates.

## Purpose

This hook serves as the primary interface for:
- Loading and paginating through user's chat sessions
- Searching and filtering sessions locally
- Grouping sessions by date (Today, Recent, Past)
- Managing session selection and deletion
- Handling real-time session updates from the server
- Providing optimistic updates for better UX

## Import Statement

```typescript
import { useChatSessionList } from '@agentc/realtime-react';
import type { 
  UseChatSessionListOptions,
  UseChatSessionListReturn,
  SessionGroup,
  GroupedSessions,
  SessionGroupMeta
} from '@agentc/realtime-react';
```

## TypeScript Types

### Options Interface

```typescript
interface UseChatSessionListOptions {
  /** Page size for pagination (default: 50) */
  pageSize?: number;
  
  /** Whether to load sessions on mount (default: true) */
  autoLoad?: boolean;
  
  /** Debounce delay for search in ms (default: 300) */
  searchDebounceMs?: number;
  
  /** Maximum number of sessions to cache (default: 500) */
  maxCachedSessions?: number;
}
```

### Return Interface

```typescript
interface UseChatSessionListReturn {
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
```

### Supporting Types

```typescript
type SessionGroup = 'today' | 'recent' | 'past';

interface GroupedSessions {
  today: ChatSessionIndexEntry[];      // Sessions from today
  recent: ChatSessionIndexEntry[];     // Past 14 days
  past: ChatSessionIndexEntry[];       // Older than 14 days
}

interface SessionGroupMeta {
  group: SessionGroup;
  label: string;
  count: number;
  sessions: ChatSessionIndexEntry[];
}

interface ChatSessionIndexEntry {
  session_id: string;
  session_name: string | null;
  created_at: string | null;
  updated_at: string | null;
  user_id: string | null;
  agent_key?: string | null;
  agent_name?: string | null;
}
```

## Return Value Descriptions

### State Properties

- **`sessions`**: Complete list of loaded sessions from the server, up to `maxCachedSessions`.
- **`filteredSessions`**: Sessions filtered by the current search query.
- **`groupedSessions`**: Sessions organized into date-based groups (today, recent, past).
- **`sessionGroups`**: Array of group metadata for UI rendering with labels and counts.
- **`searchQuery`**: Current search string being used to filter sessions.
- **`isLoading`**: True during initial session load from server.
- **`isPaginationLoading`**: True when loading additional sessions via pagination.
- **`error`**: Error object if any operation fails, null otherwise.
- **`hasMore`**: Boolean indicating if more sessions can be loaded from server.
- **`totalCount`**: Total number of sessions available on the server.
- **`currentSessionId`**: ID of the currently active session.

### Methods

- **`loadMore()`**: Loads the next page of sessions from the server.
- **`selectSession(sessionId)`**: Switches to and resumes a specific session.
- **`deleteSession(sessionId)`**: Deletes a session with optimistic UI update and rollback on failure.
- **`searchSessions(query)`**: Filters sessions locally by name or agent name with debouncing.
- **`refresh()`**: Clears cache and reloads sessions from the server.

## Usage Examples

### Basic Session List

```typescript
import React from 'react';
import { useChatSessionList } from '@agentc/realtime-react';

function SessionList() {
  const {
    sessions,
    currentSessionId,
    selectSession,
    isLoading,
    error
  } = useChatSessionList();
  
  if (isLoading) return <div>Loading sessions...</div>;
  if (error) return <div>Error: {error.message}</div>;
  
  return (
    <ul>
      {sessions.map(session => (
        <li
          key={session.session_id}
          className={session.session_id === currentSessionId ? 'active' : ''}
          onClick={() => selectSession(session.session_id)}
        >
          <h4>{session.session_name || 'Untitled Session'}</h4>
          <p>Agent: {session.agent_name}</p>
          <time>{new Date(session.updated_at || session.created_at || 0).toLocaleDateString()}</time>
        </li>
      ))}
    </ul>
  );
}
```

### Grouped Sessions Display

```typescript
function GroupedSessionList() {
  const {
    sessionGroups,
    currentSessionId,
    selectSession,
    deleteSession
  } = useChatSessionList();
  
  return (
    <div className="session-list">
      {sessionGroups.map(group => (
        <div key={group.group} className="session-group">
          <h3>{group.label} ({group.count})</h3>
          <ul>
            {group.sessions.map(session => (
              <li
                key={session.session_id}
                className={session.session_id === currentSessionId ? 'active' : ''}
              >
                <div onClick={() => selectSession(session.session_id)}>
                  {session.session_name || 'Untitled'}
                </div>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    deleteSession(session.session_id);
                  }}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
}
```

### Search Implementation

```typescript
function SearchableSessions() {
  const {
    filteredSessions,
    searchQuery,
    searchSessions,
    isLoading
  } = useChatSessionList({ 
    searchDebounceMs: 500 
  });
  
  return (
    <div>
      <input
        type="search"
        placeholder="Search sessions..."
        value={searchQuery}
        onChange={(e) => searchSessions(e.target.value)}
      />
      
      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div>
          <p>Found {filteredSessions.length} sessions</p>
          <ul>
            {filteredSessions.map(session => (
              <li key={session.session_id}>
                {session.session_name || 'Untitled'}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
```

### Infinite Scroll Implementation

```typescript
function InfiniteSessionList() {
  const {
    filteredSessions,
    hasMore,
    isPaginationLoading,
    loadMore,
    totalCount
  } = useChatSessionList({ 
    pageSize: 20,
    autoLoad: true 
  });
  
  const observerRef = React.useRef<IntersectionObserver>();
  const loadMoreRef = React.useCallback((node: HTMLDivElement | null) => {
    if (isPaginationLoading) return;
    
    if (observerRef.current) observerRef.current.disconnect();
    
    observerRef.current = new IntersectionObserver(entries => {
      if (entries[0]?.isIntersecting && hasMore) {
        loadMore();
      }
    });
    
    if (node) observerRef.current.observe(node);
  }, [isPaginationLoading, hasMore, loadMore]);
  
  return (
    <div>
      <p>Showing {filteredSessions.length} of {totalCount} sessions</p>
      
      {filteredSessions.map(session => (
        <div key={session.session_id}>
          {session.session_name}
        </div>
      ))}
      
      {hasMore && (
        <div ref={loadMoreRef}>
          {isPaginationLoading ? 'Loading more...' : 'Scroll for more'}
        </div>
      )}
    </div>
  );
}
```

## Message Handling Patterns

### Session Loading Flow

1. **Initial Load**:
   - Hook sends `get_user_sessions` event on mount (if `autoLoad` is true)
   - Server responds with `get_user_sessions_response`
   - Sessions are sorted by `updated_at` descending
   - Sessions are grouped into date categories
   - `isLoading` becomes false

2. **Pagination**:
   - User triggers `loadMore()`
   - Hook sends `get_user_sessions` with offset
   - New sessions are appended to existing list
   - Cache limit is enforced (`maxCachedSessions`)
   - `hasMore` updates based on server response

3. **Real-time Updates**:
   - Server sends events for session changes
   - Hook updates local state immediately
   - Sessions re-sort if timestamps change
   - Groups are recalculated automatically

### Optimistic Updates

The hook implements optimistic updates for better UX:

```typescript
function OptimisticDeleteExample() {
  const { deleteSession, sessions, error } = useChatSessionList();
  
  const handleDelete = async (sessionId: string) => {
    try {
      // Session disappears immediately from UI
      await deleteSession(sessionId);
      console.log('Session deleted successfully');
    } catch (err) {
      // Session reappears if delete fails
      console.error('Delete failed, session restored:', err);
      // The error is also available in the hook's error state
    }
  };
  
  return (
    <div>
      {error && <div className="error">Operation failed: {error.message}</div>}
      {sessions.map(session => (
        <div key={session.session_id}>
          <span>{session.session_name}</span>
          <button onClick={() => handleDelete(session.session_id)}>
            Delete
          </button>
        </div>
      ))}
    </div>
  );
}
```

## Session Management Strategies

### Auto-refresh on Reconnection

```typescript
function ResilientSessionList() {
  const {
    sessions,
    refresh,
    error,
    isLoading
  } = useChatSessionList({ autoLoad: true });
  
  const { connectionState } = useConnection();
  
  React.useEffect(() => {
    // Hook automatically refreshes on reconnection
    // But you can also trigger manual refresh
    if (connectionState === 'reconnected') {
      console.log('Connection restored, sessions will refresh');
    }
  }, [connectionState]);
  
  return (
    <div>
      <button onClick={refresh} disabled={isLoading}>
        Refresh Sessions
      </button>
      {/* Session list UI */}
    </div>
  );
}
```

### Session Metadata Display

```typescript
function DetailedSessionList() {
  const { 
    groupedSessions,
    currentSessionId 
  } = useChatSessionList();
  
  const formatDate = (dateStr: string | null) => {
    if (!dateStr) return 'Unknown';
    const date = new Date(dateStr);
    return new Intl.RelativeTimeFormat('en').format(
      Math.ceil((date.getTime() - Date.now()) / (1000 * 60 * 60 * 24)),
      'day'
    );
  };
  
  return (
    <div>
      <section>
        <h3>Today ({groupedSessions.today.length})</h3>
        {groupedSessions.today.map(session => (
          <div key={session.session_id}>
            <strong>{session.session_name}</strong>
            <span>{session.agent_name}</span>
            <time>{formatDate(session.updated_at)}</time>
          </div>
        ))}
      </section>
      
      <section>
        <h3>Recent ({groupedSessions.recent.length})</h3>
        {groupedSessions.recent.map(session => (
          <div key={session.session_id}>
            {session.session_name}
          </div>
        ))}
      </section>
      
      <section>
        <h3>Older ({groupedSessions.past.length})</h3>
        {groupedSessions.past.map(session => (
          <div key={session.session_id}>
            {session.session_name}
          </div>
        ))}
      </section>
    </div>
  );
}
```

### Custom Filtering

```typescript
function AdvancedFiltering() {
  const {
    sessions,
    searchSessions,
    searchQuery
  } = useChatSessionList();
  
  // Additional client-side filtering
  const [agentFilter, setAgentFilter] = React.useState('');
  
  const filteredByAgent = React.useMemo(() => {
    if (!agentFilter) return sessions;
    return sessions.filter(s => 
      s.agent_name?.toLowerCase().includes(agentFilter.toLowerCase())
    );
  }, [sessions, agentFilter]);
  
  return (
    <div>
      <input
        placeholder="Search by name..."
        value={searchQuery}
        onChange={(e) => searchSessions(e.target.value)}
      />
      
      <select 
        value={agentFilter}
        onChange={(e) => setAgentFilter(e.target.value)}
      >
        <option value="">All Agents</option>
        <option value="assistant">Assistant</option>
        <option value="expert">Expert</option>
      </select>
      
      <div>
        Showing {filteredByAgent.length} sessions
        {agentFilter && ` with agent: ${agentFilter}`}
      </div>
    </div>
  );
}
```

## Performance Considerations

### Date Parsing Optimization

The hook includes optimized date parsing for handling microsecond timestamps:

```typescript
// Handles various date formats efficiently
// '2025-09-06T20:16:26.515250' -> Microseconds truncated to milliseconds
// Standard ISO 8601 -> Direct parsing
// Invalid dates -> Returns epoch (new Date(0))
```

### Debounced Search

Search operations are debounced to prevent excessive filtering:

```typescript
const { searchSessions } = useChatSessionList({ 
  searchDebounceMs: 300 // Adjust based on your needs
});

// Rapid typing won't cause performance issues
<input onChange={(e) => searchSessions(e.target.value)} />
```

### Memory Management

Sessions are automatically limited to prevent unbounded growth:

```typescript
const { sessions } = useChatSessionList({
  maxCachedSessions: 500 // Default limit
});

// Oldest sessions are removed when limit is exceeded
```

## StrictMode Compatibility

The hook is fully compatible with React StrictMode:

1. **Event Handler Cleanup**: All event listeners are properly removed on unmount
2. **Loading State Management**: Uses refs to prevent race conditions
3. **Debouncing Cleanup**: Timeouts are cleared on unmount
4. **Deleted Sessions Tracking**: Maintains consistency across re-renders

### StrictMode Example

```typescript
function StrictModeApp() {
  return (
    <React.StrictMode>
      <AgentCProvider config={config}>
        <SessionListApp />
      </AgentCProvider>
    </React.StrictMode>
  );
}

function SessionListApp() {
  const sessionList = useChatSessionList();
  
  // Hook handles double-mounting gracefully
  React.useEffect(() => {
    console.log('Sessions loaded:', sessionList.sessions.length);
    // No special handling needed for StrictMode
  }, [sessionList.sessions.length]);
  
  return <div>{/* UI */}</div>;
}
```

## Best Practices

### 1. Handle Loading States

```typescript
const SessionListWithStates = () => {
  const { isLoading, isPaginationLoading, error, sessions } = useChatSessionList();
  
  if (isLoading) return <Skeleton />;
  if (error) return <ErrorBoundary error={error} />;
  if (sessions.length === 0) return <EmptyState />;
  
  return (
    <div>
      <SessionList sessions={sessions} />
      {isPaginationLoading && <LoadingMore />}
    </div>
  );
};
```

### 2. Efficient Re-renders

```typescript
// Use specific properties to prevent unnecessary re-renders
const SessionCount = () => {
  const { totalCount } = useChatSessionList();
  return <div>Total: {totalCount}</div>;
};

const CurrentSession = () => {
  const { currentSessionId } = useChatSessionList();
  return <div>Active: {currentSessionId}</div>;
};
```

### 3. Error Recovery

```typescript
const RobustSessionList = () => {
  const { error, refresh, sessions } = useChatSessionList();
  
  if (error) {
    return (
      <div>
        <p>Failed to load sessions: {error.message}</p>
        <button onClick={refresh}>Retry</button>
      </div>
    );
  }
  
  return <SessionList sessions={sessions} />;
};
```

### 4. Pagination Strategy

```typescript
// Manual load more button
const ManualPagination = () => {
  const { hasMore, loadMore, isPaginationLoading } = useChatSessionList();
  
  return (
    <button 
      onClick={loadMore}
      disabled={!hasMore || isPaginationLoading}
    >
      {isPaginationLoading ? 'Loading...' : hasMore ? 'Load More' : 'No More Sessions'}
    </button>
  );
};

// Auto-load on scroll to bottom
const AutoPagination = () => {
  const { hasMore, loadMore } = useChatSessionList();
  
  const handleScroll = (e: React.UIEvent) => {
    const bottom = e.currentTarget.scrollHeight - e.currentTarget.scrollTop === e.currentTarget.clientHeight;
    if (bottom && hasMore) {
      loadMore();
    }
  };
  
  return <div onScroll={handleScroll}>{/* Sessions */}</div>;
};
```

### 5. Search Optimization

```typescript
const OptimizedSearch = () => {
  const { searchSessions, filteredSessions } = useChatSessionList({
    searchDebounceMs: 500 // Longer delay for better performance
  });
  
  // Show loading state during debounce
  const [isSearching, setIsSearching] = React.useState(false);
  
  const handleSearch = (query: string) => {
    setIsSearching(true);
    searchSessions(query);
    setTimeout(() => setIsSearching(false), 500);
  };
  
  return (
    <div>
      <input onChange={(e) => handleSearch(e.target.value)} />
      {isSearching && <span>Searching...</span>}
      <div>Results: {filteredSessions.length}</div>
    </div>
  );
};
```

## Common Pitfalls

1. **Not Checking Connection**: Always verify connection before operations
2. **Ignoring Loading States**: Show appropriate UI during loads
3. **Unbounded Session Loading**: Use `maxCachedSessions` to limit memory
4. **Not Handling Errors**: Always provide error recovery options
5. **Excessive Re-renders**: Use specific properties instead of entire hook
6. **Search Performance**: Configure appropriate `searchDebounceMs`
7. **Stale Closures**: Be careful with callbacks in effects