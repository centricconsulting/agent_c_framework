'use client'

import * as React from 'react'
import { useEffect } from 'react'
import { X } from 'lucide-react'
import { cn } from '../../lib/utils'
import { Dialog, DialogContent, DialogTitle, DialogDescription } from '../ui/dialog'

export interface ImageLightboxProps {
  /** Image source URL */
  src: string
  /** Alt text */
  alt?: string
  /** Whether lightbox is open */
  isOpen: boolean
  /** Callback when closed */
  onClose: () => void
}

/**
 * Full-screen image viewer modal
 * Displays images in a lightbox overlay with full-screen support
 */
export function ImageLightbox({
  src,
  alt = 'Image',
  isOpen,
  onClose,
}: ImageLightboxProps) {
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose()
      }
    }
    
    if (isOpen) {
      document.addEventListener('keydown', handleEscape)
      document.body.style.overflow = 'hidden'
    }
    
    return () => {
      document.removeEventListener('keydown', handleEscape)
      document.body.style.overflow = ''
    }
  }, [isOpen, onClose])
  
  if (!isOpen) return null
  
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent 
        className="max-w-screen-xl w-full h-full max-h-screen p-0"
        aria-label="Image viewer"
      >
        <DialogTitle className="sr-only">Image viewer</DialogTitle>
        <DialogDescription className="sr-only">
          Full screen image view. Press Escape to close.
        </DialogDescription>
        <div className="relative w-full h-full flex items-center justify-center bg-background/95">
          <button
            onClick={onClose}
            className={cn(
              'absolute top-4 right-4 z-50',
              'p-2 rounded-full bg-background border',
              'hover:bg-muted transition-colors',
              'focus:outline-none focus:ring-2 focus:ring-primary'
            )}
            aria-label="Close image viewer"
          >
            <X className="h-6 w-6" />
          </button>
          
          <img
            src={src}
            alt={alt}
            className="max-w-full max-h-full object-contain p-8"
          />
        </div>
      </DialogContent>
    </Dialog>
  )
}
