import React, { useCallback, useEffect, useRef, useState, useMemo } from 'react';
import { VariableSizeList as List } from 'react-window';
import '../../styles/components/messages-list.css';

const VirtualizedMessagesList = ({
  messages,
  onScrollToTop,
  toolCallData,
  selectedToolId,
  onToolSelect,
  isStreaming,
  isLoadingHistory,
  children
}) => {
  const listRef = useRef(null);
  const outerRef = useRef(null);
  const sizeMap = useRef({});
  const setSize = useCallback((index, size) => {
    const prevSize = sizeMap.current[index];
    if (prevSize !== size) {
      sizeMap.current = { ...sizeMap.current, [index]: size };
      if (listRef.current) {
        listRef.current.resetAfterIndex(index);
      }
    }
  }, []);

  const getSize = useCallback(index => {
    return sizeMap.current[index] || 150; // Default height estimation
  }, []);

  // Auto-scroll handling
  const [shouldAutoScroll, setShouldAutoScroll] = useState(true);
  const prevMessagesLengthRef = useRef(messages.length);
  const isUserNearBottom = useRef(true);

  // Determine if user is scrolled near bottom
  const handleScroll = useCallback(({ scrollOffset, scrollDirection }) => {
    if (!outerRef.current) return;
    
    const { scrollHeight, clientHeight } = outerRef.current;
    const scrollBottom = scrollOffset + clientHeight;
    const isAtBottom = scrollBottom >= scrollHeight - 150;
    
    isUserNearBottom.current = isAtBottom;
    setShouldAutoScroll(isAtBottom);
    
    // Show/hide scroll to top button based on scroll position
    if (scrollOffset > 500 && onScrollToTop) {
      document.querySelector('.scroll-to-top-btn')?.classList.add('visible');
    } else {
      document.querySelector('.scroll-to-top-btn')?.classList.remove('visible');
    }
  }, [onScrollToTop]);

  // Scroll to bottom when new messages arrive or when streaming
  useEffect(() => {
    const messageAdded = messages.length > prevMessagesLengthRef.current;
    prevMessagesLengthRef.current = messages.length;
    
    if ((messageAdded && shouldAutoScroll) || isStreaming) {
      if (listRef.current && outerRef.current) {
        setTimeout(() => {
          if (listRef.current) {
            listRef.current.scrollToItem(messages.length - 1, 'end');
          }
        }, 0);
      }
    }
  }, [messages, isStreaming, shouldAutoScroll]);

  // Scroll to top handler
  const scrollToTop = useCallback(() => {
    if (listRef.current) {
      listRef.current.scrollToItem(0, 'start');
    }
    if (onScrollToTop) {
      onScrollToTop();
    }
  }, [onScrollToTop]);

  // Row renderer
  const Row = useCallback(({ index, style }) => {
    const message = messages[index];
    return (
      <MessageMeasurer
        index={index}
        style={style}
        message={message}
        setSize={setSize}
        toolCallData={toolCallData}
        selectedToolId={selectedToolId}
        onToolSelect={onToolSelect}
      />
    );
  }, [messages, toolCallData, selectedToolId, onToolSelect, setSize]);

  // Empty state
  if (!messages || messages.length === 0) {
    return (
      <div className="messages-list empty">
        <div className="empty-state">
          {isLoadingHistory ? (
            <div className="loading-indicator">Loading message history...</div>
          ) : (
            children || <div className="no-messages">No messages yet</div>
          )}
        </div>
      </div>
    );
  }

  return (
    <div className="messages-list">
      {onScrollToTop && (
        <button 
          className="scroll-to-top-btn" 
          onClick={scrollToTop} 
          aria-label="Scroll to top of conversation"
        >
          ↑
        </button>
      )}
      <List
        ref={listRef}
        outerRef={outerRef}
        className="messages-list-virtualizer"
        height={600} // This will be overridden by CSS
        width="100%"
        itemCount={messages.length}
        itemSize={getSize}
        onScroll={handleScroll}
        overscanCount={5}
      >
        {Row}
      </List>
    </div>
  );
};

// Component to measure and render each message
const MessageMeasurer = React.memo(({ index, style, message, setSize, toolCallData, selectedToolId, onToolSelect }) => {
  const measuredRef = useRef(null);
  const resizeObserver = useRef(null);

  useEffect(() => {
    if (!measuredRef.current) return;

    // Initialize ResizeObserver
    if (!resizeObserver.current) {
      resizeObserver.current = new ResizeObserver(entries => {
        if (entries && entries[0]) {
          const height = entries[0].contentRect.height;
          if (height > 0) {
            setSize(index, height);
          }
        }
      });

      resizeObserver.current.observe(measuredRef.current);
    }

    // Initial measurement
    const height = measuredRef.current.getBoundingClientRect().height;
    if (height > 0) {
      setSize(index, height);
    }

    return () => {
      if (resizeObserver.current) {
        resizeObserver.current.disconnect();
      }
    };
  }, [index, message, setSize]);

  // Import MessageItem dynamically to avoid circular dependencies
  const MessageItem = require('./MessageItem').default;

  return (
    <div ref={measuredRef} style={style}>
      <MessageItem
        message={message}
        toolCallData={toolCallData}
        selectedToolId={selectedToolId}
        onToolSelect={onToolSelect}
      />
    </div>
  );
});

export default React.memo(VirtualizedMessagesList);