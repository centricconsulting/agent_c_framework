import * as React from "react"
import { cn } from "../../lib/utils"

/**
 * InputContainer component
 * 
 * A flexible container for input components with consistent spacing and styling.
 * Provides visual focus states and proper layout structure for the input area.
 */

export interface InputContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Child components to render inside the container
   */
  children: React.ReactNode
  
  /**
   * Maximum height for the container with scrolling
   * @default "max-h-96"
   */
  maxHeight?: string
  
  /**
   * Additional CSS classes to apply to the container
   */
  className?: string
}

const InputContainer = React.forwardRef<HTMLDivElement, InputContainerProps>(
  ({ children, className, maxHeight = "max-h-96", ...props }, ref) => {
    return (
      <div
        ref={ref}
        className={cn(
          // Base flex container with proper spacing
          "flex flex-col gap-3.5 p-3.5",
          // Rounded corners and background styling (no border)
          "rounded-xl bg-background",
          // Focus-within state for child focus indication
          "focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2",
          // Smooth transitions for focus states
          "transition-all duration-200",
          // Apply max height with overflow handling
          maxHeight,
          "overflow-y-auto",
          // Custom classes
          className
        )}
        {...props}
      >
        {children}
      </div>
    )
  }
)

InputContainer.displayName = "InputContainer"

export { InputContainer }