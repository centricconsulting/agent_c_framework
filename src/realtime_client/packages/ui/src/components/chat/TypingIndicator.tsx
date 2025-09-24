import * as React from 'react'
import { cn } from '../../lib/utils'

export interface TypingIndicatorProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Animation variant
   */
  variant?: 'dots' | 'pulse' | 'wave'
  /**
   * Size of the indicator
   */
  size?: 'sm' | 'md' | 'lg'
  /**
   * Custom label for screen readers
   */
  label?: string
  /**
   * Animation speed in milliseconds
   */
  speed?: number
}

const TypingIndicator = React.forwardRef<HTMLDivElement, TypingIndicatorProps>(
  ({ 
    className,
    variant = 'dots',
    size = 'md',
    label = 'Assistant is typing',
    speed = 300,
    ...props 
  }, ref) => {
    
    // Size configurations
    const sizeClasses = {
      sm: {
        container: 'px-2 py-1',
        dot: 'h-1.5 w-1.5',
        gap: 'gap-1'
      },
      md: {
        container: 'px-3 py-2',
        dot: 'h-2 w-2',
        gap: 'gap-1.5'
      },
      lg: {
        container: 'px-4 py-3',
        dot: 'h-2.5 w-2.5',
        gap: 'gap-2'
      }
    }
    
    const currentSize = sizeClasses[size]
    
    // Render different variants
    const renderIndicator = () => {
      switch (variant) {
        case 'pulse':
          return (
            <div className="flex items-center gap-1">
              <div className={cn(
                "rounded-full bg-muted-foreground/60",
                currentSize.dot,
                "animate-pulse"
              )} />
              <div className={cn(
                "rounded-full bg-muted-foreground/40",
                currentSize.dot,
                "animate-pulse [animation-delay:150ms]"
              )} />
              <div className={cn(
                "rounded-full bg-muted-foreground/20",
                currentSize.dot,
                "animate-pulse [animation-delay:300ms]"
              )} />
            </div>
          )
          
        case 'wave':
          return (
            <div className={cn("flex items-end", currentSize.gap)}>
              {[0, 1, 2].map((index) => (
                <div
                  key={index}
                  className={cn(
                    "bg-primary/60 rounded-full",
                    currentSize.dot
                  )}
                  style={{
                    animationName: 'typing-wave',
                    animationDuration: `${speed * 4}ms`,
                    animationTimingFunction: 'ease-in-out',
                    animationIterationCount: 'infinite',
                    animationDelay: `${index * speed}ms`
                  } as React.CSSProperties}
                />
              ))}
            </div>
          )
          
        case 'dots':
        default:
          return (
            <div className={cn("flex items-center", currentSize.gap)}>
              {[0, 1, 2].map((index) => (
                <div
                  key={index}
                  className={cn(
                    "rounded-full bg-muted-foreground/60",
                    currentSize.dot
                  )}
                  style={{
                    animationName: 'typing-bounce',
                    animationDuration: `${speed * 4}ms`,
                    animationTimingFunction: 'ease-in-out',
                    animationIterationCount: 'infinite',
                    animationDelay: `${index * speed}ms`
                  } as React.CSSProperties}
                />
              ))}
            </div>
          )
      }
    }
    
    return (
      <>
        {/* Inject animations */}
        <style dangerouslySetInnerHTML={{ __html: `
          @keyframes typing-bounce {
            0%, 60%, 100% {
              transform: translateY(0);
              opacity: 0.7;
            }
            30% {
              transform: translateY(-10px);
              opacity: 1;
            }
          }
          
          @keyframes typing-wave {
            0%, 60%, 100% {
              transform: scaleY(1);
              opacity: 0.7;
            }
            30% {
              transform: scaleY(1.5);
              opacity: 1;
            }
          }
        ` }} />
        
        <div
          ref={ref}
          className={cn(
            "inline-flex items-center rounded-lg bg-muted",
            currentSize.container,
            "transition-all duration-200 ease-in-out",
            "animate-in fade-in-0 slide-in-from-bottom-1",
            className
          )}
          role="status"
          aria-label={label}
          aria-live="polite"
          {...props}
        >
          {renderIndicator()}
          
          {/* Screen reader text */}
          <span className="sr-only">{label}</span>
        </div>
      </>
    )
  }
)

TypingIndicator.displayName = 'TypingIndicator'

// Standalone CSS for animations (to be added to global styles if needed)
export const typingIndicatorStyles = `
  @keyframes typing-bounce {
    0%, 60%, 100% {
      transform: translateY(0);
      opacity: 0.7;
    }
    30% {
      transform: translateY(-10px);
      opacity: 1;
    }
  }
  
  @keyframes typing-wave {
    0%, 60%, 100% {
      transform: scaleY(1);
      opacity: 0.7;
    }
    30% {
      transform: scaleY(1.5);
      opacity: 1;
    }
  }
`

export { TypingIndicator }