"use client"

import * as React from "react"
import { 
  MessageSquare, 
  Search,
  Trash2,
  Loader2,
  AlertCircle,
  ChevronRight
} from "lucide-react"
import { cn } from "../../lib/utils"
import { Input } from "../ui/input"
import { Button, buttonVariants } from "../ui/button"
import { Skeleton } from "../ui/skeleton"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "../ui/tooltip"
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
import { useChatSessionList } from "@agentc/realtime-react"
import type { ChatSessionIndexEntry } from "@agentc/realtime-core"
import type { SessionGroupMeta } from "@agentc/realtime-react"
import { useVirtualizer } from '@tanstack/react-virtual'

/**
 * Props for the ChatSessionList component
 */
export interface ChatSessionListProps extends React.HTMLAttributes<HTMLDivElement> {
  /** Whether the sidebar is collapsed */
  isCollapsed?: boolean
  /** Additional className for styling */
  className?: string
  /** Callback when a session is selected */
  onSessionSelect?: (sessionId: string) => void
  /** Whether to auto-load sessions on mount */
  autoLoad?: boolean
}

/**
 * Virtual list item data structure
 */
interface VirtualListItem {
  type: 'header' | 'session'
  data: SessionGroupMeta | ChatSessionIndexEntry
  groupId?: string
  groupLabel?: string
  sessionIndex?: number
  globalIndex: number
}

/**
 * Calculate item heights for virtual scrolling
 */
const HEADER_HEIGHT = 40
const SESSION_ITEM_HEIGHT = 68 // Two rows with padding

/**
 * Flatten session groups into a single array for virtual scrolling
 */
function flattenSessionGroups(sessionGroups: SessionGroupMeta[]): VirtualListItem[] {
  const items: VirtualListItem[] = []
  let globalIndex = 0
  
  sessionGroups.forEach(group => {
    // Add header
    items.push({
      type: 'header',
      data: group,
      groupId: group.group,
      groupLabel: group.label,
      globalIndex: globalIndex++
    })
    
    // Add sessions
    group.sessions.forEach((session, idx) => {
      items.push({
        type: 'session',
        data: session,
        groupId: group.group,
        sessionIndex: idx,
        globalIndex: globalIndex++
      })
    })
  })
  
  return items
}

/**
 * Get item size for variable size list
 */
function getItemSize(index: number, items: VirtualListItem[]): number {
  const item = items[index]
  if (!item) return SESSION_ITEM_HEIGHT
  return item.type === 'header' ? HEADER_HEIGHT : SESSION_ITEM_HEIGHT
}

/**
 * Function to format relative time for session timestamps
 */
function getRelativeTime(timestamp: string | null | undefined): string {
  if (!timestamp) return 'Unknown'
  
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
  if (diffDays < 30) return `${Math.floor(diffDays / 7)}w`
  
  // Format as month/day for older sessions
  return `${date.getMonth() + 1}/${date.getDate()}`
}

/**
 * Function to get display name for a session
 */
function getSessionDisplayName(session: ChatSessionIndexEntry): string {
  if (session.session_name) {
    return session.session_name
  }
  
  const date = new Date(session.created_at || 0)
  const hours = date.getHours()
  const minutes = date.getMinutes()
  const ampm = hours >= 12 ? 'PM' : 'AM'
  const displayHours = hours % 12 || 12
  const displayMinutes = minutes.toString().padStart(2, '0')
  
  return `Chat from ${date.getMonth() + 1}/${date.getDate()}, ${displayHours}:${displayMinutes} ${ampm}`
}

/**
 * Memoized Search/Filter bar component
 */
const SearchFilterBar = React.memo<{
  value: string
  onChange: (value: string) => void
  onClear?: () => void
  isSearching?: boolean
}>(({ value, onChange, onClear, isSearching }) => {
  const searchInputRef = React.useRef<HTMLInputElement>(null)
  
  // Memoized event handler for escape key
  const handleKeyDown = React.useCallback((e: KeyboardEvent) => {
    if (e.key === 'Escape' && value) {
      e.preventDefault()
      onClear?.()
      searchInputRef.current?.focus()
    }
  }, [value, onClear])
  
  // Handle escape key to clear search
  React.useEffect(() => {
    const input = searchInputRef.current
    if (input) {
      input.addEventListener('keydown', handleKeyDown)
      return () => input.removeEventListener('keydown', handleKeyDown)
    }
  }, [handleKeyDown])
  
  // Memoized change handler
  const handleChange = React.useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.value)
  }, [onChange])
  
  return (
    <div className="bg-background p-2 border-b">
      <div className="relative">
        {isSearching ? (
          <Loader2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 animate-spin text-muted-foreground" />
        ) : (
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        )}
        <Input
          ref={searchInputRef}
          type="search"
          placeholder="Search sessions..."
          value={value}
          onChange={handleChange}
          className="pl-9 h-9 text-sm"
          aria-label="Search sessions"
        />
      </div>
    </div>
  )
})
SearchFilterBar.displayName = 'SearchFilterBar'

/**
 * Memoized Session group header component
 */
const SessionGroupHeader = React.memo<{
  label: string
  count: number
  groupId: string
  onHeaderClick?: () => void
}>(({ label, count, groupId, onHeaderClick }) => (
  <div 
    className={cn(
      "flex items-center gap-2 px-3 py-2",
      "bg-background sticky top-0 z-10", // Make headers sticky
      "border-b border-t",
      "cursor-pointer hover:bg-muted/50 transition-colors"
    )}
    onClick={onHeaderClick}
    role="heading"
    aria-level={3}
    id={`header-${groupId}`}
  >
    <span className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
      {label}
    </span>
    <span className="text-xs text-muted-foreground">({count})</span>
    <div className="flex-1 h-px bg-border/50" aria-hidden="true" />
  </div>
))
SessionGroupHeader.displayName = 'SessionGroupHeader'

/**
 * Memoized Delete confirmation dialog component
 */
const DeleteSessionDialog = React.memo<{
  session: ChatSessionIndexEntry | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
  isDeleting?: boolean
}>(({ session, open, onOpenChange, onConfirm, isDeleting }) => {
  const sessionName = React.useMemo(
    () => session ? getSessionDisplayName(session) : '',
    [session]
  )
  
  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete Session?</AlertDialogTitle>
          <AlertDialogDescription>
            Are you sure you want to delete &apos;{sessionName}&apos;? This action cannot be undone.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={isDeleting}>Cancel</AlertDialogCancel>
          <AlertDialogAction
            onClick={onConfirm}
            disabled={isDeleting}
            className={cn(
              buttonVariants({ variant: "destructive" }),
              isDeleting && "opacity-50 cursor-wait"
            )}
          >
            {isDeleting ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Deleting...
              </>
            ) : (
              'Delete'
            )}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
})
DeleteSessionDialog.displayName = 'DeleteSessionDialog'

/**
 * Memoized Individual session item component
 */
export const SessionItem = React.memo<{
  session: ChatSessionIndexEntry
  isActive: boolean
  isFocused?: boolean
  onSelect: () => void
  onDelete: () => void
  onFocus?: () => void
  isDeleting?: boolean
  index?: number
  totalCount?: number
}>(({ session, isActive, isFocused, onSelect, onDelete, onFocus, isDeleting, index, totalCount }) => {
  const displayName = React.useMemo(() => getSessionDisplayName(session), [session])
  const timeAgo = React.useMemo(() => getRelativeTime(session.updated_at ?? session.created_at), [session.updated_at, session.created_at])
  const agentDisplay = React.useMemo(() => session.agent_name || session.agent_key || 'Unknown Agent', [session.agent_name, session.agent_key])
  
  // Memoized keyboard handler
  const handleKeyDown = React.useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault()
      onSelect()
    } else if (e.key === 'Delete') {
      e.preventDefault()
      onDelete()
    }
  }, [onSelect, onDelete])
  
  // Memoized delete click handler
  const handleDeleteClick = React.useCallback((e: React.MouseEvent) => {
    e.stopPropagation()
    onDelete()
  }, [onDelete])
  
  return (
    <div
      className={cn(
        "group relative px-3 py-3",
        "transition-all duration-200 cursor-pointer",
        "min-h-[44px]",
        "hover:bg-muted/60",
        "focus-visible:outline-none",
        isActive && "bg-accent",
        isFocused && "ring-2 ring-ring ring-offset-1",
        isDeleting && "opacity-50 pointer-events-none",
        "motion-safe:transition-all motion-reduce:transition-none"
      )}
      onClick={onSelect}
      onFocus={onFocus}
      onKeyDown={handleKeyDown}
      tabIndex={isFocused ? 0 : -1}
      role="option"
      aria-selected={isActive}
      aria-label={`Session ${index !== undefined ? index + 1 : ''} of ${totalCount || ''}: ${displayName} with ${agentDisplay}, last updated ${timeAgo}`}
      aria-posinset={index !== undefined ? index + 1 : undefined}
      aria-setsize={totalCount}
    >
      {/* Active indicator */}
      {isActive && (
        <div 
          className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-8 bg-primary rounded-r-full" 
          aria-hidden="true" 
        />
      )}
      
      {/* Session content - Two-row layout */}
      <div className="flex flex-col gap-1">
        {/* Row 1: Session name - full width */}
        <p className="font-medium text-sm leading-tight">
          {displayName}
        </p>
        
        {/* Row 2: Agent name on left | Age + Delete button on right */}
        <div className="flex items-center justify-between gap-2">
          {/* Left side: Agent name */}
          <p className="text-xs text-muted-foreground truncate">
            {agentDisplay}
          </p>
          
          {/* Right side: Age + Delete button */}
          <div className="flex items-center gap-1 flex-shrink-0">
            <span className="text-xs text-muted-foreground whitespace-nowrap">
              {timeAgo}
            </span>
            
            {/* Delete button */}
            <Button
              variant="ghost"
              size="icon"
              className={cn(
                "h-6 w-6 flex-shrink-0 -mr-1",
                "opacity-0 group-hover:opacity-100 group-focus-within:opacity-100",
                "transition-opacity",
                "focus-visible:opacity-100"
              )}
              onClick={handleDeleteClick}
              tabIndex={isFocused ? 0 : -1}
              aria-label={`Delete session: ${displayName}`}
            >
              <Trash2 className="h-3 w-3" aria-hidden="true" />
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}, (prevProps, nextProps) => {
  // Custom comparison function to ensure isActive changes always trigger re-render
  // This is critical for highlighting the correct session
  return (
    prevProps.session.session_id === nextProps.session.session_id &&
    prevProps.isActive === nextProps.isActive &&  // MUST re-render when isActive changes
    prevProps.isFocused === nextProps.isFocused &&
    prevProps.isDeleting === nextProps.isDeleting &&
    prevProps.index === nextProps.index
  )
})
SessionItem.displayName = 'SessionItem'

/**
 * Memoized Empty state component
 */
const EmptyState = React.memo<{
  searchQuery?: string
}>(({ searchQuery }) => (
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
))
EmptyState.displayName = 'EmptyState'

/**
 * Memoized Loading skeleton component
 */
const LoadingSkeleton = React.memo(() => (
  <div className="space-y-3 p-3">
    {[...Array(5)].map((_, i) => (
      <div key={i} className="space-y-2">
        <Skeleton className="h-16 w-full rounded-lg" />
      </div>
    ))}
  </div>
))
LoadingSkeleton.displayName = 'LoadingSkeleton'

/**
 * Memoized Error state component
 */
const ErrorState = React.memo<{
  error: Error
  onRetry: () => void
}>(({ error, onRetry }) => (
  <div className="flex flex-col items-center justify-center py-12 px-4">
    <div className="rounded-full bg-destructive/10 p-4 mb-4">
      <AlertCircle className="h-6 w-6 text-destructive" />
    </div>
    <h3 className="text-base font-semibold text-foreground mb-2">
      Failed to load sessions
    </h3>
    <p className="text-sm text-muted-foreground text-center max-w-[200px] mb-4">
      {error.message || 'An unexpected error occurred'}
    </p>
    <Button 
      variant="outline" 
      size="sm"
      onClick={onRetry}
    >
      Try again
    </Button>
  </div>
))
ErrorState.displayName = 'ErrorState'

/**
 * Memoized Collapsed view component
 */
const CollapsedView = React.memo<{
  sessions: ChatSessionIndexEntry[]
  currentSessionId: string | null
  onSessionSelect: (sessionId: string) => void
  onExpand?: () => void
}>(({ sessions, currentSessionId, onSessionSelect, onExpand }) => {
  // Memoize the top 5 sessions
  const topSessions = React.useMemo(() => sessions.slice(0, 5), [sessions])
  
  return (
    <div className="flex flex-col items-center gap-2 p-2">
      <TooltipProvider>
        {topSessions.map(session => (
          <Tooltip key={session.session_id}>
            <TooltipTrigger asChild>
              <Button
                variant={session.session_id === currentSessionId ? "default" : "ghost"}
                size="icon"
                className="h-10 w-10"
                onClick={() => onSessionSelect(session.session_id)}
                aria-label={`Select ${getSessionDisplayName(session)}`}
              >
                <MessageSquare className="h-5 w-5" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="right">
              <p>{getSessionDisplayName(session)}</p>
              <p className="text-xs text-muted-foreground">
                {getRelativeTime(session.updated_at ?? session.created_at)}
              </p>
            </TooltipContent>
          </Tooltip>
        ))}
        
        {sessions.length > 5 && (
          <Tooltip>
            <TooltipTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="h-10 w-10"
                onClick={onExpand}
                aria-label="Show all sessions"
              >
                <ChevronRight className="h-5 w-5" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="right">
              <p>Show all sessions</p>
            </TooltipContent>
          </Tooltip>
        )}
      </TooltipProvider>
    </div>
  )
})
CollapsedView.displayName = 'CollapsedView'

/**
 * Virtual Session List Component with @tanstack/react-virtual
 * Efficiently renders large lists with virtual scrolling
 */
const VirtualSessionList = React.memo<{
  sessionGroups: SessionGroupMeta[]
  currentSessionId: string | null
  focusedIndex: number
  handleSessionSelect: (sessionId: string) => void
  handleDeleteRequest: (session: ChatSessionIndexEntry) => void
  setFocusedIndex: (index: number) => void
  deletingId: string | null
  filteredSessions: ChatSessionIndexEntry[]
  hasMore: boolean
  isPaginationLoading: boolean
  loadMore: () => void
  scrollContainerRef: React.RefObject<HTMLDivElement>
}>(({ 
  sessionGroups, 
  currentSessionId, 
  focusedIndex, 
  handleSessionSelect, 
  handleDeleteRequest, 
  setFocusedIndex, 
  deletingId, 
  filteredSessions,
  hasMore,
  isPaginationLoading,
  loadMore,
  scrollContainerRef
}) => {
  // Flatten groups for virtual scrolling
  const items = React.useMemo(
    () => flattenSessionGroups(sessionGroups),
    [sessionGroups]
  )
  
  // Initialize virtualizer
  const virtualizer = useVirtualizer({
    count: items.length,
    getScrollElement: () => scrollContainerRef.current,
    estimateSize: React.useCallback((index: number) => getItemSize(index, items), [items]),
    overscan: 5,
    measureElement: (element) => {
      if (element) {
        return element.getBoundingClientRect().height
      }
      return SESSION_ITEM_HEIGHT
    }
  })
  
  // Force virtualizer to measure after mount or when items change
  React.useEffect(() => {
    virtualizer.measure()
  }, [items.length, virtualizer])
  
  const virtualItems = virtualizer.getVirtualItems()
  
  // Scroll to focused item when it changes
  React.useEffect(() => {
    if (focusedIndex >= 0 && focusedIndex < items.length) {
      virtualizer.scrollToIndex(focusedIndex, { align: 'auto' })
    }
  }, [focusedIndex, virtualizer, items.length])
  
  // Handle infinite scroll
  React.useEffect(() => {
    const lastItem = virtualItems[virtualItems.length - 1]
    
    if (
      lastItem &&
      lastItem.index >= items.length - 1 &&
      hasMore &&
      !isPaginationLoading
    ) {
      loadMore()
    }
  }, [virtualItems, items.length, hasMore, isPaginationLoading, loadMore])
  
  return (
    <>
      <div
        style={{
          height: `${virtualizer.getTotalSize()}px`,
          width: '100%',
          position: 'relative'
        }}
      >
        {virtualItems.map((virtualItem) => {
          const item = items[virtualItem.index]
          if (!item) return null
          
          if (item.type === 'header') {
            const group = item.data as SessionGroupMeta
            return (
              <div
                key={virtualItem.key}
                data-index={virtualItem.index}
                ref={virtualizer.measureElement}
                style={{
                  position: 'absolute',
                  top: 0,
                  left: 0,
                  width: '100%',
                  transform: `translateY(${virtualItem.start}px)`
                }}
              >
                <SessionGroupHeader
                  label={group.label}
                  count={group.count}
                  groupId={group.group}
                  onHeaderClick={() => {
                    virtualizer.scrollToIndex(virtualItem.index, { align: 'start' })
                  }}
                />
              </div>
            )
          }
          
          const session = item.data as ChatSessionIndexEntry
          const sessionGlobalIndex = items.findIndex(
            i => i.type === 'session' && (i.data as ChatSessionIndexEntry).session_id === session.session_id
          )
          
          return (
            <div
              key={virtualItem.key}
              data-index={virtualItem.index}
              ref={virtualizer.measureElement}
              style={{
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                transform: `translateY(${virtualItem.start}px)`
              }}
            >
              <SessionItem
                session={session}
                isActive={session.session_id === currentSessionId}
                isFocused={focusedIndex === sessionGlobalIndex}
                onSelect={() => handleSessionSelect(session.session_id)}
                onDelete={() => handleDeleteRequest(session)}
                onFocus={() => setFocusedIndex(sessionGlobalIndex)}
                isDeleting={deletingId === session.session_id}
                index={item.sessionIndex}
                totalCount={filteredSessions.length}
              />
            </div>
          )
        })}
      </div>
      
      {/* Load more trigger */}
      {hasMore && (
        <div className="flex items-center justify-center py-4 min-h-[60px]">
          {isPaginationLoading ? (
            <div className="flex items-center gap-2 text-sm text-muted-foreground" role="status" aria-live="polite">
              <Loader2 className="h-4 w-4 animate-spin" aria-hidden="true" />
              <span>Loading more sessions...</span>
            </div>
          ) : (
            <div className="text-sm text-muted-foreground opacity-50">
              <span>Scroll for more</span>
            </div>
          )}
        </div>
      )}
    </>
  )
})
VirtualSessionList.displayName = 'VirtualSessionList'

/**
 * ChatSessionList component with virtual scrolling as default
 * Efficiently renders large lists of chat sessions using @tanstack/react-virtual
 */
export const ChatSessionList = React.forwardRef<HTMLDivElement, ChatSessionListProps>(
  ({ isCollapsed, className, onSessionSelect, autoLoad = true, ...props }, ref) => {
    // Use the chat session list hook
    const {
      sessions,
      filteredSessions,
      sessionGroups,
      searchQuery,
      isLoading,
      isPaginationLoading,
      error,
      hasMore,
      currentSessionId,
      loadMore,
      selectSession,
      deleteSession,
      searchSessions,
      refresh
    } = useChatSessionList({ 
      autoLoad,
      pageSize: 50,
      searchDebounceMs: 300
    })
    
    // State for delete confirmation
    const [deleteDialogOpen, setDeleteDialogOpen] = React.useState(false)
    const [sessionToDelete, setSessionToDelete] = React.useState<ChatSessionIndexEntry | null>(null)
    const [deletingId, setDeletingId] = React.useState<string | null>(null)
    
    // State for keyboard navigation
    const [focusedIndex, setFocusedIndex] = React.useState(-1)
    
    // State for announcements
    const [announcement, setAnnouncement] = React.useState('')
    
    // Refs
    const scrollContainerRef = React.useRef<HTMLDivElement>(null)
    
    // Memoized session selection handler
    const handleSessionSelect = React.useCallback((sessionId: string) => {
      selectSession(sessionId)
      onSessionSelect?.(sessionId)
      setAnnouncement('Session selected')
    }, [selectSession, onSessionSelect])
    
    // Memoized delete request handler
    const handleDeleteRequest = React.useCallback((session: ChatSessionIndexEntry) => {
      setSessionToDelete(session)
      setDeleteDialogOpen(true)
    }, [])
    
    // Memoized delete confirmation handler
    const handleDeleteConfirm = React.useCallback(async () => {
      if (!sessionToDelete) return
      
      setDeletingId(sessionToDelete.session_id)
      setDeleteDialogOpen(false)
      
      try {
        await deleteSession(sessionToDelete.session_id)
        setAnnouncement('Session deleted successfully')
      } catch (error) {
        console.error('Failed to delete session:', error)
        setAnnouncement('Failed to delete session')
      } finally {
        setDeletingId(null)
        setSessionToDelete(null)
      }
    }, [sessionToDelete, deleteSession])
    
    // Memoized clear search handler
    const handleClearSearch = React.useCallback(() => {
      searchSessions('')
    }, [searchSessions])
    
    // Memoized keyboard navigation handler
    const handleKeyboardNavigation = React.useCallback((e: React.KeyboardEvent) => {
      const allSessions = filteredSessions
      
      if (allSessions.length === 0) return
      
      switch (e.key) {
        case 'ArrowDown':
          e.preventDefault()
          setFocusedIndex(prev => {
            const next = prev < allSessions.length - 1 ? prev + 1 : prev
            return next
          })
          break
          
        case 'ArrowUp':
          e.preventDefault()
          setFocusedIndex(prev => {
            const next = prev > 0 ? prev - 1 : 0
            return next
          })
          break
          
        case 'Home':
          e.preventDefault()
          setFocusedIndex(0)
          scrollContainerRef.current?.scrollTo({ top: 0, behavior: 'smooth' })
          break
          
        case 'End':
          e.preventDefault()
          setFocusedIndex(allSessions.length - 1)
          scrollContainerRef.current?.scrollTo({ 
            top: scrollContainerRef.current.scrollHeight, 
            behavior: 'smooth' 
          })
          break
          
        case 'Enter':
        case ' ':
          if (focusedIndex >= 0 && focusedIndex < allSessions.length) {
            e.preventDefault()
            handleSessionSelect(allSessions[focusedIndex].session_id)
          }
          break
          
        case 'Delete':
          if (focusedIndex >= 0 && focusedIndex < allSessions.length) {
            e.preventDefault()
            handleDeleteRequest(allSessions[focusedIndex])
          }
          break
      }
    }, [filteredSessions, focusedIndex, handleSessionSelect, handleDeleteRequest])
    
    // When collapsed, hide the session list completely
    if (isCollapsed) {
      return null
    }
    
    // Main expanded view
    return (
      <div
        ref={ref}
        className={cn("flex flex-col h-full", className)}
        {...props}
      >
        {/* Search bar */}
        <SearchFilterBar
          value={searchQuery}
          onChange={searchSessions}
          onClear={handleClearSearch}
          isSearching={false}
        />
        
        {/* Main scrollable content area with virtual scrolling */}
        <div 
          ref={scrollContainerRef}
          className="flex-1 overflow-y-auto"
          role="listbox"
          aria-label="Chat sessions"
          onKeyDown={handleKeyboardNavigation}
          tabIndex={0}
        >
          {/* Loading state */}
          {isLoading && sessions.length === 0 && (
            <LoadingSkeleton />
          )}
          
          {/* Error state */}
          {error && sessions.length === 0 && (
            <ErrorState error={error} onRetry={refresh} />
          )}
          
          {/* Empty state */}
          {!isLoading && !error && filteredSessions.length === 0 && (
            <EmptyState searchQuery={searchQuery} />
          )}
          
          {/* Virtual Session list - DEFAULT AND ONLY IMPLEMENTATION */}
          {!isLoading && sessionGroups.length > 0 && (
            <VirtualSessionList
              sessionGroups={sessionGroups}
              currentSessionId={currentSessionId}
              focusedIndex={focusedIndex}
              handleSessionSelect={handleSessionSelect}
              handleDeleteRequest={handleDeleteRequest}
              setFocusedIndex={setFocusedIndex}
              deletingId={deletingId}
              filteredSessions={filteredSessions}
              hasMore={hasMore}
              isPaginationLoading={isPaginationLoading}
              loadMore={loadMore}
              scrollContainerRef={scrollContainerRef}
            />
          )}
        </div>
        
        {/* Delete confirmation dialog */}
        <DeleteSessionDialog
          session={sessionToDelete}
          open={deleteDialogOpen}
          onOpenChange={setDeleteDialogOpen}
          onConfirm={handleDeleteConfirm}
          isDeleting={!!deletingId}
        />
        
        {/* Screen reader announcements */}
        <div 
          className="sr-only" 
          role="status" 
          aria-live="assertive" 
          aria-atomic="true"
        >
          {announcement}
        </div>
      </div>
    )
  }
)
ChatSessionList.displayName = "ChatSessionList"