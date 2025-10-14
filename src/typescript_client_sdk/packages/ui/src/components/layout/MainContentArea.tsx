"use client"

import * as React from "react"
import { cn } from "../../lib/utils"
import { ChatMessagesView } from "../chat/ChatMessagesView"
import { AvatarDisplayView } from "../avatar/AvatarDisplayView"
import { VoiceVisualizerView } from "../audio/VoiceVisualizerView"

export type OutputMode = 'chat' | 'avatar' | 'voice'

export interface MainContentAreaProps {
  outputMode: OutputMode
  className?: string
}

/**
 * MainContentArea component - Container that switches between display modes
 * Renders different views based on the selected output mode
 */
export const MainContentArea = React.forwardRef<HTMLDivElement, MainContentAreaProps>(
  ({ outputMode, className }, ref) => {
    return (
      <div
        ref={ref}
        className={cn(
          "h-full overflow-hidden relative",
          className
        )}
      >
        {outputMode === 'chat' && <ChatMessagesView className="h-full" />}
        {outputMode === 'avatar' && <AvatarDisplayView />}
        {outputMode === 'voice' && <VoiceVisualizerView />}
      </div>
    )
  }
)
MainContentArea.displayName = "MainContentArea"