'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { FileIcon, AlertCircle, Check, X } from 'lucide-react'
import type { FileAttachment } from '@agentc/realtime-react'
import { UploadProgressIndicator } from './UploadProgressIndicator'

export interface FileAttachmentItemProps {
  /** The file attachment to display */
  attachment: FileAttachment
  /** Callback when the remove button is clicked */
  onRemove: () => void
  /** Size variant */
  size?: 'sm' | 'md' | 'lg'
  /** Additional CSS class name */
  className?: string
}

export function FileAttachmentItem({
  attachment,
  onRemove,
  size = 'md',
  className,
}: FileAttachmentItemProps) {
  const { file, status, progress, error, previewUrl } = attachment
  
  const sizeClasses = {
    sm: 'h-16 w-16',
    md: 'h-20 w-20',
    lg: 'h-24 w-24',
  }
  
  const isImage = file.type.startsWith('image/')
  
  return (
    <div
      className={cn(
        'relative group',
        'border rounded-lg overflow-hidden',
        'transition-all duration-200',
        status === 'error' && 'border-destructive',
        status === 'complete' && 'border-primary',
        sizeClasses[size],
        className
      )}
      role="listitem"
      aria-label={`${file.name}, ${status}`}
    >
      {/* Preview or icon */}
      <div className="h-full w-full relative">
        {isImage && previewUrl ? (
          <img
            src={previewUrl}
            alt={file.name}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="h-full w-full flex items-center justify-center bg-muted">
            <FileIcon className="h-8 w-8 text-muted-foreground" />
          </div>
        )}
        
        {/* Upload progress overlay */}
        {status === 'uploading' && (
          <div className="absolute inset-0 bg-background/80 flex items-center justify-center">
            <UploadProgressIndicator progress={progress} size="sm" />
          </div>
        )}
        
        {/* Error overlay */}
        {status === 'error' && (
          <div 
            className="absolute inset-0 bg-destructive/90 flex items-center justify-center"
            role="alert"
          >
            <AlertCircle className="h-6 w-6 text-destructive-foreground" />
          </div>
        )}
        
        {/* Complete checkmark */}
        {status === 'complete' && (
          <div className="absolute top-1 right-1 bg-primary rounded-full p-1">
            <Check className="h-3 w-3 text-primary-foreground" />
          </div>
        )}
      </div>
      
      {/* Remove button */}
      <button
        onClick={onRemove}
        className={cn(
          'absolute top-1 left-1',
          'p-1 rounded-full bg-background/90 border',
          'opacity-0 group-hover:opacity-100',
          'transition-opacity duration-200',
          'hover:bg-destructive hover:border-destructive hover:text-destructive-foreground',
          'focus:opacity-100 focus:outline-none focus:ring-2 focus:ring-primary'
        )}
        aria-label={`Remove ${file.name}`}
        type="button"
      >
        <X className="h-4 w-4" />
      </button>
      
      {/* Filename tooltip */}
      <div
        className={cn(
          'absolute -bottom-8 left-0 right-0',
          'px-2 py-1 bg-background border rounded text-xs',
          'opacity-0 group-hover:opacity-100',
          'transition-opacity duration-200',
          'truncate',
          'pointer-events-none'
        )}
      >
        {file.name}
      </div>
      
      {/* Error tooltip */}
      {error && (
        <div
          className={cn(
            'absolute -bottom-8 left-0 right-0',
            'px-2 py-1 bg-destructive text-destructive-foreground text-xs rounded',
            'opacity-0 group-hover:opacity-100',
            'transition-opacity duration-200'
          )}
          role="alert"
        >
          {error}
        </div>
      )}
    </div>
  )
}
