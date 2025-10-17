"use client"

/**
 * InputToolbar Component
 * 
 * Provides the toolbar for the input area with microphone controls,
 * attachment, agent selection, and send functionality.
 * 
 * Layout according to design doc:
 * [Attachments] [Tools] [OutputSelector] ... [Microphone] ... [AgentSelector] [Send]
 */

import * as React from "react"
import { cn } from "../../lib/utils"
import { Button } from "../ui/button"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "../ui/tooltip"
import { 
  Paperclip, 
  Wrench, 
  Send,
  Square
} from "lucide-react"
import { useRealtimeClient, useTurnState } from "@agentc/realtime-react"

// Import actual components
import { MicrophoneButton } from "./MicrophoneButton"
import { OutputSelector } from "../controls/OutputSelector"  // Use the fixed version from controls
import { AgentSelector } from "../controls/AgentSelector"  // Use the proper version from controls

export interface InputToolbarProps {
  /** Callback when send button is clicked */
  onSend: () => void
  /** Whether the send button should be enabled */
  canSend: boolean
  /** Whether audio is currently being recorded */
  isRecording: boolean
  /** Callback to start audio recording */
  onStartRecording: () => void
  /** Callback to stop audio recording */
  onStopRecording: () => void
  /** Current audio level (0-100) for visualization */
  audioLevel?: number
  /** Callback for attachment button */
  onAttachment?: () => void
  /** Whether attachment button should be disabled */
  disableAttachment?: boolean
  /** Callback for tools button (optional, placeholder) */
  onTools?: () => void
  /** @deprecated List of available agents - AgentSelector now uses SDK data */
  agents?: any[]
  /** @deprecated Currently selected agent - AgentSelector now uses SDK state */
  selectedAgent?: any
  /** @deprecated Callback when agent selection changes - AgentSelector now uses SDK events */
  onAgentChange?: (agent: any) => void
  /** Additional CSS classes */
  className?: string
}

/**
 * Audio level indicator component
 * Shows 5 bars that light up based on audio level
 */
const AudioLevelIndicator: React.FC<{ level: number; className?: string }> = ({ 
  level, 
  className 
}) => {
  // Calculate how many bars should be active (each bar = 20%)
  const activeBars = Math.ceil((level / 100) * 5)
  
  return (
    <div 
      className={cn("flex items-end gap-0.5 h-5", className)}
      role="img"
      aria-label={`Audio level: ${level}%`}
    >
      {[1, 2, 3, 4, 5].map((bar) => (
        <div
          key={bar}
          className={cn(
            "w-1 rounded-full transition-all duration-150",
            bar <= activeBars 
              ? "bg-primary" 
              : "bg-muted",
            // Variable heights for visual appeal
            bar === 1 && "h-2",
            bar === 2 && "h-3",
            bar === 3 && "h-4",
            bar === 4 && "h-3",
            bar === 5 && "h-2"
          )}
          aria-hidden="true"
        />
      ))}
    </div>
  )
}

/**
 * Attachment button
 */
const AttachmentButton: React.FC<{ onClick?: () => void; disabled?: boolean }> = ({ onClick, disabled }) => (
  <Button
    variant="ghost"
    size="icon"
    onClick={onClick}
    aria-label="Add attachment"
    disabled={disabled}
  >
    <Paperclip className="h-4 w-4" />
  </Button>
)

/**
 * Tools button (placeholder with "Coming Soon" tooltip)
 */
const ToolsButton: React.FC<{ onClick?: () => void }> = ({ onClick }) => (
  <TooltipProvider>
    <Tooltip>
      <TooltipTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          onClick={onClick}
          disabled
          aria-label="Tools (Coming Soon)"
        >
          <Wrench className="h-4 w-4" />
        </Button>
      </TooltipTrigger>
      <TooltipContent>
        <p>Coming Soon</p>
      </TooltipContent>
    </Tooltip>
  </TooltipProvider>
)



/**
 * InputToolbar Component
 * 
 * Main toolbar with three sections:
 * - Left: Attachments, Tools, OutputSelector (controls how AGENT responds)
 * - Center: Microphone controls with audio level
 * - Right: AgentSelector (which agent to talk to) and Send/Cancel button
 */
export const InputToolbar = React.forwardRef<HTMLDivElement, InputToolbarProps>(
  ({ 
    onSend,
    canSend,
    isRecording,
    onStartRecording,
    onStopRecording,
    audioLevel = 0,
    onAttachment,
    disableAttachment,
    onTools,
    agents,  // @deprecated - AgentSelector now manages its own state via SDK
    selectedAgent,  // @deprecated - AgentSelector now manages its own state via SDK
    onAgentChange,  // @deprecated - AgentSelector now manages its own state via SDK
    className,
    ...props 
  }, ref) => {
    // Get the realtime client and turn state
    const client = useRealtimeClient();
    const { canSendInput } = useTurnState();
    
    // Track cancellation state
    const [isCancelled, setIsCancelled] = React.useState(false);
    
    // Check if cancel functionality is available
    const hasCancelSupport = React.useMemo(() => {
      return client && typeof (client as any).cancelResponse === 'function';
    }, [client]);
    
    // Listen for response-cancelled event
    React.useEffect(() => {
      if (!client) return;
      
      const handleResponseCancelled = () => {
        setIsCancelled(true);
      };
      
      // Reset cancelled state when turn changes to user
      const handleTurnChange = () => {
        if (canSendInput) {
          setIsCancelled(false);
        }
      };
      
      // Listen for the response-cancelled event
      // Note: This event may come from SessionManager or EventStreamProcessor
      const sessionManager = client.getSessionManager();
      if (sessionManager) {
        // Try to listen for the event, but handle gracefully if not available
        try {
          (sessionManager as any).on('response-cancelled', handleResponseCancelled);
        } catch (e) {
          // Event may not be available yet
          console.debug('response-cancelled event not available on SessionManager');
        }
      }
      
      // Also try listening directly on the client for the event
      try {
        (client as any).on('response-cancelled', handleResponseCancelled);
      } catch (e) {
        // Event may not be available yet
        console.debug('response-cancelled event not available on client');
      }
      
      // Clean up when turn changes
      handleTurnChange();
      
      return () => {
        if (sessionManager) {
          try {
            (sessionManager as any).off('response-cancelled', handleResponseCancelled);
          } catch (e) {
            // Ignore cleanup errors
          }
        }
        try {
          (client as any).off('response-cancelled', handleResponseCancelled);
        } catch (e) {
          // Ignore cleanup errors  
        }
      };
    }, [client, canSendInput]);
    
    // Determine button state
    const isAgentResponding = !canSendInput;
    const showCancelButton = hasCancelSupport && (isAgentResponding || isCancelled);
    
    // Handle button click
    const handleButtonClick = React.useCallback(() => {
      if (showCancelButton && hasCancelSupport) {
        // Cancel the agent response
        console.debug(`[${Date.now()}] Cancel button clicked in UI`);
        try {
          (client as any).cancelResponse();
          console.debug(`[${Date.now()}] cancelResponse() called from UI`);
        } catch (e) {
          console.error(`[${Date.now()}] Failed to cancel response:`, e);
        }
      } else {
        // Send the message
        onSend();
      }
    }, [showCancelButton, hasCancelSupport, client, onSend]);
    
    // Determine button variant and animation
    const buttonVariant = showCancelButton ? "destructive" : "default";
    const shouldPulse = isAgentResponding && !isCancelled && hasCancelSupport;
    return (
      <div
        ref={ref}
        className={cn(
          "flex items-center gap-2.5 w-full px-4 py-2 border-t border-border",
          className
        )}
        {...props}
      >
        {/* Left Section - Attachment, Tools, OutputSelector */}
        <div className="flex items-center gap-1">
          <AttachmentButton onClick={onAttachment} disabled={disableAttachment} />
          <ToolsButton onClick={onTools} />
          
          {/* OutputSelector - Controls how AGENT responds (text/voice/avatar) */}
          <OutputSelector className="ml-1" />
        </div>

        {/* Center Section - Microphone Controls (NOT a mode toggle) */}
        <div className="flex-1 flex justify-center items-center gap-2">
          <MicrophoneButton
            isRecording={isRecording}
            onStartRecording={onStartRecording}
            onStopRecording={onStopRecording}
            audioLevel={audioLevel}
            size="default"
            showTooltip={true}
          />
          
          {/* Audio level indicator - only visible when recording */}
          {isRecording && (
            <AudioLevelIndicator 
              level={audioLevel} 
              className="ml-2"
            />
          )}
        </div>

        {/* Right Section - Agent Selector and Send */}
        <div className="flex items-center gap-2">
          {/* AgentSelector - Choose which agent to talk to */}
          <AgentSelector
            className="min-w-[150px] max-w-[200px]"
          />
          
          {/* Send/Cancel Button */}
          <Button
            variant={buttonVariant}
            size="icon"
            onClick={handleButtonClick}
            disabled={!showCancelButton && !canSend}
            aria-label={showCancelButton ? "Cancel response" : "Send message"}
            className={cn(
              shouldPulse && "animate-pulse"
            )}
          >
            {showCancelButton ? (
              <Square className="h-4 w-4" />
            ) : (
              <Send className="h-4 w-4" />
            )}
          </Button>
        </div>
      </div>
    )
  }
)

InputToolbar.displayName = "InputToolbar"