'use client'

import * as React from 'react'
import { useState, useEffect } from 'react'
import { ImageOff, Loader2 } from 'lucide-react'
import type { ImageContentBlock } from '@agentc/realtime-react'
import { cn } from '../../../lib/utils'
import { ImageLightbox } from '../ImageLightbox'

export interface ImageContentRendererProps {
  /** Image content block */
  content: ImageContentBlock
  /** Custom className */
  className?: string
  /** Max width for image */
  maxWidth?: string | number
  /** Enable lightbox/modal view on click */
  enableLightbox?: boolean
}

/**
 * Render image content blocks within messages
 * Displays images with loading states, error handling, and optional lightbox view
 */
export function ImageContentRenderer({
  content,
  className,
  maxWidth = '100%',
  enableLightbox = true,
}: ImageContentRendererProps) {
  const [isLightboxOpen, setIsLightboxOpen] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const [hasError, setHasError] = useState(false)
  
  const { source } = content
  // Fix: Use source.data for both base64 AND url types
  const src = source.type === 'url'
    ? (source.data || '')
    : `data:${source.media_type};base64,${source.data || ''}`
  
  // Convert numeric maxWidth to pixel string
  const maxWidthStyle = typeof maxWidth === 'number' ? `${maxWidth}px` : maxWidth
  
  // Handle missing data
  useEffect(() => {
    if (!source.data) {
      setHasError(true)
    }
  }, [source.data])
  
  const handleLoad = () => {
    setIsLoading(false)
  }
  
  const handleError = () => {
    setIsLoading(false)
    setHasError(true)
  }
  
  if (hasError) {
    return (
      <div 
        className={cn(
          'flex items-center gap-2 p-3 rounded-lg bg-muted text-muted-foreground',
          className
        )}
        role="alert"
      >
        <ImageOff className="h-5 w-5" />
        <span className="text-sm">Failed to load image</span>
      </div>
    )
  }
  
  return (
    <>
      <div
        className={cn('relative inline-block rounded-lg overflow-hidden', className)}
        style={{ maxWidth: maxWidthStyle }}
      >
        {isLoading && (
          <div className="absolute inset-0 flex items-center justify-center bg-muted">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        )}
        
        <img
          src={src}
          alt="Message attachment"
          onLoad={handleLoad}
          onError={handleError}
          className={cn(
            'max-w-full h-auto rounded-lg',
            enableLightbox && 'cursor-pointer hover:opacity-90 transition-opacity',
            isLoading && 'opacity-0'
          )}
          onClick={() => enableLightbox && setIsLightboxOpen(true)}
          loading="lazy"
        />
      </div>
      
      {/* Lightbox modal */}
      {enableLightbox && (
        <ImageLightbox
          src={src}
          isOpen={isLightboxOpen}
          onClose={() => setIsLightboxOpen(false)}
        />
      )}
    </>
  )
}

export default ImageContentRenderer
