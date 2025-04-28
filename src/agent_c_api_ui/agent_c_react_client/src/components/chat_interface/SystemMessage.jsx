import React from 'react';
import PropTypes from 'prop-types';
import { Card, CardContent } from "@/components/ui/card";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { cn } from "@/lib/utils";
import { InfoIcon, AlertTriangleIcon } from "lucide-react";
import CopyButton from './CopyButton';

/**
 * SystemMessage component displays system messages and errors in the chat interface
 * with consistent styling and copy functionality.
 * 
 * @param {Object} props - Component props
 * @param {string} props.content - The message content
 * @param {boolean} [props.isError=false] - Whether this is an error message
 * @param {boolean} [props.isCritical=false] - Whether this is a critical error
 * @param {string} [props.className] - Optional additional class names
 */
const SystemMessage = ({ content, isError = false, isCritical = false, className = '' }) => {
  // Use Alert for error messages, Card for regular messages
  const MessageComponent = isError ? Alert : Card;
  const ContentComponent = isError ? AlertDescription : CardContent;
  const IconComponent = isError ? AlertTriangleIcon : InfoIcon;
  
  return (
    <div 
      className={cn("flex justify-start gap-2 group system-message-container system-message-animation", className)}
      role="log"
      aria-label={isError ? "Error message" : "System message"}
    >
      <MessageComponent
        className={cn(
          isError 
            ? "max-w-[80%] rounded-2xl shadow-sm border-destructive/50 bg-destructive/10 text-destructive dark:bg-destructive/20" 
            : "system-message-normal"
        )}
        variant={isError ? "destructive" : "default"}
      >
        <ContentComponent className="flex justify-between items-start gap-4 p-4">
          <div className="flex items-start gap-2">
            <IconComponent 
              className={cn(
                "h-4 w-4 mt-1 flex-shrink-0", 
                isError ? "text-destructive" : "text-muted-foreground"
              )}
              aria-hidden="true"
            />
            <div 
              className="flex-1 prose dark:prose-invert"
              role="textbox"
              aria-readonly="true"
            >
              {isError ? "Error: " : ""}{content}
              {isCritical && (
                <div 
                  className="mt-1 text-xs font-medium text-destructive"
                  role="alert"
                >
                  This is a critical error that may require restarting the session.
                </div>
              )}
            </div>
          </div>
          <CopyButton
            content={content}
            tooltipText="Copy message"
            position="left"
            variant={isError ? "destructive" : "secondary"}
            size="xs"
            className="mt-1 flex-shrink-0 opacity-0 transition-opacity group-hover:opacity-100"
            aria-label="Copy message to clipboard"
          />
        </ContentComponent>
      </MessageComponent>
    </div>
  );
};

SystemMessage.propTypes = {
  content: PropTypes.string.isRequired,
  isError: PropTypes.bool,
  isCritical: PropTypes.bool,
  className: PropTypes.string
};

// Default props are now handled via parameter destructuring with default values

// Create a memoized version of SystemMessage to prevent unnecessary re-renders
const MemoizedSystemMessage = React.memo(SystemMessage, (prevProps, nextProps) => {
  // Only re-render if essential props have changed
  return (
    prevProps.content === nextProps.content &&
    prevProps.isError === nextProps.isError &&
    prevProps.isCritical === nextProps.isCritical
  );
});

export default MemoizedSystemMessage;