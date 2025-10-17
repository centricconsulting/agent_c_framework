'use client'

import * as React from 'react'
import type { MessageContentBlock } from '@agentc/realtime-react'
import { cn } from '../../../lib/utils'
import { TextContentRenderer } from './TextContentRenderer'
import { ImageContentRenderer } from './ImageContentRenderer'

export interface MultimodalContentRendererProps {
  /** Array of content blocks */
  content: MessageContentBlock[]
  /** Role of the message sender (required for text rendering) */
  role: 'user' | 'assistant' | 'system'
  /** Custom className */
  className?: string
}

/**
 * Render mixed content (text + images) within a single message.
 * Routes content blocks to appropriate renderers based on block type.
 */
export function MultimodalContentRenderer({
  content,
  role,
  className,
}: MultimodalContentRendererProps) {
  return (
    <div className={cn('flex flex-col gap-3', className)}>
      {content.filter(Boolean).map((block, index) => {
        // Type guard: ensure block is defined and has type property
        if (!block || typeof block !== 'object' || !('type' in block)) {
          return null
        }
        
        if (block.type === 'text') {
          return (
            <TextContentRenderer
              key={index}
              content={block.text}
              role={role}
            />
          )
        }
        
        if (block.type === 'image') {
          return (
            <ImageContentRenderer
              key={index}
              content={block}
            />
          )
        }
        
        return null
      })}
    </div>
  )
}

export default MultimodalContentRenderer
