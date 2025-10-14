"use client"

import * as React from "react"
import { ChevronDown, Edit2, Trash2, AlertCircle } from "lucide-react"
import { useRealtimeClientSafe } from "@agentc/realtime-react"
import type { 
  ChatSessionChangedEvent, 
  ChatSessionNameChangedEvent,
  SetChatSessionNameEvent,
  DeleteChatSessionEvent
} from "@agentc/realtime-core"
import { cn } from "../../lib/utils"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"
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
import { Input } from "../ui/input"
import { Label } from "../ui/label"
import { toast } from "sonner"

export interface SessionNameDropdownProps {
  sessionName?: string
  className?: string
}

/**
 * SessionNameDropdown component
 * Displays current session name with dropdown menu for session management.
 * Integrates with the realtime client to:
 * - Display and update session name from server events
 * - Allow renaming the current session
 * - Allow deleting the current session with confirmation
 */
export const SessionNameDropdown = React.forwardRef<HTMLButtonElement, SessionNameDropdownProps>(
  ({ sessionName: propSessionName, className }, ref) => {
    const client = useRealtimeClientSafe()
    
    // State management
    const [currentSessionName, setCurrentSessionName] = React.useState<string>(propSessionName || "New Chat")
    const [sessionId, setSessionId] = React.useState<string | undefined>()
    const [isRenameOpen, setIsRenameOpen] = React.useState(false)
    const [isDeleteOpen, setIsDeleteOpen] = React.useState(false)
    const [newName, setNewName] = React.useState("")
    const [isProcessing, setIsProcessing] = React.useState(false)
    const [dropdownOpen, setDropdownOpen] = React.useState(false)
    
    // Use ref to track current sessionId for event handlers
    const sessionIdRef = React.useRef<string | undefined>()
    
    // Update from prop changes
    React.useEffect(() => {
      if (propSessionName) {
        setCurrentSessionName(propSessionName)
      }
    }, [propSessionName])
    
    // Keep ref in sync with state
    React.useEffect(() => {
      sessionIdRef.current = sessionId
    }, [sessionId])
    
    // Get initial session state on mount and listen for session events
    React.useEffect(() => {
      if (!client) return
      
      // Get the SessionManager
      const sessionManager = client.getSessionManager()
      if (!sessionManager) return
      
      // Get the current session from SessionManager on mount
      const currentSession = sessionManager.getCurrentSession()
      if (currentSession) {
        setSessionId(currentSession.session_id)
        sessionIdRef.current = currentSession.session_id
        const name = currentSession.session_name || currentSession.display_name || "New Chat"
        setCurrentSessionName(name)
      }
      
      // Handler for session changes from ChatSessionManager - this is the proper event to listen to
      // The EventStreamProcessor consumes chat_session_changed and the ChatSessionManager emits chat-session-changed
      const handleSessionChanged = (event: { previousChatSession?: any; currentChatSession?: any }) => {
        if (event.currentChatSession) {
          // Always update both sessionId and name when this event fires
          const newSessionId = event.currentChatSession.session_id
          const newName = event.currentChatSession.session_name || event.currentChatSession.display_name || "New Chat"
          
          // Update state unconditionally - this event means the session has changed
          setSessionId(newSessionId)
          sessionIdRef.current = newSessionId
          setCurrentSessionName(newName)
        }
      }
      
      // Handler for session name changes from client events
      const handleSessionNameChanged = (event: ChatSessionNameChangedEvent) => {
        // Update the name if it's for the current session or no session_id is provided
        if (!event.session_id || event.session_id === sessionIdRef.current) {
          setCurrentSessionName(event.session_name || "New Chat")
        }
      }
      
      // Subscribe to ChatSessionManager events for session changes
      sessionManager.on('chat-session-changed', handleSessionChanged)
      
      // Subscribe to client events for session name changes (these are still emitted directly)
      client.on('chat_session_name_changed', handleSessionNameChanged)
      
      // Cleanup
      return () => {
        sessionManager.off('chat-session-changed', handleSessionChanged)
        client.off('chat_session_name_changed', handleSessionNameChanged)
      }
    }, [client]) // Remove sessionId from dependencies to prevent re-subscription
    
    // Handle rename action
    const handleRename = React.useCallback(() => {
      // Open rename dialog
      setNewName(currentSessionName)
      setIsRenameOpen(true)
      setDropdownOpen(false)
    }, [currentSessionName])
    
    // Handle rename confirmation
    const handleRenameConfirm = React.useCallback(async () => {
      if (!client || !newName.trim()) {
        toast.error("Please enter a valid session name")
        return
      }
      
      try {
        setIsProcessing(true)
        
        // Send rename event
        const event: SetChatSessionNameEvent = {
          type: 'set_chat_session_name',
          session_name: newName.trim(),
          // session_id is optional - server uses current session if not provided
        }
        
        await client.sendEvent(event)
        
        // Update local state immediately for responsiveness
        setCurrentSessionName(newName.trim())
        setIsRenameOpen(false)
        toast.success("Session renamed successfully")
      } catch (error) {
        console.error('Failed to rename session:', error)
        toast.error("Failed to rename session")
      } finally {
        setIsProcessing(false)
      }
    }, [client, newName])
    
    // Handle delete action
    const handleDelete = React.useCallback(() => {
      // Open delete confirmation dialog
      setIsDeleteOpen(true)
      setDropdownOpen(false)
    }, [])
    
    // Handle delete confirmation
    const handleDeleteConfirm = React.useCallback(async () => {
      if (!client) {
        toast.error("Connection not available")
        return
      }
      
      try {
        setIsProcessing(true)
        
        // Send delete event
        const event: DeleteChatSessionEvent = {
          type: 'delete_chat_session',
          // session_id is optional - server uses current session if not provided
        }
        
        await client.sendEvent(event)
        
        setIsDeleteOpen(false)
        toast.success("Session deleted successfully")
        
        // The server should handle creating a new session or redirecting
        // We'll just reset the local state
        setCurrentSessionName("New Chat")
        setSessionId(undefined)
      } catch (error) {
        console.error('Failed to delete session:', error)
        toast.error("Failed to delete session")
      } finally {
        setIsProcessing(false)
      }
    }, [client])
    
    // Handle escape key for dialogs
    const handleRenameKeyDown = React.useCallback((e: React.KeyboardEvent) => {
      if (e.key === 'Enter' && !isProcessing) {
        e.preventDefault()
        handleRenameConfirm()
      }
    }, [handleRenameConfirm, isProcessing])
    
    return (
      <>
        <DropdownMenu open={dropdownOpen} onOpenChange={setDropdownOpen}>
          <DropdownMenuTrigger asChild>
            <button
              ref={ref}
              className={cn(
                "inline-flex items-center gap-2 px-3 py-1.5 rounded-md",
                "hover:bg-muted transition-colors",
                "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
                className
              )}
              aria-label="Session options"
              aria-expanded={dropdownOpen}
              aria-haspopup="menu"
            >
              <span className="text-sm font-medium truncate max-w-[200px]">
                {currentSessionName}
              </span>
              <ChevronDown className={cn(
                "h-4 w-4 text-muted-foreground transition-transform",
                dropdownOpen && "rotate-180"
              )} />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="center" className="w-56">
            <DropdownMenuItem 
              onClick={handleRename}
              disabled={!client || isProcessing}
              className="cursor-pointer"
            >
              <Edit2 className="mr-2 h-4 w-4" />
              <span>Rename session</span>
            </DropdownMenuItem>
            <DropdownMenuItem 
              onClick={handleDelete}
              disabled={!client || isProcessing}
              className="cursor-pointer text-destructive focus:text-destructive"
            >
              <Trash2 className="mr-2 h-4 w-4" />
              <span>Delete session</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
        
        {/* Rename Dialog */}
        <AlertDialog open={isRenameOpen} onOpenChange={setIsRenameOpen}>
          <AlertDialogContent className="sm:max-w-md">
            <AlertDialogHeader>
              <AlertDialogTitle>Rename Session</AlertDialogTitle>
              <AlertDialogDescription>
                Enter a new name for this chat session.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <div className="grid gap-4 py-4">
              <div className="grid gap-2">
                <Label htmlFor="session-name">
                  Session Name
                </Label>
                <Input
                  id="session-name"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  onKeyDown={handleRenameKeyDown}
                  placeholder="Enter session name"
                  disabled={isProcessing}
                  aria-label="New session name"
                  aria-describedby="session-name-description"
                  autoFocus
                />
                <span id="session-name-description" className="sr-only">
                  Type a descriptive name for your chat session
                </span>
              </div>
            </div>
            <AlertDialogFooter>
              <AlertDialogCancel disabled={isProcessing}>
                Cancel
              </AlertDialogCancel>
              <AlertDialogAction
                onClick={handleRenameConfirm}
                disabled={isProcessing || !newName.trim()}
              >
                {isProcessing ? "Renaming..." : "Rename"}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
        
        {/* Delete Confirmation Dialog */}
        <AlertDialog open={isDeleteOpen} onOpenChange={setIsDeleteOpen}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center gap-2">
                <AlertCircle className="h-5 w-5 text-destructive" />
                Delete Session
              </AlertDialogTitle>
              <AlertDialogDescription className="text-left">
                Are you sure you want to delete this chat session? 
                <br />
                <strong className="text-foreground">{currentSessionName}</strong>
                <br />
                <br />
                This action cannot be undone and will permanently remove all messages in this session.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel disabled={isProcessing}>
                Cancel
              </AlertDialogCancel>
              <AlertDialogAction
                onClick={handleDeleteConfirm}
                disabled={isProcessing}
                className={cn(
                  "bg-destructive text-destructive-foreground hover:bg-destructive/90"
                )}
              >
                {isProcessing ? "Deleting..." : "Delete Session"}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </>
    )
  }
)
SessionNameDropdown.displayName = "SessionNameDropdown"