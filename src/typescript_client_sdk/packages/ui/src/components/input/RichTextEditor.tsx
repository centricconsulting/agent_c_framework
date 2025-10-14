"use client"

import * as React from "react"
import { cn } from "../../lib/utils"
import { MarkdownEditorClient } from "../editor/MarkdownEditorClient"
import { useTurnState } from "@agentc/realtime-react"

/**
 * RichTextEditor component
 * 
 * A wrapper around MarkdownEditor that integrates with the Agent C Realtime SDK.
 * Handles submission logic, turn management, and state synchronization.
 * 
 * Features:
 * - Integrates with SDK chat functionality for message submission
 * - Respects turn management (disabled when it's not the user's turn)
 * - Clears content after successful submission
 * - Enter key submits (Shift+Enter for new line)
 * - Full markdown support through underlying MarkdownEditor
 */

export interface RichTextEditorProps {
  /**
   * The current value of the editor
   */
  value: string
  
  /**
   * Callback fired when the editor content changes
   */
  onChange: (value: string) => void
  
  /**
   * Callback fired when the user submits the content
   * Called after the message is sent to the SDK
   */
  onSubmit: () => void
  
  /**
   * Callback fired when user pastes content
   * Used for handling file pastes
   */
  onPaste?: (e: React.ClipboardEvent) => void
  
  /**
   * Placeholder text shown when the editor is empty
   * @default "Type a message..."
   */
  placeholder?: string
  
  /**
   * Whether the editor is disabled
   * When not provided, automatically determined by turn state
   */
  disabled?: boolean
  
  /**
   * Maximum height for the editor
   * @default "max-h-96"
   */
  maxHeight?: string
  
  /**
   * Additional CSS classes to apply to the editor
   */
  className?: string
}

const RichTextEditor = React.forwardRef<HTMLDivElement, RichTextEditorProps>(
  ({ 
    value, 
    onChange, 
    onSubmit,
    onPaste,
    placeholder = "Type a message...",
    disabled,
    maxHeight = "max-h-96",
    className,
    ...props 
  }, ref) => {
    // SDK hooks
    const { canSendInput, hasTurnManager } = useTurnState()
    
    // State for internal management
    const [isSubmitting, setIsSubmitting] = React.useState(false)
    const [submitError, setSubmitError] = React.useState<string | null>(null)
    
    // Determine if editor should be disabled
    const isDisabled = React.useMemo(() => {
      // If explicitly disabled, use that
      if (disabled !== undefined) {
        return disabled
      }
      
      // If there's a turn manager, respect turn state
      if (hasTurnManager) {
        return !canSendInput || isSubmitting
      }
      
      // Otherwise, only disable during submission
      return isSubmitting
    }, [disabled, hasTurnManager, canSendInput, isSubmitting])
    
    // Handle submission - delegate actual sending to parent
    const handleSubmit = React.useCallback(async (text?: string) => {
      // Use provided text or current value
      const messageText = text || value
      
      // Don't submit empty messages
      if (!messageText?.trim()) {
        return
      }
      
      // Don't submit if disabled
      if (isDisabled) {
        return
      }
      
      setIsSubmitting(true)
      setSubmitError(null)
      
      try {
        // NOTE: Parent component (InputArea) handles the actual sendMessage call
        // We only handle editor-specific logic here
        
        // Clear the editor on submission
        onChange("")
        
        // Call the onSubmit callback to notify parent
        onSubmit()
      } catch (err) {
        // Handle submission error
        const errorMessage = err instanceof Error ? err.message : "Failed to submit message"
        setSubmitError(errorMessage)
        console.error("Failed to submit message:", err)
      } finally {
        setIsSubmitting(false)
      }
    }, [value, isDisabled, onChange, onSubmit])
    
    // Handle Enter key for submission (without Shift)
    const handleKeyDown = React.useCallback((event: KeyboardEvent) => {
      // Check if Enter was pressed without Shift
      if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault()
        handleSubmit()
        return true
      }
      return false
    }, [handleSubmit])
    
    // Create editor props for MarkdownEditor
    const editorProps = React.useMemo(() => ({
      value,
      onChange,
      placeholder,
      disabled: isDisabled,
      onSubmit: handleSubmit,
      className: cn(
        // Apply max height with scrolling
        maxHeight,
        "overflow-y-auto",
        // Visual feedback for disabled state
        isDisabled && "opacity-60 cursor-not-allowed",
        // Error state styling
        submitError && "border-destructive",
        className
      )
    }), [value, onChange, placeholder, isDisabled, handleSubmit, maxHeight, className, submitError])
    
    return (
      <div className="relative w-full">
        <MarkdownEditorClient
          ref={ref}
          {...editorProps}
          onKeyDown={handleKeyDown}
          onPaste={onPaste}
          {...props}
        />
        
        {/* Error message display */}
        {submitError && (
          <div 
            className="mt-2 text-sm text-destructive"
            role="alert"
            aria-live="polite"
          >
            {submitError}
          </div>
        )}
        
        {/* Visual indicator when it's not the user's turn */}
        {hasTurnManager && !canSendInput && !disabled && (
          <div 
            className="mt-2 text-sm text-muted-foreground"
            role="status"
            aria-live="polite"
          >
            Wait for your turn to speak...
          </div>
        )}
        
        {/* Loading indicator during submission */}
        {isSubmitting && (
          <div 
            className="absolute inset-0 flex items-center justify-center bg-background/50 rounded-md"
            aria-label="Sending message"
            role="status"
          >
            <div className="h-4 w-4 animate-spin rounded-full border-2 border-primary border-t-transparent" />
          </div>
        )}
      </div>
    )
  }
)

RichTextEditor.displayName = "RichTextEditor"

export { RichTextEditor }