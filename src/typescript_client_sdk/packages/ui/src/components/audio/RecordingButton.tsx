'use client'

import * as React from 'react'
import { Mic, MicOff, Loader2 } from 'lucide-react'
import { cn } from '../../lib/utils'
import { Button } from '../ui/button'
import { useAudio, useConnection, getConnectionStateString } from '@agentc/realtime-react'

export interface RecordingButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  /**
   * Show tooltip on hover
   */
  showTooltip?: boolean
  /**
   * Button size variant
   */
  size?: 'small' | 'default' | 'large'
  /**
   * Custom icon component
   */
  icon?: React.ReactNode
}

export const RecordingButton = React.forwardRef<HTMLButtonElement, RecordingButtonProps>(
  ({ className, showTooltip, size = 'default', icon, disabled, ...props }, ref) => {
    const { isRecording, startRecording, stopRecording } = useAudio()
    const { isConnected, connectionState } = useConnection()
    const [error, setError] = React.useState<string | null>(null)
    
    const connectionStateString = getConnectionStateString(connectionState)
    const isDisabled = disabled || !isConnected || connectionStateString === 'connecting'
    
    const handleClick = React.useCallback(async () => {
      setError(null)
      try {
        if (isRecording) {
          await stopRecording()
        } else {
          // Check for microphone availability
          if (!navigator.mediaDevices?.getUserMedia) {
            setError('Microphone not available')
            return
          }
          await startRecording()
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Recording failed')
      }
    }, [isRecording, startRecording, stopRecording])
    
    const buttonSize = {
      small: 'h-8 w-8',
      default: 'h-10 w-10',
      large: 'h-12 w-12'
    }[size]
    
    return (
      <>
        <Button
          ref={ref}
          type="button"
          variant={isRecording ? 'destructive' : 'default'}
          size="icon"
          className={cn(
            buttonSize,
            isRecording && 'animate-pulse',
            className
          )}
          onClick={handleClick}
          disabled={isDisabled}
          aria-label={isRecording ? 'Stop recording' : 'Start recording'}
          aria-pressed={isRecording}
          aria-busy={connectionStateString === 'connecting'}
          {...props}
        >
          {connectionStateString === 'connecting' ? (
            <>
              <Loader2 className="h-4 w-4 animate-spin" />
              <span className="sr-only" role="status">Connecting...</span>
            </>
          ) : icon ? (
            icon
          ) : isRecording ? (
            <MicOff className="h-4 w-4" />
          ) : (
            <Mic className="h-4 w-4" />
          )}
        </Button>
        
        {error && (
          <div role="alert" className="text-xs text-destructive mt-1">
            {error}
          </div>
        )}
        
        {showTooltip && (
          <div role="tooltip" className="sr-only">
            {isRecording ? 'Click to stop recording' : 'Click to start recording'}
          </div>
        )}
      </>
    )
  }
)

RecordingButton.displayName = 'RecordingButton'