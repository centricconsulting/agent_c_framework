import React from 'react';
import PropTypes from 'prop-types';
import { Wrench } from 'lucide-react';
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { Separator } from "@/components/ui/separator";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { cn } from "@/lib/utils";
import MarkdownMessage from './MarkdownMessage';
import TokenUsageDisplay from './TokenUsageDisplay';
import ModelIcon from './ModelIcon';
import CopyButton from './CopyButton';
import ToolCallItem from './ToolCallItem';

/**
 * AssistantMessage component displays messages from the AI assistant with
 * formatting, token usage, and expandable tool calls.
 * 
 * @param {Object} props - Component props
 * @param {string} props.content - The message content
 * @param {string} [props.vendor] - The AI model vendor
 * @param {Object} [props.tokenUsage] - Token usage information
 * @param {Array} [props.toolCalls] - Tool calls associated with this message
 * @param {boolean} [props.isToolCallsExpanded=false] - Whether tool calls are expanded
 * @param {function} props.onToggleToolCalls - Function to toggle tool call expansion
 * @param {string} [props.className] - Optional additional class names
 */
const AssistantMessage = ({
  content,
  vendor,
  tokenUsage,
  toolCalls,
  isToolCallsExpanded = false,
  onToggleToolCalls,
  className
}) => {
  // Filter out 'think' tool calls
  const displayableToolCalls = toolCalls?.filter(
    tool => tool.name !== 'think' && tool.function?.name !== 'think'
  ) || [];
  
  const hasToolCalls = displayableToolCalls.length > 0;
  const toolCallCount = displayableToolCalls.length;
  
  return (
    <div className={cn("assistant-message-wrapper", className)}>
      <div className="assistant-message-container">
        <ModelIcon vendor={vendor} className="assistant-message-avatar" />
        
        <Card className="assistant-message-card">
          <CardContent className="assistant-message-content">
            <div className="assistant-message-body">
              <div className="assistant-message-main-content">
                <MarkdownMessage content={content} />
              </div>
              
              {/* Footer with token usage and tool call info */}
              <div className="assistant-message-footer">
                {tokenUsage && <TokenUsageDisplay usage={tokenUsage} />}
                
                {/* Tool call toggle button - only show if there are tool calls */}
                {hasToolCalls && (
                  <Button
                    variant="outline"
                    size="sm"
                    className="assistant-message-tool-calls-button"
                    onClick={onToggleToolCalls}
                    aria-label={`${isToolCallsExpanded ? 'Hide' : 'Show'} ${toolCallCount} tool call${toolCallCount !== 1 ? 's' : ''}`}
                    aria-expanded={isToolCallsExpanded}
                    aria-controls="assistant-message-tool-calls-content"
                  >
                    <Wrench className="h-3 w-3" />
                    <span>Tool Calls {toolCallCount}</span>
                    <span className="assistant-message-tool-calls-arrow">
                      {isToolCallsExpanded ? "▲" : "▼"}
                    </span>
                  </Button>
                )}
              </div>
            </div>
            
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <div className="assistant-message-actions">
                    <CopyButton
                      content={content}
                      variant="secondary"
                      size="xs"
                      aria-label="Copy message content"
                    />
                  </div>
                </TooltipTrigger>
                <TooltipContent side="left">
                  <p>Copy message</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          </CardContent>
        </Card>
      </div>
      
      {/* Tool call content displayed outside and below the message bubble */}
      {hasToolCalls && isToolCallsExpanded && (
        <div 
          id="assistant-message-tool-calls-content"
          className="assistant-message-tool-calls-container"
        >
          <div className="assistant-message-tool-calls-content">
            <div className="assistant-message-tool-calls-list">
              {displayableToolCalls.map((toolCall, toolIdx) => (
                <ToolCallItem
                  key={toolCall.id || toolIdx}
                  tool={{
                    name: toolCall.name || toolCall.function?.name,
                    arguments: toolCall.arguments || toolCall.function?.arguments,
                    id: toolCall.id || toolCall.tool_call_id
                  }}
                  results={toolCall.results}
                  integrated={false}
                />
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// PropTypes validation for runtime type checking
AssistantMessage.propTypes = {
  content: PropTypes.string.isRequired,
  vendor: PropTypes.string,
  tokenUsage: PropTypes.object,
  toolCalls: PropTypes.array,
  isToolCallsExpanded: PropTypes.bool,
  onToggleToolCalls: PropTypes.func.isRequired,
  className: PropTypes.string
};

// Create a memoized version of AssistantMessage to prevent unnecessary re-renders
const MemoizedAssistantMessage = React.memo(AssistantMessage, (prevProps, nextProps) => {
  // Only re-render if essential props have changed
  
  // Check message content
  if (prevProps.content !== nextProps.content) return false;
  
  // Check vendor (affects model icon)
  if (prevProps.vendor !== nextProps.vendor) return false;
  
  // Check expansion state
  if (prevProps.isToolCallsExpanded !== nextProps.isToolCallsExpanded) return false;
  
  // Check token usage (shallow comparison is sufficient)
  const prevTokens = prevProps.tokenUsage;
  const nextTokens = nextProps.tokenUsage;
  if (
    (prevTokens && !nextTokens) || 
    (!prevTokens && nextTokens) ||
    (prevTokens && nextTokens && (
      prevTokens.total !== nextTokens.total ||
      prevTokens.completion !== nextTokens.completion ||
      prevTokens.prompt !== nextTokens.prompt
    ))
  ) return false;
  
  // Deep check tool calls
  const prevTools = prevProps.toolCalls || [];
  const nextTools = nextProps.toolCalls || [];
  
  if (prevTools.length !== nextTools.length) return false;
  
  // If there are tool calls, check if any have changed
  for (let i = 0; i < prevTools.length; i++) {
    const prevTool = prevTools[i];
    const nextTool = nextTools[i];
    
    if (
      prevTool.name !== nextTool.name || 
      prevTool.id !== nextTool.id || 
      prevTool.function?.name !== nextTool.function?.name ||
      JSON.stringify(prevTool.arguments || prevTool.function?.arguments) !== 
        JSON.stringify(nextTool.arguments || nextTool.function?.arguments) ||
      JSON.stringify(prevTool.results) !== JSON.stringify(nextTool.results)
    ) {
      return false;
    }
  }
  
  // If we got here, nothing important changed, so prevent re-render
  return true;
});

export default MemoizedAssistantMessage;