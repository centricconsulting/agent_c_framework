# ChatSessionList Component Design Specification

## Component Overview

The ChatSessionList component displays a user's chat history in the sidebar, organized by time groups with rich session information. It replaces the placeholder stub in the ChatSidebar component and integrates with the authentication API's session data.

## Data Model

```typescript
interface ChatSession {
  session_id: string
  session_name: string | null
  created_at: string | null  // ISO timestamp
  updated_at: string | null  // ISO timestamp
  token_count: number
  messages: Message[]
  agent_config: AgentConfiguration
}

interface Message {
  id: string
  role: 'user' | 'assistant' | 'system'
  content: string
  timestamp: string
}
```

## Component Architecture

### Main Component Structure with Pagination

```tsx
const ChatSessionList: React.FC = () => {
  const { 
    sessions, 
    currentSessionId, 
    loadSession,
    hasMoreSessions,
    totalSessionCount,
    fetchMoreSessions,
    isInitialLoading,
    isPaginationLoading,
    paginationError
  } = useChatSessions()
  
  const [searchQuery, setSearchQuery] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const scrollContainerRef = useRef<HTMLDivElement>(null)
  const loadMoreTriggerRef = useRef<HTMLDivElement>(null)
  
  // Group sessions by time periods
  const groupedSessions = useMemo(() => 
    groupSessionsByTime(sessions, searchQuery),
    [sessions, searchQuery]
  )
  
  // Intersection Observer for infinite scrolling
  useEffect(() => {
    if (!loadMoreTriggerRef.current || !hasMoreSessions || isPaginationLoading) return
    
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting && hasMoreSessions && !isPaginationLoading) {
          fetchMoreSessions()
        }
      },
      {
        root: scrollContainerRef.current,
        rootMargin: '100px', // Start loading 100px before reaching bottom
        threshold: 0.1
      }
    )
    
    observer.observe(loadMoreTriggerRef.current)
    
    return () => observer.disconnect()
  }, [hasMoreSessions, isPaginationLoading, fetchMoreSessions])
  
  return (
    <div className="flex-1 flex flex-col min-h-0">
      {/* Session Count Indicator */}
      {totalSessionCount > 0 && (
        <div className="px-2 py-1 text-xs text-muted-foreground/70">
          Showing {sessions.length} of {totalSessionCount} conversations
        </div>
      )}
      
      {/* Search Bar */}
      <SessionSearch />
      
      {/* Scrollable Container */}
      <div 
        ref={scrollContainerRef}
        className="flex-1 overflow-y-auto overflow-x-hidden 
                   scrollbar-thin scrollbar-thumb-border 
                   scrollbar-track-transparent"
      >
        {/* Initial Loading State */}
        {isInitialLoading && <InitialLoadingState />}
        
        {/* Session Groups */}
        {!isInitialLoading && (
          <SessionGroups 
            groups={groupedSessions}
            currentSessionId={currentSessionId}
            onSessionClick={loadSession}
            isLoading={isLoading}
          />
        )}
        
        {/* Pagination Loading Indicator */}
        {isPaginationLoading && <PaginationLoadingIndicator />}
        
        {/* Load More Trigger (invisible) */}
        {hasMoreSessions && !isPaginationLoading && (
          <div ref={loadMoreTriggerRef} className="h-1" aria-hidden="true" />
        )}
        
        {/* Pagination Error State */}
        {paginationError && (
          <PaginationErrorState 
            onRetry={fetchMoreSessions}
            error={paginationError}
          />
        )}
        
        {/* No More Sessions Indicator */}
        {!hasMoreSessions && sessions.length > 0 && (
          <div className="py-4 text-center text-xs text-muted-foreground/50">
            You've reached the end of your conversations
          </div>
        )}
        
        {/* Empty State */}
        {!isInitialLoading && sessions.length === 0 && <EmptyState />}
      </div>
    </div>
  )
}
```

## Visual Design Specifications

### Container Layout

```css
/* Main Container */
className="flex-1 flex flex-col min-h-0 px-2"

/* Scrollable Area */
className="flex-1 overflow-y-auto overflow-x-hidden 
           scrollbar-thin scrollbar-thumb-border 
           scrollbar-track-transparent"
```

### Search Component (Optional Enhancement)

```tsx
<div className="sticky top-0 z-10 pb-2 mb-1 
                bg-gradient-to-b from-background via-background/95 to-transparent">
  <div className="relative">
    <Search className="absolute left-2.5 top-2.5 h-3.5 w-3.5 text-muted-foreground" />
    <input
      type="text"
      placeholder="Search conversations..."
      className="w-full h-8 pl-8 pr-2 text-xs rounded-md
                 bg-muted/50 border border-border/50
                 focus:bg-background focus:border-border
                 focus:outline-none focus:ring-1 focus:ring-ring/20
                 placeholder:text-muted-foreground/60
                 transition-colors duration-200"
    />
  </div>
</div>
```

### Time Group Headers

```tsx
const TimeGroupHeader: React.FC<{ label: string }> = ({ label }) => (
  <div className="sticky top-8 z-[5] flex items-center gap-2 
                  pt-3 pb-1.5 px-1
                  bg-gradient-to-b from-background via-background/90 to-transparent">
    <span className="text-[11px] font-medium text-muted-foreground/70 
                     uppercase tracking-wider">
      {label}
    </span>
    <div className="flex-1 h-px bg-border/30" />
  </div>
)

// Time group labels:
- "Today"
- "Yesterday" 
- "This Week"
- "This Month"
- "Older"
```

### Session Item Component

```tsx
const SessionItem: React.FC<SessionItemProps> = ({ 
  session, 
  isActive, 
  onClick,
  isLoading 
}) => {
  const sessionName = deriveSessionName(session)
  const preview = getMessagePreview(session.messages)
  const relativeTime = getRelativeTime(session.updated_at)
  const messageCount = session.messages.length
  
  return (
    <button
      onClick={() => onClick(session.session_id)}
      disabled={isLoading}
      className={cn(
        // Base styles
        "group relative w-full text-left rounded-lg px-2.5 py-2 mb-0.5",
        "transition-all duration-200 ease-out",
        
        // Hover state
        "hover:bg-muted/60 hover:shadow-sm",
        
        // Active state
        isActive && "bg-muted shadow-sm ring-1 ring-border/50",
        
        // Loading state
        isLoading && "opacity-60 cursor-wait",
        
        // Focus state
        "focus-visible:outline-none focus-visible:ring-1",
        "focus-visible:ring-ring focus-visible:ring-offset-1"
      )}
      aria-current={isActive ? "true" : undefined}
      aria-label={`Chat: ${sessionName}, ${relativeTime}, ${messageCount} messages`}
    >
      {/* Active Indicator */}
      {isActive && (
        <div className="absolute left-0 top-1/2 -translate-y-1/2 
                        w-0.5 h-5 bg-primary rounded-r-full" 
             aria-hidden="true" />
      )}
      
      {/* Content Container */}
      <div className="flex flex-col gap-0.5 min-w-0">
        {/* Session Name and Time */}
        <div className="flex items-baseline justify-between gap-2">
          <span className={cn(
            "text-sm font-medium truncate flex-1",
            isActive ? "text-foreground" : "text-foreground/90"
          )}>
            {sessionName}
          </span>
          <span className="text-[10px] text-muted-foreground/60 flex-shrink-0">
            {relativeTime}
          </span>
        </div>
        
        {/* Message Preview */}
        <p className={cn(
          "text-xs truncate",
          isActive ? "text-muted-foreground/80" : "text-muted-foreground/60"
        )}>
          {preview}
        </p>
        
        {/* Metadata Row */}
        <div className="flex items-center gap-3 mt-0.5">
          <span className="inline-flex items-center gap-1">
            <MessageSquare className="h-2.5 w-2.5 text-muted-foreground/40" />
            <span className="text-[10px] text-muted-foreground/50">
              {messageCount}
            </span>
          </span>
          <span className="inline-flex items-center gap-1">
            <Hash className="h-2.5 w-2.5 text-muted-foreground/40" />
            <span className="text-[10px] text-muted-foreground/50">
              {formatTokenCount(session.token_count)}
            </span>
          </span>
        </div>
      </div>
      
      {/* Hover Actions (Future Enhancement) */}
      <div className={cn(
        "absolute right-2 top-2 opacity-0 group-hover:opacity-100",
        "transition-opacity duration-200",
        "flex items-center gap-1"
      )}>
        <button
          className="p-1 rounded hover:bg-muted"
          onClick={(e) => {
            e.stopPropagation()
            // Archive action
          }}
          aria-label="Archive conversation"
        >
          <Archive className="h-3 w-3 text-muted-foreground" />
        </button>
      </div>
    </button>
  )
}
```

### Empty State

```tsx
const EmptyState: React.FC = () => (
  <div className="flex flex-col items-center justify-center py-8 px-4">
    <div className="rounded-full bg-muted/50 p-3 mb-3">
      <MessageSquare className="h-5 w-5 text-muted-foreground/50" />
    </div>
    <p className="text-sm font-medium text-foreground/80 mb-1">
      No conversations yet
    </p>
    <p className="text-xs text-muted-foreground/60 text-center">
      Start a new chat to begin your conversation history
    </p>
  </div>
)
```

### Loading States

```tsx
// Initial load skeleton (shows more items)
const InitialLoadingState: React.FC = () => (
  <div className="space-y-2 px-2 py-4">
    {[...Array(8)].map((_, i) => (
      <div key={i} className="animate-pulse">
        <div className="rounded-lg bg-muted/30 p-2.5">
          <div className="h-4 bg-muted/50 rounded w-3/4 mb-2" />
          <div className="h-3 bg-muted/40 rounded w-full mb-1" />
          <div className="flex gap-3">
            <div className="h-2 bg-muted/30 rounded w-12" />
            <div className="h-2 bg-muted/30 rounded w-16" />
          </div>
        </div>
      </div>
    ))}
  </div>
)

// Pagination loading indicator (bottom of list)
const PaginationLoadingIndicator: React.FC = () => (
  <div className="flex items-center justify-center py-4 px-2">
    <div className="flex items-center gap-2 text-sm text-muted-foreground">
      <div className="h-4 w-4 border-2 border-muted-foreground/30 
                      border-t-muted-foreground rounded-full animate-spin" />
      <span>Loading more conversations...</span>
    </div>
  </div>
)

// Pagination error state with retry
const PaginationErrorState: React.FC<{
  onRetry: () => void
  error: Error
}> = ({ onRetry, error }) => (
  <div className="flex flex-col items-center py-4 px-2 gap-2">
    <div className="flex items-center gap-2 text-sm text-destructive">
      <AlertCircle className="h-4 w-4" />
      <span>Failed to load more sessions</span>
    </div>
    <button
      onClick={onRetry}
      className="px-3 py-1 text-xs font-medium rounded-md
                 bg-primary text-primary-foreground
                 hover:bg-primary/90 transition-colors
                 focus-visible:outline-none focus-visible:ring-2
                 focus-visible:ring-ring focus-visible:ring-offset-2"
    >
      Try Again
    </button>
  </div>
)

// Manual load more button (fallback)
const LoadMoreButton: React.FC<{
  onClick: () => void
  isLoading: boolean
}> = ({ onClick, isLoading }) => (
  <div className="flex justify-center py-4">
    <button
      onClick={onClick}
      disabled={isLoading}
      className="px-4 py-2 text-sm font-medium rounded-md
                 border border-border bg-background
                 hover:bg-muted transition-colors
                 disabled:opacity-50 disabled:cursor-not-allowed
                 focus-visible:outline-none focus-visible:ring-2
                 focus-visible:ring-ring focus-visible:ring-offset-2"
    >
      {isLoading ? (
        <span className="flex items-center gap-2">
          <div className="h-3 w-3 border-2 border-muted-foreground/30 
                          border-t-muted-foreground rounded-full animate-spin" />
          Loading...
        </span>
      ) : (
        'Load More Conversations'
      )}
    </button>
  </div>
)
```

## Interaction Patterns

### Session Selection

```typescript
const handleSessionClick = async (sessionId: string) => {
  // Prevent double-clicks
  if (isLoading || sessionId === currentSessionId) return
  
  setIsLoading(true)
  
  try {
    // Visual feedback immediately
    await loadSession(sessionId)
    
    // Scroll to top of messages (in main area)
    scrollToTop()
    
    // Mobile: Close sidebar
    if (isMobile) {
      closeSidebar()
    }
  } catch (error) {
    toast.error('Failed to load conversation')
  } finally {
    setIsLoading(false)
  }
}
```

### Keyboard Navigation

```typescript
// Arrow key navigation through sessions
useKeyboardNavigation({
  onArrowUp: () => selectPreviousSession(),
  onArrowDown: () => selectNextSession(),
  onEnter: () => loadSelectedSession(),
  onEscape: () => clearSelection(),
})
```

## Helper Functions

### Session Name Derivation

```typescript
function deriveSessionName(session: ChatSession): string {
  // Priority 1: Explicit session name
  if (session.session_name) {
    return session.session_name
  }
  
  // Priority 2: First user message (truncated)
  const firstUserMessage = session.messages.find(m => m.role === 'user')
  if (firstUserMessage) {
    const text = firstUserMessage.content
    return text.length > 30 ? `${text.substring(0, 30)}...` : text
  }
  
  // Priority 3: Agent name + time
  const agentName = session.agent_config?.name || 'Agent'
  const time = formatDateTime(session.created_at)
  return `${agentName} Chat - ${time}`
}
```

### Message Preview Generation

```typescript
function getMessagePreview(messages: Message[]): string {
  if (!messages.length) return 'No messages yet'
  
  // Get last meaningful message (skip system messages)
  const lastMessage = messages
    .filter(m => m.role !== 'system')
    .slice(-1)[0]
  
  if (!lastMessage) return 'No messages yet'
  
  const prefix = lastMessage.role === 'user' ? 'You: ' : 'Agent: '
  const content = lastMessage.content
    .replace(/\n/g, ' ')  // Remove line breaks
    .substring(0, 60)     // Truncate
  
  return `${prefix}${content}${content.length > 60 ? '...' : ''}`
}
```

### Time Formatting

```typescript
function getRelativeTime(timestamp: string | null): string {
  if (!timestamp) return ''
  
  const date = new Date(timestamp)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)
  
  if (diffMins < 1) return 'Just now'
  if (diffMins < 60) return `${diffMins}m`
  if (diffHours < 24) return `${diffHours}h`
  if (diffDays < 7) return `${diffDays}d`
  
  // For older dates, show month/day
  return date.toLocaleDateString('en-US', { 
    month: 'short', 
    day: 'numeric' 
  })
}
```

### Session Grouping

```typescript
function groupSessionsByTime(
  sessions: ChatSession[],
  searchQuery?: string
): GroupedSessions {
  // Filter by search
  let filtered = sessions
  if (searchQuery) {
    filtered = sessions.filter(s => 
      deriveSessionName(s).toLowerCase().includes(searchQuery.toLowerCase()) ||
      getMessagePreview(s.messages).toLowerCase().includes(searchQuery.toLowerCase())
    )
  }
  
  // Sort by updated_at (newest first)
  const sorted = filtered.sort((a, b) => 
    new Date(b.updated_at || b.created_at || 0).getTime() -
    new Date(a.updated_at || a.created_at || 0).getTime()
  )
  
  // Group by time periods
  const groups: GroupedSessions = {
    today: [],
    yesterday: [],
    thisWeek: [],
    thisMonth: [],
    older: []
  }
  
  const now = new Date()
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
  const yesterday = new Date(today)
  yesterday.setDate(yesterday.getDate() - 1)
  const weekAgo = new Date(today)
  weekAgo.setDate(weekAgo.getDate() - 7)
  const monthAgo = new Date(today)
  monthAgo.setMonth(monthAgo.getMonth() - 1)
  
  sorted.forEach(session => {
    const date = new Date(session.updated_at || session.created_at || 0)
    
    if (date >= today) {
      groups.today.push(session)
    } else if (date >= yesterday) {
      groups.yesterday.push(session)
    } else if (date >= weekAgo) {
      groups.thisWeek.push(session)
    } else if (date >= monthAgo) {
      groups.thisMonth.push(session)
    } else {
      groups.older.push(session)
    }
  })
  
  return groups
}
```

### Token Count Formatting

```typescript
function formatTokenCount(count: number): string {
  if (count < 1000) return count.toString()
  if (count < 10000) return `${(count / 1000).toFixed(1)}k`
  return `${Math.floor(count / 1000)}k`
}
```

## Mobile Considerations

### Responsive Behavior

```css
/* Mobile optimizations */
@media (max-width: 768px) {
  /* Larger touch targets */
  .session-item {
    min-height: 56px; /* Increased from 48px */
    padding: 12px;
  }
  
  /* Bigger text for readability */
  .session-name {
    font-size: 14px; /* Up from 13px */
  }
  
  /* Hide metadata on very small screens */
  @media (max-width: 360px) {
    .session-metadata {
      display: none;
    }
  }
}
```

### Touch Interactions

```typescript
// Swipe to delete (mobile only)
const [startX, setStartX] = useState(0)
const [currentX, setCurrentX] = useState(0)
const [isSwiping, setIsSwiping] = useState(false)

const handleTouchStart = (e: TouchEvent) => {
  setStartX(e.touches[0].clientX)
}

const handleTouchMove = (e: TouchEvent) => {
  const x = e.touches[0].clientX
  setCurrentX(x)
  
  if (Math.abs(x - startX) > 10) {
    setIsSwiping(true)
  }
}

const handleTouchEnd = () => {
  if (isSwiping && currentX - startX > 100) {
    // Show delete confirmation
    showDeleteConfirmation()
  }
  
  // Reset
  setStartX(0)
  setCurrentX(0)
  setIsSwiping(false)
}
```

## Performance Optimizations

### Virtual Scrolling for Large Lists

```typescript
// Use @tanstack/react-virtual for lists > 100 items
import { useVirtualizer } from '@tanstack/react-virtual'
import AutoSizer from 'react-virtualized-auto-sizer'

const VirtualizedSessionList: React.FC<{
  sessions: ChatSession[]
  groupedSessions: GroupedSessions
}> = ({ sessions, groupedSessions }) => {
  const listRef = useRef<VariableSizeList>(null)
  const rowHeights = useRef<{ [key: number]: number }>({})
  
  // Calculate item heights (group headers vs sessions)
  const getItemSize = (index: number) => {
    return rowHeights.current[index] || 64 // Default height
  }
  
  // Reset cache when sessions change
  useEffect(() => {
    listRef.current?.resetAfterIndex(0)
  }, [sessions])
  
  return (
    <AutoSizer>
      {({ height, width }) => (
        <VariableSizeList
          ref={listRef}
          height={height}
          itemCount={sessions.length}
          itemSize={getItemSize}
          width={width}
          overscanCount={5} // Render 5 items outside visible area
        >
          {({ index, style }) => (
            <div style={style}>
              <SessionItem 
                session={sessions[index]}
                onHeightChange={(height) => {
                  rowHeights.current[index] = height
                  listRef.current?.resetAfterIndex(index)
                }}
              />
            </div>
          )}
        </VariableSizeList>
      )}
    </AutoSizer>
  )
}
```

### Scroll Position Preservation

```typescript
const useScrollPreservation = (containerRef: RefObject<HTMLDivElement>) => {
  const scrollPositionRef = useRef(0)
  const isLoadingMoreRef = useRef(false)
  
  // Save scroll position before loading more
  const saveScrollPosition = useCallback(() => {
    if (containerRef.current) {
      scrollPositionRef.current = containerRef.current.scrollTop
      isLoadingMoreRef.current = true
    }
  }, [])
  
  // Restore scroll position after new items load
  const restoreScrollPosition = useCallback(() => {
    if (containerRef.current && isLoadingMoreRef.current) {
      // Calculate height difference and adjust
      const currentHeight = containerRef.current.scrollHeight
      const scrollDiff = currentHeight - scrollPositionRef.current
      
      // Maintain visual position
      containerRef.current.scrollTop = scrollPositionRef.current
      isLoadingMoreRef.current = false
    }
  }, [])
  
  return { saveScrollPosition, restoreScrollPosition }
}
```

### Debounced Scroll Events

```typescript
// Debounce scroll detection for performance
const useDebounce = <T extends (...args: any[]) => any>(
  callback: T,
  delay: number
): T => {
  const timeoutRef = useRef<NodeJS.Timeout>()
  
  return useCallback(
    ((...args) => {
      clearTimeout(timeoutRef.current)
      timeoutRef.current = setTimeout(() => callback(...args), delay)
    }) as T,
    [callback, delay]
  )
}

// Usage in scroll detection
const handleScroll = useDebounce((e: Event) => {
  const container = e.target as HTMLDivElement
  const { scrollTop, scrollHeight, clientHeight } = container
  
  // Check if near bottom (80% threshold)
  const scrollPercentage = (scrollTop + clientHeight) / scrollHeight
  
  if (scrollPercentage > 0.8 && hasMoreSessions && !isPaginationLoading) {
    fetchMoreSessions()
  }
}, 150) // 150ms debounce
```

### Memoization with Pagination

```typescript
// Memoize session items to prevent re-renders
const SessionItem = React.memo<SessionItemProps>(
  ({ session, isActive, onClick }) => {
    // Component implementation
  },
  (prevProps, nextProps) => {
    // Only re-render if these specific props change
    return (
      prevProps.session.session_id === nextProps.session.session_id &&
      prevProps.session.updated_at === nextProps.session.updated_at &&
      prevProps.isActive === nextProps.isActive
    )
  }
)

// Memoize grouped sessions computation
const groupedSessions = useMemo(() => {
  // Only recompute when sessions array or search changes
  return groupSessionsByTime(sessions, searchQuery)
}, [sessions, searchQuery])
```

### Batch Size Optimization

```typescript
const OPTIMAL_BATCH_SIZE = 50 // Default, adjustable based on performance

const useChatSessions = () => {
  const [batchSize, setBatchSize] = useState(OPTIMAL_BATCH_SIZE)
  
  // Adjust batch size based on device performance
  useEffect(() => {
    // Check device memory if available
    const memory = (navigator as any).deviceMemory
    if (memory) {
      if (memory < 4) setBatchSize(25)      // Low-end devices
      else if (memory >= 8) setBatchSize(75) // High-end devices
    }
    
    // Check connection speed
    const connection = (navigator as any).connection
    if (connection?.effectiveType === 'slow-2g' || connection?.effectiveType === '2g') {
      setBatchSize(25) // Smaller batches for slow connections
    }
  }, [])
  
  return { batchSize }
}
```

## Integration with SDK

### Enhanced Hook with Pagination Support

```typescript
// Custom hook for session management with pagination
const useChatSessions = () => {
  const { authManager } = useAuth()
  const { sessionManager, client } = useAgentC()
  const { setMessages } = useChat()
  
  const [sessions, setSessions] = useState<ChatSession[]>([])
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null)
  const [isInitialLoading, setIsInitialLoading] = useState(true)
  const [isPaginationLoading, setIsPaginationLoading] = useState(false)
  const [paginationError, setPaginationError] = useState<Error | null>(null)
  const [offset, setOffset] = useState(0)
  const [hasMoreSessions, setHasMoreSessions] = useState(true)
  const [totalSessionCount, setTotalSessionCount] = useState(0)
  
  const BATCH_SIZE = 50
  
  // Initial session load
  useEffect(() => {
    const loadInitialSessions = async () => {
      try {
        setIsInitialLoading(true)
        setPaginationError(null)
        
        // Get initial batch from auth manager
        const initialSessions = await authManager.getSessions()
        const metadata = await authManager.getSessionsMetadata()
        
        setSessions(initialSessions)
        setTotalSessionCount(metadata.total_sessions)
        setOffset(initialSessions.length)
        setHasMoreSessions(initialSessions.length < metadata.total_sessions)
      } catch (error) {
        console.error('Failed to load initial sessions:', error)
        setPaginationError(error as Error)
      } finally {
        setIsInitialLoading(false)
      }
    }
    
    loadInitialSessions()
  }, [authManager])
  
  // Fetch more sessions (pagination)
  const fetchMoreSessions = useCallback(async () => {
    if (isPaginationLoading || !hasMoreSessions) return
    
    try {
      setIsPaginationLoading(true)
      setPaginationError(null)
      
      // Fetch next batch
      const moreSessions = await client.fetchUserSessions(offset, BATCH_SIZE)
      
      if (moreSessions && moreSessions.length > 0) {
        setSessions(prev => [...prev, ...moreSessions])
        setOffset(prev => prev + moreSessions.length)
        
        // Check if we've loaded all sessions
        const newTotal = offset + moreSessions.length
        setHasMoreSessions(newTotal < totalSessionCount)
      } else {
        setHasMoreSessions(false)
      }
    } catch (error) {
      console.error('Failed to load more sessions:', error)
      setPaginationError(error as Error)
    } finally {
      setIsPaginationLoading(false)
    }
  }, [client, offset, BATCH_SIZE, totalSessionCount, isPaginationLoading, hasMoreSessions])
  
  // Load a specific session
  const loadSession = useCallback(async (sessionId: string) => {
    const session = sessions.find(s => s.session_id === sessionId)
    if (!session) return
    
    // Update chat context with session messages
    setMessages(session.messages)
    setCurrentSessionId(sessionId)
    
    // Optionally update agent config
    // updateAgentConfig(session.agent_config)
  }, [sessions, setMessages])
  
  // Refresh all sessions (pull-to-refresh, manual refresh)
  const refreshSessions = useCallback(async () => {
    setSessions([])
    setOffset(0)
    setHasMoreSessions(true)
    await loadInitialSessions()
  }, [])
  
  return {
    sessions,
    currentSessionId,
    loadSession,
    refreshSessions,
    fetchMoreSessions,
    isInitialLoading,
    isPaginationLoading,
    paginationError,
    hasMoreSessions,
    totalSessionCount
  }
}
```

### Search with Pagination

```typescript
// Handle search with paginated data
const useSearchSessions = (sessions: ChatSession[], hasMore: boolean) => {
  const [searchQuery, setSearchQuery] = useState('')
  const [isSearching, setIsSearching] = useState(false)
  const [searchResults, setSearchResults] = useState<ChatSession[]>([])
  const [showLoadAllPrompt, setShowLoadAllPrompt] = useState(false)
  
  // Debounced search
  const debouncedSearch = useDebounce((query: string) => {
    if (!query.trim()) {
      setSearchResults([])
      setShowLoadAllPrompt(false)
      return
    }
    
    setIsSearching(true)
    
    // Search through loaded sessions
    const results = sessions.filter(session => {
      const name = deriveSessionName(session).toLowerCase()
      const preview = getMessagePreview(session.messages).toLowerCase()
      const searchLower = query.toLowerCase()
      
      return name.includes(searchLower) || preview.includes(searchLower)
    })
    
    setSearchResults(results)
    
    // Show prompt if there might be more results
    if (results.length < 5 && hasMore) {
      setShowLoadAllPrompt(true)
    }
    
    setIsSearching(false)
  }, 300)
  
  useEffect(() => {
    debouncedSearch(searchQuery)
  }, [searchQuery, sessions])
  
  return {
    searchQuery,
    setSearchQuery,
    searchResults,
    isSearching,
    showLoadAllPrompt
  }
}
```

## ASCII Visual Mockup

### Standard View with Pagination

```
┌─────────────────────────────────┐
│ Showing 50 of 237 conversations │  <- Session count indicator
├─────────────────────────────────┤
│ 🔍 Search conversations...       │
├─────────────────────────────────┤
│ TODAY                            │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │ ▸ Exploring Agent C SDK   2m │ │  <- Active session
│ │   You: How do I integrate... │ │
│ │   💬 15  # 2.3k              │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │   Voice setup questions   1h │ │
│ │   Agent: To enable voice...  │ │
│ │   💬 8   # 1.2k              │ │
│ └─────────────────────────────┘ │
├─────────────────────────────────┤
│ YESTERDAY                        │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   Authentication flow     23h│ │
│ │   You: Can you explain the...│ │
│ │   💬 23  # 4.5k              │ │
│ └─────────────────────────────┘ │
├─────────────────────────────────┤
│ THIS WEEK                        │  <- User scrolls down
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   Initial setup help      3d │ │
│ │   Agent: Welcome! Let's...   │ │
│ │   💬 42  # 8.1k              │ │
│ └─────────────────────────────┘ │
│                                  │
│  ⟲ Loading more conversations... │  <- Auto-loads at 80% scroll
│                                  │
└─────────────────────────────────┘
```

### Loading More Sessions

```
┌─────────────────────────────────┐
│ Showing 100 of 237 conversations│  <- Count updates
├─────────────────────────────────┤
│ THIS MONTH                       │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   React patterns help     2w │ │
│ │   You: What's the best way...│ │
│ │   💬 31  # 5.2k              │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │   Database optimization   2w │ │
│ │   Agent: For better perf...  │ │
│ │   💬 18  # 3.1k              │ │
│ └─────────────────────────────┘ │
├─────────────────────────────────┤
│ OLDER                            │  <- Newly loaded group
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   Project kickoff         1mo│ │
│ │   You: Starting a new...     │ │
│ │   💬 67  # 12.3k             │ │
│ └─────────────────────────────┘ │
│                                  │
│  ⟲ Loading more conversations... │  <- Continues loading
│                                  │
└─────────────────────────────────┘
```

### End of List

```
┌─────────────────────────────────┐
│ Showing 237 of 237 conversations│  <- All loaded
├─────────────────────────────────┤
│ OLDER                            │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   First conversation      6mo│ │
│ │   You: Hello, I need help... │ │
│ │   💬 12  # 1.8k              │ │
│ └─────────────────────────────┘ │
│                                  │
│   You've reached the end of     │  <- End indicator
│   your conversations            │
│                                  │
└─────────────────────────────────┘
```

### Error State During Pagination

```
┌─────────────────────────────────┐
│ Showing 50 of 237 conversations │
├─────────────────────────────────┤
│ THIS WEEK                        │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   API integration test    5d │ │
│ │   You: Testing the new...    │ │
│ │   💬 5   # 0.9k              │ │
│ └─────────────────────────────┘ │
│                                  │
│   ⚠️ Failed to load more        │  <- Error message
│      sessions                    │
│                                  │
│     [ Try Again ]                │  <- Retry button
│                                  │
└─────────────────────────────────┘
```

### Search with Pagination Prompt

```
┌─────────────────────────────────┐
│ Showing 50 of 237 conversations │
├─────────────────────────────────┤
│ 🔍 "authentication"              │  <- Active search
├─────────────────────────────────┤
│ SEARCH RESULTS (3 found)         │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │   Authentication flow     23h│ │
│ │   You: Can you explain the...│ │
│ │   💬 23  # 4.5k              │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │   OAuth setup issues      3d │ │
│ │   You: Having trouble with...│ │
│ │   💬 15  # 2.8k              │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │   JWT token handling      1w │ │
│ │   Agent: For JWT tokens...   │ │
│ │   💬 28  # 4.2k              │ │
│ └─────────────────────────────┘ │
│                                  │
│   ℹ️ Only searching loaded       │  <- Search limitation notice
│      sessions. Load all?         │
│                                  │
│     [ Load All Sessions ]        │  <- Option to load all
│                                  │
└─────────────────────────────────┘
```

## Implementation Notes

### Phase 1: Core Functionality with Pagination
1. Basic session list rendering with initial batch (50 sessions)
2. Time grouping (Today, Yesterday, etc.)
3. Session name derivation
4. Active session highlighting
5. Click to load session
6. Infinite scrolling with intersection observer
7. Loading states (initial, pagination, error)

### Phase 2: Pagination Enhancements
1. Virtual scrolling for lists > 100 items
2. Scroll position preservation during pagination
3. Debounced scroll detection
4. Adaptive batch sizing based on device
5. Search with pagination awareness
6. Manual "Load More" fallback button

### Phase 3: Advanced Features
1. Session pinning (prioritized in display)
2. Pull-to-refresh on mobile
3. Optimistic updates for new sessions
4. Background session prefetching
5. Smart caching with IndexedDB
6. Export full conversation history

## Pagination Edge Cases

### Handling Edge Cases

```typescript
const handleEdgeCases = {
  // No sessions at all
  noSessions: () => {
    if (totalSessionCount === 0 && !isInitialLoading) {
      return <EmptyState />
    }
  },
  
  // Single batch (all sessions loaded at once)
  singleBatch: () => {
    if (sessions.length === totalSessionCount && totalSessionCount <= BATCH_SIZE) {
      // Don't show pagination indicators
      return null
    }
  },
  
  // Thousands of sessions
  manySessionsOptimization: () => {
    if (totalSessionCount > 1000) {
      // Force virtual scrolling
      // Increase batch size for better performance
      // Consider implementing search-first UI
      return <VirtualizedSessionList sessions={sessions} />
    }
  },
  
  // Session deleted while paginating
  sessionDeleted: (deletedId: string) => {
    // Remove from local state
    setSessions(prev => prev.filter(s => s.session_id !== deletedId))
    // Adjust total count
    setTotalSessionCount(prev => prev - 1)
    // Adjust offset if needed
    if (sessions.length < offset) {
      setOffset(sessions.length)
    }
  },
  
  // New session created
  sessionCreated: (newSession: ChatSession) => {
    // Add to beginning of list
    setSessions(prev => [newSession, ...prev])
    // Adjust total count
    setTotalSessionCount(prev => prev + 1)
  },
  
  // Network failure during pagination
  networkFailure: () => {
    // Show retry UI
    // Keep existing sessions visible
    // Don't reset scroll position
    return <PaginationErrorState onRetry={fetchMoreSessions} />
  }
}
```

### Session State Management During Pagination

```typescript
const maintainSessionState = {
  // Preserve selected session during pagination
  preserveSelection: () => {
    // Keep currentSessionId in state
    // Highlight even if session moves position
    // Don't auto-scroll to selected when paginating
  },
  
  // Handle session updates
  handleSessionUpdate: (updatedSession: ChatSession) => {
    setSessions(prev => prev.map(s => 
      s.session_id === updatedSession.session_id ? updatedSession : s
    ))
  },
  
  // Optimistic updates
  optimisticUpdate: (tempSession: Partial<ChatSession>) => {
    // Add temporary session immediately
    // Replace with real data when available
    // Handle failure gracefully
  }
}
```

## Testing Checklist

### Core Functionality
- [ ] Sessions display correctly with all fields
- [ ] Time grouping works accurately
- [ ] Active session is visually distinct
- [ ] Empty state displays when no sessions
- [ ] Loading state shows during transitions
- [ ] Search filters sessions correctly
- [ ] Mobile touch targets are adequate (44px min)
- [ ] Keyboard navigation works
- [ ] Screen reader announces session details
- [ ] Relative timestamps update appropriately
- [ ] Token counts format correctly
- [ ] Message previews truncate properly
- [ ] Session names derive correctly from data

### Pagination Testing
- [ ] Initial batch loads correctly (50 sessions)
- [ ] Infinite scroll triggers at 80% scroll depth
- [ ] Loading indicator appears while fetching
- [ ] New sessions append seamlessly
- [ ] "No more sessions" message shows at end
- [ ] Error state displays on fetch failure
- [ ] Retry mechanism works after error
- [ ] Session count indicator updates correctly
- [ ] Scroll position preserved during pagination
- [ ] Search works with partial session list
- [ ] Manual "Load More" button works as fallback

### Performance Testing
- [ ] Performance with 100+ sessions (virtual scrolling)
- [ ] Performance with 500+ sessions
- [ ] Performance with 1000+ sessions
- [ ] Scroll remains smooth during pagination
- [ ] Memory usage stays reasonable with large lists
- [ ] No unnecessary re-renders during pagination
- [ ] Debounced scroll events working
- [ ] Batch size adjusts for device capabilities

### Edge Case Testing
- [ ] Handles 0 sessions gracefully
- [ ] Handles single batch (< 50 sessions)
- [ ] Handles exactly 50, 100, 150 sessions
- [ ] Session deletion updates pagination state
- [ ] New session creation updates list
- [ ] Network timeout handled gracefully
- [ ] Rapid scrolling doesn't cause issues
- [ ] Search with pagination prompt appears
- [ ] Selected session persists during pagination

## Accessibility Compliance

- ARIA labels for all interactive elements
- Keyboard navigation support
- Focus management for session switching
- Screen reader announcements for state changes
- Sufficient color contrast (4.5:1 minimum)
- Clear focus indicators
- Semantic HTML structure
- Loading state announcements

## Summary

The ChatSessionList component provides a rich, accessible interface for browsing chat history. It leverages the session data from the authentication API to display organized, searchable conversation history with clear visual hierarchy and smooth interactions. The design follows CenSuite patterns while providing Claude-inspired organization and visual polish.