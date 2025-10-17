'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'

export interface UploadProgressIndicatorProps {
  /** Progress percentage (0-100) */
  progress: number
  /** Indicator style */
  variant?: 'circular' | 'linear'
  /** Size variant */
  size?: 'sm' | 'md' | 'lg'
  /** Custom className */
  className?: string
  /** Show percentage text */
  showPercentage?: boolean
}

export function UploadProgressIndicator({
  progress,
  variant = 'circular',
  size = 'md',
  showPercentage = true,
  className,
}: UploadProgressIndicatorProps) {
  const clampedProgress = Math.min(Math.max(progress, 0), 100)
  
  if (variant === 'linear') {
    return (
      <div
        className={cn('w-full bg-muted rounded-full overflow-hidden', className)}
        role="progressbar"
        aria-valuenow={clampedProgress}
        aria-valuemin={0}
        aria-valuemax={100}
        aria-label={`Upload progress: ${clampedProgress}%`}
      >
        <div
          className="h-2 bg-primary transition-all duration-300"
          style={{ width: `${clampedProgress}%` }}
        />
      </div>
    )
  }
  
  // Circular variant
  const sizeClasses = {
    sm: 'h-8 w-8',
    md: 'h-12 w-12',
    lg: 'h-16 w-16',
  }
  
  const radius = size === 'sm' ? 14 : size === 'md' ? 20 : 28
  const circumference = 2 * Math.PI * radius
  const strokeDashoffset = circumference - (clampedProgress / 100) * circumference
  
  return (
    <div
      className={cn('relative inline-flex items-center justify-center', sizeClasses[size], className)}
      role="progressbar"
      aria-valuenow={clampedProgress}
      aria-valuemin={0}
      aria-valuemax={100}
      aria-label={`Upload progress: ${clampedProgress}%`}
    >
      <svg className="transform -rotate-90" width="100%" height="100%">
        {/* Background circle */}
        <circle
          cx="50%"
          cy="50%"
          r={radius}
          stroke="currentColor"
          strokeWidth="2"
          fill="none"
          className="text-muted"
        />
        {/* Progress circle */}
        <circle
          cx="50%"
          cy="50%"
          r={radius}
          stroke="currentColor"
          strokeWidth="2"
          fill="none"
          strokeDasharray={circumference}
          strokeDashoffset={strokeDashoffset}
          className="text-primary transition-all duration-300"
          strokeLinecap="round"
        />
      </svg>
      {showPercentage && (
        <span className="absolute text-xs font-medium">
          {clampedProgress}%
        </span>
      )}
    </div>
  )
}
