'use client';

import * as React from 'react';
import { useChat } from '@agentc/realtime-react';
import { TypingIndicator, SystemNotification } from '@agentc/realtime-ui';
import { MessageSquarePlus, MessageSquareOff } from 'lucide-react';

// Simple subsession divider component
const SubsessionDivider = ({ type, metadata }: { type: 'start' | 'end', metadata?: any }) => {
  const isStart = type === 'start';
  return (
    <div className="flex items-center gap-2 py-2">
      <div className="flex-1 h-px bg-border" />
      <div className="flex items-center gap-1 text-xs text-muted-foreground">
        {isStart ? <MessageSquarePlus className="h-3 w-3" /> : <MessageSquareOff className="h-3 w-3" />}
        <span>
          {isStart ? 'Subsession Started' : 'Subsession Ended'}
          {metadata?.subAgentKey && ` (${metadata.subAgentKey})`}
        </span>
      </div>
      <div className="flex-1 h-px bg-border" />
    </div>
  );
};
import { cn } from '@/lib/utils';
import { User, Bot, Brain } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { ThinkBubbleRenderer } from './ThinkBubbleRenderer';
import type { ContentPart } from '@agentc/realtime-core';

interface ComparisonChatViewProps {
  className?: string;
}

/**
 * Custom chat view for comparison that renders think tools as gray bubbles
 */
export const ComparisonChatView: React.FC<ComparisonChatViewProps> = ({
  className
}) => {
  const { messages, isAgentTyping, streamingMessage } = useChat();
  const scrollRef = React.useRef<HTMLDivElement>(null);

  // Auto-scroll to bottom on new messages
  React.useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [messages, streamingMessage]);

  // Helper to check if content contains think tool
  const hasThinkTool = (content: any): boolean => {
    if (Array.isArray(content)) {
      return content.some(part => 
        part && typeof part === 'object' && 
        part.type === 'tool_use' && 
        part.name === 'think'
      );
    }
    return false;
  };

  // Helper to render content
  const renderContent = (content: any, role: string) => {
    // Handle string content
    if (typeof content === 'string') {
      return (
        <ReactMarkdown
          remarkPlugins={[remarkGfm]}
          className="prose prose-sm max-w-none"
        >
          {content}
        </ReactMarkdown>
      );
    }

    // Handle array content
    if (Array.isArray(content)) {
      return (
        <div className="space-y-2">
          {content.map((part: ContentPart, index: number) => {
            // Handle text parts
            if (part.type === 'text') {
              return (
                <ReactMarkdown
                  key={`text-${index}`}
                  remarkPlugins={[remarkGfm]}
                  className="prose prose-sm max-w-none"
                >
                  {(part as any).text || ''}
                </ReactMarkdown>
              );
            }

            // Handle think tool specially
            if (part.type === 'tool_use' && (part as any).name === 'think') {
              const thought = (part as any).input?.thought || 'Thinking...';
              return (
                <ThinkBubbleRenderer
                  key={`think-${index}`}
                  thought={thought}
                />
              );
            }

            // Handle other tool_use
            if (part.type === 'tool_use') {
              const toolPart = part as any;
              return (
                <div
                  key={`tool-${index}`}
                  className="rounded-lg border border-blue-200 bg-blue-50/50 dark:border-blue-800 dark:bg-blue-950/30 px-3 py-2"
                >
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-medium text-blue-900 dark:text-blue-100">
                      Using {toolPart.name}
                    </span>
                  </div>
                </div>
              );
            }

            // Handle tool_result (usually empty for think tools)
            if (part.type === 'tool_result') {
              // Skip empty tool results
              if (!(part as any).content) return null;
              
              return (
                <div key={`result-${index}`} className="text-sm text-muted-foreground">
                  Tool result: {JSON.stringify((part as any).content)}
                </div>
              );
            }

            return null;
          })}
        </div>
      );
    }

    return null;
  };

  return (
    <div 
      ref={scrollRef}
      className={cn("h-full overflow-y-auto p-4", className)}
    >
      <div className="max-w-4xl mx-auto space-y-4">
        {messages.map((item, index) => {
          // Handle dividers
          if (item.type === 'divider') {
            return (
              <SubsessionDivider
                key={`divider-${index}`}
                type={item.dividerType}
                metadata={item.metadata}
              />
            );
          }

          // Handle system alerts
          if (item.type === 'system_alert') {
            return (
              <SystemNotification
                key={`alert-${index}`}
                notification={{
                  id: `alert-${index}`,
                  content: item.content,
                  severity: item.severity || 'info',
                  timestamp: item.timestamp || new Date().toISOString(),
                  dismissible: true
                }}
              />
            );
          }

          // Handle regular messages
          if (item.type === 'message' || !item.type) {
            const isUser = item.role === 'user';
            const isThinkMessage = hasThinkTool(item.content);

            return (
              <div
                key={item.id || `msg-${index}`}
                className={cn(
                  "group relative flex gap-3 py-2",
                  isUser && "flex-row-reverse"
                )}
              >
                {/* Avatar */}
                <div className="flex-shrink-0">
                  <div className={cn(
                    "h-8 w-8 rounded-full flex items-center justify-center",
                    isUser ? "bg-primary" : "bg-primary/10"
                  )}>
                    {isUser ? (
                      <User className="h-4 w-4 text-primary-foreground" />
                    ) : (
                      <Bot className="h-4 w-4 text-primary" />
                    )}
                  </div>
                </div>

                {/* Message content */}
                <div className={cn(
                  "flex-1 max-w-full overflow-hidden",
                  isUser && "flex flex-col items-end"
                )}>
                  <div className={cn(
                    "rounded-xl px-4 py-2.5",
                    isUser 
                      ? "bg-muted" 
                      : isThinkMessage
                        ? "bg-transparent" // No background for think messages
                        : "bg-card border border-border/50"
                  )}>
                    {renderContent(item.content, item.role)}
                  </div>
                </div>
              </div>
            );
          }

          return null;
        })}

        {/* Streaming message */}
        {streamingMessage && (
          <div className="group relative flex gap-3 py-2">
            <div className="flex-shrink-0">
              <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center">
                <Bot className="h-4 w-4 text-primary" />
              </div>
            </div>
            <div className="flex-1">
              <div className="bg-card border border-border/50 rounded-xl px-4 py-2.5">
                {renderContent(streamingMessage.content, streamingMessage.role)}
              </div>
            </div>
          </div>
        )}

        {/* Typing indicator */}
        {isAgentTyping && !streamingMessage && (
          <TypingIndicator />
        )}
      </div>
    </div>
  );
};