'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import type { FileAttachment } from '@agentc/realtime-react'
import { FileAttachmentItem } from './FileAttachmentItem'

export interface FileAttachmentListProps {
  /** Array of file attachments */
  attachments: FileAttachment[]
  /** Callback when file is removed */
  onRemove: (index: number) => void
  /** Layout orientation */
  orientation?: 'horizontal' | 'vertical'
  /** Custom className */
  className?: string
}

export function FileAttachmentList({
  attachments,
  onRemove,
  orientation = 'horizontal',
  className,
}: FileAttachmentListProps) {
  if (attachments.length === 0) return null
  
  return (
    <div
      className={cn(
        'flex gap-2',
        orientation === 'horizontal' ? 'flex-row flex-wrap' : 'flex-col',
        className
      )}
      role="list"
      aria-label={`${attachments.length} file attachment${attachments.length !== 1 ? 's' : ''}`}
    >
      {attachments.map((attachment, index) => (
        <FileAttachmentItem
          key={index}
          attachment={attachment}
          onRemove={() => onRemove(index)}
        />
      ))}
    </div>
  )
}
