'use client'

import * as React from "react"
import { AudioWaveform } from "lucide-react"
import { cn } from "../../lib/utils"
import { useAudio } from "@agentc/realtime-react"

export interface VoiceVisualizerViewProps {
  className?: string
}

/**
 * VoiceVisualizerView component - PLACEHOLDER implementation
 * Three.js voice visualizer integration will be added later
 */
export const VoiceVisualizerView = React.forwardRef<HTMLDivElement, VoiceVisualizerViewProps>(
  ({ className }, ref) => {
    const { isRecording, audioLevel } = useAudio()

    return (
      <div
        ref={ref}
        className={cn(
          "flex items-center justify-center h-full",
          "bg-gradient-to-b from-background to-muted/20",
          className
        )}
      >
        <div className="text-center space-y-4">
          <div className="relative">
            <AudioWaveform 
              className={cn(
                "h-16 w-16 mx-auto text-primary transition-all duration-200",
                isRecording && "animate-pulse"
              )}
              style={{
                opacity: isRecording ? 0.5 + (audioLevel * 0.5) : 0.5,
                transform: `scale(${1 + (audioLevel * 0.2)})`
              }}
            />
          </div>
          <div>
            <p className="text-lg font-medium">Voice Mode Active</p>
            <p className="text-sm text-muted-foreground">
              {isRecording ? "Listening..." : "Visualizer integration coming soon"}
            </p>
          </div>
          {/* Simple audio level indicator */}
          <div className="w-48 h-2 bg-muted rounded-full overflow-hidden mx-auto">
            <div 
              className="h-full bg-primary transition-all duration-100"
              style={{ width: `${audioLevel * 100}%` }}
            />
          </div>
        </div>
      </div>
    )
  }
)
VoiceVisualizerView.displayName = "VoiceVisualizerView"