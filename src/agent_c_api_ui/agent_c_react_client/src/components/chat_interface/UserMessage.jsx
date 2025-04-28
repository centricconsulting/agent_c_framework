import React from 'react';
import PropTypes from 'prop-types';
import { User, Mic, FileIcon } from 'lucide-react';
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";
import CopyButton from './CopyButton';
import MarkdownMessage from './MarkdownMessage';

/**
 * UserMessage component displays messages sent by the user in the chat interface
 * with consistent styling and copy functionality.
 * 
 * @param {Object} props - Component props
 * @param {string} props.content - The message content
 * @param {Array} [props.files] - Optional array of file names attached to the message
 * @param {boolean} [props.isVoiceMessage] - Whether this is a voice message
 * @param {string} [props.className] - Optional additional class names
 */
const UserMessage = ({ content, files = [], isVoiceMessage = false, className = '' }) => {
  return (
    <div className={cn(
      "user-message-container group", 
      className
    )}>
      <Card className={cn(
        "user-message-content"
      )}>
        <CardContent className="p-0 flex justify-between items-start gap-2">
          <div className="flex-1 prose dark:prose-invert">
            {isVoiceMessage ? (
              <div className="user-message-voice flex items-center gap-2" role="status" aria-label="Voice message from user">
                <Mic className="h-4 w-4" aria-hidden="true" />
                <span>Voice message</span>
              </div>
            ) : (
              <div className="markdown-user-message" role="textbox" aria-readonly="true">
                <MarkdownMessage content={content} />
              </div>
            )}
            
            {/* Display file attachments if any */}
            {files && files.length > 0 && (
              <div className="user-message-files" role="region" aria-label="Attached files">
                <Separator className="my-2" />
                <div className="flex flex-wrap gap-1.5 items-center">
                  <FileIcon className="h-3.5 w-3.5" aria-hidden="true" />
                  {files.map((file, index) => (
                    <Badge key={index} variant="secondary" className="text-xs">
                      {file}
                    </Badge>
                  ))}
                </div>
              </div>
            )}
          </div>
          
          {/* Copy button with tooltip */}
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <div className="user-message-copy-container mt-1 flex-shrink-0">
                  <CopyButton
                    content={content}
                    position="left"
                    variant="secondary"
                    size="xs"
                    className="user-message-copy-button opacity-0 group-hover:opacity-100 transition-opacity duration-200"
                    aria-label="Copy message content"
                  />
                </div>
              </TooltipTrigger>
              <TooltipContent side="left" className="user-message-tooltip">
                Copy message
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </CardContent>
      </Card>
      <Avatar className="h-6 w-6 bg-transparent" aria-label="User">
        <AvatarFallback className="bg-transparent">
          <User className="h-4 w-4 text-primary" aria-hidden="true" />
        </AvatarFallback>
      </Avatar>
    </div>
  );
};

UserMessage.propTypes = {
  content: PropTypes.string.isRequired,
  files: PropTypes.arrayOf(PropTypes.string),
  isVoiceMessage: PropTypes.bool,
  className: PropTypes.string
};

// Default props are now handled via parameter destructuring with default values

// Create a memoized version of UserMessage to prevent unnecessary re-renders
const MemoizedUserMessage = React.memo(UserMessage, (prevProps, nextProps) => {
  // Check message content
  if (prevProps.content !== nextProps.content) return false;
  
  // Check if voice message status changed
  if (prevProps.isVoiceMessage !== nextProps.isVoiceMessage) return false;
  
  // Check files
  const prevFiles = prevProps.files || [];
  const nextFiles = nextProps.files || [];
  
  if (prevFiles.length !== nextFiles.length) return false;
  
  // Check if file names have changed
  for (let i = 0; i < prevFiles.length; i++) {
    if (prevFiles[i] !== nextFiles[i]) return false;
  }
  
  // If we got here, nothing important changed, so prevent re-render
  return true;
});

export default MemoizedUserMessage;