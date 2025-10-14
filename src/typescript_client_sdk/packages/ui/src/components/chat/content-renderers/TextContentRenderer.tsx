'use client'

import * as React from 'react'
import { cn } from '../../../lib/utils'
import { MarkdownRenderer } from './MarkdownRenderer'

export interface TextContentRendererProps {
  /**
   * The text content to render with markdown support
   */
  content: string
  /**
   * The role of the message sender
   */
  role: 'user' | 'assistant' | 'system'
  /**
   * Additional CSS classes
   */
  className?: string
}

/**
 * Renders text content with full markdown support
 * Uses shared MarkdownRenderer for consistent rendering across all message types
 */
export const TextContentRenderer: React.FC<TextContentRendererProps> = ({
  content,
  role,
  className
}) => {
  return (
    <MarkdownRenderer
      content={content}
      compact={false}
      ariaLabel={`${role} message with markdown content`}
      className={className}
    />
  )
}

export default TextContentRenderer