"use client"

import * as React from "react"
import { forwardRef, useCallback, useState, useEffect } from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { cn } from "../../lib/utils"
import { Mic, MicOff, Loader2 } from "lucide-react"
import { Button } from "../ui/button"
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "../ui/tooltip"

const microphoneButtonVariants = cva(
  "relative inline-flex items-center justify-center rounded-full transition-all",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/90",
        outline: "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        secondary: "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ghost: "hover:bg-accent hover:text-accent-foreground",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
      },
      size: {
        default: "h-10 w-10",
        sm: "h-8 w-8",
        lg: "h-12 w-12",
        xl: "h-14 w-14",
      },
      state: {
        idle: "",
        recording: "ring-2 ring-destructive ring-offset-2 bg-destructive text-destructive-foreground",
        loading: "opacity-70 cursor-wait",
        error: "border-destructive",
      }
    },
    defaultVariants: {
      variant: "default",
      size: "default",
      state: "idle",
    },
  }
)

export interface MicrophoneButtonProps
  extends Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'onClick'>,
    VariantProps<typeof microphoneButtonVariants> {
  isRecording?: boolean
  onStartRecording?: () => void | Promise<void>
  onStopRecording?: () => void | Promise<void>
  audioLevel?: number // 0-1 for visualization
  showTooltip?: boolean
  tooltipSide?: 'top' | 'right' | 'bottom' | 'left'
  disabled?: boolean
  loading?: boolean
  error?: string | null
}

const MicrophoneButton = forwardRef<HTMLButtonElement, MicrophoneButtonProps>(
  ({ 
    className, 
    variant = "default",
    size = "default",
    state: propState,
    isRecording = false,
    onStartRecording,
    onStopRecording,
    audioLevel = 0,
    showTooltip = true,
    tooltipSide = 'top',
    disabled = false,
    loading = false,
    error = null,
    ...props 
  }, ref) => {
    const [isLoading, setIsLoading] = useState(false)
    const [animationScale, setAnimationScale] = useState(1)

    // Determine current state
    const currentState = propState || (
      loading || isLoading ? 'loading' :
      error ? 'error' :
      isRecording ? 'recording' : 
      'idle'
    )

    // Handle click
    const handleClick = useCallback(async () => {
      if (disabled || isLoading) return

      try {
        setIsLoading(true)
        
        if (isRecording) {
          await onStopRecording?.()
        } else {
          await onStartRecording?.()
        }
      } catch (err) {
        console.error('Microphone action failed:', err)
      } finally {
        setIsLoading(false)
      }
    }, [isRecording, onStartRecording, onStopRecording, disabled, isLoading])

    // Animate based on audio level when recording
    useEffect(() => {
      if (isRecording && audioLevel > 0) {
        const scale = 1 + (audioLevel * 0.2) // Scale up to 20% based on audio level
        setAnimationScale(scale)
      } else {
        setAnimationScale(1)
      }
    }, [isRecording, audioLevel])

    // Icon to display
    const Icon = isLoading || loading ? Loader2 : isRecording ? MicOff : Mic

    // Tooltip content
    const tooltipContent = error ? error :
      isRecording ? "Stop recording" : 
      "Start recording"

    const button = (
      <Button
        ref={ref}
        className={cn(
          microphoneButtonVariants({ 
            variant: isRecording ? 'destructive' : variant, 
            size,
            state: currentState,
            className 
          })
        )}
        onClick={handleClick}
        disabled={disabled}
        aria-label={isRecording ? 'Stop recording' : 'Start recording'}
        aria-pressed={isRecording}
        {...props}
      >
        {/* Audio level visualization ring */}
        {isRecording && audioLevel > 0 && (
          <span
            className="absolute inset-0 rounded-full bg-current opacity-20 animate-ping"
            style={{
              animationDuration: `${(1.5 - audioLevel) * 1000}ms`,
              transform: `scale(${animationScale})`
            } as React.CSSProperties}
          />
        )}
        
        {/* Icon */}
        <Icon 
          className={cn(
            "h-5 w-5",
            size === 'sm' && "h-4 w-4",
            size === 'lg' && "h-6 w-6",
            size === 'xl' && "h-7 w-7",
            (isLoading || loading) && "animate-spin"
          )}
        />
        
        {/* Recording indicator dot */}
        {isRecording && (
          <span className="absolute top-0 right-0 flex h-3 w-3">
            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-red-400 opacity-75" />
            <span className="relative inline-flex rounded-full h-3 w-3 bg-red-500" />
          </span>
        )}
      </Button>
    )

    if (!showTooltip) {
      return button
    }

    return (
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            {button}
          </TooltipTrigger>
          <TooltipContent side={tooltipSide}>
            <p>{tooltipContent}</p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    )
  }
)

MicrophoneButton.displayName = "MicrophoneButton"

export { MicrophoneButton, microphoneButtonVariants }