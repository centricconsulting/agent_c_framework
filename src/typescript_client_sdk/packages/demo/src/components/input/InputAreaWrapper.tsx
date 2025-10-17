"use client"

import * as React from "react"
import dynamic from 'next/dynamic'
import { cn } from "@/lib/utils"
import { Loader2 } from "lucide-react"
import type { InputAreaProps } from "@agentc/realtime-ui"

/**
 * Loading fallback component that matches the InputArea interface
 * Provides a skeleton UI while the actual component loads
 */
const InputAreaSkeleton: React.FC<InputAreaProps> = ({ className, maxHeight = "200px" }) => {
  return (
    <div className={cn("w-full", className)}>
      <div 
        className={cn(
          "flex flex-col gap-2 p-4 border rounded-lg bg-background/50",
          "animate-pulse"
        )}
        style={{ minHeight: maxHeight }}
      >
        {/* Editor skeleton */}
        <div className="flex-1 min-h-[60px] rounded bg-muted/30" />
        
        {/* Toolbar skeleton */}
        <div className="flex items-center justify-between gap-2 pt-2 border-t">
          <div className="flex items-center gap-2">
            {/* Agent selector skeleton */}
            <div className="h-8 w-32 rounded bg-muted/30" />
            
            {/* Output mode skeleton */}
            <div className="h-8 w-24 rounded bg-muted/30" />
          </div>
          
          <div className="flex items-center gap-2">
            {/* Mic button skeleton */}
            <div className="h-8 w-8 rounded-full bg-muted/30" />
            
            {/* Send button skeleton */}
            <div className="h-8 w-20 rounded bg-muted/30" />
          </div>
        </div>
        
        {/* Loading indicator */}
        <div className="flex items-center justify-center gap-2 text-xs text-muted-foreground mt-2">
          <Loader2 className="h-3 w-3 animate-spin" />
          <span>Loading editor...</span>
        </div>
      </div>
    </div>
  )
}

/**
 * Dynamic import of InputArea with SSR disabled
 * This prevents TipTap from attempting to access browser APIs during server-side rendering
 */
const DynamicInputArea = dynamic<InputAreaProps>(
  () => import('@agentc/realtime-ui').then(mod => ({ default: mod.InputArea })),
  {
    ssr: false,
    loading: () => <InputAreaSkeleton />
  }
)

/**
 * InputAreaWrapper - A Next.js-specific wrapper for the InputArea component
 * 
 * This wrapper solves the SSR incompatibility issue with TipTap v3 by:
 * 1. Using Next.js dynamic() to disable SSR for the InputArea component
 * 2. Providing a loading fallback that maintains the UI layout
 * 3. Passing through all props to the underlying InputArea component
 * 
 * This approach allows the UI package to remain framework-agnostic while
 * solving SSR issues at the consuming application level where Next.js is available.
 */
export const InputAreaWrapper = React.forwardRef<HTMLDivElement, InputAreaProps>(
  (props, ref) => {
    // Track if we're on the client side
    const [isClient, setIsClient] = React.useState(false)
    
    React.useEffect(() => {
      setIsClient(true)
    }, [])
    
    // Only render the dynamic component on the client
    if (!isClient) {
      return <InputAreaSkeleton {...props} />
    }
    
    return <DynamicInputArea {...props} />
  }
)

InputAreaWrapper.displayName = "InputAreaWrapper"

export default InputAreaWrapper