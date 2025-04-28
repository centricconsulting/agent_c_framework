import React from 'react';
import PropTypes from 'prop-types';
import UserMessage from './UserMessage';
import SystemMessage from './SystemMessage';
import AssistantMessage from './AssistantMessage';
import ThoughtDisplay from './ThoughtDisplay';
import MediaMessage from './MediaMessage';
import ToolCallDisplay from './ToolCallDisplay';
import { cn } from '../../lib/utils';

/**
 * MessageItem is a factory component that renders the appropriate message type
 * based on the message object properties.
 * 
 * @param {Object} props - Component props
 * @param {Object} props.message - The message object to render
 * @param {number} props.index - The index of this message in the messages array
 * @param {Array} props.messages - The complete array of messages (needed for context)
 * @param {Array} props.expandedToolCallMessages - Array of message indices with expanded tool calls
 * @param {function} props.toggleToolCallExpansion - Function to toggle tool call expansion
 */
const MessageItem = ({
  message,
  index,
  messages,
  expandedToolCallMessages,
  toggleToolCallExpansion
}) => {
  // Helper function to check if this message has associated tool calls (excluding 'think')
  const getAssociatedToolCalls = () => {
    const nextMsg = messages[index + 1];
    if (nextMsg && nextMsg.type === 'tool_calls' && nextMsg.toolCalls?.length > 0) {
      // Filter out 'think' tool calls
      return nextMsg.toolCalls.filter(
        tool => tool.name !== 'think' && tool.function?.name !== 'think'
      );
    }
    return [];
  };

  // Helper function to check if tool calls should be expanded
  const isToolCallsExpanded = () => expandedToolCallMessages.includes(index);

  // Render user message
  if (message.role === 'user') {
    return (
      <div role="listitem" className="message-item user-message-container overflow-hidden w-full" aria-label="User message">
        <UserMessage 
          content={message.content} 
          files={message.files} 
          isVoiceMessage={message.isVoiceMessage} 
        />
      </div>
    );
  }
  
  // Render assistant text message
  if (message.role === 'assistant' && message.type === 'content') {
    const toolCalls = getAssociatedToolCalls();
    const hasToolCalls = toolCalls.length > 0;
    
    return (
      <div 
        role="listitem" 
        className="message-item assistant-message-container overflow-hidden w-full"
        aria-label="Assistant message"
      >
        <AssistantMessage
          content={message.content}
          vendor={message.vendor}
          tokenUsage={message.tokenUsage}
          toolCalls={toolCalls}
          isToolCallsExpanded={isToolCallsExpanded()}
          onToggleToolCalls={() => toggleToolCallExpansion(index)}
        />
      </div>
    );
  }
  
  // Handle tool calls rendering
  if (message.type === 'tool_calls') {
    // Skip rendering if this is associated with a previous assistant message
    const prevMsg = messages[index - 1];
    if (prevMsg && prevMsg.role === 'assistant' && prevMsg.type === 'content') {
      return null;
    }
    
    // Filter out 'think' tool calls
    const displayableToolCalls = message.toolCalls?.filter(
      tool => tool.name !== 'think' && tool.function?.name !== 'think'
    );
    
    // If no displayable tool calls left, don't render anything
    if (!displayableToolCalls || displayableToolCalls.length === 0) {
      return null;
    }
    
    // Otherwise, render as standalone tool calls
    return (
      <div 
        role="listitem" 
        className="message-item tool-call-container"
        aria-label="Tool call results"
      >
        <ToolCallDisplay toolCalls={displayableToolCalls} />
      </div>
    );
  }
  
  // Render thinking messages
  if (message.role === 'assistant' && message.type === 'thinking') {
    return (
      <div 
        role="listitem" 
        className="message-item thinking-container"
        aria-label="Assistant thinking process"
      >
        <ThoughtDisplay content={message.content} vendor={message.vendor} />
      </div>
    );
  }
  
  // Render media messages
  if (message.type === 'media') {
    return (
      <div 
        role="listitem" 
        className="message-item media-container"
        aria-label="Media message"
      >
        <MediaMessage message={message} />
      </div>
    );
  }
  
  // Render system messages
  if (message.role === 'system') {
    return (
      <div 
        role="listitem" 
        className="message-item system-message-container"
        aria-label={message.type === 'error' ? 'System error message' : 'System message'}
      >
        <SystemMessage 
          content={message.content} 
          isError={message.type === 'error'} 
          isCritical={message.critical} 
        />
      </div>
    );
  }
  
  // Default fallback - render nothing
  return null;
};

// PropTypes validation for runtime type checking
MessageItem.propTypes = {
  message: PropTypes.shape({
    role: PropTypes.string,
    content: PropTypes.string,
    type: PropTypes.string,
    vendor: PropTypes.string,
    tokenUsage: PropTypes.object,
    files: PropTypes.array,
    isVoiceMessage: PropTypes.bool,
    toolCalls: PropTypes.array,
    critical: PropTypes.bool
  }).isRequired,
  index: PropTypes.number.isRequired,
  messages: PropTypes.array.isRequired,
  expandedToolCallMessages: PropTypes.array.isRequired,
  toggleToolCallExpansion: PropTypes.func.isRequired
};

// Create a memoized version of MessageItem to prevent unnecessary re-renders
const MemoizedMessageItem = React.memo(MessageItem, (prevProps, nextProps) => {
  // Only re-render if essential props have changed
  
  // Check if message has changed (shallow comparison of essential properties)
  const messageChanged = 
    prevProps.message.role !== nextProps.message.role ||
    prevProps.message.content !== nextProps.message.content ||
    prevProps.message.type !== nextProps.message.type ||
    prevProps.message.vendor !== nextProps.message.vendor;
  
  // Check if the tool call expansion state for this message has changed
  const wasExpanded = prevProps.expandedToolCallMessages.includes(prevProps.index);
  const isExpanded = nextProps.expandedToolCallMessages.includes(nextProps.index);
  const expansionStateChanged = wasExpanded !== isExpanded;
  
  // Check if files or tool calls have changed
  const filesChanged = prevProps.message.files?.length !== nextProps.message.files?.length;
  const toolCallsChanged = prevProps.message.toolCalls?.length !== nextProps.message.toolCalls?.length;
  
  // Check if next message (which might contain tool calls) has changed
  const prevNextMsg = prevProps.messages[prevProps.index + 1];
  const nextNextMsg = nextProps.messages[nextProps.index + 1];
  const nextMsgChanged = 
    (prevNextMsg?.type !== nextNextMsg?.type) || 
    (prevNextMsg?.toolCalls?.length !== nextNextMsg?.toolCalls?.length);
  
  // Return true if nothing has changed (to prevent re-render)
  return !(
    messageChanged || 
    expansionStateChanged || 
    filesChanged || 
    toolCallsChanged || 
    nextMsgChanged
  );
});

export default MemoizedMessageItem;