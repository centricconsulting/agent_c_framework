"use client"

import * as React from "react"
import { Loader2 } from "lucide-react"
import { cn } from "../../lib/utils"
import { useAvatar } from "@agentc/realtime-react"

export interface AvatarDisplayViewProps {
  className?: string
}

/**
 * AvatarDisplayView component - HeyGen avatar display
 * Shows the avatar video stream when in avatar mode
 */
export const AvatarDisplayView = React.forwardRef<HTMLDivElement, AvatarDisplayViewProps>(
  ({ className }, ref) => {
    const videoRef = React.useRef<HTMLVideoElement>(null)
    const { avatarSession, isAvatarActive, isLoading, error } = useAvatar()
    const [streamReady, setStreamReady] = React.useState(false)

    // Note: The actual HeyGen stream integration will need to be handled
    // through the HeyGen SDK once the avatar session is established.
    // For now, this is a placeholder for the video display.
    React.useEffect(() => {
      // Store ref in a variable to avoid issues in cleanup
      const video = videoRef.current
      
      // This would be where we integrate with HeyGen SDK
      // to get the actual video stream
      setStreamReady(false)
      
      if (isAvatarActive && avatarSession) {
        // Simulate stream ready after a delay
        const timer = setTimeout(() => {
          setStreamReady(true)
        }, 1000)
        
        return () => {
          clearTimeout(timer)
          // Clean up video element if needed
          if (video && video.srcObject) {
            video.srcObject = null
          }
        }
      }
      
      return () => {
        // Clean up video element if needed
        if (video && video.srcObject) {
          video.srcObject = null
        }
      }
    }, [isAvatarActive, avatarSession])

    return (
      <div
        ref={ref}
        className={cn(
          "flex flex-col items-center justify-center h-full p-4",
          className
        )}
      >
        {/* Video Container */}
        <div className="relative w-full max-w-[900px] aspect-video overflow-hidden rounded-xl bg-zinc-900">
          {/* Video Element */}
          <video
            ref={videoRef}
            autoPlay
            playsInline
            muted={false}
            className="w-full h-full object-contain"
            style={{ objectFit: 'contain' }}
            aria-label="Avatar video stream"
          />

          {/* Connection Quality Indicator */}
          {isAvatarActive && streamReady && (
            <div className="absolute top-3 left-3 flex items-center gap-2 bg-background/80 backdrop-blur-sm rounded-full px-3 py-1">
              <div className="h-2 w-2 rounded-full bg-green-500" aria-hidden="true" />
              <span className="text-xs">Connected</span>
            </div>
          )}

          {/* Loading Overlay */}
          {(isLoading || (isAvatarActive && !streamReady)) && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/50">
              <div className="flex flex-col items-center gap-2">
                <Loader2 className="h-8 w-8 animate-spin" />
                <span className="text-sm text-muted-foreground">Initializing avatar...</span>
              </div>
            </div>
          )}

          {/* Error State */}
          {error && (
            <div className="absolute inset-0 flex items-center justify-center">
              <div className="text-center space-y-2">
                <p className="text-sm text-destructive">{error}</p>
              </div>
            </div>
          )}

          {/* No Avatar State */}
          {!isAvatarActive && !isLoading && !error && (
            <div className="absolute inset-0 flex items-center justify-center">
              <div className="text-center space-y-2">
                <div className="p-4 rounded-full bg-muted inline-block">
                  <svg
                    className="h-12 w-12 text-muted-foreground"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={1.5}
                      d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z"
                    />
                  </svg>
                </div>
                <p className="text-sm text-muted-foreground">Avatar mode selected</p>
                <p className="text-xs text-muted-foreground">Start chatting to see the avatar</p>
              </div>
            </div>
          )}
        </div>
      </div>
    )
  }
)
AvatarDisplayView.displayName = "AvatarDisplayView"