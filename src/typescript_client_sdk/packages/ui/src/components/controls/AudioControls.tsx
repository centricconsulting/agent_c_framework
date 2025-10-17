"use client"

import * as React from "react"
import { cn } from "../../lib/utils"
import { useAudio } from "@agentc/realtime-react"
import { Mic, MicOff, Volume2 } from "lucide-react"
import { Button } from "../ui/button"

export interface AudioControlsProps {
  size?: 'sm' | 'default' | 'lg'
  showLabel?: boolean
  showLevelIndicator?: boolean
  className?: string
}

/**
 * AudioControls component - Manages audio recording state
 * Provides visual feedback for audio levels and recording status
 */
const AudioControls = React.forwardRef<HTMLDivElement, AudioControlsProps>(
  ({ size = 'default', showLabel = true, showLevelIndicator = true, className }, ref) => {
    const { isRecording, isStreaming, audioLevel, startRecording, stopRecording, setVolume, status } = useAudio()
    const [volume, setVolumeState] = React.useState(1)

    const handleRecordingToggle = React.useCallback(async () => {
      try {
        if (isRecording) {
          stopRecording()
        } else {
          await startRecording()
        }
      } catch (error) {
        console.error('Failed to toggle recording:', error)
      }
    }, [isRecording, startRecording, stopRecording])

    const handleVolumeToggle = React.useCallback(() => {
      const newVolume = volume > 0 ? 0 : 1
      setVolumeState(newVolume)
      setVolume(newVolume)
    }, [volume, setVolume])

    return (
      <div
        ref={ref}
        className={cn(
          "inline-flex items-center gap-2 rounded-md border border-border bg-background p-1",
          className
        )}
        aria-label="Audio controls"
      >
        {/* Recording Button */}
        <Button
          variant={isRecording ? "default" : "ghost"}
          size={size === 'sm' ? 'icon' : size}
          onClick={handleRecordingToggle}
          className={cn(
            size === 'sm' ? "h-8 w-8" : "",
            isRecording && "bg-red-500 hover:bg-red-600"
          )}
          aria-label={isRecording ? "Stop recording" : "Start recording"}
          aria-pressed={isRecording}
        >
          {isRecording ? (
            <Mic className="h-4 w-4" />
          ) : (
            <MicOff className="h-4 w-4" />
          )}
          {showLabel && size !== 'sm' && (
            <span className="ml-2">{isRecording ? "Recording" : "Record"}</span>
          )}
        </Button>

        {/* Volume Button */}
        <Button
          variant="ghost"
          size={size === 'sm' ? 'icon' : size}
          onClick={handleVolumeToggle}
          className={cn(
            size === 'sm' ? "h-8 w-8" : "",
            volume === 0 && "text-muted-foreground"
          )}
          aria-label={volume === 0 ? "Unmute" : "Mute"}
          aria-pressed={volume === 0}
        >
          <Volume2 className="h-4 w-4" />
          {showLabel && size !== 'sm' && (
            <span className="ml-2">{volume === 0 ? "Unmuted" : "Muted"}</span>
          )}
        </Button>

        {/* Audio Level Indicator */}
        {showLevelIndicator && isRecording && (
          <div 
            className="h-1.5 w-16 bg-primary/20 rounded-full overflow-hidden"
            role="progressbar"
            aria-label="Audio level"
            aria-valuenow={Math.round(audioLevel * 100)}
            aria-valuemin={0}
            aria-valuemax={100}
          >
            <div 
              className="h-full bg-primary transition-all duration-100"
              style={{ width: `${audioLevel * 100}%` }}
            />
          </div>
        )}
      </div>
    )
  }
)
AudioControls.displayName = "AudioControls"

export { AudioControls }