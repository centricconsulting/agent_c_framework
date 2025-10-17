'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import type { MessageContent, ContentPart } from '@agentc/realtime-core'
import { TextContentRenderer } from './content-renderers/TextContentRenderer'
import { ImageContentRenderer } from './content-renderers/ImageContentRenderer'
import { ToolUseContentRenderer } from './content-renderers/ToolUseContentRenderer'
import { ToolResultContentRenderer } from './content-renderers/ToolResultContentRenderer'
import { UnknownContentRenderer } from './content-renderers/UnknownContentRenderer'
import { MultimodalContentRenderer } from './content-renderers/MultimodalContentRenderer'

export interface MessageContentRendererProps {
  /**
   * The message content to render
   * Can be a string, array of ContentPart objects, or null
   */
  content: MessageContent
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
 * Main content router component that handles all message content types
 * Routes string, ContentPart[], or null content to appropriate renderers
 */
export const MessageContentRenderer: React.FC<MessageContentRendererProps> = ({
  content,
  role,
  className
}) => {
  // Handle null content
  if (content === null) {
    return (
      <div 
        className={cn("text-muted-foreground italic", className)}
        role="status"
        aria-label="Empty message content"
      >
        <span>No content available</span>
      </div>
    )
  }
  
  // Handle string content - use existing markdown rendering
  if (typeof content === 'string') {
    return (
      <TextContentRenderer
        content={content}
        role={role}
        className={className}
      />
    )
  }
  
  // Handle ContentPart array
  if (Array.isArray(content)) {
    // Check if content has images (multimodal)
    const hasImages = content.some(block => 
      typeof block === 'object' && block !== null && 'type' in block && block.type === 'image'
    )
    
    // Route to MultimodalContentRenderer for messages with images
    if (hasImages) {
      return (
        <MultimodalContentRenderer
          content={content as any}
          role={role}
          className={className}
        />
      )
    }
    
    // For text-only arrays, use existing part-by-part rendering
    return (
      <div 
        className={cn("space-y-3", className)}
        role="article"
        aria-label={`${role} message with ${content.length} content parts`}
      >
        {content.map((part, index) => (
          <ContentPartRenderer
            key={`content-part-${index}`}
            part={part}
            role={role}
            index={index}
          />
        ))}
      </div>
    )
  }
  
  // Fallback for unexpected content types
  console.warn('Unexpected content type in MessageContentRenderer:', typeof content)
  return (
    <div 
      className={cn("text-muted-foreground italic", className)}
      role="alert"
    >
      <span>Unable to render message content</span>
    </div>
  )
}

/**
 * Sub-component for rendering individual content parts
 * Routes each ContentPart to its specific renderer
 */
interface ContentPartRendererProps {
  part: ContentPart
  role: 'user' | 'assistant' | 'system'
  index: number
}

const ContentPartRenderer: React.FC<ContentPartRendererProps> = ({ 
  part, 
  role,
  index 
}) => {
  // Type guard checks for safety
  if (!part || typeof part !== 'object' || !('type' in part)) {
    console.warn('Invalid content part:', part)
    return (
      <UnknownContentRenderer
        type="invalid"
        content={part}
      />
    )
  }
  
  switch (part.type) {
    case 'text':
      // Type assertion after checking
      const textPart = part as { type: 'text'; text: string }
      return (
        <TextContentRenderer 
          content={textPart.text}
          role={role}
        />
      )
    
    case 'image':
      // Type assertion for image content
      const imagePart = part as { 
        type: 'image'; 
        source: { 
          type: 'base64' | 'url';
          media_type: string;
          data: string;
        } 
      }
      return (
        <ImageContentRenderer
          content={imagePart}
        />
      )
    
    case 'tool_use':
      // Type assertion for tool use
      const toolUsePart = part as {
        type: 'tool_use';
        id: string;
        name: string;
        input: Record<string, any>;
      }
      return (
        <ToolUseContentRenderer
          id={toolUsePart.id}
          name={toolUsePart.name}
          input={toolUsePart.input}
        />
      )
    
    case 'tool_result':
      // Type assertion for tool result
      const toolResultPart = part as {
        type: 'tool_result';
        tool_use_id: string;
        content: string | ContentPart[];
        is_error?: boolean;
      }
      return (
        <ToolResultContentRenderer
          toolUseId={toolResultPart.tool_use_id}
          content={toolResultPart.content}
          isError={toolResultPart.is_error}
          role={role}
        />
      )
    
    default:
      // Handle unknown content types gracefully
      return (
        <UnknownContentRenderer
          type={(part as any).type || 'unknown'}
          content={part}
        />
      )
  }
}

// Export the sub-renderer for potential reuse
export { ContentPartRenderer }