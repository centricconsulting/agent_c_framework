'use client'

import * as React from 'react'
import { Volume2, VolumeX } from 'lucide-react'
import { cn } from '../../lib/utils'
import { Button } from '../ui/button'
import { useAudio } from '@agentc/realtime-react'

export interface MuteToggleProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  /**
   * Enable keyboard shortcuts
   */
  enableShortcut?: boolean
  /**
   * Require confirmation for unmute
   */
  requireConfirmation?: boolean
  /**
   * Show audio level indicator
   */
  showAudioIndicator?: boolean
  /**
   * Button size
   */
  size?: 'small' | 'default' | 'large'
}

export const MuteToggle = React.forwardRef<HTMLButtonElement, MuteToggleProps>(
  ({ 
    className, 
    enableShortcut = false,
    requireConfirmation = false,
    showAudioIndicator = false,
    size = 'default',
    disabled,
    ...props 
  }, ref) => {
    const { isMuted, toggleMute, isRecording, audioLevel } = useAudio()
    const [showConfirm, setShowConfirm] = React.useState(false)
    
    const isDisabled = disabled || isRecording
    
    const handleToggle = React.useCallback(() => {
      if (requireConfirmation && isMuted) {
        setShowConfirm(true)
      } else {
        toggleMute()
      }
    }, [isMuted, requireConfirmation, toggleMute])
    
    const handleConfirm = React.useCallback(() => {
      toggleMute()
      setShowConfirm(false)
    }, [toggleMute])
    
    // Keyboard shortcut handler
    React.useEffect(() => {
      if (!enableShortcut) return
      
      const handleKeyPress = (e: KeyboardEvent) => {
        if ((e.ctrlKey || e.metaKey) && e.key === 'm') {
          e.preventDefault()
          handleToggle()
        }
      }
      
      window.addEventListener('keydown', handleKeyPress)
      return () => window.removeEventListener('keydown', handleKeyPress)
    }, [enableShortcut, handleToggle])
    
    const buttonSize = {
      small: 'h-8 w-8',
      default: 'h-10 w-10',
      large: 'h-12 w-12'
    }[size]
    
    return (
      <div className="relative" aria-live="polite">
        <Button
          ref={ref}
          type="button"
          variant="ghost"
          size="icon"
          className={cn(
            buttonSize,
            isMuted && 'text-muted-foreground',
            showAudioIndicator && audioLevel > 0 && 'animate-pulse',
            className
          )}
          onClick={handleToggle}
          disabled={isDisabled}
          aria-label={isMuted ? 'Unmute audio' : 'Mute audio'}
          aria-pressed={isMuted}
          {...props}
        >
          {isMuted ? (
            <VolumeX className="h-4 w-4" />
          ) : (
            <Volume2 className="h-4 w-4" />
          )}
        </Button>
        
        {showAudioIndicator && !isMuted && audioLevel > 0 && (
          <span className="absolute -top-1 -right-1 h-2 w-2 bg-green-500 rounded-full animate-pulse" />
        )}
        
        {/* Confirmation Dialog */}
        {showConfirm && (
          <div 
            role="dialog"
            className="absolute top-full mt-2 p-4 bg-background border rounded-lg shadow-lg z-50"
          >
            <p className="text-sm mb-3">Unmute audio?</p>
            <div className="flex gap-2">
              <Button
                size="sm"
                onClick={handleConfirm}
              >
                Confirm
              </Button>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => setShowConfirm(false)}
              >
                Cancel
              </Button>
            </div>
          </div>
        )}
        
        {/* Screen reader announcement */}
        <span className="sr-only" aria-live="polite">
          Audio is {isMuted ? 'muted' : 'unmuted'}
        </span>
      </div>
    )
  }
)

MuteToggle.displayName = 'MuteToggle'