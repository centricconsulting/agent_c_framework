# ChatSessionList Component Complete Design Specification

## Executive Summary

The ChatSessionList component is a core UI element that displays a user's chat session history with rich functionality including search, filtering, lazy loading, and real-time updates. This component follows CenSuite design principles and integrates seamlessly with the Agent C Realtime SDK event system.

## Table of Contents

1. [Component Overview](#component-overview)
2. [Data Model & Event Types](#data-model--event-types)
3. [Component Architecture](#component-architecture)
4. [Visual Design Specifications](#visual-design-specifications)
5. [Sub-Components Breakdown](#sub-components-breakdown)
6. [State Management Strategy](#state-management-strategy)
7. [User Experience Flow](#user-experience-flow)
8. [Accessibility Implementation](#accessibility-implementation)
9. [Responsive Design Strategy](#responsive-design-strategy)
10. [Performance Optimization](#performance-optimization)
11. [Implementation Roadmap](#implementation-roadmap)

## Component Overview

The ChatSessionList replaces the current stub implementation with a fully-featured session management interface that:

- Displays sessions from `GetUserSessionsResponseEvent`
- Groups sessions by time periods (Today, Recent, Past Sessions)
- Supports incremental loading via infinite scroll
- Provides real-time updates through event handling
- Enables session selection, deletion, and management
- Maintains accessibility compliance (WCAG 2.1 AA)

### Key Features

- **Search & Filter**: Real-time filtering by session name or agent name
- **Smart Grouping**: Automatic time-based organization
- **Lazy Loading**: Performance-optimized infinite scroll
- **Real-time Updates**: Live session state synchronization
- **User Actions**: Select to resume, delete with confirmation
- **Visual Feedback**: Loading states, error handling, empty states

## Data Model & Event Types

### Important Note on Model Names

**CRITICAL**: This component uses `ChatSessionIndexEntry` for list items, NOT `ChatSession`. These are two distinct models:

- **`ChatSessionIndexEntry`**: Lightweight model for session lists (what we display)
- **`ChatSession`**: Full session data with messages and metadata (only used when a session is loaded)

The API provides `ChatSessionIndexEntry` for efficient list rendering without loading full message history.

### Core Types

```typescript
// Session data structure from GetUserSessionsResponseEvent
// IMPORTANT: Using ChatSessionIndexEntry for list items, NOT ChatSession
interface ChatSessionIndexEntry {
  session_id: string
  session_name: string | null
  created_at: string | null // ISO timestamp
  updated_at: string | null // ISO timestamp
  user_id: string | null
  agent_key?: string
  agent_name?: string
  
  // Note: These fields would enhance UX but don't currently exist in the API model:
  // - message_count: number
  // - token_count: number  
  // - last_message_preview: string
  // Consider requesting these be added server-side for better list display
}

// Events for real-time updates
interface GetUserSessionsEvent {
  type: 'get_user_sessions'
  offset: number
  limit: number
}

interface GetUserSessionsResponseEvent {
  type: 'get_user_sessions_response'
  sessions: ChatSessionQueryResponse  // Contains chat_sessions array
}

interface ChatSessionQueryResponse {
  chat_sessions: ChatSessionIndexEntry[]
  total_sessions: number
  offset: number
}

interface ResumeChatSessionEvent {
  type: 'resume_chat_session'
  session_id: string
}

interface DeleteChatSessionEvent {
  type: 'delete_chat_session'
  session_id: string
}

interface ChatSessionChangedEvent {
  type: 'chat_session_changed'
  session_id: string
  updated_at: string
}

interface ChatSessionNameChangedEvent {
  type: 'chat_session_name_changed'
  session_id: string
  session_name: string
}

// ChatSessionQueryResponse is already defined above
// It contains chat_sessions: ChatSessionIndexEntry[]

interface ChatSessionAddedEvent {
  type: 'chat_session_added'
  chat_session: ChatSessionIndexEntry  // Lightweight entry for list
}

interface ChatSessionDeletedEvent {
  type: 'chat_session_deleted'
  session_id: string
}
```

## Component Architecture

### Main Component Structure

```tsx
// packages/ui/src/components/session/ChatSessionList.tsx

import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { cn } from "../../lib/utils"
import { 
  Search, 
  Trash2, 
  MessageSquare, 
  Clock, 
  Hash,
  AlertCircle,
  Loader2
} from "lucide-react"
import { ScrollArea } from "../ui/scroll-area"
import { Input } from "../ui/input"
import { Button } from "../ui/button"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "../ui/alert-dialog"

const chatSessionListVariants = cva(
  "flex flex-col h-full",
  {
    variants: {
      variant: {
        default: "bg-background",
        sidebar: "bg-muted/30",
      },
      size: {
        default: "w-full",
        compact: "w-64",
        expanded: "w-80",
      }
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
)

export interface ChatSessionListProps 
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof chatSessionListVariants> {
  sessions?: ChatSessionIndexEntry[]  // Using lightweight model for list items
  currentSessionId?: string | null
  isCollapsed?: boolean
  onSessionSelect?: (sessionId: string) => void
  onSessionDelete?: (sessionId: string) => void
  onLoadMore?: (offset: number) => void
  totalCount?: number
  hasMore?: boolean
  isLoading?: boolean
  error?: Error | null
}

export const ChatSessionList = React.forwardRef<HTMLDivElement, ChatSessionListProps>(
  ({ 
    className,
    variant,
    size,
    sessions = [],
    currentSessionId,
    isCollapsed,
    onSessionSelect,
    onSessionDelete,
    onLoadMore,
    totalCount = 0,
    hasMore = false,
    isLoading = false,
    error,
    ...props 
  }, ref) => {
    // Component implementation
    const [searchQuery, setSearchQuery] = React.useState("")
    const [deleteConfirmId, setDeleteConfirmId] = React.useState<string | null>(null)
    const [localSessions, setLocalSessions] = React.useState<ChatSessionIndexEntry[]>(sessions)
    const scrollAreaRef = React.useRef<HTMLDivElement>(null)
    const loadMoreTriggerRef = React.useRef<HTMLDivElement>(null)
    
    // Implementation details follow...
  }
)
ChatSessionList.displayName = "ChatSessionList"
```

## Visual Design Specifications

### Design Tokens & Spacing

```scss
// Component-specific design tokens
--session-item-height: 72px;
--session-item-padding: 12px;
--session-group-header-height: 32px;
--search-input-height: 36px;
--delete-button-size: 28px;
--collapsed-button-size: 48px;

// Spacing scale (following CenSuite 4px base)
--spacing-xs: 4px;   // gap-1
--spacing-sm: 8px;   // gap-2
--spacing-md: 16px;  // gap-4
--spacing-lg: 24px;  // gap-6
--spacing-xl: 32px;  // gap-8
```

### Component Layout Structure

```
┌─────────────────────────────────────┐
│ Search/Filter Bar                    │ <- Sticky top
├─────────────────────────────────────┤
│ Session Count Indicator              │ <- Shows "X of Y sessions"
├═════════════════════════════════════┤
│ ┌─ Scrollable Area ────────────────┐│
│ │                                   ││
│ │ [TODAY] ─────────────────────     ││ <- Group header
│ │ ┌───────────────────────────────┐ ││
│ │ │ • Session Name           2m 🗑 │ ││ <- Active indicator
│ │ │   With Agent Smith            │ ││
│ │ └───────────────────────────────┘ ││
│ │                                   ││
│ │ [RECENT] ────────────────────     ││ <- Within 14 days
│ │ ┌───────────────────────────────┐ ││
│ │ │   Previous Chat         3d 🗑  │ ││
│ │ │   With Agent Jones            │ ││
│ │ └───────────────────────────────┘ ││
│ │                                   ││
│ │ [PAST SESSIONS] ──────────        ││ <- Older than 14 days
│ │ ┌───────────────────────────────┐ ││
│ │ │   Older Discussion     1mo 🗑  │ ││
│ │ │   With Agent Brown            │ ││
│ │ └───────────────────────────────┘ ││
│ │                                   ││
│ │ [Load More Trigger]               ││ <- Infinite scroll
│ └───────────────────────────────────┘│
└─────────────────────────────────────┘
```

### Visual States

#### Session Item States

```tsx
// Default state
className="group relative w-full text-left rounded-lg px-3 py-3 
          transition-all duration-200 hover:bg-muted/60"

// Active/Selected state  
className="bg-accent shadow-sm ring-1 ring-border/50"

// Hover state
className="hover:bg-muted/60 hover:shadow-sm"

// Loading state
className="opacity-60 cursor-wait pointer-events-none"

// Error state
className="border border-destructive/50 bg-destructive/5"
```

## Sub-Components Breakdown

### 1. SearchFilterBar Component

```tsx
const SearchFilterBar: React.FC<{
  value: string
  onChange: (value: string) => void
  isSearching?: boolean
}> = ({ value, onChange, isSearching }) => (
  <div className="sticky top-0 z-10 bg-background/95 backdrop-blur-sm p-2 border-b">
    <div className="relative">
      {isSearching ? (
        <Loader2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 animate-spin text-muted-foreground" />
      ) : (
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
      )}
      <Input
        type="search"
        placeholder="Search by session or agent name..."
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="pl-9 h-9 text-sm"
        aria-label="Search sessions"
      />
    </div>
  </div>
)
```

### 2. SessionGroupHeader Component

```tsx
const SessionGroupHeader: React.FC<{
  label: string
  count?: number
}> = ({ label, count }) => (
  <div className="sticky top-12 z-[5] flex items-center gap-2 px-3 py-2 
                  bg-background/90 backdrop-blur-sm">
    <span className="text-xs font-semibold uppercase tracking-wider 
                     text-muted-foreground">
      {label}
    </span>
    {count !== undefined && (
      <span className="text-xs text-muted-foreground">({count})</span>
    )}
    <div className="flex-1 h-px bg-border/50" />
  </div>
)
```

### 3. SessionItem Component

```tsx
const SessionItem: React.FC<{
  session: ChatSessionIndexEntry  // Using lightweight model
  isActive: boolean
  onSelect: () => void
  onDelete: () => void
  isDeleting?: boolean
}> = ({ session, isActive, onSelect, onDelete, isDeleting }) => {
  const displayName = session.session_name || `Chat from ${formatDate(session.created_at || new Date().toISOString())}`
  const timeAgo = getRelativeTime(session.updated_at || session.created_at || new Date().toISOString())
  
  return (
    <div
      className={cn(
        "group relative flex items-start gap-3 rounded-lg px-3 py-3",
        "transition-all duration-200 cursor-pointer",
        "hover:bg-muted/60 hover:shadow-sm",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
        isActive && "bg-accent shadow-sm ring-1 ring-border/50",
        isDeleting && "opacity-50 pointer-events-none"
      )}
      onClick={onSelect}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onSelect()
        }
      }}
      tabIndex={0}
      role="button"
      aria-label={`Select session: ${displayName} with ${session.agent_name}`}
      aria-current={isActive ? "true" : undefined}
    >
      {/* Active indicator */}
      {isActive && (
        <div className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-8 
                        bg-primary rounded-r-full" 
             aria-hidden="true" />
      )}
      
      {/* Session content */}
      <div className="flex-1 min-w-0">
        <div className="flex items-start justify-between gap-2">
          <div className="flex-1 min-w-0">
            <p className="font-semibold text-sm truncate">
              {displayName}
            </p>
            <p className="text-xs text-muted-foreground mt-0.5">
              {session.agent_name ? `With ${session.agent_name}` : `Agent: ${session.agent_key || 'Unknown'}`}
            </p>
          </div>
          
          <div className="flex items-center gap-1">
            <span className="text-xs text-muted-foreground">
              {timeAgo}
            </span>
            
            {/* Delete button */}
            <Button
              variant="ghost"
              size="icon"
              className="h-6 w-6 opacity-0 group-hover:opacity-100 
                         transition-opacity"
              onClick={(e) => {
                e.stopPropagation()
                onDelete()
              }}
              aria-label={`Delete session: ${displayName}`}
            >
              <Trash2 className="h-3 w-3" />
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
```

### 4. EmptyState Component

```tsx
const EmptyState: React.FC<{
  searchQuery?: string
}> = ({ searchQuery }) => (
  <div className="flex flex-col items-center justify-center py-12 px-4">
    <div className="rounded-full bg-muted/50 p-4 mb-4">
      <MessageSquare className="h-6 w-6 text-muted-foreground" />
    </div>
    <h3 className="text-base font-semibold text-foreground mb-2">
      {searchQuery ? 'No matching sessions' : 'No conversations yet'}
    </h3>
    <p className="text-sm text-muted-foreground text-center max-w-[200px]">
      {searchQuery 
        ? `No sessions found matching "${searchQuery}"`
        : 'Start a new chat to begin your conversation history'}
    </p>
  </div>
)
```

### 5. LoadingIndicator Component

```tsx
const LoadingIndicator: React.FC<{
  variant?: 'initial' | 'pagination'
}> = ({ variant = 'initial' }) => {
  if (variant === 'initial') {
    return (
      <div className="space-y-3 p-3">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="animate-pulse">
            <div className="rounded-lg bg-muted/30 h-16 p-3">
              <div className="h-4 bg-muted/50 rounded w-3/4 mb-2" />
              <div className="h-3 bg-muted/40 rounded w-1/2" />
            </div>
          </div>
        ))}
      </div>
    )
  }
  
  return (
    <div className="flex items-center justify-center py-4">
      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        <Loader2 className="h-4 w-4 animate-spin" />
        <span>Loading more sessions...</span>
      </div>
    </div>
  )
}
```

### 6. DeleteConfirmationDialog Component

```tsx
const DeleteConfirmationDialog: React.FC<{
  session: ChatSessionIndexEntry | null  // Lightweight model
  isOpen: boolean
  onConfirm: () => void
  onCancel: () => void
}> = ({ session, isOpen, onConfirm, onCancel }) => (
  <AlertDialog open={isOpen} onOpenChange={(open) => !open && onCancel()}>
    <AlertDialogContent>
      <AlertDialogHeader>
        <AlertDialogTitle>Delete conversation?</AlertDialogTitle>
        <AlertDialogDescription>
          Are you sure you want to delete "{session?.session_name || 'this conversation'}"? 
          This action cannot be undone.
        </AlertDialogDescription>
      </AlertDialogHeader>
      <AlertDialogFooter>
        <AlertDialogCancel onClick={onCancel}>Cancel</AlertDialogCancel>
        <AlertDialogAction 
          onClick={onConfirm}
          className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
        >
          Delete
        </AlertDialogAction>
      </AlertDialogFooter>
    </AlertDialogContent>
  </AlertDialog>
)
```

## State Management Strategy

### Local State Management

```typescript
interface SessionListState {
  // Core data
  sessions: ChatSessionIndexEntry[]  // Lightweight entries for list
  filteredSessions: ChatSessionIndexEntry[]
  groupedSessions: GroupedSessions
  
  // UI state
  searchQuery: string
  currentSessionId: string | null
  deleteConfirmId: string | null
  
  // Loading states
  isInitialLoading: boolean
  isPaginationLoading: boolean
  isDeleting: boolean
  
  // Pagination
  offset: number
  totalCount: number
  hasMore: boolean
  
  // Error handling
  error: Error | null
  retryCount: number
}

interface GroupedSessions {
  today: ChatSessionIndexEntry[]
  recent: ChatSessionIndexEntry[]  // Past 14 days
  past: ChatSessionIndexEntry[]     // Older than 14 days
}
```

### State Update Patterns

```typescript
// Optimistic updates for better UX
const optimisticDelete = (sessionId: string) => {
  // Immediately remove from UI
  setLocalSessions(prev => prev.filter(s => s.session_id !== sessionId))
  
  // Send delete event
  sendDeleteEvent(sessionId)
    .catch(() => {
      // Restore on failure
      setLocalSessions(prev => [...prev, deletedSession])
      showError('Failed to delete session')
    })
}

// Incremental loading pattern
const loadMoreSessions = async () => {
  if (isPaginationLoading || !hasMore) return
  
  setIsPaginationLoading(true)
  
  try {
    const event: GetUserSessionsEvent = {
      type: 'get_user_sessions',
      offset: sessions.length,
      limit: 50
    }
    
    await sendEvent(event)
    // Response handled by event listener
  } catch (error) {
    setError(error)
  } finally {
    setIsPaginationLoading(false)
  }
}
```

## User Experience Flow

### Interaction Patterns

1. **Session Selection Flow**
   ```
   User clicks session → Visual feedback (highlight) → 
   Send ResumeChatSessionEvent → Load messages → 
   Update UI with confirmation
   ```

2. **Delete Flow**
   ```
   User clicks delete → Show confirmation dialog → 
   User confirms → Optimistic removal → 
   Send DeleteChatSessionEvent → Handle response
   ```

3. **Search Flow**
   ```
   User types → Debounced filter (300ms) → 
   Update filtered sessions → Show results → 
   Display "no results" if empty
   ```

4. **Infinite Scroll Flow**
   ```
   User scrolls to 80% → Trigger load more → 
   Show loading indicator → Fetch next batch → 
   Append to list → Update scroll position
   ```

### Loading States

```typescript
// Three-tier loading strategy
const LoadingStates = {
  initial: "Full skeleton UI with 5-8 items",
  pagination: "Bottom spinner with 'Loading more...'",
  refresh: "Subtle top bar progress indicator"
}
```

### Error Recovery

```typescript
const ErrorRecovery = {
  network: {
    message: "Connection lost. Retrying...",
    action: "Automatic retry with exponential backoff"
  },
  deletion: {
    message: "Failed to delete session",
    action: "Show retry button, restore item"
  },
  loading: {
    message: "Failed to load sessions",
    action: "Show retry button with manual trigger"
  }
}
```

## Accessibility Implementation

### ARIA Attributes

```tsx
// Main list container
<div
  role="navigation"
  aria-label="Chat sessions"
  aria-busy={isLoading}
  aria-live="polite"
>
  {/* Session count announcement */}
  <div className="sr-only" aria-live="polite" aria-atomic="true">
    {`Showing ${sessions.length} of ${totalCount} conversations`}
  </div>
  
  {/* Session items */}
  <div role="list">
    {sessions.map(session => (
      <div
        role="listitem"
        tabIndex={0}
        aria-current={isActive ? "page" : undefined}
        aria-label={`${session.session_name}, with ${session.agent_name}, 
                     last updated ${getRelativeTime(session.updated_at)}`}
      >
        {/* Content */}
      </div>
    ))}
  </div>
</div>
```

### Keyboard Navigation

```typescript
const useKeyboardNavigation = () => {
  const handleKeyDown = (e: KeyboardEvent) => {
    switch(e.key) {
      case 'ArrowDown':
        e.preventDefault()
        focusNextSession()
        break
      case 'ArrowUp':
        e.preventDefault()
        focusPreviousSession()
        break
      case 'Enter':
      case ' ':
        e.preventDefault()
        selectFocusedSession()
        break
      case 'Delete':
        if (e.shiftKey) {
          e.preventDefault()
          deleteFocusedSession()
        }
        break
      case 'Escape':
        e.preventDefault()
        clearFocus()
        break
      case '/':
        e.preventDefault()
        focusSearchInput()
        break
    }
  }
  
  return { handleKeyDown }
}
```

### Screen Reader Announcements

```tsx
// Announce state changes
const announceChange = (message: string) => {
  const announcement = document.createElement('div')
  announcement.setAttribute('role', 'status')
  announcement.setAttribute('aria-live', 'polite')
  announcement.className = 'sr-only'
  announcement.textContent = message
  document.body.appendChild(announcement)
  
  setTimeout(() => {
    document.body.removeChild(announcement)
  }, 1000)
}

// Usage
announceChange(`Session "${sessionName}" selected`)
announceChange(`Session deleted`)
announceChange(`${newSessions.length} new sessions loaded`)
```

## Responsive Design Strategy

### Breakpoint Behaviors

```scss
// Mobile (< 640px)
@media (max-width: 639px) {
  .session-item {
    min-height: 64px; // Larger touch targets
    padding: 12px;
  }
  
  .session-name {
    font-size: 14px; // Increased readability
  }
  
  .delete-button {
    // Always visible on mobile (no hover)
    opacity: 1;
  }
}

// Tablet (640px - 1024px)
@media (min-width: 640px) and (max-width: 1023px) {
  .session-list {
    max-width: 320px;
  }
}

// Desktop (>= 1024px)
@media (min-width: 1024px) {
  .session-list {
    width: 100%;
    max-width: 360px;
  }
}
```

### Collapsed State (Mobile/Sidebar)

```tsx
const CollapsedView: React.FC<{
  sessions: ChatSessionIndexEntry[]  // Lightweight model for list
  currentSessionId: string | null
  onExpand: () => void
}> = ({ sessions, currentSessionId, onExpand }) => (
  <div className="flex flex-col items-center gap-2 p-2">
    {/* Show only icons with tooltip */}
    {sessions.slice(0, 5).map(session => (
      <TooltipProvider key={session.session_id}>
        <Tooltip>
          <TooltipTrigger asChild>
            <Button
              variant={session.session_id === currentSessionId ? "default" : "ghost"}
              size="icon"
              className="h-10 w-10"
              onClick={() => selectSession(session.session_id)}
              aria-label={`Select ${session.session_name}`}
            >
              <MessageSquare className="h-5 w-5" />
            </Button>
          </TooltipTrigger>
          <TooltipContent side="right">
            <p>{session.session_name}</p>
            <p className="text-xs text-muted-foreground">
              {getRelativeTime(session.updated_at)}
            </p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    ))}
    
    {/* Expand button */}
    <Button
      variant="ghost"
      size="icon"
      onClick={onExpand}
      aria-label="Show all sessions"
    >
      <ChevronRight className="h-5 w-5" />
    </Button>
  </div>
)
```

## Performance Optimization

### Virtualization for Large Lists

```tsx
import { useVirtualizer } from '@tanstack/react-virtual'
import AutoSizer from 'react-virtualized-auto-sizer'

const VirtualizedSessionList: React.FC<{
  groupedSessions: GroupedSessions
}> = ({ groupedSessions }) => {
  const items = React.useMemo(() => {
    const result: (ChatSessionIndexEntry | { type: 'header', label: string })[] = []
    
    if (groupedSessions.today.length > 0) {
      result.push({ type: 'header', label: 'Today' })
      result.push(...groupedSessions.today)
    }
    
    if (groupedSessions.recent.length > 0) {
      result.push({ type: 'header', label: 'Recent' })
      result.push(...groupedSessions.recent)
    }
    
    if (groupedSessions.past.length > 0) {
      result.push({ type: 'header', label: 'Past Sessions' })
      result.push(...groupedSessions.past)
    }
    
    return result
  }, [groupedSessions])
  
  const getItemSize = (index: number) => {
    const item = items[index]
    return item && 'type' in item && item.type === 'header' ? 32 : 72
  }
  
  const Row = ({ index, style }: { index: number; style: React.CSSProperties }) => {
    const item = items[index]
    
    if (!item) return null
    
    if ('type' in item && item.type === 'header') {
      return (
        <div style={style}>
          <SessionGroupHeader label={item.label} />
        </div>
      )
    }
    
    return (
      <div style={style}>
        <SessionItem session={item as ChatSessionIndexEntry} {...sessionItemProps} />
      </div>
    )
  }
  
  return (
    <AutoSizer>
      {({ height, width }) => (
        <List
          height={height}
          itemCount={items.length}
          itemSize={getItemSize}
          width={width}
          overscanCount={3}
        >
          {Row}
        </List>
      )}
    </AutoSizer>
  )
}
```

### Memoization Strategies

```tsx
// Memoize expensive computations
const filteredSessions = React.useMemo(() => {
  if (!searchQuery) return sessions
  
  const query = searchQuery.toLowerCase()
  return sessions.filter(session => {
    const name = (session.session_name || '').toLowerCase()
    const agent = session.agent_name.toLowerCase()
    return name.includes(query) || agent.includes(query)
  })
}, [sessions, searchQuery])

const groupedSessions = React.useMemo(() => 
  groupSessionsByTime(filteredSessions),
  [filteredSessions]
)

// Memoize components to prevent re-renders
const MemoizedSessionItem = React.memo(SessionItem, (prev, next) => {
  return (
    prev.session.session_id === next.session.session_id &&
    prev.session.updated_at === next.session.updated_at &&
    prev.isActive === next.isActive &&
    prev.isDeleting === next.isDeleting
  )
})
```

### Debouncing & Throttling

```tsx
// Debounce search input
const debouncedSearch = React.useMemo(
  () => debounce((value: string) => {
    setSearchQuery(value)
  }, 300),
  []
)

// Throttle scroll events for infinite loading
const throttledScroll = React.useMemo(
  () => throttle(() => {
    if (!scrollAreaRef.current) return
    
    const { scrollTop, scrollHeight, clientHeight } = scrollAreaRef.current
    const scrollPercentage = (scrollTop + clientHeight) / scrollHeight
    
    if (scrollPercentage > 0.8 && hasMore && !isPaginationLoading) {
      loadMoreSessions()
    }
  }, 150),
  [hasMore, isPaginationLoading]
)
```

## Implementation Roadmap

### Phase 1: Core Functionality (Week 1)
- [ ] Replace stub with functional component
- [ ] Implement basic session display
- [ ] Add time-based grouping
- [ ] Implement session selection
- [ ] Add active session highlighting

### Phase 2: Search & Filter (Week 1-2)
- [ ] Add search input component
- [ ] Implement filtering logic
- [ ] Add debouncing for search
- [ ] Handle empty search states
- [ ] Add search result highlighting

### Phase 3: Infinite Scroll & Pagination (Week 2)
- [ ] Implement intersection observer
- [ ] Add loading indicators
- [ ] Handle pagination events
- [ ] Implement scroll position preservation
- [ ] Add error recovery for failed loads

### Phase 4: User Actions (Week 2-3)
- [ ] Implement delete functionality
- [ ] Add confirmation dialog
- [ ] Handle delete events
- [ ] Add optimistic updates
- [ ] Implement error recovery

### Phase 5: Real-time Updates (Week 3)
- [ ] Subscribe to session events
- [ ] Handle ChatSessionChangedEvent
- [ ] Handle ChatSessionNameChangedEvent
- [ ] Handle ChatSessionAddedEvent
- [ ] Handle ChatSessionDeletedEvent

### Phase 6: Polish & Optimization (Week 3-4)
- [ ] Add virtualization for large lists
- [ ] Implement proper memoization
- [ ] Add comprehensive error handling
- [ ] Polish animations and transitions
- [ ] Complete accessibility testing

### Phase 7: Testing & Documentation (Week 4)
- [ ] Unit tests for all components
- [ ] Integration tests with SDK
- [ ] Accessibility audit
- [ ] Performance profiling
- [ ] Documentation updates

## Testing Checklist

### Functional Testing
- [ ] Sessions display with correct information
- [ ] Time grouping works correctly
- [ ] Search filters sessions properly
- [ ] Infinite scroll loads more sessions
- [ ] Session selection triggers ResumeChatSessionEvent
- [ ] Delete confirmation and execution work
- [ ] Real-time updates reflect immediately
- [ ] Error states display appropriately
- [ ] Empty states show correct messages

### Visual Testing
- [ ] Component matches design specifications
- [ ] All color tokens from CenSuite are used
- [ ] Spacing follows 4px base unit system
- [ ] Typography hierarchy is correct
- [ ] Focus states are clearly visible
- [ ] Hover states work as expected
- [ ] Active session highlighting is clear
- [ ] Loading animations are smooth

### Accessibility Testing
- [ ] Keyboard navigation works completely
- [ ] Screen reader announces all changes
- [ ] ARIA attributes are properly set
- [ ] Color contrast meets WCAG AA
- [ ] Focus trap works in dialogs
- [ ] Touch targets are minimum 44x44px
- [ ] Error messages are associated with actions

### Performance Testing
- [ ] Handles 100+ sessions smoothly
- [ ] Handles 500+ sessions with virtualization
- [ ] Search doesn't cause lag
- [ ] Scroll performance remains smooth
- [ ] Memory usage stays reasonable
- [ ] No unnecessary re-renders

### Edge Cases
- [ ] Empty session list
- [ ] Single session
- [ ] Sessions with long names
- [ ] Sessions with no names
- [ ] Rapid delete actions
- [ ] Network failures during operations
- [ ] Concurrent updates from events

## Code Examples

### Complete Hook Implementation

```tsx
// hooks/useChatSessionList.ts
export const useChatSessionList = () => {
  const client = useRealtimeClient()
  const [state, dispatch] = React.useReducer(sessionListReducer, initialState)
  
  // Load initial sessions
  React.useEffect(() => {
    const loadInitial = async () => {
      dispatch({ type: 'LOADING_START' })
      
      try {
        await client.send({
          type: 'get_user_sessions',
          offset: 0,
          limit: 50
        })
      } catch (error) {
        dispatch({ type: 'ERROR', payload: error })
      }
    }
    
    loadInitial()
  }, [client])
  
  // Subscribe to events
  React.useEffect(() => {
    const handlers = {
      get_user_sessions_response: (event: GetUserSessionsResponseEvent) => {
        // Note: event.sessions is ChatSessionQueryResponse type
        dispatch({ 
          type: 'SESSIONS_LOADED', 
          payload: {
            sessions: event.sessions.chat_sessions,  // Extract the array
            totalSessions: event.sessions.total_sessions,
            offset: event.sessions.offset
          }
        })
      },
      
      chat_session_changed: (event: ChatSessionChangedEvent) => {
        dispatch({ 
          type: 'SESSION_UPDATED', 
          payload: event 
        })
      },
      
      chat_session_deleted: (event: ChatSessionDeletedEvent) => {
        dispatch({ 
          type: 'SESSION_DELETED', 
          payload: event.session_id 
        })
      }
    }
    
    Object.entries(handlers).forEach(([event, handler]) => {
      client.on(event, handler)
    })
    
    return () => {
      Object.entries(handlers).forEach(([event, handler]) => {
        client.off(event, handler)
      })
    }
  }, [client])
  
  const selectSession = React.useCallback((sessionId: string) => {
    client.send({
      type: 'resume_chat_session',
      session_id: sessionId
    })
  }, [client])
  
  const deleteSession = React.useCallback((sessionId: string) => {
    dispatch({ type: 'DELETE_OPTIMISTIC', payload: sessionId })
    
    client.send({
      type: 'delete_chat_session',
      session_id: sessionId
    }).catch(() => {
      dispatch({ type: 'DELETE_REVERT', payload: sessionId })
    })
  }, [client])
  
  const loadMore = React.useCallback(() => {
    if (state.isPaginationLoading || !state.hasMore) return
    
    client.send({
      type: 'get_user_sessions',
      offset: state.sessions.length,
      limit: 50
    })
  }, [client, state])
  
  return {
    ...state,
    selectSession,
    deleteSession,
    loadMore
  }
}
```

### Utility Functions

```tsx
// utils/sessionHelpers.ts

export const groupSessionsByTime = (sessions: ChatSessionIndexEntry[]): GroupedSessions => {
  const now = new Date()
  const today = startOfDay(now)
  const twoWeeksAgo = subDays(today, 14)
  
  const groups: GroupedSessions = {
    today: [],
    recent: [],
    past: []
  }
  
  sessions.forEach(session => {
    const updatedAt = new Date(session.updated_at)
    
    if (updatedAt >= today) {
      groups.today.push(session)
    } else if (updatedAt >= twoWeeksAgo) {
      groups.recent.push(session)
    } else {
      groups.past.push(session)
    }
  })
  
  return groups
}

export const getRelativeTime = (timestamp: string): string => {
  const date = new Date(timestamp)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)
  
  if (diffMins < 1) return 'Just now'
  if (diffMins < 60) return `${diffMins}m ago`
  if (diffHours < 24) return `${diffHours}h ago`
  if (diffDays < 7) return `${diffDays}d ago`
  if (diffDays < 30) return `${Math.floor(diffDays / 7)}w ago`
  
  return format(date, 'MMM d')
}

export const deriveSessionName = (session: ChatSessionIndexEntry): string => {
  if (session.session_name) {
    return session.session_name
  }
  
  // Note: last_message_preview not available in ChatSessionIndexEntry
  // Would need to be added server-side or fetched separately
  
  return `Chat from ${format(new Date(session.created_at), 'MMM d, h:mm a')}`
}
```

## Summary

The ChatSessionList component represents a cornerstone of the user experience, providing intuitive access to conversation history with enterprise-grade performance and accessibility. By following CenSuite design principles and implementing comprehensive state management, the component delivers a polished, responsive interface that scales from mobile to desktop while maintaining excellent performance with hundreds of sessions.

Key achievements:
- **Design System Compliance**: Strict adherence to CenSuite tokens and patterns
- **Accessibility**: Full WCAG 2.1 AA compliance with comprehensive keyboard navigation
- **Performance**: Virtualization and memoization for smooth scrolling with 500+ sessions
- **User Experience**: Intuitive interactions with clear visual feedback
- **Real-time Updates**: Seamless event-driven synchronization
- **Responsive Design**: Adaptive layouts for all screen sizes

This design ensures the ChatSessionList component meets production requirements while providing a foundation for future enhancements.