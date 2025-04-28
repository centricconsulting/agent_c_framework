import React, { useRef, useEffect, useState, useCallback } from 'react';
import { ScrollArea } from '../ui/scroll-area';
import MessageItem from './MessageItem';
import { cn } from '../../lib/utils';
import { Button } from '../ui/button';
import { ChevronUp } from 'lucide-react';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '../ui/tooltip';
import VirtualizedMessagesList from './VirtualizedMessagesList';
import { useMessageContext } from '@/contexts/MessageContext';
import { useToolCalls } from './ToolCallContext';

// Import component CSS
import '@/styles/components/messages-list.css';

/**
 * MessagesList component handles rendering a list of messages with automatic scrolling
 * and manages the expanded state of tool calls.
 * 
 * @param {Object} props - Component props
 * @param {string} props.className - Optional additional CSS classes for the container
 */
const MessagesList = ({ className }) => {
  // Get message and tool call contexts
  const { messages, expandedToolCallMessages, toggleToolCallExpansion } = useMessageContext();
  const { toolSelectionState } = useToolCalls();
  
  const messagesEndRef = useRef(null);
  const viewportRef = useRef(null);
  const [showScrollTopButton, setShowScrollTopButton] = useState(false);
  const [isAutoScrolling, setIsAutoScrolling] = useState(false);
  const [shouldUseVirtualList, setShouldUseVirtualList] = useState(false);
  
  // Determine if we should use virtualization (> 30 messages or message count growing rapidly)
  useEffect(() => {
    if (messages.length > 30) {
      setShouldUseVirtualList(true);
    }
  }, [messages.length]);
  
  // Scroll to bottom when messages change
  const scrollToBottom = (smooth = true) => {
    messagesEndRef.current?.scrollIntoView({ 
      behavior: smooth ? 'smooth' : 'auto' 
    });
  };
  
  // Scroll to top function
  const scrollToTop = useCallback(() => {
    if (shouldUseVirtualList) {
      // Handled by VirtualizedMessagesList
      setShowScrollTopButton(false);
    } else {
      viewportRef.current?.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }, [shouldUseVirtualList]);
  
  // Track scroll position to show/hide scroll to top button
  const handleScroll = () => {
    if (viewportRef.current) {
      const scrollTop = viewportRef.current.scrollTop;
      setShowScrollTopButton(scrollTop > 200); // Show after scrolling 200px
    }
  };
  
  // Add event listener for scroll events
  useEffect(() => {
    if (!shouldUseVirtualList) {
      const viewport = viewportRef.current;
      if (viewport) {
        viewport.addEventListener('scroll', handleScroll);
        return () => viewport.removeEventListener('scroll', handleScroll);
      }
    }
  }, [shouldUseVirtualList]);
  
  // Auto-scroll to bottom when messages change
  useEffect(() => {
    if (!shouldUseVirtualList) {
      setIsAutoScrolling(true);
      scrollToBottom();
      const timer = setTimeout(() => setIsAutoScrolling(false), 500);
      return () => clearTimeout(timer);
    }
  }, [messages, shouldUseVirtualList]);
  
  // Auto-scroll when tool selection indicator appears
  useEffect(() => {
    if (toolSelectionState.inProgress && !shouldUseVirtualList) {
      scrollToBottom();
    }
  }, [toolSelectionState.inProgress, shouldUseVirtualList]);

  // Prepare tool call data for the virtualized list
  const toolCallData = {
    expandedToolCallMessages,
    toggleToolCallExpansion,
    toolSelectionInProgress: toolSelectionState.inProgress,
    toolSelectionName: toolSelectionState.toolName
  };
  
  // If using virtualized list
  if (shouldUseVirtualList) {
    return (
      <div className={cn(
        "messages-list-container relative flex h-full w-full min-h-0 flex-col overflow-hidden",
        className
      )}>
        <VirtualizedMessagesList
          messages={messages}
          onScrollToTop={scrollToTop}
          toolCallData={toolCallData}
          isStreaming={false}
          isLoadingHistory={false}
        />
        
        {/* Tool selection in progress indicator */}
        {toolSelectionState.inProgress && (
          <div className="flex items-center gap-2 text-sm italic ml-8 my-1 text-muted-foreground absolute bottom-2 left-0">
            <div className="h-2 w-2 rounded-full bg-primary animate-pulse"></div>
            <span>Preparing to use: {toolSelectionState.toolName?.replace(/-/g, ' ') || 'tool'}</span>
          </div>
        )}
      </div>
    );
  }

  // Original implementation for smaller message counts
  return (
    <div className={cn(
      "messages-list-container relative flex h-full w-full min-h-0 flex-col overflow-hidden overflow-x-hidden",
      className
    )}>
      <ScrollArea 
        className="flex-1 p-2 pb-3 overflow-hidden"
        type="always"
        scrollHideDelay={100}
        viewportRef={viewportRef}
        aria-label="Chat messages"
      >
        <div className="w-full overflow-x-hidden max-w-full pt-4 pb-4">
          {messages.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full text-muted-foreground p-4 text-center">
              <p>No messages yet. Start a conversation!</p>
            </div>
          ) : (
            <div className="flex flex-col space-y-4 min-h-full">
              {messages.map((message, index) => (
                <div key={index} className="message-item-wrapper w-full">
                  <MessageItem
                    message={message}
                    index={index}
                    messages={messages}
                    expandedToolCallMessages={expandedToolCallMessages}
                    toggleToolCallExpansion={toggleToolCallExpansion}
                  />
                </div>
              ))}
              
              {/* Tool selection in progress indicator */}
              {toolSelectionState.inProgress && (
                <div className="flex items-center gap-2 text-sm italic ml-8 my-1 text-muted-foreground">
                  <div className="h-2 w-2 rounded-full bg-primary animate-pulse"></div>
                  <span>Preparing to use: {toolSelectionState.toolName?.replace(/-/g, ' ') || 'tool'}</span>
                </div>
              )}
              
              {/* Ref for scrolling to bottom */}
              <div ref={messagesEndRef} />
            </div>
          )}
        </div>
      </ScrollArea>
      
      {/* Scroll to top button with tooltip */}
      {showScrollTopButton && (
        <div className="scroll-indicator">
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button 
                  onClick={scrollToTop} 
                  size="sm" 
                  variant="secondary" 
                  className="w-10 h-10 rounded-full flex items-center justify-center opacity-80 transition-opacity shadow-md hover:opacity-100"
                  aria-label="Scroll to top"
                >
                  <ChevronUp className="h-4 w-4" />
                </Button>
              </TooltipTrigger>
              <TooltipContent side="left">
                <p>Scroll to top</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </div>
      )}
    </div>
  );
};

export default React.memo(MessagesList);