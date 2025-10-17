'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { Upload } from 'lucide-react'

export interface FileDropZoneProps {
  /** Whether drop zone is active */
  isActive: boolean
  /** Allowed file types for display */
  allowedTypes?: string[]
  /** Custom className */
  className?: string
}

/**
 * DropOverlay Component
 * 
 * Provides visual feedback during drag-drop operations.
 * Note: Drag-drop logic is handled by react-dropzone, this component only provides the visual overlay.
 * 
 * @example
 * ```tsx
 * <DropOverlay isActive={isDragging} allowedTypes={['images', 'documents']} />
 * ```
 */
export function DropOverlay({
  isActive,
  allowedTypes = ['images'],
  className,
}: FileDropZoneProps) {
  if (!isActive) return null
  
  return (
    <div
      className={cn(
        'absolute inset-0 z-50',
        'flex items-center justify-center',
        'bg-primary/10 backdrop-blur-sm',
        'border-2 border-dashed border-primary rounded-lg',
        'pointer-events-none',
        className
      )}
      role="status"
      aria-live="polite"
    >
      <div className="flex flex-col items-center gap-2 text-primary">
        <Upload className="h-12 w-12" />
        <p className="text-lg font-medium">
          Drop {allowedTypes.join(', ')} here
        </p>
      </div>
    </div>
  )
}
